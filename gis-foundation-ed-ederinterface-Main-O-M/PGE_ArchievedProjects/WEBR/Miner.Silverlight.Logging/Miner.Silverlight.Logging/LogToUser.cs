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
    public class LogToUser : Miner.Silverlight.Logging.ICustomLogger
    {

        private LogType _logtype;
        private LogLevel _loglevel;


        public LogToUser()
        {
        }


        public void LogException(string errorType, string errorMessage)
        {
            MessageBox.Show(errorMessage);
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
                return _logtype;
            }
            set
            {
                _logtype = value;
            }
        }

        public LogLevel CurrentLogLevel
        {
            get
            {
                return _loglevel;
            }
            set
            {
                _loglevel = value;
            }
        }
    }
}
