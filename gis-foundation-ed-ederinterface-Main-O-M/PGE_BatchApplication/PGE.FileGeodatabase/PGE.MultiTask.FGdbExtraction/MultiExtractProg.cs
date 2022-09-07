using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Configuration;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.MultiTask.FGdbExtraction
{
    class MultiExtractProg
    {
        static string mstrFieldName = "division";

        [STAThread()]
        static int Main(string[] args)
        {
            //**This connects to Division FC, calls file geodatabase Extraction.exe for each division feature.
            Stopwatch stpWatch = new Stopwatch();
            stpWatch.Start();
            IFeatureCursor pFeatCur = null;
            ModFunctions.InitMasterPath();
            bool blnProcessFailed = false;
            try
            {               
                ModFunctions.gstrFgdbPath = ConfigurationManager.AppSettings["FGDB_PATH"];
                if (String.IsNullOrEmpty(ModFunctions.gstrFgdbPath))
                    throw new Exception("Unable to get FGDB path from config file. Exiting");

                if (ModFunctions.InitProcess() == false)
                    throw new Exception("Failed to initialise 'Service' procees. Please check error log for details.");

                string strExe = ConfigurationManager.AppSettings["Extract_Exe"];
                strExe = ModFunctions.gstrAssemblyPath + strExe;
                if (!System.IO.File.Exists(strExe))
                    throw new Exception("Failed to get extraction executable.");                
                
                string strSQL = ConfigurationManager.AppSettings["SQL_AOI"];
                IQueryFilter pQF = new QueryFilterClass();
                pQF.WhereClause = strSQL;// "objectid <100";
                pFeatCur = ModFunctions.gmAOI_FC.Search(pQF, false);
                IFeature pFeat = pFeatCur.NextFeature();
                IDataset pDataset = ModFunctions.gmAOI_FC as IDataset;
                Console.WriteLine(DateTime.Now.ToString() + " Start multi extraction process...");
                int i = 0;
                int intLimit = 30;
                int intWaitTime = 5;
                string[] argsValues = new string[2];                
                string strLimitNo = ConfigurationManager.AppSettings["LIMIT_NO_AOIs"];
                string strWaitTime = ConfigurationManager.AppSettings["WAIT_TIME_MINUTES"];
                
                if (!string.IsNullOrEmpty(strLimitNo))
                    intLimit = Convert.ToInt16(strLimitNo);
                if (!string.IsNullOrEmpty(strWaitTime))
                {
                    //**Limit wait time to 5 minutes.
                    intWaitTime = Convert.ToInt16(strWaitTime);
                    if (intWaitTime > 5)
                        intWaitTime = 5;
                }
                ModFunctions.LogMessage("Process limited to " + intLimit + " AOI processes.");
                List<System.Diagnostics.Process> processLst = new List<Process>();
                while (pFeat != null)
                {
                    //**For each feature call file geodatabase extraction process.                    
                    ModFunctions.LogMessage(pDataset.Name + " OID: " + pFeat.OID);
                    System.Diagnostics.Process process = new System.Diagnostics.Process();
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = strExe;
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    string strName = ModFunctions.GetFieldValue(pFeat, pFeat.Fields.FindField(mstrFieldName));
                    Console.WriteLine("Process starting for: " + strName);
                    if (!string.IsNullOrEmpty(strName))
                        strName = strName.Replace(" ", "-");
                    startInfo.Arguments = pFeat.OID.ToString() + " " + strName;
                    process.StartInfo = startInfo;
                    processLst.Add(process);
                    process.Start();

                    Thread.Sleep(((10000 * 6) * intWaitTime));
                    pFeat = pFeatCur.NextFeature();
                    i++;
                    if (i == intLimit) break;
                }

                //**This is required to get the return codes from the processes, which is required by UC4 environment to determine 
                //**if the process ran successfully.
                for (int y = 0; y < processLst.Count; y++)
                {
                    do
                    {
                        //if (processLst[y].HasExited)
                        //{                            
                        //}
                    }
                    while (!processLst[y].WaitForExit(1000));
                    if (processLst[y].ExitCode != 0)
                    {
                        blnProcessFailed = true;
                    }
                    if (processLst[y].HasExited)
                    {
                        // Refresh the current process property values.
                        //processLst[y].Refresh();
                        ModFunctions.LogMessage("Process '" + processLst[y].StartInfo.Arguments + "' complete.");
                        ModFunctions.LogMessage("\t Start time:     " + processLst[y].StartTime);
                        ModFunctions.LogMessage("\t Complete time:  " + processLst[y].ExitTime);
                        ModFunctions.LogMessage("\t Exit code:      " + processLst[y].ExitCode);
                        ModFunctions.LogMessage("");
                    }
                }
                TimeSpan tmSpan =stpWatch.Elapsed;
                TimeSpan span = stpWatch.Elapsed;
                string strTmElapsed = null;
                if (span.Days > 0)
                    strTmElapsed= string.Format("{0} days, {1} hours, {2} minutes", span.Days, span.Hours, span.Minutes);
                else if (span.Hours > 0)
                    strTmElapsed= string.Format("{0} hours, {1} minutes", span.Hours, span.Minutes);
                else 
                    strTmElapsed= string.Format("{0} minutes", span.Minutes);
                ModFunctions.LogMessage("Time to complete the process: " + strTmElapsed);
                if (blnProcessFailed)//**One or more process failed, return non-zero error code.
                    return (int)ExitCodes.Error;
                else
                    return (int)ExitCodes.Success;
            }

            catch (Exception Ex)
            { ModFunctions.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); return (int)ExitCodes.Error; ; }
            finally 
            {
                try
                {   //**Release memory.
                    while (Marshal.ReleaseComObject(pFeatCur) != 0) { }; 
                    pFeatCur = null;
                    ModFunctions.Dispose();                                          
                }
                catch (Exception Exf) { } 
            }            
        }
        enum ExitCodes : int
        {
            Success = 0,
            Error = 1,
            //SignToolNotInPath = 1,
            //AssemblyDirectoryBad = 2,
            //PFXFilePathBad = 4,
            //PasswordMissing = 8,
            //SignFailed = 16,
            //UnknownError = 32
        }
    }
}
