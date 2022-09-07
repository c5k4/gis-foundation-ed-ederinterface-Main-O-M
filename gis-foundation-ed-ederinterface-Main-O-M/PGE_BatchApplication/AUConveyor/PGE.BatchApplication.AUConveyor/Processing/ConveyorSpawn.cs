using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using PGE.BatchApplication.AUConveyor.Utilities;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.AUConveyor.Processing
{
    /// <summary>
    /// Contains the logic necessary to create child processes.
    /// </summary>
    public class ConveyorSpawn
    {
        Conveyor _baseConveyor;
        int _processesRunning = 0;
        Dictionary<Process, int> _processVersions;
        List<int> _childVersionsToDelete;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseConveyer">The parent conveyor used in this process.</param>
        public ConveyorSpawn(Conveyor baseConveyer)
        {
            _baseConveyor = baseConveyer;
        }

        /// <summary>
        /// Manages process creation.
        /// </summary>
        public void SpawnThreads()
        {
            //Split the work.
            List<ProcessOIDInfo> featureLoad = _baseConveyor.SplitFeatureLoad();

            //Write the input files that the spawned processes will use
            WriteInputFile(featureLoad);
        }

        /// <summary>
        /// Executes each process and waits for every process to finish.
        /// </summary>
        public void StartThreads()
        {
            _processesRunning = 0;

            string executingLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingLocation = executingLocation.Substring(0, executingLocation.LastIndexOf("\\"));

            string[] inputFiles = Directory.GetFiles(ToolSettings.Instance.InputFileDirectory);
            _baseConveyor.DeleteChildVersions(ToolSettings.Instance.VersionName, inputFiles.Length);

            LogManager.WriteLine("    Starting processes...");
            _processVersions = new Dictionary<Process, int>();
            _childVersionsToDelete = new List<int>();
            int counter = 0;
            foreach (string fileName in inputFiles)
            {
                _baseConveyor.CreateChildVersion(ToolSettings.Instance.VersionName, counter);

                //string fName = fileName.Substring(fileName.LastIndexOf("\\") + 1);
                string arguments = ToolSettings.Instance.ToArguments(counter, fileName);

                ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
                {
                    CreateNoWindow = ToolSettings.Instance.HiddenProcessing,
                    WindowStyle = (ToolSettings.Instance.HiddenProcessing ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal),
                    UseShellExecute = !ToolSettings.Instance.HiddenProcessing,
                    RedirectStandardOutput = ToolSettings.Instance.HiddenProcessing
                };
                Process childProcess = new Process();
                childProcess.StartInfo = startInfo;
                childProcess.EnableRaisingEvents = true;
                childProcess.Exited += new EventHandler(childProcess_Exited);
                _processVersions.Add(childProcess, counter);

                _processesRunning++;
                childProcess.Start();
                counter++;
            }

            //Wait for child processes to go away before leaving this area.
            LogManager.WriteLine("    Waiting for child processes to finish.", ConsoleColor.Cyan, null);
            while (_processesRunning > 0)
                Thread.Sleep(5000);

            //Delete any child versions that sent back a code to do so unless the DoNotPostChildVersions flag was set
            if (_childVersionsToDelete != null && _childVersionsToDelete.Count > 0 && ToolSettings.Instance.DoNotPostChildVersions == false)
            {
                foreach(int append in _childVersionsToDelete)
                    _baseConveyor.DeleteChildVersion(ToolSettings.Instance.VersionName, append);
            }
        }

        /// <summary>
        /// Writes input files.
        /// </summary>
        /// <param name="featureLoad">
        ///     A list of class and object ID information indicating the objects that will be written
        ///     to this input file.
        /// </param>
        private void WriteInputFile(List<ProcessOIDInfo> featureLoad)
        {
            LogManager.WriteLine("    Writing input files.");
            if (Directory.Exists(ToolSettings.Instance.InputFileDirectory))
                Directory.Delete(ToolSettings.Instance.InputFileDirectory, true);

            Directory.CreateDirectory(ToolSettings.Instance.InputFileDirectory);

            StreamWriter[] writers = new StreamWriter[ToolSettings.Instance.NumProcesses];
            for (int i = 0; i < ToolSettings.Instance.NumProcesses; i++)
                writers[i] = new StreamWriter(ToolSettings.Instance.InputFileDirectory + "\\" + ToolSettings.InputFilePrefix + i + ".txt");

            int processedcnt = 0;
            for (int i = 0; i < featureLoad.Count; i++)
            {
                int delegatedProcess = (processedcnt % ToolSettings.Instance.NumProcesses);
                writers[delegatedProcess].WriteLine(featureLoad[i].ClassID + "." + featureLoad[i].OID);
                processedcnt++;
            }

            for (int i = 0; i < ToolSettings.Instance.NumProcesses; i++)
            {
                writers[i].Flush();
                writers[i].Close();
            }
        }

        /// <summary>
        /// Used to indicate that one fewer process is running. When this reaches 0, the program will continue.
        /// </summary>
        private void childProcess_Exited(object sender, EventArgs e)
        {
            try
            {
                //Exit code determines whether or not to post the version.
                int versionAppend = _processVersions[sender as Process];
                ProcessBitgate exitCode = new ProcessBitgate((sender as Process).ExitCode);
                string thisVersionName = PGE.BatchApplication.AUConveyor.Conveyor.GetAppendedVersionName(ToolSettings.Instance.VersionName, versionAppend, true);

                if (exitCode.DeleteVersion)
                    _childVersionsToDelete.Add(versionAppend);
                else if (ToolSettings.Instance.DoNotPostChildVersions == true)
                    LogManager.WriteLine("The auto-generated version " + thisVersionName + " remains for reconciling & posting to the parent");
                else
                    Program.WriteWarning("The auto-generated version " + thisVersionName + " was not deleted. Ensure that the version posted correctly.");

                if (exitCode.ErrorsEncountered)
                    Program.WriteError("Critical errors were generated from the version " + thisVersionName + ". Check the logs within the input files folder for more information.", false);

                if (exitCode.WarningsEncountered)
                    Program.WriteWarning("Warnings were generated from the version " + thisVersionName + ". Check the logs within the input files folder for more information.");
            }
            catch { }

            _processesRunning--;
        }
    }
}
