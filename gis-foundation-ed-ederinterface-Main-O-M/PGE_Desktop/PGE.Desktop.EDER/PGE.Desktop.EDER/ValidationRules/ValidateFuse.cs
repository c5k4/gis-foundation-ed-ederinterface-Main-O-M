/*
 * PreLoad info
 * insert into sde.telvent_validation_severitymap values (sde.gdb_util.next_rowid('SDE', 'telvent_validation_severitymap'),'PGE Validate Fuse',1);
 * commit;
 * 
 * Check box in ArcFM Properties - Object Info - Rules ...
 */

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
    /// Validates Fuse values.
    /// </summary>
    [ComVisible(true)]
    [Guid("A4F99DDE-8980-46E7-98E0-5DB2DCCBA040")]
    [ProgId("PGE.Desktop.EDER.ValidateFuse")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateFuse : BaseValidationRule
    {
        #region Private Variables
        private static readonly string[] _enabledModelNames = {SchemaInfo.Electric.ClassModelNames.Fuse};

        private static readonly string _msgValidLinkType = "SubSurface Fuse must be LinkType CL.";
        private static readonly string _msgValidateFusePerPhase = "SubSurface Fuse {0} can only have {1} NumberFusesPerPhase.";
        private static readonly string _msgValidateOpVoltLinkRating = "SubSurface Fuse Link Rating should be {0} when OperatingVolatage is {1}.";

        // Voltages below 15.5kV.
        private static readonly List<int> _opVoltBelow = new List<int>() {0,1,2,3,4,5,10,11,12,13,35 };
        // Voltages above 15.5kV.
        private static readonly List<int> _opVoltAbove = new List<int>() { 6, 8, 9, 14, 65, 85 };
        // Valid link ratings for below 15.5 kV
        private static readonly List<int> _linkRatingBelow = new List<int>() { 12, 25, 30, 40, 50 };
        // Valid link ratings for above 15.5kV
        private static readonly List<int> _linkRatingAbove = new List<int>() { 20, 25, 30, 40, 50 };
        /// <summary>
        /// Logger to log all the information, warning and errors.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion Private Variables

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValidateFuse()
            : base("PGE Validate Fuse", _enabledModelNames)
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
            _logger.Debug("Validate Fuse Started.");
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
                    FuseObject fuseObject = new FuseObject(obj);
                    RunSubSurfaceRules(fuseObject);
                }

                _logger.Debug("ValidateFuse Ended.");
                return _ErrorList;
            }
            catch (System.Exception ex)
            {
                _logger.Error("Error occurred while Validating Fuse rule", ex);
                return _ErrorList;
            }
        }

        #endregion Overrides for validation rule

        #region SubSurface Rules

        /// <summary>
        /// Run rules for SubSurface Fuses only.
        /// </summary>
        /// <param name="fuseObject"></param>
        private void RunSubSurfaceRules(FuseObject fuseObject)
        {
            // Just want SubSurface types (subtype = 2).
            if (fuseObject.IsSubSurface)
            {
                ValidLinkType(fuseObject);
                ValidateFusePerPhase(fuseObject);
                ValidateOpVoltLinkRating(fuseObject);
            }

        }

        /// <summary>
        /// LinkType must be CL for any SS Fuse (Subtype=2,DeviceGroupType(7,8).
        /// </summary>
        /// <returns></returns>
        private bool ValidLinkType(FuseObject fuseObject)
        {
            bool result = false;

            // 5 = CL in domain.
            if (fuseObject.LinkType != 5)
            {
                int dgt = fuseObject.DeviceGroupType;
                if ((dgt == 7) || (dgt == 8))
                {
                    // Not what we want. Report message.
                    AddError(_msgValidLinkType);
                }            
            }
            return result;
        }

        /// <summary>
        /// Checks NumberFusePerPhase for Dual Well and Non-Dual Well.
        /// </summary>
        /// <returns></returns>
        private bool ValidateFusePerPhase(FuseObject fuseObject)
        {
          
            bool result = false;

            if (fuseObject.IsDualWell)
            {
                // Dual Well's should have 2 FusesPerPhase.
                if (fuseObject.NumberFusesPerPhase != 2)
                    AddError(string.Format(_msgValidateFusePerPhase, "Dual Well", 2));
            }
            else
            {
                // Non-Dual Well's should have 1 FusesPerPhase.
                if (fuseObject.NumberFusesPerPhase != 1)
                    AddError(string.Format(_msgValidateFusePerPhase, "Non-Dual Well", 1));

            }
            return result;
        }

        /// <summary>
        /// OperatingVoltage with set LinkRating.
        /// </summary>
        /// <returns></returns>
        private bool ValidateOpVoltLinkRating(FuseObject fuseObject)
        {
            bool result = false;

            // Look for low items.
            if (_opVoltBelow.Contains(fuseObject.OperatingVoltage))
            {
                if (!_linkRatingBelow.Contains(fuseObject.LinkRating))
                {
                    AddError(string.Format(_msgValidateOpVoltLinkRating, "12A, 25A, 30A, 40A or 50A",fuseObject.OperatingVoltageDomainValue));
                }
            }

            // Looking for high items.
            if (_opVoltAbove.Contains(fuseObject.OperatingVoltage))
            {
                if (!_linkRatingAbove.Contains(fuseObject.LinkRating))
                {
                    AddError(string.Format(_msgValidateOpVoltLinkRating, "20A, 25A, 30A, 40A or 50A", fuseObject.OperatingVoltageDomainValue));
                }
            }

            return result;
        }

        #endregion

    }

    /// <summary>
    /// Fuse helper class.
    /// </summary>
    public class FuseObject
    {
        private int _getDeviceGroupType = -1;
        private int _deviceGroupSubtype = -1;
        private string _operatingVoltageDomainValue = string.Empty;

        public FuseObject(IFeature obj)
        {
            this.Feature = obj;
            this.OID = obj.OID;
            this.SubType = obj.GetFieldValue("SubTypeCD", false,"").Convert<int>(-1);
            this.LinkRating = obj.GetFieldValue("LinkRating", false, "").Convert<int>(-1);
            this.LinkType = obj.GetFieldValue("LinkType", false, "").Convert<int>(-1);
            this.OperatingVoltage = obj.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.OperatingVoltage).Convert<int>(-1);
            this.NumberFusesPerPhase = obj.GetFieldValue("NumberFusesPerPhase", false, "").Convert<int>(-1);
        }
        public IFeature Feature { get; set; }
        public int OID { get; set; }

        [System.ComponentModel.DefaultValue(-999)]
        public int SubType { get; set; }

        public bool IsSubSurface
        {
            get
            {
                if ((this.DeviceGroupSubtype == 2))
                    return true;
                else
                    return false;
            }
        }

        [System.ComponentModel.DefaultValue(-999)]
        public int DeviceGroupType
        {
            get
            {
                if (_getDeviceGroupType <= 0)
                    _getDeviceGroupType = GetDeviceGroupInfo(this.Feature);
                return _getDeviceGroupType;
            }
        }

        public int DeviceGroupSubtype
        {
            get
            {
                if (_deviceGroupSubtype <= 0)
                    GetDeviceGroupInfo(this.Feature);

                return _deviceGroupSubtype;
            }
        }

        public bool IsDualWell
        {
            get
            {
                if (this.DeviceGroupType == 8)
                    return true;
                else
                    return false;
            }
        }
        public int LinkType { get; set; }
        public int LinkRating { get; set; }

        public int NumberFusesPerPhase { get; set; }

        public int OperatingVoltage { get; set; }

        public string OperatingVoltageDomainValue
        {
            get
            {
                if (_operatingVoltageDomainValue.Length <= 0)
                {
                    object obj = this.Feature.GetFieldValue(null, true, SchemaInfo.Electric.FieldModelNames.OperatingVoltage);
                    if (obj != null)
                        _operatingVoltageDomainValue = obj.ToString();
                }
                return _operatingVoltageDomainValue;
            }
        }

        /// <summary>
        /// Pulls field values from a Device Group feature.
        /// </summary>
        /// <param name="feature">EDGIS.DeviceGroup feature.</param>
        /// <returns></returns>
        private int GetDeviceGroupInfo(IFeature feature)
        {
            int result = -1;
            IEnumerable<IObject> relatedObjects =  feature.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.DeviceGroup);
            foreach (IObject obj in relatedObjects)
            {

                int relatedValue = obj.GetFieldValue("DeviceGroupType",false).Convert(-1);
                if (relatedValue > 0)
                {
                    result = relatedValue;
                    _getDeviceGroupType = result;
                    _deviceGroupSubtype = obj.GetFieldValue("SubTypeCD",false).Convert(-1);

                    break;
                }
            }
                return result;
        }
    }

}
