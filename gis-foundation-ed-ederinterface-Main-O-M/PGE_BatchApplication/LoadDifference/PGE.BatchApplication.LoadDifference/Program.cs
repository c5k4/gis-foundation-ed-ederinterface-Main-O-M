// ========================================================================
// Copyright © 2021 PGE 
// <history>
// To execute SSD,Circuit Color,Jet Operating Number and DeviceGroupCircuitID Action Handler logic through batchjob
// YXA6 4/14/2021	Created
// JeeraID-> EDGISRearch-376
// </history>
// All rights reserved.
// ========================================================================
using System;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using System.Collections.Generic;
using System.Data;

namespace PGE.BatchApplication.LoadDifference
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static DBHelper clsdbhelper = new DBHelper();
        private static Miner.Interop.IMMAppInitialize _appInitialize;
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        public static IList<string> ClassNotExist = new List<string>();
        public static IDictionary<string, IObjectClass> Featureclasslist = new Dictionary<string, IObjectClass>();
        public static IDictionary<string, IObjectClass> EDGMCFeatureclasslist = new Dictionary<string, IObjectClass>();
        public static IDictionary<string, IObjectClass> DefaultFeatureclasslist = new Dictionary<string, IObjectClass>();
        private static GeoDBHelper clsgdbhelper = new GeoDBHelper();
        public static string sOracleConnectionstring = ReadConfigurations.OracleConnString;
        private static Common CommonFuntions = new Common();
        public static DataTable InsertTable = null; 
        static void Main(string[] args)
        { 
           

            try
            {
                

                //Initialize License
                InitializeLicense();
                
                StartUpdateDataProcess(args[0].ToString());

            }
            catch (Exception exp)
            {
                CommonFuntions.WriteLine_Error(exp.Message + " at " + exp.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                m_AOLicenseInitializer.ShutdownApplication();

            }
        }

             
        private static void StartUpdateDataProcess(string arguments)
        {
            IWorkspace EDERDefaultVersion = null;
            IWorkspace EDERSUBDefaultVersion = null;

            IWorkspace TargetEDERVersion = null;
            IWorkspace TargetEDERSUBVersion = null;
            try
            {
                if (string.IsNullOrEmpty(arguments)) throw new Exception("Terminate the process as no arguments present ");

                arguments = arguments.Trim().ToUpper();
                //fdg
                UpdateOLDTRecords();

                CommonFuntions.WriteLine_Info("Update Status from OLD T-> A(Archieve) where capture date is " + ReadConfigurations.T_RETENTION_PERIOD.ToString() + " days old");
                // change the status-P->T
                Updatestatus();
                CommonFuntions.WriteLine_Info("Update Status from P-> T");


                if (ReadConfigurations.FORRecovery == "FALSE")
                {
                    // Get Default Workspace  
                    //Rename the temporary version.
                    // Create Temp Version 


                    if (arguments.Trim().ToUpper() == "EDER")
                    {
                        EDERDefaultVersion = clsgdbhelper.GetWorkspace(ReadConfigurations.SDEEDERConnectionString);
                        if (CheckOldProcessCompleted(arguments.Trim().ToUpper()))
                        {
                            TargetEDERVersion = clsgdbhelper.RollVersion(EDERDefaultVersion, ReadConfigurations.VersionName) as IWorkspace;

                        }
                        else
                        {
                            TargetEDERVersion = clsgdbhelper.FindVersion(EDERDefaultVersion as IVersionedWorkspace, ReadConfigurations.VersionName) as IWorkspace;
                        }
                        //reset the process state flag
                        UpdateOldProcessStatus(arguments, "0");


                        //Get all Insert
                        InsertTable = clsdbhelper.GetDataTable("select feat_oid,feat_classname from " + ReadConfigurations.pAHInfoTableName + "  where status='T'  and ACTION='I'");

                        //Update Insert case status-
                        UpdateInsertCaseStatus();
                        CommonFuntions.WriteLine_Info("Status of Insert cases is updated from T->C");


                        if (EDERDefaultVersion != null)
                        {
                            //Get versioned workspace

                            if (TargetEDERVersion != null)
                            {
                                CommonFuntions.WriteLine_Info("Process is started for EDER");
                                if (new LoadDifference().LoadDifferenceData(TargetEDERVersion as IVersion, EDERDefaultVersion as IFeatureWorkspace,"EDER"))
                                {
                                    CommonFuntions.WriteLine_Info("Process Completed Successfully for EDER .");
                                    UpdateOldProcessStatus(arguments, "1");
                                }
                            }
                            else
                            {
                                throw new Exception("<PGE_UPDATEOLDDATA> Version Not found ,Terminate the Process ");
                            }
                        }
                    }
                    else if (arguments.Trim().ToUpper() == "EDERSUB")
                    {
                        EDERSUBDefaultVersion = clsgdbhelper.GetWorkspace(ReadConfigurations.SDEEDERSUBConnectionString);
                        if (CheckOldProcessCompleted(arguments.Trim().ToUpper()))
                        {
                            TargetEDERSUBVersion = clsgdbhelper.RollVersion(EDERSUBDefaultVersion, ReadConfigurations.VersionName) as IWorkspace;

                        }
                        else
                        {
                            TargetEDERSUBVersion = clsgdbhelper.FindVersion(EDERSUBDefaultVersion as IVersionedWorkspace, ReadConfigurations.VersionName) as IWorkspace;
                        }
                        //reset the process state flag
                        UpdateOldProcessStatus(arguments, "0");
                        //Get all Insert
                        InsertTable = clsdbhelper.GetDataTable("select feat_oid,feat_classname from " + ReadConfigurations.pAHInfoTableName + "  where status='T'  and ACTION='I'");

                        //Update Insert case status-
                        UpdateInsertCaseStatus();
                        CommonFuntions.WriteLine_Info("Status of Insert cases is updated from T->C");


                        // Get Default Workspace  

                        if (EDERSUBDefaultVersion != null)
                        {

                            //Get versioned workspace
                            if (TargetEDERSUBVersion != null)
                            {
                                CommonFuntions.WriteLine_Info("Process is started for EDERSUB ");
                                if (new LoadDifference().LoadDifferenceData(TargetEDERSUBVersion as IVersion, EDERSUBDefaultVersion as IFeatureWorkspace,"EDERSUB"))
                                {
                                    CommonFuntions.WriteLine_Info("Process Completed Successfully for EDERSUB.");
                                    UpdateOldProcessStatus(arguments, "1");

                                    }
                            }
                            else
                            {
                                throw new Exception("<PGE_UPDATEOLDDATA> Version Not found ,Terminate the Process ");
                            }
                        }
                    }

                }
                else
                {
                    if (new LoadDifference().LoadDifferenceData(null, null,string.Empty))
                    {
                        CommonFuntions.WriteLine_Info("Process Completed Successfully for those whose delete information is not found .");

                    }
                }

            }
            catch (Exception ex)
            { }
        }
        private static bool UpdateOldProcessStatus(string processname, string value)
        {
            bool oldprocess = false;
            try
            {
                string squery = string.Empty;
                if (processname == "EDER")
                {
                    squery = "update  " + ReadConfigurations.EderConfigTable + " set Value = '" + value + "' where key='EDER_LD_STATUS'";

                }
                else if (processname == "EDERSUB")
                {
                    squery = "update  " + ReadConfigurations.EderConfigTable + " set Value = '" + value + "' where key='EDERSUB_LD_STATUS'";
                }
                clsdbhelper.UpdateQuery(squery);

            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception while checking the old process status " + ex.ToString());
            }
            return oldprocess;
        }

        private static bool CheckOldProcessCompleted(string processname)
        {
            bool oldprocess = false;

            try
            {
                string squery = string.Empty;
                if (processname == "EDER")
                {
                    squery = "select Value from " + ReadConfigurations.EderConfigTable + " where key='EDER_LD_STATUS'";

                }
                else if (processname == "EDERSUB")
                {
                    squery = "select Value from " + ReadConfigurations.EderConfigTable + " where key='EDERSUB_LD_STATUS'";
                }
                DataRow dr = clsdbhelper.GetSingleDataRowByQuery(squery);
                if (dr["Value"].ToString() == "1")
                {
                    oldprocess = true;
                }

            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception while checking the old process status " + ex.ToString());
            }
            return oldprocess;
        }

        private static void UpdateOLDTRecords()
        {
            try
            {
                
                System.DateTime result = DateTime.Now.AddDays(-Convert.ToInt32(ReadConfigurations.T_RETENTION_PERIOD));
                string strquery = "update " + ReadConfigurations.pAHInfoTableName + "  set status = 'A'  WHERE  Capture_Date  < '" + result.ToString("MM-dd-yyyy HH:mm:ss") + "' and Status = 'T' ";
                clsdbhelper.UpdateQuery(strquery);
            }
            catch { }
        }

        private static void UpdateDuplicateStatus()
        {
            try
            {
            //  string strquery=  "update " + ReadConfigurations.pAHInfoTableName + "  set status = 'L' " +
            //        " where objectid in (WITH CTE AS(" 
            //      + " SELECT FEAT_GLOBALID, FEAT_OID, FEAT_CLASSNAME, VERSIONNAME, action, objectid,"
            //    //  + " ROW_NUMBER() OVER(PARTITION BY FEAT_OID, FEAT_CLASSNAME, VERSIONNAME,ACTION ORDER BY FEAT_OID,"
            //    //  + " FEAT_CLASSNAME, VERSIONNAME, action) as RN FROM " + ReadConfigurations.pAHInfoTableName + ") " +
            //    //  " select objectid FROM CTE WHERE RN > 1)" ;
            //    //clsdbhelper.UpdateQuery(strquery);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// change status from posted-P to transition->T
        /// </summary>
        private static void Updatestatus()
        {
            string strUpdateQuery = "Update " + ReadConfigurations.pAHInfoTableName + " set Status='T' where status='P' ";
            clsdbhelper.UpdateQuery(strUpdateQuery);

        }
        /// <summary>
        /// Change the status from T->C for Insert Records
        /// </summary>
        private static void UpdateInsertCaseStatus()
        {

            try
            {
                string strUpdateQuery = "Update " + ReadConfigurations.pAHInfoTableName + " set Status='C' " +
                    " where ((ACTION ='I') and (status='T') and (Feat_GlobalID is not null)) ";
                clsdbhelper.UpdateQuery(strUpdateQuery);


                strUpdateQuery = "Update " + ReadConfigurations.pAHInfoTableName + " set Status='C' " +
                    " where ( status='T' and (FEAT_CLASSNAME like ('%ANNO%') or Feat_ClassName in ('EDGIS.ELECTRICDISTNETWORK_JUNCTIONS'))) ";
                clsdbhelper.UpdateQuery(strUpdateQuery);



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Check Same day Insert/Update, Update/Delete or Insert/Delete
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="featureWorkspace"></param>
        /// <returns></returns>
        private bool CheckSamedayEdits(DataRow dr, IFeatureWorkspace featureWorkspace)
        {
            bool samedayrecord = false;
            try
            {
                string strquery = "select feat_oid, count(feat_oid) from " +  ReadConfigurations.pAHInfoTableName + " where status in ('T', 'C') group by(feat_oid) having count(feat_oid)> 1";
                DataTable DT= clsdbhelper.GetDataTable(strquery);
                foreach (DataRow DuplicateRow in DT.Rows)
                {
                    strquery = "select dsitinct status,feat_oid from " + ReadConfigurations.pAHInfoTableName + " where status in ('T', 'C') and feat_oid= " + Convert.ToInt32(DuplicateRow[0]);
                    DataTable DuplicateDetailRecords = clsdbhelper.GetDataTable(strquery);
                    //if

                }


            }
            catch (Exception ex)
            { }
            return samedayrecord;
        }

        /// <summary>
        /// Roll Versions
        /// </summary>
        /// <param name="sdB"></param>
        /// <param name="DailyTempVersion"></param>
        public static void RollVersions(string sdB, IVersion2 DailyTempVersion)
        {
            try
            {
                //IVersion2 versDailyChange = TargetVersion as IVersion2;
                string versionNameToReplace = ReadConfigurations.VersionName;

              //  KillConnections();

                //Delete daily change version.
                //_logger.Info("Deleting version (" + versionNameToReplace + ").");
                // This should reconnect
                IVersionedWorkspace vWorkspace = (IVersionedWorkspace)clsgdbhelper.GetWorkspace(sdB);
                IVersion2 versDailyChange = (IVersion2)vWorkspace.FindVersion(versionNameToReplace);



                try
                {
                    //Delete old Version

                    DailyTempVersion.VersionName = versionNameToReplace + "_bkp";
                    versDailyChange.Delete();


                    versDailyChange = null;
                    DailyTempVersion.VersionName = versionNameToReplace;
                    //CommonFuntions.WriteLine_Info("  Renamed version " + VersionTempName + " to " + versionNameToReplace + ".");

                }
                catch (Exception ex) { }
                //versDailyChangeTemp.Access = esriVersionAccess.esriVersionAccessProtected;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Issue rolling versions forward: " + ex.Message + " at " + ex.StackTrace);
            }
        }
        //private static void KillConnections()
        //{
        //    if (SourceVersion != null)
        //    {
        //        Marshal.FinalReleaseComObject(SourceVersion);
        //        SourceVersion = null;
        //    }

        //    if (TargetVersion != null)
        //    {
        //        Marshal.FinalReleaseComObject(TargetVersion);
        //        TargetVersion = null;
        //    }

        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //}


        #region InitializeLicense
        private static void InitializeLicense()
        {

            Common CommonFunctions = new Common();
            try
            {
                if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced, esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                 new esriLicenseExtensionCode[] { }))
                {
                    System.Console.WriteLine(m_AOLicenseInitializer.LicenseMessage());
                    System.Console.WriteLine("This application could not initialize with the correct ArcGIS license and will shutdown.");
                    m_AOLicenseInitializer.ShutdownApplication();
                    return;
                }

                //Check out Telvent license
                if (CheckOutArcFMLicense(mmLicensedProductCode.mmLPArcFM) != mmLicenseStatus.mmLicenseCheckedOut)
                {
                    CommonFuntions.WriteLine_Error("This application could not initialize with the correct ArcFM license and will shutdown.");
                    return;
                }
                CommonFuntions.WriteLine_Info("License initialized successfully.");
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while intializing the ESRI/ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        private static mmLicenseStatus CheckOutArcFMLicense(mmLicensedProductCode productCode)
        {
            Common CommonFunctions = new Common();
            try
            {
                if (_appInitialize == null) _appInitialize = new MMAppInitializeClass();

                var licenseStatus = _appInitialize.IsProductCodeAvailable(productCode);
                if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
                {
                    licenseStatus = _appInitialize.Initialize(productCode);
                }

                return licenseStatus;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while Checking out the ArcFM license." + ex.Message.ToString());
                throw ex;
            }
        }
        #endregion
    }
}
