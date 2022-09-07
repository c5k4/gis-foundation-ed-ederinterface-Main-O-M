using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Collections.Generic;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Geometry;
using System.Timers;
using System.Diagnostics;
using Miner.Geodatabase.Edit;
using PGE.Common.Delivery.Framework.FeederManager;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("48D8DBA1-78BA-41F6-B60D-3378CBEEE778")]
    [ProgId("PGE.Desktop.EDER.ValidateSourceConnectivity")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateSourceConnectivity:BaseValidationRule
    {
        #region Constructors
        public ValidateSourceConnectivity()
            : base("PGE Validate Source Connectivity", _modelNames)
        {    
        }
        #endregion Constructors

        #region Private
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        
        //private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.Transformer, SchemaInfo.Electric.ClassModelNames.Conductor };
        // Modified for all connected features to be validated
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.FieldModelNames.FeederID };
       
        private const string _errMsg = "In-Service {0} is de-energized.";
        private const int _inServiceStatus = 5;

        /// <summary>
        /// Creates ErrorMessage to be added if error found in validation.
        /// </summary>
        /// <param name="featureClass">The Feature Class.</param>
        /// <returns></returns>
        private string CreateErrorMessage(IFeatureClass featureClass)
        {
            string classType = "";
            //Check if FeatureClass is Conductor
            if(ModelNameFacade.ContainsClassModelName(featureClass, SchemaInfo.Electric.ClassModelNames.Conductor))
            {
                classType="Conductor";
            }
            //Check if FeatureClass is Transformer
            else if (ModelNameFacade.ContainsClassModelName(featureClass, SchemaInfo.Electric.ClassModelNames.Transformer))
            {
                classType = "Transformer";
            }
            //if FeatureClass is not Conductor nor Transformer then log the error and return error message as empty string.
            else
            {
                classType = featureClass.AliasName;
                //_logger.Debug("Feature class calling this validation is Other than Transformer and Conductor.");
                //return "";
            }
            return (string.Format(_errMsg, classType));
        }
        #endregion Private

        #region Override for Source Connectivity validator
        /// <summary>
        /// Validtes the object for defined rule.
        /// </summary>
        /// <param name="row">the Object to be validated.</param>
        /// <returns>Error list</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            IFeature feature = row as IFeature;

            //Check if feature is not null
            if (feature != null)
            {
                //Get the Status value
                int currentStatus = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).Convert(-1);
                //Check if status is the default value
                if (currentStatus == -1)
                {
                    _logger.Debug("Status is <Null>.");
                    return _ErrorList;
                }

                IFeatureClass featureClass = feature.Class as IFeatureClass;
                string errorMessage = CreateErrorMessage(featureClass);
                
                //Check if errorMessage is not null/empty
                if(!string.IsNullOrEmpty(errorMessage))
                {
                    IField feederManagerNonTraceable = ModelNameFacade.FieldFromModelName(feature.Class, SchemaInfo.Electric.FieldModelNames.FeederManagerNonTraceable);
                    bool shouldValidate = false;

                    //Check if current status is equal to InService status value
                    if (currentStatus == _inServiceStatus)
                    {
                        if (feederManagerNonTraceable != null)
                        {
                            object nonTraceableValue = feature.get_Value(feature.Fields.FindField(feederManagerNonTraceable.Name));
                            if (nonTraceableValue != null && nonTraceableValue.ToString() != "1")
                            {
                                //Non traceable field is set to traceable so we should validate this feature
                                shouldValidate = true;
                            }

                        }
                        else
                        {
                            shouldValidate = true;
                        }

                    }
                    else
                    {
                        _logger.Debug( "As status is other than InService no need to validate.");
                    }

                    if (shouldValidate)
                    {
                        //Obtaining via our new feeder manager functionality for feeder manager 2.0
                        string[] circuitIDs = FeederManager2.GetCircuitIDsFORGDBM(feature);
                        //Check if any circuit IDs exist.  If our list is less that one then none exist
                        if (circuitIDs == null || circuitIDs.Length < 1)
                        {
                            _logger.Debug(errorMessage);
                            AddError(errorMessage);
                        }
                    }
                }
                else
                {
                    _logger.Debug("InternalEnable failed / configured incorrectly on feature class.");
                }
            }
            else 
            {
                _logger.Warn("Feature is <Null>.");
            }
            return _ErrorList;
        }
        #endregion Override for Source Connectivity validator

    }
}
