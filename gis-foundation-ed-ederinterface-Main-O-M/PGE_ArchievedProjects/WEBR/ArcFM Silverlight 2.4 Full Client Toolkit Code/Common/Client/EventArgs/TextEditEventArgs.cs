using System;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// The event arguments for applying the text edit.
    /// </summary>
    public class TextEditEventArgs : EventArgs
    {
        public TextEditEventArgs(string xaml)
        {
            Xaml = xaml;
        }

        /// <summary>
        /// Property containing the Xaml content of the text edit.
        /// </summary>
        public string Xaml { get; set; }
    }
}
