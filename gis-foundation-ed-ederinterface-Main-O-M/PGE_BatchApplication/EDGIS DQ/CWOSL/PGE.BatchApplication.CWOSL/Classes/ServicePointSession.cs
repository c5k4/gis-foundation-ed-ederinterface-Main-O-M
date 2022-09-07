using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Location;
using ESRI.ArcGIS.GISClient;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF;
//using Telvent.Delivery.Diagnostics;
//using Miner.Interop.Process;
//using Miner.Process.GeodatabaseManager.Subtasks;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Common.ChangesManagerShared;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;

namespace PGE.BatchApplication.CWOSL.Classes
{
    class ServicePointSession
    {
        #region Private Variables
        private static readonly Log4NetLoggerCWOSL _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config");
        private MinerSession _minerSession = null;
        private ITable _servicePointTable = null;
        private IFeatureClass _serviceLocationFC = null;
        private IFeatureClass _secOHConductorFC = null;
        private IFeatureClass _secUGConductorFC = null;
        private IRelationshipClass _servLoc_SevPoint_relationshipClass = null;
        private IRelationshipClass _secLoad_SevPoint_relationshipClass = null;
        private PGE.Common.ChangesManagerShared.SDEWorkspaceConnection _sdeWorkspaceConnection = null;
        private AdoOleDbConnection _sessionManagerOleDbConnection = null;
        private string _sessionname = null;
        private int MaxMinutesToRun { get; set; }
        private ArrayList _servicePointsProcessed = new ArrayList();
        private Dictionary<string, List<IFeature>> _parcel_CustomerPoints = new Dictionary<string, List<IFeature>>();
        static private Dictionary<IFeature, IPoint> _parcel_Points = new Dictionary<IFeature, IPoint>();
        private const int DIRECTPOST = 5;
        private const int DISTANCEFROMLABELPOINT = 10;
        //private const double GEOCODEBUFFERDIAMETER = 0.0000022;//Roughly 22 feet.
        #endregion

        #region Static Field Indices
        public static int OH_STATUS_FldIdx { get; set; }
        public static int OH_SUBTYPECD_FldIdx { get; set; }
        public static int OH_JOBNUMBER_FldIdx { get; set; }
        public static int OH_CircuitID_FldIdx { get; set; }
        public static int OH_PhaseD_FldIdx { get; set; }

        public static int UG_STATUS_FldIdx { get; set; }
        public static int UG_SUBTYPECD_FldIdx { get; set; }
        public static int UG_JOBNUMBER_FldIdx { get; set; }
        public static int UG_CircuitID_FldIdx { get; set; }
        public static int UG_PhaseD_FldIdx { get; set; }
        public static int ProcessedCount { get; set; }
        public static int LimitSegUGProcessing { get; set; }
        public static int IgnoreESRIGeocodeError { get; set; }
        public static int ESRIGeocodeScore { get; set; }
        public static int BufferGeocodeInFeet { get; set; }
        public static int MaxParcelCustomerPoints { get; set; }
        public static int MaxParcelArea { get; set; }
        //1368195
        public static string SketchPreFeature { get; set; }
        public Initialize InitializeProcess { get; set; }
        #endregion

        #region Count Variables
        private int _servicePointInserts = 0;
        private int _seudoServiceInserts = 0;
        private int _servicePointRelates = 0;
        private int _relatedtoSegUGService = 0;

        private int _incompleteAddress = 0;
        private int _failedtoGeocode = 0;
        private int _failedtoFindparcel = 0;

        private int _transformerGreaterthan = 0;
        private int _failedToFindTransformer = 0;
        private int _failedToRelateServicePointTo_secondary_load_point = 0;
        private int _esriGeocodeServiceCalled = 0;
        private static int _failedToMeetGeocodeScore = 0;
        private static int _closestParcelsUsed = 0;
        private static int _laregeParcelSkipped = 0;
        #endregion

        public ServicePointSession(PGE.Common.ChangesManagerShared.SDEWorkspaceConnection sdeWorkspaceConnection, AdoOleDbConnection sessionManagerOleDbConnection, string sessionName)
        {
            _minerSession = null;
            _servicePointTable = null;
            _secOHConductorFC = null;
            _secUGConductorFC = null;
            _servLoc_SevPoint_relationshipClass = null;
            _secLoad_SevPoint_relationshipClass = null;
            _sdeWorkspaceConnection = sdeWorkspaceConnection;
            _sessionManagerOleDbConnection = sessionManagerOleDbConnection;
            _sessionname = sessionName;
            _parcel_CustomerPoints = new Dictionary<string, List<IFeature>>();

            _servicePointInserts = 0;
            _seudoServiceInserts = 0;
            _servicePointRelates = 0;
            _relatedtoSegUGService = 0;
            _incompleteAddress = 0;
            _failedtoGeocode = 0;
            _failedtoFindparcel = 0;
            _transformerGreaterthan = 0;
            _failedToFindTransformer = 0;
            _failedToRelateServicePointTo_secondary_load_point = 0;
            _failedToMeetGeocodeScore = 0;
            _closestParcelsUsed = 0;
            _laregeParcelSkipped = 0;
        }

        //FeederWorkspaceExtension instance = null;
        public void ProcessSession(ArrayList servicePointsList, int maxMtsToRun)
        {
            try
            {
                //Create session
                if (!CreateSession()) return;
                Init();

                //Start editing
                if (!StartEditing()) return;
                //Initialize feeder manager extension
                //ConnectionProperties connectionProps = new ConnectionProperties(_minerSession.Workspace);
                //instance = FeederWorkspaceExtension.GetInstance(connectionProps);
                //instance.OnStartEditing(false);
                //IVersion version = ((IVersion)_minerSession.Version

                //instance.OnStartEditing(false);

                //Process the servicepoints in the array list
                ProcesssServiceLocations(servicePointsList, maxMtsToRun);

                //Populate CWOSL
                Insert_CWOSL(_servicePointsProcessed);

                //Stop and save editing
                if (!StopEditing(true)) return;

                ////Insert rows for SDE.MM_Edited_Features table. For some reason feeder manager AUs are not taking care of this
                InsertMMEditedFeatures();

                //instance.OnStopEditing(true);
                LogProcessedCounts();

                //Post session
                //ME Q1 - 2020 : Commented the code , No need to send for posting
                //if (!Debugger.IsAttached)
                //{
                //    if (!SubmitSessonToDirectPost()) return;
                //}
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        private void InsertMMEditedFeatures()
        {
            _logger.Info("Logging edited rows to the SDE.MM_EDITED_FEATURES table");

            try
            {
                int featuresAdded = 0;
                IVersionedWorkspace versionWS = _minerSession.Workspace as IVersionedWorkspace;

                _logger.Info("     Initializing version differences");
                DifferenceManager differenceManager = new DifferenceManager(versionWS.DefaultVersion, _minerSession.Version);
                differenceManager.Initialize();

                _logger.Info("     Loading differences");
                esriDataChangeType[] differeces = { esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate };
                differenceManager.LoadDifferences(false, differeces);

                _logger.Info("     Determining inserts");
                DifferenceDictionary insertDictionary = differenceManager.Inserts;
                Dictionary<string, DiffTable>.Enumerator inserts = insertDictionary.GetEnumerator();

                Dictionary<int, string> tableNameByClassID = new Dictionary<int, string>();
                Dictionary<int, List<int>> editedOIDsByClassID = new Dictionary<int, List<int>>();
                Dictionary<string, List<int>> editedOIDsByTableName = new Dictionary<string, List<int>>();
                while (inserts.MoveNext())
                {
                    _logger.Info("     Processing inserts for " + inserts.Current.Key);
                    editedOIDsByTableName.Add(inserts.Current.Key, new List<int>());
                    foreach (DiffRow diffRow in inserts.Current.Value)
                    {
                        editedOIDsByTableName[inserts.Current.Key].Add(diffRow.OID);
                    }

                    _logger.Info("          " + editedOIDsByTableName[inserts.Current.Key].Count + " inserts were found");
                }

                _logger.Info("     Determining Updates");
                DifferenceDictionary updatedDictionary = differenceManager.Updates;
                Dictionary<string, DiffTable>.Enumerator updates = updatedDictionary.GetEnumerator();
                while (updates.MoveNext())
                {
                    _logger.Info("     Processing updates for " + updates.Current.Key);
                    if (!editedOIDsByTableName.ContainsKey(updates.Current.Key)) { editedOIDsByTableName.Add(updates.Current.Key, new List<int>()); }
                    foreach (DiffRow diffRow in updates.Current.Value)
                    {
                        editedOIDsByTableName[updates.Current.Key].Add(diffRow.OID);
                    }

                    _logger.Info("          " + editedOIDsByTableName[updates.Current.Key].Count + " updates were found");
                }

                foreach (KeyValuePair<string, List<int>> kvp in editedOIDsByTableName)
                {
                    ITable table = ((IFeatureWorkspace)_minerSession.Version).OpenTable(kvp.Key);
                    if (!(table is INetworkClass))
                    {
                        _logger.Info(string.Format("{0} is not a network feature class and will be skipped", kvp.Key));
                        continue;
                    }
                    List<int> oids = kvp.Value.Distinct().ToList();
                    _logger.Info("     Inserting " + oids.Count + " records for " + kvp.Key);

                    int objetClassID = ((IObjectClass)table).ObjectClassID;
                    foreach (int oid in oids)
                    {
                        string sql = string.Format("INSERT INTO SDE.MM_EDITED_FEATURES VALUES({0}, {1}, '{2}')", objetClassID, oid, _minerSession.Version.VersionName);
                        _logger.Debug(string.Format("     Executing sql: {0}", sql));
                        _minerSession.Workspace.ExecuteSQL(sql);
                        _minerSession.Workspace.ExecuteSQL("COMMIT");
                        featuresAdded++;
                    }
                }

                _logger.Info(string.Format("     {0} rows were inserted into the SDE.MM_EDITED_FEATURES table for processing", featuresAdded));
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to insert into the SDE.MM_EDITED_FEATURES table: " + ex.Message + " StackTrace: " + ex.StackTrace);
                throw ex;
            }
        }

        private void ProcesssServiceLocations(ArrayList servicePointsList, int maxMtsToRun)
        {
            IFeatureCursor featCur = null;
            _servicePointsProcessed = new ArrayList();
            try
            {
                IFeature parcelFeature = null;
                for (int i = 0; i < servicePointsList.Count; i++)
                {
                    if (HelperCls.ApplicationHasExceededMaxMinutesToRun(Initialize.AppStartDateTime, maxMtsToRun))
                        break;//Graceful exit if time limit reached.
                    ProcessedCount = i;
                    ServicePoint sevPoint = servicePointsList[i] as ServicePoint;
                    _servicePointsProcessed.Add(sevPoint);

                    _logger.Info(i.ToString() + "). Processing Service Point, service point OID: " + sevPoint.ServicePointOID.ToString());
                    if (sevPoint.AddressMissing)
                    {
                        _logger.Info("Address component/s missing. Address: " + sevPoint.Address);
                        _incompleteAddress++;
                        continue;
                    }
                    //Check if service point address matches parcel address.
                    _logger.Info("ServicePoint Address geocoding against: " + sevPoint.Address);
                    parcelFeature = GetMatchedParcelFeature(sevPoint);
                    string parcelMsg = null;
                    if (parcelFeature == null)
                    {
                        //Parcel not found, use ESRI geocoder to find parcel.                        
                        if (!ESRI_GeocodeService(sevPoint.Address, out parcelFeature, out parcelMsg))
                        {
                            ////Geocode error occured, check config value, and exit process if true, or else continue to next service point.
                            //if (ServicePointSession.IgnoreESRIGeocodeError==1)
                            //{
                            //    //Remove servicePoint from arraylist, 
                            //    _servicePointsProcessed.Remove(sevPoint);
                            //    continue;
                            //}
                        }
                        _esriGeocodeServiceCalled++;
                    }
                    if (parcelFeature == null)
                    {
                        //Could not find parcel either in landbase or through ESRI geocode, write status and process next service point.
                        if (string.IsNullOrEmpty(parcelMsg))
                        {
                            parcelMsg = "Failed to find parcel";
                            _failedtoFindparcel++;
                        }
                        _logger.Info(parcelMsg + ": " + sevPoint.Address);
                        sevPoint.Status = parcelMsg;
                        continue;
                    }
                    _logger.Info("Parcel OID: " + parcelFeature.OID.ToString());
                    //Means parcel found.
                    //Check for existing customer point in parcel.
                    if (_serviceLocationFC == null)
                    {
                        _serviceLocationFC = ((IFeatureWorkspace)_minerSession.Workspace).OpenFeatureClass(HelperCls.FC_ServiceLocationName);
                        InitFields();
                    }
                    DetermineCustomerPointFC(sevPoint);
                    IFeatureClass customerPointFC = _serviceLocationFC;
                    if (sevPoint.IsSecondaryLoadPoint)
                        customerPointFC = Initialize.SecondaryLoadPointFC;

                    //Add Parcel to dictionary.
                    List<IFeature> customerPoints = new List<IFeature>();
                    bool parcelFirstTimeProcess = false;
                    parcelFirstTimeProcess = GetParcel_CustomerPoints(ref _parcel_CustomerPoints, parcelFeature.OID.ToString(), out customerPoints);

                    if (!GetParcel_CustomerPoints(ref _parcel_CustomerPoints, parcelFeature.OID.ToString(), out customerPoints))
                    {
                        customerPoints = new List<IFeature>();

                        //Perform spatial query against the parcel to find the location feature/s.
                        featCur = HelperCls.SpatialQuery(customerPointFC, parcelFeature.ShapeCopy, esriSpatialRelEnum.esriSpatialRelIntersects, null);
                        IFeature customerPoint = featCur.NextFeature();
                        while (customerPoint != null)
                        {
                            customerPoints.Add(customerPoint);
                            customerPoint = featCur.NextFeature();
                        }

                        //Add parcel and customer poitns within the parcel to dictionary.
                        _logger.Info("Number of existing customer points on the Parcel: " + customerPoints.Count.ToString());
                        _parcel_CustomerPoints.Add(parcelFeature.OID.ToString(), customerPoints);

                        if (featCur != null)
                            while (Marshal.ReleaseComObject(featCur) > 0) { }
                    }

                    if (customerPoints.Count == 0)
                    {
                        if (sevPoint.IsSecondaryLoadPoint) //If secondary load point, then end and log status.
                        {
                            //write status and End.
                            string msg = "Cannot relate service point. Secondary load point does not exist.";
                            sevPoint.Status = msg;
                            _logger.Info(msg);
                            _failedToRelateServicePointTo_secondary_load_point++;
                            continue;
                        }
                        //Failed to find Customer point, now use centroid of parcel to measure distance to transformer.
                        Process_Using_ParcelCentroid(parcelFeature, ref sevPoint);
                    }
                    else
                    {
                        //Found Customer point/s.
                        if (Skip_LargeParcelWithLargeServiceLocaitons(parcelFeature.ShapeCopy, customerPoints.Count, ref sevPoint))
                        {
                            //Large parcel detected with high number of customer points.
                            continue;
                        }
                        Process_Using_CustomerPoint(parcelFeature, customerPointFC, sevPoint, customerPoints);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            finally
            {
                if (featCur != null)
                    while (Marshal.ReleaseComObject(featCur) > 0) { }
            }
        }

        private void LogProcessedCounts()
        {
            _logger.Info("----------------------------------------------------------");
            _logger.Info("----------------------------------------------------------");
            _logger.Info("Service Locations Created: " + _servicePointInserts.ToString());
            _logger.Info("Pseudo Services Created: " + _seudoServiceInserts.ToString());
            _logger.Info("Related to SeccondaryUG Services: " + _relatedtoSegUGService.ToString());
            _logger.Info("Service Points Related: " + _servicePointRelates.ToString());

            _logger.Info("Probable Multitenant Parcels(closest Parcel taken): " + _closestParcelsUsed.ToString());
            _logger.Info("Incomplete addresses: " + _incompleteAddress.ToString());
            _logger.Info("Failed to meet Geocode score: " + _failedToMeetGeocodeScore.ToString());
            _logger.Info("Failed to geocode: " + _failedtoGeocode.ToString());
            _logger.Info("Failed to find Parcel: " + _failedtoFindparcel.ToString());
            _logger.Info("Failed to find Parcel: " + _failedtoFindparcel.ToString());
            _logger.Info("Probable Subdivision issue(Large Parcel Skipped): " + _laregeParcelSkipped.ToString());

            _logger.Info("Transformer greater than 850 ft: " + _transformerGreaterthan.ToString());
            _logger.Info("Failed to find Transformer: " + _failedToFindTransformer.ToString());
            _logger.Info("Failed to relate to Secondary Load Point: " + _failedToRelateServicePointTo_secondary_load_point.ToString());
            _logger.Info("----------------------------------------------------------");
            _logger.Info("----------------------------------------------------------");
        }

        #region Search for features function
        private IFeature GetMatchedParcelFeature(ServicePoint serPoint)
        {
            IFeatureCursor curParcel = null;
            try
            {
                //select * from lbgis.parcels where 
                //string sqlSP = "SITE_ADDRESS = '5298 MATTHEW TER' AND PROPERTY_CITY = 'FREMONT' AND PROPERTY_STATE = 'CA'";
                string sqlSP = HelperCls.Parcel_SITE_ADDRESS + " = '" + serPoint.StreetNumber + " " + serPoint.StreetName1 + "' AND " +
                               HelperCls.Parcel_PROPERTY_CITY + " = '" + serPoint.City + "' AND " +
                               HelperCls.Parcel_PROPERTY_STATE + " = '" + serPoint.State + "'";
                HelperCls.GetFeatureCursor(out curParcel, Initialize.Parcels, sqlSP);
                if (curParcel == null)
                {
                    _logger.Debug("Zero records returned when searching Parcel FC. SQL: " + sqlSP);
                    return null;
                }
                IFeature parcelRow = curParcel.NextFeature();
                if (parcelRow == null) _logger.Debug("Zero records returned when searching Parcel FC. SQL: " + sqlSP);
                return parcelRow;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return null;
            }
            finally
            {
                if (curParcel != null)
                    while (Marshal.ReleaseComObject(curParcel) > 0) { }
            }
        }

        private void DetermineCustomerPointFC(ServicePoint serPoint)
        {
            ICursor cur = null;
            try
            {
                //Return Service Loction FC or Secondary Load Point FC.
                //Determine wheather to use Service Location FC or Secondary Load Point FC

                //select * from edgis.transformer where cgc12 = '599999845086' and subtypecd = 5;
                string sqlSP = HelperCls.CGC12_FldName + " = " + serPoint.CGC12 + " AND " +
                               HelperCls.SubTypeCD_FldName + " = " + HelperCls.TransformerSubTypeNetwork; ;
                HelperCls.GetCursor(out cur, Initialize.TransformerFC as ITable, sqlSP);
                if (cur == null)
                {
                    //_logger.Debug("Zero records returned when searching Transformer FC. SQL: " + sqlSP);
                    return;
                }
                IRow trans = cur.NextRow();
                if (trans == null)
                {
                    //_logger.Debug("Zero records returned when searching Parcel FC. SQL: " + sqlSP);
                    return;
                }
                serPoint.TransformerOID = trans.OID;
                serPoint.IsSecondaryLoadPoint = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                if (cur != null)
                    while (Marshal.ReleaseComObject(cur) > 0) { }
            }
        }

        private void CGC12_TransformerSearch(ref ServicePoint serPoint, out IPoint transformerShape)
        {
            IFeatureCursor cur = null;
            transformerShape = null;
            try
            {
                //Return Transformer shape.
                //select * from edgis.transformer where cgc12 = 599999845086;
                string sql = HelperCls.CGC12_FldName + " = " + serPoint.CGC12;
                HelperCls.GetFeatureCursor(out cur, Initialize.TransformerFC, sql);
                if (cur == null)
                    return;

                IFeature trans = cur.NextFeature();
                if (trans == null)
                    return;
                serPoint.TransformerOID = trans.OID;
                transformerShape = trans.ShapeCopy as IPoint;
                serPoint.TransformerType = HelperCls.GetValue(trans as IRow, Initialize.TransformerInstallationTyp_FldIdx);
                serPoint.CircuitID = HelperCls.GetValue(trans as IRow, Initialize.TransformerCircuitID_FldIdx);
                serPoint.PhaseDesignation = HelperCls.GetValue(trans as IRow, Initialize.PhaseDesignation_FldIdx);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                if (cur != null)
                    while (Marshal.ReleaseComObject(cur) > 0) { }
            }
        }
        #endregion

        #region Add to CWOSL Tble
        private void Insert_CWOSL(ArrayList servicePointsList)
        {
            try
            {
                for (int i = 0; i < servicePointsList.Count; i++)
                {
                    ServicePoint sevPoint = servicePointsList[i] as ServicePoint;
                    string resolvedStaus = "N";

                    if (sevPoint.DelayProcessing)
                    {
                        //log with delay status.
                        resolvedStaus = "D";
                    }
                    if (string.IsNullOrEmpty(sevPoint.Status))
                    {
                        resolvedStaus = "Y";
                    }
                    PopulateCWOSL(resolvedStaus, sevPoint.ServicePointID, sevPoint.Status);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
        private void PopulateCWOSL(string resolvedValue, string servicePointID, string comments)
        {
            try
            {
                //"insert into CWOSL " +
                //"(objectid, servicepointid, createdate)" +
                // " values " +
                //" (SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS', (select registration_id from sde.table_registry where table_name= 'CWOSL')), " + 
                //"'" +417672251290 +"'" + ", (To_Date('09/21/2017', 'mm/dd/yyyy')))";
                DateTime datNow = DateTime.Today;
                string dateValue = datNow.ToString("MM/dd/yyyy");

                string sqlCWOSL = "insert into EDGIS.CWOSL " +
                                  "(objectid, servicepointid, createdate, resolved, comments)" +
                                  " values " +
                                  " (SDE.VERSION_USER_DDL.NEXT_ROW_ID('EDGIS', (select registration_id from sde.table_registry where table_name= 'CWOSL')), " +
                                  "'" + servicePointID + "'" + ", (To_Date('" + dateValue + "', 'mm/dd/yyyy')), '" + resolvedValue + "', '" + comments + "')";


                _minerSession.Workspace.ExecuteSQL(sqlCWOSL);
                _minerSession.Workspace.ExecuteSQL("COMMIT");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
        #endregion

        #region Session Related Functions
        private bool CreateSession()
        {
            try
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);
                _minerSession = new MinerSession(_sdeWorkspaceConnection, _sessionManagerOleDbConnection);
                _minerSession.CreateMMSessionVersion(_sessionname);
                _logger.Info("Session created - version name: " + _minerSession.Version.VersionName);
                _logger.Info("Session name: " + _minerSession.SessionName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message); return false;
            }
            return true;
        }

        private bool StartEditing()
        {
            try
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);
                if (_minerSession != null && _minerSession.Workspace != null)
                {
                    HelperCls.EnsureInEditSession(_minerSession.Workspace);
                    StartOperation();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message); return false;
            }
            return false;
        }
        private bool StopEditing(bool saveEdits)
        {
            try
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);
                if (_minerSession != null && _minerSession.Workspace != null)
                {
                    StopOperation();
                    HelperCls.EnsureCloseEditSession(_minerSession.Workspace, saveEdits);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message); return false;
            }
            return false;
        }
        private void StartOperation()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (_minerSession != null && _minerSession.Workspace != null)
                HelperCls.StartEditOperation(_minerSession.Workspace);
        }
        private void StopOperation()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (_minerSession != null && _minerSession.Workspace != null)
                HelperCls.StopEditOperation(_minerSession.Workspace);
        }
        private bool SubmitSessonToDirectPost()
        {
            try
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);

                if (_minerSession != null)
                {
                    _minerSession.SubmitDirectly_To_Post_InGDBM(DIRECTPOST);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message); return false;
            }
            return false;
        }

        #endregion

        #region Geocode Functions
        static private bool ESRI_GeocodeService(string address, out IFeature parcelFeature, out string messageLog)
        {
            IProximityOperator proxOp = null;
            parcelFeature = null;
            messageLog = null;
            try
            {
                _parcel_Points = _parcel_Points = new Dictionary<IFeature, IPoint>();
                string singleLineField = "";
                ILocator agoLocator = GetAGSLocator();
                if (agoLocator is ISingleLineAddressInput)
                {
                    singleLineField = (agoLocator as ISingleLineAddressInput).SingleLineAddressField.Name;
                }
                // Set the Address that will be geocoded
                IPropertySet addressProperties = new PropertySetClass();
                // Get the address inputs for the locator
                IAddressInputs addressInputs = agoLocator as IAddressInputs;
                addressProperties.SetProperty(singleLineField, address);

                IAddressGeocoding addressGeocoding = agoLocator as IAddressGeocoding;
                IPropertySet propertySet = addressGeocoding.MatchAddress(addressProperties);
                if (propertySet != null)
                {
                    IPropertySet2 candidatePropertySet = propertySet as IPropertySet2;
                    //OutputPropertySet(candidatePropertySet);
                    string matchingAdress = null;
                    IPoint foundPoint = null;
                    PropertySet2ToGeocodeResult(candidatePropertySet, out foundPoint, out matchingAdress, out messageLog);

                    //Get parcels after buffering process.
                    if (foundPoint != null)
                    {
                        IPoint pointGeocode = null;
                        pointGeocode = GetBufferredParcel(foundPoint);
                        _logger.Info("No. of Polygon features found after buffering process: " + _parcel_Points.Count.ToString());
                        if (_parcel_Points.Count < 1)
                        {
                            foundPoint = null;
                            return true;
                        }

                        //Now buffer the found point, and grac label-points of all Parcels,
                        //then reverse geocode to determine the right Parcel.
                        IPoint reversePoint = null;
                        foreach (KeyValuePair<IFeature, IPoint> pair in _parcel_Points)
                        {
                            reversePoint = pair.Value as IPoint;
                            if (reversePoint != null)
                            {
                                IReverseGeocoding reverGeocoding = agoLocator as IReverseGeocoding;
                                IPropertySet propertySetReverse = reverGeocoding.ReverseGeocode(reversePoint, false);
                                if (propertySetReverse != null)
                                {
                                    IPropertySet2 candidatePropertySetReverse = propertySetReverse as IPropertySet2;
                                    string reverseMatchingAddress = null;
                                    //reversePoint = (IPoint)candidatePropertySetReverse.GetProperty("Shape");
                                    //_logger.Info("ESRI Geocode Score: " + score.ToString());
                                    reverseMatchingAddress = (string)candidatePropertySetReverse.GetProperty("Match_addr");
                                    _logger.Info("Parcel OID and Reverse Address: " + pair.Key.OID.ToString() + ", " + reverseMatchingAddress);
                                    if (!string.IsNullOrEmpty(reverseMatchingAddress))
                                    {
                                        if (!string.IsNullOrEmpty(matchingAdress))
                                        {
                                            if (matchingAdress.ToUpper().Equals(reverseMatchingAddress.ToUpper()))
                                            {
                                                //Check if parcel-labelpoint intersects LotLines.
                                                if (DoesIntersectLotlines(reversePoint, true))
                                                {
                                                    //Through reverse geocoding, addresses match, and lotline intersects, correct parcel is found.
                                                    parcelFeature = pair.Key as IFeature;
                                                    return true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //Reverse Geocode did not find the address, now get the closest parcel to Geocode point.
                        proxOp = (IProximityOperator)pointGeocode;
                        double closestDistance = 999999;
                        IFeature currentParcel = null;
                        parcelFeature = null;
                        foreach (KeyValuePair<IFeature, IPoint> pair in _parcel_Points)
                        {
                            //Check if parcel-labelpoint intersects LotLines.
                            reversePoint = pair.Value as IPoint;
                            if (DoesIntersectLotlines(reversePoint, false))
                            {
                                currentParcel = pair.Key as IFeature;
                                bool iscloser = false;
                                if (currentParcel != null)
                                {
                                    iscloser = HelperCls.GetClosestFeatureToPoint(proxOp, currentParcel.ShapeCopy, ref closestDistance);
                                    if (iscloser)
                                        parcelFeature = currentParcel; //Probable Multitenant Parcel
                                }
                            }
                        }
                        if (parcelFeature != null)
                        {
                            messageLog = "Probable Multitenant Parcel";
                            _logger.Info(messageLog + ", parcel distance to point: " + closestDistance.ToString());
                            _closestParcelsUsed++;
                            return true;
                        }
                    }
                }
                //If here, means reverse geocoding could not find the right parcel.
                parcelFeature = null;//
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return false;
            }
            finally
            {
                if (proxOp != null)
                    while (Marshal.FinalReleaseComObject(proxOp) > 0) { }
            }
            return true;
        }
        static private ILocator GetAGSLocator()
        {
            Console.WriteLine("GetAGSLocator()");

            ILocator agoLocator = null;
            IPropertySet connectionProperties = new PropertySetClass();
            connectionProperties.SetProperty("Url", "https://geocode.arcgis.com/arcgis/services");

            // Open a workspace with an ArcGIS Server connection
            System.Object obj = Activator.CreateInstance(Type.GetTypeFromProgID("esriGISClient.AGSServerConnectionFactory"));
            IAGSServerConnectionFactory AGSServerConnectionFactory = obj as AGSServerConnectionFactory;
            IAGSServerConnection AGSServerConnection = AGSServerConnectionFactory.Open(connectionProperties, 0);

            // Get the locator workspace from the open workspace
            obj = Activator.CreateInstance(Type.GetTypeFromProgID("esriLocation.LocatorManager"));
            ILocatorManager2 locatorManager = obj as ILocatorManager2;
            IAGSServerConnectionName agsServerConnectionName = AGSServerConnection.FullName as IAGSServerConnectionName;
            ILocatorWorkspace locatorWorkspace = locatorManager.GetAGSLocatorWorkspace(agsServerConnectionName);
            //agoLocator = locatorWorkspace.GetLocator("Locators/TA_Address_NA");
            //agoLocator = locatorWorkspace.GetLocator("USA Streets");
            agoLocator = locatorWorkspace.GetLocator("World");
            if (agoLocator is ISingleLineAddressInput)
            {
                Console.WriteLine((agoLocator as ISingleLineAddressInput).SingleLineAddressField.Name);
            }
            return agoLocator;
        }
        static private void PropertySet2ToGeocodeResult(IPropertySet2 candidatePropertySet, out IPoint foundPoint, out string matchingAdress, out string messageLog)
        {
            foundPoint = null;
            matchingAdress = null;
            messageLog = null;
            //OutputPropertySet(candidatePropertySet);

            bool isTied = ((string)candidatePropertySet.GetProperty(HelperCls.geocoderFieldStatus)) == HelperCls.geocoderValueTied;
            if (isTied)
            {
                _logger.Info("Tied address.");
            }

            // NB if it's a tie the caller can deal with it
            if (((string)candidatePropertySet.GetProperty(HelperCls.geocoderFieldStatus)) == HelperCls.geocoderValueMatched)
            {
                int score = Convert.ToInt32(candidatePropertySet.GetProperty("Score"));
                _logger.Info("ESRI Geocode Score: " + score.ToString());
                if (score < ServicePointSession.ESRIGeocodeScore)
                {
                    _failedToMeetGeocodeScore++;
                    messageLog = "Failed to meet Geocode score";
                    return;
                }
                foundPoint = (IPoint)candidatePropertySet.GetProperty("Shape");
                matchingAdress = (string)candidatePropertySet.GetProperty("Match_addr");
                //Console.WriteLine(string.Format("Matched [ {0} ] with score [ {1} ] isTied [ {2} ]", matchingAdress, score.ToString(),
                //             isTied));
                //    //geocodeResult = new GeocodeResult(score, point, locatorType, matchingAdress, isTied);
            }
            else // Unmatched! Couldn't find it...
            {
                _logger.Info("Failed to match address.");
            }
        }
        static private void OutputPropertySet(IPropertySet propertySet)
        {
            object propertyNames;
            object propertyValues;
            propertySet.GetAllProperties(out propertyNames, out propertyValues);

            System.Array propNameArray = (System.Array)propertyNames;
            System.Array propValuesArray = (System.Array)propertyValues;

            for (int i = 0; i < propNameArray.Length; i++)
            {
                Console.WriteLine("[ {0} ] = [ {1} ]", propNameArray.GetValue(i), propValuesArray.GetValue(i));
                _logger.Info(propNameArray.GetValue(i).ToString() + ": " + propValuesArray.GetValue(i).ToString());
            }
        }
        static private IPoint GetBufferredParcel(IPoint geocodedPoint)
        {
            IFeatureCursor curParcels = null;
            IPoint pointUTM = new PointClass();
            //originalGeocodedParcel = null; 
            try
            {
                if (geocodedPoint == null)
                    return null;
                //Get the input and output spatial references 
                int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;
                ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
                ISpatialReference pWGS84_SR = pSRF.CreateGeographicCoordinateSystem(geoType);
                ISpatialReference pUTM10_SR = ((IGeoDataset)Initialize.Parcels).SpatialReference;

                //Set the shape                 
                pointUTM.PutCoords(geocodedPoint.X, geocodedPoint.Y);
                pointUTM.SpatialReference = pWGS84_SR;
                pointUTM.Project(pUTM10_SR);
                //Get Polygon using point in polygon.
                //curParcels = HelperCls.SpatialQuery(Initialize.Parcels, pointUTM, esriSpatialRelEnum.esriSpatialRelIntersects, null);
                //originalGeocodedParcel = curParcels.NextFeature();                
                //if (curParcels != null)
                //    while (Marshal.ReleaseComObject(curParcels) > 0) { }
                //long oringilanOId = 0;
                ////if (originalGeocodedParcel == null)
                ////    return;
                //if (originalGeocodedParcel != null)
                //{
                //    oringilanOId = originalGeocodedParcel.OID;
                //}

                IGeometry geoBuffer = BufferedGeometry(pointUTM);
                if (geoBuffer == null)
                    return null;
                curParcels = HelperCls.SpatialQuery(Initialize.Parcels, geoBuffer, esriSpatialRelEnum.esriSpatialRelIntersects, null);
                if (curParcels == null)
                {
                    _logger.Info("Zero records returned when spatial querying buffered-shape against Parcel FC.");
                    return null;
                }

                IFeature parcel = curParcels.NextFeature();
                if (curParcels == null)
                {
                    _logger.Info("Zero records returned when spatial querying buffered-shape against Parcel FC.");
                    return null;
                }

                while (parcel != null)
                {
                    IArea areaParcel = parcel.ShapeCopy as IArea;
                    _parcel_Points.Add(parcel, areaParcel.LabelPoint);
                    parcel = curParcels.NextFeature();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                if (curParcels != null)
                    while (Marshal.FinalReleaseComObject(curParcels) > 0) { }
            }
            return pointUTM;
        }
        static private IGeometry BufferedGeometry(IPoint pointShape)
        {
            IGeometry bufferedGeom = null;
            ITopologicalOperator topologicalOperator = null;
            try
            {
                if (pointShape == null) return null;
                IClone geometryClone = pointShape as IClone;
                IGeometry geometry = geometryClone.Clone() as IGeometry;
                topologicalOperator = geometry as ITopologicalOperator;

                //Get the buffer distance from config file
                bufferedGeom = topologicalOperator.Buffer(ServicePointSession.BufferGeocodeInFeet);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            finally
            {
                if (topologicalOperator != null) Marshal.FinalReleaseComObject(topologicalOperator);
            }
            return bufferedGeom;
        }
        static private bool DoesIntersectLotlines(IPoint reversePoint, bool writeInfo)
        {
            IFeatureCursor featCur = null;
            try
            {
                //Check if intersects LotLines FC using point in polygon.
                featCur = HelperCls.SpatialQuery(Initialize.Parcels, reversePoint, esriSpatialRelEnum.esriSpatialRelIntersects, null);
                IFeature lotLine = featCur.NextFeature();

                //If lotline intersected, fight polygon feature found, exit.
                if (lotLine != null)
                {
                    if (writeInfo)
                        _logger.Info("Lotline Feature OID: " + lotLine.OID.ToString());
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            finally
            {
                if (featCur != null)
                    while (Marshal.FinalReleaseComObject(featCur) > 0) { }
            }
            return false;
        }
        #endregion

        #region Initialize Functions
        private void Init()
        {
            try
            {
                _servicePointTable = ((IFeatureWorkspace)_minerSession.Workspace).OpenTable(HelperCls.FC_ServicePointName);
                //IPropertySet pset = _minerSession.Workspace.ConnectionProperties;

                //object propertyNames;
                //object propertyValues;
                //pset.GetAllProperties(out propertyNames, out propertyValues);

                //System.Array propNameArray = (System.Array)propertyNames;
                //System.Array propValuesArray = (System.Array)propertyValues;

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }
        private void InitFields()
        {
            try
            {
                if (OH_STATUS_FldIdx < 1)
                {
                    int fldIndex = -1;
                    if (_secOHConductorFC != null)
                    {
                        HelperCls.GetFldIdx((ITable)_secOHConductorFC, HelperCls.STATUS_FldName, out fldIndex);
                        OH_STATUS_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secOHConductorFC, HelperCls.SubTypeCD_FldName, out fldIndex);
                        OH_SUBTYPECD_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secOHConductorFC, HelperCls.INSTALLJOBNUMBER_FldName, out fldIndex);
                        OH_JOBNUMBER_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secOHConductorFC, HelperCls.CircuitID_FldName, out fldIndex);
                        OH_CircuitID_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secOHConductorFC, HelperCls.PHASEDESIGNATION_FldName, out fldIndex);
                        OH_PhaseD_FldIdx = fldIndex;
                    }

                    if (_secUGConductorFC != null)
                    {
                        HelperCls.GetFldIdx((ITable)_secUGConductorFC, HelperCls.STATUS_FldName, out fldIndex);
                        UG_STATUS_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secUGConductorFC, HelperCls.SubTypeCD_FldName, out fldIndex);
                        UG_SUBTYPECD_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secUGConductorFC, HelperCls.INSTALLJOBNUMBER_FldName, out fldIndex);
                        UG_JOBNUMBER_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secUGConductorFC, HelperCls.CircuitID_FldName, out fldIndex);
                        UG_CircuitID_FldIdx = fldIndex;

                        HelperCls.GetFldIdx((ITable)_secUGConductorFC, HelperCls.PHASEDESIGNATION_FldName, out fldIndex);
                        UG_PhaseD_FldIdx = fldIndex;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }
        #endregion

        #region Create Service Line and Customer Point

        private bool GetServiceLineAndPoint_Shape(IPoint transformerPoint, IPoint polygonCentroid, out IPolyline newServiceLine, out IPoint newServicePoint)
        {
            newServicePoint = new PointClass();
            newServiceLine = new PolylineClass();
            try
            {
                IPolyline newTempline = new PolylineClass();
                newTempline.FromPoint = polygonCentroid;
                newTempline.ToPoint = transformerPoint as IPoint;


                ICurve curve = newTempline as ICurve;
                //Get the point 25 feet from centroid of the polygon toward the direction of the transformer.
                curve.QueryPoint(esriSegmentExtension.esriNoExtension, DISTANCEFROMLABELPOINT, false, newServicePoint);

                //Occasionally there may be the potential for a service location to be placed on top of another one.  We need
                //to verify that there is not already a service location here and offset it if so
                while (CoincidentServiceLocation(newServicePoint))
                {
                    //Coincident service location found.  Let's off set by another 0.5 feet
                    newServicePoint.X += 0.5;
                }

                newServiceLine.FromPoint = transformerPoint as IPoint;
                newServiceLine.ToPoint = newServicePoint;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message); return false;
            }
            return true;
        }

        //Check for coincident service location 
        private bool CoincidentServiceLocation(IPoint potentialPoint)
        {
            ISpatialFilter sf = new SpatialFilterClass();
            ITopologicalOperator topOp = potentialPoint as ITopologicalOperator;
            sf.Geometry = topOp.Buffer(0.1);
            sf.GeometryField = Initialize.ServiceLocationFC.ShapeFieldName;
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            int count = Initialize.ServiceLocationFC.FeatureCount(sf);

            if (sf != null) { while (Marshal.ReleaseComObject(sf) > 0) { } }

            return count > 0;
        }

        private bool createPseudoService(IPolyline servieLine, ServicePoint sevPoint)
        {
            try
            {
                IFeature serviceFeature = null;
                int fldIdxStatus = -1;
                int fldIdxJobnumber = -1;
                int fldIdxSubTyp = -1;
                int fldIdxCircuitID = -1;
                int fldIdxPhaseD = -1;
                string subTypValue = HelperCls.PseudoServiceSubtype_OH;
                if (string.IsNullOrEmpty(sevPoint.TransformerType))
                {
                    _logger.Info("Transformer-Type is Null");
                    return false;
                }

                if (sevPoint.TransformerType.Contains(HelperCls.TransformerType_OH))
                {
                    if (_secOHConductorFC == null)
                    {
                        _secOHConductorFC = ((IFeatureWorkspace)_minerSession.Workspace).OpenFeatureClass(HelperCls.FC_SecOHConductofName);
                        InitFields();
                    }
                    serviceFeature = _secOHConductorFC.CreateFeature();
                    fldIdxStatus = OH_STATUS_FldIdx;
                    fldIdxJobnumber = OH_JOBNUMBER_FldIdx;
                    fldIdxSubTyp = OH_SUBTYPECD_FldIdx;
                    fldIdxCircuitID = OH_CircuitID_FldIdx;
                    fldIdxPhaseD = OH_PhaseD_FldIdx;
                }
                else
                {
                    if (_secUGConductorFC == null)
                    {
                        _secUGConductorFC = ((IFeatureWorkspace)_minerSession.Workspace).OpenFeatureClass(HelperCls.FC_SecUGConductofName);
                        InitFields();
                    }
                    serviceFeature = _secUGConductorFC.CreateFeature();
                    fldIdxStatus = UG_STATUS_FldIdx;
                    fldIdxJobnumber = UG_JOBNUMBER_FldIdx;
                    fldIdxSubTyp = UG_SUBTYPECD_FldIdx;
                    fldIdxCircuitID = UG_CircuitID_FldIdx;
                    fldIdxPhaseD = UG_PhaseD_FldIdx;
                    subTypValue = HelperCls.PseudoServiceSubtype_UG;
                }
                HelperCls.SetFldVl(serviceFeature, null, HelperCls.StatusDefault, false, fldIdxStatus);
                HelperCls.SetFldVl(serviceFeature, null, HelperCls.JobNumberDefault, false, fldIdxJobnumber);
                HelperCls.SetFldVl(serviceFeature, null, subTypValue, false, fldIdxSubTyp);
                //HelperCls.SetFldVl(serviceFeature, null, sevPoint.CircuitID, false, fldIdxCircuitID);
                HelperCls.SetFldVl(serviceFeature, null, sevPoint.PhaseDesignation, false, fldIdxPhaseD);

                serviceFeature.Shape = (IGeometry)servieLine;
                serviceFeature.Store();

                /*
                IMMSpecialAUStrategyEx cacheMaintAU = new Miner.Geodatabase.AutoUpdaters.FeederCacheMaintenanceAU();
                bool enabled = cacheMaintAU.get_Enabled(serviceFeature.Table as IObjectClass, mmEditEvent.mmEventFeatureCreate);
                ConnectionProperties connProps = new ConnectionProperties(((IDataset)serviceFeature.Table).Workspace);
                FeederWorkspaceExtension instance = FeederWorkspaceExtension.GetInstance(connProps);
                cacheMaintAU.Execute(serviceFeature, mmAutoUpdaterMode.mmAUMArcMap, mmEditEvent.mmEventFeatureCreate);
                //instance.get
                */
                _logger.Info("Pseudo Service " + serviceFeature.OID.ToString() + " created");
                ConnectFeature(serviceFeature);
                _seudoServiceInserts++;
                if (serviceFeature != null) { while (Marshal.ReleaseComObject(serviceFeature) > 0) { } }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error("Erroe@createPseudoService. Processing sevPoint OID:" + sevPoint.ServicePointOID.ToString() + " Error: " + ex.Message);
            }
            return false;
        }
        private void createServiceLocation(IPoint serviceLocPoint, ServicePoint sevPoint, string parcelOID)
        {
            try
            {
                if (_serviceLocationFC == null)
                {
                    _serviceLocationFC = ((IFeatureWorkspace)_minerSession.Workspace).OpenFeatureClass(HelperCls.FC_ServiceLocationName);
                    InitFields();
                }
                IFeature newLocFeature = _serviceLocationFC.CreateFeature();
                HelperCls.SetFldVl(newLocFeature, null, HelperCls.StatusDefault, false, Initialize.SL_STATUS_FldIdx);
                HelperCls.SetFldVl(newLocFeature, null, HelperCls.SubType_SL, false, Initialize.SL_SUBTYPECD_FldIdx);
                HelperCls.SetFldVl(newLocFeature, null, HelperCls.JobNumberDefault, false, Initialize.SL_JOBNUMBER_FldIdx);
                HelperCls.SetFldVl(newLocFeature, null, sevPoint.PhaseDesignation, false, Initialize.SL_PhaseD_FldIdx);
                HelperCls.SetFldVl(newLocFeature, null, sevPoint.CGC12, false, Initialize.SL_CGC12_FldIdx);
                //HelperCls.SetFldVl(newLocFeature, null, sevPoint.CircuitID, false, Initialize.SL_CircuitID_FldIdx);

                newLocFeature.Shape = (IGeometry)serviceLocPoint;
                newLocFeature.Store();
                _logger.Info("New Service Location " + newLocFeature.OID.ToString() + " created");
                ConnectFeature(newLocFeature);
                _servicePointInserts++;

                //Relate to Service Point.
                RelateServicePointTo_CustomerPoint(sevPoint, _serviceLocationFC, newLocFeature);
                AddCustPntToList(parcelOID, newLocFeature);
            }
            catch (Exception ex)
            {
                _logger.Error("Erroe@createServiceLocation." + ex.Message);
            }
        }
        private void RelateServicePointTo_CustomerPoint(ServicePoint serPoint, IFeatureClass customerPoint_FC, IFeature customerFeat)
        {
            ICursor cur = null;
            try
            {
                IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)_minerSession.Workspace;
                IRow originRow = customerPoint_FC.GetFeature(customerFeat.OID) as IRow;
                IRow destRow = _servicePointTable.GetRow(serPoint.ServicePointOID);

                IDataset dataSet = customerPoint_FC as IDataset;
                if (dataSet.Name.Equals(HelperCls.FC_SecondaryLoadPointName))
                {
                    if (_secLoad_SevPoint_relationshipClass == null)
                        _secLoad_SevPoint_relationshipClass = featureWorkspace.OpenRelationshipClass(HelperCls.RelCls_SecLoadPoint_ServicePointName);
                    IRelationship relationship = _secLoad_SevPoint_relationshipClass.CreateRelationship(originRow as IObject, destRow as IObject);
                }
                else
                {
                    if (_servLoc_SevPoint_relationshipClass == null)
                        _servLoc_SevPoint_relationshipClass = featureWorkspace.OpenRelationshipClass(HelperCls.RelCls_ServiceLocation_ServicePointName);
                    IRelationship relationship = _servLoc_SevPoint_relationshipClass.CreateRelationship(originRow as IObject, destRow as IObject);
                }

                //***                
                //originTable = featureWorkspace.OpenTable("EDGIS.ServiceLocation");
                //destTable = featureWorkspace.OpenTable("EDGIS.ServicePoint");
                //originRow = customerFeat as IRow;// originTable.GetRow(customerFeat.OID);
                //destRow = serPoint.ServicePointRow as IRow;// destTable.GetRow(serPoint.ServicePointRow.OID);
                //IRelationship relationship = relationshipClass.CreateRelationship(originRow as IObject, destRow as IObject);

                //***
                //int GUIDFldIdx = Initialize.ServiceLocGUIDFldIdx;
                //int fromGUID_FldIdx = Initialize.SLoc_GUIDFldIdx;
                //string relatedTable = HelperCls.FC_ServicePointName;                

                //string guidValue = HelperCls.GetValue(customerFeat, fromGUID_FldIdx);
                //Populate GUID field
                //Get the service point from the current session.
                //IRow serviceRow = _servicePointTable.GetRow(serPoint.ServicePointRow.OID);  
                //HelperCls.SetFldVl(serviceRow, null, guidValue, false, GUIDFldIdx);
                //serviceRow.Store();

                //CreateRelatedRow(customerFeat, serPoint.ServicePointRow, relatedTable, true);
                _logger.Info("Service Point " + serPoint.ServicePointOID + " related to Customer Point " + customerFeat.OID.ToString());

            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
            finally
            {
                if (cur != null)
                    while (Marshal.ReleaseComObject(cur) > 0) { }
            }
        }

        #endregion

        #region Existing SecUG in Parcel Process
        private bool Process_SecUGService(IFeature parcelFeature, ServicePoint sevPoint, out bool secUGProcessed)
        {
            secUGProcessed = false;
            try
            {
                if (_secUGConductorFC == null)
                {
                    _secUGConductorFC = ((IFeatureWorkspace)_minerSession.Workspace).OpenFeatureClass(HelperCls.FC_SecUGConductofName);
                    InitFields();
                }
                IPoint serviceEndPnt = null;
                int serviceFound = 0;
                List<IPoint> lstServiceEndPnts = new List<IPoint>();
                //Check any real service exists in the lot. true means service found
                if (GetintersetSecUGConductors(_secUGConductorFC, parcelFeature, ref serviceEndPnt, ref serviceFound, ref lstServiceEndPnts, ref sevPoint))
                {
                    _logger.Info("Number of Seconday-UG/s found: " + lstServiceEndPnts.Count.ToString());
                    for (int i = 0; i < lstServiceEndPnts.Count; i++)
                    {
                        //If serviceLocation not found at the end of service, use end of service end-point to create new Service location.
                        IPoint transformerShape = null;
                        if (HelperCls.findFeedTrans(serviceEndPnt as IPoint, sevPoint.CGC12, null, Initialize.GeometricNetwork, out transformerShape))
                        {
                            //Find Upstream transformer matching with processing servicepoint feeding transformer.
                            _logger.Info("Relating to existing SecUG Service");
                            createServiceLocation(serviceEndPnt, sevPoint, parcelFeature.OID.ToString());
                            secUGProcessed = true;
                            _relatedtoSegUGService++;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EXCP@Error at GetintersetSecUGConductors " + ex.Message); return false;
            }
            return true;
        }
        private bool GetintersetSecUGConductors(IFeatureClass pServiceFC, IFeature pGridFeat, ref IPoint Servicept, ref int serviceFound, ref List<IPoint> lstServiceEndPnts, ref ServicePoint sevPoint)
        {

            IFeature pSrcFeat = null;
            string strSubType = null;
            int intSerFnd = 0, intstrFnd = 0, intEndFnd = 0, UGrowcount = 0;
            try
            {
                IFeatureCursor ConnectedMapCursor = null;
                IGeometry queryGeometry = pGridFeat.ShapeCopy;
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = queryGeometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                //spatialFilter.WhereClause = "subtypecd ='3' and  OBJECTID is not null order by OBJECTID";
                spatialFilter.WhereClause = HelperCls.SubTypeCD_FldName + "='" + HelperCls.ServiceSubtype_UG + "' and  OBJECTID is not null order by OBJECTID";
                spatialFilter.SubFields = "OBJECTID,SUBTYPECD,SHAPE";
                using (ComReleaser comReleaser = new ComReleaser())
                {
                    ConnectedMapCursor = pServiceFC.Search(spatialFilter, true);
                    comReleaser.ManageLifetime(ConnectedMapCursor);
                    UGrowcount = pServiceFC.FeatureCount(spatialFilter);

                    //If high numver of seconday UG services found, then don't process the service and log to CWOSL table with a message and status.
                    if (UGrowcount > LimitSegUGProcessing)
                    {
                        string msg = UGrowcount.ToString() + " Seccondary UGs found. Process delayed.";
                        _logger.Info(msg);
                        sevPoint.Status = msg;
                        sevPoint.DelayProcessing = true;
                        return false;
                    }
                    if ((UGrowcount > 0))
                    {
                        IPolyline Ppoly = null;

                        IRelationalOperator2 relation = pGridFeat.Shape as IRelationalOperator2;
                        //int count = 0;
                        while ((pSrcFeat = ConnectedMapCursor.NextFeature()) != null)
                        {
                            comReleaser.ManageLifetime(pSrcFeat);
                            Ppoly = null;
                            Ppoly = pSrcFeat.ShapeCopy as IPolyline;
                            //strSubType = pSrcFeat.get_Value(clsArcGISfunctionality.GetFieldIndex((IObjectClass)pSrcFeat.Table, "SUBTYPECD")).ToString();
                            strSubType = HelperCls.GetValue(pSrcFeat, UG_SUBTYPECD_FldIdx);
                            if (strSubType == HelperCls.ServiceSubtype_UG)//Service
                            {
                                //Service from and to points are in same grid,So we are checking at both ends
                                if (relation.Contains(Ppoly.FromPoint))
                                {
                                    if (true == functofindjunctions(pSrcFeat, Ppoly.FromPoint))
                                    {
                                        lstServiceEndPnts.Add(Ppoly.FromPoint);
                                        Servicept = Ppoly.FromPoint;
                                        intstrFnd = intstrFnd + 1;
                                        intSerFnd = intSerFnd + 1;
                                    }
                                }
                                if (relation.Contains(Ppoly.ToPoint))
                                {
                                    if (true == functofindjunctions(pSrcFeat, Ppoly.ToPoint))
                                    {
                                        lstServiceEndPnts.Add(Ppoly.ToPoint);
                                        Servicept = Ppoly.ToPoint;
                                        intEndFnd = intEndFnd + 1;
                                        intSerFnd = intSerFnd + 1;
                                    }
                                }
                            }
                        }
                        serviceFound = lstServiceEndPnts.Count;
                        if (intSerFnd > 0)
                        {
                            Servicept = lstServiceEndPnts[0];
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("EXCP@Error at GetintersetSecUGConductors " + ex.Message);
            }
            return false;

        }

        private bool functofindjunctions(IFeature pSrcFeat, IPoint iPoint)
        {
            bool status = false;
            int intLinearCnt_fromPnt = 0;
            int intLinearCnt_EndPnt = 0;
            try
            {
                IPolyline pPolyline = (IPolyline)pSrcFeat.Shape;
                IPointCollection pPointCollection = (IPointCollection)pPolyline;

                getConnectedConductors_AtPnt(pPointCollection.get_Point(0), ref intLinearCnt_fromPnt);
                getConnectedConductors_AtPnt(pPointCollection.get_Point(pPointCollection.PointCount - 1), ref intLinearCnt_EndPnt);

                if (intLinearCnt_fromPnt == 1)
                {
                    //IFeature pEndFeat = (IFeature)pfromJunctionFeature;
                    //IPoint pJuncpt = (IPoint)pEndFeat.Shape;
                    double Dist = HelperCls.GetPointDistance(pPointCollection.get_Point(0), iPoint);
                    if (Dist < 1)
                    {
                        return true;
                    }
                }
                else if (intLinearCnt_EndPnt == 1)
                {
                    //IFeature ptoFeat = (IFeature)pSimpleJunctionFeature;
                    //IPoint pJuncpt = (IPoint)ptoFeat.Shape;
                    double Dist = HelperCls.GetPointDistance(pPointCollection.get_Point(pPointCollection.PointCount - 1), iPoint);
                    if (Dist < 1)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("functofindjunctions: Manual review required for this feature." + pSrcFeat.Class.AliasName + ":OID:" + pSrcFeat.OID + "  " + ex.Message);
            }
            return status;
        }

        /// <summary>
        /// Function to get linear features count from GeonetWork at given input point,
        /// And also validate is there any other featur exists other than netwotk junction(),if otherthan netwotk junction exists consider morefeatures connected 
        /// </summary>
        /// <param name="pTstPnt"></param>        
        /// <param name="intcnt"></param>
        /// <returns></returns>
        private void getConnectedConductors_AtPnt(IPoint pTstPnt, ref int intcnt)
        {
            IFeature pFeat = null;
            intcnt = 0;
            IEnumFeature pEnum = null;
            IEnumFeature pEnum2 = null;
            IEnumFeature pEnum3 = null;
            try
            {
                pEnum = Initialize.GeometricNetwork.SearchForNetworkFeature(pTstPnt, esriFeatureType.esriFTComplexEdge);
                pEnum.Reset();

                pFeat = pEnum.Next();
                while (pFeat != null)
                {
                    intcnt++;
                    pFeat = pEnum.Next();
                }
                pEnum2 = Initialize.GeometricNetwork.SearchForNetworkFeature(pTstPnt, esriFeatureType.esriFTSimpleEdge);
                pEnum2.Reset();

                pFeat = pEnum2.Next();
                while (pFeat != null)
                {
                    intcnt++;
                    pFeat = pEnum2.Next();
                }

                if (intcnt == 1)
                {
                    pEnum3 = Initialize.GeometricNetwork.SearchForNetworkFeature(pTstPnt, esriFeatureType.esriFTSimpleJunction);
                    pEnum3.Reset();
                    pFeat = pEnum.Next();

                    if (pFeat != null)
                    {
                        if (!((IDataset)pFeat.Class).Name.ToUpper().Contains("JUNCTIONS") && ((IDataset)pFeat.Class).Name.ToUpper() != HelperCls.FC_ServiceLocationName.ToUpper())
                        {
                            intcnt = 2; //so this pTstPnt will skip
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("EXCP@getConnectedConductors_AtPnt " + ex.Message);
            }
            finally
            {
                if (pEnum != null)
                {
                    while (Marshal.ReleaseComObject(pEnum) > 0) { }
                    pEnum = null;
                }
                if (pEnum2 != null)
                {
                    while (Marshal.ReleaseComObject(pEnum2) > 0) { }
                    pEnum2 = null;
                }
            }
        }
        #endregion

        #region Process ServicePoint with Parcel-Centroid
        private void Process_Using_ParcelCentroid(IFeature parcelFeature, ref ServicePoint sevPoint)
        {
            try
            {
                //Transformer not found, perform CGC12 tranformer search
                string msg = null;
                IPoint transformerShape = null;
                CGC12_TransformerSearch(ref sevPoint, out transformerShape);
                if (transformerShape == null)
                {
                    msg = "Failed to find Transformer with CGC12 number " + sevPoint.CGC12;
                    _logger.Info(msg);
                    sevPoint.Status = msg;
                    _failedToFindTransformer++;
                    return;
                }

                //Tranformer found
                IArea areaParcel = parcelFeature.ShapeCopy as IArea;
                double transformerDistance = HelperCls.GetPointDistance(transformerShape, (IPoint)areaParcel.Centroid);
                if (transformerDistance > 850)
                {
                    //write satus and End.
                    msg = "Transformer distance greater than 850 feet";
                    _logger.Info(msg);
                    sevPoint.Status = msg;
                    _transformerGreaterthan++;
                    return;
                }

                //Process SecUG service if exists.
                bool secUGProcessed = false;
                Process_SecUGService(parcelFeature, sevPoint, out secUGProcessed);
                if (secUGProcessed) return;
                if (sevPoint.DelayProcessing) return;//process has been delayed due to hihg number of Seg-UGs found.

                //Create Seudo service and service location.
                //Create new Servie geometery and new service location geometry.
                IPolyline newServiceLine = null;
                IPoint newServiceLocPoint = null;
                GetServiceLineAndPoint_Shape(transformerShape, (IPoint)areaParcel.Centroid, out newServiceLine, out newServiceLocPoint);

                //Create new Service
                //1368195 START
                if (ServicePointSession.SketchPreFeature == "INC")
                {
                    if (!createPseudoService(newServiceLine, sevPoint))
                    {
                        //write satus and End.
                        msg = "Failed to create new service.";
                        _logger.Info(msg);
                        sevPoint.Status = msg; ;
                        return;
                    }
                }
                //Create new Service Location
                createServiceLocation(newServiceLocPoint, sevPoint, parcelFeature.OID.ToString());

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
        #endregion

        #region Process ServicePoint when customer point is found
        private void Process_Using_CustomerPoint(IFeature parcelFeature, IFeatureClass customerPointFC, ServicePoint sevPoint, List<IFeature> customerPoints)
        {
            try
            {
                //Perform upstream-trace to determine the feeding transformer.
                //IPoint transformerShape = null;
                IFeature customerPoint = null;
                string cgc12Value = null;
                string msg = null;

                //Nov-29: Perform CGC12 tranformer search to exit if No Transformers exist.                
                IPoint transformerShape = null;
                CGC12_TransformerSearch(ref sevPoint, out transformerShape);
                if (transformerShape == null)
                {
                    msg = "Failed to find Transformer with CGC12 number " + sevPoint.CGC12;
                    _logger.Info(msg);
                    sevPoint.Status = msg;
                    _failedToFindTransformer++;
                    return;
                }
                //Transformer found.
                IArea area = parcelFeature.ShapeCopy as IArea;
                IPoint labelPoint = area.LabelPoint as IPoint;
                double transformerDistance = HelperCls.GetPointDistance(transformerShape, labelPoint);
                if (transformerDistance > 850)
                {
                    //write satus and End.
                    msg = "Transformer distance greater than 850 feet";
                    sevPoint.Status = msg;
                    _logger.Info(msg);
                    _transformerGreaterthan++;
                    return;
                }

                for (int i = 0; i < customerPoints.Count; i++)
                {
                    customerPoint = customerPoints[i];

                    //check if customer point matched the CGC12 value, if yes, the relate to it.
                    cgc12Value = HelperCls.GetValue((IRow)customerPoint, Initialize.SL_CGC12_FldIdx);
                    if (!string.IsNullOrEmpty(cgc12Value))
                    {
                        if (cgc12Value.Equals(sevPoint.CGC12, StringComparison.OrdinalIgnoreCase))
                        {
                            //Relate ServicePoint to Customer point and End
                            RelateServicePointTo_CustomerPoint(sevPoint, customerPointFC, customerPoint);
                            _servicePointRelates++;
                            return;
                        }
                    }
                }

                transformerShape = null;
                for (int i = 0; i < customerPoints.Count; i++)
                {
                    customerPoint = customerPoints[i];
                    //Trace to find the tranformer.
                    if (HelperCls.findFeedTrans(customerPoint.ShapeCopy as IPoint, sevPoint.CGC12, customerPoint.OID.ToString(), Initialize.GeometricNetwork, out transformerShape))
                    {
                        //Relate ServicePoint to Customer point and End
                        RelateServicePointTo_CustomerPoint(sevPoint, customerPointFC, customerPoint);
                        _servicePointRelates++;
                        return;
                    }
                }

                //If you are here means failed to find Transformer (using tracing) using customer-point.

                if (sevPoint.IsSecondaryLoadPoint) //If secondar load point, then end and log status.
                {
                    //write satus and End.
                    msg = "Cannot relate service point to secondary load point";
                    sevPoint.Status = msg;
                    _logger.Info(msg);
                    _failedToRelateServicePointTo_secondary_load_point++;
                    return;
                }

                //Transformer not found, perform CGC12 tranformer search
                Process_Using_ParcelCentroid(parcelFeature, ref sevPoint);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
        #endregion

        #region Helper functions
        private bool GetParcel_CustomerPoints(ref Dictionary<string, List<IFeature>> parcel_PointsList, string parcelOID, out List<IFeature> customerPoints)
        {
            customerPoints = new List<IFeature>();
            try
            {
                if (parcel_PointsList.TryGetValue(parcelOID, out customerPoints))
                    return true; //Parcel exist in list, return the customer-point list.

                //parcel_PointsList.Add(parcelOID, customerPoints);                
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return false;
        }
        private void AddCustPntToList(string parcelOID, IFeature customerPoint)
        {
            try
            {
                List<IFeature> customerPoints = new List<IFeature>();
                if (_parcel_CustomerPoints.TryGetValue(parcelOID, out customerPoints))
                {
                    customerPoints.Add(customerPoint);
                    _parcel_CustomerPoints[parcelOID] = customerPoints;
                }
                else
                {
                    throw new Exception("Failed to find Parcel in Dictionary.");
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }
        public void ConnectFeature(IFeature current)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            try
            {
                INetworkFeature networkFeature = current as INetworkFeature;
                if (networkFeature != null)
                {
                    INetElements netElements = networkFeature.GeometricNetwork.Network as INetElements;
                    //if (netElements.IsValidElement(current.EID, esriElementType.esriETJunction))
                    //{
                    try
                    {
                        networkFeature.Connect();
                    }
                    catch (Exception ex)
                    {
                        _logger.Info(ex.ToString());
                    }
                    //}
                    //else
                    //{
                    //    enumerator.MoveNext();
                    //    current = enumerator.Current;
                    //}
                }
            }
            catch (Exception ex)
            {
                //_logger.Error(ex.ToString());
            }
            //finally
            //{
            //    if (relationshipEnum != null) Marshal.FinalReleaseComObject(relationshipEnum);
            //}
        }
        private bool Skip_LargeParcelWithLargeServiceLocaitons(IGeometry geometryParcel, int customerPointCount, ref ServicePoint sevPoint)
        {

            try
            {
                IArea areaParcel = geometryParcel as IArea;
                if (areaParcel.Area > MaxParcelArea && customerPointCount > MaxParcelCustomerPoints)
                {
                    Int32 areaInt = Convert.ToInt32(areaParcel.Area);
                    string msg = "Skipped large Parcel with area " + areaInt.ToString() + " and " + customerPointCount.ToString() + " Customer Points.";
                    _logger.Info(msg);
                    sevPoint.Status = msg;
                    sevPoint.DelayProcessing = true;
                    _laregeParcelSkipped++;
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
            return false;
        }
        #endregion

    }
}