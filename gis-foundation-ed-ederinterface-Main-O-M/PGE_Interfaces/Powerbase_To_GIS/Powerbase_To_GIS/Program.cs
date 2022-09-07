using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Powerbase_To_GIS;
using System.Configuration;
using PGE_DBPasswordManagement;
using System;

namespace Powerbase_To_GIS
{
    class Program
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        public static string comment = default;
        public static string startTime;
        private static StringBuilder remark = default;
        public static string Errorcomment = string.Empty;
        private log4net.ILog Logger = null;
        private static StringBuilder Argumnet = default;


        private static int dt = 0;
        static void Main(string[] args)
        {
            try
            {
                startTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                MainClass.StartProcess();


            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                Program.Errorcomment = "Exception" + exp.Message.ToString();
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                if (string.IsNullOrEmpty(Program.comment))
                {
                    Program.comment = "PROCESS Completed Successfully .";
                    ExecutionSummary(Program.startTime, Program.comment, ReadConfigurations.Argument.Done);
                }
                else
                {
                    ExecutionSummary(Program.startTime, Program.comment, ReadConfigurations.Argument.Error);
                }
            }
        }
        /// <summary>
        /// This methos is to use for log the interface success or fail in interface summary table for EDGIS Rearch Project-V1t8
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
                Argumnet.Append(ReadConfigurations.Argument.Interface);
                Argumnet.Append(ReadConfigurations.Argument.Type);
                Argumnet.Append(ReadConfigurations.Argument.Integration);
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
                //

            }
        }

    }


}