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
namespace ArcFMSilverlight
{
    /// <summary>
    /// Responsible for persisting JET Objects via ArcGIS REST API / SL API
    /// </summary>
    public class JetEquipmentModel
    {

        /// <summary>
        /// 
        /// </summary>Dispatcher
        ///  public event EventHandler JetJobsLoadedSuccess;
        //public event EventHandler JetJobsLoadedFailed;

        //public event EventHandler JetJobSaveSuccess;
        //public event EventHandler JetJobSaveFailed;

        //public event EventHandler ValidateJobNumberIsUniqueComplete;
        //public event EventHandler ValidateJobNumberIsNotUniqueComplete;
        //
        public event EventHandler EquipmentLoadedSuccess;
        public event EventHandler EquipmentLoadedFailed;

        public event EventHandler EquipmentTypeIdLoadedSuccess;
        public event EventHandler EquipmentTypeIdLoadedFailed;

        public event EventHandler EquipmentSaveSuccess;
        public event EventHandler EquipmentSaveFailed;

        //public event EventHandler EquipmentLoaded;

        private ObservableCollection<JetEquipment> _equipment = new ObservableCollection<JetEquipment>();
        //private Map _map;
        //        private string _generateOperatingNumbersUrl;
        //private string _equipmentServiceUrl;
        //private string _equipmentIdTypeServiceUrl;
        //private string _jobServiceUrl;
        //private string _domainFieldName;

        //private FeatureLayer _equipmentFeatureLayer;
        //TODO: replace FL with QT - lighter weight, one fewer call
        //private FeatureLayer _equipmentIdTypeFeatureLayer;
        private string _user;
        //private string prevJobNum;
        //private string newJobNum;
        public static Dictionary<string, string> _jetInstallTypes = new Dictionary<string, string>();
        public static Dictionary<Int16, JetEquipmentType> _equipmentIdTypeValues = new Dictionary<short, JetEquipmentType>();


        public IDictionary<string, ObservableCollection<JetEquipment>> JetEquipmentByJobNumber
        {
            get
            {
                return _equipmentByJobNumberDictionary;
            }
        }
        private IDictionary<int, JetEquipment> _equipmentDictionary = new Dictionary<int, JetEquipment>();
        private IDictionary<string, ObservableCollection<JetEquipment>> _equipmentByJobNumberDictionary = new Dictionary<string, ObservableCollection<JetEquipment>>();

        //public event EventHandler OnQueryValidateJobNumComplete;
        public JetEquipmentModel(XElement element, string user)
        {
            //_generateOperatingNumbersUrl = element.Attribute("generateOperatingNumbersUrl").Value;
            //Commented for WEBR Stability - Replace Feature Service with WCF
            //_jobServiceUrl = element.Element("JobService").Attribute("Url").Value + "/" + element.Element("JobService").Attribute("LayerId").Value;
            //_equipmentServiceUrl = element.Element("EquipmentService").Attribute("Url").Value + "/" + element.Element("EquipmentService").Attribute("LayerId").Value;
            //_equipmentIdTypeServiceUrl = element.Element("EquipmentIdTypeService").Attribute("Url").Value + "/" + element.Element("EquipmentIdTypeService").Attribute("LayerId").Value;
            _user = user;


        }

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void InitializeEquipmentIdTypeFeatureLayer()
        //{
        //    _equipmentIdTypeFeatureLayer = new FeatureLayer() { Url = _equipmentIdTypeServiceUrl, Mode = FeatureLayer.QueryMode.Snapshot };
        //    ESRI.ArcGIS.Client.Tasks.OutFields myOutFields = new ESRI.ArcGIS.Client.Tasks.OutFields();
        //    myOutFields.Add("*");
        //    _equipmentIdTypeFeatureLayer.OutFields = myOutFields;
        //    _equipmentIdTypeFeatureLayer.Initialized += new EventHandler<EventArgs>(_equipmentIdTypeFeatureLayer_Initialized);
        //    _equipmentIdTypeFeatureLayer.InitializationFailed += new EventHandler<EventArgs>(_equipmentIdTypeFeatureLayer_InitializationFailed);
        //    _equipmentIdTypeFeatureLayer.UpdateCompleted += new EventHandler(equipmentIdTypeFeatureLayer_UpdateCompleted);
        //    _equipmentIdTypeFeatureLayer.UpdateFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(equipmentIdTypeFeatureLayer_UpdateFailed);
        //    // equivalent to GetDetails -- not getting any features, which happens on Update
        //    _equipmentIdTypeFeatureLayer.Initialize();
        //}

        //void _equipmentIdTypeFeatureLayer_InitializationFailed(object sender, EventArgs e)
        //{
        //    if (EquipmentTypeIdLoadedFailed != null)
        //    {
        //        this.EquipmentTypeIdLoadedFailed(this, new EventArgs());
        //    }
        //}

        //void _equipmentIdTypeFeatureLayer_Initialized(object sender, EventArgs e)
        //{
        //    var equipmentIdTypeFeatureLayer = sender as FeatureLayer;
        //    _equipmentIdTypeFeatureLayer = equipmentIdTypeFeatureLayer;

        //    equipmentIdTypeFeatureLayer.Update();
        //}

        //void equipmentIdTypeFeatureLayer_UpdateFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        //{
        //    if (EquipmentTypeIdLoadedFailed != null)
        //    {
        //        this.EquipmentTypeIdLoadedFailed(this, new EventArgs());
        //    }
        //}

        //void equipmentIdTypeFeatureLayer_UpdateCompleted(object sender, EventArgs e)
        //{
        //    _equipmentIdTypeFeatureLayer = sender as FeatureLayer;

        //    for (int i = 0; i < _equipmentIdTypeFeatureLayer.Graphics.Count; i++)
        //    {
        //        Graphic graphic = _equipmentIdTypeFeatureLayer.Graphics[i];

        //        JetEquipmentType jetEquipmentType = JetEquipmentType.GraphicToPoco(graphic);
        //        _equipmentIdTypeValues.Add(jetEquipmentType.EquipTypeId, jetEquipmentType);
        //    }

        //    List<KeyValuePair<short, JetEquipmentType>> jetListKvps = _equipmentIdTypeValues.ToList();
        //    jetListKvps.Sort((x, y) => x.Value.EquipTypeName.CompareTo(y.Value.EquipTypeName));
        //    _equipmentIdTypeValues = jetListKvps.ToDictionary(x => x.Key, x => x.Value);
        //    JetEquipmentType selectJetEquipmentType = new JetEquipmentType()
        //    {
        //        EquipTypeId = -1,
        //        EquipTypeName = "--Select--"
        //    };
        //    IDictionary<short, JetEquipmentType> placeHolderSelect = new Dictionary<short, JetEquipmentType>();
        //    placeHolderSelect.Add(-1, selectJetEquipmentType);
        //    _equipmentIdTypeValues = placeHolderSelect.Concat(_equipmentIdTypeValues).ToDictionary(x => x.Key, x => x.Value);

        //    if (this.EquipmentTypeIdLoadedSuccess != null)
        //    {
        //        this.EquipmentTypeIdLoadedSuccess(this, null);
        //    }
        //}

        //WEBR Stability - Replace Feature Service with WCF
        public void OnEquipIdTypeLoadFailure()
        {
            if (EquipmentTypeIdLoadedFailed != null)
            {
                this.EquipmentTypeIdLoadedFailed(this, new EventArgs());
            }
        }

        public void LoadEquipIdType(List<EquipmentIdType> equipIdTypeList)
        {
            for (int i = 0; i < equipIdTypeList.Count; i++)
            {
                JetEquipmentType jetEquipmentType = JetEquipmentType.GraphicToPoco(equipIdTypeList[i]);
                _equipmentIdTypeValues.Add(jetEquipmentType.EquipTypeId, jetEquipmentType);
            }

            List<KeyValuePair<short, JetEquipmentType>> jetListKvps = _equipmentIdTypeValues.ToList();
            jetListKvps.Sort((x, y) => x.Value.EquipTypeName.CompareTo(y.Value.EquipTypeName));
            _equipmentIdTypeValues = jetListKvps.ToDictionary(x => x.Key, x => x.Value);
            JetEquipmentType selectJetEquipmentType = new JetEquipmentType()
            {
                EquipTypeId = -1,
                EquipTypeName = "--Select--"
            };
            IDictionary<short, JetEquipmentType> placeHolderSelect = new Dictionary<short, JetEquipmentType>();
            placeHolderSelect.Add(-1, selectJetEquipmentType);
            _equipmentIdTypeValues = placeHolderSelect.Concat(_equipmentIdTypeValues).ToDictionary(x => x.Key, x => x.Value);

            if (this.EquipmentTypeIdLoadedSuccess != null)
            {
                this.EquipmentTypeIdLoadedSuccess(this, null);
            }
        }

        //public void Delete(JetEquipment jetEquipment)
        //{
        //    //foreach (Graphic graphic in _equipmentFeatureLayer.Graphics)
        //    //{
        //    //    if (Convert.ToInt32(graphic.Attributes["OBJECTID"]) == jetEquipment.ObjectId)
        //    //    {
        //    //        _equipmentFeatureLayer.Graphics.Remove(graphic);
        //    //        break;
        //    //    }
        //    //}

        //    //_equipmentFeatureLayer.SaveEdits();

        //}

        //public void DeleteList(IList<JetEquipment> jetEquipments)
        //{
        //    IList<int> objectIds = jetEquipments.Select(j => j.ObjectId).ToList();
        //    foreach (Graphic graphic in _equipmentFeatureLayer.Graphics)
        //    {
        //        if (objectIds.Contains(Convert.ToInt32(graphic.Attributes["OBJECTID"])))
        //        {
        //            _equipmentFeatureLayer.Graphics.Remove(graphic);
        //            break;
        //        }
        //    }

        //    _equipmentFeatureLayer.SaveEdits();

        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void InitializeEquipmentFeatureLayer()
        //{
        //    FeatureLayer equipmentFeatureLayer = new FeatureLayer() { Url = _equipmentServiceUrl, Mode = FeatureLayer.QueryMode.Snapshot };
        //    ESRI.ArcGIS.Client.Tasks.OutFields myOutFields = new ESRI.ArcGIS.Client.Tasks.OutFields();
        //    myOutFields.Add("*");
        //    equipmentFeatureLayer.OutFields = myOutFields;
        //    equipmentFeatureLayer.Initialized += new EventHandler<EventArgs>(_equipmentFeatureLayer_Initialized);
        //    equipmentFeatureLayer.InitializationFailed += new EventHandler<EventArgs>(_equipmentFeatureLayer_InitializationFailed);
        //    // equivalent to GetDetails -- not getting any features, which happens on Update
        //    equipmentFeatureLayer.Initialize();
        //    equipmentFeatureLayer.UpdateFailed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(equipmentFeatureLayer_UpdateFailed);
        //    equipmentFeatureLayer.UpdateCompleted += new EventHandler(equipmentFeatureLayer_UpdateCompleted);
        //    equipmentFeatureLayer.EndSaveEdits +=
        //        new EventHandler<ESRI.ArcGIS.Client.Tasks.EndEditEventArgs>(_equipmentFeatureLayer_EndSaveEdits);
        //    equipmentFeatureLayer.SaveEditsFailed +=
        //        new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(
        //            _equipmentFeatureLayer_SaveEditsFailed);
        //    _equipmentFeatureLayer = equipmentFeatureLayer;
        //}

        /// <summary>
        /// Empty parameters mean no filter on that attribute
        /// </summary>
        /// <param name="division"></param>
        /// <param name="user"></param>
        /// <param name="active"></param>
        //public void SetFilter(IList<string> jobNumbers)
        //{
        //    if (jobNumbers.Count > 0)
        //    {
        //        string jobNumbersCsv = string.Join(",", jobNumbers.Select(j => "'" + j + "'"));
        //        //TODO: Massive bug, _equipmentFeatureLayer can be null. Timing issue. Resolve.
        //        _equipmentFeatureLayer.Where = "JOBNUMBER IN (" + jobNumbersCsv + ")";
        //    }
        //    else
        //    {
        //        // Grab all
        //        _equipmentFeatureLayer.Where = "";
        //    }
        //}

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public void Save(JetEquipment jetEquipment)
        //{
        //    Graphic graphic = PocoToGraphic(jetEquipment);

        //    if (graphic == null && this.EquipmentSaveFailed != null)
        //    {
        //        // Can't find the job!
        //        this.EquipmentSaveFailed(this, new EventArgs());
        //    }
        //    else
        //    {
        //        _equipmentFeatureLayer.SaveEdits();
        //    }
        //}
        //Code change-TCS-INC000004186986
        //public void SaveJobNumber(string prevJobNo, string newJobNo)
        //{
        //    this.prevJobNum = prevJobNo;
        //    this.newJobNum = newJobNo;
        //    ValidateJobNumberIsUnique(newJobNo);
        //}

        ////Code change-TCS-INC000004186986

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

        //    if (isValid)
        //    {
        //    //    List<Graphic> graphic = PocoToGraphic_jobnumberchange(this.prevJobNum, this.newJobNum);

        //    //    if (graphic == null && this.EquipmentSaveFailed != null)
        //    //    {
        //    //        // Can't find the job!
        //    //        this.EquipmentSaveFailed(this, new EventArgs());
        //    //    }
        //    //    else
        //    //    {
        //    //        _equipmentFeatureLayer.SaveEdits();
        //        //    }
        //        if (this.OnQueryValidateJobNumComplete != null)
        //        {
        //            this.OnQueryValidateJobNumComplete(new object(), new EventArgs());
        //        }
        //    }
        //}

        //
        //void _equipmentFeatureLayer_SaveEditsFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        //{
        //    // Raise an event to say that this is done
        //    if (EquipmentSaveFailed != null)
        //    {
        //        this.EquipmentSaveFailed(new object(), new EventArgs());
        //    }
        //}

        //void _equipmentFeatureLayer_EndSaveEdits(object sender, ESRI.ArcGIS.Client.Tasks.EndEditEventArgs e)
        //{
        //    // Raise an event to say that this is done
        //    if (e.Success)
        //    {
        //        // Update the local collection
        //        //_equipmentFeatureLayer.Update();


        //        // Raise an event to say that this is done
        //        if (EquipmentSaveSuccess != null)
        //        {
        //            this.EquipmentSaveSuccess(new object(), new EventArgs());
        //        }
        //    }
        //    else if (EquipmentSaveFailed != null)
        //    {
        //        EquipmentSaveFailed(this, new EventArgs());
        //    }
        //}

        public void OnEquipmentEditingFailure()
        {
            if (EquipmentSaveFailed != null)
            {
                EquipmentSaveFailed(this, new EventArgs());
            }
        }

        //public void RefreshEquipmentList()
        //{
        //    ConfigUtility.UpdateStatusBarText("Loading JET Equipment...");
        //    _equipmentFeatureLayer.Update();
        //}

        //void _equipmentFeatureLayer_InitializationFailed(object sender, EventArgs e)
        //{
        //    if (EquipmentLoadedFailed != null)
        //    {
        //        this.EquipmentLoadedFailed(new object(), new EventArgs());
        //    }
        //}

        //void _equipmentFeatureLayer_Initialized(object sender, EventArgs e)
        //{
        //    var equipmentFeatureLayer = sender as FeatureLayer;
        //    _equipmentFeatureLayer = equipmentFeatureLayer;
        //    InitializeDomains(equipmentFeatureLayer.LayerInfo);
        //}

        //WEBR Stability - Replace Feature Service with WCF
        //private void InitializeDomains(FeatureLayerInfo featureLayerInfo)
        //{
        //    Domain domain = featureLayerInfo.Fields.Where(f => f.Name == "INSTALLCD").First().Domain;

        //    CodedValueDomain codedValueDomain = domain as CodedValueDomain;

        //    _jetInstallTypes = codedValueDomain.CodedValues.ToDictionary(x => x.Key.ToString(), x => x.Value);
        //    Dictionary<string, string> placeHolderSelect = new Dictionary<string, string>() {{"", "--Select--"}
        //};
        //    Dictionary<string, string> valuesSelect = placeHolderSelect.Concat(_jetInstallTypes).ToDictionary(x => x.Key, x => x.Value);

        //    _jetInstallTypes = valuesSelect;//.ToDictionary(x => Convert.ToString(x.Key), x => x.Value);


        //    if (EquipmentLoaded != null)
        //    {
        //        EquipmentLoaded(null, null);
        //    }

        //}

        public void InitializeInsTypeDomain(Dictionary<string, string> jetInstallTypes)
        {
            _jetInstallTypes = jetInstallTypes;
            Dictionary<string, string> placeHolderSelect = new Dictionary<string, string>() {{"", "--Select--"}};
            Dictionary<string, string> valuesSelect = placeHolderSelect.Concat(_jetInstallTypes).ToDictionary(x => x.Key, x => x.Value);

            _jetInstallTypes = valuesSelect;
        }

        //void equipmentFeatureLayer_UpdateFailed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        //{
        //    if (EquipmentLoadedFailed != null)
        //    {
        //        this.EquipmentLoadedFailed(new object(), new EventArgs());
        //    }
        //}

        //void equipmentFeatureLayer_UpdateCompleted(object sender, EventArgs e)
        //{
        //    //var fl = sender as FeatureLayer;
        //    //_equipmentFeatureLayer = fl;
        //    //// No attributes are coming thru, just ObjectId
        //    //GraphicsToPocos();

        //    //// Raise an event to say that this is done
        //    //if (EquipmentLoadedSuccess != null)
        //    //{
        //    //    this.EquipmentLoadedSuccess(new object(), new EventArgs());
        //    //}

        //}

        public void PushJobEquipmentList(List<jetequipmentlist> jetEquipmentList)
        {
            // No attributes are coming thru, just ObjectId
            GraphicsToPocosFromEquipList(jetEquipmentList);

            // Raise an event to say that this is done
            if (EquipmentLoadedSuccess != null)
            {
                this.EquipmentLoadedSuccess(new object(), new EventArgs());
            }
        }

        public void GraphicsToPocosFromEquipList(List<jetequipmentlist> jetEquipmentList)
        {
            _equipment.Clear();
            _equipmentDictionary.Clear();
            _equipmentByJobNumberDictionary.Clear();

            for (int i = 0; i < jetEquipmentList.Count; i++)
            {
                JetEquipment jetEquipment = new JetEquipment();
                jetEquipment.ObjectId = Convert.ToInt32(jetEquipmentList[i].OBJECTID);
                jetEquipment.EquipTypeId = Convert.ToInt16(jetEquipmentList[i].EQUIPTYPEID);
                jetEquipment.JobNumber = Convert.ToString(jetEquipmentList[i].JOBNUMBER);
                jetEquipment.OperatingNumber = Convert.ToString(jetEquipmentList[i].OperatingNumber);
                jetEquipment.Cgc12 = Convert.ToString(jetEquipmentList[i].Cgc12);
                jetEquipment.Address = Convert.ToString(jetEquipmentList[i].Address);
                jetEquipment.City = Convert.ToString(jetEquipmentList[i].City);
                jetEquipment.SketchLoc = Convert.ToString(jetEquipmentList[i].SketchLoc);
                jetEquipment.InstallCd = Convert.ToString(jetEquipmentList[i].InstallCdName);
                jetEquipment.Latitude = Convert.ToDouble(jetEquipmentList[i].LATITUDE);
                jetEquipment.Longitude = Convert.ToDouble(jetEquipmentList[i].LONGITUDE);

                jetEquipment.EntryDate = Convert.ToDateTime(jetEquipmentList[i].ENTRYDATE);
                jetEquipment.LastModifiedDate = Convert.ToDateTime(jetEquipmentList[i].LastModifiedDateLocal);
                jetEquipment.UserAudit = Convert.ToString(jetEquipmentList[i].USERAUDIT);
                jetEquipment.CustOwned = Convert.ToString(jetEquipmentList[i].CUSTOWNED);

                _equipment.Add(jetEquipment);
                _equipmentDictionary.Add(jetEquipment.ObjectId, jetEquipment);

                if (_equipmentByJobNumberDictionary.ContainsKey(jetEquipment.JobNumber))
                {
                    _equipmentByJobNumberDictionary[jetEquipment.JobNumber].Add(jetEquipment);
                }
                else
                {
                    _equipmentByJobNumberDictionary.Add(jetEquipment.JobNumber, new ObservableCollection<JetEquipment>() { jetEquipment });
                }
            }
            _equipment = new ObservableCollection<JetEquipment>(_equipment.OrderBy(j => j.ToString()));
        }

        //Commented for WEBR Stability - Replace Feature Service with WCF
        //public Graphic PocoToGraphic(JetEquipment jetEquipment)
        //{
        //    Graphic graphicEquipment = null;

        //    //if (jetEquipment.ObjectId > 0)
        //    //{
        //    //    try
        //    //    {
        //    //        graphicEquipment =
        //    //            _equipmentFeatureLayer.Graphics.Where(
        //    //                g => g.Attributes["OBJECTID"].ToString() == jetEquipment.ObjectId.ToString()).First();
        //    //    }
        //    //    catch
        //    //    {
        //    //        return null;
        //    //    }
        //    //}
        //    //else
        //    //{
        //    graphicEquipment = new Graphic();
        //    _equipmentFeatureLayer.Graphics.Add(graphicEquipment);
        //    // }

        //    jetEquipment.LastModifiedDate = DateTime.Now;
        //    jetEquipment.UserAudit = _user;

        //    graphicEquipment.Attributes["JOBNUMBER"] = jetEquipment.JobNumber;
        //    graphicEquipment.Attributes["OPERATINGNUMBER"] = jetEquipment.OperatingNumber;
        //    graphicEquipment.Attributes["CGC12"] = jetEquipment.Cgc12;
        //    graphicEquipment.Attributes["CUSTOWNED"] = jetEquipment.CustOwned;
        //    graphicEquipment.Attributes["SKETCHLOC"] = jetEquipment.SketchLoc;
        //    graphicEquipment.Attributes["INSTALLCD"] = jetEquipment.InstallCd;
        //    graphicEquipment.Attributes["EQUIPTYPEID"] = Convert.ToInt16(jetEquipment.EquipTypeId);
        //    graphicEquipment.Attributes["LATITUDE"] = jetEquipment.Latitude;
        //    graphicEquipment.Attributes["LONGITUDE"] = jetEquipment.Longitude;
        //    graphicEquipment.Attributes["CITY"] = jetEquipment.City;
        //    graphicEquipment.Attributes["ADDRESS"] = jetEquipment.Address;
        //    graphicEquipment.Attributes["INSTALLCD"] = jetEquipment.InstallCd;
        //    graphicEquipment.Attributes["USERAUDIT"] = jetEquipment.UserAudit;
        //    graphicEquipment.Attributes["ENTRYDATE"] = jetEquipment.EntryDate;
        //    graphicEquipment.Attributes["LASTMODIFIEDDATE"] = jetEquipment.LastModifiedDate;

        //    return graphicEquipment;
        //}



        //public List<Graphic> PocoToGraphic_jobnumberchange(string prevJobNum, string newJobNum)
        //{
        //    List<Graphic> graphicEquipList = null;

        //    try
        //    {
        //        graphicEquipList = _equipmentFeatureLayer.Graphics.Where(g => g.Attributes["JOBNUMBER"].ToString() == prevJobNum).ToList();
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //    foreach (Graphic graphicEquip in graphicEquipList)
        //    {
        //        graphicEquip.Attributes["JOBNUMBER"] = newJobNum;

        //        graphicEquip.Attributes["USERAUDIT"] = _user;

        //        graphicEquip.Attributes["LASTMODIFIEDDATE"] = DateTime.Now;
        //    }
        //    return graphicEquipList;
        //}
        //public void GraphicsToPocos()
        //{
        //    _equipment.Clear();
        //    _equipmentDictionary.Clear();
        //    _equipmentByJobNumberDictionary.Clear();

        //    for (int i = 0; i < _equipmentFeatureLayer.Graphics.Count; i++)
        //    {
        //        Graphic graphic = _equipmentFeatureLayer.Graphics[i];

        //        JetEquipment jetEquipment = new JetEquipment();
        //        jetEquipment.ObjectId = Convert.ToInt32(graphic.Attributes["OBJECTID"]);
        //        jetEquipment.EquipTypeId = Convert.ToInt16(graphic.Attributes["EQUIPTYPEID"]);
        //        jetEquipment.JobNumber = Convert.ToString(graphic.Attributes["JOBNUMBER"]);
        //        jetEquipment.OperatingNumber = Convert.ToString(graphic.Attributes["OPERATINGNUMBER"]);
        //        jetEquipment.Cgc12 = Convert.ToString(graphic.Attributes["CGC12"]);
        //        jetEquipment.Address = Convert.ToString(graphic.Attributes["ADDRESS"]);
        //        jetEquipment.City = Convert.ToString(graphic.Attributes["CITY"]);
        //        jetEquipment.SketchLoc = Convert.ToString(graphic.Attributes["SKETCHLOC"]);
        //        jetEquipment.InstallCd = Convert.ToString(graphic.Attributes["INSTALLCD"]);
        //        jetEquipment.Latitude = Convert.ToDouble(graphic.Attributes["LATITUDE"]);
        //        jetEquipment.Longitude = Convert.ToDouble(graphic.Attributes["LONGITUDE"]);

        //        jetEquipment.EntryDate = Convert.ToDateTime(graphic.Attributes["ENTRYDATE"]);
        //        jetEquipment.LastModifiedDate = Convert.ToDateTime(graphic.Attributes["LASTMODIFIEDDATE"]);
        //        jetEquipment.UserAudit = Convert.ToString(graphic.Attributes["USERAUDIT"]);
        //        jetEquipment.CustOwned = Convert.ToString(graphic.Attributes["CUSTOWNED"]);

        //        _equipment.Add(jetEquipment);
        //        _equipmentDictionary.Add(jetEquipment.ObjectId, jetEquipment);

        //        if (_equipmentByJobNumberDictionary.ContainsKey(jetEquipment.JobNumber))
        //        {
        //            _equipmentByJobNumberDictionary[jetEquipment.JobNumber].Add(jetEquipment);
        //        }
        //        else
        //        {
        //            _equipmentByJobNumberDictionary.Add(jetEquipment.JobNumber, new ObservableCollection<JetEquipment>() { jetEquipment });
        //        }
        //    }
        //    _equipment = new ObservableCollection<JetEquipment>(_equipment.OrderBy(j => j.ToString()));

        //}


        //Change code


    }

    public class JetEquipmentType
    {
        public Int16 EquipTypeId { get; set; }
        public string EquipTypeName { get; set; }
        public string HasOperatingNumber { get; set; }
        public string HasCgc12 { get; set; }

        public bool HasOperatingNumberBool
        {
            get
            {
                if (String.IsNullOrEmpty(HasOperatingNumber) || HasOperatingNumber == "0")
                {
                    return false;
                }
                return true;
            }
            set
            {
                HasOperatingNumber = value ? "1" : "0";
            }
        }

        public bool HasCgc12Bool
        {
            get
            {
                if (String.IsNullOrEmpty(HasCgc12) || HasCgc12 == "0")
                {
                    return false;
                }
                return true;
            }
            set
            {
                HasCgc12 = value ? "1" : "0";
            }
        }

        public override string ToString()
        {
            return EquipTypeName;
        }

        //static public JetEquipmentType GraphicToPoco(Graphic graphic)
        //{
        //    JetEquipmentType jetEquipmentType = new JetEquipmentType();
        //    jetEquipmentType.EquipTypeId = Convert.ToInt16(graphic.Attributes["EQUIPTYPEID"]);
        //    jetEquipmentType.EquipTypeName = Convert.ToString(graphic.Attributes["EQUIPTYPEDESC"]);
        //    jetEquipmentType.HasOperatingNumber = Convert.ToString(graphic.Attributes["HASOPERATINGNUM"]);
        //    jetEquipmentType.HasCgc12 = Convert.ToString(graphic.Attributes["HASCGC12"]);

        //    return jetEquipmentType;
        //}

        static public JetEquipmentType GraphicToPoco(EquipmentIdType equipIdType)
        {
            JetEquipmentType jetEquipmentType = new JetEquipmentType();
            jetEquipmentType.EquipTypeId = Convert.ToInt16(equipIdType.EquipTypeId);
            jetEquipmentType.EquipTypeName = Convert.ToString(equipIdType.EquipTypeDesc);
            jetEquipmentType.HasOperatingNumber = Convert.ToString(equipIdType.HasOperatingNum);
            jetEquipmentType.HasCgc12 = Convert.ToString(equipIdType.HasCGC12);

            return jetEquipmentType;
        }
    }
    public class ChangeJobNumberJetEquipment
    {

        public string JobNumber { get; set; }
        public string PreviousJobNumber { get; set; }

        public DateTime LastModifiedDate { get; set; }
        public string UserAudit { get; set; }

        public DateTime LastModifiedDateLocal
        {
            get
            {
                return LastModifiedDate.ToLocalTime();
            }
        }
    }

    public class JetEquipment
    {
        public int ObjectId { get; set; }
        public Int16 EquipTypeId { get; set; }
        public string JobNumber { get; set; }
        public string OperatingNumber { get; set; }
        public string Address { get; set; }
        public string SketchLoc { get; set; }
        public string InstallCd { get; set; }
        public string City { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string CustOwned { get; set; }
        public string LastEquipLoc { get; set; }      //for Use Last Equipment Location checkbox - INC000004150734
        public string Cgc12 { get; set; }

        public bool HasCgc12 { get; set; }
        public bool HasOperatingNumber { get; set; }

        public DateTime EntryDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public string UserAudit { get; set; }

        public DateTime LastModifiedDateLocal
        {
            get
            {
                return LastModifiedDate.ToLocalTime();
            }
        }

        public bool CustOwnedBool
        {
            get
            {
                if (String.IsNullOrEmpty(CustOwned) || CustOwned == "0")
                {
                    return false;
                }
                return true;
            }
            set
            {
                CustOwned = value ? "1" : "0";
            }
        }

        //for Use Last Equipment Location checkbox - INC000004150734
        public bool LastEquipLocBool
        {
            get
            {
                if (String.IsNullOrEmpty(LastEquipLoc) || LastEquipLoc == "0")
                {
                    return false;
                }
                return true;
            }
            set
            {
                LastEquipLoc = value ? "1" : "0";
            }
        }

        public string EquipTypeName
        {
            get
            {
                return JetEquipmentModel._equipmentIdTypeValues[EquipTypeId].EquipTypeName;
            }
        }
        public string InstallCdName
        {
            get
            {
                return JetEquipmentModel._jetInstallTypes[InstallCd];
            }
        }

        public string ToCsv()
        {
            return EquipTypeName + "," + OperatingNumber + "," + Cgc12 + "," + InstallCdName + "," + SketchLoc + "," +
                   Address.Replace(",", " ") + "," + City.Replace(",", "") + "," + Latitude + "," + Longitude;
        }

        static public string HeadersToCsv()
        {
            return "Equipment,Operating Number,CGC12,Install Type,Sketch Location,Address,City,Latitude,Longitude";
        }
        //public override string ToString()
        //{
        //    return JobNumber + " " + Description + " (" + DivisionName + ")";
        //}
    }

}
