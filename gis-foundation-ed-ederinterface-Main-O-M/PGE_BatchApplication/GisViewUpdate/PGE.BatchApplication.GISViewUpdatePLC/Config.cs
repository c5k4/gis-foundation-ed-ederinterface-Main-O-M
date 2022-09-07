using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Reflection;


namespace PGE.BatchApplication.GISViewUpdatePLC
{
    public class Config
    {
        /// <summary>
        /// Name of the configuration file
        /// </summary>
        private static string _configFileName = "PGE.BatchApplication.GISViewUpdatePLC.exe.config";

        private static Configuration _configObject = null;
        /// <summary>
        /// Reads the Configuration object
        /// </summary>
        /// <returns>Returns the configuration read from the configurable location</returns>
        public static Configuration ReadConfigFromExeLocation()
        {

            if (_configObject == null)
            {
                //Get the Installation directory
                string installDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string configFileFullName = System.IO.Path.Combine(installDir, _configFileName);

                //Open configuration object
                ExeConfigurationFileMap newConfigFileMap = new ExeConfigurationFileMap();
                newConfigFileMap.ExeConfigFilename = configFileFullName;
                _configObject = ConfigurationManager.OpenMappedExeConfiguration(newConfigFileMap, ConfigurationUserLevel.None);

            }
            return _configObject;
        }

        /// <summary>
        /// Reads the Configuration object
        /// </summary>
        /// <returns>Returns the configuration read from the configurable location</returns>
        public static Configuration ReadConfig()
        {
            if (_configObject == null)
            {
                //Get the Installation directory
             //    Telvent.Delivery.Systems.Configuration.SystemRegistry registry = new Telvent.Delivery.Systems.Configuration.SystemRegistry();
                string installDir = "";
                string configFileFullName = System.IO.Path.Combine(installDir, "Config");
                configFileFullName = System.IO.Path.Combine(configFileFullName, _configFileName);

                //Open configuration object
                ExeConfigurationFileMap newConfigFileMap = new ExeConfigurationFileMap();
                newConfigFileMap.ExeConfigFilename = configFileFullName;
                _configObject = ConfigurationManager.OpenMappedExeConfiguration(newConfigFileMap, ConfigurationUserLevel.None);

            }
            return _configObject;
        }

    }

    public class FieldMappingSection : ConfigurationSection
    {
        static FieldMappingSection() { }

        /// <summary>
        /// Mapping of the XML Node and Field
        /// </summary>
        [ConfigurationProperty("FieldMapping", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(FieldMap), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
        public FieldMapping FieldMapping
        {
            get
            {
                return base["FieldMapping"] as FieldMapping;
            }
            set
            {
                base["FieldMapping"] = value;
            }
        }

    }

    /// <summary>
    /// XML Node and Field Map each node and field
    /// </summary>
    public class FieldMap : ConfigurationElement
    {
        static FieldMap() { }

        [ConfigurationProperty("key")]
        public string Key
        {
            get
            {
                return (this["key"].ToString());
            }
            set
            {
                this["key"] = value;
            }
        }
        [ConfigurationProperty("value")]
        public string Value
        {
            get
            {
                return (this["value"].ToString());
            }
            set
            {
                this["value"] = value;
            }
        }
    }

    /// <summary>
    /// Array of FieldMap elements
    /// </summary>
    [ConfigurationCollection(typeof(FieldMap), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    public class FieldMapping : ConfigurationElementCollection
    {
        public new FieldMap this[string name]
        {
            get { return (FieldMap)BaseGet(name); }
        }

        public FieldMap this[int index]
        {
            get { return (FieldMap)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new FieldMap();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((FieldMap)element).Key;
        }
        protected override string ElementName
        {
            get
            {
                return "add";
            }
        }

        protected override bool IsElementName(string elementName)
        {
            return elementName == "add";
        }
    }

}
