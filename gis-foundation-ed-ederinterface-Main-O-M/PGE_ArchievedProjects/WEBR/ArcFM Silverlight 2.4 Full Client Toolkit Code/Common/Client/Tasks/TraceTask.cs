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
    /// Task for tracing features from a map service. 
    /// </summary>
    public abstract class TraceTask : Task
    {
        private List<IResultSet> _lastResult;

        /// <summary>
        /// Initializes a new instance of the TraceTask class.
        /// </summary>
        public TraceTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the TraceTask class.
        /// </summary>
        /// <param name="url">The URL of the REST MapService.</param>
        public TraceTask(string url) : base(url)
        {
        }

        /// <summary>
        /// Occurs when the trace completes.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<TaskResultEventArgs> ExecuteCompleted;

        /// <summary>
        /// Gets or sets the last trace result.
        /// </summary>
        public List<IResultSet> LastResult
        {
            get
            {
                return this._lastResult;
            }
            protected set
            {
                this._lastResult = value;
                base.OnPropertyChanged("LastResult");
            }
        }

        protected virtual Dictionary<string, string> GetParameters(TraceParameters parameters)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            if (parameters.StartPoint != null)
            {
                dictionary.Add("startPoint", parameters.StartPoint.ToJson(true));
            }
            dictionary.Add("drawComplexEdges", parameters.DrawComplexEdges.ToString());
            dictionary.Add("includeEdges", parameters.IncludeEdges.ToString());
            dictionary.Add("includeJunctions", parameters.IncludeJunctions.ToString());
            dictionary.Add("returnAttributes", parameters.ReturnAttributes.ToString());
            dictionary.Add("tolerance", parameters.Tolerance.ToString());
            dictionary.Add("returnGeometries", "true");
            if (parameters.SpatialReference != null)
            {
                dictionary.Add("spatialReference", parameters.SpatialReference.ToJson());
            }

            return dictionary;
        }

        protected virtual void OnExecuteCompleted(TaskResultEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }

        protected string GetCollection(IEnumerable<int> collection)
        {
            return string.Join(",", collection.Select<int, string>(delegate(int x)
             {
                 return x.ToString();
             }).ToArray<string>());
        }
    }
}