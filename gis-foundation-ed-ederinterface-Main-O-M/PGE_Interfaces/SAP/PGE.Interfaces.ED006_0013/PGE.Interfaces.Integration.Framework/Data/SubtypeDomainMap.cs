using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PGE.Interfaces.Integration.Framework.Data
{
    /// <summary>
    /// Data structure for storing the domain name for a subtype of a feature class
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(IsNullable = false)]
    public class SubtypeDomainMap
    {
        private int _subtype;
        private string _domain;
        /// <summary>
        /// Public constructor needed for serialization
        /// </summary>
        public SubtypeDomainMap()
        {

        }
        /// <summary>
        /// The name of the domain
        /// </summary>
        [XmlAttributeAttribute()]
        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }
        /// <summary>
        /// The code for the subtype
        /// </summary>
        [XmlAttributeAttribute()]
        public int Subtype
        {
            get { return _subtype; }
            set { _subtype = value; }
        }
    }
}
