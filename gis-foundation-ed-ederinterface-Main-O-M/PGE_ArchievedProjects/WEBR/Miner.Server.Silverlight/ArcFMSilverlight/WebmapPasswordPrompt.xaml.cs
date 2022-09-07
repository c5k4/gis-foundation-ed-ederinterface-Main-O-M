using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ESRI.ArcGIS.Client.WebMap;
using System.Xml.Linq;

namespace ArcFMSilverlight
{
    public partial class WebmapPasswordPrompt : ChildWindow
    {

        public WebmapPasswordPrompt()
        {
            InitializeComponent();
            Loaded += WebmapPasswordPromptLoaded;
        }

        private void WebmapPasswordPromptLoaded(object sender, RoutedEventArgs e)
        {
            // Give Username textbox focus
            System.Windows.Browser.HtmlPage.Plugin.Focus();
            Dispatcher.BeginInvoke(() => { Username.Focus(); });
        }

        public WebmapPasswordPrompt(Document doc, string mapID, string mapDisplayName, string proxyUrl, string tokenServerUrl, XElement layers, bool visible) : this()
        {
            Doc = doc;
            MapID = mapID;
            MapDisplayName = mapDisplayName;
            ProxyUrl = proxyUrl;
            TokenServerUrl = tokenServerUrl;
            Layers = layers;
            Visible = visible;

            MapIDBlock.Text = MapDisplayName;
        }

        public Document Doc { get; set; }
        public string MapID { get; set; }
        private string _mapDisplayName;
        public string MapDisplayName { get { return string.IsNullOrWhiteSpace(_mapDisplayName) ? MapID : _mapDisplayName; } set { _mapDisplayName = value; } }
        public string ProxyUrl { get; set; }
        public string TokenServerUrl { get; set; }
        public XElement Layers { get; set; }
        public bool Visible { get; set; }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            Done();
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void LayoutRootKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Done();
            else if (e.Key == Key.Escape) Cancel();
        }

        private void Done()
        {
            DialogResult = true;
        }

        private void Cancel()
        {
            DialogResult = false;
        }
    }
}

