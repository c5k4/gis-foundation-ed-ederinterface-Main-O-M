using System;
using System.Collections.Generic;
using System.Linq;
#if SILVERLIGHT
using System.Windows.Browser;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Task for querying features.
    /// </summary>
    public class LocateTask : Task, ILocateTask
    {
        private List<IResultSet> _lastResult;
        private string SearchTitle = null;

        /// <summary>
        /// Initializes a new instance of the LocateTask.
        /// </summary>
        public LocateTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the LocateTask.
        /// </summary>
        /// <param name="url">The URL of the MapService.</param>
        public LocateTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Occurs when the locate completes.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<TaskResultEventArgs> ExecuteCompleted;

        /// <summary>
        /// The result of the last execution of the LocateTask.
        /// </summary>
        public List<IResultSet> LastResult
        {
            get
            {
                return this._lastResult;
            }
            private set
            {
                this._lastResult = value;
                base.OnPropertyChanged("LastResult");
            }
        }

        /// <summary>
        /// Executes a query against an ArcFM Server map layer. The result is returned as a List of ResultSet. 
        /// If the locate is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="parameters">Specifies the criteria used to locate the features.</param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(LocateParameters parameters, object userToken = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("search");
            }
            if ((parameters.SearchLayer == null) || (parameters.SearchLayer == null))
            {
                throw new InvalidOperationException("Must contain a SearchLayer");
            }
            if (string.IsNullOrEmpty(Url))
            {
                throw new InvalidOperationException("Url is not set");
            }
            base.SubmitRequest(Url + @"/exts/ArcFMMapServer/id/" + parameters.SearchLayer.ID + "/locate", this.GetParameters(parameters), new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private Dictionary<string, string> GetParameters(LocateParameters parameters)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (parameters.UserQuery == "%" || parameters.UserQuery == "*")
            {
                parameters.ExactMatch = false;
            }

            dictionary.Add("query", parameters.UserQuery);
            dictionary.Add("returnAttributes", parameters.ReturnAttributes.ToString());
            dictionary.Add("exactMatch", parameters.ExactMatch.ToString());
            dictionary.Add("maxRecords", parameters.MaxRecords.ToString());

            SearchLayer layer = parameters.SearchLayer;
            if (layer.Fields != null)
            {
                dictionary.Add("layerFields", string.Join(",", layer.Fields.Select<string, string>(delegate(string x)
                {
                    return x.ToString();
                }).ToArray<string>()));
            }
            if (layer.SearchRelationship != null)
            {
                dictionary.Add("table", layer.SearchRelationship.TableName);
                dictionary.Add("relationshipFields", string.Join(",", layer.SearchRelationship.Fields.Select<string, string>(delegate(string x)
                {
                    return x.ToString();
                }).ToArray<string>()));
                dictionary.Add("path", layer.SearchRelationship.Path);
            }
            if (parameters.SpatialReference != null)
            {
                dictionary.Add("spatialReference", parameters.SpatialReference.ToJson());
            }
            SearchTitle = parameters.SearchTitle;
            return dictionary;
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            List<IResultSet> results = ResultSet.FromResults(e.Result, Url, ProxyURL, SearchTitle);
            this.LastResult = results;
            TaskResultEventArgs args = new TaskResultEventArgs(results, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(TaskResultEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }
    }
}
