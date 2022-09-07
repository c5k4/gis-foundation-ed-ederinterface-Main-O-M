﻿#region Organized and sorted using

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

#endregion


namespace PGE.Desktop.EDER.LocatorTools.MlxRelationshipTool
{
    /// <summary>
    /// PMOrderNumberLocator responsible for the searching the PM Order Number based on user provided input from locator tool.
    /// </summary>
    [Guid("49547C08-4300-4257-9203-6D00B8FE628A")]
    [ProgId("PGE.Desktop.EDER.PMOrderNumberLocator")]
    public class PMOrderNumberLocator : IMMSearchStrategy
    {
        #region Private Fields
        /// <summary>
        /// Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Store the result of the search.
        /// </summary>
        //private IMMRowSearchResults _results = null;
        private IMMRowLayerSearchResults _results = null;
        /// <summary>
        /// Used for stopping the search in between of processing .
        /// </summary>
        private IMMSearchControl _searchControl = null;

        /// <summary>
        /// Hold the current reference.
        /// </summary>
        private IMap _map = null;

        /// <summary>
        /// Hold the user input value.
        /// </summary>
        private string _userInput = string.Empty;

        /// <summary>
        /// Hold the result of layout changes.
        /// </summary>
        private bool _layerCountChanged = false;

        /// <summary>
        /// Hold the registry value of the threshold.
        /// </summary>
        private int _thresholdValue = 0;

        /// <summary>
        /// Hold the feature class list reference for searching.
        /// </summary>
        private static HashSet<IFeatureLayer> _featLayerList = null;

        #endregion

        #region Constructors

        public PMOrderNumberLocator()
        {
        }

        #endregion

        #region #region IMMSearchStrategy Members

        /// <summary>
        /// Execute the search based on user provided input and return Result Interface.
        /// </summary>
        /// <param name="searchConfig">Contain <see cref="IMMSearchConfiguration"/> information about user input, map etc. </param>
        /// <param name="searchControl">Used for stop search in between the progress.</param>
        /// <returns>Result of search<see cref="IMMSearchResults"/>.</returns>
        public IMMSearchResults Find(IMMSearchConfiguration searchConfig, IMMSearchControl searchControl)
        {
            try
            {
                _searchControl = searchControl;
                //get the settings set by the user(PM Order Number from the text box)
                ExtractSettings(searchConfig);
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
        /// Extract search related settings like. map, user input etc.
        /// </summary>
        /// <param name="config">User search related configuration</param>
        /// <exception cref="InvalidOperationException">If IMMSearchConfiguration is null or some thing goes wrong while casting in IPropertySet. Throws the exception.</exception>
        private void ExtractSettings(IMMSearchConfiguration config)
        {
            try
            {
                if (config == null)
                {
                    throw new InvalidOperationException("Null IMMSearchConfiguration object. Find can't done");
                }

                IPropertySet properties = config.SearchParameters as IPropertySet;
                if (properties == null) throw new InvalidOperationException("No PropertySet in IMMSearchConfiguration; find cannot proceed.");
                _map = (IMap)properties.GetProperty("Map");
                _layerCountChanged = (bool)properties.GetProperty("LayerCountChanged");
                _userInput = (string)properties.GetProperty("PM Order Number");
                _thresholdValue = (int)properties.GetProperty("LocateThresholdValue");
                if (_userInput.Length > 11 || _userInput.Length == 0)
                {
                    MessageBox.Show("PM Order Number length must be 1 to 11 characters.", "Info", MessageBoxButtons.OK);
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
        }

        /// <summary>
        /// Perform the search operation 
        /// </summary>
        /// <param name="PMOrderNumber">PM Order Number provided by user.</param>
        private void ExecuteFind(string PMOrderNumber)
        {
            IFeatureCursor cursor = null;
            try
            {
                if (PMOrderNumber.IsNullOrWhitespace())
                {
                    return;
                }

                if (_featLayerList == null || _layerCountChanged)
                {
                    InitializeLayerList(_map);
                }

                if (_featLayerList.Count == 0) { return; }

                Func<IObjectClass, string> layerOperatingNumberIsNull =
                objectClass =>
                    ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.JobNumber).Name + " IS NULL";

                Func<IObjectClass, string, string> layerOperatingNumberEquals =
                    (objectClass, value) =>
                        string.Format("UPPER({0}) = UPPER('{1}')", ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.JobNumber).Name, value);

                if (_results == null)
                {
                    //_results = new Miner.Framework.Search.RowSearchResults();
                    _results = new Miner.Framework.Search.RowLayerSearchResults();
                }

                var uniqueResults = new HashSet<Comparer>();
                foreach (var layer in _featLayerList)
                {
                    //ME Q4 19 Release DA#190403
                    #region WIP search by PM OrderNumber DA#190403
                    string layerName = layer.Name;
                    if (layerName.Equals("Wip"))
                    {
                        _logger.Debug("Search started for Layer : " + layerName);
                        var objectClassWip = (IObjectClass)layer.FeatureClass;
                        IQueryFilter queryFilterWip = new QueryFilterClass
                        {
                            WhereClause = "UPPER(INSTALLJOBNUMBER) = UPPER("+PMOrderNumber+")"
                        };
                        if (_thresholdValue > 0)
                        {
                            queryFilterWip.WhereClause += " And ROWNUM <= " + _thresholdValue;
                        }
                        _logger.Debug("WhereClause statement: " + queryFilterWip.WhereClause);
                        //April 2019 release - Fix for Green halo ME item# 39 - to avoid issue on PGEReplaceAsset as Switch and DPD layers are configured into two (Opened and Closed) sublayers
                        cursor = (IFeatureCursor)layer.Search(queryFilterWip, false);
                        IFeature currentRowWip;
                        while ((currentRowWip = cursor.NextFeature()) != null)
                        {
                            if (Stopped)
                            {
                                return;
                            }
                            var comparerWip = new Comparer(currentRowWip);
                            if (uniqueResults.Contains(comparerWip))
                            {
                                continue;
                            }
                            uniqueResults.Add(comparerWip);
                            _results.AddRow(currentRowWip, layer);
                        }
                    }
                    #endregion
                    // check whether the layer name has the JobNumber field model name
                    if (!ModelNameFacade.ContainsFieldModelName(layer.FeatureClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.JobNumber))
                        continue;

                    var objectClass = (IObjectClass)layer.FeatureClass;

                    IQueryFilter queryFilter = new QueryFilterClass
                    {
                        WhereClause = string.IsNullOrEmpty(PMOrderNumber) ? layerOperatingNumberIsNull(objectClass) : layerOperatingNumberEquals(objectClass, PMOrderNumber)
                    };

                    if (_thresholdValue > 0)
                    {
                        queryFilter.WhereClause += " And ROWNUM <= " + _thresholdValue;
                    }
                    _logger.Debug("WhereClause statement: " + queryFilter.WhereClause);
                    //April 2019 release - Fix for Green halo ME item# 39 - to avoid issue on PGEReplaceAsset as Switch and DPD layers are configured into two (Opened and Closed) sublayers
                    cursor = (IFeatureCursor)layer.Search(queryFilter, false);
                    IFeature currentRow;
                    while ((currentRow = cursor.NextFeature()) != null)
                    {
                        if (Stopped)
                        {
                            return;
                        }

                        var comparer = new Comparer(currentRow);
                        if (uniqueResults.Contains(comparer))
                        {
                            continue;
                        }
                        uniqueResults.Add(comparer);
                        _results.AddRow(currentRow, layer);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
            finally
            {
                if (cursor != null)
                {
                    //Release COM object
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                    cursor = null;
                }
            }
        }

        /// <summary>
        /// Initialized the feature layer for processing by using current map.
        /// </summary>
        /// <param name="map">Currently selected map.</param>
        private void InitializeLayerList(IMap map)
        {
            try
            {
                if (_featLayerList == null)
                {
                    _featLayerList = new HashSet<IFeatureLayer>();
                }
                _featLayerList.Clear();
                IEnumLayer enumerator = map.Layers[CartoFacade.UIDFacade.FeatureLayers, true];
                ILayer layer;

                while ((layer = enumerator.Next()) != null)
                {
                    if (layer is IFeatureLayer && layer.Valid)
                    {
                        _featLayerList.Add((IFeatureLayer)layer);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
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
