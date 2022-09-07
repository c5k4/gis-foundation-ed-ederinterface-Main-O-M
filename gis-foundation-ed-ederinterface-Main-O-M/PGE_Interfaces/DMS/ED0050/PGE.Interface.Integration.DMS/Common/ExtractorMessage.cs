using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.Interface.Integration.DMS.Common
{
    /// <summary>
    /// Data class containing input parameters for processing circuits
    /// </summary>
    public class ExtractorMessage
    {
        private extracttype _extractType;
        private int _numberOfDays;
        private List<string> _aor;
        private List<string> _includeCircuits;
        private List<string> _excludeCircuits;
        private string _serverName;
        private List<string> _substations;
        private string _processID;

        /// <summary>
        /// Initialize the ExtractorMessage data structures
        /// </summary>
        public ExtractorMessage()
        {
            _aor = new List<string>();
            _includeCircuits = new List<string>();
            _excludeCircuits = new List<string>();
            _substations = new List<string>();
        }

        /// <summary>
        /// The ID of the process. Typically a GUID
        /// </summary>
        public string ProcessID
        {
            get { return _processID; }
            set { _processID = value; }
        }
        /// <summary>
        /// A list of the substation IDs to process
        /// </summary>
        public List<string> Substations
        {
            get { return _substations; }
            set { _substations = value; }
        }
        /// <summary>
        /// The connection for the server where the staging schema resides. This is only for debugging. To see where a process is saving data
        /// </summary>
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }
        /// <summary>
        /// Functionality descoped. Was supposed to be for a list of circuits not to process
        /// </summary>
        public List<string> ExcludeCircuits
        {
            get { return _excludeCircuits; }
            set { _excludeCircuits = value; }
        }
        /// <summary>
        /// A list of ED circuits to extract. Only present when the ExtractType is Batch
        /// </summary>
        public List<string> IncludeCircuits
        {
            get { return _includeCircuits; }
            set { _includeCircuits = value; }
        }

        /// <summary>
        /// Functionality descoped. Was supposed to be for dividing the batches into the 8 DMS Areas of Responsibility
        /// </summary>
        public List<string> AOR
        {
            get { return _aor; }
            set { _aor = value; }
        }
        /// <summary>
        /// Functionality descoped. Was supposed to be for
        /// </summary>
        public int NumberOfDays
        {
            get { return _numberOfDays; }
            set { _numberOfDays = value; }
        }
        /// <summary>
        /// The type of extract to perform
        /// </summary>
        public extracttype ExtractType
        {
            get { return _extractType; }
            set { _extractType = value; }
        }
    }
    /// <summary>
    /// The type of extract to perform
    /// </summary>
    public enum extracttype
    {
        /// <summary>
        /// Extract the substations and ed circuits that are in the changes table
        /// </summary>
        Changes = 0,
        /// <summary>
        /// Extract all of the substations and ed circuits
        /// </summary>
        Bulk = 1,
        /// <summary>
        /// Extract a specific set of substations and/or circuits
        /// </summary>
        Batch = 2,
        /// <summary>
        /// Dump the data currently in the staging schema to the CSV files
        /// </summary>
        Dump = 3
    }
}
