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
using System.Xml.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using NLog;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit.Events;
using System.Globalization;

namespace ArcFMSilverlight
{
    public partial class WIPEditorWidget : UserControl, IActiveControl
    {
        #region Constructor

        public WIPEditorWidget()
        {
            InitializeComponent();
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            wipAttributeEditorWindow.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(wipAttributeEditorWindow_Closing);
            FeatureDataForm = wipAttributeEditorWindow.WIPAttributeEditorForm;
            wipAttributeEditorWindow.WipEditorWidgetObj = this;
        }

        #endregion

        #region Private Fields

        private bool _isAttributeEditor = false;
        private bool _isEditingLabels = false;
        private List<string> _wipLabelFieldNames = new List<string>();
        private Dictionary<string, object> _newFeatureAttributes = new Dictionary<string, object>();
        private List<Graphic> _addedWIPLabels = new List<Graphic>();
        private const string WipUID = "PARENTID";
        private Logger logger = LogManager.GetCurrentClassLogger();
        private string _wipPolygonLayerName = string.Empty;
        private string _wipLabelLayerName = string.Empty;
        private string _wipOutlineLayerName = string.Empty;  //INC000004019908, INC000004273558
        private FeatureLayer _wipLabelLayer;
        private FeatureLayer _wipPolygonLayer;
        //private Button attributeEditorClose;
        private FeatureDataForm _featureDataForm;
        bool _isActive = true;
        private WIPAttributeEditor wipAttributeEditorWindow = new WIPAttributeEditor();
        Graphic Addwipgraphic = null;
        FeatureDataForm AddWipfeatureDataForm = null;
        private string jobNumberLayerUrl = string.Empty;
        /********************PLC CHANGE 27 Dec Starts****************/
        private Dictionary<string, string> _wipLabelJobNumber = null;
        private FeatureLayer _getWIPGraphicsLayer = null;
        private int wipJobQueryCount = 0;
        /********************PLC CHANGE 27 Dec Starts****************/

        //CAP #114707237 ---ME Q3 2019 Release
        private bool _isMemOfDeleteADGrps = false;

        #endregion

        #region Properties

        public List<string> WipLabelFieldNames
        {
            get { return _wipLabelFieldNames; }
            set { _wipLabelFieldNames = value; }
        }

        public string WipPolygonLayerName
        {
            get { return _wipPolygonLayerName; }
            set { _wipPolygonLayerName = value; }
        }

        public string WipLabelLayerName
        {
            get { return _wipLabelLayerName; }
            set { _wipLabelLayerName = value; }
        }

        //INC000004019908, INC000004273558
        public string WipOutlineLayerName
        {
            get { return _wipOutlineLayerName; }
            set { _wipOutlineLayerName = value; }
        }

        //CAP #114707237 ---ME Q3 2019 Release
        public bool IsMemOfDeleteADGrps
        {
            get { return _isMemOfDeleteADGrps; }
            set { _isMemOfDeleteADGrps = value; }
        }

        public bool IsAttributeEditor
        {
            get { return _isAttributeEditor; }
            set { _isAttributeEditor = value; }
        }
      
        public FeatureLayer WipLabelLayer
        {
            get { return _wipLabelLayer; }
            set
            {
                _wipLabelLayer = value;
                //NotifyPropertyChanged("WipLabelLayer");
                _wipLabelLayer.UpdateCompleted += new EventHandler(WIPLabelUpdateCompleted);
                _wipLabelLayer.EndSaveEdits += new EventHandler<EndEditEventArgs>(WipLabellayer_EndSaveEdits);
                _wipLabelLayer.SaveEditsFailed += new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
            }            
        }

        public FeatureLayer WipPolygonLayer
        {
            get { return _wipPolygonLayer; }
            set
            {
                _wipPolygonLayer = value;
                //NotifyPropertyChanged("WipPolygonLayer");
                _wipPolygonLayer.UpdateFailed += new EventHandler<TaskFailedEventArgs>(_wipPolygonLayer_UpdateFailed);
                _wipPolygonLayer.EndSaveEdits += new EventHandler<EndEditEventArgs>(LayerOnEndSaveEdits);
                _wipPolygonLayer.SaveEditsFailed += new EventHandler<TaskFailedEventArgs>(LayerOnSaveEditsFailed);
                //_wipPolygonLayer.UpdateCompleted += new EventHandler(LayerOnUpdateCompleted);
                _wipPolygonLayer.UpdateCompleted += new EventHandler(_wipPolygonLayer_UpdateCompleted);
            }
        }

        //public Border AttributeEditorContainer { get; set; }
        
        //public Button AttributeEditorCloseButton
        //{
        //    get
        //    {
        //        return attributeEditorClose;
        //    }
        //    set
        //    {
        //        attributeEditorClose = value;
        //        attributeEditorClose.Click +=new RoutedEventHandler(CloseAttributeEditor_OnClick);
        //    }
        //}
        
        public FeatureDataForm FeatureDataForm
        {
            get { return _featureDataForm; }
            set { _featureDataForm = value; _featureDataForm.EditEnded += new EventHandler<EventArgs>(AttributeEditor_EditEnded); }
        }

        public double Resolution { get; set; }
        public TextBlock StatusBar { get; set; }

        #endregion

        #region Dependancy Properties

        public Map MapProperty
        {
            get { return (Map)GetValue(MapPropertyProperty); }
            set { SetValue(MapPropertyProperty, value); EditorWIP.Map = value; WIPTemplatePicker.Map = value; }
        }

        // Using a DependencyProperty as the backing store for MapProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapPropertyProperty =
            DependencyProperty.Register("MapProperty", typeof(Map), typeof(WIPEditorWidget), null);

        public string GeometryServiceUrl
        {
            get { return (string)GetValue(GeometryServiceUrlProperty); }
            set { SetValue(GeometryServiceUrlProperty, value); EditorWIP.GeometryServiceUrl = value; WIPTemplatePicker.GeometryServiceUrl = value; }
        }

        // Using a DependencyProperty as the backing store for GeometryServiceUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GeometryServiceUrlProperty =
            DependencyProperty.Register("GeometryServiceUrl", typeof(string), typeof(WIPEditorWidget), null);

        public string[] LayerIDs
        {
            get { return (string[])GetValue(LayerIDsProperty); }
            set { SetValue(LayerIDsProperty, value); EditorWIP.LayerIDs = value; WIPTemplatePicker.LayerIDs = value; }
        }

        // Using a DependencyProperty as the backing store for LayerIDs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayerIDsProperty =
            DependencyProperty.Register("LayerIDs", typeof(string[]), typeof(WIPEditorWidget), null);

        #endregion

        #region Event Handlers

        #region WIP Layer Event Handlers        
        
        void _wipPolygonLayer_UpdateFailed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Layer Update Failed! " + e.Error);
            Layer layer = sender as Layer;
            StatusBar.Text = "'" + layer.ID + "' layer has failed to refresh. " + e.Error.Message; 
        }

        void _wipPolygonLayer_UpdateCompleted(object sender, EventArgs e)
        {
            LabelGraphics(sender as FeatureLayer, _wipLabelFieldNames);
            RefreshWIPOutlineLayer();  //INC000004019908, INC000004273558
        }

        void _wipPolygonLayer_SaveEditsFailed(object sender, TaskFailedEventArgs e)
        {
            RefreshWipLayer();
            RefreshWIPOutlineLayer();  //INC000004019908, INC000004273558
            logger.Error("Failed to save Edits! " + e.Error);
            Layer layer = sender as Layer;
            StatusBar.Text = "Save edits request to server has failed for layer '" + layer.ID + "'. " + e.Error.Message;
        }

        private void LayerOnSaveEditsFailed(object sender, TaskFailedEventArgs taskFailedEventArgs)
        {
            RefreshWipLayer();
            RefreshWIPOutlineLayer();  //INC000004019908, INC000004273558
            logger.Error("Failed to save WIP edits: " + taskFailedEventArgs.Error);
            Layer layer = sender as Layer;
            StatusBar.Text = "Save edits request to server has failed for layer '" + layer.ID + "'. " + taskFailedEventArgs.Error.Message;
            ((FeatureLayer)sender).Update();
            WipLabelEditPanel.IsEnabled = true;
        }

        private void LayerOnEndSaveEdits(object sender, EndEditEventArgs endEditEventArgs)
        {
            for (int i = 0; i < endEditEventArgs.Results.AddResults.Count; i++)
            {
                if (_addedWIPLabels.Count != endEditEventArgs.Results.AddResults.Count)
                {
                    logger.Error("There are a different amount of WIPs(" + endEditEventArgs.Results.AddResults.Count + ") and WIP Lables(" + _addedWIPLabels.Count + ").");
                    logEditError(endEditEventArgs.Results);
                    break;
                }
                if (_addedWIPLabels[i] != null)
                    _addedWIPLabels[i].Attributes[WipUID] = Int32.Parse(endEditEventArgs.Results.AddResults[i].ObjectID.ToString());
            }
            SaveEdits(_wipLabelLayerName);
            RefreshWIPOutlineLayer();  //INC000004019908, INC000004273558
            StatusBar.Text = "Edits saved successfully.";
            WipLabelEditPanel.IsEnabled = true;
        }
        private void logEditError(EditResults results)
        {
            string output = "Editing Error - Add: ";
            foreach (EditResultItem item in results.AddResults)
            {
                output += printEditResult(item) + ", ";
            }
            output += "Delete: ";
            foreach (EditResultItem item in results.DeleteResults)
            {
                output += printEditResult(item) + ", ";
            }
            output += "Update: ";
            foreach (EditResultItem item in results.UpdateResults)
            {
                output += printEditResult(item) + ", ";
            }
            logger.Error(output);
        }
        private string printEditResult(EditResultItem item)
        {
            string output = "Error Code:" + item.ErrorCode + ", Error Description:" + item.ErrorDescription + ", Global ID:" + item.GlobalID + ", Object ID:" + item.ObjectID + ", Success:" + item.Success;

            return output;
        }
        void WipLabellayer_EndSaveEdits(object sender, EndEditEventArgs e)
        {
            _addedWIPLabels.Clear();
            ((FeatureLayer)sender).Update();
            StatusBar.Text = "Edits saved successfully.";
            WipLabelEditPanel.IsEnabled = true;
        }

        void LayerOnUpdateCompleted(object sender, EventArgs eventArgs)
        {
            LabelGraphics(sender as FeatureLayer, _wipLabelFieldNames);
        }

        void WIPLabelUpdateCompleted(object sender, EventArgs e)
        {
            FeatureLayer wipLayer = MapProperty.Layers[_wipPolygonLayerName] as FeatureLayer;
            wipLayer.Update();
        }

        #endregion

        #region WIP Editor Event Handlers

        private void WIPEditor_EditCompleted(object sender, Editor.EditEventArgs e)
        {
            /*****************PLC Changes RK 07/21/2017*********************/
            string lanId = WebContext.Current.User.Name.Replace("PGE\\", "").ToLower().Replace("admin", "");           
            /*****************PLC changes End*********************/
            switch (e.Action)
            {
                case ESRI.ArcGIS.Client.Editor.EditAction.Add:
                    {
                        foreach (Editor.Change edit in e.Edits)
                        {
                            FeatureDataForm.FeatureLayer = (FeatureLayer)edit.Layer;
                            FeatureDataForm.GraphicSource = edit.Graphic;
                        }
                        //FeatureDataForm.GraphicSource.AttributeValueChanged += new EventHandler<ESRI.ArcGIS.Client.Graphics.DictionaryChangedEventArgs>(GraphicSource_AttributeValueChanged);   //INC000004102693, INC000004055302
                        //AttributeEditorCloseButton.Visibility = Visibility.Collapsed;
                        //AttributeEditorContainer.Visibility = Visibility.Visible;
                        wipAttributeEditorWindow.ResetWIPAttributes();
                        /*****************PLC Changes  RK 07/21/2017*********************/                      
                        DateTime currentDate = DateTime.Today;
                        FeatureDataForm.GraphicSource.Attributes["ESTIMATORLANID"] = lanId.ToUpper();
                        FeatureDataForm.GraphicSource.Attributes["DATECREATED"] = currentDate; 
                        populateJobNumber(FeatureDataForm.GraphicSource.Attributes["ESTIMATORLANID"].ToString());
                        /*****************PLC changes Ends*********************/
                        wipAttributeEditorWindow.Show();
                        break;
                    }
                case ESRI.ArcGIS.Client.Editor.EditAction.Select:
                    {
                        WipDelete.IsEnabled = !WipSaveEdits.IsEnabled;
                        if (_isAttributeEditor)
                        {
                           
                            foreach (Editor.Change edit in e.Edits)
                            {
                                //if (e.Edits.Count() == 1)
                                //    edit.Graphic.Select();
                                _isAttributeEditor = true;
                              
                                FeatureDataForm.FeatureLayer = (FeatureLayer)edit.Layer;
                                FeatureDataForm.GraphicSource = edit.Graphic;
                                //FeatureDataForm.GraphicSource.AttributeValueChanged += new EventHandler<ESRI.ArcGIS.Client.Graphics.DictionaryChangedEventArgs>(GraphicSource_AttributeValueChanged);   //INC000004102693, INC000004055302
                                //AttributeEditorContainer.Visibility = Visibility.Visible;
                                /********************PLC Changes  RK 07/21/2017*************************/
                                if (!string.IsNullOrEmpty(Convert.ToString(FeatureDataForm.GraphicSource.Attributes["ESTIMATORLANID"])))
                                {
                                    populateJobNumber(FeatureDataForm.GraphicSource.Attributes["ESTIMATORLANID"].ToString());
                                }
                                if (FeatureDataForm.GraphicSource.Attributes.ContainsKey("INSTALLJOBNUMBER"))
                                {
                                    wipAttributeEditorWindow.PmorderNumberAutoCompleteTextBlock.Text = FeatureDataForm.GraphicSource.Attributes["INSTALLJOBNUMBER"].ToString();

                                }
                                else
                                {
                                   
                                        getWipJobNumber(FeatureDataForm.GraphicSource.Attributes["OBJECTID"].ToString());

                                        wipJobQueryCount++;
                                }
                                /********************PLC Changes Ends*************************/
                                wipAttributeEditorWindow.Show();
                                //_isAttributeEditor = false;
                                

                            }
                            EditorWIP.SelectionMode = DrawMode.Rectangle;
                            EditorWIP.CancelActive.Execute(null);
                        }
                        else
                        {
                            //AttributeEditorContainer.Visibility = Visibility.Collapsed;
                        }
                        break;
                    }
                case ESRI.ArcGIS.Client.Editor.EditAction.ClearSelection:
                    {
                        //AttributeEditorContainer.Visibility = Visibility.Collapsed;
                        break;
                    }
                case ESRI.ArcGIS.Client.Editor.EditAction.DeleteSelected:
                    {
                        FeatureLayer WIPLabelLayer = MapProperty.Layers[_wipLabelLayerName] as FeatureLayer;
                        if (WIPLabelLayer == null) return;

                        //check for delete access for all the selected wips  ----  CAP #114707237 ---ME Q3 2019 Release
                        int validEditsCount = 0;
                        int totalEditsCount = 0;
                        foreach (Editor.Change edit in e.Edits)
                        {
                            totalEditsCount++;
                            if (edit.Graphic.Attributes["ESTIMATORLANID"].ToString().ToUpper() == WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper().Replace("admin", "") || _isMemOfDeleteADGrps)
                            {
                                validEditsCount++;
                            }
                        }

                        if (validEditsCount == 0)
                        {
                            if (EditorWIP.UndoEdits.CanExecute(null)) { EditorWIP.UndoEdits.Execute(null); }
                            System.Windows.MessageBox.Show("You do not have permission to delete this WIP polygon." + "\n" + "Please contact the user who created the WIP or Electric Mapping for deletion.");
                            if (EditorWIP.ClearSelection.CanExecute(null))
                                EditorWIP.ClearSelection.Execute(null);
                        }
                        else if (validEditsCount > 0 && validEditsCount < totalEditsCount)
                        {
                            if (EditorWIP.UndoEdits.CanExecute(null)) { EditorWIP.UndoEdits.Execute(null); }
                            System.Windows.MessageBox.Show("You do not have permission to delete one/some of the selected WIP polygons." + "\n" + "Please select one WIP at a time and contact the user who created the WIP or" + "\n" + "Electric Mapping for deletion.");
                            if (EditorWIP.ClearSelection.CanExecute(null))
                                EditorWIP.ClearSelection.Execute(null);
                        }

                        if (validEditsCount == totalEditsCount)
                        {
                            foreach (Editor.Change edit in e.Edits)
                            {
                                string parentID = string.Empty;
                                if (edit.Graphic.Attributes["OBJECTID"] != null)
                                    parentID = edit.Graphic.Attributes["OBJECTID"].ToString();
                                RemoveLabel(WIPLabelLayer, parentID);
                            }
                            WipSaveEdits.IsEnabled = true;
                        }
                        //AttributeEditorContainer.Visibility = Visibility.Collapsed;
                        break;
                    }
                case ESRI.ArcGIS.Client.Editor.EditAction.EditVertices:
                    {
                        WipSaveEdits.IsEnabled = true;
                        break;
                    }
                case ESRI.ArcGIS.Client.Editor.EditAction.Reshape:
                    {
                        WipSaveEdits.IsEnabled = true;
                        break;
                    }
                case ESRI.ArcGIS.Client.Editor.EditAction.Save:
                    {
                        SaveEdits(_wipPolygonLayerName);

                        WipLabelEditPanel.IsEnabled = false;
                        //WipSaveEdits.IsEnabled = false;
                        break;
                    }
            }
        }


        //INC000004102693, INC000004055302
        //void GraphicSource_AttributeValueChanged(object sender, ESRI.ArcGIS.Client.Graphics.DictionaryChangedEventArgs e)
        //{
        //    string field = e.Key;
        //    if (field.Equals("LOCATIONNUMBER"))
        //    {
        //        int newvalue = Convert.ToInt32(e.NewValue);
        //        if (newvalue > 999)
        //        {
        //            MessageBox.Show("Location Number must be less than 1000. Value will not be saved.");
        //            ((Graphic)sender).Attributes["LOCATIONNUMBER"] = e.OldValue;
        //        }
        //    }
        //}

        //*************************PLC Changes  RK 07/21/2017*********************//
        //Populate Job Number in Job Number Dropdown
        private void populateJobNumber(string lanid)
        {
            getLayerUrl("PmOrder");

            Query _jobNumerQuery = new Query();
            _jobNumerQuery.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            _jobNumerQuery.ReturnGeometry = false;

            _jobNumerQuery.OutFields.AddRange(new string[] { "INSTALLJOBNUMBER" });
            _jobNumerQuery.Where = "PRIMARYESTIMATOR LIKE '%" + lanid.ToUpper() + "%' OR SECONDARYESTIMATOR LIKE '%" + lanid.ToUpper() + "%' OR PRIMARYESTIMATOR LIKE '%" + lanid.ToLower() + "%' OR SECONDARYESTIMATOR LIKE '%" + lanid.ToLower() + "%'";

            QueryTask _jobNumberqueryTask = new QueryTask(jobNumberLayerUrl);
            _jobNumberqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_populateJobNoqueryTask_ExecuteCompleted);
            _jobNumberqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_populateJobNumberqueryTask_Failed);
            _jobNumberqueryTask.ExecuteAsync(_jobNumerQuery);
        }

        void _populateJobNoqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {


                    List<string> featureset = new List<string>();
                    for (int i = 0; i < e.FeatureSet.Features.Count; i++)
                    {
                        if (e.FeatureSet.Features[i].Attributes["INSTALLJOBNUMBER"] != null)
                        {
                            featureset.Add(e.FeatureSet.Features[i].Attributes["INSTALLJOBNUMBER"].ToString());
                        }
                    }
                    wipAttributeEditorWindow.PmorderNumberAutoCompleteTextBlock.ItemsSource = featureset;

                }

            }
            catch (Exception ex)
            {
                logger.Error("WIP populate job number exception: " + ex.Message);
            }
        }

        void _populateJobNumberqueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            
            logger.Error("WIP Job Number query Failed: " + e.Error.Message);
        }

        private void getWipJobNumber(string objId)
        {
            getLayerUrl("WIP");
           
            Query _jobNumerQuery = new Query();
            _jobNumerQuery.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            _jobNumerQuery.ReturnGeometry = false;

            _jobNumerQuery.OutFields.AddRange(new string[] { "INSTALLJOBNUMBER" });
            _jobNumerQuery.Where = "OBJECTID=" + objId;

            QueryTask _jobNumberqueryTask = new QueryTask(jobNumberLayerUrl);
            _jobNumberqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_getWipJobNoqueryTask_ExecuteCompleted);
            _jobNumberqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_populateJobNumberqueryTask_Failed);
            _jobNumberqueryTask.ExecuteAsync(_jobNumerQuery);
        }

        void _getWipJobNoqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            wipJobQueryCount--;
            if (e.FeatureSet.Features.Count > 0)
            {

                wipAttributeEditorWindow.PmorderNumberAutoCompleteTextBlock.Text = Convert.ToString(e.FeatureSet.Features[0].Attributes["INSTALLJOBNUMBER"]);
            }
            if (wipJobQueryCount == 0)
            {
                _isAttributeEditor = false;
            }
            
            
        }
        //Function to get layer url from config file
        private void getLayerUrl(string layerName)
        {
            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
                config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
                {
                    if (layerName == "PmOrder")
                    {
                        if (element.Name.LocalName == "WIPJobNumberLayer")
                        {
                            if (element.HasElements)
                            {
                                var i = 0;

                                foreach (XElement childelement in element.Elements())
                                {
                                    jobNumberLayerUrl = childelement.LastAttribute.Value;
                                    break;
                                }
                            }
                        }
                    }
                    else if (layerName == "WIP")
                    {
                        if (element.Name.LocalName == "Layers")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement layerElement in element.Elements())
                                {

                                    if (layerElement.Name == "Layer")
                                    {
                                        foreach (XElement childelement in element.Elements())
                                        {
                                            if (childelement.FirstAttribute.Value == "WIP Cloud")
                                            {
                                                jobNumberLayerUrl = childelement.FirstAttribute.NextAttribute.Value;

                                            }

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //*************************PLC Changes End*********************//
        private void AttributeEditor_EditEnded(object sender, EventArgs e)
        {
            //get the labeling point
            //*************************PLC Changes  RK 07/21/2017*********************//
            getLayerUrl("PmOrder");
            //*************************PLC Changes End*********************//
            AddWipfeatureDataForm = sender as FeatureDataForm;
            if (AddWipfeatureDataForm != null)
            {
                Addwipgraphic = AddWipfeatureDataForm.GraphicSource;
                string jobNumber = Addwipgraphic.Attributes["INSTALLJOBNUMBER"].ToString();
                Query _query = new Query();
                _query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
                _query.ReturnGeometry = false;
                //_query.Geometry = GetPolygonFromEnvelope(_map.Extent);
                _query.OutFields.AddRange(new string[] { "INSTALLJOBNUMBER", "MAT" });
                _query.Where = "INSTALLJOBNUMBER=" + jobNumber;
                QueryTask _circuitqueryTask = new QueryTask(jobNumberLayerUrl);
                _circuitqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_wipJobNumberqueryTask_ExecuteCompleted);
                _circuitqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_wipJobNumberqueryTask_Failed);
                _circuitqueryTask.ExecuteAsync(_query);
            }
            //if (graphic != null)
            //{
            //    PutCurrentDateIfNull(graphic);
            //    if (!graphic.Selected)
            //    {
            //        GetLabelingPoint(graphic);
            //    }
            //    else
            //    {
            //        LabelGraphics(featureDataForm.FeatureLayer, _wipLabelFieldNames);
            //        if (EditorWIP.Save.CanExecute(null))
            //            EditorWIP.Save.Execute(null);
            //        if (EditorWIP.ClearSelection.CanExecute(null))
            //            EditorWIP.ClearSelection.Execute(null);
            //    }
            //    //WipSaveEdits.IsEnabled = true;
                
            //}
            //AttributeEditorCloseButton.Visibility = Visibility.Visible;
            //AttributeEditorContainer.Visibility = Visibility.Collapsed;
        }

        ////////////////////////17/11/2016 starts //////////////////////////
        void _wipJobNumberqueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();
            //*************************PLC Changes  RK 07/21/2017*********************//
            System.Windows.MessageBox.Show("Please Select/Enter correct job number.");
            wipAttributeEditorWindow.Show();
            //*************************PLC Changes End*********************//
        }

        void _wipJobNumberqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {

            if (e.FeatureSet.Features.Count > 0)
            {
                /*************PLC changes for WIP*************/
                if(e.FeatureSet.Features[0].Attributes["MAT"]!=null)
                {
                    if (e.FeatureSet.Features[0].Attributes["MAT"].ToString() == "16Y")
                    {
                        if (Addwipgraphic.Attributes["NOTIFICATION_NUMBER"] != null)
                        {
                            if (Addwipgraphic.Attributes["NOTIFICATION_NUMBER"].ToString() != null && Addwipgraphic.Attributes["NOTIFICATION_NUMBER"].ToString() != "")
                            {
                                bool validNotNo = validateNotificationNo(Addwipgraphic.Attributes["NOTIFICATION_NUMBER"]);
                                if (validNotNo)
                                {
                                    if (Addwipgraphic != null)
                                    {
                                        PutCurrentDateIfNull(Addwipgraphic);
                                        if (!Addwipgraphic.Selected)
                                        {
                                            GetLabelingPoint(Addwipgraphic);
                                        }
                                        else
                                        {
                                            LabelGraphics(AddWipfeatureDataForm.FeatureLayer, _wipLabelFieldNames);
                                            if (EditorWIP.Save.CanExecute(null))
                                                EditorWIP.Save.Execute(null);
                                            if (EditorWIP.ClearSelection.CanExecute(null))
                                                EditorWIP.ClearSelection.Execute(null);
                                        }
                                        //WipSaveEdits.IsEnabled = true;

                                    }
                                }
                                else
                                {
                                    System.Windows.MessageBox.Show(" Notification Number should contain number only");
                                    wipAttributeEditorWindow.Show();
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("Please Enter Notification Number");
                                wipAttributeEditorWindow.Show();
                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show("Please Enter Notification Number");
                            wipAttributeEditorWindow.Show();
                        }
                    }
                    else
                    {
                        bool validNotiNo = validateNotificationNo(Addwipgraphic.Attributes["NOTIFICATION_NUMBER"]);
                        if (validNotiNo)
                        {
                            if (Addwipgraphic != null)
                            {
                                PutCurrentDateIfNull(Addwipgraphic);
                                if (!Addwipgraphic.Selected)
                                {
                                    GetLabelingPoint(Addwipgraphic);
                                }
                                else
                                {
                                    LabelGraphics(AddWipfeatureDataForm.FeatureLayer, _wipLabelFieldNames);
                                    if (EditorWIP.Save.CanExecute(null))
                                        EditorWIP.Save.Execute(null);
                                    if (EditorWIP.ClearSelection.CanExecute(null))
                                        EditorWIP.ClearSelection.Execute(null);
                                }
                                //WipSaveEdits.IsEnabled = true;

                            }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(" Notification Number should contain number only");
                            wipAttributeEditorWindow.Show();
                        }
                    }
                
                }
                else
                {
                     bool validNotiNo = validateNotificationNo(Addwipgraphic.Attributes["NOTIFICATION_NUMBER"]);
                        if (validNotiNo)
                        {
                    if (Addwipgraphic != null)
                    {
                        PutCurrentDateIfNull(Addwipgraphic);
                        if (!Addwipgraphic.Selected)
                        {
                            GetLabelingPoint(Addwipgraphic);
                        }
                        else
                        {
                            LabelGraphics(AddWipfeatureDataForm.FeatureLayer, _wipLabelFieldNames);
                            if (EditorWIP.Save.CanExecute(null))
                                EditorWIP.Save.Execute(null);
                            if (EditorWIP.ClearSelection.CanExecute(null))
                                EditorWIP.ClearSelection.Execute(null);
                        }
                        //WipSaveEdits.IsEnabled = true;

                    }
                        }
                        else
                        {
                            System.Windows.MessageBox.Show(" Notification Number should contain number only");
                            wipAttributeEditorWindow.Show();
                        }
                }
                /*************PLC changes for WIP*************/
            }
            else
            {
                /*************PLC changes RK 21/07/2017 for WIP*************/
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Install Job Number not found. Do you want to continue?", "Confirmation", System.Windows.MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    if (Addwipgraphic != null)
                    {
                        PutCurrentDateIfNull(Addwipgraphic);
                        if (!Addwipgraphic.Selected)
                        {
                            GetLabelingPoint(Addwipgraphic);
                        }
                        else
                        {
                            LabelGraphics(AddWipfeatureDataForm.FeatureLayer, _wipLabelFieldNames);
                            if (EditorWIP.Save.CanExecute(null))
                                EditorWIP.Save.Execute(null);
                            if (EditorWIP.ClearSelection.CanExecute(null))
                                EditorWIP.ClearSelection.Execute(null);
                        }
                        //WipSaveEdits.IsEnabled = true;

                    }

                }
                else
                {
                    wipAttributeEditorWindow.Show();
                }
                ///*************PLC changes for WIP*************/
                //System.Windows.MessageBox.Show("Install Job Number not found. Please Select/Enter correct job number.");
                //wipAttributeEditorWindow.Show();
                ///*************PLC changes for WIP Ends*************/

            }
        }
        ////////////////////////17/11/2016 ends //////////////////////////
        /*************PLC changes for WIP*************/
        private bool validateNotificationNo(object notificationNo)
        {
            try
            {
                if (notificationNo != null)
                {
                    if (notificationNo.ToString() != "")
                    {
                        ulong notNo = Convert.ToUInt64(notificationNo);
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /*************PLC changes for WIP Ends*************/

        void wipAttributeEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WIPAttributeEditor childWindow = sender as WIPAttributeEditor;
            
            if (childWindow.DialogResult == true) return;

            if (EditorWIP.ClearSelection.CanExecute(null))
                EditorWIP.ClearSelection.Execute(null);

            if (childWindow.WIPAttributeEditorForm.FeatureLayer != null)
                childWindow.WIPAttributeEditorForm.FeatureLayer.Update();
        }

        private void DisplayAttribute_Click(object sender, RoutedEventArgs e)
        {
            if (EditorWIP.ClearSelection.CanExecute(null))
                EditorWIP.ClearSelection.Execute(null);
            _isAttributeEditor = true;
            EditorWIP.SelectionMode = DrawMode.Point;
            EditorWIP.Select.Execute("New");
            StatusBar.Text = string.Empty;
        }

        void geoService_LabelPointsCompleted(object sender, GraphicsEventArgs e)
        {
            FeatureLayer WIPLabelLayer = MapProperty.Layers[_wipLabelLayerName] as FeatureLayer;
            if (WIPLabelLayer == null) return;

            foreach (Graphic graphic in e.Results)
            {
                WIPLabelLayer.Graphics.Add(graphic);

                graphic.Symbol = GenerateCustomTextSymbol(GetLabelText(_newFeatureAttributes));

                _addedWIPLabels.Add(graphic);
            }

            if (EditorWIP.Save.CanExecute(null))
                EditorWIP.Save.Execute(null);
        }

        void geoService_Failed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Label creation failed: " + e.Error);
            StatusBar.Text = "Geometry Service Failed. " + e.Error.Message;
        }

        private void CloseAttributeEditor_OnClick(object sender, RoutedEventArgs e)
        {
            if (EditorWIP.ClearSelection.CanExecute(null))
                EditorWIP.ClearSelection.Execute(null);
            //AttributeEditorContainer.Visibility = Visibility.Collapsed;
        }

        private void WIPEditorTemplate_OnEditorActivated(object sender, Editor.CommandEventArgs e)
        {
            if (!(e.Action == Editor.EditAction.Cancel))
                IsActive = true;
            CursorSet.SetID(MapProperty, "Arrow");
            StatusBar.Text = string.Empty;
        }

        private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
        {
            if (_isEditingLabels) return;
            if (WipSaveEdits.IsEnabled)
            {
                MessageBox.Show("Please save your edits before attempting to move labels.", "Save Edits", MessageBoxButton.OK);
                ((ToggleButton)sender).IsChecked = false;
                return;
            }
            SwitchLayerInEditor(_wipLabelLayerName);
            ToggleWipToolsVisibility(true);
            _isEditingLabels = true;
            StatusBar.Text = string.Empty;
        }

        private void ToggleButton_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_isEditingLabels) return;
            if (WipSaveEdits.IsEnabled)
            {
                MessageBox.Show("Please save your edits before editing the WIP.", "Save Edits", MessageBoxButton.OK);
                ((ToggleButton)sender).IsChecked = true;
                return;
            }
            SwitchLayerInEditor(_wipPolygonLayerName);
            ToggleWipToolsVisibility(false);
            EditorWIP.EditVertices.Execute(null);
            EditorWIP.CancelActive.Execute(null);
            _isEditingLabels = false;
            StatusBar.Text = string.Empty;
        }

        #endregion

        #endregion

        #region WIP Editor Utility Methods

        private WipCustomTextSymbol CreateMinerLabelSymbol(string labelText)
        {
            var labelSymbol = new WipCustomTextSymbol();

            var factor = Resolution / MapProperty.Resolution;
            labelSymbol.ScaleFactor = factor == 0 ? 1 : factor;

            labelSymbol.CompressedXaml = labelText;

            labelSymbol.BorderBrush = new SolidColorBrush(Colors.Transparent);
            labelSymbol.Background = new SolidColorBrush(Colors.Transparent);

            labelSymbol.GenerateTextImage();

            return labelSymbol;
        }

        private TextSymbol CreateLabelSymbol(string labelText)
        {
            var labelSymbol = new TextSymbol()
            {
                FontFamily = new FontFamily("Arial"),
                Foreground = new SolidColorBrush(Colors.Black),
                FontSize = 8,
                Text = labelText
            };

            return labelSymbol;
        }

        private void SaveEdits(string layerName)
        {
            var layer = MapProperty.Layers[layerName] as FeatureLayer;
            if (layer != null)
            {
                //if (layer.HasEdits)
                if (layer.ValidateEdits) layer.SaveEdits();
            }
        }

        private void RemoveLabel(FeatureLayer layer, string parentID)
        {
            if (string.IsNullOrEmpty(parentID)) return;
            var labelsToRemove = new List<Graphic>();
            foreach (var graphic in layer.Graphics)
            {
                if (graphic.Attributes[WipUID] == null) continue;

                if (graphic.Attributes[WipUID].ToString() == parentID)
                {
                    labelsToRemove.Add(graphic);
                }
            }

            if (labelsToRemove.Count <= 0) return;

            foreach (var graphic in labelsToRemove)
            {
                layer.Graphics.Remove(graphic);
            }
            layer.Refresh();
        }

        private void GetLabelingPoint(Graphic polygon)
        {
            if (!string.IsNullOrEmpty(EditorWIP.GeometryServiceUrl))
            {
                GeometryService geoService = new GeometryService(EditorWIP.GeometryServiceUrl);
                geoService.LabelPointsCompleted += geoService_LabelPointsCompleted;
                geoService.Failed += geoService_Failed;

                StoreAttributesForNewLabel(polygon);

                IList<Graphic> polygonsToLabel = new List<Graphic>();
                polygonsToLabel.Add(polygon);

                //execute geometry service
                geoService.LabelPointsAsync(polygonsToLabel);
            }
        }

        private void StoreAttributesForNewLabel(Graphic feature)
        {
            _newFeatureAttributes.Clear();

            foreach (string field in _wipLabelFieldNames)
            {
                if (feature.Attributes.ContainsKey(field))
                {
                    _newFeatureAttributes.Add(field, feature.Attributes[field]);
                }
            }
        }

        private string GetLabelText(Dictionary<string, object> attributes)
        {
            string labelText = string.Empty;
            if (attributes == null) return labelText;
            //foreach (string field in attributes.Keys)
            foreach (string field in _wipLabelFieldNames)
            {
                /********************PLC CHANGE 27 Dec Starts****************/
                if (field == "INSTALLJOBNUMBER")
                {
                    if (attributes.ContainsKey("OBJECTID") && !attributes.ContainsKey("INSTALLJOBNUMBER"))
                    {
                        if (_wipLabelJobNumber.ContainsKey(attributes["OBJECTID"].ToString()))
                        {
                            if (!string.IsNullOrEmpty(labelText))
                                labelText += Environment.NewLine;
                            labelText += _wipLabelJobNumber[attributes["OBJECTID"].ToString()];
                        }
                    }
                    
                }
                /********************PLC CHANGE 27 Dec ends****************/
                if (!attributes.ContainsKey(field)) continue;
                if (attributes[field] == null) continue;
                if (attributes[field] is string)
                {
                    if (attributes[field] != "")
                    {
                        if (!string.IsNullOrEmpty(labelText))
                            labelText += Environment.NewLine;
                        labelText += attributes[field];
                    }
                }
                else if (attributes[field] is DateTime)
                {
                    DateTime date = (DateTime)attributes[field];
                    if (!string.IsNullOrEmpty(labelText))
                        labelText += Environment.NewLine;
                    labelText += date.ToShortDateString();
                }
                else if (attributes[field] is int || attributes[field] is double || attributes[field] is short || attributes[field] is long || attributes[field] is Single)
                {
                    if (!string.IsNullOrEmpty(labelText))
                        labelText += Environment.NewLine;
                    labelText += attributes[field].ToString();
                }
            }
            return labelText;
        }
        /********************PLC CHANGE 27 Dec Starts****************/
        void getInstallJobNumber(List<string> objIdList)
        {
            if (objIdList.Count > 0)
            {
                string commaSaperatedObjidList = string.Join(",", objIdList);
                getLayerUrl("WIP");

                Query _jobNumerQuery = new Query();
                _jobNumerQuery.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
                _jobNumerQuery.ReturnGeometry = false;

                _jobNumerQuery.OutFields.AddRange(new string[] { "INSTALLJOBNUMBER", "OBJECTID" });
                _jobNumerQuery.Where = "OBJECTID in(" + commaSaperatedObjidList + ")";

                QueryTask _jobNumberqueryTask = new QueryTask(jobNumberLayerUrl);
                _jobNumberqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_getWipJobNoLabelqueryTask_ExecuteCompleted);
                _jobNumberqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_populateJobNumberqueryTask_Failed);
                _jobNumberqueryTask.ExecuteAsync(_jobNumerQuery);
            }
            
        }

        void _getWipJobNoLabelqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if (e.FeatureSet.Features.Count > 0)
            {
                _wipLabelJobNumber = new Dictionary<string, string>();
                for (int i = 0; i < e.FeatureSet.Features.Count; i++)
                {
                    if (e.FeatureSet.Features[i].Attributes["INSTALLJOBNUMBER"] != null)
                    {
                        _wipLabelJobNumber.Add(e.FeatureSet.Features[i].Attributes["OBJECTID"].ToString(), e.FeatureSet.Features[i].Attributes["INSTALLJOBNUMBER"].ToString());
                    }
                }
                Graphic labelGraphic;
                
                FeatureLayer layer = _getWIPGraphicsLayer;
                if (layer != null)
                {
                    foreach (Graphic graphic in layer.Graphics)
                    {
                        if (graphic.Attributes[layer.LayerInfo.ObjectIdField] == null) continue;
                        labelGraphic = GetRelatedLabel(graphic.Attributes[layer.LayerInfo.ObjectIdField], graphic);
                        if (labelGraphic == null) continue;

                        labelGraphic.Symbol = GenerateCustomTextSymbol(GetLabelText(new Dictionary<string, object>(graphic.Attributes)));
                    }

                    layer.Refresh();
                }

            }
        }
        /********************PLC CHANGE 27 Dec ends****************/
        private void LabelGraphics(FeatureLayer layer, List<string> labelingFields)
        {
            /********************PLC CHANGE 27 Dec Starts****************/
            List<string> ObjectIdList=new List<string>();
            _getWIPGraphicsLayer = layer;
            foreach (Graphic graphic in layer.Graphics)
            {
                ObjectIdList.Add(graphic.Attributes[layer.LayerInfo.ObjectIdField].ToString());
            }
            getInstallJobNumber(ObjectIdList);

           
            Graphic labelGraphic;
            
            
            //foreach (Graphic graphic in layer.Graphics)
            //{
            //    if (graphic.Attributes[layer.LayerInfo.ObjectIdField] == null) continue;
            //    labelGraphic = GetRelatedLabel(graphic.Attributes[layer.LayerInfo.ObjectIdField], graphic);
            //    if (labelGraphic == null) continue;

            //    labelGraphic.Symbol = GenerateCustomTextSymbol(GetLabelText(new Dictionary<string, object>(graphic.Attributes)));
            //}

            //layer.Refresh();
            /********************PLC CHANGE 27 Dec ends****************/
        }

        private WipCustomTextSymbol GenerateCustomTextSymbol(string labelText)
        {
            var symbol = CreateMinerLabelSymbol(labelText);

            ResizeSymbol(symbol);

            return symbol;
        }

        public void ResizeSymbol(WipCustomTextSymbol symbol)
        {
            if (symbol != null)
            {
                var factor = Resolution / MapProperty.Resolution;

                symbol.Width = (factor == 0.0 ? 1.0 : factor) * (symbol.Source as WriteableBitmap).PixelWidth;
                symbol.Height = (factor == 0.0 ? 1.0 : factor) * (symbol.Source as WriteableBitmap).PixelHeight;

                symbol.ScaleFactor = factor == 0 ? 1 : factor;
            }
        }

        private Graphic GetRelatedLabel(object OID, Graphic graphic)
        {
            Graphic labelGraphic = null;
            FeatureLayer labelLayer = MapProperty.Layers[_wipLabelLayerName] as FeatureLayer;
            foreach (Graphic label in labelLayer.Graphics)
            {
                if (label.Attributes[WipUID] == null) continue;
                if (label.Attributes[WipUID].ToString() == OID.ToString())
                {
                    labelGraphic = label;
                    break;
                }
            }
            return labelGraphic;
        }

        private void SwitchLayerInEditor(string layerID)
        {
            List<string> layers = new List<string>();
            layers.Add(layerID);

            EditorWIP.LayerIDs = layers.ToArray();
        }

        private void ToggleWipToolsVisibility(bool isEditingLabel)
        {
            Visibility visible = (isEditingLabel) ? Visibility.Collapsed : Visibility.Visible;

            WIPTemplatePicker.IsEnabled = !isEditingLabel;
            WipSelect.Visibility = visible;
            WipClearSelect.Visibility = visible;
            WipDelete.Visibility = visible;
            //WipReshape.Visibility = visible;
            DisplayAttribute.Visibility = visible;
        }

        private void PutCurrentDateIfNull(Graphic graphic)
        {
            if (_wipPolygonLayer == null) return;
            foreach (Field field in _wipPolygonLayer.LayerInfo.Fields)
            {
                if (field.Type == Field.FieldType.Date && _wipPolygonLayer.OutFields.Contains(field.Name))
                {
                    if (graphic.Attributes[field.Name] == null) graphic.Attributes[field.Name] = DateTime.Today;
                }
            }
        }
        
        private void RefreshWipLayer()
        {
            FeatureLayer wipLayer = MapProperty.Layers[_wipLabelLayerName] as FeatureLayer;
            wipLayer.Update();            
        }

        //INC000004019908, INC000004273558
        private void RefreshWIPOutlineLayer()
        {
            FeatureLayer wipOutlineLayer = MapProperty.Layers[_wipOutlineLayerName] as FeatureLayer;
            wipOutlineLayer.Update();
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
                    if (this.EditorWIP.CancelActive.CanExecute(null)) this.EditorWIP.CancelActive.Execute(null);
                    //StatusBar.Text = string.Empty;
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
            IsActive = false;
        }

        #endregion
    }
}
