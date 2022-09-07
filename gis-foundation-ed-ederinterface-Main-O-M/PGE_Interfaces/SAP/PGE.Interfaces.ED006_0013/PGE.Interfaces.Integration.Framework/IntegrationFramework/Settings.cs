using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Data structure for storing a list of settings
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(IsNullable = false)]
    public partial class Settings
    {
        #region Private Member
        private List<Setting> _settingList;
        #endregion

        #region Public Serializable Members
        /// <summary>
        /// The list of settings
        /// </summary>
        [XmlElement("Setting", typeof(Setting))]
        public List<Setting> SettingList
        {
            get
            {
                return this._settingList;
            }
            set
            {
                this._settingList = new List<Setting>(value);
            }
        }
        /// <summary>
        /// Method for accessing a setting by name
        /// </summary>
        /// <param name="settingName">Name of the setting</param>
        /// <returns>The value of the setting or an empty string</returns>
        public string this[string settingName]
        {
            get
            {
                string retVal = string.Empty; 
                 Setting setting=_settingList.Find(tc => tc.Name.Equals(settingName, StringComparison.InvariantCultureIgnoreCase));
                 if (setting != null)
                 {
                     retVal = setting.Value;
                 }
                 return retVal;
            }
        }

        #endregion
    }

    /// <summary>
    /// Data structure for storing a setting
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(IsNullable = false)]
    public partial class Setting
    {
        #region Private Member
        private string _name;

        private string _value;
        #endregion

        #region Public Serializable Members
        /// <summary>
        /// The setting name
        /// </summary>
        [XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        /// <summary>
        /// The setting value
        /// </summary>
        [XmlAttributeAttribute()]
        public string Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        #endregion
    }

}
