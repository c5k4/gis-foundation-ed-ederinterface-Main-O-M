using System;
using log4net;
using log4net.Appender;
using log4net.Repository.Hierarchy;

namespace PGE.BatchApplication.DLMTools.Utility.Logs
{
    public class Log4NetFileHelper
    {
        Type _logType;
        ILog _logger = null;

        #region Static Variables and Methods
        static bool _initialized = false;
        static Logger root;
        static string DEFAULT_LOG_FILENAME = string.Format("application_log_{0}.log", DateTime.Now.ToString("yyyyMMMdd_hhmm"));

        public delegate void Log4NetInitializedEventHandler(Logger root);
        public static event Log4NetInitializedEventHandler Log4NetInitialized;

        public static void Init()
        {
            root = ((Hierarchy)LogManager.GetRepository()).Root;
            //root.AddAppender(GetConsoleAppender());
            //root.AddAppender(GetFileAppender(sFileName));
            root.Repository.Configured = true;

            LogSettings.SetupDefaultAppenders();

            if (Log4NetInitialized != null)
                Log4NetInitialized(root);
            _initialized = true;
        }


        #region Public Helper Methods
        #region Console Logging
        public static void AddConsoleLogging()
        {
            ConsoleAppender C = GetConsoleAppender();
            AddConsoleLogging(C);
        }

        public static void AddConsoleLogging(ConsoleAppender C)
        {
            root.AddAppender(C);
        }
        #endregion

        #region File Logging
        public static FileAppender AddFileLogging()
        {
            return AddFileLogging(DEFAULT_LOG_FILENAME);
        }

        public static FileAppender AddFileLogging(string sFileFullPath)
        {
            return AddFileLogging(sFileFullPath, true);
        }

        public static FileAppender AddFileLogging(string sFileFullPath, log4net.Core.Level threshold)
        {
            return AddFileLogging(sFileFullPath, threshold, true);
        }

        public static FileAppender AddFileLogging(string sFileFullPath, bool bAppendfile)
        {
            return AddFileLogging(sFileFullPath, LogSettings.Threshold, bAppendfile);
        }

        public static FileAppender AddFileLogging(string sFileFullPath, log4net.Core.Level threshold, bool bAppendfile)
        {
            FileAppender appender = GetFileAppender(sFileFullPath, threshold, bAppendfile);
            root.AddAppender(appender);
            return appender;
        }

        public static SmtpAppender AddSMTPLogging(string smtpHost, string From, string To, string subject)
        {
            SmtpAppender appender = GetSMTPAppender(smtpHost, From, To, subject, LogSettings.Threshold);
            root.AddAppender(appender);
            return appender;
        }

        #endregion

        #region EventLog
        public static EventLogAppender AddWindowsEventLogging()
        {
            EventLogAppender appender = GetEventLogAppender(LogSettings.APPNAME, LogSettings.LOGNAME, LogSettings.Threshold);
            root.AddAppender(appender);
            return appender;
        }
        #endregion

        public static log4net.Appender.IAppender GetLogAppender(string AppenderName)
        {
            AppenderCollection ac = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository()).Root.Appenders;

            foreach (log4net.Appender.IAppender appender in ac)
            {
                if (appender.Name == AppenderName)
                {
                    return appender;
                }
            }

            return null;
        }

        public static void CloseAppender(string AppenderName)
        {
            log4net.Appender.IAppender appender = GetLogAppender(AppenderName);
            CloseAppender(appender);
        }

        private static void CloseAppender(log4net.Appender.IAppender appender)
        {
            appender.Close();
        }

        #endregion

        #region Private Methods

        private static SmtpAppender GetSMTPAppender(string smtpHost, string From, string To, string subject, log4net.Core.Level threshhold)
        {
            SmtpAppender lAppender = new SmtpAppender();
            lAppender.To = To;
            lAppender.From = From;
            lAppender.SmtpHost = smtpHost;
            lAppender.Subject = subject;
            lAppender.BufferSize = 512;
            lAppender.Lossy = false;
            lAppender.Layout = new
            log4net.Layout.PatternLayout("%date{dd-MM-yyyy HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
            lAppender.Threshold = threshhold;
            lAppender.ActivateOptions();
            return lAppender;
        }

        private static ConsoleAppender GetConsoleAppender()
        {
            ConsoleAppender lAppender = new ConsoleAppender();
            lAppender.Name = "Console";
            lAppender.Layout = new
            log4net.Layout.PatternLayout(" %message %n");
            lAppender.Threshold = log4net.Core.Level.All;
            lAppender.ActivateOptions();
            return lAppender;
        }
        /// <summary>
        /// DETAILED Logging 
        /// log4net.Layout.PatternLayout("%date{dd-MM-yyyy HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
        ///  
        /// </summary>
        /// <param name="sFileName"></param>
        /// <param name="threshhold"></param>
        /// <returns></returns>
        private static FileAppender GetFileAppender(string sFileName, log4net.Core.Level threshhold, bool bFileAppend)
        {
            FileAppender lAppender = new FileAppender();
            lAppender.Name = sFileName;
            lAppender.AppendToFile = bFileAppend;
            lAppender.File = sFileName;
            lAppender.LockingModel = new FileAppender.MinimalLock();
            lAppender.Layout = new
                log4net.Layout.PatternLayout("%date{dd-MM-yyyy HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
            lAppender.Threshold = threshhold;
            lAppender.ActivateOptions();
            return lAppender;
        }

        private static EventLogAppender GetEventLogAppender(string appName, string logName, log4net.Core.Level threshhold)
        {
            EventLogAppender lAppender = new EventLogAppender();
            lAppender.Name = appName;
            lAppender.ApplicationName = appName;
            lAppender.LogName = logName;
            lAppender.Layout = new
                log4net.Layout.PatternLayout("%date{dd-MM-yyyy HH:mm:ss,fff} %5level [%2thread] %message (%logger{1}:%line)%n");
            lAppender.Threshold = threshhold;
            lAppender.ActivateOptions();
            return lAppender;
        }
        #endregion
        #endregion

        public Log4NetFileHelper(Type logType)
        {
            _logType = logType;
        }

        public ILog DefaultLogger
        {
            get
            {
                if (!_initialized)
                    Init();

                if (_logger == null)
                    _logger = LogManager.GetLogger(_logType);

                return _logger;
            }
        }
    }
}
