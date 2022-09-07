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

namespace ArcFMSilverlight.Controls.PLC
{
    public class ZOrderDataAttr
    {
        public ulong PGE_SAPEQUIPID
        {
            get;
            set;
        }

        public string PGE_SketchLocation
        {
            get;
            set;
        }

        public string PGE_Status
        {
            get;
            set;
        }
        public ulong PLDBID
        {
            get;
            set;
        }

        public string PLDB_STATUS_SET_AT
        {
            get;
            set;
        }
        public string PoleSubType
        {
            get;
            set;
        }

    }
}
