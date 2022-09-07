using System.Windows;
using System.Windows.Interactivity;

namespace ArcFMSilverlight
{
    public class MaintainObservedElementWidthBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ObservedElementProperty =
            DependencyProperty.Register("ObservedElement", typeof(FrameworkElement),
                                        typeof(MaintainObservedElementWidthBehavior),
                                        new PropertyMetadata(null, ObservedElementChanged));

        private static void ObservedElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = e.NewValue as FrameworkElement;
            if (element == null) return;
            var behavior = (MaintainObservedElementWidthBehavior)d;
            behavior.WatchSize(element);
        }

        public void WatchSize(FrameworkElement observedElement)
        {
            if (observedElement != null && AssociatedObject != null)
            {
                observedElement.SizeChanged += ObservedElementSizeChanged;
                AssociatedObject.Width = observedElement.ActualWidth;
            }
        }

        public FrameworkElement ObservedElement
        {
            get { return (FrameworkElement)GetValue(ObservedElementProperty); }
            set
            {
                SetValue(ObservedElementProperty, value);
            }
        }

        void ObservedElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Width = e.NewSize.Width;
            }
        }
    }
}