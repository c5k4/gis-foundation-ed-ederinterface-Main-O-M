using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.esriSystem;
using Comm = PGE.Interfaces.CCBServicePointUpdater.Common;
using Miner.Interop;
namespace PGE.Interfaces.CCBServicePointUpdater
{
    class ModifierClient
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Comm.LogManager.WriteLine("Usage: PGE.Interfaces.CCBServicePointUpdater.exe <SDE Connection Filename> <Version Name>");
            }

            try
            {
                //Initialization of logs/licenses
                Comm.LogManager.AddConsoleLogger();
                Comm.LogManager.AddFileLogger("ccb_serv_pt_updater");
                Comm.Common.InitializeESRILicense();
                Comm.Common.InitializeArcFMLicense();

                //Code block to disable AUs as we cannot normally edit Servicepoints
                Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
                IMMAutoUpdater auController = (IMMAutoUpdater)Activator.CreateInstance(type);
                mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;
                auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

                //Perform inserts/updates/deletes to the Servicepoint table
                VersionedEditor versionedEditor = new VersionedEditor(args[0], args[1]);
                ServicePointModifier spModifier = new ServicePointModifier(versionedEditor);
                spModifier.MakeAllChangesToServicepointTable();

                //Restore AU mode to what it was before
                auController.AutoUpdaterMode = prevAUMode;
            }
            catch (Exception e)
            {
                Comm.LogManager.WriteLine(e.ToString());
            }
        }
    }
}
