using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using Miner.Geodatabase.Edit;
using ESRI.ArcGIS.Carto;
using System.Collections.Generic;

using System.Runtime.Serialization.Json;
using System.IO;
using System.Reflection;
using Miner.ComCategories;
using System.Net;
using System.Text;
using PGE.Common.Delivery.Diagnostics;
using stdole;
using System.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Display;
using Miner.Interop;
using Microsoft.Win32;
using ESRI.ArcGIS.Editor;
using System.Text.RegularExpressions;

namespace PGE.Desktop.EDER.PLC
{
    /// <summary>
    /// Class to create new pole during exception.
    /// </summary>
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PLC.CreatePoleTool")]
    [ComVisible(true)]
    [Guid("ede92ba9-7ac5-49cc-9d9e-c8f768b1038d")]

    public sealed class CreatePoleTool : BaseTool
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
       
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        IFeature _newFeature = null;
        double _dLongitude = 0;
        double _dLatitude = 0;
        private IEditor m_editor;
       
        string _jobNumberFromMemory = null;
        static String SLookupTable = "EDGIS.PGE_PLC_CONFIG";
        IEditor _editor = null;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CreatePoleTool()
        {
           
            try
            {
                base.m_cursor = Cursors.Cross;
                base.m_category = "SPoT Tools";
                base.m_caption = "Exception New Pole Tool";  //localizable text 
                base.m_message = "Exception New Pole Tool";  //localizable text
                base.m_toolTip = "Exception New Pole Tool";  //localizable text
                base.m_name = "PGEPLCTools_CreatePoleTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")

                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
               
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #endregion

        private IApplication m_application;
       

        #region Overridden Class Methods
        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            IRow codeRow = null;
            QueryFilter queryFilter = null;

            try
            {
                #region Define Editor
                if (_editor == null)
                {
                    Type t = Type.GetTypeFromProgID("esriFramework.AppRef");
                    object o = Activator.CreateInstance(t);
                    m_application = o as IApplication;
                    IMxDocument mxDocument = m_application.Document as IMxDocument;
                    _editor = m_application.FindExtensionByName("Esri Object Editor") as IEditor;
                }
                # endregion


                #region Check JobNumber
                _jobNumberFromMemory = PGE.Desktop.EDER.ArcMapCommands.PopulateLastJobNumberUC.jobNumber;
                //if the jobnumber is null or empty then show message else assign the value to current obj.
                if (string.IsNullOrEmpty(_jobNumberFromMemory) || _jobNumberFromMemory.Trim().Length < 1)
                {
                    throw new COMException(
                       "Please provide a Job Number within the PG&E Job Number toolbar and press Enter before creating assets.",
                       (int)mmErrorCodes.MM_E_CANCELEDIT);
                }

          
                #endregion



                if (_editor.EditState == esriEditState.esriStateNotEditing)
                    return;

                _editor.StartOperation();

                IDisplayTransformation transform = GetDisplayTransformation(m_application as IMxApplication);

                if (transform == null)
                    return;

                IPoint pPole = transform.ToMapPoint(X, Y);

                //Create supportstructure and assign attribute

                Double XCoord = pPole.X;
                Double YCoord = pPole.Y;
                IFeatureWorkspace iFeatWs = (IFeatureWorkspace)_editor.EditWorkspace;

                ITable tPLCLookup = new MMTableUtilsClass().OpenTable(SLookupTable, iFeatWs);

                if (tPLCLookup == null)
                    throw new Exception("Failed to load table " + SLookupTable);

                ICursor ICur = null;

                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("ClassData='{0}'", "EXP_CREATE_POLE");
                ICur = tPLCLookup.Search(queryFilter, false);
                codeRow = ICur.NextRow();

                if (codeRow == null) throw new Exception("Configuration Data not found for CREATE_POLE in " + SLookupTable + " table");

                IFeatureClass IfcSupportStructure = iFeatWs.OpenFeatureClass((codeRow.get_Value(codeRow.Fields.FindField("SupportStructure_TblName"))).ToString());

                IGeometry SupportStructure = (IGeometry)pPole;

                //Create Pole Feature

                _newFeature = IfcSupportStructure.CreateFeature();

                _newFeature.Shape = SupportStructure;

                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("InstallJobNumber_ColName"))).ToString()), _jobNumberFromMemory);
                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("subtypecd_ColName"))).ToString()), 1);
                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Datecreated_ColName"))).ToString()), System.DateTime.Now.ToString());


                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("CustomerOwner_ColName"))).ToString()), "N");
                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("Status_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("STATUS_DEFAULT_VAL"))).ToString());

                XY_LatLong(_newFeature);

                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("GpsLongitude_ColName"))).ToString()), _dLongitude);
                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("GpsLatitude_ColName"))).ToString()), _dLatitude);

                //find intersecting EDGIS.MIANTENANCEPLAT feature
                String strLocalOfcId = String.Empty;
                IFeatureClass featureClassMP = iFeatWs.OpenFeatureClass((codeRow.get_Value(codeRow.Fields.FindField("Maintenanceplat_TblName"))).ToString());
                IFeature iFeatMp = GetFeatureFromSpatialQuery(_newFeature, featureClassMP);

                if (iFeatMp == null)
                    strLocalOfcId = "XJ";
                else
                    strLocalOfcId = iFeatMp.get_Value(iFeatMp.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("LocalOfficeId_ColName"))).ToString())).ToString().Trim();

                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("LocalOfficeId_ColName"))).ToString()), strLocalOfcId);

                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("POLEUSE_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("POLEUSE_DEFAULT_VAL"))).ToString());
                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("POLETYPE_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("POLETYPE_DEFAULT_VAL"))).ToString());
                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("MATERIAL_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("MATERIAL_DEFAULT_VAL"))).ToString());
                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("JOINTCOUNT_ColName"))).ToString()), (codeRow.get_Value(codeRow.Fields.FindField("JOINTCOUNT_DEFAULT_VAL"))).ToString());

                //Calling Start Anaysis to get PLDBID

                StartAnalysisCall sacObj = new StartAnalysisCall();
                double dpld = sacObj.Createpldbid(XCoord, YCoord, _newFeature, _jobNumberFromMemory);

                if (dpld == 0.0)
                {
                    throw new Exception("PLDBID not returned from the StartAnalysis service");
                }

                _newFeature.set_Value(_newFeature.Fields.FindField((codeRow.get_Value(codeRow.Fields.FindField("pldbid_ColName"))).ToString()), dpld);
                _newFeature.Store();
                _editor.StopOperation("PGEPLCTools_CreatePoleTool");
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Exception Pole Tool on Notification, Error in OnMouseDown function", ex);
                _editor.AbortOperation();
                System.Windows.Forms.MessageBox.Show(ex.Message.ToString(), "PG&E PLC Exception Pole Tool", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

            }
            finally
            {
                //release all objects goes here
                ReleaseAllTheThings(codeRow, queryFilter);
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


        #region Unused Methods

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
           
        }
      
        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add CreatePoleTool.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add CreatePoleTool.OnMouseUp implementation
        }
        #endregion
        #endregion

        #region Internal Methods

        public IDisplayTransformation GetDisplayTransformation(IMxApplication mxApp)
        {
            try
            {
                IAppDisplay appDisplay = mxApp.Display;
                if (appDisplay == null)
                    return null;

                IDisplay display = (IDisplay)appDisplay.FocusScreen;

                if (display == null)
                    return null;
                return display.DisplayTransformation;
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Exception Pole Tool on Notification, Error in GetDisplayTransformation function", ex);
            }

            return null;
        }

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
                _logger.Error("PG&E PLC Exception Pole Tool on Notification, Error in ReleaseAllTheThings function", ex);
            }

        }



        /// <summary>Get feature from spatial serch </summary>
        /// <param name="feat">esri feature</param>
        /// <param name="polyFeatClass">feature class to serch</param>
        /// <returns>return polygon feature</returns>
        private IFeature GetFeatureFromSpatialQuery(IFeature feat, IFeatureClass polyFeatClass)
        {
            IFeature searchedFeature = null;
            IFeatureCursor featCursor = null;
            try
            {
                //perform spatial filter
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = feat.Shape;
                spatialFilter.GeometryField = polyFeatClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //get featureCursor
                featCursor = polyFeatClass.Search(spatialFilter, false);

                //Get the feature from feature cursor
                while ((searchedFeature = featCursor.NextFeature()) != null)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Exception Pole Tool on Notification, Error in GetFeatureFromSpatialQuery function", ex);
            }
            finally
            {
                //Release COM object
                while (Marshal.ReleaseComObject(featCursor) > 0) { }
                featCursor = null;
            }
            return searchedFeature;

        }

        //FUnction to convert XY to LAtitude longitude
        public void XY_LatLong(IFeature feature)
        {
            try
            {
                ISpatialReferenceFactory2 srFactory = new SpatialReferenceEnvironmentClass();
                int NAD27_Geograpic = (4326);
                ISpatialReference GeographicSR = srFactory.CreateSpatialReference(NAD27_Geograpic);

                IFeatureChanges pFeatChange = (IFeatureChanges)feature;
                if (pFeatChange.ShapeChanged)
                {
                    IPoint pSupportStruct = (IPoint)feature.ShapeCopy;
                    //ISpatialReference SpatialReference;
                    IPoint geographicPoint = new ESRI.ArcGIS.Geometry.Point();
                    geographicPoint.X = pSupportStruct.X;
                    geographicPoint.Y = pSupportStruct.Y;
                    geographicPoint.Z = pSupportStruct.Z;
                    geographicPoint.SpatialReference = pSupportStruct.SpatialReference; // PCS

                    geographicPoint.Project(GeographicSR);

                    _dLatitude = geographicPoint.Y;
                    _dLongitude = geographicPoint.X;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Exception Pole Tool on Notification , Latitude Longtitude converter ", ex);
            }
        }
        #endregion
    }
}
