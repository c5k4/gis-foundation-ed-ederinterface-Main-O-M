using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ESRI.ArcGIS.Client;
using ArcFM.Silverlight.PGE.CustomTools;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Media.Imaging;
using ESRI.ArcGIS.Client.Tasks;
using Miner.Server.Client.Toolkit;
using System.Windows.Controls.Primitives;
using ESRI.ArcGIS.Client.Toolkit;

using NLog;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit.Events;
using System.Xml.Linq;
using ESRI.ArcGIS.Client.Geometry;

namespace ArcFMSilverlight
{
    public partial class NotesWidget : UserControl
    {
        #region Constructor

        public NotesWidget()
        {
            InitializeComponent();
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            notesAttributeEditorWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(notesAttributeEditorWindow_Closing);
            if (ToggleNotes.IsChecked == false)
            {
                NotesCreate.IsEnabled = false;
                NotesEdit.IsEnabled = false;
                NotesDelete.IsEnabled = false;
            }
        }

        #endregion

        #region Private Fields

        private List<string> _notesLabelFieldNames = new List<string>();
        private string _notesLayerName = string.Empty;
        private string _notesMapServiceUrl = string.Empty;
        private string _notesFeatureServiceUrl = string.Empty;
        bool _isActive = true;
        private bool isDelete = false;
        private static bool isCreateNotesCurrentlyActive = false;
        private static bool isEditNotesCurrentlyActive = false;
        private static bool isDeleteNotesCurrentlyActive = false;
        private const string PinCursor = @"/Images/pin.png";
        private const string SelectionCursor = @"/ESRI.ArcGIS.Client.Toolkit;component/Images/NewSelection.png";
        private const string DeleteCursor = @"/ESRI.ArcGIS.Client.Toolkit;component/Images/deleteFeature.png";
        private Logger logger = LogManager.GetCurrentClassLogger();
        NotesAttributeEditor notesAttributeEditorWindow = new NotesAttributeEditor();
        private static string NotesMarkerID = "NotesMarkerID";

        #endregion

        #region Properties

        public List<string> NotesLabelFieldNames
        {
            get { return _notesLabelFieldNames; }
            set { _notesLabelFieldNames = value; }
        }

        public string NotesLayerName
        {
            get { return _notesLayerName; }
            set { _notesLayerName = value; }
        }

        public string NotesMapServiceUrl
        {
            get { return _notesMapServiceUrl; }
            set { _notesMapServiceUrl = value; }
        }

        public string NotesFeatureServiceUrl
        {
            get { return _notesFeatureServiceUrl; }
            set { _notesFeatureServiceUrl = value; }
        }

        public TextBlock StatusBar { get; set; }
        #endregion

        #region Dependancy Properties

        public Map MapProperty
        {
            get { return (Map)GetValue(MapPropertyProperty); }
            set { SetValue(MapPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPropertyProperty =
            DependencyProperty.Register("MapProperty", typeof(Map), typeof(NotesWidget), null);

        public string[] LayerIDs
        {
            get { return (string[])GetValue(LayerIDsProperty); }
            set { SetValue(LayerIDsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayerIDs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayerIDsProperty =
            DependencyProperty.Register("LayerIDs", typeof(string[]), typeof(NotesWidget), null);


        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveCreateNotesProperty = DependencyProperty.Register(
            "IsCreateNotesActive",
            typeof(bool),
            typeof(NotesWidget),
            new PropertyMetadata(OnIsCreateNotesActiveChanged));

        private static void OnIsCreateNotesActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            var control = (NotesWidget)d;
            isCreateNotesCurrentlyActive = (bool)e.NewValue;
            if (isCreateNotesCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, PinCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("pin.png") == true)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveEditNotesProperty = DependencyProperty.Register(
            "IsEditNotesActive",
            typeof(bool),
            typeof(NotesWidget),
            new PropertyMetadata(OnIsEditNotesActiveChanged));

        private static void OnIsEditNotesActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NotesWidget)d;
            isEditNotesCurrentlyActive = (bool)e.NewValue;
            if (isEditNotesCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, SelectionCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("NewSelection.png") == true)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveDeleteNotesProperty = DependencyProperty.Register(
            "IsDeleteNotesActive",
            typeof(bool),
            typeof(NotesWidget),
            new PropertyMetadata(OnIsDeleteNotesActiveChanged));

        private static void OnIsDeleteNotesActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NotesWidget)d;
            isDeleteNotesCurrentlyActive = (bool)e.NewValue;
            if (isDeleteNotesCurrentlyActive)
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.MapProperty == null) return;
                CursorSet.SetID(control.MapProperty, DeleteCursor);
            }
            else
            {
                string currentMapCursor = CursorSet.GetID(control.MapProperty);
                if (currentMapCursor.Contains("deleteFeature.png") == true)
                    CursorSet.SetID(control.MapProperty, "Arrow");
            }
        }

        #endregion

        #region Notes Event Handlers

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            if (NotesCreate.IsChecked == true)
            {
                IsCreateNotesActive = true;
                isDelete = false;
            }
            else
            {
                IsCreateNotesActive = false;
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            if (NotesEdit.IsChecked == true)
            {
                IsEditNotesActive = true;
                isDelete = false;
            }
            else
            {
                IsEditNotesActive = false;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            if (NotesDelete.IsChecked == true)
            {
                IsDeleteNotesActive = true;
                isDelete = true;
            }
            else
            {
                IsDeleteNotesActive = false;
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            GraphicsLayer layer = MapProperty.Layers[NotesMarkerID] as GraphicsLayer;
            if (layer != null)
            {
                if (layer.Graphics.Count > 0)
                {
                    layer.Graphics.Clear();
                    ConfigUtility.StatusBar.Text = "Notes highlights cleared.";
                    NotesClear.IsEnabled = false;
                }
            }
        }

        private void Map_MouseClickCreate(object sender, Map.MouseEventArgs e)
        {
            var geometry = e.MapPoint;
            Graphic graphic = new Graphic()
            {
                Geometry = e.MapPoint,
            };

            AddGraphic(geometry);
            SetNotesAttributeWinFields(NotesLayerName, MapProperty, graphic, null, null);

            notesAttributeEditorWindow.Show();
        }

        void notesAttributeEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NotesAttributeEditor childWindow = sender as NotesAttributeEditor;
            if (childWindow.CancelButton.IsPressed)
            {
                GraphicsLayer layer = MapProperty.Layers[NotesMarkerID] as GraphicsLayer;
                if (layer != null)
                {
                    if (layer.Graphics.Count > 0)
                    {
                        layer.Graphics.Clear();
                        NotesClear.IsEnabled = false;
                    }
                }
            }
            if (childWindow.DialogResult == true) return;

            if (childWindow.NotesLayer != null)
                childWindow.NotesLayer.Update();
        }

        private void Map_MouseClickEditDelete(object sender, Map.MouseEventArgs e)
        {
            ExecuteIdentifyTask(e.MapPoint);
        }

        private void identifyTask_ExecuteCompleted(object sender, IdentifyEventArgs e)
        {
            ConfigUtility.StatusBar.Text = string.Empty;
            try
            {
                if (e.IdentifyResults != null)
                {
                    if (e.IdentifyResults.Count > 0)
                    {
                        ESRI.ArcGIS.Client.Geometry.Geometry selectedGeom = e.IdentifyResults[0].Feature.Geometry;
                        AddGraphic(selectedGeom);
                        object objectIdSelected = e.IdentifyResults[0].Feature.Attributes["OBJECTID"];
                        object notesSelected = e.IdentifyResults[0].Feature.Attributes["NOTES"];
                        object ownerIdSelected = e.IdentifyResults[0].Feature.Attributes["OWNER"];
                        
                        if (isDelete)
                        {
                            object creationDate = e.IdentifyResults[0].Feature.Attributes["CREATIONDATE"].ToString();
                            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete following Notes?\n'" + ownerIdSelected + " (" + creationDate + ")\n" + notesSelected + "'\nClick OK to confirm.", "Delete Notes", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                SetNotesAttributeWinFields(NotesLayerName, MapProperty, null, objectIdSelected, notesSelected);
                                notesAttributeEditorWindow.DeleteSelectedNotes();
                                NotesClear.IsEnabled = false;
                            }

                        }
                        else
                        {
                            if (ownerIdSelected != null)
                            {
                                if (ownerIdSelected.ToString().ToUpper() != WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper())
                                {
                                    MessageBox.Show("Selected Note has been created by " + ownerIdSelected + ". Only same user can edit Notes.");
                                    return;
                                }
                            }
                            SetNotesAttributeWinFields(NotesLayerName, MapProperty, null, objectIdSelected, notesSelected);
                            notesAttributeEditorWindow.Show();
                        }
                    }
                    else
                        ConfigUtility.StatusBar.Text = "No Notes found at clicked location.";
                }
            }
            catch (Exception err)
            {
            }

        }

        private void identifyTask_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Failed to find the clicked Notes: " + e.Error);
            ConfigUtility.StatusBar.Text = "Selection of Notes has failed. " + e.Error.Message;
        }

        private void NotesLayerVisibleToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (ConfigUtility.StatusBar != null)
                ConfigUtility.StatusBar.Text = string.Empty;
            if (MapProperty != null)
            {
                Layer NotesLayer = MapProperty.Layers["WIP Notes"];
                if (NotesLayer != null)
                {
                    NotesLayer.Visible = true;
                    NotesCreate.IsEnabled = true;
                    NotesEdit.IsEnabled = true;
                    NotesDelete.IsEnabled = true;
                }
                CursorSet.SetID(MapProperty, "Arrow");
            }
        }

        private void NotesLayerVisibleToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (ConfigUtility.StatusBar != null)
                ConfigUtility.StatusBar.Text = string.Empty;
            if (MapProperty != null)
            {
                Layer NotesLayer = MapProperty.Layers["WIP Notes"];
                if (NotesLayer != null)
                {
                    NotesLayer.Visible = false;
                    NotesCreate.IsEnabled = false;
                    NotesEdit.IsEnabled = false;
                    NotesDelete.IsEnabled = false;
                    NotesCreate.IsChecked = false;
                    NotesEdit.IsChecked = false;
                    NotesDelete.IsChecked = false;
                    IsCreateNotesActive = false;
                    IsEditNotesActive = false;
                    IsDeleteNotesActive = false;
                }
                GraphicsLayer layer = MapProperty.Layers[NotesMarkerID] as GraphicsLayer;
                if (layer != null)
                {
                    if (layer.Graphics.Count > 0)
                    {
                        layer.Graphics.Clear();
                        NotesClear.IsEnabled = false;
                    }
                }
                CursorSet.SetID(MapProperty, "Arrow");
            }
        }

        #endregion

        #region Notes Utility Methods

        private void ExecuteIdentifyTask(MapPoint mapPoint)
        {
            var identifyParameters = new IdentifyParameters();
            identifyParameters.Geometry = mapPoint;
            identifyParameters.LayerIds.Add(0);
            identifyParameters.MapExtent = MapProperty.Extent;
            identifyParameters.LayerOption = LayerOption.visible;
            identifyParameters.Height = (int)MapProperty.ActualHeight;
            identifyParameters.Width = (int)MapProperty.ActualWidth;
            identifyParameters.SpatialReference = MapProperty.SpatialReference;
            identifyParameters.Tolerance = 7;

            var url = _notesMapServiceUrl;
            var identifyTask = new IdentifyTask(url);
            identifyTask.Failed += new EventHandler<TaskFailedEventArgs>(identifyTask_Failed);
            identifyTask.ExecuteCompleted += new EventHandler<IdentifyEventArgs>(identifyTask_ExecuteCompleted);
            identifyTask.ExecuteAsync(identifyParameters);
        }

        private void SetNotesAttributeWinFields(string notesLayerName, Map mapProperty, Graphic graphic, object objectIdSelected, object notesSelected)
        {
            notesAttributeEditorWindow.NotesLayerName = NotesLayerName;
            notesAttributeEditorWindow.MapProperty = MapProperty;
            notesAttributeEditorWindow.Graphic = graphic;
            notesAttributeEditorWindow.ObjectIdSelected = objectIdSelected;
            notesAttributeEditorWindow.NotesSelected = notesSelected;
            if (objectIdSelected != null)   //edit or delete case
            {
                notesAttributeEditorWindow.NotesData.Text = notesSelected.ToString();
                if (notesSelected.ToString().Length > 0)
                {
                    notesAttributeEditorWindow.SaveButton.IsEnabled = true;
                    notesAttributeEditorWindow.ResetButton.IsEnabled = true;
                }
            }
            else  //create case
            {
                notesAttributeEditorWindow.NotesData.Text = "";
                notesAttributeEditorWindow.SaveButton.IsEnabled = false;
                notesAttributeEditorWindow.ResetButton.IsEnabled = false;
            }
            notesAttributeEditorWindow.NotesFeatureServiceUrl = _notesFeatureServiceUrl;
        }

        private GraphicsLayer CreateNotesMarker()
        {
            GraphicsLayer layer = new GraphicsLayer { ID = NotesMarkerID };
            return layer;
        }

        private void AddGraphic(ESRI.ArcGIS.Client.Geometry.Geometry geom)
        {
            GraphicsLayer layer = MapProperty.Layers[NotesMarkerID] as GraphicsLayer;
            if (layer == null)
            {
                layer = CreateNotesMarker();
                MapProperty.Layers.Add(layer);
            }
            layer.Visible = true;
            layer.Graphics.Clear();
            layer.Graphics.Add(new Graphic
            {
                Symbol = new SimpleMarkerSymbol
                {
                    Color = new SolidColorBrush(Color.FromArgb(0x66, 255, 0, 0)),
                    Size = 20,
                    Style = SimpleMarkerSymbol.SimpleMarkerStyle.Circle
                },
                Geometry = geom
            });
            NotesClear.IsEnabled = true;
        }
        #endregion

        #region IActiveControl Members

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (!_isActive)
                {
                    //if (this.EditorNotes.CancelActive.CanExecute(null)) this.EditorNotes.CancelActive.Execute(null);
                }
                else
                {
                    EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                }
            }
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsCreateNotesActive = false;
            IsEditNotesActive = false;
            IsDeleteNotesActive = false;
            if (MapProperty != null)
            {
                GraphicsLayer layer = MapProperty.Layers[NotesMarkerID] as GraphicsLayer;
                if (layer != null)
                    MapProperty.Layers.Remove(layer);
            }
        }

        public bool IsCreateNotesActive
        {
            get
            {
                return (bool)GetValue(IsActiveCreateNotesProperty);
            }
            set
            {
                SetValue(IsActiveCreateNotesProperty, value);
                if (IsCreateNotesActive)
                {
                    IsEditNotesActive = false;
                    IsDeleteNotesActive = false;
                    if (MapProperty != null)
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickCreate);
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickCreate);
                }

                NotesCreate.IsChecked = value;
            }
        }

        public bool IsEditNotesActive
        {
            get
            {
                return (bool)GetValue(IsActiveEditNotesProperty);
            }
            set
            {
                SetValue(IsActiveEditNotesProperty, value);
                if (IsEditNotesActive)
                {
                    IsCreateNotesActive = false;
                    IsDeleteNotesActive = false;
                    if (MapProperty != null)
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                }

                NotesEdit.IsChecked = value;
            }
        }

        public bool IsDeleteNotesActive
        {
            get
            {
                return (bool)GetValue(IsActiveDeleteNotesProperty);
            }
            set
            {
                SetValue(IsActiveDeleteNotesProperty, value);
                if (IsDeleteNotesActive)
                {
                    IsCreateNotesActive = false;
                    IsEditNotesActive = false;
                    if (MapProperty != null)
                        MapProperty.MouseClick += new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                }
                else
                {
                    if (MapProperty != null)
                        MapProperty.MouseClick -= new EventHandler<ESRI.ArcGIS.Client.Map.MouseEventArgs>(Map_MouseClickEditDelete);
                }

                NotesDelete.IsChecked = value;
            }
        }

        #endregion
    }
}
