using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.ChangesManagerShared;

namespace PGE.ChangesManagerShared.Interfaces
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
    }
}
