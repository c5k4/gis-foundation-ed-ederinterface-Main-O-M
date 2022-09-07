using System.Configuration;
using System;
using System.Xml;

namespace PGE.Common.Delivery.ArcFM
{
    /// <summary>
    /// 
    /// </summary>
    public class FacilityIDConfigSection : ConfigurationSection
    {
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("facilityiddefinition",IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(FacilityIDDefinitionCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public FacilityIDDefinitionCollection FacilityIDDefinition
        {
            get
            {
                FacilityIDDefinitionCollection facIdDefinition =(FacilityIDDefinitionCollection)base["facilityiddefinition"];
                return facIdDefinition;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        protected override void DeserializeSection(System.Xml.XmlReader reader)
        {
            base.DeserializeSection(reader);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="name"></param>
        /// <param name="saveMode"></param>
        /// <returns></returns>
        protected override string SerializeSection(ConfigurationElement parentElement,string name, ConfigurationSaveMode saveMode)
        {
            string s = base.SerializeSection(parentElement,name, saveMode);
            return s;
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class FacilityIDDefinitionCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public FacilityIDDefinitionCollection()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new FacilityIDDefinition();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new FacilityIDDefinition(elementName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((FacilityIDDefinition)element).FeatureClassName;
        }

        /// <summary>
        /// 
        /// </summary>
        public new string AddElementName
        {
            get
            { return base.AddElementName; }

            set
            { base.AddElementName = value; }

        }
        /// <summary>
        /// 
        /// </summary>
        public new string ClearElementName
        {
            get
            { return base.ClearElementName; }

            set
            { base.AddElementName = value; }

        }
        /// <summary>
        /// 
        /// </summary>
        public new string RemoveElementName
        {
            get
            { return base.RemoveElementName; }
        }
        /// <summary>
        /// 
        /// </summary>
        public new int Count
        {
            get { return base.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public FacilityIDDefinition this[int index]
        {
            get
            {
                return (FacilityIDDefinition)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FeatureClassName"></param>
        /// <returns></returns>
        new public FacilityIDDefinition this[string FeatureClassName]
        {
            get
            {
                return (FacilityIDDefinition)BaseGet(FeatureClassName);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fcDefinition"></param>
        /// <returns></returns>
        public int IndexOf(FacilityIDDefinition fcDefinition)
        {
            return BaseIndexOf(fcDefinition);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fcDefinition"></param>
        public void Add(FacilityIDDefinition fcDefinition)
        {
            BaseAdd(fcDefinition);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fcDefinition"></param>
        public void Remove(FacilityIDDefinition fcDefinition)
        {
            if (BaseIndexOf(fcDefinition) >= 0)
                BaseRemove(fcDefinition.FeatureClassName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            BaseClear();
        }

    }
    /// <summary>
    /// 
    /// </summary>
    public class FacilityIDDefinition : ConfigurationElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureclassName"></param>
        /// <param name="prefix"></param>
        /// <param name="sequenceName"></param>
        public FacilityIDDefinition(string featureclassName, string prefix, string sequenceName)
        {
            FeatureClassName = featureclassName;
            Prefix = prefix;
            SequenceName = sequenceName;
        }
        /// <summary>
        /// 
        /// </summary>
        public FacilityIDDefinition()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="elementName"></param>
        public FacilityIDDefinition(string elementName)
        {
            FeatureClassName = elementName;
        }
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("featureclassname",IsRequired = true,IsKey = true)]
        public string FeatureClassName
        {
            get
            {
                return (string)this["featureclassname"];
            }
            set
            {
                this["featureclassname"] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("prefix",IsRequired = true)]
        public string Prefix
        {
            get
            {
                return (string)this["prefix"];
            }
            set
            {
                this["prefix"] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [ConfigurationProperty("sequencename",IsRequired = true)]
        public string SequenceName
        {
            get
            {
                return (string)this["sequencename"];
            }
            set
            {
                this["sequencename"] = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="serializeCollectionKey"></param>
        protected override void DeserializeElement(XmlReader reader,bool serializeCollectionKey)
        {
            base.DeserializeElement(reader,
                serializeCollectionKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="serializeCollectionKey"></param>
        /// <returns></returns>
        protected override bool SerializeElement(XmlWriter writer,bool serializeCollectionKey)
        {
            bool ret = base.SerializeElement(writer,serializeCollectionKey);
            return ret;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool IsModified()
        {
            bool ret = base.IsModified();
            return ret;
        }

    }

}
