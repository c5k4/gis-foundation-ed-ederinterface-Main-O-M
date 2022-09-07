#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace Telvent.PGE.Test.Data
{
    public class TestTable: ITable,IObjectClass, IDataset, IWorkspace, IFeatureWorkspace
    {
        #region Internal Methods
        private Dictionary<string, int> _fieldIndexes;
        private Dictionary<string, TestRelationshipClass> _rels;
        private string _name;

        public Dictionary<string, int> FieldIndexes
        {
            get { return _fieldIndexes; }
            set { _fieldIndexes = value; }
        }
        public TestTable()
        {
            _fieldIndexes = new Dictionary<string, int>();
            _rels = new Dictionary<string, TestRelationshipClass>();
        }
        public TestTable(string[] fields, string name)
        {
            _fieldIndexes = new Dictionary<string, int>();
            _name = name;
            _rels = new Dictionary<string, TestRelationshipClass>();
            SetFields(fields);
        }
        public void AddRelatedRow(IRow row, string relationshipName)
        {
            if (_rels.ContainsKey(relationshipName))
            {
                TestRelationshipClass rel = _rels[relationshipName];
                rel.AddRow(row);
            }
            else
            {
                TestRelationshipClass newRel = new TestRelationshipClass();
                newRel.AddRow(row);
                _rels.Add(relationshipName, newRel);
            }
        }
        public void SetFields(string[] fields)
        {
            _fieldIndexes.Clear();
            for (int i = 0; i < fields.Length; i++)
            {
                _fieldIndexes.Add(fields[i], i);
            }
        }
        #endregion

        #region ITable Members
        public void AddField(IField Field)
        {
            throw new NotImplementedException();
        }

        public void AddIndex(IIndex Index)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.UID CLSID
        {
            get { throw new NotImplementedException(); }
        }

        public IRow CreateRow()
        {
            throw new NotImplementedException();
        }

        public IRowBuffer CreateRowBuffer()
        {
            throw new NotImplementedException();
        }

        public void DeleteField(IField Field)
        {
            throw new NotImplementedException();
        }

        public void DeleteIndex(IIndex Index)
        {
            throw new NotImplementedException();
        }

        public void DeleteSearchedRows(IQueryFilter QueryFilter)
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.UID EXTCLSID
        {
            get { throw new NotImplementedException(); }
        }

        public object Extension
        {
            get { throw new NotImplementedException(); }
        }

        public ESRI.ArcGIS.esriSystem.IPropertySet ExtensionProperties
        {
            get { throw new NotImplementedException(); }
        }

        public IFields Fields
        {
            get { throw new NotImplementedException(); }
        }

        public int FindField(string Name)
        {
            if (_fieldIndexes.ContainsKey(Name))
            {
                return _fieldIndexes[Name];
            }
            return -1;
        }

        public IRow GetRow(int OID)
        {
            throw new NotImplementedException();
        }

        public ICursor GetRows(object oids, bool Recycling)
        {
            throw new NotImplementedException();
        }

        public bool HasOID
        {
            get { throw new NotImplementedException(); }
        }

        public IIndexes Indexes
        {
            get { throw new NotImplementedException(); }
        }

        public ICursor Insert(bool useBuffering)
        {
            throw new NotImplementedException();
        }

        public string OIDFieldName
        {
            get { return "OID"; }
        }

        public int RowCount(IQueryFilter QueryFilter)
        {
            throw new NotImplementedException();
        }

        public ICursor Search(IQueryFilter QueryFilter, bool Recycling)
        {
            throw new NotImplementedException();
        }

        public ISelectionSet Select(IQueryFilter QueryFilter, esriSelectionType selType, esriSelectionOption selOption, IWorkspace selectionContainer)
        {
            throw new NotImplementedException();
        }

        public ICursor Update(IQueryFilter QueryFilter, bool Recycling)
        {
            throw new NotImplementedException();
        }

        public void UpdateSearchedRows(IQueryFilter QueryFilter, IRowBuffer buffer)
        {
            throw new NotImplementedException();
        }
        #endregion
        
        #region IDataset Members
        public string BrowseName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public bool CanCopy()
        {
            throw new NotImplementedException();
        }

        public bool CanDelete()
        {
            throw new NotImplementedException();
        }

        public bool CanRename()
        {
            throw new NotImplementedException();
        }

        public string Category
        {
            get { throw new NotImplementedException(); }
        }

        public IDataset Copy(string copyName, IWorkspace copyWorkspace)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.esriSystem.IName FullName
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return _name; }
        }

        public ESRI.ArcGIS.esriSystem.IPropertySet PropertySet
        {
            get { throw new NotImplementedException(); }
        }

        public void Rename(string Name)
        {
            throw new NotImplementedException();
        }

        public IEnumDataset Subsets
        {
            get { throw new NotImplementedException(); }
        }

        public esriDatasetType Type
        {
            get { throw new NotImplementedException(); }
        }

        public IWorkspace Workspace
        {
            get { return this; }
        }
        #endregion

        #region IWorkspace Members

        public ESRI.ArcGIS.esriSystem.IPropertySet ConnectionProperties
        {
            get { throw new NotImplementedException(); }
        }

        public void ExecuteSQL(string sqlStmt)
        {
            throw new NotImplementedException();
        }

        public bool Exists()
        {
            throw new NotImplementedException();
        }

        public bool IsDirectory()
        {
            throw new NotImplementedException();
        }

        public string PathName
        {
            get { throw new NotImplementedException(); }
        }

        esriWorkspaceType IWorkspace.Type
        {
            get { throw new NotImplementedException(); }
        }

        public IWorkspaceFactory WorkspaceFactory
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumDatasetName get_DatasetNames(esriDatasetType DatasetType)
        {
            throw new NotImplementedException();
        }

        public IEnumDataset get_Datasets(esriDatasetType DatasetType)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IFeatureWorkspace Members

        public IFeatureClass CreateFeatureClass(string Name, IFields Fields, ESRI.ArcGIS.esriSystem.UID CLSID, ESRI.ArcGIS.esriSystem.UID EXTCLSID, esriFeatureType FeatureType, string ShapeFieldName, string ConfigKeyword)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataset CreateFeatureDataset(string Name, ESRI.ArcGIS.Geometry.ISpatialReference SpatialReference)
        {
            throw new NotImplementedException();
        }

        public IQueryDef CreateQueryDef()
        {
            throw new NotImplementedException();
        }

        public IRelationshipClass CreateRelationshipClass(string relClassName, IObjectClass OriginClass, IObjectClass DestinationClass, string ForwardLabel, string BackwardLabel, esriRelCardinality Cardinality, esriRelNotification Notification, bool IsComposite, bool IsAttributed, IFields relAttrFields, string OriginPrimaryKey, string destPrimaryKey, string OriginForeignKey, string destForeignKey)
        {
            throw new NotImplementedException();
        }

        public ITable CreateTable(string Name, IFields Fields, ESRI.ArcGIS.esriSystem.UID CLSID, ESRI.ArcGIS.esriSystem.UID EXTCLSID, string ConfigKeyword)
        {
            throw new NotImplementedException();
        }

        public IFeatureClass OpenFeatureClass(string Name)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataset OpenFeatureDataset(string Name)
        {
            throw new NotImplementedException();
        }

        public IFeatureDataset OpenFeatureQuery(string QueryName, IQueryDef QueryDef)
        {
            throw new NotImplementedException();
        }

        public IRelationshipClass OpenRelationshipClass(string Name)
        {
            if (_rels.ContainsKey(Name))
            {
                return _rels[Name];
            }
            return null;
        }

        public ITable OpenRelationshipQuery(IRelationshipClass relClass, bool joinForward, IQueryFilter SrcQueryFilter, ISelectionSet SrcSelectionSet, string TargetColumns, bool DoNotPushJoinToDB)
        {
            throw new NotImplementedException();
        }

        public ITable OpenTable(string Name)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IObjectClass Members

        private string _aliasName;
        public void setAliasName(string name)
        {
            _aliasName = name;
        }
        public string AliasName
        {
            get { return _aliasName; }
        }

        private int _objectClassID;
        public void setObjectClassID(int id)
        {
            _objectClassID = id;
        }
        public int ObjectClassID
        {
            get { return _objectClassID; }
        }

        public IEnumRelationshipClass get_RelationshipClasses(esriRelRole Role)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
