using System;

#if SILVERLIGHT
namespace Miner.Server.Client.Editing
#elif WPF
namespace Miner.Mobile.Client.Editing
#endif
{
    /// <exclude/>
    [Obsolete("Now using the ESRI.ArcGIS.Client.DrawMode")]
    public enum DrawOption
    {
        Polygon,
        Rectangle,
        Circle
    }
}
