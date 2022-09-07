using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.BatchApplication.ArcFmWebConfigLimited.Core
{
    public static class Logger
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "arcfmwebconfig");

        public static void Debug(string message)
        {
            _logger.Debug(message);
        }

        public static void Warn(string message)
        {
            _logger.Warn(message);
        }

        public static void Error(string message)
        {
            _logger.Error(message);
        }

        public static void Info(string message)
        {
            _logger.Info(message);
        }
    }
}