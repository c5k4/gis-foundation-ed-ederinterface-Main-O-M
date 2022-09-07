using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace PGE.Common.Delivery.Systems.Data
{
    /// <summary>
    /// Provides methods for sending and receiving information from a database.
    /// </summary>
    [ComVisible(false)]
    public interface IDatabaseConnection : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the database connection is open.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the database connection is open; otherwise, <c>false</c>.
        /// </value>
        bool IsOpen
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the underlying table by calling the respective INSERT, UPDATE, or DELETE statements for each inserted,
        /// updated, or deleted row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int ApplyChanges(DataTable table, string commandText);


        /// <summary>
        /// Updates the underlying table by calling the respective INSERT, UPDATE, or DELETE statements for each inserted,
        /// updated, or deleted row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="insertCommand">The Dbcommand for Insert.</param>
        /// <param name="updateCommand">The Dbcommand for Update.</param>
        /// <param name="deleteCommand">The Dbcommand for Delete.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int ApplyChanges(DataTable table, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand);

        /// <summary>
        /// Begins the database transaction using the <see cref="IsolationLevel.ReadCommitted"/> isolation level.
        /// </summary>
        /// <returns>The <see cref="DbTransaction"/> transaction.</returns>
        DbTransaction BeginTransaction();

        /// <summary>
        /// Begins the database transaction using the specified <paramref name="isolationLevel"/>.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>
        /// The <see cref="DbTransaction"/> transaction.
        /// </returns>
        DbTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        void Close();

        /// <summary>
        /// Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int ExecuteNonQuery(string commandText);

        /// <summary>
        /// Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        int ExecuteNonQuery(string commandText, DbTransaction transaction);

        /// <summary>
        /// Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// A <see cref="DbDataReader"/> of the results.
        /// </returns>
        DbDataReader ExecuteReader(string commandText);

        /// <summary>
        /// Retrieves a single value (for example, an aggregate value) from a database using the specified <paramref name="commandText"/> statement.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <returns>The value from the statement.</returns>
        TValue ExecuteScalar<TValue>(string commandText);

        /// <summary>
        /// Fills a <see cref="DataTable"/> with table data from the specified <paramref name="commandText"/> statement.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>The <see cref="DataTable"/> containing the data.</returns>
        DataTable Fill(string commandText, string tableName);

        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        void Open();

        /// <summary>
        /// Creates a DbCommand for the given DatabaseConnection
        /// </summary>
        /// <returns></returns>
        DbCommand CreateCommand();

        /// <summary>
        /// Creates a Database Parameter for the specified Database connection type
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        DbParameter CreateParameter(string parameterName);

        /// <summary>
        /// Get the Prefix character used by the given database type
        /// </summary>
        string ParameterPrefix
        {
            get;
        }

        #endregion
    }
}