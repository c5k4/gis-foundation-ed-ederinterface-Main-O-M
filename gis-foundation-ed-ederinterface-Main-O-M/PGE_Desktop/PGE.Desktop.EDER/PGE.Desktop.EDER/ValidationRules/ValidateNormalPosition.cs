using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates NormalPosision Values to PhaseDesignation for:
    ///   DynamicProtectiveDevice, Fuse, OpenPoint, Switch.
    /// </summary>
    [ComVisible(true)]
    [Guid("825EE3B3-C141-42CF-BFED-DCB48D9ABCA5")]
    [ProgId("PGE.Desktop.EDER.ValidateNormalPosition")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateNormalPosition : BaseValidationRule
    {
        #region Private Variables
        private static readonly string[] _enabledModelNames = { SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice,
                                                      SchemaInfo.Electric.ClassModelNames.Fuse,
                                                      SchemaInfo.Electric.ClassModelNames.PGEOpenPoint,
                                                      SchemaInfo.Electric.ClassModelNames.PGESwitch};

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateNormalPosition()
            : base("PGE Validate NormalPosition", _enabledModelNames)
        {
           
        }
        #endregion Constructor

        #region Overrides for validation rule

        /// <summary>
        /// Determines if the provided row is valid.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            _logger.Debug("ValidateNormalPosition Started.");
            try
            {
                // Cast row as Ifeature.
                IFeature obj = row as IFeature;

                // Check if casting is successful.
                if (obj == null)
                {
                    _logger.Debug("Row casting to Ifeature failed.");
                }
                else
                {
                    ValidateNormalPositions(obj);
                }

                _logger.Debug("ValidateNormalPosition Ended.");
                return _ErrorList;
            }
            catch(System.Exception ex)
            {
                _logger.Error("Error occurred while Validating NormalPosition rule", ex);
                return _ErrorList;
            }
        }

        #endregion Overrides for validation rule


        /// <summary>
        /// Validate normal position(s).
        /// </summary>
        /// <param name="feature"></param>
        private void ValidateNormalPositions(IFeature feature)
        {
            string result = string.Empty;
            bool fldModelNames = false;
            _logger.Debug("ValidateNormalPositions started.");
            NormalPositionQAQCC normItems;

            try
            {
                // List of fields needed for validation.
                string[] strArryFldModelNames = new string[] { SchemaInfo.Electric.FieldModelNames.NormalPosition, SchemaInfo.Electric.FieldModelNames.NormalpositionA,
                                                           SchemaInfo.Electric.FieldModelNames.NormalpositionB, SchemaInfo.Electric.FieldModelNames.NormalPositionC,
                                                           SchemaInfo.Electric.FieldModelNames.PhaseDesignation};

                // Insure fields exist on feature.
                fldModelNames = ModelNameFacade.ContainsFieldModelName(feature.Class, strArryFldModelNames);
                if (fldModelNames)
                {
                    _logger.Debug("Feature in field model name.");
                    // Collect field values.
                    normItems = new NormalPositionQAQCC(feature);

                    // Check for exclusions.
                    if (!normItems.ExlcudeFeature(feature))
                    {
                        // Can't compare empty phase so just report.
                        if (normItems.PhaseDesignation.Length > 0)
                        {
                            // Start validation.
                            List<KeyValuePair<int, string>> finds = normItems.ValidateNormalPositions();
                            foreach (KeyValuePair<int, string> find in finds)
                            {
                                if (find.Key > 0)
                                {
                                    _logger.Debug(find.Value);
                                    AddError(find.Value);
                                }
                            }
                        }
                        else
                        {
                            // Null phasedesignation is handled in ValidatePhaseDesignation.
                            _logger.Debug("Phasedesignation Null-Empty on " + feature.Class.AliasName + " OID: " + feature.OID);
                        }
                        
                    }
                    else
                        _logger.Debug("Feature excluded " + feature.Class.AliasName + " OID: " + feature.OID);

                }
                else
                {
                    _logger.Warn(string.Format("NormalPosition Validation, feature is missing model names. OID {0}", feature.OID));
                }
                _logger.Debug("ValidateNormalPositions Ended.");
            }
            catch (System.Exception ex)
            {
                _logger.Error("Error occurred while validating NormalPosition rule.", ex);
            }
        }

    }

    /// <summary>
    /// Class to hold values and validates normalposition to phase.
    /// </summary>
    internal class NormalPositionItems
    {
        #region "Private vars/messages"
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private string _phaseDesignation = string.Empty;
        private string _notPhaseDesignation = string.Empty;
        private int? _normalposition = -1;
        private int? _normalpositionA;
        private int? _normalpositionB;
        private int? _normalpositionC;
        private int? _status;

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        internal NormalPositionItems()
        {

        }


        /// <summary>
        /// Load feature into this object.
        /// </summary>
        /// <param name="feature"></param>
        internal void LoadFeature(IFeature feature)
        {
            this.Feature = feature;
            this.OID = feature.OID;

            this.NormalPostion = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.NormalPosition).Convert<int>(-1);
            this.NormalPostionA = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.NormalpositionA).Convert<int>(-1);
            this.NormalPostionB = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.NormalpositionB).Convert<int>(-1);
            this.NormalPostionC = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.NormalPositionC).Convert<int>(-1);

            this.Status = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).Convert<int>(-1);
            this.PhaseDesignation = feature.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<string>(string.Empty);

        }

        #region "Properties"

        internal IFeature Feature { get; set; }
        internal int OID { get; set; }

        /// <summary>
        /// Features phasedesignation.
        /// </summary>
        internal string PhaseDesignation
        {
            get { return _phaseDesignation; }
            set
            {
                _phaseDesignation = value;
                if (_phaseDesignation.Length > 0)
                    _notPhaseDesignation = ReturnUnusedPhases(value);
            }
        }

        /// <summary>
        /// Missing Phases on designated.
        /// </summary>
        internal string NotPhaseDesignation { get; set; }

        internal int? NormalPostion { get { return _normalposition; } set { _normalposition = value; } }
        internal int? NormalPostionA { get { return _normalpositionA; } set { _normalpositionA = value; } }
        internal int? NormalPostionB { get { return _normalpositionB; } set { _normalpositionB = value; } }
        internal int? NormalPostionC { get { return _normalpositionC; } set { _normalpositionC = value; } }

        // 0 = Proposed, 5 = InService, 30 = idle.
        internal int? Status { get { return _status; } set { _status = value; } }

        #endregion

        /// <summary>
        /// Returns phases not 'energized'.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>string</returns>
        private string ReturnUnusedPhases(string input)
        {
            string result = string.Empty;
            string primer = "ABC";

            input = input.ToUpper();

            foreach (char c in input)
            {
                primer = primer.Replace(c.ToString(), "");
                result = primer;
            }
            return result;
        }


    }

    /// <summary>
    /// Validates features with normalposition fields.
    /// </summary>
    internal class NormalPositionQAQCC : NormalPositionItems
    {
        #region "Private vars/messages"

        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private readonly string _msg1MissMatch = "NormalPositionA_B_C fields must match PhaseDesignation field.";
        private readonly string _msgUnUsedOpenOrClosed = "NormalPositionA_B_C fields must match PhaseDesignation field.";

        private readonly string _msg2PhaseMissMatch = "NormalPositionA_B_C fields must match PhaseDesignation field.";
        private readonly string _msgOpenCloseMix = "NormalPositionA_B_C fields cannot contain both an Open and a Closed position.";
        private readonly string _msg3MissMatch = "NormalPositionA_B_C fields must all match the NormalPosition field for three-phase devices.";
        private readonly string _msg12NormalNotNull = "Normal Position attribute must be null for non-three phase devices.";
        private readonly string _msg123NullABC = "NormalPositionA_B_C must contain a valid domain value (not Null)!";

        #endregion

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="feature"></param>
        internal NormalPositionQAQCC(IFeature feature)
        {
            LoadFeature(feature);
        }

        #region "Validations"
        /// <summary>
        /// Validate phase directs one,two or three phase to proper validation section.
        /// </summary>
        /// <returns>
        ///    keyvaluepair<int,string>
        ///    int = 0: Good, 1: warning, 2: error, 3: system exception
        ///    string = Message to report
        /// </returns>
        internal List<KeyValuePair<int, string>> ValidateNormalPositions()
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            if (!ExlcudeFeature(this.Feature))
            {
                if (this.PhaseDesignation.Length == 1)
                    result = ValidateSinglePhase();
                else if (this.PhaseDesignation.Length == 2)
                    result = ValidateTwoPhase();
                else if (this.PhaseDesignation.Length == 3)
                    result = ValidateThreePhase();
                else
                {
                    KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(1, "PhaseDesignation not populated correctly.");
                    result.Add(kvp);
                }
            }

            return result;
        }

        /// <summary>
        /// Validate one phase devices.
        /// </summary>
        /// <returns>
        ///    keyvaluepair<int,string>
        ///    int = 0: Good, 1: warning, 2: error, 3: system exception
        ///    string = Message to report
        /// </returns>
        internal List<KeyValuePair<int, string>> ValidateSinglePhase()
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            _logger.Debug("ValidateSinglePhase started.");
            if (this.PhaseDesignation == "A")
            {
                result = SinglePhaseCheck(this.NormalPostionA, new int?[2] { this.NormalPostionB, this.NormalPostionC });
            }
            else if (this.PhaseDesignation == "B")
            {
                result = SinglePhaseCheck(this.NormalPostionB, new int?[2] { this.NormalPostionA, this.NormalPostionC });
            }
            else if (this.PhaseDesignation == "C")
            {
                result = SinglePhaseCheck(this.NormalPostionC, new int?[2] { this.NormalPostionA, this.NormalPostionB });
            }
            _logger.Debug("ValidateSinglePhase ended.");
            return result;
        }

        /// <summary>
        /// Validate two phase devices.
        /// </summary>
        /// <returns>
        ///    keyvaluepair<int,string>
        ///    int = 0: Good, 1: warning, 2: error, 3: system exception
        ///    string = Message to report
        /// </returns>
        internal List<KeyValuePair<int, string>> ValidateTwoPhase()
        {
            _logger.Debug("ValidateTwoPhase started.");
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            if (this.PhaseDesignation == "AB")
            {
                result = DoublePhaseCheck(new int?[2] { this.NormalPostionA, this.NormalPostionB }, this.NormalPostionC);
            }
            else if (this.PhaseDesignation == "AC")
            {
                result = DoublePhaseCheck(new int?[2] { this.NormalPostionA, this.NormalPostionC }, this.NormalPostionB);
            }
            else if (this.PhaseDesignation == "BC")
            {
                result = DoublePhaseCheck(new int?[2] { this.NormalPostionB, this.NormalPostionC }, this.NormalPostionA);
            }
            _logger.Debug("ValidateTwoPhase ended.");
            return result;
        }

        /// <summary>
        /// Validate three phase devices.
        /// </summary>
        /// <returns>
        ///    keyvaluepair<int,string>
        ///    int = 0: Good, 1: warning, 2: error, 3: system exception
        ///    string = Message to report
        /// </returns>
        internal List<KeyValuePair<int, string>> ValidateThreePhase()
        {
            _logger.Debug("ValidateThreePhase started.");
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            if (this.PhaseDesignation == "ABC")
            {
                result = TriplePhaseCheck(new int?[3] { this.NormalPostionA, this.NormalPostionB, this.NormalPostionC });
            }
            _logger.Debug("ValidateThreePhase ended.");
            return result;
        }


        /// <summary>
        /// Runs validation rules for features with three phases (ABC).
        /// </summary>
        /// <param name="currentPhases">ABC</param>
        /// <returns>
        ///    keyvaluepair<int,string>
        ///    int = 0: Good, 1: warning, 2: error, 3: system exception
        ///    string = Message to report
        /// </returns>
        internal List<KeyValuePair<int, string>> TriplePhaseCheck(int?[] currentPhases)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();

            KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(0, string.Empty);
            // Check if current phases are open or closed.
            if (IsOpenOrClosed(currentPhases))
            {
                // Check that unused phases are not open or closed (should be NA or NULL).
                if (!PositionsMatch(currentPhases))
                {
                    // Warning phase do NOT match.
                    kvp = new KeyValuePair<int, string>(1, _msgOpenCloseMix); 
                    result.Add(kvp);
                }
                else
                {

                }
            }
            else
            {
                // Warning phase do NOT match.
                kvp = new KeyValuePair<int, string>(1, _msg3MissMatch); 
                result.Add(kvp);
            }

            // This is an extra validation only for three phases.
            // NormalPosition should be eq to NormalPostionA,B and C).
            if ((this.NormalPostion != this.NormalPostionA) || (this.NormalPostion != this.NormalPostionB) || (this.NormalPostion != this.NormalPostionC))
            {
                kvp = new KeyValuePair<int, string>(1, _msg3MissMatch);
                result.Add(kvp);
            }

            // Null normalABC.
            if (IsNull(new int?[] { this.NormalPostionA, this.NormalPostionB, this.NormalPostionC }))
            {
                // Warning UnUsed NormalPosition(s) are NULL should be NA.
                kvp = new KeyValuePair<int, string>(1, _msg123NullABC);
                result.Add(kvp);
            }

            return result;
        }

        /// <summary>
        ///  Runs validation rules for features with two phase (AB, AC, BC).
        /// </summary>
        /// <param name="currentPhases"></param>
        /// <param name="unUsedPhase"></param>
        /// <returns>
        ///    keyvaluepair<int,string>
        ///    int = 0: Good, 1: warning, 2: error, 3: system exception
        ///    string = Message to report
        /// </returns>
        internal List<KeyValuePair<int, string>> DoublePhaseCheck(int?[] currentPhases, int? unUsedPhase)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(0, string.Empty);

            if (IsOpenOrClosed(currentPhases))
            {
                if (!PositionsMatch(currentPhases))
                {
                    // Warning phase do NOT match.
                    kvp = new KeyValuePair<int, string>(1, _msgOpenCloseMix);
                    result.Add(kvp);
                }
            }
            else
            {
                // Warning not open or closed.
                kvp = new KeyValuePair<int, string>(1, _msg2PhaseMissMatch);
                result.Add(kvp);
            }

            if (!IsNA(new int?[] { unUsedPhase }))
            {
                // Warning UnUsed NormalPosition(s) are Open or Closed and should be NA.
                kvp = new KeyValuePair<int, string>(1, _msgUnUsedOpenOrClosed);
                result.Add(kvp);
            }

            // Null normalABC.
            if (IsNull(new int?[] { this.NormalPostionA, this.NormalPostionB, this.NormalPostionC }))
            {
                // Warning UnUsed NormalPosition(s) are NULL should be NA.
                kvp = new KeyValuePair<int, string>(1, _msg123NullABC);
                result.Add(kvp);
            }

            // Not null normal for 1 and 2 phases normalposition should be null.
            if (!IsNull(new int?[] { this.NormalPostion }))
            {
                kvp = new KeyValuePair<int, string>(1, _msg12NormalNotNull);
                result.Add(kvp);
            }

            return result;
        }

        /// <summary>
        /// Runs validation rules for features with single phase (A, B, C).
        /// </summary>
        /// <param name="currentPhase"></param>
        /// <param name="unUsedPhases"></param>
        /// <returns>
        ///    keyvaluepair<int,string>
        ///    int = 0: Good, 1: warning, 2: error, 3: system exception
        ///    string = Message to report
        /// </returns>
        internal List<KeyValuePair<int, string>> SinglePhaseCheck(int? currentPhase, int?[] unUsedPhases)
        {
            List<KeyValuePair<int, string>> result = new List<KeyValuePair<int, string>>();
            KeyValuePair<int, string> kvp = new KeyValuePair<int, string>(0, string.Empty);

            _logger.Debug("SinglePhaseCheck started.");

            if (IsOpenOrClosed(new int?[1] { currentPhase }))
            {
                // Thats it for single phase, no comparing mixed open/closed.
            }
            else
            {
                // Warning not open or closed, doesn't match phase designation.
                kvp = new KeyValuePair<int, string>(1, _msg1MissMatch);
                result.Add(kvp);
            }

            if (!IsNA(unUsedPhases))
            {
                // Warning not na.
                kvp = new KeyValuePair<int, string>(1, _msgUnUsedOpenOrClosed);
                result.Add(kvp);
            }

            // Null normalABC.
            if (IsNull(new int?[] { this.NormalPostionA, this.NormalPostionB, this.NormalPostionC }))
            {
                // Warning UnUsed NormalPosition(s) are NULL should be NA.
                kvp = new KeyValuePair<int, string>(1, _msg123NullABC);
                result.Add(kvp);
            }

            // Not null normal for 1 and 2 phases normalposition should be null.
            if (!IsNull(new int?[] { this.NormalPostion }))
            {
                kvp = new KeyValuePair<int, string>(1, _msg12NormalNotNull);
                result.Add(kvp);
            }
            _logger.Debug("SinglePhaseCheck ended.");
            return result;
        }

        #endregion

        #region "Helpers"

        /// <summary>
        /// Do all normalpositions in list match.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true/false</returns>
        private bool PositionsMatch(int?[] input)
        {
            bool result = true;
            int? start = -1;
            if (input.Length > 0)
            {
                start = input[0];

                foreach (int? i in input)
                {
                    if (i != start)
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                // Nothing in input array.
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Is normalposition open or closed.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true/false</returns>
        private bool IsOpenOrClosed(int?[] input)
        {
            bool result = false;
            // 0 = Open, 1 = Closed, 2 = NA.
            foreach (int? i in input)
            {
                if ((i != null) && (i != 2))
                {
                    if ((i == 0) || (i == 1))
                    {
                        result = true;
                    }
                }
                else
                {
                    // We have an NA so exit with False.
                    result = false;
                    break;
                }

            }

            return result;
        }

        /// <summary>
        /// Is normalposition null.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true/false</returns>
        private bool IsNull(int?[] input)
        {
            bool result = false;
            // 0 = Open, 1 = Closed, 2 = NA.
            foreach (int? i in input)
            {
                // Code converts null's to -1.
                if ((i == null) || (i == -1))
                {
                    result = true;
                    break;
                }

            }

            return result;
        }

        /// <summary>
        /// Is normalposition NA, for unused positions.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>true/false</returns>
        private bool IsNA(int?[] input)
        {
            bool result = true;
            // 0 = Open, 1 = Closed, 2 = NA.
            foreach (int? i in input)
            {
                if (i == null)
                {
                    result = false;
                    break;
                }
                else if (i != 2)
                {
                    result = false;
                    break;
                }

            }

            return result;
        }

        /// <summary>
        /// Check if features SecondaryIDC field is Yes or No. 
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>True = Yes, False = No</returns>
        private bool SecondaryIndicatorValue(IFeature feature)
        {
            bool result = false;

            IObjectClass objClass = feature.Class as IObjectClass;
            if (ModelNameFacade.ContainsFieldModelName(objClass, new string[] { SchemaInfo.Electric.FieldModelNames.SecondaryIDC }))
            {
                // Domain Yes No Indicator N = No, Y = Yes.
                object obj = feature.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.SecondaryIDC);
                if (obj != null)
                {
                    if ((obj.ToString().ToUpper() == "YES") || (obj.ToString().ToUpper() == "Y"))
                    {
                        // This feature is marked as secondary.
                        result = true;
                    }
                }
            }

            return result;
        }

        #endregion

        #region "Exclusions"

        /// <summary>
        /// Exclude features based on rules.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>True to exclude, False to keep.</returns>
        internal bool ExlcudeFeature(IFeature feature)
        {
            _logger.Debug("ExludeFeature started.");
            bool result = false;
            string name = ((IDataset)feature.Class).Name.ToUpper();

            // Global exclusions here only need one to not process feature.
            if (StatusHelper.IsIdle(feature))
            {
                return true;
            }

            IObjectClass objClass = feature.Class as IObjectClass;
            if (ModelNameFacade.ContainsClassModelName(objClass, new string[] { SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice }))
            {
                result = ExcludeDynamicProtectiveDevice(feature);
            }
            else if (ModelNameFacade.ContainsClassModelName(objClass, new string[] { SchemaInfo.Electric.ClassModelNames.Fuse }))
            {
                result = ExcludeFuse(feature);
            }
            else if (ModelNameFacade.ContainsClassModelName(objClass, new string[] { SchemaInfo.Electric.ClassModelNames.PGEOpenPoint }))
            {
                result = ExcludeOpenPoint(feature);
            }
            else if (ModelNameFacade.ContainsClassModelName(objClass, new string[] { SchemaInfo.Electric.ClassModelNames.Switch }))
            {
                result = ExcludeSwitch(feature);
            }
            _logger.Debug("ExludeFeature end.");
            return result;
        }

        /// <summary>
        /// Check to exclude Dynamic Protective Device features.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>true/false</returns>
        private bool ExcludeDynamicProtectiveDevice(IFeature feature)
        {
            bool result = false;
            _logger.Debug("ExcludeDynamicProtectiveDevice started.");

            _logger.Debug("ExcludeDynamicProtectiveDevice end.");
            return result;
        }

        /// <summary>
        /// Check to exclude fuse features.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>true/false</returns>
        private bool ExcludeFuse(IFeature feature)
        {
            bool result = false;
            _logger.Debug("ExcludeFuse started.");

            _logger.Debug("ExcludeFuse end.");

            return result;
        }

        /// <summary>
        /// Check to exclude openpoint features.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>true/false</returns>
        private bool ExcludeOpenPoint(IFeature feature)
        {
            _logger.Debug("ExcludeOpenPoint started.");
            bool result = true;
            // Openpoint not connected to an primary.
            IObjectClass objClass;

            if (SecondaryIndicatorValue(feature))
            {
                _logger.Debug("SecondaryIDC value is Yes.");
                // Feature marked as secondary.
                return true;
            }

            List<IFeature> list = TraceFacade.GetFirstConnectedFeatureOfType(feature, esriElementType.esriETEdge);
            foreach (IFeature f in list)
            {
                objClass = f.Class as IObjectClass;
                if (ModelNameFacade.ContainsClassModelName(objClass, new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryConductor }))
                {
                    _logger.Debug("Is connected to primary conductor.");
                    // This is a primary conductor.
                    return false;
                }
                else if (ModelNameFacade.ContainsClassModelName(objClass, new string[] { SchemaInfo.Electric.ClassModelNames.PGEBusBar }))
                {
                    if (f.HasSubtypeCode(1))
                    {
                        _logger.Debug("Is connected to primary busbar.");
                        // This is a primary bus bar.
                        return false;
                    }
                    else
                        result = true;
                }
                else
                {
                    _logger.Debug("No primary line found.");
                    // Not a primary busbar or primaryconductor.
                    result = true;
                }
            }
            _logger.Debug("ExcludeOpenPoint end.");
            return result;
        }

        /// <summary>
        /// Check to exclude switch features.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>true/false</returns>
        private bool ExcludeSwitch(IFeature feature)
        {
            bool result = false;
            _logger.Debug("ExcludeSwitch started.");

            _logger.Debug("ExcludeSwitch end.");
            return result;
        }
        #endregion
    }


}
