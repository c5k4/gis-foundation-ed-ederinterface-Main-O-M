using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using Miner.Geodatabase.GeodatabaseManager;
using Miner.Process.GeodatabaseManager;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;

namespace PGE.Desktop.EDER.GDBM
{
    public class MemoryCheck : PxActionHandler
    {

        public MemoryCheck()
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            this.Name = "PGE Process Memory Check";
            this.Description = "This action handler will check the status of memory usage for the process. If memory usage exceeds the " + 
                "configured amount, then it will force the process to restart itself";
        }

        public override bool Enabled(ActionType actionType, Actions gdbmAction, PostVersionType versionType)
        {
            return true;
        }

        private static bool RestartRequestIssued = false;
        int memoryThreshold = 1000;

        protected override bool PxSubExecute(PxActionData actionData, GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            try
            {
                foreach (GdbmParameter parameter in parameters)
                {
                    if (parameter.Name.ToUpper() == "MEMORYTHRESHOLD") { memoryThreshold = Int32.Parse(parameter.Value); }
                }
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Failed obtaining parameters: " + ex.Message);
                throw ex;
            }

            //Report current memory usage     
          
            Process currentProcess = Process.GetCurrentProcess();
            long workingSetMB = currentProcess.WorkingSet64 / 1000000;
            long peakWorkingSetMB = currentProcess.PeakWorkingSet64 / 1000000;
            this.Log.Info(ServiceConfiguration.Name, "Memory check started. Configured threshold is " + memoryThreshold + " MB");
            this.Log.Info(ServiceConfiguration.Name, "Current memory usage: " + workingSetMB + " MB");
            this.Log.Info(ServiceConfiguration.Name, "Peak memory usage: " + peakWorkingSetMB + " MB");

            if (memoryThreshold < peakWorkingSetMB)
            {
                this.Log.Info(ServiceConfiguration.Name, "Peak memory usage exceeds the configured amount. The process will now restart");

                //Write a new batch file for this service if needed.
                string FileName = Path + "MemoryRestart_" + ServiceConfiguration.Name + ".bat";
                if (!File.Exists(FileName))
                {
                    StreamWriter writer = new StreamWriter(FileName);
                    writer.WriteLine("net stop " + ServiceConfiguration.Name);
                    writer.WriteLine("timeout /t 35 /nobreak");
                    writer.WriteLine("net start " + ServiceConfiguration.Name);
                    writer.Flush();
                    writer.Close();
                }

                if (!RestartRequestIssued)
                {
                    this.Log.Info(ServiceConfiguration.Name, "Executing batch file to restart the service: " + FileName);
                    System.Diagnostics.Process.Start(FileName);
                    System.Threading.Thread.Sleep(10000);
                    RestartRequestIssued = true;
                }
            }
            else
            {
                this.Log.Info(ServiceConfiguration.Name, "Memory usage is below the configured limit. Processing will continue");
            }

            return true;
        }

        string _path = "";
        private string Path
        {
            get
            {
                if (String.IsNullOrEmpty(_path))
                {
                    try
                    {
                        //First check the path in the registry from the wix installers
                        string HKEY = @"Software\Miner and Miner\PGE";
                        RegistryKey _RegistryKey = Registry.LocalMachine.OpenSubKey(HKEY, false);
                        string path = _RegistryKey.GetValue("Directory").ToString();
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        _path = path;
                    }
                    catch { }

                    if (string.IsNullOrEmpty(_path))
                    {
                        Uri loc = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                        _path = System.IO.Path.GetDirectoryName(loc.LocalPath);
                    }
                }
                return _path;
            }
            set
            {
                _path = value;
            }
        }

    }
}
