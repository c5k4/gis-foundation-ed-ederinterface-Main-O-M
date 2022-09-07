using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;
using ESRI.ArcGIS.Client.Tasks;
using System.Windows.Data;
using System.ServiceModel;
using System.Windows.Threading;
using System.Text.RegularExpressions;
namespace ArcFMSilverlight
{
    /// <summary>
    /// Responsible for persisting JET Objects via ArcGIS REST API / SL API
    /// </summary>
    public class JetModel
    {
        public event EventHandler JetJobsLoadedSuccess;
        public event EventHandler JetJobsLoadedFailed;
        public List<JetJoblist> objjetjobResultlist = new List<JetJoblist>();
        //public event EventHandler<JetSearchEventArgs> JetJobSearchSuccess;
        public event EventHandler JetJobSearchFailure;

        public event EventHandler JetJobSaveSuccess;
        public event EventHandler JetJobSaveFailed;

        public event EventHandler ValidateJobNumberIsUniqueComplete;
        public event EventHandler ValidateJobNumberIsNotUniqueComplete;

        public event EventHandler JetJobsLoaded;

        //TODO: something about this 
        public const int STATUS_ACTIVE = 1;
        public const int STATUS_RETIRED = 2;
        private JetRonTool _jetRonTool = null;
        private JetJobsManager _jetJobsManager;
        private JetJobsTool _jetJobsTool;
        private ObservableCollection<JetJob> _jetJobs = new ObservableCollection<JetJob>();
        private ObservableCollection<JetJob> _jetAllJobs = new ObservableCollection<JetJob>();
        private Map _map;
        private string _usersUrl;
        //private string _jobServiceUrl;
        //private string _jobSearchServiceUrl;
        //private string _domainFieldName;
        private string _filterBase;
        //private FeatureLayer _jobFeatureLayer;
        //private FeatureLayer _jobSearchFeatureLayer;
        private string _user;
        public static Dictionary<object, string> _divisionValuesTemp = null;
        public static Dictionary<int, string> _divisionValuesAll = null;
        public static Dictionary<int, string> _divisionValuesSelect = null;


        public ObservableCollection<JetJob> JetJobs
        {
            get
            {
                return _jetJobs;
            }
        }
        public ObservableCollection<JetJob> JetAllJobs
        {
            get
            {
                return _jetAllJobs;
            }
        }
        private IDictionary<int, JetJob> _jetJobsDictionary = new Dictionary<int, JetJob>();
        private IDictionary<int, JetJob> _jetAllJobsDictionary = new Dictionary<int, JetJob>();
        bool _callWCF = false;

        public JetModel(Map map, XElement element, string user)
        {
            _map = map;
            //_jobServiceUrl = element.Element("JobService").Attribute("Url").Value + "/" + element.Element("JobService").Attribute("LayerId").Value;
            //_jobSearchServiceUrl = element.Element("JobSearchService").Attribute("Url").Value + "/" + element.Element("JobSearchService").Attribute("LayerId").Value;
            _user = user;
            //_domainFieldName = element.Element("DivisionTypeFilter").Attribute("domainFieldName").Value;


        }

        public string GetJobFilterQuery()
        {
            return _filterBase;
        }

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void Search(string userInput)
        //{
        //    if (String.IsNullOrEmpty(userInput)) return;

        //    var searchQueryTask =
        //        new ESRI.ArcGIS.Client.Tasks.QueryTask(_jobSearchServiceUrl);
        //    searchQueryTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(searchQueryTask_ExecuteCompleted);
        //    searchQueryTask.Failed += new EventHandler<TaskFailedEventArgs>(searchQueryTask_Failed);
        //    ESRI.ArcGIS.Client.Tasks.Query query = new ESRI.ArcGIS.Client.Tasks.Query();
        //    query.Where = "( upper(JOBNUMBER) like '%" + userInput + "%' OR ";
        //    query.Where += "upper(DESCRIPTION) like '%" + userInput + "%' OR ";
        //    query.Where += "upper(CGC12) like '%" + userInput + "%' OR ";
        //    query.Where += "upper(ADDRESS) like '%" + userInput + "%')";
        //    query.Where += " AND (" + _filterBase + " )";
        //    query.OutFields.Add("*");
        //    searchQueryTask.ExecuteAsync(query);

        //}

        //void searchQueryTask_Failed(object sender, TaskFailedEventArgs e)
        //{
        //}

        //void searchQueryTask_ExecuteCompleted(object sender, QueryEventArgs e)
        //{
        //    IList<Graphic> graphics = e.FeatureSet.OrderBy(f => f.Attributes["OBJECTID"]).ToList();
        //    IList<JetJobEquip> jetJobEquips = graphics.Select(
        //        f => new JetJobEquip()
        //        {
        //            ObjectId = Convert.ToInt32(f.Attributes["OBJECTID"]),
        //            JobNumber = Convert.ToString(f.Attributes["JOBNUMBER"]),
        //            Description = Convert.ToString(f.Attributes["DESCRIPTION"]),
        //            Address = Convert.ToString(f.Attributes["ADDRESS"]),
        //            CGC12 = Convert.ToString(f.Attributes["CGC12"])
        //        }).ToList();

        //    if (this.JetJobSearchSuccess != null)
        //    {
        //        this.JetJobSearchSuccess(this, new JetSearchEventArgs(jetJobEquips));
        //    }
        //}

        //public void InitializeJobFeatureLayer()
        //{
        //    _jobFeatureLayer = new FeatureLayer() { Url = _jobServiceUrl, Mode = FeatureLayer.QueryMode.Snapshot };
        //    ESRI.ArcGIS.Client.Tasks.OutFields myOutFields = new ESRI.ArcGIS.Client.Tasks.OutFields();
        //    myOutFields.Add("*");
        //    _jobFeatureLayer.OutFields = myOutFields;
        //    _jobFeatureLayer.Initialized += new EventHandler<EventArgs>(_jobFeatureLayer_Initialized);
        //    _jobFeatureLayer.InitializationFailed += new EventHandler<EventArgs>(_jobFeatureLayer_InitializationFailed);
        //    // equivalent to GetDetails -- not getting any features, which happens on Update
        //    _jobFeatureLayer.Initialize();

        //}


        /// <summary>
        /// Empty parameters mean no filter on that attribute
        /// </summary>
        /// <param name="division"></param>
        /// <param name="user"></param>
        /// <param name="active"></param>
        public void SetFilterBase(int division, string user, bool active) //Job number search - INC000004132584
        {
            _filterBase = GetFilterWhereClause(division, user, active);
            // _jobFeatureLayer.Where = _filterBase;  //INC000004298392
        }

        private string GetFilterWhereClause(int division, string user, bool active)
        {
            return (active ? "STATUS=1 " : "STATUS=2 ") +
            (!String.IsNullOrEmpty(user) ? " AND RESERVEDBY='" + user + "'" : "") +
            (division > -1 ? " AND DIVISION=" + division : "");
        }

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void SetFilter(string jobNo)
        //{
        //    _jobFeatureLayer.Where += !String.IsNullOrEmpty(jobNo) ? " AND JOBNUMBER LIKE '" + jobNo + "%'" : ""; //Job number search - INC000004132584
        //}

        //public void SetFilter(int objectId)
        //{
        //    _jobFeatureLayer.Where = "OBJECTID=" + objectId;
        //}

        //public void SetFilter(IList<int> objectIds)
        //{

        //    _jobFeatureLayer.Where = "OBJECTID in (" + string.Join(",", objectIds) + ") ";
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public bool Delete(JetJob jetJob)
        //{
        //    bool ifObjAvailable = false;
        //    foreach (Graphic graphic in _jobFeatureLayer.Graphics)
        //    {
        //        if (Convert.ToInt32(graphic.Attributes["OBJECTID"]) == jetJob.ObjectId)
        //        {
        //            _jobFeatureLayer.Graphics.Remove(graphic);
        //            ifObjAvailable = true;
        //            break;
        //        }
        //    }
        //    if (ifObjAvailable)
        //    {
        //        _jobFeatureLayer.EndSaveEdits += new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_jobFeatureLayer_EndSaveEdits);
        //        _jobFeatureLayer.SaveEditsFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_jobFeatureLayer_SaveEditsFailed);
        //        _jobFeatureLayer.SaveEdits();
        //    }
        //    return ifObjAvailable;
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public bool Save(JetJob jetJob)
        //{
        //    jetJob.UserAudit = _user;
        //    jetJob.LastModifiedDate = DateTime.Now;

        //    Graphic graphic = PocoToGraphic(jetJob);

        //    //INC000004298392
        //    if (_callWCF)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        if (graphic == null && this.JetJobSaveFailed != null)
        //        {
        //            // Can't find the job!
        //            this.JetJobSaveFailed(this, new EventArgs());
        //        }
        //        else
        //        {
        //            _jobFeatureLayer.EndSaveEdits += new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_jobFeatureLayer_EndSaveEdits);
        //            _jobFeatureLayer.SaveEditsFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(_jobFeatureLayer_SaveEditsFailed);
        //            _jobFeatureLayer.SaveEdits();
        //        }
        //        return false;
        //    }
        //}

        //void _jobFeatureLayer_SaveEditsFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        //{
        //    // Raise an event to say that this is done
        //    if (JetJobSaveFailed != null)
        //    {
        //        this.JetJobSaveFailed(new object(), new EventArgs());
        //    }
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void ValidateJobNumberIsUnique(string jobNumber)
        //{
        //    QueryTask queryTask = new QueryTask(_jobServiceUrl);
        //    Query query = new Query();
        //    query.OutFields.Add("JOBNUMBER");
        //    query.Where = "JOBNUMBER='" + jobNumber + "'";
        //    queryTask.ExecuteCompleted += QueryTaskOnExecuteCompleted;
        //    queryTask.Failed += new EventHandler<TaskFailedEventArgs>(queryTask_Failed);
        //    queryTask.ExecuteAsync(query);
        //}

        //void queryTask_Failed(object sender, TaskFailedEventArgs e)
        //{
        //    if (JetJobsLoadedFailed != null)
        //    {
        //        this.JetJobsLoadedFailed(this, new EventArgs());
        //    }
        //}

        //private void QueryTaskOnExecuteCompleted(object sender, QueryEventArgs queryEventArgs)
        //{
        //    bool isValid = queryEventArgs.FeatureSet.Features.Count == 0;

        //    if (isValid && ValidateJobNumberIsUniqueComplete != null)
        //    {
        //        ValidateJobNumberIsUniqueComplete(this, new EventArgs());



        //        //  _jetRonTool.updateJobnumberEquipment();
        //    }
        //    else if (!isValid && ValidateJobNumberIsNotUniqueComplete != null)
        //    {
        //        ValidateJobNumberIsNotUniqueComplete(this, new EventArgs());
        //    }
        //}

        //

        //void _jobFeatureLayer_EndSaveEdits(object sender, ESRI.ArcGIS.Client.Tasks.EndEditEventArgs e)
        //{

        //    // Raise an event to say that this is done
        //    if (e.Success)
        //    {
        //        // Update the local collection
        //        //_jobFeatureLayer.Update();  // commented for INC000004298392
        //        if (JetJobSaveSuccess != null)
        //        {
        //            JetJobSaveSuccess(new object(), new EventArgs());
        //        }
        //    }
        //    else if (JetJobSaveFailed != null)
        //    {
        //        JetJobSaveFailed(this, new EventArgs());
        //    }
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void RefreshJobList()
        //{
        //    _jobFeatureLayer.Update();
        //}

        //void _jobFeatureLayer_InitializationFailed(object sender, EventArgs e)
        //{
        //    if (JetJobsLoadedFailed != null)
        //    {
        //        this.JetJobsLoadedFailed(new object(), new EventArgs());
        //    }
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //void _jobFeatureLayer_Initialized(object sender, EventArgs e)
        //{
        //    var jobFeatureLayer = sender as FeatureLayer;
        //    _jobFeatureLayer = jobFeatureLayer;
        //    InitializeDivision(jobFeatureLayer.LayerInfo);

        //    jobFeatureLayer.UpdateCompleted += new EventHandler(jobFeatureLayer_UpdateCompleted);
        //    jobFeatureLayer.UpdateFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(jobFeatureLayer_UpdateFailed);
        //    jobFeatureLayer.Update();
        //}

        //private void InitializeDivision(FeatureLayerInfo featureLayerInfo)
        //{
        //    Domain domain = featureLayerInfo.Fields.Where(f => f.Name == _domainFieldName).First().Domain;

        //    CodedValueDomain codedValueDomain = domain as CodedValueDomain;

        //    Dictionary<object, string> placeHolder = new Dictionary<object, string>() {{-1, "--All--"}
        //};
        //    Dictionary<object, string> values = placeHolder.Concat(codedValueDomain.CodedValues).ToDictionary(x => x.Key, x => x.Value);
        //    Dictionary<object, string> placeHolderSelect = new Dictionary<object, string>() {{-1, "--Select--"}
        //};
        //    Dictionary<object, string> valuesSelect = placeHolderSelect.Concat(codedValueDomain.CodedValues).ToDictionary(x => x.Key, x => x.Value);

        //    _divisionValuesAll = values.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);
        //    _divisionValuesSelect = valuesSelect.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);


        //    if (JetJobsLoaded != null)
        //    {
        //        JetJobsLoaded(null, null);
        //    }

        //}

        public void InitializeDivision(Dictionary<object, string> divisionDomain)
        {
            Dictionary<object, string> placeHolder = new Dictionary<object, string>() {{-1, "--All--"}};
            Dictionary<object, string> values = placeHolder.Concat(divisionDomain).ToDictionary(x => x.Key, x => x.Value);
            Dictionary<object, string> placeHolderSelect = new Dictionary<object, string>() {{-1, "--Select--"}};
            Dictionary<object, string> valuesSelect = placeHolderSelect.Concat(divisionDomain).ToDictionary(x => x.Key, x => x.Value);

            _divisionValuesAll = values.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);
            _divisionValuesSelect = valuesSelect.ToDictionary(x => Convert.ToInt32(x.Key), x => x.Value);


            if (JetJobsLoaded != null)
            {
                JetJobsLoaded(null, null);
            }
        }

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //void jobFeatureLayer_UpdateFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        //{
        //    if (JetJobsLoadedFailed != null)
        //    {
        //        this.JetJobsLoadedFailed(new object(), new EventArgs());
        //    }
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //void jobFeatureLayer_UpdateCompleted(object sender, EventArgs e)
        //{
        //    var fl = sender as FeatureLayer;
        //    _jobFeatureLayer = fl;
        //    // No attributes are coming thru, just ObjectId
        //    // GraphicsToPocos();

        //    // Raise an event to say that this is done
        //    //commented for INC000004298392
        //    //if (JetJobsLoadedSuccess != null)
        //    //{
        //    //    this.JetJobsLoadedSuccess(new object(), new EventArgs());
        //    //}

        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public Graphic PocoToGraphic(JetJob jetJob)
        //{
        //    Graphic graphicJob = null;
        //    if (jetJob.ObjectId > 0)
        //    {
        //        try
        //        {
        //            //INC000004298392
        //            List<Graphic> graphicList = _jobFeatureLayer.Graphics.Where(
        //                    g => g.Attributes["OBJECTID"].ToString() == jetJob.ObjectId.ToString()).ToList();
        //            if (graphicList.Count > 0)
        //            {
        //                graphicJob = graphicList.FirstOrDefault();
        //            }
        //            else
        //            {
        //                _callWCF = true;
        //            }
        //            //graphicJob =
        //            //    _jobFeatureLayer.Graphics.Where(
        //            //        g => g.Attributes["OBJECTID"].ToString() == jetJob.ObjectId.ToString()).First();
        //        }
        //        catch
        //        {
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        graphicJob = new Graphic();
        //        _jobFeatureLayer.Graphics.Add(graphicJob);
        //        _callWCF = false;
        //    }

        //    jetJob.UserAudit = _user;
        //    jetJob.LastModifiedDate = DateTime.Now;

        //    if (!_callWCF)
        //    {
        //        graphicJob.Attributes["DESCRIPTION"] = jetJob.Description;
        //        graphicJob.Attributes["DIVISION"] = Convert.ToInt16(jetJob.Division);
        //        graphicJob.Attributes["ENTRYDATE"] = jetJob.EntryDate;
        //        graphicJob.Attributes["JOBNUMBER"] = jetJob.JobNumber;
        //        //graphicJob.Attributes["JOBNUMBER"] = jetJob.PreviousJobNumber;
        //        graphicJob.Attributes["LASTMODIFIEDDATE"] = jetJob.LastModifiedDate;
        //        graphicJob.Attributes["RESERVEDBY"] = jetJob.ReservedBy;
        //        graphicJob.Attributes["USERAUDIT"] = jetJob.UserAudit;
        //        graphicJob.Attributes["STATUS"] = Convert.ToInt16(jetJob.Status);
        //    }

        //    return graphicJob;
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void GraphicsToPocos()
        //{
        //    _jetJobs.Clear();
        //    _jetJobsDictionary.Clear();

        //    for (int i = 0; i < _jobFeatureLayer.Graphics.Count; i++)
        //    {
        //        Graphic graphic = _jobFeatureLayer.Graphics[i];

        //        JetJob jetJob = new JetJob();
        //        jetJob.Description = Convert.ToString(graphic.Attributes["DESCRIPTION"]);
        //        jetJob.Division = Convert.ToInt32(graphic.Attributes["DIVISION"]);
        //        jetJob.EntryDate = Convert.ToDateTime(graphic.Attributes["ENTRYDATE"]);
        //        jetJob.JobNumber = Convert.ToString(graphic.Attributes["JOBNUMBER"]);
        //        //Code change-TCS-INC000004186986
        //        jetJob.PreviousJobNumber = Convert.ToString(graphic.Attributes["JOBNUMBER"]);
        //        jetJob.LastModifiedDate = Convert.ToDateTime(graphic.Attributes["LASTMODIFIEDDATE"]);
        //        jetJob.ObjectId = Convert.ToInt32(graphic.Attributes["OBJECTID"]);
        //        jetJob.ReservedBy = Convert.ToString(graphic.Attributes["RESERVEDBY"]);
        //        jetJob.Status = Convert.ToInt32(graphic.Attributes["STATUS"]);
        //        jetJob.UserAudit = Convert.ToString(graphic.Attributes["USERAUDIT"]);

        //        _jetJobs.Add(jetJob);
        //        _jetJobsDictionary.Add(jetJob.ObjectId, jetJob);
        //    }
        //    _jetJobs = new ObservableCollection<JetJob>(_jetJobs.OrderBy(j => j.ToString()));
        //    //List<JetJob> a = new List<JetJob>();
        //    //a = _jetJobs.ToList();
        //    // PagedCollectionView tempListView = new PagedCollectionView(Jobpager());
        //    // _jetJobsManager.jobsListBox.ItemsSource = tempListView;
        //    //_jetJobsManager.JobDataPager.Source = tempListView;
        //}
        //public List<JetJob> Jobpager()
        //{
        //    List<JetJob> a = new List<JetJob>();
        //    a = _jetJobs.ToList();
        //    return a;
        //}

        //INC000004298392
        //void jetDispatcherTimer_Tick(object sender, EventArgs e)
        //{
        //    ((DispatcherTimer)sender).Stop();
        //}
        public void PushJobList(List<JetJoblist> jetJobList, string division, string reservedBy, string status, string jobNoLike)
        {

            if (division == "" && reservedBy == "" && status == "1" && jobNoLike == "")
            {
                //DispatcherTimer rowDetailsDispatcherTimer = new DispatcherTimer();
                //rowDetailsDispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 100); // Milliseconds
                //rowDetailsDispatcherTimer.Tick += new EventHandler(jetDispatcherTimer_Tick);
                //rowDetailsDispatcherTimer.Start();
                var AllfilteredJobList = jetJobList.Where(p => (p.STATUS == status)).ToList();
                GraphicsToPocosFromJobList(AllfilteredJobList);
            }
            else
            {
                var filteredJobList = FilterJobList(jetJobList, division, reservedBy, status, jobNoLike);

                GraphicsToPocosFromJobList(filteredJobList);
            }


            if (JetJobsLoadedSuccess != null)
            {
                this.JetJobsLoadedSuccess(new object(), new EventArgs());
            }
        }

        private List<JetJoblist> FilterJobList(List<JetJoblist> jetJobList, string division, string reservedBy, string status, string jobNoLike)
        {
            var filteredJobList = jetJobList.Where(p => (p.STATUS == status)).ToList();
            if (!String.IsNullOrEmpty(reservedBy))
            {
                filteredJobList = filteredJobList.Where(p => (p.RESERVEDBY == reservedBy)).ToList();
            }
            if (!String.IsNullOrEmpty(division))
            {
                filteredJobList = filteredJobList.Where(p => p.DIVISION == division).ToList();
            }
            if (!String.IsNullOrEmpty(jobNoLike))
            {
                filteredJobList = filteredJobList.Where(p => p.JOBNUMBER.StartsWith(jobNoLike)).ToList();

            }
            return filteredJobList;
        }

        public void GraphicsToPocosFromJobList(List<JetJoblist> jetJobList)
        {
            _jetJobs.Clear();
            _jetJobsDictionary.Clear();

            for (int i = 0; i < jetJobList.Count; i++)
            {
                JetJob jetJob = new JetJob();
                jetJob.Description = jetJobList[i].DESCRIPTION.ToString();
                jetJob.Division = Convert.ToInt32(jetJobList[i].DIVISION.ToString());
                jetJob.EntryDate = Convert.ToDateTime(jetJobList[i].ENTRYDATE.ToString());
                jetJob.JobNumber = Convert.ToString(jetJobList[i].JOBNUMBER.ToString());
                //Code change-TCS-INC000004186986
                jetJob.PreviousJobNumber = Convert.ToString(jetJobList[i].JOBNUMBER.ToString());
                jetJob.LastModifiedDate = Convert.ToDateTime(jetJobList[i].LASTMODIFIEDDATE.ToString());
                jetJob.ObjectId = Convert.ToInt32(jetJobList[i].OBJECTID.ToString());
                jetJob.ReservedBy = Convert.ToString(jetJobList[i].RESERVEDBY.ToString());
                jetJob.Status = Convert.ToInt32(jetJobList[i].STATUS.ToString());
                jetJob.UserAudit = Convert.ToString(jetJobList[i].USERAUDIT.ToString());

                _jetJobs.Add(jetJob);
                _jetJobsDictionary.Add(jetJob.ObjectId, jetJob);
            }

            _jetJobs = new ObservableCollection<JetJob>(_jetJobs);
            //PagedCollectionView tempListView = new PagedCollectionView(Jobpager());
        }

    }

    public class JetJob
    {
        public int ObjectId { get; set; }
        //Code change-TCS-INC000004186986
        public string PreviousJobNumber { get; set; }
        public string JobNumber { get; set; }
        public string ReservedBy { get; set; }
        public int Division { get; set; }

        public string DivisionName
        {
            get
            {
                return JetModel._divisionValuesAll[Division];
            }
        }
        public string Description { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int Status { get; set; }
        public string UserAudit { get; set; }

        public DateTime EntryDate { get; set; }
        public DateTime EntryDateLocal
        {
            get
            {
                return EntryDate.ToLocalTime();
            }
        }

        public DateTime LastModifiedDateLocal
        {
            get
            {
                return LastModifiedDate.ToLocalTime();
            }
        }


        public override string ToString()
        {
            return JobNumber + " " + Description + " (" + DivisionName + ")";
        }
    }

    //public class JetJobEquip
    //{
    //    public int ObjectId { get; set; }
    //    public string CGC12 { get; set; }
    //    public string JobNumber { get; set; }
    //    public string Address { get; set; }
    //    public string Description { get; set; }

    //    public string ReservedBy { get; set; }
    //    public int Division { get; set; }
    //    public DateTime LastModifiedDate { get; set; }
    //    public int Status { get; set; }
    //    public string UserAudit { get; set; }
    //    public DateTime EntryDate { get; set; }

    //    public JetJob JetJob
    //    {
    //        get
    //        {
    //            return new JetJob()
    //            {
    //                ObjectId = ObjectId,
    //                JobNumber = JobNumber,
    //                Description = Description,
    //                ReservedBy = ReservedBy,
    //                Division = Division,
    //                LastModifiedDate = LastModifiedDate,
    //                Status = Status,
    //                UserAudit = UserAudit,
    //                EntryDate = EntryDate
    //            };
    //        }
    //    }
    //}

    public class JetSearch
    {
        public int ObjectId { get; set; }
        public string SearchTerm { get; set; }

        public override string ToString()
        {
            return SearchTerm;
        }
    }

    //public class JetSearchEventArgs : EventArgs
    //{

    //    public JetSearchEventArgs(IList<JetJobEquip> jetJobEquips)
    //    {
    //        JetJobEquips = jetJobEquips;
    //    }

    //    /// <summary>
    //    /// Property containing the results, used by the attribute viewer. 
    //    /// </summary>
    //    public IList<JetJobEquip> JetJobEquips { get; set; }
    //}


}
