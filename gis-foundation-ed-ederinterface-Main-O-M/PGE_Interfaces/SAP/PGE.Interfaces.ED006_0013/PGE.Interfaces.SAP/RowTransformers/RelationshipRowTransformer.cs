using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Common.ChangeDetectionAPI;
using System.Linq;
using ESRI.ArcGIS.Geometry;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// This class is used to process asset classes that involve more than one GIS feature/object class.
    /// For example, Regulator asset attributes come from VoltageRegulatorUnit, VoltageRegulator, and along
    /// with attributes from these two classes, are attributes of Controller object related to VoltageRegulator.
    /// </summary>
    public class RelationshipRowTransformer : BaseRowTransformer
    {
        private IRowTransformer<IRow> _originRowTransformer;

        /// <summary>
        /// Create an instance of the RowTransformer that will be used to process the incoming current row (origin row relative to rows related to itself).
        /// </summary>
        /// <param name="sysMapper">This holds all configuration information including TrackedClasses and their RowTransformers</param>
        /// <param name="trackedClass">Holding class of RowTransformer, has information including MappedFields, Subtypes etc.</param>
        public override void Initialize(SystemMapper sysMapper, TrackedClass trackedClass)
        {
            base.Initialize(sysMapper, trackedClass);

            if (string.IsNullOrEmpty(trackedClass.TransformerTypeForOriginClass) == true)
            {
                throw new InvalidConfigurationException(trackedClass.SourceClass + " does not have a valid TransformerTypeForOriginClass defined.");
            }

            Type transformerTypeForOriginClass = Type.GetType(trackedClass.TransformerTypeForOriginClass);
            if (transformerTypeForOriginClass == null)
            {
            }

            _originRowTransformer = (IRowTransformer<IRow>)Activator.CreateInstance(transformerTypeForOriginClass);
            if (_originRowTransformer == null)
            {
            }

            _originRowTransformer.Initialize(sysMapper, trackedClass);
        }
        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns></returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            try
            {
                GISSAPIntegrator._logger.Info("Processing OID :: " + sourceRow.OID);

                List<IRowData> originRowDataList = _originRowTransformer.ProcessRow(sourceRow, targetRow, changeType);

                // if the incoming row defines AssetID, it could have been adequately processed and stored.
                if (originRowDataList.Count == 0)
                {
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " No valid processed result returned, related records will not be processed " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info(sourceRow.OID + " No valid processed result returned, related records will not be processed ");
                    GISSAPIntegrator._logger.Error(sourceRow.OID + " No valid processed result returned, related records will not be processed ");

                    return originRowDataList;
                }

                ISet relatedSourceObjects = null;
                ISet relatedTargetObjects = null;
                IRow relatedSourceRow = null;
                IRow relatedTargetRow = null;

                if (_trackedClass.RelatedClass.SourceClass != "Controller")//xwu-controller is no longer embeded in device 4.17.2013. this assume each tracked class only has at most one related class.
                {
                    GISSAPIntegrator._logger.Info("Getting OID :: " + sourceRow.OID + " Related Records");
                    GetRelatedObjects(sourceRow, targetRow, changeType, ref relatedSourceObjects, ref relatedTargetObjects);
                    GetNextRelatedRows(ref relatedSourceObjects, ref relatedTargetObjects, changeType, ref relatedSourceRow, ref relatedTargetRow);
                }
                // get related IRows
                // changeType can be an issue if we can't assume the same between Origin and Destination

                // need to make sure that relatedSourceRow and relatedTargetRow match, in case of Updated Bank->Units.
                // fortunately Units could just be treated as though they are the same across versions (source vs target) 
                // so we could pass source related row as source and target related row.
                // even though Units here can be Inserted/Updated, they still can be treated the same across versions, 
                // because they have been processed on their own configuration and won't be sent again.
                // more robust way is ITable.GetRow(relatedSourceRow.OID) to get related target row


                IRowData originRowData = originRowDataList[0];

                List<IRowData> mergedRowDataList = new List<IRowData>();

                // no related rows, 
                // for example, no Controller to VoltageRegulator
                // or, no VoltageRegulatorUnit to VoltageRegulator
                if (relatedSourceRow == null && relatedTargetRow == null)
                {
                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                    Settings settings = relatedClass.Settings;
                    if (settings != null && settings["NullRowOk"] == "True")
                    //if (relatedClass["NullRowOk"] != null && relatedClass["NullRowOk"].Value == "True")
                    {
                        // add place holders
                        foreach (MappedField mappedFld in relatedClass.Fields)
                        {
                            FieldMapper fldMapper = mappedFld.FieldMapper;
                            IFieldTransformer<IRow> fldTransformer = fldMapper.FieldTransformer;

                            try
                            {
                                originRowData.FieldValues.Add(mappedFld.Sequence, string.Empty);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [START]
                        foreach (MappedField mappedFld in relatedClass.Fields)
                        {
                            try
                            {
                                if (!originRowData.FieldKeyValue.ContainsKey(mappedFld.OutName))
                                    originRowData.FieldKeyValue.Add(mappedFld.OutName, string.Empty);

                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [END]

                        mergedRowDataList.Add(originRowData);
                    }
                    else
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " No valid related records Found" + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info(sourceRow.OID + " No valid related records Found");
                        GISSAPIntegrator._logger.Error(sourceRow.OID + " No valid related records Found");
                    }
                }

                while (relatedSourceRow != null || relatedTargetRow != null)
                {
                    GISSAPIntegrator._logger.Info("Processing OID :: " + sourceRow.OID + " Related Records");

                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                    IRowTransformer<IRow> relatedRowTransformer = relatedClass.RowTransformer;
                    List<IRowData> relatedRowDataList = relatedRowTransformer.ProcessRow(relatedSourceRow, relatedTargetRow, changeType);

                    foreach (IRowData relatedRowData in relatedRowDataList)
                    {
                        foreach (KeyValuePair<int, string> fieldSequenceValue in originRowData.FieldValues)
                        {
                            try
                            {
                                relatedRowData.FieldValues.Add(fieldSequenceValue.Key, fieldSequenceValue.Value);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }

                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [START]
                        foreach (KeyValuePair<string, string> fieldOutname in originRowData.FieldKeyValue)
                        {
                            try
                            {
                                relatedRowData.FieldKeyValue.Add(fieldOutname.Key, fieldOutname.Value);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [END]

                        // make sure AssetID accessible after merging field values
                        if (_trackedClass.AssetIDField > 0)
                        {
                            relatedRowData.AssetID = originRowData.AssetID;
                        }

                        ISAPRowData sapRowData = (ISAPRowData)originRowData;
                        if (sapRowData.SAPType != SAPType.NotApplicable)
                        {
                            ISAPRowData relatedSapRowData = (ISAPRowData)relatedRowData;
                            relatedSapRowData.SAPType = sapRowData.SAPType;
                        }

                        if (originRowData.OID > 0)
                        {
                            relatedRowData.OID = originRowData.OID;
                            relatedRowData.FCID = originRowData.FCID;
                        }
                    }

                    mergedRowDataList.AddRange(relatedRowDataList);

                    if (relatedSourceRow != null) { while (Marshal.ReleaseComObject(relatedSourceRow) > 0) { } }
                    if (relatedTargetRow != null) { while (Marshal.ReleaseComObject(relatedTargetRow) > 0) { } }
                    GetNextRelatedRows(ref relatedSourceObjects, ref relatedTargetObjects, changeType, ref relatedSourceRow, ref relatedTargetRow);
                }

                //Clear out resources
                IRow row = null;
                if (relatedSourceObjects != null)
                {
                    relatedSourceObjects.Reset();
                    while ((row = relatedSourceObjects.Next() as IRow) != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                    while (Marshal.ReleaseComObject(relatedSourceObjects) > 0) { }
                }

                if (relatedTargetObjects != null)
                {
                    relatedTargetObjects.Reset();
                    while ((row = relatedTargetObjects.Next() as IRow) != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                    while (Marshal.ReleaseComObject(relatedTargetObjects) > 0) { }
                }

                return mergedRowDataList;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message);
                GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message);
                throw ex;
            }
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
            try
            {
                GISSAPIntegrator._logger.Info("Processing OID :: " + sourceRow.OID);

                List<IRowData> originRowDataList = _originRowTransformer.ProcessRow(sourceRow, targetRow, changeType);

                // if the incoming row defines AssetID, it could have been adequately processed and stored.
                if (originRowDataList.Count == 0)
                {
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " No valid processed result returned, related records will not be processed " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info(sourceRow.OID + " No valid processed result returned, related records will not be processed ");
                    GISSAPIntegrator._logger.Error(sourceRow.OID + " No valid processed result returned, related records will not be processed ");

                    return originRowDataList;
                }

                ISet relatedSourceObjects = null;
                ISet relatedTargetObjects = null;
                IRow relatedSourceRow = null;
                IRow relatedTargetRow = null;

                if (_trackedClass.RelatedClass.SourceClass != "Controller")//xwu-controller is no longer embeded in device 4.17.2013. this assume each tracked class only has at most one related class.
                {
                    GISSAPIntegrator._logger.Info("Getting OID :: " + sourceRow.OID + " Related Records");
                    GetRelatedObjects(sourceRow, targetRow, changeType, ref relatedSourceObjects, ref relatedTargetObjects);
                    GetNextRelatedRows(ref relatedSourceObjects, ref relatedTargetObjects, changeType, ref relatedSourceRow, ref relatedTargetRow);
                }
                // get related IRows
                // changeType can be an issue if we can't assume the same between Origin and Destination

                // need to make sure that relatedSourceRow and relatedTargetRow match, in case of Updated Bank->Units.
                // fortunately Units could just be treated as though they are the same across versions (source vs target) 
                // so we could pass source related row as source and target related row.
                // even though Units here can be Inserted/Updated, they still can be treated the same across versions, 
                // because they have been processed on their own configuration and won't be sent again.
                // more robust way is ITable.GetRow(relatedSourceRow.OID) to get related target row


                IRowData originRowData = originRowDataList[0];

                List<IRowData> mergedRowDataList = new List<IRowData>();

                // no related rows, 
                // for example, no Controller to VoltageRegulator
                // or, no VoltageRegulatorUnit to VoltageRegulator
                if (relatedSourceRow == null && relatedTargetRow == null)
                {
                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                    Settings settings = relatedClass.Settings;
                    if (settings != null && settings["NullRowOk"] == "True")
                    //if (relatedClass["NullRowOk"] != null && relatedClass["NullRowOk"].Value == "True")
                    {
                        // add place holders
                        foreach (MappedField mappedFld in relatedClass.Fields)
                        {
                            FieldMapper fldMapper = mappedFld.FieldMapper;
                            IFieldTransformer<IRow> fldTransformer = fldMapper.FieldTransformer;

                            try
                            {
                                originRowData.FieldValues.Add(mappedFld.Sequence, string.Empty);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                    
                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message );
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message );

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [START]
                        foreach (MappedField mappedFld in relatedClass.Fields)
                        {
                            try
                            {
                                if (!originRowData.FieldKeyValue.ContainsKey(mappedFld.OutName))
                                    originRowData.FieldKeyValue.Add(mappedFld.OutName, string.Empty);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {

                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [END]

                        mergedRowDataList.Add(originRowData);
                    }
                    else
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " No valid related records Found" + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info(sourceRow.OID + " No valid related records Found");
                        GISSAPIntegrator._logger.Error(sourceRow.OID + " No valid related records Found");
                    }
                }

                while (relatedSourceRow != null || relatedTargetRow != null)
                {
                    GISSAPIntegrator._logger.Info("Processing OID :: " + sourceRow.OID + " Related Records");

                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                    IRowTransformer<IRow> relatedRowTransformer = relatedClass.RowTransformer;
                    List<IRowData> relatedRowDataList = relatedRowTransformer.ProcessRow(relatedSourceRow, relatedTargetRow, changeType);

                    foreach (IRowData relatedRowData in relatedRowDataList)
                    {
                        foreach (KeyValuePair<int, string> fieldSequenceValue in originRowData.FieldValues)
                        {
                            try
                            {
                                relatedRowData.FieldValues.Add(fieldSequenceValue.Key, fieldSequenceValue.Value);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }

                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [START]
                        foreach (KeyValuePair<string, string> fieldOutname in originRowData.FieldKeyValue)
                        {
                            try
                            {
                                relatedRowData.FieldKeyValue.Add(fieldOutname.Key, fieldOutname.Value);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [END]

                        // make sure AssetID accessible after merging field values
                        if (_trackedClass.AssetIDField > 0)
                        {
                            relatedRowData.AssetID = originRowData.AssetID;
                        }

                        ISAPRowData sapRowData = (ISAPRowData)originRowData;
                        if (sapRowData.SAPType != SAPType.NotApplicable)
                        {
                            ISAPRowData relatedSapRowData = (ISAPRowData)relatedRowData;
                            relatedSapRowData.SAPType = sapRowData.SAPType;
                        }

                        if (originRowData.OID > 0)
                        {
                            relatedRowData.OID = originRowData.OID;
                            relatedRowData.FCID = originRowData.FCID;
                        }
                    }

                    mergedRowDataList.AddRange(relatedRowDataList);

                    if (relatedSourceRow != null) { while (Marshal.ReleaseComObject(relatedSourceRow) > 0) { } }
                    if (relatedTargetRow != null) { while (Marshal.ReleaseComObject(relatedTargetRow) > 0) { } }
                    GetNextRelatedRows(ref relatedSourceObjects, ref relatedTargetObjects, changeType, ref relatedSourceRow, ref relatedTargetRow);
                }

                //Clear out resources
                IRow row = null;
                if (relatedSourceObjects != null)
                {
                    relatedSourceObjects.Reset();
                    while ((row = relatedSourceObjects.Next() as IRow) != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                    while (Marshal.ReleaseComObject(relatedSourceObjects) > 0) { }
                }

                if (relatedTargetObjects != null)
                {
                    relatedTargetObjects.Reset();
                    while ((row = relatedTargetObjects.Next() as IRow) != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                    while (Marshal.ReleaseComObject(relatedTargetObjects) > 0) { }
                }

                return mergedRowDataList;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message);
                GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message);
                throw ex;
            }
        }

        public override List<IRowData> ProcessRow(DeleteFeat sourceRow, ChangeType changeType)
        {
            try
            {
                GISSAPIntegrator._logger.Info("Processing OID :: " + sourceRow.OID);

                List<IRowData> originRowDataList = _originRowTransformer.ProcessRow(sourceRow, changeType);

                // if the incoming row defines AssetID, it could have been adequately processed and stored.
                if (originRowDataList.Count == 0)
                {
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " No valid processed result returned, related records will not be processed " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info(sourceRow.OID + " No valid processed result returned, related records will not be processed ");
                    GISSAPIntegrator._logger.Error(sourceRow.OID + " No valid processed result returned, related records will not be processed ");
                    return originRowDataList;
                }

                //ISet relatedSourceObjects = null;
                //ISet relatedTargetObjects = null;
                DeleteFeat relatedSourceRow = null;
                List<string> relatedClasses = new List<string>();
                Dictionary<string, List<DeleteFeat>> RelatedDeletedFeat = new Dictionary<string, List<DeleteFeat>>();
                //DeleteFeat relatedTargetRow = null;

                //V3SF - Related Record - Default 0000
                if (_trackedClass.RelatedClass.SourceClass != "Controller")//xwu-controller is no longer embeded in device 4.17.2013. this assume each tracked class only has at most one related class.
                {
                    if (_trackedClass.RelatedClass != null)
                    {
                        if (_trackedClass.RelatedClass.QualifiedSourceClass != null)
                        {
                            relatedClasses.Add(_trackedClass.RelatedClass.QualifiedSourceClass.ToUpper());
                        }
                    }
                    GISSAPIntegrator._logger.Info("Getting OID :: " + sourceRow.OID + " Related Records");
                    RelatedDeletedFeat = GetRelatedRows(sourceRow.Table, sourceRow, relatedClasses, sourceRow.ChangeFeatures);
                    //GetRelatedObjects(sourceRow, changeType, ref relatedSourceObjects, ref relatedTargetObjects);
                    //GetNextRelatedRows(ref relatedSourceObjects, ref relatedTargetObjects, changeType, ref relatedSourceRow, ref relatedTargetRow);
                }
                // get related IRows
                // changeType can be an issue if we can't assume the same between Origin and Destination

                // need to make sure that relatedSourceRow and relatedTargetRow match, in case of Updated Bank->Units.
                // fortunately Units could just be treated as though they are the same across versions (source vs target) 
                // so we could pass source related row as source and target related row.
                // even though Units here can be Inserted/Updated, they still can be treated the same across versions, 
                // because they have been processed on their own configuration and won't be sent again.
                // more robust way is ITable.GetRow(relatedSourceRow.OID) to get related target row


                IRowData originRowData = originRowDataList[0];

                List<IRowData> mergedRowDataList = new List<IRowData>();

                // no related rows, 
                // for example, no Controller to VoltageRegulator
                // or, no VoltageRegulatorUnit to VoltageRegulator
                //if (relatedSourceRow == null && relatedTargetRow == null)
                if (RelatedDeletedFeat == null || !(RelatedDeletedFeat.Count > 0))
                {
                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                    Settings settings = relatedClass.Settings;
                    if (settings != null && settings["NullRowOk"] == "True")
                    //if (relatedClass["NullRowOk"] != null && relatedClass["NullRowOk"].Value == "True")
                    {
                        // add place holders
                        foreach (MappedField mappedFld in relatedClass.Fields)
                        {
                            FieldMapper fldMapper = mappedFld.FieldMapper;
                            IFieldTransformer<IRow> fldTransformer = fldMapper.FieldTransformer;

                            try
                            {
                                originRowData.FieldValues.Add(mappedFld.Sequence, string.Empty);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {

                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [START]
                        foreach (MappedField mappedFld in relatedClass.Fields)
                        {
                            try
                            {
                                if (!originRowData.FieldKeyValue.ContainsKey(mappedFld.OutName))
                                    originRowData.FieldKeyValue.Add(mappedFld.OutName, string.Empty);
                            }
                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                            catch (Exception ex)
                            {
                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                {
                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                    GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'"
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message);

                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                        + " Multiple field sequence values for " + mappedFld.Sequence + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                }
                                else { throw ex; }
                            }
                        }
                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [END]

                        mergedRowDataList.Add(originRowData);
                    }
                    else
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " No valid related records Found" + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info(sourceRow.OID + " No valid related records Found");
                        GISSAPIntegrator._logger.Error(sourceRow.OID + " No valid related records Found");
                    }
                }

                //while (relatedSourceRow != null || relatedTargetRow != null)
                {
                    if (RelatedDeletedFeat != null && RelatedDeletedFeat.Count > 0)
                    {
                        GISSAPIntegrator._logger.Info("Processing OID :: " + sourceRow.OID + " Related Records");

                        foreach (string FCName in RelatedDeletedFeat.Keys)
                        {
                            if (FCName.ToUpper() == _trackedClass.RelatedClass.QualifiedSourceClass.ToUpper())
                            {
                                foreach (DeleteFeat deleteFeat in RelatedDeletedFeat[FCName])
                                {
                                    relatedSourceRow = deleteFeat;
                                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                                    IRowTransformer<IRow> relatedRowTransformer = relatedClass.RowTransformer;
                                    List<IRowData> relatedRowDataList = relatedRowTransformer.ProcessRow(relatedSourceRow, changeType);

                                    foreach (IRowData relatedRowData in relatedRowDataList)
                                    {
                                        foreach (KeyValuePair<int, string> fieldSequenceValue in originRowData.FieldValues)
                                        {
                                            try
                                            {
                                                relatedRowData.FieldValues.Add(fieldSequenceValue.Key, fieldSequenceValue.Value);
                                            }
                                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                                            catch (Exception ex)
                                            {
                                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                                {

                                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                                        + " Multiple field sequence values for " + fieldSequenceValue.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                                }
                                                else { throw ex; }
                                            }
                                        }

                                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [START]
                                        foreach (KeyValuePair<string, string> fieldOutname in originRowData.FieldKeyValue)
                                        {
                                            try
                                            {
                                                relatedRowData.FieldKeyValue.Add(fieldOutname.Key, fieldOutname.Value);
                                            }
                                            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                                            catch (Exception ex)
                                            {
                                                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                                if (ex.Message.ToUpper().Contains("An item with the same key has already been added".ToUpper()))
                                                {
                                                    GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + "Error :: ( " + "Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers" + " ) " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

                                                    throw new Exception("Invalid configuration for tracked class with SourceClass='" + _trackedClass.SourceClass + "', OutName='" + _trackedClass.OutName + "'."
                                                        + " Multiple field sequence values for " + fieldOutname.Key + " encountered.  Verify the fields associated with these keys and correct the sequence numbers");
                                                }
                                                else { throw ex; }
                                            }
                                        }
                                        //V3SF - CD API (EDGISREARC-1452) - Added - Resolve issue of missing Fields from Payload [END]

                                        // make sure AssetID accessible after merging field values
                                        if (_trackedClass.AssetIDField > 0)
                                        {
                                            relatedRowData.AssetID = originRowData.AssetID;
                                        }

                                        ISAPRowData sapRowData = (ISAPRowData)originRowData;
                                        if (sapRowData.SAPType != SAPType.NotApplicable)
                                        {
                                            ISAPRowData relatedSapRowData = (ISAPRowData)relatedRowData;
                                            relatedSapRowData.SAPType = sapRowData.SAPType;
                                        }

                                        if (originRowData.OID > 0)
                                        {
                                            relatedRowData.OID = originRowData.OID;
                                            relatedRowData.FCID = originRowData.FCID;
                                        }
                                    }

                                    mergedRowDataList.AddRange(relatedRowDataList);
                                }
                            }
                        }
                        //if (relatedSourceRow != null) { while (Marshal.ReleaseComObject(relatedSourceRow) > 0) { } }
                        //if (relatedTargetRow != null) { while (Marshal.ReleaseComObject(relatedTargetRow) > 0) { } }
                        //GetNextRelatedRows(ref relatedSourceObjects, ref relatedTargetObjects, changeType, ref relatedSourceRow, ref relatedTargetRow);
                    }
                }

                //Clear out resources
                //IRow row = null;
                //if (relatedSourceObjects != null)
                //{
                //    relatedSourceObjects.Reset();
                //    while ((row = relatedSourceObjects.Next() as IRow) != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                //    while (Marshal.ReleaseComObject(relatedSourceObjects) > 0) { }
                //}

                //if (relatedTargetObjects != null)
                //{
                //    relatedTargetObjects.Reset();
                //    while ((row = relatedTargetObjects.Next() as IRow) != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                //    while (Marshal.ReleaseComObject(relatedTargetObjects) > 0) { }
                //}

                return mergedRowDataList;
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message);
                GISSAPIntegrator._logger.Error("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message);
                throw ex;
            }
        }

        private void GetNextRelatedRows(ref ISet relatedSourceObjects, ref ISet relatedTargetObjects, ChangeType changeType, ref IRow relatedSourceRow, ref IRow relatedTargetRow)
        {
            // reset rows to null
            relatedSourceRow = null;
            relatedTargetRow = null;

            if (changeType == ChangeType.Insert || changeType == ChangeType.Update || changeType == ChangeType.Reprocess)
            {
                if (relatedSourceObjects != null && relatedSourceObjects.Count > 0)
                {
                    relatedSourceRow = (IRow)relatedSourceObjects.Next();

                    if ((changeType == ChangeType.Update || changeType == ChangeType.Reprocess) && relatedSourceRow != null)
                    {
                        //ITable relatedTargetTable = _trackedClass.RelatedClass.TargetTable;
                        //relatedTargetRow = relatedTargetTable.GetRow(relatedSourceRow.OID);
                        relatedTargetRow = relatedSourceRow;
                        GISSAPIntegrator._logger.Info("Related Record(s) found with OID :: " + relatedTargetRow.OID);
                    }
                }
            }
            else if (changeType == ChangeType.Delete)
            {
                if (relatedTargetObjects != null && relatedTargetObjects.Count > 0)
                {
                    relatedTargetRow = (IRow)relatedTargetObjects.Next();
                    GISSAPIntegrator._logger.Info("Related Record(s) found with OID :: " + relatedTargetRow.OID + " from Feature Class :: " + _trackedClass.RelatedClass);
                }
            }
        }

        private void GetRelatedObjects(IRow sourceRow, IRow targetRow, ChangeType changeType, ref ISet relatedSourceObjects, ref ISet relatedTargetObjects)
        {
            if (changeType == ChangeType.Insert || changeType == ChangeType.Update || changeType == ChangeType.Reprocess)
            {
                //relatedSourceObjects = GetSourceRelatedObjects(sourceRow, _trackedClass.RelationshipName);
                relatedSourceObjects = GetSourceRelatedObjects(sourceRow, _trackedClass.QualifiedRelationshipName);
                if ((changeType == ChangeType.Update || changeType == ChangeType.Reprocess) && relatedSourceObjects.Count > 0)
                {
                    GISSAPIntegrator._logger.Info("Related Record(s) found for OID :: " + sourceRow.OID + " Count :: " + relatedSourceObjects.Count + " from Feature Class :: " + _trackedClass.RelatedClass);

                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                    ITable relatedTargetTable = null;
                    //string relatedSourceClassName = relatedClass.SourceClass;
                    string relatedSourceClassName = relatedClass.QualifiedSourceClass;
                    if (relatedTargetTable == null)
                    {
                        ITable table = targetRow.Table;
                        IDataset dataset = (IDataset)table;
                        IWorkspace workspace = dataset.Workspace;
                        IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
                        relatedTargetTable = featWorkspace.OpenTable(relatedSourceClassName);
                        relatedClass.TargetTable = relatedTargetTable;
                    }

                    //relatedTargetObjects = GetTargetRelatedObjects(targetRow, _trackedClass.RelationshipName);
                }
            }
            else if (changeType == ChangeType.Delete)
            {
                //relatedTargetObjects = GetTargetRelatedObjects(targetRow, _trackedClass.RelationshipName);
                relatedTargetObjects = GetTargetRelatedObjects(targetRow, _trackedClass.QualifiedRelationshipName);
            }
        }

        private void GetRelatedObjects(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType, ref ISet relatedSourceObjects, ref ISet relatedTargetObjects)
        {
            if (changeType == ChangeType.Insert || changeType == ChangeType.Update || changeType == ChangeType.Reprocess)
            {
                //relatedSourceObjects = GetSourceRelatedObjects(sourceRow, _trackedClass.RelationshipName);
                relatedSourceObjects = GetSourceRelatedObjects(sourceRow, _trackedClass.QualifiedRelationshipName);
                if ((changeType == ChangeType.Update || changeType == ChangeType.Reprocess) && relatedSourceObjects.Count > 0)
                {
                    GISSAPIntegrator._logger.Info("Related Record(s) found for OID :: "+ sourceRow.OID+" Count :: "+ relatedSourceObjects.Count+" from Feature Class :: "+ _trackedClass.RelatedClass);

                    TrackedClass relatedClass = _trackedClass.RelatedClass;
                    ITable relatedTargetTable = null;
                    //string relatedSourceClassName = relatedClass.SourceClass;
                    string relatedSourceClassName = relatedClass.QualifiedSourceClass;
                    if (relatedTargetTable == null)
                    {
                        ITable table = targetRow.Table;
                        IDataset dataset = (IDataset)table;
                        IWorkspace workspace = dataset.Workspace;
                        IFeatureWorkspace featWorkspace = (IFeatureWorkspace)workspace;
                        relatedTargetTable = featWorkspace.OpenTable(relatedSourceClassName);
                        relatedClass.TargetTable = relatedTargetTable;
                    }

                    //relatedTargetObjects = GetTargetRelatedObjects(targetRow, _trackedClass.RelationshipName);
                }
            }
            //else if (changeType == ChangeType.Delete)
            //{
            //    //relatedTargetObjects = GetTargetRelatedObjects(targetRow, _trackedClass.RelationshipName);
            //    relatedTargetObjects = GetTargetRelatedObjects(targetRow, _trackedClass.QualifiedRelationshipName);
            //}
        }


        //V3SF (28-Mar-2022) Added Additional Parameter to display Skipping Record in Staging
        //IVARA
        public override bool IsValid(IRow sourceRow, out string errorMsg)
        {
            bool retValue = true;
            errorMsg = "";
            if (_originRowTransformer != null)
            {
                retValue = _originRowTransformer.IsValid(sourceRow, out errorMsg);
                return retValue;
                //return _originRowTransformer.IsValid(sourceRow);
            }
            else
            {
                retValue = base.IsValid(sourceRow, out errorMsg);
                return retValue;
                //return base.IsValid(sourceRow);
            }
        }

        //V3SF (28-Mar-2022) Added Additional Parameter to display Skipping Record in Staging
        public override bool IsValid(DeleteFeat sourceRow, out string errorMsg)
        {
            bool retValue = true;
            errorMsg = "";
            if (_originRowTransformer != null)
            {
                retValue = _originRowTransformer.IsValid(sourceRow, out errorMsg);
                return retValue;
                //return _originRowTransformer.IsValid(sourceRow);
            }
            else
            {
                retValue = base.IsValid(sourceRow, out errorMsg);
                return retValue;
                //return base.IsValid(sourceRow);
            }
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
                                        GISSAPIntegrator._logger.Info("Related Feature Found OID :: "+ deleteFeat.OID+" of Feature Class :: "+ ((IDataset)relationshipClass.DestinationClass).Name.ToUpper());
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
                                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                                catch (Exception ex)
                                {
                                    _errorMessage.AppendLine("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                    GISSAPIntegrator._logger.Info("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
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

                                    GISSAPIntegrator._logger.Info("Related Feature Found OID :: " + feat.OID + " of Feature Class :: " + ((IDataset)relationshipClass.DestinationClass).Name.ToUpper());

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

                            #region Commented
                            //if (changedFeatures.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                            //{
                            //    if (changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Delete.Count > 0)
                            //    {
                            //        if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                            //        {
                            //            try
                            //            {
                            //                #region Commented
                            //                //if (changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Delete.Any(str => str.fields_Old[fieldOriginPrimaryKey] == row.fields_Old[fieldOriginForeignKey]))
                            //                //{
                            //                //    foreach (DeleteFeat deleteFeat in changedFeatures[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Action.Delete.Where(str => str.fields_Old[fieldOriginPrimaryKey] == row.fields_Old[fieldOriginForeignKey]))
                            //                //    {
                            //                //        if (deleteFeat != null)
                            //                //        {
                            //                //            //_logger.Debug("Found Related Object [ " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() +
                            //                //            //          " ] OID [ " + deleteFeat.OID + " ]");

                            //                //            if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()))
                            //                //            {
                            //                //                relatedRow.Add(((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper(), new List<DeleteFeat>());
                            //                //            }

                            //                //            if (!relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Any(str => str.OID == deleteFeat.OID))
                            //                //            {
                            //                //                deleteFeat.Table = (ITable)relationshipClass.OriginClass;
                            //                //                deleteFeat.ChangeFeatures = row.ChangeFeatures;
                            //                //                relatedRow[((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper()].Add(deleteFeat);
                            //                //            }


                            //                //        }
                            //                //    }
                            //                //}
                            //                #endregion
                            //            }
                            //            catch
                            //            {
                            //                //_logger.Debug(fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper());
                            //                //_logger.Debug(fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper());
                            //            }
                            //        }
                            //    }



                            //}
                            #endregion

                            if (!string.IsNullOrEmpty(fieldOriginForeignKey) && !string.IsNullOrEmpty(fieldOriginPrimaryKey))
                            {
                                try
                                {
                                    foreach (DeleteFeat deleteFeat in dBHelper.GetRelatedCD(((IDataset)relationshipClass.OriginClass).Name.ToUpper(), fieldOriginPrimaryKey, row.fields_Old[fieldOriginForeignKey], (IDataset)relationshipClass.OriginClass))
                                    {
                                        if (deleteFeat != null)
                                        {
                                            GISSAPIntegrator._logger.Info("Related Feature Found OID :: " + deleteFeat.OID + " of Feature Class :: " + ((IDataset)relationshipClass.OriginClass).Name.ToUpper());
                                            //_logger.Debug("Found Related Object [ " + relationshipClass.DestinationClass.AliasName.ToUpper() +
                                            //          " ] OID [ " + deleteFeat.OID + " ]");
                                            if (!relatedRow.ContainsKey(((IDataset)relationshipClass.OriginClass).Name.ToUpper()))
                                            {
                                                relatedRow.Add(((IDataset)relationshipClass.OriginClass).Name.ToUpper(), new List<DeleteFeat>());
                                            }


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
                                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
                                catch (Exception ex)
                                {
                                    _errorMessage.AppendLine("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                    GISSAPIntegrator._logger.Info("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
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

                                    GISSAPIntegrator._logger.Info("Related Feature Found OID :: " + feat.OID + " of Feature Class :: " + ((IDataset)relationshipClass.OriginClass).Name.ToUpper());

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
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); if (GISSAPIntegrator.HandleOracleErrorCodes || GISSAPIntegrator.OracleErrorCodes.Contains(Convert.ToString(oex.ErrorCode))) { throw oex; } }
            catch (Exception exception)
            {
                //_logger.Error(exception.ToString(), exception);
                GISSAPIntegrator._logger.Info(exception.ToString(), exception);
                GISSAPIntegrator._logger.Error(exception.ToString(), exception);
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
            GISSAPIntegrator._logger.Info("Get SpatialReference from Dataset");

            if (dataset is IGeoDataset geoDataset)
            {
                return geoDataset.SpatialReference;
            }
            else
            {
                return null; //otherwise return null
            }
        }

    }
}
