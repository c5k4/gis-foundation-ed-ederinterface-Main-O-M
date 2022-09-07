using ESRI.ArcGIS.Geometry;
using System;

namespace Miner.Geodatabase.FeederManager
{
    internal class Location
    {
        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        public double Tolerance
        {
            get;
            set;
        }

        public Location()
        {
        }

        public Location(IPoint point)
        {
            if (point != null)
            {
                this.X = point.X;
                this.Y = point.Y;
            }
        }

        public override bool Equals(object obj)
        {
            bool flag = false;
            Location location = obj as Location;
            if (location != null)
            {
                flag = (location.X == this.X && location.Y == this.Y);
                if (!flag)
                {
                    double num = Math.Abs(this.X - location.X);
                    double num2 = Math.Abs(this.Y - location.Y);
                    double num3 = Math.Sqrt(num * num + num2 * num2);
                    flag = (num3 < this.Tolerance);
                }
            }
            return flag;
        }

        public override int GetHashCode()
        {
            return this.X.GetHashCode() ^ this.Y.GetHashCode();
        }
    }
}
