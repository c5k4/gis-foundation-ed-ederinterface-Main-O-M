using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TLM.Common.Interfaces
{
    public interface ITransformerPeakCustTypeInfo
    {
        string CustomerType { get; set; }
        string Season { get; set; }
        decimal Qty { get; set; }
        decimal KVA { get; set; }

    }
}
