using System;
using PGE.Common.Delivery.Systems.Data;
using System.Data;
namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Data Manager interface used to access and update the Map Look Up Table
    /// </summary>
    public interface IMPDBDataManager
    {
        /// <summary>
        /// The Database connection to use
        /// </summary>
        IDatabaseConnection DatabaseConnection { get; }
        /// <summary>
        /// Gets the Look Up table from the database for the given table name
        /// </summary>
        /// <param name="tableName">The look up table to open</param>
        /// <returns></returns>
        DataTable GetLookUpTable(string tableName, params string[] whereClause);
        /// <summary>
        /// Gets the Map Look Up table using the IMPMapLookUpTable's TableName Property
        /// </summary>
        /// <returns>Returns a DataTable for the IMPMapLookUpTable with the data from the table</returns>
        DataTable GetMapLookUpTable(params string[] whereClause);
        /// <summary>
        /// Gets the Feeder Look Up table using the IMPFeederLookUpTable's TableName Property
        /// </summary>
        /// <returns>Returns a DataTable for the IMPFeederLookUpTable with the data from the table</returns>
        DataTable GetFeederLookUpTable(params string[] whereClause);
        /// <summary>
        /// Query the next sequence number for a given sequence
        /// </summary>
        /// <param name="sequenceName">Sequence Name</param>
        /// <returns>Returns the next sequence number</returns>
        int GetNextSequence(string sequenceName);
        /// <summary>
        /// Gets the IMPMapLookUpTable to be used
        /// </summary>
        IMPMapLookUpTable MapLookUpTable { get; }
        /// <summary>
        /// Updates the LookUpTable with the data from the given DataTable.
        /// </summary>
        /// <param name="dataTable">Datatable that holds the data to be updated</param>
        /// <returns>Returns true if hte updates are successful</returns>
        int UpdateLookUpTable(DataTable dataTable);

        int UpdateLookUpTable(DataTable dataTable, string sqlCommand);

        /// <summary>
        /// add all data to the Database.
        /// Ustilizes the InsertCommand/UpdateCommand/DeleteCommand from the IMPMapLookUpTable interface to perform the updates
        /// </summary>
        /// <param name="dataTable">Datatable with data that should be persisted to database.</param>
        /// <returns>status</returns>
        int AddLookUpRows(DataTable dataTable);
    }
}
