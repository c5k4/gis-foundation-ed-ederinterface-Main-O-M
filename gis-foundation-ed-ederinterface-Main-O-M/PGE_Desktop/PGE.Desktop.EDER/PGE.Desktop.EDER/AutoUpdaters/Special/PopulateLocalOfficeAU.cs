using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using System.Collections.Generic;
using System.Linq;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Update Local Office ID field for Electrice feature class.
    /// </summary>
    [ComVisible(true)]
    [Guid("A94DF6C4-F9F5-4D90-B2A2-3DDA0F50B563")]
    [ProgId("PGE.Desktop.EDER.PopulateLocalOfficeAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
   public class PopulateLocalOfficeAU:BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        // Add a sneaky option allowing us to bypass model names lookups. The bypass is disabled by default but callers can turn it on via property
        private static bool _lookupLocalOfficeByModelName = true;
        public static bool LookupLocalOfficeByModelName
        {
            set
            {
                _lookupLocalOfficeByModelName = value;
            }
        }

        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public PopulateLocalOfficeAU() : base("PGE Populate Local Office AU")
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
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
                _logger.Debug("Field model name :" + SchemaInfo.Electric.FieldModelNames.LocalOfficeID + " Found-" + enabled);

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
            IFeatureCursor featCursor = null;
            IFeature localOfficePolygon = null;
            IFeatureClass localOfficeFeatureClass = null;

            try
            {
                IFeature pFeature = obj as IFeature;
                if (_lookupLocalOfficeByModelName == true)
                {
                    localOfficeFeatureClass = ModelNameFacade.FeatureClassByModelName(((IDataset)pFeature.Class).Workspace, SchemaInfo.General.ClassModelNames.LOPC);
                    if (localOfficeFeatureClass == null)
                    {
                        _logger.Debug("Local office feature class with " + SchemaInfo.General.ClassModelNames.LOPC + " class model name assigned could not be found");
                        return;
                    }

                    //Set the query for spatial filter
                    ISpatialFilter spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = pFeature.ShapeCopy;
                    spatialFilter.GeometryField = localOfficeFeatureClass.ShapeFieldName;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    featCursor = localOfficeFeatureClass.Search(spatialFilter, true);
                    localOfficePolygon = featCursor.NextFeature();
                    if (localOfficePolygon != null)
                    {
                        int locOfficeIndex = ModelNameFacade.FieldIndexFromModelName(localOfficeFeatureClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
                        int featureLocOfficeFieldIndex = ModelNameFacade.FieldIndexFromModelName(pFeature.Class, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);

                        object val = localOfficePolygon.get_Value(locOfficeIndex);
                        if (val != null)
                        {
                            pFeature.set_Value(featureLocOfficeFieldIndex, val);
                        }
                        else
                        {
                            _logger.Error("PGE Populate Local Office AU --  cannot update Local Office ID. Local Office ID is null in the LocalOfficePolygon");
                        }
                    }
                }
                else
                {
                    string val = GetLocalOfficeFromDistMap(obj as IFeature);
                    if (val != null)
                    {
                        int featureLocOfficeFieldIndex = ModelNameFacade.FieldIndexFromModelName(pFeature.Class, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
                        pFeature.set_Value(featureLocOfficeFieldIndex, val);
                    }
                    else
                    {
                        _logger.Error("PGE Populate Local Office AU --  cannot update Local Office ID. Local Office ID is null in the LocalOfficePolygon");
                    }

                    //localOfficeFeatureClass = ((obj.Class as IDataset).Workspace as IFeatureWorkspace).OpenFeatureClass("PGE_LOPC");
                }
            }
            catch (Exception e)
            {
                _logger.Error("PGE Populate Local Office AU failed. Message: " + e.Message + " Stacktrace: " + e.StackTrace);
            }
            finally
            {
                if (localOfficePolygon != null) { while (Marshal.ReleaseComObject(localOfficePolygon) > 0) { } }
                if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                if (localOfficeFeatureClass != null) { Marshal.ReleaseComObject(localOfficeFeatureClass); }
            }
        }
        #endregion

        #region Private methods

        private string GetLocalOfficeFromDistMap(IFeature deviceObject)
        {
            _logger.Debug("deviceObject [ " + ((IDataset)deviceObject.Table).Name + " ] OID [ " + deviceObject.OID + " ]");
            IFeatureCursor pFeaCursor = null;
            string localOffice = string.Empty;

            try
            {
                //Checks DistributiMap ObjectClass is assigned with ModelName
                IWorkspace pWorkspace = (deviceObject.Class as IDataset).Workspace;
                IFeatureClass pDistMapClass = ModelNameFacade.FeatureClassByModelName(pWorkspace, SchemaInfo.Electric.ClassModelNames.DistributionMap);
                if (pDistMapClass == null)
                {
                    _logger.Debug(SchemaInfo.Electric.ClassModelNames.DistributionMap + " ModelName is not assigned to DistributionMap objectclass, exiting.");
                    return localOffice;
                }

                // Get the Subtype field name
                string distMapSubtypeFldName = (pDistMapClass as ISubtypes).SubtypeFieldName;

                // Apply Spatial filter using input object on DistributionMap ObjectClass
                ISpatialFilter pSFilter = new SpatialFilterClass();
                pSFilter.Geometry = deviceObject.Shape;
                pSFilter.GeometryField = (deviceObject.Class as IFeatureClass).ShapeFieldName;
                pSFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                pSFilter.WhereClause = distMapSubtypeFldName + "=1"; // Subtype code of plat
                pFeaCursor = pDistMapClass.Search(pSFilter, false);

                // Get the Dist Map Feature List from cursor
                List<DistributionMapFeature> distMapFeatureList = PrepareListFromCursor(pDistMapClass, pFeaCursor);
                distMapFeatureList.Sort();

                //Get the list of unique local office IDs for the lowest scale
                List<string> uniqueLocalOffices = new List<string>();
                int minScale = 999;
                GetLowestScaleLocalOfficeValues(distMapFeatureList, ref uniqueLocalOffices, ref minScale);

                if (uniqueLocalOffices.Count != 1)
                {
                    //Multiple local office values found in the plat_unified feature class.  Let's check out the ElecLocalOffice feature class and compare
                    IFeatureClass LocalOfficePolygons = ((IFeatureWorkspace)pWorkspace).OpenFeatureClass("EDGIS.ElecLocalOffice");
                    pSFilter.WhereClause = "";
                    IFeatureCursor lopcCursor = LocalOfficePolygons.Search(pSFilter, false);
                    List<DistributionMapFeature> lopcFeatures = PrepareListFromCursor(LocalOfficePolygons, lopcCursor);
                    lopcFeatures.Sort();

                    List<string> lopcUniqueLocalOffices = new List<string>();
                    int minScaleLOPC = 10000;
                    GetLowestScaleLocalOfficeValues(lopcFeatures, ref lopcUniqueLocalOffices, ref minScaleLOPC);

                    if (uniqueLocalOffices.Count > 1)
                    {
                        //Set initially to the first item in the local offices found from plat layer
                        localOffice = uniqueLocalOffices[0];

                        //Compare the LOPC local offices with the plat local office values.  If one matches, we'll use that one instead
                        foreach (string lopcLocalOffice in lopcUniqueLocalOffices)
                        {
                            if (uniqueLocalOffices.Contains(lopcLocalOffice)) 
                            {
                                //Go with the first local office that matches
                                localOffice = lopcLocalOffice;
                                break;
                            }
                        }
                    }
                    else if (lopcUniqueLocalOffices.Count > 0)
                    {
                        //Plat grids had no local office matches, so use one found in the LOPC local office polygons
                        localOffice = lopcUniqueLocalOffices[0];
                    }
                }
                else if (uniqueLocalOffices.Count == 1)
                {
                    //Only one plat local office was found at the smallest scale so we will just use this one
                    localOffice = uniqueLocalOffices[0];
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to get local office value from dist maps: " + ex.ToString());
            }
            finally
            {
                if (pFeaCursor != null) { Marshal.ReleaseComObject(pFeaCursor); }
            }

            return localOffice;
        }

        private string GetLocalOfficeFromElop(IWorkspace pWorkspace, ISpatialFilter pSFilter)
        {
            _logger.Debug("GetLocalOfficeFromElop");
            string localOffice = "";

            //Multiple local office values found in the plat_unified feature class.  Let's check out the ElecLocalOffice feature class and compare
            IFeatureClass localOfficePolygons = null;
            IFeatureCursor lopCursor = null;
            IFeature lopFeature = null;

            try
            {
                localOfficePolygons = ((IFeatureWorkspace)pWorkspace).OpenFeatureClass("EDGIS.ElecLocalOffice");
                pSFilter.WhereClause = "";
                lopCursor = localOfficePolygons.Search(pSFilter, false);

                while ((lopFeature = lopCursor.NextFeature()) != null)
                {
                    localOffice = Convert.ToString(lopFeature.get_Value(localOfficePolygons.FindField("LOCALOFFICEID")));
                    _logger.Debug("localOffice [ " + localOffice + " ]");
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (lopFeature != null) Marshal.FinalReleaseComObject(lopFeature);
                if (lopCursor != null) Marshal.FinalReleaseComObject(lopCursor);
                if (localOfficePolygons != null) Marshal.FinalReleaseComObject(localOfficePolygons);
            }

            return localOffice;
        }

        private void GetLowestScaleLocalOfficeValues(List<DistributionMapFeature> distMapFeatures, ref List<string> localOffices, ref int minScale)
        {
            foreach (DistributionMapFeature distMapFeature in distMapFeatures)
            {
                if (distMapFeature.MapType < minScale)
                {
                    minScale = distMapFeature.MapType;
                    localOffices.Add(distMapFeature.LocalOffice);
                }
            }
            localOffices = localOffices.Distinct().ToList();
        }

        /// <summary>
        /// Prepares Map Polygon List from the cusror
        /// </summary>
        /// <param name="mapCursor">Cursor to retrieve the Map Polygon features</param>
        /// <returns>Returns the list of map polygon features</returns>
        private List<DistributionMapFeature> PrepareListFromCursor(IObjectClass pDistMapClass, IFeatureCursor mapCursor)
        {
            // Create a list of map features
            List<DistributionMapFeature> mapFeatureList = new List<DistributionMapFeature>();

            // Get some field indexes
            int localOfficePolyFieldIdx = ModelNameFacade.FieldIndexFromModelName(pDistMapClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.LocalOfficeID);
            if (localOfficePolyFieldIdx < 0 && !PopulateLocalOfficeAU._lookupLocalOfficeByModelName) { localOfficePolyFieldIdx = pDistMapClass.FindField("LOCALOFFICEID"); }
            if (localOfficePolyFieldIdx == -1)
            {
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.LocalOfficeID + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                return mapFeatureList;
            }

            int mapTypeFieldIdx = ModelNameFacade.FieldIndexFromModelName(pDistMapClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.MapType);
            if (mapTypeFieldIdx < 0 && !PopulateLocalOfficeAU._lookupLocalOfficeByModelName) { mapTypeFieldIdx = pDistMapClass.FindField("MAPTYPE"); }
            if (mapTypeFieldIdx == -1)
            {
                //Try and use the actual MAPTYPE field name
                mapTypeFieldIdx = pDistMapClass.FindField("MAPTYPE");
                if (mapTypeFieldIdx == -1)
                {
                    _logger.Debug(SchemaInfo.Electric.FieldModelNames.MapType + " - FieldModelname is not assigned for DistributionMap objectclass, exiting.");
                    return mapFeatureList;
                }
            }

            if (mapCursor == null) return mapFeatureList;
            IFeature mapFeature = null;

            DistributionMapFeature distMapFeature = null;

            // For each polygon
            while ((mapFeature = mapCursor.NextFeature()) != null)
            {
                // Build a dist map feature
                distMapFeature = new DistributionMapFeature();
                //distMapFeature.MapNumber = mapFeature.get_Value(_mapNoFieldIdx).Convert<string>(string.Empty);
                //distMapFeature.MapOffice = mapFeature.get_Value(_mapOfficeFldIx).Convert<string>(string.Empty);
                distMapFeature.LocalOffice = mapFeature.get_Value(localOfficePolyFieldIdx).Convert<string>(string.Empty).ToUpper();

                try { distMapFeature.MapType = mapFeature.get_Value(mapTypeFieldIdx).Convert<int>(0); }
                catch { distMapFeature.MapType = 0; }

                //If the MapType=0, then it is "Other" map type ( 0 = Domain code and "Other" = Domain Description).
                //This should be treated with least priority if any MapPolygon exists scale whose MapType is not "Other"
                if (distMapFeature.MapType == 0)
                {
                    if (distMapFeature.MapType == 0) distMapFeature.MapType = 9999; //This is set just for sorting purpose
                }                

                // Add to the list
                mapFeatureList.Add(distMapFeature);
            }

            // Return the resulting list
            return mapFeatureList;
        }

        #endregion
    }
}
