using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Theming
{
    public class SecondaryTabControl : TabControl
    {
        public SecondaryTabControl()
        {
            DefaultStyleKey = typeof (SecondaryTabControl);
        }
    }

    public class SecondaryTabItem : TabItem
    {
        public SecondaryTabItem()
        {
            DefaultStyleKey = typeof(SecondaryTabItem);
        }
    }

    public class IconButton : Button
    {
        public IconButton()
        {
            DefaultStyleKey = typeof (IconButton);
        }
    }

    public class SecondaryToggleButton : ToggleButton
    {
        public SecondaryToggleButton()
        {
            DefaultStyleKey = typeof (SecondaryToggleButton);
        }
    }
}