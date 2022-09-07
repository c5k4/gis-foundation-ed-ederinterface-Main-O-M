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
    /// Update HFRA field for Electrice feature class.
    /// </summary>
    [ComVisible(true)]
    [Guid("42E98BBF-F704-4FC9-A381-6EDB40CC7E80")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateHFRAIDC")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEUpdateHFRAIDC : BaseSpecialAU
    {
        #region Varriables 
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public static string HFRALayerName = "LBGIS.HighFireRiskArea";
        public static string HFRAIDCFieldName = "HFRA";
        public static string HFTDLayerName = "LBGIS.HighFireThreatDist";
        public static string HFTDFieldName = "HFTD";
        private static string lbgis_sde_path ;
        private static IWorkspace lbWksp ;
        #endregion
        
        #region AU Name

        public PGEUpdateHFRAIDC()
            : base("PGE Update HFRAIDC AU")
        {
        }
        #endregion

        #region Base Special AU Overrides
        /// <summary>
        /// Determines in which class the AU will be enabled
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {           
            return true;
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

            try
            {
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
                        IFeatureCursor pFCursorHFRA = null;
                        IFeatureCursor pFCursorHFTD = null;
                        int featureHFRAIDCFieldIndex = obj.Fields.FindField(HFRAIDCFieldName);

                            string HFRA_Availble = "N";
                            string HFTD_Available = "N";
                            IFeatureClass HFRAIDCfeatureClass = lbgisWkp.OpenFeatureClass(HFRALayerName);
                            if (HFRAIDCfeatureClass != null)
                            {                                
                                ISpatialFilter pSFilter = new SpatialFilterClass();
                                pSFilter.Geometry = (obj as IFeature).Shape;                                
                                pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                pFCursorHFRA = HFRAIDCfeatureClass.Search(pSFilter, true);
                                if (pFCursorHFRA != null)
                                {
                                    IFeature HFRAFeat = null;
                                    if ((HFRAFeat = pFCursorHFRA.NextFeature()) != null)
                                    {
                                        HFRA_Availble = "Y";
                                    } 
                                }
                                //Releasing cursor
                                if (pFCursorHFRA != null)
                                {
                                    Marshal.FinalReleaseComObject(pFCursorHFRA); pFCursorHFRA = null;
                                }                               
                              }

                            IFeatureClass HFTDfeatureClass = lbgisWkp.OpenFeatureClass(HFTDLayerName);
                            if (HFTDfeatureClass != null)
                            {                                
                                ISpatialFilter pSFilter = new SpatialFilterClass();
                                pSFilter.Geometry = (obj as IFeature).Shape;
                                pSFilter.SubFields = HFTDFieldName;
                                pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                                pFCursorHFTD = HFTDfeatureClass.Search(pSFilter, true);                                
                                if (pFCursorHFTD != null)
                                {
                                    IFeature hftdFeat = null;
                                    int HFTDindex = pFCursorHFTD.FindField(HFTDFieldName);
                                    if ((hftdFeat = pFCursorHFTD.NextFeature()) != null)
                                    {
                                        HFTD_Available = "Y";
                                    }
                                }
                                //Releasing cursor
                                if (pFCursorHFTD != null)
                                {
                                    Marshal.FinalReleaseComObject(pFCursorHFTD); pFCursorHFTD = null;
                                } 
                            }
                            //Getting HFRA IDC field value
                            string HFRAIDC_Value = getHFRAIDCValue(HFRA_Availble, HFTD_Available);
                            //Setting HFRA IDC value
                            ((IFeature)obj).set_Value(featureHFRAIDCFieldIndex, HFRAIDC_Value);
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("PGE Update HFRAIDC AU. Message: " + e.Message + " Stacktrace: " + e.StackTrace);
            }            
        }

        #endregion

        #region Private Methods

        //Method to get HFRAIDC field value
        private string getHFRAIDCValue(string HFRA_Availble, string HFTD_Available)
        {
            string HFRAIDC = string.Empty;
            if (HFRA_Availble.Equals("N") && HFTD_Available.Equals("N")) { HFRAIDC = "N"; }
            if (HFRA_Availble.Equals("Y") && HFTD_Available.Equals("N")) { HFRAIDC = "A"; }
            if (HFRA_Availble.Equals("N") && HFTD_Available.Equals("Y")) { HFRAIDC = "R"; }
            if (HFRA_Availble.Equals("Y") && HFTD_Available.Equals("Y")) { HFRAIDC = "B"; }
            return HFRAIDC;
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

    }
}

