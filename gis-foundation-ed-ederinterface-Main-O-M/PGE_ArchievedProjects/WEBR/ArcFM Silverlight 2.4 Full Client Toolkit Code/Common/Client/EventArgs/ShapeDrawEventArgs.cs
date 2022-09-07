using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    class ShapeDrawEventArgs : DrawEventArgs
    {
        public new Geometry Geometry { get; set; }
    }
}
