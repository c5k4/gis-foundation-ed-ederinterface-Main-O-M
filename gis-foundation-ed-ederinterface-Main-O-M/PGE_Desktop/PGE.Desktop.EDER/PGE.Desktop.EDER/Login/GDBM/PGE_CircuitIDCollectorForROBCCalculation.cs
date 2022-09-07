using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.Framework.FeederManager;
using Miner.Framework.Trace.Strategies;
using Miner.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.NetworkAnalysis;
using Telvent.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using Miner.Geodatabase.GeodatabaseManager.Serialization;

namespace Telvent.PGE.ED.Desktop.GDBM
{
    class PGE_CircuitIDCollectorForROBCCalculation : PxActionHandler
    {

        public PGE_CircuitIDCollectorForROBCCalculation()
        {
            this.Name = "PGE CircuitID Collector For ROBC calculation";
            this.Description = "Collects circuit IDs from the posted features to recalculate Established ROBC";
        }

        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {
//#if DEBUG
//            System.Diagnostics.Debugger.Launch();
//#endif
            this.Log.Info("Collecting Circuit IDs to recalculate ROBC: " + actionData.Version.VersionName);

            try
            {
                HashSet<string> htCircuitIds = new HashSet<string>();

                DifferenceDictionary insertededDictionary = null;
                DifferenceDictionary updatedDictionary = null;
                DifferenceDictionary deletedDictionary = null;

                IVersionedWorkspace versionWS = actionData.Version as IVersionedWorkspace;

                this.Log.Info("Initializing DifferenceManager... ");
                DifferenceManager differenceManager = new DifferenceManager(versionWS.DefaultVersion, actionData.Version);
                differenceManager.Initialize();

                this.Log.Info("Loading differences for insert, update and delete...");
                differenceManager.LoadDifferences(false, esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate, esriDataChangeType.esriDataChangeTypeDelete);

                insertededDictionary = differenceManager.Inserts;
                updatedDictionary = differenceManager.Updates;
                deletedDictionary = differenceManager.Deletes;

                this.Log.Info("Fetching circuit IDs for inserts... ");
                Dictionary<string, DiffTable>.Enumerator inserts = insertededDictionary.GetEnumerator();
                while (inserts.MoveNext())
                {
                    IEnumerable<IRow> insertedRows = differenceManager.GetInserts(inserts.Current.Key, "");
                    IEnumerator<IRow> insertsRowsEnum = insertedRows.GetEnumerator();

                    while (insertsRowsEnum.MoveNext())
                    {
                        IRow row = insertsRowsEnum.Current;
                        List<string> circuits = GetCircuitIDs(row);
                        foreach (string circuitID in circuits)
                        {
                            if (!htCircuitIds.Contains(circuitID))
                                htCircuitIds.Add(circuitID);
                        }
                    }
                }

                this.Log.Info("Fetching circuit IDs for updates... ");
                Dictionary<string, DiffTable>.Enumerator updates = updatedDictionary.GetEnumerator();
                while (updates.MoveNext())
                {
                    this.Log.Info("Inside updates... ");
                    IEnumerable<IRow> updatedRows = differenceManager.GetUpdates(updates.Current.Key, "");
                    IEnumerator<IRow> updatesRowsEnum = updatedRows.GetEnumerator();

                    while (updatesRowsEnum.MoveNext())
                    {
                        IRow row = updatesRowsEnum.Current;
                        IFeatureClass fc = FeederManager2.GetFeatureClassFromRow(row);

                        List<string> circuits = GetCircuitIDs(row);
                        foreach (string circuitID in circuits)
                        {
                            if (!htCircuitIds.Contains(circuitID))
                                htCircuitIds.Add(circuitID);
                        }
                    }
                }

                this.Log.Info("Fetching circuit IDs for deletes... ");
                Dictionary<string, DiffTable>.Enumerator deletes = deletedDictionary.GetEnumerator();
                while (deletes.MoveNext())
                {
                    IEnumerable<IRow> deletedRows = differenceManager.GetDeletes(deletes.Current.Key, "");
                    IEnumerator<IRow> deletesRowsEnum = deletedRows.GetEnumerator();

                    while (deletesRowsEnum.MoveNext())
                    {
                        IRow row = deletesRowsEnum.Current;
                        List<string> circuits = GetCircuitIDs(row);
                        foreach (string circuitID in circuits)
                        {
                            if (!htCircuitIds.Contains(circuitID))
                                htCircuitIds.Add(circuitID);
                        }
                    }
                }

                this.Log.Info("Circuit count: " + htCircuitIds.Count);
                if (htCircuitIds.Count > 0)
                {
                    string[] circuitIDs = htCircuitIds.ToArray<string>();
                    actionData["ROBCCircuitIDs"] = string.Join(",", circuitIDs);
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during collecting Circuit IDs for ROBC processing: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }

            return false;
        }

        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Miner.Geodatabase.GeodatabaseManager.Actions gdbmAction, Miner.Geodatabase.GeodatabaseManager.PostVersionType versionType)
        {
            if (actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post && gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.BeforePost)
            {
                return true;
            }
            else
                return false;
        }

        private List<string> GetCircuitIDs(IRow row)
        {
            ITable tbl = row.Table;
            List<string> circuitsIDs = new List<string>();

            int circuitIDIndex = GetFieldIxFromModelName(tbl as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID);
            int circuitID2Index = GetFieldIxFromModelName(tbl as IObjectClass, SchemaInfo.Electric.FieldModelNames.CircuitID2);

            if (circuitIDIndex != -1)
            {
                object circuitID = row.get_Value(circuitIDIndex);
                if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                {
                    circuitsIDs.Add(circuitID.ToString());
                }
            }

            if (circuitID2Index != -1)
            {
                object circuitID = row.get_Value(circuitID2Index);
                if (circuitID != null && !string.IsNullOrEmpty(circuitID.ToString()))
                {
                    circuitsIDs.Add(circuitID.ToString());
                }
            }

            return circuitsIDs;
        }


        private int GetFieldIxFromModelName(IObjectClass table, string fieldModelName)
        {
            int fieldIndex = -1;
            try
            {
                IField field = ModelNameManager.Instance.FieldFromModelName(table, fieldModelName);
                if (field != null)
                {
                    fieldIndex = table.FindField(field.Name);
                }
            }
            catch
            {
                fieldIndex = -1;
            }

            return fieldIndex;
        }
    }
}
