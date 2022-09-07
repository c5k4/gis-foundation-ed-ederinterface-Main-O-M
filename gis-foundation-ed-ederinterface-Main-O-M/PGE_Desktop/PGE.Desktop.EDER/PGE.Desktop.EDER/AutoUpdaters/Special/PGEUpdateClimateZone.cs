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
    /// Update Climate Zone field for Transfomer.
    /// </summary>
    [ComVisible(true)]
    [Guid("DF6238DF-288A-4B98-89CD-271230D32F0F")]
    [ProgId("PGE.Desktop.EDER.PGEUpdateClimateZone")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEUpdateClimateZone : BaseSpecialAU
    {
       private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // Add a sneaky option allowing us to bypass model names lookups. The bypass is disabled by default but callers can turn it on via property
        private static bool _lookupClimateZoneByModelName = true;
        //Referring Climate Zone data from Landbase
        private string ClimateZoneLayerName = "LBGIS.ElecClimateZone";
        private string ClimateZoneFieldName = "CZ_CODE";        
        private static string lbgis_sde_path ;
        private static IWorkspace lbWksp ;
        
        public static bool LookupClimateZoneByModelName
        {
            set
            {
                _lookupClimateZoneByModelName = value;
            }
        }

        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public PGEUpdateClimateZone()
            : base("PGE Populate ClimateZone AU")
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
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.ClimateZone);
                _logger.Debug("Field model name :" + SchemaInfo.Electric.FieldModelNames.ClimateZone + " Found-" + enabled);

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
            //Skip the step to find ClimateZone if the shape has not been changed...
            //if ((eEvent == mmEditEvent.mmEventFeatureUpdate) && (ShapeChanged(obj) == false))
            //{
            //    _logger.Info("Feature shape is not changed, hence ClimateZone is not fetching from landbase");
            //    return;
            //}
            
            try
            {
                string ClimateZoneValue = string.Empty;
                string ClimateZoneCode = string.Empty;
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
                        IFeatureClass ClimateZonefeatureClass = lbgisWkp.OpenFeatureClass(ClimateZoneLayerName);
                        if (ClimateZonefeatureClass != null)
                        {
                            ISpatialFilter pSFilter = new SpatialFilterClass();
                            pSFilter.Geometry = (obj as IFeature).Shape;
                            pSFilter.SubFields = ClimateZoneFieldName;
                            pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            pFCursor = ClimateZonefeatureClass.Search(pSFilter, true);
                            IFeature ClimateZoneFeat = pFCursor.NextFeature();
                            int ClimateZoneindex = pFCursor.FindField(ClimateZoneFieldName);
                            if (ClimateZoneFeat != null)
                            {
                                ClimateZoneValue = ClimateZoneFeat.get_Value(ClimateZoneindex).ToString();
                            }                            
                            int featureClimateZoneFieldIndex = ModelNameFacade.FieldIndexFromModelName(((IFeature)obj).Class, SchemaInfo.Electric.FieldModelNames.ClimateZone);
                            //For climate zone value Null :  ??
                            if (ClimateZoneValue.Equals(DBNull.Value) || string.IsNullOrEmpty(ClimateZoneValue))
                            {
                                //
                            }
                            //For climate zone value 1 : ??
                            if (ClimateZoneValue.Equals("1"))
                            {
                                //
                            }
                            ((IFeature)obj).set_Value(featureClimateZoneFieldIndex, ClimateZoneValue);
                            _logger.Info("ClimateZone value : " + ClimateZoneValue + " updated for Feature objectid : " + obj.OID.ToString());

                        }
                        else
                        {
                            _logger.Debug("Climate zone feature class : " + ClimateZoneLayerName + " could not be opened");
                            return;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                _logger.Error("PGE Populate ClimateZone AU failed. Message: " + e.Message + " Stacktrace: " + e.StackTrace);
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
