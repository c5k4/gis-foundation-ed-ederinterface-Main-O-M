using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in South-West direction
    /// </summary>
    [ProgId("PGE.Desktop.EDER.AdjacentGridSouthWestATE")]
    [Guid("0AC2602D-FE27-43E5-90BB-1A55093C9CC5")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridSouthWestATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridSouthWestATE"/>
        /// </summary>
        public AdjacentGridSouthWestATE() :
            base("PGE.Desktop.EDER.AdjacentGridSouthWestATE", "PGE Adjacent Grid South-West", "PGE Adjacent Grid South-West", "PGE Adjacent Grid South-West", AdjacentGridDirection.SouthWest) { }
    }
}
