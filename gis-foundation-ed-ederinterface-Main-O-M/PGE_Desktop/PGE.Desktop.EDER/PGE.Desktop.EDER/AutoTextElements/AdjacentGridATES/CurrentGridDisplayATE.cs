using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// AutoText element for displaying the Primary Display Field value of the current grid
    /// </summary>
    [Guid("DF4ACB2E-FAA1-43D1-82A7-240FA8501C99")]
    [ProgId("PGE.Desktop.EDER.CurrentGridDisplayATE")]
    [ComponentCategory(ComCategory.MMCustomTextSources)]
    public class CurrentGridDisplayATE : BaseAdjacentGridATE
    {
        /// <summary>
        /// Initializes new instance of <see cref="CurrentGridDisplayATE"/>
        /// </summary>
        public CurrentGridDisplayATE() :
            base("PGE.Desktop.EDER.CurrentGridDisplayATE", "PGE Map Grid Primary Display Field", "PGE Map Grid Primary Display Field", "PGE Map Grid Primary Display Field", AdjacentGridDirection.None)
        { }
    }

}