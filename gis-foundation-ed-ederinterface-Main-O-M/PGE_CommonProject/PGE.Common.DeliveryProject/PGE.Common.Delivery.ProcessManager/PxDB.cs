using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;
using ADODB;
using Miner.Interop.Process;
using System.IO;
using Miner.Interop;
using System.Runtime.InteropServices;
using System.Globalization;
using PGE.Common.Delivery.Systems;

namespace PGE.Common.Delivery.Process
{
    /// <summary>
    /// A supporting class that allows interfacing with the product px framework tables and 
    /// frequent operations that use the <see cref="IMMPxApplication"/> interface.
    /// </summary>
    public class PxDb
    {
        /// <summary>
        /// The process framework application reference.
        /// </summary>
        protected IMMPxApplication _PxApp;

        /// <summary>
        /// Initializes a new instance of the <see cref="PxDb"/> class.
        /// </summary>
        /// <param name="pxApp">The px app </param>        
        public PxDb(IMMPxApplication pxApp)
        {
            _PxApp = pxApp;
        }

        #region Public Members
        /// <summary>
        /// Gets the name of the schema.
        /// </summary>
        /// <value>The name of the schema.</value>
        public string SchemaName
        {
            get
            {
                return _PxApp.Login.SchemaName;
            }
        }

        /// <summary>
        /// Gets or sets the current node.
        /// </summary>
        /// <value>The current node.</value>
        public IMMPxNode CurrentNode
        {
            get
            {
                IMMPxApplicationEx pxAppEx = (IMMPxApplicationEx)_PxApp;
                return pxAppEx.CurrentNode;
            }
            set
            {
                IMMPxApplicationEx pxAppEx = (IMMPxApplicationEx)_PxApp;
                pxAppEx.CurrentNode = value;
            }
        }

        /// <summary>
        /// Finds the task using the specified node and task name, when the task is found the current application node will be updated.
        /// </summary>
        /// <param name="node">The node </param>
        /// <param name="taskName">Name of the task.</param>
        /// <returns>The <see cref="IMMPxTask"/> matching the specified task name for the given node; otherwise an exception.</returns>
        /// <exception cref="ArgumentException">"The task could not be found as an available enabled task."</exception>
        public IMMPxTask TaskByName(IMMPxNode node, string taskName)
        {
            IMMPxNode3 node3 = (IMMPxNode3)node;
            if (node3 == null) return null;

            IMMEnumPxTasks enumTasks = node3.EnabledTasks;
            if (enumTasks == null) return null;

            enumTasks.Reset();
            IMMPxTask task;
            while ((task = enumTasks.Next()) != null)
            {
                if (string.Equals(task.Name, taskName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // Update the current node to the given task because how often do you get a task without executing it on the given node?
                    this.CurrentNode = node;

                    // Return the task.
                    return task;
                }
            }

            throw new ArgumentException("The '" + taskName + "' could not be found as an available enabled task.");
        }

        /// <summary>
        /// Finds the filter using the specified filter name.
        /// </summary>
        /// <param name="filterName">Name of the filter.</param>
        /// <returns>The <see cref="IMMPxFilter"/> matching the filter name; otherwise an exception.</returns>
        /// <exception cref="ArgumentException">"The filter could not be found as an available filter."</exception>
        public IMMPxFilter FilterByName(string filterName)
        {
            IMMEnumPxFilter filters = _PxApp.Filters;
            filters.Reset();
            IMMPxFilter filter;

            while ((filter = filters.Next()) != null)
            {
                if (string.Equals(filter.Name, filterName, StringComparison.InvariantCultureIgnoreCase))
                {
                    filter.Initialize(_PxApp);
                    return filter;
                }
            }

            throw new ArgumentException("The " + filterName + " could not be found as a available filter.", "filterName");
        }

        /// <summary>
        /// Finds the state using the specified state name.
        /// </summary>
        /// <param name="stateName">Name of the state.</param>
        /// <returns>The <see cref="IMMPxState"/> matching the specified state name; otherwise an exception</returns>
        /// <exception cref="System.ArgumentException">The state could not be found as an available state.</exception>
        public IMMPxState StateByName(string stateName)
        {
            IMMEnumPxState enumStates = _PxApp.States;
            if (enumStates == null) return null;
            IMMPxState ps;
            enumStates.Reset();

            while ((ps = enumStates.Next()) != null)
            {
                if (string.Equals(ps.Name, stateName, StringComparison.InvariantCultureIgnoreCase))
                    return ps;
            }

            throw new ArgumentException("The " + stateName + " could not be found as a available state.", "stateName");
        }

        /// <summary>
        /// Finds the state using the specified state identifier.
        /// </summary>
        /// <param name="stateID">The state ID.</param>
        /// <returns>The <see cref="IMMPxState"/> matching the specified state identifier; otherwise an exception</returns>
        /// <exception cref="System.ArgumentException">The state could not be found as an available state.</exception>
        public IMMPxState StateByID(int stateID)
        {
            IMMEnumPxState enumStates = _PxApp.States;
            if (enumStates == null) return null;
            IMMPxState state;
            enumStates.Reset();

            while ((state = enumStates.Next()) != null)
            {
                if (state.StateID == stateID)
                    return state;
            }

            throw new ArgumentException("The " + stateID + " could not be found as a available state identifier.", "stateID");
        }

        /// <summary>
        /// Finds the transitions using the specified node and transition name.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="transitionName">Name of the transition.</param>
        /// <returns>The <see cref="IMMPxTransition"/> matching the specified transition name; otherwise an exception.</returns>
        /// <exception cref="System.ArgumentException">The transition could not be found on the specified node.</exception>
        public IMMPxTransition TransitionByName(IMMPxNode node, string transitionName)
        {
            IMMEnumPxTransition enumTransitions = node.Transitions;
            IMMPxTransition transition;
            enumTransitions.Reset();

            while ((transition = enumTransitions.Next()) != null)
            {
                if (string.Equals(transition.Name, transitionName, StringComparison.InvariantCultureIgnoreCase))
                    return transition;

                if (string.Equals(transition.DisplayName, transitionName, StringComparison.InvariantCultureIgnoreCase))
                    return transition;
            }

            throw new ArgumentException("The " + transitionName + " transition could not be found on the specified node.", "transitionName");
        }

        /// <summary>
        /// Finds the user using the specified name.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>The <see cref="IMMPxUser"/> matching the specified user, otheriwse null.</returns>
        public IMMPxUser UserByName(string userName)
        {
            IMMEnumPxUser enumUsers = _PxApp.Users;
            if (enumUsers == null) return null;
            enumUsers.Reset();
            IMMPxUser user;

            while ((user = enumUsers.Next()) != null)
            {
                if (string.Equals(user.Name, userName, StringComparison.InvariantCultureIgnoreCase))
                    return user;

                IMMPxUser2 user2 = (IMMPxUser2)user;
                if (string.Equals(user2.DisplayName, userName, StringComparison.InvariantCultureIgnoreCase))
                    return user;
            }

            return null;
        }

        /// <summary>
        /// Finds the user that matches the specified ID
        /// </summary>
        /// <param name="userID">The user ID.</param>
        /// <returns>The <see cref="IMMPxUser"/> matching the specified user, otheriwse null.</returns>
        public IMMPxUser UserByID(int userID)
        {
            IMMEnumPxUser enumUsers = _PxApp.Users;
            if (enumUsers == null) return null;
            enumUsers.Reset();
            IMMPxUser user;

            while ((user = enumUsers.Next()) != null)
            {
                if (user.Id == userID)
                    return user;
            }

            return null;
        }

        /// <summary>
        /// Returns the current Px Top Level node of the list that the specified node belongs to.
        /// </summary>
        /// <param name="node">The starting node.</param>
        /// <returns>The top level <see cref="IMMPxNode"/>; otherwise null.</returns>
        public IMMPxNode FindTopLevelNode(IMMPxNode node)
        {
            while (node != null)
            {
                if (((IMMPxNode2)node).IsPxTopLevel) return node;
                node = ((ID8ListItem)node).ContainedBy as IMMPxNode;
            }

            return null;
        }

        /// <summary>
        /// Hydrates the already initialized, specified node
        /// </summary>
        /// <param name="node">The node.</param>
        public void Hydrate(IMMPxNode node)
        {
            ((IMMPxApplicationEx)_PxApp).HydrateNodeFromDB(node);
        }

        /// <summary>
        /// Determines if the <see cref="IMMPxNode"/> is valid, which means it has an associated row in the corresponding a system table.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns><c>true</c> if valid; otherwise <c>false</c></returns>
        public bool IsValid(IMMPxNode node)
        {
            int value;

            if (node.NodeType == _PxApp.Helper.GetNodeTypeID("WorkRequest"))
            {
                value = this.ExecuteScalar<int>("SELECT COUNT(*) FROM " + this.SchemaName + PxDb.Tables.WorkflowManager.WorkRequest + " WHERE ID = " + node.Id);
            }
            else if (node.NodeType == _PxApp.Helper.GetNodeTypeID("Design"))
            {
                value = this.ExecuteScalar<int>("SELECT COUNT(*) FROM " + this.SchemaName + PxDb.Tables.WorkflowManager.Design + " WHERE ID = " + node.Id);
            }
            else if (node.NodeType == _PxApp.Helper.GetNodeTypeID("WorkLocation"))
            {
                value = this.ExecuteScalar<int>("SELECT COUNT(*) FROM " + this.SchemaName + PxDb.Tables.WorkflowManager.WorkLocation + " WHERE ID = " + node.Id);
            }
            else if (node.NodeType == _PxApp.Helper.GetNodeTypeID("CompatibleUnit"))
            {
                value = this.ExecuteScalar<int>("SELECT COUNT(*) FROM " + this.SchemaName + PxDb.Tables.WorkflowManager.CompatibleUnit + " WHERE ID = " + node.Id);
            }
            else if (node.NodeType == _PxApp.Helper.GetNodeTypeID("Session"))
            {
                value = this.ExecuteScalar<int>("SELECT COUNT(*) FROM " + this.SchemaName + PxDb.Tables.SessionManager.Session + " WHERE ID = " + node.Id);
            }
            else
            {
                return false;
            }

            return (value > 0);
        }

        /// <summary>
        /// Updates the fields with the values using the specified SQL select statement.
        /// </summary>
        /// <param name="sqlSelect">The SQL select</param>
        /// <param name="fieldNames">The field names</param>
        /// <param name="fieldValues">The field values</param>
        public void Update(string sqlSelect, object[] fieldNames, object[] fieldValues)
        {
            // Create a recordset corresponding to the specified SQL.
            Connection connection = _PxApp.Connection;
            Recordset recordset = new RecordsetClass();
            recordset.Open(sqlSelect, connection, CursorTypeEnum.adOpenDynamic,
                LockTypeEnum.adLockOptimistic, 0);

            if (recordset.BOF)
            {
                recordset.AddNew(fieldNames, fieldValues);
            }
            else
            {
                recordset.Update(fieldNames, fieldValues);
            }

            recordset.Close();
            while (Marshal.ReleaseComObject(recordset) > 0) { }
        }

        /// <summary>
        /// Updates the specified data row and it's corresponding table as the destination for the update.
        /// </summary>
        /// <param name="dataRow">The data row </param>
        /// <exception cref="InvalidConstraintException">The DataTable must have the TableName property set in order to update.</exception>
        /// <exception cref="InvalidConstraintException">The DataTable must have atleast 1 primary key set.</exception>
        public void Update(DataRow dataRow)
        {
            if (dataRow == null) return;

            DataTable table = dataRow.Table;
            if (table == null || table.TableName.Length == 0)
                throw new InvalidConstraintException("The DataTable must have the TableName property set in order to update.");

            if (table.PrimaryKey.Length == 0)
                throw new InvalidConstraintException("The DataTable must have atleast 1 primary key set.");

            List<DataColumn> primaryKeys = new List<DataColumn>(table.PrimaryKey);

            Command command = new CommandClass();
            command.ActiveConnection = _PxApp.Connection;
            command.CommandType = CommandTypeEnum.adCmdText;

            StringBuilder sqlSet = new StringBuilder();
            string x = "";

            foreach (DataColumn column in table.Columns)
            {
                // If this column is not a primary key continue otherwise skip.
                if (!primaryKeys.Contains(column))
                {
                    Parameter parameter = command.CreateParameter(column.ColumnName, this.GetDataType(column.DataType), ParameterDirectionEnum.adParamInput, column.MaxLength, dataRow[column]);
                    command.Parameters.Append(parameter);

                    sqlSet.Append(x);
                    sqlSet.Append(column.ColumnName);
                    sqlSet.Append(" = ?");
                    x = ", ";
                }
            }

            // Build where statement based on primary keys
            int i = 1;
            StringBuilder sqlWhere = new StringBuilder();
            foreach (DataColumn key in primaryKeys)
            {
                if (key.DataType == typeof(string))
                {
                    sqlWhere.Append(key.ColumnName + " = '" + dataRow[key] + "'");
                }
                else
                {
                    sqlWhere.Append(key.ColumnName + " = " + dataRow[key]);
                }

                // If we have more then 1 primary key we need to append AND
                if (primaryKeys.Count > 1)
                    if (i < primaryKeys.Count)
                        sqlWhere.Append(" AND ");
                i++;
            }

            object recordsAffected;
            object parameters = null;

            command.CommandText = "UPDATE " + table.TableName + " SET " + sqlSet + " WHERE " + sqlWhere;
            command.Execute(out recordsAffected, ref parameters, (int)CommandTypeEnum.adCmdText | (int)ExecuteOptionEnum.adExecuteNoRecords);
        }

        /// <summary>
        /// Retrieves a single value (for example, an aggregate value) from a database.
        /// </summary>
        /// <param name="sqlStatement">The sql statement to perform.</param>
        /// <returns>object value of the execution.</returns>
        public T ExecuteScalar<T>(string sqlStatement)
        {
            T value = default(T);
            object recordsAffected;
            object parameters = null;

            Command command = new CommandClass();
            command.ActiveConnection = _PxApp.Connection;
            command.CommandType = CommandTypeEnum.adCmdText;
            command.CommandText = sqlStatement;

            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            Recordset recordset = command.Execute(out recordsAffected, ref parameters, (int)CommandTypeEnum.adCmdText);

            OleDbDataAdapter adapter = new OleDbDataAdapter();
            adapter.Fill(table, recordset);
            recordset.Close();

            if (table.Rows.Count == 1 && table.Columns.Count == 1)
                value = TypeCastFacade.Cast(table.Rows[0][0], default(T));

            while (Marshal.ReleaseComObject(recordset) > 0) { }

            return value;
        }

        /// <summary>
        /// Queries the database and populates a <see cref="DataTable"/> with the resulting data from the specified SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement </param>
        /// <returns>A <see cref="DataTable"/> filled with the information returned by the sql query.</returns>
        public DataTable ExecuteQuery(string sqlStatement)
        {
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            Connection connection = _PxApp.Connection;
            Recordset recordset = new RecordsetClass();
            recordset.Open(sqlStatement, connection, CursorTypeEnum.adOpenDynamic, LockTypeEnum.adLockOptimistic, 0);

            OleDbDataAdapter adapter = new OleDbDataAdapter();
            adapter.Fill(table, recordset);
            recordset.Close();

            while (Marshal.ReleaseComObject(recordset) > 0) { }

            return table;
        }

        /// <summary>
        /// Fills DataTable with the specified SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement </param>
        /// <returns>A <see cref="DataTable"/> filled with the information returned by the sql query.</returns>
        public DataTable Fill(string sqlStatement)
        {
            DataTable table = new DataTable();
            table.Locale = System.Globalization.CultureInfo.InvariantCulture;

            // Try to extract table name out of the sqlStatement if it's too complicated (i.e joins, or multiple tables forget about it.);
            Match match = Regex.Match(sqlStatement, @"(?<=^|FROM)\s+(\w+\.\w+|\w+)(?=WHERE|$|\s*)");
            if (match.Success) table.TableName = match.Value.Trim();

            object recordsAffected;
            object parameters = null;

            Command command = new CommandClass();
            command.ActiveConnection = _PxApp.Connection;
            command.CommandType = CommandTypeEnum.adCmdText;
            command.CommandText = sqlStatement;

            Recordset recordset = command.Execute(out recordsAffected, ref parameters, (int)CommandTypeEnum.adCmdText);

            OleDbDataAdapter adapter = new OleDbDataAdapter();
            adapter.Fill(table, recordset);
            recordset.Close();

            return table;
        }

        /// <summary>
        /// Executes the given statement which is usually an Insert, Update or Delete statement and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns>The number of rows affected.</returns>
        /// <remarks>Oracle doesn't like the semi-colon at the end of statements make sure they are removed for oracle databases.</remarks>
        public int ExecuteNonQuery(string sqlStatement)
        {
            object recordsEffected;
            _PxApp.Connection.Execute(sqlStatement, out recordsEffected, (int)CommandTypeEnum.adCmdText | (int)ExecuteOptionEnum.adExecuteNoRecords);

            return TypeCastFacade.Cast(recordsEffected, -1);
        }

        /// <summary>
        /// Given the Config name gets the Config value from MM_PX_CONFIG table.
        /// The configuration is set using the Configuration tab of Process Framework Administration tool.
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public string GetConfigValue(string config)
        {
            IMMPxHelper2 pxHelper = (IMMPxHelper2)_PxApp.Helper;
            return pxHelper.GetConfigValue(config);
        }
        #endregion

        #region Private Members

        /// <summary>
        /// Gets the data type that corresponds to an <see cref="ADODB.DataTypeEnum"/> enumeration.
        /// </summary>
        /// <param name="columnType">Type of the column.</param>
        /// <returns>The <see cref="ADODB.DataTypeEnum"/> equivalent of the type.</returns>
        private DataTypeEnum GetDataType(Type columnType)
        {
            switch (columnType.UnderlyingSystemType.ToString())
            {
                case "System.Bool":
                    return DataTypeEnum.adBoolean;

                case "System.Byte":
                    return DataTypeEnum.adUnsignedTinyInt;

                case "System.Char":
                    return DataTypeEnum.adChar;

                case "System.DateTime":
                    return DataTypeEnum.adDate;

                case "System.Decimal":
                    return DataTypeEnum.adInteger;

                case "System.Double":
                    return DataTypeEnum.adDouble;

                case "System.Int16":
                    return DataTypeEnum.adSmallInt;

                case "System.Int32":
                    return DataTypeEnum.adInteger;

                case "System.Int64":
                    return DataTypeEnum.adBigInt;

                case "System.SByte":
                    return DataTypeEnum.adTinyInt;

                case "System.Single":
                    return DataTypeEnum.adSingle;

                case "System.UInt16":
                    return DataTypeEnum.adUnsignedSmallInt;

                case "System.UInt32":
                    return DataTypeEnum.adUnsignedInt;

                case "System.UInt64":
                    return DataTypeEnum.adUnsignedBigInt;

                case "System.String":
                    return DataTypeEnum.adVarChar;

                default:
                    return DataTypeEnum.adVarChar;
            }
        }

        #endregion

        #region Table Struct
        /// <summary>
        /// The struct of the core product table names.
        /// </summary>
        public struct Tables
        {
            /// <summary>
            /// The users table name.
            /// </summary>
            public const string Users = "MM_PX_USER";

            /// <summary>
            /// The session manager specific tables.
            /// </summary>
            public struct SessionManager
            {
                /// <summary>
                /// The session table name.
                /// </summary>
                public const string Session = "MM_SESSION";
            }

            /// <summary>
            /// The workflow manager specific tables.
            /// </summary>
            public struct WorkflowManager
            {
                /// <summary>
                /// The work request table name.
                /// </summary>
                public const string WorkRequest = "MM_WMS_WORK_REQUEST";
                /// <summary>
                /// The design table name.
                /// </summary>
                public const string Design = "MM_WMS_DESIGN";
                /// <summary>
                /// The work location table name.
                /// </summary>
                public const string WorkLocation = "MM_WMS_WORK_LOCATION";
                /// <summary>
                /// The compatible unit table name.
                /// </summary>
                public const string CompatibleUnit = "MM_WMS_COMPATIBLE_UNIT";
                /// <summary>
                /// The approved design table name.
                /// </summary>
                public const string ApprovedDesign = "MM_WMS_APPROVED_DESIGNS";
            }
        }
        #endregion

        #region Task Struct
        /// <summary>
        /// A structure that holds the common process framework task names.
        /// </summary>
        public struct Tasks
        {
            /// <summary>
            /// The workflow manager specific task names.
            /// </summary>
            public struct WorkflowManager
            {
                /// <summary>
                /// The task that will create a design.
                /// </summary>
                public const string CreateDesign = "Create Design";
                /// <summary>
                /// The task that will save the design.
                /// </summary>
                public const string SaveDesign = "Save Design";
                /// <summary>
                /// The task that will close the design.
                /// </summary>
                public const string CloseDesign = "Close Design";
                /// <summary>
                /// The task that will submit the design.
                /// </summary>
                public const string SubmitDesign = "Submit Design";
                /// <summary>
                /// The task that will open the design.
                /// </summary>
                public const string OpenDesign = "Open Design";
                /// <summary>
                /// The task that will delete the design or work request.
                /// </summary>
                public const string Delete = "Delete";
                /// <summary>
                /// The task that will create a work request.
                /// </summary>
                public const string CreateWorkRequest = "Create Work Request";
            }

            /// <summary>
            /// The session manager specific task names.
            /// </summary>
            public struct SessionManager
            {
                /// <summary>
                /// The task that will save the session.
                /// </summary>
                public const string SaveSession = "Save Session";
                /// <summary>
                /// The task that will close the session.
                /// </summary>
                public const string CloseSession = "Close Session";
                /// <summary>
                /// The task that will open a redline session.
                /// </summary>
                public const string OpenRedline = "Open Redline Session";
                /// <summary>
                /// The task that will open the session for editing.
                /// </summary>
                public const string OpenSession = "Open Edit Session";
                /// <summary>
                /// The task will send the session back to the enterprise or field.
                /// </summary>
                public const string SendSession = "Send Session";
                /// <summary>
                /// The task will delete the session from the database.
                /// </summary>
                public const string DeleteSession = "Delete Session";
                /// <summary>
                /// The task will Create a new session in the database.
                /// </summary>
                public const string CreateSession = "Create Session";
                /// <summary>
                /// The task will perform the send/receive task for session data.
                /// </summary>
                public const string SendReceive = "Send/Receive Mobile Data";

            }
        }
        #endregion

        #region Filter Struct
        /// <summary>
        /// A structure that holds the common process framework filter names.
        /// </summary>
        public struct Filters
        {
            /// <summary>
            /// The workflow manager specific filters.
            /// </summary>
            public struct WorkflowManager
            {
                /// <summary>
                /// The name for the all work request filter.
                /// </summary>
                public const string AllWorkRequests = "All Work Requests";
                /// <summary>
                /// The name for the user work request filter.
                /// </summary>
                public const string UserWorkRequests = "My Work Requests";
            }
            /// <summary>
            /// The session manager specific filters.
            /// </summary>
            public struct SessionManager
            {
                /// <summary>
                /// The name for the all sessions filter.
                /// </summary>
                public const string AllSessions = "All Sessions";
                /// <summary>
                /// The name for the user sessions filter.
                /// </summary>
                public const string UserSessions = "My Sessions";
            }
        }
        #endregion

        #region Configurations Struct
        /// <summary>
        /// A structure that holds the common process framework configurations.
        /// </summary>
        public struct Configuration
        {
            /// <summary>
            /// The session manager specific configurations.
            /// </summary>
            public struct SessionManager
            {
                /// <summary>
                /// PxExtension name for Session Manager
                /// </summary>
                public const string ExtensionName = "MMSessionManager";
                /// <summary>
                /// The configuration name for the enterprise unc path location for the mobile packets.
                /// </summary>
                public const string Enterprise = "ProcessFrameworkEnterprise";
                /// <summary>
                /// The configuration name for the field path location for the mobile packets.
                /// </summary>
                public const string Field = "ProcessFrameworkField";
            }

            /// <summary>
            /// The workflow manager specific configurations.
            /// </summary>
            public struct WorkflowManager
            {
                /// <summary>
                /// PxExtension name for Workflow Manager
                /// </summary>
                public const string ExtensionName = "MMWorkflowManager";
                /// <summary>
                /// The configuration name for the enterprise unc path location for the mobile packets.
                /// </summary>
                public const string Enterprise = "ProcessFrameworkEnterprise";
                /// <summary>
                /// The configuration name for the field path location for the mobile packets.
                /// </summary>
                public const string Field = "ProcessFrameworkField";
            }
        }
        #endregion

        #region Role Struct
        /// <summary>
        /// A structure that holds the common process framework roles.
        /// </summary>
        public struct Roles
        {
            /// <summary>
            /// The session manager specific roles.
            /// </summary>
            public struct SessionManager
            {
                /// <summary>
                /// The redline techician user role.
                /// </summary>
                public const string RedlineTechnician = "SESSION_REDLINE_TECH";
                /// <summary>
                /// The editor user role.
                /// </summary>
                public const string Editor = "SESSION_EDITOR";
                /// <summary>
                /// The always mobile session user role.
                /// </summary>
                public const string AlwaysMobile = "SESSION_ALWAYS_MOBILE_USER";
                /// <summary>
                /// The approval officer user role.
                /// </summary>
                public const string ApprovalOfficer = "SESSION_APPROVE";
            }

            /// <summary>
            /// The workflow manager specific roles.
            /// </summary>
            public struct WorkflowManager
            {
                /// <summary>
                /// The designer user role.
                /// </summary>
                public const string Designer = "WMS_DESIGNER";
                /// <summary>
                /// The mobile designer user role.
                /// </summary>
                public const string Mobile = "WMS_MOBILE_DESIGNER";

            }
        }
        #endregion
    }
}
