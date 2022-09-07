using System;
using System.Collections.Generic;
using System.Linq;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;

namespace PGE.Desktop.EDER.Subtasks
{
    /// <summary>
    /// A subtask to record the changes in Circuits / Substations
    /// </summary>
    [ComVisible(true)]
    [Guid("D7942851-09A6-4F26-90E6-737E0FA7314F")]
    [ProgId("PGE.Desktop.EDER.SubTask.RecordDMSChanges")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class RecordDMSChanges : BaseChangeDetectionSubtask
    {
        #region Private Variables
        /// <summary>
        /// Stores the Circuit / Substation changes
        /// </summary>
        private IChange _change = null;
        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Initialises the new instance of <see cref="RecordDMSChanges"/> class
        /// </summary>
        public RecordDMSChanges()
            : base("PGE Record DMS Changes")
        {
        }
        #endregion Constructor

        #region Protected Properties

        /// <summary>
        /// Holds an instance to read and persist changes to the tables
        /// </summary>
        protected override IChange Changedetector
        {
            get
            {
                if (_change == null) _change = new DMSChangeDetector();
                return _change;
            }
        }

        #endregion Protected Properties
    }

    /// <summary>
    /// Provides methods to read the changes and persist to the the CHANGE tables
    /// </summary>
    public class DMSChangeDetector : IChange
    {
        #region Private Variables and Constants

        /// <summary>
        /// Logs the messages
        /// </summary>
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        /// <summary>
        /// Refer to the database instance
        /// </summary>
        private IWorkspace _workspace = null;
        /// <summary>
        /// Holds a reference to the Feeder Changes table
        /// </summary>
        private ITable _feederChangeTable = null;
        /// <summary>
        /// Holds a reference to the Substation Changes table
        /// </summary>
        private ITable _substationTable = null;
        /// <summary>
        /// Holds the collection of changes in Feeders and Substations
        /// </summary>
        private List<DMSData> _changes = new List<DMSData>();
        /// <summary>
        /// Indicates whether the changes table refer to Substation Changes or Feeder Changes
        /// </summary>
        private bool _substation = false;
        /// <summary>
        /// Holds the already processed UniqueID of Feeder / Substation
        /// </summary>
        private List<int> _alreadyProcessed = new List<int>();
        /// <summary>
        /// Table Name that stores Feeder Changes
        /// </summary>
        private const string _feedertableModelName = "EDGIS.PGE_CHANGED_CIRCUIT";
        /// <summary>
        /// Table Name that stores Substation Changes
        /// </summary>
        private const string _substationTableModelName = "EDGIS.PGE_CHANGED_SUBSTATION";
        /// <summary>
        /// Object Class model name assigned to the dms feature class to record the Feeder or Substation changes
        /// </summary>
        private const string _dmsFeatureMN = "PGE_DMSFEATURE";
        /// <summary>
        /// Object Class model name assigned to the object class to record the Feeder or Substation changes
        /// </summary>
        private const string _dmsRelateMN = "PGE_DMSRELATE";
        /// <summary>
        /// Model Name to recognise Substation classes
        /// </summary>
        private const string _substationClassMN = "PGE_SUBSTATION";
        /// <summary>
        /// Model Name assigned to UniqueID field of Substation
        /// </summary>
        private const string _substationIdFldMN = "PGE_SUBSTATIONID";
        /// <summary>
        /// Model name assigned to FeederID field
        /// </summary>
        private const string _feederIDMN = "FEEDERID";
        /// <summary>
        /// Model name assigned to FeederID2 field
        /// </summary>
        private const string _feederid2MN = "FEEDERID2";
        /// <summary>
        /// Last User Field name
        /// </summary>
        private const string _lastUserIdFldName = "LASTUSER";
        /// <summary>
        /// Creation User Field name
        /// </summary>
        private const string _creationUserIdFldName = "CREATIONUSER";


        #endregion Private Variables and Constants

        #region Public Properties

        /// <summary>
        /// Refer to Changes table either PGE_CHANGED_CIRCUIT table or PGE_CHANGED_SUBSTATION table
        /// </summary>
        public ITable ChangeTable
        {
            get
            {
                if (_substation)
                {
                    if (_substationTable == null)
                    {
                        _substationTable = (_workspace as IFeatureWorkspace).OpenTable(_substationTableModelName);
                    }
                    return _substationTable;
                }
                else
                {
                    if (_feederChangeTable == null)
                    {
                        _feederChangeTable = (_workspace as IFeatureWorkspace).OpenTable(_feedertableModelName);
                    }
                    return _feederChangeTable;
                }
            }
        }

        /// <summary>
        /// Current workspace being edited
        /// </summary>
        public ESRI.ArcGIS.Geodatabase.IWorkspace Workspace
        {
            private get { return _workspace; }
            set
            {
                _workspace = value;
                _feederChangeTable = null; _substationTable = null;
            }
        }

        /// <summary>
        /// Indicates whether to consider only Feature changes to be recorded
        /// </summary>
        /// <remarks>
        /// True to consider only Features; False to consider changes on Features and Rows(non Features)
        /// </remarks>
        public bool FeatureOnly
        {
            get { return false; }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="d8List"></param>
        public void AddChange(ID8List d8List)
        {
            ID8GeoAssoc geoAssoc = (ID8GeoAssoc)d8List;
            IRow row = geoAssoc.AssociatedGeoRow;
            //Check if the Row has PGEDMSCLASS modelname if not go ahead and return
            if (!ModelNameFacade.ContainsClassModelName((IObjectClass)row.Table, new string[] { _dmsFeatureMN, _dmsRelateMN }))
            {
                return;
            }
            if (FeatureOnly && !(row is IFeature))
            {
                return;
            }
            else
            {
                _changes.AddRange(GetDMSDataforRow(row));
            }
            //Clear the processed list for the row
            if (_alreadyProcessed.Count > 0) _alreadyProcessed.Clear();
        }

        /// <summary>
        /// Persists the changes to the tables.
        /// </summary>
        /// <returns>Returns true if changed are persisted successfully; false, otherwise.</returns>
        public bool PersistChange()
        {
            bool success = false;
            //Validate the DMSChange data            
            if (_changes == null) return success = false;
            if (_changes.Count < 1) return success = true;
            DateTime postDate = DateTime.Now.Date;
            //Persist FeerderIDs
            success = InternalPersistChanges(false, postDate);
            //Persist SubstationIDs
            success = success & InternalPersistChanges(true, postDate);
            //Clear the changes on successful updation of the changes tables
            if (success) _changes.Clear();
            return success;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Process the Row to record the changes
        /// </summary>
        /// <param name="row">Row to be processed</param>
        /// <returns>Returns the collection of changes. Note that one row can have 2 changes. e.g., One row can have both FeederID and FeederID2; hence this returns the both the uniqueIDs</returns>
        private List<DMSData> GetDMSDataforRow(IRow row)
        {
            //We know if the Row is of Type IFeature the data is on the feature no need to traverse relationship
            if (row is IFeature)
            {
                return PopulateDMSData((IFeature)row);
            }
            //If the Row is not of type IFeature then we need to traverse relationship until we find a feature with modelname "PGE_DMSFEATURE"
            //Look for feature/object with modelname PGE_DMSRELATE. Once you find the relationship check if any of the feature has PGE_DMSFEATURE. 
            //If it has that then that is the IObject that should be used to get the FeederID or substationID. If not get the relationship from that related object that has the modelname "PGE_DMSRELATE"
            else
            {
                IFeature rowToProcess = GetDMSFeature((IObject)row);
                return PopulateDMSData(rowToProcess);
            }
        }

        /// <summary>
        /// If the Row is not of type IFeature then we need to traverse relationship until we find a feature with modelname "PGE_DMSFEATURE"
        /// Look for feature/object with modelname PGE_DMSRELATE. Once you find the relationship check if any of the feature has PGE_DMSFEATURE. 
        /// If it has that then that is the IObject that should be used to get the FeederID or substationID. If not get the relationship from that related object that has the modelname "PGE_DMSRELATE"
        /// This way we can traverse and get upto n relationship. For eg. Controller is sent to DMS Controller is related to VoltageRegulatorUnit which in turn is related to VoltageRegulator.
        /// So voltageRegulator and VoltageRegulatorUnit will have modelnames "PGE_DMSRELATE" and VoltageRegulator will have modelname "PGE_DMSFEATURE"
        /// The code will find the voltageRegulatorUnit using the PGE_DMSRELATE modelname then since the VoltageRegulatorUnit does not have PGE_DMSFEATURE modelname will get all the other relationship that has the PGE_DMSRELATE modelname. while doing so will ignore the VoltageRegulatorUnit class as the search is performed using voltageRegulatorUnit.
        /// It will find the voltageRegulator featureclass which will have PGE_DMSFEATURE so that would be used to get the FeederID or substationid for persisting
        /// This method is called recursively
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private IFeature GetDMSFeature(IObject row)
        {
            //to avoid Multiple loops of same relationshipclass
            //Test this thorough. 
            _alreadyProcessed.Add(row.Class.ObjectClassID);
            List<IRelationshipClass> relatedClasses = RelationshipWithModelNameIgnoreAlreadyProcessed(row.Class, _dmsRelateMN);
            ISet relatedSet = null;
            object relatedObj = null;
            foreach (IRelationshipClass relClass in relatedClasses)
            {
                relatedSet = relClass.GetObjectsRelatedToObject(row);
                relatedSet.Reset();
                relatedObj = relatedSet.Next();
                if (relatedObj != null)
                {
                    if (ModelNameFacade.ContainsClassModelName(((IObject)relatedObj).Class, _dmsFeatureMN) && relatedObj is IFeature)
                    {
                        return (IFeature)relatedObj;
                    }
                    else
                    {
                        return GetDMSFeature((IObject)relatedObj);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Prepares a list to record the changes.
        /// If the feature is a substation then record the DMSData with UniqueID equals to SubstationID.
        /// If the feature participates in a feeder then record the 2 DMSData records with UniqueID equals to FeederID and FeederID2
        /// </summary>
        /// <param name="feature">Features to record the Unique IDs</param>
        /// <returns>Returns the list of DMS data containing the UniqueID of the changes</returns>
        private List<DMSData> PopulateDMSData(IFeature feature)
        {
            List<DMSData> retVal = new List<DMSData>();
            if (feature == null) return retVal;

            //Get LastUserName, if this is null then get CreationUserName
            string userName = feature.GetFieldValue(_lastUserIdFldName, false, null).Convert(string.Empty);
            if (userName == string.Empty)
            {
                userName = feature.GetFieldValue(_creationUserIdFldName, false, null).Convert(string.Empty);
            }

            //Check if the feature is a substation
            if (ModelNameFacade.ContainsClassModelName(feature.Class, _substationClassMN))
            {
                string substationId = feature.GetFieldValue(null, false, _substationIdFldMN).Convert(string.Empty);
                if (!string.IsNullOrEmpty(substationId))
                {
                    DMSData dmsdata = new DMSData();
                    dmsdata.IsSubstation = true;
                    dmsdata.UniqueID = substationId;
                    dmsdata.UserName = userName;
                    retVal.Add(dmsdata);
                }
            }
            else
            {
                //Check for FeederID and FeederID2 field and populate the DMSData for each
                string feederid = feature.GetFieldValue(null, false, _feederIDMN).Convert(string.Empty);
                if (!string.IsNullOrEmpty(feederid))
                {
                    DMSData dmsdata = new DMSData();
                    dmsdata.IsSubstation = false;
                    dmsdata.UniqueID = feederid;
                    dmsdata.UserName = userName;
                    retVal.Add(dmsdata);
                }
                string feederid2 = feature.GetFieldValue(null, false, _feederid2MN).Convert(string.Empty);
                if (!string.IsNullOrEmpty(feederid2))
                {
                    DMSData dmsdata = new DMSData();
                    dmsdata.IsSubstation = false;
                    dmsdata.UniqueID = feederid2;
                    dmsdata.UserName = userName;
                    retVal.Add(dmsdata);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Gets the Relationship by modelname the difference here is filter the class that is passed in.
        /// </summary>
        /// <param name="objectClass">Object class to get its related classes</param>
        /// <param name="modelname">Model name assigned to the related class</param>
        /// <returns>Returns the list of the classes assigned with <paramref name="modelname"/> and related to the <paramref name="objectClass"/> class. 
        /// This also ignores unncessary looping from parent to child and back to parent though the model names are wrongly assigned</returns>
        private List<IRelationshipClass> RelationshipWithModelNameIgnoreAlreadyProcessed(IObjectClass objectClass, string modelname)
        {
            List<IRelationshipClass> retVal = new List<IRelationshipClass>();
            IEnumerable<IRelationshipClass> relClasses = objectClass.RelationshipClasses[esriRelRole.esriRelRoleAny].AsEnumerable();
            IObjectClass classtocheck = null;
            foreach (IRelationshipClass relClass in relClasses)
            {
                if (relClass.DestinationClass.ObjectClassID == objectClass.ObjectClassID)
                {
                    classtocheck = relClass.OriginClass;
                }
                else
                {
                    classtocheck = relClass.DestinationClass;
                }
                //To avoid Multple loop of same RelationshipClass
                if (!_alreadyProcessed.Contains(classtocheck.ObjectClassID) && ModelNameFacade.ContainsClassModelName(classtocheck, modelname))
                {
                    retVal.Add(relClass);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Persists the changes to the FeederChanges / SubstationChanges tables
        /// </summary>
        /// <param name="substation">True to persist changes to SubstationChanges table; else persist changes to FeederChanges table</param>
        /// <param name="postDate">Date Time written to the changes tables</param>
        /// <returns>True if the process successfully writes the changes; false, otherwise</returns>
        private bool InternalPersistChanges(bool substation, DateTime postDate)
        {
            ICursor insertCursor = null;
            IRowBuffer newRowBuffer = null;
            bool success = false;
            try
            {
                //Get the unique ID changes
                List<DMSData> dmsChanges = _changes.Where(change => change.IsSubstation == substation)
                    .Distinct()
                    .ToList();
                if (dmsChanges.Count < 1) return true;
                _substation = substation;
                //Get the UniqueID Field Name
                string UniqueIDFiedlName = substation ? ChangeLookupFields.SubstationIDFieldName : ChangeLookupFields.CircuitIDFieldName;

                //Validate the table
                if (ChangeTable == null) return false;

                //Get the field indexes
                int uniqueIdFldIx = ChangeTable.FindField(UniqueIDFiedlName);
                int postDateFldIx = ChangeTable.FindField(ChangeLookupFields.PostDateFieldName);
                int userIdFldIx = ChangeTable.FindField(ChangeLookupFields.UserIDFieldName);
                int actionFldIx = ChangeTable.FindField(ChangeLookupFields.ActionFieldName);
                if (uniqueIdFldIx == -1 || postDateFldIx == -1 || userIdFldIx == -1 || (!substation && actionFldIx == -1))
                {
                    _logger.Info((ChangeTable as IDataset).Name + " does not have all the mandatory fields.");
                    return false;
                }

                //Get the insert cursor on the table
                insertCursor = ChangeTable.Insert(true);
                newRowBuffer = ChangeTable.CreateRowBuffer();
                //Set post which is same for all records
                newRowBuffer.set_Value(postDateFldIx, postDate);
                if (!substation) newRowBuffer.set_Value(actionFldIx, "UPDATE");
                foreach (DMSData changeData in dmsChanges)
                {
                    //Set the remaining values
                    newRowBuffer.set_Value(uniqueIdFldIx, changeData.UniqueID);
                    newRowBuffer.set_Value(userIdFldIx, changeData.UserName);
                    //Insert the row into cursor
                    insertCursor.InsertRow(newRowBuffer);
                }
                //Flush the cursor
                insertCursor.Flush();
                success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                success = false;
            }
            finally
            {
                if (newRowBuffer != null)
                {
                    while (Marshal.ReleaseComObject(newRowBuffer) > 0) { };
                }
                if (insertCursor != null)
                {
                    while (Marshal.ReleaseComObject(insertCursor) > 0) { };
                }
            }
            return success;
        }

        #endregion Private Methods
    }

    /// <summary>
    /// Class that stores the UniqueID of changes
    /// </summary>
    public class DMSData
    {
        public string UserName;
        public string UniqueID;
        public bool IsSubstation;
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is DMSData)) return false;
            int compare = string.Compare(UniqueID, (obj as DMSData).UniqueID);
            return (compare == 0);
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrEmpty(UniqueID)) { return base.GetHashCode(); }
            else return UniqueID.GetHashCode();
        }
    }

    /// <summary>
    /// Field Names in the Changes tables
    /// </summary>
    public struct ChangeLookupFields
    {
        /// <summary>
        /// Circuit Field Name in Feeder Changes table
        /// </summary>
        public const string CircuitIDFieldName = "CIRCUITID";
        /// <summary>
        /// SubstationID Field Name in Substation Changes table
        /// </summary>
        public const string SubstationIDFieldName = "SUBSTATIONID";
        /// <summary>
        /// Post Date field in Feeder and Substation Changes table
        /// </summary>
        public const string PostDateFieldName = "POSTDATE";
        /// <summary>
        /// User Name field in Feeder and Substation Changes table
        /// </summary>
        public const string UserIDFieldName = "USERID";
        /// <summary>
        /// Action Field Name in Feeder Changes table
        /// </summary>
        public const string ActionFieldName = "CHANGED_ACTION";
    }
}
