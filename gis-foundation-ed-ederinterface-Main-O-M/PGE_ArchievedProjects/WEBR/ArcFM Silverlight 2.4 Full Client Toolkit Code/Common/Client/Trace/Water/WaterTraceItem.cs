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
    /// Generic class that represents water trace items.
    /// </summary>
    public abstract class WaterTraceItem : TraceItem
    {
        #region ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        protected WaterTraceItem()
        {
            WaterTraceParameters = new WaterTraceParameters();
        }
        #endregion ctor

        #region public properties
        public WaterTraceParameters WaterTraceParameters { get; set; }
        #endregion public properties
    }
}
