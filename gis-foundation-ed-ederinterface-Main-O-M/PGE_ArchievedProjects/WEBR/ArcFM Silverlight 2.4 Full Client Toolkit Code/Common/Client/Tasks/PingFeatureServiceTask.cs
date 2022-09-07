using System;
using System.Json;
#if SILVERLIGHT
using System.Windows.Browser;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{

    /// <exclude/>
    public class PingFeatureServiceTask : Task
    {

        public new string Url
        {
            get
            {
                return base.Url;
            }

            set
            {
                string url = value;
                base.Url = url.Replace("MapServer", "FeatureServer");
            }
        }

#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<ServiceAliveEventArgs> ExecuteCompleted;

        public void ExecuteAsync(int layerId, object userToken = null)
        {
            if (layerId < 0)
            {
                throw new IndexOutOfRangeException("layerId");
            }
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }

            base.Url += @"/" + layerId.ToString();
            base.SubmitRequest(base.Url, null, new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            bool isAlive = FromResults(e.Result);
            ServiceAliveEventArgs args = new ServiceAliveEventArgs(isAlive, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(ServiceAliveEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }

        internal static bool FromResults(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;

            JsonObject jsonObject = JsonObject.Parse(json) as JsonObject;
            if (jsonObject.ContainsKey("error")) return false;

            return true;
        }
    }
}
