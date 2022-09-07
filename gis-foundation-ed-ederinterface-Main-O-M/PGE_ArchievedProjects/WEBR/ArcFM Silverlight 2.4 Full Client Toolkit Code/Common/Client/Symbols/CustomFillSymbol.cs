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
    internal class CustomFillSymbol : FillSymbol, ILayerName
    {
        private readonly string _name;

        public CustomFillSymbol(SolidColorBrush fillColor, SolidColorBrush selectionColor, string name, string imagePath)
        {
            if (string.IsNullOrWhiteSpace(name)) name = "UNDEFINED";
            if (fillColor == null) fillColor = new SolidColorBrush(Colors.Red);

            _name = name;
            string template =
                @"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                             xmlns:miner=""http://schemas.miner.com/arcfm/client/2010"">
                            <Grid>
                                <Grid.Resources>
                                    <miner:MarkerManager x:Key=""MarkerManager"" Name=""MARKERNAME""/>
                                    <miner:BooleanToVisibilityConverter x:Key=""BooleanToVisibilityConverter"" />
                                </Grid.Resources>

                                <Path x:Name=""Element""
                                    Stroke=""White""
                                    StrokeStartLineCap=""Round""
                                    StrokeThickness=""2""
                                    IsHitTestVisible=""{Binding Source={StaticResource MarkerManager}, Path=IsSelected, Mode=OneWay}""
                                    StrokeLineJoin=""Round""
                                    Opacity=""{Binding Source={StaticResource MarkerManager}, Path=FillOpacity, Mode=OneWay}""
                                    StrokeEndLineCap=""Round""
                                    Fill=""Red""/>

                                <Canvas HorizontalAlignment=""Center"" VerticalAlignment=""Center"" Width=""24"" Height=""24"" Margin=""-12 -12 0 0"" IsHitTestVisible=""{Binding Source={StaticResource MarkerManager}, Path=IsSelected, Mode=OneWay}"" >
                                    <Grid HorizontalAlignment=""Stretch"" VerticalAlignment=""Stretch"" Opacity=""{Binding Source={StaticResource MarkerManager}, Path=Opacity}"">
                                        <!-- Cheap Shadow -->
                                        <Border BorderThickness=""0"" Background=""#333"" Margin=""0 1 0 -1"" Width=""24"" Height=""24"" CornerRadius=""12""/>
                                        <Border x:Name=""Pin"" BorderBrush=""#FFF"" BorderThickness=""2"" Background=""FILLCOLOR"" Margin=""0"" Width=""24"" Height=""24"" CornerRadius=""12""/>
                                        <!-- Viewbox to all the display of large numbers in the pin -->
                                        <Viewbox VerticalAlignment=""Center"" HorizontalAlignment=""Center"" Width=""17"" Height=""18"" Margin=""0"">
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
                                                <PlaneProjection LocalOffsetX=""-3"" LocalOffsetY=""2"" />
                                            </Image.Projection>
                                        </Image>
                                    </Grid>
                                </Canvas>

                                <VisualStateManager.VisualStateGroups>
                                    <VisualStateGroup x:Name=""SelectionStates"">
                                        <VisualState x:Name=""Unselected""/>
                                        <VisualState x:Name=""Selected"">
                                            <Storyboard>
                                                <ColorAnimation Storyboard.TargetName=""Element""
                                                            Storyboard.TargetProperty=""(Path.Fill).(SolidColorBrush.Color)""
                                                            To=""" + selectionColor.Color.ToString() + @"""
                                                            Duration=""00:00:00.25""/>
                                                <ColorAnimation Storyboard.TargetName=""Element""
                                                            Storyboard.TargetProperty=""(Path.Stroke).(SolidColorBrush.Color)""
                                                            To=""Red""
                                                            Duration=""00:00:00.25""/>

                                                <ColorAnimation Storyboard.TargetName=""Pin""
                                                            Storyboard.TargetProperty=""(Border.BorderBrush).(SolidColorBrush.Color)""
                                                            To=""FILLCOLOR"" Duration=""00:00:0.25""/>
                                                <ColorAnimation Storyboard.TargetName=""Pin""
                                                            Storyboard.TargetProperty=""(Border.Background).(SolidColorBrush.Color)""
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
            ControlTemplate = (ControlTemplate)XamlReader.Load(xaml);
#elif WPF
            var array = Encoding.UTF8.GetBytes(xaml);
            var stream = new MemoryStream(array);
            ControlTemplate = (ControlTemplate)XamlReader.Load(stream);
#endif
        }

        #region ILayerName Members

        public string Name
        {
            get { return _name; }
        }

        #endregion
    }
}