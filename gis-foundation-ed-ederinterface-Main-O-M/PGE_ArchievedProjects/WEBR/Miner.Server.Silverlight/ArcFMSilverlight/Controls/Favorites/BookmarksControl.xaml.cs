using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ArcFM.Silverlight.PGE.CustomTools;
using ArcFMSilverlight.Controls.Bookmarks;
using ArcFMSilverlight.Controls.Favorites;
using ESRI.ArcGIS.Client;

namespace ArcFMSilverlight
{
    public partial class BookmarksControl : UserControl
    {
        private BookmarksModel _bookmarksModel;
        private Bookmark _selectedBookmark = null;
        private ContextMenu _contextMenuBookmarks = null;
        private bool _firstTime = true;
        private Map _map;

        public BookmarksControl()
        {
            InitializeComponent();
            ContextMenuService.SetContextMenu(BookmarksListBox, BookmarksListContextMenu);
        }
        public bool FirstTime
        {
            get { return _firstTime; }
            set { _firstTime = value; }
        }
        private ContextMenu BookmarksListContextMenu
        {
            get
            {
                if (_contextMenuBookmarks == null)
                {
                    _contextMenuBookmarks = new ContextMenu();
                    MenuItem openFavoriteBookmark = new MenuItem();
                    openFavoriteBookmark.Click += new RoutedEventHandler(openFavoriteBookmark_Click);
                    openFavoriteBookmark.Header = "Open";
                    _contextMenuBookmarks.Items.Add(openFavoriteBookmark);
                    MenuItem deleteFavoriteBookmarks = new MenuItem();
                    deleteFavoriteBookmarks.Click += new RoutedEventHandler(deleteFavoriteBookmarks_Click);
                    deleteFavoriteBookmarks.Header = "Delete";
                    _contextMenuBookmarks.Items.Add(deleteFavoriteBookmarks);
                    MenuItem setDefaultBookmarks = new MenuItem();
                    setDefaultBookmarks.Click += new RoutedEventHandler(setDefaultBookmarks_Click);
                    setDefaultBookmarks.Header = "Set Startup";
                    _contextMenuBookmarks.Items.Add(setDefaultBookmarks);
                    MenuItem unsetDefaultBookmarks = new MenuItem();
                    unsetDefaultBookmarks.Click += new RoutedEventHandler(unsetDefaultBookmarks_Click);
                    unsetDefaultBookmarks.Header = "Unset Startup";
                    _contextMenuBookmarks.Items.Add(unsetDefaultBookmarks);
                }
                return _contextMenuBookmarks;
            }
        }

        void setDefaultBookmarks_Click(object sender, RoutedEventArgs e)
        {
            _bookmarksModel.SetDefault(_selectedBookmark);
        }

        void unsetDefaultBookmarks_Click(object sender, RoutedEventArgs e)
        {
            _bookmarksModel.UnsetDefault();
        }

        void deleteFavoriteBookmarks_Click(object sender, RoutedEventArgs e)
        {
            _bookmarksModel.Delete(_selectedBookmark.ObjectId);
        }

        void openFavoriteBookmark_Click(object sender, RoutedEventArgs e)
        {
            _bookmarksModel.Open(_selectedBookmark);
        }

        public void Initialize(XElement element, StoredViewControl storedViewControl, Map map)
        {
            string bookmarkServiceUrl = element.Element("BookmarksService").Attribute("Url").Value + "/" +
                                        element.Element("BookmarksService").Attribute("LayerId").Value;

            _bookmarksModel = new BookmarksModel(bookmarkServiceUrl, storedViewControl, map);
            _bookmarksModel.BookmarksLoadedSuccess += new EventHandler(_bookmarksModel_BookmarksLoadedSuccess);
            _bookmarksModel.BookmarkSaveSuccess += new EventHandler(_bookmarksModel_BookmarkSaveSuccess);

            _bookmarksModel.BookmarksLoadedFailed += new EventHandler(_bookmarksModel_BookmarksLoadedFailed);
            _bookmarksModel.BookmarkSaveFailed += new EventHandler(_bookmarksModel_BookmarkSaveFailed);

            _bookmarksModel.BookmarkLoaded += new EventHandler(_bookmarksModel_BookmarkLoaded);
            _map = map;
        }

        void _bookmarksModel_BookmarkLoaded(object sender, EventArgs e)
        {
            BookmarksListBox.SelectedIndex = -1;
        }

        void _map_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BookmarksListBox.MaxHeight = _map.ActualHeight;
        }

        void _bookmarksModel_BookmarkSaveFailed(object sender, EventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("An error occurred while saving Bookmarks");
        }

        void _bookmarksModel_BookmarksLoadedFailed(object sender, EventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("An error occurred while loading Bookmarks");
        }

        public void Load()
        {
            ConfigUtility.UpdateStatusBarText("Loading Bookmarks/Favorites...");
            _bookmarksModel.Load();            
        }
        void _bookmarksModel_BookmarkSaveSuccess(object sender, EventArgs e)
        {
            BookmarksListBox.UpdateLayout();
        }

        void _bookmarksModel_BookmarksLoadedSuccess(object sender, EventArgs e)
        {
            BookmarksButton.IsEnabled = true;
            BookmarksListBox.DataContext = _bookmarksModel.Bookmarks;
            BookmarksListBox.UpdateLayout();
            if (_firstTime)
            {
                _firstTime = false;
                _bookmarksModel.RestoreDefault();
            }
            ConfigUtility.UpdateStatusBarText("");
            BookmarksListBox.MaxHeight = _map.ActualHeight;
            // For some reason dynamic binding won't refresh -- 
            //BookmarksListBox.SetBinding(ListBox.MaxHeightProperty,
            //    new Binding() { Source = _map.Parent, Path = new PropertyPath("ActualHeight"), Mode = BindingMode.OneWay, BindsDirectlyToSource = true });
            //BookmarksListBox.UpdateLayout();
            _map.SizeChanged += new SizeChangedEventHandler(_map_SizeChanged);
        }


        private void AddBookmarkButton_OnClick(object sender, RoutedEventArgs e)
        {
            NewBookmarkWindow newBookmarkWindow = new NewBookmarkWindow();
            newBookmarkWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(newBookmarkWindow_Closing);
            newBookmarkWindow.Closed += new EventHandler(newBookmarkWindow_Closed);
            newBookmarkWindow.Show();
        }

        void newBookmarkWindow_Closed(object sender, EventArgs e)
        {
            NewBookmarkWindow newBookmarkWindow = sender as NewBookmarkWindow;

            if (newBookmarkWindow.DialogResult == true)
            {
                _bookmarksModel.Add(newBookmarkWindow.TxtBookmark.Text);
            }
        }

        void newBookmarkWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (((ChildWindow) sender).DialogResult == false) return;

            NewBookmarkWindow newBookmarkWindow = sender as NewBookmarkWindow;
            if (_bookmarksModel.BookmarksDictionary.ContainsKey(newBookmarkWindow.TxtBookmark.Text.ToLower()))
            {
                MessageBoxResult result = MessageBox.Show(
                    "The Bookmark [ " + newBookmarkWindow.TxtBookmark.Text +
                    " ] already exists. Press OK to overwrite it",
                    "Add a Bookmark", MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                {
                    e.Cancel = true;
                }
            }
        }

        private void BookmarksListBox_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), (sender as ListBox)).OfType<ListBoxItem>().First();
            _selectedBookmark = (Bookmark)listBoxItem.Content;
            BookmarksListBox.SelectedItem = null;
        }


        private void BookmarksListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0 || BookmarksListBox.SelectedIndex == -1) return;

            Bookmark bookmark = e.AddedItems[0] as Bookmark;
            _bookmarksModel.Open(bookmark);
            //            ((ListBox)sender).SelectedItem = null;
        }

        private void BookmarksButton_OnChecked(object sender, RoutedEventArgs e)
        {
//            FavoritesPopup.IsOpen = false;
            BookmarksPopup.IsOpen = true;
        }

        private void BookmarksButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            BookmarksPopup.IsOpen = false;
        }
    }
}
