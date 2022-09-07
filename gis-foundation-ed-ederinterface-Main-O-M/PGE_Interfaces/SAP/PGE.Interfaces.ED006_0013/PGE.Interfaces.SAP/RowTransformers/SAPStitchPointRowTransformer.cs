using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangeDetectionAPI;
using PGE.Common.Delivery.Framework;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Process Functional Location asset classes: StitchPoint
    /// </summary>
    public class SAPStitchPointRowTransformer : SAPEquipmentRowTransformer
    {
        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            List<IRowData> rowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
            if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
            {
                GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + Convert.ToString(base._errorMessage));
                GISSAPIntegrator._logger.Error("OID :: " + sourceRow.OID + Convert.ToString(base._errorMessage));
            }

            foreach (IRowData rowData in rowDataList)
            {
                rowData.FieldValues.Remove(_trackedClass.AssetIDField);
            }

            return rowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            List<IRowData> rowDataList = base.ProcessRow(sourceRow, targetRow, changeType);
            if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
            {
                GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + Convert.ToString(base._errorMessage));
                GISSAPIntegrator._logger.Error("OID :: " + sourceRow.OID + Convert.ToString(base._errorMessage));    
            }
            foreach (IRowData rowData in rowDataList)
            {
                rowData.FieldValues.Remove(_trackedClass.AssetIDField);
            }

            return rowDataList;
        }

        /// <summary>
        /// Process a GIS IRow field by field and output the transformed data as a list of IRowData
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>A list of IRowData</returns>
        public override List<IRowData> ProcessRow(DeleteFeat sourceRow,ChangeType changeType)
        {
            List<IRowData> rowDataList = base.ProcessRow(sourceRow, changeType);
            if (!string.IsNullOrWhiteSpace(Convert.ToString(base._errorMessage)))
                {
                GISSAPIntegrator._errorMessage.AppendLine(Convert.ToString(base._errorMessage) + " at ( " + System.Reflection.MethodBase.GetCurrentMethod().Name + " )");
                GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + Convert.ToString(base._errorMessage));
                GISSAPIntegrator._logger.Error("OID :: " + sourceRow.OID + Convert.ToString(base._errorMessage));
                }
            foreach (IRowData rowData in rowDataList)
            {
                rowData.FieldValues.Remove(_trackedClass.AssetIDField);
            }

            return rowDataList;
        }

        /// <summary>
        /// Find the SAP Type the GIS row is equivalent to
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP Type</returns>
        public override SAPType GetSAPType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: FunctionalLocation");
            return SAPType.FunctionalLocation;
        }

        /// <summary>
        /// Find the SAP Type the GIS row is equivalent to
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP Type</returns>
        public override SAPType GetSAPType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: FunctionalLocation");
            return SAPType.FunctionalLocation;
        }

        /// <summary>
        /// Find the SAP Type the GIS row is equivalent to
        /// </summary>
        /// <param name="sourceRow">The edited IRow</param>
        /// <param name="targetRow">The IRow before it was edited</param>
        /// <param name="changeType">The type of edit i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP Type</returns>
        public override SAPType GetSAPType(DeleteFeat sourceRow, ITable FCName, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: FunctionalLocation");
            return SAPType.FunctionalLocation;
        }
    }
}
