using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Toolkit;

using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;

using NLog;
using System.Security;
using ArcFM.Silverlight.PGE.CustomTools;

namespace ArcFMSilverlight
{
    /// <summary>
    /// Identifies the coordinate information of a click on the map.
    /// </summary>
    [TemplatePart(Name = ElementCoordinateInformationTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = ElementCoordinateCopyToClipboardButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementLayoutGrid, Type = typeof(Grid))]
    [TemplatePart(Name = ElementFindPointButton, Type = typeof(ToggleButton))]
    public class CoordinatesControl : Control, IActiveControl
    {
        #region private declarations

        private const int WgsWKID = 4326;

        private const string ElementCoordinateInformationTextBox = "PART_CoordinateInformationTextBox";
        private const string ElementCoordinateCopyToClipboardButton = "PART_CoordinateCopyToClipboard";
        private const string ElementLayoutGrid = "PART_ControlLayout";
        private const string ElementFindPointButton = "PART_FindPointButton";
        private const string LatLongTemplate = "LatlonTemplate";
        private const string XYTemplate = "XYTemplate";

        private const string CursorUri = @"/Images/coordinfo_cursor.png";

        private TextBox _coordinateInfoTextBox;
        private Grid _layoutGrid;
        private ToggleButton _findPointButton;

        private readonly Dictionary<string, CoordinatePairs> _dictionaryCoordinatePoints;
        private int _windowCount;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        #endregion private declarations

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CoordinatesControl()
        {
            DefaultStyleKey = typeof(CoordinatesControl);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _dictionaryCoordinatePoints = new Dictionary<string, CoordinatePairs>();            
        }

        #endregion Constructor

        #region Dependency Properties

        /// <summary>
        /// Gets the identifier for the <see cref="Map"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(CoordinatesControl),
            null);

        /// <summary>
        /// Gets the identifier for the <see cref="GeometryService"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register(
            "GeometryService",
            typeof(string),
            typeof(CoordinatesControl),
            null);

        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive",
            typeof(bool),
            typeof(CoordinatesControl),
            new PropertyMetadata(OnIsActiveChanged));

        /// <summary>
        /// Gets the identifier for the <see cref="IsWgs"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsWgsProperty = DependencyProperty.Register(
            "IsWgs",
            typeof(bool),
            typeof(CoordinatesControl),
            new PropertyMetadata(false, OnWgsChanged));

        /// <summary>
        /// Gets the identifier for the <see cref="LocationTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LocationTemplateProperty = DependencyProperty.Register(
          "LocationTemplate",
          typeof(DataTemplate),
          typeof(CoordinatesControl),
          new PropertyMetadata(CreateLocationTemplate()));

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        [Category("Coordinates")]
        [Description("Gets or sets the Map.")]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Geometry Service URL.
        /// </summary>
        [Category("Coordinates")]
        [Description("Gets or sets the Geometry Service URL.")]
        public string GeometryService
        {
            get { return (string)GetValue(GeometryServiceProperty); }
            set { SetValue(GeometryServiceProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        [Category("Coordinates")]
        [Description("Gets or sets the Map.")]
        public bool IsWgs
        {
            get { return (bool)GetValue(IsWgsProperty); }
            set { SetValue(IsWgsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template for the window that displays the location of the point.
        /// </summary>
        [Category("Coordinates")]
        [Description("Gets or sets the template for the window that displays the location of the point.")]
        public DataTemplate LocationTemplate
        {
            get { return (DataTemplate)GetValue(LocationTemplateProperty); }
            set { SetValue(LocationTemplateProperty, value); }
        }

        #endregion Dependency Properties

        #region IActiveControl Members

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (CoordinatesControl)d;
            var isActive = (bool)e.NewValue;
            if (isActive == false)
            {
                if (control.Map == null) return;
                control.Map.MouseClick -= control.MapControl_MouseClick;
                string currentMapCursor = CursorSet.GetID(control.Map);
                if (currentMapCursor.Contains("coordinfo_cursor.png") == true)
                {
                    CursorSet.SetID(control.Map, "Arrow");
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;

                control.Map.MouseClick += control.MapControl_MouseClick;
                CursorSet.SetID(control.Map, CursorUri);
            }
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }

        #endregion IActiveControl Members

        #region Public Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Map == null) return;

            if ((Map.SpatialReference != null) && (Map.SpatialReference.WKID == WgsWKID))
            {
                IsWgs = true;
            }

            Button copyButton = GetTemplateChild(ElementCoordinateCopyToClipboardButton) as Button ?? new Button();
            copyButton.Click += CoordinateCopyToClipboardClicked;

            _coordinateInfoTextBox = GetTemplateChild(ElementCoordinateInformationTextBox) as TextBox ?? new TextBox();
            _coordinateInfoTextBox.AcceptsReturn = true;
            _coordinateInfoTextBox.KeyDown += CoordinateInfoTextBoxKeyDown;

            _layoutGrid = GetTemplateChild(ElementLayoutGrid) as Grid;

            _findPointButton = GetTemplateChild(ElementFindPointButton) as ToggleButton;
            _findPointButton.Click += new RoutedEventHandler(_findPointButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                IsEnabled = false;
                _coordinateInfoTextBox.Text = "No Geometry Service";
            }

            IsWgs = true;
        }

        #endregion Public Overrides

        #region private events

        private static void OnWgsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoordinatesControl control = (CoordinatesControl)d;
            bool isWgs = (bool)e.NewValue;

            control.ChangeFindPointButtonImage();
            control.ChangeCoordinateInfoTooltip();

            if (isWgs)
            {
                control.CalculateWGS();
            }
            else
            {
                control.DisplayMapCoordinates();
            }
        }

        private void CoordinateInfoTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            _coordinateInfoTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            if (e.Key != Key.Enter) return;

            var coordinateText = _coordinateInfoTextBox.Text.Trim();
            coordinateText = CleanCoordinateLabels(coordinateText);

            var decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var seperators = (decimalSeparator == ".") ?
                new[] { ',', ' ', '\n', '\r'} : 
                new[] { ' ', '\n', '\r' };
            var coords = coordinateText.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            if (coords.Length < 2) return;

            e.Handled = true;

            if (IsWgs)
            {
                //Reversing coords array to get the input as Lat,Long instead of Log,Lat
                Array.Reverse(coords);
                ValidateAndPan(WgsWKID, coords);
            }
            else
            {
                ValidateAndPan(Map.SpatialReference.WKID, coords);
            }
        }

        private void MapControl_MouseClick(object sender, Map.MouseEventArgs e)
        {
            if (e == null) return;
            if (e.MapPoint == null) return;

            InfoWindow infoWindow = CreateInfoWindow(new CoordinatePairs { MapCoordinatePoint = e.MapPoint });
            if (IsWgs)
            {
                CalculatePoint(e.MapPoint, infoWindow.Name);
            }
            else
            {
                infoWindow.Content = e.MapPoint;
            }
        }

        private void InfoWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InfoWindow infoWindow = sender as InfoWindow;
            if (infoWindow != null)
            {
                e.Handled = true;
                Border border = VisualTreeHelper.GetChild(Map, 0) as Border;
                (border.Child as Grid).Children.Remove(infoWindow);
                _dictionaryCoordinatePoints.Remove(infoWindow.Name);
            }
        }

        private void CoordinateCopyToClipboardClicked(object sender, RoutedEventArgs e)
        {
            if ((_dictionaryCoordinatePoints == null) || (_dictionaryCoordinatePoints.Count <= 0)) return;

            string clipboardText = string.Empty;
            try
            {
                Clipboard.SetText("");
            }
            catch (SecurityException se)
            {
                //ignore we will hit this again later
                //MessageBox.Show("You must give the application permission to access your clipboard.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }

            if (IsWgs)
            {
                clipboardText = "Lat/Long" + Environment.NewLine;
            }
            else
            {
                clipboardText = "Map" + Environment.NewLine;
            }

            foreach (KeyValuePair<string, CoordinatePairs> kvp in _dictionaryCoordinatePoints)
            {
                if (IsWgs)
                {
                    clipboardText = clipboardText + "Lat = " + kvp.Value.WGSPoint.Y + ", Long = " + kvp.Value.WGSPoint.X + Environment.NewLine;
                }
                else
                {
                    clipboardText = clipboardText + "X = " + kvp.Value.MapCoordinatePoint.X + ", Y = " + kvp.Value.MapCoordinatePoint.Y + Environment.NewLine;
                }
            }

            //sometimes it blows up for no apparent reason. Silverlight problem?
            try
            {
                Clipboard.SetText(clipboardText);
            }
            catch (SecurityException se)
            {
                
                MessageBox.Show("You must give the application permission to access your clipboard.");
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }

        private void GeometryServiceFailed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Coordinates tool geometry task failed! " + e.Error.Message);
            throw new Exception("CoordinatesControl: ProjectionError: " + e.Error.Message);
        }

        private void GeometryServiceProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var wgsPoint = (MapPoint)e.Results[0].Geometry;
            String infoWindowName = e.UserState as String;

            if (_dictionaryCoordinatePoints.ContainsKey(infoWindowName) == true)
            {
                _dictionaryCoordinatePoints[infoWindowName].WGSPoint = wgsPoint;
            }
            RefreshInfoWindow(wgsPoint, infoWindowName);
        }

        private void GeometryServiceNavigateProjectCompleted(object sender, GraphicsEventArgs e)
        {
            try
            {
                var point = (MapPoint)e.Results[0].Geometry;
                PanToPoint(point, e.UserState as MapPoint);
            }
            catch (Exception ex)
            {
                throw new Exception("CoordinatesControl: An error occurred. " + ex.Message);
            }
        }

        private void _findPointButton_Click(object sender, RoutedEventArgs e)
        {
            _coordinateInfoTextBox.SelectionStart = 0;
        }

        #endregion private events

        #region private methods

        private void ChangeFindPointButtonImage()
        {
            var imageURIString = IsWgs ? @"/Images/Maps-icon.png" : @"/Images/NavToPoint.png";
            if (imageURIString == null) return;

            Image wgsImage = new Image()
            {
                Source = new BitmapImage(new Uri(imageURIString, UriKind.Relative)),
                Stretch = Stretch.Fill,
                Height = 16,
                Width = 16
            };
            _findPointButton.Content = wgsImage;
        }

        private void ChangeCoordinateInfoTooltip()
        {
            string tooltipText = IsWgs ? @"Enter Latitude,Longitude to pan to location." : "Enter coordinates to pan to location. Click map to get coordinate information.";
            ToolTipService.SetToolTip(_coordinateInfoTextBox, tooltipText);

            tooltipText = IsWgs ? @"Navigate to Lat/Long" : @"Navigate to coordinates";
            ToolTipService.SetToolTip(_findPointButton, tooltipText);
        }

        private void RefreshInfoWindow(MapPoint mapPoint, string infoWindowName)
        {
            InfoWindow infoWindow = FindName(infoWindowName) as InfoWindow;
            if (infoWindow != null)
            {
                if (IsWgs)
                {
                    LatLongDisplayObj lld = new LatLongDisplayObj(mapPoint.Y, mapPoint.X);
                    infoWindow.ContentTemplate = GetDataTemplate();
                    infoWindow.Content = lld;
                    infoWindow.UpdateLayout();
                }
                else
                {
                    infoWindow.ContentTemplate = GetDataTemplate();
                    infoWindow.Content = mapPoint;
                    infoWindow.UpdateLayout();
                }                
            }
        }

        private void CalculateWGS()
        {
            if ((_dictionaryCoordinatePoints == null) || (_dictionaryCoordinatePoints.Count <= 0)) return;
            foreach (KeyValuePair<string, CoordinatePairs> kvp in _dictionaryCoordinatePoints)
            {
                if (kvp.Value.WGSPoint == null)
                {
                    CalculatePoint(kvp.Value.MapCoordinatePoint, kvp.Key);
                }
                else
                {
                    RefreshInfoWindow(kvp.Value.WGSPoint, kvp.Key);
                }
            }
        }

        private void DisplayMapCoordinates()
        {
            if (_dictionaryCoordinatePoints == null) return;

            foreach (KeyValuePair<string, CoordinatePairs> kvp in _dictionaryCoordinatePoints)
            {
                InfoWindow infoWindow = FindName(kvp.Key) as InfoWindow;
                if (infoWindow != null)
                {
                    infoWindow.ContentTemplate = GetDataTemplate();
                    infoWindow.Content = kvp.Value.MapCoordinatePoint;
                }
            }
        }

        private void CalculatePoint(MapPoint mapCoordinatePoint, string infoWindowName)
        {
            // If there is no WKID or WKT, we can't project.
            if ((mapCoordinatePoint.SpatialReference.WKID == 0) && (string.IsNullOrEmpty(mapCoordinatePoint.SpatialReference.WKT)))
            {
                _dictionaryCoordinatePoints[infoWindowName].WGSPoint = mapCoordinatePoint;
                return;
            }

            // Prepare a call to the geometry service.
            var geometryService = new GeometryService(GeometryService);

            geometryService.Failed += GeometryServiceFailed;
            geometryService.ProjectCompleted += GeometryServiceProjectCompleted;

            var graphic = new Graphic { Geometry = mapCoordinatePoint };
            var graphics = new List<Graphic> { graphic };

            geometryService.ProjectAsync(graphics, new SpatialReference(WgsWKID), infoWindowName);
        }

        /// <summary>
        /// Cleans any label text found on the passed in string.  Returns only possible coordinate values.
        /// </summary>
        /// <param name="coordinateText">the string to clean</param>
        /// <returns>possible coordinate values</returns>
        private static string CleanCoordinateLabels(string coordinateText)
        {
            // Clean labels
            coordinateText = coordinateText.Replace("x", string.Empty);
            coordinateText = coordinateText.Replace("y", string.Empty);
            coordinateText = coordinateText.Replace("X", string.Empty);
            coordinateText = coordinateText.Replace("Y", string.Empty);
            coordinateText = coordinateText.Replace(":", string.Empty);
            coordinateText = coordinateText.Replace("=", string.Empty);
            coordinateText = coordinateText.Replace("Northing", string.Empty);
            coordinateText = coordinateText.Replace("Easting", string.Empty);
            coordinateText = coordinateText.Replace("Latitude", string.Empty);
            coordinateText = coordinateText.Replace("Longitude", string.Empty);
            coordinateText = coordinateText.Replace("Lat", string.Empty);
            coordinateText = coordinateText.Replace("Lon", string.Empty);

            // Replace DMS symbols with characters for parsing
            coordinateText = coordinateText.Replace("°", "d");
            coordinateText = coordinateText.Replace("'", "m");
            coordinateText = coordinateText.Replace("\"", "s");

            return coordinateText;
        }

        /// <summary>
        /// Validates coordinate entries and pans to valid points.
        /// </summary>
        /// <param name="coordinateSystem">the selected coordinate system</param>
        /// <param name="coords">the coordinates</param>
        private void ValidateAndPan(int coordinateSystem, string[] coords)
        {
            // Activate if the tools isn't already.
            if (!IsActive)
            {
                IsActive = true;
            }
            const int x = 0;
            const int y = 1;

            if (coords.Length > 2)
            {
                _coordinateInfoTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                return;
            }

            if (Map == null) return;

            double xCoord;
            double yCoord;

            PanToPointType mode;

            if (!ValidInput(coordinateSystem, coords[x], coords[y], out mode))
            {
                _coordinateInfoTextBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
                return;
            }

            switch (mode)
            {
                case PanToPointType.DegreesMinutesSeconds:
                    xCoord = ConvertDmsToDecDeg(coords[x]);
                    yCoord = ConvertDmsToDecDeg(coords[y]);
                    break;
                case PanToPointType.DecimalDegrees:
                    xCoord = Convert.ToDouble(coords[x]);
                    yCoord = Convert.ToDouble(coords[y]);
                    break;
                case PanToPointType.EastingNorthing:
                    xCoord = Convert.ToDouble(coords[x]);
                    yCoord = Convert.ToDouble(coords[y]);
                    break;
                default:
                    throw new Exception("Invalid Coordinate Mode");
            }

            SpatialReference sp = new SpatialReference(coordinateSystem);

            if (coordinateSystem == 0 && !string.IsNullOrEmpty(Map.SpatialReference.WKT))
                sp = new SpatialReference(Map.SpatialReference.WKT);

            var point = new MapPoint(xCoord, yCoord, sp);

            var graphic = new Graphic
            {
                Geometry = point
            };

            if (coordinateSystem != 0 && coordinateSystem != Map.SpatialReference.WKID)
            {
                if ((Map.SpatialReference.WKID == 0) && (string.IsNullOrEmpty(Map.SpatialReference.WKT)))
                {
                    throw new Exception("Invalid or Undefined SpatialReference on the Map");
                }

                var graphicList = new List<Graphic> { graphic };

                var geometryService = new GeometryService(GeometryService);
                geometryService.Failed += GeometryServiceFailed;
                geometryService.ProjectCompleted += GeometryServiceNavigateProjectCompleted;
                geometryService.ProjectAsync(graphicList, Map.SpatialReference, point);
            }
            else
            {
                PanToPoint(point);
            }
        }

        /// <summary>
        /// Validates coordinate input based on the type of coordinate system.
        /// </summary>
        /// <param name="coordSys">the coordinate systems spatial reference WKID</param>
        /// <param name="xValue">the x axis value</param>
        /// <param name="yValue">the y axis value</param>
        /// <param name="mode">the type of values entered</param>
        /// <returns>true if valid, false if not</returns>
        private static bool ValidInput(int coordSys, string xValue, string yValue, out PanToPointType mode)
        {
            // Decide which rules to use in validation based on coordinate system.
            if (coordSys == WgsWKID)
            {
                if (xValue.Contains("d"))
                {
                    mode = PanToPointType.DegreesMinutesSeconds;

                    // DMS values
                    try
                    {
                        xValue = xValue.ToLower();
                        yValue = yValue.ToLower();

                        // Parse X the value
                        int dLoc = xValue.IndexOf("d");
                        int mLoc = xValue.IndexOf("m");
                        int sLoc = xValue.IndexOf("s");

                        int xDeg = Convert.ToInt32(xValue.Substring(0, dLoc));
                        int xMin = Convert.ToInt32(xValue.Substring(dLoc + 1, mLoc - dLoc - 1));
                        double xSec = Convert.ToDouble(xValue.Substring(mLoc + 1, sLoc - mLoc - 1));

                        // Parse the Y value
                        dLoc = yValue.IndexOf("d");
                        mLoc = yValue.IndexOf("m");
                        sLoc = yValue.IndexOf("s");

                        int yDeg = Convert.ToInt32(yValue.Substring(0, dLoc));
                        int yMin = Convert.ToInt32(yValue.Substring(dLoc + 1, mLoc - dLoc - 1));
                        double ySec = Convert.ToDouble(yValue.Substring(mLoc + 1, sLoc - mLoc - 1));

                        // X degrees can be between 0 and 180, Y degrees can be between 0 and 90
                        if ((xDeg < 0) || (yDeg < 0) || (xDeg > 180) || (yDeg > 90)) return false;

                        // Minutes and seconds can be between 0 and 60
                        if ((xMin < 0) || (yMin < 0) || (xMin > 60) || (yMin > 60)) return false;
                        if ((xSec < 0) || (ySec < 0) || (xSec > 60) || (ySec > 60)) return false;

                        // Can only end in North, South, East, or West
                        return ((xValue.EndsWith("n")) || (xValue.EndsWith("s")) || (xValue.EndsWith("e")) || (xValue.EndsWith("w")));
                    }
                    catch
                    {
                        return false;
                    }
                }

                mode = PanToPointType.DecimalDegrees;
                // Decimal Degree value
                try
                {
                    double x = Convert.ToDouble(xValue);
                    double y = Convert.ToDouble(yValue);

                    if ((x >= -180.0) && (x <= 180.0))
                    {
                        // Y can be +-90
                        return ((y >= -90.0) && (y <= 90.0));
                    }

                    return false;
                }
                catch
                {
                    // Probably failed convert to double.
                    return false;
                }
            }

            mode = PanToPointType.EastingNorthing;
            // Easting Northing value
            try
            {
                // Is it a double?
                Convert.ToDouble(xValue);
                Convert.ToDouble(yValue);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converts a degree minute second string to a decimal degree double
        /// </summary>
        /// <param name="dms">the degree minute second string to convert</param>
        /// <returns>the decimal degree</returns>
        private static double ConvertDmsToDecDeg(string dms)
        {
            try
            {
                // Get the positions of the separators.
                int dLoc = dms.IndexOf("d");
                int mLoc = dms.IndexOf("m");
                int sLoc = dms.IndexOf("s");

                // Parse out the values.
                double deg = Convert.ToDouble(dms.Substring(0, dLoc));
                double min = Convert.ToDouble(dms.Substring(dLoc + 1, mLoc - dLoc - 1));
                double sec = Convert.ToDouble(dms.Substring(mLoc + 1, sLoc - mLoc - 1));

                // Create the decimal degree value.
                double value = deg + (min / 60) + (sec / 3600);

                // Set positive od negative based on directional modifier.  West and South are negative, East and North are positive.
                if (dms.ToLower().EndsWith("w") || dms.ToLower().EndsWith("s")) value = (value * (-1));

                return value;

            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Pans the map to the give graphic.
        /// </summary>
        /// <param name="graphic"></param>
        private void PanToPoint(MapPoint point, MapPoint wgsPoint = null)
        {
            Map.PanTo(point);

            CoordinatePairs coordinatePairs = new CoordinatePairs();
            coordinatePairs.MapCoordinatePoint = point;
            coordinatePairs.WGSPoint = wgsPoint;

            InfoWindow infoWindow = CreateInfoWindow(coordinatePairs);

            if (IsWgs == false)
            {
                infoWindow.Content = point;
            }
            else if (wgsPoint != null)
            {
                LatLongDisplayObj lld = new LatLongDisplayObj(wgsPoint.Y, wgsPoint.X);
                infoWindow.Content = lld;
            }
            else
            {
                CalculatePoint(point, infoWindow.Name);
            }
        }

        private InfoWindow CreateInfoWindow(CoordinatePairs coordinatePairs)
        {
            InfoWindow infoWindow = new InfoWindow();
            infoWindow.Name = "InfoWindow" + _windowCount++;
            _dictionaryCoordinatePoints.Add(infoWindow.Name, coordinatePairs);

            infoWindow.Anchor = coordinatePairs.MapCoordinatePoint;
            infoWindow.Map = Map;
            infoWindow.IsOpen = true;
            infoWindow.ContentTemplate = GetDataTemplate();
            infoWindow.MouseLeftButtonDown += new MouseButtonEventHandler(InfoWindow_MouseLeftButtonDown);

            Border border = VisualTreeHelper.GetChild(Map, 0) as Border;
            Grid grid = VisualTreeHelper.GetChild(border, 0) as Grid;
            grid.Children.Add(infoWindow);
            return infoWindow;
        }

        private static DataTemplate CreateLocationTemplate()
        {
            string template = @"
                            <DataTemplate
            					xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                                <StackPanel Orientation=""Vertical"" Margin=""2"">
                                    <TextBlock Text=""Location:"" />
                                    <TextBlock Text=""{Binding X, StringFormat=X\=\{0:0.0000\}}"" />
                                    <TextBlock Text=""{Binding Y, StringFormat=Y\=\{0:0.0000\}}"" />
                                </StackPanel>
                            </DataTemplate>";
            return XamlReader.Load(template) as DataTemplate;
        }

        private DataTemplate GetDataTemplate()
        {
            if (IsWgs)
            {
                return _layoutGrid.Resources[LatLongTemplate] as DataTemplate;
            }

            return _layoutGrid.Resources[XYTemplate] as DataTemplate;
        }

        #endregion private methods
    }
}
