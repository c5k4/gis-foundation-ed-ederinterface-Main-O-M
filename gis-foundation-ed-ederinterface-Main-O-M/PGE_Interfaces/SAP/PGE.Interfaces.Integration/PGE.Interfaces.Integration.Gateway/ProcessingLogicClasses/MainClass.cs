using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Timers;

namespace PGE.Interfaces.Integration.Gateway
{
    class MainClass
    {

        public static DateTime m_dStartTime = default;
        private PullData pullDataObject = null;
        private PushData pushDataObject = null;
        public static string interfaceName = string.Empty;
        public static string interfaceType = string.Empty;

        internal bool StartProcess(string[] Arguments)
        {
            IWorkspace m_SDEEDERDefaultworkspace = null;

            bool ProcessCompleted = false;
            try
            {
                ReadConfiguration.readConfigValues();
                ReadConfiguration.LoadConfiguration();

                interfaceName = Arguments[0];
                if (Arguments[1].ToUpper() == "Inbound".ToUpper())
                {
                    interfaceType = Arguments[1];
                    pullDataObject = new PullData();
                    pullDataObject.CreateWebRequest(Arguments[0]);

                }
                else if (Arguments[1].ToUpper() == "Outbound".ToUpper())
                {
                    interfaceType = Arguments[1];
                    pushDataObject = new PushData();
                    pushDataObject.CreateWebRequest(Arguments[0].ToUpper());
                }
                else
                {
                    throw new Exception("Invallid Argument Passed");
                }
                ProcessCompleted = true;
            }
            catch (Exception exp)
            {
                Common._log.Error(exp.Message + " at " + exp.StackTrace);
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                if (m_SDEEDERDefaultworkspace != null) { Marshal.ReleaseComObject(m_SDEEDERDefaultworkspace); m_SDEEDERDefaultworkspace = null; }

            }
            return ProcessCompleted;
        }

        void TimerTick(System.Object obj, ElapsedEventArgs e)
        {

            Common._log.Error("");
            Common._log.Error("");
            Environment.Exit(0);

        }



    }
}
