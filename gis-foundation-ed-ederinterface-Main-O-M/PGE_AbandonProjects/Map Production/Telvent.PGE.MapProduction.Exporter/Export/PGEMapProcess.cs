using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geoprocessing;
using Telvent.Delivery.Systems.Configuration;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Display;
using Telvent.Delivery.Framework;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessor;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace Telvent.PGE.MapProduction.Export
{
    /// <summary>
    /// This class had methods to export the Map to PDF and TIFF formats
    /// </summary>
    public class PGEMapProcess
    {
        public static bool LoadTiffMxd = true;
        public static bool LoadPDFMxd = true;
        private static IMapDocument PDFMapDocument = null;
        private static IMapDocument TIFFMapDocument = null;

        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\MapProduction1.0.log4net.config", "MapProduction");

        /* GDI delegate to SystemParametersInfo function */
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref int pvParam, uint fWinIni);

        /* constants used for user32 calls */
        private const uint SPI_GETFONTSMOOTHING = 74;
        private const uint SPI_SETFONTSMOOTHING = 75;
        private const uint SPIF_UPDATEINIFILE = 0x1;

        #region Public Static Extern method
        /// <summary>
        /// Release the window graphcis handle
        /// </summary>
        /// <param name="handle">Graphics handle to be released</param>
        /// <returns>Returns if the handle is release. False, otherwise</returns>
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);
        #endregion Public Static Extern method

        #region Public Properties

        public string XMin
        {
            get;
            set;
        }
        public string YMin
        {
            get;
            set;
        }
        public string XMax
        {
            get;
            set;
        }
        public string YMax
        {
            get;
            set;
        }
        public string PdfMxdFileName
        {
            get;
            set;
        }
        public string TiffMxdFileName
        {
            get;
            set;
        }
        public string MapNumber
        {
            get;
            set;
        }
        public string TiffOutputfileLocation
        {
            get;
            set;
        }
        public string PdfOutputfileLocation
        {
            get;
            set;
        }
        public string MxdLocation
        {
            get;
            set;
        }
        public string MapScale
        {
            get;
            set;
        }
        public bool IncludeAttributesforPDF
        {
            get;
            set;
        }
        public string OutputFileName
        {
            get;
            set;
        }
        public int TiffDPI
        {
            get;
            set;
        }
        public int Tiff500ScaleDPI
        {
            get;
            set;
        }
        public int PdfDPI
        {
            get;
            set;
        }

        public string MapOfficeCode
        {
            get;
            set;
        }

        public string WKTPolygon
        {
            get;
            set;
        }
        #endregion Public Properties

        #region Private Properties
        ///// <summary>
        ///// Map Document either new or already opened
        ///// </summary>
        //private IMapDocument MapDocument { get; set; }
        ///// <summary>
        ///// Map's Data View
        ///// </summary>
        //private IActiveView MapDataView { get; set; }
        ///// <summary>
        ///// Export Frame rectangle attached the Layout view
        ///// </summary>
        //private tagRECT ExportFrameRect { get; set; }
        ///// <summary>
        ///// Device Envelope used for exporting
        ///// </summary>
        //private IEnvelope PixelBounds { get; set; }
        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Executes the Export to PDF operation
        /// </summary>
        /// <returns>Returns list of errors, if any, while executing the operation</returns>
        public List<string> ExportPDF()
        {
            string tempDirectory = System.IO.Path.GetTempPath();
            tempDirectory += this.MapOfficeCode;
            List<string> errors = new List<string>();
            
            ExportPDFClass exportMap = null;
            try
            {
                //Validate
                if (IsInvalid())
                {
                    errors.Add("One or more required parameters missing for the PDF export operation for Map Number = " + this.MapNumber);
                    return errors;
                }
                //Open Map Document
                //Get the Right MapDocument based on size
                IEnvelope env = new EnvelopeClass();
                env.PutCoords(double.Parse(XMin), double.Parse(YMin), double.Parse(XMax), double.Parse(YMax));
                string mxdPath = System.IO.Path.Combine(this.MxdLocation, this.PdfMxdFileName);
                if (MapProductionConfigurationHandler.GetSettingValue("UseLandscape").ToUpper() == "TRUE")
                {
                    if (env.Height < env.Width)
                    {
                        mxdPath = System.IO.Path.Combine(this.MxdLocation, "LANDSCAPE_" + this.PdfMxdFileName);
                    }
                }

                
                if (PDFMapDocument != null && LoadPDFMxd)
                {
                    if (PDFMapDocument != null)
                    {
                        PDFMapDocument.Close();
                        while (Marshal.ReleaseComObject(PDFMapDocument) > 0) { }
                        PDFMapDocument = null;
                    }
                }
                if (LoadPDFMxd)
                {
                    PDFMapDocument = new MapDocumentClass();
                    _logger.Debug("Opening map document : " + mxdPath);
                    PDFMapDocument.Open(mxdPath);
                    LoadPDFMxd = false;
                }

                try
                {
                    //Check if temp directory exists.  If not create it.
                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("Unable to create temporary directory: " + tempDirectory);
                    throw e;
                }

                //If old temp pdf exists let's delete it
                try
                {
                    if (File.Exists(tempDirectory + "\\" + this.MapNumber + ".pdf"))
                    {
                        File.Delete(tempDirectory + "\\" + this.MapNumber + ".pdf");
                    }
                }
                catch { }

                IMap map = SetExtentsAndGetIMapToPlot(PDFMapDocument, env);
                //MapProductionFacade.PGEMap = mapDocument.Map[0];
                MapProductionFacade.PGEMap = map;
                MapProductionFacade.PGEPageLayout = PDFMapDocument.PageLayout;
                //Set Extent of the Map Data view
                //IActiveView mapDataView = mapDocument.Map[0] as IActiveView;
                IActiveView mapDataView = map as IActiveView;
                _logger.Debug("Zoomed to extent");
                _logger.Debug("Setting Map Scale");
                if (string.IsNullOrEmpty(this.MapScale.Trim()))
                {
                    _logger.Debug("Map Scale can not be set as the value of scale not found.");
                }
                else
                {
                    //SetMapScale(double.Parse(this.MapScale), ref mapDataView, mapDocument.PageLayout);
                }
                //mapDataView.Refresh();

                //Check if the document has the pge hash auto text.
                //if found then set the map number.
                _logger.Debug("Setting MapGrid Number to:"+this.MapNumber+":"+this.MapOfficeCode);
                IElement ateElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGEHashBufferATECaption);
                if (ateElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(ateElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGEHashBufferATECaption + " not found in the document " + PdfMxdFileName); }

                IElement xminATEElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGEXMinCaption);
                if (xminATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(xminATEElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGEXMinCaption + " not found in the document " + PdfMxdFileName); }

                IElement yminATEElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGEYMinCaption);
                if (yminATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(yminATEElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGEYMinCaption + " not found in the document " + PdfMxdFileName); }

                IElement xmaxATEElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGEXMaxCaption);
                if (xmaxATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(xmaxATEElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGEXMaxCaption + " not found in the document " + PdfMxdFileName); }

                IElement ymaxATEElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGEYMaxCaption);
                if (ymaxATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(ymaxATEElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGEYMaxCaption + " not found in the document " + PdfMxdFileName); }

                IElement countyATEElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGECountyCaption);
                if (countyATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(countyATEElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGECountyCaption + " not found in the document " + PdfMxdFileName); }

                IElement divisionATEElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGEDivisionCaption);
                if (divisionATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(divisionATEElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGEDivisionCaption + " not found in the document " + PdfMxdFileName); }

                IElement zipATEElement = MapProductionFacade.GetArcFMAutoTextElement(PDFMapDocument.PageLayout, MapProductionFacade.PGEZipCaption);
                if (divisionATEElement != null)
                {
                    MapProductionFacade.SetMapGridNumberFromATE(zipATEElement, this.MapNumber + ":" + this.MapOfficeCode);
                }
                else { _logger.Debug("ATE Element " + MapProductionFacade.PGEZipCaption + " not found in the document " + PdfMxdFileName); }

                //Store the Prmary key of the current Map grid to be plotted
                MapProductionFacade.CurrentMapGridPrimaryKey = this.MapNumber + ":" + this.MapOfficeCode;

                _logger.Debug("Preparing Pixel bounds");
                //Prepare PixelBounds
                IActiveView pageLayoutView = PDFMapDocument.PageLayout as IActiveView;
                pageLayoutView.Extent = pageLayoutView.FullExtent;
                pageLayoutView.Refresh();
                //tagRECT exportFrameRect = pageLayoutView.ExportFrame;
                tagRECT exportFrameRect = new tagRECT();
                exportFrameRect.left = pageLayoutView.ExportFrame.left;
                exportFrameRect.top = pageLayoutView.ExportFrame.top;
                exportFrameRect.right = pageLayoutView.ExportFrame.right * (this.PdfDPI / 96);
                exportFrameRect.bottom = pageLayoutView.ExportFrame.bottom * (this.PdfDPI / 96);
                IEnvelope pixelBounds = new EnvelopeClass();
                pixelBounds.PutCoords(exportFrameRect.left, exportFrameRect.bottom, exportFrameRect.right, exportFrameRect.top);

                _logger.Debug("Initializing PDF Exporter");
                //Initialize exporter class
                exportMap = new ExportPDFClass();
                //Set the required input parameters
                exportMap.ExportFileName = System.IO.Path.Combine(tempDirectory, this.MapNumber + ".pdf");
                _logger.Debug("Temporary Export File Name:" + exportMap.ExportFileName); 
                exportMap.Resolution = this.PdfDPI;
                exportMap.PixelBounds = pixelBounds;

                _logger.Debug("Set the Export attribute Property");
                //Set the Export attribute Property
                IExportPDF2 exportPDF = exportMap as IExportPDF2;
                exportPDF.ExportPDFLayersAndFeatureAttributes = this.IncludeAttributesforPDF ?
                    esriExportPDFLayerOptions.esriExportPDFLayerOptionsLayersAndFeatureAttributes :
                    esriExportPDFLayerOptions.esriExportPDFLayerOptionsLayersOnly;

                exportMap.Compressed = true;
                exportMap.ImageCompression = esriExportImageCompression.esriExportImageCompressionDeflate;
                exportMap.EmbedFonts = true;
                //exportMap.JPEGCompressionQuality = 100; 
                exportMap.ResampleRatio = 1;

                _logger.Debug("Starting the export");
                //Start the export process
                int hDc = exportMap.StartExporting();
                tagRECT acViewRect = exportFrameRect;

                pageLayoutView = PDFMapDocument.PageLayout as IActiveView;

                pageLayoutView.Output(hDc, this.PdfDPI, ref acViewRect, null, null);
                exportMap.FinishExporting();
                _logger.Debug("Finished exporting");

                _logger.Debug("Copying temporary pdf file to final location");
                MovePDFToFinalLocation(tempDirectory, this.PdfOutputfileLocation);

                _logger.Debug("Final Export File Name:" + this.PdfOutputfileLocation + "\\" + this.MapNumber + ".pdf");

                _logger.Debug("Cleaning up resources");
                //Clean any in-memory resources
                exportMap.Cleanup();
                while (Marshal.ReleaseComObject(exportMap) > 0) { };
                //Release window graphics handle
                CloseHandle(new IntPtr(hDc));

                if (ateElement != null)
                    while ((Marshal.ReleaseComObject(ateElement)) > 0) { }
                if (xminATEElement != null)
                    while ((Marshal.ReleaseComObject(xminATEElement)) > 0) { }
                if (xmaxATEElement != null)
                    while ((Marshal.ReleaseComObject(xmaxATEElement)) > 0) { }
                if (yminATEElement != null)
                    while ((Marshal.ReleaseComObject(yminATEElement)) > 0) { }
                if (ymaxATEElement != null)
                    while ((Marshal.ReleaseComObject(ymaxATEElement)) > 0) { }
                if (countyATEElement != null)
                    while ((Marshal.ReleaseComObject(countyATEElement)) > 0) { }
                if (divisionATEElement != null)
                    while ((Marshal.ReleaseComObject(divisionATEElement)) > 0) { }
                if (zipATEElement != null)
                    while ((Marshal.ReleaseComObject(zipATEElement)) > 0) { }

                //Release the references to the current map grid being plotted
                MapProductionFacade.CurrentMapGridPrimaryKey = null;
                if (MapProductionFacade.CurrentMapGridFeature != null)
                {
                    while (Marshal.ReleaseComObject(MapProductionFacade.CurrentMapGridFeature) > 0) { }
                    MapProductionFacade.CurrentMapGridFeature = null;
                }
                //Release the resources
                Marshal.ReleaseComObject(env);
                Marshal.ReleaseComObject(pixelBounds);

            }
            catch (Exception ex)
            {
                errors.Add(ex.Message + Environment.NewLine + ex.StackTrace);
                _logger.Debug("Error processing map :" + this.MapNumber, ex);
            }
            finally
            {
                if (exportMap != null)
                {
                    while (Marshal.ReleaseComObject(exportMap) > 0) { }
                }
                if (MapProductionFacade.PGEMap != null)
                {
                    while (Marshal.ReleaseComObject(MapProductionFacade.PGEMap) > 0) { }
                }
                if (MapProductionFacade.PGEPageLayout != null)
                {
                    while (Marshal.ReleaseComObject(MapProductionFacade.PGEPageLayout) > 0) { }
                }
                GC.Collect();
            }
            return errors;
        }

        /// <summary>
        /// Executes the Export to TIFF operation
        /// </summary>
        /// <returns>Returns list of errors, if any, while executing the operation</returns>
        public List<string> ExportTiff(int processID)
        {
            string tempDirectory = System.IO.Path.GetTempPath();
            tempDirectory += this.MapOfficeCode;
            double XMaxOrig = double.Parse(XMax);
            double XMinOrig = double.Parse(XMin);
            double YMaxOrig = double.Parse(YMax);
            double YMinOrig = double.Parse(YMin);
            IExport exportMap = null;
            tagRECT exportFrameRect = new tagRECT();
            List<string> errors = new List<string>();
            try
            {
                //Validate
                if (IsInvalid())
                {
                    errors.Add("One or more required parameters missing for the TIFF export operation for Map Number = " + this.MapNumber);
                    return errors;
                }
                //Open Map Document
                string mxdPath = System.IO.Path.Combine(this.MxdLocation, this.TiffMxdFileName);

                if (LoadTiffMxd)
                {
                    if (TIFFMapDocument != null)
                    {
                        _logger.Debug("Closing document:" + TIFFMapDocument.DocumentFilename);
                        TIFFMapDocument.Close();
                        while (Marshal.ReleaseComObject(TIFFMapDocument) > 0) { }
                        TIFFMapDocument = null;
                    }
                    _logger.Debug("Opening Tiff MXD:" + mxdPath);
                    TIFFMapDocument = new MapDocumentClass();
                    TIFFMapDocument.Open(mxdPath);
                    LoadTiffMxd = false;
                }

                try
                {
                    //Check if temp directory exists.  If not create it.
                    if (!Directory.Exists(tempDirectory))
                    {
                        Directory.CreateDirectory(tempDirectory);
                    }
                }
                catch (Exception e)
                {                    
                    _logger.Error("Unable to create temporary directory: " + tempDirectory);
                    throw e;
                }

                //Check if any files are left over from a previous run exists.  If so delete.
                try
                {
                    CleanupFiles(tempDirectory);

                    if (File.Exists(tempDirectory + "\\" + this.MapNumber + ".tif"))
                    {
                        File.Delete(tempDirectory + "\\" + this.MapNumber + ".tif");
                    }
                    if (File.Exists(tempDirectory + "\\" + this.MapNumber + ".tfw"))
                    {
                        File.Delete(tempDirectory + "\\" + this.MapNumber + ".tfw");
                    }
                }
                catch { }

                if (double.Parse(MapScale) <= 1200)
                {
                    _logger.Debug("Starting TIFF Export");

                    //_logger.Debug("Setting Extent");

                    //Set Extent of the Map Data view
                    IActiveView mapDataView = TIFFMapDocument.Map[0] as IActiveView;

                    ((IMap)mapDataView).ClipGeometry = WKTPolygonToIPolygon(WKTPolygon, ((IMap)mapDataView).SpatialReference);
                    //((IMap)mapDataView).ClipGeometry = mapExtent;
                    SetMaintenancePlatDefQuery((IMap)mapDataView);


                    //IMapAdmin mapAdmin = (IMapAdmin)mapDataView;
                    //mapAdmin.ClipBounds = WKTPolygonToIPolygon(WKTPolygon);  
                    IEnvelope mapExtent = new EnvelopeClass();
                    mapExtent.PutCoords(double.Parse(XMin), double.Parse(YMin), double.Parse(XMax), double.Parse(YMax));
                    mapDataView.Extent = mapExtent;

                    //_logger.Debug("Setting Map Scale");
                    //SetMapScale(double.Parse(this.MapScale), mapDataView, mapDocument.PageLayout);
                    mapDataView.Refresh();

                    //_logger.Debug("Preparing Pixel bounds");
                    //Prepare PixelBounds
                    IActiveView pageLayoutView = TIFFMapDocument.PageLayout as IActiveView;


                    exportFrameRect.left = ((IActiveView)mapDataView).ExportFrame.left;
                    exportFrameRect.top = ((IActiveView)mapDataView).ExportFrame.top;

                    int scaleForDPIRight = Int32.Parse(Math.Floor(((((double)this.TiffDPI) / 96.0) * ((IActiveView)mapDataView).ExportFrame.right)).ToString());
                    int scaleForDPIBottom = Int32.Parse(Math.Floor(((((double)this.TiffDPI) / 96.0) * ((IActiveView)mapDataView).ExportFrame.bottom)).ToString());
                    exportFrameRect.right = this.TiffDPI >= 96 ? scaleForDPIRight : ((IActiveView)mapDataView).ExportFrame.right;
                    exportFrameRect.bottom = this.TiffDPI >= 96 ? scaleForDPIBottom : ((IActiveView)mapDataView).ExportFrame.bottom;
                    //exportFrameRect.right = exportFrameRect.right * 2;
                    //exportFrameRect.bottom = exportFrameRect.bottom * 2;
                    IEnvelope pixelBounds = new EnvelopeClass();
                    pixelBounds.PutCoords(exportFrameRect.left, exportFrameRect.bottom, exportFrameRect.right, exportFrameRect.top);



                    //_logger.Debug("Initializing TIFF Exporter");
                    //Initialize exporter class
                    exportMap = new ExportTIFFClass();
                    //Set the required input parameters
                    exportMap.ExportFileName = System.IO.Path.Combine(tempDirectory, this.MapNumber + ".tif");
                    _logger.Debug("Temporary Export File Name:" + exportMap.ExportFileName);
                    exportMap.Resolution = this.TiffDPI;
                    exportMap.PixelBounds = pixelBounds;

                    //_logger.Debug("Setting Color mode and compression type");
                    //Set the color mode and compression type
                    IExportTIFF exportTiff = exportMap as IExportTIFF;
                    exportTiff.GeoTiff = true;
                    exportTiff.BiLevelThreshold = 128;
                    exportTiff.CompressionType = esriTIFFCompression.esriTIFFCompressionFax4;
                    (exportTiff as IExportImage).ImageType = esriExportImageType.esriExportImageTypeBiLevelThreshold;

                    //_logger.Debug("Seeting up for TFW file creation");

                    IWorldFileSettings worldFileSettings = exportMap as IWorldFileSettings;
                    worldFileSettings.OutputWorldFile = true;
                    worldFileSettings.MapExtent = mapExtent;


                    //Start the export process
                    int hDc = exportMap.StartExporting();
                    tagRECT acViewRect = exportFrameRect;
                    mapDataView.Output(hDc, this.TiffDPI, ref acViewRect, mapExtent, null);
                    exportMap.FinishExporting();


                    //_logger.Debug("Cleaning up resources");
                    //Clean any in-memory resources
                    exportMap.Cleanup();
                    while (Marshal.ReleaseComObject(exportMap) > 0) { };
                    //Release window graphics handle
                    CloseHandle(new IntPtr(hDc));

                    Marshal.ReleaseComObject(mapExtent);
                    Marshal.ReleaseComObject(pixelBounds);

                    _logger.Debug("Resampling to normalize tfw file");
                    Resample(tempDirectory);

                    _logger.Debug("Cleaning up leftover resampling files");
                    CleanupFiles(tempDirectory);

                    _logger.Debug("Copying temporary tif file to final location");
                    MoveTIFFToFinalLocation(tempDirectory, this.TiffOutputfileLocation);

                    _logger.Debug("Final Export File Name:" + this.TiffOutputfileLocation + "\\" + this.MapNumber + ".tif");

                    _logger.Debug("TIFF Export finished.");
                }
                else
                {

                    double XDiff = (double.Parse(XMax) - double.Parse(XMin)) / 5;
                    double YDiff = (double.Parse(YMax) - double.Parse(YMin)) / 5;
                    double xMin = double.Parse(XMin);
                    double xMax = xMin + XDiff;
                    double yMin = double.Parse(YMin);
                    double yMax = yMin + YDiff;

                    //1% overlap
                    double xOverlap = (xMax - xMin) * 0.01;
                    double yOverlap = (yMax - yMin) * 0.01;

                    List<IEnvelope> envelopes = new List<IEnvelope>();
                    for (int i = 0; i < 5; i++)
                    {
                        if (xMax < double.Parse(XMax)) { xMax += xOverlap; }
                        for (int j = 0; j < 5; j++)
                        {
                            if (j == 4) { yMax = double.Parse(YMax); }
                            else if (yMax < double.Parse(YMax)) { yMax += yOverlap; }
                            IEnvelope mapExtent = new EnvelopeClass();
                            mapExtent.PutCoords(xMin, yMin, xMax, yMax);
                            envelopes.Add(mapExtent);
                            if (yMax < double.Parse(YMax)) { yMax -= yOverlap; }

                            yMin = yMax;
                            yMax = yMax + YDiff;
                        }

                        if (i == 4) { xMax = double.Parse(XMax); }
                        else if (xMax < double.Parse(XMax)) { xMax -= xOverlap; }

                        double width = xMax - xMin;

                        xMin = xMax;
                        xMax = xMax + XDiff;

                        //reset yMin and yMax
                        yMin = double.Parse(YMin);
                        yMax = yMin + YDiff;
                    }

                    _logger.Debug("Starting TIFF Export");

                    //_logger.Debug("Setting Extent");

                    //Set Extent of the Map Data view
                    IActiveView mapDataView = TIFFMapDocument.Map[0] as IActiveView;

                    ((IMap)mapDataView).ClipGeometry = WKTPolygonToIPolygon(WKTPolygon, ((IMap)mapDataView).SpatialReference);
                    //((IMap)mapDataView).ClipGeometry = mapExtent;
                    SetMaintenancePlatDefQuery((IMap)mapDataView);

                    int counter = 0;
                    foreach (IEnvelope envelope in envelopes)
                    {
                        XMax = envelope.XMax.ToString();
                        XMin = envelope.XMin.ToString();
                        YMax = envelope.YMax.ToString();
                        YMin = envelope.YMin.ToString();

                        //IMapAdmin mapAdmin = (IMapAdmin)mapDataView;
                        //mapAdmin.ClipBounds = WKTPolygonToIPolygon(WKTPolygon);  
                        IEnvelope mapExtent = new EnvelopeClass();
                        mapExtent.PutCoords(double.Parse(XMin), double.Parse(YMin), double.Parse(XMax), double.Parse(YMax));
                        mapDataView.Extent = mapExtent;

                        //_logger.Debug("Setting Map Scale");
                        //SetMapScale(double.Parse(this.MapScale), mapDataView, mapDocument.PageLayout);
                        mapDataView.Refresh();

                        //_logger.Debug("Preparing Pixel bounds");
                        //Prepare PixelBounds
                        IActiveView pageLayoutView = TIFFMapDocument.PageLayout as IActiveView;


                        exportFrameRect.left = ((IActiveView)mapDataView).ExportFrame.left;
                        exportFrameRect.top = ((IActiveView)mapDataView).ExportFrame.top;

                        int scaleForDPIRight = Int32.Parse(Math.Floor(((((double)this.TiffDPI) / 96.0) * ((IActiveView)mapDataView).ExportFrame.right)).ToString());
                        int scaleForDPIBottom = Int32.Parse(Math.Floor(((((double)this.TiffDPI) / 96.0) * ((IActiveView)mapDataView).ExportFrame.bottom)).ToString());

                        if (double.Parse(MapScale) >= 6000)
                        {
                            //We have a different dpi for 500 scales to allow for smaller dimensions for the downstream DART application.
                            scaleForDPIRight = Int32.Parse(Math.Floor(((((double)this.Tiff500ScaleDPI) / 96.0) * ((IActiveView)mapDataView).ExportFrame.right)).ToString());
                            scaleForDPIBottom = Int32.Parse(Math.Floor(((((double)this.Tiff500ScaleDPI) / 96.0) * ((IActiveView)mapDataView).ExportFrame.bottom)).ToString());
                        }
                        exportFrameRect.right = this.TiffDPI >= 96 ? scaleForDPIRight : ((IActiveView)mapDataView).ExportFrame.right;
                        exportFrameRect.bottom = this.TiffDPI >= 96 ? scaleForDPIBottom : ((IActiveView)mapDataView).ExportFrame.bottom;
                        //exportFrameRect.right = ((IActiveView)mapDataView).ExportFrame.right;
                        //exportFrameRect.bottom = ((IActiveView)mapDataView).ExportFrame.bottom;
                        IEnvelope pixelBounds = new EnvelopeClass();
                        pixelBounds.PutCoords(exportFrameRect.left, exportFrameRect.bottom, exportFrameRect.right, exportFrameRect.top);



                        //_logger.Debug("Initializing TIFF Exporter");
                        //Initialize exporter class
                        exportMap = new ExportTIFFClass();
                        //Set the required input parameters
                        exportMap.ExportFileName = System.IO.Path.Combine(tempDirectory, this.MapNumber + "_" + counter + ".tif");
                        _logger.Debug("ExportFileName:" + exportMap.ExportFileName);
                        exportMap.Resolution = this.TiffDPI;
                        exportMap.PixelBounds = pixelBounds;

                        //_logger.Debug("Setting Color mode and compression type");
                        //Set the color mode and compression type
                        IExportTIFF exportTiff = exportMap as IExportTIFF;
                        exportTiff.GeoTiff = true;
                        exportTiff.BiLevelThreshold = 128;
                        exportTiff.CompressionType = esriTIFFCompression.esriTIFFCompressionFax4;
                        (exportTiff as IExportImage).ImageType = esriExportImageType.esriExportImageTypeBiLevelThreshold;

                        //_logger.Debug("Seeting up for TFW file creation");

                        IWorldFileSettings worldFileSettings = exportMap as IWorldFileSettings;
                        worldFileSettings.OutputWorldFile = true;
                        worldFileSettings.MapExtent = mapExtent;


                        //Start the export process
                        int hDc = exportMap.StartExporting();
                        tagRECT acViewRect = exportFrameRect;
                        mapDataView.Output(hDc, this.TiffDPI, ref acViewRect, mapExtent, null);
                        exportMap.FinishExporting();


                        //_logger.Debug("Cleaning up resources");
                        //Clean any in-memory resources
                        exportMap.Cleanup();
                        while (Marshal.ReleaseComObject(exportMap) > 0) { };
                        //Release window graphics handle
                        CloseHandle(new IntPtr(hDc));

                        Marshal.ReleaseComObject(mapExtent);
                        Marshal.ReleaseComObject(pixelBounds);
                        counter++;
                    }
                    
                    _logger.Debug("Merging TIFFs together with python script");
                    MergeAndCleanup(((IMap)mapDataView).SpatialReference.Name, tempDirectory);

                    _logger.Debug("Resampling to normalize tfw file");
                    Resample(tempDirectory);

                    _logger.Debug("Cleaning up leftover merge and resampling files");
                    CleanupFiles(tempDirectory);

                    _logger.Debug("Copying temporary tif file to final location");
                    MoveTIFFToFinalLocation(tempDirectory, this.TiffOutputfileLocation);

                    _logger.Debug("TIFF Export finished.");
                }
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message + Environment.NewLine + ex.StackTrace);
                _logger.Error("Error  processing map:" + this.MapNumber, ex);
            }
            finally
            {
                
                if (exportMap != null)
                {
                    while (Marshal.ReleaseComObject(exportMap) > 0) { }
                }
                if (MapProductionFacade.PGEMap != null)
                {
                    while (Marshal.ReleaseComObject(MapProductionFacade.PGEMap) > 0) { };
                }
                if (MapProductionFacade.PGEPageLayout != null)
                {
                    while (Marshal.ReleaseComObject(MapProductionFacade.PGEPageLayout) > 0) { }
                }
                GC.Collect();

                XMax = XMaxOrig.ToString();
                XMin = XMinOrig.ToString();
                YMax = YMaxOrig.ToString();
                YMin = YMinOrig.ToString();

                

            }
            return errors;
        }

        private void Resample(string outputLocation)
        {
            string resampledTIFName = outputLocation + "\\" + this.MapNumber + "_Resample.tif";
            string newTifName = outputLocation + "\\" + this.MapNumber + ".tif";
            string resampledTFWName = outputLocation + "\\" + this.MapNumber + "_Resample.tfw";
            string newTFWName = outputLocation + "\\" + this.MapNumber + ".tfw";

            try
            {
                if (gp == null)
                {
                    gp = new Geoprocessor();
                    gp.SetEnvironmentValue("compression", @"'CCITT Group 4' 75");
                    gp.SetEnvironmentValue("workspace", outputLocation);
                    gp.SetEnvironmentValue("rasterStatistics", "NONE");
                    gp.SetEnvironmentValue("pyramid", "NONE");
                }
                Resample resampleTool = new Resample();
                resampleTool.in_raster = newTifName;
                resampleTool.out_raster = resampledTIFName;
                gp.Execute(resampleTool, null);
            }
            catch (Exception e)
            {
                _logger.Error("Failed resampling Tif");
                throw e;
            }

            try
            {
                ReSave(resampledTIFName, newTifName, resampledTFWName, newTFWName);
            }
            catch (Exception e)
            {
                _logger.Error("Failed saving resampled Tif");
                throw e;
            }

            
        }

        /// <summary>
        /// Merges all 25 tiffs into one.  This must be done in groups of 5 as the geoprocessor doesn't seem to want to merge 25 in one go.
        /// </summary>
        /// <param name="reference"></param>
        private void MergeAndCleanup(string prjName, string outputDirectory)
        {
            //Determine the file name of the projection
            //string prjName = reference.Name;
            string coordinateSystem = "";

            
            if (prjName.Contains("0401")) { coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_I_FIPS_0401',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-122.0],PARAMETER['Standard_Parallel_1',40.0],PARAMETER['Standard_Parallel_2',41.66666666666666],PARAMETER['Latitude_Of_Origin',39.33333333333334],UNIT['Foot_US',0.3048006096012192]]"; }
            else if (prjName.Contains("0402")) { coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_II_FIPS_0402',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-122.0],PARAMETER['Standard_Parallel_1',38.33333333333334],PARAMETER['Standard_Parallel_2',39.83333333333334],PARAMETER['Latitude_Of_Origin',37.66666666666666],UNIT['Foot_US',0.3048006096012192]]"; }
            else if (prjName.Contains("0403")) { coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_III_FIPS_0403',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-120.5],PARAMETER['Standard_Parallel_1',37.06666666666667],PARAMETER['Standard_Parallel_2',38.43333333333333],PARAMETER['Latitude_Of_Origin',36.5],UNIT['Foot_US',0.3048006096012192]]"; }
            else if (prjName.Contains("0404")) { coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_IV_FIPS_0404',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-119.0],PARAMETER['Standard_Parallel_1',36.0],PARAMETER['Standard_Parallel_2',37.25],PARAMETER['Latitude_Of_Origin',35.33333333333334],UNIT['Foot_US',0.3048006096012192]]"; }
            else if (prjName.Contains("0405")) { coordinateSystem = "PROJCS['NAD_1927_StatePlane_California_V_FIPS_0405',GEOGCS['GCS_North_American_1927',DATUM['D_North_American_1927',SPHEROID['Clarke_1866',6378206.4,294.9786982]],PRIMEM['Greenwich',0.0],UNIT['Degree',0.0174532925199433]],PROJECTION['Lambert_Conformal_Conic'],PARAMETER['False_Easting',2000000.0],PARAMETER['False_Northing',0.0],PARAMETER['Central_Meridian',-118.0],PARAMETER['Standard_Parallel_1',34.03333333333333],PARAMETER['Standard_Parallel_2',35.46666666666667],PARAMETER['Latitude_Of_Origin',33.5],UNIT['Foot_US',0.3048006096012192]]"; }

            //Perform the merges.  These have to be separated out due to what appears to be limitations in the Esri geoprocessor tool
            string fileNames = "";
            string baseFilename = this.MapNumber;
            string outputFileName = "";

            fileNames = baseFilename + "_0.tif;" + baseFilename + "_1.tif;" + baseFilename + "_2.tif";
            outputFileName = baseFilename + "_Merge0.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_3.tif;" + baseFilename + "_4.tif";
            outputFileName = baseFilename + "_Merge1.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_5.tif;" + baseFilename + "_6.tif;" + baseFilename + "_7.tif";
            outputFileName = baseFilename + "_Merge2.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_8.tif;" + baseFilename + "_9.tif";
            outputFileName = baseFilename + "_Merge3.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_10.tif;" + baseFilename + "_11.tif;" + baseFilename + "_12.tif";
            outputFileName = baseFilename + "_Merge4.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_13.tif;" + baseFilename + "_14.tif";
            outputFileName = baseFilename + "_Merge5.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_15.tif;" + baseFilename + "_16.tif;" + baseFilename + "_17.tif";
            outputFileName = baseFilename + "_Merge6.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_18.tif;" + baseFilename + "_19.tif";
            outputFileName = baseFilename + "_Merge7.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_20.tif;" + baseFilename + "_21.tif;" + baseFilename + "_22.tif";
            outputFileName = baseFilename + "_Merge8.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_23.tif;" + baseFilename + "_24.tif";
            outputFileName = baseFilename + "_Merge9.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge0.tif;" + baseFilename + "_Merge1.tif";
            outputFileName = baseFilename + "_Merge10.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge2.tif;" + baseFilename + "_Merge3.tif";
            outputFileName = baseFilename + "_Merge11.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge4.tif;" + baseFilename + "_Merge5.tif";
            outputFileName = baseFilename + "_Merge12.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge6.tif;" + baseFilename + "_Merge7.tif";
            outputFileName = baseFilename + "_Merge13.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge8.tif;" + baseFilename + "_Merge9.tif";
            outputFileName = baseFilename + "_Merge14.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge10.tif;" + baseFilename + "_Merge11.tif;" + baseFilename + "_Merge12.tif";
            outputFileName = baseFilename + "_Merge15.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge13.tif;" + baseFilename + "_Merge14.tif";
            outputFileName = baseFilename + "_Merge16.tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

            fileNames = baseFilename + "_Merge15.tif;" + baseFilename + "_Merge16.tif";
            outputFileName = baseFilename + ".tif";
            Merge(fileNames, outputFileName, coordinateSystem, true, outputDirectory);

        }

        /// <summary>
        /// Moves the specified file to its final location.  Deletes any existing file if it exists.
        /// </summary>
        private void MovePDFToFinalLocation(string tempDirectory, string newDirectory)
        {
            string tempFilename = tempDirectory + "\\" + this.MapNumber;
            string newFileName = newDirectory + "\\" + this.MapNumber;

            try
            {
                //Attempt to delete the current tif.
                if (File.Exists(newFileName + ".pdf"))
                {
                    File.Delete(newFileName + ".pdf");
                }
            }
            catch (Exception e) { _logger.Error("Unable to delete current pdf: " + newFileName + "\n" + e.Message); }

            try
            {
                //Move the temp file to it's new location and delete the temp file.
                File.Copy(tempFilename + ".pdf", newFileName + ".pdf");
                File.Delete(tempFilename + ".pdf");
            }
            catch (Exception e) 
            { 
                _logger.Error("Unable to copy temporary pdf to final location: " + newFileName + "\n" + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Moves the specified file to its final location.  Deletes any existing file if it exists.
        /// </summary>
        private void MoveTIFFToFinalLocation(string tempDirectory, string newDirectory)
        {
            string tempFilename = tempDirectory + "\\" + this.MapNumber;
            string newFileName = newDirectory + "\\" + this.MapNumber;

            try
            {
                //Attempt to delete the current tif.
                if (File.Exists(newFileName + ".tif"))
                {
                    File.Delete(newFileName + ".tif");
                }
                if (File.Exists(newFileName + ".tfw"))
                {
                    File.Delete(newFileName + ".tfw");
                }
            }
            catch (Exception e) { _logger.Error("Unable to delete current tif or tfw: " + newFileName + "\n" + e.Message); }

            try
            {
                //Move the temp file to it's new location and delete the temp file.
                File.Copy(tempFilename + ".tif", newFileName + ".tif");
                File.Copy(tempFilename + ".tfw", newFileName + ".tfw");
                File.Delete(tempFilename + ".tif");
                File.Delete(tempFilename + ".tfw");
            }
            catch (Exception e)
            {
                _logger.Error("Unable to copy temporary tif and tfw to final location: " + newFileName + "\n" + e.Message);
                throw e;
            }
        }

        /// <summary>
        /// Cleans up any leftover files from merges and resampling
        /// </summary>
        private void CleanupFiles(string outputDirectory)
        {
            string baseFilename = outputDirectory + "\\" + this.MapNumber;

            for (int i = 0; i < 25; i++)
            {
                try
                {
                    File.Delete(baseFilename + "_" + i + ".tif");
                    File.Delete(baseFilename + "_" + i + ".tfw");
                }
                catch { }
            }

            for (int i = 0; i < 17; i++)
            {
                try
                {
                    File.Delete(baseFilename + "_Merge" + i + ".tif");
                    File.Delete(baseFilename + "_Merge" + i + ".tfw");
                    File.Delete(baseFilename + "_Merge" + i + ".tif.aux.xml");
                    File.Delete(baseFilename + "_Merge" + i + ".tif.xml");
                }
                catch { }
            }

            try
            {
                //Clean up unnecessary files created during resample
                File.Delete(baseFilename + "_Resample.tif.aux.xml");
                File.Delete(baseFilename + "_Resample.tif.xml");
            }
            catch { }

            try
            {
                File.Delete(baseFilename + ".tif.aux.xml");
                File.Delete(baseFilename + ".tif.xml");
            }
            catch { }
        }

        Geoprocessor gp = null;

        /// <summary>
        /// Merge the file names specified to the output file name specified.
        /// </summary>
        /// <param name="fileNamesList"></param>
        /// <param name="outputFileName"></param>
        /// <param name="coordinateSystem"></param>
        /// <param name="retry"></param>
        public void Merge(string fileNames, string outputFileName, string coordinateSystem, bool retry, string outputDirectory)
        {
            try
            {
                if (gp == null)
                {
                    gp = new Geoprocessor();
                    gp.SetEnvironmentValue("compression", @"'CCITT Group 4' 75");
                    gp.SetEnvironmentValue("workspace", outputDirectory);
                    gp.SetEnvironmentValue("rasterStatistics", "NONE");
                    gp.SetEnvironmentValue("pyramid", "NONE");
                }
                MosaicToNewRaster mosaicToNewRaster = new MosaicToNewRaster();
                mosaicToNewRaster.input_rasters = fileNames;
                mosaicToNewRaster.output_location = outputDirectory;
                mosaicToNewRaster.raster_dataset_name_with_extension = outputFileName;
                mosaicToNewRaster.coordinate_system_for_the_raster = coordinateSystem;
                mosaicToNewRaster.pixel_type = "1_BIT";
                mosaicToNewRaster.cellsize = 0;
                mosaicToNewRaster.number_of_bands = 1;
                mosaicToNewRaster.mosaic_method = "MAXIMUM";
                mosaicToNewRaster.mosaic_colormap_mode = "FIRST";
                gp.Execute(mosaicToNewRaster, null);
            }
            catch (Exception e)
            {
                if (retry)
                {
                    Thread.Sleep(5000);
                    Merge(fileNames, outputFileName, coordinateSystem, false, outputDirectory);
                }
                else
                {
                    string gpMessages = "";
                    for (int i = 0; i < gp.MessageCount; i++)
                    {
                        gpMessages += gp.GetMessage(i);
                    }
                    _logger.Debug("Geoprocessor failed to merge: " + fileNames + ": Geoprocessor messages: " + gpMessages);
                    throw e;
                }
            }
        }

        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private void ReSave(string origFileName, string newFileName, string origTfwFileName, string newTfwFileName)
        {
            Image image = new Bitmap(origFileName);

            ImageCodecInfo myImageCodecInfo;
            System.Drawing.Imaging.Encoder myEncoder;
            EncoderParameter myEncoderParameter;
            EncoderParameters myEncoderParameters;

            // Get an ImageCodecInfo object that represents the TIFF codec.
            myImageCodecInfo = GetEncoderInfo("image/tiff");

            // Create an Encoder object based on the GUID 
            // for the Compression parameter category.
            myEncoder = System.Drawing.Imaging.Encoder.Compression;

            // Create an EncoderParameters object. 
            // An EncoderParameters object has an array of EncoderParameter 
            // objects. In this case, there is only one 
            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a TIFF file with CCIT4 compression.
            myEncoderParameter = new EncoderParameter(
                myEncoder,
                (long)EncoderValue.CompressionCCITT4);
            myEncoderParameters.Param[0] = myEncoderParameter;

            if (File.Exists(newTfwFileName)) {File.Delete(newTfwFileName);}
            if (File.Exists(newFileName)) { File.Delete(newFileName); }

            image.Save(newFileName, myImageCodecInfo, myEncoderParameters);
            image.Dispose();
            image = null;

            //Pause to ensure the file is released before deleting original
            Thread.Sleep(2000);

            if (File.Exists(origFileName)) { File.Delete(origFileName); }
            if (File.Exists(origTfwFileName)) {File.Move(origTfwFileName, newTfwFileName);}
        }

        string GetPythonPath()
        {
            string pythonPath = null;
            var localmachineKey = Registry.LocalMachine;
            // Check whether we are on a 64-bit OS by checking for the Wow6432Node key (32-bit version of the Software registry key)
            var softwareKey = localmachineKey.OpenSubKey(@"SOFTWARE\Wow6432Node"); // This is the correct key for 64-bit OS's
            if (softwareKey == null)
            {
                softwareKey = localmachineKey.OpenSubKey("SOFTWARE"); // This is the correct key for 32-bit OS's
            }
            var esriKey = softwareKey.OpenSubKey("ESRI");
            var realVersion = (string)esriKey.OpenSubKey("ArcGIS").GetValue("RealVersion"); // Get the "real", canonical version of ArcGIS
            var shortVersion = String.Join(".", realVersion.Split('.').Take(2).ToArray()); // Get just the Major.Minor part of the version number, e.g. 10.1
            var pythonKey = esriKey.OpenSubKey("Python" + shortVersion); // Open the Python10.x sub-key
            if (pythonKey == null)
            {
                throw new InvalidOperationException("Python not installed with ArcGIS!");
            }
            var pythonDirectory = (string)pythonKey.GetValue("PythonDir");
            if (Directory.Exists(pythonDirectory))
            {
                // Build path to python.exe
                string pythonPathFromReg = System.IO.Path.Combine(System.IO.Path.Combine(pythonDirectory, "ArcGIS" + shortVersion), "python.exe");
                if (File.Exists(pythonPathFromReg))
                {
                    pythonPath = pythonPathFromReg;
                }
            }
            return pythonPath;
        }

        private void SetVisibleScale(IMap map)
        {
            double mapScale = double.Parse(MapScale);

            IEnumLayer layers = map.get_Layers(null, true);
            layers.Reset();
            ILayer layer = layers.Next();
            while (layer != null)
            {
                SetVisibleScale(layer, mapScale, 0.0);
                layer = layers.Next();
            }
        }

        private void SetVisibleScale(ILayer layer, double mapScale, double newScale)
        {
            if (layer is ICompositeLayer)
            {
                ICompositeLayer compLayer = layer as ICompositeLayer;
                for (int i = 0; i < compLayer.Count; i++)
                {
                    ILayer tempLayer = compLayer.get_Layer(i);
                    SetVisibleScale(tempLayer, mapScale, newScale);
                }
            }

            if (layer.MinimumScale <= mapScale)
            {
                layer.MinimumScale = newScale;
            }
        }

        /// <summary>
        /// This method will export a map based on the input paramters
        /// </summary>
        /// <param name="iOutputResolution">Resolution requested in dpi</param>
        /// <param name="lResampleRatio">Sample ratio 1 to 5. 1 corresponds to the best quality</param>
        /// <param name="ExportType">String that represents the export type to create (i.e. PDF)</param>
        /// <param name="sOutputDir">Directory to where the document should be created</param>
        /// <param name="bClipToGraphicsExtent">Assign True or False to determine if export image will be clipped to the graphic * extent of layout elements.  This value is ignored for data view exports</param>
        /// <param name="Name">Name of the exported file without the file extension (i.e. Output not Output.pdf)</param>
        private void ExportActiveViewParameterized(IActiveView docActiveView, long iOutputResolution, long lResampleRatio, string ExportType, string sOutputDir, Boolean bClipToGraphicsExtent, string Name)
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

            // using predefined static member
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
                    //font smoothing is NOT successfully disabled, error out.
                    return;
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
                docExport = new ExportTIFFClass();
                //Set the required input parameters
                //docExport.Resolution = iOutputResolution;
                //docExport.PixelBounds = pixelBounds;


                //Set the color mode and compression type
                IExportTIFF exportTiff = docExport as IExportTIFF;
                exportTiff.GeoTiff = true;
                exportTiff.BiLevelThreshold = 128;
                exportTiff.CompressionType = esriTIFFCompression.esriTIFFCompressionFax4;
                (exportTiff as IExportImage).ImageType = esriExportImageType.esriExportImageTypeBiLevelThreshold;


                IWorldFileSettings worldFileSettings = docExport as IWorldFileSettings;
                worldFileSettings.OutputWorldFile = true;
                worldFileSettings.MapExtent = docActiveView.Extent;

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
                //Log("Unsupported export type " + ExportType + ", defaulting to EMF.");
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


            docPrintExport.Export(docActiveView, docExport, iOutputResolution, bClipToGraphicsExtent, null);

            try
            {
                //Attempt to cleanup resources
                docExport.Cleanup();
            }
            catch
            {

            }

            //Log("Finished exporting " + sOutputDir + sNameRoot + "." + docExport.Filter.Split('.')[1].Split('|')[0].Split(')')[0] + ".");

            if (bReenable)
            {
                /* reenable font smoothing if we disabled it before */
                EnableFontSmoothing();
                bReenable = false;
                if (!GetFontSmoothing())
                {
                    //error: cannot reenable font smoothing.
                    //Log("Unable to reenable Font Smoothing");
                }
            }
        }

        /// <summary>
        /// Returns the first instance of the IFeatureLayer that matches the request feature class name
        /// </summary>
        /// <param name="map">IMap to search</param>
        /// <param name="featureClassName">Name of feature class to find</param>
        /// <returns></returns>
        public static IFeatureLayer getFeatureLayerFromMap(IMap map, string featureClassName)
        {
            //Get all of the IFeatureLayers
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
            IEnumLayer enumLayer = map.get_Layers(uid, true);
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                IFeatureLayer featLayer = layer as IFeatureLayer;
                if (featLayer != null)
                {
                    IDataset ds = featLayer as IDataset;
                    if (ds.BrowseName.ToUpper() == featureClassName.ToUpper())
                    {
                        return featLayer;
                    }
                }
                layer = enumLayer.Next();
            }
            return null;
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

        #endregion Public Methods

        #region Private Methods

        private bool IsInvalid()
        {
            bool isInvalid = false;
            isInvalid = string.IsNullOrEmpty(this.MxdLocation) || string.IsNullOrEmpty(this.PdfMxdFileName) || string.IsNullOrEmpty(this.TiffMxdFileName);
            isInvalid = isInvalid || string.IsNullOrEmpty(this.MapNumber) || string.IsNullOrEmpty(this.MapScale);
            isInvalid = isInvalid || string.IsNullOrEmpty(this.PdfOutputfileLocation) || string.IsNullOrEmpty(this.TiffOutputfileLocation);
            return isInvalid;
        }

        /// <summary>
        /// Updates the Extent of the Data View such that the Data is displayed at the specified scale
        /// </summary>
        /// <param name="mapScale">Mapscale to display the data</param>
        private void SetMapScale(double mapScale, ref IActiveView mapDataView, IPageLayout layout)
        {
            IPage page = layout.Page;
            double height, width;
            page.QuerySize(out width, out height);
            IUnitConverter unitConvertor = new UnitConverterClass();
            esriUnits mapUnits = (mapDataView as IMap).MapUnits;
            height = unitConvertor.ConvertUnits(height * mapScale, page.Units, mapUnits);
            width = unitConvertor.ConvertUnits(width * mapScale, page.Units, mapUnits);

            IEnvelope mapExtent = mapDataView.Extent;
            mapExtent.Expand((width - mapExtent.Width) / 2, (height - mapExtent.Height) / 2, false);
            mapDataView.Extent = mapExtent;
        }

        private IMap SetExtentsAndGetIMapToPlot(IMapDocument mapDocument, IEnvelope envelope)
        {
            IMap retVal = null;
            IGraphicsContainer gc = (IGraphicsContainer)mapDocument.PageLayout;
            IActiveView activeView = (IActiveView)mapDocument.PageLayout;
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
            //IEnvelope zoomedEnvelope = envelope.Envelope;
            //zoomedEnvelope.Expand(1.2, 1.2, true);
            //while ((element = gc.Next()) != null)
            //{
            //    if (element is IMapFrame)
            //    { 
            //        if (element.Equals(largestMapFrame))
            //        {
            //            ((IActiveView)((IMapFrame)element).Map).Extent = envelope;
            //        }
            //        else
            //        {
            //            ((IActiveView)((IMapFrame)element).Map).Extent = zoomedEnvelope;
            //        }
            //    }
            //}
            //Loop through and set the extent to smaller map frames by expanding the Actual Envelope 
            if (largestMapFrame != null)
            {
                ((IActiveView)((IMapFrame)largestMapFrame).Map).Extent = envelope;
                retVal = ((IMapFrame)largestMapFrame).Map;
                //retVal.ClipGeometry = WKTPolygonToIPolygon(WKTPolygon, retVal.SpatialReference);
                SetMaintenancePlatDefQuery(retVal);
            }
            return retVal;
        }

        private void GetWidthAndHeight(IElement element, IActiveView activeView, out double width, out double height)
        {
            width = 0.0;
            height = 0.0;
            IEnvelope env = new EnvelopeClass();
            element.QueryBounds(activeView.ScreenDisplay, env);
            width = Math.Round(env.Width);
            height = Math.Round(env.Height);
        }

        private IGeometry WKTPolygonToIPolygon(string wktPolygon,ISpatialReference spRef)
        {
            IGeometry retVal = null;
            string wktString = wktPolygon.Replace("),(", ")|(");
            wktString = wktString.Replace("POLYGON(", string.Empty);
            wktString = wktString.Substring(1, wktString.Length - 3);
            string[] ringsInWKT = wktString.Split("|".ToCharArray());
            string ring = string.Empty;
            List<string> pointColl = new List<string>();
            foreach(string wktRing in ringsInWKT)
            {
                ring = wktRing.Replace("(", string.Empty);
                ring = wktRing.Replace(")", string.Empty);
                string[] points = ring.Split(",".ToCharArray());
                pointColl.AddRange(points); 
            }
            IPointCollection4 pointColl4 = new PolygonClass();
            IGeometryBridge2 geomBridge = new GeometryEnvironmentClass();
            WKSPoint[] wkspoints = new WKSPoint[pointColl.Count];

            for (int i = 0; i < pointColl.Count; i++)
            {
                wkspoints[i] = new WKSPoint();
                wkspoints[i].X = Convert.ToDouble(pointColl[i].Split(" ".ToCharArray())[0]);
                wkspoints[i].Y = Convert.ToDouble(pointColl[i].Split(" ".ToCharArray())[1]);
            }
            geomBridge.SetWKSPoints(pointColl4, ref wkspoints);
            retVal = (IPolygon)pointColl4;
            retVal.SpatialReference = spRef; 
            return retVal;
        }
        /// <summary>
        /// Currently hard coded to meet PGE Requirements.
        /// </summary>
        /// <param name="map"></param>
        private void SetMaintenancePlatDefQuery(IMap map)
        {
            string defQuery = MapProductionConfigurationHandler.GetSettingValue("DefinitionQuery");
            if (string.IsNullOrEmpty(defQuery)) defQuery="MapType<>{0} or (MapType={1} and MapNumber='{2}' and MapOffice='{3}')";
            List<IFeatureLayer> featLayers = getFeatureLayersFromMap(map, "EDGIS.MaintenancePlat");
            IFeatureLayer featLayer = featLayers[0];
            ITableDefinition tableDef = (ITableDefinition)featLayer;
            string mapscale = GetMapScale();
            tableDef.DefinitionExpression = string.Format(defQuery, mapscale, mapscale, MapNumber, MapOfficeCode); 
        }

        /// <summary>
        /// Returns a List of IFeatureLayers that match the provided feature class name
        /// </summary>
        /// <param name="map">IMap to search</param>
        /// <param name="featureClassName">Name of feature class to find</param>
        /// <returns></returns>
        public static List<IFeatureLayer> getFeatureLayersFromMap(IMap map, string featureClassName)
        {
            List<IFeatureLayer> featLayers = new List<IFeatureLayer>();

            //Get all of the IFeatureLayers
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
            IEnumLayer enumLayer = map.get_Layers(uid, true);
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                IFeatureLayer featLayer = layer as IFeatureLayer;
                if (featLayer != null)
                {
                    IDataset ds = featLayer as IDataset;
                    if (ds.BrowseName.ToUpper() == featureClassName.ToUpper())
                    {
                        featLayers.Add(featLayer);
                    }
                }
                layer = enumLayer.Next();
            }
            return featLayers;
        }
        
        private string GetMapScale()
        {
            switch (MapScale)
            {
                case "300":
                    return "25";
                case "600":
                    return "50";
                case "1200":
                    return "100";
                case "2400":
                    return "200";
                case "3000":
                    return "250";
                case "6000":
                    return "500";
                default:
                    return "100";
            }
        }
        #endregion Private Methods

    }
}
