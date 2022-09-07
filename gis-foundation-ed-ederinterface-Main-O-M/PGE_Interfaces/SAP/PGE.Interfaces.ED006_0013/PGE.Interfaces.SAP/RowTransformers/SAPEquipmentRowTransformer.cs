using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangeDetectionAPI;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Process Unit equipments that determine AssetID, Action of a composite asset row.
    /// This class works with other RowTransformers configured for related Bank Device, Controller rows.
    /// </summary>
    public class SAPEquipmentRowTransformer : SAPRowTransformer
    {
        private int? _assetIDFieldIndex;
        private int? _statusFieldIndex;
        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData. Uses base class for most processing
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            List<IRowData> sapRowDataList = new List<IRowData>();

            IRow activeRow = sourceRow;
            if (changeType == ChangeType.Delete)
            {
                activeRow = targetRow;
            }
            GISSAPIntegrator._logger.Info("Processing OID :: "+ activeRow.OID);

            string outName = _trackedClass.OutName;
            Dictionary<string, Dictionary<string, IRowData>> assetIDandRowDataByOutName = _systemMapper.AssetIDandRowDataByOutName;

            if (assetIDandRowDataByOutName.ContainsKey(outName))
            {
                Dictionary<string, IRowData> assetIDandRowData = assetIDandRowDataByOutName[outName];
                string assetID = GetAssetID(activeRow);
                if (assetIDandRowData.ContainsKey(assetID))
                {
                    GISSAPIntegrator._logger.Info("Already Processed OID :: " + activeRow.OID);
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " Already Processed " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    return sapRowDataList;
                }
            }

            sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
            if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
            {
                GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                GISSAPIntegrator._logger.Error("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
            }

            foreach (IRowData rowData in sapRowDataList)
            {
                if (rowData.FieldValues.ContainsKey(_trackedClass.AssetIDField))
                {
                    rowData.AssetID = rowData.FieldValues[_trackedClass.AssetIDField];
                }
                else
                {
                    throw new InvalidConfigurationException(String.Format("{0} is configured for Asset ID, but no Asset ID was found", _trackedClass.SourceClass));
                }
            }

            return sapRowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData. Uses base class for most processing
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            List<IRowData> sapRowDataList = new List<IRowData>();

            IRow activeRow = sourceRow;
            //if (changeType == ChangeType.Delete)
            //{
            //    activeRow = targetRow;
            //}
            GISSAPIntegrator._logger.Info("Processing OID :: "+ activeRow.OID);

            string outName = _trackedClass.OutName;
            Dictionary<string, Dictionary<string, IRowData>> assetIDandRowDataByOutName = _systemMapper.AssetIDandRowDataByOutName;

            if (assetIDandRowDataByOutName.ContainsKey(outName))
            {
                Dictionary<string, IRowData> assetIDandRowData = assetIDandRowDataByOutName[outName];
                string assetID = GetAssetID(activeRow);
                if (assetIDandRowData.ContainsKey(assetID))
                {
                    GISSAPIntegrator._logger.Info("Already Processed OID :: " + activeRow.OID);
                    GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " Already Processed " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    return sapRowDataList;
                }
            }

            sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
            if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
            {
                GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                GISSAPIntegrator._logger.Error("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
            }

            foreach (IRowData rowData in sapRowDataList)
            {
                if (rowData.FieldValues.ContainsKey(_trackedClass.AssetIDField))
                {
                    rowData.AssetID = rowData.FieldValues[_trackedClass.AssetIDField];
                }
                else
                {
                    throw new InvalidConfigurationException(String.Format("{0} is configured for Asset ID, but no Asset ID was found", _trackedClass.SourceClass));
                }
            }

            return sapRowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData. Uses base class for most processing
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(DeleteFeat sourceRow, ChangeType changeType)
        {
            List<IRowData> sapRowDataList = new List<IRowData>();
            
            //dynamic activeRow = sourceRow;
            //if (changeType == ChangeType.Delete)
            //{
            //    activeRow = targetRow;
            //}
            GISSAPIntegrator._logger.Info("Processing OID :: "+ sourceRow.OID);


            string outName = _trackedClass.OutName;
            Dictionary<string, Dictionary<string, IRowData>> assetIDandRowDataByOutName = _systemMapper.AssetIDandRowDataByOutName;

            if (assetIDandRowDataByOutName.ContainsKey(outName))
            {
                Dictionary<string, IRowData> assetIDandRowData = assetIDandRowDataByOutName[outName];
                string assetIDFieldName = _trackedClass.Settings["AssetIDFieldName"];

                if (sourceRow.fields_Old.ContainsKey(assetIDFieldName))
                {
                    //string assetID = GetAssetID(activeRow);
                    string assetID = sourceRow.fields_Old[assetIDFieldName];
                    if (assetIDandRowData.ContainsKey(assetID))
                    {
                        GISSAPIntegrator._logger.Info("Already Processed OID :: " + sourceRow.OID);
                        GISSAPIntegrator._errorMessage.AppendLine(sourceRow.OID + " Already Processed " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        return sapRowDataList;
                    }
                }
            }

            sapRowDataList = base.ProcessRow(sourceRow, changeType);
            if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                GISSAPIntegrator._logger.Error("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                }

            foreach (IRowData rowData in sapRowDataList)
            {
                if (rowData.FieldValues.ContainsKey(_trackedClass.AssetIDField))
                {
                    rowData.AssetID = rowData.FieldValues[_trackedClass.AssetIDField];
                }
                else
                {
                    throw new InvalidConfigurationException(String.Format("{0} is configured for Asset ID, but no Asset ID was found", _trackedClass.SourceClass));
                }
            }

            return sapRowDataList;
        }

        /// <summary>
        /// Determine what action SAP should take based on how the row was edited
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP action type</returns>
        public override ActionType GetActionType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            ActionType action = ActionType.Invalid;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Invalid");


            // previous/pre-post
            int preStatus = -1;

            // current
            int currentStatus = -1;

            if (changeType == ChangeType.Insert)
            {
                currentStatus = GetStatus(sourceRow);
                if (currentStatus == (int)ConstructionStatus.InService)
                {
                    action = ActionType.Insert;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Insert");

                }
                else if (currentStatus == (int)ConstructionStatus.ProposedInstall)
                {
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");

                }
                else if (currentStatus == (int)ConstructionStatus.Idle )
                {
                    action = ActionType.Idle ;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");

                }
            }
            else if (changeType == ChangeType.Delete)
            {
                preStatus = GetStatus(targetRow);

                if (preStatus == (int)ConstructionStatus.ProposedChange)
                {
                    // ideally, search for its replacing object (ReplacedObjectGUID = GUID of this object),
                    // if found, and if the replacing object has status In Service, this is Delete part of a valid replacement
                    action = ActionType.Delete;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");


                    // if the replacing object has current status Proposed Install, it's an error
                }
                else if (preStatus == (int)ConstructionStatus.InService)
                {
                    action = ActionType.Delete;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");

                }
                else if (preStatus == (int)ConstructionStatus.Idle )
                {
                    action = ActionType.Delete;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");
                }
                else if (preStatus == (int)ConstructionStatus.ProposedInstall)
                {
                    //Change for Remedy Incident INC000003740626 unable to delete a 
                    //VoltageRegulatorUnit with status 'Proposed Install' since premap 
                    //features should not be in SAP set the ActionType to delete should 
                    //not cause a problem from the SAP side and should allow jobs with 
                    //this type of edit to Post successfully 
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");
                }
            }
            else //Update
            {
                currentStatus = GetStatus(sourceRow);
                preStatus = GetStatus(targetRow);

                if (currentStatus == (int)ConstructionStatus.InService)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedInstall)
                    {
                        // this could be a new install or 
                        // the install part of replacement if its ReplaceGUID = ObjectID of a previously ProposedChange and currently deleted
                        action = ActionType.Insert;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Insert");
                    }
                    else if (preStatus == (int)ConstructionStatus.ProposedRemove)
                    {
                        // can we really assume that no attributes were updated?
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");
                    }
                    else if (preStatus == (int)ConstructionStatus.ProposedChange)
                    {
                        // this is the replaced object (ObjectID = ReplaceGUID of another current In Service and previously Proposed Install)
                        // with a wrong current status, it's an error, it shall have been deleted
                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");
                    }
                    else if (preStatus == (int)ConstructionStatus.Idle )
                    {
                        action = ActionType.Update ;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");
                    }
                }
                else if (currentStatus == (int)ConstructionStatus.Deactivated)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedDeactivated)
                    {
                        action = ActionType.Delete;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");
                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Delete;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");
                    }
                }
                else if (currentStatus == (int)ConstructionStatus.Idle)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedDeactivated)
                    {
                        action = ActionType.Idle ;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                    }
                    else if (preStatus == (int)ConstructionStatus.Idle)
                    {
                        action = ActionType.Idle ;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                    }
                    else if (preStatus == (int)ConstructionStatus.InService )
                    {
                        action = ActionType.Idle;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                    }
                }
                else if (currentStatus == (int)ConstructionStatus.ProposedDeactivated ||
                    currentStatus == (int)ConstructionStatus.ProposedChange ||
                    currentStatus == (int)ConstructionStatus.ProposedRemove||
                    currentStatus ==(int)ConstructionStatus .ProposedInstall )

                {
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");
                }
            }

            return action;
        }

        /// <summary>
        /// Determine what action SAP should take based on how the row was edited
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP action type</returns>
        public override ActionType GetActionType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            ActionType action = ActionType.Invalid;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Invalid");


            // previous/pre-post
            int preStatus = -1;

            // current
            int currentStatus = -1;

            if (changeType == ChangeType.Insert)
            {
                currentStatus = GetStatus(sourceRow);
                if (currentStatus == (int)ConstructionStatus.InService)
                {
                    action = ActionType.Insert;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Insert");

                }
                else if (currentStatus == (int)ConstructionStatus.ProposedInstall)
                {
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");

                }
                else if (currentStatus == (int)ConstructionStatus.Idle)
                {
                    action = ActionType.Idle;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");

                }
            }
            //else if (changeType == ChangeType.Delete)
            //{
            //    preStatus = GetStatus(targetRow);

            //    if (preStatus == (int)ConstructionStatus.ProposedChange)
            //    {
            //        // ideally, search for its replacing object (ReplacedObjectGUID = GUID of this object),
            //        // if found, and if the replacing object has status In Service, this is Delete part of a valid replacement
            //        action = ActionType.Delete;

            //        // if the replacing object has current status Proposed Install, it's an error
            //    }
            //    else if (preStatus == (int)ConstructionStatus.InService)
            //    {
            //        action = ActionType.Delete;
            //    }
            //    else if (preStatus == (int)ConstructionStatus.Idle)
            //    {
            //        action = ActionType.Delete;
            //    }
            //    else if (preStatus == (int)ConstructionStatus.ProposedInstall)
            //    {
            //        //Change for Remedy Incident INC000003740626 unable to delete a 
            //        //VoltageRegulatorUnit with status 'Proposed Install' since premap 
            //        //features should not be in SAP set the ActionType to delete should 
            //        //not cause a problem from the SAP side and should allow jobs with 
            //        //this type of edit to Post successfully 
            //        action = ActionType.PreMap;
            //    }
            //}
            else //Update
            {
                currentStatus = GetStatus(sourceRow);
                preStatus = GetStatus(targetRow);

                if (currentStatus == (int)ConstructionStatus.InService)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedInstall)
                    {
                        // this could be a new install or 
                        // the install part of replacement if its ReplaceGUID = ObjectID of a previously ProposedChange and currently deleted
                        action = ActionType.Insert;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Insert");

                    }
                    else if (preStatus == (int)ConstructionStatus.ProposedRemove)
                    {
                        // can we really assume that no attributes were updated?
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");

                    }
                    else if (preStatus == (int)ConstructionStatus.ProposedChange)
                    {
                        // this is the replaced object (ObjectID = ReplaceGUID of another current In Service and previously Proposed Install)
                        // with a wrong current status, it's an error, it shall have been deleted
                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");

                    }
                    else if (preStatus == (int)ConstructionStatus.Idle)
                    {
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");

                    }
                }
                else if (currentStatus == (int)ConstructionStatus.Deactivated)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedDeactivated)
                    {
                        action = ActionType.Delete;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");

                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Delete;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");

                    }
                }
                else if (currentStatus == (int)ConstructionStatus.Idle)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedDeactivated)
                    {
                        action = ActionType.Idle;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                    }
                    else if (preStatus == (int)ConstructionStatus.Idle)
                    {
                        action = ActionType.Idle;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");

                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Idle;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");

                    }
                }
                else if (currentStatus == (int)ConstructionStatus.ProposedDeactivated ||
                    currentStatus == (int)ConstructionStatus.ProposedChange ||
                    currentStatus == (int)ConstructionStatus.ProposedRemove ||
                    currentStatus == (int)ConstructionStatus.ProposedInstall)

                {
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");

                }
            }

            return action;
        }

        /// <summary>
        /// Determine what action SAP should take based on how the row was edited
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP action type</returns>
        public override ActionType GetActionType(DeleteFeat sourceRow, ChangeType changeType)
        {
            ActionType action = ActionType.Invalid;
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Invalid");

            // previous/pre-post
            int preStatus = -1;

            // current
            int currentStatus = -1;

            if (changeType == ChangeType.Insert)
            {
                currentStatus = GetStatus(sourceRow);
                if (currentStatus == (int)ConstructionStatus.InService)
                {
                    action = ActionType.Insert;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Insert");
                }
                else if (currentStatus == (int)ConstructionStatus.ProposedInstall)
                {
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");
                }
                else if (currentStatus == (int)ConstructionStatus.Idle)
                {
                    action = ActionType.Idle;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                }
            }
            else if (changeType == ChangeType.Delete)
            {
                preStatus = GetStatus(sourceRow);

                if (preStatus == (int)ConstructionStatus.ProposedChange)
                {
                    // ideally, search for its replacing object (ReplacedObjectGUID = GUID of this object),
                    // if found, and if the replacing object has status In Service, this is Delete part of a valid replacement
                    action = ActionType.Delete;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");

                    // if the replacing object has current status Proposed Install, it's an error
                }
                else if (preStatus == (int)ConstructionStatus.InService)
                {
                    action = ActionType.Delete;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");
                }
                else if (preStatus == (int)ConstructionStatus.Idle)
                {
                    action = ActionType.Delete;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");
                }
                else if (preStatus == (int)ConstructionStatus.ProposedInstall)
                {
                    //Change for Remedy Incident INC000003740626 unable to delete a 
                    //VoltageRegulatorUnit with status 'Proposed Install' since premap 
                    //features should not be in SAP set the ActionType to delete should 
                    //not cause a problem from the SAP side and should allow jobs with 
                    //this type of edit to Post successfully 
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");
                }
            }
            else //Update
            {
                currentStatus = GetStatus(sourceRow);
                preStatus = GetStatus(sourceRow);

                if (currentStatus == (int)ConstructionStatus.InService)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedInstall)
                    {
                        // this could be a new install or 
                        // the install part of replacement if its ReplaceGUID = ObjectID of a previously ProposedChange and currently deleted
                        action = ActionType.Insert;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Insert");
                    }
                    else if (preStatus == (int)ConstructionStatus.ProposedRemove)
                    {
                        // can we really assume that no attributes were updated?
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");
                    }
                    else if (preStatus == (int)ConstructionStatus.ProposedChange)
                    {
                        // this is the replaced object (ObjectID = ReplaceGUID of another current In Service and previously Proposed Install)
                        // with a wrong current status, it's an error, it shall have been deleted
                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");
                    }
                    else if (preStatus == (int)ConstructionStatus.Idle)
                    {
                        action = ActionType.Update;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Update");
                    }
                }
                else if (currentStatus == (int)ConstructionStatus.Deactivated)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedDeactivated)
                    {
                        action = ActionType.Delete;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");
                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Delete;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Delete");
                    }
                }
                else if (currentStatus == (int)ConstructionStatus.Idle)
                {
                    if (preStatus == (int)ConstructionStatus.ProposedDeactivated)
                    {
                        action = ActionType.Idle;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                    }
                    else if (preStatus == (int)ConstructionStatus.Idle)
                    {
                        action = ActionType.Idle;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                    }
                    else if (preStatus == (int)ConstructionStatus.InService)
                    {
                        action = ActionType.Idle;
                        GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: Idle");
                    }
                }
                else if (currentStatus == (int)ConstructionStatus.ProposedDeactivated ||
                    currentStatus == (int)ConstructionStatus.ProposedChange ||
                    currentStatus == (int)ConstructionStatus.ProposedRemove ||
                    currentStatus == (int)ConstructionStatus.ProposedInstall)

                {
                    action = ActionType.PreMap;
                    GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " Action Type :: PreMap");
                }
            }

            return action;
        }

        private string GetAssetID(IRow row)
        {
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting AssetID");

            if (_assetIDFieldIndex == null)
            {
                string assetIDFieldName = _trackedClass.Settings["AssetIDFieldName"];
                ITable tbl = row.Table;
                _assetIDFieldIndex = GetFieldIndex((IObjectClass)tbl, assetIDFieldName);
            }

            object tmp = row.get_Value((int)_assetIDFieldIndex);
            string assetID = string.Empty;
            if (tmp != null & tmp != DBNull.Value)
            {
                assetID = Convert.ToString(tmp);
            }

            if (string.IsNullOrWhiteSpace(assetID))
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " AssetID not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

            return assetID;
        }

        

        private int GetStatus(IRow row)
        {
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting Status");
            if (_statusFieldIndex == null)
            {
                string assetStatusFieldName = _trackedClass.Settings["AssetStatusFieldName"];
                if (string.IsNullOrEmpty(assetStatusFieldName) == false)
                {
                    ITable tbl = row.Table;
                    _statusFieldIndex = GetFieldIndex((IObjectClass)tbl, assetStatusFieldName);
                }
            }

            int status = -1;
            if (_statusFieldIndex != null && _statusFieldIndex != -1)
            {
                object tmp = row.get_Value((int)_statusFieldIndex);
                if (tmp != null && tmp != DBNull.Value)
                {
                    status = Convert.ToInt32(tmp);
                }
            }

            if (status == -1)
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " Asset Status not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

            return status;
        }

        private int GetStatus(UpdateFeat row)
        {
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting Status");
            int status = -1;
            string assetStatusFieldName = _trackedClass.Settings["AssetStatusFieldName"];
            if (row.fields_Old.ContainsKey(assetStatusFieldName.ToUpper()))
            {
                object tmp = row.fields_Old[assetStatusFieldName.ToUpper()];
                if (tmp != null && tmp != DBNull.Value)
                {
                    status = Convert.ToInt32(tmp);
                }
            }

            if (status == -1)
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " Asset Status not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");

            return status;

        }

        private int GetStatus(DeleteFeat row)
        {
            GISSAPIntegrator._logger.Info("OID :: " + row.OID + " Getting Status");
            int status = -1;
            string assetStatusFieldName = _trackedClass.Settings["AssetStatusFieldName"];
            if (row.fields_Old.ContainsKey(assetStatusFieldName.ToUpper()))
            {
                object tmp = row.fields_Old[assetStatusFieldName.ToUpper()];
                if (tmp != null && tmp != DBNull.Value)
                {
                    status = Convert.ToInt32(tmp);
                }
            }

            if (status == -1)
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " Asset Status not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");


            return status;
        }
    }
}
