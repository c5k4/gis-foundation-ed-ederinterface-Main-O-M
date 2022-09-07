using System;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// The event arguments for the visibility of LayerItemViewModel. 
    /// </summary>
    public class IsVisibleEventArgs : EventArgs
    {
        public IsVisibleEventArgs(bool isVisible, bool isEnabledChanged)
        {
            IsVisible = isVisible;
            IsEnabledChanged = isEnabledChanged;
        }

        /// <summary>
        /// Property containing the visibility, used by the TOC control.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Whether the enabled property is changed.
        /// </summary>
        public bool IsEnabledChanged { get; set; }
    }
}