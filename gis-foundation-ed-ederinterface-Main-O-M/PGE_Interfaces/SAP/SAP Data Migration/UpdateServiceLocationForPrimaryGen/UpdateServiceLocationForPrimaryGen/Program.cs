using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

// ENOS2EDGIS - Code changes for ENOS to SAP migration
namespace PGE.Interfaces.UpdateServiceLocationForPrimaryGen
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    DateTime dtStart = DateTime.Now;

                    Common._log.Info("Service location update for gen category as primary is started at :" + DateTime.Now);

                    bool pProcess = MainClass.StartProcess();

                    if (pProcess)
                    {
                        Common._log.Info("Service location update for gen category as primary completed successfully.");
                    }
                    else
                    {
                        Common._log.Info("Error occured in Service location update for gen category as primary process.");
                    }

                    Common._log.Info("Service location update for gen category completed at :" + DateTime.Now);

                    Common._log.Info("Total Time Taken :" + (DateTime.Now - dtStart));
                }
                else
                {
                    if (args[0] == "TEST")
                    {
                        Common._log.Info("Log is generating successfully.");
                    }
                }
            }
            catch (Exception exp)
            {

            }
            finally
            {
                //Console.WriteLine("Press return to exit");
                //Console.ReadLine();
                TerminateProcess(Process.GetCurrentProcess().Handle, Convert.ToUInt32(Environment.ExitCode));
            }  
        }
    }
}
