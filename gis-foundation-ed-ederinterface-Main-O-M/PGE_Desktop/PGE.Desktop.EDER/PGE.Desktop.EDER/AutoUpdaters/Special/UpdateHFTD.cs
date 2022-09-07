using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geometry;
using System.Threading;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Update HFTD ID field for Electrice feature class.
    /// </summary>
    [ComVisible(true)]
    [Guid("D2421128-F789-4C30-A798-0E5EE3B41CC4")]
    [ProgId("PGE.Desktop.EDER.UpdateHFTD")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class UpdateHFTD : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // Add a sneaky option allowing us to bypass model names lookups. The bypass is disabled by default but callers can turn it on via property
        private static bool _lookupHFTDByModelName = true;
        //ME Q2 2020 : DA#2
        private string HFTDLayerName = "LBGIS.HighFireThreatDist";
        private string HFTDieldName = "HFTD";
        private static string config_table = "PGEDATA.APPLICATIONCONFIG";
        private static string lbgis_sde_path ;
        private static IWorkspace lbWksp ;
        
        public static bool LookupHFTDByModelName
        {
            set
            {
                _lookupHFTDByModelName = value;
            }
        }

        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public UpdateHFTD()
            : base("PGE Populate HFTD AU")
        {
        }
        #region Base Special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.HFTD);
                _logger.Debug("Field model name :" + SchemaInfo.Electric.FieldModelNames.HFTD + " Found-" + enabled);

            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should be run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode"> The auto updater mode. </param>
        /// <returns> <c>true</c> if the AuoUpdater should be executed; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            if (eAUMode != mmAutoUpdaterMode.mmAUMFeederManager)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            IFeatureCursor pFCursor = null;            
            IFeatureWorkspace lbgisWkp = null;
            //Skip the step to find HFTD if the shape has not been changed...
            //if ((eEvent == mmEditEvent.mmEventFeatureUpdate) && (ShapeChanged(obj) == false))
            //{
            //    _logger.Info("Feature shape is not changed, hence HFTD is not fetching from landbase");
            //    return;
            //}
            
            try
            {
                string hftdValue = string.Empty;
                string hftdCode = string.Empty;
                //Connecting to landbase databse with read_only user
                if (lbWksp == null)
                {
                    //geting path from registry
                    //Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Miner and Miner\PGE");
                    //if (key != null) { lbgis_sde_path = key.GetValue("LBGISRO_SDEPath").ToString(); }
                    //if (!string.IsNullOrEmpty(lbgis_sde_path))
                    //{
                    //    lbWksp = ArcSdeWorkspaceFromFile(lbgis_sde_path);
                    //}

                    lbWksp = ArcSdeWorkspaceFromFile(PGE_DBPasswordManagement.ReadEncryption.GetSDEPath("LBGIS_RO@LANDBASE"));
                    
                }
                         
                if (lbWksp == null)
                {
                    _logger.Info("landbase workspace is not found");
                    return;
                }
                if (lbWksp != null)
                {
                    lbgisWkp = lbWksp as IFeatureWorkspace;
                    if (lbgisWkp != null)
                    {
                        IFeatureClass HFTDfeatureClass = lbgisWkp.OpenFeatureClass(HFTDLayerName);
                        if (HFTDfeatureClass != null)
                        {
                            ISpatialFilter pSFilter = new SpatialFilterClass();
                            pSFilter.Geometry = (obj as IFeature).Shape;
                            pSFilter.SubFields = HFTDieldName;
                            pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            pFCursor = HFTDfeatureClass.Search(pSFilter, true);
                            IFeature hftdFeat = pFCursor.NextFeature();
                            int HFTDindex = pFCursor.FindField(HFTDieldName);
                            if (hftdFeat != null)
                            {
                                hftdValue = hftdFeat.get_Value(HFTDindex).ToString();
                            }
                            //getting HFTD code
                            hftdCode = getHFTDCode(hftdValue);
                            //Setting HFTD value
                            int featureHFTDFieldIndex = ModelNameFacade.FieldIndexFromModelName(((IFeature)obj).Class, SchemaInfo.Electric.FieldModelNames.HFTD);
                            ((IFeature)obj).set_Value(featureHFTDFieldIndex, hftdCode);
                            _logger.Info("HFTD value : " + hftdCode + " updated for Feature objectid : " + obj.OID.ToString());

                        }
                    }
                }

                //Break Fix 05/08/2019
                //int featureHFTDFieldIndex = ModelNameFacade.FieldIndexFromModelName(((IFeature)obj).Class, SchemaInfo.Electric.FieldModelNames.HFTD);


                /*http://wwwlbgis/arcgis/rest/services/OMS/dms_HighFireThreatDistrict/MapServer/0/query?where=&text=&objectIds=&time=&geometry=%7B%22x%22%3A1443631.21335296%2C%22y%22%3A14549868.156982%7D&geometryType=esriGeometryPoint&inSR=%7B%22wkt%22%3A%22PROJCS%5B%5C%22NAD_1983_UTM_Zone_10N%5C%22%2CGEOGCS%5B%5C%22GCS_North_American_1983%5C%22%2CDATUM%5B%5C%22D_North_American_1983%5C%22%2CSPHEROID%5B%5C%22GRS_1980%5C%22%2C6378137.0%2C298.257222101%5D%5D%2CPRIMEM%5B%5C%22Greenwich%5C%22%2C0.0%5D%2CUNIT%5B%5C%22Degree%5C%22%2C0.0174532925199433%5D%5D%2CPROJECTION%5B%5C%22Transverse_Mercator%5C%22%5D%2CPARAMETER%5B%5C%22False_Easting%5C%22%2C1640416.666666667%5D%2CPARAMETER%5B%5C%22False_Northing%5C%22%2C0.0%5D%2CPARAMETER%5B%5C%22Central_Meridian%5C%22%2C-123.0%5D%2CPARAMETER%5B%5C%22Scale_Factor%5C%22%2C0.9996%5D%2CPARAMETER%5B%5C%22Latitude_Of_Origin%5C%22%2C0.0%5D%2CUNIT%5B%5C%22Foot_US%5C%22%2C0.3048006096012192%5D%5D%22%7D&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=HFTD&returnGeometry=false&returnTrueCurves=false&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&resultOffset=&resultRecordCount=&f=html*/
                //GETHFTD("http://wwwlbgis/arcgis/rest/services/OMS/dms_HighFireThreatDistrict/MapServer/0/query?where=&text=&objectIds=&time=&geometry=%7B%22x%22%3A" + Convert.ToString(((IPoint)((IFeature)obj).Shape).X) + "%2C%22y%22%3A" + Convert.ToString(((IPoint)((IFeature)obj).Shape).Y) + "%7D&geometryType=esriGeometryPoint&inSR=%7B%22wkt%22%3A%22PROJCS%5B%5C%22NAD_1983_UTM_Zone_10N%5C%22%2CGEOGCS%5B%5C%22GCS_North_American_1983%5C%22%2CDATUM%5B%5C%22D_North_American_1983%5C%22%2CSPHEROID%5B%5C%22GRS_1980%5C%22%2C6378137.0%2C298.257222101%5D%5D%2CPRIMEM%5B%5C%22Greenwich%5C%22%2C0.0%5D%2CUNIT%5B%5C%22Degree%5C%22%2C0.0174532925199433%5D%5D%2CPROJECTION%5B%5C%22Transverse_Mercator%5C%22%5D%2CPARAMETER%5B%5C%22False_Easting%5C%22%2C1640416.666666667%5D%2CPARAMETER%5B%5C%22False_Northing%5C%22%2C0.0%5D%2CPARAMETER%5B%5C%22Central_Meridian%5C%22%2C-123.0%5D%2CPARAMETER%5B%5C%22Scale_Factor%5C%22%2C0.9996%5D%2CPARAMETER%5B%5C%22Latitude_Of_Origin%5C%22%2C0.0%5D%2CUNIT%5B%5C%22Foot_US%5C%22%2C0.3048006096012192%5D%5D%22%7D&spatialRel=esriSpatialRelIntersects&relationParam=&outFields=HFTD&returnGeometry=false&returnTrueCurves=false&maxAllowableOffset=&geometryPrecision=&outSR=&returnIdsOnly=false&returnCountOnly=false&orderByFields=&groupByFieldsForStatistics=&outStatistics=&returnZ=false&returnM=false&gdbVersion=&returnDistinctValues=false&resultOffset=&resultRecordCount=&f=json",obj);
                //Break Fix 05/08/2019
                //string[] val1 = val.Split(':');
                //int iTier = 0;
                //for (int i = 0; i < val1.Length; ++i)
                //    if (val1[i].Contains("Tier"))
                //        iTier = Convert.ToInt16(val1[i].Split(' ')[1]);
                //((IFeature)obj).set_Value(featureHFTDFieldIndex, iTier);
            }
            catch (Exception e)
            {
                _logger.Error("PGE Populate HFTD AU failed. Message: " + e.Message + " Stacktrace: " + e.StackTrace);
            }
            finally
            {
                if (pFCursor != null)
                {
                    while (Marshal.ReleaseComObject(pFCursor) != 0) { };
                    pFCursor = null;
                }
                
            }
        }
        #endregion


        #region Private Methods

        //Get HFTD code by HFTD Values/Description
        private string getHFTDCode(string hftdValue)
        {
            //Default Value
            string hftdCode = "0";
            if(string.IsNullOrEmpty(hftdValue))
            {
                hftdCode = "0";
            }
            else
            {
                if (hftdValue.Contains("0")) { hftdCode = "0"; }
                //HFTD/HFRA Project : Adding Zone 1 to HFTD
                if (hftdValue.Contains("1")) { hftdCode = "1"; }
                if (hftdValue.Contains("2")) { hftdCode = "2"; }
                if (hftdValue.Contains("3")) { hftdCode = "3"; }
            }

            return hftdCode;
        }
        
            
        // This Function evaluates whether or not the shape of the object has been changed.        
        private bool ShapeChanged(IObject obj)
        {
            IFeatureChanges featureChanges = obj as IFeatureChanges;
            return featureChanges != null && featureChanges.ShapeChanged;

        }       

        //get workspace from sde
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }

        #endregion

       

        //#region Private Methods

        //private void GETHFTD(string url,IObject obj)
        //{
        //    int featureHFTDFieldIndex = ModelNameFacade.FieldIndexFromModelName(((IFeature)obj).Class, SchemaInfo.Electric.FieldModelNames.HFTD);
        //    int iCount = 0;
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //Again:
        //    try
        //    {
        //        WebResponse response = request.GetResponse();
        //        using (Stream responseStream = response.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
        //            //Break Fix 05/08/2019
        //            //return reader.ReadToEnd();
        //            string[] val1 = reader.ReadToEnd().Split(':');
        //            int iTier = 0;
        //            for (int i = 0; i < val1.Length; ++i)
        //                if (val1[i].Contains("Tier"))
        //                    iTier = Convert.ToInt16(val1[i].Split(' ')[1]);
        //            ((IFeature)obj).set_Value(featureHFTDFieldIndex, iTier);
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        if (iCount < 3)
        //        {
        //            ++iCount;
        //            Thread.Sleep(1000);
        //            goto Again;
        //        }
        //        WebResponse errorResponse = ex.Response;
        //        using (Stream responseStream = errorResponse.GetResponseStream())
        //        {
        //            StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
        //            String errorText = reader.ReadToEnd();
        //            // log errorText
        //        }
        //        throw;
        //    }
        //}

        //#endregion

       
    }
}
