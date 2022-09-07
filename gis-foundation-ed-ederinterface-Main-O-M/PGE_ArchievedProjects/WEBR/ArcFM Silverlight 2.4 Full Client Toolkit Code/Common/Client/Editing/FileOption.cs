#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    /// <summary>
    /// File options for exporting to xml.
    /// </summary>
    public enum FileOption
    {
        IsolatedStorage,
        FileSystem
    }
}
