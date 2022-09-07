using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geoprocessing;
using PGE.Common.Delivery.Systems.Configuration;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.PSPSMapProduction.Export
{
    /// <summary>
    /// Executes the GPProcess by calling a model in toolbox
    /// </summary>
    public class PGEGeoProcess
    {
        #region Private Fields
        /// <summary>
        /// Toolbox Name
        /// </summary>
        private string _toolboxName = "PGECustomTools.tbx";
        /// <summary>
        /// Export to PDF tool name
        /// </summary>
        private string _pdfToolName = "ExportPDF";
        /// <summary>
        /// Object used in executing the GeoProcess
        /// </summary>
        internal static IGeoProcessor GP = null;
        #endregion Private Fields

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
        public int DPI
        {
            get;
            set;
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Executes the Export to PDF operation
        /// </summary>
        /// <returns>Returns list of errors, if any, while executing the operation</returns>
        public List<string> ExportPDF()
        {
            List<string> retVal = new List<string>();
            if (GP == null)
            {
                GP = new GeoProcessor();
                SystemRegistry registry = new SystemRegistry();
                GP.AddToolbox(Path.Combine(Path.Combine(registry.Directory, "PSPSToolbox"), _toolboxName));
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
            gpParams.Add(IncludeAttributesforPDF);//Parameter 8 Include Attributes
            gpParams.Add(DPI);//Parameter 9 DPI
            gpParams.Add(PdfOutputfileLocation);//Parameter 10 Output File Location
            IGeoProcessorResult gpResult = GP.Execute(_pdfToolName, gpParams, null);
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

        #endregion Public Methods
    }
}
