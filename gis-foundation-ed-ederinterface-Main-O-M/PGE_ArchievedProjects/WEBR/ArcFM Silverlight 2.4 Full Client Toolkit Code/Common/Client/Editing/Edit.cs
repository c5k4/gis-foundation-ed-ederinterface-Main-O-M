using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    /// <summary>
    /// Base class for types of edits.
    /// </summary>
    public abstract class Edit
    {
        public Edit(GraphicsLayer layer)
        {
            Layer = layer;
        }

        public GraphicsLayer Layer { get; set; }

        public Graphic Graphic { get; set; }

        public int PasteSum { get; set; }

        public abstract void Replay();

        //public virtual void Serialize(XmlWriter writer)
        //{
        //    writer.WriteStartElement("Edit");
        //    writer.WriteAttributeString("Type", this.GetType().AssemblyQualifiedName);
        //    this.Graphics.SerializeGraphics(writer);
        //    writer.WriteEndElement();
        //}

        //public static virtual Edit Deserialize(XElement element)
        //{
        //    if (element.Name.LocalName.Equals("Edit") == false) return null;

        //    string type = element.Attribute("Type").Value;
        //    Edit edit = (Edit)Activator.CreateInstance(Type.GetType(type));
        //    foreach (XElement graphicsElement in element.Elements())
        //    {
        //        MemoryStream memoryStream = new MemoryStream(new System.Text.UTF8Encoding().GetBytes(graphicsElement.ToString()));
        //        GraphicCollection graphics = new GraphicCollection();
        //        graphics.DeserializeGraphics(XmlReader.Create(memoryStream));
        //        foreach (Graphic graphic in graphics)
        //        {
        //            edit.Graphics.Add(graphic);
        //        }
        //    }

        //    return edit;
        //}
    }
}
