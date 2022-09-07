using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Systems.Data;
using ESRI.ArcGIS.Geodatabase;

namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// Core Processor Interface. Provides ability to process the Map Production Tool set
    /// </summary>
    public interface IMPProcessor
    {
        /// <summary>
        /// Database Connection to use to access Look Up Tables
        /// </summary>
        IDatabaseConnection DatabaseConnection
        {
            get;
        }
        /// <summary>
        /// Workspace to use to get the Map Polygon featureclass
        /// </summary>
        IWorkspace Workspace
        {
            get;
        }
        /// <summary>
        /// The SuccessMessage to be used if the Map Processing completes successfully.
        /// </summary>
        string SuccessMessage
        {
            get;
        }
        /// <summary>
        /// Geodatabase Field Lookup to use to get data from Geodatabase
        /// </summary>
        IMPGDBFieldLookUp GDBFieldLookUp
        {
            get;
        }
        /// <summary>
        /// Map Look up table to use for getting Lookup table information and data
        /// </summary>
        IMPMapLookUpTable MapLookUpTable
        {
            get;
        }
        /// <summary>
        /// Map between the GDBLookUp and MapLookUp fields.
        /// </summary>
        Dictionary<string, string> MapLUTGDBFieldLookUp
        {
            get;
        }
        /// <summary>
        /// The Database Manager object to use to fetch the Look up table information
        /// </summary>
        IMPDBDataManager DatabaseManager
        {
            get;
        }
        /// <summary>
        /// The Geodatabase Manager to use to fetch the Map Polygon feature information
        /// </summary>
        IMPGDBDataManager GeodatabaseManager
        {
            get;
        }
        /// <summary>
        /// Change Detector object to use to get the Changed Object keys
        /// </summary>
        IMPChangeDetector ChangeDetector
        {
            get;
        }
        /// <summary>
        /// Executes the core logic to process data
        /// </summary>
        /// <param name="exportType">ExportType to use <see cref="ExportType"/></param>
        /// <param name="totalNumberOfExporters">Total number of Exporters to set on hte ServiceToProcess<see cref="ExportType"/></param>
        /// <param name="keyData">Key Data to filter the data processed</param>
        /// <returns></returns>
        List<string> Process(ExportType exportType,int totalNumberOfExporters, params string[] keyData);

        /// <summary>
        /// Total number of Records processed by the Processor
        /// </summary>
        int RecordsProcessed
        {
            get;
        }

        /// <summary>
        /// Sets the Process integer on the Mapping table and makes it ready for being processed by the Exporters.
        /// All it does is take the number of processes and the number number of records to processed and divides the total number of records to be processed equally between the processes.
        /// </summary>
        /// <param name="noOfProcesses">Total number of Processes.</param>
        /// <returns>total number of rows that has been affected</returns>
        int SetProcessInfo(int noOfProcesses);
    }
}
