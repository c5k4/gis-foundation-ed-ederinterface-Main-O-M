using System.Collections.ObjectModel;

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    internal class Edits : ObservableCollection<Edit>
    {
    }
}
