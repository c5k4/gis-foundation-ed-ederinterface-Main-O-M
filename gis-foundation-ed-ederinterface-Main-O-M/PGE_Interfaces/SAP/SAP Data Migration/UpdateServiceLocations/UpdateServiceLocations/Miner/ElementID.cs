using ESRI.ArcGIS.Geodatabase;
using Miner.NetworkModel;
using System;

namespace Miner.Geodatabase
{
    internal class ElementID : IEquatable<ElementID>, IElement
    {
        public int EID;

        public esriElementType ElementType;

        public int ID
        {
            get
            {
                return this.EID;
            }
        }

        public ElementID(int eid, esriElementType elementType)
        {
            this.EID = eid;
            this.ElementType = elementType;
        }

        public override bool Equals(object obj)
        {
            return obj != null && base.GetType() == obj.GetType() && this.Equals((ElementID)obj);
        }

        public override int GetHashCode()
        {
            return this.EID ^ (int)this.ElementType;
        }

        public bool Equals(ElementID other)
        {
            return this.EID == other.EID && this.ElementType == other.ElementType;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", (int)this.ElementType, this.EID);
        }
    }
}
