using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ArcFMSilverlight.Butterfly;
using ArcFMSilverlight.Controls.Butterfly;
using ArcFMSilverlight.Controls.DeviceSettings;
using Miner.Server.Client.Toolkit;
using PageTemplates;
//ENOSChange
using ArcFMSilverlight.Controls.GenerationOnFeeder;

namespace ArcFMSilverlight.Controls.DataGridContextMenu
{
    // Note that this Class is doing a couple of things that are less than optimal to get this functionality
    // 1) It's finding elements by hardcoded path. Doing this because faster than a search.
    // 2) It's using timers instead of hooking up to a DataContextChanged event. That functionality was added at SL5 and we are at v4.
    public class DataGridContextMenu
    {
        private const string ASSIGNED_TAG = "Assigned";
        private AttributesViewerControl _attributesViewerControl;
        private DataGrid _attributeViewerDataGrid;
        private string _copiedCellValue = "";
        private StackPanel _printStackPanel = null;
        private ContextMenu _contextMenu = new ContextMenu();
        private MouseButtonEventArgs _lastCellClick;
        private MouseEventArgs _lastCPMouseMove;
        private FrameworkElement _tabContainerFrameworkElement;
        private bool _isFirstTime = true;
        private Button _rowDetailsButton;
        private ChildWindow _childWindow;
        public Action<Miner.Server.Client.Tasks.ResultSet, Miner.Server.Client.Row, string> SubDataGridRowSelected { get; set; }
        private ContextMenu _contextMenuRowDetails = new ContextMenu();
        private MouseButtonEventArgs _lastMouseLeftButtonUpClick = null;
        public event EventHandler DataGridBound;
        private IDictionary<string, IDataGridCustomButton> _customButtons;
        private MenuItem _outageHistoryMenuItem = null;
        private MenuItem _ConductorInTrenchMenuItem = null;
        private MenuItem _butterflyMenuItem = null;
        //*******ENOSChange Start**********
        private MenuItem _showGenMenuItem = null;
        private MenuItem _genOnFeederMenuItem = null;
        //*******ENOSChange End**********
        private bool _setTabContainerTimer = true;
        public bool LayerVisibilityControlIsVisible { get; set; }

        public DataGrid AttributeViewDataGrid
        {
            get
            {
                return _attributeViewerDataGrid;
            }
        }

        public DataGridContextMenu(AttributesViewerControl attributesViewerControl, IDictionary<string, IDataGridCustomButton> customButtons)
        {
            _attributesViewerControl = attributesViewerControl;
            _customButtons = customButtons;
            var menuItemCopy = new MenuItem() { Header = "Copy" };
            menuItemCopy.Click += new RoutedEventHandler(menuItem_Click);
            _contextMenu.Items.Add(menuItemCopy);
            var menuItemRowDetails = new MenuItem() { Header = "Row Details" };
            menuItemRowDetails.Click += new RoutedEventHandler(menuItemRowDetails_Click);
            _contextMenu.Items.Add(menuItemRowDetails);
            foreach (KeyValuePair<string, IDataGridCustomButton> dataGridCustomButton in customButtons)
            {
                var menuItemSettings = new MenuItem() { Header = dataGridCustomButton.Key };
                menuItemSettings.Click += new RoutedEventHandler(menuItemDataGridButton_Click);
                if (dataGridCustomButton.Value.ShowMe)
                {
                    _contextMenu.Items.Add(menuItemSettings);
                }
                if (dataGridCustomButton.Value.Name.Contains("Outage"))
                {
                    _outageHistoryMenuItem = menuItemSettings;
                }
                if (dataGridCustomButton.Value.Name.Contains("Butterfly"))
                {
                    _butterflyMenuItem = menuItemSettings;
                }
                //CIT for non-identify and grid tool *start
                if (dataGridCustomButton.Value.Name.Contains("Update FilledDuct"))
                {
                    _ConductorInTrenchMenuItem = menuItemSettings;
                }
                //CIT for non-identify and grid tool *end
                /*ENOSChange Start*/
                if (dataGridCustomButton.Value.Name.Contains("Show Generation"))
                {
                    _showGenMenuItem = menuItemSettings;
                }
                if (dataGridCustomButton.Value.Name.Contains("Generation On Feeder"))
                {
                    _genOnFeederMenuItem = menuItemSettings;
                }
                /*ENOSChange End*/
            }

            _contextMenu.Opened += new RoutedEventHandler(_contextMenu_Opened);

            _rowDetailsButton = MainPage.GetChildObject<Button>(_attributesViewerControl, "PART_RowDetailsButton");
            _rowDetailsButton.Click +=new RoutedEventHandler(_rowDetailsButton_Click);
            var menuItemCopyRowDetails = new MenuItem() { Header = "Copy" };
            menuItemCopyRowDetails.Click += new RoutedEventHandler(menuItemCopyRowDetails_Click);
            _contextMenuRowDetails.Items.Add(menuItemCopyRowDetails);
            var menuItemPrintRowDetails = new MenuItem() { Header = "Print" };
            menuItemPrintRowDetails.Click += new RoutedEventHandler(menuItemPrintRowDetails_Click);
            _contextMenuRowDetails.Items.Add(menuItemPrintRowDetails);
        }

        public DataGridContextMenu()
        {
            //INC000004469254 - For Details menu on right click
        }

        void menuItemPrintRowDetails_Click(object sender, RoutedEventArgs e)
        {
            PrintRowDetails printRowDetails = new PrintRowDetails();
//            printRowDetails.PrintScaled(_printStackPanel);
            printRowDetails.PrintPages(_printStackPanel);
        }

        void menuItemDataGridButton_Click(object sender, RoutedEventArgs e)
        {
            IDataGridCustomButton customButton = _customButtons[((MenuItem) sender).Header.ToString()];
            customButton.ButtonClicked();
        }

        void menuItemCopyRowDetails_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_copiedCellValue);
        }


        private void _rowDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer rowDetailsDispatcherTimer = new DispatcherTimer();
            rowDetailsDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000); // Milliseconds
            rowDetailsDispatcherTimer.Tick += new EventHandler(rowDetailsDispatcherTimer_Tick);
            rowDetailsDispatcherTimer.Start();
        }

        void rowDetailsDispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            foreach (var popup in VisualTreeHelper.GetOpenPopups())
            {
                if (popup.Child is ChildWindow)
                {
                    _childWindow = popup.Child as ChildWindow;
                    FrameworkElement frameworkElement = MainPage.GetChildObject<RowDetailsView>(_childWindow, "");
                    AttachContextMenuToChildWindow();
                    // Attach to Treeview
                    TreeView treeView = MainPage.GetChildObject<TreeView>(frameworkElement, "treeView");
                    treeView.SelectedItemChanged +=new RoutedPropertyChangedEventHandler<object>(treeView_SelectedItemChanged);
                }
            }
        }

        private void AttachPrintButton()
        {
            var fe = (FrameworkElement)VisualTreeHelper.GetChild(_childWindow, 0); // grid "root"
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 1); // grid "contentroot"
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 4); // border 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // border 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // grid 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // border 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // grid 
            Grid grid = fe as Grid;
            // Add button to top
            if (grid.Children.Count < 3)
            {
                Border border = new Border();
                border.Width = 80;
                border.Height = 20;
                Button button = new Button();
                //TODO: print button image?
                button.Content = "Print";
                button.Click += new RoutedEventHandler(button_Click);
                border.Child = button;

                grid.Children.Insert(1, border);
            }

        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = MainPage.GetChildObject<RowDetailsView>(_childWindow, "");
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // grid 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // dockpanel 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 1); // grid 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 1); // dockpanel 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 2); // scrollpanel
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // border 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // grid 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // Scrollcontentpresenter 
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // itemscontrol
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // itemspresenter
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // stackpanel

            PrintRowDetails printRowDetails = new PrintRowDetails();
//            printRowDetails.PrintScaled((StackPanel)fe);
            printRowDetails.PrintPages((StackPanel)fe);
        }

        private void AttachContextMenuToChildWindow()
        {
            AttachPrintButton();

            FrameworkElement frameworkElement = MainPage.GetChildObject<RowDetailsView>(_childWindow, "");
            var fe = (FrameworkElement)VisualTreeHelper.GetChild(frameworkElement, 0); // grid "layoutroot"
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // dockpanel
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 1); // grid[1]
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 1); // dockpanel[1]
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 2); // scrollviewer[2]
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // border
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // grid
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // scrollcontentpresenter
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // itemscontrol
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // itemspresenter
            fe = (FrameworkElement)VisualTreeHelper.GetChild(fe, 0); // stackpanel

            StackPanel stackPanel = fe as StackPanel;
            foreach (var contentPresenterFE in stackPanel.Children)
            {
                ContentPresenter contentPresenter = MainPage.GetChildObject<ContentPresenter>(
                    contentPresenterFE, "");
                TextBlock textBlock = MainPage.GetChildObject<TextBlock>(contentPresenter, "");
                textBlock.MouseRightButtonDown += new MouseButtonEventHandler(textBlock_MouseRightButtonDown);
                textBlock.MouseRightButtonUp += new MouseButtonEventHandler(textBlock_MouseRightButtonUp);
            }
            
        }

        void treeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            DispatcherTimer treeViewChangedDispatcherTimer = new DispatcherTimer();
            treeViewChangedDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500); // Milliseconds 
            treeViewChangedDispatcherTimer.Tick += new EventHandler(treeViewChangedDispatcherTimer_Tick);
            treeViewChangedDispatcherTimer.Start();

            // see if we can overload this pup
            if (e.NewValue != null)
                ((TreeViewItem)e.NewValue).MouseRightButtonDown += new MouseButtonEventHandler(DataGridContextMenu_MouseRightButtonDown);
        }

        void DataGridContextMenu_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        void treeViewChangedDispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();

            AttachContextMenuToChildWindow();
        }


        void textBlock_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Well, this is fun. The ContextMenu apparently belongs to the parent window so you have to offset it thus or it appears incorrectly positioned
            ContextMenuService.SetContextMenu((DependencyObject)sender, _contextMenuRowDetails);
            _contextMenuRowDetails.IsOpen = true;
            GeneralTransform gt2 = _contextMenuRowDetails.TransformToVisual(Application.Current.RootVisual as UIElement);
            _contextMenuRowDetails.IsOpen = false;
            Point contextMenuPoint = gt2.Transform(new Point(0, 0));
            double mouseClickPositionXAbsolute = e.GetPosition(Application.Current.RootVisual as UIElement).X;
            double mouseClickPositionYAbsolute = e.GetPosition(Application.Current.RootVisual as UIElement).Y;
            double offsetX = mouseClickPositionXAbsolute - contextMenuPoint.X;
            double offsetY = mouseClickPositionYAbsolute - contextMenuPoint.Y;

            _contextMenuRowDetails.HorizontalOffset += offsetX;
            _contextMenuRowDetails.VerticalOffset += offsetY; 
            _copiedCellValue = ((TextBlock) sender).Text;

            var parent = VisualTreeHelper.GetParent(sender as TextBlock);
            parent = VisualTreeHelper.GetParent(parent);
            parent = VisualTreeHelper.GetParent(parent);
            parent = VisualTreeHelper.GetParent(parent);
            parent = VisualTreeHelper.GetParent(parent);
            _printStackPanel = parent as StackPanel;
            _contextMenuRowDetails.IsOpen = true;
        }

        void textBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }



        void menuItemRowDetails_Click(object sender, RoutedEventArgs e)
        {
            // Calls click on the RowDetails programmatically
            if (_rowDetailsButton is Button)
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer((Button)_rowDetailsButton);

                IInvokeProvider ip = (IInvokeProvider)peer;
                ip.Invoke();
            }
        }

        void menuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_copiedCellValue != null)
            {
                Clipboard.SetText(_copiedCellValue);
            }

        }
        void _contextMenu_Opened(object sender, RoutedEventArgs e)
        {
            foreach (IDataGridCustomButton dataGridCustomButton in _customButtons.Values)
            {
                if (dataGridCustomButton.Visible)
                {
                    MenuItem menuItem =
                        _contextMenu.Items.Cast<MenuItem>().Where<MenuItem>(mi => mi.Header.ToString() == dataGridCustomButton.Name).First();
                    if (dataGridCustomButton.IsEnabled)
                    {
                        menuItem.IsEnabled = true;
                        menuItem.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
                        VisualStateManager.GoToState(menuItem, "Normal", true);
                    }
                    else
                    {
                        menuItem.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
                        menuItem.IsEnabled = false;
                        VisualStateManager.GoToState(menuItem, "Disabled", true);
                    }
                }
            }
        }

        public void InitializeScrollableTabControl()
        {
            _tabContainerFrameworkElement = MainPage.GetChildObject<ScrollableTabControl>(_attributesViewerControl, "PART_TabControl");
        }

        public void tabContainerLoaded(object sender, RoutedEventArgs e)
        {
            _tabContainerFrameworkElement = (FrameworkElement)sender;

            InitializeDataGrid(_tabContainerFrameworkElement, true);
            if (_attributeViewerDataGrid != null)
            {
                if (_isFirstTime)
                {
                    _attributeViewerDataGrid.LoadingRow += new EventHandler<DataGridRowEventArgs>(dataGrid_LoadingRow);
                    _attributeViewerDataGrid.Tag = ASSIGNED_TAG;
                    _isFirstTime = false;
                }
                else
                {
                    WireDataGridRowEvents(_attributeViewerDataGrid);
                }
            }
            AttachCloseableTabItemEvent(null);

            // The LayerVisibilityControl is somehow interfering with this grid's events, causing us to have to refresh
            if (this.LayerVisibilityControlIsVisible || _setTabContainerTimer)
            {
                AttachTabContainerTimer();
            }
            _setTabContainerTimer = true;
        }

        void AttachCloseableTabItemEvent(FrameworkElement element)
        {
            StackPanel stackPanel = MainPage.GetChildObject<StackPanel>(_attributesViewerControl, "");
            stackPanel.MouseLeftButtonUp += new MouseButtonEventHandler(stackPanel_MouseLeftButtonUp);
        }
        void stackPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AttachDataGridTimer();
        }

        // This timer is because the AttributeViewer will occasionally (under heavy identify) switch tabs of its own accord
        // to recreate, do an identify with >200 features
        void AttachTabContainerTimer()
        {
            DispatcherTimer tabContainerLoadedTimer = new DispatcherTimer();
            tabContainerLoadedTimer.Interval = new TimeSpan(0, 0, 0, 0, 25000); // Milliseconds 
            tabContainerLoadedTimer.Tick += new EventHandler(tabContainerLoadedTimer_Tick);
            tabContainerLoadedTimer.Start();
        }

        void tabContainerLoadedTimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            _setTabContainerTimer = false;
            InitializeScrollableTabControl();
            tabContainerLoaded(_tabContainerFrameworkElement, null);
        }

        // This timer is in lieu of the DataGrid not telling us when it's finished loading. Fixed in SL5.
        void AttachDataGridTimer()
        {
            DispatcherTimer myDispatcherTimer = new DispatcherTimer();
            myDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1300); // Milliseconds 
            myDispatcherTimer.Tick += new EventHandler(myDispatcherTimer_Tick);
            myDispatcherTimer.Start();
        }

        void myDispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
            InitializeDataGrid(_tabContainerFrameworkElement);
            if (_attributeViewerDataGrid.Tag == null)
            {
                WireDataGridRowEvents(_attributeViewerDataGrid);
            }
        }

        void WireDataGridRowEvents(DataGrid attributeViewerDataGrid, bool wireLeftButtonUp = true)
        {
            attributeViewerDataGrid.Tag = ASSIGNED_TAG;
            foreach (DataGridRow rowItem in attributeViewerDataGrid.GetRows())
            {
                rowItem.MouseRightButtonDown +=
                    new MouseButtonEventHandler(Row_MouseRightButtonDown);
                rowItem.MouseRightButtonUp +=
                    new MouseButtonEventHandler(Row_MouseRightButtonUp);
                if (wireLeftButtonUp)
                {
                    rowItem.MouseLeftButtonUp += new MouseButtonEventHandler(rowItem_MouseLeftButtonUp);
                }
            }
            
        }

        void rowItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_lastMouseLeftButtonUpClick != null && _lastMouseLeftButtonUpClick == e) return;

            _lastMouseLeftButtonUpClick = e;

            // Find out the relationship name if it was selected
            DataGridRow row = sender as DataGridRow;
            AccordionItem accordionItem = GetParentFromVisualTree<AccordionItem>(row);
            string selectedLayer = ((Miner.Server.Client.Tasks.ResultSet) _attributesViewerControl.SelectedItem).Name;
            SetMenuItemVisibilityFromLayer(selectedLayer);
            if (accordionItem != null)
            {
                AccordionButton accordionButton = MainPage.GetChildObject<AccordionButton>(accordionItem,
                    "ExpanderButton");
                TextBlock textBlock = MainPage.GetChildObject<TextBlock>(accordionButton, "");
                selectedLayer = textBlock.Text.Substring(0, textBlock.Text.IndexOf(" ("));
            }
            if (row != null && SubDataGridRowSelected != null)
            {
                SubDataGridRowSelected(_attributesViewerControl.SelectedItem as Miner.Server.Client.Tasks.ResultSet, row.DataContext as Miner.Server.Client.Row, selectedLayer);
            }
        }

        void SetMenuItemVisibilityFromLayer(string layer)
        {
            if (layer == "Transformer")
            {
                _outageHistoryMenuItem.Visibility = Visibility.Visible;
                /*ENOSChange Start*/
                _showGenMenuItem.Visibility = Visibility.Visible;
                /*ENOSChange End*/
            }
            else
            {
                _outageHistoryMenuItem.Visibility = Visibility.Collapsed;
                /*ENOSChange Start*/
                _showGenMenuItem.Visibility = Visibility.Collapsed;
                /*ENOSChange End*/
            }
            //CIT non-identify and grid tool *start
            if (layer.ToUpper() == "PRIMARY UNDERGROUND CONDUCTOR")
            {
                if (_ConductorInTrenchMenuItem != null)
                {
                    _ConductorInTrenchMenuItem.Visibility = Visibility.Visible;
                    /*ENOSChange Start*/
                    //_showGenMenuItem.Visibility = Visibility.Visible;
                }
                /*ENOSChange End*/
            }
            else
            {
                if (_ConductorInTrenchMenuItem != null)
                {
                    _ConductorInTrenchMenuItem.Visibility = Visibility.Collapsed;
                    /*ENOSChange Start*/
                    //_showGenMenuItem.Visibility = Visibility.Collapsed;
                }
                /*ENOSChange End*/
            }
            //CIT non-identify and grid tool *end
            if (ButterflyTool.ButterflyRolloverLayerNames.Contains(layer))
            {
                _butterflyMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                _butterflyMenuItem.Visibility = Visibility.Collapsed;
            }

            /*ENOSChange Start*/
            if (GetGenOnFeeder.GenFeederLayerName.Contains(layer))
            {
                _genOnFeederMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                _genOnFeederMenuItem.Visibility = Visibility.Collapsed;
            }
            /*ENOSChange End*/
        }

        void InitializeDataGrid(FrameworkElement element, bool addCPEventHandler = false)
        {
            if (VisualTreeHelper.GetChildrenCount(element) == 0) return;

            var scrollingGrid = VisualTreeHelper.GetChild(element, 0) as Grid;
            var contentGrid = ((FrameworkElement)VisualTreeHelper.GetChild(scrollingGrid, 5));
            var contentPresenter = ((FrameworkElement)VisualTreeHelper.GetChild(contentGrid, 0));
            if (addCPEventHandler)
            {
                contentPresenter.MouseMove += new MouseEventHandler(contentPresenter_MouseMove);
            }
            if (VisualTreeHelper.GetChildrenCount(contentPresenter) == 0) return;
            var attributeGrid = ((FrameworkElement)VisualTreeHelper.GetChild(contentPresenter, 0));
            var attributeGridGrid = ((FrameworkElement)VisualTreeHelper.GetChild(attributeGrid, 0));
            var mainGrid = ((FrameworkElement)VisualTreeHelper.GetChild(attributeGridGrid, 0));
            _attributeViewerDataGrid = ((FrameworkElement)VisualTreeHelper.GetChild(mainGrid, 0)) as DataGrid;
            _attributeViewerDataGrid.SelectionChanged += new SelectionChangedEventHandler(attributeDataGrid_SelectionChanged);

            if (DataGridBound != null)
            {
                DataGridBound(this, new EventArgs());
            }
            UIElement uiElement = _attributesViewerControl.Parent as UIElement;
            Grid grid = uiElement as Grid;
            grid.SizeChanged += new SizeChangedEventHandler(grid_SizeChanged);

            // Deal with sub
            var qryAllDGDPs = _attributeViewerDataGrid.Descendents().OfType<DataGridDetailsPresenter>();
            foreach (DataGridDetailsPresenter dataGridDetailsPresenter in qryAllDGDPs)
            {
                dataGridDetailsPresenter.SizeChanged += new SizeChangedEventHandler(dataGridDetailsPresenter_SizeChanged);
            }
        }


        void dataGridDetailsPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Children Grids
            DataGridDetailsPresenter dgdp = sender as DataGridDetailsPresenter;
            var qryAllCPs = dgdp.Descendents("ContentSite").OfType<ContentPresenter>();
            foreach (ContentPresenter contentPresenter in qryAllCPs)
            {
                if (VisualTreeHelper.GetChildrenCount(contentPresenter) > 0)
                {
                    // Can't attach to mouse move
                    var attributeGrid = ((FrameworkElement)VisualTreeHelper.GetChild(contentPresenter, 0));
                    var attributeGridGrid = ((FrameworkElement)VisualTreeHelper.GetChild(attributeGrid, 0));
                    var mainGrid = ((FrameworkElement)VisualTreeHelper.GetChild(attributeGridGrid, 0));
                    var attributeDataGrid = ((FrameworkElement)VisualTreeHelper.GetChild(mainGrid, 0)) as DataGrid;
                    if (attributeDataGrid.Tag == null || attributeDataGrid.Tag.ToString() == "")
                    {
                        WireDataGridRowEvents(attributeDataGrid, true);
                    }

                }
            }

        }

        void attributeDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _attributeViewerDataGrid = sender as DataGrid;
        }

        void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender == null) return;

            Grid grid = sender as Grid;

            if (grid.Height > 50)
            {
                _attributeViewerDataGrid.Tag = null;
                AttachDataGridTimer();
            }
        }


        void contentPresenter_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _lastCPMouseMove = e;
        }
        void dataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseRightButtonDown += new MouseButtonEventHandler(Row_MouseRightButtonDown);
            e.Row.MouseRightButtonUp += new MouseButtonEventHandler(Row_MouseRightButtonUp);
        }

        private void Row_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        void Row_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_lastCellClick != null && _lastCellClick == e) return;

            _lastCellClick = e;
            ContextMenuService.SetContextMenu((DependencyObject)sender, _contextMenu);
            // Maybe check to see if the current row is selected
            if (ClickWasOverSelectedRow(e.GetPosition(null)) && ClickWasOverSelectedRow(_lastCPMouseMove.GetPosition(null)))
            {
                DataGridRow row = sender as DataGridRow;
                DataGrid dataGrid = GetParentFromVisualTree<DataGrid>(row);

                UIElement element = (UIElement)dataGrid.Columns[dataGrid.CurrentColumn.DisplayIndex].GetCellContent(row);
                TextBlock txtBlock = MainPage.GetChildObject<TextBlock>(element, "");
                if (txtBlock != null && !String.IsNullOrEmpty(txtBlock.Text))
                {
                    string selectedCellText = txtBlock.Text;
                    _copiedCellValue = selectedCellText;
                }
                else
                {
                    _copiedCellValue = "";
                }

                _contextMenu.IsOpen = true;
            }
        }

        bool ClickWasOverSelectedRow(Point pt)
        {
            if (_attributeViewerDataGrid.SelectedIndex > -1)
            {
                int row;
                int col;
                object dataContext;
                GetGridRowColumnIndex(pt, _attributeViewerDataGrid, out row, out col, out dataContext);
                if (row == _attributeViewerDataGrid.SelectedIndex)
                {
                    return true;
                }
            }

            return false;
        }
        private void GetGridRowColumnIndex(Point pt, DataGrid grid, out int rowIndex, out int colIndex, out object dataContext)
        {
            rowIndex = -1;
            colIndex = -1;
            dataContext = null;
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(pt, grid);
            if (null == elements ||
               elements.Count() == 0)
            {
                return;
            }

            // Get the rows and columns.
            var rowQuery = from gridRow in elements where gridRow is DataGridRow select gridRow as DataGridRow;
            var cellQuery = from gridCell in elements where gridCell is DataGridCell select gridCell as DataGridCell;
            var cells = cellQuery.ToList<DataGridCell>();
            if (cells.Count == 0)
            {
                return;
            }

            foreach (var row in rowQuery)
            {
                dataContext = row.DataContext;
                rowIndex = row.GetIndex();
                foreach (DataGridColumn col in grid.Columns)
                {
                    var colContent = col.GetCellContent(row);
                    var parent = GetParentFromVisualTree<DataGridCell>(colContent);
                    if (parent != null)
                    {
                        var thisCell = (DataGridCell)parent;
                        if (object.ReferenceEquals(thisCell, cells[0]))
                        {
                            colIndex = col.DisplayIndex;
                        }
                    }
                }
            }
        }

        private T GetParentFromVisualTree<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            // Iteratively traverse the visual tree
            while (dependencyObject != null && !(dependencyObject is T))
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);

            if (dependencyObject == null)
                return null;

            return dependencyObject as T;
        }

        //INC000004469254 - Details on Right click----To get print button
        public void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            _rowDetailsButton_Click(sender, e);
        }
    }

    /// <summary>
    /// Extends the DataGrid.
    /// </summary>
    public static class DataGridExtensions
    {
        /// <summary>
        /// Gets the list of DataGridRow objects.
        /// </summary>
        /// <param name="grid">The grid wirhrows.</param>
        /// <returns>List of rows of the grid.</returns>
        public static ICollection<DataGridRow> GetRows(this DataGrid grid)
        {
            List<DataGridRow> rows = new List<DataGridRow>();

            foreach (var rowItem in grid.ItemsSource)
            {
                // Ensures that all rows are loaded.
                //todo: get rid of this?
                //grid.ScrollIntoView(rowItem, grid.Columns.Last());

                // Get the content of the cell.
                FrameworkElement el = grid.Columns.Last().GetCellContent(rowItem);

                // Retrieve the row which is parent of given element.
                if (el != null)
                {
                    DataGridRow row = DataGridRow.GetRowContainingElement(el.Parent as FrameworkElement);

                    // Sometimes some rows for some reason can be null.
                    if (row != null)
                        rows.Add(row);
                }
            }

            return rows;
        }
    }
    public static class VisualTreeEnumeration
    {
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendent in Descendents(child))
                    yield return descendent;
            }
        }

        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is FrameworkElement && ((FrameworkElement) child).Name == name)
                {
                    yield return child;
                }
                foreach (var descendent in Descendents(child))
                {
                    if (descendent is FrameworkElement && ((FrameworkElement) descendent).Name == name)
                    {
                        yield return descendent;
                    }
                }
            }
        }

    } 

}
