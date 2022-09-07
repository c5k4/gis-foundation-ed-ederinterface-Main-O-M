using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

using PGE.BatchApplication.ROBC_API;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.ROBCService.Common
{
    public static class Util
    {
        private static Log4NetLogger _logger;
        public static Log4NetLogger Logger
        {
            get
            {
                return _logger;
            }
        }

        private static ROBCManager _RobcManager;
        public static ROBCManager RobcManager
        {
            get
            {
                try
                {
                    return _RobcManager;
                }
                catch (Exception ex)
                {
                    var Info = string.Format("{0}////{1}", Constants.EDERDatabaseTNSName, Constants.EDERSUBDatabaseTNSName);
                    throw new Exception(string.Format("An exption from Get method:{0}\n\n\nStackTrace:{1}\n\n\n{2}", ex.Message, ex.StackTrace, Info), ex.InnerException);
                }
            }
        }


        static Util()
        {
            try
            {
                _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.ROBCApp.log4net.config");
                _logger.Info("Checking out Lisences and connecting to EDER...");
                _logger.Info(string.Format("EDER Database TNSName: {0}", Constants.EDERDatabaseTNSName));
                _logger.Info(string.Format("EDER User Name: {0}", Constants.EDERUserName));
                _logger.Info(string.Format("EDERSUB Database TNSName: {0}", Constants.EDERSUBDatabaseTNSName));
                _logger.Info(string.Format("EDERSUB User Name: {0}", Constants.EDERSUBUserName));

                _RobcManager = new ROBCManager(Constants.EDERDatabaseTNSName, Constants.EDERUserName, Constants.EDERPassword, Constants.EDERSUBDatabaseTNSName, Constants.EDERSUBUserName, Constants.EDERSUBPassword, Constants.EDERGeometricNetworkName, Constants.SUBGeometricNetworkName);
            }
            catch (Exception ex)
            {
                var Info = string.Format("{0}////{1}", Constants.EDERDatabaseTNSName, Constants.EDERSUBDatabaseTNSName);
                throw new Exception(string.Format("An exption from Util constructor:{0}\n\n\nStackTrace:{1}\n\n\n{2}", ex.Message, ex.StackTrace, Info), ex.InnerException);
            }
        }

    }
}
