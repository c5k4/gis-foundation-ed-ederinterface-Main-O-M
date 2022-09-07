using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ESRI.ArcGIS.Geodatabase;
using System.Xml.Serialization;

namespace Telvent.PGE.ED.Desktop.AutoUpdaters.Special.SymbolNumber.Schema
{
    /// <summary>
    /// A class representing a feature class and the rules used to determine it's symbol number.
    /// </summary>
    public class SymbolNumberCriteria 
    {
        // initialize serializer. XmlSerializer is thread safe, only a single instance is necessary.
        public static System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(SymbolNumberCriteria));

        /// <summary>
        /// The feature class name.  Used for indexing.
        /// </summary>
        [XmlElement(typeof(FeatureClass), ElementName = "FeatureClass")]
        public FeatureClass featureClass;

        /// <summary>
        /// Deserializes the specified stream.  Caller must handle exceptions thrown by the XmlSerializer.Deseralize method.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        public static SymbolNumberCriteria Deserialize(Stream stream)
        {
            return serializer.Deserialize(stream) as SymbolNumberCriteria;
        }

        /// <summary>
        /// Gets the symbol number bye evaluating the rules defined in the XML stream.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        public int GetSymbolNumber(IFeature feature)
        {
            return featureClass.GetSymbolNumber(feature);
        }

        /// <summary>
        /// Initializes the specified object class by passing initialization parameters to children.
        /// </summary>
        /// <param name="objectClass">The object class.</param>
        public void Initialize(IObjectClass objectClass)
        {
            featureClass.Initialize(objectClass);
        }

        public List<string> GetModelNames() {

            List<string> modelNames = new List<string>();

            featureClass.AppendModelNames(modelNames);

            return modelNames;
        } 
    }
}
