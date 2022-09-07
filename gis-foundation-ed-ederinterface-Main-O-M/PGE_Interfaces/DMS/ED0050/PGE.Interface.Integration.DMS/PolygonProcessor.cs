using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using System.Data;
using PGE.Interface.Integration.DMS.Common;
using Miner.Geodatabase.Integration;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;

namespace PGE.Interface.Integration.DMS
{
    public class PolygonProcessor
    {
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        private IWorkspace _ws;
        private DataSet _data;
        private DataTable _table;
        private string _featureclass;
        private string _field;
        private Dictionary<string, bool> _added;

        public DataSet Data
        {
            get { return _data; }
            set
            {
                _data = value;
                if (_data.Tables.Contains("DMSSTAGING.SITE"))
                {
                    _table = _data.Tables["DMSSTAGING.SITE"];
                }
            }
        } 

        public PolygonProcessor(IWorkspace ws)
        {
            _ws = ws;
            _featureclass = Configuration.getStringSetting("SubstationPolygon", "SUBStationBoundary");
            _field = Configuration.getStringSetting("SubstationField", "SUBSTATIONID");
            _added = new Dictionary<string, bool>();
        }

        public void AddJunction(JunctionFeatureInfo junct, string id)
        {
            if (!_added.ContainsKey(id))
            {
                _added.Add(id, true);
                DataRow row = _table.NewRow();
                //may want to get this from the IFeature instead?
                //row["SINAME"] = Utilities.GetDBFieldValue(junct, "SUBSTATIONNAME");
                double subid = Utilities.getID(junct);
                row["SIFPOS"] = subid;
                row["NO_KEY"] = subid;
                row["GSITE_ANG"] = 0;
                IFeatureCursor cursor = null;
                IQueryFilter queryFilter = null;
                try
                {
                    IFeatureClass fc = ((IFeatureWorkspace)_ws).OpenFeatureClass(_featureclass);
                    int fieldIndex = fc.Fields.FindField("SUBSTATIONNAME");
                    queryFilter = new QueryFilterClass();
                    queryFilter.WhereClause = _field + "=" + id + "";
                    cursor = fc.Search(queryFilter, true);
                    IFeature feat = cursor.NextFeature();
                    if (feat != null)
                    {
                        IEnvelope env = feat.Shape.Envelope;
                        row["GXSITE"] = Utilities.ConvertXY(env.UpperLeft.X);
                        row["GYSITE"] = Utilities.ConvertXY(env.UpperLeft.Y);
                        row["GSITEH"] = Utilities.ConvertXY(env.Height);
                        row["GSITEW"] = Utilities.ConvertXY(env.Width);

                        row["SINAME"] = feat.get_Value(fieldIndex);
                        row["HYPERLINK"] = feat.get_Value(fc.Fields.FindField("HYPERLINK"));
                        row["COMMENTS"] = feat.get_Value(fc.Fields.FindField("COMMENTS"));
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
                _table.Rows.Add(row);
            }
        }
    }
}
