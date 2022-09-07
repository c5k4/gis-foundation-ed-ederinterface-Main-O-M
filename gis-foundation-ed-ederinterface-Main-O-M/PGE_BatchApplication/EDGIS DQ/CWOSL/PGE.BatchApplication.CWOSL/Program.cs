using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CommandFlags;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor;
using System.Reflection;
using System.IO;
//using Telvent.Delivery.Diagnostics;
using PGE.BatchApplication.CWOSL.Interfaces;
using System.Xml.Linq;
using Miner.Interop;
using PGE.Common.ChangesManagerShared;

namespace PGE.BatchApplication.CWOSL
{
    /// <summary>
    /// Entry point for Change Detection execution.
    /// </summary>
    public class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);


        #region Private Static Members

        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static Log4NetLoggerCWOSL _logger;
        private static IWindsorContainer _container;

        public const string LogFileDirectory = "CWOSL Log Files";
        private const string logFileName = "CWOSL";
        private static IMMAppInitialize _MmAppInit;
        private static mmLicensedProductCode _MmLicense;
        private static bool arcfmLicenseInitialized = false;
        //ME Q1 : 2020 varriable for : number of records to be processed 
        public static int noOfRecords = 0;
        public static int programRumTime = 0;
        //1368195  START
        public static string HFTD = string.Empty;
        public static string Division = string.Empty;
        public static string County = string.Empty;
        public static string LocalOfficeID = string.Empty;
        public static string SeconderyLoadPoint = string.Empty;
        public static string TransformerSubtype = string.Empty;
        public static string SketchPseudoserviceFeatures = string.Empty;
        public static string FULLPROCESS = string.Empty;
        public static string CIRCUITID = string.Empty;
        //1368195   END
        #endregion

        #region Main Execution Methods

        public static void Main(string[] args)
        {
            SetLogger(args);
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            Initialize.AppStartDateTime = DateTime.Now;
            OutputVersion();
            if (args.Length == 0)
            {
                Console.Title = "CWOSL Process";
            }
            DateTime exeStart = DateTime.Now;


            try
            {
                if (args.Length == 1)
                {
                    if (args[0].ToUpper().Equals("POST"))
                    {
                        //ESRI License Initializer generated code.
                        if( m_AOLicenseInitializer == null)
                            m_AOLicenseInitializer = new LicenseInitializer();
                        if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                        new esriLicenseExtensionCode[] { }))
                        {
                            _logger.Info(m_AOLicenseInitializer.LicenseMessage());
                            throw new ErrorCodeException(ErrorCode.LicenseCheckout, "This application could not initialize with the correct ArcGIS license and will shutdown.");
                        }
                        else
                        {  //initialize arcfm license
                            if (arcfmLicenseInitialized == true || (arcfmLicenseInitialized == false && InitializeArcFM(ToArcFM("ArcFM"))))
                            {

                                //Method will reconcile and post the version of this process and delte the session
                                ReconcileAndPostVersion();
                                return;
                            }

                            else
                            {
                                _logger.Info("ArcFM License failed to initialize.");
                                throw new ErrorCodeException(ErrorCode.LicenseCheckout, "This application could not initialize with the correct ArcFM license and will shutdown.");
                            }
                        }
                    }

                }

                if (args.Length > 1)
                {
                    //1368195 START
                    HFTD = Convert.ToString(args[0]);
                    Division = Convert.ToString(args[1]);
                    County = Convert.ToString(args[2]);
                    LocalOfficeID = Convert.ToString(args[3]);
                    SeconderyLoadPoint = Convert.ToString(args[4]);
                    TransformerSubtype = Convert.ToString(args[5]);
                    SketchPseudoserviceFeatures = Convert.ToString(args[6]);
                    noOfRecords = Convert.ToInt32(args[7]);
                    programRumTime = Convert.ToInt32(args[8]);
                    FULLPROCESS = Convert.ToString(args[9]);
                    CIRCUITID = Convert.ToString(args[10]);
                }
                //1368195 END
                //Report start time.
                _logger.Info("Process started: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
                _logger.Info("");

                //ESRI License Initializer generated code.
                if (m_AOLicenseInitializer == null)
                    m_AOLicenseInitializer = new LicenseInitializer();
                if (!m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { }))
                {
                    _logger.Info(m_AOLicenseInitializer.LicenseMessage());
                    throw new ErrorCodeException(ErrorCode.LicenseCheckout, "This application could not initialize with the correct ArcGIS/ArcFM license and will shutdown.");
                }
                else
                {
                    //initialize arcfm license
                    if (arcfmLicenseInitialized == true || (arcfmLicenseInitialized == false && InitializeArcFM(ToArcFM("ArcFM"))))
                    {
                        RunProcess();
                    }

                    else
                    {
                        _logger.Info("ArcFM License failed to initialize.");
                        throw new ErrorCodeException(ErrorCode.LicenseCheckout, "This application could not initialize with the correct ArcFM license and will shutdown.");
                    }
                }

            }
            catch (ErrorCodeException ece)
            {
                WriteError(ece);
                Environment.ExitCode = ece.CodeNumber;
            }
            catch (Exception ex)
            {
                //Format this error as an ErrorCodeException and throw it the same way (becomes a general error).
                ErrorCodeException ece = new ErrorCodeException(ex);
                WriteError(ece);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                WriteEndStatus(exeStart);
                if (m_AOLicenseInitializer != null) m_AOLicenseInitializer.ShutdownApplication();
                m_AOLicenseInitializer = null;
                _logger = null;
                if (_container != null) _container.Dispose();
                _container = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                // Rolling Versions appears to occasionally lock the process, leaving it unable to exit
                // The exit code should have been set so as the last line in the program this should be safe to execute
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press return to exit");
                    Console.ReadLine();
                }
                if (args.Length == 0)
                    TerminateProcess(Process.GetCurrentProcess().Handle, Convert.ToUInt32(Environment.ExitCode));
            }
        }

        /// Converts the string version of the arc fm product code into the enumeration.
        /// </summary>
        /// <param name="arcfm">The string version of the mmLicenseProductCode either with or without the mmLP.</param>
        /// <returns>mmLicensedProductCode converted; mmLPArcFM otherwise.</returns>
        public static mmLicensedProductCode ToArcFM(string arcfm)
        {
            foreach (string prod in Enum.GetNames(typeof(mmLicensedProductCode)))
            {
                if (string.Compare(prod, arcfm, true) == 0)
                    return (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), prod);

                string value = prod.Replace("mmLP", "");
                if (string.Compare(value, arcfm, true) == 0)
                    return (mmLicensedProductCode)Enum.Parse(typeof(mmLicensedProductCode), prod);
            }

            return mmLicensedProductCode.mmLPArcFM;
        }

        /// <summary>        
        /// Attempts to checkout a license for the specified Miner and Miner product.
        /// </summary>
        /// <param name="prodCode">mmLicenseProductCode to check out.</param>
        /// <returns>True if successfull; false otherwise.</returns>    
        public static bool InitializeArcFM(mmLicensedProductCode prodCode)
        {
            _MmAppInit = new MMAppInitializeClass();

            if (_MmAppInit == null)
            {
                //EventLogger.Warn("Warning! Unable to initialize ArcFM.  No licenses can be checked out.");
                return false;
            }

            // Determine if the product license is available or is already checked out
            mmLicenseStatus mmlicenseStatus = _MmAppInit.IsProductCodeAvailable(prodCode);
            if (mmlicenseStatus == mmLicenseStatus.mmLicenseCheckedOut)
            {
                return true;
            }

            if (mmlicenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                mmlicenseStatus = _MmAppInit.Initialize(prodCode);
                if (mmlicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                {
                    //Trace.WriteLine("Warning! A license cannot be checked out for M&M product " + prodCode);
                    //EventLogger.Warn("Warning! A license cannot be checked out for M&M product " + prodCode); 
                    return false;
                }

                _MmLicense = prodCode;
                arcfmLicenseInitialized = true;
                //EventLogger.Log("Checking out ArcFM License: " + prodCode, EventLogEntryType.Information);

                return true;
            }

            //EventLogger.Warn("Warning! No license is available for M&M product " + prodCode); 

            return false;
        }
        private string GetOutline(int indentLevel, XElement element)
        {
            StringBuilder result = new StringBuilder();

            if (element.Attribute("name") != null)
            {
                result = result.AppendLine(new string(' ', indentLevel * 2) + element.Attribute("name").Value);
            }

            foreach (XElement childElement in element.Elements())
            {
                result.Append(GetOutline(indentLevel + 1, childElement));
            }

            return result.ToString();
        }

        public static void OutputVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.ProductVersion;
            _logger.Info("Version [ " + version + " ]");
        }

        protected static void SetLogger(string[] args)
        {
            string configFile = "";
            var flags = new FlagParser()
                {
                    { "c", "config", true, false, "The config file to load", f => configFile = f },
                };
            flags.Parse(args);

            if (!String.IsNullOrEmpty(configFile))
            {
                string log4netFile = "CWOSL.log4net." + configFile;
                _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, log4netFile);
            }

            if (_logger == null)
            {
                _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config");
            }
            else if (args.Count() >= 1) //if CWOSL run from CWOSL Assignment UI tool
            {
                _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config", true);
            }

        }

        public static void RunProcess()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            string configFile = "CWOSL.config";

            LoadConfig(configFile);
            RunCWOSL();
        }

        protected static void LoadConfig(string configFile)
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            _logger.Debug("Loading config file from [ " + configFile + " ]");
            Console.WriteLine("Loading config file from [ " + configFile + " ]");

            if (!File.Exists(configFile))
            {
                throw new ArgumentException("Config file doesn't exist! [ " + configFile + " ]");
            }
            _container = new WindsorContainer(new XmlInterpreter(configFile));

        }
        public static void RunCWOSL()
        {

            try
            {
                IExtractTransformLoad csosEtl = _container.Resolve<IExtractTransformLoad>();

                csosEtl.ProcessEtl();
            }
            catch (Castle.MicroKernel.ComponentNotFoundException)
            {
                _logger.Debug("Not running ETL (not set in configuration)");
            }
        }

        public static void ReconcileAndPostVersion()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
            string configFile = "CWOSL.config";

            LoadConfig(configFile);
            IExtractTransformLoad csosEtl = _container.Resolve<IExtractTransformLoad>();
            csosEtl.ReconcileAndPostVersion();

        }


        public static void WriteEndStatus(DateTime exeStart)
        {
            //Report end time.
            DateTime exeEnd = DateTime.Now;
            _logger.Info("");
            _logger.Info("Completed");
            _logger.Info("Process start time: " + exeStart.ToLongDateString() + " " + exeStart.ToLongTimeString());
            _logger.Info("Process end time: " + exeEnd.ToLongDateString() + " " + exeEnd.ToLongTimeString());
            _logger.Info("Process length: " + (exeEnd - exeStart).ToString());
            _logger.Info("");

#if DEBUG
            //Console.Write("Press any key to exit.");
            //Console.ReadKey();
#endif

        }


        #endregion

        #region Public Static Helper Methods

        /// <summary>
        /// Opens an SDE connection file from a file name and loads it into an IWorkspace.
        /// </summary>
        /// <param name="sFile">The file path of the desired SDE connection file.</param>
        /// <returns>An IWorkspace object from the connection.</returns>
        public static ESRI.ArcGIS.Geodatabase.IWorkspace IWorkspace_OpenFromFile(string sFile)
        {
            IWorkspaceFactory workspaceFactory = new ESRI.ArcGIS.DataSourcesGDB.SdeWorkspaceFactory();
            return workspaceFactory.OpenFromFile(sFile, 0);
        }

        /// <summary>
        /// Writes a message to the console and the log.
        /// </summary>
        /// <param name="status">The message/status to write.</param>
        /// <param name="writeToConsole">The message/status to write.</param>
        public static void WriteStatus(string status, bool writeToConsole)
        {
            status = DateTime.Now.ToString() + ": " + status;

            _logger.Info(status);
        }

        /// <summary>
        /// Writes exception details to the console and the log.
        /// </summary>
        /// <param name="ece">The exception about which to write details. All errors should be wrapped in the <see cref="ErrorCodeException"/> class.</param>
        public static void WriteError(ErrorCodeException ece)
        {
            string tempString = DateTime.Now.ToString() + ": " + "**** Error encountered  ****: ";
            tempString += "[" + ece.CodeNumber + "] ";
            _logger.Error(tempString);
            _logger.Error(ece.ToString());
            Console.WriteLine(tempString);//enable only for testing phase.
        }

        /// <summary>
        /// Writes exception details to the console and the log.
        /// </summary>
        /// <param name="ex">The exception about which to write details. These exceptions should be considered warnings that do not break execution.</param>
        public static void WriteWarning(Exception ex)
        {
            string tempString = DateTime.Now.ToString() + ": " + "**** Error encountered  ****: ";
            _logger.Warn(tempString);
            _logger.Warn(ex.ToString());
        }

        public static string GetLogFilePath()
        {
            try
            {
                if (_logger == null)
                {
                    _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config");
                }
                return _logger.GetLogFilePath();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string[] GetNoFeat_RunTime()
        {
            try
            {
                string configFile = "CWOSL.config";
                string noOfFeature = "";
                string progRunTime = "";
                if (!File.Exists(configFile))
                {
                    return (new string[] { "", "" });
                }
                var container = new WindsorContainer(new XmlInterpreter(configFile));
                IExtractTransformLoad csosEtl = container.Resolve<IExtractTransformLoad>();
                noOfFeature = Convert.ToString(((PGE.BatchApplication.CWOSL.CWOSL)(csosEtl)).InitializeProcess.NumberOfJobs);
                progRunTime = Convert.ToString(((PGE.BatchApplication.CWOSL.CWOSL)(csosEtl)).InitializeProcess.MaxMinutesToRun);
                return (new string[] { noOfFeature, progRunTime });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}