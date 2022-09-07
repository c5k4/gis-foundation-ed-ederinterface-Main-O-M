using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;

using EsriEditor = ESRI.ArcGIS.Client.Editor;
#if WPF
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;
#endif

#if SILVERLIGHT
using Miner.Server.Client.Controls;
using Miner.Server.Client.Symbols;
#elif WPF
using Miner.Mobile.Client.Controls;
using Miner.Mobile.Client.Symbols;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    public class Editor : DependencyObject
    {
        #region Member Variables

        private readonly EsriEditor _editor = new EsriEditor();
        private readonly EsriEditor _textEditor = new EsriEditor();
        private readonly ClipBoardItem _clipBoard = new ClipBoardItem();

        private Graphic _textSymbol;
        private Graphic _simplifiedGraphic;
        private MapPoint _lastDragPoint;
        private ShapeDraw _activeDraw;
        private bool _isGraphicsMarked;

        internal RedlineTextController _redlineText = new RedlineTextController();
        internal Edits _edits = new Edits();
        internal bool _hasExtraEdits;

        private const string TextEditorTitle = "Text Editor";
        private const string VisiblePropertyName = "visible";
        private const string TempLayerID = "TextTemporary";
        private const string TextValue = "TEXTVALUE";
        private const string TextGraphicKey = "TextGraphic";

        #endregion Member Variables

        #region Constructor

        public Editor()
        {
            SaveToFile = new DelegateCommand<FileOption>(Save, HasEdits);
            OpenFile = new DelegateCommand<FileOption>(Open);
            CopySelected = new DelegateCommand<object>(Copy, HasSelection);
            Paste = new DelegateCommand<object>(OnPaste, ClipBoardHasItems);
            AddText = new DelegateCommand<object>(OnAddText, CanAddText);
            AddShape = new DelegateCommand<object>(OnAddShape, CanAddPolygon);

            Add = new DelegateCommand<object>(OnAdd, _editor.Add.CanExecute);
            Select = new DelegateCommand<object>(OnSelect, _editor.Select.CanExecute);
            EditGeometry = new DelegateCommand<object>(OnEdit, _editor.EditVertices.CanExecute);
            DeleteSelected = _editor.DeleteSelected;
            SaveToDatabase = new DelegateCommand<object>(PerformSaveToDatabase, CanSaveToDatabase);

            _edits.CollectionChanged += new NotifyCollectionChangedEventHandler(Edits_CollectionChanged);
            _editor.EditCompleted += new EventHandler<EsriEditor.EditEventArgs>(Editor_EditCompleted);
            _textEditor.EditCompleted += new EventHandler<EsriEditor.EditEventArgs>(TextEditor_EditCompleted);

            _editor.Save.CanExecuteChanged += SaveToDatabaseExecuteChanged;
            _editor.Select.CanExecuteChanged += (s, e) => (Select as DelegateCommand<object>).RaiseCanExecuteChanged();
            _editor.EditVertices.CanExecuteChanged += (s, e) => (EditGeometry as DelegateCommand<object>).RaiseCanExecuteChanged();
        }

        #endregion Constructor

        #region Events

        public event EventHandler<EsriEditor.EditEventArgs> EditCompleted;
        public event EventHandler EditTurnedOn;
        public event EventHandler EditTurnedOff;
        public event EventHandler EditPublishing;
        public event EventHandler EditPublished;
        public event EventHandler EditSaving;
        public event EventHandler<ModifyEditEventArgs> EditCopying;
        public event EventHandler<ModifyEditEventArgs> EditPasting;
        public event EventHandler EditsQuerying;

        #endregion Events

        #region Public Properties

        //public ICommand ClearEdits { get; private set; }

        public ICommand SaveToFile { get; private set; }
        public ICommand SaveToDatabase { get; private set; }
        public ICommand OpenFile { get; private set; }
        public ICommand CopySelected { get; private set; }
        public ICommand Paste { get; private set; }
        public ICommand AddText { get; set; }

        public ICommand Add { get; set; }
        public ICommand Select { get; private set; }
        public ICommand DeleteSelected { get; private set; }
        public ICommand EditGeometry { get; private set; }

        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set
            {
                SetValue(MapProperty, value);
                Map.KeyUp += Map_KeyUp;
                Map.KeyDown += Map_KeyDown;
            }
        }

        /// <summary>
        /// Gets or sets the layer IDs this editor works against.
        /// </summary>
        [TypeConverter(typeof(StringToStringArrayConverter))]
        public string[] LayerIDs
        {
            get { return (string[])GetValue(LayerIDsProperty); }
            set { SetValue(LayerIDsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether continuous mode is enabled.
        /// </summary>
        public bool ContinuousMode
        {
            get { return (bool)GetValue(ContinuousModeProperty); }
            set { SetValue(ContinuousModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layer ID for text this editor works against.
        /// </summary>
        public string TextLayerID
        {
            get { return (string)GetValue(TextLayerIDProperty); }
            set { SetValue(TextLayerIDProperty, value); }
        }

        /// <summary>
        /// Gets or sets the active symbol for editing.
        /// </summary>
        public Symbol ActiveSymbol
        {
            get { return (Symbol)GetValue(ActiveSymbolProperty); }
            set { SetValue(ActiveSymbolProperty, value); }
        }

        public ControlTemplate TextTemplate
        {
            get { return (ControlTemplate)GetValue(TextTemplateProperty); }
            set { SetValue(TextTemplateProperty, value); }
        }

        public string GeometryService { get; set; }

        #endregion Public Properties

        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="TextTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextTemplateProperty = DependencyProperty.Register(
            "TextTemplate",
            typeof(ControlTemplate),
            typeof(Editor),
            new PropertyMetadata(OnTextTemplateChanged));

        /// <summary>
        /// Identifies the <see cref="ActiveSymbol"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActiveSymbolProperty = DependencyProperty.Register(
            "ActiveSymbol",
            typeof(Symbol),
            typeof(Editor),
            new PropertyMetadata(OnActiveSymbolChanged));

        /// <summary>
        /// Identifies the <see cref="TextLayerID"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextLayerIDProperty = DependencyProperty.Register(
            "TextLayerID",
            typeof(string),
            typeof(Editor),
            new PropertyMetadata(OnTextLayerIDChanged));

        /// <summary>
        /// Identifies the <see cref="Map"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(Editor),
            new PropertyMetadata(OnMapChanged));

        /// <summary>
        /// Identifies the <see cref="LayerIDs"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LayerIDsProperty = DependencyProperty.Register(
            "LayerIDs",
            typeof(string[]),
            typeof(Editor),
            new PropertyMetadata(OnLayerIdsChanged));

        /// <summary>
        /// Identifies the <see cref="ContinuousMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContinuousModeProperty = DependencyProperty.Register(
            "ContinuousMode",
            typeof(bool),
            typeof(Editor),
            new PropertyMetadata(OnContinuousModeChanged));

        /// <summary>
        /// Text to display to the user to confirm saving edits to the database. If this is empty or null, a confirmation dialog will
        /// not be displayed and the edits will be saved.
        /// </summary>
        public string SaveToDatabaseConfirmationText
        {
            get { return (string)GetValue(SaveToDatabaseConfirmationTextProperty); }
            set { SetValue(SaveToDatabaseConfirmationTextProperty, value); }
        }

        /// <summary>
        /// Dependency property for the <see cref="SaveToDatabaseConfirmationText"/> string.
        /// </summary>
        public static readonly DependencyProperty SaveToDatabaseConfirmationTextProperty = DependencyProperty.Register(
            "SaveToDatabaseConfirmationText",
            typeof(string),
            typeof(Editor),
            new PropertyMetadata(null));

        #endregion Dependency Properties

        #region Public Methods

        public void Cancel()
        {
            OnEditTurnedOff();

            if (Map == null) return;
            Map.MouseClick -= new EventHandler<Map.MouseEventArgs>(Map_MouseClick);

            var layer = Map.Layers[TempLayerID] as GraphicsLayer;
            layer.MouseRightButtonDown -= new GraphicsLayer.MouseButtonEventHandler(Layer_MouseRightButtonDown);

            if (_textEditor.CancelActive.CanExecute(null)) _textEditor.CancelActive.Execute(null);

            Editor editor = this.Map.GetValue(ActiveEditorProperty) as Editor;
            if (editor == null) return;
            if (editor._activeDraw == null) return;

            editor._activeDraw.IsShapeEnabled = false;
            editor._activeDraw = null;
            editor.DetachLayerEvents(GetGraphicsLayers(editor.Map, LayerIDs));
            editor.DetachLayerEvents(GetGraphicsLayers(editor.Map, new string[] { TextLayerID }));
            this.Map.ClearValue(ActiveEditorProperty);
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnEditCompleted(EsriEditor.EditEventArgs e)
        {
            EventHandler<EsriEditor.EditEventArgs> handler = this.EditCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnEditTurnedOn()
        {
            EventHandler handler = this.EditTurnedOn;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected virtual void OnEditTurnedOff()
        {
            EventHandler handler = this.EditTurnedOff;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected virtual void OnEditPublishing()
        {
            EventHandler handler = this.EditPublishing;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected virtual void OnEditPublished()
        {
            EventHandler handler = this.EditPublished;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected virtual void OnEditSaving()
        {
            EventHandler handler = this.EditSaving;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected virtual void OnEditsQuerying()
        {
            EventHandler handler = this.EditsQuerying;
            if (handler != null)
            {
                handler(this, null);
            }
        }

        protected virtual void OnEditCopying(Edit edit, Graphic originalGraphic)
        {
            EventHandler<ModifyEditEventArgs> handler = this.EditCopying;
            if (handler != null)
            {
                handler(this, new ModifyEditEventArgs(edit, originalGraphic));
            }
        }

        protected virtual void OnEditPasting(Edit edit)
        {
            EventHandler<ModifyEditEventArgs> handler = this.EditPasting;
            if (handler != null)
            {
                handler(this, new ModifyEditEventArgs(edit, null));
            }
        }

        #endregion Protected Methods

        #region Internal Properties

        internal ICommand AddShape { get; private set; }

        #endregion Internal Properties

        #region Internal Events

        internal event EventHandler<Editor.EditEventArgs> EditCompleted2;

        #endregion Internal Events

        #region Internal Methods

        internal void Open(Stream stream)
        {
            if (Map == null) return;
            if (Map.Layers == null) return;

            using (var reader = new StreamReader(stream))
            {
                string xml = reader.ReadToEnd();
                if (string.IsNullOrEmpty(xml)) return;

                XElement layersElement = XElement.Parse(xml);
                Envelope envelope = new Envelope();
                string unimportedLayers = string.Empty;
                bool errorImport = false;

                foreach (XElement layerElement in layersElement.Descendants("Layer"))
                {
                    string layerID = layerElement.Attribute("ID").Value;
                    FeatureLayer layer = Map.Layers.FirstOrDefault(l => l.ID == layerID) as FeatureLayer;
                    if (layer != null)
                    {
                        foreach (XElement editElement in layerElement.Elements())
                        {
                            string type = UpdateType(editElement.Attribute("Type").Value);

                            Edit edit = (Edit)Activator.CreateInstance(Type.GetType(type), layer);
                            edit.Layer = layer;

                            foreach (XElement graphicsElement in editElement.Elements())
                            {
                                MemoryStream memoryStream = new MemoryStream(new System.Text.UTF8Encoding().GetBytes(graphicsElement.ToString()));
                                List<EditGraphic> graphics = new List<EditGraphic>();
                                graphics.DeserializeGraphics(XmlReader.Create(memoryStream));
                                foreach (EditGraphic graphic in graphics)
                                {
                                    if (Map.SpatialReference.WKID != graphic.Geometry.SpatialReference.WKID)
                                    {
                                        System.Windows.MessageBox.Show("Graphics with the spatial reference different from that of the map cannot be imported from the XML.", "Import", MessageBoxButton.OK);
                                        errorImport = true;
                                        break;
                                    }
                                    edit.Graphic = graphic;
                                    envelope = envelope.Union(graphic.Geometry.Extent);
                                }
                                if (errorImport == true) break;
                                edit.Replay();
                                _edits.Add(edit);
                            }

                            if (errorImport == true) break;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(unimportedLayers) == true)
                        {
                            unimportedLayers = layerID;
                        }
                        else
                        {
                            unimportedLayers = unimportedLayers + Environment.NewLine + layerID;
                        }
                    }

                    if (errorImport == true) break;
                }

                Map.ZoomTo(envelope.Expand(2));

                if (unimportedLayers.Length > 0)
                {
                    System.Windows.MessageBox.Show("The following layers were not imported: " + Environment.NewLine + Environment.NewLine + unimportedLayers, "Import", MessageBoxButton.OK);
                }
            }
        }

        internal void Save(Stream stream, string fileName)
        {
            if (stream == null) return;

            for (int i = 0; i < _edits.Count; i++)
            {
                var edit = _edits[i];
                if (edit.Graphic.Symbol is ESRI.ArcGIS.Client.Symbols.TextSymbol && edit.Graphic.Attributes[TextValue].ToString() == "")
                {
                    _edits.RemoveAt(i);
                    i--;
                }
            }

            OnEditSaving();

            if (_edits.Count == 0) return;

            XmlWriterSettings xmlSettings = null;
#if DEBUG
            xmlSettings = new XmlWriterSettings { Indent = true, IndentChars = ("\t"), };
            xmlSettings.ConformanceLevel = ConformanceLevel.Auto;
#endif
            using (XmlWriter writer = XmlWriter.Create(stream, xmlSettings))
            {
                writer.WriteStartElement("Layers");
                var groupedEdits = _edits.GroupBy(edit => edit.Layer);
                foreach (var group in groupedEdits)
                {
                    writer.WriteStartElement("Layer");
                    writer.WriteAttributeString("ID", group.Key.ID); // LayerID
                    foreach (Edit edit in group)
                    {
                        writer.WriteStartElement("Edit");
                        writer.WriteAttributeString("Type", edit.GetType().AssemblyQualifiedName);
                        List<EditGraphic> graphics = new List<EditGraphic> { new EditGraphic(edit.Graphic) };
                        graphics.SerializeGraphics(writer);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            System.Windows.MessageBox.Show(string.Format("Finished exporting to XML file: {0}", fileName), "Export To XML", MessageBoxButton.OK);
        }

        internal void AddEditOperation(EsriEditor.EditAction action, IEnumerable<Change> edits)
        {
            foreach (Change change in edits)
            {
                GraphicsLayer layer = change.Layer as GraphicsLayer;
                Graphic graphic = change.Graphic;

                AddEdit(action, layer, graphic);
            }
        }

        internal void AddEditOperation(EsriEditor.EditAction action, IEnumerable<EsriEditor.Change> edits)
        {
            if (edits == null) return;

            foreach (EsriEditor.Change change in edits)
            {
                GraphicsLayer layer = change.Layer as GraphicsLayer;
                Graphic graphic = change.Graphic;

                if (action == EsriEditor.EditAction.EditVertices)
                {
                    graphic.Geometry.SpatialReference = Map.SpatialReference;

                    if (graphic.Geometry is Polygon)
                    {
                        GeometryService geometryService = new GeometryService(GeometryService);
                        geometryService.SimplifyCompleted += GeometryService_SimplifyCompleted;

                        var graphicList = new List<Graphic>();
                        graphicList.Add(new Graphic { Geometry = graphic.Geometry });
                        geometryService.SimplifyAsync(graphicList);
                        _simplifiedGraphic = graphic;
                    }
                }

                AddEdit(action, layer, graphic);
            }
        }

        #endregion Internal Methods

        #region Private Dependency Properties

        private static readonly DependencyProperty ActiveEditorProperty = DependencyProperty.RegisterAttached(
            "ActiveEditor",
            typeof(Editor),
            typeof(Editor),
            null);

        #endregion Private Dependency Properties

        #region Event Handlers

        private void SaveToDatabaseExecuteChanged(object sender, EventArgs e)
        {
            ((DelegateCommand<object>)SaveToDatabase).RaiseCanExecuteChanged();
        }

        void Layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Compare(e.PropertyName, VisiblePropertyName, StringComparison.CurrentCultureIgnoreCase) == 0)
            {
                ((DelegateCommand<object>)AddText).RaiseCanExecuteChanged();
            }
        }

        private void TextEditor_EditCompleted(object sender, EsriEditor.EditEventArgs e)
        {
            if (e == null) return;

            if (e.Action == EsriEditor.EditAction.Add)
            {
                OnEditCompleted(e);
            }
            AddEditOperation(e.Action, e.Edits);
        }

        private void Editor_EditCompleted(object sender, EsriEditor.EditEventArgs e)
        {
            if (e == null) return;

            OnEditCompleted(e);
            AddEditOperation(e.Action, e.Edits);
        }

        private void Edits_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            (SaveToFile as DelegateCommand<FileOption>).RaiseCanExecuteChanged();
            //((DelegateCommand<object>)ClearEdits).RaiseCanExecuteChanged();
            ((DelegateCommand<object>)SaveToDatabase).RaiseCanExecuteChanged();
        }

        private void Layer_MouseLeftButtonDown(object sender, GraphicMouseButtonEventArgs e)
        {
            if (e == null) return;
            if (e.Graphic == null) return;
            if (e.Graphic.Selected == false) return;
            if (Keyboard.Modifiers == ModifierKeys.Shift) return;

            _isGraphicsMarked = false;
            _lastDragPoint = Map.ScreenToMap(e.GetPosition(this.Map));

            Map.MouseMove += new MouseEventHandler(Map_MouseMove);
            Map.MouseLeftButtonUp += new MouseButtonEventHandler(Map_MouseLeftButtonUp);

            e.Handled = true;
        }

        private void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Map.MouseMove -= Map_MouseMove;
            Map.MouseLeftButtonUp -= Map_MouseLeftButtonUp;
            _isGraphicsMarked = false;

            // Update all of the Geometries so that the Esri editor says we can save them.
            foreach (GraphicsLayer layer in GetGraphicsLayers(Map, _editor.LayerIDs))
            {
                foreach (Graphic graphic in layer.SelectedGraphics)
                {
                    graphic.Geometry = Geometry.Clone(graphic.Geometry);
                }
            }
        }

        private void Map_MouseMove(object sender, MouseEventArgs e)
        {
            if (e == null) return;

            System.Windows.Point position = e.GetPosition(this.Map);
            if (position == null) return;

            MapPoint mapPoint = Map.ScreenToMap(position);
            if (mapPoint == null) return;

            Distance distance = GetRelativeDistance(mapPoint, _lastDragPoint);
            if (distance == null) return;

            if (_isGraphicsMarked == false)
            {
                MarkGraphicsAsModified();
            }

            foreach (GraphicsLayer layer in GetGraphicsLayers(Map, _editor.LayerIDs))
            {
                MoveGraphics(layer.SelectedGraphics, distance);
            }

            _lastDragPoint = mapPoint;
        }

        void Map_MouseClick(object sender, Map.MouseEventArgs e)
        {
            _textSymbol = new Graphic() { Geometry = e.MapPoint };

            var layer = Map.Layers[TextLayerID] as FeatureLayer;
            var textField = layer.LayerInfo.Fields.FirstOrDefault(field => field.Name == TextValue);
            
            TextEditor textEditor;
            if (textField != null && textField.Length > 0)
            {
                textEditor = new TextEditor(textField.Length);
            }
            else
            {
                textEditor = new TextEditor();
            }
            textEditor.TextEditApplied += new EventHandler<TextEditEventArgs>(TextEditor_TextEditApplied);

            var textWindow = new ChildWindow
            {
#if SILVERLIGHT
                HasCloseButton = true,
                Title = TextEditorTitle,
#elif WPF
                Caption = TextEditorTitle,
#endif
                Content = textEditor,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textWindow.Show();
        }

        void TextEditor_TextEditApplied(object sender, TextEditEventArgs e)
        {
            if (e.Xaml != "")
            {
                _textSymbol.Attributes[TextValue] = e.Xaml;

                var layer = Map.Layers[TextLayerID] as GraphicsLayer;
                if (!layer.Graphics.Contains(_textSymbol))
                {
                    layer.Graphics.Add(_textSymbol);
                }

                var edits = new List<Change> { new Change { Graphic = _textSymbol, Layer = layer } };
                AddEditOperation(EsriEditor.EditAction.Add, edits);
            }
        }

        #endregion Event Handlers

        #region Property Changed Callbacks

        private static void OnTextTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Editor editor = (Editor)d;
            editor._redlineText.TextTemplate = editor.TextTemplate;
        }

        private static void OnActiveSymbolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Editor editor = (Editor)d;

            (editor.AddShape as DelegateCommand<object>).RaiseCanExecuteChanged();
            editor.ClearSelection();
        }

        private static void OnTextLayerIDChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Editor editor = (Editor)d;
            string oldValue = e.OldValue as string;
            string newValue = e.NewValue as string;

            List<string> layerIDs = new List<string>();
            if (editor.LayerIDs != null)
            {
                layerIDs.AddRange(editor.LayerIDs);
            }
            layerIDs.Add(newValue);
            editor._editor.LayerIDs = layerIDs;
            editor._textEditor.LayerIDs = new string[] { newValue };
            editor._redlineText.LayerID = newValue;

            editor.DetachLayerEvents(GetGraphicsLayers(editor.Map, new string[] { oldValue }));
            editor.AttachLayerEvents(GetGraphicsLayers(editor.Map, new string[] { newValue }));
        }

        private static void OnContinuousModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Editor editor = (Editor)d;
            editor._editor.ContinuousMode = editor.ContinuousMode;
            editor._textEditor.ContinuousMode = editor.ContinuousMode;
        }

        private static void OnLayerIdsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Editor editor = (Editor)d;
            IEnumerable<string> newValue = e.NewValue as IEnumerable<string>;
            IEnumerable<string> oldValue = e.OldValue as IEnumerable<string>;

            List<string> layerIDs = newValue.ToList();
            if (editor.TextLayerID != null)
            {
                layerIDs.Add(editor.TextLayerID);
            }
            editor._editor.LayerIDs = layerIDs;

            editor.DetachLayerEvents(GetGraphicsLayers(editor.Map, oldValue));
            editor.AttachLayerEvents(GetGraphicsLayers(editor.Map, newValue));
        }

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            Editor editor = (Editor)d;
            Map oldMap = args.OldValue as Map;
            Map newMap = args.NewValue as Map;

            if (oldMap != null)
            {
                editor.DetachLayerEvents(GetGraphicsLayers(oldMap, editor._editor.LayerIDs));
            }

            if (newMap != null)
            {
                editor.AttachLayerEvents(GetGraphicsLayers(newMap, editor._editor.LayerIDs));
            }

            editor._editor.Map = newMap;
            editor._textEditor.Map = newMap;
            editor._redlineText.Map = newMap;
        }

        #endregion Property Changed Callbacks

        #region Private Methods

        private string UpdateType(string oldType)
        {
            const string publicKeyToken = @"PublicKeyToken=";
            string newType = oldType;

            string assemblyName = Assembly.GetExecutingAssembly().ToString();
            string newKey = assemblyName.Substring(assemblyName.IndexOf(publicKeyToken));

            string oldKey = oldType.Substring(oldType.IndexOf(publicKeyToken));

            if (string.Compare(oldKey, newKey, StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                newType = oldType.Replace(oldKey, newKey);
            }

            return newType;
        }

        private bool CanSaveToDatabase(object arg)
        {
            (SaveToFile as DelegateCommand<FileOption>).RaiseCanExecuteChanged();

            OnEditsQuerying();
            return (_edits.Count > 0 || _hasExtraEdits);
        }

        private void PerformSaveToDatabase(object obj)
        {
            string confirmationText = SaveToDatabaseConfirmationText;
#if SILVERLIGHT
            if (string.IsNullOrWhiteSpace(confirmationText) || System.Windows.Browser.HtmlPage.Window.Confirm(confirmationText))
#elif WPF
            if (string.IsNullOrWhiteSpace(confirmationText) || System.Windows.MessageBox.Show(confirmationText, "Publish to Database", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
#endif
            {
                OnEditPublishing();

                if (_editor.Save.CanExecute(null))
                {
                    _editor.Save.Execute(obj);
                }

                OnEditPublished();
            }
        }

        internal bool HasEdits(FileOption option)
        {
            OnEditsQuerying();
            return _edits.Count > 0 || _hasExtraEdits;
        }

        private bool ClipBoardHasItems(object param)
        {
            return _clipBoard.Edits.Count > 0;
        }

        private bool CanAddText(object param)
        {
            bool canAddText = true;
            if (string.IsNullOrEmpty(TextLayerID))
            {
                canAddText = false;
            }
            else if (Map == null)
            {
                canAddText = false;
            }
            else if (Map.Layers[TextLayerID].Visible == false)
            {
                canAddText = false;
            }

            if (canAddText == false)
            {
                _textEditor.CancelActive.Execute(null);
            }

            return canAddText;
        }

        private bool CanAddPolygon(object param)
        {
            return (ActiveSymbol != null) && (ActiveSymbol is FillSymbol) && (_editor.Add.CanExecute(param));
        }

        private bool HasSelection(object param)
        {
            IEnumerable<GraphicsLayer> layers = _editor.GraphicsLayers;
            var test = from layer in layers
                       where (true && layer.IsInitialized == true) && ((FeatureLayer)layer).IsReadOnly
                       select layer;
            if (layers == null) return false;

            foreach (GraphicsLayer layer in layers)
            {
                if (layer.SelectionCount > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void Open(FileOption option)
        {
            switch (option)
            {
                case FileOption.IsolatedStorage:
#if SILVERLIGHT
                    IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
#elif WPF
                    IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly();
#endif
                    if (store.FileExists("Edits.xml"))
                    {
                        using (Stream stream = store.OpenFile("Edits.xml", FileMode.Open, FileAccess.Read))
                        {
                            Open(stream);
                        }
                    }
                    break;
                case FileOption.FileSystem:
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Filter = "Xml Files|*.xml";
                    dialog.Multiselect = false;
                    if ((bool)dialog.ShowDialog())
                    {
#if SILVERLIGHT
                        using (Stream stream = dialog.File.OpenRead())
#elif WPF
                        using (Stream stream = dialog.OpenFile())
#endif
                        {
                            Open(stream);
                        }
                    }
                    break;
                default:
                    // Not supported
                    break;
            }
        }

        private void Save(FileOption option)
        {
            string fileName;
            using (Stream file = GetFileStream(option, out fileName))
            {
                if (file != null)
                {
                    switch (option)
                    {
                        case FileOption.IsolatedStorage:
                        case FileOption.FileSystem:
                            Save(file, fileName);
                            break;
                        default:
                            // Not Supported
                            break;
                    }
                }
            }
        }

        private void OnPaste(object param)
        {
            if (_clipBoard == null) return;
            ClearSelection();

            foreach (Edit edit in _clipBoard.Edits)
            {
                Edit pastedEdit = new AddEdit(edit.Layer);
                
                // We need to clone for both the edit, and the Layer.
                // Each needs it's own "copy" of the graphic. 
                // If we do not clone for both, then if we move the graphic, the edit
                // will show the new location, and not the original as it should.
                Graphic g = edit.Graphic.Clone();
                pastedEdit.Graphic = g.Clone();

                var factor = 5 * ++edit.PasteSum * Map.Resolution;

                if(pastedEdit.Graphic.Geometry is Polygon)
                {
                    var polygon = pastedEdit.Graphic.Geometry as Polygon;
                    foreach (var points in polygon.Rings)
                    {
                        foreach (var point in points)
                        {
                            point.X += factor;
                            point.Y -= factor;
                        }
                    }
                }
                else if (pastedEdit.Graphic.Geometry is Polyline)
                {
                    var polyline = pastedEdit.Graphic.Geometry as Polyline;
                    foreach (var points in polyline.Paths)
                    {
                        foreach (var point in points)
                        {
                            point.X += factor;
                            point.Y -= factor;
                        }
                    }
                }
                else
                {
                    var point = pastedEdit.Graphic.Geometry as MapPoint;
                    point.X += factor;
                    point.Y -= factor;

                    if (pastedEdit.Graphic.Symbol is CustomPointSymbol)
                    {
                        OnEditPasting(pastedEdit);
                    }
                }
                edit.Layer.Graphics.Add(pastedEdit.Graphic);
                _edits.Add(pastedEdit);
            }
            (CopySelected as DelegateCommand<object>).RaiseCanExecuteChanged();
        }

        private void OnAdd(object param)
        {
            Cancel();

            ActiveSymbol = null;
            _editor.Add.Execute(param);
        }

        private void OnEdit(object param)
        {
            Cancel();

            ClearSelection();

            ActiveSymbol = null;
            _editor.EditVertices.Execute(param);

            OnEditTurnedOn();
            
            var layer = Map.Layers[TempLayerID] as GraphicsLayer;
            layer.MouseRightButtonDown += new GraphicsLayer.MouseButtonEventHandler(Layer_MouseRightButtonDown);
        }

        void Layer_MouseRightButtonDown(object sender, GraphicMouseButtonEventArgs e)
        {
            var layer = Map.Layers[TextLayerID] as FeatureLayer;
            var textField = layer.LayerInfo.Fields.FirstOrDefault(field => field.Name == TextValue);

            _textSymbol = e.Graphic.Attributes[TextGraphicKey] as Graphic;

            var textEditor = new TextEditor((e.Graphic.Symbol as CustomTextSymbol).UncompressedXaml);
            textEditor.TextEditApplied += new EventHandler<TextEditEventArgs>(TextEditor_TextEditApplied);
            textEditor.TextFieldLength = textField.Length;

            var textWindow = new ChildWindow
            {
#if SILVERLIGHT
                HasCloseButton = true,
                Title = TextEditorTitle,
#elif WPF
                Caption = TextEditorTitle,
#endif
                Content = textEditor,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            textWindow.Show();

            e.Handled = true;
        }

        private void OnSelect(object param)
        {
            Cancel();

            ActiveSymbol = null;
            _editor.Select.Execute(param);
        }

        private void OnAddText(object param)
        {
            Cancel();
            var layer = Map.Layers[TextLayerID] as FeatureLayer;
            if (layer == null) return;
            if (layer.Visible == false) return;

            Map.MouseClick += new EventHandler<Map.MouseEventArgs>(Map_MouseClick);
        }

        private void OnAddShape(object param)
        {
            Cancel();
            Map.SetValue(ActiveEditorProperty, this);

            FillSymbol symbol = new FillSymbol();
            FeatureLayer layer = Map.Layers[LayerIDs.First()] as FeatureLayer;
            FeatureTemplate template = null;
            if (layer.Renderer != null)
            {
                symbol = layer.Renderer.GetSymbol(null) as FillSymbol;
                var collection = (param as object[])[1] as object[];
                if (collection != null)
                {
                    if ((collection.Length > 0) && (layer.LayerInfo.FeatureTypes.Count > 0))
                    {
                        FeatureType featureType = layer.LayerInfo.FeatureTypes[collection[0]];
                        if (collection.Length > 1)
                        {
                            template = featureType.Templates[collection[1].ToString()];
                            if (template != null)
                            {
                                symbol = template.GetSymbol(layer.Renderer) as FillSymbol;
                            }
                        }
                    }
                }
            }

            DrawMode mode;
            switch((param as object[])[0].ToString())
            {
                case "arrow":
                    mode = DrawMode.Arrow;
                    break;
                case "circle":
                    mode = DrawMode.Circle;
                    break;
                case "ellipse":
                    mode = DrawMode.Ellipse;
                    break;
                case "freehand":
                    mode = DrawMode.Freehand;
                    break;
                case "rectangle":
                    mode = DrawMode.Rectangle;
                    break;
                case "triangle":
                    mode = DrawMode.Triangle;
                    break;
                default:
                    mode = DrawMode.Polygon;
                    break;
            }

            _activeDraw = new ShapeDraw(Map, mode) { FillSymbol = symbol, IsShapeEnabled = true };
            _activeDraw.DrawComplete += (s, e) =>
            {
                if (e.Geometry == null)
                {
                    return;
                }

                var geometry = e.Geometry;
                if (e.Geometry is Envelope)
                {
                    geometry = new Polygon();

                    var point1 = new MapPoint(e.Geometry.Extent.XMin, e.Geometry.Extent.YMin);
                    var point2 = new MapPoint(e.Geometry.Extent.XMax, e.Geometry.Extent.YMin);
                    var point3 = new MapPoint(e.Geometry.Extent.XMax, e.Geometry.Extent.YMax);
                    var point4 = new MapPoint(e.Geometry.Extent.XMin, e.Geometry.Extent.YMax);
                    (geometry as Polygon).Rings.Add(new PointCollection(new List<MapPoint>() { point1, point2, point3, point4, point1 }));
                }
                else if (e.Geometry is Polyline)
                {
                    geometry = new Polygon();
                    geometry.SpatialReference = Map.SpatialReference;
                    (geometry as Polygon).Rings.Add((e.Geometry as Polyline).Paths[0]);
                    (geometry as Polygon).Rings[0].Add((e.Geometry as Polyline).Paths[0][0]);

                    GeometryService geometryService = new GeometryService(GeometryService);
                    geometryService.SimplifyCompleted += GeometryService_SimplifyCompleted;

                    var graphicList = new List<Graphic>();
                    graphicList.Add(new Graphic { Geometry = geometry });
                    geometryService.SimplifyAsync(graphicList);
                }
                geometry.SpatialReference = Map.SpatialReference;

                var graphic = new Graphic { Symbol = symbol, Geometry = geometry };
                _simplifiedGraphic = graphic;
                if (template != null)
                {
                    foreach (var attribute in template.PrototypeAttributes)
                    {
                        graphic.Attributes.Add(attribute);
                    }
                }
                layer.Graphics.Add(graphic);

                EditEventArgs args = new EditEventArgs
                {
                    Action = EsriEditor.EditAction.Add,
                    Edits = new List<Change> { new Change { Graphic = graphic, Layer = layer } }
                };
                OnEditCompleted2(args);
            };
        }

        private void GeometryService_SimplifyCompleted(object sender, GraphicsEventArgs args)
        {
            _simplifiedGraphic.Geometry = args.Results[0].Geometry;
        }

        void Map_KeyDown(object sender, KeyEventArgs e)
        {
#if SILVERLIGHT
            if (e.Key == Key.Shift)
#elif WPF
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
#endif
            {
                _editor.MaintainAspectRatio = true;
            }
#if SILVERLIGHT
            else if (e.Key == Key.Ctrl)
#elif WPF
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
#endif
            {
                _editor.EditVerticesEnabled = false;
            }
        }

        void Map_KeyUp(object sender, KeyEventArgs e)
        {
#if SILVERLIGHT
            if (e.Key == Key.Shift)
#elif WPF
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
#endif
            {
                _editor.MaintainAspectRatio = false;
            }
#if SILVERLIGHT
            else if (e.Key == Key.Ctrl)
#elif WPF
            else if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
#endif
            {
                _editor.EditVerticesEnabled = true;
            }
        }

        private void Copy(object param)
        {
            _clipBoard.Edits.Clear();

            var oids = new string[] { "Object ID", "ObjectID", "OBJECTID" };
            foreach (GraphicsLayer layer in _editor.GraphicsLayers)
            {
                if (layer.SelectionCount > 0)
                {
                    foreach (Graphic graphic in layer.SelectedGraphics)
                    {
                        if (graphic.Symbol is ESRI.ArcGIS.Client.Symbols.TextSymbol && graphic.Attributes[TextValue].ToString() == "") continue;

                        Edit edit = new AddEdit(layer) { Graphic = graphic.Clone() };

                        var oid = oids.FirstOrDefault(id => edit.Graphic.Attributes[id] != null);
                        if (oid != null)
                        {
                            edit.Graphic.Attributes[oid] = null;
                        }

                        OnEditCopying(edit, graphic);
                        _clipBoard.Edits.Add(edit);
                    }
                }
            }

            (Paste as DelegateCommand<object>).RaiseCanExecuteChanged();
        }

        private void AddEdit(EsriEditor.EditAction action, GraphicsLayer layer, Graphic graphic)
        {
            if (layer == null) return;

            Edit edit = null;
            switch (action)
            {
                case EsriEditor.EditAction.Add:
                    edit = new AddEdit(layer) { Graphic = graphic };
                    _edits.Add(edit);
                    break;
                case EsriEditor.EditAction.Cancel:
                    break;
                case EsriEditor.EditAction.ClearSelection:
                    break;
                case EsriEditor.EditAction.Cut:
                    break;
                case EsriEditor.EditAction.DeleteSelected:
                    edit = _edits.FirstOrDefault(e => e.Graphic == graphic);
                    if (edit != null)
                    {
                        _edits.Remove(edit);
                    }
                    if ((edit == null) || (edit is ModifyEdit))
                    {
                        edit = new RemoveEdit(layer) { Graphic = graphic };
                        _edits.Add(edit);
                    }
                    break;
                case EsriEditor.EditAction.EditVertices:
                    if (_edits.Any(e => e.Graphic == graphic) == false)
                    {
                        edit = new ModifyEdit(layer) { Graphic = graphic };
                        _edits.Add(edit);
                    }

                    break;
                case EsriEditor.EditAction.Remove:
                    break;
                case EsriEditor.EditAction.Reshape:
                    break;
                case EsriEditor.EditAction.Save:
                    _edits.Clear();
                    break;
                case EsriEditor.EditAction.Select:
                    break;
                case EsriEditor.EditAction.Union:
                    break;
                default:
                    break;
            }
            (CopySelected as DelegateCommand<object>).RaiseCanExecuteChanged();
        }

        private static IEnumerable<GraphicsLayer> GetGraphicsLayers(Map map, IEnumerable<string> layerIDs)
        {
            return (map == null) || (layerIDs == null) ?
                new List<GraphicsLayer>() :
                from layer in map.Layers
                where layerIDs.Contains(layer.ID) && layer is GraphicsLayer
                select layer as GraphicsLayer;
        }

        private Stream GetFileStream(FileOption option, out string fileName)
        {
            Stream fileStream = null;
            fileName = "";

            if (option == FileOption.IsolatedStorage)
            {
#if SILVERLIGHT
                IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
#elif WPF
                IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForAssembly();
#endif
                fileStream = store.OpenFile("Edits.xml", FileMode.Create);
                fileName = "Edits.xml";
            }
            else if (option == FileOption.FileSystem)
            {
                var dialog = new SaveFileDialog
                {
                    DefaultExt = "*.xml",
                    Filter = "Xml Files|*.xml"
                };
                if (dialog.ShowDialog() == true)
                {
                    fileStream = dialog.OpenFile();
                    fileName = dialog.SafeFileName;
                }
            }

            return fileStream;
        }

        private void AttachLayerEvents(IEnumerable<GraphicsLayer> layers)
        {
            foreach (GraphicsLayer layer in layers)
            {
                this.AttachLayerEvents(layer);
            }
        }

        private void AttachLayerEvents(GraphicsLayer layer)
        {
            layer.MouseLeftButtonDown += new GraphicsLayer.MouseButtonEventHandler(Layer_MouseLeftButtonDown);
            layer.PropertyChanged += new PropertyChangedEventHandler(Layer_PropertyChanged);

            //layer.Initialized += new EventHandler<EventArgs>(this.layer_Initialized);
            //layer.Graphics.CollectionChanged += new NotifyCollectionChangedEventHandler(this.Graphics_CollectionChanged);
        }

        private void DetachLayerEvents(IEnumerable<GraphicsLayer> layers)
        {
            foreach (GraphicsLayer layer in layers)
            {
                this.DetachLayerEvents(layer);
            }
        }

        private void DetachLayerEvents(GraphicsLayer layer)
        {
            layer.MouseLeftButtonDown -= Layer_MouseLeftButtonDown;

            // layer.Initialized -= new EventHandler<EventArgs>(this.layer_Initialized);
            // layer.PropertyChanged -= new PropertyChangedEventHandler(this.Layer_PropertyChanged);
            // layer.Graphics.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.Graphics_CollectionChanged);
        }

        private void MoveGraphics(IEnumerable<Graphic> graphics, Distance distance)
        {
            if (graphics == null) return;
            if (distance == null) return;

            foreach (Graphic graphic in graphics)
            {
                MoveGraphic(graphic, distance);
            }
        }

        private void MoveGraphic(Graphic graphic, Distance distance)
        {
            if (graphic == null) return;
            if (distance == null) return;

            Geometry geometry = graphic.Geometry;

            if (geometry is MapPoint)
            {
                MapPoint mapPoint = geometry as MapPoint;
                MovePoint(mapPoint, distance);
            }
            else if (geometry is Polyline)
            {
                MoveVertices((geometry as Polyline).Paths, distance);
            }
            else if (geometry is Polygon)
            {
                MoveVertices((geometry as Polygon).Rings, distance);
            }
            else if (geometry is MultiPoint)
            {
                MoveVertices((geometry as MultiPoint).Points, distance);
            }
            else if (geometry is Envelope)
            {
                MoveVertices(geometry as Envelope, distance);
            }
            else
            {
                throw new Exception("Invalid geometry type");
            }
        }

        private void MoveVertices(Envelope envelope, Distance distance)
        {
            envelope.XMin += distance.X;
            envelope.YMin += distance.Y;
            envelope.XMax += distance.X;
            envelope.YMax += distance.Y;
        }

        private static void MovePoint(MapPoint mapPoint, Distance distance)
        {
            mapPoint.X += distance.X;
            mapPoint.Y += distance.Y;
        }

        private void MoveVertices(IEnumerable<PointCollection> points, Distance distance)
        {
            foreach (PointCollection vertices in points)
            {
                MoveVertices(vertices, distance);
            }
        }

        private void MoveVertices(PointCollection vertices, Distance distance)
        {
            foreach (MapPoint vertex in vertices)
            {
                MovePoint(vertex, distance);
            }
        }

        private Distance GetRelativeDistance(MapPoint mapPoint1, MapPoint mapPoint2)
        {
            Distance distance = new Distance();

            if (mapPoint1 == null) return distance;
            if (mapPoint2 == null) return distance;

            distance.X = mapPoint1.X - mapPoint2.X;
            distance.Y = mapPoint1.Y - mapPoint2.Y;

            return distance;
        }

        private void OnEditCompleted2(EditEventArgs args)
        {
            EventHandler<EditEventArgs> handler = this.EditCompleted2;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void ClearSelection()
        {
            if (_editor.ClearSelection.CanExecute(null))
            {
                _editor.ClearSelection.Execute(null);
            }
        }

        private void MarkGraphicsAsModified()
        {
            _isGraphicsMarked = true;
            foreach (GraphicsLayer layer in GetGraphicsLayers(Map, _editor.LayerIDs))
            {
                foreach (Graphic graphic in layer.SelectedGraphics)
                {
                    // No need to create a ModifyEdit if there is already an Edit associated
                    // with this graphic.
                    if (_edits.Any(ed => ed.Graphic == graphic) == false)
                    {
                        ModifyEdit edit = new ModifyEdit(layer) { Graphic = graphic };
                        _edits.Add(edit);
                    }
                }
            }
        }

        #endregion Private Methods

        #region Distance Class

        class Distance
        {
            public double X { get; set; }
            public double Y { get; set; }
        }

        #endregion Distance Class

        #region EditEventArgs Class

        public class EditEventArgs : EventArgs
        {
            // Summary:
            //     Gets the type of edit.
            public EsriEditor.EditAction Action { get; set; }
            //
            // Summary:
            //     Gets a list of edits.
            public IEnumerable<Editor.Change> Edits { get; set; }
        }

        #endregion EditEventArgs Class

        #region Change Class

        public class Change
        {
            // Summary:
            //     Gets the graphic that was changed.
            public Graphic Graphic { get; internal set; }
            //
            // Summary:
            //     Gets the layer the change was performed on.
            public Layer Layer { get; internal set; }
        }

        #endregion Change Class
    }
}
