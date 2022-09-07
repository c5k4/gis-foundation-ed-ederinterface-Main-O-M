using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Oracle.DataAccess.Client;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;


namespace PGE.Interfaces.Settings.DivDist
{
    class Program
    {
        private static int _TotalServiceAreaCount;
        private static List<Process> _childProcessList;
        private static esriLicenseProductCode[] _prodCodes = { esriLicenseProductCode.esriLicenseProductCodeAdvanced };
        private static LicenseInitializer _licenceManager = new LicenseInitializer();
        private static List<IFeature> _serviceAreaFeatureList = new List<IFeature>();
        private static Dictionary<string, DataTable> _results = new Dictionary<string, DataTable>();
        

        static void Main(string[] args)
        {
            if (CheckLicense(_licenceManager))
            {
                try
                {
                    string table = ConfigurationManager.AppSettings["DataTable"];

                    IWorkspace workSpace = Common.SetWorkspace("SDEConnectionFile");
                    IFeatureWorkspace fWorkspace = workSpace as IFeatureWorkspace;

                    string serviceAreaClassName = ConfigurationManager.AppSettings["ServiceAreaFeatureClass"];

                    IFeatureClass serviceAreaFC = fWorkspace.OpenFeatureClass(serviceAreaClassName);

                    if (args.Length > 0)
                    {
                        int serviceAreaOID;
                        bool isOID = int.TryParse(args[0], out serviceAreaOID);

                        if (isOID)
                        {
                            IFeature serviceArea = serviceAreaFC.GetFeature(serviceAreaOID);
                            ProcessServiceArea(fWorkspace, serviceArea, serviceAreaOID.ToString());
                        }
                    }
                    else if(args.Length == 0)
                    {
                        _childProcessList = new List<Process>();
                        IFeatureCursor cursor = null;

                        using (OracleConnection oraConn = Common.GetDBConnection())
                        {
                            OracleCommand oraCmd = new OracleCommand("delete from GIS_DIVDIST", oraConn);
                            oraCmd.ExecuteNonQuery();
                        }

                        try
                        {
                            cursor = serviceAreaFC.Search(null, false);
                            IFeature serviceArea = null;

                            while ((serviceArea = cursor.NextFeature()) != null)
                            {
                                _serviceAreaFeatureList.Add(serviceArea);
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error: " + ex.Message);
                            Common.WriteToLog("Error: " + ex.Message, LoggingLevel.Error);
                            Environment.Exit(1);
                        }
                        finally
                        {
                            if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0);}
                            cursor = null;
                        }

                        Console.WriteLine("Processing Distrct-Division data for settings...");
                        Common.WriteToLog("Processing Distrct-Division data for settings...", LoggingLevel.Info);

                        _TotalServiceAreaCount = _serviceAreaFeatureList.Count;

                        foreach (IFeature serviceArea in _serviceAreaFeatureList)
                        {
                            CreateProcess(serviceArea.OID);
                        }

                        //wait untill all child processes finish their tasks.
                        while (_childProcessList.Count > 0)
                        {
                            Thread.Sleep(100);
                        }

                        //Delete duplicate rows if any...
                        string deleteSQL = "delete from GIS_DIVDIST a where rowid > (select min(rowid) from GIS_DIVDIST b where b.global_id = a.global_id and b.division = a.division and b.district = a.district)";
                        
                        using (OracleConnection oraConn = Common.GetDBConnection())
                        {
                            try
                            {
                                OracleCommand oraCmd = new OracleCommand(deleteSQL, oraConn);
                                oraCmd.ExecuteNonQuery();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error [Delete duplicate rows]: " + ex.Message);
                                Common.WriteToLog("Error: " + ex.Message, LoggingLevel.Error);
                            }
                        }

                        Console.WriteLine("Done!");
                        Common.WriteToLog("Done!", LoggingLevel.Info);
                        Console.WriteLine("");
                        Console.WriteLine("Press 'Enter' to exit...");
                        Console.ReadLine();
                    }
                    
                }
                catch(Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Common.WriteToLog("Error: " + ex.Message, LoggingLevel.Error);
                    Environment.Exit(1);
                }
            }
            else
            {
                Environment.Exit(1);
            }

            //Exit gracefully...
            Environment.Exit(0);
        }

        private static void CreateProcess(int oID)
        {
            string argument = oID.ToString();

            ProcessStartInfo processStartInfo = new ProcessStartInfo("PGE.Interfaces.Settings.DivDist.exe", argument);
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.CreateNoWindow = true;

            Process proc = new Process();
            proc.StartInfo = processStartInfo;
            proc.EnableRaisingEvents = true;
            proc.OutputDataReceived += new DataReceivedEventHandler(proc_OutputDataReceived);
            proc.Exited += new EventHandler(proc_Exited);
            proc.Start();
            proc.BeginOutputReadLine();

            _childProcessList.Add(proc);
        }

        private static void ProcessServiceArea(IFeatureWorkspace fWorkspace, IFeature serviceArea, string key)
        {
            DivDistUtility utility = new DivDistUtility(fWorkspace);

            //Console.WriteLine("Processing ServiceArea [ OID:" + key + " ]");

            var result = utility.Process(serviceArea);

            string table = ConfigurationManager.AppSettings["DataTable"];
            if (result.Rows.Count > 0)
            {
                DivDistUtility.SaveToDatabase(result, table);
            }
            Console.WriteLine("ServiceArea [ OID:" + key + " ] processed successfully...");
        }

        /// <summary>
        /// Event handler that prints process messages in the console 
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">DataReceivedEventArgs</param>
        static void proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                try
                {
                    Console.WriteLine(e.Data);
                }
                catch (Exception ex)
                {
                    Common.WriteToLog("Settings DistDiv processing on error: " + ex.Message, LoggingLevel.Error);
                }
            }
        }


        /// <summary>
        /// Event handler that fires when each process exits
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">EventArgs</param>
        static void proc_Exited(object sender, EventArgs e)
        {
            try
            {
                _childProcessList.Remove(sender as Process);

                if ((sender as Process).ExitCode != 0)
                {
                    Console.WriteLine("Settings DistDiv processing aborted due to error " + DateTime.Now.ToLocalTime());
                    Common.WriteToLog("Settings DistDiv processing aborted due to error " + DateTime.Now.ToLocalTime(), LoggingLevel.Error);

                    Environment.Exit(1);
                }
                else
                {
                    double currentProgress = Math.Round(((Convert.ToDouble(_TotalServiceAreaCount) - Convert.ToDouble(_childProcessList.Count)) / Convert.ToDouble(_TotalServiceAreaCount)) * 100.00);

                    Console.WriteLine("Total Progress: " + currentProgress.ToString() + "%");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Common.WriteToLog("Error: " + ex.Message, LoggingLevel.Error);
            }
        }


        /// <summary>
        /// Event handler which responds on CTRL+C key press
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="e">ConsoleCancelEventArgs</param>
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Shutting down...");
        }

        
        /// <summary>
        /// Initializes ArcGIS license
        /// </summary>
        /// <param name="licenceManager"></param>
        /// <returns></returns>
        private static bool CheckLicense(LicenseInitializer licenceManager)
        {
            bool isOk = false;

            try
            {
                isOk = licenceManager.InitializeApplication(_prodCodes);
                //isOk = licenceManager.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize license : " + ex.Message);
                Common.WriteToLog("Failed to initialize license : " + ex.Message, LoggingLevel.Error);
            }

            return isOk;
        }
    }
}
