using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ArcFM.Silverlight.PGE.CustomTools;
using ArcFMSilverlight.Controls.Favorites;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using Miner.Server.Client;
//WEBR Stability - Replace Feature Service with WCF for Favorites
using System.ServiceModel;
using System.Windows.Browser;
namespace ArcFMSilverlight
{
    public partial class FavoritesControl : UserControl
    {
        private FavoritesModel _favoritesModel;
        private ContextMenu _contextMenu = null;
        private Favorite _selectedFavorite = null;
        private bool _favLoaded = false;

        //WEBR Stability - Replace Feature Service with WCF for Favorites
        string _WCFService_URL = "";
        string endpointaddress = string.Empty;
        private string _user;
        private StoredViewControl _storedViewControl;

        public FavoritesControl()
        {
            InitializeComponent();
            ContextMenuService.SetContextMenu(FavoritesListBox, ListContextMenu);
        }

        private ContextMenu ListContextMenu
        {
            get
            {
                if (_contextMenu == null)
                {
                    _contextMenu = new ContextMenu();
                    MenuItem openFavorite = new MenuItem();
                    openFavorite.Click += new RoutedEventHandler(openFavorite_Click);
                    openFavorite.Header = "Open";
                    _contextMenu.Items.Add(openFavorite);
                    MenuItem deleteFavorite = new MenuItem();
                    deleteFavorite.Click += new RoutedEventHandler(deleteFavorite_Click);
                    deleteFavorite.Header = "Delete";
                    _contextMenu.Items.Add(deleteFavorite);
                }
                return _contextMenu;
            }
        }

        private Map _map;
        public void Initialize(XElement element, MapTools mapTools, Map map)
        {
            if (element.Attribute("disableFavorites") != null &&
                Convert.ToBoolean(element.Attribute("disableFavorites").Value))
            {
                this.IsEnabled = false;
                return;
            }

            //WEBR Stability - Replace Feature Service with WCF for Favorites
            // string favoriteServiceUrl = element.Element("FavoritesService").Attribute("Url").Value + "/" +
            // element.Element("FavoritesService").Attribute("LayerId").Value;

            //_favoritesModel = new FavoritesModel(favoriteServiceUrl, mapTools, map);
            _favoritesModel = new FavoritesModel(mapTools, map);

            _favoritesModel.FavoritesLoadedSuccess += new EventHandler(_favoritesModel_FavoritesLoadedSuccess);
            //_favoritesModel.FavoriteSaveSuccess += new EventHandler(_favoritesModel_FavoriteSaveSuccess);

            //_favoritesModel.FavoritesLoadedFailed += new EventHandler(_favoritesModel_FavoritesLoadedFailed);
            //_favoritesModel.FavoriteSaveFailed += new EventHandler(_favoritesModel_FavoriteSaveFailed);
            _favoritesModel.FavoriteLoaded += new EventHandler(_favoritesModel_FavoriteLoaded);
            _map = map;

            //WEBR Stability - Replace Feature Service with WCF for Favorites
            LoadFavoritesWCFConfig();
            string prefixUrl = GetPrefixUrl();
            endpointaddress = prefixUrl + _WCFService_URL;

            _user = WebContext.Current.User.Name.Replace("PGE\\", "");
            _storedViewControl = mapTools.StoredViewControl;
        }

        //WEBR Stability - Replace Feature Service with WCF for Favorites
        private void LoadFavoritesWCFConfig()
        {

            try
            {
                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    var attribute = "";
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement elements = XElement.Parse(config);
                    foreach (XElement element in elements.Elements())
                    {
                        if (element.Name.LocalName == "Favorites")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement childelement in element.Elements())
                                {

                                    if (childelement.Name.LocalName == "FavoritesService")
                                    {
                                        attribute = childelement.Attribute("WCFService_URL").Value;
                                        if (attribute != null)
                                        {
                                            _WCFService_URL = attribute;
                                        }
                                    }

                                }

                            }
                        }
                    }
                }

            }
            catch
            {
            }
        }

        public string GetPrefixUrl()
        {
            string prefixUrl;
            var pageUri = "";
            if (System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != null && System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != "")
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString().Replace(System.Windows.Browser.HtmlPage.Document.DocumentUri.Query.ToString(), "");
            else
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString();
            if (pageUri.Contains("Default.aspx"))
            {
                prefixUrl = pageUri.Substring(0, pageUri.IndexOf("Default.aspx"));
            }
            else
            {
                prefixUrl = pageUri;
            }
            return prefixUrl;
        }

        void _favoritesModel_FavoriteLoaded(object sender, EventArgs e)
        {
            FavoritesListBox.SelectedIndex = -1;
        }

        void _map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            FavoritesListBox.MaxHeight = _map.ActualHeight;
        }
        void favoriteSaveFailed()
        {
            ConfigUtility.UpdateStatusBarText("An error occurred Saving Favorites");
        }

        void favoritesLoadedFailed()
        {
            ConfigUtility.UpdateStatusBarText("An error occurred Loading Favorites");
        }

        public void Load()
        {
            if (this.IsEnabled == false) return;
            if (!_favLoaded)
            {
                ConfigUtility.UpdateStatusBarText("Loading Bookmarks/Favorites...");
                //_favoritesModel.Load();
                GetFavoritesList();  //WEBR Stability - Replace Feature Service with WCF for Favorites
            }
            _favLoaded = true;
        }

        //WEBR Stability - Replace Feature Service with WCF for Favorites
        private void GetFavoritesList()
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();
            try
            {

                cusservice.BeginGetFavoritesList(_user,
                delegate(IAsyncResult result)
                {
                    List<FavoritesData> favoritesList = ((IServicePons)result.AsyncState).EndGetFavoritesList(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {
                        ClientFavoritesListHandle_Async(favoritesList);
                    }
                    );
                }, cusservice);
            }
            catch
            {
                favoritesLoadedFailed();
            }
        }

        private void ClientFavoritesListHandle_Async(List<FavoritesData> favoritesList)
        {
            _favoritesModel.PushFavoritesList(favoritesList.ToList());
        }

        void favoriteSaveSuccess()
        {
            FavoritesListBox.UpdateLayout();
        }

        void _favoritesModel_FavoritesLoadedSuccess(object sender, EventArgs e)
        {
            FavoritesButton.IsEnabled = true;
            FavoritesListBox.DataContext = _favoritesModel.Favorites;
            FavoritesListBox.UpdateLayout();
            ConfigUtility.UpdateStatusBarText("");
            FavoritesListBox.MaxHeight = _map.ActualHeight;
            // For some reason dynamic binding won't refresh -- 
            //BookmarksListBox.SetBinding(ListBox.MaxHeightProperty,
            //    new Binding() { Source = _map.Parent, Path = new PropertyPath("ActualHeight"), Mode = BindingMode.OneWay, BindsDirectlyToSource = true });
            //BookmarksListBox.UpdateLayout();
            _map.SizeChanged += new SizeChangedEventHandler(_map_SizeChanged);
        }

        //WEBR Stability - Replace Feature Service with WCF for Favorites
        void deleteFavorite_Click(object sender, RoutedEventArgs e)
        {
            //_favoritesModel.Delete(_selectedFavorite.ObjectId);
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();
            try
            {

                cusservice.BeginDeleteFavorites(_selectedFavorite.ObjectId, _user,
                delegate(IAsyncResult result)
                {
                    FavoritesSuccessData favDeleteData = ((IServicePons)result.AsyncState).EndDeleteFavorites(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {
                        if (favDeleteData.isSuccess)
                        {
                            // Update the local collection
                            _favoritesModel.PushFavoritesList(favDeleteData.FavoritesList.ToList());

                            favoriteSaveSuccess();
                        }
                        else
                        {
                            //TODO: remove this when error is propagated
                            MessageBox.Show("An error occurred Saving Favorites");
                            favoriteSaveFailed();
                        }
                    }
                    );
                }, cusservice);
            }
            catch
            {
                MessageBox.Show("An error occurred Saving Favorites");
                favoriteSaveFailed();
            }
        }

        void openFavorite_Click(object sender, RoutedEventArgs e)
        {
            _favoritesModel.Open(_selectedFavorite);
        }

        void newFavoriteWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((ChildWindow)sender).DialogResult == false) return;

            NewFavoriteWindow newFavoriteWindow = sender as NewFavoriteWindow;
            if (_favoritesModel.FavoritesDictionary.ContainsKey(newFavoriteWindow.TxtFavorite.Text.ToLower()))
            {
                MessageBoxResult result = MessageBox.Show(
                    "The Favorite [ " + newFavoriteWindow.TxtFavorite.Text +
                    " ] already exists. Press OK to overwrite it",
                    "Add a Favorite", MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        private void newFavoriteWindow_Closed(object sender, EventArgs eventArgs)
        {
            NewFavoriteWindow newFavoriteWindow = sender as NewFavoriteWindow;

            if (newFavoriteWindow.DialogResult == true)
            {
                //_favoritesModel.Add(newFavoriteWindow.TxtFavorite.Text);
                addFavorites(newFavoriteWindow.TxtFavorite.Text);
            }
        }

        //WEBR Stability - Replace Feature Service with WCF for Favorites
        public void addFavorites(string name)
        {
            try
            {
                string insertOrUpdate = _favoritesModel.InsertOrUpdate(name);
                string layerVisibilities = _favoritesModel.GetCurrentLayerVisibilities();
                EndpointAddress endPoint = new EndpointAddress(endpointaddress);
                BasicHttpBinding httpbinding = new BasicHttpBinding();
                httpbinding.MaxBufferSize = 2147483647;
                httpbinding.MaxReceivedMessageSize = 2147483647;
                httpbinding.TransferMode = TransferMode.Buffered;
                httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
                IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();
                try
                {

                    cusservice.BeginAddEditFavorites(insertOrUpdate, _user, name, _storedViewControl._selectedStoredView.StoredViewName, layerVisibilities,
                    delegate(IAsyncResult result)
                    {
                        FavoritesSuccessData favAddEditData = ((IServicePons)result.AsyncState).EndAddEditFavorites(result);
                        this.Dispatcher.BeginInvoke(
                        delegate()
                        {
                            if (favAddEditData.isSuccess)
                            {
                                // Update the local collection
                                _favoritesModel.PushFavoritesList(favAddEditData.FavoritesList.ToList());

                                favoriteSaveSuccess();
                            }
                            else
                            {
                                MessageBox.Show("An error occurred Saving Favorites");
                                favoriteSaveFailed();
                            }
                        }
                        );
                    }, cusservice);
                }
                catch
                {
                    MessageBox.Show("An error occurred Saving Favorites");
                    favoriteSaveFailed();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void FavoritesButton_OnChecked(object sender, RoutedEventArgs e)
        {
            FavoritesPopup.IsOpen = true;
        }

        private void FavoritesButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            FavoritesPopup.IsOpen = false;
        }


        private void AddFavoriteButton_OnClick(object sender, RoutedEventArgs e)
        {
            //TestLvt();
            //return;
            NewFavoriteWindow newFavoriteWindow = new NewFavoriteWindow();
            newFavoriteWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newFavoriteWindow_Closing);
            newFavoriteWindow.Closed += new EventHandler(newFavoriteWindow_Closed);
            //+= new EventHandler(newFavoriteWindow_Closed);
            newFavoriteWindow.Show();

        }

        private void FavoritesListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || FavoritesListBox.SelectedIndex == -1) return;

            Favorite favorite = e.AddedItems[0] as Favorite;
            _favoritesModel.Open(favorite);
        }


        private void FavoritesListBox_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (sender as ListBox)).OfType<ListBoxItem>().First();
            _selectedFavorite = (Favorite)listBoxItem.Content;
        }
    }
}
