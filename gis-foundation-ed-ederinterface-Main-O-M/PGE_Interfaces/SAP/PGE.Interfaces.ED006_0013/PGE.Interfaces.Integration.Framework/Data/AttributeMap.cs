using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PGE.Interfaces.Integration.Framework.Data
{
    /// <summary>
    /// Data structure for storing an attribute value and the translated value that SAP needs
    /// This is different from domain mapping - it is for non-domained fields.
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(IsNullable = false)]
    public class AttributeMap
    {
        private string _fromValue;
        private string _toValue;
        /// <summary>
        /// Public constructor needed for serialization
        /// </summary>
        public AttributeMap()
        {

        }
        /// <summary>
        /// The from value that is stored in the geodatabase
        /// </summary>
        [XmlAttributeAttribute()]
        public string FromValue
        {
            get { return _fromValue; }
            set { _fromValue = value; }
        }
        /// <summary>
        /// The to value that SAP wants.
        /// </summary>
        [XmlAttributeAttribute()]
        public string ToValue
        {
            get { return _toValue; }
            set { _toValue = value; }
        }
    }
}