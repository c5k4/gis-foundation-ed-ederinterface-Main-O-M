using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace ETGIS_AssetSync_Process
{
    /// <summary>
    ///     Logging utility wraps up external logging frameworks so other classes do not have to deal with it
    /// </summary>
    public class Log4NetLogger
    {
        /// <summary>
        /// </summary>
        protected static bool _configured;

        /// <summary>
        /// </summary>
        protected static bool _failed;

        /// <summary>
        /// </summary>
        protected static string _repository = "PGECUSTOM";

        /// <summary>
        /// </summary>
        protected static string _file = "\\ETGISAssetSync.log4net.config";

        /// <summary>
        /// </summary>
        protected static string _fileUsed;

        /// <summary>
        /// </summary>
        protected static ILoggerRepository _repo;

        /// <summary>
        /// </summary>
        protected ILog _logger;

        /// <summary>
        ///     Instantiate a logger
        /// </summary>
        /// <param name="source">The name this logger will use</param>
        public Log4NetLogger(Type source)
            : this(source, _file, _repository)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        public Log4NetLogger(string name)
            : this(name, _file, _repository)
        {
        }

        /// <summary>
        /// Instantiate a logger
        /// </summary>
        /// <param name="source">The name this logger will use</param>
        /// <param name="logFileName">Name of the log file (not including path)</param>
        public Log4NetLogger(Type source, string logFileName)
            : this(source, logFileName, _repository)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// /// <param name="logFileName">Name of the log file (not including path)</param>
        public Log4NetLogger(string name, string logFileName)
            : this(name, logFileName, _repository)
        {

        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <param name="repo"></param>
        public Log4NetLogger(string name, string file, string repo)
        {
            //we only need to configure once
            if (!_configured && !_failed)
            {
                configure(file, repo);
            }
            _logger = LogManager.GetLogger(_repository, name);
        }

        /// <summary>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="file"></param>
        /// <param name="repo"></param>
        public Log4NetLogger(Type source, string file, string repo)
        {
            //we only need to configure once
            if (!_configured && !_failed)
            {
                configure(file, repo);
            }
            _logger = LogManager.GetLogger(_repository, source);
        }

        /// <summary>
        /// </summary>
        protected static bool Configured
        {
            get { return _configured; }
        }

        /// <summary>
        /// </summary>
        protected static string FileUsed
        {
            get { return _fileUsed; }
        }

        /// <summary>
        ///     Check is debug is enabled
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }

        /// <summary>
        ///     Check if error is enabled
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return _logger.IsErrorEnabled; }
        }

        /// <summary>
        ///     Check if fatal is enabled
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return _logger.IsFatalEnabled; }
        }

        /// <summary>
        ///     Check if info is enabled
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return _logger.IsInfoEnabled; }
        }

        /// <summary>
        ///     Check if warn is enabled
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return _logger.IsWarnEnabled; }
        }

        /// <summary>
        /// </summary>
        /// <param name="file"></param>
        /// <param name="repo"></param>
        protected void configure(string file, string repo)
        {
            _repository = repo;
            _repo = LogManager.CreateRepository(_repository);
            var loc = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            String path = Path.GetDirectoryName(loc.LocalPath);

            var configfile = new FileInfo(Path.Combine(path, file.Replace("\\", "")));
            if (configfile.Exists)
            {
                _fileUsed = configfile.FullName;
                XmlConfigurator.ConfigureAndWatch(_repo, configfile);
                _configured = true;
            }
            else
            {
                _failed = true;
            }


        }

        /// <summary>
        /// </summary>
        public static void ConfigureLog()
        {
            var loc = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            String path = Path.GetDirectoryName(loc.LocalPath);
            ConfigureLog(path, _file, _repository);
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        public static void ConfigureLog(string path)
        {
            ConfigureLog(path, _file, _repository);
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        public static void ConfigureLog(string path, string file)
        {
            ConfigureLog(path, file, _repository);
        }

        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="repo"></param>
        public static void ConfigureLog(string path, string file, string repo)
        {
            if (!_configured)
            {
                if (_repo == null) //if it was already created before we don't want to loose any loggers
                {
                    _repository = repo;
                    _repo = LogManager.CreateRepository(_repository);
                }
                var configfile = new FileInfo(path + file);
                _fileUsed = configfile.FullName;
                XmlConfigurator.ConfigureAndWatch(_repo, configfile);
                _configured = true;
            }
        }

        /// <summary>
        ///     Log a debug message. If debug is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Debug(string message, Exception exception)
        {
            _logger.Debug(message, exception);
        }

        /// <summary>
        ///     Log a debug message. If debug is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        /// <summary>
        ///     Log an error message. If error is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        /// <summary>
        ///     Log a error message. If error is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Error(string message)
        {
            _logger.Error(message);
        }

        /// <summary>
        ///     Log a fatal message. If fatal is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }

        /// <summary>
        ///     Log a fatal message. If fatal is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        /// <summary>
        ///     Log an info message. If info is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Info(string message, Exception exception)
        {
            _logger.Info(message, exception);
        }

        /// <summary>
        ///     Log a info message. If info is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Info(string message)
        {
            _logger.Info(message);
        }

        /// <summary>
        ///     Log a warn message. If warn is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        /// <summary>
        ///     Log a warn message. If warn is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}