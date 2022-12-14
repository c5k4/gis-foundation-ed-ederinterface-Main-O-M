using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DLMTools.Utility.Logs;


namespace DLMTools
{
    [RunInstaller(true)]
    public partial class InstallerDlm : System.Configuration.Install.Installer
    {
        public InstallerDlm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Install assembly
        /// </summary>
        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.RegisterAssembly(GetType().Assembly,
                AssemblyRegistrationFlags.SetCodeBase);

            /*
             * This approach is superceded by the one below.
             * 
             * Write to the event log to activate it, just in case, while we have admin privileges.
             * EventLog registerEL = new System.Diagnostics.EventLog(LogSettings.LOGNAME);
             * registerEL.Source = LogSettings.APPNAME;
             * registerEL.WriteEntry("Installing PG&E GIS Customizations.", EventLogEntryType.Information, 0);
            */

            // Create the log source, if it does not already exist. 
            if (!EventLog.SourceExists(LogSettings.APPNAME))
            {
                //An event log source should not be created and immediately used. 
                EventLog.CreateEventSource(LogSettings.APPNAME, LogSettings.LOGNAME);
            }


        }

        /// <summary>
        /// Uninstall assembly
        /// </summary>
        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            RegistrationServices regSrv = new RegistrationServices();
            regSrv.UnregisterAssembly(GetType().Assembly);
        }
    }
}
