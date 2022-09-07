using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.Integration.Framework.Utilities;
using System.Reflection;
using Diagnostics = PGE.Common.Delivery.Diagnostics;
namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Data structure for storing all the information and logic for mapping data from one system to another, i.e. GIS -> SAP
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class SystemMapper
    {
        #region Private Members
        private string _mapperType;

        private string _domainXMLPath;
        private string _dataWriter;

        private List<TrackedClass> _trackedClasses;
        private Settings _settings;

        private List<ReverseSyncedClass> _reverseSyncedClasses;

        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        #endregion

        #region Public Serializable Members
        /// <summary>
        /// Not currently used.
        /// </summary>
        public string MapperType
        {
            get
            {
                return this._mapperType;
            }
            set
            {
                this._mapperType = value;
            }
        }

        /// <summary>
        /// Path to the XML file containing value mappings
        /// </summary>
        public string DomainXMLPath
        {
            get
            {
                return this._domainXMLPath;
            }
            set
            {
                this._domainXMLPath = value;
            }
        }

        /// <summary>
        /// Not currently used
        /// </summary>
        public string DataWriter
        {
            get
            {
                return this._dataWriter;
            }
            set
            {
                this._dataWriter = value;
            }
        }
        /// <summary>
        /// List of tracked classes. These are each of the GIS class that data will be extracted from.
        /// </summary>
        [XmlArrayItemAttribute("TrackedClass", typeof(TrackedClass), IsNullable = false)]
        public List<TrackedClass> TrackedClasses
        {
            get
            {
                return this._trackedClasses;
            }
            set
            {
                this._trackedClasses = new List<TrackedClass>(value);
            }
        }

        /// <summary>
        /// List of Settings for the SystemMapper
        /// </summary>
        public Settings Settings
        {
            get
            {
                return this._settings;
            }
            set
            {
                this._settings = value;
            }
        }


        /// <summary>
        /// List of tracked classes. These are each of the GIS class that data will be extracted from.
        /// </summary>
        [XmlArrayItemAttribute("ReverseSyncedClass", typeof(ReverseSyncedClass), IsNullable = false)]
        public List<ReverseSyncedClass> ReverseSyncedClasses
        {
            get
            {
                return this._reverseSyncedClasses;
            }
            set
            {
                this._reverseSyncedClasses = new List<ReverseSyncedClass>(value);
            }
        }

        #endregion
        #region Pulic Non-serializable

        /// <summary>
        /// Cached Asset IDs that have already been processed
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, Dictionary<string, IRowData>> AssetIDandRowDataByOutName
        {
            get;
            set;
        }

        /// <summary>
        /// Store Object Id and its fields that failed transformation by source name
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, Dictionary<int, List<string>>> ObjectIDandFailedFieldsBySourceName
        {
            get;
            set;
        }

        /// <summary>
        /// Cached Device Group IDs that have been processed
        /// </summary>
        [XmlIgnore]
        public List<string> DeviceGroupGlobalIDs
        {
            get;
            set;
        }
        /// <summary>
        /// Cached Relationship classes so they do not have to be repeatedly opened and closed
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, IRelationshipClass2> RelationshipClassByNameInEditVersion
        {
            get;
            set;
        }
        /// <summary>
        /// Cached Relationship classes so they do not have to be repeatedly opened and closed
        /// </summary>
        [XmlIgnore]
        public Dictionary<string, IRelationshipClass2> RelationshipClassByNameInTargetVersion
        {
            get;
            set;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize this SystemMapper and its TrackedClasses. Must be called before using the SystemMapper.
        /// </summary>
        public void Initialize()
        {
            if (TrackedClasses != null)
            {
                foreach (TrackedClass trackedCls in TrackedClasses)
                {
                   // _logger .Debug (string.Format ("outName={0}, TransformerType={1}",trackedCls .OutName ,trackedCls .TransformerType ));
                    trackedCls.Initialize(this);
                }
            }

            //ReverseSync
            if (ReverseSyncedClasses != null)
            {
                foreach (ReverseSyncedClass reverseSyncCls in ReverseSyncedClasses)
                {
                    // _logger .Debug (string.Format ("outName={0}, TransformerType={1}",trackedCls .OutName ,trackedCls .TransformerType ));
                    reverseSyncCls.Initialize(this);
                }
            }

            AssetIDandRowDataByOutName = new Dictionary<string, Dictionary<string, IRowData>>();
            ObjectIDandFailedFieldsBySourceName = new Dictionary<string, Dictionary<int, List<string>>>();

            DeviceGroupGlobalIDs = new List<string>();
            RelationshipClassByNameInEditVersion = new Dictionary<string, IRelationshipClass2>();
            RelationshipClassByNameInTargetVersion = new Dictionary<string, IRelationshipClass2>();
        }
        /// <summary>
        /// Add a Setting to the SystemMapper
        /// </summary>
        /// <param name="setting">The Setting to add</param>
        public void AddSetting(Setting setting)
        {
            if (_settings == null)
            {
                _settings = new Settings();
                _settings.SettingList = new List<Setting>(); 
            }
            if(_settings.SettingList.FindIndex(s=>string.Equals(s.Name,setting.Name,StringComparison.InvariantCultureIgnoreCase))<0) 
                _settings.SettingList.Add(setting);
        }
        /// <summary>
        /// Add a TrackedClass to the SystemMapper
        /// </summary>
        /// <param name="trackedClass">The TrackedClass to add</param>
        public void AddClass(TrackedClass trackedClass)
        {
            if (_trackedClasses == null)
            {
                _trackedClasses = new List<TrackedClass>();
            }
            _trackedClasses.Add(trackedClass);
        }
        /// <summary>
        /// Get a TrackedClass object based on its source class
        /// </summary>
        /// <param name="name">The name of the source class</param>
        /// <returns>The TrackedClass</returns>
        public TrackedClass this[string name]
        {
            get
            {
                return _trackedClasses.Find(tc => tc.QualifiedSourceClass.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            }
        }

        #endregion
    }
}
