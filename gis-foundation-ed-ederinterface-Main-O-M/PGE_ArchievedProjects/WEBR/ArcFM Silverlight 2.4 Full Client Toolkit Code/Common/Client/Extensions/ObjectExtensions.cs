#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    internal static class ObjectExtensions
    {
        internal static string NullSafeToString(this object obj)
        {
            return obj != null ? obj.ToString() : "Null";
        }
    } 
}
