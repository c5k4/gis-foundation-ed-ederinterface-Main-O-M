using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Display;
using System.Collections.Generic;
using System.IO;
using Ionic.Zip;
using System.Reflection;
using System.Linq;
using System.Timers;

namespace PGE.BatchApplication.DelineationPrintService
{
    public partial class DelineationPrintService : ServiceBase
    {
        #region Static Variables

        /* GDI delegate to SystemParametersInfo function */
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);
        [DllImport("User32.dll")]
        public static extern int GetDesktopWindow();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\logs\\";//Logfile_" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm") + ".log";

        #endregion Static Variables

        #region Constants

        /* constants used for user32 calls */
        const uint SPI_GETFONTSMOOTHING = 74;
        const uint SPI_SETFONTSMOOTHING = 75;
        const uint SPIF_UPDATEINIFILE = 0x1;

        #endregion Constants

        #region Enums

        private enum MapScale
        {
            Scale_50 = 50,
            Scale_100 = 100
        }

        private enum MapType
        {
            ET,
            ED
        }

        #endregion Enums

        #region Variables

        protected static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        string tempFolder = AppDomain.CurrentDomain.BaseDirectory + "temp\\";

        private Thread _ServiceThread;
        Int32 serviceInterval = 0;
        //Int32 _iTransmitWaitTime = 0;

        //private static InitLicenseManager m_AOLicenseInitializer = new InitLicenseManager();
        private string _wipSdeConnection = string.Empty;
        private string _ederSdeConnection = string.Empty;
        private string _delineationPrintFeatureClass = string.Empty;
        //private string _delineationMxdPath = string.Empty;
        private string _delineationPrintTemplatePath = string.Empty;
        //private string _delineationMxd50 = string.Empty;
        //private string _delineationMxd100 = string.Empty;


        //IMap map50 = null;
        //IMap map100 = null;

        //IMapDocument map50 = null;
        //IMapDocument map50 = null;

        //IMapDocument mapPoratrait50 = null;
        //IMapDocument mapLanscape50 = null;
        //IMapDocument mapPoratrait100 = null;
        //IMapDocument mapLanscape100 = null;

        //IFeatureWorkspace _ederFeatureWorkspace = default(IFeatureWorkspace);
        //IFeatureClass pFClassScaleBoundary = default(IFeatureClass);
        //ISpatialFilter spatialFilter = default(ISpatialFilter);
        //IFeatureCursor featureCursorBoundary = default(IFeatureCursor);

        string layerNameET_50 = string.Empty;
        string layerNameED_50 = string.Empty;
        string layerNameET_100 = string.Empty;
        string layerNameED_100 = string.Empty;

        #endregion Variables

        #region Constructor

        public DelineationPrintService()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();

            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }

            //Uncomment the below line - just for debug (Comment below line while compile/deployment)
            //StartService();
        }

        #endregion Constructor

        #region Protected Methods

        protected override void OnStart(string[] args)
        {
            try
            {
                EventLog.WriteEntry("DelineationPrint Service Started");
                _ServiceThread = new Thread(new ThreadStart(StartService));
                _ServiceThread.Start();
            }
            catch (Exception Ex)
            {
                Log.Error("Error: ", Ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                Log.Info("Stopping PGE Delineation Print Service");
                KillThreads();
                Utility.ShutdownLicense();

                _ServiceThread.Abort();

                Process proc = Process.GetCurrentProcess();
                proc.Close();
            }
            catch (Exception Ex)
            {
                Log.Error("Error: ", Ex);
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.DelineationPrint();
        }

        private void StartService()
        {
            try
            {
                LoadConfiguration();
                InitializeLicense();
                while (true)
                {
                   this.DelineationPrint();
                    Log.Info("Waiting interval : " + serviceInterval);
                   Thread.Sleep(serviceInterval);


                 
                }
            }
            catch (Exception Ex)
            {
                Log.Error("Error: ", Ex);
            }
        }
        private void DelineationPrint()
        {
            //DeleteTempFolder
            DeleteExistingTempFolder();
            IFeatureWorkspace workspace = default(IFeatureWorkspace);
            IQueryFilter queryFilterDelin = default(IQueryFilter);
            IQueryFilterDefinition2 queryFilterDef = default(IQueryFilterDefinition2);
            IFeatureCursor featureCursorDelin = default(IFeatureCursor);
            IFeature delPrntFeature = default(IFeature);

            string objectID = string.Empty;
            workspace = Utility.ConnectToSDE(_wipSdeConnection);
            IFeatureClass pFClass = workspace.OpenFeatureClass(_delineationPrintFeatureClass);
            try
            {
                //Delete old completed & error requests
                int requestValidityDays = Convert.ToInt32(ConfigurationManager.AppSettings["REQUEST_VALIDITY_DAYS"]);
                int processCount = Convert.ToInt32(ConfigurationManager.AppSettings["PROCESS_THREAD_COUNT"]);
                Log.Info("Deleting old requests.");
                DeleteFeatures(pFClass, " (STATUS = 'C' OR STATUS = 'E') AND TO_DATE(CREATION_DATE_TEXT, 'MM/dd/yyyy hh:mi:ss AM') < (SYSDATE - " + requestValidityDays + ") ");

                Log.Info("Deleting old log files.");
                var dir = new DirectoryInfo(sPath);
                FileInfo[] files = dir.GetFiles();
                var filesToDelete = files.Where(f => f.CreationTime < (DateTime.Now.AddDays(requestValidityDays * -1))).ToList();
                filesToDelete.ForEach(f => f.Delete());

                Log.Info("Starting Delineation Printing");
                Log.Info("Export Folder Path: " + tempFolder);
                queryFilterDelin = new QueryFilterClass();
                queryFilterDelin.WhereClause = " STATUS = 'N' AND ROWNUM <= " + processCount;

                string postfixClause = " ORDER BY OBJECTID ";
                queryFilterDef = (IQueryFilterDefinition2)queryFilterDelin;
                queryFilterDef.PostfixClause = postfixClause;

                featureCursorDelin = pFClass.Search(queryFilterDelin, false);
                delPrntFeature = null;
                if (System.IO.Directory.Exists(_delineationPrintTemplatePath))
                {
                    List<string> lstObjectIds = new List<string>();
                    while ((delPrntFeature = featureCursorDelin.NextFeature()) != null)
                    {
                        lstObjectIds.Add(delPrntFeature.OID.ToString());
                    }

                    if (lstObjectIds.Count > 0)
                    {
                        string objectIDs = String.Join(",", lstObjectIds.ToArray());

                        //Set STATUS as in progress
                        SetFieldValue(pFClass, "STATUS", "P", null, null, " OBJECTID IN (" + objectIDs + ") ");

                        foreach (string objId in lstObjectIds)
                        {
                            objectID = objId;
                            Thread thread = new Thread(() => DelineationPrintThread(objId));
                            thread.IsBackground = true;
                            thread.Name = string.Format("Thread: {0}", objectID);

                            _threads.Add(thread);
                            thread.Start();
                            Thread.Sleep(5000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(objectID))
                {
                    //Set STATUS as ERROR
                    SetFieldValue(pFClass, "STATUS", "E", null, "", " OBJECTID = " + objectID);
                }

                Log.Error("Error: ", ex);
            }
            finally
            {
             
                if (featureCursorDelin != null) Marshal.ReleaseComObject(featureCursorDelin);
               
                GC.Collect();
            }
        }

        private void DeleteExistingTempFolder()
        {
            try
            {
                int requestValidityDays = Convert.ToInt32(ConfigurationManager.AppSettings["REQUEST_VALIDITY_DAYS"]);
                //Directory.Delete(tempFolder,true);
                Log.Info("Deleting old temp folder files.");
                var dir = new DirectoryInfo(tempFolder);
                FileInfo[] files = dir.GetFiles();
                var filesToDelete = files.Where(f => f.CreationTime < (DateTime.Now.AddDays(requestValidityDays * -1))).ToList();
                try { filesToDelete.ForEach(f => f.Delete()); } catch { }

                foreach (DirectoryInfo subdir in dir.GetDirectories())
                {
                    if (subdir.CreationTime < (DateTime.Now.AddDays(requestValidityDays * -1)))
                    {
                        try { subdir.Delete(true); } catch { }
                    }
                    //FileInfo[] sfiles = subdir.GetFiles();
                    //var sfilesToDelete = sfiles.Where(f => f.CreationTime < (DateTime.Now.AddDays(requestValidityDays * -1))).ToList();
                    //try { sfilesToDelete.ForEach(f => f.Delete()); } catch { }

                }
            }
            catch
            { }
        }

        private IMapDocument OpenMXD(string mxdPath, StreamWriter pSWriter, string logPath)
        {
            IMapDocument mapDoc = new MapDocumentClass();
            if (mapDoc.IsPresent[mxdPath] && mapDoc.IsMapDocument[mxdPath] && !mapDoc.IsPasswordProtected[mxdPath])
            {
                Log.Info("Opening MXD: " + mxdPath);
                WriteLog(pSWriter, logPath, "Opening MXD: " + mxdPath);
                mapDoc.Open(mxdPath);
                return mapDoc;
            }
            return null;
        }

        private void SetFieldValue(IFeatureClass featureClass, string fieldName, object value, StreamWriter pSWriter, string logPath, string condition = "")
        {

            // Restrict the number of features to be updated.
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = condition;
            queryFilter.SubFields = fieldName + ",MODIFIED_DATE_TEXT,MACHINENAME";

            // Use IFeatureClass.Update to populate IFeatureCursor.
            IFeatureCursor updateCursor = featureClass.Update(queryFilter, false);

            int fieldIndex = featureClass.FindField(fieldName);
            int modifiedDateFieldIndex = featureClass.FindField("MODIFIED_DATE_TEXT");
            int machineNameFieldIndex = featureClass.FindField("MACHINENAME");
            IFeature feature = null;
            try
            {
                while ((feature = updateCursor.NextFeature()) != null)
                {
                    Log.Info("Setting field value: " + fieldName + "=" + value + " of OID: " + feature.OID);
                    if (pSWriter != null)
                        WriteLog(pSWriter, logPath, "Setting field value: " + fieldName + "=" + value + " of OID: " + feature.OID);

                    feature.set_Value(fieldIndex, value);

                    if (modifiedDateFieldIndex > -1)
                        feature.set_Value(modifiedDateFieldIndex, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt"));

                    if (fieldName.Equals("STATUS") && value.ToString().Equals("P"))
                    {
                        if (machineNameFieldIndex > -1)
                            feature.set_Value(machineNameFieldIndex, Environment.MachineName);
                    }

                    updateCursor.UpdateFeature(feature);
                }
            }
            catch (COMException comExc)
            {
                Log.Error("Error: ", comExc);
                if (pSWriter != null)
                    WriteLog(pSWriter, logPath, "Error: " + comExc.ToString());
                // Handle any errors that might occur on NextFeature().
            }
            finally
            {
                if (queryFilter != null) Marshal.FinalReleaseComObject(queryFilter);
                if (updateCursor != null) Marshal.FinalReleaseComObject(updateCursor);
                if (feature != null) Marshal.FinalReleaseComObject(feature);
                GC.Collect();
            }
        }

        private void DeleteFeatures(IFeatureClass featureClass, string condition)
        {
            // Restrict the number of features to be updated.
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = condition;

            // Use IFeatureClass.Update to populate IFeatureCursor.
            IFeatureCursor updateCursor = featureClass.Update(queryFilter, false);

            IFeature feature = null;
            try
            {
                while ((feature = updateCursor.NextFeature()) != null)
                {
                    updateCursor.DeleteFeature();
                }
            }
            catch (COMException comExc)
            {
                Log.Error("Error: ", comExc);
                //WriteLog(pSWriter,  logPath,"Error: " + comExc.ToString());
                // Handle any errors that might occur on NextFeature().
            }
            finally
            {
                if (queryFilter != null) Marshal.FinalReleaseComObject(queryFilter);
                if (updateCursor != null) Marshal.FinalReleaseComObject(updateCursor);
                if (feature != null) Marshal.FinalReleaseComObject(feature);
                GC.Collect();
            }
        }

        IList<Thread> _threads = new List<Thread>();

        //private void KillThread(int index)
        private void KillThreads()
        {
            //string id = string.Format("Thread: {0}", index);
            foreach (Thread thread in _threads)
            {
                //if (thread.Name == id)
                thread.Abort();
            }
        }

        private static void WriteLog(StreamWriter pSWriter, string logPath, string sMsg)
        {
            //try
            //{
            pSWriter = File.AppendText(logPath);
            pSWriter.WriteLine(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt") + " " + sMsg);
            pSWriter.Close();
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
        }

        private void DelineationPrintThread(string objectId)
        {
            IFeatureWorkspace _wipFeatureWorkspace = default(IFeatureWorkspace);
            IQueryFilter queryFilter = default(IQueryFilter);
            IFeatureCursor featureCursor = default(IFeatureCursor);
            IFeature delPrntFeature = default(IFeature);
            IMapDocument mapDoc = default(IMapDocument);

            string mxdTempPath = string.Empty;
            _wipFeatureWorkspace = Utility.ConnectToSDE(_wipSdeConnection);
            IFeatureClass pFClass = _wipFeatureWorkspace.OpenFeatureClass(_delineationPrintFeatureClass);
            string logPath = string.Empty;
            int objectID = 0;
            StreamWriter pSWriter = default(StreamWriter);
            try
            {

                //queryFilter = new QueryFilterClass();
                //queryFilter.WhereClause = " OBJECTID = " + objectId;
                //featureCursor = pFClass.Search(queryFilter, false);
                //delPrntFeature = null;
                delPrntFeature = pFClass.GetFeature(Convert.ToInt32(objectId));
                if (delPrntFeature != null)
                {
                    objectID = delPrntFeature.OID;

                    string lanID = delPrntFeature.get_Value(delPrntFeature.Fields.FindField("LANID")).ToString();
                    string template = delPrntFeature.get_Value(delPrntFeature.Fields.FindField("TEMPLATE")).ToString();
                    double scale = Convert.ToDouble(delPrntFeature.get_Value(delPrntFeature.Fields.FindField("SCALE")).ToString());
                    string projectName = delPrntFeature.get_Value(delPrntFeature.Fields.FindField("PROJECTNAME")).ToString();

                    logPath = sPath + lanID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + projectName + ".log";
                    pSWriter = default(StreamWriter);
                    (pSWriter = File.CreateText(logPath)).Close();

                    mapDoc = null;
                    //Get map scale based on the polygon shape
                    var maspScale = FindMapScale(delPrntFeature.Shape, pSWriter, logPath);
                    string mxdName = string.Empty;
                    if (maspScale.Equals(MapScale.Scale_50))
                    {
                        mxdName = template + "_50.mxd";
                        //mapDoc = OpenMXD(_delineationPrintTemplatePath + @"\" + template + "_50.mxd", pSWriter, logPath);
                    }
                    else if (maspScale.Equals(MapScale.Scale_100))
                    {
                        mxdName = template + "_100.mxd";
                        //mapDoc = OpenMXD(_delineationPrintTemplatePath + @"\" + template + "_100.mxd", pSWriter, logPath);
                    }

                    string mxdSourcePath = string.Empty;
                    string mxdDestinationPath = string.Empty;
                    mxdTempPath = tempFolder + @"\" + lanID + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + projectName;
                    mxdSourcePath = _delineationPrintTemplatePath + @"\" + mxdName;
                    mxdDestinationPath = mxdTempPath + @"\" + mxdName;

                    if (!string.IsNullOrEmpty(mxdTempPath) && !Directory.Exists(mxdTempPath))
                    {
                        Directory.CreateDirectory(mxdTempPath);
                    }
                    File.Copy(mxdSourcePath, mxdDestinationPath);
                    mapDoc = OpenMXD(mxdDestinationPath, pSWriter, logPath);

                    ZoomToShape(mapDoc, delPrntFeature.ShapeCopy, pSWriter, logPath);

                    #region Set Map Scale

                    Log.Info("Setting map scale to " + scale);
                    WriteLog(pSWriter, logPath, "Setting map scale to " + scale);
                    mapDoc.Map[0].MapScale = scale;
                    mapDoc.ActiveView.Refresh();

                    #endregion Set Map Scale

                    string pdfPathET = ClipAndGeneratePDF(mapDoc, delPrntFeature.ShapeCopy, maspScale, scale, MapType.ET, lanID, template, projectName, pSWriter, logPath); //Clip and generate PDF for ET
                    string pdfPathED = ClipAndGeneratePDF(mapDoc, delPrntFeature.ShapeCopy, maspScale, scale, MapType.ED, lanID, template, projectName, pSWriter, logPath); //Clip and generate PDF for ED

                    if (!string.IsNullOrEmpty(pdfPathET) && !string.IsNullOrEmpty(pdfPathED))
                    {
                        #region Zip PDF Files

                        string zipFileName = tempFolder + projectName + "_" + lanID.ToUpper() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".zip";
                        Log.Info("Zipping PDFs to " + zipFileName);
                        WriteLog(pSWriter, logPath, "Zipping PDFs to " + zipFileName);
                        using (ZipFile zip = new ZipFile())
                        {
                            zip.AddFile(pdfPathET, "");
                            zip.AddFile(pdfPathED, "");
                            zip.Save(zipFileName);
                        }
                        File.Delete(pdfPathET);
                        File.Delete(pdfPathED);

                        #endregion Zip PDF Files

                        #region Send Email

                        SendEmail(lanID, zipFileName, projectName, pSWriter, logPath);
                        File.Delete(zipFileName);

                        #endregion Send Email

                        //Set STATUS as completed
                        SetFieldValue(pFClass, "STATUS", "C", pSWriter, logPath, " OBJECTID = " + delPrntFeature.OID);
                    }
                    else
                    {
                        //Set STATUS as error
                        SetFieldValue(pFClass, "STATUS", "E", pSWriter, logPath, " OBJECTID = " + delPrntFeature.OID);
                    }

                  
                }
                if (!string.IsNullOrEmpty(mxdTempPath) && Directory.Exists(mxdTempPath))
                {
                    try
                    {
                        Directory.Delete(mxdTempPath, true);
                    }
                    catch
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                if (objectID > 0)
                {
                    //Set STATUS as ERROR
                    SetFieldValue(pFClass, "STATUS", "E", pSWriter, logPath, " OBJECTID = " + objectID);
                }

                Log.Error("Error: ", ex);
                WriteLog(pSWriter, logPath, "Error: " + ex.ToString());
            }
            finally
            {
                
             
             
              
                GC.Collect();
            }
        }

      
        private void SendEmail(string lanID, string zipFileName, string projectName, StreamWriter pSWriter, string logPath)
        {
            //try
            //{
            Log.Info("Sending email to " + lanID);
            WriteLog(pSWriter, logPath, "Sending email to " + lanID);
            string fromDisplayName = ConfigurationManager.AppSettings["MAIL_FROM_DISPLAY_NAME"];
            string fromAddress = ConfigurationManager.AppSettings["MAIL_FROM_ADDRESS"];
            string subject = ConfigurationManager.AppSettings["MAIL_SUBJECT"] + ": " + projectName;
            string bodyText = ConfigurationManager.AppSettings["MAIL_BODY"];

            EmailService.Send(mail =>
            {
                mail.From = fromAddress;
                mail.FromDisplayName = fromDisplayName;
                mail.To = lanID + "@pge.com;";
                mail.Subject = subject;
                mail.BodyText = bodyText;
                mail.Attachments.Add(zipFileName);
            });
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("Error: ", ex);
            //}
        }

        private IEnumerable<IMap> GetMaps(IMapDocument pMapDocument)
        {
            int N = pMapDocument.MapCount;
            for (int i = 0; i < N; i++)
                yield return pMapDocument.get_Map(i);
        }

        private string ClipAndGeneratePDF(IMapDocument mapDoc, IGeometry shape, MapScale maspScale, double scale, MapType type, string lanID, string template, string projectName, StreamWriter pSWriter, string logPath)
        {
            //try
            //{
            #region On/Off ET & ED layers based on scale

            bool setVisiblityET = false;
            bool setVisiblityED = false;

            Log.Info("Setting ET/ED layer visibility");
            WriteLog(pSWriter, logPath, "Setting ET/ED layer visibility");
            IEnumLayer enumLayers = mapDoc.Map[0].Layers;
            //IEnumLayer enumLayers = templateDoc.Map[0].Layers;
            enumLayers.Reset();
            if (maspScale.Equals(MapScale.Scale_50))
            {
                ILayer layer = null;
                while ((layer = enumLayers.Next()) != null)
                {
                    if (type.Equals(MapType.ET))
                    {
                        if (layer.Name.Equals(layerNameET_50))//ET Layer
                        {
                            layer.Visible = true;
                            setVisiblityET = true;
                        }
                        else if (layer.Name.Equals(layerNameED_50))//ED Layer
                        {
                            layer.Visible = false;
                            setVisiblityED = true;
                        }
                    }
                    else if (type.Equals(MapType.ED))
                    {
                        if (layer.Name.Equals(layerNameET_50))//ET Layer
                        {
                            layer.Visible = false;
                            setVisiblityET = true;
                        }
                        else if (layer.Name.Equals(layerNameED_50))//ED Layer
                        {
                            layer.Visible = true;
                            setVisiblityED = true;
                        }
                    }

                    if (setVisiblityED && setVisiblityET)
                        break;
                }
            }
            else if (maspScale.Equals(MapScale.Scale_100))
            {
                ILayer layer = null;
                while ((layer = enumLayers.Next()) != null)
                {
                    if (type.Equals(MapType.ET))
                    {
                        if (layer.Name.Equals(layerNameET_100))//ET Layer
                        {
                            layer.Visible = true;
                            setVisiblityET = true;
                        }
                        else if (layer.Name.Equals(layerNameED_100))//ED Layer
                        {
                            layer.Visible = false;
                            setVisiblityED = true;
                        }
                    }
                    else if (type.Equals(MapType.ED))
                    {
                        if (layer.Name.Equals(layerNameET_100))//ET Layer
                        {
                            layer.Visible = false;
                            setVisiblityET = true;
                        }
                        else if (layer.Name.Equals(layerNameED_100))//ED Layer
                        {
                            layer.Visible = true;
                            setVisiblityED = true;
                        }
                    }

                    if (setVisiblityED && setVisiblityET)
                        break;
                }
            }

            #endregion On/Off ET & ED layers based on scale

            #region Set layer visibility based on the scale
            //try
            //{
            //    // Set layer visibility based on the scale
            //    enumLayers = mapDoc.Map[0].Layers;
            //    enumLayers.Reset();
            //    ILayer lyr = null;
            //    while ((lyr = enumLayers.Next()) != null)
            //    {
            //        if (lyr.Visible)
            //        {
            //            if (lyr.MinimumScale > scale && scale > lyr.MaximumScale)
            //            {
            //                lyr.Visible = true;
            //            }
            //            else
            //            {
            //                lyr.Visible = false;
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            #endregion Set layer visibility based on the scale

            #region Clip the map based on polygon shape

            Log.Info("Clipping the map based on the geometry");
            WriteLog(pSWriter, logPath, "Clipping the map based on the geometry");
            IBorder border = new SymbolBorder();
            mapDoc.Map[0].ClipGeometry = shape;
            mapDoc.Map[0].ClipBorder = border;
            //templateDoc.ActiveView.Activate(GetDesktopWindow());
            mapDoc.ActiveView.Refresh();

            #endregion Clip the map based on polygon shape

            #region Export map to PDF

            string fileName = string.Empty;
            if (type.Equals(MapType.ET))
            {
                fileName += projectName + "_Transmission_" + lanID.ToUpper() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else if (type.Equals(MapType.ED))
            {
                fileName += projectName + "_Distribution_" + lanID.ToUpper() + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

            //PDF|EPS|AI|BMP|TIFF|SVG|PNG|GIF|EMF|JPEG
            string outputFormat = "PDF";// ConfigurationManager.AppSettings["OUTPUT_FORMAT"]; 

            //string exportFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\DelineationPrint\\";
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            string filePath = ExportMap(mapDoc.ActiveView, 300, 1, outputFormat, tempFolder + "\\", fileName, false, pSWriter, logPath);

            return filePath;

            #endregion Export map to PDF
            //}
            //catch (Exception ex)
            //{
            //    Log.Error("Error: ", ex);
            //    return null;
            //}
        }

        private MapScale FindMapScale(IGeometry shape, StreamWriter pSWriter, string logPath)
        {
            IFeatureWorkspace _ederFeatureWorkspace = default(IFeatureWorkspace);
            IFeatureClass pFClassScaleBoundary = default(IFeatureClass);
            ISpatialFilter spatialFilter = default(ISpatialFilter);
            IFeatureCursor featureCursorBoundary = default(IFeatureCursor);

            try
            {
                string featureClassName = "EDGIS.PGE_50ScaleBoundary";
                Log.Info("Finding map scale based on featureclass: " + featureClassName);
                WriteLog(pSWriter, logPath, "Finding map scale based on featureclass: " + featureClassName);
                _ederFeatureWorkspace = Utility.ConnectToSDE(_ederSdeConnection);
                pFClassScaleBoundary = _ederFeatureWorkspace.OpenFeatureClass(featureClassName);
                spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = shape;
                spatialFilter.GeometryField = pFClassScaleBoundary.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialFilter.SubFields = "OBJECTID";

                featureCursorBoundary = pFClassScaleBoundary.Search(spatialFilter, false);
                if ((featureCursorBoundary.NextFeature()) != null)
                {
                    return MapScale.Scale_50;
                }
                if (featureCursorBoundary != null) Marshal.ReleaseComObject(featureCursorBoundary);
            }
            catch (Exception ex)
            {
                Log.Error("Error: ", ex);
                WriteLog(pSWriter, logPath, "Error: " + ex.ToString());
            }
            finally
            {
                //if (_ederFeatureWorkspace != null) Marshal.FinalReleaseComObject(_ederFeatureWorkspace);
                //if (pFClassScaleBoundary != null) Marshal.FinalReleaseComObject(pFClassScaleBoundary);
                //if (spatialFilter != null) Marshal.FinalReleaseComObject(spatialFilter);
                //if (featureCursorBoundary != null) Marshal.FinalReleaseComObject(featureCursorBoundary);
                GC.Collect();
            }
            return MapScale.Scale_100;
        }

        private void InitializeLicense()
        {
            try
            {
                Log.Info("Starting PGE Delineation Print Service");
                if (Utility.InitializeLicense() == true)
                {
                    Log.Info("License initiated successfully");
                    //_wipFeatureWorkspace = Utility.ConnectToSDE(_wipSdeConnection);
                    //if (_wipFeatureWorkspace == null)
                    //{
                    //    Utility.ShutdownLicense();
                    //    Log.Info("Terminating PGE Delineation Print Service.");
                    //    Environment.Exit(1);
                    //}
                }
                else
                {
                    Log.Info("Unable to contact license server, please verify license server is available, stopping service");
                    //_ServiceThread.Abort();
                    Utility.ShutdownLicense();
                    Process proc = Process.GetCurrentProcess();
                    proc.Close();
                    Environment.Exit(0);
                }
            }
            catch (Exception Ex)
            {
                OnStop();
                Log.Error("Error: ", Ex);
            }
        }

        private void SetMapScale(IMapDocument mapDoc, double scale)
        {
            IMapDocument doc = mapDoc;
            IMap map = (IMap)doc.Map[0];
            IActiveView pActiveView = (IActiveView)map;

            map.MapScale = scale;
            pActiveView.Activate(GetDesktopWindow());
            pActiveView.Refresh();
        }

        private void ZoomToShape(IMapDocument mapDoc, IGeometry geometry, StreamWriter pSWriter, string logPath)
        {
            Log.Info("Zooming to the geometry shape");
            WriteLog(pSWriter, logPath, "Zooming to the geometry shape");
            IMapDocument doc = mapDoc;
            IMap map = (IMap)doc.Map[0];
            IActiveView pActiveView = (IActiveView)map;

            IEnvelope pEnvelope;
            if (geometry is IPoint)
            {
                IEnvelope currentEnv = pActiveView.Extent;
                IPoint point = (IPoint)geometry;
                currentEnv.CenterAt(point);
                pActiveView.Extent = currentEnv;
                map.MapScale = 100; //to set the scale to 1:100         
            }
            else
            {
                pEnvelope = geometry.Envelope;
                pEnvelope.Expand(1, 1, true);

                pActiveView.Extent = pEnvelope;
            }

            pActiveView.Refresh();
        }

        private void LoadConfiguration()
        {
            _wipSdeConnection = PGE_DBPasswordManagement.ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["WIP_SDE_CONNECTION"]);
            if (String.IsNullOrEmpty(_wipSdeConnection))
            {
                Log.Error("Unable to get sde connection file from config file. Exiting (WIP_SDE_CONNECTION)");
                OnStop();
                //Environment.Exit(1);
            }

            _ederSdeConnection = PGE_DBPasswordManagement.ReadEncryption.GetSDEPath(ConfigurationManager.AppSettings["EDER_SDE_CONNECTION"]);
            if (String.IsNullOrEmpty(_ederSdeConnection))
            {
                Log.Error("Unable to get sde connection file from config file. Exiting (EDER_SDE_CONNECTION)");
                OnStop();
                //Environment.Exit(1);
            }

            _delineationPrintFeatureClass = ConfigurationManager.AppSettings["DELINEATION_PRINT_FEATURE_CLASS"];
            if (String.IsNullOrEmpty(_delineationPrintFeatureClass))
            {
                Log.Error("Unable to get delineation print feature class. Exiting (DELINEATION_PRINT_FEATURE_CLASS)");
                OnStop();
                //Environment.Exit(1);
            }

            //_delineationMxdPath = ConfigurationManager.AppSettings["DELINEATION_MXD_PATH"];
            //if (String.IsNullOrEmpty(_delineationMxdPath))
            //{
            //    Log.Error("Unable to get delineation MXD path. Exiting (DELINEATION_MXD_PATH)");
            //    OnStop();
            //    //Environment.Exit(1);
            //}

            _delineationPrintTemplatePath = ConfigurationManager.AppSettings["DELINEATION_PRINT_TEMPLATE_PATH"];
            if (String.IsNullOrEmpty(_delineationPrintTemplatePath))
            {
                Log.Error("Unable to get delineation print template path. Exiting (DELINEATION_PRINT_TEMPLATE_PATH)");
                OnStop();
                //Environment.Exit(1);
            }
            serviceInterval = Convert.ToInt32(ConfigurationManager.AppSettings["SERVICE_INTERVAL"]) * 60 * 1000;

            //_iTransmitWaitTime = Convert.ToInt32(ConfigurationManager.AppSettings["TRANSMIT_WAIT_TIME"]);
            //_delineationMxd50 = ConfigurationManager.AppSettings["DELINEATION_MXD_50"];
            //_delineationMxd100 = ConfigurationManager.AppSettings["DELINEATION_MXD_100"];

            layerNameET_50 = ConfigurationManager.AppSettings["LAYER_NAME_ET_50"];
            layerNameED_50 = ConfigurationManager.AppSettings["LAYER_NAME_ED_50"];
            layerNameET_100 = ConfigurationManager.AppSettings["LAYER_NAME_ET_100"];
            layerNameED_100 = ConfigurationManager.AppSettings["LAYER_NAME_ED_100"];
        }

        private string ExportMap(IActiveView activeView, long outputResolution, long resampleRatio, string exportType, string outputDir, string fileName, Boolean clipToGraphicsExtent, StreamWriter pSWriter, string logPath)
        {
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


            Log.Info("Exporting the map to " + exportType);
            WriteLog(pSWriter, logPath, "Exporting the map to " + exportType);
             
            IExport docExport = default(IExport);
            IPrintAndExport docPrintExport = default(IPrintAndExport);
            IOutputRasterSettings RasterSettings = default(IOutputRasterSettings);
            bool bReenable = false;

            try
            {
               
                if (exportType == "PDF")
                {
                    docExport = new ExportPDFClass();
                }
                else if (exportType == "EPS")
                {
                    docExport = new ExportPSClass();
                }
                else if (exportType == "AI")
                {
                    docExport = new ExportAIClass();
                }
                else if (exportType == "BMP")
                {
                    docExport = new ExportBMPClass();
                }
                else if (exportType == "TIFF")
                {
                    docExport = new ExportTIFFClass();
                }
                else if (exportType == "SVG")
                {
                    docExport = new ExportSVGClass();
                }
                else if (exportType == "PNG")
                {
                    docExport = new ExportPNGClass();
                }
                else if (exportType == "GIF")
                {
                    docExport = new ExportGIFClass();
                }
                else if (exportType == "EMF")
                {
                    docExport = new ExportEMFClass();
                }
                else if (exportType == "JPEG")
                {
                    docExport = new ExportJPEGClass();
                }
                else
                {
                    Log.Error("Unsupported export type " + exportType + ", defaulting to EMF.");
                    WriteLog(pSWriter, logPath, "Unsupported export type " + exportType + ", defaulting to EMF.");
                    exportType = "EMF";
                    docExport = new ExportEMFClass();
                }
                //Log.Info("test: 2 ");

                docPrintExport = new PrintAndExportClass();

              

                //set the export filename (which is the nameroot + the appropriate file extension)
                string exportFileName = outputDir + fileName + "." + docExport.Filter.Split('.')[1].Split('|')[0].Split(')')[0];
                docExport.ExportFileName = exportFileName;
                //Log.Info("File Name: " + exportFileName);
              // check if export is vector or raster
                if (docExport is IOutputRasterSettings)
                {
                    // for vector formats, assign the desired ResampleRatio to control drawing of raster layers at export time   
                    RasterSettings = (IOutputRasterSettings)docExport;
                    RasterSettings.ResampleRatio = (int)resampleRatio;

                    // NOTE: for raster formats output quality of the DISPLAY is set to 1 for image export 
                    // formats by default which is what should be used
                }

                Log.Info("Exporting the map: " + exportFileName);
                WriteLog(pSWriter, logPath, "Exporting the map: " + exportFileName);

                docPrintExport.Export(activeView, docExport, outputResolution, clipToGraphicsExtent, null);

                Log.Info("Finished exporting. ");
                WriteLog(pSWriter, logPath, "Finished exporting. ");

                if (bReenable)
                {
                    /* reenable font smoothing if we disabled it before */
                    EnableFontSmoothing();
                    bReenable = false;
                    if (!GetFontSmoothing())
                    {
                        //error: cannot reenable font smoothing.
                        Log.Error("Font Smoothing error: " + "Unable to reenable Font Smoothing.");
                        WriteLog(pSWriter, logPath, "Font Smoothing error: " + "Unable to reenable Font Smoothing.");
                    }
                }
                //Log.Info("test: 4 ");
                return exportFileName;
            }
            catch (Exception ex)
            {
                Log.Error("Error: ", ex);
                if (pSWriter != null)
                    WriteLog(pSWriter, logPath, "Error: " + ex.ToString());
            }
            finally
            {
                
                //if (docExport != null) Marshal.FinalReleaseComObject(docExport);
                //if (docPrintExport != null) Marshal.FinalReleaseComObject(docPrintExport);
                //if (RasterSettings != null) Marshal.FinalReleaseComObject(RasterSettings);
                GC.Collect();
            }
            return "";
        }

        private void DisableFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            /* call to systemparametersinfo to set the font smoothing value */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 0, ref pv, SPIF_UPDATEINIFILE);
        }

        private void EnableFontSmoothing()
        {
            bool iResult;
            int pv = 0;

            /* call to systemparametersinfo to set the font smoothing value */
            iResult = SystemParametersInfo(SPI_SETFONTSMOOTHING, 1, ref pv, SPIF_UPDATEINIFILE);

        }

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

        #endregion Private Methods
    }
}
