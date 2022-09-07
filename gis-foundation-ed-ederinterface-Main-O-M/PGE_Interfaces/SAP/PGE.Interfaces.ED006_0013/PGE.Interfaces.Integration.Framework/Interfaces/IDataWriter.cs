using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Interfaces.Integration.Framework.Data;

namespace PGE.Interfaces.Integration.Framework
{
    /// <summary>
    /// Defines a method to take in data meant to be written to some source, i.e. csv, xml, database, etc.
    /// </summary>
    public interface IDataWriter
    {
        /// <summary>
        /// Take in data and process it.
        /// </summary>
        /// <param name="data">Data to process</param>
        void OutputData(List<IRowData> data);
    }
}
