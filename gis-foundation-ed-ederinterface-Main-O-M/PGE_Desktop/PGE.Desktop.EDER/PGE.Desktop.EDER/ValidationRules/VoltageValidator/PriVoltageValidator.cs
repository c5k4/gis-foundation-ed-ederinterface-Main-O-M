using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using PGE.Common.Delivery.Geodatabase;

namespace PGE.Desktop.EDER.ValidationRules.VoltageValidators
{
    /// <summary>
    /// Business logic for validating voltage on primary conductors.  See specification for details.
    /// 
    /// </summary>
    public class PriVoltageValidator : VoltageValidator
    {
        #region Private Variables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// A list of model names which identify StepDown feature classes.
        /// </summary>
        private static readonly string[] _stepDownModelNames = { SchemaInfo.Electric.ClassModelNames.StepDown };
        /// <summary>
        /// Model & Domain Names, QA/QC rule will be available for all features with a field that has the OPERATINGVOLTAGE model name.
        /// </summary>
        private static readonly string _phaseDesingationModelName = SchemaInfo.Electric.FieldModelNames.PhaseDesignation;
        private static readonly string _operatingVoltageModelName = SchemaInfo.Electric.FieldModelNames.OperatingVoltage4PrimaryValidation;
        private static readonly string _outputVoltageModelName = SchemaInfo.Electric.FieldModelNames.SecondaryVoltage;
        private static readonly string _phaseDesignationDomainName = SchemaInfo.General.PhaseDesignationDomainName;
        private static readonly string _primaryVoltageDomainName = SchemaInfo.General.PrimaryVoltageDomainName;

        /// <summary>
        /// Add Prefix to the message so it stands out among all the errors as important. 
        /// This will only be used if the severity is Warning.
        /// </summary>
        private static readonly string _voltagePrefix = "VOLTAGE ISSUE: ";


        /// <summary>
        /// A list features used to determine if the upstream lines are primary or not.
        /// </summary>
        // bug 1503  
        // private static readonly string[] PrimaryOverheadModelNames = { SchemaInfo.Electric.ClassModelNames.OverheadConductor, SchemaInfo.Electric.ClassModelNames.PrimaryConductor };
        // private static readonly string[] _primaryOverheadModelNames = {SchemaInfo.Electric.ClassModelNames.PrimaryConductor };
        private static readonly string[] _primaryModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.PGEBusBar,
            SchemaInfo.Electric.ClassModelNames.PrimaryConductor
            };
        /// <summary>
        /// NominalVoltage field name - a model name doesn't exist.
        /// </summary>
        private static readonly string _nomialVoltageFieldName = "NOMINALVOLTAGE";

        #endregion Private Variables

        #region Public Properties
        /// <summary>
        /// Upstream features used for validation.  Initialized by the VoltageValidatorFactory using traces.
        /// </summary>
        public IFeature ValidatingFeature { get; private set; }

        /// <summary>
        /// Gets the upstream features. Initialized by the VoltageValidatorFactory using traces.
        /// </summary>
        /// Q1-2021 QA/QC voltage rule change for ED-GIS Scripting project, accounts for multiple sourcelines.
        //public IFeature UpstreamFeature { get; private set; }
        public List<IFeature> UpstreamFeatures { get; private set; }
        /// <summary>
        /// Gets the junction feature.  Initialized by the VoltageValidatorFactory using traces.
        /// </summary>
        ///Q1 -2021 QA/QC phase rule change for ADMS - GIS Scripting project - 
        /// Populate when immediate upstream junction is a Stepdown indicating that the output voltage should be 
        /// used instead of the upstream Lines.
        /// public IFeature JunctionFeature { get; private set; }
        public IFeature StepDownFeature { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the source and upstream conductors [has phase change indicating VoltageDrop is needed].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if upstream conductors(combined if multiple) has Three-Phase & source feature has Single-Phase; 
        /// 	otherwise, <c>false</c>.
        /// </value>
        public bool HasPhaseDrop { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the feature is Transformer or VoltageRegulator has an Open Wye unit configuration.
        /// </summary>
        /// <value>
        /// 	<c>true</c> Transformer or VoltageRegulator (Bank or Single) has 2 single phase units; 
        /// 	otherwise, <c>false</c>.
        /// </value>
        public bool HasOpenWyeUnitConfiguration { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the feature has the Primary Voltage domain assigned to the operating voltage field.
        /// </summary>
        /// <value>
        /// 	<c>true</c> has Primary Voltage; 
        /// 	otherwise, <c>false (StreetLight Transformers, OpenPoint (subtype3), etc</c>.
        /// </value>
        public bool IsPrimaryVoltageFeature { get; private set; }

        /// <summary>
        /// Validating Feature's Operating Voltage value, integer primary voltage domain code to be use in voltage comparisons.
        /// </summary>
        /// <value>
        /// 	int - domain code
        /// </value>
        public int OperatingVoltage { get; private set; }


        /// <summary>
        /// Validating Feature's Operating Voltage description, string used for messaging.
        /// </summary>
        /// <value>
        /// 	int - domain code
        /// </value>
        public string OperatingVoltageDescription { get; private set; }

        /// <summary>
        /// Validating Feature's Phase Designation value, integer Phase Designation domain code to be use in voltage comparisons.
        /// </summary>
        /// <value>
        /// 	int - domain code
        /// </value>
        public int Phase { get; private set; }


        /// <summary>
        /// Validating Feature's Phase Designation description, string used for messaging.
        /// </summary>
        /// <value>
        /// 	string - domain description.
        /// </value>
        public string PhaseDescription { get; private set; }


        /// <summary>
        /// Upstream Phase value, integer phase designation domain code to be use in voltage comparisons.
        /// </summary>
        /// <value>
        /// 	If StepDownFeature id present, then PhaseDesgination value will be used. Otherwise, UpstreamFeatures will be
        /// 	used (for multiple features Phases will be combined into a superset (A,B,C would ABC(7)).
        /// </value>
        public int UpstreamPhases { get; private set; }

        /// <summary>
        /// Upstream Phase description, string phase designation domain description to be used for messaging.
        /// </summary>
        /// <value>
        ///     string - domain description
        /// 	If StepDownFeature id present, then PhaseDesgination value will be used. Otherwise, UpstreamFeatures will be 
        /// 	used (for multiple features Phases will be combined into a superset (A,B,C would ABC(7)).
        /// </value>
        public string UpstreamPhasesDescription { get; private set; }


        /// <summary>
        /// Upstream Conductor has Primary, Features with only secondary upstream will be skipped.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the upstream conductors contains at least 1 primary; otherwise, <c>false</c>.
        /// </value>
        public bool UpstreamHasPrimary { get; private set; }

        /// <summary>
        /// When multiple Upstream Conductors are present and the Nominal Voltages do not match, error will occur.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the upstream conductors contains at least 1 primary; otherwise, <c>false</c>.
        /// </value>
        public bool UpstreamHasMultipleVoltages { get; private set; }


        /// <summary>
        /// When multiple Upstream contain Busbars we may need to construct the Nominal voltage to compare if 
        /// mutliple lines exist.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the upstream conductors contains at least 1 busbar; otherwise, <c>false</c>.
        /// </value>
        public bool UpstreamHasBusbars { get; private set; }

        /// <summary>
        /// Upstream Voltage value, integer primary voltage domain code to be use in voltage comparisons.
        /// </summary>
        /// <value>
        /// 	If StepDownFeature id present, then Output Voltage value will be used.
        /// 	Otherwise, UpstreamFeatures will be used:
        /// 	    (for multiple features Nominal Voltage will be used) 
        /// 	    (for a single feature the Operating Voltage value will be used).
        /// </value>
        public int UpstreamVoltage { get; private set; }


        /// <summary>
        /// Upstream Voltage description, string used for messaging.
        /// </summary>
        /// <value>
        /// 	UpstreamVoltage domain code will be used to determine the description from the primary voltage domain.
        /// </value>
        public string UpstreamVoltageDescription { get; private set; }

        #region Old properties prior to Q1 -2021 QA/QC phase rule change for ADMS
        /// Q1 -2021 QA/QC phase rule change for ADMS - GIS Scripting project - 
        /// OLD Properties
        ///// <summary>
        ///// Gets a value indicating whether this instance has step down feature present.
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if this instance has step down feature present; otherwise, <c>false</c>.
        ///// </value>
        //public bool IsStepDownFeaturePresent { get; private set; }

        ///// <summary>
        ///// Gets a value indicating whether this instance has step down feature present.
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if this instance has step down feature present; otherwise, <c>false</c>.
        ///// </value>
        //public bool IsJunctionBooster { get; private set; }

        ///// <summary>
        ///// Gets a value indicating whether the source and upstream conductors [have same number of phases].
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if the conductors [have same number of phases]; otherwise, <c>false</c>.
        ///// </value>
        //public bool HaveSameNumberOfPhases   { get; private set;}



        ///// <summary>
        ///// Gets a value indicating whether the source and upstream conductors [have neutral conductors].
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if the source and upstream conductors [have neutral conductors]; otherwise, <c>false</c>.
        ///// </value>
        //public bool HaveNeutralConductors    { get; private set;}

        ///// <summary>
        ///// Gets a value indicating whether source and upstream conductors [have same primary voltage].
        ///// </summary>
        ///// <value>
        ///// 	<c>true</c> if the source and upstream conductors [have same primary voltage]; otherwise, <c>false</c>.
        ///// </value>
        //public bool HaveSamePrimaryVoltage   { get; private set;}
        #endregion
        
        
        #endregion Public Properties

        #region Initialization
        /// <summary>
        /// Use the PriVoltageValidator(List<IFeature> upstream, IFeature downstream, IFeature stepdown)constructor. 
        /// Initializes a new instance of the <see cref="PriVoltageValidator"/> class. 
        /// </summary>
        protected PriVoltageValidator() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriVoltageValidator"/> class.
        /// </summary>
        /// <param name="upstream">The immediate upstream conductors.</param>
        /// <param name="downstream">The downstream feature.</param>
        /// <param name="stepdown">The immediate upstream stepdown feature. Stepdown's Output Voltage 
        /// will be used instead of the upstream lines when there's one connected immediately upstream.</param>
        public PriVoltageValidator(List<IFeature> upstream, IFeature downstream, IFeature stepdown = null)
        {

            ValidatingFeature = downstream;
            if (upstream != null)
            {
                UpstreamFeatures = upstream;
            }
            else
            {
                UpstreamFeatures = new List<IFeature>();
            }
            StepDownFeature = stepdown;
            IsPrimaryVoltageFeature = HasPrimaryVoltageDomain();
            if (IsPrimaryVoltageFeature)
            {
                Initialize();
            }

        }

        /// <summary>
        /// Initializes this instance by evaluating prerequisite metadata. 
        /// </summary>
        protected void Initialize()
        {
            UpstreamHasPrimary = HasPrimaryUpstream();
            UpstreamPhases = initUpstreamPhases();
            UpstreamPhasesDescription = LookupDomainDescription(_phaseDesignationDomainName, UpstreamPhases.ToString(), "").ToString();
            UpstreamVoltage = initUpstreamVoltage();
            UpstreamVoltageDescription = LookupDomainDescription(_primaryVoltageDomainName, UpstreamVoltage.ToString(), "").ToString();
            OperatingVoltage = ValidatingFeature.GetFieldValue(useDomain: false, modelName: _operatingVoltageModelName).Convert<int>(-1);
            OperatingVoltageDescription = LookupDomainDescription(_primaryVoltageDomainName, OperatingVoltage.ToString(), "").ToString();
            Phase = ValidatingFeature.GetFieldValue(useDomain: false, modelName: _phaseDesingationModelName).Convert<int>(0);
            PhaseDescription = LookupDomainDescription(_phaseDesignationDomainName, Phase.ToString(), "").ToString();
            HasPhaseDrop = initPhaseDrop();
            HasOpenWyeUnitConfiguration = UnitHelper.IsOpenWyeConfiguration(ValidatingFeature);



        }
        #endregion Initialization


        #region Public Methods
        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public List<string> Validate(int severity)
        {
            // Add a prefix to the message to make it stand out if it's a warning.
            string voltagePrefix = "";
            if (severity == 1)
            {
                voltagePrefix = _voltagePrefix;
            }
            string upstreamOID = GetUpstreamOIDs();


            // Error Message strings.
            string _nullAttributesErrorMessage = "{0}Cannot verify Operating Voltage: {1}.";
            string _multiNominalErrorMessage = "{0}Upstream SourceLines have conflicting {1} Voltages, Sources: ({2}).";
            string _openWyeErrorMessage = "{0}Operating Voltage should be {1} when using (2) single phase units connected to {2} {3} Source: ({4})";
            string _voltageDropErrorMessage = "{0}Operating Voltage not valid for single phase feature based on source feature phasing. Source: ({1})";
            string _sameVoltageErrorMessage = "{0}Operating Voltage should match the source feature: ({1}).";
            string _sameVoltageMulitpleSourcesErrorMessage = "{0}Operating Voltage should be ({1}) when more than 1 phase is energized.";
            string _phaseErrorMessage = "{0}Single phase features should not exist on {1}. Source: ({2})";
            string _phaseOpenWyeErrorMessage = "{0}Open Wye {1} (2) single phase units should not exist on a {2} Circuit.";
            string _singlePhaseErrorMessage = "{0}Operating Voltage not valid for single phase feature based on source feature phasing. Source: ({1})";
            string _multiPhaseErrorMessage = "{0}{1} Operating Voltage should only be used for single phase features. Source: ({2})";

            string _singlePhaseBankedErrorMessage = "{0} Single Phase Banked Voltage Regulator Operating Voltage should equal upstream Operating Voltage. Source: ({1})";
            // Single Phase Banked VoltageRegulators Operating Voltage should equal upstream Operating Voltage.
            List<string> errors = new List<string>();

            var tmp = VoltageHelper.PrimaryVoltageDropConfiguration;

            if ((IsPrimaryVoltageFeature == false) || (UpstreamHasPrimary == false))
            {
                // Skip Validation if doesn't have primary upstream or has primary voltage domain assigned.
                if (IsPrimaryVoltageFeature == false)
                {
                    _logger.Info(string.Format("{0} OID: {1} Doesn't have the 'Primary Voltage' domain assigned to the Operating Voltage field, skipping Validate Primary Voltage Rule.", ValidatingFeature.Class.AliasName, ValidatingFeature.OID));
                }
                else if (UpstreamHasPrimary == false)
                {
                    _logger.Info(string.Format("{0} OID: {1} Doesn't have any primary upstream, skipping Validate Primary Voltage Rule.", ValidatingFeature.Class.AliasName, ValidatingFeature.OID));
                }


            }
            else if (UpstreamHasMultipleVoltages == true)
            {
                // If there are mutlipel upstream Lines with differnt Nominal Voltages.
                string fieldDescription = "";
                if (UpstreamHasBusbars)
                {
                    fieldDescription = "Circuit";
                }
                else
                {
                    fieldDescription = "Nominal";
                }
                errors.Add(string.Format(_multiNominalErrorMessage, voltagePrefix, fieldDescription, GetUpstreamOIDs()));
            }
            // Check for null values.
            else if ((UpstreamPhases == 0) || (Phase == 0) || (UpstreamVoltage == -1) || (OperatingVoltage == -1))
            {
                string nullDetails = GetNullAttributesErrorDetail();
                errors.Add(string.Format(_nullAttributesErrorMessage, voltagePrefix, nullDetails));

            }
            else if (HasPhaseDrop || HasOpenWyeUnitConfiguration)
            {
                // This indicates that there should be voltageDrop (OperatingVoltage / √(3) ) because either: 
                // 1) the upstream phase is TwoPhase or ThreePhase AND the validating feature is single phase    
                // -OR-
                // 2) the validating feature is a Transformer, Stepdown, or VoltageRegulator AND has 2 Single-Phase unit records,
                //      which is an Open Wye Unit configuration.

                // BEC Update 10/14/2021 - Update of Operating Voltage rule for Banked Voltage Regulators
                bool isBanked = UnitHelper.IsBankedVoltageRegulator(ValidatingFeature);
                int expectedVoltageDrop = -1;

                // Voltage regulators should match 1:1 on voltage, drops are not allowed.
                //  set expected voltage to upstream voltage.
                if (ModelNameFacade.ContainsClassModelName(ValidatingFeature.Class, SchemaInfo.Electric.ClassModelNames.VoltageRegulator))
                {
                    expectedVoltageDrop = UpstreamVoltage;
                }
                else
                {
                    expectedVoltageDrop = VoltageHelper.GetValidPrimaryVoltageDropCode(UpstreamVoltage);
                }

                if (expectedVoltageDrop == -1)
                {
                    // There is no valid voltage drop for the upstreamVoltage.
                    if (HasOpenWyeUnitConfiguration)
                    {
                        errors.Add(string.Format(_phaseOpenWyeErrorMessage, voltagePrefix, ValidatingFeature.Class.AliasName, UpstreamVoltageDescription));
                    }
                    else if (HasPhaseDrop)
                    {
                        errors.Add(string.Format(_phaseErrorMessage, voltagePrefix, OperatingVoltageDescription, upstreamOID));
                    }


                }
                else
                {
                    if (OperatingVoltage != expectedVoltageDrop)
                    {
                        // The voltage should match the valid voltage drop value when one exists.                       
                        string expectedVoltageDescription = LookupDomainDescription(_primaryVoltageDomainName, expectedVoltageDrop.ToString(), "").ToString();
                        if (HasOpenWyeUnitConfiguration)
                        {

                            errors.Add(string.Format(_openWyeErrorMessage, voltagePrefix, expectedVoltageDescription, UpstreamVoltageDescription, UpstreamPhasesDescription, upstreamOID));

                        }
                        else if (HasPhaseDrop)
                        {
                            if (isBanked && (PhaseDescription.Length == 1))
                            {
                                if (OperatingVoltage != UpstreamVoltage)
                                    errors.Add(string.Format(_singlePhaseBankedErrorMessage, voltagePrefix, upstreamOID));
                            }
                            else 
                                errors.Add(string.Format(_voltageDropErrorMessage, voltagePrefix, upstreamOID));
                        }
                    }
                }


            }
            else
            {
                // Otherwise it should equal the circuit voltage (which "should" be the upstream voltage).

                if (OperatingVoltage != UpstreamVoltage)
                {
                    if (UpstreamFeatures.Count > 1)
                    {
                        // Different message for multiple upstream lines to avoid confusion.
                        errors.Add(string.Format(_sameVoltageMulitpleSourcesErrorMessage, voltagePrefix, UpstreamVoltageDescription));
                    }
                    else
                    {
                        // Otherwise it should equal the upstream Operating Voltage
                        errors.Add(string.Format(_sameVoltageErrorMessage, voltagePrefix, upstreamOID));
                    }
                }
                else
                {
                    // If the value matches the source let's double check that the voltage is appropriate for the phase.
                    if (PhaseDescription.Length == 1)
                    {
                        // BEC Update 10/14/2021 - banked voltage reg
                        bool isBanked = UnitHelper.IsBankedVoltageRegulator(ValidatingFeature);

                        
                        if (!isBanked && !VoltageHelper.IsSinglePhaseVoltageValid(OperatingVoltage))
                        {

                            errors.Add(string.Format(_singlePhaseErrorMessage, voltagePrefix, upstreamOID));
                        }
                        else if (isBanked && (OperatingVoltage != UpstreamVoltage))
                        {
                            errors.Add(string.Format(_singlePhaseBankedErrorMessage, voltagePrefix, upstreamOID));
                        }
                    }
                    else if (PhaseDescription.Length > 1)
                    {
                        if (!VoltageHelper.IsMultiPhaseVoltageValid(OperatingVoltage))
                        {
                            errors.Add(string.Format(_multiPhaseErrorMessage, voltagePrefix, OperatingVoltageDescription, upstreamOID));
                        }
                    }
                }
            }


            return errors;
        }
        
        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Gets Upstream Phases (combines multiple upstream lines).
        /// </summary>
        /// <returns>int - phase designation code</returns>
        private int initUpstreamPhases()
        {
            int upstreamPhaseDesignation = 0;
            if (StepDownFeature != null)
            {
                upstreamPhaseDesignation = StepDownFeature.GetFieldValue(useDomain: false, modelName: _phaseDesingationModelName).Convert<int>(0);
            }
            else
            {
                foreach (IFeature upstreamLine in UpstreamFeatures)
                {
                    // Only include upstream secondary lines.
                    if (IsPrimaryLine(upstreamLine))
                    {
                        int upstreamPhase = upstreamLine.GetFieldValue(useDomain: false, modelName: _phaseDesingationModelName).Convert<int>(0);
                        // Bitwise OR to combine all phases into one superset.
                        upstreamPhaseDesignation = upstreamPhaseDesignation | upstreamPhase;
                    }
                }
            }

            return upstreamPhaseDesignation;

        }

        /// <summary>
        /// Determines if any of the upstream lines are primary features.
        /// </summary>
        /// <returns><c>true</c> if upstream conductors(combined if multiple) has any primary lines. 
        /// otherwise, <c>false</c>.
        /// </returns>
        private bool HasPrimaryUpstream()
        {
            bool hasPrimary = false;
            foreach (IFeature line in UpstreamFeatures)
            {
                if (IsPrimaryLine(line))
                {
                    hasPrimary = true;
                }
            }
            return hasPrimary;
        }

        /// <summary>
        /// Checks to see if a line is a primary conductor or a primary busbar.
        /// </summary>
        /// <param name="line">upstream line</param>
        /// <returns>true if it's primary or false if it's not</returns>
        private bool IsPrimaryLine(IFeature line)
        {
            bool isPrimary = false;

            if (ModelNameFacade.ContainsClassModelName(line.Class, _primaryModelNames))
            {
                if (ModelNameFacade.ContainsClassModelName(line.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar))
                {
                    // Case for Busbar.
                    if (line.HasSubtypeCode(1)) 
                    {
                        // PrimaryBusBar.
                        isPrimary = true;
                    }
                }
                else
                {
                    isPrimary = true;
                }
            }
            return isPrimary;

        }
        /// <summary>
        /// Gets the Upstream Voltage, considers multiple upstream lines when present.
        /// </summary>
        /// <returns>Primary Voltage Code</returns>
        private int initUpstreamVoltage()
        {
            int upstreamVoltage = -1;
            UpstreamHasMultipleVoltages = false;

            if (StepDownFeature != null)
            {
                upstreamVoltage = StepDownFeature.GetFieldValue(useDomain: false, modelName: _outputVoltageModelName).Convert<int>(-1);
            }
            else
            {
                // Upstream Lines if there isn't a stepdown.
                if (UpstreamFeatures.Count == 1)
                {
                    // If only one upstream line just get the operating voltage.
                    IFeature upstreamLine = UpstreamFeatures.FirstOrDefault();
                    if (IsPrimaryLine(upstreamLine))
                    {
                        upstreamVoltage = upstreamLine.GetFieldValue(useDomain: false, modelName: _operatingVoltageModelName).Convert<int>(-1);
                    }
                }
                // If there are mutliple upstream lines present need to do more work.
                else if (UpstreamFeatures.Count > 1)
                {
                    List<int> nominalVoltages = new List<int>();
                    UpstreamHasBusbars = false;

                    // Check for different nominal voltages if there are multiple lines upstream.
                    foreach (IFeature upstreamLine in UpstreamFeatures)
                    {
                        if (IsPrimaryLine(upstreamLine))
                        {
                            if (ModelNameFacade.ContainsClassModelName(upstreamLine.Class, SchemaInfo.Electric.ClassModelNames.PGEBusBar))
                            {
                                // Busbars do not have Nominal voltages so we need to calculate what it
                                // would be based on the phase.
                                UpstreamHasBusbars = true;
                                int nominalVoltage = -1;
                                int opVoltage = upstreamLine.GetFieldValue(useDomain: false, fieldName: _operatingVoltageModelName).Convert<int>(-1);
                                string phase = upstreamLine.GetFieldValue(useDomain: true, fieldName: _phaseDesingationModelName).Convert<string>("");
                                if (phase.Length == 1)
                                {
                                    // Need to get the circuit voltage if it's single phase.
                                    nominalVoltage = VoltageHelper.GetValidPrimaryCircuitVoltageCode(opVoltage);
                                }
                                else
                                {
                                    // If it's multi-phase that the operating voltage would be the same as the circuit voltage.
                                    nominalVoltage = opVoltage;
                                }
                                if (!nominalVoltages.Contains(nominalVoltage))
                                {
                                    nominalVoltages.Add(nominalVoltage);
                                }
                            }
                            else
                            {
                                // If its primary then get the Nominal voltage.
                                int nominalVoltage = upstreamLine.GetFieldValue(useDomain: false, fieldName: _nomialVoltageFieldName).Convert<int>(-1);
                                if (!nominalVoltages.Contains(nominalVoltage))
                                {
                                    nominalVoltages.Add(nominalVoltage);
                                }
                            }
                        }
                    }

                    if (nominalVoltages.Count == 1)
                    {
                        // Use nominal voltage if combined phase is Multi-Phase.
                        if (UpstreamPhasesDescription.Length > 1)
                        {
                            upstreamVoltage = nominalVoltages.FirstOrDefault();

                        }
                        // Use the voltage drop value if combined phase is Single-Phase.
                        else if (UpstreamPhasesDescription.Length == 1)
                        {
                            upstreamVoltage = VoltageHelper.GetValidPrimaryVoltageDropCode(nominalVoltages.FirstOrDefault());
                        }
                    }
                    else if (nominalVoltages.Count > 1)
                    {
                        // Specific error for mutliple upstream line with conflicting nominal voltages.
                        UpstreamHasMultipleVoltages = true;
                    }
                }

            }

            return upstreamVoltage;

        }


        /// <summary>
        /// Determines if the Phase has changed from MultiPhase to Single Phase, used to determine if Circuit 
        /// or VoltageDrop values should be used.
        /// </summary>
        /// <returns></returns>
        private bool initPhaseDrop()
        {
            bool hasPhaseDrop = false;
            if ((UpstreamPhasesDescription.Length > 1) && (PhaseDescription.Length == 1))
            {
                hasPhaseDrop = true;
            }
            return hasPhaseDrop;
        }

        /// <summary>
        /// Determines if the Operating Voltage field has the Primary Voltage domain assigned. 
        /// Some subtypes have secondary voltage assigned, so need to check if we should skip validation.
        /// </summary>
        /// <returns>true or false</returns>
        private bool HasPrimaryVoltageDomain()
        {
            bool hasDomain = false;
            // Determine if the field has a domain for the given subtype.
            IField field = GDBFacade.GetFieldFromModelName(ValidatingFeature.Class, _operatingVoltageModelName);
            string fieldDomainName = "";
            if (field.Domain != null)
            {
                fieldDomainName = field.Domain.Name;
            }
            IRowSubtypes rowSubtype = ValidatingFeature as IRowSubtypes;
            if (rowSubtype != null)
            {
                int subtypeCode = rowSubtype.SubtypeCode;
                ISubtypes subtypes = ValidatingFeature.Table as ISubtypes;
                IDomain assignedDomain = subtypes.get_Domain(subtypeCode, field.Name);
                if (assignedDomain != null)
                {
                    fieldDomainName = assignedDomain.Name;
                }
            }
            if (fieldDomainName == _primaryVoltageDomainName)
            {
                hasDomain = true;
            }
            return hasDomain;

        }

        /// <summary>
        /// Attempts to match the domain code to a description, optionally returning a default string if the code is not found.
        /// If no default value string is provided, null is returned.
        /// </summary>
        /// <param name="domainName">The domain to search on.</param>
        /// <param name="domainCode">The code to look for in the domain.</param>
        /// <param name="defaultValue">An optional default value to return in liu of a result.</param>
        /// <returns></returns>
        private object LookupDomainDescription(string domainName, string domainCode, string defaultValue = null)
        {
            IDomain domain = GDBFacade.FindByName((ValidatingFeature.Class as IDataset).Workspace, domainName);
            var codedValueDomain = domain as ICodedValueDomain;
            if (codedValueDomain != null)
            {
                for (var i = 0; i < codedValueDomain.CodeCount; i++)
                {
                    var code = codedValueDomain.Value[i].Convert(String.Empty);
                    if (domainCode.Equals(code)) return codedValueDomain.Name[i];
                }
            }
            return defaultValue;
        }

        
        /// <summary>
        /// Gets Null field details for null error message.
        /// </summary>
        /// <returns>message string</returns>
        private string GetNullAttributesErrorDetail()
        {
            string retValue = "";
            List<string> returnList = new List<string>();
            if (Phase == 0)
            {
                returnList.Add("Class: " + ValidatingFeature.Class.AliasName + " OID: " + ValidatingFeature.OID.ToString() + " Phase Designation is null");
            }
            if (OperatingVoltage == -1)
            {
                returnList.Add("Class: " + ValidatingFeature.Class.AliasName + " OID: " + ValidatingFeature.OID.ToString() + " Operating Voltage is null");
            }
            if ((UpstreamPhases == 0) || (UpstreamVoltage == -1))
            {
                returnList.Add(GetUpstreamErrorDetail());
            }
            retValue = string.Join(", ", returnList.ToArray());
            return retValue;
        }

        /// <summary>
        /// Gets the Upstream Source FC & OIDs for error messages.
        /// </summary>
        /// <returns></returns>
        private string GetUpstreamOIDs()
        {
            List<string> OIDs = new List<string>();
            if (StepDownFeature == null)
            {
                foreach (var feature in UpstreamFeatures)
                {
                    OIDs.Add(feature.Class.AliasName + " " + feature.OID.ToString());

                }
            }
            else
            {
                OIDs.Add(StepDownFeature.Class.AliasName + " " + StepDownFeature.OID.ToString());
            }
            return string.Join(", ", OIDs.ToArray());
        }

        /// <summary>
        /// Gets upstream error details for null error message.
        /// </summary>
        /// <returns></returns>
        private string GetUpstreamErrorDetail()
        {
            string retValue = "";
            if (StepDownFeature == null)
            {
                // Upstream line is the source.
                if (UpstreamFeatures.Count == 1)
                {
                    var feature = UpstreamFeatures.FirstOrDefault();

                    retValue = feature.Class.AliasName + " OID: " + feature.OID.ToString();
                    if (string.IsNullOrEmpty(UpstreamPhasesDescription))
                    {
                        retValue += " Phase is null";
                    }
                    if (string.IsNullOrEmpty(UpstreamVoltageDescription))
                    {
                        retValue += " Operating Voltage is null";
                    }

                }
                // Multiple upstream lines are the source.
                else if (UpstreamFeatures.Count > 1)
                {
                    List<string> OIDs = new List<string>();
                    foreach (var feature in UpstreamFeatures)
                    {
                        OIDs.Add(feature.OID.ToString());

                    }
                    retValue = "OIDs: " + string.Join(",", OIDs.ToArray());
                    if (string.IsNullOrEmpty(UpstreamPhasesDescription))
                    {
                        retValue += " Phase is null";
                    }
                    if (string.IsNullOrEmpty(UpstreamVoltageDescription))
                    {
                        retValue += " Operating Voltage is null";
                    }


                }


            }
            else
            {
                // Upstream feature is a StepDown.
                retValue = StepDownFeature.Class.AliasName + " OID: " + StepDownFeature.OID.ToString();
                if (string.IsNullOrEmpty(UpstreamPhasesDescription))
                {
                    retValue += " Phase is null";
                }
                if (string.IsNullOrEmpty(UpstreamVoltageDescription))
                {
                    retValue += " Output Voltage is null";
                }

            }
            return retValue;
        }
        #endregion Private Methods

        #region Old code prior to Q1 -2021 QA/QC phase rule change for ED-GIS Scripting project
        ///// <summary>
        ///// Initializes a new instance of the <see cref="PriVoltageValidator"/> class.
        ///// </summary>
        ///// <param name="upstream">The upstream conductor.</param>
        ///// <param name="downstream">The downstream conductor.</param>
        ///// <param name="junction">The junction feature.</param>
        //public PriVoltageValidator(IFeature upstream, IFeature downstream, IFeature junction)
        //{
        //    SourceFeature = downstream;
        //    UpstreamFeature = upstream;
        //    JunctionFeature = junction;

        //    Initialize();
        //}

        ///// <summary>
        ///// Initializes this instance by evaluating prerequisite metadata. 
        ///// </summary>
        //protected void Initialize() 
        //{
        //    HaveSameNumberOfPhases = initHaveSameNumberOfPhases();
        //    HaveNeutralConductors = initHaveNeutralConductors();
        //    HaveSamePrimaryVoltage = initHaveSamePrimaryVoltage();
        //    IsJunctionBooster = initIsJunctionBooster();
        //    IsStepDownFeaturePresent = initIsStepDownFeaturePresent();
        //}

        ///// <summary>
        ///// Checks to see if stepdown feature is present by testing the junction features model name against the stepdown model name list.
        ///// </summary>
        ///// <returns></returns>
        //private bool initIsStepDownFeaturePresent()
        //{
        //    if (IsJunctionBooster) return true;
        //    return ModelNameFacade.ContainsClassModelName(JunctionFeature.Class, StepDownModelNames);
        //}

        ///// <summary>
        ///// Determines if the conductors have the same number of phases, by comparing the subtype code.
        ///// </summary>
        ///// <returns></returns>
        //private bool initHaveSameNumberOfPhases()
        //{

        //    IRowSubtypes srcRowSubtype = SourceFeature as IRowSubtypes;
        //    IRowSubtypes upRowSubtype = UpstreamFeature as IRowSubtypes;

        //    return (srcRowSubtype.SubtypeCode == upRowSubtype.SubtypeCode || (srcRowSubtype.SubtypeCode >= 4 && upRowSubtype.SubtypeCode >= 4));

        //}

        ///// <summary>
        ///// Determines if both conductors have a neutral wire. 
        ///// </summary>
        ///// <returns></returns>
        //private bool initHaveNeutralConductors()
        //{
        //    return (hasConductorUseNetural(SourceFeature) && hasConductorUseNetural(UpstreamFeature));
        //}

        ///// <summary>
        ///// Determines if the conductors have the same primary voltage.
        ///// </summary>
        ///// <returns></returns>
        //private bool initHaveSamePrimaryVoltage()
        //{
        //    // Get primary voltage indexes
        //    int sourcePrimayVoltageIx = ModelNameFacade.FieldIndexFromModelName(SourceFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
        //    int upstreamPrimayVoltageIx = ModelNameFacade.FieldIndexFromModelName(UpstreamFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);

        //    // Compare values.
        //    return SourceFeature.get_Value(sourcePrimayVoltageIx) as int? == UpstreamFeature.get_Value(upstreamPrimayVoltageIx) as int?;
        //}

        ///// <summary>
        ///// Determines if the conductors have the same primary voltage.
        ///// </summary>
        ///// <returns></returns>
        //private bool initIsJunctionBooster()
        //{
        //    if (!ModelNameFacade.ContainsClassModelName(JunctionFeature.Class, new string[] { SchemaInfo.Electric.ClassModelNames.VoltageRegulator })) return false;

        //    IRowSubtypes subtype = JunctionFeature as IRowSubtypes;

        //    return subtype.SubtypeCode == 3;

        //    //return ModelNameFacade.ContainsClassModelName(JunctionFeature.Class, new string[] { SchemaInfo.Electric.ClassModelNames.VoltageRegulator });
        //}

        ///// <summary>
        ///// Determines whether the conductor ([the specified feature]) [has conductor use netural] .
        ///// </summary>
        ///// <param name="feature">The feature.</param>
        ///// <returns>
        /////   <c>true</c> if [has conductor use netural] [the specified feature]; otherwise, <c>false</c>.
        ///// </returns>
        //private bool hasConductorUseNetural(IFeature feature)
        //{

        //    if (!ModelNameFacade.ContainsAllClassModelNames(feature.Class, PrimaryOverheadModelNames)) return false;

        //    return NeutralPhaseHelper.Instance.HasNeutral(feature);

        //    //// Find the conductor - conductor_info table.
        //    //String relClassName = RelClassResolver.Instance.GetRelClassName((feature.Class as IDataset).Name);

        //    //IRelationshipClass relClass = ((IFeatureWorkspace)((IDataset)feature.Class).Workspace).OpenRelationshipClass(relClassName);

        //    //// Get the related object 
        //    //ISet relatedObjects = relClass.GetObjectsRelatedToObject(feature);

        //    //relatedObjects.Reset();

        //    //IObject relObject = null;

        //    //// Fund the conductor use field.
        //    //// bug 1496: changed from SchemaInfo.Electric.FieldModelNames.PhaseDesignation to SchemaInfo.Electric.FieldModelNames.CondutorUse
        //    //int conductorUseIx = ModelNameFacade.FieldIndexFromModelName(relClass.DestinationClass, SchemaInfo.Electric.FieldModelNames.CondutorUse);

        //    //int value;

        //    //while ((relObject = relatedObjects.Next() as IObject) != null)
        //    //{
        //    //    object conductorUseObj = relObject.get_Value(conductorUseIx);
        //    //    if (conductorUseObj is System.DBNull)
        //    //        continue;

        //    //    value = Convert.ToInt32(conductorUseObj);

        //    //    if (value == 2) return true;
        //    //}

        //    //return false;
        //}

        ///// <summary>
        ///// Validates state where no stepdown feature is present between the source and upstream conductor.
        ///// </summary>
        ///// <param name="errors">The errors.</param>
        //private void validateFewerPhasesUpstreamStepDownAbsent(List<string> errors)
        //{ 
        //    int sourcePrimayVoltageIx = ModelNameFacade.FieldIndexFromModelName(SourceFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
        //    int upstreamPrimayVoltageIx = ModelNameFacade.FieldIndexFromModelName(UpstreamFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);

        //    // No voltage drop.
        //    if (SourceFeature.get_Value(sourcePrimayVoltageIx) == UpstreamFeature.get_Value(upstreamPrimayVoltageIx)) return;

        //    int? upVoltage = UpstreamFeature.get_Value(upstreamPrimayVoltageIx) as int?;
        //    int? downVoltage = SourceFeature.get_Value(sourcePrimayVoltageIx) as int?;

        //    //check if the voltage is the same. If the voltage has not changed return.
        //    //Added to fix bugzilla bug 685 
        //    if (upVoltage == downVoltage) return;

        //    //0 : 480 V
        //    //1	: 2.4 kV
        //    //2	: 4.16 kV
        //    //3	: 4.8 kV
        //    //4	: 7.2 kV
        //    //5	: 12.0 kV
        //    //6	: 17.2 kV
        //    //7	: 22 kV
        //    //8	: 21 kV
        //    //9	: 44.0 kV
        //    //10: 34.5 kV
        //    //11: 19.2


        //    if (!(
        //        (upVoltage == 2 && downVoltage == 1) ||
        //        (upVoltage == 4 && downVoltage == 2) ||
        //        (upVoltage == 5 && downVoltage == 4) ||
        //        (upVoltage == 8 && downVoltage == 5) 
        //        )) 
        //    {

        //        StringBuilder sb = new StringBuilder();

        //        errors.Add(buildErrorString("Load line voltage is not allowed with current sourcing line", UpstreamFeature));

        //    }

        //}

        ///// <summary>
        ///// Validates the state where a stepdown feature is present between the source and upstream features.
        ///// </summary>
        ///// <param name="errors">The errors.</param>
        //private void validateStepDownPresent(List<string> errors)
        //{
        //    if (IsJunctionBooster) return;

        //    // Find field indexes.
        //    int upstreamPrimayVoltageIx = ModelNameFacade.FieldIndexFromModelName(UpstreamFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
        //    int loadPrimaryVoltageIx = ModelNameFacade.FieldIndexFromModelName(SourceFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);

        //    int stepDownPrimaryIx = ModelNameFacade.FieldIndexFromModelName(JunctionFeature.Class, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
        //    int stepDownOutputIx = ModelNameFacade.FieldIndexFromModelName(JunctionFeature.Class, SchemaInfo.Electric.FieldModelNames.SecondaryVoltage);

        //    // Verify that the upstream features voltage matches the stepdown feature's highside voltage and the source voltage matches the stepdown lowside voltage.
        //    if (UpstreamFeature.get_Value(upstreamPrimayVoltageIx) as int? != JunctionFeature.get_Value(stepDownPrimaryIx) as int? &&
        //        SourceFeature.get_Value(loadPrimaryVoltageIx) as int? != JunctionFeature.get_Value(stepDownOutputIx) as int?)
        //    {

        //        StringBuilder sb = new StringBuilder();

        //        errors.Add(buildErrorString("Load line voltage is not allowed with current StepDown voltage and Output Voltage ", UpstreamFeature));
        //    }
        //    else if (UpstreamFeature.get_Value(upstreamPrimayVoltageIx) as int? != JunctionFeature.get_Value(stepDownPrimaryIx) as int? &&
        //       SourceFeature.get_Value(loadPrimaryVoltageIx) as int? == JunctionFeature.get_Value(stepDownOutputIx) as int?)
        //    {
        //        errors.Add(buildErrorString("Load line voltage is not allowed with current StepDown voltage", UpstreamFeature));
        //    }
        //    else if (UpstreamFeature.get_Value(upstreamPrimayVoltageIx) as int? == JunctionFeature.get_Value(stepDownPrimaryIx) as int? &&
        //       SourceFeature.get_Value(loadPrimaryVoltageIx) as int? != JunctionFeature.get_Value(stepDownOutputIx) as int?)
        //    {
        //        errors.Add(buildErrorString("Load line voltage is not allowed with current StepDown Output Voltage ", UpstreamFeature));
        //    }


        //}

        ///// <summary>
        ///// Validates state where no stepdown feature is present.
        ///// </summary>
        ///// <param name="errors">The errors.</param>
        //private void validateStepDownAbsent(List<string> errors)
        //{
        //    // If the conductors have a neutral wire return.
        //    if (HaveNeutralConductors) return;

        //    if (HaveSameNumberOfPhases)
        //    {
        //        // No step down feature is present and phases match check for equal voltage.
        //        if (HaveSamePrimaryVoltage)
        //        {
        //            return;
        //        }
        //        else
        //        {
        //            errors.Add(buildErrorString("Load line voltage does not match sourcing line", UpstreamFeature));
        //        }
        //    }
        //    else {
        //        // No step down feature is present check for a valid voltage drop.
        //        validateFewerPhasesUpstreamStepDownAbsent(errors);
        //    }
        //}

        ///// <summary>
        ///// Validates states where a step down feature is preset between the source and upstream feature.
        ///// </summary>
        ///// <param name="errors">The errors.</param>
        //private void validateStepDown(List<string> errors)
        //{
        //    if (IsStepDownFeaturePresent)
        //    {
        //        // Theres a step down feature check to see if the coductor voltages match the Stepdown feature.
        //        validateStepDownPresent(errors);
        //    }
        //    else { 
        //        // No stepdown feature is present check for phase differences.
        //        validateStepDownAbsent(errors);
        //    }

        //}

        ///// <summary>
        ///// Validates states where a neutral conductor is present.
        ///// </summary>
        ///// <param name="errors">The errors.</param>
        //private void validateNeutralConductor(List<string> errors)
        //{
        //    // Neutral conductor is present.  do not generate an error.
        //    if (HaveNeutralConductors) return;

        //    errors.Add(buildErrorString("Load line voltage is not allowed with current StepDown voltage or Output Voltage", UpstreamFeature));
        //}


        ///// <summary>
        ///// Validates conductors with the same number of phases.
        ///// </summary>
        ///// <param name="errors">The errors.</param>
        //private void validateSamePhasesUpstream(List<string> errors)
        //{
        //    // see bug 1506 and 1507
        //    //if (HaveSamePrimaryVoltage) return;

        //    validateStepDown(errors);

        //}

        ///// <summary>
        ///// Validates this instance.
        ///// </summary>
        ///// <returns></returns>
        //public List<string> Validate() {

        //    List<string> errors = new List<string>();

        //    if (HaveSameNumberOfPhases)
        //    {
        //        validateSamePhasesUpstream(errors);
        //    }
        //    else {
        //        validateStepDown(errors);
        //    }

        //    return errors;
        //}
        
        #endregion
    }
}
