using System.Collections.Generic;
using System.Collections.ObjectModel;

#if SILVERLIGHT
namespace Miner.Server.Client.Query
#elif WPF
namespace Miner.Mobile.Client.Query
#endif
{
    /// <summary>
    /// A collection of searches.
    /// </summary>
    public class SearchCollection : Collection<SearchItem>
    {
        public SearchCollection()
            : base()
        { }


        public SearchCollection(IList<SearchItem> list)
            : base(list)
        { }
    }
}