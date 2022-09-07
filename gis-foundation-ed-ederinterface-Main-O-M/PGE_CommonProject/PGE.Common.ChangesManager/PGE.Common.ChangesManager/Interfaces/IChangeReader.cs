using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.ChangesManager;

namespace PGE.ChangesManager.Interfaces
{
    /// <summary>
    /// Interface for anything reading changes
    /// </summary>
    public interface IChangeReader
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
    }
}
