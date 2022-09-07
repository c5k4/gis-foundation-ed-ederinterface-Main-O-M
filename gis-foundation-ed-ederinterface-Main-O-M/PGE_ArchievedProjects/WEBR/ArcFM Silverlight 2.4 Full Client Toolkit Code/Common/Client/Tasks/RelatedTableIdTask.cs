using System;
using System.Collections.Generic;
using System.Json;
#if SILVERLIGHT
using System.Windows.Browser;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    internal class RelatedTableIdTask : Task
    {
        private int _relatedTableId;

        /// <summary>
        /// Initializes a new instance of the RelatedTableIdTask.
        /// </summary>
        public RelatedTableIdTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the RelatedTableIdTask.
        /// </summary>
        /// <param name="url">The URL of the MapService.</param>
        public RelatedTableIdTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Occurs when the query completes.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<RelatedTableIdEventArgs> ExecuteCompleted;

        /// <summary>
        /// The result of the last execution of the RelatedTableIdTask.
        /// </summary>
        public int LastResult
        {
            get
            {
                return this._relatedTableId;
            }
            private set
            {
                this._relatedTableId = value;
                base.OnPropertyChanged("LastResult");
            }
        }

        /// <summary>
        /// Executes a query against an ArcGIS Server map layer. The result is returned as a List of ResultSet. 
        /// If the locate is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="layerId">Specifies the criteria used to locate the features.</param>
        /// <param name="relationshipTableId"></param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(int layerId, int relationshipTableId, object userToken = null)
        {
            if (layerId < 0)
            {
                throw new IndexOutOfRangeException("layerId");
            }
            if (string.IsNullOrEmpty(Url))
            {
                throw new InvalidOperationException("Url is not set");
            }
            base.SubmitRequest(Url + "/" + layerId, new Dictionary<string, string>(), (s,e) => Request_Completed(e, relationshipTableId), userToken);
        }

        private void Request_Completed(WebRequest.RequestEventArgs e, int relationshipTableId)
        {
            int relationshipId = GetRelatedTableId(e.Result, relationshipTableId);
            this.LastResult = relationshipId;
            RelatedTableIdEventArgs args = new RelatedTableIdEventArgs(relationshipId, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(RelatedTableIdEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }

        private int GetRelatedTableId(string jsonResult, int relationshipId)
        {
            JsonObject json = JsonObject.Parse(jsonResult) as JsonObject;
            if (json.ContainsKey(Constants.RelationshipsKey))
            {
                JsonArray jsonRelationships = json[Constants.RelationshipsKey] as JsonArray;
                if (jsonRelationships.Count > 0)
                {
                    foreach (JsonObject jsonRelationship in jsonRelationships)
                    {
                        if (jsonRelationship.ContainsKey(Constants.IDKey))
                        {
                            int id = Convert.ToInt32(jsonRelationship[Constants.IDKey].ToString());
                            if (id == relationshipId)
                            {
                                return Convert.ToInt32(jsonRelationship[Constants.RelatedTableIdKey].ToString());
                            }
                        }
                    }
                }
            }
            return -1;
        }
    }
}
