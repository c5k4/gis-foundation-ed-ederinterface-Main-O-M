using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using PGE.Common.Delivery.Systems.Data;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Interface to handle the Map Look Up table in teh database
    /// </summary>
    public interface IMPMapLookUpTable
    {
        /// <summary>
        /// Name of the Look Up Table
        /// </summary>
        string TableName
        {
            get;
        }
        /// <summary>
        /// Name of the XMin field
        /// </summary>
        string XMin
        {
            get;
        }
        /// <summary>
        /// Name of the YMin field
        /// </summary>
        string YMin
        {
            get;
        }
        /// <summary>
        /// Name of the XMax field
        /// </summary>
        string XMax
        {
            get;
        }
        /// <summary>
        /// Name of the YMax field
        /// </summary>
        string YMax
        {
            get;
        }
        /// <summary>
        /// Name of the Map Number field
        /// </summary>
        string MapId
        {
            get;
        }
        ///// <summary>
        ///// Name of the Map Scale field
        ///// </summary>
        //string MapScale
        //{
        //    get;
        //}
        ///// <summary>
        ///// Name of the PolygonCoordinate field
        ///// </summary>
        //string PolygonCoordinates
        //{
        //    get;
        //}
        /// <summary>
        /// Name of the field that holds the Exportstate
        /// </summary>
        string MapExportState
        {
            get;
        }
        /// <summary>
        /// Name of the field that holds the LastModifiedDate
        /// </summary>
        string LastModifiedDate
        {
            get;
        }
        /// <summary>
        /// Service to Process
        /// </summary>
        string ProcessingService
        {
            get;
        }
        /// <summary>
        /// Circuit Id
        /// </summary>
        string CircuitId
        {
            get;
        }
        /// <summary>
        /// Feeder Circuit Id
        /// </summary>
        string FeederCircuitId
        {
            get;
        }
        /// <summary>
        /// Circuit Name
        /// </summary>
        string CircuitName
        {
            get;
        }
        /// <summary>
        /// ChildCircuit Name
        /// </summary>
        string ChildCircuitName
        {
            get;
        }
        /// <summary>
        /// PSPA Name
        /// </summary>
        string PSPSName
        {
            get;
        }
        /// <summary>
        /// Total Mileage
        /// </summary>
        string TotalSegmentLength
        {
            get;
        }
        /// <summary>
        /// Layout Type
        /// </summary>
        string LayoutType
        {
            get;
        }
        /// <summary>
        /// Names of otherfields in Look Up Table
        /// </summary>
        List<string> OtherFields
        {
            get;
        }
        /// <summary>
        /// Names of field that should be used to uniquely identify a record in the database excluding the MapNumber field.
        /// The fields added here is a subset of Otherfields.
        /// </summary>
        List<string> KeyFields
        {
            get;
        }
        /// <summary>
        /// All column names of a record in the database.
        /// </summary>
        List<string> AllColumns
        {
            get;
        }

        /// <summary>
        /// Given a datatable will verify if the datatable has every field that is required.
        /// </summary>
        /// <param name="lookUpTable"></param>
        /// <returns></returns>
        bool VerifyLookUpTable(DataTable lookUpTable);
        /// <summary>
        /// For the instance will return an Insert command to be used for updating the table in database
        /// </summary>
        /// <param name="parameterPrefix">The Parameter prefix used by the given database. Use the IDatabaseConnection.GetParameterPrefix to get this info</param>
        /// <param name="databaseConnection">Database Connection to which the Command should be tied to</param>
        /// <returns></returns>
        DbCommand InsertCommand(string parameterPrefix, IDatabaseConnection databaseConnection);
        /// <summary>
        /// For the instance will return an Update command to be used for updating the table in database
        /// </summary>
        /// <param name="parameterPrefix">The Parameter prefix used by the given database. Use the IDatabaseConnection.GetParameterPrefix to get this info</param>
        /// <param name="databaseConnection">Database Connection to which the Command should be tied to</param>
        /// <returns></returns>
        DbCommand UpdateCommand(string parameterPrefix, IDatabaseConnection databaseConnection);
        /// <summary>
        /// For the instance will return an Delete command to be used for updating the table in database
        /// </summary>
        /// <param name="parameterPrefix">The Parameter prefix used by the given database. Use the IDatabaseConnection.GetParameterPrefix to get this info</param>
        /// <param name="databaseConnection">Database Connection to which the Command should be tied to</param>
        /// <returns></returns>
        DbCommand DeleteCommand(string parameterPrefix, IDatabaseConnection databaseConnection);
    }
}
