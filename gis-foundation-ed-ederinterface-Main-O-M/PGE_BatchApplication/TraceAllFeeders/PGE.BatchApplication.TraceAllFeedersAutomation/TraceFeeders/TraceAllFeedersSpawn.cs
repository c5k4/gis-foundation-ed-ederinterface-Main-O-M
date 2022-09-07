using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Configuration;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using PGE.BatchApplication.TraceAllFeeders.Common;

namespace PGE.BatchApplication.TraceAllFeeders.TraceFeeders
{
    /// <summary>
    /// The purpose of this class is to read in what maps need to be produced and then to spawn new processes
    /// to execute the map production based on this information.
    /// </summary>
    public class TraceAllFeedersSpawn
    {
        #region Private Vars

        private IWorkspace workspace = null;
        private int NumProcessesPer = 1;
        private string DirectConnectString = "";
        private string User = "";
        private string Pass = "";
        private string sdeUser = "";
        private string sdePass = "";
        private Dictionary<string, int> feederIDsDictionary = new Dictionary<string, int>();
        private Queue<string> feederIDs = new Queue<string>();
        private int currentFeeder = 0;
        int feedersTraced = 0;
        private string NetworkName;
        private Dictionary<int, Process> runningProcesses;
        StreamWriter writer = null;
        StreamWriter finishedFeedersWriter = null;
        MailSlot toReTraceMailSlot = new MailSlot(Common.Common.TraceAllReTraceListMailSlot);
        MailSlot FinishedTracingMailSlot = new MailSlot(Common.Common.TraceAllFinishedMailSlot);
        int TotalFeederIDsToTrace = 0;
        Process recAndPost = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directConnectString">Direct connect string to geodatabase (i.e. sde:oracle11g:/;local=EDGISA1T)</param>
        public TraceAllFeedersSpawn(string directConnectString, string user, string pass, string networkName, string numProcesses, string sdeUser, string sdePass)
        {
            this.sdeUser = sdeUser;
            this.sdePass = sdePass;
            NumProcessesPer = Int32.Parse(numProcesses);
            workspace = Common.Common.OpenWorkspace(directConnectString, user, pass);

            if (workspace == null)
            {
                Console.WriteLine("Unable to connect to the specified database.");
                return;
            }

            if (!Directory.Exists(Common.Common.LogFileDirectory))
            {
                Directory.CreateDirectory(Common.Common.LogFileDirectory);
            }
            writer = new StreamWriter(Common.Common.LogFileDirectory + "\\" + "TraceAllFeedersMain" + ".txt", true);
            Log("Process ID: " + Process.GetCurrentProcess().Id);

            finishedFeedersWriter = new StreamWriter(Common.Common.LogFileDirectory + "\\" + "FinishedFeeders" + ".txt", true);

            this.DirectConnectString = directConnectString;
            this.User = user;
            this.Pass = pass;
            this.NetworkName = networkName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method will spawn all of the processes that will perform the export of the maps
        /// </summary>
        /// <param name="AllMapGrids"></param>
        public void SpawnThreads()
        {
            StartReconcileAndPostProcess();

            //Determine all of the feederIDs first.
            GetFeederIDList();

            //Now I should know what Mxds and scales I need to start processes with.
            StartThreads();

            recAndPost.Kill();
        }

        #endregion

        #region Private Methods

        public void StartReconcileAndPostProcess()
        {
            string arguments = "-i " + DirectConnectString + " -u " + User + " -p " + Pass + " -su " + sdeUser + " -sp " + sdePass + " -r";

            ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            recAndPost = Process.Start(startInfo);
        }

        /*
        /// <summary>
        /// This method will sort our list of feederids based on how many of each there are.
        /// </summary>
        private void sortFeederIDs()
        {
            List<KeyValuePair<string, int>> list = feederIDsDictionary.ToList();
            list.Sort((x,y)=>x.Value.CompareTo(y.Value));

            foreach (KeyValuePair<string, int> kvp in list)
            {
                feederIDs.Add(kvp.Key);
            }
        }
        */

        public void Log(string log)
        {
            writer.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.Flush();
        }

        public void LogFinishedFeeder(string feederID)
        {

            finishedFeedersWriter.WriteLine(feederID);
            finishedFeedersWriter.Flush();
        }

        /// <summary>
        /// This method will obtain the entire feeder id list from the circuit source table
        /// </summary>
        private void GetFeederIDList()
        {
            IGeometricNetwork geomNetwork = Common.Common.GetNetwork(workspace, NetworkName);

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);
            IMMFeederExt feederExt = obj as IMMFeederExt;
            IMMFeederSpace feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
            IMMEnumFeederSource feederSources = feederSpace.FeederSources;
            IMMFeederTracer feederTracer = new MMFeederTracer();
            feederSources.Reset();
            try
            {
                IMMFeederSource feederSource = feederSources.Next();
                while (feederSource != null)
                {
                    if (!feederIDs.Contains(feederSource.FeederID.ToString()))
                    {
                        feederIDs.Enqueue(feederSource.FeederID.ToString());
                    }
                    if (feederSource != null) { while (Marshal.ReleaseComObject(feederSource) > 0) { } }
                    feederSource = feederSources.Next();
                }
            }
            finally
            {
                if (feederSpace != null) { while (Marshal.ReleaseComObject(feederSpace) > 0);}
                if (feederSources != null) { while (Marshal.ReleaseComObject(feederSources) > 0);}
                if (feederTracer != null) { while (Marshal.ReleaseComObject(feederTracer) > 0);}
                if (feederIDs != null) { TotalFeederIDsToTrace = feederIDs.Count; }
            }
        }

        /// <summary>
        /// This method will fire all of the threads that will perform that actual tracing
        /// </summary>
        private void StartThreads()
        {
            //Create our base version first
            CreateBaseVersion();

            runningProcesses = new Dictionary<int, Process>();

            string executingLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingLocation = executingLocation.Substring(0, executingLocation.LastIndexOf("\\"));

            Console.WriteLine("Starting processes to trace feeders...");
            int counter = 0;
            for (int i = 0; i < NumProcessesPer; i++)
            {
                if ((feedersTraced < TotalFeederIDsToTrace) && feederIDs.Count > 0)
                {
                    string fName = "Input" + counter + ".txt";
                    WriteInputFile(fName);
                    string arguments = "-i " + DirectConnectString + " -u " + User + " -p " + Pass + " -n " + NetworkName
                        + " -I \"" + executingLocation + "\\" + Common.Common.InputFileDirectory + "\\" + fName + "\"" + " -v "
                        + Common.Common.BaseChildVersion + counter + " -su " + sdeUser + " -sp " + sdePass;

                    ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
                    {
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        RedirectStandardOutput = true
                    };

                    runningProcesses.Add(counter, Process.Start(startInfo));
                    counter++;
                }
            }

            TrackProcessesProgress();
        }

        /// <summary>
        /// This will trace all of the running processes so that when they are finished it will spawn another new process with another
        /// 5 new feeders.
        /// </summary>
        private void TrackProcessesProgress()
        {
            string executingLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingLocation = executingLocation.Substring(0, executingLocation.LastIndexOf("\\"));
            int numProcesses = runningProcesses.Count;
            
            while (runningProcesses.Count > 0)
            {
                for (int i = 0; i < numProcesses; i++)
                {
                    if (runningProcesses[i] != null && runningProcesses[i].HasExited)
                    {
                        if ((feedersTraced < TotalFeederIDsToTrace) && feederIDs.Count > 0)
                        {
                            string fName = "Input" + i + ".txt";
                            WriteInputFile(fName);
                            string arguments = "-i " + DirectConnectString + " -u " + User + " -p " + Pass + " -n " + NetworkName
                                + " -I \"" + executingLocation + "\\" + Common.Common.InputFileDirectory + "\\" + fName + "\"" + " -v "
                                + Common.Common.BaseChildVersion + i + " -su " + sdeUser + " -sp " + sdePass;

                            ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
                            {
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                UseShellExecute = false,
                                RedirectStandardOutput = true
                            };
                            runningProcesses[i] = Process.Start(startInfo);
                        }
                        else if (feedersTraced >= TotalFeederIDsToTrace)
                        {
                            runningProcesses.Remove(i);
                        }
                    }
                    else if (runningProcesses[i] == null)
                    {
                        //Start a new process
                        if ((feedersTraced < TotalFeederIDsToTrace) && feederIDs.Count > 0)
                        {
                            string fName = "Input" + i + ".txt";
                            WriteInputFile(fName);
                            string arguments = "-i " + DirectConnectString + " -u " + User + " -p " + Pass + " -n " + NetworkName
                                + " -I \"" + executingLocation + "\\" + Common.Common.InputFileDirectory + "\\" + fName + "\"" + " -v "
                                + Common.Common.BaseChildVersion + i + " -su " + sdeUser + " -sp " + sdePass;

                            ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
                            {
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                UseShellExecute = false,
                                RedirectStandardOutput = true
                            };
                            runningProcesses[i] = Process.Start(startInfo);
                        }
                        else if (feedersTraced >= TotalFeederIDsToTrace)
                        {
                            runningProcesses.Remove(i);
                        }
                    }
                }

                CheckMailSlots();
                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// Checks our mail slots to determine if there are more feederIDs that need to be re-run (due to conflicts) and also updates
        /// the list of feeders that have been finished by the reconcile and post thread. 
        /// </summary>
        private void CheckMailSlots()
        {
            List<string> toReTrace = toReTraceMailSlot.ReadMailslotMessages();
            foreach (string feederID in toReTrace)
            {
                feederIDs.Enqueue(feederID);
            }

            List<string> finishedFeeders = FinishedTracingMailSlot.ReadMailslotMessages();
            foreach (string feederID in finishedFeeders)
            {
                feedersTraced++;
                LogFinishedFeeder(feederID);
            }

            double DoublePercentComplete = (((double)feedersTraced / (double)TotalFeederIDsToTrace) * 100.0);
            Console.Write("\rTraced " + feedersTraced + " out of " + TotalFeederIDsToTrace + " CircuitIDs: " + Math.Floor(DoublePercentComplete) + "% completed");
            Log("\rTraced " + feedersTraced + " out of " + TotalFeederIDsToTrace + " CircuitIDs: " + Math.Floor(DoublePercentComplete) + "% completed");
        }

        /// <summary>
        /// Create our base version which all of our child versions will be based off of for our tracing
        /// </summary>
        private void CreateBaseVersion()
        {
            IVersionedWorkspace versionedWorkspace = workspace as IVersionedWorkspace;
            IVersion baseVersion = null;
            try
            {
                baseVersion = versionedWorkspace.FindVersion(Common.Common.BaseVersion);
            }
            catch (Exception e) { }

            if (baseVersion != null)
            {
                //Try to delete the children first
                IVersionInfo baseVersionInfo = baseVersion.VersionInfo;
                IEnumVersionInfo childInfo = baseVersionInfo.Children;
                childInfo.Reset();
                IVersionInfo childVersionInfo = childInfo.Next();
                while (childVersionInfo != null)
                {
                    IVersion childVersion = null;
                    try
                    {
                        childVersion = versionedWorkspace.FindVersion(childVersionInfo.VersionName);
                        childVersion.Delete();
                    }
                    catch (Exception e) { Log("Unable to delete child version " + childVersionInfo.VersionName); }
                    if (childVersion != null) { while (Marshal.ReleaseComObject(childVersion) > 0) { } }
                    childVersionInfo = childInfo.Next();
                }

                baseVersion.Delete();
                while (Marshal.ReleaseComObject(baseVersion) > 0) { }
            }

            IVersion newVersion = versionedWorkspace.DefaultVersion.CreateVersion(Common.Common.BaseVersion);
            while (Marshal.ReleaseComObject(newVersion) > 0) { }
        }

        /// <summary>
        /// This process will split the map numbers that need to be generated into many different input files based on
        /// the number of processes to spawn
        /// </summary>
        private void WriteInputFile(string inputFileName)
        {
            //Delete our input file directory before we start creating new input files
            if (!Directory.Exists(Common.Common.InputFileDirectory))
            {
                Directory.CreateDirectory(Common.Common.InputFileDirectory);
            }

            //Delete any current input file with this name
            if (File.Exists(Common.Common.InputFileDirectory + "\\" + inputFileName))
            {
                File.Delete(Common.Common.InputFileDirectory + "\\" + inputFileName);
            }

            StreamWriter writer = new StreamWriter(Common.Common.InputFileDirectory + "\\" + inputFileName);

            for (int i = 0; i < 1; i++)
            {
                if (feederIDs.Count > 0)
                {
                    writer.WriteLine(feederIDs.Dequeue());
                }
                currentFeeder++;
            }

            writer.Flush();
            writer.Close();

        }

        #endregion

    }
}
