using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.Delivery.Systems.Data;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.PSPSMapProduction
{
    public interface IMPExporter
    {
        IDatabaseConnection DatabaseConnection
        {
            get;
        }
        IMPDBDataManager DatabaseManager
        {
            get;
        }
        /// <summary>
        /// Returns an instance of PGEMapLookUpTable
        /// </summary>
        IMPMapLookUpTable MapLookUpTable
        {
            get;
        }
        string SuccessMessage
        {
            get;
        }
        List<string> Export(int processId, string circuitId, string circuitName, bool mergePDF);
    }
}
