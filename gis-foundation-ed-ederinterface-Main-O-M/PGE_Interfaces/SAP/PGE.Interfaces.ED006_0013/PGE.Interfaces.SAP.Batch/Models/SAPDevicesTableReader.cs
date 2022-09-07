﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;

using PGE.Interfaces.SAP.Interfaces;
using PGE.Interfaces.SAP;
using PGE.Interfaces.SAP.Data;
using PGE.Interfaces.Integration.Framework.Data;


namespace PGE.Interfaces.SAP.Batch
{
    /// <summary>
    /// This class has SQL to query rows specific to devices from the staging table
    /// </summary>
    public class SAPDevicesTableReader : ISAPDataReader
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SAPBatch.log4net.config");

        /// <summary>
        /// Default Constructor. Creates a new DBInteraction
        /// </summary>
        public SAPDevicesTableReader()
        {
        }

        /// <summary>
        /// This method queries staging table in a specific order for devices.
        /// Insert equipment
        /// Insert subequipment
        /// Update equipment
        /// Update subequipment
        /// Delete subequipment
        /// Delete equipment
        /// </summary>
        /// <returns>A list of SAPRowData converted from Staging table data</returns>
        public List<SAPRowData> ReadData()
        {
            List<SAPRowData> data = new List<SAPRowData>();

            using (DataHelper dbHelper = new DataHelper())
            {
                var EquipmentInserts = dbHelper.GetRecords((short)SAPType.DeviceEquipment, (char)ActionType.Insert);
                _logger.Info(string.Format("     {0} Equipment Inserts to process", EquipmentInserts.Count()));

                var SubEquipmentInserts = dbHelper.GetRecords((short)SAPType.DeviceSubEquipment, (char)ActionType.Insert);
                _logger.Info(string.Format("     {0} SUB Equipment Inserts to process", SubEquipmentInserts.Count()));

                var EquipmentUpdates = dbHelper.GetRecords((short)SAPType.DeviceEquipment, (char)ActionType.Update);
                _logger.Info(string.Format("     {0} Equipment Updates to process", EquipmentUpdates.Count()));

                var SubEquipmentUpdates = dbHelper.GetRecords((short)SAPType.DeviceSubEquipment, (char)ActionType.Update);
                _logger.Info(string.Format("     {0} SUB Equipment Updates to process", SubEquipmentUpdates.Count()));

                var EquipmentIdles = dbHelper.GetRecords((short)SAPType.DeviceEquipment, (char)ActionType.Idle );
                _logger.Info(string.Format("     {0} Equipment Idles to process", EquipmentIdles.Count()));

                var SubEquipmentIdles = dbHelper.GetRecords((short)SAPType.DeviceSubEquipment, (char)ActionType.Idle );
                _logger.Info(string.Format("     {0} SUB Equipment Idles to process", SubEquipmentIdles.Count()));

                var EquipmentDeletes = dbHelper.GetRecords((short)SAPType.DeviceEquipment, (char)ActionType.Delete);
                _logger.Info(string.Format("     {0} Equipment Deletes to process", EquipmentDeletes.Count()));

                var SubEquipmentDeletes = dbHelper.GetRecords((short)SAPType.DeviceSubEquipment, (char)ActionType.Delete);
                _logger.Info(string.Format("     {0} SUB Equipment Deletes to process", SubEquipmentDeletes.Count()));

                foreach (var eInsert in EquipmentInserts)
                {
                    SAPRowData row = ProcessRow(eInsert);
                    if (row != null) data.Add(row);
                }

                _logger.Info(EquipmentInserts.Count() + " devices equipment inserted");

                foreach (var seInsert in SubEquipmentInserts)
                {
                    SAPRowData row = ProcessRow(seInsert);
                    if (row != null) data.Add(row);
                }

                _logger.Info(SubEquipmentInserts.Count() + " devices subequipment inserted");

                foreach (var eUpdate in EquipmentUpdates)
                {
                    SAPRowData row = ProcessRow(eUpdate);
                    if (row != null) data.Add(row);
                }

                _logger.Info(EquipmentUpdates.Count() + " devices equipment updated");

                foreach (var seUpdate in SubEquipmentUpdates)
                {
                    SAPRowData row = ProcessRow(seUpdate);
                    if (row != null) data.Add(row);
                }

                _logger.Info(SubEquipmentUpdates.Count() + " devices subequipment updated");

                foreach (var eIdle in EquipmentIdles)
                {
                    SAPRowData row = ProcessRow(eIdle );
                    if (row != null) data.Add(row);
                }

                _logger.Info(EquipmentIdles.Count() + " devices equipment idled");

                foreach (var seIdle in SubEquipmentIdles)
                {
                    SAPRowData row = ProcessRow(seIdle );
                    if (row != null) data.Add(row);
                }

                _logger.Info(SubEquipmentIdles.Count() + " devices subequipment idled");

                foreach (var seDelete in SubEquipmentDeletes)
                {
                    SAPRowData row = ProcessRow(seDelete);
                    if (row != null) data.Add(row);
                }

                _logger.Info(SubEquipmentDeletes.Count() + " devices subequipment deleted");

                foreach (var eDelete in EquipmentDeletes)
                {
                    SAPRowData row = ProcessRow(eDelete);
                    if (row != null) data.Add(row);
                }

                _logger.Info(EquipmentDeletes.Count() + " devices equipment deleted");
            }

            return data;
        }

        /// <summary>
        /// This method takes in a database row and converts it to SAPRowData
        /// </summary>
        /// <param name="dbrow">Database row</param>
        /// <returns>SAPRowData</returns>
        private SAPRowData ProcessRow(GISSAP_ASSETSYNCH dbrow)
        {
            try
            {
                SAPRowData row = new SAPRowData(new RowData());
                row.FieldValues = new Dictionary<int, string>();

                //row.ActionType = (ActionType)dbrow.ACTIONTYPE[0];
                //row.SAPType = (SAPType)dbrow.TYPE;
                row.AssetID = dbrow.ASSETID;
                //row.DateProcessed = dbrow.DATEPROCESSED;
                row.FieldValues.Add(1, dbrow.SAPATTRIBUTES);
                _logger.Debug("Succesfully converted Devices DB row to SAP row data. AssetID - " + dbrow.ASSETID);
                return row;
            }
            catch(Exception e)
            {
                _logger.Error("Error converting Devices DB row to SAP row data. Assetid - " + dbrow.ASSETID, e);
                return null;
            }
        }

    }
}
