using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ArcFMSilverlight
{
    public class MyAutoCompleteBox : AutoCompleteBox
    {
        public static readonly DependencyProperty HandleKeyEventsProperty = DependencyProperty.Register(
            "HandleKeyEvents",
            typeof(bool),
            typeof(MyAutoCompleteBox),
            new PropertyMetadata(true));

        public bool HandleKeyEvents
        {
            get { return (bool)GetValue(HandleKeyEventsProperty); }
            set { SetValue(HandleKeyEventsProperty, value); }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.HandleKeyEvents || e.Key != Key.Enter)
            {
                base.OnKeyDown(e);
            }
        }
    }
}
