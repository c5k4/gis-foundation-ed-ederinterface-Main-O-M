using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Browser;
using System.Xml.Linq;
using NLog.Config;
using NLog.Layouts;
using NLog;
using NLog.Targets;

namespace Miner.Silverlight.Logging.Client
{
    public class LogHelper
    {
        public static readonly Uri DefaultSilverlightUri = Application.Current.Host.Source;
        public static string DefaultLocalLogFileName = "Miner.Silverlight.Client";
        public static string DefaultServicesUrl = "Services/nLogReceiverService.asmx/logmessage";
        public static int DefaultMaxArchiveFiles = 2;

        /// <summary>
        /// Initializes the N log.
        /// </summary>
        /// <param name="element">The element.</param>
        public static void InitializeNLog(XElement element)
        {
            // init logger for reporting of config errors
            Logger logger = LogManager.GetCurrentClassLogger();
            logger.Info("Loading custom log configuration");

            // We don't want to mess with case sensitivity even though its XML
            string toLowerXMLTags = LowerCaseTags(element.ToString());

            System.Diagnostics.Debug.WriteLine(toLowerXMLTags);

            XElement configXML = new XElement("empty");
            try
            {
                configXML = XElement.Parse(toLowerXMLTags);
            }
            catch (Exception e)
            {
                logger.Error("The Page.config xml is not well formatted. XML: {0}", HttpUtility.HtmlEncode(element.ToString()));
            }

            // Our new logging context
            LoggingConfiguration logConfig = new LoggingConfiguration();

            // Loop through each target
            int targetCount = 0;
            IEnumerable<XElement> loggingNode = configXML.Elements("logging");
            foreach (XElement loggingElement in loggingNode.DescendantNodes())
            {
                targetCount++;
                switch (GetConfigAttribute(loggingElement, "type").ToLower())
                {
                    case "file":
                        // Create File Target
                        logger.Info("Target found- File type");

                        // Create file target
                        var targetLocal = CreateFileTarget(loggingElement, targetCount);
                        
                        // Get log above level
                        var logAboveLevelFile = GetConfigAttribute(loggingElement, "loglevel");
                        LogLevel levelToLogAbove = GetLogLevelEnum(logAboveLevelFile);
                        if (logAboveLevelFile.Length == 0)
                        {
                            logger.Warn(
                                String.Format(
                                    "Logging Configuration- Target File- target #{0} - Has no 'loglevel' attribute - system is using default {1} - XML: {2}",
                                    targetCount.ToString(), "Warn", HttpUtility.HtmlEncode(loggingElement.ToString())));
                        }


                        // Attach target and set its rule for logging above
                        logConfig.AddTarget("LocalStorage", targetLocal);
                        var logRuleLocal = new LoggingRule("*", levelToLogAbove, targetLocal);
                        logConfig.LoggingRules.Add(logRuleLocal);

                        logger.Info("Target load complete- File type");

                        break;


                    case "service":
                        logger.Info("Target found- Service type");

                        var targetWeb = CreateServiceTarget(loggingElement, targetCount);

                        // Attach service target to logger
                        logConfig.AddTarget("ServerStorage", targetWeb);

                        // Set its logging rules
                        String logAboveLevelService = GetConfigAttribute(loggingElement, "loglevel");
                        var levelToLogAboveService = GetLogLevelEnum(logAboveLevelService);
                        if (logAboveLevelService.Length == 0)
                        {
                            logger.Warn(String.Format("Logging Configuration- Target Service- target #{0}- Has no 'loglevel' attribute - system is using default {1} - XML: {2}", targetCount.ToString(), "Warn", HttpUtility.HtmlEncode(loggingElement.ToString())));
                        }

                        LoggingRule logRuleWeb = new LoggingRule("*", levelToLogAboveService, targetWeb);
                        logConfig.LoggingRules.Add(logRuleWeb);

                        logger.Info("Target load complete- File Service");
                        break;

                    default:
                        // Got a strange tag
                        logger.Warn(String.Format("Logging Configuration- Target UNKNOWN- target #{0} - XML: {1} ", targetCount.ToString(), HttpUtility.HtmlEncode(loggingElement.ToString())));
                        break;
                }
            }

            LogManager.Configuration = logConfig;
            logger.Info("Loading custom log configuration Complete");

        }

        protected static WebServiceTarget CreateServiceTarget(XElement loggingElement, int targetCount)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            // Set up Service target
            // Pull settings
            string serviceURL = GetConfigAttribute(loggingElement, "service");
            if (serviceURL.Length == 0)
            {
                serviceURL = DefaultServicesUrl;
                logger.Warn(
                    String.Format(
                        "Logging Configuration- Target Service- target #{0}- Has no 'service' attribute - system is using default {1} - XML: {2}",
                        targetCount.ToString(), DefaultServicesUrl, HttpUtility.HtmlEncode(loggingElement.ToString())));
            }

            // Create proper uri from current url + services url fragment
            String webRoot = DefaultSilverlightUri.OriginalString.Substring(0,
                                                                            DefaultSilverlightUri.OriginalString.IndexOf(
                                                                                "ClientBin", System.StringComparison.Ordinal));
            int endOfHost = webRoot.IndexOf(DefaultSilverlightUri.Host, System.StringComparison.Ordinal) +
                            DefaultSilverlightUri.Host.Length;
            webRoot = webRoot.Substring(endOfHost);

            UriBuilder uriBuilder = new UriBuilder(DefaultSilverlightUri.Scheme, DefaultSilverlightUri.Host,
                                                   DefaultSilverlightUri.Port);

            // Full services url to send log messages to
            uriBuilder.Path = webRoot + serviceURL;

            WebServiceTarget targetWeb = new WebServiceTarget
                {
                    Name = "ServerStorage",
                    Url = uriBuilder.Uri,
                    MethodName = "logmessage",
                    Protocol = WebServiceProtocol.HttpPost
                };
            targetWeb.Parameters.Add(new MethodCallParameter("level", "${level}"));
            targetWeb.Parameters.Add(new MethodCallParameter("message", "${message}"));

            return targetWeb;
        }

        protected static IsolatedStorageTarget CreateFileTarget(XElement loggingElement, int targetCount)
        {
            Logger logger = LogManager.GetCurrentClassLogger();

            // Pull settings
            var fileName = GetConfigAttribute(loggingElement, "filename");
            var maxArchiveFiles = GetConfigAttribute(loggingElement, "maxarchivefiles");
            var archiveAboveSize = GetConfigAttribute(loggingElement, "archiveabovesize");

            // Create layout
            IsolatedStorageTarget targetLocal = new IsolatedStorageTarget
                {
                    Layout = new CsvLayout()
                        {
                            Columns =
                                {
                                    new CsvColumn("Time", "${longdate}"),
                                    new CsvColumn("Level", "${level}"),
                                    new CsvColumn("Message", "${message}"),
                                    new CsvColumn("Logger", "${logger}"),
                                },
                        },
                    ConcurrentWrites = true,
                    ArchiveNumbering = ArchiveNumberingMode.Rolling,
                    MaxArchiveFiles = maxArchiveFiles.Length > 0 ? int.Parse(maxArchiveFiles) : DefaultMaxArchiveFiles
                };

            if (maxArchiveFiles.Length == 0)
            {
                logger.Warn(
                    "Logging Configuration- Target File- target #{0}- Has no 'maxarchivefiles' attribute - system is using default {1} - XML: {2}",
                    targetCount.ToString(), DefaultMaxArchiveFiles, HttpUtility.HtmlEncode(loggingElement.ToString()));
            }

            // Set file name
            if (fileName.Length > 0)
            {
                targetLocal.FileName = fileName;
                targetLocal.localFileName = fileName;
            }
            else
            {
                logger.Warn(
                    String.Format(
                        "Logging Configuration- Target File- target #{0}- Has no 'filename' attribute - system is using default {1} - XML: {2}",
                        targetCount.ToString(), DefaultLocalLogFileName, HttpUtility.HtmlEncode(loggingElement.ToString())));
                targetLocal.FileName = DefaultLocalLogFileName;
                targetLocal.localFileName = DefaultLocalLogFileName;
            }

            // Set Archive size
            if (archiveAboveSize.Length > 0)
            {
                targetLocal.ArchiveAboveSize = int.Parse(archiveAboveSize);
            }
            else
            {
                targetLocal.ArchiveAboveSize = 1024*100; //1mb                            
                logger.Warn(
                    String.Format(
                        "Logging Configuration- Target File- target #{0}- Has no 'archiveabovesize' attribute - system is using default {1} - XML: {2}",
                        targetCount.ToString(), targetLocal.ArchiveAboveSize.ToString(),
                        HttpUtility.HtmlEncode(loggingElement.ToString())));
            }
            return targetLocal;
        }

        protected static LogLevel GetLogLevelEnum(string logLevel)
        {
            LogLevel levelToLogAbove;
            switch (logLevel.ToLower())
            {
                case "info":
                    levelToLogAbove = LogLevel.Info;
                    break;
                case "warn":
                    levelToLogAbove = LogLevel.Warn;
                    break;
                case "error":
                    levelToLogAbove = LogLevel.Error;
                    break;
                case "fatal":
                    levelToLogAbove = LogLevel.Fatal;
                    break;
                default:
                    levelToLogAbove = LogLevel.Info;
                    break;
            }
            return levelToLogAbove;
        }

        private static string GetConfigAttribute(XElement node, string attributeName)
        {
            string nodeType = "";
            var logNodeTypeElement = node.Attribute(attributeName);
            if (logNodeTypeElement != null)
            {
                return logNodeTypeElement.Value;
            }
            else
            {
                return "";
            }
        }

        public static void InitializeNLog()
        {
            LoggingConfiguration logConfig = new LoggingConfiguration();

            IsolatedStorageTarget targetLocal = new IsolatedStorageTarget();
            targetLocal.Layout = new CsvLayout
                {
                    Columns =
                        {
                            new CsvColumn("Time", "${longdate}"),
                            new CsvColumn("Level", "${level}"),
                            new CsvColumn("Message", "${message}"),
                            new CsvColumn("Logger", "${logger}"),
                        }
                };
            targetLocal.FileName = DefaultLocalLogFileName;
            targetLocal.localFileName = DefaultLocalLogFileName;
            targetLocal.MaxArchiveFiles = 2;
            targetLocal.ConcurrentWrites = true;
            targetLocal.ArchiveFileName = "MinerSilverlightClient.{#####}.log.txt";
            targetLocal.ArchiveAboveSize = 1024*100; //1mb
            targetLocal.ArchiveNumbering = ArchiveNumberingMode.Rolling;
            targetLocal.MaxArchiveFiles = 2;

            String webRoot = DefaultSilverlightUri.OriginalString.Substring(0,
                                                                     DefaultSilverlightUri.OriginalString.IndexOf("ClientBin"));

            int endOfHost = webRoot.IndexOf(DefaultSilverlightUri.Host) + DefaultSilverlightUri.Host.Length;
            webRoot = webRoot.Substring(endOfHost);

            UriBuilder uriBuilder = new UriBuilder(DefaultSilverlightUri.Scheme, DefaultSilverlightUri.Host, DefaultSilverlightUri.Port);
            uriBuilder.Path = webRoot + DefaultServicesUrl;

            WebServiceTarget targetWeb = new WebServiceTarget()
                {
                    Name = "ServerStorage",
                    Url = uriBuilder.Uri,
                    MethodName = "logmessage",
                    Protocol = WebServiceProtocol.HttpPost
                };
            targetWeb.Parameters.Add(new MethodCallParameter("level", "${level}"));
            targetWeb.Parameters.Add(new MethodCallParameter("message", "${message}"));

            logConfig.AddTarget("LocalStorage", targetLocal);
            logConfig.AddTarget("ServerStorage", targetWeb);

            LoggingRule logRuleLocal = new LoggingRule("*", LogLevel.Info, targetLocal);
            logConfig.LoggingRules.Add(logRuleLocal);
            LoggingRule logRuleWeb = new LoggingRule("*", LogLevel.Warn, targetWeb);
            logConfig.LoggingRules.Add(logRuleWeb);

            LogManager.Configuration = logConfig;
        }

        public static string LowerCaseTags(string xml)
        {
            return Regex.Replace(
                xml,
                @"<[^<>]+>",
                m => { return m.Value.ToLower(); },
                RegexOptions.Multiline | RegexOptions.Singleline);
        }
    }
}
