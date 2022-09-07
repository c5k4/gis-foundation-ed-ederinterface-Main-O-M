using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using ESRI.ArcGIS.Geometry;

namespace PGE.BatchApplication.PSPSMapProduction
{
    /// <summary>
    /// Prepares Data for the export operation
    /// </summary>
    public class PSPSSegmentRecord
    {
        #region Public Fields

        public string CircuitId = "";
        public string CircuitName = "";
        public string ChildCircuitName = "";
        public string PSPSSegmentName = "";
        public double TotalMilage = 0.0;
        public IEnvelope Extent;
        public int LayoutIndex = 1; // 1: Portray; 2: Landscape
        public int MapId = 0; // map index
        public int MapType = 2; // 1: Overview Map; 2: Segment Map
        public bool deleted = false; // used to remove the child circuit
        public string FeederCircuitId = "";

        #endregion public fields
    }
}
