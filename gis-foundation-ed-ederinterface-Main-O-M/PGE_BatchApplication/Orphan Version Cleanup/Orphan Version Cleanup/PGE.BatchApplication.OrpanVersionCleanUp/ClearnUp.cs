using System;
using System.IO;
using System.Runtime.InteropServices;
using ADODB;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Miner.Interop.Process;

namespace Telvent.PGE.OrphanVersionCleanup
{
    /// <summary>
    /// This class is in charge of logging into the geodatabase and Px database and run the actual cleanup
    /// tools (the deleters) that are assigned to each px node.  Information about the cleanup process will be
    /// passed into a text log file as well as the console.
    /// 
    /// To set this up, you will need to make the following modifications
    /// 1.  The directory and file name of the log file
    /// 2.  The connection properties to log into the geodatabase
    /// 3.  The connection string for the Px database
    /// </summary>
    class CleanUp : IMMMessageCallback
    {
        StreamWriter _log;

        public void StartCleanup()
        {
            //Set your log file path and file name
            string logPath = "D:\\Temp\\AutomatedCleanupLog.txt";

            if (!File.Exists(logPath))
            {
                try
                {
                    _log = new StreamWriter(logPath);
                }
                catch (DirectoryNotFoundException dirExc)
                {
                    // Let the user know that the directory did not exist.
                    ProcessMessage(dirExc.Message + Environment.NewLine + "Logging will be shown in the console.");
                }
            }
            else
            {
                _log = File.AppendText(logPath);
            }

            ProcessMessage("\r\nStarting Automated Cleanup Tool: " + DateTime.Now);

            ProcessMessage("Running automated cleanup");
            RunCleanUp();
            ProcessMessage("Completed automated cleanup.");
            if (_log != null)
            {
                _log.Close();
            }
        }

        private void RunCleanUp()
        {
            try
            {
                //log into the geodatabase
                IWorkspace workspace = GetWorkspace();

                if (workspace == null)
                {
                    ProcessMessage("Could not connect to the Geodatabase! Exiting application.");
                    return;
                }

                IMMPxLogin pxLogin = new PxLoginClass();
                ADODB.Connection pxConnection = new ConnectionClass();

                //********Choose and modify one of the following Px Connection strings
                //SQL: 
                //string connString = "Provider=SQLOLEDB;Data Source=Your_Server_Name;Initial Catalog= Your_Database_Name;UserId=Your_Username;Password=Your_Password;";

                //Oracle:
                //string connString = "Provider=OraOLEDB.Oracle;Data Source=MyOracleDB;User Id=username;Password=password;";

                //Access: 
                //string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\Sample Data\\WorkflowManager.mdb;OLE DB Services=-2;";
                //********End Px Connection strings***********************************

                //Set the Px Login connection string
                pxLogin.ConnectionString = connString;

                //Log into and start process framework
                IMMPxApplication pxApp = new PxApplicationClass();
                pxApp.Startup(pxLogin);

                //Run the cleanup tools
                IMMPxApplicationEx2 pxAppEx2 = (IMMPxApplicationEx2)pxApp;
                pxAppEx2.Workspace = workspace;
                IMMPxCleanup pxCleanup = pxApp as IMMPxCleanup;
                pxCleanup.Cleanup(this);
                pxApp.Shutdown();
            }
            catch (Exception e)
            {
                ProcessMessage("Automated cleanup tool encountered an error and had to exit. Error: " + e.ToString());
            }
        }

        /// <summary>
        /// GetWorkspace allows the AutomatedCleanup tool to connect to the Geodatabase where the orphaned
        /// information exists. 
        /// </summary>
        /// <returns>The workspace the application connected to.</returns>
        private IWorkspace GetWorkspace()
        {
            IWorkspace workspace = null;
            try
            {
                IPropertySet connection = new PropertySetClass();

                //Enter your connection properties here
                connection.SetProperty("SERVER", "server_name");
                connection.SetProperty("INSTANCE", "server_instance");
                connection.SetProperty("USER", "username");  //uncomment this if you are not using OSA
                connection.SetProperty("PASSWORD", "password");  //uncomment this if you are not using OSA
                connection.SetProperty("VERSION", "SDE.DEFAULT"); //VERSION is case sensitive
                //connection.SetProperty("DATABASE", "Minerville"); //DATABASE property only needed for SQL Server SDE instances
                connection.SetProperty("AUTHENTICATION_MODE", "DBMS");  //set to "DBMS" to utilize normal login credentials; set to OSA for operating system credentials

                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                workspace = workspaceFactory.Open(connection, 0);

                Marshal.ReleaseComObject(workspaceFactory);
                Marshal.ReleaseComObject(connection);
            }
            catch { }
            return workspace;
        }

        private void ProcessMessage(string message)
        {
            if (_log != null)
            {
                _log.WriteLine(message);
            }
            Console.WriteLine(message);
        }

        #region IMMMessageCallback Members

        public void InitProgressBar(int lMin, int lMax)
        {

        }

        public int ProgressValue
        {
            set { ; }
        }

        public string StatusMessage
        {
            set { ; }
        }

        public bool Stopped
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Processes the message returned by the px node's Deleter.
        /// </summary>
        /// <param name="sMessage"></param>
        /// <param name="eMessageType"></param>
        public void UserMessage(string sMessage, mmUserMessageType eMessageType)
        {
            ProcessMessage(sMessage);
        }

        #endregion
    }
}
