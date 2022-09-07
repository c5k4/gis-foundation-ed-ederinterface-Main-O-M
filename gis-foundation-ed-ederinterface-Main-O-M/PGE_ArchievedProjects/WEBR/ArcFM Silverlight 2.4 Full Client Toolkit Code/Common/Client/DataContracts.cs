using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    /// <summary>
    /// Class for serializing a GraphicCollection.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CollectionDataContract(Name = "Graphics", ItemName = "Graphic")]
    public class SerializableGraphicCollection : List<SerializableGraphic>
    {
        public SerializableGraphicCollection() { }

        public SerializableGraphicCollection(IEnumerable<EditGraphic> graphicCollection)
        {
            foreach (EditGraphic g in graphicCollection)
            {
                Add(new SerializableGraphic(g));
            }
        }
    }

    /// <summary>
    /// Class for serializing a Graphic.
    /// </summary>
    [DataContract]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SerializableGraphic
    {
        public SerializableGraphic() { }

        public SerializableGraphic(EditGraphic graphic)
        {
            Geometry = graphic.Geometry;
            Attributes = new SerializableAttributes(graphic.Attributes);
            Guid = graphic.Guid;
        }

        [DataMember]
        public Guid Guid;

        [DataMember]
        public SerializableAttributes Attributes;

        [DataMember]
        public Geometry Geometry;

        /// <summary>
        /// Gets the underlying graphic (useful after deserialization).
        /// </summary>
        /// <value>The graphic.</value>
        internal EditGraphic Graphic
        {
            get
            {
                EditGraphic g = new EditGraphic() { Geometry = Geometry, Guid = Guid };
                foreach (KeyValuePair<string, object> kvp in Attributes)
                {
                    g.Attributes.Add(kvp);
                }
                return g;
            }
        }
    }

    /// <summary>
    /// Class for serialization of Attributes.
    /// </summary>
    [CollectionDataContract(ItemName = "Attribute")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SerializableAttributes : List<KeyValuePair<string, object>>
    {
        public SerializableAttributes() { }

        public SerializableAttributes(IEnumerable<KeyValuePair<string, object>> items)
        {
            foreach (KeyValuePair<string, object> item in items)
            {
                if (item.Value is Graphic) continue; // Graphics as attributes don't make serializers happy.
                Add(item);
            }

        }
    }
}
