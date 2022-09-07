using System;
#if SILVERLIGHT
using System.Windows.Browser;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    internal class DomainValuesTask : Task, IDomainValuesTask
    {
        private LayerInformation _lastResult;

        /// <summary>
        /// Initializes a new instance of the LayerTask class.
        /// </summary>
        public DomainValuesTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ArcFMDataTask class.
        /// </summary>
        /// <param name="url">The URL of the map service.</param>
        internal DomainValuesTask(string url)
            : base(url)
        {
        }

        /// <summary>
        /// Occurs when the task completes.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<LayerInformationEventArgs> ExecuteCompleted;

        /// <summary>
        /// The result of the last execution of the LayerTask.
        /// </summary>
        internal LayerInformation LastResult
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
        /// Executes a query against a ArcFMData service for its individual fields. 
        /// The result is returned as a List of Fields. 
        /// If the task is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="index">Index of the Layer/Table.</param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        public void ExecuteAsync(int index, object userToken = null)
        {
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }
            base.SubmitRequest(base.Url + "/" + index, null, new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            LayerInformation info = LayerInformation.FromResults(e.Result, Url, ProxyURL);
            this.LastResult = info;
            LayerInformationEventArgs args = new LayerInformationEventArgs(info, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(LayerInformationEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }

    }
}
