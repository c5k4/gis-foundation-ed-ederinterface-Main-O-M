using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;

namespace PGE.BatchApplication.TraceAllFeeders.Common
{
    public static class Common
    {
        #region Public variables

        public const string InputFileDirectory = "Trace all feeders input files";
        public const string LogFileDirectory = "TraceLogs";
        public static string BaseVersion = "PGE_TraceAllFeeders";
        public static string BaseChildVersion = "PGE.BatchApplication.TraceAllFeeders";
        public static string Reconciling = "";
        public static string Posting = "";
        public static string ReconcileAndPostMailSlot = @"\\.\mailslot\TraceAllFeedersReconcileList";
        public static string TraceAllReTraceListMailSlot = @"\\.\mailslot\TraceAllFeedersToRunList";
        public static string TraceAllFinishedMailSlot = @"\\.\mailslot\TraceAllFeedersFinishedList";

        #endregion

        #region Private static variables

        

        #endregion

        #region Public Static methods

        public static void CreateMailSlots(string mailBoxName)
        {
            
        }

        /// <summary>
        /// Returns an ITable given the workspace and modelname
        /// </summary>
        /// <param name="ws">Workspace to search</param>
        /// <param name="ModelName">Class ModelName</param>
        /// <returns></returns>
        public static ITable GetTableByModelName(IWorkspace ws, string ModelName)
        {
            IMMModelNameManager mnm = Miner.Geodatabase.ModelNameManager.Instance;
            if (mnm != null)
            {
                IMMEnumTable enumTable = mnm.TablesFromModelNameWS(ws, ModelName);
                enumTable.Reset();
                ITable table = enumTable.Next();
                return table;
            }
            return null;
        }

        /// <summary>
        /// Returns the field index of the field containing the specified field model name
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static int FieldIndexFromModelName(ITable table, string fieldModelName, ref string fieldName)
        {
            IMMModelNameManager mnm = Miner.Geodatabase.ModelNameManager.Instance;
            if (mnm != null)
            {
                IField field = mnm.FieldFromModelName(table as IObjectClass, fieldModelName);
                if (field != null)
                {
                    fieldName = field.Name;
                    return table.FindField(field.Name);
                }
            }
            return -1;
        }

        /// <summary>
        /// This method will return the INetwork with the specified network name from the specified workspace
        /// </summary>
        /// <param name="networkName"></param>
        /// <returns></returns>
        public static IGeometricNetwork GetNetwork(IWorkspace ws, string networkName)
        {
            IGeometricNetwork network = null;
            IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            enumDataset.Reset();
            IDataset dsName = enumDataset.Next();
            while (dsName != null)
            {
                IFeatureDataset featDataset = dsName as IFeatureDataset;
                if (featDataset != null)
                {
                    IEnumDataset geomDatasets = featDataset.Subsets;
                    geomDatasets.Reset();
                    IDataset geomDataset = geomDatasets.Next();
                    while (geomDataset != null)
                    {
                        if (geomDataset.BrowseName.ToUpper() == networkName.ToUpper())
                        {
                            network = geomDataset as IGeometricNetwork;
                            break;
                        }
                        geomDataset = geomDatasets.Next();
                    }
                }

                if (network != null) { break; }
                dsName = enumDataset.Next();
            }

            if (network != null)
            {
                return network;
            }
            else
            {
                throw new Exception("Could not find the specified geometric network in the specified database");
            }
        }

        /// <summary>
        /// Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string directConnectString, string user, string pass)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                Console.WriteLine("Unable to determine user name and password.  Please run Telvent.PGE.LoginEncryptor.exe");
                return null;
            }

            // Create and populate the property set to connect to the database.
            ESRI.ArcGIS.esriSystem.IPropertySet propertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
            propertySet.SetProperty("SERVER", "");
            propertySet.SetProperty("INSTANCE", directConnectString);
            propertySet.SetProperty("DATABASE", "");
            propertySet.SetProperty("USER", user);
            propertySet.SetProperty("PASSWORD", pass);
            propertySet.SetProperty("VERSION", "sde.DEFAULT");

            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();
            return wsFactory.Open(propertySet, 0);
        }


        #endregion

        #region Private static methods


        #endregion

    }
}
