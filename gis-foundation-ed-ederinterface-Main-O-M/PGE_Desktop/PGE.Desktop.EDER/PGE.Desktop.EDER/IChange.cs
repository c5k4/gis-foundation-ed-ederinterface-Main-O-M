using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.Desktop.EDER
{
    /// <summary>
    /// An interface for objects which track changes.
    /// </summary>
    public interface IChange
    {
        /// <summary>
        /// The altered table.
        /// </summary>
        ITable ChangeTable { get; }
        /// <summary>
        /// The workspace the change was made in.
        /// </summary>
        IWorkspace Workspace { set; }
        /// <summary>
        /// Adds a change to the object.
        /// </summary>
        /// <param name="d8List">A list of changes.</param>
        void AddChange(ID8List d8List);
        /// <summary>
        /// Tests whether the changes should be persisted.
        /// </summary>
        /// <returns>True if the changes should be persisted; otherwise false.</returns>
        bool PersistChange();
        /// <summary>
        /// Gets an Boolean indicating whether the change was at the feature level only.
        /// </summary>
        bool FeatureOnly{get;}
    }
}
