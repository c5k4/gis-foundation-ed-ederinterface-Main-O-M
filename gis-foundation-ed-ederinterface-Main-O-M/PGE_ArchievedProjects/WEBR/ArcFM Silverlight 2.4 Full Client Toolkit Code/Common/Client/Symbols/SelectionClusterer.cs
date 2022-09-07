using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
using Miner.Server.Client.Symbols;
#elif WPF
using Miner.Mobile.Client.Symbols;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// A utility class to manage clustering behavior for selected graphics.
    /// </summary>
    public class SelectionClusterer : GraphicsClusterer
    {
        private static readonly object Padlock = new object();
        private static readonly List<WeakReference> References = new List<WeakReference>();
        private readonly string _layerName;

        static SelectionClusterer()
        {
            MarkerManager.GroupRadiusChanged += MarkerManagerGroupRadiusChanged;
        }

        public SelectionClusterer() : this(string.Empty)
        {
        }

        public SelectionClusterer(string layerName)
        {
            _layerName = layerName;
            Radius = MarkerManager.GlobalGroupRadius;
            RegisterInstance(this);
        }

        private static void MarkerManagerGroupRadiusChanged(object sender, EventArgs e)
        {
            ChangeClusterRadius(MarkerManager.GlobalGroupRadius);
        }

        private static void RegisterInstance(SelectionClusterer clusterer)
        {
            References.Add(new WeakReference(clusterer));
        }

        private static void ChangeClusterRadius(int radius)
        {
            lock (Padlock)
            {
                var deadReferences = new List<WeakReference>();
                foreach (WeakReference weakReference in References)
                {
                    if (weakReference.IsAlive)
                    {
                        ((SelectionClusterer) weakReference.Target).Radius = radius;
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

        protected override Graphic OnCreateGraphic(GraphicCollection cluster, MapPoint center, int maxClusterCount)
        {
            if (cluster.Count == 1)
            {
                return cluster[0];
            }
            var graphics = new Graphic();
            var marker = new SelectionClusterMarker(_layerName);
            graphics.Symbol = marker;
            int count = 0;
            int selectedCount = 0;
            foreach (Graphic g in cluster)
            {
                g.PropertyChanged += (s, e) =>
                {
                    // We only care about changes to the Selected state.
                    if (!e.PropertyName.Equals("Selected", StringComparison.InvariantCulture))
                    {
                        return;
                    }
                    var changedGraphic = (Graphic) s;
                    var priorCount = (int) graphics.Attributes["SelectedCount"];
                    if (changedGraphic.Selected)
                    {
                        priorCount += 1;
                        // Flash the cluster to provide more visual indication of the location.
                        graphics.Selected = false;
                        graphics.Selected = true;
                    }
                    else
                    {
                        priorCount -= 1;
                        graphics.Selected = priorCount > 0;
                    }
                    graphics.Attributes["SelectedCount"] = priorCount;
                };
                if (g.Selected)
                {
                    selectedCount += 1;
                }
                count++;
            }

            graphics.Geometry = center;
            graphics.Attributes.Add("Count", count);
            graphics.Attributes["SelectedCount"] = selectedCount;
            if (selectedCount > 0)
            {
                graphics.Selected = true;
            }

            return graphics;
        }
    }
}