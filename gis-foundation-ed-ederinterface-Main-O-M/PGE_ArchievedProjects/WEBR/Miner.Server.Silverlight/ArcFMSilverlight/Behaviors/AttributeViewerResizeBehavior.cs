using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace ArcFMSilverlight
{
    public class AttributeViewerResizeBehavior : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty ResizeElementNameProperty =
            DependencyProperty.Register("ResizeElementName", typeof (string), typeof (AttributeViewerResizeBehavior),
                                        new PropertyMetadata(string.Empty, ResizeElementNameChanged));

        public static readonly DependencyProperty MinimumSizeProperty =
            DependencyProperty.Register("MinimumSize", typeof (double), typeof (AttributeViewerResizeBehavior),
                                        new PropertyMetadata(0d));

        public static readonly DependencyProperty SnapSizeProperty =
            DependencyProperty.Register("SnapSize", typeof (double), typeof (AttributeViewerResizeBehavior),
                                        new PropertyMetadata(double.NaN));

        public static readonly DependencyProperty ResizeButtonProperty =
            DependencyProperty.Register("ResizeButton", typeof (Button), typeof (AttributeViewerResizeBehavior),
                                        new PropertyMetadata(null, ResizeButtonChanged));

        private static void ResizeButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as AttributeViewerResizeBehavior;
            if (behavior == null) return;
            if (e.OldValue != null)
            {
                ((Button)e.OldValue).Click -= behavior.ResizeClick;
            }
            if(e.NewValue != null)
            {
                ((Button)e.NewValue).Click += behavior.ResizeClick;
            }
        }

        private void ResizeClick(object sender, RoutedEventArgs e)
        {
            if (ResizeElement == null)
            {
                ResizeElement = _dragElement.FindName(ResizeElementName) as FrameworkElement;
            }
            if (ResizeElement == null) return;
            if (ResizeElement.Height == MinimumSize && !double.IsNaN(SnapSize))
            {
                ResizeElement.Height = SnapSize;
            }
            else
            {
                ResizeElement.Height = MinimumSize;
            }
        }


        /// <summary>
        /// The AssociatedObject wich will be dragged for the resizing.
        /// Reference wil be kept in this property so events can be descriped on the detach.
        /// </summary>
        private FrameworkElement _dragElement;

        private double _initialHeight;

        /// <summary>
        /// Point to remember the mouse movement change.
        /// </summary>
        private Point _initialResizePoint;

        private bool _isDoubleClick;

        private bool _isEnabled = true;

        /// <summary>
        /// Flag to see if resizing is in progress.
        /// </summary>
        private bool _isResizing;

        private Timer _timer;

        [Category("Common Properties"),
         Description("A button to resize the editor.")]
        public Button ResizeButton
        {
            get { return (Button)GetValue(ResizeButtonProperty); }
            set { SetValue(ResizeButtonProperty, value); }
        }

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
                if (_dragElement == null)
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

            _dragElement = AssociatedObject;
            _dragElement.Cursor = Cursors.SizeNS;
            _dragElement.MouseLeftButtonDown += DragElementMouseLeftButtonDown;
            _dragElement.MouseLeftButtonUp += DragElementMouseLeftButtonUp;
            _dragElement.MouseMove += DragElementMouseMove;
            _dragElement.MouseLeave += DragElementMouseLeave;
        }

        /// <summary>
        /// Event when the behavior is detached from the element.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _dragElement.MouseLeftButtonDown -= DragElementMouseLeftButtonDown;
            _dragElement.MouseLeftButtonUp -= DragElementMouseLeftButtonUp;
            _dragElement.MouseMove -= DragElementMouseMove;
            _dragElement.MouseLeave -= DragElementMouseLeave;
        }

        /// <summary>
        /// Event when the selected resize elementname changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ResizeElementNameChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var behavior = sender as AttributeViewerResizeBehavior;
            if (behavior != null && behavior._dragElement != null)
            {
                behavior.ResizeElement = behavior._dragElement.FindName(behavior.ResizeElementName) as FrameworkElement;
            }
        }

        private void DoubleClickTimerElapsed(object state)
        {
            _isDoubleClick = false;
        }

        /// <summary>
        /// Event when the left mouse button is down on the dragging element.
        /// Starts resizing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ResizeElement == null)
                ResizeElement = _dragElement.FindName(ResizeElementName) as FrameworkElement;

            if (!CanResize) return;

            // Capture the mouse
            ((FrameworkElement) sender).CaptureMouse();
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
                return;
            }
            _isDoubleClick = true;
            _timer = new Timer(DoubleClickTimerElapsed);
            _timer.Change(new TimeSpan(0, 0, 0, 0, 500), new TimeSpan(0, 0, 0, 0, -1));

            // Store the start position
            if (ResizeElement != null)
            {
                _initialResizePoint = e.GetPosition(null);
                _initialHeight = (!double.IsNaN(ResizeElement.Height)
                                      ? ResizeElement.Height
                                      : ResizeElement.ActualHeight);
            }

            // Set resizing to true
            _isResizing = true;
        }

        /// <summary>
        /// Event when the left mouse button is up on the dragging element.
        /// Stops resizing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragElementMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isResizing) return;
            // Release the mouse
            ((FrameworkElement) sender).ReleaseMouseCapture();

            // Set resizing to false
            _isResizing = false;
        }

        /// <summary>
        /// Event when the mouse moves on the dragging element.
        /// Calculates the resizing when in dragging mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragElementMouseMove(object sender, MouseEventArgs e)
        {
            if (!CanResize || !_isResizing) return;

            Resize(e.GetPosition(null));
        }

        /// <summary>
        /// Event when the mouse leaves the draggin element.
        /// Stops dragging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DragElementMouseLeave(object sender, MouseEventArgs e)
        {
            //_IsResizing = false;
        }

        /// <summary>
        /// Calculates the distance between the two points and resizes the resizeelement.
        /// </summary>
        /// <param name="mousePosition">Point referenced to element.</param>
        private void Resize(Point mousePosition)
        {
            if (ResizeElement == null)
                return;

            double deltaY = mousePosition.Y - _initialResizePoint.Y;
            double newHeight = _initialHeight - deltaY;
            if (!double.IsNaN(SnapSize) && newHeight < SnapSize && newHeight > MinimumSize)
            {
                newHeight = SnapSize;
            }
            if (newHeight < MinimumSize)
            {
                newHeight = MinimumSize;
            }
            ResizeElement.Height = newHeight;
        }
    }
}