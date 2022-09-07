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
////////////////////////17/11/2016 starts //////////////////////////
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
////////////////////////17/11/2016 ends //////////////////////////
/*****************PLC Changes 07/21/2017*********************/
using System.Xml.Linq;
/*****************PLC Changes  End*********************/
namespace ArcFMSilverlight
{
    public partial class WIPAttributeEditor : ChildWindow
    {

        private object _installJobNumber = null;
        private object _mlxNumber = null;
        private object _description = null;
        private object _locationNumber = null;
        private object _estimatorLanId = null;
        private object _dateCreated = null;
        /*****************PLC Changes *********************/
        private object _notificationNo = null;
        private string jobNumberLayerUrl = null;
        string selectedOrderNumber = null;
        private WIPEditorWidget _wipEditorWidgetObj = null;
        private string jobNoChanged = null;
        /*****************PLC Changes  End*********************/

        public WIPAttributeEditor()
        {
            InitializeComponent();
        }

        public WIPEditorWidget WipEditorWidgetObj
        {
            get { return _wipEditorWidgetObj; }
            set { _wipEditorWidgetObj = value; }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
           // string jobNumber = ((Graphic)sender).Attributes["INSTALLJOBNUMBER"].ToString();
            /*****************PLC Changes   07/21/2017*********************/
            selectedOrderNumber = PmorderNumberAutoCompleteTextBlock.Text;

            if (selectedOrderNumber != null)
            {
                if (selectedOrderNumber.Length <= 14)
                {
                    WIPAttributeEditorForm.GraphicSource.Attributes["INSTALLJOBNUMBER"] = selectedOrderNumber;
                }
                else
                {
                    WIPAttributeEditorForm.GraphicSource.Attributes["INSTALLJOBNUMBER"] = null;
                }
            }


            // string jobNumber = ((Graphic)sender).Attributes["INSTALLJOBNUMBER"].ToString();

            SaveWIPAttributes();

            if (_installJobNumber != null && _installJobNumber != "")
            {

                WIPAttributeEditorForm.ApplyChanges();

            }
            else
            {
                if (selectedOrderNumber != null)
                {
                    if (selectedOrderNumber.Length > 14)
                    {
                        MessageBox.Show("PM Order Number Length should be less than 14");
                    }
                    else
                    {
                        MessageBox.Show("Please Select/Enter Job Number");
                    }
                }
                else
                {
                    MessageBox.Show("Please Select/Enter Job Number");
                }

                this.Show();
            }
            /*****************PLC changes for WIP ENDS*********************/

        }
      
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            OKButton.IsEnabled = true;
            this.DialogResult = false;
            /*****************PLC Changes 07/21/2017*********************/
            ResetWIPAttributes();
            /*****************PLC changes for WIP ENDS*********************/
        }

        private void WIPAttributeEditorForm_BindingValidationError(object sender, ValidationErrorEventArgs e)
        {
            switch (e.Action)
            {
                case ValidationErrorEventAction.Added:
                    OKButton.IsEnabled = false;
                    break;
                case ValidationErrorEventAction.Removed:
                    OKButton.IsEnabled = true;
                    break;
                default:
                    break;
            }
        }
       
        public void SaveWIPAttributes()
        {
            
            _installJobNumber = WIPAttributeEditorForm.GraphicSource.Attributes["INSTALLJOBNUMBER"];
            _mlxNumber = WIPAttributeEditorForm.GraphicSource.Attributes["MLXNUMBER"];
            _description = WIPAttributeEditorForm.GraphicSource.Attributes["DESCRIPTION"];
            _locationNumber = WIPAttributeEditorForm.GraphicSource.Attributes["LOCATIONNUMBER"];
            _estimatorLanId = WIPAttributeEditorForm.GraphicSource.Attributes["ESTIMATORLANID"];
            _dateCreated = WIPAttributeEditorForm.GraphicSource.Attributes["DATECREATED"];
            /***********PLC Changes start**********/
            _notificationNo = WIPAttributeEditorForm.GraphicSource.Attributes["NOTIFICATION_NUMBER"];
            /***********PLC Changes end**********/
        }
      
        public void RestoreWIPAttributes()
        {
            WIPAttributeEditorForm.GraphicSource.Attributes["INSTALLJOBNUMBER"] = _installJobNumber;
            WIPAttributeEditorForm.GraphicSource.Attributes["MLXNUMBER"] = _mlxNumber;
            WIPAttributeEditorForm.GraphicSource.Attributes["DESCRIPTION"] = _description;
            WIPAttributeEditorForm.GraphicSource.Attributes["LOCATIONNUMBER"] = _locationNumber;
            WIPAttributeEditorForm.GraphicSource.Attributes["ESTIMATORLANID"] = _estimatorLanId;
            WIPAttributeEditorForm.GraphicSource.Attributes["DATECREATED"] = _dateCreated;
            /***********PLC Changes start**********/
            WIPAttributeEditorForm.GraphicSource.Attributes["NOTIFICATION_NUMBER"]= _notificationNo;
            /***********PLC Changes end**********/
            /*****************PLC Changes  07/21/2017*********************/
            PmorderNumberAutoCompleteTextBlock.Text = "";

            /*****************PLC Changes Ends*********************/
        }

        public void ResetWIPAttributes(bool flush = true)
        {
            /*****************PLC Changes 07/21/2017*********************/
            string lanId = WebContext.Current.User.Name.Replace("PGE\\", "").ToLower().Replace("admin", "");
            PmorderNumberAutoCompleteTextBlock.Text = "";

            /*****************PLC changes Ends*********************/
            if (flush)
            {
                // For some reason the panel is not updating unless you set it to a non-null value first
                WIPAttributeEditorForm.GraphicSource.Attributes["INSTALLJOBNUMBER"] = "";
                WIPAttributeEditorForm.GraphicSource.Attributes["MLXNUMBER"] = "";
                WIPAttributeEditorForm.GraphicSource.Attributes["DESCRIPTION"] = "";
                WIPAttributeEditorForm.GraphicSource.Attributes["LOCATIONNUMBER"] = ""; //Convert.ToInt16(0); ---- INC000004102693, INC000004055302
                WIPAttributeEditorForm.GraphicSource.Attributes["ESTIMATORLANID"] = "";
                WIPAttributeEditorForm.GraphicSource.Attributes["DATECREATED"] = DateTime.Now;
                /*****************PLC Changes *********************/
                WIPAttributeEditorForm.GraphicSource.Attributes["NOTIFICATION_NUMBER"] = "";
                /*****************PLC Changes  End*********************/
            }
            WIPAttributeEditorForm.GraphicSource.Attributes["INSTALLJOBNUMBER"] = null;
            WIPAttributeEditorForm.GraphicSource.Attributes["MLXNUMBER"] = null;
            WIPAttributeEditorForm.GraphicSource.Attributes["DESCRIPTION"] = null;
            WIPAttributeEditorForm.GraphicSource.Attributes["LOCATIONNUMBER"] = null;
            /*****************PLC Changes  *********************/
            DateTime currentDate = DateTime.Today;
            WIPAttributeEditorForm.GraphicSource.Attributes["ESTIMATORLANID"] = lanId.ToUpper();
            WIPAttributeEditorForm.GraphicSource.Attributes["NOTIFICATION_NUMBER"] = null;
            WIPAttributeEditorForm.GraphicSource.Attributes["DATECREATED"] = currentDate; 
            /*****************PLC changes Ends*********************/

            _installJobNumber = null;
            _mlxNumber = null;
            _description = null;
            _locationNumber = null;
            /*****************PLC Changes *********************/
            _notificationNo = null;
            _estimatorLanId = lanId.ToUpper();
            _dateCreated = currentDate; 
            /*****************PLC changes Ends*********************/

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetWIPAttributes();
        }

        private void ChildWindow_Closed(object sender, EventArgs e)
        {

        }
        /*****************PLC Changes  RK 07/21/2017*********************/
        private void PmorderNumberAutoCompleteTextBlock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_wipEditorWidgetObj.IsAttributeEditor)
            {
                if (jobNoChanged == null || jobNoChanged != PmorderNumberAutoCompleteTextBlock.Text)
                {
                    jobNoChanged = PmorderNumberAutoCompleteTextBlock.Text;
                    populateWipAttribute(PmorderNumberAutoCompleteTextBlock.Text);
                }

            }
            else
            {
                jobNoChanged = PmorderNumberAutoCompleteTextBlock.Text;
            }
        }

        private void populateWipAttribute(string jobnumber)
        {
            getLayerUrl();

            Query _jobNumerQuery = new Query();
            _jobNumerQuery.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
            _jobNumerQuery.ReturnGeometry = false;

            _jobNumerQuery.OutFields.AddRange(new string[] { "*" });
            _jobNumerQuery.Where = "INSTALLJOBNUMBER='" + jobnumber + "'";

            QueryTask _jobNumberqueryTask = new QueryTask(jobNumberLayerUrl);
            _jobNumberqueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(_populateJobNoqueryTask_ExecuteCompleted);
            _jobNumberqueryTask.Failed += new EventHandler<TaskFailedEventArgs>(_populateJobNumberqueryTask_Failed);
            _jobNumberqueryTask.ExecuteAsync(_jobNumerQuery);
        }

        void _populateJobNoqueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {

            if (e.FeatureSet.Features.Count > 0)
            {
                var feature = e.FeatureSet.Features[0];


                WIPAttributeEditorForm.GraphicSource.Attributes["DESCRIPTION"] = feature.Attributes["JOBDESCRIPTION"];

            }
            else
            {
                WIPAttributeEditorForm.GraphicSource.Attributes["DESCRIPTION"] = "";
            }
        }

        void _populateJobNumberqueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            //throw new NotImplementedException();

        }


        private void getLayerUrl()
        {
            if (Application.Current.Host.InitParams.ContainsKey("Config"))
            {
                string config = Application.Current.Host.InitParams["Config"];
                //var attribute = "";
                config = System.Windows.Browser.HttpUtility.HtmlDecode(config).Replace("***", ",");
                XElement elements = XElement.Parse(config);
                foreach (XElement element in elements.Elements())
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
            }
        }



        private void WipChildWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.RootVisual.SetValue(Control.IsEnabledProperty, true);
        }

        /*****************PLC changes Ends*********************/
    }
}

