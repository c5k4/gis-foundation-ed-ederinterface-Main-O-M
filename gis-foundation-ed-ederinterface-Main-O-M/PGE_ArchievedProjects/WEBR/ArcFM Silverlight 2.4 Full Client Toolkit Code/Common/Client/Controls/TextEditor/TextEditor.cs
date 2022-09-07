using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
#if WPF
using Xceed.Wpf.Toolkit;
using System.Text;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Controls
#elif WPF
namespace Miner.Mobile.Client.Controls
#endif
{
    /// <summary>
    /// Text editor.
    /// </summary>
    /// <exclude/>
    [TemplatePart(Name = ElementContentRichTextBox, Type = typeof(ExtendedRichTextBox))]
    [TemplatePart(Name = ElementValidationTextBlock, Type = typeof(TextBlock))]
    [TemplatePart(Name = ElementApplyButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementCancelButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementCutButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementCopyButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementPasteButton, Type = typeof(Button))]
    [TemplatePart(Name = ElementBoldToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementItalicToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementUnderlineToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementFontNameComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = ElementFontSizeComboBox, Type = typeof(ComboBox))]
    [TemplatePart(Name = ElementFontColorPicker, Type = typeof(ColorPicker))]
#if SILVERLIGHT
    [TemplatePart(Name = ElementInsertHyperlinkToggleButton, Type = typeof(ToggleButton))]
    [TemplatePart(Name = ElementInsertHyperlinkPopup, Type = typeof(Popup))]
    [TemplatePart(Name = ElementURLTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = ElementURLDescriptionTextBox, Type = typeof(TextBox))]
    [TemplatePart(Name = ElementInsertButton, Type = typeof(Button))]
#endif
    public class TextEditor : Control
    {
        private const string ElementContentRichTextBox = "PART_ContentRichTextBox";
        private const string ElementValidationTextBlock = "PART_ValidationTextBlock";
        private const string ElementApplyButton = "PART_ApplyButton";
        private const string ElementCancelButton = "PART_CancelButton";
        private const string ElementCutButton = "PART_CutButton";
        private const string ElementCopyButton = "PART_CopyButton";
        private const string ElementPasteButton = "PART_PasteButton";
        private const string ElementBoldToggleButton = "PART_BoldToggleButton";
        private const string ElementItalicToggleButton = "PART_ItalicToggleButton";
        private const string ElementUnderlineToggleButton = "PART_UnderlineToggleButton";
        private const string ElementFontNameComboBox = "PART_FontNameComboBox";
        private const string ElementFontSizeComboBox = "PART_FontSizeComboBox";
        private const string ElementFontColorPicker = "PART_FontColorPicker";
#if SILVERLIGHT
        private const string ElementInsertHyperlinkToggleButton = "PART_InsertHyperlinkToggleButton";
        private const string ElementInsertHyperlinkPopup = "PART_InsertHyperlinkPopup";
        private const string ElementURLTextBox = "PART_URLTextBox";
        private const string ElementURLDescriptionTextBox = "PART_URLDescriptionTextBox";
        private const string ElementInsertButton = "PART_InsertButton";
#endif

        private ExtendedRichTextBox _contentRichTextBox;
        private TextBlock _validationTextBlock;
        private Button _applyButton;
        private Button _cancelButton;
        private Button _cutButton;
        private Button _copyButton;
        private Button _pasteButton;
        private ToggleButton _boldToggleButton;
        private ToggleButton _italicToggleButton;
        private ToggleButton _underlineToggleButton;
        private ComboBox _fontNameComboBox;
        private ComboBox _fontSizeComboBox;
        private ColorPicker _fontColorPicker;
#if SILVERLIGHT
        private ToggleButton _insertHyperlinkToggleButton;
        private Popup _insertHyperlinkPopup;
        private TextBox _urlTextBox;
        private TextBox _urlDescriptionTextBox;
        private Button _insertButton;
#endif

        private string _initialXaml;
        private TextPointer _selectionStart;
        private bool _isFontNameChanged;
        private bool _isFontSizeChanged;
        private bool _isFontColorChanged;

        public TextEditor()
        {
            this.DefaultStyleKey = typeof(TextEditor);
        }

        public TextEditor(int textLength) : this()
        {
            TextFieldLength = textLength;
        }

        public TextEditor(string xaml)
        {
            this.DefaultStyleKey = typeof(TextEditor);

            _initialXaml = xaml;
        }

        public event EventHandler<TextEditEventArgs> TextEditApplied;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _contentRichTextBox = GetTemplateChild(ElementContentRichTextBox) as ExtendedRichTextBox;
#if SILVERLIGHT
            _contentRichTextBox.ContentChanged += new ContentChangedEventHandler(ContentRichTextBox_ContentChanged);
#elif WPF
            _contentRichTextBox.TextChanged += new TextChangedEventHandler(ContentRichTextBox_ContentChanged);
#endif
            _contentRichTextBox.SelectionChanged += new RoutedEventHandler(ContentRichTextBox_SelectionChanged);
            _contentRichTextBox.Drop += new DragEventHandler(ContentRichTextBox_Drop);
            _contentRichTextBox.KeyUp+= new KeyEventHandler(ContentRichTextBox_KeyDown);

            if (!string.IsNullOrEmpty(_initialXaml))
            {
                _contentRichTextBox.Xaml = _initialXaml;
            }

            _validationTextBlock = GetTemplateChild(ElementValidationTextBlock) as TextBlock;

            _applyButton = GetTemplateChild(ElementApplyButton) as Button;
            _applyButton.Click += new RoutedEventHandler(ApplyButton_Click);

            _cancelButton = GetTemplateChild(ElementCancelButton) as Button;
            _cancelButton.Click += new RoutedEventHandler(CancelButton_Click);

            _cutButton = GetTemplateChild(ElementCutButton) as Button;
            _cutButton.Click += new RoutedEventHandler(CutButton_Click);

            _copyButton = GetTemplateChild(ElementCopyButton) as Button;
            _copyButton.Click += new RoutedEventHandler(CopyButton_Click);

            _pasteButton = GetTemplateChild(ElementPasteButton) as Button;
            _pasteButton.Click += new RoutedEventHandler(PasteButton_Click);

            _boldToggleButton = GetTemplateChild(ElementBoldToggleButton) as ToggleButton;
            _boldToggleButton.Click += new RoutedEventHandler(BoldToggleButton_Click);

            _italicToggleButton = GetTemplateChild(ElementItalicToggleButton) as ToggleButton;
            _italicToggleButton.Click += new RoutedEventHandler(ItalicToggleButton_Click);

            _underlineToggleButton = GetTemplateChild(ElementUnderlineToggleButton) as ToggleButton;
            _underlineToggleButton.Click += new RoutedEventHandler(UnderlineToggleButton_Click);

            _fontNameComboBox = GetTemplateChild(ElementFontNameComboBox) as ComboBox;
            _fontNameComboBox.SelectionChanged += new SelectionChangedEventHandler(FontNameComboBox_SelectionChanged);

            _fontSizeComboBox = GetTemplateChild(ElementFontSizeComboBox) as ComboBox;
            _fontSizeComboBox.SelectionChanged += new SelectionChangedEventHandler(FontSizeComboBox_SelectionChanged);

            _fontColorPicker = GetTemplateChild(ElementFontColorPicker) as ColorPicker;
            _fontColorPicker.ColorChanged += new EventHandler(FontColorPicker_ColorChanged);

#if SILVERLIGHT
            _insertHyperlinkToggleButton = GetTemplateChild(ElementInsertHyperlinkToggleButton) as ToggleButton;

            _insertHyperlinkPopup = GetTemplateChild(ElementInsertHyperlinkPopup) as Popup;

            _urlTextBox = GetTemplateChild(ElementURLTextBox) as TextBox;
            _urlDescriptionTextBox = GetTemplateChild(ElementURLDescriptionTextBox) as TextBox;

            _insertButton = GetTemplateChild(ElementInsertButton) as Button;
            _insertButton.Click += new RoutedEventHandler(InsertButton_Click);
#endif
        }

        protected virtual void OnTextEditApplied(string xaml)
        {
            var handler = this.TextEditApplied;
            if (handler != null)
            {
                handler(this, new TextEditEventArgs(xaml));
            }
        }

#if SILVERLIGHT
        private void ContentRichTextBox_ContentChanged(object sender, ContentChangedEventArgs e)
#elif WPF
        private void ContentRichTextBox_ContentChanged(object sender, TextChangedEventArgs e)
#endif
        {
            if (CompressXaml(_contentRichTextBox.Xaml).Length > TextFieldLength)
            {
                _validationTextBlock.Text = "Error: This content and formatting exceed the maximum allowable characters for this field.";
                _applyButton.IsEnabled = false;
            }
            else if (_contentRichTextBox.Xaml == "")
            {
                _applyButton.IsEnabled = false;
            }
            else
            {
                _validationTextBlock.Text = "";
                _applyButton.IsEnabled = true;
            }
        }

        public int TextFieldLength { get; set; }

        private void ContentRichTextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (_contentRichTextBox.Selection.Text.Length == 0 && (_selectionStart == null || _selectionStart != null && _selectionStart.CompareTo(_contentRichTextBox.Selection.Start) == 0))
            {
                if (_isFontNameChanged)
                {
                    _contentRichTextBox.Selection.ApplyPropertyValue(Run.FontFamilyProperty, new FontFamily((_fontNameComboBox.SelectedItem as ComboBoxItem).Tag.ToString()));

                    _isFontNameChanged = false;
                }

                if (_isFontSizeChanged)
                {
                    _contentRichTextBox.Selection.ApplyPropertyValue(Run.FontSizeProperty, double.Parse((_fontSizeComboBox.SelectedItem as ComboBoxItem).Tag.ToString()));

                    _isFontSizeChanged = false;
                }

                if (_isFontColorChanged)
                {
                    _contentRichTextBox.Selection.ApplyPropertyValue(Run.ForegroundProperty, _fontColorPicker.Color);

                    _isFontColorChanged = false;
                }
            }
            else
            {
                _fontNameComboBox.SelectionChanged -= new SelectionChangedEventHandler(FontNameComboBox_SelectionChanged);
                _fontSizeComboBox.SelectionChanged -= new SelectionChangedEventHandler(FontSizeComboBox_SelectionChanged);
                _fontColorPicker.ColorChanged -= new EventHandler(FontColorPicker_ColorChanged);

                if (_contentRichTextBox.Selection.Start.Parent is Run)
                {
                    _fontNameComboBox.SelectedItem = _fontNameComboBox.Items.Cast<object>().FirstOrDefault(i => (i as ComboBoxItem).Tag.ToString() == ((Run)_contentRichTextBox.Selection.Start.Parent).FontFamily.Source);
                    _fontSizeComboBox.SelectedItem = _fontSizeComboBox.Items.Cast<object>().First(i => (i as ComboBoxItem).Tag.ToString() == ((Run)_contentRichTextBox.Selection.Start.Parent).FontSize.ToString());
                    _fontColorPicker.Color = (((Run)_contentRichTextBox.Selection.Start.Parent).Foreground as SolidColorBrush).Color;
                }
                else
                {
                    _fontNameComboBox.SelectedItem = _fontNameComboBox.Items.Cast<object>().FirstOrDefault(i => (i as ComboBoxItem).Tag.ToString() == ((Paragraph)_contentRichTextBox.Selection.Start.Parent).FontFamily.Source);
                    _fontSizeComboBox.SelectedItem = _fontSizeComboBox.Items.Cast<object>().First(i => (i as ComboBoxItem).Tag.ToString() == ((Paragraph)_contentRichTextBox.Selection.Start.Parent).FontSize.ToString());
                    _fontColorPicker.Color = (((Paragraph)_contentRichTextBox.Selection.Start.Parent).Foreground as SolidColorBrush).Color;
                }

                _fontNameComboBox.SelectionChanged += new SelectionChangedEventHandler(FontNameComboBox_SelectionChanged);
                _fontSizeComboBox.SelectionChanged += new SelectionChangedEventHandler(FontSizeComboBox_SelectionChanged);
                _fontColorPicker.ColorChanged += new EventHandler(FontColorPicker_ColorChanged);
            }

            if (_contentRichTextBox.Xaml.Length != 0)
            {
                if (_contentRichTextBox.Selection.Start.Parent is Run)
                {
                    _boldToggleButton.IsChecked = ((Run)_contentRichTextBox.Selection.Start.Parent).FontWeight == FontWeights.Bold;
                    _italicToggleButton.IsChecked = ((Run)_contentRichTextBox.Selection.Start.Parent).FontStyle == FontStyles.Italic;
                    _underlineToggleButton.IsChecked = ((Run)_contentRichTextBox.Selection.Start.Parent).TextDecorations != null;
                }
                else
                {
                    _boldToggleButton.IsChecked = ((Paragraph)_contentRichTextBox.Selection.Start.Parent).FontWeight == FontWeights.Bold;
                    _italicToggleButton.IsChecked = ((Paragraph)_contentRichTextBox.Selection.Start.Parent).FontStyle == FontStyles.Italic;
                    _underlineToggleButton.IsChecked = false;
                }

                _selectionStart = _contentRichTextBox.Selection.Start;
            }
        }

        private void ContentRichTextBox_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data == null)
            {
                ReturnFocus();
                return;
            }

            var dataObject = e.Data as IDataObject;
            if (dataObject == null)
            {
                ReturnFocus();
                return;
            }

            var data = dataObject.GetData(DataFormats.FileDrop);
            var files = data as FileInfo[];
            if (files == null)
            {
                ReturnFocus();
                return;
            }

            foreach (FileInfo file in files)
            {
                if (file == null) continue;

                ParseTextFile(file);
            }
            ReturnFocus();
        }

        private void ContentRichTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.X:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        CutButton_Click(null, null);
                    }
                    break;
                case Key.C:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        CopyButton_Click(null, null);
                    }
                    break;
                case Key.V:
                    if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                    {
                        PasteButton_Click(null, null);
                    }
                    break;
                case Key.B:
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt))
                    {
                        _boldToggleButton.IsChecked = !_boldToggleButton.IsChecked.Value;
                        BoldToggleButton_Click(null, null);
                    }
                    break;
                case Key.I:
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt))
                    {
                        _italicToggleButton.IsChecked = !_italicToggleButton.IsChecked.Value;
                        ItalicToggleButton_Click(null, null);
                    }
                    break;
                case Key.U:
                    if ((Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Alt)) == (ModifierKeys.Control | ModifierKeys.Alt))
                    {
                        _underlineToggleButton.IsChecked = !_underlineToggleButton.IsChecked.Value;
                        UnderlineToggleButton_Click(null, null);
                    }
                    break;
            }
        }

        private void FontNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_contentRichTextBox != null)
            {
                _contentRichTextBox.Selection.ApplyPropertyValue(Run.FontFamilyProperty, new FontFamily((_fontNameComboBox.SelectedItem as ComboBoxItem).Tag.ToString()));

                _isFontNameChanged = true;
            }

            ReturnFocus();
        }

        private void FontSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_contentRichTextBox != null)
            {
                _contentRichTextBox.Selection.ApplyPropertyValue(Run.FontSizeProperty, double.Parse((_fontSizeComboBox.SelectedItem as ComboBoxItem).Tag.ToString()));

                _isFontSizeChanged = true;
            }

            ReturnFocus();
        }

        void FontColorPicker_ColorChanged(object sender, EventArgs e)
        {
            if (_contentRichTextBox != null)
            {
                _contentRichTextBox.Selection.ApplyPropertyValue(Run.ForegroundProperty, new SolidColorBrush(_fontColorPicker.Color));

                _isFontColorChanged = true;
            }

            ReturnFocus();
        }

        private void BoldToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_contentRichTextBox != null)
            {
                _contentRichTextBox.Selection.ApplyPropertyValue(Run.FontWeightProperty, _boldToggleButton.IsChecked.Value ? FontWeights.Bold : FontWeights.Normal);
            }

            ReturnFocus();
        }

        private void ItalicToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_contentRichTextBox != null)
            {
                _contentRichTextBox.Selection.ApplyPropertyValue(Run.FontStyleProperty, _italicToggleButton.IsChecked.Value ? FontStyles.Italic : FontStyles.Normal);
            }

            ReturnFocus();
        }

        private void UnderlineToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_contentRichTextBox != null)
            {
                _contentRichTextBox.Selection.ApplyPropertyValue(Run.TextDecorationsProperty, _underlineToggleButton.IsChecked.Value ? TextDecorations.Underline : null);
            }

            ReturnFocus();
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            Clipboard.SetText(_contentRichTextBox.Selection.Xaml);
#elif WPF
            Clipboard.SetText(_contentRichTextBox.GetSelection());
#endif
            _contentRichTextBox.Selection.Text = "";

            ReturnFocus();
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            Clipboard.SetText(_contentRichTextBox.Selection.Xaml);
#elif WPF
            Clipboard.SetText(_contentRichTextBox.GetSelection());
#endif

            ReturnFocus();
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            var content = Clipboard.GetText().Replace("\r\n", "&#13;").Replace("\r", "&#13;").Replace("\n", "&#13;");
            try
            {
#if SILVERLIGHT
                _contentRichTextBox.Selection.Xaml = content;
#elif WPF
                _contentRichTextBox.SetSelection(content);
#endif
            }
            catch
            {
                content = content.Replace("&#13;", "\n");
                content = content.Replace("&", "&amp;").Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
                content = content.Replace("\n", "&#13;");
                content = content.Replace("&#13;", @"</Run></Paragraph><Paragraph FontFamily=""Arial"" FontSize=""12"" Foreground=""Black""><Run>");

                var xaml = @"<Section xml:space=""preserve"" HasTrailingParagraphBreakOnPaste=""False"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""><Paragraph FontFamily=""Arial"" FontSize=""12"" Foreground=""Black""><Run>" + content + @"</Run></Paragraph></Section>";
#if SILVERLIGHT
                _contentRichTextBox.Selection.Xaml = xaml;
#elif WPF
                _contentRichTextBox.SetSelection(xaml);
#endif
            }

            ReturnFocus();
        }

#if SILVERLIGHT
        private void InsertButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(_urlTextBox.Text);
            }
            catch
            {
            }

            if (uri != null)
            {
                var hyperlink = new Hyperlink();
                hyperlink.TargetName = "_blank";
                hyperlink.NavigateUri = uri;
                hyperlink.Inlines.Add(_urlDescriptionTextBox.Text.Length > 0 ? _urlDescriptionTextBox.Text : _urlTextBox.Text);

                _insertHyperlinkToggleButton.IsChecked = false;
                _contentRichTextBox.Selection.Insert(hyperlink);

                ReturnFocus();
            }
            else
            {
                MessageBox.Show("Invalid URL.", "Text Editor", MessageBoxButton.OK);
            }
        }
#endif

        void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as ChildWindow).Close();
            OnTextEditApplied(CompressXaml(_contentRichTextBox.Xaml));
        }

        void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            (this.Parent as ChildWindow).Close();
        }

        private string CompressXaml(string xaml)
        {
            if (string.IsNullOrEmpty(xaml))
            {
                return xaml;
            }

            var blocks = new XElement(XName.Get("S"));
            XElement section = XElement.Parse(xaml);
            foreach (var paragraph in section.Elements())
            {
                var line = new XElement(XName.Get("P"));
                foreach (var element in paragraph.Elements().Where(e => e.Name.LocalName == "Run"))
                {
                    var run = new XElement(XName.Get("R"));
                    run.Value = element.Value;

                    foreach (var attribute in element.Attributes())
                    {
                        switch(attribute.Name.LocalName)
                        {
                            case "Text":
                                run.Value = attribute.Value;
                                break;
                            case "FontFamily":
                                run.SetAttributeValue("NM", attribute.Value);
                                break;
                            case "FontSize":
                                run.SetAttributeValue("SZ", attribute.Value);
                                break;
                            case "Foreground":
                                run.SetAttributeValue("FG", attribute.Value);
                                break;
                            case "FontWeight":
                                run.SetAttributeValue("WT", attribute.Value);
                                break;
                            case "FontStyle":
                                run.SetAttributeValue("ST", attribute.Value);
                                break;
                            case "TextDecorations":
                                run.SetAttributeValue("DC", attribute.Value);
                                break;
                        };
                    }

                    line.Add(run);
                }

                blocks.Add(line);
            }

            var compressedXaml = blocks.ToString().Replace("\r\n  ", "").Replace("\r\n", "").Replace(">  ", ">");
            return compressedXaml.Replace("(", "&lb;").Replace(")", "&rb;").Replace("<", "(").Replace(">", ")");
        }

        private void ParseTextFile(FileInfo file)
        {
            Stream stream = null;
            try
            {
                stream = file.OpenRead();
                using (var reader = new StreamReader(stream))
                {
                    _contentRichTextBox.Selection.Text = reader.ReadToEnd();
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        private void ReturnFocus()
        {
            if (_contentRichTextBox != null)
            {
                _contentRichTextBox.Focus();
            }
        }
    }

    public class ExtendedRichTextBox : System.Windows.Controls.RichTextBox
    {
#if WPF
        public static readonly DependencyProperty XamlProperty = DependencyProperty.Register(
            "Xaml",
            typeof(string),
            typeof(ExtendedRichTextBox),
            new PropertyMetadata(string.Empty));
        
        public string Xaml
        {
            get { return GetValue(XamlProperty).ToString(); }
            set { SetValue(XamlProperty, value); }
        }

        public void SetSelection(string content)
        {
            var array = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(array);
            Selection.Load(stream, DataFormats.Xaml);
        }

        public string GetSelection()
        {
            var stream = new MemoryStream();
            Selection.Save(stream, DataFormats.Xaml);
            var reader = new StreamReader(stream);

            return reader.ReadToEnd();
        }
#endif
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (e.Key == Key.X || e.Key == Key.C || e.Key == Key.V)) return;

            base.OnKeyDown(e);
        }
    }
}
