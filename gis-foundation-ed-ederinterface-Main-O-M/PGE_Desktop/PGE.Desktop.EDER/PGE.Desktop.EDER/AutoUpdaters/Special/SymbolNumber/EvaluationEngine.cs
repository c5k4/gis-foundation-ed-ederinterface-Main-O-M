using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using PGE.Desktop.EDER.SymbolNumber.Schema;

namespace PGE.Desktop.EDER.AutoUpdaters
{
    /// <summary>
    /// Singleton class for evaluating symbol number criteria and operating number criteria.  Handles loading symbol number/validating operating number
    /// criteria and executing queries against that data. 
    /// </summary>
    public class EvaluationEngine
    {
        #region Private
        /// <summary>
        /// Logger to log all the information as Debug/Warning/Error.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Dictionary to store cached symbol number criteria by feature class name.
        /// </summary>
        private static Dictionary<string, SymbolNumberRules> symbolNumberRuleCache;

        /// <summary>
        /// Dictionary to store cached symbol number criteria by feature class name.
        /// </summary>
        private static Dictionary<string, SymbolNumberRules> operatingNumberRuleCache;

        /// <summary>
        /// Private instance member.
        /// </summary>
        private static EvaluationEngine _instance;
        private bool _enableSymbolNumberAU;

        /// <summary>
        /// Load all the external XML configured rules in SymbolNumberRules using
        /// feature, dataset and criteria class model name.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <param name="dataset">The dataset.</param>
        /// <param name="criteriaClassModelName">The criteria class model name.</param>
        /// <returns></returns>
        private static SymbolNumberRules LoadCriteria(IFeature feature, IDataset dataset, string criteriaClassModelName)
        {
            SymbolNumberRules criteria = null;
            ICursor cursor = null;

            // Load the xml symbol number parameters from the database
            try
            {

                // Read rules form the symbol number table
                ITable symbolNumbeRules = ModelNameFacade.ObjectClassByModelName(dataset.Workspace, criteriaClassModelName) as ITable;
                _logger.Debug("Got Rules table from model name:" + criteriaClassModelName);

                QueryFilter qf = new QueryFilter();
                qf.WhereClause = string.Format("FEATURE_CLASS_NAME='{0}'", dataset.Name);
                _logger.Debug("Querying the Rules table for Feature Class:" + dataset.Name);
                cursor = symbolNumbeRules.Search(qf, true);

                IRow row = null;
                //check if received cursor is not empty
                if ((row = cursor.NextRow()) != null)
                {
                    _logger.Debug("Retrieved cursor is not empty.");
                    // Deserialize rules.
                    string xmlRules = StringFacade.GetDefaultNullString(row.get_Value(ModelNameFacade.FieldIndexFromModelName(symbolNumbeRules as IObjectClass, "PGE_SYMBOL_NUMBER_XML")), null);

                    //check if deserialized rule is not null
                    if (xmlRules != null)
                    {
                        _logger.Debug("Deserialized xml rule is not <Null>.");
                        MemoryStream xmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xmlRules));

                        //reset the stream position to 0
                        xmlStream.Position = 0;
                        //Deserialize the xml stream and retrieve criteria object
                        _logger.Debug("Retrieving Criteria object from XML Stream.");
                        criteria = SymbolNumberRules.Deserialize(xmlStream);
                        _logger.Debug("Retrieved Criteria object from XML Stream.");
                        //initialize the criteria for received feature.Class

                        _logger.Debug("Initializing Criteria object for feature class" + feature.Class.AliasName + ".");
                        criteria.Initialize(feature.Class);
                        //return the criteria
                        return criteria;
                    }
                }
            }
            catch (Exception ex)
            {
                //throw if any excetption occurred
                _logger.Warn("Exception occurred while loading criteria.", ex);
                throw;
            }
            finally
            {
                //check if cursor object is not null then release it
                if (cursor != null)
                {
                    //release cursor object
                    _logger.Debug("Releasing the cursor object.");
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                    _logger.Debug("Released the cursor object.");
                }
            }
            return criteria;
        }
        #endregion Private

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="EvaluationEngine"/> class.
        /// </summary>
        public EvaluationEngine()
        {
            symbolNumberRuleCache = new Dictionary<string, SymbolNumberRules>();
            operatingNumberRuleCache = new Dictionary<string, SymbolNumberRules>();
        }
        #endregion Constructor

        #region Public
        #region Property
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static EvaluationEngine Instance
        {
            get
            {
                _logger.Debug("Checking if instance is already initialized.");
                // Check to see if the instance has been initialized.
                if (_instance == null)
                {
                    _logger.Debug("Initializing the instance for first time.");
                    // Instance is not initialized.  Create it.
                    _instance = new EvaluationEngine();
                    _logger.Debug("Instance initialized.");
                }
                _logger.Debug("returnig Instance object.");
                return _instance;
            }

        }

        /// <summary>
        /// Get/Set the EnableSymbolNumberAU 
        /// </summary>
        public bool EnableSymbolNumberAU
        {
            get { return _enableSymbolNumberAU; }
            set { _enableSymbolNumberAU = value; }
        }
        #endregion Property

        /// <summary>
        /// Resets the feature class.  Call to refresh the evaluation engine after configuration changes.
        /// </summary>
        /// <param name="name">The name.</param>
        public void ResetFeatureClass(string name)
        {
            //check if featureclass already available in cache
            _logger.Debug("Checking if feature class " + name + " already present in cahce.");
            if (symbolNumberRuleCache.ContainsKey(name))
            {
                //remove featureclass from symbol number rule cache
                _logger.Debug("feature class " + name + " already present in Symbol number rule cahce, removing it.");
                symbolNumberRuleCache.Remove(name);
                _logger.Debug("feature class " + name + " removed from cahce.");
            }

            if (operatingNumberRuleCache.ContainsKey(name))
            {
                //remove featureclass from symbol number rule cache
                _logger.Debug("feature class " + name + " already present in Operating number rule cahce, removing it.");
                symbolNumberRuleCache.Remove(name);
                _logger.Debug("feature class " + name + " removed from cahce.");
            }
        }

        /// <summary>
        /// Gets the symbol number by loading the symbol number criteria associated with the 
        /// specified feature class if necessary, and evaluates the symbol number query.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public int GetSymbolNumber(IFeature feature)
        {

            // Get the dataset for future use
            IDataset dataset = (feature.Class as IDataset);
            string featureClassName = dataset.Name;
            SymbolNumberRules criteria = null;

            _logger.Debug("Checking if criteria for feature class:" + featureClassName + " is already cahced.");
            // Check to see if the criteria for the specified featureclass has been loaded.  if not load and store the criteria for future use.
            if (symbolNumberRuleCache.ContainsKey(featureClassName))
            {
                // rules have been previously cached.  Evaluate using the cached object.
                _logger.Debug("Criteria for feature class:" + featureClassName + " is already cahced.");
                criteria = symbolNumberRuleCache[featureClassName];


                // ****************** Ticket No. INC000003747909 - Improper Symbol for Contact pole ***************
                // ****************** The criteria Object is not initialized in the second session  ***************                

                criteria.Initialize(feature.Class);

                // ************************************************************************************************
                // ************************************************************************************************

            }
            else
            {
                //rules are not previously cached evaluate and cahce now
                _logger.Debug("Criteria for feature class:" + featureClassName + " is not cahced, evaluating.");
                criteria = LoadCriteria(feature, dataset, SchemaInfo.General.ClassModelNames.SymbolNumberRules);
                //check if criteria is not null
                if (criteria != null)
                {
                    _logger.Debug("Caching criteria for feature class:" + featureClassName + ".");
                    symbolNumberRuleCache[dataset.Name] = criteria;
                }

            }


            // evaluate the symbol number query and return the symbol number.
            if (criteria != null)
            {
                _logger.Debug("Evaluating symbol number for current feature.");
                return criteria.GetSymbolNumber(feature);
            }
            //return -1 if criteria is null
            _logger.Debug("Criteria is <Null> returning -1.");
            return -1;
        }

        /// <summary>
        /// Gets the operating number error from criteria
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public List<string> GetOpertingNumberCriteria(IFeature feature)
        {
            // Get the dataset for future use
            IDataset dataset = (feature.Class as IDataset);
            string featureClassName = dataset.Name;
            SymbolNumberRules criteria = null;

            _logger.Debug("Checking if criteria for feature class:" + featureClassName + " is already cahced.");
            // Check to see if the criteria for the specified featureclass has been loaded.  if not load and store the criteria for future use.
            if (operatingNumberRuleCache.ContainsKey(featureClassName))
            {
                // rules have been previously cached.  Evaluate using the cached object.
                _logger.Debug("Criteria for feature class:" + featureClassName + " is already cahced.");
                criteria = operatingNumberRuleCache[featureClassName];
            }
            else
            {
                _logger.Debug("Criteria for feature class:" + featureClassName + " is not cahced, evaluating.");
                criteria = LoadCriteria(feature, dataset, SchemaInfo.General.ClassModelNames.OperatingNumberRules);
                //check if criteria is not null
                if (criteria != null)
                {
                    //cache the criteria for future use
                    _logger.Debug("Caching criteria for feature class:" + featureClassName + ".");
                    operatingNumberRuleCache[dataset.Name] = criteria;
                }
            }


            // evaluate the operating number query and return the operating number criteria.
            if (criteria != null)
            {
                _logger.Debug("Evaluating symbol number for current feature.");
                return criteria.ValidateOperatingNumber(feature);
            }

            _logger.Debug("Criteria is <Null> returning null.");
            return null;
        }
        #endregion Public
    }
}
