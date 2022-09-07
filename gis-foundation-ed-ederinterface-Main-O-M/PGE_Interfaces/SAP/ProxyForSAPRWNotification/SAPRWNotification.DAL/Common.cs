using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace PGE.ProxyForSAPRWNotification.DAL
{
    internal class Common
    {
        private static readonly Telvent.Delivery.Diagnostics.Log4NetLogger _logger = new Telvent.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static int OracleTimeout = 360;

        public static Common Lock = new Common();

        public static void WriteToLog(string content, LoggingLevel level)
        {
            if (level == LoggingLevel.Debug)
            {
                _logger.Debug(content);
            }
            else if (level == LoggingLevel.Error)
            {
                _logger.Error(content);
            }
            else if (level == LoggingLevel.Info)
            {
                _logger.Info(content);
            }
            else if (level == LoggingLevel.Warning)
            {
                _logger.Warn(content);
            }
        }

        public enum LoggingLevel
        {
            Error,
            Info,
            Debug,
            Warning
        }
    }
}