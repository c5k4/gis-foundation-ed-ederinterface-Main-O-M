using System.Linq;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    internal class RemoveEdit : Edit
    {
        public RemoveEdit(GraphicsLayer layer) : base(layer) { }

        public override void Replay()
        {
            if (Graphic == null) return;
            if (Layer == null) return;

            if (Graphic.Geometry == null) return;

            // Some remove edits don't give us the ability to capture the original position so it needs
            // to match based on object ID.  I don't know if we can only use object ID so I left in the
            // geometry bit with an OR.  Also, "==" doesn't work for some reason, thus the .Equals().
            Graphic graphicToRemove = (from g in Layer.Graphics
                                       where (g.Geometry != null) &&
                                       (Utility.GeometriesAreEqual(g.Geometry, Graphic.Geometry) ||
                                       ((g.Attributes["OBJECTID"] != null) &&
                                       (g.Attributes["OBJECTID"].Equals(Graphic.Attributes["OBJECTID"]))))
                                       select g).FirstOrDefault();

            if (graphicToRemove != null)
            {
                Layer.Graphics.Remove(graphicToRemove);
            }
        }
    }
}
