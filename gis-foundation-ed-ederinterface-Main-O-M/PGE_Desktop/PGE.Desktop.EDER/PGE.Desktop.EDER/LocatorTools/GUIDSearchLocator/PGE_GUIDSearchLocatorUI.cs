#region Organized and sorted using

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.ComCategories;
using Miner.Framework.Search;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using Miner;

#endregion

namespace PGE.Desktop.EDER.LocatorTools.GUIDSearchLocator
{
    /// <summary>
    /// Class for UI and searching the GUID Search based on user provided input "Global ID" from ArcFM locator tool
    /// </summary>
    [Guid("E741CBD1-6667-4342-A736-ACB1C09DD8A8")]
    [ProgId("PGE.Desktop.EDER.LocatorTools.GUIDSearchLocator")]
    [ComponentCategory(ComCategory.MMLocatorSearchStrategyUI)]
    public partial class PGE_GUIDSearchLocatorUI : UserControl, IMMSearchStrategyUI, IMMResultsProcessor
    {
        public PGE_GUIDSearchLocatorUI()
        {
            InitializeComponent();
        }

        #region Private Fields
        /// <summary>
        /// Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Hold the caption which appear the locator search drop down box.
        /// </summary>
        private const string _caption = "GUID Search";

        /// <summary>
        /// Set the priority of the caption. We can change the priority when we want our caption should be shown up.
        /// </summary>
        private int _priority = 2; // As XY Coordinates has 500 reserved.

        /// <summary>
        /// Default map count.
        /// </summary>
        private int _mapLayerCount = -1;

        /// <summary>
        /// Current focus map.
        /// </summary>
        private IMap _map = null;

        /// <summary>
        /// Current active view.
        /// </summary>
        private IActiveView _activeView = null;

        /// <summary>
        /// An store feature class that has been registered with the geodatabase.
        /// </summary>
        private IObjectClass _classFilter = null;

        /// <summary>
        /// Store the registry settings.
        /// </summary>
        private IMMRegistry _registry = null;
        #endregion

        #region Private Properties

        /// <summary>
        /// get the mmRegistey
        /// </summary>
        private IMMRegistry Registry
        {
            get { return _registry ?? (_registry = new MMRegistry()); }
        }

        #endregion Private Properties

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
        ///  Receives the search results as an <see cref="IMMSearchResults"/> object
        /// </summary>
        /// <param name="searchResults">search Results</param>
        /// <param name="optionFlags"></param>
        /// <param name="searchControl"></param>
        /// <returns></returns>
        public ID8List AddResults(IMMSearchResults searchResults, mmSearchOptionFlags optionFlags, IMMSearchControl searchControl)
        {
            ID8List topLevel = new D8ListClass();
            var rows = (IMMRowSearchResults)searchResults;
            rows.Reset();
            IRow row;
            while ((row = rows.NextRow()) != null)
            {
                D8FeatureClass d8fe = new D8FeatureClass
                {
                    Name = (row as IFeature).Class.AliasName,
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
        ///  indicating whether the results tree should be expanded automatically.
        /// </summary>
        public bool AutoExpandTree
        {
            get { return false; }
        }

        /// <summary>
        /// indicating whether to automatically select the first node in the results tree.
        /// </summary>
        public bool AutoSelectFirstNode
        {
            get { return false; }
        }

        /// <summary>
        /// Clears results from the previous search.  
        /// </summary>
        public void ClearResults()
        {

        }

        /// <summary>
        /// Deletes the IObject passed in.  
        /// </summary>
        /// <param name="item"></param>
        public void Delete(ESRI.ArcGIS.Geodatabase.IObject item)
        {

        }

        /// <summary>
        /// Updates the IObject passed in.  
        /// </summary>
        /// <param name="item"></param>
        public void Update(ESRI.ArcGIS.Geodatabase.IObject item)
        {

        }
        #endregion

        #region IMMSearchStrategyUI Members
        public string COMProgID
        {
            get { return null; }
        }

        /// <summary>
        /// Name of the search strategy as it appears in the locator tool pulldown menu 
        /// </summary>
        public string Caption
        {
            get { return _caption; }
        }

        /// <summary>
        /// Notifies a current open locator that the user has selected another locator and provides the opportunity to perform any necessary state changes.
        /// </summary>
        public void Deactivated()
        {

        }

        /// <summary>
        /// Contains the propertyset specific to the type of search being conducted.  
        /// </summary>
        /// <param name="optionFlags"></param>
        /// <returns></returns>
        public IMMSearchConfiguration GetSearchConfiguration(mmSearchOptionFlags optionFlags)
        {
            IPropertySet properties = new PropertySetClass();
            properties.SetProperty("Map", _map);
            properties.SetProperty("GUID Search", txtGUIDSearch.Text);
            properties.SetProperty("LayerCountChanged", _mapLayerCount != _map.LayerCount);
            _mapLayerCount = _map.LayerCount;
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
        /// Takes a map and an object class to initialize the search strategy.
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="classFilter"></param>
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
        /// set the order in which the locators appear in the dropdown menu.  
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
        }

        /// <summary>
        /// Method clears the user interface in preparation for the next search. 
        /// </summary>
        public void Reset()
        {
            txtGUIDSearch.Text = string.Empty;
        }

        /// <summary>
        /// Determine how the objects in the tree appear/
        /// </summary>
        public IMMResultsProcessor ResultsProcessor
        {
            get { return null; }
        }

        /// <summary>
        ///  Takes an IMMSearchStrategy object. This object contains the Find method that executes the search.
        /// </summary>
        public IMMSearchStrategy SearchStrategy
        {
            get { return new GUIDSearchLocator(); }
        }

        /// <summary>
        /// Release the com objetcs.
        /// </summary>
        public void Shutdown()
        {
            if (_classFilter != null)
            {
                Marshal.FinalReleaseComObject(_classFilter);
                _classFilter = null;
            }
            if (_map != null)
            {
                Marshal.FinalReleaseComObject(_map);
                _map = null;
            }
        }
        #endregion

        #region Private Methods

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
    }
}
