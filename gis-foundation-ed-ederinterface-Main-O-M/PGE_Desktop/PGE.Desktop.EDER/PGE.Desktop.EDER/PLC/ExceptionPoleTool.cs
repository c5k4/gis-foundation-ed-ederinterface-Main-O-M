using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using Miner.Interop;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geometry;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;

namespace Telvent.PGE.ED.Desktop.PLC
{
    /// <summary>
    /// Summary description for ExceptionPoleTool.
    /// </summary>
    [Guid("9e54bbdb-5832-4329-af6a-fa8659235668")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Telvent.PGE.ED.Desktop.PLC.ExceptionPoleTool")]
    public sealed class ExceptionPoleTool : BaseCommand
    {
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
          #region Private Members

        private IPoint _lastPoint;
        private string _toolName;

        private EditorClass _editor;
        private IEditSketch m_editSketch;
        private IEditLayers _editLayers;
        IMMAttributeAgent attributeAgent;


        //Private Member Constants
        private const string AttrAgentExtn = "mmDesktop.MMAttributeAgent";
        private const string EditorExt = "ESRI Object Editor";

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        IFeature _newFeature = null;
        double _dLongitude = 0;
        double _dLatitude = 0;
        private IEditor m_editor;
       
        string _jobNumberFromMemory = null;
        static String SLookupTable = "EDGIS.PGE_PLC_CONFIG";
       
        # endregion
        private IApplication m_application;
        public ExceptionPoleTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "SPoT Tools"; //localizable text
            base.m_caption = "Exception Linear Sketch Tool";  //localizable text
            base.m_message = "Exception Linear Sketch Tool";  //localizable text 
            base.m_toolTip = "Exception Linear Sketch Tool";  //localizable text 
            base.m_name = "ExceptionPoleTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")

            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            try
            {
                #region Define Editor
                if (m_editor == null)
                {
                    Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object o = Activator.CreateInstance(t);
                    m_application = o as IApplication;
                    IMxDocument mxDocument = m_application.Document as IMxDocument;
                    m_editor = m_application.FindExtensionByName("Esri Object Editor") as IEditor;
                }
                # endregion

                #region Check JobNumber
                _jobNumberFromMemory = Telvent.PGE.ED.Desktop.ArcMapCommands.PopulateLastJobNumberUC.jobNumber;
                //if the jobnumber is null or empty then show message else assign the value to current obj.
                if (string.IsNullOrEmpty(_jobNumberFromMemory) || _jobNumberFromMemory.Trim().Length < 1)
                {
                    throw new COMException(
                       "Please provide a Job Number within the PG&E Job Number toolbar and press Enter before creating assets.",
                       (int)mmErrorCodes.MM_E_CANCELEDIT);
                }
                #endregion try
            
                //Create supportstructure and assign attribute
                ITable tPLCLookup = GetLookUpTable();
                IRow ConfigTableRow = GetCodeRowForNewExceptionTool(tPLCLookup);
                //Get Support Structure Featureclass Name
                string SupportStructureFeatClassName = ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("SupportStructure_TblName")).ToString();
                IFeatureLayer SupportStructureLayer = GetLayerFromMap(SupportStructureFeatClassName);

               
                IMMFieldAdapter PMMF = null;
                #region  ArcFM Linear Point Implementation

                EditorClass _editorC = Marshal.CreateWrapperOfType(m_editor, typeof(EditorClass)) as EditorClass;
                m_editSketch = _editorC;
                _editorC.SetCurrentLayer(SupportStructureLayer, 1);

                IEditTask pedittask = GetEditTaskByName(_editorC, "ArcFM Linear Point");

                _editorC.CurrentTask = pedittask;
                
                _editLayers = _editorC;
                attributeAgent = GetAttributeAgentExtension();


            #region SetUP
            //This event is fired when Stop Editing is called
            //Exit if there is no target (current) layer

            if (_editLayers.CurrentLayer == null)
            {
                return;
            }
            if (attributeAgent == null)
            {
                return;
            }

            IFeatureLayer attrFeatureLayer = attributeAgent.EditFeatureLayer;
            IMMFieldManager fieldManager = attributeAgent.FieldManager;
            if (attrFeatureLayer != null)
            {
                string EditLayerName = ((IDataset)attrFeatureLayer).BrowseName.ToUpper();
                if (EditLayerName != SupportStructureFeatClassName)
                {
                    throw new COMException(
                    "Please select the SupportStructure Layer in ArcFM Attribute Editor <Target> Tab.",
                    (int)mmErrorCodes.MM_E_CANCELEDIT);
                }
                else
                {
                    PMMF = fieldManager.FieldByName(ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("subtypecd_ColName")).ToString());
                    int SUBTYPE_POLE = 1;
                     
                    int subtypeCd=Convert.ToInt32(PMMF.Value);

                    if ((subtypeCd != SUBTYPE_POLE))
                    {
                        throw new COMException(
                    "To run the exception Tool -please select the SupportStructure Layer in ArcFM Attribute Editor <Target> tab with subtype 'Pole'.",
                    (int)mmErrorCodes.MM_E_CANCELEDIT);
                    }
                }
            }
            else
            {
                throw new COMException(
                                    "Please select the SupportStructure Layer in ArcFM Attribute Editor <Target> tab with subtype 'Pole'.",
                                    (int)mmErrorCodes.MM_E_CANCELEDIT);
            }

            if (_editLayers.CurrentLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)
            {
                m_editSketch.GeometryType = esriGeometryType.esriGeometryPolyline;
            }
            else
            {
                m_editSketch.GeometryType = esriGeometryType.esriGeometryNull;
            }
            # endregion

           
            //fieldManager.Shape = pPole;

            #region Update Field Value
            //// find intersecting EDGIS.MIANTENANCEPLAT feature to update the LocalOfficeID
            String strLocalOfcId = String.Empty;
            //IFeatureClass featureClassMP = ((IFeatureWorkspace)m_editor.EditWorkspace).OpenFeatureClass((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("Maintenanceplat_TblName"))).ToString());
            //IFeature iFeatMp = GetFeatureFromSpatialQuery(pPole, featureClassMP);

            //if (iFeatMp == null)
                strLocalOfcId = "XJ";
           // else
                //strLocalOfcId = iFeatMp.get_Value(iFeatMp.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("LocalOfficeId_ColName"))).ToString())).ToString().Trim();

           

            PMMF = fieldManager.FieldByName(ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("LocalOfficeId_ColName")).ToString());
            //string OldLocalOfficeID = PMMF.Value.ToString();
            UpdateAttibute(ref fieldManager, ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("LocalOfficeId_ColName")).ToString(), strLocalOfcId);

            PMMF = fieldManager.FieldByName(ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("POLEUSE_ColName")).ToString());
            //string OldPoleUse = PMMF.Value.ToString();
            UpdateAttibute(ref fieldManager, ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("POLEUSE_ColName")).ToString(), ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("POLEUSE_DEFAULT_VAL")).ToString());

            PMMF = fieldManager.FieldByName(ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("InstallJobNumber_ColName")).ToString());
            //string OldLocalOfficeID = PMMF.Value.ToString();
            UpdateAttibute(ref fieldManager, ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("InstallJobNumber_ColName")).ToString(), 0);

            # endregion      
            #endregion
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Exception Linear Sketch Tool on Notification, Error in OnMouseDown function", ex);
                m_editor.AbortOperation();
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString(), "PG&E PLC Exception Linear Sketch Tool", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

            }
            finally
            {
                
            }
        }

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
        #endregion

        #region Internal Methods
        private void UpdateAttibute(ref IMMFieldManager fieldManager, string sFieldName, object sFieldValue)
        {
            try
            {
                IMMFieldAdapter PMMFA = null;

                PMMFA = fieldManager.FieldByName(sFieldName);
                PMMFA.Value = sFieldValue;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while updating the field - " + sFieldName, ex);
            }
        }

        private void ResetAttribute(ref IMMFieldManager fieldManager, string sFieldName)
        {
            try
            {
                IMMFieldAdapter PMMFA = null;

                PMMFA = fieldManager.FieldByName(sFieldName);
                PMMFA.Value = DBNull.Value;
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while reset the field - " + sFieldName, ex);
            }
        }

        private string GetUserID()
        {
            string sUserLanID = string.Empty;
            try
            {
                IMMDefaultConnection _defaultConnection;
                string mmproprtiesextension = "MMPropertiesExt";
                _defaultConnection = (IMMDefaultConnection)m_application.FindExtensionByName(mmproprtiesextension);
                if (_defaultConnection != null)
                {
                    sUserLanID = _defaultConnection.UserName;
                }
                else
                {
                    _logger.Info("PGE_LANID user name is not updated.");
                }
            }
            catch (Exception ex)
            { }
            return sUserLanID;

        }
        private ITable GetLookUpTable()
        {
            ITable tPLCLookup = null;
            try
            {

                IFeatureWorkspace iFeatWs = (IFeatureWorkspace)m_editor.EditWorkspace;

                tPLCLookup = new MMTableUtilsClass().OpenTable(SLookupTable, iFeatWs);

                if (tPLCLookup == null)
                    throw new Exception("Failed to load table " + SLookupTable);

            }
            catch (Exception ex)
            { }

            return tPLCLookup;
        }

        private IRow GetCodeRowForNewExceptionTool(ITable tPLCLookup)
        {
            IRow codeRow = null;
            try
            {
                ICursor ICur = null;

                QueryFilter queryFilter = null;
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("ClassData='{0}'", "EXP_CREATE_POLE");
                ICur = tPLCLookup.Search(queryFilter, false);
                codeRow = ICur.NextRow();
                if (ICur != null) { Marshal.ReleaseComObject(ICur); }
                if (codeRow == null) throw new Exception("Configuration Data not found for CREATE_POLE in " + SLookupTable + " table");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return codeRow;
        }

        private IFeatureLayer GetLayerFromMap(string LayerName)
        {
            IFeatureLayer pLayer = null;
            try
            {
                #region Get Layer
                UID id = new UIDClass();
                id.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; //IGeoFeatureLayer

                IEnumLayer enumLayer = (IEnumLayer)((IMap)((IMxDocument)m_application.Document).FocusMap).get_Layers(id, true);
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    IFeatureLayer fLayer = layer as IFeatureLayer;
                    if (fLayer != null)
                    {
                        IFeatureClass fClass = fLayer.FeatureClass as IFeatureClass;
                        if (fClass != null)
                        {
                            //Run with layer
                            if (LayerName == ((IDataset)fLayer).BrowseName.ToUpper())
                            {
                                pLayer = fLayer;
                                break;
                            }

                        }

                    }
                    layer = enumLayer.Next();
                }


                #endregion
            }
            catch { }

            return pLayer;

        }

    
        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>

        private IEditTask GetEditTaskByName(IEditor _editor, string strEditTaskName)
        {
            IEditTask GetEditTaskByName = null;
            for (int i = 0; i <= _editor.TaskCount - 1; i++)
            {
                if (_editor.get_Task(i).Name.ToString().ToUpper() == strEditTaskName.ToString().ToUpper())
                {
                    GetEditTaskByName = _editor.get_Task(i);
                    break;
                }

            }
            return GetEditTaskByName;
        }


        private IMMAttributeAgent GetAttributeAgentExtension()
        {
            IMMAttributeAgent attributeAgent = null;

            UID pUID = new UID { Value = AttrAgentExtn };

            if (m_editor != null)
            {
                IExtension pExt = m_editor.FindExtension(pUID);
                if (pExt == null)
                {
                    throw new Exception("-1, " + "Failed getting: " + AttrAgentExtn);
                }
                attributeAgent = pExt as IMMAttributeAgent;
            }
            return attributeAgent;
        }

 

 
        #endregion

 
    }
}
