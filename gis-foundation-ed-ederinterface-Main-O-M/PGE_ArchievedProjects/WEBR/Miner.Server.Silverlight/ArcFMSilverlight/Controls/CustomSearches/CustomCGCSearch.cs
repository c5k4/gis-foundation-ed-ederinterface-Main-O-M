using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using ESRI.ArcGIS.Client.Tasks;

using Miner.Server.Client;
using Miner.Server.Client.Query;
using MinerTask = Miner.Server.Client.Tasks;
using NLog;
using Miner.Server.Client.Tasks;
using System.Collections.ObjectModel;
using ArcFMSilverlight;
using ESRI.ArcGIS.Client;

namespace ArcFM.Silverlight.PGE.CustomTools
{
    public class CustomCGCSearch : SearchItem   
    {
        #region member variables

       
        private Locator _locatorTask;
        private List<Miner.Server.Client.Tasks.IResultSet> _lastResult;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private string  matchAddress = "";
        private Query _query;
        ObservableCollection<IResultSet> observables = new ObservableCollection<IResultSet>();         
        int layerSearchCount;
        #endregion member variables

        #region public properties

        //public int ClassID { get; set; }
        public string ClassID { get; set; } //ME Q4 2019 - DA# 190906

        public List<int> LayerIDs { get; set; }
        public string Fields { get; set; }
        public string Service { get; set; }       
        public string _searchTitle { get; set; }
        public ESRI.ArcGIS.Client.Geometry.SpatialReference LocalMapSpatialRef {get; set;}
        

        #endregion public properties

        #region Constructor

        public CustomCGCSearch(MinerTask.ILocateTask locate)
            : base(locate)
        {        
            Service = "";
            Fields = "";
        }

        #endregion

        #region public overrides

        /// <summary>
        /// LocateAsync(string query): Configures and Submits Address Locate using the user input single line address
        /// </summary>
        /// <param name="query">Input Where Clause</param>
        public override void LocateAsync(string query)
        {

            layerSearchCount = 0;
            observables.Clear();
            Results.Clear(); 
            if (query == null)
            {
                logger.Warn("Address query value is invalid. Did not execute Address Locate Operation.");
                return;
            }
            
            if (query.Length < 1)
            {
                logger.Warn("Address query string has a length of Zero. Enter Address and Search again. Did not execute Address Locate Operation.");
                return;
            }

            query = query.Replace("\"", "");


            foreach (Layer layer in ConfigUtility.CurrentMap.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                {
                    ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;
                    string selectURL = dynamicMapLayer.Url;

                    //Adjust for schematics classID
                    //int classID = Int32.Parse(item[0]);

                    //List<int> currentSelectLayerIDs = ConfigUtility.GetLayerIDFromClassID(selectURL, ClassID).ToList();
                    //ME Q4 2019 - DA# 190906
                    List<int> currentSelectLayerIDList = new List<int>(); ;
                    List<int> classIdList = ClassID.Split(',').Select(int.Parse).ToList();
                    foreach (int selectedClassID in classIdList) {
                        List<int> currentSelectLayerID = ConfigUtility.GetLayerIDFromClassID(selectURL, selectedClassID).ToList();
                        if (currentSelectLayerID.Count > 0)
                            currentSelectLayerIDList.AddRange(currentSelectLayerID);
                    }

                    if (currentSelectLayerIDList.Count < 1) { continue; }
                    this.LayerIDs = currentSelectLayerIDList;
                    foreach (int layerID in currentSelectLayerIDList)
                    {
                        
                        Query _query = new Query();
                        _query.ReturnGeometry = true;
                        _query.OutFields.Add("*");
                        if (ReturnQuery(query) != "")
                            _query.Where = ReturnQuery(query);
                        else
                        {
                            OnLocateComplete(new ResultEventArgs(Results));
                            return;
                        }

                        //Now query the first layerID returned to zoom to the feature
                        QueryTask _queryTask = new QueryTask(selectURL + "/" + layerID);
                        _queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_task_ExecuteCompleted);
                        _queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_task_Failed);

                        _queryTask.ExecuteAsync(_query);
                    }
                }
            }

        }

        private string ReturnQuery(string query)
        {
            int  _strLen = query.Trim().Length;
            string _query = "";
            switch(_strLen)
            {
                case 12:
                    {
                        _query = "CGC12 = '" + query + "'";
                        break;
                    }
                case 8:
                    {
                        _query = "CGC12 LIKE '%" + query.Trim().Substring(0, 4) + "%" + query.Trim().Substring(4, 4) + "'";
                        break;
                    }
                case 9:
                    {
                        string[] _arry = null ;
                        _arry = query.Trim().Split('-');
                        if (_arry.Length == 2)
                        {
                            _query = "CGC12 LIKE '%" + _arry[0] + "%" + _arry[1] + "'";
                        }
                        break;
                    }                
            }

            return _query;
        }

        void _task_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }
        
        void _task_ExecuteCompleted(object sender, QueryEventArgs e)
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

            if (e.FeatureSet.Features.Count  > 0)
            {
                ResultSet results = new ResultSet(e.FeatureSet);
                results.Service = mapServer;
                results.ID = layerID;
                results.Name = layerAlias;
                observables.Add(results);
            }
            if (this.LayerIDs.Count == layerSearchCount) 
                OnLocateComplete(new ResultEventArgs(observables));          
            
        }

        #endregion
    }
}
