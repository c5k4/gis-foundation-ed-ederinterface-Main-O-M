using System;
using System.Collections;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using Miner.Geodatabase;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using Miner.Process.GeodatabaseManager;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using Telvent.Delivery.Framework;
using Telvent.Delivery.Geodatabase.ChangeDetection;
using Telvent.PGE.ED.Desktop.Utility;

using System.Windows.Forms;
namespace Telvent.PGE.ED.Desktop.GDBM
{
    public class PGEOperatingNumberHandler : PxActionHandler
    {
        public PGEOperatingNumberHandler()
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            Name = "PGE Operating Number Exists Handler";
            Description =
                "Updates the tables in the WIP database that track operating numbers for devices and job numbers for a set of operating numbers.";

            this.Log.Info("(PGEOperatingNumberHandler) action handler name = " + Name + " description " + Description);
        }
        
        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            return true;
        }

       

        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            try
            {
                if ((parameters[0].Value.ToString() == null) || (parameters[1].Value.ToString() == null))
                {
                    this.Log.Info("PGEOperatingNumberHandler - Error: Null Parameters");
                }
                

                if (ExecuteActionHandler(actionData.Version, parameters[0].Value, parameters[1].Value))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception EX)
            {
                this.Log.Error(EX.Message + " - " + EX.StackTrace);
                throw EX;
            }
        }

        private bool ExecuteActionHandler(IVersion v, String username, String password)
        {
            try
            {
                this.Log.Info("PGEOperatingNumberHandler - Execute Action Handler...");

                WipDbApi api = new WipDbApi(username, password, this.Log);

                IWorkspace workspace = (IWorkspace)v;
                IVersionedWorkspace versionWs = (IVersionedWorkspace)v;

                DifferenceManager differenceManager = new DifferenceManager(versionWs.DefaultVersion, v);
                differenceManager.Initialize();

                List<DifferenceDictionary> dicts = new List<DifferenceDictionary>();
                Dictionary<String, IList<String>> jobNumsDictOpNum = new Dictionary<String, IList<String>>();
                Dictionary<String, IList<String>> jobNumsDictCGC12 = new Dictionary<String, IList<String>>();

                this.Log.Info("PGEOperatingNumberHandler.ExecuteActionHandler - Populate Differences Dictionary (Inserts and Updates)...");
                
                differenceManager.LoadDifferences(false,
                    new[] { esriDataChangeType.esriDataChangeTypeUpdate, esriDataChangeType.esriDataChangeTypeInsert });
                dicts.Add(differenceManager.Inserts);
                dicts.Add(differenceManager.Updates);

                foreach (DifferenceDictionary dict in dicts)
                {
                    foreach (KeyValuePair<string, DiffTable> kvp in dict)
                    {
                        DiffTable currentDiffTable = kvp.Value;
                        IEnumerator<DiffRow> diffRowEnumerator = currentDiffTable.GetEnumerator();

                        diffRowEnumerator.Reset();
                        while (diffRowEnumerator.MoveNext())
                        {
                            IRow diffRow = diffRowEnumerator.Current.GetIRow(workspace, kvp.Value);

                           // if (ModelNameFacade.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.CGC12))

                            if (ModelNameManager.Instance.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.CGC12))
                            {
                                String jobNum = diffRow.get_Value(diffRow.Fields.FindField("INSTALLJOBNUMBER")).ToString();

                                if (DBNull.Value.Equals(diffRow.get_Value(diffRow.Fields.FindField("CGC12"))))
                                {
                                    //if (ModelNameFacade.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.OperatingNumber))
                                    if (ModelNameManager.Instance.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.OperatingNumber))
                                    {
                                        String opNum = diffRow.get_Value(diffRow.Fields.FindField("OPERATINGNUMBER")).ToString();

                                        if (!jobNumsDictOpNum.ContainsKey(jobNum)) jobNumsDictOpNum[jobNum] = new List<string>();
                                        jobNumsDictOpNum[jobNum].Add(opNum);

                                        this.Log.Info("PGEOperatingNumberHandler.ExecuteActionHandler - Values - Job Number " + jobNum + " Operating Number =  " + opNum);
                                    }
                                }
                                    else
                                    {
                                         String cgc12 = diffRow.get_Value(diffRow.Fields.FindField("CGC12")).ToString();

                                        if (!jobNumsDictCGC12.ContainsKey(jobNum)) jobNumsDictCGC12[jobNum] = new List<string>();
                                        jobNumsDictCGC12[jobNum].Add(cgc12);
                                    }
                            }
                            //else if (ModelNameFacade.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.OperatingNumber))
                            else if (ModelNameManager.Instance.ContainsClassModelName((IObjectClass)diffRow.Table, SchemaInfo.Electric.ClassModelNames.OperatingNumber))
                            {
                                String jobNum = diffRow.get_Value(diffRow.Fields.FindField("INSTALLJOBNUMBER")).ToString();
                                String opNum = diffRow.get_Value(diffRow.Fields.FindField("OPERATINGNUMBER")).ToString();

                                if (!jobNumsDictOpNum.ContainsKey(jobNum)) jobNumsDictOpNum[jobNum] = new List<string>();
                                jobNumsDictOpNum[jobNum].Add(opNum);

                                this.Log.Info("PGEOperatingNumberHandler.ExecuteActionHandler - Values - Job Number " + jobNum + " Operating Number =  " + opNum);

                            }
                        }
                    }
                }

                api.CheckRetireInJETJobsTable(jobNumsDictOpNum, true);
                api.CheckRetireInJETJobsTable(jobNumsDictCGC12, false);

                return true;
            }
            catch (Exception EX)
            {
                Log.Error(EX.Message + " - " + EX.StackTrace);
                throw EX;

                return false;
            }
        }
    }
}
