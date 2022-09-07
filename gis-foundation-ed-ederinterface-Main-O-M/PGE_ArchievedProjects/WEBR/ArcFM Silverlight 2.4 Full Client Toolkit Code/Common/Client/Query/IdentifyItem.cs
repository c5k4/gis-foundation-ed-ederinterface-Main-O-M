using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Query
#elif WPF
namespace Miner.Mobile.Client.Query
#endif
{
    /// <summary>
    /// A class that represents an identify item.
    /// </summary>
    public abstract class IdentifyItem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IdentifyItem()
        {
            Results = new Collection<IResultSet>();
            Items = new ObservableCollection<IdentifyItem>();
        }

        /// <summary>
        /// Event that fires when an identify operation is done.
        /// </summary>
        public event EventHandler<ResultEventArgs> IdentifyComplete;

        /// <summary>
        /// Name of the IdentifyItem.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// A list of items on which to perform identify.
        /// </summary>
        public IList<IdentifyItem> Items { get; private set; }

        /// <summary>
        /// An asynchronous call to perform an identify operation.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="identifiedArea"></param>
        public abstract void IdentifyAsync(Map map, Geometry identifiedArea);

        /// <summary>
        /// Cancels the identify operation currently in progress.
        /// </summary>
        public abstract void CancelAsync();

        /// <exclude/>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Collection of results from the identify operation.
        /// </summary>
        protected Collection<IResultSet> Results { get; private set; }

        /// <exclude/>
        protected virtual void OnIdentifyComplete(ResultEventArgs e)
        {
            EventHandler<ResultEventArgs> handler = this.IdentifyComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
