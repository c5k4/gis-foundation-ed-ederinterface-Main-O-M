using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Data;

using System.Configuration;

using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using PGE_SqlServerUtils_Thin;
using Osmose.PGE.PLS.Services;
using Osmose.PGE.PLS.Services.Models;
using System.ServiceModel;
using System.Configuration;

namespace PLDBBatchProcess
{   

    class PLDBProcessor
    {
        private ISpatialReference m_pSR_WGS84;
        private SqlConnection _pConn = null; 
        
        public void SynchronizePLDB()
        {
            try
            {
                //Load up the workspaces 
                Shared.InitializeLogfile();
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Populate the SYNC_EDGIS_POLES table
                if (ConfigurationManager.AppSettings[
                    PLDBBatchConstants.CONFIG_ENABLE_POP_EDGIS_POLES]
                    .ToUpper().Trim() == "T")
                {
                    PopulateEdgisPoles();
                }

                //Populate the SYNC_PLDB_POLES table
                if (ConfigurationManager.AppSettings[
                    PLDBBatchConstants.CONFIG_ENABLE_POP_PLDB_POLES]
                    .ToUpper().Trim() == "T")
                {
                    PopulatePLDBPoles();
                }

                //Update the Lat, Long, Elevation
                if (ConfigurationManager.AppSettings[
                    PLDBBatchConstants.CONFIG_ENABLE_UPDATE_LOCATION]
                    .ToUpper().Trim() == "T")
                {
                    UpdateLocation();
                }

                //Decommission poles 
                if (ConfigurationManager.AppSettings[
                    PLDBBatchConstants.CONFIG_ENABLE_DECOMMISSION]
                    .ToUpper().Trim() == "T")
                {
                    Decommission();
                }

                //Update IDs (SapEquipId and GUID in PLDB)
                if (ConfigurationManager.AppSettings[
                    PLDBBatchConstants.CONFIG_ENABLE_UPDATE_IDS]
                    .ToUpper().Trim() == "T")
                {
                    UpdateIDs();
                }                 
                
                //Finished 
                Shared.WriteToLogfile("Finished Processing");
                Shared.WriteToLogfile("Leaving " + MethodBase.GetCurrentMethod()); 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
            finally
            {
                if (_pConn != null)
                {
                    if (_pConn.State == System.Data.ConnectionState.Open)
                        _pConn.Close();
                }
            }
        }
        //private void PopulateEdgisPoles()
        //{
        //    try
        //    {
        //        Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

        //        //Get the SQL to create the table 
        //        string sql = Shared.GetSQLFromSQLFile(
        //            PLDBBatchConstants.CREATE_EDGIS_POLES_TABLE_SQL);

        //        //First have to delete and rfecreate the table 
        //        DropTable(PLDBBatchConstants.EDGIS_POLES_TABLE);
        //        CreateTable(sql); 
                                
        //        //Connect to EDGIS and get all 
        //        //Have to connect using ESRI as need to convert the shape
        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
        //        IFeatureClass pPoleFC = pFWS.OpenFeatureClass(PLDBBatchConstants.SUPPORT_STRUCTURE_FC);
        //        IQueryFilter pQF = new QueryFilterClass();

        //        int poleCount = 0;
        //        int fileCounter = 1; 
        //        string guid = string.Empty;
        //        string replaceGuid = string.Empty; 
        //        string sapequipid = string.Empty;
        //        string pldbid = string.Empty;
        //        object theVal = null;
        //        double gisLat = 0;
        //        double gisLong = 0;

        //        Shared.InitializeSQLfile(fileCounter);
        //        pQF.WhereClause = ConfigurationManager.AppSettings.Get(
        //            PLDBBatchConstants.CONFIG_POLE_FILTER);
        //        pQF.SubFields = "globalid,sapequipid,pldbid,replaceguid,shape";
        //        int totalFeatureCount = pPoleFC.FeatureCount(pQF); 
        //        IFeatureCursor pFCursor = pPoleFC.Search(pQF, false);
        //        IFeature pFeature = pFCursor.NextFeature(); 
        //        int fldIdxguid = pFCursor.Fields.FindField("GLOBALID");
        //        int fldIdxrepguid = pFCursor.Fields.FindField("REPLACEGUID");
        //        int fldIdxEquipId = pFCursor.Fields.FindField("SAPEQUIPID");
        //        int fldIdxPLDBID = pFCursor.Fields.FindField("PLDBID");
                
        //        while (pFeature != null)
        //        {
        //            poleCount++;
        //            guid = string.Empty;
        //            replaceGuid = string.Empty;
        //            sapequipid = "NULL";
        //            pldbid = string.Empty;
        //            gisLat = 0;
        //            gisLong = 0; 

        //            theVal = pFeature.get_Value(fldIdxguid);
        //            if (theVal != DBNull.Value)
        //                guid = theVal.ToString();

        //            theVal = pFeature.get_Value(fldIdxPLDBID);
        //            if (theVal != DBNull.Value)
        //                pldbid = theVal.ToString();

        //            theVal = pFeature.get_Value(fldIdxEquipId);
        //            if (theVal != DBNull.Value)
        //                sapequipid = pFeature.get_Value(fldIdxEquipId).ToString();

        //            theVal = pFeature.get_Value(fldIdxrepguid);
        //            if (theVal != DBNull.Value)
        //                replaceGuid = pFeature.get_Value(fldIdxrepguid).ToString();

        //            PopulateLatLongForPole(
        //                    (IPoint)pFeature.ShapeCopy, ref gisLat, ref gisLong);

        //            WriteInsertToPolesFile(pldbid, guid, sapequipid, replaceGuid,
        //                gisLat, gisLong, fileCounter);

        //            if (poleCount % PLDBBatchConstants.INSERTS_PER_FILE == 0)
        //            {
        //                //launch the batch file against the previous batch 
        //                Shared.LaunchSQLBatchFile(fileCounter, false); 
        //                fileCounter++;
        //                Shared.InitializeSQLfile(fileCounter); 
        //            }                    

        //            if (poleCount % 5000 == 0)
        //                Shared.WriteToLogfile("Processed " + poleCount.ToString() +
        //                    " of: " + totalFeatureCount.ToString() + " poles");

        //            pFeature = pFCursor.NextFeature();
        //        }
                
        //        Marshal.FinalReleaseComObject(pFCursor);

        //        //Make sure to update any residual 
        //        if (poleCount % PLDBBatchConstants.INSERTS_PER_FILE != 0)
        //        {
        //            //There must be a residual - so send the SQL for 
        //            //the remainder 
        //            Shared.WriteToLogfile("Remainder exists");
        //            Shared.LaunchSQLBatchFile(fileCounter, false); 
        //        }

        //        bool poleCountIncreasing = true;
        //        long currentPoleCount = 0;
        //        long previousPoleCount = 0; 

        //        while (poleCountIncreasing)
        //        {
        //            currentPoleCount = GetTableRowCount( 
        //                PLDBBatchConstants.EDGIS_POLES_TABLE);
        //            if ((currentPoleCount > previousPoleCount) && 
        //                (currentPoleCount < poleCount)) 
        //            {
        //                poleCountIncreasing = true;
        //                previousPoleCount = currentPoleCount;
        //                Shared.WriteToLogfile(
        //                    "previousPoleCount: " + previousPoleCount.ToString() +
        //                    "currentPoleCount: " + previousPoleCount.ToString() +
        //                    "poleCountIncreasing: " + poleCountIncreasing.ToString()); 

        //                //Sleep
        //                System.Threading.Thread.Sleep(
        //                Convert.ToInt32(ConfigurationManager.
        //                AppSettings.Get(PLDBBatchConstants.CONFIG_SQL_SLEEP_INTERVAL)));
        //            }
        //            else
        //            {
        //                poleCountIncreasing = false;
        //            }
        //        }
                
        //        Shared.CleanupSQLfiles();
        //        Shared.NullOutBatchFile();

        //        //Verify all the poles made it to the SQL DB 
        //        if (currentPoleCount != poleCount)
        //        {
        //            Shared.WriteToLogfile("Warning: " + 
        //                "sqlDBPoleCount " + currentPoleCount.ToString() + " and " + 
        //                "poleCount: " + poleCount.ToString() + " do not match");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw new Exception("Error populating EDGIS poles"); 
        //    }
        //}
        /// <summary>
        /// Using bulk insert is MUCH faster and simpler 
        /// 
        /// </summary>
        private void PopulateEdgisPoles()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                
                //Get the SQL to create the table 
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.CREATE_EDGIS_POLES_TABLE_SQL);

                //First have to delete and rfecreate the table 
                DropTable(PLDBBatchConstants.EDGIS_POLES_TABLE);
                CreateTable(sql);

                //Connect to EDGIS and get all 
                //Have to connect using ESRI as need to convert the shape
                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureClass pPoleFC = pFWS.OpenFeatureClass(PLDBBatchConstants.SUPPORT_STRUCTURE_FC);
                IQueryFilter pQF = new QueryFilterClass();

                int poleCount = 0;
                int fileCounter = 1;
                string guid = string.Empty;
                string replaceGuid = string.Empty;
                string sapequipid = string.Empty;
                Int64 sapEquipIdLong = 0;
                string pldbid = string.Empty;
                object theVal = null;
                double gisLat = 0;
                double gisLong = 0;

                Shared.InitializeSQLfile(fileCounter);
                pQF.WhereClause = ConfigurationManager.AppSettings.Get(
                    PLDBBatchConstants.CONFIG_POLE_FILTER);
                pQF.SubFields = "globalid,sapequipid,pldbid,replaceguid,shape";
                int totalFeatureCount = pPoleFC.FeatureCount(pQF);
                IFeatureCursor pFCursor = pPoleFC.Search(pQF, false);
                IFeature pFeature = pFCursor.NextFeature();
                int fldIdxguid = pFCursor.Fields.FindField("GLOBALID");
                int fldIdxrepguid = pFCursor.Fields.FindField("REPLACEGUID");
                int fldIdxEquipId = pFCursor.Fields.FindField("SAPEQUIPID");
                int fldIdxPLDBID = pFCursor.Fields.FindField("PLDBID");
                List<Pole> pEdgisPoles = new List<Pole>(); 
                
                while (pFeature != null)
                {
                    poleCount++;
                    guid = string.Empty;
                    replaceGuid = string.Empty;
                    sapequipid = "NULL";
                    pldbid = string.Empty;
                    gisLat = 0;
                    gisLong = 0;
                    sapEquipIdLong = 0; 

                    theVal = pFeature.get_Value(fldIdxguid);
                    if (theVal != DBNull.Value)
                        guid = theVal.ToString();

                    theVal = pFeature.get_Value(fldIdxPLDBID);
                    if (theVal != DBNull.Value)
                        pldbid = theVal.ToString();

                    theVal = pFeature.get_Value(fldIdxEquipId);
                    if (theVal != DBNull.Value)
                    {
                        sapequipid = pFeature.get_Value(fldIdxEquipId).ToString();
                        Int64.TryParse(sapequipid, out sapEquipIdLong); 
                    }

                    theVal = pFeature.get_Value(fldIdxrepguid);
                    if (theVal != DBNull.Value)
                        replaceGuid = pFeature.get_Value(fldIdxrepguid).ToString();

                    PopulateLatLongForPole(
                            (IPoint)pFeature.ShapeCopy, ref gisLat, ref gisLong);

                    Pole thePole = new Pole( 
                        Convert.ToInt64(pldbid), 
                        guid,
                        sapEquipIdLong, 
                        replaceGuid, 
                        gisLat, 
                        gisLong); 
                    pEdgisPoles.Add(thePole);
                    
                    if (poleCount % PLDBBatchConstants.INSERTS_PER_FILE == 0)
                    {
                        BulkUploadToSql myData = BulkUploadToSql.Load(
                            pEdgisPoles,
                            PLDBBatchConstants.EDGIS_POLES_TABLE);
                        myData.Flush();
                        pEdgisPoles.Clear(); 
                    }

                    if (poleCount % 5000 == 0)
                        Shared.WriteToLogfile("Processed " + poleCount.ToString() +
                            " of: " + totalFeatureCount.ToString() + " poles");

                    pFeature = pFCursor.NextFeature();
                }

                Marshal.FinalReleaseComObject(pFCursor);

                //Make sure to update any residual 
                if (pEdgisPoles.Count != 0)
                {
                    //There must be a residual - so send the SQL for 
                    //the remainder 
                    Shared.WriteToLogfile("Remainder exists");
                    BulkUploadToSql myData = BulkUploadToSql.Load( 
                        pEdgisPoles,  
                        PLDBBatchConstants.EDGIS_POLES_TABLE);
                    myData.Flush();
                    pEdgisPoles.Clear();
                } 

                long currentPoleCount = GetTableRowCount(
                        PLDBBatchConstants.EDGIS_POLES_TABLE);

                //Verify all the poles made it to the SQL DB 
                if (currentPoleCount != poleCount)
                {
                    Shared.WriteToLogfile("Warning: " +
                        "sqlDBPoleCount " + currentPoleCount.ToString() + " and " +
                        "poleCount: " + poleCount.ToString() + " do not match");
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error populating EDGIS poles");
            }
        }

        private void CreateTable(string sqlCreateStatement)
        {
            SqlCommand pCmd = null;

            try
            {
                if (_pConn == null)
                    _pConn = new SqlConnection(ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants.
                        CONFIG_PLDB_CONNECTION].
                        ConnectionString);
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();
                pCmd = _pConn.CreateCommand();

                //Execute the command against the database 
                if (sqlCreateStatement != string.Empty)
                {
                    pCmd.CommandText = sqlCreateStatement;
                    object o = pCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }

            finally
            {
                //release database 
                pCmd = null;
            }
        }

        private void DropTable(string tableName)
        {
            SqlCommand pCmd = null;

            try
            {
                if (_pConn == null)
                    _pConn = new SqlConnection(ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants.
                        CONFIG_PLDB_CONNECTION].
                        ConnectionString);
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();
                pCmd = _pConn.CreateCommand();

                //Populate string to hold the SQL statement
                int recordCount = 0;
                string sqlStatement =
                    "DROP TABLE " + tableName;

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    pCmd.CommandText = sqlStatement;
                    object o = pCmd.ExecuteNonQuery();
                    recordCount = Convert.ToInt32(o);
                }

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
            }

            finally
            {
                //release database 
                pCmd = null;
            }
        }

        private void TruncateTable(string tableName)
        {
            SqlCommand pCmd = null;

            try
            {
                if (_pConn == null)
                    _pConn = new SqlConnection(ConfigurationManager.
                        ConnectionStrings[ 
                        PLDBBatchConstants.CONFIG_PLDB_CONNECTION].
                        ConnectionString);
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();
                pCmd = _pConn.CreateCommand();

                //Populate string to hold the SQL statement
                int recordCount = 0;
                string sqlStatement =
                    "TRUNCATE TABLE " + tableName;

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    pCmd.CommandText = sqlStatement;
                    object o = pCmd.ExecuteNonQuery();
                    recordCount = Convert.ToInt32(o);
                }

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }

            finally
            {
                //release database 
                pCmd = null;
            }
        }
               

        private void UpdateLocation()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Create table wth lat / long / elevation updates 
                CreateLatLongUpdateTable(); 

                //Execute stored proc to update the PLDB 
                ExecuteUpdateLocationStoredProc();
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error updating lat long");
            }
        }

        private void Decommission()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                List<DecommissionPoleInfo> pDecommPoleInfoList =
                    new System.Collections.Generic.List<DecommissionPoleInfo>();

                //Create the WQS84 spatial reference 
                ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironmentClass();
                IGeographicCoordinateSystem gcs = srFactory.CreateGeographicCoordinateSystem(
                    (int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
                ISpatialReference pWGS84SR = (ISpatialReference)gcs;
                string decommWhereClause = ConfigurationManager.
                    AppSettings.Get(PLDBBatchConstants.CONFIG_DECOMM_WHERECLAUSE);

                //Open the supportstructure featureclass 
                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureClass pPoleFC = pFWS.OpenFeatureClass(
                    PLDBBatchConstants.SUPPORT_STRUCTURE_FC);
                IQueryFilter pQF = new QueryFilterClass(); 
                                
                //Get the SQL that will identify the PLDBIDs that 
                //potentially should be decommissioned   
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.DECOMM_DISCREPANCY_QUERY_SQL);

                List<Pole> pDecommPolesList = GetPoles(sql);
                long pldbidOfCoincidentPole = 0;
                string guidOfCoincidentPole = string.Empty;
                bool poleExistsAtLocation = false;
                Hashtable hshPLDBIDs = new Hashtable();
                int updateCounter = 0;

                //Loop through the poles and build the decommissions list 
                Shared.WriteToLogfile("DECOMM_DISCREPANCY_QUERY identified: " + 
                    pDecommPolesList.Count.ToString() + " poles"); 
                foreach (Pole pPole in pDecommPolesList)
                {
                    updateCounter++; 
                    if (pPole.PLDBID != 0)
                    {
                        //Check for an existing edgis pole at that location 
                        poleExistsAtLocation = PoleExistsAtLocation(
                            pWGS84SR,
                            pPole.Latitude,
                            pPole.Longitude,
                            pPoleFC,
                            decommWhereClause,
                            ref pldbidOfCoincidentPole,
                            ref guidOfCoincidentPole);
                        if (poleExistsAtLocation)
                        {
                            //if (pPole.PLDBID != pldbidOfCoincidentPole)
                            //    Debug.Print("coincident pole has different pldbid");
                            //else
                            //    Debug.Print("coincident pole has same pldbid");

                            Shared.WriteToLogfile("Process will not decommission PLDBID: " +
                                pPole.PLDBID.ToString() + " because an edgis pole with guid: " + guidOfCoincidentPole +
                                "and PLDBID: " + pldbidOfCoincidentPole + " was found at that location!");
                        }
                        else
                        {
                            //There is no coincident edgis pole at this location so 
                            //we can decommission this pole 
                            if (!hshPLDBIDs.ContainsKey(pPole.PLDBID))
                            {
                                //Create a new DecommissionPoleInfo for each PLDBID
                                Shared.WriteToLogfile(
                                        "pldbid: " + pPole.PLDBID.ToString() +
                                        " has been tagged for decommissioning");
                                DecommissionPoleInfo decomInfo = new DecommissionPoleInfo();
                                decomInfo.PGE_NotificationNumber = null;
                                decomInfo.PGE_OrderNumber = null;
                                decomInfo.PLDBID = pPole.PLDBID.ToString();
                                pDecommPoleInfoList.Add(decomInfo);
                                hshPLDBIDs.Add(pPole.PLDBID, 0);
                            }
                        }
                    }

                    if (updateCounter % 100 == 0)
                        Shared.WriteToLogfile("Processed " + updateCounter.ToString() + " decommissions"); 
                }

                //Now call the service to decommission the pDecommPoleInfoList
                if (pDecommPoleInfoList.Count > 0)
                {
                    //Split this list onto an array of digestable size 
                    //because the database tends to kill the connection 
                    //if the process takes too long 
                    List<DecommissionPoleInfo>[] pSplitDecommList =
                        SplitDecommissionInfoListIntoArray(pDecommPoleInfoList, 100);
                    for (int i = 0; i < pSplitDecommList.Length - 1; i++)
                    {
                        try
                        {
                            Shared.WriteToLogfile("Calling DecommissionPoles on " +
                                pSplitDecommList[i].Count.ToString() + " PLDBIDs");
                            WcfService service = new WcfService();
                            service.DecommissionPoles(pSplitDecommList[i]);

                        }
                        catch (FaultException<ErrorDetail> ex)
                        {
                            //Write any errors to the logfile
                            Shared.WriteToLogfile("One or more PLDB error were encountered " +
                                "in decommissioning poles - writing these errors into the logfile...");
                            foreach (DecommissionPolesError e in ex.Detail.DecommissionPolesErrors)
                            {
                                Shared.WriteToLogfile("{e.PLDBID}\t{e.ErrorMessage}");
                            }
                        }
                    }
                }
                else
                {
                    Shared.WriteToLogfile("No poles were identified for decommissioning"); 
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error performing Decommission operation");
            }
        }

        private List<DecommissionPoleInfo>[] SplitDecommissionInfoListIntoArray( 
            List<DecommissionPoleInfo> pDecommPoleInfoList, 
            int arraySize)
        {
            try
            {
                if (pDecommPoleInfoList.Count == 0)
                    return null; 
                int counter = 0;
                int splitCount = 0; 

                //Figure out the size of our array                  
                foreach (DecommissionPoleInfo pDecommInfo in pDecommPoleInfoList)
                {
                    counter++;
                    if (counter % arraySize == 0)
                    {
                        splitCount++;
                    }
                }

                //Convert this to an array 
                List<DecommissionPoleInfo>[] pSplitDecommList = new List<DecommissionPoleInfo>[splitCount + 1]; 
                counter = 0;
                splitCount = 0;
                List<DecommissionPoleInfo> pSplitList = new List<DecommissionPoleInfo>();
                foreach (DecommissionPoleInfo pDecommInfo in pDecommPoleInfoList)
                {
                    counter++;
                    pSplitList.Add(pDecommInfo);

                    if (counter % arraySize == 0)
                    {
                        pSplitDecommList[splitCount] = pSplitList; 
                        splitCount++;
                        pSplitList = new List<DecommissionPoleInfo>(); 
                    }
                }

                if (pSplitList.Count != 0)
                {
                    pSplitDecommList[splitCount] = pSplitList;
                }

                //return array 
                return pSplitDecommList;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning array of DecommissionPoleInfo objects");
            }
        }
        
        public void Decommission_FIX_DUPs()
        {
            try
            {
                Shared.InitializeLogfile();
                Shared.InitializeSQLfile(0);
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");

                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Create the WQS84 spatial reference 
                ISpatialReferenceFactory srFactory = new SpatialReferenceEnvironmentClass();
                IGeographicCoordinateSystem gcs = srFactory.CreateGeographicCoordinateSystem(
                    (int)esriSRGeoCSType.esriSRGeoCS_WGS1984);
                ISpatialReference pWGS84SR = (ISpatialReference)gcs;
                string decommWhereClause = ConfigurationManager.
                    AppSettings.Get(PLDBBatchConstants.CONFIG_DECOMM_WHERECLAUSE);

                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureClass pPoleFC = pFWS.OpenFeatureClass(
                    PLDBBatchConstants.SUPPORT_STRUCTURE_FC);
                IQueryFilter pQF = new QueryFilterClass();

                Hashtable hshUnmatchedPoleGUIDs = GetUnMatchedPoleGUIDsFromTest("Data Source=TSETAPPDBSWC003;Initial Catalog=PLDB;User id=PLDB_I;Password=pldbTemp!;");

                //loop through each one 
                //1. try to do a direct match on guid in the PLDB 
                //2. if no match - look at that location in the PLDB 

                string[] guidsArray = GetArrayOfCommaSeparatedKeys(hshUnmatchedPoleGUIDs, 999, true);
                List<Pole> pAllUnmatchedPoles = new List<Pole>();

                for (int i = 0; i < guidsArray.Length; i++)
                {
                    List<Pole> thePoles = GetEDGISPoles(pPoleFC, "globalid IN(" + guidsArray[i] + ")");
                    foreach (Pole thePole in thePoles)
                    {
                        pAllUnmatchedPoles.Add(thePole);
                    }
                }

                //Get the SQL that will report the discrepancy  
                string sql = string.Empty;
                //List<Pole> pDecommPolesList = GetPoles(sql);

                long pldbidOfCoincidentPole = 0;
                string guidOfCoincidentPole = string.Empty;
                bool poleExistsAtLocation = false;
                Hashtable hshPLDBIDs = new Hashtable();
                int updateCounter = 0;
                long edgisMatchCount = 0; 
                int doesNotRequirePLDBID = 0; 
                double pldbid = 0;
                int resolvedCount = 0; 
                
                //Loop through the poles and build the decommissions list 
                foreach (Pole pPole in pAllUnmatchedPoles)
                {

                    updateCounter++;
                    Debug.Print("updateCounter: " + updateCounter.ToString());

                    if ((pPole.Subtype != 1) && (pPole.Subtype != 4) && (pPole.Subtype != 5)
                        && (pPole.Subtype != 8))
                    {
                        Debug.Print("does not require pldbid: " + pPole.Subtype.ToString());
                        doesNotRequirePLDBID++;
                        resolvedCount++; 
                        continue; 
                    }

                    //1.Try to find a pole in the PLDB based on the 
                    //GUID 
                    Pole pGuidMatchedPole = null; 
                    List<Pole> pProdPoles = GetProdPLDBPolesByGUID(pPole.GUID);
                    if (pProdPoles.Count == 1)
                    {
                        pGuidMatchedPole = pProdPoles.First();
                        pldbid = pGuidMatchedPole.PLDBID;
                        pQF = new QueryFilterClass();
                        pQF.WhereClause = "PLDBID = " + pldbid.ToString();
                        edgisMatchCount = pPoleFC.FeatureCount(pQF);
                        if (edgisMatchCount == 0)
                        {
                            Debug.Print("this one is resolved using guid");
                            resolvedCount++;
                            WriteSQLFilePLDBUpdate(pPole.GUID, pldbid.ToString());
                            continue;
                        }
                        else if (edgisMatchCount > 1)
                        {
                            Debug.Print("this should have been resolved already"); 
                        }                        
                    }
                    else if (pProdPoles.Count > 1)
                    {
                        Debug.Print("investigate");
                    }

                    if (pPole.ReplaceGUID != string.Empty)
                    {
                        pProdPoles = GetProdPLDBPolesByGUID(pPole.ReplaceGUID);
                        if (pProdPoles.Count == 1)
                        {
                            pGuidMatchedPole = pProdPoles.First();
                            pldbid = pGuidMatchedPole.PLDBID;
                            pQF = new QueryFilterClass();
                            pQF.WhereClause = "PLDBID = " + pldbid.ToString();
                            edgisMatchCount = pPoleFC.FeatureCount(pQF);
                            if (edgisMatchCount == 0)
                            {
                                Debug.Print("this one is resolved using replaceguid");
                                resolvedCount++;
                                WriteSQLFilePLDBUpdate(pPole.GUID, pldbid.ToString());
                                continue;
                            }
                            else if (edgisMatchCount > 1)
                            {
                                //resolvedCount++;
                                //WriteSQLFilePLDBUpdate(pPole.GUID, pldbid.ToString()); 
                                //continue; 
                            }
                        }
                        else if (pProdPoles.Count > 1)
                        {
                            Debug.Print("investigate");
                        }
                    }

                    //2. look for a pole in the PLDB at that location
                    //Check for an existing edgis pole at that location 
                    //List<Pole> pNearbyPoles = GetPLDBPoleAtLocation(pPole.Latitude, pPole.Longitude);
                    //if (pNearbyPoles.Count == 1)
                    //{
                    //    //There is one pole at this location 
                    //    Debug.Print("found a single pole at this location");
                    //    Pole pNearbyPole = pNearbyPoles.First();
                        
                    //    //Check for a duplicate 
                    //    //List<Pole> pDupPoles = GetPoles 

                    //}
                    //else if (pNearbyPoles.Count > 1)
                    //{
                    //    Debug.Print("multiple PLDBIDs at location!");
                    //}

                    if (updateCounter % 100 == 0)
                        Shared.WriteToLogfile("Processed " + updateCounter.ToString() + " decommissions");

                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error performing Decommission operation");
            }
        }

        private List<Pole> GetPLDBPoleAtLocation(double latitude, double longitude)
        {
            try
            {
                string sqlForPLDBPoleByPLDBID =
                    "SELECT b.PLDBID, b.PGE_GLOBALID, b.PGE_SAPEQUIPID, b.Latitude, b.Longitude, " +
                    "a.STATUSIDX, a.AnalysisID " +
                    "FROM dbo.STATUS a " +
                    "INNER JOIN " +
                    "(select x.Analysisid, x.Latitude, x.Longitude, x.PLDBID, x.Elevation," +
                    "x.Class, x.LenghtInInches, x.PGE_SAPEQUIPID, x.PGE_GLOBALID, x.BendingFactorOfSafety, " +
                    "x.PoleFactorOfSafety, x.Species, x.PGE_SnowLoadDistrict, " +
                    "y.status_date FROM dbo.OCalcProAnalysis x " +
                    "LEFT JOIN " +
                    "(select max(dbo.STATUS.[STATUS_SET_AT]) as status_date, dbo.OCalcProAnalysis.PLDBID " +
                    "from dbo.OCalcProAnalysis, dbo.STATUS WHERE " +
                    "dbo.OCalcProAnalysis.PLDBID = dbo.STATUS.PLDBID " +
                    "group by dbo.OCalcProAnalysis.PLDBID) y " +
                    "ON x.PLDBID = y.PLDBID) b " +
                    "ON a.AnalysisID = b.AnalysisID " +
                    "WHERE a.STATUS_SET_AT = b.status_date And " +
                    "(ABS(" + latitude.ToString() + " - b.Latitude) < 0.000001 AND " + 
                    "ABS(" + longitude.ToString() + " - b.Longitude) > 0.000001)";
                return GetPoles(sqlForPLDBPoleByPLDBID);

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error performing Decommission operation");
            }
        }


        private void UpdateIDs()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());                                                 
                List<SAPEQUIPInfo> pSapEquipInfoList =
                    new List<SAPEQUIPInfo>(); 

                //Run the query to find the moved poles 
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[
                            PLDBBatchConstants.CONFIG_PLDB_CONNECTION].
                            ConnectionString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();
                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureClass pPoleFC = pFWS.OpenFeatureClass(
                    PLDBBatchConstants.SUPPORT_STRUCTURE_FC);
                IQueryFilter pQF = new QueryFilterClass();

                //Get the SQL that will report the discrepancy  
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.ID_DISCREPANCY_QUERY_SQL);

                //Run the SQL select statement 
                pCmd.CommandText = sql;
                SqlDataReader reader = pCmd.ExecuteReader();

                int updateCounter = 0;
                string edgis_globalid = string.Empty;
                string edgis_sapEquipId = string.Empty;
                long pldbId = 0;
                object o = null;
                Hashtable hshPLDBIDs = new Hashtable();

                while (reader.Read())
                {
                    updateCounter++;
                    pldbId = 0;
                    edgis_globalid = string.Empty;
                    edgis_sapEquipId = string.Empty; 

                    o = reader.GetValue(reader.GetOrdinal("PLDBID"));
                    if (o != DBNull.Value)
                        pldbId = Convert.ToInt64(o);

                    o = reader.GetValue(reader.GetOrdinal("EDGIS_SAPEQUIPID"));
                    if (o != DBNull.Value)
                        edgis_sapEquipId = o.ToString();

                    o = reader.GetValue(reader.GetOrdinal("EDGIS_GUID"));
                    if (o != DBNull.Value)
                        edgis_globalid = o.ToString(); 

                    if (pldbId != 0)
                    {
                        if (!hshPLDBIDs.ContainsKey(pldbId))
                        {
                            //Create a new SAPEQUIPInfo for each PLDBID ID update 
                            Shared.WriteToLogfile(
                                    "pldbid: " + pldbId.ToString() +
                                    " has been identified as requiring ID updates");
                            SAPEQUIPInfo sapEquipInfo = new SAPEQUIPInfo();
                            Guid pGuid = new Guid(edgis_globalid.ToUpper());
                            sapEquipInfo.PGE_GLOBALID = pGuid;
                            sapEquipInfo.PGE_SAPEQUIPID = edgis_sapEquipId;
                            sapEquipInfo.PLDBID = pldbId.ToString();
                            pSapEquipInfoList.Add(sapEquipInfo);
                            hshPLDBIDs.Add(sapEquipInfo, 0);
                        }                    
                    }

                    if (updateCounter % 100 == 0)
                        Shared.WriteToLogfile("Processed " + updateCounter.ToString() + " UpdateID Records");
                }
                reader.Close();
                pCmd = null;
                Shared.WriteToLogfile("Identified: " + updateCounter.ToString() + " ID discrepancies");

                //==================================================
                //First step: set the SapEquipIds to 0
                //================================================== 

                WcfService service = new WcfService();
                AnalysisService analysisService = new AnalysisService();
                STATUS currentStatus = null;
                PGE_Custom_Data_Block dataBlock = null;
                Shared.WriteToLogfile("Setting the SapEquipIds to 0 first"); 
                long counter = 0; 
                List<SAPEQUIPInfo> itemsReset = new List<SAPEQUIPInfo>();
                Shared.WriteToLogfile("Identified: " + 
                    pSapEquipInfoList.Count.ToString() + 
                    " PLDBIDs to set to zero");
                foreach (var item in pSapEquipInfoList)
                {
                    counter++; 
                    try
                    {
                        pldbId = Convert.ToInt64(item.PLDBID);                        
                        currentStatus = service.GetCurrentStatus(pldbId);
                        if (currentStatus.AnalysisID != Guid.Empty)
                        {
                            dataBlock = service.GetCurrentAnalysisSummary(pldbId); 
                            string pplxData = analysisService.GetAnalysis(currentStatus.AnalysisID ?? Guid.Empty);

                            // Zero out the equipment ID on the analysis to be saved
                            dataBlock.PGE_SAPEQUIPID = 0;

                            // Save a new analysis with the same status as the current one
                            service.StoreAnalysisWithComment(pldbId, currentStatus.STATUSIDX, dataBlock, pplxData, "Automated Process to Reset the SAPEQUIPID");

                            // Keep track of the ones that were successful
                            itemsReset.Add(item);
                        }
                    }
                    catch (FaultException<ErrorDetail> ex)
                    {
                        Shared.WriteToLogfile("Service error occurred while resetting SAPEQUIPID for PLDBID {item.PLDBID}:\n{ex.Detail.ClientUserMessage}\n{ex.Detail.AdditionalInfo}");
                    }
                    catch (Exception ex)
                    {
                        Shared.WriteToLogfile("Unexpected error occurred while resetting SAPEQUIPID for PLDBID {item.PLDBID}:\n{ex.Message}");
                    }
                    if (counter % 100 == 0)
                        Shared.WriteToLogfile("Processed " + counter.ToString() + " set to zero");

                }

                //==================================================
                //Second step: Reset the SAPEQUIPID's on records that 
                //were zeroed-out successfully
                //================================================== 

                Shared.WriteToLogfile("Now updating records for SapEquipIds that were zeroed-out successfully");
                Shared.WriteToLogfile("itemsReset.Count " + itemsReset.Count.ToString());

                if (itemsReset.Count > 0)
                {
                    //Split this list onto an array of digestable size 
                    //because the database tends to kill the connection 
                    //if the process takes too long 
                    List<SAPEQUIPInfo>[] pSplitUpdateIDList =
                        SplitSapEquipInfoListIntoArray(itemsReset, 500);
                    for (int i = 0; i < pSplitUpdateIDList.Length - 1; i++)
                    {
                        try
                        {
                            Shared.WriteToLogfile("Calling SetSAPEQUIPID on " +
                                pSplitUpdateIDList[i].Count.ToString() + " PLDBIDs");
                            service.SetSAPEQUIPID(pSplitUpdateIDList[i]);
                        }
                        catch (FaultException<ErrorDetail> ex)
                        {
                            Shared.WriteToLogfile("SETSAPEQUIPID Error information received:");
                            Shared.WriteToLogfile("PLDBID\tSAPEQUIPID\tGLOBALID\tErrorMessage");

                            // The LocationUpdateErrors property has one specific use, 
                            // to hold error information from UpdateLocations method
                            foreach (SetSAPEQUIPIDError e in ex.Detail.SetSAPEQUIPIDErrors)
                            {
                                Shared.WriteToLogfile("{e.PLDBID}\t{e.PGE_SAPEQUIPID}\t{e.PGE_GLOBALID}\t{e.ErrorMessage}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Shared.WriteToLogfile("Unexpected error occurred during SETSAPEQUIPID process:\n{ex.Message}");
                        }
                    }
                }
                else
                {
                    Shared.WriteToLogfile("No PLDBIDs were identified for ID updates");
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error performing Update ID operation");
            }
        }

        private List<SAPEQUIPInfo>[] SplitSapEquipInfoListIntoArray(
            List<SAPEQUIPInfo> pUpdateIDList,
            int arraySize)
        {
            try
            {
                if (pUpdateIDList.Count == 0)
                    return null;
                int counter = 0;
                int splitCount = 0;

                //Figure out the size of our array                  
                foreach (SAPEQUIPInfo pSapEquipInfo in pUpdateIDList)
                {
                    counter++;
                    if (counter % arraySize == 0)
                    {
                        splitCount++;
                    }
                }

                //Convert this to an array 
                List<SAPEQUIPInfo>[] pSplitUpdateIDList = new List<SAPEQUIPInfo>[splitCount + 1];
                counter = 0;
                splitCount = 0;
                List<SAPEQUIPInfo> pSplitList = new List<SAPEQUIPInfo>();
                foreach (SAPEQUIPInfo pSapEquipInfo in pUpdateIDList)
                {
                    counter++;
                    pSplitList.Add(pSapEquipInfo);

                    if (counter % arraySize == 0)
                    {
                        pSplitUpdateIDList[splitCount] = pSplitList;
                        splitCount++;
                        pSplitList = new List<SAPEQUIPInfo>();
                    }
                }

                if (pSplitList.Count != 0)
                {
                    pSplitUpdateIDList[splitCount] = pSplitList;
                }

                //return array 
                return pSplitUpdateIDList;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error splitting updateInfo objects");
            }
        }

        /// <summary>
        /// Routine to check if an EDGIS pole exists at the 
        /// location of the PLDBID. If so do not Decommission the 
        /// Pole as there is still a pole in the ground at this spot  
        /// </summary>
        /// <param name="pFC"></param>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        private bool PoleExistsAtLocation(
            ISpatialReference pWGS84, 
            double pldbPoleLat, 
            double pldbPoleLong, 
            IFeatureClass pPoleFC, 
            string poleWhereClause, 
            ref long pldbidOfCoincidentPole, 
            ref string guidOfCoincidentPole)
        {
            try
            {
                bool poleExists = false;
                IGeoDataset pGDS = (IGeoDataset)pPoleFC;
                double edgisLat = 0;
                double edgisLong = 0;
                pldbidOfCoincidentPole = 0;
                guidOfCoincidentPole = string.Empty; 
                long counter = 0; 

                //Get the point at that lat long and project 
                //it to the lat / long of the Pole featureclass 
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(pldbPoleLong, pldbPoleLat);
                pPoint.SpatialReference = pWGS84;
                //pPoint.Project(pGDS.SpatialReference);
                
                //Buffer by 1 foot 
                ITopologicalOperator pTopo = (ITopologicalOperator)pPoint;
                IPolygon pPolygon = (IPolygon)pTopo.Buffer(0.00001);

                //Search for Poles within the buffer 
                ISpatialFilter pSF = new SpatialFilterClass();
                pSF.WhereClause = poleWhereClause;
                pSF.Geometry = pPolygon;
                pSF.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects; 

                IFeatureCursor pFCursor = pPoleFC.Search(pSF, false);
                IFeature pFeature = pFCursor.NextFeature(); 

                while (pFeature != null)
                {
                    PopulateLatLongForPole( 
                        (IPoint)pFeature.ShapeCopy, ref edgisLat, ref edgisLong);
                    if (((Math.Abs(edgisLat - pldbPoleLat)) < 0.000001) &&
                        ((Math.Abs(edgisLong - pldbPoleLong)) < 0.000001))
                    {
                        poleExists = true; 
                        if (pFeature.get_Value(pFeature.Fields.FindField("pldbid")) != DBNull.Value)
                            pldbidOfCoincidentPole = Convert.ToInt64(pFeature.get_Value(pFeature.Fields.FindField("pldbid")));
                        if (pFeature.get_Value(pFeature.Fields.FindField("globalid")) != DBNull.Value)
                            guidOfCoincidentPole = pFeature.get_Value(pFeature.Fields.FindField("globalid")).ToString();
                        counter++; 
                    }                    

                    pFeature = pFCursor.NextFeature(); 
                }
                Marshal.FinalReleaseComObject(pFCursor);

                if (counter > 1)
                {
                    Shared.WriteToLogfile("Warning: multiple coincident poles in edgis for pldid: " + 
                        pldbidOfCoincidentPole.ToString() + " and GUID: " + guidOfCoincidentPole);
                }

                return poleExists; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                return false; 
            }
        }

        //private void CreateLatLongUpdateTable()
        //{
        //    bool mapDocOpen = false;
        //    IMapDocument pMapDoc = null;

        //    try
        //    {
        //        //Setup the SQL file 
        //        Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
        //        int fileCounter = 1;
        //        Shared.InitializeSQLfile(fileCounter);

        //        //Get the SQL to create the table 
        //        string sql = Shared.GetSQLFromSQLFile(
        //            PLDBBatchConstants.CREATE_LAT_LONG_UPDATES_TABLE_SQL);

        //        //First have to delete and recreate the table 
        //        DropTable(PLDBBatchConstants.LAT_LONG_UPDATES_TABLE);
        //        CreateTable(sql); 

        //        //Run the query to find the moved poles 
        //        if (_pConn == null)
        //        {
        //            //Get output DB connection 
        //            _pConn = new SqlConnection(
        //                ConfigurationManager.
        //                ConnectionStrings[
        //                    PLDBBatchConstants.
        //                    CONFIG_PLDB_CONNECTION].
        //                    ConnectionString);
        //        }
        //        if (_pConn.State != System.Data.ConnectionState.Open)
        //            _pConn.Open();

        //        //Setup the command 
        //        SqlCommand pCmd = _pConn.CreateCommand();

        //        //Return the SQL from the file in the install 
        //        //directory 
        //        sql = Shared.GetSQLFromSQLFile(
        //            PLDBBatchConstants.LAT_LONG_DISCREPANCY_QUERY_SQL);

        //        //Open the map document  for the purposes of pulling the elevation                 
        //        Shared.WriteToLogfile("Opening map document");
        //        string mapDocPath = Shared.GetMapDocumentFullFilename();
        //        pMapDoc = new MapDocumentClass();
        //        pMapDoc.Open(mapDocPath);
        //        mapDocOpen = true;

        //        //Get the DEM layer for determining pole heights 
        //        List<ILayer> pLayers = GetLayersFromMap(pMapDoc.Map[0], 
        //            PLDBBatchConstants.HEIGHTS_LAYER_NAME);
        //        IRasterLayer pRasterLayer = (IRasterLayer)pLayers[0];
        //        IIdentify pIdentify = (IIdentify)pRasterLayer;

        //        IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
        //        IFeatureClass pPoleFC = pFWS.OpenFeatureClass(PLDBBatchConstants.SUPPORT_STRUCTURE_FC);

        //        //Run the SQL select statement 
        //        pCmd.CommandText = sql;
        //        SqlDataReader reader = pCmd.ExecuteReader();

        //        int updateCounter = 0;
        //        string analysisId = string.Empty;
        //        string sapIdString = string.Empty;
        //        string edgis_guid = string.Empty;
        //        double elevation = 0; 
        //        double edgis_lat = 0;
        //        double edgis_long = 0;
        //        long pldbId = 0;
        //        object o = null;

        //        while (reader.Read())
        //        {
        //            updateCounter++;
        //            elevation = 0;  
        //            edgis_lat = 0;
        //            edgis_long = 0;
        //            pldbId = 0;
        //            edgis_guid = string.Empty;

        //            o = reader.GetValue(reader.GetOrdinal("edgis_guid"));
        //            if (o != DBNull.Value)
        //                edgis_guid = o.ToString().ToUpper();

        //            o = reader.GetValue(reader.GetOrdinal("edgis_lat"));
        //            if (o != DBNull.Value)
        //                edgis_lat = Convert.ToDouble(o);

        //            o = reader.GetValue(reader.GetOrdinal("edgis_long"));
        //            if (o != DBNull.Value)
        //                edgis_long = Convert.ToDouble(o);

        //            o = reader.GetValue(reader.GetOrdinal("pldbid"));
        //            if (o != DBNull.Value)
        //                pldbId = Convert.ToInt64(o);
                    
        //            //Get the elevation at this location 
        //            elevation = GetElevation(pPoleFC, edgis_guid, pIdentify);

        //            WriteInsertToLatLongUpdatesFile(
        //                pldbId.ToString(),
        //                edgis_guid,
        //                elevation,
        //                edgis_lat,
        //                edgis_long, 
        //                fileCounter);

        //            if (updateCounter % PLDBBatchConstants.INSERTS_PER_FILE == 0)
        //            {
        //                //launch the batch file against the previous batch 
        //                Shared.LaunchSQLBatchFile(fileCounter, false);
        //                fileCounter++;
        //                Shared.InitializeSQLfile(fileCounter);
        //            }                    

        //            if (updateCounter % 100 == 0)
        //                Shared.WriteToLogfile("Processed " + updateCounter.ToString() +
        //                    " latitude/longitude/elevation updates");
        //        }

        //        reader.Close();
        //        pCmd = null;

        //        //Make sure to update any residual 
        //        if (updateCounter % PLDBBatchConstants.INSERTS_PER_FILE != 0)
        //        {
        //            //There must be a residual - so send the SQL for 
        //            //the remainder 
        //            Shared.LaunchSQLBatchFile(fileCounter, false);
        //        }

        //        bool poleCountIncreasing = true;
        //        long currentPoleCount = 0;
        //        long previousPoleCount = 0;

        //        while (poleCountIncreasing)
        //        {
        //            currentPoleCount = GetTableRowCount(
        //                PLDBBatchConstants.LAT_LONG_UPDATES_TABLE);
        //            if ((currentPoleCount > previousPoleCount) &&
        //                (currentPoleCount < updateCounter))
        //            {
        //                poleCountIncreasing = true;
        //                previousPoleCount = currentPoleCount;
        //                Shared.WriteToLogfile(
        //                    "previousPoleCount: " + previousPoleCount.ToString() +
        //                    "currentPoleCount: " + previousPoleCount.ToString() +
        //                    "poleCountIncreasing: " + poleCountIncreasing.ToString());

        //                //Sleep
        //                System.Threading.Thread.Sleep(
        //                Convert.ToInt32(ConfigurationManager.
        //                AppSettings.Get(PLDBBatchConstants.CONFIG_SQL_SLEEP_INTERVAL)));
        //            }
        //            else
        //            {
        //                poleCountIncreasing = false;
        //            }
        //        }

        //        Shared.CleanupSQLfiles();
        //        Shared.NullOutBatchFile();

        //        //Verify all the poles made it to the SQL DB 
        //        if (currentPoleCount != updateCounter)
        //        {
        //            Shared.WriteToLogfile("Warning: " +
        //                "sqlDBPoleCount " + currentPoleCount.ToString() + " and " +
        //                "poleCount: " + updateCounter.ToString() + " do not match");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw new Exception("Error populating EDGIS poles");
        //    }
        //    finally
        //    {
        //        if (mapDocOpen)
        //        {
        //            pMapDoc.Close();
        //            Marshal.FinalReleaseComObject(pMapDoc); 
        //        }
        //    }
        //}

        private void CreateLatLongUpdateTable()
        {
            bool mapDocOpen = false;
            IMapDocument pMapDoc = null;

            try
            {
                //Setup the SQL file 
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                //int fileCounter = 1;
                //Shared.InitializeSQLfile(fileCounter);

                //Get the SQL to create the table 
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.CREATE_LAT_LONG_UPDATES_TABLE_SQL);

                //First have to delete and recreate the table 
                DropTable(PLDBBatchConstants.LAT_LONG_UPDATES_TABLE);
                CreateTable(sql);

                //Run the query to find the moved poles 
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[
                            PLDBBatchConstants.
                            CONFIG_PLDB_CONNECTION].
                            ConnectionString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = _pConn.CreateCommand();

                //Return the SQL from the file in the install 
                //directory 
                sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.LAT_LONG_DISCREPANCY_QUERY_SQL);

                //Open the map document  for the purposes of pulling the elevation                 
                Shared.WriteToLogfile("Opening map document");
                string mapDocPath = Shared.GetMapDocumentFullFilename();
                pMapDoc = new MapDocumentClass();
                pMapDoc.Open(mapDocPath);
                mapDocOpen = true;

                //Get the DEM layer for determining pole heights 
                List<ILayer> pLayers = GetLayersFromMap(pMapDoc.Map[0],
                    PLDBBatchConstants.HEIGHTS_LAYER_NAME);
                IRasterLayer pRasterLayer = (IRasterLayer)pLayers[0];
                IIdentify pIdentify = (IIdentify)pRasterLayer;

                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureClass pPoleFC = pFWS.OpenFeatureClass(PLDBBatchConstants.SUPPORT_STRUCTURE_FC);

                //Run the SQL select statement 
                pCmd.CommandText = sql;
                SqlDataReader reader = pCmd.ExecuteReader();

                int updateCounter = 0;
                string analysisId = string.Empty;
                string sapIdString = string.Empty;
                string edgis_guid = string.Empty;
                double elevation = 0;
                double edgis_lat = 0;
                double edgis_long = 0;
                long pldbId = 0;
                object o = null;
                List<Pole> pEdgisPoles = new List<Pole>(); 

                while (reader.Read())
                {
                    updateCounter++;
                    elevation = 0;
                    edgis_lat = 0;
                    edgis_long = 0;
                    pldbId = 0;
                    edgis_guid = string.Empty;

                    o = reader.GetValue(reader.GetOrdinal("edgis_guid"));
                    if (o != DBNull.Value)
                        edgis_guid = o.ToString().ToUpper();

                    o = reader.GetValue(reader.GetOrdinal("edgis_lat"));
                    if (o != DBNull.Value)
                        edgis_lat = Convert.ToDouble(o);

                    o = reader.GetValue(reader.GetOrdinal("edgis_long"));
                    if (o != DBNull.Value)
                        edgis_long = Convert.ToDouble(o);

                    o = reader.GetValue(reader.GetOrdinal("pldbid"));
                    if (o != DBNull.Value)
                        pldbId = Convert.ToInt64(o);

                    //Get the elevation at this location 
                    elevation = GetElevation(pPoleFC, edgis_guid, pIdentify);

                    Pole thePole = new Pole(
                        pldbId,
                        edgis_guid,
                        edgis_lat,
                        edgis_long,
                        elevation);
                    pEdgisPoles.Add(thePole);

                    if (updateCounter % PLDBBatchConstants.INSERTS_PER_FILE == 0)
                    {
                        BulkUploadToSql myData = BulkUploadToSql.Load( 
                            pEdgisPoles, 
                            PLDBBatchConstants.LAT_LONG_UPDATES_TABLE);
                        myData.Flush();
                        pEdgisPoles.Clear();
                    }

                    if (updateCounter % 100 == 0)
                        Shared.WriteToLogfile("Processed " + updateCounter.ToString() +
                            " latitude/longitude/elevation updates");
                }

                reader.Close();
                pCmd = null;

                //Make sure to update any residual 
                if (pEdgisPoles.Count != 0)
                {
                    //There must be a residual - so send the SQL for 
                    //the remainder 
                    Shared.WriteToLogfile("Remainder exists");
                    BulkUploadToSql myData = BulkUploadToSql.Load(
                        pEdgisPoles,
                        PLDBBatchConstants.LAT_LONG_UPDATES_TABLE);
                    myData.Flush();
                    pEdgisPoles.Clear();
                }
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error populating EDGIS poles");
            }
            finally
            {
                if (mapDocOpen)
                {
                    pMapDoc.Close();
                    Marshal.FinalReleaseComObject(pMapDoc);
                }
            }
        }

        private void ExecuteUpdateLocationStoredProc()
        {
            SqlDataReader reader = null;

            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Run the query to find the moved poles 
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[
                            PLDBBatchConstants.
                            CONFIG_PLDB_CONNECTION].
                            ConnectionString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                SqlCommand pCmd = new SqlCommand( 
                    PLDBBatchConstants.UPDATE_LAT_LONG_STORED_PROC, _pConn);
                pCmd.CommandTimeout = 300; 
                pCmd.CommandType = System.Data.CommandType.StoredProcedure;
                pCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error populating EDGIS poles");
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private List<ILayer> GetLayersFromMap(IMap map, string featureClassName)
        {
            List<ILayer> layers = new List<ILayer>();

            //Get all of the IFeatureLayers
            //string shortFCName = GetShortDatasetName(featureClassName).ToUpper();
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
            IEnumLayer enumLayer = map.Layers;
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {

                if (layer.Name.ToLower() == featureClassName.ToLower())
                {
                    layers.Add(layer);
                }

                layer = enumLayer.Next();
            }
            return layers;
        }


        private double GetElevation(IFeatureClass pPoleFC, string guid, IIdentify pIdentify)
        {
            try
            {
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = "GLOBALID = " + "'{" + guid + "}'";
                double elevation = -1;
                IFeatureCursor pFCursor = pPoleFC.Search(pQF, false);
                IFeature pFeature = pFCursor.NextFeature(); 
                IPoint pPoleLocation = null;
                if (pFeature != null)
                    pPoleLocation = (IPoint)pFeature.ShapeCopy;
                Marshal.FinalReleaseComObject(pFCursor);
                if (pPoleLocation != null)
                {
                    elevation = GetElevationAtPoint(pPoleLocation, pIdentify);
                }
                else
                {
                    Shared.WriteToLogfile("Unable to find guid: " + guid.ToString()); 
                }
                return elevation; 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning elevation at point");
            }
        }

        private double GetElevationAtPoint(IPoint pPoint, IIdentify pIdentify)
        {
            try
            {
                double rasterValue = -1;

                //Get RasterIdentifyObject on that point 
                IArray pIDArray = pIdentify.Identify(pPoint);
                if (pIDArray != null)
                {
                    IRasterIdentifyObj pRIDObj = (IRasterIdentifyObj)pIDArray.get_Element(0);
                    rasterValue = Convert.ToDouble(pRIDObj.Name);
                }
                return Math.Round(rasterValue, 1);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error returning elevation at point");
            }
        }

        public void FixDUP_PLDBIDs()
        {
            try
            {
                Shared.InitializeLogfile();
                Shared.InitializeSQLfile(0); 
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");

                //Get a reader for all SYNC_EDGIS_POLES which 
                //have a count(*) > 1 (DUP PLDBIDs) 
                //Run the query to find the DUP PLDBIDs 

                Hashtable hshPLDBID_DUPs = GetPLDBID_DUPs();
                int counter = 0;
                string sql = string.Empty;
                int otherPoleUpdateCount = 0; 
                List<Pole> pEDGISPoles = null;
                List<Pole> pPLDBPoles = null;
                List<Pole> pPLDBPolesViaGlobalId = null;
                List<Pole> pPLDBPolesViaSapEquipID = null;
                List<Pole> pPLDBPolesViaReplaceGUID = null;
                List<Pole> pNearbyPLDBPoles = null;
                List<Pole> pDupEDGISPoles = null;
                string pldbidForGUID = "NULL";
                int matchCount = 0;
                int multiMatchCount = 0;
                string currentGUIDForPLDBID = string.Empty;
                int currnetMatchRating = 0; 

                
                foreach (long pldbid in hshPLDBID_DUPs.Keys)
                {
                    counter++; 
                    Shared.WriteToLogfile("Processing ref pldbid: " + pldbid.ToString() + 
                        " number: " + counter.ToString());

                    //Get the EDGIS Poles from SYNC_EDGIS_POLES 
                    //Lat, Long, PLDBID, SapEquipId, GUID, replaceguid  
                    //Based on the PLDBID (there will be > 1)
                    sql = "select * from dbo.sync_edgis_poles where pldbid = " + pldbid.ToString();
                    pEDGISPoles = GetPoles(sql);
                    if (pEDGISPoles.Count < 2)
                        Debug.Print("this scenario should not arise by definition");
                    
                    //Get the PLDB Pole for the ref PLDBID  
                    sql = "select * from dbo.sync_pldb_poles where pldbid = " + pldbid.ToString();
                    pPLDBPoles = GetPoles(sql);
                    if (pEDGISPoles.Count != 1)
                        Debug.Print("this is the result of a problem with my query jus grab first one!");
                    if (pPLDBPoles.Count == 0)
                        Debug.Print("this is a problem");
                    Pole pPLDBPole = pPLDBPoles.First(); 
                    
                    //For each EDGIS Poles with this PLDBID 
                    //correctPoleGUIDForPLDBID = string.Empty;
                    matchCount = 0;
                    currentGUIDForPLDBID = string.Empty;
                    currnetMatchRating = 0;

                    foreach (Pole pEDGISPole in pEDGISPoles)
                    {
                        pldbidForGUID = "NULL"; 

                        //look for match on sapequipid so this is probably correct 
                            if ((pEDGISPole.SapEquipID == pPLDBPole.SapEquipID) &&
                                (pEDGISPole.SapEquipID != 0))
                                pldbidForGUID = pldbid.ToString(); 


                        //look for a match on globalid 

                            if (pEDGISPole.GUID == pPLDBPole.GUID)
                            {
                                pldbidForGUID = pldbid.ToString();
                                currentGUIDForPLDBID = pEDGISPole.GUID;
                                currnetMatchRating++; 
                            }


                        //look for a match on replaceguid   

                            if (pEDGISPole.ReplaceGUID == pPLDBPole.GUID)
                            {
                                pldbidForGUID = pldbid.ToString();
                                currentGUIDForPLDBID = pEDGISPole.GUID;
                                currnetMatchRating++;
                            }


                        //check if the reference pole is in the vicinity

                            if ((Math.Abs(pEDGISPole.Latitude - pPLDBPole.Latitude) < 0.000001) &&
                            (Math.Abs(pEDGISPole.Longitude - pPLDBPole.Longitude) < 0.000001))
                            {
                                pldbidForGUID = pldbid.ToString();
                                currentGUIDForPLDBID = pEDGISPole.GUID;
                                currnetMatchRating++;
                            }


                        //try looking for all poles in the vicinity 
                        if (pldbidForGUID == "NULL")
                        {
                            //Find poles in the vicinity (with different pldbids) 
                            sql = "select * from dbo.sync_pldb_poles where " +
                                "ABS(latitude - " + pEDGISPole.Latitude.ToString() + ") < 0.000001 And " +
                                "ABS(longitude - " + pEDGISPole.Longitude.ToString() + ") < 0.000001";
                            pNearbyPLDBPoles = GetPoles(sql);
                            if (pNearbyPLDBPoles.Count == 0)
                            {
                                //for this guid there are no nearby poles so we 
                                //have to set the PLDBID to null for this GUID 
                                
                                //look for another pldb pole 
 
                                //try GUID match 
                                sql = "select * from dbo.sync_pldb_poles where pge_globalid = " + "'" +
                                    pEDGISPole.GUID + "'";
                                pPLDBPolesViaGlobalId = GetPoles(sql);
                                if (pPLDBPolesViaGlobalId.Count == 1)
                                {
                                    pldbidForGUID = pPLDBPolesViaGlobalId.First().PLDBID.ToString(); 
                                }

                                //try SapEquipId match 
                                if (pldbidForGUID == "NULL")
                                {
                                    if (pEDGISPole.SapEquipID != 0)
                                    {
                                        sql = "select * from dbo.sync_pldb_poles where pge_sapequipid = " +
                                        pEDGISPole.SapEquipID;
                                        pPLDBPolesViaSapEquipID = GetPoles(sql);
                                        if (pPLDBPolesViaSapEquipID.Count == 1)
                                        {
                                            pldbidForGUID = pPLDBPolesViaSapEquipID.First().PLDBID.ToString();
                                        }
                                    }
                                }

                                if (pldbidForGUID == "NULL")
                                {
                                    if (pEDGISPole.ReplaceGUID != string.Empty)
                                    {
                                        sql = "select * from dbo.sync_pldb_poles where pge_globalid = '" +
                                        pEDGISPole.ReplaceGUID + "'";
                                        pPLDBPolesViaReplaceGUID = GetPoles(sql);
                                        if (pPLDBPolesViaReplaceGUID.Count == 1)
                                        {
                                            pldbidForGUID = pPLDBPolesViaReplaceGUID.First().PLDBID.ToString();
                                        }
                                    }
                                }                                
                            }
                            else if (pNearbyPLDBPoles.Count == 1)
                            {
                                //Only one nearby pole in the vicinity 
                                //So this is most likey the correct PLDBID
                                pldbidForGUID = pNearbyPLDBPoles.First().PLDBID.ToString();                                                                
                            }
                            else if (pNearbyPLDBPoles.Count > 1)
                            {
                                //Multiple nearby PLDB Poles
                                Debug.Print("multiple nearby poles");
                                foreach (Pole pNearbyPLDBPole in pNearbyPLDBPoles)
                                {
                                    //look for match on sapequipid so this is probably correct
                                    if (pEDGISPole.GUID == pNearbyPLDBPole.GUID)
                                    {
                                        pldbidForGUID = pNearbyPLDBPole.PLDBID.ToString();
                                        break; 
                                    }

                                    //look for a match on globalid 
                                    if (pldbidForGUID == "NULL")
                                    {
                                        if (pEDGISPole.GUID == pNearbyPLDBPole.GUID)
                                        {
                                            pldbidForGUID = pNearbyPLDBPole.PLDBID.ToString();
                                            break; 
                                        }
                                    }

                                    //look for a match on replaceguid   
                                    if (pldbidForGUID == "NULL")
                                    {
                                        if (pEDGISPole.ReplaceGUID == pNearbyPLDBPole.GUID)
                                        {
                                            pldbidForGUID = pNearbyPLDBPole.PLDBID.ToString();
                                            break; 
                                        }
                                    }                                    
                                }
                            } 
                        }

                        if (pldbidForGUID == pldbid.ToString())
                        {
                            matchCount++; 
                        }  
                        
                        WriteSQLFilePLDBUpdate(
                                    pEDGISPole.GUID,
                                    pldbidForGUID);
                    }

                    if (matchCount > 1) 
                    {
                        Debug.Print("multiple match count");
                        multiMatchCount++; 

                        //for (int i =0; i < pEDGISPoles.Count; i++)
                        //{
                        //    Debug.Print(pEDGISPoles[i].Latitude.ToString());
                        //    Debug.Print(pEDGISPoles[i].Longitude.ToString());
                        //}
                    }                        
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message); 
            }
        }

        public void FixDUP_PLDBIDs2()
        {
            try
            {
                Shared.InitializeLogfile();
                Shared.InitializeSQLfile(0);
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");

                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureClass pPolesFC = pFWS.OpenFeatureClass("edgis.supportstructure");

                //Get a reader for all SYNC_EDGIS_POLES which 
                //have a count(*) > 1 (DUP PLDBIDs) 
                //Run the query to find the DUP PLDBIDs 
                List<Pole> pAllEDGIS_DUPPoles = new List<Pole>(); 
                Hashtable hshPoleGUIDs = GetDupPoleGUIDs();
                string[] guidsArray = GetArrayOfCommaSeparatedKeys(hshPoleGUIDs, 999, true);

                for (int i = 0; i < guidsArray.Length; i++)
                {
                    List<Pole> thePoles = GetEDGISPoles(pPolesFC, "globalid IN(" + guidsArray[i] + ")");
                    foreach (Pole thePole in thePoles)
                    {
                        pAllEDGIS_DUPPoles.Add(thePole); 
                    }
                }


                List<Pole> pPLDBPoles = GetPoles("SELECT * FROM SYNC_PLDB_POLES WHERE PLDBID IN(SELECT PLDBID FROM SYNC_DuplicatePLDBIDs)");                
                int counter = 0;
                string sql = string.Empty;
                List<Pole> pEDGISPoles = null;                
                string currentGUIDForPLDBID = string.Empty;
                string matchingEDGISGUID = string.Empty; 
                long pldbMatch = -1;
                string pldbMatchString = string.Empty;
                double noMatchCount = 0;
                double matchCount = 0;
                long pldbid = -1; 
                
                
                                
                foreach (Pole pPLDBPole in pPLDBPoles)
                {
                    counter++;
                    pldbid = pPLDBPole.PLDBID; 
                    Shared.WriteToLogfile("Processing ref pldbid: " + pldbid.ToString() +
                        " number: " + counter.ToString());
                    Shared.WriteToLogfile("match count: " + matchCount.ToString() + " no match count: " + noMatchCount.ToString());

                    pEDGISPoles = new List<Pole>();
                    foreach (Pole thePole in pAllEDGIS_DUPPoles)
                    {
                        if (thePole.PLDBID == pPLDBPole.PLDBID)
                            pEDGISPoles.Add(thePole);
                    }
                    if (pEDGISPoles.Count < 2)
                        throw new Exception("error this is not a duplicate"); 
                    
                    Pole pMatchingEDGISPole = FindMatchingEDGISPole(pEDGISPoles, pPLDBPole); 
                    matchingEDGISGUID = string.Empty;
                    if (pMatchingEDGISPole == null)
                    {
                        Shared.WriteToLogfile("No match found for pldbid: " + pldbid.ToString());
                        noMatchCount++;
                    }
                    else
                    {
                        matchCount++; 
                        matchingEDGISGUID = pMatchingEDGISPole.GUID;
                        Shared.WriteToLogfile("Match found! - PLDBID: " + 
                            pldbid.ToString() + " matches edgis pole: " + matchingEDGISGUID); 
                    }

                    if (matchingEDGISGUID != string.Empty) 
                        WriteSQLFilePLDBUpdate(matchingEDGISGUID, pldbid.ToString());
                    Shared.WriteToLogfile("===========================================");

                    //For each non matching edgis pole try to find 
                    //a matching pldb Pole 
                    sql = "SELECT * FROM SYNC_LOOKUP_PLDBID where PLDBID IS NULL";
                    List<Pole> thePoles = GetPoles(sql); 
                    foreach (Pole thePole in thePoles)
                    {
                        if (thePole.GUID != matchingEDGISGUID)
                        {

                            //WriteSQLFilePLDBUpdate(pEDGISPole.GUID, "NULL");

                            //pldbMatch = -1;

                            ////Try to find a PLDBID for this one based on the GUID 
                            //Shared.WriteToLogfile("looking for PLDBID for: " + pEDGISPole.GUID);
                            //if (pldbMatch == -1)
                            //{
                            //    pPLDBPolesViaSearch = GetProdPLDBPolesByGUID(pEDGISPole.GUID);
                            //    if (pPLDBPolesViaSearch.Count == 1)
                            //    {
                            //        pldbMatch = pPLDBPolesViaSearch.First().PLDBID;
                            //    }
                            //}

                            ////Try to find a PLDBID for this one based on the Lat, Long  
                            //if (pldbMatch == -1)
                            //{
                            //    sql = "SELECT b.PLDBID, b.PGE_GLOBALID, b.PGE_SAPEQUIPID, b.Latitude, b.Longitude, " +
                            //    "a.STATUSIDX, a.AnalysisID " +
                            //    "FROM dbo.STATUS a " +
                            //    "INNER JOIN " +
                            //    "(select x.Analysisid, x.Latitude, x.Longitude, x.PLDBID, x.Elevation, " +
                            //    "x.Class, x.LenghtInInches, x.PGE_SAPEQUIPID, x.PGE_GLOBALID, x.BendingFactorOfSafety, " +
                            //    "x.PoleFactorOfSafety, x.Species, x.PGE_SnowLoadDistrict, " +
                            //    "y.status_date FROM dbo.OCalcProAnalysis x " +
                            //    "LEFT JOIN " +
                            //    "(select max(dbo.STATUS.[STATUS_SET_AT]) as status_date, dbo.OCalcProAnalysis.PLDBID " +
                            //    "from dbo.OCalcProAnalysis, dbo.STATUS WHERE " +
                            //    "dbo.OCalcProAnalysis.PLDBID = dbo.STATUS.PLDBID " +
                            //    "group by dbo.OCalcProAnalysis.PLDBID) y " +
                            //    "ON x.PLDBID = y.PLDBID) b " +
                            //    "ON a.AnalysisID = b.AnalysisID " +
                            //    "WHERE a.STATUS_SET_AT = b.status_date And " +
                            //    "(ABS(b.latitude - " + pEDGISPole.Latitude.ToString() + ") < 0.000001 And " +
                            //    "ABS(b.longitude - " + pEDGISPole.Longitude.ToString() + ") < 0.000001)";

                            //    pPLDBPolesViaSearch = GetPoles(sql);

                            //    Hashtable hshPLDBIDs = new Hashtable();
                            //    foreach (Pole thePole in pPLDBPolesViaSearch)
                            //    {
                            //        if (thePole.StatusIndex != 90)
                            //        {
                            //            if (!hshPLDBIDs.ContainsKey(thePole.PLDBID))
                            //                hshPLDBIDs.Add(thePole.PLDBID, 0);
                            //        }
                            //    }


                            //    if (hshPLDBIDs.Count == 1)
                            //    {
                            //        pldbMatch = pPLDBPolesViaSearch.First().PLDBID;
                            //    }
                            //    else if (hshPLDBIDs.Count > 1)
                            //    {
                            //        string pldbidsOnSameLocation = string.Empty;
                            //        foreach (long thepldbid in hshPLDBIDs.Keys)
                            //        {
                            //            pldbidsOnSameLocation += " " + thepldbid.ToString();
                            //        }
                            //        Shared.WriteToLogfile("Warning! Multiple pldbid's at same location. PLDBIDs: " +
                            //            pldbidsOnSameLocation);
                            //    }
                            //}

                            //if (pldbMatch == -1)
                            //    pldbMatchString = "NULL";
                            //else
                            //{
                            //    pldbMatchString = pldbMatch.ToString();

                            //    //Check for duplicate 
                            //    pEDGIS_DUP_Poles = GetEDGISPoles(pPolesFC, "pldbid = " + pldbMatch);
                            //    if (pEDGIS_DUP_Poles.Count != 0)
                            //        pldbMatchString = "NULL";
                            //}

                            //if (pldbMatchString == "NULL")
                            //    Shared.WriteToLogfile("Unable to find a pldbid for edgis pole: " + pEDGISPole.GUID);
                            //else
                            //    Shared.WriteToLogfile("Able to find a pldbid: " + pldbMatchString + " for edgis pole: " + pEDGISPole.GUID);
                            //WriteSQLFilePLDBUpdate(pEDGISPole.GUID, pldbMatchString);

                        }
                    }

                    Shared.WriteToLogfile("==========================================="); 
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        public void FixDecommissions()
        {
            try
            {
                Shared.InitializeLogfile();
                Shared.InitializeSQLfile(0);
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());
                Shared.WriteToLogfile("Connecting to workspaces");
                Shared.LoadWorkspaces();
                Shared.WriteToLogfile("======================================================");

                IFeatureWorkspace pFWS = (IFeatureWorkspace)Shared.GetWorkspaceByName("electric");
                IFeatureClass pPolesFC = pFWS.OpenFeatureClass("edgis.supportstructure");

                //First get a list of all the decommissions poles 
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.DECOMM_DISCREPANCY_QUERY_SQL);
                List<Pole> pPotentialDecommPolesList = GetPoles(sql);
                Hashtable hshPoleGUIDs = new Hashtable();
                Hashtable hshPoleSapEquipIDs = new Hashtable();

                //Loop through all of the Decommissions and first see 
                //how many can be matched up using the GUID 
                foreach (Pole thePole in pPotentialDecommPolesList)
                {
                    if (!hshPoleGUIDs.ContainsKey(thePole.GUID))
                        hshPoleGUIDs.Add(thePole.GUID, 0); 
                }

                List<Pole> pEDGIS_Poles_With_GUIDs = new List<Pole>(); 
                string[] guidsArray = GetArrayOfCommaSeparatedKeys(hshPoleGUIDs, 999, true);                
                for (int i = 0; i < guidsArray.Length; i++)
                {
                    List<Pole> thePoles = GetEDGISPoles(pPolesFC, "globalid IN(" + guidsArray[i] + ")");
                    foreach (Pole thePole in thePoles)
                    {
                        pEDGIS_Poles_With_GUIDs.Add(thePole);
                    }
                }

                foreach (Pole thePole in pPotentialDecommPolesList)
                {
                        if (!hshPoleSapEquipIDs.ContainsKey(thePole.SapEquipID))
                            hshPoleSapEquipIDs.Add(thePole.SapEquipID, 0);

                }
                guidsArray = GetArrayOfCommaSeparatedKeys(hshPoleSapEquipIDs, 999, false);
                for (int i = 0; i < guidsArray.Length; i++)
                {
                    List<Pole> thePoles = GetEDGISPoles(pPolesFC, "sapequipid IN(" + guidsArray[i] + ")");
                    foreach (Pole thePole in thePoles)
                    {
                        pEDGIS_Poles_With_GUIDs.Add(thePole);
                    }
                }



                List<Pole> pPLDBPoles = GetPoles("SELECT * FROM SYNC_PLDB_POLES WHERE PLDBID IN(SELECT PLDBID FROM SYNC_DuplicatePLDBIDs)");
                int counter = 0;
                List<Pole> pEDGISPoles = null;
                string currentGUIDForPLDBID = string.Empty;
                string matchingEDGISGUID = string.Empty;
                long pldbMatch = -1;
                string pldbMatchString = string.Empty;
                double noMatchCount = 0;
                double matchCount = 0;
                long pldbid = -1;



                //foreach (Pole pPLDBPole in pPLDBPoles)
                //{
                //    counter++;
                //    pldbid = pPLDBPole.PLDBID;
                //    Shared.WriteToLogfile("Processing ref pldbid: " + pldbid.ToString() +
                //        " number: " + counter.ToString());
                //    Shared.WriteToLogfile("match count: " + matchCount.ToString() + " no match count: " + noMatchCount.ToString());

                //    pEDGISPoles = new List<Pole>();
                //    foreach (Pole thePole in pAllEDGIS_DUPPoles)
                //    {
                //        if (thePole.PLDBID == pPLDBPole.PLDBID)
                //            pEDGISPoles.Add(thePole);
                //    }
                //    if (pEDGISPoles.Count < 2)
                //        throw new Exception("error this is not a duplicate");

                //    Pole pMatchingEDGISPole = FindMatchingEDGISPole(pEDGISPoles, pPLDBPole);
                //    matchingEDGISGUID = string.Empty;
                //    if (pMatchingEDGISPole == null)
                //    {
                //        Shared.WriteToLogfile("No match found for pldbid: " + pldbid.ToString());
                //        noMatchCount++;
                //    }
                //    else
                //    {
                //        matchCount++;
                //        matchingEDGISGUID = pMatchingEDGISPole.GUID;
                //        Shared.WriteToLogfile("Match found! - PLDBID: " +
                //            pldbid.ToString() + " matches edgis pole: " + matchingEDGISGUID);
                //    }

                //    if (matchingEDGISGUID != string.Empty)
                //        WriteSQLFilePLDBUpdate(matchingEDGISGUID, pldbid.ToString());
                //    Shared.WriteToLogfile("===========================================");

                //    //For each non matching edgis pole try to find 
                //    //a matching pldb Pole 

                //    //foreach (Pole pEDGISPole in pEDGISPoles)
                //    //{
                //    //    if (pEDGISPole.GUID != matchingEDGISGUID)
                //    //    {

                //    //        WriteSQLFilePLDBUpdate(pEDGISPole.GUID, "NULL");

                //    //        //pldbMatch= -1; 

                //    //        ////Try to find a PLDBID for this one based on the GUID 
                //    //        //Shared.WriteToLogfile("looking for PLDBID for: " + pEDGISPole.GUID);
                //    //        //if (pldbMatch == -1) 
                //    //        //{
                //    //        //    pPLDBPolesViaSearch = GetProdPLDBPolesByGUID(pEDGISPole.GUID);
                //    //        //    if (pPLDBPolesViaSearch.Count == 1)
                //    //        //    {
                //    //        //        pldbMatch = pPLDBPolesViaSearch.First().PLDBID; 
                //    //        //    }
                //    //        //}

                //    //        ////Try to find a PLDBID for this one based on the Lat, Long  
                //    //        //if (pldbMatch == -1) 
                //    //        //{
                //    //        //    sql = "SELECT b.PLDBID, b.PGE_GLOBALID, b.PGE_SAPEQUIPID, b.Latitude, b.Longitude, " + 
                //    //        //    "a.STATUSIDX, a.AnalysisID " + 
                //    //        //    "FROM dbo.STATUS a " + 
                //    //        //    "INNER JOIN " + 
                //    //        //    "(select x.Analysisid, x.Latitude, x.Longitude, x.PLDBID, x.Elevation, " + 
                //    //        //    "x.Class, x.LenghtInInches, x.PGE_SAPEQUIPID, x.PGE_GLOBALID, x.BendingFactorOfSafety, " + 
                //    //        //    "x.PoleFactorOfSafety, x.Species, x.PGE_SnowLoadDistrict, " + 
                //    //        //    "y.status_date FROM dbo.OCalcProAnalysis x " + 
                //    //        //    "LEFT JOIN " + 
                //    //        //    "(select max(dbo.STATUS.[STATUS_SET_AT]) as status_date, dbo.OCalcProAnalysis.PLDBID " + 
                //    //        //    "from dbo.OCalcProAnalysis, dbo.STATUS WHERE " + 
                //    //        //    "dbo.OCalcProAnalysis.PLDBID = dbo.STATUS.PLDBID " + 
                //    //        //    "group by dbo.OCalcProAnalysis.PLDBID) y " + 
                //    //        //    "ON x.PLDBID = y.PLDBID) b " + 
                //    //        //    "ON a.AnalysisID = b.AnalysisID " + 
                //    //        //    "WHERE a.STATUS_SET_AT = b.status_date And " + 
                //    //        //    "(ABS(b.latitude - " + pEDGISPole.Latitude.ToString() + ") < 0.000001 And " +
                //    //        //    "ABS(b.longitude - " + pEDGISPole.Longitude.ToString() + ") < 0.000001)"; 

                //    //        //    pPLDBPolesViaSearch = GetPoles(sql);

                //    //        //    Hashtable hshPLDBIDs = new Hashtable(); 
                //    //        //    foreach (Pole thePole in pPLDBPolesViaSearch)
                //    //        //    {
                //    //        //        if (thePole.StatusIndex != 90)
                //    //        //        {
                //    //        //            if (!hshPLDBIDs.ContainsKey(thePole.PLDBID))
                //    //        //                hshPLDBIDs.Add(thePole.PLDBID, 0);
                //    //        //        }
                //    //        //    }


                //    //        //    if (hshPLDBIDs.Count == 1)
                //    //        //    {
                //    //        //        pldbMatch = pPLDBPolesViaSearch.First().PLDBID; 
                //    //        //    }
                //    //        //    else if (hshPLDBIDs.Count > 1) 
                //    //        //    {
                //    //        //        string pldbidsOnSameLocation = string.Empty;
                //    //        //        foreach (long thepldbid in hshPLDBIDs.Keys)
                //    //        //        {
                //    //        //            pldbidsOnSameLocation += " " + thepldbid.ToString(); 
                //    //        //        }
                //    //        //        Shared.WriteToLogfile("Warning! Multiple pldbid's at same location. PLDBIDs: " + 
                //    //        //            pldbidsOnSameLocation); 
                //    //        //    }
                //    //        //}

                //    //        //if (pldbMatch == -1) 
                //    //        //    pldbMatchString = "NULL"; 
                //    //        //else 
                //    //        //{
                //    //        //    pldbMatchString = pldbMatch.ToString();

                //    //        //    //Check for duplicate 
                //    //        //    pEDGIS_DUP_Poles = GetEDGISPoles(pPolesFC, "pldbid = " + pldbMatch);
                //    //        //    if (pEDGIS_DUP_Poles.Count != 0) 
                //    //        //        pldbMatchString = "NULL";
                //    //        //}

                //    //        //if (pldbMatchString == "NULL")
                //    //        //    Shared.WriteToLogfile("Unable to find a pldbid for edgis pole: " + pEDGISPole.GUID);
                //    //        //else
                //    //        //    Shared.WriteToLogfile("Able to find a pldbid: " + pldbMatchString + " for edgis pole: " + pEDGISPole.GUID);
                //    //        //WriteSQLFilePLDBUpdate(pEDGISPole.GUID, pldbMatchString); 

                //    //    }
                //    //}

                //    Shared.WriteToLogfile("===========================================");
                //}
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }


        private List<Pole> GetProdPLDBPolesByPLDBID(long pldbid)
        {
            try
            {
                string sqlForPLDBPoleByPLDBID =
                    "SELECT b.PLDBID, b.PGE_GLOBALID, b.PGE_SAPEQUIPID, b.Latitude, b.Longitude, " +
                    "a.STATUSIDX, a.AnalysisID " +
                    "FROM dbo.STATUS a " +
                    "INNER JOIN " +
                    "(select x.Analysisid, x.Latitude, x.Longitude, x.PLDBID, x.Elevation," +
                    "x.Class, x.LenghtInInches, x.PGE_SAPEQUIPID, x.PGE_GLOBALID, x.BendingFactorOfSafety, " +
                    "x.PoleFactorOfSafety, x.Species, x.PGE_SnowLoadDistrict, " +
                    "y.status_date FROM dbo.OCalcProAnalysis x " +
                    "LEFT JOIN " +
                    "(select max(dbo.STATUS.[STATUS_SET_AT]) as status_date, dbo.OCalcProAnalysis.PLDBID " +
                    "from dbo.OCalcProAnalysis, dbo.STATUS WHERE " +
                    "dbo.OCalcProAnalysis.PLDBID = dbo.STATUS.PLDBID " +
                    "group by dbo.OCalcProAnalysis.PLDBID) y " +
                    "ON x.PLDBID = y.PLDBID) b " +
                    "ON a.AnalysisID = b.AnalysisID " +
                    "WHERE a.STATUS_SET_AT = b.status_date And " +
                    "a.PLDBID = " + pldbid.ToString();
                return GetPoles(sqlForPLDBPoleByPLDBID);  

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex; 
            }
        }

        private List<Pole> GetProdPLDBPolesByGUID(string guid)
        {
            try
            {                
                string sqlForPLDBPoleByGUID =
                    "SELECT b.PLDBID, b.PGE_GLOBALID, b.PGE_SAPEQUIPID, b.Latitude, b.Longitude, " +
                    "a.STATUSIDX, a.AnalysisID " +
                    "FROM dbo.STATUS a " +
                    "INNER JOIN " +
                    "(select x.Analysisid, x.Latitude, x.Longitude, x.PLDBID, x.Elevation," +
                    "x.Class, x.LenghtInInches, x.PGE_SAPEQUIPID, x.PGE_GLOBALID, x.BendingFactorOfSafety, " +
                    "x.PoleFactorOfSafety, x.Species, x.PGE_SnowLoadDistrict, " +
                    "y.status_date FROM dbo.OCalcProAnalysis x " +
                    "LEFT JOIN " +
                    "(select max(dbo.STATUS.[STATUS_SET_AT]) as status_date, dbo.OCalcProAnalysis.PLDBID " +
                    "from dbo.OCalcProAnalysis, dbo.STATUS WHERE " +
                    "dbo.OCalcProAnalysis.PLDBID = dbo.STATUS.PLDBID " +
                    "group by dbo.OCalcProAnalysis.PLDBID) y " +
                    "ON x.PLDBID = y.PLDBID) b " +
                    "ON a.AnalysisID = b.AnalysisID " +
                    "WHERE a.STATUS_SET_AT = b.status_date And " +
                    "b.PGE_GLOBALID = '" + guid + "'";
                return GetPoles(sqlForPLDBPoleByGUID);

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex;
            }
        }


        private Pole FindMatchingEDGISPole(List<Pole> pEDGISPoles, Pole pPLDBPole)
        {
            try
            {

                foreach (Pole pEDGISPole in pEDGISPoles)
                {
                    //1. try to match on globalid   
                    if (pPLDBPole.StatusIndex == 10)
                    {
                        //baseline pole scenario
                        if (pEDGISPole.GUID == pPLDBPole.GUID)
                        {
                            return pEDGISPole; 
                        }
                    }                    
                    else
                    {
                        //non-baseline pole scenario
                        if (pEDGISPole.GUID == pPLDBPole.GUID)
                        {
                            return pEDGISPole;
                        }

                        if (pEDGISPole.ReplaceGUID == pPLDBPole.GUID)
                        {
                            return pEDGISPole;
                        }
                    }
                }

                return null; 
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex; 
            }
        }

        private List<Pole> GetEDGISPoles(IFeatureClass pPolesFC, string whereclause)
        {
            try
            {
                long pldbId = 0;
                double latitude = 0;
                double longitude = 0;
                long sapequipId = 0;
                string guid = string.Empty;
                string replaceGuid = string.Empty;                
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = whereclause; 
                List<Pole> pPoles = new List<Pole>();
                object o = null;
                int subtypeCd = -1; 
                IFeatureCursor pPolesFCursor = pPolesFC.Search(pQF, false);
                IFeature pPoleFeature = pPolesFCursor.NextFeature();

                while (pPoleFeature != null)
                {
                    pldbId = 0;
                    sapequipId = 0;
                    guid = string.Empty;
                    replaceGuid = string.Empty;
                    subtypeCd = -1; 

                    o = pPoleFeature.get_Value(pPoleFeature.Fields.FindField("globalid"));
                    if (o != DBNull.Value)
                        guid = o.ToString();

                    o = pPoleFeature.get_Value(pPoleFeature.Fields.FindField("replaceguid"));
                    if (o != DBNull.Value)
                        replaceGuid = o.ToString();

                    o = pPoleFeature.get_Value(pPoleFeature.Fields.FindField("sapequipid"));
                    if (o != DBNull.Value)
                    {
                        if (!Int64.TryParse(o.ToString(), out sapequipId)) 
                            sapequipId = 0;
                    }

                    o = pPoleFeature.get_Value(pPoleFeature.Fields.FindField("pldbid"));
                    if (o != DBNull.Value)
                    {
                        if (!Int64.TryParse(o.ToString(), out pldbId))
                            pldbId = 0;
                    }

                    o = pPoleFeature.get_Value(pPoleFeature.Fields.FindField("subtypecd"));
                    if (o != DBNull.Value)
                    {
                        if (!Int32.TryParse(o.ToString(), out subtypeCd))
                            subtypeCd = -1;
                    }


                    PopulateLatLongForPole((IPoint)pPoleFeature.ShapeCopy, ref latitude, ref longitude);
                    
                    Pole pPole = new Pole(
                        pldbId,
                        guid,
                        sapequipId,
                        replaceGuid,
                        latitude,
                        longitude, 
                        0);
                    pPole.Subtype = subtypeCd; 
                    pPoles.Add(pPole); 

                    pPoleFeature = pPolesFCursor.NextFeature(); 
                }
                Marshal.FinalReleaseComObject(pPolesFCursor); 


                return pPoles;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex;
            }

        }



        private void WriteSQLFilePLDBUpdate(string guid, string pldid)
        {
            try
            {
                Shared.WriteToSQLFile(
                        "UPDATE EDGIS.SUPPORTSTRUCTURE SET PLDBID = " +
                        pldid +
                        " WHERE GLOBALID = '" + guid + "';");
                Shared.WriteToSQLFile(
                        "UPDATE EDGIS.A144 SET PLDBID = " +
                        pldid +
                        " WHERE GLOBALID = '" + guid + "';");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private List<Pole> GetPoles(string sql)
        {
            try
            {
                List<Pole> pPoles = new List<Pole>(); 

                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants.
                        CONFIG_PLDB_CONNECTION].ConnectionString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                Hashtable hshPLDBIDs = new Hashtable();
                SqlCommand pCmd = _pConn.CreateCommand();
                pCmd.CommandText = sql;
                string sapIdString = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader();
                long pldbId = 0;
                long sapequipId = 0;
                string guid = string.Empty;
                string replaceGuid = string.Empty;
                int statusIdx = 0; 
                double latitude = 0; 
                double longitude = 0; 
                object o = null;

                while (reader.Read())
                {
                    pldbId = 0;
                    sapequipId = 0;
                    guid = string.Empty;
                    replaceGuid = string.Empty;
                    latitude = 0; 
                    longitude = 0;
                    statusIdx = 0;

                    if (ReaderHasField(reader, "pldbid"))
                    {
                        o = reader.GetValue(reader.GetOrdinal("pldbid"));
                        if (o != DBNull.Value)
                            pldbId = Convert.ToInt64(o);
                    }

                    if (ReaderHasField(reader, "pge_globalid"))
                    {
                        o = reader.GetValue(reader.GetOrdinal("pge_globalid"));
                        if (o != DBNull.Value)
                            guid = o.ToString();
                    }

                    if (ReaderHasField(reader, "pge_sapequipid"))
                    {
                        o = reader.GetValue(reader.GetOrdinal("pge_sapequipid"));
                        if (o != DBNull.Value)
                            sapequipId = Convert.ToInt64(o);
                    }

                    if (ReaderHasField(reader, "pge_replaceguid"))
                    {
                        if (reader.GetOrdinal("pge_replaceguid") > 0)
                        {
                            o = reader.GetValue(reader.GetOrdinal("pge_replaceguid"));
                            if (o != DBNull.Value)
                                replaceGuid = o.ToString();
                        }
                    }

                    if (ReaderHasField(reader, "latitude"))
                    {
                        o = reader.GetValue(reader.GetOrdinal("latitude"));
                        if (o != DBNull.Value)
                            latitude = Convert.ToDouble(o);
                    }

                    if (ReaderHasField(reader, "longitude"))
                    {
                        o = reader.GetValue(reader.GetOrdinal("longitude"));
                        if (o != DBNull.Value)
                            longitude = Convert.ToDouble(o);
                    }

                    if (ReaderHasField(reader, "STATUSIDX"))
                    {
                        o = reader.GetValue(reader.GetOrdinal("STATUSIDX"));
                        if (o != DBNull.Value)
                            statusIdx = Convert.ToInt32(o);
                    }  


                    Pole pPole = new Pole( 
                        pldbId, 
                        guid, 
                        sapequipId, 
                        replaceGuid, 
                        latitude, 
                        longitude, 
                        statusIdx);
                    pPoles.Add(pPole); 

                }

                reader.Close();  
                pCmd.Dispose(); 

                return pPoles; 
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex; 
            }

        }

        private bool ReaderHasField(SqlDataReader theReader, string fieldName)
        {
            try
            {
                for (int i = 0; i < theReader.FieldCount; i++)
                {
                    if (theReader.GetName(i).ToLower() == fieldName.ToLower())
                        return true; 
                }
                return false; 
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex;
            }
        }

        private Hashtable GetPLDBID_DUPs()
        {
            try
            {
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants.
                        CONFIG_PLDB_CONNECTION].ConnectionString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();

                //Setup the command 
                Hashtable hshPLDBIDs = new Hashtable(); 
                SqlCommand pCmd = _pConn.CreateCommand();
                string sql = "SELECT count(*), pldbid from SYNC_EDGIS_POLES group by pldbid having count(*) > 1";
                pCmd.CommandText = sql;
                string sapIdString = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader(); 
                long pldbId = 0;
                object o = null;

                while (reader.Read())
                {
                    pldbId = 0; 
                    o = reader.GetValue(reader.GetOrdinal("pldbid"));
                    if (o != DBNull.Value)
                    {
                        pldbId = Convert.ToInt64(o);
                        if (!hshPLDBIDs.ContainsKey(pldbId))  
                            hshPLDBIDs.Add(pldbId, 0); 
                    }
                }
                reader.Close();
                pCmd.Dispose(); 

                return hshPLDBIDs; 
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex; 
            }
        }

        private Hashtable GetUnMatchedPoleGUIDsFromTest(string connectionString)
        {
            SqlConnection pConn = null; 

            try
            {
                pConn = new SqlConnection(connectionString); 
                if (pConn.State != System.Data.ConnectionState.Open)
                    pConn.Open();

                //Setup the command 
                Hashtable hshGUIDs = new Hashtable();
                SqlCommand pCmd = pConn.CreateCommand();
                string sql = "SELECT GLOBALID from SYNC_LOOKUP_PLDBID WHERE PLDBID IS NULL";
                pCmd.CommandText = sql;
                string guid = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader();
                long pldbId = 0;
                object o = null;

                while (reader.Read())
                {
                    guid = string.Empty; 
                    o = reader.GetValue(reader.GetOrdinal("globalid"));
                    if (o != DBNull.Value)
                    {
                        guid = o.ToString().ToUpper();
                        if (!hshGUIDs.ContainsKey(guid))
                            hshGUIDs.Add(guid, 0); 
                    }
                }
                reader.Close();
                pCmd.Dispose();

                return hshGUIDs;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex;
            }
            finally
            {
                pConn.Close(); 
            }
        }


        private Hashtable GetDupPoleGUIDs()
        {
            try
            {
                //Connect to test for this 
                SqlConnection pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants.
                        CONFIG_PLDB_CONNECTION].ConnectionString);
                pConn.Open();

                //Setup the command 
                Hashtable hshGUIDs = new Hashtable();
                SqlCommand pCmd = pConn.CreateCommand();
                string sql = "SELECT pge_globalid from SYNC_DuplicatePLDBIDs";
                pCmd.CommandText = sql;
                string sapIdString = string.Empty;
                SqlDataReader reader = pCmd.ExecuteReader();
                long pldbId = 0;
                object o = null;
                string theGUID = string.Empty; 
                
                while (reader.Read())
                {
                    theGUID = reader.GetValue( 
                        reader.GetOrdinal("pge_globalid")).
                        ToString().ToUpper();
                    if (!hshGUIDs.ContainsKey(theGUID))
                        hshGUIDs.Add(theGUID, 0);


                    //if (o != DBNull.Value)
                    //{
                    //    pldbId = Convert.ToInt64(o);
                    //    if (!hshPLDBIDs.ContainsKey(pldbId))
                    //        hshPLDBIDs.Add(pldbId, 0);
                    //}
                }
                reader.Close();
                pCmd.Dispose();
                return hshGUIDs;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                throw ex;
            }
        }

        private string[] GetArrayOfCommaSeparatedKeys(Hashtable hshKeys, int batchSize, bool addApostrophe)
        {
            try
            {
                Hashtable hshCommaSeparatedKeys = new Hashtable();
                int counter = 0;
                StringBuilder batchLine = new StringBuilder();

                foreach (object key in hshKeys.Keys)
                {
                    if (counter == 0)
                    {
                        if (addApostrophe)
                            batchLine.Append("'" + key.ToString() + "'");
                        else
                            batchLine.Append(key.ToString());
                    }
                    else
                    {
                        if (addApostrophe)
                            batchLine.Append("," + "'" + key.ToString() + "'");
                        else
                            batchLine.Append("," + key.ToString());
                    }

                    counter++;
                    if (counter == batchSize)
                    {
                        hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);
                        batchLine = new StringBuilder();
                        counter = 0;
                    }
                }

                //Add what is left over 
                if (batchLine.ToString().Length != 0)
                    hshCommaSeparatedKeys.Add(batchLine.ToString(), 0);

                //Convert this to an array 
                counter = 0;
                string[] commaSepKeys = new string[hshCommaSeparatedKeys.Count];
                foreach (string line in hshCommaSeparatedKeys.Keys)
                {
                    commaSepKeys[counter] = line;
                    counter++;
                }

                //return array 
                return commaSepKeys;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Entering error handler for " + MethodBase.GetCurrentMethod() + " error: " +
                    ex.Message);
                throw new Exception("Error returning array of comma separated keys");
            }
        }


        private int GetTableRowCount(string tableName)
        {
            SqlCommand pCmd = null;

            try
            {
                if (_pConn == null)
                {
                    //Get output DB connection 
                    _pConn = new SqlConnection(
                        ConfigurationManager.
                        ConnectionStrings[PLDBBatchConstants.
                        CONFIG_PLDB_CONNECTION].ConnectionString);
                }
                if (_pConn.State != System.Data.ConnectionState.Open)
                    _pConn.Open();
                

                //Populate string to hold the SQL statement
                int recordCount = 0;
                string sqlStatement =
                    "SELECT COUNT(*) FROM " +
                    tableName;

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                {
                    pCmd = _pConn.CreateCommand();
                    pCmd.CommandText = sqlStatement;
                    object o = pCmd.ExecuteScalar();
                    recordCount = Convert.ToInt32(o);
                }

                return recordCount;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
            finally
            {
                //release database 
                pCmd = null;
            }
        }

        private void WriteInsertToPolesFile(
            string pldbid,
            string guid,
            string sapEquipId, 
            string replaceguid, 
            double x,
            double y, 
            int fileIndex)
        {
            try
            {
                //String to hold the SQL insert statement 
                StringBuilder valuesList = new StringBuilder();
                valuesList.Append(pldbid);
                valuesList.Append(",");
                valuesList.Append("'" + guid + "'");
                valuesList.Append(",");
                valuesList.Append("'" + replaceguid + "'");
                valuesList.Append(",");
                valuesList.Append(sapEquipId);
                valuesList.Append(",");
                valuesList.Append(x);
                valuesList.Append(",");
                valuesList.Append(y);

                string sqlStatement = String.Format("INSERT INTO {0} ({1}) VALUES({2})",
                    PLDBBatchConstants.EDGIS_POLES_TABLE,
                    PLDBBatchConstants.EDGIS_POLES_FIELDS,
                    valuesList.ToString());

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                    Shared.WriteToSQLFile(sqlStatement);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }

        private void WriteInsertToLatLongUpdatesFile(
            string pldbid,
            string guid,
            double elevation,
            double latitude,
            double longitude,
            int fileIndex)
        {
            try
            {
                //String to hold the SQL insert statement 
                StringBuilder valuesList = new StringBuilder();
                valuesList.Append(pldbid);
                valuesList.Append(",");
                valuesList.Append("'" + guid + "'");
                valuesList.Append(",");
                valuesList.Append(elevation);
                valuesList.Append(",");
                valuesList.Append(latitude);
                valuesList.Append(",");
                valuesList.Append(longitude);

                string sqlStatement = String.Format("INSERT INTO {0} ({1}) VALUES({2})",
                    PLDBBatchConstants.LAT_LONG_UPDATES_TABLE,
                    PLDBBatchConstants.LAT_LONG_UPDATE_FIELDS,
                    valuesList.ToString());

                //Execute the command against the database 
                if (sqlStatement != string.Empty)
                    Shared.WriteToSQLFile(sqlStatement);
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw ex;
            }
        }


        //private void WriteLatLongUpdateToFile(
        //    long pldbid,
        //    double latitude,
        //    double longitude,
        //    int fileIndex)
        //{
        //    try
        //    {
        //        //SQL statement to update lat, long in OCalcProAnalysis 
        //        string sqlStatement = String.Format("UPDATE {0} SET {1} = {2}, {3} = {4} WHERE PLDBID = {5};",
        //            PLDBBatchConstants.OCALCPROANALYSIS_TABLE,
        //            PLDBBatchConstants.OPA_LAT_FIELD,
        //            latitude,
        //            PLDBBatchConstants.OPA_LONG_FIELD,
        //            longitude, 
        //            pldbid); 

        //        //Execute the command against the database 
        //        if (sqlStatement != string.Empty)
        //            Shared.WriteToSQLFile(sqlStatement);

        //        //SQL statement to update lat, long in OCalcProAnalysisRoot
        //        //UPDATE OCalcProAnalysisRoot SET lat = xxx, long  = yyy where analysisid IN(
        //        //SELECT ANALYSISID From OCalcProAnalysis WHERE PLDBID = xyz and AnalysisId <> 
        //        //'00000000-0000-0000-0000-000000000000');

        //        sqlStatement = String.Format( 
        //            "UPDATE {0} SET {1} = {2}, {3} = {4} WHERE ANALYSISID IN" + 
        //            "(" + 
        //                "SELECT ANALYSISID FROM {5} WHERE PLDBID = {6} AND ANALYSISID <> '00000000-0000-0000-0000-000000000000'" + 
        //            ");", 
        //            PLDBBatchConstants.OCALCPROANALYSISROOT_TABLE,
        //            PLDBBatchConstants.OPAR_LAT_FIELD,
        //            latitude,
        //            PLDBBatchConstants.OPAR_LONG_FIELD,
        //            longitude,
        //            PLDBBatchConstants.OCALCPROANALYSIS_TABLE,
        //            pldbid); 

        //        //Execute the command against the database 
        //        if (sqlStatement != string.Empty)
        //            Shared.WriteToSQLFile(sqlStatement);
        //    }
        //    catch (Exception ex)
        //    {
        //        Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
        //            " details: " + ex.Message);
        //        throw ex;
        //    }
        //}



        public void PopulateLatLongForPole(IPoint pPolePoint, ref double latValue, ref double longValue)
        {
            try
            {
                //First open the source featureclass and do a spatial search 
                //for all plats within the clip
                if (m_pSR_WGS84 == null)
                {
                    int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;
                    ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();
                    m_pSR_WGS84 = pSRF.CreateGeographicCoordinateSystem(geoType);
                }

                //Clone the ref pole point 
                IPoint pPolePointClone = (IPoint)CloneShape(pPolePoint, pPolePoint.SpatialReference);
                pPolePointClone.Project(m_pSR_WGS84);

                longValue = pPolePointClone.X;
                latValue = pPolePointClone.Y;

            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error in routine PopulateLatLongForPole");
            }
        }

        /// <summary>
        /// Clones the passed geometry 
        /// </summary>
        /// <param name="pGeometryToClone"></param>
        /// <param name="pSR"></param>
        /// <returns></returns>
        private IGeometry CloneShape(IGeometry pGeometryToClone, ISpatialReference pSR)
        {
            try
            {
                IClone pClone = (IClone)pGeometryToClone;
                IGeometry pGeom = (IGeometry)pClone.Clone();
                pGeom.SpatialReference = pSR;
                return pGeom;
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error cloning shape");
            }
        }

        private void PopulatePLDBPoles()
        {
            try
            {
                Shared.WriteToLogfile("Entering " + MethodBase.GetCurrentMethod());

                //Get the SQL to create the table 
                string sql = Shared.GetSQLFromSQLFile(
                    PLDBBatchConstants.CREATE_PLDB_POLES_TABLE_SQL);

                //Drop and create the table  
                DropTable(PLDBBatchConstants.PLDB_POLES_TABLE);
                CreateTable(sql); 
            }
            catch (Exception ex)
            {
                Shared.WriteToLogfile("Error in " + MethodBase.GetCurrentMethod() +
                    " details: " + ex.Message);
                throw new Exception("Error populating EDGIS poles");
            }
        }

    }

    public class BulkUploadToSql

    {

        private List<Pole> internalStore;
        protected string tableName;
        protected DataTable dataTable = new DataTable();
        protected int recordCount;
        protected int commitBatchSize;

        private BulkUploadToSql(
            string tableName,
            int commitBatchSize)

        {

            internalStore = new List<Pole>();
            this.tableName = tableName;
            this.dataTable = new DataTable(tableName);
            this.recordCount = 0;
            this.commitBatchSize = commitBatchSize;

            // add columns to this data table
            InitializeStructures();
        }

        private void InitializeStructures()
        {
            if (this.tableName == PLDBBatchConstants.EDGIS_POLES_TABLE)
            {
                this.dataTable.Columns.Add("PLDBID", typeof(Int64));
                this.dataTable.Columns.Add("PGE_GLOBALID", typeof(System.Guid));
                this.dataTable.Columns.Add("PGE_REPLACEGUID", typeof(string));
                this.dataTable.Columns.Add("PGE_SAPEQUIPID", typeof(string));
                this.dataTable.Columns.Add("Latitude", typeof(double));
                this.dataTable.Columns.Add("Longitude", typeof(double));
            }
            else if (this.tableName == PLDBBatchConstants.LAT_LONG_UPDATES_TABLE)
            {
                this.dataTable.Columns.Add("PLDBID", typeof(Int64));
                this.dataTable.Columns.Add("EDGIS_GLOBALID", typeof(System.Guid));
                this.dataTable.Columns.Add("ELEVATION", typeof(double));
                this.dataTable.Columns.Add("Latitude", typeof(double));
                this.dataTable.Columns.Add("Longitude", typeof(double));
            }
            else
                throw new Exception("Unable table name"); 
        }

        public static BulkUploadToSql Load(List<Pole> thePoles, string tableName)
        {
            // create a new object to return
            BulkUploadToSql o = new BulkUploadToSql(tableName, 50000); 
            foreach(Pole thePole in thePoles)
            {
                o.internalStore.Add(thePole);
            }
            return o;
        }

        public void Flush()

        {
            // transfer data to the datatable 
            foreach (Pole thePole in this.internalStore)

            {
                this.PopulateDataTable(thePole);
                if (this.recordCount >= this.commitBatchSize)
                    this.WriteToDatabase();
            }

            // write remaining records to the DB 
            if (this.recordCount > 0)
                this.WriteToDatabase();
        }

        private void WriteToDatabase()

        {
            // get your connection string
            string connString = ConfigurationManager.
                    ConnectionStrings[PLDBBatchConstants.
                    CONFIG_PLDB_CONNECTION].
                    ConnectionString;
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.
                    ConnectionStrings[PLDBBatchConstants.
                    CONFIG_PLDB_CONNECTION].
                    ConnectionString))
            { 

                SqlBulkCopy bulkCopy =
                    new SqlBulkCopy(
                    connection,
                    SqlBulkCopyOptions.TableLock |
                    SqlBulkCopyOptions.FireTriggers |
                    SqlBulkCopyOptions.UseInternalTransaction,
                    null);

                // set the destination table name
                bulkCopy.DestinationTableName = this.tableName;
                connection.Open();

                // write the data in the "dataTable"
                bulkCopy.WriteToServer(dataTable);
                connection.Close();
            }

            // reset
            this.dataTable.Clear();
            this.recordCount = 0;
        }


        private void PopulateDataTable(Pole thePole)
        {
            DataRow row = this.dataTable.NewRow();

            // populate the datarow with a pole
            if (this.tableName == PLDBBatchConstants.EDGIS_POLES_TABLE)
            {
                row[0] = thePole.PLDBID;
                Guid theGuid = new Guid(thePole.GUID);
                row[1] = theGuid;
                row[2] = thePole.ReplaceGUID;
                row[3] = thePole.SapEquipID;
                row[4] = thePole.Latitude;
                row[5] = thePole.Longitude;
            }
            else if (this.tableName == PLDBBatchConstants.LAT_LONG_UPDATES_TABLE)
            {
                row[0] = thePole.PLDBID;
                Guid theGuid = new Guid(thePole.GUID);
                row[1] = theGuid;
                row[2] = thePole.Elevation;
                row[3] = thePole.Latitude;
                row[4] = thePole.Longitude;
            }
            
            // add it to the base for final addition to the DB
            this.dataTable.Rows.Add(row);
            this.recordCount++;
        }
    }  
}
