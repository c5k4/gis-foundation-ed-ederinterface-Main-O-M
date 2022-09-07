using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interfaces.CCBServicePointUpdater.SQL
{
    class SQLConstants
    {
        public static readonly string ALL_CHANGES_TABLE = "SP_ACTION", SERVICEPOINT_TABLE = "EDGIS.SERVICEPOINT", NEW_SERVICEPOINT_DATA = "SERVICEPOINT_TAB";
        public static readonly string ACTION_COL = "ACTION", UNIQUEID_COL = "SERVICEPOINTID", ALL_COLUMNS = "*";
        public static readonly char INSERT = 'I', UPDATE = 'U', DELETE = 'D';
    }
}
