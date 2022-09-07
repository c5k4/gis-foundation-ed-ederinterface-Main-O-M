using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

using ESRI.ArcGIS.Client.Symbols;
#if WPF
using System.IO;
using System.Text;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Symbols
#elif WPF
namespace Miner.Mobile.Client.Symbols
#endif
{
    /// <summary>
    /// Custom Marker Symbol for query results
    /// </summary>
    public class CustomMarkerSymbol : MarkerSymbol, ILayerName
    {
        private readonly string _name;
        private SolidColorBrush _fillColor;
        private SolidColorBrush _selectionColor;
        private string _imagePath;

        public CustomMarkerSymbol(SolidColorBrush fillColor, SolidColorBrush selectionColor, string name, string imagePath)
        {
            if (string.IsNullOrWhiteSpace(name)) name = "UNDEFINED";
            if (fillColor == null) fillColor = new SolidColorBrush(Colors.Red);
            
            _name = name;
            _fillColor = fillColor;
            _selectionColor = selectionColor;
            _imagePath = imagePath;

            string template =
                @"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                   xmlns:miner=""http://schemas.miner.com/arcfm/client/2010"">
                    <Grid Margin=""-25.5,-35,0,0"" HorizontalAlignment=""Center"" > 
                        <Grid.Resources>
                            <miner:MarkerManager x:Key=""MarkerManager"" Name=""MARKERNAME""/>
                            <miner:BooleanToVisibilityConverter x:Key=""BooleanToVisibilityConverter"" />
                            <miner:MarkerSizeToMarginConverter x:Key=""MarkerSizeToMarginConverter"" />
                        </Grid.Resources>

                        <Canvas>
                            <Ellipse x:Name=""Element"" Height=""{Binding Symbol.Size}"" Width=""{Binding Symbol.Size}"" Margin=""{Binding Symbol.Size,Converter={StaticResource MarkerSizeToMarginConverter}}"" Fill=""Red"" Opacity=""{Binding Source={StaticResource MarkerManager}, Path=FillOpacity, Mode=OneWay}"" />
                            <Grid Opacity=""{Binding Source={StaticResource MarkerManager}, Path=Opacity}"" IsHitTestVisible=""{Binding Source={StaticResource MarkerManager}, Path=IsSelected, Mode=OneWay}"">    
                                <!-- Cheap Shadow -->
                                <Path Stroke=""#333"" StrokeThickness=""2"" Fill=""#333"" Margin=""0 1 0 -1""
                                            Data=""M 16,21.5 12.5,32 9.5,22 A 11,11 360 1 1 15.5,22""/>
                                <Path x:Name=""Pin"" Stroke=""White"" StrokeThickness=""2"" Fill=""FILLCOLOR""
                                            Data=""M 16,21.5 12.5,32 9.5,22 A 11,11 360 1 1 15.5,22""/>
                                <!-- Viewbox to all the display of large numbers in the pin -->
                                <Viewbox VerticalAlignment=""Center"" HorizontalAlignment=""Center"" Width=""17"" Height=""18"" Margin=""0 -13 0 0"">
                                    <TextBlock x:Name=""Number"" Text=""{Binding Attributes[RowIndex]}"" HorizontalAlignment=""Center"" VerticalAlignment=""Center"" TextAlignment=""Center"" Margin=""0"" Foreground=""White""/>
                                </Viewbox>
                                <Image Width=""16"" 
                                        Height=""16""
                                        VerticalAlignment=""Bottom""
                                        HorizontalAlignment=""Left""
                                        Opacity="".9""
                                        Source=""" + imagePath + @"""
                                        Visibility=""{Binding Attributes[Locked],Converter={StaticResource BooleanToVisibilityConverter}}"">
                                    <Image.Projection>
                                        <PlaneProjection LocalOffsetY=""-6"" />
                                    </Image.Projection>
                                </Image>
                            </Grid>
                        </Canvas>

                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name=""SelectionStates"">
                                <VisualState x:Name=""Unselected"" />
                                <VisualState x:Name=""Selected"">
                                    <Storyboard>
                                        <ColorAnimation Storyboard.TargetName=""Element""
                                                    Storyboard.TargetProperty=""(Ellipse.Fill).(SolidColorBrush.Color)""
                                                    To=""" + selectionColor.Color.ToString() + @"""
                                                        Duration=""00:00:00.25"" />
                                        <ColorAnimation Storyboard.TargetName=""Pin""
                                                    Storyboard.TargetProperty=""(Path.Stroke).(SolidColorBrush.Color)""
                                                    To=""FILLCOLOR"" Duration=""00:00:0.25""/>
                                        <ColorAnimation Storyboard.TargetName=""Pin""
                                                    Storyboard.TargetProperty=""(Path.Fill).(SolidColorBrush.Color)""
                                                    To=""White"" Duration=""00:00:0.25""/>
                                        <ColorAnimation Storyboard.TargetName=""Number""
                                                    Storyboard.TargetProperty=""(Foreground).(SolidColorBrush.Color)""
                                                    To=""FILLCOLOR"" Duration=""00:00:0.25""/>
                                    </Storyboard>
                                </VisualState>
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                    </Grid>
                </ControlTemplate>";

            name = name.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
            string xaml = template.Replace("FILLCOLOR", fillColor.Color.ToString()).Replace("MARKERNAME", name);
          
#if SILVERLIGHT
            ControlTemplate =
                (ControlTemplate)
                XamlReader.Load(xaml);
#elif WPF
            var array = Encoding.UTF8.GetBytes(name);
            var stream = new MemoryStream(array);
            ControlTemplate = (ControlTemplate)XamlReader.Load(stream);
#endif
        }

        public double Size
        {
            get
            {
                return (double)base.GetValue(SizeProperty);
            }
            set
            {
                base.SetValue(SizeProperty, value);
            }
        }

        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size",
            typeof(double),
            typeof(CustomMarkerSymbol),
            null);

        public CustomMarkerSymbol Clone()
        {
            return new CustomMarkerSymbol(_fillColor, _selectionColor, _name, _imagePath)
            {
                Size = Size
            };
        }

        #region ILayerName Members

        public string Name
        {
            get { return _name; }
        }

        #endregion
    }
}