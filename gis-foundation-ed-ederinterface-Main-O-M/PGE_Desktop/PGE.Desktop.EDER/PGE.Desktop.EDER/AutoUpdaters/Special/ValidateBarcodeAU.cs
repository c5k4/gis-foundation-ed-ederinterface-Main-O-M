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
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Validate Barcode field for SupportStructure.
    /// </summary>
    [ComVisible(true)]
    [Guid("31B5753A-CCDF-49CA-937A-96079438F50F")]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.ValidateBarcodeAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class ValidateBarcodeAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public ValidateBarcodeAU()
            : base("PGE Validate Barcode AU")
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
                enabled = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.Barcode);
                _logger.Debug("Field model name :" + SchemaInfo.Electric.FieldModelNames.Barcode + " Found-" + enabled);

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
            List<int> fldIndex = new List<int>();

            //Get the Barcode field index from model name
            fldIndex = ModelNameFacade.FieldIndicesFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.Barcode);
            object fldValue = obj.GetFieldValue(obj.Fields.get_Field(fldIndex[0]).Name, false, SchemaInfo.Electric.FieldModelNames.Barcode).Convert<object>(System.DBNull.Value);
             
            if(!ValidateBarcode(obj.OID, fldValue, 9))
                throw new COMException("Barcode length must be 9 digits, Cancelling the edits.", (int)mmErrorCodes.MM_E_CANCELEDIT);

        }
        #endregion

        #region Private Methods

        // This function checks if Barcode is Numaric & is 9 digit or not.
        private bool ValidateBarcode(int OID, object objBarcode, int digitLimit)
        {            
            bool returnResult = false;            
            if (objBarcode.ToString() != String.Empty)
            {
                if (objBarcode.ToString().Length == digitLimit)
                {
                    if (objBarcode.ToString().All(char.IsDigit))
                    {                        
                        returnResult = true;
                        return returnResult;
                    }                    
                }                
            }
            else
            {
                returnResult = true;
                return returnResult;
            }
            return returnResult;
        }       

        #endregion
    }
}
