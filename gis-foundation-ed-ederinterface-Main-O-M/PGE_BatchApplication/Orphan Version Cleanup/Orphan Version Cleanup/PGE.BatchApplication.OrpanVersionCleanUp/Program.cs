using System.Reflection;
using System;
using PGE.BatchApplication.OrpanVersionCleanUp.Control;
using PGE.BatchApplication.OrpanVersionCleanUp.@interface;
using PGE.BatchApplication.OrpanVersionCleanUp.Model;

namespace PGE.BatchApplication.OrpanVersionCleanUp
{
    /// <summary>
    /// This console application will be a Windows Scheduled Task for Orphan Version Cleanup. 
    /// This will run under ArcGIS Engine.
    /// Create a separate installer for the console app.
    /// </summary>
    class Program
    {
        private const string ElectricDb = "Electric", SubstationDb = "Substation";

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger Logger =
            new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType,
                "OrphanVersionCleanup.log4net.config");

        static void Main()
        {

            try
            {
                IConfigModel config;
                OrphanVersionCleaner cleaner;

                config = new ConfigModel(ElectricDb);
                cleaner = new OrphanVersionCleaner(config);
                cleaner.RunOrphanVersionCleanUp();

                config = new ConfigModel(SubstationDb);
                cleaner = new OrphanVersionCleaner(config);
                cleaner.RunOrphanVersionCleanUp();

#if DEBUG
                Console.ReadLine();
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
            }
        }
    }
}
