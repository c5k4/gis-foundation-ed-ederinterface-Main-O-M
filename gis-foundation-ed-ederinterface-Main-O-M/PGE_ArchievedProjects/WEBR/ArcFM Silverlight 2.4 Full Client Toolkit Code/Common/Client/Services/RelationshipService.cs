using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;

using TaskFailedEventArgs = ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs;
#if SILVERLIGHT
using MinerTasks = Miner.Server.Client.Tasks;
using ResultSet = Miner.Server.Client.Tasks.ResultSet;
#elif WPF
using MinerTasks = Miner.Mobile.Client.Tasks;
using ResultSet = Miner.Mobile.Client.Tasks.ResultSet;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// This class processes relationships and returns related data.
    /// </summary>
    public class RelationshipService : IRelationshipService
    {
        //**********ENOS2EDGIS Start***********************
        //ENOS2EDGIS, Added a new public variable to apply a Definition Expression while searching for realted features.
        private string _defExpress = string.Empty;
        public string DefinitionExpression
        {
            get { return _defExpress; }
            set { _defExpress = value; }
        }
        //***********End**********************************

        #region Public Events

        public event EventHandler<RelationshipEventArgs> ExecuteCompleted;

        #endregion Public Events

        #region Internal Methods

        /// <summary>
        /// Gets the related rows for the specified objectIds
        /// </summary>
        /// <param name="data">configured relationship data</param>
        /// <param name="objectIDs">the object IDs of the features to discover relationships for</param>
        /// <param name="spatialReference">spatial reference to display related features in</param>
        public void GetRelationshipsAsync(IEnumerable<RelationshipInformation> data, int[] objectIDs, SpatialReference spatialReference = null)
        {
            if (this.IsBusy)
            {
                throw new NotSupportedException("Service does not support concurrent I/O operations.");
            }

            if (data == null)
            {
                throw new ArgumentNullException("data is null.");
            }

            Result = new Dictionary<int, RelationshipResult>();

            if (data.Count() == 0)
            {
                OnCompleted(new RelationshipEventArgs(ResultSet.FromResults(Result)));
            }
            else
            {
                IsBusy = true;
                foreach (RelationshipInformation relationshipInfo in data)
                {
                    if (relationshipInfo == null)
                    {
                        throw new NullReferenceException("relationshipInfo is null.");
                    }

                    var resultMap = new Dictionary<int, RelationshipResult>();

                    RelationshipArguments arguments = new RelationshipArguments
                    {
                        // Copy the info so we do not change the original
                        RelationshipInfo = new RelationshipInformation
                        {
                            LayerId = relationshipInfo.LayerId,
                            Service = relationshipInfo.Service,
                            RelationshipIds = relationshipInfo.RelationshipIds,
                            ProxyUrl = relationshipInfo.ProxyUrl
                        },
                        ObjectIds = new Dictionary<int, IEnumerable<int>> { { relationshipInfo.LayerId, objectIDs } }
                    };

                    ProcessRelationship(arguments, resultMap, spatialReference);
                }
            }
        }

        #endregion Internal Methods

        #region Protected Methods

        protected virtual void OnCompleted(RelationshipEventArgs args)
        {
            IsBusy = false;
            EventHandler<RelationshipEventArgs> handler = ExecuteCompleted;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion Protected Methods

        #region Private Properties

        private IDictionary<int, RelationshipResult> Result { get; set; }

        private int ProcessingWorkers { get; set; }

        private bool IsBusy { get; set; }

        #endregion Private Properties

        #region Private Methods

        private void RelationshipQuery_Failed(object sender, TaskFailedEventArgs e)
        {
            ProcessingWorkers--;
            if (ProcessingWorkers == 0)
            {
                // We are done
                OnCompleted(new RelationshipEventArgs(ResultSet.FromResults(Result, null, false)));
            }
        }

        private void ProcessRelationship(RelationshipArguments arguments, Dictionary<int, RelationshipResult> relationshipResults, SpatialReference spatialReference = null)
        {
            ProcessingWorkers++;
           //******ENOS2EDGIS start****************************
           // string[] outFields = GetOutFields(arguments);
            string[] outFields = new string[1];
            outFields[0] = "*";
           //******ENOS2EDGIS end****************************

            if (arguments.RelationshipInfo.RelationshipIds.Count <= arguments.Index)
            {
                throw new ArgumentOutOfRangeException("RelationshipInfoIds does not have this index.");
            }

            QueryTask task = new QueryTask(arguments.RelationshipInfo.Service + "/" + arguments.RelationshipInfo.LayerId);
            RelationshipParameter relationshipParameter = new RelationshipParameter
            {
                ObjectIds = arguments.ObjectIds[arguments.RelationshipInfo.LayerId],
                RelationshipId = arguments.RelationshipInfo.RelationshipIds[arguments.Index],
                OutFields = outFields,
                OutSpatialReference = spatialReference
            };

            task.ProxyURL = arguments.RelationshipInfo.ProxyUrl;
            task.ExecuteRelationshipQueryCompleted += (s, e) =>
            {
                relationshipResults.Add(arguments.RelationshipInfo.LayerId, e.Result);
                RelationshipQueryCompleted(arguments, relationshipParameter, relationshipResults);
            };
            task.Failed += RelationshipQuery_Failed;
            task.ExecuteRelationshipQueryAsync(relationshipParameter);
        }

        private void RelationshipQueryCompleted(RelationshipArguments arguments,
                                                RelationshipParameter relationshipParameter,
                                                Dictionary<int, RelationshipResult> resultMap)
        {
            MinerTasks.RelatedTableIdTask relatedIdTask = new MinerTasks.RelatedTableIdTask(arguments.RelationshipInfo.Service);
            relatedIdTask.ExecuteCompleted += (s1, e) =>
            {
                RelatedIdTaskComplete(arguments, resultMap, e.RelationshipId);
            };
            relatedIdTask.Failed += RelatedIdTaskFailed;
            relatedIdTask.ExecuteAsync(arguments.RelationshipInfo.LayerId, relationshipParameter.RelationshipId);
        }

        private void RelatedIdTaskFailed(object sender, Tasks.TaskFailedEventArgs taskFailedEventArgs)
        {
            ProcessingWorkers--;
            if (ProcessingWorkers == 0) OnCompleted(new RelationshipEventArgs(ResultSet.FromResults(Result, null, false)));
        }

        private void RelatedIdTaskComplete(RelationshipArguments arguments, Dictionary<int, RelationshipResult> resultMap, int relationshipId)
        {
            ProcessingWorkers--;
            arguments.Index++;
            if (arguments.Index == arguments.RelationshipInfo.RelationshipIds.Count)
            {
                if (arguments.RelationshipInfo.RelationshipIds.Count > 1)
                {
                    var result = CorrelateResults(resultMap);
                }
                if (!Result.ContainsKey(relationshipId))
                    Result.Add(new KeyValuePair<int, RelationshipResult>(relationshipId, resultMap[arguments.RelationshipInfo.LayerId]));
                if (ProcessingWorkers == 0)
                {
                    OnCompleted(new RelationshipEventArgs(ResultSet.FromResults(Result, null, false)));
                }
            }
            else
            {
                // ===== Begin fix for PR52251/MM52250/QFE52266 =====
                // When traversing multilevel related data, if a relationship returns no Object Ids then
                // we do not want to continue on to call ProcessRelationship because that would send a
                // time consuming query to the server (potentially timing out if the server is under load)
                // and not get results anyway.
                //
                // So, we'll call OnCompleted() to clear the accordion and display error message in the
                // attribute viewer.  Then we'll return.
                var relatedObjectIds = GetRelatedObjectIds(arguments.ObjectIds[arguments.RelationshipInfo.LayerId], resultMap[arguments.RelationshipInfo.LayerId]);
                if (relatedObjectIds == null)
                {
                    // I copied this line from RelationshipQuery_Failed()
                    OnCompleted(new RelationshipEventArgs(ResultSet.FromResults(Result, null, false)));
                    return;
                }
                // ===== End fix for PR52251/MM52250/QFE52266 =====

                arguments.ObjectIds.Add(relationshipId, relatedObjectIds);
                arguments.RelationshipInfo.LayerId = relationshipId;

                ProcessRelationship(arguments, resultMap);
            }
        }

        private static string[] GetOutFields(RelationshipArguments arguments)
        {
            string[] outFields = new string[1];
            if (arguments.Index == arguments.RelationshipInfo.RelationshipIds.Count - 1)
            {
                outFields[0] = "*";
            }
            else
            {
                outFields[0] = "OBJECTID";
            }
            return outFields;
        }

        private static List<int> GetRelatedObjectIds(IEnumerable<int> objectIds, RelationshipResult result)
        {
            if (result.RelatedRecordsGroup.Count <= 0) return null;

            List<int> relatedObjectIds = new List<int>();
            foreach (int objectID in objectIds)
            {
                if (result.RelatedRecordsGroup.ContainsKey(objectID)) //this is very unlikely to happen
                {
                    IEnumerable<Graphic> graphics = result.RelatedRecordsGroup[objectID];
                    foreach (Graphic graphic in graphics)
                    {
                        int relatedObjectId = Convert.ToInt32(graphic.Attributes["OBJECTID"]);
                        relatedObjectIds.Add(relatedObjectId);
                    }
                }
            }
            return relatedObjectIds;
        }


        internal static RelationshipResult CorrelateResults(IDictionary<int, RelationshipResult> relationshipResults)
        {
            if (relationshipResults == null) return null;

            var compressedRelationshipResults = new RelationshipResult();

            if (relationshipResults.Count <= 0) return compressedRelationshipResults;

            try
            {
                // Get the first one as it will be the object IDs we want.
                var oidsToGraphics = relationshipResults.First().Value.RelatedRecordsGroup;
                var temp = new Dictionary<int, List<Graphic>>();

               bool first = true;

                foreach (var relationshipResultByLayerId in relationshipResults)
                {
                    // We already have the first one so skip it.
                    if (first)
                    {
                        first = false;
                        continue;
                    }

                    RelationshipResult relationshipResult = relationshipResultByLayerId.Value;
                    if (relationshipResult.RelatedRecordsGroup.Count == 0) continue;

                   

                    // Go through the results in the top level item
                    foreach (var oidGraphics in oidsToGraphics)
                    {
                        // Collect the next level graphics where there is a relationship.
                        foreach (var graphic in oidGraphics.Value)
                        {
                            if (!relationshipResult.RelatedRecordsGroup.ContainsKey((int)graphic.Attributes["OBJECTID"])) continue;
                            //****************ENOS2EDGIS start************************************
                            if (relationshipResults.First().Key == 123)
                            {
                                foreach (var genGraphics in relationshipResult.RelatedRecordsGroup[(int)graphic.Attributes["OBJECTID"]])
                                {
                                    genGraphics.Attributes["STREETNAME1"] = graphic.Attributes["STREETNAME1"];
                                    genGraphics.Attributes["STREETNAME2"] = graphic.Attributes["STREETNAME2"];
                                    genGraphics.Attributes["STATE"] = graphic.Attributes["STATE"];
                                    genGraphics.Attributes["COUNTY"] = graphic.Attributes["COUNTY"];
                                    genGraphics.Attributes["STREETNUMBER"] = graphic.Attributes["STREETNUMBER"];
                                    genGraphics.Attributes["SPID"] = graphic.Attributes["SERVICEPOINTID"];
                                }
                            }
                            //****************ENOS2EDGIS end************************************

                            if (temp.ContainsKey(oidGraphics.Key))
                            {
                                temp[oidGraphics.Key].AddRange(relationshipResult.RelatedRecordsGroup[(int)graphic.Attributes["OBJECTID"]]);
                            }
                            else
                            {
                                temp.Add(oidGraphics.Key, relationshipResult.RelatedRecordsGroup[(int)graphic.Attributes["OBJECTID"]].ToList());
                            }
                        }
                    }

                    // Replace the current graphic results with the next level results.
                    foreach (var kvp in temp)
                    {
                        if (oidsToGraphics.ContainsKey(kvp.Key)) {
                            oidsToGraphics[kvp.Key] = kvp.Value;                          
                        }
                    }

                    // Set the compress results so we have the correct fields at the end since fields doesn't have a setter or an Add.
                    compressedRelationshipResults = relationshipResult;

                }

                // Clear the lowest level results as they are irrelevant.
                compressedRelationshipResults.RelatedRecordsGroup.Clear();

                // Add the compiled final results to the set.
                foreach (var kvp in oidsToGraphics)
                {
                    compressedRelationshipResults.RelatedRecordsGroup.Add(kvp);
                }

            }
            catch
            {
                //resultSets = null;
            }

            return compressedRelationshipResults;
        }

        #endregion Private Methods

        class RelationshipArguments
        {
            public RelationshipInformation RelationshipInfo { get; set; }
            public Dictionary<int, IEnumerable<int>> ObjectIds { get; set; }
            public int Index { get; set; }
        }
    }
}
