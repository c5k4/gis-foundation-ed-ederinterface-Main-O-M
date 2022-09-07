using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Data structure for storing mapped field information
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute("Field", IsNullable = false)]
    public partial class MappedField 
    {
        #region Private Member
        private FieldMapper _fieldMapper;

        private string _outName;

        private int _sequence;
        #endregion

        #region Public Serializable Members
        /// <summary>
        /// Fieldmapper that will do the transformation
        /// </summary>
        public FieldMapper FieldMapper
        {
            get
            {
                return this._fieldMapper;
            }
            set
            {
                this._fieldMapper = value;
            }
        }

        /// <summary>
        /// The name of the field in the external system
        /// </summary>
        [XmlAttributeAttribute()]
        public string OutName
        {
            get
            {
                return this._outName;
            }
            set
            {
                this._outName = value;
            }
        }

        /// <summary>
        /// The position of the field in the external system
        /// </summary>
        [XmlAttributeAttribute()]
        public int Sequence
        {
            get
            {
                return this._sequence;
            }
            set
            {
                this._sequence = value;
            }
        }
        #endregion
    }

}
