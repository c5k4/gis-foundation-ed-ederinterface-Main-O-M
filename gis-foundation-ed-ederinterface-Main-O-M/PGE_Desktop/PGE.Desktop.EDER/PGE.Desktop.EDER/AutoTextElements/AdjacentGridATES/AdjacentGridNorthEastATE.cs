using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in North-East direction
    /// </summary>
    [ProgId("PGE.Desktop.EDER.AdjacentGridNorthEastATE")]
    [Guid("A5E00E3C-0EEB-48BD-A8F8-1F547567D07A")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridNorthEastATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridNorthEastATE"/>
        /// </summary>
        public AdjacentGridNorthEastATE() :
            base("PGE.Desktop.EDER.AdjacentGridNorthEastATE", "PGE Adjacent Grid North-East", "PGE Adjacent Grid North-East", "PGE Adjacent Grid North-East", AdjacentGridDirection.NorthEast) { }
    }
}
