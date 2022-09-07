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
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Tasks;
using NLog;


namespace ArcFMSilverlight
{
    public partial class NotesAttributeEditor : ChildWindow
    {
        #region private fields
        private object _ownerLanId = null;
        private object _notesData = null;
        private string _notesLayerName = string.Empty;
        private Map _MapProperty = null;
        private Graphic _graphic = null;
        private FeatureLayer _notesLayer = null;
        private object _objectIdSelected = null;
        private object _notesSelected = null;
        private Logger logger = LogManager.GetCurrentClassLogger();
        private bool _isDelete = false;
        private string _notesFeatureServiceUrl = string.Empty;
        private static string NotesMarkerID = "NotesMarkerID";
        private bool _deleteSuccess = false;

        #endregion

        #region constructor
        public NotesAttributeEditor()
        {
            InitializeComponent();
        }
            
        #endregion

        #region Properties
        public FeatureLayer NotesLayer
        {
            get { return _notesLayer; }
            set { _notesLayer = value; }
        }

        public string NotesLayerName
        {
            get { return _notesLayerName; }
            set { _notesLayerName = value; }
        }

        public Map MapProperty
        {
            get { return _MapProperty; }
            set { _MapProperty = value; }
        }

        public Graphic Graphic
        {
            get { return _graphic; }
            set { _graphic = value; }
        }

        public object ObjectIdSelected
        {
            get { return _objectIdSelected; }
            set { _objectIdSelected = value; }
        }

        public object NotesSelected
        {
            get { return _notesSelected; }
            set { _notesSelected = value; }
        }

        public string NotesFeatureServiceUrl
        {
            get { return _notesFeatureServiceUrl; }
            set { _notesFeatureServiceUrl = value; }
        }

        #endregion

        #region event handling
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            SaveNotesAttributes();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = true;
            this.DialogResult = false;
        }       

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetNotesAttributes();
        }

        private void TxtNotes_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (NotesData.Text.ToString().Length > 0)
            {
                SaveButton.IsEnabled = true;
                ResetButton.IsEnabled = true;
            }
            else
            {
                SaveButton.IsEnabled = false;
                ResetButton.IsEnabled = false;
            }
        }        

        private void Noteslayer_EndSaveEdits(object sender, EndEditEventArgs e)
        {
            RefreshNotesLayer();
            if (_isDelete)
            {
                ConfigUtility.StatusBar.Text = "Notes deleted successfully.";
                GraphicsLayer layer = MapProperty.Layers[NotesMarkerID] as GraphicsLayer;
                if (layer != null)
                {
                    if (layer.Graphics.Count > 0)
                    {
                        layer.Graphics.Clear();
                    }
                }
            }
            else
            {
                ConfigUtility.StatusBar.Text = "Notes saved successfully.";
            }
            _notesLayer.EndSaveEdits -= new EventHandler<EndEditEventArgs>(Noteslayer_EndSaveEdits);
            _notesLayer.SaveEditsFailed -= new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
        }

        private void LayerOnSaveEditsFailed(object sender, TaskFailedEventArgs taskFailedEventArgs)
        {
            RefreshNotesLayer();
            if (_isDelete)
            {
                logger.Error("Failed to delete Notes: " + taskFailedEventArgs.Error);
                ConfigUtility.StatusBar.Text = "Delete Notes request to server has failed. " + taskFailedEventArgs.Error.Message;
            }
            else
            {
                logger.Error("Failed to save Notes: " + taskFailedEventArgs.Error);
                ConfigUtility.StatusBar.Text = "Save Notes request to server has failed. " + taskFailedEventArgs.Error.Message;
            }
            _notesLayer.EndSaveEdits -= new EventHandler<EndEditEventArgs>(Noteslayer_EndSaveEdits);
            _notesLayer.SaveEditsFailed -= new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
        }
        #endregion
        #region utility methods
        private void SaveNotesAttributes()
        {
            _notesData = NotesData.Text;
            _isDelete = false;
            InitializeNotesFeatureLayer();                  

        }

        private void InitializeNotesFeatureLayer()
        {
            _notesLayer = new FeatureLayer() { Url = _notesFeatureServiceUrl, Mode = FeatureLayer.QueryMode.Snapshot };

            ESRI.ArcGIS.Client.Tasks.OutFields myOutFields = new ESRI.ArcGIS.Client.Tasks.OutFields(); ;
            myOutFields.Add("*");
            _notesLayer.OutFields = myOutFields;
            _notesLayer.Initialized += new EventHandler<EventArgs>(_notesFeatureLayer_Initialized);
            _notesLayer.InitializationFailed += new EventHandler<EventArgs>(_notesFeatureLayer_InitializationFailed);
            _notesLayer.Initialize(); 
        }

        void notesFeatureLayer_UpdateFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            logger.Error("Notes Feature Layer update failed!");
            var _notesLayer = sender as FeatureLayer;
            _notesLayer.UpdateCompleted -= new EventHandler(notesFeatureLayer_UpdateCompleted);
            _notesLayer.UpdateFailed -= new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(notesFeatureLayer_UpdateFailed);
        }

        void notesFeatureLayer_UpdateCompleted(object sender, EventArgs e)
        {            
            var _notesLayer = sender as FeatureLayer;
            _notesLayer.UpdateCompleted -= new EventHandler(notesFeatureLayer_UpdateCompleted);
            _notesLayer.UpdateFailed -= new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(notesFeatureLayer_UpdateFailed);
            if (_notesLayer != null)
            {
                if (_objectIdSelected == null)    //Create Notes
                {
                    _ownerLanId = WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper();

                    _graphic.Attributes["NOTES"] = _notesData;
                    _graphic.Attributes["OWNER"] = _ownerLanId;
                    _graphic.Attributes["CREATIONDATE"] = DateTime.Now.ToString();

                    _notesLayer.Graphics.Add(_graphic);
                }
                else     //Edit / Delete Notes
                {
                    try
                    {
                        _graphic =
                            _notesLayer.Graphics.Where(
                                g => g.Attributes["OBJECTID"].ToString() == _objectIdSelected.ToString()).First();

                        
                        if (_isDelete)
                            _notesLayer.Graphics.Remove(_graphic);
                        else
                        {
                            _graphic.Attributes["NOTES"] = _notesData;
                            _graphic.Attributes["MODIFIEDDATE"] = DateTime.Now.ToString();
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            if (_graphic == null)
            {
                RefreshNotesLayer();
                if (_isDelete)
                    ConfigUtility.StatusBar.Text = "Failed to delete Notes.";
                else
                    ConfigUtility.StatusBar.Text = "Failed to save Notes.";
            }
            else
            {
                _notesLayer.EndSaveEdits += new EventHandler<EndEditEventArgs>(Noteslayer_EndSaveEdits);
                _notesLayer.SaveEditsFailed += new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
                if (_notesLayer.ValidateEdits) _notesLayer.SaveEdits();

            }
                       
        }

        void _notesFeatureLayer_InitializationFailed(object sender, EventArgs e)
        {
            logger.Error("Notes Feature Layer initialization failed!");
            var _notesLayer = sender as FeatureLayer;
            _notesLayer.Initialized -= new EventHandler<EventArgs>(_notesFeatureLayer_Initialized);
            _notesLayer.InitializationFailed -= new EventHandler<EventArgs>(_notesFeatureLayer_InitializationFailed);
        }

        void _notesFeatureLayer_Initialized(object sender, EventArgs e)
        {
            FeatureLayer _notesLayer = sender as FeatureLayer;
            _notesLayer.Initialized -= new EventHandler<EventArgs>(_notesFeatureLayer_Initialized);
            _notesLayer.InitializationFailed -= new EventHandler<EventArgs>(_notesFeatureLayer_InitializationFailed);
            _notesLayer.UpdateCompleted += new EventHandler(notesFeatureLayer_UpdateCompleted);
            _notesLayer.UpdateFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(notesFeatureLayer_UpdateFailed);
            _notesLayer.Update();   
        }

        private void ResetNotesAttributes(bool flush = true)
        {
            if (flush)
            {
                // For some reason the panel is not updating unless you set it to a non-null value first
                NotesData.Text = "";
            }
            _notesData = null;
        }
        public void DeleteSelectedNotes()            
        {
            _isDelete = true;
            InitializeNotesFeatureLayer();
            
        }

        private void RefreshNotesLayer()
        {
            //FeatureLayer notesLayer = new FeatureLayer() { Url = _notesFeatureServiceUrl };
            //notesLayer.Update();
            ArcGISDynamicMapServiceLayer notesLayer = MapProperty.Layers[_notesLayerName] as ArcGISDynamicMapServiceLayer;
            notesLayer.Refresh();
        }
        #endregion
    }
}

