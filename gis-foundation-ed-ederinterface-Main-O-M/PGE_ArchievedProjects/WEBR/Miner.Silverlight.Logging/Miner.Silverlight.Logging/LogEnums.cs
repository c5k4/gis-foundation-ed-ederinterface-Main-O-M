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
    public enum LogLevel
    {
        None,
        Debug,
        Normal
    }

    public enum LogType
    {
        ServerLog,
        BugZilla,
        User
    }

    enum ErrorType
    {
        WARN,
        DEBUG,
        TRACE,
        INFO
    }
}
