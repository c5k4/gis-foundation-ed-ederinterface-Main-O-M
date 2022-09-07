using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace PGE.BatchApplication.PSPSMapProduction.Export
{
    /// <summary>
    /// Prepares Data for the export operation
    /// </summary>
    public class PGEExportData
    {
        int ProcessID = 1;
        #region Private Fields
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");
        private IMPDBDataManager _dataManager = null;

        /// <summary>
        /// MXD location retrieved from config file
        /// </summary>
        private string _mxdLocation = null;
        /// <summary>
        /// PDF location retrieved from config file
        /// </summary>
        private string _pdfLocation = null;
        /// <summary>
        /// PDF DIP retrieved from config file
        /// </summary>
        private int _pdfdpi = 0;
        /// <summary>
        /// Wheter to include Attributes while exporting attributes. This is retrieved from config file
        /// </summary>
        private bool? _includeAttributes = null;
        #endregion Private Fields

        #region Constructor

        /// <summary>
        /// Initialises the new instance of <see cref="PGEExportData"/>
        /// </summary>
        /// <param name="dataManager"></param>
        public PGEExportData(IMPDBDataManager dataManager, int processID)
        {
            this.ProcessID = processID;
            _dataManager = dataManager;
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Process the row for the export operation
        /// </summary>
        /// <param name="row">Row to be processed for export operation</param>
        /// <param name="mapLookupTable">Table with required column names</param>
        /// <param name="tmpPDFFilePath">Temporary PDF File path</param>
        /// <param name="childCircuitIds">Child Circuit Ids separated by commas - for overview map</param>
        /// <returns>Processes the export operation and returns errors if any.</returns>
        public List<string> ProcessRow(DataRow row, IMPMapLookUpTable mapLookupTable, string tmpPDFFilePath, string childCircuitIds)
        {
            List<string> retVal = new List<string>();
            try
            {
                DateTime startTime = DateTime.Now;
                double totalMileages = double.Parse(row[mapLookupTable.TotalSegmentLength].ToString());
                PGEMapProcess mapExportProcess = new PGEMapProcess();
                mapExportProcess.XMax = row[mapLookupTable.XMax].ToString();
                mapExportProcess.YMax = row[mapLookupTable.YMax].ToString();
                mapExportProcess.XMin = row[mapLookupTable.XMin].ToString();
                mapExportProcess.YMin = row[mapLookupTable.YMin].ToString();
                mapExportProcess.IncludeAttributesforPDF = IncludeAttributes;
                mapExportProcess.MapId = row[mapLookupTable.MapId].ToString();
                mapExportProcess.CircuitId = row[mapLookupTable.CircuitId].ToString();
                string feederCircuitId = row[mapLookupTable.FeederCircuitId].ToString();
                string childCircuitName = row[mapLookupTable.ChildCircuitName].ToString();
                mapExportProcess.CircuitName = row[mapLookupTable.CircuitName].ToString();
                mapExportProcess.ChildCircuitName = row[mapLookupTable.ChildCircuitName].ToString();
                mapExportProcess.PSPSName = row[mapLookupTable.PSPSName].ToString();
                mapExportProcess.TotalMileage = (totalMileages > 0.0)? totalMileages.ToString("0.##") : "";
                mapExportProcess.LayoutType = int.Parse(row[mapLookupTable.LayoutType].ToString());
                mapExportProcess.MxdLocation = MXDLocation;
                mapExportProcess.PdfMxdFileName = GetMxdFileName(mapExportProcess.LayoutType);
                mapExportProcess.PdfOutputfileLocation = tmpPDFFilePath;
                mapExportProcess.PdfDPI = PDFDPI;
                mapExportProcess.ChildCircuitIds = childCircuitIds;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Exporting map with following options");
                sb.AppendLine("=====================================================================");
                sb.AppendLine("MapNumber:" + mapExportProcess.MapId.ToString());
                sb.AppendLine("XMax:" + mapExportProcess.XMax);
                sb.AppendLine("YMax:" + mapExportProcess.YMax);
                sb.AppendLine("XMin:" + mapExportProcess.XMin);
                sb.AppendLine("YMin:" + mapExportProcess.YMin);
                sb.AppendLine("PDFMxd:" + mapExportProcess.PdfMxdFileName);
                sb.AppendLine("PDF Output location:" + mapExportProcess.PdfOutputfileLocation);
                sb.AppendLine("=====================================================================");
                _logger.Debug(sb.ToString());
                long availMem = Process.GetCurrentProcess().VirtualMemorySize64;
                _logger.Debug("Exporting PDF");
                string exportPDFSetting = MapProductionConfigurationHandler.GetSettingValue("ExportPDF").ToUpper();
                exportPDFSetting = string.IsNullOrEmpty(exportPDFSetting) ? "TRUE" : exportPDFSetting;
                if (exportPDFSetting == "TRUE")
                {
                    if (mapExportProcess.PdfMxdFileName.Length > 0)
                    {
                        retVal.AddRange(mapExportProcess.ExportPDF());
                    }
                    else
                    {
                        retVal.Add("Pdf mxd file name not available.");
                    }
                }
                mapExportProcess = null;
                long memused = Process.GetCurrentProcess().VirtualMemorySize64 - availMem;
                _logger.Debug("Memory Used:" + memused);
                _logger.Debug(mapExportProcess.MapId.ToString() + ":" + memused.ToString());
            }
            catch (Exception ex)
            {
                retVal.Add(ex.Message + Environment.NewLine + ex.StackTrace);
                _logger.Error("Failed processing:", ex);
            }

            return retVal;
        }

        #endregion Public Methods

        #region Protected Properties


        /// <summary>
        /// MXD location retrieved from config file
        /// </summary>
        protected string MXDLocation
        {
            get
            {
                if (_mxdLocation == null)
                {
                    _mxdLocation = MapProductionConfigurationHandler.Settings["MxdLocation"];
                }
                return _mxdLocation;
            }
        }

        /// <summary>
        /// MXD location retrieved from config file
        /// </summary>
        public string PDFLocation
        {
            get
            {
                if (_pdfLocation == null)
                {
                    _pdfLocation = MapProductionConfigurationHandler.Settings["PDFLocation"];
                }
                return _pdfLocation;
            }
        }

        protected int PDFDPI
        {
            get
            {
                if (_pdfdpi == 0)
                {
                    _pdfdpi = int.TryParse(MapProductionConfigurationHandler.Settings["PDFDPI"], out _pdfdpi) ? _pdfdpi : 300;
                }
                return _pdfdpi;
            }
        }
        /// <summary>
        /// Wheter to include Attributes while exporting attributes. This is retrieved from config file
        /// </summary>
        protected bool IncludeAttributes
        {
            get
            {
                if (_includeAttributes == null)
                {
                    bool value;
                    _includeAttributes = bool.TryParse(MapProductionConfigurationHandler.Settings["IncludeAttributes"], out value) ? value : false;
                }
                return _includeAttributes.Value;
            }

        }

        #endregion Protected Properties

        #region Private Methods

        /// <summary>
        /// Gets the MXD file name directory stored in specified column in the row
        /// </summary>
        /// <param name="row">Row to retrieve the information</param>
        /// <param name="dirFieldName">Column Name storing the MXD file name</param>
        /// <returns>
        /// Returns the MXD file name directory stored in specified column in the row
        /// </returns>
        private string GetMxdFileName(int layoutType)
        {
            string retVal = "Portrait Segment Map.mxd";
        
            if (layoutType == MapLayoutType.PortraitOverviewMap)
            {
                retVal = "Portrait Overview Map.mxd";
            }
            else if (layoutType == MapLayoutType.LandscapeSegmentMap)
            {
                retVal = "Landscape Segment Map.mxd";
            }
            else if (layoutType == MapLayoutType.LandscapeOverviewMap)
            {
                retVal = "Landscape Overview Map.mxd";
            }
            return retVal;
        }


        #endregion Private Methods

    }
}
