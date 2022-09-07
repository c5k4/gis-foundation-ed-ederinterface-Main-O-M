using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PGE.BatchApplication.CircuitProtectionZoneCL
{
    public class Args
    {
        public string ConnectionString { get; set; }
        public string LandbaseConnectionFile { get; set; }
        public string SubstationConnectionFile { get; set; }
        public string DbOwner { get; set; }
        public string ResultFeatureClass { get; set; }
        public string FireTierFeatureClass { get; set; }
        public string LogLevel { get; set; }
        public string LogDir { get; set; }
        public string LogName { get; set; }
        public string ReportType { get; set; }
        public string ReportFileLocation { get; set; }
        public string ReportFileName { get; set; }

        private string _numProcesses = "0";
        public string NumProcesses
        {
            get { return _numProcesses; }
            set { _numProcesses = value; }
        }

        private string _processID = "-1";
        public string ProcessID
        {
            get { return _processID; }
            set { _processID = value; }
        }

        public static Args Create(string[] args)
        {
            IDictionary<string, string> argsDictionary = CreateArgsDictionary(args);

            string logDir = FindArg("-logdir", argsDictionary, ".\\");
            if (logDir.EndsWith(@"\") == false)
            {
                logDir = logDir + "\\";
            }

            Args Args = new Args
            {
                SubstationConnectionFile = FindRequiredArg("-S", argsDictionary),
                LandbaseConnectionFile = FindRequiredArg("-L", argsDictionary),
                ConnectionString = FindRequiredArg("-c", argsDictionary),
                ProcessID = FindArg("-pid", argsDictionary),
                NumProcesses = FindArg("-NumProcesses", argsDictionary),
                DbOwner = FindArg("-o", argsDictionary),
                FireTierFeatureClass = FindArg("-f", argsDictionary),
                ResultFeatureClass = FindArg("-r", argsDictionary),
                LogLevel = FindArg("-loglevel", argsDictionary, "Info", new[] { "Info", "Error", "Debug" }),
                LogDir = logDir,
                LogName = FindArg("-logname", argsDictionary, "CpzCommandLine.log"),
                ReportType = FindArg("-reporttype", argsDictionary, "ProtectionZone", new[] { "ProtectionZone", "EnergizedZone" }),
                ReportFileLocation = FindArg("-reportlocation", argsDictionary, "c:\\temp"),
                ReportFileName = FindArg("-reportname", argsDictionary, "EnergizedCircuitReport"),
            };

            return Args;
        }

        private static int FindArgAsInt(string key, IDictionary<string, string> argsDictionary, int defaultValue)
        {
            int returnVal = defaultValue;

            try
            {
                string valAsString = argsDictionary[key];
                int.TryParse(valAsString, out returnVal);
            }
            catch
            {
            }

            return returnVal;
        }

        private static readonly string[] BooleanValidValues = new[] { "true", "false" };

        private static bool FindArgAsBool(string key, IDictionary<string, string> argsDictionary, bool defaultValue)
        {
            bool returnVal = defaultValue;
            try
            {
                string valAsString = argsDictionary[key];

                if (BooleanValidValues.FirstOrDefault(item => item == valAsString) != null)
                {
                    bool.TryParse(valAsString, out returnVal);
                }
            }
            catch
            {
                returnVal = defaultValue;
            }

            return returnVal;
        }

        private static string FindRequiredArg(string key, IDictionary<string, string> argsDictionary)
        {
            try
            {
                return argsDictionary[key];
            }
            catch
            {
                string message = string.Format("Required argument {0} not supplied.", key);
                throw new ArgsException(message);
            }
        }

        private static string FindArg(string key, IDictionary<string, string> argsDictionary)
        {
            string returnVal;
            try
            {
                returnVal = argsDictionary[key];
            }
            catch
            {
                returnVal = string.Empty;
            }

            return returnVal;
        }

        private static string FindArg(string key, IDictionary<string, string> argsDictionary, string defaultValue, params string[] validValues)
        {
            string returnVal;
            try
            {
                returnVal = argsDictionary[key];

                if (validValues.Length > 0 && validValues.FirstOrDefault(item => item == returnVal) == null)
                {
                    returnVal = defaultValue;
                }
            }
            catch
            {
                returnVal = defaultValue;
            }

            return returnVal;
        }

        private static IDictionary<string, string> CreateArgsDictionary(string[] args)
        {
            IDictionary<string, string> argsDictionary = new Dictionary<string, string>();

            for (int i = 0; i < args.Length; i++)
            {
                string key = args[i];
                if (++i < args.Length)
                {
                    string value = args[i];
                    if (string.IsNullOrEmpty(value) == false)
                    {
                        argsDictionary[key] = value;
                    }
                }
            }

            return argsDictionary;
        }

        public void Validate()
        {
            if (File.Exists(ConnectionString) == false)
            {
                throw new ArgsException("Connection file (" + ConnectionString + ") does not exist or you do not have the correct permissions to access.");
            }


            //if (File.Exists(FireTierFeatureClass) == false)
            //{
            //    throw new ArgsException("Fire tier feature class (" + FireTierFeatureClass + ") does not exist or you do not have the correct permissions to access.");
            //}

            //if (File.Exists(ResultFeatureClass) == false)
            //{
            //    throw new ArgsException("Result feature class (" + ResultFeatureClass + ") already exists. Please specify a new file.");
            //}

            if (Directory.Exists(LogDir) == false)
            {
                throw new ArgsException("Log Dir (" + LogDir + ") does not exist or you do not have the correct permissions to access.");
            }
        }
    }
}
