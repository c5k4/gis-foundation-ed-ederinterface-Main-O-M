using System.Windows.Input;

namespace PGE.Desktop.SchematicsMaintenance.Core
{
    public interface IToolCommand
    {
        ICommand SetCommandItemActivatedCommand { get; }
    }
}
