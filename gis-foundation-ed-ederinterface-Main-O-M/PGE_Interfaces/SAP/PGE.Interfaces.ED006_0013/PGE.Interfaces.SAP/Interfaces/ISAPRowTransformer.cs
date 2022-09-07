using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Data;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.SAP
{
    /// <summary>
    /// Defines methods to help in converting GIS data to data to send to SAP
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISAPRowTransformer<T>: IRowTransformer<T>
    {
        /// <summary>
        /// Get the row's parent ID. For a device this will be the structure it is related to. For a structure it will be the ID of the structure
        /// it is replacing, if it is a replace operation.
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row before it was edited</param>
        /// <returns>The parent ID of the row</returns>
        string GetParentID(T sourceRow,T targetRow);
        /// <summary>
        /// Get the row's SAP type
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row before it was edited</param>
        /// <param name="changeType">The type of edit, i.e. Update, Insert, or Delete</param>
        /// <returns>The type of SAP record the row represents</returns>
        SAPType GetSAPType(T sourceRow, T targetRow, ChangeType changeType);
        /// <summary>
        /// Get the SAP action type
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row before it was edited</param>
        /// <param name="changeType">The type of change in the GIS, i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP action type</returns>
        ActionType GetActionType(T sourceRow, T targetRow, ChangeType changeType);


        /// <summary>
        /// Get the row's parent ID. For a device this will be the structure it is related to. For a structure it will be the ID of the structure
        /// it is replacing, if it is a replace operation.
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row before it was edited</param>
        /// <returns>The parent ID of the row</returns>
        string GetParentID(DeleteFeat sourceRow);
        /// <summary>
        /// Get the row's SAP type
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row before it was edited</param>
        /// <param name="changeType">The type of edit, i.e. Update, Insert, or Delete</param>
        /// <returns>The type of SAP record the row represents</returns>
        SAPType GetSAPType(DeleteFeat sourceRow, ITable FCName, ChangeType changeType);
        /// <summary>
        /// Get the SAP action type
        /// </summary>
        /// <param name="sourceRow">The edited row</param>
        /// <param name="targetRow">The original row before it was edited</param>
        /// <param name="changeType">The type of change in the GIS, i.e. Insert, Update, or Delete</param>
        /// <returns>The SAP action type</returns>
        ActionType GetActionType(DeleteFeat sourceRow, ChangeType changeType);
    }
    /// <summary>
    /// The GIS construction status representing the actual device's status in the field
    /// </summary>
    public enum ConstructionStatus
    {
        /// <summary>
        /// The device is going to be installed in the field
        /// </summary>
        ProposedInstall = 0,
        /// <summary>
        /// The device is installed in the field, but a change has been proposed
        /// </summary>
        ProposedChange = 1,
        /// <summary>
        /// The device is installed in the field, but a removal of the device has been proposed
        /// </summary>
        ProposedRemove = 2,
        /// <summary>
        /// The device is installed in the field, but it has been proposed to deactivate it
        /// </summary>
        ProposedDeactivated = 3,
        /// <summary>
        /// The device is active in the field
        /// </summary>
        InService = 5,
        /// <summary>
        /// The device is in the field, but has been deactivated
        /// </summary>
        Deactivated = 10,
        /// <summary>
        /// The device has been removed from the field
        /// </summary>
        Removed = 20,
        /// <summary>
        /// The device is in the field, but is idle
        /// </summary>
        Idle = 30
    }
    /// <summary>
    /// Action type to send to SAP
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// The record is to be inserted
        /// </summary>
        Insert = 'I',
        /// <summary>
        /// The record exists in SAP and needs updated
        /// </summary>
        Update = 'U',
        /// <summary>
        /// The record exists in SAP and needs deactivated
        /// </summary>
        Delete = 'D',

        /// <summary>
        /// The records in SAP need to have User Status set to Idle
        /// </summary>
        Idle = 'L',

        /// <summary>
        /// Pre-map, don't need to go to SAP yet.
        /// </summary>
        PreMap = 'P',
        /// <summary>
        /// Something is wrong with the record's Status and the session should not be processed by SAP
        /// </summary>
        Invalid = 'X',
        /// <summary>
        /// The record doesn't designate an asset record's ActionType
        /// </summary>
        NotApplicable = 'N'
    }
    /// <summary>
    /// The type of SAP record
    /// </summary>
    public enum SAPType
    {
        /// <summary>
        /// Not a SAP record
        /// </summary>
        NotApplicable = 0,
        /// <summary>
        /// A record that represents a functional location
        /// </summary>
        FunctionalLocation = 1,
        /// <summary>
        /// A record that represents a structure
        /// </summary>
        StructureEquipment = 2,
        /// <summary>
        /// A record that represents equipment on a structure
        /// </summary>
        StructureSubEquipment = 3,
        /// <summary>
        /// A record that represents a device
        /// </summary>
        DeviceEquipment = 4,
        /// <summary>
        /// A record that represents equipment attached to a device
        /// </summary>
        DeviceSubEquipment = 5
    }
}
