using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using ESRI.ArcGIS.Client;

using EsriSymbols = ESRI.ArcGIS.Client.Symbols;

#if SILVERLIGHT
namespace Miner.Server.Client.Symbols
#elif WPF
namespace Miner.Mobile.Client.Symbols
#endif
{
    internal class RedlineTextController : DependencyObject
    {
        #region Constants

        private const string TextValue = "TEXTVALUE";
        private const string TextColor = "TEXTCOLOR";
        private const string TextSize = "FONTSIZE";
        private const string Angle = "ROTATION_ANGLE";

        private const string TextGraphicKey = "TextGraphic";
        private const string TextLayerID = "TextTemporary";

        private const string DefaultTextColor = "Blue";
        private const int DefaultTextSize = 14;

        #endregion

        #region Private Variables

        private GraphicsLayer _tempLayer;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(RedlineTextController),
            new PropertyMetadata(OnMapChanged));

        [Category("Redline Text Properties")]
        [Description("Map Control")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ID of the FeatureLayer on which to act.
        /// </summary>
        public string LayerID
        {
            get { return (string)GetValue(LayerIDProperty); }
            set { SetValue(LayerIDProperty, value); }
        }

        public static readonly DependencyProperty LayerIDProperty = DependencyProperty.Register(
            "LayerID",
            typeof(string),
            typeof(RedlineTextController),
            new PropertyMetadata(OnLayerIDChanged));

        public ControlTemplate TextTemplate
        {
            get { return (ControlTemplate)GetValue(TextTemplateProperty); }
            set { SetValue(TextTemplateProperty, value); }
        }

        public static readonly DependencyProperty TextTemplateProperty = DependencyProperty.Register(
            "TextTemplate",
            typeof(ControlTemplate),
            typeof(RedlineTextController),
            new PropertyMetadata(null));

        public Brush Background
        {
            get
            {
                return (Brush)base.GetValue(BackgroundProperty);
            }
            set
            {
                base.SetValue(BackgroundProperty, value);
            }
        }

        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background",
            typeof(Brush),
            typeof(RedlineTextController),
            null);

        public Brush BorderBrush
        {
            get
            {
                return (Brush)base.GetValue(BorderBrushProperty);
            }
            set
            {
                base.SetValue(BorderBrushProperty, value);
            }
        }

        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush",
            typeof(Brush),
            typeof(RedlineTextController),
            null);

        #endregion

        #region Event Handlers

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var redlineText = (RedlineTextController)d;

            if (redlineText._tempLayer != null)
            {
                redlineText._tempLayer.ClearGraphics();
            }

            if (redlineText.Map.Layers[TextLayerID] != null)
            {
                redlineText._tempLayer = redlineText.Map.Layers[TextLayerID] as GraphicsLayer;
            }
            else
            {
                redlineText._tempLayer = new GraphicsLayer { ID = TextLayerID };
                redlineText.Map.Layers.Add(redlineText._tempLayer);
            }

            if (!string.IsNullOrEmpty(redlineText.LayerID))
            {
                FeatureLayer layer = redlineText.Map.Layers[redlineText.LayerID] as FeatureLayer;
                if (layer != null)
                {
                    redlineText._tempLayer.MinimumResolution = layer.MinimumResolution;
                    redlineText._tempLayer.MaximumResolution = layer.MaximumResolution;

                    layer.Graphics.CollectionChanged += redlineText.GraphicsCollectionChanged;
                    SetBinding(redlineText._tempLayer, Layer.VisibleProperty, "Visible", layer);
                    redlineText.CreateTextGraphics(layer);
                }
            }

            // TODO: Add Map.Layers.CollectionChanged
        }

        private static void OnLayerIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var redlineText = d as RedlineTextController;
            if (redlineText == null) return;
            if (redlineText.Map == null) return;

            if (e.OldValue != null)
            {
                string layerId = e.OldValue.ToString();
                FeatureLayer layer = redlineText.Map.Layers[layerId] as FeatureLayer;
                if (layer != null)
                {
                    layer.Graphics.CollectionChanged -= redlineText.GraphicsCollectionChanged;
                }
            }
            if (e.NewValue != null)
            {
                string layerId = e.NewValue.ToString();
                FeatureLayer layer = redlineText.Map.Layers[layerId] as FeatureLayer;
                if (layer != null)
                {
                    layer.Graphics.CollectionChanged += redlineText.GraphicsCollectionChanged;
                }
                redlineText.CreateTextGraphics(layer);
            }
        }

        private void GraphicsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var layer = sender as IList;
            if (layer == null) return;

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    CreateTextGraphics(Map.Layers[LayerID] as FeatureLayer);
                }

                return;
            }

            if (layer == null && e.NewItems == null) return;
            if (layer.Count <= 0 && e.NewItems.Count <= 0) return;

            foreach (var item in e.Action == NotifyCollectionChangedAction.Add ? e.NewItems : layer)
            {
                var graphic = item as Graphic;
                if (graphic == null) continue;

                if (graphic.Attributes[TextValue] == null)
                {
                    graphic.Attributes[TextValue] = "";
                    graphic.Attributes[TextColor] = "Black";
                    graphic.Attributes[TextSize] = (short)12;
                    graphic.Attributes[Angle] = 0.0;
                }
                else if (graphic.Attributes[Angle] == null)
                {
                    graphic.Attributes[Angle] = 0.0;
                }

                graphic.Symbol = new EsriSymbols.TextSymbol();

                var textGraphic = new Graphic
                {
                    Symbol = new CustomTextSymbol()
                    {
                        Background = Background,
                        BorderBrush = BorderBrush
                    },
                    Geometry = graphic.Geometry
                };
                var textSymbol = CreateTextSymbol(graphic, textGraphic);
                textSymbol.GenerateTextImage();

                graphic.Attributes[TextGraphicKey] = textGraphic;
                textGraphic.Attributes[TextGraphicKey] = graphic;

                _tempLayer.Graphics.Add(textGraphic);
                graphic.AttributeValueChanged += GraphicAttributeValueChanged;
                graphic.PropertyChanged += GraphicPropertyChanged;
            }
        }

        static void GraphicPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Geometry") return;

            var graphic = sender as Graphic;
            if (graphic == null) return;

            ((Graphic)graphic.Attributes[TextGraphicKey]).Geometry = graphic.Geometry;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates text graphics for every graphic in every layer being tracked as a text layer.
        /// </summary>
        /// <param name="layer">The layer to create text graphics for.</param>
        private void CreateTextGraphics(IEnumerable<Graphic> layer)
        {
            _tempLayer.Graphics.Clear();

            if (layer == null) return;

            foreach (Graphic graphic in layer)
            {
                graphic.Symbol = new EsriSymbols.TextSymbol();

                var textGraphic = new Graphic
                {
                    Symbol = new CustomTextSymbol()
                    {
                        Background = Background,
                        BorderBrush = BorderBrush
                    },
                    Geometry = graphic.Geometry
                };
                var textSymbol = CreateTextSymbol(graphic, textGraphic);
                textSymbol.GenerateTextImage();

                graphic.Attributes[TextGraphicKey] = textGraphic;
                textGraphic.Attributes[TextGraphicKey] = graphic;

                _tempLayer.Graphics.Add(textGraphic);
                graphic.AttributeValueChanged += GraphicAttributeValueChanged;
            }
        }

        private CustomTextSymbol CreateTextSymbol(Graphic graphic, Graphic tempGraphic)
        {
            var textSymbol = tempGraphic.Symbol as CustomTextSymbol;
            textSymbol.FontFamily = new FontFamily("Arial");
            textSymbol.SelectionColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255));

            SetBinding(tempGraphic, Graphic.SelectedProperty, "Selected", graphic);

            if (graphic.Attributes.ContainsKey(TextValue))
            {
                SetBinding(textSymbol, CustomTextSymbol.CompressedXamlProperty, "[" + TextValue + "]", graphic.Attributes);
            }
            if (graphic.Attributes.ContainsKey(TextColor))
            {
                SetBinding(textSymbol, CustomTextSymbol.ForegroundProperty, "[" + TextColor + "]", graphic.Attributes, BindingMode.OneWay);
            }
            if (graphic.Attributes.ContainsKey(TextSize))
            {
                SetBinding(textSymbol, CustomTextSymbol.FontSizeProperty, "[" + TextSize + "]", graphic.Attributes);
            }
            if (graphic.Attributes.ContainsKey(Angle))
            {
                SetBinding(textSymbol, CustomTextSymbol.AngleProperty, "[" + Angle + "]", graphic.Attributes);
            }
            return textSymbol;
        }

        private static void SetBinding(DependencyObject d, DependencyProperty property, string path, object source, BindingMode mode = BindingMode.TwoWay)
        {
            Binding binding = new Binding(path) { Source = source, Mode = mode };
            BindingOperations.SetBinding(d, property, binding);
        }

        static void GraphicAttributeValueChanged(object sender, ESRI.ArcGIS.Client.Graphics.DictionaryChangedEventArgs e)
        {
            var graphic = (Graphic)sender;

            if (graphic.Attributes.ContainsKey(TextValue))
            {
                if (graphic.Attributes[TextValue] == null)
                {
                    graphic.Attributes[TextValue] = "";
                }
            }

            var symbol = (graphic.Attributes[TextGraphicKey] as Graphic).Symbol as CustomTextSymbol;

            symbol.CompressedXaml = graphic.Attributes[TextValue].ToString();

            if (graphic.Attributes.ContainsKey(Angle))
            {
                if (graphic.Attributes[Angle] == null)
                {
                    symbol.Angle = 0;
                }
                else
                {
                    symbol.Angle = (double)graphic.Attributes[Angle];
                }
            }

            if (graphic.Attributes.ContainsKey(TextSize))
            {
                if (graphic.Attributes[TextSize] == null)
                {
                    symbol.FontSize = 8;
                }
                else
                {
                    symbol.FontSize = (short)graphic.Attributes[TextSize];
                }
            }

            if (graphic.Attributes.ContainsKey(TextColor))
            {
                if (graphic.Attributes[TextColor] == null)
                {
                    symbol.Foreground = new SolidColorBrush(Colors.Black);
                }
                else
                {
                    var color = graphic.Attributes[TextColor].ToString();
                    color = color[0].ToString().ToUpper() + color.Substring(1);

                    Type colorType = (typeof(Colors));
                    if (colorType.GetProperty(color) != null)
                    {
                        object o = colorType.InvokeMember(color, BindingFlags.GetProperty, null, null, null);
                        if (o != null)
                        {
                            symbol.Foreground = new SolidColorBrush((Color)o);
                        }
                    }
                }
            }

            if (e.Key == Angle) return;

            symbol.GenerateTextImage();
        }

        #endregion
    }
}
