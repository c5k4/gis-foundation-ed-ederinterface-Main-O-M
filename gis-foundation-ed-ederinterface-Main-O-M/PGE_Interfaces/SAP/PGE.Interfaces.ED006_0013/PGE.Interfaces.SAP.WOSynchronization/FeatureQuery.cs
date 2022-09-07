using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using log4net;
using ESRI.ArcGIS.DataSourcesGDB;


namespace Telvent.PGE.SAP.WOSynchronization
{
    public class FeatureQuery
    {
        private ITable _table = null;
        private string _tableName = System.Configuration.ConfigurationManager.AppSettings["WIPCloudName"];
        private log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #region Constructor

        public FeatureQuery(IWorkspace workspace)
        {
            _table = GetTable(workspace, _tableName);
        }

        #endregion

        #region Public Methods

        public List<IRow> SearchFeature(string whereClause)
        {
            if (whereClause == null) whereClause = string.Empty;
            List<IRow> wipFeatures = new List<IRow>();
            ICursor cursor = _table.Search(null, false);
            IRow wipFeature = null;
            while ((wipFeature = cursor.NextRow()) != null)
            {
                wipFeatures.Add(wipFeature);
            }
            return wipFeatures;
        }

        #endregion Public Methods

        #region Private Methods

        protected ITable GetTable(IWorkspace wSpace, string tableName)
        {
            if (wSpace == null) return null;
            if (string.IsNullOrEmpty(tableName)) return null;
            IWorkspace2 wSpace2 = wSpace as IWorkspace2;
            ITable table = null;
            IFeatureWorkspace featureWSpace = wSpace as IFeatureWorkspace;

            if (wSpace2.get_NameExists(esriDatasetType.esriDTTable, tableName))
            {
                table = featureWSpace.OpenTable(tableName);
            }
            else if (wSpace2.get_NameExists(esriDatasetType.esriDTFeatureClass, tableName))
            {
                table = featureWSpace.OpenFeatureClass(tableName) as ITable;
            }
            if (table == null)
            {
                _logger.Debug(tableName + " does not exist in " + wSpace.PathName);
            }
            return table;
        }

        #endregion Private Methods

    }
}
