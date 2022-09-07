using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ArcFM.Silverlight.PGE.CustomTools;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using Miner.Server.Client;
using Polygon = ESRI.ArcGIS.Client.Geometry.Polygon;

namespace ArcFMSilverlight.Controls.Bookmarks
{
    public class BookmarksModel
    {
        public event EventHandler BookmarksLoadedSuccess;
        public event EventHandler BookmarksLoadedFailed;

        public event EventHandler BookmarkSaveSuccess;
        public event EventHandler BookmarkSaveFailed;

        public event EventHandler BookmarkLoaded;

        private string _serviceUrl;
        private string _user;
        private FeatureLayer _featureLayer = null;
        private StoredViewControl _storedViewControl;
        private Map _map;
        private ObservableCollection<Bookmark> _bookmarks = new ObservableCollection<Bookmark>();
        private IDictionary<string,Bookmark> _bookmarksDictionary = new Dictionary<string, Bookmark>();

        public IList<Bookmark> Bookmarks
        {
            get
            {
                return _bookmarks;
            }
        }

        public IDictionary<string, Bookmark> BookmarksDictionary
        {
            get
            {
                return _bookmarksDictionary;
            }
        }

        public BookmarksModel(string serviceUrl, StoredViewControl storedViewControl, Map map)
        {
            _serviceUrl = serviceUrl;
            _user = WebContext.Current.User.Name.Replace("PGE\\", "");
            _storedViewControl = storedViewControl;
            _map = map;
        }

        public void Load()
        {
            _featureLayer = new FeatureLayer() { Url = _serviceUrl, Mode = FeatureLayer.QueryMode.Snapshot};
            _featureLayer.Where = "USERNAME='" + _user + "'";
            ESRI.ArcGIS.Client.Tasks.OutFields myOutFields = new ESRI.ArcGIS.Client.Tasks.OutFields();
            myOutFields.Add("*");
            _featureLayer.OutFields = myOutFields;
            _featureLayer.Initialized += new EventHandler<EventArgs>(_featureLayer_Initialized);
            _featureLayer.InitializationFailed += new EventHandler<EventArgs>(_featureLayer_InitializationFailed);
            _featureLayer.Initialize();
        }

        void _featureLayer_InitializationFailed(object sender, EventArgs e)
        {
            if (BookmarksLoadedFailed != null)
            {
                this.BookmarksLoadedFailed(new object(), new EventArgs());
            }
        }

        void _featureLayer_Initialized(object sender, EventArgs e)
        {
            var fl = sender as FeatureLayer;
            fl.UpdateCompleted += new EventHandler(fl_UpdateCompleted);
            fl.UpdateFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(fl_UpdateFailed);
            fl.Update();
        }

        void fl_UpdateFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            if (BookmarksLoadedFailed != null)
            {
                this.BookmarksLoadedFailed(new object(), new EventArgs());
            }
        }

        void fl_UpdateCompleted(object sender, EventArgs e)
        {
            var fl = sender as FeatureLayer;
            // No attributes are coming thru, just ObjectId
            GraphicsToPocos(fl.Graphics);

            // Raise an event to say that this is done
            if (BookmarksLoadedSuccess != null)
            {
                this.BookmarksLoadedSuccess(new object(), new EventArgs());
            }
        }

        public void UnsetDefault()
        {
            IList<Bookmark> defaultBookmarks = _bookmarks.Where(b => b.DefaultYN == true).ToList();
            foreach (Bookmark defaultBookmark in defaultBookmarks)
            {
                defaultBookmark.DefaultYN = false;
            }

            SynchronizeDefault();
        }

        void SynchronizeDefault()
        {
            foreach (Graphic graphic in _featureLayer.Graphics)
            {
                graphic.Attributes["DEFAULTYN"] = _bookmarksDictionary[graphic.Attributes["NAME"].ToString().ToLower()].DefaultYN ? "Y" : "N";
            }

            _featureLayer.EndSaveEdits += new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_featureLayer_EndSaveEdits);
            _featureLayer.SaveEditsFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_featureLayer_SaveEditsFailed);
            _featureLayer.SaveEdits();
            
        }

        public void SetDefault(Bookmark bookmark)
        {
            IList<Bookmark> defaultBookmarks = _bookmarks.Where(b => b.DefaultYN == true).ToList();
            foreach (Bookmark defaultBookmark in defaultBookmarks)
            {
                defaultBookmark.DefaultYN = false;
            }
            bookmark.DefaultYN = true;

            SynchronizeDefault();
        }

        public void RestoreDefault()
        {
            Bookmark defaultBookmark = _bookmarks.Where(b => b.DefaultYN == true).FirstOrDefault();
            if (defaultBookmark != null)
            {
                ConfigUtility.UpdateStatusBarText("Setting Default Bookmark...");
                Open(defaultBookmark);
            }
        }

        private void GraphicsToPocos(GraphicCollection graphics)
        {
            _bookmarks.Clear();
            _bookmarksDictionary.Clear();

            for (int i = 0; i < graphics.Count; i++)
            {
                Graphic graphic = graphics[i];

                Bookmark Bookmark = new Bookmark();
                Bookmark.UserName = graphic.Attributes["USERNAME"].ToString();
                Bookmark.Name = graphic.Attributes["NAME"].ToString();
                Bookmark.Extent = graphic.Geometry;
                Bookmark.ObjectId = Convert.ToInt32(graphic.Attributes["OBJECTID"]);
                Bookmark.DefaultYN = graphic.Attributes["DEFAULTYN"] != null && graphic.Attributes["DEFAULTYN"].ToString() == "Y" ? true: false;

                _bookmarks.Add(Bookmark);
                _bookmarksDictionary.Add(Bookmark.Name.ToLower(), Bookmark);
            }
            _bookmarks = new ObservableCollection<Bookmark>(_bookmarks.OrderBy(f => f.Name));
            
        }

        public void Add(string name)
        {
            Graphic graphic = null;
            if (_bookmarksDictionary.ContainsKey(name.ToLower()))
            {
                // Update existing
                graphic =
                    _featureLayer.Graphics.Where(g => g.Attributes["NAME"].ToString().ToLower() == name.ToLower())
                        .First();
            }
            else
            {
                // Add new
                graphic = new Graphic();
                graphic.Attributes.Add("USERNAME", _user);
                graphic.Attributes.Add("NAME", name);
                _featureLayer.Graphics.Add(graphic);
            }
            Polygon polygon = new Polygon();
            polygon.SpatialReference = _map.SpatialReference;
            polygon.Rings.Add(new ESRI.ArcGIS.Client.Geometry.PointCollection());
            polygon.Rings[0].Add(new MapPoint(_map.Extent.XMin, _map.Extent.YMin));
            polygon.Rings[0].Add(new MapPoint(_map.Extent.XMin, _map.Extent.YMax));
            polygon.Rings[0].Add(new MapPoint(_map.Extent.XMax, _map.Extent.YMax));
            polygon.Rings[0].Add(new MapPoint(_map.Extent.XMax, _map.Extent.YMin));
            graphic.Geometry = polygon;
            _featureLayer.EndSaveEdits += new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_featureLayer_EndSaveEdits);
            _featureLayer.SaveEditsFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_featureLayer_SaveEditsFailed);
            _featureLayer.SaveEdits();
        }

        public void Open(Bookmark bookmark)
        {
            _map.Progress += new EventHandler<ProgressEventArgs>(_map_Progress);
            _map.Extent = bookmark.Extent.Extent;            
        }

        void _map_Progress(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                _map.Progress -= _map_Progress;
                if (BookmarkLoaded != null)
                {
                    BookmarkLoaded(new object(), new EventArgs());
                }
            }
        }


        void _featureLayer_SaveEditsFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            // Raise an event to say that this is done
            if (BookmarkSaveFailed != null)
            {
                this.BookmarkSaveFailed(new object(), new EventArgs());
            }
        }

        void _featureLayer_EndSaveEdits(object sender, ESRI.ArcGIS.Client.Tasks.EndEditEventArgs e)
        {
            // Update the local collection
            _featureLayer.Update();

            // Raise an event to say that this is done
            if (BookmarkSaveSuccess != null)
            {
                this.BookmarkSaveSuccess(new object(), new EventArgs());
            }
        }

        public void Delete(int objectId)
        {
            foreach (Graphic graphic in _featureLayer.Graphics)
            {
                if (Convert.ToInt32(graphic.Attributes["OBJECTID"]) == objectId)
                {
                    _featureLayer.Graphics.Remove(graphic);
                    break;
                }
            }

            _featureLayer.EndSaveEdits += new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_featureLayer_EndSaveEdits);
            _featureLayer.SaveEditsFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_featureLayer_SaveEditsFailed);
            _featureLayer.SaveEdits();
        }

    }

    public class Bookmark
    {
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public bool DefaultYN { get; set; }
        public ESRI.ArcGIS.Client.Geometry.Geometry Extent;
        public string UserName;
    }


}
