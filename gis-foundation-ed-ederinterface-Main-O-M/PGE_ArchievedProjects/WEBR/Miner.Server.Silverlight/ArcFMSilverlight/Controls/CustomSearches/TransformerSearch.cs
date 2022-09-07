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
    public class TransformerSearch : SearchItem   
    {
        #region member variables

       
        private Locator _locatorTask;
        private List<Miner.Server.Client.Tasks.IResultSet> _lastResult;
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private string  matchAddress = "";
        private Query _query;
        ObservableCollection<IResultSet> observables = new ObservableCollection<IResultSet>();         
        int layerSearchCount;
        public SearchLayer SearchTransLayer;
        public SearchLayer SearchCustAddLayer;
        public string custAddFields;
        public string relationField;
        #endregion member variables

        #region public properties

        public List<int> LayerIDs { get; set; }
        public string Fields { get; set; }
        public string Service { get; set; }       
        public string _searchTitle { get; set; }
        public ESRI.ArcGIS.Client.Geometry.SpatialReference LocalMapSpatialRef {get; set;}
        

        #endregion public properties

        #region Constructor

        public TransformerSearch(MinerTask.ILocateTask locate)
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
            observables.Clear();
            Results.Clear(); 

            if (SearchCustAddLayer == null)
            {
                OnLocateComplete(new ResultEventArgs(Results));
            }
            else
            {
                //LocateParameters parameter = new LocateParameters();

                //parameter = GetParameters(SearchCustAddLayer, query);
                //ExecuteTaskAsync(parameter, SearchCustAddLayer);

               Query _query = new Query();
                _query.ReturnGeometry = false;
                _query.OutFields.Add("TRANSFORMERGUID");
                if (ReturnCustAddQuery(query) != "")
                    _query.Where = ReturnCustAddQuery(query);
                else{
                    OnLocateComplete(new ResultEventArgs(Results));
                    return;
                }

                QueryTask _queryTask = new QueryTask(SearchCustAddLayer.Url + "/" + SearchCustAddLayer.ID);
                _queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_cust_task_ExecuteCompleted);
                _queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_task_Failed);

                _queryTask.ExecuteAsync(_query);

            }
        }

        private LocateParameters GetParameters(SearchLayer layer, string query)
        {
            const string quote = "\"";

            LocateParameters parameter = new LocateParameters();
            bool exactMatch = query.StartsWith(quote) && query.EndsWith(quote);
            if (exactMatch)
            {
                // Remove the last quote, then the first quote
                query = query.Remove(query.Length - 1).Remove(0, 1);
            }
            parameter.SearchTitle = Title;
           // query = "%" + query + "%";
            parameter.ExactMatch = exactMatch;
            parameter.SearchLayer = layer;
            parameter.ReturnAttributes = true;
            parameter.MaxRecords = MaxRecords;
            parameter.UserQuery = RemoveSpecialChars(query);
            parameter.UserQuery = query;
            parameter.SpatialReference = SpatialReference;

            // parameter.SearchTitle = "Transformer";
            return parameter;
        }

         public string RemoveSpecialChars(string str)
            {
                // Create  a string array and add the special characters you want to remove
   
                string[] chars = new string[] { " ",",", ".", "/", "!", "@","#", "$","^", "&", "*", "'", "\"", ";","_", "(", ")", ":", "|", "[", "]" }; 
                //Iterate the number of times based on the String array length.
                for(int i=0; i<chars.Length;i++)
             {
                 if(str.Contains(chars[i]))
                    {
                 str=str.Replace(chars[i],"%").ToUpper();
                 }
            }

            return str;
            }

        private string ReturnCustAddQuery(string query)
        {
            query = RemoveSpecialChars(query);
            string[] fields = custAddFields.Split(',');
            string whereClause = "";
            for (int i = 0; i < fields.Length; i++){
                if(i == 0){
                     whereClause += "(" + fields[i] + ")LIKE ('" + query + "')";
                }
                else{
                    whereClause += " OR " + "(" + fields[i] + ")LIKE ('" + query + "')";
                }
            }
            return whereClause;
        }

        void _task_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        void _cust_task_ExecuteCompleted(object sender, QueryEventArgs e)
        {  
            
           // layerSearchCount++;
            string transGlobalIds = "";
            if (e.FeatureSet.Features.Count > 0)
            {
                for (int i = 0; i < e.FeatureSet.Features.Count; i++)
                {
                    if (i == 0)
                        transGlobalIds = "'{" + Convert.ToString(e.FeatureSet.Features[i].Attributes["TRANSFORMERGUID"]).ToUpper() + "}'";
                    else
                        transGlobalIds = transGlobalIds+",'{" + Convert.ToString(e.FeatureSet.Features[i].Attributes["TRANSFORMERGUID"]).ToUpper() + "}'";
                    
                }
                if (transGlobalIds.Length > 0)
                {

                    Query _query = new Query();
                    _query.ReturnGeometry = true;
                    _query.OutFields.Add("*");
                    _query.Where = "UPPER(GLOBALID) IN (" + transGlobalIds + ")";
                    _query.Where = _query.Where.Replace("(,", "(").Replace(",)", ")");

                    QueryTask _queryTask = new QueryTask(SearchTransLayer.Url + "/" + SearchTransLayer.ID);
                    _queryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_trans_task_ExecuteCompleted);
                    _queryTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_task_Failed);

                    _queryTask.ExecuteAsync(_query);

                }
            }
           
            else
            {
                OnLocateComplete(new ResultEventArgs(Results));
                return;
            }    
            
        }


        void _trans_task_ExecuteCompleted(object sender, QueryEventArgs e)
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

                if (e.FeatureSet.Features.Count > 0)
                {

                    observables.Clear();
                    ResultSet results = new ResultSet(e.FeatureSet);
                    results.Service = mapServer;
                    results.ID = layerID;
                    results.Name = layerAlias;
                    observables.Add(results);
                }
                OnLocateComplete(new ResultEventArgs(observables));
            }
            else
            {
                OnLocateComplete(new ResultEventArgs(Results));
                return;
            }
           
        }
        #endregion
    }
}
