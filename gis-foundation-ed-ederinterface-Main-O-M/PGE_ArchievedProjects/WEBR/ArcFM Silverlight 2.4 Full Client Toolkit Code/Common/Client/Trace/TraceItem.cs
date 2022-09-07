using System;
using System.Collections.Generic;

#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Trace
#elif WPF
namespace Miner.Mobile.Client.Trace
#endif
{
    /// <summary>
    /// Generic class that represnts trace items.
    /// </summary>
    public abstract class TraceItem : ToolbarItem
    {
        #region public properties
        [Obsolete("Results property is obsolete. Please do not use it")]
        public IEnumerable<IResultSet> Results { get; set; }

        /// <summary>
        /// Trace parameters.
        /// </summary>
        public TraceParameters TraceParameters { get; set; }

        /// <summary>
        /// Additional trace parameters (usually used with custom traces).
        /// </summary>
        public TraceAdditionalParameters TraceAdditionalParameters { get; set; }
        #endregion public properties

        #region events
        /// <summary>
        /// Fires when a trace completes.
        /// </summary>
        public event EventHandler<ResultEventArgs> Traced;
        protected virtual void OnTraced(ResultEventArgs e)
        {
            EventHandler<ResultEventArgs> handler = this.Traced;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion events
    }
}
