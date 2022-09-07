using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using PGE.Common.ChangeDetectionAPI;
using PGE.Common.Delivery.Framework;
using PGE.Interfaces.Integration.Framework.Data;

namespace PGE.Interfaces.SAP.RowTransformers
{
    /// <summary>
    /// Process Structure equipment that determine AssetID, ActionType by themselves and have their own SAPType.
    /// </summary>
    public class SAPStructureRowTransformer : SAPEquipmentRowTransformer
    {
        /// <summary>
        /// Default constructor calls base constructor
        /// </summary>
        public SAPStructureRowTransformer()
            : base()
        {
        }
        /// <summary>
        /// Get the SAP record type for the row
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row</param>
        /// <param name="changeType">The type of edit to the row</param>
        /// <returns>SAPType.StructureEquipment</returns>
        public override SAPType GetSAPType(IRow sourceRow, IRow targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: StructureEquipment");

            return SAPType.StructureEquipment;
        }

        /// <summary>
        /// Get the SAP record type for the row
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row</param>
        /// <param name="changeType">The type of edit to the row</param>
        /// <returns>SAPType.StructureEquipment</returns>
        public override SAPType GetSAPType(IRow sourceRow, UpdateFeat targetRow, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: StructureEquipment");
            return SAPType.StructureEquipment;
        }

        /// <summary>
        /// Get the SAP record type for the row
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row</param>
        /// <param name="changeType">The type of edit to the row</param>
        /// <returns>SAPType.StructureEquipment</returns>
        public override SAPType GetSAPType(DeleteFeat sourceRow, ITable FCName, ChangeType changeType)
        {
            GISSAPIntegrator._logger.Info("OID :: " + sourceRow.OID + " SAP Type :: StructureEquipment");
            return SAPType.StructureEquipment;
        }
    }
}
