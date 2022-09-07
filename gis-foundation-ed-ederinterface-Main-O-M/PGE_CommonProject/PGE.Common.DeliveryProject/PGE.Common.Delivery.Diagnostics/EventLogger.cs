using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Security.Principal;
using System.Configuration;

namespace PGE.Common.Delivery.Diagnostics
{
    /// <summary>
    /// Handles event logging.
    /// </summary>
    /// <remarks>Static members provide functionality to log messages to the Miner event log, 
    /// MessageBox, or combinations of these.</remarks>
    public static class EventLogger
    {
        #region Static Fields
        /// <summary>
        /// The source of the event log errors.
        /// Defaults to PGE.Common
        /// </summary>
        public static string Source = string.IsNullOrEmpty(ConfigurationManager.AppSettings["eventLogSource"])?"PGE.Common":ConfigurationManager.AppSettings["eventLogSource"];

        #endregion

        #region Private Members

        /// <summary>
        /// Returns the user currently logged in to Windows.
        /// </summary>
        private static string UserName
        {
            get
            {
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                return (wi != null) ? wi.Name : "";
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Warns the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void Warn(string message)
        {        
            WriteEntry(Source, "User: " + UserName + "\n\n" +
                message, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Displays a message box with the error and stack trace information and
        /// writes the information to the Application Event log.
        /// </summary>
        /// <param name="e">The Exception to log.</param>
        /// <param name="message">The message.</param>
        /// <param name="title">The title for the message box to display.</param>
        public static void Error(Exception e, string message, string title)
        {
            Error(e);
            MessageBox.Show(message + "\n\n" + e.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Displays a message box with the error and stack trace information and 
        /// writes the information to the Application Event log.
        /// </summary>
        /// <param name="e">The Exception to log.</param>
        /// <param name="title">The title for the message box to display.</param>
        public static void Error(Exception e, string title)
        {
            Error(e);
            MessageBox.Show(e.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
       
        /// <summary>
        /// Logs the exception information to the Application Event log.
        /// </summary>
        /// <param name="e">The Exception to log.</param>
        public static void Error(Exception e)
        {
            string message = e.ToString();

            if (e is COMException)
            {
                COMException com = (COMException)e;
                message = "Error Code: " + com.ErrorCode + "\n" + com;                
            }

            WriteEntry(Source, "User: " + UserName + "\n\n" + message + "\n\nStack Trace:\n\n" + e.StackTrace, EventLogEntryType.Error);
        }

        /// <summary>
        /// Logs a message to the Application Event log.
        /// </summary>
        /// <param name="source">The Source to be displayed in the Event log.</param>
        /// <param name="message">The message to be written to the log.</param>
        /// <param name="logtype">The type of message to be written to the log.</param>
        public static void Log(string source, string message, EventLogEntryType logtype)
        {
            message = "User: " + UserName + "\n\n" + message;            
            WriteEntry(source, message, logtype);
        }

        /// <summary>
        /// Logs a message to the Application Event log.
        /// </summary>
        /// <param name="message">The message to be written to the log.</param>
        /// <param name="logtype">The type of message to be written to the log.</param>
        public static void Log(string message, EventLogEntryType logtype)
        {
            message = "User: " + UserName + "\n\n" + message;
            WriteEntry(Source, message, logtype);
        }
        #endregion   
             
        #region Private Members

        /// <summary>
        /// Writes the entry into the event log.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="entryType">Type of the entry.</param>
        private static void WriteEntry(string source, string message, EventLogEntryType entryType)
        {
            try
            {                
                if (!EventLog.SourceExists(source)) // Make sure this event source is registered.
                    EventLog.CreateEventSource(source, "Miner"); // Register the source for this event log.

                EventLog.WriteEntry(source, message, entryType);                
            }
            catch(Exception ex)
            {                                
                Trace.WriteLine(ex);
            }        
        }
        #endregion
    }
}
