using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// One of two interfaces to implement in custom page templates.
    /// This one represents the custom control and has a map property. 
    /// </summary>
    public interface IPageTemplateView
    {
        Map Map { get; }
    }
}
