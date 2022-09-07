using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading; 
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop; 

namespace PGE.Interface.ENOS.Batch
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Assume success initially 
            Environment.ExitCode = (int)ENOSExitCodes.success;
            LicenseManager licenseManager = new LicenseManager();

            try
            {
                bool isChild = false;
                bool resetData = false;
                bool deleteVersions = false;
                int processIndex = 0;
                string childSPIDDigits = "";

                //Initialize ESRI and Schneider licenses 
                licenseManager.Initialize(ESRI.ArcGIS.esriSystem.esriLicenseProductCode.esriLicenseProductCodeAdvanced, mmLicensedProductCode.mmLPArcFM);


                //////////////////////////////////////////////////////////////////////////////
                //*****************ENOS USAGE AT THE COMMAND LINE **************************//
                //                                                                          //
                // 1) to run configuration and set connection properties using GUI:         //  
                // PGE.Interface.ENOS.Batch.exe config                                                //     
                //                                                                          //
                // 2) to run the parent process (which spawns the children per the config): //
                // PGE.Interface.ENOS.Batch.exe                                                       //    
                //                                                                          //
                // 3) to reset data - removes all generation records and sets GENCATEGORY   //
                //    field to 0 for all servicelocation features:                          //
                // PGE.Interface.ENOS.Batch.exe -r                                                    //
                //                                                                          //
                // 4) to delete all of the edit versions (including the edit version for    //  
                //    the parent process):                                                  //
                // PGE.Interface.ENOS.Batch.exe -d                                                    //
                //                                                                          //
                //////////////////////////////////////////////////////////////////////////////
                
                //Show config form 
                ENOSConfig pConfig = new ENOSConfig(); 
                if (args.Length != 0)
                {
                    if (args[0].ToLower() == "config")
                    {
                        frmConfig frm = new frmConfig(pConfig);
                        frm.ShowDialog();
                        return;
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].ToLower() == "-child")
                    {
                        isChild = true;
                    }
                    else if (args[i].ToLower() == "-i")
                    {
                        processIndex = Int32.Parse(args[i + 1]);
                    }
                    else if (args[i].ToLower() == "-l")
                    {
                        childSPIDDigits = args[i + 1];
                    }
                    else if (args[i].ToLower() == "-r")
                    {
                        resetData = true; 
                    }
                    else if (args[i].ToLower() == "-d")
                    {
                        deleteVersions = true;
                    }
                }

                //Get the configuration settings                
                ENOSHelper pENOSHelper = new ENOSHelper(
                    processIndex,
                    pConfig);
                pENOSHelper.Log("Beginning process with Index: " + processIndex.ToString());

                //Utility Functions************************************
                //These are utility functions used to reset the data, and 
                //To delete the edits versions 

                //Reset the data 
                if (resetData)
                {
                    pENOSHelper.ResetData();
                    return; 
                }

                //Delete the parent version (sometimes this does not delete properly) 
                if (deleteVersions)
                {
                    pENOSHelper.DeleteVersions();
                    return;
                }
                //End of Utility Functions*****************************
                


                if (isChild)
                {
                    //*****************************************************************
                    //Child process 
                    //Process this child 
                    pENOSHelper.Log("Calling ProcessChild");
                    pENOSHelper.ProcessChild(processIndex, childSPIDDigits);
                    //***************************************************************** 
                }                 
                else
                {
                    //***************************************************************** 
                    //Parent process

                    //Delete the child edit version and the parent version  
                    pENOSHelper.DeleteVersions();
                    
                    //Launch child processes 
                    pENOSHelper.Log("Parent process launching child processes: ");
                    Hashtable hshProcesses = new Hashtable();
                    for (int j = 0; j < pConfig.ChildProcesses.Length; j++)
                    {
                        ChildProcess pChildProc = pConfig.ChildProcesses[j];
                        hshProcesses.Add(pChildProc.ProcessIndex, SpawnProcess(
                            pChildProc.ProcessIndex, pChildProc.LastDigits, true, true,
                            ProcessWindowStyle.Normal, false));
                    }

                    //Monitor each of the the child processes until they all return 
                    pENOSHelper.Log("parent process monitoring child processes");
                    bool allExited = false;
                    while (!allExited)
                    {
                        allExited = true;
                        foreach (int processIdx in hshProcesses.Keys)
                        {
                            //pENOSHelper.Log("parent process monitoring child process: " + processIdx.ToString());
                            Process pProcess = (Process)hshProcesses[processIdx];

                            //Check to see if all child processes have exited 
                            if (!pProcess.HasExited)
                                allExited = false;
                            else
                            {
                                if (pProcess.ExitCode != (int)ENOSExitCodes.success)
                                {
                                    pENOSHelper.Log("child process: " + processIdx.ToString() + " has failed");
                                    Environment.ExitCode = (int)ENOSExitCodes.failure;
                                }
                                else
                                {
                                    pENOSHelper.Log("child process: " + processIdx.ToString() + " has exited successfully");
                                }
                            }
                        }
                        Thread.Sleep(10000);
                    }

                    //Throw an exception if child process returns failure code 
                    if (Environment.ExitCode != (int)ENOSExitCodes.success)
                        throw new Exception("Child process execution failed");

                    //Reconcile and post all child versions to SDE.DEFAULT
                    pENOSHelper.ReconcileAndPostAllVersions(hshProcesses); 

                    //Get the list of all servicepoints that have generation
                    //must use the default version
                    Hashtable hshSPsWithGeneration = pENOSHelper.GetServicePointsWithGeneration();

                    //Update the GenType field on the ServiceLocation 
                    pENOSHelper.UpdateServiceLocation(hshSPsWithGeneration);

                    //Reconcile and post the parent verion to SDE.DEFAULT
                    pENOSHelper.ReconcileAndPostParentVersion();

                    //Delete the child edit version and the parent version  
                    pENOSHelper.DeleteVersions();

                    //Make sure that all equipment records that are successfully loaded 
                    //are not in the ENOS_Error table - should be no longer required 
                    pENOSHelper.UpdateENOSError(pConfig); 
                    
                    //Archive records to ENOS_ARCHIVE
                    pENOSHelper.Archive();

                    //Delete records from ENOS_ARCHIVE older than MAX_ARCHIVE_AGE 
                    pENOSHelper.DeleteOldArchiveRecords();

                    //Delete all records from the ENOS_STAGE table
                    pENOSHelper.DeleteAllENOSStageRecords(); 
                }

                pENOSHelper.ReleaseResources(); 
            }
            catch (Exception ex)
            {
                //Return exit code of failure
                Environment.ExitCode = (int)ENOSExitCodes.failure;
            }
            finally
            {
                licenseManager.Shutdown(); 
            }
        }

        /// <summary>
        /// Create a child process 
        /// </summary>
        /// <param name="processID"></param>
        /// <param name="isChild"></param>
        /// <param name="createWindow"></param>
        /// <param name="windowStyle"></param>
        /// <param name="redirectOutput"></param>
        /// <returns></returns>
        private static Process SpawnProcess(
            int processIdx, 
            string lastDigits, 
            bool isChild,
            bool createWindow,
            ProcessWindowStyle windowStyle,
            bool redirectOutput)
        {
            string arguments = string.Empty; 
            if (isChild)
            {
                arguments += " -child";
                arguments += " -i " + processIdx.ToString();
                arguments += " -l " + lastDigits; 
            }

            ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
            {
                CreateNoWindow = createWindow,
                WindowStyle = windowStyle,
                UseShellExecute = false,
                RedirectStandardOutput = redirectOutput
            };
            return Process.Start(startInfo);
        }

    }
}