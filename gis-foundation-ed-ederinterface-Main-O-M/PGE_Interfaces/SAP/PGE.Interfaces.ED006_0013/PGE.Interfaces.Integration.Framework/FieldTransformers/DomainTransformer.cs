using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using PGE.Interfaces.Integration.Framework.Utilities;
using PGE.Common.ChangeDetectionAPI;

namespace PGE.Interfaces.Integration.Framework.FieldTransformers
{
    /// <summary>
    /// Look of the value in the external system based on the GIS value
    /// </summary>
    [SerializableAttribute()]
    [XmlRootAttribute(IsNullable = false)]
    public class DomainTransformer : FieldValueTransformer
    {
        private string _domainName;
        /// <summary>
        /// Default Constructor
        /// </summary>
        public DomainTransformer()
        {

        }

        /// <summary>
        /// The name of the mapping to use
        /// </summary>
        [XmlAttributeAttribute()]
        public string DomainName
        {
            get { return _domainName; }
            set { _domainName = value; }
        }


        #region IConfigurable Members
        /// <summary>
        /// Set the FieldName and DomainName of the Field Transformer
        /// </summary>
        /// <param name="config">The XML configuration</param>
        /// <returns>True if successful, otherwise false</returns>
        public override bool Initialize(XmlNode config)
        {
            DomainTransformer typeMapper = base.DeserializeXML<DomainTransformer>(config.OuterXml);
            this.FieldName = typeMapper.FieldName;
            this.Value = typeMapper.Value;
            this.DomainName = typeMapper.DomainName;
            return true;
        }

        #endregion

        #region IFieldTransformer<IRow> Members
        /// <summary>
        /// Get the mapped value
        /// </summary>
        /// <typeparam name="T">The return type, i.e. String</typeparam>
        /// <param name="row">The row to process</param>
        /// <returns>The mapped value</returns>
        public override T GetValue<T>(IRow row)
        {
            var value = base.GetValue(row);

            return (T)(object)DomainManager.Instance.GetValue(_domainName, value.ToString());

        }
        
        /// <summary>
        /// Get the mapped value
        /// </summary>
        /// <typeparam name="T">The return type, i.e. String</typeparam>
        /// <param name="row">The row to process</param>
        /// <returns>The mapped value</returns>
        public override T GetValue<T>(DeleteFeat row,ITable FCName)
        {
            var value = base.GetValue(row,FCName);

            return (T)(object)DomainManager.Instance.GetValue(_domainName, value.ToString());

        }
        #endregion
    }
}
