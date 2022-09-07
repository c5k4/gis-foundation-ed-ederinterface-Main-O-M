using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using PGE.Common.Delivery.Framework;

using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Interfaces.Integration.Framework;
using PGE.Common.ChangeDetectionAPI;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Process Bank Device equipments, including VoltageRegulator, Transformer and Stepdown, that are part of a composite asset row.
    /// Bank Device do not determine a composite asset row's Action, AssetID,
    /// but determine its SAPType (DeviceEquipment when related o Structure or DeviceSubEquipment when related to DeviceGroup).
    /// </summary>
    public class SAPBankDeviceRowTransformer : SAPRowTransformer
    {
        private int? _parentIDFieldIndex;

        /// <summary>
        /// A composite asset row's SAPType is determined by its Bank Device.
        /// The bank device's parent ID is evaluated to see if the bank feature is related to a structure or device group feature. 
        /// </summary>
        /// <param name="sourceRow">Row in edit version</param>
        /// <param name="targetRow">Row in target version</param>
        /// <param name="changeType">Edit type</param>
        /// <returns>DeviceEquipment when related o Structure or DeviceSubEquipment when related to DeviceGroup</returns>
        public override SAPType GetSAPType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");
            SAPType sapType = SAPType.DeviceEquipment;

            // if a row with GlobalID = sourceRow.StructureGUID can be found from DeviceGroup table, this is sub-equipment
            string structureGUID = string.Empty;
            if (changeType == ChangeType.Delete)
            {
                structureGUID = GetParentID(targetRow);
            }
            else
            {
                structureGUID = GetParentID(sourceRow);
            }

            if (string.IsNullOrEmpty(structureGUID) == false)
            {
                if (_systemMapper.DeviceGroupGlobalIDs.Contains(structureGUID))
                {
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                    sapType = SAPType.DeviceSubEquipment;
                }
                else
                {
                    string relationNameWithDeviceGroup = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithDeviceGroup"];
                    ISet relatedObjects = null;
                    if (changeType == ChangeType.Delete)
                    {
                        relatedObjects = GetTargetRelatedObjects(targetRow, relationNameWithDeviceGroup);
                    }
                    else
                    {
                        relatedObjects = GetSourceRelatedObjects(sourceRow, relationNameWithDeviceGroup);
                    }

                    if (relatedObjects != null && relatedObjects.Count > 0)
                    {
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                        sapType = SAPType.DeviceSubEquipment;
                        _systemMapper.DeviceGroupGlobalIDs.Add(structureGUID);
                    }
                }
            }

            return sapType;
        }

        /// <summary>
        /// A composite asset row's SAPType is determined by its Bank Device.
        /// The bank device's parent ID is evaluated to see if the bank feature is related to a structure or device group feature. 
        /// </summary>
        /// <param name="sourceRow">Row in edit version</param>
        /// <param name="targetRow">Row in target version</param>
        /// <param name="changeType">Edit type</param>
        /// <returns>DeviceEquipment when related o Structure or DeviceSubEquipment when related to DeviceGroup</returns>
        public override SAPType GetSAPType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");
            SAPType sapType = SAPType.DeviceEquipment;

            // if a row with GlobalID = sourceRow.StructureGUID can be found from DeviceGroup table, this is sub-equipment
            string structureGUID = string.Empty;
            if (changeType == ChangeType.Delete)
            {
                structureGUID = GetParentID(targetRow);
            }
            else
            {
                structureGUID = GetParentID(sourceRow);
            }

            if (string.IsNullOrEmpty(structureGUID) == false)
            {
                if (_systemMapper.DeviceGroupGlobalIDs.Contains(structureGUID))
                {
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                    sapType = SAPType.DeviceSubEquipment;
                }
                else
                {
                    string relationNameWithDeviceGroup = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithDeviceGroup"];
                    ISet relatedObjects = null;
                    //if (changeType == ChangeType.Delete)
                    //{
                    //    relatedObjects = GetTargetRelatedObjects(targetRow, relationNameWithDeviceGroup);
                    //}
                    //else
                    {
                        relatedObjects = GetSourceRelatedObjects(sourceRow, relationNameWithDeviceGroup);
                    }

                    if (relatedObjects != null && relatedObjects.Count > 0)
                    {
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                        sapType = SAPType.DeviceSubEquipment;
                        _systemMapper.DeviceGroupGlobalIDs.Add(structureGUID);
                    }
                }
            }

            return sapType;
        }

        /// <summary>
        /// A composite asset row's SAPType is determined by its Bank Device.
        /// The bank device's parent ID is evaluated to see if the bank feature is related to a structure or device group feature. 
        /// </summary>
        /// <param name="sourceRow">Row in edit version</param>
        /// <param name="targetRow">Row in target version</param>
        /// <param name="changeType">Edit type</param>
        /// <returns>DeviceEquipment when related o Structure or DeviceSubEquipment when related to DeviceGroup</returns>
        public override SAPType GetSAPType(DeleteFeat sourceRow, ITable FCName, ChangeType changeType) // Add ITable
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");
            SAPType sapType = SAPType.DeviceEquipment;

            // if a row with GlobalID = sourceRow.StructureGUID can be found from DeviceGroup table, this is sub-equipment
            string structureGUID = string.Empty;
            
            structureGUID = GetParentID(sourceRow, FCName);
            
            if (string.IsNullOrEmpty(structureGUID) == false)
            {
                if (_systemMapper.DeviceGroupGlobalIDs.Contains(structureGUID))
                {
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                    sapType = SAPType.DeviceSubEquipment;
                }
                else
                {
                    //V3SF - Related Record - Default 0000
                    //sapType = SAPType.DeviceSubEquipment;
                    //string relationNameWithDeviceGroup = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithDeviceGroup"];
                    //ISet relatedObjects = null;
                    Dictionary<string, List<DeleteFeat>> RelatedDeletedFeat = new Dictionary<string, List<DeleteFeat>>();
                    List<string> relatedClasses = new List<string>();

                    if (_trackedClass.RelatedClass != null)
                    {
                        if (_trackedClass.RelatedClass.QualifiedSourceClass != null)
                        {
                            relatedClasses.Add(_trackedClass.RelatedClass.QualifiedSourceClass.ToUpper());
                        }
                    }

                    RelatedDeletedFeat = GetRelatedRows(sourceRow.Table, sourceRow, relatedClasses, sourceRow.ChangeFeatures);
                    //relatedObjects = GetSourceRelatedObjects(sourceRow, relationNameWithDeviceGroup);

                    //if (relatedObjects != null && relatedObjects.Count > 0)
                    //{
                    //    sapType = SAPType.DeviceSubEquipment;
                    //    _systemMapper.DeviceGroupGlobalIDs.Add(structureGUID);
                    //}

                    if(RelatedDeletedFeat != null && RelatedDeletedFeat.Count>0)
                    {
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                        sapType = SAPType.DeviceSubEquipment;
                        _systemMapper.DeviceGroupGlobalIDs.Add(structureGUID);
                    }
                }
            }

            return sapType;
        }

        private string GetParentID(IRow row)
        {
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting ParentID");
            if (_parentIDFieldIndex == null)
            {
                Settings settings = _trackedClass.Settings;
                if (settings != null)
                {
                    string parentIDFieldName = settings["ParentIDFieldName"];
                    if (string.IsNullOrEmpty(parentIDFieldName) == false)
                    {
                        ITable tbl = row.Table;
                        _parentIDFieldIndex = BaseRowTransformer.GetFieldIndex((IObjectClass)tbl, parentIDFieldName);
                    }
                }
            }

            if (_parentIDFieldIndex == null || _parentIDFieldIndex == -1)
            {
                string msg = "ParentIDFieldName is not correct for " + _trackedClass.OutName + " " + _trackedClass.SourceClass;
                GISSAPIntegrator._logger.Info("Error OID :: " + row.OID + " " + msg);
                InvalidConfigurationException exception = new InvalidConfigurationException(msg);
                throw exception;
            }

            object tmp = row.get_Value((int)_parentIDFieldIndex);

            string parentID = string.Empty;

            if (tmp != null && tmp != DBNull.Value)
            {
                parentID = Convert.ToString(tmp);
            }

            if (string.IsNullOrWhiteSpace(parentID))
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " ParentID not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

            return parentID;
        }

        private string GetParentID(UpdateFeat row)
        {
            object tmp = default;
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting ParentID");

            Settings settings = _trackedClass.Settings;
            if (settings != null)
            {
                string parentIDFieldName = settings["ParentIDFieldName"];
                if (row.fields_Old.ContainsKey(parentIDFieldName.ToUpper()))
                {
                    tmp = row.fields_Old[parentIDFieldName.ToUpper()];
                }
                else
                {
                    string msg = "ParentIDFieldName is not correct for " + _trackedClass.OutName + " " + _trackedClass.SourceClass;
                    GISSAPIntegrator._logger.Info("Error OID :: " + row.OID + " " + msg);
                    InvalidConfigurationException exception = new InvalidConfigurationException(msg);
                    throw exception;
                }
            }
            string parentID = string.Empty;

            if (tmp != null && tmp != DBNull.Value)
            {
                parentID = Convert.ToString(tmp);
            }

            if (string.IsNullOrWhiteSpace(parentID))
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " ParentID not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

            return parentID;
        }

        private string GetParentID(DeleteFeat row, ITable FCName)
        {
            object tmp = default;
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting ParentID");

            Settings settings = _trackedClass.Settings;
            if (settings != null)
            {
                string parentIDFieldName = settings["ParentIDFieldName"];
                if (row.fields_Old.ContainsKey(parentIDFieldName.ToUpper()))
                {
                    tmp = row.fields_Old[parentIDFieldName.ToUpper()];
                }
                else
                {
                    string msg = "ParentIDFieldName is not correct for " + _trackedClass.OutName + " " + _trackedClass.SourceClass;
                    GISSAPIntegrator._logger.Info("Error OID :: " + row.OID + " " + msg);
                    InvalidConfigurationException exception = new InvalidConfigurationException(msg);
                    throw exception;
                }
            }
            string parentID = string.Empty;

            if (tmp != null && tmp != DBNull.Value)
            {
                parentID = Convert.ToString(tmp);
            }

            if (string.IsNullOrWhiteSpace(parentID))
            {
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " ParentID not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info(row.OID + " ParentID not found ");
                GISSAPIntegrator._logger.Error(row.OID + " ParentID not found ");
            }

                return parentID;
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
                                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                                catch (Exception ex)
                                {
                                    _errorMessage.AppendLine("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                    GISSAPIntegrator._logger.Info("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error: " + fieldOriginForeignKey + " not found in " + ((IDataset)relationshipClass.DestinationClass).BrowseName.ToUpper() + " " + ex.Message);
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
                                catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
                                catch (Exception ex)
                                {
                                    _errorMessage.AppendLine("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                                    GISSAPIntegrator._logger.Info("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
                                    GISSAPIntegrator._logger.Error("Error: " + fieldOriginPrimaryKey + " not found in " + ((IDataset)relationshipClass.OriginClass).BrowseName.ToUpper() + " " + ex.Message);
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
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
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
