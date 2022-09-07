using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Framework;
using System.Configuration;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;
using System.Security.Principal;
using Miner.Interop;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// 
    /// </summary>
    public class TableSequenceFacilityIDGenerator:IFacilityIDGenerator
    {
        private const string FacilityIDTable = "MM_CUSTOM_FACILITY_ID";
        private const string LockTable = "MM_CUSTOM_FACILITY_ID_LOCK";
        private const string FeatureclassField = "FEATURECLASSNAME";
        private const string CurrentValueField = "CURRENTVALUE";
        private const string PrefixField = "PREFIX";
        private const string IncrementField = "STEPVALUE";
        private const string UserNameField = "USERNAME";
        private string updateSQL = "update {0} set {1} = {2} where {3}='{4}'";
        //private string sumSQL = "(select {0}+{1} from {2} where {3}='{4}')";

        #region IFacilityIDGenerator Members
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetFacilityID(IObject obj)
        {
            ICursor cursor = null;
            IDataset ds = null;
            string retVal = string.Empty;
            try
            {
                string tableName = FacilityIDTable;
                int prefixIndex = -1;
                int currentValueIndex = -1;
                int incrementIndex = -1;
                ds = (IDataset)obj.Class;
                IFeatureWorkspace fws = (IFeatureWorkspace)ds.Workspace;
                if (ds.Workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                    tableName = "SDE." + tableName;
                ITable table = fws.OpenTable(tableName);
                prefixIndex = table.FindField(PrefixField);
                currentValueIndex = table.FindField(CurrentValueField);
                incrementIndex = table.FindField(IncrementField);
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = FeatureclassField + "='" + ds.Name + "'";
                //Add these lines if there is a severe concurrency issue seen while testing. 
                //Even after adding this line there is a chance for concurrency issue to reappear.
                if (CheckandLockRowForUser(ds.Name, ds.Workspace, GetCurrentUserName()))
                {
                    cursor = table.Search(qf, false);
                    IRow row = cursor.NextRow();
                    if (row != null)
                    {
                        //ds.Workspace.ExecuteSQL(string.Format(updateSQL, tableName, CurrentValueField, 
                        //    string.Format(sumSQL,CurrentValueField,IncrementField,FacilityIDTable,FeatureclassField,ds.Name),
                        //    FeatureclassField,ds.Name));
                        //if (ds.Workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                        //    ds.Workspace.ExecuteSQL("commit");
                        string currVal = GetDefaultValue(row.get_Value(currentValueIndex), "1");
                        string prefixVal = GetDefaultValue(row.get_Value(prefixIndex), string.Empty);
                        string increment = GetDefaultValue(row.get_Value(incrementIndex), "1");
                        retVal = prefixVal + currVal;
                        ds.Workspace.ExecuteSQL(string.Format(updateSQL, tableName, CurrentValueField,
                                                                Convert.ToInt32(currVal) + Convert.ToInt32(increment),
                                                                FeatureclassField, ds.Name));
                        if (ds.Workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                            ds.Workspace.ExecuteSQL("commit");

                    }
                }
                else
                    throw new COMException("Unable to lock table for facilityid generation - Try after sometime", (int)mmErrorCodes.MM_E_CANCELEDIT);
                return retVal;
            }
            catch (COMException e)
            {
                if (e.ErrorCode == (int)mmErrorCodes.MM_E_CANCELEDIT)
                    throw;
                throw new COMException("Failed getting facility id - /n"+e.InnerException, (int)mmErrorCodes.MM_E_CANCELEDIT); ;
            }
            catch(Exception ex)
            {
                throw new COMException("Failed getting facility id  - /n" + ex.StackTrace, (int)mmErrorCodes.MM_E_CANCELEDIT); ;
            }
            finally
            {
                if(cursor!=null)
                    while (Marshal.ReleaseComObject(cursor) > 0) { };
                if (ds != null) ReleaseLock(ds.Name, ds.Workspace); 
                    
            }

        }

        #endregion

        private string GetDefaultValue(object value, string defaultVal)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultVal;
            else
                return value.ToString();
        }

        private bool CheckandLockRowForUser(string featureclassname,IWorkspace ws,string userName)
        {
            IFeatureWorkspace fws=(IFeatureWorkspace)ws;
            IQueryDef querydef = fws.CreateQueryDef();
            querydef.Tables = GetQualifiedTableName(ws, LockTable);
            querydef.WhereClause = FeatureclassField + "='" + featureclassname.ToUpper()+"'";
            ICursor cursor = null;
            try
            {
                cursor = querydef.Evaluate();
                IRow row = cursor.NextRow();
                if (row != null)
                {
                    if (row.get_Value(row.Fields.FindField(UserNameField)).ToString().ToUpper() != userName.ToUpper())
                    {
                        CheckandLockRowForUser(featureclassname, ws, userName);
                    }
                    else
                        return true;
                }
                else
                {
                    ws.ExecuteSQL("insert into " + querydef.Tables + " values ('" + featureclassname.ToUpper() + "','" + userName.ToUpper() + "')");
                    ws.ExecuteSQL("commit");
                    return true;
                }
            }
            catch { throw; }
            finally
            {
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                }
            }
            return false;
        }

        private void ReleaseLock(string featureclassname,IWorkspace ws)
        {
            IFeatureWorkspace fws = (IFeatureWorkspace)ws;
            IQueryDef querydef = fws.CreateQueryDef();
            querydef.Tables = GetQualifiedTableName(ws, LockTable);
            querydef.WhereClause = FeatureclassField + "='" + featureclassname.ToUpper() + "' and "+UserNameField+"='"+GetCurrentUserName().ToUpper()+"'";
            ICursor cursor = null;
            try
            {
                cursor = querydef.Evaluate();
                IRow row = cursor.NextRow();
                if (row != null)
                {
                    ws.ExecuteSQL("delete from " + querydef.Tables + " where " + FeatureclassField+"='"+ featureclassname.ToUpper() + "' and "+ UserNameField +"='"+ GetCurrentUserName().ToUpper() + "'");
                    ws.ExecuteSQL("commit");
                }
            }
            catch { throw; }
            finally
            {
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                }
            }
        }

        private string GetCurrentUserName()
        {
            return WindowsIdentity.GetCurrent().Name;
        }

        private string GetQualifiedTableName(IWorkspace workspace,string tableName)
        {
            if (workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                    return "SDE." + tableName;
            else
                return tableName;
        }
    }
}
