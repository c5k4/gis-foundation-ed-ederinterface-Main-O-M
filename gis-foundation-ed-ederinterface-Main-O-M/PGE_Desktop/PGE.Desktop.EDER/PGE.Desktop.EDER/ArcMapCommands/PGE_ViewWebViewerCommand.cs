using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geometry;
using System.Windows.Forms;
using System.Diagnostics;
using PGE.Common.Delivery.Process;
using Miner.Interop.Process;
using System.Data;
using Miner;
using PGE.Desktop.EDER.Login;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// </summary>
    [Guid("d41152e2-57a9-4ac5-a94b-351e8c677082")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ComVisible(true)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.ViewWebr")]
    public sealed class PGE_ViewWebViewerCommand : BaseCommand
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
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IApplication m_application;
        #region Private Variable
        Double dLat = 0.0;
        Double dLong = 0.0;
        #endregion
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        

        public PGE_ViewWebViewerCommand()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = "PG&E View Web Viewer"; //localizable text
            base.m_caption = "Get WebViewer URL at this location";  //localizable text
            base.m_message = "Get WebViewer URL at this location";  //localizable text 
            base.m_toolTip = "Get WebViewer URL at this location Tool";  //localizable text 
            base.m_name = "PGE.Desktop.EDER.ArcMapCommands.ViewWebr";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")


            try
            {
                //
                // TODO: change bitmap name if necessary
                //
                //string bitmapResourceName = GetType().Name + ".bmp";
                //base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_bitmap = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Desktop.EDER.Bitmaps.PGE_ViewWebViewer.bmp"));
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
            try
            {
                if (hook == null)
                    return;

                m_application = hook as IApplication;

                ////Disable if it is not ArcMap
                //if (hook is IMxApplication)
                //    base.m_enabled = true;
                //else
                //    base.m_enabled = false;

                //base.m_enabled = false;

                // TODO:  Add other initialization code
                //if (m_application != null)
                //PGEGlobal.Application = m_application;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            //
            //  TODO: Sample code showing how to access button host
            //

            try
            {
                //YXA6 -JeeraID EDGISREARC-1218
                //Open WEBR from EDER where URL is configured in PGEDATA.PGE_EDERCONFIG Config table
                String URLPath = FindUrlFromRegistry("WEBR_URL");
                ////string URLPATH= "http://wwwedgis/EDViewer/?LAT=X&LONG=Y&SCALE=100";

                m_application.CurrentTool = null;
                IMxDocument mxDoc = (IMxDocument)m_application.Document;

                IEnvelope envelope = mxDoc.ActiveView.Extent;
                double xMin = envelope.XMin;
                double xMax = envelope.XMax;
                double yMin = envelope.YMin;
                double yMax = envelope.YMax;

                //TO get center point of Active view
                IPoint pt = new ESRI.ArcGIS.Geometry.Point();
                pt.SpatialReference = mxDoc.ActiveView.Extent.SpatialReference;
                pt.X = (xMax + xMin) / 2;
                pt.Y = (yMax + yMin) / 2;

                //Get Lat Long of center Point
                GetLatLong(pt);

                //To replace lat long
                URLPath = URLPath.Replace("X", dLat.ToString()).Replace("Y", dLong.ToString());

                //Copy Complete URL with Lat long in clipboard
                CopyContent(URLPath);

         

                m_application.StatusBar.Message[0] = "Web Viewer URL for current location copied to clipboard succesfully.";
                m_application.RefreshWindow();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        private string FindUrlFromRegistry(string surlKeyName)
        {
            string surlValue=string.Empty;
            IMMRegistry _reg = new MMRegistry();
            try
            {

                object o_url = MMRegistrySettings.WEBR_URL;
                if (o_url != null)
                {
                    surlValue = o_url.ToString();
                } 
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return surlValue;
        }

        /// <summary>
        /// Find WEBR url from EDER config file
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        private string FindUrlFromConfigFile(string skey)
        {
            string returnURL = string.Empty;


            IMMPxApplication _PxApp = null;
            IMMPxIntegrationCache intcache = (IMMPxIntegrationCache)m_application.FindExtensionByName("Session Manager Integration Extension");
            if (intcache != null)
            {
                _PxApp = intcache.Application;

            }
            System.Data.DataTable dt = new System.Data.DataTable();
            
            PxDb localPXDb = new PxDb(_PxApp);
            string Query = string.Empty;
            try
            {

                // Get the WEBR URL to copy on Clip Board
                Query = "Select VALUE FROM " + SchemaInfo.General.pEDERCONFIGTABLE  + " WHERE KEY = '" + skey + "'";
                dt = localPXDb.ExecuteQuery(Query);
                foreach (DataRow dr in dt.Rows)
                {
                    returnURL = dr["VALUE"].ToString();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return returnURL;
        }

        #region Private Methods
        private void GetLatLong(IPoint ctrPoint)
        {
            try
            {
                //Find Latitude and Longitude
                ISpatialReferenceFactory2 srFactory = new SpatialReferenceEnvironmentClass();
                int NAD27_Geograpic = (4326);
                ISpatialReference GeographicSR = srFactory.CreateSpatialReference(NAD27_Geograpic);
                // IPoint pPoint = (IPoint)ctrPoint.ShapeCopy;
                IPoint pPoint = ctrPoint;
                IPoint geographicPoint = new ESRI.ArcGIS.Geometry.Point();
                geographicPoint.X = pPoint.X;
                geographicPoint.Y = pPoint.Y;
                geographicPoint.Z = pPoint.Z;

                geographicPoint.SpatialReference = pPoint.SpatialReference; // PCS

                geographicPoint.Project(GeographicSR);

                dLat = geographicPoint.Y;
                dLong = geographicPoint.X;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }
        private void CopyContent(string URL)
        {
            Clipboard.SetText(URL);
        }
        #endregion

        #endregion
    }
}
