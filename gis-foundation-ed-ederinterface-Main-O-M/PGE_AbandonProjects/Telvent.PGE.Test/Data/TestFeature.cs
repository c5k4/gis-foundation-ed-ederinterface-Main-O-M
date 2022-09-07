#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Telvent.PGE.Test.Data
{
    public class TestFeature : IFeature, IRow, IObject, IFields, ISubtypes, IRowSubtypes
    {
        #region Internal Methods
        private List<object> _data;
        private IGeometry _geo;
        private ITable _table;
        private int _subtype;
        private Dictionary<string, object> _modelnames;

        public Dictionary<string, object> Modelnames
        {
            get { return _modelnames; }
            set { _modelnames = value; }
        }

        public TestFeature()
        {
            _data = new List<object>();
            Initialize();
        }
        public TestFeature(List<object> data)
        {
            Initialize();
            SetData(data);
        }
        public TestFeature(string[] fields, object[] values, string tableName)
        {
            Initialize();
            CreateTestRow(fields, values, tableName);
        }

        private void Initialize()
        {
            _modelnames = new Dictionary<string, object>();
        }
        public void SetData(List<object> data)
        {
            _data = data;
        }
        public void SetXY(double x, double y)
        {
            _geo = new TestGeometry(x, y);
        }
        public void AddRelatedRow(IRow row, string relationshipName)
        {
            if (_table != null)
            {
                ((TestTable)_table).AddRelatedRow(row, relationshipName);
            }
        }
        public void SetSubtype(int subtype)
        {
            _subtype = subtype;
        }
        public void CreateTestRow(string[] fields, object[] values, string tableName)
        {
            _table = new TestTable(fields, tableName);
            LoadData(values);
        }
        public void LoadData(object[] data)
        {
            _data = new List<object>();
            foreach (object value in data)
            {
                _data.Add(value);
            }
        }
        #endregion

        #region IFeature Members
        public IObjectClass Class
        {
            get { return (IObjectClass)_table; }
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public ESRI.ArcGIS.Geometry.IEnvelope Extent
        {
            get
            {
                if (_geo != null)
                {
                    return _geo.Envelope;
                }
                return null;
            }
        }

        public esriFeatureType FeatureType
        {
            get { throw new NotImplementedException(); }
        }

        public IFields Fields
        {
            get { return this; }
        }

        public bool HasOID
        {
            get { throw new NotImplementedException(); }
        }

        public int OID
        {
            get
            {
                return Convert.ToInt32( get_Value( FindField("OID") ) );
            }
        }

        public ESRI.ArcGIS.Geometry.IGeometry Shape
        {
            get
            {
                return _geo;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public ESRI.ArcGIS.Geometry.IGeometry ShapeCopy
        {
            get { throw new NotImplementedException(); }
        }

        public void Store()
        {
            throw new NotImplementedException();
        }

        
        public ITable Table
        {
            get { return _table; }
        }

        public object get_Value(int Index)
        {
            return _data[Index];
        }

        public void set_Value(int Index, object Value)
        {
            _data[Index] = Value;
        }
        #endregion

        #region IFields Members
        public int FieldCount
        {
            get { throw new NotImplementedException(); }
        }

        public int FindField(string Name)
        {
            if (_table != null)
            {
                return _table.FindField(Name);
            }
            return -1;
        }

        public int FindFieldByAliasName(string Name)
        {
            throw new NotImplementedException();
        }

        public IField get_Field(int Index)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ISubtypes Members
        public void AddSubtype(int SubtypeCode, string SubtypeName)
        {
            throw new NotImplementedException();
        }

        public int DefaultSubtypeCode
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

        public void DeleteSubtype(int SubtypeCode)
        {
            throw new NotImplementedException();
        }

        public bool HasSubtype
        {
            get { throw new NotImplementedException(); }
        }

        public int SubtypeFieldIndex
        {
            get { throw new NotImplementedException(); }
        }

        public string SubtypeFieldName
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

        public IEnumSubtype Subtypes
        {
            get { throw new NotImplementedException(); }
        }

        public object get_DefaultValue(int SubtypeCode, string FieldName)
        {
            throw new NotImplementedException();
        }

        public IDomain get_Domain(int SubtypeCode, string FieldName)
        {
            throw new NotImplementedException();
        }

        public string get_SubtypeName(int SubtypeCode)
        {
            throw new NotImplementedException();
        }

        public void set_DefaultValue(int SubtypeCode, string FieldName, object Value)
        {
            throw new NotImplementedException();
        }

        public void set_Domain(int SubtypeCode, string FieldName, IDomain Domain)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region IRowSubtypes Members

        public void InitDefaultValues()
        {
            throw new NotImplementedException();
        }

        public int SubtypeCode
        {
            get
            {
                return _subtype;
            }
            set
            {
                _subtype = value;
            }
        }

        #endregion
    }
}
