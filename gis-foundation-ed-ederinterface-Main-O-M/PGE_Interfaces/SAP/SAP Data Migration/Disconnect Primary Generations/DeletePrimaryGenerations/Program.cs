using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace DeletePrimaryGenerations
{
    class Program
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            try
            {
                VersionOperations._log.Info("");
                VersionOperations._log.Info("");
                VersionOperations._log.Info("Process to disconnect Primary generation started at : " + start);

                if (args.Length == 0)
                {                    
                    VersionOperations verOp = new VersionOperations();
                    verOp.StartProcess();
                }
                else
                {
                    if (args[0] == "TEST")
                    {
                        VersionOperations._log.Info("Log is generating successfully.");
                    }                                  
                }                
            }
            catch (Exception exp)
            {
                //  throw;
            }
            finally
            {
                VersionOperations._log.Info("");
                VersionOperations._log.Info("Process to disconnect Primary generation completed at : " + DateTime.Now);
                VersionOperations._log.Info("Total time taken : " + (DateTime.Now - start)); 
                
                //Console.WriteLine("Press return to exit");
                //Console.ReadLine();
                TerminateProcess(Process.GetCurrentProcess().Handle, Convert.ToUInt32(Environment.ExitCode));
                VersionOperations._log.Info("");
                VersionOperations._log.Info("");
            }            
        }
    }
}
