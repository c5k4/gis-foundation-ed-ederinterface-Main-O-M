using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;


using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using log4net;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Ensures the Unique Circuit Id of Circuit Source
    /// </summary>
    [ComVisible(true)]
    [Guid("7322905E-6092-4E48-8CFF-296F7501E664")]
    [ProgId("PGE.Desktop.EDER.UniqueCircuitIdAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class UniqueCircuitIdAU : BaseSpecialAU
    {
        #region Constructor

        /// <summary>
        /// Initializes new instance of <see cref="UniqueCircuitIdAU"/> class
        /// </summary>
        public UniqueCircuitIdAU() : base("PGE Unique Circuit ID") { }

        #endregion Constructor

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #region Overridden BaseSpecialAU Methods

        /// <summary>
        /// Determines whether the AU should be Enabled when the Create/Update object AU category is selected.
        /// </summary>
        /// <param name="pObjectClass">The selected object class.</param>
        /// <param name="eEvent">The AU event type.</param>
        /// <returns>True if condition satisfied else False.</returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            bool enabled = false;

            //Enable only if Feature Event type is either Create or Update 
            if (eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                //Checks whether CircuitSource ModelName assigned to ObjectClass
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.CircuitSource);
                if (enabled)
                {
                    //Checks for FeederID ModelName assigned to ObjectClassField
                    enabled = ModelNameFacade.ContainsFieldModelName(objectClass, SchemaInfo.Electric.FieldModelNames.FeederID);
                }
            }

            return enabled;
        }

        /// <summary>
        /// Executes the AU while working with ArcMap Application only.
        /// </summary>
        /// <param name="eAUMode">The AU application mode.</param>
        /// <returns>Returns true if condition satisfied else false.</returns>
        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            return (eAUMode == mmAutoUpdaterMode.mmAUMArcMap);
        }

        /// <summary>
        /// Execute UniqueCircuitID AU
        /// </summary>
        /// <param name="obj">The Object being Updated.</param>
        /// <param name="eAUMode">The AU Mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {

            ITable pTable = pObject.Class as ITable;
            string stringError = string.Empty;
            string stringErrorTitle = string.Empty;

            //Checks for CircuitSource ModelName assigned to ObjectClass
            bool isCircuitSource = ModelNameFacade.ContainsClassModelName(pObject.Class, new string[] { SchemaInfo.Electric.ClassModelNames.CircuitSource });
            if (isCircuitSource)
            {
                //Checks for FeederID ModelName assigned to CircuitSource field
                bool isCircuitField = ModelNameFacade.ContainsFieldModelName(pObject.Class, new string[] { SchemaInfo.Electric.FieldModelNames.FeederID });
                if (isCircuitField)
                {
                    //Gets New Circuit ID value from input object                    
                    int indexValue = ModelNameFacade.FieldIndexFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.FeederID);
                    IField pField = ModelNameFacade.FieldFromModelName(pObject.Class, SchemaInfo.Electric.FieldModelNames.FeederID);

                    //Checks field value for null
                    string newCircuitId = Convert.ToString(pObject.get_Value(indexValue));

                    //Checks New Circuit ID value for nulls
                    if (!string.IsNullOrEmpty(newCircuitId))
                    {
                        //Checks New Circuit ID value already exists                        
                        IQueryFilter pFilter = new QueryFilterClass();
                        pFilter.WhereClause = pField.Name + "='" + newCircuitId + "'";
                        pFilter.SubFields = pField.Name;
                        int rowCount = pTable.RowCount(pFilter);

                        //Prompt user for CircuitID Violation
                        if (rowCount > 1)
                        {
                            stringErrorTitle = "Unique CircuitID Violation";
                            stringError = "The CircuitID must be a unique value.";
                            MessageBox.Show(stringError, stringErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                        }
                    }
                    else
                    {
                        stringError = "The CircuitID must not be null.";
                        _logger.Error(stringError);
                        throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                    }
                }
                else
                {
                    stringError = "Not Found " + SchemaInfo.Electric.FieldModelNames.FeederID + " ModelName on ObjectClass Field.";
                    _logger.Error(stringError);
                    throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
                }
            }
            else
            {
                stringError = "Not Found " + SchemaInfo.Electric.ClassModelNames.CircuitSource + " ModelName on Object Class.";
                _logger.Error(stringError);
                throw new COMException(stringError, (int)mmErrorCodes.MM_E_CANCELEDIT);
            }
        }

        #endregion Overridden BaseSpecialAU Methods

     
    }
}
