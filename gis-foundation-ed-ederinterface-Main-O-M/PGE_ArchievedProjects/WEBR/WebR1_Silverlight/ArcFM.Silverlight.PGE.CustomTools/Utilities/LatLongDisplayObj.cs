using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ArcFM.Silverlight.PGE.CustomTools
{
    public class LatLongDisplayObj
    {
        private double _lat = 0.0;
        private double _lon = 0.0;

        public LatLongDisplayObj(double inLat, double inLong)
        {
            _lat = inLat;
            _lon = inLong;
        }

        public double Lat
        {
            get { return _lat; }
           
        }

        public double Long
        {
            get { return _lon; }

        }

    }
}
