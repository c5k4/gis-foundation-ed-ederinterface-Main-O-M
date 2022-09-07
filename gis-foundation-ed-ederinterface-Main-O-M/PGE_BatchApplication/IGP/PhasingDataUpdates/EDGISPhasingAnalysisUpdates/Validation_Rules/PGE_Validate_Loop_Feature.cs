using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.Framework.FeederManager;
using Telvent.Delivery.Framework;
using System.Runtime.InteropServices;

namespace PGE.BatchApplication.IGPPhaseUpdate.Validation_Rules
{
    public class PGE_Validate_Loop_Feature
    {
        Common CommonFuntions = new Common();
        ModelNameFacade ModelNameFacadeClass = new ModelNameFacade();
        private const string _errMessage = "The feature participates in loop.";
        private const int _inServiceStatus = 5;

        public bool InternalIsValid(IRow row, string sCircuitID, ITable LOOP_FEATURE_QAQC_LIST_table)
        {
            string condition = null;
            string[] cond_separate_options = null;
            string[] cond_options_else = null;
            string[] condit_loop = null;
            string[] condit_comma = null;
            //string new_loop = null;
            bool shouldValidate1 = true;
            //bool shouldValidate3 = true;
            //cast row as Ifeature
            IFeature feature = null;
            //cehck if casting is successful

            int currentStatus = -1;
            IField feederManagerNonTraceable = null;
            IField pFieldStatus = null;
            try
            {
                feature = row as IFeature;
                if (feature == null)
                {
                    //_logger.Debug("Row casting to Ifeature failed.");
                    return false;
                }

                //int currentStatus = feature.GetFieldValue(null, false, SchemaInfo.Electric.FieldModelNames.Status).Convert(-1);
                pFieldStatus = ModelNameFacadeClass.FieldFromModelName(feature.Class, ReadConfigurations.Validation_ModelNames_Field.Status);
                if (feature.Fields.FindField(pFieldStatus.Name) != -1)
                {
                    if (!string.IsNullOrEmpty(feature.get_Value(feature.Fields.FindField(pFieldStatus.Name)).ToString()))
                    {
                        currentStatus = Convert.ToInt16(feature.get_Value(feature.Fields.FindField(pFieldStatus.Name)).ToString());
                    }
                }
                //Check if status is the default value
                if (currentStatus == -1)
                {
                    //_logger.Debug("Status is <Null>.");
                    return false;
                }

                feederManagerNonTraceable = ModelNameFacadeClass.FieldFromModelName(feature.Class, ReadConfigurations.Validation_ModelNames_Field.FeederManagerNonTraceable);

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
                        //if (CheckIfValidClass(feature.Class) == true)
                        //{
                        if (ModelNameFacadeClass.ContainsClassModelName(feature.Class as IObjectClass, ReadConfigurations.Validation_ModelNames_Class.SEC_GRID_SPOT_NTWRK) != true)
                        {
                            if (LOOP_FEATURE_QAQC_LIST_table == null)
                                throw new Exception("Failed to load table " +((IDataset) LOOP_FEATURE_QAQC_LIST_table).BrowseName.ToString());
                            QueryFilter queryFilter = new QueryFilterClass();
                            queryFilter.WhereClause = string.Format("FEATURECLASSNAME='{0}'", ((IDataset)feature.Class).BrowseName);
                            //IRow codeRow = new Extensions.CursorEnumerator(() => LOOP_FEATURE_QAQC_LIST_table.Search(queryFilter, false)).FirstOrDefault();
                            ICursor pCursor = null;
                            pCursor = LOOP_FEATURE_QAQC_LIST_table.Search(queryFilter, false);
                            IRow codeRow = pCursor.NextRow();
                            if (codeRow != null)
                            {
                                condition = Convert.ToString((codeRow.get_Value(codeRow.Fields.FindField("CONDITION"))));
                            }
                            if(pCursor !=null) {Marshal.ReleaseComObject(pCursor);}
                            #region QAQC loop feature condition check
                            if (condition != null)
                            {
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
                        CommonFuntions.WriteLine_Error("PGE_Validate_Loop_Feature Rule is not executed successfully for feature -OBJECTID = " + row.OID + " and Class = " + ((IDataset)row.Table).BrowseName);
             
                    }

                }
                else
                {
                    CommonFuntions.WriteLine_Error("As status is other than InService no need to validate.OBJECTID = " + row.OID  + " and Class = "  + ((IDataset)row.Table).BrowseName);
                }


                if (shouldValidate)
                {
                    //Check the loop flag -- the 6th bit
                    bool isLoop = FeederManager2.IsInLoop(feature);

                    if (isLoop)
                    {
                        //_logger.Debug(_errMessage);
                        //AddError(_errMessage);
                        string sGuid = null;
                        sGuid = feature.get_Value(feature.Fields.FindField(ReadConfigurations.GUIDFIELDNAME)).ToString();

                        Common pCommonClass = new Common();
                        pCommonClass.InsertRecordInDatatable_QAQC(((IDataset)feature.Class).BrowseName, feature.OID,sGuid, _errMessage, sCircuitID);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace); 
                return false;
            }
        }



    }
}
