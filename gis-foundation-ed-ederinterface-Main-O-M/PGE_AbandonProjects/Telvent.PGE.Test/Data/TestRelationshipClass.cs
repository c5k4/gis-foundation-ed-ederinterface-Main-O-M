#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

namespace Telvent.PGE.Test.Data
{
    public class TestRelationshipClass : IRelationshipClass, ISet, IRelationshipClass2
    {
        private List<IObject> _objects;
        private int _index;

        public TestRelationshipClass()
        {
            _objects = new List<IObject>();
            _index = 0;
        }
        public void AddRow(IRow row)
        {
            _objects.Add((IObject)row);
        }
        public void AddRelationshipRule(IRule Rule)
        {
            throw new NotImplementedException();
        }

        public string BackwardPathLabel
        {
            get { throw new NotImplementedException(); }
        }

        public esriRelCardinality Cardinality
        {
            get { throw new NotImplementedException(); }
        }

        public IRelationship CreateRelationship(IObject OriginObject, IObject DestinationObject)
        {
            throw new NotImplementedException();
        }

        public void DeleteRelationship(IObject OriginObject, IObject DestinationObject)
        {
            throw new NotImplementedException();
        }

        public void DeleteRelationshipRule(IRule Rule)
        {
            throw new NotImplementedException();
        }

        public void DeleteRelationshipsForObject(IObject anObject)
        {
            throw new NotImplementedException();
        }

        public void DeleteRelationshipsForObjectSet(ESRI.ArcGIS.esriSystem.ISet anObjectSet)
        {
            throw new NotImplementedException();
        }

        public IObjectClass DestinationClass
        {
            get { throw new NotImplementedException(); }
        }

        public string DestinationForeignKey
        {
            get { throw new NotImplementedException(); }
        }

        public string DestinationPrimaryKey
        {
            get { throw new NotImplementedException(); }
        }

        public IFeatureDataset FeatureDataset
        {
            get { throw new NotImplementedException(); }
        }

        public string ForwardPathLabel
        {
            get { throw new NotImplementedException(); }
        }

        public IRelClassEnumRowPairs GetObjectsMatchingObjectSet(ESRI.ArcGIS.esriSystem.ISet srcObjectSet)
        {
            throw new NotImplementedException();
        }

        public ISet GetObjectsRelatedToObject(IObject anObject)
        {
            return this;
        }

        public ESRI.ArcGIS.esriSystem.ISet GetObjectsRelatedToObjectSet(ESRI.ArcGIS.esriSystem.ISet anObjectSet)
        {
            throw new NotImplementedException();
        }

        public IRelationship GetRelationship(IObject OriginObject, IObject DestinationObject)
        {
            throw new NotImplementedException();
        }

        public IEnumRelationship GetRelationshipsForObject(IObject anObject)
        {
            throw new NotImplementedException();
        }

        public IEnumRelationship GetRelationshipsForObjectSet(ESRI.ArcGIS.esriSystem.ISet anObjectSet)
        {
            throw new NotImplementedException();
        }

        public bool IsAttributed
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsComposite
        {
            get { throw new NotImplementedException(); }
        }

        public esriRelNotification Notification
        {
            get { throw new NotImplementedException(); }
        }

        public IObjectClass OriginClass
        {
            get { throw new NotImplementedException(); }
        }

        public string OriginForeignKey
        {
            get { throw new NotImplementedException(); }
        }

        public string OriginPrimaryKey
        {
            get { throw new NotImplementedException(); }
        }

        public int RelationshipClassID
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumRule RelationshipRules
        {
            get { throw new NotImplementedException(); }
        }

        #region ISet Members

        public void Add(object unk)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _objects.Count; }
        }

        public bool Find(object unk)
        {
            throw new NotImplementedException();
        }

        public object Next()
        {
            if (_index < _objects.Count)
            {
                object output = _objects[_index];
                _index++;
                return output;
            }
            return null;
        }

        public void Remove(object unk)
        {
            throw new NotImplementedException();
        }

        public void RemoveAll()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            _index = 0;
        }

        #endregion

        #region IRelationshipClass2 Members


        public IRelClassEnumRowPairs GetObjectsMatchingObjectArray(IArray srcObjectArray, IQueryFilter queryFilterAppliedToMatchingObjects, bool returnAllObjectMatches)
        {
            throw new NotImplementedException();
        }

        public IRelClassEnumRowPairs GetObjectsMatchingObjectSetEx(ISet srcObjectSet, IQueryFilter queryFilterAppliedToMatchingObjects, bool returnAllObjectMatches)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
