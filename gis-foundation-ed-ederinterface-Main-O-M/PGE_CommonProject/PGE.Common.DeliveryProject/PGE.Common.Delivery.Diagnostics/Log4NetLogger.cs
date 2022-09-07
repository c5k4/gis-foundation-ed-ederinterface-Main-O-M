using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.IO;
using log4net.Repository;
using Microsoft.Win32;

namespace PGE.Common.Delivery.Diagnostics
{
    /// <summary>
    /// Logging utility wraps up external logging frameworks so other classes do not have to deal with it
    /// </summary>
    public class Log4NetLogger
    {
        /// <summary>
        /// 
        /// </summary>
        protected ILog _logger;
        /// <summary>
        /// 
        /// </summary>
        protected static bool _configured;
        /// <summary>
        /// 
        /// </summary>
        protected static bool _failed;
        /// <summary>
        /// 
        /// </summary>
        protected static string _repository = "TELCUSTOM";
        /// <summary>
        /// 
        /// </summary>
        protected static string _file = "";
        /// <summary>
        /// 
        /// </summary>
        protected static string _fileUsed;  
        /// <summary>
        /// 
        /// </summary>
        protected static ILoggerRepository _repo;

        /// <summary>
        /// 
        /// </summary>
        protected static bool Configured
        {
            get { return Log4NetLogger._configured; }
        }
        /// <summary>
        /// 
        /// </summary>
        protected static string FileUsed
        {
            get { return Log4NetLogger._fileUsed; }
        }

        public static string FileName
        {
            get
            {
                return _file;
            }
            set
            {
                _file = value;
            }
        }

        protected static string _path = "";
        public static string Path
        {
            get
            {
                if (String.IsNullOrEmpty(_path))
                {
                    try
                    {
                        //First check the path in the registry from the wix installers
                        string HKEY = @"Software\Miner and Miner\PGE";
                        RegistryKey _RegistryKey = Registry.LocalMachine.OpenSubKey(HKEY, false);
                        string path = _RegistryKey.GetValue("LoggingConfigurations").ToString();
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        _path = path;
                    }
                    catch { }

                    if (string.IsNullOrEmpty(_path))
                    {
                        Uri loc = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                        _path = System.IO.Path.GetDirectoryName(loc.LocalPath);
                    }
                }
                return _path;
            }
            set
            {
                _path = value;
            }
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
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="logFileName">Name of the log file (not including path)</param>
        /// <param name="repo"></param>
        public Log4NetLogger(string name, string file, string repo)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                if (string.IsNullOrEmpty(file))
                {
                    file = "\\PGE.Common.custom.log4net.config";
                    FileName = "\\PGE.Common.custom.log4net.config";
                }
                else
                {
                    if (file.StartsWith("\\")) { FileName = file; }
                    else
                    {
                        file = "\\" + file;
                        FileName = file;
                    }
                }
            }

            //we only need to configure once
            if (!_configured && !_failed)
            {
                configure(file, repo);
            }
            _logger = LogManager.GetLogger(_repository, name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="logFileName">Name of the log file (not including path)</param>
        /// <param name="repo"></param>
        public Log4NetLogger(Type source, string file, string repo)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                if (string.IsNullOrEmpty(file))
                {
                    file = "\\Default.log4net.config";
                    FileName = "\\Default.log4net.config";
                }
                else
                {
                    if (file.StartsWith("\\")) { FileName = file; }
                    else
                    {
                        file = "\\" + file;
                        FileName = file;
                    }
                }
            }

            //we only need to configure once
            if (!_configured && !_failed)
            {
                configure(file, repo);
            }
            _logger = LogManager.GetLogger(_repository, source);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="repo"></param>
        protected void configure(string file, string repo)
        {
            _repository = repo;
            _repo = LogManager.CreateRepository(_repository);
            FileInfo configfile = new FileInfo(Path + file);
            if (configfile.Exists)
            {
                _fileUsed = configfile.FullName;
                log4net.Config.XmlConfigurator.ConfigureAndWatch(_repo, configfile);
                _configured = true;
            }
            else
            {
                _failed = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static void ConfigureLog()
        {
            ConfigureLog(Path, FileName, _repository);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void ConfigureLog(string path)
        {
            ConfigureLog(path, FileName, _repository);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        public static void ConfigureLog(string path, string file)
        {
            ConfigureLog(path, file, _repository);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="repo"></param>
        public static void ConfigureLog(string path, string file, string repo)
        {
            if (!_configured)
            {
                if (_repo == null)//if it was already created before we don't want to loose any loggers
                {
                    _repository = repo;
                    _repo = LogManager.CreateRepository(_repository);
                }
                FileInfo configfile = new FileInfo(path + file);
                _fileUsed = configfile.FullName;
                log4net.Config.XmlConfigurator.ConfigureAndWatch(_repo, configfile);
                _configured = true;
            }
        }
        /// <summary>
        /// Log a debug message. If debug is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Debug(string message, Exception exception)
        {
            _logger.Debug(message, exception);
        }
        /// <summary>
        /// Log a debug message. If debug is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Debug(string message)
        {
            _logger.Debug(message);
        }
        /// <summary>
        /// Log an error message. If error is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }
        /// <summary>
        /// Log a error message. If error is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Error(string message)
        {
            _logger.Error(message);
        }
        /// <summary>
        /// Log a fatal message. If fatal is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(message, exception);
        }
        /// <summary>
        /// Log a fatal message. If fatal is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }
        /// <summary>
        /// Log an info message. If info is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Info(string message, Exception exception)
        {
            _logger.Info(message, exception);
        }
        /// <summary>
        /// Log a info message. If info is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Info(string message)
        {
            _logger.Info(message);
        }
        /// <summary>
        /// Check is debug is enabled
        /// </summary>
        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }
        }
        /// <summary>
        /// Check if error is enabled
        /// </summary>
        public bool IsErrorEnabled
        {
            get { return _logger.IsErrorEnabled; }
        }
        /// <summary>
        /// Check if fatal is enabled
        /// </summary>
        public bool IsFatalEnabled
        {
            get { return _logger.IsFatalEnabled; }
        }
        /// <summary>
        /// Check if info is enabled
        /// </summary>
        public bool IsInfoEnabled
        {
            get { return _logger.IsInfoEnabled; }
        }
        /// <summary>
        /// Check if warn is enabled
        /// </summary>
        public bool IsWarnEnabled
        {
            get { return _logger.IsWarnEnabled; }
        }
        /// <summary>
        /// Log a warn message. If warn is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="exception">The exception associated with this log entry</param>
        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message, exception);
        }
        /// <summary>
        /// Log a warn message. If warn is not enabled this will not do anything.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void Warn(string message)
        {
            _logger.Warn(message);
        }
    }
}
