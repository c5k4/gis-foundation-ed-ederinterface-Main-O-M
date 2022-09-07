using System.Linq;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
using Miner.Server.Client.Symbols;
#elif WPF
using Miner.Mobile.Client.Symbols;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    internal class ModifyEdit : Edit
    {
        private const string TextGraphicKey = "TextGraphic";

        public ModifyEdit(GraphicsLayer layer)
            : base(layer)
        {

        }

        //public Geometry OriginalGeometry { get; set; }

        public override void Replay()
        {
            if (Graphic == null) return;
            if (Layer == null) return;

            if (Graphic.Geometry == null) return;

            // Some edits don't give us the ability to capture the original position so it needs
            // to match based on object ID.
            Graphic graphicToUpdate = (from g in Layer.Graphics
                                       where (g.Geometry != null) &&
                                       (Utility.GeometriesAreEqual(g.Geometry, Graphic.Geometry) ||
                                       ((g.Attributes["OBJECTID"] != null) &&
                                       (g.Attributes["OBJECTID"].Equals(Graphic.Attributes["OBJECTID"]))))
                                       select g).FirstOrDefault();

            if (graphicToUpdate == null) return;

            // Update the Geometry
            graphicToUpdate.Geometry = Graphic.Geometry;

            if (Graphic.Attributes == null) return;

            // Update the attributes
            foreach (var attribute in Graphic.Attributes.Where(a => a.Key != "ROTATION_ANGLE"))
            {
                if (attribute.Value != null && graphicToUpdate.Attributes[attribute.Key].ToString() != attribute.Value.ToString())
                {
                    graphicToUpdate.Attributes[attribute.Key] = attribute.Value;
                }
            }

            if (graphicToUpdate.Symbol is CustomPointSymbol && Graphic.Attributes.ContainsKey("ROTATION_ANGLE"))
            {
                (graphicToUpdate.Symbol as CustomPointSymbol).Angle = (double)Graphic.Attributes["ROTATION_ANGLE"];
            }
            else
            {
                if (graphicToUpdate.Attributes.ContainsKey(TextGraphicKey))
                {
                    ((graphicToUpdate.Attributes[TextGraphicKey] as Graphic).Symbol as CustomTextSymbol).Angle = (double)Graphic.Attributes["ROTATION_ANGLE"];
                }
            }
        }
    }
}
