using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in North direction
    /// </summary>
    [ProgId("PGE.Desktop.EDER.AdjacentGridNorthATE")]
    [Guid("C2172BDC-8380-4FBF-AB7A-F6FC755F3898")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridNorthATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridNorthATE"/>
        /// </summary>
        public AdjacentGridNorthATE() :
            base("PGE.Desktop.EDER.AdjacentGridNorthATE", "PGE Adjacent Grid North", "PGE Adjacent Grid North", "PGE Adjacent Grid North", AdjacentGridDirection.North) { }
    }
}
