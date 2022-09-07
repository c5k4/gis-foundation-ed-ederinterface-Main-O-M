#region Organized and sorted using
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;
using Miner.Interop;
using PGE_DBPasswordManagement;
#endregion

namespace PGE.BatchApplication.ReconcilVersions
{
    /// <summary>
    /// Class for Version Reconciliation.
    /// </summary>
    public class VersionReconcile
    {
        #region Private Variable

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ReconcileVersions.log4net.config");
        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static StreamWriter fileWriter = System.IO.File.AppendText(ConfigurationManager.AppSettings["Result_and_Exception_File"]);
        private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private static bool exitFlag = false;
        private static int dt = 0;
        private static int passResult = 0;
        private static int failResult = 1;
        private static int resultInt = failResult;

        #endregion

        #region Private Method

        /// <summary>
        ///  Run when the timer is raised.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private static void TimerEventProcessor(System.Object myObject, EventArgs myEventArgs)
        {
            dt += myTimer.Interval;
            try
            {
                myTimer.Stop();
                IWorkspace wokspace = OpenWorkspace();
                IVersion targetVersion = null;
                IVersion defaultVersion = VersionedWorkspace(wokspace, ref targetVersion);
                if (defaultVersion != null && targetVersion != null)
                {
                    ReconcileandPost(defaultVersion, targetVersion);
                    myTimer.Enabled = true;
                    // Stops the timer.
                    exitFlag = true;
                    Application.ExitThread();
                }
                else
                {
                    if ((dt / 60000) >= Convert.ToInt32(ConfigurationManager.AppSettings["maxAppRunningTime"]))
                    {
                        Console.WriteLine("Version workspace not found." + DateTime.Now + ". ");
                        fileWriter.WriteLine("Version workspace not found. " + DateTime.Now + ". ");
                        exitFlag = true;
                        Application.ExitThread();
                    }
                    else
                    {
                        myTimer.Enabled = true; // Now wait for given time and come again for loading the data.
                    }
                }
            }
            catch (Exception ex)
            {
                fileWriter.WriteLine("Error while executing StartReconcile(). At " + DateTime.Now + ". ");
                _logger.Debug("Error while executing StartReconcile(). At " + DateTime.Now + " ", ex);
            }
        }

        /// <summary>
        /// Reconcile the current version with target version.
        /// </summary>
        /// <param name="editVersion"></param>
        /// <param name="targetVersion"></param>
        private static void ReconcileandPost(IVersion editVersion, IVersion targetVersion)
        {
            try
            {
                IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)editVersion;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)editVersion;
                IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;

                if (muWorkspaceEdit.SupportsMultiuserEditSessionMode(esriMultiuserEditSessionMode.esriMESMVersioned))
                {
                    muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                    bool acquireLock = Boolean.Parse(ConfigurationManager.AppSettings["Reconcile4_AcquireLock"]);
                    bool abortIfConflicts = Boolean.Parse(ConfigurationManager.AppSettings["Reconcile4_AbortIfConflicts"]);
                    bool childWins = Boolean.Parse(ConfigurationManager.AppSettings["Reconcile4_ChildWins"]);
                    bool columnLevel = Boolean.Parse(ConfigurationManager.AppSettings["Reconcile4_ColumnLevel"]);

                    //Reconcile with the target version.
                    bool conflicts = versionEdit.Reconcile4(targetVersion.VersionName, acquireLock, abortIfConflicts, childWins, columnLevel);
                    if (conflicts)
                    {
                        if (abortIfConflicts)
                        {
                            resultInt = failResult;
                            _logger.Error(string.Format("Conflicts Detected between {0} and {1} {2} , edits will not be saved rolling back reconcile.", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                            fileWriter.WriteLine(string.Format("Conflicts Detected between {0} and {1} {2} , edits will not be saved rolling back reconcile.", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                            Console.WriteLine(string.Format("Conflicts Detected between {0} and {1} {2} , edits will not be saved rolling back reconcile.", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                            workspaceEdit.StopEditing(false);
                        }
                        else
                        {
                            resultInt = passResult;
                            _logger.Error(string.Format("Conflicts Detected between {0} and {1} {2} , however the AbortifConflicts is false, so will continue and save edits after reconcile, checking if post needed....", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                            fileWriter.WriteLine(string.Format("Conflicts Detected between {0} and {1} {2} , however the AbortifConflicts is false, so will continue and save edits after reconcile, checking if post needed....", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                            Console.WriteLine(string.Format("Conflicts Detected between {0} and {1} {2} , however the AbortifConflicts is false, so will continue and save edits after reconcile, checking if post needed....", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                            if (Boolean.Parse(ConfigurationManager.AppSettings["PostAfterReconcile"]))
                            {
                                workspaceEdit.StartEditOperation();
                                //Post to the target version.
                                if (versionEdit.CanPost())
                                {
                                    versionEdit.Post(targetVersion.VersionName);
                                    Console.WriteLine("Posted the current version to the reconciled version.");
                                    fileWriter.WriteLine("Posted the current version to the reconciled version.");
                                }
                                workspaceEdit.StopEditOperation();
                            }
                            workspaceEdit.StopEditing(true);
                        }
                    }
                    else
                    {
                        resultInt = passResult;
                        _logger.Error(string.Format("No Conflicts Detected between {0} and {1} {2} , saving the reconcile", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                        fileWriter.WriteLine(string.Format("No Conflicts Detected between {0} and {1} {2} , saving the reconcile", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                        Console.WriteLine(string.Format("No Conflicts Detected between {0} and {1} {2} , saving the reconcile", editVersion.VersionName, targetVersion.VersionName, DateTime.Now + ". "));
                        if (Boolean.Parse(ConfigurationManager.AppSettings["PostAfterReconcile"]))
                        {
                            workspaceEdit.StartEditOperation();
                            //Post to the target version.
                            if (versionEdit.CanPost())
                            {
                                versionEdit.Post(targetVersion.VersionName);
                                Console.WriteLine("Posted the current version to the reconciled version.");
                                fileWriter.WriteLine("Posted the current version to the reconciled version.");
                            }
                            workspaceEdit.StopEditOperation();
                        }
                        workspaceEdit.StopEditing(true);
                    }

                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while executing ReconcileandPost. At " + DateTime.Now + ". " + ex);
                fileWriter.WriteLine("Error while executing ReconcileandPost. At " + DateTime.Now + ". ");
            }
        }

        /// <summary>
        /// Get the target and edit version form Version Workspace
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="findVersion"></param>
        /// <returns></returns>
        private static IVersion VersionedWorkspace(IWorkspace workspace, ref IVersion findVersion)
        {
            try
            {
                if (workspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)workspace;
                    string targetVersion = ConfigurationManager.AppSettings["TargetVersion_Name"];
                    if (string.IsNullOrEmpty(targetVersion))
                    {
                        findVersion = versionedWorkspace.DefaultVersion;
                    }
                    else
                    {
                        findVersion = versionedWorkspace.FindVersion(targetVersion);
                    }
                    string versionEdit = ConfigurationManager.AppSettings["ReconcileVersion_Name"];
                    if (!string.IsNullOrEmpty(versionEdit))
                    {
                        //IEnumVersionInfo enumVersionInfo = versionedWorkspace.Versions;
                        //enumVersionInfo.Reset();
                        //IVersionInfo versionInfo = enumVersionInfo.Next();
                        //while (versionInfo != null)
                        //{
                        //    string versionName = versionInfo.VersionName;
                        //    //MessageBox.Show(" Version Name: " + versionName);
                        //    versionInfo = enumVersionInfo.Next();
                        //}
                        return versionedWorkspace.FindVersion(versionEdit);
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(" Error while executing VersionedWorkspace(). At " + DateTime.Now + ". " + ex);
                fileWriter.WriteLine("Error while executing VersionedWorkspace(). At " + DateTime.Now + ". ");
                return null;
            }
        }

        /// <summary>
        /// Open current workspace.
        /// </summary>
        /// <returns></returns>
        private static IWorkspace OpenWorkspace()
        {
            try
            {
                SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();

                // m4jf edgisrearch 919 - get sde file using Password management tool
                // return wsFactory.OpenFromFile(ConfigurationManager.AppSettings["SDE_FilePath"], 0);
                return wsFactory.OpenFromFile(ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper()), 0);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while executing VersionedWorkspace(). At " + DateTime.Now + ". " + ex);
                fileWriter.WriteLine("Error while executing OpenWorkspace(). At " + DateTime.Now + ". " + ex);
                return null;
            }
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Initialize the license and start timer event handler with provided timer settings.
        /// </summary>
        /// <param name="args"></param>
        public static int Main(string[] args)
        {
            try
            {
                bool esriLicenseCheckoutSuccess = false;
                bool arcfmLicenseCheckoutSuccess = false;
                _logger.Debug("Checking out licenses");
                try
                {
                    //ESRI License Initializer generated code.
                    esriLicenseCheckoutSuccess = m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { });
                    arcfmLicenseCheckoutSuccess = m_AOLicenseInitializer.GetArcFMLicense(mmLicensedProductCode.mmLPArcFM);
                }
                catch (Exception e)
                {
                    _logger.Error("Unable to check out licenses: Message: " + e.Message + ": StackTrace: " + e.StackTrace);
                    resultInt = failResult;
                    throw e;
                }

                if (esriLicenseCheckoutSuccess && arcfmLicenseCheckoutSuccess)
                {
                    // Adds the event and the event handler for the method that will process the timer event to the timer.
                    myTimer.Tick += new EventHandler(TimerEventProcessor);
                    // Sets the timer interval.               
                    myTimer.Interval = Convert.ToInt32(ConfigurationManager.AppSettings["VersionCheckInterval"].ToString());
                    myTimer.Start();

                    // Runs the timer, and raises the event. 
                    while (exitFlag == false)
                    {
                        // Processes all the events in the queue.
                        Application.DoEvents();
                    }
                    m_AOLicenseInitializer.ReleaseArcFMLicense();
                    m_AOLicenseInitializer.ShutdownApplication();
                    return resultInt;
                }
                else
                {
                    if (!esriLicenseCheckoutSuccess)
                    {
                        _logger.Error("Failed to check out Esri license");
                    }
                    else if (!arcfmLicenseCheckoutSuccess)
                    {
                        _logger.Error("Failed to check out ArcFM license");
                    }
                    return failResult;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error while executing StartReconcile(). At " + DateTime.Now + ". " + ex);
                return resultInt;
            }
            finally
            {
                fileWriter.Close();
                try { m_AOLicenseInitializer.ReleaseArcFMLicense(); }
                catch { }
                try { m_AOLicenseInitializer.ShutdownApplication(); }
                catch { }
            }
        }

        #endregion
    }
}
