using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Configuration;
using PGE_DBPasswordManagement;


namespace PGE.Interfaces.SAP.ExportToSAP
{
    class Program
    {
       
        //Private Variable 
        private static string startTime;
        private static StringBuilder remark = default;
        private static StringBuilder argument = default;
         
        //Public Variable 
        public static string comment = default;
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        public static string sdeConnFile = ReadEncryption.GetSDEPath(ReadConfigurations.GetValue(ReadConfigurations.EDWorkSpaceConnString));
        static void Main(string[] args)
        {
            try
            {
                startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                if (args.Length == 0)
                {
                    ExportToSAPProcess.PrepareAndSendStatusFileToSAP(0);
                    comment = "Process completed successfully";
                }
                else if (args.Length == 1 && args[0].ToUpper() == "CREATEINITIALVERSION")
                {
                    PGE.Interfaces.SAP.ExportToSAP.VersionOperations.PreRequiste();
                    PGE.Interfaces.SAP.ExportToSAP.CapturingGISUpdates.InitializeVersion();
                    comment = "Process completed successfully for CREATEINITIALVERSION";
                }
                else if (args.Length == 1 && args[0].ToUpper() == "CAPTUREVERSIONDIFFERENCE")
                {
                    ExportToSAPProcess.PrepareAndSendStatusFileToSAP(1);
                    comment = "Process completed successfully for CAPTUREVERSIONDIFFERENCE";
                }
                else if (args.Length == 1 && args[0].ToUpper() == "EXPORTDATA")
                {
                    ExportToSAPProcess.PrepareAndSendStatusFileToSAP(2);

                }
                else { comment = "Please verify the arguments passed"; }
                ExecutionSummary(startTime, comment, Argument.Done);

            }
            catch (Exception ex)
            {
                _log.Error("Exception occures while executing the ED16 Interface :" + args[0].ToUpper() + " "+ ex.Message);
                _log.Info(ex.Message + "   " + ex.StackTrace);
                Console.WriteLine("Exception occures while executing the ED16 Interface :" + args[0].ToUpper() + " " + ex.Message, ex.StackTrace);
                ExecutionSummary(startTime, ex.Message + comment, Argument.Error);
                ErrorCodeException ece = new ErrorCodeException(ex);
                Environment.ExitCode = ece.CodeNumber;
            }
        }

        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-V1t8
        /// </summary>
        /// <param name="startTime">string </param>
        /// <param name="comment">string</param>
        /// <param name="status">comment</param>
        private static void ExecutionSummary(string startTime, string comment, string status)
        {
            try
            {

                argument = new StringBuilder();
                remark = new StringBuilder();
                //Setting arguments
                argument.Append(Argument.Interface);
                argument.Append(Argument.Type );
                argument.Append(Argument.Integration);
                argument.Append(startTime + ";");
                remark.Append(comment);
                argument.Append(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ";");
                argument.Append(status);
                argument.Append(remark);

                //To execution for interface execution summary exe.This exe must need some input argument to run successffully. 
                ProcessStartInfo processStartInfo = new ProcessStartInfo(ConfigurationManager.AppSettings["IntExecutionSummaryExePath"], "\"" + Convert.ToString(argument) + "\"");
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
