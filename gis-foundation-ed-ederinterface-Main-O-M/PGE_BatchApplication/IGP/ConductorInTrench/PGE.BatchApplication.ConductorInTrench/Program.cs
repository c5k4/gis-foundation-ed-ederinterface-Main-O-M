using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PGE.BatchApplication.ConductorInTrench
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        static void Main(string[] args)
        {
            Common._log.Info("Log is generating successfully.");           
            DateTime dtStart = DateTime.Now;
            string argProcess = null;
            try
            {
                MainClass._processtype = ConfigurationManager.AppSettings["Process_Name"];
                Common._log.Info("Process type is : " + MainClass._processtype);                

                if (args.Length == 1 && args[0] == "TESTLOG")
                {
                    return;
                }
                else if (args.Length == 1 && args[0] == "BULK_50SCALE")
                {
                    MainClass._processtype = "BULK_50SCALE";
                }
                else if (args.Length == 1 && args[0] == "FINDALLCHANGES")
                {
                    MainClass._processtype = "FINDALLCHANGES";
                }                
                else if (MainClass._processtype == "BULK")
                {                   
                    string strWhereClause = string.Empty;
                    if (args.Length > 0)
                    {
                        strWhereClause = args[0];
                    }

                    if (args.Length > 0 )
                    {
                        if (Convert.ToString(args[0]) == "LEFTRECORDS")
                        {
                            Bulk_Process._isLeftRecords = true;
                        }
                        else if (Convert.ToString(args[0]) == "RUNCHANGES")
                        {
                            Bulk_Process._isRunningProcessForChanges = true;
                        }
                    }

                    Common._log.Info("Process is started at :" + DateTime.Now);                 
                    bool bulkProcess = Bulk_Process.StartBulkProcess(strWhereClause);
                  
                    if (bulkProcess)
                    {
                        Common._log.Info("Process completed successfully.");
                    }
                    else
                    {
                        Common._log.Info("Error occured in process.");
                    }

                    Common._log.Info("Process is completed at :" + DateTime.Now);
                    Common._log.Info("Total Time Taken :" + (DateTime.Now - dtStart));
                }
                            
                //This process runs for schedule and command line arguments 
                else
                {
                    //DATAUPDATE//VERSIONPOST
                    if (args.Length > 0)
                    {
                        argProcess = args[0];

                        if (args.Length > 1 && args[1] == "RUNCHANGES")
                        {
                            MainClass._bisRunningProcessForChanges = true;
                            MainClass._queryToGetChangedRecordsBasedOnCounty = args[2];
                            Common._log.Info("The process is running for " + args[0] + " and " + args[1] + " for query : " + args[2]);
                        }                    
                    }

                    dtStart = DateTime.Now;
                    Common._log.Info("Process is started at :" + DateTime.Now);

                    bool pProcess = MainClass.StartProcess(argProcess);

                    if (pProcess)
                    {
                        Common._log.Info("Process completed successfully.");
                    }
                    else
                    {
                        Common._log.Info("Error occured in process.");
                    }

                    Common._log.Info("Process is completed at :" + DateTime.Now);
                    Common._log.Info("Total Time Taken :" + (DateTime.Now - dtStart));
                }

                if (MainClass._processtype == "FINDALLCHANGES")
                {
                    // Function will get all possible conductors for which filledduct value will be calculated
                    string tableForChangedPriUGCond = ConfigurationManager.AppSettings["tableForChangedPriUGCond"];
                    string tableForAllChangedPriUGCond = ConfigurationManager.AppSettings["tableForAllChangedPriUGCond"];

                    bool bProcess = Bulk_Process.StartProcessForCapturingAllpossibleCOnductorsForChangedConductors(tableForChangedPriUGCond, tableForAllChangedPriUGCond);

                    if (bProcess)
                    {
                        Common._log.Info("FINDALLCHANGES Process completed successfully.");
                    }
                    else
                    {
                        Common._log.Info("Error occured in process FINDALLCHANGES.");
                    }

                    Common._log.Info("FINDALLCHANGES Process is completed at :" + DateTime.Now);
                    Common._log.Info("Total Time Taken :" + (DateTime.Now - dtStart));

                }
                else if (MainClass._processtype == "BULK_50SCALE")
                {
                    bool bProcess = Bulk_Process.StartProcessForCapturing50ScaleData();
                    
                    if (bProcess)
                    {
                        Common._log.Info("BULK_50SCALE Process completed successfully.");
                    }
                    else
                    {
                        Common._log.Info("Error occured in process BULK_50SCALE.");
                    }

                    Common._log.Info("BULK_50SCALE Process is completed at :" + DateTime.Now);
                    Common._log.Info("Total Time Taken :" + (DateTime.Now - dtStart));
                }
                else if (MainClass._processtype == "CAPTURE_CONDUCTORS_FOR_ALL_COUNTY")
                {
                    bool bProcess = Bulk_Process.StartProcessForCapturingAllDataForCounties();

                    if (bProcess)
                    {
                        Common._log.Info(MainClass._processtype +" Process completed successfully.");
                    }
                    else
                    {
                        Common._log.Info("Error occured in process " + MainClass._processtype + ".");
                    }

                    Common._log.Info(MainClass._processtype +" Process is completed at :" + DateTime.Now);
                    Common._log.Info("Total Time Taken :" + (DateTime.Now - dtStart));
                }

            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message +" |  Stack Trace | " + exp.StackTrace+" at "+exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                Common.ReleaseStaticVariables();
                TerminateProcess(Process.GetCurrentProcess().Handle, Convert.ToUInt32(Environment.ExitCode));
            }
        }
    }
}
