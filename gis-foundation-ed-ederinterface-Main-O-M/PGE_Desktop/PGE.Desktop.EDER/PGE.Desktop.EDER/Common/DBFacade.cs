using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using Miner.Framework;
using ESRI.ArcGIS.Carto;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Framework;

namespace PGE.Desktop.EDER
{
    public class DBFacade
    {
        ADODB.Connection _connection = null;
        static IWorkspace _workspace = null;
        static IWorkspace _Currentworkspace = null;

        public DBFacade(ADODB.Connection pConnection)
        {
            _connection = pConnection;
        }

        /// <summary>
        /// Fills a <see cref="DataTable"/> with table data from the specified <paramref name="commandText"/> statement.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// The <see cref="DataTable"/> containing the data.
        /// </returns>
        public DataTable Fill(string commandText, string tableName)
        {
            DataTable dt = new DataTable();
            ADODB.Recordset recordSet = null;
            try
            {
                // Read the results from the reader into a table.
                ADODB.CommandClass command = new ADODB.CommandClass();
                command.CommandText = commandText;
                command.CommandType = ADODB.CommandTypeEnum.adCmdText;
                command.ActiveConnection = _connection;
                object dummy = Type.Missing;
                recordSet = command.Execute(
                out dummy, ref dummy, 0);
                OleDbDataAdapter adapter = new OleDbDataAdapter();

                adapter.Fill(dt, recordSet);
                //dt.Load(dr, LoadOption.PreserveChanges);
                
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex);
            }
            finally
            {
                if (recordSet != null)
                {
                    while (Marshal.ReleaseComObject(recordSet) > 0) ;
                }
            }
            return dt;
        }

        public void Execute(String commandText)
        {
            try
            {
                object dummy = Type.Missing;
                _connection.Execute(commandText, out dummy);
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex);
            }
        }

        public static IWorkspace Workspace
        {
            get
            {
                if (_workspace == null)
                {
                    //IEnumLayer featLayers = GetLayers(SchemaInfo.General.UIDs.IGeoFeatureLayer);
                    //featLayers.Reset();
                    //IFeatureLayer featLayer = featLayers.Next() as IGeoFeatureLayer;
                    //IWorkspace ws = null;

                    //while (featLayer != null)
                    //{
                    //    if (((IDataset)featLayer.FeatureClass) != null)
                    //        return ((IDataset)featLayer.FeatureClass).Workspace;
                    //    featLayer = featLayers.Next() as IGeoFeatureLayer;
                    //}
                    
                    //use the arFM login to get the workspace instead of looping through layers
                    Miner.Interop.IMMWorkspaceManager manager = new Miner.Interop.MMWorkspaceManagerClass();
                    _workspace = manager.GetWorkspace(null, (int)Miner.Interop.mmWorkspaceOptions.mmWSOIncludeLogin);

                }
                return _workspace;
            }
        }
        public static IWorkspace CurrentWorkspace
        {
            get
            {               
                //use the arFM to get the workspace.
                Miner.Interop.IMMWorkspaceManager manager = new Miner.Interop.MMWorkspaceManagerClass();
                _Currentworkspace = manager.GetWorkspace(null, (int)Miner.Interop.mmWorkspaceOptions.mmWSOReturnFirst);
                return _Currentworkspace;
            }
        }
        public static IEnumLayer GetLayers(String guid)
        {
            UID uid = new UIDClass();
            uid.Value = guid; // IGeoFeatureLayer
            return Document.ActiveView.FocusMap.get_Layers(uid, true);
        }

        internal static string GetCurrentLoginUser()
        {
            try
            {
                string _currentUser = string.Empty;
                Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                object obj = Activator.CreateInstance(type);
                IApplication app = (IApplication)obj;
                _currentUser = ((IMMLogin2)app.FindExtensionByName("MMPROPERTIESEXT")).LoginObject.UserName;
                return _currentUser;
            }
            catch
            {
                throw;
            }
        }

        internal bool GetValidUser(string _currentUser)
        {
            bool isvaliduser = false;

            try
            {
                if (_connection != null)
                {
                    string userID = GetUserID(_currentUser);
                    if (!string.IsNullOrEmpty(userID))
                    {
                        DataTable dt = GetValidRole(userID, "406");
                        if (dt.Rows.Count == 0)
                        {
                            isvaliduser = false;
                        }
                        else
                            isvaliduser = true;
                    }
                }
            }
            catch
            {
                isvaliduser = false;
                return isvaliduser;
                throw;
            }
            return isvaliduser;
        }

        private DataTable GetValidRole(string userID, string roleid)
        {
            DataTable dt = null;
            try
            {
                string query = "select role_id from process.mm_px_user_role where user_id = " + userID + " and role_id =" + roleid;
                dt = Fill(query, "");
                return dt;
            }
            catch
            {
                return null;
            }
        }

        private string GetUserID(string _currentUser)
        {
            try
            {
                string query = "select user_id from process.mm_px_user where user_name = upper('" + _currentUser + "')";
                DataTable dt = Fill(query, "");
                DataRow row = dt.Rows[0];
                string userid = (row[0] == DBNull.Value) ? "" : row[0].ToString();
                return userid;
            }
            catch
            {
                return null;
            }
        }

    }
}
