using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in South-East direction
    /// </summary>
    [ProgId("PGE.Desktop.EDER.AdjacentGridSouthEastATE")]
    [Guid("44D682D1-8F57-4A29-BE09-A42B7BAFA4C6")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridSouthEastATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridSouthEastATE"/>
        /// </summary>
        public AdjacentGridSouthEastATE() :
            base("PGE.Desktop.EDER.AdjacentGridSouthEastATE", "PGE Adjacent Grid South-East", "PGE Adjacent Grid South-East", "PGE Adjacent Grid South-East", AdjacentGridDirection.SouthEast) { }
    }
}
