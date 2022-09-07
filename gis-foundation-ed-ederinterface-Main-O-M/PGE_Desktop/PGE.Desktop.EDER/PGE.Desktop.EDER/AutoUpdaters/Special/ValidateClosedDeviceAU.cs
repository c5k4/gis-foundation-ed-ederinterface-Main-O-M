using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using log4net;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Validates the conductors associated to a Switchable Device for Energizing standards
    /// </summary>
    [ComVisible(true)]
    [Guid("73794BA5-8D42-406E-8432-A581D716A616")]
    [ProgId("PGE.Desktop.EDER.ValidateClosedDeviceAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class ValidateClosedDeviceAU : BaseSpecialAU
    {
        #region Constructor

        /// <summary>
        /// Initilizes the instance of <see cref="ValidateClosedDeviceAU"/> class
        /// </summary>
        public ValidateClosedDeviceAU()
            : base("PGE Validate Closed Device")
        { }

        #endregion Constructor

        #region Overridden BaseSpecialAU methods

        /// <summary>
        /// Returns true if the EditEvent is either FeatureCreate or FeatureUpdate and the ObjectClass is assigned with required modelname
        /// </summary>
        /// <param name="objectClass">Object Class to validate against for the availability of the Autoupdater</param>
        /// <param name="eEvent">AU Event mode to be validated against</param>
        /// <returns>Returns true if the EditEvent is either FeatureCreate or FeatureUpdate and the ObjectClass is assigned with required modelname; false, otherwise</returns>
        protected override bool InternalEnabled(ESRI.ArcGIS.Geodatabase.IObjectClass objectClass, Miner.Interop.mmEditEvent eEvent)
        {
            bool enabled = false;
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.Switchable);
            }
            return enabled;
        }

        /// <summary>
        /// Returns true if AU is fired in ArcMap mode
        /// </summary>
        /// <param name="eAUMode">Autoupdater execution mode</param>
        /// <returns>Returns true if AU is fired in ArcMap mode; false, otherwise</returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            return (eAUMode == mmAutoUpdaterMode.mmAUMArcMap);
        }

        /// <summary>
        /// Executed when an AU fired and validates the Conductors associated with the Switchable Device for the Energizing standards
        /// </summary>
        /// <param name="obj">Switchable Device object</param>
        /// <param name="eAUMode">AU firing mode</param>
        /// <param name="eEvent">AU event mode</param>
        /// <remarks>
        /// Validates the Conductors associated with the Switchable Device for the Energizing standards and cancesl the edit task if the validation fails
        /// </remarks>
        protected override void InternalExecute(ESRI.ArcGIS.Geodatabase.IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            //validate for the SwitchableDevice model names again
            if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.Switchable) == false) return;
            //Check if Shape is updated

            //Check the Device Status and Proceed only in case of 'Closed' status devices
            if (ValidateClosedDevice.IsDeviceClosed(obj, eEvent == mmEditEvent.mmEventFeatureUpdate) == false) return;

            //Validate the connecting conductors for the equality of the fields : NumberOfPhases, PhaseDesignation, operatingVoltage
            string errorMessage;
            if (ValidateClosedDevice.IsValid(obj, out errorMessage)) return;
            else
            {
                //Prompt the user and stop the editing task
                if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
                {
                    MessageBox.Show(errorMessage, "Energizing Problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                throw new COMException(errorMessage, (int)mmErrorCodes.MM_E_CANCELEDIT);
            }
        }

        #endregion Overridden BaseSpecialAU methods
    }
}
