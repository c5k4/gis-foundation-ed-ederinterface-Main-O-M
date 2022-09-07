using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Synchronizes edits made to custom DuctDefinition fields within the conduit configuration (blob) to
    /// any duct feature classes on walls at each end of the conduit.
    /// </summary>
    [Guid("94C88EF8-05BF-4003-BAE4-81E093AE7234")]
    [ProgId("PGE.Desktop.EDER.SyncDuctAttributesAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ComVisible(true)]
    public class SyncDuctAttributesAU : BaseSpecialAU
    {
        #region Member vars

        // For debug logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SumUnitCountsAU"/> class.  
        /// </summary>
        public SyncDuctAttributesAU() : base("PGE Sync Duct Attributes AU") 
        {
        }

        #endregion

        #region Special AU overrides

        /// <summary>
        /// Returns true if the supplied object is a conduit and were editing
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
                // And its a conduit feature
                if (ModelNameFacade.ContainsClassModelName(pObjectClass, SchemaInfo.UFM.ClassModelNames.Conduit) == true)
                {
                    // Were enabled
                    enable = true;
                }
            }

            // Return the result
            return enable;
        }

        /// <summary>
        /// Copies custom fields from DuctDefinition marked with the PGE_DUCTSYNCATTR model name to any
        /// duct features representing the conduit configuration.
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="eAUMode"></param>
        /// <param name="eEvent"></param>
        protected override void InternalExecute(IObject pObject, Miner.Interop.mmAutoUpdaterMode eAUMode, Miner.Interop.mmEditEvent eEvent)
        {
            // Log entry
            _logger.Debug("SyncDuctAttributesAU fired");
            _logger.Debug("Feature Class: " + pObject.Class.AliasName);
            _logger.Debug("OID          : " + pObject.OID);

            try
            {
                // Get any related duct banks
                ISet objSet = UfmHelper.GetRelatedObjects(pObject, SchemaInfo.UFM.ClassModelNames.UfmDuctBank);

                // If there are some...
                if (objSet.Count > 0)
                {
                    // Assume they have ducts as well, lets open the blob
                    IMMDuctBankConfig ductBankConfig = UfmHelper.GetDuctBankConfig(pObject as IFeature);

                    // Update the corresponding duct feature class with the attribution from the blob
                    if (ductBankConfig != null)
                    {
                        ProcessDucts(ductBankConfig, objSet);
                    }
                }
                else
                {
                    // Log that there are no related duct bank features
                    _logger.Info("No related duct bank features found");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to execute SyncDuctAttributesAU: " + ex.ToString());
            }
        }

        #endregion Special AU Overrides

        #region Private methods

        private void ProcessDucts(IMMDuctBankConfig dbc, ISet ductBanks)
        {
            // Log entry
            string method = MethodBase.GetCurrentMethod().Name;
            _logger.Debug("Entered " + method);

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
                        // Update any matching duct features from that duct
                        UfmSyncDuctAttrHelper syncHelper = new UfmSyncDuctAttrHelper();
                        syncHelper.UpdateDuctInDuctBanks(ductBanks, listItem as IMMDuctDefinition);
                    }

                    // Move to the next item in the list
                    listItem = ductViewList.Next(false);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to sync duct features: " + ex.ToString());
            }
        }

        #endregion
    }
}
