using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client.Tasks;

using Miner.Server.Client;
using Miner.Server.Client.Query;
using Miner.Server.Client.Tasks;

using NLog;

namespace ArcFM.Silverlight.PGE.CustomTools
{
    public class ServiceSearch: SearchItem
    {
        #region Member Variables

        private Dictionary<string, FeatureSet> _collection = new Dictionary<string, FeatureSet>();
        private Dictionary<string, int> _featSetToID = new Dictionary<string, int>();

        public string Service { get; set;}
        public string Fields { get; set; }
        public string Layers { get; set; }

        public static Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructor

        public ServiceSearch(ILocateTask locate):base(locate)
        {}

        #endregion

        #region Public overrides

        public override void LocateAsync(string query)
        {
            //CancelAsync();
            //Clear if any previous result is present
            Results.Clear();
            //remove the double quotes from the query string
            query = query.Substring(1, query.Length - 2);

            ExecuteTasks(query);
        }

        private void ExecuteTasks(string findText)
        {
            FindTask findTask = new FindTask(Service);
            findTask.ExecuteCompleted += new EventHandler<FindEventArgs>(findTask_ExecuteCompleted);
            findTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(findTask_Failed);
            FindParameters findparams = GetFindParameters(findText);
            findTask.ExecuteAsync(findparams);
        }

        private FindParameters GetFindParameters(string findText)
        {
            FindParameters findParams = new FindParameters();
            findParams.SearchText = findText;
            findParams.Contains = true;
            findParams.ReturnGeometry = true;
            findParams.SpatialReference = this.SpatialReference;
            string[] fields = Fields.Split(',');
            foreach (string field in fields)
            {
                findParams.SearchFields.Add(field);
            }

            foreach (string layer in Layers.Split(','))
            {
                int layerID;
                if (int.TryParse(layer, out layerID))
                    findParams.LayerIds.Add(layerID);
            }
            return findParams;
        }

        void findTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            logger.Error("Service Search task failed! " + e.Error.Message);
        }

        void findTask_ExecuteCompleted(object sender, FindEventArgs e)
        {
            var find = sender as FindTask;

            if (find == null || e.FindResults == null)
            {
                OnLocateComplete(new ResultEventArgs(Results));
                return;
            }
            int featureCount = e.FindResults.Count;
            if (featureCount > MaxRecords && MaxRecords > 0)            
                e.FindResults.RemoveRange(MaxRecords, featureCount - MaxRecords);
            
            foreach(FindResult result in e.FindResults)
            {
                
                if (!_collection.ContainsKey(result.LayerName))
                    AddFeatureSet(result);

                FeatureSet featSet = _collection[result.LayerName];
                if (!_featSetToID.ContainsKey(result.LayerName)) _featSetToID.Add(result.LayerName, result.LayerId);
                featSet.Features.Add(result.Feature);                
            }

            foreach (string key in _collection.Keys)
            {
                IResultSet resSet = new ResultSet(_collection[key]);
                resSet.Name = key;
                resSet.Service = Service;
                resSet.ID = _featSetToID[key];
                Results.Add(resSet);
               
            }
            
            OnLocateComplete(new ResultEventArgs(Results));
        }

        private void AddFeatureSet(FindResult result)
        {
            FeatureSet featureSet = new FeatureSet();
            _collection.Add(result.LayerName, featureSet);
        }

        #endregion
    }
}
