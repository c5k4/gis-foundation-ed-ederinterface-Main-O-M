using System;
using System.Collections.Generic;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Task for tracing gas features from a map service. 
    /// </summary>
    public class GasTraceTask : TraceTask
    {
        /// <summary>
        /// Initializes a new instance of the GasTraceTask class.
        /// </summary>
        public GasTraceTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the GasTraceTask class.
        /// </summary>
        /// <param name="url">The URL of the map service.</param>
        public GasTraceTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Executes a gas trace against an ArcFM Server map service. The result is returned as a List of ResultSet. 
        /// If the trace is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="parameters">Specifies the criteria used to trace features.</param>
        /// <param name="additionalParameters">Specifies the additional criteria that a caller may want to add to affect the trace.</param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(GasTraceParameters parameters, TraceAdditionalParameters additionalParameters, object userToken = null)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("idParameters");
            }
            if (parameters.StartPoint == null)
            {
                throw new InvalidOperationException("GasTraceParameters.Point is null");
            }
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }
            base.SubmitRequest(base.Url + "/exts/ArcFMMapServer/Gas Trace", this.GetParameters(parameters, additionalParameters), new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private Dictionary<string, string> GetParameters(GasTraceParameters parameters, TraceAdditionalParameters additionalParameters)
        {
            Dictionary<string, string> dictionary = base.GetParameters(parameters);
            dictionary.Add("traceType", parameters.TraceType.ToString());

            if (parameters.ValveIsolationTraceBarriers.HasValue)
            {
                dictionary.Add("isolationTraceBarriers", parameters.ValveIsolationTraceBarriers.Value.ToString());
            }
            if ((parameters.ExcludedValves != null) && (parameters.ExcludedValves.Count > 0))
            {
                dictionary.Add("excludedValves", GetCollection(parameters.ExcludedValves));
            }
            if ((parameters.IncludedValves != null) && (parameters.IncludedValves.Count > 0))
            {
                dictionary.Add("includedValves", GetCollection(parameters.IncludedValves));
            }
            if ((parameters.SqueezeOffs != null) && (parameters.SqueezeOffs.Count > 0))
            {
                dictionary.Add("squeezeOffs", GetCollection(parameters.SqueezeOffs));
            }
            if ((parameters.TemporarySources != null) && (parameters.TemporarySources.Count > 0))
            {
                dictionary.Add("temporarySources", GetCollection(parameters.TemporarySources));
            }
            if (parameters.PressureTraceBarriers.HasValue)
            {
                dictionary.Add("pressureSystemTraceBarriers", parameters.PressureTraceBarriers.Value.ToString());
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
