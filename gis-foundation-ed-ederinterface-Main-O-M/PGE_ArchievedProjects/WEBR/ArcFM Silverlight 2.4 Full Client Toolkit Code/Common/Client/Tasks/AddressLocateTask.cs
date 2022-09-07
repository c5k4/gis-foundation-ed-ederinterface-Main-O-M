using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Task for querying addresses.
    /// </summary>
    public class AddressLocateTask : Task, ILocateTask
    {
        private const int AddressScore = 50;
        private static int _addressIndex = 0;

        private Locator _locatorTask;
        private List<IResultSet> _lastResult;
        private AddressLocateParameters _lastParameters;

        /// <summary>
        /// Initializes a new instance of the AddressLocateTask.
        /// </summary>
        public AddressLocateTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the AddressLocateTask.
        /// </summary>
        /// <param name="url">The URL of the geocode service.</param>
        public AddressLocateTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Occurs when the address locate completes.
        /// </summary>
        public event EventHandler<TaskResultEventArgs> ExecuteCompleted;

        /// <summary>
        /// The result of the last execution of the AddressLocateTask.
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
        /// Executes a query against the geocode service. The result is returned as a List of ResultSet. 
        /// If the address locate is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="parameters">Specifies the criteria used to locate the address.</param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(LocateParameters parameters, object userToken = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("Address search");
            }
            _lastParameters = parameters as AddressLocateParameters;
            if (string.IsNullOrWhiteSpace( _lastParameters.Fields))
            {
                throw new InvalidOperationException("Fields is not set");
            }

            if (string.IsNullOrEmpty(Url))
            {
                throw new InvalidOperationException("Url is not set");
            }

            if (_lastParameters.AddressScore > 100 || _lastParameters.AddressScore <= 0)
            {
                _lastParameters.AddressScore = AddressScore;
            }

            _locatorTask = new Locator(Url);
            _locatorTask.AddressToLocationsCompleted += Request_Completed;
            _locatorTask.Failed += Request_Failed;

            var addressParams = new AddressToLocationsParameters();
            addressParams.OutSpatialReference = parameters.SpatialReference;

            string query = _lastParameters.UserQuery;
            if (string.IsNullOrWhiteSpace(_lastParameters.AddressDefault) == false)
            {
                query += "," + _lastParameters.AddressDefault;
            }
            addressParams.Address.Add(_lastParameters.Fields, query);

            _locatorTask.AddressToLocationsAsync(addressParams, null);
        }

        private void Request_Completed(object sender, AddressToLocationsEventArgs e)
        {
            ConvertToResultset(e.Results, e.UserState);
        }

        private void Request_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            _lastResult = new List<IResultSet>();
            this.OnExecuteCompleted(new TaskResultEventArgs(_lastResult, e.UserState));
        }

        private void ConvertToResultset(List<AddressCandidate> returnedCandidates, object userToken = null)
        {
            var candidateList = new List<AddressCandidate>();
            if (returnedCandidates == null)
            {
                this.OnExecuteCompleted(new TaskResultEventArgs(_lastResult, userToken));

                return;
            }

            int candidateIndex = 0;
            var resultSet = new ResultSet();
            if (returnedCandidates.Count > 0)
            {
                resultSet.Name = "Address";
                resultSet.ID = 0;
                resultSet.ExceededThreshold = false;
                resultSet.DisplayFieldName = "MATCHED";
                resultSet.SpatialReference = _lastParameters.SpatialReference;
                resultSet.GeometryType = GeometryType.Point;
                resultSet.Service = Url;

                resultSet.FieldAliases.Add("MATCHED", "Matched Address");
                resultSet.FieldAliases.Add("SCORE", "Score");
                resultSet.FieldAliases.Add("X", "X");
                resultSet.FieldAliases.Add("Y", "Y");
                foreach (var kvp in returnedCandidates[0].Attributes)
                {
                    resultSet.FieldAliases.Add(kvp.Key, kvp.Key);
                }
            }

            foreach (var candidate in returnedCandidates)
            {
                if (candidate.Score >= _lastParameters.AddressScore)
                {
                    candidateIndex++;

                    candidateList.Add(candidate);
                    if (resultSet != null)
                    {
                        var graphic = new GraphicPlus();
                        string oid = (_addressIndex++).ToString();
                        graphic.Attributes.Add("OBJECTID", oid);
                        graphic.Attributes.Add("MATCHED", candidate.Address);
                        graphic.Attributes.Add("SCORE", candidate.Score);
                        graphic.Attributes.Add("X", candidate.Location.X);
                        graphic.Attributes.Add("Y", candidate.Location.Y);
                        foreach (var kvp in candidate.Attributes)
                        {
                            graphic.Attributes.Add(kvp.Key, kvp.Value);
                        }

                        var point = new MapPoint();
                        point.X = candidate.Location.X;
                        point.Y = candidate.Location.Y;

                        var geometry = point;
                        geometry.SpatialReference = _lastParameters.SpatialReference;
                        graphic.Geometry = geometry;
                        graphic.Attributes.Add("RowIndex", candidateIndex);
                        graphic.FieldAliases = new Dictionary<string, string>();
                        foreach (var kvp in resultSet.FieldAliases)
                        {
                            graphic.FieldAliases.Add(kvp.Key, kvp.Value);
                        }

                        resultSet.Features.Add(graphic);
                    }
                }
            }

            _lastResult = new List<IResultSet>();
            if (candidateList.Count > 0)
            {
                _lastResult.Add(resultSet);
            }

            this.OnExecuteCompleted(new TaskResultEventArgs(_lastResult, userToken));
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
