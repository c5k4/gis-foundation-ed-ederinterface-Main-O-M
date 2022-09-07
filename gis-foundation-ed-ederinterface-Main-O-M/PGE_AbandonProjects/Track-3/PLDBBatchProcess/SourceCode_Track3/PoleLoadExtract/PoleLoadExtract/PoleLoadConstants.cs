using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PoleLoadExtract
{
    public static class PoleLoadConstants
    {
        public const string CONFIG_FILENAME = "PoleLoadExtract.Config.xml";
        public const string MAP_FILENAME = "PoleLoadExtract.mxd";
        public const double COINCIDENCE_THRESHOLD = 0.001;
        public const string CONSTRUCTION_TYPE_DEFAULT = "HORIZONTAL"; 


        //public const string STATIC_DATA_TBL = "dbo.STATIC_DATA";
        //public const string SPAN_ANGLE_TBL = "dbo.SPAN_ANGLE";
        //public const string FULL_DATA_TBL = "dbo.FULL_DATA";

        public const string SPANS_TBL = "dbo.SPANS";
        public const string EQUIPMENT_TBL = "dbo.EQUIPMENT";
        public const string POLES_TBL = "dbo.POLES";

    }
}
