using System;
using System.Collections.Generic;
using System.Linq;

using ESRI.ArcGIS.Client;
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
    /// Task for getting display names for rows in a table 
    /// </summary>
    internal class DisplayNamerTask : Task
    {

        /// <summary>
        /// Initializes a new instance of the DisplayNamerTask class.
        /// </summary>
        internal DisplayNamerTask()
        {
        }

        /// <summary>
        /// Initializes a new instance of the DisplayNamerTask class.
        /// </summary>
        /// <param name="url">The URL of the map service.</param>
        internal DisplayNamerTask(string url)
            : base(url)
        {
           
        }

        /// <summary>
        /// Occurs when the task completes.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        internal event EventHandler<DisplayNamerEventArgs> ExecuteCompleted;

        /// <summary>
        /// Executes a query against a DisplayNamer service  
        /// The result is a list of names in a table for each ObjectID
        /// If the task is successful, the user-specified responder is invoked with the result.
        /// </summary>
        /// <param name="layerIndex">Index of the Layer/Table.</param>
        /// <param name="features"></param>
        /// <param name="userToken">A user-defined object that is passed to the method invoked when the asynchronous operation completes.</param>
        internal void ExecuteAsync(int layerIndex, IEnumerable<Graphic> features, object userToken = null)
        {
            if (features == null)
            {
                throw new ArgumentNullException("parameters");
            }
            if (string.IsNullOrEmpty(base.Url))
            {
                throw new InvalidOperationException("Url is not set");
            }

            string url = base.Url + @"/exts/ArcFMMapServer/id/" + layerIndex + "/displayNamer";

            base.SubmitRequest(url, this.GetParameters(features), new EventHandler<WebRequest.RequestEventArgs>(Request_Completed), userToken);
        }

        private Dictionary<string, string> GetParameters(IEnumerable<Graphic> features)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            if (features != null)
            {
                IEnumerable<int> objectIds = from feature in features
                                             select Utility.GetObjectIDValue(feature.Attributes);

                // Order them so that we always have the same query
                // This allows us to take advantage of REST caching
                string parameter = string.Join(",", objectIds.OrderBy(objectId => objectId));

                dictionary.Add("objectIDs", parameter);
            }
            return dictionary;
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            DisplayNamerResults info = DisplayNamerResults.FromResults(e.Result);
            DisplayNamerEventArgs args = new DisplayNamerEventArgs(info, e.UserState);
            this.OnExecuteCompleted(args);
        }

        private void OnExecuteCompleted(DisplayNamerEventArgs args)
        {
            var handler = this.ExecuteCompleted;
            if (handler != null)
            {
                base.Dispatcher.BeginInvoke(handler, new object[] { this, args });
            }
        }
    }

}
