using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using PGE.Interface.Integration.DMS.Common;
using Miner.Geodatabase.Integration.Configuration;

namespace PGE.Interface.Integration.DMS.Tracers
{
    public class GenericExportScope : ElectricExportScope
    {
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        private string _featureclass;
        private string _where;
        private string _field;
        ScopeGroups _selectionTree;

        public GenericExportScope(NetworkAdapterSettingsElement settings)
            : base(settings)
        {

        }
        public override ScopeGroups SelectionTree
        {
            get
            {
                
                if (_selectionTree == null)
                {
                    CircuitTree result = new CircuitTree();
                    foreach (SourceGroup sg in GetSources())
                    {
                        result.AddChild(sg);
                    }
                    _selectionTree = result;
                }
                return _selectionTree;
            }
        }

        private void Configure()
        {
            _where = Configuration.getStringSetting("SubstationFilter", "SUBTYPECD=2");
            _featureclass = Configuration.getStringSetting("SubstationSource", "SUBElectricStitchPoint");
            _field = Configuration.getStringSetting("SubstationID", "SUBSTATIONID");
        }
        private List<SourceGroup> GetSources()
        {
            List<SourceGroup> output = new List<SourceGroup>();
            if (_where == null)
            {
                Configure();
            }
            IFeatureCursor cursor = null;
            IQueryFilter queryFilter = null;
            try
            {
                IFeatureClass fc = ((IFeatureWorkspace)GdbAccess.Workspace).OpenFeatureClass(_featureclass);
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = _where;
                cursor = fc.Search(queryFilter, true);
                IFeature feat = cursor.NextFeature();
                //Substations may have multiple sources, we only need to add one
                Dictionary<string, bool> IDs = new Dictionary<string, bool>();
                while (feat != null)
                {
                    int EID = ((ISimpleJunctionFeature)feat).EID;
                    string id = feat.Value[feat.Fields.FindField(_field)].ToString();
                    if (!IDs.ContainsKey(id))
                    {
                        SourceGroup sg = new SourceGroup(id, EID, id);
                        output.Add(sg);
                        IDs.Add(id, true);
                    }
                    feat = cursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error adding substation site.", ex);
            }
            finally
            {
                //release resources
                if (queryFilter != null)
                {
                    while (Marshal.ReleaseComObject(queryFilter) > 0) { }
                    queryFilter = null;
                }
                if (cursor != null)
                {
                    while (Marshal.ReleaseComObject(cursor) > 0) { }
                    cursor = null;
                }
            }
            return output;
        }
    }
}
