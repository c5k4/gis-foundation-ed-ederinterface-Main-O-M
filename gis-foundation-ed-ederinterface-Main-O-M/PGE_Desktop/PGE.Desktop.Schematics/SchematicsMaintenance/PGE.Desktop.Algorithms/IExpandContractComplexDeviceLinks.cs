using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PGE.Desktop.SchematicsMaintenance.Algorithms
{
    /// <summary>
    /// Interface to define the properties for the expand/contract complex device links algorithm.
    /// </summary>
    [ComVisible(true)]
    public interface IExpandContractComplexDeviceLinks
    {
        double ExpandContractFactor
        {
            get;
            set;
        }
    }
}
