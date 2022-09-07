using System.Collections.Generic;
using System.Data.Common;
using PGE.Common.Delivery.Systems.Data;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Base implementation of IMPMapLookUpTable
    /// </summary>
    public abstract class BaseMapLookUpTable : IMPMapLookUpTable
    {
        private string _pspsMapRowsTableName = null;
        /// <summary>
        /// Virtual property
        /// Returns the MapLookUpTableName
        /// </summary>
        public virtual string TableName
        {
            get
            {
                if (_pspsMapRowsTableName == null)
                {
                    _pspsMapRowsTableName = MapProductionConfigurationHandler.Settings["PSPSMapRowsTableName"];
                }
                return _pspsMapRowsTableName;
            }
        }

        /// <summary>
        /// Virtual property
        /// Returns the XMin field name
        /// </summary>
        public virtual string XMin
        {
            get { return "XMIN"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the YMin field Name
        /// </summary>
        public virtual string YMin
        {
            get { return "YMIN"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the XMax field Name
        /// </summary>
        public virtual string XMax
        {
            get { return "XMAX"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the YMax field Name
        /// </summary>
        public virtual string YMax
        {
            get { return "YMAX"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the FeederCircuitId field Name
        /// </summary>
        public virtual string FeederCircuitId
        {
            get { return "FEEDER_CIRCUITID"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the MapScale field Name
        /// </summary>
        public virtual string MapScale
        {
            get { return "MAPSCALE"; }
        }

        /// <summary>
        /// Virtual property
        /// Returns the ExortState field Name
        /// </summary>
        public virtual string MapExportState
        {
            get { return "EXPORTSTATE"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the Processing Service field Name
        /// </summary>
        public virtual string ProcessingService
        {
            get { return "SERVICETOPROCESS"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the LastModifiedDate field Name
        /// </summary>
        public virtual string LastModifiedDate
        {
            get { return "LASTMODIFIEDDATE"; }
        }

        /// <summary>
        /// Virtual property
        /// Returns the MAPID field Name
        /// </summary>
        public virtual string MapId
        {
            get { return "MAPID"; }
        }

        /// <summary>
        /// Virtual property
        /// Returns the RIANAME field Name
        /// </summary>
        public virtual string CircuitId
        {
            get { return "CIRCUITID"; }
        }

        /// <summary>
        /// Virtual property
        /// Returns the CIRCUITNAME field Name
        /// </summary>
        public virtual string CircuitName
        {
            get { return "CIRCUITNAME"; }
        }

        /// <summary>
        /// Virtual property
        /// Returns the Child_CircuitName field Name
        /// </summary>
        public virtual string ChildCircuitName
        {
            get { return "CHILD_CIRCUITNAME"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the PSPSNAME field Name
        /// </summary>
        public virtual string PSPSName
        {
            get { return "PSPSNAME"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the LAYOUTTYPE field Name
        /// </summary>
        public virtual string LayoutType
        {
            get { return "LAYOUTTYPE"; }
        }
        /// <summary>
        /// Virtual property
        /// Returns the TOTALSEGMENTLEN field Name
        /// </summary>
        public virtual string TotalSegmentLength
        {
            get { return "TOTALSEGMENTLEN"; }
        }
        /// <summary>
        /// Abstract property
        /// Returns the List of other field Names
        /// </summary>
        public abstract List<string> OtherFields
        {
            get; 
        }
        /// <summary>
        /// Abstract property
        /// Returns the List of key field Names
        /// This should be a subset of Otherfield Names excluding the MapNumber
        /// </summary>
        public abstract List<string> KeyFields
        {
            get;

        }

        /// <summary>
        /// Abstract property
        /// Returns the List of key field Names
        /// This should be a subset of Otherfield Names excluding the MapNumber
        /// </summary>
        public virtual List<string> AllColumns
        {
            get {
                return (new List<string>() {
                        MapId,
                        XMin,
                        YMin,
                        XMax,
                        YMax,
                        CircuitId,
                        FeederCircuitId,
                        CircuitName,
                        ChildCircuitName,
                        PSPSName,
                        TotalSegmentLength,
                        LayoutType,
                        MapExportState,
                        ProcessingService,
                        LastModifiedDate
                });
            }
        }

        /// <summary>
        /// Verifies if the given datatable has all the fields as defined in the instance of IMPMapLookUpTable 
        /// </summary>
        /// <param name="lookUpTable"><see cref="System.Data.DataTable"/> to verify</param>
        /// <returns>Returns <see cref="bool"/> indicating if the given table is valid</returns>
        public bool VerifyLookUpTable(System.Data.DataTable lookUpTable)
        {
            foreach (string colName in AllColumns)
            {
                if (!lookUpTable.Columns.Contains(colName))
                    return false;
            } 

            foreach (string keyField in KeyFields)
            {
                if (!lookUpTable.Columns.Contains(keyField))
                    return false;
            }
            foreach (string otherField in OtherFields)
            {
                if (!lookUpTable.Columns.Contains(otherField))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Using the given DatabaseConnection creates a <see cref="System.Data.Common.DbCommand"/> and builds a new InsertSQL and sets all the parameters for the instance of IMPMapLookUpTable
        /// </summary>
        /// <param name="parameterPrefix">The Parameter Prefix as defined by the Database. Use <see cref="PGE.Common.Delivery.System.Data.IDatabaseConnection"/>.GetParameterPrefix method to get the right parameter prefix</param>
        /// <param name="databaseConnection"><see cref="PGE.Common.Delivery.System.Data.IDatabaseConnection"/></param>
        /// <returns>Return <see cref="System.Data.Common.DbCommand"/> with proper insert sql and parameters that could be used in DataAdapter.Update method</returns>
        public DbCommand InsertCommand(string parameterPrefix,IDatabaseConnection databaseConnection)
        {
            DbCommand retVal = databaseConnection.CreateCommand();
            retVal.CommandText = GetInsertSQL(parameterPrefix.ToUpper());
            AddParameters(ref retVal, databaseConnection, false);
            return retVal;
        }
        /// <summary>
        /// Using the given DatabaseConnection creates a <see cref="System.Data.Common.DbCommand"/> and builds a new UpdateSQL and sets all the parameters for the instance of IMPMapLookUpTable
        /// </summary>
        /// <param name="parameterPrefix">The Parameter Prefix as defined by the Database. Use <see cref="PGE.Common.Delivery.System.Data.IDatabaseConnection"/>.GetParameterPrefix method to get the right parameter prefix</param>
        /// <param name="databaseConnection"><see cref="PGE.Common.Delivery.System.Data.IDatabaseConnection"/></param>
        /// <returns>Return <see cref="System.Data.Common.DbCommand"/> with proper update sql and parameters that could be used in DataAdapter.Update method</returns>
        public DbCommand UpdateCommand(string parameterPrefix,IDatabaseConnection databaseConnection)
        {
            DbCommand retVal = databaseConnection.CreateCommand();
            AddParameters(ref retVal, databaseConnection, false);
            retVal.CommandText = GetUpdateSQL(parameterPrefix.ToUpper());
            return retVal;
        }
        /// <summary>
        /// Using the given DatabaseConnection creates a <see cref="System.Data.Common.DbCommand"/> and builds a new DeletSQL and sets all the parameters for the instance of IMPMapLookUpTable
        /// </summary>
        /// <param name="parameterPrefix">The Parameter Prefix as defined by the Database. Use <see cref="PGE.Common.Delivery.System.Data.IDatabaseConnection"/>.GetParameterPrefix method to get the right parameter prefix</param>
        /// <param name="databaseConnection"><see cref="PGE.Common.Delivery.System.Data.IDatabaseConnection"/></param>
        /// <returns>Return <see cref="System.Data.Common.DbCommand"/> with proper delete sql and parameters that could be used in DataAdapter.Update method</returns>
        public DbCommand DeleteCommand(string parameterPrefix,IDatabaseConnection databaseConnection)
        {
            DbCommand retVal = databaseConnection.CreateCommand();
            retVal.CommandText = GetDeleteSQL(parameterPrefix.ToUpper());
            AddParameters(ref retVal, databaseConnection, true);
            return retVal;
        }

        /// <summary>
        /// Gets the InsertSQL using the properties as defined in the current instance of the IMPMapLookUpTable
        /// </summary>
        /// <param name="parameterPrefix">Parameter Prefix character to use for the given database connection</param>
        /// <returns>Returns the SQL to be applied on a DbCommand object to insert records into the table defined by IMPMapLookUpTable.TableName</returns>
        private string GetInsertSQL(string parameterPrefix)
        {
            string ss = "insert into " + TableName + "(" + string.Join(",", AllColumns.ToArray());
            
            if (OtherFields.Count > 0)
                ss = ss + "," + string.Join(",", OtherFields.ToArray());
            ss = ss + ") values (";
            ss = ss + parameterPrefix + string.Join("," + parameterPrefix, AllColumns.ToArray());
            if (OtherFields.Count > 0)
                ss = ss + ","+parameterPrefix + string.Join("," + parameterPrefix, OtherFields.ToArray());
            ss = ss + ")";
            return ss;
        }
        /// <summary>
        /// Gets the UpdateSQL using the properties as defined in the current instance of the IMPMapLookUpTable
        /// </summary>
        /// <param name="parameterPrefix">Parameter Prefix character to use for the given database connection</param>
        /// <returns>Returns the SQL to be applied on a DbCommand object to update records into the table defined by IMPMapLookUpTable.TableName</returns>
        private string GetUpdateSQL(string parameterPrefix)
        {
            string ss = "update " + TableName + " set ";
            foreach (string colName in AllColumns)
            {
                ss += colName + "=" + parameterPrefix + colName + ",";
            }
            ss = ss.TrimEnd(',');

            foreach (string field in OtherFields)
            {
                ss = ss + "," + field + "=" + parameterPrefix + field;
            }
            ss = ss + " where "+ MapId+"="+parameterPrefix+MapId;
            foreach (string field in KeyFields)
            {
                ss = ss = ss + " and " + field + "=" + parameterPrefix + field;
            }
            return ss;
        }
        /// <summary>
        /// Gets the DeleteSQL using the properties as defined in the current instance of the IMPMapLookUpTable
        /// </summary>
        /// <param name="parameterPrefix">Parameter Prefix character to use for the given database connection</param>
        /// <returns>Returns the SQL to be applied on a DbCommand object to delete records into the table defined by IMPMapLookUpTable.TableName</returns>
        private string GetDeleteSQL(string parameterPrefix)
        {
            string ss = "delete from " + TableName;
            ss = ss + " where (" + MapId + "=" + parameterPrefix + MapId;
            foreach (string field in KeyFields)
            {
                ss = ss = ss + " and " + field + "=" + parameterPrefix + field;
            }
            ss = ss + ")";
            return ss;
        }

        /// <summary>
        /// Adds parameters to the DbCommand based on the fields from IMPMapLookUpTable
        /// </summary>
        /// <param name="dbCommand">DbCommand to which the Parameters should be added</param>
        /// <param name="DatabaseConnection">IDatabaseConnection object that should be used to create Parameter</param>
        /// <param name="onlyKey">If True will only create parameters for the MapNumber field and Keyfields as defined in the given instance of IMPMapLookUpTable</param>
        private void AddParameters(ref DbCommand dbCommand,IDatabaseConnection DatabaseConnection,bool onlyKey)
        {
            List<DbParameter> retVal = new List<DbParameter>();
            //dbCommand.Parameters.Add(DatabaseConnection.CreateParameter(MapId));
            if (onlyKey)
            {
                foreach (string field in KeyFields)
                    dbCommand.Parameters.Add(DatabaseConnection.CreateParameter(field));
            }
            else
            {
                foreach (string field in AllColumns)
                {
                    dbCommand.Parameters.Add(DatabaseConnection.CreateParameter(field));
                }

                foreach (string field in OtherFields)
                    dbCommand.Parameters.Add(DatabaseConnection.CreateParameter(field));
            }
        }
    }
}
