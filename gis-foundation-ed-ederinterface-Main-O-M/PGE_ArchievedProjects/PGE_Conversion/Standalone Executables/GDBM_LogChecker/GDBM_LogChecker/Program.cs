using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GDBM_LogChecker
{
    class Program
    {
        static string _LogFilePath = @"D:\GeodatabaseManager\GDBM_Logs\DataProcessing_08.log",// @"\\edgisbtcprd01\GDBM_Logs\DataProcessing_08.log",
            _BatchRestartGDBM = @"D:\edgisdbmaint\GDBM_Hanged_RestartDP08.bat";
        static int _MinutesRecheckChange = 5,
            _MinutesRecheckNoChange = 5;
        static double _SizeNow = 0,
            _SizeNext = 0;
        static void Main(string[] args)
        {
            try
            {
                ReadParam(args);
                FileInfo pInfoNow = new FileInfo(_LogFilePath);

            recheck:
                //if (ExecuteOrExit())
                //    return;
                ExecuteOrExit();

                _SizeNow = pInfoNow.Length;
                Thread.Sleep(_MinutesRecheckChange * 60000);
                pInfoNow = new FileInfo(_LogFilePath);
                _SizeNext = pInfoNow.Length;

                if (!(_SizeNext > _SizeNow))
                {
                    Thread.Sleep(_MinutesRecheckNoChange * 60000);

                    pInfoNow = new FileInfo(_LogFilePath);
                    _SizeNext = pInfoNow.Length;
                    if (!(_SizeNext > _SizeNow))
                    {
                        Process.Start(_BatchRestartGDBM);
                    }
                }

                goto recheck;

            }
            catch
            {
            }
            finally
            {
            }
        }

        private static bool ExecuteOrExit()
        {
            try
            {
                if (DateTime.Now > Convert.ToDateTime("17:45:00"))// && DateTime.Now < Convert.ToDateTime("00:15:00"))
                {                    
                    Console.WriteLine(DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + "GDBM is not running now. Holding monitoring");
                    Thread.Sleep(Convert.ToDateTime("00:15:00").AddDays(1).Subtract(DateTime.Now));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static void ReadParam(string[] args)
        {
            string sArgs=string.Empty;
            for (int iCount = 0; iCount < args.Length; ++iCount)
            {
                sArgs = args[iCount].ToUpper();
                if (sArgs == "/LP")
                    _LogFilePath = args[iCount + 1].ToUpper();
                else if (sArgs == "/BR")
                    _BatchRestartGDBM = args[iCount + 1].ToUpper();
                else if (sArgs == "/MC")
                    _MinutesRecheckChange = Convert.ToInt32(args[iCount + 1]);
                else if (sArgs == "/MN")
                    _MinutesRecheckNoChange = Convert.ToInt32(args[iCount + 1]);
                else if (sArgs == "/?")
                {
                    Console.WriteLine("This tool is for monitoring GDBM log file update. Below are the options to be provided");
                    Console.WriteLine(@"/LP      Logfile path. [Default is " + _LogFilePath + "]");
                    Console.WriteLine(@"/BR      Batch file path to restart the DataProcessing 08 Services. [Default is " + _BatchRestartGDBM + "]");
                    Console.WriteLine(@"/MC      Minutes to check for logfile update. [Default is " + _MinutesRecheckChange.ToString() + " minutes]. Make sure MC + MN MUST be lesser than scheduled frequency");
                    Console.WriteLine(@"/MN      Minutes to wait if log is not updated. [Default is " + _MinutesRecheckNoChange.ToString() + " minutes]. Make sure MC + MN MUST be lesser than scheduled frequency");
                }
            }
        }
    }
}
