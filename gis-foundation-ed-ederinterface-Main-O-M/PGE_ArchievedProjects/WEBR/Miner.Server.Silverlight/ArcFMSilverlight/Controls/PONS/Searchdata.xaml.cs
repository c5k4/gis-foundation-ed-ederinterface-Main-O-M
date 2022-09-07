
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Bing.RouteService;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Toolkit.DataSources;
using ESRI.ArcGIS.Client.WebMap;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.FeatureService.Symbols;
using System.Collections;
using System.Threading;

namespace ArcFMSilverlight
{
    public partial class Searchdata : ChildWindow
    {

        public string Value { get; set; }
        public string GetSubstationid { get; set; }
        public string GetcircuitId { get; set; }
        public string SearchType { get; set; }
        public int CValue { get; set; }
        public int Rdtype { get; set; }
        string SetDeviceID = string.Empty;
        string ChOPERATINGNUMBER = string.Empty;
        public string GetChOPERATINGNUMBER { get; set; }
        public string GetChEndOPERATINGNUMBER { get; set; }
        public int SD_Count { get; set; }
        public int ED_Count { get; set; }
        int ChDivisionnumber;
        public int GetChDivisionnumber { get; set; }
        long strCGC;
        //Start Device
        string setstartoprationnumber = string.Empty;
        string setstartGUID = string.Empty;
        string setstartFeederID = string.Empty;
        string StartDeviceType = string.Empty;
        string setStartcircuitID2 = string.Empty;
        //End Device
        string setendoprationnumber = string.Empty;
        string setendGUID = string.Empty;
        string setendFeederID = string.Empty;
        string setendcircuitID2 = string.Empty;
        string EndDeviceType = string.Empty;
        //Circuit

        string setCircuitID = string.Empty;
        string rtype = string.Empty;
        string strFeederId = string.Empty;
        //Page.config
        public string _TransformerService = string.Empty;
        public string _deviceType = string.Empty;
        private string _SubstationService = string.Empty;
        private string _CircuitService = string.Empty;
        List<clsdevicegridbind> selectedList = new List<clsdevicegridbind>();

        //Service1Client Ponsservice = new Service1Client();

        public Searchdata()
        {
            InitializeComponent();
            LoadLayerConfiguration();
            this.Loaded += new RoutedEventHandler(SearchWindow_Loaded);


        }

        void SearchWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rtype = this.Rdtype.ToString();
            if (rtype == "1")
            {
                busyIndicator_SD.IsBusy = true;
                this.ChOPERATINGNUMBER = this.GetChOPERATINGNUMBER.ToString();
                this.GetChEndOPERATINGNUMBER = this.GetChEndOPERATINGNUMBER.ToString();
                this.SD_Count = Convert.ToInt32(this.SD_Count.ToString());
                this.ED_Count = Convert.ToInt32(this.ED_Count.ToString());

                this.GetChDivisionnumber = Convert.ToInt32(this.GetChDivisionnumber.ToString());
                SearchlistDeviceDataGrid.Visibility = Visibility.Visible;
                SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                SearchlistDataGrid.Visibility = Visibility.Collapsed;
                SearchlistDataGrid.ItemsSource = null;
                SearchlistEndDeviceDataGrid.ItemsSource = null;
                SearchlistCircuitDataGrid.ItemsSource = null;
                SearchlistDeviceDataGrid.ItemsSource = null;

                DevicesearchData();

            }
            if (rtype == "2")
            {

                this.SetDeviceID = this.Value.ToString();
                SearchlistCircuitDataGrid.Visibility = Visibility.Collapsed;
                SearchlistDeviceDataGrid.Visibility = Visibility.Collapsed;
                SearchlistDataGrid.Visibility = Visibility.Visible;
                SearchlistDataGrid.ItemsSource = null;
                BindTransformersearchdata();
            }
            if (rtype == "3")
            {
                this.SetDeviceID = this.GetSubstationid.ToString();
                SearchlistDataGrid.Visibility = Visibility.Collapsed;
                SearchlistDeviceDataGrid.Visibility = Visibility.Collapsed;
                SearchlistCircuitDataGrid.Visibility = Visibility.Visible;
                SearchlistCircuitDataGrid.ItemsSource = null;

                BindSubstationsearchdata();
            }

        }
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {


            this.DialogResult = true;

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        //LOad from Page.config
        private void LoadLayerConfiguration()
        {

            try
            {
                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    var attribute = "";
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement elements = XElement.Parse(config);
                    foreach (XElement element in elements.Elements())
                    {
                        if (element.Name.LocalName == "PONSInformation")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement childelement in element.Elements())
                                {
                                    if (childelement.Name.LocalName == "TransformerService")
                                    {
                                        attribute = childelement.Attribute("Name").Value;
                                        if (attribute != null)
                                        {
                                            _TransformerService = attribute;
                                        }
                                    }

                                    if (childelement.Name.LocalName == "SubstationService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _SubstationService = attribute;
                                        }
                                    }
                                    if (childelement.Name.LocalName == "CircuitService")
                                    {
                                        attribute = childelement.Attribute("url").Value;
                                        if (attribute != null)
                                        {
                                            _CircuitService = attribute;
                                        }
                                    }

                                }

                            }

                        }
                    }
                }

            }
            catch
            {
            }
        }
        # region Property
        public string DeviceID
        {

            get { return strFeederId; }
        }
        public string Rdsearchtype
        {
            get { return SearchType; }
        }
        public long TransID
        {
            get { return strCGC; }
        }
        public string CircuitID
        {

            get { return setCircuitID; }
        }
        //Start Device
        public string SetStartOpno
        {
            get { return setstartoprationnumber; }
        }
        public string Setstartguid
        {
            get { return setstartGUID; }
        }
        public string Setstartfeeder
        {
            get { return setstartFeederID; }
        }
        public string Setstartdevicetype
        {
            get { return StartDeviceType; }
        }
        public string SetStartcircuitID2
        {
            get { return setStartcircuitID2; }
        }
        
        //End Device

        public string SetEndOpno
        {
            get { return setendoprationnumber; }
        }
        public string Setendguid
        {
            get { return setendGUID; }
        }
        public string Setendfeeder
        {
            get { return setendFeederID; }
        }
        public string SetendcircuitID2
        {
            get { return setendcircuitID2; }
        }
        
        public string Setenddevicetype
        {
            get { return EndDeviceType; }
        }

        # endregion
        //Search Data
        private void AdddataButton_Click(object sender, RoutedEventArgs e)
        {

           
        }
        #region Device
        void DevicesearchData()
        {
            Ponsservicelayercall objPonsServiceCall = new Ponsservicelayercall();
            //string strDeviceSearchURL = objPonsServiceCall.GetDevicestartcusdataURL(GetChDivisionnumber);
            //GetDevicestartcusdataSearch1(strDeviceSearchURL);
            string strDeviceSURL = objPonsServiceCall.GetDevicedataCustomerSearchURL(ChOPERATINGNUMBER, GetChEndOPERATINGNUMBER, GetChDivisionnumber);
            GetDevicestartcusdataSearch1(strDeviceSURL);
        }
        public void GetDevicestartcusdataSearch1(string strURL)
        {
            try
            {
                WebClient client = new WebClient();
                UriBuilder uriBuilder = new UriBuilder(strURL);
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(Client_Devicestartcusdata);
                client.DownloadStringAsync(uriBuilder.Uri);
            }
            catch (Exception ex)
            {
                
            }
        }

        //start device selection

        private void radioDeviceSelectdevice_Checked(object sender, RoutedEventArgs e)
        {
            ArrayList objList = new ArrayList();

            RadioButton r = SearchlistDeviceDataGrid.Columns[0].GetCellContent(this.SearchlistDeviceDataGrid.SelectedItem) as RadioButton;

            if (r.IsChecked == true)
            {


                selectedList.Clear();
                if (this.SearchlistDeviceDataGrid.SelectedItem is clsdevicegridbind)
                {
                    selectedList.Add(((clsdevicegridbind)this.SearchlistDeviceDataGrid.SelectedItem));
                }
                ArrayList objList1 = new ArrayList();

                foreach (var item in selectedList)
                {
                    //objList1.Add(item);

                    StartDeviceType = item.DeviceName;
                    setstartFeederID = item.CircuitID;
                    setStartcircuitID2 = item.CircuitID2;
                    setstartoprationnumber = item.OPERATINGNUMBER;
                    setstartGUID = item.GUID;
                }
                SearchType = "1";
                if (ED_Count == 1)
                {
                    if (setstartFeederID == setendFeederID || setStartcircuitID2 == setendFeederID || setstartFeederID == setendcircuitID2)
                    {

                        SearchType = "1";
                        OKButton.Visibility = Visibility.Visible;
                        AdddataButton.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        MessageBox.Show("Select same circuit ID (End Device circuit ID is '"+ setendFeederID+"') in both device or Close the  window");
                    }
                }
                else if (ED_Count > 1)
                {

                    SearchlistDeviceDataGrid.Visibility = Visibility.Collapsed;
                    SearchlistEndDeviceDataGrid.Visibility = Visibility.Visible;
                   // MessageBox.Show("Please select Start and End device on same feeder");
                }
                else
                {
                    OKButton.Visibility = Visibility.Visible;
                    AdddataButton.Visibility = Visibility.Collapsed;
                }



            }
            else
            {
                MessageBox.Show("Select any row from table");
            }
        }
        void Client_Devicestartcusdata(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                List<Devicestartcustomerdata> objSearchResult = new List<Devicestartcustomerdata>();
                string result = e.Result;
                objSearchResult = JsonHelper.Deserialize<List<Devicestartcustomerdata>>(result);
                int Enddevicecount = 0;
                int startdevicecount = 0;
                var Deviceinfo = objSearchResult;
                if (GetChOPERATINGNUMBER.ToString() != "")
                {
                    var startdevicequery_where1 = from a in Deviceinfo

                                                  where a.OPERATINGNUMBER.Equals(GetChOPERATINGNUMBER.ToString())
                                                  select new
                                                  {
                                                      a.DeviceName,
                                                      a.OPERATINGNUMBER,
                                                      a.circuitid,
                                                      a.circuitid2,
                                                      a.globalid
                                                  };

                    var Sd = startdevicequery_where1.ToList();

                    startdevicecount = startdevicequery_where1.Count();
                }
                if (GetChEndOPERATINGNUMBER.ToString() != "")
                {
                    var Enddevicequery_where1 = from a in Deviceinfo
                                                where a.OPERATINGNUMBER.Equals(GetChEndOPERATINGNUMBER.ToString())
                                                select a;
                    Enddevicecount = Enddevicequery_where1.Count();
                }
                if (GetChEndOPERATINGNUMBER.ToString() == "")
                {
                    if (startdevicecount > 1)
                    {

                        var startdevicequery_where1 = from a in Deviceinfo

                                                      where a.OPERATINGNUMBER.Equals(GetChOPERATINGNUMBER.ToString())
                                                      select new
                                                      {
                                                          a.DeviceName,
                                                          a.OPERATINGNUMBER,
                                                          a.circuitid,
                                                          a.circuitid2,
                                                          a.globalid,
                                                          a.objectid
                                                      };

                        var Sd = startdevicequery_where1.ToList();


                        List<clsdevicegridbind> startdeviceclass = new List<clsdevicegridbind>();
                        for (int i = 0; i < Sd.Count; i++)
                        {


                            startdeviceclass.Add(new clsdevicegridbind()
                            {

                                DeviceName = Sd.ElementAt(i).DeviceName,
                                OPERATINGNUMBER = Sd.ElementAt(i).OPERATINGNUMBER,
                                CircuitID = Sd.ElementAt(i).circuitid,
                                CircuitID2 = Sd.ElementAt(i).circuitid2,
                                GUID = Sd.ElementAt(i).globalid,
                                OBJECTID = Sd.ElementAt(i).objectid

                            });
                        }
                        SearchlistDeviceDataGrid.Visibility = Visibility.Visible;
                        SearchlistEndDeviceDataGrid.Visibility = Visibility.Collapsed;
                        ThreadPool.QueueUserWorkItem((state) =>
                        {
                            Thread.Sleep(5 * 1000);
                            SearchlistDeviceDataGrid.Dispatcher.BeginInvoke(() => SearchlistDeviceDataGrid.ItemsSource = startdeviceclass);
                            Dispatcher.BeginInvoke(() => busyIndicator_SD.IsBusy = false);
                        });
                        // SearchlistDeviceDataGrid.ItemsSource = startdeviceclass;

                    }
                }

                    //More than one start device
                else if (startdevicecount > 1 && Enddevicecount == 1)
                {
                    var startdevicequery_where1 = from a in Deviceinfo

                                                  where a.OPERATINGNUMBER.Equals(GetChOPERATINGNUMBER.ToString())
                                                  select new
                                                  {
                                                      a.DeviceName,
                                                      a.OPERATINGNUMBER,
                                                      a.circuitid,
                                                      a.circuitid2,
                                                      a.globalid,
                                                      a.objectid
                                                  };

                    var Sd = startdevicequery_where1.ToList();


                    List<clsdevicegridbind> startdeviceclass = new List<clsdevicegridbind>();
                    for (int i = 0; i < Sd.Count; i++)
                    {


                        startdeviceclass.Add(new clsdevicegridbind()
                        {

                            DeviceName = Sd.ElementAt(i).DeviceName,
                            OPERATINGNUMBER = Sd.ElementAt(i).OPERATINGNUMBER,
                            CircuitID = Sd.ElementAt(i).circuitid,
                            CircuitID2=Sd.ElementAt(i).circuitid2,
                            GUID = Sd.ElementAt(i).globalid,
                            OBJECTID =Sd.ElementAt(i).objectid

                        });
                    }


                    var Enddevicequery_where1 = from a in Deviceinfo
                                                where a.OPERATINGNUMBER.Equals(GetChEndOPERATINGNUMBER.ToString())
                                                select a;
                    EndDeviceType = Enddevicequery_where1.ElementAt(0).DeviceName;
                    setendFeederID = Enddevicequery_where1.ElementAt(0).circuitid;
                    setendcircuitID2 = Enddevicequery_where1.ElementAt(0).circuitid2;
                    setendoprationnumber = Enddevicequery_where1.ElementAt(0).OPERATINGNUMBER;
                    setendGUID = Enddevicequery_where1.ElementAt(0).globalid;

                    SearchlistDeviceDataGrid.ItemsSource = null;
                    SearchlistEndDeviceDataGrid.ItemsSource = null;
                    SearchlistDeviceDataGrid.Visibility = Visibility.Visible;
                    SearchlistEndDeviceDataGrid.Visibility = Visibility.Collapsed;
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        Thread.Sleep(5 * 1000);
                        SearchlistDeviceDataGrid.Dispatcher.BeginInvoke(() => SearchlistDeviceDataGrid.ItemsSource = startdeviceclass);
                        Dispatcher.BeginInvoke(() => busyIndicator_SD.IsBusy = false);
                    });
                }
                // More than one End device
                else if (startdevicecount == 1 && Enddevicecount > 1)
                {
                    busyIndicator_SD.IsBusy = false;
                    var Enddevicequery_where = from a in Deviceinfo

                                               where a.OPERATINGNUMBER.Equals(GetChEndOPERATINGNUMBER.ToString())
                                               select new
                                               {
                                                   a.DeviceName,
                                                   a.OPERATINGNUMBER,
                                                   a.circuitid,
                                                   a.circuitid2,
                                                   a.globalid,
                                                   a.objectid
                                               };

                    var Enddevice = Enddevicequery_where.ToList();

                    //int devicecount = startdevicequery_where1.Count();



                    List<clsdevicegridbind> Enddeviceclass = new List<clsdevicegridbind>();
                    for (int i = 0; i < Enddevice.Count; i++)
                    {


                        Enddeviceclass.Add(new clsdevicegridbind()
                        {

                            DeviceName = Enddevice.ElementAt(i).DeviceName,
                            OPERATINGNUMBER = Enddevice.ElementAt(i).OPERATINGNUMBER,
                            CircuitID = Enddevice.ElementAt(i).circuitid,
                            CircuitID2 = Enddevice.ElementAt(i).circuitid2,
                            GUID = Enddevice.ElementAt(i).globalid,
                            OBJECTID = Enddevice.ElementAt(i).objectid

                        });
                    }

                    var startdevicequery_where1 = from a in Deviceinfo
                                                  where a.OPERATINGNUMBER.Equals(GetChOPERATINGNUMBER.ToString())
                                                  select a;
                    StartDeviceType = startdevicequery_where1.ElementAt(0).DeviceName;
                    setstartFeederID = startdevicequery_where1.ElementAt(0).circuitid;
                    setstartoprationnumber = startdevicequery_where1.ElementAt(0).OPERATINGNUMBER;
                    setstartGUID = startdevicequery_where1.ElementAt(0).globalid;
                    setStartcircuitID2 = startdevicequery_where1.ElementAt(0).circuitid2;

                    SearchlistDeviceDataGrid.ItemsSource = null;
                    SearchlistEndDeviceDataGrid.ItemsSource = null;
                    SearchlistDeviceDataGrid.Visibility = Visibility.Collapsed;
                    SearchlistEndDeviceDataGrid.Visibility = Visibility.Visible;



                    SearchlistEndDeviceDataGrid.ItemsSource = Enddeviceclass;

                }
                //More than one device from devicelist
                else if (startdevicecount > 1 && Enddevicecount > 1)
                {
                    busyIndicator_SD.IsBusy = false;
                    var Enddevicequery_where = from a in Deviceinfo

                                               where a.OPERATINGNUMBER.Equals(GetChEndOPERATINGNUMBER.ToString())
                                               select new
                                               {
                                                   a.DeviceName,
                                                   a.OPERATINGNUMBER,
                                                   a.circuitid,
                                                   a.circuitid2,
                                                   a.globalid,
                                                   a.objectid
                                               };

                    var Enddevice = Enddevicequery_where.ToList();

                    //int devicecount = startdevicequery_where1.Count();



                    List<clsdevicegridbind> Enddeviceclass = new List<clsdevicegridbind>();
                    for (int i = 0; i < Enddevice.Count; i++)
                    {


                        Enddeviceclass.Add(new clsdevicegridbind()
                        {

                            DeviceName = Enddevice.ElementAt(i).DeviceName,
                            OPERATINGNUMBER = Enddevice.ElementAt(i).OPERATINGNUMBER,
                            CircuitID = Enddevice.ElementAt(i).circuitid,
                            CircuitID2 = Enddevice.ElementAt(i).circuitid2,
                            GUID = Enddevice.ElementAt(i).globalid,
                            OBJECTID=Enddevice.ElementAt(i).objectid

                        });
                    }
                    SearchlistDeviceDataGrid.ItemsSource = null;
                    SearchlistEndDeviceDataGrid.ItemsSource = null;
                    SearchlistEndDeviceDataGrid.ItemsSource = Enddeviceclass;
                    var startdevicequery_where1 = from a in Deviceinfo

                                                  where a.OPERATINGNUMBER.Equals(GetChOPERATINGNUMBER.ToString())
                                                  select new
                                                  {
                                                      a.DeviceName,
                                                      a.OPERATINGNUMBER,
                                                      a.circuitid,
                                                      a.circuitid2,
                                                      a.globalid,
                                                      a.objectid
                                                  };

                    var Sd = startdevicequery_where1.ToList();


                    List<clsdevicegridbind> startdeviceclass = new List<clsdevicegridbind>();
                    for (int i = 0; i < Sd.Count; i++)
                    {


                        startdeviceclass.Add(new clsdevicegridbind()
                        {

                            DeviceName = Sd.ElementAt(i).DeviceName,
                            OPERATINGNUMBER = Sd.ElementAt(i).OPERATINGNUMBER,
                            CircuitID = Sd.ElementAt(i).circuitid,
                            CircuitID2 = Sd.ElementAt(i).circuitid2,
                            GUID = Sd.ElementAt(i).globalid,
                            OBJECTID = Sd.ElementAt(i).objectid

                        });
                    }


                    SearchlistDeviceDataGrid.Visibility = Visibility.Visible;
                    SearchlistEndDeviceDataGrid.Visibility = Visibility.Collapsed;
                    ThreadPool.QueueUserWorkItem((state) =>
                    {
                        Thread.Sleep(5 * 1000);
                        SearchlistDeviceDataGrid.Dispatcher.BeginInvoke(() => SearchlistDeviceDataGrid.ItemsSource = startdeviceclass);
                        Dispatcher.BeginInvoke(() => busyIndicator_SD.IsBusy = false);
                    });

                }


            }
            catch
            {
                //busyIndicator_ED.IsBusy = false;
                busyIndicator_SD.IsBusy = false;
            }


        }

        #endregion
        #region Transformer

        void BindTransformersearchdata()
        {
            clsDevicename dd = new clsDevicename();
            QueryTask TransqueryTask = new QueryTask(_TransformerService);
            TransqueryTask.ExecuteCompleted += Transquery_ExecuteCompleted;
            TransqueryTask.Failed += TransqueryTask_Failed;
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            string par = this.SetDeviceID;
            dd.Devicename = "PrimaryMet";

            query.Where = "OPERATINGNUMBER = '" + par.Trim().ToUpper() + "' AND DIVISION = " + this.GetChDivisionnumber ;    //string.Format("OBJECTID ='{0}'", txtTransformer.Text);
            query.OutFields.Add("CGC12,CIRCUITID");
            //query.OutFields.Add("*");
            TransqueryTask.ExecuteAsync(query);
        }

        private void Transquery_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;

            if (featureSet != null && featureSet.Features.Count > 0)
            {
                for (int iCount = 0; iCount < featureSet.Features.Count; iCount++)
                {
                    featureSet.Features[iCount].Attributes.Add("DEVICETYPE", _deviceType); 
                }
                SearchlistDataGrid.ItemsSource = featureSet.Features;
            }
            else
            {
                MessageBox.Show("No Transformer returned from query");

            }
        }
        private void TransqueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }

        private void radioSelectdevice_Checked(object sender, RoutedEventArgs e)
        {
            ArrayList objList = new ArrayList();

            RadioButton r = SearchlistDataGrid.Columns[0].GetCellContent(this.SearchlistDataGrid.SelectedItem) as RadioButton;

            if (r.IsChecked == true)
            {
                IDictionary<string, object> rddata = ((ESRI.ArcGIS.Client.Graphic)(SearchlistDataGrid.Columns[1].GetCellContent(this.SearchlistDataGrid.SelectedItem).DataContext)).Attributes;

                foreach (object value in rddata.Values)
                {
                    objList.Add(value);
                }

                strCGC = Convert.ToInt64(objList[0].ToString());
                strFeederId = objList[1] as string;

                SearchType = "2";
                OKButton.Visibility = Visibility.Visible;
                AdddataButton.Visibility = Visibility.Collapsed;


            }
            else
            {
                MessageBox.Show("Select any row from table");
            }
        }
        #endregion
        #region Substation-Circuit

        void BindSubstationsearchdata()
        {
            clsDevicename dd = new clsDevicename();
            QueryTask SubstationqueryTask = new QueryTask(_CircuitService);
            SubstationqueryTask.ExecuteCompleted += SubstationqueryTask_ExecuteCompleted;
            SubstationqueryTask.Failed += SubstationqueryTask_Failed;
            ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            string par = this.SetDeviceID;
            dd.Devicename = "Circuit";

            query.Where = "SUBSTATIONNAME  ='" + par + "'";    //string.Format("OBJECTID ='{0}'", txtTransformer.Text);
            query.OutFields.Add("circuitid,circuitid");
            //query.OutFields.Add("*");
            SubstationqueryTask.ExecuteAsync(query);
        }

        private void SubstationqueryTask_ExecuteCompleted(object sender, ESRI.ArcGIS.Client.Tasks.QueryEventArgs args)
        {
            FeatureSet featureSet = args.FeatureSet;

            if (featureSet != null && featureSet.Features.Count > 0)
            {
                SearchlistCircuitDataGrid.ItemsSource = featureSet.Features;


            }
            else
            {
                MessageBox.Show("No Circuit returned from query");

            }
        }
        private void SubstationqueryTask_Failed(object sender, TaskFailedEventArgs args)
        {
            MessageBox.Show("Query failed: " + args.Error);
        }

        private void radioCircuitSelectdevice_Checked(object sender, RoutedEventArgs e)
        {
            ArrayList objList = new ArrayList();

            RadioButton r = SearchlistCircuitDataGrid.Columns[0].GetCellContent(this.SearchlistCircuitDataGrid.SelectedItem) as RadioButton;

            if (r.IsChecked == true)
            {
                IDictionary<string, object> rddata = ((ESRI.ArcGIS.Client.Graphic)(SearchlistCircuitDataGrid.Columns[1].GetCellContent(this.SearchlistCircuitDataGrid.SelectedItem).DataContext)).Attributes;

                foreach (object value in rddata.Values)
                {
                    objList.Add(value);
                }

                setCircuitID = objList[0].ToString();
                //strFeederId = objList[1] as string;
                SearchType = "3";
                OKButton.Visibility = Visibility.Visible;
                AdddataButton.Visibility = Visibility.Collapsed;



            }
            else
            {
                MessageBox.Show("Select any row from table");
            }
        }
        #endregion
        //Device selection from datagrid
        private void radioEndDeviceSelectdevice_Checked(object sender, RoutedEventArgs e)
        {
            ArrayList objList = new ArrayList();

            RadioButton r = SearchlistEndDeviceDataGrid.Columns[0].GetCellContent(this.SearchlistEndDeviceDataGrid.SelectedItem) as RadioButton;

            if (r.IsChecked == true)
            {


                selectedList.Clear();
                if (this.SearchlistEndDeviceDataGrid.SelectedItem is clsdevicegridbind)
                {
                    selectedList.Add(((clsdevicegridbind)this.SearchlistEndDeviceDataGrid.SelectedItem));
                }
                ArrayList objList1 = new ArrayList();
                foreach (var item in selectedList)
                {
                    //objList1.Add(item);

                    EndDeviceType = item.DeviceName;
                    setendFeederID = item.CircuitID;
                    setendcircuitID2 = item.CircuitID2;
                    setendoprationnumber = item.OPERATINGNUMBER;
                    setendGUID = item.GUID;
                }
                if (setstartFeederID == setendFeederID || setStartcircuitID2 == setendFeederID || setstartFeederID == setendcircuitID2)
                    {

                SearchType = "1";
                OKButton.Visibility = Visibility.Visible;
                AdddataButton.Visibility = Visibility.Collapsed;
                    }
                    else{

                        MessageBox.Show("Select same circuit ID( Start Device circuit ID is '"+ setstartFeederID+"') in both device or close the window");
                        //CancelButton.Visibility = Visibility;
                    }

            }
            else
            {
                MessageBox.Show("Select any row from table");
            }
        }

    }
    public class clsdevicegridbind
    {
        public string DeviceName { get; set; }
        public string OPERATINGNUMBER { get; set; }
        public string CircuitID { get; set; }
        public string CircuitID2 { get; set; }
        public string GUID { get; set; }
        public string OBJECTID { get; set; }
    }
    public class clsDevicename
    {
        private string _Devicename;
        public string Devicename
        {
            set
            {
                _Devicename = value;
            }
            get
            {
                return _Devicename;
            }
        }
    }
}

