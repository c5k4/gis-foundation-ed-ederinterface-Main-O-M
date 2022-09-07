using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("902914FA-5173-4092-A80B-CEFA7E58641F")]
    [ProgId("PGE.Desktop.EDER.ValidateSumUnitCount")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateSumUnitCount : BaseValidationRule
    {

        #region Constructors
        public ValidateSumUnitCount()
            : base("PGE Validate Sum Unit Count", _modelNames)
        {
        }
        #endregion Constructors

        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static ConductorAttributeFacade _conductorAttribFacade = new ConductorAttributeFacade();
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryConductor, SchemaInfo.Electric.ClassModelNames.SecondaryFeeder };


        /// <summary>
        /// Add error to ID8List base._ErrorList
        /// </summary>
        /// <param name="listOfErrors">The List of Errors.</param>
        private void AddError(List<string> listOfErrors, ESRI.ArcGIS.Geodatabase.IRow pRow)
        {
            //Check if List of Errors is null
            if(listOfErrors==null)
            {
                //return the control
                _logger.Debug("No validation errors found.");
                return;
            }
            //add each msg from List of errors to base._ErrorList
            foreach (string err  in listOfErrors)
            {
                //Add error message
                _logger.Debug(err);
                AddError(err);
            }
        }
        #endregion private

        #region overrides
        /// <summary>
        /// Validates featureclass for CEDSANumberofPhases field value with all realated Objectclass records
        /// </summary>
        /// <param name="pRow">FeatureclassRow</param>
        /// <returns>list of errors reported while validating</returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow pRow)
        {
            //Change to address INC000003803926 QA/QC freezing on the QAQC subtask  
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            // get the list of eerors to report
            List<string> errList= _conductorAttribFacade.ExecuteForValidateSumUnitCount(pRow);
            //add each error to the base._ErrorList
            AddError(errList, pRow);
            //return _ErrorList
            return _ErrorList;
        }
        #endregion overrides
    }
}
