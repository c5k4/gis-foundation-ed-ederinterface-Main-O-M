using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geoprocessing;
using Telvent.Delivery.Systems.Configuration;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace Telvent.PGE.MapProduction.Export
{
    public class PGEGeoProcess
    {
        private string _toolboxName = "PGE Custom Tools.tbx";
        private string _pdfToolName = "ExportPDF_PGE Custom Tools";
        private string _tiffToolName = "ExportTIFF_PGE Custom Tools";
        internal static IGeoProcessor GP = null;
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
        public List<string> ExportPDF()
        {
            List<string> retVal=new List<string>();
            if (GP == null)
            {
                GP = new GeoProcessor();
                SystemRegistry registry = new SystemRegistry();
                GP.AddToolbox(Path.Combine(Path.Combine(registry.Directory, "Toolbox"), _toolboxName));
                GP.OverwriteOutput = true;
            }
            IVariantArray gpParams = new VarArrayClass();
            gpParams.Add(XMin);//Parameter 0 XMin
            gpParams.Add(YMin);//Parameter 1 YMin
            gpParams.Add(XMax);//Parameter 2 XMax
            gpParams.Add(YMax);//Parameter 3 YMax
            gpParams.Add(MapNumber);//Parameter 4 OutputName
            gpParams.Add(MxdLocation);//Parameter 5 MxdLocation
            gpParams.Add(PdfMxdFileName);//Parameter 6 MxdFileName
            gpParams.Add(MapScale);//Parameter 7 MapScale
            gpParams.Add(IncludeAttributesforPDF);//Parameter 8 Include Attributes
            gpParams.Add(300);//Parameter 9 Title
            //gpParams.Add(TiffOutputfileLocation);
            gpParams.Add(PdfOutputfileLocation);//Parameter 10 Output File Location
            IGeoProcessorResult gpResult = GP.Execute("ExportPDF_PGE Custom Tools", gpParams, null);
            IGPMessages gpMessages = gpResult.GetResultMessages();
            for (int i = 0; i < gpMessages.Count; i++)
            {
                if (gpMessages.GetMessage(i).IsError())
                {
                    retVal.Add(gpMessages.GetMessage(i).Description);
                }
            }
            return retVal;
        }
        public List<string> ExportTiff()
        {
            List<string> retVal = new List<string>();
            if (GP == null)
            {
                GP = new GeoProcessor();
                SystemRegistry registry = new SystemRegistry();
                GP.AddToolbox(Path.Combine(Path.Combine(registry.Directory, "Toolbox"), _toolboxName));
                GP.OverwriteOutput = true;
            }
            IVariantArray gpParams = new VarArrayClass();
            gpParams.Add(XMin);
            gpParams.Add(YMin);
            gpParams.Add(XMax);
            gpParams.Add(YMax);
            gpParams.Add(MxdLocation);
            gpParams.Add(TiffMxdFileName);
            gpParams.Add(MapNumber);
            gpParams.Add(MapScale);
            gpParams.Add(string.Empty);
            gpParams.Add(TiffOutputfileLocation);
            //gpParams.Add(PdfOutputfileLocation);
            IGeoProcessorResult gpResult = GP.Execute("ExportTIFF_PGE Custom Tools", gpParams, null);
            IGPMessages gpMessages = gpResult.GetResultMessages();
            for (int i = 0; i < gpMessages.Count; i++)
            {
                if (gpMessages.GetMessage(i).IsError())
                {
                    retVal.Add(gpMessages.GetMessage(i).Description);
                }
            }
            return retVal;
        }
    }
}
