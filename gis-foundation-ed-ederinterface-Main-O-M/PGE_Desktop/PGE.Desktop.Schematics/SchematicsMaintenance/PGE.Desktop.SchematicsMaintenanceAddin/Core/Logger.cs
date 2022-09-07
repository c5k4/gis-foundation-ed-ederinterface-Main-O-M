using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;

namespace PGE.Desktop.SchematicsMaintenance.Core
{
    /// <summary>
    /// Log4net helper to allow for a single point to log from.
    /// </summary>
    public static class Logger
    {
        private const string _repositoryName = "PGE";
        private const string _configName = "log4net.config";

        private static readonly ILog _log = null;

        static Logger()
        {
            // Get a file reference to the logging config file 
            var executionLocation = new FileInfo(Assembly.GetExecutingAssembly().Location);

            var log4NetConfigurationFileInfo =
                new FileInfo(Path.Combine(executionLocation.Directory.ToString(),
                                          _configName));

            // Configure the repository using the specific config file
            XmlConfigurator.Configure(LogManager.CreateRepository(_repositoryName),
                log4NetConfigurationFileInfo);

            _log = LogManager.GetLogger(_repositoryName, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Log4net logging object
        /// </summary>
        public static ILog Log
        {
            get
            {
                return _log;
            }
        }
    }
}
