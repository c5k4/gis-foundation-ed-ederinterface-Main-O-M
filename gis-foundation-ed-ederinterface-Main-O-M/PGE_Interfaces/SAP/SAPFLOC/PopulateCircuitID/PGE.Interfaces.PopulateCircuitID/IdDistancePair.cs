using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Interface.PopulateCircuitID
{
    class IdDistancePair
    {
        public   int closestoid;
        public   double closestdistance;
        public IFeature closestfeature;

        public IdDistancePair(int closestoid, double closestdistance)
        {
            // TODO: Complete member initialization
            this.closestoid = closestoid;
            this.closestdistance = closestdistance;
        }

        public IdDistancePair(IFeature feature, double closestdistance)
        {
            // TODO: Complete member initialization
            this.closestfeature = feature;
            this.closestdistance = closestdistance;
        }
         
    }
}
