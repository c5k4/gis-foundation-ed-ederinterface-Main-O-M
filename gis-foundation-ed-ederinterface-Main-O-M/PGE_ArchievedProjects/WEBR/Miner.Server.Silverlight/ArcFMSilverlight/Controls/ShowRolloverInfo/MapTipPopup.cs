using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ESRI.ArcGIS.Client;
//using ESRI.SilverlightViewer.Utility;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.SilverlightViewer.Controls;


namespace ArcFMSilverlight.Controls.ShowRolloverInfo
{
    public class MapTipPopup : PopupWindow
    {
        private string _subtypeLookupUrl;
        public IDictionary<object, string> _subtypeCodeDescriptions = new Dictionary<object, string>();

        public bool ShowGlobalId { get; set; }

        /// <summary>
        /// Create a friendly popup window with a template format string
        /// </summary>
        /// <param name="template">A template format string</param>
        public MapTipPopup(string template)
        {
            this.ShowCloseButton = false;

            if (!string.IsNullOrEmpty(template))
            {
                CreateTemplateTipPopup(template);
            }
        }

        /// <summary>
        /// Create a simple popup tip window
        /// </summary>
        /// <param name="lyrFields">The info of the feature layer fields (from LayerInfo)</param>
        /// <param name="outFields">Fields whose values to be output in the Tip popup</param>
        /// <param name="titleField">A field whose value to be used in the Tip window title</param>
        /// <param name="lyrName">Optional - the name of the feature layer</param>
        public MapTipPopup(List<Field> lyrFields, List<string> outFields, string titleField, string lyrName, string subtypeLookupUrl)
        {
            this.ShowCloseButton = false;

            if (lyrFields != null && outFields != null)
            {
                CreateSimpleTipPopup(lyrFields, outFields, titleField, lyrName);
            }
            _subtypeLookupUrl = subtypeLookupUrl;
            FeatureLayer fl = new FeatureLayer() {Url = _subtypeLookupUrl};
            fl.Initialized += new EventHandler<EventArgs>(fl_Initialized);
            fl.Initialize();
        }

        void fl_Initialized(object sender, EventArgs e)
        {
            FeatureLayerInfo featureLayerInfo = (sender as FeatureLayer).LayerInfo;

            if (featureLayerInfo.FeatureTypes == null) return;

            foreach (KeyValuePair<object,FeatureType> kvp in featureLayerInfo.FeatureTypes)
            {
                _subtypeCodeDescriptions.Add(kvp.Value.Id, kvp.Value.Name);
            }
        }

        #region Create a Simple Popup Window without a Template Format String
        /// <summary>
        /// Create a simple popup tip window
        /// </summary>
        /// <param name="lyrFields">The info of the feature layer fields (from LayerInfo)</param>
        /// <param name="outFields">Fields whose values to be output in the Tip popup</param>
        /// <param name="titleField">A field whose value to be used in the Tip window title</param>
        /// <param name="lyrName">Optional - the name of the feature layer</param>
        protected virtual void CreateSimpleTipPopup(List<Field> lyrFields, List<string> outFields, string titleField, string lyrName)
        {
            Binding titleBinding = new Binding() { Path = new PropertyPath(string.Format("[{0}]", titleField)) };
            this.SetBinding(PopupWindow.TitleProperty, titleBinding);
            if (!string.IsNullOrEmpty(lyrName))
            {
                this.TitleFormat = lyrName + ": {0}";
            }

            StackPanel stackBox = new StackPanel() { Margin = new Thickness(4, 1, 4, 1), Orientation = Orientation.Vertical };

            if (outFields.Count == 1 && outFields[0] == "*")
            {
                foreach (Field lyrField in lyrFields)
                {
                    CreateDataBoundTextBlock(stackBox, lyrField);
                }                
            }
            else
            {
                foreach (string field in outFields)
                {
                    Field lyrField = lyrFields.First(f => f.Name.Equals(field, StringComparison.CurrentCultureIgnoreCase));
                    if (lyrField != null)
                    {
                        CreateDataBoundTextBlock(stackBox, lyrField);
                    }
                }
            }

            this.Content = stackBox;
        }

        private void CreateDataBoundTextBlock(StackPanel stackBox, Field lyrField)
        {
            if (!ShowGlobalId && lyrField.Name.ToUpper() == "GLOBALID") return;

            TextBlock valueBlock = new TextBlock() { TextWrapping = TextWrapping.NoWrap };
            Binding valueBinding = new Binding() { Path = new PropertyPath(string.Format("[{0}]", lyrField.Name)), StringFormat = lyrField.Alias + ": {0}" };

            if (lyrField.Domain != null && lyrField.Domain is CodedValueDomain)
            {
                valueBinding.Converter = new DictionaryValuePickerConverter();
                valueBinding.ConverterParameter = lyrField.Domain as CodedValueDomain;
            }
            else if (lyrField.Name.ToUpper().StartsWith("SUBTYPE"))
            {
                valueBinding.Converter = new DictionaryValuePickerConverter();
                valueBinding.ConverterParameter = _subtypeCodeDescriptions;
            }
            else if (lyrField.Name.ToUpper() == "LENGTHININCHES")//PLC Change CR
            {
                valueBinding.Converter = new PoleHeightValuePickerConverter();
            }

            valueBlock.SetBinding(TextBlock.TextProperty, valueBinding);
            stackBox.Children.Add(valueBlock);
        }
       
        #endregion

        #region Create a Friendly Popup Window with a Template Format String
        /// <summary>
        /// Create a Friendly Popup Window with a Template Format String
        /// </summary>
        /// <param name="template">A template format string for Popup Tip Window</param>
        protected virtual void CreateTemplateTipPopup(string template)
        {
            try
            {
                FrameworkElement lineBlock = null;
                Dictionary<string, string> templateMap = null;// StringHelper.ConvertToDictionary(template, ';', '=');
                StackPanel contentBox = new StackPanel() { Margin = new Thickness(2, 2, 2, 2), Orientation = Orientation.Vertical };

                foreach (string key in templateMap.Keys)
                {
                    if (key.Equals("Title", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string titleField = "";
                        string[] splits = Regex.Split(templateMap["Title"], "\\{(.*?)\\}");
                        for (int i = 0; i < splits.Length; i++)
                        {
                            if (splits[i].StartsWith("#")) { titleField = splits[i]; break; }
                        }

                        if (titleField != "")
                        {
                            this.TitleFormat = templateMap["Title"].Replace(titleField, "0");
                            Binding titleBinding = new Binding() { Path = new PropertyPath(string.Format("[{0}]", titleField.Replace("#", ""))) };
                            this.SetBinding(PopupWindow.TitleProperty, titleBinding);
                        }
                        else this.TitleFormat = templateMap["Title"];
                    }
                    else if (key.Equals("Content", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string[] contentLines = Regex.Split(templateMap["Content"], @"\\n");
                        for (int i = 0; i < contentLines.Length; i++)
                        {
                            lineBlock = CreateTextLineStack(contentLines[i], false);
                            contentBox.Children.Add(lineBlock);
                        }
                    }
                    else if (key.Equals("Note", StringComparison.CurrentCultureIgnoreCase))
                    {
                        lineBlock = CreateTextLineStack(templateMap["Note"], true);
                        contentBox.Children.Add(lineBlock);
                    }
                    else if (key.Equals("Image", StringComparison.CurrentCultureIgnoreCase))
                    {
                        double imgWidth = double.NaN;
                        double imgHeight = double.NaN;
                        Match matchSize = Regex.Match(templateMap["Image"], "\\[(.*?)\\*(.*?)\\]");
                        if (matchSize.Success)
                        {
                            if (!string.IsNullOrEmpty(matchSize.Result("$1")))
                                imgWidth = double.Parse(matchSize.Result("$1"));

                            if (!string.IsNullOrEmpty(matchSize.Result("$2")))
                                imgHeight = double.Parse(matchSize.Result("$2"));
                        }

                        Match matchUrl = Regex.Match(templateMap["Image"], "\\{#(.*?)\\}");
                        if (matchUrl.Success)
                        {
                            string imgUrl = matchUrl.Result("$1");
                            Image tipImage = new Image() { Stretch = Stretch.Uniform, Width = imgWidth, Height = imgHeight };
                            Binding imgBinding = new Binding() { Path = new PropertyPath(string.Format("[{0}]", imgUrl)) };
                            tipImage.SetBinding(Image.SourceProperty, imgBinding);
                            contentBox.Children.Add(tipImage);
                        }
                    }
                }

                if (templateMap.ContainsKey("Link"))
                {
                    Match matchUrl = Regex.Match(templateMap["Link"], "\\{#(.*?)\\}");
                    if (matchUrl.Success)
                    {
                        this.Resources.Add("HyperlinkField", matchUrl.Result("$1"));
                        TextBlock textBlock = new TextBlock() { Text = "<CTRL>+Click to open the link", Foreground = new SolidColorBrush(Colors.Purple), FontSize = 9, Margin = new Thickness(4, 2, 4, 2) };
                        contentBox.Children.Add(textBlock);
                    }
                }

                this.Content = contentBox;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Your Map Tip template syntax is invalid: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        protected virtual FrameworkElement CreateTextLineStack(string lineText, bool isNote)
        {
            Binding textBinding = null;
            TextBlock textBlock = null;
            StackPanel lineStack = null;
            string bindingFormat = "";

            string[] splits = Regex.Split(lineText, "\\{(.*?)\\}");

            if (splits.Length > 3)
            {
                lineStack = new StackPanel() { Margin = new Thickness(2, 1, 2, 1), Orientation = Orientation.Horizontal };
            }

            for (int i = 0; i < splits.Length; i++)
            {
                if (!splits[i].Equals(""))
                {
                    if (splits[i].StartsWith("#"))
                    {
                        bindingFormat = splits[i - 1] + "{0}" + ((i == splits.Length - 2) ? splits[i + 1] : "");
                        textBlock = new TextBlock() { TextWrapping = (isNote ? TextWrapping.Wrap : TextWrapping.NoWrap) };
                        textBinding = new Binding() { Path = new PropertyPath(string.Format("[{0}]", splits[i].Replace("#", ""))), StringFormat = bindingFormat };
                        textBlock.SetBinding(TextBlock.TextProperty, textBinding);
                        if (lineStack != null) lineStack.Children.Add(textBlock);
                        bindingFormat = "";
                    }
                }
            }

            if (lineStack != null) return lineStack;
            else { textBlock.Margin = new Thickness(2, 1, 2, 1); return textBlock; }
        }
        #endregion
    }
}
