using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Loads, retains, and handles rules for neutral phase conductors.
    /// </summary>
    public class NeutralPhaseHelper
    {
        #region private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private static NeutralPhaseHelper _instance = null;

        private string _defaultIntegration = string.Empty;
        private static string _xmlFilePath = @"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Config";// @"C:\Projects\pge\ED\Desktop\PGE.Desktop.EDER\ConfigNeutralPhase\"; ///
        private const string _integratnName = "Desktop-NeutralConductor";

        /// <summary>
        /// Initializes the DomainManager with the configuration in _xmlFilePath.
        /// </summary>
        private void Initialize()
        {
            //check if Domain name manager is already instantiated
            if (string.IsNullOrEmpty(_defaultIntegration))
            {
                //Read the config location from the Registry Entry
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                _xmlFilePath = sysRegistry.ConfigPath;
                //prepare the xmlfile if config path exist
                _logger.Debug("Creating Instance of group in cache.");
                string domainsXmlFilePath = System.IO.Path.Combine(_xmlFilePath, "NeutralConductorConfig.xml");
                XmlDocument document = new XmlDocument();
                if (File.Exists(domainsXmlFilePath))//check for file path
                {
                    //create xml document
                    document.Load(domainsXmlFilePath);
                    //instantiate the DomaunManager class
                    DomainManager.Instance.Initialize(document.FirstChild, _integratnName);
                    //set the default integration value so that next time it wont instantiate again
                    _defaultIntegration = DomainManager.Instance.DefaultIntegration;
                }
                else
                {
                    _logger.Warn("NeturalConductorConfig.xml file not found.");
                }
                _logger.Debug("Created Instance of group in cache.");
            }
            _logger.Debug("Initialized.");
        }

        /// <summary>
        /// Returns feature value of if OverHead then ConductorUse else ConductorCode field.
        /// </summary>
        /// <param name="infoObject">The InfoObject.</param>
        /// <param name="isOverHead">True if featureClass is Overhead.</param>
        /// <returns></returns>
        private string GetValue(IObject infoObject, bool isOverHead)
        {
            //Get the value of ConductorUse or ConductorCode depending on IsOverhead flag
            _logger.Debug("Getting vaue depending on isOverHead flag.");
            string fieldModel = isOverHead ? SchemaInfo.Electric.FieldModelNames.CondutorUse : SchemaInfo.Electric.FieldModelNames.ConductorCode;
            _logger.Debug("Field:" + fieldModel + ".");
            //Get the value of field and convert to "" if Null return the same
            return infoObject.GetFieldValue(null, false, fieldModel).Convert("");

            //int idx = ModelNameFacade.FieldIndexFromModelName(infoObject.Class, fieldModel);
            //_logger.Debug("FieldIndex:" + idx + ".");
            //if (idx != -1)
            //{
            //    object objVal = infoObject.get_Value(idx);
            //    if (!DBNull.Value.Equals(objVal))
            //    {
            //        _logger.Debug("Value is " + objVal.ToString() + ".");
            //        return objVal.ToString();
            //    }
            //    _logger.Debug("Value is Null.");
            //}
            //_logger.Debug("Returning empty string.");
            //return string.Empty;
        }

        /// <summary>
        /// Checks if parametar feature is either Primary Overhead conductor or Secondary Overhead Conductor.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        private bool IsOverHead(IObject feature)
        {
            //check for defined OverHead model names against this feature
            _logger.Debug("Checking for OverHead.");
            string[] modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryOHConductor, SchemaInfo.Electric.ClassModelNames.SecondaryOHConductor, SchemaInfo.Electric.ClassModelNames.PrimaryOHConductorInfo, SchemaInfo.Electric.ClassModelNames.SecondaryOHConductorInfo };
            IObjectClass featureObjectClass = feature.Class;
            //check if any of OverHead class model name present then return true else false
            bool isOverHead = ModelNameFacade.ContainsClassModelName(featureObjectClass, modelNames);
            _logger.Debug("Checked for OverHead:" + isOverHead);
            return isOverHead;
        }

        /// <summary>
        /// Returns if ConductorInfo record is Neutral.
        /// </summary>
        /// <param name="ConductorInfoObject">The ConductorInfo record.</param>
        /// <param name="isOverHead">True if featureClass is Overhead.</param>
        /// <param name="CheckBeforeEdit">Defatult false, True only if Need to check if info record before edit.</param>
        /// <returns></returns>
        private bool IsNeutral(IObject ConductorInfoObject, bool isOverHead, bool CheckBeforeEdit = false)
        {
            //Initialize the external config
            Initialize();
            _logger.Debug("Checking IsNeutral.");
            //if overhead then  ConductorUse else ConductorCode
            string fieldModelName = isOverHead ? SchemaInfo.Electric.FieldModelNames.CondutorUse : SchemaInfo.Electric.FieldModelNames.ConductorCode;
            int idx = ModelNameFacade.FieldIndexFromModelName(ConductorInfoObject.Class, fieldModelName);
            //check if modelname found
            if (idx == -1)
            {
                _logger.Warn("FieldModelName:" + fieldModelName + " is not present in ObjectClass:" + ConductorInfoObject.Class.AliasName);
                return false;
            }
            //get the field
            IField fld = ConductorInfoObject.Fields.get_Field(idx);
            //check if field is not null
            if (fld.Domain == null)
            {
                //log the warning
                _logger.Warn("FieldModelName:" + fieldModelName + " do not have domain assigned in ObjectClass:" + ConductorInfoObject.Class.AliasName);
                return false;
            }

            string domainName = fld.Domain.Name.ToUpper();

            string value = "";

            if (CheckBeforeEdit)
            {
                IRowChanges rowChanges = ConductorInfoObject as IRowChanges;
                value = rowChanges.get_OriginalValue(idx).Convert("");
            }
            else
            {
                value = GetValue(ConductorInfoObject, isOverHead);
            }
            //get mapping value for domain
            string mappingValue = DomainManager.Instance.GetValue(domainName, value, _defaultIntegration);
            _logger.Debug("Checked IsNeutral:" + mappingValue + ".");
            //check if mapping returns Yes return true else return false
            return (mappingValue != null && mappingValue.ToUpper() == "YES");
        }

        /// <summary>
        /// Returns if PhaseDesignation value is Neutral or Unknown.
        /// </summary>
        /// <param name="ConductorInfoObject">The ConductorInfo record.</param>
        /// <param name="isOverHead">True if featureClass is Overhead.</param>
        /// <param name="CheckBeforeEdit">Defatult false, True only if Need to check if info record before edit.</param>
        /// <returns></returns>
        private bool IsPhaseDesignationNeutral(IObject ConductorInfoObject, bool CheckBeforeEdit)
        {
            //Initialize the external config
            Initialize();
            _logger.Debug("Checking IsNeutral.");
            //if overhead then  ConductorUse else ConductorCode
            string fieldModelName = SchemaInfo.Electric.FieldModelNames.PhaseDesignation;
            int idx = ModelNameFacade.FieldIndexFromModelName(ConductorInfoObject.Class, fieldModelName);
            //check if modelname found
            if (idx == -1)
            {
                _logger.Warn("FieldModelName:" + fieldModelName + " is not present in ObjectClass:" + ConductorInfoObject.Class.AliasName);
                return false;
            }
            //get the field
            IField fld = ConductorInfoObject.Fields.get_Field(idx);
            //check if field is not null
            if (fld.Domain == null)
            {
                //log the warning
                _logger.Warn("FieldModelName:" + fieldModelName + " do not have domain assigned in ObjectClass:" + ConductorInfoObject.Class.AliasName);
                return false;
            }

            string domainName = fld.Domain.Name.ToUpper();

            string value = "";

            if (CheckBeforeEdit)
            {
                IRowChanges rowChanges = ConductorInfoObject as IRowChanges;
                value = rowChanges.get_OriginalValue(idx).Convert("");
            }
            else
            {
                value = ConductorInfoObject.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.PhaseDesignation).Convert("");
            }
            //get mapping value for domain
            string mappingValue = DomainManager.Instance.GetValue(domainName, value, _defaultIntegration);
            _logger.Debug("Checked IsNeutral:" + mappingValue + ".");
            //check if mapping returns Yes return true else return false
            return (mappingValue != null && mappingValue.ToUpper() == "YES");
        }

        #endregion private

        #region public

        #region properties
        /// <summary>
        /// Gets default instanceof NeutralPhaseHelper class.
        /// </summary>
        public static NeutralPhaseHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NeutralPhaseHelper();
                }
                return _instance;
            }
        }
        #endregion properties

        /// <summary>
        /// Returns if Conductor record is neutral.
        /// </summary>
        /// <param name="ConductorFeature">The CondictorFeature.</param>
        /// <returns></returns>
        public bool HasNeutral(IFeature ConductorFeature)
        {
            _logger.Debug("Checking for HasNeutral.");
            bool isOverHead = IsOverHead(ConductorFeature as IObject);
            IEnumerable<IObject> relatedObjects = ConductorFeature.GetRelatedObjects(null, esriRelRole.esriRelRoleAny, SchemaInfo.Electric.ClassModelNames.ConductorInfo);
            foreach (IObject currentObject in relatedObjects)
            {
                if (IsNeutral(currentObject, isOverHead))
                {
                    //Neutral info record found
                    _logger.Debug("Checked for HasNeutral returning(true).");
                    return true;
                }
            }
            //checked for all Info records no Neutral found 
            _logger.Debug("Checked for HasNeutral returning(false).");
            return false;
        }

        /// <summary>
        /// Returns if conductor info record is neutral.
        /// </summary>
        /// <param name="ConductorInfoObject">The ConductorInfo record.</param>
        /// <returns></returns>
        public bool IsPhaseDesignationNeutral(IObject ConductorInfoObject)
        {
            return this.IsPhaseDesignationNeutral(ConductorInfoObject, false);
        }

        /// <summary>
        /// Returns if conductor info record is neutral.
        /// </summary>
        /// <param name="ConductorInfoObject">The ConductorInfo record.</param>
        /// <returns></returns>
        public bool IsPhaseDesignationNeutralBeforeEdit(IObject ConductorInfoObject)
        {
            return this.IsPhaseDesignationNeutral(ConductorInfoObject, true);
        }


        /// <summary>
        /// Returns if conductor info record is neutral.
        /// </summary>
        /// <param name="ConductorInfoObject">The ConductorInfo record.</param>
        /// <returns></returns>
        public bool IsNeutral(IObject ConductorInfoObject)
        {
            //get the flag if current ConductorInfo is OverHead or UnderGround
            bool isOverHead = IsOverHead(ConductorInfoObject);
            //pass the params to process further
            return IsNeutral(ConductorInfoObject, isOverHead);
        }

        /// <summary>
        /// Returns if conductor info record is neutral before it was edited.
        /// </summary>        
        /// <param name="ConductorInfoObject">The ConductorInfo record.</param>
        /// <returns></returns>
        public bool IsNeutralBeforeEdit(IObject ConductorInfoObject)
        {
            //get the flag if current ConductorInfo is OverHead or UnderGround
            bool isOverHead = IsOverHead(ConductorInfoObject);
            //pass the params to process further
            return IsNeutral(ConductorInfoObject, isOverHead, true);
        }


        #endregion public
    }
}
