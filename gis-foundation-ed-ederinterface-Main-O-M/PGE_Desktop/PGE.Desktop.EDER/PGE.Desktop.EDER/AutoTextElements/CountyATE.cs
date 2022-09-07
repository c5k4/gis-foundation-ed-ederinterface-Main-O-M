using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using PGE.Common.Delivery.Framework;


namespace PGE.Desktop.EDER
{
    /// <summary>
    /// County AutoTextElement
    /// </summary>
    [Guid("B2B93C3D-9B09-4C96-A2F1-D3F9CF63AF4A")]
    [ProgId("PGE.Desktop.EDER.CountyATE")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class CountyATE : BasePointInPolyATE
    {
        #region Constructor
        /// <summary>
        /// Initializes the new instance of <see cref="CountyATE"/>
        /// </summary>
        public CountyATE()
            : base("PGE.Desktop.EDER.CountyATE", " ", MapProductionFacade.PGECountyCaption, MapProductionFacade.PGECountyCaption, null, "County", null, "CNTY_NAME")
        { }
        #endregion Constructor
    }
}
