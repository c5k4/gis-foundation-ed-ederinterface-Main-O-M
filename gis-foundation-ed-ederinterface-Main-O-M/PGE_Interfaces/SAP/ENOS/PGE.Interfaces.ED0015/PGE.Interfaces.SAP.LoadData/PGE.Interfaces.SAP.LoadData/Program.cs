using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using PGE_DBPasswordManagement;
using System.Configuration;

namespace PGE.Interfaces.SAP.LoadData
{
    public class Program
    {

        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public static string comment = default;
        public static string startTime;
        private static StringBuilder remark = default;
        private static StringBuilder Argumnet = default;
        private static string InterfaceName = default;
        
        private static int dt = 0;
        public static string sdeConnFile = ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDEConnection"].ToUpper());
       
        static void Main(string[] args)
        {
            Environment.ExitCode = (int)ExitCodes.Success;
            startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

             // m4jf edgisrearch 919
            
            try
            {
                // Added to capture GIS updates - SPID,CircuitID and CGC updates to send back to SAP -- Added here Just for testing will be commented later 
                //PGE.Interfaces.SAP.LoadData.Classes.VersionOperations.PreRequiste();
                //PGE.Interfaces.SAP.LoadData.Classes.CapturingGISUpdates.StartVersionDifferenceProcess();

                // Added to make edits in main DB from stage 2 tables --- Added here Just for testing will be commented later 
                //StageTwoToMainDB.StartMainProcess();
                //Stage2ToMainTablesInSettings s = new Stage2ToMainTablesInSettings();
                //s.StartProcess();


                myTimer.Stop();
                if (args.Length == 0)
                {

                    if (!(SAPDailyInterfaceProcess.StartSAPDailyInterfaceProcess()))
                    {
                        Environment.ExitCode = (int)ExitCodes.Failure;
                    }
                }
                else if (args.Length == 1 && args[0].ToUpper() == "PROCESSAFTERSTAGE1LOAD")
                {
                    if (!(SAPDailyInterfaceProcess.InitializeForDMDataLoad()))
                    {
                        Environment.ExitCode = (int)ExitCodes.Failure;
                    }
                }
                else if (args.Length == 1 && args[0].ToUpper() == "PREPOSTPROCESS")
                {
                    // m4jf edgisrearch 416 - ed 15 improvements
                    InterfaceName = Argument.Interface_Part1 ;
                    if(!(SAPDailyInterfaceProcess.StartSAPDailyInterfaceProcess()))
                    {
                        ExecutionSummary(Program.startTime, Program.comment, Argument.Error);
                        Environment.ExitCode = (int)ExitCodes.Failure;
                    }
                }
                else if (args.Length == 1 && args[0].ToUpper() == "POSTPROCESS")
                {
                    // m4jf edgisrearch 416 - ed 15 improvements
                    InterfaceName = Argument.Interface_Part2 ;

                    if (!(StageTwoToMainDB.StartMainProcess()))
                    {
                        ExecutionSummary(Program.startTime, Program.comment, Argument.Error);
                        Environment.ExitCode = (int)ExitCodes.Failure;

                    }
                }
                else if (args.Length == 1 && args[0].ToUpper() == "LOADINSETTINGS")
                {
                    if(!(SAPDailyInterfaceProcess.ProcessDataAfterPost()))
                    {
                        Environment.ExitCode = (int)ExitCodes.Failure;
                    }
                }
            }
            catch (Exception exp)
            {
                _log.Error("Error in function Main. "+exp.Message);
                //Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
                Environment.ExitCode = (int)ExitCodes.Failure;
            }
            //Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
            Environment.Exit(Environment.ExitCode);
        }

        //Fix by: m4jf -EDGISREARCH 416 - Interface Improvement ED15
        public enum ExitCodes
        {
            Success,
            Failure
        };
        /// <summary>
        /// This method is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-m4jf
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="comment"></param>
        /// <param name="status"></param>
        public static void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {

                Argumnet = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments

                Argumnet.Append(InterfaceName);
                Argumnet.Append(Argument.Type);
                Argumnet.Append(Argument.Integration);
                Argumnet.Append(startTime + ";");
                remark.Append(comment);
                Argumnet.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                Argumnet.Append(status);
                Argumnet.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(ReadConfigurations.intExecutionSummary, "\"" + Convert.ToString(Argumnet) + "\"");
                processStartInfo.UseShellExecute = false;
                processStartInfo.RedirectStandardOutput = true;
                processStartInfo.CreateNoWindow = true;

                Process proc = new Process();
                proc.StartInfo = processStartInfo;
                proc.EnableRaisingEvents = true;
                proc.Start();
                proc.BeginOutputReadLine();
                while (!proc.HasExited)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _log.Error("Exception occures while executing the Interface Summary exe" + ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the Interface Summary exe", ex.StackTrace);
                //  fileWriter.WriteLine(" Error Occurred while executing the Interface Summary exe at " + DateTime.Now + " " + ex);
            }
        }

    }
}
