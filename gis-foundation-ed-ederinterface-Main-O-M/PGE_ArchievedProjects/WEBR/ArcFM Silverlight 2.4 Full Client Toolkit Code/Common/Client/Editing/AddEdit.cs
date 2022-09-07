using System;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.FeatureService;

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    internal class AddEdit : Edit
    {
        public AddEdit(GraphicsLayer layer) : base(layer) { }

        public override void Replay()
        {
            if (Layer == null) return;

            try
            {
                Layer.Graphics.Add(Graphic);
            }
            catch(Exception e)
            {
                if (e is ValidationException)
                {
                    foreach (var attribute in Graphic.Attributes)
                    {
                        if (attribute.Value is int)
                        {
                            Graphic.Attributes[attribute.Key] = (short)(int)attribute.Value;

                            break;
                        }
                    }
                }
            }
        }
    }
}
