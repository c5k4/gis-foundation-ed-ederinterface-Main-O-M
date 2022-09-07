using System.Windows;
using System.Windows.Controls;

namespace ArcFMSilverlight
{
    public class RibbonPanel : ContentControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title",
                                        typeof (string),
                                        typeof (RibbonPanel),
                                        new PropertyMetadata(string.Empty));

        public RibbonPanel()
        {
            DefaultStyleKey = typeof (RibbonPanel);
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}