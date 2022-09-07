using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;

using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using Miner.ComCategories;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.ArcFM;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// <summary>
    /// Update the job number to the feature at the event onCreateFeature. 
    /// </summary>
    [Guid("13A5CEE8-8FF4-41D6-BD3C-F56310131B47")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    [ProgId("PGE.Desktop.EDER.PGEPopulateLastJobNumberAU")]
    [ComVisible(true)]
    public class PGEPopulateLastJobNumberAU : BaseSpecialAU
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PGEPopulateLastJobNumberAU"/> class.  
        /// </summary>
        public PGEPopulateLastJobNumberAU() : base("PGE Populate Last Job Number AU") { }
        #endregion Constructor

        #region Overridden Base special AU Methods

        /// <summary>
        /// Determines the availability of AU in a class
        /// </summary>
        /// <param name="objectClass"> Object class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns>Returns <c>true</c> to enable the AutoUpdater; otherwise <c>false</c></returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;
            //Enable only on event mmEventFeatureCreate and the objectclass model name is PGE_JOBNUMBER.
            if (eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.FieldModelNames.JobNumber);
                _logger.Debug("Is the class (" + objectClass.AliasName + ") in model name " + SchemaInfo.Electric.FieldModelNames.JobNumber + " = " + enabled.ToString());
            }

            return enabled;
        }

        /// <summary>
        /// Determines whether actually this AU should run, based on the AU Mode.
        /// </summary>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <returns>Returns <c>true</c> to execute the AutoUpdater; otherwise <c>false</c> </returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            return (eAUMode == mmAutoUpdaterMode.mmAUMArcMap);
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            // runs only if the aumode is arcmap and event is featurecreate.
            if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap && eEvent == mmEditEvent.mmEventFeatureCreate)
            {
                IObjectClass objClass = obj.Class;
                //Check the objclass has the modelname "PGE_JOBNUMBER". if not logging the message in debug mode.
                if (ModelNameFacade.ContainsClassModelName(objClass, SchemaInfo.Electric.FieldModelNames.JobNumber))
                {
                    //checking the field with jobnumber is available or not.
                    int indexJobNumber = ModelNameFacade.FieldIndexFromModelName(objClass, SchemaInfo.Electric.FieldModelNames.JobNumber);
                    if (indexJobNumber == -1)
                    {
                        throw new COMException("Field with model name" + SchemaInfo.Electric.FieldModelNames.JobNumber + " not found.", (int)mmErrorCodes.MM_E_CANCELEDIT);
                    }


                    //get the value (jobnumber) from memory and assign to current obj
                    string jobNumberFromMemory = PGE.Desktop.EDER.ArcMapCommands.PopulateLastJobNumberUC.jobNumber;
                    //if the jobnumber is null or empty then show message else assign the value to current obj.
                    object objJobNumber = obj.get_Value(indexJobNumber);
                    if (objJobNumber == null || objJobNumber == DBNull.Value || objJobNumber.ToString() == string.Empty || objJobNumber.ToString() == "0")
                    {
                        if (string.IsNullOrEmpty(jobNumberFromMemory) || jobNumberFromMemory.Trim().Length < 1)
                        {
                            throw new COMException(
                                "Please provide a Job Number within the PG&E Job Number toolbar and press Enter before creating assets.",
                                (int) mmErrorCodes.MM_E_CANCELEDIT);

                        }
                        else
                        {
                            obj.set_Value(indexJobNumber, jobNumberFromMemory);
                        }
                    }
                }
                else
                {
                    _logger.Debug("The class (" + objClass.AliasName + ") does not has model name " + SchemaInfo.Electric.FieldModelNames.JobNumber);
                }

            }
        }


        #endregion Overridden Base special AU Methods
    }

}
