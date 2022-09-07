using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems.Configuration;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace PGE.Desktop.EDER.ArcMapCommands.PONS
{
    public class ConfigurationHelper
    {
        #region private variables
       private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        //private log4net.ILog Logger = null;
        string ConfigFilePath = string.Empty;
        #endregion

        UtilityFunctions objUtilityFunctions = new UtilityFunctions();
        public ConfigurationHelper(/*log4net.ILog logger*/)
        {
            ConfigFilePath = string.Empty;
            try
            {
                string _xmlFilePath = GetConfigPath();
                //prepare the xmlfile if config path exist
                ConfigFilePath = System.IO.Path.Combine(_xmlFilePath, "PONSDesktopConfig.xml");
                //ConfigFilePath = objUtilityFunctions.ReadConfigurationValue("DESKTOP_CONFIG_FILE_WITHPATH");// Properties.Resources.DESKTOP_CONFIG_FILE_WITHPATH;
                if (!string.IsNullOrEmpty(ConfigFilePath))
                    PGEGlobal.ConfigFilePath = ConfigFilePath;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Logger = logger;
        }

        public string GetConfigPath()
        {
            string _xmlFilePath = string.Empty;
            try
            {
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                _xmlFilePath = sysRegistry.ConfigPath;
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return _xmlFilePath;
        }
        
        /// <summary>
        /// Checks Presence of Application Configfile in specific location of system's program file folder
        /// </summary>
        /// <returns></returns>
        public Boolean CheckPresenceofApplicationConfigfile()
        {
            return System.IO.File.Exists(ConfigFilePath);            
        }
        public List<string> GetListSeparatedbyComma(string keyname)
        {
            List<string> fieldlist = new List<string>();
            try
            {
                // UniqueFields

                string strtmp = ReadNodeInnerValue(keyname, "/config");
                string[] arr = strtmp.Split(',');
                fieldlist.AddRange(arr);
                return fieldlist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return fieldlist;
        }
        /// <summary>
        /// parses xml configuration entries path and returns node value
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="nodePath"></param>
        /// <returns></returns>
        public string ReadNodeInnerValue(string nodeName, string nodePath)//nodePath="/config"
        {
            if (!CheckPresenceofApplicationConfigfile())
            {
                _logger.Error("Application Configuration File not present.");
                return "NO_FILE";
            }
            string innerValue = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(ConfigFilePath);
                XmlNodeList configfilepathnodelist = doc.DocumentElement.SelectNodes(nodePath);
                foreach (XmlNode node in configfilepathnodelist)
                {
                    innerValue = node.SelectSingleNode(nodeName).InnerText;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Configuration entry for " + nodeName + " has errors.");
                _logger.Error(ex.Message);
            }
            return innerValue;

        }
       
    }
}