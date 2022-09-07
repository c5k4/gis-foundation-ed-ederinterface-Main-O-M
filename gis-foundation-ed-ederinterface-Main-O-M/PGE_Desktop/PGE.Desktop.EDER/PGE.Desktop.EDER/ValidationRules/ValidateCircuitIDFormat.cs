
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates the CircuitId format
    /// </summary>
    [ComVisible(true)]
    [Guid("38DBA823-4256-466B-A429-EEC5474B02F3")]
    [ProgId("PGE.Desktop.EDER.ValidateCircuitidFormat")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateCircuitIDFormat : BaseValidationRule
    {
        #region Private variable

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string WarnObjectClassMissingModelNames = "Missing object class model names:\r\n{0}";
        private const string WarnFieldMissingModelNames = "Missing field model names:\r\n{0}";
        private const string CircuitDistrictDomainName = "CircuitDivision";
        private const string CircuitSubStationIDDomainName = "SubstationID";

        #endregion

        #region Constructor

        /// <summary>
        /// Initilizes the instance of <see cref="ValidateCircuitIDFormat"/> class
        /// </summary>
        public ValidateCircuitIDFormat()
            : base("PGE Validate CircuitID Format", SchemaInfo.Electric.ClassModelNames.CircuitSource)
        { }

        #endregion Constructor

        #region Overridden BaseValidationRule Methods

        /// <summary>
        /// Determines if the specified parameter is an object class that has been configured with a class model name identified
        /// in the _modelNames array.
        /// </summary>
        /// <param name="param">The object class to validate.</param>
        /// <returns>Boolean indicating if the specified object class has any of the appropriate model name(s).</returns>
        protected override bool EnableByModelNames(object param)
        {
            if (base.EnableByModelNames(param))
            {
                return true;
            }

            _logger.Warn(string.Format(WarnObjectClassMissingModelNames, _ModelNames.Concatenate("\r\n")));
            return false;
        }

        /// <summary>
        /// Validates the Operating Voltage of DynamicProtectiveDevice against its upstream conductors
        /// </summary>
        /// <param name="row">Row to validate</param>
        /// <returns>Returns the error list after validation</returns>
        protected override ID8List InternalIsValid(IRow row)
        {

            IObject Obj = row as IObject;
            // finding the circuitid field indx using the fieldmodel name.
            int circuitIDFldIndx = ModelNameFacade.FieldIndexFromModelName(Obj.Class,SchemaInfo.Electric.FieldModelNames.FeederID);
            if(circuitIDFldIndx == -1)
            {
                _logger.Warn(string.Format(WarnFieldMissingModelNames, SchemaInfo.Electric.FieldModelNames.FeederID));
                return _ErrorList;
            }
            // getting the value of the cuircuitid.
            object circuitIdValue = Obj.get_Value(circuitIDFldIndx);
            // checking if value is null or empty.
            if (circuitIdValue == System.DBNull.Value && string.IsNullOrEmpty(circuitIdValue.ToString()))
            {
                AddError(Obj.Fields.Field[circuitIDFldIndx].AliasName + " field cannot be <Null>.");
            }
            else
            {
                // checking if the value length is < 8 or > 9.
                int circuitIdLength = circuitIdValue.ToString().Trim().Length;
                if (circuitIdLength < 8 || circuitIdLength > 9)
                {
                    AddError(Obj.Fields.Field[circuitIDFldIndx].AliasName + " value must be 8 or 9 characters.");
                }
                else
                {
                    string circuitDistrictValue = string.Empty;
                    string circuitSubstationValue = string.Empty;
                    //getting the district and substation from circuitid based on the length.
                    if (circuitIdLength == 8)
                    {
                        circuitDistrictValue = circuitIdValue.ToString().Substring(0, 1);
                        circuitSubstationValue = circuitIdValue.ToString().Substring(1, 3);
                    }
                    else
                    {
                        circuitDistrictValue = circuitIdValue.ToString().Substring(0, 2);
                        circuitSubstationValue = circuitIdValue.ToString().Substring(2, 3);
                    }

                    // getting the circuitdistrict domain from the workspace.
                    IDomain districtDomain = getDomain(((IDataset)Obj.Class).Workspace, CircuitDistrictDomainName);
                    if (districtDomain == null)
                    {
                        AddError("Domain '" + CircuitDistrictDomainName + "' not found in the database.");
                        return _ErrorList;
                    }
                    //checking the circuitdistrict value exist in the domain.
                    if (districtDomain is ICodedValueDomain)
                    {
                        int cdValue = -1;
                        if(int.TryParse(circuitDistrictValue,out cdValue))
                        {
                            if (!valueExistInDomain(districtDomain, cdValue))
                            {
                                AddError("District code in CircuitID does not match any existing district codes.");
                            }
                        }
                    }
                    // getting the circuitSubStationID domain from the workspace.
                    IDomain subStationIDDomain = getDomain(((IDataset)Obj.Class).Workspace, CircuitSubStationIDDomainName);
                    if (subStationIDDomain == null)
                    {
                        AddError("Domain '" + CircuitSubStationIDDomainName + "' not found in the database.");
                        return _ErrorList;
                    }
                    //checking the circuitSubStationID value exist in the domain.
                    if (subStationIDDomain is ICodedValueDomain)
                    {
                        int csValue = -1;
                        if (int.TryParse(circuitSubstationValue, out csValue))
                        {
                            if (!valueExistInDomain(subStationIDDomain, csValue))
                            {
                                AddError("Substation code in CircuitID does not match any existing substation codes.");
                            }
                        }
                    }

                    //check that the last four characters are numeric
                    string circuitID = string.Empty;
                    circuitID = circuitIdValue.ToString().GetLast(4);
                    if (circuitID.IsNumeric() == false)
                    {
                        AddError("CircuitID must be numeric.");
                    }
                }
            }
            return _ErrorList;
        }

        #endregion Overridden BaseValidationRule Methods

        #region Private Method
        /// <summary>
        /// This will search return the specified domain from workspace.
        /// </summary>
        /// <param name="workSpace">Workspace</param>
        /// <param name="domainName">Name of the domain.</param>
        /// <returns>IDomain</returns>
        private IDomain getDomain(IWorkspace workSpace, string domainName)
        {
            IWorkspaceDomains workspaceDomains = (IWorkspaceDomains)workSpace;
            return workspaceDomains.DomainByName[domainName];
        }
        /// <summary>
        /// This method will check the coded domain value exists in the specified domain or not.
        /// </summary>
        /// <param name="domain">Idomain.</param>
        /// <param name="domainValue">Integer value to search in the domain.</param>
        /// <returns>Boolean indicating if the specified domain value is exist in the domain.</returns>
        private Boolean valueExistInDomain(IDomain domain, int domainValue)
        {
            Dictionary<string, int> domainDictionary = domain.ToDictionary(-1);
            return domainDictionary.ContainsValue(domainValue);

        }
        #endregion
    }

    public static class StringExtension
    {
        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }

        public static bool IsNumeric(this string theValue)
        {
            long retNum;
            return long.TryParse(theValue, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
        }
    }
}
