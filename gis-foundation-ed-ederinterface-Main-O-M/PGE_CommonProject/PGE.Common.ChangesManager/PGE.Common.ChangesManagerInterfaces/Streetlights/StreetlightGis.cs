using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Diagnostics;
using System.Text;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Common.ChangesManagerShared.Streetlights
{
    internal class DomainMap
    {
        public bool UseDescription { get; set; }
        public bool UseMapping { get; set; }
        public IDictionary<string, string> Mappings { get; set; }
    }

    class StreetlightGis
    {
        #region Constants

        public static string[] INV_FIELDS = { "ACCOUNTNUMBER", "ALTNM", "STREETLIGHTNUMBER", "BALLASTCHANGEDATE", "CCBOVERWRITEFLAG",
                                                "CHANGEOFPARTYDATE", "CITY", "DESCRIPTIVEADDRESS", "DIFFADDR", "DIFFBADGE", "DIFFIX", 
                                                "DIFFIT", "DIFFMAP", "DIFFRS", "FAPPLIANCE1", "FAPPLIANCE2", "FAPPLIANCE3", "FAPPLIANCE4",
                                                "FAPPLIANCE5", "FAR1", "FAR2", "FAR3", "FAR4", "FAR5", "FAROTHER", "FIXTURECODE", 
                                                "FIXTUREMANUFACTURER", "FMETRICOM", "GEMSDISTRMAPNUM", "STREETLIGHTSGISID", "HALFHRADJTYPE",
                                                "HISTGEMSAKA", "HISTGEMSBADGENUM", "HISTGEMSMAPNUM", "HISTGEMSMAPRATE", "INSTALLATIONDATE",
                                                "INVENTORIEDBY", "INVENTORYDATE", "ITEMTYPECODE", "LAMPCHANGEDATE", "LASTMODIFYDATE", 
                                                "LITESIZETYPE", "LITETYPETYPE", "LUMNCHANGEDATE", "MAILADDR1", "MAILADDR2", "MAILCITY",
                                                "MAILSTATE", "MAILZIP", "MAINTNOTE", "MAPNUMBER", "MAPNUMBERNEW", "METER", "NEARESTST",
                                                "NEWGRIDMAPNUM", "OFFICE", "OPERATINGSCHEDULE", "PAINTPOLE", "PCELL", "PCELLCHANGEDATE",
                                                "POLECHANGEDATE", "POLELENGTH", "POLEPAINTDATE", "POLETYPE", "POLEUSE", "PREMISEID",
                                                "RATESCHEDULE", "RECEIVEDATE", "REMOVEDATE", "SERVICEAGREEMENTID", "SERVICEPOINTID",
                                                "SERVICETYPE", "SPITEMHISTORY", "STATUS", "STATUSFLAG", "STREETNM", "STRTCHDT", "SUSPENSION", 
                                                "TOTCODE", "TOWNTERRDESC", "STREETLIGHTTRANSACTION", "UNIQUESPID", "USERID"
                                            };

        public static string[] INV_STAGING_FIELDS = { "ACCOUNT_NUMBER", "ALTNM", "BADGE_NUMBER", "BALLAST_CH_DT", "CCB_OVERWRITE_FLAG",
                                                "CHANGE_OF_PARTY_DATE", "CITYNAME", "DESCRIPTIVE_ADDRESS", "DIFFADDR", "DIFFBADGE", "DIFFFIX", 
                                                "DIFFIT", "DIFFMAP", "DIFFRS", "FAPPLIANCE1", "FAPPLIANCE2", "FAPPLIANCE3", "FAPPLIANCE4",
                                                "FAPPLIANCE5", "FAR1", "FAR2", "FAR3", "FAR4", "FAR5", "FAROTHER", "FIXTURE_CODE", 
                                                "FIXTURE_MANUFACTURER", "FMETRICOM", "GEMS_DISTR_MAPNUM", "GIS_ID", "HALFHRADJ_TYPE",
                                                "HIST_GEMS_AKA", "HIST_GEMS_BADGENUM", "HIST_GEMS_MAP_NUM", "HIST_GEMS_MAP_RATE", "INSTALL_DATE",
                                                "INVENTORIED_BY", "INVENTORY_DATE", "ITEM_TYPE_CODE", "LAMP_CH_DT", "LAST_MODIFY_DATE", 
                                                "LITESIZE_TYPE", "LITETYPE_TYPE", "LUMN_CH_DT", "MAIL_ADDR1", "MAIL_ADDR2", "MAIL_CITY",
                                                "MAIL_STATE", "MAIL_ZIP", "MAINTNOTE", "MAP_NUMBER", "MAP_NUMBER_NEW", "METER", "NEAREST_ST",
                                                "NEW_GRID_MAPNUM", "OFFICE", "OPERATING_SCHEDULE", "PAINTPOLE", "PCELL", "PCELL_CH_DT",
                                                "POLE_CH_DT", "POLE_LENGTH", "POLE_PT_DT", "POLE_TYPE", "POLE_USE", "PREM_ID",
                                                "RATE_SCHEDULE", "RECEIVE_DATE", "REMOVAL_DATE", "SA_ID", "SP_ID", "SERVICE",
                                                "SP_ITEM_HIST", "STATUS", "STATUS_FLAG", "STREETNM", "STRT_CH_DT", "SUSPENSION", "TOT_CODE",
                                                "TOT_TERR_DESC", "TRANSACTION_", "UNIQUE_SP_ID", "USERID"
                                            };

        public static string[] CCB_EXPORT_FIELDS = { "OFFICE", "ACCOUNTNUMBER", "", "", "STREETLIGHTNUMBER", 
                                                       "FIXTURECODE", "STATUS", "INSTALLATIONDATE", "DESCRIPTIVEADDRESS", 
                                                       "MAPNUMBER", "RATESCHEDULE", "ITEMTYPECODE", "OPERATINGSCHEDULE", "SERVICETYPE",
                                                        "FIXTUREMANUFACTURER", "POLETYPE", "POLELENGTH", "SUSPENSION", "POLEUSE",
                                                        "PREMISEID", "INVENTORYDATE", "INVENTORIEDBY", "STREETLIGHTSGISID", "" };
        //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
        public static string[] CCB_EXPORT_FIELDS_FIELDPTS = { "OFFICE","ACCOUNT_NUMBER","LOCATIONCODE","NEWBADGE","BADGE_NUMBER",
                                                                "FIXTURE_CODE","STATUS","INSTALL_DATE","DESCRIPTIVE_ADDRESS",
                                                                "MAP_NUMBER","RATE_SCHEDULE","ITEM_TYPE_CODE","OPERATING_SCHEDULE","SERVICE",
                                                                "FIXTURE_MANUFACTURER","POLE_TYPE","POLE_LENGTH","SUSPENSION","POLE_USE","XXXXXXX","PERSON_NAME",
                                                                "DIFFBADGE","DIFFFIX","DIFFADDR","DIFFMAP","DIFFRS","SIZE","UNIQUE_SP_ID","INVENTORY_DATE","INVENTORIED_BY","GIS_ID" };

        public static string[] FIELDPTS_FIELDS = { "OFFICE", "CUSTOMERNAME", "ACCOUNTNUMBER", "STREETLIGHTNUMBER", "FIXTURECODE", 
                                                    "STATUS", "STATUSFLAG", "RECEIVEDATE", "INSTALLATIONDATE", "REMOVEDATE",
                                                    "CHANGEOFPARTYDATE", "DESCRIPTIVEADDRESS", "MAPNUMBER", "RATESCHEDULE", 
                                                    "ITEMTYPECODE", "OPERATINGSCHEDULE", "SERVICETYPE", "FIXTUREMANUFACTURER", "POLETYPE", 
                                                    "POLELENGTH", "SUSPENSION", "POLEUSE", "SERVICEPOINTID", "SERVICEAGREEMENTID", "PREMISEID",
                                                    "TOTCODE", "TOWNTERRDESC", "INVENTORYDATE", "INVENTORIEDBY", "SPITEMHISTORY", 
                                                    "UNIQUESPID", "STREETLIGHTSGISID", "STREETLIGHTTRANSACTION", "FMETRICOM", 
                                                    "FAR1", "FAR2", "FAR3", "FAR4", "FAR5", "FAROTHER", "MAINTNOTE", "METER", "DIFFBADGE",
                                                    "DIFFIX", "DIFFADDR", "DIFFMAP", "DIFFRS", "DIFFIT", "PAINTPOLE", "FAPPLIANCE1",
                                                    "FAPPLIANCE2", "FAPPLIANCE3", "FAPPLIANCE4" };

        // Feature classes
        public const string FC_STREETLIGHT = "EDGIS.STREETLIGHT";
        public const string FC_SECOH = "EDGIS.SecOHConductor";
        public const string FC_SECUG = "EDGIS.SecUGConductor";
        public const string FC_SUPPORTSTRUCT = "EDGIS.SupportStructure";
        public const string FC_STREETLIGHT_INV = "EDGIS.STREETLIGHT_INV";

        //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
        public const string T_SLCDX_DATA = "PGEDATA.SLCDX_DATA";
        public const string FC_FIELDPTS = "PGEDATA.FIELDPTS";

        //SLCDX SPID FIELD
        public const string SLCDX_SPID = "SP_ID";
        public const string FIELDPTS_GIS_ID = "GIS_ID";
        public const string FIELDPTS_UNIQUE_SP_ID = "UNIQUE_SP_ID";
        public const string FIELDPTS_ACCOUNT_NUMBER = "ACCOUNT_NUMBER";

        public const string STREETLIGHTSGISID = "STREETLIGHTSGISID";
        public const string SL_UNIQUESPID = "UNIQUESPID";
        public const string SL_ACCOUNTNUMBER = "ACCOUNTNUMBER";

        // Streetlight inventory field names
        public const string INV_FIELD_GISID = "GIS_ID";
        public const string INV_FIELD_SPID = "UNIQUE_SP_ID";
        public const string INV_FIELD_BADGE = "BADGE_NUMBER";
        public const string INV_FIELD_FIXTURE = "FIXTURE_CODE";
        public const string INV_FIELD_OFFICE = "OFFICE";

        // Streetlight FC field names
        public const string GIS_FIELD_GISID = "STREETLIGHTSGISID";
        public const string GIS_FIELD_SPID = "UNIQUESPID";
        public const string GIS_FIELD_BADGE = "STREETLIGHTNUMBER";
        public const string GIS_FIELD_FIXTURE = "FIXTURECODE";
        public const string GIS_FIELD_OFFICE = "LOCALOFFICEID";
        public const string GIS_FIELD_STRUCTUREGUID = "STRUCTUREGUID";
        public const string GIS_FIELD_GLOBALID = "GLOBALID";
        public const string GIS_FIELD_RECORDSTATUS = "RECORDSTATUS";

        // Support structure FC field names
        public const string GIS_FIELD_POLEUSE = "POLEUSE";
        public const string GIS_FIELD_INSTALLJOBNO = "INSTALLJOBNUMBER";
        public const string GIS_FIELD_CUSTOMEROWNED = "CUSTOMEROWNED";
        public const string GIS_FIELD_MATERIAL = "MATERIAL";
        public const string GIS_FIELD_JOINTCOUNT = "JOINTCOUNT";
        public const string GIS_FIELD_STATUS = "STATUS";

        // Support structure FC field values
        public const int GIS_VALUE_SUBTYPE_OTHER = 7;
        public const int GIS_VALUE_POLEUSE_STREETLIGHT = 3;
        public const string GIS_VALUE_INSTALLJOBNO = "slinv";
        public const string GIS_VALUE_MATERIAL_UNKNOWN = "UNK";
        public const int GIS_VALUE_STATUS_INSERVICE = 5;

        #endregion

        #region Enums

        public enum FetchType
        {
            FetchTypeCcb = 0,
            FetchTypeInv = 1,
            FetchTypeFieldPts = 2
        }

        #endregion

        #region Member vars

        // For logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        // For working with the GIS
        private IWorkspace _workspace = null;
        private IWorkspace _nonVersionedWorkspace = null;
        private IFeatureClass _streetlightFc = null;
        private IFeatureClass _secondaryOhFc = null;
        private IFeatureClass _secondaryUgFc = null;
        private IFeatureClass _supportStructureFc = null;
        private IFeatureClass _streetlightInvFc = null;

        //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
        private ITable _slcdxDataT = null;
        private IFeatureClass _fieldptsFc = null;
        //private ITable _dupeBadgeView = null;

        // For translations
        private IDictionary<string, DomainMap> _domainMap = null;
        private IDictionary<string, string> _dateFormats = null;

        // For comparing distances between matched inventory and GIS streetlights
        private double _misplacedSlDistanceThreshold = 100;

        // For writing reports
        private string _reportPath = "c:\\temp\\streetlights\\";

        #endregion

        #region Constructor

        public StreetlightGis(IWorkspace workspace, IWorkspace nonVersionedWorkspace)
        {
            _workspace = workspace;
            _nonVersionedWorkspace = nonVersionedWorkspace;
        }

        #endregion

        #region Properties

        public double MisplacedSlDistanceThreshold
        {
            get
            {
                return _misplacedSlDistanceThreshold;
            }
            set
            {
                _misplacedSlDistanceThreshold = value;
            }
        }

        private IFeatureClass StreetlightFc
        {
            get
            {
                if (_streetlightFc == null)
                {
                    _streetlightFc = (_workspace as IFeatureWorkspace).OpenFeatureClass(FC_STREETLIGHT);
                }
                return _streetlightFc;
            }
        }
        //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
        private ITable SLCDX_DATAT
        {
            get
            {
                if (_slcdxDataT == null)
                {
                    _slcdxDataT = (_workspace as IFeatureWorkspace).OpenTable(T_SLCDX_DATA);
                }
                return _slcdxDataT;
            }
        }
        private IFeatureClass FIELDPTSFc
        {
            get
            {
                if (_fieldptsFc == null)
                {
                    _fieldptsFc = (_workspace as IFeatureWorkspace).OpenFeatureClass(FC_FIELDPTS);
                }
                return _fieldptsFc;
            }
        }

        //private ITable DupeBadgeView
        //{
        //    get
        //    {
        //        if (_dupeBadgeView == null)
        //        {
        //            _dupeBadgeView = (_workspace as IFeatureWorkspace).OpenTable("EDGIS.V_DUPE_BADGE_INV");
        //        }
        //        return _dupeBadgeView;
        //    }
        //}

        private IFeatureClass SecondaryOhFc
        {
            get
            {
                if (_secondaryOhFc == null)
                {
                    _secondaryOhFc = (_workspace as IFeatureWorkspace).OpenFeatureClass(FC_SECOH);
                }
                return _secondaryOhFc;
            }
        }

        private IFeatureClass SecondaryUgFc
        {
            get
            {
                if (_secondaryUgFc == null)
                {
                    _secondaryUgFc = (_workspace as IFeatureWorkspace).OpenFeatureClass(FC_SECUG);
                }
                return _secondaryUgFc;
            }
        }

        private IFeatureClass SupportStructureFc
        {
            get
            {
                if (_supportStructureFc == null)
                {
                    _supportStructureFc = (_workspace as IFeatureWorkspace).OpenFeatureClass(FC_SUPPORTSTRUCT);
                }
                return _supportStructureFc;
            }
        }

        private IFeatureClass StreetlightInvFc
        {
            get
            {
                if (_streetlightInvFc == null)
                {
                    _streetlightInvFc = (_workspace as IFeatureWorkspace).OpenFeatureClass("EDGIS.STREETLIGHT_INV");
                }
                return _streetlightInvFc;
            }
        }

        private IDictionary<string, DomainMap> DomainMapping
        {
            get
            {
                // This could/should be moved to a mapping table thats read dynamically
                if (_domainMap == null)
                {
                    _domainMap = new Dictionary<string, DomainMap>();

                    DomainMap map = new DomainMap();
                    map.UseDescription = false;
                    map.UseMapping = true;
                    map.Mappings = new Dictionary<string, string>();
                    map.Mappings.Add("A", "5");  //"In Service");
                    map.Mappings.Add("R", "30"); // "Idle");
                    _domainMap.Add("Construction Status", map);

                    DomainMap map2 = new DomainMap();
                    map2.UseDescription = true;
                    map2.UseMapping = false;
                    map2.Mappings = null;
                    _domainMap.Add("Town Territory Code", map2);

                    //DomainMap map3 = new DomainMap();
                    //map3.UseDescription = true;
                    //map3.UseMapping = false;
                    //map3.Mappings = null;
                    //_domainMap.Add("Street Light Item Type", map3);

                    DomainMap map4 = new DomainMap();
                    map4.UseDescription = false;
                    map4.UseMapping = true;
                    map4.Mappings = new Dictionary<string, string>();
                    map4.Mappings.Add("Y", "UG");
                    map4.Mappings.Add("N", "OH");
                    _domainMap.Add("Street Light Service Type", map4);
                }

                return _domainMap;
            }
        }

        public string ReportPath
        {
            get
            {
                return _reportPath;
            }
            set
            {
                _reportPath = value;
            }
        }

        #endregion

        #region Public methods

        public void AddToDomainMap(string name, IDictionary<string, string> domainMap)
        {
            // If its not already in the domain map, add it
            if (DomainMapping.ContainsKey(name) == false)
            {
                DomainMap newMap = new DomainMap();
                newMap.UseDescription = false;
                newMap.UseMapping = true;
                newMap.Mappings = new Dictionary<string, string>();
                foreach (string key in domainMap.Keys)
                {
                    newMap.Mappings.Add(key, domainMap[key]);
                }
                DomainMapping.Add(name, newMap);
            }
        }

        //ME Q3 ITEM 2018: DATA FROM GIS WILL BE SENT AS UPDATE ONLY IF DATA EXISTS IN SLCDX DATA
        public void IsSPIDInSLCDXData(IFeature sourceSL, out bool Isfound)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            Isfound = false;
            try
            {
                String spId = GetFieldValue(sourceSL, GIS_FIELD_SPID);
                IQueryFilter gisUiqueSPIDFilter = new QueryFilterClass();
                gisUiqueSPIDFilter.WhereClause = SLCDX_SPID + "='" + spId + "'";
                ICursor slcdxCursor = SLCDX_DATAT.Search(gisUiqueSPIDFilter, false);
                IRow slcdx_row = slcdxCursor.NextRow();
                if (slcdx_row != null)
                    Isfound = true;

            }

            catch (Exception ex)
            {
                _logger.Warn("Failed to check SPID in SLCDX_DATA : " + ex.ToString());
            }
        }
        public IFeature GetFieldPtsDataForSL(object unique_spid, object account_number)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            IQueryFilter gisFieldPTSFilter = default(IQueryFilter);
            IFeatureCursor fieldPTSCursor = default(IFeatureCursor);
            IFeature fieldpts_feat = default(IFeature);
            try
            {
                gisFieldPTSFilter = new QueryFilterClass();
                gisFieldPTSFilter.WhereClause = FIELDPTS_ACCOUNT_NUMBER + "='" + account_number.ToString() + "' AND " + FIELDPTS_UNIQUE_SP_ID + "='" + unique_spid.ToString() + "'";
                fieldPTSCursor = FIELDPTSFc.Search(gisFieldPTSFilter, false);
                fieldpts_feat = fieldPTSCursor.NextFeature();

                return fieldpts_feat;

            }

            catch (Exception ex)
            {
                _logger.Warn("Failed to fetch Field PTS data frm UNIQUE_SP_ID : " + unique_spid.ToString() + " " + ex.ToString());
                return null;
            }
            finally
            {
                if (gisFieldPTSFilter != null) Marshal.ReleaseComObject(gisFieldPTSFilter);
                if (fieldPTSCursor != null) Marshal.ReleaseComObject(fieldPTSCursor);
            }
        }

        public void WasAssignedSPID(IFeature sourceSL, IFeature parentSL, out bool wasAssignedSPID, out bool wasAlreadyAssignedSPID)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            wasAssignedSPID = false;
            wasAlreadyAssignedSPID = false;

            try
            {
                String spId = GetFieldValue(sourceSL, GIS_FIELD_SPID);

                // If this is new, we only need to check thats its in service (status = 5)
                if (parentSL == null)
                {
                    if (spId != "" && spId != "0")
                    {
                        wasAssignedSPID = true;
                    }
                }
                else
                {
                    // If its an update, we need to check the old with da new
                    String priorSpId = GetFieldValue(parentSL, GIS_FIELD_SPID);
                    if ((priorSpId == "" || priorSpId == "0") && spId != "" && spId != "0")
                    {
                        wasAssignedSPID = true;
                    }
                    else if (priorSpId == spId && spId != "" && spId != "0")
                    {
                        wasAlreadyAssignedSPID = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if GIS streetlight was assigned an SPID: " + ex.ToString());
            }
        }

        public void CDWasAssignedSPID(IFeature sourceSL, UpdateFeat parentSL, out bool wasAssignedSPID, out bool wasAlreadyAssignedSPID)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            wasAssignedSPID = false;
            wasAlreadyAssignedSPID = false;

            try
            {
                String spId = GetFieldValue(sourceSL, GIS_FIELD_SPID);

                // If this is new, we only need to check thats its in service (status = 5)
                if (parentSL == null)
                {
                    if (spId != "" && spId != "0")
                    {
                        wasAssignedSPID = true;
                    }
                }
                else
                {
                    // If its an update, we need to check the old with da new
                    String priorSpId = CDGetFieldValue(parentSL, GIS_FIELD_SPID);
                    if ((priorSpId == "" || priorSpId == "0") && spId != "" && spId != "0")
                    {
                        wasAssignedSPID = true;
                    }
                    else if (priorSpId == spId && spId != "" && spId != "0")
                    {
                        wasAlreadyAssignedSPID = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if GIS streetlight was assigned an SPID: " + ex.ToString());
            }
        }

        public void WasPlacedInService(IFeature sourceSL, IFeature parentSL, out bool wasPlacedInService, out bool wasAlreadyInService)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            wasPlacedInService = false;
            wasAlreadyInService = false;

            try
            {
                String status = GetFieldValue(sourceSL, GIS_FIELD_STATUS);

                // If this is new, we only need to check thats its in service (status = 5)
                if (parentSL == null)
                {
                    if (status == "5")
                    {
                        wasPlacedInService = true;
                    }
                }
                else
                {
                    // If its an update, we need to check the old with da new
                    String priorStatus = GetFieldValue(parentSL, GIS_FIELD_STATUS);
                    if (priorStatus != "5" && status == "5")
                    {
                        wasPlacedInService = true;
                    }
                    else if (priorStatus == status && status == "5")
                    {
                        wasAlreadyInService = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if GIS streetlight was placed In Service: " + ex.ToString());
            }
        }

        public void CDWasPlacedInService(IFeature sourceSL, UpdateFeat parentSL, out bool wasPlacedInService, out bool wasAlreadyInService)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            wasPlacedInService = false;
            wasAlreadyInService = false;

            try
            {
                String status = GetFieldValue(sourceSL, GIS_FIELD_STATUS);

                // If this is new, we only need to check thats its in service (status = 5)
                if (parentSL == null)
                {
                    if (status == "5")
                    {
                        wasPlacedInService = true;
                    }
                }
                else
                {
                    // If its an update, we need to check the old with da new
                    String priorStatus = CDGetFieldValue(parentSL, GIS_FIELD_STATUS);
                    if (priorStatus != "5" && status == "5")
                    {
                        wasPlacedInService = true;
                    }
                    else if (priorStatus == status && status == "5")
                    {
                        wasAlreadyInService = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if GIS streetlight was placed In Service: " + ex.ToString());
            }
        }

        public bool HasGisID(IFeature sourceSL)
        {
            bool hasGisId = false;

            try
            {
                String gisId = GetFieldValue(sourceSL, GIS_FIELD_GISID);
                if (gisId != string.Empty)
                {
                    hasGisId = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to check if GIS ID exists: " + ex.ToString());
            }

            return hasGisId;
        }

        public void UpdateGisID(IRow sourceRow, string gisId)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                IRow streetlightRow = StreetlightFc.GetFeature(sourceRow.OID);
                if (streetlightRow != null)
                {
                    int gisIdFieldIndex = StreetlightFc.FindField("STREETLIGHTSGISID");
                    object currValue = streetlightRow.get_Value(gisIdFieldIndex);

                    // Set the in-memory copy
                    sourceRow.set_Value(gisIdFieldIndex, gisId);

                    // And the database version via the current session
                    if (currValue != null && gisId != currValue.ToString())
                    {
                        ((IWorkspaceEdit)_workspace).StartEditOperation();
                        streetlightRow.set_Value(gisIdFieldIndex, gisId);
                        streetlightRow.Store();
                        ((IWorkspaceEdit)_workspace).StopEditOperation();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to update GIS_ID: " + ex.ToString());
                // Ignore for now
                // throw ex; 
            }
        }

        public IList<string> GetValues(IRow streetlightRow, String[] fields, FetchType systemFetchType)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            object gis_id_sl = null; ;
            IList<string> values = new List<string>();
            try
            {
                //IRow streetlightRow = StreetlightFc.GetFeature(sourceRow.OID);
                if (streetlightRow != null)
                {
                    IFeature FieldPTS_Feat = default(IFeature);
                    if (systemFetchType == FetchType.FetchTypeCcb)
                    {
                        object UNIQUESPID = streetlightRow.get_Value(StreetlightFc.Fields.FindField(SL_UNIQUESPID));
                        object ACCOUNTNUMBER = streetlightRow.get_Value(StreetlightFc.Fields.FindField(SL_ACCOUNTNUMBER));
                        gis_id_sl = streetlightRow.get_Value(StreetlightFc.Fields.FindField(STREETLIGHTSGISID));
                        if (UNIQUESPID != null && ACCOUNTNUMBER != null)
                        {
                            FieldPTS_Feat = GetFieldPtsDataForSL(UNIQUESPID, ACCOUNTNUMBER);
                            streetlightRow = (IRow)FieldPTS_Feat;
                        }
                    }
                    if (streetlightRow != null)
                    {
                        foreach (string field in fields)
                        {
                            string value = string.Empty;
                            int gisIdFieldIndex;
                            if (systemFetchType == FetchType.FetchTypeCcb)
                            {
                                gisIdFieldIndex = FIELDPTSFc.FindField(field);
                            }
                            else
                            {
                                gisIdFieldIndex = StreetlightFc.FindField(field);
                            }
                            if (gisIdFieldIndex > -1)
                            {
                                object currValue = streetlightRow.get_Value(gisIdFieldIndex);
                                if (currValue != null)
                                {
                                    if (systemFetchType == FetchType.FetchTypeCcb)
                                    {
                                        // This condition is applied to get GIS_ID value from Streetlight feature class and rest from EDGIS.FIELDPTS FC
                                        if (field.ToUpper() == "GIS_ID")
                                        {
                                            value = gis_id_sl.ToString();
                                        }
                                        else
                                        {
                                            value = ProcessCcbField(streetlightRow as IFeature, field, currValue);
                                        }
                                    }
                                    else if (systemFetchType == FetchType.FetchTypeFieldPts)
                                    {
                                        value = ProcessFieldPtsField(streetlightRow as IFeature, field, currValue);
                                    }
                                    else if (systemFetchType == FetchType.FetchTypeInv)
                                    {
                                        value = ProcessInvField(streetlightRow as IFeature, field, currValue);
                                    }
                                    else
                                    {
                                        // Default to that value
                                        value = currValue.ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (field != "")
                                {
                                    if (systemFetchType == FetchType.FetchTypeCcb)
                                    {
                                        if (field == "SIZE")
                                        {
                                            _logger.Warn("The Value for this field has been deliberately set to 1 as per ME Q3 2018 requirement of Streetlight, Failed to find GIS field: " + field);
                                            value = "1";
                                        }
                                        else
                                        {
                                            _logger.Warn("The Value for this field has been deliberately set to empty as per ME Q3 2018 requirement of Streetlight,Failed to find GIS field: " + field);
                                            value = "";
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warn("Failed to find GIS field: " + field);
                                        value = "TODO-FieldNotFound";
                                    }
                                }
                            }
                            values.Add(value);
                        }
                    }
                    else { _logger.Warn("Failed to find Corresponding data in EDGIS.FIELDPTS of CCB Type transaction."); }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get values: " + ex.ToString());
                throw ex;
            }

            return values;
        }

        public void AddSL(IFeature stagingRow)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Processing Streetlight Add with OID: " + stagingRow.OID.ToString() + "...");

            IFeatureCursor slCursor = null;

            try
            {
                // Check that the SP_ID doesn't already exist...
                if (StreetlightExists(stagingRow, INV_FIELD_SPID, GIS_FIELD_SPID) == false)
                {
                    // Check for matches by badge/office/fixture
                    slCursor = GetStreetlightByBadge(stagingRow);
                    IFeature existingSl = null;
                    if (slCursor != null)
                    {
                        existingSl = slCursor.NextFeature();
                    }

                    // If found...
                    if (existingSl != null)
                    {
                        // Update the SP_ID and GIS_ID's
                        UpdateExistingSl(existingSl, stagingRow);
                    }
                    else
                    {
                        // Otherwise create a new one
                        if (stagingRow.Shape != null)
                        {
                            CreateNewSl(stagingRow);
                        }
                        else
                        {
                            _logger.Warn("Staging row does not have shape. Unable to create new streetlight and skipping");
                        }
                    }
                }
                else
                {
                    _logger.Warn("New streetlight in staging table already exists in GIS");
                }
            }
            catch (Exception ex)
            {
                // Epic fail
                throw ex;
            }
            finally
            {
                // Clean up those cursors and such
                _logger.Info("###");
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
            }
        }

        public void DeleteSL(IFeature stagingRow)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Processing Streetlight Delete...");

            IFeatureCursor slCursor = null;

            try
            {
                // Check for streetlight by SPID
                slCursor = GetStreetlight(stagingRow, INV_FIELD_SPID, GIS_FIELD_SPID);
                IFeature existingSl = slCursor.NextFeature();

                // If found...
                if (existingSl != null)
                {
                    // Update the SP_ID and GIS_ID's
                    DeleteExistingSl(existingSl);
                }
                else
                {
                    _logger.Info("Streetlight not found in GIS");
                }
            }
            catch (Exception ex)
            {
                // Epic fail
                throw ex;
            }
            finally
            {
                // Clean up those cursors and such
                _logger.Info("###");
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
            }
        }

        public bool StreetlightExists(IFeature sourceRow, string sourceFieldName, string gisFieldName, bool checkDistances = false, bool logExists = false)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            bool exists = false;
            IFeatureCursor slCursor = null;

            try
            {
                // Check for existence
                slCursor = GetStreetlight(sourceRow, sourceFieldName, gisFieldName);
                if (slCursor != null)
                {
                    IFeature existingSl = slCursor.NextFeature();
                    if (existingSl != null)
                    {
                        exists = true;

                        if (logExists == true)
                        {
                            _logger.Info("Found match on " + gisFieldName + " for OID: " + sourceRow.OID.ToString());
                        }

                        if (checkDistances == true)
                        {
                            CheckPlacement(sourceRow, existingSl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = false;
                _logger.Error("Failed to check if streetlight exists: " + ex.ToString());
            }
            finally
            {
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
            }

            // Return the result
            return exists;
        }

        public bool ConflateByGisId(IFeature sourceRow, string sourceFieldName, string gisFieldName, bool checkDistances = false, bool logExists = false)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            bool exists = false;
            IFeatureCursor slCursor = null;

            try
            {
                // Check for existence
                slCursor = GetStreetlight(sourceRow, sourceFieldName, gisFieldName, true);
                if (slCursor != null)
                {
                    IFeature existingSl = slCursor.NextFeature();
                    if (existingSl != null)
                    {
                        exists = true;

                        if (logExists == true)
                        {
                            _logger.Info("Found match on " + gisFieldName + " for OID: " + sourceRow.OID.ToString());
                        }

                        if (checkDistances == true)
                        {
                            CheckPlacement(sourceRow, existingSl);
                        }

                        bool proceed = true;
                        string sourceVal = GetFieldValue(sourceRow, INV_FIELD_SPID);
                        if (sourceVal != "")
                        {
                            string targetVal = GetFieldValue(existingSl, GIS_FIELD_SPID);
                            if (sourceVal == targetVal)
                            {
                                proceed = false;
                            }
                        }

                        if (proceed == true)
                        {
                            _logger.Info("Updating Streetlights SP ID");
                            ((IWorkspaceEdit)_workspace).StartEditOperation();
                            UpdateValue(sourceRow, INV_FIELD_SPID, existingSl, GIS_FIELD_SPID);
                            slCursor.UpdateFeature(existingSl);
                            ((IWorkspaceEdit)_workspace).StopEditOperation();
                        }
                        else
                        {
                            _logger.Info("Streetlights SP ID already matches INV value of " + sourceVal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = false;
                _logger.Error("Failed to check if streetlight exists: " + ex.ToString());
            }
            finally
            {
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
            }

            // Return the result
            return exists;
        }

        public bool ConflateBySpId(IFeature sourceRow, string sourceFieldName, string gisFieldName, bool checkDistances = false, bool logExists = false)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            bool exists = false;
            IFeatureCursor slCursor = null;

            try
            {
                // Check for existence
                slCursor = GetStreetlight(sourceRow, sourceFieldName, gisFieldName, true);
                if (slCursor != null)
                {
                    IFeature existingSl = slCursor.NextFeature();
                    if (existingSl != null)
                    {
                        exists = true;

                        if (logExists == true)
                        {
                            _logger.Info("Found match on " + gisFieldName + " for OID: " + sourceRow.OID.ToString());
                        }

                        if (checkDistances == true)
                        {
                            CheckPlacement(sourceRow, existingSl);
                        }

                        bool proceed = true;
                        string sourceVal = GetFieldValue(sourceRow, INV_FIELD_GISID);
                        if (sourceVal != "")
                        {
                            string targetVal = GetFieldValue(existingSl, GIS_FIELD_GISID);
                            if (sourceVal == targetVal)
                            {
                                proceed = false;
                            }
                        }

                        if (proceed == true)
                        {
                            _logger.Info("Updating Streetlights GIS ID");
                            ((IWorkspaceEdit)_workspace).StartEditOperation();
                            UpdateValue(sourceRow, INV_FIELD_GISID, existingSl, GIS_FIELD_GISID);
                            slCursor.UpdateFeature(existingSl);
                            ((IWorkspaceEdit)_workspace).StopEditOperation();
                        }
                        else
                        {
                            _logger.Info("Streetlights GIS ID already matches INV value of " + sourceVal);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                exists = false;
                _logger.Error("Failed to check if streetlight exists: " + ex.ToString());
            }
            finally
            {
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
            }

            // Return the result
            return exists;
        }

        public bool ConflateByBadge(IFeature sourceFeature)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            bool matched = false;
            IFeatureCursor slCursor = null;
            IFeatureCursor nearbyStreetlights = null;

            try
            {
                ((IWorkspaceEdit)_workspace).StartEditOperation();
                slCursor = GetStreetlightByBadge(sourceFeature, false);
                if (slCursor != null)
                {
                    IFeature streetlight = slCursor.NextFeature();
                    if (streetlight != null)
                    {
                        // Check if this badge is duped in inventory
                        bool isDupeBadge = IsDupeBadge(sourceFeature);

                        // If not, update
                        if (isDupeBadge == true)
                        {
                            nearbyStreetlights = GetStreetlightByBadge(sourceFeature, true, 250, false);
                            IFeature nearbyStreetlight = nearbyStreetlights.NextFeature();
                            if (nearbyStreetlight != null)
                            {
                                IFeature secondNearbyStreetlight = nearbyStreetlights.NextFeature();
                                if (secondNearbyStreetlight != null)
                                {
                                    // Log to a report
                                    _logger.Info("Unable to conflate OID: " + sourceFeature.OID.ToString() + ". Badge has multiple matches and multiple streetlights found in search radius");
                                    WriteCannotConflateSL(sourceFeature);
                                }
                                else
                                {
                                    // Conflate
                                    _logger.Info("Matched on STREETLIGHTNUMBER with spatial check for OID: " + sourceFeature.OID.ToString() + ". Updating GIS and SP IDs");
                                    matched = true;
                                    if (nearbyStreetlights != null) Marshal.FinalReleaseComObject(nearbyStreetlights);
                                    nearbyStreetlights = GetStreetlightByBadge(sourceFeature, true, 250, false);
                                    nearbyStreetlight = nearbyStreetlights.NextFeature();
                                    CheckPlacement(sourceFeature, nearbyStreetlight);
                                    UpdateValue(sourceFeature, INV_FIELD_GISID, nearbyStreetlight, GIS_FIELD_GISID);
                                    UpdateValue(sourceFeature, INV_FIELD_SPID, nearbyStreetlight, GIS_FIELD_SPID);
                                    nearbyStreetlights.UpdateFeature(nearbyStreetlight);
                                }
                            }
                            else
                            {
                                _logger.Info("Matched on badge but there are multiple badge/office/fixcode matches and none found in search radius. Adding as new streetlight");
                            }
                        }
                        else
                        {
                            _logger.Info("Matched on STREETLIGHTNUMBER for OID: " + sourceFeature.OID.ToString() + ". Updating GIS and SP IDs");
                            matched = true;
                            CheckPlacement(sourceFeature, streetlight);
                            UpdateValue(sourceFeature, INV_FIELD_GISID, streetlight, GIS_FIELD_GISID);
                            UpdateValue(sourceFeature, INV_FIELD_SPID, streetlight, GIS_FIELD_SPID);
                            slCursor.UpdateFeature(streetlight);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ((IWorkspaceEdit)_workspace).StopEditOperation();
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
                if (nearbyStreetlights != null) Marshal.FinalReleaseComObject(nearbyStreetlights);
            }

            return matched;
        }

        public bool IsDupeBadge(IFeature sourceFeature)
        {
            bool dupeBadge = false;

            IFeatureCursor dupeBadges = null;

            try
            {
                //// Otherwise fetch it...
                //string sql = "select objectid from EDGIS.V_DUPE_BADGE_INV where objectid=" + sourceFeature.OID.ToString();
                //OracleCommand queryCommand = new OracleCommand(sql, _adoOracleConnectionSource.OracleConnection);
                //OracleDataReader reader = queryCommand.ExecuteReader();
                //if (reader.HasRows == true)
                //{
                //    dupeBadge = true;
                //}

                dupeBadges = GetStreetlightInvByBadge(sourceFeature);
                //ME Q2 2018 DA Item# 19- Allow duplicate streetlight when the badge number is set to ZZZZZZ, and UniqueSPID is set to 0
                IFeature firstRow = dupeBadges.NextFeature();
                if (firstRow != null)
                {
                    IFeature secInvRow = dupeBadges.NextFeature();
                    while (secInvRow != null)
                    {
                        string spid = GetFieldValue(secInvRow, INV_FIELD_SPID);
                        string badge = GetFieldValue(secInvRow, INV_FIELD_BADGE);
                        if (spid == "0" && badge.ToUpper() == "ZZZZZZ")
                        {
                            return false;
                        }
                        else
                        {
                            dupeBadge = true;
                        }
                        secInvRow = dupeBadges.NextFeature();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed top detect if badge was duped: " + ex.ToString());
                throw ex;
            }
            finally
            {
                if (dupeBadges != null) Marshal.FinalReleaseComObject(dupeBadges);
            }

            return dupeBadge;
        }

        private void CheckPlacement(IFeature source, IFeature target)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Check how far apart the features are
                IGeometry projectedGeometry = source.ShapeCopy;
                projectedGeometry.Project((StreetlightFc as IGeoDataset).SpatialReference);
                IProximityOperator proxOp = projectedGeometry as IProximityOperator;
                double distance = proxOp.ReturnDistance(target.Shape);

                // If above a threshold, log
                if (distance > MisplacedSlDistanceThreshold)
                {
                    WriteMisplacedSL(target, distance);
                }
            }
            catch (Exception ex)
            {
                // Log but don't stop, not the end of the world
                _logger.Warn("Could not check placement of streetlight: " + ex.ToString());
            }
        }

        public void DetectInvalid(bool delete)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor slCursor = null;
            StreamWriter writer = null;

            try
            {
                _logger.Info("Checking GIS for invalid streetlights...");

                // Create an output file
                writer = CreateInvalidSlFile();

                // Get the streetlights to delete
                slCursor = FetchDeletes();

                _logger.Info("Deleting invalid streetlights...");
                IFeature streetlight = slCursor.NextFeature();
                int counter = 0;
                int commitInterval = 1000;

                // For each one found
                while (streetlight != null)
                {
                    counter++;

                    // Report it and optionally delete it
                    WriteDeleted(streetlight, writer);
                    if (delete == true)
                    {
                        try
                        {
                            ((IWorkspaceEdit)_workspace).StartEditOperation();
                            streetlight.Delete();
                            ((IWorkspaceEdit)_workspace).StopEditOperation();
                        }
                        catch (Exception ex)
                        {
                            _logger.Info("Error when deleting streetlight: " + ex.ToString());
                            _logger.Info("Continuing...");
                        }
                    }

                    if (counter % 10 == 0)
                    {
                        _logger.Info("Deleted " + counter.ToString() + " invalid streetlights...");
                    }

                    // Save every once in a while in case things bomb
                    if (counter % commitInterval == 0)
                    {
                        _logger.Info("Commit interval hit, checking for deletes...");
                        IWorkspaceEdit wsEdit = _workspace as IWorkspaceEdit;
                        bool hasEdits = false;
                        wsEdit.HasEdits(ref hasEdits);
                        if (hasEdits == true)
                        {
                            _logger.Info("Deletes found. Saving...");
                            int lastObjId = streetlight.OID;
                            wsEdit.StopEditing(true);
                            wsEdit.StartEditing(false);
                            _logger.Info("Save complete. Re-fetching streetlights to delete...");
                            if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
                            slCursor = FetchDeletes(lastObjId + 1);
                            _logger.Info("Fetch complete. Resuming Deleting...");
                        }
                        else
                        {
                            _logger.Info("No edits found. Resuming Deleting...");
                        }
                    }

                    // Move to the next one
                    streetlight = slCursor.NextFeature();
                }
                _logger.Info(counter.ToString() + " invalid streetlights deleted.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
                if (writer != null) writer.Close();
            }
        }

        #endregion

        #region Private methods

        public IFeatureCursor FetchDeletes(int startObjectId = 1)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor slCursor = null;

            try
            {
                // Build a query for null GIS ID's, SP ID's and null badge numbers
                string clause = "((" + GIS_FIELD_GISID + " IS NULL AND " + GIS_FIELD_SPID + " IS NULL AND (" + GIS_FIELD_BADGE + " IS NULL OR " + GIS_FIELD_BADGE + "='0')) OR ";
                clause += "(" + GIS_FIELD_RECORDSTATUS + "='R'))";
                clause += " AND OBJECTID>=" + startObjectId.ToString();
                _logger.Info("Deletes 'Where' clause: " + clause);
                IQueryFilter qfSl = new QueryFilterClass();
                qfSl.WhereClause = clause;
                IQueryFilterDefinition qfDef = qfSl as IQueryFilterDefinition;
                qfDef.PostfixClause = "ORDER BY OBJECTID";
                slCursor = StreetlightFc.Update(qfSl, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return slCursor;
        }

        #region Reports

        private StreamWriter CreateInvalidSlFile()
        {
            // Make sure we have a handle on the file
            FileInfo badSlFile = new FileInfo(ReportPath + "InvalidSl.txt");

            // Create a writer
            StreamWriter writer = null;
            if (badSlFile.Exists == false)
            {
                writer = badSlFile.CreateText();
            }
            else
            {
                writer = badSlFile.AppendText();
            }

            writer.WriteLine("New Invalid SL file created on " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
            writer.WriteLine("(Format: OID,GLOBALID,x,y)");

            return writer;
        }

        private void WriteDeleted(IFeature feature, StreamWriter writer)
        {
            try
            {
                if (writer != null)
                {
                    // Get the OID and x, y coords
                    string globalId = GetFieldValue(feature, GIS_FIELD_GLOBALID);
                    string output = feature.OID.ToString() + "," + globalId + "," + (feature.Shape as IPoint).X.ToString() + "," + (feature.Shape as IPoint).Y.ToString();
                    writer.WriteLine(output);
                    writer.Flush();
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        public void CreateSuspectSlFile()
        {
            // Make sure we have a handle on the file
            FileInfo slFile = new FileInfo(ReportPath + "SuspectSl.txt");

            // Create a writer
            StreamWriter writer = null;
            if (slFile.Exists == false)
            {
                writer = slFile.CreateText();
            }
            else
            {
                writer = slFile.AppendText();
            }

            writer.WriteLine("New Suspect SL file created on " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
            writer.WriteLine("(Format: OID,GLOBALID,GISID,SPID,BADGE,OFFICE,FIXTURECODE)");
            writer.Flush();
            writer.Close();
        }

        private void WriteSuspect(IFeature feature)
        {
            try
            {
                // Make sure we have a handle on the file
                FileInfo slFile = new FileInfo(ReportPath + "SuspectSl.txt");

                // Create a writer
                StreamWriter writer = null;
                if (slFile.Exists == false)
                {
                    writer = slFile.CreateText();
                }
                else
                {
                    writer = slFile.AppendText();
                }

                if (writer != null)
                {
                    string gisId = GetFieldValue(feature, GIS_FIELD_GISID);
                    string globalId = GetFieldValue(feature, GIS_FIELD_GLOBALID);
                    string spId = GetFieldValue(feature, GIS_FIELD_SPID);
                    string badge = GetFieldValue(feature, GIS_FIELD_BADGE);
                    string office = GetFieldValue(feature, GIS_FIELD_OFFICE);
                    string fixCode = GetFieldValue(feature, GIS_FIELD_FIXTURE);

                    // Get the OID and x, y coords
                    string output = feature.OID.ToString() + "," + globalId + "," + gisId + "," + spId + "," + badge + "," + office + "," + fixCode;
                    writer.WriteLine(output);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        public void CreateBadDomainValueFile()
        {
            // Make sure we have a handle on the file
            FileInfo slFile = new FileInfo(ReportPath + "SlBadDomainValue.txt");

            // Create a writer
            StreamWriter writer = null;
            if (slFile.Exists == false)
            {
                writer = slFile.CreateText();
            }
            else
            {
                writer = slFile.AppendText();
            }

            writer.WriteLine("New SL bad domain value file created on " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
            writer.WriteLine("(Format: OID,FEATURECLASS,GLOBALID,FIELD,VALUE)");
            writer.Flush();
            writer.Close();
        }

        private void WriteBadDomainValue(IFeature feature, string field, string value)
        {
            try
            {
                // Make sure we have a handle on the file
                FileInfo slFile = new FileInfo(ReportPath + "SlBadDomainValue.txt");

                // Create a writer
                StreamWriter writer = null;
                if (slFile.Exists == false)
                {
                    writer = slFile.CreateText();
                }
                else
                {
                    writer = slFile.AppendText();
                }

                if (writer != null)
                {
                    // Get the OID and x, y coords
                    string globalId = GetFieldValue(feature, GIS_FIELD_GLOBALID);
                    string output = feature.OID.ToString() + "," + feature.Class.AliasName + "," + globalId + "," + field + "," + value;
                    writer.WriteLine(output);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        public void CreateMisplacedSlFile()
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Make sure we have a handle on the file
            FileInfo slFile = new FileInfo(ReportPath + "MisplacedSl.txt");

            // Create a writer
            StreamWriter writer = null;
            if (slFile.Exists == false)
            {
                writer = slFile.CreateText();
            }
            else
            {
                writer = slFile.AppendText();
            }

            writer.WriteLine("Mis-placed SL file created on " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
            writer.WriteLine("Streetlights here matched on inventory data but their shapes did not match closely (Format: OID,GLOBALID,GISID,SPID,BADGE,OFFICE,FIXCODE,DISTANCE)");
            writer.Flush();
            writer.Close();
        }

        private void WriteMisplacedSL(IFeature feature, double distance)
        {
            try
            {
                // Make sure we have a handle on the file
                FileInfo slFile = new FileInfo(ReportPath + "MisplacedSl.txt");

                // Create a writer
                StreamWriter writer = null;
                if (slFile.Exists == false)
                {
                    writer = slFile.CreateText();
                }
                else
                {
                    writer = slFile.AppendText();
                }

                if (writer != null)
                {
                    string gisId = GetFieldValue(feature, GIS_FIELD_GISID);
                    string globalId = GetFieldValue(feature, GIS_FIELD_GLOBALID);
                    string spId = GetFieldValue(feature, GIS_FIELD_SPID);
                    string badge = GetFieldValue(feature, GIS_FIELD_BADGE);
                    string office = GetFieldValue(feature, GIS_FIELD_OFFICE);
                    string fixCode = GetFieldValue(feature, GIS_FIELD_FIXTURE);

                    // Get the OID and x, y coords
                    string output = feature.OID.ToString() + "," + globalId + "," + gisId + "," + spId + "," + badge + "," + office + "," + fixCode + "," + distance.ToString();
                    writer.WriteLine(output);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        public void CreateCannotConflateFile()
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Make sure we have a handle on the file
            FileInfo slFile = new FileInfo(ReportPath + "DidNotConflate.txt");

            // Create a writer
            StreamWriter writer = null;
            if (slFile.Exists == false)
            {
                writer = slFile.CreateText();
            }
            else
            {
                writer = slFile.AppendText();
            }

            writer.WriteLine("SL Did Not Conflate file created on " + DateTime.Now.ToShortDateString() + " at " + DateTime.Now.ToShortTimeString());
            writer.WriteLine("Streetlights here matched on badge to multiple streetlights hat were within the search radius (Format: OID,GISID,SPID,BADGE,OFFICE,FIXCODE)");
            writer.Flush();
            writer.Close();
        }

        private void WriteCannotConflateSL(IFeature feature)
        {
            try
            {
                // Make sure we have a handle on the file
                FileInfo slFile = new FileInfo(ReportPath + "DidNotConflate.txt");

                // Create a writer
                StreamWriter writer = null;
                if (slFile.Exists == false)
                {
                    writer = slFile.CreateText();
                }
                else
                {
                    writer = slFile.AppendText();
                }

                if (writer != null)
                {
                    string gisId = GetFieldValue(feature, StreetlightInv.INV_FIELD_GISID);
                    string spId = GetFieldValue(feature, StreetlightInv.INV_FIELD_UNIQUESPID);
                    string badge = GetFieldValue(feature, StreetlightInv.INV_FIELD_BADGENO);
                    string office = GetFieldValue(feature, StreetlightInv.INV_FIELD_OFFICE);
                    string fixCode = GetFieldValue(feature, StreetlightInv.INV_FIELD_FIXTURECODE);

                    // Get the OID and x, y coords
                    string output = feature.OID.ToString() + "," + gisId + "," + spId + "," + badge + "," + office + "," + fixCode;
                    writer.WriteLine(output);
                    writer.Flush();
                    writer.Close();
                }
            }
            catch (Exception)
            {
                // do nothing
            }
        }

        #endregion

        #region Create streetlight methods

        private IFeatureCursor GetStreetlight(IFeature sourceRow, string sourceFieldName, string gisFieldName, bool useUpdateCursor = false, bool logMatches = true)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor slCursor = null;

            // Get the field value from the source
            string val = GetFieldValue(sourceRow, sourceFieldName, logMatches);
            if (val != "")
            {
                // Query the target
                IQueryFilter gisIdFilter = new QueryFilterClass();
                gisIdFilter.WhereClause = gisFieldName + "='" + val + "'";
                if (useUpdateCursor == false)
                {
                    slCursor = StreetlightFc.Search(gisIdFilter, true);
                }
                else
                {
                    slCursor = StreetlightFc.Update(gisIdFilter, true);
                }
            }
            else
            {
                _logger.Info("Could not find streetlight with " + gisFieldName + ": '" + val + "' in Inventory table");
            }

            // Return the result
            return slCursor;
        }

        private IFeatureCursor GetStreetlightByBadge(IFeature sourceSystemFeature, bool useSpatialFilter = true, int radius = 100, bool log = true)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            if (log == true)
            {
                _logger.Info("Checking for nearby streetlights in GIS by badge and office");
            }

            IFeatureCursor featureCursor = null;

            // Get the source attribute values
            string badge = GetFieldValue(sourceSystemFeature, INV_FIELD_BADGE);
            string fixCode = GetFieldValue(sourceSystemFeature, INV_FIELD_FIXTURE);
            string office = GetFieldValue(sourceSystemFeature, INV_FIELD_OFFICE);
            if (badge != "" && fixCode != "" && office != "")
            {
                int result;
                if (int.TryParse(fixCode, out result) == false)
                {
                    _logger.Warn("Invalid fixture code found in Inventory table: '" + fixCode + "'. Using default value of 1.");
                    fixCode = "1";
                }

                // Search for a matching streetlight
                if (useSpatialFilter == true)
                {
                    ISpatialFilter radiusFilter = new SpatialFilterClass();
                    ITopologicalOperator topOp = sourceSystemFeature.ShapeCopy as ITopologicalOperator;
                    radiusFilter.Geometry = topOp.Buffer(radius);
                    radiusFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    radiusFilter.WhereClause = GIS_FIELD_BADGE + "='" + badge + "' AND " + GIS_FIELD_FIXTURE + "=" + fixCode + " AND " + GIS_FIELD_OFFICE + "='" + office + "'";
                    featureCursor = StreetlightFc.Update(radiusFilter, false);
                }
                else
                {
                    IQueryFilter queryFilter = new QueryFilterClass();
                    queryFilter.WhereClause = GIS_FIELD_BADGE + "='" + badge + "' AND " + GIS_FIELD_FIXTURE + "=" + fixCode + " AND " + GIS_FIELD_OFFICE + "='" + office + "'";
                    featureCursor = StreetlightFc.Update(queryFilter, false);
                }
            }
            else
            {
                _logger.Warn("Could not get streetlight by badge because some attributes are unset");
            }

            // Return the result
            return featureCursor;
        }

        private IFeatureCursor GetStreetlightInvByBadge(IFeature sourceSystemFeature)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor featureCursor = null;

            // Get the source attribute values
            string badge = GetFieldValue(sourceSystemFeature, INV_FIELD_BADGE);
            string fixCode = GetFieldValue(sourceSystemFeature, INV_FIELD_FIXTURE);
            string office = GetFieldValue(sourceSystemFeature, INV_FIELD_OFFICE);
            if (badge != "" && fixCode != "" && office != "")
            {
                int result;
                if (int.TryParse(fixCode, out result) == false)
                {
                    _logger.Warn("Invalid fixture code found in Inventory table: '" + fixCode + "'. Using default value of 1.");
                    fixCode = "1";
                }

                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = StreetlightInv.INV_FIELD_BADGENO + "='" + badge + "' AND " + StreetlightInv.INV_FIELD_FIXTURECODE + "='" + fixCode + "' AND " + StreetlightInv.INV_FIELD_OFFICE + "='" + office + "'";
                featureCursor = StreetlightInvFc.Update(queryFilter, false);
            }
            else
            {
                _logger.Warn("Could not get streetlight by badge because some attributes are unset");
            }

            // Return the result
            return featureCursor;
        }

        private void UpdateExistingSl(IFeature streetlight, IFeature sourceRow)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Match found. Updating streetlight with OID: " + streetlight.OID.ToString());

            // Update the streetlight
            ((IWorkspaceEdit)_workspace).StartEditOperation();
            UpdateValue(sourceRow, INV_FIELD_GISID, streetlight, GIS_FIELD_GISID);
            UpdateValue(sourceRow, INV_FIELD_SPID, streetlight, GIS_FIELD_SPID);
            streetlight.Store();
            ((IWorkspaceEdit)_workspace).StopEditOperation();
            _logger.Info("Update complete");
        }

        private void CreateNewSl(IFeature sourceRow)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Match not found, creating new streetlight");

            IFeatureCursor nearbyElectrolierCursor = null;

            try
            {
                // Find out where to add the streetlight
                string feeder = string.Empty;
                IPoint slInsertionPoint = GetStreetlightInsertionPoint(sourceRow, out feeder);

                // Add it
                IFeature newGisStreetlight = CreateSlFromInv(sourceRow, slInsertionPoint, feeder);
                _logger.Info("New Streetlight added with OID: " + newGisStreetlight.OID.ToString());

                // Check to see if the new SL got related to a structure and if not, add one
                string structureFieldValue = GetFieldValue(newGisStreetlight, GIS_FIELD_STRUCTUREGUID);
                if (structureFieldValue == null || structureFieldValue == String.Empty)
                {
                    // Create a new Electrolier Support structure
                    _logger.Info("No relatable support structure found, creating one...");
                    IFeature newGisElectrolier = CreateElectrolier(newGisStreetlight);
                    _logger.Info("New support structure added with OID: " + newGisElectrolier.OID.ToString());
                }
                else
                {
                    _logger.Info("Existing support structure found with GUID: " + structureFieldValue);

                    // Do we need to report it?
                    if (HasMultipleMatchingSl(newGisStreetlight, structureFieldValue) == true)
                    {
                        WriteSuspect(newGisStreetlight);
                    }
                }


                _logger.Info("Streetlight Add Complete");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (nearbyElectrolierCursor != null) Marshal.FinalReleaseComObject(nearbyElectrolierCursor);
            }

        }

        private bool HasMultipleMatchingSl(IFeature streetlight, string structureId)
        {
            bool hasMultiple = false;

            try
            {
                // Get the fixture code to look for
                string fixCode = GetFieldValue(streetlight, GIS_FIELD_FIXTURE);

                // Get all streetlights hanging off of the structure
                IQueryFilter qfSupport = new QueryFilterClass();
                qfSupport.WhereClause = "GLOBALID='" + structureId + "'";
                IFeatureCursor supportCursor = SupportStructureFc.Search(qfSupport, true);
                IFeature structure = supportCursor.NextFeature();
                //IFeature structure = SupportStructureFc.GetFeature(int.Parse(structureId));
                IRelationshipClass structToStreetlightRel = GetRelationship(streetlight);
                ISet relatedStreetlights = structToStreetlightRel.GetObjectsRelatedToObject(structure);

                // Check fixture code on each for matches - we don't want two with same fix code
                IFeature relatedStreetlight = relatedStreetlights.Next() as IFeature;
                while (relatedStreetlight != null)
                {
                    if (relatedStreetlight.OID != streetlight.OID)
                    {
                        string newFixCode = GetFieldValue(relatedStreetlight, GIS_FIELD_FIXTURE);
                        if (newFixCode == fixCode)
                        {
                            hasMultiple = true;
                            break;
                        }
                    }

                    // Get the next one
                    relatedStreetlight = relatedStreetlights.Next() as IFeature;
                }

            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to test for multiple matching SL hanging off of one structure: " + ex.ToString());
            }

            // Return the result
            return hasMultiple;
        }

        private IPoint GetStreetlightInsertionPoint(IFeature sourceRow, out string secondaryFeeder)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IPoint slInsertionPoint = null;
            IFeatureCursor nearbySecOhCursor = null;
            IFeatureCursor nearbySecUgCursor = null;
            secondaryFeeder = string.Empty;

            try
            {
                // If the new feature will be within 6 feet of OH secondary                
                _logger.Info("Checking for nearby Secondary feature");
                ISpatialFilter nearbySecondaryFilter = new SpatialFilterClass();
                ITopologicalOperator topOp = sourceRow.ShapeCopy as ITopologicalOperator;
                nearbySecondaryFilter.Geometry = topOp.Buffer(6);
                nearbySecondaryFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                nearbySecOhCursor = SecondaryOhFc.Search(nearbySecondaryFilter, true);
                IFeature sec = nearbySecOhCursor.NextFeature();

                // If not, try UG as well
                if (sec == null)
                {
                    nearbySecUgCursor = SecondaryUgFc.Search(nearbySecondaryFilter, true);
                    sec = nearbySecUgCursor.NextFeature();
                }

                // Project the staging point to match the GIS (has same coordinate system but different projection)
                IGeometry projectedGeometry = sourceRow.ShapeCopy;
                projectedGeometry.Project((StreetlightFc as IGeoDataset).SpatialReference);
                IPoint projectedPoint = projectedGeometry as IPoint;

                if (sec != null)
                {
                    _logger.Info("Secondary found. Finding point on secondary to add streetlight...");
                    secondaryFeeder = GetFieldValue(sec, "CIRCUITID");

                    // Find the nearest point on the secondary
                    IProximityOperator proxOp = sec.ShapeCopy as IProximityOperator;
                    slInsertionPoint = new PointClass();
                    proxOp.QueryNearestPoint(projectedGeometry as IPoint, esriSegmentExtension.esriNoExtension, slInsertionPoint);
                    _logger.Debug("Linear geometry has OID: " + sec.OID);
                    _logger.Debug("Input point is at: " + projectedPoint.X.ToString() + ", " + projectedPoint.Y.ToString());
                    _logger.Debug("Point found is at: " + slInsertionPoint.X.ToString() + ", " + slInsertionPoint.Y.ToString());

                    // If existing junction on secondary, place new feature with offset
                    bool atJunctionLocation = IsAtJunction(slInsertionPoint, sec);
                    int iCount = 0; // Temp bypass
                    while (atJunctionLocation == true && iCount < 0) // What if we get to the other end of the line?
                    {
                        iCount++;
                        _logger.Info("Existing junction found, offseting...");
                        slInsertionPoint = OffsetLocation(slInsertionPoint, sec);
                        atJunctionLocation = IsAtJunction(slInsertionPoint, sec);
                        _logger.Info("Moved point to: " + slInsertionPoint.X.ToString() + ", " + slInsertionPoint.Y.ToString());
                    }
                }
                else
                {
                    // Place new feature in space
                    _logger.Info("Secondary not found, creating new streetlight in space");
                    slInsertionPoint = projectedPoint;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (nearbySecOhCursor != null) Marshal.FinalReleaseComObject(nearbySecOhCursor);
                if (nearbySecUgCursor != null) Marshal.FinalReleaseComObject(nearbySecUgCursor);
            }

            return slInsertionPoint;
        }

        private bool IsAtJunction(IPoint point, IFeature secondary)
        {
            bool isAtEnd = false;

            IEdgeFeature linearNetworkFeature = secondary as IEdgeFeature;
            IJunctionFeature fromFeature = linearNetworkFeature.FromJunctionFeature;
            IGeometry fromGeom = (fromFeature as IFeature).Shape; // fromFeature.GeometryForJunctionElement;

            IRelationalOperator relOp = point as IRelationalOperator;
            isAtEnd = relOp.Equals(fromGeom);

            if (isAtEnd == false)
            {
                IJunctionFeature toFeature = linearNetworkFeature.ToJunctionFeature;
                IGeometry toGeom = (toFeature as IFeature).Shape;
                isAtEnd = relOp.Equals(toGeom);
            }

            return isAtEnd;
        }

        private IPoint OffsetLocation(IPoint point, IFeature secondary)
        {
            // Get the line            
            IPolyline line = secondary.Shape as IPolyline;

            // Get a point up the line
            double distanceFromCurve = 0;
            double distanceAlongCurve = 0;
            IPoint closestPoint = new PointClass();
            line.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, point, false, closestPoint, ref distanceAlongCurve, ref distanceFromCurve, false);
            IPoint newLocation = new PointClass();
            line.QueryPoint(esriSegmentExtension.esriNoExtension, distanceAlongCurve + 0.5, false, newLocation);

            // Return the result
            return newLocation;
        }

        private IFeature CreateSlFromInv(IFeature source, IPoint location, string feeder)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Creating new GIS streetlight from staging table");
            IFeature sl = null;

            try
            {
                // Create feature, set shape and subtype
                sl = StreetlightFc.CreateFeature();
                sl.Shape = location;
                ISubtypes slSubtypes = StreetlightFc as ISubtypes;
                IRowSubtypes newStreetlightSubtype = sl as IRowSubtypes;
                if (slSubtypes.HasSubtype == true)
                {
                    newStreetlightSubtype.SubtypeCode = slSubtypes.DefaultSubtypeCode; // Street light
                    newStreetlightSubtype.InitDefaultValues();
                }

                //if (feeder != string.Empty)
                //{
                //    UpdateValue(sl, "CIRCUITID", feeder);
                //}

                // Clone field data from the staging table...
                int gisFieldIndex = 0;
                foreach (string field in INV_STAGING_FIELDS)
                {
                    // Get source value
                    string val = GetFieldValue(source, field);
                    if (val != string.Empty || (val == string.Empty && field == "STATUS"))
                    {
                        try
                        {
                            // Find the field in the GIS table
                            int index = StreetlightFc.FindField(INV_FIELDS[gisFieldIndex]);
                            if (index > -1)
                            {
                                // If the field has a domain
                                if (StreetlightFc.Fields.Field[index].Domain != null && StreetlightFc.Fields.Field[index].Domain.Type == esriDomainType.esriDTCodedValue)
                                {
                                    // Default the status to active is its not set
                                    if (INV_FIELDS[gisFieldIndex] == "STATUS")
                                    {
                                        if (val == string.Empty)
                                        {
                                            val = "A";
                                        }
                                    }
                                    SetDomainField(sl, index, gisFieldIndex, val);
                                }
                                else
                                {
                                    SetNonDomainField(sl, index, gisFieldIndex, val);
                                }

                                // Special handling for the year installed
                                if (INV_FIELDS[gisFieldIndex] == "INSTALLATIONDATE")
                                {
                                    if (val.Length > 4)
                                    {
                                        try
                                        {
                                            int loc = val.LastIndexOf("/");
                                            int year = int.Parse(val.Substring(loc + 1, 4));
                                            int yearInst = StreetlightFc.FindField("INSTALLJOBYEAR");
                                            if (yearInst > -1)
                                            {
                                                sl.set_Value(yearInst, year);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Warn("Bad year detected (" + val + "). Skipping setting of installjobyear: " + ex.ToString());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                _logger.Warn("Failed to set GIS field '" + INV_FIELDS[gisFieldIndex] + ". Field not found.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Warn("Failed to set GIS field '" + INV_FIELDS[gisFieldIndex] + "' with value '" + val + "' source from INV field '" + field + "'");
                            _logger.Debug(ex.ToString());
                        }
                    }
                    gisFieldIndex++;
                }

                // Save
                ((IWorkspaceEdit)_workspace).StartEditOperation();
                sl.Store();
                ((IWorkspaceEdit)_workspace).StopEditOperation();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Return the result
            return sl;
        }

        private void SetDomainField(IFeature sl, int index, int gisFieldIndex, string val)
        {
            // The source of our pain... domains... assume we'll use the domain code (vs description) and we don't need a map (values in source matches the GIS)
            bool bUseCode = true;
            bool bUseMap = false;

            // Check to see if we need to map the value
            DomainMap map = null;
            if (DomainMapping.ContainsKey(StreetlightFc.Fields.Field[index].Domain.Name) == true)
            {
                map = DomainMapping[StreetlightFc.Fields.Field[index].Domain.Name];
                if (map.UseDescription == true)
                {
                    bUseCode = false;
                }
                if (map.UseMapping == true)
                {
                    bUseMap = true;
                }
            }

            // Set the field based on those flags
            if (bUseCode == true && bUseMap == false)
            {
                _logger.Debug("Set field '" + INV_FIELDS[gisFieldIndex] + "' to field value of '" + val);
                sl.set_Value(index, val);
            }
            else if (bUseCode == true && bUseMap == true)
            {
                if (map.Mappings.ContainsKey(val) == true)
                {
                    val = map.Mappings[val];
                    _logger.Debug("Set field '" + INV_FIELDS[gisFieldIndex] + "' to field value of '" + val + "' [" + val + "]");
                    sl.set_Value(index, val);
                }
                else
                {
                    _logger.Warn("Failed to set domain field '" + INV_FIELDS[gisFieldIndex] + "'. Field value of '" + val + "' not found in domain");
                    WriteBadDomainValue(sl, INV_FIELDS[gisFieldIndex], val);
                }
            }
            else
            {
                if (bUseMap == true)
                {
                    val = map.Mappings[val];
                }

                bool bFound = false;
                // Convert the field value
                ICodedValueDomain domain = StreetlightFc.Fields.Field[index].Domain as ICodedValueDomain;
                for (int domainIndex = 0; domainIndex < domain.CodeCount; domainIndex++)
                {
                    if (domain.get_Name(domainIndex) == val)
                    {
                        bFound = true;
                        _logger.Debug("Set field '" + INV_FIELDS[gisFieldIndex] + "' to field value of '" + domain.get_Value(domainIndex) + "' [" + val + "]");
                        sl.set_Value(index, domain.get_Value(domainIndex));
                        break;
                    }
                }

                if (bFound == false)
                {
                    _logger.Warn("Failed to set domain field '" + INV_FIELDS[gisFieldIndex] + "'. Field value of '" + val + "' not found in domain");
                    WriteBadDomainValue(sl, INV_FIELDS[gisFieldIndex], val);
                }
            }
        }

        private void SetNonDomainField(IFeature sl, int index, int gisFieldIndex, string val)
        {
            // Sometimes we can't proceed, like if the value is too long for the field length
            bool bProceed = true;

            if (StreetlightFc.Fields.Field[index].Type == esriFieldType.esriFieldTypeString)
            {
                if (StreetlightFc.Fields.Field[index].Length < val.Length)
                {
                    bProceed = false;
                    _logger.Warn("Failed to set field '" + INV_FIELDS[gisFieldIndex] + "'. Field value of '" + val + "' is too long");
                }
            }

            // Update the field 
            if (bProceed == true)
            {
                _logger.Debug("Set field '" + INV_FIELDS[gisFieldIndex] + "' to field value of '" + val + "'");
                sl.set_Value(index, val);
            }
        }

        private IFeature CreateElectrolier(IFeature streetlight)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Create a new Support Structure and set the subtype and shape
            IFeature newElectrolier = SupportStructureFc.CreateFeature();
            ISubtypes supportStructSubtypes = SupportStructureFc as ISubtypes;
            IRowSubtypes newElectrolierSubtype = newElectrolier as IRowSubtypes;
            if (supportStructSubtypes.HasSubtype == true)
            {
                newElectrolierSubtype.SubtypeCode = GIS_VALUE_SUBTYPE_OTHER;
                newElectrolierSubtype.InitDefaultValues();
            }
            newElectrolier.Shape = streetlight.ShapeCopy;

            // Set some other fields
            UpdateValue(newElectrolier, GIS_FIELD_POLEUSE, GIS_VALUE_POLEUSE_STREETLIGHT);
            UpdateValue(newElectrolier, GIS_FIELD_INSTALLJOBNO, GIS_VALUE_INSTALLJOBNO);
            UpdateValue(newElectrolier, GIS_FIELD_MATERIAL, GIS_VALUE_MATERIAL_UNKNOWN);
            UpdateValue(newElectrolier, GIS_FIELD_JOINTCOUNT, 1);
            UpdateValue(newElectrolier, GIS_FIELD_STATUS, GIS_VALUE_STATUS_INSERVICE);

            // Special handling for customer owned
            string custOwned = GetCustomerOwned(streetlight);
            UpdateValue(newElectrolier, GIS_FIELD_CUSTOMEROWNED, custOwned);

            // Save and relate to the streetlight
            ((IWorkspaceEdit)_workspace).StartEditOperation();
            newElectrolier.Store();
            IRelationshipClass structToStreetlightRel = GetRelationship(streetlight);
            IRelationship rel = structToStreetlightRel.CreateRelationship(newElectrolier as IObject, streetlight as IObject);
            ((IWorkspaceEdit)_workspace).StopEditOperation();

            // Return the result
            return newElectrolier;
        }

        private string GetCustomerOwned(IFeature streetlight)
        {
            string custOwned = string.Empty;

            try
            {
                // Build a rate schedule mapping
                IDictionary<string, string> dicRates = new Dictionary<string, string>();
                dicRates.Add("LS2-A", "Y");
                dicRates.Add("LS1-A", "N");
                dicRates.Add("LS1-E", "N");
                dicRates.Add("LS1-C", "N");
                dicRates.Add("LS1-D", "N");
                dicRates.Add("LS1-F", "N");
                dicRates.Add("LS2-C", "Y");
                dicRates.Add("OL1", "N");
                dicRates.Add("LS1-B", "N");
                dicRates.Add("LS2-B", "Y");
                dicRates.Add("LS3", "Y");
                dicRates.Add("LS1-F1", "N");

                // Get the rate schedule
                string rateSchedule = GetFieldValue(streetlight, "RATESCHEDULE");

                // Map it to the customer owned value
                if (dicRates.ContainsKey(rateSchedule) == true)
                {
                    custOwned = dicRates[rateSchedule];
                }
                else
                {
                    _logger.Warn("Unable to determine if new streetlight is customer owned (Rate schedule: " + rateSchedule + ")");
                }
            }
            catch (Exception ex)
            {
                _logger.Warn("Unable to determine if new streetlight is customer owned. Error occured: " + ex.ToString());
            }

            // Return the result
            return custOwned;
        }

        private void UpdateValue(IFeature feature, string field, object value)
        {
            int fieldIndex = feature.Class.FindField(field);
            if (fieldIndex != -1)
            {
                feature.set_Value(fieldIndex, value);
            }
        }

        private IRelationshipClass GetRelationship(IFeature streetlight)
        {
            IEnumRelationshipClass enumRelClass = streetlight.Class.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            IRelationshipClass rel = enumRelClass.Next();
            while (rel.OriginClass != SupportStructureFc)
            {
                rel = enumRelClass.Next();
            }

            return rel;
        }

        private void DeleteExistingSl(IFeature streetlight)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Info("Deleting streetlight in GIS with OID: " + streetlight.OID.ToString());

            IFeatureCursor slCursor = null;
            IFeatureCursor stCursor = null;

            try
            {
                // Start an edit transaction
                ((IWorkspaceEdit)_workspace).StartEditOperation();

                // Check to see if an electrolier exists
                string structureFieldValue = GetFieldValue(streetlight, GIS_FIELD_STRUCTUREGUID);
                if (structureFieldValue != null && structureFieldValue != String.Empty)
                {
                    IQueryFilter qfSl = new QueryFilterClass();
                    qfSl.WhereClause = GIS_FIELD_STRUCTUREGUID + "='" + structureFieldValue + "'";
                    slCursor = StreetlightFc.Search(qfSl, true);
                    int iCount = 0;
                    IFeature sl = slCursor.NextFeature();
                    while (sl != null)
                    {
                        iCount++;
                        sl = slCursor.NextFeature();
                    }
                    _logger.Info("Streetlight has support structure with " + iCount.ToString() + " connected streetlights");

                    // If so, if it has only 1 streetlight on it
                    if (iCount == 1)
                    {
                        // Get the electrolier and delete it
                        IQueryFilter qfStruct = new QueryFilterClass();
                        qfStruct.WhereClause = "GLOBALID='" + structureFieldValue + "'";
                        stCursor = SupportStructureFc.Search(qfStruct, false);
                        IFeature structure = stCursor.NextFeature();
                        if (structure != null)
                        {
                            _logger.Info("Deleting support structure with OID: " + structure.OID.ToString());
                            structure.Delete();
                        }
                    }

                }

                // Delete the streetlight and complete the transaction
                streetlight.Delete();
                ((IWorkspaceEdit)_workspace).StopEditOperation();
                _logger.Info("Streetlight " + streetlight.OID.ToString() + " Deleted");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (slCursor != null) Marshal.FinalReleaseComObject(slCursor);
                if (stCursor != null) Marshal.FinalReleaseComObject(stCursor);
            }
        }

        #endregion

        #region Support methods

        private string GetFieldValue(IFeature feature, string fieldName, bool log = false)
        {
            string value = "";

            int fieldIndex = feature.Class.FindField(fieldName);
            if (fieldIndex > -1)
            {
                object val = feature.get_Value(fieldIndex);
                if (val != null)
                {
                    value = val.ToString().Trim();
                    if (log == true)
                    {
                        _logger.Info(fieldName + " is '" + value + "'");
                    }
                }
            }
            else
            {
                _logger.Warn("Could not find field " + fieldName + " to retrieve its value");
            }

            return value;
        }

        private string CDGetFieldValue(UpdateFeat feature, string fieldName, bool log = false)
        {
            string value = "";

            if(feature.fields_Old.ContainsKey(fieldName.ToUpper()))
            {
                value = feature.fields_Old[fieldName.ToUpper()].Trim();
                if (log == true)
                {
                    _logger.Info(fieldName + " is '" + value + "'");
                }
            }
            else
            {
                _logger.Warn("Could not find field " + fieldName + " to retrieve its value");
            }

            return value;
        }

        private void UpdateValue(IFeature source, string sourceField, IFeature target, string targetField)
        {
            // Log entry
            _logger.Debug("Updating GIS streetlight");

            // Get the source value
            string val = GetFieldValue(source, sourceField);
            if (val != string.Empty)
            {
                try
                {
                    int index = target.Class.FindField(targetField);

                    if (index > -1)
                    {
                        // If the field has a domain
                        if (target.Class.Fields.Field[index].Domain != null && StreetlightFc.Fields.Field[index].Domain.Type == esriDomainType.esriDTCodedValue)
                        {
                            bool bFound = false;
                            // Convert the field value
                            ICodedValueDomain domain = StreetlightFc.Fields.Field[index].Domain as ICodedValueDomain;
                            for (int domainIndex = 0; domainIndex < domain.CodeCount; domainIndex++)
                            {
                                if (domain.get_Name(domainIndex) == val)
                                {
                                    bFound = true;
                                    _logger.Debug("Set field '" + targetField + "' to field value of '" + domain.get_Value(domainIndex) + "' [" + val + "]");
                                    target.set_Value(index, domain.get_Value(domainIndex));
                                }
                            }

                            if (bFound == false)
                            {
                                _logger.Warn("Failed to set domain field '" + targetField + "'. Field value of '" + val + "' not found in domain");
                                WriteBadDomainValue(target, targetField, val);
                                try
                                {
                                    target.set_Value(index, val);
                                }
                                catch (Exception)
                                {
                                    // Ignore
                                    //string s = "1";
                                }
                            }
                        }
                        else
                        {
                            bool bProceed = true;
                            if (StreetlightFc.Fields.Field[index].Type == esriFieldType.esriFieldTypeString)
                            {
                                if (StreetlightFc.Fields.Field[index].Length < val.Length)
                                {
                                    bProceed = false;
                                    _logger.Warn("Failed to set field '" + targetField + "'. Field value of '" + val + "' is too long");
                                }
                            }

                            if (bProceed == true)
                            {
                                _logger.Debug("Set field '" + targetField + "' to field value of '" + val + "'");
                                target.set_Value(index, val);
                            }
                        }
                    }
                    else
                    {
                        _logger.Warn("Failed to set GIS field '" + targetField + ". Field not found.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn("Failed to set GIS field '" + targetField + "' with value '" + val + "' source from INV field '" + sourceField + "'");
                    _logger.Debug(ex.ToString());
                }
            }
        }

        private string ProcessFieldPtsField(IFeature sourceSl, string fieldName, object inputVal)
        {
            string val = inputVal.ToString();

            switch (fieldName)
            {
                case "STATUSFLAG":
                    // Map to the status field
                    string status = GetFieldValue(sourceSl, "STATUS");
                    DomainMap domainMappingState = DomainMapping["Construction Status"];
                    foreach (string key in domainMappingState.Mappings.Keys)
                    {
                        if (domainMappingState.Mappings[key] == status)
                        {
                            val = key;
                        }
                    }
                    break;

                case "STATUS":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["Construction Status"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "RECEIVEDATE":
                case "RETIREDATE":
                case "INSTALLATIONDATE":
                case "REMOVALDATE":
                case "CHANGEOFPARTYDATE":
                case "INVENTORYDATE":
                    if (inputVal != null && inputVal != DBNull.Value)
                    {
                        DateTime installDate = Convert.ToDateTime(val);
                        val = String.Format("{0:dd-MMM-yy}", installDate);
                    }
                    break;

                case "ITEMTYPECODE":
                    // But check if we want a domain value instead
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["CCBTOGIS_ITEMTYPECODE_LOOKUP"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "OPERATINGSCHEDULE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["CCBTOGIS_OPSCHEDULE_LOOKUP"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "SERVICETYPE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["Street Light Service Type"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                default:
                    val = inputVal.ToString();
                    break;

            }

            return val;
        }

        //private string ProcessCcbField(IFeature sourceSl, string fieldName, object inputVal)
        //{
        //    string val = inputVal.ToString();

        //    switch (fieldName)
        //    {
        //        case "OFFICE":
        //            // Merge transction with the raw office value
        //            //string trans = GetFieldValue(sourceSl, "STREETLIGHTTRANSACTION");
        //            //val = trans + inputVal;
        //            // Hard code transaction to update (“3”) and merge with the raw office value
        //            val = "3" + inputVal;
        //            break;

        //        case "STATUS":
        //            if (inputVal != null)
        //            {
        //                DomainMap domainMapping = DomainMapping["Construction Status"];
        //                foreach(string key in domainMapping.Mappings.Keys)
        //                {
        //                    if (domainMapping.Mappings[key] == inputVal.ToString())
        //                    {
        //                        val = key;
        //                    }
        //                }
        //            }
        //            break;

        //        case "INSTALLATIONDATE":
        //        case "INVENTORYDATE":
        //            if (inputVal != null && inputVal != DBNull.Value)
        //            {
        //                DateTime installDate = Convert.ToDateTime(val);
        //                val = String.Format("{0:yyyy-MM-dd}", installDate);
        //            }
        //            break;

        //        case "ITEMTYPECODE":
        //            if (inputVal != null)
        //            {
        //                DomainMap domainMapping = DomainMapping["CCBTOGIS_ITEMTYPECODE_LOOKUP"];
        //                foreach(string key in domainMapping.Mappings.Keys)
        //                {
        //                    if (domainMapping.Mappings[key] == inputVal.ToString())
        //                    {
        //                        val = key;
        //                    }
        //                }
        //            }
        //            break;

        //        case "SERVICETYPE":
        //            if (inputVal != null)
        //            {
        //                DomainMap domainMapping = DomainMapping["Street Light Service Type"];
        //                foreach (string key in domainMapping.Mappings.Keys)
        //                {
        //                    if (domainMapping.Mappings[key] == inputVal.ToString())
        //                    {
        //                        val = key;
        //                    }
        //                }
        //            }
        //            break;

        //        case "OPERATINGSCHEDULE":
        //            if (inputVal != null)
        //            {
        //                DomainMap domainMapping = DomainMapping["CCBTOGIS_OPSCHEDULE_LOOKUP"];
        //                foreach (string key in domainMapping.Mappings.Keys)
        //                {
        //                    if (domainMapping.Mappings[key] == inputVal.ToString())
        //                    {
        //                        val = key;
        //                    }
        //                }
        //            }
        //            break;

        //        default:
        //            val = inputVal.ToString();
        //            break;
        //    }

        //    return val;
        //}
        private string ProcessCcbField(IFeature sourceSl, string fieldName, object inputVal)
        {
            string val = inputVal.ToString();

            switch (fieldName)
            {
                case "OFFICE":
                    // Merge transction with the raw office value
                    //string trans = GetFieldValue(sourceSl, "STREETLIGHTTRANSACTION");
                    //val = trans + inputVal;
                    // Hard code transaction to update (“3”) and merge with the raw office value
                    val = "3" + inputVal;
                    break;
                ////"DIFFBADGE","DIFFFIX","DIFFADDR","DIFFMAP","DIFFRS" these field value has been set to 1 as per ME Q3 2018 requirement and confirmation frm business
                
                case "DIFFBADGE":                    
                    val = "1";
                    break;
                case "DIFFFIX":
                    val = "1";
                    break;
                case "DIFFADDR":
                    val = "1";
                    break;
                case "DIFFMAP":
                    val = "1";
                    break;
                case "DIFFRS":
                    val = "1";
                    break;                    

                case "STATUS":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["Construction Status"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "INSTALLATIONDATE":
                case "INVENTORYDATE":
                    if (inputVal != null && inputVal != DBNull.Value)
                    {
                        DateTime installDate = Convert.ToDateTime(val);
                        val = String.Format("{0:yyyy-MM-dd}", installDate);
                    }
                    break;
                case "INVENTORY_DATE":
                    if (inputVal != null && inputVal != DBNull.Value)
                    {
                        DateTime inventoryDate = Convert.ToDateTime(val);
                        val = String.Format("{0:yyyy-MM-dd}", inventoryDate);
                    }
                    break;
                case "INSTALL_DATE":
                    if (inputVal != null && inputVal != DBNull.Value)
                    {
                        DateTime installDate = Convert.ToDateTime(val);
                        val = String.Format("{0:yyyy-MM-dd}", installDate);
                    }
                    break;

                case "ITEMTYPECODE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["CCBTOGIS_ITEMTYPECODE_LOOKUP"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "SERVICETYPE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["Street Light Service Type"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "OPERATINGSCHEDULE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["CCBTOGIS_OPSCHEDULE_LOOKUP"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                default:
                    val = inputVal.ToString();
                    break;
            }

            return val;
        }
        private string ProcessInvField(IFeature sourceSl, string fieldName, object inputVal)
        {
            string val = inputVal.ToString();

            switch (fieldName)
            {
                case "STATUSFLAG":
                    // Map to the status field
                    string status = GetFieldValue(sourceSl, "STATUS");
                    DomainMap domainMappingState = DomainMapping["Construction Status"];
                    foreach (string key in domainMappingState.Mappings.Keys)
                    {
                        if (domainMappingState.Mappings[key] == status)
                        {
                            val = key;
                        }
                    }
                    break;

                case "STATUS":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["Construction Status"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "OPERATINGSCHEDULE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["CCBTOGIS_OPSCHEDULE_LOOKUP"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "ITEMTYPECODE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["CCBTOGIS_ITEMTYPECODE_LOOKUP"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                case "SERVICETYPE":
                    if (inputVal != null)
                    {
                        DomainMap domainMapping = DomainMapping["Street Light Service Type"];
                        foreach (string key in domainMapping.Mappings.Keys)
                        {
                            if (domainMapping.Mappings[key] == inputVal.ToString())
                            {
                                val = key;
                            }
                        }
                    }
                    break;

                default:
                    val = inputVal.ToString();
                    break;

            }

            return val;
        }

        #endregion

        #endregion
    }
}
