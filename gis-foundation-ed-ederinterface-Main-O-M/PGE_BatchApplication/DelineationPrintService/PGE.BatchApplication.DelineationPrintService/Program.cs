using System.ServiceProcess;

//Here is the once-per-application setup information
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace PGE.BatchApplication.DelineationPrintService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
			{ 
				new DelineationPrintService() 
			};
            ServiceBase.Run(ServicesToRun);
        }
    }
}
