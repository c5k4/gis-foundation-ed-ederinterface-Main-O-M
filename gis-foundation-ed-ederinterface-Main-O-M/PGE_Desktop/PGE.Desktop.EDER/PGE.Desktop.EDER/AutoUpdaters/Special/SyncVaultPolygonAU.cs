using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using Miner.ComCategories;
using Miner.Interop;
using Miner.Geodatabase.Edit;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("E9ADDA3B-F3C7-447f-8BE8-336C3B33349B")]
    [ProgId("PGE.Desktop.EDER.SyncVaultPolygonAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class SyncVaultPolygonAU : BaseSpecialAU
    {
        #region Member vars

        // For debug logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SumUnitCountsAU"/> class.  
        /// </summary>
        public SyncVaultPolygonAU() : base("PGE Sync Vault Poly AU") 
        {
        }

        #endregion

        #region Special AU overrides

        /// <summary>
        /// Returns true if the supplied object is a subsurface structure and were updating
        /// </summary>
        /// <param name="pObjectClass"></param>
        /// <param name="eEvent"></param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, Miner.Interop.mmEditEvent eEvent)
        {
            bool enable = false;

            // If were updating...
            if (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate)
            {
                // And its a subsurface structure feature
                if (ModelNameFacade.ContainsClassModelName(pObjectClass, SchemaInfo.Electric.ClassModelNames.SubSurfaceStructure) == true)
                {
                    // Were enabled
                    enable = true;
                }
            }

            // Return the result
            return enable;
        }

        /// <summary>
        /// When a subsurface structures structure number is edited, any vault polygons with that structure number
        /// must be updated to reflect the new structure number.
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="eAUMode"></param>
        /// <param name="eEvent"></param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            IFeatureCursor cursorVaultPolys = null;

            // Log entry
            _logger.Debug("SyncVaultPolyAU fired");
            _logger.Debug("Feature Class: " + pObject.Class.AliasName);
            _logger.Debug("OID          : " + pObject.OID);

            try
            {
                // If the structure number was edited...
                IRowChanges rowChanges = pObject as IRowChanges;
                int subsurfaceStructNumIndex = ModelNameFacade.FieldIndexFromModelName(pObject.Class, SchemaInfo.UFM.FieldModelNames.StructureNumber);
                if (rowChanges.get_ValueChanged(subsurfaceStructNumIndex) == true)
                {
                    // Get the original subsurface structure number
                    string origStructNum = GetOriginalValue(rowChanges, subsurfaceStructNumIndex);
                    object structGuid = UfmHelper.GetFieldValue(pObject as IRow, SchemaInfo.UFM.FieldModelNames.FacilityId);
                    string structureGuid = string.Empty;
                    if (structGuid != null)
                    {
                        structureGuid = structGuid.ToString();
                    }

                    // We can only update vault polys if the structure number was set
                    if (structureGuid != string.Empty)
                    {
                        // Query for any vault polys with that number on them
                        IWorkspace ws = (pObject.Class as IDataset).Workspace;
                        IFeatureClass fcVaultPolys = ModelNameFacade.FeatureClassByModelName(ws, SchemaInfo.UFM.ClassModelNames.VaultPoly);
                        cursorVaultPolys = GetVaultPolysForUpdate(fcVaultPolys, structureGuid);
                        if (cursorVaultPolys != null)
                        {
                            // Get the new subsurface structure number
                            string newStructureNumber = GetNewStructureNumber(pObject);

                            // Update the vault poly features with the new structure number value
                            int vaultPolyStructNumIndex = ModelNameFacade.FieldIndexFromModelName(fcVaultPolys, SchemaInfo.UFM.FieldModelNames.StructureNumber);
                            UpdateFeatures(cursorVaultPolys, vaultPolyStructNumIndex, newStructureNumber);
                        }
                    }
                    else
                    {
                        // Log a warning
                        _logger.Warn("Structure number was not set on subsurface structure, so unable to update vault polygons");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't raise an error (should this display message if in ArcMap and cancel the edit?)
                _logger.Error("Failed to execute sync vault polygons: " + ex.ToString());
            }
            finally
            {
                // Clean-up
                if (cursorVaultPolys != null)
                {
                    Marshal.ReleaseComObject(cursorVaultPolys);
                }
            }
        }

        #endregion Special AU Overrides

        #region Private methods

        private string GetOriginalValue(IRowChanges rowChanges, int fieldIndex)
        {
            string fieldValue = string.Empty;

            // Get the original subsurface structure number
            object value = rowChanges.get_OriginalValue(fieldIndex);
            if (value != null)
            {
                fieldValue = value.ToString();
            }

            return fieldValue;
        }

        private IFeatureCursor GetVaultPolysForUpdate(IFeatureClass fcVaultPolys, string subStructKey)
        {
            // Caller must clean this up
            IFeatureCursor cursorVaultPolys = null;

            try
            {
                // Filter for vault polys with the supplied structure number
                IQueryFilter queryVaultPolys = new QueryFilterClass();
                IField subStructKeyField = ModelNameFacade.FieldFromModelName(fcVaultPolys as IObjectClass, SchemaInfo.General.FieldModelNames.SubStructGUID);
                queryVaultPolys.WhereClause = subStructKeyField.Name + "='" + subStructKey + "'";

                // Execute the query
                cursorVaultPolys = fcVaultPolys.Update(queryVaultPolys, false);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get Vault polygons: " + ex.ToString());
                throw ex;
            }

            // Return the result
            return cursorVaultPolys;
        }

        private string GetNewStructureNumber(IObject subsurfaceStructure)
        {
            string newStructureNumber = string.Empty;

            // Get the field value and convert to a string
            object newNumber = UfmHelper.GetFieldValue(subsurfaceStructure as IRow, SchemaInfo.UFM.FieldModelNames.StructureNumber);            
            if (newNumber != null)
            {
                newStructureNumber = newNumber.ToString();
            }

            // Return the result
            return newStructureNumber;
        }

        private void UpdateFeatures(IFeatureCursor cursorFeatures, int fieldIndex, string fieldValue)
        {
            try
            {
                // Update their structure numbers to match
                IFeature feature = cursorFeatures.NextFeature();
                while (feature != null)
                {
                    // Update the supplied field with the supplied value
                    feature.set_Value(fieldIndex, fieldValue);
                    feature.Store();

                    // Move to the next feature
                    feature = cursorFeatures.NextFeature();
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to update features: " + ex.ToString());
                throw ex;
            }
        }

        #endregion
    }
}
