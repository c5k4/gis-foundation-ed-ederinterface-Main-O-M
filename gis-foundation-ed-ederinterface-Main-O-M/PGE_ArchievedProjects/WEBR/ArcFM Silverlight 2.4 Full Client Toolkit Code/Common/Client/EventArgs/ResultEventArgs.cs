using System;
using System.Collections.Generic;

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
    /// <summary>
    /// The event arguments for Results coming back from identify, locate, trace, etc. 
    /// </summary>
    public class ResultEventArgs : EventArgs
    {

        public ResultEventArgs(IEnumerable<IResultSet> results)
        {
            Results = results;
        }

        /// <summary>
        /// Property containing the results, used by the attribute viewer. 
        /// </summary>
        public IEnumerable<IResultSet> Results { get; set; }
    }
}
