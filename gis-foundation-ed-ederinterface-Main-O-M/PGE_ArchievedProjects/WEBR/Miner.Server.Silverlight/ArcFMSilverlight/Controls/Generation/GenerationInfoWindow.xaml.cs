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
using System.Collections.ObjectModel;
using ESRI.ArcGIS.Client;
using System.Windows.Browser;
//ENOS2EDGIS
namespace ArcFMSilverlight.Controls.Generation
{
    public partial class GenerationInfoWindow : ChildWindow
    {
        //GraphicCollection generationGraphic;
        ObservableCollection<GenerationData> _generationInfoData = new ObservableCollection<GenerationData>();
        public GenerationInfoWindow()
        {
            InitializeComponent();                       
            //generationGraphic = this.LayoutRoot.Resources["MyGenerationCollection"] as GraphicCollection;
        }
        
        string BuildSettingsURL(string globalID, string layerName)
        {
            string url = DeviceSettingURL + "?";
            url += "globalID=" + HttpUtility.UrlEncode(globalID) + "&";
            url += "layerName=" + HttpUtility.UrlEncode(layerName) + "&";

            url = url.TrimEnd('&');

            return url;
        }


        public ObservableCollection<GenerationData> GenerationInfoData
        {
            get;
            set;
        }

        public void InitializeViewModel(ObservableCollection<Graphic> generations)
        {
            TxtHeader.Text = "Generators related to Service Location OID: " + ServiceLocationOID;
            if (generations.Count > 0)
            {
                _generationInfoData.Clear();
                foreach (Graphic generationData in generations)
                {
                    //********ENOS2EDGIS Start**********
                    string address = generationData.Attributes["STREETNUMBER"].ToString() + "," + generationData.Attributes["STREETNAME1"].ToString() + "," + generationData.Attributes["STATE"].ToString();
                    //********ENOS2EDGIS End**********
                    _generationInfoData.Add(new GenerationData()
                    {
                        ObjectID = generationData.Attributes["OBJECTID"] != null ? generationData.Attributes["OBJECTID"].ToString():string.Empty,
                        GlobalID = generationData.Attributes["GLOBALID"] != null ? generationData.Attributes["GLOBALID"].ToString():string.Empty,
                       // GenCategory = generationData.Attributes["GENTYPE"] != null ? generationData.Attributes["GENTYPE"].ToString() : string.Empty,
                        //ENOS2EDGIS, showing Gen Type Domain values 
                       GenCategory = generationData.Attributes["GENTYPE"] != null ? ConfigUtility.DivisionCodedDomains.CodedValues[generationData.Attributes["GENTYPE"].ToString()] : string.Empty,
                        ProjectName = generationData.Attributes["PROJECTNAME"] != null ? generationData.Attributes["PROJECTNAME"].ToString() : string.Empty,
                        //********ENOS2EDGIS Start**********
                        SPID = generationData.Attributes["SPID"] != null ? generationData.Attributes["SPID"].ToString() : string.Empty,
                        Address = address
                        //********ENOS2EDGIS End**********
                    });
                
                    //generationGraphic.Add(generationData);
                    //string url = BuildSettingsURL(generationData.Attributes["GLOBALID"].ToString(), "Generation");
                    //HyperlinkButton btn = new HyperlinkButton();
                    //btn.NavigateUri = new Uri(url);
                    //btn.TargetName = "_blank";
                    //btn.Content = "Generation OID : " + generationData.Attributes["OBJECTID"].ToString();
                    //LinksStackPanel.Children.Add(btn);
                }
                generationGrid.ItemsSource = _generationInfoData;
            }
            this.Width = generationGrid.Width + 20;
            //ENOS2EDGIS, showing Gen Type Domain values 
            ConfigUtility.DivisionCodedDomains = null;
        }

        //******************* “Changes made for Service Location Grid More Info 16/05/2017” **************************************
        public void InitializeViewModel(IList<Graphic> generations)
        {
            TxtHeader.Text = "Generators related to Service Location OID: " + ServiceLocationOID;
            if (generations.Count > 0)
            {
                _generationInfoData.Clear();
                foreach (Graphic generationData in generations)
                {
                    string address = generationData.Attributes["STREETNUMBER"].ToString() + "," + generationData.Attributes["STREETNAME1"].ToString() + "," + generationData.Attributes["STATE"].ToString();
                    _generationInfoData.Add(new GenerationData()
                    {
                        ObjectID = generationData.Attributes["OBJECTID"] != null ? generationData.Attributes["OBJECTID"].ToString() : string.Empty,
                        GlobalID = generationData.Attributes["GLOBALID"] != null ? generationData.Attributes["GLOBALID"].ToString() : string.Empty,
                        //GenCategory = generationData.Attributes["GENTYPE"] != null ? generationData.Attributes["GENTYPE"].ToString() : string.Empty,
                        //ENOS2EDGIS, showing Gen Type Domain values 
                        GenCategory = generationData.Attributes["GENTYPE"] != null ? ConfigUtility.DivisionCodedDomains.CodedValues[generationData.Attributes["GENTYPE"].ToString()] : string.Empty,
                        ProjectName = generationData.Attributes["PROJECTNAME"] != null ? generationData.Attributes["PROJECTNAME"].ToString() : string.Empty,
                        SPID = generationData.Attributes["SPID"] != null ? generationData.Attributes["SPID"].ToString() : string.Empty,
                        Address = address
                    });                    
                }
                generationGrid.ItemsSource = _generationInfoData;
            }
            this.Width = generationGrid.Width + 200;
            //ENOS2EDGIS, showing Gen Type Domain values 
            ConfigUtility.DivisionCodedDomains = null;
        }
        //*****************************************************************************

        private void Generation_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //IEnumerable<UIElement> elementsUnderMouse =
            //    VisualTreeHelper
            //        .FindElementsInHostCoordinates(e.GetPosition(null), this);
            //DataGridRow row =
            //    elementsUnderMouse
            //        .Where(uie => uie is DataGridRow)
            //        .Cast<DataGridRow>()
            //        .FirstOrDefault();
            //if (row != null)
            //    generationGrid.SelectedItem = row.DataContext;
            if ((sender) as DataGridRow != null)
            {
                generationGrid.SelectedItem = ((sender) as DataGridRow).DataContext;
            }
        }

        private void Generation_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid)
            {
                GenerationData selectedData = ((DataGrid)sender).SelectedItem != null ? ((DataGrid)sender).SelectedItem as GenerationData : null;
                if (selectedData != null)
                {
                  MessageBoxResult messageResult = MessageBox.Show("Do you want to proceed to open SETTINGS UI for the clicked generation ?", "Want to open SETTINGS APP?", MessageBoxButton.OKCancel);
                  if (messageResult == MessageBoxResult.OK)
                  {
                      OpenSettings(selectedData.GlobalID, "Generation");
                  }
                  else
                  {
                      generationGrid.Focus();
                  }
                }
            }
            ///get the clicked row
            /////VisualTreeHelper.GetChildrenCount(parent);
            //DataGridRow row = VisualTreeHelper.GetChild(e.OriginalSource as DependencyObject,0) as DataGridRow;
            //var obj = e.OriginalSource as DependencyObject;
            //if (obj == null) return;

            //var rowTest = FindParentOfType(obj as DataGridRow);
            //if (row == null) return;

            ////DataGridRow row = (e.OriginalSource as DependencyObject).FindParentOfType<DataGridRow>() as DataGridRow;
            /////get the data object of the row
            //if (row != null && row.DataContext is GenerationData) 
            //{

            //}

        }

        public void OpenSettings(string globalID, string layerName)
        {
            HtmlPage.Window.Navigate(new Uri(BuildSettingsURL(globalID, layerName)), "_blank", "scrollbars=yes,toolbar=no,location=no,status=no,menubar=no,resizable=yes,width=1000px,height=600px");
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = null;
            if (generationGrid.SelectedItem != null)
            {
                GenerationData selectedData = generationGrid.SelectedItem != null ? generationGrid.SelectedItem as GenerationData : null;
                if (selectedData != null)
                {
                    OpenSettings(selectedData.GlobalID, "EDGIS.GENERATIONINFO");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        public string ServiceLocationOID
        {
                get;set;
        }

        public string DeviceSettingURL
        {
            get;
            set;
        }

        public static DependencyObject FindParentOfType(DependencyObject element)
        {
            while (true)
            {
                if (element != null)
                {
                    element = VisualTreeHelper.GetParent(element);
                    if (element == null) return null;
                    if (element is DependencyObject)
                    {
                        return (DependencyObject)element;
                    }
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (generationGrid.SelectedItem != null)
            {
                GenerationData selectedData = generationGrid.SelectedItem != null ? generationGrid.SelectedItem as GenerationData : null;
                if (selectedData != null)
                {
                    OpenSettings(selectedData.GlobalID, "EDGIS.GENERATIONINFO");
                }
            }
        }

        private void generationGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseRightButtonDown += new MouseButtonEventHandler(Generation_MouseRightButtonUp);
        }
    }  
}

