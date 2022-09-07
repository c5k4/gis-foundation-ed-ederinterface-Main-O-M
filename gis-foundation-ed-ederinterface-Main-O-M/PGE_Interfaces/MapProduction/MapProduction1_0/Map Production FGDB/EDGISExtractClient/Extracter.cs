using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

using GeodatabaseUtils;

//using ESRI.ArcGIS.Framework;
//using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE_DBPasswordManagement;

namespace EDGISExtract
{

    public enum ExportMode
    {
        ExportModeElectric = 0,
        ExportModeWIP = 1,
        ExportModeLandbase = 2
    }

    public enum ExitCodes
    {
        Success,
        Failure,
        ChildFailure,
        InvalidArguments,
        LicenseFailure
    };

    public enum ExtractDatasetType
    {
        etTable = 0,
        etFeatureclass = 1,
        etAnnoFeatureclass = 2,
        etJoinFeatureclass = 3,
        etRelationshipclass = 4
    }

    public enum ExtractType
    {
        ExtractTypeLandbase = 0,
        ExtractTypeEDGIS = 1
    }

    class Extracter
    {
        private string _logFilename = "";
        private string _installDirectory = "";
        private int _processId;
        private FileInfo _logfileInfo = null;
        private Hashtable _hshWorkspacePaths = null;
        private Hashtable _hshWorkspaces = null;
        private List<ExtractDataset> _extractDatasets;
        private string _workingGDBFolderPath = "";
        private string _workingGDBName = "";
        private int _maxTries;
        ITransformation _transformation = null;
        ISpatialReference _toSpatialRef = null;
        private const int SERVICE_TO_PROCESS_OUTSIDE_RANGE = 99;

        public Extracter(int processId, ExtractType pExtractType)
        {
            //Read the config settings 
            _processId = processId;
            LoadConfigurationSettings(pExtractType);

            //Initialize the logfile 
            InitializeLogfile();
        }

        /// <summary>
        /// Assigns the ServiceToProcess in the EDGIS.PGE_MAPNUMBERCOORDLUT table using 
        /// percentage 10% 250, 40% 500, 50% 100 
        /// </summary>
        /// <param name="office"></param>
        /// <param name="startProcessId"></param>
        /// <param name="endProcessId"></param>
        //public void AssignServiceToProcess( 
        //    string office, 
        //    int startProcessId, 
        //    int endProcessId)
        //{
        //    OleDbConnection conn = null;
        //    DateTime pNow = DateTime.Now;

        //    try
        //    {
        //        WriteToLogfile("Entering AssignServiceToProcess");
        //        WriteToLogfile("    office: " + office);

        //        conn = new OleDbConnection(_oracleConnectionString);
        //        conn.Open();
        //        WriteToLogfile("Connected via connectionstring");

        //        OleDbDataAdapter da = new OleDbDataAdapter();
        //        da.SelectCommand = new OleDbCommand( 
        //            "Select * from EDGIS.PGE_MAPNUMBERCOORDLUT where " + 
        //            "EXPORTSTATE IN(" + _clipExportStateList + ")" + " And " + 
        //            "MAPOFFICE = '" + office + "'", conn);
        //        WriteToLogfile("Created command");
        //        OleDbCommandBuilder cb = new OleDbCommandBuilder(da);
        //        DataSet ds = new DataSet();
        //        da.Fill(ds, "PGE_MAPNUMBERCOORDLUT");
        //        WriteToLogfile("Performed fill");
        //        DataTable dt = ds.Tables["PGE_MAPNUMBERCOORDLUT"];
        //        int serviceToProcess = startProcessId;

        //        //50% of the processors -> 100 scale 
        //        //40% of the processors -> 500 scale 
        //        //10% of the processors -> 250 scale (but at least one) 
        //        //other scales - ignore the map (give it service to process = 0) 
        //        int noOfProcesses = endProcessId - startProcessId + 1;

        //        //Figure out the number of 250's make sure at least 1 thread 
        //        int num250 = Convert.ToInt32(Math.Round((noOfProcesses * 0.10), 0));
        //        if (num250 == 0)
        //            num250 = 1;

        //        //Figure out the number of 500's make sure at least 1 process
        //        int num500 = Convert.ToInt32(Math.Round((noOfProcesses * 0.40), 0));
        //        if (num500 == 0)
        //            num500 = 1;

        //        //Figure out the number of 100's make sure at least 1 process 
        //        int num100 = noOfProcesses - (num250 + num500);
        //        if (num100 == 0)
        //            num100 = 1;

        //        int start100 = startProcessId;
        //        int curr100 = start100;                
        //        int end100 = startProcessId + (num100 - 1);

        //        int start250 = end100 + 1;
        //        int curr250 = start250;
        //        int end250 = start250 + (num250 - 1);

        //        int start500 = end250 + 1;
        //        int curr500 = start500;
        //        int end500 = endProcessId; 

        //        int totalMaps = dt.Rows.Count;                
        //        int counter = 0;

        //        if (totalMaps > 0)
        //        {
        //            foreach (DataRow row in dt.Rows)
        //            {
        //                //Get the map scale 
        //                counter++; 
        //                int mapScale = Convert.ToInt32(row["mapscale"]);

        //                switch (mapScale)
        //                {
        //                    case 100:
        //                        row["servicetoprocess"] = curr100;
        //                        if (curr100 == end100)
        //                            curr100 = start100;
        //                        else
        //                            curr100++;
        //                        break;
        //                    case 250:
        //                        row["servicetoprocess"] = curr250;
        //                        if (curr250 == end250)
        //                            curr250 = start250;
        //                        else
        //                            curr250++;
        //                        break;
        //                    case 500:
        //                        row["servicetoprocess"] = curr500;
        //                        if (curr500 == end500)
        //                            curr500 = start500;
        //                        else
        //                            curr500++;
        //                        break;
        //                    default:
        //                        //We can ignore these ones according to George 2/24/2014 
        //                        break;
        //                }
        //            }                   
        //        }

        //        WriteToLogfile("Updating dataset");
        //        da.Update(ds, "PGE_MAPNUMBERCOORDLUT");
        //        WriteToLogfile("Updated dataset");
        //        conn.Close();
        //        WriteToLogfile("Closing connection");
        //        WriteToLogfile("Leaving AssignServiceToProcess");
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("Entering AssignServiceToProcess Error Handler: " +
        //            ex.Message);
        //    }
        //}

        ///// <summary>
        ///// Updates the status field in the PGE_MAPGEOLUT_MP1 / 2 
        ///// </summary>
        ///// <param name="pFWS"></param>
        ///// <param name="status"></param>
        ///// <param name="mapGeo"></param>
        ///// <param name="machineName"></param>
        ///// <param name="extractStartDate"></param>
        ///// <param name="extractEndDate"></param>
        ///// <param name="mappingEndDate"></param>
        //public void UpdateMapGeoStatus(
        //    IFeatureWorkspace pFWS,
        //    string status,
        //    string mapGeo,
        //    string machineName,
        //    DateTime extractStartDate,
        //    DateTime extractEndDate,
        //    DateTime mappingEndDate)
        //{
        //    ITransactions pTransactions = null;
        //    //string sql = "";
        //    //bool successFlag = true; 

        //    try
        //    {
        //        WriteToLogfile("Entering UpdateMapOfficeStatus");
        //        WriteToLogfile("    mapofficecode: " + mapGeo);
        //        WriteToLogfile("    status: " + status);

        //        //Open the dirty feeders table default version 
        //        ITable pMapGeoLUT = pFWS.OpenTable(_mapGeoTable);
        //        int statusFldIdx = pMapGeoLUT.Fields.FindField("STATUS");
        //        int machineFldIdx = pMapGeoLUT.Fields.FindField("MACHINE");
        //        int extractStartDateFldIdx = pMapGeoLUT.Fields.FindField("EXTRACT_START");
        //        int extractEndDateFldIdx = pMapGeoLUT.Fields.FindField("EXTRACT_END");
        //        int mappingEndDateFldIdx = pMapGeoLUT.Fields.FindField("MAPPING_END");
        //        int failureCountFldIdx = pMapGeoLUT.Fields.FindField("FAILURECOUNT");
        //        bool updateFailureCount = false; 

        //        //Search for all records that have that mapGeo 
        //        IQueryFilter pQF = new QueryFilterClass();
        //        pQF.WhereClause = "MAPGEO" + " = " + "'" + mapGeo + "'";
        //        if (pMapGeoLUT.RowCount(pQF) == 0)
        //        {
        //            WriteToLogfile("Unable to update Map Office status because MAPOFFICE: " + mapGeo +
        //                " was not found!");
        //        }

        //        //Get the initial failurecount 
        //        ICursor pCursor = pMapGeoLUT.Search(pQF, false);
        //        IRow pRow = pCursor.NextRow();
        //        int failureCount = Convert.ToInt32(pRow.get_Value(failureCountFldIdx));
        //        Marshal.FinalReleaseComObject(pCursor);

        //        //Increment the failurecount if necessary 
        //        if (status == ExtractConstants.EXTRACT_STATUS_EXTRACT_ERROR ||
        //            status == ExtractConstants.EXTRACT_STATUS_MAPPING_ERROR)
        //        {
        //            updateFailureCount = true; 
        //            failureCount++; 
        //        }

        //        IWorkspaceEdit2 pWSE2 = (IWorkspaceEdit2)pFWS;
        //        if (pWSE2.IsInEditOperation)
        //        {
        //            throw new Exception("Cannot use ITransactions during an edit session");
        //        }
        //        pTransactions = (ITransactions)pWSE2;
        //        pTransactions.StartTransaction();
        //        pCursor = pMapGeoLUT.Search(pQF, false);
        //        pRow = pCursor.NextRow();

        //        if (pRow != null)
        //        {
        //            pRow.set_Value(statusFldIdx, status);

        //            if (machineName != string.Empty)
        //                pRow.set_Value(machineFldIdx, machineName);
        //            if (extractStartDate.Year != 1900)
        //                pRow.set_Value(extractStartDateFldIdx, extractStartDate);
        //            if (extractEndDate.Year != 1900)
        //                pRow.set_Value(extractEndDateFldIdx, extractEndDate);
        //            if (mappingEndDate.Year != 1900)
        //                pRow.set_Value(mappingEndDateFldIdx, mappingEndDate);

        //            if (updateFailureCount)
        //                pRow.set_Value(failureCountFldIdx, failureCount);

        //            pRow.Store();
        //        }
        //        pTransactions.CommitTransaction();

        //        //Ensure that if we are setting the status to extracting that no 
        //        //other machine is already extracting the mapGeo - because we want to 
        //        //avoid the situation where 2 processes are trying to copy the 
        //        //file GDB to the central GDB path 

        //        //string dateUpdateSQL = "";
        //        //if (extractStartDate.Year != 1900)
        //        //    dateUpdateSQL = "EXTRACT_START" + " = " + "'" + extractStartDate + "'";
        //        //if (extractEndDate.Year != 1900)
        //        //{
        //        //    if (dateUpdateSQL == "")
        //        //        dateUpdateSQL = "EXTRACT_END" + " = " + "'" + extractEndDate + "'"; 
        //        //    else
        //        //        dateUpdateSQL += ", " + "EXTRACT_END" + " = " + "'" + extractEndDate + "'";  

        //        //}
        //        //if (mappingEndDate.Year != 1900)
        //        //{
        //        //    if (dateUpdateSQL == "")
        //        //        dateUpdateSQL = "MAPPING_END" + " = " + "'" + mappingEndDate + "'";
        //        //    else
        //        //        dateUpdateSQL += ", " + "MAPPING_END" + " = " + "'" + mappingEndDate + "'";
        //        //}

        //        //switch (status) 
        //        //{
        //        //    case ExtractConstants.EXTRACT_STATUS_EXTRACTING:
        //        //        sql = "UPDATE " + _mapGeoTable + " SET " +
        //        //            "STATUS" + " = " + "'" + status + "'" + ", " +
        //        //            "MACHINE" + " = " + "'" + machineName + "'" + ", " +
        //        //            dateUpdateSQL + 
        //        //            " Where " +
        //        //            "MAPGEO" + " = " + "'" + mapGeo + "'" + " And " +
        //        //            "MACHINE" + " IS NULL"; 
        //        //        break; 
        //        //    case ExtractConstants.EXTRACT_STATUS_EXTRACT_ERROR:
        //        //        sql = "UPDATE " + _mapGeoTable + " SET " +
        //        //            "STATUS" + " = " + "'" + status + "'" + ", " +
        //        //            "MACHINE" + " = " + "'" + machineName + "'" + ", " +
        //        //            "FAILURECOUNT" + " = " + failureCount.ToString() + 
        //        //            " Where " +
        //        //            "MAPGEO" + " = " + "'" + mapGeo + "'";
        //        //        break; 
        //        //    case ExtractConstants.EXTRACT_STATUS_MAPPING:
        //        //        sql = "UPDATE " + _mapGeoTable + " SET " +
        //        //            "STATUS" + " = " + "'" + status + "'" + ", " + 
        //        //            "MACHINE" + " = " + "'" + machineName + "'" + ", " +
        //        //            dateUpdateSQL + 
        //        //            " Where " +
        //        //            "MAPGEO" + " = " + "'" + mapGeo + "'";  
        //        //        break;
        //        //    case ExtractConstants.EXTRACT_STATUS_MAPPING_ERROR:
        //        //        sql = "UPDATE " + _mapGeoTable + " SET " +
        //        //            "STATUS" + " = " + "'" + status + "'" + ", " +
        //        //            "MACHINE" + " = " + "'" + machineName + "'" + ", " +
        //        //            "FAILURECOUNT" + " = " + failureCount.ToString() + 
        //        //            " Where " +
        //        //            "MAPGEO" + " = " + "'" + mapGeo + "'";
        //        //        break;
        //        //    case ExtractConstants.EXTRACT_STATUS_FINISHED:
        //        //        sql = "UPDATE " + _mapGeoTable + " SET " +
        //        //            "STATUS" + " = " + "'" + status + "'" + ", " + 
        //        //            "MACHINE" + " = " + "'" + machineName + "'" + ", " +
        //        //            dateUpdateSQL + 
        //        //            " Where " +
        //        //            "MAPGEO" + " = " + "'" + mapGeo + "'";
        //        //        break;
        //        //}

        //        ////Execute the SQL 
        //        //IWorkspace pWS = (IWorkspace)pFWS;
        //        //pWS.ExecuteSQL(sql);

        //        //if (status == ExtractConstants.EXTRACT_STATUS_EXTRACTING)
        //        //{
        //        //    //Check to see if the machine is set to the 
        //        //    //current machine name otherwise another machine 
        //        //    //must have started extracting this mapGeo 
        //        //    string actualMachineName = "";
        //        //    string actualStatus = ""; 

        //        //    pCursor = pMapGeoLUT.Search(pQF, false);
        //        //    pRow = pCursor.NextRow();
        //        //    if (pRow != null)
        //        //    {
        //        //        if (pRow.get_Value(machineFldIdx) != DBNull.Value)
        //        //            actualMachineName = pRow.get_Value(machineFldIdx).ToString();
        //        //        if (pRow.get_Value(statusFldIdx) != DBNull.Value)
        //        //            actualStatus = pRow.get_Value(statusFldIdx).ToString();
        //        //    }
        //        //    Marshal.FinalReleaseComObject(pCursor); 

        //        //    if ((actualMachineName != machineName) || 
        //        //        (actualStatus != status))
        //        //    {
        //        //        WriteToLogfile("mapGeo: " + mapGeo + 
        //        //            " must be already extracting on another machine - returning false"); 
        //        //        successFlag = false; 
        //        //    }
        //        //}                

        //        WriteToLogfile("Leaving UpdateMapOfficeStatus");
        //    }
        //    catch (Exception ex)
        //    {
        //        //pTransactions.AbortTransaction();
        //        WriteToLogfile("Entering UpdateMapOfficeStatus Error Handler: " +
        //            ex.Message);
        //    }
        //}

        /// <summary>
        /// Clears any GDB folder that might be left in the central 
        /// directory from previous runs of map production 
        /// </summary>
        //public void ClearWorkingDirectory()
        //{
        //    try
        //    {
        //        DirectoryInfo workingDir = new DirectoryInfo(_workingGDBFolderPath);
        //        if (workingDir.Exists)
        //        {
        //            //Delete all folders in this central folder  
        //            DirectoryInfo[] dirs = workingDir.GetDirectories();
        //            for (int i = 0; i < dirs.Length; i++)
        //            {
        //                if (!dirs[i].Name.StartsWith("MASTER_")) 
        //                    dirs[i].Delete(true);
        //            }

        //            //Delete any files in this central folder
        //            FileInfo[] files = workingDir.GetFiles();
        //            for (int i = 0; i < files.Length; i++)
        //            {
        //                if (!files[i].Name.StartsWith("MASTER_"))
        //                    files[i].Delete();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("Entering ClearWorkingDirectory Error Handler: " +
        //            ex.Message);
        //        throw new Exception("Error clearing out the working directory!");
        //    }
        //}

        //public void ResetServiceToProcess(
        //    int startProcessId, 
        //    int endProcessId, 
        //    int serviceToProcessOutsideRange)
        //{
        //    try
        //    {
        //        WriteToLogfile("Entering ResetServiceToProcess");
        //        IWorkspace pWS = (IWorkspace)_hshWorkspaces["electric"];
        //        string sql = "Update EDGIS.PGE_MAPNUMBERCOORDLUT SET SERVICETOPROCESS = " + 
        //            serviceToProcessOutsideRange.ToString() + " Where " + 
        //            "(" + 
        //            "SERVICETOPROCESS <= " + endProcessId + " And " +
        //            "SERVICETOPROCESS >= " + startProcessId + 
        //            ")" + " And " + 
        //            "EXPORTSTATE IN (" + _clipExportStateList + ")";
        //        WriteToLogfile("Running the following SQL to reset ServiceToProcess: " + sql); 
        //        pWS.ExecuteSQL(sql); 
        //        WriteToLogfile("Leaving ResetServiceToProcess");
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("Entering ResetServiceToProcess Error Handler: " +
        //            ex.Message);
        //        throw new Exception("Error reseting the ServiceToProcess in table EDGIS.PGE_MAPNUMBERCOORDLUT"); 
        //    }
        //}

        ///// <summary>
        ///// Finds the next MapOffice to be extracted and mapped by looking for 
        ///// the next MapOffice in the EDGIS.PGE_MAPGEOLUT_MP1/2 table, favoring 
        ///// the one that is at the earliest stage of processing 
        ///// </summary>
        ///// <returns></returns>
        //public string GetNextMapGeo(IFeatureWorkspace pFWS)
        //{
        //    try
        //    {
        //        WriteToLogfile("Entering GetNextMapGeo");

        //        //Search the list of geos in the 
        //        ICursor pMapGeoCursor = null;
        //        ITable pMapGeoLUTTbl = pFWS.OpenTable(_mapGeoTable);
        //        int mapGeoFldIdx = pMapGeoLUTTbl.Fields.FindField("MAPGEO");
        //        int statusFldIdx = pMapGeoLUTTbl.Fields.FindField("STATUS");
        //        int failureCountFldIdx = pMapGeoLUTTbl.Fields.FindField("FAILURECOUNT");
        //        IQueryFilter pQF = new QueryFilterClass();
        //        string mapGeo = string.Empty;
        //        string nextMapGeo = string.Empty; 
        //        string targetStatus = string.Empty;
        //        string curStatus = string.Empty;
        //        string curMapGeo = string.Empty; 
        //        int curFailureCount = 0; 
        //        string additionalCriteria = string.Empty;
        //        bool processComplete = false; 

        //        //First check the assigned map geo list to determine if one of 
        //        //these geos has a status of 'Ready' 
        //        string[] commaSeparatedList = GetArrayOfCommaSeparatedKeys(
        //            _hshAssignedMapGeos, 1000, true);
        //        if (commaSeparatedList.Length == 1)
        //        {
        //            pQF.WhereClause =
        //                "STATUS" + " = " + "'" + ExtractConstants.EXTRACT_STATUS_READY + "'" + " And " +
        //                "MAPGEO" + " IN" + 
        //                "(" + 
        //                    commaSeparatedList[0] + 
        //                ")";
        //            pMapGeoCursor = pMapGeoLUTTbl.Search(pQF, false);
        //            IRow pMapGeoRow = pMapGeoCursor.NextRow();
        //            while (pMapGeoRow != null)
        //            {
        //                mapGeo = pMapGeoRow.get_Value(mapGeoFldIdx).ToString();

        //                //Check the landbase exists 
        //                if (GeodatabaseExistsInFolder(_centralGDBFolderPath, "LANDBASE_" + mapGeo))
        //                {
        //                    WriteToLogfile("Landbase found for: " + mapGeo);
        //                    nextMapGeo = mapGeo;
        //                    break;
        //                }
        //                pMapGeoRow = pMapGeoCursor.NextRow(); 
        //            }
        //            Marshal.FinalReleaseComObject(pMapGeoCursor); 
        //        }
        //        if (nextMapGeo != string.Empty)
        //        {
        //            WriteToLogfile("Returning assigned mapgeo: " + nextMapGeo +
        //                        " with status of Ready");
        //            return nextMapGeo; 
        //        }

        //        //Second look for the next map geography to process, favour 
        //        //the one which is at the least advanced status  

        //        //statuses include: 
        //        //  Ready 
        //        //  Extracting 
        //        //  Extract Error                  
        //        //  Mapping Error 
        //        //  Mapping
        //        //  Finished   

        //        //Keep trying until either all maps are finished or 
        //        //the failure count has been reached 
        //        while (!processComplete)
        //        {                    
        //            processComplete = true; 
        //            pMapGeoCursor = pMapGeoLUTTbl.Search(null, false);
        //            IRow pMapGeoRow = pMapGeoCursor.NextRow();
        //            while (pMapGeoRow != null)
        //            {
        //                curStatus = pMapGeoRow.get_Value(statusFldIdx).ToString();
        //                curFailureCount = Convert.ToInt32(pMapGeoRow.get_Value(failureCountFldIdx));
        //                curMapGeo = pMapGeoRow.get_Value(mapGeoFldIdx).ToString(); 

        //                if ((curStatus != ExtractConstants.EXTRACT_STATUS_FINISHED) && 
        //                    (curFailureCount < ExtractConstants.MAX_FAILURE_COUNT))
        //                {
        //                    WriteToLogfile( 
        //                        "found geo: " + curMapGeo + 
        //                        " with status: " + curStatus + 
        //                        " and failurecount: " + curFailureCount + 
        //                        " so the process is not yet completed!");
        //                    processComplete = false;
        //                    break; 
        //                }
        //                pMapGeoRow = pMapGeoCursor.NextRow();
        //            }
        //            Marshal.FinalReleaseComObject(pMapGeoCursor);                                        

        //            //Look for the next map geo 
        //            for (int i = 1; i < 6; i++)
        //            {
        //                additionalCriteria = string.Empty;
        //                switch (i)
        //                {
        //                    case (1):
        //                        targetStatus = ExtractConstants.EXTRACT_STATUS_READY;
        //                        break;
        //                    case (2):
        //                        targetStatus = ExtractConstants.EXTRACT_STATUS_EXTRACT_ERROR;
        //                        additionalCriteria = " And " + "FAILURECOUNT" + " < " +
        //                            ExtractConstants.MAX_FAILURE_COUNT.ToString();
        //                        break;
        //                    case (3):
        //                        targetStatus = ExtractConstants.EXTRACT_STATUS_MAPPING_ERROR;
        //                        additionalCriteria = " And " + "FAILURECOUNT" + " < " +
        //                            ExtractConstants.MAX_FAILURE_COUNT.ToString();
        //                        break;
        //                    case (4):
        //                        targetStatus = ExtractConstants.EXTRACT_STATUS_MAPPING;
        //                        break;
        //                    case (5):
        //                        targetStatus = ExtractConstants.EXTRACT_STATUS_EXTRACTING;
        //                        break;
        //                }

        //                //Prior to returning the geo make sure that the landbase has already 
        //                //been successfully extracted 

        //                pQF.WhereClause = "STATUS" + " = " + "'" + targetStatus + "'" + additionalCriteria;
        //                pMapGeoCursor = pMapGeoLUTTbl.Search(pQF, false);
        //                pMapGeoRow = pMapGeoCursor.NextRow();
        //                while (pMapGeoRow != null)
        //                {
        //                    mapGeo = pMapGeoRow.get_Value(mapGeoFldIdx).ToString();

        //                    //Check the landbase exists 
        //                    if (GeodatabaseExistsInFolder(_centralGDBFolderPath, "LANDBASE_" + mapGeo))
        //                    {
        //                        WriteToLogfile("Landbase found for: " + mapGeo);
        //                        nextMapGeo = mapGeo; 
        //                        break;
        //                    }
        //                    pMapGeoRow = pMapGeoCursor.NextRow();
        //                }
        //                Marshal.FinalReleaseComObject(pMapGeoCursor);

        //                //Next MapGeo found so we break out  
        //                if (nextMapGeo != string.Empty)
        //                    break; 
        //            }

        //            //Next MapGeo found so we break out  
        //            if (nextMapGeo != string.Empty)
        //                break;

        //            if ((!processComplete) && (nextMapGeo == ""))  
        //            {
        //                //Sleep for 3 minutes 
        //                WriteToLogfile("Process not complete and NextMapGeo not found so sleeping for: " +
        //                    ExtractConstants.NEXT_GEO_SLEEP_INTERVAL.ToString());
        //                Thread.Sleep(ExtractConstants.NEXT_GEO_SLEEP_INTERVAL); 
        //            }
        //        }

        //        //Be sure to release cursor 
        //        if (pMapGeoCursor != null)
        //            Marshal.FinalReleaseComObject(pMapGeoCursor);

        //        //Return the next mapGeo 
        //        if (nextMapGeo.Trim() != string.Empty)
        //            WriteToLogfile("Returning next map geo: " + nextMapGeo + " of current status: " + targetStatus);
        //        else
        //            WriteToLogfile("No more map geos found to process!");

        //        return nextMapGeo;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("Entering GetNextMapGeo Error Handler: " +
        //            ex.Message);
        //        return "";
        //    }
        //}


        /// <summary>
        /// Returns the actual name of the projection from the key that 
        /// is used as the name of the file GDB - these are the same thing 
        /// in map prod 2.0 
        /// </summary>
        /// <param name="mapGeoKey"></param>
        /// <returns></returns>
        private string GetMapGeoFromKey(string mapGeoKey)
        {
            try
            {
                WriteToLogfile("Entering GetMapGeoFromKey");

                //Convert the mapGeoKey into a valid legacycoord in the case of 
                //map prod 1.0 (for 2.0 the key and the mapGeo are the same thing) 
                string mapGeo = "";
                if (mapGeoKey != "")
                    mapGeo = GetLegacyCoordinateFromKey(mapGeoKey);
                return mapGeo;
            }
            catch (Exception ex)
            {
                WriteToLogfile("Entering GetMapGeoFromKey Error Handler: " +
                    ex.Message);
                throw new Exception("Error returing map geo from key: " + mapGeoKey);
            }
        }

        /// <summary>
        /// This routine goes to the configuration xml 
        /// file and looks up a number of settings that 
        /// the application uses as it runs. 
        /// </summary>
        /// 
        public void LoadConfigurationSettings(ExtractType pExtractType)
        {
            try
            {
                //Load the config document 
                string xPath = string.Empty;
                XmlDocument xmlDoc = GetConfigXMLDocument();
                XmlNode pXMLNode = null;

                //Logfile     
                xPath = "//ConfigSettings/AppSettings/Logfile";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                string logfilePrefix = "ED";
                if (pExtractType == ExtractType.ExtractTypeLandbase)
                    logfilePrefix = "LB";
                if (pXMLNode != null)
                {
                    //Append the threadId so if run in a multithreaded 
                    //situation each thread will have its own logfile 
                    string windowsProcessId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();
                    _logFilename = pXMLNode.InnerText + windowsProcessId +
                        logfilePrefix + "_" + _processId.ToString() + ".txt";
                }

                //Max tries for each featureclass  
                xPath = "//ConfigSettings/AppSettings/MaxTries";
                pXMLNode = xmlDoc.SelectSingleNode(xPath);
                if (pXMLNode != null)
                    _maxTries = Convert.ToInt32(pXMLNode.InnerText);

                //Workspaces 
                _hshWorkspacePaths = new Hashtable();
                string workspaceName = string.Empty;
                string workspacePath = string.Empty;
                XmlNode pSubNode = null;
                xPath = "//ConfigSettings/AppSettings/Workspaces";
                XmlNode pTopNode = xmlDoc.SelectSingleNode(xPath);
                for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
                {
                    pXMLNode = pTopNode.ChildNodes.Item(i);
                    if (pXMLNode != null)
                    {
                        for (int j = 0; j < pXMLNode.ChildNodes.Count; j++)
                        {
                            pSubNode = pXMLNode.ChildNodes.Item(j);
                            if (pSubNode.LocalName == "Name")
                                workspaceName = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "Connection")
                                //m4jf edgisrearch 919
                                if (pSubNode.InnerText.ToLower().EndsWith(".gdb"))
                                {
                                    workspacePath = pSubNode.InnerText.ToLower();
                                }
                                else
                                {
                                    workspacePath = ReadEncryption.GetSDEPath(pSubNode.InnerText.ToLower());
                                }
                               
                        }
                    }

                    if (workspaceName == "output")
                    {
                        int posOfLastSlash = workspacePath.LastIndexOf(@"\");
                        _workingGDBFolderPath = workspacePath.Substring(
                            0, posOfLastSlash);
                        _workingGDBName = workspacePath.Substring(
                            posOfLastSlash + 1, workspacePath.Length - (posOfLastSlash + 1));
                        _workingGDBName = _workingGDBName.Replace(".gdb", "").ToUpper();
                    }

                    if (pExtractType == ExtractType.ExtractTypeLandbase)
                    {
                        if ((workspaceName != "electric") &&
                            (workspaceName != "wip"))
                        {
                            if (!_hshWorkspacePaths.ContainsKey(workspaceName))
                                _hshWorkspacePaths.Add(workspaceName, workspacePath);
                        }
                    }
                    else
                    {
                        if (!_hshWorkspacePaths.ContainsKey(workspaceName))
                            _hshWorkspacePaths.Add(workspaceName, workspacePath);
                    }
                }

                //Extract datasets  
                _extractDatasets = new List<ExtractDataset>();
                ExtractDataset pExtractDS = null;
                string datasetSource = string.Empty;
                string datasetTarget = string.Empty;
                string filter = string.Empty;
                string sourceFeatureFilter = string.Empty;
                string sourceFeatureRelCondition = string.Empty;
                string workspace = string.Empty;
                string featureDataset = string.Empty;
                string joinWhereClause = string.Empty;
                string sourceJoinTable = string.Empty;
                bool useClip = false;
                bool includeAttributes = false;
                bool projectAfterExtract = false;
                bool mustHaveRel = false;
                ExtractDatasetType pDatasetType;
                int processId = -1;
                int posOfPeriod = -1;
                pSubNode = null;
                Hashtable hshOwnerNames = new Hashtable();
                string ownerName = "";
                if (pExtractType == ExtractType.ExtractTypeLandbase)
                {
                    hshOwnerNames.Add("lbgis", 0);
                }
                else
                {
                    hshOwnerNames.Add("edgis", 0);
                    hshOwnerNames.Add("webr", 0);
                }

                xPath = "//ConfigSettings/AppSettings/Datasets";
                pTopNode = xmlDoc.SelectSingleNode(xPath);
                for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
                {
                    pXMLNode = pTopNode.ChildNodes.Item(i);
                    if (pXMLNode.Name == "#comment")
                        continue;
                    pDatasetType = GetDatasetTypeFromString(pXMLNode.Name);
                    processId = -1;
                    datasetSource = string.Empty;
                    datasetTarget = string.Empty;
                    filter = string.Empty;
                    sourceFeatureFilter = string.Empty;
                    workspace = string.Empty;
                    featureDataset = string.Empty;
                    joinWhereClause = string.Empty;
                    sourceJoinTable = string.Empty;
                    ownerName = string.Empty;
                    sourceFeatureRelCondition = string.Empty;
                    mustHaveRel = false;

                    if (pXMLNode != null)
                    {
                        for (int j = 0; j < pXMLNode.ChildNodes.Count; j++)
                        {
                            pSubNode = pXMLNode.ChildNodes.Item(j);
                            if (pSubNode.LocalName == "ProcessNumber")
                                processId = Convert.ToInt32(pSubNode.InnerText);
                            if (pSubNode.LocalName == "SourceName")
                                datasetSource = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "TargetName")
                                datasetTarget = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "Filter")
                                filter = GetQueryFilterText(pSubNode.InnerText).ToLower();
                            if (pSubNode.LocalName == "Workspace")
                                workspace = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "FeatureDataset")
                                featureDataset = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "JoinWhereClause")
                                joinWhereClause = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "SourceJoinTable")
                                sourceJoinTable = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "UseClip")
                                useClip = Convert.ToBoolean(pSubNode.InnerText);
                            if (pSubNode.LocalName == "IncludeAttributes")
                                includeAttributes = Convert.ToBoolean(pSubNode.InnerText);
                            if (pSubNode.LocalName == "ProjectAfterExtract")
                                projectAfterExtract = Convert.ToBoolean(pSubNode.InnerText);
                            if (pSubNode.LocalName == "SourceFeatureFilter")
                                sourceFeatureFilter = pSubNode.InnerText.ToLower();
                            if (pSubNode.LocalName == "SourceFeatureRelCondition")
                            {
                                sourceFeatureRelCondition = pSubNode.InnerText.ToLower();
                                mustHaveRel = Convert.ToBoolean(pSubNode.Attributes["MustHaveRel"].Value.ToString());
                            }
                        }
                    }

                    //Strip out the owner name 
                    posOfPeriod = datasetSource.IndexOf(".");
                    if (posOfPeriod != -1)
                        ownerName = datasetSource.Substring(0, posOfPeriod).ToLower();

                    //Add the dataset based on the type of extract  
                    if (hshOwnerNames.ContainsKey(ownerName))
                    {
                        pExtractDS = new ExtractDataset(
                            pDatasetType,
                            datasetSource,
                            datasetTarget,
                            filter,
                            sourceFeatureFilter,
                            sourceFeatureRelCondition,
                            mustHaveRel,
                            workspace,
                            featureDataset,
                            sourceJoinTable,
                            joinWhereClause,
                            useClip,
                            includeAttributes,
                            projectAfterExtract,
                            processId);
                        _extractDatasets.Add(pExtractDS);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading configuration settings");
            }
        }

        private ExtractDatasetType GetDatasetTypeFromString(string datasetType)
        {
            try
            {
                ExtractDatasetType pDatasetType;
                switch (datasetType.ToLower())
                {
                    case "featureclass":
                        pDatasetType = ExtractDatasetType.etFeatureclass;
                        break;
                    case "annofeatureclass":
                        pDatasetType = ExtractDatasetType.etAnnoFeatureclass;
                        break;
                    case "joinfeatureclass":
                        pDatasetType = ExtractDatasetType.etJoinFeatureclass;
                        break;
                    case "table":
                        pDatasetType = ExtractDatasetType.etTable;
                        break;
                    case "relationshipclass":
                        pDatasetType = ExtractDatasetType.etRelationshipclass;
                        break;
                    default:
                        throw new Exception("Configuration error - unknown dataset type");
                }
                return pDatasetType;
            }
            catch (Exception ex)
            {
                throw new Exception("Configuration error - unknown dataset type");
            }
        }

        private string GetStringFromDatasetType(ExtractDatasetType pDatasetType)
        {
            try
            {
                string pDatasetTypeString;
                switch (pDatasetType)
                {
                    case ExtractDatasetType.etFeatureclass:
                        pDatasetTypeString = "featureclass";
                        break;
                    case ExtractDatasetType.etJoinFeatureclass:
                        pDatasetTypeString = "joinfeatureclass";
                        break;
                    case ExtractDatasetType.etTable:
                        pDatasetTypeString = "table";
                        break;
                    case ExtractDatasetType.etRelationshipclass:
                        pDatasetTypeString = "relationshipclass";
                        break;
                    default:
                        throw new Exception("Configuration error - unknown dataset type");
                }
                return pDatasetTypeString;
            }
            catch (Exception ex)
            {
                throw new Exception("Configuration error - unknown dataset type");
            }
        }

        private string GetQueryFilterText(string xmlQFText)
        {
            try
            {
                string qfText = xmlQFText;
                if (xmlQFText.IndexOf("[CDATA[") == 0)
                {
                    int lengthOfQF = xmlQFText.Length;
                    qfText = xmlQFText.Substring(7, lengthOfQF - 9);
                }
                return qfText;
            }
            catch (Exception ex)
            {
                return xmlQFText;
            }
        }

        public XmlDocument GetConfigXMLDocument()
        {
            try
            {
                //Open up the xml configuration file  
                string configFilename = "";

                //-------------------------------------------------------
                //ExportNetworkToDFIS 
                _installDirectory = new FileInfo(Assembly.GetExecutingAssembly().Location).DirectoryName;
                //------------------------------------------------------- 

                //-------------------------------------------------------
                //ExportArcFMNetwork 
                //string appFileName = Environment.GetCommandLineArgs()[0];
                //DEData.g_InstallDirectory = System.IO.Path.GetDirectoryName(appFileName);
                //------------------------------------------------------- 

                configFilename = _installDirectory + "\\" + ExtractConstants.CONFIG_FILENAME;
                if (!File.Exists(configFilename))
                {
                    throw new Exception("Unable to find config file: " + configFilename);
                }

                //Get the xml config document 
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configFilename);
                return xmlDoc;
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading configuration file");
            }
        }


        public void InitializeLogfile()
        {
            try
            {
                //Create a logfile
                System.IO.FileStream fs = File.Open(_logFilename,
                    FileMode.OpenOrCreate);
                fs.Close();
                if (File.Exists(_logFilename) == true)
                {
                    //Clear out the logfile 
                    File.WriteAllText(_logFilename, "");
                }
                _logfileInfo = new FileInfo(_logFilename);
            }
            catch
            {
                Debug.Print("Error initializing logfile");
            }
        }
        /// <summary>
        /// Loads all the workspaces referenced in the application 
        /// </summary>
        /// <param name="mapGeo"></param>
        private void LoadWorkspaces()
        {
            try
            {
                _hshWorkspaces = new Hashtable();
                ExtractUtils pExtractUtils = new ExtractUtils();
                foreach (string workspace in _hshWorkspacePaths.Keys)
                {
                    WriteToLogfile("connecting to workspace: " + workspace);
                    _hshWorkspaces.Add(workspace, pExtractUtils.GetWorkspace(_hshWorkspacePaths[workspace].ToString()));
                }
            }
            catch (Exception ex)
            {
                Debug.Print("Error in LoadWorkspaces: " + ex.Message);
            }
        }

        public void WriteToLogfile(string msg)
        {
            try
            {
                //fInfo.Create();
                using (StreamWriter sWriter = _logfileInfo.AppendText())
                {
                    sWriter.WriteLine(DateTime.Now.ToLongTimeString() + " " + msg);
                    sWriter.Close();
                }
            }
            catch
            {
                //Do nothing 
            }
        }

        /// <summary>
        /// Places all featureclasses in their featuredataset - prior to this they 
        /// were not in the featuredataset because multiple threads cannot edit 
        /// that same featuredataset at the same time with a file GDB at 10.0 
        /// </summary>
        //public void MoveDatasetsToFeatureDataset()
        //{
        //    try
        //    {
        //        //Load up the workspaces
        //        WriteToLogfile("Entering MoveDatasetsToFeatureDataset");
        //        IFeatureWorkspace pFWS = GetWorkspace(_hshWorkspacePaths["output"].ToString());

        //        //Open the featureclass 
        //        IFeatureClass pFC = null;
        //        IRelationshipClass pRC = null;
        //        IDataset pDS = null; 

        //        //Get a list of all the featuredatasets                 
        //        Hashtable hshFeatureDatasets = new Hashtable();
        //        foreach (ExtractDataset pExtractDataset in _extractDatasets)
        //        {
        //            if (pExtractDataset.FeatureDataset != string.Empty)
        //            {
        //                if (!hshFeatureDatasets.ContainsKey(pExtractDataset.FeatureDataset))
        //                    hshFeatureDatasets.Add(pExtractDataset.FeatureDataset, 0);
        //            }
        //        }

        //        foreach (string fd in hshFeatureDatasets.Keys)
        //        {
        //            if (fd != String.Empty)
        //            {
        //                IFeatureDataset pFD = pFWS.OpenFeatureDataset(fd);
        //                IDatasetContainer pDSContainer = (IDatasetContainer)pFD;
        //                foreach (ExtractDataset pExtractDataset in _extractDatasets)
        //                {
        //                    if (pExtractDataset.FeatureDataset == fd) 
        //                    {                                
        //                        if (pExtractDataset.DatasetType == ExtractDatasetType.etFeatureclass || 
        //                            pExtractDataset.DatasetType == ExtractDatasetType.etAnnoFeatureclass || 
        //                            pExtractDataset.DatasetType == ExtractDatasetType.etJoinFeatureclass) 
        //                        {
        //                            WriteToLogfile("Opening: " + pExtractDataset.TargetName + " to FeatureDataset: " + fd);
        //                            if (!pExtractDataset.TargetName.Contains("unproj"))
        //                            {
        //                                pFC = pFWS.OpenFeatureClass(pExtractDataset.TargetName);
        //                                WriteToLogfile("Rebuilding spatial index");
        //                                RebuildSpatialIndex(pFC);
        //                                pDS = (IDataset)pFC;
        //                                try
        //                                {
        //                                    WriteToLogfile("Adding: " + pExtractDataset.TargetName + " to FeatureDataset: " + fd); 
        //                                    pDSContainer.AddDataset((IDataset)pDS);
        //                                }
        //                                catch
        //                                {
        //                                    WriteToLogfile("Failed to add: " + pExtractDataset.TargetName + " to FeatureDataset: " + fd);
        //                                }
        //                            }
        //                        }                                
        //                    }
        //                }
        //            }
        //        }

        //        WriteToLogfile("Finished moving datasets to FeatureDataset"); 
        //    }
        //    catch (Exception ex)
        //    {
        //        //Do nothing - does not matter if dataset is not within a certain 
        //        //featuredataset - map production can still function ok 
        //        WriteToLogfile("Error in MoveDatasetsToFeatureDataset: " + ex.Message);                
        //    }
        //}

        /// <summary>
        /// Rebuilds indexes for all featureclasses that were populated and 
        /// compresses the File GDB for performance 
        /// </summary>
        /// <param name="mapGeo"></param>
        /// <param name="pExtractType"></param>
        public void PackageOutputGDB(string mapGeo, ExtractType pExtractType)
        {
            try
            {
                WriteToLogfile("Entering PackageOutputGDB");
                WriteToLogfile("    pExtractType: " + pExtractType.ToString());
                GeodatabaseUtils.ExtractUtils pExtractUtils =
                    new GeodatabaseUtils.ExtractUtils(_logfileInfo);
                //Places all datasets into their configured featuredataset 
                RebuildSpatialIndexesForFCs(pExtractUtils);
                //Compacts the GDB (for performance reasons) 
                pExtractUtils.CompactGeodatabase(
                    pExtractUtils.GetWorkspace(_hshWorkspacePaths["output"].ToString()));
            }
            catch (Exception ex)
            {
                throw new Exception("Error packaging the output GDB: " + ex.Message);
            }
        }

        public void CreatePlatLabels(string masterGDBPath)
        {
            try
            {
                WriteToLogfile("Entering CreatePlatLabels");
                GeodatabaseUtils.ExtractUtils pExtractUtils =
                    new GeodatabaseUtils.ExtractUtils(_logfileInfo);

                //Get the SDE workspace 
                LoadWorkspaces();
                IFeatureWorkspace pSourceFWS = (IFeatureWorkspace)_hshWorkspaces["electric"];
                IFeatureClass pMPLabelSourceFC = pSourceFWS.OpenFeatureClass("edgis.maintenanceplat");

                for (int i = 1; i <= 5; i++)
                {
                    IFeatureWorkspace pDestFWS = null;
                    switch (i)
                    {
                        case 1:
                            pDestFWS = pExtractUtils.GetWorkspace(masterGDBPath + "\\" + "MASTER_0401.gdb");
                            pExtractUtils.Transformation = new NADCONTransformationClass();
                            pExtractUtils.SpatialRef = GetSpatialReference(GetSRID(GetMapGeoFromKey("0401LL")));
                            WriteToLogfile("Processing MASTER_0401");
                            break;
                        case 2:
                            pDestFWS = pExtractUtils.GetWorkspace(masterGDBPath + "\\" + "MASTER_0402.gdb");
                            pExtractUtils.Transformation = new NADCONTransformationClass();
                            pExtractUtils.SpatialRef = GetSpatialReference(GetSRID(GetMapGeoFromKey("0402LL")));
                            WriteToLogfile("Processing MASTER_0402");
                            break;
                        case 3:
                            pDestFWS = pExtractUtils.GetWorkspace(masterGDBPath + "\\" + "MASTER_0403.gdb");
                            pExtractUtils.Transformation = new NADCONTransformationClass();
                            pExtractUtils.SpatialRef = GetSpatialReference(GetSRID(GetMapGeoFromKey("0403LL")));
                            WriteToLogfile("Processing MASTER_0403");
                            break;
                        case 4:
                            pDestFWS = pExtractUtils.GetWorkspace(masterGDBPath + "\\" + "MASTER_0404.gdb");
                            pExtractUtils.Transformation = new NADCONTransformationClass();
                            pExtractUtils.SpatialRef = GetSpatialReference(GetSRID(GetMapGeoFromKey("0404LL")));
                            WriteToLogfile("Processing MASTER_0404");
                            break;
                        case 5:
                            pDestFWS = pExtractUtils.GetWorkspace(masterGDBPath + "\\" + "MASTER_0405.gdb");
                            pExtractUtils.Transformation = new NADCONTransformationClass();
                            pExtractUtils.SpatialRef = GetSpatialReference(GetSRID(GetMapGeoFromKey("0405LL")));
                            WriteToLogfile("Processing MASTER_0405");
                            break;
                    }

                    IFeatureClass pMPLabelDestFC = pDestFWS.OpenFeatureClass("Maintenance_Plat_Labels");
                    WriteToLogfile("Deleting all records from Maintenance_Plat_Labels");
                    IWorkspaceEdit pWSE = (IWorkspaceEdit)pDestFWS;
                    pWSE.StartEditing(false);
                    pWSE.StartEditOperation();

                    //First delete all existing rows 
                    ((ITable)pMPLabelDestFC).DeleteSearchedRows(null);
                    pWSE.StopEditOperation();

                    //pMPLabelDestFC.                
                    pExtractUtils.ExportMPLabels(pMPLabelSourceFC, pMPLabelDestFC);

                    //Stop editing saving edits 
                    pWSE.StopEditing(true);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating Plat Labels: " + ex.Message);
            }
        }

        /// <summary>
        /// REbuilds the spatial index for every featureclass  
        /// </summary>
        public void RebuildSpatialIndexesForFCs(GeodatabaseUtils.ExtractUtils pExtractUtils)
        {
            try
            {
                //Load up the workspaces
                WriteToLogfile("Entering RebuildSpatialIndexesForFCs");
                IFeatureWorkspace pFWS = pExtractUtils.GetWorkspace(_hshWorkspacePaths["output"].ToString());

                //Open the featureclass 
                IFeatureClass pFC = null;
                string targetName = "";

                foreach (ExtractDataset pExtractDataset in _extractDatasets)
                {

                    if (pExtractDataset.DatasetType == ExtractDatasetType.etFeatureclass ||
                        pExtractDataset.DatasetType == ExtractDatasetType.etAnnoFeatureclass ||
                        pExtractDataset.DatasetType == ExtractDatasetType.etJoinFeatureclass)
                    {
                        targetName = pExtractDataset.TargetName;
                        //if (targetName.IndexOf("unproj") != -1)
                        //    targetName = targetName.Replace("_unproj", "");
                        pFC = pFWS.OpenFeatureClass(targetName);
                        WriteToLogfile("Rebuilding spatial index for: " + targetName);
                        pExtractUtils.RebuildSpatialIndex(pFC);
                    }
                }


                WriteToLogfile("Finished RebuildSpatialIndexesForFCs");
            }
            catch (Exception ex)
            {
                //Do nothing - does not matter if dataset is not within a certain 
                //featuredataset - map production can still function ok 
                WriteToLogfile("Error in RebuildSpatialIndexesForFCs: " + ex.Message);
            }
        }
        /// <summary>
        /// Populates extra field on the MaintenancePlat featureclass so 
        /// that they do not have to be determined using a point in polygon 
        /// query when the map is refreshed 
        /// </summary>

        //public void PopulatePlatLocationInfo()
        //{
        //    try
        //    {
        //        WriteToLogfile("Entering PopulatePlatLocationInfo");

        //        if (GetShortDatasetName(_clipFeatureclass).ToLower() != "plat_unified") 
        //        {
        //            WriteToLogfile("Exiting function as this routine is only relevent for plat_unified");
        //            return; 
        //        }

        //        //Get the output featureclass 
        //        IFeatureWorkspace pOutputFWS = (IFeatureWorkspace)_hshWorkspaces["output"];

        //        //Get the clip polygon
        //        //IFeatureClass pClipFC = pOutputFWS.OpenFeatureClass("Clip"); 
        //        //IFeatureCursor pFCursor = pClipFC.Search(null, false); 
        //        //IFeature pClipFeature = pFCursor.NextFeature();
        //        //IPolygon pClipPolygon = (IPolygon)pClipFeature.ShapeCopy; 
        //        //Marshal.FinalReleaseComObject(pFCursor); 

        //        //Get a hashtable of all serviceareas that are intersected by the entire clip 
        //        //polygon                
        //        Hashtable hshServiceAreaPolygons = new Hashtable(); 
        //        Hashtable hshServiceAreaAttributes = new Hashtable(); 
        //        IFeatureClass pServiceAreaFC = pOutputFWS.OpenFeatureClass("ServiceArea");
        //        int districtFldIdx = pServiceAreaFC.Fields.FindField("district"); 
        //        int divisionFldIdx = pServiceAreaFC.Fields.FindField("division"); 
        //        int regionFldIdx = pServiceAreaFC.Fields.FindField("region");
        //        //ISpatialFilter pSF = new SpatialFilterClass(); 
        //        //pSF.Geometry = pClipPolygon;
        //        //pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //        IFeatureCursor pFCursor = pServiceAreaFC.Search(null,false); 

        //        //Get a hashtable of the attributes for each of the service areas
        //        string servArea = ""; 
        //        IFeature pServAreaFeature = pFCursor.NextFeature(); 
        //        while (pServAreaFeature != null) 
        //        {
        //            servArea = "";
        //            string region = pServAreaFeature.get_Value(regionFldIdx).ToString();
        //            string division = pServAreaFeature.get_Value(divisionFldIdx).ToString();
        //            string district = pServAreaFeature.get_Value(districtFldIdx).ToString();
        //            servArea = region + "," + division + "," + district;
        //            hshServiceAreaAttributes.Add(pServAreaFeature.OID, servArea);
        //            hshServiceAreaPolygons.Add(pServAreaFeature.OID, pServAreaFeature.ShapeCopy);

        //            pServAreaFeature = pFCursor.NextFeature(); 
        //        }
        //        Marshal.FinalReleaseComObject(pFCursor); 

        //        IWorkspaceEdit pWSE = (IWorkspaceEdit)pOutputFWS;
        //        pWSE.StartEditing(false);
        //        pWSE.StartEditOperation();

        //        //Now loop through all of the plat_unified features and populate the 
        //        //ServiceAreas field 
        //        IFeatureClass pPlatUnifiedFC = pOutputFWS.OpenFeatureClass("plat_unified");
        //        int serviceAreasFldIdx = pPlatUnifiedFC.Fields.FindField("serviceareas"); 
        //        pFCursor = pPlatUnifiedFC.Update(null, false); 
        //        IRelationalOperator pRelOp = null;
        //        string serviceAreas = "";
        //        int serviceAreaCount = 0; 
        //        IFeature pPlatFeature = pFCursor.NextFeature();

        //        while (pPlatFeature != null) 
        //        {
        //            //Find which service areas the plat falls within
        //            pRelOp = (IRelationalOperator)pPlatFeature.ShapeCopy;
        //            serviceAreaCount = 0;
        //            serviceAreas = "";
        //            foreach (int oid in hshServiceAreaPolygons.Keys) 
        //            {                          
        //                if (!pRelOp.Disjoint((IGeometry)hshServiceAreaPolygons[oid])) 
        //                {
        //                    serviceAreaCount++; 
        //                    if (serviceAreas == "") 
        //                        serviceAreas = hshServiceAreaAttributes[oid].ToString();
        //                    else 
        //                        serviceAreas += "*" + hshServiceAreaAttributes[oid].ToString(); 
        //                }
        //            }

        //            //Set the field value on the plat_unified feature 
        //            if (serviceAreas == "")
        //                Debug.Print("no service area data found!"); 
        //            if (serviceAreaCount == 0)
        //                Debug.Print("no service area data found!"); 
        //            else if (serviceAreaCount > 1)
        //                Debug.Print("multiple service areas found!");

        //            pPlatFeature.set_Value(serviceAreasFldIdx, serviceAreas);
        //            pFCursor.UpdateFeature(pPlatFeature); 
        //            pPlatFeature = pFCursor.NextFeature();
        //        }

        //        //Stop editing and save edits 
        //        pWSE.StopEditOperation();
        //        pWSE.StopEditing(true); 

        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("Error in PopulatePlatLocationInfo: " + ex.Message);
        //    }
        //}

        /// <summary>
        /// Populates regional attributes for plat features so that they do not have to be 
        /// queried 'on the fly' during map production 
        /// </summary>
        public void PopulatePlatRegionalAttributes(IPolygon pClipPolygon, string mapGeo)
        {
            try
            {
                WriteToLogfile("Entering PopulatePlatRegionalAttributes");
                IFeatureWorkspace pDestFWS = (IFeatureWorkspace)_hshWorkspaces["output"];
                IFeatureWorkspace pSourceElecFWS = (IFeatureWorkspace)_hshWorkspaces["electric"];
                IFeatureWorkspace pSourceLandbaseFWS = (IFeatureWorkspace)_hshWorkspaces["landbase"];
                IFeatureClass pDivisionFC = pSourceLandbaseFWS.OpenFeatureClass("LBGIS.ElecDivision");
                IFeatureClass pCountyFC = pSourceLandbaseFWS.OpenFeatureClass("LBGIS.CountyUnclipped");
                IFeatureClass pDestPlatFC = pDestFWS.OpenFeatureClass(GetShortDatasetName(
                    ExtractConstants.PLAT_FC_NAME));
                IFeatureClass pSourcePlatFC = pSourceElecFWS.OpenFeatureClass(
                    ExtractConstants.PLAT_FC_NAME);

                IWorkspaceEdit pWSE = (IWorkspaceEdit)pDestFWS;
                pWSE.StartEditing(false);
                pWSE.StartEditOperation();

                PopulateMaintencePlatRegionalAttribute(pClipPolygon, pDivisionFC, pSourcePlatFC, pDestPlatFC, "division", "division_name");
                PopulateMaintencePlatRegionalAttribute(pClipPolygon, pCountyFC, pSourcePlatFC, pDestPlatFC, "cnty_name", "county_name");

                pWSE.StopEditOperation();
                pWSE.StopEditing(true);
            }
            catch (Exception ex)
            {
                WriteToLogfile("Error in PopulatePlatRegionalAttributes: " + ex.Message);
                throw new Exception("Error in PopulatePlatRegionalAttributes");
            }
        }

        /// <summary>
        /// Populates regional attributes on the maintenanceplat features 
        /// </summary>
        /// <param name="pLandbaseFC"></param>
        /// <param name="pMaintenancePlatFC"></param>
        /// <param name="landbaseFieldName"></param>
        /// <param name="maintenancePlatFieldName"></param>
        public void PopulateMaintencePlatRegionalAttribute(
            IPolygon pClipPolygon,
            IFeatureClass pLandbaseFC,
            IFeatureClass pSourcePlatFC,
            IFeatureClass pDestPlatFC,
            string landbaseFieldName,
            string maintenancePlatFieldName)
        {
            try
            {
                //Loop through each division/county - query all intersecting maintenanceplats 
                //Find the centroid of each maintenanceplat 
                //If the centroid of the maintenanceplat is inside the division/county 
                //Populate the division/county field on the maintenanceplat 
                WriteToLogfile("Populating maintenancePlatFieldName");
                Hashtable hshRegionalAttributes = new Hashtable();
                int regAttFldIdx = pLandbaseFC.Fields.FindField(landbaseFieldName);
                int mapNumberFldIdx = pSourcePlatFC.Fields.FindField("mapnumber");
                int mapOfficeFldIdx = pSourcePlatFC.Fields.FindField("mapoffice");
                string mapNumber = string.Empty;
                string mapOffice = string.Empty;
                string regAtt = string.Empty;
                ISpatialFilter pSFAll = new SpatialFilterClass();
                pSFAll.Geometry = pClipPolygon;
                pSFAll.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pLandbaseFCursor = pLandbaseFC.Search(pSFAll, false);
                IFeatureCursor pSourceMaintPlatFCursor;
                IPolygon pMPPolygon = null;
                IPolygon pLBPolygon = null;
                IArea pArea = null;
                IFeature pSourceMaintPlatFeature;
                IRelationalOperator pRelOp = null;
                IFeature pLBFeature = pLandbaseFCursor.NextFeature();

                while (pLBFeature != null)
                {
                    regAtt = pLBFeature.get_Value(regAttFldIdx).ToString();
                    pLBPolygon = (IPolygon)pLBFeature.ShapeCopy;
                    ISpatialFilter pSF = new SpatialFilterClass();
                    pSF.Geometry = pLBPolygon;
                    pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pRelOp = (IRelationalOperator)pLBPolygon;
                    pSourceMaintPlatFCursor = pSourcePlatFC.Search(pSF, false);
                    pSourceMaintPlatFeature = pSourceMaintPlatFCursor.NextFeature();

                    while (pSourceMaintPlatFeature != null)
                    {
                        if (pSourceMaintPlatFeature.Shape != null)
                        {
                            pMPPolygon = (IPolygon)pSourceMaintPlatFeature.ShapeCopy;
                            pArea = (IArea)pMPPolygon;

                            if (pRelOp.Contains(pArea.Centroid))
                            {
                                if (pSourceMaintPlatFeature.get_Value(mapNumberFldIdx) != DBNull.Value)
                                    mapNumber = pSourceMaintPlatFeature.get_Value(mapNumberFldIdx).ToString();
                                if (pSourceMaintPlatFeature.get_Value(mapOfficeFldIdx) != DBNull.Value)
                                    mapOffice = pSourceMaintPlatFeature.get_Value(mapOfficeFldIdx).ToString();

                                //Store the envelope parmaters in a hashtable 
                                if (!hshRegionalAttributes.ContainsKey(mapNumber + "*" + mapOffice))
                                    hshRegionalAttributes.Add(mapNumber + "*" + mapOffice, regAtt);
                                else
                                    Debug.Print("Unexpected");

                            }
                            else
                            {
                                Debug.Print("Centroid not contained");
                            }
                        }
                        else
                        {
                            Debug.Print("MaintenancePlat: " + pSourceMaintPlatFeature.OID.ToString() + " has null shape!");
                        }

                        pSourceMaintPlatFeature = pSourceMaintPlatFCursor.NextFeature();
                    }
                    Marshal.FinalReleaseComObject(pSourceMaintPlatFCursor);

                    pLBFeature = pLandbaseFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pLandbaseFCursor);


                //Now do an update cursor to update the dest MaintenancePlat 
                int maintPlatFldIdx = pDestPlatFC.Fields.FindField(maintenancePlatFieldName);
                IFeatureCursor pDestMPFCursor = pDestPlatFC.Update(null, false);
                IFeature pDestMPFFeature = pDestMPFCursor.NextFeature();
                while (pDestMPFFeature != null)
                {
                    if (pDestMPFFeature.get_Value(mapNumberFldIdx) != DBNull.Value)
                        mapNumber = pDestMPFFeature.get_Value(mapNumberFldIdx).ToString();
                    if (pDestMPFFeature.get_Value(mapOfficeFldIdx) != DBNull.Value)
                        mapOffice = pDestMPFFeature.get_Value(mapOfficeFldIdx).ToString();

                    if (hshRegionalAttributes.ContainsKey(mapNumber + "*" + mapOffice))
                    {
                        regAtt = hshRegionalAttributes[mapNumber + "*" + mapOffice].ToString();
                        pDestMPFFeature.set_Value(maintPlatFldIdx, regAtt);
                    }
                    else
                    {
                        Debug.Print("Outside the envelope!");
                        pDestMPFFeature.set_Value(maintPlatFldIdx, string.Empty);
                    }
                    pDestMPFCursor.UpdateFeature(pDestMPFFeature);
                    pDestMPFFeature = pDestMPFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pDestMPFCursor);

            }
            catch (Exception ex)
            {
                WriteToLogfile("Error in PopulateMaintencePlatRegionalAttribute: " + ex.Message);
                throw new Exception("Error in PopulateMaintencePlatRegionalAttribute");
            }
        }

        /// <summary>
        /// Populates attributes on the maintenanceplat that describe which 
        /// plats are directly adjacent 
        /// </summary>
        public void PopulatePlatAdjacentATEAttributes(IPolygon pClipPolygon, ExtractUtils pExtractUtils)
        {
            try
            {
                WriteToLogfile("Populating PopulatePlatAdjacentATEAttributes");

                //Project the polygon to the output spatial reference
                IClone pClone = (IClone)pClipPolygon;
                IPolygon pClipPolygonProj = (IPolygon)pClone.Clone();
                pClipPolygonProj.SpatialReference = pClipPolygon.SpatialReference;
                pClipPolygonProj = (IPolygon)pExtractUtils.Project(pClipPolygonProj, _toSpatialRef, _transformation);

                //Open the source maintenanceplat featureclass 
                IFeatureWorkspace pDestFWS = (IFeatureWorkspace)_hshWorkspaces["output"];
                IFeatureClass pDestPlatFC = pDestFWS.OpenFeatureClass(GetShortDatasetName(
                    ExtractConstants.PLAT_FC_NAME));
                IGeoDataset pGS = (IGeoDataset)pDestPlatFC;

                //Cache field indexes for performance 
                int destMapScaleFldIdx = pDestPlatFC.Fields.FindField(
                    ExtractConstants.PLAT_FC_MAPSCALE);
                int destMapNumberFldIdx = pDestPlatFC.Fields.FindField(
                    ExtractConstants.PLAT_FC_MAPNUMBER);
                int destNorthATEFldIdx = pDestPlatFC.Fields.FindField("NORTH_ATE");
                int destSouthATEFldIdx = pDestPlatFC.Fields.FindField("SOUTH_ATE");
                int destEastATEFldIdx = pDestPlatFC.Fields.FindField("EAST_ATE");
                int destWestATEFldIdx = pDestPlatFC.Fields.FindField("WEST_ATE");
                int destNWATEFldIdx = pDestPlatFC.Fields.FindField("NORTHWEST_ATE");
                int destNEATEFldIdx = pDestPlatFC.Fields.FindField("NORTHEAST_ATE");
                int destSWATEFldIdx = pDestPlatFC.Fields.FindField("SOUTHWEST_ATE");
                int destSEATEFldIdx = pDestPlatFC.Fields.FindField("SOUTHEAST_ATE");

                IWorkspaceEdit pWSE = (IWorkspaceEdit)pDestFWS;
                pWSE.StartEditing(false);
                pWSE.StartEditOperation();

                int adjacentFieldIdx = -1;
                int counter = 0;
                string curMapType = string.Empty;
                string curMapNumber = string.Empty;
                string adjacentATEDisplayText = string.Empty;
                string adjacentMapNumber = string.Empty;
                Hashtable hshUsedMapNumbers = new Hashtable();

                double envelopeWidth = 0;
                double envelopeHeight = 0;
                IEnvelope mapGridEnvelope = null;
                IPoint centerPoint = null;
                IPoint NPoint = null;
                IPoint SPoint = null;
                IPoint WPoint = null;
                IPoint EPoint = null;
                IPoint NWPoint = null;
                IPoint NEPoint = null;
                IPoint SWPoint = null;
                IPoint SEPoint = null;
                IPoint pRefPoint = null;
                IPolygon pRefPointBuffer = null;
                ITopologicalOperator pTopo = null;
                IRelationalOperator pRelOp = null;
                ISpatialFilter pSF;
                ISpatialFilter pSFAll = new SpatialFilterClass();
                pSFAll.Geometry = pClipPolygonProj;
                pSFAll.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor pDestMPFCursor = pDestPlatFC.Update(pSFAll, false);
                IFeature pDestMPFFeature = pDestMPFCursor.NextFeature();
                IFeatureCursor pSourceMPFCursor;
                IFeature pSourceMPFeature = null;

                while (pDestMPFFeature != null)
                {
                    //Query the source maintenanceplat featureclass using this 
                    //polygon 
                    counter++;
                    hshUsedMapNumbers.Clear();

                    if (destMapScaleFldIdx != -1)
                        curMapType = pDestMPFFeature.get_Value(destMapScaleFldIdx).ToString();
                    if (destMapNumberFldIdx != -1)
                        curMapNumber = pDestMPFFeature.get_Value(destMapNumberFldIdx).ToString();

                    pSF = new SpatialFilterClass();
                    pSF.Geometry = pDestMPFFeature.ShapeCopy;
                    pSF.WhereClause =
                            ExtractConstants.PLAT_FC_MAPNUMBER + " <> " + "'" + curMapNumber + "'" + " And " +
                            ExtractConstants.PLAT_FC_MAPSCALE + " = " + curMapType;
                    pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                    mapGridEnvelope = pDestMPFFeature.ShapeCopy.Envelope;
                    envelopeWidth = mapGridEnvelope.XMax - mapGridEnvelope.XMin;
                    envelopeHeight = mapGridEnvelope.YMax - mapGridEnvelope.YMin;

                    centerPoint = new PointClass();
                    centerPoint.SpatialReference = pGS.SpatialReference;
                    NPoint = new PointClass();
                    NPoint.SpatialReference = pGS.SpatialReference;
                    SPoint = new PointClass();
                    SPoint.SpatialReference = pGS.SpatialReference;
                    EPoint = new PointClass();
                    EPoint.SpatialReference = pGS.SpatialReference;
                    WPoint = new PointClass();
                    WPoint.SpatialReference = pGS.SpatialReference;
                    NWPoint = new PointClass();
                    NWPoint.SpatialReference = pGS.SpatialReference;
                    NEPoint = new PointClass();
                    NEPoint.SpatialReference = pGS.SpatialReference;
                    SWPoint = new PointClass();
                    SWPoint.SpatialReference = pGS.SpatialReference;
                    SEPoint = new PointClass();
                    SEPoint.SpatialReference = pGS.SpatialReference;

                    //set the points to the N,S,E,W,NW,NE,SW and SE of current map grid feature

                    centerPoint.X = mapGridEnvelope.XMin + (envelopeWidth / 2);
                    centerPoint.Y = mapGridEnvelope.YMin + (envelopeHeight / 2);
                    NPoint.X = centerPoint.X;
                    NPoint.Y = centerPoint.Y + envelopeHeight;
                    SPoint.X = centerPoint.X;
                    SPoint.Y = centerPoint.Y - envelopeHeight;
                    EPoint.X = centerPoint.X + envelopeWidth;
                    EPoint.Y = centerPoint.Y;
                    WPoint.X = centerPoint.X - envelopeWidth;
                    WPoint.Y = centerPoint.Y;
                    NWPoint.X = centerPoint.X - envelopeWidth;
                    NWPoint.Y = centerPoint.Y + envelopeHeight;
                    NEPoint.X = centerPoint.X + envelopeWidth;
                    NEPoint.Y = centerPoint.Y + envelopeHeight;
                    SWPoint.X = centerPoint.X - envelopeWidth;
                    SWPoint.Y = centerPoint.Y - envelopeHeight;
                    SEPoint.X = centerPoint.X + envelopeWidth;
                    SEPoint.Y = centerPoint.Y - envelopeHeight;

                    pSourceMPFCursor = pDestPlatFC.Search(pSF, false);
                    pSourceMPFeature = pSourceMPFCursor.NextFeature();

                    while (pSourceMPFeature != null)
                    {

                        //Initially get a default value for each ATE 
                        adjacentATEDisplayText = " ";
                        adjacentMapNumber = pSourceMPFeature.get_Value(destMapNumberFldIdx).ToString();

                        //Set the rel operator 
                        pRelOp = (IRelationalOperator)pSourceMPFeature.ShapeCopy;

                        //First do the N,S,E,W ATEs
                        for (int i = 1; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 1:
                                    adjacentFieldIdx = destNorthATEFldIdx;
                                    pRefPoint = NPoint;
                                    break;
                                case 2:
                                    adjacentFieldIdx = destSouthATEFldIdx;
                                    pRefPoint = SPoint;
                                    break;
                                case 3:
                                    adjacentFieldIdx = destEastATEFldIdx;
                                    pRefPoint = EPoint;
                                    break;
                                case 4:
                                    adjacentFieldIdx = destWestATEFldIdx;
                                    pRefPoint = WPoint;
                                    break;
                            }

                            //Buffer by one foot 
                            pTopo = (ITopologicalOperator)pRefPoint;
                            pRefPointBuffer = (IPolygon)pTopo.Buffer(1);

                            //There is intersection 
                            if (!pRelOp.Disjoint(pRefPointBuffer))
                            {
                                if (!hshUsedMapNumbers.ContainsKey(adjacentMapNumber))
                                {
                                    pDestMPFFeature.set_Value(adjacentFieldIdx, adjacentMapNumber);
                                    hshUsedMapNumbers.Add(adjacentMapNumber, 0);
                                }
                            }
                        }

                        //Now do the corner ATEs  
                        for (int i = 1; i <= 4; i++)
                        {
                            switch (i)
                            {
                                case 1:
                                    adjacentFieldIdx = destNWATEFldIdx;
                                    pRefPoint = NWPoint;
                                    break;
                                case 2:
                                    adjacentFieldIdx = destNEATEFldIdx;
                                    pRefPoint = NEPoint;
                                    break;
                                case 3:
                                    adjacentFieldIdx = destSWATEFldIdx;
                                    pRefPoint = SWPoint;
                                    break;
                                case 4:
                                    adjacentFieldIdx = destSEATEFldIdx;
                                    pRefPoint = SEPoint;
                                    break;
                            }

                            //Buffer by one foot 
                            pTopo = (ITopologicalOperator)pRefPoint;
                            pRefPointBuffer = (IPolygon)pTopo.Buffer(1);

                            //There is intersection 
                            if (!pRelOp.Disjoint(pRefPointBuffer))
                            {
                                if (!hshUsedMapNumbers.ContainsKey(adjacentMapNumber))
                                {
                                    pDestMPFFeature.set_Value(adjacentFieldIdx, adjacentMapNumber);
                                    hshUsedMapNumbers.Add(adjacentMapNumber, 0);
                                }
                            }
                        }
                        pSourceMPFeature = pSourceMPFCursor.NextFeature();
                    }
                    Marshal.FinalReleaseComObject(pSourceMPFCursor);
                    pSourceMPFCursor = null;

                    //Periodically do a garbage collect as memory useage is going up due to 
                    //the nested cursors 
                    if (counter == 100)
                    {
                        GC.Collect();
                        counter = 0;
                    }

                    pDestMPFCursor.UpdateFeature(pDestMPFFeature);
                    pDestMPFFeature = pDestMPFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pDestMPFCursor);

                //Stop editing and save edits 
                pWSE.StopEditOperation();
                pWSE.StopEditing(true);
            }
            catch (Exception ex)
            {
                WriteToLogfile("Error in PopulatePlatAdjacentATEAttributes: " + ex.Message);
                throw new Exception("Error in PopulatePlatAdjacentATEAttributes");
            }
        }

        /// <summary>
        /// Populates attributes on the maintenanceplat that describe the bounding  
        /// coordinates of the plat in WGS 84 coordinates  
        /// </summary>
        public void PopulatePlatEnvelopeParameters(IPolygon pClipPolygon)
        {
            try
            {
                WriteToLogfile("Populating PopulatePlatMinMaxCoordinates");

                //Open the source maintenanceplat featureclass 
                IFeatureWorkspace pSourceFWS = (IFeatureWorkspace)_hshWorkspaces["electric"];
                IFeatureWorkspace pDestFWS = (IFeatureWorkspace)_hshWorkspaces["output"];
                IFeatureClass pDestPlatFC = pDestFWS.OpenFeatureClass(GetShortDatasetName(
                    ExtractConstants.PLAT_FC_NAME));
                IFeatureClass pSourcePlatFC = pSourceFWS.OpenFeatureClass(
                    ExtractConstants.PLAT_FC_NAME);

                //First open the source featureclass and do a spatial search 
                //for all plats within the clip 
                ISpatialFilter pSF;
                IFeatureCursor pSourceMPFCursor;
                IFeature pSourceMPFeature = null;
                IEnvelope mapGridFeatureEnvelope = null;
                EnvelopeParameters pEnvParams = null;
                Hashtable hshEnvelopeParams = new Hashtable();
                int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;
                ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
                ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem pSR_WGS84;
                pSR_WGS84 = pSRF.CreateGeographicCoordinateSystem(geoType);

                pSF = new SpatialFilterClass();
                pSF.Geometry = pClipPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                int mapNumberFldIdx = pSourcePlatFC.Fields.FindField("mapnumber");
                int mapOfficeFldIdx = pSourcePlatFC.Fields.FindField("mapoffice");
                string mapNumber = string.Empty;
                string mapOffice = string.Empty;
                pSourceMPFCursor = pSourcePlatFC.Search(pSF, false);
                pSourceMPFeature = pSourceMPFCursor.NextFeature();

                while (pSourceMPFeature != null)
                {
                    if (pSourceMPFeature.get_Value(mapNumberFldIdx) != DBNull.Value)
                        mapNumber = pSourceMPFeature.get_Value(mapNumberFldIdx).ToString();
                    if (pSourceMPFeature.get_Value(mapOfficeFldIdx) != DBNull.Value)
                        mapOffice = pSourceMPFeature.get_Value(mapOfficeFldIdx).ToString();

                    //Get the envelope and project to WGS 1984 
                    mapGridFeatureEnvelope = pSourceMPFeature.ShapeCopy.Envelope;
                    mapGridFeatureEnvelope.Project(pSR_WGS84);
                    pEnvParams = new EnvelopeParameters(
                        Math.Round(mapGridFeatureEnvelope.XMin, 6),
                        Math.Round(mapGridFeatureEnvelope.XMax, 6),
                        Math.Round(mapGridFeatureEnvelope.YMin, 6),
                        Math.Round(mapGridFeatureEnvelope.YMax, 6));

                    //Store the envelope parmaters in a hashtable 
                    if (!hshEnvelopeParams.ContainsKey(mapNumber + "*" + mapOffice))
                        hshEnvelopeParams.Add(mapNumber + "*" + mapOffice, pEnvParams);

                    pSourceMPFeature = pSourceMPFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pSourceMPFCursor);

                //Cache field indexes for performance 
                int destMapScaleFldIdx = pDestPlatFC.Fields.FindField(
                    ExtractConstants.PLAT_FC_MAPSCALE);
                int destMapNumberFldIdx = pDestPlatFC.Fields.FindField(
                    ExtractConstants.PLAT_FC_MAPNUMBER);
                int xMinFldIdx = pDestPlatFC.Fields.FindField("XMIN");
                int xMaxFldIdx = pDestPlatFC.Fields.FindField("XMAX");
                int yMinFldIdx = pDestPlatFC.Fields.FindField("YMIN");
                int yMaxFldIdx = pDestPlatFC.Fields.FindField("YMAX");
                mapNumberFldIdx = pDestPlatFC.Fields.FindField("mapnumber");
                mapOfficeFldIdx = pDestPlatFC.Fields.FindField("mapoffice");

                IWorkspaceEdit pWSE = (IWorkspaceEdit)pDestFWS;
                pWSE.StartEditing(false);
                pWSE.StartEditOperation();

                IFeatureCursor pDestMPFCursor = pDestPlatFC.Update(null, false);
                IFeature pDestMPFFeature = pDestMPFCursor.NextFeature();

                while (pDestMPFFeature != null)
                {

                    if (pDestMPFFeature.get_Value(mapNumberFldIdx) != DBNull.Value)
                        mapNumber = pDestMPFFeature.get_Value(mapNumberFldIdx).ToString();
                    if (pDestMPFFeature.get_Value(mapOfficeFldIdx) != DBNull.Value)
                        mapOffice = pDestMPFFeature.get_Value(mapOfficeFldIdx).ToString();

                    if (hshEnvelopeParams.ContainsKey(mapNumber + "*" + mapOffice))
                    {
                        pEnvParams = (EnvelopeParameters)hshEnvelopeParams[mapNumber + "*" + mapOffice];
                        pDestMPFFeature.set_Value(xMinFldIdx, pEnvParams.XMin);
                        pDestMPFFeature.set_Value(xMaxFldIdx, pEnvParams.XMax);
                        pDestMPFFeature.set_Value(yMinFldIdx, pEnvParams.YMin);
                        pDestMPFFeature.set_Value(yMaxFldIdx, pEnvParams.YMax);
                    }
                    else
                    {
                        Debug.Print("Unable to set the envelope params!");
                        pDestMPFFeature.set_Value(xMinFldIdx, 0);
                        pDestMPFFeature.set_Value(xMaxFldIdx, 0);
                        pDestMPFFeature.set_Value(yMinFldIdx, 0);
                        pDestMPFFeature.set_Value(yMaxFldIdx, 0);
                    }
                    pDestMPFCursor.UpdateFeature(pDestMPFFeature);
                    pDestMPFFeature = pDestMPFCursor.NextFeature();
                }
                Marshal.FinalReleaseComObject(pDestMPFCursor);

                //Stop editing and save edits 
                pWSE.StopEditOperation();
                pWSE.StopEditing(true);
            }
            catch (Exception ex)
            {
                WriteToLogfile("Error in PopulatePlatEnvelopeParameters: " + ex.Message);
                throw new Exception("Error in PopulatePlatEnvelopeParameters");
            }
        }

        /// <summary>
        /// The main routine that creates a clip polygon and calls the 
        /// ExportGeodatabase_GOLD routine which performs the extraction 
        /// </summary>
        /// <param name="mapGeo"></param>
        /// <param name="persistClipPolygon"></param>
        /// <returns></returns>
        public bool ProcessExtract(
            string mapGeo,
            bool useChangeDetection)
        {
            try
            {
                //Connect to the necessary workspaces
                WriteToLogfile("Performing extract");
                WriteToLogfile("    PROCESSID: " + _processId.ToString());
                WriteToLogfile("    MAPGEO: " + mapGeo);
                WriteToLogfile("    useChangeDetection: " + useChangeDetection.ToString());

                //Load up the workspaces 
                WriteToLogfile("connecting to workspaces");
                LoadWorkspaces();
                WriteToLogfile("======================================================");

                //Find the clip polygon for the extract 
                IPolygon pClipPolygon = null;
                pClipPolygon = GetClipPolygon(mapGeo, useChangeDetection);

                //Export the datasets 
                ExportGeodatabase_GOLD(
                    pClipPolygon,
                    mapGeo,
                    _maxTries);
                WriteToLogfile("ProcessExtract completed");
                return true;

            }
            catch (Exception ex)
            {
                WriteToLogfile("An error occurred in ProcessExtract: " +
                    ex.Message);
                return false;
            }
        }


        /// <summary>
        /// A routine that passes the extraction to the appropriate 
        /// extraction routine depending on the parameters supplied 
        /// for example - the extraction may or may not require the 
        /// attributes to be extracted 
        /// </summary>
        /// <param name="msgFC"></param>
        /// <param name="pSourceFC"></param>
        /// <param name="pDestFC"></param>
        /// <param name="pClipPolygon"></param>
        private void ExportAnno(
            string msgFC,
            string mapGeo,
            ExtractDataset pExtractDataset,
            IFeatureClass pSourceFC,
            IFeatureClass pDestFC,
            IPolygon pClipPolygon,
            ExtractUtils pExtractUtils)
        {

            try
            {

                if (pExtractDataset.IncludeAttributes)
                {
                    //Pull back the anno attributes 
                    pExtractUtils.ExportAnnoFeature(
                        msgFC,
                        pExtractDataset.Filter,
                        pExtractDataset.SourceFeatureFilter,
                        pExtractDataset.UseClip,
                        !pExtractDataset.ProjectAfterExtract,
                        pSourceFC,
                        pDestFC,
                        pClipPolygon);
                }
                else
                {
                    //Pull back the shape only 
                    if (pExtractDataset.SourceFeatureRelClass != string.Empty)
                    {
                        pExtractUtils.ExportAnnoGraphicWithSourceRelCondition(
                            msgFC,
                            pExtractDataset.Filter,
                            pExtractDataset.SourceFeatureFilter,
                            mapGeo,
                            pExtractDataset.SourceFeatureRelClass,
                            pExtractDataset.MustHaveRel,
                            pExtractDataset.UseClip,
                            !pExtractDataset.ProjectAfterExtract,
                            pSourceFC,
                            pDestFC,
                            pClipPolygon);
                    }
                    else
                    {
                        pExtractUtils.ExportAnnoGraphic(
                            msgFC,
                            pExtractDataset.Filter,
                            pExtractDataset.SourceFeatureFilter,
                            mapGeo,
                            pExtractDataset.UseClip,
                            !pExtractDataset.ProjectAfterExtract,
                            pSourceFC,
                            pDestFC,
                            pClipPolygon);
                    }
                }

            }
            catch (Exception ex)
            {
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw ex;
            }
        }

        private Hashtable GetRelatedFeatureOIds(
            IFeatureClass pAnnoFC,
            string outputAnnoDSName,
            string sourceFeatureFilter)
        {
            try
            {
                WriteToLogfile("Entering GetRelatedFeatureOIds");
                //applyFilter = true; 
                IEnumRelationshipClass pEnumRel = pAnnoFC.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                IRelationshipClass pRelCls = pEnumRel.Next();
                IDataset pDS = null;
                string originDSName = "";
                //string defQuery = "";
                ITable pSourceFC = null;
                Hashtable hshOIds = new Hashtable();

                if (pRelCls != null)
                {
                    pDS = (IDataset)pRelCls.OriginClass;
                    pSourceFC = (ITable)pRelCls.OriginClass;
                    originDSName = pDS.Name.ToLower();
                    originDSName = GetShortDatasetName(originDSName);
                    WriteToLogfile("originDSName: " + originDSName);
                }

                //If there is no definition query do not apply the filter 
                if (sourceFeatureFilter == "")
                    return hshOIds;

                WriteToLogfile("Applying whereclause: " + sourceFeatureFilter);
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = sourceFeatureFilter;
                pQF.SubFields = pSourceFC.OIDFieldName;
                ICursor pCursor = pSourceFC.Search(pQF, false);
                IRow pRow = pCursor.NextRow();

                //if (isOdd)
                //{
                //    while (pRow != null)
                //    {
                //        if (pRow.OID % 2 == 1)
                //            hshOIds.Add(pRow.OID, 0);
                //        pRow = pCursor.NextRow(); 
                //    }
                //}
                //else if (isEven)
                //{
                //    while (pRow != null)
                //    {
                //        if (pRow.OID % 2 == 0)
                //            hshOIds.Add(pRow.OID, 0);
                //        pRow = pCursor.NextRow();
                //    }
                //}
                //else 
                //{
                while (pRow != null)
                {
                    hshOIds.Add(pRow.OID, 0);
                    pRow = pCursor.NextRow();
                }
                //}

                Marshal.FinalReleaseComObject(pCursor);

                //return the definition query 
                return hshOIds;
            }
            catch (Exception ex)
            {
                WriteToLogfile("An error occurred in: " + "GetRelatedFeatureOIds: " + ex.Message);
                throw new Exception("Error returning the OIds for source annotation featureclass");
            }
        }

        /// <summary>
        /// Returns the clip shape from the working file GDB 
        /// </summary>
        /// <param name="mapGeo"></param>
        /// <returns></returns>
        private IPolygon GetClipPolygon(string mapGeo, bool useChangeDetection)
        {
            try
            {
                WriteToLogfile("Entering GetClipPolygon");
                WriteToLogfile("mapGeography: " + mapGeo);
                WriteToLogfile("useChangeDetection: " + useChangeDetection);

                //Get the MapGeo_UTM featureclass 
                IPolygon pClipPolygon = null;

                ExtractUtils pExtractUtils = new ExtractUtils();
                WriteToLogfile("fileGDB: " + _workingGDBFolderPath + @"\" + "ClipGDB.gdb");
                IFeatureWorkspace pFWS = pExtractUtils.GetWorkspace(_workingGDBFolderPath + @"\" + "ClipGDB.gdb");
                IFeatureClass pMapGeoFC = null;

                //if (useChangeDetection)
                  //  pMapGeoFC = pFWS.OpenFeatureClass("MapGeo_UTM_CD");
               // else
                    pMapGeoFC = pFWS.OpenFeatureClass("MapGeo_UTM_FULL");

                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = "MAPGEO" + " = " + "'" + mapGeo + "'";
                IFeatureCursor pFCursor = pMapGeoFC.Search(pQF, false);
                IFeature pFeature = pFCursor.NextFeature();
                if (pFeature != null)
                    pClipPolygon = (IPolygon)pFeature.ShapeCopy;
                else
                    WriteToLogfile("Error no clip polygon found!!!");
                Marshal.FinalReleaseComObject(pFCursor);

                if (pClipPolygon == null)
                {
                    WriteToLogfile("The Clip Polygon is null!");
                }
                else if (pClipPolygon.IsEmpty)
                {
                    WriteToLogfile("The Clip Polygon is empty!");
                }

                //Return the clip polygon 
                return pClipPolygon;
            }
            catch (Exception ex)
            {
                WriteToLogfile("An error occurred in GetClipPolygon: " +
                    ex.Message);
                WriteToLogfile("An error occurred in GetClipPolygon: " +
                    ex.StackTrace);
                return null;
            }
        }


        private string GetDomainValueFromCode(IObjectClass pOC, string fieldName, string domainCode)
        {
            try
            {
                int fldIdx = pOC.Fields.FindField(fieldName);
                IField pField = pOC.Fields.get_Field(fldIdx);
                string domainValue = "";
                if (pField.Domain != null)
                {
                    if (pField.Domain is ICodedValueDomain)
                    {
                        ICodedValueDomain codedValueDomain = (ICodedValueDomain)pField.Domain;
                        for (int i = 0; i < codedValueDomain.CodeCount; i++)
                        {
                            if (codedValueDomain.get_Value(i).ToString().ToLower() == domainCode.ToLower())
                            {
                                domainValue = codedValueDomain.get_Name(i).ToString();
                                break;
                            }
                        }
                    }
                }
                return domainValue;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning domain code from domain vallue");
            }
        }

        //private IPolygon GetClipPolygon2()
        //{
        //    try
        //    {
        //        //Perform the query and return the polygon 
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)_hshWorkspaces[_clipWorkspace.ToLower()];
        //        string mapNumberList = "";
        //        IQueryFilter pQF = new QueryFilterClass();
        //        if (_clipOnlyExportStateFromList)
        //        {
        //            ITable pMapNumberCoordTbl = pFWS.OpenTable("EDGIS.PGE_MAPNUMBERCOORDLUT");
        //            pQF.WhereClause = "EXPORTSTATE IN(" + _clipExportStateList + ")";
        //            ICursor pCursor = pMapNumberCoordTbl.Search(pQF, false);
        //            string mapNumber = "";
        //            int mapNumberFldIdx = pMapNumberCoordTbl.Fields.FindField("mapnumber");
        //            Hashtable hshMapNumbers = new Hashtable();
        //            IRow pRow = pCursor.NextRow();
        //            while (pRow != null)
        //            {
        //                if (pRow.get_Value(mapNumberFldIdx) != DBNull.Value)
        //                {
        //                    mapNumber = pRow.get_Value(mapNumberFldIdx).ToString();
        //                    if (!hshMapNumbers.ContainsKey(mapNumber))
        //                        hshMapNumbers.Add(mapNumber, 0);
        //                }
        //                pRow = pCursor.NextRow();
        //            }
        //            Marshal.FinalReleaseComObject(pCursor);
        //            mapNumberList = GetCommaSeparatedList(hshMapNumbers);
        //        }

        //        string clipFinalWhereClause = "";
        //        IFeatureClass pClipFC = pFWS.OpenFeatureClass(_clipFeatureclass);
        //        clipFinalWhereClause = _clipWhereclause;
        //        if (_clipOnlyExportStateFromList)
        //        {
        //            WriteToLogfile("Intersecting clip area with just maps with: " +
        //                "EXPORTSTATE IN(" + _clipExportStateList + ")");
        //            clipFinalWhereClause = clipFinalWhereClause + " AND MAPNUMBER IN (" + mapNumberList + ")";
        //        }
        //        pQF.WhereClause = clipFinalWhereClause;
        //        WriteToLogfile("Applying clip whereclause: " + pQF.WhereClause);
        //        IFeatureCursor pFCursor = pClipFC.Search(pQF, false);
        //        IFeature pFeature = pFCursor.NextFeature();
        //        IPolygon pUnionPolygon = null;
        //        ITopologicalOperator pTopo = null; 

        //        while (pFeature != null)
        //        {
        //            if (pUnionPolygon == null)
        //            {
        //                pUnionPolygon = (IPolygon)pFeature.ShapeCopy;
        //                pUnionPolygon.SpatialReference = pFeature.Shape.SpatialReference;
        //                pTopo = (ITopologicalOperator)pUnionPolygon; 
        //            }
        //            else
        //            {
        //                pUnionPolygon = (IPolygon)pTopo.Union(pFeature.ShapeCopy); 
        //            }
        //            pFeature = pFCursor.NextFeature();
        //        }
        //        Marshal.FinalReleaseComObject(pFCursor);

        //        //Construct the union of the polygons 
        //        //ITopologicalOperator pTopo = new PolygonClass();
        //        //IPolygon pClipPolygon = (IPolygon)pTopo;
        //        //pTopo.ConstructUnion(pGeometryBag as IEnumGeometry);
        //        //Simplify the shape to be safe 
        //        ITopologicalOperator2 pTopo2 = (ITopologicalOperator2)pUnionPolygon;
        //        pTopo2.IsKnownSimple_2 = false;
        //        pTopo2.Simplify();

        //        //Apply a buffer if necessary passed in the configuration 
        //        if (_clipBufferDistance != 0)
        //        {
        //            ITopologicalOperator kTopo = (ITopologicalOperator)pUnionPolygon;
        //            pUnionPolygon = (IPolygon)kTopo.Buffer(Convert.ToDouble(_clipBufferDistance));

        //            //Simplify again after the buffer 
        //            ITopologicalOperator2 kTopo2 = (ITopologicalOperator2)pUnionPolygon;
        //            kTopo2.IsKnownSimple_2 = false;
        //            kTopo2.Simplify();
        //        }

        //        //Return the clip polygon 
        //        return (IPolygon)pUnionPolygon;
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteToLogfile("An error occurred in GetClipPolygon: " +
        //            ex.Message);
        //        return null;
        //    }
        //}

        //private void SaveClipGeometry(IPolygon pClipPolygon, string mapGeo)
        //{
        //    IWorkspaceEdit pWSE = null; 

        //    try
        //    {
        //        pWSE = (IWorkspaceEdit)_hshWorkspaces["output"];
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSE;
        //        IFeatureClass pClipFC = pFWS.OpenFeatureClass("clip");
        //        int mapOfficeFldIdx = pClipFC.Fields.FindField("MAPGEO");  

        //        pWSE = (IWorkspaceEdit)pFWS;
        //        pWSE.StartEditing(false);
        //        pWSE.StartEditOperation();

        //        IFeature pNewFeature = pClipFC.CreateFeature();
        //        pNewFeature.Shape = pClipPolygon;
        //        if (mapOfficeFldIdx != -1)
        //            pNewFeature.set_Value(mapOfficeFldIdx, mapGeo); 

        //        pNewFeature.Store(); 

        //        pWSE.StopEditOperation();
        //        pWSE.StopEditing(true); 
        //    }
        //    catch (Exception ex)
        //    {
        //        if (pWSE != null) 
        //            pWSE.AbortEditOperation(); 
        //        WriteToLogfile("Error saving the clip geometry: " + ex.Message); 
        //    }
        //}

        /// <summary>
        /// Exports the SDE data into a file GDB - you can clip and 
        /// if no clip polygon is supplied it will take all the data 
        /// </summary>
        /// <param name="pClipPolygon"></param>
        /// <param name="maxTries"></param>
        /// <param name="saveClip"></param>
        public void ExportGeodatabase_GOLD(
            IPolygon pClipPolygon,
            string mapGeo,
            int maxTries)
        {
            IWorkspaceEdit pWSE = null;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                WriteToLogfile("Entering ExportGeodatabase_GOLD");
                WriteToLogfile("    mapGeo: " + mapGeo);
                WriteToLogfile("    maxTries: " + maxTries.ToString());

                //Get reference to geodatabase utils to assist with extraction 
                _transformation = new NADCONTransformationClass();
                _toSpatialRef = GetSpatialReference(GetSRID(GetMapGeoFromKey(mapGeo)));
                GeodatabaseUtils.ExtractUtils pExtractUtils = new GeodatabaseUtils.ExtractUtils(
                    _logfileInfo,
                    _toSpatialRef,
                    _transformation);

                //Scroll through each dataset exporting each one 
                string datasetType = string.Empty;
                bool exportSucceeded = false;
                bool includesPlatFC = false;
                int fcTotal = 0;
                int fcCount = 0;
                string msgDS = "";
                IFeatureWorkspace pSourceFWS = null;
                IFeatureWorkspace pDestFWS = (IFeatureWorkspace)_hshWorkspaces["output"];
                pWSE = (IWorkspaceEdit)pDestFWS;
                IFeatureClass pSourceFC = null;
                IFeatureClass pDestFC = null;
                ITable pSourceTable = null;
                ITable pSourceJoinTable = null;
                ITable pDestTable = null;
                string msg = string.Empty;
                string shortSourceName = string.Empty;

                //Export the normal FCs first, then the Anno FCs, then any tables 
                for (int j = 0; j < 4; j++)
                {

                    //Determine the number of datasets to be processed 
                    fcTotal = 0;
                    fcCount = 0;
                    foreach (ExtractDataset pExtractDS in _extractDatasets)
                    {
                        if (ProcessDataset(j, pExtractDS))
                            fcTotal++;
                    }

                    //Start editing output workspace
                    if (fcTotal > 0)
                        pWSE.StartEditing(false);

                    foreach (ExtractDataset pExtractDS in _extractDatasets)
                    {
                        if (ProcessDataset(j, pExtractDS))
                        {
                            fcCount++;
                            exportSucceeded = false;

                            for (int i = 0; i < maxTries; i++)
                            {
                                msgDS = "Exporting " + datasetType + ": " + pExtractDS.TargetName + " " +
                                    fcCount + " of: " + fcTotal.ToString();
                                WriteToLogfile("======================================================");
                                WriteToLogfile(msgDS);

                                //Open source and destination featureclass/table 
                                pSourceFWS = (IFeatureWorkspace)_hshWorkspaces[pExtractDS.Workspace];
                                if (pExtractDS.DatasetType == ExtractDatasetType.etFeatureclass ||
                                    pExtractDS.DatasetType == ExtractDatasetType.etAnnoFeatureclass ||
                                    pExtractDS.DatasetType == ExtractDatasetType.etJoinFeatureclass)
                                {
                                    WriteToLogfile("opening source " + datasetType + ": " + pExtractDS.SourceName);
                                    pSourceFC = pSourceFWS.OpenFeatureClass(pExtractDS.SourceName);
                                    WriteToLogfile("opening target " + datasetType + ": " + pExtractDS.TargetName);
                                    pDestFC = pDestFWS.OpenFeatureClass(pExtractDS.TargetName);
                                    if (((IGeoDataset)pDestFC).SpatialReference.Name != _toSpatialRef.Name)
                                        throw new Exception("Spatial references do not match!");
                                    else
                                        WriteToLogfile("spatial ref check is ok");

                                }
                                else if (pExtractDS.DatasetType == ExtractDatasetType.etTable)
                                {
                                    WriteToLogfile("opening source " + datasetType + ": " + pExtractDS.SourceName);
                                    pSourceTable = pSourceFWS.OpenTable(pExtractDS.SourceName);
                                    WriteToLogfile("opening target " + datasetType + ": " + pExtractDS.TargetName);
                                    pDestTable = pDestFWS.OpenTable(pExtractDS.TargetName);
                                }

                                if (pExtractDS.DatasetType == ExtractDatasetType.etJoinFeatureclass)
                                {
                                    WriteToLogfile("opening source " + datasetType + ": " + pExtractDS.SourceJoinTable);
                                    pSourceJoinTable = pSourceFWS.OpenTable(pExtractDS.SourceJoinTable);
                                }

                                try
                                {
                                    //Perform the export                                        
                                    switch (j)
                                    {
                                        case 0:
                                            pExtractUtils.ExportFeatureClassLO(
                                            msgDS,
                                            pExtractDS.UseClip,
                                            true,
                                            pSourceFC,
                                            pDestFC,
                                            pClipPolygon,
                                            pExtractDS.Filter);
                                            break;
                                        case 1:

                                            ExportAnno(
                                            msgDS,
                                            mapGeo,
                                            pExtractDS,
                                            pSourceFC,
                                            pDestFC,
                                            pClipPolygon,
                                            pExtractUtils);
                                            break;
                                        case 2:
                                            pExtractUtils.ExportTable(
                                            msgDS,
                                            pSourceTable,
                                            pDestTable,
                                            pExtractDS.Filter);
                                            break;
                                        case 3:
                                            pExtractUtils.ExportFeatureClassWithJoin(
                                            msgDS,
                                            pExtractDS.UseClip,
                                            true,
                                            pSourceFC,
                                            pDestFC,
                                            pSourceJoinTable,
                                            pClipPolygon,
                                            pExtractDS.JoinWhereClause);
                                            break;
                                    }

                                    //succeeded so break out 
                                    exportSucceeded = true;

                                    WriteToLogfile(datasetType + ": " + pExtractDS.TargetName +
                                        " succeeded on attempt: " + (i + 1).ToString());
                                    if (pExtractDS.TargetName.ToLower() == GetShortDatasetName(
                                        ExtractConstants.PLAT_FC_NAME).ToLower())
                                        includesPlatFC = true;
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    WriteToLogfile(datasetType + ": " + pExtractDS.TargetName +
                                        " failed on attempt " + (i + 1).ToString() +
                                        " with error: " + ex.Message);
                                }
                            }
                            if (!exportSucceeded)
                                throw new Exception("Export of ds: " + pExtractDS.TargetName + " failed");
                        }
                    }

                    //Stop editing saving edits 
                    if (fcTotal > 0)
                        pWSE.StopEditing(true);

                    switch (j)
                    {
                        case 0:
                            msg = "Normal FCs completed successfully";
                            break;
                        case 1:
                            msg = "Anno FCs completed successfully";
                            break;
                        case 2:
                            msg = "Tables completed successfully";
                            break;
                        case 3:
                            msg = "Joined FCs completed successfully";
                            break;
                    }
                    WriteToLogfile(msg);
                }

                //If this includes the maintenanceplat/plat_unified then populate extra attributes 
                if (includesPlatFC)
                {
                    WriteToLogfile("Polulating the plat attributes");
                    PopulatePlatRegionalAttributes(pClipPolygon, mapGeo);
                    PopulatePlatAdjacentATEAttributes(pClipPolygon, pExtractUtils);
                    PopulatePlatEnvelopeParameters(pClipPolygon);
                }

                pSourceFWS = null;
                pDestFWS = null;
                pWSE = null;
                pSourceFC = null;
                pDestFC = null;
                pSourceTable = null;
                pDestTable = null;

                WriteToLogfile("Finished entire export for process: " + _processId.ToString() +
                " in: " + stopWatch.ElapsedMilliseconds + " milliseconds");
                stopWatch.Stop();

            }
            catch (Exception ex)
            {
                WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                if (pWSE != null)
                    pWSE.StopEditing(false);
                WriteToLogfile("An error occurred in " + MethodBase.GetCurrentMethod() +
                    "edits have been aborted");
                throw new Exception("Error in ExportGeodatabase_GOLD");
            }
        }

        /// <summary>
        /// Returns the ISpatialReference corresponding to the 
        /// passed srid 
        /// </summary>
        /// <param name="srid"></param>
        /// <returns></returns>
        private ISpatialReference GetSpatialReference(int srid)
        {
            try
            {

                ISpatialReference pSpRef = null;
                ISpatialReferenceFactory spRefFactory = new SpatialReferenceEnvironmentClass();
                pSpRef = spRefFactory.CreateProjectedCoordinateSystem(srid);
                return pSpRef;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning spatialreference from SRID");
            }
        }

        /// <summary>
        /// Looks up the projection file for projecting a dataset 
        /// to stateplane 
        /// </summary>
        /// <param name="mapGeo"></param>
        /// <returns></returns>
        private string GetSpatialReferenceProjectionFile(string mapGeo)
        {
            try
            {
                string projFile = System.IO.Path.GetDirectoryName(
                        System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
                if (mapGeo.Contains("0401"))
                    projFile += ExtractConstants.PROJ_FILE_0401;
                else if (mapGeo.Contains("0402"))
                    projFile += ExtractConstants.PROJ_FILE_0402;
                else if (mapGeo.Contains("0403"))
                    projFile += ExtractConstants.PROJ_FILE_0403;
                else if (mapGeo.Contains("0404"))
                    projFile += ExtractConstants.PROJ_FILE_0404;
                else if (mapGeo.Contains("0405"))
                    projFile += ExtractConstants.PROJ_FILE_0405;
                if (!File.Exists(projFile))
                {
                    WriteToLogfile("Configured path to projection file is incorrect: " +
                        projFile);
                    throw new Exception("Configured path to projection file is incorrect");
                }

                return projFile;
            }
            catch (Exception ex)
            {
                throw new Exception("Error finding projection the .prj file referenced in the config xml document");
            }
        }

        ///// <summary>
        ///// Projects the datasets to the State Plane projection of the 
        ///// particular map e.g. NAD 1927 StatePlane California III FIPS 0403 
        ///// </summary>
        //private void ProjectDatasets(string legacyCoord)
        //{
        //    try
        //    {
        //        int fcCount = 0;
        //        Geoprocessor gp = new Geoprocessor();
        //        IFeatureWorkspace pOutputFWS = (IFeatureWorkspace)_hshWorkspaces["output"];
        //        //IFeatureWorkspace pProjectedFWS = (IFeatureWorkspace)_hshWorkspaces["projected"];

        //        foreach (ExtractDataset pExtractDS in _extractDatasets)
        //        {
        //            if (ProcessDataset(0, pExtractDS))
        //            {
        //                fcCount++;
        //                //msgDS = "Projecting " + pExtractDS.TargetName + " " + fcCount + 
        //                //    " of: " + _extractDatasets.Count.ToString();
        //                //WriteToLogfile("======================================================");
        //                //WriteToLogfile(msgDS);

        //                //Open source and destination featureclass/table 
        //                //pSourceFWS = (IFeatureWorkspace)_hshWorkspaces[pExtractDS.Workspace];
        //                if (pExtractDS.DatasetType == ExtractDatasetType.etFeatureclass ||
        //                    pExtractDS.DatasetType == ExtractDatasetType.etAnnoFeatureclass ||
        //                    pExtractDS.DatasetType == ExtractDatasetType.etJoinFeatureclass)
        //                {
        //                    //gp.SetParameterValue(0, @"C:\EDGISExtracts1.0\Output\MERCED.gdb\" + pExtractDS.TargetName); 
        //                    //gp.SetParameterValue(1, @"C:\EDGISExtracts1.0\Output\PROJECTED.gdb\" + pExtractDS.TargetName);
        //                    //gp.SetParameterValue(2, "NAD_1927_StatePlane_California_III_FIPS_0403");
        //                    //gp.SetParameterValue(3, "NAD_1927_To_NAD_1983_NADCON");
        //                    WriteToLogfile("Before execute for: " + pExtractDS.TargetName);
        //                    Project projTool = new Project();
        //                    projTool.in_dataset = @"C:\EDGISExtracts1.0\Output\EDGIS.gdb\" + pExtractDS.TargetName;
        //                    projTool.out_dataset = @"C:\EDGISExtracts1.0\Output\EDGIS_PROJ.gdb\" + pExtractDS.TargetName;
        //                    projTool.out_coor_system = GetCoordSystemKey(legacyCoord);
        //                    //projTool.out_coor_system = "NAD 1927 StatePlane California IV FIPS 0404";
        //                    projTool.transform_method = "NAD_1927_To_NAD_1983_NADCON";
        //                    gp.Execute(projTool, null);
        //                    WriteToLogfile("After execute for: " + pExtractDS.TargetName);
        //                }
        //                else if (pExtractDS.DatasetType == ExtractDatasetType.etTable)
        //                {
        //                    //WriteToLogfile("opening source " + datasetType + ": " + pExtractDS.SourceName);
        //                    //pSourceTable = pSourceFWS.OpenTable(pExtractDS.SourceName);
        //                    //WriteToLogfile("opening target " + datasetType + ": " + pExtractDS.TargetName);
        //                    //pDestTable = pDestFWS.OpenTable(pExtractDS.TargetName);
        //                    //Have to copy the table to the output workspace 

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error projecting datasets to State Plane Coordinate System"); 
        //    }
        //}

        /// <summary>
        /// Looks up the name of the legacycoordinate for the passed 
        /// mspGeo 
        /// </summary>
        /// <param name="mapGeoKey"></param>
        /// <returns></returns>
        private string GetLegacyCoordinateFromKey(string mapGeoKey)
        {
            try
            {
                string mapGeo = string.Empty;
                if (mapGeoKey.Contains("0401"))
                    mapGeo = ExtractConstants.LEG_COORD_0401;
                else if (mapGeoKey.Contains("0402"))
                    mapGeo = ExtractConstants.LEG_COORD_0402;
                else if (mapGeoKey.Contains("0403"))
                    mapGeo = ExtractConstants.LEG_COORD_0403;
                else if (mapGeoKey.Contains("0404"))
                    mapGeo = ExtractConstants.LEG_COORD_0404;
                else if (mapGeoKey.Contains("0405"))
                    mapGeo = ExtractConstants.LEG_COORD_0405;
                return mapGeo;
            }
            catch (Exception ex)
            {
                throw new Exception("Error unknown legacy coordinate");
            }
        }

        /// <summary>
        /// Gets the SRID of the coordinate system represented by the 
        /// passed legacyCoord 
        /// </summary>
        /// <param name="legacyCoord"></param>
        /// <returns></returns>
        private int GetSRID(string legacyCoord)
        {
            try
            {
                int SRID = -1;
                switch (legacyCoord)
                {
                    case ExtractConstants.LEG_COORD_0401:
                        SRID = 26741;
                        break;
                    case ExtractConstants.LEG_COORD_0402:
                        SRID = 26742;
                        break;
                    case ExtractConstants.LEG_COORD_0403:
                        SRID = 26743;
                        break;
                    case ExtractConstants.LEG_COORD_0404:
                        SRID = 26744;
                        break;
                    case ExtractConstants.LEG_COORD_0405:
                        SRID = 26745;
                        break;
                    default:
                        throw new Exception("Unknown spatial referenence factory code");
                }
                return SRID;
            }
            catch (Exception ex)
            {
                throw new Exception("Error projecting datasets to State Plane Coordinate System");
            }
        }

        /// <summary>
        /// Routine to determine whether the passed ExtractDataset should 
        /// be extracted by the currently running process 
        /// </summary>
        /// <param name="j">flag to indicate the type of dataset e.g. Anno Featureclass</param>
        /// <param name="pExtractDS">the ExtractDataset in question</param>
        /// <returns></returns>
        private bool ProcessDataset(int j, ExtractDataset pExtractDS)
        {
            try
            {
                bool processDataset = false;
                if (j == 0)
                {
                    if ((pExtractDS.DatasetType == ExtractDatasetType.etFeatureclass) &&
                        (pExtractDS.TargetName.ToLower().Contains("anno") == false) &&
                        (pExtractDS.ProcessNumber == _processId))
                        processDataset = true;
                }
                else if (j == 1)
                {
                    if ((pExtractDS.DatasetType == ExtractDatasetType.etAnnoFeatureclass) &&
                        ((pExtractDS.ProcessNumber == _processId)))
                        processDataset = true;
                }
                else if (j == 2)
                {
                    if ((pExtractDS.DatasetType == ExtractDatasetType.etTable) &&
                        (pExtractDS.ProcessNumber == _processId))
                        processDataset = true;
                }
                else if (j == 3)
                {
                    if ((pExtractDS.DatasetType == ExtractDatasetType.etJoinFeatureclass) &&
                        (pExtractDS.ProcessNumber == _processId))
                        processDataset = true;
                }
                return processDataset;
            }
            catch (Exception ex)
            {
                throw new Exception("Error determining if dataset should be processed");
            }
        }

        private string GetShortDatasetName(string datasetName)
        {
            try
            {
                string shortDatasetName = "";
                int posOfLastPeriod = -1;

                posOfLastPeriod = datasetName.LastIndexOf(".");
                if (posOfLastPeriod != -1)
                {
                    shortDatasetName = datasetName.Substring(
                        posOfLastPeriod + 1, (datasetName.Length - posOfLastPeriod) - 1);
                }
                else
                {
                    shortDatasetName = datasetName;
                }

                return shortDatasetName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning the shortened dataset name");
            }
        }

        private string GetOwnerFromDatasetName(string datasetName)
        {
            try
            {
                string owner = "";
                int posOfLastPeriod = -1;

                posOfLastPeriod = datasetName.LastIndexOf(".");
                if (posOfLastPeriod != -1)
                    owner = datasetName.Substring(0, posOfLastPeriod);

                return owner;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning the shortened dataset name");
            }
        }

        public void SetWorkspaceSpatialReference(IWorkspace pWS, ISpatialReference pSpRef)
        {
            try
            {
                IFeatureWorkspace pFWS = (IFeatureWorkspace)pWS;
                IEnumDataset datasets = pWS.get_Datasets(esriDatasetType.esriDTAny);
                IDataset dataset = null;
                IGeoDataset pGDS = null;
                ExtractUtils pExtractUils = new ExtractUtils();

                while ((dataset = datasets.Next()) != null)
                {
                    if (dataset is IGeoDataset)
                    {
                        pGDS = (IGeoDataset)dataset;
                        if (pGDS.SpatialReference.Name != pSpRef.Name)
                            pExtractUils.SetDatasetSpatialReference(ref pGDS, pSpRef);
                    }
                    dataset = datasets.Next();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error setting spatial reference of one or more datasets in workspace");
            }
        }

        //public void RemoveFieldsNotRequired()
        //{
        //    try
        //    {
        //        LoadConfigurationSettings(); 
        //        string xPath = "";
        //        string temp = "";
        //        XmlDocument xmlDoc = GetConfigXMLDocument();
        //        XmlNode pXMLNode = null;

        //        //FieldsNotRequired  
        //        Hashtable hshFieldsNotRequired = new Hashtable();
        //        string fieldName = "";
        //        XmlNode pSubNode = null;
        //        xPath = "//ConfigSettings/AppSettings/FieldsNotRequired";
        //        XmlNode pTopNode = xmlDoc.SelectSingleNode(xPath);
        //        for (int i = 0; i < pTopNode.ChildNodes.Count; i++)
        //        {
        //            pXMLNode = pTopNode.ChildNodes.Item(i);
        //            if (pXMLNode != null)
        //            {
        //                for (int j = 0; j < pXMLNode.ChildNodes.Count; j++)
        //                {
        //                    pSubNode = pXMLNode.ChildNodes.Item(j);
        //                    if (pSubNode.LocalName == "Name")
        //                        fieldName = pSubNode.InnerText.ToLower();
        //                }
        //            }
        //            if (!hshFieldsNotRequired.ContainsKey(fieldName))
        //                hshFieldsNotRequired.Add(fieldName, 0);
        //        }      

        //        //Get the master workspace 
        //        IFeatureWorkspace pFWS = GetWorkspace( 
        //            _workingGDBFolderPath + "\\" + _workingGDBName);

        //        //Open every featureclass 
        //        foreach (ExtractDataset pExtractDS in _extractDatasets)
        //        {
        //            if ((pExtractDS.DatasetType == ExtractDatasetType.etFeatureclass) &&
        //                (pExtractDS.TargetName.ToLower().Contains("anno") == false))
        //            {
        //                IFeatureClass pFC = pFWS.OpenFeatureClass(pExtractDS.TargetName);
        //                Debug.Print("Before for: " + pExtractDS.TargetName + " fieldcount: " + pFC.Fields.FieldCount.ToString());

        //                if (!pExtractDS.TargetName.ToLower().Contains("wip"))
        //                {
        //                    foreach (string fldName in hshFieldsNotRequired.Keys)
        //                    {
        //                        int fldIdx = pFC.Fields.FindField(fldName);
        //                        if (fldIdx != -1)
        //                        {
        //                            Debug.Print("removing field: " + fldName);
        //                            pFC.DeleteField(pFC.Fields.get_Field(fldIdx));
        //                        }
        //                    }
        //                }
        //                Debug.Print("After for: " + pExtractDS.TargetName + " fieldcount: " + pFC.Fields.FieldCount.ToString());
        //            }                    
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.Print("Error in RemoveFieldsNotRequired: " + ex.Message); 
        //    }
        //}        

    }

    public class ExtractDataset
    {
        //Property storage variables 
        private ExtractDatasetType _datasetType;
        private string _sourceName;
        private string _targetName;
        private int _processNumber;
        private string _filter;
        private string _sourceFeatureFilter;
        private string _workspace;
        private string _featuredataset;
        private string _sourceJoinTable;
        private string _sourceFeatureRelClass;
        private string _joinWhereClause;
        bool _includeAttributes;
        bool _useClip;
        bool _projectAfterExtract;
        bool _mustHaveRel;

        public ExtractDataset(
            ExtractDatasetType datasetType,
            string sourceName,
            string targetName,
            string filter,
            string sourceFeatureFilter,
            string sourceFeatureRelClass,
            bool mustHaveRel,
            string workspace,
            string featureDataset,
            string sourceJoinTable,
            string joinWhereClause,
            bool useClip,
            bool includeAttribs,
            bool projectAfterExtract,
            int processNumber)
        {
            _datasetType = datasetType;
            _sourceName = sourceName;
            _targetName = targetName;
            _filter = filter;
            _sourceFeatureFilter = sourceFeatureFilter;
            _sourceFeatureRelClass = sourceFeatureRelClass;
            _mustHaveRel = mustHaveRel;
            _workspace = workspace;
            _featuredataset = featureDataset;
            _sourceJoinTable = sourceJoinTable;
            _joinWhereClause = joinWhereClause;
            _useClip = useClip;
            _includeAttributes = includeAttribs;
            _projectAfterExtract = projectAfterExtract;
            _processNumber = processNumber;
        }

        public ExtractDatasetType DatasetType
        {
            get { return _datasetType; }
        }
        //public ExtractFrequency ExtractFrequency
        //{
        //    get { return _extractFrequency; }
        //    set { _extractFrequency = value;}
        //}
        public string SourceName
        {
            get { return _sourceName; }
        }
        public string SourceJoinTable
        {
            get { return _sourceJoinTable; }
        }
        public string JoinWhereClause
        {
            get { return _joinWhereClause; }
        }
        public string TargetName
        {
            get { return _targetName; }
        }
        public string Filter
        {
            get { return _filter; }
        }
        public string Workspace
        {
            get { return _workspace; }
        }
        public string FeatureDataset
        {
            get { return _featuredataset; }
        }
        public int ProcessNumber
        {
            get { return _processNumber; }
        }
        public bool UseClip
        {
            get { return _useClip; }
        }
        public bool IncludeAttributes
        {
            get { return _includeAttributes; }
        }
        public bool ProjectAfterExtract
        {
            get { return _projectAfterExtract; }
        }
        public string SourceFeatureFilter
        {
            get { return _sourceFeatureFilter; }
        }
        public string SourceFeatureRelClass
        {
            get { return _sourceFeatureRelClass; }
        }
        public bool MustHaveRel
        {
            get { return _mustHaveRel; }
        }
    }


    public class EnvelopeParameters
    {
        //Property storage variables 
        private double _xMin;
        private double _xMax;
        private double _yMin;
        private double _yMax;

        public EnvelopeParameters(
            double xMin,
            double xMax,
            double yMin,
            double yMax)
        {
            _xMin = xMin;
            _xMax = xMax;
            _yMin = yMin;
            _yMax = yMax;
        }

        public double XMin
        {
            get { return _xMin; }
        }
        public double XMax
        {
            get { return _xMax; }
        }
        public double YMin
        {
            get { return _yMin; }
        }
        public double YMax
        {
            get { return _yMax; }
        }
    }
}
