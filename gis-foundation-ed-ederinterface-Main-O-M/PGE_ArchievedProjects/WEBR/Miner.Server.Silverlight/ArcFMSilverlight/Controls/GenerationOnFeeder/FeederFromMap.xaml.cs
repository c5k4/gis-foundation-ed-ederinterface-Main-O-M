using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using NLog;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Actions;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows.Browser;
using Miner.Server.Client.Toolkit;
using System.ServiceModel;
using ArcFMSilverlight.Controls.GenerationOnFeeder;
using System.Text;
using System.Reflection;


namespace ArcFMSilverlight
{
    public partial class FeederFromMap : UserControl
    {

        #region declarations
        //ENOS2SAP After Release -- Start
        public double total_genFeeder_size = 0;
        public double total_gensize_SYNCH = 0;
        public double total_gensize_INVEXT = 0;
        public double total_gensize_INVINC = 0;
        public double total_gensize_INDCT = 0;
        public double total_gensize_MIXD = 0;
        public double total_Nameplate = 0;
        //ENOS2SAP After Release -- End

        //ENOS Tariff Change -- Start
        public double total_nameplate_SYNCH = 0;
        public double total_nameplate_INVEXT = 0;
        public double total_nameplate_INVINC = 0;
        public double total_nameplate_INDCT = 0;
        public double total_nameplate_MIXD = 0;
        public double total_nameplate_StandByGen = 0;
        //ENOS Tariff Change -- End

        private static Map _map;
        private static Grid _mapArea;
        private string endpointaddress_map = string.Empty;
        //Zoom to Circuit--start
        private string _circuitSourceURL;
        private const string DEFINITION_CIRCUIT = "CIRCUITID='{0}'";
        private const int FONT_SIZE = 35;
        private FontFamily _fontFamily = new FontFamily("Arial");

        private IDictionary<string, string> _circuitIDDictionary = new Dictionary<string, string>();


        private GraphicsLayer _graphicsLayer = new GraphicsLayer();
        private GraphicsLayer _graphicsLayerPoint = new GraphicsLayer();
        private GraphicsLayer _graphicsLayerText = new GraphicsLayer();



        private IList<Graphic> _lineGraphics = new List<Graphic>();
        private int _scaleLimit;


        //Zoom to Circuit--end



        private GraphicsLayer _MappolygongraphicsLayer = new GraphicsLayer();
        //Polygon Selection
        //private GraphicsLayer _graphicsLayer;

        public FillSymbol FillSymbol { get; set; }
        public LineSymbol LineSymbol { get; set; }

        public MeasureAction.Mode MeasureMode
        { get; set; }
        private List<PolygonSearchItem> _polygonSearchList = new List<PolygonSearchItem>();
        private const string ExportCursor = @"/Images/cursor_measure.png";

        private List<SegmentLayer> _layers = new List<SegmentLayer>();
        private const double MetersToMiles = 0.0006213700922;
        private const double MetersToFeet = 3.280839895;
        private const double SqMetersToSqMiles = 0.0000003861003;
        private const double SqMetersToSqFeet = 10.76391;
        private const double SqMetersToHectare = 0.0001;
        private const double SqMetersToAcre = 0.00024710538;
        private const int TotalDistanceSymbolPosition = 0;
        private const int TotalAreaSymbolPosition = 1;

        public bool isPolygonDrawn = false;
        private const string STAND_POLYGON_GRAPHIC_LAYER = "StandPolygonGraphics";
        private string _GeometryService_Standard;
        List<ComboBoxItem> gridNumberCBIs = new List<ComboBoxItem>();
        List<string> SelectGridlist = new List<string>();
        List<string> SelectGridScale = new List<string>();
        Ponsservicelayercall objPonsServiceCall;
        private static string _deviceSettingsURL;
        string _WCFService_URL = "";

        List<GenFeederDetails> GenDetailsinfo_map = new List<GenFeederDetails>();

        public bool isCalledFromTool = false;
        private List<Graphic> generations = new List<Graphic>();
        #endregion


        public static void ReadFeederConfiguration(Map map, Grid mapArea, string deviceSettingURL)
        {
            _map = map;
            _mapArea = mapArea;

            //  ReadTemplatesConfig();            

            _deviceSettingsURL = deviceSettingURL;

        }
        public FeederFromMap(Grid mapArea)
        {
            _mapArea = mapArea;
            InitializeComponent();
            LoadConfiguration();
            LoadLayerConfiguration_jetwcf();

        }
        private void LoadLayerConfiguration_jetwcf()
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

                                    if (childelement.Name.LocalName == "PONSService")
                                    {
                                        attribute = childelement.Attribute("WCFService_URL").Value;
                                        if (attribute != null)
                                        {
                                            _WCFService_URL = attribute;
                                        }
                                    }






                                }

                            }
                            // Print



                        }
                    }
                }

            }
            catch
            {
            }
        }

        private void LoadConfiguration()
        {
            try
            {
                // logger.Info("Loading configuration started");
                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement element = XElement.Parse(config);
                    // LogHelper.InitializeNLog(element);
                    //LoadConfiguration(element);
                }

                if (Application.Current.Host.InitParams.ContainsKey("Config"))
                {
                    string config = Application.Current.Host.InitParams["Config"];
                    var attribute = "";
                    config = HttpUtility.HtmlDecode(config).Replace("***", ",");
                    XElement elements = XElement.Parse(config);
                    foreach (XElement element in elements.Elements())
                    {

                        switch (element.Name.LocalName)
                        {
                            case "ENOSFeeder":


                                //if (element.Name.LocalName == "ENOSFeeder")
                                //{
                                if (element.HasElements)
                                {
                                    foreach (XElement childelement in element.Elements())
                                    {




                                        if (childelement.Name.LocalName == "CircuitService")
                                        {
                                            attribute = childelement.Attribute("url").Value;
                                            if (attribute != null)
                                            {
                                                _circuitSourceURL = attribute;
                                            }
                                        }


                                        if (childelement.Name.LocalName == "GeometryService_Standard")
                                        {
                                            attribute = childelement.Attribute("url").Value;
                                            if (attribute != null)
                                            {
                                                _GeometryService_Standard = attribute;
                                            }
                                        }
                                        if (childelement.Name.LocalName == "MapScale")
                                        {
                                            attribute = childelement.Attribute("id").Value;
                                            if (attribute != null)
                                            {
                                                _scaleLimit = Convert.ToInt32(attribute);
                                            }
                                        }

                                    }

                                }

                                //}
                                //ReadDivisionCodes(element);
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //logger.FatalException("Error loading configuration: ", e);
            }

            // logger.Info("Loading configuration complete");
            string currentUser = WebContext.Current.User.Name;
        }


        public string _circuitId = string.Empty;
        //call wcf 
        public void GendatafromMapFeederdata(string FeederID)
        {

            _circuitId = FeederID;
            GenDetailsinfo_map.Clear();
            objPonsServiceCall = new Ponsservicelayercall();
            string prefixUrl = objPonsServiceCall.GetPrefixUrl();
            endpointaddress_map = prefixUrl + _WCFService_URL;
            EndpointAddress endPoint = new EndpointAddress(endpointaddress_map);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginGetGenFeederDetails(FeederID,
                 delegate(IAsyncResult result)
                 {
                     List<GenFeederDetails> CombindjobList = ((IServicePons)result.AsyncState).EndGetGenFeederDetails(result);
                     this.Dispatcher.BeginInvoke(
                     delegate()
                     {
                         ClientMapFeederResultHandle_Async(CombindjobList);
                     }
                     );
                 }, cusservice);
            }
            catch
            {
                ConfigUtility.UpdateStatusBarText("");
            }


        }

        List<DataToExportDetails> DataToExportDetailsResult = new List<DataToExportDetails>(); //ENOS Tariff Change
        private void ClientMapFeederResultHandle_Async(List<GenFeederDetails> objCustomerResult)
        {

            GenDetailsinfo_map.Clear();
            //ENOS2SAP After Release -- Start
            total_genFeeder_size = 0;
            total_gensize_SYNCH = 0;
            total_gensize_INVEXT = 0;
            total_gensize_INVINC = 0;
            total_gensize_INDCT = 0;
            total_gensize_MIXD = 0;
            total_Nameplate = 0;
            //ENOS2SAP After Release -- End

            //ENOS Tariff Change -- Start      
            total_nameplate_SYNCH = 0;
            total_nameplate_INVEXT = 0;
            total_nameplate_INVINC = 0;
            total_nameplate_INDCT = 0;
            total_nameplate_MIXD = 0;
            total_nameplate_StandByGen = 0;
            //ENOS Tariff Change -- End

            foreach (GenFeederDetails tc in objCustomerResult)
            {
                //ENOS2SAP After Release -- Start
                try
                {
                    //ENOS Tariff Change- Start
                    DataToExportDetails objDataToExport = new DataToExportDetails();
                    objDataToExport.Address = tc.Address;
                    objDataToExport.METERNUMBER = tc.METERNUMBER;
                    objDataToExport.SPID = tc.SPID;
                    objDataToExport.CGC12 = tc.CGC12;
                    objDataToExport.GenSize = tc.GenSize;
                    objDataToExport.Nameplate = tc.Nameplate;
                    objDataToExport.ProjectReference = tc.ProjectReference;
                    objDataToExport.FEEDERNUM = tc.FEEDERNUM;
                    objDataToExport.LimitedExport = tc.LimitedExport;
                    if (objDataToExport != null)
                        DataToExportDetailsResult.Add(objDataToExport);
                    //ENOS Tariff Change- End

                    GenDetailsinfo_map.Add(tc);

                    //if (tc.GenSize != 0)
                    //{
                    //    total_genFeeder_size += Convert.ToDouble(tc.GenSize);
                    //    if (tc.GenType == "INVEXT" || tc.GenType == "INVINC")
                    //    {
                    //        total_gensize_INVEXT += Convert.ToDouble(tc.GenSize);
                    //    }
                    //    else if (tc.GenType == "SYNCH")
                    //    {
                    //        total_gensize_SYNCH += Convert.ToDouble(tc.GenSize);
                    //    }
                    //    else if (tc.GenType == "INDCT")
                    //    {
                    //        total_gensize_INDCT += Convert.ToDouble(tc.GenSize);
                    //    }
                    //    else if (tc.GenType == "MIXD")
                    //    {
                    //        total_gensize_MIXD += Convert.ToDouble(tc.GenSize);
                    //    }
                    //}
                    //if (tc.Nameplate != 0)
                    //{
                    //    total_Nameplate += Convert.ToDouble(tc.Nameplate);
                    //}
                }
                catch (Exception ex)
                {

                }
                //ENOS2SAP After Release -- End
            }
            if (GenDetailsinfo_map.Count > 0)
            {
                genOnFeederGrid_map.ItemsSource = null;
                FeederFromMapTool _feederFromMapToolObj = new FeederFromMapTool();
                genOnFeederGrid_map.ItemsSource = GenDetailsinfo_map;
                ConfigUtility.UpdateStatusBarText("");

                //ENOS2SAP After Release ---Start
                //try
                //{
                //    txtGenFeederSize.Text = "Total NP Rating (kW): " + Math.Round(total_Nameplate, 3) + "\n";
                //    txtGenFeederSize.Text += "Total Effective Rating (kW): " + Math.Round(total_genFeeder_size, 3) + "\n";
                //    if (total_gensize_INVEXT != 0)
                //    {
                //        txtGenFeederSize.Text += "Inverter Based NP (kW): " + Math.Round(total_gensize_INVEXT, 3) + "\n";
                //    }
                //    if (total_gensize_SYNCH != 0)
                //    {
                //        txtGenFeederSize.Text += "Synchronous (kW): " + Math.Round(total_gensize_SYNCH, 3) + "\n";
                //    }
                //    if (total_gensize_INDCT != 0)
                //    {
                //        txtGenFeederSize.Text += "Induction (kW): " + Math.Round(total_gensize_INDCT, 3) + "\n";
                //    }
                //    if (total_gensize_MIXD != 0)
                //    {
                //        txtGenFeederSize.Text += "Mixed (kW): " + Math.Round(total_gensize_MIXD, 3) +"\n";
                //    }
                //}
                //catch (Exception ex)
                //{

                //}
                //ENOS2SAP After Release ---End
                PrepareDataToExport(); //ENOS Tariff Change
                _feederFromMapToolObj.OpenDialog(_mapArea, _map, this, _circuitId);


            }
            else
            {
                //  ClearGrid();
                ConfigUtility.UpdateStatusBarText("");
                MessageBox.Show("No Generations are attached to selected data.", "No Generations Found", MessageBoxButton.OK);
            }

        }

        //ENOS2SAP After Release --Start
        //private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = "csv", Filter = "Excel XML (*.xml)|*.xml", FilterIndex = 1 };
        //    int i = 1;

        //    if (objSFD.ShowDialog() == true)
        //    {
        //        StringBuilder strBuilder = new StringBuilder();

        //        if (genOnFeederGrid_map.ItemsSource == null)
        //            return;

        //        List<string> lstFields = new List<string>();
        //        //lstFields.Add(FormatHeaderField("S.No"));
        //        if (genOnFeederGrid_map.HeadersVisibility == DataGridHeadersVisibility.Column || genOnFeederGrid_map.HeadersVisibility == DataGridHeadersVisibility.All)
        //        {

        //            foreach (DataGridColumn dgcol in genOnFeederGrid_map.Columns)
        //            {
        //                if (dgcol.Header.ToString() != "")

        //                    lstFields.Add(FormatHeaderField(dgcol.Header.ToString()));
        //            }

        //            BuildStringOfRow(strBuilder, lstFields);

        //        }
        //        foreach (object data in genOnFeederGrid_map.ItemsSource)
        //        {
        //            lstFields.Clear();
        //            foreach (DataGridColumn col in genOnFeederGrid_map.Columns)
        //            {
        //                string strValue = "";
        //                strValue = i.ToString();

        //                System.Windows.Data.Binding objBinding = null;
        //                if (col is DataGridBoundColumn)
        //                    objBinding = (col as DataGridBoundColumn).Binding;
        //                if (col is DataGridTemplateColumn)
        //                {
        //                    //This is a template column...
        //                    //    let us see the underlying dependency object
        //                    DependencyObject objDO =
        //                        (col as DataGridTemplateColumn).CellTemplate.LoadContent();
        //                    FrameworkElement oFE = (FrameworkElement)objDO;
        //                    System.Reflection.FieldInfo oFI = oFE.GetType().GetField("TextProperty");
        //                    if (oFI != null)
        //                    {
        //                        if (oFI.GetValue(null) != null)
        //                        {
        //                            if (oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)) != null)
        //                                objBinding = oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)).ParentBinding;
        //                        }
        //                    }
        //                }
        //                if (objBinding != null)
        //                {
        //                    if (objBinding.Path.Path != "")
        //                    {
        //                        PropertyInfo pi = data.GetType().GetProperty(objBinding.Path.Path);
        //                        if (pi != null)
        //                            strValue = pi.GetValue(data, null).ToString();
        //                    }
        //                    if (objBinding.Converter != null)
        //                    {
        //                        if (strValue != "")
        //                            strValue = objBinding.Converter.Convert(strValue, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
        //                        //else
        //                        //    strValue = objBinding.Converter.Convert(data, typeof(string), objBinding.ConverterParameter, objBinding.ConverterCulture).ToString();
        //                    }
        //                }
        //                lstFields.Add(FormatField(strValue));
        //            }
        //            BuildStringOfRow(strBuilder, lstFields);
        //            i++;
        //        }

        //        StreamWriter sw = new StreamWriter(objSFD.OpenFile());
        //        sw.WriteLine("<?xml version=\"1.0\" " + "encoding=\"utf-8\"?>");
        //        sw.WriteLine("<?mso-application progid" + "=\"Excel.Sheet\"?>");
        //        sw.WriteLine("<Workbook xmlns=\"urn:" + "schemas-microsoft-com:office:spreadsheet\">");
        //        sw.WriteLine("<DocumentProperties " + "xmlns=\"urn:schemas-microsoft-com:" + "office:office\">");
        //        sw.WriteLine("<Created>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</Created>");
        //        sw.WriteLine("</DocumentProperties>");
        //        sw.WriteLine("<Styles>");
        //        sw.WriteLine("<Style ss:ID=\"head\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Bottom\"/><Borders/><Font ss:Underline=\"Single\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
        //        sw.WriteLine("<Style ss:ID=\"title\"><Alignment ss:Horizontal=\"Center\"/><Borders/><Font ss:Size=\"13\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
        //        sw.WriteLine("<Style ss:ID=\"date\"><Alignment ss:Horizontal=\"Right\"/><Borders/><Font/><Interior/><NumberFormat/><Protection/></Style>");
        //        sw.WriteLine("<Style ss:ID=\"rowhead\"><Alignment ss:Horizontal=\"Left\"/><Borders/><Font ss:Color=\"#ffffff\" ss:Bold=\"1\"/><Interior ss:Color=\"#4f81BD\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
        //        sw.WriteLine("<Style ss:ID=\"data\"><Borders><Border ss:Color=\"#4f81BD\" ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
        //                 "<Border ss:Color=\"#4f81BD\" ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
        //                  "<Border ss:Color=\"#4f81BD\" ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
        //                 "<Border ss:Color=\"#4f81BD\" ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" /></Borders></Style>");
        //        sw.WriteLine("</Styles>");
        //        sw.WriteLine("<Worksheet ss:Name=\"Customer Information\" " + "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
        //        sw.WriteLine("<Table>");
        //        sw.WriteLine("<Column ss:Index=\"1\" ss:AutoFitWidth=\"0\" ss:Width=\"30\"/>");
        //        sw.WriteLine("<Column ss:Index=\"2\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
        //        sw.WriteLine("<Column ss:Index=\"3\" ss:AutoFitWidth=\"0\" ss:Width=\"72\"/>");
        //        sw.WriteLine("<Column ss:Index=\"4\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
        //        sw.WriteLine("<Column ss:Index=\"5\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
        //        //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Date:") + String.Format("<Cell ss:MergeAcross=\"1\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDateTime) + "</Row>");
        //        //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Area:") + String.Format("<Cell><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDecLoc) + "</Row>");
        //        //sw.WriteLine("<Row>" + String.Format("<Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell ss:StyleID=\"date\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", DateTime.Now.ToLocalTime().ToLongDateString()) + "</Row>");
        //        sw.WriteLine("<Row></Row>");
        //        //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"title\" ss:MergeAcross=\"4\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Customer Notification Wizard") + "</Row>");
        //        sw.WriteLine("<Row></Row>");
        //        sw.Write(strBuilder.ToString());
        //        sw.WriteLine(AddGenSizeRowtoExcel("Total NP Rating (kW)", Math.Round(total_Nameplate, 3).ToString()));
        //        sw.WriteLine(AddGenSizeRowtoExcel("Total Generation on Feeder (kW)", total_genFeeder_size.ToString()));
        //        sw.WriteLine(AddGenSizeRowtoExcel("Inverter Based (kW)", total_gensize_INVEXT.ToString()));
        //        sw.WriteLine(AddGenSizeRowtoExcel("Synchronous (kW)", total_gensize_SYNCH.ToString()));
        //        sw.WriteLine(AddGenSizeRowtoExcel("Induction (kW)", total_gensize_INDCT.ToString()));
        //        sw.WriteLine(AddGenSizeRowtoExcel("Mixed (kW)", total_gensize_MIXD.ToString()));
        //        sw.WriteLine("</Table>");
        //        sw.WriteLine("</Worksheet>");
        //        sw.WriteLine("</Workbook>");
        //        sw.Close();
        //    }
        //}
        //NOS Tariff Change Start
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog objSFD = new SaveFileDialog() { DefaultExt = "csv", Filter = "Excel XML (*.xml)|*.xml", FilterIndex = 1 };

            //GenDetailsinfo_map.Clear();
            if (objSFD.ShowDialog() == true)
            {
                StreamWriter sw = new StreamWriter(objSFD.OpenFile());
                sw.WriteLine("<?xml version=\"1.0\" " + "encoding=\"utf-8\"?>");
                sw.WriteLine("<?mso-application progid" + "=\"Excel.Sheet\"?>");
                sw.WriteLine("<Workbook xmlns=\"urn:" + "schemas-microsoft-com:office:spreadsheet\">");
                sw.WriteLine("<DocumentProperties " + "xmlns=\"urn:schemas-microsoft-com:" + "office:office\">");
                sw.WriteLine("<Created>" + DateTime.Now.ToLocalTime().ToLongDateString() + "</Created>");
                sw.WriteLine("</DocumentProperties>");
                sw.WriteLine("<Styles>");
                sw.WriteLine("<Style ss:ID=\"head\" ss:Name=\"Normal\"><Alignment ss:Vertical=\"Bottom\"/><Borders/><Font ss:Underline=\"Single\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"title\"><Alignment ss:Horizontal=\"Center\"/><Borders/><Font ss:Size=\"13\" ss:Bold=\"1\"/><Interior/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"date\"><Alignment ss:Horizontal=\"Right\"/><Borders/><Font/><Interior/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"rowhead\"><Alignment ss:Horizontal=\"Left\"/><Borders/><Font ss:Color=\"#ffffff\" ss:Bold=\"1\"/><Interior ss:Color=\"#4f81BD\" ss:Pattern=\"Solid\"/><NumberFormat/><Protection/></Style>");
                sw.WriteLine("<Style ss:ID=\"data\"><Borders><Border ss:Color=\"#4f81BD\" ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                         "<Border ss:Color=\"#4f81BD\" ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                          "<Border ss:Color=\"#4f81BD\" ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" />" +
                         "<Border ss:Color=\"#4f81BD\" ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" /></Borders></Style>");
                sw.WriteLine("</Styles>");
                sw.WriteLine("<Worksheet ss:Name=\"Customer Information\" " + "xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                sw.WriteLine("<Table>");
                sw.WriteLine("<Column ss:Index=\"1\" ss:AutoFitWidth=\"0\" ss:Width=\"30\"/>");
                sw.WriteLine("<Column ss:Index=\"2\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                sw.WriteLine("<Column ss:Index=\"3\" ss:AutoFitWidth=\"0\" ss:Width=\"72\"/>");
                sw.WriteLine("<Column ss:Index=\"4\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                sw.WriteLine("<Column ss:Index=\"5\" ss:AutoFitWidth=\"0\" ss:Width=\"100\"/>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Date:") + String.Format("<Cell ss:MergeAcross=\"1\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDateTime) + "</Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"head\" ss:MergeAcross=\"2\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Interruption Area:") + String.Format("<Cell><Data ss:Type=\"String" + "\">{0}</Data></Cell>", strDecLoc) + "</Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell></Cell><Cell></Cell><Cell></Cell><Cell></Cell><Cell ss:StyleID=\"date\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", DateTime.Now.ToLocalTime().ToLongDateString()) + "</Row>");
                sw.WriteLine("<Row></Row>");
                //sw.WriteLine("<Row>" + String.Format("<Cell ss:StyleID=\"title\" ss:MergeAcross=\"4\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", "Customer Notification Wizard") + "</Row>");
                sw.WriteLine("<Row></Row>");
                sw.Write(strBuilder.ToString());
                sw.WriteLine(AddGenSizeRowtoExcel("Total NP Rating (kW)", Math.Round(total_Nameplate, 3).ToString()));
                //sw.WriteLine(AddGenSizeRowtoExcel("Total Generation on Feeder (kW)", total_genFeeder_size.ToString()));
                sw.WriteLine(AddGenSizeRowtoExcel("Total Effective Rating (kW)", total_genFeeder_size.ToString())); //ENOS Tariff Name Change 
                sw.WriteLine(AddGenSizeRowtoExcel("Inverter Based NP (kW)", total_nameplate_INVEXT.ToString())); //ENOS Tariff Name Change
                //sw.WriteLine(AddGenSizeRowtoExcel("Inverter Based (kW)", total_gensize_INVEXT.ToString()));
                sw.WriteLine(AddGenSizeRowtoExcel("Synchronous (kW)", total_nameplate_SYNCH.ToString()));
                sw.WriteLine(AddGenSizeRowtoExcel("Induction (kW)", total_nameplate_INDCT.ToString()));
                //sw.WriteLine(AddGenSizeRowtoExcel("Mixed (kW)", total_nameplate_MIXD.ToString()));
                sw.WriteLine(AddGenSizeRowtoExcel("Standby Generation (kW)", total_nameplate_StandByGen.ToString()));//ENOS Tariff Change
                sw.WriteLine("</Table>");
                sw.WriteLine("</Worksheet>");
                sw.WriteLine("</Workbook>");
                sw.Close();
            }

        }
        private void PrepareDataToExport()
        {
            string sapEGINOTI = string.Empty;
            string strValue = "";
            StringBuilder strBuilder = new StringBuilder();

            if (genOnFeederGrid_map.ItemsSource == null)
                return;

            List<string> lstFields = new List<string>();
            //lstFields.Add(FormatHeaderField("S.No"));
            if (genOnFeederGrid_map.HeadersVisibility == DataGridHeadersVisibility.Column || genOnFeederGrid_map.HeadersVisibility == DataGridHeadersVisibility.All)
            {

                foreach (DataGridColumn dgcol in genOnFeederGrid_map.Columns)
                {
                    if (dgcol.Header.ToString() != "")
                        lstFields.Add(FormatHeaderField(dgcol.Header.ToString()));
                }

                BuildStringOfRow(strBuilder, lstFields);

            }
            foreach (object data in genOnFeederGrid_map.ItemsSource)
            {
                lstFields.Clear();
                foreach (DataGridColumn col in genOnFeederGrid_map.Columns)
                {

                    System.Windows.Data.Binding objBinding = null;
                    if (col is DataGridBoundColumn)
                        objBinding = (col as DataGridBoundColumn).Binding;
                    if (col is DataGridTemplateColumn)
                    {
                        //This is a template column...
                        //    let us see the underlying dependency object
                        DependencyObject objDO =
                            (col as DataGridTemplateColumn).CellTemplate.LoadContent();
                        FrameworkElement oFE = (FrameworkElement)objDO;
                        System.Reflection.FieldInfo oFI = oFE.GetType().GetField("TextProperty");
                        if (oFI != null)
                        {
                            if (oFI.GetValue(null) != null)
                            {
                                if (oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)) != null)
                                    objBinding = oFE.GetBindingExpression((DependencyProperty)oFI.GetValue(null)).ParentBinding;
                            }
                        }
                    }
                    if (objBinding != null)
                    {
                        if (objBinding.Path.Path != "")
                        {

                            PropertyInfo pi = data.GetType().GetProperty(objBinding.Path.Path);
                            if (pi != null)
                            {
                                strValue += "'" + ((ArcFMSilverlight.GenFeederDetails)(data)).ProjectReference.ToString() + "',";
                            }
                        }
                    }
                }
            }

            strValue = strValue.Remove(strValue.Length - 1);


            EndpointAddress endPoint = new EndpointAddress(endpointaddress_map);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BegingetDataToExportFromSettings(strValue,
                 delegate(IAsyncResult result)
                 {
                     List<DataToExportDetails> CombindjobList = ((IServicePons)result.AsyncState).EndgetDataToExportFromSettings(result);
                     this.Dispatcher.BeginInvoke(
                     delegate()
                     {
                         ClientDataExportjobResultHandle_Async(CombindjobList);
                     }
                     );
                 }, cusservice);
            }
            catch { }
        }
        StringBuilder strBuilder = new StringBuilder();
        private void ClientDataExportjobResultHandle_Async(List<DataToExportDetails> objCustomerResult)
        {

            try
            {
                //ENOS Tariff Change -- Start  
                total_genFeeder_size = 0;
                total_nameplate_SYNCH = 0;
                total_nameplate_INVEXT = 0;
                total_nameplate_INVINC = 0;
                total_nameplate_INDCT = 0;
                total_nameplate_MIXD = 0;
                total_nameplate_StandByGen = 0;
                //ENOS Tariff Change -- End

                List<DataToExportDetails> DataToExportList = new List<DataToExportDetails>();
                DataToExportList.Clear();
                for (int i = 0; i < DataToExportDetailsResult.Count; i++)
                {

                    foreach (DataToExportDetails row in objCustomerResult)
                    {
                        DataToExportDetails objDataToExport = new DataToExportDetails();
                        if (row.ProjectReference.ToString() == DataToExportDetailsResult[i].ProjectReference.ToString())
                        {
                            objDataToExport.Address = DataToExportDetailsResult[i].Address;
                            objDataToExport.METERNUMBER = DataToExportDetailsResult[i].METERNUMBER;
                            objDataToExport.SPID = DataToExportDetailsResult[i].SPID;
                            objDataToExport.CGC12 = DataToExportDetailsResult[i].CGC12;
                            //objDataToExport.GenSize = DataToExportDetailsResult[i].GenSize;
                            objDataToExport.GenSize = row.GenSize;
                            //objDataToExport.Nameplate = DataToExportDetailsResult[i].Nameplate;
                            objDataToExport.Nameplate = row.Nameplate;
                            objDataToExport.ProjectReference = DataToExportDetailsResult[i].ProjectReference;
                            objDataToExport.FEEDERNUM = DataToExportDetailsResult[i].FEEDERNUM;
                            objDataToExport.GenType = row.GenType;
                            objDataToExport.TechType = row.TechType.ToString();
                            objDataToExport.EquipmentType = row.EquipmentType.ToString();
                            objDataToExport.SAPEquipmentID = row.SAPEquipmentID.ToString();
                            objDataToExport.EquipSAPEquipmentID = row.EquipSAPEquipmentID.ToString();
                            objDataToExport.Derated = row.Derated.ToString();
                            //objDataToExport.LimitedExport = row.LimitedExport.ToString();
                            objDataToExport.LimitedExport = DataToExportDetailsResult[i].LimitedExport;
                            objDataToExport.ProgramType = row.ProgramType.ToString();
                            objDataToExport.ExportToGrid = row.ExportToGrid.ToString();
                            objDataToExport.StandByGen = row.StandByGen.ToString();

                            if (objDataToExport.StandByGen == "Y")
                            {
                                total_nameplate_StandByGen += Convert.ToDouble(objDataToExport.Nameplate);
                            }
                            else //(objDataToExport.StandByGen != "Y")
                            {
                                //{
                                //    total_nameplate_StandByGen += objDataToExport.Nameplate;
                                //}
                                if (objDataToExport.GenSize != 0)
                                {
                                    total_genFeeder_size += Convert.ToDouble(objDataToExport.GenSize);
                                }

                                if (objDataToExport.Nameplate != 0)
                                {
                                    total_Nameplate += Convert.ToDouble(objDataToExport.Nameplate);

                                    if (objDataToExport.GenType == "INVEXT" || objDataToExport.GenType == "INVINC")
                                    {
                                        total_nameplate_INVEXT += Convert.ToDouble(objDataToExport.Nameplate);
                                    }
                                    else if (objDataToExport.GenType == "SYNCH")
                                    {
                                        total_nameplate_SYNCH += Convert.ToDouble(objDataToExport.Nameplate);
                                    }
                                    else if (objDataToExport.GenType == "INDCT")
                                    {
                                        total_nameplate_INDCT += Convert.ToDouble(objDataToExport.Nameplate);
                                    }
                                    //else if (objDataToExport.GenType == "MIXD")
                                    //{
                                    //    total_nameplate_MIXD += Convert.ToDouble(objDataToExport.Nameplate);
                                    //}
                                }
                            }
                            DataToExportList.Add(objDataToExport);
                        }

                        //if (objDataToExport != null)
                        //    DataToExportList.Add(objDataToExport);
                    }
                }

                try
                {
                    txtGenFeederSize.Text = "Total NP Rating (kW): " + Math.Round(total_Nameplate, 3) + "\n";
                    //txtSumGenSize_Feeder.Text += "Total Generation on Feeder (kW): " + Math.Round(total_gensize_Feeder, 3) + "\n";
                    txtGenFeederSize.Text += "Total Effective Rating (kW): " + Math.Round(total_genFeeder_size, 3) + "\n"; //ENOS Tariff change 
                    if (total_nameplate_INVEXT != 0)
                    {
                        txtGenFeederSize.Text += "Inverter Based NP (kW): " + Math.Round(total_nameplate_INVEXT, 3) + "\n"; //ENOS Tariff NP added in name 
                    }
                    if (total_nameplate_SYNCH != 0)
                    {
                        txtGenFeederSize.Text += "Synchronous (kW): " + Math.Round(total_nameplate_SYNCH, 3) + "\n";
                    }
                    if (total_nameplate_INDCT != 0)
                    {
                        txtGenFeederSize.Text += "Induction (kW): " + Math.Round(total_nameplate_INDCT, 3) + "\n";
                    }
                    if (total_nameplate_MIXD != 0)
                    {
                        //txtSumGenSize_Feeder.Text += "Mixed (kW): " + Math.Round(total_nameplate_MIXD, 3) + "\n";
                    }
                    if (total_nameplate_StandByGen != 0)
                    {
                        txtGenFeederSize.Text += "Standby Generation (kW):" + Math.Round(total_nameplate_StandByGen, 3);
                    }
                }
                catch (Exception ex)
                {
                    //logger.Error("Error while displaying gensize: " + ex.Message);
                }

                List<string> lstFields = new List<string>();

                lstFields.Add(FormatHeaderField("Service Point ID"));
                lstFields.Add(FormatHeaderField("Meter Number"));
                lstFields.Add(FormatHeaderField("CGC12"));
                lstFields.Add(FormatHeaderField("Eff.Rating(kW)"));
                lstFields.Add(FormatHeaderField("NP Rating(kW)"));
                lstFields.Add(FormatHeaderField("Project Reference"));
                lstFields.Add(FormatHeaderField("Feeder Number"));
                lstFields.Add(FormatHeaderField("Gen Type"));
                lstFields.Add(FormatHeaderField("Tech Type"));
                lstFields.Add(FormatHeaderField("Equipment Type"));
                lstFields.Add(FormatHeaderField("Generator SAPID"));
                lstFields.Add(FormatHeaderField("Gen Equip SAPID"));
                lstFields.Add(FormatHeaderField("Derated"));
                lstFields.Add(FormatHeaderField("Limited Export"));
                lstFields.Add(FormatHeaderField("Program Type"));
                lstFields.Add(FormatHeaderField("Export To Grid"));
                lstFields.Add(FormatHeaderField("StandByGen"));
                lstFields.Add(FormatHeaderField("Address"));

                //foreach (PropertyInfo p in typeof(DataToExportDetails).GetProperties())
                //{
                //    lstFields.Add(FormatHeaderField(p.Name));

                //}
                BuildStringOfRow(strBuilder, lstFields);
                foreach (DataToExportDetails data in DataToExportList)
                {
                    lstFields.Clear();
                    lstFields.Add(FormatField(data.SPID));

                    lstFields.Add(FormatField(data.METERNUMBER));

                    lstFields.Add(FormatField(data.CGC12));

                    lstFields.Add(FormatField(data.GenSize.ToString()));

                    lstFields.Add(FormatField(data.Nameplate.ToString()));

                    lstFields.Add(FormatField(data.ProjectReference));

                    lstFields.Add(FormatField(data.FEEDERNUM));

                    lstFields.Add(FormatField(data.GenType));

                    lstFields.Add(FormatField(data.TechType));

                    lstFields.Add(FormatField(data.EquipmentType));

                    lstFields.Add(FormatField(data.SAPEquipmentID.ToString()));

                    lstFields.Add(FormatField(data.EquipSAPEquipmentID.ToString()));

                    lstFields.Add(FormatField(data.Derated));

                    lstFields.Add(FormatField(data.LimitedExport));

                    lstFields.Add(FormatField(data.ProgramType));

                    lstFields.Add(FormatField(data.ExportToGrid));

                    lstFields.Add(FormatField(data.StandByGen));

                    lstFields.Add(FormatField(data.Address));
                    BuildStringOfRow(strBuilder, lstFields);

                }
            }

            catch (Exception ex)
            {
                //logger.Error("Error while Exporting Data: " + ex.Message);
            }
        }

        //ENOS Tariff Change- End

        private string AddGenSizeRowtoExcel(string genType, string sum)
        {
            if (!string.IsNullOrEmpty(sum) && sum != "0")
            {
                string rowtoAdd = null;
                rowtoAdd = "<Row><Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\">" + genType + "</Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\">" + sum + "</Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell>";
                rowtoAdd += "<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String\"></Data></Cell></Row>";
                return rowtoAdd;
            }
            else
            {
                return null;
            }
        }
        //ENOS2SAP After Release --End

        private static void BuildStringOfRow(StringBuilder strBuilder, List<string> lstFields)
        {
            strBuilder.AppendLine("<Row>");
            strBuilder.AppendLine(String.Join("\r\n", lstFields.ToArray()));
            strBuilder.AppendLine("</Row>");
        }

        private static void BuildStringOfRowHeader(StringBuilder strBuilder, string strHeading)
        {
            strBuilder.AppendLine("<Row>");
            strBuilder.AppendLine("<Cell ss:MergeAcross=\"8\"><Data ss:Type=\"String" + "\">" + strHeading + "</Data></Cell>");
            strBuilder.AppendLine("</Row>");
        }

        private static string FormatField(string data)
        {
            return String.Format("<Cell ss:StyleID=\"data\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", data);
        }

        private static string FormatHeaderField(string data)
        {
            return String.Format("<Cell ss:StyleID=\"rowhead\"><Data ss:Type=\"String" + "\">{0}</Data></Cell>", data);
        }

        public string ServiceLocationOID
        {
            get;
            set;
        }

        public string DeviceSettingURL
        {
            get;
            set;
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            // this.DialogResult = null;
            if (genOnFeederGrid_map.SelectedItem != null)
            {
                GenOnFeeder selectedData = genOnFeederGrid_map.SelectedItem != null ? genOnFeederGrid_map.SelectedItem as GenOnFeeder : null;
                if (((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid_map.SelectedItem)).GENGLOBALID != "NA")
                {

                    OpenSettingsUI(((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid_map.SelectedItem)).GENGLOBALID);

                    //OpenSettingsUI(selectedData.GENGLOBALID);
                }
                else
                {
                    MessageBox.Show("No Generations are attached to this service point.", "No Generations Found", MessageBoxButton.OK);
                }
            }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (genOnFeederGrid_map.SelectedItem != null)
            {
                GenOnFeeder selectedData = genOnFeederGrid_map.SelectedItem != null ? genOnFeederGrid_map.SelectedItem as GenOnFeeder : null;
                if (((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid_map.SelectedItem)).GENGLOBALID != "NA")
                {

                    OpenSettingsUI(((ArcFMSilverlight.GenFeederDetails)(genOnFeederGrid_map.SelectedItem)).GENGLOBALID);


                }
                else
                {
                    MessageBox.Show("No Generations are attached to this service point.", "No Generations Found", MessageBoxButton.OK);
                }
            }
        }

        private void genFeederGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.MouseRightButtonDown += new MouseButtonEventHandler(GenFeeder_MouseRightButtonDown);

            e.Row.MouseLeftButtonUp += new MouseButtonEventHandler(GenFeeder_MouseLeftButtonUp);

        }

        void GenFeeder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Zooming to feature...");
            GenOnFeeder generationRow = ((sender) as DataGridRow).DataContext as GenOnFeeder;
            GetGenOnFeeder genFeederObj = new GetGenOnFeeder();
            //  genFeederObj.LocateServicePoint(generationRow.SPID, generationRow.SLGUID, generationRow.TransGUID, generationRow.PMGUID);
            genFeederObj.LocateServicePoint(((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).SPID, ((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).SLGUID, ((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).TransGUID, ((ArcFMSilverlight.GenFeederDetails)(((sender) as DataGridRow).DataContext)).PMGUID);
        }

        private void GenFeeder_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((sender) as DataGridRow != null)
            {
                genOnFeederGrid_map.SelectedItem = ((sender) as DataGridRow).DataContext;
            }
        }

        private void OpenSettingsUI(string genGlobalID)
        {
            GenerationOnTransformer obj = new GenerationOnTransformer();
            obj.DeviceSettingURL = _deviceSettingsURL;
            obj.OpenSettings(genGlobalID, "EDGIS.GENERATIONINFO");
        }

    }
}
