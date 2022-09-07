using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Geometry;
using Miner.Server.Client.Events;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows.Threading;
namespace ArcFMSilverlight
{
    public class JetJobsTool : Control, IActiveControl
    {
        private const int SIZE_WIDTH = 500;
        private const int SIZE_HEIGHT = 450;
        protected const string PanCursor = @"/Images/cursor_pan.png";
        private ToggleButton _jetToggleButton;
        private FloatableWindow _fw = null;
        private Grid _mapArea;
        private Map _map;
        public delegate void SetPreviousControl();
        public SetPreviousControl PreviousControl { get; set; }
        private string _usersUrl;
        //private string _jobServiceUrl;
        private JetJobsManager _jetJobsManager;
        private JetRonTool _jetRonTool = null;
        private JobAssignWindow _jobAssignWindow = null;
        private string _user;
        private int _serviceAreaId;
        private string _divisionMapService;
        public JetModel _jetModel;
        private JetEquipmentModel _jetEquipmentModel;

        private string _selectedJobNumber = "";
        private int _selectedObjectId = 0;
        private string _selectedOperatingNumber = "";
        private string _selectedCgc12 = "";
        private double jetActualHeight;
        private double jetActualWidth;
        //public List<jetequipmentlist> SelectEquipmentList = new List<jetequipmentlist>();
        private ObservableCollection<JetEquipment> _jetEquipment;

        public ToggleButton JetToggleButton
        {
            get
            {
                return _jetToggleButton;
            }
        }

        public JetJob SelectedJob
        {
            get
            {
                return _jetJobsManager.jobsListBox.SelectedItem as JetJob;
            }
        }

        public JetJobsTool(Map map, Grid mapArea, ToggleButton jetToggleButton, XElement element, MapTools mapTools)
        {
            _map = map;
            _mapArea = mapArea;
            _jetToggleButton = jetToggleButton;
            _usersUrl = element.Attribute("usersUrl").Value;

            _serviceAreaId = Convert.ToInt32(element.Element("DivisionMapService").Attribute("LayerId").Value);
            _divisionMapService = element.Element("DivisionMapService").Attribute("Url").Value;
            _user = WebContext.Current.User.Name.Replace("PGE\\", "").ToUpper();

            _jetModel = new JetModel(map, element, _user);

            _jetModel.JetJobsLoaded += new EventHandler(_jetModel_JetJobsLoaded);
            _jetModel.JetJobsLoadedSuccess += new EventHandler(_jetModel_JetJobsLoadedSuccess);
            _jetModel.JetJobsLoadedFailed += new EventHandler(_jetModel_JetJobsLoadedFailed);
            _jetModel.JetJobSaveSuccess += new EventHandler(_jetModel_JetJobSaveSuccess);
            _jetModel.JetJobSaveFailed += new EventHandler(_jetModel_JetJobSaveFailed);
            //_jetModel.JetJobSearchSuccess += new EventHandler<JetSearchEventArgs>(_jetModel_JetJobSearchSuccess);
            //_jetModel.InitializeJobFeatureLayer();
            _jetModel.ValidateJobNumberIsNotUniqueComplete += new EventHandler(_jetModel_ValidateJobNumberIsNotUniqueComplete);
            _jetModel.ValidateJobNumberIsUniqueComplete += new EventHandler(_jetModel_ValidateJobNumberIsUniqueComplete);

            _jetEquipmentModel = new JetEquipmentModel(element, _user);
            _jetRonTool = new JetRonTool(_map, this, mapArea, element, _jetEquipmentModel, mapTools);
            _jetEquipmentModel.EquipmentLoadedSuccess += new EventHandler(_jetEquipmentModel_EquipmentLoadedSuccess);
            _jetEquipmentModel.EquipmentLoadedFailed += new EventHandler(_jetEquipmentModel_EquipmentLoadedFailed);
            //Commented for WEBR Stability - Replace Feature Service with WCF
            //_jetEquipmentModel.InitializeEquipmentIdTypeFeatureLayer();  
            //_jetEquipmentModel.InitializeEquipmentFeatureLayer();
        }

        //WEBR Stability - Replace Feature Service with WCF
        public void OnEquipIdTypeLoadFailure()
        {
            _jetEquipmentModel.OnEquipIdTypeLoadFailure();
        }

        public void LoadEquipIdType(List<EquipmentIdType> equipIdTypeList)
        {
            _jetEquipmentModel.LoadEquipIdType(equipIdTypeList);
        }

        public void InitializeInsTypeDomain(Dictionary<string, string> jetInstallTypes)
        {
            _jetEquipmentModel.InitializeInsTypeDomain(jetInstallTypes);
        }

        public void InitializeDivision(Dictionary<object, string> divisionDomain)
        {
            _jetModel.InitializeDivision(divisionDomain);
        }

        public void JetJobSearchSuccess(List<JetViewJobEquip> searchResult)
        {
            IList<JetSearch> jetSearches = new List<JetSearch>();
            AutoCompleteBox txtFilter = _jetJobsManager.txtFilter;

            foreach (var jetJobEquip in searchResult)
            {
                if (!String.IsNullOrEmpty(jetJobEquip.JobNumber) && jetJobEquip.JobNumber.ToUpper().Contains(txtFilter.SearchText.ToUpper()))
                    jetSearches.Add(new JetSearch { ObjectId = jetJobEquip.ObjectId, SearchTerm = jetJobEquip.JobNumber });
                if (!String.IsNullOrEmpty(jetJobEquip.Address) && jetJobEquip.Address.ToUpper().Contains(txtFilter.SearchText.ToUpper()))
                    jetSearches.Add(new JetSearch { ObjectId = jetJobEquip.ObjectId, SearchTerm = jetJobEquip.Address });
                if (!String.IsNullOrEmpty(jetJobEquip.CGC12) && jetJobEquip.CGC12.ToUpper().Contains(txtFilter.SearchText.ToUpper()))
                    jetSearches.Add(new JetSearch { ObjectId = jetJobEquip.ObjectId, SearchTerm = jetJobEquip.CGC12 });
                if (!String.IsNullOrEmpty(jetJobEquip.Description) && jetJobEquip.Description.ToUpper().Contains(txtFilter.SearchText.ToUpper()))
                    jetSearches.Add(new JetSearch { ObjectId = jetJobEquip.ObjectId, SearchTerm = jetJobEquip.Description });
            }

            // Need to remove duplicate OID/SearchTerms
            jetSearches = jetSearches.GroupBy(js => new { js.ObjectId, js.SearchTerm }).Select(group => group.First()).ToList();  //             = e.FeatureSet.Features.GroupBy(f => f.Attributes["OBJECTID"]).Select(y => y.First()).Select(

            _jetJobsManager.txtFilter.ItemsSource = jetSearches.OrderBy(j => j.SearchTerm).ToList();
            _jetJobsManager.txtFilter.PopulateComplete();
        }

        void _jetModel_JetJobsLoadedFailed(object sender, EventArgs e)
        {
            _jetJobsManager.JobEditWindow.ShowMessage("Failed to load Jobs", "Load Failure");
        }

        void _jetEquipmentModel_EquipmentLoadedFailed(object sender, EventArgs e)
        {

            _jetJobsManager.JobEditWindow.ShowMessage("Failed to load Equipment", "Load Failure");

        }

        void _jetModel_JetJobSaveFailed(object sender, EventArgs e)
        {
            _jetJobsManager.JobEditWindow.ShowMessage("Failed to Save Edits!", "Save Failure");
        }

        public string User
        {
            get
            {
                return _user;
            }
        }

        public JetJobsManager JetJobsManager
        {
            get
            {
                return _jetJobsManager;
            }
        }

        public void StoreSelectedEquipmentList(string operatingNumber, string cgc12, string jobNumber = "")
        {
            _selectedOperatingNumber = operatingNumber;
            _selectedCgc12 = cgc12;
            _selectedJobNumber = jobNumber;
        }

        public string EquipmentToCsv(bool exportHeaders = true)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (exportHeaders)
            {
                stringBuilder.Append(JetEquipment.HeadersToCsv() + "\n");
            }
            if (_jetEquipment != null)
            {
                foreach (JetEquipment jetEquipment in _jetEquipment)
                {
                    stringBuilder.Append(jetEquipment.ToCsv() + "\n");
                }
            }
            return stringBuilder.ToString();
        }

        void _jetModel_ValidateJobNumberIsUniqueComplete(object sender, EventArgs e)
        {
            // Assumption that CW is open

            // Update the props because 
            _jetJobsManager.JobEditWindow.JetJob.Description = _jetJobsManager.JobEditWindow.TxtDescription.Text;
            _jetJobsManager.JobEditWindow.JetJob.Division = ((KeyValuePair<int, string>)_jetJobsManager.JobEditWindow.cboDivision.SelectedItem).Key;

            //WEBR Stability - Replace Feature Service with WCF
            //bool callWCF = _jetModel.Save(_jetJobsManager.JobEditWindow.JetJob);
            //if (callWCF)
            //{
            //call wcf service for edit job
            _jetJobsManager.EditJob(_jetJobsManager.JobEditWindow.JetJob);
            // }
        }

        public void _jetModel_ValidateJobNumberIsNotUniqueComplete(object sender, EventArgs e)
        {
            _jetJobsManager.JobEditWindow.ShowMessage(
                "The JobNumber [ " + _jetJobsManager.JobEditWindow.JetJob.JobNumber +
                " ] already exists\nEnter another JobNumber", "Job Number exists");

        }

        void _jetEquipmentModel_EquipmentLoadedSuccess(object sender, EventArgs e)
        {

            ConfigUtility.UpdateStatusBarText("");

            if (_jetJobsManager.jobsListBox.SelectedItem != null)
            {

                string JNo = ((JetJob)_jetJobsManager.jobsListBox.SelectedItem).JobNumber.ToString();
                // Selected Job may have no equipment
                if (
                    _jetEquipmentModel.JetEquipmentByJobNumber.ContainsKey(
                        ((JetJob)_jetJobsManager.jobsListBox.SelectedItem).JobNumber))
                {
                    _jetEquipment =
                        _jetEquipmentModel.JetEquipmentByJobNumber[
                            ((JetJob)_jetJobsManager.jobsListBox.SelectedItem).JobNumber];
                }
                else
                {
                    _jetEquipment = null;
                }
                _jetJobsManager.equipmentListBox.ItemsSource = _jetEquipment;
                // var Equidata = SelectEquipmentList.Where(p => (p.JOBNUMBER == JNo)).ToList();
                //_jetJobsManager.equipmentListBox.ItemsSource = Equidata;
                _jetJobsManager.equipmentListBox.UpdateLayout();

                SetSelectedEquipById();
            }
            // Set up binding to currently selected job
            _jetRonTool.SetInstallCdCbo();
        }
        public void PushEquipmentlist(List<jetequipmentlist> jetJobEquipmentList)
        {
            _jetEquipmentModel.PushJobEquipmentList(jetJobEquipmentList);
            //SelectEquipmentList = jetJobEquipmentList.ToList();
        }
        public void OnDeleteEquipmentSuccess(List<jetequipmentlist> jetJobEquipmentList)
        {
            if (_jetRonTool != null)
            {
                _jetRonTool.EquipmentEditingDone();
            }
            PushEquipmentlist(jetJobEquipmentList.ToList());
        }

        public void OnEquipmentEditingFailure()
        {
            if (_jetEquipmentModel != null)
            {
                _jetEquipmentModel.OnEquipmentEditingFailure();
            }
        }

        private void SetSelectedEquipById()
        {
            try
            {
                if (_jetEquipment == null) return;

                if (_selectedOperatingNumber != "" || _selectedCgc12 != "")
                {
                    foreach (JetEquipment jetEquipment in _jetEquipment)
                    {
                        if ((_selectedOperatingNumber != "" && jetEquipment.OperatingNumber == _selectedOperatingNumber) ||
                            (_selectedCgc12 != "" && jetEquipment.Cgc12 == _selectedCgc12))
                        {
                            _jetJobsManager.equipmentListBox.SelectedItem = jetEquipment;
                            _jetJobsManager.equipmentListBox.UpdateLayout();
                            _jetJobsManager.equipmentListBox.ScrollIntoView(_jetJobsManager.equipmentListBox.SelectedItem,
                                _jetJobsManager.equipmentListBox.Columns[0]);
                            break;
                        }
                    }
                }

                _selectedCgc12 = "";
                _selectedOperatingNumber = "";
            }
            catch
            {
            }
        }

        public void OnWCFSuccess()
        {
            _jetModel_JetJobSaveSuccess(new object(), new EventArgs());
        }

        public void OnWCFFailure()
        {
            _jetModel_JetJobSaveFailed(new object(), new EventArgs());
        }

        public void OnJobLoadFailure()
        {
            _jetModel_JetJobsLoadedFailed(new object(), new EventArgs());
        }
        
        public void OnEquipmentLoadFailure()
        {
            _jetEquipmentModel_EquipmentLoadedFailed(new object(), new EventArgs());
        }

        void _jetModel_JetJobSaveSuccess(object sender, EventArgs e)
        {
            // Assume that the CW is open
            if (_jetJobsManager.JobEditWindow != null && _jetJobsManager.JobEditWindow.JetJob != null)
            {
                _jetJobsManager.JobEditWindow.BusyIndicator.IsBusy = false;
                _jetJobsManager.RestoreStateFromJobEditor();
                _jetJobsManager.JobEditWindow.DialogResult = true; // closes the dialog
                _jetJobsManager.JobEditWindow.ResetInternalVariables();
                //_jetJobsManager.JobEditWindow.Close();
            }
            //_jetJobsManager.jobsListBox.UpdateLayout();  //commented for INC000004298392
            _jetJobsManager.startjob();
            // Set current list entry
        }

        public void ValidateJob(JetJob jetJob)
        {
            //WEBR Stability - Replace Feature Service with WCF

            _jetJobsManager.JobEditWindow.JetJob.Description = _jetJobsManager.JobEditWindow.TxtDescription.Text;
            _jetJobsManager.JobEditWindow.JetJob.Division = ((KeyValuePair<int, string>)_jetJobsManager.JobEditWindow.cboDivision.SelectedItem).Key;
            jetJob.UserAudit = _user;
            jetJob.LastModifiedDate = DateTime.Now;

            // If this is a new Job then we have to validate
            if (jetJob.ObjectId == 0)
            {
                //_jetModel.ValidateJobNumberIsUnique(_jetJobsManager.JobEditWindow.TxtJobNumber.Text);                
                _jetJobsManager.ValidateAddEditJob(_jetJobsManager.JobEditWindow.TxtJobNumber.Text, _jetJobsManager.JobEditWindow.JetJob, "INSERT");
            }
            else // This was an Edit to an existing Job
            {

                //Code change-TCS-INC000004186986 
                if (_jetJobsManager.JobEditWindow.JetJob.PreviousJobNumber == _jetJobsManager.JobEditWindow.TxtJobNumber.Text)
                {
                    //_jetJobsManager.JobEditWindow.JetJob.Description = _jetJobsManager.JobEditWindow.TxtDescription.Text;
                    //_jetJobsManager.JobEditWindow.JetJob.Division = ((KeyValuePair<int, string>)_jetJobsManager.JobEditWindow.cboDivision.SelectedItem).Key;

                    //bool callWCF = _jetModel.Save(_jetJobsManager.JobEditWindow.JetJob);
                    //if (callWCF)
                    //{
                    //call wcf service for edit job
                    _jetJobsManager.EditJob(_jetJobsManager.JobEditWindow.JetJob);
                    //}
                }
                else
                {
                    //_jetModel.ValidateJobNumberIsUnique(_jetJobsManager.JobEditWindow.TxtJobNumber.Text);
                    _jetJobsManager.ValidateAddEditJob(_jetJobsManager.JobEditWindow.TxtJobNumber.Text, _jetJobsManager.JobEditWindow.JetJob, "UPDATE");
                    _jetRonTool.UpdateJobNumberEquipment();
                }
            }
        }

        public void DeleteJob(JetJob jetJob)
        {
            _selectedJobNumber = "";
            _selectedOperatingNumber = "";
            _selectedCgc12 = "";

            //WEBR Stability - Replace Feature Service with WCF for Favorites
            //bool ifObjAvailable = _jetModel.Delete(jetJob);
            //if (!ifObjAvailable)
            //{
            //call WCF service for delete job
            _jetJobsManager.Deletejob(jetJob.ObjectId.ToString(), jetJob.ReservedBy);
            //}

        }
        public void DeleteEquipment(JetEquipment equipment)
        {
            _selectedCgc12 = "";
            _selectedOperatingNumber = "";

            //_jetEquipmentModel.Delete(equipment);
            //call WCF for delete and fetch all equipments again
            _jetJobsManager.DeleteEquipment(equipment.ObjectId.ToString());
            _jetEquipment.Remove(equipment);
        }

        public void SetCurrentJob(string jobNumber)
        {
            _selectedJobNumber = jobNumber;
        }

        void _jetModel_JetJobsLoadedSuccess(object sender, EventArgs e)
        {

            // PagedCollectionView tempListView=null;
            _jetJobsManager.jobsListBox.ItemsSource = _jetModel.JetJobs;
            ConfigUtility.UpdateStatusBarText("");
            ////ssbl
            //tempListView = new PagedCollectionView(_jetModel.JetJobs);
            //_jetJobsManager.jobsListBox.ItemsSource = tempListView;
            //_jetJobsManager.JobDataPager.Source = tempListView;
            RadioButton checkedRadioButton =
                    ((RadioButton)
                        _jetJobsManager.stpJobSearchType.Children.Where(
                            c => c is RadioButton && ((RadioButton)c).IsChecked.Value).First());
            //if (checkedRadioButton == _jetJobsManager.rdoAllActiveJobs ||
            //    checkedRadioButton == _jetJobsManager.rdoAllPastJobs)
            //{
            //    _jetEquipmentModel.SetFilter(new List<string>());
            //}
            //else
            //{
            //    _jetEquipmentModel.SetFilter(_jetModel.JetJobs.Select(j => j.JobNumber).ToList());
            //}

            if (_selectedJobNumber != "" || _selectedObjectId > 0)
            {
                foreach (JetJob jetJob in _jetModel.JetJobs)
                {
                    if ((_selectedJobNumber != "" && jetJob.JobNumber == _selectedJobNumber) ||
                        (_selectedObjectId > 0 && jetJob.ObjectId == _selectedObjectId))
                    {
                        _jetJobsManager.jobsListBox.SelectedItem = jetJob;
                        _selectedJobNumber = jetJob.JobNumber;
                        _jetJobsManager.jobsListBox.UpdateLayout();
                        _jetJobsManager.jobsListBox.ScrollIntoView(_jetJobsManager.jobsListBox.SelectedItem, _jetJobsManager.jobsListBox.Columns[0]);
                        break;
                    }
                }

                // Refresh equipmentlist if selected objectid
                //if (_selectedJobNumber != "")
                //{
                //    _jetEquipmentModel.SetFilter(new List<string>() { _selectedJobNumber });
                //}

                _selectedJobNumber = "";
                _selectedObjectId = 0;
            }
            //_jetEquipmentModel.RefreshEquipmentList();
        }


        void _jetModel_JetJobsLoaded(object sender, EventArgs e)
        {
            _jetJobsManager.cboDivision.DisplayMemberPath = "Value";
            _jetJobsManager.cboDivision.ItemsSource = JetModel._divisionValuesAll;
            _jetJobsManager.cboDivision.SelectedIndex = 0;

            // Per UAT not finding current division by default
            // FindCurrentDivision();
        }

        public void ConfigureJobsFilter()
        {
            RadioButton checkedRadioButton =
                    ((RadioButton)
                        _jetJobsManager.stpJobSearchType.Children.Where(
                            c => c is RadioButton && ((RadioButton)c).IsChecked.Value).First());

            int division = ((KeyValuePair<int, string>)_jetJobsManager.cboDivision.SelectedItem).Key;
            string user = "";
            bool active = true;

            if (checkedRadioButton == _jetJobsManager.rdoMyActiveJobs)
            {
                user = _user;
            }
            else if (checkedRadioButton == _jetJobsManager.rdoAllActiveJobs)
            {

            }
            else if (checkedRadioButton == _jetJobsManager.rdoMyPastJobs)
            {
                user = _user;
                active = false;
            }
            else if (checkedRadioButton == _jetJobsManager.rdoAllPastJobs)
            {
                active = false;
            }

            _jetModel.SetFilterBase(division, user, active);
        }

        public void GetUsers()
        {
            var pageUri = "";
            if (System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != null && System.Windows.Browser.HtmlPage.Document.DocumentUri.Query != "")
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString().Replace(System.Windows.Browser.HtmlPage.Document.DocumentUri.Query.ToString(), "");
            else
                pageUri = System.Windows.Browser.HtmlPage.Document.DocumentUri.ToString();

            string prefixUrl;
            if (pageUri.Contains("Default.aspx"))
            {
                prefixUrl = pageUri.Substring(0, pageUri.IndexOf("Default.aspx"));
            }
            else
            {
                prefixUrl = pageUri;
            }
            UriBuilder uriBuilder = new UriBuilder(prefixUrl + _usersUrl);
            WebClient client = new WebClient();
            client.DownloadStringCompleted += (s, args) =>
            {
                JsonObject users = (JsonObject)JsonObject.Parse(args.Result);

                var dictUsers = users.ToDictionary(t => t.Key, t => (t.Key + " " + t.Value).Replace("\"", ""));
                if (_jobAssignWindow == null)
                {
                    _jobAssignWindow = new JobAssignWindow();
                }

                _jobAssignWindow.usersListBox.ItemsSource = dictUsers;
                _jobAssignWindow.usersListBox.DisplayMemberPath = "Value";
                Binding valueBinding = new Binding() { Path = new PropertyPath("SelectedItem"), Mode = BindingMode.TwoWay };
                valueBinding.Source = _jetJobsManager.jobsListBox;
                KeyValuePair<string, string> pair = new KeyValuePair<string, string>("", "");
                if (dictUsers.ContainsKey(_user))
                {
                    pair = dictUsers.GetEntry(_user);
                    _jobAssignWindow.usersListBox.SelectedItem = pair;
                }
                else if (_user.EndsWith("ADMIN") && dictUsers.ContainsKey(_user.Replace("ADMIN", "")))
                {
                    pair = dictUsers.GetEntry(_user.Replace("ADMIN", ""));
                    _jobAssignWindow.usersListBox.SelectedItem = pair;
                }

                _jobAssignWindow.TxtJobDescription.SetBinding(TextBox.TextProperty, valueBinding);
                if (dictUsers.ContainsKey(((JetJob)_jetJobsManager.jobsListBox.SelectedItem).ReservedBy))
                {
                    _jobAssignWindow.TxtJobAssignedTo.Text =
                        dictUsers[((JetJob)_jetJobsManager.jobsListBox.SelectedItem).ReservedBy];
                }
                else
                {
                    _jobAssignWindow.TxtJobAssignedTo.Text = ((JetJob)_jetJobsManager.jobsListBox.SelectedItem).ReservedBy;
                }
                _jobAssignWindow.Closed += new EventHandler(jobAssignWindow_Closed);
                _jobAssignWindow.Show();
            };
            client.DownloadStringAsync(uriBuilder.Uri);

        }

        void jobAssignWindow_Closed(object sender, EventArgs e)
        {
            _jetJobsManager.RestoreRadioButtonState();

            if (_jobAssignWindow.DialogResult.HasValue && _jobAssignWindow.DialogResult.Value)
            {
                // Save job
                ((JetJob)_jetJobsManager.jobsListBox.SelectedItem).ReservedBy =
                    ((KeyValuePair<string, string>)_jobAssignWindow.usersListBox.SelectedItem).Key;

                //Commented for WEBR Stability - Replace Feature Service with WCF
                //bool callWCF = _jetModel.Save((JetJob)_jetJobsManager.jobsListBox.SelectedItem);
                //if (callWCF)
                //{
                //call wcf service for edit job
                _jetJobsManager.EditJob((JetJob)_jetJobsManager.jobsListBox.SelectedItem);
                //}
            }
        }

        public void ResetJobsFilter()  //INC000004298392-made public
        {
            ConfigUtility.UpdateStatusBarText("Loading JET Jobs...");
            //_jetEquipmentModel.JetEquipmentByJobNumber.Clear();
            _jetJobsManager.equipmentListBox.ItemsSource = null;
            ConfigureJobsFilter();
        }

        //INC000004298392
        public void PushJobList(List<JetJoblist> jetJobList)
        {

            int div = ((KeyValuePair<int, string>)_jetJobsManager.cboDivision.SelectedItem).Key;
            string division = (div > -1 ? div.ToString() : "");

            string userAndStatus = GetUserAndStatus();
            string reservedBy = userAndStatus.Split(',')[0].ToString();
            string status = userAndStatus.Split(',')[1].ToString();

            string jobNo = _jetJobsManager.txtJobFilter.Text;

            string jobNoLike = !String.IsNullOrEmpty(jobNo) ? jobNo : "";

            //if (div == -1 && reservedBy == "" && status == "1" && jobNoLike == "")
            //{
            //    DispatcherTimer rowDetailsDispatcherTimer = new DispatcherTimer();
            //    rowDetailsDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 50); // Milliseconds
            //    rowDetailsDispatcherTimer.Tick += new EventHandler(jetDispatcherTimer_Tick);
            //    rowDetailsDispatcherTimer.Start();
            //   // _jetModel.GraphicsToPocosFromJobList(jetJobList.Where(p => (p.STATUS == status)).ToList());


            //}

            _jetModel.PushJobList(jetJobList, division, reservedBy, status, jobNoLike);
        }
        void jetDispatcherTimer_Tick(object sender, EventArgs e)
        {
            ((DispatcherTimer)sender).Stop();
        }
        private string GetUserAndStatus()
        {
            RadioButton checkedRadioButton =
                   ((RadioButton)
                       _jetJobsManager.stpJobSearchType.Children.Where(
                           c => c is RadioButton && ((RadioButton)c).IsChecked.Value).First());

            string user = "";
            int status = 1;

            if (checkedRadioButton == _jetJobsManager.rdoMyActiveJobs)
            {
                user = _user;
            }
            else if (checkedRadioButton == _jetJobsManager.rdoAllActiveJobs)
            {

            }
            else if (checkedRadioButton == _jetJobsManager.rdoMyPastJobs)
            {
                user = _user;
                status = 2;
            }
            else if (checkedRadioButton == _jetJobsManager.rdoAllPastJobs)
            {
                status = 2;
            }
            return user + "," + status;
        }

        public void RefreshJobList()
        {
            //Code comment
            //ResetJobsFilter();
            //string jobNo = _jetJobsManager.txtJobFilter.Text;
            //_jetModel.SetFilter(jobNo);
            //_jetModel.RefreshJobList();
        }


        public void RefreshJobList(int objectId)
        {
            //commented for INC000004298392
            //ResetJobsFilter();
            //_jetModel.SetFilter(objectId);
            //_selectedObjectId = objectId;
            //_jetModel.RefreshJobList();
            ResetJobsFilter();
            _selectedObjectId = objectId;
        }

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void RefreshJobList(IList<int> objectIds)
        //{
        //    const int MAX_NUM_FOR_IN_QUERY = 999;

        //    ResetJobsFilter();
        //    if (objectIds.Count > MAX_NUM_FOR_IN_QUERY)
        //    {
        //        objectIds = objectIds.Take(MAX_NUM_FOR_IN_QUERY).ToList();
        //    }

        //    _jetModel.SetFilter(objectIds);
        //    _jetModel.RefreshJobList();
        //}

        //public void Search(string userInput)
        //{
        //    _jetModel.Search(userInput);
        //}

        public string GetJobFilterQuery()
        {
            return _jetModel.GetJobFilterQuery();
        }

        public void FindCurrentDivision()
        {
            MapPoint mapPoint = _map.Extent.GetCenter();
            QueryTask divisionQueryTask = new QueryTask(_divisionMapService + "/" + _serviceAreaId);
            Query query = new ESRI.ArcGIS.Client.Tasks.Query();
            query.Geometry = mapPoint;
            query.OutSpatialReference = _map.SpatialReference;
            query.OutFields.Add("DIVISION");
            divisionQueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(divisionQueryTask_ExecuteCompleted);
            divisionQueryTask.Failed += new EventHandler<TaskFailedEventArgs>(divisionQueryTask_Failed);
            divisionQueryTask.ExecuteAsync(query);
        }

        public void SetVisible(bool visible)
        {
            if (_jetRonTool != null && _jetRonTool.IsActive && !visible)
            {
                _jetRonTool.setActive(false, false);
            }
            else
            {
                setActive(visible);
            }
        }

        void divisionQueryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            //TODO:
        }

        void divisionQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            if (e.FeatureSet != null && e.FeatureSet.Features != null && e.FeatureSet.Features.Count > 0)
            {
                IDictionary<int, string> divisionDomains =
                    (IDictionary<int, string>)_jetJobsManager.cboDivision.ItemsSource;
                string divisionString = e.FeatureSet.Features[0].Attributes["DIVISION"].ToString();

                object selectedItem = divisionDomains.Where(d => d.Value.ToUpper() == divisionString).First();
                if (_jetJobsManager.JobEditWindow != null &&
                    _jetJobsManager.JobEditWindow.Visibility == Visibility.Visible)
                {
                    _jetJobsManager.JobEditWindow.cboDivision.SelectedItem = selectedItem;
                }
                else
                {
                    _jetJobsManager.cboDivision.SelectedItem = selectedItem;
                }
            }
            //            RefreshJobList();
        }

        /// <summary>
        /// Geometry Service URL.
        /// </summary>
        public static readonly DependencyProperty GeometryServiceProperty = DependencyProperty.Register("GeometryService", typeof(string),
            typeof(SAPNotificationTool), null);

        [Category("Measure Properties")]
        [Description("Geometry Service URL")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public string GeometryService
        {
            get { return (string)GetValue(GeometryServiceProperty); }
            set
            {
                SetValue(GeometryServiceProperty, value);
            }
        }
        /// <summary>
        /// Current map.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(JetJobsTool),
            new PropertyMetadata(OnMapChanged));

        [Category("Export Properties")]
        [Description("Associated Map")]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public Map Map
        {
            get { return (Map)GetValue(MapProperty); }
            set { SetValue(MapProperty, value); }
        }

        private static void OnMapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // TODO ???
            var measure = d as JetJobsTool;
            if (measure == null) return;

        }

        #region IActiveControl Members

        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set { setActive(value); }
        }

        private void setActive(bool isActive)
        {
            _isActive = isActive;
            if (getFloatableWindow() == null) return;

            if (!isActive)
            {
                // Clear everything
                _fw.Visibility = System.Windows.Visibility.Collapsed;
            }
            else // Active
            {
                // Reset in case the widget gets lost. Not ideal.
                _fw.Height = _mapArea.ActualHeight - 60;// < SIZE_HEIGHT ? _mapArea.ActualHeight : SIZE_HEIGHT;
                _fw.Width = SIZE_WIDTH;
                _fw.Visibility = System.Windows.Visibility.Visible;
            }


        }

        private void SetVerticalAlignment(double height)
        {
            if (height >= SIZE_HEIGHT)
            {
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            }
            else
            {
                _fw.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                _fw.Height = height - 100;
            }
        }

        private FloatableWindow getFloatableWindow()
        {
            if (_fw == null)
            {
                _fw = new FloatableWindow();
                _fw.ParentLayoutRoot = _mapArea;
                _mapArea.SizeChanged += new SizeChangedEventHandler(_mapArea_SizeChanged);
                _fw.Title = "Job Editor";
                _fw.ResizeMode = ResizeMode.NoResize;
                _jetJobsManager = new JetJobsManager(_jetRonTool, this);
                _jetJobsManager.JobSelectionChanged += new EventHandler(_jetJobsManager_JobSelectionChanged);
                _fw.Content = _jetJobsManager;
                _fw.Closing += new EventHandler<CancelEventArgs>(_fw_Closing);
                _fw.SizeChanged += new SizeChangedEventHandler(_fw_SizeChanged);
                _fw.Show();
                //TODO: offset http://floatablewindow.codeplex.com/discussions/279618
                //BUG: I cannot seem to modify the position of this window, come hell or high water. 
                // Presumably this is because we are setting ourselves to the MapArea, not a Canvas
                setActive(true);
            }
            else
            {
                MinimizeWin(); // INC000004165265
            }

            return _fw;
        }

        void _jetJobsManager_JobSelectionChanged(object sender, EventArgs e)
        {
            if (_jetJobsManager.jobsListBox.SelectedItem == null || _jetEquipmentModel.JetEquipmentByJobNumber.Count == 0)
            {
                _jetEquipment = null;
            }
            else
            {
                if (
                    _jetEquipmentModel.JetEquipmentByJobNumber.ContainsKey(
                        ((JetJob)_jetJobsManager.jobsListBox.SelectedItem).JobNumber))
                {
                    _jetEquipment =
                        _jetEquipmentModel.JetEquipmentByJobNumber[
                            ((JetJob)_jetJobsManager.jobsListBox.SelectedItem).JobNumber];
                }
                else
                {
                    _jetEquipment = null;
                }
            }
            _jetJobsManager.equipmentListBox.ItemsSource = _jetEquipment;
            //var Equidata = SelectEquipmentList.Where(p => (p.JOBNUMBER == JNo)).ToList();
            //_jetJobsManager.equipmentListBox.ItemsSource = Equidata;
        }

        void _mapArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetVerticalAlignment(e.NewSize.Height);
        }

        void _fw_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            const double MIN_HEIGHT = 50;
            const double MIN_WIDTH = 30;

            if (_fw.ActualHeight <= MIN_HEIGHT || _fw.Width <= MIN_WIDTH)
            {
                //_legendControl.Legend.MinWidth = _fw.ActualWidth;
                //_legendControl.Legend.MinHeight = _fw.ActualHeight;
            }
            else
            {
                //_legendControl.Legend.MinWidth = _fw.ActualWidth - 30;
                //_legendControl.Legend.MinHeight = _fw.ActualHeight - 50;
            }
        }

        void _fw_Closing(object sender, CancelEventArgs e)
        {
            _jetToggleButton.IsChecked = false;
            this.IsActive = false;
            // Always cancel the real "closing" because "unclosing" seems tricky
            e.Cancel = true;
            _fw.Visibility = System.Windows.Visibility.Collapsed;
            //if (this.PreviousControl != null) this.PreviousControl();
        }

        public void OnControlActivated(DependencyObject control)
        {
            if (control == this) return;
            IsActive = false;
        }

        // INC000004165265
        public void MinimizeWin()
        {
            _fw.Width = jetActualWidth;
            _fw.Height = jetActualHeight;
            _fw.moveToOriginal();
            _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            SetVerticalAlignment(_mapArea.ActualHeight);
            JetJobsManager.minButton.Visibility = System.Windows.Visibility.Collapsed;
            JetJobsManager.maxButton.Visibility = System.Windows.Visibility.Visible;
        }

        // INC000004165265
        public void MaximizeWin()
        {
            jetActualWidth = _fw.ActualWidth;
            jetActualHeight = _fw.ActualHeight;
            _fw.moveToOriginal();
            _fw.Width = _mapArea.ActualWidth; //1360-@100%   //1810-@75%
            _fw.Height = _mapArea.ActualHeight;//475-@100%     //670-@75%
            _fw.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            SetVerticalAlignment(_mapArea.ActualHeight);
            JetJobsManager.maxButton.Visibility = System.Windows.Visibility.Collapsed;
            JetJobsManager.minButton.Visibility = System.Windows.Visibility.Visible;
        }

        #endregion


    }

    static public class DictExtensions
    {
        public static object TryFindResource(this FrameworkElement element, object resourceKey)
        {
            var currentElement = element;

            while (currentElement != null)
            {
                var resource = currentElement.Resources[resourceKey];
                if (resource != null)
                {
                    return resource;
                }

                currentElement = currentElement.Parent as FrameworkElement;
            }

            return Application.Current.Resources[resourceKey];
        }

        public static KeyValuePair<string, string> GetEntry
            (this IDictionary<string, string> dictionary,
             string key)
        {
            return new KeyValuePair<string, string>(key, dictionary[key]);
        }
    }

}
