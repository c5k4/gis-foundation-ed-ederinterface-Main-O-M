using System;
using System.Data;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace PGE.Common.Delivery.Systems.Data
{
    /// <summary>
    /// A supporting class used to handle querying a <see cref="DbConnection"/> for information.
    /// </summary>
    /// <typeparam name="TConnection">The type of the connection.</typeparam>
    [ComVisible(false)]
    [ClassInterface(ClassInterfaceType.None)]
    public abstract class DatabaseConnection<TConnection> : IDatabaseConnection
        where TConnection : DbConnection
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConnection&lt;TConnection&gt;"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <exception cref="NullReferenceException">The connection cannot be null.</exception>
        public DatabaseConnection(TConnection connection)
        {
            if (connection == null)
                throw new NullReferenceException("The connection cannot be null.");

            this.Connection = connection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the database connection is open.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the database connection is open; otherwise, <c>false</c>.
        /// </value>
        public bool IsOpen
        {
            get
            {
                return this.Connection.State == ConnectionState.Open;
            }
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        protected TConnection Connection
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the underlying table by calling the respective INSERT, UPDATE, or DELETE statements for each inserted,
        /// updated, or deleted row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public virtual int ApplyChanges(DataTable table, string commandText)
        {
            if (table == null)
                throw new NullReferenceException("The table cannot be null.");

            // Open the connection.
            this.Open();

             //Create a new INSERT, UPDATE or DELETE command for the connection.
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = commandText.ToUpper();
                cmd.Connection = this.Connection;
                cmd.CommandType = CommandType.Text;

                using (DbDataAdapter da = this.CreateApdater())
                {
                    if (cmd.CommandText.StartsWith("UPDATE"))
                        da.UpdateCommand = cmd;
                    else if (cmd.CommandText.StartsWith("INSERT"))
                        da.InsertCommand = cmd;
                    else if (cmd.CommandText.StartsWith("DELETE"))
                        da.DeleteCommand = cmd;
                    else
                        throw new NotSupportedException("The command text is not supported.");

                    return da.Update(table);
                }
            }
        }
        
        /// <summary>
        /// Updates the underlying table by calling the respective INSERT, UPDATE, or DELETE statements for each inserted,
        /// updated, or deleted row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="insertCommand">The Dbcommand for Insert. Use CreateCommand and CreateParameter to get the correct types</param>
        /// <param name="updateCommand">The Dbcommand for Update. Use CreateCommand and CreateParameter to get the correct types</param>
        /// <param name="deleteCommand">The Dbcommand for Delete. Use CreateCommand and CreateParameter to get the correct types</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public virtual int ApplyChanges(DataTable table, DbCommand insertCommand, DbCommand updateCommand, DbCommand deleteCommand)
        {
            if (table == null)
                throw new NullReferenceException("The table cannot be null.");

            // Open the connection.
            this.Open();
            using (DbDataAdapter da = this.CreateApdater())
            {
                da.TableMappings.Add(GetTableMapping(table)); 
                if (updateCommand != null)
                    da.UpdateCommand = updateCommand;
                if (insertCommand != null)
                    da.InsertCommand = insertCommand;
                if (deleteCommand != null)
                    da.DeleteCommand = deleteCommand;
                return da.Update(table);
            }
        }

        /// <summary>
        /// Begins the database transaction using the <see cref="IsolationLevel.ReadCommitted"/> isolation level.
        /// </summary>
        /// <returns>
        /// The <see cref="DbTransaction"/> transaction.
        /// </returns>
        public DbTransaction BeginTransaction()
        {
            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Begins the database transaction using the specified <paramref name="isolationLevel"/>.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>
        /// The <see cref="DbTransaction"/> transaction.
        /// </returns>
        public DbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            // Open the connection.
            this.Open();

            return this.Connection.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public void Close()
        {
            if (this.IsOpen)
                this.Connection.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public virtual int ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(commandText, null);
        }

        /// <summary>
        /// Executes the given INSERT, UPDATE or DELETE statement and returns the number of rows affected.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public virtual int ExecuteNonQuery(string commandText, DbTransaction transaction)
        {
            // Open the connection.
            this.Open();

            // Create a new INSERT, UPDATE or DELETE command for the connection.
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = this.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.Transaction = transaction;

                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes the given SELECT statement and returns the rows as part of a <see cref="DbDataReader"/>.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// A <see cref="DbDataReader"/> of the results.
        /// </returns>
        public virtual DbDataReader ExecuteReader(string commandText)
        {
            // Open the connection.
            this.Open();

            // Create a new select command for the connection.
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = this.Connection;
                cmd.CommandType = CommandType.Text;

                // Return the reader.
                return cmd.ExecuteReader();
            }
        }

        /// <summary>
        /// Retrieves a single value (for example, an aggregate value) from a database using the specified <paramref name="commandText"/> statement.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="commandText">The command text.</param>
        /// <returns>
        /// The value from the statement.
        /// </returns>
        public virtual TValue ExecuteScalar<TValue>(string commandText)
        {
            // Open the connection.
            this.Open();

            // Create a new select command for the connection.
            using (DbCommand cmd = this.Connection.CreateCommand())
            {
                cmd.CommandText = commandText;
                cmd.Connection = this.Connection;
                cmd.CommandType = CommandType.Text;

                return TypeCastFacade.Cast(cmd.ExecuteScalar(), default(TValue));
            }
        }

        /// <summary>
        /// Fills a <see cref="DataTable"/> with table data from the specified <paramref name="commandText"/> statement.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// The <see cref="DataTable"/> containing the data.
        /// </returns>
        public virtual DataTable Fill(string commandText, string tableName)
        {
            // Read the results from the reader into a table.
            using (DbDataReader dr = this.ExecuteReader(commandText))
            {
                // Load the table.
                DataTable dt = new DataTable(tableName);
                dt.Load(dr, LoadOption.PreserveChanges);
                return dt;
            }
        }

        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        public void Open()
        {
            if (!this.IsOpen)
                this.Connection.Open();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the apdater that is used by the <see cref="DbConnection"/>.
        /// </summary>
        /// <returns>The <see cref="DbDataAdapter"/> for the specified connection.</returns>
        protected abstract DbDataAdapter CreateApdater();

        #endregion


        /// <summary>
        /// Creates a DbCommand for the given DatabaseConnection
        /// </summary>
        /// <returns></returns>
        public DbCommand CreateCommand()
        {
            return this.Connection.CreateCommand();
        }
        /// <summary>
        /// Creates the Parameter that is used by the <see cref="DbCommand"/>.
        /// </summary>
        /// <returns>The <see cref="DbDataAdapter"/> for the specified connection.</returns>
        public abstract DbParameter CreateParameter(string parameterName);

        /// <summary>
        /// Get the Prefix character used by the given database type
        /// </summary>
        public string ParameterPrefix
        {
            get
            {
                DataTable schema = this.Connection.GetSchema("DataSourceInformation");
                string parameterMarkerPattern = (string)schema.Rows[0]["ParameterMarkerFormat"];
                return string.Format(parameterMarkerPattern,string.Empty);
            }
        }


        private DataTableMapping GetTableMapping(DataTable table)
        {
            DataTableMapping mapping = new DataTableMapping();
            mapping.DataSetTable = table.TableName;
            mapping.SourceTable = table.TableName;
            foreach (DataColumn dc in table.Columns)
            {
                DataColumnMapping columnMapipng = new DataColumnMapping(dc.ColumnName, dc.ColumnName);
                mapping.ColumnMappings.Add(columnMapipng);  
            }
            return mapping;
        }

    }
}