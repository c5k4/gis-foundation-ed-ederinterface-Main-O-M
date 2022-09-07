using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.Framework;


namespace PGE.Desktop.EDER.AutoTextElements
{
    /// <summary>
    /// Zip AutoTextElement
    /// </summary>
    [Guid("F284E45F-C45C-40F5-952A-4180219F03F8")]
    [ProgId("PGE.Desktop.EDER.AutoTextElements.ZipATE")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class ZipATE : BasePointInPolyATE
    {
        #region Constructor
        /// <summary>
        /// Initializes the new instance of <see cref="ZipATE"/>
        /// </summary>
        public ZipATE()
            : base("PGE.Desktop.EDER.AutoTextElements.ZipATE", " ", MapProductionFacade.PGEZipCaption, MapProductionFacade.PGEZipCaption, null, "ZipCode", null, "ZipCode")
        { }
        #endregion Constructor
    }
}
