using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Framework;
using PGE.Interfaces.SAP.RowTransformers;

using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Process Controller rows.
    /// With special cashing and ordering of configured trackedClasses involving Controller,
    /// this class tries to process a changed Controller row only once or as few times as possible.
    /// 
    /// When a Controller row is related to a changed hosting  row (e.g. regulator, booster, capacitorBank etc.),
    /// it will be processed as part of its hosting row and cashed in memory so it's not necessary to process
    /// it again when Controller is configured as the root class of a TrackedClass entry in configuration file.
    /// 
    /// If a changed Controller row doesn't get processed as part of any hosting rows, it may be processed more than once
    /// until the row and its hosting row have been processed.
    /// </summary>
    public class ControllerRowTransformer : SAPEquipmentRowTransformer
    {
        /// <summary>
        /// List of object IDs that have been processed.
        /// This should be reset to empty for every edit version (instead of static constructor ) so it should be assigned a new list 
        /// only once during initialization method of the main class GISSAPIntegrator.
        /// </summary>
        public static List<int> ProcessedOIDs = new List<int>();

        [ThreadStatic]
        private static int? _deviceGUIDFieldIndex;

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns></returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            List<IRowData> sapRowDataList = new List<IRowData>();

            IRow activeRow = sourceRow;
            if (changeType == ChangeType.Delete)
            {
                activeRow = targetRow;
            }

            if (ProcessedOIDs.Contains(activeRow.OID) == false)
            {
                GISSAPIntegrator._logger.Info("Processing OID :: " + activeRow.OID);

                sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                }


                if (_trackedClass.RelatedClass == null)
                {
                    ProcessedOIDs.Add(activeRow.OID);
                }
                else
                {
                    GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " getting GUID");
                    string deviceGUID = GetDeviceGUID(activeRow);
                    if (string.IsNullOrEmpty(deviceGUID))
                    {
                        ProcessedOIDs.Add(activeRow.OID);
                        GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " got GUID :: " + deviceGUID);
                    }
                }
            }
            else
            {
                //xwu return data even if the activeRow has been processed to test against 'add new capacitor with new controller'
                GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);
                sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));

                }

                Settings settings = _trackedClass.Settings;
                if (settings != null && settings["ProcessOnceAsRelatedRow"] == "False")
                {
                    GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);
                    sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    }

                }
            }

            
            int customerOwnedSequence = GetCustomerOwnedFieldSequence();
            if (customerOwnedSequence != -1)
            {
                GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " Checking Customer Owned");
                List<IRowData> processedSapRowDataList = new List<IRowData>();

                foreach (IRowData rowData in sapRowDataList)
                {
                    string value = rowData.FieldValues[customerOwnedSequence];
                    if (value != "Y")
                    {
                        GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " Customer Owned Field Removed from Sequence");
                        rowData.FieldValues.Remove(customerOwnedSequence);
                        processedSapRowDataList.Add(rowData);
                    }
                    
                }

                return processedSapRowDataList;
            }
            else
            {
                return sapRowDataList;
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
            List<IRowData> sapRowDataList = new List<IRowData>();

            IRow activeRow = sourceRow;
            //if (changeType == ChangeType.Delete)
            //{
            //    activeRow = targetRow;
            //}

            if (ProcessedOIDs.Contains(activeRow.OID) == false)
            {
                GISSAPIntegrator._logger.Info("Processing OID :: " + activeRow.OID);

                sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                }


                if (_trackedClass.RelatedClass == null)
                {
                    ProcessedOIDs.Add(activeRow.OID);
                }
                else
                {
                    GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " getting GUID");
                    string deviceGUID = GetDeviceGUID(activeRow);
                    if (string.IsNullOrEmpty(deviceGUID))
                    {
                        GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " got GUID :: " + deviceGUID);
                        ProcessedOIDs.Add(activeRow.OID);
                    }
                }
            }
            else
            {
                //xwu return data even if the activeRow has been processed to test against 'add new capacitor with new controller'
                GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);
                sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                }

                    Settings settings = _trackedClass.Settings;
                if (settings != null && settings["ProcessOnceAsRelatedRow"] == "False")
                {
                    GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);
                    sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    }

                    }
            }


            int customerOwnedSequence = GetCustomerOwnedFieldSequence();
            if (customerOwnedSequence != -1)
            {
                GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " Checking Customer Owned");
                List<IRowData> processedSapRowDataList = new List<IRowData>();

                foreach (IRowData rowData in sapRowDataList)
                {
                    string value = rowData.FieldValues[customerOwnedSequence];
                    if (value != "Y")
                    {
                        GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " Customer Owned Field Removed from Sequence");
                        rowData.FieldValues.Remove(customerOwnedSequence);
                        processedSapRowDataList.Add(rowData);
                    }

                }

                return processedSapRowDataList;
            }
            else
            {
                return sapRowDataList;
            }

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
            List<IRowData> sapRowDataList = new List<IRowData>();
            
            DeleteFeat activeRow = sourceRow;
            
            if (ProcessedOIDs.Contains(activeRow.OID) == false)
            {
                GISSAPIntegrator._logger.Info("Processing OID :: " + activeRow.OID);

                sapRowDataList = base.ProcessRow(sourceRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                }

                if (_trackedClass.RelatedClass == null)
                {
                    ProcessedOIDs.Add(activeRow.OID);
                }
                else
                {
                    GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " getting GUID");
                    string deviceGUID = GetDeviceGUID(activeRow);
                    if (string.IsNullOrEmpty(deviceGUID))
                    {
                        ProcessedOIDs.Add(activeRow.OID);
                        GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " got GUID :: " + deviceGUID);
                    }
                }
            }
            else
            {
                GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);
                //xwu return data even if the activeRow has been processed to test against 'add new capacitor with new controller'
                sapRowDataList = base.ProcessRow(sourceRow, changeType);
                if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                    GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                    GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                }

                Settings settings = _trackedClass.Settings;
                if (settings != null && settings["ProcessOnceAsRelatedRow"] == "False")
                {
                    GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);
                    sapRowDataList = base.ProcessRow(sourceRow, changeType);
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    }

                }
            }

            
            int customerOwnedSequence = GetCustomerOwnedFieldSequence();
            if (customerOwnedSequence != -1)
            {
                GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " Checking Customer Owned");
                List<IRowData> processedSapRowDataList = new List<IRowData>();

                foreach (IRowData rowData in sapRowDataList)
                {
                    string value = rowData.FieldValues[customerOwnedSequence];
                    if (value != "Y")
                    {
                        GISSAPIntegrator._logger.Info("OID :: " + activeRow.OID + " Customer Owned Field Removed from Sequence");
                        rowData.FieldValues.Remove(customerOwnedSequence);
                        processedSapRowDataList.Add(rowData);
                    }

                }

                return processedSapRowDataList;
            }
            else
            {
                return sapRowDataList;
            }

        }

        private int GetCustomerOwnedFieldSequence()
        {
            int sequence = -1;
            foreach (MappedField field in _trackedClass.Fields)
            {
                if (field.OutName == "CustomerOwned")
                {
                    sequence = field.Sequence;
                    break;
                }
            }
            return sequence;
        }

        private string GetDeviceGUID(IRow row)
        {
            if (_deviceGUIDFieldIndex == null)
            {
                ITable tbl = row.Table;
                _deviceGUIDFieldIndex = BaseRowTransformer.GetFieldIndex((IObjectClass)tbl, "DeviceGUID");
            }

            string deviceGUID = string.Empty;
            object tmp = row.get_Value((int)_deviceGUIDFieldIndex);
            if (tmp != null && tmp != DBNull.Value)
            {
                deviceGUID = Convert.ToString(tmp);
            }

            if (string.IsNullOrWhiteSpace(deviceGUID))
            {
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " GUID not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info(row.OID + " GUID not found ");
                GISSAPIntegrator._logger.Error(row.OID + " GUID not found ");
            }
            
            return deviceGUID;
        }

        private string GetDeviceGUID(DeleteFeat row)
        {
            object tmp = default;
            if (row.fields_Old.ContainsKey("DeviceGUID".ToUpper()))
                tmp = row.fields_Old["DeviceGUID".ToUpper()];

            string deviceGUID = string.Empty;
            
            if (tmp != null && tmp != DBNull.Value)
            {
                deviceGUID = Convert.ToString(tmp);
            }

            if (string.IsNullOrWhiteSpace(deviceGUID))
            {
                GISSAPIntegrator._errorMessage.AppendLine(row.OID + " GUID not found " + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info(row.OID + " GUID not found ");
                GISSAPIntegrator._logger.Error(row.OID + " GUID not found ");
            }

                return deviceGUID;
        }

        /// <summary>
        /// Find the SAP Type the GIS row is equivalent to
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP Type</returns>
        public override SAPType GetSAPType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");
            return SAPType.DeviceEquipment ;
        }

        /// <summary>
        /// Find the SAP Type the GIS row is equivalent to
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP Type</returns>
        public override SAPType GetSAPType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");
            return SAPType.DeviceEquipment;
        }

        /// <summary>
        /// Find the SAP Type the GIS row is equivalent to
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP Type</returns>
        public override SAPType GetSAPType(DeleteFeat sourceRow, ITable FCName, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: DeviceEquipment");

            return SAPType.DeviceEquipment;
        }
    }
}
