using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Unit Helper Class for Transformer, Stepdowns and VoltageRegulator Unit Records.
    /// 
    /// </summary>
    public class UnitHelper
    {
        #region Class Variables
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static readonly string _phaseDomainName = SchemaInfo.General.PhaseDesignationDomainName;
        private static readonly string[] _shellModelNames = new string[] {
            SchemaInfo.Electric.ClassModelNames.PGETransformer,
            SchemaInfo.Electric.ClassModelNames.VoltageRegulator,
            SchemaInfo.Electric.ClassModelNames.StepDown
        };
        public static Dictionary<string, string> PhaseDesignationDomain = null;
        #endregion Class Variables

        #region Public Methods
        /// <summary>
        /// Validate shell(feature) to unit record(s). 
        /// </summary>
        /// <param name="row"></param>
        /// <returns>List of any errors.</returns>
        public static List<string> ValidateUnits(IRow row)
        {
            _logger.Debug("Starting Unit Record Validation");
            List<string> errors = new List<string>();

            //var obj = row as IFeature;
            if (row == null)
            {
                return errors;
            }
            // Get the TX or VR feature (shell) with the unit info and other metadata.
            var shellRecord = UnitHelper.CreateShellInstance(row);
            if (shellRecord == null)
            {
                string tableName = (row as IObject).Class.AliasName;
                int oid = row.OID;



                _logger.Warn(String.Format("Shell Instance was unable to be created when Validating Related Unit Records. Class: {0}, OID: {1}.", tableName, oid));

                return errors;
            }
            // Skip idle features.
            if (StatusHelper.IsIdle(shellRecord.Status))
            {
                _logger.Info(string.Format("{0} OID: {1} has an Idle Status, Skipping Validating Related Unit Records.", shellRecord.ClassAlias, shellRecord.OID));
                return errors;
            }
            if ((shellRecord.IsTransformer) || (shellRecord.IsStepDown))
            {
                shellRecord.IsBankedVoltageRegulator = false;
                ValidateUnitRecords(ref errors, shellRecord);
                return errors;
            }
            else
            {
                // Voltage Regulator - determine if it's banked or single.
                var bankedVoltageRegulator = UnitHelper.CheckforBankedVoltageRegulator(ref shellRecord);
                if (bankedVoltageRegulator != null)
                {
                    // Banked Voltage Regulators are 2 or 3 Voltage Regulator Features that have 
                    //  the same CircuitID and OperatingNumber and are located on differnt poles
                    //  banked Voltage Regulators should use each VR features as the units for the unit logic.
                    ValidateUnitRecords(ref errors, bankedVoltageRegulator);
                    return errors;
                }
                else
                {
                    // Single Votage Regulator Features should use the unit records to validate.
                    ValidateUnitRecords(ref errors, shellRecord);
                    return errors;
                }
            }

        }



        /// <summary>
        /// Determines if the feature is part of a voltage regulator bank (same CircuitID and OperatingNumbers).
        /// </summary>
        /// <param name="shell"></param>
        /// <returns>BankedInstance</returns>
        public static BankedInstance CheckforBankedVoltageRegulator(ref ShellInstance shell)
        {
            shell.IsBankedVoltageRegulator = false;
            ICursor cursor = null;
            IRow pRow = null;
            IObjectClass _queryTable = null;
            List<ShellInstance> bankList = new List<ShellInstance>();
            BankedInstance bank = null;
            try
            {
                // Search for other voltage regulator records that have the same CircuitId & Operating Number.
                IQueryFilter qf = new QueryFilterClass();
                string whereClause = "CIRCUITID='" + shell.CircuitID + "'";
                whereClause += " AND OPERATINGNUMBER='" + shell.OperatingNumber + "'";
                whereClause += " AND OBJECTID <> " + shell.OID;

                qf.WhereClause = whereClause;
                IWorkspace wkspc = ((IDataset)shell.ObjectClass).Workspace;
                if (wkspc is IFeatureWorkspace)
                {
                    _queryTable = ModelNameFacade.ObjectClassByModelName(wkspc, SchemaInfo.Electric.ClassModelNames.VoltageRegulator);
                }

                cursor = ((ITable)_queryTable).Search(qf, false);
                pRow = cursor.NextRow();
                while (pRow != null)
                {

                    if (bankList.Count == 0)
                    {
                        shell.IsBankedVoltageRegulator = true;
                        bankList.Add(shell);
                    }
                    ShellInstance newShell = new ShellInstance(pRow as IObject)
                    {
                        IsBankedVoltageRegulator = true
                    };
                    bankList.Add(newShell);
                    pRow = cursor.NextRow();
                }
                if (bankList.Count > 0)
                {
                    bank = new BankedInstance(bankList);
                }
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { };
                }


            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            finally
            {
                // Release the cursor.
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { };
                }
            }

            return bank;

        }

        /// <summary>
        /// Simple checks if voltage regulator is banked.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsBankedVoltageRegulator(IFeature input)
        {
            bool result = false;
            int cnt = 0;
            ICursor cursor = null;
            IRow pRow = null;

            try
            {
                if (input != null)
                {
                    string circuitid = input.GetFieldValue("CIRCUITID").ToString();
                    string operatingnumber = input.GetFieldValue("OPERATINGNUMBER").ToString();
                    IQueryFilter qf = new QueryFilterClass();
                    string whereClause = "CIRCUITID = '" + circuitid + "'";
                    whereClause += " AND OPERATINGNUMBER = '" + operatingnumber + "'";

                    qf.WhereClause = whereClause;
                    cursor = input.Table.Search(qf, false);
                    pRow = cursor.NextRow();
                    while (pRow != null)
                    {
                        cnt++;
                        pRow = cursor.NextRow();
                    }
                }
                if (cnt > 1)
                    result = true;

                return result;
            }
            catch
            {
                return result;
            }
            finally
            {
                if (cursor != null)
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
            }
        }

        /// <summary>
        /// Creates TX or VR feature (shell) with the unit info and other metadata. 
        /// </summary>
        /// <param name="row">either a TX, or VR, or a unit record</param>
        /// <returns>Shell Instance</returns>
        public static ShellInstance CreateShellInstance(IRow row)
        {

            if (ModelNameFacade.ContainsClassModelName(row.Table as IObjectClass, SchemaInfo.Electric.ClassModelNames.PGEUnitTable))
            {
                // If this is a unit record then go get the parent feature.
                IObject obj = row as IObject;
                IEnumerable<IObject> relatedOrginObjects = obj.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, _shellModelNames);
                ShellInstance shell = null;
                if (relatedOrginObjects.FirstOrDefault() != null)
                {
                    shell = new ShellInstance(relatedOrginObjects.FirstOrDefault())
                    {
                        StartObject = ShellInstance.ShellorUnit.Unit
                    };
                }
                return shell;

            }
            else
            {
                // This is a parent record - hydrate the ShellInstance.
                ShellInstance shell = null;
                shell = new ShellInstance(row as IObject)
                {
                    StartObject = ShellInstance.ShellorUnit.Shell
                };
                return shell;
            }


        }

        /// <summary>
        /// Determines if a Transformer or VoltageRegulator has an Open Wye Config (2-Phase with 2 single-Phase Units).
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>true or false</returns>
        public static bool IsOpenWyeConfiguration(IFeature feature)
        {
            bool isOpeWye = false;
            if (ModelNameFacade.ContainsClassModelName(feature.Class, _shellModelNames))
            {
                ShellInstance shellRecord = CreateShellInstance(feature as IRow);
                var bankedVoltageRegulator = CheckforBankedVoltageRegulator(ref shellRecord);
                if (bankedVoltageRegulator != null)
                {
                    isOpeWye = bankedVoltageRegulator.HasOpenWyeUnitConfiguration;
                }
                else
                {
                    isOpeWye = shellRecord.HasOpenWyeUnitConfiguration;
                }

            }

            return isOpeWye;
        }

        /// <summary>
        /// Gets the Phase Designation as a dictionary.
        /// </summary>
        /// <param name="workspace"></param>
        public static void GetPhaseDesignationDomain(IWorkspace workspace)
        {
            PhaseDesignationDomain = GetDomainAsDictionary(_phaseDomainName, workspace);
        }
        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Validate Banked Voltage Regulator instance.
        /// </summary>
        /// <param name="bank"></param>
        private static void ValidateUnitRecords(ref List<string> errorList, BankedInstance bank)
        {
            // Error Messages.
            string unitRecordError = "UNIT ISSUE: " + "Banked Voltage Regulators unit record phasing must match parent record. (VoltageRegulator: {0}, Phase: {1}, Unit Record Phase: {2}).";
            string unitRecordCountError = "UNIT ISSUE: " + "Banked Voltage Regulators can only have 1 unit record per Voltage Regulator. (VoltageRegulator: {0}, Unit Record Count: {1}).";
            string duplicatePhaseError = "PHASE ISSUE: " + "Banked Voltage Regulator has Voltage Regulators with duplicated phase. (OIDs/Phase: {0}).";
            string nullPhaseError = "PHASE ISSUE: " + "Banked Voltage Regulator has Voltage Regulators with null Phase Designation. (OIDs: {0}).";
            string phaseLengthError = "PHASE ISSUE: " + "Banked Voltage Regulators cannot have Voltage Regulators with mixed number of phases: (OIDs/Phase: {0}).";

            foreach (var feature in bank.ShellRecords)
            {
                // Check the unit records of each feature in the VR Bank.
                if (feature.UnitRecords.Count() == 0)
                {
                    // No unit records.
                    errorList.Add(string.Format(unitRecordCountError, feature.OID, feature.UnitRecords.Count));
                }
                else if (feature.UnitRecords.Count() > 1)
                {
                    // More than one unit record.
                    errorList.Add(string.Format(unitRecordCountError, feature.OID, feature.UnitRecords.Count));
                }
                else if (feature.UnitRecords.Count() == 1)
                {
                    // Phase designation of unit must match shell feature.
                    if (feature.PhaseDesignation != feature.UnitRecords.First().PhaseDesignation)
                    {
                        errorList.Add(string.Format(unitRecordError, feature.OID, feature.PhaseDesignationDescription, feature.UnitRecords.FirstOrDefault().PhaseDesignationDescription));
                    }
                }

            }
            // Check the Voltage Regulators in the Bank.
            var unitPhaseList = bank.ShellRecords.Select(x => x.PhaseDesignationDescription).ToList();
            var anyDuplicate = bank.ShellRecords.GroupBy(x => x.PhaseDesignationDescription).Any(g => g.Count() > 1);
            // Can't have duplicate phasing.         
            if (anyDuplicate)
            {
                var duplicatePhases = String.Join(", ", bank.ShellRecords.Select(op => op.OID.ToString() + "/" + (string.IsNullOrEmpty(op.PhaseDesignationDescription) ? "null" : op.PhaseDesignationDescription)).ToArray());
                errorList.Add(String.Format(duplicatePhaseError, duplicatePhases));

            }
            // Can't have null phases.
            if (unitPhaseList.Any(x => x.IsNullOrEmpty()))
            {
                var nullPhase = bank.ShellRecords.Where(n => n.PhaseDesignationDescription.IsNullOrEmpty());
                var nullIds = String.Join(", ", nullPhase.Select(o => o.OID.ToString()).ToArray());
                errorList.Add(String.Format(nullPhaseError, nullIds));
            }
            // Can't have a mix of Single-Phase, Two-Phase & Three-Phase.
            var unitPhaseLengths = unitPhaseList.Select(l => string.IsNullOrEmpty(l) ? 0 : l.Length);
            if (unitPhaseLengths.Any(x => x != unitPhaseLengths.First()))
            {
                //if (unitPhaseList.Any(x => x.Length != unitPhaseList.First().Length))

                var mixedPhases = String.Join(", ", bank.ShellRecords.Select(op => op.OID.ToString() + "/" + (string.IsNullOrEmpty(op.PhaseDesignationDescription) ? "null" : op.PhaseDesignationDescription)).ToArray());
                errorList.Add(String.Format(phaseLengthError, mixedPhases));
            }

        }


        /// <summary>
        /// Validate Single Voltage Regulators & Transformers.
        /// </summary>
        /// <param name="feature"></param>
        private static void ValidateUnitRecords(ref List<string> errorList, ShellInstance feature)
        {
            // Error Messages.
            string duplicatePhaseError = "PHASE ISSUE: " + "{0} (OID: {1}) has unit records with duplicate phases. (OIDs/Phase: {2}).";
            string nullPhaseError = "PHASE ISSUE: " + "{0} (OID: {1}) has unit records with null Phase Designation.";
            string unitRecordCountError = "UNIT ISSUE: " + "{0} (OID: {1}) has no unit records.";
            //string featureNullPhaseError = "PHASE ISSUE: " + "{0} (OID: {1}) has a null Phase Designation.";
            string phaseLengthError = "PHASE ISSUE: " + "{0} (OID: {1}) cannot have unit records that have mixed number of phases: (OIDs/Phase: {2}).";
            string phaseMismatchError = "PHASE ISSUE: " + "{0} (OID: {1}) Phase Designation ({2}) must equal the combined phasing of its unit records: ({3}).";

            // Check unit info.
            var unitPhaseList = feature.UnitRecords.Select(x => x.PhaseDesignationDescription).ToList();
            var anyDuplicate = feature.UnitRecords.GroupBy(x => x.PhaseDesignationDescription).Any(g => g.Count() > 1);
            // Can't have ducplicate phases on unit records.
            if (anyDuplicate)
            {
                var duplicatePhases = String.Join(", ", feature.UnitRecords.Select(op => op.ObjectId.ToString() + "/" + (string.IsNullOrEmpty(op.PhaseDesignationDescription) ? "null" : op.PhaseDesignationDescription)).ToArray());
                errorList.Add(String.Format(duplicatePhaseError, feature.ClassAlias, feature.OID, duplicatePhases));

            }
            // Must have unit records.
            if (unitPhaseList.Count == 0)
            {
                errorList.Add(String.Format(unitRecordCountError, feature.ClassAlias, feature.OID, feature.ClassAlias + "Unit"));

            }
            // Can't have unit records wih null phases.
            else if (unitPhaseList.Any(x => x.IsNullOrEmpty()))
            {
                // Unit records with null phase.
                var nullPhase = feature.UnitRecords.Where(n => n.PhaseDesignationDescription.IsNullOrEmpty());
                var nullIds = String.Join(", ", nullPhase.Select(o => o.ObjectId.ToString()).ToArray());
                errorList.Add(String.Format(nullPhaseError, feature.ClassAlias, feature.OID));
            }
            // Can't have mixed phase lengths (e.g. Single-Phase, Two-Phase, Three-Phase).
            var unitPhaseLengths = unitPhaseList.Select(l => string.IsNullOrEmpty(l) ? 0 : l.Length);
            if (unitPhaseLengths.Any(x => x != unitPhaseLengths.First()))
            {
                var mixedPhases = String.Join(", ", feature.UnitRecords.Select(op => op.ObjectId.ToString() + "/" + (string.IsNullOrEmpty(op.PhaseDesignationDescription) ? "null" : op.PhaseDesignationDescription)).ToArray());
                errorList.Add(String.Format(phaseLengthError, feature.ClassAlias, feature.OID, mixedPhases));

            }
            // Can't have Phase mismatch between PhaseDesignation on shell feature & Combined unit records.
            if (feature.UnitPhaseDesignations != feature.PhaseDesignation)
            {
                string featurePhase = string.IsNullOrEmpty(feature.PhaseDesignationDescription) ? "null" : feature.PhaseDesignationDescription;
                string unitPhase = string.IsNullOrEmpty(feature.UnitPhaseDesignationDescriptions) ? "null" : feature.UnitPhaseDesignationDescriptions;
                // Phase mismatch.                
                errorList.Add(String.Format(phaseMismatchError, feature.ClassAlias, feature.OID, featurePhase, unitPhase));
            }
        }

        /// <summary>
        /// Given a DomainName and workspace gets the domain and covnerts it to Dictionary.
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="ws"></param>
        /// <returns></returns>
        private static Dictionary<string, string> GetDomainAsDictionary(string domainName, IWorkspace ws)
        {
            Dictionary<string, string> retValue = new Dictionary<string, string>();
            try
            {
                IWorkspaceDomains wsDomains = (IWorkspaceDomains)ws;
                IDomain domain = wsDomains.get_DomainByName(domainName);
                if (domain is ICodedValueDomain)
                {
                    retValue = GetDomainAsDictionary(domain as ICodedValueDomain);
                }
                return retValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        /// <summary>
        /// Given a Domain gets the Domain as Dictionary, with the domain code as Key and Domain description as value.
        /// </summary>
        /// <param name="codedDomain">An object of type ICodedDomain whose value has to be converted.</param>
        /// <returns>Returns a Dictionary of type string,string</returns>
        private static Dictionary<string, string> GetDomainAsDictionary(ICodedValueDomain codedDomain)
        {
            Dictionary<string, string> retValue = new Dictionary<string, string>();
            try
            {
                for (int i = 0; i < codedDomain.CodeCount; i++)
                {
                    retValue.Add(codedDomain.get_Value(i).ToString(), codedDomain.get_Name(i));
                }
                return retValue;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        #endregion Private Methods

    }

    #region Feature and Unit Classes

    /// <summary>
    /// Transformer, StepDown, or Voltage Regular Feature with Metadata needed for evalution.
    /// </summary>
    public class ShellInstance
    {
        #region Private Properties.
        private readonly IObject _obj;
        private bool _isBankedVoltageRegulator;
        private bool _isDuplex;
        private List<UnitRecordInstance> _unitRecords;
        private ShellorUnit _startobj = ShellorUnit.Shell;

        #endregion

        #region Public Properties.
        /// <summary>
        /// Flag to indicate origin row.
        /// </summary>
        public enum ShellorUnit
        {
            Shell,
            Unit
        }

        public ShellInstance(IObject obj)
        {
            _obj = obj;

        }

        public List<UnitRecordInstance> UnitRecords
        {

            get
            {
                return _unitRecords
                    ?? (_unitRecords = _obj.GetRelatedObjects(modelNames: SchemaInfo.Electric.ClassModelNames.PGEUnitTable)
                                            .Select(obj => new UnitRecordInstance(obj))
                                            .ToList());
            }
        }

        public bool IsTransformer
        {
            get { return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.PGETransformer); }
        }

        public bool IsStepDown
        {

            get { return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.StepDown); }
        }

        public bool IsVoltageRegulator
        {
            get { return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.VoltageRegulator); }
        }

        public bool IsCapacitorBank
        {
            get { return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.CapacitorBank); }
        }

        public bool IsBankedVoltageRegulator
        {
            get { return _isBankedVoltageRegulator; }

            set { _isBankedVoltageRegulator = value; }

        }

        public bool IsDuplex
        {
            get
            {
                _isDuplex = false;
                int[] unitTranTypes = { 21, 32 };

                if (IsTransformer)
                {
                    foreach (UnitRecordInstance u in this.UnitRecords)
                    {
                        object obj = u.GetFieldValue("TRANSFORMERTYPE");
                        if (obj != null)
                        {
                            int i = int.Parse(obj.ToString());
                            if (Array.IndexOf(unitTranTypes, i) >= 0)
                            {
                                _isDuplex = true;
                                break;
                            }
                        }
                    }

                }
                return _isDuplex;
            }
        }
        public int OID
        {
            get { return _obj.OID; }
        }


        public string OperatingNumber
        {
            get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.OperatingNumber).Convert<string>(); }
        }

        public string CircuitID
        {
            get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.CircuitID).Convert<string>(); }
        }
        public int PhaseDesignation
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<int>(); }
        }
        public string PhaseDesignationDescription
        {
            get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<string>(); }
        }

        public double RatedKVA
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.General.Fields.RatedKVA).Convert<double>(0.0); }
        }

        public double RatedAMP
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.RatedAMPS).Convert<double>(0.0); }
        }

        /// <summary>
        /// Just to let us know what the rule is running on to help with messaging.
        /// </summary>
        public ShellorUnit StartObject
        {
            get { return _startobj; }
            set { _startobj = value; }
        }
        public int Status
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.Status).Convert<int>(-1); }
        }
        public string HighSideConfiguration
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.HighSideConfiguration).Convert<string>(String.Empty); }
        }
        public string HighSideConfigurationDescription
        {
            get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.HighSideConfiguration).Convert<string>(String.Empty); }
        }

        public int UnitPhaseDesignations
        {
            get { return GetUnitPhaseDesignations(); }
        }
        public string UnitPhaseDesignationDescriptions
        {
            get { return GetUnitPhaseDesignationDescriptions(); }
        }
        public string ClassAlias
        {
            get { return _obj.Class.AliasName; }
        }
        public IClass ObjectClass
        {
            get { return _obj.Class; }
        }

        public IObject Object
        {
            get { return _obj; }
        }
        public int SubTypeCode
        {
            get
            {
                int i = -1;
                object obj = _obj.GetFieldValue("SUBTYPECD", false, null);
                if (obj != null)
                {
                    if (int.TryParse(obj.ToString(), out i))
                    {
                        return i;
                    }
                }
                return i;
            }
        }
        public bool HasOpenWyeUnitConfiguration
        {
            get { return IsOpenWyeConfiguration(); }
        }
        public bool Has3SinglePhaseUnits
        {
            get { return Contains3SinglePhaseUnits(); }
        }
        public bool Has3TwoPhaseUnits
        {
            get { return Contains3TwoPhaseUnits(); }
        }

        public bool Has1ThreePhaseUnit
        {
            get { return Contains1ThreePhaseUnit(); }
        }
        public override string ToString()
        {
            return "ShellRecordInstance\r\n"
                + _obj.GetFields()
                      .Select(field => (field.Alias + " = " + field.Value.Convert("<null>")).PadLeft(4))
                      .Concatenate("\r\n");
        }
        #endregion Public Properties

        #region Private Functions
        /// <summary>
        /// Gets the combined unit phase designation. 
        /// </summary>
        /// <returns>int - PhaseDesignation Code</returns>
        private int GetUnitPhaseDesignations()
        {
            int unitPhaseDesignation = 0;
            foreach (var unit in UnitRecords)
            {

                // Bitwise OR to combine all phases into one superset.
                unitPhaseDesignation = unitPhaseDesignation | unit.PhaseDesignation;
            }
            return unitPhaseDesignation;

        }

        /// <summary>
        /// Gets the combined unit phase designation. 
        /// </summary>
        /// <returns>string - PhaseDesignation Description</returns>
        private string GetUnitPhaseDesignationDescriptions()
        {

            if (UnitHelper.PhaseDesignationDomain == null)
            {
                IWorkspace wkspc = ((IDataset)ObjectClass).Workspace;
                UnitHelper.GetPhaseDesignationDomain(wkspc);
            }
            return UnitHelper.PhaseDesignationDomain.SingleOrDefault(x => x.Key == this.UnitPhaseDesignations.ToString()).Value;

        }

        /// <summary>
        /// Determines if Shell Feature has an OpenWye Unit Configuration (2phase with (2) Single-Phase Unit Records.
        /// </summary>
        /// <returns>true or false</returns>
        private bool IsOpenWyeConfiguration()
        {
            bool isOpenWye = false;
            if ((this.PhaseDesignationDescription != null) && (this.PhaseDesignationDescription.Length == 2) && (this.UnitRecords.Count == 2))
            {
                var singlePhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length == 1).ToList();
                var nonSinglePhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length != 1).ToList();
                if ((singlePhaseUnits.Count == 2) && (nonSinglePhaseUnits.Count == 0))
                {
                    isOpenWye = true;
                }

            }
            return isOpenWye;

        }
        /// <summary>
        /// Determines if Shell Feature has (3) Single-Phase Unit Records.
        /// </summary>
        /// <returns>true or false</returns>
        private bool Contains3SinglePhaseUnits()
        {
            bool has3SinglePhaseUnits = false;

            var singlePhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length == 1).ToList();
            var nonSinglePhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length != 1).ToList();
            if ((singlePhaseUnits.Count == 3) && (nonSinglePhaseUnits.Count == 0))
            {
                has3SinglePhaseUnits = true;
            }


            return has3SinglePhaseUnits;

        }

        /// <summary>
        /// Determines if Shell Feature has (3) Two-Phase Unit Records.
        /// </summary>
        /// <returns>true or false</returns>
        private bool Contains3TwoPhaseUnits()
        {
            bool has3TwoPhaseUnits = false;
            var twoPhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length == 2).ToList();
            var nonTwoPhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length != 2).ToList();
            if ((twoPhaseUnits.Count == 3) && (nonTwoPhaseUnits.Count == 0))
            {
                has3TwoPhaseUnits = true;
            }

            return has3TwoPhaseUnits;

        }

        /// <summary>
        /// Determines if Shell Feature has (1) Three-Phase Unit Record.
        /// </summary>
        /// <returns>true or false</returns>
        private bool Contains1ThreePhaseUnit()
        {
            bool has1ThreePhaseUnits = false;

            var threePhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length == 3).ToList();
            var nonThreePhaseUnits = this.UnitRecords.Where(unit => unit.PhaseDesignationDescription.Length != 3).ToList();
            if ((threePhaseUnits.Count == 11) && (nonThreePhaseUnits.Count == 0))
            {
                has1ThreePhaseUnits = true;
            }
            return has1ThreePhaseUnits;

        }

        #endregion
    }

    /// <summary>
    /// Unit Record Object with meta data for evaluation.
    /// </summary>
    public class UnitRecordInstance
    {
        #region Private Properties
        private readonly IObject _obj;
        #endregion

        #region Public Properties
        public UnitRecordInstance(IObject record)
        {
            _obj = record;

        }

        public int ObjectId
        {
            get { return _obj.OID; }
        }

        public string ClassName
        {

            get { return _obj.Class.AliasName; }
        }

        public int PhaseDesignation
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<int>(); }
        }
        public string PhaseDesignationDescription
        {
            get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert<string>(); }
        }

        public double RatedKVA
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.General.Fields.RatedKVA).Convert<double>(0.0); }
        }

        public double RatedAMP
        {
            get { return _obj.GetFieldValue(useDomain: false, modelName: SchemaInfo.Electric.FieldModelNames.RatedAMPS).Convert<double>(0.0); }
        }

        public override string ToString()
        {
            return "UnitRecordInstance\r\n"
                + _obj.GetFields()
                      .Select(field => (field.Alias + " = " + field.Value.Convert("<null>")).PadLeft(4))
                      .Concatenate("\r\n");
        }

        public object GetFieldValue(string fieldName)
        {
            object result = null;
            result = _obj.GetFieldValue(fieldName, false);
            return result;
        }
        #endregion
    }

    /// <summary>
    /// Banked Voltage Regular Object (multiple Voltage Regulars with same CircuitID and Operating Number, with Metadata needed for evalution).
    /// </summary>
    public class BankedInstance
    {
        #region Public Properties
        public readonly List<ShellInstance> ShellRecords;

        public BankedInstance(List<ShellInstance> records)
        {
            ShellRecords = records;
        }

        public int ShellRecordCount
        {
            get { return ShellRecords.Count; }
        }
        public int BankPhaseDesignation
        {
            get { return GetBankPhaseDesignation(); }
        }
        public string BankPhaseDesignationDescription
        {
            get { return GetBankPhaseDesignationDescription(); }
        }

        /// <summary>
        /// Banked Voltage Regulators CircuitID.
        /// </summary>
        public string CircuitID
        {
            get { return (ShellRecords[0] != null) ? ShellRecords[0].CircuitID : ""; }
        }
        /// <summary>
        /// Banked Voltage Regulators OperatingNumber.
        /// </summary>
        public string OperatingNumber
        {
            get { return (ShellRecords[0] != null) ? ShellRecords[0].OperatingNumber : ""; }
        }

        /// <summary>
        /// Has 2 single phase units which is an OpenWye Unit Configuration.
        /// </summary>
        public bool HasOpenWyeUnitConfiguration
        {
            get { return IsOpenWyeConfiguration(); }
        }

        /// <summary>
        /// Banked Features has (3) Single-Phase features.
        /// </summary>
        public bool Has3SinglePhaseUnits
        {
            get { return Contains3SinglePhaseUnits(); }
        }
        /// <summary>
        /// Banked Features has (3) Two-Phase features.
        /// </summary>
        public bool Has3TwoPhaseUnits
        {
            get { return Contains3TwoPhaseUnits(); }
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Gets Combined Phase Designation of all features in Bank.
        /// </summary>
        /// <returns>int Phase Designation Code</returns>
        private int GetBankPhaseDesignation()
        {
            int phaseDesignation = 0;
            foreach (var shell in ShellRecords)
            {

                // Bitwise OR to combine all phases into one superset.
                phaseDesignation = phaseDesignation | shell.PhaseDesignation;
            }
            return phaseDesignation;

        }
        /// <summary>
        /// Gets Combined Phase Designation of all features in Bank.
        /// </summary>
        /// <returns>string Phase Designation Description </returns>
        private string GetBankPhaseDesignationDescription()
        {

            if (UnitHelper.PhaseDesignationDomain == null)
            {
                IWorkspace wkspc = ((IDataset)ShellRecords[0].ObjectClass).Workspace;
                UnitHelper.GetPhaseDesignationDomain(wkspc);
            }
            return UnitHelper.PhaseDesignationDomain.SingleOrDefault(x => x.Key == this.BankPhaseDesignation.ToString()).Value;

        }

        /// <summary>
        /// Determines if Banked Feature has an OpenWye Unit Configuration (2phase with (2) Single-Phase features).
        /// </summary>
        /// <returns></returns>
        private bool IsOpenWyeConfiguration()
        {
            bool isOpenWye = false;
            if ((this.BankPhaseDesignationDescription != null) && (this.BankPhaseDesignationDescription.Length == 2) && (this.ShellRecordCount == 2))
            {
                var singlePhaseUnits = this.ShellRecords.Where(sh => sh.PhaseDesignationDescription.Length == 1).ToList();
                var nonSinglePhaseUnits = this.ShellRecords.Where(sh => sh.PhaseDesignationDescription.Length != 1).ToList();
                if ((singlePhaseUnits.Count == 2) && (nonSinglePhaseUnits.Count == 0))
                {
                    isOpenWye = true;
                }

            }
            return isOpenWye;

        }

        /// <summary>
        /// Determines if Banked Features has (3) Single-Phase features.
        /// </summary>
        /// <returns>true or false</returns>
        private bool Contains3SinglePhaseUnits()
        {
            bool has3SinglePhaseUnits = false;

            var singlePhaseUnits = this.ShellRecords.Where(unit => unit.PhaseDesignationDescription.Length == 1).ToList();
            var nonSinglePhaseUnits = this.ShellRecords.Where(unit => unit.PhaseDesignationDescription.Length != 1).ToList();
            if ((singlePhaseUnits.Count == 3) && (nonSinglePhaseUnits.Count == 0))
            {
                has3SinglePhaseUnits = true;
            }


            return has3SinglePhaseUnits;

        }

        /// <summary>
        /// Determines if Banked Features has (3) Two-Phase feature.
        /// </summary>
        /// <returns>true or false</returns>
        private bool Contains3TwoPhaseUnits()
        {
            bool has3TwoPhaseUnits = false;

            var twoPhaseUnits = this.ShellRecords.Where(unit => unit.PhaseDesignationDescription.Length == 2).ToList();
            var nonTwoPhaseUnits = this.ShellRecords.Where(unit => unit.PhaseDesignationDescription.Length != 2).ToList();
            if ((twoPhaseUnits.Count == 3) && (nonTwoPhaseUnits.Count == 0))
            {
                has3TwoPhaseUnits = true;
            }


            return has3TwoPhaseUnits;

        }
        #endregion Private Functions

    }
    #endregion Feature and Unit Classes
}