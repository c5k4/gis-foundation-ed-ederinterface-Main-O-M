using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ESRI.ArcGIS.Client;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using System.Xml.Linq;

namespace ArcFMSilverlight
{
    public class AddRemoveDataTool : Control
    {
        private ToggleButton _addDataToggleButton;
        private ToggleButton _removeDataToggleButton;
        public FloatableWindow _fwAddData = null;
        public FloatableWindow _fwRemoveData = null;
        private Grid _mapArea;
        private AddDataControl _addDataControl;
        private RemoveDataControl _removeDataControl;
        public AddDataControl AddDataControl
        {
            get { return _addDataControl; }
        }
        public RemoveDataControl RemoveDataControl
        {
            get { return _removeDataControl; }
        }
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }
        private List<TempLayer> _templayerList;

        public AddRemoveDataTool(ToggleButton addDataToggleButton, ToggleButton removeDataToggleButton, Map map, Grid mapArea, XElement addDataElement, MapTools Tools)
        {
            DefaultStyleKey = typeof(AddRemoveDataTool);
            EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);
            _addDataToggleButton = addDataToggleButton;
            _removeDataToggleButton = removeDataToggleButton;
            this.Map = map;
            _mapArea = mapArea;
            OnApplyTemplate();
            _addDataToggleButton.IsEnabled = true;
            _removeDataToggleButton.IsEnabled = true;
            ToolTipService.SetToolTip(_addDataToggleButton, "Add Data");
            ToolTipService.SetToolTip(_removeDataToggleButton, "Remove Temporary Layers");
            _addDataControl = new AddDataControl(map, addDataElement, Tools);
            _removeDataControl = new RemoveDataControl(map, Tools);
            _addDataControl.TempLayerList = _templayerList;
            _removeDataControl.TempLayerList = _templayerList;
            _addDataControl.RemoveFileFromServer();
        }

        /// <summary>
        /// Current map.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(AddRemoveDataTool),
            new PropertyMetadata(OnMapChanged));

        [Category("Export Properties")]
        [Description("Associated Map")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        #region Public Overrides

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _templayerList = new List<TempLayer>();
            _addDataToggleButton.Click += new RoutedEventHandler(_addDataToggleButton_Click);
            _removeDataToggleButton.Click += new RoutedEventHandler(_removeDataToggleButton_Click);

        }

        #endregion Public Overrides

        #region IActiveControl Members

        private bool _isAddDataActive = false;
        private bool _isRemoveDataActive = false;
        public bool isAddDataActive
        {
            get { return _isAddDataActive; }
            set { setAddDataActive(value); }
        }

        public bool isRemoveDataActive
        {
            get { return _isRemoveDataActive; }
            set { setRemoveDataActive(value); }
        }

        private void setAddDataActive(bool isActive)
        {
            _isAddDataActive = isActive;
            if (_fwAddData == null) return;

            if (!isActive)
            {
                //close window
                _fwAddData.Visibility = System.Windows.Visibility.Collapsed;
                _addDataToggleButton.IsChecked = false;
            }
            else // Active
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                isRemoveDataActive = false;
                _addDataToggleButton.IsChecked = true;
                _addDataControl.IsEnabled = true;
                _fwAddData.Visibility = System.Windows.Visibility.Visible;
                ConfigUtility.StatusBar.Text = "";
            }

        }

        private void setRemoveDataActive(bool isActive)
        {
            _isRemoveDataActive = isActive;
            if (_fwRemoveData == null) return;

            if (!isActive)
            {
                // close window
                _fwRemoveData.Visibility = System.Windows.Visibility.Collapsed;
                _removeDataToggleButton.IsChecked = false;
            }
            else // Active
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(this);
                isAddDataActive = false;
                _removeDataToggleButton.IsChecked = true;
                _removeDataControl.IsEnabled = true;
                _fwRemoveData.Visibility = System.Windows.Visibility.Visible;
                _removeDataControl.FillLayerCboBox();
                ConfigUtility.StatusBar.Text = "";
            }

        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            isAddDataActive = false;
            isRemoveDataActive = false;
        }

        #endregion IActiveControl Members

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tool = d as AddRemoveDataTool;
            if (tool == null) return;
        }

        void _addDataToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fwAddData == null)
            {
                _fwAddData = new FloatableWindow();
                _fwAddData.ParentLayoutRoot = _mapArea;
                _fwAddData.Width = 340;
                _fwAddData.Height = 330;
                _fwAddData.Title = "Add Data";
                _fwAddData.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fwAddData.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fwAddData.ResizeMode = ResizeMode.NoResize;
                _fwAddData.Content = _addDataControl;
                _fwAddData.Closing += new EventHandler<CancelEventArgs>(_fw_AddData_Closing);
                _fwAddData.Show();
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                setAddDataActive(true);
            }
            else
            {
                if (_fwAddData.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fwAddData.Visibility = System.Windows.Visibility.Visible;
                    _addDataToggleButton.IsChecked = true;
                    setAddDataActive(true);
                }
                else
                {
                    _fwAddData.Visibility = System.Windows.Visibility.Collapsed;
                    _addDataToggleButton.IsChecked = false;
                    if (this.PreviousControl != null) this.PreviousControl();
                }
            }
        }

        void _removeDataToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_fwRemoveData == null)
            {
                _fwRemoveData = new FloatableWindow();
                _fwRemoveData.ParentLayoutRoot = _mapArea;
                _fwRemoveData.Width = 300;
                _fwRemoveData.Height = 120;
                _fwRemoveData.Title = "Remove Temporary Layers";
                _fwRemoveData.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                _fwRemoveData.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fwRemoveData.ResizeMode = ResizeMode.NoResize;
                _fwRemoveData.Content = _removeDataControl;
                _fwRemoveData.Closing += new EventHandler<CancelEventArgs>(_fw_RemoveData_Closing);
                _fwRemoveData.Show();
                
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                setRemoveDataActive(true);
            }
            else
            {
                if (_fwRemoveData.Visibility == System.Windows.Visibility.Collapsed)
                {
                    _fwRemoveData.Visibility = System.Windows.Visibility.Visible;
                    _removeDataToggleButton.IsChecked = true;
                    setRemoveDataActive(true);
                }
                else
                {
                    _fwRemoveData.Visibility = System.Windows.Visibility.Collapsed;
                    _removeDataToggleButton.IsChecked = false;
                    if (this.PreviousControl != null) this.PreviousControl();
                }
            }
        }

        void _fw_AddData_Closing(object sender, CancelEventArgs e)
        {
            this.isAddDataActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fwAddData.Visibility = System.Windows.Visibility.Collapsed;
            if (this.PreviousControl != null) this.PreviousControl();
        }

        void _fw_RemoveData_Closing(object sender, CancelEventArgs e)
        {
            this.isRemoveDataActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fwAddData.Visibility = System.Windows.Visibility.Collapsed;
            if (this.PreviousControl != null) this.PreviousControl();
        }
    }

    public class TempLayer
    {
        public Layer layer;
        public string title;
    }
}
