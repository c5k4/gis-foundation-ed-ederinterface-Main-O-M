using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Browser;
using System.Windows.Threading;
using System.Xml.Linq;
using System.ServiceModel;

namespace ArcFMSilverlight
{
    public partial class JetJobsManager : UserControl
    {
        private JetRonTool _jetRonTool = null;
        private JetJobsTool _jetJobsTool = null;
        private bool _initialized = false;
        private RadioButton _lastCheckedRadioButton = null;
        public event EventHandler JobSelectionChanged;
        private JobEditWindow _jobEditWindow = null;
        private int _jobpagesize;
        //private JetModel _jetModel;
        Ponsservicelayercall objPonsServiceCall;
        string _WCFService_URL = "";
        string endpointaddress = string.Empty;
        private List<JetJoblist> objjetjobResult = new List<JetJoblist>();
        private List<jetequipmentlist> objjetequipmentlistResult = new List<jetequipmentlist>();
        private bool isFirstTimeLoad = true;
        public JobEditWindow JobEditWindow
        {
            get
            {
                return _jobEditWindow;
            }

        }

        public JetJobsManager(JetRonTool jetRonTool, JetJobsTool jetJobsTool)
        {
            _jetRonTool = jetRonTool;
            _jetJobsTool = jetJobsTool;
            InitializeComponent();
            LoadLayerConfiguration();
            LoadLayerConfiguration_jetwcf();
            //JobDataPager.PageSize = _jobpagesize;

            objPonsServiceCall = new Ponsservicelayercall();
            string prefixUrl = objPonsServiceCall.GetPrefixUrl();
            endpointaddress = prefixUrl + _WCFService_URL;

            //startjob();
            //WEBR Stability - Replace Feature Service with WCF
            InitializeDivision();
            GetEquipmentIDTypes();
            GetInstallTypeCodedDomain();
        }
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
                        if (element.Name.LocalName == "Jet")
                        {
                            if (element.HasElements)
                            {
                                foreach (XElement childelement in element.Elements())
                                {

                                    if (childelement.Name.LocalName == "Pagesize")
                                    {
                                        attribute = childelement.Attribute("_pagesize").Value;
                                        if (attribute != null)
                                        {
                                            _jobpagesize = Convert.ToInt32(attribute);
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
                        }
                    }
                }

            }
            catch
            {
            }
        }
        private void ExportJobButton_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog objSFD = new SaveFileDialog()
            {
                DefaultExt = "csv",
                Filter = "CSV Files (*.csv)|*.csv",
                FilterIndex = 1

            };
            objSFD.GetType().GetMethod("set_DefaultFileName").Invoke(objSFD, new object[] { _jetJobsTool.SelectedJob.JobNumber + ".csv" });
            if (objSFD.ShowDialog() == true)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(objSFD.OpenFile());
                    sw.WriteLine(_jetJobsTool.EquipmentToCsv());
                    sw.Close();
                }
                catch (Exception exception)
                {
                    MessageBox.Show("An error occurred exporting to CSV\n" + exception.ToString());
                }
            }
        }

        private void NewJobButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_jobEditWindow == null)
            {
                _jobEditWindow = new JobEditWindow();
                _jobEditWindow.Closed += new EventHandler(jobEditWindow_Closed);
            }
            _jobEditWindow.Initialize(cboDivision, _jetJobsTool);
            _jobEditWindow.EditExisting = false;
            _jobEditWindow.Show();
            _jetJobsTool.FindCurrentDivision();
        }

        public void RestoreStateFromJobEditor()
        {
            if (!_jobEditWindow.DialogResult.HasValue || _jobEditWindow.DialogResult.Value)
            {
                _jetJobsTool.SetCurrentJob(_jobEditWindow.JetJob.JobNumber);
            }
            RestoreRadioButtonState();

        }
        public void ShowMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK);
        }

        void jobEditWindow_Closed(object sender, EventArgs e)
        {
            _jetJobsTool.JetJobsManager.RestoreRadioButtonState();
        }

        private void ReassignJobButton_OnClick(object sender, RoutedEventArgs e)
        {
            _jetJobsTool.GetUsers();
        }

        private void ReserveOperatingNumButton_OnClick(object sender, RoutedEventArgs e)
        {
            _jetRonTool.ShowReserveOperatingNumber();
        }
        private void EditOperatingNumButton_OnClick(object sender, RoutedEventArgs e)
        {
            _jetRonTool.ShowReserveOperatingNumber(false);
        }

        private void CboDivision_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //INC000004298392
            if (isFirstTimeLoad)
            {
                isFirstTimeLoad = false;
                _jetJobsTool.ResetJobsFilter();
                startjob();
                EquipmentJobdataload();
            }
            else
            {
                _jetJobsTool.ResetJobsFilter();
                int div = ((KeyValuePair<int, string>)cboDivision.SelectedItem).Key;
                string division = (div > -1 ? div.ToString() : "");

                if (!String.IsNullOrEmpty(division))
                {
                    _jetJobsTool.PushJobList(objjetjobResult.Where(p => p.DIVISION == division).ToList());
                }
                else
                {
                    _jetJobsTool.PushJobList(objjetjobResult.ToList());
                }
                //_jetJobsTool.RefreshJobList();
            }

        }

        private void RdoFilterJobs_OnChecked(object sender, RoutedEventArgs e)
        {
            _lastCheckedRadioButton = sender as RadioButton;

            if (!_initialized)
            {
                _initialized = true;
                return;
            }

            _jetJobsTool.ResetJobsFilter();
            _jetJobsTool.PushJobList(objjetjobResult.ToList());
            //_jetJobsTool.RefreshJobList();
        }
        void jetDispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
        }
        //INC000004132584 - for Search by Job Number
        private void TxtJobNo_OnChanged(object sender, RoutedEventArgs e)
        {
            _jetJobsTool.ResetJobsFilter();
            string jobNo = txtJobFilter.Text;

            if (!String.IsNullOrEmpty(jobNo))
            {
                _jetJobsTool.PushJobList(objjetjobResult.Where(p => p.JOBNUMBER.StartsWith(jobNo)).ToList());
            }
            else
            {

                _jetJobsTool.PushJobList(objjetjobResult.ToList());

            }

            // _jetJobsTool.RefreshJobList();
        }


        public void RestoreRadioButtonState()
        {
            _initialized = false;
            _lastCheckedRadioButton.IsChecked = false;
            _lastCheckedRadioButton.IsChecked = true;
        }

        private void JobsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.JobSelectionChanged != null)
            {
                //_jetJobsTool.PushEquipmentlist(objjetequipmentlistResult.ToList());
                this.JobSelectionChanged(sender, e);
            }
        }

        public void startjob()
        {

            objjetjobResult.Clear();
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();
            try
            {

                cusservice.BeginGetjetjoblist("", "ALL", "ALL", "", 0,
                delegate(IAsyncResult result)
                {
                    List<JetJoblist> CombindjobList = ((IServicePons)result.AsyncState).EndGetjetjoblist(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {
                        ClientJetjobResultHandle_Async(CombindjobList);
                    }
                    );
                }, cusservice);
            }
            catch { }


        }


        private void ClientJetjobResultHandle_Async(List<JetJoblist> objCustomerResult)
        {

            objjetjobResult.Clear();
            foreach (JetJoblist tc in objCustomerResult)
            {
                objjetjobResult.Add(tc);

            }
            _jetJobsTool.PushJobList(objjetjobResult.ToList());
            // EquipmentJobdataload();
        }

        //Delete Job
        public void Deletejob(string objectid, string userid)
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginGetDeleteJob(objectid, userid,
                delegate(IAsyncResult result)
                {
                    string jobresult;
                    jobresult = ((IServicePons)result.AsyncState).EndGetDeleteJob(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientDeleteJetjobResultHandle_Async(jobresult);

                    }
                    );
                }, cusservice);
            }
            catch { }
        }
        private void ClientDeleteJetjobResultHandle_Async(string jobresult)
        {
            if (jobresult == "S")
            {
                _jetJobsTool.OnWCFSuccess();
            }
            else
            {
                _jetJobsTool.OnWCFFailure();
            }
        }

        //Edit/Re-assign job
        //Delete Job
        public void EditJob(JetJob jetJob)
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {


                cusservice.BeginGetEditJob(jetJob.ObjectId.ToString(), Convert.ToInt32(jetJob.Status), jetJob.ReservedBy, Convert.ToInt32(jetJob.Division), jetJob.JobNumber, jetJob.UserAudit, jetJob.Description, jetJob.EntryDate,
                delegate(IAsyncResult result)
                {
                    string jobresult;
                    jobresult = ((IServicePons)result.AsyncState).EndGetEditJob(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientEditJetjobResultHandle_Async(jobresult);

                    }
                    );
                }, cusservice);
            }
            catch { }
        }
        private void ClientEditJetjobResultHandle_Async(string jobresult)
        {
            if (jobresult == "S")
            {
                _jetJobsTool.OnWCFSuccess();
            }
            else
            {
                _jetJobsTool.OnWCFFailure();
            }
        }
        /// <summary>
        ///  Equipment data load
        public void EquipmentJobdataload()
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {


                cusservice.BeginGetEquipmentJob("", "",
                delegate(IAsyncResult result)
                {

                    List<jetequipmentlist> CombindjobList = ((IServicePons)result.AsyncState).EndGetEquipmentJob(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientJetEquipmentJoblistResultHandle_Async(CombindjobList);

                    }
                    );
                }, cusservice);
            }
            catch {
                _jetJobsTool.OnEquipmentLoadFailure();
            }
        }

        private void ClientJetEquipmentJoblistResultHandle_Async(List<jetequipmentlist> objCustomerResult)
        {
            objjetequipmentlistResult.Clear();
            foreach (jetequipmentlist tc in objCustomerResult)
            {
                objjetequipmentlistResult.Add(tc);

            }
            _jetJobsTool.PushEquipmentlist(objjetequipmentlistResult.ToList());
        }
        //Delete Equipment-WCF
        public void DeleteEquipment(string objectid)
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginGetDeleteEquipment(objectid, "D",
                delegate(IAsyncResult result)
                {

                    List<jetequipmentlist> CombindjobList = ((IServicePons)result.AsyncState).EndGetDeleteEquipment(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientupdateJetEquipmentResultHandle_Async(CombindjobList);

                    }
                    );
                }, cusservice);
            }
            catch { }
        }
        private void ClientupdateJetEquipmentResultHandle_Async(List<jetequipmentlist> objCustomerResult)
        {
            if (objCustomerResult.Count == 0)
            {
                _jetJobsTool.OnEquipmentEditingFailure();
            }
            else
            {
                objjetequipmentlistResult.Clear();
                foreach (jetequipmentlist tc in objCustomerResult)
                {
                    objjetequipmentlistResult.Add(tc);
                }
                _jetJobsTool.OnDeleteEquipmentSuccess(objjetequipmentlistResult.ToList());
            }
        }
        //Jet Add/Edit Equipment-WCF
        public void AddEditEquipment(JetEquipment jetEquipment)
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginAddEditJobEquipment(jetEquipment.ObjectId.ToString(), 1, Convert.ToInt32(jetEquipment.EquipTypeId), jetEquipment.OperatingNumber, jetEquipment.City, jetEquipment.SketchLoc, jetEquipment.InstallCd, jetEquipment.Address, Convert.ToDecimal(jetEquipment.Latitude), Convert.ToDecimal(jetEquipment.Longitude), jetEquipment.CustOwned, jetEquipment.JobNumber, jetEquipment.UserAudit, jetEquipment.Cgc12,
                delegate(IAsyncResult result)
                {

                    List<jetequipmentlist> CombindjobList = ((IServicePons)result.AsyncState).EndAddEditJobEquipment(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientAddEditJetEquipResultHandle_Async(CombindjobList);

                    }
                    );
                }, cusservice);
            }
            catch {
                _jetJobsTool.OnEquipmentEditingFailure();
            }

        }

        private void ClientAddEditJetEquipResultHandle_Async(List<jetequipmentlist> objCustomerResult)
        {
            if (objCustomerResult.Count == 0)
            {
                _jetJobsTool.OnEquipmentEditingFailure();
            }
            else
            {
                objjetequipmentlistResult.Clear();
                foreach (jetequipmentlist tc in objCustomerResult)
                {
                    objjetequipmentlistResult.Add(tc);
                }
                _jetJobsTool.PushEquipmentlist(objjetequipmentlistResult.ToList());
            }
        }
        //Edit job number
        public void EditJobNumInEquipment(string oldJobNum, string newJobNum)
        {

            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginEditJobNumberEquipment(oldJobNum, newJobNum,
                delegate(IAsyncResult result)
                {

                    List<jetequipmentlist> CombindjobList = ((IServicePons)result.AsyncState).EndEditJobNumberEquipment(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientEditJonNumberJetEquipmentResultHandle_Async(CombindjobList);

                    }
                    );
                }, cusservice);
            }
            catch { }

        }
        private void ClientEditJonNumberJetEquipmentResultHandle_Async(List<jetequipmentlist> objCustomerResult)
        {
            if (objCustomerResult.Count == 0)
            {
                _jetJobsTool.OnEquipmentEditingFailure();
            }
            else
            {
                objjetequipmentlistResult.Clear();
                foreach (jetequipmentlist tc in objCustomerResult)
                {
                    objjetequipmentlistResult.Add(tc);
                }
                _jetJobsTool.PushEquipmentlist(objjetequipmentlistResult.ToList());
            }
        }
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditJobButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (_jobEditWindow == null)
            {
                _jobEditWindow = new JobEditWindow();
                _jobEditWindow.Closed += new EventHandler(jobEditWindow_Closed);
            }
            _jobEditWindow.Initialize(cboDivision, _jetJobsTool, jobsListBox.SelectedItem as JetJob);
            _jobEditWindow.EditExisting = true;
            _jobEditWindow.Show();
        }

        private void DeleteJobButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (equipmentListBox.ItemsSource == null ||
                ((ObservableCollection<JetEquipment>)equipmentListBox.ItemsSource).Count == 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to Delete this Job?\nClick OK to confirm", "Delete Job", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    // Delete Equipment & then Job?
                    _jetJobsTool.DeleteJob(jobsListBox.SelectedItem as JetJob);
                }
            }
            else
            {
                MessageBox.Show(
                    "You cannot delete a Job with Operating Numbers.\nPlease unreserve all Operating Numbers first");
            }
        }

        private void UnreserveOpreratingNumButton_OnClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to Unreserve this Equipment?\nClick OK to confirm", "Unreserve Equipment", MessageBoxButton.OKCancel);

            if (result == MessageBoxResult.OK)
            {
                // Delete the Equipment
                _jetJobsTool.DeleteEquipment(equipmentListBox.SelectedItem as JetEquipment);
            }
        }


        private void EquipmentListBox_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IEnumerable<UIElement> elementsUnderMouse =
                VisualTreeHelper
                    .FindElementsInHostCoordinates(e.GetPosition(null), this);
            DataGridRow row =
                elementsUnderMouse
                    .Where(uie => uie is DataGridRow)
                    .Cast<DataGridRow>()
                    .FirstOrDefault();
            if (row != null)
            {
                equipmentListBox.SelectedItem = row.DataContext;
            }
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(((JetEquipment)equipmentListBox.SelectedItem).OperatingNumber + "," + ((JetEquipment)equipmentListBox.SelectedItem).Cgc12);
        }

        private void CloseWindowButton_OnClick(object sender, RoutedEventArgs e)
        {
            _jetJobsTool.JetToggleButton.IsChecked = false;
        }

        // INC000004165265
        private void Maximize_OnClick(object sender, RoutedEventArgs e)
        {
            _jetJobsTool.MaximizeWin();
        }

        // INC000004165265s
        private void Minimize_OnClick(object sender, RoutedEventArgs e)
        {
            _jetJobsTool.MinimizeWin();
        }

        private void txtFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                JetSearch jetSearch = (JetSearch)e.AddedItems[0];
                _jetJobsTool.RefreshJobList(jetSearch.ObjectId);

                _jetJobsTool.PushJobList(objjetjobResult.Where(p => p.OBJECTID == jetSearch.ObjectId).ToList());
            }

        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            //TODO: check ESC/Enter etc
            if (e.Key == Key.Escape)
            {
                // _jetJobsTool.RefreshJobList();
                _jetJobsTool.ResetJobsFilter();
                _jetJobsTool.PushJobList(objjetjobResult.ToList());
            }
            else if (e.Key == Key.Enter)
            {
                // Only going to do the partial job search 
                if (txtFilter.SelectedItem == null &&
                    txtFilter.ItemsSource != null &&
                    ((IList<JetSearch>)txtFilter.ItemsSource).Count > 0)
                {
                    _jetJobsTool.ResetJobsFilter();

                    IList<int> objectIds = ((IList<JetSearch>)txtFilter.ItemsSource).Select(js => js.ObjectId).ToList();
                    //_jetJobsTool.RefreshJobList(objectIds);

                    const int MAX_NUM_FOR_IN_QUERY = 999;
                    if (objectIds.Count > MAX_NUM_FOR_IN_QUERY)
                    {
                        objectIds = objectIds.Take(MAX_NUM_FOR_IN_QUERY).ToList();
                    }
                    _jetJobsTool.PushJobList(objjetjobResult.Where(p => objectIds.Contains(p.OBJECTID)).ToList());

                }
            }
        }

        private void txtFilter_Populating(object sender, PopulatingEventArgs e)
        {
            if (String.IsNullOrEmpty(e.Parameter.TrimStart())) return;
            //WEBR Stability - Replace Feature Service with WCF
            //_jetJobsTool.Search(e.Parameter.TrimStart().ToUpper());
            //call WCF
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {
                cusservice.BeginSearchJetJobEquip(e.Parameter.TrimStart().ToUpper(), _jetJobsTool.GetJobFilterQuery(),
                delegate(IAsyncResult result)
                {

                    List<JetViewJobEquip> searchResult = ((IServicePons)result.AsyncState).EndSearchJetJobEquip(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {
                        _jetJobsTool.JetJobSearchSuccess(searchResult);
                        //ClientValidateAddEditJobResultHandle_Async(addEditResult);

                    }
                    );
                }, cusservice);
            }
            catch
            {
                //_jetJobsTool.OnJobLoadFailure();
            }
        }

        private void txtFilter_TextChanged(object sender, RoutedEventArgs e)
        {
            if (txtFilter.Text == "")
            {
                // _jetJobsTool.RefreshJobList();
                _jetJobsTool.ResetJobsFilter();
                _jetJobsTool.PushJobList(objjetjobResult.ToList());
            }

        }

        //WEBR Stability - Replace Feature Service with WCF
        public void ValidateAddEditJob(string jobNumber, JetJob jetJob, string action)     //action is "INSERT" for add job and "UPDATE" for edit job
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginValidateAddEditJob(jetJob.Description, jetJob.Division, jetJob.EntryDate, jetJob.JobNumber,
                    jetJob.LastModifiedDate, jetJob.ReservedBy, jetJob.UserAudit, jetJob.Status, jetJob.ObjectId, action,
                delegate(IAsyncResult result)
                {

                    bool[] addEditResult = ((IServicePons)result.AsyncState).EndValidateAddEditJob(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientValidateAddEditJobResultHandle_Async(addEditResult);

                    }
                    );
                }, cusservice);
            }
            catch
            {
                _jetJobsTool.OnJobLoadFailure();
            }
        }

        private void ClientValidateAddEditJobResultHandle_Async(bool[] addEditResult)
        {
            //addEditResult is a bool array with 2 elements - first one is for unique job number validation & second one is for successful insertion of record in database
            if (addEditResult[0])
            {
                //job number is unique
                //check if record has been inserted successfully
                if (addEditResult[1])
                {
                    _jetJobsTool.OnWCFSuccess();
                }
                else
                {
                    _jetJobsTool.OnWCFFailure();
                }
            }
            else
            {
                //job number is not unique
                _jetJobsTool._jetModel_ValidateJobNumberIsNotUniqueComplete(new object(), new EventArgs());
            }
        }

        //Get Equipment Id Types from database
        private void GetEquipmentIDTypes()
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginGetEquipmentIDTypes(
                delegate(IAsyncResult result)
                {
                    List<EquipmentIdType> equipIdTypeResult = ((IServicePons)result.AsyncState).EndGetEquipmentIDTypes(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {

                        ClientGetEquipmentIdTypeResultHandle_Async(equipIdTypeResult);

                    }
                    );
                }, cusservice);
            }
            catch
            {
                _jetJobsTool.OnEquipIdTypeLoadFailure();
            }
        }

        private void ClientGetEquipmentIdTypeResultHandle_Async(List<EquipmentIdType> equipIdTypeResult)
        {
            _jetJobsTool.LoadEquipIdType(equipIdTypeResult);
        }

        //Get the coded domain values for Install Type
        private void GetInstallTypeCodedDomain()
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {
                cusservice.BeginGetInstallTypeDomain(
                delegate(IAsyncResult result)
                {
                    Dictionary<string, string> intallTypeResult = ((IServicePons)result.AsyncState).EndGetInstallTypeDomain(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {
                        _jetJobsTool.InitializeInsTypeDomain(intallTypeResult);

                    }
                    );
                }, cusservice);
            }
            catch
            {
                _jetJobsTool.OnEquipmentLoadFailure();
            }
        }

        //WEBR Stability - Replace Feature Service with WCF
        public string GetWCFEndPointAddress()
        {
            return endpointaddress;
        }

        private void InitializeDivision()
        {
            EndpointAddress endPoint = new EndpointAddress(endpointaddress);
            BasicHttpBinding httpbinding = new BasicHttpBinding();
            httpbinding.MaxBufferSize = 2147483647;
            httpbinding.MaxReceivedMessageSize = 2147483647;
            httpbinding.TransferMode = TransferMode.Buffered;
            httpbinding.SendTimeout = TimeSpan.FromSeconds(300);
            IServicePons cusservice = new ChannelFactory<IServicePons>(httpbinding, endPoint).CreateChannel();

            try
            {

                cusservice.BeginGetJETDivisions(
                delegate(IAsyncResult result)
                {
                    Dictionary<object, string> divisionDomain = ((IServicePons)result.AsyncState).EndGetJETDivisions(result);
                    this.Dispatcher.BeginInvoke(
                    delegate()
                    {
                        _jetJobsTool.InitializeDivision(divisionDomain);

                    }
                    );
                }, cusservice);
            }
            catch
            {
                _jetJobsTool.OnJobLoadFailure();
            }
        }
    }

    public class SingleSelectedItemToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList list = (IList)value;
            return (list != null && list.Count == 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedItemToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class SelectedItemToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    // jet class-ssbl


    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
