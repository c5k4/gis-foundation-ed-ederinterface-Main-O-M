using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.Delivery.Diagnostics;
using Miner.Interop;
using Miner.ComCategories;
using Miner.Framework.Search;
using System.Xml.Serialization;
using System.IO;
using Miner;


namespace PGE.Desktop.EDER.LocatorTools
{
    [Guid("2981EC83-B0AF-4E57-BB58-51D42E401400")]
    [ProgId("PGE.Desktop.EDER.PGE_OperatingNumberLocatorUI")]
    [ComponentCategory(ComCategory.MMLocatorSearchStrategyUI)]
    public partial class PGE_OperatingNumberLocatorUI : UserControl, IMMSearchStrategyUI, IMMResultsProcessor
    {
        #region Private Fields
        private const string _caption = "PGE Operating Number Search";
        

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private IMap _map;
        private IActiveView _activeView;
        private IObjectClass _classFilter;
        private int _priority = 0;
        private int _mapLayerCount = -1;
        private Dictionary<string, int> _domainMapping;

        #endregion

        #region Private Properties
        /// <summary>
        /// get the mmRegistey
        /// </summary>
        private IMMRegistry Registry
        {
            get { return _registry ?? (_registry = new MMRegistry()); }
        }
        private IMMRegistry _registry;

        private static ConfigurationFile Configuration
        {
            get { return _configuration ?? (_configuration = ConfigurationFile.LoadOrCreate()); }
        }
        private static ConfigurationFile _configuration;
        #endregion Private Properties

        #region Constructors
        /// <summary>
        /// Initialize the component
        /// </summary>
        public PGE_OperatingNumberLocatorUI()
        {
            try
            {
                InitializeComponent();
                
                TryGetLastDivision();
            }
            catch (Exception ex)
            {
                var message = GetType().Name + " failed to initialize: " + ex.ToString();
                MessageBox.Show(message);
                _logger.Error(message);
            }
        }

        #endregion

        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            MMLocatorSearchStrategyUI.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            MMLocatorSearchStrategyUI.Unregister(regKey);
        }

        #endregion

        #region IMMResultsProcessor Members
        /// <summary>
        /// Process the results and add to ID8List
        /// </summary>
        /// <param name="searchResults">results in a row</param>
        /// <param name="optionFlags">user selected Locator option flags</param>
        /// <param name="searchControl"> provides the ability to stop processing</param>
        /// <returns>return results in ID8List</returns>
        public ID8List AddResults(IMMSearchResults searchResults, mmSearchOptionFlags optionFlags, IMMSearchControl searchControl)
        {        
            ID8List topLevel = new D8ListClass();      
            //cast mmsearchresults to mmrowsearchresults
            var rows = (IMMRowSearchResults)searchResults;
            rows.Reset();
            //get row
            IRow row;
            while ((row = rows.NextRow()) != null)
            {
                D8FeatureClass d8fe = new D8FeatureClass
                {
                    Name = (row as IFeature).Class.AliasName, // row.Value[nextRow.Fields.FindField("STNAME")].ToString(),
                    AssociatedGeoRow = row
                };
                _logger.Debug("item name: " + d8fe.Name);
                //set related row
                ID8Layer d8Layer = null;
                if (topLevel.HasChildren)
                {
                    //check whether the list item already present
                    d8Layer = FindChild(topLevel, (row as IFeature).Class.AliasName);

                    bool hasChild = d8Layer != null;
                    _logger.Debug("listItem present: " + hasChild);

                    if (!hasChild)// if not present
                    {
                        d8Layer = new D8LayerClass { LayerName = (row as IFeature).Class.AliasName };
                        _logger.Debug("listIem name: " + d8Layer.LayerName);
                    }
                }
                else// if no listItem present
                {
                    d8Layer = new D8LayerClass { LayerName = (row as IFeature).Class.AliasName };
                    _logger.Debug("listIem name: " + d8Layer.LayerName);
                }
                // add list item to item
                topLevel.Add(d8Layer as ID8ListItem);
                // add  item to list item
                ((ID8List)d8Layer).Add(d8fe);
            }
            return topLevel as ID8List;
        }

        /// <summary>
        /// need autoexpand of tree results
        /// </summary>
        public bool AutoExpandTree
        {
             get { return false; }
            
        }

        /// <summary>
        /// need auto select the first node element
        /// </summary>
        public bool AutoSelectFirstNode
        {
            
             get { return false; }
        }


        public void ClearResults()
        {

        }

        public void Delete(ESRI.ArcGIS.Geodatabase.IObject item)
        {

        }

        public void Update(ESRI.ArcGIS.Geodatabase.IObject item)
        {

        }
        #endregion

        #region IMMSearchStrategyUI Members
        /// <summary>
        /// get ComProgID
        /// </summary>
        public string COMProgID
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// get Caption in drop down list
        /// </summary>
        public string Caption
        {
            get
            {
                return _caption;
            }
        }

        public void Deactivated()
        {
            
        }

        /// <summary>
        /// set user inputs
        /// </summary>
        /// <param name="optionFlags">user selected Locator option flags</param>
        /// <returns>return IMMSearchConfiguration</returns>
        public IMMSearchConfiguration GetSearchConfiguration(mmSearchOptionFlags optionFlags)
        {
            IPropertySet properties = new PropertySetClass();
            // set ESRI map
            properties.SetProperty("Map", _map);

            int divisionCode;
            var divisionCodeText = cbDivision.Text ?? string.Empty;
            if (_domainMapping.TryGetValue(cbDivision.Text, out divisionCode))
            {
                //StoreDivisionName(cbDivision.Text);
            }
            else
            {
                divisionCode = -1;
            }

            properties.SetProperty("Division Code", divisionCode);
            // set operating number from user
            properties.SetProperty("Operating Number", txtOperatingNumber.Text);
            _logger.Debug("Operating Number: " + txtOperatingNumber.Text);

            properties.SetProperty("LayerCountChanged", _mapLayerCount != _map.LayerCount);
            _mapLayerCount = _map.LayerCount;


            properties.SetProperty("DebugDisplay", (ModifierKeys & Keys.Control) == Keys.Control);

            _logger.Debug("Layer Count: " + _mapLayerCount.ToString());

            // open the registry key for locate threshold
            Registry.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "LocatorToolSettings");

            int thresholdValue = (int)Registry.Read("EnforceLocateThreshold", 0) == 0
                                ? 0
                                : (int)Registry.Read("LocateThreshold", 0);
            properties.SetProperty("LocateThresholdValue", thresholdValue);

            IMMSearchConfiguration config = new SearchConfiguration();
            if (null == config) return null;

            config.SearchParameters = properties;

            return config;
        }

       

        /// <summary>
        /// Initialize search startegy
        /// </summary>
        /// <param name="Map">esri map</param>
        /// <param name="classFilter">esri object class</param>
        public void InitializeStrategyUI(ESRI.ArcGIS.Carto.IMap Map, ESRI.ArcGIS.Geodatabase.IObjectClass classFilter)
        {
            // esri map
            _map = Map; 
            // set active view
            _activeView = Map as IActiveView;
            //set object class
            _classFilter = classFilter;
        }

        /// <summary>
        /// get priority to show the caption (0 means top/first in list item);
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
        }

        /// <summary>
        /// reset/clear the user input in text box
        /// </summary>
        public void Reset()
        {
            
            txtOperatingNumber.Text = "";
        }
        /// <summary>
        ///  get result processer
        /// </summary>
        public IMMResultsProcessor ResultsProcessor
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// get search strategy
        /// </summary>
        public IMMSearchStrategy SearchStrategy
        {
            get
            {
                return new OperatingNumberLocator();
            }
        }

        public void Shutdown()
        {
            
        }
        #endregion

        #region Private Methods
        /********* Unused registry locations retained for reference purposes ***********
        const string _divisionRegistryValue = @"LastOpNumberDivision";
        const string _fullDivisionKeyName
           = @"HKCU\Software\Miner and Miner\PGE\Locators\""" + _divisionRegistryValue + @"""=[division]";
        const string _shortDivisionKeyName = @"PGE\ArcFM\Locators";
         *******************************************************************************/

        void StoreDivisionName(string divisionName)
        {
            try
            {
                Configuration.LastDivision = divisionName;
                Configuration.Save();
            }
            catch(Exception ex)
            {
                const string errorMessage = "Unable to save division to configuration file\r\n";
                _logger.Error(errorMessage + ex.Message);
            }
        }

        private void TryGetLastDivision()
        {
            if (string.IsNullOrEmpty(Configuration.LastDivision))
                return;

            try
            {
                InitializeDivisions(this, new EventArgs());

                if (_domainMapping == null)
                    throw new Exception("Missing division domain.");

                int domainValue;
                if (_domainMapping.TryGetValue(Configuration.LastDivision, out domainValue))
                    cbDivision.SelectedIndex = cbDivision.Items.IndexOf(Configuration.LastDivision);
                else
                    throw new Exception("No such division as " + Configuration.LastDivision);
            }
            catch(Exception ex)
            {
                const string errorMessage = "Failed to initialize last division\r\n";
                _logger.Error(errorMessage + ex.Message);
            }
        }

        private void InitializeDivisions(object sender, EventArgs e)
        {
            if (_domainMapping != null) return;

            ConfigurationFile.LoadOrCreate();

            var type = Type.GetTypeFromProgID("esriFramework.AppRef");
            var app = (ESRI.ArcGIS.Framework.IApplication)Activator.CreateInstance(type);
            IWorkspaceDomains workspaceDomains = null;

            IMMLogin2 login = app.FindExtensionByName("MMPROPERTIESEXT") as IMMLogin2;
            if (login != null)
            {
                workspaceDomains = login.LoginObject.LoginWorkspace as IWorkspaceDomains;
            }

            if (workspaceDomains == null)
            {
                //Try to get it from a layer in the map
                var document = app.Document;
                var mxDoc = (IMapDocument)document;
                var currentMap = mxDoc.get_Map(0);
                if (currentMap == null) return;

                // must get a valid feature layer in order to access the workspace...
                var layers = currentMap.Layers;
                IWorkspace workspace = null;
                while (workspace == null)
                {
                    var layer = layers.Next();
                    if (layer == null) break;
                    IFeatureLayer featureLayer = layer as IFeatureLayer;
                    if (featureLayer != null)
                    {
                        var featureClass = featureLayer.FeatureClass;
                        if (featureClass != null)
                        {
                            var featureDataset = featureClass.FeatureDataset;
                            if (featureDataset != null)
                            {
                                workspace = featureDataset.Workspace;
                            }
                        }
                    }
                }

                workspaceDomains = workspace as IWorkspaceDomains;
            }

            if (workspaceDomains == null) return;
            
            _domainMapping = workspaceDomains.DomainByName["Division Name"].ToDictionary(-1);

            _domainMapping.Add(string.Empty, -1);
            cbDivision.Items.Clear();
            cbDivision.Items.Add("<All Divisions>");

            foreach (var description in _domainMapping.Keys)
            {
                if (!string.IsNullOrEmpty(description))
                {
                    cbDivision.Items.Add(description);
                }
            }
        }


        /// <summary>
        /// Check whether an item is already present in the list.
        /// </summary>
        /// <param name="topLevel">The list to search.</param>
        /// <param name="itemName">The name of the item to look for.</param>
        /// <returns>Returns the first matching item; otherwise null.</returns>
        private ID8Layer FindChild(ID8List topLevel, string itemName)
        {
            topLevel.Reset();

            ID8ListItem lstItem;
            while ((lstItem = topLevel.Next()) != null)
            {
                if (string.Compare(lstItem.DisplayName, itemName, true) != 0)
                    continue;
                _logger.Debug("listItem name: " + lstItem.DisplayName);

                return lstItem as ID8Layer;
            }
            return null;
        }

        #endregion

        #region Nested Types
        // Location: %appdata%\ESRI\Desktop10.0\ArcMap
        [XmlRoot("config")]
        public class ConfigurationFile
        {
            static readonly string FilePath
                = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                               @"ESRI\Desktop10.8\ArcMap\LocatorConfiguration.xml");
            static readonly XmlSerializer Serializer = new XmlSerializer(typeof(ConfigurationFile));

            bool _changed;

            public ConfigurationFile() { }
            public static ConfigurationFile LoadOrCreate()
            {
                if(!File.Exists(FilePath))
                    return new ConfigurationFile();
                using (var stream = File.Open(FilePath, FileMode.Open))
                {
                    var configuration = (ConfigurationFile)Serializer.Deserialize(stream);
                    configuration._changed = false;
                    return configuration;
                }
            }
            public void Save()
            {
                if (!_changed) return;
                using (var stream = File.Open(FilePath, FileMode.Create))
                    Serializer.Serialize(stream, this);
                _changed = false;
            }

            public bool IndexChanged
            {
                set 
                {
                    _changed = true;
                }
            }

            string _lastDivision;
            [XmlElement("last-division")]
            public string LastDivision
            {
                get { return _lastDivision; }
                set
                {
                    if(_lastDivision != value)
                        _changed = true;
                    _lastDivision = value;
                }
            }
        }
        #endregion Nested Types

        private void cbDivision_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cboDivision = (ComboBox)sender;

            Configuration.IndexChanged = true;
            Configuration.LastDivision = cboDivision.SelectedItem.ToString();
            Configuration.Save();
        }
    }
}
