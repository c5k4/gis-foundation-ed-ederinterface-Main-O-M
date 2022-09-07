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
    /// Division AutoTextElement
    /// </summary>
    [Guid("35E36EB2-22BE-4D88-A789-A011B2CD8C01")]
    [ProgId("PGE.Desktop.EDER.DivisionATE")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class DivisionATE : BasePointInPolyATE
    {
        #region Constructor
        /// <summary>
        /// Initializes the new instance of <see cref="DivisionATE"/>
        /// </summary>
        public DivisionATE()
            : base("PGE.Desktop.EDER.DivisionATE", " ", MapProductionFacade.PGEDivisionCaption, MapProductionFacade.PGEDivisionCaption, null, "Division", null, "Division")
        { }
        #endregion Constructor
    }
}
