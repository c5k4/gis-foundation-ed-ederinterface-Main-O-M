using System.Windows;

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit
#elif WPF
namespace Miner.Mobile.Client.Toolkit
#endif
{
    /// <summary>
    /// Interface that helps the application determine which controls are active and which are not. 
    /// </summary>
    public interface IActiveControl
    {
        bool IsActive { get; set; }
        void OnControlActivated(DependencyObject control);
    }
}
