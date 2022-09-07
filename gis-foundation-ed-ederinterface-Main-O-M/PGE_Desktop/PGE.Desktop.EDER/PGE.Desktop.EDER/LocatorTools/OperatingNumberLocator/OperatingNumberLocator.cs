using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Carto;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.LocatorTools
{
    [Guid("8A7B8716-D5AA-4064-8D60-D78F85F37475")]
    [ProgId("PGE.Desktop.EDER.OperatingNumberLocator")]

    public class OperatingNumberLocator : IMMSearchStrategy
    {
        #region Private Fields
        private const string _featureLayerClassID = "7158830B-DAE3-461F-A900-58F201BF873E";

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static HashSet<IFeatureLayer> _featLayerList;

        //private IMMRowSearchResults _results;
        private IMMRowLayerSearchResults _results;
        private IMMSearchControl _searchControl;
        private IMap _map; 
        private string _userInput = string.Empty;
        private bool _layerCountChanged;
        private int _thresholdValue;
        private int _divisionCode;
        private bool _debugDisplay;
        #endregion

        #region Constructors

        public OperatingNumberLocator()
		{
		}

        #endregion

        #region #region IMMSearchStrategy Members
        /// <summary>
        /// find the search results
        /// </summary>
        /// <param name="searchConfig">get userConfigurations</param>
        /// <param name="searchControl">provides the ability to stop processing</param>
        /// <returns>return IMMsearchResults</returns>
        public IMMSearchResults Find(IMMSearchConfiguration searchConfig, IMMSearchControl searchControl)
        {
            
            try
            {
                _searchControl = searchControl;
                //get the settings set by the user(operating number from the text box)
                ExtractSettings(searchConfig);
                //Search for the results
                _logger.Debug("user input: " + _userInput);
                _userInput = _userInput.Replace("'", "''");
                ExecuteFind(_userInput);
                return _results as IMMSearchResults;
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
                return null;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Extract the setting set by the user
        /// </summary>
        /// <param name="config">Configuration set by the user</param>
        private void ExtractSettings(IMMSearchConfiguration config)
        {
            if (null == config)throw new InvalidOperationException("Null IMMSearchConfiguration; find cannot proceed.");

            IPropertySet properties = config.SearchParameters as IPropertySet;

            if (null == properties) throw new InvalidOperationException("No PropertySet in IMMSearchConfiguration; find cannot proceed.");
            // get Imap
            _map = (IMap)properties.GetProperty("Map");
            _divisionCode = (int)properties.GetProperty("Division Code");
            //get user input for operating number
            _userInput = (string)properties.GetProperty("Operating Number");
            //check whether layer added
            _layerCountChanged = (bool)properties.GetProperty("LayerCountChanged");
            //get threshold Value 
            _thresholdValue = (int)properties.GetProperty("LocateThresholdValue");

            _debugDisplay = (bool)properties.GetProperty("DebugDisplay");
        }

        /// <summary>
        /// Check if the process stopped.
        /// </summary>
        private bool Stopped
        {
            get
            {
                return _searchControl != null ? _searchControl.Stopped : false;
            }
        }

        /// <summary>
        /// Execute the search process for getting the  results  
        /// </summary>
        /// <param name="operatingNumber"></param>
        private void ExecuteFind(string operatingNumber)
        {
            if (operatingNumber.IsNullOrWhitespace())
                return;
            // Check for whether feature layer list count less than 1 or any layer added(true: if added to current instance and false: if not added)

            if (_featLayerList == null || _layerCountChanged)
            {
                InitializeLayerList(_map);
            }

            if (_featLayerList.Count == 0)
                return;

            Func<IObjectClass, string> layerOperatingNumberIsNull =
                objectClass =>
                    ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorOperatingNumber).Name + " IS NULL";

            Func<IObjectClass, string, string> layerOperatingNumberEquals =
                (objectClass, value) =>
                    string.Format("UPPER({0}) = UPPER('{1}')", ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorOperatingNumber).Name, value);

            Func<IObjectClass, int, string> layerDivisionNumberEquals =
                (objectClass, value) => ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorDivision) == null? null :
                    string.Format("{0} = {1}", ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorDivision).Name, value);

            if (_results == null)
            {
                _results = new Miner.Framework.Search.RowLayerSearchResults();
            }

            List<string> debugList = null;
            var uniqueResults = new HashSet<Comparer>();
            //for (int i = 0; i < _featLayerList.Count(); i++)// loop through each layers
            foreach(var layer in _featLayerList)
            {
                // check whether the layer name has the OperatingNumber field model name
                if (!ModelNameFacade.ContainsFieldModelName(layer.FeatureClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.LocatorOperatingNumber))
                    continue;
                
                _logger.Debug(SchemaInfo.Electric.FieldModelNames.LocatorOperatingNumber + " found");

                //query using IQueryFilter
                var objectClass = (IObjectClass)layer.FeatureClass;

                IQueryFilter queryFilter = new QueryFilterClass
                    {
                        WhereClause =
                            string.IsNullOrEmpty(operatingNumber)
                            ? layerOperatingNumberIsNull(objectClass)
                            : layerOperatingNumberEquals(objectClass, operatingNumber)
                    };

                if (_thresholdValue > 0)
                {
                    queryFilter.WhereClause += " And ROWNUM <= " + _thresholdValue;
                }

                if (_divisionCode != -1)
                {
                    if (layerDivisionNumberEquals(objectClass, _divisionCode) != null)
                        queryFilter.WhereClause += " And " + layerDivisionNumberEquals(objectClass, _divisionCode);
                    else {
                        _logger.Error("Layer : " + objectClass.AliasName + " does not have Division field or the field model name " + SchemaInfo.Electric.FieldModelNames.LocatorDivision + " is not assigned to Division field");
                        continue;
                    }
                        //logger
                }
                _logger.Debug("WhereClause statement: " + queryFilter.WhereClause);
                
                var cursor = (IFeatureCursor)((IFeatureLayer2)layer).Search(queryFilter, false);
                try
                {
                    IFeature currentRow;
                    while ((currentRow = cursor.NextFeature()) != null)
                    {
                        if (Stopped)
                            return;

                        var comparer = new Comparer(currentRow);
                        if (_debugDisplay)
                        {
                            if (debugList == null)
                                debugList = new List<string>();
                            debugList.Add(comparer.ToString());
                        }
                        if (uniqueResults.Contains(comparer))
                        {
                            if (_debugDisplay)
                            {
                                var item = debugList[debugList.Count - 1];
                                debugList[debugList.Count - 1] = "duplicate " + item;
                            }
                            continue;
                        }
                        uniqueResults.Add(comparer);
                        _results.AddRow(currentRow, layer);
                    }

                }
                finally
                {
                    //Release COM object
                    Marshal.FinalReleaseComObject(cursor);
                    cursor = null;
                }
            }

            if (_debugDisplay)
            {
                var form = new Form();
                form.SuspendLayout();
                form.Text = "Debug Locator Results";
                var textBox = new TextBox
                {
                    ReadOnly = true,
                    Multiline = true,
                    Dock = DockStyle.Fill,
                    Text = debugList.Concatenate("\n")
                };
                form.Controls.Add(textBox);
                form.ResumeLayout(false);
                form.Show();
            }
        }

        //public static mmSearchOptionFlags optionFlags(IMMSearchStrategyUI strategyUI)
        //{
        //    strategyUI.
        //}
        /// <summary>
        /// get all layers in enumerator
        /// </summary>
        /// <param name="map">Esri Map</param>
        /// <returns></returns>
        public static IEnumLayer GetFeatureLayers(IMap map)
        {
            var filterUID = new ESRI.ArcGIS.esriSystem.UIDClass();
           //set UID
            filterUID.Value = _featureLayerClassID;
            //return layer in enum
            return map.Layers[filterUID];
        }

        /// <summary>
        /// get layer list for current map
        /// </summary>
        /// <param name="map">esri map document</param>
        /// <returns>return all layer in list</returns>
        public static void InitializeLayerList(IMap map)
        {
            if(_featLayerList == null)
                _featLayerList = new HashSet<IFeatureLayer>();
            _featLayerList.Clear();

            //get all layer in enumerator
            IEnumLayer enumerator = map.Layers[CartoFacade.UIDFacade.FeatureLayers];
            ILayer layer;

            while ((layer = enumerator.Next()) != null)// if layer not null
            {
                //check if layer is feature layer and layer is valid
                if (layer is IFeatureLayer && layer.Valid)
                {
                    //add layer to list 
                    _featLayerList.Add((IFeatureLayer)layer);
                }
            }
        }

        
        #endregion

        #region Nested Types
        class Comparer : IEquatable<Comparer>
        {
            int _objectId;
            int _classId;
            public Comparer(IRow row)
            {
                _objectId = row.OID;
                _classId = ((IObject)row).Class.ObjectClassID;
            }

            public bool Equals(Comparer other)
            {
                return _objectId == other._objectId && _classId == other._classId;
            }

            public override bool Equals(object obj)
            {
                var other = obj as Comparer;
                if (obj is IRow)
                    other = new Comparer((IRow)obj);
                if (other == null)
                    return false;
                return Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked { return _classId * 397 ^ _objectId; }
            }

            public override string ToString()
            {
                return string.Format("OID: {0}, CLSID: {1}", _objectId, _classId);
            }
        }
        #endregion
    }
}
