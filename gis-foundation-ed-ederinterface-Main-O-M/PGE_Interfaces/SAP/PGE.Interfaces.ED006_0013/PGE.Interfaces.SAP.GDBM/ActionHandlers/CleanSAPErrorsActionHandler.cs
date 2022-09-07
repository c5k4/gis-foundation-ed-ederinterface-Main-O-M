using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.GeodatabaseManager.ActionHandlers;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;

namespace PGE.Interfaces.SAP.GDBM.ActionHandlers
{
    public class CleanSAPErrorsActionHandler : ActionHandler
    {
        public CleanSAPErrorsActionHandler():base()
        {
            base.Name = "Clean SAP Errors";
            base.Description = "Cleans SAP errors belonging to the posting session.";            
        }

        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            bool enabled = false;

            //Enable only after Posting sessions
            if (actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post &&
                gdbmAction == Actions.AfterPost &&
                versionType == PostVersionType.Session)
            {
                enabled = true;
            }

            return enabled;
        }

        protected override bool SubExecute(ActionData actionData, GdbmParameter[] parameters)
        {
            string sapErrorTableName = "PROCESS.PGE_SAPERRORS";
            string sapErrors_VersionFieldName = "VERSIONNAME";
            ITable sapErrorsTable = null;
            IQueryFilter filter = null;

            try
            {
                //Get the version and its name
                IVersion currentVersion = actionData.Version;                
                //Check whether the version is being edited
                if ((currentVersion as IWorkspaceEdit2).IsBeingEdited() == false)
                {
                    base.Log.Debug(actionData.VersionName + " version must be in edit session."); return false;
                }
                IWorkspace versionedWorkSpace = currentVersion as IWorkspace;
                if ((versionedWorkSpace as IWorkspace2).get_NameExists(esriDatasetType.esriDTTable, sapErrorTableName) == false)
                {
                    base.Log.Debug(sapErrorTableName + " table does not exist."); return false;
                }
                
                //Get the Process table
                sapErrorsTable = (currentVersion as IFeatureWorkspace).OpenTable(sapErrorTableName);
                //Delete the rows where VersionName= current verison name
                filter = new QueryFilterClass();
                filter.WhereClause = sapErrors_VersionFieldName + "='" + actionData.VersionName.ToUpper() + "'";
                //Validate the table
                sapErrorsTable.DeleteSearchedRows(filter);

                return true;
            }
            catch (Exception ex)
            {
                base.Log.Error("Failed to clean '" + sapErrorTableName + "' table.", ex); return false;
            }
            finally
            {
                //Release recources
                if (sapErrorsTable != null)
                {
                    while (Marshal.ReleaseComObject(sapErrorsTable) > 0) { };
                }
                if (filter != null)
                {
                    while (Marshal.ReleaseComObject(sapErrorsTable) > 0) { };
                }
            }


        }
    }
}
