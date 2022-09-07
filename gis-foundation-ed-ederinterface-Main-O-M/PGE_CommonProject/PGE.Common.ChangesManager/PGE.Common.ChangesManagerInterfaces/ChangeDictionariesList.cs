using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using System.Reflection;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Common.ChangesManagerShared
{
    public class ChangeDictionariesList : Collection<ChangeDictionary>
    {
        protected static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        public Action<ITable, ChangeRow> ProcessChangeRow { get; set; }
        public Action<ITable, IList<ChangeRow>> ProcessChangeRowsByChunk { get; set; }
        public Action<ITable, IList<ChangeRow>, ITable> ProcessChangeRowsByChunkSecondary { get; set; }

        public Action<ITable, ChangeRow, IDictionary<string, ChangedFeatures>> CDProcessChangeRow { get; set; }
        public Action<ITable, IList<ChangeRow>, IDictionary<string, ChangedFeatures>> CDProcessChangeRowsByChunk { get; set; }
        public Action<ITable, IList<ChangeRow>, ITable, IDictionary<string, ChangedFeatures>> CDProcessChangeRowsByChunkSecondary { get; set; }

        private int? _inserts = null;
        public int Inserts
        {
            get
            {
                if (_inserts == null)
                {
                    CalculateEditCount();
                }
                return (int)_inserts;
            }
        }
        private int? _updates = null;
        public int Updates
        {
            get
            {
                if (_updates == null)
                {
                    CalculateEditCount();
                }
                return (int)_updates;
            }
        }
        private int? _deletes = null;
        public int Deletes
        {
            get
            {
                if (_deletes == null)
                {
                    CalculateEditCount();
                }
                return (int)_deletes;
            }
        }

        public bool ProcessObjectClasses { get; set; }

        public void CalculateEditCount()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            _inserts = 0;
            _updates = 0;
            _deletes = 0;

            foreach (ChangeDictionary changeDictionary in this)
            {

                switch (changeDictionary.DifferenceTypes.First())
                {
                    case esriDataChangeType.esriDataChangeTypeInsert:
                        foreach (KeyValuePair<string, ChangeTable> p in changeDictionary) { _inserts += p.Value.Count; }
                        break;
                    case esriDataChangeType.esriDataChangeTypeUpdate:
                        foreach (KeyValuePair<string, ChangeTable> p in changeDictionary) { _updates += p.Value.Count; }
                        break;
                    case esriDataChangeType.esriDataChangeTypeDelete:
                        foreach (KeyValuePair<string, ChangeTable> p in changeDictionary) { _deletes += p.Value.Count; }
                        break;
                    default:
                        break;

                }
            }

            
            _logger.Info("Inserts [ " + _inserts.ToString() + " ] Updates [ " + _updates.ToString() + " ] Deletes [ " + _deletes.ToString() + " ]");
        }

        public void ProcessChanges()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            foreach (ChangeDictionary changeDictionary in this)
            {
                _logger.Debug(changeDictionary.ToString());

                foreach (KeyValuePair<string, ChangeTable> changeTableKVP in changeDictionary)
                {
                    _logger.Debug(changeTableKVP.Value.ToString());

                    //Retrieve the table from the object.
                    //Currently not using ChangeRow.GetIRow() as the table object is instantiated every time using that method.
                    //  We'll grab the table once on this level and use it for every change row.
                    ITable changeTable = changeDictionary.AsTable(changeTableKVP.Key);
                    if ((!ProcessObjectClasses && !(changeTable is IFeatureClass)) ||
                        changeTableKVP.Value.Count == 0)
                    {
                        //while (Marshal.ReleaseComObject(changeTable) > 0) { };
                        //continue;

                        /*Changes for ENOS to SAP migration -DMS - change detection for EDER ..Start.*/
                        /*Changes for IGP for EDGIS - requirement CIT*/
                        if (changeTableKVP.Key == "EDGIS.GENERATIONINFO" || changeTableKVP.Key.ToUpper() == "EDGIS.CONDUITSYSTEM_PRIUG")
                        {

                        }
                        else
                        {                            
                            while (Marshal.ReleaseComObject(changeTable) > 0) { };
                            continue;
                        }
                        /*Changes for ENOS to SAP migration -DMS - change detection for EDER ..End.*/

                    }

                    _logger.Info("  Detecting " + Enum.GetName(typeof(esriDataChangeType), changeTableKVP.Value.First().DifferenceType) + " for " + (changeTable as IDataset).Name);
                    _logger.Info("    " + changeTableKVP.Value.Count + " " + Enum.GetName(typeof(esriDataChangeType), changeTableKVP.Value.First().DifferenceType) + " to process");

                    ITable changeTableSecondaryWorkspace = null;
                    if (this.ProcessChangeRowsByChunkSecondary != null && changeDictionary.SecondaryWorkspace != null)
                    {
                        changeTableSecondaryWorkspace =
                        ((IFeatureWorkspace)changeDictionary.SecondaryWorkspace).OpenTable(changeTableKVP.Key);
                    }
 

                    //List for our oids
                    List<ChangeRow> changeRows = new List<ChangeRow>();

                    int counter = 0;
                    //Loop through all change rows in this class.
                    foreach (ChangeRow changeRow in changeTableKVP.Value)
                    {
                        counter++;
                        changeRows.Add(changeRow);

                        //Query every 999 features or if this is our last changeRow
                        if (this.ProcessChangeRowsByChunk != null &&
                            ((counter % 999 == 0) || changeTableKVP.Value.Count == counter))
                        {
                            if (changeDictionary.changedFeatures == null || changeDictionary.changedFeatures.Count == 0)
                            {
                                ProcessChangeRowsByChunk(changeTable, changeRows);
                                changeRows.Clear();
                            }
                            else
                            {
                                CDProcessChangeRowsByChunk(changeTable, changeRows,changeDictionary.changedFeatures);
                                changeRows.Clear();
                            }
                        }
                        //Query every 999 features or if this is our last changeRow
                        if (this.ProcessChangeRowsByChunkSecondary != null &&
                            ((counter % 999 == 0) || changeTableKVP.Value.Count == counter))
                        {
                            if (changeDictionary.changedFeatures == null || changeDictionary.changedFeatures.Count == 0)
                            {
                                ProcessChangeRowsByChunkSecondary(changeTable, changeRows, changeTableSecondaryWorkspace);
                                changeRows.Clear();
                            }
                            else
                            {
                                CDProcessChangeRowsByChunkSecondary(changeTable, changeRows, changeTableSecondaryWorkspace,changeDictionary.changedFeatures);
                                changeRows.Clear();
                            }
                        }
                        if (this.ProcessChangeRow != null)
                        {
                            if (changeDictionary.changedFeatures == null || changeDictionary.changedFeatures.Count == 0)
                            {
                                ProcessChangeRow(changeTable, changeRow);
                            }
                            else
                            {
                                CDProcessChangeRow(changeTable, changeRow,changeDictionary.changedFeatures);
                            }
                        }
                    }
                }
            }
        }
    }
}

