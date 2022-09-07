using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.Integration.Framework;
using PGE.Interfaces.Integration.Framework.Exceptions;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Data Structure for storing information about how to transform a field along with the actual field transformer.
    /// </summary>
    [Serializable()]
    [XmlRootAttribute("FieldMapper", IsNullable = false)]
    public partial class FieldMapper//:ITMMFieldTransformer<IRow>
    {
        #region Private Member
        private IFieldTransformer<IRow> _fieldTransformer = null;

        private XmlNode _item;

        private string _transformerType;
        #endregion

        #region Public Serializable Members
        /// <summary>
        /// The XML used to initialize the IFieldTransformer
        /// </summary>
        [XmlAnyElement]
        public XmlNode Item
        {
            get
            {
                return this._item;
            }
            set
            {
                this._item = value;
            }
        }

        /// <summary>
        /// The fully qualified name of the IFieldTransformer to load
        /// </summary>
        [XmlAttributeAttribute("TransformerType")]
        public string TransformerType
        {
            get
            {
                return this._transformerType;
            }
            set
            {
                this._transformerType = value;
            }
        }

        #endregion


        #region Public Non-Serializable Members
        /// <summary>
        /// The FieldTransformer that will do the data transformation
        /// </summary>
        [XmlIgnore]
        public IFieldTransformer<IRow> FieldTransformer
        {
            get { return _fieldTransformer; }
            set { _fieldTransformer = value; }
        }
        #endregion
        //#region ITMMFieldTransformer Members

        //public T GetValue<T>(IRow row)
        //{
        //    T retVal = default(T);
        //    Initialize();
        //    if (_fieldTransformer != null)
        //    {
        //        retVal = _fieldTransformer.GetValue<T>(row);
        //    }
        //    return retVal;
        //}

        //#endregion

        #region Public Methods
        /// <summary>
        /// Initialize the FieldTransformer for the FieldMapper
        /// </summary>
        public void Initialize()
        {
            if (_fieldTransformer == null)
            {
                Type type = Type.GetType(TransformerType);
                if (type == null)
                {
                    throw new InvalidConfigurationException(string.Format("Cannot create object from type {0}. Check the configuration.", TransformerType));
                }
                _fieldTransformer = (IFieldTransformer<IRow>)Activator.CreateInstance(type);
                IConfigurable configurable = (IConfigurable)_fieldTransformer;
                if (!configurable.Initialize(Item))
                {
                    _fieldTransformer = null;
                }
            }
        }
        #endregion

    }
}
