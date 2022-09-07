using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using Miner.Interop;
using Miner.Geodatabase;
using Miner.ComCategories;
using log4net;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("61be9d63-6a4b-4482-a6f9-24d9a1248354")]
    [ProgId("PGE.Desktop.EDER.ValidationRules.ScadaValidator")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ScadaValidator : BaseValidationRule
    {
        private static readonly Log4NetLogger Log = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static readonly string[]
            AssignmentModelNames = new[]
                                   {
                                          SchemaInfo.Electric.ClassModelNames.Scada.ScadaOperateableDevice,
                                          //SchemaInfo.Electric.ClassModelNames.Scada.ScadaInfo
                                   };

        private const string PlaceholderNull = "<null>";
        private const string HintRadioCommunication = "radio";

        public ScadaValidator()
            : base("PGE Validate SCADA / Controller Device", AssignmentModelNames)
        {

        }


        bool ViolatesScadaConditions(ObjectInstance instance)
        {
            if (instance.IsScadaMateDevice)
            {
                if (instance.IsSwitch)
                {
                    if (!instance.IsScadaSwitch) return true;
                    if (!instance.SupervisoryControlIsYes) return true;
                    if (!instance.HasScadaRecords) return true;
                    if (instance.ScadaRecords.Any(o => o.ScadaType == null)) return true;
                    return false;
                }
                if (instance.IsSectionalizer)
                {
                    if (!instance.SupervisoryControlIsYes) return true;
                    if (!instance.HasScadaRecords) return true;
                    if (instance.ScadaRecords.Any(o => o.ScadaType == null)) return true;
                    return false;
                }
            }

            if (instance.IsScadaSwitch)
            {
                if (!instance.SupervisoryControlIsYes) return true;
                if (!instance.HasScadaRecords) return true;
                if (instance.ScadaRecords.Any(o => o.ScadaType == null)) return true;
                return false;
            }

            if (instance.SupervisoryControlIsYes && (instance.IsSectionalizer || instance.IsCapacitorBank || instance.IsVoltageRegulator))
            {
                if (!instance.HasScadaRecords) return true;
                if (instance.ScadaRecords.Any(o => o.ScadaType == null)) return true;
                return false;
            }

            if (instance.HasScadaRecords)
            {
                if (!instance.SupervisoryControlIsYes) return true;
                return false;
            }

            return false;
        }

        /// <summary>
        /// Checks whether or not the object should have or should not 
        /// have a controller record 
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        List<string> ViolatesControllerConditions(ObjectInstance instance)
        {
            //Changes for INC3845223 details are in Remedy 

            List<string> validationErrors = new List<string>();

            //if (!instance.HasControllerRecord) return false;

            //if (instance.IsSwitch && (instance.IsScadaSwitch || instance.IsPdacSwitch))
            if (instance.IsSwitch && (instance.SupervisoryControlIsYes))
            {
                if (!instance.HasControllerRecord)
                {
                    validationErrors.Add("All Switches with Supervisory Control MUST have a related Controller record");
                }
            }

            if (instance.IsDynamicProtectiveDevice)
            {
                //if (instance.IsSectionalizer)
                //{
                //    if (!instance.IsScadaMateDevice)
                //    {
                //        return true;
                //    }
                //}
                if (!instance.HasControllerRecord)
                {
                    validationErrors.Add("All DynamicProtectiveDevice features MUST have a related Controller record");
                }
            }

            //if (instance.IsCapacitorBank && instance.SubtypeIsFixed)
            if (instance.IsCapacitorBank)
            {
                if ((instance.SubtypeIsFixed) && (instance.HasControllerRecord))
                {
                    validationErrors.Add("Fixed Bank Capacitor features MUST NOT have a related Controller record");
                }
                else if ((instance.SubtypeIsSwitched) && (!instance.HasControllerRecord))
                {
                    validationErrors.Add("Switched Bank Capacitor features MUST have a related Controller record");
                }
            }

            if (instance.IsVoltageRegulator)
            {
                if ((instance.SubtypeIsOverhead) || (instance.SubtypeIsPadMounted))
                {
                    if (!instance.HasControllerRecord)
                        validationErrors.Add("Overhead Regulator and Pad Mounted Regulator features MUST have a related Controller record");                
                }
                else if ((instance.SubtypeIsBooster) && (instance.IsAutoBooster)) 
                {
                    if (!instance.HasControllerRecord)
                        validationErrors.Add("Booster Regulator features with AutoBooster set to Yes MUST have a related Controller record");
                }
                else if ((instance.SubtypeIsBooster) && (!instance.IsAutoBooster))
                {
                    if (instance.HasControllerRecord)
                        validationErrors.Add("Booster Regulator features with AutoBooster set to No MUST NOT have a related Controller record");
                }
            }

            return validationErrors;
        }

        /// <summary>
        /// Scans the row for QA/QC violations and returns a list of errors.
        /// </summary>
        /// <param name="row">The row to scan.</param>
        /// <returns>A list of QA/QC errors.</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            //Change to address INC000003803926 QA/QC freezing on the QAQC subtask 
            //if (!ValidationFilterEngine.Instance.IsQAQCRuleEnabled(_Name, base.Severity))
            //    return _ErrorList; 

            var obj = row as IObject;
            if (obj == null) return _ErrorList;

            try
            {
                if (obj.HasModelName(SchemaInfo.Electric.ClassModelNames.Scada.ScadaOperateableDevice))
                {
                    var objectInstance = new ObjectInstance(obj);

                    if (ViolatesScadaConditions(objectInstance))
                        AddError("Please verify SCADA attributes and/or SCADA related records.");

                    List<string> valMessages = ViolatesControllerConditions(objectInstance);
                    for (int i = 0; i < valMessages.Count; i++)
                    {
                        AddError(valMessages[i]);
                    }

                    foreach (var record in objectInstance.ScadaRecords)
                    {
                        if (record.CommunicationType == null)
                            AddError("SCADA " + record.ObjectId + ": Communication Type cannot be null.");

                        if (record.CommunicationType != null)
                        {
                            if (record.CommunicationType.ToUpper() == HintRadioCommunication.ToUpper())
                            {
                                if (record.RadioManufacturer == null)
                                    AddError("SCADA " + record.ObjectId + ": Radio Manufacturer cannot be null.");
                            }
                        }
                    }

                    return _ErrorList;
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex.ToString());
            }


            return _ErrorList;
        }

        #region Nested Classes
        class ObjectInstance
        {
            private const string PlaceholderNoOid = "<no object ID>";

            private const string FieldNameSwitchType = "SwitchType";
            private const string FieldNameDeviceType = "DEVICETYPE";

            private const string HintScadaSwitch = "scada";
            private const string HintPdacSwitch = "pdac";
            private const string HintScadaMate = "scadamate";
            private const string HintYes = "y";
            private const string HintSubtypeFixed = "fixed";
            private const string HintSubtypeSwitched = "switched";
            private const string HintSubtypeBooster = "booster";
            private const string HintSubtypePadMounted = "pad";
            private const string HintSubtypeOverhead = "overhead";


            private readonly IObject _obj;

            public ObjectInstance(IObject obj)
            {
                _obj = obj;
            }

            private bool? _hasScadaRecord;
            public bool HasScadaRecords
            {
                get { return _hasScadaRecord ?? (_hasScadaRecord = ScadaRecords.Any()).Value; }
            }

            IEnumerable<ScadaRecordInstance> _scadaRecords;
            public IEnumerable<ScadaRecordInstance> ScadaRecords
            {
                get
                {
                    return _scadaRecords
                        ?? (_scadaRecords = _obj.GetRelatedObjects(modelNames: SchemaInfo.Electric.ClassModelNames.Scada.ScadaInfo)
                                                .Select(obj => new ScadaRecordInstance(obj))
                                                .ToList()
                                                .AsReadOnly());
                }
            }

            public bool SupervisoryControlIsYes
            {
                get
                {
                    return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.ScadaSupervisoryControl)
                                .Convert(PlaceholderNull)
                                .Contains(HintYes, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool IsSwitch
            {
                get { return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.Switch); }
            }

            public string SwitchType
            {
                get
                {
                    return _obj.GetFieldValue(FieldNameSwitchType)
                               .Convert(PlaceholderNull);
                }
            }

            public bool IsScadaSwitch
            {
                get
                {
                    return SwitchType.Contains(HintScadaSwitch, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool IsPdacSwitch
            {
                get
                {
                    return SwitchType.Contains(HintPdacSwitch, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool IsSectionalizer
            {
                get
                {
                    return IsDynamicProtectiveDevice
                        && _obj.HasSubtypeCode(SchemaInfo.Electric.Subtypes.DynamicProtectiveDevice.Sectionalizer);
                }
            }

            public bool IsScadaMateDevice
            {
                get
                {
                    return (IsSwitch && _obj.HasSubtypeCode(SchemaInfo.Electric.Subtypes.Switch.ScadaMateSwitch))
                        || (IsSectionalizer && _obj.GetFieldValue(FieldNameDeviceType)
                                                   .Convert(PlaceholderNull)
                                                   .Contains(HintScadaMate, StringComparison.InvariantCultureIgnoreCase));
                }
            }

            public bool IsCapacitorBank
            {
                get { return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.CapacitorBank); }
            }

            public bool IsVoltageRegulator
            {
                get { return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.VoltageRegulator); }
            }


            private IEnumerable<ControllerRecordInstance> _controllerRelatedRecords;
            public IEnumerable<ControllerRecordInstance> ControllerRelatedRecords
            {
                get
                {
                    return _controllerRelatedRecords
                        ?? (_controllerRelatedRecords = _obj.GetRelatedObjects(null, modelNames: SchemaInfo.Electric.ClassModelNames.Scada.ScadaController)
                                                            .Select(record => new ControllerRecordInstance(record))
                                                            .ToList()
                                                            .AsReadOnly());
                }
            }

            public bool HasControllerRecord
            {      
                get
                {
                    if (!IsVoltageRegulator)
                        return ControllerRelatedRecords.Any();
                    else
                    {
                        //Look at the related voltageregulatorunits and check if they have a controller 
                        bool hasController = false; 
                        var relSet = _obj.GetRelatedObjects(null, modelNames: SchemaInfo.Electric.ClassModelNames.PGEUnitTable)
                        .Cast<IObject>();
                        if (relSet.Any()) 
                        {
                            foreach (var voltRegUnit in relSet)
                            {
                                var controllerSet = voltRegUnit.GetRelatedObjects(null, modelNames: SchemaInfo.Electric.ClassModelNames.Scada.ScadaController);
                                if (controllerSet.Any())
                                {
                                    hasController = true;
                                    break; 
                                }                      
                            }
                        }
                        return hasController; 
                    }
                }
            }

            public bool IsDynamicProtectiveDevice
            {
                get
                {
                    return _obj.HasModelName(SchemaInfo.Electric.ClassModelNames.DynamicProtectiveDevice);
                }
            }

            public bool SubtypeIsFixed
            {
                get
                {
                    return _obj.GetSubtypeDescription()
                               .Contains(HintSubtypeFixed, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool SubtypeIsSwitched
            {
                get
                {
                    return _obj.GetSubtypeDescription()
                               .Contains(HintSubtypeSwitched, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool SubtypeIsBooster
            {
                get
                {
                    return _obj.GetSubtypeDescription()
                               .Contains(HintSubtypeBooster, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool SubtypeIsPadMounted
            {
                get
                {
                    return _obj.GetSubtypeDescription()
                               .Contains(HintSubtypePadMounted, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool SubtypeIsOverhead
            {
                get
                {
                    return _obj.GetSubtypeDescription()
                               .Contains(HintSubtypeOverhead, StringComparison.InvariantCultureIgnoreCase);
                }
            }

            public bool IsAutoBooster
            {
                get
                {
                    return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.AutoBooster)
                                .Convert(PlaceholderNull)
                                .Contains(HintYes, StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }

        class ScadaRecordInstance
        {
            private readonly IObject _obj;

            public ScadaRecordInstance(IObject record)
            {
                _obj = record;
            }

            public int ObjectId
            {
                get { return _obj.OID; }
            }

            public string CommunicationType
            {
                get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.ScadaCommunication).Convert<string>(); }
            }

            public string RadioManufacturer
            {
                get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.ScadaRadioManufacturer).Convert<string>(); }
            }

            public string ScadaType
            {
                get { return _obj.GetFieldValue(modelName: SchemaInfo.Electric.FieldModelNames.ScadaType).Convert<string>(); }
            }

            public override string ToString()
            {
                return "ScadaRecordInstance\r\n"
                    + _obj.GetFields()
                          .Select(field => (field.Alias + " = " + field.Value.Convert("<null>")).PadLeft(4))
                          .Concatenate("\r\n");
            }
        }

        class ControllerRecordInstance
        {
            private readonly IObject _obj;
            public ControllerRecordInstance(IObject record)
            {
                _obj = record;
            }
        }

        #endregion
    }
}
