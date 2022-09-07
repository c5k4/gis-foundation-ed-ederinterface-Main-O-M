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

namespace ArcFMSilverlight
{
    public class PoleData
    {
        public ulong PLDBID
        {
            get;
            set;
        }

        public double Latitude
        {
            get;
            set;
        }
        public double Longitude
        {
            get;
            set;
        }
        public double Elevation
        {
            get;
            set;
        }
        public double PoleStrengthFactor
        {
            get;
            set;
        }
        public double PoleFactorOfSafety
        {
            get;
            set;
        }
        public double BendingFactorOfSafety
        {
            get;
            set;
        }
        public double VerticalFactorOfSafety
        {
            get;
            set;
        }
        public string PoleClass
        {
            get;
            set;
        }
        public double LenghtInInches
        {
            get;
            set;
        }
        public string Species
        {
            get;
            set;

        }
        public ulong PGE_SAPEQUIPID
        {
            get;
            set;
        }
        public string PGE_GLOBALID
        {
            get;
            set;
        }
        public Int64 PGE_OBJECTID
        {
            get;
            set;
        }
        public string PGE_MatCode
        {
            get;
            set;
        }
        public ulong PGE_OrderNumber
        {
            get;
            set;
        }
        public string PGE_SketchLocation
        {
            get;
            set;
        }
        public ulong PGE_NotificationNumber
        {
            get;
            set;
        }
        public ulong PGE_CopiedPLDBID
        {
            get;
            set;

        }
        public string PGE_LANID
        {
            get;
            set;
        }
        public string PGE_SnowLoadDistrict
        {
            get;
            set;
        }
        public string PGE_Description
        {
            get;
            set;
        }

        //public string PGE_PossibleFAALookup
        //{
        //    get;
        //    set;
        //}

        //public double PGE_AltitudeChange
        //{
        //    get;
        //    set;
        //}
    }
}
