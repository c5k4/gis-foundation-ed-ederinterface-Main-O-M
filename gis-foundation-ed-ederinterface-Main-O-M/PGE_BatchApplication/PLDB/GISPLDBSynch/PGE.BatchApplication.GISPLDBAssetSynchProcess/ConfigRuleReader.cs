using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PGE.BatchApplication.GISPLDBAssetSynchProcess
{
    public class ConfigRuleReader : ConfigurationSection
    {
        public const string SectionName = "ConflationRules";

        private const string RuleCollectionName = "Rules";

        [ConfigurationProperty(RuleCollectionName)]
        [ConfigurationCollection(typeof(RuleCollection), AddItemName = "add")]
        public RuleCollection ConnectionManagerEndpoints { get { return (RuleCollection)base[RuleCollectionName]; } }
    }
    public class RuleCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RuleElement)element).Name;
        }
    }
    public class RuleElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("HFTD", IsRequired = true)]
        public string HFTD
        {
            get { return (string)this["HFTD"]; }
            set { this["HFTD"] = value; }
        }

        [ConfigurationProperty("SourceAccuracy", IsRequired = true)]
        public string SourceAccuracy
        {
            get { return (string)this["SourceAccuracy"]; }
            set { this["SourceAccuracy"] = value; }
        }

        [ConfigurationProperty("LIDARConflatedDate", IsRequired = true)]
        public string LIDARConflatedDate
        {
            get { return (string)this["LIDARConflatedDate"]; }
            set { this["LIDARConflatedDate"] = value; }
        }

        [ConfigurationProperty("ruleDesc", IsRequired = true)]
        public string ruleDesc
        {
            get { return (string)this["ruleDesc"]; }
            set { this["ruleDesc"] = value; }
        }
        
       
    }
}
