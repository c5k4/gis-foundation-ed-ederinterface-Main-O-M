using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace PGE.Common.Delivery.Diagnostics
{
    /// <summary>
    /// This static class is use in conjunction with the TraceListeners. It uses a SourceSwitch named LoggerSwitch to control the tracing level.
    /// </summary>
    public static class Logger
    {
        private static SourceSwitch _Switch = new SourceSwitch("LoggerSwitch", "All");

        /// <summary>
        /// Gets or sets the trace level.
        /// </summary>
        /// <value>The level.</value>
        public static SourceLevels Level
        {
            get { return _Switch.Level; }
            set { _Switch.Level = value; }
        }

        /// <summary>
        /// Writes the line to the trace listeners
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public static void WriteLine(TraceEventType eventType, string format, params object[] args)
        {
            WriteLine(eventType, string.Format(format, args));
        }

        /// <summary>
        /// Writes the line to the trace listeners
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public static void WriteLine(TraceEventType eventType, string message)
        {
            LogEntry logEntry = new LogEntry();
            logEntry.Severity = eventType;
            logEntry.Message = message;

            ProtectEntry(logEntry);

            if(_Switch.ShouldTrace(eventType))
                Trace.WriteLine(logEntry);                
        }

        /// <summary>
        /// Writes the specified event type.
        /// </summary>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="message">The message.</param>
        public static void Write(TraceEventType eventType, string message)
        {
            LogEntry logEntry = new LogEntry();
            logEntry.Severity = eventType;
            logEntry.Message = message;

            ProtectEntry(logEntry);

            if(_Switch.ShouldTrace(eventType))
                Trace.Write(logEntry);
        }

        /// <summary>
        /// Protects the entry.
        /// </summary>
        /// <param name="logEntry">The log entry.</param>
        private static void ProtectEntry(LogEntry logEntry)
        {
            if (logEntry == null) return;

            Match match = Regex.Match(logEntry.Message, @"\b(?:password|pwd)\b", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (match.Success) logEntry.Message = "##### Due to security restrictions (illegal words contained), the message to log has been discarded.";
        }

        /// <summary>
        /// Writes the line to the trace listeners
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void WriteLine(Exception ex)
        {
            string message = ex.ToString();

            if (ex is COMException)
            {
                COMException com = (COMException)ex;
                message = "Error Code: " + com.ErrorCode + "\n" + com;
            }

            LogEntry logEntry = new LogEntry();
            logEntry.Severity =  TraceEventType.Error;
            logEntry.Message = message;

            if(_Switch.ShouldTrace(logEntry.Severity))
                Trace.WriteLine(logEntry);
        }

        /// <summary>
        /// Writes the specified event type to the trace listeners
        /// </summary>
        /// <param name="ex">The ex.</param>
        public static void Write(Exception ex)
        {
            string message = ex.ToString();

            if (ex is COMException)
            {
                COMException com = (COMException)ex;
                message = "Error Code: " + com.ErrorCode + "\n" + com;
            }

            LogEntry logEntry = new LogEntry();
            logEntry.Severity = TraceEventType.Error;
            logEntry.Message = message + "\n\n" + ex.StackTrace;

            if(_Switch.ShouldTrace(logEntry.Severity))
                Trace.Write(logEntry);
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public static void Flush()
        {
            Trace.Flush();
        }
    }
}
