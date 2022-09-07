using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Telvent.PGE.MapProduction.Export
{
    /// <summary>
    /// Prepares Data for the export operation
    /// </summary>
    public class PGEExportData
    {
        int ProcessID = 1;
        #region Private Fields
        //private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\MapProduction1.0.log4net.config", "MapProduction");
        private IMPDBDataManager _dataManager = null;
        /// <summary>
        /// Stores the PDFMxd and TIFFMxd file names for the given CoordinateSystem and MapScale
        /// </summary>
        private DataTable _coordinateMxdtable = null;
        /// <summary>
        /// Stores the output folder location
        /// </summary>
        private DataTable _mapOfficeOutputTable = null;
        /// <summary>
        /// MXD location retrieved from config file
        /// </summary>
        private string _mxdLocation = null;
        /// <summary>
        /// TIFF DIP retrieved from config file
        /// </summary>
        private int _tiffdpi = 0;
        /// <summary>
        /// TIFF 500 SCALE DPI retrieved from config file
        /// </summary>
        private int _tiff500scaledpi = 0;
        /// <summary>
        /// Process timeout for when a process will be restarted if no maps have been produced
        /// </summary>
        private int _processTimeout = 0;
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
        /// <returns>Processes the export operation and returns errors if any.</returns>
        public List<string> ProcessRow(DataRow row, IMPMapLookUpTable mapLookupTable)
        {
            List<string> retVal = new List<string>();
            try
            {
                DateTime startTime = DateTime.Now;
                PGEMapProcess mapExportProcess = new PGEMapProcess();
                mapExportProcess.XMax = row[mapLookupTable.XMax].ToString();
                mapExportProcess.YMax = row[mapLookupTable.YMax].ToString();
                mapExportProcess.XMin = row[mapLookupTable.XMin].ToString();
                mapExportProcess.YMin = row[mapLookupTable.YMin].ToString();
                mapExportProcess.IncludeAttributesforPDF = IncludeAttributes;
                mapExportProcess.MapNumber = row[mapLookupTable.MapNumber].ToString();
                mapExportProcess.MxdLocation = MXDLocation;
                mapExportProcess.PdfMxdFileName = PdfMxdFileName(row);

                mapExportProcess.TiffMxdFileName = TiffMxdFileName(row);
                mapExportProcess.MapScale = GetMappedMapScale(row[mapLookupTable.MapScale].ToString());                
                //if (mapExportProcess.MapNumber != "30276") return retVal;
                //mapExportProcess.MapScale = "30000";
                //mapExportProcess.PdfMxdFileName = "PDF-0401-25.mxd";
                //if (mapExportProcess.MapNumber != "2927185") return retVal;
                //if (!mapExportProcess.PdfMxdFileName.ToUpper().Equals("PDF-0404-500.mxd".ToUpper())) return retVal;
                mapExportProcess.TiffOutputfileLocation = TiffOutputFileLocation(row);
                mapExportProcess.PdfOutputfileLocation = PdfOutputFileLocation(row);
                mapExportProcess.TiffDPI = TIFFDPI;
                mapExportProcess.Tiff500ScaleDPI = TIFF500SCALEDPI;
                mapExportProcess.PdfDPI = PDFDPI;
                //PGE Specific Harccode the MapOffice field
                mapExportProcess.MapOfficeCode = MapOfficeCode(row);
                //mapExportProcess.MapScale = "50000";
                mapExportProcess.WKTPolygon = row[mapLookupTable.PolygonCoordinates].ToString(); 

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Exporting map with following options");
                sb.AppendLine("=====================================================================");
                sb.AppendLine("MapNumber:" + mapExportProcess.MapNumber);
                sb.AppendLine("XMax:" + mapExportProcess.XMax);
                sb.AppendLine("YMax:" + mapExportProcess.YMax);
                sb.AppendLine("XMin:" + mapExportProcess.XMin);
                sb.AppendLine("YMin:" + mapExportProcess.YMin);
                sb.AppendLine("PDFMxd:" + mapExportProcess.PdfMxdFileName);
                sb.AppendLine("PDFMxd:" + mapExportProcess.TiffMxdFileName);
                sb.AppendLine("MapScale:" + mapExportProcess.MapScale);
                sb.AppendLine("Tiff Otuput Location:" + mapExportProcess.TiffOutputfileLocation);
                sb.AppendLine("PDF Output location:" + mapExportProcess.PdfOutputfileLocation);
                sb.AppendLine("=====================================================================");
                _logger.Debug(sb.ToString());
                long availMem = Process.GetCurrentProcess().VirtualMemorySize64;
                _logger.Debug("Exporting TIFF");
                string exportTIFFSetting = MapProductionConfigurationHandler.GetSettingValue("ExportTIFF").ToUpper();
                exportTIFFSetting = string.IsNullOrEmpty(exportTIFFSetting) ? "TRUE" : exportTIFFSetting;
                if (exportTIFFSetting == "TRUE")
                {
                    if (mapExportProcess.TiffMxdFileName.Length > 0)
                    {
                        retVal = mapExportProcess.ExportTiff(ProcessID);
                    }
                    else
                    {
                        retVal.Add("Tiff mxd file name not available.");
                    }
                }
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
                long memused = Process.GetCurrentProcess().VirtualMemorySize64 - availMem;
                _logger.Debug("Memory Used:" + memused);
                _logger.Debug(mapExportProcess.MapNumber + "-" + mapExportProcess.MapOfficeCode + ":" + memused.ToString());
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
        /// Stores the PDFMxd and TIFFMxd file names for the given CoordinateSystem and MapScale
        /// </summary>
        protected DataTable CoordinateMXDLookUp
        {
            get
            {
                if (_coordinateMxdtable == null)
                {
                    _coordinateMxdtable = _dataManager.GetLookUpTable(CoordinateMxdLookupDefintion.TableName);
                }
                return _coordinateMxdtable;
            }
        }

        /// <summary>
        /// Stores the output folder location
        /// </summary>
        protected DataTable MapOfficeOutputLookUp
        {
            get
            {
                if (_mapOfficeOutputTable == null)
                {
                    _mapOfficeOutputTable = _dataManager.GetLookUpTable(MapOfficeOutputLookupDefinition.TableName);
                }
                return _mapOfficeOutputTable;
            }
        }

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
        /// DIP retrieved from config file
        /// </summary>
        protected int TIFFDPI
        {
            get
            {
                if (_tiffdpi == 0)
                {
                    _tiffdpi = int.TryParse(MapProductionConfigurationHandler.Settings["TIFFDPI"], out _tiffdpi) ? _tiffdpi : 96;
                }
                return _tiffdpi;
            }
        }
        protected int TIFF500SCALEDPI
        {
            get
            {
                try
                {
                    if (_tiff500scaledpi == 0)
                    {
                        _tiff500scaledpi = int.TryParse(MapProductionConfigurationHandler.Settings["TIFF500SCALEDPI"], out _tiff500scaledpi) ? _tiff500scaledpi : 150;
                    }
                }
                catch (Exception e) { _tiff500scaledpi = 150; }
                return _tiff500scaledpi;
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
        /// Gets the TIFF output folder location
        /// </summary>
        /// <param name="row">Row to retrive the information</param>
        /// <returns>Returns the TIFF output folder location</returns>
        private string TiffOutputFileLocation(DataRow row)
        {
            return GetFolderLocation(row, MapOfficeOutputLookupDefinition.TiffLocationFieldName);
        }

        /// <summary>
        /// Gets the PDF output folder location
        /// </summary>
        /// <param name="row">Row to retrive the information</param>
        /// <returns>Returns the PDF output folder location</returns>
        private string PdfOutputFileLocation(DataRow row)
        {
            return GetFolderLocation(row, MapOfficeOutputLookupDefinition.PdfLocationFieldName);
        }

         /// <summary>
        /// Gets the TIFF Mxd file name
        /// </summary>
        /// <param name="row">Row to retrive the information</param>
        /// <returns>Returns the TIFF Mxd file name</returns>
        private string TiffMxdFileName(DataRow row)
        {
            return GetMxdFileName(row, CoordinateMxdLookupDefintion.TiffMxdFieldName);
        }

        /// <summary>
        /// Gets the PDF Mxd file name
        /// </summary>
        /// <param name="row">Row to retrive the information</param>
        /// <returns>Returns the PDF Mxd file name</returns>
        private string PdfMxdFileName(DataRow row)
        {
            return GetMxdFileName(row, CoordinateMxdLookupDefintion.PdfMxdFieldName);
        }

        /// <summary>
        /// Gets the MapOfficeCode for the MapOffice
        /// Used by the ATEs
        /// </summary>
        /// <param name="row">Row to retrive the information</param>
        /// <returns>Returns the MapOfficeCode field value</returns>
        private string MapOfficeCode(DataRow row)
        {
            return GetFieldValueFromMapOfficeLUT(row, MapOfficeOutputLookupDefinition.MapOfficeCode);
        }

        /// <summary>
        /// Gets the folder name as stored in the database. If the returned field value is empty string will return My documents folder.
        /// If the fodler returned from the Table does not exist will create it.
        /// </summary>
        /// <param name="row">From the row gets the directory name as stored in teh database.</param>
        /// <param name="fieldName">Name of the field containing the directory name</param>
        /// <returns>
        /// Returns output directory stored in specified column in the row.
        /// If the retrieved directory is invalid, then User's "My Documents" folder is considered by default.
        /// </returns>
        private string GetFolderLocation(DataRow row, string fieldName)
        {
            string retVal = GetFieldValueFromMapOfficeLUT(row, fieldName);
            string defaultRetVal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (string.IsNullOrEmpty(retVal))
            {
                retVal = defaultRetVal;
                //_logger.Warn(dirFieldName + " directory not configured. Defaulting output to: " + retVal);
            }
            else if (!Directory.Exists(retVal))
            {
                try
                {
                    Directory.CreateDirectory(retVal);
                }
                catch (Exception ex)
                {
                    retVal = defaultRetVal;
                    //_logger.Warn("Unable to create output directory:" + dirFieldName + ". Defaulting to " + retVal);
                }
            }
            return retVal;
        }
        /// <summary>
        /// Gets the field value stored in specified column in the row
        /// </summary>
        /// <param name="row">Row to retrieve the information</param>
        /// <param name="fieldName">Column Name storing the directory path</param>
        /// <returns>
        /// string value as stored in the Database
        /// </returns>
        private string GetFieldValueFromMapOfficeLUT(DataRow row, string fieldName)
        {
            string retVal = string.Empty;
            string mapOffice = row[MapCoordLookupOtherFieldDefintion.MapOffice].ToString();
            DataRow[] result = MapOfficeOutputLookUp.Select(MapOfficeOutputLookupDefinition.MapOfficeFieldName + " = '" + mapOffice + "'");
            if (result != null && result.Count() > 0)
            {
                retVal = result[0][fieldName] != null ? result[0][fieldName].ToString() : string.Empty;
            }
            return retVal;
        }

        /// <summary>
        /// Gets the MXD file name directory stored in specified column in the row
        /// </summary>
        /// <param name="row">Row to retrieve the information</param>
        /// <param name="dirFieldName">Column Name storing the MXD file name</param>
        /// <returns>
        /// Returns the MXD file name directory stored in specified column in the row
        /// </returns>
        private string GetMxdFileName(DataRow row, string mxdFieldName)
        {
            string retVal = string.Empty;
            string coordinateSystem = row[MapCoordLookupOtherFieldDefintion.LegacyCoordinate].ToString();
            string mapScale = row[MapCoordLookupOtherFieldDefintion.MapScale].ToString();
            string selectClause = CoordinateMxdLookupDefintion.CoordinateFieldName + " = '" + coordinateSystem + "' and " + CoordinateMxdLookupDefintion.MapScaleFieldName + "=" + mapScale;
            DataRow[] result = CoordinateMXDLookUp.Select(selectClause);
            if (result != null && result.Count() > 0)
            {
                retVal = result[0][mxdFieldName] != null ? result[0][mxdFieldName].ToString() : string.Empty;
            }
            return retVal;
        }

        /// <summary>
        /// Returns the Mapped Scale value for MapType value in the MaintenancePolygon feature
        /// </summary>
        /// <param name="originalScale">Value retrieved from MapType field</param>
        /// <returns>Returns the Mapped Scale value for MapType value in the MaintenancePolygon feature</returns>
        private string GetMappedMapScale(string originalScale)
        {
            int mappedScale = 0;
            string settingValue = null;
            //Get the Mapped scale value for the original scale value from settings file
            if (!string.IsNullOrEmpty(originalScale) && MapProductionConfigurationHandler.Settings.ContainsKey(originalScale))
            {
                settingValue = MapProductionConfigurationHandler.Settings[originalScale];
                _logger.Debug(string.Format("Mapped scale value for MapType = {0} is {1}", originalScale, settingValue));
                int.TryParse(settingValue, out mappedScale);
            }
            //Return the default value
            else if (MapProductionConfigurationHandler.Settings.ContainsKey("DEFAULTSCALE"))
            {
                settingValue = MapProductionConfigurationHandler.Settings["DEFAULTSCALE"];
                _logger.Debug(string.Format("Mappped scale for MapType ={0} is not found. Using Mapped scale Default value : {1}", originalScale, settingValue));
                int.TryParse(settingValue, out mappedScale);
            }

            return mappedScale.ToString();
        }

        #endregion Private Methods

        #region PGE Table Definitions
        public struct MapOfficeOutputLookupDefinition
        {
            public static string TableName = string.IsNullOrEmpty(MapProductionConfigurationHandler.GetSettingValue("TableOwner")) ? "PGE_MAPOFFICEFOLDERLUT" : MapProductionConfigurationHandler.GetSettingValue("TableOwner") + ".PGE_MAPOFFICEFOLDERLUT";
            public const string MapOfficeFieldName = "MAPOFFICE";
            public const string TiffLocationFieldName = "TIFFFOLDER";
            public const string PdfLocationFieldName = "PDFFOLDER";
            public const string MapOfficeCode = "MAPOFFICECODE";
        }

        public struct CoordinateMxdLookupDefintion
        {
            public static string TableName = string.IsNullOrEmpty(MapProductionConfigurationHandler.GetSettingValue("TableOwner")) ? "PGE_COORDMXDLUT" : MapProductionConfigurationHandler.GetSettingValue("TableOwner") + ".PGE_COORDMXDLUT";
            public const string CoordinateFieldName = "COORDSYSTEM";
            public const string TiffMxdFieldName = "TIFFMXD";
            public const string PdfMxdFieldName = "PDFMXD";
            public const string MapScaleFieldName = "MAPSCALE";
        }
        public struct MapCoordLookupOtherFieldDefintion
        {
            public const string MapOffice = "MAPOFFICE";
            public const string LegacyCoordinate = "LEGACYCOORDINATE";
            public const string MapScale = "MAPSCALE";
        }
        #endregion
    }
}
