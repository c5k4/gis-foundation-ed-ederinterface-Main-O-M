using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;

using Miner.Interop;


namespace PGE.Interfaces.MapBooksPrintUI
{
    public static class Common
    {
        public const string KeyPDFBaseDirectory = "PDFBaseDirectory";
        public const string KeyAdobeReaderDirectory = "AdobeReaderDirectory";
        public const string KeyExpandByDefault = "ExpandByDefault";
        public static string AdobeReaderExe = "";
        public static string PDFBaseDirectory = "";
        public static bool ExpandByDefault = false;

        public const string PromptWindowTitle = "Map Print";
        public const string SelectAllText = "(Select All)";

        public const int WindowFullHeight = 415;
        public const int WindowCondensedHeight = 250;
        public const int WindowResizeIncrement = 5;

        private static string _dbConnectionRegKey;
        private static string _dbConnectionStringKey;
        private static string _dbUserKey;
        private static string _dbPwdKey;

        public static string MapBookDBConnectionRegKey
        {
            get { return _dbConnectionRegKey; }
        }

        public static string MapBookDBConnectionKeyName
        {
            get { return _dbConnectionStringKey; }
        }
        public static string MapBookDBUserKeyName
        {
            get { return _dbUserKey; }
        }
        public static string MapBookDBPasswordKeyName
        {
            get { return _dbPwdKey; }
        }

        public struct ClassModelNames
        {
            public const string CircuitSource = "CIRCUITSOURCE";
            public const string MapGrid = "PGE_MAPGRID";
            public const string MapPrint = "PGE_MAPPRINT";
        }

        public struct FieldModelNames
        {
            public const string SubstationID = "SUBSTATIONID";
            public const string FeederID = "FEEDERID";
            public const string FeederName = "FEEDERNAME";
            public const string MapGridName = "MAPGRIDNAME";
            public const string MapScale = "PGE_MAPSCALE";
        }


        static Common()
        {
            _dbConnectionRegKey = ConfigurationManager.AppSettings["MapBookDBConnectionRegKey"];
            _dbConnectionStringKey = ConfigurationManager.AppSettings["MapBookDBConnectionKeyName"];
            _dbUserKey = ConfigurationManager.AppSettings["MapBookDBUserKeyName"];
            _dbPwdKey = ConfigurationManager.AppSettings["MapBookDBPasswordKeyName"];
        }

        /// <summary>
        /// This method will return the collection of netwotrks from the specified workspace
        /// </summary>
        /// <param name="networkName"></param>
        /// <returns></returns>
        public static Dictionary<string, IGeometricNetwork> GetNetworks(IWorkspace ws, out List<string> networkList, string weightName = "")
        {
            Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();
            networkList = new List<string>();
            IGeometricNetwork geomNetwork = null;
            IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
            enumDataset.Reset();

            IDataset dsName = enumDataset.Next();
            while (dsName != null)
            {
                IFeatureDataset featureDataset = dsName as IFeatureDataset;
                if (featureDataset != null)
                {
                    IEnumDataset geomDatasets = featureDataset.Subsets;
                    INetworkCollection networkCollection = featureDataset as INetworkCollection;
                    for (int index = 0; index <= networkCollection.GeometricNetworkCount - 1; index++)
                    {
                        geomNetwork = networkCollection.get_GeometricNetwork(index);

                        if (!string.IsNullOrEmpty(weightName))
                        {
                            INetSchema networkSchema = geomNetwork.Network as INetSchema;
                            INetWeight networkWeight = networkSchema.get_WeightByName(weightName);
                            if (networkWeight != null)
                            {
                                geomNetworks.Add((geomNetwork as IDataset).BrowseName, geomNetwork);
                                networkList.Add((geomNetwork as IDataset).BrowseName);
                            }
                        }
                        else
                        {
                            geomNetworks.Add((geomNetwork as IDataset).BrowseName, geomNetwork);
                            networkList.Add((geomNetwork as IDataset).BrowseName);
                        }
                    }

                    //geomDatasets.Reset();
                    //IDataset geomDataset = geomDatasets.Next();
                    //while (geomDataset != null)
                    //{
                    //    if (geomDataset is IGeometricNetwork)
                    //    {
                    //        geomNetwork = geomDataset as IGeometricNetwork;
                    //        geomNetworks.Add(geomDataset.BrowseName, geomNetwork);
                    //        networkList.Add(geomDataset.BrowseName);
                    //    }
                    //    geomDataset = geomDatasets.Next();
                    //}
                }

                if (geomNetwork != null) { break; }
                dsName = enumDataset.Next();
            }

            if (geomNetworks.Count > 0)
            {
                return geomNetworks;
            }
            else
            {
                throw new Exception("Could not find the geometric networks in the specified database");
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
                Console.WriteLine("Unable to determine user name and password.  Please run PGE.Desktop.LoginEncryptor.exe");
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


    }
}
