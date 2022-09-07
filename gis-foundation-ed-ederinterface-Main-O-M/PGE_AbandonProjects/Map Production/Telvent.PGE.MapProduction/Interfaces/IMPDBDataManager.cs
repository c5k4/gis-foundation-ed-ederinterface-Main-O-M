using System;
using Telvent.Delivery.Systems.Data;
using System.Data;
namespace Telvent.PGE.MapProduction
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
        /// Gets the IMPMapLookUpTable to be used
        /// </summary>
        IMPMapLookUpTable MapLookUpTable { get; }
        /// <summary>
        /// Updates the LookUpTable with the data from the given DataTable.
        /// </summary>
        /// <param name="dataTable">Datatable that holds the data to be updated</param>
        /// <returns>Returns true if hte updates are successful</returns>
        bool UpdateLookUpTable(DataTable dataTable);
    }
}
