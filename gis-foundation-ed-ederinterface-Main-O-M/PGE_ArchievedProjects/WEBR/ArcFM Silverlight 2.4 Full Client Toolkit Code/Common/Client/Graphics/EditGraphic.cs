using System;
using System.ComponentModel;

using ESRI.ArcGIS.Client;

#if SILVERLIGHT
namespace Miner.Server.Client
#elif WPF
namespace Miner.Mobile.Client
#endif
{
    ///<exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class EditGraphic : Graphic
    {
        internal EditGraphic()
        {
            Guid = Guid.NewGuid();
        }
        
        internal EditGraphic(Graphic graphic) : this()
        {
            if (graphic == null)
            {
                throw new NullReferenceException("graphic is null.");
            }

            Geometry = graphic.Geometry;
            MapTip = graphic.MapTip;
            Selected = graphic.Selected;
            Symbol = graphic.Symbol;
            TimeExtent = graphic.TimeExtent;
            foreach (var attribute in graphic.Attributes)
            {
                Attributes.Add(attribute);
            }
        }

        public Guid Guid { get; set; }

#if SILVERLIGHT
        public override bool Equals(object obj)
#elif WPF
        public bool Equals(object obj)
#endif
        {
            EditGraphic graphic = obj as EditGraphic;
            if(graphic == null) return false;

            return Guid.Equals(graphic.Guid);
        }

#if SILVERLIGHT
        public override int GetHashCode()
#elif WPF
        public int GetHashCode()
#endif
        {
            return Guid.GetHashCode();
        }
    }
}
