using System.Collections.Generic;

#if SILVERLIGHT
namespace Miner.Server.Client.Tasks
#elif WPF
namespace Miner.Mobile.Client.Tasks
#endif
{
    /// <summary>
    /// Event arguments used by the LayerService
    /// </summary>
    public class LayerEventArgs : TaskEventArgs
    {
        public LayerEventArgs(ICollection<LayerInformation> results, object userToken)
            : base(userToken)
        {
            Results = results;
        }

        /// <summary>
        /// Collection of layer information retrieved from the LayerService
        /// </summary>
        public ICollection<LayerInformation> Results { get; private set; }
    }
}
