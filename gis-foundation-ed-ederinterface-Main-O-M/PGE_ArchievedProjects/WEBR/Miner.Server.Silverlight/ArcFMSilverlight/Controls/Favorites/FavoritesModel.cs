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
using ESRI.ArcGIS.Client.Toolkit.Primitives;
using Miner.Server.Client;

namespace ArcFMSilverlight.Controls.Favorites
{
    public class FavoritesModel
    {
        public event EventHandler FavoritesLoadedSuccess;

        //Commented for WEBR Stability - Replace Feature Service with WCF for Favorites
        //public event EventHandler FavoritesLoadedFailed;

        //public event EventHandler FavoriteSaveSuccess;
        //public event EventHandler FavoriteSaveFailed;

        public event EventHandler FavoriteLoaded;

        //private string _serviceUrl;
        private string _user;
        private FeatureLayer _featureLayer = null;
        private StoredViewControl _storedViewControl;
        private MapTools _mapTools;
        private Map _map;
        private ObservableCollection<Favorite> _favorites = new ObservableCollection<Favorite>();
        private IDictionary<string, Favorite> _favoritesDictionary = new Dictionary<string, Favorite>();
        private Favorite _selectedFavorite;

        public IList<Favorite> Favorites
        {
            get
            {
                return _favorites;
            }

        }


        private ESRI.ArcGIS.Client.Toolkit.Legend LegendTOC
        {
            get
            {
                return _mapTools.LayerControl.TOC;
            }
        }

        public IDictionary<string, Favorite> FavoritesDictionary
        {
            get
            {
                return _favoritesDictionary;
            }
        }

        public FavoritesModel(MapTools mapTools, Map map)   //WEBR Stability - Replace Feature Service with WCF for Favorites
        {
            //_serviceUrl = serviceUrl;
            _user = WebContext.Current.User.Name.Replace("PGE\\", "");
            _storedViewControl = mapTools.StoredViewControl;
            _mapTools = mapTools;
            _map = map;
        }

        //public void Load()
        //{
        //    _featureLayer = new FeatureLayer() { Url = _serviceUrl, Mode = FeatureLayer.QueryMode.Snapshot};
        //    _featureLayer.Where = "USERNAME='" + _user + "'";
        //    ESRI.ArcGIS.Client.Tasks.OutFields myOutFields = new ESRI.ArcGIS.Client.Tasks.OutFields();
        //    myOutFields.Add("*");
        //    _featureLayer.OutFields = myOutFields;
        //    _featureLayer.Initialized += new EventHandler<EventArgs>(_featureLayer_Initialized);
        //    _featureLayer.InitializationFailed += new EventHandler<EventArgs>(_featureLayer_InitializationFailed);
        //    _featureLayer.Initialize();
        //}

        //void _featureLayer_InitializationFailed(object sender, EventArgs e)
        //{
        //    if (FavoritesLoadedFailed != null)
        //    {
        //        this.FavoritesLoadedFailed(new object(), new EventArgs());
        //    }
        //}

        //void _featureLayer_Initialized(object sender, EventArgs e)
        //{
        //    var fl = sender as FeatureLayer;
        //    fl.UpdateCompleted += new EventHandler(fl_UpdateCompleted);
        //    fl.UpdateFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(fl_UpdateFailed);
        //    fl.Update();
        //}

        //void fl_UpdateFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        //{
        //    if (FavoritesLoadedFailed != null)
        //    {
        //        this.FavoritesLoadedFailed(new object(), new EventArgs());
        //    }
        //}

        //void fl_UpdateCompleted(object sender, EventArgs e)
        //{
        //    var fl = sender as FeatureLayer;
        //    // No attributes are coming thru, just ObjectId
        //    GraphicsToPocos(fl.Graphics);

        //    // Raise an event to say that this is done
        //    if (FavoritesLoadedSuccess != null)
        //    {
        //        this.FavoritesLoadedSuccess(new object(), new EventArgs());
        //    }
        //}


        //private void GraphicsToPocos(GraphicCollection graphics)
        //{
        //    _favorites.Clear();
        //    _favoritesDictionary.Clear();

        //    for (int i = 0; i < graphics.Count; i++)
        //    {
        //        Graphic graphic = graphics[i];

        //        Favorite favorite = new Favorite();
        //        favorite.UserName = graphic.Attributes["USERNAME"].ToString();
        //        favorite.Name = graphic.Attributes["NAME"].ToString();
        //        favorite.LayerVisibilitiesJson = graphic.Attributes["LAYERVISIBILITIES"].ToString();
        //        favorite.StoredView = graphic.Attributes["STOREDVIEW"].ToString();
        //        favorite.ObjectId = Convert.ToInt32(graphic.Attributes["OBJECTID"]);

        //        _favorites.Add(favorite);
        //        _favoritesDictionary.Add(favorite.Name.ToLower(), favorite);
        //    }
        //    _favorites = new ObservableCollection<Favorite>(_favorites.OrderBy(f => f.Name));
        //}

        public void PushFavoritesList(List<FavoritesData> favoritesList)
        {
            GraphicsToPocos(favoritesList);

            // Raise an event to say that this is done
            if (FavoritesLoadedSuccess != null)
            {
                this.FavoritesLoadedSuccess(new object(), new EventArgs());
            }
        }

        private void GraphicsToPocos(List<FavoritesData> favoritesList)
        {
            _favorites.Clear();
            _favoritesDictionary.Clear();

            for (int i = 0; i < favoritesList.Count; i++)
            {
                FavoritesData graphic = favoritesList[i];

                Favorite favorite = new Favorite();
                favorite.UserName = Convert.ToString(favoritesList[i].UserName);
                favorite.Name = Convert.ToString(favoritesList[i].Name);
                favorite.LayerVisibilitiesJson = Convert.ToString(favoritesList[i].LayerVisibilities);
                favorite.StoredView = Convert.ToString(favoritesList[i].StoredView);
                favorite.ObjectId = Convert.ToInt32(favoritesList[i].ObjectId);

                _favorites.Add(favorite);
                _favoritesDictionary.Add(favorite.Name.ToLower(), favorite);
            }
            _favorites = new ObservableCollection<Favorite>(_favorites.OrderBy(f => f.Name));
        }

        //public void Add(string name)
        //{
        //    Graphic graphic = null;
        //    if (_favoritesDictionary.ContainsKey(name.ToLower()))
        //    {
        //        // Update existing
        //        graphic =
        //            _featureLayer.Graphics.Where(g => g.Attributes["NAME"].ToString().ToLower() == name.ToLower())
        //                .First();
        //        graphic.Attributes.Remove("STOREDVIEW");
        //        graphic.Attributes.Remove("LAYERVISIBILITIES");
        //    }
        //    else
        //    {
        //        // Add new
        //        graphic = new Graphic();
        //        graphic.Attributes.Add("USERNAME", _user);
        //        graphic.Attributes.Add("NAME", name);
        //        _featureLayer.Graphics.Add(graphic);
        //    }
        //    graphic.Attributes.Add("STOREDVIEW", _storedViewControl._selectedStoredView.StoredViewName);
        //    try
        //    {
        //        graphic.Attributes.Add("LAYERVISIBILITIES", GetCurrentLayerVisibilities());
        //        _featureLayer.EndSaveEdits += new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_featureLayer_EndSaveEdits);
        //        _featureLayer.SaveEditsFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_featureLayer_SaveEditsFailed);
        //        _featureLayer.SaveEdits();
        //    }
        //    catch (Exception exception)
        //    {
        //        MessageBox.Show(exception.ToString());
        //    }
        //}

        public string InsertOrUpdate(string name)
        {
            if (_favoritesDictionary.ContainsKey(name.ToLower()))
                return "UPDATE";
            else
                return "INSERT";
        }

        public void Open(Favorite favorite)
        {
            favorite.LayerVisibilities = DeserializeLayerVisibilitiesJson(favorite.LayerVisibilitiesJson);

            // Switch stored display, when that's done then apply layervisibilities
            if (favorite.StoredView != _storedViewControl._selectedStoredView.StoredViewName)
            {
                _storedViewControl.StoredViewChangeEvent += new EventHandler(_storedViewControl_StoredViewChangeEvent);
                _selectedFavorite = favorite;
                _storedViewControl.SetStoredViewByName(favorite.StoredView);
                // Do we have to wait for the refresh to occur here and if so, which event to watch for?
            }
            else
            {
                RestoreLayerVisibilities(favorite.LayerVisibilities);
            }

            _map.Progress += new EventHandler<ProgressEventArgs>(_map_Progress);
        }

        void _map_Progress(object sender, ProgressEventArgs e)
        {
            if (e.Progress == 100)
            {
                _map.Progress -= _map_Progress;

                if (FavoriteLoaded != null)
                {
                    FavoriteLoaded(new object(), new EventArgs());
                }
            }
        }

        void _storedViewControl_StoredViewChangeEvent(object sender, EventArgs e)
        {
            _storedViewControl.StoredViewChangeEvent -= new EventHandler(_storedViewControl_StoredViewChangeEvent);
            RestoreLayerVisibilities(_selectedFavorite.LayerVisibilities);
        }



        private void RestoreLayerVisibilities(LayerVisibilityContainer layerVisibilityContainer)
        {
            foreach (LayerVisibility layerVisibility in layerVisibilityContainer.LayerVisibilities)
            {
                try
                {
                    Layer layer = _map.Layers[layerVisibility.Name];

                    layer.Visible = layerVisibility.Visible;

                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        ArcGISDynamicMapServiceLayer dynamicMapServiceLayer = (ArcGISDynamicMapServiceLayer)layer;
                        // NB this will restore default visibility if it's null
                        dynamicMapServiceLayer.VisibleLayers = layerVisibility.VisibleLayers;
                        // If we're not at default visibility then let's restore subLayers to their checked state
                        //if (layerVisibility.VisibleLayers != null)
                        //{
                        LayerItemViewModel layerItemViewModel = GetLayerItemViewModelByLayerId(layer.ID);
                        RestoreIsEnabledLayers(layerItemViewModel.LayerItems, layerVisibility.IsEnabledLayers);
                        //}
                    }
                }
                catch
                {
                }

            }

        }

        private void RestoreIsEnabledLayers(ObservableCollection<LayerItemViewModel> layerItems, int[] isEnabledLayers)
        {
            foreach (LayerItemViewModel layerItemViewModel in layerItems)
            {
                // If this layer should be enabled but isn't currently then let's enable it
                if (isEnabledLayers.Contains(layerItemViewModel.SubLayerID) && layerItemViewModel.IsEnabled == false)
                {
                    // NB this changes the TOC and calls an exportMap :(. Not sure how to disable that.
                    layerItemViewModel.IsEnabled = true;
                }
                if (layerItemViewModel.LayerItems != null)
                {
                    RestoreIsEnabledLayers(layerItemViewModel.LayerItems, isEnabledLayers);
                }
            }
        }


        private LayerVisibilityContainer DeserializeLayerVisibilitiesJson(string layerVisibilityJson)
        {
            using (MemoryStream stream1 = new MemoryStream(Encoding.UTF8.GetBytes(layerVisibilityJson)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LayerVisibilityContainer));

                return ser.ReadObject(stream1) as LayerVisibilityContainer;
            }
        }

        private void GetLayerVisibilitiesFromTree(ObservableCollection<LayerItemViewModel> layerItems, IList<int> layerVisibilities)
        {
            foreach (LayerItemViewModel layerItemViewModel in layerItems)
            {
                if (layerItemViewModel.IsEnabled)
                {
                    layerVisibilities.Add(layerItemViewModel.SubLayerID);
                }
                if (layerItemViewModel.LayerItems != null)
                {
                    GetLayerVisibilitiesFromTree(layerItemViewModel.LayerItems, layerVisibilities);
                }
            }
        }


        public LayerItemViewModel GetLayerItemViewModelByLayerId(string id)
        {
            try
            {
                for (int i = 0; i < LegendTOC.LayerIDs.Length; i++)
                {
                    if (LegendTOC.LayerIDs[i] == id)
                    {
                        return LegendTOC.LayerItems[i];
                    }
                }
                return null;
            }
            catch (Exception)
            {
                MessageBox.Show("GLTIVWMBI");
                throw;
            }
        }

        static public LayerItem GetLayerItemByLayerId(string id)
        {
            foreach (LayerItem layerItem in LayerVisibilityTree.Root)
            {
                if (layerItem.Layer.ID == id)
                {
                    return layerItem;
                }
            }
            return null;
        }

        public string GetCurrentLayerVisibilities()
        {
            string json = null;
            string layerIdDebug = "";

            try
            {
                LayerVisibilityContainer containerObject = new LayerVisibilityContainer();

                // A null error is happening somewhere below but only running on other machines
                IList<LayerVisibility> layerVisibilitys = new List<LayerVisibility>();
                for (int i = 0; i < _map.Layers.Count; i++)
                {
                    Layer layer = _map.Layers[i];
                    if (layer == null || layer.ID == null || layer.ID.Contains(ShowRolloverInfo.ShowRolloverInfo.ROLLOVER_FEATURELAYER_PREFIX) || layer.ID == "PLCINFO_Labels")
                    {
                        continue;
                    }
                    layerIdDebug = layer.ID;

                    PopulateLayerVisibility(layer, layerVisibilitys);

                }

                containerObject.LayerVisibilities = layerVisibilitys.ToArray();
                json = SerializeLayerVisibilityContainer(containerObject);

                return json;
            }
            catch (Exception exception)
            {
                MessageBox.Show(layerIdDebug + " " + exception.ToString());
                throw;
            }

        }

        private void PopulateLayerVisibility(Layer layer, IList<LayerVisibility> layerVisibilitys)
        {
            LayerVisibility layerVisibility = new LayerVisibility();
            layerVisibility.Name = layer.ID;
            layerVisibility.Visible = layer.Visible;
            if (layer is ArcGISDynamicMapServiceLayer)
            {
                LayerItemViewModel layerItemViewModel = GetLayerItemViewModelByLayerId(layer.ID);
                if (layerItemViewModel == null) MessageBox.Show(layer.ID + " no LIVM!!");

                IList<int> layerVisibilities = new List<int>();
                if (layerItemViewModel.LayerItems != null)
                {
                    GetLayerVisibilitiesFromTree(layerItemViewModel.LayerItems, layerVisibilities);
                    layerVisibility.IsEnabledLayers = layerVisibilities.ToArray();
                }
                layerVisibility.VisibleLayers = ((ArcGISDynamicMapServiceLayer)layer).VisibleLayers;
            }
            layerVisibilitys.Add(layerVisibility);
        }

        private string SerializeLayerVisibilityContainer(LayerVisibilityContainer containerObject)
        {
            MemoryStream stream1 = new MemoryStream();
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(LayerVisibilityContainer));
            ser.WriteObject(stream1, containerObject);
            stream1.Position = 0;
            StreamReader sr = new StreamReader(stream1);

            return sr.ReadToEnd();
        }


        //void _featureLayer_SaveEditsFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        //{
        //    //TODO: remove this when error is propagated
        //    MessageBox.Show("An error occurred Saving Favorites");
        //    // Raise an event to say that this is done
        //    if (FavoriteSaveFailed != null)
        //    {
        //        this.FavoriteSaveFailed(new object(), new EventArgs());
        //    }
        //}

        //void _featureLayer_EndSaveEdits(object sender, ESRI.ArcGIS.Client.Tasks.EndEditEventArgs e)
        //{
        //    // Update the local collection
        //    _featureLayer.Update();

        //    // Raise an event to say that this is done
        //    if (FavoriteSaveSuccess != null)
        //    {
        //        this.FavoriteSaveSuccess(new object(), new EventArgs());
        //    }
        //}

        //public void Delete(int objectId)
        //{
        //    foreach (Graphic graphic in _featureLayer.Graphics)
        //    {
        //        if (Convert.ToInt32(graphic.Attributes["OBJECTID"]) == objectId)
        //        {
        //            _featureLayer.Graphics.Remove(graphic);
        //            break;
        //        }
        //    }

        //    _featureLayer.EndSaveEdits += new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_featureLayer_EndSaveEdits);
        //    _featureLayer.SaveEditsFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_featureLayer_SaveEditsFailed);
        //    _featureLayer.SaveEdits();
        //}

    }

    public class Favorite
    {
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public string LayerVisibilitiesJson;
        public string LayersIsEnabledJson;
        public LayerVisibilityContainer LayerVisibilities;
        public string UserName;
        public string StoredView;
    }

    [DataContract]
    public class LayerVisibilityContainer
    {
        [DataMember]
        public LayerVisibility[] LayerVisibilities;
    }
    [DataContract]
    public class LayerVisibility
    {
        [DataMember]
        public string Name;
        [DataMember]
        public bool Visible;
        [DataMember]
        public int[] VisibleLayers;
        [DataMember]
        public int[] IsEnabledLayers;
    }

}
