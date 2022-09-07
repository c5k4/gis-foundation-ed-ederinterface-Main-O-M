using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Framework;
using PGE.Interfaces.SAP.RowTransformers;

using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Process CircuitSource rows as part of StitchPoint asset.
    /// It cashes ObjectID of each processed CircuitSource row related to ElectricStitchPoint (1:1)
    /// so that CircuitCourse changes are only processed once.
    /// </summary>
    public class CircuitSourceRowTransformer : SAPRowTransformer
    {
        /// <summary>
        /// List of object IDs that have been processed.
        /// This should be reset to empty for every edit version so it should be assigned a new list 
        /// only once during initialization method of the main class GISSAPIntegrator.
        /// </summary>
        public static List<int> ProcessedOIDs = new List<int>();
        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData. Uses base class for most processing
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            List<IRowData> sapRowDataList = new List<IRowData>();
            int objID = -1;
            try
            {
                IRow activeRow = sourceRow;
                if (changeType == ChangeType.Delete)
                {
                    activeRow = targetRow;
                }

                objID = activeRow.OID;
                if (ProcessedOIDs.Contains(activeRow.OID) == false)
                {
                    GISSAPIntegrator._logger.Info("Processing OID :: "+ activeRow.OID);
                    sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    }

                    ProcessedOIDs.Add(activeRow.OID);
                }
                else
                {
                    GISSAPIntegrator._logger.Info("Already Processed OID :: " + activeRow.OID);
                    Settings settings = _trackedClass.Settings;
                    if (settings != null && settings["ProcessOnceAsRelatedRow"] == "False")
                    {
                        GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);
                        sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                            GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        }
                    }
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                throw new ApplicationException("Error " + objID + " Change was " + changeType.ToString(), ex);
            }

            return sapRowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData. Uses base class for most processing
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            List<IRowData> sapRowDataList = new List<IRowData>();
            int objID = -1;
            try
            {
                IRow activeRow = sourceRow;
                //if (changeType == ChangeType.Delete)
                //{
                //    activeRow = targetRow;
                //}

                objID = activeRow.OID;
                if (ProcessedOIDs.Contains(activeRow.OID) == false)
                {
                    GISSAPIntegrator._logger.Info("Processing OID :: " + activeRow.OID);
                    sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    }

                    ProcessedOIDs.Add(activeRow.OID);
                }
                else
                {
                    GISSAPIntegrator._logger.Info("Already Processed OID :: " + activeRow.OID);

                    Settings settings = _trackedClass.Settings;
                    if (settings != null && settings["ProcessOnceAsRelatedRow"] == "False")
                    {
                        GISSAPIntegrator._logger.Info("Re-Processing OID :: " + activeRow.OID);

                        sapRowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            GISSAPIntegrator._logger.Info("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                            GISSAPIntegrator._logger.Error("Error OID :: " + activeRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        }
                    }
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                throw new ApplicationException("Error " + objID + " Change was " + changeType.ToString(), ex);
            }

            return sapRowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData. Uses base class for most processing
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(DeleteFeat sourceRow, ChangeType changeType)
        {
            List<IRowData> sapRowDataList = new List<IRowData>();
            int objID = -1;
            try
            {
                //IRow activeRow = sourceRow;
                //if (changeType == ChangeType.Delete)
                //{
                //    activeRow = targetRow;
                //}

                objID = sourceRow.OID;
                if (ProcessedOIDs.Contains(sourceRow.OID) == false)
                {
                    GISSAPIntegrator._logger.Info("Processing OID :: " + sourceRow.OID);

                    sapRowDataList = base.ProcessRow(sourceRow, changeType);
                    if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                    {
                        GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                        GISSAPIntegrator._logger.Info("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        GISSAPIntegrator._logger.Error("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                    }

                    ProcessedOIDs.Add(sourceRow.OID);
                }
                else
                {
                    GISSAPIntegrator._logger.Info("Already Processed OID :: " + sourceRow.OID);

                    Settings settings = _trackedClass.Settings;
                    if (settings != null && settings["ProcessOnceAsRelatedRow"] == "False")
                    {
                        GISSAPIntegrator._logger.Info("Re-Processing OID :: " + sourceRow.OID);

                        sapRowDataList = base.ProcessRow(sourceRow, changeType);
                        if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                        {
                            GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                            GISSAPIntegrator._logger.Info("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                            GISSAPIntegrator._logger.Error("Error OID :: " + sourceRow.OID + " :: " + Convert.ToString(base._errorMessage));
                        }
                    }
                }
            }
            catch (Oracle.DataAccess.Client.OracleException oex) { GISSAPIntegrator._logger.Error(oex.GetType() + " || " + oex.Number + " || " + oex.Message + " || \n" + oex.StackTrace); throw oex; }
            catch (Exception ex)
            {
                GISSAPIntegrator._errorMessage.AppendLine("Error " + sourceRow.OID + " Change was " + changeType.ToString() + " Error :: " + ex.Message + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                throw new ApplicationException("Error " + objID + " Change was " + changeType.ToString(), ex);
            }

            return sapRowDataList;
        }
    }
}
