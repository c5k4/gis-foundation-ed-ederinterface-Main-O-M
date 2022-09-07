using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.AUConveyor.Utilities
{
    /// <summary>
    /// Static reference to the settings and configuration-related constants throughout the application.
    /// </summary>
    internal static class ToolSettings
    {
        /// <summary>
        /// The default input file directory name. This will be used unless the user specifies a new folder name.
        /// </summary>
        public const string InputFileDirDefault = "PGE.BatchApplication.AUConveyor Input files";

        /// <summary>
        /// The prefix for each input file's file name.
        /// </summary>
        public const string InputFilePrefix = "OIDs_";

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
            internal string AuName = null;
            internal string AuProgId = null;
            internal string TableName = null;
            internal string VersionName = null;
            internal string CustomWhereClause = null;
            internal bool BypassAuStoreWhenFieldsEqual = false;
            internal bool UpdateRelatedIfAuNotFound = false;
            internal esriFeatureType? ForceType = null;
            internal List<string> ForceClassNames = new List<string>();
            internal bool RecreateRelated = false;
            internal bool RecreateOnlyWhenExists = false;
            internal int NumProcesses = 1;
            internal bool GenerateInputFilesOnly = false;
            internal bool HiddenProcessing = false;
            internal bool PostVersion = false;
            internal string InputFileDirectory = InputFileDirDefault;
            internal string InputFile = null;
            internal int MaxReconcileAttempts = 1000;
            internal bool SeamlessMode = false;
            internal int SaveRowInterval = 1000;
            internal bool DoNotPostChildVersions = false; // Double-negative

            /// <summary>
            /// Constructor
            /// </summary>
            internal ExecutionProperties()
            {
            }

            /// <summary>
            /// Converts the current settings back into arguments for use in the command line.
            /// </summary>
            /// <returns>A string containing the arguments corresponding to this program's execution.</returns>
            internal string ToArguments()
            {
                return ToArguments(false, null, null);
            }

            /// <summary>
            /// Converts the current settings into arguments used to create child processes.
            /// </summary>
            /// <param name="versionAppend">The number to append onto the base version name (indicates the child version).</param>
            /// <param name="inputFileOverride">
            ///     If this is given a value, the arguments will use this specified input file instead of the one 
            ///     in the current execution's settings.
            /// </param>
            /// <returns>A string containing the arguments to use in the child process's execution.</returns>
            internal string ToArguments(int versionAppend, string inputFileOverride)
            {
                return ToArguments(true, versionAppend, inputFileOverride);
            }

            /// <summary>
            /// Converts the current settings into arguments used either to represent the current execution's arguments
            /// or to create arguments for child processes, depending on the point of entry for this method.
            /// </summary>
            /// <param name="versionAppend">The number to append onto the base version name (indicates the child version).</param>
            /// <param name="inputFileOverride">
            ///     If this is given a value, the arguments will use this specified input file instead of the one 
            ///     in the current execution's settings.
            /// </param>
            /// <param name="isChildProcess">Whether or not this will be used for a chlid process</param>
            /// <returns>A string containing the arguments to use in the desired process's execution.</returns>
            private string ToArguments(bool isChildProcess, int? versionAppend, string inputFileOverride)
            {
                string inputFile = InputFile;
                if (!string.IsNullOrEmpty(inputFileOverride))
                    inputFile = inputFileOverride;

                string version = VersionName;
                if (versionAppend.HasValue)
                {
                    //Strip out any owner from the version name, as the append only exists when child processes are created.
                    //  (child processes may have a different owner depending on the connection, and an explicit owner isn't
                    //   needed because the child was created from the same connection we're passing in.)
                    version = PGE.BatchApplication.AUConveyor.Conveyor.GetAppendedVersionName(version, versionAppend.Value, true);
                }

                //Get all the -k flags.
                string kFlags = string.Empty;
                foreach (string tableName in ForceClassNames)
                    kFlags += "-k \"" + tableName + "\" ";

                return "-c \"" + DatabaseLocation + "\" -v \"" + version + "\" " +
                    (!string.IsNullOrEmpty(AuProgId) ? "-a \"" + AuProgId + "\" " : "") +
                    (!string.IsNullOrEmpty(TableName) ? "-t \"" + TableName + "\" " : "") +
                    (!string.IsNullOrEmpty(CustomWhereClause) && !isChildProcess ? "-w \"" + CustomWhereClause + "\" " : "") +
                    (NumProcesses != 1 && !isChildProcess ? "-e \"" + NumProcesses + "\" " : "") +
                    (GenerateInputFilesOnly && !isChildProcess ? "-g " : "") +
                    (BypassAuStoreWhenFieldsEqual ? "-b " : "") + 
                    (UpdateRelatedIfAuNotFound && ForceType == null && kFlags == string.Empty ? "-f " : "") +
                    (ForceType != null ? "-x \"" + ForceType.ToString() + "\" " : "") +
                    kFlags + 
                    (RecreateRelated ? "-n " : "") +
                    (RecreateOnlyWhenExists ? "-o " : "") +
                    (HiddenProcessing ? "-z " : "") +
                    (SeamlessMode ? "-s " : "") +
                    (SaveRowInterval > 0 ? "-r \"" + SaveRowInterval + "\" " : "") +
                    (PostVersion || (isChildProcess && !DoNotPostChildVersions) ? "-p " : "") +
                    (!string.IsNullOrEmpty(inputFile) ? "-I \"" + inputFile + "\" " : "") +
                    (!string.IsNullOrEmpty(InputFileDirectory) && InputFileDirectory != InputFileDirDefault && !isChildProcess ? "-d \"" + InputFileDirectory+ "\" " : "");
            }
        }
    }
}
