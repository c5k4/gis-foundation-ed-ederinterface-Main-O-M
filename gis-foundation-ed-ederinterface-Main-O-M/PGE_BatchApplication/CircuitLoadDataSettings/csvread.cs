using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using Oracle.DataAccess.Client;
using System.Configuration;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
namespace PGE.BatchApplication.LoadData
{
    public class csvread
    {
        //public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionStr"].ConnectionString;
        public static string ConnectionString = string.Empty;
        public static string userID = ConfigurationManager.AppSettings["UserID"];

        private string SetConnectionString()
        {
            string returnStr = string.Empty;
            string userPass = default;
            try
            {
                userPass = ConfigurationManager.ConnectionStrings["ConnectionStr"].ConnectionString;
                returnStr = PGE_DBPasswordManagement.ReadEncryption.GetConnectionStr(userPass);
                returnStr = returnStr + "PERSIST SECURITY INFO=True;";
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return returnStr;
        }

        //CSV or excle file  convert to datatable  and join both files
        public bool ProcessCsvData(string csv_file_path1, string csv_file_path2, string csv_file_path3)
        {
            try
            {
                Console.Write("CSV to datatable conversion started.");
                System.Data.DataTable dtPeakLoad = new System.Data.DataTable();
                System.Data.DataTable dtFeederPeak = new System.Data.DataTable();
                System.Data.DataTable dtFrmExcel = new System.Data.DataTable();
                System.Data.DataTable dtResult = new System.Data.DataTable();
                dtResult.Columns.Add("NETWORKID", typeof(string));
                dtResult.Columns.Add("Date", typeof(string));
                //dtResult.Columns.Add("MONTH", typeof(string));
                //dtResult.Columns.Add("YEAR", typeof(string));
                //dtResult.Columns.Add("TIME", typeof(string));
                dtResult.Columns.Add("KW PEAK TOTAL", typeof(string));
                dtResult.Columns.Add("KW PROJECTED", typeof(double));

                dtResult.Columns.Add("dbo_tbl_division.str_name", typeof(string));
                dtResult.Columns.Add("dbo_tbl_feeder.str_name", typeof(string));
                dtResult.Columns.Add("int_year", typeof(string));
                dtResult.Columns.Add("int_external_id", typeof(string));
                dtResult.Columns.Add("MW_PEAK", typeof(string));
                dtResult.Columns.Add("CONVERTED_KW", typeof(string));
                dtResult.Columns.Add("dtm_peak_date", typeof(string));
                dtResult.Columns.Add("flt_normal_cap_amps", typeof(string));

                dtPeakLoad = csvread.GetDataTabletFromCSVFile(csv_file_path1);
                dtFeederPeak = csvread.GetDataTabletFromCSVFile(csv_file_path2);
                //if data is provided in excel format       
                //dtPeakLoad = csvread.convertExcelToDataTable(csv_file_path3);         
                //dtFeederPeak = csvread.convertExcelToDataTable(csv_file_path2);
                Console.Write("CSVs converted to datatables.");
                var result = from tbPeakLoad in dtPeakLoad.AsEnumerable()
                             join tbFeederPeak in dtFeederPeak.AsEnumerable() on tbPeakLoad["NETWORKID"] equals tbFeederPeak["int_external_id"]
                             select dtResult.LoadDataRow(new object[]                         {
                              (string)tbPeakLoad["NETWORKID"],
                                "01-"+ tbPeakLoad["MONTH"] + "-" + tbPeakLoad["YEAR"]+ " "+tbPeakLoad["TIME"],                             
                              (string)tbPeakLoad["KW PEAK TOTAL"],
                              Convert.ToDouble(tbPeakLoad["KW PROJECTED"]),
                              (string)tbFeederPeak["dbo_tbl_division.str_name"],
                              (string)tbFeederPeak["dbo_tbl_feeder.str_name"],
                              (string)tbFeederPeak["int_year"],
                              (string)tbFeederPeak["int_external_id"],
                              (string)tbFeederPeak["MW_PEAK"],
                              (string)tbFeederPeak["CONVERTED_KW"],
                              (string)tbFeederPeak["dtm_peak_date"],
                              (string)tbFeederPeak["flt_normal_cap_amps"]
                             
                         }, false);
                result.CopyToDataTable();
                DataView view = new DataView(dtResult);
                System.Data.DataTable dtFeederName = view.ToTable(true, "dbo_tbl_feeder.str_name", "dtm_peak_date");
                InsertDataIntoDatabase(dtResult, dtFeederName);
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
        //Read csv file 
        public static  System.Data.DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
             System.Data.DataTable csvData = new  System.Data.DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    //read column names
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }

                            if (i == 1 && colFields[1] == "MONTH") {
                                fieldData[i] = fieldData[i].Substring(0, 3).ToUpper();
                               // fieldData[i] = DateTime.ParseExact(fieldData[i], "MMMM", CultureInfo.InvariantCulture).Month.ToString();
                            }

                            if (i == 3 && colFields[3] == "TIME")
                            {
                                IFormatProvider format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                                DateTime time_24 = DateTime.ParseExact(fieldData[i], "HHmm", format);
                                fieldData[i] = time_24.ToString("HH:mm:ss");                             
                            }
                            if ((i == 2 && colFields[2] == "flt_normal_cap_mw") || (i == 4 && colFields [4]== "flt_mw"))
                            {
                                fieldData[i] = (Convert.ToDouble(fieldData[i]) * 1000).ToString();
                            }

                            if (i == 1 && colFields[1] == "dbo_tbl_feeder.str_name")
                            {
                                int numberIndex = fieldData[i].IndexOfAny("0123456789".ToCharArray());

                                int spaceIndex = fieldData[i].ToString().IndexOf(' ');

                                while (spaceIndex >= 0)
                                {
                                    // do whatever you want with myString @ pos
                                    if (spaceIndex == (numberIndex - 1))
                                    {
                                        StringBuilder sb = new StringBuilder(fieldData[i].ToString());
                                        char newChar = '-';
                                        sb[spaceIndex] = newChar;
                                        fieldData[i] = sb.ToString();
                                        break;
                                    }

                                    // find next
                                    spaceIndex = fieldData[i].ToString().IndexOf(' ', spaceIndex + 1);
                                }
                            }                         

                        }
                        csvData.Rows.Add(fieldData);
                    }
                }
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
            }
             return csvData;
        }
        //Data insert and update of circuit load and circuit load history  tables
        private void InsertDataIntoDatabase( System.Data.DataTable dtResult,  System.Data.DataTable dtFeederName)
        {
            Console.Write("Data insertion in database started.");
            int[] WinterMonths = new int[] { 1, 2, 3, 11, 12 };
            int[] SummerMonths = new int[] { 4, 5, 6, 7, 8, 9, 10 };
            string columnName = string.Empty;
            string[] triggerNames = new string[] { "SM_CIRCUIT_LOAD_UPDT", "SM_CIRCUIT_LOAD_HIST_INS", "SM_CIRCUIT_LOAD_HIST_UPDT" };
            triggerDisable(triggerNames);

            //(V3SF) Get Connection String
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = SetConnectionString();

            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(ConnectionString))
            {
                connection.Open();
                OracleTransaction transaction;
                // Start a local transaction
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                // Assign transaction object for a pending local transaction
                string qrUpdateCL = string.Empty;
                try
                {
                    for (int i = 0; i < dtFeederName.Rows.Count; )
                    {                       
                        string qrCircuitBreaker = "Select ID from sm_circuit_breaker where OPERATING_NUM = '" + dtFeederName.Rows[i][0] + "' and current_future ='C'";
                        OracleCommand cmdCB = new OracleCommand(qrCircuitBreaker, connection);
                        if (cmdCB.ExecuteScalar() != null)
                        {

                            string circuitBreakerID = cmdCB.ExecuteScalar().ToString();

                            string qrCircuitLoad = "Select ID from SM_CIRCUIT_LOAD where REF_DEVICE_ID='" + circuitBreakerID + "'";
                            OracleCommand cmdCL = new OracleCommand(qrCircuitLoad, connection);
                            string circuitLoadID = cmdCL.ExecuteScalar().ToString();


                            var qrMonths = from row in dtResult.AsEnumerable()
                                           where row["dbo_tbl_feeder.str_name"] == dtFeederName.Rows[i][0]
                                           select Convert.ToDateTime(row["Date"]).Month;

                            int[] months = qrMonths.ToArray();                                                

                            double maxSummerProjKW = Convert.ToDouble((from row in dtResult.AsEnumerable()
                                                                       where row["dbo_tbl_feeder.str_name"] == dtFeederName.Rows[i][0]
                                                                       where SummerMonths.Contains(Convert.ToDateTime(row["Date"]).Month)
                                                                       select row["KW PROJECTED"]).Max());

                            double maxWinterProjKW = Convert.ToDouble((from row in dtResult.AsEnumerable()
                                                                       where row["dbo_tbl_feeder.str_name"] == dtFeederName.Rows[i][0]
                                                                       where WinterMonths.Contains(Convert.ToDateTime(row["Date"]).Month)
                                                                       select row["KW PROJECTED"]).Max());


                            if (months.Intersect(WinterMonths).Any() && months.Intersect(SummerMonths).Any())
                            {
                                qrUpdateCL = "update SM_CIRCUIT_LOAD set SUMMER_UPDT_DTM = sysdate, WINTER_UPDT_DTM = sysdate, UPDATE_DTM = sysdate, SUMMER_PROJ_KW = " + maxSummerProjKW + ", WINTER_PROJ_KW = " + maxWinterProjKW + "  where id = '" + circuitLoadID + "'";
                            }
                            else if (months.Intersect(WinterMonths).Any() == true && months.Intersect(SummerMonths).Any() == false)
                            {
                                qrUpdateCL = "update SM_CIRCUIT_LOAD set WINTER_UPDT_DTM = sysdate, UPDATE_DTM = sysdate, WINTER_PROJ_KW = " + maxWinterProjKW + " where id = '" + circuitLoadID + "'";
                            }
                            else if (months.Intersect(WinterMonths).Any() == false && months.Intersect(SummerMonths).Any() == true)
                            {
                                qrUpdateCL = "update SM_CIRCUIT_LOAD set SUMMER_UPDT_DTM = sysdate, UPDATE_DTM = sysdate, SUMMER_PROJ_KW = " + maxSummerProjKW + " where id = '" + circuitLoadID + "'";
                            }

                         
                            OracleCommand cmdUpdateCL = new OracleCommand(qrUpdateCL, connection);
                            cmdUpdateCL.Transaction = transaction;
                            cmdUpdateCL.ExecuteNonQuery();

                            var results = from myRow in dtResult.AsEnumerable()
                                          where myRow["dbo_tbl_feeder.str_name"] == dtFeederName.Rows[i][0]
                                          select myRow;

                            System.Data.DataTable dtSingleID = results.CopyToDataTable();

                            for (int j = 0; j < dtSingleID.Rows.Count; j++)
                            {
                                string peakDate = DateTime.ParseExact(dtSingleID.Rows[j][1].ToString(), "dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy hh:mm:ss tt");

                                string queryString = "Select count(*) from sm_circuit_load_hist where sm_circuit_load_id='" + circuitLoadID + "' and PEAK_DTM like '%" + Convert.ToDateTime(dtSingleID.Rows[j][1]).ToString("dd-MMM-yy").ToUpper() + "%'";
                                OracleCommand commandCount = new OracleCommand(queryString, connection);                                
                                Int32 rowCount = Convert.ToInt32(commandCount.ExecuteScalar());
                                if (rowCount == 0)
                                {
                                    string qrInsertCLH = "Insert into sm_circuit_load_hist (ID, SM_CIRCUIT_LOAD_ID,CREATE_DTM,CREATE_USERID,UPDATE_DTM,UPDATE_USERID,PEAK_DTM,TOTAL_KW_LOAD) " +
                                                         "Values((select  max(id) from sm_circuit_load_hist)+1," + circuitLoadID + ",sysdate,'" + userID + "',sysdate,'" + userID + "','" + peakDate + "'," + dtSingleID.Rows[j][2] + ") ";
                                    OracleCommand cmdInsertCLH = new OracleCommand(qrInsertCLH, connection);
                                    cmdInsertCLH.Transaction = transaction;
                                    cmdInsertCLH.ExecuteNonQuery();
                                }
                                else
                                {
                                    string qrUpdateCLH = "update sm_circuit_load_hist set UPDATE_DTM = sysdate, UPDATE_USERID = '" + userID + "', PEAK_DTM = '" + peakDate + "',TOTAL_KW_LOAD = " + dtSingleID.Rows[j][2] + "  where sm_circuit_load_id='" + circuitLoadID + "' and PEAK_DTM like '%" + Convert.ToDateTime(dtSingleID.Rows[j][1]).ToString("dd-MMM-yy").ToUpper() + "%'";
                                    OracleCommand cmdUpdateCLH = new OracleCommand(qrUpdateCLH, connection);
                                    cmdUpdateCLH.Transaction = transaction;
                                    cmdUpdateCLH.ExecuteNonQuery();
                                }                                
                            }
                            i++;
                        }
                        else
                        {
                            i++;
                        }
                    }
                    transaction.Commit();
                    connection.Close();
                    Console.Write("Data inserted in database.");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }
            }
            triggerEnable(triggerNames);     
        }

        //Read Excel data
        public static System.Data.DataTable convertExcelToDataTable(string fileName)
        {
            //create the Application object we can use in the member functions.
            Microsoft.Office.Interop.Excel.Application _excelApp = new Microsoft.Office.Interop.Excel.Application();
            // _excelApp.Visible = true;

            ////open the workbook
            //Workbook workbook = _excelApp.Workbooks.Open(fileName,
            //    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //    Type.Missing, Type.Missing, Type.Missing, Type.Missing,
            //    Type.Missing, Type.Missing);

            Workbook workbook = _excelApp.Workbooks.Open(fileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
          
            //select the first sheet        
            Worksheet worksheet = (Worksheet)workbook.Worksheets[1];
           

            //find the used range in worksheet
            Range excelRange = worksheet.UsedRange;
          

            //get an object array of all of the cells in the worksheet (their values)
            object[,] valueArray = (object[,])excelRange.get_Value(
                        XlRangeValueDataType.xlRangeValueDefault);

            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {

                //access the cells
                for (int row = 1; row <= worksheet.UsedRange.Rows.Count; row++)
                {
                    System.Data.DataRow dr = dt.NewRow();
                    for (int col = 1; col <= worksheet.UsedRange.Columns.Count; ++col)
                    {
                        if (row == 1)
                        {
                            dt.Columns.Add(valueArray[row, col].ToString());
                        }
                        else
                        {
                            if (valueArray[row, col] != null)
                                dr[col - 1] = valueArray[row, col].ToString();
                            else
                                dr[col - 1] = "";
                        }
                    }
                    Range cellValue = (Range)worksheet.Cells[row, 1];
                    if (row != 1)
                    {
                        if (!string.IsNullOrEmpty(cellValue.Text.ToString()))
                        {
                            dt.Rows.Add(dr);
                        }
                        else {
                            break;
                        }
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {              
                return null;
            }
            finally
            {
                //clean up stuffs
                workbook.Close(false, Type.Missing, Type.Missing);
                Marshal.FinalReleaseComObject(workbook);
                _excelApp.Quit();
                Marshal.FinalReleaseComObject(_excelApp);
            }
        }
        //Trigger Disable
        public void triggerDisable(string[] triggerNames)
        {
            //(V3SF) Get Connection String
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = SetConnectionString();

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = connection;
                    for (int i = 0; i < triggerNames.Length; i++)
                    {
                        string qrUpdateCL = string.Format("Alter TRIGGER  " + triggerNames[i] + " disable");
                        cmd.CommandText = qrUpdateCL;//string.Format("UPDATE SM_UTILITY SET LAST_RUN_DATE=To_Date('{0}','MM-DD-YYYY HH24:MI:SS') WHERE PROCESS_NAME='{1}'", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss"), Constants.OMSProcess);
                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }
        //Trigger Enable
        public void triggerEnable(string[] triggerNames)
        {
            //(V3SF) Get Connection String
            if (string.IsNullOrEmpty(ConnectionString))
                ConnectionString = SetConnectionString();

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = connection;
                    for (int i = 0; i < triggerNames.Length; i++)
                    {
                        string qrUpdateCL = string.Format("Alter TRIGGER  " + triggerNames[i] + " enable");
                        cmd.CommandText = qrUpdateCL;//string.Format("UPDATE SM_UTILITY SET LAST_RUN_DATE=To_Date('{0}','MM-DD-YYYY HH24:MI:SS') WHERE PROCESS_NAME='{1}'", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss"), Constants.OMSProcess);
                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }       
       
    }


}
