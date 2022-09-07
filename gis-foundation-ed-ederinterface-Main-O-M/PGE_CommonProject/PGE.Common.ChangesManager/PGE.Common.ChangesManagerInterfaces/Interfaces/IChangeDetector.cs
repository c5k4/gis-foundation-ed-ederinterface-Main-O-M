using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.Common.ChangesManagerShared;

namespace PGE.Common.ChangesManagerShared.Interfaces
{
    /// <summary>
    /// Interface for anything reading changes
    /// </summary>
    public interface IChangeDetector
    {
        ChangeDictionary Deletes
        {
            get;
        }
        ChangeDictionary Updates
        {
            get;
        }
        ChangeDictionary Inserts
        {
            get;
        }

        void Read();

        void Cleanup();

        //V3SF - CD API (EDGISREARC-1452) - Added
        void ReadCD(DateTime toDate, DateTime fromDate);

        void ReadCDBatch(DateTime toDate, DateTime fromDate, string TableName, string BatchID);

    }
}
