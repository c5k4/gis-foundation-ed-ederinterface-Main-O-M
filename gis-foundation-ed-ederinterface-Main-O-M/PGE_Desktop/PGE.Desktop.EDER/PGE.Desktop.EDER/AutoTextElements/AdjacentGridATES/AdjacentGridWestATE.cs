using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;


namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in West direction
    /// </summary>
    [ProgId("PGE.Desktop.EDER.AdjacentGridWestATE")]
    [Guid("C277243C-62C6-483C-8343-F2F56548771E")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridWestATE:BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridWestATE"/>
        /// </summary>
        public AdjacentGridWestATE() : base("PGE.Desktop.EDER.AdjacentGridWestATE", "PGE Adjacent Grid West", "PGE Adjacent Grid West", "PGE Adjacent Grid West", AdjacentGridDirection.West) { }
    }
}
