using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;

#if SILVERLIGHT
namespace Miner.Server.Client.Toolkit
#elif WPF
namespace Miner.Mobile.Client.Toolkit
#endif
{
    /// <summary>
    /// Implements custom cursors.
    /// </summary>
    /// <exclude/>
    public class CursorSet
    {
        #region private declarations

        private static Popup _popup;
        private static Canvas _adornerLayer;
        private static Dictionary<FrameworkElement, ContentControl> _activeElements;
        private static MouseEventArgs _currentPosition;

        #endregion private declarations

        #region ctor

        /// <summary>
        /// Constructor
        /// </summary>
        static CursorSet()
        {
            _activeElements = new Dictionary<FrameworkElement, ContentControl>();
#if SILVERLIGHT
            Application.Current.Host.Content.Resized += OnContentResized;
#elif WPF
            Application.Current.MainWindow.SizeChanged += OnContentResized;
#endif
        }

        #endregion ctor

        #region Custom IDs implementation

        /// <summary>
        /// The ID property of the cursor to be displayed.
        /// </summary>
        public static readonly DependencyProperty IDProperty = DependencyProperty.RegisterAttached("ID", typeof(string), typeof(CursorSet), new PropertyMetadata(new PropertyChangedCallback(OnIDPropertyChanged)));
        
        /// <summary>
        /// Gets the image path of the current cursor
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetID(DependencyObject d)
        {
            if (d == null) return string.Empty;

            return (string)d.GetValue(IDProperty);
        }

        /// <summary>
        /// Sets the current cursor
        /// </summary>
        /// <param name="d">the current control for the cursor</param>
        /// <param name="id">the path to the image used by the cursor</param>
        public static void SetID(DependencyObject d, string id)
        {
            if (d == null) return;

            d.SetValue(IDProperty, id);
        }

        #endregion Custom IDs implementation

        #region events
        private static void OnIDPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = d as FrameworkElement;
            string id = e.NewValue as string;

            if (IsValidID(id))
            {
                Cursor cursor;

                if (IsSystemType(id, out cursor))
                {
                    element.Cursor = cursor;
                    RemoveMouseEventHandlers(element);

                    if (_activeElements.ContainsKey(element))
                        _activeElements.Remove(element);
                }
                else
                {
                    ContentControl control = CreateControl(id);
                    if (control == null)
                    {
                        SetDefaultCursor(element);
                    }
                    else
                    {
                        if (_activeElements.ContainsKey(element))
                            _activeElements[element] = control;
                        else
                        {
                            _activeElements.Add(element, control);
                            AddMouseEventHandlers(element);
                        }
                    }
                }
            }
            else
            {
                SetDefaultCursor(element);
            }

            if (_adornerLayer != null && _adornerLayer.Children.Count > 0)
            {
                _adornerLayer.Children.Clear();

                try
                {
                    OnElementMouseEnter(element, _currentPosition);
                }
                catch
                {
                }
            }
        }
        #endregion events

        #region private static methods

        private static void SetDefaultCursor(FrameworkElement element)
        {
            if (element == null) return;

            element.Cursor = Cursors.Arrow;
            RemoveMouseEventHandlers(element);

            if (_activeElements.ContainsKey(element))
            {
                _activeElements.Remove(element);
            }
        }

        private static ContentControl CreateControl(string id)
        {
            if (string.IsNullOrEmpty(id)) return new ContentControl();

            Image image = new Image()
            {
                Margin = new Thickness(-12, -10, 0, 0),
                Source = new BitmapImage(new Uri(id, UriKind.RelativeOrAbsolute))
            };

            Canvas canvas = new Canvas();
            canvas.Children.Add(image);

            return new ContentControl { Content = canvas };
        }

        private static bool IsValidID(string id)
        {
            Cursor cursor;

            if (IsSystemType(id, out cursor))
                return true;

            return !(string.IsNullOrEmpty(id));
        }

        private static bool IsSystemType(string id, out Cursor cursor)
        {
            cursor = null;

            if (string.IsNullOrEmpty(id))
                return true;

            switch (id)
            {
                case "Arrow":
                    cursor = Cursors.Arrow;
                    return true;
                case "Hand":
                    cursor = Cursors.Hand;
                    return true;
                case "IBeam":
                    cursor = Cursors.IBeam;
                    return true;
                case "None":
                    cursor = Cursors.None;
                    return true;
                case "SizeNS":
                    cursor = Cursors.SizeNS;
                    return true;
                case "SizeWE":
                    cursor = Cursors.SizeWE;
                    return true;
                case "Wait":
                    cursor = Cursors.Wait;
                    return true;
#if SILVERLIGHT
                case "Eraser":
                    cursor = Cursors.Eraser;
                    return true;
                case "Stylus":
                    cursor = Cursors.Stylus;
                    return true;
#endif
            }

            return false;
        }

        private static void OnContentResized(object sender, EventArgs e)
        {
            if (_adornerLayer != null)
            {
#if SILVERLIGHT
                _adornerLayer.Width = Application.Current.Host.Content.ActualWidth;
                _adornerLayer.Height = Application.Current.Host.Content.ActualHeight;
#elif WPF
                _adornerLayer.Width = Application.Current.MainWindow.ActualWidth;
                _adornerLayer.Height = Application.Current.MainWindow.ActualHeight;
#endif
            }
        }

        private static void EnsurePopup()
        {
            if (_popup == null || _adornerLayer == null)
            {
                _adornerLayer = new Canvas()
                {
                    IsHitTestVisible = false,
#if SILVERLIGHT
                Width = Application.Current.Host.Content.ActualWidth,
                Height = Application.Current.Host.Content.ActualHeight
#elif WPF
                Width = Application.Current.MainWindow.ActualWidth,
                Height = Application.Current.MainWindow.ActualHeight
#endif
                };

                _popup = new Popup
                {
                    IsHitTestVisible = false,
                    Child = _adornerLayer
                };
            }
        }

        private static void AddMouseEventHandlers(FrameworkElement element)
        {
            element.MouseEnter += OnElementMouseEnter;
            element.MouseMove += OnElementMouseMove;
            element.MouseLeave += OnElementMouseLeave;
        }

        private static void RemoveMouseEventHandlers(FrameworkElement element)
        {
            element.MouseEnter -= OnElementMouseEnter;
            element.MouseMove -= OnElementMouseMove;
            element.MouseLeave -= OnElementMouseLeave;
        }

        private static void OnElementMouseEnter(object sender, MouseEventArgs e)
        {
            EnsurePopup();

            FrameworkElement element = sender as FrameworkElement;
            ContentControl control = _activeElements[element];

            element.Cursor = Cursors.None;
            if (_adornerLayer.Children.Contains(control) == false)
            {
                _adornerLayer.Children.Add(control);
            }

            Point p = e.GetPosition(null);
            Canvas.SetTop(control, p.Y);
            Canvas.SetLeft(control, p.X);

            _popup.IsOpen = true;
        }

        private static void OnElementMouseMove(object sender, MouseEventArgs e)
        {
            _currentPosition = e;

            FrameworkElement element = sender as FrameworkElement;
            ContentControl control = _activeElements[element];

            Point p = e.GetPosition(null);
            Canvas.SetTop(control, p.Y);
            Canvas.SetLeft(control, p.X);
        }

        private static void OnElementMouseLeave(object sender, MouseEventArgs e)
        {
            if (_adornerLayer == null) return;

            FrameworkElement element = sender as FrameworkElement;
            ContentControl control = _activeElements[element];

            element.Cursor = null;
            _adornerLayer.Children.Remove(control);

            _popup.IsOpen = false;
        }

        #endregion private static methods
    }
}
