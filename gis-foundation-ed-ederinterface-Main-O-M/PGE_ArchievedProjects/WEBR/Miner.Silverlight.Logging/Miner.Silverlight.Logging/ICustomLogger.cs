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

namespace Miner.Silverlight.Logging
{
    public interface ICustomLogger
    {
        void LogException(string errorType, string errorMessage);
        void InitLog();
        void CloseLog();

        LogType CurrentLogType
        {
            get;
            set;
        }

        LogLevel CurrentLogLevel
        {
            get;
            set;
        }
    }
}
