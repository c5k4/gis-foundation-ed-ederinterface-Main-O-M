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
    /// Task for getting protective devices names and ids from a map service. 
    /// </summary>
    public class ProtectiveDevicesTask : Task
    {
        private List<ProtectiveDevice> _lastResult;

        public ProtectiveDevicesTask()
        {
        }

        public ProtectiveDevicesTask(string url)
            : base(url)
        {
        }

#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<ProtectiveDevicesTaskEventArgs> ExecuteCompleted;

        /// <summary>
        /// Result of the last IdentifyModelNameTask execution.
        /// </summary>
        public List<ProtectiveDevice> LastResult
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

            base.SubmitRequest(base.Url + "/exts/ArcFMMapServer", null, Request_Completed, Url);
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            List<ProtectiveDevice> results = ProtectiveDevice.FromResults(e.Result, e.UserState.ToString());
            this.LastResult = results;
            ProtectiveDevicesTaskEventArgs args = new ProtectiveDevicesTaskEventArgs(results, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(ProtectiveDevicesTaskEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }
    }
}