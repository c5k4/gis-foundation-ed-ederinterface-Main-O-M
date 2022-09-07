using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ArcFMSilverlight
{
    public class ExpandOnDoubleClickBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ResizeElementNameProperty =
            DependencyProperty.Register("ResizeElementName", typeof (string), typeof (ExpandOnDoubleClickBehavior),
                                        new PropertyMetadata(string.Empty, ResizeElementNameChanged));

        public static readonly DependencyProperty MinimumSizeProperty =
            DependencyProperty.Register("MinimumSize", typeof (double), typeof (ExpandOnDoubleClickBehavior),
                                        new PropertyMetadata(0d));

        public static readonly DependencyProperty ListeningAreaHeightProperty =
           DependencyProperty.Register("ListeningAreaHeight", typeof(double), typeof(ExpandOnDoubleClickBehavior),
                                       new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty SnapSizeProperty =
            DependencyProperty.Register("SnapSize", typeof (double), typeof (ExpandOnDoubleClickBehavior),
                                        new PropertyMetadata(double.NaN));


        /// <summary>
        /// The AssociatedObject wich will be dragged for the resizing.
        /// Reference wil be kept in this property so events can be descriped on the detach.
        /// </summary>
        private FrameworkElement _attachedElement;

        private bool _isDoubleClick;

        private bool _isEnabled = true;

        private Timer _timer;

        /// <summary>
        /// The elementname which has to resize.
        /// </summary>
        [Category("Common Properties"),
         Description("The targeted element that must be resized."),
         CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ResizeElementName
        {
            get { return (string) GetValue(ResizeElementNameProperty); }
            set { SetValue(ResizeElementNameProperty, value); }
        }

        /// <summary>
        /// Gets or Sets the value whether this behavior is active.
        /// </summary>
        [Category("Common Properties"),
         Description("The targeted element that must be resized."),
         EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(true)]
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        [Category("Common Properties"),
         Description("The minimum size of the element to be resized."),
         EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(0d)]
        public double MinimumSize
        {
            get { return (double) GetValue(MinimumSizeProperty); }
            set { SetValue(MinimumSizeProperty, value); }
        }

        [Category("Common Properties"),
         Description("A size to snap to changing the size of the element to be resized."),
         EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(double.NaN)]
        public double SnapSize
        {
            get { return (double) GetValue(SnapSizeProperty); }
            set { SetValue(SnapSizeProperty, value); }
        }

        [Category("Common Properties"),
         Description("Height of the area listening for a double-click."),
         EditorBrowsable(EditorBrowsableState.Advanced), DefaultValue(double.NaN)]
        public double ListeningAreaHeight
        {
            get { return (double)GetValue(ListeningAreaHeightProperty); }
            set { SetValue(ListeningAreaHeightProperty, value); }
        }

        /// <summary>
        /// The element which has to resize.
        /// </summary>
        private FrameworkElement ResizeElement { get; set; }

        /// <summary>
        /// Read-Only. Checks if all required properties are available for the resize ability.
        /// </summary>
        private bool CanResize
        {
            get
            {
                if (_attachedElement == null)
                    return false;
                return ResizeElementName != null;
            }
        }
        
        /// <summary>
        /// Event when the behavior is attached to a element.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (!IsEnabled) return;
            _attachedElement = AssociatedObject;
            _attachedElement.MouseLeftButtonUp += AttachedElementMouseLeftButtonUp;
        }

        /// <summary>
        /// Event when the behavior is detached from the element.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _attachedElement.MouseLeftButtonUp -= AttachedElementMouseLeftButtonUp;
        }

        /// <summary>
        /// Event when the selected resize elementname changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ResizeElementNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = sender as ExpandOnDoubleClickBehavior;
            if (behavior != null && behavior._attachedElement != null)
            {
                behavior.ResizeElement = behavior._attachedElement.FindName(behavior.ResizeElementName) as FrameworkElement;
            }
        }

        private void DoubleClickTimerElapsed(object state)
        {
            _isDoubleClick = false;
        }

        /// <summary>
        /// Event when the left mouse button is up on the dragging element.
        /// Stops resizing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttachedElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;

            if (ResizeElement == null)
                ResizeElement = _attachedElement.FindName(ResizeElementName) as FrameworkElement;

            if (!CanResize)
            {
                return;
            }

            if (!double.IsNaN(ListeningAreaHeight))
            {
                var point = e.GetPosition(_attachedElement);
                if(point.Y > ListeningAreaHeight)
                {
                    return;
                }
            }

            if (_isDoubleClick && ResizeElement != null)
            {
                if (ResizeElement.Height == MinimumSize && !double.IsNaN(SnapSize))
                {
                    ResizeElement.Height = SnapSize;
                }
                else
                {
                    ResizeElement.Height = MinimumSize;
                }
                _timer.Dispose();
                _isDoubleClick = false;
                e.Handled = true;
                return;
            }
            _isDoubleClick = true;
            _timer = new Timer(DoubleClickTimerElapsed);
            _timer.Change(new TimeSpan(0, 0, 0, 0, 400), new TimeSpan(0, 0, 0, 0, -1));
        }
    }
}
