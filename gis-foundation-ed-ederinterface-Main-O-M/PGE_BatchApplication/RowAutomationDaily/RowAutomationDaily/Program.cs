using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Net.Mail;
using PGE_DBPasswordManagement;
namespace PGE.BatchApplication.RowAutomationDaily
{
    class Program
    {
        #region private static variables
        private static LicenseInitializer m_AOLicenseInitializer = new PGE.BatchApplication.RowAutomationDaily.LicenseInitializer();
        private static IWorkspace workspace = default(IWorkspace);
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_FILE_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOG_FILE_PATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static List<string> objectIdList = new List<string>();
        private static string WHERE_CLAUSE = string.Empty;

        // m4jf edgisrearch 919 - get sde file using ssword management tool
        //private static string SDE_FILE_PATH = ConfigurationManager.AppSettings["SDE_FILE_PATH"];

        private static string SDE_FILE_PATH = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["LANDBASE_SDEConnection"].ToUpper());

        private static int ROW_GROUP_SIZE = Convert.ToInt16(ConfigurationManager.AppSettings["ROW_GROUP_SIZE"]);
        private static string ROW_OLDER_VERSION = ConfigurationManager.AppSettings["ROW_OLDER_VERSION"];
        private static string ROW_UPDATE_VERSION = ConfigurationManager.AppSettings["ROW_UPDATE_VERSION"];
        private static string DATASET_NAME = "LBGIS.PGE_Common_Landbase";
        private static string FEATURE_CLASS_NAME = "LBGIS.LotLines";
        private static string GDB_FILE_NAME = "DAILY_PROCESS_GDB"; 
        private static string LOG_FILE_PATH = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOG_FILE_PATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOG_FILE_PATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static string curr_loc = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string GDB_FILE_PATH = curr_loc + "\\" + GDB_FILE_NAME;
        private static string mailhost = ConfigurationManager.AppSettings["mailhost"];
        private static string INCLUDE = ConfigurationManager.AppSettings["INCLUDE"];
        private static string EMAIL_TO = ConfigurationManager.AppSettings["EMAIL_TO"];
        private static string EMAIL_FROM = ConfigurationManager.AppSettings["EMAIL_FROM"];
        private static string MSG_BODY = ConfigurationManager.AppSettings["MSG_BODY"];

        #endregion
        /// <summary>
        /// main method
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            string arguments = string.Empty;
            IVersionedWorkspace pVersionedWorkspace = null;
            IVersion pDefaultVersion = null;
            IVersion pRowOlderVersion = null;
            IVersion pRowUpdateVersion = null;
            IVersion pNewOlderVersion = null;
            IVersion pExistingUpdateVersion = null;

            try
            {
                //create log file
                createLogFile();
                WriteLine("START");
                WriteLine("Initializing License");
                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                  new esriLicenseExtensionCode[] { });
                
                WriteLine("Getting workspace from SDE connection file. ");
                workspace = OpenSDEWorkspacefromsdefile(SDE_FILE_PATH);
                pVersionedWorkspace = workspace as IVersionedWorkspace;
                pDefaultVersion = pVersionedWorkspace.DefaultVersion;
                //old version
                try
                {
                    pRowOlderVersion = pVersionedWorkspace.FindVersion(ROW_OLDER_VERSION);
                    WriteLine("Version found - " + ROW_OLDER_VERSION);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Version not found"))
                    {
                        WriteLine("Assuming no previous ROW version exists, Creating a new version from default version");
                        VersionCreate(pVersionedWorkspace, pDefaultVersion, ROW_OLDER_VERSION);
                        WriteLine("Version created: " + ROW_OLDER_VERSION + " as child of SDE.Default.");
                        pRowOlderVersion = pVersionedWorkspace.FindVersion(ROW_OLDER_VERSION);
                    }
                }
                //update version
                try
                {
                    pExistingUpdateVersion = pVersionedWorkspace.FindVersion(ROW_UPDATE_VERSION);
                    WriteLine("Version found - " + ROW_UPDATE_VERSION);
                    if (pExistingUpdateVersion != null)
                    {
                        pExistingUpdateVersion.Delete();
                        WriteLine("Version deleted - " + ROW_UPDATE_VERSION);
                    }
                    pRowUpdateVersion = pDefaultVersion.CreateVersion(ROW_UPDATE_VERSION);
                    pRowUpdateVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                    WriteLine("Version created: " + ROW_UPDATE_VERSION + " as child of Default.");

                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Version not found"))
                    {

                        VersionCreate(pVersionedWorkspace, pDefaultVersion, ROW_UPDATE_VERSION);
                        WriteLine("Version created: " + ROW_UPDATE_VERSION + " as child of Default");
                        pRowUpdateVersion = pVersionedWorkspace.FindVersion(ROW_UPDATE_VERSION);
                        //WriteLine("process completed at " + DateTime.Now.ToString() + " .");
                    }
                }
                WriteLine("Comparing " + ROW_OLDER_VERSION + " with SDE.Default");
                IFeatureClass pFcLotLines_old_version = ((IFeatureWorkspace)pRowOlderVersion).OpenFeatureClass("LBGIS.LotLines");
                IFeatureClass pFcLotLines_default_version = ((IFeatureWorkspace)pDefaultVersion).OpenFeatureClass("LBGIS.LotLines");
                OutputDifferences((ITable)pFcLotLines_old_version, (ITable)pFcLotLines_default_version, esriDifferenceType.esriDifferenceTypeUpdateUpdate);
                OutputDifferences((ITable)pFcLotLines_old_version, (ITable)pFcLotLines_default_version, esriDifferenceType.esriDifferenceTypeUpdateNoChange);
                OutputDifferences((ITable)pFcLotLines_old_version, (ITable)pFcLotLines_default_version, esriDifferenceType.esriDifferenceTypeInsert);
                OutputDifferences((ITable)pFcLotLines_old_version, (ITable)pFcLotLines_default_version, esriDifferenceType.esriDifferenceTypeDeleteNoChange);
                OutputDifferences((ITable)pFcLotLines_old_version, (ITable)pFcLotLines_default_version, esriDifferenceType.esriDifferenceTypeDeleteUpdate);
                OutputDifferences((ITable)pFcLotLines_old_version, (ITable)pFcLotLines_default_version, esriDifferenceType.esriDifferenceTypeUpdateDelete);

                if (objectIdList.Count == 0)
                {
                    WriteLine("No version differences were found between SDE.DEFAULT and SDE." + ROW_OLDER_VERSION);
                    return;
                }

                int loopCount = (objectIdList.Count / ROW_GROUP_SIZE) + 1;
                int index = 0, c = 0;
                string pyLogFile = string.Empty;
                System.Diagnostics.Process proc = new System.Diagnostics.Process();

                for (c = 0; c < loopCount; c++)
                {
                    WHERE_CLAUSE = "";

                    for (; index < ROW_GROUP_SIZE * (c + 1); index++)
                    {
                        if (index == objectIdList.Count)
                        {
                            break;
                        }
                        if (WHERE_CLAUSE.Length == 0)
                            WHERE_CLAUSE = objectIdList[index].ToString();
                        else
                            WHERE_CLAUSE = WHERE_CLAUSE + "," + objectIdList[index].ToString();
                    }
                    if (WHERE_CLAUSE != null)
                    {
                        proc.StartInfo.FileName = curr_loc + "\\" + "DailyProcess.py";
                        proc.StartInfo.UseShellExecute = true;
                        pyLogFile = curr_loc + "\\Log_DailyProcess.txt";
                        proc.StartInfo.Arguments = SDE_FILE_PATH + " " + curr_loc + " " + GDB_FILE_NAME + " " + pyLogFile + " " + WHERE_CLAUSE + " SDE." + ROW_UPDATE_VERSION;
                        proc.Start();
                        proc.WaitForExit();
                        if (Reconcileandpostchanges(pRowUpdateVersion, true) == true)
                        {
                            WriteLine(ROW_UPDATE_VERSION + " posted successfully to SDE.default.");
                            //row update version
                            try
                            {
                                if (pRowUpdateVersion != null)
                                {
                                    pRowUpdateVersion.Delete();
                                    WriteLine("Version deleted - " + ROW_UPDATE_VERSION);
                                }
                            }
                            catch(Exception ex)
                            {
                                WriteLine("Error in Deleting version - " + ROW_UPDATE_VERSION + " " + ex.Message);
                                
                            }
                            //row older version
                            try
                            {

                                if (workspace != null) Marshal.FinalReleaseComObject(workspace);
                                if (pVersionedWorkspace != null) Marshal.FinalReleaseComObject(pVersionedWorkspace);
                                if (pRowOlderVersion != null) Marshal.FinalReleaseComObject(pRowOlderVersion);
                                if (pFcLotLines_old_version != null) Marshal.FinalReleaseComObject(pFcLotLines_old_version);
                                System.GC.Collect();
                                workspace = OpenSDEWorkspacefromsdefile(SDE_FILE_PATH);
                                pVersionedWorkspace = workspace as IVersionedWorkspace;
                                pRowOlderVersion = pVersionedWorkspace.FindVersion(ROW_OLDER_VERSION);
                                pRowOlderVersion.Delete();
                                WriteLine("Version deleted - " + ROW_OLDER_VERSION);
                                pDefaultVersion = pVersionedWorkspace.DefaultVersion;
                                pNewOlderVersion = pDefaultVersion.CreateVersion(ROW_OLDER_VERSION);
                                pNewOlderVersion.Access = esriVersionAccess.esriVersionAccessPublic;
                                WriteLine("Version created: " + ROW_OLDER_VERSION + " as child of SDE.Default.");
                            }
                            catch (Exception ex)
                            {
                                WriteLine("Error in Deleting version - " + ROW_OLDER_VERSION + " Error - " + ex.Message);
                                WriteLine("Reconciling " + ROW_OLDER_VERSION + " with default");
                                Reconcileandpostchanges(pRowOlderVersion, false);
                            }
                           
                        }
                        else
                        {
                            WriteLine("\nROW update version could not be posted to default. Aborting the process. ");
                            WriteLine(ROW_OLDER_VERSION  + " has not been reconciled with todays Default Version.");
                            WriteLine(ROW_UPDATE_VERSION + " will be deleted in the next run and recreated from the version differnece between " + ROW_OLDER_VERSION  + " Default.");
                            WriteLine("Sending Email ");
                            WriteLine("EMAIL_FROM : " + EMAIL_FROM);
                            WriteLine("EMAIL_TO : " + EMAIL_TO);
                            WriteLine("MSG_BODY : " + MSG_BODY);
                            SendEmail(INCLUDE, EMAIL_TO, EMAIL_FROM, MSG_BODY);
                            return;
                        }
                    }
                }
                
                WriteLine("Daily Process completed.");

            }

            catch (Exception ex)
            {
                WriteLine("ERROR " + ex.Message);
            }
            finally
            {
                WriteLine("");
                WriteLine("Finally");
                if (workspace != null) Marshal.FinalReleaseComObject(workspace);
                if (pNewOlderVersion != null) Marshal.FinalReleaseComObject(pNewOlderVersion);
                if (pDefaultVersion != null) Marshal.FinalReleaseComObject(pDefaultVersion);
                if (pVersionedWorkspace != null) Marshal.FinalReleaseComObject(pVersionedWorkspace);
                if (pRowOlderVersion != null) Marshal.FinalReleaseComObject(pRowOlderVersion);
                if (pRowUpdateVersion != null) Marshal.FinalReleaseComObject(pRowUpdateVersion);
                if (pExistingUpdateVersion != null) Marshal.FinalReleaseComObject(pExistingUpdateVersion);
                WriteLine("END");
            }
        }
        public static Boolean Reconcileandpostchanges(IVersion EditVersion, bool shouldPost)
        {
            IWorkspaceEdit pwEdit = null;
            pwEdit = EditVersion as IWorkspaceEdit;
            if (pwEdit.IsBeingEdited())
            {
                pwEdit.StopEditing(false);
            }
            pwEdit.StartEditing(true);
            try
            {
                // Reconcile and post version here
                IVersion pChildVersion = EditVersion as IVersion;
                IVersionEdit4 pVersionEdit = EditVersion as IVersionEdit4;
                // Acquire locks first, if lock not acquired, then wait in loop, try 10-15 times
                bool IsVersionPosted = false;
                int iRetryCount = 0;
                while (iRetryCount <= Convert.ToInt32(10))
                {
                    if (!pVersionEdit.Reconcile4(pChildVersion.VersionInfo.Parent.VersionName, true, false, false, true))
                    {
                        if (shouldPost == true)
                        {
                            if (pVersionEdit.CanPost())
                            {
                                pVersionEdit.Post(pChildVersion.VersionInfo.Parent.VersionName);
                                IsVersionPosted = true;
                                break;
                            }
                        }
                        else
                        {
                            WriteLine("Version Reconciled -" + pChildVersion.VersionName);
                            break;
                        }
                    }
                    else
                    {
                        iRetryCount++;
                        WriteLine("Version reconcile retry count " + iRetryCount.ToString());

                    }
                }

                if (iRetryCount > Convert.ToInt32(10))
                {
                    WriteLine("Could not reconcile/post version - " + pChildVersion.VersionName);
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (pwEdit.IsBeingEdited())
                {
                    pwEdit.StopEditing(false);
                }
                WriteLine("Could not reconcile/post version - " + ex.Message);
                return false;
            }
            finally
            {
                if (pwEdit != null)
                {
                    if (pwEdit.IsBeingEdited())
                    {
                        pwEdit.StopEditing(true);
                    }
                }
            }
            return true;
        }

        public static void SendEmail(string include, string emailTo, string emailFrom, string strMsgBody)
        {
            bool Flag = false;

            try
            {
                DateTime localDate = DateTime.Now;
                string txtEmailTo = emailTo;
                string EmailFrom = emailFrom;

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(EmailFrom);
                if (include == "0")
                    msg.To.Add(new MailAddress(txtEmailTo));
                else
                {
                    msg.CC.Add(new MailAddress(EmailFrom));
                    msg.To.Add(new MailAddress(txtEmailTo));
                }
                msg.Subject = "LBGIS ROW Daily Update Error ";
                msg.Body = strMsgBody;
                SmtpClient smtp = new SmtpClient(mailhost);
                smtp.UseDefaultCredentials = true;
                smtp.Send(msg);
                Flag = true;
            }
            catch (Exception exception)
            {
                WriteLine("Error in sending email. " + exception.Message);
            }
            finally
            {

            }
        }

        private static IWorkspace OpenSDEWorkspacefromsdefile(String connectionFile)
        {
            IWorkspace _returnWorkspace = null;
            try
            {
                _returnWorkspace = ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                    OpenFromFile(connectionFile, 0);
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error in Creating SDE Connection from -- : " + connectionFile + " Error Message -- " + ex.Message);

                _returnWorkspace = null;
                throw ex;
            }
            return _returnWorkspace;
        }
        private static Boolean Reconcile(IVersion editVersion, IVersion targetVersion)
        {

            IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)editVersion;
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)editVersion;
            IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;

            if (muWorkspaceEdit.SupportsMultiuserEditSessionMode
                (esriMultiuserEditSessionMode.esriMESMVersioned))
            {

                //Reconcile with the target version. 
                bool conflicts = true;
                while (conflicts)
                {
                    try
                    {
                        muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                        conflicts = versionEdit.Reconcile4(targetVersion.VersionName, false, false, false, true);
                    }
                    catch (System.Runtime.InteropServices.COMException excom)
                    {
                        if (excom.ErrorCode == -2147217137) //ESRI.ArcGIS.Geodatabase.fdoError.FDO_E_RECONCILE_VERSION_NOT_AVAILABLE 
                        {
                            WriteLine("The target version  " + targetVersion.VersionName + "  is currently being reconciled against some other version.");
                        }
                        else if (excom.ErrorCode == -2147217139) //FDO_E_VERSION_BEING_EDITED 
                        {
                            WriteLine("Reconcile not allowed as the version is being edited by another application.");
                        }
                        else if (excom.ErrorCode == -2147217146)//FDO_E_VERSION_NOT_FOUND 
                        {
                            WriteLine("reconcile failed.The version " + targetVersion.VersionName + " could not be located");
                        }

                        if (workspaceEdit.IsBeingEdited())
                            workspaceEdit.StopEditing(false);
                        conflicts = true;
                    }
                }
                if (workspaceEdit.IsBeingEdited())
                    workspaceEdit.StopEditing(true);
                return true;
            }
            return false;
        }
        public static void VersionCreate(IVersionedWorkspace versionedWorkspace, IVersion parentVersion, string newversionName)
        {

            IVersion version;
            version = parentVersion.CreateVersion(newversionName);
            //setting the versions access    
            version.Access = esriVersionAccess.esriVersionAccessPublic;
            //setting the versiones description
            version.Description = "For Lotline and Row Daily Update process";
        }

        /// <summary>
        /// create log file 
        /// </summary>
        public static void createLogFile()
        {
            (pSWriter = File.CreateText(sPath)).Close();
        }
        /// <summary>
        /// Write on console and log file
        /// </summary>
        /// <param name="sMsg"></param>
        private static void WriteLine(string sMsg)
        {
            sMsg = !String.IsNullOrEmpty(sMsg) ? DateTime.Now.ToLongTimeString() + " -- " + sMsg : sMsg;
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
        /// <summary>
        /// Initialize licence
        /// </summary>
        //private static void initializeLicence()
        //{
        //    //ESRI License Initializer generated code.
        //    WriteLine("Initializing Licence");
        //    m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
        //    new esriLicenseExtensionCode[] { });
        //    mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
        //    if (RuntimeManager.ActiveRuntime == null)
        //        RuntimeManager.BindLicense(ProductCode.Desktop);

        //}
        /// <summary>
        /// get workspace from connection file
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <returns></returns>
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }
        /// <summary>
        /// ArcFM Licence Initializer
        /// </summary>
        /// <param name="productCode">Code to checkout</param>
        /// <returns>Licence Status</returns>
        //private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        //{
        //    mmLicenseStatus licenseStatus;
        //    licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
        //    if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
        //    {
        //        licenseStatus = arcFMAppInitialize.Initialize(productCode);
        //    }
        //    return licenseStatus;
        //}
        public static void OutputDifferences(ITable sourceTable, ITable differenceTable, esriDifferenceType differenceType)
        {
            IVersionedTable versionedTable = (IVersionedTable)sourceTable;
            IDifferenceCursor differenceCursor = versionedTable.Differences(differenceTable, differenceType, null);
            IRow differenceRow = null;
            int objectID;

            try
            {
                differenceCursor.Next(out objectID, out differenceRow);
                //Detect and output all the differences between the two versions
                int count = 0;
                while (objectID != -1)
                {
                    // MessageBox.Show(" feature " + objectID + " has been modified");
                    //IWorkspace gdb_Workspace = FileGdbWorkspaceFromPropertySet(GDB_FILE_PATH);
                    //IFeatureClass edited_lotlines_fc = ((IFeatureWorkspace)gdb_Workspace).OpenFeatureClass("EDITED_LOTLINES_FC");
                    //IFeature feat_lotline = edited_lotlines_fc.CreateFeature();
                    if (!(objectIdList.Contains(objectID.ToString())))
                        objectIdList.Add(objectID.ToString());
                    differenceCursor.Next(out objectID, out differenceRow);
                    //featGeom = ((IFeature)differenceRow).Shape;
                    //ITopologicalOperator topo = featGeom as ITopologicalOperator;
                    //unionGeom = topo.Union(unionGeom);
                    count++;
                }
            }
            catch (Exception ex)
            {
                WriteLine("Error in finding output difference - " + ex.Message);
            }
            finally
            {
                //if (versionedTable != null) Marshal.FinalReleaseComObject(versionedTable);
                //if (differenceCursor != null) Marshal.FinalReleaseComObject(differenceCursor);
                //if (differenceRow != null) Marshal.FinalReleaseComObject(differenceRow);
            }

        }
        //public static IWorkspace FileGdbWorkspaceFromPropertySet(string database)
        //{
        //    IPropertySet propertySet = new PropertySetClass();
        //    propertySet.SetProperty("DATABASE", database);
        //    IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
        //    return workspaceFactory.Open(propertySet, 0);
        //}

    }

}
