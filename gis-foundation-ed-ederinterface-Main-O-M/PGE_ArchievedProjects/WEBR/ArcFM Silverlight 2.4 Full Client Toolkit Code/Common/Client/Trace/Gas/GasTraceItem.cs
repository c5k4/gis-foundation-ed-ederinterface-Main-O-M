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
    /// Generic class that represents the gas trace items.
    /// </summary>
    public abstract class GasTraceItem : TraceItem
    {
        #region ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        protected GasTraceItem()
        {
            GasTraceParameters = new GasTraceParameters();
        }
        #endregion ctor

        #region public properties
        public GasTraceParameters GasTraceParameters { get; set; }
        #endregion public properties
    }
}
