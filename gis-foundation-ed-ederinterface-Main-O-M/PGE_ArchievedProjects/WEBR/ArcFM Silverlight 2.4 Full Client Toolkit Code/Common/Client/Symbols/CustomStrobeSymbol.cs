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
    internal class CustomStrobeSymbol : MarkerSymbol
    {
        public CustomStrobeSymbol()
        {
            string template =
                @"<ControlTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                   xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                   xmlns:miner=""http://schemas.miner.com/arcfm/client/2010"">
                      <Canvas>
                            <VisualStateManager.VisualStateGroups>
                                <VisualStateGroup x:Name=""CommonStates"">
                                    <VisualState x:Name=""Normal"">
                                        <Storyboard RepeatBehavior=""3x"">

                                            <DoubleAnimation BeginTime=""0""
																 Storyboard.TargetName=""ellipse"" Storyboard.TargetProperty=""(UIElement.RenderTransform).(ScaleTransform.ScaleX)""
																 From=""1"" To=""10"" Duration=""00:00:0.5"" />

                                            <DoubleAnimation BeginTime=""0""
																 Storyboard.TargetName=""ellipse"" Storyboard.TargetProperty=""(UIElement.RenderTransform).(ScaleTransform.ScaleY)""
																 From=""1"" To=""10"" Duration=""00:00:0.5"" />

                                            <DoubleAnimation BeginTime=""0""
																 Storyboard.TargetName=""ellipse"" Storyboard.TargetProperty=""(UIElement.Opacity)""
																 From=""1"" To=""0"" Duration=""00:00:0.5"" />
                                        </Storyboard>
                                    </VisualState>
                                </VisualStateGroup>
                            </VisualStateManager.VisualStateGroups>
                            <!--Strobe ellipse-->
                            <Ellipse Height=""10"" Width=""10"" Canvas.Left=""-5"" Canvas.Top=""-5"" 
										 RenderTransformOrigin=""0.5,0.5"" x:Name=""ellipse"">
                                <Ellipse.RenderTransform>
                                    <ScaleTransform />
                                </Ellipse.RenderTransform>
                                <Ellipse.Fill>
                                    <RadialGradientBrush>
                                        <GradientStop Color=""#00FF0000"" />
                                        <GradientStop Color=""#FFFF0000"" Offset=""0.25""/>
                                        <GradientStop Color=""#00FF0000"" Offset=""0.5""/>
                                        <GradientStop Color=""#FFFF0000"" Offset=""0.75""/>
                                        <GradientStop Color=""#00FF0000"" Offset=""1""/>
                                    </RadialGradientBrush>
                                </Ellipse.Fill>
                            </Ellipse>
                            <!--Static symbol on top-->
                            <Ellipse Height=""10"" Width=""10"" Canvas.Left=""-5"" Canvas.Top=""-5"" 
										 Fill=""#00FF0000"" x:Name=""ellipse1""/>
                        </Canvas>              
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
