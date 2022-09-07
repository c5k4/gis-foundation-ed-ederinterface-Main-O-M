using System;
using System.Collections.Generic;
using System.Reflection;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using PGE.Common.Delivery.Diagnostics;
using System.Runtime.InteropServices;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Common.ChangesManagerShared.Streetlights
{
    public class StreetlightInv
    {
        #region Constants

        public const string INV_FIELD_GISID = "GIS_ID";
        public const string INV_FIELD_UNIQUESPID = "UNIQUE_SP_ID";
        public const string INV_FIELD_BADGENO = "BADGE_NUMBER";
        public const string INV_FIELD_OFFICE = "OFFICE";
        public const string INV_FIELD_FIXTURECODE = "FIXTURE_CODE";

        private static string[] INV_FIELDS = { "ACCOUNT_NUMBER", "ALTNM", "BADGE_NUMBER", "BALLAST_CH_DT", "CCB_OVERWRITE_FLAG",
                                                "CHANGE_OF_PARTY_DATE", "CITYNAME", "DESCRIPTIVE_ADDRESS", "DIFFADDR", "DIFFBADGE",
                                                "DIFFIX", "DIFFIT", "DIFFMAP", "DIFFRS", "FAPPLIANCE1", "FAPPLIANCE2", "FAPPLIANCE3", 
                                                "FAPPLIANCE4", "FAPPLIANCE5", "FAR1", "FAR2", "FAR3", "FAR4", "FAR5", "FAROTHER", 
                                                "FIXTURE_CODE", "FIXTURE_MANUFACTURER", "FMETRICON", "GEMS_DISTR_MAPNUM", "GIS_ID", 
                                                "HALFHRADJ_TYPE", "HIST_GEMS_AKA", "HIST_GEMS_BADGENUM", "HIST_GEMS_MAP_NUM", 
                                                "HIST_GEMS_MAP_RATE", "INSTALL_DATE", "INVENTORIED_BY", "INVENTORY_DATE", "ITEM_TYPE_CODE", 
                                                "LAMP_CH_DT", "LAST_MODIFY_DATE", "LITESIZE_TYPE", "LITETYPE_TYPE", "LUMN_CH_DT", "MAIL_ADDR1", 
                                                "MAIL_ADDR2", "MAIL_CITY", "MAIL_STATE", "MAIL_ZIP", "MAINTNOTE", "MAP_NUMBER", "MAP_NUMBER_NEW", 
                                                "METER", "NEAREST_ST", "NEW_GRID_MAPNUM", "OFFICE", "OPERATING_SCHEDULE", "PAINTPOLE", "PCELL", 
                                                "PCELL_CH_DT", "POLE_CH_DT", "POLE_LENGTH", "POLE_PT_DT", "POLE_TYPE", "POLE_USE", "PREM_ID", 
                                                "RATE_SCHEDULE", "RECEIVE_DATE", "REMOVAL_DATE", "SA_ID", "SP_ID", "SERVICE", "SP_ITEM_HIST", 
                                                "STATUS", "STATUS_FLAG", "STREETNM", "STRT_CH_DT", "SUSPENSION", "TOT_CODE", "TOT_TERR_DESC", 
                                                "TRANSACTION_", "UNIQUE_SP_ID", "USERID" };

        //private static string STATUS_FIELD = "STATUS";
        //private static string STATUS_FIELD_RETIRED = "R";

        #endregion

        #region Member vars

        // For debug logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        // For working with streetlight_inv feature class
        private IWorkspace _workspace = null;
        private IFeatureClass _streetlightInvFc = null;

        #endregion

        #region Constructor

        public StreetlightInv(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        #endregion

        #region Properties

        public IFeatureClass StreetlightInvFc
        {
            get
            {
                if (_streetlightInvFc == null)
                {
                    _streetlightInvFc = (_workspace as IFeatureWorkspace).OpenFeatureClass("EDGIS.STREETLIGHT_INV");
                }
                return _streetlightInvFc;
            }
        }

        #endregion

        #region Public methods

        public IFeatureCursor Fetch(int startObjectId = 1, string whereClause = "")
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor slInvCur = null;

            try
            {
                IQueryFilter filter = new QueryFilterClass();
                filter.WhereClause = "OBJECTID>=" + startObjectId.ToString();
                if (whereClause != string.Empty)
                {
                    filter.WhereClause += " AND " + whereClause;
                }
                IQueryFilterDefinition qfDef = filter as IQueryFilterDefinition;
                qfDef.PostfixClause = "ORDER BY OBJECTID";
                slInvCur = StreetlightInvFc.Search(filter, true);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return slInvCur;
        }

        public void Insert(IGeometry shape, IList<string> values)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Create a new feature
                ((IWorkspaceEdit)_workspace).StartEditOperation();
                IFeature streetlightInvRow = StreetlightInvFc.CreateFeature();
                streetlightInvRow.Shape = shape;

                // Set its fields
                int fieldIndex = 0;
                foreach (string value in values)
                {
                    if (value != "")
                    {
                        int gisIdFieldIndex = StreetlightInvFc.FindField(INV_FIELDS[fieldIndex]);
                        if (gisIdFieldIndex > -1)
                        {
                            streetlightInvRow.set_Value(gisIdFieldIndex, value);
                        }
                        else
                        {
                            _logger.Warn("Failed to find STREETLIGHT_INV field: " + INV_FIELDS[fieldIndex]);
                        }
                    }
                    fieldIndex++;
                }

                // Save
                streetlightInvRow.Store();
                ((IWorkspaceEdit)_workspace).StopEditOperation();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(IRow row)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor curs = null;

            try
            {
                // Fetch the existing feature                
                int indexGisField = (row as IFeature).Class.Fields.FindField("STREETLIGHTSGISID");
                IQueryFilter qfInv = new QueryFilterClass();
                qfInv.WhereClause = "GIS_ID='" + row.get_Value(indexGisField) + "'";
                curs = StreetlightInvFc.Update(qfInv, true);
                IFeature streetlightInvRow = curs.NextFeature();

                //// Update its status to be deleted
                //int gisIdFieldIndex = StreetlightInvFc.FindField(STATUS_FIELD);
                //if (gisIdFieldIndex > -1)
                //{
                //    streetlightInvRow.set_Value(gisIdFieldIndex, STATUS_FIELD_RETIRED);
                //}

                // Delete the row if found...
                if (streetlightInvRow != null)
                {
                    ((IWorkspaceEdit)_workspace).StartEditOperation();
                    streetlightInvRow.Delete();
                    ((IWorkspaceEdit)_workspace).StopEditOperation();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (curs != null) Marshal.FinalReleaseComObject(curs);
            }
        }

        public void CDDelete(DeleteFeat row)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            IFeatureCursor curs = null;

            try
            {
                // Fetch the existing feature                
                
                string GisField = string.Empty;
                if (row.fields_Old.ContainsKey("STREETLIGHTSGISID".ToUpper()))
                {
                    GisField = row.fields_Old["STREETLIGHTSGISID".ToUpper()];
                }
                else
                {
                    _logger.Error("STREETLIGHTSGISID not found for OID " + row.OID);
                    return;
                }

                IQueryFilter qfInv = new QueryFilterClass();
                qfInv.WhereClause = "GIS_ID='" + GisField + "'";
                curs = StreetlightInvFc.Update(qfInv, true);
                IFeature streetlightInvRow = curs.NextFeature();

                //// Update its status to be deleted
                //int gisIdFieldIndex = StreetlightInvFc.FindField(STATUS_FIELD);
                //if (gisIdFieldIndex > -1)
                //{
                //    streetlightInvRow.set_Value(gisIdFieldIndex, STATUS_FIELD_RETIRED);
                //}

                // Delete the row if found...
                if (streetlightInvRow != null)
                {
                    ((IWorkspaceEdit)_workspace).StartEditOperation();
                    streetlightInvRow.Delete();
                    ((IWorkspaceEdit)_workspace).StopEditOperation();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (curs != null) Marshal.FinalReleaseComObject(curs);
            }
        }

        #endregion
    }
}
