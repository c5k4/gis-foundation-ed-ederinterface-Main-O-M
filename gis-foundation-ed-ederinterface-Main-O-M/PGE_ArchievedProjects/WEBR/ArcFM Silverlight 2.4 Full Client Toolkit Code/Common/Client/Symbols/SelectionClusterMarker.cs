using System;
using System.Windows.Controls;
using System.Windows.Markup;

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
    /// A class to manage binding for clustered graphics. 
    /// </summary>
    /// <exclude/>
    public class SelectionClusterMarker : MarkerSymbol, ILayerName
    {
        private const double OffsetSize = 16d;
        private readonly string _name;

        public SelectionClusterMarker(string name)
        {
            ControlTemplate = GetControlTemplate(name);
            _name = name;
        }

        public override double OffsetX
        {
            get { return OffsetSize; }
            set { throw new InvalidOperationException(); }
        }

        public override double OffsetY
        {
            get { return OffsetSize; }
            set { throw new InvalidOperationException(); }
        }

        #region ILayerName Members

        public string Name
        {
            get { return _name; }
        }

        #endregion

        private static ControlTemplate GetControlTemplate(string name)
        {
            const string template =
                @"
                <ControlTemplate 
                    xmlns=""http://schemas.microsoft.com/client/2007""
                    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                    xmlns:miner=""http://schemas.miner.com/arcfm/client/2010"">
                    <Grid IsHitTestVisible=""False"">
                        <Grid.Resources>
                            <miner:BooleanToVisibilityConverter x:Key=""BooleanToVisibilityConverter""/>
                            <miner:MarkerManager x:Key=""MarkerManager"" Name=""MARKERNAME""/>
                        </Grid.Resources>
                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name=""CommonStates"">
                                <VisualState x:Name=""MouseOver"" />
                                <VisualState x:Name=""Normal"" />
                            </VisualStateGroup>
                            <VisualStateGroup x:Name=""SelectionStates"">
                                <VisualState x:Name=""Selected"">
                                    
                                    <Storyboard> 
                                        <ColorAnimation Storyboard.TargetName=""Pin""
                                                        Storyboard.TargetProperty=""(Path.Stroke).(SolidColorBrush.Color)""
                                                        To=""#A333"" Duration=""00:00:0.25""/>
                                        <ColorAnimation Storyboard.TargetName=""Pin""
                                                        Storyboard.TargetProperty=""(Path.Fill).(SolidColorBrush.Color)""
                                                        To=""#AFFF"" Duration=""00:00:0.25""/>
                                        <ColorAnimation Storyboard.TargetName=""Number""
                                                        Storyboard.TargetProperty=""(Foreground).(SolidColorBrush.Color)""
                                                        To=""#333"" Duration=""00:00:0.25""/>
                                    </Storyboard>

                                </VisualState>
                                <VisualState x:Name=""Unselected"" />
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                        <Grid Visibility=""{Binding Source={StaticResource MarkerManager}, Path=IsSelected, Mode=OneWay, Converter={StaticResource BooleanToVisibilityConverter}}"">
					        <Ellipse
                                x:Name=""Pin""
                                Stroke=""#AFFF"" 
                                StrokeThickness=""2""
                                StrokeDashArray=""1 1""
						        Fill=""#A333"" 
						        Width=""32""
						        Height=""32"" />
					        <Grid HorizontalAlignment=""Center"" VerticalAlignment=""Center"" Width=""17"" Height=""22"">
                                <Viewbox VerticalAlignment=""Center"" HorizontalAlignment=""Center"" >
                                    <TextBlock x:Name=""Number"" Text=""{Binding Attributes[Count]}"" HorizontalAlignment=""Center"" VerticalAlignment=""Center"" TextAlignment=""Center"" Foreground=""White""/>
                                </Viewbox>
					        </Grid>
                        </Grid>
				    </Grid>
			    </ControlTemplate>";

#if SILVERLIGHT
            return XamlReader.Load(template.Replace("MARKERNAME", name.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;"))) as ControlTemplate;
#elif WPF
            var array = Encoding.UTF8.GetBytes(template.Replace("MARKERNAME", name.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;")));
            var stream = new MemoryStream(array);
            return XamlReader.Load(stream) as ControlTemplate;
#endif
        }
    }
}