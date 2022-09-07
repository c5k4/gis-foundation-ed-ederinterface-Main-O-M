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
    public class MapProductionSpawn
    {
        #region Private Vars

        private IWorkspace workspace = null;
        private Dictionary<string, List<string>> MxdConfiguration = new Dictionary<string, List<string>>();
        private Dictionary<string, string> MxdStoredDisplayConfiguration = new Dictionary<string, string>();
        private Dictionary<string, List<string>> MapNumbersDictionary = new Dictionary<string, List<string>>();
        private Dictionary<string, string> HasDataFeatureClassDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> OutputFormatDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> MxdOutputNames = new Dictionary<string, string>();
        private Dictionary<string, string> MxdNumProcessesConfiguration = new Dictionary<string, string>();
        private List<string> UniqueScales = new List<string>();
        private List<string> ChangedMapGridNumbers = new List<string>();
        private List<IFeatureClass> hasDataFeatureClasses = null;
        private List<Process> runningProcesses = new List<Process>();
        private string sdeConnectionFile = "";
        private StreamWriter writer = null;
        private ITable changeDetectionTable = null;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directConnectString">Direct connect string to geodatabase (i.e. sde:oracle11g:/;local=EDGISA1T)</param>
        public MapProductionSpawn(string sdeConnectionFile)
        {
            Common.Common.ReadAppSettings();

            string path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf("\\"));
            string logFileDirectory = System.IO.Path.Combine(path, Common.Common.LogFileDirectory);
            //DateTime.Today.Year.ToString() + DateTime.Today.Month.ToString() + DateTime.Today.Day.ToString())
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

            writer = new StreamWriter(logFileDirectory + "\\" + "MapProduction_Log.txt", true);


            workspace = Common.Common.OpenWorkspace(sdeConnectionFile);

            if (workspace == null)
            {
                Log("Unable to connect to the specified database.");
                return;
            }

            this.sdeConnectionFile = sdeConnectionFile;
        }

        #endregion

        #region Public Methods

        public void Log(string log)
        {
            Console.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.Flush();
        }

        /// <summary>
        /// This method will spawn all of the processes that will perform the export of the maps
        /// </summary>
        /// <param name="AllMapGrids"></param>
        public void SpawnProcesses(bool AllMapGrids, bool OverWrite)
        {
            if (!ReadConfigurationFile()) { return; }

            //Get our map grid numbers that we will be generating maps for.  If using change detection we don't have to do
            //anything.  But if not we need to add all numbers to the change detection table
            if (AllMapGrids)
            {
                GetMapGridNumbers();

                //Write to the change detection table
                WriteChangeDetectionMapNumbers();
            }

            //Now I should know what Mxds and scales I need to start processes with.
            StartProcess(OverWrite);

            //Monitor our child processes.  With this we can also keep a progress percentage
            MonitorChildProcesses();

            //Finally we need to delete our processed rows from the database
            DeleteProcessedRows();

            Log("Finished Processing");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This process will monitor all child processes and will restart them as necessary
        /// </summary>
        private void MonitorChildProcesses()
        {
            bool continueRunning = true;
            while (continueRunning)
            {
                bool allExited = true;

                foreach (Process p in runningProcesses)
                {
                    if (!p.HasExited)
                    {
                        allExited = false;
                    }
                    else
                    {
                        if (p.ExitCode != (int)Common.Common.ExitCodes.Success)
                        {
                            Environment.ExitCode = (int)Common.Common.ExitCodes.ChildFailure;
                        }
                    }
                }

                if (allExited) { continueRunning = false; }

                string progress = CheckCurrentProgress();

                // George --- Commented console.clear - it fails UC4 job

               // Console.Clear();
                Console.Write(progress + "\n");

                Thread.Sleep(5000);
            }
        }

        private void DeleteProcessedRows()
        {
            Log("Deleting Processed Rows");
            if (changeDetectionTable == null)
            {
                changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
            }

            int mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);

            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = Common.Common.FieldChangeDetectionMapNumber;
            qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " is not null "
                + " AND " + Common.Common.FieldChangeDetectionExportState + " = '" + Common.Common.GetExportState(Common.Common.ExportStates.Processed) + "'";

            changeDetectionTable.DeleteSearchedRows(qf);

            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

            qf = new QueryFilterClass();
            qf.SubFields = Common.Common.FieldChangeDetectionMapNumber;
            qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " is not null "
                + " AND " + Common.Common.FieldChangeDetectionExportState + " = '" + Common.Common.GetExportState(Common.Common.ExportStates.Error) + "'";
            ICursor errorRows = changeDetectionTable.Search(qf, true);
            IRow errorRow = errorRows.NextRow();
            List<string> errorMapNumbers = new List<string>();
            while (errorRow != null)
            {
                string mapNumber = errorRow.get_Value(mapNumIndex).ToString();
                errorMapNumbers.Add(mapNumber);
                errorRow = errorRows.NextRow();
            }

            if (errorRows != null) { while (Marshal.ReleaseComObject(errorRows) > 0) { } }
            if (errorRow != null) { while (Marshal.ReleaseComObject(errorRow) > 0) { } }
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

            qf = new QueryFilterClass();
            qf.SubFields = Common.Common.FieldChangeDetectionMapNumber;
            qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " is null";
            ICursor rowsToDelete = changeDetectionTable.Search(qf, true);
            IRow rowToDelete = rowsToDelete.NextRow();
            List<string> rowsToDeleteList = new List<string>();

            while (rowToDelete != null)
            {
                string mapNumber = rowToDelete.get_Value(mapNumIndex).ToString();
                if (!errorMapNumbers.Contains(mapNumber))
                {
                    rowsToDeleteList.Add(mapNumber);
                }
                rowToDelete = rowsToDelete.NextRow();
            }

            if (rowsToDelete != null) { while (Marshal.ReleaseComObject(rowsToDelete) > 0) { } }
            if (rowToDelete != null) { while (Marshal.ReleaseComObject(rowToDelete) > 0) { } }
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

            if (rowsToDeleteList.Count > 0)
            {
                string[] mapNumberDeleteArray = rowsToDeleteList.ToArray();
                bool keepProcessing = true;
                StringBuilder builder = new StringBuilder();
                int startIndex = 0;

                while (keepProcessing)
                {
                    builder.Remove(0, builder.Length);
                    //The following section is to provide an IN query for finding the maps that we actually care about.  Oracle only
                    //allows up to 1000 entries in the IN query so we need to break up our initial list if it is too large.
                    if ((startIndex + 1000) < mapNumberDeleteArray.Length)
                    {
                        for (int i = startIndex; i < startIndex + 1000; i++)
                        {
                            builder.Append("\'" + mapNumberDeleteArray[i] + "\',");
                        }
                        builder.Remove(builder.Length - 1, 1);
                        startIndex += 1000;
                    }
                    else
                    {
                        for (int i = startIndex; i < mapNumberDeleteArray.Length; i++)
                        {
                            builder.Append("\'" + mapNumberDeleteArray[i] + "\',");
                        }
                        builder.Remove(builder.Length - 1, 1);
                        keepProcessing = false;
                    }

                    qf = new QueryFilterClass();
                    qf.SubFields = Common.Common.FieldChangeDetectionMapNumber;
                    qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " is null" + " AND " + Common.Common.FieldChangeDetectionMapNumber + " in (" + builder + ")";
                    changeDetectionTable.DeleteSearchedRows(qf);
                }
            }
        }

        private string CheckCurrentProgress()
        {
            string progress = "Current Progress:";

            if (changeDetectionTable == null)
            {
                changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
            }

            double TotalFinishedCounter = 0.0;
            double TotalToProcessCounter = 0.0;

            try
            {
                foreach (KeyValuePair<string, List<string>> kvp in MxdConfiguration)
                {
                    string mxdName = kvp.Key;
                    string storedDisplayName = MxdStoredDisplayConfiguration[mxdName];
                    string OutputFileName = MxdOutputNames[mxdName];
                    int numProcesses = Int32.Parse(MxdNumProcessesConfiguration[mxdName]);
                    if (numProcesses < 1) { continue; }

                    List<string> scalesList = kvp.Value;
                    for (int i = 0; i < scalesList.Count; i++)
                    {
                        string scale = scalesList[i];

                        IQueryFilter qf = new QueryFilterClass();
                        qf.SubFields = Common.Common.FieldChangeDetectionExportState;
                        qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " = '" + OutputFileName + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'"
                            + " AND " + Common.Common.FieldChangeDetectionExportState + " is null";
                        double StillToProcessCount = changeDetectionTable.RowCount(qf);

                        qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " = '" + OutputFileName + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'"
                            + " AND " + Common.Common.FieldChangeDetectionExportState + " is not null";
                        double FinishedProcessingCount = changeDetectionTable.RowCount(qf);

                        if ((FinishedProcessingCount + StillToProcessCount) != 0)
                        {
                            progress += "\n" + OutputFileName + "-" + scale + ": " + Math.Round(((FinishedProcessingCount) / (FinishedProcessingCount + StillToProcessCount)) * 100.0, 1) + "% complete";
                        }
                        else
                        {
                            progress += "\n" + OutputFileName + "-" + scale + ": " + "Unkown";
                        }

                        TotalFinishedCounter += FinishedProcessingCount;
                        TotalToProcessCounter += StillToProcessCount;

                        if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    }
                }
            }
            catch (Exception e)
            {
                Log("Failed checking progress.  Will retry shortly");

                //If something fails let's refresh our connection to the database
                if (workspace != null) { while (Marshal.ReleaseComObject(workspace) > 0) { } }
                if (changeDetectionTable != null) { while (Marshal.ReleaseComObject(changeDetectionTable) > 0) { } }

                workspace = Common.Common.OpenWorkspace(sdeConnectionFile);
                changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
            }

            progress += "\n" + "Overall Progress" + ": " + Math.Round(((TotalFinishedCounter) / (TotalFinishedCounter + TotalToProcessCounter)) * 100.0,1) + "% complete";

            return progress;
        }

        /// <summary>
        /// This method will return all map grid numbers separated based on their scale
        /// </summary>
        /// <param name="checkChangedGrids">Determines whether this will only return those grid numbers who have changed</param>
        private void GetMapGridNumbers()
        {
            Log("Determining unified map grids to create...");

            IFeatureClass mapGridFeatClass = Common.Common.getFeatureClass(workspace, Common.Common.TableUnifiedMapGrid);

            if (mapGridFeatClass == null)
            {
                Log("Could not find the " + Common.Common.TableUnifiedMapGrid + " table");
                return;
            }

            int mapNoIndex = mapGridFeatClass.Fields.FindField(Common.Common.FieldMapGridNo);
            int mapScaleIndex = mapGridFeatClass.Fields.FindField(Common.Common.FieldMapGridScale);
            List<string> checkedMapGrids = new List<string>();

            foreach (string scale in UniqueScales)
            {
                List<string> scaleList = null;
                if (MapNumbersDictionary.ContainsKey(scale))
                {
                    scaleList = MapNumbersDictionary[scale];
                }
                else
                {
                    scaleList = new List<string>();
                }

                //This spatial query filter is getting all of the map grid features which intersect the specified 
                //maintenance plat grid and contain the specified scale.  This could be removed (and replaced) 
                //when the plat_unified map grid feature class has the "NoData" field added.  Map grids with no 
                //data will not be exported.
                IQueryFilter qf = new QueryFilterClass();
                string subFields = Common.Common.FieldMapGridNo + "," + Common.Common.FieldMapGridScale;
                string whereClause = Common.Common.FieldMapGridScale + " = " + scale;
                IFeatureCursor featCursor = GetMapGridCursor(mapGridFeatClass, subFields, whereClause);
                IFeature mapGridFeature = featCursor.NextFeature();
                while (mapGridFeature != null)
                {
                    string mapNo = "";
                    try
                    {
                        mapNo = mapGridFeature.get_Value(mapNoIndex).ToString();
                    }
                    catch (Exception e)
                    {
                        //In case the mapNo is null
                    }
                    if (!string.IsNullOrEmpty(mapNo))
                    {
                        scaleList.Add(mapNo);
                    }

                    mapGridFeature = featCursor.NextFeature();
                }

                if (!MapNumbersDictionary.ContainsKey(scale)) { MapNumbersDictionary.Add(scale, scaleList); }

                //Release our cursor
                if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
            }
        }


        /// <summary>
        /// This method will verify if the given feature contains any electric data within the map grid
        /// </summary>
        /// <param name="mapGridFeature">Map grid feature to check</param>
        /// <returns></returns>
        private IFeatureCursor GetMapGridCursor(IFeatureClass mapGridFeatureClass, string subfields, string whereClause)
        {
            if (hasDataFeatureClasses == null)
            {
                IDataset ds = mapGridFeatureClass as IDataset;
                IWorkspace ws = ds.Workspace;
                hasDataFeatureClasses = new List<IFeatureClass>();

                foreach (KeyValuePair<string, string> kvp in HasDataFeatureClassDictionary)
                {
                    string[] featClasses = Regex.Split(kvp.Value, ",");

                    for (int i = 0; i < featClasses.Length; i++)
                    {
                        IFeatureClass featClass = Common.Common.getFeatureClass(ws, featClasses[i]);
                        if (featClass != null && !hasDataFeatureClasses.Contains(featClass))
                        {
                            hasDataFeatureClasses.Add(featClass);
                        }
                    }
                }
            }

            IEnvelope envelope = null;

            //Determine overall envelope of all the featureclasses
            foreach (IFeatureClass featClass in hasDataFeatureClasses)
            {
                
                IGeoDataset geoDataset = featClass as IGeoDataset;
                if (envelope == null) { envelope = geoDataset.Extent; }
                else
                {
                    envelope.Union(geoDataset.Extent);
                }
            }

            ISpatialFilter qf = new SpatialFilterClass();
            qf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            qf.Geometry = envelope;
            qf.GeometryField = mapGridFeatureClass.ShapeFieldName;
            qf.SubFields = subfields;
            qf.WhereClause = whereClause;
            return mapGridFeatureClass.Search(qf, true);
        }

        

        /// <summary>
        /// This method will return only those map grid numbers which exist in the change detection table for the plat_unified table
        /// </summary>
        private void GetChangeDetectionMapGridNumbers()
        {
            
            try
            {
                if (changeDetectionTable == null)
                {
                    changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
                }
            }
            catch (Exception e)
            {
                Log("Unable to find the change detection table: " + Common.Common.TableChangeDetection);
                return;
            }

            //Get our field indices
            int unifiedGridNumberIndex = changeDetectionTable.Fields.FindField(Common.Common.FieldChangeDetectionMapNumber);

            //Let's search our change detection table
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = Common.Common.FieldChangeDetectionMapNumber;// +"," + Common.Common.FieldHasError;
            qf.WhereClause = Common.Common.FieldChangeDetectionMapNumber + " is not null";

            ICursor rowCursor = changeDetectionTable.Search(qf, true);
            IRow row = rowCursor.NextRow();
            while (row != null)
            {
                string unifiedGridNumber = row.get_Value(unifiedGridNumberIndex).ToString();
                if (!string.IsNullOrEmpty(unifiedGridNumber))
                {
                    ChangedMapGridNumbers.Add(unifiedGridNumber);
                }
                
                row = rowCursor.NextRow();
            }

            //Delete all of the rows
            changeDetectionTable.DeleteSearchedRows(qf);

            //Release our cursor
            if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }

        }

        /// <summary>
        /// This method will fire all of the processes that will perform that actual export of the maps.
        /// </summary>
        private void StartProcess(bool overwrite)
        {
            Log("Starting processes to create maps...");
            string executingLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
            executingLocation = executingLocation.Substring(0, executingLocation.LastIndexOf("\\"));

            foreach (KeyValuePair<string, List<string>> kvp in MxdConfiguration)
            {
                string mxdName = kvp.Key;
                string storedDisplayName = MxdStoredDisplayConfiguration[mxdName];
                string OutputFileName = MxdOutputNames[mxdName];
                string outputFormat = OutputFormatDictionary[mxdName];
                string numProcesses = MxdNumProcessesConfiguration[mxdName];

                List<string> scalesList = kvp.Value;
                for (int i = 0; i < scalesList.Count; i++)
                {
                    string scale = scalesList[i];
                    if (File.Exists(executingLocation + "\\" + Common.Common.MxdFileDirectory + "\\" + mxdName))
                    {
                        string arguments =  
                            "-c \"" + sdeConnectionFile + "\" " + 
                            "-M \"" + executingLocation + "\\" + 
                            Common.Common.MxdFileDirectory + "\\" + mxdName + 
                            "\" -E \"" + Common.Common.ExportFileDirectory + "\\" + OutputFileName + 
                            "\" -T \"" + OutputFileName +
                            "\" -TL \"" + Common.Common.TempFileDirectory + "\\" + OutputFileName + 
                            "\" -S " + scale + 
                            " -sd \"" + storedDisplayName + "\"" + 
                            " -f " + outputFormat + 
                            " -p " + 
                            " -n " + numProcesses;
                        if (overwrite) { arguments += " -O"; }
                        if (!String.IsNullOrEmpty(HasDataFeatureClassDictionary[mxdName])) { arguments += " -D " + HasDataFeatureClassDictionary[mxdName]; }
                        Log("arguments: " + arguments);

                        //Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments);
                        ProcessStartInfo startInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().Location, arguments)
                        {
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            UseShellExecute = false,
                            RedirectStandardOutput = true
                        };
                        Process p = Process.Start(startInfo);
                        runningProcesses.Add(p);
                        Log("Process started with ID: " + p.Id);
                    }
                    else
                    {
                        Log("Unable to find the specified Mxd: " + executingLocation + "\\" + Common.Common.MxdFileDirectory + "\\" + mxdName);
                    }
                }
            }
        }

        private List<string> ExistingMapNumbers(string scale)
        {
            ITable changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);

            int mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
            int mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);
            int scaleIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionScale);

            string currentMapNum = "";
            List<string> alreadyExistingMapNumbers = new List<string>();

            //First determine any rows that already exist for this map type.
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = Common.Common.FieldChangeDetectionMapNumber + "," + Common.Common.FieldChangeDetectionMapType;
            qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " is null AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'";
            ICursor rowCursor = changeDetectionTable.Search(qf, true);
            IRow changeDetectionRow = rowCursor.NextRow();

            while (changeDetectionRow != null)
            {
                currentMapNum = changeDetectionRow.get_Value(mapNumIndex).ToString();
                if (!String.IsNullOrEmpty(currentMapNum))
                {
                    alreadyExistingMapNumbers.Add(currentMapNum);
                }
                changeDetectionRow = rowCursor.NextRow();
            }

            if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
            if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

            return alreadyExistingMapNumbers;
        }

        /// <summary>
        /// This process will split the map numbers that need to be generated into many different input files based on
        /// the number of processes to spawn
        /// </summary>
        private void WriteChangeDetectionMapNumbers()
        {
            Log("Writing map numbers to " + Common.Common.TableChangeDetection + "...");

            ITable changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);

            foreach (KeyValuePair<string, List<string>> kvp in MapNumbersDictionary)
            {
                List<string> MapNumbers = kvp.Value;
                string scale = kvp.Key;
                MapNumbers.Sort();

                List<string> existingMapNumbers = ExistingMapNumbers(scale);

                //Calculate the differences set using linq for performance sake
                IEnumerable<string> DifferencesSet = MapNumbers.ToArray().Except(existingMapNumbers.ToArray());
                int setCount = DifferencesSet.Count();

                int mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
                int scaleIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionScale);

                //And finally actually add the rows to the table
                ICursor insertCursor = changeDetectionTable.Insert(true);
                int counter = 0;
                IRowBuffer newRow = changeDetectionTable.CreateRowBuffer();

                foreach (string mapNumber in DifferencesSet)
                {
                    newRow.set_Value(mapNumIndex, mapNumber);
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

        }

        /// <summary>
        /// This method will open the application configuration file and read in any configuration information necessary
        /// for the process to execute
        /// </summary>
        private bool ReadConfigurationFile()
        {
            Log("Reading configuration file...");
            try
            {
                //Open our configuration file
                string assemLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(assemLocation + ".config");

                //Determine how many processes will be spawned per mxd, scale combination
                XmlNodeList MxdsNodeList = xmlDoc.SelectNodes("//configuration/Mxds");

                //Grab our Mxd configuration section to see what Mxds we will be using and also
                //what scales are required for those Mxds
                XmlNodeList nodeList = xmlDoc.SelectNodes("//configuration/Mxds/Mxd");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    List<string> mxdList = null;
                    XmlNode node = nodeList[i];

                    //Grab the name of the Mxd
                    string mxdName = node.Attributes["MxdName"].Value;
                    string outputName = node.Attributes["OutputName"].Value;
                    string storedDisplayName = node.Attributes["StoredDisplayName"].Value;
                    string outputFormat = node.Attributes["OutputFormat"].Value;
                    string numProcessors = node.Attributes["NumProcessors"].Value;

                    //Grab the has data feature classes
                    string hasData = node.Attributes["HasDataFeatureClasses"].Value;

                    if (!HasDataFeatureClassDictionary.ContainsKey(mxdName))
                    {
                        HasDataFeatureClassDictionary.Add(mxdName, hasData);
                    }

                    //Grab the num processors configuration
                    if (!MxdNumProcessesConfiguration.ContainsKey(mxdName))
                    {
                        MxdNumProcessesConfiguration.Add(mxdName, numProcessors);
                    }

                    //Add the stored display to the list
                    if (!MxdStoredDisplayConfiguration.ContainsKey(mxdName))
                    {
                        MxdStoredDisplayConfiguration.Add(mxdName, storedDisplayName);
                    }

                    //Add the output format to its list
                    if (!OutputFormatDictionary.ContainsKey(mxdName))
                    {
                        OutputFormatDictionary.Add(mxdName, outputFormat);
                    }

                    //Add this Mxd output name pairing
                    if (!MxdOutputNames.ContainsKey(mxdName))
                    {
                        MxdOutputNames.Add(mxdName, outputName);
                    }

                    //Check this in case the configuration file has duplicates for some reason.
                    if (!MxdConfiguration.ContainsKey(mxdName))
                    {
                        mxdList = new List<string>();
                    }
                    else { continue; }

                    //Grab the scale of the mxd.  This may have multiple comma separated values so we need to split it up
                    string scale = node.Attributes["Scale"].Value;
                    string[] scales = Regex.Split(scale, ",");
                    for (int j = 0; j < scales.Length; j++)
                    {
                        //Add each scale to our list
                        if (!mxdList.Contains(scales[j]))
                        {
                            mxdList.Add(scales[j]);

                            if (!UniqueScales.Contains(scales[j]))
                            {
                                UniqueScales.Add(scales[j]);
                            }
                        }
                    }

                    MxdConfiguration.Add(mxdName, mxdList);
                }
            }
            catch (Exception e)
            {
                Log("Invalid configuration.  Please check the configuration file\n" + e.Message);
                return false;
            }
            return true;
        }

        #endregion

    }
}
