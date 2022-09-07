using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Telvent.PGE.MapProduction
{
    /// <summary>
    /// Interface to implement for identifying change detection information that should be used by Map Production Tool Set
    /// </summary>
    public interface IMPChangeDetector
    {
        /// <summary>
        /// Gets a list of changedkeys. The Key format should match the Format that is used by IMPMapLookUpTable interface
        /// </summary>
        /// <returns>Returns a list of strings that represent a Key value to identify a map from the Map Look Up table or the Geodatabase </returns>
        List<string> GetChangedKey();
        /// <summary>
        /// Method to clear all the records in Map Production ChangeDeteciton table
        /// </summary>
        /// <returns>Returns true if the records were cleared successfully.</returns>
        bool ClearAllChanges();
        /// <summary>
        /// Method to add a record to ChangeDetection table
        /// </summary>
        /// <param name="gdbData"></param>
        /// <returns></returns>
        bool MarkAsChanged(IMPGDBData gdbData);
    }
}
