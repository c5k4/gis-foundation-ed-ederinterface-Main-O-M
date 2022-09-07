/*
 *PreLoad info
* insert into sde.telvent_validation_severitymap values (sde.gdb_util.next_rowid('SDE', 'telvent_validation_severitymap'),'PGE Validate Duplex Transformers',1);
*commit;
*
*Check box in ArcFM Properties - Object Info - Rules for Transformer ...
 */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;

using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;

using PGE.Desktop.EDER.AutoUpdaters;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Validates Duplex Transformers.
    /// </summary>
    [ComVisible(true)]
    [Guid("BD7A9D5B-5A55-4562-BA51-B68532B1F650")]
    [ProgId("PGE.Desktop.EDER.ValidateTransformerDuplex")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateTransformerDuplex : BaseValidationRule
    {

        #region Private
        /// <summary>
        /// Constant error message.
        /// 
        /// </summary>

        private static readonly string[] _modelNames = {     SchemaInfo.Electric.ClassModelNames.Transformer, 
                                                             SchemaInfo.Electric.ClassModelNames.DeviceGroup, 
                                                             SchemaInfo.Electric.ClassModelNames.TransformerUnit };

        private static readonly string _msgDuplexWrongDeviceType = "Duplex transformer unit record must be related to Duplex transformer DeviceGroupType; DeviceGroup OID: {0} ";
     //   private static readonly string _msgDuplexWrongTransformerType = "Duplex Unit Record {0} must have TransformerType of {1}";
        private static readonly string _msgDuplexWrongDeviceSubtype = "Duplex Related DeviceGroup OID: {0} must have Subtype of {1}";
        private static readonly string _msgDuplexWrongTranSubType = "Duplex DeviceGroupType must have Duplex TransformerType of {0} on related unit Record {1}.";
        // If duplex, then unit record must have RatedKVA IN (35,60,90,125,150) (i.e. sum of two windings for allowable combinations)
        private static readonly double[] _duplexRatedKVASub = { 35, 60, 90, 125, 150 };
       // private static readonly object[] _duplexRatedKVAPad = { 35, 60,  125, 150 };
        private static readonly string _msgDuplexRatedKVA = "Duplex Unit Record OID: {0} must have a RatedKVA one of {1}";
        // Duplex transformer unit record must be related to Duplex transformer DeviceGroupType; DeviceGroup OID: {0} 
        // If duplex, then must have one unit record
        private static readonly string _msgDuplexOneUnitRec = "Duplex has {0} Unit records, Only one is allowed.";
        private static readonly string _msgDuplexOH = "Duplex can not be Subtype of Overhead.";
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        /// 
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");


        #endregion Private

        #region Constructor
        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateTransformerDuplex()
            : base("PGE Validate Duplex Transformers", _modelNames)
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
            try
            {
                DuplexShellInstance shellRecord = new DuplexShellInstance(row as IObject);
                string tableName;
                if (shellRecord == null)
                {
                    tableName = (row as IObject).Class.AliasName;
                    int oid = row.OID;

                    _logger.Warn(String.Format("Shell Instance was unable to be created when when Validating Unit KVA-Amps. Class: {0}, OID: {1}.", tableName, oid));

                    return _ErrorList;
                }

                // Skip idle features.
                if (StatusHelper.IsIdle(shellRecord.Status))
                {
                    _logger.Info(string.Format("{0} OID: {1} has an Idle Status, Skipping Validating Unit KVA-Amps.", shellRecord.ClassAlias, shellRecord.OID));
                    return _ErrorList;
                }

                if (shellRecord.IsTransformer)
                {
                    ValidateDuplex(shellRecord);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred while validating Unit Kva-Amps rule.", ex);
            }

            return _ErrorList;
        }
        #endregion

        /// <summary>
        /// Validate duplex transformers.
        /// </summary>
        /// <param name="shellRecord"></param>
        /// <returns></returns>
        private bool ValidateDuplex(DuplexShellInstance shellRecord)
        {
            bool result = false;

            // If unit record TransformerType IN (21,32) then DeviceGroup must be Subtype=2, DeviceGroupType IN (3,6) OR Subtype=3, DeviceGroupType=18. And viceversa.
            result = CheckUnitRecords(shellRecord);

            return result;
        }


        /// <summary>
        /// Check transformer unit records.
        /// </summary>
        /// <param name="shellRecord"></param>
        /// <returns></returns>
        private bool CheckUnitRecords(DuplexShellInstance shellRecord)
        {
            bool result = true;
            string flag = string.Empty;
            int countUnits = 0;

            foreach (UnitRecordInstance u in shellRecord.UnitRecords)
            {
                flag = string.Empty;
                int[] deviceType = { 3, 6 };

                object o = u.GetFieldValue("TRANSFORMERTYPE");
                if (o != null)
                {
                    int i = int.Parse(o.ToString());
                    if (i == 21 || i == 32)
                    {
                        countUnits++;
                        if (!deviceType.Contains(shellRecord.DeviceGroupType) && shellRecord.DeviceGroupSubtype == 2)
                        {
                            AddError(string.Format(_msgDuplexWrongDeviceType, shellRecord.DeviceGroupOID));
                            result = false;
                        }

                        if (shellRecord.DeviceGroupSubtype != 2 && deviceType.Contains(shellRecord.DeviceGroupType) && result)
                        {
                            AddError(string.Format(_msgDuplexWrongDeviceType, shellRecord.DeviceGroupOID, 2));
                            result = false;
                        }

                        if ((shellRecord.DeviceGroupType != 18) && shellRecord.DeviceGroupSubtype == 3 && result)
                        {
                            AddError(string.Format(_msgDuplexWrongDeviceType, shellRecord.DeviceGroupOID));
                            result = false;
                        }

                        if (shellRecord.DeviceGroupSubtype != 3 && (shellRecord.DeviceGroupType == 18) && result)
                        {
                            AddError(string.Format(_msgDuplexWrongDeviceType, shellRecord.DeviceGroupOID, 3));
                            result = false;
                        }

                        // RATEDKVA Check.
                        // If result is still true this is a valid setup, lets check kva on the unit.
                        if (result)
                        {
                            if ((!_duplexRatedKVASub.Contains(u.RatedKVA)))
                            {
                                AddError(string.Format(_msgDuplexRatedKVA,  u.ObjectId, "35, 60, 90, 125, 150"));
                                result = false;
                            }
                           
                        }
                    }
                    else if (deviceType.Contains(shellRecord.DeviceGroupType) && shellRecord.DeviceGroupSubtype == 2)
                    {
                        countUnits++;
                        result = false;
                        string tmp = "21";
                        // Switch if Padmount.
                        if (shellRecord.SubTypeCode == 3)
                            tmp = "32";

                        AddError(string.Format(_msgDuplexWrongTranSubType, tmp, u.ObjectId));
                    }
                    else if (shellRecord.DeviceGroupSubtype == 3 && (shellRecord.DeviceGroupType == 18))
                    {
                        countUnits++;
                        result = false;
                        string tmp = "21";
                        // Switch if Padmount.
                        if (shellRecord.SubTypeCode == 3)
                            tmp = "32";
                        AddError(string.Format(_msgDuplexWrongTranSubType, tmp, u.ObjectId));
                    }
                }

            }

            // Only want to add the message once per unit record.
            if (!result)
            {
                ////  _msgDuplexWrongTranSubType = "Duplex DeviceGroupType must have Duplex TransformerType of {0} on related unit Record {1}.";
                //AddError(string.Format(_msgDuplexWrongTranSubType, shellRecord.));
            }
            // Duplex is only allowed one unit record.
            if (countUnits > 1)
            {
                AddError(string.Format(_msgDuplexOneUnitRec, countUnits));
                result = false;
            }

            // Duplex can not be Overhead subtype.
            if (countUnits >= 1 && shellRecord.SubTypeCode == 1)
            {
                AddError(string.Format(_msgDuplexOH, countUnits));
                result = false;
            }

            return result;
        }      

    }

    /// <summary>
    /// Duplex object containter.
    /// </summary>
    public class DuplexShellInstance : ShellInstance
    {
        private static readonly object[] _duplexTranTypes = { 21, 32 };
        private static readonly string _transformerTypeFieldName = "TRANSFORMERTYPE";
        private int _getDeviceGroupType = -1;
        private int _deviceGroupSubtype = -1;
        private int _deviceGroupOID = -1;

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="obj"></param>
        public DuplexShellInstance(IObject obj) : base(obj)
        {
            this.Feature = obj as IFeature;
        }

        public IFeature Feature { get; set; }

        public int DeviceGroupType
        {
            get
            {
                if (_getDeviceGroupType == -1)
                    _getDeviceGroupType = GetDeviceGroupInfo(this.Feature);
                return _getDeviceGroupType;
            }
        }

        public int DeviceGroupSubtype
        {
            get
            {
                if (_deviceGroupSubtype == -1)
                    GetDeviceGroupInfo(this.Feature);

                return _deviceGroupSubtype;
            }
        }

        public int DeviceGroupOID
        {
            get
            {
                if (_deviceGroupOID <= 0)
                    GetDeviceGroupInfo(this.Feature);

                return _deviceGroupOID;
            }
        }


        /// <summary>
        /// Pulls field info from DeviceGroup.
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private int GetDeviceGroupInfo(IFeature feature)
        {
            int result = -1;
            IEnumerable<IObject> relatedObjects = feature.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
            foreach (IObject obj in relatedObjects)
            {

                int relatedValue = obj.GetFieldValue("DeviceGroupType", false,null).Convert(-1);
                if (relatedValue > 0)
                {
                    result = relatedValue;
                    _getDeviceGroupType = result;
                    _deviceGroupSubtype = obj.GetFieldValue("SubTypeCD", false, null).Convert(-1);
                    _deviceGroupOID = obj.OID;
                    break;
                }
            }

            if (result == -1)
            {
                // None found, set value so we don't repeat this function for this feature.
                _getDeviceGroupType = -99;
                _deviceGroupSubtype = -99;
            }
            return result;
        }
    }

}
