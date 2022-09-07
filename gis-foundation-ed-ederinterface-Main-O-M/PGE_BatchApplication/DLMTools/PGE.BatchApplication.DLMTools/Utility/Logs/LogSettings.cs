using System.IO;
using System.Reflection;
using log4net.Repository.Hierarchy;

namespace PGE.BatchApplication.DLMTools.Utility.Logs
{
    /// <summary>
    /// log4net normally stores settings in an XML file.
    /// This allows for less configuration possibilities outside of code but makes things easier overall with no config xml file attached to the DLL.
    /// The config file can definitely be added back in if desired.
    /// </summary>
    internal static class LogSettings
    {
        internal static readonly log4net.Core.Level Threshold = log4net.Core.Level.Info;
        internal static readonly string APPNAME = "EDER Auxiliary";
        //internal static readonly string APPNAME = "Miner";
        internal static readonly string LOGNAME = "Miner";

        internal static void SetupDefaultAppenders()
        {
            Log4NetFileHelper.Log4NetInitialized += new Log4NetFileHelper.Log4NetInitializedEventHandler(Log4NetFileHelper_Log4NetInitialized);
        }

        static void Log4NetFileHelper_Log4NetInitialized(Logger root)
        {
            try
            {
                // Configure the default appenders to add.
                string assemblyLoc = MethodBase.GetCurrentMethod().DeclaringType.Assembly.Location;

                // Add file logging
                Log4NetFileHelper.AddFileLogging(assemblyLoc + ".log");
                if (Threshold != log4net.Core.Level.Debug && File.Exists(assemblyLoc + ".debug.log"))
                    Log4NetFileHelper.AddFileLogging(assemblyLoc + ".debug.log", log4net.Core.Level.All);

                // Add event viewer logging
                Log4NetFileHelper.AddWindowsEventLogging();
            }
            catch { }
        }
    }
}
