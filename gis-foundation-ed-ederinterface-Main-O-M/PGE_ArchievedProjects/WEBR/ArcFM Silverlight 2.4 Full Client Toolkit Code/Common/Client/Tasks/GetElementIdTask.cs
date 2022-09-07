using System;
using System.Collections.Generic;
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
    /// Task for retrieving an element ID of a feature. 
    /// </summary>
    public class GetElementIdTask : Task
    {
        /// <summary>
        /// Initializes a new instance of the GetElementIdTask class.
        /// </summary>
        public GetElementIdTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GetElementIdTask class.
        /// </summary>
        /// <param name="url">The URL of the map service.</param>
        public GetElementIdTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Executes the task against an ArcFM Server map service. The result is returned as a FeatureElementId. 
        /// If the task is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="parameters">Specifies the criteria used to get an element id from a feature.</param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(GetElementIdParameters parameters, object userToken = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            if (parameters.Point == null)
            {
                throw new InvalidOperationException("ElectricTraceParameters.Point is null");
            }
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }

            base.SubmitRequest(base.Url + "/exts/ArcFMMapServer/GetNetworkEID", this.GetParameters(parameters), new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private Dictionary<string, string> GetParameters(GetElementIdParameters parameters)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (parameters.Point != null)
            {
                dictionary.Add("point", parameters.Point.ToJson(true));
            }
            dictionary.Add("modelName", parameters.ModelName);
            dictionary.Add("tolerance", parameters.Tolerance.ToString());
            if (parameters.SpatialReference != null)
            {
                dictionary.Add("spatialReference", parameters.SpatialReference.ToJson());
            }


            return dictionary;
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            FeatureElementID featureElementID = FeatureElementID.FromResults(e.Result);
            FeatureElementIDArgs args = new FeatureElementIDArgs(featureElementID, e.UserState);
            this.OnExecuteCompleted(args);
        }

        protected virtual void OnExecuteCompleted(FeatureElementIDArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }

        /// <summary>
        /// Occurs when the task completes.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<FeatureElementIDArgs> ExecuteCompleted;

    }
}
