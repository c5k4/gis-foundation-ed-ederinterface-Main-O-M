using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Telvent.Delivery.Diagnostics;
using System.Reflection;
using System.IO;

namespace Telvent.PGE.MapProduction.Export
{
    public class PGEExportData
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IMPDBDataManager _dataManager = null;
        private DataTable _coordinateMxdtable = null;
        private DataTable _mapOfficeOutputTable = null;


        public PGEExportData(IMPDBDataManager dataManager)
        {
            _dataManager = dataManager;
        }

        public List<string> ProcessRow(DataRow row,IMPMapLookUpTable mapLookupTable)
        {
            List<string> retVal = new List<string>();
            PGEGeoProcess geoprocess = new PGEGeoProcess();
            geoprocess.XMax = row[mapLookupTable.XMax].ToString();
            geoprocess.YMax = row[mapLookupTable.YMax].ToString();
            geoprocess.XMin = row[mapLookupTable.XMin].ToString();
            geoprocess.YMin = row[mapLookupTable.YMin].ToString();
            geoprocess.IncludeAttributesforPDF = false;
            geoprocess.MapNumber = row[mapLookupTable.MapNumber].ToString();
            geoprocess.PdfMxdFileName = PdfMxdFileName(row);
            geoprocess.TiffMxdFileName = TiffMxdFileName(row);
            geoprocess.MapScale = row[mapLookupTable.MapScale].ToString();
            geoprocess.TiffOutputfileLocation = TiffOutputFileLocation(row);
            geoprocess.PdfOutputfileLocation = PdfOutputFileLocation(row);
            retVal = geoprocess.ExportTiff();
            retVal.AddRange(geoprocess.ExportPDF()); 
            return retVal;
        }


        protected DataTable CoordinateMXDLookUp
        {
            get
            {
                if (_coordinateMxdtable == null)
                {
                    _dataManager.GetLookUpTable(CoordinateMxdLookupDefintiion.TableName); 
                }
                return _coordinateMxdtable;
            }
        }
        protected DataTable MapOfficeOutputLookUp
        {
            get
            {
                if (_mapOfficeOutputTable == null)
                {
                    _dataManager.GetLookUpTable(MapOfficeOutputLookupDefinition.TableName);
                }
                return _mapOfficeOutputTable;
            }
        }


        private string TiffOutputFileLocation(DataRow row)
        {
            return GetOutputDirectory(row, MapOfficeOutputLookupDefinition.TiffLocationFieldName);
        }

        private string PdfOutputFileLocation(DataRow row)
        {
            return GetOutputDirectory(row,MapOfficeOutputLookupDefinition.PdfLocationFieldName);
        }

        private string TiffMxdFileName(DataRow row)
        {
            return GetMxdFileName(row, CoordinateMxdLookupDefintiion.TiffMxdFieldName);
        }

        private string PdfMxdFileName(DataRow row)
        {
            return GetMxdFileName(row, CoordinateMxdLookupDefintiion.PdfMxdFieldName);
        }

        private string GetOutputDirectory(DataRow row,string dirFieldName)
        {
            string retVal = string.Empty;
            string defaultRetVal = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string mapOffice = row[MapCoordLookupOtherFieldDefintiion.MapOffice].ToString();
            DataRow[] result = MapOfficeOutputLookUp.Select(MapOfficeOutputLookupDefinition.MapOfficeFieldName + " = " + mapOffice);
            if (result != null && result.Count() > 0)
            {
                retVal = result[0][dirFieldName] != null ? result[0][dirFieldName].ToString() : string.Empty;
            }
            if (string.IsNullOrEmpty(retVal))
            {
                retVal = defaultRetVal;
                _logger.Warn(dirFieldName+" directory not configured. Defaulting output to: " + retVal);
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
                    _logger.Warn("Unable to create output directory:" + dirFieldName + ". Defaulting to " + retVal);
                }
            }
            return retVal;
        }

        private string GetMxdFileName(DataRow row, string mxdFieldName)
        {
            string retVal = string.Empty;
            string mapOffice = row[MapCoordLookupOtherFieldDefintiion.MapOffice].ToString();
            DataRow[] result = MapOfficeOutputLookUp.Select(MapOfficeOutputLookupDefinition.MapOfficeFieldName + " = " + mapOffice);
            if (result != null && result.Count() > 0)
            {
                retVal = result[0][mxdFieldName] != null ? result[0][mxdFieldName].ToString() : string.Empty;
            }
            return retVal;
        }

        #region PGE Table Definitions
        public struct MapOfficeOutputLookupDefinition
        {
            public const string TableName = "";
            public const string MapOfficeFieldName = "MAPOFFICE";
            public const string TiffLocationFieldName = "";
            public const string PdfLocationFieldName = "";
        }

        public struct CoordinateMxdLookupDefintiion
        {
            public const string TableName = "";
            public const string CoordinateFieldName = "LEGACYCOORDINATE";
            public const string TiffMxdFieldName = "";
            public const string PdfMxdFieldName = "";
        }
        public struct MapCoordLookupOtherFieldDefintiion
        {
            public const string MapOffice = "MAPOFFICE";
            public const string LegacyCoordinate = "LEGACYCOORDINATE";
        }
        #endregion

    }
}
