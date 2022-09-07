using System.Windows.Controls;
using System.Windows.Markup;
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
    /// <exclude/>
    public class CustomPointSymbol : BasicCustomPointSymbol
    {
        public CustomPointSymbol()
        {
            string template =
              @"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                 xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                 xmlns:miner=""http://schemas.miner.com/arcfm/client/2010"">

                    <Grid RenderTransformOrigin=""0.5,0.5"">
                        <Grid.Resources>
                            <miner:PointSizeToMarginConverter x:Key=""PointSizeToMarginConverter"" />
                        </Grid.Resources>

                        <Grid.RenderTransform>
                            <RotateTransform Angle=""{Binding Symbol.Angle}"" />
                        </Grid.RenderTransform>

                        <Grid.Margin>
                            <Binding Path=""Symbol.Size"" Converter=""{StaticResource PointSizeToMarginConverter}"" />
                        </Grid.Margin>

                        <Border x:Name=""SymbolBorder"">
                            <Image Source=""{Binding Symbol.Source}""
                                x:Name=""SymbolImage""
                                Stretch=""Fill""
                                Width=""{Binding Symbol.Width}""
                                Height=""{Binding Symbol.Height}"" />
                        </Border>

                        <VisualStateManager.VisualStateGroups>
                            <VisualStateGroup x:Name=""SelectionStates"">
                                <VisualState x:Name=""Unselected"" />
                                <VisualState x:Name=""Selected"">
                                    <Storyboard>
                                        <ObjectAnimationUsingKeyFrames Storyboard.TargetName=""SymbolBorder"" Storyboard.TargetProperty=""Background"">
                                            <DiscreteObjectKeyFrame KeyTime=""0:0:0"" Value=""{Binding Symbol.SelectionColor}"" />
                                        </ObjectAnimationUsingKeyFrames>
                                        <DoubleAnimation Storyboard.TargetName=""SymbolImage""
					  			                         Storyboard.TargetProperty=""Opacity""
                                                         To=""0.5"" Duration=""0"" />
                                    </Storyboard>
                                </VisualState>
                            </VisualStateGroup>
                        </VisualStateManager.VisualStateGroups>
                    </Grid>
                </ControlTemplate>";

#if SILVERLIGHT
            ControlTemplate = (ControlTemplate)XamlReader.Load(template);
#elif WPF
            var array = Encoding.UTF8.GetBytes(template);
            var stream = new MemoryStream(array);
            ControlTemplate = (ControlTemplate)XamlReader.Load(stream);
#endif
        }
    }
}