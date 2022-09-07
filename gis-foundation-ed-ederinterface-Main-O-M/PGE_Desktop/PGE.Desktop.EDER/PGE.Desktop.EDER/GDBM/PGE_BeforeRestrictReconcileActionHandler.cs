// ========================================================================
// Copyright © 2021 PGE 
// <history>
// Restrict Reconcile Process through GDBM as session should reconcile only once in 24 hours 
// YXA6 4/14/2021	Created
// </history>
// All rights reserved.
// ========================================================================
using ADODB;
using ESRI.ArcGIS.DataSourcesOleDB;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Interop.Process;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using PGE.Desktop.EDER.Login;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGE.Desktop.EDER.GDBM
{
    class PGE_BeforeRestrictReconcileActionHandler: PxActionHandler
    {
        IMMPxApplication _pxApplication = null;
        public PGE_BeforeRestrictReconcileActionHandler()
        {
            this.Name = "PGE Before Restrict Reconcile Process";
            this.Description = "Make the entry in GDBM.GDBM_NO_RECONCILE_VERSIONS table to Restrict the reconcile if already done -once in 24 hours(configurable).";

        }
        #region Overidden methods
        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {

            if(gdbmAction == Actions.BeforeReconcile)
            {
               // IWorkspace pworkspace = GetWorkspace("C:\\EDER\\Data\\Connections\\edgis@eder.sde");
                //ExecuteActionHandler(pworkspace, pworkspace as IVersion);
                return true;
            }
            return false;
        }
        #region Getting Workspace
        public IWorkspace GetWorkspace(string argStrworkSpaceConnectionstring)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {
                // this.Log.Info("Connection string used in process ." + ReadConfigurations.GetValue(ReadConfigurations.SDEConnectionString));
                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(argStrworkSpaceConnectionstring, 0);
                //this.Log.Info("Workspace checked out successfully.");

                if (workspace == null)
                {
                    // this.Log.Error("Workspace not found for conn string : " + ReadConfigurations.GetValue(ReadConfigurations.SDEConnectionString));
                    throw new Exception("Exiting the process.");

                }

            }
            catch (Exception exp)
            {
                throw exp;
            }
            return workspace;
        }
        #endregion

        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            IWorkspace pworkspace = (IWorkspace)actionData.Version;
            ExecuteActionHandler(pworkspace, actionData.Version);
            return true;
        }

        private void ExecuteActionHandler(IWorkspace pworkspace,IVersion pVersion)
        {
            Random _random = new Random();
                bool brunAH = false;
                string sversionname = string.Empty;
                string strQuery = string.Empty;
            try
            {
                
                //Query to find that queried version already been reconciled in last 24 hours
                strQuery = "select * from SDE.GDBM_RECONCILE_HISTORY where  trunc(RECONCILE_START_DT)= TRUNC(sysdate) and version_name='" + pVersion.VersionName + "'";
                //If yes then skip the reconcile process  \
                this.Log.Debug(ServiceConfiguration.Name, "Check Version is reconciled or not in last 24 hours with query " + strQuery);


                // CHeck data
                Recordset pRSet = new Recordset();
                pRSet.CursorLocation = CursorLocationEnum.adUseClient;
                IFDOToADOConnection _fdoToadoConnection = new FdoAdoConnectionClass();

                ADODB.Connection _adoConnection  = _fdoToadoConnection.CreateADOConnection(pworkspace)  as  ADODB.Connection;

                pRSet.Open(strQuery, _adoConnection, ADODB.CursorTypeEnum.adOpenStatic, ADODB.LockTypeEnum.adLockBatchOptimistic, 0);

               
                this.Log.Debug(ServiceConfiguration.Name, "RecordCount=" + pRSet.RecordCount);

                if (pRSet.RecordCount > 0)
                {

                    this.Log.Debug(ServiceConfiguration.Name, "Sesion already reconciled in last 24 hours so skip the reconcile process.");
                    brunAH = true;
                   
                }
                pRSet.Close();
                if (_adoConnection.State == 1)
                {
                    _adoConnection.Close();
                }
                //Sesion already reconciled in last 24 hours so skip the reconcile process by inserting the sesion name in SDE.GDBM_NO_RECONCILE_VERSIONS table 
                if (brunAH == true)
                {
                    string[] words = pVersion.VersionName.Split('.');
                    int ID = _random.Next(2200, 9999);
                    if (words.Length > 0)
                    {
                        if (words.Length == 1)
                        {
                            sversionname = words[0];
                            strQuery = "insert into sde.GDBM_NO_RECONCILE_VERSIONS values(" + ID + ",'" + string.Empty + "','" + words[0] + "',0)";

                        }
                        else if (words.Length == 2)
                        {
                            sversionname = words[1];
                            strQuery = "insert into sde.GDBM_NO_RECONCILE_VERSIONS values(" + ID + ",'" + words[0] + "','" + words[1] + "',0)";
                        }
                    }

                    pworkspace.ExecuteSQL(strQuery);
                    this.Log.Info(ServiceConfiguration.Name, "Made entry in GDBM_NO_RECONCILE_VERSIONS table--" + strQuery);
                }
                else
                {
                    this.Log.Info(ServiceConfiguration.Name, "Made entry in GDBM_NO_RECONCILE_VERSIONS table--" + strQuery);
                }
            }
            catch (Exception ex)
            {
                this.Log.Info(ServiceConfiguration.Name, ex.Message.ToString() + " Query-  " + strQuery);

            }
        }
        #endregion
    }
}
