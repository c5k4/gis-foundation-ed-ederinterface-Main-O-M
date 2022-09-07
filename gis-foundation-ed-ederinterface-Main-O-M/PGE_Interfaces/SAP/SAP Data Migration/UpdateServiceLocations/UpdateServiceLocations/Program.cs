using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace UpdateServiceLocations
{
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        static void Main(string[] args)
        {
            try
            {
                VersionOperations._log.Info("");
                VersionOperations._log.Info("Service location replacement process is started.");

                if (args.Length == 0)
                {
                    VersionOperations verOp = new VersionOperations();
                    verOp.InitializeStaticVariables();
                    verOp.StartProcess("");
                    verOp.ReleaseStaticVariables();
                }
                else
                {
                    if (args[0] == "TEST")
                    {
                        VersionOperations._log.Info("Log is generating successfully.");
                    }
                    else
                    {
                        VersionOperations verOp = new VersionOperations();
                        verOp.InitializeStaticVariables();
                        verOp.StartProcess(args[0]);
                        verOp.ReleaseStaticVariables();
                    }
                }
            }
            catch (Exception exp)
            {
                // throw;
            }
            finally
            {
                VersionOperations._log.Info("");
                VersionOperations._log.Info("Service location replacement process is completed.");
                VersionOperations._log.Info("");
                VersionOperations._log.Info("");
                
                //Console.WriteLine("Press return to exit");
                //Console.ReadLine();
                TerminateProcess(Process.GetCurrentProcess().Handle, Convert.ToUInt32(Environment.ExitCode));
            }            
        }
    }
}
