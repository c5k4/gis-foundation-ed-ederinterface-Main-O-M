using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EDGISExtract
{    
    public static class ExtractConstants
    {
        public const string CONFIG_FILENAME = "EDGISExtract.Config.xml";        
        public const int MOUSE_CURSOR_HOURGLASS = 2;
        //public const double XMIN_CUTOFF_0403 = 1950000;

        //LEGACYCOORDINATE values 
        public const string LEG_COORD_0401 = "NAD 1927 StatePlane California I FIPS 0401";
        public const string LEG_COORD_0402 = "NAD 1927 StatePlane California II FIPS 0402";
        public const string LEG_COORD_0403 = "NAD 1927 StatePlane California III FIPS 0403";
        public const string LEG_COORD_0404 = "NAD 1927 StatePlane California IV FIPS 0404";
        public const string LEG_COORD_0405 = "NAD 1927 StatePlane California V FIPS 0405";

        //Stateplane projection file names 
        public const string PROJ_FILE_0401 = "NAD 1927 StatePlane California I FIPS 0401.prj";
        public const string PROJ_FILE_0402 = "NAD 1927 StatePlane California II FIPS 0402.prj";
        public const string PROJ_FILE_0403 = "NAD 1927 StatePlane California III FIPS 0403.prj";
        public const string PROJ_FILE_0404 = "NAD 1927 StatePlane California IV FIPS 0404.prj";
        public const string PROJ_FILE_0405 = "NAD 1927 StatePlane California V FIPS 0405.prj";

        //Constants relating to MapGeo processing 
        public const int NEXT_GEO_SLEEP_INTERVAL = 180000;
        public const int CHECK_STATUS_SLEEP_INTERVAL = 30000;                 
        public const int MAX_FAILURE_COUNT = 2;

        //Plat featureclass constants 
        public const string PLAT_FC_NAME = "EDGIS.MaintenancePlat";
        public const string PLAT_FC_MAPSCALE = "MAPTYPE";
        public const string PLAT_FC_MAPNUMBER = "MAPNUMBER";
        public const string PLAT_FC_MAPOFFICE = "MAPOFFICE";

    }
}


