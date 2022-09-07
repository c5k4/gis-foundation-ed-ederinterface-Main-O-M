using System;
using System.IO;
using System.Xml;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.CircuitProtectionZone.Common
{
    /// <summary>
    /// Summary description for LogFile.
    /// </summary>
    public class Logger
    {
        private static Log4NetLogger logger = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.CircuitProtectionZoneCL.log4net.config");
        private static bool on = true;

        #region Public Static Members


        /// <summary>
        /// Turn the logger on/off for unit testing
        /// </summary>
        public static bool On
        {
            get { return on; }
            set { on = value; }
        }

        public static void Debug(string message, bool logToConsole)
        {
            try
            {
                if (logToConsole)
                {
                    Writeln("Debug", message);
                }
                if (logger.IsDebugEnabled == true)
                {
                    logger.Debug(message);
                }
            }
            catch (Exception ex)
            {
                throw new LogException("Error Writing to Log File. " + ex.Message);
            }
        }

        public static void Info(string message, bool logToConsole)
        {
            try
            {
                if (logToConsole)
                {
                    Writeln("Info", message);
                }
                if (logger.IsInfoEnabled == true)
                {
                    logger.Info(message);
                }
            }
            catch (Exception ex)
            {
                throw new LogException("Error Writing to Log File. " + ex.Message);
            }
        }

        public static void Error(string message, bool logToConsole)
        {
            try
            {
                if (logToConsole)
                {
                    Writeln("Error", message);
                }
                logger.Error(message);
            }
            catch (Exception ex)
            {
                throw new LogException("Error Writing to Log File. " + ex.Message);
            }
            return;
        }

        public static void Error(string message, Exception ex, bool logToConsole)
        {
            try
            {
                if (logToConsole)
                {
                    Writeln("Error", message + " Exception: " + ex.Message);
                }
                logger.Error(message, ex);
                if (ex.InnerException != null)
                {
                    logger.Error("Inner Exception: ", ex.InnerException);
                }
            }
            catch (Exception e)
            {
                throw new LogException("Error Writing to Log File. " + e.Message);
            }
            return;
        }

        #endregion Public Static Members

        private static void Writeln(string level, string message)
        {
            if (on == true)
            {
                Console.Out.WriteLine( String.Format("{0} ({1}) {2}" , 
                    DateTime.Now.ToString("hh:mm:ss:ff"), level, message) );
                Console.Out.Flush();
            }
        }
    }

    public class LogException : Exception
    {
        public LogException(string message) : base(message)
        {
        }

        public LogException(string message, Exception ex) : base(message, ex)
        {
        }
    }
}