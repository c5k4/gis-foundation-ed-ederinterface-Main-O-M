using System.Windows.Controls.Theming;

namespace Theming
{
    public partial class TelventGrayTheme : Theme
    {
        public TelventGrayTheme()
            : base(typeof (TelventGrayTheme).Assembly, "Theming.TelventGray.Theme.xaml")
        {
            DefaultStyleKey = typeof (TelventGrayTheme);
        }
    }
}