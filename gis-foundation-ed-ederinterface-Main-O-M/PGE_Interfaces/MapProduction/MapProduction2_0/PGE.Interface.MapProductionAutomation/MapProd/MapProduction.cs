using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using System.Text.RegularExpressions;
using PGE.Common.Delivery.Framework;
using System.Diagnostics;
using System.Xml;
using System.Threading;

namespace PGE.Interfaces.MapProductionAutomation.MapProd
{
    public class MapProduction
    {
        #region Private Vars

        /* GDI delegate to SystemParametersInfo function */
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        /* constants used for user32 calls */
        private const uint SPI_GETFONTSMOOTHING = 74;
        private const uint SPI_SETFONTSMOOTHING = 75;
        private const uint SPIF_UPDATEINIFILE = 0x1;

     
        private int regionFieldIndex = -1;
        private int districtFieldIndex = -1;
        private int divisionFieldIndex = -1;
        private IFeatureClass ServiceAreaFeatClass = null;
         
        private List<IFeatureClass> hasDataFeatureClasses = null;

        private int processId = -1;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for MapProduction
        /// </summary>
        /// <param name="directConnectString">Direct connect string to geodatabase (i.e. sde:oracle11g:/;local=EDGISA1T)</param>
        /// <param name="mxdLocation">Location of the mxd to use</param>
        /// <param name="mapNoInputLocation">Map number input file location</param>
        /// <param name="expLocation">Export file location</param>
        /// <param name="TemplateName">Template name (i.e. CircuitMap)</param>
        /// <param name="scale">Scale that will be exported at</param>
        /// <param name="hasDataLayers">Comma separated string to specify the feature classes to be considered when checking for data</param>
        //public MapProduction(string sdeConnectionFile, string mxdLocation, string expLocation, string tempLocation, 
        //    string TemplateName, string scale, bool overwrite, string hasDataLayers, string StoredDisplayName,
        //    string format, string logFileName)
        //{ 
        //    //Store our arguments in case we need to restart this process
        //    this.sdeConnectionFile = sdeConnectionFile;
        //    this.mxdLocation = mxdLocation;
        //    this.expLocation = expLocation;
        //    this.tpLocation = tempLocation; 
        //    this.TemplateName = TemplateName;
        //    this.scale = scale;
        //    this.overwrite = overwrite;
        //    this.hasDataLayers = hasDataLayers;
        //    this.StoredDisplayName = StoredDisplayName;
        //    this.format = form
        //    this.logFileName = logFileName;

        //    Common.Common.ReadAppSettings();
        //    Format = format;

        //    CreateLogFile();
        //}

        public MapProduction(int processId, string configurationFile)
        {
            Common.Common.ReadAppSettings(configurationFile);
            this.processId = processId;
            //this.MapDivision = mapDivision; 
            //this.logFileName = processId.ToString() + "_Log" + ".txt";
            //CreateLogFile();
        }


        //~MapProduction()
        //{
        //    try
        //    {
        //        if (writer != null)
        //        {
        //            writer.Close();
        //        }
        //    }
        //    catch { }
        //}

        #endregion
        List<Process> m_pList_ChildProcessesRunning = new List<Process>();
        #region Public Methods
        IList<Thread> _threads = new List<Thread>();

       

        /// <summary>
        /// This method handles the printing of the PDFs
        /// </summary>
        public void ExportMaps(int processId, string configurationFile)
        {
#if DEBUG
            //Debugger.Launch();
#endif
            #region Intialize Varaibles
            MapProductionHelper pMapProdHelper = new MapProductionHelper(
               Common.Common.MaxTries, processId, true);
            MapProductionHelper.Log("Entering ExportMaps for ProcessID: " + Process.GetCurrentProcess().Id, MapProductionHelper.LogType.Debug, null);
            bool overWrite = Convert.ToBoolean(Common.Common.OverWrite);
           

            string exportLocation = Common.Common.ExportFileDirectory;
            string tempLocation = Common.Common.TempFileDirectory;
            string mxdDirectory = Common.Common.MxdFolder;
            string machineName = System.Environment.MachineName;

            MapProductionHelper.Log("exportLocation: " + exportLocation, MapProductionHelper.LogType.Info, null);
            MapProductionHelper.Log("tempLocation: " + tempLocation, MapProductionHelper.LogType.Info, null);
            MapProductionHelper.Log("mxdDirectory: " + mxdDirectory, MapProductionHelper.LogType.Info, null);
            MapProductionHelper.Log("machineName: " + machineName, MapProductionHelper.LogType.Info, null);

            //Create a list of strings for errors 
           
            string curMapNumber = "";
            string Map_Type = "";
            string Map_Mxd = "";
            string mapScale = "";
            string mxdFile = "";
            int failCount = -1;
             MapProductionHelper.Log("connection opened", MapProductionHelper.LogType.Debug, null);

            #endregion
            //Get the necessary stuff from the configuration settings

           
            MapProductionHelper.Log("Entering ExportMaps for ProcessID: " + Process.GetCurrentProcess().Id, MapProductionHelper.LogType.Debug, null);
          
           

           
            MapProductionHelper.Log("machineName: " + machineName, MapProductionHelper.LogType.Info, null);
            MapProductionHelper.Log("connection opened", MapProductionHelper.LogType.Debug, null);
            IDictionary<string, MapDocument> MXDList = new Dictionary<string, MapDocument>();

            #region Process Map
            try
            {

                Dictionary<string, MapDocument> MapDocumentList = new Dictionary<string, MapDocument>();
                while (pMapProdHelper.GetNextMapToProcess(configurationFile, processId, machineName, ref Map_Type, ref mapScale, ref Map_Mxd, ref failCount, ref curMapNumber))
                {
                    //GetMapDocument
                    MapDocument mapDoc = new MapDocument();
                    mxdFile = Common.Common.MxdFolder + "\\" + Map_Mxd;
                    if (!File.Exists(mxdFile))
                        throw new Exception("Map Document file not found!");

                    #region Open Map Document
                    MapDocumentList.TryGetValue(Map_Type, out mapDoc);
                    if (mapDoc == null)
                    {
                        mapDoc = new MapDocument();
                        mapDoc.Open(mxdFile);
                        if (mapDoc != null)
                        {
                            MapDocumentList.Add(Map_Type, mapDoc);
                        }
                        else continue;
                    }
                    #endregion
                    MapProductionHelper.Log("Export Map -> Current_MAP_NUMBER:MAP_TYPE:MAP_SCALE " + curMapNumber + "_" + Map_Type + "_" + mapScale, MapProductionHelper.LogType.Info, null);

                    MapProductionHelper.Log("Initializing Mxd", MapProductionHelper.LogType.Info, null);
                    InitActiveViews(mapDoc);
                    //MapProductionHelper.Log("Turning off transparency", MapProductionHelper.LogType.Debug, null);
                    TurnOffTransparency(mapDoc);
                    //MapProductionHelper.Log("Removing any instances of maintenance plat layers", MapProductionHelper.LogType.Debug, null);
                    RemoveMainPlatLayers(mapDoc);

                    NextMaptoProcess(mapDoc, configurationFile, processId, machineName, Map_Type, mapScale, Map_Mxd, failCount, curMapNumber);


                }
                #endregion

                MapProductionHelper.Log("No more maps found to process", MapProductionHelper.LogType.Info, null);
                MapProductionHelper.Log("Export completed for process: " + processId.ToString(), MapProductionHelper.LogType.Info, null);

                //Close down database resources
                EndProcess();
                //closeAllmaps
                foreach (object skey in MapDocumentList.Keys)
                {
                    MapDocument Mapdocs = null;
                    MapDocumentList.TryGetValue(skey.ToString(), out Mapdocs);
                    if (Mapdocs != null)
                    {
                        Mapdocs.Close();
                    }


                }
            }
            catch (Exception ex)
            { }
        }


        private void EndProcess()
        {
            try
            {
                //Wait untill Max Process limit reached
                while (m_pList_ChildProcessesRunning.Count >0)
                {
                    Thread.Sleep(Common.Common.SleepTime);
                    for (int ii = 0; ii < m_pList_ChildProcessesRunning.Count; ii++)
                    {
                        int min = System.DateTime.Now.Minute - m_pList_ChildProcessesRunning[ii].StartTime.Minute;
                        if (min > 30)
                        {
                            try
                            {
                                m_pList_ChildProcessesRunning[ii].Kill();
                               break;
                            }
                            catch { }
                        }
                    }

                }
            }
            catch { }
        }
        
        /// <summary>
        /// Next Map TO process
        /// </summary>
        /// <param name="configurationFile"></param>
        /// <param name="processId"></param>
        /// <param name="machineName"></param>
        /// <param name="map_Type"></param>
        /// <param name="mapScale"></param>
        /// <param name="map_Mxd"></param>
        /// <param name="failCount"></param>
        /// <param name="curMapNumber"></param>
        public void NextMaptoProcess(MapDocument mapDoc, string configurationFile, int processId, string machineName, string map_Type, string mapScale, string map_Mxd,  int failCount, string curMapNumber)
        {
            string reqDataLayers = "";
            string outputFormat = "";
            string outputName = "";
            double widthDouble = -1; double heightDouble = -1;
            MapProductionHelper pMapProdHelper = null;
            string errorMsg = "";
            try
            {
               // Common.Common.ReadAppSettings(configurationFile);
                
                string exportLocation = Common.Common.ExportFileDirectory;
                string tempLocation = Common.Common.TempFileDirectory;
               
                MapProductionHelper.Log("exportLocation: " + exportLocation, MapProductionHelper.LogType.Info, null);
                MapProductionHelper.Log("tempLocation: " + tempLocation, MapProductionHelper.LogType.Info, null);
                
                pMapProdHelper = new MapProductionHelper(Common.Common.MaxTries, processId, true);
                MapProductionHelper.Log("Entering ExportMaps for ProcessID: " + Process.GetCurrentProcess().Id, MapProductionHelper.LogType.Debug, null);
                //Get the current mxd and check if the mxd is changed - if so reload mapdocument 
                

                    //Query for page size
                    mapDoc.PageLayout.Page.QuerySize(out widthDouble, out heightDouble);
                   int height = Int32.Parse(Math.Round(heightDouble).ToString());
                   int width = Int32.Parse(Math.Round(widthDouble).ToString());

                    MapProductionHelper.Log("Searching for " + Common.Common.TableUnifiedMapGrid + " layer", MapProductionHelper.LogType.Debug, null);

                // to handle TIFF 
                IFeatureLayer pFL = null;
                try { pFL = Common.Common.getFeatureLayerFromMap(mapDoc.get_Map(1), Common.Common.TableUnifiedMapGrid); } catch { }
                if (pFL == null) { try { pFL = Common.Common.getFeatureLayerFromMap(mapDoc.get_Map(0), Common.Common.TableUnifiedMapGrid); } catch { } }

                IFeatureClass pMapGridFC = pFL.FeatureClass;
                   IWorkspace pWS = ((IDataset)pMapGridFC).Workspace;
                    
                MapProductionHelper.Log("Layer " + Common.Common.TableUnifiedMapGrid + " found", MapProductionHelper.LogType.Debug, null);
                bool isColor = false;
                
                Common.Common.PopulateMXDAttributesFromConfigFile(configurationFile,
                    map_Mxd,
                    mapScale,
                    ref reqDataLayers,
                    ref outputFormat,
                    ref outputName,
                    ref isColor);
                       

                    MapProductionHelper.Log("Map Mxd: " + map_Mxd, MapProductionHelper.LogType.Info, null);
                    MapProductionHelper.Log("Map Type: " + map_Type, MapProductionHelper.LogType.Info, null);
                    MapProductionHelper.Log("Map Scale: " + mapScale, MapProductionHelper.LogType.Info, null);
                

                MapProductionHelper.Log("Map Number: " + curMapNumber + " Failure Count: " + failCount.ToString(), MapProductionHelper.LogType.Info, null);

                //Get the map grid feature 
                if ((pMapGridFC == null) || (pWS == null))
                {
                    MapProductionHelper.Log("Unable to find the " + Common.Common.TableUnifiedMapGrid + " featureclass", MapProductionHelper.LogType.Error, null);
                    throw new Exception("Unable to find the " + Common.Common.TableUnifiedMapGrid + " featureclass");
                }
                IFeature pMapGridFeature = pMapProdHelper.GetMapGridFeature(pMapGridFC, curMapNumber, mapScale);

                //Check if this map grid actually has data
                if (!HasData(pMapGridFeature, reqDataLayers, pMapProdHelper))
                {
                    MapProductionHelper.Log("HasData returned FALSE no data for map: " + curMapNumber + " was found in the following layers: " + reqDataLayers, MapProductionHelper.LogType.Debug, null);
                    pMapProdHelper.UpdateMapStatus(processId, -1, Common.Common.ExportStates.RequiredDataMissing,
                        curMapNumber, mapScale, map_Type, machineName, 0, "Map grid does not contain all the required data");

                    //Go to the next map and continue 
                    //MapProductionHelper.Log("Moving to the next map", MapProductionHelper.LogType.Info, null);
                    
                }

                MapProductionHelper.Log("Processing map number: " + curMapNumber, MapProductionHelper.LogType.Debug, null);
                MapProductionHelper.Log("Setting up page template", MapProductionHelper.LogType.Debug, null);
                MapProductionHelper.Log("HasData returned TRUE for map: " + curMapNumber, MapProductionHelper.LogType.Debug, null);
                //Needed for the PGE custom auto text elements
                MapProductionFacade.PGEMap = mapDoc.ActiveView.FocusMap;
                MapProductionFacade.PGEPageLayout = mapDoc.PageLayout;

                #region Set all of the autotext elements Map grid number for the custom auto text elements
                IElement ateElement = MapProductionFacade.GetArcFMAutoTextElement(mapDoc.PageLayout, MapProductionFacade.PGEHashBufferATECaption);
                if (ateElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(ateElement, curMapNumber);
                }

                IElement xminATEElement = MapProductionFacade.GetArcFMAutoTextElement(mapDoc.PageLayout, MapProductionFacade.PGEXMinCaption);
                if (xminATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(xminATEElement, curMapNumber);
                }

                IElement yminATEElement = MapProductionFacade.GetArcFMAutoTextElement(mapDoc.PageLayout, MapProductionFacade.PGEYMinCaption);
                if (yminATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(yminATEElement, curMapNumber);
                }

                IElement xmaxATEElement = MapProductionFacade.GetArcFMAutoTextElement(mapDoc.PageLayout, MapProductionFacade.PGEXMaxCaption);
                if (xmaxATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(xmaxATEElement, curMapNumber);
                }

                IElement ymaxATEElement = MapProductionFacade.GetArcFMAutoTextElement(mapDoc.PageLayout, MapProductionFacade.PGEYMaxCaption);
                if (ymaxATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(ymaxATEElement, curMapNumber);
                }

                IElement ateElementCounty = MapProductionFacade.GetArcFMAutoTextElement(mapDoc.PageLayout, MapProductionFacade.PGECountyCaption);
                if (ateElementCounty != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementCounty, curMapNumber);
                }

                IElement ateElementDivision = MapProductionFacade.GetArcFMAutoTextElement(mapDoc.PageLayout, MapProductionFacade.PGEDivisionCaption);
                if (ateElementDivision != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(ateElementDivision, curMapNumber);
                }
                #endregion

                MapProductionFacade.CurrentMapGridFeature = pMapGridFeature;
                MapProductionFacade.CurrentMapGridPrimaryKey = curMapNumber;


                //Zoom and pan to feature
                IEnvelope featEnvelope = pMapGridFeature.Shape.Envelope;
                mapDoc.ActiveView.Extent = featEnvelope;
                mapDoc.ActiveView.Refresh();

                #region Set our scale 
                MapProductionHelper.Log("Setting scale", MapProductionHelper.LogType.Debug, null);
                string OID = pMapGridFeature.OID.ToString();
                for (int i = 0; i < mapDoc.MapCount; i++)
                {
                    if (mapDoc.get_Map(i).Name == "MainDataFrame")
                    {
                        IActiveView activeView = mapDoc.get_Map(i) as IActiveView;
                        activeView.Extent = featEnvelope;
                        // George - code change - increased map scale for tiff to accomodate geo graphic tif

                        // George - code change - increased map scale for geographic tiff
                        if (outputFormat.ToUpper() == "TIF")
                        {
                            MapProductionHelper.Log("Setting scale for Tif", MapProductionHelper.LogType.Debug, null);
                            mapDoc.get_Map(i).MapScale = Double.Parse(mapScale ) * 16;
                        }
                        else
                        {
                            MapProductionHelper.Log("Setting scale for pdf", MapProductionHelper.LogType.Debug, null);
                            mapDoc.get_Map(i).MapScale = Double.Parse(mapScale) * 12;

                            if (Common.Common.EnableMapCache)
                            {
                                IMapCache mapCache = activeView as IMapCache;
                                mapCache.EmptyCache();
                                if (!mapCache.AutoCacheActive)
                                    mapCache.AutoCacheActive = true;
                            }
                        }
                        activeView.Activate(0);
                    }
                }
                #endregion

                #region find Location
                MapProductionHelper.Log("Finding location information", MapProductionHelper.LogType.Debug, null);
                List<string> mapGridLocationInfo = GetLocationInformation(
                    pMapGridFeature, pWS, pMapProdHelper, mapDoc);

                //If no location information is found we will log an error
                if (mapGridLocationInfo.Count < 1)
                {

                    MapProductionHelper.Log("No Location Info found for map: " + curMapNumber, MapProductionHelper.LogType.Info, null);
                    pMapProdHelper.UpdateMapStatus(processId, -1, Common.Common.ExportStates.LocationInformationMissing,
                        curMapNumber, mapScale, map_Type , machineName, 0, "Map grid does not contain necessary location information");
                    //Go to the next map and continue 
                    MapProductionHelper.Log("Moving to the next map", MapProductionHelper.LogType.Debug, null);
                    
                }
                if (mapGridLocationInfo.Count > 1)
                    Debug.Print("Multiple Location Info found (it crosses regional boundaries) for map: " + curMapNumber);
                #endregion
                           
                GeneratePDF(mapGridLocationInfo, outputName, curMapNumber, width, height, mapScale, outputFormat, exportLocation, tempLocation, pMapGridFeature, isColor, pMapProdHelper, ref failCount, map_Type, machineName, mapDoc);


                //Check Process memory to see if we need to restart the process
                if (RestartDueToMemory(pMapProdHelper) || (errorMsg != string.Empty))
                {
                    MapProductionHelper.Log("Memory threshold exceeded or error encountered - killing process", MapProductionHelper.LogType.Info, null);
                    if (mapDoc != null)
                        mapDoc.Close();
                    MapProductionHelper.Log("Killing process - errorMsg: " + errorMsg, MapProductionHelper.LogType.Info, null);
                    pMapProdHelper.Disconnect();
                    Process.GetCurrentProcess().Kill();
                }
                else
                {
                    if (mapDoc != null)
                        mapDoc.Close();
                }
            }
            catch (Exception ex)
            {
                pMapProdHelper.UpdateMapStatus(processId, -1, Common.Common.ExportStates.Error,
                       curMapNumber, mapScale, map_Type, machineName, 0, "Exception Occurred");
                MapProductionHelper.Log("Error processing maps: " + ex.Message, MapProductionHelper.LogType.Error, ex);
                throw ex;
            }
            finally
            {
                ////Kill all the threads
                //foreach (Thread thread in _threads)
                //{
                //    //if (thread.Name == id)
                //    thread.Abort();
                //}
            }

        }

        /// <summary>
        /// GENERATE PDF
        /// </summary>
        /// <param name="mapGridLocationInfo"></param>
        /// <param name="outputName"></param>
        /// <param name="curMapNumber"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="curMapScale"></param>
        /// <param name="outputFormat"></param>
        /// <param name="exportLocation"></param>
        /// <param name="tempLocation"></param>
        /// <param name="pMapGridFeature"></param>
        /// <param name="isColor"></param>
        /// <param name="pMapProdHelper"></param>
        /// <param name="failCount"></param>
        /// <param name="curMap_Type"></param>
        /// <param name="machineName"></param>
        private void GeneratePDF(List<string> mapGridLocationInfo,string outputName,string curMapNumber, int width,
            int height,string curMapScale, string outputFormat,string exportLocation,string tempLocation,IFeature pMapGridFeature,
            bool isColor,MapProductionHelper pMapProdHelper,ref int failCount,string curMap_Type, string machineName,MapDocument mapDoc)
        {
            string errorMsg = string.Empty;
            int mapsActuallyCreated = 0;

            try
            {
                MapProductionHelper.Log("Export Started", MapProductionHelper.LogType.Info, null);
                bool overWrite = Convert.ToBoolean(Common.Common.OverWrite);
                bool mapHasError = false;
                string successfulTempLocation = "";
                foreach (string locationInfo in mapGridLocationInfo)
                {
                    string[] location = Regex.Split(locationInfo, ",");
                    string mapFilename = outputName + "_" + curMapNumber + "_" + width + "x" + height + "_" + curMapScale;
                    int resolution = Common.Common.Resolution;

                    if (outputFormat.ToUpper() == "TIF")
                    {
                        mapFilename = curMapNumber;
                        resolution = 200;
                    }

                    //Append the map number to the export location
                    string newExportLocation = exportLocation + "\\" + outputName + "-" + curMapScale + "\\" + location[0] + "\\" + location[1] + "\\" + location[2] + "\\";
                    string newTempLocation = tempLocation + "\\" + outputName + "-" + curMapScale + "\\" + location[0] + "\\" + location[1] + "\\" + location[2] + "\\";

                    //Check if directory exists
                    if (!Directory.Exists(newExportLocation))
                        Directory.CreateDirectory(newExportLocation);
                    if (!Directory.Exists(newTempLocation))
                        Directory.CreateDirectory(newTempLocation);

                    bool fileExists = File.Exists(newExportLocation + mapFilename + "." + outputFormat.ToLower());
                    bool tempFileExists = File.Exists(newTempLocation + mapFilename + "." + outputFormat.ToLower());

                    //generate a temporary file to let other processes know that this map number is already being generated.
                    MapProductionHelper.Log("fileExists: " + fileExists.ToString(), MapProductionHelper.LogType.Debug, null);
                    MapProductionHelper.Log("tempFileExists: " + tempFileExists.ToString(), MapProductionHelper.LogType.Debug, null);
                    MapProductionHelper.Log("OverWrite: " + overWrite.ToString(), MapProductionHelper.LogType.Debug, null);
                    MapProductionHelper.Log("Beginning file export", MapProductionHelper.LogType.Info, null);
                    //run the export                         
                    try
                    {
                        if (successfulTempLocation == "")
                        {
                            deleteMap(newTempLocation + mapFilename, outputFormat);
                            if (!ExportActiveViewParameterized(resolution, 1, outputFormat.ToUpper(),
                                  newTempLocation, false, mapFilename, isColor, pMapProdHelper, mapDoc))
                            {
                                //Add to errors list and restart before going to next map 
                                errorMsg = "Error performing ExportActiveViewParameterized";
                                mapHasError = true;
                            }
                            successfulTempLocation = newTempLocation + mapFilename;
                        }
                    }
                    catch
                    {
                        //Add to errors list and restart before going to next map 
                         errorMsg = "Error performing ExportActiveViewParameterized";
                        mapHasError = true;
                    }

                    MapProductionHelper.Log("Export completed", MapProductionHelper.LogType.Info, null);
                    if (fileExists && !overWrite)
                    {
                        MapProductionHelper.Log(outputFormat.ToUpper() + " already exists. Skipping this map", MapProductionHelper.LogType.Info, null);
                    }
                    else
                    {
                        //Copy the file to the new location 
                        MapProductionHelper.Log("Copying file to the final location", MapProductionHelper.LogType.Info, null);
                        if (!copyPDF(
                            successfulTempLocation,
                            newExportLocation + mapFilename,
                            outputFormat,
                            ref errorMsg,
                            pMapProdHelper))
                        {
                            mapHasError = true;
                        }
                    }

                    if (outputFormat.ToUpper() == "TIF")
                    {
                        MapProductionHelper.Log("Writing tiff information", MapProductionHelper.LogType.Info, null);
                        IEnvelope mapExtent = ((IActiveView)mapDoc.get_Map(0)).Extent;
                        IEnvelope pageTemplateExtent = mapDoc.ActiveView.Extent;
                        IEnvelope fullPageTemplateExtent = mapDoc.ActiveView.FullExtent;
                        IEnvelope mapFullExtent = ((IActiveView)mapDoc.get_Map(0)).FullExtent;
                        pMapProdHelper.InsertTIFFInformation(
                            ((IActiveView)mapDoc.get_Map(0)).Extent,
                            pMapGridFeature,
                            curMapNumber,
                            location[1],
                            location[2]);
                    }

                    //increment the count of maps created 
                    mapsActuallyCreated++;
                }

                //Update status to sucess / Error 
                if (!mapHasError)
                {
                    //Update the status to success for the map 
                    MapProductionHelper.Log("Updating status to success: " + Common.Common.ExportStates.Processed.ToString(), MapProductionHelper.LogType.Info, null);
                    pMapProdHelper.UpdateMapStatus(processId, -1, Common.Common.ExportStates.Processed,
                        curMapNumber, curMapScale, curMap_Type, machineName, 0, "");
                }
                else
                {
                    //increment the failure count 
                    failCount++;
                    MapProductionHelper.Log("Updating status to failure: " + Common.Common.ExportStates.Error, MapProductionHelper.LogType.Info, null);
                    pMapProdHelper.UpdateMapStatus(processId, -1, Common.Common.ExportStates.Error,
                        curMapNumber, curMapScale, curMap_Type, machineName, failCount, errorMsg);
                }
            }
            catch(Exception ex) {  }
        }

     

        /// <summary>
        /// Intialized MXdList
        /// </summary>
        /// <param name="configurationFile"></param>
        /// <returns></returns>
        private IDictionary<string, MapDocument> IntializeMXDs(string configurationFile)
        {
            IDictionary<string, MapDocument> MxdsList = new Dictionary<string, MapDocument>();
            try
            {
                #region InitializeMXD
                //Initialize our active views, turn off transparency and remove 
                //maintenanceplat layers (because of problem with ATEs)
                string mxdDirectory = Common.Common.MxdFolder;

                MapProductionHelper.Log("Map Type or Scale changed", MapProductionHelper.LogType.Debug, null);
                MapProductionHelper.Log("mxdDirectory: " + mxdDirectory, MapProductionHelper.LogType.Info, null);



                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(configurationFile);

                //Determine how many processes will be spawned per mxd, scale combination
                XmlNodeList MxdsNodeList = xmlDoc.SelectNodes("//configuration/Mxds");

                //Grab our Mxd configuration section to see what Mxds we will be using and also
                //what scales are required for those Mxds
                XmlNodeList nodeList = xmlDoc.SelectNodes("//configuration/Mxds/Mxd");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlNode node = nodeList[i];
                    MapDocument mapDoc = new MapDocument();
                    //Grab the name of the Mxd
                    string curmxdName = node.Attributes["MxdName"].Value.ToLower();
                    string mxdFile = mxdDirectory + "\\" + curmxdName;
                    if (!File.Exists(mxdFile))
                        throw new Exception("Map Document file not found!");
                    mapDoc.Open(mxdFile);
                    MapProductionHelper.Log("Initializing Mxd", MapProductionHelper.LogType.Info, null);
                    InitActiveViews(mapDoc);
                    MapProductionHelper.Log("Turning off transparency", MapProductionHelper.LogType.Debug, null);
                    TurnOffTransparency(mapDoc);
                    MapProductionHelper.Log("Removing any instances of maintenance plat layers", MapProductionHelper.LogType.Debug, null);
                    RemoveMainPlatLayers(mapDoc);
                    if (!MxdsList.ContainsKey(curmxdName))
                    {
                        MxdsList.Add(curmxdName, mapDoc);

                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return MxdsList;
        }
        /// <summary>
        /// Initialize MXD
        /// </summary>
        /// <param name="configurationFile"></param>
        /// <returns></returns>
        private MapDocument  IntializeMXD(string curmxdName,ref IDictionary<string, MapDocument> MXDList)
        {
            MapDocument _returnMapDoc = null;
            try
            {
                #region InitializeMXD
                //Initialize our active views, turn off transparency and remove 
                //maintenanceplat layers (because of problem with ATEs)
                string mxdDirectory = Common.Common.MxdFolder;

                MapProductionHelper.Log("Map Type or Scale changed", MapProductionHelper.LogType.Debug, null);
                MapProductionHelper.Log("mxdDirectory: " + mxdDirectory, MapProductionHelper.LogType.Info, null);
                string mxdFile = mxdDirectory + "\\" + curmxdName;
                if (!File.Exists(mxdFile))
                    throw new Exception("Map Document file not found!");
                _returnMapDoc = new MapDocument();
                _returnMapDoc.Open(mxdFile);
                MapProductionHelper.Log("Initializing Mxd", MapProductionHelper.LogType.Info, null);
                InitActiveViews(_returnMapDoc);
                MapProductionHelper.Log("Turning off transparency", MapProductionHelper.LogType.Debug, null);
                TurnOffTransparency(_returnMapDoc);
                MapProductionHelper.Log("Removing any instances of maintenance plat layers", MapProductionHelper.LogType.Debug, null);
                RemoveMainPlatLayers(_returnMapDoc);
                if (!MXDList.ContainsKey(curmxdName))
                {
                    MXDList.Add(curmxdName, _returnMapDoc);

                }

                #endregion
            }
            catch (Exception ex)
            {
               
                throw ex;
            }
            return _returnMapDoc;
        }
        /// <summary>
        /// Assumed that the gemoetry passed in here is always going to be in WGS 1984 Geographic Coordinate system and not GCS 1927 or HARN
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public IGeometry ProjectToWGS84(IGeometry geometry)
        {
            int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;

            ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();

            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem m_GeographicCoordinateSystem;

            m_GeographicCoordinateSystem = pSRF.CreateGeographicCoordinateSystem(geoType);

            ESRI.ArcGIS.Geometry.IPoint pPoint = new ESRI.ArcGIS.Geometry.PointClass();
            IGeometry geom = (IGeometry)((IClone)geometry).Clone();
            geom.Project(m_GeographicCoordinateSystem);
            return geom;
        }

        /// <summary>
        /// Refresh the Map from the PageLayout
        /// </summary>
        /// <param name="pageLayout">PageLayout</param>
        public static void RefreshMapFromPageLayout(IPageLayout pageLayout)
        {

            if (pageLayout != null)
            {
                IGraphicsContainer graphicsContainer = pageLayout as IGraphicsContainer;
                graphicsContainer.Reset();
                IElement element = null;
                //Looping through the Elements of the Map Document to find the element
                while ((element = graphicsContainer.Next()) != null)
                {
                    //Get the IMapFrame object from the GraphicsContainer Elements. 
                    if (element is IMapFrame)
                    {
                        //Get Map from IMapFrame
                        IMap map = (element as IMapFrame).Map;
                        //Cast the IActiveView from Map and refresh the map.
                        (map as IActiveView).Refresh();
                    }
                }
            }
        }

        /// <summary>
        /// Create a hash symbol by specifying Boder/buffer.
        /// </summary>
        /// <param name="fillColorRGB">A System.int[] that is the RGB color of the fill symbol. Example: {0,255,0}</param>
        /// <param name="borderColorRGB">A System.int[] that is the RGB color of the outside line border. Example: {0,255,0}</param>
        /// <param name="fillStyle">A System.int that is the fill style of the fill symbol. Exmple: 0 = Solid</param>
        /// <param name="borderLineWidth">A System.Double that is the width of the outside line border in points. Example: 2</param>
        /// <param name="bufferTransperancy">A System.byte that is the transperancy of the fill symbol.</param>
        /// <returns></returns>
        private IFillSymbol CreateHashSymbol(int[] fillColorRGB, int[] borderColorRGB, esriSimpleFillStyle fillStyle, double borderLineWidth, byte bufferTransperancy)
        {
            //Create fill color
            IRgbColor fillColor = new RgbColorClass();
            fillColor.Red = fillColorRGB[0];
            fillColor.Green = fillColorRGB[1];
            fillColor.Blue = fillColorRGB[2];
            //fillColor.Transparency = bufferTransperancy;

            //Create border color
            IRgbColor borderColor = new RgbColorClass();
            borderColor.Red = borderColorRGB[0];
            borderColor.Green = borderColorRGB[1];
            borderColor.Blue = borderColorRGB[2];

            //Create simple line symbol
            //ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            //simpleLineSymbol.Width = borderLineWidth;
            //simpleLineSymbol.Color = borderColor;

            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Width = borderLineWidth;
            simpleLineSymbol.Color = borderColor;
            simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;

            IFillSymbol fillSymbol = null;
            if (fillStyle == esriSimpleFillStyle.esriSFSSolid)
            {
                // Create simple fill symbol
                ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
                simpleFillSymbol.Outline = simpleLineSymbol;
                simpleFillSymbol.Style = fillStyle;
                simpleFillSymbol.Color = fillColor;
                fillSymbol = simpleFillSymbol;
            }
            else
            {
                double angle = 45;
                switch (fillStyle)
                {
                    case esriSimpleFillStyle.esriSFSForwardDiagonal:
                        angle = -45; break;
                    case esriSimpleFillStyle.esriSFSBackwardDiagonal:
                        angle = 45; break;
                    case esriSimpleFillStyle.esriSFSHorizontal:
                        angle = 0; break;
                    case esriSimpleFillStyle.esriSFSVertical:
                        angle = 90; break;

                }
                ILineFillSymbol lineFilleSymbol = new LineFillSymbolClass();
                lineFilleSymbol.Angle = angle;
                //lineFilleSymbol.LineSymbol = simpleLineSymbol;
                lineFilleSymbol.Color = fillColor;
                lineFilleSymbol.Outline = simpleLineSymbol;
                lineFilleSymbol.Separation = 20;
                fillSymbol = lineFilleSymbol;
            }

            //return simpleFillSymbol;
            return fillSymbol;
        }

        private IPolygon convertMapGeometryToLayoutGraphic(IMapFrame mapFrame, IPolygon polygon, out Boolean isGeometryOutOfExtent)
        {
            isGeometryOutOfExtent = false;
            IElement mapElement = mapFrame as IElement;

            //mapFrame.ExtentType = esriExtentTypeEnum.esriExtentBounds;
            // Get the bounds of the MapFrame.
            Double dLayoutXMin = mapElement.Geometry.Envelope.XMin;
            Double dLayoutXMax = mapElement.Geometry.Envelope.XMax;
            Double dLayoutYMin = mapElement.Geometry.Envelope.YMin;
            Double dLayoutYMax = mapElement.Geometry.Envelope.YMax;
            //mapFrame.ExtentType = esriExtentTypeEnum.esriExtentScale;
            IEnvelope mapEnv = (mapFrame.Map as IActiveView).Extent;
            mapFrame.MapBounds = mapEnv;
            //IEnvelope newMapEnv = GetExtentToDraw(mapFrame.Map, mapElement.Geometry.Envelope.Width, mapElement.Geometry.Envelope.Height);
            (mapFrame.Map as IActiveView).Extent = mapEnv;
            mapEnv = (mapFrame.Map as IActiveView).Extent;
            //Get the bounds of the Map.
            Double dMapXMin = mapEnv.XMin;
            Double dMapXMax = mapEnv.XMax;
            Double dMapYMin = mapEnv.YMin;
            Double dMapyMax = mapEnv.YMax;
            Double dMapScale = mapFrame.MapScale;


            double pageHeight = 0;//(_pageLayout as IActiveView).Extent.Height;
            double pageWidth = 0;
            //_pageLayout.Page.QuerySize(out pageWidth, out pageHeight);
            pageWidth = mapElement.Geometry.Envelope.Width;
            pageHeight = mapElement.Geometry.Envelope.Height;
            IClone polygonClone = polygon as IClone;
            IGeometry geometryPolygon = polygonClone.Clone() as IGeometry;
            geometryPolygon.SpatialReference = polygon.SpatialReference;

            IPolygon4 polygonGeometry = MapProductionFacade.ProjectGeometryToMapCoordinateSystem(geometryPolygon, mapFrame.Map) as IPolygon4;  //polygonClone.Clone() as IPolygon4;
            //Getting all the exteriorRing of the polygon
            IGeometryCollection exteriorRingGeometryCollection = polygonGeometry.ExteriorRingBag as IGeometryCollection;
            //Loop through each exterior ring
            for (int i = 0; i < exteriorRingGeometryCollection.GeometryCount; i++)
            {
                //Getting exterior ring.
                IRing exteriorRing = exteriorRingGeometryCollection.get_Geometry(i) as IRing;
                //Getting all the interior ring in the current exterior ring.
                IGeometryCollection interiorRingGeometryCollection = polygonGeometry.InteriorRingBag[exteriorRing] as IGeometryCollection;

                //loop through each exterior ring points.
                IPointCollection exteriorRingPoints = exteriorRingGeometryCollection.get_Geometry(i) as IPointCollection;
                for (int k = 0; k < (exteriorRingPoints.PointCount - 1); k++)
                {
                    //converting the map point coordinate to page layout coordinate.
                    IPoint layoutPoint = new PointClass();
                    //double xX = (((exteriorRingPoints.get_Point(k).X - dMapXMin) * pageWidth) / dMapScale) + dLayoutXMin;
                    //double yY = (((exteriorRingPoints.get_Point(k).Y - dMapYMin) * pageHeight) / dMapScale) + dLayoutYMin;
                    double xX = (((exteriorRingPoints.get_Point(k).X - dMapXMin)) / (mapEnv.Width / pageWidth)) + dLayoutXMin;
                    double yY = (((exteriorRingPoints.get_Point(k).Y - dMapYMin)) / (mapEnv.Height / pageHeight)) + dLayoutYMin;

                    ////The extent of the polygon is not within the current map extent.
                    //if (xX < dLayoutXMin || xX > dLayoutXMax)
                    //{
                    //    isGeometryOutOfExtent = true;
                    //}
                    //if (yY < dLayoutYMin || yY > dLayoutYMax)
                    //{
                    //    isGeometryOutOfExtent = true;
                    //}

                    layoutPoint.PutCoords(xX, yY);
                    //Updating the current point to layout point.
                    exteriorRingPoints.UpdatePoint(k, layoutPoint);

                }

                //Loop through each interior ring.
                for (int interiorGeometryIndx = 0; interiorGeometryIndx < interiorRingGeometryCollection.GeometryCount; interiorGeometryIndx++)
                {
                    //Getting the points of the current interior ring.
                    IPointCollection interiorRingPoints = interiorRingGeometryCollection.get_Geometry(interiorGeometryIndx) as IPointCollection;
                    //Loop through each interior ring points.
                    for (int j = 0; j < (interiorRingPoints.PointCount - 1); j++)
                    {
                        //converting the map point coordinate to page layout coordinate.
                        IPoint layoutPoint = new PointClass();
                        //double xX = (((interiorRingPoints.get_Point(j).X - dMapXMin) * 12) / dMapScale) + dLayoutXMin;
                        //double yY = (((interiorRingPoints.get_Point(j).Y - dMapYMin) * 12) / dMapScale) + dLayoutYMin;
                        double xX = (((interiorRingPoints.get_Point(j).X - dMapXMin)) / (mapEnv.Width / pageWidth)) + dLayoutXMin;
                        double yY = (((interiorRingPoints.get_Point(j).Y - dMapYMin)) / (mapEnv.Height / pageHeight)) + dLayoutYMin;

                        //if (xX < dLayoutXMin || xX > dLayoutXMax)
                        //{
                        //    isGeometryOutOfExtent = true;
                        //}
                        //if (yY < dLayoutYMin || yY > dLayoutYMax)
                        //{
                        //    isGeometryOutOfExtent = true;
                        //}

                        layoutPoint.PutCoords(xX, yY);
                        //Updating the current point to layout point.
                        interiorRingPoints.UpdatePoint(j, layoutPoint);
                    }
                }
            }

            return polygonGeometry as IPolygon;

        }

        /// <summary>
        /// Add hash element to the Pagelayout
        /// </summary>
        /// <param name="map">Current Map</param>
        /// <param name="hashGeometry">Hash Geometry</param>
        /// <param name="hashSymbol">Hash Symbol</param>
        private void AddHashLayoutElement(IMapFrame pMapFrame, IPageLayout pageLayOut, IGeometry hashGeometry, IFillSymbol hashSymbol, bool shouldStoreEleProps, MapProductionHelper pMapProdHelper)
        {
            IGraphicsContainer layoutGraphicsContainer = pageLayOut as IGraphicsContainer;
            //Finding the mapframe element in pagelayout.
            layoutGraphicsContainer.Reset();
            IElement elementMap = layoutGraphicsContainer.Next();
            //IMapFrame mapFrame = null;
            //while (elementMap != null)
            //{

            //    mapFrame = elementMap as IMapFrame;
            //    if (mapFrame != null)
            //    {
            //        break;
            //    }

            //    elementMap = layoutGraphicsContainer.Next();
            //}


            //Finding layout polygon element.
            IElement elementGeo = MapProductionFacade.GetElementByName("PGE Hash Buffer Graphic Simon", layoutGraphicsContainer);

            //Store the original Geometry and Symbol of the polygon element before drawing
            if (shouldStoreEleProps) MapProductionFacade.StoreElementPropeties(elementGeo);

            //checking if both mapframe and layout graphic polygon found then add the hash geometry.
            if (pMapFrame != null && elementGeo != null)
            {
                IElement mapElement = pMapFrame as IElement;
                //graphicsContainer = mapFrame.Container;
                IPolygon hashLayoutPolygon = hashGeometry as IPolygon;

                elementGeo.Geometry = hashLayoutPolygon as IGeometry;
                if (hashSymbol != null)
                {
                    (elementGeo as IFillShapeElement).Symbol = hashSymbol;
                }
                layoutGraphicsContainer.UpdateElement(elementGeo);
                layoutGraphicsContainer.UpdateElement(pMapFrame as IElement);
                (pMapFrame.Map as IActiveView).Refresh();
            }
            else
            {
                MapProductionHelper.Log("MapFrame or graphic polygon '" + MapProductionFacade.PGELayoutPolygonGraphicCaption + "' not found in the pagelayout.", MapProductionHelper.LogType.Info, null);
            }
        }

        private IMapFrame GetMapFrame(IPageLayout pPageLayout)
        {
            IMapFrame retVal = null;
            IGraphicsContainer gc = (IGraphicsContainer)pPageLayout;
            IActiveView activeView = (IActiveView)pPageLayout;
            IElement element = null;
            gc.Reset();
            double currWidth = 0.0;
            double currHeight = 0.0;
            double width = 0.0;
            double height = 0.0;
            IElement largestMapFrame = null;
            while ((element = gc.Next()) != null)
            {
                if (element is IMapFrame)
                {
                    GetWidthAndHeight(element, activeView, out currWidth, out currHeight);
                    if ((width * height) < (currWidth * currHeight))
                    {
                        largestMapFrame = element;
                        width = currWidth;
                        height = currHeight;
                    }
                }
            }
            gc.Reset();
            element = null;
            if (largestMapFrame != null)
            {
                retVal = (IMapFrame)largestMapFrame;
                //MapToWork = _mapFrame.Map;
            }
            return retVal;
        }

        /// <summary>
        /// Gets the Height and Widht of the passed in IElement object
        /// </summary>
        /// <param name="element"></param>
        /// <param name="activeView"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void GetWidthAndHeight(IElement element, IActiveView activeView, out double width, out double height)
        {
            width = 0.0;
            height = 0.0;
            IEnvelope env = new EnvelopeClass();
            element.QueryBounds(activeView.ScreenDisplay, env);
            width = Math.Round(env.Width);
            height = Math.Round(env.Height);
        }
        #endregion
        #region Private Methods


        /// <summary>
        /// Inserts and error to the change detection table for the map number specified
        /// </summary>
        /// <param name="mapNumber">Map number with an error</param>
        /// <param name="scale">Scale of the map</param>
        /// <param name="error">Error description</param>
        //private void InsertErrorToChangeDetectionTable(IWorkspace pWS, string mapNumber, string scale, string map_type, string error)
        //{
        //    try
        //    {
        //        if (changeDetectionTable == null)
        //        {
        //            changeDetectionTable = Common.Common.getTable(pWS, Common.Common.TableChangeDetection);
        //            mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
        //            mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);
        //            errorIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionError);
        //            exportStateIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionExportState);
        //        }

        //        List<string> mapNumbersToProcess = new List<string>();

        //        IQueryFilter qf = new QueryFilterClass();
        //        qf.SubFields = Common.Common.FieldChangeDetectionError;
        //        qf.WhereClause = Common.Common.FieldChangeDetectionMapNumber + " = '" + mapNumber + "' AND " + Common.Common.FieldChangeDetectionMapType + " = '" + map_type + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'";
        //        ICursor rowCursor = changeDetectionTable.Update(qf, true);
        //        IRow changeDetectionRow = rowCursor.NextRow();

        //        while (changeDetectionRow != null)
        //        {
        //            changeDetectionRow.set_Value(errorIndex, error);
        //            changeDetectionRow.Store();
        //            changeDetectionRow = rowCursor.NextRow();
        //        }

        //        if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
        //        if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
        //        if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

        //    }
        //    catch (Exception e)
        //    {
        //        Log("Unable to insert rows into the " + Common.Common.TableChangeDetection + " table for map number " + mapNumber);
        //    }
        //}

        /// <summary>
        /// Insert the necessary information into the Tiff Table
        /// </summary>
        /// <param name="mapGridFeature">IFeature for map grid to get information from</param>
        /// <param name="mapNumber">Map Number for the feature</param>
        /// <param name="division">Division the map grid belongs to</param>
        /// <param name="district">District the map grid belongs to</param>
        //private void InsertTIFFInformation(IWorkspace pWS, IEnvelope mapExtent, IFeature mapGrid, string mapNumber, string division, string district)
        //{
        //    try
        //    {
        //        if (TiffTable == null)
        //        {
        //            TiffTable = Common.Common.getTable(pWS, Common.Common.TableTIFUnifiedGrid);
        //            FieldTIFMapDivision = TiffTable.FindField(Common.Common.FieldTIFMapDivision);
        //            FieldTIFMapDistrict = TiffTable.FindField(Common.Common.FieldTIFMapDistrict);
        //            FieldTIFLongMin = TiffTable.FindField(Common.Common.FieldTIFLongMin);
        //            FieldTIFLongMax = TiffTable.FindField(Common.Common.FieldTIFLongMax);
        //            FieldTIFLatMin = TiffTable.FindField(Common.Common.FieldTIFLatMin);
        //            FieldTIFLatMax = TiffTable.FindField(Common.Common.FieldTIFLatMax);
        //            FieldTIFMapNumber = TiffTable.FindField(Common.Common.FieldTIFMapNumber);
        //            FieldTIFLastModified = TiffTable.FindField(Common.Common.FieldTIFLastModified);
        //        }

        //        //Determine our maximum latitude and longitude values for this feature
        //        double LongMin;
        //        double LongMax;
        //        double LatMin;
        //        double LatMax;
        //        GetLatLong(mapExtent, mapGrid, out LongMin, out LongMax, out LatMin, out LatMax);

        //        //Get the current date in an oracle friendly format
        //        string changeDate = new System.Data.OracleClient.OracleDateTime(DateTime.Now).ToString();

        //        IQueryFilter qf = new QueryFilterClass();
        //        qf.SubFields = "*";
        //        qf.WhereClause = Common.Common.FieldTIFMapNumber + " = '" + mapNumber + "'";
        //        ICursor rowCursor = TiffTable.Update(qf, true);
        //        IRow TiffRow = rowCursor.NextRow();

        //        if (TiffRow != null)
        //        {
        //            TiffRow.set_Value(FieldTIFMapDivision, division);
        //            TiffRow.set_Value(FieldTIFMapDistrict, district);
        //            TiffRow.set_Value(FieldTIFLongMin, LongMin);
        //            TiffRow.set_Value(FieldTIFLongMax, LongMax);
        //            TiffRow.set_Value(FieldTIFLatMin, LatMin);
        //            TiffRow.set_Value(FieldTIFLatMax, LatMax);
        //            TiffRow.set_Value(FieldTIFMapNumber, mapNumber);
        //            TiffRow.set_Value(FieldTIFLastModified, changeDate);
        //            TiffRow.Store();

        //            if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
        //            if (TiffRow != null) { while (Marshal.ReleaseComObject(TiffRow) > 0) { } }
        //            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
        //        }
        //        else
        //        {
        //            ICursor insertCursor = TiffTable.Insert(true);
        //            IRowBuffer newRow = TiffTable.CreateRowBuffer();
        //            newRow.set_Value(FieldTIFMapDivision, division);
        //            newRow.set_Value(FieldTIFMapDistrict, district);
        //            newRow.set_Value(FieldTIFLongMin, LongMin);
        //            newRow.set_Value(FieldTIFLongMax, LongMax);
        //            newRow.set_Value(FieldTIFLatMin, LatMin);
        //            newRow.set_Value(FieldTIFLatMax, LatMax);
        //            newRow.set_Value(FieldTIFMapNumber, mapNumber);
        //            newRow.set_Value(FieldTIFLastModified, changeDate);
        //            insertCursor.InsertRow(newRow);
        //            insertCursor.Flush();

        //            if (insertCursor != null) { while (Marshal.ReleaseComObject(insertCursor) > 0) { } }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        //Do not throw an error here 
        //        Log("Unable to insert rows into the " + Common.Common.TableTIFUnifiedGrid + " table for map number " + mapNumber);
        //    }
        //}

        ///// <summary>
        ///// Returns the max and min latitude and longitude values of the envelope for the provided feature
        ///// </summary>
        ///// <param name="mapGridFeature">IFeature to get the envelope from</param>
        ///// <param name="LongMin">Longitude Minimum value</param>
        ///// <param name="LongMax">Longitude Maximum value</param>
        ///// <param name="LatMin">Latitude Minimum value</param>
        ///// <param name="LatMax">Latitude Maximum value</param>
        private void GetLatLong(IEnvelope mapExtent, IFeature mapGrid, out double LongMin, out double LongMax, out double LatMin, out double LatMax)
        {
            //Get our coordinate system to project to
            //if (geoLatLongSpatReference == null)
            //{
            //    int geoType = (int)esriSRGeoCSType.esriSRGeoCS_WGS1984;
            //    //int geoType = 26910;
            //    ESRI.ArcGIS.Geometry.ISpatialReferenceFactory pSRF = new ESRI.ArcGIS.Geometry.SpatialReferenceEnvironmentClass();
            //    ISpatialReferenceFactory2 pSRF2 = pSRF as ISpatialReferenceFactory2;

            //    //geoLatLongSpatReference = pSRF.CreateGeographicCoordinateSystem(geoType);
            //    geoLatLongSpatReference = pSRF2.CreateSpatialReference(geoType);
            //}

            //Determine the max,min values for the features envelope
            IEnvelope featEnvelope = mapExtent;
            double xMin = featEnvelope.XMin;
            double xMax = featEnvelope.XMax;
            double yMin = featEnvelope.YMin;
            double yMax = featEnvelope.YMax;

            LongMin = xMin;
            LongMax = xMax;
            LatMin = yMin;
            LatMax = yMax;

            ////Current coordinate system
            //ISpatialReference spatReference = mapGrid.Shape.SpatialReference;


            ////Build our min and max points for projection
            //IPoint lowerLeft = new PointClass();
            //IPoint upperRight = new PointClass();
            //IPoint upperLeft = new PointClass();
            //IPoint lowerRight = new PointClass();
            //lowerLeft.X = xMin;
            //lowerLeft.Y = yMin;
            //upperRight.X = xMax;
            //upperRight.Y = yMax;
            //upperLeft.X = xMin;
            //upperLeft.Y = yMax;
            //lowerRight.X = xMax;
            //lowerRight.Y = yMin;

            ////Project our points to the new coordinate system to get latitude and longitude values
            //IPoint projectedupperRight = ProjectPoint(lowerLeft, spatReference, geoLatLongSpatReference);
            //IPoint projectedlowerLeft = ProjectPoint(upperRight, spatReference, geoLatLongSpatReference);
            //IPoint projectedlowerRight = ProjectPoint(lowerRight, spatReference, geoLatLongSpatReference);
            //IPoint projectedupperLeft = ProjectPoint(upperLeft, spatReference, geoLatLongSpatReference);

            //double lowerRightX = projectedlowerRight.X;
            //double lowerRightY = projectedlowerRight.Y;
            //double upperLeftX = projectedupperLeft.X;
            //double upperLeftY = projectedupperLeft.Y;

            //LongMin = projectedlowerLeft.X;
            //LongMax = projectedupperRight.X;
            //LatMin = projectedlowerLeft.Y;
            //LatMax = projectedupperRight.Y;

            //double lowerLeftX = projectedlowerLeft.X;
            //double lowerLeftY = projectedlowerLeft.Y;
            //double UpperRightX = projectedupperRight.X;
            //double UpperRightY = projectedupperRight.Y;
        }

        /// <summary>
        /// Projects the given point from the original Reference to the new spatial reference
        /// </summary>
        /// <param name="origPoint">Original IPoint</param>
        /// <param name="origReference">Original Spatial Reference</param>
        /// <param name="newCoordSystem">New Spatial Reference</param>
        /// <returns></returns>
        private IPoint ProjectPoint(IPoint origPoint, ISpatialReference origReference, ISpatialReference newCoordSystem)
        {
            //Create a new point to project
            IPoint newPoint = new PointClass();
            newPoint.X = origPoint.X;
            newPoint.Y = origPoint.Y;
            newPoint.SpatialReference = origReference;

            //Project our point
            ESRI.ArcGIS.Geometry.ISpatialReference pGCS = new ESRI.ArcGIS.Geometry.GeographicCoordinateSystemClass();
            newPoint.Project(newCoordSystem);

            return newPoint;
        }
        

        /// <summary>
        /// This method will attempt to delete any temp files (that aren't currently in use) so the folder is always clean of them
        /// </summary>
        //private void DeleteTempFiles()
        //{
        //    try
        //    {
        //        string tempFileDirectory = exportLocation + "-" + Scale;

        //        foreach (string fileName in Directory.GetFiles(tempFileDirectory))
        //        {
        //            try
        //            {
        //                if (fileName.Contains(".temp"))
        //                {
        //                    File.Delete(fileName);
        //                }
        //            }
        //            catch
        //            {

        //            }
        //        }
        //    }
        //    catch (Exception e) { }
        //}

        /// <summary>
        /// Removes any feature layers referring to the Maintenance Plat feature class.  This is necessary
        /// to keep auto text elements working properly.
        /// </summary>
        /// <param name="featClassName"></param>
        private void RemoveMainPlatLayers(MapDocument mapDoc)
        {
            string featureClassName = Common.Common.TableMaintenancePlat;
            for (int i = 0; i < mapDoc.MapCount; i++)
            {
                IMap currentMap = mapDoc.get_Map(i);
                List<IFeatureLayer> featLayers = Common.Common.getFeatureLayersFromMap(currentMap, featureClassName);
                foreach (IFeatureLayer featLayer in featLayers)
                {
                    currentMap.DeleteLayer(featLayer);
                }
            }
        }

        /// <summary>
        /// This method will open the desired stored display into all of the IMap objects of the mxd
        /// </summary>
        /// <param name="SDName">Name of the Stored Display</param>
        //public void OpenStoredDisplay(string SDName)
        //{
        //    Log("Loading Stored Display...");
        //    int mapCount = mapDoc.MapCount;
        //    for (int i = 0; i < mapCount; i++)
        //    {
        //        string dataFrameName = mapDoc.get_Map(i).Name;
        //        if (!Common.Common.OpenStoredDisplay(SDName, workspace, mapDoc.get_Map(i)))
        //        {
        //            Log("Unable to find stored display: " + SDName);
        //        }
        //        else
        //        {
        //            if (dataFrameName.ToUpper() == "MainDataFrame".ToUpper())
        //            {
        //                mapDoc.get_Map(i).Name = "MainDataFrame";
        //            }
        //        }
        //    }
        //}        

        /// <summary>
        /// Initialize all of the activeviews of the current map document
        /// </summary>
        private void InitActiveViews(MapDocument mapDoc)
        {
            #region Open Map Document
            bool Error = true;
            int itry = 0;
            while (!Error)
            {
                try
                {
                    if (itry < 4)
                    {
                        int mapCount = mapDoc.MapCount;
                        for (int i = 0; i < mapCount; i++)
                        {
                            IActiveView mapActiveView = mapDoc.get_Map(i) as IActiveView;
                        }
                        IActiveView activeView = mapDoc.PageLayout as IActiveView;
                        activeView.Activate(0);

                        Error = false;
                        itry = itry + 1;
                        break;
                    }
                    else
                    {
                        Error = false;
                        break;
                    }

                }
                catch
                { Thread.Sleep(Common.Common.SleepTime); }
            }
            #endregion

            
        }

        /// <summary>
        /// This method will turn transparency off on all feature layers.  This is necessary to work around an Esri bug
        /// http://support.esri.com/en/knowledgebase/techarticles/detail/22070
        /// http://support.esri.com/en/bugs/nimbus/TklNMDcwMTQ4
        /// </summary>
        private void TurnOffTransparency(MapDocument mapDoc)
        {
            int mapCount = mapDoc.MapCount;
            for (int i = 0; i < mapCount; i++)
            {
                IMap map = mapDoc.get_Map(i);

                //Get all of the ILayers
                IEnumLayer enumLayer = map.get_Layers(null, true);
                enumLayer.Reset();
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    TurnOffTransparency(layer);
                    layer = enumLayer.Next();
                }
            }
        }

        /// <summary>
        /// Turns off transparency for a given layer
        /// </summary>
        /// <param name="layer">ILayer to turn off transparency on</param>
        private void TurnOffTransparency(ILayer layer)
        {
            //If this layer supports transparency and it is not set 0, then set it to 0 now.
            ILayerEffects effects = layer as ILayerEffects;
            if (effects != null && effects.SupportsTransparency)
            {
                short transparency = effects.Transparency;
                if (effects.Transparency != 0)
                {
                    effects.Transparency = 0;
                }
            }

            //Loop through this layers containing layers if it is a composite layer
            if (layer is ICompositeLayer)
            {
                ICompositeLayer compLayer = layer as ICompositeLayer;
                for (int i = 0; i < compLayer.Count; i++)
                {
                    TurnOffTransparency(compLayer.get_Layer(i));
                }
            }
        }

        /// <summary>
        /// Deletes the specified pdf file
        /// </summary>
        /// <param name="location">Location of the file without the ".pdf" extension</param>
        /// <returns>True if file is deleted (or it didn't exist).  False if delete fails</returns>
        private bool deleteMap(string location, string format)
        {
            string deleteFile = location + "." + format.ToLower();
            string deleteTFW = location + "." + "tfw"; 

            try
            {
                if (File.Exists(deleteFile))
                {
                    File.Delete(deleteFile);
                }

                if (format.ToLower() == ".tif")
                {
                    if (File.Exists(deleteTFW))
                    {
                        File.Delete(deleteTFW);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Copies the map from the output location to the final 
        /// location 
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="toLocation"></param>
        /// <param name="format"></param>
        /// <param name="pMapProdHelper"></param>
        /// <returns></returns>
        private bool copyPDF(
            string fromLocation,
            string toLocation,
            string format,
            ref string errorMsg,
            MapProductionHelper pMapProdHelper)
        {

            string theFromLocation = "";
            string theToLocation = "";

            try
            {
                for (int i = 1; i <= 2; i++)
                {
                    if (i == 1)
                    {
                        theFromLocation = fromLocation + "." + format.ToLower();
                        theToLocation = toLocation + "." + format.ToLower();

                        if (File.Exists(theFromLocation))
                        {
                            if (IsFileSizeGreaterThanMinimumThreshold(
                                theFromLocation,
                                format,
                                pMapProdHelper))
                            {
                                File.Copy(theFromLocation, theToLocation, true);
                            }
                            else
                            {
                                errorMsg = format.ToUpper() + " output file is less than minumum threshold";
                                return false;
                            }
                        }
                        else
                        {
                            errorMsg = format.ToUpper() + " file copy failed, because export output was not found at temp location";
                            return false;
                        }
                    }
                    else
                    {
                        if (format.ToLower() == "tif")
                        {
                            theFromLocation = fromLocation + "." + "tfw";
                            theToLocation = toLocation + "." + "tfw";
                        }
                        else
                            break;

                        if (File.Exists(theFromLocation))
                            File.Copy(theFromLocation, theToLocation, true);
                        else
                        {
                            errorMsg = format.ToUpper() + " file copy failed, because export output was not found at temp location";
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MapProductionHelper.Log("Unable to copy map to ouput directory", MapProductionHelper.LogType.Error, ex);
                errorMsg = "Failed copying map to final export location";
                return false;
            }
        }
        /// <summary>
        /// Copies the map from the output location to the final 
        /// location 
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="toLocation"></param>
        /// <param name="format"></param>
        /// <param name="pMapProdHelper"></param>
        /// <returns></returns>
        //private bool copyPDF(
        //    string fromLocation,
        //    string toLocation,
        //    string format,
        //    MapProductionHelper pMapProdHelper)
        //{

        //    string theFromLocation = "";
        //    string theToLocation = "";

        //    try
        //    {

        //        for (int i = 1; i <= 2; i++)
        //        {
        //            if (i == 1)
        //            {
        //                theFromLocation = fromLocation + "." + format.ToLower();
        //                theToLocation = toLocation + "." + format.ToLower();

        //                if (File.Exists(theFromLocation))
        //                    File.Copy(theFromLocation, theToLocation, true);
        //            }
        //            else
        //            {
        //                if (format.ToLower() == "tif")
        //                {
        //                    theFromLocation = fromLocation + "." + "tfw";
        //                    theToLocation = toLocation + "." + "tfw";
        //                }
        //                else/////////////
        //                    break;

        //                if (File.Exists(theFromLocation))
        //                    File.Copy(theFromLocation, theToLocation, true);
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        pMapProdHelper.Log("Unable to copy map to ouput directory. Skipping update of \"" + toLocation + "\" " + e.Message);
        //        return false;
        //    }
        //}


        /// <summary>
        /// Returns the file size of the passed filename 
        /// </summary>
        /// <param name="theFromLocation"></param>
        /// <param name="pMapProdHelper"></param>
        /// <returns></returns>
        private bool IsFileSizeGreaterThanMinimumThreshold( 
            string theFromLocation, 
            string format, 
            MapProductionHelper pMapProdHelper)
        {
            long fileSize = -1;
            long minimumSize = -1;
            bool isFileSizeOk = true; 

            try
            {
                format = format.ToLower(); 
                FileInfo fInfo = new FileInfo(theFromLocation);
                fileSize = fInfo.Length;

                if (format == "pdf")
                    minimumSize = Common.Common.MinFileSizePDF; 
                else
                    minimumSize = Common.Common.MinFileSizeTIFF;

                if (fInfo.Length < minimumSize)
                {
                    MapProductionHelper.Log( 
                        theFromLocation + " size: " + fInfo.Length.ToString() +
                        " is less than threshold: " + minimumSize.ToString(), MapProductionHelper.LogType.Info, null);
                    isFileSizeOk = false;
                }

                fInfo = null; 
                return isFileSizeOk;                               
            }
            catch (Exception ex) 
            {
                //throw new exception 
                MapProductionHelper.Log("Error returning file size", MapProductionHelper.LogType.Error, ex);
                return false; 
            }
        }


        /// <summary>
        /// Sets the export state for a given map number/scale combination to the specified export state value
        /// </summary>
        /// <param name="mapNumber">Map Number</param>
        /// <param name="scale">Scale of map number</param>
        /// <param name="exportState">Export State (obtain from Common.Common.GetExportState method)</param>
        //private void SetExportState(string mapNumber, string scale, int exportState)
        //{
        //    if (changeDetectionTable == null)
        //    {
        //        changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
        //        mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
        //        mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);
        //        errorIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionError);
        //        exportStateIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionExportState);
        //    }

        //    List<string> mapNumbersToProcess = new List<string>();

        //    IQueryFilter qf = new QueryFilterClass();
        //    qf.SubFields = Common.Common.FieldChangeDetectionExportState + "," + Common.Common.FieldChangeDetectionError;
        //    qf.WhereClause = Common.Common.FieldChangeDetectionMapNumber + " = '" + mapNumber + "' AND " + Common.Common.FieldChangeDetectionMapType + " = '" + TemplateName + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'";
        //    ICursor rowCursor = changeDetectionTable.Update(qf, true);
        //    IRow changeDetectionRow = rowCursor.NextRow();

        //    while (changeDetectionRow != null)
        //    {
        //        //changeDetectionRow.set_Value(errorIndex, "");
        //        changeDetectionRow.set_Value(exportStateIndex, exportState);
        //        changeDetectionRow.Store();
        //        changeDetectionRow = rowCursor.NextRow();
        //    }

        //    if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
        //    if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
        //    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
        //}

        /// <summary>
        /// Gets the current export state for the map number
        /// </summary>
        /// <param name="mapNumber">Map Number to check</param>
        /// <param name="scale">Scale of the provided map number</param>
        /// <returns></returns>
        //private string GetExportState(string mapNumber, string scale)
        //{
        //    string state = "";

        //    if (changeDetectionTable == null)
        //    {
        //        changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
        //        mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
        //        mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);
        //        errorIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionError);
        //        exportStateIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionExportState);
        //    }

        //    List<string> mapNumbersToProcess = new List<string>();

        //    IQueryFilter qf = new QueryFilterClass();
        //    qf.SubFields = Common.Common.FieldChangeDetectionExportState;
        //    qf.WhereClause = Common.Common.FieldChangeDetectionMapNumber + " = '" + mapNumber + "' AND " + Common.Common.FieldChangeDetectionMapType + " = '" + TemplateName + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'";
        //    ICursor rowCursor = changeDetectionTable.Search(qf, true);
        //    IRow changeDetectionRow = rowCursor.NextRow();

        //    if (changeDetectionRow != null)
        //    {
        //        state = changeDetectionRow.get_Value(exportStateIndex).ToString();
        //    }

        //    if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
        //    if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
        //    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

        //    return state;
        //}

        /// <summary>
        /// This method will read in the input that needs to be processed from the change detection grid table.
        /// </summary>
        /// <returns>String array of the numbers to process</returns>
        //private string[] ReadMapNumbers()
        //{
        //    Log("Reading map numbers to process");

        //    if (changeDetectionTable == null)
        //    {
        //        changeDetectionTable = Common.Common.getTable(workspace, Common.Common.TableChangeDetection);
        //        mapNumIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapNumber);
        //        mapTypeIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionMapType);
        //        errorIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionError);
        //        exportStateIndex = changeDetectionTable.FindField(Common.Common.FieldChangeDetectionExportState);
        //    }

        //    string currentMapNum = "";
        //    List<string> mapNumbersToProcess = new List<string>();
            
        //    IQueryFilter qf = new QueryFilterClass();
        //    qf.SubFields = Common.Common.FieldChangeDetectionMapNumber + "," + Common.Common.FieldChangeDetectionMapType;
        //    qf.WhereClause = Common.Common.FieldChangeDetectionMapType + " = '" + TemplateName + "' AND " + Common.Common.FieldChangeDetectionScale + " = '" + scale + "'"
        //        + " AND " + Common.Common.FieldChangeDetectionExportState + " is null";
        //    ICursor rowCursor = changeDetectionTable.Search(qf, true);

        //    IRow changeDetectionRow = rowCursor.NextRow();

        //    while (changeDetectionRow != null)
        //    {
        //        currentMapNum = changeDetectionRow.get_Value(mapNumIndex).ToString();
        //        mapNumbersToProcess.Add(currentMapNum);
        //        changeDetectionRow = rowCursor.NextRow();
        //    }

        //    if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
        //    if (changeDetectionRow != null) { while (Marshal.ReleaseComObject(changeDetectionRow) > 0) { } }
        //    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }

        //    string[] mapNumArray = mapNumbersToProcess.ToArray();

        //    return mapNumArray;
        //}

        /// <summary>
        /// This method will export a map based on the input paramters
        /// </summary>
        /// <param name="iOutputResolution">Resolution requested in dpi</param>
        /// <param name="lResampleRatio">Sample ratio 1 to 5. 1 corresponds to the best quality</param>
        /// <param name="ExportType">String that represents the export type to create (i.e. PDF)</param>
        /// <param name="sOutputDir">Directory to where the document should be created</param>
        /// <param name="bClipToGraphicsExtent">Assign True or False to determine if export image will be clipped to the graphic * extent of layout elements.  This value is ignored for data view exports</param>
        /// <param name="Name">Name of the exported file without the file extension (i.e. Output not Output.pdf)</param>
        private bool ExportActiveViewParameterized( 
            long iOutputResolution, 
            long lResampleRatio, 
            string ExportType, 
            string sOutputDir, 
            bool bClipToGraphicsExtent, 
            string Name, 
            bool isColor, 
            MapProductionHelper pMapProdHelper,
            MapDocument mapDoc)
        {

            bool exportSuccesss= false;
            /* EXPORT PARAMETER: (iOutputResolution) the resolution requested.
             * EXPORT PARAMETER: (lResampleRatio) Output Image Quality of the export.  The value here will only be used if the export
             * object is a format that allows setting of Output Image Quality, i.e. a vector exporter.
             * The value assigned to ResampleRatio should be in the range 1 to 5.
             * 1 corresponds to "Best", 5 corresponds to "Fast"
             * EXPORT PARAMETER: (ExportType) a string which contains the export type to create.
             * EXPORT PARAMETER: (sOutputDir) a string which contains the directory to output to.
             * EXPORT PARAMETER: (bClipToGraphicsExtent) Assign True or False to determine if export image will be clipped to the graphic 
             * extent of layout elements.  This value is ignored for data view exports
             */

            /* Exports the Active View of the document to selected output format. */

            // using predefined static member
           //IActiveView docActiveView = mapDoc.PageLayout as IActiveView;
            IExport docExport;
            IPrintAndExport docPrintExport;
            IOutputRasterSettings RasterSettings;
            string sNameRoot;
            bool bReenable = false;

            if (GetFontSmoothing())
            {
                /* font smoothing is on, disable it and set the flag to reenable it later. */
                bReenable = true;
                DisableFontSmoothing();
                if (GetFontSmoothing())
                {
                    //font smoothing is NOT successfully disabled, error out. Font smoothing can't be disabled while
                    //executing in a remote session, but it still claims to be enabled even if it is disabled at OS level.  Let's
                    //not return so that the export can finish and rely on the OS to disable font smoothing when being run in a remote
                    //session such as UC4
                    //return;
                }
                //else font smoothing was successfully disabled.
            }


            // The Export*Class() type initializes a new export class of the desired type.
            if (ExportType == "PDF")
            {
                ExportPDFClass pdfClass = new ExportPDFClass();
                //Turn on compression
                pdfClass.Compressed = true;
                pdfClass.ImageCompression = esriExportImageCompression.esriExportImageCompressionDeflate;
                pdfClass.EmbedFonts = true;
                docExport = pdfClass;
            }
            else if (ExportType == "EPS")
            {
                docExport = new ExportPSClass();
            }
            else if (ExportType == "AI")
            {
                docExport = new ExportAIClass();
            }
            else if (ExportType == "BMP")
            {
                docExport = new ExportBMPClass();
            }
            else if (ExportType == "TIF")
            {
                //Set the color mode and compression type 
                docExport = new ExportTIFFClass();                
                IExportTIFF exportTiff = docExport as IExportTIFF;
                exportTiff.GeoTiff = true;
                exportTiff.BiLevelThreshold = 128;

                //Enhancement by PGE IT - requirement to output maps in Color TIFF as 
                //well as black and white 
                if (isColor)
                {
                    exportTiff.CompressionType = esriTIFFCompression.esriTIFFCompressionDeflate;
                    (exportTiff as IExportImage).ImageType = esriExportImageType.esriExportImageTypeIndexed;
                    //(exportTiff as IExportImage).ImageType = esriExportImageType.esriExportImageTypeTrueColor;
                }
                else
                {
                    exportTiff.CompressionType = esriTIFFCompression.esriTIFFCompressionFax4;
                    (exportTiff as IExportImage).ImageType = esriExportImageType.esriExportImageTypeBiLevelThreshold;
                }
                IWorldFileSettings worldFileSettings = docExport as IWorldFileSettings;
                worldFileSettings.OutputWorldFile = true;
                worldFileSettings.MapExtent = ((IActiveView)mapDoc.get_Map(0)).Extent;
                
            }
            else if (ExportType == "SVG")
            {
                docExport = new ExportSVGClass();
            }
            else if (ExportType == "PNG")
            {
                docExport = new ExportPNGClass();
            }
            else if (ExportType == "GIF")
            {
                docExport = new ExportGIFClass();
            }
            else if (ExportType == "EMF")
            {
                docExport = new ExportEMFClass();
            }
            else if (ExportType == "JPEG")
            {
                docExport = new ExportJPEGClass();
            }
            else
            {
                MapProductionHelper.Log("Unsupported export type " + ExportType + ", defaulting to EMF.", MapProductionHelper.LogType.Info, null);
                ExportType = "EMF";
                docExport = new ExportEMFClass();
            }

            docPrintExport = new PrintAndExportClass();

            //set the name root for the export
            sNameRoot = Name;

            //set the export filename (which is the nameroot + the appropriate file extension)
            docExport.ExportFileName = sOutputDir + sNameRoot + "." + docExport.Filter.Split('.')[1].Split('|')[0].Split(')')[0];

            //Output Image Quality of the export.  The value here will only be used if the export
            // object is a format that allows setting of Output Image Quality, i.e. a vector exporter.
            // The value assigned to ResampleRatio should be in the range 1 to 5.
            // 1 corresponds to "Best", 5 corresponds to "Fast"

            // check if export is vector or raster
            if (docExport is IOutputRasterSettings)
            {
                // for vector formats, assign the desired ResampleRatio to control drawing of raster layers at export time   
                RasterSettings = (IOutputRasterSettings)docExport;
                RasterSettings.ResampleRatio = (int)lResampleRatio;

                // NOTE: for raster formats output quality of the DISPLAY is set to 1 for image export 
                // formats by default which is what should be used
            }
           
            //Set to be compressed

            try
            {
                docPrintExport.Export(mapDoc.PageLayout as IActiveView, docExport, iOutputResolution, bClipToGraphicsExtent, null);
                exportSuccesss = true;
            }
            catch (Exception e)
            {
                MapProductionHelper.Log("Failed to docPrintExport", MapProductionHelper.LogType.Error, e);
               
            }
           

            try
            {
                //Attempt to cleanup resources
                docExport.Cleanup();
            }
            catch (Exception e)
            {
                MapProductionHelper.Log("Failed to clean doc export", MapProductionHelper.LogType.Error, e);
            }

            MapProductionHelper.Log("Finished exporting " + sOutputDir + sNameRoot + "." + docExport.Filter.Split('.')[1].Split('|')[0].Split(')')[0] + ".", MapProductionHelper.LogType.Info, null);

            if (bReenable)
            {
                /* reenable font smoothing if we disabled it before */
                EnableFontSmoothing();
                bReenable = false;
                if (!GetFontSmoothing())
                {
                    //error: cannot reenable font smoothing.
                    MapProductionHelper.Log("Unable to reenable Font Smoothing", MapProductionHelper.LogType.Info, null);
                }
            }
            return exportSuccesss;
        }

        /// <summary>
        /// Disable font smooting
        /// </summary>
        private void DisableFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            /* call to systemparametersinfo to set the font smoothing value */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 0, ref pv, SPIF_UPDATEINIFILE);
        }

        /// <summary>
        /// Enable font smoothing
        /// </summary>
        private void EnableFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            /* call to systemparametersinfo to set the font smoothing value */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 1, ref pv, SPIF_UPDATEINIFILE);

        }

        /// <summary>
        /// Determines if font smoothing is currently turned on
        /// </summary>
        /// <returns>True if font smoothing is turned on</returns>
        private Boolean GetFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            /* call to systemparametersinfo to get the font smoothing value */
            iResult = SystemParametersInfo(SPI_GETFONTSMOOTHING, 0, ref pv, 0);

            if (pv > 0)
            {
                //pv > 0 means font smoothing is ON.
                return true;
            }
            else
            {
                //pv == 0 means font smoothing is OFF.
                return false;
            }
        }

        /// <summary>
        /// Returns the desired feature class from the maps in the map document.
        /// </summary>
        /// <param name="featureClassName">Name of the feature class</param>
        /// <returns></returns>
        private IFeatureClass getFeatureClassFromMaps(string featureClassName,MapDocument mapDoc)
        {
            for (int i = 0; i < mapDoc.MapCount; i++)
            {
                IFeatureLayer featLayer = Common.Common.getFeatureLayerFromMap(mapDoc.get_Map(i), featureClassName);
                if (featLayer != null)
                {
                    return featLayer.FeatureClass;
                }
            }
            return null;
        }

        /// <summary>
        /// This will get the location information from the Division feature class in the landbase database.
        /// Region, Division, and District
        /// </summary>
        /// <param name="feature">Feature to get the location of</param>
        /// <returns></returns>
        private List<string> GetLocationInformation( 
            IWorkspace pWS, 
            IFeature feature, 
            MapProductionHelper pMapProdHelper,MapDocument mapDoc)
        {
            List<string> locationInfo = new List<string>();

            //We also need to get the feature classes necessary to determine the directory structure of the map grids
            if (ServiceAreaFeatClass == null)
            {
                ServiceAreaFeatClass = Common.Common.getFeatureClass(pWS, Common.Common.TableServiceArea);
                if (ServiceAreaFeatClass == null)
                {
                    //Try getting it from the map instead 
                    ServiceAreaFeatClass = getFeatureClassFromMaps(Common.Common.TableServiceArea, mapDoc);
                }

                //if it still couldn't be found notify the user
                if (ServiceAreaFeatClass == null)
                {
                    MapProductionHelper.Log("Unable to find the " + Common.Common.TableServiceArea + " feature class in the specified database or the mxd", MapProductionHelper.LogType.Info, null);
                    return null;
                }
            }

            //Get field indices
            if (regionFieldIndex == -1) { regionFieldIndex = ServiceAreaFeatClass.Fields.FindField(Common.Common.FieldRegion); }
            if (districtFieldIndex == -1) { districtFieldIndex = ServiceAreaFeatClass.Fields.FindField(Common.Common.FieldDistrict); }
            if (divisionFieldIndex == -1) { divisionFieldIndex = ServiceAreaFeatClass.Fields.FindField(Common.Common.FieldDivision); }

            //Query the division feature class for the polygon which intersects this feature.  May be more than one
            ISpatialFilter qf = new SpatialFilterClass();
            qf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            qf.Geometry = feature.ShapeCopy;
            qf.GeometryField = ServiceAreaFeatClass.ShapeFieldName;
            qf.SubFields = Common.Common.FieldRegion + "," + Common.Common.FieldDistrict + "," + Common.Common.FieldDivision;
            IFeatureCursor featCursor = ServiceAreaFeatClass.Search(qf, true);
            IFeature feat = featCursor.NextFeature();
            while (feat != null)
            {
                try
                {
                    string region = feat.get_Value(regionFieldIndex).ToString();
                    string division = feat.get_Value(divisionFieldIndex).ToString();
                    string district = feat.get_Value(districtFieldIndex).ToString();
                    locationInfo.Add(region + "," + division + "," + district);
                }
                catch (Exception e) { }

                feat = featCursor.NextFeature();
            }

            //Release our cursor
            if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }

            return locationInfo;

        }

        //private List<string> GetLocationInformation(string serviceLocationString)
        //{
        //    List<string> locationInfo = new List<string>();
        //    string[] serviceAreas = serviceLocationString.Split('*');
        //    if (serviceAreas.Length > 0)
        //    {
        //        for (int i = 0; i < serviceAreas.Length; i++)
        //        {
        //            locationInfo.Add(serviceAreas[i]);
        //        }
        //    }
        //    return locationInfo;
        //}

        /// <summary>
        /// Returns the location information for a given feature based on the intersection of the provided feature
        /// and the provided feature class.
        /// </summary>
        /// <param name="featClass">IFeatureClass to intersect with</param>
        /// <param name="feature">IFeature to instersect with</param>
        /// <param name="fieldName">Field name to return</param>
        /// <param name="fieldIndex">Field index of the field to return</param>
        /// <returns></returns>
        private List<string> GetLocationInformation(IFeature feature, IWorkspace pWS, MapProductionHelper pMapProdHelper,MapDocument mapDoc)
        {
            List<string> locationInfo = new List<string>();

            //We also need to get the feature classes necessary to determine the directory structure of the map grids
            if (ServiceAreaFeatClass == null)
            {
                ServiceAreaFeatClass = Common.Common.getFeatureClass(pWS, Common.Common.TableServiceArea);
                if (ServiceAreaFeatClass == null)
                {
                    //Try getting it from the map instead 
                    ServiceAreaFeatClass = getFeatureClassFromMaps(Common.Common.TableServiceArea, mapDoc);
                }

                //if it still couldn't be found notify the user
                if (ServiceAreaFeatClass == null)
                {
                    MapProductionHelper.Log("Unable to find the " + Common.Common.TableServiceArea + " feature class in the specified database or the mxd", MapProductionHelper.LogType.Info, null);
                    return null;
                }
            }

            //Get field indices
            if (regionFieldIndex == -1) { regionFieldIndex = ServiceAreaFeatClass.Fields.FindField(Common.Common.FieldRegion); }
            if (districtFieldIndex == -1) { districtFieldIndex = ServiceAreaFeatClass.Fields.FindField(Common.Common.FieldDistrict); }
            if (divisionFieldIndex == -1) { divisionFieldIndex = ServiceAreaFeatClass.Fields.FindField(Common.Common.FieldDivision); }

            //Query the division feature class for the polygon which intersects this feature.  May be more than one
            ISpatialFilter qf = new SpatialFilterClass();
            qf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            qf.Geometry = feature.ShapeCopy;
            qf.GeometryField = ServiceAreaFeatClass.ShapeFieldName;
            qf.SubFields = Common.Common.FieldRegion + "," + Common.Common.FieldDistrict + "," + Common.Common.FieldDivision;
            IFeatureCursor featCursor = ServiceAreaFeatClass.Search(qf, true);
            IFeature feat = featCursor.NextFeature();
            while (feat != null)
            {
                try
                {
                    string region = feat.get_Value(regionFieldIndex).ToString();
                    string division = feat.get_Value(divisionFieldIndex).ToString();
                    string district = feat.get_Value(districtFieldIndex).ToString();
                    locationInfo.Add(region + "," + division + "," + district);
                }
                catch (Exception e) { }

                feat = featCursor.NextFeature();
            }

            //Release our cursor
            if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }

            return locationInfo;

        }

        /// <summary>
        /// Checks if the map grid feature contains any features from the specified feature classes
        /// </summary>
        /// <param name="mapFeature"></param>
        /// <returns></returns>
        private bool HasData( 
            IFeature mapFeature, 
            string reqDataLayers, 
            MapProductionHelper pMapProdHelper)
        {
            MapProductionHelper.Log("Entering : HasData", MapProductionHelper.LogType.Debug, null);
            try
            {
                if (hasDataFeatureClasses == null)
                {
                    IDataset ds = mapFeature.Class as IDataset;
                    IWorkspace ws = ds.Workspace;
                    hasDataFeatureClasses = new List<IFeatureClass>();
                    string[] featClasses = Regex.Split(reqDataLayers, ",");

                    for (int i = 0; i < featClasses.Length; i++)
                    {
                        IFeatureClass featClass = Common.Common.getFeatureClass(ws, featClasses[i]);
                        if (featClass != null && !hasDataFeatureClasses.Contains(featClass))
                        {
                            hasDataFeatureClasses.Add(featClass);
                        }
                    }
                }

                foreach (IFeatureClass featClass in hasDataFeatureClasses)
                {
                    if (Common.Common.HasData(mapFeature, featClass)) { return true; }
                }

                return false;
            }
            catch (Exception ex)
            {
                MapProductionHelper.Log("Entering : HasData Error Handler", MapProductionHelper.LogType.Error, ex);
                return true; 
            }
        }

        /// <summary>
        /// Determines whether the memory threshold of the process has been reached.
        /// </summary>
        /// <returns></returns>
        private bool RestartDueToMemory(MapProductionHelper pMapProdHelper)
        {
            Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            long totalBytesOfMemoryUsed = currentProcess.PeakWorkingSet64;
            if (((totalBytesOfMemoryUsed / 1028) / 1000) > Common.Common.MemoryThreshhold)
            {
                //Memory allocation is too high.  Need to restart this process.
                MapProductionHelper.Log("Memory Usage: " + ((totalBytesOfMemoryUsed / 1028) / 1000) + "MB", MapProductionHelper.LogType.Info, null);
                MapProductionHelper.Log("Memory usage is higher than configured value.  Restarting process", MapProductionHelper.LogType.Info, null);
                return true;
            }
            else
            {
                //Log("Memory Usage: " + ((totalBytesOfMemoryUsed / 1028) / 1000) + "MB");
            }

            return false;
        }

        #endregion

      
    }
}
