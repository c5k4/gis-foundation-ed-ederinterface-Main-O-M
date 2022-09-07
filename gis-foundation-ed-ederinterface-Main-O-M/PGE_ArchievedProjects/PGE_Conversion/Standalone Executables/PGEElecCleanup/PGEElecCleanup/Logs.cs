using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Reflection;
//using Microsoft.Office.Interop.Excel;

namespace PGEElecCleanup
{
    class Logs
    {
        private static StreamWriter objTextWriter = null;
        public static string strFilePath = string.Empty;
        public static bool blnerrors = false;

        public static void createLogfile(string strFile)
        {
            try
            {
                string strLogFilePath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString();

                // check whether selected path exists or not
                if (Directory.Exists(strLogFilePath) == false)
                {
                    Directory.CreateDirectory(strLogFilePath);
                }
                //get current time to make filename
                string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;

                //if the file already exists then append to the existing file
                strFilePath = strLogFilePath + strTime + "_" + strFile;
                objTextWriter = new StreamWriter(strFilePath);
                objTextWriter.Close();
            }
            catch (Exception exce)
            {
                throw new Exception("Error occured in createLogfile" + exce.Message);
            }
         }

        public static string ApplicationPath
        {
            get
            {
                Assembly objAsssembly = Assembly.GetExecutingAssembly();
                return System.IO.Path.GetDirectoryName(objAsssembly.Location);
            }
        }
        public static void writeLine(string strText)
        {
            if (objTextWriter != null)
            {
                objTextWriter = new StreamWriter(strFilePath, true);
                objTextWriter.WriteLine(strText);
                objTextWriter.Close();
            }
        }
        /// <summary>
        /// This method is called while closing of the application 
        /// </summary>
        public static void close()
        {
            try
            {
                if (objTextWriter != null)
                {
                    objTextWriter = new StreamWriter(strFilePath, true);
                    objTextWriter.WriteLine("Successfully terminated...");
                    objTextWriter.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
    }
}
