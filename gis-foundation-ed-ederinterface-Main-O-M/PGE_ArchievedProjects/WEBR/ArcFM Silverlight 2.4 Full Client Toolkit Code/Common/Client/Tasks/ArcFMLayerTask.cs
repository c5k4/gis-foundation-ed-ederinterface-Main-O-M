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
    internal class ArcFMLayerTask : Task, IArcFMLayerTask
    {
        private List<LayerInformation> _lastResult;

        /// <summary>
        /// Initializes a new instance of the ArcFMLayerTask class.
        /// </summary>
        public ArcFMLayerTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ArcFMLayerTask class.
        /// </summary>
        /// <param name="url">The URL of the map service.</param>
        public ArcFMLayerTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Occurs when the task completes.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<LayerEventArgs> ExecuteCompleted;

        /// <summary>
        /// The result of the last execution of the ArcFMLayerTask.
        /// </summary>
        public List<LayerInformation> LastResult
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
        /// Executes a query against an ArcFM map service for its individual layers. 
        /// The result is returned as a List of LayerInformation. 
        /// If the task is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(object userToken = null)
        {
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }
            base.SubmitRequest(base.Url + "/exts/ArcFMMapServer", null, new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            List<LayerInformation> results = LayerInformation.ListFromResults(e.Result, this.Url, this.ProxyURL);
            this.LastResult = results;
            LayerEventArgs args = new LayerEventArgs(results, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(LayerEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }
    }
}
