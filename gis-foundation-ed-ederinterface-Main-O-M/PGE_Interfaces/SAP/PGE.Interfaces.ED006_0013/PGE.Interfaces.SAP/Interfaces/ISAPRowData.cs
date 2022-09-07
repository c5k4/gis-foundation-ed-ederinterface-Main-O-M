using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interfaces.SAP
{
    /// <summary>
    /// RowData that is specific to SAP
    /// </summary>
    public interface ISAPRowData
    {
        /// <summary>
        /// The type of SAP record this RowData represents
        /// </summary>
        SAPType SAPType { get; set; }
        /// <summary>
        /// The type of action SAP should take
        /// </summary>
        ActionType ActionType { get; set; }  
    }
}
