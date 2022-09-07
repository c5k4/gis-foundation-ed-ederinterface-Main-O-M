using System;
using System.Collections.Generic;
using System.Reflection;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.ChangesManagerShared.Streetlights
{
    public class StreetlightFieldPts
    {
        #region Constants

        private static string[] FIELDPTS_FIELDS = { "OFFICE", "PERSON_NAME", "ACCOUNT_NUMBER", "BADGE_NUMBER", "FIXTURE_CODE", "STATUS",
                                                    "STATUS_FLAG", "RECEIVE_DATE", "INSTALL_DATE", "REMOVAL_DATE", 
                                                    "CHANGE_OF_PARTY_DATE", "DESCRIPTIVE_ADDRESS", "MAP_NUMBER", "RATE_SCHEDULE", "ITEM_TYPE_CODE", 
                                                    "OPERATING_SCHEDULE", "SERVICE", "FIXTURE_MANUFACTURER", "POLE_TYPE", "POLE_LENGTH", 
                                                    "SUSPENSION", "POLE_USE", "SP_ID", "SA_ID", "PREM_ID", "TOT_CODE", "TOT_TERR_DESC", 
                                                    "INVENTORY_DATE", "INVENTORIED_BY", "SP_ITEM_HIST", "UNIQUE_SP_ID", "GIS_ID", "TRANSACTION", 
                                                    "FMETRICON", "FAR1", "FAR2", "FAR3", "FAR4", "FAR5", "FAROTHER", 
                                                    "MAINTNOTE", "METER", "DIFFBADGE", "DIFFIX", "DIFFADDR", "DIFFMAP", "DIFFRS", "DIFFIT", 
                                                    "PAINTPOLE", "FAPPLIANCE1", "FAPPLIANCE2", "FAPPLIANCE3", "FAPPLIANCE4" };

        #endregion

        #region Member vars

        // For debug logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        // For interacting with FieldPts feature class
        private IWorkspace _workspace = null;
        private IFeatureClass _fieldPtsFc = null;

        #endregion

        #region Constructor

        public StreetlightFieldPts(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        #endregion

        #region Properties

        private IFeatureClass FieldPtsFc
        {
            get
            {
                if (_fieldPtsFc == null)
                {
                    _fieldPtsFc = (_workspace as IFeatureWorkspace).OpenFeatureClass("PGEDATA.FIELDPTS");
                }
                return _fieldPtsFc;
            }
        }

        #endregion

        #region Public methods

        public void Insert(IGeometry shape, IList<string> values)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Create a new feature
                ((IWorkspaceEdit)_workspace).StartEditOperation();
                IFeature streetlightInvRow = FieldPtsFc.CreateFeature();
                streetlightInvRow.Shape = shape;

                // Set its fields
                int fieldIndex = 0;
                foreach (string value in values)
                {
                    if (value != "")
                    {
                        int gisIdFieldIndex = FieldPtsFc.FindField(FIELDPTS_FIELDS[fieldIndex]);
                        if (gisIdFieldIndex > -1)
                        {
                            if (FIELDPTS_FIELDS[fieldIndex] == "TRANSACTION")
                            {
                                streetlightInvRow.set_Value(gisIdFieldIndex, "8");
                            }
                            else
                            {
                                streetlightInvRow.set_Value(gisIdFieldIndex, value);
                            }
                        }
                        else
                        {
                            _logger.Warn("Failed to find FIELDPTS field: " + FIELDPTS_FIELDS[fieldIndex]);
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

        public void Delete(IFeature feature)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Fetch the existing feature
                ((IWorkspaceEdit)_workspace).StartEditOperation();
                IRow streetlightInvRow = FieldPtsFc.GetFeature(feature.OID);
                streetlightInvRow.Delete();
                ((IWorkspaceEdit)_workspace).StopEditOperation();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
