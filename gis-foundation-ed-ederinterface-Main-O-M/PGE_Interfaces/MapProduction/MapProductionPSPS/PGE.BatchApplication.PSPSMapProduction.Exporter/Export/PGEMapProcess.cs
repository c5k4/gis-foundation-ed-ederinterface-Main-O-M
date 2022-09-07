using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geoprocessing;
using PGE.Common.Delivery.Systems.Configuration;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Display;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.DataManagementTools;
using ESRI.ArcGIS.Geoprocessor;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;

namespace PGE.BatchApplication.PSPSMapProduction.Export
{
    /// <summary>
    /// This class had methods to export the Map to PDF and TIFF formats
    /// </summary>
    public class PGEMapProcess
    {
        public static bool LoadTiffMxd = true;
        private static IMapDocument PDFMapDocument = null;

        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");

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
        public string MapId
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
        public string CircuitId
        {
            get;
            set;
        }
        public string ChildCircuitIds
        {
            get;
            set;
        }
        public string CircuitName
        {
            get;
            set;
        }
        public string ChildCircuitName
        {
            get;
            set;
        }
        public string PSPSName
        {
            get;
            set;
        }
        public int LayoutType
        {
            get;
            set;
        }
        public string TotalMileage
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

        #endregion Public Properties

        #region Private Properties
        /// <summary>
        /// 
        /// </summary>
        private IMap PGEMap { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private IPageLayout PGEPageLayout { get; set; }

        /// <summary>
        /// Caption of the <c>PGE Hash Buffer</c> AutoTextElement
        /// </summary>
        private string PGECircuitNameCaption
        {
            get { return "[CircuitName]"; }
        }

        /// <summary>
        /// Caption of the <c>PGE Hash Buffer</c> AutoTextElement
        /// </summary>
        private string PGEChildCircuitNameCaption
        {
            get { return "[ChildCircuitName]"; }
        }
        /// <summary>
        /// Caption of the <c>PGE Hash Buffer</c> AutoTextElement
        /// </summary>
        private string PGEPSPSNameCaption
        {
            get { return "[PSPSName]"; }
        }

        /// <summary>
        /// Caption of the <c>PGE Hash Buffer</c> AutoTextElement
        /// </summary>
        private string PGETotalMileageCaption
        {
            get { return "[TotalMileages]"; }
        }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Executes the Export to PDF operation
        /// </summary>
        /// <returns>Returns list of errors, if any, while executing the operation</returns>
        public List<string> ExportPDF()
        {
            List<string> errors = new List<string>();
            
            ExportPDFClass exportMap = null;
            try
            {
                //Validate
                if (IsInvalid())
                {
                    errors.Add("One or more required parameters missing for the PDF export operation for Map Number = " + this.MapId);
                    return errors;
                }
                //Open Map Document
                //Get the Right MapDocument based on size
                IEnvelope env = new EnvelopeClass();
                env.PutCoords(double.Parse(XMin), double.Parse(YMin), double.Parse(XMax), double.Parse(YMax));
                // make the extent a little bigger (2%)
                bool isLandscape = (this.LayoutType == MapLayoutType.LandscapeOverviewMap || this.LayoutType == MapLayoutType.LandscapeSegmentMap);

                env.Expand(env.Width * 0.02, env.Height * 0.02, false);

                string mxdPath = System.IO.Path.Combine(this.MxdLocation, this.PdfMxdFileName);
               
                if (PDFMapDocument != null)
                {
                    if (PDFMapDocument != null)
                    {
                        PDFMapDocument.Close();
                        while (Marshal.ReleaseComObject(PDFMapDocument) > 0) { }
                        PDFMapDocument = null;
                    }
                }

                PDFMapDocument = new MapDocumentClass();
                _logger.Debug("Opening map document : " + mxdPath);
                PDFMapDocument.Open(mxdPath);

                //If old temp pdf exists let's delete it
                try
                {
                    if (File.Exists(this.PdfOutputfileLocation + "\\" + this.MapId + ".pdf"))
                    {
                        File.Delete(this.PdfOutputfileLocation + "\\" + this.MapId + ".pdf");
                    }
                }
                catch { }

                IMap map = SetExtentsAndGetIMapToPlot(PDFMapDocument, env, isLandscape);
                PGEMap = map;
                PGEPageLayout = PDFMapDocument.PageLayout;
                //Set Extent of the Map Data view
                IActiveView mapDataView = map as IActiveView;
                _logger.Debug("Zoomed to extent");
                _logger.Debug("Setting Map Scale");

                _logger.Debug("Setting Circuit Name to:" + this.CircuitName);
                ChangeDynamicTextElements(PDFMapDocument.PageLayout, this.CircuitName, this.ChildCircuitName, this.PSPSName, this.TotalMileage);

                //Store the Prmary key of the current Map grid to be plotted
                string currentMapGridPrimaryKey = this.CircuitName + ":" + this.PSPSName;

                _logger.Debug("Preparing Pixel bounds");
                //Prepare PixelBounds
                IActiveView pageLayoutView = PDFMapDocument.PageLayout as IActiveView;
                pageLayoutView.Extent = pageLayoutView.FullExtent;
                pageLayoutView.Refresh();
                tagRECT exportFrameRect = new tagRECT();
                exportFrameRect.left = pageLayoutView.ExportFrame.left;
                exportFrameRect.top = pageLayoutView.ExportFrame.top;
                exportFrameRect.right = pageLayoutView.ExportFrame.right * 100 / 96 * (this.PdfDPI / 96);
                exportFrameRect.bottom = pageLayoutView.ExportFrame.bottom * 100 / 96 * (this.PdfDPI / 96);
                IEnvelope pixelBounds = new EnvelopeClass();
                pixelBounds.PutCoords(exportFrameRect.left, exportFrameRect.top, exportFrameRect.right, exportFrameRect.bottom);

                _logger.Debug("Initializing PDF Exporter");
                //Initialize exporter class
                exportMap = new ExportPDFClass();
                //Set the required input parameters
                exportMap.ExportFileName = System.IO.Path.Combine(this.PdfOutputfileLocation, this.MapId + ".pdf");
                _logger.Debug("Temporary Export File Name:" + exportMap.ExportFileName); 
                exportMap.Resolution = this.PdfDPI;
                exportMap.PixelBounds = pixelBounds;

                _logger.Debug("Set the Export attribute Property");
                //Set the Export attribute Property
                IExportPDF2 exportPDF = exportMap as IExportPDF2;
                exportPDF.ExportPDFLayersAndFeatureAttributes = this.IncludeAttributesforPDF ?
                    esriExportPDFLayerOptions.esriExportPDFLayerOptionsLayersAndFeatureAttributes :
                    esriExportPDFLayerOptions.esriExportPDFLayerOptionsNone;

                exportMap.Compressed = true;
                exportMap.ImageCompression = esriExportImageCompression.esriExportImageCompressionDeflate;
                exportMap.EmbedFonts = true;
                //exportMap.JPEGCompressionQuality = 100; 
                exportMap.ResampleRatio = 1;
                exportMap.ExportPictureSymbolOptions = esriPictureSymbolOptions.esriPSORasterizeIfRasterData;
                exportMap.PolygonizeMarkers = true;

                _logger.Debug("Starting the export");
                //Start the export process
                int hDc = exportMap.StartExporting();
                tagRECT acViewRect = exportFrameRect;

                pageLayoutView = PDFMapDocument.PageLayout as IActiveView;

                pageLayoutView.Output(hDc, this.PdfDPI, ref acViewRect, null, null);
                exportMap.FinishExporting();
                _logger.Debug("Finished exporting");
                _logger.Debug("Cleaning up resources");
                //Clean any in-memory resources
                exportMap.Cleanup();
                while (Marshal.ReleaseComObject(exportMap) > 0) { };
                //Release the resources
                Marshal.ReleaseComObject(env);
                Marshal.ReleaseComObject(pixelBounds);

            }
            catch (Exception ex)
            {
                errors.Add(ex.Message + Environment.NewLine + ex.StackTrace);
                _logger.Debug("Error processing map :" + this.MapId, ex);
            }
            finally
            {
                if (exportMap != null)
                {
                    while (Marshal.ReleaseComObject(exportMap) > 0) { }
                }
                if (PGEMap != null)
                {
                    while (Marshal.ReleaseComObject(PGEMap) > 0) { }
                }
                if (PGEPageLayout != null)
                {
                    while (Marshal.ReleaseComObject(PGEPageLayout) > 0) { }
                }
                GC.Collect();
            }
            return errors;
        }

        /// <summary>
        /// Returns the ATE with the given caption
        /// </summary>
        /// <param name="pageLayout">Pagelayout to look for the ATE</param>
        /// <param name="autoTextElementCaption">Caption of the ATE looking for </param>
        /// <returns>Returns the first ATE with the given caption</returns>
        public IElement ChangeDynamicTextElements(IPageLayout pageLayout, string circuitName, string childCircuitName, string pspsName, string totalMileages)
        {
            IElement element = null;

            if (pageLayout == null) return element;

            IGraphicsContainer graphicsContainer = pageLayout as IGraphicsContainer;
            graphicsContainer.Reset();

            //Looping through the Elements of the Map Document to find the element with requested name
            while ((element = graphicsContainer.Next()) != null)
            {
                if (element is ITextElement)
                {
                    ITextElement textElement = element as ITextElement;
                    if (textElement.Text == PGECircuitNameCaption)
                    {
                        textElement.Text = circuitName;
                    }
                    else if (textElement.Text == PGEChildCircuitNameCaption)
                    {
                        textElement.Text = String.IsNullOrEmpty(childCircuitName) ? "" : childCircuitName;
                    }
                    else if (textElement.Text == PGEPSPSNameCaption)
                    {
                        textElement.Text = pspsName;
                    }
                    else if (textElement.Text == PGETotalMileageCaption)
                    {
                        if (!string.IsNullOrEmpty(totalMileages))
                        {
                            textElement.Text = string.Format("Segment OH Primary Length - {0} Miles", totalMileages);
                        }
                        else
                        {
                            textElement.Text = "";
                        }
                    }
                }
            }

            graphicsContainer.Reset();
            return element;
        }

        /// <summary>
        /// Moves the specified file to its final location.  Deletes any existing file if it exists.
        /// </summary>
        private void MovePDFToFinalLocation(string tempDirectory, string newDirectory)
        {
            string tempFilename = tempDirectory + "\\" + this.MapId;
            string newFileName = newDirectory + "\\" + this.MapId;

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


        #endregion Public Methods

        #region Private Methods

        private bool IsInvalid()
        {
            bool isInvalid = false;
            isInvalid = string.IsNullOrEmpty(this.MxdLocation) || string.IsNullOrEmpty(this.PdfMxdFileName);
            isInvalid = isInvalid || string.IsNullOrEmpty(this.MapId);
            isInvalid = isInvalid || string.IsNullOrEmpty(this.PdfOutputfileLocation);
            return isInvalid;
        }

        private IMap SetExtentsAndGetIMapToPlot(IMapDocument mapDocument, IEnvelope envelope, bool isLandscape)
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
            //Loop through and set the extent to smaller map frames by expanding the Actual Envelope 
            if (largestMapFrame != null)
            {
                //round the map scale to 10'
                //IElement mapElement = (IElement)pMapFrame;

                double mapExtentFeet = envelope.Width;
                double distMapDistanctPerFoot = Math.Round((mapExtentFeet / width) / 10, 0) * 10;
                if (envelope.Height / height > envelope.Width / width)
                {
                    distMapDistanctPerFoot = Math.Round((envelope.Height / height) / 10, 0) * 10;
                }
                if (isLandscape)
                {
                    distMapDistanctPerFoot = Math.Round((envelope.Width / height) / 10, 0) * 10;
                    if (envelope.Height / width > envelope.Width / height)
                    {
                        distMapDistanctPerFoot = Math.Round((envelope.Height / width) / 10, 0) * 10;
                    }
                }
                // make the minimum scale as 50'
                if (distMapDistanctPerFoot < 50)
                {
                    distMapDistanctPerFoot = 50;
                }
                _logger.Debug(string.Format("screen size: {0} {1}", width, height));
                _logger.Debug("map scale: " + distMapDistanctPerFoot.ToString());
                // recalculate extent
                _logger.Debug(string.Format("old extent: {0} {1}", envelope.Width, envelope.Height));
                double centerX = (envelope.XMin + envelope.XMax) / 2;
                double centerY = (envelope.YMin + envelope.YMax) / 2;
                double newWidth = distMapDistanctPerFoot * width;
                double newHeight = distMapDistanctPerFoot * height;
                envelope.PutCoords(centerX - newWidth / 2.0, centerY - newHeight / 2.0, centerX + newWidth / 2.0, centerY + newHeight / 2.0);
                _logger.Debug(string.Format("new extent: {0} {1}", newWidth, newHeight));

                ((IActiveView)((IMapFrame)largestMapFrame).Map).Extent = envelope;
                retVal = ((IMapFrame)largestMapFrame).Map;
                SetMaintenancePlatDefQuery(retVal, this.CircuitId);
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

        /// <summary>
        /// Currently hard coded to meet PGE Requirements.
        /// </summary>
        /// <param name="map"></param>
        private void SetMaintenancePlatDefQuery(IMap map, string circuitId)
        {
            string defQuery = MapProductionConfigurationHandler.GetSettingValue("DefinitionQuery");
            if (string.IsNullOrEmpty(defQuery)) defQuery="";

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
                    ITableDefinition tableDef = (ITableDefinition)featLayer;

                    if (this.ChildCircuitIds != "")
                    {
                        string allCircuitIds = String.Format("'{0}'", String.Join("','", this.ChildCircuitIds.Split(',')));
                        if (!this.ChildCircuitIds.Contains(circuitId))
                        {
                            allCircuitIds = String.Format("'{0}',{1}", circuitId, allCircuitIds);
                        }
                        if (ds.Name.ToUpper() == "PrimaryOH_FIA".ToUpper())
                        {
                            tableDef.DefinitionExpression = string.Format("EDGIS.PRIOHCONDUCTOR.CIRCUITID in ({0})", allCircuitIds);
                        }
                        else if (ds.BrowseName.ToUpper() == "EDGIS.FUSE".ToUpper())
                        {
                            if (ds.Name == "fuse Segment")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Fuse.PSPS_CODE = 'Segment' AND (EDGIS.Fuse.CIRCUITID in ({0}) OR EDGIS.Fuse.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                            else if (ds.Name == "fuse Open")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Fuse.PSPS_CODE = 'Open' AND (EDGIS.Fuse.CIRCUITID in ({0}) OR EDGIS.Fuse.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                            else
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Fuse.PSPS_CODE = 'N/A' AND (EDGIS.Fuse.CIRCUITID in ({0}) OR EDGIS.Fuse.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                        }
                        else if (ds.BrowseName.ToUpper() == "EDGIS.SWITCH".ToUpper())
                        {
                            if (ds.Name == "switch_SEGMENT")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Switch.STATUS <> 0 AND EDGIS.Switch.PSPS_CODE = 'Segment' AND (EDGIS.Switch.CIRCUITID in ({0}) OR EDGIS.Switch.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                            else if (ds.Name == "switch Open")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Switch.PSPS_CODE = 'Open' AND (EDGIS.Switch.CIRCUITID in ({0}) OR EDGIS.Switch.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                            else
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Switch.STATUS <> 0 AND EDGIS.Switch.PSPS_CODE = 'N/A' AND (EDGIS.Switch.CIRCUITID in ({0}) OR EDGIS.Switch.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                        }
                        else if (ds.BrowseName.ToUpper() == "EDGIS.DYNAMICPROTECTIVEDEVICE".ToUpper())
                        {
                            if (ds.Name == "Dynamic Protective Device_SEGMENT")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.DynamicProtectiveDevice.PSPS_CODE = 'Segment' AND (EDGIS.DynamicProtectiveDevice.CIRCUITID in ({0}) OR EDGIS.DynamicProtectiveDevice.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                            else if (ds.Name == "Dynamic Protective Device Open")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.DynamicProtectiveDevice.PSPS_CODE = 'Open' AND (EDGIS.DynamicProtectiveDevice.CIRCUITID in ({0}) OR EDGIS.DynamicProtectiveDevice.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                            else
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.DynamicProtectiveDevice.PSPS_CODE = 'N/A' AND (EDGIS.DynamicProtectiveDevice.CIRCUITID in ({0}) OR EDGIS.DynamicProtectiveDevice.CIRCUITID2 in ({0}))", allCircuitIds);
                            }
                        }
                    }
                    else
                    {
                        if (ds.Name.ToUpper() == "PrimaryOH_FIA".ToUpper())
                        {
                            tableDef.DefinitionExpression = string.Format("EDGIS.PRIOHCONDUCTOR.CIRCUITID = '{0}'", circuitId);
                        }
                        else if (ds.BrowseName.ToUpper() == "EDGIS.FUSE".ToUpper())
                        {
                            if (ds.Name == "fuse Segment")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Fuse.PSPS_CODE = 'Segment' AND (EDGIS.Fuse.CIRCUITID = '{0}' OR EDGIS.Fuse.CIRCUITID2 = '{0}')", circuitId);
                            }
                            else if (ds.Name == "fuse Open")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Fuse.PSPS_CODE = 'Open' AND (EDGIS.Fuse.CIRCUITID = '{0}' OR EDGIS.Fuse.CIRCUITID2 = '{0}')", circuitId);
                            }
                            else
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Fuse.PSPS_CODE = 'N/A' AND (EDGIS.Fuse.CIRCUITID = '{0}' OR EDGIS.Fuse.CIRCUITID2 = '{0}')", circuitId);
                            }
                        }
                        else if (ds.BrowseName.ToUpper() == "EDGIS.SWITCH".ToUpper())
                        {
                            if (ds.Name == "switch_SEGMENT")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Switch.STATUS <> 0 AND EDGIS.Switch.PSPS_CODE = 'Segment' AND (EDGIS.Switch.CIRCUITID = '{0}' OR EDGIS.Switch.CIRCUITID2 = '{0}')", circuitId);
                            }
                            else if (ds.Name == "switch Open")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Switch.PSPS_CODE = 'Open' AND (EDGIS.Switch.CIRCUITID = '{0}' OR EDGIS.Switch.CIRCUITID2 = '{0}')", circuitId);
                            }
                            else
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.Switch.STATUS <> 0 AND EDGIS.Switch.PSPS_CODE = 'N/A' AND (EDGIS.Switch.CIRCUITID = '{0}' OR EDGIS.Switch.CIRCUITID2 = '{0}')", circuitId);
                            }
                        }
                        else if (ds.BrowseName.ToUpper() == "EDGIS.DYNAMICPROTECTIVEDEVICE".ToUpper())
                        {
                            if (ds.Name == "Dynamic Protective Device_SEGMENT")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.DynamicProtectiveDevice.PSPS_CODE = 'Segment' AND (EDGIS.DynamicProtectiveDevice.CIRCUITID = '{0}' OR EDGIS.DynamicProtectiveDevice.CIRCUITID2 = '{0}')", circuitId);
                            }
                            else if (ds.Name == "Dynamic Protective Device Open")
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.DynamicProtectiveDevice.PSPS_CODE = 'Open' AND (EDGIS.DynamicProtectiveDevice.CIRCUITID = '{0}' OR EDGIS.DynamicProtectiveDevice.CIRCUITID2 = '{0}')", circuitId);
                            }
                            else
                            {
                                tableDef.DefinitionExpression = string.Format("EDGIS.DynamicProtectiveDevice.PSPS_CODE = 'N/A' AND (EDGIS.DynamicProtectiveDevice.CIRCUITID = '{0}' OR EDGIS.DynamicProtectiveDevice.CIRCUITID2 = '{0}')", circuitId);
                            }
                        }
                    }
                    _logger.Debug(string.Format("Layer: {0} - {1}", ds.Name, tableDef.DefinitionExpression));
                }
                layer = enumLayer.Next();
            }

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
                    if (ds.BrowseName.ToUpper() == featureClassName.ToUpper() || featLayer.Name.ToUpper() == featureClassName)
                    {
                        featLayers.Add(featLayer);
                    }
                }
                layer = enumLayer.Next();
            }
            return featLayers;
        }
        
        #endregion Private Methods

    }
}
