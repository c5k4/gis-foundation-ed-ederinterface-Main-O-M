using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in South direction
    /// </summary>
    [ProgId("PGE.Desktop.EDER.AdjacentGridSouthATE")]
    [Guid("6030539D-9031-4E22-8544-53BD1BA96C25")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridSouthATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridSouthATE"/>
        /// </summary>
        public AdjacentGridSouthATE() :
            base("PGE.Desktop.EDER.AdjacentGridSouthATE", "PGE Adjacent Grid South", "PGE Adjacent Grid South", "PGE Adjacent Grid South", AdjacentGridDirection.South) { }
    }
}
