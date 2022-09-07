using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;

using PGE.Common.Delivery.Framework;

using ESRI.ArcGIS.esriSystem;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Interfaces.Integration.Framework;
using PGE.Common.ChangeDetectionAPI;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Process Device equipments that are not modeled to have related Units.
    /// Device equipments' SAPType is DeviceEquipment or DeviceSubEquipment, and include DeviceGroup, CapacitorBank, 
    /// Interruper, Recloser, Sectionalizer, Switch, StreetLight, FaultIndicator, Elbow etc.
    /// 
    /// DeviceGroup and StreetLight are always DeviceEquipment.
    /// </summary>
    public class SAPDeviceRowTransformer : SAPEquipmentRowTransformer
    {
        private int? _parentIDFieldIndex;

        /// <summary>
        /// The device's parent ID is evaluated to see if the device feature is related to a structure or device group feature.
        /// This works together with a class' configuration setting.
        /// If a ParentIDFieldName is not given, the evaluation is not done and the device is considered DeviceEquipment
        /// </summary>
        /// <param name="sourceRow">Row in edit version</param>
        /// <param name="targetRow">Row in target version</param>
        /// <param name="changeType">Edit type</param>
        /// <returns>DeviceEquipment when related o Structure or DeviceSubEquipment when related to DeviceGroup</returns>
        public override SAPType GetSAPType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            SAPType sapType = SAPType.DeviceEquipment;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");

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
                    sapType = SAPType.DeviceSubEquipment;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
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
                        sapType = SAPType.DeviceSubEquipment;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                        _systemMapper.DeviceGroupGlobalIDs.Add(structureGUID);
                    }
                }
            }

            return sapType;
        }

        /// <summary>
        /// The device's parent ID is evaluated to see if the device feature is related to a structure or device group feature.
        /// This works together with a class' configuration setting.
        /// If a ParentIDFieldName is not given, the evaluation is not done and the device is considered DeviceEquipment
        /// </summary>
        /// <param name="sourceRow">Row in edit version</param>
        /// <param name="targetRow">Row in target version</param>
        /// <param name="changeType">Edit type</param>
        /// <returns>DeviceEquipment when related o Structure or DeviceSubEquipment when related to DeviceGroup</returns>
        public override SAPType GetSAPType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            SAPType sapType = SAPType.DeviceEquipment;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");

            // if a row with GlobalID = sourceRow.StructureGUID can be found from DeviceGroup table, this is sub-equipment
            string structureGUID = string.Empty;
            //if (changeType == ChangeType.Delete)
            //{
            //    structureGUID = GetParentID(targetRow);
            //}
            //else
            {
                structureGUID = GetParentID(sourceRow);
            }

            if (string.IsNullOrEmpty(structureGUID) == false)
            {
                if (_systemMapper.DeviceGroupGlobalIDs.Contains(structureGUID))
                {
                    sapType = SAPType.DeviceSubEquipment;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
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
                        sapType = SAPType.DeviceSubEquipment;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                        _systemMapper.DeviceGroupGlobalIDs.Add(structureGUID);
                    }
                }
            }

            return sapType;
        }

        /// <summary>
        /// The device's parent ID is evaluated to see if the device feature is related to a structure or device group feature.
        /// This works together with a class' configuration setting.
        /// If a ParentIDFieldName is not given, the evaluation is not done and the device is considered DeviceEquipment
        /// </summary>
        /// <param name="sourceRow">Row in edit version</param>
        /// <param name="targetRow">Row in target version</param>
        /// <param name="changeType">Edit type</param>
        /// <returns>DeviceEquipment when related o Structure or DeviceSubEquipment when related to DeviceGroup</returns>
        public override SAPType GetSAPType(DeleteFeat sourceRow, ITable FCName, ChangeType changeType)
        {
            SAPType sapType = SAPType.DeviceEquipment;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");

            // if a row with GlobalID = sourceRow.StructureGUID can be found from DeviceGroup table, this is sub-equipment
            string structureGUID = string.Empty;
            
            structureGUID = GetParentID(sourceRow);
            
            if (string.IsNullOrEmpty(structureGUID) == false)
            {
                if (_systemMapper.DeviceGroupGlobalIDs.Contains(structureGUID))
                {
                    sapType = SAPType.DeviceSubEquipment;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
                }
                else
                {
                    //V3SF - Related Record - Default 0000
                    //sapType = SAPType.DeviceSubEquipment;
                    //string relationNameWithDeviceGroup = _systemMapper.Settings["DataOwner"] + "." + _trackedClass.Settings["RelationshipNameWithDeviceGroup"];
                    //ISet relatedObjects = null;
                    //if (changeType == ChangeType.Delete)
                    //{
                    //    relatedObjects = GetTargetRelatedObjects(targetRow, relationNameWithDeviceGroup);
                    //}
                    //else
                    //{
                    //    relatedObjects = GetSourceRelatedObjects(sourceRow, relationNameWithDeviceGroup);
                    //}

                    //if (relatedObjects != null && relatedObjects.Count > 0)
                    //{
                    //    sapType = SAPType.DeviceSubEquipment;
                    //    _systemMapper.DeviceGroupGlobalIDs.Add(structureGUID);
                    //}

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

                    if (RelatedDeletedFeat != null && RelatedDeletedFeat.Count > 0)
                    {
                        sapType = SAPType.DeviceSubEquipment;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceSubEquipment");
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

            string parentID = string.Empty;

            if (_parentIDFieldIndex != null && _parentIDFieldIndex != -1)
            {
                object tmp = row.get_Value((int)_parentIDFieldIndex);
                if (tmp != null && tmp != DBNull.Value)
                {
                    parentID = Convert.ToString(tmp);
                }
            }

            return parentID;
        }

        private string GetParentID(DeleteFeat row)
        {
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting ParentID");
            string parentID = string.Empty;
            Settings settings = _trackedClass.Settings;
            if (settings != null)
            {
                string parentIDFieldName = settings["ParentIDFieldName"];
                if(row.fields_Old.ContainsKey(parentIDFieldName.ToUpper()))
                {
                    object tmp = row.fields_Old[parentIDFieldName.ToUpper()];
                    if (tmp != null && tmp != DBNull.Value)
                    {
                        parentID = Convert.ToString(tmp);
                    }
                }
                else
                {
                    GISSAPIntegrator._logger.Info("OID :: " + row.OID + " ParentID not found");
                }
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
