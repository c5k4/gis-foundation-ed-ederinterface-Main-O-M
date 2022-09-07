using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.UFM;

using Miner.ComCategories;
using Miner.Interop;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [Guid("D144B1C4-A9D0-471f-A108-29BBBFAB9CA9")]
    [ProgId("PGE.Desktop.EDER.SetDuctAttributesAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class SetDuctAttributesAU : BaseSpecialAU
    {
        #region Member vars

        // For debug logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Constructor

        public SetDuctAttributesAU() : base("PGE Set Duct Attributes AU") 
        {
        }

        #endregion

        #region Special AU overrides

        /// <summary>
        /// Returns true if the supplied object is a duct and were 'creating'
        /// </summary>
        /// <param name="pObjectClass"></param>
        /// <param name="eEvent"></param>
        /// <returns></returns>
        protected override bool InternalEnabled(IObjectClass pObjectClass, mmEditEvent eEvent)
        {
            bool enable = false;

            // If were updating...
            if (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate)
            {
                // And its a conduit feature
                if (ModelNameFacade.ContainsClassModelName(pObjectClass, SchemaInfo.UFM.ClassModelNames.UfmDuct) == true)
                {
                    // Were enabled
                    enable = true;
                }
            }

            // Return the result
            return enable;
        }

        /// <summary>
        /// Copies custom fields for the duct from the related conduits blob field to itself
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="eAUMode"></param>
        /// <param name="eEvent"></param>
        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            // Log entry
            _logger.Debug("PgeDuctConduitSyncAU fired");
            _logger.Debug("Feature Class: " + pObject.Class.AliasName);
            _logger.Debug("OID          : " + pObject.OID);

            try
            {
                // Get the conduit for the current duct
                IFeature conduit = GetConduitForDuctFeature(pObject);

                // If one was found...
                if (conduit != null)
                {
                    // Open its blob field
                    IMMDuctBankConfig ductBankConfig = UfmHelper.GetDuctBankConfig(conduit);

                    // Update the corresponding duct feature class with the attribution from the blob
                    if (ductBankConfig != null)
                    {
                        // Get the duct number for the supplied duct
                        int ductPosition = GetDuctPosition(pObject);

                        // If we determined the position...
                        if (ductPosition > 0)
                        {
                            // Get the duct definition record for that position
                            IMMDuctDefinition ductDefinition = GetDuctDefinition(ductBankConfig, ductPosition);

                            // If we found one...
                            if (ductDefinition != null)
                            {
                                // Update the duct from that definition record
                                UfmSyncDuctAttrHelper helper = new UfmSyncDuctAttrHelper();
                                helper.UpdateDuct(pObject as IFeature, ductDefinition);
                            }
                        }
                    }
                }
                else
                {
                    // Log that there are no related duct bank features
                    _logger.Info("No related conduit feature found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to execute SyncDuctAttributesAU: " + ex.ToString());
            }
        }

        #endregion

        #region Private methods

        private IFeature GetConduitForDuctFeature(IObject pDuct)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            // Assume we won't find a conduit
            IFeature conduit = null;

            try
            {
                // Get the parent duct bank
                IFeature ductBank = GetDuctBank(pDuct as IFeature);

                // Get the duct banks related conduit
                ISet conduits = UfmHelper.GetRelatedObjects(ductBank as IRow, SchemaInfo.UFM.ClassModelNames.Conduit);
                if (conduits.Count > 0)
                {
                    conduit = conduits.Next() as IFeature;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to find conduit for duct: " + ex.ToString());
            }

            // Return the result
            return conduit;
        }

        private int GetDuctPosition(IObject duct)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            // Assume we don't find the position
            int ductPosition = 0;

            try
            {
                // Determine the duct position and get its value
                int ductPosIndex = ModelNameFacade.FieldIndexFromModelName(duct.Class, SchemaInfo.UFM.FieldModelNames.DuctId);
                if (ductPosIndex != -1)
                {
                    ductPosition = int.Parse(duct.get_Value(ductPosIndex).ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to find conduit for duct: " + ex.ToString());
            }

            // Return the result
            return ductPosition;
        }

        private IMMDuctDefinition GetDuctDefinition(IMMDuctBankConfig dbc, int ductPosition)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            // Assume we won't find it
            IMMDuctDefinition matchingDuctDefinition = null;

            try
            {
                // Reset the config
                ID8List dbcList = dbc as ID8List;
                dbcList.Reset();

                // Get the list of ducts
                ID8List ductViewList = (ID8List)dbcList.Next(false);
                ductViewList.Reset();

                // While there are items in the list...
                ID8ListItem listItem = ductViewList.Next(false);
                while (listItem != null)
                {
                    // If its a duct...
                    if (listItem is IMMDuctDefinition)
                    {
                        // Check to see if its for the supplied position
                        IMMDuctDefinition ductDefinition = listItem as IMMDuctDefinition;
                        if (ductDefinition.ductNumber == ductPosition)
                        {
                            // If it is, save and bail out
                            matchingDuctDefinition = ductDefinition;
                            break;
                        }
                    }

                    // Move to the next item in the list
                    listItem = ductViewList.Next(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to sync duct features: " + ex.ToString());
            }

            // Return the result
            return matchingDuctDefinition;
        }

        private IFeature GetDuctBank(IFeature duct)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

            // Assume we won't find one
            IFeature ductBank = null;

            // We'll need a cursor
            IFeatureCursor ductBankCursor = null;

            try
            {
                // Get the duct FC
                IFeatureClass ductBankFc = ModelNameFacade.FeatureClassByModelName((duct.Class as IDataset).Workspace,
                                                                                SchemaInfo.UFM.ClassModelNames.UfmDuctBank);

                if (ductBankFc != null)
                {
                    // Create the spatial filter - we want to check all ducts within the duct bank...
                    ISpatialFilter spatialFilter = new SpatialFilterClass();
                    spatialFilter.Geometry = duct.Shape;
                    spatialFilter.GeometryField = ductBankFc.ShapeFieldName;
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;

                    // Execute the query.
                    ductBankCursor = ductBankFc.Search(spatialFilter, true);
                }

                // Get the first feature found (should only be one)
                if (ductBankCursor != null)
                {
                    ductBank = ductBankCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to find duct from duct bank: " + ex.ToString());
            }
            finally
            {
                // Clean up
                Marshal.ReleaseComObject(ductBankCursor);
            }

            // Return the result
            return ductBank;
        }

        #endregion
    }
}
