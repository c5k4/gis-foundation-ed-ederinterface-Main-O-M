using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Configuration;
using System.Windows.Forms;



namespace PGEReGeoCodingProcess
{
    class ClsLogReports
    {
        private static StreamWriter objTextWriter = null;
        public static string strFilePath = string.Empty;
        private static string strEnd = string.Empty;
        public static bool blnerrors = false;

        /// <summary>
        /// Global variable decleration for application reports
        /// </summary>
        /// <param name="strFile"></param>
      
        private static IList<string> Strlst;

        public static string strLogPath = string.Empty;
        public static IList<string> lstSkipLayers
        {
            get { return Strlst; }
            set { Strlst = value; }
        }
        public static string strStartEnd
        {
            get { return strEnd; }
            set { strEnd = value; }
        }    


        [STAThread]

        public static void createLogfile(string strFile)
        {
            
            try
            {
                //string strLogFilePath = System.Configuration.ConfigurationSettings.AppSettings["LogFilePath"].ToString();
                
                //string strLogFilePath = "C:\\PGEReports\\";
                //string strLogFilePath = clsCommonVariables.strApplicationPath+"Reports";
                strLogPath = clsCommonVariables.strLogsPath;
                // check whether selected path exists or not
                if (Directory.Exists(strLogPath) == false)
                {
                    Directory.CreateDirectory(strLogPath);
                }
                //get current time to make filename
                string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;

                //if the file already exists then append to the existing file
                strFilePath = strLogPath+"\\" + strTime + "_" + strFile;
                objTextWriter = new StreamWriter(strFilePath);
                objTextWriter.Close();
            }
            catch (Exception exce)
            {
                throw new Exception("Error Occured in createLogfile" + exce.Message);
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
                    objTextWriter.WriteLine("Application Executed Successfully..!");
                    objTextWriter.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }

        public static void Common_addColumnToReportTable(System.Data.DataTable ObjReportTable, string strReportFldNames)
        {

            try
            {
                string[] strFldNames = strReportFldNames.Split(',');

                for (int x = 0; x < strFldNames.Length; x++)
                {
                    ObjReportTable.Columns.Add(strFldNames[x]);
                }
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("EXCP@Common_addColumnToReportTable::" + ex.Message);
            }
        }
        public static void Common_addRowstoReportTable(System.Data.DataTable ObjReportTable, string strReportValues)
        {
            try
            {
                DataRow pDataRow = ObjReportTable.NewRow();

                string[] strFldVals = strReportValues.Split(',');

                for (int x = 0; x < strFldVals.Length; x++)
                {
                    pDataRow[x] = strFldVals[x];
                }
                ObjReportTable.Rows.Add(pDataRow);
            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("Error in Common_addRowstoReportTable :: " + ex.Message);
            }
        }
        /// <summary>
        /// Create log file with header info
        /// </summary>
        /// <param name="LogName"></param>
        /// <param name="AppName"></param>
        public static void Common_initSummaryTable(string LogName, string AppName)
        {
            try
            {
                //initialize the datatable
                //Update project details in log file.
                ClsLogReports.createLogfile(LogName + ".log");
                ClsLogReports.writeLine("Project Name :" + "PGE");
                ClsLogReports.writeLine("Application Name :" + AppName);
                ClsLogReports.writeLine("Developed By : Utilities Software Team");
                ClsLogReports.writeLine("Start Date and Time  :" + System.DateTime.Now);
                ClsLogReports.writeLine("******************************************************************************");
            }
            catch (Exception Ex)
            {
                ClsLogReports.writeLine("Error on initialize the summary table method : " + Ex.Message);
            }
        }

        public static void GenerateTheReport(System.Data.DataTable ReportTable, string fineName)
        {
            try
            {
                string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                //string strReporthpath = ConfigurationSettings.AppSettings["LogFilePath"].ToString() + strTime + ".CSV";
               // string strReporthpath = "C:\\PGEReports\\" + strTime+"_"+fineName + ".CSV"; 
                DataTable2CSV(ReportTable, clsCommonVariables.strLogsPath+"\\"+strTime+"_"+ fineName + ".CSV", true);
                ClsLogReports.writeLine(clsCommonVariables.strLogsPath + "\\" + strTime + "_" + fineName + ".CSV");
                //MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                ClsLogReports.writeLine("Error while generating report. Error Details: " + ex.Message.ToString());
            }

        }
        /// <summary>
        /// To create Application log and result report file
        /// </summary>
        /// <param name="ObjReportTable"></param>
        public static void InitializeReports(System.Data.DataTable ObjReportTable, string strLogName, string strAppName, string strReporttableFormat)
        {
            //string strReporttableFormat = string.Empty;
            //Common_initSummaryTable("PIDPA_ImportGDB_App", "PIDPA_ImportGDB");
            Common_initSummaryTable(strLogName, strAppName);

            //DataTable ObjReportTable = new DataTable();
            Common_addColumnToReportTable(ObjReportTable, strReporttableFormat);
        }

        public static void DataTable2CSV(System.Data.DataTable table, string filename, bool blnHeader)
        {
            string seprater = ",";
            DataTable2CSV(table, filename, seprater, blnHeader);
        }

        public static void DataTable2CSV(System.Data.DataTable table, string filename, string sepChar, bool blnHeader)
        {
           // System.IO.StreamWriter writer = default(System.IO.StreamWriter);
            StreamWriter writer = new StreamWriter(filename);
            try
            {
                //writer = new System.IO.StreamWriter(filename);

                // first write a line with the columns name
                string sep = "";
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                if (blnHeader)
                {
                    foreach (System.Data.DataColumn col in table.Columns)
                    {
                        if ("Cleanup" == col.ColumnName)
                            continue;
                        builder.Append(sep).Append(col.ColumnName);
                        sep = sepChar;
                    }
                }
                writer.WriteLine(builder.ToString());

                // then write all the rows
                foreach (DataRow row in table.Rows)
                {
                    sep = "";
                    builder = new System.Text.StringBuilder();

                    foreach (System.Data.DataColumn col in table.Columns)
                    {
                        builder.Append(sep).Append(row[col.ColumnName]);
                        sep = sepChar;
                    }
                    writer.WriteLine(builder.ToString());
                }
            }
            finally
            {
                if ((writer != null)) writer.Close();
            }
        }

        internal static void CloseReports(System.Data.DataTable ObjReportTable, string strCSVname)
        {
            ClsLogReports.close();
            GenerateTheReport(ObjReportTable, strCSVname);           
        }
    }
}
