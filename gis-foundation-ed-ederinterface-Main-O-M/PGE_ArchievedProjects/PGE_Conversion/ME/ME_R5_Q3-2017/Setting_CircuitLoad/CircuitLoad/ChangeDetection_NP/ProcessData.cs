using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using System.Data;
using System.IO;
using log4net;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using Oracle.DataAccess.Client;
using System.Configuration;
//using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Reflection;
namespace ChangeDetection_NP
{
    public class csvread
    {
        public static string ConnectionString = ConfigurationManager.ConnectionStrings["ConnectionStr"].ConnectionString;
        public static string userID = ConfigurationManager.AppSettings["UserID"];
        public static string summerCapacity = ConfigurationManager.AppSettings["SummerCapacity"];
        public static string winterCapacity = ConfigurationManager.AppSettings["WinterCapacity"];
        public static string substationName = ConfigurationManager.AppSettings["SubstationName"];
        public static string feederName = ConfigurationManager.AppSettings["FeederName"];
        public static string date = ConfigurationManager.AppSettings["Date"];
        public static string KWPeak = ConfigurationManager.AppSettings["KW_Peak"];
        public static string conversionFactor = ConfigurationManager.AppSettings["conversionFactor"];
        public Log4NetLogger logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "LoadData.config");
        public bool ProcessCsvData(string csv_file_path1, string csv_file_path2, string csv_file_path3)
        {
            try
            {
                Console.Write("CSV to datatable conversion started.");
                System.Data.DataTable dtPeakLoad = new System.Data.DataTable();
                System.Data.DataTable dtFeederPeak = new System.Data.DataTable();
                System.Data.DataTable dtEDPIFeeder = new System.Data.DataTable();
                System.Data.DataTable dtResult = new System.Data.DataTable();
                System.Data.DataTable opNumdt = new System.Data.DataTable();
                dtResult.Columns.Add("NETWORKID", typeof(string));
                dtResult.Columns.Add("Date", typeof(string));
                //dtResult.Columns.Add("MONTH", typeof(string));
                //dtResult.Columns.Add("YEAR", typeof(string));
                //dtResult.Columns.Add("TIME", typeof(string));
                dtResult.Columns.Add("KW PEAK TOTAL", typeof(string));
                dtResult.Columns.Add("KW PROJECTED", typeof(double));

                dtResult.Columns.Add("NETWORK ID", typeof(string));
                dtResult.Columns.Add("DEVICE TYPE", typeof(string));
                dtResult.Columns.Add("CIRCUIT BREAKER", typeof(string));
                dtResult.Columns.Add("GLOBAL_ID", typeof(string));
                dtResult.Columns.Add("OPERATING_NUM", typeof(string));
                //dtResult.Columns.Add("dbo_tbl_division.str_name", typeof(string));
                //dtResult.Columns.Add("dbo_tbl_feeder.str_name", typeof(string));
                //dtResult.Columns.Add("int_year", typeof(string));
                //dtResult.Columns.Add("int_external_id", typeof(string));
                //dtResult.Columns.Add("MW_PEAK", typeof(string));
                //dtResult.Columns.Add("CONVERTED_KW", typeof(string));
                //dtResult.Columns.Add("dtm_peak_date", typeof(string));
                //dtResult.Columns.Add("flt_normal_cap_amps", typeof(string));

                if (Path.GetExtension(csv_file_path1) == ".csv")
                {
                    dtPeakLoad = csvread.GetDataTabletFromCSVFile(csv_file_path1);
                }
                //else if (Path.GetExtension(csv_file_path1) == ".xlsx" || Path.GetExtension(csv_file_path1) == ".xls")
                //{
                //    dtPeakLoad = csvread.convertExcelToDataTable(csv_file_path1);
                //}
                Console.Write("First file converted to datatable.");

                if (Path.GetExtension(csv_file_path2) == ".csv")
                {
                    dtFeederPeak = csvread.GetDataTabletFromCSVFile(csv_file_path2);
                }
                //else if (Path.GetExtension(csv_file_path2) == ".xlsx" || Path.GetExtension(csv_file_path2) == ".xls")
                //{
                //    dtFeederPeak = csvread.convertExcelToDataTable(csv_file_path2);
                //}
                Console.Write("Second file converted to datatable.");
                if (Path.GetExtension(csv_file_path3) == ".csv")
                {
                    dtEDPIFeeder = csvread.GetDataTabletFromCSVFile(csv_file_path3);
                }
                //else if (Path.GetExtension(csv_file_path3) == ".xlsx" || Path.GetExtension(csv_file_path3) == ".xls")
                //{
                //    dtEDPIFeeder = csvread.convertExcelToDataTable(csv_file_path3);
                //}
                Console.Write("Third file converted to datatable.");
                // dtFeederPeak = CreateOperatingNum(dtFeederPeak);
                dtEDPIFeeder = CreateOperatingNum(dtEDPIFeeder);


                opNumdt = OPERATING_NUMfromGUID();


                var result = from tbPeakLoad in dtPeakLoad.AsEnumerable()
                             join tbFeederPeak in dtFeederPeak.AsEnumerable() on tbPeakLoad["NETWORKID"] equals tbFeederPeak["NETWORK ID"]
                             join a in opNumdt.AsEnumerable() on tbFeederPeak["GLOBAL_ID"] equals a["global_id"]
                             //where (string)a["OPERATING_NUM"] = "KING CITY-1106"
                             orderby (string)a["OPERATING_NUM"]
                             select dtResult.LoadDataRow(new object[]{
                              (string)tbPeakLoad["NETWORKID"],
                                "01-"+ tbPeakLoad["MONTH"] + "-" + tbPeakLoad["YEAR"]+ " "+tbPeakLoad["TIME"],                             
                              (string)tbPeakLoad["KW PEAK TOTAL"],
                              Convert.ToDouble(tbPeakLoad["KW PROJECTED"]),
                           
                              (string)tbFeederPeak["NETWORK ID"],
                              (string)tbFeederPeak["DEVICE TYPE"],
                              (string)tbFeederPeak["CIRCUIT BREAKER"],
                              (string)tbFeederPeak["GLOBAL_ID"],
                               (string)a["OPERATING_NUM"],
                              
                         }, false);
                result.CopyToDataTable();

                                                 
                DataView view = new DataView(dtResult);
                int count = view.Count;
                System.Data.DataTable dtFeederName = view.ToTable(true, "NETWORKID", "OPERATING_NUM");
                logger.Info("All files upload. ");
                InsertDataIntoDatabase(dtResult,dtFeederName, dtEDPIFeeder);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static System.Data.DataTable GetDataTabletFromCSVFile(string csv_file_path)
        {
            System.Data.DataTable csvData = new System.Data.DataTable();
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

                            if (colFields[i] == "MONTH")
                            {
                                fieldData[i] = fieldData[i].Substring(0, 3).ToUpper();
                                // fieldData[i] = DateTime.ParseExact(fieldData[i], "MMMM", CultureInfo.InvariantCulture).Month.ToString();
                            }
                            //if (colFields[i] == "CIRCUIT BREAKER")
                            //{
                            //    fieldData[i] = fieldData[i].IndexOf("/").ToString();

                            //}

                            if (colFields[i] == "TIME")
                            {
                                IFormatProvider format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                                DateTime time_24 = DateTime.ParseExact(fieldData[i], "HHmm", format);
                                fieldData[i] = time_24.ToString("HH:mm:ss");
                            }
                            //if (colFields[i] == "flt_normal_cap_amps")
                            //{
                            //    fieldData[i] = (Convert.ToDouble(fieldData[i]) * Convert.ToInt16(conversionFactor)).ToString();
                            //}                           

                            if (colFields[i] == summerCapacity || colFields[i] == winterCapacity)
                            {
                                fieldData[i] = (Convert.ToDouble(fieldData[i]) * Convert.ToInt16(conversionFactor)).ToString();
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
        public System.Data.DataTable OPERATING_NUMfromGUID()
        {
            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(ConnectionString))
            {
                connection.Open();
                System.Data.DataTable dt = new System.Data.DataTable();
                string qrGetCBId = "Select OPERATING_NUM,replace (replace( global_id,'{',''),'}','') as global_id from sm_circuit_breaker where OPERATING_NUM is not null and current_future ='C'";
                OracleCommand cmdCBId = new OracleCommand(qrGetCBId, connection);
                OracleDataAdapter da = new OracleDataAdapter(cmdCBId);
                da.Fill(dt);
                connection.Close();
                return dt;
            }
        }

        private void InsertDataIntoDatabase(System.Data.DataTable dtResult, System.Data.DataTable dtFeederName, System.Data.DataTable dtEDPIFeeder)
        {
            Console.Write("Data insertion in database started.");
            int[] WinterMonths = new int[] { 1, 2, 3, 11, 12 };
            int[] SummerMonths = new int[] { 4, 5, 6, 7, 8, 9, 10 };
            string columnName = string.Empty;
            string[] triggerNames = new string[] { "SM_CIRCUIT_LOAD_UPDT", "SM_CIRCUIT_LOAD_HIST_INS", "SM_CIRCUIT_LOAD_HIST_UPDT" };
            triggerDisable(triggerNames);
            string qrUpdateCL = string.Empty;
            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(ConnectionString))
            {
                connection.Open();
                OracleTransaction transaction;
                // Start a local transaction
                transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                
              
                try
                {
                    for (int i = 0; i < dtEDPIFeeder.Rows.Count; )
                    {
                        //string op1 =dtEDPIFeeder.Rows[i][substationName]+"-"+ dtEDPIFeeder.Rows[i]["CircuitName"].ToString().Substring(3,4);
                        string qrGetCBId = "Select ID from sm_circuit_breaker where OPERATING_NUM = '" + dtEDPIFeeder.Rows[i][substationName] + "' and current_future ='C'";
                        OracleCommand cmdCBId = new OracleCommand(qrGetCBId, connection);
                        if (cmdCBId.ExecuteScalar() != null)
                        {
                            string cbID = cmdCBId.ExecuteScalar().ToString();

                            string qrCircuitLoad = "Select ID from SM_CIRCUIT_LOAD where REF_DEVICE_ID='" + cbID + "'";
                            OracleCommand commandCL = new OracleCommand(qrCircuitLoad, connection);
                            if (commandCL.ExecuteScalar() != null)
                            {

                                //double SummerKVCap = Convert.ToDouble(dtEDPIFeeder.Rows[i][summerCapacity]) * 1000;
                                //double WinterKVCap = Convert.ToDouble(dtEDPIFeeder.Rows[i][winterCapacity]) * 1000;
                                double SummerKVCap = Convert.ToDouble(dtEDPIFeeder.Rows[i][summerCapacity]);
                                double WinterKVCap = Convert.ToDouble(dtEDPIFeeder.Rows[i][winterCapacity]);
                                qrUpdateCL = "update SM_CIRCUIT_LOAD set UPDATE_DTM = sysdate, SUMMER_KVA_CAP = " + SummerKVCap + ", WINTER_KVA_CAP = " + WinterKVCap + "  where REF_DEVICE_ID = '" + cbID + "'";
                                OracleCommand commandUpdateCL = new OracleCommand(qrUpdateCL, connection);
                                commandUpdateCL.Transaction = transaction;
                                commandUpdateCL.ExecuteNonQuery();
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
                    Console.Write(" SUMMER_KVA_CAP Data inserted in database.");
                    logger.Info("SUMMER_KVA_CAP Data inserted in database. ");
                    }
                   catch
                      {
                          transaction.Rollback();
                          logger.Info("Error SUMMER_KVA_CAP Data not inserted in SM_CIRCUIT_LOAD table ");
                          Console.Write("Error SUMMER_KVA_CAP Data not inserted in SM_CIRCUIT_LOAD table.");

                    }
               // triggerEnable(triggerNames);
            }
            using (Oracle.DataAccess.Client.OracleConnection newconnection = new Oracle.DataAccess.Client.OracleConnection(ConnectionString))
            {
                newconnection.Open();
                OracleTransaction transaction1;
                // Start a local transaction
                transaction1 = newconnection.BeginTransaction(IsolationLevel.ReadCommitted);
                Oracle.DataAccess.Client.OracleConnection connection1 = new Oracle.DataAccess.Client.OracleConnection(ConnectionString);
                try
                {

                    // Start a local transaction
                    // transaction1 = newconnection.BeginTransaction(IsolationLevel.ReadCommitted);
                    for (int i = 0; i < dtFeederName.Rows.Count; )
                    {
                        string qrCircuitBreaker = "Select ID from sm_circuit_breaker where OPERATING_NUM = '" + dtFeederName.Rows[i]["OPERATING_NUM"] + "' and current_future ='C'";
                        OracleCommand cmdCB = new OracleCommand(qrCircuitBreaker, newconnection);
                        if (cmdCB.ExecuteScalar() != null)
                        {

                            string circuitBreakerID = cmdCB.ExecuteScalar().ToString();
                            string circuitLoadID = string.Empty;
                            string qrCircuitLoad = "Select ID from SM_CIRCUIT_LOAD where REF_DEVICE_ID='" + circuitBreakerID + "'";
                            connection1.Open();
                            OracleCommand cmdCL = new OracleCommand(qrCircuitLoad, connection1);
                            int circuitrowCount = Convert.ToInt32(cmdCL.ExecuteScalar());
                            if (circuitrowCount == 0)
                            {
                                logger.Info("No data of  " + dtFeederName.Rows[i]["OPERATING_NUM"]);
                            }
                            if (circuitrowCount > 0)
                            {
                                //Console.Write(" Start data insert in History table");
                                circuitLoadID = cmdCL.ExecuteScalar().ToString();
                                connection1.Close();

                                var qrMonths = from row in dtResult.AsEnumerable()
                                               where row["OPERATING_NUM"] == dtFeederName.Rows[i]["OPERATING_NUM"]
                                               select Convert.ToDateTime(row["Date"]).Month;

                                int[] months = qrMonths.ToArray();

                                var maxSummerProjKW1 = (from row in dtResult.AsEnumerable()
                                                        where row["OPERATING_NUM"] == dtFeederName.Rows[i]["OPERATING_NUM"]
                                                        where SummerMonths.Contains(Convert.ToDateTime(row["Date"]).Month)
                                                        select row["KW PROJECTED"]).ToList();



                                double maxSummerProjKW = Convert.ToDouble(maxSummerProjKW1.Max());



                                var maxWinterProjKW1 = (from row in dtResult.AsEnumerable()
                                                        where row["OPERATING_NUM"] == dtFeederName.Rows[i]["OPERATING_NUM"]
                                                        where WinterMonths.Contains(Convert.ToDateTime(row["Date"]).Month)
                                                        select row["KW PROJECTED"]).ToList();
                                double maxWinterProjKW = Convert.ToDouble(maxWinterProjKW1.Max());

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



                                OracleCommand cmdUpdateCL = new OracleCommand(qrUpdateCL, newconnection);
                                cmdUpdateCL.Transaction = transaction1;
                                cmdUpdateCL.ExecuteNonQuery();

                                var results = from myRow in dtResult.AsEnumerable()
                                              where myRow["OPERATING_NUM"] == dtFeederName.Rows[i]["OPERATING_NUM"]
                                              select myRow;

                                System.Data.DataTable dtSingleID = results.CopyToDataTable();
                                Console.Write(" ID: " + dtSingleID.Rows[0]["OPERATING_NUM"]);
                                // Console.Write("/      ");
                                string op = dtSingleID.Rows[0]["OPERATING_NUM"].ToString();

                                for (int j = 0; j < dtSingleID.Rows.Count; j++)
                                {
                                    string peakDate = DateTime.ParseExact(dtSingleID.Rows[j][date].ToString(), "dd-MMM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy hh:mm:ss tt");

                                    string queryString = "Select count(*) from sm_circuit_load_hist where sm_circuit_load_id='" + circuitLoadID + "' and PEAK_DTM like '%" + Convert.ToDateTime(dtSingleID.Rows[j][date]).ToString("dd-MMM-yy").ToUpper() + "%'";
                                    //  Console.Write("/      ");
                                    //Console.Write("Record inserted Query in Settings database for ID: " + queryString);
                                    connection1.Open();
                                    OracleCommand commandCount = new OracleCommand(queryString, connection1);

                                    int rowCount = Convert.ToInt32(commandCount.ExecuteScalar());
                                    connection1.Close();

                                    if (rowCount == 0)
                                    {

                                        string qrInsertCLH = "Insert into sm_circuit_load_hist (ID, SM_CIRCUIT_LOAD_ID,CREATE_DTM,CREATE_USERID,UPDATE_DTM,UPDATE_USERID,PEAK_DTM,TOTAL_KW_LOAD) " +
                                                             "Values((select  max(id) from sm_circuit_load_hist)+1," + circuitLoadID + ",sysdate,'" + userID + "',sysdate,'" + userID + "','" + peakDate + "'," + dtSingleID.Rows[j][KWPeak].ToString().Trim() + ") ";
                                        OracleCommand cmdInsertCLH = new OracleCommand(qrInsertCLH, newconnection);
                                        Console.Write(" ID: " + qrInsertCLH);
                                        cmdInsertCLH.CommandTimeout = 1000;
                                        cmdInsertCLH.Transaction = transaction1;
                                        cmdInsertCLH.ExecuteNonQuery();
                                        logger.Info("Record inserted in Settings database for ID: " + circuitLoadID);

                                    }
                                    else
                                    {
                                        string qrUpdateCLH = "update sm_circuit_load_hist set UPDATE_DTM = sysdate, UPDATE_USERID = '" + userID + "', PEAK_DTM = '" + peakDate + "',TOTAL_KW_LOAD = " + dtSingleID.Rows[j][KWPeak] + "  where sm_circuit_load_id='" + circuitLoadID + "' and PEAK_DTM like '%" + Convert.ToDateTime(dtSingleID.Rows[j][date]).ToString("dd-MMM-yy").ToUpper() + "%'";
                                        OracleCommand cmdUpdateCLH = new OracleCommand(qrUpdateCLH, newconnection);
                                        cmdUpdateCLH.Transaction = transaction1;
                                        cmdUpdateCLH.ExecuteNonQuery();
                                        logger.Info("Record Update in Settings database for ID: " + circuitLoadID);

                                    }
                                }
                            }
                            else
                            {
                                connection1.Close();
                            }
                            i++;
                        }
                        else
                        {
                            i++;
                        }

                    }

                    transaction1.Commit();
                    newconnection.Close();
                    logger.Info("Record update into table");
                    Console.Write("Data inserted in database.");
                }
                catch 
                {
                    transaction1.Rollback();
                    logger.Info("Error Data not inserted in database");
                    Console.Write("Error Data not inserted in database.");
                   // throw (ex);
                }

            }
            triggerEnable(triggerNames);
        }

        
        //public static System.Data.DataTable convertExcelToDataTable(string fileName)
        //{
        //    //create the Application object we can use in the member functions.
        //    Microsoft.Office.Interop.Excel.Application _excelApp = new Microsoft.Office.Interop.Excel.Application();
        //    // _excelApp.Visible = true;

        //    ////open the workbook
        //    Workbook workbook = _excelApp.Workbooks.Open(fileName,
        //        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //        Type.Missing, Type.Missing, Type.Missing, Type.Missing,
        //        Type.Missing, Type.Missing);

        //    // Workbook workbook = _excelApp.Workbooks.Open(fileName, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

        //    //select the first sheet        
        //    Worksheet worksheet = (Worksheet)workbook.Worksheets[1];

        //    //find the used range in worksheet
        //    Range excelRange = worksheet.UsedRange;

        //    //get an object array of all of the cells in the worksheet (their values)
        //    object[,] valueArray = (object[,])excelRange.get_Value(
        //                XlRangeValueDataType.xlRangeValueDefault);

        //    System.Data.DataTable dt = new System.Data.DataTable();
        //    try
        //    {

        //        //access the cells
        //        for (int row = 1; row <= worksheet.UsedRange.Rows.Count; row++)
        //        {
        //            System.Data.DataRow dr = dt.NewRow();
        //            for (int col = 1; col <= worksheet.UsedRange.Columns.Count; ++col)
        //            {
        //                if (row == 1)
        //                {
        //                    dt.Columns.Add(valueArray[row, col].ToString());
        //                }
        //                else
        //                {
        //                    if (valueArray[row, col] != null)
        //                    {
        //                        string fieldName = valueArray[1, col].ToString();
        //                        string fieldData = valueArray[row, col].ToString();
        //                        if (fieldName == "MONTH")
        //                        {
        //                            valueArray[row, col] = valueArray[row, col].ToString().Substring(0, 3).ToUpper();
        //                            // fieldData[i] = DateTime.ParseExact(fieldData[i], "MMMM", CultureInfo.InvariantCulture).Month.ToString();
        //                        }
        //                        if (fieldName == "TIME")
        //                        {
        //                            IFormatProvider format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
        //                            DateTime time_24 = DateTime.ParseExact(valueArray[row, col].ToString(), "HHmm", format);
        //                            valueArray[row, col] = time_24.ToString("HH:mm:ss");
        //                        }
        //                        //if (fieldName == "flt_normal_cap_amps")
        //                        //{
        //                        //    valueArray[row, col] = (Convert.ToDouble(valueArray[row, col]) * 1000).ToString();
        //                        //}

        //                        //if (fieldName == "dbo_tbl_feeder.str_name")
        //                        //{
        //                        //    int numberIndex = valueArray[row, col].ToString().IndexOfAny("0123456789".ToCharArray());

        //                        //    int spaceIndex = valueArray[row, col].ToString().IndexOf(' ');

        //                        //    while (spaceIndex >= 0)
        //                        //    {
        //                        //        // do whatever you want with myString @ pos
        //                        //        if (spaceIndex == (numberIndex - 1))
        //                        //        {
        //                        //            StringBuilder sb = new StringBuilder(valueArray[row, col].ToString());
        //                        //            char newChar = '-';
        //                        //            sb[spaceIndex] = newChar;
        //                        //            valueArray[row, col] = sb.ToString();
        //                        //            break;
        //                        //        }

        //                        //        // find next
        //                        //        spaceIndex = valueArray[row, col].ToString().IndexOf(' ', spaceIndex + 1);
        //                        //    }
        //                        //}

        //                        dr[col - 1] = valueArray[row, col].ToString();
        //                    }
        //                    else
        //                        dr[col - 1] = "";
        //                }
        //            }
        //            Range cellValue = (Range)worksheet.Cells[row, 1];
        //            if (row != 1)
        //            {
        //                if (!string.IsNullOrEmpty(cellValue.Text.ToString()))
        //                {
        //                    dt.Rows.Add(dr);
        //                }
        //                else
        //                {
        //                    break;
        //                }
        //            }
        //        }
        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        //clean up stuffs
        //        workbook.Close(false, Type.Missing, Type.Missing);
        //        Marshal.FinalReleaseComObject(workbook);
        //        _excelApp.Quit();
        //        Marshal.FinalReleaseComObject(_excelApp);
        //    }
        //}

        public void triggerDisable(string[] triggerNames)
        {
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

        public void triggerEnable(string[] triggerNames)
        {
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

        public System.Data.DataTable CreateOperatingNum(System.Data.DataTable dt)
        {
            var rowsToUpdate = dt.AsEnumerable();
            string fieldValue = string.Empty;
            foreach (var row in rowsToUpdate)
            {
                //if (row.Table.Columns.Count == 8) {
                //    fieldValue = row.ItemArray[1].ToString();
                //}
                //else if (row.Table.Columns.Count == 46)
                //{
                // fieldValue = row.ItemArray[4].ToString() + " " + Regex.Replace(row.ItemArray[6].ToString(), "[^0-9]+", string.Empty);
                //}
                fieldValue = row.ItemArray[4].ToString() + " " + Regex.Replace(row.ItemArray[6].ToString(), "[^0-9]+", string.Empty);
                //if (fieldName == "dbo_tbl_feeder.str_name")
                //{
                int numberIndex = fieldValue.IndexOfAny("0123456789".ToCharArray());

                int spaceIndex = fieldValue.ToString().IndexOf(' ');

                while (spaceIndex >= 0)
                {
                    // do whatever you want with myString @ pos
                    if (spaceIndex == (numberIndex - 1))
                    {
                        StringBuilder sb = new StringBuilder(fieldValue);
                        char newChar = '-';
                        sb[spaceIndex] = newChar;
                        fieldValue = sb.ToString();
                        break;
                    }

                    // find next
                    spaceIndex = fieldValue.ToString().IndexOf(' ', spaceIndex + 1);
                }
                //if (row.Table.Columns.Count == 8)
                //{
                //     row.SetField("dbo_tbl_feeder.str_name", fieldValue);

                //}
                //  else if (row.Table.Columns.Count == 46){
                //    row.SetField("SubstationName", fieldValue);
                //}
                row.SetField("SubstationName", fieldValue);
                //}
            }
            return dt;
        }
    }

}
