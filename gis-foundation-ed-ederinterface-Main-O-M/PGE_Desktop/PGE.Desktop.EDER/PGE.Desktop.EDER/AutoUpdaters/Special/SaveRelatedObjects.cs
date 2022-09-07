using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

using Miner.Interop;
using Miner.ComCategories;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [ProgId("PGE.Desktop.EDER.SaveRelatedObjects")]
    [Guid("a18105ca-9b1a-409e-aa46-478b20a4f4d9")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]

    public class SaveRelatedObjects : BaseSplitAU
    {

        #region Constuctor

        public SaveRelatedObjects() : base("PGE Save Related Objects") { }

        #endregion

        #region BaseSplitAU Overrides

        #endregion
    }
}



