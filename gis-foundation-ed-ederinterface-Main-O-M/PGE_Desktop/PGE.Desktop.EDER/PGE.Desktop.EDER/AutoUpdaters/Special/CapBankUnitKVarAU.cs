// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing-General - EDER Component Specification
// Shaikh Rizuan 09/28/2012	Created
// </history>
// All rights reserved. 
// ========================================================================
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Update Total KVAR field for Capacitor Bank.
    /// </summary>
    [ComVisible(true)]
    [Guid("E7A24F4F-782F-439F-B137-ABAFB85AF9F7")]
    [ProgId("PGE.Desktop.EDER.CapBankUnitKVarAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
   public class CapBankUnitKVarAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Constructor, pass in name.
        /// </summary>
        /// 

        public CapBankUnitKVarAU():base("PGE Calculate CapBank Unit KVAR AU")
        {
        }

        #region Base special AU Overrides
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
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.CapacitorBank);
                _logger.Debug("Class model name :" + SchemaInfo.Electric.ClassModelNames.CapacitorBank + " Found-" + enabled);
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
            if ( eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
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
            int totalCountKvar;            
            IObjectClass objClass = obj.Class;
            
            // Get the Field Index for following fields
            int unitKvarIndx = obj.Fields.FindField(SchemaInfo.Electric.UnitKvar);
            int unitCountIndx = obj.Fields.FindField(SchemaInfo.Electric.UnitCount);
            //IField detectionSchemeFld = resolver.FieldFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.CapBankTotalKVAR);
            int totalCountIndx = ModelNameFacade.FieldIndexFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.CapBankTotalKVAR);
            if (unitKvarIndx != -1 && unitCountIndx != -1 && totalCountIndx != -1)
            {
                // get the value for fields
                int intUnitKvar;
                int intUnitCount;
                object unitKvarValue = obj.get_Value(unitKvarIndx);
                object unitCountValue = obj.get_Value(unitCountIndx);
                if (unitKvarValue != System.DBNull.Value && !string.IsNullOrEmpty(unitKvarValue.ToString()) && unitCountValue != System.DBNull.Value && !string.IsNullOrEmpty(unitCountValue.ToString()))
                {
                    
                    // Get the totalCount Kvar
                    // totalCountIndx = obj.Fields.FindField(detectionSchemeFld.Name);
                   bool unitKvar = int.TryParse(unitKvarValue.ToString(),out intUnitKvar);
                   bool unitCount = int.TryParse(unitCountValue.ToString(), out intUnitCount);

                    //totalCountKvar = Convert.ToInt32(unitCountValue) * Convert.ToInt32(unitKvarValue);
                   if (unitKvar == true && unitCount == true)
                   {
                       totalCountKvar = intUnitKvar * intUnitCount;
                       _logger.Debug("totalCountKvar: " +totalCountKvar.ToString());
                       obj.set_Value(totalCountIndx, (object)totalCountKvar);
                   }
                   else
                   {
                       _logger.Debug("unitKvar: " + unitKvar + ",unitCount: " + unitCount + ". Could not able to convert to integer.");
                   }

                    
                }
                else
                {
                    _logger.Debug("Verify that UNIT KVAR or UNIT COUNT Field value is not <Null>.");
                   // throw new COMException("Verify that UNIT KVAR or UNIT COUNT Field value is not null.", (int)mmErrorCodes.MM_S_NOCHANGE); 

                }
            }
            else
            {
                _logger.Warn("Verify that UNIT KVAR and UNIT COUNT Field is present and Model Name: " + SchemaInfo.Electric.UnitKvar + " and " + SchemaInfo.Electric.UnitCount + " assigned to TOTAL KVAR Field.");
               // throw new COMException("Verify that UNIT KVAR and UNIT COUNT Field is present and Model Name assigned to TOTAL KVAR Field.", (int)mmErrorCodes.MM_S_NOCHANGE); 
            }

        }
        #endregion


    }
}
