using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the adjacent grid in East direction
    /// </summary>
    [Guid("437DBA49-7EE4-41E9-BA2E-DBCBBCAF46F0")]
    [ProgId("PGE.Desktop.EDER.AdjacentGridEastATE")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class AdjacentGridEastATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="AdjacentGridEastATE"/>
        /// </summary>
        public AdjacentGridEastATE() :
            base("PGE.Desktop.EDER.AdjacentGridEastATE", "PGE Adjacent Grid East", "PGE Adjacent Grid East", "PGE Adjacent Grid East", AdjacentGridDirection.East)
        { }
    }
       
}
