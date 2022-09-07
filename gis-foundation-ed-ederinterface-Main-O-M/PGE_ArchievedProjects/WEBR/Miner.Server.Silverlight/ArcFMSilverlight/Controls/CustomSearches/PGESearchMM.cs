using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;

using ESRI.ArcGIS.Client.Tasks;
using Microsoft.Practices.Prism;
using Miner.Server.Client;
using Miner.Server.Client.Query;
using Miner.Server.Client.Tasks;

using NLog;
using System.Collections.ObjectModel;
using ArcFMSilverlight;
using ESRI.ArcGIS.Client;


namespace ArcFM.Silverlight.PGE.CustomTools
{
    /// <summary>
    /// This class needs to be removed. The database column for SAPEQUIPID is having issues with SupportStructure
    /// </summary>
    public class PGESearchMM:SearchItem
    {
        #region member variables

        private int responseCount;
        private int layerSearchCount = 0;
        private Locator _locatorTask;
        private List<Miner.Server.Client.Tasks.IResultSet> _lastResult;
        public static Logger logger = LogManager.GetCurrentClassLogger();        
        private Query _query;
        ObservableCollection<IResultSet> observables = new ObservableCollection<IResultSet>();
        #endregion member variables

        #region public properties

        public string Fields { get; set; }
        public string Service { get; set; }       
        public string _searchTitle { get; set; }
        public ESRI.ArcGIS.Client.Geometry.SpatialReference LocalMapSpatialRef {get; set;}
        

        #endregion public properties

        #region Constructor

        public PGESearchMM(ILocateTask locate)
            : base(locate)
        {        
            Service = "";
            Fields = "";
        }

        #endregion

        #region public overrides
        public override void LocateAsync(string query)
        {
            CancelAsync();
            responseCount = 0;
            layerSearchCount = 0;
            observables.Clear(); 
            //Clear any existing Results
            this.Results.Clear();
            
            //Set Original Title
            _searchTitle = this.Title;
            int supportStructureLayerId = GetSupportStructureLayerId(this.SearchLayers[0].Url);
            //Get Each Configured TableSearch and Build a Query
            foreach (SearchLayer sl in this.SearchLayers)
            {
                // if support structure then...
                if (sl.ID == supportStructureLayerId)
                {
                    ExecuteQueryAsync(query, sl);
                }
                else
                {
                    ExecuteLocateAsync(query, sl);
                }
            }

        }

        private int GetSupportStructureLayerId(string url)
        {
            int layerId = -1;

//            string mapServer = url.Substring(0, url.LastIndexOf("/"));

            layerId = ConfigUtility.GetLayerIDFromLayerName(ConfigUtility.CurrentMap, url, "Support Structure");

            return layerId;
        }

        private void ExecuteQueryAsync(string query, SearchLayer sl)
        {
            const string wildCard = "%";//changed from "*" -- * will work against FileGDB and PersonalGDB                
            var q = new Query();
            if (query.Contains("\"") == true)
            {
                q = new Query
                {
                    Where =
                        string.Join(" OR ",
                                    from field in sl.Fields
                                    select field + " = '" + (string)query.ToUpper() + "'"),
                    OutFields = new OutFields { "*" }
                };
            }
            else
            {
                q = new Query
                {
                    Where =
                        string.Join(" OR ",
                                    from field in sl.Fields
                                    select field + " like '%" + query.ToUpper() + wildCard + "'"),
                    OutFields = new OutFields { "*" }
                };
            }
            q.Where = q.Where.Replace("\"", "");
            q.ReturnGeometry = true;
            _query = q;

            //Now query the first layerID returned to zoom to the feature
            QueryTask _queryTask = new QueryTask(sl.Url + "/" + sl.ID);
            _queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_queryTask_ExecuteCompleted);
            _queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_queryTask_Failed);

            _queryTask.ExecuteAsync(_query);
            
        }

        private void ExecuteLocateAsync(string query, SearchLayer sl)
        {
            string url = sl.Url;
            LocateTask locateTask = new LocateTask(url);
            LocateParameters locateParameters = new LocateParameters();

            locateParameters.UserQuery = query.Replace("\"", "");
            locateParameters.ExactMatch = true;
            locateParameters.ReturnAttributes = true;
            locateParameters.MaxRecords = 500;
            locateParameters.SearchLayer = sl;


            locateTask.ExecuteCompleted += new EventHandler<TaskResultEventArgs>(locateTask_ExecuteCompleted);
            locateTask.Failed += new EventHandler<Miner.Server.Client.Tasks.TaskFailedEventArgs>(locateTask_Failed);

            locateTask.ExecuteAsync(locateParameters);
        }


        void locateTask_Failed(object sender, Miner.Server.Client.Tasks.TaskFailedEventArgs e)
        {
//            throw new NotImplementedException();
        }

        void locateTask_ExecuteCompleted(object sender, TaskResultEventArgs e)
        {
            layerSearchCount++;
            observables.AddRange(e.Results);

            if (layerSearchCount == this.SearchLayers.Count)
            {
                OnLocateComplete(new ResultEventArgs(observables));
            }
        }

        void _queryTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _queryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            layerSearchCount++;

            
            QueryTask queryTask = sender as QueryTask;
            string url = queryTask.Url;
            string mapServer = url.Substring(0, url.LastIndexOf("/"));
            int layerID = -1;
            string layerIDString = url.Substring(url.LastIndexOf("/") + 1);
            Int32.TryParse(layerIDString, out layerID);
            string layerAlias = null;
            if ((queryTask == null) || (e.FeatureSet == null))
            {
                OnLocateComplete(new ResultEventArgs(Results));
                return;
            }
            

            foreach (Layer layer in ConfigUtility.CurrentMap.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer)
                {
                    ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                    if (mapServer == dynamicLayer.Url)
                    {
                        foreach (LayerInfo layerInfo in dynamicLayer.Layers)
                        {
                            if (layerInfo.ID == layerID)
                            {
                                layerAlias = layerInfo.Name;
                                break;
                            }
                        }
                    }
                }
            }

            ResultSet results = new ResultSet(e.FeatureSet);
            results.Service = mapServer;
            results.ID = layerID;
            results.Name = layerAlias;
            observables.Add(results);

            if(layerSearchCount == this.SearchLayers.Count)
                OnLocateComplete(new ResultEventArgs(observables)); 
        }
        #endregion

    }
}
