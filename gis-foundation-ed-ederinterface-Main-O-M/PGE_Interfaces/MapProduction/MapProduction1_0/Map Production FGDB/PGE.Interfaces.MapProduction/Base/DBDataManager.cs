using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Systems.Data;
using System.Data;
using System.Data.Common;

namespace PGE.Interfaces.MapProduction
{
    /// <summary>
    /// Abstract Implementation of IMPDBDataManager
    /// </summary>
    public class DBDataManager : IMPDBDataManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="databaseConnection">IDatabaseConnection used to connect to the Database holding MapLookUpTable</param>
        /// <param name="mapLookUpTable">Class that implements IMPMapLookUpTable</param>
        public DBDataManager(IDatabaseConnection databaseConnection, IMPMapLookUpTable mapLookUpTable)
        {
            DatabaseConnection = databaseConnection;
            MapLookUpTable = mapLookUpTable;
        }

        /// <summary>
        /// Gets the Object that implementes IMPMapLookUpTable as passed in the Constructor
        /// </summary>
        public virtual IMPMapLookUpTable MapLookUpTable
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns the IDatabaseConnection object as passed in the constructor
        /// </summary>
        public virtual IDatabaseConnection DatabaseConnection
        {
            get;
            private set;
        }


        /// <summary>
        /// Uses the IDatabaseConnection to query the MapLook up table from the database and return results as a Datatable
        /// </summary>
        /// <returns><see cref="System.Data.DataTable"/></returns>
        public virtual DataTable GetMapLookUpTable(params string[] whereClause)
        {
            return GetLookUpTable(MapLookUpTable.TableName, whereClause);
        }

        /// <summary>
        /// Populates changes made to the Datatable to the Database.
        /// Ustilizes the InsertCommand/UpdateCommand/DeleteCommand from the IMPMapLookUpTable interface to perform the updates
        /// </summary>
        /// <param name="dataTable">Datatable with data that should be persisted to database.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"/>
        public virtual bool UpdateLookUpTable(DataTable dataTable)
        {
            if (dataTable == null)
                throw new NullReferenceException("Datatable cannot be null");
            bool retVal = false;
            DbCommand insertCommand = MapLookUpTable.InsertCommand(DatabaseConnection.ParameterPrefix, DatabaseConnection);
            DbCommand updateCommand = MapLookUpTable.UpdateCommand(DatabaseConnection.ParameterPrefix, DatabaseConnection);
            DbCommand deleteCommand = MapLookUpTable.DeleteCommand(DatabaseConnection.ParameterPrefix, DatabaseConnection);
            int affectedRecords = DatabaseConnection.ApplyChanges(dataTable, insertCommand, updateCommand, deleteCommand);
            return retVal = true;
        }

        /// <summary>
        /// Given TableName will fetch the given table from the Database as defined by th IDatabaseConneciton object
        /// </summary>
        /// <param name="tableName">Name of the table to fetch</param>
        /// <param name="whereClause">Where clause to use to filter the data. Do not include the WHERE key word</param>
        /// <returns>Returns a Datatable with data from the Table defined by table name in the given database.</returns>
        /// <exception cref="System.NullReferenceException"/>
        public DataTable GetLookUpTable(string tableName, params string[] whereClause)
        {
            if (string.IsNullOrEmpty(tableName))
                throw new NullReferenceException("Table name cannot be null or empty");
            DataTable retVal = null;
            string selectClause = "select * from " + tableName;
            if (whereClause.Count() > 0)
            {
                selectClause = selectClause + " " + whereClause[0];
            }
            if (DatabaseConnection != null)
            {
                retVal = DatabaseConnection.Fill(selectClause, tableName);
                retVal.AcceptChanges();
            }
            return retVal;

        }
    }
}
