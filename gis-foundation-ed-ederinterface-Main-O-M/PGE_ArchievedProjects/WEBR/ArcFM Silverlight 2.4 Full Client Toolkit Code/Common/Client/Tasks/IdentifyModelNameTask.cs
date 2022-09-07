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
    /// Task for getting identify model names from a map service. 
    /// </summary>
    public class IdentifyModelNameTask : Task
    {
        private List<ModelName> _lastResult;

        public IdentifyModelNameTask()
        {
        }

        public IdentifyModelNameTask(string url)
            : base(url)
        {
        }

#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<ModelNameTaskEventArgs> ExecuteCompleted;

        /// <summary>
        /// Result of the last IdentifyModelNameTask execution.
        /// </summary>
        public List<ModelName> LastResult
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
        /// Execute the current IdentifyModelNameTask
        /// </summary>
        /// <param name="userToken"></param>
        public void ExecuteAsync(object userToken = null)
        {
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }

            if (base.Url.Contains("/FeatureServer")) return;

            base.SubmitRequest(base.Url + "/exts/ArcFMMapServer", null, Request_Completed, Url);
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            List<ModelName> results = ModelName.FromResults(e.Result, e.UserState.ToString());
            this.LastResult = results;
            ModelNameTaskEventArgs args = new ModelNameTaskEventArgs(results, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(ModelNameTaskEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }
    }
}