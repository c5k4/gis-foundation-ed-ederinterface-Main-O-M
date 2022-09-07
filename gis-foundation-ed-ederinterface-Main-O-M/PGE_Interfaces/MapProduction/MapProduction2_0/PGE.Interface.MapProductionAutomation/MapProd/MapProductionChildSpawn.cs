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
using MapProductionAutomation.Common;
using ESRI.ArcGIS.Geometry;
using System.Reflection;
using System.Threading;

namespace MapProductionAutomation.MapProd
{
    /// <summary>
    /// The purpose of this class is to read in what maps need to be produced and then to spawn new processes
    /// to execute the map production based on this information.
    /// </summary>
    public class MapProductionChildSpawn
    {
        #region Private Vars

        //Holds the process and the logfile name that it will use.
        Dictionary<Process, string> RunningProcesses = new Dictionary<Process, string>();
        Dictionary<Process, TimeSpan> RunningProcessesIdleCheck = new Dictionary<Process, TimeSpan>();

        ITable changeDetectionTable = null;
        IWorkspace workspace = null;
        private StreamWriter writer = null;
        private string logFileName = "";

        //The below will hold the passed in arguments in case this process must restart itself
        private string sdeConnectionFile;
        private string mxdLocation;
        private string mapNoInputLocation;
        private string expLocation;
        private string tpLocation; 
        private string TemplateName;
        private string scale;
        private bool overwrite;
        private string hasDataLayers;
        private string StoredDisplayName;
        private string format;
        private int numProcesses = 1;
        private long idleThreshold = 60000;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directConnectString">Direct connect string to geodatabase (i.e. sde:oracle11g:/;local=EDGISA1T)</param>
        public MapProductionChildSpawn(string sdeConnectionFile, string mxdLocation, string expLocation, string tempLocation,
            string TemplateName, string scale, bool overwrite, string hasDataLayers, string StoredDisplayName,
            string format, int numProcesses)
        {
            //Store our arguments in case we need to restart this process
            this.sdeConnectionFile = sdeConnectionFile;
            this.mxdLocation = mxdLocation;
            this.expLocation = expLocation;
            this.tpLocation = tempLocation; 
            this.TemplateName = TemplateName;
            this.scale = scale;
            this.overwrite = overwrite;
            this.hasDataLayers = hasDataLayers;
            this.StoredDisplayName = StoredDisplayName;
            this.format = format;
            this.numProcesses = numProcesses;

            workspace = Common.Common.OpenWorkspace(sdeConnectionFile);

            Common.Common.ReadAppSettings();
            this.format = format;

            CreateLogFile();
        }

        #endregion

        #region Public Methods

        public void Log(string log)
        {
            //Console.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.Flush();
        }

        public void SpawnChildren()
        {
            //Insert all of our map numbers to process for the children
            InsertMapNumbersForChildProcessing();

            //Reset all error flags so the children will retry them
            SetErrorsToNotProcessed();

            if (numProcesses > 0)
            {
                SpawnChildrenProcesses();
            }

            //Monitor our processes until they all finish and there are no maps left to process
            MonitorChildProcesses();

            Log("Finished processing");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This process will monitor all child processes and will restart them as necessary
        /// </summary>
        private void MonitorChildProcesses()
        {
            TimeSpan idleProcessThreshold = TimeSpan.FromMilliseconds(idleThreshold);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            bool continueRunning = true;
            while (continueRunning)
            {
                bool allExited = true;

                foreach (KeyValuePair<Process, string> kvp in RunningProcesses)
                {
                    if (kvp.Key.HasExited)
                    {
                        if (mapsLeftToProcess())
                        {
                            //Updating the dictionary so we will need to break out of the loop
                            string log = kvp.Value;
                            RunningProcesses.Remove(kvp.Key);
                            RunningProcessesIdleCheck.Remove(kvp.Key);
                            Process p = SpawnProcess(kvp.Value);
                            RunningProcesses.Add(p, log);
                            RunningProcessesIdleCheck.Add(p, TimeSpan.FromMilliseconds(-1*idleThreshold));
                            allExited = false;
                            break;
                        }
                    }
                    else
                    {
                        allExited = false;
                    }
                }

                //Every fifteen minutes let's check on all the processes to ensure that none are idle and not working
                //any longer
                if (stopWatch.ElapsedMilliseconds > 900000)
                {
                    Dictionary<Process, TimeSpan> newValues = new Dictionary<Process, TimeSpan>();
                    foreach (KeyValuePair<Process, TimeSpan> kvp in RunningProcessesIdleCheck)
                    {
                        if (!kvp.Key.HasExited)
                        {
                            TimeSpan end_cpu_time = kvp.Key.TotalProcessorTime;
                            if ((end_cpu_time - kvp.Value) < idleProcessThreshold)
                            {
                                //Process appears to be idle.  Let's kill it so it restarts on next check.
                                kvp.Key.Kill();
                            }
                            else
                            {
                                //Process does not appear to be idle.  Let's update the cpu time
                                newValues.Add(kvp.Key, kvp.Key.TotalProcessorTime);
                            }
                        }
                    }

                    //Now update our dictionary since we can't update it in the loop.
                    foreach (KeyValuePair<Process, TimeSpan> kvp in newValues)
                    {
                        RunningProcessesIdleCheck[kvp.Key] = kvp.Value;
                    }

                    //restart our stopWatch
                    stopWatch.Stop();
                    stopWatch.Reset();
                    stopWatch.Start();
                }

                if (allExited) { continueRunning = false; }

                Thread.Sleep(5000);
            }
        }

        private string AssemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;

        private Process SpawnProcess(string logFileName)
        {
            string arguments = 
                "-c \"" + sdeConnectionFile + "\" " + 
                "-M \"" + mxdLocation + 
                "\" -I \"" + mapNoInputLocation +
                "\" -E \"" + expLocation + 
                "\" -T \"" + TemplateName +
                "\" -TL \"" + tpLocation +  
                "\" -S " + scale +
                " -sd \"" + StoredDisplayName + "\"" + 
                " -f " + format + 
                " -l " + logFileName;
            if (overwrite) { arguments += " -O"; }
            if (!String.IsNullOrEmpty(hasDataLayers)) { arguments += " -D " + hasDataLayers; }

            Log("Process Arguments: " + AssemblyLocation + " " + arguments);

            ProcessStartInfo startInfo = new ProcessStartInfo(AssemblyLocation, arguments)
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };
            Process p = Process.Start(startInfo);
            return p;
        }

        private void SpawnChildrenProcesses()
        {
            for (int i = 0; i < numProcesses; i++)
            {
                try
                {
                    Log("Obtaining next log file name");
                    string logFileName = GetNextLogFileName();
                    Log("Logfile: " + logFileName);

                    Log("Starting process");
                    Process p = SpawnProcess(logFileName);

                    Log("Adding process to monitor list");
                    RunningProcesses.Add(p, logFileName);
                    RunningProcessesIdleCheck.Add(p, TimeSpan.FromMilliseconds(0));
                    Log("Finished spawning process" + i);

                    //Pause before starting each new process
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    Log("Failed to start new process. Message: " + e.Message);
                }
            }
        }

        private bool mapsLeftToProcess()
        {
            bool mapsLeftToProcess = false;

            try
            {
                if (changeDetectionTable == null)
                {
                    changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
                }

                int mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
                int mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);


                //First determine any rows that already exist for this map type.
                IQueryFilter qf = new QueryFilterClass();
                qf.SubFields = Common.Common.FieldChangeDetectionMapNumber + "," + Common.Common.FieldChangeDetectionMapType;
                qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " = '" + TemplateName + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'"
                    + " AND " + Common.Common.FieldChangeDetectionExportState + " is null";
                ICursor rowCursor = changeDetectionTable.Search(qf, true);
                IRow changeDetectionRow = rowCursor.NextRow();

                if (changeDetectionRow != null)
                {
                    mapsLeftToProcess = true;
                }

                if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
                if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            }
            catch (Exception e)
            {
                Log("Failed checking maps left to process.  Will retry shortly");
                mapsLeftToProcess = true;

                //If something fails let's refresh our connection to the database
                if (workspace != null) { while (Marshal.ReleaseComObject(workspace) > 0) { } }
                if (changeDetectionTable != null) { while (Marshal.ReleaseComObject(changeDetectionTable) > 0) { } }

                workspace = Common.Common.OpenWorkspace(sdeConnectionFile);
                changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
            }
            return mapsLeftToProcess;
        }

        private string GetNextLogFileName()
        {
            StreamWriter tempWriter = null;
            string tempLogFileName = "";

            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf("\\"));
            string logFileDirectory = System.IO.Path.Combine(path, Common.Common.LogFileDirectory);
            string day = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            if (day.Length == 1) { day = "0" + day; }
            if (month.Length == 1) { month = "0" + month; }
            string year = DateTime.Today.Year.ToString();
            logFileDirectory = System.IO.Path.Combine(logFileDirectory, year + month + day);
            if (!Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFileDirectory);
            }

            int num = 0;
            bool createLogfile = true;

            while (createLogfile)
            {
                if (!File.Exists(logFileDirectory + "\\" + TemplateName + "_" + scale + "_Log" + num + ".txt"))
                {
                    try
                    {
                        tempWriter = new StreamWriter(logFileDirectory + "\\" + TemplateName + "_" + scale + "_Log" + num + ".txt", true);
                        tempLogFileName = TemplateName + "_" + scale + "_Log" + num + ".txt";
                        createLogfile = false;
                    }
                    catch { }
                }
                num++;
            }

            try
            {
                if (tempWriter != null)
                {
                    tempWriter.Close();
                }
            }
            catch { }

            return tempLogFileName;
        }

        private void CreateLogFile()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf("\\"));
            string logFileDirectory = System.IO.Path.Combine(path, Common.Common.LogFileDirectory);
            string day = DateTime.Today.Day.ToString();
            string month = DateTime.Today.Month.ToString();
            if (day.Length == 1) { day = "0" + day; }
            if (month.Length == 1) { month = "0" + month; }
            string year = DateTime.Today.Year.ToString();
            logFileDirectory = System.IO.Path.Combine(logFileDirectory, year + month + day);
            if (!Directory.Exists(logFileDirectory))
            {
                Directory.CreateDirectory(logFileDirectory);
            }

            int num = 0;
            bool createLogfile = true;

            while (createLogfile)
            {
                if (!File.Exists(logFileDirectory + "\\" + TemplateName + "_" + scale + "_Parent" + "_Log" + num + ".txt"))
                {
                    try
                    {
                        writer = new StreamWriter(logFileDirectory + "\\" + TemplateName + "_" + scale + "_Parent" + "_Log" + num + ".txt", true);
                        logFileName = TemplateName + "_" + scale + "_Parent" + "_Log" + num + ".txt";
                        createLogfile = false;
                    }
                    catch { }
                }
                num++;
            }
        }

        /// <summary>
        /// This method will set any rows for this map type and scale combination that currently
        /// have an export state value that refers to an error.  This way every run of the tool
        /// will retry the rows that errored out on the previous attempt.
        /// </summary>
        private void SetErrorsToNotProcessed()
        {
            if (changeDetectionTable == null)
            {
                changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
            }

            int mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
            int mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);
            int scaleIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionScale);
            int exportStateIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionExportState);

            //First determine any rows that already exist for this map type.
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = Common.Common.FieldChangeDetectionExportState;
            qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " = '" + TemplateName 
                + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'"
                + " AND " + Common.Common.FieldChangeDetectionExportState + " = '" + Common.Common.GetExportState(Common.Common.ExportStates.Error) + "'";
            ICursor rowCursor = changeDetectionTable.Update(qf, true);
            IRow changeDetectionRow = rowCursor.NextRow();

            while (changeDetectionRow != null)
            {
                changeDetectionRow.set_Value(exportStateIndex, "");
                changeDetectionRow.Store();
                changeDetectionRow = rowCursor.NextRow();
            }
        }

        /// <summary>
        /// Adds new rows to the change detection grids table for the child processes to utilize
        /// </summary>
        private void InsertMapNumbersForChildProcessing()
        {
            try
            {
                if (changeDetectionTable == null)
                {
                    changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
                }

                int mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
                int mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);
                int scaleIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionScale);

                string currentMapNum = "";
                List<string> alreadyExistingMapNumbers = new List<string>();


                //First determine any rows that already exist for this map type.
                IQueryFilter qf = new QueryFilterClass();
                qf.SubFields = Common.Common.FieldChangeDetectionMapNumber + "," + Common.Common.FieldChangeDetectionMapType;
                qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " = '" + TemplateName + "' AND " + Common.Common.FieldChangeDetectionScale + " = " + scale;
                ICursor rowCursor = changeDetectionTable.Search(qf, true);
                IRow changeDetectionRow = rowCursor.NextRow();

                while (changeDetectionRow != null)
                {
                    currentMapNum = changeDetectionRow.get_Value(mapNumIndex).ToString();
                    alreadyExistingMapNumbers.Add(currentMapNum);
                    changeDetectionRow = rowCursor.NextRow();
                }

                if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
                if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

                List<string> mapNumbersToAdd = new List<string>();
                //Now that we know which maps already exist in the table, lets add those that don't yet
                qf = new QueryFilterClass();
                qf.SubFields = Common.Common.FieldChangeDetectionMapNumber;
                qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " is null" + " AND " + Common.Common.FieldChangeDetectionScale + " = " + scale;
                rowCursor = changeDetectionTable.Search(qf, true);
                changeDetectionRow = rowCursor.NextRow();

                while (changeDetectionRow != null)
                {
                    currentMapNum = changeDetectionRow.get_Value(mapNumIndex).ToString();
                    mapNumbersToAdd.Add(currentMapNum);
                    changeDetectionRow = rowCursor.NextRow();
                }

                if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
                if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

                //Calculate the differences set using linq for performance sake
                IEnumerable<string> DifferencesSet = mapNumbersToAdd.ToArray().Except(alreadyExistingMapNumbers.ToArray());
                int setCount = DifferencesSet.Count();

                //And finally actually add the rows to the table
                ICursor insertCursor = changeDetectionTable.Insert(true);
                int counter = 0;

                foreach (string mapNumber in DifferencesSet)
                {
                    IRowBuffer newRow = changeDetectionTable.CreateRowBuffer();
                    newRow.set_Value(mapNumIndex, mapNumber);
                    newRow.set_Value(mapTypeIndex, TemplateName);
                    newRow.set_Value(scaleIndex, scale);
                    insertCursor.InsertRow(newRow);
                    
                    if (counter > 100000)
                    {
                        insertCursor.Flush();
                        counter = 0;
                    }
                    else
                    {
                        counter++;
                    }
                }
                insertCursor.Flush();

                if (insertCursor != null) { while (Marshal.ReleaseComObject(insertCursor) > 0) { } }
            }
            catch (Exception e)
            {
                Log("Error inserting rows into the " + Common.Common.TableChangeDetection + " table. Message: " + e.Message + " StackTrace: " + e.StackTrace);
            }
        }

        

        #endregion

    }
}
