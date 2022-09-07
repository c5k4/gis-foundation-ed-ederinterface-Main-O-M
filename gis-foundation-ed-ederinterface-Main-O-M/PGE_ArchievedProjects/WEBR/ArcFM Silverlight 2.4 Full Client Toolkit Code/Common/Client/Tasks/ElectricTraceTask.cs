using System;
using System.Collections.Generic;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Task for tracing electric features from a map service. 
    /// </summary>
    public class ElectricTraceTask : TraceTask
    {
        /// <summary>
        /// Initializes a new instance of the ElectricTraceTask class.
        /// </summary>
        public ElectricTraceTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ElectricTraceTask class.
        /// </summary>
        /// <param name="url">The URL of the map service.</param>
        public ElectricTraceTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Executes an electric trace against an ArcFM Server map service. The result is returned as a List of ResultSet. 
        /// If the trace is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="parameters">Specifies the criteria used to trace features.</param>
        /// <param name="additionalParameters">Specifies the additional criteria that a caller may want to add to affect the trace.</param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(ElectricTraceParameters parameters, TraceAdditionalParameters additionalParameters = null, object userToken = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }
            if (parameters.StartPoint == null)
            {
                throw new InvalidOperationException("ElectricTraceParameters.Point is null");
            }
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }
            base.SubmitRequest(base.Url + "/exts/ArcFMMapServer/Electric Trace", this.GetParameters(parameters, additionalParameters), new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private Dictionary<string, string> GetParameters(ElectricTraceParameters parameters, TraceAdditionalParameters additionalParameters)
        {
            Dictionary<string, string> dictionary = base.GetParameters(parameters);
            dictionary.Add("traceType", parameters.TraceType.ToString());
            dictionary.Add("phasesToTrace", parameters.PhasesToTrace.ToString());

            if (parameters.ProtectiveDevices != null)
            {
                dictionary.Add("protectiveDevices", GetCollection(parameters.ProtectiveDevices));
            }

            //append addional parameters
            if (additionalParameters != null)
            {
                foreach (KeyValuePair<string, string> kvp in additionalParameters.Parameters)
                {
                    if (dictionary.ContainsKey(kvp.Key) == false)
                    {
                        dictionary.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return dictionary;
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            string service = WebRequest.ProxyEncodeUrlAsString(Url, ProxyURL);
            List<IResultSet> results = ResultSet.FromResults(e.Result, service);
            base.LastResult = results;
            TaskResultEventArgs args = new TaskResultEventArgs(results, e.UserState);
            this.OnExecuteCompleted(args);
        }

    }
}