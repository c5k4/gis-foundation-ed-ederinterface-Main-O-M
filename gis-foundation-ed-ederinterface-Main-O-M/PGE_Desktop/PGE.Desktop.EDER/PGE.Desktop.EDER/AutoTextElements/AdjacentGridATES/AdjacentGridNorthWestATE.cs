using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in North-West direction
    /// </summary>
    [ProgId("PGE.Desktop.EDER.AdjacentGridNorthWestATE")]
    [Guid("5C753991-79A8-4E97-986B-A9305AABD0C5")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridNorthWestATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridNorthWestATE"/>
        /// </summary>
        public AdjacentGridNorthWestATE() :
            base("PGE.Desktop.EDER.AdjacentGridNorthWestATE", "PGE Adjacent Grid North-West", "PGE Adjacent Grid North-West", "PGE Adjacent Grid North-West", AdjacentGridDirection.NorthWest) { }
    }
}
