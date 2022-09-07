using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Reflection;

using PGE.Interfaces.SAP.Interfaces;
using PGE.Interfaces.SAP;
using PGE.Interfaces.SAP.Data;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Interfaces.Integration.Framework.Utilities;

namespace PGE.Interfaces.SAP.Batch
{
    /// <summary>
    /// This class has SQL to query rows specific to functional location from the staging table
    /// </summary>
    public class SAPFunctionalLocationTableReader : ISAPDataReader
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SAPBatch.log4net.config");

        /// <summary>
        /// Default Constructor. Creates a new DBInteraction
        /// </summary>
        public SAPFunctionalLocationTableReader()
        {
        }

        /// <summary>
        /// This method queries staging table in a specific order for funtional locations.
        /// First insert rows are queried and added. Then update rows are queried and added.
        /// Lastly delete rows are queried and added.
        /// </summary>
        /// <returns>A list of SAPRowData converted from Staging table data</returns>
        public List<SAPRowData> ReadData()
        {
            List<SAPRowData> data = new List<SAPRowData>();

            Console.WriteLine("Processing functional locations");
            using (DataHelper dbHelper = new DataHelper())
            {
                var functionalLocationInserts = dbHelper.GetRecords((short)SAPType.FunctionalLocation, (char)ActionType.Insert);
                _logger.Info(string.Format("     {0} functional location inserts to process", functionalLocationInserts.Count()));

                var functionalLocationUpdates = dbHelper.GetRecords((short)SAPType.FunctionalLocation, (char)ActionType.Update);
                _logger.Info(string.Format("     {0} functional location updates to process", functionalLocationUpdates.Count()));

                var functionalLocationDeletes = dbHelper.GetRecords((short)SAPType.FunctionalLocation, (char)ActionType.Delete);
                _logger.Info(string.Format("     {0} functional location deletes to process", functionalLocationDeletes.Count()));
                
                foreach (var insert in functionalLocationInserts)
                {
                    SAPRowData row = ProcessRow(insert);
                    if (row != null) data.Add(row);
                }

                _logger.Debug(functionalLocationInserts.Count() + " Functional Location inserted");

                foreach (var update in functionalLocationUpdates)
                {
                    SAPRowData row = ProcessRow(update);
                    if (row != null) data.Add(row);
                }

                _logger.Debug(functionalLocationUpdates.Count() + " Functional Location updated");

                foreach (var delete in functionalLocationDeletes)
                {
                    SAPRowData row = ProcessRow(delete);
                    if (row != null) data.Add(row);
                }

                _logger.Debug(functionalLocationDeletes.Count() + " Functional Location deleted");

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
                _logger.Debug("Successfully converted Functional Location DB row to SAP row data. AssetID - " + dbrow.ASSETID);
                return row;
            }
            catch(Exception e)
            {
                _logger.Error("Error converting Functional Location DB row to SAP row data. Assetid - " + dbrow.ASSETID, e);
                return null;
            }
        }
    }
}
