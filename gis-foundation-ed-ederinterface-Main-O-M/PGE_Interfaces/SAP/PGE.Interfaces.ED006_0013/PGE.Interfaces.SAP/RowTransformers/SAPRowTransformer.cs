using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.SAP.Data;
using System.Reflection;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Class for converting GIS data to its SAP equivalent. Adds extra SAP logic on top of the BaseRowTransformer.
    /// </summary>
    public class SAPRowTransformer : BaseRowTransformer, ISAPRowTransformer<IRow>
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.Interfaces.SAP.log4net.config");
        private DataHelper dbObject = new DataHelper(false);
        public static Dictionary<int, Dictionary<string , string>> fieldNameValueSeq;

        #region IRowTransformer<IRow> Members
        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns></returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            ActionType action;
            List<IRowData> sapRowDataList = new List<IRowData>();
            try
            {
                action = GetActionType(sourceRow, targetRow, changeType);
                //the value returned can be NotApplicable, Invalid, Insert, update, delete, idle, premap

                if (action == ActionType.Invalid)
                {
                    int objId;
                    if (changeType == ChangeType.Insert)
                    {
                        objId = sourceRow.OID;
                    }
                    else
                    {
                        objId = targetRow.OID;
                    }

                    string msg = "Invalid Action Type for row " + objId + " of " + _trackedClass.SourceClass;
                    Exception e = new Exception(msg, null);
                    throw e;
                }

                

                if (action == ActionType.PreMap)
                {
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " ActionType PreMap " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info(sourceRow.OID + " ActionType PreMap " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Error(sourceRow.OID + " ActionType PreMap " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    return sapRowDataList;
                }

                //by here, the action can only be NotApplicable, Insert, update, delete, idle
                if (action != ActionType.NotApplicable)
                {
                    IRow activeRow = sourceRow;
                    if (action == ActionType.Delete)
                    {
                        activeRow = targetRow;
                    }

                    string customerOwned = GetCustomerOwnedValue(activeRow);


                    if (action == ActionType.Delete || action == ActionType.Insert)
                    {
                        if (customerOwned == "Y")
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                            _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                            return sapRowDataList;
                        }
                    }
                    //Below code added for EDGIS-Rearch to exculde changes made by ED07 user-v1t8
                    bool Ed07User = dbObject.ValidateLastUser(activeRow);
                    
                    if (action == ActionType.Update || action == ActionType.Idle)
                    {

                        //Below check is added for EDGIS-Rearch project to exculde changes made by ED07 user-v1t8
                        if (Ed07User)
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as modified user is ED07 user " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            _logger.Info("skipping this feature as modified user is ED07 user ");
                            _logger.Info(sourceRow.OID + "skipping this feature as modified user is ED07 user ");
                            return sapRowDataList;
                        }

                        if (targetRow != null)
                        {
                            string preCustomerOwned = "";
                            Settings settings = _trackedClass.Settings;
                            //if the processed sourceRow and targetRow are passed down with values of relatedSourceRow and relatedTargetRow in relationshipRowTransformer
                            //the targetRow needs to be overriden since the passed in relatedTargetRow is the same as relatedSourceRow
                            //but do not override the targetRow to avoid affect other logic. Just use a temparary object in getting customer owned value to ensure the target version info is not missing
                            if (settings != null && !string.IsNullOrEmpty(settings["OverrideTargetRows"]))
                            {
                                if (settings["OverrideTargetRows"] == "Y")
                                {
                                    //Change INC000003855685======================================= 
                                    //This breaks for new TransformerUnits when the Transformer is 
                                    //CustomerOwned (because the row does not exist in sde.default) 
                                    //IRow targetRow1 = _trackedClass.TargetTable.GetRow(sourceRow.OID);
                                    //preCustomerOwned = GetCustomerOwnedValue(targetRow1);                            
                                    IQueryFilter pQF = new QueryFilterClass();
                                    pQF.WhereClause = _trackedClass.TargetTable.OIDFieldName + " = " +
                                        sourceRow.OID.ToString();
                                    int rowCount = _trackedClass.TargetTable.RowCount(pQF);
                                    if (rowCount != 0)
                                    {
                                        IRow targetRow1 = _trackedClass.TargetTable.GetRow(sourceRow.OID);
                                        preCustomerOwned = GetCustomerOwnedValue(targetRow1);
                                    }
                                    else
                                    {
                                        //This transformerunit is being inserted - since it does not exist 
                                        //in target (sde.DEFAULT) 
                                        if (customerOwned == "Y")
                                        {
                                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                            GISSAPIntegrator._logger.Info(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes ");
                                            GISSAPIntegrator._logger.Info(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes ");
                                            return sapRowDataList; //ignore this - should not go to SAP 
                                        }
                                    }
                                    //============================================================= 
                                }
                            }
                            else
                            {
                                preCustomerOwned = GetCustomerOwnedValue(targetRow);
                            }
                            if (customerOwned == "Y" && preCustomerOwned == "Y")
                            {
                                GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                                _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                                return sapRowDataList;

                            }
                            else if (customerOwned == "Y" && !(preCustomerOwned == "Y"))
                            {
                                action = ActionType.Delete;
                            }
                            else if (customerOwned != "Y" && (preCustomerOwned == "Y") && action == ActionType.Update)
                            {
                                action = ActionType.Insert;
                            }
                        }
                        else
                        {
                            if (customerOwned == "Y")
                            {
                                GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                                _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                                return sapRowDataList;

                            }
                        }
                    }
                }

                List<IRowData> rowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                #region Added for Rearch Project -v1t8
                // fieldNameSeq = base.fieldSequenceAndNameValue; 

                #endregion


                foreach (IRowData rowData in rowDataList)
                {
                    SAPRowData sapRow = new SAPRowData(rowData);

                    // if this is one of the related rows of a composite asset row, 
                    // only one of origin-destination/origin-destination rows will be used to determine ActionType
                    if (action != ActionType.NotApplicable)
                    {
                        // this was here so asset rows with InvalidFieldTransformationException will be passed
                        // to DataPersistor as Invalid rows
                        //if (rowData.Valid == false)
                        //{
                        //    action = ActionType.Invalid;
                        //}
                        sapRow.ActionType = action;
                    }

                    sapRow.SAPType = GetSAPType(sourceRow, targetRow, changeType);

                    //sapRow.ParentID = GetParentID(sourceRow, targetRow);
                    sapRowDataList.Add(sapRow);
                    _logger.Info("This feature is added in sapRowDataList" + sapRow.ToString());
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch(Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                _logger.Error("Exception in ProcessRow"+ ex.Message);
                return sapRowDataList;
            }

            return sapRowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns></returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            ActionType action;
            List<IRowData> sapRowDataList = new List<IRowData>();
            try
            {
                action = GetActionType(sourceRow, targetRow, changeType);
                //the value returned can be NotApplicable, Invalid, Insert, update, delete, idle, premap

                if (action == ActionType.Invalid)
                {
                    int objId;
                    if (changeType == ChangeType.Insert)
                    {
                        objId = sourceRow.OID;
                    }
                    else
                    {
                        objId = targetRow.OID;
                    }

                    string msg = "Invalid Action Type for row " + objId + " of " + _trackedClass.SourceClass;
                    Exception e = new Exception(msg, null);
                    throw e;
                }



                if (action == ActionType.PreMap)
                {
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " ActionType PreMap " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info(sourceRow.OID + " ActionType PreMap ");
                    GISSAPIntegrator._logger.Error(sourceRow.OID + " ActionType PreMap ");
                    return sapRowDataList;
                }

                //by here, the action can only be NotApplicable, Insert, update, delete, idle
                if (action != ActionType.NotApplicable)
                {
                    IRow activeRow = sourceRow;
                    //if (action == ActionType.Delete)
                    //{
                    //    activeRow = targetRow;
                    //}

                    string customerOwned = GetCustomerOwnedValue(activeRow);


                    if (action == ActionType.Delete || action == ActionType.Insert)
                    {
                        if (customerOwned == "Y")
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                            _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                            return sapRowDataList;
                        }
                    }
                    //Below code added for EDGIS-Rearch to exculde changes made by ED07 user-v1t8
                    bool Ed07User = dbObject.ValidateLastUser(activeRow);
                    
                    if (action == ActionType.Update || action == ActionType.Idle)
                    {

                        //Below check is added for EDGIS-Rearch project to exculde changes made by ED07 user-v1t8
                        if (Ed07User)
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as modified user is ED07 user " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            _logger.Info("skipping this feature as modified user is ED07 user ");
                            _logger.Error(sourceRow.OID + "skipping this feature as modified user is ED07 user ");
                            return sapRowDataList;
                        }

                        if (targetRow != null)
                        {
                            string preCustomerOwned = "";
                            Settings settings = _trackedClass.Settings;
                            //if the processed sourceRow and targetRow are passed down with values of relatedSourceRow and relatedTargetRow in relationshipRowTransformer
                            //the targetRow needs to be overriden since the passed in relatedTargetRow is the same as relatedSourceRow
                            //but do not override the targetRow to avoid affect other logic. Just use a temparary object in getting customer owned value to ensure the target version info is not missing
                            if (settings != null && !string.IsNullOrEmpty(settings["OverrideTargetRows"]))
                            {
                                if (settings["OverrideTargetRows"] == "Y")
                                {
                                    //Change INC000003855685======================================= 
                                    //This breaks for new TransformerUnits when the Transformer is 
                                    //CustomerOwned (because the row does not exist in sde.default) 
                                    //IRow targetRow1 = _trackedClass.TargetTable.GetRow(sourceRow.OID);
                                    //preCustomerOwned = GetCustomerOwnedValue(targetRow1);                            
                                    IQueryFilter pQF = new QueryFilterClass();
                                    pQF.WhereClause = _trackedClass.TargetTable.OIDFieldName + " = " +
                                        sourceRow.OID.ToString();
                                    int rowCount = _trackedClass.TargetTable.RowCount(pQF);
                                    if (rowCount != 0)
                                    {
                                        IRow targetRow1 = _trackedClass.TargetTable.GetRow(sourceRow.OID);
                                        preCustomerOwned = GetCustomerOwnedValue(targetRow1);
                                    }
                                    else
                                    {
                                        //This transformerunit is being inserted - since it does not exist 
                                        //in target (sde.DEFAULT) 
                                        if (customerOwned == "Y")
                                        {
                                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                            GISSAPIntegrator._logger.Info(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes ");
                                            GISSAPIntegrator._logger.Error(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes ");
                                            return sapRowDataList; //ignore this - should not go to SAP 
                                        }
                                    }
                                    //============================================================= 
                                }
                            }
                            else
                            {
                                preCustomerOwned = GetCustomerOwnedValue(targetRow);
                            }
                            if (customerOwned == "Y" && preCustomerOwned == "Y")
                            {
                                GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                                _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                                return sapRowDataList;

                            }
                            else if (customerOwned == "Y" && !(preCustomerOwned == "Y"))
                            {
                                action = ActionType.Delete;
                            }
                            else if (customerOwned != "Y" && (preCustomerOwned == "Y") && action == ActionType.Update)
                            {
                                action = ActionType.Insert;
                            }
                        }
                        else
                        {
                            if (customerOwned == "Y")
                            {
                                _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                                _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                                return sapRowDataList;
                            }
                        }
                    }
                }

                List<IRowData> rowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                #region Added for Rearch Project -v1t8
                // fieldNameSeq = base.fieldSequenceAndNameValue; 

                #endregion


                foreach (IRowData rowData in rowDataList)
                {
                    SAPRowData sapRow = new SAPRowData(rowData);

                    // if this is one of the related rows of a composite asset row, 
                    // only one of origin-destination/origin-destination rows will be used to determine ActionType
                    if (action != ActionType.NotApplicable)
                    {
                        // this was here so asset rows with InvalidFieldTransformationException will be passed
                        // to DataPersistor as Invalid rows
                        //if (rowData.Valid == false)
                        //{
                        //    action = ActionType.Invalid;
                        //}
                        sapRow.ActionType = action;
                    }

                    sapRow.SAPType = GetSAPType(sourceRow, targetRow, changeType);

                    //sapRow.ParentID = GetParentID(sourceRow, targetRow);
                    sapRowDataList.Add(sapRow);
                    _logger.Info("This feature is added in sapRowDataList" + sapRow.ToString());
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                _logger.Error("Exception in ProcessRow" + ex.Message);
                return sapRowDataList;
            }

            return sapRowDataList;
        }


        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns></returns>
        public override List<IRowData> ProcessRow(DeleteFeat sourceRow, ChangeType changeType)
        {
            //dynamic sourceRow = sourceRow1;
            ActionType action;
            List<IRowData> sapRowDataList = new List<IRowData>();
            try
            {
                //V3SF - Check and Update Pending
                //action = ActionType.Delete;//GetActionType(sourceRow, changeType);
                action = GetActionType(sourceRow, changeType);
                //the value returned can be NotApplicable, Invalid, Insert, update, delete, idle, premap

                if (action == ActionType.Invalid)
                {
                    int objId;
                    if (changeType == ChangeType.Insert)
                    {
                        objId = sourceRow.OID;
                    }
                    else
                    {
                        objId = sourceRow.OID;
                    }

                    string msg = "Invalid Action Type for row " + objId + " of " + _trackedClass.SourceClass;
                    Exception e = new Exception(msg, null);
                    throw e;
                }



                if (action == ActionType.PreMap)
                {
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " ActionType PreMap " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info(sourceRow.OID + " ActionType PreMap ");
                    GISSAPIntegrator._logger.Error(sourceRow.OID + " ActionType PreMap ");
                    return sapRowDataList;
                }

                //by here, the action can only be NotApplicable, Insert, update, delete, idle
                if (action != ActionType.NotApplicable)
                {
                    //IRow activeRow = sourceRow;
                    DeleteFeat activeRow = sourceRow;
                    //if (action == ActionType.Delete)
                    //{
                    //    activeRow = targetRow;
                    //}

                    //activeRow.
                    string customerOwned = GetCustomerOwnedValue(activeRow);


                    if (action == ActionType.Delete || action == ActionType.Insert)
                    {
                        if (customerOwned == "Y")
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                            _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                            return sapRowDataList;
                        }
                    }
                    dbObject = new DataHelper(false);
                    //Below code added for EDGIS-Rearch to exculde changes made by ED07 user-v1t8
                    bool Ed07User = dbObject.ValidateLastUser(activeRow);
                    
                    if (action == ActionType.Update || action == ActionType.Idle)
                    {

                        //Below check is added for EDGIS-Rearch project to exculde changes made by ED07 user-v1t8
                        if (Ed07User)
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as modified user is ED07 user " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            _logger.Info("skipping this feature as modified user is ED07 user ");
                            _logger.Error(sourceRow.OID + "skipping this feature as modified user is ED07 user ");
                            return sapRowDataList;
                        }

                        if (activeRow != null)
                        {
                            string preCustomerOwned = "";
                            Settings settings = _trackedClass.Settings;
                            //if the processed sourceRow and targetRow are passed down with values of relatedSourceRow and relatedTargetRow in relationshipRowTransformer
                            //the targetRow needs to be overriden since the passed in relatedTargetRow is the same as relatedSourceRow
                            //but do not override the targetRow to avoid affect other logic. Just use a temparary object in getting customer owned value to ensure the target version info is not missing
                            if (settings != null && !string.IsNullOrEmpty(settings["OverrideTargetRows"]))
                            {
                                if (settings["OverrideTargetRows"] == "Y")
                                {
                                    //Change INC000003855685======================================= 
                                    //This breaks for new TransformerUnits when the Transformer is 
                                    //CustomerOwned (because the row does not exist in sde.default) 
                                    //IRow targetRow1 = _trackedClass.TargetTable.GetRow(sourceRow.OID);
                                    //preCustomerOwned = GetCustomerOwnedValue(targetRow1);                            
                                    //IQueryFilter pQF = new QueryFilterClass();
                                    //pQF.WhereClause = _trackedClass.TargetTable.OIDFieldName + " = " +
                                    //    activeRow.OID.ToString();
                                    //int rowCount = activeRow.;//_trackedClass.TargetTable.RowCount(pQF);
                                    //if (rowCount != 0)
                                    if (activeRow != null)
                                    {
                                        //IRow targetRow1 = _trackedClass.TargetTable.GetRow(activeRow.OID);
                                        preCustomerOwned = GetCustomerOwnedValue(activeRow);
                                    }
                                    else
                                    {
                                        //This transformerunit is being inserted - since it does not exist 
                                        //in target (sde.DEFAULT) 
                                        if (customerOwned == "Y")
                                        {
                                            GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                            GISSAPIntegrator._logger.Info(sourceRow.OID + " ActionType PreMap ");
                                            GISSAPIntegrator._logger.Error(sourceRow.OID + " ActionType PreMap ");
                                            return sapRowDataList; //ignore this - should not go to SAP 
                                        }
                                    }
                                    //============================================================= 
                                }
                            }
                            else
                            {
                                preCustomerOwned = GetCustomerOwnedValue(activeRow);
                            }
                            if (customerOwned == "Y" && preCustomerOwned == "Y")
                            {
                                GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                                _logger.Error(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                                return sapRowDataList;

                            }
                            else if (customerOwned == "Y" && !(preCustomerOwned == "Y"))
                            {
                                action = ActionType.Delete;
                            }
                            else if (customerOwned != "Y" && (preCustomerOwned == "Y") && action == ActionType.Update)
                            {
                                action = ActionType.Insert;
                            }
                        }
                        else
                        {
                            if (customerOwned == "Y")
                            {
                                GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " skipping this feature as Customer Owned is selected as Yes " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                _logger.Info("skipping this feature as Customer Owned is selected as Yes");
                                _logger.Info(sourceRow.OID + "skipping this feature as Customer Owned is selected as Yes");
                                return sapRowDataList;

                            }
                        }
                    }
                }

                List<IRowData> rowDataList = base.ProcessRow(sourceRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    {
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info(sourceRow.OID + Convert.ToString(base._errorMessage));
                    GISSAPIntegrator._logger.Error(sourceRow.OID + Convert.ToString(base._errorMessage));
                }

                #region Added for Rearch Project -v1t8
                // fieldNameSeq = base.fieldSequenceAndNameValue; 

                #endregion


                foreach (IRowData rowData in rowDataList)
                {
                    SAPRowData sapRow = new SAPRowData(rowData);

                    // if this is one of the related rows of a composite asset row, 
                    // only one of origin-destination/origin-destination rows will be used to determine ActionType
                    if (action != ActionType.NotApplicable)
                    {
                        // this was here so asset rows with InvalidFieldTransformationException will be passed
                        // to DataPersistor as Invalid rows
                        //if (rowData.Valid == false)
                        //{
                        //    action = ActionType.Invalid;
                        //}
                        sapRow.ActionType = action;
                    }

                    sapRow.SAPType = GetSAPType(sourceRow,_trackedClass.SourceTable, changeType);

                    //sapRow.ParentID = GetParentID(sourceRow, targetRow);
                    sapRowDataList.Add(sapRow);
                    _logger.Info("This feature is added in sapRowDataList" + sapRow.ToString());
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                _logger.Error("Exception in ProcessRow" + ex.Message);
                return sapRowDataList;
            }

            return sapRowDataList;
        }

        #endregion

        #region ISAPRowTransformer<IRow> Members
        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <returns>Throws NotImplementedException</returns>
        public string GetParentID(IRow sourceRow, IRow targetRow)
        {
            GISSAPIntegrator._logger.Error("OID :: " + sourceRow.OID + " Getting ParentID");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <returns>Throws NotImplementedException</returns>
        public string GetParentID(DeleteFeat sourceRow)
        {
            GISSAPIntegrator._logger.Error("OID :: " + sourceRow.OID + " Getting ParentID");
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns Not Applicable SAP type
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>SAPType.NotApplicable</returns>
        public virtual SAPType GetSAPType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            SAPType sapType = SAPType.NotApplicable;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: NotApplicable");
            return sapType;
        }

        /// <summary>
        /// Returns Not Applicable SAP type
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>SAPType.NotApplicable</returns>
        public virtual SAPType GetSAPType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            SAPType sapType = SAPType.NotApplicable;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: NotApplicable");
            return sapType;
        }

        /// <summary>
        /// Returns Not Applicable SAP type
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>SAPType.NotApplicable</returns>
        public virtual SAPType GetSAPType(DeleteFeat sourceRow,ITable FCName, ChangeType changeType)
        {
            SAPType sapType = SAPType.NotApplicable;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: NotApplicable");
            return sapType;
        }

        /// <summary>
        /// Subclasses will use asset status in source or edit, and target versions, along with ArcMap edit type
        /// to infer its ActionType.
        /// </summary>
        /// <param name="sourceRow">The row in source or edit version</param>
        /// <param name="targetRow">The row in target or parent version</param>
        /// <param name="changeType">GIS edit action: insert/delete/update</param>
        /// <returns></returns>
        public virtual ActionType GetActionType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: NotApplicable");
            return ActionType.NotApplicable;
        }

        /// <summary>
        /// Subclasses will use asset status in source or edit, and target versions, along with ArcMap edit type
        /// to infer its ActionType.
        /// </summary>
        /// <param name="sourceRow">The row in source or edit version</param>
        /// <param name="targetRow">The row in target or parent version</param>
        /// <param name="changeType">GIS edit action: insert/delete/update</param>
        /// <returns></returns>
        public virtual ActionType GetActionType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: NotApplicable");
            return ActionType.NotApplicable;
        }

        /// <summary>
        /// Subclasses will use asset status in source or edit, and target versions, along with ArcMap edit type
        /// to infer its ActionType.
        /// </summary>
        /// <param name="sourceRow">The row in source or edit version</param>
        /// <param name="targetRow">The row in target or parent version</param>
        /// <param name="changeType">GIS edit action: insert/delete/update</param>
        /// <returns></returns>
        public virtual ActionType GetActionType(DeleteFeat sourceRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: NotApplicable");

            return ActionType.NotApplicable;
        }

        #endregion
    }


}
