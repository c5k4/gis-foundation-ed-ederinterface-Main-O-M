using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal class VisibleLayerService : INotifyPropertyChanged
    {
        public VisibleLayerService(ITaskFactory<LayerTask> taskFactory)
        {
            if (taskFactory == null) throw new ArgumentNullException("taskFactory");

            TaskFactory = taskFactory;
            Layers = new ObservableCollection<LayerInformation>();
            ExecutingTasks = new List<Task>();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public IList<LayerInformation> Layers { get; private set; }

        public Map Map { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void SetMap(Map map)
        {
            if (Map == map) return;

            Layers.Clear();

            if (Map != null)
            {
                Map.Layers.CollectionChanged -= Layers_CollectionChanged;
                foreach (Layer layer in Map.Layers)
                {
                    layer.PropertyChanged -= Layer_PropertyChanged;
                }
            }

            Map = map;
            if (Map != null)
            {
                Map.Layers.CollectionChanged += Layers_CollectionChanged;
                foreach (Layer layer in Map.Layers)
                {
                    AddLayers(layer);
                    layer.PropertyChanged += Layer_PropertyChanged;
                }
            }
        }

        #endregion Public Methods

        #region Protected Properties

        protected IList<Task> ExecutingTasks { get; set; }

        #endregion Protected Properties

        #region Protected Methods

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void AddLayers(Layer layer)
        {
            if (layer.ContainsLayers() == false) return;
            if (layer.Visible == false) return;

            string url = layer.Url();
            if (string.IsNullOrEmpty(url)) return;

            var obj = ExecutingTasks.FirstOrDefault(t => t.Url == url);
            if (obj != null) return;

            //Get layers async
            LayerTask task = TaskFactory.CreateTask();
            task.Url = url;
            task.ProxyURL = layer.ProxyUrl();
            task.ExecuteCompleted += new EventHandler<LayerEventArgs>(AddLayerTask_ExecuteCompleted);
            task.ExecuteAsync();
            ExecutingTasks.Add(task);
        }

        protected void AddLayers(LayerEventArgs e, Task task)
        {
            ExecutingTasks.Remove(task);

            if (e == null) return;

            if (e.Results.Count > 0)
            {
                foreach (LayerInformation layerInfo in e.Results)
                {
                    Layers.Add(layerInfo);
                }
                OnPropertyChanged("Layers");
            }
        }

        #endregion Protected Methods

        #region Event Handlers

        private void Layers_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e == null) return;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Collection has been reset, so cancel any add/remove tasks currently being executed
                CancelTasks();
                Layers.Clear();
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (Layer layer in e.OldItems)
                    {
                        RemoveLayers(layer);
                        layer.PropertyChanged -= Layer_PropertyChanged;
                    }
                }
                if (e.NewItems != null)
                {
                    foreach (Layer layer in e.NewItems)
                    {
                        AddLayers(layer);
                        layer.PropertyChanged += Layer_PropertyChanged;
                    }
                }
            }
        }

        private void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Layer layer = (Layer)sender;
            if (e.PropertyName == "Visible")
            {
                if (layer.Visible)
                {
                    AddLayers(layer);
                }
                else
                {
                    RemoveLayers(layer);
                }
            }
        }

        private void AddLayerTask_ExecuteCompleted(object sender, LayerEventArgs e)
        {
            Task task = (Task)sender;
            AddLayers(e, task);
        }

        #endregion Event Handlers

        #region Private Properties

        private ITaskFactory<LayerTask> TaskFactory { get; set; }

        #endregion Private Properties

        #region Private Methods

        private void RemoveLayers(Layer layer)
        {
            string url = layer.Url();
            if (string.IsNullOrEmpty(url)) return;

            Task curTask;
            while ((curTask = ExecutingTasks.FirstOrDefault(t => t.Url == url)) != null)
            {
                curTask.CancelAsync();
                ExecutingTasks.Remove(curTask);
            }

            //No need to remove if nothing there
            if (Layers.Count == 0) return;

            List<LayerInformation> layers = Layers.Where(l => l.MapService == url).ToList();
            foreach (LayerInformation layerInfo in layers)
            {
                Layers.Remove(layerInfo);
            }
            OnPropertyChanged("Layers");
        }

        private void CancelTasks()
        {
            foreach (Task task in ExecutingTasks)
            {
                task.CancelAsync();
            }
            ExecutingTasks.Clear();
        }

        #endregion Private Methods
    }
}