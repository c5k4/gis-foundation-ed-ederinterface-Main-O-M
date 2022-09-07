using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using Comm = PGE.BatchApplication.DataFramePropertiesReader.Common.Common;

namespace PGE.BatchApplication.DataFramePropertiesReader
{
    class ReaderClient
    {
        public enum ExitCode : int
        {
            Success = 0,
            InvalidParameters = 1
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: " +
                    "StoredDisplayReader.exe <SDE Connection 1>" +
                    "<SDE Connection 2> ... <SDE Connection n>");

                Environment.Exit((int)ExitCode.InvalidParameters);
            }

            Comm.InitializeESRILicense();
            Comm.InitializeArcFMLicense();

            HashSet<string> distinctSDs = new HashSet<string>();
            CSVCreator cc = new CSVCreator();

            Console.WriteLine("Initializing PGE.BatchApplication.DataFramePropertiesReader");

            for (int i = 0; i < args.Length; i++)
            {
                try
                {
                    IWorkspace wksp = Comm.OpenWorkspaceFromSDEFile(@args[i]);
                    string dbName = (string) wksp.ConnectionProperties.GetProperty("INSTANCE");
                    dbName = dbName.Substring(dbName.ToUpper().IndexOf("EDGIS"));
                    Console.WriteLine("Starting " + dbName);

                    Dictionary<string, string> descs = DataFramePropertiesReader.GetAllStoredDisplayDescriptions(wksp);
                    string value;

                    foreach (string sdName in descs.Keys)
                    {
                        distinctSDs.Add(sdName);

                        if (descs.TryGetValue(sdName, out value))
                        {
                            cc.AddDatum(new CSVDatum(sdName, dbName, value));
                        }
                        else
                        {
                            Console.WriteLine("Unable to get SD: " + sdName);
                        }
                    }

                    Console.WriteLine("Done with " + dbName);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to open SDE file: " + args[i]+". Continuing onto next file.");
                }
            }

            cc.FormatDataToCSV();
            cc.WriteToCSV(AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("ddMMyy_HHmmtt") + ".csv");

            Comm.CloseLicenseObject();
        }
    }
}


