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
    public class Utilities
    {
        public static string characterEntityConverter(string s)
        {
            if (!s.Contains("&")) return s;
            else
            {
                if (s.Contains("&amp")) return s = s.Replace("&amp;", "&");
                if (s.Contains("&lt;")) return s = s.Replace("&lt;", "<");
                if (s.Contains("&gt;")) return s = s.Replace("&gt;", ">");
                if (s.Contains("&quot")) return s = s.Replace("&quot;", "\""); 
                else return s;                
            }
        }
    }
}
