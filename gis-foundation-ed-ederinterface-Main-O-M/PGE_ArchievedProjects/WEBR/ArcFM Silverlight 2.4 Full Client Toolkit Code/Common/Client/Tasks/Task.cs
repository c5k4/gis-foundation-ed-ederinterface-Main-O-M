using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
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
    /// Base class for tasks.
    /// </summary>
    public abstract class Task : INotifyPropertyChanged, ITask
    {
        private WebRequest _request;

        /// <summary>
        /// Initializes a new instance of the Task class.
        /// </summary>
        internal Task() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Task class.
        /// </summary>
        /// <param name="url">The URL of the MapService.</param>
        internal Task(string url)
        {
            this.Url = url;
            this._request = new WebRequest();
            this._request.Completed += new EventHandler<WebRequest.RequestEventArgs>(Request_Completed);
            this._request.Failed += new EventHandler<WebRequest.RequestEventArgs>(Request_Failed);
        }

        /// <summary>
        /// Occurs when the query fails.
        /// </summary>
#if SILVERLIGHT
        [ScriptableMember]
#endif
        public event EventHandler<TaskFailedEventArgs> Failed;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Cancels a pending asynchronous operation. 
        /// </summary>
        /// <remarks>If an operation is pending, this method calls Abort on the underlying WebRequest.</remarks>
        public void CancelAsync()
        {
            if (IsBusy)
            {
                _request.CancelAsync();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if client caching should be disabled.
        /// If true, adds a timestamp parameter ("_ts") to the request to prevent it from being loaded from the browser's cache. 
        /// </summary>
        /// <value>true to disable client caching otherwise, false.</value>
        public bool DisableClientCaching
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this._request.IsBusy;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ProxyURL
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        protected internal Dispatcher Dispatcher
        {
            get
            {
#if SILVERLIGHT
                return Application.Current.RootVisual.Dispatcher;
#elif WPF
                return Application.Current.Dispatcher;
#endif
                
            }
        }

        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        internal void OnTaskFailed(TaskFailedEventArgs args)
        {
            if (this.Failed != null)
            {
                this.Dispatcher.BeginInvoke(new EventHandler<TaskFailedEventArgs>(this.Failed.Invoke), new object[] { this, args });
            }
        }

        internal void SubmitRequest(string url, Dictionary<string, string> parameters, EventHandler<WebRequest.RequestEventArgs> onCompleted, object state)
        {
            this.SubmitRequest(url, parameters, onCompleted, state, false);
        }

        internal void SubmitRequest(string url, Dictionary<string, string> parameters, EventHandler<WebRequest.RequestEventArgs> onCompleted, object state, bool forcePost)
        {
            if (this.IsBusy)
            {
                throw new NotSupportedException("Task does not support concurrent I/O operations. Cancel the pending request or wait for it to complete.");
            }
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }
            this.AppendBaseQueryParameters(parameters);
            this._request.Url = url;
            this._request.Parameters = parameters;
            this._request.ProxyUrl = this.ProxyURL;
            this._request.ForcePost = forcePost;
            this._request.BeginRequest(new object[] { state, onCompleted });
        }


        private void AppendBaseQueryParameters(Dictionary<string, string> parameters)
        {
            parameters.Add("f", "json");
            if (this.DisableClientCaching)
            {
                parameters.Add("_ts", DateTime.Now.Ticks.ToString());
            }
            if (!string.IsNullOrEmpty(this.Token))
            {
                parameters.Add("token", this.Token);
            }
        }

        private void Request_Completed(object sender, WebRequest.RequestEventArgs e)
        {
            if (!this.CheckForServiceError(e))
            {
                object[] userState = (object[])e.UserState;
                EventHandler<WebRequest.RequestEventArgs> handler = userState[1] as EventHandler<WebRequest.RequestEventArgs>;
                if (handler != null)
                {
                    handler(this, new WebRequest.RequestEventArgs(userState[0], e.Result));
                }
            }
        }

        private bool CheckForServiceError(WebRequest.RequestEventArgs e)
        {
            ServiceException error = ServiceException.CreateFromJson(e.Result);
            if (error != null)
            {
                this.OnTaskFailed(new TaskFailedEventArgs(error, e.UserState));
                return true;
            }
            return false;
        }

        private void Request_Failed(object sender, WebRequest.RequestEventArgs e)
        {
            this.OnTaskFailed(new TaskFailedEventArgs(e.Error, e.UserState));
        }
    }
}