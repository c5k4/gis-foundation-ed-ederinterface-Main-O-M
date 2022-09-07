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


namespace ArcFMSilverlight.Controls.ShowRolloverInfo
{
    public class ShowDetailsMenu
    {
         #region properties
        public string GlobalId;
        public string ServiceUrl;
        public int LayerId;
        ObservableCollection<IResultSet> observables = new ObservableCollection<IResultSet>();    
        
        /// <summary>
        /// Event that fires when data for Details (on Rollover) is fetched.
        /// </summary>
        public event EventHandler<ResultEventArgs> DetailsComplete;

        private Collection<Miner.Server.Client.Tasks.IResultSet> results = new Collection<Miner.Server.Client.Tasks.IResultSet>();
        #endregion properties

        
        protected virtual void OnDetailsComplete(ResultEventArgs e)
         {
             EventHandler<ResultEventArgs> handler = this.DetailsComplete;
             if (handler != null)
             {
                 handler(this, e);
             }
         }

        /// <summary>
        /// LocateAsync(string query): Configures and Submits Address Locate using the user input single line address
        /// </summary>
        public void LocateAsync()
        {
            observables.Clear();
            results.Clear();

            Query _query = new Query();
            _query.ReturnGeometry = true;
            _query.OutFields.Add("*");
            _query.Where = "GLOBALID = '" + GlobalId + "'";

            QueryTask _queryTask = new QueryTask(ServiceUrl + "/" + LayerId);
            _queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(query_task_ExecuteCompleted);
            _queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_task_Failed);

            _queryTask.ExecuteAsync(_query);
        }

        void _task_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Unable to show Details.");
        }


        void query_task_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    QueryTask queryTask = sender as QueryTask;
                    string url = queryTask.Url;
                    string mapServer = url.Substring(0, url.LastIndexOf("/"));
                    int layerID = -1;
                    string layerIDString = url.Substring(url.LastIndexOf("/") + 1);
                    Int32.TryParse(layerIDString, out layerID);
                    string layerAlias = null;
                    if ((queryTask == null) || (e.FeatureSet == null))
                    {
                        OnDetailsComplete(new ResultEventArgs(results));
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
                    observables.Clear();
                    if (e.FeatureSet.Features.Count > 0)
                    {
                        ResultSet results = new ResultSet()
                       {
                           DisplayFieldName = e.FeatureSet.DisplayFieldName,
                           ID = layerID,
                           Name = layerAlias,
                           Service = mapServer
                       };

                        if (e.FeatureSet.FieldAliases != null)
                        {
                            foreach (var field in e.FeatureSet.FieldAliases)
                            {
                                results.FieldAliases.Add(field.Key, field.Value);
                            }
                        }
                        int index = 1;
                        foreach (var feature in e.FeatureSet.Features)
                        {
                            GraphicPlus graphic = new GraphicPlus(feature) { FieldAliases = results.FieldAliases, DisplayFieldName = results.DisplayFieldName };
                            graphic.Attributes["RowIndex"] = index++;
                            results.Features.Add(graphic);
                        }
                        results.SpatialReference = e.FeatureSet.SpatialReference;
                        observables.Add(results);

                    }
                    OnDetailsComplete(new ResultEventArgs(observables));
                }
                else
                {
                    OnDetailsComplete(new ResultEventArgs(results));
                    return;
                }
            }
            catch (Exception ex)
            {
                ConfigUtility.UpdateStatusBarText("Unable to show Details: " + ex.Message);
            }
           
        }
    }
}
