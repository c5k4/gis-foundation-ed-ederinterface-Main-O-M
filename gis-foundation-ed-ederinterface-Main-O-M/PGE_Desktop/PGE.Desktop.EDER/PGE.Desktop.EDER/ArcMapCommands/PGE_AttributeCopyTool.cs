// ========================================================================
// Copyright © 2021 PGE.
// <history>
// PGE Attribute Copy Tool
// TCS V3SF (EDGISREARC-1363)              09/11/2021               Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using System.Collections.Generic;
using System.Reflection;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using Miner.Interop;
using ESRI.ArcGIS.Editor;
using Miner.Geodatabase;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    [Guid("50D554E8-D4BA-4A02-81C2-75C745C8A703")]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_AttributeCopyTool")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComVisible(true)]
    public sealed class PGE_AttributeCopyTool : BaseTool
    {
        #region Private Members
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string m_ToolName = "PGE Attribute Copy Tool";
        IEditor _editor = null;
        private IMap _map = null;
        private IStatusBar statusBar = default;
        private IApplication m_application = default;
        #endregion

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion

        #endregion

        #region Constructor

        public PGE_AttributeCopyTool()
        {

            try
            {
                base.m_cursor = Cursors.Cross;
                base.m_category = "PGE Tools";
                base.m_caption = m_ToolName;  //localizable text 
                base.m_message = m_ToolName;  //localizable text
                base.m_toolTip = m_ToolName;  //localizable text
                base.m_name = "PGE_AttributeCopyTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")

                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
                _logger.Error("PG&E " + m_ToolName + ", Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " at " + ex.StackTrace, ex);
            }
        }

        #endregion


        #region Overridden Class Methods

        /// <summary>
        /// Enable Criteria
        /// </summary>
        public override bool Enabled
        {
            get
            {

                if (((IWorkspaceEdit)(Miner.Geodatabase.Edit.Editor.EditWorkspace)) == null)
                {
                    base.m_enabled = false;
                }
                else
                {
                    if (((IWorkspaceEdit)(Miner.Geodatabase.Edit.Editor.EditWorkspace)).IsBeingEdited())
                        base.m_enabled = true;
                    else
                        base.m_enabled = false;
                }

                return base.m_enabled;
            }
        }

        /// <summary>
        /// Set Required ArcMap Variables
        /// </summary>
        /// <param name="hook"></param>
        public override void OnCreate(object hook)
        {
            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // Store the application and the map
            IApplication application = hook as IApplication;
            IMxDocument mxDocument = application.Document as IMxDocument;
            IActiveView activeView = mxDocument.FocusMap as IActiveView;
            _map = mxDocument.FocusMap as IMap;
        }

        /// <summary>
        /// Validate source Features(s)
        /// </summary>
        /// <param name="Button"></param>
        /// <param name="Shift"></param>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            #region Data Member

            bool process = false;
            string message = string.Empty;
            bool processResult = default;

            IntPtr p = default;
            IRow row = null;
            ICursor cursorFeatures = null;
            ISelectionSet ACSourceSelectionSet = default;
            clsRelationshipInfo sourceFeature = default;
            PGE_AttributeCopyForm pGE_AttributeCopyForm = default;

            #endregion

            try
            {
                //Show Status and Animation
                statusBar = m_application.StatusBar;
                statusBar.ProgressAnimation.Show();

                #region Define Editor
                if (_editor == null)
                {
                    Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object o = Activator.CreateInstance(t);
                    m_application = o as IApplication;
                    IMxDocument mxDocument = m_application.Document as IMxDocument;
                    _editor = m_application.FindExtensionByName("Esri Object Editor") as IEditor;
                }
                #endregion

                //Check Editor State
                if (_editor.EditState == esriEditState.esriStateNotEditing)
                    return;

                statusBar.Message[0] = m_ToolName + " Validating Source Feature";

                if (_editor.SelectionCount != 1)
                {
                    statusBar.Message[0] = "Invalid Source Selection Count for " + m_ToolName + " :: " + _editor.SelectionCount;
                    MessageBox.Show("Invalid Source Selection Count for " + m_ToolName + " :: " + _editor.SelectionCount, m_ToolName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                #region Get Selected Counductor Feature and Validate

                //Get Selected Counductor Feature Count
                ACSourceSelectionSet = GetSelectedConduits();

                //Check Set is not Null
                if (ACSourceSelectionSet == null)
                {
                    statusBar.Message[0] = "No valid Source Selection Found for " + m_ToolName + " :: " + ACSourceSelectionSet.Count;
                    MessageBox.Show("No valid Source Selection Found for " + m_ToolName + " :: " + ACSourceSelectionSet.Count, m_ToolName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Check Set Count
                if (ACSourceSelectionSet.Count != 1)
                {
                    statusBar.Message[0] = "Invalid Source Selection Count for " + m_ToolName + " :: " + ACSourceSelectionSet.Count;
                    MessageBox.Show("Invalid Source Selection Count for " + m_ToolName + " :: " + ACSourceSelectionSet.Count, m_ToolName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //Start Editing Operation
                _editor.StartOperation();

                //Get Feature
                ACSourceSelectionSet.Search(null, false, out cursorFeatures);
                row = cursorFeatures.NextRow();

                sourceFeature = new clsRelationshipInfo();
                sourceFeature.intObjectOID = row.OID;
                sourceFeature.relationshipMapping = GetRelationshipMapping((IObject)row);

                //Check Validity of Feature
                process = IsValid(sourceFeature);

                #endregion

                //Run Copy Attribute 
                if (process)
                {
                    message = string.Empty;
                    processResult = ProcessAttributeCopy((IFeature)row, sourceFeature, out message);


                    if (processResult)
                    {
                        _editor.StopOperation(m_ToolName);
                        p = new IntPtr(m_application.hWnd);
                        pGE_AttributeCopyForm = new PGE_AttributeCopyForm(message);
                        pGE_AttributeCopyForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                        pGE_AttributeCopyForm.Show((Form)System.Windows.Forms.Form.FromHandle(p));
                    }
                    else
                    {
                        statusBar.Message[0] = message;
                        MessageBox.Show(message, m_ToolName, System.Windows.Forms.MessageBoxButtons.OK, MessageBoxIcon.Error);
                        _editor.AbortOperation();
                    }

                }
                else
                {
                    statusBar.ProgressAnimation.Stop();
                    statusBar.Message[0] = "Please select Valid Asset";
                    _editor.AbortOperation();
                    MessageBox.Show("Please select Valid Asset");
                }

            }
            catch (Exception ex)
            {
                _logger.Error("PG&E " + m_ToolName + ", Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " at " + ex.StackTrace, ex);
                _editor.AbortOperation();
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString(), m_ToolName, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            finally
            {
                statusBar.Message[0] = "";
                statusBar.ProgressAnimation.Stop();
                //release all objects goes here
                ReleaseAllTheThings(cursorFeatures);
            }

        }

        #endregion

        #region Internal Function(s)

        /// <summary>
        /// Get Selected Counduit
        /// </summary>
        /// <returns></returns>
        private ISelectionSet GetSelectedConduits()
        {
            IFeatureSelection selection = default;
            //IFeatureSelection returnSelection = default;
            List<IFeatureLayer> featureLayers = default;
            ISelectionSet selectionSet = default;
            bool inValid = false;
            int totalSelectionCount = 0;
            try
            {
                featureLayers = new List<IFeatureLayer>();
                featureLayers = ModelNameFacade.FeatureLayerByModelName(SchemaInfo.Electric.ClassModelNames.AttributeCopySourceClass, _map, true);

                foreach (IFeatureLayer featureLayer in featureLayers)
                {
                    // Get the current selection for the Conductor layer
                    selection = featureLayer as IFeatureSelection;

                    totalSelectionCount += selection.SelectionSet.Count;

                    if (selection.SelectionSet.Count > 1 || totalSelectionCount > 1)
                    {
                        inValid = true;
                        break;
                    }

                    if (selection.SelectionSet.Count == 1)
                        selectionSet = selection.SelectionSet;

                }

                if (inValid)
                    selectionSet = default;

            }
            catch (Exception ex)
            {
                _logger.Error("PG&E " + m_ToolName + ", Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " at " + ex.StackTrace, ex);
            }
            // Return the selection as a SelectionSet
            return selectionSet;
        }

        /// <summary>
        /// Release Com Object
        /// </summary>
        /// <param name="comObjects"></param>
        void ReleaseAllTheThings(params object[] comObjects)
        {
            try
            {
                foreach (var item in comObjects)
                {
                    if (item is IDisposable)
                        ((IDisposable)item).Dispose();
                    else if (item != null && Marshal.IsComObject(item))
                        while (Marshal.ReleaseComObject(item) > 0) ;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E " + m_ToolName + ", Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " at " + ex.StackTrace, ex);
                //_logger.Error("PG&E " + m_ToolName + ", Error in ReleaseAllTheThings function", ex);
            }

        }

        /// <summary>
        /// Get Target Features from Map
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="sourceFeature"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool ProcessAttributeCopy(IFeature feature, clsRelationshipInfo sourceFeature, out string message)
        {
            #region Data Members

            int count = 0;
            int relCount = default;
            message = string.Empty;
            bool processResult = false;
            string _logMessage = string.Empty;
            string tempLogMessage = string.Empty;
            string inValidMessage = string.Empty;
            Dictionary<int, IFeature> intFeature = default;

            IGeometry geometry = default;
            IRubberBand rubberBand = default;
            IFeature updateFeature = default;
            IFeatureCursor featureCursor = default;
            ISpatialFilter spatialFilter = default;
            IScreenDisplay screenDisplay = default;
            clsRelationshipInfo clsRelationshipInfo = default;
            List<clsRelationshipInfo> targetFeatures = default;

            #endregion

            try
            {
                //Initiazlize Screen Display Element
                statusBar.Message[0] = "Select Target Feature(s) on Map";
                statusBar.ProgressAnimation.Stop();
                screenDisplay = (m_application.Document as IMxDocument).ActivatedView.ScreenDisplay;
                rubberBand = new RubberPolygonClass();
                geometry = rubberBand.TrackNew(screenDisplay, null);

                if(geometry == null)
                {
                    message = "Invalid Geometry Selected";
                    processResult = false;
                    return processResult;
                }

                if (geometry.IsEmpty)
                {
                    message = "Invalid Geometry Selected";
                    processResult = false;
                    return processResult;
                }

                #region Get Target Features

                spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = geometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialFilter.GeometryField = ((IFeatureClass)feature.Class).ShapeFieldName;
                spatialFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;

                featureCursor = ((IFeatureClass)feature.Class).Search(spatialFilter, false);

                //Initialize
                count = 0;
                _logMessage = string.Empty;
                tempLogMessage = string.Empty;
                clsRelationshipInfo = new clsRelationshipInfo();
                targetFeatures = new List<clsRelationshipInfo>();
                intFeature = new Dictionary<int, IFeature>();

                //Update Staus
                statusBar.Message[0] = "Validating/Updating Target Feature(s)";
                statusBar.ProgressAnimation.Show();

                while ((updateFeature = featureCursor.NextFeature()) != null)
                {
                    if (updateFeature.OID == feature.OID)
                        continue;

                    tempLogMessage = string.Empty;
                    clsRelationshipInfo = new clsRelationshipInfo();
                    clsRelationshipInfo.intObjectOID = updateFeature.OID;
                    clsRelationshipInfo.relationshipMapping = GetRelationshipMapping(updateFeature);
                    targetFeatures.Add(clsRelationshipInfo);
                    if (!intFeature.ContainsKey(updateFeature.OID))
                    {
                        intFeature.Add(updateFeature.OID, updateFeature);
                    }

                }

                #endregion

                if (targetFeatures.Count > 20)
                {
                    message = "InValid Selection Found (More than 20 records Selected) Selection Count :: " + targetFeatures.Count;
                    processResult = false;
                    return processResult;
                }

                foreach (clsRelationshipInfo targetFeat in targetFeatures)
                {
                    if (IsValid(sourceFeature, targetFeat))
                    {
                        if (CopyAttribute(sourceFeature, targetFeat, intFeature, out tempLogMessage))
                        {
                            _logMessage += "Updated OID " + targetFeat.intObjectOID + tempLogMessage + Environment.NewLine;
                            count++;
                        }
                    }
                    else
                    {
                        inValidMessage += "InValid Target Feature :: " + targetFeat.intObjectOID + Environment.NewLine;
                    }
                }

                if (count == 0)
                {
                    message = "No Valid Selection Found ";
                    processResult = false;
                    return processResult;
                }

                relCount = GetRelatedCount(sourceFeature);
                _logMessage = "Source Feature " + ((IDataset)feature.Class).BrowseName + " OID : " + feature.OID + Environment.NewLine
                    + "Related Records # : " + relCount + Environment.NewLine + Environment.NewLine
                    + _logMessage + Environment.NewLine + Environment.NewLine
                    + inValidMessage;

                message = _logMessage;

                processResult = true;
                statusBar.Message[0] = "Sucessfully Updated Target Feature(s)";
                statusBar.ProgressAnimation.Stop();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                _logger.Error("PG&E " + m_ToolName + ", Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " at " + ex.StackTrace, ex);
            }
            finally
            {
                //release all objects goes here
                ReleaseAllTheThings(featureCursor, spatialFilter);
            }
            return processResult;
        }

        /// <summary>
        /// Get Related Count
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <returns></returns>
        private int GetRelatedCount(clsRelationshipInfo sourceFeature)
        {
            int count = 0;
            foreach (RelationshipClass relationshipClass in sourceFeature.relationshipMapping.Keys)
            {
                count += sourceFeature.relationshipMapping[relationshipClass].Count;
            }
            return count;
        }

        /// <summary>
        /// Check Valid
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <returns></returns>
        private bool IsValid(clsRelationshipInfo sourceFeature)
        {
            bool result = false;
            bool enterValidation = false;
            IField field = default;
            int index = -1;

            foreach (RelationshipClass relationshipClass in sourceFeature.relationshipMapping.Keys)
            {
                foreach (IObject obj in sourceFeature.relationshipMapping[relationshipClass])
                {
                    field = ModelNameManager.Instance.FieldFromModelName((IObjectClass)obj.Class, SchemaInfo.Electric.FieldModelNames.AttributeCopyValidateField);

                    if (field != null)
                    {
                        enterValidation = true;

                        index = obj.Fields.FindField(field.Name);
                        if (index != -1)
                        {
                            if (Convert.ToString(obj.get_Value(index)).ToUpper() == "Primary".ToUpper() || Convert.ToString(obj.get_Value(index)).ToUpper() == "1".ToUpper())
                            {
                                result = true;
                            }
                        }
                    }
                }
            }

            if (!enterValidation && sourceFeature.relationshipMapping.Count != 0)
                result = true;

            return result;
        }

        /// <summary>
        /// Check Valid
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="targetFeat"></param>
        /// <returns></returns>
        private bool IsValid(clsRelationshipInfo sourceFeature, clsRelationshipInfo targetFeat)
        {
            bool result = true;

            foreach (RelationshipClass relationshipClass in sourceFeature.relationshipMapping.Keys)
            {
                if (targetFeat.relationshipMapping.ContainsKey(relationshipClass))
                {
                    if (sourceFeature.relationshipMapping[relationshipClass].Count < targetFeat.relationshipMapping[relationshipClass].Count)
                        result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Copy Related value form Source Feature to Target Feature
        /// </summary>
        /// <param name="sourceFeat"></param>
        /// <param name="targetFeat"></param>
        /// <param name="intFeature"></param>
        /// <param name="tempLogMessage"></param>
        /// <returns></returns>
        private bool CopyAttribute(clsRelationshipInfo sourceFeat, clsRelationshipInfo targetFeat, Dictionary<int, IFeature> intFeature, out string tempLogMessage)
        {
            bool returnResult = false;
            tempLogMessage = "";
            int sourceCounter = 0;
            int counter = 0;
            int newCounter = 0;
            IRow row = default;
            IMMEnumField mMEnumField = default;
            IField field = default;
            try
            {
                foreach (IRelationshipClass relationshipClass in sourceFeat.relationshipMapping.Keys)
                {
                    counter = sourceFeat.relationshipMapping[relationshipClass].Count;

                    if (targetFeat.relationshipMapping.ContainsKey(relationshipClass))
                    {
                        foreach (IObject obj in targetFeat.relationshipMapping[relationshipClass])
                        {
                            mMEnumField = ModelNameManager.Instance.FieldsFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.AttributeCopyCopyField);
                            field = null;
                            while ((field = mMEnumField.Next()) != null)
                            {
                                int index = obj.Fields.FindField(field.Name);
                                obj.Value[index] = sourceFeat.relationshipMapping[relationshipClass][sourceCounter].Value[index];
                            }

                            //tempLogMessage += " Target OID " + obj.OID + " in Table " + ((IDataset)obj.Class).BrowseName + " updated with Source OID " + sourceFeat.relationshipMapping[relationshipClass][sourceCounter].OID + "\n";
                            obj.Store();
                            counter--;
                            sourceCounter++;
                        }
                    }

                    while (counter != 0)
                    {
                        row = ((ITable)sourceFeat.relationshipMapping[relationshipClass][sourceCounter].Class).CreateRow();
                        mMEnumField = ModelNameManager.Instance.FieldsFromModelName(sourceFeat.relationshipMapping[relationshipClass][sourceCounter].Class, SchemaInfo.Electric.FieldModelNames.AttributeCopyCopyField);
                        field = null;
                        while ((field = mMEnumField.Next()) != null)
                        {
                            int index = row.Fields.FindField(field.Name);
                            row.Value[index] = sourceFeat.relationshipMapping[relationshipClass][sourceCounter].Value[index];
                        }
                        row.Store();
                        relationshipClass.CreateRelationship(intFeature[Convert.ToInt32(targetFeat.intObjectOID)], (IObject)row);
                        newCounter++;
                        //tempLogMessage += " Target OID " + row.OID + " in Table " + ((IDataset)sourceFeat.relationshipMapping[relationshipClass][sourceCounter].Class).BrowseName + " created with values from Source OID " + sourceFeat.relationshipMapping[relationshipClass][sourceCounter].OID + "\n";
                        counter--;
                        sourceCounter++;
                    }
                }

                returnResult = true;

                if (newCounter != 0)
                    tempLogMessage = " : " + newCounter + " new Related Records Created ";
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E " + m_ToolName + ", Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + " at " + ex.StackTrace, ex);
                throw ex;
            }

            return returnResult;
        }

        /// <summary>
        /// Get Realted Record Mapping
        /// </summary>
        /// <param name="pObject"></param>
        /// <returns></returns>
        private Dictionary<IRelationshipClass, List<IObject>> GetRelationshipMapping(IObject pObject)
        {
            #region Data Members

            int index = -1;
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = new Dictionary<IRelationshipClass, List<IObject>>();

            IField field = default;
            IObject obj = default;
            ISet relatedFeatures = default;
            IObjectClass objClass = default;
            IObjectClass originClass = null;
            IRelationshipClass relClass = default;
            IEnumRelationshipClass relClasses = default;

            #endregion

            objClass = pObject.Class as IObjectClass;
            relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
            relClasses.Reset();
            relClass = relClasses.Next();

            while (relClass != null)
            {
                index = -1;

                if (ModelNameManager.Instance.ContainsClassModelName(relClass.DestinationClass, SchemaInfo.Electric.ClassModelNames.AttributeCopyRelatedClass))
                {
                    originClass = relClass.DestinationClass;

                    System.Diagnostics.Debug.Print(originClass.AliasName);

                    field = ModelNameManager.Instance.FieldFromModelName(originClass, SchemaInfo.Electric.FieldModelNames.AttributeCopyValidateField);

                    if (field != null)
                    {
                        index = originClass.Fields.FindField(field.Name);
                    }

                    if (!relationshipMapping.ContainsKey(relClass))
                    {
                        relationshipMapping.Add(relClass, new List<IObject>());
                    }

                    relatedFeatures = relClass.GetObjectsRelatedToObject(pObject);
                    relatedFeatures.Reset();

                    for (int i = 0; i < relatedFeatures.Count; i++)
                    {
                        obj = relatedFeatures.Next() as IObject;
                        if (index != -1)
                        {
                            string test = Convert.ToString(obj.Value[index]);
                            if (Convert.ToString(obj.Value[index]) == "PRIMARY" || Convert.ToString(obj.Value[index]) == "1")
                            {
                                relationshipMapping[relClass].Add(obj);
                            }
                        }
                        else
                        {
                            relationshipMapping[relClass].Add(obj);
                        }
                    }

                    if (relationshipMapping[relClass].Count == 0)
                    {
                        relationshipMapping.Remove(relClass);
                    }

                }

                if (ModelNameManager.Instance.ContainsClassModelName(relClass.OriginClass, SchemaInfo.Electric.ClassModelNames.AttributeCopyRelatedClass))
                {
                    originClass = relClass.OriginClass;

                    System.Diagnostics.Debug.Print(originClass.AliasName);

                    field = ModelNameManager.Instance.FieldFromModelName(originClass, SchemaInfo.Electric.FieldModelNames.AttributeCopyValidateField);

                    if (field != null)
                    {
                        index = originClass.Fields.FindField(field.Name);
                    }

                    if (!relationshipMapping.ContainsKey(relClass))
                    {
                        relationshipMapping.Add(relClass, new List<IObject>());
                    }

                    relatedFeatures = relClass.GetObjectsRelatedToObject(pObject);
                    relatedFeatures.Reset();

                    for (int i = 0; i < relatedFeatures.Count; i++)
                    {
                        obj = relatedFeatures.Next() as IObject;
                        if (index != -1)
                        {
                            string test = Convert.ToString(obj.Value[index]);
                            if (Convert.ToString(obj.Value[index]) == "PRIMARY" || Convert.ToString(obj.Value[index]) == "1")
                            {
                                relationshipMapping[relClass].Add(obj);
                            }
                        }
                        else
                        {
                            relationshipMapping[relClass].Add(obj);
                        }
                    }

                    if (relationshipMapping[relClass].Count == 0)
                    {
                        relationshipMapping.Remove(relClass);
                    }

                }

                relClass = relClasses.Next();

            }

            return relationshipMapping;
        }


        #endregion

    }

}
