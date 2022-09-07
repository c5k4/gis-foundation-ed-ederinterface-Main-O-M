#pragma warning disable 1591
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace Telvent.PGE.Test.Data
{
    public class TestWorkspace:IFeatureWorkspace,IWorkspace
    {
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

        public esriWorkspaceType Type
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
            throw new NotImplementedException();
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
    }
}
