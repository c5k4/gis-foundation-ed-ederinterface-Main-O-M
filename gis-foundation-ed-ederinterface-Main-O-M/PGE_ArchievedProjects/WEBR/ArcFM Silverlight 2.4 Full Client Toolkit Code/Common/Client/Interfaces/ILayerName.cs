#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Provides the name for custom symbols
    /// </summary>
    public interface ILayerName
    {
        string Name { get; }
    }
}