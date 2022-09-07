using System;
using System.Reflection;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Framework.FeederManager;
using Microsoft.CSharp;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)]
    [Guid("62D93F45-F23D-4076-A58F-A6A299F2F21F")]
    [ProgId("PGE.Desktop.EDER.ValidatesLoopFeature")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class ValidateLoopFeature : BaseValidationRule
    {
        #region Private
        /// <summary>
        /// constant error message
        /// </summary>

        private const string _errMessage = "The feature participates in loop.";

        private static string[] _modelNames = new string[] { SchemaInfo.Electric.FieldModelNames.FeederInfo };

        private const int _inServiceStatus = 5;

        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion Private
        #region Constructor
        public ValidateLoopFeature()
            : base("PGE Validate Loop Feature", _modelNames)
        {
        }
        #endregion Constructor

        #region Overrides for validation rule
        /// <summary>
        /// Returns true if "FeederInfo" Field ModelName exist on Objectclass. 
        /// </summary>
        /// <param name="param">the ObjectClass</param>
        /// <returns></returns>
        protected override bool EnableByModelNames(object param)
        {
            if (!(param is IObjectClass))
            {
                _logger.Debug("Parameter is not type of IObjectClass, exiting");
                return false;
            }

            IObjectClass oclass = param as IObjectClass;
            _logger.Debug("ObjectClass:" + oclass.AliasName);

            //Check if FieldModelName exist on current ObjectClass fields
            bool enableForFieldModel = ModelNameFacade.ContainsFieldModelName(oclass, _modelNames);
            _logger.Debug("FieldModelName:" + _modelNames[0] + ", in ObjectClass :" + oclass.AliasName + "exist(" + enableForFieldModel + ")");

            _logger.Debug(string.Format("Returning Visible:{0}", enableForFieldModel));

            return (enableForFieldModel);
        }


        protected override ID8List InternalIsValid(IRow row)
        {
            string condition = null;
            string[] cond_separate_options = null;
            string[] cond_options_else = null;
            string[] condit_loop = null;
            string[] condit_comma = null;
            string new_loop = null;
            bool shouldValidate1 = true;
            bool shouldValidate3 = true;
            //cast row as Ifeature
            IFeature feature = row as IFeature;
            //cehck if casting is successful
            if (feature == null)
            {
                _logger.Debug("Row casting to Ifeature failed.");
                return _ErrorList;
            }

            int currentStatus = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).Convert(-1);
            //Check if status is the default value
            if (currentStatus == -1)
            {
                _logger.Debug("Status is <Null>.");
                return _ErrorList;
            }

            IField feederManagerNonTraceable = ModelNameFacade.FieldFromModelName(feature.Class, SchemaInfo.Electric.FieldModelNames.FeederManagerNonTraceable);
            bool shouldValidate = false;

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

                // QAQC- INC000004371045 - EDGIS QA/QC Rule to allow Loops
                try
                {
                    if (ModelNameFacade.ContainsClassModelName(feature.Class as IObjectClass, SchemaInfo.UFM.ClassModelNames.SEC_GRID_SPOT_NTWRK) != true)
                    {
                        IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)((IDataset)((IRow)row).Table).Workspace;
                        string LOOP_FEATURE_QAQC_LIST = "EDGIS.LOOP_FEATURE_QAQC_LIST";
                        ITable LOOP_FEATURE_QAQC_LIST_table = new MMTableUtilsClass().OpenTable(LOOP_FEATURE_QAQC_LIST, featureWorkspace);
                        if (LOOP_FEATURE_QAQC_LIST_table == null)
                            throw new Exception("Failed to load table " + LOOP_FEATURE_QAQC_LIST);
                        QueryFilter queryFilter = new QueryFilterClass();
                        queryFilter.WhereClause = string.Format("FEATURECLASSNAME='{0}'", ((IDataset)feature.Class).BrowseName);
                        IRow codeRow = new Extensions.CursorEnumerator(() => LOOP_FEATURE_QAQC_LIST_table.Search(queryFilter, false)).FirstOrDefault();
                        if (codeRow != null)
                        {
                            condition = Convert.ToString((codeRow.get_Value(codeRow.Fields.FindField("CONDITION"))));
                        }

                        #region QAQC loop feature condition check

                        if (condition.Contains(':'))
                        {
                            cond_separate_options = condition.Split(':');
                            int count = cond_separate_options.Count();
                            for (int cnt = 0; cnt < count; cnt++)
                            {
                                condit_loop = cond_separate_options[cnt].Split('=');
                                if (cond_separate_options[cnt].Contains(","))
                                {
                                    condit_loop[1] = condit_loop[1].Trim();
                                    condit_loop[1] = condit_loop[1].TrimStart('(');
                                    condit_loop[1] = condit_loop[1].TrimEnd(')');
                                    condit_comma = condit_loop[1].Split(',');
                                    int num_count = condit_comma.Count();
                                    for (int ct = 0; ct < num_count; ct++)
                                    {
                                        if (Convert.ToString(feature.get_Value(feature.Fields.FindField(condit_loop[0].Trim()))) == condit_comma[ct].Trim())
                                        {
                                            shouldValidate = false;
                                            shouldValidate1 = false;
                                            break;
                                        }
                                        else
                                        {
                                            shouldValidate1 = true;
                                        }
                                    }
                                    if (shouldValidate == true)
                                    {
                                        cnt = count;
                                    }
                                    Array.Clear(condit_loop, 0, condit_loop.Length);
                                }
                                else
                                {
                                    if (Convert.ToString(feature.get_Value(feature.Fields.FindField(condit_loop[0].Trim()))) == condit_loop[1].Trim())
                                    {
                                        shouldValidate = false;
                                        shouldValidate1 = false;
                                    }
                                    else
                                    {
                                        shouldValidate1 = true;
                                        cnt = count;
                                        break;
                                    }
                                    Array.Clear(condit_loop, 0, condit_loop.Length);
                                }
                            }
                        }
                        else
                        {
                            cond_options_else = condition.Split('=');
                            if (condition.Contains(","))
                            {
                                cond_options_else[1] = cond_options_else[1].Trim();
                                cond_options_else[1] = cond_options_else[1].TrimStart('(');
                                cond_options_else[1] = cond_options_else[1].TrimEnd(')');
                                condit_comma = cond_options_else[1].Split(',');
                                int num_count = condit_comma.Count();
                                for (int ct = 0; ct < num_count; ct++)
                                {
                                    if (Convert.ToString(feature.get_Value(feature.Fields.FindField(cond_options_else[0].Trim()))) == condit_comma[ct].Trim())
                                    {
                                        shouldValidate = false;
                                        shouldValidate1 = false;

                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (Convert.ToString(feature.get_Value(feature.Fields.FindField(cond_options_else[0].Trim()))) == cond_options_else[1].Trim())
                                {
                                    shouldValidate = false;
                                    shouldValidate1 = false;

                                }
                            }
                        }
                        if (shouldValidate == false && shouldValidate1 == false)
                        {
                            shouldValidate = false;
                        }
                        else
                        {
                            shouldValidate = true;
                        }
                        #endregion
                    }
                }
                catch (Exception ee)
                {
                }

            }
            else
            {
                _logger.Debug("As status is other than InService no need to validate.");
            }


            if (shouldValidate)
            {
                //Check the loop flag -- the 6th bit
                bool isLoop = FeederManager2.IsInLoop(feature);

                if (isLoop)
                {
                    _logger.Debug(_errMessage);
                    AddError(_errMessage);
                }
            }

            return _ErrorList;

        }

        #endregion Overrides for validation rule

    }
}
