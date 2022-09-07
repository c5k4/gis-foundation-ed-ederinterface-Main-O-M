using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Threading;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// A utility class to manage marker symbols - opacity, clustering and visibility.
    /// 'Internal' class, only public for view binding. 
    /// </summary>
    /// <exclude/>
    public class MarkerManager : INotifyPropertyChanged
    {
        private static double _globalOpacity = 1d;
        private static int _globalGroupRadius = 25;
        private static string _name;
        private static readonly object Padlock = new object();
        private static readonly List<WeakReference> References = new List<WeakReference>();
        private static bool _showAll;
        private static readonly DispatcherTimer RefreshTimer = new DispatcherTimer();
        private static double _tempOpacity;
        private static bool _isHidden;

        private string _instanceName;
        private bool _isSelected = true;
        private double _opacity = 1d;

        static MarkerManager()
        {
            _tempOpacity = _globalOpacity;
            RefreshTimer.Tick += UpdateGraphicsLayers;
        }

        public MarkerManager()
        {
            RegisterInstance(this);
        }

        /// <summary>
        /// Default opacity for every marker
        /// </summary>
        public static double GlobalOpacity
        {
            get { return _globalOpacity; }
            set
            {
                value = ConditionOpacityValue(value);
                if (value == _globalOpacity) return;
                _globalOpacity = value;
                UpdateOnNamed();
            }
        }

        /// <summary>
        /// Default clustering radius for every marker
        /// </summary>
        public static int GlobalGroupRadius
        {
            get { return _globalGroupRadius; }
            private set
            {
                if (value == _globalGroupRadius) return;
                
                foreach (var layer in GraphicsLayers.AllLayers())
                {
                    if (value > 0 && _globalGroupRadius == 0)
                    {
                        layer.Clusterer = new SelectionClusterer(layer.ID.Substring(0, layer.ID.Length - 8));
                    }
                    else if (value == 0 && _globalGroupRadius > 0)
                    {
                        layer.Clusterer = null;
                    }
                }

                foreach (WeakReference weakReference in References)
                {
                    if (!weakReference.IsAlive) continue;
                    var target = (MarkerManager) weakReference.Target;
                    target.NotifyPropertyChanged("GroupRadius");
                }

                _globalGroupRadius = value;
                RefreshTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
                RefreshTimer.Start();
                OnGroupRadiusChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Instance name
        /// </summary>
        public string Name
        {
            get { return _instanceName; }
            set
            {
                _instanceName = value;
                _opacity = DetermineOpacity(this);
                IsSelected = IsMarkerSelected(this);
            }
        }

        /// <summary>
        /// Bindable opacity for a single marker
        /// </summary>
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                value = ConditionOpacityValue(value);
                if (value == _opacity) return;
                _opacity = value;
                NotifyPropertyChanged("Opacity");
            }
        }

        /// <summary>
        /// Bindable selection for a single marker
        /// </summary>
        public bool IsSelected
        {
            get { return _isSelected; }
            private set
            {
                if (value == _isSelected) return;
                _isSelected = value;
                NotifyPropertyChanged("IsSelected");
                NotifyPropertyChanged("FillOpacity");
            }
        }

        /// <summary>
        /// Clustering radius for a single marker
        /// </summary>
        public int GroupRadius
        {
            get { return GlobalGroupRadius; }
            set { GlobalGroupRadius = value; }
        }

        /// <summary>
        /// Fill opacity for a single marker
        /// </summary>
        public double FillOpacity
        {
            get { return IsSelected ? 0.5d : 0.1d; }
        }

        /// <summary>
        /// Bindable wrapper on the static GlobalOpacity property
        /// </summary>
        public double GlobalOpacityValue
        {
            get { return GlobalOpacity; }
            set
            {
                value = ConditionOpacityValue(value);
                GlobalOpacity = value;
                NotifyPropertyChanged("GlobalOpacityValue");
            }
        }

        /// <summary>
        /// Gets and sets the value that determines whether all markers should be shown, 
        /// instead of just the markers for the current tab in the attribute viewer.
        /// </summary>
        public bool ShowAllMarkers
        {
            get { return _showAll; }
            set
            {
                if (value == _showAll) return;
                _showAll = value;
                foreach (WeakReference weakReference in References)
                {
                    if (!weakReference.IsAlive) continue;
                    var target = (MarkerManager) weakReference.Target;
                    target.NotifyPropertyChanged("ShowAllMarkers");
                }
                UpdateOnNamed();
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        /// <summary>
        /// Convenience method that hides the markers until we're done zooming in.
        /// </summary>
        public static void HideMarkers()
        {
            if (_isHidden) return;
            _isHidden = true;
            _tempOpacity = _globalOpacity;
            GlobalOpacity = 0d;
        }

        /// <summary>
        /// Convenience method that shows the markers when we're done zooming in. 
        /// </summary>
        public static void ShowMarkers()
        {
            if (!_isHidden) return;
            GlobalOpacity = _tempOpacity;
            _isHidden = false;
        }

        public static event EventHandler GroupRadiusChanged;

        private static void OnGroupRadiusChanged(EventArgs e)
        {
            EventHandler handler = GroupRadiusChanged;
            if (handler != null)
            {
                handler(null, e);
            }
        }

        private static void UpdateGraphicsLayers(object state, EventArgs e)
        {
            RefreshTimer.Stop();
            GraphicsLayers.Refresh();
        }

        public static void ToggleOnNamed(string name)
        {
            _name = name;
            UpdateOnNamed();
        }

        private static void UpdateOnNamed()
        {
            lock (Padlock)
            {
                var deadReferences = new List<WeakReference>();
                foreach (WeakReference weakReference in References)
                {
                    if (weakReference.IsAlive)
                    {
                        var target = (MarkerManager) weakReference.Target;
                        UpdateMarkerInformation(target);
                    }
                    else
                    {
                        deadReferences.Add(weakReference);
                    }
                }
                foreach (WeakReference deadReference in deadReferences)
                {
                    References.Remove(deadReference);
                }
            }
        }

        private static void UpdateMarkerInformation(MarkerManager target)
        {
            if (IsMarkerSelected(target))
            {
                target.IsSelected = true;
                target.Opacity = GlobalOpacity;
            }
            else
            {
                target.IsSelected = false;
                target.Opacity = 0d;
            }
        }

        public static double DetermineOpacity(MarkerManager markerManager)
        {
            return IsMarkerSelected(markerManager) ? GlobalOpacity : 0d;
        }

        private static bool IsMarkerSelected(MarkerManager markerManager)
        {
            return _showAll ||
                   string.IsNullOrEmpty(_name) ||
                   _name.Equals(markerManager.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        private static double ConditionOpacityValue(double value)
        {
            if (value < 0.50d)
            {
                value = 0d;
            }
            else if (value < 1.0d)
            {
                value = 0.50d;
            }
            else
            {
                value = 1d;
            }
            return value;
        }

        private static void RegisterInstance(MarkerManager markerManager)
        {
            References.Add(new WeakReference(markerManager));
            markerManager._opacity = DetermineOpacity(markerManager);
            markerManager.IsSelected = IsMarkerSelected(markerManager);
        }

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}