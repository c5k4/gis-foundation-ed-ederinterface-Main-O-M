using System;
using System.Configuration;
using System.Reflection;
using System.Collections .Generic ;

using PGE.Interfaces.SAP;
using PGE.Interfaces.SAP.Interfaces;

using System.IO;
using System.Diagnostics;

namespace PGE.Interfaces.SAP.Batch
{
    /// <summary>
    /// Batch process that reads from the staging table and writes to CSV files.
    /// </summary>
    public class Program
    {
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "SAPBatch.log4net.config");
        /// <summary>
        /// Entry point to the batch process
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            Process(args);
        }
        /// <summary>
        /// Contains all batch processing logic. Can be used by external classes, just make sure all the proper configuration is in place.
        /// </summary>
        /// <param name="args"></param>
        public static void Process(string[] args)
        {
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();

            try
            {
                // Unlike in GDBM
                if (args.Length > 1 && args[1].ToUpper() == "-D")
                {
                    PGE.Interfaces.SAP.DataHelper.DisableSapRWNotificationWrites = true;
                }
                //Check trigger file (empty .txt file)
                string triggerFileName = ConfigurationManager.AppSettings["TriggerFileName"];
                if (File.Exists(triggerFileName))
                {
                    throw new Exception(string.Format("{0} file currently exists. Export will not be processed as the previous export results are not transferred. Please transfer the files first before start a new batch.", triggerFileName));
                }
                //use the same timestamp for all files
                string fileEnd = "_" +
                    DateTime.Now.ToString("yyyyMMdd") + "_" +
                    DateTime.Now.ToString("HHmmss") + ".csv";
                // Creating a functional location table reader
                ISAPDataReader reader = new SAPFunctionalLocationTableReader();

                // Creating a functional location csv file writer
                SAPCSVWriter functionalLocationWriter = new SAPCSVWriter();

                // Setting functional location csv file location and name from app setting 
                functionalLocationWriter.CSVFileName =
                    ConfigurationManager.AppSettings["FunctionalLocationsFileName"] + fileEnd;

                // Writing functional location records to output csv file
                functionalLocationWriter.OutputData(reader.ReadData(), false);

                _logger.Info("Functional Location csv file created");

                // Creating a structure table reader
                reader = new SAPStructureTableReader();

                // Creating a structure csv file writer
                SAPCSVWriter structuresWriter = new SAPCSVWriter();

                // Setting structure csv file location and name from app setting 
                structuresWriter.CSVFileName =
                    ConfigurationManager.AppSettings["StructuresFileName"] + fileEnd;

                // Writing structure records to output csv file
                structuresWriter.OutputData(reader.ReadData(), false);

                _logger.Info("Structures csv file created");

                // Creating a devices table reader
                reader = new SAPDevicesTableReader();

                // Creating a devices csv file writer
                SAPCSVWriter devicesWriter = new SAPCSVWriter();

                // Setting devices csv file location and name from app setting 
                devicesWriter.CSVFileName =
                    ConfigurationManager.AppSettings["DevicesFileName"] + fileEnd;

                // Writing devices records to output csv file
                devicesWriter.OutputData(reader.ReadData(), false);

                _logger.Info("Devices csv file created.");

                //back up files
                List<string> files = new List<string>();
                files.Add(functionalLocationWriter.CSVFileName);
                files.Add(structuresWriter.CSVFileName);
                files.Add(devicesWriter.CSVFileName);
                BackupFiles(files);

                //create trigger file (empty .txt file)
                File.CreateText(triggerFileName);
                _logger.Info(string.Format("Trigger file created at '{0}'", triggerFileName));

                // clear the staging table
                using (DataHelper dbHelper = new DataHelper())
                {
                    _logger.Info("Deleting Data");
                    int numRowsDeleted = dbHelper.DeleteData();

                    _logger.Info(numRowsDeleted + " rows cleaned from GISSap_AssetSynch staging table.");
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error in Batch Process", e);
                Console.WriteLine("Failed " + e);
            }
            elapsedTime.Stop();
#if DEBUG
            //Console.WriteLine("Time spent in getRWStatus: " + Math.Round(((DataHelper.SW_getRWStatus.ElapsedMilliseconds / 1000.0) / 60.0), 2) + " minutes");
            Console.WriteLine("Total execution time: " + Math.Round(((elapsedTime.ElapsedMilliseconds / 1000.0) / 60.0),2) + " minutes");
            Console.WriteLine("Press any key to exit");
            Console.ReadLine();
#endif

            Console.WriteLine("bye");

        }

        /// <summary>
        /// Given the name of the files, make backup to the configured destination
        /// </summary>
        /// <param name="fileNames">The fully qualified fileName, e.g, with path </param>
        private static void BackupFiles(List <string > fileNames  )
        {
            _logger.Info("Backing up files...");
            //get config backup dir
            //string targetPath = @"C:\GisSap_AssetSynch\backup";
            string targetPath = ConfigurationManager.AppSettings["BackUpFolder"];
            // Create a new target folder, if necessary. 
            if (!System.IO.Directory.Exists(targetPath))
            {
                _logger.Info("Creating Directory [ " + targetPath + " ]");
                System.IO.Directory.CreateDirectory(targetPath);
            }
            // Use Path class to manipulate file and directory paths. 
            foreach (string sourceFile in fileNames)
            {
                // To copy a folder's contents to a new location: 
                int index = sourceFile.Contains("\\") ? sourceFile.LastIndexOf("\\") :-1;
                //get the filename without path
                string fileName = sourceFile.Substring(index  + 1);
                string destFile = System.IO.Path.Combine(targetPath, fileName);

                _logger.Info("Copying file from  [ " + sourceFile + " ] to [ " + destFile + " ]");
                System.IO.File.Copy(sourceFile, destFile, true);
            }

            _logger.Info(string.Format("Files backed up to {0}", targetPath));
        }
    }
}
