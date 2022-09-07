using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Systems.Configuration;
using System.Xml;
using System.IO;
using Telvent.Delivery.Systems.Data;
using Telvent.Delivery.Systems.Data.OleDb;
using System.Data;
using Telvent.Delivery.Framework;
using Telvent.Delivery.Systems;

namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// Class that is used to handle the Map Production Configuration
    /// </summary>
    public class MapProductionConfigurationHandler
    {
        #region private members
        /// <summary>
        /// Registry Path for getting the Config file
        /// </summary>
        private const string _registryPath = "PGE";
        /// <summary>
        /// Name fo the Map Configuration file name
        /// </summary>
        private static string _configFileName="Telvent.MapProduction.xml";
        /// <summary>
        /// Path of the config file
        /// </summary>
        private static string _configPath = string.Empty;
        /// <summary>
        /// The Serialized Config file
        /// </summary>
        private static config _config = null;
        /// <summary>
        /// Settings from the Configuration
        /// </summary>
        private static Dictionary<string, string> _settingsMap = null;
        #endregion

        #region CTOR
        public MapProductionConfigurationHandler()
        {

        }
        #endregion

        #region public members
        /// <summary>
        /// User name as configured in Config file
        /// The Set value sets an Excrypted value of the string passed in
        /// </summary>
        public static string UserName
        {
            get
            {
                return EncryptionFacade.Decrypt(Config.DatabaseConfig.UserName);
            }
            set
            {
                Config.DatabaseConfig.UserName = EncryptionFacade.Encrypt(value);
            }
        }
        /// <summary>
        /// Password as configured in Config file
        /// The Set value sets an Excrypted value of the string passed in
        /// </summary>
        public static string Password
        {
            get
            {
                return EncryptionFacade.Decrypt(Config.DatabaseConfig.Password);
            }
            set
            {
                Config.DatabaseConfig.Password = EncryptionFacade.Encrypt(value);
            }
        }
        /// <summary>
        /// Database Datasource as configured in Config file
        /// The Set value sets an Excrypted value of the string passed in
        /// </summary>
        public static string DataSource
        {
            get
            {
                return EncryptionFacade.Decrypt(Config.DatabaseConfig.Datasource);
            }
            set
            {
                Config.DatabaseConfig.Datasource = EncryptionFacade.Encrypt(value);
            }
        }
        /// <summary>
        /// Database Server Name as configured in Config file
        /// The Set value sets an Excrypted value of the string passed in
        /// </summary>
        public static string DatabaseServer
        {
            get
            {
                return EncryptionFacade.Decrypt(Config.DatabaseConfig.Server);
            }
            set
            {
                Config.DatabaseConfig.Server = EncryptionFacade.Encrypt(value);
            }
        }
        /// <summary>
        /// Geodatababse settings as configured in Config file
        /// </summary>
        public struct GeodatabaseSetting
        {
            /// <summary>
            /// Geodatabse Server Name as configured in Config file
            /// The Set value sets an Excrypted value of the string passed in
            /// </summary>
            public static string Server
            {
                get
                {
                    return EncryptionFacade.Decrypt(Config.DatabaseConfig.GeoDatabaseSettings.Server);
                }
                set
                {
                    Config.DatabaseConfig.GeoDatabaseSettings.Server = EncryptionFacade.Encrypt(value);
                }
            }
            /// <summary>
            /// Geodatabse Version as configured in Config file
            /// The Set value sets an Excrypted value of the string passed in
            /// </summary>
            public static string Version
            {
                get
                {
                    return EncryptionFacade.Decrypt(Config.DatabaseConfig.GeoDatabaseSettings.Version);
                }
                set
                {
                    Config.DatabaseConfig.GeoDatabaseSettings.Version = EncryptionFacade.Encrypt(value);
                }
            }
            /// <summary>
            /// Geodatabse Database as configured in Config file
            /// The Set value sets an Excrypted value of the string passed in
            /// </summary>
            public static string Database
            {
                get
                {
                    return EncryptionFacade.Decrypt(Config.DatabaseConfig.GeoDatabaseSettings.Database);
                }
                set
                {
                    Config.DatabaseConfig.GeoDatabaseSettings.Database = EncryptionFacade.Encrypt(value);
                }
            }
            /// <summary>
            /// Geodatabse Instance as configured in Config file
            /// The Set value sets an Excrypted value of the string passed in
            /// </summary>
            public static string Instance
            {
                get
                {
                    return EncryptionFacade.Decrypt(Config.DatabaseConfig.GeoDatabaseSettings.Instance);
                }
                set
                {
                    Config.DatabaseConfig.GeoDatabaseSettings.Instance = EncryptionFacade.Encrypt(value);
                }
            }
        }
        /// <summary>
        /// Name Pair values of Settings as configured in Config file
        /// </summary>
        public static Dictionary<string, string> Settings
        {
            get
            {
                if (_settingsMap == null)
                {
                    _settingsMap = new Dictionary<string, string>();
                    foreach (Setting setting in Config.DataSettings.Setting)
                    {
                        _settingsMap.Add(setting.name, EncryptionFacade.Decrypt(setting.value));
                    }
                }
                return _settingsMap;
            }

        }

        public static string ConfigFile
        {
            get
            {
                return _configFileName;
            }
            set
            {
                _configFileName = value;
                PopulateConfig();
            }
        }
        #endregion

        #region public methods
        /// <summary>
        /// Given he name of the setting will fetch a Setting Value
        /// </summary>
        /// <param name="name">Name of the setting to fetch</param>
        /// <returns></returns>
        public static string GetSettingValue(string name)
        {
            return Settings.ContainsKey(name) ? EncryptionFacade.Decrypt(Settings[name]) : string.Empty;
        }

        /// <summary>
        /// Adds a new setting to the Config Object
        /// </summary>
        /// <param name="name">Name of the Setting</param>
        /// <param name="value">Value of hte Setting</param>
        public static void AddSetting(string name, string value)
        {
            Setting setting = new Setting();
            setting.name = name;
            setting.value = EncryptionFacade.Encrypt(value);
            Config.DataSettings.Setting.Add(setting);
        }

        /// <summary>
        /// Removes a setting from teh Config.
        /// </summary>
        /// <param name="name">Name of the setting to remove</param>
        public void RemoveSetting(string name)
        {
            int i = 0;
            foreach (Setting setting in Config.DataSettings.Setting)
            {
                if (setting.name.Equals(name)) 
                    break;
                i++;
            }
            Config.DataSettings.Setting.RemoveAt(i); 
        }

        /// <summary>
        /// Clears all the settings from Config
        /// </summary>
        public static void ClearAllSetting()
        {
            Config.DataSettings.Setting = new List<Setting>();
        }

        /// <summary>
        /// Persists the current state of Config object to the configuration file
        /// If any change is made to the config object that should be persisted it is required to call this method.
        /// </summary>
        /// <returns></returns>
        public static bool SaveConfiguration()
        {
            try
            {
                if (Config != null)
                {
                    XmlDocument doc = XmlFacade.Serialize<config>(Config);
                    doc.Save(Path.Combine(ConfigPath, _configFileName));
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
        #endregion

        #region private methods and members
        /// <summary>
        /// Gets the location of the config file from the system registry
        /// </summary>
        private static string ConfigPath
        {
            get
            {
                if (_configPath == string.Empty)
                {
                    SystemRegistry sysRegistry = new SystemRegistry(_registryPath);
                    _configPath = sysRegistry.ConfigPath;
                }
                return _configPath;
            }

        }
        /// <summary>
        /// Gets the Config object by deserializing the Configuration XML
        /// </summary>
        private static config Config
        {
            get
            {
                if (_config == null)
                {
                    PopulateConfig();
                }
                return _config;
            }
        }

        /// <summary>
        /// Called by Config private member will deserialize the Configuration XML from the Configuration file
        /// </summary>
        private static void PopulateConfig()
        {
            XmlDocument doc = new XmlDocument();
            if (File.Exists((Path.Combine(ConfigPath, _configFileName))))
            {
                doc.Load(Path.Combine(ConfigPath, _configFileName));
                _config = XmlFacade.DeserializeFromXml<config>(doc);
            }
        }

        #endregion
    }
}