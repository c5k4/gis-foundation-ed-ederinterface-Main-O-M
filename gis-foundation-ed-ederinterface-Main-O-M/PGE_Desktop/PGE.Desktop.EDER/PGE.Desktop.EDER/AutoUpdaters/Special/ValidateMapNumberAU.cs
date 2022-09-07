using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Forms;
using log4net;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    /// <summary>
    /// Validates the Map Number of the Distribution Map feature
    /// </summary>
    [ComVisible(true)]
    [Guid("C91A8B5B-0BED-4846-9D37-806A290FBC1E")]
    [ProgId("PGE.Desktop.EDER.ValidateMapNumberAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class ValidateMapNumberAU : BaseSpecialAU
    {
        #region Private Variables

        /// <summary>
        /// Logs the debug/error information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Initializes new instance of <see cref="ValidateMapNumberAU"/> class
        /// </summary>
        public ValidateMapNumberAU() : base("PGE Validate Map Number AU") { }

        #endregion Constructor

        #region Overridden BaseSpecialAU Methods

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
                enabled = ModelNameFacade.ContainsClassModelName(objectClass, SchemaInfo.Electric.ClassModelNames.DistributionMap);
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
        /// Executed when an AU fired and validates the MapNumber of Distribution Map
        /// </summary>
        /// <param name="obj">Switchable Device object</param>
        /// <param name="eAUMode">AU firing mode</param>
        /// <param name="eEvent">AU event mode</param>
        /// <remarks>
        /// Allows the edit task to proceed if the validation succeeds.Otherwise, cancels the edit task.
        /// </remarks>
        protected override void InternalExecute(ESRI.ArcGIS.Geodatabase.IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            bool valid = true;
            //validate for the DistributionMap model names again
            if (ModelNameFacade.ContainsClassModelName(obj.Class, SchemaInfo.Electric.ClassModelNames.DistributionMap) == false) return;
            string missingField = "Filed not found with {0} model name in " + obj.Class.AliasName;

            //Get the field indexes and validate
            int mapNumberIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.MapNumber);
            if (mapNumberIndex == -1)
            {
                _logger.Debug(string.Format(missingField, SchemaInfo.Electric.FieldModelNames.MapNumber)); return;
            }

            int mapTypeIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.MapType);
            if (mapTypeIndex == -1)
            {
                _logger.Debug(string.Format(missingField, SchemaInfo.Electric.FieldModelNames.MapType)); return;
            }

            int divisionIndex = ModelNameFacade.FieldIndexFromModelName(obj.Class, SchemaInfo.Electric.FieldModelNames.Division);
            if (divisionIndex == -1)
            {
                _logger.Debug(string.Format(missingField, SchemaInfo.Electric.FieldModelNames.Division)); return;
            }

            //If the event is update then check whether a particular set of fields are updated
            if (eEvent == mmEditEvent.mmEventFeatureUpdate)
            {
                //Validate if any of the above fields are updated
                IRowChanges changes = obj as IRowChanges;
                if (changes.get_ValueChanged(mapNumberIndex) || changes.get_ValueChanged(mapTypeIndex) || changes.get_ValueChanged(divisionIndex)) { }
                else return;
            }

            //Get MapType and Division and MapNumber values
            int mapType = Convert.ToInt32(obj.get_Value(mapTypeIndex));
            string division = Convert.ToString(obj.get_Value(divisionIndex));
            string mapNumber = Convert.ToString(obj.get_Value(mapNumberIndex));

            //Validate the map number
            if (string.IsNullOrEmpty(mapNumber))
            {
                _logger.Debug("Mapnumber is empty no need to validate for any rule, validation failed.");
                valid = false;
            }
            else
            {
                _logger.Debug("Mapnumber is not empty need to validate first rule, validation starts.");
                //Get the Regular expression
                string pattern = PrepareRegularExpression(mapType, division);
                _logger.Debug("First rule, validation pattern " + pattern);

                if (pattern == string.Empty) return;
                //Validate the MapNumber values against the Regular expression
                Regex regExValidator = new Regex(pattern);
                valid = regExValidator.IsMatch(mapNumber);
                _logger.Debug(string.Format("First Validation pattern {0} and mapnumber {1} validation result {2}" ,pattern, mapNumber, valid));

                //Prompts the user and cancels the edit task if the mapNumber format is invalid
                if (!valid)
                {
                    _logger.Debug(string.Format("First Validation failed, Township Range Rule validation starts."));
                    //check for Township Range Rule
                    //Get the Regular expression
                    string patternTR = PrepareRegularExpressionTR(mapType, division);
                    //for scale 25 and 200 no pattern isapplied its simply invalid to have those maptypes in  TownshipRange Rule
                    if (patternTR == string.Empty)
                    {
                        _logger.Debug(string.Format("Township range rule validation pattern is empty, mapnumber {0} division {1}.", mapNumber, division));
                        valid = false;
                    }
                    else
                    {
                        //Validate the MapNumber values against the Regular expression
                        Regex regExValidatorTR = new Regex(patternTR);
                        ///////////////////////////////////////////////////////////////////////////////////
                        //modify the mapnumber to accomodate n# pattern as explained in comp spec document.
                        ///////////////////////////////////////////////////////////////////////////////////
                        string modifiedMapNumber = ModifyMapNumberForTestingTR(mapNumber, mapType, division);
                        valid = regExValidatorTR.IsMatch(modifiedMapNumber);
                        _logger.Debug(string.Format("Township range rule validation pattern {0} and mapnumber {1} validation result {2}", patternTR, mapNumber, valid));
                    }
                }
            }

            //finally check
            if (!valid)
            {
                _logger.Debug(string.Format("Validation failed, cancelling edit."));
                string errorMessage = "Inconsistent Map Number for the Distribution Map polygon.";
                //Prompt the user and stop the editing task
                if (eAUMode == mmAutoUpdaterMode.mmAUMArcMap)
                {
                    MessageBox.Show(errorMessage, "Inconsistent Map Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                throw new COMException(errorMessage, (int)mmErrorCodes.MM_E_CANCELEDIT);
            }
        }

        #endregion Overridden BaseSpecialAU Methods

        #region Regular Expression related methods

        /// <summary>
        /// Prepares regular expression pattern based on <paramref name="mapType"/> and <paramref name="division"/>
        /// </summary>
        /// <param name="mapType">Map Type Number of the Map</param>
        /// <param name="division">Division of the Map</param>
        /// <returns>Returns the regular expression pattern for the given <paramref name="mapType"/> and <paramref name="division"/></returns>
        private string PrepareRegularExpression(int mapType, string division)
        {
            StringBuilder pattern = new StringBuilder();
            const string sacramento = "Sacramento";
            const string stockton = "Stockton";
            const string northCoast = "North Coast";

            //Se the first part of the string depending on division
            switch (division)
            {
                case sacramento:
                    pattern.Append("[A-Z]{1,2}");//AO ; A=Required Alphabet O=Optional Alphabet
                    break;
                case stockton:
                    pattern.Append(@"(([A-Z])\2?|[A-Z][23]?)"); //Either AA or A#; # Numeric
                    break;
                default:
                    pattern.Append(@"([A-Z])\1?");//AO; If O presents, then O=A
                    break;
            }

            //Prepare the regular expression depending on mapType value and Division value
            switch (mapType)
            {
                case 25:
                    pattern.Append("[0-9]{2}(0[1-9]|1[0-9]|2[0-5])[A-D][1-4]");//## ## A #
                    break;
                case 50:
                    pattern.Append("[0-9]{2}(0[1-9]|1[0-9]|2[0-5])[A-D]");//## ## A
                    break;
                case 100:
                    pattern.Append("[0-9]{2}(0[1-9]|1[0-9]|2[0-5])");//## ##
                    if (division == northCoast)
                    {
                        pattern.Append("[A-Z][0-9]"); //A#
                    }
                    break;
                case 200:
                    pattern.Append("[0-9]{2}[A-Z]");//## A
                    if (division == northCoast)
                    {
                        pattern.Append("[A-Z]?");//A
                    }
                    break;
                case 250:
                    pattern.Append("[0-9]{2}[A-D]");//## A
                    break;
                case 500:
                    pattern.Append("[0-9]{2}");//##
                    if (division == northCoast)
                    {
                        pattern.Append("([A-Z][0-9])?");//A#
                    }
                    break;
                default:
                    pattern = new StringBuilder();
                    break;
            }


            //Append ^, $ at the start and end old Regular expression respectively
            if (pattern.Length > 0)
            {
                return "^" + pattern.ToString() + "$";

            }
            else return string.Empty;
        }

        /// <summary>
        /// Modifies and returns the mapnumber to test its pattern against Township Range rule
        /// </summary>
        /// <param name="mapNumber">The MapNumber</param>
        /// <param name="mapType">The MapType</param>
        /// <param name="division">TheDivision</param>
        /// <returns></returns>
        private string ModifyMapNumberForTestingTR(string mapNumber, int mapType, string division)
        {
            string modifiedMapNumber = mapNumber;

            const string fresno = "Fresno";

            if ((mapType == 50 || mapType == 100 || mapType == 250 || mapType == 500) &&
                (division == fresno))
            {
                //Prepare the regular expression depending on mapType value and Division value
                switch (mapType)
                {
                    case 50:
                    //n#;##;n#;#;A
                    //For the purposes of checking the map name against the standard, 
                    //I propose we check the first digit of  Fresno’s maps and if it’s a 9, 
                    //zero pad it to 09, and then check the length of the potentially zero 
                    //padded name, if it’s 7 characters long insert a 0 in the 5th place so 
                    //we can check the Fresno maps against the traditional TR format.
                            
                    //I.e. TR map name  91326A becomes 0913026A for checking purposes only, 
                    //the stored map name in the database is still 91326A
                        if (mapNumber.StartsWith("9"))
                        {
                            modifiedMapNumber = "0" + mapNumber;
                        }
                        if (modifiedMapNumber.Length == 7)
                        {
                            modifiedMapNumber = modifiedMapNumber.Insert(4, "0");
                        }
                        break;
                    case 100:
                    //n#;##;n#;#
                    //I propose the same solution as with the Fresno 50 scale maps, zero pad a 
                    //leading 9 and check the length to determine if we need to 0 pad the third 
                    //section (5th character) of the name and run the result through the standard 
                    //TR name checker.
                        if (mapNumber.StartsWith("9"))
                        {
                            modifiedMapNumber = "0" + mapNumber;
                        }
                        if (modifiedMapNumber.Length == 6)
                        {
                            modifiedMapNumber = modifiedMapNumber.Insert(4, "0");
                        }
                        break;
                    case 250:
                    //n#;##;#;A
                    //I propose we zero pad a leading 9 and run the result through the standard 
                    //TR name checker.

                        if (mapNumber.StartsWith("9"))
                        {
                            modifiedMapNumber = "0" + mapNumber;
                        }
                        break;
                    case 500:
                    //n#;##;#
                    //I propose we zero pad a leading 9 and run the result through the standard 
                    //TR name checker.

                        if (mapNumber.StartsWith("9"))
                        {
                            modifiedMapNumber = "0" + mapNumber;
                        }
                        break;
                }
            }
            else
            {
                modifiedMapNumber = mapNumber;
            }
            return modifiedMapNumber;
        }

        /// <summary>
        /// Prepares regular expression pattern Township Range Rule based on <paramref name="mapType"/> and <paramref name="division"/>
        /// </summary>
        /// <param name="mapType">Map Type Number of the Map</param>
        /// <param name="division">Division of the Map</param>
        /// <returns>Returns the regular expression pattern for the given <paramref name="mapType"/> and <paramref name="division"/></returns>
        private string PrepareRegularExpressionTR(int mapType, string division)
        {
            StringBuilder pattern = new StringBuilder();
            const string fresno = "Fresno";
            const string yosemite = "Yosemite";


            //Prepare the regular expression depending on mapType value and Division value
            switch (mapType)
            {
                case 25:
                    //does not have it or allowed
                    pattern = new StringBuilder();
                    break;
                case 50:
                    switch (division)
                    { 
                        case fresno:
                            pattern.Append("(0[9]|1[0-9]|2[0-5])(1[3-9]|2[0-9])(0[1-9]|1[0-9]|2[0-9]|3[0-6])[1-6][A-D]");//n# ## n# # A
                            break;
                        case yosemite:
                            pattern.Append("[0-9].*(0[1-9]|1[0-9]|2[0-9]|3[0-6])[1-6][A-D]"); //n# n# ## # A
                            break;
                        default:
                            pattern.Append("[0-9]{4}(0[1-9]|1[0-9]|2[0-9]|3[0-6])[1-6][A-D]");//## ## ## # A
                            break;
                    }
                    break;
                case 100:
                    switch (division)
                    {
                        case fresno:
                            pattern.Append("(0[9]|1[0-9]|2[0-5])(1[3-9]|2[0-9])(0[1-9]|1[0-9]|2[0-9]|3[0-6])[1-6]");//n# ## n# # 
                            break;
                        case yosemite:
                            pattern.Append("[0-9].*(0[1-9]|1[0-9]|2[0-9]|3[0-6])[1-6]"); //n# n# ## #
                            break;
                        default:
                            pattern.Append("[0-9]{4}(0[1-9]|1[0-9]|2[0-9]|3[0-6])[1-6]");
                            break;
                    }
                    break;
                case 200:
                    //does not have or allowed
                    pattern = new StringBuilder();
                    break;
                case 250:
                    switch (division)
                    {
                        case fresno:
                            pattern.Append("(0[1-9]|1[0-9]|2[0-5])(1[3-9]|2[0-9])[1-6][A-D]");//n# ## # A
                            break;
                        case yosemite:
                            pattern.Append("[0-9].*[1-6][A-D]"); //n# n# # A
                            break;
                        default:
                            pattern.Append("[0-9]{4}[1-6][A-D]");//## ## # A
                            break;
                    }
                    break;
                case 500:
                    switch (division)
                    {
                        case fresno:
                            pattern.Append("(0[1-9]|1[0-9]|2[0-5])(1[3-9]|2[0-9])[1-6]");//n# ## #
                            break;
                        case yosemite:
                            pattern.Append("[0-9].*[1-6]"); //n# n# #
                            break;
                        default:
                            pattern.Append("[0-9]{4}[1-6]");//## ## #
                            break;
                    }
                    break;
                default:
                    pattern = new StringBuilder();
                    break;
            }


            //Append ^, $ at the start and end old Regular expression respectively
            if (pattern.Length > 0)
            {
                return "^" + pattern.ToString() + "$";

            }
            else return string.Empty;
        }
        #endregion Regular Expression related methods
    }
}
