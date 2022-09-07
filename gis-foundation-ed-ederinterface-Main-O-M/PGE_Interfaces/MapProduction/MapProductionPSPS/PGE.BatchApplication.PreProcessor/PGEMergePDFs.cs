using System;
using System.IO;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Diagnostics;


namespace PGE.BatchApplication.PSPSMapProduction.PreProcessor
{
    /// <summary>
    /// This class had methods to export the Map to PDF and TIFF formats
    /// </summary>
    public class PGEMergePDFs
    {
        private static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "\\PGE.BatchApplication.PSPSMapProduction1.0.log4net.config", "PSPSMapProduction");

        private string _pythonExePath = "";

        #region Private Properties

        /// <summary>
        /// Python Executable path
        /// </summary>
        protected string PythonExePath
        {
            get
            {
                if (_pythonExePath == "")
                {
                    _pythonExePath = MapProductionConfigurationHandler.Settings["PythonExePath"];
                }
                return _pythonExePath;
            }
        }

        #endregion Private Properties

        #region Public Methods


        /// <summary>
        /// Executes the Export to PDF operation
        /// </summary>
        /// <returns>Returns list of errors, if any, while executing the operation</returns>
        public void MergePDF(string mergePdfPy, string fileLocation)
        {
            ProcessStartInfo start = new ProcessStartInfo();

            try
            {
                start.FileName = this.PythonExePath;
                start.Arguments = string.Format("\"{0}\" \"{1}\"", mergePdfPy, fileLocation);
                _logger.Debug(start.Arguments);
                start.UseShellExecute = false;
                start.RedirectStandardOutput = true;
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        _logger.Debug("Merge Results: " + result);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("Error merging PDF files: " + fileLocation + "; ", ex);
            }

        }
        #endregion Public Methods
    }
}
