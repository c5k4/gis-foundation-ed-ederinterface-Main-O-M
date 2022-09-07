using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Miner.Framework;

namespace PGE.Desktop.EDER.UFM.Operations
{
    public class SetButterflyFilter : Operation
    {
        public SetButterflyFilter()
        {
            base.Name = "Set Diagram Filter";
            base.Description = "Sets the view filter for butterfly diagrams";
            this.Status = OperationStatus.Tool;
        }
    }
}
