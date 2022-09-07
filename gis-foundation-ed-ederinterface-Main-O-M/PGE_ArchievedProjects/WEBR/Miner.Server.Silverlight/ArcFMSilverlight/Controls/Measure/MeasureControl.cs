using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using AreaUnit = ESRI.ArcGIS.Client.Actions.AreaUnit;
using PointCollection = ESRI.ArcGIS.Client.Geometry.PointCollection;

using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;

namespace ArcFMSilverlight
{
    /// <summary>
    /// Measures distance, area, and radius on a map.
    /// </summary>
    [TemplatePart(Name = ElementMeasureAreaButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementMeasureDistanceButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementMeasureRadiusButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementMeasureClearButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementMeasureUnitsComboBox, Type = typeof(ComboBox))]
    public class MeasureControl : Control, IActiveControl
    {
        #region Private Declarations

        private const double MetersToMiles = 0.0006213700922;
        private const double MetersToFeet = 3.280839895;
        private const double SqMetersToSqMiles = 0.0000003861003;
        private const double SqMetersToSqFeet = 10.76391;
        private const double SqMetersToHectare = 0.0001;
        private const double SqMetersToAcre = 0.00024710538;

        private const string MeasureGraphicsLayer = "MeasureGraphics";

        private const string ElementMeasureUnitsComboBox = "PART_MeasureUnitsComboBox";
        private const string ElementMeasureAreaButton = "PART_MeasureAreaButton";
        private const string ElementMeasureDistanceButton = "PART_MeasureDistanceButton";
        private const string ElementMeasureRadiusButton = "PART_MeasureRadiusButton";
        private const string ElementMeasureClearButton = "PART_MeasureClearButton";

        private const string MeasureCursor = @"/Images/cursor_measure.png";

        private ComboBox _measureUnitsComboBox;
        private ToggleButton _measureAreaButton;
        private ToggleButton _measureDistanceButton;
        private ToggleButton _measureRadiusButton;
        private Button _measureClearButton;

        private ArcFmMeasureAction _measureAction;
        private RadiusDistance _radiusAction;
        /******PLC Changes   RK 07/21/2017*******/
        private PLCWidget _plcWidgetObj = null;
        /*****************PLC Changes End************/
        #endregion Private Declarations

        #region Ctor

        /// <summary>
        /// Constructor
        /// </summary>
        public MeasureControl()
        {
            DefaultStyleKey = typeof(MeasureControl);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
        }

        #endregion Ctor

        #region Dependency Properties

        /// <summary>
        /// Current map.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(MeasureControl),
            new PropertyMetadata(OnMapChanged));

        [Category("Measure Properties")]
        [Description("Associated Map")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        /// <summary>
        /// Measured projection WKID appropriate for area of interest.
        /// </summary>
        public static readonly DependencyProperty MeasuredProjectionProperty = DependencyProperty.Register("MeasuredProjection", typeof(int),
                                                                                            typeof(MeasureControl),
                                                                                            new PropertyMetadata(32611));

        [Category("Measure Properties")]
        [Description("Measured Projection WKID")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int MeasuredProjection
        {
            get { return (int)GetValue(MeasuredProjectionProperty); }
            set { SetValue(MeasuredProjectionProperty, value); }
        }

        /// <summary>
        /// Gets the identifier for the <see cref="StatusBar"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusBarProperty = DependencyProperty.Register(
            "StatusBar",
            typeof(TextBlock),
            typeof(MeasureControl),
            null);

        /// <summary>
        /// Gets or sets the StatusBar.
        /// </summary>
        [Category("Measure Properties")]
        [Description("Gets or sets the StatusBar")]
        public TextBlock StatusBar
        {
            get { return (TextBlock)GetValue(StatusBarProperty); }
            set { SetValue(StatusBarProperty, value); }
        }


        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(MeasureControl), null);

        [Category("Measure Properties")]
        [Description("Geometry Service URL")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string GeometryService
        {
            get { return (string)GetValue(GeometryServiceProperty); }
            set { SetValue(GeometryServiceProperty, value); }
        }

        public static readonly DependencyProperty SignificantDigitsProperty =
           DependencyProperty.Register("SignificantDigits", typeof(int), typeof(MeasureControl),
                                       new PropertyMetadata(2));

        [Category("Measure Properties")]
        [Description("Significant Digits")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int SignificantDigits
        {
            get { return (int)GetValue(SignificantDigitsProperty); }
            set { SetValue(SignificantDigitsProperty, value); }
        }

        public static readonly DependencyProperty SegmentLengthsProperty =
            DependencyProperty.Register("SegmentLengths", typeof(List<double>), typeof(MeasureControl),
                                        new PropertyMetadata(new List<double>()));

        [Category("Measure Properties")]
        [Description("Segment Lengths")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<double> SegmentLengths
        {
            get { return (List<double>)GetValue(SegmentLengthsProperty); }
            set { SetValue(SegmentLengthsProperty, value); }
        }

        public static readonly DependencyProperty AreaTotalProperty =
            DependencyProperty.Register("AreaTotal", typeof(double), typeof(MeasureControl),
                                        new PropertyMetadata(0.0));

        [Category("Measure Properties")]
        [Description("Area Total")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double AreaTotal
        {
            get { return (double)GetValue(AreaTotalProperty); }
            set { SetValue(AreaTotalProperty, value); }
        }

        public static readonly DependencyProperty RadiusLengthProperty =
            DependencyProperty.Register("RadiusLength", typeof(double), typeof(MeasureControl),
                                        new PropertyMetadata(0.0));

        [Category("Measure Properties")]
        [Description("Radius Length")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double RadiusLength
        {
            get { return (double)GetValue(RadiusLengthProperty); }
            set { SetValue(RadiusLengthProperty, value); }
        }

        #endregion Dependency Properties

        #region Public Properties
        //PLC Changes  RK 07/21/2017
        public PLCWidget PLCWidgetObj
        {
            get { return _plcWidgetObj; }
            set { _plcWidgetObj = value; }
        }
        //PLC Changes  end
        public MeasureAction.Mode? MeasureMode
        {
            get { return (MeasureAction.Mode?)GetValue(MeasureModeProperty); }
            set { SetValue(MeasureModeProperty, value); }
        }

        public AreaUnit AreaUnit
        {
            get { return (AreaUnit)GetValue(AreaUnitProperty); }
            set { SetValue(AreaUnitProperty, value); }
        }

        public DistanceUnit DistanceUnit
        {
            get { return (DistanceUnit)GetValue(DistanceUnitProperty); }
            set { SetValue(DistanceUnitProperty, value); }
        }

        public DistanceUnit MapUnits
        {
            get { return (DistanceUnit)GetValue(MapUnitsProperty); }
            set { SetValue(MapUnitsProperty, value); }
        }

        public static readonly DependencyProperty MeasureModeProperty =
         DependencyProperty.Register("MeasureMode", typeof(MeasureAction.Mode?), typeof(MeasureControl), new PropertyMetadata(OnMeasureModeChanged));

        public static readonly DependencyProperty AreaUnitProperty =
            DependencyProperty.Register("AreaUnit", typeof(AreaUnit), typeof(MeasureControl), null);

        public static readonly DependencyProperty DistanceUnitProperty =
            DependencyProperty.Register("DistanceUnit", typeof(DistanceUnit), typeof(MeasureControl), null);

        public static readonly DependencyProperty MapUnitsProperty =
            DependencyProperty.Register("MapUnits", typeof(DistanceUnit), typeof(MeasureControl), null);

        #endregion Public Properties

        #region Public Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _measureUnitsComboBox = GetTemplateChild(ElementMeasureUnitsComboBox) as ComboBox ?? new ComboBox();
            _measureUnitsComboBox.SelectionChanged += MeasureUnitsComboBoxSelectionChanged;

            _measureAreaButton = GetTemplateChild(ElementMeasureAreaButton) as ToggleButton ?? new ToggleButton();
            _measureDistanceButton = GetTemplateChild(ElementMeasureDistanceButton) as ToggleButton ?? new ToggleButton();
            _measureRadiusButton = GetTemplateChild(ElementMeasureRadiusButton) as ToggleButton ?? new ToggleButton();

            _measureClearButton = GetTemplateChild(ElementMeasureClearButton) as Button ?? new Button();
            _measureClearButton.Click += new RoutedEventHandler(_measureClearButton_Click);

            if (string.IsNullOrEmpty(GeometryService))
            {
                _measureRadiusButton.IsEnabled = false;
                _measureAreaButton.IsEnabled = false;
                _measureDistanceButton.IsEnabled = false;

                ToolTipService.SetToolTip(_measureUnitsComboBox, "No Geometry Service Configured.");
            }

            PopulateUnitsDropdown();
            UpdateProperties();
            InitializeMeasureAction();
        }

        #endregion Public Overrides

        #region IActiveControl Members

        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsActive.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(MeasureControl), new PropertyMetadata(OnIsActiveChanged));

        private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var measure = (MeasureControl)d;
            if (measure._measureAction == null) return;
            if (measure._radiusAction == null) return;

            var isActive = (bool)e.NewValue;

            if (isActive == false)
            {
                measure.MeasureMode = null;
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(measure);
                CursorSet.SetID(measure.Map, MeasureCursor);
                /*******PLC Changes  RK 07/21/2017*********/
                measure._measureAction.PLCWidgetObj = measure._plcWidgetObj;
                measure._plcWidgetObj.MeasureActionObj = measure._measureAction;
                /*******PLC Changes End**********/
            }
            measure._measureAction.IsActivated = false;
            measure._radiusAction.IsActivated = false;

            if (measure.MeasureMode != MeasureAction.Mode.Radius)
            {
                measure._measureAction.IsActivated = isActive;
            }
            else
            {
                measure._radiusAction.IsActivated = isActive;
            }
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }

        #endregion IActiveControl Members

        #region Private Events

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var measure = d as MeasureControl;
            if (measure == null) return;

            measure.PopulateUnitsDropdown();
            measure.UpdateProperties();
            measure.InitializeMeasureAction();

            //measure.IsActive = false;
        }

        private static void OnMeasureModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null) return;

            var measure = (MeasureControl)d;

            if (measure.MeasureMode == null) return;

            measure.IsActive = true;
            measure._measureUnitsComboBox.Items.Remove(Enum.GetName(typeof(AreaUnit), AreaUnit.Acres));
            measure._measureUnitsComboBox.Items.Remove(Enum.GetName(typeof(AreaUnit), AreaUnit.Hectares));

            switch (measure.MeasureMode.Value)
            {
                case MeasureAction.Mode.Polygon:
                    measure._measureUnitsComboBox.Items.Add(Enum.GetName(typeof(AreaUnit), AreaUnit.Acres));
                    measure._measureUnitsComboBox.Items.Add(Enum.GetName(typeof(AreaUnit), AreaUnit.Hectares));

                    measure._measureAction.IsActivated = true;
                    measure._radiusAction.IsActivated = false;
                    break;
                case MeasureAction.Mode.Polyline:
                    measure._measureAction.IsActivated = true;
                    measure._radiusAction.IsActivated = false;
                    break;
                case MeasureAction.Mode.Radius:
                    measure._measureAction.IsActivated = false;
                    measure._radiusAction.IsActivated = true;
                    break;
                default:
                    break;
            }
            if (measure._measureUnitsComboBox.SelectedItem == null) measure._measureUnitsComboBox.SelectedIndex = 0;

            measure.AreaTotal = 0.0;
            measure.SegmentLengths = new List<double>();
            measure.RadiusLength = 0.0;

            measure.UpdateProperties();
        }

        private void MeasureUnitsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProperties();
        }

        void _measureClearButton_Click(object sender, RoutedEventArgs e)
        {
            _measureAction.ClearLayers();
            _radiusAction.ClearLayers();
        }

        #endregion Private Events

        #region Private Methods

        private void PopulateUnitsDropdown()
        {
            if (_measureUnitsComboBox == null) return;

            _measureUnitsComboBox.SelectionChanged -= MeasureUnitsComboBoxSelectionChanged;
            _measureUnitsComboBox.Items.Clear();

            //  Get a list of all the possible units.
            var lengthUnits = new MeasureItemCollection(typeof(DistanceUnit));

            foreach (MeasureItem unit in lengthUnits)
            {
                // Remove all the items that are not supported.
                if (unit.Text == DistanceUnit.DecimalDegrees.ToString()) continue;
                if (unit.Text == DistanceUnit.NauticalMiles.ToString()) continue;
                if (unit.Text == DistanceUnit.Undefined.ToString()) continue;
                if (unit.Text == DistanceUnit.Yards.ToString()) continue;

                _measureUnitsComboBox.Items.Add(unit.Text);
            }

            _measureUnitsComboBox.SelectedIndex = 0;
            _measureUnitsComboBox.SelectionChanged += MeasureUnitsComboBoxSelectionChanged;
        }

        private void InitializeMeasureAction()
        {
            if (_measureAction != null) return;
            if (Map == null) return;

            var areaUnitBinding = new Binding { Source = this, Path = new PropertyPath("AreaUnit") };
            var distanceUnitBinding = new Binding { Source = this, Path = new PropertyPath("DistanceUnit") };
            var mapUnitBinding = new Binding { Source = this, Path = new PropertyPath("MapUnits") };
            var measureModeBinding = new Binding { Source = this, Path = new PropertyPath("MeasureMode") };
            var statusBarBinding = new Binding {Source = this, Path = new PropertyPath("StatusBar")};

            _measureAction = new ArcFmMeasureAction
            {
                CursorUri = MeasureCursor,
                LineSymbol = new SimpleLineSymbol(Colors.Red, 2),
                FillSymbol = new SimpleFillSymbol
                {
                    BorderBrush = new SolidColorBrush(Colors.Red),
                    BorderThickness = 2,
                    Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.5 }
                },
                MeasuredProjection = MeasuredProjection,
                Map = Map,
                SignificantDigits = SignificantDigits,
                GeometryServiceUrl = GeometryService,
            };

            BindingOperations.SetBinding(_measureAction, ArcFmMeasureAction.AreaUnitsProperty, areaUnitBinding);
            BindingOperations.SetBinding(_measureAction, ArcFmMeasureAction.DistanceUnitsProperty, distanceUnitBinding);
            BindingOperations.SetBinding(_measureAction, ArcFmMeasureAction.MapUnitsProperty, mapUnitBinding);
            BindingOperations.SetBinding(_measureAction, ArcFmMeasureAction.MeasureModeProperty, measureModeBinding);
            BindingOperations.SetBinding(_measureAction, ArcFmMeasureAction.StatusBarProperty, statusBarBinding);

            if (_radiusAction != null) return;

            _radiusAction = new RadiusDistance
            {
                IsActivated = false,
                Map = Map,
                SignificantDigits = SignificantDigits,
                GeometryServiceUrl = GeometryService
            };

            BindingOperations.SetBinding(_radiusAction, RadiusDistance.DistanceUnitsProperty, distanceUnitBinding);

            var segmentLengthsBinding = new Binding { Source = this, Path = new PropertyPath("SegmentLengths"), Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(_measureAction, ArcFmMeasureAction.SegmentLengthsProperty, segmentLengthsBinding);

            var areaTotalBinding = new Binding { Source = this, Path = new PropertyPath("AreaTotal"), Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(_measureAction, ArcFmMeasureAction.AreaTotalProperty, areaTotalBinding);

            var radiusLengthBinding = new Binding { Source = this, Path = new PropertyPath("RadiusLength"), Mode = BindingMode.TwoWay };
            BindingOperations.SetBinding(_radiusAction, RadiusDistance.RadiusLengthProperty, radiusLengthBinding);

            // Binding in this direction works too but you need the classes to be public not internal.
            //var segmentLengthsBinding = new Binding { Source = _measureAction, Path = new PropertyPath("SegmentLengths") };
            //BindingOperations.SetBinding(this, SegmentLengthsProperty, segmentLengthsBinding);

            //var areaTotalBinding = new Binding { Source = _measureAction, Path = new PropertyPath("AreaTotal") };
            //BindingOperations.SetBinding(this, AreaTotalProperty, areaTotalBinding);

            //var radiusLengthBinding = new Binding { Source = _radiusAction, Path = new PropertyPath("RadiusLength") };
            //BindingOperations.SetBinding(this, RadiusLengthProperty, radiusLengthBinding);

        }

        private string GetUnits()
        {
            var units = Enum.GetName(typeof(DistanceUnit), DistanceUnit.Feet);

            if (_measureUnitsComboBox == null) return units;

            var item = _measureUnitsComboBox.SelectedItem as string;
            return item ?? units;
        }

        private static AreaUnit GetAreaUnits(string units)
        {
            AreaUnit areaUnit;

            try
            {
                areaUnit = (AreaUnit)Enum.Parse(typeof(AreaUnit), units, true);
            }
            catch
            {
                units = "Square" + units;
                try
                {
                    areaUnit = (AreaUnit)Enum.Parse(typeof(AreaUnit), units, true);
                }
                catch
                {
                    areaUnit = AreaUnit.SquareFeet;
                }
            }

            return areaUnit;
        }

        private static DistanceUnit GetDistanceUnits(string units)
        {
            DistanceUnit distanceUnit;

            try
            {
                distanceUnit = (DistanceUnit)Enum.Parse(typeof(DistanceUnit), units, true);
            }
            catch
            {
                distanceUnit = DistanceUnit.Feet;
            }

            return distanceUnit;
        }

        /// <summary>
        /// Get the map units from the map.
        /// </summary>
        /// <returns>the map's units</returns>
        private static esriUnits GetMapUnits(Map map)
        {
            esriUnits mapUnits = esriUnits.esriUnknownUnits;

            try
            {
                foreach (Layer layer in map.Layers)
                {
                    if (layer is ArcGISTiledMapServiceLayer)
                    {
                        Enum.TryParse(((ArcGISTiledMapServiceLayer)layer).Units, out mapUnits);
                        break;
                    }

                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        Enum.TryParse(((ArcGISDynamicMapServiceLayer)layer).Units, out mapUnits);
                        break;
                    }
                }
            }
            catch
            {
                mapUnits = esriUnits.esriUnknownUnits;
            }

            return mapUnits;
        }

        private DistanceUnit GetMapUnits()
        {
            esriUnits mapUnits = GetMapUnits(Map);

            switch (mapUnits)
            {
                case esriUnits.esriUnknownUnits:
                case esriUnits.esriInches:
                case esriUnits.esriPoints:
                case esriUnits.esriFeet:
                    return DistanceUnit.Feet;
                case esriUnits.esriYards:
                    return DistanceUnit.Yards;
                case esriUnits.esriMiles:
                    return DistanceUnit.Miles;
                case esriUnits.esriNauticalMiles:
                    return DistanceUnit.NauticalMiles;
                case esriUnits.esriMillimeters:
                case esriUnits.esriCentimeters:
                case esriUnits.esriDecimeters:
                case esriUnits.esriMeters:
                    return DistanceUnit.Meters;
                case esriUnits.esriKilometers:
                    return DistanceUnit.Kilometers;
                case esriUnits.esriDecimalDegrees:
                    return DistanceUnit.DecimalDegrees;
                default:
                    return DistanceUnit.Feet;
            }
        }

        private void UpdateProperties()
        {
            MapUnits = GetMapUnits();

            var units = GetUnits();

            AreaUnit = GetAreaUnits(units);
            DistanceUnit = GetDistanceUnits(units);
        }

        private static double ConvertUnits(double item, DistanceUnit units)
        {
            if (double.IsNaN(item)) return double.NaN;
            double convertedItem = double.NaN;

            //  Assume from meters.
            switch (units)
            {
                case DistanceUnit.Miles:
                    convertedItem = item * MetersToMiles;
                    break;
                case DistanceUnit.Kilometers:
                    convertedItem = item / 1000;
                    break;
                case DistanceUnit.Feet:
                    convertedItem = item * MetersToFeet;
                    break;
                case DistanceUnit.Meters:
                    convertedItem = item;
                    break;
                default:
                    break;
            }

            return convertedItem;
        }

        private static double ConvertUnits(double item, AreaUnit units)
        {
            if (double.IsNaN(item)) return double.NaN;
            double convertedItem = double.NaN;

            //  Assume from meters.
            switch (units)
            {
                case AreaUnit.SquareMiles:
                    convertedItem = item * SqMetersToSqMiles;
                    break;
                case AreaUnit.SquareFeet:
                    convertedItem = item * SqMetersToSqFeet;
                    break;
                case AreaUnit.SquareKilometers:
                    convertedItem = item / 1000000;
                    break;
                case AreaUnit.SquareMeters:
                    convertedItem = item;
                    break;
                case AreaUnit.Hectares:
                    convertedItem = item * SqMetersToHectare;
                    break;
                case AreaUnit.Acres:
                    convertedItem = item * SqMetersToAcre;
                    break;
                default:
                    break;
            }

            return convertedItem;
        }

        #endregion Private Methods
    }

    #region Extensions

    internal static class MapMouse
    {
        private const int DoubleClickInterval = 200;
        private static readonly DependencyProperty DoubleClickTimerProperty = DependencyProperty.RegisterAttached("DoubleClickTimer", typeof(DispatcherTimer), typeof(UIElement), null);
        private static readonly DependencyProperty DoubleClickHandlersProperty = DependencyProperty.RegisterAttached("DoubleClickHandlers", typeof(List<MouseButtonEventHandler>), typeof(UIElement), null);
        private static readonly DependencyProperty DoubleClickPositionProperty = DependencyProperty.RegisterAttached("DoubleClickPosition", typeof(Point), typeof(UIElement), null);

        /// <summary>
        /// Adds a double click event handler.
        /// </summary>
        /// <param name="element">The Element to listen for double clicks on.</param>
        /// <param name="handler">The handler.</param>
        public static void AddDoubleClick(this UIElement element, MouseButtonEventHandler handler)
        {
            element.MouseLeftButtonDown += ElementMouseLeftButtonDown;
            var handlers = element.GetValue(DoubleClickHandlersProperty) as List<MouseButtonEventHandler>;
            if (handlers == null)
            {
                handlers = new List<MouseButtonEventHandler>();
                element.SetValue(DoubleClickHandlersProperty, handlers);
            }
            handlers.Add(handler);
        }
        /// <summary>
        /// Removes a double click event handler.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void RemoveDoubleClick(this UIElement element, MouseButtonEventHandler handler)
        {
            element.MouseLeftButtonDown -= ElementMouseLeftButtonDown;
            var handlers = element.GetValue(DoubleClickHandlersProperty) as List<MouseButtonEventHandler>;
            if (handlers == null) return;
            handlers.Remove(handler);
            if (handlers.Count == 0) element.ClearValue(DoubleClickHandlersProperty);
        }
        private static void ElementMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as UIElement;
            if (element == null) return;

            Point position = e.GetPosition(element);
            var timer = element.GetValue(DoubleClickTimerProperty) as DispatcherTimer;
            if (timer != null) //DblClick
            {
                timer.Stop();
                var oldPosition = (Point)element.GetValue(DoubleClickPositionProperty);
                element.ClearValue(DoubleClickTimerProperty);
                element.ClearValue(DoubleClickPositionProperty);
                if (Math.Abs(oldPosition.X - position.X) < 1 && Math.Abs(oldPosition.Y - position.Y) < 1) //mouse didn't move => Valid double click
                {
                    var handlers = element.GetValue(DoubleClickHandlersProperty) as List<MouseButtonEventHandler>;
                    if (handlers != null)
                    {
                        foreach (MouseButtonEventHandler handler in handlers)
                        {
                            handler(sender, e);
                        }
                    }
                    return;
                }
            }

            //First click or mouse moved. Start a new timer
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(DoubleClickInterval) };
            timer.Tick += (s, args) =>
            {  //DblClick timed out
                var d = s as DispatcherTimer;
                if (d != null)
                {
                    d.Stop();
                }
                element.ClearValue(DoubleClickTimerProperty); //clear timer
                element.ClearValue(DoubleClickPositionProperty); //clear first click position
            };
            element.SetValue(DoubleClickTimerProperty, timer);
            element.SetValue(DoubleClickPositionProperty, position);
            timer.Start();
        }
    }

    #endregion

    #region Internal Classes

    #region Enum Helpers

    internal class MeasureItemCollection : List<MeasureItem>
    {
        public MeasureItemCollection(Type enumType)
        {
            int count = 0;
            foreach (FieldInfo fieldInfo in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                int value;

                try
                {
                    value = ((int)Enum.Parse(enumType, fieldInfo.Name, false));
                }
                catch (Exception)
                {
                    value = count;
                }

                var item = new MeasureItem
                {
                    Text = fieldInfo.Name,
                    Value = value
                };

                Add(item);
                count++;
            }
        }
    }

    internal class MeasureItem : IComparable
    {
        public int Value { get; set; }
        public string Text { get; set; }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            return Text.CompareTo(((MeasureItem)obj).Text);
        }

        #endregion
    }

    #endregion

    #region Actions

    /*********PLC Changes Rk 07/21/2017********/
    public class ArcFmMeasureAction : DependencyObject
    /*********PLC Changes End********/
    {
        #region Constants

        private const int WgsId = 4326;
        private const int TotalDistanceSymbolPosition = 0;
        private const int TotalAreaSymbolPosition = 1;

        private const double MetersToMiles = 0.0006213700922;
        private const double MetersToFeet = 3.280839895;
        private const double SqMetersToSqMiles = 0.0000003861003;
        private const double SqMetersToSqFeet = 10.76391;
        private const double SqMetersToHectare = 0.0001;
        private const double SqMetersToAcre = 0.00024710538;

        #endregion

        #region Public Properties

        public double NumberDecimals { get; set; }
        public double TotalLength { get; set; }
        public double TotalArea { get; set; }
        public FillSymbol FillSymbol { get; set; }
        public LineSymbol LineSymbol { get; set; }
        public string CursorUri { get; set; }
        /*******PLC Changes  RK 07/21/2017*********/
        public PLCWidget PLCWidgetObj { get; set; }
        /**************PLC Changes End*************/

        #endregion

        #region Private Properties

        private readonly SimpleMarkerSymbol _markerSymbol;
        private List<SegmentLayer> _layers = new List<SegmentLayer>();
        private int _mouseMove;
        private int _areaAsyncCalls;
        private int _graphicCount;
        private double _mapDistance;
        private double _distanceFactor;
        private bool _firstTime;
        private bool _isMeasuring;
        private bool _panning;
        private bool _finishingSegment;
        private bool _finishingArea;
        private bool _finishMeasure;
        private bool _finishedSegment;

        #endregion

        #region Dependecy Properties

        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(null, OnMapPropertyChanged));

        public static readonly DependencyProperty StatusBarProperty =
            DependencyProperty.Register("StatusBar", typeof (TextBlock),
                                        typeof (ArcFmMeasureAction), null);

        public static readonly DependencyProperty SegmentLengthsProperty =
            DependencyProperty.Register("SegmentLengths", typeof(List<double>), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(null));

        public static readonly DependencyProperty AreaTotalProperty =
            DependencyProperty.Register("AreaTotal", typeof(double), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(0.0));

        public static readonly DependencyProperty GeometryServiceUrlProperty =
            DependencyProperty.Register("GeometryServiceUrl", typeof(string), typeof(ArcFmMeasureAction), null);

        public static readonly DependencyProperty DistanceUnitsProperty =
            DependencyProperty.Register("DistanceUnits", typeof(DistanceUnit), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(DistanceUnit.Feet, UnitsChanged));

        public static readonly DependencyProperty MapUnitsProperty =
            DependencyProperty.Register("MapUnits", typeof(DistanceUnit), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(DistanceUnit.Feet));

        public static readonly DependencyProperty AreaUnitsProperty =
            DependencyProperty.Register("AreaUnits", typeof(AreaUnit), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(AreaUnit.SquareFeet, UnitsChanged));

        public static readonly DependencyProperty IsActivatedProperty =
            DependencyProperty.Register("IsActivated", typeof(bool), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(false, OnIsActivatedPropertyChanged));

        public static readonly DependencyProperty MeasureModeProperty =
            DependencyProperty.Register("MeasureMode", typeof(MeasureAction.Mode?), typeof(ArcFmMeasureAction),
                                        new PropertyMetadata(MeasureAction.Mode.Polyline, OnMeasureModePropertyChanged));

        public static readonly DependencyProperty SignificantDigitsProperty =
           DependencyProperty.Register("SignificantDigits", typeof(int), typeof(ArcFmMeasureAction),
                                       new PropertyMetadata(2));

        [Category("Measure Action Properties")]
        [Description("Significant Digits")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SignificantDigits
        {
            get { return (int)GetValue(SignificantDigitsProperty); }
            set { SetValue(SignificantDigitsProperty, value); }
        }

        /// <summary>
        /// Measured projection WKID appropriate for area of interest.
        /// </summary>
        public static readonly DependencyProperty MeasuredProjectionProperty = DependencyProperty.Register("MeasuredProjection", typeof(int),
                                                                                            typeof(ArcFmMeasureAction),
                                                                                            new PropertyMetadata(32611));

        [Category("Measure Action Properties")]
        [Description("Measured Projection WKID")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int MeasuredProjection
        {
            get { return (int)GetValue(MeasuredProjectionProperty); }
            set { SetValue(MeasuredProjectionProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Map Control")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Gets or sets the StatusBar")]
        public TextBlock StatusBar
        {
            get { return (TextBlock)GetValue(StatusBarProperty); }
            set { SetValue(StatusBarProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Display units for linear measurements")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DistanceUnit DistanceUnits
        {
            get { return (DistanceUnit)GetValue(DistanceUnitsProperty); }
            set { SetValue(DistanceUnitsProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Map Units")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DistanceUnit MapUnits
        {
            get { return (DistanceUnit)GetValue(MapUnitsProperty); }
            set { SetValue(MapUnitsProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Display units for area measurements")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AreaUnit AreaUnits
        {
            get { return (AreaUnit)GetValue(AreaUnitsProperty); }
            set { SetValue(AreaUnitsProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Measure Mode")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MeasureAction.Mode? MeasureMode
        {
            get { return (MeasureAction.Mode?)GetValue(MeasureModeProperty); }
            set { SetValue(MeasureModeProperty, value); }
        }


        [Category("Measure Action Properties")]
        [Description("Is Activated")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsActivated
        {
            get { return (bool)GetValue(IsActivatedProperty); }
            set { SetValue(IsActivatedProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Segment Lengths")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public List<double> SegmentLengths
        {
            get { return (List<double>)GetValue(SegmentLengthsProperty); }
            set { SetValue(SegmentLengthsProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Area Total")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double AreaTotal
        {
            get { return (double)GetValue(AreaTotalProperty); }
            set { SetValue(AreaTotalProperty, value); }
        }

        [Category("Measure Action Properties")]
        [Description("Geometry Serive URL")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GeometryServiceUrl
        {
            get { return (string)GetValue(GeometryServiceUrlProperty); }
            set { SetValue(GeometryServiceUrlProperty, value); }
        }

        #endregion

        #region Constructor

        public ArcFmMeasureAction()
        {
            //Set up defaults
            _firstTime = false;
            _areaAsyncCalls = 0;
            _mouseMove = 0;

            MapUnits = DistanceUnit.DecimalDegrees;
            NumberDecimals = 2;
            DistanceUnits = DistanceUnit.Kilometers;
            AreaUnits = AreaUnit.SquareKilometers;
            MeasureMode = MeasureAction.Mode.Polyline;
            
            LineSymbol = new SimpleLineSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                Width = 2,
                Style = SimpleLineSymbol.LineStyle.Solid
            };
            FillSymbol = new SimpleFillSymbol
            {
                Fill = new SolidColorBrush(Color.FromArgb(0x22, 255, 255, 255)),
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)),
                BorderThickness = 2
            };
            _markerSymbol = new SimpleMarkerSymbol
            {
                Color = new SolidColorBrush(Color.FromArgb(0x66, 255, 0, 0)),
                Size = 5,
                Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
            };

            SegmentLengths = new List<double>();
        }

        #endregion

        #region Property Change Events

        private static void OnIsActivatedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gd = d as ArcFmMeasureAction;
            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;
            if (newValue == oldValue) return;

            if (gd != null)
            {
                if (gd.Map != null)
                {
                    if (newValue)
                    {
                        gd.Map.MouseMove += gd.MapMouseMove;
                        gd.Map.MouseMove += gd.MapMouseMovePan;
                        gd.Map.MouseLeftButtonDown += gd.MapMouseLeftButtonDown;
                        gd._layers.Add(new SegmentLayer());
                        gd.Map.Layers.Add(gd._layers.Last().Layer);
                    }
                    else
                    {
                        if (gd._isMeasuring && gd._layers.Any())
                        {
                            if (gd.Map.Layers.Contains(gd._layers.Last().Layer))
                            {
                                gd.Map.Layers.Remove(gd._layers.Last().Layer);
                            }
                            gd._layers.Remove(gd._layers.Last());
                        }

                        gd.Map.MouseMove -= gd.MapMouseMove;
                        gd.Map.MouseMove -= gd.MapMouseMovePan;
                        gd.Map.MouseLeftButtonDown -= gd.MapMouseLeftButtonDown;
                        gd.SegmentLengths = new List<double>();
                        gd.AreaTotal = 0.0;
                        gd._panning = false;
                        gd._finishingSegment = false;
                        gd._finishingArea = false;
                        gd._isMeasuring = false;
                        ResetValues(gd);
                    }
                }
            }
        }

        private static void OnMeasureModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gd = d as ArcFmMeasureAction;
            if (gd == null) return;

            if (gd._isMeasuring && gd._layers.Any())
            {
                gd.Map.Layers.Remove(gd._layers.Last().Layer);
                gd._layers.Remove(gd._layers.Last());

                gd._layers.Add(new SegmentLayer());
                gd.Map.Layers.Add(gd._layers.Last().Layer);
            }
        }

        private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gd = d as ArcFmMeasureAction;
            var oldMap = e.OldValue as Map;
            if (gd == null) return;
            bool isActive = gd.IsActivated;
            if (isActive)
                gd.IsActivated = false;
            if (oldMap != null)
            {
                foreach (var layer in gd._layers)
                {
                    layer.Layer.ClearGraphics();
                    gd.Map.Layers.Remove(layer.Layer);
                }

                gd._layers.Clear();
            }
            gd.IsActivated = isActive;
        }

        private static void UnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var measure = d as ArcFmMeasureAction;
            if (measure == null) return;

            measure.UpdateMeasurementUnits();
        }

        #endregion

        #region Geometry Service Event Methods

        private void GeometryServiceAreaProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var poly = e.Results[0].Geometry as Polygon;

            var geometryService = new GeometryService(GeometryServiceUrl);
            geometryService.Failed += GeometryServiceFailed;
            geometryService.AreasAndLengthsCompleted += GeometryServiceAreaCompleted;

            var polyGraphic = new Graphic { Symbol = new SimpleFillSymbol(), Geometry = poly };
            polyGraphic.Geometry.SpatialReference = new SpatialReference(MeasuredProjection);

            var polyGraphicList = new List<Graphic> { polyGraphic };

            geometryService.AreasAndLengthsAsync(polyGraphicList, LinearUnit.Meter, LinearUnit.Meter, string.Empty);

        }

        private void GeometryServiceAreaCompleted(object sender, AreasAndLengthsEventArgs e)
        {
            _areaAsyncCalls--;

            var area = Math.Abs(e.Results.Areas[0]);

            if (_layers.Last().Layer.Graphics.Count < 2) return;

            var totSym = _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol as RotatingTextSymbol;
            if (totSym != null) totSym.Text = string.Format("Area = {0}{1}", RoundToSignificantDigit(ConvertUnits(area, AreaUnits)), PrettyUnits(AreaUnits));

            _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol = totSym;
        }

        private void GeometryServiceFinalAreaProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var poly = e.Results[0].Geometry as Polygon;

            var geometryService = new GeometryService(GeometryServiceUrl);
            geometryService.Failed += GeometryServiceFailed;
            geometryService.AreasAndLengthsCompleted += GeometryServiceFinalAreaCompleted;

            var polyGraphic = new Graphic { Symbol = new SimpleFillSymbol(), Geometry = poly };
            polyGraphic.Geometry.SpatialReference = new SpatialReference(MeasuredProjection);

            var polyGraphicList = new List<Graphic> { polyGraphic };

            geometryService.AreasAndLengthsAsync(polyGraphicList, LinearUnit.Meter, LinearUnit.Meter, string.Empty);
        }

        private void GeometryServiceFinalAreaCompleted(object sender, AreasAndLengthsEventArgs e)
        {
            // Re-enable clicks
            _finishingArea = false;

            var area = Math.Abs(e.Results.Areas[0]);

            // Set the property now that we have the final area.
            AreaTotal = area;
            _layers.Last().AreaTotal = area;

            if (_layers.Last().Layer.Graphics.Count < 2) return;

            var totSym = _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol as RotatingTextSymbol;
            if (totSym != null) totSym.Text = string.Format("Area = {0}{1}", RoundToSignificantDigit(ConvertUnits(area, AreaUnits)), PrettyUnits(AreaUnits));

            _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Symbol = totSym;

            if (!_finishingSegment && MeasureMode != MeasureAction.Mode.Radius) ResetValues();
        }

        private void GeometryServiceFinalDistanceProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var geometryService = new GeometryService(GeometryServiceUrl);
            geometryService.Failed += GeometryServiceFailed;
            geometryService.DistanceCompleted += GeometryServiceFinalDistanceCompleted;

            int g = MeasureMode == MeasureAction.Mode.Polyline ? g = _layers.Last().Layer.Graphics.Count - 5 : g = _layers.Last().Layer.Graphics.Count - 4;

            var distParams = new DistanceParameters
            {
                DistanceUnit = LinearUnit.Meter,
                Geodesic = true
            };

            _finishingSegment = true;
            geometryService.DistanceAsync(e.Results[0].Geometry, e.Results[1].Geometry, distParams, g);
        }

        private void GeometryServiceFinalDistanceCompleted(object sender, DistanceEventArgs e)
        {
            // Re-enable clicks
            _finishingSegment = false;

            double dist = e.Distance;

            var g = (int)e.UserState;

            if (_layers.Last().Layer.Graphics.Count < 1) return;
            if (_layers.Last().Layer.Graphics.Count < g + 1) return;

            var symb = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
            if (symb != null)
            {
                var displayDistance = ConvertMetersToActiveUnits(dist, DistanceUnits);

                symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
                _layers.Last().Layer.Graphics[g].Symbol = symb;
            }

            _layers.Last().TotalLength += dist;
            SegmentLengths.Add(dist);
            _layers.Last().SegmentLengths.Add(dist);

            // Used to trigger Property Change.
            SegmentLengths = new List<double>(SegmentLengths);

            if (MeasureMode != MeasureAction.Mode.Polyline) return;

            // Update total distance graphic.
            var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
            if (totSym == null) return;

            totSym.Text = string.Format("Total = {0}{1}",
                                        RoundToSignificantDigit(ConvertMetersToActiveUnits(_layers.Last().TotalLength, DistanceUnits)),
                                        PrettyUnits(DistanceUnits));
            _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;

            if (_finishMeasure && MeasureMode != MeasureAction.Mode.Radius) ResetValues();
        }

        private void GeometryServiceAreaFinalSegmentDistanceProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var geometryService = new GeometryService(GeometryServiceUrl);
            geometryService.Failed += GeometryServiceFailed;
            geometryService.DistanceCompleted += GeometryServiceAreaFinalSegmentDistanceCompleted;

            int g = _layers.Last().Layer.Graphics.Count - 1;

            var distParams = new DistanceParameters
            {
                DistanceUnit = LinearUnit.Meter,
                Geodesic = true
            };

            geometryService.DistanceAsync(e.Results[0].Geometry, e.Results[1].Geometry, distParams, g);
        }

        private void GeometryServiceAreaFinalSegmentDistanceCompleted(object sender, DistanceEventArgs e)
        {
            // Re-enable clicks
            _finishingSegment = false;

            var g = (int)e.UserState;
            var layer = _layers.Last().Layer.Graphics.Count < g + 1 ? _layers[_layers.Count - 2] : _layers.Last();

            var dist = e.Distance;

            SegmentLengths.Add(dist);
            layer.SegmentLengths.Add(dist);

            // Used to trigger Property Change.
            SegmentLengths = new List<double>(SegmentLengths);

            if (layer.Layer.Graphics.Count < g + 1) return;

            var symb = layer.Layer.Graphics[g].Symbol as RotatingTextSymbol;
            if (symb == null) return;

            var displayDistance = ConvertMetersToActiveUnits(dist, DistanceUnits);

            symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
            layer.Layer.Graphics[g].Symbol = symb;

            if (!_finishingArea && MeasureMode != MeasureAction.Mode.Radius) ResetValues();
        }

        private void GeometryServiceMoveDistanceProjectCompleted(object sender, GraphicsEventArgs e)
        {
            if (GraphicHasNaN(e.Results[0]) || GraphicHasNaN(e.Results[1]))
            {
                StatusBar.Text = "Measure tool: Measurement is outside of area covered by ProjectionWKID spatial reference.";
            }
            else
            {
                StatusBar.Text = "";
            }

            var geometryService = new GeometryService(GeometryServiceUrl);
            geometryService.Failed += GeometryServiceFailed;
            geometryService.DistanceCompleted += GeometryServiceMoveDistanceCompleted;

            int g = _layers.Last().Layer.Graphics.Any() ? _layers.Last().Layer.Graphics.Count - 1 : _layers[_layers.Count - 2].Layer.Graphics.Count - 1;

            var distParams = new DistanceParameters
            {
                DistanceUnit = LinearUnit.Meter,
                Geodesic = true
            };

            geometryService.DistanceAsync(e.Results[0].Geometry, e.Results[1].Geometry, distParams, g);
        }

        private bool GraphicHasNaN(Graphic graphic)
        {
            var mapPoint = graphic.Geometry as MapPoint;
            return double.IsNaN(mapPoint.X) || double.IsNaN(mapPoint.Y);
        }

        private void GeometryServiceMoveDistanceCompleted(object sender, DistanceEventArgs e)
        {
            var dist = e.Distance;
            _distanceFactor = dist / _mapDistance;

            var g = (int)e.UserState;

            if (_layers.Last().Layer.Graphics.Count < g + 1) return;

            var symb = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
            if (symb != null)
            {
                var displayDistance = ConvertMetersToActiveUnits(dist, DistanceUnits);

                symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
                _layers.Last().Layer.Graphics[g].Symbol = symb;
            }

            _layers.Last().TempTotalLength = _layers.Last().TotalLength + dist;

            if (MeasureMode != MeasureAction.Mode.Polyline) return;

            var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
            if (totSym == null) return;

            totSym.Text = string.Format("Total = {0}{1}", RoundToSignificantDigit(ConvertMetersToActiveUnits(_layers.Last().TempTotalLength, DistanceUnits)), PrettyUnits(DistanceUnits));

            _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
        }


        private void GeometryServiceFailed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show("An error occured reaching the geometry service " + GeometryServiceUrl + "\n" + e.Error.Message);
        }

        #endregion

        #region Map Event Methods

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            var map = sender as Map;
            if (map == null) return;

            if (!_layers.Any() || _layers.Last().OriginPoint == null || !_isMeasuring) return;

            if (_finishMeasure) return;

            _graphicCount = _layers.Last().Layer.Graphics.Count;
            int g = _graphicCount - 1;

            MapPoint p = Map.ScreenToMap(e.GetPosition(Map));

            // Update the total distance geometry for Polyline mode.
            if (MeasureMode == MeasureAction.Mode.Polyline)
            {
                var totSymGeo = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Geometry;
                if (totSymGeo == null) return;

                totSymGeo = p;

                _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Geometry = totSymGeo;
            }

            // We could use a geometry service.
            #region Geometry Service Length

            var mapDistance = _layers.Last().OriginPoint.DistanceTo(p);
            if (_firstTime)
            {
                _firstTime = false;
                _mapDistance = mapDistance;
                
                var geometryService = new GeometryService(GeometryServiceUrl);
                geometryService.Failed += GeometryServiceFailed;
                geometryService.ProjectCompleted += GeometryServiceMoveDistanceProjectCompleted;

                var graphics = new List<Graphic>();
                var graphic1 = new Graphic();
                var graphic2 = new Graphic();
                graphic1.Geometry = _layers.Last().OriginPoint;
                graphic2.Geometry = p;
                graphics.Add(graphic1);
                graphics.Add(graphic2);
                geometryService.ProjectAsync(graphics, new SpatialReference(MeasuredProjection));
            }
            else
            {
                var dist = _distanceFactor * mapDistance;

                var symb = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
                if (symb != null)
                {
                    var displayDistance = ConvertMetersToActiveUnits(dist, DistanceUnits);

                    symb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
                    _layers.Last().Layer.Graphics[g].Symbol = symb;
                }

                _layers.Last().TempTotalLength = _layers.Last().TotalLength + dist;

                if (MeasureMode == MeasureAction.Mode.Polyline)
                {
                    var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                    if (totSym == null) return;

                    totSym.Text = string.Format("Total = {0}{1}", RoundToSignificantDigit(ConvertMetersToActiveUnits(_layers.Last().TempTotalLength, DistanceUnits)), PrettyUnits(DistanceUnits));

                    _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
                }
            }
            
            #endregion Geometry Service Length

            // Or we could estimate.
            #region Estimate Length

            var midpoint = new MapPoint((p.X + _layers.Last().OriginPoint.X) / 2, (p.Y + _layers.Last().OriginPoint.Y) / 2);
            var polypoints = new PointCollection();
            Polygon poly;

            if (MeasureMode == MeasureAction.Mode.Polygon && _layers.Last().Points.Count > 2)
            {
                Graphic graphic = _layers.Last().Layer.Graphics[0];
                poly = graphic.Geometry as Polygon;

                if (poly != null)
                {
                    polypoints = poly.Rings[0];
                    int lastPt = polypoints.Count - 1;
                    polypoints[lastPt] = p;
                }
            }
            _layers.Last().Layer.Graphics[g - 2].Geometry = midpoint;

            var line = _layers.Last().Layer.Graphics[g - 1].Geometry as Polyline;
            if (line != null) line.Paths[0][1] = p;
            _layers.Last().Layer.Graphics[g].Geometry = midpoint;

            double angle = Math.Atan2((p.X - _layers.Last().OriginPoint.X), (p.Y - _layers.Last().OriginPoint.Y)) / Math.PI * 180 - 90;
            if (angle > 90 || angle < -90) angle -= 180;

            var symbol = _layers.Last().Layer.Graphics[g].Symbol as RotatingTextSymbol;
            if (symbol != null)
            {
                symbol.Angle = angle;
                _layers.Last().Layer.Graphics[g].Symbol = symbol;
            }

            if (MeasureMode == MeasureAction.Mode.Polyline)
            {
                var totSym = _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                if (totSym == null) return;

                _layers.Last().Layer.Graphics[TotalDistanceSymbolPosition].Symbol = totSym;
            }

            #endregion Estimate Length

            if (MeasureMode != MeasureAction.Mode.Polygon) return;

            if (polypoints.Count <= 2) return;

            poly = _layers.Last().Layer.Graphics[0].Geometry as Polygon;
            if (poly == null) return;

            MapPoint anchor = poly.Extent.GetCenter();

            _layers.Last().Layer.Graphics[TotalAreaSymbolPosition].Geometry = anchor;

            // Don't fire too many in mouse move.
            if (_areaAsyncCalls >= 1) return;

            _mouseMove++;
            if (_mouseMove >= 50)
            {
                _mouseMove = 0;

                var areaGeometryService = new GeometryService(GeometryServiceUrl);
                areaGeometryService.Failed += GeometryServiceFailed;
                areaGeometryService.ProjectCompleted += GeometryServiceAreaProjectCompleted;

                var polyGraphic = new Graphic { Symbol = new SimpleFillSymbol(), Geometry = poly };
                polyGraphic.Geometry.SpatialReference = map.SpatialReference;

                var polyGraphicList = new List<Graphic> { polyGraphic };

                _areaAsyncCalls++;
                areaGeometryService.ProjectAsync(polyGraphicList, new SpatialReference(MeasuredProjection));
            }
        }

        private void MapMouseMovePan(object sender, MouseEventArgs e)
        {
            const double percentBuffer = .15;
            const double panFactor = .04;

            if (!_isMeasuring) return;
            if (_finishMeasure) return;

            var map = sender as Map;
            if (map == null) return;

            // Get the current mouse position.
            var mouseLocation = map.ScreenToMap(e.GetPosition(map));

            if (mouseLocation == null) return;

            var mX = mouseLocation.X;
            var mY = mouseLocation.Y;

            double percentX = 1.0;
            double percentY = 1.0;

            var innerExtent = new Envelope
            {
                XMax =
                    map.Extent.XMax -
                    ((map.Extent.XMax - map.Extent.XMin) * percentBuffer),
                YMax =
                    map.Extent.YMax -
                    ((map.Extent.YMax - map.Extent.YMin) * percentBuffer),
                XMin =
                    map.Extent.XMin +
                    ((map.Extent.XMax - map.Extent.XMin) * percentBuffer),
                YMin =
                    map.Extent.YMin +
                    ((map.Extent.YMax - map.Extent.YMin) * percentBuffer)
            };

            if ((((mX >= innerExtent.XMin) && (mX <= innerExtent.XMax)) &&
                 (mY >= innerExtent.YMin)) && (mY <= innerExtent.YMax))
            {
                _panning = false;
                return;
            }

            _panning = true;

            if (mX < innerExtent.XMin)
            {
                double amount = innerExtent.XMin - mX;
                percentX = amount / (innerExtent.XMin - map.Extent.XMin);
            }
            else if (mX > innerExtent.XMax)
            {
                double amount = mX - innerExtent.XMax;
                percentX = amount / (map.Extent.XMax - innerExtent.XMax);
            }

            if (mY < innerExtent.YMin)
            {
                double amount = innerExtent.YMin - mY;
                percentY = amount / (innerExtent.YMin - map.Extent.YMin);
            }
            else if (mY > innerExtent.YMax)
            {
                double amount = mY - innerExtent.YMax;
                percentY = amount / (map.Extent.YMax - innerExtent.YMax);
            }

            MapPoint centerPoint = map.Extent.GetCenter();

            if (mX < innerExtent.XMin) centerPoint.X -= (map.Extent.Width * panFactor * percentX * percentX);
            else if (mX > innerExtent.XMax) centerPoint.X += (map.Extent.Width * panFactor * percentX * percentX);

            if (mY < innerExtent.YMin) centerPoint.Y -= (map.Extent.Height * panFactor * percentY * percentY);
            else if (mY > innerExtent.YMax) centerPoint.Y += (map.Extent.Height * panFactor * percentY * percentY);

            map.PanTo(centerPoint);
        }

        private void MapMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e != null)
            {
                e.Handled = true;
            }

            if (!_isMeasuring) return;
            if (!_layers.Any()) return;

            var map = sender as Map;
            if (map == null) return;

            int lastone = _layers.Last().Layer.Graphics.Count - 1;
            if (lastone < 0)
            {
                ResetValues();
                return;
            }

            _firstTime = true;
            _finishedSegment = true;
            _layers.Last().Layer.Graphics.RemoveAt(lastone);

            if (MeasureMode == MeasureAction.Mode.Polygon)
            {
                if (_layers.Last().Layer.Graphics.Count < 1)
                {
                    ResetValues();
                    return;
                }

                var poly1 = _layers.Last().Layer.Graphics[0].Geometry as Polygon;
                if (poly1 == null)
                {
                    ResetValues();
                    return;
                }

                MapPoint firstpoint = poly1.Rings[0][0];
                poly1.Rings[0].Add(new MapPoint(firstpoint.X, firstpoint.Y, firstpoint.SpatialReference));

                _layers.Last().Layer.Graphics[0].Geometry = poly1;

                if (_layers.Last().Points.Count > 2)
                {
                    var midpoint = new MapPoint((firstpoint.X + _layers.Last().OriginPoint.X) / 2, (firstpoint.Y + _layers.Last().OriginPoint.Y) / 2);

                    double angle = Math.Atan2((firstpoint.X - _layers.Last().OriginPoint.X), (firstpoint.Y - _layers.Last().OriginPoint.Y)) / Math.PI * 180 - 90;
                    if (angle > 90 || angle < -90) angle -= 180;

                    var symb = new RotatingTextSymbol { Angle = angle };

                    var textGraphic = new Graphic
                    {
                        Geometry = midpoint,
                        Symbol = symb
                    };

                    textGraphic.SetZIndex(1);

                    _layers.Last().Layer.Graphics.Add(textGraphic);

                    _finishingSegment = true;
                    _finishMeasure = true;

                    var geometryService = new GeometryService(GeometryServiceUrl);
                    geometryService.Failed += GeometryServiceFailed;
                    geometryService.ProjectCompleted += GeometryServiceAreaFinalSegmentDistanceProjectCompleted;

                    var graphics = new List<Graphic>();
                    var graphic1 = new Graphic();
                    var graphic2 = new Graphic();
                    graphic1.Geometry = _layers.Last().OriginPoint;
                    graphic2.Geometry = firstpoint;
                    graphics.Add(graphic1);
                    graphics.Add(graphic2);
                    geometryService.ProjectAsync(graphics, new SpatialReference(MeasuredProjection));

                    _finishingArea = true;

                    // Get the final area.
                    var areaGeometryService = new GeometryService(GeometryServiceUrl);
                    areaGeometryService.Failed += GeometryServiceFailed;
                    areaGeometryService.ProjectCompleted += GeometryServiceFinalAreaProjectCompleted;

                    var polyGraphic = new Graphic { Symbol = new SimpleFillSymbol(), Geometry = poly1 };
                    polyGraphic.Geometry.SpatialReference = map.SpatialReference;

                    var polyGraphicList = new List<Graphic> { polyGraphic };

                    areaGeometryService.ProjectAsync(polyGraphicList, new SpatialReference(MeasuredProjection));
                }
            }
            else
            {
                if (!_finishingSegment) ResetValues();
            }
        }

        private void MapMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (_panning) return;

            _firstTime = true;
            if (!_layers.Any())
            {
                _layers.Add(new SegmentLayer());
                Map.Layers.Add(_layers.Last().Layer);
            }

            Point pt = e.GetPosition(null);

            if (!_finishedSegment && Math.Abs(pt.X - _layers.Last().LastClick.X) < 2 && Math.Abs(pt.Y - _layers.Last().LastClick.Y) < 2)
            {
                _finishMeasure = true;
                /*******PLC Changes  RK 07/21/2017*********/
                if (PLCWidgetObj.IsmeasureNewPoleActive)
                {
                    PLCWidgetObj.createPoleAfterMeasure(sender, e);
                }
                /*******PLC Changes  End*********/
                MapMouseDoubleClick(sender, e);
            }
            else if (!(_finishingSegment || _finishingArea))
            {
                _finishedSegment = false;

                var map = sender as Map;
                if (map == null) return;

                if (_layers.Last().Points.Count == 0)
                {
                    if (MeasureMode == MeasureAction.Mode.Polygon)
                    {
                        var areaGraphic = new Graphic
                        {
                            Symbol = FillSymbol
                        };

                        _layers.Last().Layer.Graphics.Add(areaGraphic);
                        var areaTotalGraphic = new Graphic
                        {
                            Symbol = new RotatingTextSymbol()
                        };

                        // Bump the Z up so fill will be underneath.
                        areaTotalGraphic.SetZIndex(2);

                        _layers.Last().Layer.Graphics.Add(areaTotalGraphic);
                    }
                }

                if (_layers.Last().OriginPoint != null) _layers.Last().PrevOrigin = _layers.Last().OriginPoint;

                _layers.Last().OriginPoint = map.ScreenToMap(e.GetPosition(map));
                _layers.Last().EndPoint = map.ScreenToMap(e.GetPosition(map));

                var line = new Polyline();
                var points = new PointCollection { _layers.Last().OriginPoint, _layers.Last().EndPoint };

                line.Paths.Add(points);
                _layers.Last().Points.Add(_layers.Last().EndPoint);

                if (_layers.Last().Points.Count == 2) _layers.Last().Points.Add(_layers.Last().EndPoint);
                _layers.Last().LineCount++;

                if (MeasureMode == MeasureAction.Mode.Polygon && _layers.Last().Points.Count > 2)
                {
                    var poly = new Polygon();
                    poly.Rings.Add(_layers.Last().Points);
                    _layers.Last().Layer.Graphics[0].Geometry = poly;
                }

                if (MeasureMode == MeasureAction.Mode.Polyline)
                {
                    var totalSymbol = new RotatingTextSymbol();
                    totalSymbol.OffsetY += 25;

                    var totalTextGraphic = new Graphic
                    {
                        Geometry = _layers.Last().OriginPoint,
                        Symbol = totalSymbol
                    };

                    // Bump the Z up so lines will be underneath.
                    totalTextGraphic.SetZIndex(2);

                    _layers.Last().Layer.Graphics.Add(totalTextGraphic);
                }

                var marker = new Graphic
                {
                    Geometry = _layers.Last().EndPoint,
                    Symbol = _markerSymbol
                };

                _layers.Last().Layer.Graphics.Add(marker);

                var lineGraphic = new Graphic
                {
                    Geometry = line,
                    Symbol = LineSymbol
                };

                _layers.Last().Layer.Graphics.Add(lineGraphic);

                var textGraphic = new Graphic
                {
                    Geometry = _layers.Last().EndPoint,
                    Symbol = new RotatingTextSymbol()
                };

                textGraphic.SetZIndex(1);

                _layers.Last().Layer.Graphics.Add(textGraphic);

                if (_layers.Last().Points.Count > 1)
                {
                    var geometryService = new GeometryService(GeometryServiceUrl);
                    geometryService.Failed += GeometryServiceFailed;
                    geometryService.ProjectCompleted += GeometryServiceFinalDistanceProjectCompleted;

                    var graphics = new List<Graphic>();
                    var graphic1 = new Graphic();
                    var graphic2 = new Graphic();
                    graphic1.Geometry = _layers.Last().PrevOrigin;
                    graphic2.Geometry = _layers.Last().EndPoint;
                    graphics.Add(graphic1);
                    graphics.Add(graphic2);
                    geometryService.ProjectAsync(graphics, new SpatialReference(MeasuredProjection));
                }
                else
                {
                    _layers.Last().TotalLength = 0;
                    SegmentLengths = new List<double>();
                }

                _isMeasuring = true;
            }
            _layers.Last().LastClick = pt;
        }

        #endregion

        #region Private Methods

        private void UpdateMeasurementUnits()
        {
            if (!_layers.Any()) return;

            int segSymbolIndex;
            int symbolIncrement;

            double totalLength;

            foreach (var layer in _layers)
            {
                if (!layer.Layer.Graphics.Any()) continue;

                totalLength = 0d;
                if (layer.MeasureMode == MeasureAction.Mode.Polygon)
                {
                    segSymbolIndex = 4;
                    symbolIncrement = 3;
                }
                else
                {
                    segSymbolIndex = 3;
                    symbolIncrement = 4;
                }

                foreach (var segmentLength in layer.SegmentLengths)
                {
                    totalLength += segmentLength;

                    var displayDistance = ConvertMetersToActiveUnits(segmentLength, DistanceUnits);

                    var segSym = layer.Layer.Graphics[segSymbolIndex].Symbol as RotatingTextSymbol;
                    if (segSym == null) break;

                    segSym.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
                    segSymbolIndex += symbolIncrement;

                    if (segSymbolIndex > layer.Layer.Graphics.Count) break;
                }

                if (layer.MeasureMode == MeasureAction.Mode.Polyline)
                {
                    var totSym = layer.Layer.Graphics[TotalDistanceSymbolPosition].Symbol as RotatingTextSymbol;
                    if (totSym == null) break;

                    var displayTotalDistance = ConvertMetersToActiveUnits(totalLength, DistanceUnits);

                    totSym.Text = string.Format("Total = {0}{1}",
                                                RoundToSignificantDigit(displayTotalDistance),
                                                PrettyUnits(DistanceUnits));
                }

                if (layer.MeasureMode == MeasureAction.Mode.Polygon)
                {
                    if (layer.Layer.Graphics.Count < 2) break;

                    var totSym = layer.Layer.Graphics[TotalAreaSymbolPosition].Symbol as RotatingTextSymbol;
                    if (totSym != null) totSym.Text = string.Format("Area = {0}{1}", RoundToSignificantDigit(ConvertUnits(layer.AreaTotal, AreaUnits)), PrettyUnits(AreaUnits));
                }
            }
        }

        private void ResetValues()
        {
            _layers.Last().MeasureMode = MeasureMode;

            _isMeasuring = false;
            _panning = false;
            _finishMeasure = false;

            _layers.Add(new SegmentLayer());
            Map.Layers.Add(_layers.Last().Layer);
        }

        private static void ResetValues(ArcFmMeasureAction action)
        {
            if (action._layers.Any())
            {
                action._layers.Last().MeasureMode = action.MeasureMode;
            }

            action._isMeasuring = false;
            action._panning = false;
            action._finishMeasure = false;
        }

        public void ClearLayers()
        {
            foreach (var layer in _layers)
            {
                layer.Layer.ClearGraphics();
                Map.Layers.Remove(layer.Layer);
            }

            _layers.Clear();
        }

        private double RoundToSignificantDigit(double value)
        {
            int roundDigits = (SignificantDigits - 2) >= 0 ? SignificantDigits - 2 : 0;

            if (value == 0) return 0;
            else if (value < .01)
            {
                roundDigits = SignificantDigits;
                while (roundDigits < 15 && Math.Round(value, ++roundDigits) == 0) ;
            }
            else if (value < 1) roundDigits = SignificantDigits; // Was 3 but changed per DE254
            else if (value < 10) roundDigits = SignificantDigits;
            else if (value < 100) roundDigits = SignificantDigits - 1;

            double round = value >= 100 ? Math.Round(value) : Math.Round(value, roundDigits);

            return round;
        }

        private static string PrettyUnits(DistanceUnit unit)
        {
            string display = string.Empty;

            switch (unit)
            {
                case DistanceUnit.Miles:
                    display = "mi";
                    break;
                case DistanceUnit.Kilometers:
                    display = "km";
                    break;
                case DistanceUnit.Feet:
                    display = "ft";
                    break;
                case DistanceUnit.Meters:
                    display = "m";
                    break;
            }

            return display;
        }

        private static string PrettyUnits(AreaUnit unit)
        {
            string display = string.Empty;

            // (Alt+0178) is the ASCII for superscript 2

            switch (unit)
            {
                case AreaUnit.SquareMiles:
                    display = "mi²";
                    break;
                case AreaUnit.SquareKilometers:
                    display = "km²";
                    break;
                case AreaUnit.SquareFeet:
                    display = "ft²";
                    break;
                case AreaUnit.SquareMeters:
                    display = "m²";
                    break;
                case AreaUnit.Acres:
                    display = "a";
                    break;
                case AreaUnit.Hectares:
                    display = "ha";
                    break;
            }

            return display;
        }

        private static double ConvertMetersToActiveUnits(double item, DistanceUnit units)
        {
            if (double.IsNaN(item)) return double.NaN;
            double convertedItem = double.NaN;

            //  Assume from meters.
            switch (units)
            {
                case DistanceUnit.Miles:
                    convertedItem = item * MetersToMiles;
                    break;
                case DistanceUnit.Kilometers:
                    convertedItem = item / 1000;
                    break;
                case DistanceUnit.Feet:
                    convertedItem = item * MetersToFeet;
                    break;
                case DistanceUnit.Meters:
                    convertedItem = item;
                    break;
                default:
                    break;
            }

            return convertedItem;


        }

        private static double ConvertUnits(double item, AreaUnit units)
        {
            if (double.IsNaN(item)) return double.NaN;
            double convertedItem = double.NaN;

            //  Assume from meters.
            switch (units)
            {
                case AreaUnit.SquareMiles:
                    convertedItem = item * SqMetersToSqMiles;
                    break;
                case AreaUnit.SquareFeet:
                    convertedItem = item * SqMetersToSqFeet;
                    break;
                case AreaUnit.SquareKilometers:
                    convertedItem = item / 1000000;
                    break;
                case AreaUnit.SquareMeters:
                    convertedItem = item;
                    break;
                case AreaUnit.Hectares:
                    convertedItem = item * SqMetersToHectare;
                    break;
                case AreaUnit.Acres:
                    convertedItem = item * SqMetersToAcre;
                    break;
                default:
                    break;
            }

            return convertedItem;
        }

        #endregion
    }

    internal class SegmentLayer
    {
        public PointCollection Points = new PointCollection();
        public MapPoint OriginPoint = null;
        public MapPoint PrevOrigin = null;
        public MapPoint EndPoint = null;
        public Point LastClick = new Point(0, 0);

        public double TempTotalLength = 0;
        public double TotalLength = 0;
        public double AreaTotal = 0;
        public int LineCount = 0;

        public GraphicsLayer Layer = new GraphicsLayer();
        public MeasureAction.Mode? MeasureMode;
        public List<double> SegmentLengths = new List<double>();
    }

    /// <summary>
    /// Extension methods for geodesic calculations.
    /// 
    /// All points must be in WGS decimal degrees. (WKID 4326)
    /// 
    /// Uses meter lengths based on the constant definition for the earths radius.
    /// </summary>
    internal static class Geodesic
    {
        // Earth's Radius Numbers
        // Equatorial radius (6,378.1370 km)
        // Polar radius (6,356.7523 km)
        // Mean radius (6,371.009 km)
        // Authalic radius (6,371.0072 km)
        //private const double EarthRadius = 6378.137; //kilometers. Change to miles to return all values in miles instead
        private const double EarthRadius = 6378137; // Meters. Change to other units to get all values in that unit
        private const int WgsId = 4326;

        /// <summary>
        /// Gets the distance between two points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static double GetSphericalDistance(this MapPoint start, MapPoint end)
        {
            if (start.SpatialReference.WKID != WgsId) return double.NaN;
            if (end.SpatialReference.WKID != WgsId) return double.NaN;

            // Convert to radians
            double lonS = start.X / 180 * Math.PI;
            double lonF = end.X / 180 * Math.PI;
            double latS = start.Y / 180 * Math.PI;
            double latF = end.Y / 180 * Math.PI;

            double dLon = lonS - lonF;

            // Less precise and with more opportunity for error (however slight that opportunity may be)
            //double dAngle = 2 * Math.Asin(Math.Sqrt(Math.Pow((Math.Sin((latS - latF) / 2)), 2) + Math.Cos(latS) * Math.Cos(latF) * Math.Pow(Math.Sin((lonS - lonF) / 2), 2)));

            double dAngle = Math.Atan2((Math.Sqrt(Math.Pow(Math.Cos(latF) * Math.Sin(dLon), 2) + Math.Pow(Math.Cos(latS) * Math.Sin(latF) - Math.Sin(latS) * Math.Cos(latF) * Math.Cos(dLon), 2))), (Math.Sin(latS) * Math.Sin(latF) + Math.Cos(latS) * Math.Cos(latF) * Math.Cos(dLon)));
            double dist = EarthRadius * dAngle;

            return dist;
        }

        /// <summary>
        /// Returns a polygon with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="dist">Radius</param>
        /// <returns></returns>
        public static Polygon GetRadiusAsPolygon(this MapPoint center, double dist)
        {
            if (center.SpatialReference.WKID != WgsId) return new Polygon();
            if (double.IsNaN(dist)) return new Polygon();

            Polyline line = GetRadius(center, dist);
            var poly = new Polygon();

            if (line.Paths.Count > 1)
            {
                PointCollection ring = line.Paths[0];
                MapPoint last = ring[ring.Count - 1];
                for (int i = 1; i < line.Paths.Count; i++)
                {
                    PointCollection pnts = line.Paths[i];
                    ring.Add(new MapPoint(180 * Math.Sign(last.X), 90 * Math.Sign(center.Y), new SpatialReference(WgsId)));
                    last = pnts[0];
                    ring.Add(new MapPoint(180 * Math.Sign(last.X), 90 * Math.Sign(center.Y), new SpatialReference(WgsId)));
                    foreach (MapPoint p in pnts)
                        ring.Add(p);
                    last = pnts[pnts.Count - 1];
                }
                poly.Rings.Add(ring);
                //pnts.Add(first);
            }
            else
            {
                poly.Rings.Add(line.Paths[0]);
            }
            if (dist > EarthRadius * Math.PI / 2 && line.Paths.Count != 2)
            {
                var pnts = new PointCollection
				               {
				                   new MapPoint(-180, -90, new SpatialReference(WgsId)),
				                   new MapPoint(180, -90, new SpatialReference(WgsId)),
				                   new MapPoint(180, 90, new SpatialReference(WgsId)),
				                   new MapPoint(-180, 90, new SpatialReference(WgsId)),
				                   new MapPoint(-180, -90, new SpatialReference(WgsId))
				               };
                poly.Rings.Add(pnts); //Exterior
            }

            return poly;
        }

        /// <summary>
        /// Returns a polyline with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="dist">Radius</param>
        /// <returns></returns>
        public static Polyline GetRadius(this MapPoint center, double dist)
        {
            if (center.SpatialReference.WKID != WgsId) return new Polyline();
            if (double.IsNaN(dist)) return new Polyline();

            var line = new Polyline();
            var pnts = new PointCollection();
            line.Paths.Add(pnts);
            for (int i = 0; i < 360; i++)
            {
                //double angle = i / 180.0 * Math.PI;
                MapPoint p = GetPointFromHeading(center, dist, i);
                if (pnts.Count > 0)
                {
                    MapPoint lastPoint = pnts[pnts.Count - 1];
                    int sign = Math.Sign(p.X);
                    if (Math.Abs(p.X - lastPoint.X) > 180)
                    {
                        //We crossed the date line
                        double lat = LatitudeAtLongitude(lastPoint, p, sign * -180);
                        pnts.Add(new MapPoint(sign * -180, lat, new SpatialReference(WgsId)));
                        pnts = new PointCollection();
                        line.Paths.Add(pnts);
                        pnts.Add(new MapPoint(sign * 180, lat, new SpatialReference(WgsId)));
                    }
                }
                pnts.Add(p);
            }
            pnts.Add(line.Paths[0][0]);

            return line;
        }

        /// <summary>
        /// Gets the shortest path line between two points. The line will be following the great
        /// circle described by the two points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polyline GetGeodesicLine(this MapPoint start, MapPoint end)
        {
            if (start.SpatialReference.WKID != WgsId) return new Polyline();
            if (end.SpatialReference.WKID != WgsId) return new Polyline();

            var line = new Polyline();
            if (Math.Abs(end.X - start.X) <= 180) // Doesn't cross dateline 
            {
                PointCollection pnts = GetGeodesicPoints(start, end);
                line.Paths.Add(pnts);
            }
            else
            {
                double lon1 = start.X / 180 * Math.PI;
                double lon2 = end.X / 180 * Math.PI;
                double lat1 = start.Y / 180 * Math.PI;
                double lat2 = end.Y / 180 * Math.PI;
                double latA = LatitudeAtLongitude(lat1, lon1, lat2, lon2, Math.PI) / Math.PI * 180;
                //double latB = LatitudeAtLongitude(lat1, lon1, lat2, lon2, -180) / Math.PI * 180;

                line.Paths.Add(GetGeodesicPoints(start, new MapPoint(start.X < 0 ? -180 : 180, latA, new SpatialReference(WgsId))));
                line.Paths.Add(GetGeodesicPoints(new MapPoint(start.X < 0 ? 180 : -180, latA, new SpatialReference(WgsId)), end));
            }

            return line;
        }

        /// <summary>
        /// Returns a distance estimate between two points.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static double DistanceTo(this MapPoint start, MapPoint end)
        {
            if ((start.SpatialReference.WKID == WgsId) && (end.SpatialReference.WKID == WgsId)) return start.GetSphericalDistance(end);

            double x = Math.Abs(start.X - end.X);
            double y = Math.Abs(start.Y - end.Y);
            double dist = Math.Sqrt((x * x) + (y * y));

            return dist;
        }

        /// <summary>
        /// Returns a polyline with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polyline GetQuickCircleLine(this MapPoint start, MapPoint end)
        {
            var line = new Polyline();
            var pnts = new PointCollection();
            line.Paths.Add(pnts);
            line.SpatialReference = start.SpatialReference;

            double radius = Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));

            for (int i = 0; i < 359; i++)
            {
                double degree = i * (Math.PI / 180);
                double x = start.X + Math.Cos(degree) * radius;
                double y = start.Y + Math.Sin(degree) * radius;

                var p = new MapPoint(x, y, start.SpatialReference);
                pnts.Add(p);
            }
            pnts.Add(line.Paths[0][0]);

            return line;
        }

        /// <summary>
        /// Returns a polygon with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polygon GetQuickCirclePolygon(this MapPoint start, MapPoint end)
        {
            var poly = new Polygon();
            var pnts = new PointCollection();
            poly.Rings.Add(pnts);
            poly.SpatialReference = start.SpatialReference;

            double radius = Math.Sqrt(Math.Pow(start.X - end.X, 2) + Math.Pow(start.Y - end.Y, 2));

            for (int i = 0; i < 359; i++)
            {
                double degree = i * (Math.PI / 180);
                double x = start.X + Math.Cos(degree) * radius;
                double y = start.Y + Math.Sin(degree) * radius;

                var p = new MapPoint(x, y, start.SpatialReference);
                pnts.Add(p);
            }
            //pnts.Add(poly.Rings[0][0]);

            return poly;
        }

        /// <summary>
        /// Returns a polygon with a constant distance from the center point measured on the sphere.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <returns></returns>
        public static Polygon GetQuickOvalPolygon(this MapPoint start, MapPoint end)
        {
            var poly = new Polygon();
            var pnts = new PointCollection();
            poly.Rings.Add(pnts);
            poly.SpatialReference = start.SpatialReference;

            double radiusX = start.X - end.X;
            double radiusY = start.Y - end.Y;

            for (int i = 0; i < 359; i++)
            {
                double degree = i * (Math.PI / 180);
                double x = start.X + Math.Cos(degree) * radiusX;
                double y = start.Y + Math.Sin(degree) * radiusY;

                var p = new MapPoint(x, y, start.SpatialReference);
                pnts.Add(p);
            }
            pnts.Add(poly.Rings[0][0]);

            return poly;
        }

        /// <summary>
        /// Gets the true bearing at a distance from the start point towards the new point.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The point to get the bearing towards.</param>
        /// <param name="distance">The distance travelled between start and end.</param>
        /// <returns></returns>
        public static double GetTrueBearing(MapPoint start, MapPoint end, double distance)
        {
            if (start.SpatialReference.WKID != WgsId) return double.NaN;
            if (end.SpatialReference.WKID != WgsId) return double.NaN;

            double d = distance / EarthRadius; //Angular distance in radians
            double lon1 = start.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            double tc1;
            if (Math.Sin(lon2 - lon1) < 0)
                tc1 = Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) / (Math.Sin(d) * Math.Cos(lat1)));
            else
                tc1 = 2 * Math.PI - Math.Acos((Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(d)) / (Math.Sin(d) * Math.Cos(lat1)));
            return tc1 / Math.PI * 180;
        }

        /// <summary>
        /// Gets the point based on a start point, a heading and a distance.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="distance">The distance.</param>
        /// <param name="heading">The heading.</param>
        /// <returns></returns>
        public static MapPoint GetPointFromHeading(MapPoint start, double distance, double heading)
        {
            if (start.SpatialReference.WKID != WgsId) return new MapPoint();

            double brng = heading / 180 * Math.PI;
            double lon1 = start.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double dR = distance / EarthRadius; //Angular distance in radians
            double lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(dR) + Math.Cos(lat1) * Math.Sin(dR) * Math.Cos(brng));
            double lon2 = lon1 + Math.Atan2(Math.Sin(brng) * Math.Sin(dR) * Math.Cos(lat1), Math.Cos(dR) - Math.Sin(lat1) * Math.Sin(lat2));
            double lon = lon2 / Math.PI * 180;
            double lat = lat2 / Math.PI * 180;
            while (lon < -180) lon += 360;
            while (lat < -90) lat += 180;
            while (lon > 180) lon -= 360;
            while (lat > 90) lat -= 180;

            return new MapPoint(lon, lat, new SpatialReference(WgsId));
        }

        private static PointCollection GetGeodesicPoints(MapPoint start, MapPoint end)
        {
            double lon1 = start.X / 180 * Math.PI;
            double lon2 = end.X / 180 * Math.PI;
            double lat1 = start.Y / 180 * Math.PI;
            double lat2 = end.Y / 180 * Math.PI;
            double dX = end.X - start.X;
            var points = (int)Math.Floor(Math.Abs(dX));
            dX = lon2 - lon1;
            var pnts = new PointCollection { start };
            for (int i = 1; i < points; i++)
            {
                double lon = lon1 + dX / points * i;
                double lat = LatitudeAtLongitude(lat1, lon1, lat2, lon2, lon);
                pnts.Add(new MapPoint(lon / Math.PI * 180, lat / Math.PI * 180, new SpatialReference(WgsId)));
            }
            pnts.Add(end);
            return pnts;
        }

        /// <summary>
        /// Gets the latitude at a specific longitude for a great circle defined by p1 and p2.
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="lon">The longitude in degrees.</param>
        /// <returns></returns>
        private static double LatitudeAtLongitude(MapPoint p1, MapPoint p2, double lon)
        {
            double lon1 = p1.X / 180 * Math.PI;
            double lon2 = p2.X / 180 * Math.PI;
            double lat1 = p1.Y / 180 * Math.PI;
            double lat2 = p2.Y / 180 * Math.PI;
            lon = lon / 180 * Math.PI;
            return LatitudeAtLongitude(lat1, lon1, lat2, lon2, lon) / Math.PI * 180;
        }

        /// <summary>
        /// Gets the latitude at a specific longitude for a great circle defined by lat1,lon1 and lat2,lon2.
        /// </summary>
        /// <param name="lat1">The start latitude in radians.</param>
        /// <param name="lon1">The start longitude in radians.</param>
        /// <param name="lat2">The end latitude in radians.</param>
        /// <param name="lon2">The end longitude in radians.</param>
        /// <param name="lon">The longitude in radians for where the latitude is.</param>
        /// <returns></returns>
        private static double LatitudeAtLongitude(double lat1, double lon1, double lat2, double lon2, double lon)
        {
            return Math.Atan((Math.Sin(lat1) * Math.Cos(lat2) * Math.Sin(lon - lon2) - Math.Sin(lat2) * Math.Cos(lat1) * Math.Sin(lon - lon1)) / (Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(lon1 - lon2)));
        }
    }

    internal class RadiusDistance : DependencyObject
    {
        private const int WgsId = 4326;

        #region Dependency Properties

        public static readonly DependencyProperty MapProperty =
            DependencyProperty.Register("Map", typeof(Map), typeof(RadiusDistance),
                                        new PropertyMetadata(null, OnMapPropertyChanged));

        [Category("Radius Distance Properties")]
        [Description("Map Control")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        public static readonly DependencyProperty GeometryServiceUrlProperty =
            DependencyProperty.Register("GeometryServiceUrl", typeof(string), typeof(RadiusDistance), null);

        [Category("Radius Distance Properties")]
        [Description("Geometry Service URL")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string GeometryServiceUrl
        {
            get { return (string)GetValue(GeometryServiceUrlProperty); }
            set { SetValue(GeometryServiceUrlProperty, value); }
        }

        public static readonly DependencyProperty DistanceUnitsProperty =
            DependencyProperty.Register("DistanceUnits", typeof(DistanceUnit), typeof(RadiusDistance),
                                        new PropertyMetadata(DistanceUnitsChanged));

        [Category("Radius Distance Properties")]
        [Description("Distance Units")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DistanceUnit DistanceUnits
        {
            get { return (DistanceUnit)GetValue(DistanceUnitsProperty); }
            set { SetValue(DistanceUnitsProperty, value); }
        }

        public static readonly DependencyProperty SignificantDigitsProperty =
            DependencyProperty.Register("SignificantDigits", typeof(int), typeof(RadiusDistance),
                                        new PropertyMetadata(2));

        [Category("Radius Distance Properties")]
        [Description("Significant Digits")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SignificantDigits
        {
            get { return (int)GetValue(SignificantDigitsProperty); }
            set { SetValue(SignificantDigitsProperty, value); }
        }

        public static readonly DependencyProperty IsActivatedProperty =
            DependencyProperty.Register("IsActivated", typeof(bool), typeof(RadiusDistance),
                                        new PropertyMetadata(false, OnIsActivatedPropertyChanged));

        public bool IsActivated
        {
            get { return (bool)GetValue(IsActivatedProperty); }
            set { SetValue(IsActivatedProperty, value); }
        }

        public static readonly DependencyProperty RadiusLengthProperty =
            DependencyProperty.Register("RadiusLength", typeof(double), typeof(RadiusDistance),
                                        new PropertyMetadata(0.0));

        [Category("Radius Distance Properties")]
        [Description("Radius Length")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public double RadiusLength
        {
            get { return (double)GetValue(RadiusLengthProperty); }
            set { SetValue(RadiusLengthProperty, value); }
        }

        #endregion

        private List<RadiusLayer> _layers = new List<RadiusLayer>();

        private int _lengthAsync;

        private bool _finalLinesInProg;
        private bool _finalCircleInProg;

        private bool _isMeasuring;
        private bool _panning;

        #region Constructor

        public RadiusDistance()
        {
            _lengthAsync = 0;
            _finalLinesInProg = false;
            _finalCircleInProg = false;
        }

        #endregion

        private static void OnMapPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gd = d as RadiusDistance;
            var oldMap = e.OldValue as Map;
            if (gd == null) return;
            bool isActive = gd.IsActivated;
            if (isActive)
                gd.IsActivated = false;
            if (oldMap != null)
            {
                foreach (var layer in gd._layers)
                {
                    layer.Layer.ClearGraphics();
                    gd.Map.Layers.Remove(layer.Layer);
                }

                gd._layers.Clear();
            }
            gd.IsActivated = isActive;
        }

        private static void OnIsActivatedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var gd = d as RadiusDistance;
            var oldValue = (bool)e.OldValue;
            var newValue = (bool)e.NewValue;
            if (newValue == oldValue) return;
            if (gd != null)
                if (gd.Map != null)
                {
                    if (newValue)
                    {
                        gd.Map.MouseMove += gd.MapMouseMove;
                        gd.Map.MouseMove += gd.MapMouseMovePan;
                        gd.Map.MouseLeftButtonDown += gd.MapMouseClick;
                        InitializeGraphicsLayer(gd);
                    }
                    else
                    {
                        gd.Map.MouseMove -= gd.MapMouseMove;
                        gd.Map.MouseMove -= gd.MapMouseMovePan;
                        gd.Map.MouseLeftButtonDown -= gd.MapMouseClick;
                        gd._isMeasuring = false;
                        gd._panning = false;
                        gd.RadiusLength = 0.0;

                        if (gd._layers.Any())
                        {
                            gd.Map.Layers.Remove(gd._layers.Last().Layer);
                            gd._layers.Remove(gd._layers.Last());
                        }
                    }
                }
        }

        private void MapMouseMovePan(object sender, MouseEventArgs e)
        {
            const double percentBuffer = .15;
            const double panFactor = .04;

            if (!_isMeasuring) return;

            var map = sender as Map;
            if (map == null) return;

            // Get the current mouse position.
            var mouseLocation = map.ScreenToMap(e.GetPosition(map));

            var mX = mouseLocation.X;
            var mY = mouseLocation.Y;

            double percentX = 1.0;
            double percentY = 1.0;

            var innerExtent = new Envelope
            {
                XMax =
                    map.Extent.XMax -
                    ((map.Extent.XMax - map.Extent.XMin) * percentBuffer),
                YMax =
                    map.Extent.YMax -
                    ((map.Extent.YMax - map.Extent.YMin) * percentBuffer),
                XMin =
                    map.Extent.XMin +
                    ((map.Extent.XMax - map.Extent.XMin) * percentBuffer),
                YMin =
                    map.Extent.YMin +
                    ((map.Extent.YMax - map.Extent.YMin) * percentBuffer)
            };

            if ((((mX >= innerExtent.XMin) && (mX <= innerExtent.XMax)) &&
                 (mY >= innerExtent.YMin)) && (mY <= innerExtent.YMax))
            {
                _panning = false;
                return;
            }

            _panning = true;

            if (mX < innerExtent.XMin)
            {
                double amount = innerExtent.XMin - mX;
                percentX = amount / (innerExtent.XMin - map.Extent.XMin);
            }
            else if (mX > innerExtent.XMax)
            {
                double amount = mX - innerExtent.XMax;
                percentX = amount / (map.Extent.XMax - innerExtent.XMax);
            }

            if (mY < innerExtent.YMin)
            {
                double amount = innerExtent.YMin - mY;
                percentY = amount / (innerExtent.YMin - map.Extent.YMin);
            }
            else if (mY > innerExtent.YMax)
            {
                double amount = mY - innerExtent.YMax;
                percentY = amount / (map.Extent.YMax - innerExtent.YMax);
            }

            MapPoint centerPoint = map.Extent.GetCenter();

            if (mX < innerExtent.XMin) centerPoint.X -= (map.Extent.Width * panFactor * percentX * percentX);
            else if (mX > innerExtent.XMax) centerPoint.X += (map.Extent.Width * panFactor * percentX * percentX);

            if (mY < innerExtent.YMin) centerPoint.Y -= (map.Extent.Height * panFactor * percentY * percentY);
            else if (mY > innerExtent.YMax) centerPoint.Y += (map.Extent.Height * panFactor * percentY * percentY);

            map.PanTo(centerPoint);
        }

        private void MapMouseClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            if (_panning) return;

            var map = sender as Map;
            if (map == null) return;

            if (_finalLinesInProg || _finalCircleInProg) return;

            if (!_layers.Any())
            {
                InitializeGraphicsLayer(this);
            }

            if (_layers.Last().Origin == null)
            {
                _isMeasuring = true;
                _layers.Last().Origin = map.ScreenToMap(e.GetPosition(map));
                _layers.Last().MidMarker.Geometry = _layers.Last().Origin;

                var line = _layers.Last().StraightLine.Geometry as Polyline;
                if (line == null) return;
                line.Paths[0][0] = _layers.Last().Origin;
                line.Paths[0][1] = _layers.Last().Origin;
                _layers.Last().TextSymb.Text = string.Empty;
                _layers.Last().GreatCircleLine.Geometry = null;
                _layers.Last().RadiusLine.Geometry = null;
                _layers.Last().RadiusFill.Geometry = null;
            }
            else
            {
                // If not WGS, get actual geodetic line and circle using final point.
                if (map.SpatialReference.WKID != WgsId)
                {
                    var p = map.ScreenToMap(e.GetPosition(map));

                    var geometryService = new GeometryService(GeometryServiceUrl);
                    geometryService.Failed += GeometryServiceFailed;
                    geometryService.ProjectCompleted += GeometryServicFinalProjectCompleted;

                    var graphic1 = new Graphic { Symbol = new SimpleMarkerSymbol(), Geometry = p };
                    var graphic2 = new Graphic { Symbol = new SimpleMarkerSymbol(), Geometry = _layers.Last().Origin };
                    var graphicList = new List<Graphic> { graphic1, graphic2 };

                    _finalLinesInProg = true;
                    _finalCircleInProg = true;
                    geometryService.ProjectAsync(graphicList, new SpatialReference(WgsId));
                }

                InitializeGraphicsLayer(this);
                _isMeasuring = false;
            }
        }

        private void MapMouseMove(object sender, MouseEventArgs e)
        {
            if (!_layers.Any() || _layers.Last().Origin == null) return;

            var map = sender as Map;
            if (map == null) return;

            MapPoint p = map.ScreenToMap(e.GetPosition(map));
            if (p == null) return;

            SpatialReference spatial = map.SpatialReference;

            var midpoint = new MapPoint((p.X + _layers.Last().Origin.X) / 2, (p.Y + _layers.Last().Origin.Y) / 2, spatial);

            _layers.Last().TextPoint.Geometry = midpoint;
            _layers.Last().MidMarker.Geometry = midpoint;

            var line = _layers.Last().StraightLine.Geometry as Polyline;
            if (line == null) return;

            line.Paths[0][1] = p;

            double angle = Math.Atan2((p.X - _layers.Last().Origin.X), (p.Y - _layers.Last().Origin.Y)) / Math.PI * 180 - 90;
            if (angle > 90 || angle < -90) angle -= 180;
            _layers.Last().TextSymb.Angle = angle;

            if (map.SpatialReference.WKID != WgsId)
            {
                // We could make some assumptions here and assume a measured spatial reference.
                // In which case, we only need to calculate and we don't need to make any calls
                // to a geometry service.
                double dist = _layers.Last().Origin.DistanceTo(p);

                _layers.Last().GreatCircleLine.Geometry = null;
                _layers.Last().RadiusLine.Geometry = _layers.Last().Origin.GetQuickCircleLine(p);
                _layers.Last().RadiusFill.Geometry = _layers.Last().Origin.GetQuickCirclePolygon(p);

                string mapUnits = null;
                foreach (var layer in map.Layers)
                {
                    if (layer is ArcGISTiledMapServiceLayer)
                    {
                        mapUnits = (layer as ArcGISTiledMapServiceLayer).Units;

                        break;
                    }
                    else if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        mapUnits = (layer as ArcGISDynamicMapServiceLayer).Units;

                        break;
                    }
                }

                if (string.IsNullOrEmpty(mapUnits)) return;

                if (mapUnits == "esriFeet")
                {
                    // Convert to meters.
                    const double metersToFeet = 3.280839895;
                    dist = dist / metersToFeet;
                }
                else if (mapUnits != "esriMeters") return;

                var displayDistance = ConvertMetersToActiveUnits(dist);

                _layers.Last().TextSymb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
            }
            else
            {
                double dist = _layers.Last().Origin.GetSphericalDistance(p);

                _layers.Last().GreatCircleLine.Geometry = _layers.Last().Origin.GetGeodesicLine(p);
                _layers.Last().RadiusLine.Geometry = _layers.Last().Origin.GetRadius(dist);
                _layers.Last().RadiusFill.Geometry = _layers.Last().Origin.GetRadiusAsPolygon(dist);

                var displayDistance = ConvertMetersToActiveUnits(dist);

                _layers.Last().TextSymb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
            }
        }

        private static void DistanceUnitsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var radius = d as RadiusDistance;
            if (radius == null) return;

            foreach (var layer in radius._layers)
            {
                layer.TextSymb.Text = string.Format("{0}{1}", radius.RoundToSignificantDigit(radius.ConvertMetersToActiveUnits(layer.RadiusLength)), PrettyUnits(radius.DistanceUnits));
            }
        }

        private void GeometryServiceMouseMoveDistanceCompleted(object sender, DistanceEventArgs e)
        {
            _lengthAsync--;

            var displayDistance = ConvertMetersToActiveUnits(e.Distance);

            _layers.Last().TextSymb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
        }

        private void GeometryServicFinalProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var wgsP = e.Results[0].Geometry as MapPoint;
            if (wgsP == null) return;

            var wgsOrigin = e.Results[1].Geometry as MapPoint;
            if (wgsOrigin == null) return;

            double dist = wgsOrigin.GetSphericalDistance(wgsP);

            int layerIndex = 0;
            for (; layerIndex < _layers.Count; layerIndex++)
            {
                if (_layers[layerIndex].RadiusLength == 0 && _layers[layerIndex].Origin != null)
                {
                    break;
                }
            }

            RadiusLength = dist;
            _layers[layerIndex].RadiusLength = dist;

            var displayDistance = ConvertMetersToActiveUnits(dist);

            _layers[layerIndex].TextSymb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));

            var geometryServiceLines = new GeometryService(GeometryServiceUrl);
            geometryServiceLines.Failed += GeometryServiceFailed;
            geometryServiceLines.ProjectCompleted += GeometryServiceGeo1ProjectCompleted;

            var geometryServicePolygons = new GeometryService(GeometryServiceUrl);
            geometryServicePolygons.Failed += GeometryServiceFailed;
            geometryServicePolygons.ProjectCompleted += GeometryServiceGeo2ProjectCompleted;

            var circleGeometry = wgsOrigin.GetGeodesicLine(wgsP);
            circleGeometry.SpatialReference = new SpatialReference(WgsId);

            var radiusLineGeometry = wgsOrigin.GetRadius(dist);
            radiusLineGeometry.SpatialReference = new SpatialReference(WgsId);

            var radiusFillGeometry = wgsOrigin.GetRadiusAsPolygon(dist);
            radiusFillGeometry.SpatialReference = new SpatialReference(WgsId);

            var circleLineGraphic = new Graphic { Symbol = new SimpleLineSymbol(Color.FromArgb(0x99, 255, 0, 0), 4), Geometry = circleGeometry };
            var radiusLineGraphic = new Graphic { Symbol = new SimpleLineSymbol(Color.FromArgb(0xff, 255, 255, 0), 1), Geometry = radiusLineGeometry };
            var radiusFillGraphic = new Graphic { Symbol = new SimpleFillSymbol { Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.5 }, BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), BorderThickness = 2 }, Geometry = radiusFillGeometry };

            var graphicList1 = new List<Graphic> { circleLineGraphic, radiusLineGraphic };
            var graphicList2 = new List<Graphic> { radiusFillGraphic };

            geometryServiceLines.ProjectAsync(graphicList1, Map.SpatialReference);
            geometryServicePolygons.ProjectAsync(graphicList2, Map.SpatialReference);
        }

        private void GeometryServiceMouseMoveProjectCompleted(object sender, GraphicsEventArgs e)
        {
            _lengthAsync--;

            var wgsP = e.Results[0].Geometry as MapPoint;
            if (wgsP == null) return;

            var wgsOrigin = e.Results[1].Geometry as MapPoint;
            if (wgsOrigin == null) return;

            double dist = wgsOrigin.GetSphericalDistance(wgsP);

            var displayDistance = ConvertMetersToActiveUnits(dist);

            _layers.Last().TextSymb.Text = string.Format("{0}{1}", RoundToSignificantDigit(displayDistance), PrettyUnits(DistanceUnits));
        }


        private void GeometryServiceGeo1ProjectCompleted(object sender, GraphicsEventArgs e)
        {
            _finalLinesInProg = false;
        }

        private void GeometryServiceGeo2ProjectCompleted(object sender, GraphicsEventArgs e)
        {
            _finalCircleInProg = false;
        }

        private static void GeometryServiceFailed(object sender, TaskFailedEventArgs e)
        {
            MessageBox.Show(string.Format("The Geometry Service reported an error. {0}", e.Error));
        }

        private double RoundToSignificantDigit(double value)
        {
            int roundDigits = (SignificantDigits - 2) >= 0 ? SignificantDigits - 2 : 0;

            if (value == 0) return 0;
            else if (value < .01)
            {
                roundDigits = SignificantDigits;
                while (roundDigits < 15 && Math.Round(value, ++roundDigits) == 0) ;
            }
            else if (value < 1) roundDigits = SignificantDigits; // Was 3 but changed per DE254
            else if (value < 10) roundDigits = SignificantDigits;
            else if (value < 100) roundDigits = SignificantDigits - 1;

            double round = value >= 100 ? Math.Round(value) : Math.Round(value, roundDigits);

            return round;
        }

        private static void InitializeGraphicsLayer(RadiusDistance gd)
        {
            if (gd == null) return;

            //Create graphics layer and populate with the needed necessary graphics
            var layer = new RadiusLayer();

            layer.RadiusFill.Geometry = null;
            layer.Layer.Graphics.Add(layer.RadiusFill);

            layer.GreatCircleLine.Geometry = null;
            layer.Layer.Graphics.Add(layer.GreatCircleLine);

            layer.RadiusLine.Geometry = null;
            layer.Layer.Graphics.Add(layer.RadiusLine);

            var line = new Polyline();
            var pnts = new PointCollection { new MapPoint(), new MapPoint() };
            line.Paths.Add(pnts);
            layer.StraightLine.Geometry = line;
            layer.Layer.Graphics.Add(layer.StraightLine);

            layer.MidMarker.Geometry = null;
            layer.Layer.Graphics.Add(layer.MidMarker);

            layer.TextPoint = new Graphic { Symbol = layer.TextSymb };
            layer.TextPoint.SetZIndex(2);
            layer.Layer.Graphics.Add(layer.TextPoint);

            gd._layers.Add(layer);
            gd.Map.Layers.Add(layer.Layer);
        }

        private static string PrettyUnits(DistanceUnit unit)
        {
            string display = string.Empty;

            switch (unit)
            {
                case DistanceUnit.Miles:
                    display = "mi";
                    break;
                case DistanceUnit.Kilometers:
                    display = "km";
                    break;
                case DistanceUnit.Feet:
                    display = "ft";
                    break;
                case DistanceUnit.Meters:
                    display = "m";
                    break;
            }

            return display;
        }

        private double ConvertMetersToActiveUnits(double dist)
        {
            const double metersToMiles = 0.0006213700922;
            const double metersToFeet = 3.280839895;

            double displayMeasurement = dist;

            // Assume from meters.
            switch (DistanceUnits)
            {
                case DistanceUnit.Miles:
                    displayMeasurement = dist * metersToMiles;
                    break;
                case DistanceUnit.Kilometers:
                    displayMeasurement = dist / 1000;
                    break;
                case DistanceUnit.Feet:
                    displayMeasurement = dist * metersToFeet;
                    break;
                case DistanceUnit.Meters:
                    displayMeasurement = dist;
                    break;
            }

            return displayMeasurement;
        }

        public void ClearLayers()
        {
            foreach (var layer in _layers)
            {
                layer.Layer.ClearGraphics();
                Map.Layers.Remove(layer.Layer);
            }

            _layers.Clear();
        }

        ///// <summary>
        ///// Converts UTM coords to lat/long.  Equations from USGS Bulletin 1532 
        ///// East Longitudes are positive, West longitudes are negative. 
        ///// North latitudes are positive, South latitudes are negative
        ///// Lat and Long are in decimal degrees.  
        ///// </summary>
        ///// <param name="utmNorthing">Northing coordinate</param>
        ///// <param name="utmEasting">Easting coordinate</param>
        ///// <param name="zoneNumber">the zone number</param>
        ///// <param name="latitude">converted latitude</param>
        ///// <param name="longitude">converted longitude</param>
        //private static void UtmToLatLong(double utmEasting, double utmNorthing, int zoneNumber, out double longitude, out double latitude)
        //{
        //    const double pi = Math.PI;
        //    const double rad2Deg = 180.0 / pi;
        //    const double deg2Rad = pi / 180.0;

        //    const double k0 = 0.9996; // Scale along long0 (constant)
        //    const double a = 6378137; // Equatorial Radius for WGS-84
        //    const double b = 6356752.3142; // Polar Radius for WGS-84
        //    //const double eccSquared = 0.00669438; // Square of Eccentricity for WGS-84

        //    double ecc = Math.Sqrt(1.0 - (b * b) / (a * a));
        //    double eccSquared = (ecc * ecc);
        //    double eccPrimeSquared = (eccSquared) / (1 - eccSquared);

        //    double e1 = (1 - Math.Sqrt(1 - eccSquared)) / (1 + Math.Sqrt(1 - eccSquared));

        //    double x = utmEasting - 500000.0; // Relative to central meridian.
        //    double y = utmNorthing;

        //    double longOdeg;

        //    if (zoneNumber <= 30) longOdeg = (30.0 - zoneNumber) * -6.0 - 3.0;
        //    else longOdeg = (zoneNumber - 31.0) * 6.0 + 3.0;

        //    double longOrad = longOdeg * deg2Rad;

        //    double m = y / k0;
        //    double mu = m / (a * (1 - eccSquared / 4 - 3 * eccSquared * eccSquared / 64 - 5 * eccSquared * eccSquared * eccSquared / 256));

        //    double j1 = (3 * e1 / 2 - 27 * e1 * e1 * e1 / 32);
        //    double j2 = (21 * e1 * e1 / 16 - 55 * e1 * e1 * e1 * e1 / 32);
        //    double j3 = (151 * e1 * e1 * e1 / 96);
        //    double j4 = (1097 * e1 * e1 * e1 * e1 / 512);

        //    double fp = mu + j1 * Math.Sin(2 * mu)
        //                     + j2 * Math.Sin(4 * mu)
        //                     + j3 * Math.Sin(6 * mu)
        //                     + j4 * Math.Sin(8 * mu);

        //    double n1 = a / Math.Sqrt(1 - eccSquared * Math.Sin(fp) * Math.Sin(fp));
        //    double t1 = Math.Tan(fp) * Math.Tan(fp);
        //    double c1 = eccPrimeSquared * Math.Cos(fp) * Math.Cos(fp);
        //    double r1 = a * (1 - eccSquared) / Math.Pow(1 - eccSquared * Math.Sin(fp) * Math.Sin(fp), 1.5);
        //    double d = x / (n1 * k0);

        //    double q1 = (n1 * Math.Tan(fp) / r1);
        //    double q2 = (d * d / 2);
        //    double q3 = ((5 + 3 * t1 + 10 * c1 - 4 * c1 * c1 - 9 * eccPrimeSquared) * d * d * d * d / 24);
        //    double q4 = ((61 + 90 * t1 + 298 * c1 + 45 * t1 * t1 - 3 * c1 * c1 - 252 * eccPrimeSquared) * d * d * d * d * d * d / 720);
        //    double q5 = d;
        //    double q6 = ((1 + 2 * t1 + c1) * d * d * d / 6);
        //    double q7 = ((5 - 2 * c1 + 28 * t1 - 3 * c1 * c1 + 8 * eccPrimeSquared + 24 * t1 * t1) * d * d * d * d * d / 120);

        //    latitude = fp - q1 * (q2 - q3 + q4);
        //    latitude = latitude * rad2Deg;

        //    longitude = longOrad + (q5 - q6 + q7) / Math.Cos(fp);
        //    longitude = longitude * rad2Deg;
        //}
    }

    internal class RadiusLayer
    {
        public readonly Graphic GreatCircleLine = new Graphic { Symbol = new SimpleLineSymbol { Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), Width = 2, Style = SimpleLineSymbol.LineStyle.Solid } };
        public readonly Graphic MidMarker = new Graphic { Symbol = new SimpleMarkerSymbol { Color = new SolidColorBrush(Color.FromArgb(0x66, 255, 0, 0)), Size = 5 } };
        public readonly Graphic RadiusFill = new Graphic { Symbol = new SimpleFillSymbol { Fill = new SolidColorBrush(Colors.Red) { Opacity = 0.5 }, BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), BorderThickness = 2 } };
        public readonly Graphic RadiusLine = new Graphic { Symbol = new SimpleLineSymbol { Color = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)), Width = 2, Style = SimpleLineSymbol.LineStyle.Solid } };
        public readonly Graphic StraightLine = new Graphic { Symbol = new SimpleLineSymbol(Color.FromArgb(0x99, 0, 0, 0), 1), };
        public readonly RotatingTextSymbol TextSymb = new RotatingTextSymbol();

        public GraphicsLayer Layer = new GraphicsLayer();
        public Graphic TextPoint;

        public MapPoint Origin = null;
        public double RadiusLength;
    }

    #endregion

    #endregion

    #region Symbols

    public sealed class RotatingTextSymbol : MarkerSymbol
    {
        private const int Width = 300;  // Match to grid screenWidth in control template below.
        private string Template =
            @"<ControlTemplate
					xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" 
					xmlns:localBob=""clr-namespace:{0};assembly={1}"">
						<Grid RenderTransformOrigin=""0.5,0.5"" Width=""300"" 
							  localBob:RotatingTextSymbol.AngleBinder=""{{Binding Path=Symbol.Angle}}"">
							<TextBlock FontWeight=""Bold"" Foreground=""White"" VerticalAlignment=""Center"" HorizontalAlignment=""Center"" 
								Text=""{{Binding Symbol.Text}}"" >
								<TextBlock.Effect><BlurEffect Radius=""5"" /></TextBlock.Effect>
							</TextBlock>
							<TextBlock FontWeight=""Bold"" HorizontalAlignment=""Center"" VerticalAlignment=""Center""
								Text=""{{Binding Symbol.Text}}"" />
						</Grid>
					</ControlTemplate>";


        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(RotatingTextSymbol),
                                        new PropertyMetadata(0.0, OnAnglePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RotatingTextSymbol),
                                        new PropertyMetadata("", OnTextPropertyChanged));

        public static readonly DependencyProperty AngleBinderProperty =
            DependencyProperty.RegisterAttached("AngleBinder", typeof(double),
                                                typeof(RotatingTextSymbol),
                                                new PropertyMetadata(0.0, OnAngleBinderChanged));

        public RotatingTextSymbol()
        {
            Type t = typeof(RotatingTextSymbol);

            string temp = string.Format(Template, t.Namespace,
                                        t.Assembly.FullName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)[0
                                            ]);
            ControlTemplate = XamlReader.Load(temp) as ControlTemplate;
            OffsetX = Width / 2;
        }

        public RotatingTextSymbol(string fontFamily, double fontSize, string foregroundColor)
        {
            Template = Template.Replace(@"HorizontalAlignment=""Center"" VerticalAlignment=""Center""",
                @"HorizontalAlignment=""Center"" VerticalAlignment=""Center"" Foreground=""" + foregroundColor + @""" FontSize=""" + 
                    fontSize + @""" FontFamily=""" + fontFamily + @"""");

            Type t = typeof(RotatingTextSymbol);

            string temp = string.Format(Template, t.Namespace,
                                        t.Assembly.FullName.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)[0
                                            ]);
            ControlTemplate = XamlReader.Load(temp) as ControlTemplate;
            OffsetX = Width / 2;

        }


        /// <summary>
        /// Gets or sets Angle.
        /// </summary>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        /// <summary>
        /// Gets or sets Text.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// AngleProperty property changed handler. 
        /// </summary>
        /// <param name="d">ownerclass that changed its Angle.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as RotatingTextSymbol;
            if (dp != null) dp.OnPropertyChanged("Angle");

            //dp.Text = "Angle : " + Math.Round((double)e.NewValue);
            //dp.OffsetX = 50 * Math.Sin((double) e.NewValue);
            //dp.OffsetY = 10 * Math.Cos((double)e.NewValue);
        }


        /// <summary>
        /// TextProperty property changed handler. 
        /// </summary>
        /// <param name="d">ownerclass that changed its Angle.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs.</param> 
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dp = d as RotatingTextSymbol;
            if (dp != null) dp.OnPropertyChanged("Text");
        }

        public static double GetAngleBinder(DependencyObject d)
        {
            return (double)d.GetValue(AngleBinderProperty);
        }

        public static void SetAngleBinder(DependencyObject d, double value)
        {
            d.SetValue(AngleBinderProperty, value);
        }

        private static void OnAngleBinderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement)
            {
                var b = d as UIElement;
                if (e.NewValue is double)
                {
                    var c = (double)e.NewValue;
                    if (!double.IsNaN(c))
                    {
                        if (b.RenderTransform is RotateTransform)
                            (b.RenderTransform as RotateTransform).Angle = c;
                        else
                            b.RenderTransform = new RotateTransform { Angle = c };

                        // Convert to Radians
                        double angle = Math.PI * c / 180.0;

                        // Adjust the transform origin to prevent text from falling on the line.
                        b.RenderTransformOrigin = new Point(0.5, Math.Abs(Math.Cos(angle)) / 2);
                    }
                }
            }
        }
    }
    #endregion
}