using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using ESRI.ArcGIS.Client.Geometry;
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Query
#elif WPF
namespace Miner.Mobile.Client.Query
#endif
{
    /// <summary>
    /// A class that represents a search item.
    /// </summary>
    public abstract class SearchItem
    {
        private string _description;

        /// <summary>
        /// Gets or sets the SpatialReference that results should be returned in.
        /// </summary>
        public SpatialReference SpatialReference { get; set; }

        internal ITaskFactory<ILocateTask> TaskFactory { get; set; }

        protected List<ILocateTask> ExecutingTasks { get; set; }

        /// <summary>
        /// Initializes a new instance of the Search class.
        /// </summary>
        protected SearchItem(ILocateTask task)
        {
            Results = new Collection<IResultSet>();
            SearchLayers = new SearchLayerCollection();
            MaxRecords = 500;
            AnnotationLayerID = -1;
            ExecutingTasks = new List<ILocateTask>();
            TaskFactory = new TaskFactory<ILocateTask>(task);
        }

        /// <summary>
        /// Event that fires when a search operation is done.
        /// </summary>
        public event EventHandler<ResultEventArgs> LocateComplete;

        /// <summary>
        /// Gets or sets the name of the Search.
        /// </summary>
        [Category("Common")]
        [Description("The title of the Search.")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the layers to search.
        /// </summary>
        [Category("Common")]
        [Description("The layers to search.")]
        public virtual SearchLayerCollection SearchLayers { get; set; }

        /// <summary>
        /// Gets or sets the max records to return.
        /// </summary>
        [Category("Common")]
        [Description("The Maximum number of records to return.")]
        public virtual int MaxRecords { get; set; }

        /// <summary>
        /// Gets or sets the ID of the annotaion layer.
        /// </summary>
        [Category("Common")]
        [Description("The ID of the annotaion layer.")]
        public virtual int AnnotationLayerID { get; set; }

        /// <summary>
        /// Gets or sets the description of the SearchItem.
        /// </summary>
        [Category("Common")]
        [Description("Description of the Search.")]
        public virtual string Description
        {
            get { return _description; }
            set { _description = value == null ? value : value.Replace("\\r\\n", Environment.NewLine); }
        }

        /// <summary>
        /// Gets or sets the whether the SearchItem is custom.
        /// </summary>
        [Category("Common")]
        [Description("Custom Search.")]
        public virtual bool IsCustom { get; set; }

        /// <summary>
        /// Cancel the locate action
        /// </summary>
        public void CancelAsync()
        {
            foreach (Task task in ExecutingTasks)
            {
                task.CancelAsync();
            }
            ExecutingTasks.Clear();
        }

        /// <summary>
        /// an Asynchronous call to perform a search operation.
        /// </summary>
        /// <param name="query"></param>
        public abstract void LocateAsync(string query);

        /// <summary>
        /// Gets or sets a collection of results from the search operation.
        /// </summary>
        protected Collection<IResultSet> Results { get; set; }

        /// <exclude/>
        protected virtual void OnLocateComplete(ResultEventArgs e)
        {
            EventHandler<ResultEventArgs> handler = this.LocateComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void Task_Failed(object sender, TaskFailedEventArgs e)
        {
            RemoveTask((LocateTask)sender);
        }

        protected virtual void Task_ExecuteCompleted(object sender, TaskResultEventArgs e)
        {
            if (e == null) return;
            if (e.Results == null) return;

            foreach (var result in e.Results)
            {
                Results.Add(result);
            }

            RemoveTask((ILocateTask)sender);
        }

        protected void ExecuteTaskAsync(LocateParameters parameters, string url)
        {
            ExecuteTaskAsync(parameters, url, null);
        }

        protected void ExecuteTaskAsync(LocateParameters parameters, SearchLayer layer)
        {
            ExecuteTaskAsync(parameters, layer.Url, layer.ProxyUrl);
        }

        protected virtual void ExecuteTaskAsync(LocateParameters parameters, string url, string proxyUrl)
        {
            ILocateTask task = TaskFactory.CreateTask();
            if (task == null) throw new ArgumentException("TaskFactory did not generate LocateTask");

            task.Url = url;
            if (proxyUrl != null)
            {
                task.ProxyURL = proxyUrl;
            }
            ExecutingTasks.Add(task);
            task.ExecuteCompleted += Task_ExecuteCompleted;
            task.Failed += Task_Failed;
            task.ExecuteAsync(parameters);
        }

        private void RemoveTask(ILocateTask task)
        {
            ExecutingTasks.Remove(task);

            if (ExecutingTasks.Count == 0)
            {
                OnLocateComplete(new ResultEventArgs(Results));
            }
        }
    }
}