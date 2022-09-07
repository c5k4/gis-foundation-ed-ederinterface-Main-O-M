using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
#if WPF
using System.Text;
using System.IO;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    static internal class Utility
    {
        internal static List<string> HiddenFields = new List<string>
        {
                "ROWINDEX",
                "LAYERNAME",
                "SHAPE"
        };

        #region internal static methods

        internal static DataTemplate CreateRelatedDataColumn()
        {
            DataTemplate cellTemplate = null;

            try
            {
                // set up the column data template to display the data
                string template =
                        @"<DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
                                            xmlns:miner=""http://schemas.miner.com/arcfm/client/2010""" +
#if SILVERLIGHT
                        @"                    xmlns:local=""clr-namespace:Miner.Server.Client.Toolkit""" +
#elif WPF
                        @"                    xmlns:local=""clr-namespace:Miner.Mobile.Client.Toolkit""" +
#endif
 @"                  xmlns:ei=""clr-namespace:Microsoft.Expression.Interactivity.Core;assembly=Microsoft.Expression.Interactions""
                                            xmlns:i=""clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity""
                                            xmlns:toolkit=""clr-namespace:System.Windows.Controls;assembly=System.Windows.Controls.Toolkit"">
                            <Border Background=""#2000"">
                                <i:Interaction.Triggers>
                                    <i:EventTrigger EventName=""MouseEnter"" SourceName=""Marker"">
                                        <ei:ChangePropertyAction PropertyName=""IsOpen"" 
                                                                 TargetName=""ToolPopup""
                                                                 Value=""True"" />
                                    </i:EventTrigger>
                                    <i:EventTrigger EventName=""MouseEnter"" SourceName=""Menu"">
                                        <ei:ChangePropertyAction PropertyName=""IsOpen"" 
                                                                 TargetName=""ToolPopup""
                                                                 Value=""True"" />
                                    </i:EventTrigger>
                                    <i:EventTrigger EventName=""MouseLeave"" SourceName=""Marker"">
                                        <ei:ChangePropertyAction PropertyName=""IsOpen"" 
                                                                 TargetName=""ToolPopup""
                                                                 Value=""False"" />
                                    </i:EventTrigger>
                                    <i:EventTrigger EventName=""MouseLeave"" SourceName=""Menu"">
                                        <ei:ChangePropertyAction PropertyName=""IsOpen"" 
                                                                 TargetName=""ToolPopup""
                                                                 Value=""False"" />
                                    </i:EventTrigger>
                                </i:Interaction.Triggers>

                                <Grid x:Name=""Marker"" 
                                      MinHeight=""22""
                                      Margin=""3 1 0 1""
                                      VerticalAlignment=""Center""
                                      Background=""Transparent"">
                                    <Grid.Resources>
                                        <Style x:Key=""GridButton"" TargetType=""Button"">
                                            <Setter Property=""Background"" Value=""Transparent"" />
                                            <Setter Property=""Foreground"" Value=""#FF000000"" />
                                            <Setter Property=""Margin"" Value=""0"" />
                                            <Setter Property=""BorderThickness"" Value=""0"" />
                                            <Setter Property=""Template"">
                                                <Setter.Value>
                                                    <ControlTemplate TargetType=""Button"">
                                                        <Grid>
                                                            <VisualStateManager.VisualStateGroups>
                                                                <VisualStateGroup x:Name=""CommonStates"">
                                                                    <VisualState x:Name=""Normal"" />
                                                                    <VisualState x:Name=""MouseOver"" />
                                                                    <VisualState x:Name=""Pressed"" />
                                                                    <VisualState x:Name=""Disabled"">
                                                                        <Storyboard>
                                                                            <DoubleAnimation Duration=""0"" 
                                                                                             Storyboard.TargetName=""DisabledVisualElement""
                                                                                             Storyboard.TargetProperty=""Opacity""
                                                                                             To="".55"" />
                                                                        </Storyboard>
                                                                    </VisualState>
                                                                </VisualStateGroup>
                                                                <VisualStateGroup x:Name=""FocusStates"">
                                                                    <VisualState x:Name=""Focused"">
                                                                        <Storyboard>
                                                                            <DoubleAnimation Duration=""0"" 
                                                                                             Storyboard.TargetName=""FocusVisualElement""
                                                                                             Storyboard.TargetProperty=""Opacity""
                                                                                             To=""1"" />
                                                                        </Storyboard>
                                                                    </VisualState>
                                                                    <VisualState x:Name=""Unfocused"" />
                                                                </VisualStateGroup>
                                                            </VisualStateManager.VisualStateGroups>
                                                            <Border x:Name=""Background"" 
                                                                    Background=""White""
                                                                    BorderBrush=""{TemplateBinding BorderBrush}""
                                                                    BorderThickness=""{TemplateBinding BorderThickness}""
                                                                    CornerRadius=""0"">
                                                                <Grid Margin=""1"" Background=""{TemplateBinding Background}"">
                                                                    <Border x:Name=""BackgroundAnimation"" 
                                                                            Background=""#FF448DCA""
                                                                            Opacity=""0"" />
                                                                    <Rectangle x:Name=""BackgroundGradient"">
                                                                        <Rectangle.Fill>
                                                                            <LinearGradientBrush StartPoint="".7,0"" EndPoint="".7,1"">
                                                                                <GradientStop Offset=""0"" Color=""#FFFFFFFF"" />
                                                                                <GradientStop Offset=""0.375"" Color=""#F9FFFFFF"" />
                                                                                <GradientStop Offset=""0.625"" Color=""#E5FFFFFF"" />
                                                                                <GradientStop Offset=""1"" Color=""#C6FFFFFF"" />
                                                                            </LinearGradientBrush>
                                                                        </Rectangle.Fill>
                                                                    </Rectangle>
                                                                </Grid>
                                                            </Border>
                                                            <ContentPresenter x:Name=""contentPresenter"" 
                                                                              Margin=""{TemplateBinding Padding}""
                                                                              HorizontalAlignment=""{TemplateBinding HorizontalContentAlignment}""
                                                                              VerticalAlignment=""{TemplateBinding VerticalContentAlignment}""
                                                                              Content=""{TemplateBinding Content}""
                                                                              ContentTemplate=""{TemplateBinding ContentTemplate}"" />
                                                            <Rectangle x:Name=""DisabledVisualElement"" 
                                                                       Fill=""#FFFFFFFF""
                                                                       IsHitTestVisible=""false""
                                                                       Opacity=""0""
                                                                       RadiusX=""3""
                                                                       RadiusY=""3"" />
                                                            <Rectangle x:Name=""FocusVisualElement"" 
                                                                       Margin=""1""
                                                                       IsHitTestVisible=""false""
                                                                       Opacity=""0""
                                                                       RadiusX=""2""
                                                                       RadiusY=""2""
                                                                       Stroke=""#FF6DBDD1""
                                                                       StrokeThickness=""1"" />
                                                        </Grid>
                                                    </ControlTemplate>
                                                </Setter.Value>
                                            </Setter>
                                        </Style>
                                    </Grid.Resources>

                                    <Grid.ColumnDefinitions>
                                        <ColumnDefinition Width=""Auto"" />
                                        <ColumnDefinition Width=""*"" />
                                        <ColumnDefinition Width=""0"" />
                                    </Grid.ColumnDefinitions>
                                    
                                    <TextBlock Grid.Column=""1"" 
                                               Margin=""3,0,3,0""
                                               VerticalAlignment=""Center""
                                               Text=""{Binding Graphic.PrimaryDisplay}"" />

                                    <Popup x:Name=""ToolPopup"" 
                                           Grid.Column=""2""
                                           HorizontalOffset=""-1""
                                           VerticalOffset=""0"">
                                        <StackPanel x:Name=""Menu"" Orientation=""Horizontal"">
                                            <Button Command=""{Binding RowToolCommand}"" 
                                                    CommandParameter=""Zoom""
                                                    Style=""{StaticResource GridButton}"">
                                                <Image Width=""16"" 
                                                       Height=""16""" +
#if SILVERLIGHT
                        @"                             Source=""/Miner.Server.Client.Toolkit;component/Images/zoom_in.png"" />" +
#elif WPF
                        @"                             Source=""/Miner.Mobile.Client.Toolkit;component/Images/zoom_in.png"" />" +
#endif
                        @"                  </Button>
                                            <Button Command=""{Binding RowToolCommand}"" 
                                                    CommandParameter=""Pan""
                                                    Style=""{StaticResource GridButton}""
                                                    ToolTipService.Placement=""Right""
                                                    ToolTipService.PlacementTarget=""{Binding ElementName=RemoveButton}"">
                                                <Image Width=""16"" 
                                                       Height=""16""" +
#if SILVERLIGHT
                        @"                             Source=""/Miner.Server.Client.Toolkit;component/Images/Pan.png"" />" +
#elif WPF
                        @"                             Source=""/Miner.Mobile.Client.Toolkit;component/Images/Pan.png"" />" +
#endif
                        @"                  </Button>
                                        </StackPanel>
                                    </Popup>
                                </Grid>
                            </Border>
                    </DataTemplate>";

#if SILVERLIGHT
                cellTemplate = (DataTemplate)XamlReader.Load(template);
#elif WPF
                var array = Encoding.UTF8.GetBytes(template);
                var stream = new MemoryStream(array);
                cellTemplate = (DataTemplate)XamlReader.Load(stream);
#endif
            }
            catch (Exception ex)
            {
                LoggingService.Write("Unable to create related data column", ex);
            }

            return cellTemplate;
        }

        internal static bool FieldIsHidden(string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) == false)
            {
                return HiddenFields.Any(field => fieldName.Equals(field, StringComparison.InvariantCultureIgnoreCase));
            }
            return false;
        }

        internal static string DecodeString(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;

            string newValue = value;

            newValue = newValue.Replace("\"", string.Empty); //remove quotes coming from REST and Json
            newValue = newValue.Replace("&quot", "\"");
            //newValue = newValue.Replace("\\", string.Empty); //remove \ coming from REST and Json
            const string replaceSequence = @"\/";
            newValue = newValue.Replace(replaceSequence, "/"); //remove escape characters from Json

            return newValue;
        }

        /// <summary>
        ///  One of several egregious hacks required by Esri returning identify results 
        ///  with alias names instead of field names
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        internal static string GetObjectIDFieldName(IDictionary<string, object> attributes)
        {
            string oidFieldName = string.Empty;
            if (attributes != null)
            {
                if (attributes.ContainsKey("OBJECTID"))
                {
                    oidFieldName = "OBJECTID";
                }
                else if (attributes.ContainsKey("OBJECT ID"))
                {
                    oidFieldName = "OBJECT ID";
                }
                else if (attributes.ContainsKey("Object ID"))
                {
                    oidFieldName = "Object ID";
                }
                else if (attributes.ContainsKey("ObjectID"))
                {
                    oidFieldName = "ObjectID";
                }
                else if (attributes.ContainsKey("ObjectId"))
                {
                    oidFieldName = "ObjectId";
                }
                else if (attributes.ContainsKey("Object Id"))
                {
                    oidFieldName = "Object Id";
                }
                else if (attributes.ContainsKey("OID"))
                {
                    oidFieldName = "OID";
                }
            }
            return oidFieldName;
        }

        internal static int GetObjectIDValue(IDictionary<string, object> attributes)
        {
            int objectID = -1;

            string oidFieldName = GetObjectIDFieldName(attributes);
            if (!string.IsNullOrEmpty(oidFieldName))
            {
                Int32.TryParse(attributes[oidFieldName].ToString(), out objectID);
            }
            return objectID;
        }

        internal static Layer GetLayerFromUrl(Map map, string url)
        {
            if (map == null) return null;

            foreach (Layer layer in map.Layers)
            {
                ArcGISDynamicMapServiceLayer dynamic = layer as ArcGISDynamicMapServiceLayer;
                if ((dynamic != null) && (dynamic.Url == url)) return dynamic;

                ArcGISTiledMapServiceLayer tiled = layer as ArcGISTiledMapServiceLayer;
                if ((tiled != null) && (tiled.Url == url)) return tiled;

                FeatureLayer featureLayer = layer as FeatureLayer;
                if ((featureLayer != null) && (featureLayer.Url == url)) return featureLayer;
            }
            return null;
        }

        internal static GeometryType GetGeometryType(Geometry geometry)
        {
            if (geometry is MapPoint) return GeometryType.Point;
            else if (geometry is Polyline) return GeometryType.Polyline;
            else if (geometry is Envelope) return GeometryType.Envelope;
            else if (geometry is Polygon) return GeometryType.Polygon;

            return GeometryType.MultiPoint;
        }

        /// <summary>
        /// Determines whether to bind to the field alias or the field name
        /// ESRI returns field aliases in Identify when they should return the field name
        /// </summary>
        /// <param name="field"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        internal static string FieldToBind(Field field, IDictionary<string, object> attributes)
        {
            return FieldToBind(field.Name, field.Alias, attributes);
        }

        internal static string FieldToBind(string fieldName, string fieldAlias, IDictionary<string, object> attributes)
        {
            string fieldToBind = fieldName;
            if (attributes.ContainsKey(fieldName) == false)
            {
                fieldToBind = fieldAlias;
            }

            return fieldToBind;
        }

        internal static string RemoveSpecialCharactersFromAlias(string fieldAlias)
        {
            string newFieldAlias = Regex.Replace(fieldAlias, @"[\[\]&<>]", " ");
            return Regex.Replace(newFieldAlias, @"[']", "");
        }
      
        // Have to do the cast to check for equals because .Equals on the base class does not return correct value
        // http://forums.arcgis.com/threads/18472-Equals-method-returns-incorrect-value-for-Geometry?p=58723#post58723
        internal static bool GeometriesAreEqual(Geometry geometry, Geometry geometry2)
        {
            return (geometry.GetType() == geometry2.GetType()) &&
                    (((geometry is MapPoint) && (geometry as MapPoint).Equals(geometry2 as MapPoint)) ||
                    ((geometry is Envelope) && (geometry as Envelope).Equals(geometry2 as Envelope)) ||
                    ((geometry is Polygon) && (geometry as Polygon).EqualsPolygon(geometry2 as Polygon)) ||
                    ((geometry is MultiPoint) && (geometry as MultiPoint).EqualsMultiPoint(geometry2 as MultiPoint)) ||
                    ((geometry is Polyline) && (geometry as Polyline).EqualsPolyline(geometry2 as Polyline)) ||
                    geometry.Equals(geometry2));
                    
        }

        #endregion internal static methods
    }
}
