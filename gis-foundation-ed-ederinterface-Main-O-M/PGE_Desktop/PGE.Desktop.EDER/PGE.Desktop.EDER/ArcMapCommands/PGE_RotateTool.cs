using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using Miner.ComCategories;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using System.Drawing;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Controls;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    [Guid("60f1e57b-4c39-4a42-ba89-462f47fdfc06")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_RotateTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public class PGE_RotateTool : ITool, ICommand
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
        }

        #endregion

        public static bool RotateInProgress = false;
        ITool rotateTool = null;
        ICommand rotateCommand = null;
        ICommandItem rotateCommandItem = null;
        Bitmap bitmap;
        private IntPtr m_hBitmap;

        IApplication _app;
        IMxApplication _mxapp;
        IMxDocument pMxDoc;
        IMap pMap;
        IActiveView pAV;

        Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;
        List<clsRelationshipInfo> lstFeaturesRelationships = null;

        private static bool askedUser = false;
        private static bool rotateAnno = false;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");



        public PGE_RotateTool()
        {
            UID uid = new UIDClass();
            uid.Value = "esriEditor.EditorRotateTool";
            object rotateObj = Activator.CreateInstance(Type.GetTypeFromProgID("esriEditor.EditorRotateTool"));
            rotateCommand = rotateObj as ICommand;
            rotateTool = rotateObj as ITool;

            string bitmapResourceName = GetType().Name + ".bmp";
            bitmap = new Bitmap(GetType(), bitmapResourceName);
            m_hBitmap = bitmap.GetHbitmap();

            _app = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            _mxapp = _app as IMxApplication;
            pMxDoc = _app.Document as IMxDocument;
            pMap = pMxDoc.FocusMap as IMap;
            pAV = pMxDoc.ActiveView;

        }

        #region ITool implementation

        public int Cursor
        {
            get { return rotateTool.Cursor; }
        }

        public bool Deactivate()
        {
            return rotateTool.Deactivate();
        }

        public bool OnContextMenu(int x, int y)
        {
            return rotateTool.OnContextMenu(x, y);
        }

        public void OnDblClick()
        {
            rotateTool.OnDblClick();
        }

        public void OnKeyDown(int keyCode, int shift)
        {
            rotateTool.OnKeyDown(keyCode, shift);
        }

        public void OnKeyUp(int keyCode, int shift)
        {
            rotateTool.OnKeyUp(keyCode, shift);
        }

        public void OnMouseDown(int button, int shift, int x, int y)
        {
            IFeature feat = null;
            clsRelationshipInfo _objRelationshipInfo = null;
            try
            {
                if (rotateAnno == false)
                {
                    IEnumFeature enumFeat = pMap.FeatureSelection as IEnumFeature;
                    if (enumFeat != null)
                    {
                        lstFeaturesRelationships = new List<clsRelationshipInfo>();
                        while ((feat = enumFeat.Next()) != null)
                        {
                            _objRelationshipInfo = new clsRelationshipInfo();
                            relationshipMapping = GetRelationshipMapping(feat);
                            _objRelationshipInfo.relationshipMapping = relationshipMapping;
                            _objRelationshipInfo.intObjectOID = feat.OID;
                            lstFeaturesRelationships.Add(_objRelationshipInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error RotateTool On Mouse Down: " + ex.Message);
            }
            rotateTool.OnMouseDown(button, shift, x, y);
        }

        public void OnMouseMove(int button, int shift, int x, int y)
        {
            rotateTool.OnMouseMove(button, shift, x, y);
        }

        public void OnMouseUp(int button, int shift, int x, int y)
        {
            try
            {
                //RotateInProgress = true;
                rotateTool.OnMouseUp(button, shift, x, y);
            }
            finally
            {
                if (lstFeaturesRelationships != null && lstFeaturesRelationships.Count > 0)
                {
                    SetFeatureAnnotationRelationship();
                    //pMap.ClearSelection();
                    //pAV.Refresh();
                }
                //PGEPreserveAnnoAngle.SetAnnoAngle = false;
                //pMap.ClearSelection();
                pAV.Refresh();
                //RotateInProgress = false;
            }
        }

        public void Refresh(int hdc)
        {
            rotateTool.Refresh(hdc);
        }

        #endregion

        #region ICommand Implementation

        public int Bitmap
        {
            get
            {
                return m_hBitmap.ToInt32();
            }
        }

        public string Caption
        {
            get { return "PGE Rotate Tool"; }
        }

        public string Category
        {
            get { return "PGE Tools"; }
        }

        public bool Checked
        {
            get { return rotateCommand.Checked; }
        }

        public bool Enabled
        {
            get { return rotateCommand.Enabled; }
        }

        public int HelpContextID
        {
            get { return rotateCommand.HelpContextID; }
        }

        public string HelpFile
        {
            get { return rotateCommand.HelpFile; }
        }

        public string Message
        {
            get { return "PGE Rotate Tool"; }
        }

        public string Name
        {
            get { return "PGE Rotate Tool"; }
        }


        public void OnClick()
        {
            rotateAnno = false;
            askedUser = false;
            try
            {
                if (pMap.SelectionCount > 0)
                {
                    bool isAllFeatAreAnno = CheckIfAllSelectedFeaturesAreAnno();
                    if (isAllFeatAreAnno == false)
                    {
                        bool hasSelectedFeaturesAnnotations = CheckIfSelectedFeatureHasRelatedAnnotations();
                        if (hasSelectedFeaturesAnnotations == true & askedUser == false)
                        {
                            DialogResult result = MessageBox.Show("Rotate annotation features as well?", "Rotate Annotation", MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                rotateAnno = true;
                                lstFeaturesRelationships = null;
                            }
                            askedUser = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error PGE Rotate Tool On Click: " + ex.Message);
            }
            rotateCommand.OnClick();
        }

        public void OnCreate(object Hook)
        {
            rotateCommand.OnCreate(Hook);
        }

        public string Tooltip
        {
            get { return "PGE Rotate Tool"; } //return rotateCommand.Tooltip; }
        }

        #endregion

        /// <summary>
        /// Function sets annotaion relationship after rotatios is done if user do not want to rotate annotations
        /// </summary>
        private void SetFeatureAnnotationRelationship()
        {
            IFeature feat = null;
            try
            {
                if (lstFeaturesRelationships != null && lstFeaturesRelationships.Count > 0)
                {
                    IEnumFeature enumFeat = pMap.FeatureSelection as IEnumFeature;
                    if (enumFeat != null)
                    {
                        //lstFeaturesRelationships = new List<clsRelationshipInfo>();
                        while ((feat = enumFeat.Next()) != null)
                        {
                            for (int iCnt = 0; iCnt < lstFeaturesRelationships.Count; iCnt++)
                            {
                                if (feat.OID == lstFeaturesRelationships[iCnt].intObjectOID)
                                {
                                    SetRelationships(lstFeaturesRelationships[iCnt].relationshipMapping, feat);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error PGE Rotate Tool,SetFeatureAnnotationRelationship: " + ex.Message);
            }
        }

        private bool CheckIfAllSelectedFeaturesAreAnno()
        {
            IFeature feat;
            bool isAllSelFeatAreAnno = true;
            try
            {
                 IEnumFeature enumFeat = pMap.FeatureSelection as IEnumFeature;
                 if (enumFeat != null)
                 {
                     while ((feat = enumFeat.Next()) != null)
                     {
                         if (feat.FeatureType != esriFeatureType.esriFTAnnotation)
                             return false;
                     }
                 }
            }
            catch (Exception ex)
            {
                _logger.Error("Error PGE Rotate Tool,CheckIfSelectedFeatureHasRelatedAnnotations: " + ex.Message);
            }
            return isAllSelFeatAreAnno;
        }
        /// <summary>
        /// Function to check if selected feature has related annotations
        /// </summary>
        /// <returns></returns>
        private bool CheckIfSelectedFeatureHasRelatedAnnotations()
        {
            IFeature feat;
            bool hasRelatedAnno = false;
            try
            {
                IEnumFeature enumFeat = pMap.FeatureSelection as IEnumFeature;
                if (enumFeat != null)
                {
                    //lstFeaturesRelationships = new List<clsRelationshipInfo>();
                    while ((feat = enumFeat.Next()) != null)
                    {
                        IEnumRelationshipClass pRelClasses = feat.Class.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                        IRelationshipClass pRelClass = pRelClasses.Next();
                        while (pRelClass != null)
                        {
                            if (pRelClass.DestinationClass is IFeatureClass)
                            {
                                if (((IFeatureClass)pRelClass.DestinationClass).FeatureType ==
                                    esriFeatureType.esriFTAnnotation)
                                {
                                    hasRelatedAnno = true;
                                    return hasRelatedAnno;
                                }
                            }
                            pRelClass = pRelClasses.Next();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error PGE Rotate Tool,CheckIfSelectedFeatureHasRelatedAnnotations: " + ex.Message);
            }
            return hasRelatedAnno;
        }

        /// <summary>
        /// This method will take all relationships that are current assigned to the oldFeature, remove
        /// them and then apply them to the new feature.

        /// </summary>
        /// <param name="oldObject">Feature being replaced</param>
        private Dictionary<IRelationshipClass, List<IObject>> GetRelationshipMapping(IObject oldObject) //IFeature oldFeature
        {
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = new Dictionary<IRelationshipClass, List<IObject>>();
            //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
            IObjectClass objClass = oldObject.Class as IObjectClass;

            //Process relationships where this class is the destination

            //IEnumRelationshipClass relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            //relClasses.Reset();
            //IRelationshipClass relClass = relClasses.Next();
            //while (relClass != null)
            //{
            //    //Don't map annotation relationships
            //    if (relClass.OriginClass is IFeatureClass)
            //    {
            //        IFeatureClass originClass = relClass.OriginClass as IFeatureClass;
            //        if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
            //        {
            //            relClass = relClasses.Next();
            //            continue;
            //        }
            //    }
            //    if (!relationshipMapping.ContainsKey(relClass))
            //    {
            //        relationshipMapping.Add(relClass, new List<IObject>());
            //    }
            //    ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
            //    relatedFeatures.Reset();
            //    for (int i = 0; i < relatedFeatures.Count; i++)
            //    {
            //        IObject obj = relatedFeatures.Next() as IObject;
            //        relationshipMapping[relClass].Add(obj);
            //        relClass.DeleteRelationship(obj, oldObject);
            //        //relClass.CreateRelationship(obj, newFeature);
            //    }
            //    relClass = relClasses.Next();
            //}


            //Process relationships where this class is the origin
            IEnumRelationshipClass relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
            relClasses.Reset();
            IRelationshipClass relClass = relClasses.Next();
            while (relClass != null)
            {
                //Don't map annotation relationships
                if (relClass.DestinationClass is IFeatureClass)
                {
                    IFeatureClass originClass = relClass.DestinationClass as IFeatureClass;

                    //Simon Change remove this (we want to include the anno) 
                    if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        System.Diagnostics.Debug.Print(originClass.AliasName);



                        if (!relationshipMapping.ContainsKey(relClass))
                        {
                            relationshipMapping.Add(relClass, new List<IObject>());
                        }
                        ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
                        relatedFeatures.Reset();
                        for (int i = 0; i < relatedFeatures.Count; i++)
                        {
                            IObject obj = relatedFeatures.Next() as IObject;
                            relationshipMapping[relClass].Add(obj);
                            relClass.DeleteRelationship(obj, oldObject);
                            System.Diagnostics.Debug.Print("Deleting rel to OId: " + obj.OID.ToString());
                            //relClass.CreateRelationship(obj, newFeature);
                        }
                        //relClass = relClasses.Next();
                    }
                }
                relClass = relClasses.Next();
            }


            return relationshipMapping;
        }

        /// <summary>
        /// Relates all features that were related with the previous feature to the newly created feature
        /// </summary>
        /// <param name="relationshipMapping"></param>
        /// <param name="newObject"></param>
        private void SetRelationships(Dictionary<IRelationshipClass, List<IObject>> relationshipMapping, IObject newObject) //, IFeature newFeature
        {

            //First delete any new anno that has been created for the new object 
            //since we are going to use the old anno 
            //IEnumRelationshipClass pRelClasses = newObject.Class.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
            //IRelationshipClass pRelClass = pRelClasses.Next();
            //while (pRelClass != null)
            //{
            //    if (pRelClass.DestinationClass is IFeatureClass)
            //    {
            //        if (((IFeatureClass)pRelClass.DestinationClass).FeatureType ==
            //            esriFeatureType.esriFTAnnotation)
            //        {
            //            ISet pSet = pRelClass.GetObjectsRelatedToObject(newObject);
            //            IObject pRelObj = (IObject)pSet.Next();
            //            while (pRelObj != null)
            //            {
            //                pRelObj.Delete();
            //                pRelObj = (IObject)pSet.Next();
            //            }
            //        }
            //    }
            //    pRelClass = pRelClasses.Next();
            //}


            foreach (KeyValuePair<IRelationshipClass, List<IObject>> kvp in relationshipMapping)
            {
                bool isOrigin = false;
                if (kvp.Key.OriginClass == newObject.Class)
                {
                    isOrigin = true;
                }

                if (isOrigin)
                {
                    foreach (IObject obj in kvp.Value)
                    {
                        if (obj != null)
                        {
                            kvp.Key.CreateRelationship(newObject, obj);
                        }
                    }
                }
                else
                {
                    foreach (IObject obj in kvp.Value)
                    {
                        if (obj != null)
                        {
                            kvp.Key.CreateRelationship(obj, newObject);
                        }
                    }
                }
            }
        }
    }

    class clsRelationshipInfo
    {
        public Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;
        public Int64 intObjectOID;
    }
}
