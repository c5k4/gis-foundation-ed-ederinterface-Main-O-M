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
    public class GenOnTransformer
    {
        public string SPID { get; set; }
        public string Address { get; set; }
        public string GenType { get; set; }
        public string GenSize { get; set; }
        public string Nameplate { get; set; }
        public string ProjectReference { get; set; }
        public string GENGLOBALID { get; set; }
        public string SLGUID { get; set; }
        public string TransGUID { get; set; }
        public string PMGUID { get; set; }
        public string METERNUMBER { get; set; }
        public string CGC12 { get; set; }
        public string FEEDERNUM { get; set; }
        /*****ENOS Tariff Changes 02/11/2020 Starts*******/
        public string Derated { get; set; }
        public string LimitedExport { get; set; }

        /*****ENOS Tariff Changes 02/11/2020 Ends*******/
    }
}
