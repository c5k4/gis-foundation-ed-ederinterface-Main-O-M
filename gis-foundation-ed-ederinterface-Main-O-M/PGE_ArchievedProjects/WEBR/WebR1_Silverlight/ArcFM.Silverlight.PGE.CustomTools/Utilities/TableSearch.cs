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

namespace ArcFM.Silverlight.PGE.CustomTools
{
    public class TableSearch : SearchItem
    {
        #region member variables

        private QueryTask _task;
        private List<TableSearch> _tableSearchCollection;
        private int responseCount;
        private int tableSearchCount = 0;

        public int TableIndex { get; set; }
        public string Fields { get; set; }
        public string Service { get; set; }
        public string layerType { get; set; }
        public string _searchTitle { get; set; }
        public Query tsquery;
        public bool isExactMatch { get; set; }

        public static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion member variables

        #region Constructor

        public TableSearch(MinerTask.ILocateTask locate):base(locate)
        {}

        #endregion
        
        #region public methods

        /// <summary>
        /// TableSearchCollction : List of Table Search Items to process
        /// </summary>
        public List<TableSearch> TableSearchCollction
        {
            get { return _tableSearchCollection; }
            set { _tableSearchCollection = value; }
        }
        #endregion public methods

        #region public overrides

        /// <summary>
        /// CancelAsync() : Cancels the Query Task
        /// </summary>        
        //public override void CancelAsync()
        //{
        //    if (_task != null)
        //    {
        //        _task.CancelAsync();
        //    }
        //}

        /// <summary>
        /// LocateAsync(string query): Configures and Submits the Query Task using the input query statement
        /// </summary>
        /// <param name="query">Input Where Clause</param>
        public override void LocateAsync(string query)
        {
            //Initialize
            const string wildCard = "%";//changed from "*" -- * will work against FileGDB and PersonalGDB                
            CancelAsync();
            responseCount = 0;
            tableSearchCount = 0;

            //Clear any existing Results
            this.Results.Clear();

            //Set Original Title
            _searchTitle = this.Title;

            //Get Each Configured TableSearch and Build a Query
            foreach (TableSearch ts in _tableSearchCollection)
            {
                var q = new Query();
                //if (query.Contains("\"") == true)
                if (isExactMatch)
                {
                    //query = this.getExactMatchQueryString(query);

                    q = new Query
                    {
                        Where =
                            string.Join(" OR ",
                                        from field in ts.Fields.Split(',')
                                        select "UPPER(" + field + ") = '" + (string)query.ToUpper() + "'"),
                        OutFields = new OutFields { "*" }
                    };
                }
                else
                {
                    q = new Query
                    {
                        Where =
                            string.Join(" OR ",
                                        from field in ts.Fields.Split(',')
                                        select "UPPER(" + field + ") like '" + query.ToUpper() + wildCard + "'"),
                        OutFields = new OutFields { "*" }
                    };
                }



                //If the TableSearch is a FeatureLayer then Return Geometries
                if (ts.layerType == "FeatureLayer") q.ReturnGeometry = true;

                //Build and Submit the QueryTask
                q.Where = q.Where.Replace("\"", "");
                ts.tsquery = q;
                WebClient wsc = new WebClient();
                wsc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(wsc_DownloadStringCompleted);
                wsc.DownloadStringAsync(new Uri(ts.Service + "/" + ts.TableIndex + "?f=json"), ts);

            }

        }
        #endregion public overrides

        #region private methods

        /// <summary>
        /// FireTasks(): Fires the Query Tasks in Sequence -- As they are oredered in the Search Item Collection
        /// </summary>
        private void FireTasks()
        {
            foreach (TableSearch ts in _tableSearchCollection)
            {
                _task = new QueryTask(ts.Service + "/" + ts.TableIndex);
                _task.ExecuteCompleted += TaskExecuteCompleted;
                _task.Failed += TaskFailed;
                ts.tsquery.Where = ts.tsquery.Where.ToString();
                _task.ExecuteAsync(ts.tsquery);
            }
        }

        /// <summary>
        /// GetNewFindParameters(TableSearch inTs): Build Find Parameters from the TableSearch
        /// </summary>
        /// <param name="inTs">Input Table Search</param>
        /// <returns>ESRI Task Find Opeartion Parameters</returns>
        private FindParameters GetNewFindParameters(TableSearch inTs)
        {

            FindParameters _fp = new FindParameters();
            string[] s = inTs.Fields.Split(','); //Get the Fields to Search

            //Add the Fields listed in the search config to the Find Parameters SearchFields list
            foreach (string s1 in s)
            {
                _fp.SearchFields.Add(s1);
            }

            //Add the Layer ID to Search
            _fp.LayerIds.Add(inTs.TableIndex);


            return _fp;

        }

        /// <summary>
        /// getFieldList(string inFieldList): Convert the Fields entered into the search config to a List
        /// </summary>
        /// <param name="inFieldList">comma delimitted string of Field names</param>
        /// <returns>List<string> : List of Field names to search</returns>
        private List<string> getFieldList(string inFieldList)
        {
            List<string> _outlist = new List<string>();
            string[] s = inFieldList.Split(',');

            foreach (string s1 in s)
            {
                _outlist.Add(s1);
            }

            return _outlist;

        }

        /// <summary>
        /// getExactMatchQueryString(string inString): Defines the Format needed for an Exact Match query
        /// </summary>
        /// <param name="inString">Input Query String</param>
        /// <returns>String : Query String without a wildcard</returns>
        private string getExactMatchQueryString(string inString)
        {
            const string quote = "\"";
            string outString = inString;

            //Check for the existence of quotes at start and end.
            bool exactMatch = inString.Trim().StartsWith(quote) && inString.Trim().EndsWith(quote);

            //If it is an Exact Match Query, remove the quotes
            if (exactMatch)
            {
                // Remove the last quote, then the first quote
                outString = inString.Replace(quote, "").Trim();
            }

            return outString;
        }

        /// <summary>
        /// getTableIndexFromQueryTaskUrl(string inUrl): Extract the Table index from the query URL
        /// </summary>
        /// <param name="inUrl">Map Service URL with Table Index</param>
        /// <returns>int : Layer/Table Index</returns>
        private int getTableIndexFromQueryTaskUrl(string inUrl)
        {
            int ilastindexof = 0;
            int remainderlength = 0;
            int responseTableIndex = 0;

            try
            {
                ilastindexof = inUrl.LastIndexOf("/") + 1;
                remainderlength = inUrl.Length - ilastindexof;
                responseTableIndex = System.Convert.ToInt32(inUrl.Substring(ilastindexof, remainderlength));
            }
            catch
            {
                responseTableIndex = TableSearchCollction[responseCount - 1].TableIndex;
            }


            return responseTableIndex;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inUrl"></param>
        /// <returns></returns>
        private string getMapServiceUrlFromQueryTaskUrl(string inUrl)
        {
            int ilastindexof = 0;
            int remainderlength = 0;
            string serviceUrl = string.Empty;

            try
            {
                ilastindexof = inUrl.LastIndexOf("/") + 1;
                remainderlength = inUrl.Length - ilastindexof;
                serviceUrl = inUrl.Substring(0, (ilastindexof - 1));
            }
            catch
            {
                serviceUrl = TableSearchCollction[responseCount - 1].Service;
            }


            return serviceUrl;

        }

        /// <summary>
        /// GetLayerTitle(string inQueryTaskUrl): Extract the Layer Name from the Query Task Url
        /// </summary>
        /// <param name="inQueryTaskUrl">Query Task Url</param>
        /// <returns>string : Layer Name/Title</returns>
        private string GetLayerTitle(string inQueryTaskUrl)
        {
            string tsUrl = string.Empty;
            string layerTitle = string.Empty;

            foreach (TableSearch _ts in TableSearchCollction)
            {

                tsUrl = _ts.Service + "/" + _ts.TableIndex.ToString();
                if (tsUrl == inQueryTaskUrl)
                {
                    layerTitle = _ts.Title;
                    break;
                }
            }

            return layerTitle;
        }
        private bool isNumber(string value)
        {
            int number1;
            return int.TryParse(value, out number1);
        }
        #endregion private methods

        #region Events

        /// <summary>
        /// wsc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e): Completed Event Handler for Web Service Request
        /// </summary>
        /// <param name="sender">Originating Object</param>
        /// <param name="e">Download String Completed Event Args</param>
        void wsc_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            tableSearchCount++;
            string json = e.Result;
            TableSearch ts = e.UserState as TableSearch;

            if ((e.Result == null) || (e.UserState == null) || (ts == null)) return;

            var jsonObject = JsonValue.Parse(json) as JsonObject;
            if (jsonObject != null)
                if (jsonObject.ContainsKey("name"))
                    ts.Title = jsonObject["name"];

            if (tableSearchCount == _tableSearchCollection.Count)
                FireTasks();
        }


        /// <summary>
        /// TaskFailed(object sender, TaskFailedEventArgs e): Query Task Failed Event Handler
        /// </summary>
        /// <param name="sender">Originating Object</param>
        /// <param name="e">Task Failed Event Args</param>
        void TaskFailed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Table Search task failed!" + e.Error.Message);

            //increment the response count
            responseCount++;

            //If responses have completed, kick off the completed event
            if (responseCount == _tableSearchCollection.Count)
                OnLocateComplete(new ResultEventArgs(Results));
        }

        /// <summary>
        /// TaskExecuteCompleted(object sender, QueryEventArgs e): Query Task Completed Event Handler
        /// </summary>
        /// <param name="sender">Originating Object</param>
        /// <param name="e">Query Event Args</param>
        void TaskExecuteCompleted(object sender, QueryEventArgs e)
        {

            var queryTask = sender as QueryTask;


            if ((queryTask == null) || (e.FeatureSet == null))
            {
                OnLocateComplete(new ResultEventArgs(Results));
                return;
            }

            //increment the response count
            responseCount++;

            int featureCount = e.FeatureSet.Features.Count;
            if (featureCount > this.MaxRecords && this.MaxRecords > 0)
            {
                for (int i = featureCount - 1; i >= MaxRecords; i--)
                {
                    e.FeatureSet.Features.RemoveAt(i);
                }
            }

            //Create the New ResultSet
            var set = new Miner.Server.Client.Tasks.ResultSet(e.FeatureSet)
            {
                ID = getTableIndexFromQueryTaskUrl(queryTask.Url),
                Service = getMapServiceUrlFromQueryTaskUrl(queryTask.Url),
                Name = GetLayerTitle(queryTask.Url),
                ExceededThreshold = (featureCount > this.MaxRecords) ? true : false,
            };

            //Check for features before adding to the Locate Results Collection
            if (e.FeatureSet.Features.Count > 0)
            {
                Results.Add(set);
            }

            //If responses have completed, kick off the completed event
            if (responseCount == _tableSearchCollection.Count)
            {
                this.Title = _searchTitle;
                OnLocateComplete(new ResultEventArgs(Results));
            }

        }
        #endregion Events
    }
}
