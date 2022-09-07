using System;
using System.Collections.Generic;
using System.Text;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using System.Diagnostics;

namespace PGE.Common.Delivery.Process
{
    /// <summary>
    /// This class provides the necessary connection to the process framework and is only necessary outside of the ArcGIS Environment.
    /// </summary>
    public class ProcessManager
    {
        private string _ConnectionString;
        private IMMPxApplication _PxApp;

        /// <summary>
        /// Create Process Manager object used to connect to the process framework outside of ArcMap.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ProcessManager(string connectionString)
        {
            _ConnectionString = connectionString;
        }

        #region Public Properties

        /// <summary>
        /// Get if the px application started.
        /// </summary>
        public bool IsConnected
        {
            get { return (_PxApp != null); }
        }

        /// <summary>
        /// Get the px application core object.
        /// </summary>
        public IMMPxApplication Application
        {
            get
            {
                return _PxApp;
            }
        }

        /// <summary>
        /// Get the connection string used to connnect to the process framework.
        /// </summary>
        public string ConnectionString
        {
            get { return _ConnectionString; }
        }
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Connect to the process framework database. 
        /// </summary>
        public void Connect()
        {
            if (_PxApp == null)
            {
                ADODB.Connection connection = new ADODB.ConnectionClass();

                try
                {
                    int index = this.ConnectionString.IndexOf("Password");
                    EventLogger.Log("Connecting to the Process Framework: " + this.ConnectionString.Substring(0, index), EventLogEntryType.Information); 
                    //Logger.WriteLine(TraceEventType.Verbose, "Connecting to the Process Framework: " + this.ConnectionString);

                    // Open the connection with the given properties we can omitt the user name and password because it's in the connection string.
                    connection.Open(this.ConnectionString, "", "", (int)ADODB.ConnectOptionEnum.adConnectUnspecified);

                    // Create PxLogin object.
                    IMMPxLogin pxLogin = new PxLoginClass();
                    pxLogin.Connection = connection;

                    // Create PxApplication and start it up with the pxlogin object.
                    _PxApp = new PxApplicationClass();
                    _PxApp.Startup(pxLogin);
                }
                catch (Exception)
                {
                    // Clean up.
                    if (_PxApp != null)
                    {
                        _PxApp.Shutdown();

                        while ((Marshal.ReleaseComObject(_PxApp)) > 0)
                        {
                        }

                        _PxApp = null;
                    }

                    if (connection.State == (int)ADODB.ObjectStateEnum.adStateOpen)
                        connection.Close();

                    throw;
                }
            }
        }

        /// <summary>
        /// Disconnect from the px framework database. 
        /// </summary>
        public void Disconnect()
        {
            try
            {
                if (_PxApp != null)
                {
                    _PxApp.Shutdown();
                    _PxApp = null;

                    EventLogger.Log("Disconnecting from Process Framework and Px Application Shutdown...", EventLogEntryType.Information); 
                }
            }
            catch (Exception ex)
            {
                EventLogger.Error(ex);
            }
        }
        #endregion
    }
}
