using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Data;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Interfaces.Integration.Framework.Exceptions;
using PGE.Interfaces.Integration.Framework.Utilities;

using System.Runtime.InteropServices; //IVARA
using PGE.Common.Delivery.Framework.FeederManager;
using Miner.Geodatabase;
using Miner.Interop;
using System.Text.RegularExpressions;
using Diagnostics = PGE.Common.Delivery.Diagnostics;
using PGE.Common.ChangeDetectionAPI;
using ESRI.ArcGIS.Geometry;
using Oracle.DataAccess.Client;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Generic IRowTransformer that contains common functionality other IRowTransformers can use.
    /// </summary>
    public class BaseRowTransformer : IRowTransformer<IRow>
    {
        //private static log4net.ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        public static bool UseFeederManager20Fields = true;
        //private static log4net.ILog _logger = LogManager.GetLogger("PGE.Interfaces.SAP");

        protected Dictionary<int, Dictionary<string, string>> fieldSequenceAndNameValue;
        public StringBuilder _errorMessage = new StringBuilder();

        /// <summary>
        /// The SystemMapper that this IRowTransformer belongs to
        /// </summary>
        protected SystemMapper _systemMapper;
        /// <summary>
        /// The TrackClass this IRowTransformer belongs to
        /// </summary>
        protected TrackedClass _trackedClass;
        /// <summary>
        /// Default Constructor
        /// </summary>
        public BaseRowTransformer()
        {
        }
        /// <summary>
        /// Pass references to the parent SystemMapper and TrackedClass
        /// </summary>
        /// <param name="sysMapper">The parent SystemMapper</param>
        /// <param name="trackedClass">The parent TrackedClass</param>
        public virtual void Initialize(SystemMapper sysMapper, TrackedClass trackedClass)
        {
            this._systemMapper = sysMapper;
            this._trackedClass = trackedClass;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public virtual List<IRowData> ProcessRow(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            _errorMessage = new StringBuilder();
            List<IRowData> rowDataList = new List<IRowData>();

            IRow activeRow = sourceRow;
            if (changeType == ChangeType.Delete)
            {
                activeRow = targetRow;
            }

            // verify if the row has valid subtype
            // for example, a valid and changed TransformerUnit may be related to a Transformer with an out-of-scope subtype, or vice versa (in theory)
            // this may cause more processing time but can be minimized when configured properly (leave Subtypes empty whenever possible)
            if (Subtypes.Count > 0)
            {
                IRowSubtypes rowSubtypes = (IRowSubtypes)activeRow;
                int subtypeCD = rowSubtypes.SubtypeCode;
                if (Subtypes.Contains(Convert.ToString(subtypeCD)) == false)
                {
                    _errorMessage.AppendLine(activeRow.OID + " Not Valid Subtype : " + subtypeCD + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                    return rowDataList;
                }
            }

            ////return empty data if customer owned, e.g, process and sent to SAP only PGE owned assets
            //string customerOwned = GetCustomerOwnedValue(activeRow);
            //if (customerOwned == "Y")
            //{
            //    return rowDataList;
            //}

            Dictionary<int, List<string>> objectIdAndFailedFields = null;
            if (_systemMapper.ObjectIDandFailedFieldsBySourceName.ContainsKey(_trackedClass.SourceClass))
            {
                objectIdAndFailedFields = _systemMapper.ObjectIDandFailedFieldsBySourceName[_trackedClass.SourceClass];
            }

            List<string> failedFields = null;

            IRowData rowData = new RowData();
            Dictionary<int, string> fieldSequenceAndValue = new Dictionary<int, string>();
            Dictionary<string, string> fieldNameValue = new Dictionary<string, string>();
            fieldSequenceAndNameValue = new Dictionary<int, Dictionary<string, string>>();

            foreach (MappedField mappedFld in Fields)
            {
                try
                {
                    FieldMapper fldMapper = mappedFld.FieldMapper;
                    IFieldTransformer<IRow> fldTransformer = fldMapper.FieldTransformer;
                    string fldNameUpper = mappedFld.OutName.ToUpper();
                    string mappedFldValue = "";

                    //To support feeder manager 2.0 we need to obtain circuit ids differently
                    if (changeType != ChangeType.Reprocess && (fldNameUpper == "CIRCUITID" || fldNameUpper == "CIRCUITID2") && UseFeederManager20Fields)
                    {
                        //Workaround for deleted features -- Failing to get Circuit IDs
                        try
                        {
                            string[] circuitIDs = FeederManager2.GetCircuitIDs(activeRow);
                            if (circuitIDs != null)
                            {
                                if (circuitIDs.Length > 0 && fldNameUpper == "CIRCUITID") { mappedFldValue = circuitIDs[0]; }
                                else if (circuitIDs.Length > 1) { mappedFldValue = circuitIDs[1]; }
                                else { mappedFldValue = ""; }
                            }
                        }
                        catch 
                        {
                            _errorMessage.AppendLine(activeRow.OID + " Not Valid CircuitID " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            mappedFldValue = "";
                        }
                    }
                    else
                    {
                        mappedFldValue = fldTransformer.GetValue<string>(activeRow);
                    }
                    //commented below code for  EDGIS Re-Arch project-v1t8
                    // fieldSequenceAndValue.Add(mappedFld.Sequence, mappedFldValue);

                    //V3SF - CD API (EDGISREARC-1452) - Added - to resolve ',newline and return charactor in json string
                    //if (!string.IsNullOrWhiteSpace(mappedFldValue))
                    //    mappedFldValue = mappedFldValue.Replace("'", "\\'").Replace("\n", "").Replace("\r", "");
                    //Added below to fix error key already exist for EDGIS Re-Arch project-v1t8
                    if (!fieldSequenceAndValue.ContainsKey(mappedFld.Sequence))
                    {
                        fieldSequenceAndValue.Add(mappedFld.Sequence, mappedFldValue);
                    }

                    //Added below for key value formate for EDGIS Re-Arch project-v1t8
                    //            {"KeyField" : "AutoBooster","KeyValue" : "5" },
                  //  string keyValue = "{" + "KeyField" + ":" + fldNameUpper + "," + "KeyValue" + ":" + mappedFldValue + "}";
                  //  fieldSequenceAndValue.Add(mappedFld.Sequence, keyValue);

                    #region edgis Rearch
                    //Below code is added for EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
                    if (!fieldNameValue.ContainsKey(mappedFld.OutName))
                    {
                        fieldNameValue.Add(mappedFld.OutName, mappedFldValue);
                    }
                    //if (!fieldSequenceAndNameValue.ContainsKey(mappedFld.Sequence))
                    //{
                    //    fieldSequenceAndNameValue.Add(mappedFld.Sequence, fieldNameValue);
                    //}

                    #endregion 
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                catch (Exception e)
                {
                    rowData.Valid = false;

                    if (e is InvalidConfigurationException)
                    {
                        rowData.ErrorMessage += e.Message + "\r\n";
                        throw e;
                    }

                    string msg = string.Format("Field with OutName: {0} and Sequence: {1} of the row with ObjectID: {2} and ChangeType: {3} of Class: {4} failed.",
                        mappedFld.OutName, mappedFld.Sequence, activeRow.OID, changeType.ToString(), _trackedClass.QualifiedSourceClass);
                    rowData.ErrorMessage += string.Format("Field {0} (Sequence {1}) failed due to {2}", mappedFld.OutName, mappedFld.Sequence, e.Message + "\r\n");

                    _errorMessage.AppendLine(activeRow.OID + string.Format(" Field with OutName: {0} and Sequence: {1} of the row with ObjectID: {2} and ChangeType: {3} of Class: {4} failed.",
                        mappedFld.OutName, mappedFld.Sequence, activeRow.OID, changeType.ToString(), _trackedClass.QualifiedSourceClass) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");


                    Exception fieldTransformationException = null;
                    if (_logger.IsDebugEnabled)
                    {
                        fieldTransformationException = new Exception(msg + e.StackTrace, e);
                    }
                    else
                    {
                        fieldTransformationException = new Exception(msg, e);
                    }

                    _logger.Error("Error processing row.", fieldTransformationException);

                    if (objectIdAndFailedFields == null)
                    {
                        objectIdAndFailedFields = new Dictionary<int, List<string>>();
                        _systemMapper.ObjectIDandFailedFieldsBySourceName[_trackedClass.SourceClass] = objectIdAndFailedFields;
                    }

                    if (failedFields == null)
                    {
                        if (objectIdAndFailedFields.ContainsKey(activeRow.OID))
                        {
                            failedFields = objectIdAndFailedFields[activeRow.OID];
                        }
                        else
                        {
                            failedFields = new List<string>();
                            objectIdAndFailedFields[activeRow.OID] = failedFields;
                        }
                    }

                    string failedField = mappedFld.OutName + " " + mappedFld.Sequence + " : " + e.Message;
                    if (failedFields.Contains(failedField) == false)
                    {
                        failedFields.Add(failedField);
                    }

                    // maybe should use a special string indicating the field value is wrong instead of the composed error message
                    fieldSequenceAndValue.Add(mappedFld.Sequence, msg);

                    // this stops the Data Collector, and thus also Post

                    //We need not stop the post here. We will collect all the issues with the edits and stop the post in the GISSAPIntegrator Class. - Ravi J
                    //throw fieldTransformationException;
                }
            }

            rowData.FieldValues = fieldSequenceAndValue;
            //Below code is added for EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
            rowData.FieldKeyValue =fieldNameValue ;

            // if feature, populate OID and FCID for debugging
            if (activeRow is IFeature)
            {
                ITable tbl = activeRow.Table;
                IObjectClass objCls = (IObjectClass)tbl;
                rowData.FCID = objCls.ObjectClassID;
                rowData.FeatureClassName = ((IDataset)objCls).BrowseName;
                rowData.OID = activeRow.OID;
            }

            //Process any potential adjustments as configured
            PostProcessMethods(ref rowData, activeRow);

            rowDataList.Add(rowData);
            GC.Collect();

            return rowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <param name="_errorMessage"></param>
        /// <returns>A list of IRowData</returns>
        public virtual List<IRowData> ProcessRow(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            _errorMessage = new StringBuilder();
            List<IRowData> rowDataList = new List<IRowData>();

            IRow activeRow = sourceRow;
            //if (changeType == ChangeType.Delete)
            //{
            //    activeRow = targetRow;
            //}

            // verify if the row has valid subtype
            // for example, a valid and changed TransformerUnit may be related to a Transformer with an out-of-scope subtype, or vice versa (in theory)
            // this may cause more processing time but can be minimized when configured properly (leave Subtypes empty whenever possible)
            if (Subtypes.Count > 0)
            {
                IRowSubtypes rowSubtypes = (IRowSubtypes)activeRow;
                int subtypeCD = rowSubtypes.SubtypeCode;
                if (Subtypes.Contains(Convert.ToString(subtypeCD)) == false)
                {
                    _errorMessage.AppendLine(activeRow.OID + " Not Valid Subtype : " + subtypeCD + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    return rowDataList;
                }
            }

            ////return empty data if customer owned, e.g, process and sent to SAP only PGE owned assets
            //string customerOwned = GetCustomerOwnedValue(activeRow);
            //if (customerOwned == "Y")
            //{
            //    return rowDataList;
            //}

            Dictionary<int, List<string>> objectIdAndFailedFields = null;
            if (_systemMapper.ObjectIDandFailedFieldsBySourceName.ContainsKey(_trackedClass.SourceClass))
            {
                objectIdAndFailedFields = _systemMapper.ObjectIDandFailedFieldsBySourceName[_trackedClass.SourceClass];
            }

            List<string> failedFields = null;

            IRowData rowData = new RowData();
            Dictionary<int, string> fieldSequenceAndValue = new Dictionary<int, string>();
            Dictionary<string, string> fieldNameValue = new Dictionary<string, string>();
            fieldSequenceAndNameValue = new Dictionary<int, Dictionary<string, string>>();

            foreach (MappedField mappedFld in Fields)
            {
                try
                {
                    FieldMapper fldMapper = mappedFld.FieldMapper;
                    IFieldTransformer<IRow> fldTransformer = fldMapper.FieldTransformer;
                    string fldNameUpper = mappedFld.OutName.ToUpper();
                    string mappedFldValue = "";

                    //To support feeder manager 2.0 we need to obtain circuit ids differently
                    if (changeType != ChangeType.Reprocess && (fldNameUpper == "CIRCUITID" || fldNameUpper == "CIRCUITID2") && UseFeederManager20Fields)
                    {
                        //Workaround for deleted features -- Failing to get Circuit IDs
                        try
                        {
                            string[] circuitIDs = FeederManager2.GetCircuitIDs(activeRow);
                            if (circuitIDs != null)
                            {
                                if (circuitIDs.Length > 0 && fldNameUpper == "CIRCUITID") { mappedFldValue = circuitIDs[0]; }
                                else if (circuitIDs.Length > 1) { mappedFldValue = circuitIDs[1]; }
                                else { mappedFldValue = ""; }
                            }
                        }
                        catch 
                        {
                            _errorMessage.AppendLine(activeRow.OID + " Not Valid CircuitID " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            
                            mappedFldValue = ""; 
                        }
                    }
                    else
                    {
                        mappedFldValue = fldTransformer.GetValue<string>(activeRow);
                    }
                    //commented below code for  EDGIS Re-Arch project-v1t8
                    // fieldSequenceAndValue.Add(mappedFld.Sequence, mappedFldValue);

                    //V3SF - CD API (EDGISREARC-1452) - Added - to resolve ',newline and return charactor in json string
                    //if (!string.IsNullOrWhiteSpace(mappedFldValue))
                    //    mappedFldValue = mappedFldValue.Replace("'", "\\'").Replace("\n", "").Replace("\r", "");
                    //Added below to fix error key already exist for EDGIS Re-Arch project-v1t8
                    if (!fieldSequenceAndValue.ContainsKey(mappedFld.Sequence))
                    {
                        fieldSequenceAndValue.Add(mappedFld.Sequence, mappedFldValue);
                    }

                    //Added below for key value formate for EDGIS Re-Arch project-v1t8
                    //            {"KeyField" : "AutoBooster","KeyValue" : "5" },
                    //  string keyValue = "{" + "KeyField" + ":" + fldNameUpper + "," + "KeyValue" + ":" + mappedFldValue + "}";
                    //  fieldSequenceAndValue.Add(mappedFld.Sequence, keyValue);

                    #region edgis Rearch
                    //Below code is added for EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
                    if (!fieldNameValue.ContainsKey(mappedFld.OutName))
                    {
                        fieldNameValue.Add(mappedFld.OutName, mappedFldValue);
                    }
                    //if (!fieldSequenceAndNameValue.ContainsKey(mappedFld.Sequence))
                    //{
                    //    fieldSequenceAndNameValue.Add(mappedFld.Sequence, fieldNameValue);
                    //}

                    #endregion 
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                catch (Exception e)
                {
                    rowData.Valid = false;

                    if (e is InvalidConfigurationException)
                    {
                        rowData.ErrorMessage += e.Message + "\r\n";
                        throw e;
                    }

                    string msg = string.Format("Field with OutName: {0} and Sequence: {1} of the row with ObjectID: {2} and ChangeType: {3} of Class: {4} failed.",
                        mappedFld.OutName, mappedFld.Sequence, activeRow.OID, changeType.ToString(), _trackedClass.QualifiedSourceClass);
                    rowData.ErrorMessage += string.Format("Field {0} (Sequence {1}) failed due to {2}", mappedFld.OutName, mappedFld.Sequence, e.Message + "\r\n");

                    _errorMessage.AppendLine(activeRow.OID + string.Format(" Field with OutName: {0} and Sequence: {1} of the row with ObjectID: {2} and ChangeType: {3} of Class: {4} failed.",
                        mappedFld.OutName, mappedFld.Sequence, activeRow.OID, changeType.ToString(), _trackedClass.QualifiedSourceClass) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                    Exception fieldTransformationException = null;
                    if (_logger.IsDebugEnabled)
                    {
                        fieldTransformationException = new Exception(msg + e.StackTrace, e);
                    }
                    else
                    {
                        fieldTransformationException = new Exception(msg, e);
                    }

                    _logger.Error("Error processing row.", fieldTransformationException);

                    if (objectIdAndFailedFields == null)
                    {
                        objectIdAndFailedFields = new Dictionary<int, List<string>>();
                        _systemMapper.ObjectIDandFailedFieldsBySourceName[_trackedClass.SourceClass] = objectIdAndFailedFields;
                    }

                    if (failedFields == null)
                    {
                        if (objectIdAndFailedFields.ContainsKey(activeRow.OID))
                        {
                            failedFields = objectIdAndFailedFields[activeRow.OID];
                        }
                        else
                        {
                            failedFields = new List<string>();
                            objectIdAndFailedFields[activeRow.OID] = failedFields;
                        }
                    }

                    string failedField = mappedFld.OutName + " " + mappedFld.Sequence + " : " + e.Message;
                    if (failedFields.Contains(failedField) == false)
                    {
                        failedFields.Add(failedField);
                    }

                    // maybe should use a special string indicating the field value is wrong instead of the composed error message
                    fieldSequenceAndValue.Add(mappedFld.Sequence, msg);

                    // this stops the Data Collector, and thus also Post

                    //We need not stop the post here. We will collect all the issues with the edits and stop the post in the GISSAPIntegrator Class. - Ravi J
                    //throw fieldTransformationException;
                }
            }

            rowData.FieldValues = fieldSequenceAndValue;
            //Below code is added for EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
            rowData.FieldKeyValue = fieldNameValue;

            // if feature, populate OID and FCID for debugging
            if (activeRow is IFeature)
            {
                ITable tbl = activeRow.Table;
                IObjectClass objCls = (IObjectClass)tbl;
                rowData.FCID = objCls.ObjectClassID;
                rowData.FeatureClassName = ((IDataset)objCls).BrowseName;
                rowData.OID = activeRow.OID;
            }

            //Process any potential adjustments as configured
            PostProcessMethods(ref rowData, activeRow);

            rowDataList.Add(rowData);
            GC.Collect();

            return rowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public virtual List<IRowData> ProcessRow(DeleteFeat sourceRow, ChangeType changeType)
        {
            _errorMessage = new StringBuilder();
            List<IRowData> rowDataList = new List<IRowData>();
            
            DeleteFeat activeRow = sourceRow;
            //if (changeType == ChangeType.Delete)
            //{
            //    activeRow = targetRow;
            //}

            // verify if the row has valid subtype
            // for example, a valid and changed TransformerUnit may be related to a Transformer with an out-of-scope subtype, or vice versa (in theory)
            // this may cause more processing time but can be minimized when configured properly (leave Subtypes empty whenever possible)
            if (Subtypes.Count > 0)
            {
                //IRowSubtypes rowSubtypes = (IRowSubtypes)activeRow;
                if (sourceRow.fields_Old.ContainsKey("SUBTYPECD"))
                    {
                    int subtypeCD = Convert.ToInt32(activeRow.fields_Old["SUBTYPECD"]);
                    if (Subtypes.Contains(Convert.ToString(subtypeCD)) == false)
                    {
                        _errorMessage.AppendLine(activeRow.OID + " Not Valid Subtype : " + subtypeCD + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        return rowDataList;
                    }
                }
            }

            ////return empty data if customer owned, e.g, process and sent to SAP only PGE owned assets
            //string customerOwned = GetCustomerOwnedValue(activeRow);
            //if (customerOwned == "Y")
            //{
            //    return rowDataList;
            //}

            Dictionary<int, List<string>> objectIdAndFailedFields = null;
            if (_systemMapper.ObjectIDandFailedFieldsBySourceName.ContainsKey(_trackedClass.SourceClass))
            {
                objectIdAndFailedFields = _systemMapper.ObjectIDandFailedFieldsBySourceName[_trackedClass.SourceClass];
            }

            List<string> failedFields = null;

            IRowData rowData = new RowData();
            Dictionary<int, string> fieldSequenceAndValue = new Dictionary<int, string>();
            Dictionary<string, string> fieldNameValue = new Dictionary<string, string>();
            fieldSequenceAndNameValue = new Dictionary<int, Dictionary<string, string>>();

            foreach (MappedField mappedFld in Fields)
            {
                try
                {
                    FieldMapper fldMapper = mappedFld.FieldMapper;
                    IFieldTransformer<IRow> fldTransformer = fldMapper.FieldTransformer;
                    string fldNameUpper = mappedFld.OutName.ToUpper();
                    string mappedFldValue = "";

                    //To support feeder manager 2.0 we need to obtain circuit ids differently
                    if (changeType != ChangeType.Reprocess && (fldNameUpper == "CIRCUITID" || fldNameUpper == "CIRCUITID2") && UseFeederManager20Fields)
                    {
                        //Workaround for deleted features -- Failing to get Circuit IDs
                        try
                        {
                            //string[] circuitIDs = FeederManager2.GetCircuitIDs(activeRow);
                            if (activeRow.fields_Old.ContainsKey("CIRCUITID"))
                                mappedFldValue = activeRow.fields_Old["CIRCUITID"];
                            else
                                mappedFldValue = "";
                            //if (circuitIDs != null)
                            //{
                            //    if (circuitIDs.Length > 0 && fldNameUpper == "CIRCUITID") { mappedFldValue = circuitIDs[0]; }
                            //    else if (circuitIDs.Length > 1) { mappedFldValue = circuitIDs[1]; }
                            //    else { mappedFldValue = ""; }
                            //}
                        }
                        catch
                        {
                            _errorMessage.AppendLine(activeRow.OID + " Not Valid CircuitID " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                            mappedFldValue = "";
                        }
                    }
                    else
                    {
                        try
                        {
                            if (_trackedClass.SourceTable == null)
                                _trackedClass.SourceTable = _trackedClass.TargetTable;
                            if (_trackedClass.SourceTable == null)
                                _trackedClass.SourceTable = activeRow.Table;
                            mappedFldValue = fldTransformer.GetValue<string>(activeRow, _trackedClass.SourceTable);
                        }
                        catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                        catch(Exception ex)
                        {
                            _errorMessage.AppendLine(activeRow.OID + " Error processing Field Transformer : " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            //V3SF - Related Record - Default 0000
                            mappedFldValue = "0000";
                        }
                        //if(activeRow.fields_Old.ContainsKey(fldNameUpper))
                        //    mappedFldValue = activeRow.fields_Old[fldNameUpper];
                        //else 
                        //    mappedFldValue = "";
                    }
                    //commented below code for  EDGIS Re-Arch project-v1t8
                    // fieldSequenceAndValue.Add(mappedFld.Sequence, mappedFldValue);

                    //V3SF - CD API (EDGISREARC-1452) - Added - to resolve ',newline and return charactor in json string
                    //if (!string.IsNullOrWhiteSpace(mappedFldValue))
                    //    mappedFldValue = mappedFldValue.Replace("'", "\\'").Replace("\n", "").Replace("\r", "");
                    //Added below to fix error key already exist for EDGIS Re-Arch project-v1t8
                    if (!fieldSequenceAndValue.ContainsKey(mappedFld.Sequence))
                    {
                        fieldSequenceAndValue.Add(mappedFld.Sequence, mappedFldValue);
                    }

                    //Added below for key value formate for EDGIS Re-Arch project-v1t8
                    //            {"KeyField" : "AutoBooster","KeyValue" : "5" },
                    //  string keyValue = "{" + "KeyField" + ":" + fldNameUpper + "," + "KeyValue" + ":" + mappedFldValue + "}";
                    //  fieldSequenceAndValue.Add(mappedFld.Sequence, keyValue);

                    #region edgis Rearch
                    //Below code is added for EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
                    if (!fieldNameValue.ContainsKey(mappedFld.OutName))
                    {
                        fieldNameValue.Add(mappedFld.OutName, mappedFldValue);
                    }
                    //if (!fieldSequenceAndNameValue.ContainsKey(mappedFld.Sequence))
                    //{
                    //    fieldSequenceAndNameValue.Add(mappedFld.Sequence, fieldNameValue);
                    //}

                    #endregion 
                }
                catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                catch (Exception e)
                {
                    rowData.Valid = false;

                    if (e is InvalidConfigurationException)
                    {
                        rowData.ErrorMessage += e.Message + "\r\n";
                        throw e;
                    }

                    string msg = string.Format("Field with OutName: {0} and Sequence: {1} of the row with ObjectID: {2} and ChangeType: {3} of Class: {4} failed.",
                        mappedFld.OutName, mappedFld.Sequence, activeRow.OID, changeType.ToString(), _trackedClass.QualifiedSourceClass);
                    rowData.ErrorMessage += string.Format("Field {0} (Sequence {1}) failed due to {2}", mappedFld.OutName, mappedFld.Sequence, e.Message + "\r\n");

                    _errorMessage.AppendLine(activeRow.OID + string.Format(" Field with OutName: {0} and Sequence: {1} of the row with ObjectID: {2} and ChangeType: {3} of Class: {4} failed.",
                        mappedFld.OutName, mappedFld.Sequence, activeRow.OID, changeType.ToString(), _trackedClass.QualifiedSourceClass) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                    Exception fieldTransformationException = null;
                    if (_logger.IsDebugEnabled)
                    {
                        fieldTransformationException = new Exception(msg + e.StackTrace, e);
                    }
                    else
                    {
                        fieldTransformationException = new Exception(msg, e);
                    }

                    _logger.Error("Error processing row.", fieldTransformationException);

                    if (objectIdAndFailedFields == null)
                    {
                        objectIdAndFailedFields = new Dictionary<int, List<string>>();
                        _systemMapper.ObjectIDandFailedFieldsBySourceName[_trackedClass.SourceClass] = objectIdAndFailedFields;
                    }

                    if (failedFields == null)
                    {
                        if (objectIdAndFailedFields.ContainsKey(activeRow.OID))
                        {
                            failedFields = objectIdAndFailedFields[activeRow.OID];
                        }
                        else
                        {
                            failedFields = new List<string>();
                            objectIdAndFailedFields[activeRow.OID] = failedFields;
                        }
                    }

                    string failedField = mappedFld.OutName + " " + mappedFld.Sequence + " : " + e.Message;
                    if (failedFields.Contains(failedField) == false)
                    {
                        failedFields.Add(failedField);
                    }

                    // maybe should use a special string indicating the field value is wrong instead of the composed error message
                    fieldSequenceAndValue.Add(mappedFld.Sequence, msg);

                    // this stops the Data Collector, and thus also Post

                    //We need not stop the post here. We will collect all the issues with the edits and stop the post in the GISSAPIntegrator Class. - Ravi J
                    //throw fieldTransformationException;
                }
            }

            rowData.FieldValues = fieldSequenceAndValue;
            //Below code is added for EDGIS Rearch Project 2021 edgisrearch-374 -v1t8
            rowData.FieldKeyValue = fieldNameValue;

            // if feature, populate OID and FCID for debugging
            //if (activeRow is IFeature)
            //{
            //    //ITable tbl = activeRow.Table;
            //    IObjectClass objCls = (IObjectClass)tbl;
            //    rowData.FCID = objCls.ObjectClassID;
            //    rowData.FeatureClassName = ((IDataset)objCls).BrowseName;
            //    rowData.OID = activeRow.OID;
            //}
            if(activeRow.Table!=null)
            {
                if (activeRow.Table is IFeatureClass)
                {
                    ITable tbl = activeRow.Table;
                    IObjectClass objCls = (IObjectClass)tbl;
                    rowData.FCID = objCls.ObjectClassID;
                    rowData.FeatureClassName = ((IDataset)objCls).BrowseName;
                    rowData.OID = activeRow.OID;
                }
            }

            //Process any potential adjustments as configured
            PostProcessMethods(ref rowData, activeRow);

            rowDataList.Add(rowData);
            GC.Collect();

            return rowDataList;
        }

        private void PostProcessMethods(ref IRowData rowData, IRow activeRow)
        {
            Settings settings = _trackedClass.Settings;
            if (settings != null)
            {
                string[] postProcessingMethods = Regex.Split(settings["PostProcessingMethods"], ";");

                foreach (string postProcessingMethod in postProcessingMethods)
                {
                    string[] arguments = Regex.Split(postProcessingMethod, ",");
                    if (arguments[0].ToUpper() == "ClearParentGuidField".ToUpper())
                    {
                        ClearParentGuidField(ref rowData, activeRow, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
                    }
                }
            }
        }

        private void PostProcessMethods(ref IRowData rowData, DeleteFeat activeRow)
        {
            Settings settings = _trackedClass.Settings;
            if (settings != null)
            {
                string[] postProcessingMethods = Regex.Split(settings["PostProcessingMethods"], ";");

                foreach (string postProcessingMethod in postProcessingMethods)
                {
                    string[] arguments = Regex.Split(postProcessingMethod, ",");
                    if (arguments[0].ToUpper() == "ClearParentGuidField".ToUpper())
                    {
                        ClearParentGuidField(ref rowData, activeRow, arguments[1], arguments[2], arguments[3], arguments[4], arguments[5]);
                    }
                }
            }
        }

        private void ClearParentGuidField(ref IRowData rowData, IRow activeRow, string parentGuidFieldName, string relatedClassModelName, string relatedClassField,
            string fieldSequenceToNull, string validWhereClauseCriteria)
        {
            bool clearParentGuidValue = true;

            //Find the parent guid field specified
            int parentGuidFieldIdx = activeRow.Table.Fields.FindField(parentGuidFieldName);
            if (parentGuidFieldIdx < 0) { throw new Exception("Unable to find specified field '" + parentGuidFieldName + "' for feature class '" + ((IDataset)activeRow.Table).BrowseName + "''"); }
            string parentGuidValue = activeRow.get_Value(parentGuidFieldIdx).ToString();

            //Find the feature classes that match the specified model name
            IFeatureWorkspace featWorkspace = ((IDataset)_trackedClass.SourceTable).Workspace as IFeatureWorkspace;
            IMMEnumObjectClass overheadStructures = ModelNameManager.Instance.ObjectClassesFromModelNameWS(featWorkspace as IWorkspace, relatedClassModelName);
            IObjectClass overheadStructureFeatClass = overheadStructures.Next();

            //Proces all of the feature classes found and look for any related features that match the specified criteria.
            //If a match is found, then the parent guid field is not nulled out
            while (overheadStructureFeatClass != null)
            {
                int relatedClassFieldIdx = overheadStructureFeatClass.Fields.FindField(relatedClassField);
                if (relatedClassFieldIdx > -1)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = string.Format("{0}='{1}' AND ({2})", relatedClassField, parentGuidValue, validWhereClauseCriteria);
                    int rowCount = ((ITable)overheadStructureFeatClass).RowCount(qf);
                    if (rowCount > 0) { clearParentGuidValue = false; }
                    while (Marshal.ReleaseComObject(qf) > 0) { }
                }
                else
                {
                    throw new Exception("Unable to find specified field '" + relatedClassField + "' for feature class '" + ((IDataset)overheadStructureFeatClass).BrowseName + "''");
                }

                if (overheadStructureFeatClass != null) { Marshal.ReleaseComObject(overheadStructureFeatClass); }
                overheadStructureFeatClass = overheadStructures.Next();
            }
            if (overheadStructureFeatClass != null) { Marshal.ReleaseComObject(overheadStructureFeatClass); }
            if (overheadStructures != null) { Marshal.ReleaseComObject(overheadStructures); }

            if (clearParentGuidValue)
            {
                int fieldSequenceValue = -1;
                if (!Int32.TryParse(fieldSequenceToNull, out fieldSequenceValue) || !rowData.FieldValues.ContainsKey(fieldSequenceValue))
                {
                    throw new Exception("Invalid field sequence value specified to null out '" + fieldSequenceToNull + "'");
                }

                //Null out the parent guid field
                rowData.FieldValues[fieldSequenceValue] = "";
            }
        }

        private void ClearParentGuidField(ref IRowData rowData, DeleteFeat activeRow, string parentGuidFieldName, string relatedClassModelName, string relatedClassField,
            string fieldSequenceToNull, string validWhereClauseCriteria)
        {
            bool clearParentGuidValue = true;

            //Find the parent guid field specified
            //int parentGuidFieldIdx = activeRow.Table.Fields.FindField(parentGuidFieldName);
            //if (parentGuidFieldIdx < 0) { throw new Exception("Unable to find specified field '" + parentGuidFieldName + "' for feature class '" + ((IDataset)activeRow.Table).BrowseName + "''"); }
            //string parentGuidValue = activeRow.get_Value(parentGuidFieldIdx).ToString();

            string parentGuidValue = string.Empty;
            //dynamic activerow = activeRow1;
            if (activeRow.fields_Old.ContainsKey(parentGuidFieldName))
                parentGuidValue = activeRow.fields_Old[parentGuidFieldName];

            //Find the feature classes that match the specified model name
            IFeatureWorkspace featWorkspace = ((IDataset)_trackedClass.SourceTable).Workspace as IFeatureWorkspace;
            IMMEnumObjectClass overheadStructures = ModelNameManager.Instance.ObjectClassesFromModelNameWS(featWorkspace as IWorkspace, relatedClassModelName);
            IObjectClass overheadStructureFeatClass = overheadStructures.Next();

            //Proces all of the feature classes found and look for any related features that match the specified criteria.
            //If a match is found, then the parent guid field is not nulled out
            while (overheadStructureFeatClass != null)
            {
                int relatedClassFieldIdx = overheadStructureFeatClass.Fields.FindField(relatedClassField);
                if (relatedClassFieldIdx > -1)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = string.Format("{0}='{1}' AND ({2})", relatedClassField, parentGuidValue, validWhereClauseCriteria);
                    int rowCount = ((ITable)overheadStructureFeatClass).RowCount(qf);
                    if (rowCount > 0) { clearParentGuidValue = false; }
                    while (Marshal.ReleaseComObject(qf) > 0) { }
                }
                else
                {
                    throw new Exception("Unable to find specified field '" + relatedClassField + "' for feature class '" + ((IDataset)overheadStructureFeatClass).BrowseName + "''");
                }

                if (overheadStructureFeatClass != null) { Marshal.ReleaseComObject(overheadStructureFeatClass); }
                overheadStructureFeatClass = overheadStructures.Next();
            }
            if (overheadStructureFeatClass != null) { Marshal.ReleaseComObject(overheadStructureFeatClass); }
            if (overheadStructures != null) { Marshal.ReleaseComObject(overheadStructures); }

            if (clearParentGuidValue)
            {
                int fieldSequenceValue = -1;
                if (!Int32.TryParse(fieldSequenceToNull, out fieldSequenceValue) || !rowData.FieldValues.ContainsKey(fieldSequenceValue))
                {
                    throw new Exception("Invalid field sequence value specified to null out '" + fieldSequenceToNull + "'");
                }

                //Null out the parent guid field
                rowData.FieldValues[fieldSequenceValue] = "";
            }
        }

        #region IRowTransformer<IRow> Members
        /// <summary>
        /// List of MappedField objects that contain field information and the transformers for each field
        /// </summary>
        public List<MappedField> Fields
        {
            get
            {
                return _trackedClass.Fields;
            }
        }
        /// <summary>
        /// Get a list of the subtypes the parent TrackedClass operates on
        /// </summary>
        public List<string> Subtypes
        {
            get
            {
                if (!string.IsNullOrEmpty(_trackedClass.Subtypes))
                {
                    return new List<string>(_trackedClass.Subtypes.Split(",".ToCharArray()));
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        /// <summary>
        /// The name of the class this IRowTransformer operates on
        /// </summary>
        public string SourceClass
        {
            get
            {
                return _trackedClass.SourceClass;
            }
        }
        /// <summary>
        /// Not used
        /// </summary>
        public List<string> SupportClasses
        {
            get
            {
                if (!string.IsNullOrEmpty(_trackedClass.SupportClasses))
                {
                    return new List<string>(_trackedClass.SupportClasses.Split(",".ToCharArray()));
                }
                else
                {
                    return new List<string>();
                }
            }
        }
        /// <summary>
        /// The name of the Class in the external system
        /// </summary>
        public string OutName
        {
            get
            {
                return _trackedClass.OutName;
            }
        }

        //V3SF (28-Mar-2022) Added Additional Parameter to display Skipping Record in Staging
        //IVARA
        /// <summary>
        /// Validates the feature if it needs any special condition to satisfy. 
        /// For example network switch or network open point features
        /// </summary>
        /// <param name="sourceRow">Source row</param>
        /// <param name="errorMsg"></param>
        /// <returns>true or false</returns>
        public virtual bool IsValid(IRow sourceRow,out string errorMsg)
        {
            errorMsg = string.Empty;
            return true;
        }

        /// <summary>
        /// Validates the feature if it needs any special condition to satisfy. 
        /// For example network switch or network open point features
        /// </summary>
        /// <param name="sourceRow">Source row</param>
        /// <param name="errorMsg"></param>
        /// <returns>true or false</returns>
        public virtual bool IsValid(DeleteFeat sourceRow, out string errorMsg)
        {
            errorMsg = string.Empty;
            return true;
        }

        #endregion
        /// <summary>
        /// Get rows related to the edited row
        /// </summary>
        /// <param name="row">The edited row</param>
        /// <param name="relationshipName">The name of the relationship to use</param>
        /// <returns>A set of related rows</returns>
        protected ISet GetSourceRelatedObjects(IRow row, string relationshipName)
        {
            Dictionary<string, IRelationshipClass2> relationshipClassByName = _systemMapper.RelationshipClassByNameInEditVersion;
            IRelationshipClass2 sourceRelationshipClass = null;
            if (relationshipClassByName.ContainsKey(relationshipName))
            {
                sourceRelationshipClass = relationshipClassByName[relationshipName];
            }
            else
            {
                sourceRelationshipClass = GetRelationshipClass(row, relationshipName);
                // this cashed relationshipClass seems to cause errors to be thrown inconsistently when GetObjectsRelatedToObject
                //relationshipClassByName.Add(relationshipName, sourceRelationshipClass);
            }

            ISet relatedObjects = sourceRelationshipClass.GetObjectsRelatedToObject((IObject)row);

            return relatedObjects;
        }
        /// <summary>
        /// Get rows related to the original row
        /// </summary>
        /// <param name="row">The edited row</param>
        /// <param name="relationshipName">The name of the relationship to use</param>
        /// <returns>A set of related rows</returns>
        protected ISet GetTargetRelatedObjects(IRow row, string relationshipName)
        {
            Dictionary<string, IRelationshipClass2> relationshipClassByName = _systemMapper.RelationshipClassByNameInTargetVersion;
            IRelationshipClass2 targetRelationshipClass = null;
            if (relationshipClassByName.ContainsKey(relationshipName))
            {
                targetRelationshipClass = relationshipClassByName[relationshipName];
            }
            else
            {
                targetRelationshipClass = GetRelationshipClass(row, relationshipName);
                relationshipClassByName.Add(relationshipName, targetRelationshipClass);
            }

            ISet relatedObjects = targetRelationshipClass.GetObjectsRelatedToObject((IObject)row);

            return relatedObjects;
        }

        private IRelationshipClass2 GetRelationshipClass(IRow row, string relationshipName)
        {
            ITable table = row.Table;
            IDataset dataset = (IDataset)table;
            IWorkspace workspace = dataset.Workspace;
            IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
            IRelationshipClass2 relationshipClass = (IRelationshipClass2)featWorkspace.OpenRelationshipClass(relationshipName);
            if (relationshipClass == null)
            {
                throw new InvalidConfigurationException(string.Format("Relationshipclass {0} cannot be found for class {1}", relationshipName, ((IDataset)row.Table).Name));
            }
            return relationshipClass;
        }

        private static Dictionary<string, int> FieldNameToFieldIndexMap = new Dictionary<string, int>();
        public static int GetFieldIndex(IObjectClass objClass, string fieldName)
        {
            try
            {
                string searchString = ((IDataset)objClass).BrowseName + "-" + fieldName;
                return FieldNameToFieldIndexMap[searchString];
            }
            catch
            {
                string searchString = ((IDataset)objClass).BrowseName + "-" + fieldName;
                int fieldIndex = objClass.FindField(fieldName);
                FieldNameToFieldIndexMap.Add(searchString, fieldIndex);
                return fieldIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns> the value of the customerOwned field if exists, otherwise, return empty string</returns>
        protected virtual string GetCustomerOwnedValue(IRow row)
        {
            int fieldIndex = -1;

            ITable tbl = row.Table;
            fieldIndex = BaseRowTransformer.GetFieldIndex((IObjectClass)tbl, "CustomerOwned");


            if (fieldIndex == -1)
            {

                //check if it's on related object class table

                Settings settings = _trackedClass.Settings;
                string relationNameWithCustomerOwned = "";
                if (settings != null && !string.IsNullOrEmpty(settings["RelationshipNameWithCustomerOwned"]))
                {
                    relationNameWithCustomerOwned = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithCustomerOwned"];

                    //it's better to do another NULLorEmpty check but since RelationshipNameWithCustomerOwned and SourceClassNameWithCustomerOwned two config are set like a bundle, so skip it for now
                    string relatedSourceClassName = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["SourceClassNameWithCustomerOwned"];

                    IDataset dataset = (IDataset)tbl;
                    IWorkspace workspace = dataset.Workspace;
                    IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
                    ITable relatedTable = featWorkspace.OpenTable(relatedSourceClassName);

                    if (relatedTable != null)
                    {
                        int index = BaseRowTransformer.GetFieldIndex((IObjectClass)relatedTable, "CustomerOwned");
                        if (index != -1)
                        {
                            //get related object
                            IRelationshipClass2 relationshipClass = (IRelationshipClass2)featWorkspace.OpenRelationshipClass(relationNameWithCustomerOwned);
                            if (relationshipClass == null)
                            {
                                throw new InvalidConfigurationException(string.Format("Relationshipclass {0} cannot be found for class {1}", relationNameWithCustomerOwned, ((IDataset)row.Table).Name));
                            }
                            ISet relatedObjects = relationshipClass.GetObjectsRelatedToObject((IObject)row);
                            relatedObjects.Reset();
                            IObject relatedObject = (IObject)relatedObjects.Next();
                            if (relatedObject != null)
                            {
                                return relatedObject.get_Value((int)index).ToString();
                            }
                        }
                    }

                }

                _errorMessage.AppendLine(row.OID + " CustomerOwned not Found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                //CusomerOwned field not found
                return "";
            }
            return row.get_Value((int)fieldIndex).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns> the value of the customerOwned field if exists, otherwise, return empty string</returns>
        protected virtual string GetCustomerOwnedValue(UpdateFeat row)
        {
            int fieldIndex = -1;
            string retValue = string.Empty;

            ITable tbl = row.feature.Table;
            //fieldIndex = BaseRowTransformer.GetFieldIndex((IObjectClass)tbl, "CustomerOwned");


            //if (fieldIndex == -1)
            if (!row.fields_Old.ContainsKey("CustomerOwned".ToUpper()))
            {

                //check if it's on related object class table

                Settings settings = _trackedClass.Settings;
                string relationNameWithCustomerOwned = "";
                if (settings != null && !string.IsNullOrEmpty(settings["RelationshipNameWithCustomerOwned"]))
                {
                    relationNameWithCustomerOwned = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithCustomerOwned"];

                    //it's better to do another NULLorEmpty check but since RelationshipNameWithCustomerOwned and SourceClassNameWithCustomerOwned two config are set like a bundle, so skip it for now
                    string relatedSourceClassName = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["SourceClassNameWithCustomerOwned"];

                    IDataset dataset = (IDataset)tbl;
                    IWorkspace workspace = dataset.Workspace;
                    IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
                    ITable relatedTable = featWorkspace.OpenTable(relatedSourceClassName); 
                    List<string> relatedClasses = new List<string>();
                    Dictionary<string, List<UpdateFeat>> RelatedUpdatedFeat = new Dictionary<string, List<UpdateFeat>>();
                    UpdateFeat updateFeat = default;

                    if (relatedTable != null)
                    {
                        IRelationshipClass2 relationshipClass = (IRelationshipClass2)featWorkspace.OpenRelationshipClass(relationNameWithCustomerOwned);
                        if (relationshipClass == null)
                        {
                            throw new InvalidConfigurationException(string.Format("Relationshipclass {0} cannot be found for class {1}", relationNameWithCustomerOwned, ((IDataset)row.Table).Name));
                        }

                        relatedClasses.Add(relatedSourceClassName.ToUpper());

                        //Get Update feat from PGE_GDBM_AH_INFO Table
                        RelatedUpdatedFeat = GetRelatedRows(row.Table, row, relatedClasses, row.ChangeFeatures);

                        if (RelatedUpdatedFeat.ContainsKey(relatedSourceClassName.ToUpper()))
                            updateFeat = RelatedUpdatedFeat[relatedSourceClassName.ToUpper()].First();

                        if (updateFeat != null)
                        {
                            if (updateFeat.fields_Old.ContainsKey("CustomerOwned".ToUpper()))
                                return updateFeat.fields_Old["CustomerOwned".ToUpper()];
                        }
                    }

                }

                _errorMessage.AppendLine(row.OID + " CustomerOwned not Found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                //CusomerOwned field not found
                return "";
            }
            else
            {
                retValue = row.fields_Old["CustomerOwned".ToUpper()];
            }

            return retValue;// row.get_Value((int)fieldIndex).ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <returns> the value of the customerOwned field if exists, otherwise, return empty string</returns>
        protected virtual string GetCustomerOwnedValue(DeleteFeat row)
        {
            //int fieldIndex = -1;
            //dynamic activeRow = row;
            ITable tbl = row.Table;
            if (row.fields_Old.ContainsKey("CustomerOwned".ToUpper()))
            {
                return row.fields_Old["CustomerOwned".ToUpper()];
            }
            else
            {
                //check if it's on related object class table

                Settings settings = _trackedClass.Settings;
                string relationNameWithCustomerOwned = "";
                if (settings != null && !string.IsNullOrEmpty(settings["RelationshipNameWithCustomerOwned"]))
                {
                    relationNameWithCustomerOwned = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithCustomerOwned"];

                    //it's better to do another NULLorEmpty check but since RelationshipNameWithCustomerOwned and SourceClassNameWithCustomerOwned two config are set like a bundle, so skip it for now
                    string relatedSourceClassName = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["SourceClassNameWithCustomerOwned"];

                    IDataset dataset = (IDataset)tbl;
                    IWorkspace workspace = dataset.Workspace;
                    IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
                    ITable relatedTable = featWorkspace.OpenTable(relatedSourceClassName);
                    List<string> relatedClasses = new List<string>();
                    Dictionary<string, List<DeleteFeat>> RelatedDeletedFeat = new Dictionary<string, List<DeleteFeat>>();
                    DeleteFeat deleteFeat = default;

                    if (relatedTable != null)
                    {
                        IRelationshipClass2 relationshipClass = (IRelationshipClass2)featWorkspace.OpenRelationshipClass(relationNameWithCustomerOwned);
                        if (relationshipClass == null)
                        {
                            throw new InvalidConfigurationException(string.Format("Relationshipclass {0} cannot be found for class {1}", relationNameWithCustomerOwned, ((IDataset)row.Table).Name));
                        }

                        relatedClasses.Add(relatedSourceClassName.ToUpper());

                        //Get Update feat from PGE_GDBM_AH_INFO Table
                        RelatedDeletedFeat = GetRelatedRows(row.Table, row, relatedClasses, row.ChangeFeatures);

                        if (RelatedDeletedFeat.ContainsKey(relatedSourceClassName.ToUpper()))
                            deleteFeat = RelatedDeletedFeat[relatedSourceClassName.ToUpper()].First();

                        if (deleteFeat != null)
                        {
                            if (deleteFeat.fields_Old.ContainsKey("CustomerOwned".ToUpper()))
                                return deleteFeat.fields_Old["CustomerOwned".ToUpper()];
                        }
                    }

                }

                #region Commented
                //Settings settings = _trackedClass.Settings;
                //string relationNameWithCustomerOwned = "";
                //if (settings != null && !string.IsNullOrEmpty(settings["RelationshipNameWithCustomerOwned"]))
                //{
                //    relationNameWithCustomerOwned = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithCustomerOwned"];

                //    //it's better to do another NULLorEmpty check but since RelationshipNameWithCustomerOwned and SourceClassNameWithCustomerOwned two config are set like a bundle, so skip it for now
                //    string relatedSourceClassName = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["SourceClassNameWithCustomerOwned"];

                //    IDataset dataset = (IDataset)_trackedClass.SourceTable;
                //    IWorkspace workspace = dataset.Workspace;
                //    IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
                //    ITable relatedTable = featWorkspace.OpenTable(relatedSourceClassName);

                //    if (relatedTable != null)
                //    {
                //        int index = BaseRowTransformer.GetFieldIndex((IObjectClass)relatedTable, "CustomerOwned");
                //        if (index != -1)
                //        {
                //            //get related object
                //            IRelationshipClass2 relationshipClass = (IRelationshipClass2)featWorkspace.OpenRelationshipClass(relationNameWithCustomerOwned);
                //            if (relationshipClass == null)
                //            {
                //                throw new InvalidConfigurationException(string.Format("Relationshipclass {0} cannot be found for class {1}", relationNameWithCustomerOwned, (_trackedClass.QualifiedSourceClass)));
                //            }
                //            ISet relatedObjects = relationshipClass.GetObjectsRelatedToObject1((IObject)row);
                //            relatedObjects.Reset();
                //            IObject relatedObject = (IObject)relatedObjects.Next();
                //            if (relatedObject != null)
                //            {
                //                return relatedObject.get_Value((int)index).ToString();
                //            }
                //        }
                //    }

                //}
                #endregion

                _errorMessage.AppendLine(row.OID + " CustomerOwned not Found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                //CusomerOwned field not found
                return "";
            }
            //return row.get_Value((int)fieldIndex).ToString();
        }

        //IVARA
        /// <summary>
        /// Validates if the feature is a network feature [ i.e. if FeederType = 2]
        /// </summary>
        /// <param name="row">Source row</param>
        /// <returns>true or false</returns>
        public virtual bool IsNetworkFeature(IRow row)
        {
            int circuitIDIndex = -1;
            int circuitID2Index = -1;
            int netFeederType = Convert.ToInt32(_systemMapper.Settings["NetworkFeederType"]);
            bool isNet = false;

            string circuitIDField = _trackedClass.Settings["CircuitID"];
            string circuitID2Field = _trackedClass.Settings["CircuitID2"];

            ITable tbl = row.Table;
            if (!string.IsNullOrEmpty(circuitIDField))
            {
                circuitIDIndex = tbl.FindField(circuitIDField);
                if (circuitIDIndex == -1)
                {
                    // exception or message as return value so the process continues?
                    throw new InvalidConfigurationException(string.Format("Field {0} not found in table {1}.", circuitIDField, ((IDataset)row.Table).Name));
                }

                object circuitID = row.get_Value((int)circuitIDIndex);
                if (circuitID != DBNull.Value)
                {
                    object feederType = GetFeederType(circuitID.ToString(), row);
                    if (feederType != DBNull.Value && Convert.ToInt32(feederType) == netFeederType)
                    {
                        isNet = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(circuitID2Field))
            {
                circuitID2Index = tbl.FindField(circuitID2Field);
                if (circuitID2Index == -1)
                {
                    // exception or message as return value so the process continues?
                    throw new InvalidConfigurationException(string.Format("Field {0} not found in table {1}.", circuitID2Field, ((IDataset)row.Table).Name));
                }

                object circuitID2 = row.get_Value((int)circuitID2Index);
                if (circuitID2 != DBNull.Value)
                {
                    object feederType = GetFeederType(circuitID2.ToString(), row);
                    if (feederType != DBNull.Value && Convert.ToInt32(feederType) == netFeederType)
                    {
                        isNet = true;
                    }
                }
            }

            return isNet;
        }

        /// <summary>
        /// Validates if the feature is a network feature [ i.e. if FeederType = 2]
        /// </summary>
        /// <param name="row">Source row</param>
        /// <returns>true or false</returns>
        public virtual bool IsNetworkFeature(DeleteFeat row)
        {
            int circuitIDIndex = -1;
            int circuitID2Index = -1;
            int netFeederType = Convert.ToInt32(_systemMapper.Settings["NetworkFeederType"]);
            bool isNet = false;

            string circuitIDField = _trackedClass.Settings["CircuitID"];
            string circuitID2Field = _trackedClass.Settings["CircuitID2"];

            ITable tbl = row.Table;
            if (!string.IsNullOrEmpty(circuitIDField))
            {
                circuitIDIndex = tbl.FindField(circuitIDField);
                if (circuitIDIndex == -1)
                {
                    // exception or message as return value so the process continues?
                    throw new InvalidConfigurationException(string.Format("Field {0} not found in table {1}.", circuitIDField, ((IDataset)row.Table).Name));
                }

                string circuitID = string.Empty;
                if (row.fields_Old.ContainsKey(circuitIDField.ToUpper()))
                    circuitID = row.fields_Old[circuitIDField.ToUpper()];

                //object circuitID = row.get_Value((int)circuitIDIndex);
                //if (circuitID != DBNull.Value)
                if(!string.IsNullOrWhiteSpace(circuitID))
                {
                    object feederType = GetFeederType(circuitID.ToString(), row);
                    if (feederType != DBNull.Value && Convert.ToInt32(feederType) == netFeederType)
                    {
                        isNet = true;
                    }
                }
            }

            if (!string.IsNullOrEmpty(circuitID2Field))
            {
                circuitID2Index = tbl.FindField(circuitID2Field);
                if (circuitID2Index == -1)
                {
                    // exception or message as return value so the process continues?
                    throw new InvalidConfigurationException(string.Format("Field {0} not found in table {1}.", circuitID2Field, ((IDataset)row.Table).Name));
                }

                string circuitID2 = string.Empty;
                if (row.fields_Old.ContainsKey(circuitID2Field.ToUpper()))
                    circuitID2 = row.fields_Old[circuitID2Field.ToUpper()];

                //object circuitID2 = row.get_Value((int)circuitID2Index);
                //if (circuitID2 != DBNull.Value)
                if(!string.IsNullOrWhiteSpace(circuitID2))
                {
                    object feederType = GetFeederType(circuitID2.ToString(), row);
                    if (feederType != DBNull.Value && Convert.ToInt32(feederType) == netFeederType)
                    {
                        isNet = true;
                    }
                }
            }

            return isNet;
        }

        private object GetFeederType(string circuitID, IRow row)
        {
            string feederTypeField;
            string circuitSourcetable;

            object data = null;
            Settings settings = _trackedClass.Settings;
            if (settings != null && !string.IsNullOrEmpty(_systemMapper.Settings["CircuitSourceFeederTypeField"]) && !string.IsNullOrEmpty(_systemMapper.Settings["CircuitSourceTable"]))
            {
                feederTypeField = _systemMapper.Settings["CircuitSourceFeederTypeField"];
                circuitSourcetable = _systemMapper.Settings["CircuitSourceTable"];

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("CIRCUITID = '{0}'", circuitID);

                IWorkspace workspace = ((IDataset)row.Table).Workspace;
                IFeatureWorkspace fWorkspace = (IFeatureWorkspace)workspace;
                ITable circuitSourceTable = fWorkspace.OpenTable(circuitSourcetable);

                int fieldIndx = circuitSourceTable.FindField(feederTypeField);

                if (fieldIndx == -1)
                {
                    // exception or message as return value so the process continues?
                    throw new InvalidConfigurationException(string.Format("Field {0} not found in table {1}.", feederTypeField, ((IDataset)circuitSourceTable).Name));
                }

                ICursor cursor = null;

                try
                {
                    cursor = circuitSourceTable.Search(queryFilter, true);
                    while ((row = cursor.NextRow()) != null)
                    {
                        data = row.get_Value((int)fieldIndx);
                        break;
                    }
                }
                finally
                {
                    if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) ; cursor = null; }
                }
            }

            return data;
        }

        private object GetFeederType(string circuitID, DeleteFeat row)
        {
            string feederTypeField;
            string circuitSourcetable;

            object data = null;
            Settings settings = _trackedClass.Settings;
            if (settings != null && !string.IsNullOrEmpty(_systemMapper.Settings["CircuitSourceFeederTypeField"]) && !string.IsNullOrEmpty(_systemMapper.Settings["CircuitSourceTable"]))
            {
                feederTypeField = _systemMapper.Settings["CircuitSourceFeederTypeField"];
                circuitSourcetable = _systemMapper.Settings["CircuitSourceTable"];

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("CIRCUITID = '{0}'", circuitID);

                IWorkspace workspace = ((IDataset)row.Table).Workspace;
                IFeatureWorkspace fWorkspace = (IFeatureWorkspace)workspace;
                ITable circuitSourceTable = fWorkspace.OpenTable(circuitSourcetable);

                int fieldIndx = circuitSourceTable.FindField(feederTypeField);

                if (fieldIndx == -1)
                {
                    // exception or message as return value so the process continues?
                    throw new InvalidConfigurationException(string.Format("Field {0} not found in table {1}.", feederTypeField, ((IDataset)circuitSourceTable).Name));
                }

                ICursor cursor = null;
                IRow circuitRow = null;
                try
                {
                    cursor = circuitSourceTable.Search(queryFilter, true);
                    while ((circuitRow = cursor.NextRow()) != null)
                    {
                        data = circuitRow.get_Value((int)fieldIndx);
                        break;
                    }
                }
                finally
                {
                    if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0) ; cursor = null; }
                }
            }

            return data;
        }

        private Dictionary<string, List<DeleteFeat>> GetRelatedRows(ITable table, DeleteFeat row, IList<string> relatedClasses, IDictionary<string, ChangedFeatures> changedFeatures, [Optional] IRelationshipClass relClass)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);
            IEnumRelationshipClass enumRelationshipClass = null;
            Dictionary<string, List<DeleteFeat>> relatedRow = null;
            string fieldOriginForeignKey = string.Empty;
            string fieldOriginPrimaryKey = string.Empty;
            ITable relatedTable = default;
            QueryFilter queryFilter = default;
            DeleteFeat feat = default;
            IRow relRow = default;
            ICursor cursor = default;
            DBHelper dBHelper = default;
            try
            {
                dBHelper = new DBHelper();
                relatedRow = new Dictionary<string, List<DeleteFeat>>();
                enumRelationshipClass = ((table as IObjectClass)).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass relationshipClass;
                bool containsRelatedClass = false;
                while ((relationshipClass = enumRelationshipClass.Next()) != null)
                {
                    containsRelatedClass = false;
                    fieldOriginForeignKey = string.Empty;
                    fieldOriginPrimaryKey = string.Empty;
                    // If the target is on our list 
                    if (!containsRelatedClass && relatedClasses != null && relatedClasses.Count > 0)
                    {
                        containsRelatedClass =
                            relatedClasses.Contains(((IDataset)relationshipClass.DestinationClass).Name.ToUpper());
                    }
                    if (!containsRelatedClass && relatedClasses != null && relatedClasses.Count > 0)
                    {
                        containsRelatedClass =
                            relatedClasses.Contains(((IDataset)relationshipClass.OriginClass).Name.ToUpper());
                    }
                    if (!containsRelatedClass && relatedClasses != null && relatedClasses.Count > 0)
                    {
                        if (relClass != null)
                        {
                            if (relationshipClass.RelationshipClassID == relClass.RelationshipClassID)
                            {
                                containsRelatedClass = true;
                            }
                        }
                    }
                    if (containsRelatedClass)
                    {
                        //_logger.Debug("Found RelationshipClass Orig [ " + ((IDataset)relationshipClass.OriginClass).BrowseName +
                        //              " ] Dest [ " + ((IDataset)relationshipClass.DestinationClass).BrowseName + " ]");

                        if (((IDataset)table).BrowseName.ToUpper() == ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper())
                        {
                            fieldOriginForeignKey = relationshipClass.OriginForeignKey.ToUpper();
                            fieldOriginPrimaryKey = relationshipClass.OriginPrimaryKey.ToUpper();

                            if (changedFeatures.ContainsKey(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()))
                            {
                                if (changedFeatures[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Action.Delete.Count > 0)
                                {

                                    if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                                    {
                                        try
                                        {
                                            #region Commented
                                            //if (changedFeatures[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Action.Delete.Any(str => str.fields_Old[fieldOriginForeignKey] == row.fields_Old[fieldOriginPrimaryKey]))
                                            //{
                                            //    //relatedRow = 
                                            //    //foreach (DeleteFeat deleteFeat in changedFeatures[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Action.Delete.Where(str => str.fields_Old[fieldOriginForeignKey] == row.fields_Old[fieldOriginPrimaryKey]))
                                            //    //{
                                            //    //    if (deleteFeat != null)
                                            //    //    {
                                            //    //        //_logger.Debug("Found Related Object [ " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() +
                                            //    //        //          " ] OID [ " + deleteFeat.OID + " ]");
                                            //    //        if (!relatedRow.ContainsKey(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()))
                                            //    //        {
                                            //    //            relatedRow.Add(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper(), new List<DeleteFeat>());
                                            //    //        }

                                            //    //        if (!relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Any(str => str.OID == deleteFeat.OID))
                                            //    //        {
                                            //    //            deleteFeat.Table = (ITable)relationshipClass.DestinationClass;
                                            //    //            deleteFeat.ChangeFeatures = row.ChangeFeatures;
                                            //    //            relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Add(deleteFeat);
                                            //    //        }
                                            //    //    }
                                            //    //}

                                            //}
                                            #endregion

                                        }
                                        catch
                                        {
                                            //_logger.Debug(fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper());
                                            //_logger.Debug(fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper());
                                        }
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                            {
                                try
                                {
                                    foreach (DeleteFeat deleteFeat in dBHelper.GetRelatedCD(((IDataset)relationshipClass.DestinationClass).Name.ToUpper(), fieldOriginForeignKey, row.fields_Old[fieldOriginPrimaryKey], (IDataset)relationshipClass.DestinationClass))
                                    {
                                        if (deleteFeat != null)
                                        {
                                            //_logger.Debug("Found Related Object [ " + relationshipClass.DestinationClass.AliasName.ToUpper() +
                                            //          " ] OID [ " + deleteFeat.OID + " ]");
                                            if (!relatedRow.ContainsKey(((IDataset)relationshipClass.DestinationClass).Name.ToUpper()))
                                            {
                                                relatedRow.Add(((IDataset)relationshipClass.DestinationClass).Name.ToUpper(), new List<DeleteFeat>());
                                            }
                                            //else
                                            {
                                                deleteFeat.Table = (ITable)relationshipClass.DestinationClass;
                                                deleteFeat.ChangeFeatures = row.ChangeFeatures;
                                                if (deleteFeat.geometry_Old != null)
                                                {
                                                    ISpatialReference spatialReference = default;
                                                    spatialReference = GetSpatialReferenceFromDataset((IDataset)relationshipClass.DestinationClass);
                                                    deleteFeat.geometry_Old.SpatialReference = spatialReference;
                                                }
                                                relatedRow[((IDataset)relationshipClass.DestinationClass).Name.ToUpper()].Add(deleteFeat);
                                            }
                                        }
                                    }
                                }
                                catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                                catch (Exception ex)
                                {
                                    _errorMessage.AppendLine("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                    _logger.Info("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
                                    _logger.Error("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
                                    throw ex;
                                }

                                relatedTable = (ITable)relationshipClass.DestinationClass;
                                queryFilter = new QueryFilterClass();

                                queryFilter.WhereClause = fieldOriginForeignKey + " = '" + row.fields_Old[fieldOriginPrimaryKey] + "'";
                                feat = new DeleteFeat();
                                relRow = default;
                                cursor = relatedTable.Search(queryFilter, false);
                                while ((relRow = cursor.NextRow()) != null)
                                {
                                    feat = CDVersionManager.CreateDeleteFeat(relRow);
                                    feat.Table = relatedTable;
                                    feat.ChangeFeatures = row.ChangeFeatures;

                                    if (!relatedRow.ContainsKey(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()))
                                        relatedRow.Add(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper(), new List<DeleteFeat>());

                                    if (!relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Any(str => str.OID == feat.OID))
                                    {
                                        relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Add(feat);
                                    }
                                }
                            }
                        }
                        else
                        {
                            fieldOriginForeignKey = relationshipClass.OriginForeignKey.ToUpper();
                            fieldOriginPrimaryKey = relationshipClass.OriginPrimaryKey.ToUpper();

                            if (changedFeatures.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                            {
                                if (changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Delete.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                                    {
                                        try
                                        {
                                            #region Commented
                                            //if (changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Delete.Any(str => str.fields_Old[fieldOriginPrimaryKey] == row.fields_Old[fieldOriginForeignKey]))
                                            //{
                                            //    foreach (DeleteFeat deleteFeat in changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Delete.Where(str => str.fields_Old[fieldOriginPrimaryKey] == row.fields_Old[fieldOriginForeignKey]))
                                            //    {
                                            //        if (deleteFeat != null)
                                            //        {
                                            //            //_logger.Debug("Found Related Object [ " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() +
                                            //            //          " ] OID [ " + deleteFeat.OID + " ]");

                                            //            if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                                            //            {
                                            //                relatedRow.Add(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper(), new List<DeleteFeat>());
                                            //            }

                                            //            if (!relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Any(str => str.OID == deleteFeat.OID))
                                            //            {
                                            //                deleteFeat.Table = (ITable)relationshipClass.OriginClass;
                                            //                deleteFeat.ChangeFeatures = row.ChangeFeatures;
                                            //                relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Add(deleteFeat);
                                            //            }


                                            //        }
                                            //    }
                                            //}
                                            #endregion
                                        }
                                        catch
                                        {
                                            //_logger.Debug(fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper());
                                            //_logger.Debug(fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper());
                                        }
                                    }
                                }

                            }

                            if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                            {
                                try
                                {
                                    foreach (DeleteFeat deleteFeat in dBHelper.GetRelatedCD(((IDataset)relationshipClass.OriginClass).Name.ToUpper(), fieldOriginPrimaryKey, row.fields_Old[fieldOriginForeignKey], (IDataset)relationshipClass.OriginClass))
                                    {
                                        if (deleteFeat != null)
                                        {
                                            //_logger.Debug("Found Related Object [ " + relationshipClass.DestinationClass.AliasName.ToUpper() +
                                            //          " ] OID [ " + deleteFeat.OID + " ]");
                                            if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).Name.ToUpper()))
                                            {
                                                relatedRow.Add(((IDataset)relationshipClass.OriginClass).Name.ToUpper(), new List<DeleteFeat>());
                                            }
                                            //else
                                            {
                                                deleteFeat.Table = (ITable)relationshipClass.OriginClass;
                                                deleteFeat.ChangeFeatures = row.ChangeFeatures;
                                                if (deleteFeat.geometry_Old != null)
                                                {
                                                    ISpatialReference spatialReference = default;
                                                    spatialReference = GetSpatialReferenceFromDataset((IDataset)relationshipClass.OriginClass);
                                                    deleteFeat.geometry_Old.SpatialReference = spatialReference;
                                                }
                                                relatedRow[((IDataset)relationshipClass.OriginClass).Name.ToUpper()].Add(deleteFeat);
                                            }
                                        }
                                    }
                                }
                                catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                                catch (Exception ex)
                                {
                                    _errorMessage.AppendLine("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                    _logger.Info("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
                                    _logger.Error("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
                                    throw ex;
                                }

                                relatedTable = (ITable)relationshipClass.OriginClass;
                                queryFilter = new QueryFilterClass();

                                queryFilter.WhereClause = fieldOriginPrimaryKey + " = '" + row.fields_Old[fieldOriginForeignKey] + "'";
                                feat = new DeleteFeat();
                                relRow = default;
                                cursor = relatedTable.Search(queryFilter, false);
                                while ((relRow = cursor.NextRow()) != null)
                                {
                                    feat = CDVersionManager.CreateDeleteFeat(relRow);
                                    feat.Table = relatedTable;
                                    feat.ChangeFeatures = row.ChangeFeatures;

                                    if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                                        relatedRow.Add(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper(), new List<DeleteFeat>());

                                    if (!relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Any(str => str.OID == feat.OID))
                                    {
                                        relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Add(feat);
                                    }
                                }
                            }
                        }



                        //ISet objectSet = relationshipClass.GetObjectsRelatedToObject(row as IObject);
                        //if (objectSet != null)
                        //{
                        //    relatedRow = objectSet.Next() as IRow;
                        //    if (relatedRow != null)
                        //    {
                        //        _logger.Debug("Found Related Object [ " + ((IDataset)relatedRow.Table).Name +
                        //                      " ] OID [ " + relatedRow.OID + " ]");
                        //        break;
                        //    }
                        //}

                        //_logger.Debug("No Related Objects for " + row.GetRowDescription());
                    }

                }

            }
            catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception exception)
            {
                //_logger.Error(exception.ToString(), exception);
                throw exception;
            }
            finally
            {
                if (enumRelationshipClass != null) Marshal.FinalReleaseComObject(enumRelationshipClass);
                if (cursor != null) Marshal.FinalReleaseComObject(cursor);
            }

            return relatedRow;
        }

        /// <summary>
        /// Get Spatial Reference of Given Dataset
        /// </summary>
        /// <param name="dataset"></param>
        /// <returns>Spatial Reference</returns>
        public static ISpatialReference GetSpatialReferenceFromDataset(ESRI.ArcGIS.Geodatabase.IDataset dataset)
        {
            //then grab the spatial reference information and return it.
            //If the dataset supports IGeoDataset
            if (dataset is IGeoDataset geoDataset)
            {
                return geoDataset.SpatialReference;
            }
            else
            {
                return null; //otherwise return null
            }
        }

        private Dictionary<string, List<UpdateFeat>> GetRelatedRows(ITable table, UpdateFeat row, IList<string> relatedClasses, IDictionary<string, ChangedFeatures> changedFeatures, [Optional] IRelationshipClass relClass)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);
            IEnumRelationshipClass enumRelationshipClass = null;
            Dictionary<string, List<UpdateFeat>> relatedRow = null;
            string fieldOriginForeignKey = string.Empty;
            string fieldOriginPrimaryKey = string.Empty;
            ITable relatedTable = default;
            QueryFilter queryFilter = default;
            UpdateFeat feat = default;
            IRow relRow = default;
            ICursor cursor = default; 
            DBHelper dBHelper = default;
            try
            {
                dBHelper = new DBHelper();
                relatedRow = new Dictionary<string, List<UpdateFeat>>();
                enumRelationshipClass = ((table as IObjectClass)).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass relationshipClass;
                bool containsRelatedClass = false;
                while ((relationshipClass = enumRelationshipClass.Next()) != null)
                {
                    containsRelatedClass = false;
                    fieldOriginForeignKey = string.Empty;
                    fieldOriginPrimaryKey = string.Empty;

                    // If the target is on our list 
                    if (!containsRelatedClass && relatedClasses != null && relatedClasses.Count > 0)
                    {
                        containsRelatedClass =
                            relatedClasses.Contains(((IDataset)relationshipClass.DestinationClass).Name.ToUpper());
                    }
                    if (!containsRelatedClass && relatedClasses != null && relatedClasses.Count > 0)
                    {
                        containsRelatedClass =
                            relatedClasses.Contains(((IDataset)relationshipClass.OriginClass).Name.ToUpper());
                    }
                    if (!containsRelatedClass && relatedClasses != null && relatedClasses.Count > 0)
                    {
                        if (relClass != null)
                        {
                            if (relationshipClass.RelationshipClassID == relClass.RelationshipClassID)
                            {
                                containsRelatedClass = true;
                            }
                        }
                    }
                    if (containsRelatedClass)
                    {
                        //_logger.Debug("Found RelationshipClass Orig [ " + ((IDataset)relationshipClass.OriginClass).BrowseName +
                        //              " ] Dest [ " + ((IDataset)relationshipClass.DestinationClass).BrowseName + " ]");

                        if (((IDataset)table).BrowseName.ToUpper() == ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper())
                        {
                            fieldOriginForeignKey = relationshipClass.OriginForeignKey.ToUpper();
                            fieldOriginPrimaryKey = relationshipClass.OriginPrimaryKey.ToUpper();

                            if (changedFeatures.ContainsKey(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()))
                            {
                                if (changedFeatures[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Action.Update.Count > 0)
                                {
                                    if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                                    {
                                        try
                                        {
                                            #region Commented
                                            //if (changedFeatures[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Action.Update.Any(str => str.fields_Old[fieldOriginForeignKey] == row.fields_Old[fieldOriginPrimaryKey]))
                                            //{
                                            //    //relatedRow = 
                                            //    foreach (UpdateFeat updateFeat in changedFeatures[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Action.Update.Where(str => str.fields_Old[fieldOriginForeignKey] == row.fields_Old[fieldOriginPrimaryKey]))
                                            //    {
                                            //        if (updateFeat != null)
                                            //        {
                                            //            //_logger.Debug("Found Related Object [ " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() +
                                            //            //          " ] OID [ " + deleteFeat.OID + " ]");
                                            //            if (!relatedRow.ContainsKey(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()))
                                            //            {
                                            //                relatedRow.Add(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper(), new List<UpdateFeat>());
                                            //            }
                                                        
                                            //            if (!relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Any(str => str.OID == feat.OID))
                                            //            {
                                            //                updateFeat.Table = (ITable)relationshipClass.DestinationClass;
                                            //                updateFeat.ChangeFeatures = row.ChangeFeatures;
                                            //                relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Add(updateFeat);
                                            //            }
                                            //        }
                                            //    }
                                            //}
                                            #endregion

                                            
                                        }
                                        catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                                        catch(Exception ex)
                                        {
                                            _errorMessage.AppendLine("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                            throw ex;
                                            //_logger.Debug(fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper());
                                            //_logger.Debug(fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper());
                                        }
                                    }
                                }
                            }

                            try
                            {
                                foreach (UpdateFeat updateFeat in dBHelper.GetRelatedUCD(((IDataset)relationshipClass.DestinationClass).Name.ToUpper(), fieldOriginForeignKey, row.fields_Old[fieldOriginPrimaryKey], (IDataset)relationshipClass.DestinationClass))
                                {
                                    if (updateFeat != null)
                                    {
                                        //_logger.Debug("Found Related Object [ " + relationshipClass.DestinationClass.AliasName.ToUpper() +
                                        //          " ] OID [ " + deleteFeat.OID + " ]");
                                        if (!relatedRow.ContainsKey(((IDataset)relationshipClass.DestinationClass).Name.ToUpper()))
                                        {
                                            relatedRow.Add(((IDataset)relationshipClass.DestinationClass).Name.ToUpper(), new List<UpdateFeat>());
                                        }
                                        //else
                                        {
                                            updateFeat.Table = (ITable)relationshipClass.DestinationClass;
                                            updateFeat.ChangeFeatures = row.ChangeFeatures;
                                            if (updateFeat.geometry_Old != null)
                                            {
                                                ISpatialReference spatialReference = default;
                                                spatialReference = GetSpatialReferenceFromDataset((IDataset)relationshipClass.DestinationClass);
                                                updateFeat.geometry_Old.SpatialReference = spatialReference;
                                            }
                                            relatedRow[((IDataset)relationshipClass.DestinationClass).Name.ToUpper()].Add(updateFeat);
                                        }
                                    }
                                }
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                            catch (Exception ex)
                            {
                                _errorMessage.AppendLine("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                _logger.Info("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
                                _logger.Error("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
                                throw ex;
                            }

                            if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                            {
                                relatedTable = (ITable)relationshipClass.DestinationClass;
                                queryFilter = new QueryFilterClass();

                                queryFilter.WhereClause = fieldOriginForeignKey + " = '" + row.fields_Old[fieldOriginPrimaryKey] + "'";
                                feat = new UpdateFeat();
                                relRow = default;
                                cursor = relatedTable.Search(queryFilter, false);
                                while ((relRow = cursor.NextRow()) != null)
                                {
                                    feat = CDVersionManager.CreateUpdateFeat(relRow);
                                    feat.Table = relatedTable;
                                    feat.ChangeFeatures = row.ChangeFeatures;

                                    if (!relatedRow.ContainsKey(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()))
                                        relatedRow.Add(((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper(), new List<UpdateFeat>());

                                    if (!relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Any(str => str.OID == feat.OID))
                                    {
                                        relatedRow[((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper()].Add(feat);
                                    }
                                }
                            }
                        }
                        else
                        {
                            fieldOriginForeignKey = relationshipClass.OriginForeignKey.ToUpper();
                            fieldOriginPrimaryKey = relationshipClass.OriginPrimaryKey.ToUpper();

                            if (changedFeatures.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                            {
                                #region Commented
                                //if (changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Delete.Count > 0)
                                //{
                                //    if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                                //    {
                                //        try
                                //        {
                                //            if (changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Update.Any(str => str.fields_Old[fieldOriginPrimaryKey] == row.fields_Old[fieldOriginForeignKey]))
                                //            {
                                //                foreach (UpdateFeat updateFeat in changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Update.Where(str => str.fields_Old[fieldOriginPrimaryKey] == row.fields_Old[fieldOriginForeignKey]))
                                //                {
                                //                    if (updateFeat != null)
                                //                    {
                                //                        //_logger.Debug("Found Related Object [ " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() +
                                //                        //          " ] OID [ " + deleteFeat.OID + " ]");

                                //                        if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                                //                        {
                                //                            relatedRow.Add(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper(), new List<UpdateFeat>());
                                //                        }

                                //                        if (!relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Any(str => str.OID == feat.OID))
                                //                        {
                                //                            updateFeat.Table = (ITable)relationshipClass.OriginClass;
                                //                            updateFeat.ChangeFeatures = row.ChangeFeatures;
                                //                            relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Add(updateFeat);
                                //                        }
                                //                    }
                                //                }
                                //            }
                                //        }
                                //        catch(Exception ex)
                                //        {
                                //            _errorMessage.AppendLine("Error: "+fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()+" "+ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                //            //_logger.Debug(fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper());
                                //            //_logger.Debug(fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper());
                                //        }
                                //    }
                                //}
                                #endregion  
                            }
                            
                            try
                            {
                                foreach (UpdateFeat updateFeat in dBHelper.GetRelatedUCD(((IDataset)relationshipClass.OriginClass).Name.ToUpper(), fieldOriginPrimaryKey, row.fields_Old[fieldOriginForeignKey], (IDataset)relationshipClass.OriginClass))
                                {
                                    if (updateFeat != null)
                                    {
                                        //_logger.Debug("Found Related Object [ " + relationshipClass.DestinationClass.AliasName.ToUpper() +
                                        //          " ] OID [ " + deleteFeat.OID + " ]");
                                        if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).Name.ToUpper()))
                                        {
                                            relatedRow.Add(((IDataset)relationshipClass.OriginClass).Name.ToUpper(), new List<UpdateFeat>());
                                        }
                                        //else
                                        {
                                            updateFeat.Table = (ITable)relationshipClass.OriginClass;
                                            updateFeat.ChangeFeatures = row.ChangeFeatures;
                                            if (updateFeat.geometry_Old != null)
                                            {
                                                ISpatialReference spatialReference = default;
                                                spatialReference = GetSpatialReferenceFromDataset((IDataset)relationshipClass.OriginClass);
                                                updateFeat.geometry_Old.SpatialReference = spatialReference;
                                            }
                                            relatedRow[((IDataset)relationshipClass.OriginClass).Name.ToUpper()].Add(updateFeat);
                                        }
                                    }
                                }
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                            catch (Exception ex)
                            {
                                _errorMessage.AppendLine("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                _logger.Info("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
                                _logger.Error("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
                                throw ex;
                            }

                            if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                            {
                                relatedTable = (ITable)relationshipClass.OriginClass;
                                queryFilter = new QueryFilterClass();

                                queryFilter.WhereClause = fieldOriginPrimaryKey + " = '" + row.fields_Old[fieldOriginForeignKey] + "'";
                                feat = new UpdateFeat();
                                relRow = default;
                                cursor = relatedTable.Search(queryFilter, false);
                                while ((relRow = cursor.NextRow()) != null)
                                {
                                    feat = CDVersionManager.CreateUpdateFeat(relRow);
                                    feat.Table = relatedTable;
                                    feat.ChangeFeatures = row.ChangeFeatures;

                                    if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                                        relatedRow.Add(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper(), new List<UpdateFeat>());

                                    if (!relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Any(str => str.OID == feat.OID))
                                    {
                                        relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Add(feat);
                                    }
                                }
                            }

                        }



                        //ISet objectSet = relationshipClass.GetObjectsRelatedToObject(row as IObject);
                        //if (objectSet != null)
                        //{
                        //    relatedRow = objectSet.Next() as IRow;
                        //    if (relatedRow != null)
                        //    {
                        //        _logger.Debug("Found Related Object [ " + ((IDataset)relatedRow.Table).Name +
                        //                      " ] OID [ " + relatedRow.OID + " ]");
                        //        break;
                        //    }
                        //}

                        //_logger.Debug("No Related Objects for " + row.GetRowDescription());
                    }

                }

            }
            catch (Oracle.DataAccess.Client.OracleException oex) { _logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception exception)
            {
                //_logger.Error(exception.ToString(), exception);
                throw exception;
            }
            finally
            {
                if (enumRelationshipClass != null) Marshal.FinalReleaseComObject(enumRelationshipClass);
                if (cursor != null) Marshal.FinalReleaseComObject(cursor);
            }

            return relatedRow;
        }

    }
}
