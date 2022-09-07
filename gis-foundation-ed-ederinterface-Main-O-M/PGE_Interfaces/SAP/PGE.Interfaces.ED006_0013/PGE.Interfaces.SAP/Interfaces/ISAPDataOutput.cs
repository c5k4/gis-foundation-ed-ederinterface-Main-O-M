using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interfaces.SAP.Data;

namespace PGE.Interfaces.SAP.Interfaces
{
    /// <summary>
    /// Defines a method for sending data to an external system
    /// </summary>
    public interface ISAPDataOutput
    {
        /// <summary>
        /// Output data to an external system
        /// </summary>
        /// <param name="SAPRowDataList">List of rows containing data to output</param>
        /// <param name="sanitize">If True, clean the data before outputting it</param>
        void OutputData(IList<SAPRowData> SAPRowDataList,bool sanitize);
    }
}
