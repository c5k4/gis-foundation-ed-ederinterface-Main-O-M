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
    public class StartAnalysisResponse
    {
        public ulong PLDBID
        {
            get;
            set;
        }

        public string AdditionalInfo
        {
            get;
            set;
        }

        public string ClientUserMessage
        {
            get;
            set;
        }

        public string DeveloperMessage
        {
            get;
            set;
        }

        public string ErrorCode
        {
            get;
            set;
        }

        public string StatusCode
        {
            get;
            set;
        }
    }
}
