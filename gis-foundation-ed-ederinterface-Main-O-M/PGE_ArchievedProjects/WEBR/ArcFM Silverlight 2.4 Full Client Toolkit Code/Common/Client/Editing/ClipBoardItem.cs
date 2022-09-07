using System.Collections.Generic;

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    internal class ClipBoardItem
    {
        public ClipBoardItem()
        {
            Edits = new List<Edit>();
        }
        public List<Edit> Edits { get; private set; }
    }
}
