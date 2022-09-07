using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Process.BaseClasses;
using Miner.Interop.Process;
using System.Runtime.InteropServices;
using Miner.ComCategories;

namespace Telvent.PGE.ED.Desktop.Subtasks
{
    [ComVisible(true)]
    [Guid("B7F77E57-A659-4C53-8D10-BFD1694F67BA")]
    [ProgId("Telvent.PGE.ED.RecordEditedMaps")]
    [ComponentCategory(ComCategory.MMPxSubtasks)]
    public class RecordEditedMaps : BaseVersionDifferenceSubtask
    {
        public RecordEditedMaps()
            : base("PGE Record Modified Map Polygons")
        {

        }

        protected override bool InternalExecute(IMMPxNode pPxNode)
        {
            return true;
            //return base.InternalExecute(pPxNode);
        }
    }
}
