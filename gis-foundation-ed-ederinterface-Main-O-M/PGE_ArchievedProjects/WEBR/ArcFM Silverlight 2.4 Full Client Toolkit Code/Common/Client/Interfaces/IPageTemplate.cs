#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// One of two interfaces to implement in custom page templates.
    /// This one holds the name and a reference to the control(view). 
    /// </summary>
    public interface IPageTemplate
    {
        string Name { get; }

        IPageTemplateView View { get; }

    }
}
