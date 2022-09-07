using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geometry;
using Miner.Framework;
using PGE.Desktop.EDER.UFM;
using ESRI.ArcGIS.ArcMapUI;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("811B673C-F7E2-4F25-88AC-2BFD0969D083")]
    [ProgId("PGE.Desktop.EDER.UfmSyncFacilityIdsAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class UfmSyncFacilityIdsAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public UfmSyncFacilityIdsAU() : base("PGE Sync Facility Ids AU")
        {
        }

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            return eEvent == mmEditEvent.mmEventFeatureCreate && objectClass is IFeatureClass;
        }

        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            return eAUMode == mmAutoUpdaterMode.mmAUMArcMap || eAUMode == mmAutoUpdaterMode.mmAUMSplit;
        }

        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            try
            {
                string facIdNewVal = GetFacilityId();
                if (facIdNewVal.Equals(String.Empty)) return;

                IFeature facIdFt = (IFeature) pObject;

                int facIdIdx = ModelNameFacade.FieldIndexFromModelName(pObject.Class,
                    SchemaInfo.UFM.FieldModelNames.FacilityId);

                facIdFt.Value[facIdIdx] = facIdNewVal;
            }
            catch (Exception e)
            {
                Logger.Error("Error executing sync facility id AU. Message: " + e.Message);
            }
        }

        private string GetFacilityId()
        {
            string facilityId = string.Empty;

            // Attempt to find based on the map first
            IFeature vault = GetVault();
            if (vault != null)
            {
                object facId = ArcFMUtilities.GetFieldValueFromFeature(vault, SchemaInfo.UFM.FieldModelNames.FacilityId);
                if (facId != null)
                {
                    facilityId = facId.ToString();
                }
            }

            // If that fails, check the filter
            if (facilityId == string.Empty)
            {
                facilityId = VaultFilter.FilteredVault;
            }

            Logger.Warn("UfmSyncFacilityIdsAU found facilityID: " + facilityId);
            return facilityId;
        }

        private IFeature GetVault()
        {
            // Assume there is more than one
            IFeature vault = null;

            try
            {
                // Get the current screen envelope
                IMxDocument doc = UfmHelper.GetCurrentMxDocument();
                IEnvelope diagram = doc.ActiveView.Extent;

                // Get the number of vaults on display
                IFeatureClass vaultFc = MapUtilities.GetFeatureClassFromLayersByModelName("MANHOLEFEATURESNAP");
                int vaultCount = MapUtilities.FindFeatureCountSpatially(diagram, vaultFc);

                // If its just one, life is good
                if (vaultCount == 1)
                {
                    IFeatureCursor vaults = ArcFMUtilities.FindFeaturesSpatially(diagram, vaultFc);
                    vault = vaults.NextFeature();
                    Marshal.FinalReleaseComObject(vaults);
                }
                else
                {
                    if (vaultCount > 1)
                    {
                        IFeatureCursor vaults = ArcFMUtilities.FindFeaturesSpatially(diagram, vaultFc);
                        IFeature currVault = vaults.NextFeature();
                        IFeature selectedVault = null;
                        while (currVault != null)
                        {
                            object structNumObj = UfmHelper.GetFieldValue(currVault as IRow, SchemaInfo.UFM.FieldModelNames.StructureNumber);
                            if (structNumObj != null)
                            {
                                string currVaultNum = structNumObj.ToString();
                                if (currVaultNum != string.Empty)
                                {
                                    if (selectedVault == null)
                                    {
                                        selectedVault = currVault;
                                    }
                                    else
                                    {
                                        selectedVault = null;
                                        break;
                                    }
                                }
                            }
                            currVault = vaults.NextFeature();
                        }

                        if (selectedVault != null)
                        {
                            vault = selectedVault;
                        }
                        Marshal.FinalReleaseComObject(vaults);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to get vault: " + ex.ToString());
            }

            // Return whatever we ended up with
            return vault;
        }
    }
}