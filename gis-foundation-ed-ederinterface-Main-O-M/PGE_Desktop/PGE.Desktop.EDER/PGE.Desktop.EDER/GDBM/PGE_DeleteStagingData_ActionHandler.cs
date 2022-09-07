// ========================================================================
// Copyright © 2021 PGE 
// <history>
// Delete data from Staging Area 
// YXA6 4/14/2021	Created
// </history>
// All rights reserved.
// ========================================================================
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGE.Desktop.EDER.GDBM
{
    class PGE_DeleteStagingData_ActionHandler : PxActionHandler
    {
        public PGE_DeleteStagingData_ActionHandler()
        {
            this.Name = "PGE Delete Staging Data Updater";
            this.Description = "If Session falied in GDBM then Delete  Staging Table data.";
        }

        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {

            return true;
        }
        /// <summary>
        /// If session failed in GDBM then this action handler will rollback the data
        /// </summary>
        /// <param name="actionData"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
            bool success = false;
            string strDeleteQuery = string.Empty;

            try
            {
                if (actionData == null) return true;
                if (actionData.Version == null) return true;
               
                IWorkspace WEditor = (IWorkspace)actionData.Version;
             
                success = true;
                this.Log.Info(ServiceConfiguration.Name, "Start- Delete the data from Staging table for Version-: " + actionData.VersionName);
                //Delete the data for failed session
                strDeleteQuery = "Delete from " + SchemaInfo.General.pAHInfoTableName + " Where VERSIONNAME = '" + actionData.VersionName.ToString() + "'";

                (new DBHelper()).UpdateQuery(strDeleteQuery);
                this.Log.Info(ServiceConfiguration.Name, "Complete -Delete the data from Staging table for Version-: " + actionData.VersionName);

            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name," Query is throwing error -- " + strDeleteQuery);
                this.Log.Error(ServiceConfiguration.Name, "Exception occurred while deleting the records from staging table: " + ex.Message + " StackTrace: " + ex.StackTrace);

            }
            return success;
        }
    }
}
