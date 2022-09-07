using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Miner.Framework;

namespace PGE.Desktop.EDER.UFM.Operations
{
    public class CreateButterflyAnno : Operation
    {
        public CreateButterflyAnno()
        {
            base.Name = "Create Duct Bank Annotation";
            base.Description = "Creates conductor annotation text for duct banks on butterfly diagrams";
            this.Status = OperationStatus.Tool;
        }
    }
}
