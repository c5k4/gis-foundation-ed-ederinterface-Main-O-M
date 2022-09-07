#region Organized and sorted using

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
    /// PGE_AgreementNumber responsible for the searching the MLX Number/Customer Agreement number based on user provided input from locator tool.
    /// </summary>
    [Guid("9D44DBBF-9E49-4136-9708-621707B2A128")]
    [ProgId("PGE.Desktop.EDER.PMOrderNumberLocator")]
    public class PGE_AgreementNumber : IMMSearchStrategy
    {
        #region Private Fields
        /// <summary>
        /// Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Store the result of the search.
        /// </summary>
        private IMMRowSearchResults _results = null;

        /// <summary>
        /// Used for stopping the search in between of processing .
        /// </summary>
        private IMMSearchControl _searchControl = null;

        /// <summary>
        /// Hold the current reference.
        /// </summary>
        private IMap _map = null;

        /// <summary>
        /// Hold the user input agreement number 
        /// </summary>
        private string _userAgreementNumber = string.Empty;

        /// <summary>
        /// Hold the user input objectid 
        /// </summary>
        private int _userObjectId = -1; 

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
        private static HashSet<ITable> _standaloneTableList = null;

        #endregion

        #region IMMSearchStrategy Members

        public IMMSearchResults Find(IMMSearchConfiguration searchConfig, IMMSearchControl searchControl)
        {
            _searchControl = searchControl;
            ExtractSettings(searchConfig);
            ExecuteFind(_userAgreementNumber, _userObjectId); 
            return _results as IMMSearchResults;
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
                _userAgreementNumber = (string)properties.GetProperty("Agreement Number");

                int OId = -1;
                if (properties.GetProperty("ObjectId").ToString() != string.Empty) 
                    int.TryParse((string)properties.GetProperty("ObjectId"), out OId);
                _userObjectId = OId;
                _thresholdValue = (int)properties.GetProperty("LocateThresholdValue");
                if (_userAgreementNumber.Length > 11)
                {
                    MessageBox.Show("MLX number length must be 1 to 11 characters.", "Info", MessageBoxButtons.OK);
                    return;
                }

                if ((_userObjectId == -1) && (_userAgreementNumber.Length == 0))
                {
                    MessageBox.Show("Either an Agreement Number or an ObjectID must be entered", "Info", MessageBoxButtons.OK);
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
        private void ExecuteFind(string AgreementNumber, int ObjectId)
        {
            ICursor cursor = null;
            try
            {
                //Enforce the user to enter some criteria 
                if ((AgreementNumber.IsNullOrWhitespace()) && ObjectId.ToString().IsNullOrWhitespace())
                {
                    return;
                }

                InitializeLayerList(_map);
                if (_standaloneTableList.Count == 0) { return; }
                Func<IObjectClass, string> layerOperatingNumberIsNull =
                objectClass =>
                    ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.AgreementNumber).Name + " IS NULL";

                Func<IObjectClass, string, string> layerOperatingNumberEquals =
                    (objectClass, value) =>
                        string.Format("UPPER({0}) = UPPER('{1}')", ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.AgreementNumber).Name, value);


                if (_results == null)
                {
                    _results = new Miner.Framework.Search.RowSearchResults();
                }

                var uniqueResults = new HashSet<Comparer>();
                foreach (var layer in _standaloneTableList)
                {
                    if (layer == null)
                    {
                        continue;
                    }
                    // check whether the layer name has the JobNumber field model name
                    if (!ModelNameFacade.ContainsFieldModelName(layer as IObjectClass, SchemaInfo.Electric.FieldModelNames.AgreementNumber))
                        continue;

                    //IFeatureLayer layer2 = layer as IFeatureLayer;
                    var objectClass = (IObjectClass)layer;

                    //Just have to set the objectId here 
                    IQueryFilter queryFilter = new QueryFilterClass();
                    string whereClause = "";

                    if (AgreementNumber != string.Empty)
                    {
                        whereClause = layerOperatingNumberEquals(objectClass, AgreementNumber);
                    }

                    if (ObjectId != -1)
                    {
                        if (whereClause == "")
                            whereClause = ((IObjectClass)objectClass).OIDFieldName + " = " + 
                                ObjectId.ToString(); 
                        else
                            whereClause += " AND " + ((IObjectClass)objectClass).OIDFieldName +
                                " = " + ObjectId.ToString(); 
                    }
                    queryFilter.WhereClause = whereClause; 
                    

                    if (_thresholdValue > 0)
                    {
                        queryFilter.WhereClause += " And ROWNUM <= " + _thresholdValue;
                    }

                    _logger.Debug("WhereClause statement: " + queryFilter.WhereClause);
                    cursor = (ICursor)layer.Search(queryFilter, false);
                    IRow currentRow;
                    while ((currentRow = cursor.NextRow()) != null)
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
                        _results.AddRow(currentRow);
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
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                    cursor = null;
                }
            }
        }

        /// <summary>
        /// Initialized the feature layer for processing by using current map.
        /// </summary>
        /// <param name="map">Currently selected map.</param>
        private static void InitializeLayerList(IMap map)
        {
            try
            {
                if (_standaloneTableList == null)
                {
                    _standaloneTableList = new HashSet<ITable>();
                }
                _standaloneTableList.Clear();
                IStandaloneTableCollection pStandAloneTableCollection = map as IStandaloneTableCollection;
                IStandaloneTable pStandalonetable = null;
                for (int i = 0; i <= pStandAloneTableCollection.StandaloneTableCount - 1; i++)
                {
                    pStandalonetable = pStandAloneTableCollection.StandaloneTable[i];
                    IDisplayTable displayTable = pStandalonetable as IDisplayTable;
                    _standaloneTableList.Add(displayTable.DisplayTable as ITable);
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
