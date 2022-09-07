// ========================================================================
// Copyright © 2021 PGE 
// <history>
//  Auto Relate Conductor to Condute
// YXA6 09/28/2021	Created
// JeeraID-> EDGISRearch-1364
// </history>
// All rights reserved.
// ========================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;
using Miner.Geodatabase;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.GDBM;

namespace PGE.Desktop.EDER
{

    [ComVisible(true)]
    [Guid("4cfa82a4-6754-4e09-b67f-f54a488649ca")]
    [ClassInterface(ClassInterfaceType.None)]
    //[ProgId("PGE_AutoRelateConductorAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGE_AutoRelateConductorAU : BaseSpecialAU
    {
        public static bool bCancelOperation;
        public static bool bAutoRelate;
        public static bool bBuffer;
        #region Private Variable     
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private DataTable featureDetailsTable = new DataTable();
        CommonFunctions ComnFunctns = new CommonFunctions();
        #endregion

        #region constructor
        public PGE_AutoRelateConductorAU()
            : base("PGE Auto Relate ConduitConductor AU")
        {

        }
        #endregion

        #region IMMSpecialAUStrategyEX members
        /// <summary>
        /// Only allow execution if the Condition is true
        /// </summary>
        /// <param name="eAUMode"></param>
        /// <returns></returns>
        protected override bool CanExecute(Miner.Interop.mmAutoUpdaterMode eAUMode)
        {
            return true;
        }
        /// <summary>
        /// Return enabled if it is on feature create and it contains the Conduit FeatureClass ModelName
        /// </summary>
        /// <param name="objectClass"></param>
        /// <param name="eEvent"></param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass objectClass, Miner.Interop.mmEditEvent eEvent)
        {
            if (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate)
            {
                //if (ModelNameManager.ContainsClassModelName(objectClass, SchemaInfo.General.ClassModelNames.PGE_CONDUIT))
                //{
                   return true;
                //}

            }
            else 
            {
                return false;
            }

           


        }
        /// <summary>
        /// This AU is soley to provide the necessary information to the PGE_Asset_Replacement D8 tree tool to be able to
        /// related the two features to each other.  No actual editing is done with this AU.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="eAUMode"></param>
        /// <param name="eEvent"></param>
        protected override void InternalExecute(IObject obj, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            if (eEvent == mmEditEvent.mmEventFeatureSplit) { return; }
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap && (eEvent == mmEditEvent.mmEventFeatureCreate))
            {

                ExecuteAU(obj);
            }
        }
        /// <summary>
        /// Method to Execute Auto Relate functionlity
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="eAUMode"></param>
        /// <param name="eEvent"></param>
        public void ExecuteAU(IObject PConductorObject)
        {

            try
            {

                // Added to restrict execution of Auto relation if job number is not entered
                Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
                object o = Activator.CreateInstance(t);
                IApplication application = o as IApplication;
                IMxDocument mxDocument = application.Document as IMxDocument;
                IDataset pDataset = (IDataset)PConductorObject.Class;
                IObjectClass objClass = (IObjectClass)PConductorObject.Class;
                IWorkspace pWorkspace = pDataset.Workspace;
                // find conduit featureclass
                IEnumFeatureClass  pEnumfeatureclass = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(pWorkspace, SchemaInfo.General.ClassModelNames.PGE_CONDUIT);
                IFeatureClass Conduitclasses = pEnumfeatureclass.Next();
                while (Conduitclasses != null)
                {
                    //Auto relate the conductor to conduite
                    AutorelateConductor(application, PConductorObject as IFeature, Conduitclasses, 0, pWorkspace);
                    Conduitclasses = pEnumfeatureclass.Next();
                }
               
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    ex.Message,
                    this.GetType() + "Auto Relate AU",
                    System.Windows.Forms.MessageBoxButtons.OK);
                bAutoRelate = false;
            }
            finally { bAutoRelate = false; }
        }
        #region Commented Code
        ///// <summary>
        ///// Method to Execute Auto Relate functionlity
        ///// </summary>
        ///// <param name="pObject"></param>
        ///// <param name="eAUMode"></param>
        ///// <param name="eEvent"></param>
        //public void ExecuteAU_old(IObject pObject)
        //{

        //    try
        //    {

        //        // Added to restrict execution of Auto relation if job number is not entered
        //        Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
        //        object o = Activator.CreateInstance(t);
        //        IApplication application = o as IApplication;
        //        IMxDocument mxDocument = application.Document as IMxDocument;
        //        IDataset pDataset = (IDataset)pObject.Class;
        //        IObjectClass objClass = (IObjectClass)pObject.Class;
        //        IWorkspace pWorkspace = pDataset.Workspace;
        //        int buffer = 0;
        //        ADODB.Recordset pRSet = ComnFunctns.GetRecordset("Select value from PGEDATA.PGE_EDERCONFIG where KEY='BUFFER'", pWorkspace);
        //        if (pRSet != null)
        //        {
        //            if ((pRSet.RecordCount > 0))
        //            {
        //                pRSet.MoveFirst();
        //                buffer = Convert.ToInt32(pRSet.Fields[0].Value.ToString());
        //            }
        //        }
        //        IFeatureClass TargetFeatureclasses = null;
        //        if (ModelNameFacade.ModelNameManager.ContainsClassModelName(objClass, SchemaInfo.General.ClassModelNames.PGE_CONDUIT))
        //        {

        //            IEnumFeatureClass pEnumfeatureclass = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(pWorkspace, "PGE_SECONDARYUNDERGROUND");
        //            TargetFeatureclasses = pEnumfeatureclass.Next();
        //            while (TargetFeatureclasses != null)
        //            {
        //                AutorelateConductor(application, pObject as IFeature, TargetFeatureclasses, buffer, pWorkspace);
        //                TargetFeatureclasses = pEnumfeatureclass.Next();
        //            }

        //            pEnumfeatureclass = ModelNameFacade.ModelNameManager.FeatureClassesFromModelNameWS(pWorkspace, "PGE_PRIMARYUGCONDUCTOR");
        //            TargetFeatureclasses = pEnumfeatureclass.Next();
        //            while (TargetFeatureclasses != null)
        //            {
        //                AutorelateConductor(application, pObject as IFeature, TargetFeatureclasses, buffer, pWorkspace);
        //                TargetFeatureclasses = pEnumfeatureclass.Next();
        //            }

        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.Forms.MessageBox.Show(
        //            ex.Message,
        //            this.GetType() + "Auto Relate AU",
        //            System.Windows.Forms.MessageBoxButtons.OK);
        //        bAutoRelate = false;
        //    }
        //    finally { bAutoRelate = false; }
        //}
        #endregion
        /// <summary>
        /// Auto Relate Conductor
        /// </summary>
        /// <param name="application"></param>
        /// <param name="PConductorObject"></param>
        /// <param name="Conduitclasses"></param>
        /// <param name="buffer"></param>
        /// <param name="pWorkspace"></param>
        private void AutorelateConductor(IApplication application, IFeature PConductorObject, IFeatureClass Conduitclasses, int buffer, IWorkspace pWorkspace)
        {
            try
            {
                Dictionary<long, IFeature> ConduteList = new Dictionary<long, IFeature>();
                // Find overlapped Conduit List
                FillSearchConduteList(PConductorObject, buffer, Conduitclasses, out ConduteList);
                foreach (object skey in ConduteList.Keys)
                {
                    IFeature ConduitFeature = null;
                    ConduteList.TryGetValue(Convert.ToInt32(skey), out ConduitFeature);
                    // Find Relationship Class
                    IRelationshipClass pRelationshipclass = ComnFunctns.GetRelationshipClassbyFeatureClassAliasName(PConductorObject as IObject, ConduitFeature.Class.AliasName);
                    // Find if any feature already related to selected conduit feature
                    ISet relatedFeatures = pRelationshipclass.GetObjectsRelatedToObject(ConduitFeature as IObject);
                    if (relatedFeatures.Count == 0)
                    {
                        //if not then Create relationship
                        
                        pRelationshipclass.CreateRelationship(PConductorObject as IObject, ConduitFeature as IObject);

                    }
                    //else
                    //{                      
                    //    // if Yes then Delete relationship 
                    //    pRelationshipclass.DeleteRelationship(PConductorObject, ConduitFeature);
                    //    //Create relationship
                    //    pRelationshipclass.CreateRelationship(PConductorObject as IObject, ConduitFeature as IObject);

                    //}


                }
            }
            catch (Exception ex)
            { throw ex; }
        }

        private void FillSearchConduteList(IFeature pConductorFeature, double sbuffer, IFeatureClass TargetFeatureClass, out Dictionary<long, IFeature> SearchCondute)
        {
            SearchCondute = new Dictionary<long, IFeature>();
            IFeatureCursor fcursor = null;
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            try
            {
                

                // create a spatial query filter
                spatialFilter.GeometryField = TargetFeatureClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                // specify the geometry to query with. apply a buffer if desired
                if (sbuffer > 0.0)
                {
                    // Use the ITopologicalOperator interface to create a buffer.
                    ITopologicalOperator topoOperator = (ITopologicalOperator)pConductorFeature.ShapeCopy;
                    IGeometry buffer = topoOperator.Buffer(sbuffer);
                    spatialFilter.Geometry = buffer;
                }
                else
                    spatialFilter.Geometry = pConductorFeature.ShapeCopy;

                fcursor = TargetFeatureClass.Search(spatialFilter, true);
                IFeature feature = fcursor.NextFeature();

                while (feature != null)
                {
                    // Find Search Conduit Feature 
                    if (SearchCondute.ContainsKey(feature.OID) == false)
                    {
                        // find whether it is overlapped or not, if yes then add into the conduitlist
                        IPointCollection pconduitpointcollection = feature.ShapeCopy as IPointCollection;
                        IPointCollection pconductorpointcollection = pConductorFeature.ShapeCopy as IPointCollection;
                        int MatchPoint = 0;
                        for (int i = 0; i < pconductorpointcollection.PointCount; i++)
                        {
                            for(int j=0;j<pconduitpointcollection.PointCount;j++)
                            {
                                if ((Math.Round(pconductorpointcollection.Point[i].X, 4) == Math.Round(pconduitpointcollection.Point[j].X, 4)) &&
                                    (Math.Round(pconductorpointcollection.Point[i].Y, 4) == Math.Round(pconduitpointcollection.Point[j].Y, 4)))
                                {
                                    MatchPoint = MatchPoint + 1;
                                    break;
                                }
                            }
                            if (MatchPoint > 2)
                            {
                                if (!SearchCondute.ContainsKey(feature.OID))
                                {
                                    SearchCondute.Add(feature.OID, feature);
                                    break;
                                }
                            }
                        }

                    }
                    feature = fcursor.NextFeature();
                }
            }



            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (fcursor != null) { Marshal.ReleaseComObject(fcursor); }
            }


        }

        #endregion

        #region Private method

        /// <summary>
        /// This Function evaluates whether or not the shape of the object has been changed.
        /// This is only applicable to features.
        /// </summary>
        /// <param name="obj">Object to check for shape changes.</param>
        /// <returns>True if shape was changed, false otherwise.</returns>
        private bool ShapeChanged(IObject obj)
        {
            IFeatureChanges featureChanges = obj as IFeatureChanges;
            return featureChanges != null && featureChanges.ShapeChanged;

        }
        #endregion


    }
}











