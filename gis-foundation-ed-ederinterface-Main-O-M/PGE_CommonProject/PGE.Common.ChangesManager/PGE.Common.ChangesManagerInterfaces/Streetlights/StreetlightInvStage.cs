using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.ChangesManagerShared.Streetlights
{
    public class StreetlightInvStage
    {
        #region Member vars

        // For logging
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        // For interacting with Streetlight Inv feature class
        private IWorkspace _workspace = null;
        private IFeatureClass _streetlightInvStageFc = null;

        #endregion

        #region Constructor

        public StreetlightInvStage(IWorkspace workspace)
        {
            _workspace = workspace;
        }

        #endregion

        #region Properties

        private IFeatureClass StreetlightInvStageFc
        {
            get
            {
                if (_streetlightInvStageFc == null)
                {
                    _streetlightInvStageFc = (_workspace as IFeatureWorkspace).OpenFeatureClass("EDGIS.STREETLIGHT_INV_STG");
                }
                return _streetlightInvStageFc;
            }
        }

        #endregion

        #region Public methods

        public IFeatureCursor FetchAdds(string whereClause)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Default return value
            IFeatureCursor streetlightInvStageCur = null;

            try
            {
                // Fetch features with a transaction of 1 (ie: Adds)
                IQueryFilter qfAdds = new QueryFilterClass();
                qfAdds.WhereClause = "TRANSACTION_='1'";
                if (whereClause != string.Empty)
                {
                    qfAdds.WhereClause += " AND " + whereClause;
                }
                IQueryFilterDefinition qfDef = qfAdds as IQueryFilterDefinition;
                qfDef.PostfixClause = "ORDER BY OBJECTID";
                streetlightInvStageCur = StreetlightInvStageFc.Update(qfAdds, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Return the result
            return streetlightInvStageCur;
        }

        public IFeatureCursor FetchDeletes()
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            // Default return value
            IFeatureCursor streetlightInvStageCur = null;

            try
            {
                // Fetch features with a transaction of 2 (ie: Deletes)
                IQueryFilter qfDeletes = new QueryFilterClass();
                qfDeletes.WhereClause = "TRANSACTION_='2'";
                streetlightInvStageCur = StreetlightInvStageFc.Update(qfDeletes, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // Return the result
            return streetlightInvStageCur;
        }

        public void Insert(IFeature sourceFeature)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Get transaction type
                string transaction = "1";
                int transFieldIndex = sourceFeature.Fields.FindField("TRANSACTION_");
                object trans = sourceFeature.get_Value(transFieldIndex);
                if (trans != null)
                {
                    transaction = trans.ToString();
                }

                // If it wasn't a delete
                if (transaction != "2")
                {
                    // Create new row in the staging table
                    _logger.Info("No match found and transaction = 1; Cloning data to staging for OID: " + sourceFeature.OID.ToString());
                    ((IWorkspaceEdit)_workspace).StartEditOperation();
                    IFeature newSl = StreetlightInvStageFc.CreateFeature();
                    newSl.Shape = sourceFeature.Shape;

                    // Copy all its attrs
                    for (int fieldIndex = 0; fieldIndex < sourceFeature.Fields.FieldCount; fieldIndex++)
                    {
                        IField field = sourceFeature.Class.Fields.get_Field(fieldIndex);
                        if (field.Type != esriFieldType.esriFieldTypeOID)
                        {
                            int targetFieldIndex = StreetlightInvStageFc.Fields.FindField(field.Name);
                            if (targetFieldIndex > -1)
                            {
                                try
                                {
                                    if (fieldIndex == transFieldIndex)
                                    {
                                        newSl.set_Value(targetFieldIndex, "1");
                                    }
                                    else
                                    {
                                        newSl.set_Value(targetFieldIndex, sourceFeature.get_Value(fieldIndex));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    // Ignore this error since its just bad data
                                    if (ex.Message != "Not a legal OleAut date.")
                                    {
                                        throw ex;
                                    }
                                    else
                                    {
                                        _logger.Warn("Found bad date in database for field: " + field.Name); 
                                    }
                                }
                            }
                        }
                    }

                    // Save                
                    newSl.Store();
                    ((IWorkspaceEdit)_workspace).StopEditOperation();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete(IFeature feature)
        {
            // Log entry
            _logger.Debug(MethodBase.GetCurrentMethod().Name);

            try
            {
                // Delete the existing feature - apparently this ignores transactions and is an instant operation
                _logger.Info("Deleting Inventory record with OID: " + feature.OID);
                ((IWorkspaceEdit)_workspace).StartEditOperation();
                IRow streetlightInvRow = StreetlightInvStageFc.GetFeature(feature.OID);
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
