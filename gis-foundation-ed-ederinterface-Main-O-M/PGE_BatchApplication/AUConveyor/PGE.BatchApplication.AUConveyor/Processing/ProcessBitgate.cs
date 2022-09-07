using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGE.BatchApplication.AUConveyor.Processing
{
    /// <summary>
    /// Combines multiple boolean settings into a single integer value, used in the exit codes of the process.
    /// </summary>
    public class ProcessBitgate
    {
        private bool _deleteVersion = false;
        public bool DeleteVersion
        {
            get { return _deleteVersion; }
            set
            {
                _deleteVersion = value;
                if (UpdatedBitgateAction != null)
                    UpdatedBitgateAction(this);
            }
        }

        private bool _warningsEncountered = false;
        public bool WarningsEncountered
        {
            get { return _warningsEncountered; }
            set
            {
                _warningsEncountered = value;
                if (UpdatedBitgateAction != null)
                    UpdatedBitgateAction(this);
            }
        }

        private bool _errorsEncountered = false;
        public bool ErrorsEncountered
        {
            get { return _errorsEncountered; }
            set
            {
                _errorsEncountered = value;
                if (UpdatedBitgateAction != null)
                    UpdatedBitgateAction(this);
            }
        }

        Action<ProcessBitgate> UpdatedBitgateAction = null;

        public int Bitgate
        {
            get
            {
                return
                      Convert.ToInt32(DeleteVersion)
                    | Convert.ToInt32(WarningsEncountered) << 1
                    | Convert.ToInt32(ErrorsEncountered) << 2;
            }
            set
            {
                DeleteVersion = (value & 1) > 0;
                WarningsEncountered = (value & 1 << 1) > 0;
                ErrorsEncountered = (value & 1 << 2) > 0;
            }
        }

        public ProcessBitgate(int bitgate)
        {
            Bitgate = bitgate;
        }

        public ProcessBitgate(bool deleteVersion, bool warningsEncountered, bool errorsEncountered)
        {
            DeleteVersion = deleteVersion;
            WarningsEncountered = warningsEncountered;
            ErrorsEncountered = errorsEncountered;
        }

        private static ProcessBitgate _thisExitBitgate = null;
        public static ProcessBitgate ThisExitBitgate
        {
            get
            {
                if (_thisExitBitgate == null)
                {
                    _thisExitBitgate = new ProcessBitgate(Environment.ExitCode);
                    _thisExitBitgate.UpdatedBitgateAction = delegate(ProcessBitgate pb) { Environment.ExitCode = pb.Bitgate; };
                }

                return _thisExitBitgate;
            }
            set
            {
                _thisExitBitgate = value;
                Environment.ExitCode = value.Bitgate;
            }
        }
    }
}
