using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using Miner.Interop;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace PGE.BatchApplication.PrintCircuitTrace
{
    public class CircuitTrace
    {       

        public static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.Run(new MapBookUI()); // or whatever
                //MapBookUI map = new MapBookUI();
                //map.Show();
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
            }
        }

        //public static void PopulateCircuitList(IGeometricNetwork geomNetwork)
        //{
        //    IMMFeederExt feederExt = null;
        //    IMMFeederSpace feederSpace = null;
        //    IMMFeederSource feederSource = null;

        //    Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
        //    object obj = Activator.CreateInstance(type);

        //    ITable _circuitSourceTable = ObjectClassByModelName(_workspace, CircuitSource) as ITable;


        //    string subStaionIDFiledName = (ModelNameManager.FieldFromModelName((IObjectClass)_circuitSourceTable, SubstationID)).Name;
        //    string circuitIDFieldName = (ModelNameManager.FieldFromModelName((IObjectClass)_circuitSourceTable, FeederID)).Name;
        //    //string circuitNameFieldName = (ModelNameManager.FieldFromModelName((IObjectClass)_circuitSourceTable, FeederName)).Name;

        //    IQueryFilter queryFilter = new QueryFilterClass();
        //    //queryFilter.SubFields = string.Format("{0}, {1}, {2}", subStaionIDFiledName, circuitIDFieldName, circuitNameFieldName);
        //    //if (cboSubStation.SelectedIndex > 0)
        //    //{
        //    //    queryFilter.WhereClause = string.Format("SUBSTATIONID = '{0}'", cboSubStation.SelectedItem.ToString());
        //    //}
        //    IQueryFilterDefinition queryFilterDef = (IQueryFilterDefinition)queryFilter;
        //    queryFilterDef.PostfixClause = string.Format("ORDER BY {0}, {1}", subStaionIDFiledName, circuitIDFieldName);

        //    ICursor circuitSourceCursor = null;
        //    try
        //    {
        //        circuitSourceCursor = _circuitSourceTable.Search(queryFilter, true);

        //        int subStationIDIndex = FieldIndexFromModelName((IObjectClass)_circuitSourceTable, SubstationID);
        //        int circuitIDIndex = FieldIndexFromModelName((IObjectClass)_circuitSourceTable, FeederID);
        //        int circuitNameIndex = FieldIndexFromModelName((IObjectClass)_circuitSourceTable, FeederName);

        //        IRow row;
        //        while ((row = circuitSourceCursor.NextRow()) != null)
        //        {
        //            object circuitID = Convert.ToString(row.get_Value(circuitIDIndex));

        //            feederExt = obj as IMMFeederExt;
        //            feederSpace = feederExt.get_FeederSpace(geomNetwork, false);

        //            if (feederSpace != null)
        //            {
        //                //Get the feeder source
        //                feederSource = feederSpace.FeederSources.get_FeederSourceByFeederID(circuitID);
        //                if (feederSource != null)
        //                {
        //                    _fileWriter.WriteLine(Convert.ToString(row.get_Value(subStationIDIndex)), Convert.ToString(row.get_Value(circuitIDIndex)), row.get_Value(circuitNameIndex));
        //                    //ListViewItem item = new ListViewItem();
        //                    //item.SubItems[0].Text = Convert.ToString(row.get_Value(subStationIDIndex));
        //                    //item.SubItems.Add(Convert.ToString(row.get_Value(circuitIDIndex)));
        //                    //item.SubItems.Add(Convert.ToString(row.get_Value(circuitNameIndex)));

        //                    //lstCircuitList.Items.Add(item);
        //                }

        //            }
        //        }

        //        //lstCircuitList.Refresh();
        //    }
        //    finally
        //    {

        //        if (circuitSourceCursor != null) { while (Marshal.ReleaseComObject(circuitSourceCursor) > 0);}
        //        if (queryFilter != null) { while (Marshal.ReleaseComObject(queryFilter) > 0);}
        //    }
        //}

        ///// <summary>
        ///// This method will return the collection of netwotrks from the specified workspace
        ///// </summary>
        ///// <param name="networkName"></param>
        ///// <returns></returns>
        //public static Dictionary<string, IGeometricNetwork> GetNetworks(IWorkspace ws, out List<string> networkList, string weightName = "")
        //{
        //    Dictionary<string, IGeometricNetwork> geomNetworks = new Dictionary<string, IGeometricNetwork>();
        //    networkList = new List<string>();
        //    IGeometricNetwork geomNetwork = null;
        //    IEnumDataset enumDataset = ws.get_Datasets(esriDatasetType.esriDTFeatureDataset);
        //    enumDataset.Reset();

        //    IDataset dsName = enumDataset.Next();
        //    while (dsName != null)
        //    {
        //        IFeatureDataset featureDataset = dsName as IFeatureDataset;
        //        if (featureDataset != null)
        //        {
        //            IEnumDataset geomDatasets = featureDataset.Subsets;
        //            INetworkCollection networkCollection = featureDataset as INetworkCollection;
        //            for (int index = 0; index <= networkCollection.GeometricNetworkCount - 1; index++)
        //            {
        //                geomNetwork = networkCollection.get_GeometricNetwork(index);

        //                if (!string.IsNullOrEmpty(weightName))
        //                {
        //                    INetSchema networkSchema = geomNetwork.Network as INetSchema;
        //                    INetWeight networkWeight = networkSchema.get_WeightByName(weightName);
        //                    if (networkWeight != null)
        //                    {
        //                        geomNetworks.Add((geomNetwork as IDataset).BrowseName, geomNetwork);
        //                        networkList.Add((geomNetwork as IDataset).BrowseName);
        //                    }
        //                }
        //                else
        //                {
        //                    geomNetworks.Add((geomNetwork as IDataset).BrowseName, geomNetwork);
        //                    networkList.Add((geomNetwork as IDataset).BrowseName);
        //                }
        //            }                   
        //        }

        //        if (geomNetwork != null) { break; }
        //        dsName = enumDataset.Next();
        //    }

        //    if (geomNetworks.Count > 0)
        //    {
        //        return geomNetworks;
        //    }
        //    else
        //    {
        //        throw new Exception("Could not find the geometric networks in the specified database");
        //    }
        //}

        ///// <summary>
        ///// Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        ///// </summary>
        ///// <param name="directConnectString"></param>
        ///// <returns></returns>
        //public static IWorkspace OpenWorkspace(string directConnectString, string user, string pass)
        //{
        //    //Insert this line before invoking any ArcObjects to bind Engine runtime. 
        //    ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine);
        //    if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        //    {
        //        Console.WriteLine("Unable to determine user name and password.  Please run Telvent.PGE.LoginEncryptor.exe");
        //        return null;
        //    }

        //    // Create and populate the property set to connect to the database.
        //    ESRI.ArcGIS.esriSystem.IPropertySet propertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
        //    propertySet.SetProperty("SERVER", "");
        //    propertySet.SetProperty("INSTANCE", directConnectString);
        //    propertySet.SetProperty("DATABASE", "");
        //    propertySet.SetProperty("USER", user);
        //    propertySet.SetProperty("PASSWORD", pass);
        //    propertySet.SetProperty("VERSION", "sde.DEFAULT");

        //    SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();
        //    return wsFactory.Open(propertySet, 0);
        //}

        ///// <summary>
        ///// Static instance of ModelName Manager
        ///// </summary>
        //private static IMMModelNameManager _mnManager = Miner.Geodatabase.ModelNameManager.Instance;

        ///// <summary>
        ///// Instance of the ModelNameManager
        ///// </summary>
        //public static IMMModelNameManager ModelNameManager
        //{
        //    get { return _mnManager; }
        //    set { _mnManager = value; }
        //}

        ///// <summary>
        ///// Given a Workspace and ModelName gets the Objectclass with the given modelname from workspace
        ///// </summary>
        ///// <param name="ws"></param>
        ///// <param name="modelName"></param>
        ///// <returns></returns>
        //public static IObjectClass ObjectClassByModelName(IWorkspace ws, string modelName)
        //{
        //    try
        //    {
        //        IMMEnumObjectClass pEnumOC = ModelNameManager.ObjectClassesFromModelNameWS(ws, modelName);
        //        pEnumOC.Reset();
        //        return pEnumOC.Next();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //}

        ///// <summary>
        ///// Given a ObjectClass and a modelname gets the first field index that has the modelname assigned.
        ///// </summary>
        ///// <param name="objectClass">An object of type IObjectClass</param>
        ///// <param name="modelName">Modelname to check</param>
        ///// <returns>Returns the first field index that has the given modelname assigned</returns>
        //public static int FieldIndexFromModelName(IObjectClass objectClass, string modelName)
        //{
        //    int iRetVal = -1;
        //    try
        //    {
        //        IField field = ModelNameManager.FieldFromModelName(objectClass, modelName);
        //        if (field != null)
        //            return objectClass.Fields.FindField(field.Name);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message, ex);
        //    }
        //    return iRetVal;
        //}

        //string connectionString = ConfigurationManager.AppSettings["connectionString"];
        //string usr = ConfigurationManager.AppSettings["usr"];
        //string pwd = ConfigurationManager.AppSettings["pwd"];
        //_workspace = OpenWorkspace(connectionString, usr, pwd);
        ////Initialize map directory root folders.
        //_mapTypeDirectories = Directory.GetDirectories(ConfigurationManager.AppSettings["PDFBaseDirectory"], "*", SearchOption.TopDirectoryOnly);

        ////Initialize MapType here - it won't need to be re-initialized so this makes things faster.
        ////Otherwise we would merge this logic with the InitializeDropDown(FolderDependentComboBox) method.
        //for (int i = 0; i < _mapTypeDirectories.Length; i++)
        //{
        //    string folderName = _mapTypeDirectories[i].Substring(_mapTypeDirectories[i].LastIndexOf("\\") + 1, _mapTypeDirectories[i].Length - _mapTypeDirectories[i].LastIndexOf("\\") - 1);
        //    string[] info = Regex.Split(folderName, "-");

        //    //if (!cboMapType.Items.Contains(info[0]))
        //    //{
        //    //    cboMapType.Items.Add(info[0]);
        //    //}
        //    if (info.Length >= 2)
        //    {
        //        //Second value is scale
        //        KeyValuePair<string, string> scale = new KeyValuePair<string, string>(info[1], "1:" + info[1]);
        //    }
        //    List<string> toAdd = new List<string>();

        //    string ext = ".PDF";
        //    string prefix = info[0] + "_";
        //    foreach (string fileName in Directory.GetFiles(_mapTypeDirectories[i], "*.PDF", SearchOption.AllDirectories))
        //    {
        //        string file = fileName.Substring(fileName.LastIndexOf("\\") + 1, fileName.Length - fileName.LastIndexOf("\\") - 1);
        //        if (file.Length > ext.Length + prefix.Length)
        //        {
        //            string filteredFileName = file.Substring(prefix.Length);
        //            toAdd.Add(filteredFileName.Substring(0, filteredFileName.IndexOf('_')));
        //        }
        //    }
        //}
        //List<string> networkNameList;
        //_geomNetworks = GetNetworks(_workspace, out networkNameList, "MMElectricTraceWeight");
        //PopulateCircuitList(_geomNetworks[networkNameList[0]]);



        //private static string[] _mapTypeDirectories;
        //private static IWorkspace _workspace = null;
        //private static Dictionary<string, IGeometricNetwork> _geomNetworks;
        //private static StreamWriter _fileWriter = System.IO.File.AppendText("Result.csv");
        //private const string CircuitSource = "CIRCUITSOURCE";
        //public const string SubstationID = "SUBSTATIONID";
        //public const string FeederID = "FEEDERID";
        //public const string FeederName = "FEEDERNAME";
    }
}
