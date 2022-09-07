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
    public class LogToServer : Miner.Silverlight.Logging.ICustomLogger
    {
        public LogToServer()
        {
        }


        public void LogException(string errorType, string errorMessage)
        {
            throw new NotImplementedException();
        }

        public void InitLog()
        {
            throw new NotImplementedException();
        }

        public void CloseLog()
        {
            throw new NotImplementedException();
        }

        public LogType CurrentLogType
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public LogLevel CurrentLogLevel
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
