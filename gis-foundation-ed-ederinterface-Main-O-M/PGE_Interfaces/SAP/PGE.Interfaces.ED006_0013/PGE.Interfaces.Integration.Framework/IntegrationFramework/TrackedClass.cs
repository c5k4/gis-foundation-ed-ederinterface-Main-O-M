using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using PGE.Interfaces.Integration.Framework.Data;
using PGE.Interfaces.Integration.Framework.Utilities;
using System.Reflection;
using PGE.Interfaces.Integration.Framework.Exceptions;
using Diagnostics = PGE.Common.Delivery.Diagnostics;
//using PGE.Common.Delivery.Geodatabase.ChangeDetection;
namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Class used to store information about how to transform a certain GIS feature class, including row transformers and field transformers.
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute("TrackedClass", IsNullable = false)]
    public partial class TrackedClass
    {
        #region Private Members
        private SystemMapper _systemMapper;

        private List<MappedField> _fields;

        private Settings _settings;

        private string _outName;

        private string _transformerType;

        private string _sourceClass;

        private string _subtypes;

        private string _supportClasses;

        private TrackedClass _relatedClass;

        private IRowTransformer<IRow> _rowTransformer = null;

        private bool? _assetIDDefined = null;

        private ITable _targetTable = null;

        private ITable _sourceTable = null;

        private List<int> _processedRowOIDs;

        /// <summary>
        /// IRowData indexed on the AssestID for quick lookup by AssetID
        /// </summary>
        protected Dictionary<string, IRowData> _assetIDandRowData;

        private int _actionTypeField;

        private int _assetIDField;

        private static readonly Diagnostics.Log4NetLogger _logger = new Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");

        #endregion

        #region Public Serializable Members
        /// <summary>
        /// The sequence number of the field that contains the AssetID
        /// </summary>
        [XmlAttribute]
        public int AssetIDField
        {
            get { return _assetIDField; }
            set { _assetIDField = value; }
        }
        /// <summary>
        /// Not Used
        /// </summary>
        [XmlAttribute]
        public int ActionTypeField
        {
            get { return _actionTypeField; }
            set { _actionTypeField = value; }
        }
        /// <summary>
        /// List of mapped fields for the feature class.
        /// </summary>
        [XmlArrayItemAttribute("Field", typeof(MappedField), IsNullable = false)]
        public List<MappedField> Fields
        {
            get
            {
                return this._fields;
            }
            set
            {
                this._fields = new List<MappedField>(value);
            }
        }

        /// <summary>
        /// List of Settings of the TrackedClass
        /// </summary>
        [XmlElement("Settings")]
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

        ///// <summary>
        ///// The list of Settings for this TrackedClass
        ///// </summary>
        //[XmlArrayItem]
        //public List<Setting> Settings
        //{
        //    get;
        //    set;
        //}

        /// <summary>
        /// Name of the class in the external system
        /// </summary>
        [XmlAttributeAttribute()]
        public string OutName
        {
            get
            {
                return this._outName;
            }
            set
            {
                this._outName = value;
            }
        }
        
        /// <summary>
        /// The fully qualified name of the IRowTransformer to use for this class
        /// </summary>
        [XmlAttribute]
        public string TransformerType
        {
            get
            {
                return this._transformerType;
            }
            set
            {
                this._transformerType = value;
            }
        }

        /// <summary>
        /// The name of the GIS class this TrackedClass processes
        /// </summary>
        [XmlAttributeAttribute()]
        public string SourceClass
        {
            get
            {
                return this._sourceClass;
            }
            set
            {
                this._sourceClass = value;
            }
        }

        /// <summary>
        /// The subtypes operated on. If null, all subtypes are processed.
        /// </summary>
        [XmlAttributeAttribute()]
        public string Subtypes
        {
            get
            {
                return this._subtypes;
            }
            set
            {
                this._subtypes = value;
            }
        }


        //IVARA
        /// <summary>
        /// Filter by attributes
        /// </summary>
        [XmlAttribute]
        public string FilterCondition
        {
            get;
            set;
        }

        [XmlAttribute]
        public string SingleFieldWhereClauseCondition
        {
            get;
            set;
        }


        /// <summary>
        /// Not used. Only for debugging. 
        /// Now Used for Network features. IVARA needs field value based filter. - Subhankar
        /// </summary>
        [XmlIgnore]
        public string WhereClause
        {
            get
            {
                string where = string.Empty;
                List<string> subtypes = RowTransformer.Subtypes;
                if (subtypes.Count > 0)
                {
                    foreach (string subtype in subtypes)
                    {
                        where = where + "SubtypeCD = " + subtype + " OR ";
                    }

                    // remove the ending "spaceORspace"
                    if (where.Length > 4)
                    {
                        where.Substring(0, where.Length - 4);
                    }
                }

                // Check if there is any field value based filter condition. IVARA
                if (!string.IsNullOrEmpty(SingleFieldWhereClauseCondition))
                {
                    if(!string.IsNullOrEmpty(where))
                        where = where + " AND " + SingleFieldWhereClauseCondition;
                    else
                        where = SingleFieldWhereClauseCondition;
                }

                return where;
            }
        }

        /// Not used.<remarks/>
        [XmlAttributeAttribute()]
        public string SupportClasses
        {
            get
            {
                return this._supportClasses;
            }
            set
            {
                this._supportClasses = value;
            }
        }

        /// <summary>
        /// A related class where field values will also be pulled from
        /// </summary>
        [XmlElement("RelatedClass", typeof(TrackedClass))]
        public TrackedClass RelatedClass
        {
            get
            {
                return _relatedClass;
            }
            set
            {
                _relatedClass = value;
            }
        }
        /// <summary>
        /// The name of the relationship that connects the RelatedClass to this one
        /// </summary>
        [XmlAttribute]
        public string RelationshipName
        {
            get;
            set;
        }

        /// <summary>
        /// Fully qualified name of the IRowTransformer for the origin class of the Relationship
        /// </summary>
        [XmlAttribute]
        public string TransformerTypeForOriginClass
        {
            get;
            set;
        }

        #endregion

        #region Public Non-Serializable Members
        /// <summary>
        /// The IRowTransformer used to process each IRow of this TrackedClass
        /// </summary>
        [XmlIgnore]
        public IRowTransformer<IRow> RowTransformer
        {
            get
            {
                InitializeRowTransformer(); 
                return _rowTransformer;
            }
        }
        /// <summary>
        /// Checks if there is a field with the OutName AssetID
        /// </summary>
        [XmlIgnore]
        public bool AssetIDDefined
        {
            get
            {
                if (_assetIDDefined == null)
                {
                    _assetIDDefined = false;

                    foreach (MappedField field in Fields)
                    {
                        if (field.OutName == "AssetID")
                        {
                            _assetIDDefined = true;
                            break;
                        }
                    }
                }

                return (bool)_assetIDDefined;
            }
        }
        /// <summary>
        /// The object IDs of the class that have already been processed
        /// </summary>
        [XmlIgnore]
        public List<int> ProcessedRowOIDs
        {
            get { return _processedRowOIDs; }
            set { _processedRowOIDs = value; }
        }
        /// <summary>
        /// The table that contains the original rows
        /// </summary>
        [XmlIgnore]
        public ITable TargetTable
        {
            get { return _targetTable; }
            set { _targetTable = value; }
        }
        /// <summary>
        /// The table that contains the edited rows
        /// </summary>
        [XmlIgnore]
        public ITable SourceTable
        {
            get { return _sourceTable; }
            set { _sourceTable = value; }
        }
        #endregion
        /// <summary>
        /// The fully qualified class name i.e. DataOwner.ClassName
        /// </summary>
        [XmlIgnore]
        public string QualifiedSourceClass
        {
            get
            {
                return string.Concat(_systemMapper.Settings["DataOwner"], ".", SourceClass); 
            }
        }
        /// <summary>
        /// The fully qualified relationship name, i.e. DataOwner.RelationshipName
        /// </summary>
        [XmlIgnore]
        public string QualifiedRelationshipName
        {
            get
            {
                return string.Concat(_systemMapper.Settings["DataOwner"], ".", RelationshipName);
            }
        }

        #region Public Methods
        /// <summary>
        /// Initialize the FieldMappers and RelatedClass of this TrackedClass
        /// </summary>
        /// <param name="systemMapper">The parent SystemMapper of the TrackedClass</param>
        public void Initialize(SystemMapper systemMapper)
        {

           // _logger.Debug(string.Format("trackedclass. Initialize(SystemMapper systemMapper) outName={0}, TransformerType={1}", this.OutName, this.TransformerType));
                    
            this._systemMapper = systemMapper;
            InitializeRowTransformer();

            foreach (MappedField mappedField in Fields)
            {
                mappedField.FieldMapper.Initialize();
            }


            if (RelatedClass != null)
            {
                RelatedClass.Initialize(this._systemMapper);
            }
        }

        /// <summary>
        /// Get a setting from the TrackedClass
        /// </summary>
        /// <param name="settingName">The name of the Setting</param>
        /// <returns>The Value of the Setting</returns>
        //public Setting this[string settingName]
        //{
        //    get
        //    {
        //        return this.Settings.Find(setting => string.Compare(setting.Name, settingName, StringComparison.InvariantCultureIgnoreCase) >= 0 );
        //    }
        //}

        public Setting this[string settingName]
        {
            get
            {
                return this.Settings.SettingList.Find(setting => string.Compare(setting.Name, settingName, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Add a Setting to the TrackedClass
        /// </summary>
        /// <param name="setting">The Setting to add</param>
        public void AddSetting(Setting setting)
        {
            if (_settings == null)
            {
                _settings = new Settings();
                _settings.SettingList = new List<Setting>();
            }
            _settings.SettingList.Add(setting);
        }
        /// <summary>
        /// Add a MappedField to the TrackedClass
        /// </summary>
        /// <param name="field">The MappedField to add</param>
        public void AddFieldMapping(MappedField field)
        {
            if (_fields == null)
            {
                _fields = new List<MappedField>();
            }
            if (_fields.FindIndex(fld => string.Equals(fld.OutName, field.OutName, StringComparison.InvariantCultureIgnoreCase)) < 0)
                _fields.Add(field);
        }

        private void InitializeRowTransformer()
        {
            if (_rowTransformer == null)
            {
                Type transformerType = Type.GetType(TransformerType);
                if (transformerType == null)
                {
                    throw new InvalidConfigurationException(string.Format("Cannot create object from type {0}. Check the configuration.", TransformerType));
                }
                _rowTransformer = (IRowTransformer<IRow>)Activator.CreateInstance(transformerType);

                // pass system mapper and tracedclass
                _rowTransformer.Initialize(_systemMapper, this);
            }
        }
        #endregion
    }
}
