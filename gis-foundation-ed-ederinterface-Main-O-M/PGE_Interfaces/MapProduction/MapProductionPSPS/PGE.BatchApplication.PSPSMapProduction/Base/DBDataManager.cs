using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Systems.Data;
using System.Data;
using System.Data.Common;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;

namespace PGE.BatchApplication.PSPSMapProduction
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
        /// Returns the IDatabaseConnection object as passed in the constructor
        /// </summary>
        public virtual IDatabaseConnection SDEConnection
        {
            get;
            private set;
        }

        private ITable ConnectFGDBWorkspace(String fgdbFile, string tableName)
        {
            IPropertySet propertySet = new PropertySetClass();
            propertySet.SetProperty("DATABASE", fgdbFile);
            Type factoryType = Type.GetTypeFromProgID("esriDataSourcesGDB.FileGDBWorkspaceFactory");
            IWorkspaceFactory workspaceFactory = (IWorkspaceFactory) Activator.CreateInstance(factoryType);
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory;
            ITable aoTable = featureWorkspace.OpenTable(tableName);

            return aoTable;
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
        /// Uses the IDatabaseConnection to query the MapLook up table from the database and return results as a Datatable
        /// </summary>
        /// <returns><see cref="System.Data.DataTable"/></returns>
        public virtual DataTable GetFeederLookUpTable(params string[] whereClause)
        {
            return GetLookUpTable("EDGIS.CIRCUIT_FEEDER", whereClause);
        }

        /// <summary>
        /// Populates changes made to the Datatable to the Database.
        /// Ustilizes the InsertCommand/UpdateCommand/DeleteCommand from the IMPMapLookUpTable interface to perform the updates
        /// </summary>
        /// <param name="dataTable">Datatable with data that should be persisted to database.</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"/>
        public virtual int UpdateLookUpTable(DataTable dataTable)
        {
            if (dataTable == null)
                throw new NullReferenceException("Datatable cannot be null");

            DbCommand insertCommand = MapLookUpTable.InsertCommand(DatabaseConnection.ParameterPrefix, DatabaseConnection);
            DbCommand updateCommand = MapLookUpTable.UpdateCommand(DatabaseConnection.ParameterPrefix, DatabaseConnection);
            DbCommand deleteCommand = MapLookUpTable.DeleteCommand(DatabaseConnection.ParameterPrefix, DatabaseConnection);
            int affectedRecords = DatabaseConnection.ApplyChanges(dataTable, insertCommand, updateCommand, deleteCommand);
            return affectedRecords;
        }


        /// <summary>
        /// add all data to the Database.
        /// Ustilizes the InsertCommand/UpdateCommand/DeleteCommand from the IMPMapLookUpTable interface to perform the updates
        /// </summary>
        /// <param name="dataTable">Datatable with data that should be persisted to database.</param>
        /// <returns>status</returns>
        public virtual int AddLookUpRows(DataTable dataTable)
        {
            if (dataTable == null)
                throw new NullReferenceException("Datatable cannot be null");

            if (!DatabaseConnection.IsOpen)
            {
                DatabaseConnection.Open();
            }

            DbCommand insertCommand = MapLookUpTable.InsertCommand(DatabaseConnection.ParameterPrefix, DatabaseConnection);
            int affectedRecords = DatabaseConnection.ApplyChanges(dataTable, insertCommand, null, null);

            if (DatabaseConnection.IsOpen)
            {
                DatabaseConnection.Close();
            }
            return affectedRecords;
        }


        /// <summary>
        /// Populates changes made to the Datatable to the Database.
        /// Ustilizes the single sql statement to perform the updates
        /// </summary>
        /// <param name="dataTable">Datatable with data that should be persisted to database.</param>
        /// <param name="sqlCommand">sql update command</param>
        /// <returns></returns>
        /// <exception cref="System.NullReferenceException"/>
        public virtual int UpdateLookUpTable(DataTable dataTable, string sqlCommand)
        {
            if (dataTable == null)
            {
                throw new NullReferenceException("Datatable cannot be null");
            }

            int affectedRecords = DatabaseConnection.ApplyChanges(dataTable, sqlCommand);

            return affectedRecords;
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
            {
                throw new NullReferenceException("Table name cannot be null or empty");
            }
            DataTable retVal = null;
            string selectClause = "select * from " + tableName;
            if (whereClause.Count() > 0)
            {
                if (whereClause[0].ToLower().IndexOf("where ") < 0)
                {
                    string circuitIds = whereClause[0];
                    whereClause[0] = string.Format("where CIRCUITID in ('{0}')", string.Join("','", circuitIds.Split(',')));
                }

                selectClause = selectClause + " " + whereClause[0];
                if (selectClause.ToLower().IndexOf("order by") < 0)
                {
                    string orderBy = String.Format(" order by {0},{1},{2}", MapLookUpTable.CircuitName, MapLookUpTable.CircuitId, MapLookUpTable.MapId);
                    selectClause += orderBy;
                }
            }
            if (DatabaseConnection != null)
            {
                retVal = DatabaseConnection.Fill(selectClause, tableName);
                retVal.AcceptChanges();
            }

            return retVal;

        }

        /// <summary>
        /// Query the next sequence number for a given sequence
        /// </summary>
        /// <param name="sequenceName">Sequence Name</param>
        /// <returns>Returns the next sequence number</returns>
        public int GetNextSequence(string sequenceName)
        {
            int retVal = 0;
            string selectClause = string.Format("select {0}.Nextval from dual", sequenceName);

            if (DatabaseConnection != null)
            {
                retVal = DatabaseConnection.ExecuteScalar<int>(selectClause);
            }

            return retVal;
        }

    }
}
