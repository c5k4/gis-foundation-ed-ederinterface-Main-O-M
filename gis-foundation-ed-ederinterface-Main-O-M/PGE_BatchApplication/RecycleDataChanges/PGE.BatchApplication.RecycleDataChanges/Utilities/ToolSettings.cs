using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.RecycleDataChanges.Utilities
{
    /// <summary>
    /// Static reference to the settings and configuration-related constants throughout the application.
    /// </summary>
    internal static class ToolSettings
    {
        /// <summary>
        /// The default input file directory name. This will be used unless the user specifies a new folder name.
        /// </summary>
        public static readonly string XmlFileDefault = "ChangesExport_" + DateTime.Now.ToFileTime() + ".xml";

        private static ExecutionProperties _instance = null;
        /// <summary>
        /// The singleton instance of the program's settings.
        /// </summary>
        internal static ExecutionProperties Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExecutionProperties();

                return _instance;
            }
        }

        /// <summary>
        /// Resets the program's settings.
        /// </summary>
        internal static void ResetSettings()
        {
            _instance = null;
        }

        /// <summary>
        /// The class used to hold all program execution settings used by the application,
        /// and the logic to convert them back into command line arguments where necessary.
        /// </summary>
        internal class ExecutionProperties
        {
            internal string DatabaseLocation = null;
            internal string VersionSourceName = null;
            internal string VersionTargetName = null;
            internal string VersionLoadName = null;
            internal string XmlFileLocation = XmlFileDefault;

            internal bool ImportFileEnabled = false;
            internal bool ExportFileEnabled = false;

            /// <summary>
            /// Constructor
            /// </summary>
            internal ExecutionProperties()
            {
            }
        }
    }
}
