#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Selection options for the management of result sets.
    /// </summary>
    public enum SelectionOption
    {
        CreateNewSelection,
        AddToSelection,
        RemoveFromSelection,
        SelectFromSelection
    }
}