using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ArcFMSilverlight
{
    public class ExpandElementOnMouseOverBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty LayoutPanelProperty =
            DependencyProperty.Register("LayoutPanel", typeof (FrameworkElement),
                                        typeof (ExpandElementOnMouseOverBehavior),
                                        new PropertyMetadata(null, LayoutPanelChanged));

        public static readonly DependencyProperty ShadowElementProperty =
            DependencyProperty.Register("ShadowElement", typeof (FrameworkElement),
                                        typeof (ExpandElementOnMouseOverBehavior),
                                        new PropertyMetadata(null, ShadowElementChanged));

        public static readonly DependencyProperty LockToggleButtonProperty =
            DependencyProperty.Register("LockToggleButton", typeof (ToggleButton),
                                        typeof (ExpandElementOnMouseOverBehavior),
                                        new PropertyMetadata(null, LockToggleButtonChanged));

        public static readonly DependencyProperty MinimumSizeProperty =
            DependencyProperty.Register("MinimumSize", typeof (double), typeof (ExpandElementOnMouseOverBehavior),
                                        new PropertyMetadata(0d));

        private FrameworkElement _attachedElement;
        private bool _isEnabled = true;
        private FrameworkElement _layoutPanel;
        private ToggleButton _lockToggleButton;
        private int _popupCount;

        public ToggleButton LockToggleButton
        {
            get { return _lockToggleButton; }
            set
            {
                if (_lockToggleButton != null)
                {
                    _lockToggleButton.Checked -= LockToggleButtonChecked;
                    _lockToggleButton.Unchecked -= LockToggleButtonUnchecked;
                }
                _lockToggleButton = value;
                if (_lockToggleButton == null) return;
                _lockToggleButton.Checked += LockToggleButtonChecked;
                _lockToggleButton.Unchecked += LockToggleButtonUnchecked;
            }
        }

        /// <summary>
        /// Gets or Sets the value whether this behavior is active.
        /// </summary>
        [Category("Common Properties"),
         Description("Determines if the target element may be resized."),
         EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(true)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        /// <summary>
        /// The elementname which has to resize.
        /// </summary>
        [Category("Common Properties"),
         Description("The framework element holding the control that will be resized to hold the control when locked."),
         CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public FrameworkElement LayoutPanel
        {
            get { return (FrameworkElement) GetValue(LayoutPanelProperty); }
            set { SetValue(LayoutPanelProperty, value); }
        }

        public FrameworkElement ShadowElement
        {
            get { return (FrameworkElement) GetValue(ShadowElementProperty); }
            set { SetValue(ShadowElementProperty, value); }
        }

        [Category("Common Properties"),
         Description("The minimum size of the element to be resized."),
         EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(0d)]
        public double MinimumSize
        {
            get { return (double) GetValue(MinimumSizeProperty); }
            set { SetValue(MinimumSizeProperty, value); }
        }

        private static void ShadowElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ExpandElementOnMouseOverBehavior) d;
            if (!behavior.IsEnabled)
            {
                behavior.ShadowElement.Opacity = 0d;
            }
        }

        private static void LayoutPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ExpandElementOnMouseOverBehavior) d;
            behavior.SetLayoutPanel(e.NewValue as FrameworkElement);
        }

        private void SetLayoutPanel(FrameworkElement layoutPanel)
        {
            _layoutPanel = layoutPanel;
            if (_layoutPanel != null && !IsEnabled)
            {
                _layoutPanel.Height = MinimumSize;
            }
        }

        private static void LockToggleButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            object old = e.OldValue;
            var behavior = (ExpandElementOnMouseOverBehavior) d;
            if (old != null)
            {
                ((ToggleButton) old).Checked -= behavior.LockToggleButtonChecked;
                ((ToggleButton) old).Unchecked -= behavior.LockToggleButtonUnchecked;
            }
            object newToggle = e.NewValue;
            if (newToggle == null) return;
            ((ToggleButton) newToggle).Checked += behavior.LockToggleButtonChecked;
            ((ToggleButton) newToggle).Unchecked += behavior.LockToggleButtonUnchecked;
            ((ToggleButton) newToggle).IsChecked = behavior.IsEnabled;
        }

        private void LockToggleButtonUnchecked(object sender, RoutedEventArgs e)
        {
            IsEnabled = true;
            if (LayoutPanel == null) return;
            LayoutPanel.Height = MinimumSize;
            if (ShadowElement != null)
            {
                ShadowElement.Opacity = 1d;
            }
        }

        private void LockToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            if (LayoutPanel == null || _attachedElement == null) return;
            MaxmizeElement(_attachedElement);
            LayoutPanel.Height = _attachedElement.DesiredSize.Height;
            if (ShadowElement != null)
            {
                ShadowElement.Opacity = 0d;
            }
        }

        /// <summary>
        /// Event when the behavior is attached to a element.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            _attachedElement = AssociatedObject;
            _attachedElement.MouseEnter += AttachedElementMouseEnter;
            _attachedElement.MouseLeave += AttachedElementMouseLeave;
            _attachedElement.SizeChanged += AttachedElementSizeChanged;
        }

        private void AttachedElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (IsEnabled) return;
            if (LayoutPanel != null)
            {
                LayoutPanel.Height = e.NewSize.Height;
            }
        }

        private void AttachedElementMouseLeave(object sender, MouseEventArgs e)
        {
            if (_attachedElement == null || !IsEnabled) return;
            if (VisualTreeHelper.GetOpenPopups().Count() > _popupCount)
            {
                return;
            }
            if (!IsFocusOnPopup())
            {
                MinimizeElement(_attachedElement);
            }
        }

        private void MinimizeElement(FrameworkElement element)
        {
            if (ShadowElement != null)
            {
                ShadowElement.Opacity = 0d;
            }
            var easeFunction = new CubicEase {EasingMode = EasingMode.EaseInOut};
            var animation = new DoubleAnimation
                                {
                                    From = element.RenderSize.Height,
                                    To = MinimumSize,
                                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 150)),
                                    EasingFunction = easeFunction
                                };
            var storyboard = new Storyboard {Duration = new Duration(new TimeSpan(0, 0, 0, 0, 150))};
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(storyboard, element);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath("Height"));
            storyboard.Begin();
        }

        private void MaxmizeElement(FrameworkElement element)
        {
            element.Height = double.NaN;
            element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            element.InvalidateArrange();
            element.InvalidateMeasure();

            if (IsEnabled && ShadowElement != null)
            {
                ShadowElement.Opacity = 1d;
            }

            var easeFunction = new CubicEase {EasingMode = EasingMode.EaseInOut};
            var animation = new DoubleAnimation
                                {
                                    From = element.RenderSize.Height,
                                    To = element.DesiredSize.Height,
                                    Duration = new Duration(new TimeSpan(0, 0, 0, 0, 150)),
                                    EasingFunction = easeFunction
                                };
            var storyboard = new Storyboard {Duration = new Duration(new TimeSpan(0, 0, 0, 0, 150))};
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(storyboard, element);
            Storyboard.SetTargetProperty(storyboard, new PropertyPath("Height"));
            storyboard.Completed += (s, e) => _attachedElement.Height = double.NaN;
            storyboard.Begin();
        }

        private static bool IsFocusOnPopup()
        {
            object focused = FocusManager.GetFocusedElement();
            return IsInPopup(focused as DependencyObject);
        }

        private static bool IsInPopup(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                return false;
            }
            DependencyObject parent = VisualTreeHelper.GetParent(dependencyObject);
            if (parent is Popup) return true;
            if (parent == null && dependencyObject is FrameworkElement)
            {
                return ((FrameworkElement) dependencyObject).Parent is Popup;
            }
            return IsInPopup(parent);
        }

        private void AttachedElementMouseEnter(object sender, MouseEventArgs e)
        {
            if (_attachedElement == null) return;
            MaxmizeElement(_attachedElement);
            _popupCount = VisualTreeHelper.GetOpenPopups().Count();
        }

        /// <summary>
        /// Event when the behavior is detached from the element.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _attachedElement.MouseEnter -= AttachedElementMouseEnter;
            _attachedElement.MouseLeave -= AttachedElementMouseLeave;
            _attachedElement.SizeChanged -= AttachedElementSizeChanged;
        }
    }
}