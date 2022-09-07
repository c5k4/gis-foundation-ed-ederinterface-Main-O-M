using System.Collections.Generic;
using PGE.Interfaces.SAP.Data;

namespace PGE.Interfaces.SAP.Interfaces
{
    /// <summary>
    /// Defines a method for reading data from an external source and outputting it as SAPRowData
    /// </summary>
    public interface ISAPDataReader
    {
        /// <summary>
        /// Read data from an external source
        /// </summary>
        /// <returns>The data formatted into SAPRowData containers</returns>
        List<SAPRowData> ReadData();
    }
}
