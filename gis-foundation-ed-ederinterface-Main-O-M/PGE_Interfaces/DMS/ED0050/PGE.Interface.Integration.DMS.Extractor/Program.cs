using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interface.Integration.DMS;
using PGE.Interface.Integration.DMS.Common;
using PGE.Common.Delivery.Diagnostics;
using Miner.Geodatabase.Integration.Configuration;
using ESRI.ArcGIS.esriSystem;
using PGE.Interface.Integration.DMS.Manager;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using PGE_DBPasswordManagement;


namespace PGE.Interface.Integration.DMS.Extractor
{
    class Program
    {
        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        static void Main(string[] args)
        {
            ExtractorMessage msg = new ExtractorMessage();
            try
            {
                //RBAE - 11/12/13 - check to see if output and archive folders exist.  If not, try to create them.
                //If permissions to create fail, then exist
                try
                {
                    if (!System.IO.Directory.Exists(Configuration.Path))
                        System.IO.Directory.CreateDirectory(Configuration.Path);
                }
                catch (Exception ex)
                {
                    _log.Error("Could not create output folder " + Configuration.Path + " or folder does not exist");
                    return;
                }

                try
                {
                    if (!System.IO.Directory.Exists(Configuration.ArchivePath))
                        System.IO.Directory.CreateDirectory(Configuration.ArchivePath);
                }
                catch (Exception ex)
                {
                    _log.Error("Could not create archive folder " + Configuration.ArchivePath + " or folder does not exist");
                    return;
                }

                if (Configuration.Path == Configuration.ArchivePath)
                {
                    _log.Error("The output folder " + Configuration.Path + " and archive folder " + Configuration.Path + " must be different");
                    return;
                }

                if (args.Length > 0)
                {
                    msg = parseArgs(args);
                }
                else
                {
                    string line;
                    string current = "";
                    while ((line = Console.ReadLine()) != null)
                    {
                        switch (line)
                        {
                            case "!T":
                            case "!A":
                            case "!I":
                            case "!E":
                            case "!D":
                            case "!P":
                            case "!S":
                            case "!N":
                                current = line;
                                break;
                            default:
                                switch (current)
                                {
                                    case "!T":
                                        msg.ExtractType = (extracttype)Convert.ToInt32(line);
                                        break;
                                    case "!A":
                                        msg.AOR.Add(line);
                                        break;
                                    case "!I":
                                        msg.IncludeCircuits.Add(line);
                                        break;
                                    case "!E":
                                        msg.ExcludeCircuits.Add(line);
                                        break;
                                    case "!S":
                                        msg.Substations.Add(line);
                                        break;
                                    case "!D":
                                        msg.NumberOfDays = Convert.ToInt32(line);
                                        break;
                                    case "!P":
                                        msg.ProcessID = line;
                                        break;
                                    case "!N":
                                        msg.ServerName = line;
                                        break;
                                }
                                break;
                        }
                    }

                }

                string[] val = null;
               // string[] connectionEntities = Configuration.getCommaSeparatedList("StagingConnection", val);
                //string connection = String.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID={1})));", connectionEntities[0], connectionEntities[1]);
                string connection = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDGMC_ConnectionStr_dmsstaging"].ToUpper()) + "Pooling = true; Min Pool Size = 6;";
                msg.ServerName = connection;

                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine);
                //if it is not a batch process we need to manage it and break it into batch processes
                if (msg.ExtractType == extracttype.Bulk || msg.ExtractType == extracttype.Changes)
                {
                    ExtractorManager em = new ExtractorManager();
                    if (em.GetLicenses())
                    {
                        _log.Debug("Starting Manager.");
                        // #1
                        int code = em.Process(msg);
                        em.ReleaseLicenses();
                        Environment.Exit(code);
                    }
                    else
                    {
                        Console.WriteLine("Unable to get license.");
                        _log.Debug("Unable to get license.");
                    }
                }
                else if (msg.ExtractType == extracttype.Dump)
                {
                    ControlTable control = new ControlTable(Configuration.CadopsConnection);
                    control.RemoveDuplicates();
                    Manager.ExtractorManager.ExportToCSVfile(Configuration.CadopsConnection, Configuration.ExportTables);
                    string dataErrors = DataChecks.CheckData(Configuration.CadopsConnection);
                    try
                    {
                        string dir = DateTime.Now.ToString("yyyyMMddHHmmss");
                        File.WriteAllText(Configuration.Path + dir + "_log.txt", "\n\n\n" + dataErrors);

                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error writing log file.", ex);
                    }
                }
                else
                {
                    // #15
                    //#if DEBUG
                    //                    Debugger.Launch();
                    //                    Debugger.Break();
                    //#endif
                    ControlTable controlTable = new ControlTable(Configuration.CadopsConnection);

                    if (controlTable.CircuitCountByStatus(CircuitStatus.Finished, true, true) > 0)
                    {
                        NAStandalone NASubstation1 = new NAStandalone();
                        _log.Debug("Getting licenses");
                        if (NASubstation1.GetLicenses())
                        {
                            _log.Debug("Initializing Network Adapter for substation");
                            NetworkAdapterSettingsElement config = NAStandalone.GetNetworkAdapterSettings("PGE.Interface.Integration.DMS.Extractor.exe.config", "PGES");
                            NAStandalone.SDEelementConnection = System.Configuration.ConfigurationManager.AppSettings["PGES_SDEElement"];
                            if (NASubstation1.InitializeNetworkAdapter(config))
                            {
                                _log.Debug("Network Adapter Initialized for substation");
                                _log.Debug("Processing Message.");
                                NASubstation1.Execute(msg, true, false);
                            }
                            else
                            {
                                _log.Error("Unable to configure Network Adapter for substation.");
                            }
                            NASubstation1.ReleaseLicenses();
                            NASubstation1 = null;
                        }
                        else
                        {
                            _log.Debug("Unable to get license.");
                        }
                    }
                    else if (controlTable.CircuitCountByStatus(CircuitStatus.Finished, false, true) > 0)
                    {
                        NAStandalone NAElectric1 = new NAStandalone();
                        _log.Debug("Getting licenses");
                        if (NAElectric1.GetLicenses())
                        {
                            _log.Debug("Initializing Network Adapter for electric");
                            NetworkAdapterSettingsElement config = NAStandalone.GetNetworkAdapterSettings("PGE.Interface.Integration.DMS.Extractor.exe.config", "PGE");
                            NAStandalone.SDEelementConnection = System.Configuration.ConfigurationManager.AppSettings["PGE_SDEElement"];
                            if (NAElectric1.InitializeNetworkAdapter(config))
                            {
                                _log.Debug("Network Adapter Initialized for electric");
                                _log.Debug("Processing Message.");
                                NAElectric1.Execute(msg, false, false);
                            }
                            else
                            {
                                _log.Error("Unable to configure Network Adapter for electric.");
                            }
                            NAElectric1.ReleaseLicenses();
                        }
                        else
                        {
                            _log.Debug("Unable to get license.");
                        }
                    }
                    else if (controlTable.CircuitCountByStatus(CircuitStatus.Retry, true, false) > 0)
                    {
                        _log.Debug("Processing circuits set to retry");
                        NAStandalone NASubstation = new NAStandalone();
                        _log.Debug("Getting licenses");
                        if (NASubstation.GetLicenses())
                        {
                            _log.Debug("Initializing Network Adapter for substation");
                            NetworkAdapterSettingsElement config = NAStandalone.GetNetworkAdapterSettings("PGE.Interface.Integration.DMS.Extractor.exe.config", "PGES");
                            NAStandalone.SDEelementConnection = System.Configuration.ConfigurationManager.AppSettings["PGES_SDEElement"];
                            if (NASubstation.InitializeNetworkAdapter(config))
                            {
                                _log.Debug("Network Adapter Initialized for substation");
                                _log.Debug("Processing Message.");                                
                                NASubstation.Execute(msg, true, true);
                            }
                            else
                            {
                                _log.Error("Unable to configure Network Adapter for substation.");
                            }
                            NASubstation.ReleaseLicenses();
                            NASubstation = null;
                        }
                        else
                        {
                            _log.Debug("Unable to get license.");
                        }
                    }
                    else if (controlTable.CircuitCountByStatus(CircuitStatus.Retry, false, false) > 0)
                    {
                        _log.Debug("Processing circuits set to retry");
                        NAStandalone NAElectric = new NAStandalone();
                        _log.Debug("Getting licenses");
                        if (NAElectric.GetLicenses())
                        {
                            _log.Debug("Initializing Network Adapter for electric");
                            NetworkAdapterSettingsElement config = NAStandalone.GetNetworkAdapterSettings("PGE.Interface.Integration.DMS.Extractor.exe.config", "PGE");
                            NAStandalone.SDEelementConnection = System.Configuration.ConfigurationManager.AppSettings["PGE_SDEElement"];
                            if (NAElectric.InitializeNetworkAdapter(config))
                            {
                                _log.Debug("Network Adapter Initialized for electric");
                                _log.Debug("Processing Message.");
                                NAElectric.Execute(msg, false, true);
                            }
                            else
                            {
                                _log.Error("Unable to configure Network Adapter for electric.");
                            }
                            NAElectric.ReleaseLicenses();
                        }
                        else
                        {
                            _log.Debug("Unable to get license.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error", ex);
            }
        }
        private static ExtractorMessage parseArgs(string[] args)
        {
            // #20
            ExtractorMessage msg = new ExtractorMessage();
            for(int i = 0; i < args.Length; i++)
            {
                string s = args[i];
                if (s.ToUpper().Equals("-U"))
                {
                    msg.ExtractType = extracttype.Dump;
                }
                if (s.ToUpper().Equals("-A"))
                {
                    msg.ExtractType = extracttype.Bulk;
                }
                if (s.ToUpper().Equals("-C"))
                {
                    msg.ExtractType = extracttype.Changes;
                }
                if (s.ToUpper().Equals("-B"))
                {
                    msg.ExtractType = extracttype.Batch;
                }
                if (s.ToUpper().Equals("-P"))
                {
                    msg.ProcessID = args[i+1].ToString();
                }
                if (s.ToUpper().Equals("-N"))
                {
                    msg.ServerName = args[i+1].ToString();
                }
                if (s.ToUpper().Equals("-I"))
                {
                    string[] includeCircuits = Regex.Split(args[i + 1].ToString(), ",");
                    foreach (string circuit in includeCircuits)
                    {
                        msg.IncludeCircuits.Add(circuit);
                    }
                }
                if (s.ToUpper().Equals("-S"))
                {
                    string[] includeSubstations = Regex.Split(args[i + 1].ToString(), ",");
                    foreach (string substation in includeSubstations)
                    {
                        msg.Substations.Add(substation);
                    }
                }
                if (s.ToUpper().Equals("-T"))
                {
                    extracttype extractType = msg.ExtractType = (extracttype)Convert.ToInt32(args[i + 1].ToString());
                    /*
                    if (args.Length > i + 1)
                    {
                        msg.ExtractType = extracttype.Batch;
                        msg.ProcessID = "1";
                        msg.IncludeCircuits = args[i + 1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                    }
                    else
                    {
                        
                        //Manager.ExtractorManager.ExportToCSVfile(Configuration.CadopsConnection, Configuration.ExportTables);
                        msg.ExtractType = extracttype.Batch;
                        msg.ProcessID = "1";
                        //simple test subs
                        //msg.Substations.Add("16237");
                        //msg.Substations.Add("25442");
                        msg.Substations.Add("25400");
                        
                    }
                    */
                }
                if (s.ToUpper().Equals("-X"))
                {
                    if (args.Length > i + 1)
                    {
                        msg.ExtractType = extracttype.Batch;
                        msg.ProcessID = "1";
                        msg.Substations = args[i + 1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                    }
                    else
                    {
                        //Manager.ExtractorManager.ExportToCSVfile(Configuration.CadopsConnection, Configuration.ExportTables);
                        msg.ExtractType = extracttype.Batch;
                        msg.ProcessID = "1";
                        //simple test sub
                        msg.Substations.Add("1390000457");
                    }
                }
            }
            return msg;
        }
    }
}
