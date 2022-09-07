using System.ComponentModel.Composition.Hosting;
#if WPF
using System.Collections.Generic;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <exclude/>
    public interface IComposable
    {
        AggregateCatalog Catalog { get; set; }

#if SILVERLIGHT
        void Compose();
#elif WPF
        void Compose(Dictionary<string, string> uriRelatives);
#endif
    }
}
