using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Diagnostics;
using System.Reflection;

namespace Utility
{
    class TLM
    {
      
        static TLM()
        {
            
            
            try
            {
                // check to see if the working directories are created tlm enhancement 01/10/2020
                Common.CreateDirectory(Constants.TLMWorkingFolder);
                DirectoryInfo dir = Directory.GetParent(Constants.TLMApplicationLogFile);
                Common.CreateDirectory(dir.FullName);
                dir = Directory.GetParent(Constants.TLMMonthlyLoadLogFile);
                Common.CreateDirectory(dir.FullName);
            }
            catch (Exception ex)
            {
            
            }

        }

        public static int ProcessCDWFiles(bool copyFiles, bool unZipFiles)
        {
            int retVal = 0;
            try
            {
                if (!triggerFileExist(Constants.TLMCCWSourceFile, Constants.TMLCCWTriggerFile))
                    throw new ApplicationException("Trigger file does not exist for CCW.");

                cleanDirectory(Constants.TLMWorkingFolder);
                if (copyFiles)
                {
                    Common.CopyFile(Constants.TLMCCWSourceFile, Constants.TLMWorkingFolder);
                    // Archive CCW File to local folder
                    Common.CopyFile(Constants.TLMWorkingFolder, string.Concat(Constants.TLMArchiveFolder, "\\CCWData\\", DateTime.Now.ToString("MMddyyyy")));
                }

                if (unZipFiles)
                {
                    unzipCDWFile(Constants.TLMWorkingFolder);
                }

                if (retVal == 0)
                    retVal = TLM.LoadServicePointData();

                if (retVal == 0)
                    retVal = TLM.LoadServicePointGenerationData();

                if (retVal == 0)
                    retVal = TLM.LoadTransformerGenerationData();

                if (retVal == 0)
                    retVal = TLM.LoadTransformerData();

                if (retVal == 0)
                    cleanDirectory(Constants.TLMCCWSourceFile);
            }
            catch (Exception ex)
            {
                TLM.logMessage("Error occured while processing CCW files.", ex);
                retVal = 1;
            }
            finally
            {
                cleanDirectory(Constants.TLMWorkingFolder);
            }
            return retVal;
        }

        public static int ProcessCCNBFiles(bool copyFiles, bool unZipFiles)
        {
            int retVal = 0;
            try
            {
                if (!triggerFileExist(Constants.TLMCCNBSourceFile, Constants.TMLCCNBTriggerFile))
                    throw new ApplicationException("Trigger file does not exist for CCNB.");

                cleanDirectory(Constants.TLMWorkingFolder);
                if (copyFiles)
                {
                    Common.CopyFile(Constants.TLMCCNBSourceFile, Constants.TLMWorkingFolder);
                    // Archive CCW File to local folder
                    Common.CopyFile(Constants.TLMWorkingFolder, string.Concat(Constants.TLMArchiveFolder, "\\CCBData\\", DateTime.Now.ToString("MMddyyyy")));
                }

                if (unZipFiles)
                {
                    foreach (string file in Directory.GetFiles(Constants.TLMWorkingFolder))
                    {
                        FileInfo fi = new FileInfo(file);
                        if (fi.Extension.Equals(".gz", StringComparison.CurrentCultureIgnoreCase))
                        {
                            Common.UnZipFile(file, Constants.TLMWorkingFolder);
                            fi.Delete();
                        }
                    }
                }

                if (retVal == 0)
                    retVal = TLM.LoadCCBMeterData();

                if (retVal == 0)
                    cleanDirectory(Constants.TLMCCNBSourceFile);
            }
            catch (Exception ex)
            {
                TLM.logMessage("Error occured while processing CCNB files.", ex);
                retVal = 1;
            }
            finally
            {
                cleanDirectory(Constants.TLMWorkingFolder);
            }
            return retVal;
        }

        public static int ProcessGISChanges()
        {
            int retValue = 0;
            try
            {
                List<string> procsToCall = new List<string>() { "TLM_CD_MGMT_MONTHLY.TRANSFORMER_MGMT", "TLM_CD_MGMT_MONTHLY.TRANSFORMER_BANK_MGMT", "TLM_CD_MGMT_MONTHLY.METER_MGMT" };
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                string errorMsg, errorCode = string.Empty;
                logMessage(string.Format("Start of the Monthly Load Process from GIS"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();
                    foreach (string proc in procsToCall)
                    {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = proc;
                            cmd.Parameters.Add("FromDate", OracleDbType.Date).Value = startDate;
                            cmd.Parameters.Add("ToDate", OracleDbType.Date).Value = endDate;
                            cmd.Parameters.Add("ErrorMsg", OracleDbType.Varchar2, ParameterDirection.Output).Size = 2000;
                            cmd.Parameters.Add("ErrorCode", OracleDbType.Varchar2, ParameterDirection.Output).Size = 200;
                            cmd.CommandTimeout = 60000;
                            cmd.ExecuteNonQuery();

                            errorMsg = cmd.Parameters["ErrorMsg"].Value.ToString();
                            errorCode = cmd.Parameters["ErrorCode"].Value.ToString();
                            if (!string.IsNullOrEmpty(errorCode) && errorCode != "null")
                            {
                                logMessage(string.Format("Error occured during the Monthly Load Process from GIS :::: Process - {0} and Error Message::{1}", proc, errorMsg));
                            }
                        }
                    }
                    connection.Close();
                }
                logMessage(string.Format("End of the Monthly Load Process from GIS"));
            }
            catch (Exception ex)
            {
                logMessage("Error occured during the Monthly Load Process from GIS", ex);
                retValue = 1;
            }
            return retValue;
        }

        //added  on  12/04/2019 for TLM ENHANCEMENT
        public static int ProcessGenerateCD()
        {
            int retValue = 0;
            try
            {
                string procsToCall =   "PGEDATA.Generate_cd_tables" ;

               
                logMessage(string.Format("Start of the Generate CD tables"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionStringCD))
                {
                    connection.Open();
                   
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = procsToCall;
                           cmd.CommandTimeout = 60000;
                            cmd.ExecuteNonQuery();
                            logMessage(string.Format("Completion of the Generate CD tables"));

                        }
                    
                    connection.Close();
                }
                logMessage(string.Format("End of the generate cd Process from GIS"));
            }
            catch (Exception ex)
            {
                logMessage("Error occured during the generate cd Process from GIS", ex);
                retValue = 1;
            }
            return retValue;
        }

        //added  on  12/04/2019 for TLM ENHANCEMENT
        public static int populateCDW()
        {
            int retValue = 0;
            DateTime? BatchDate=null;
            try
            {
                string procsToCall = "edtlm.transformer_loading_tools.populate_CDW_data";

                
                string batchdt = Constants.TLMPopCDWBatchDate;
                BatchDate = (string.IsNullOrEmpty(batchdt) ? (DateTime?)null : DateTime.Parse(batchdt)); ;
      
                
                logMessage(string.Format("Start of the process of populating cdw data"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procsToCall;
                        
                        cmd.Parameters.Add("batchDate", OracleDbType.Date).Value = BatchDate;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        logMessage(string.Format("Completion of the process of populating cdw data"));
                      
                    }

                    connection.Close();
                }
                logMessage(string.Format("End of the populating cdw data"));
            }
            catch (Exception ex)
            {
                logMessage("Error occured during populating cdw data", ex);
                retValue = 1;
            }
            return retValue;
        }
        //added  on  12/04/2019 for TLM ENHANCEMENT
        public static int ProcessMaintainTb()
        {
            int retValue = 0;
            try
            {
                string procsToCall = "edtlm.transformer_loading_tools.maintain_tables";
                
               // string errorMsg, errorCode = string.Empty;
                logMessage(string.Format("Start of the Maintainance tables"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procsToCall;
                      
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        logMessage(string.Format("Completion of the Maintainance tables"));
                        
                    }

                    connection.Close();
                }
                logMessage(string.Format("End of the Maintainance Table  Process "));
            }
            catch (Exception ex)
            {
                logMessage("Error occured during the Maintainance Table  Process", ex);
                retValue = 1;
            }
            return retValue;
        }
        //added  on  12/04/2019 for TLM ENHANCEMENT
        public static int ProcessMonthlyTr(string  ivariant , string  ithread)
        {
            int retValue = 0;
            string thread = null;
            string variant = null;
            DateTime ? BatchDate=null;
            try
            {
                string procsToCall = "edtlm.transformer_loading_tools.Monthly_TLM_run";
                string batchdt = Constants.TLMMonthlyRunBatchDate;
                BatchDate= (string.IsNullOrEmpty(batchdt) ?(DateTime?)null  : DateTime.Parse(batchdt));;




                if ((ivariant.ToUpper().Equals("SMART")) || (ivariant.ToUpper().Equals("LEGACY")) || (ivariant.ToUpper().Equals("ALL")))
                {

                    variant = ivariant;
                }

                else 
                {
                    throw new System.ArgumentException("Parameter cannot be other than SMART OR LEGACY OR ALL", "ivariant");
                }
                if ((ithread.ToUpper().Equals("0")) || (ithread.ToUpper().Equals("1")) || (ithread.ToUpper().Equals("2")) || (ithread.ToUpper().Equals("3"))
                    || (ithread.ToUpper().Equals("4"))
                    || (ithread.ToUpper().Equals("5"))
                    || (ithread.ToUpper().Equals("6"))
                    || (ithread.ToUpper().Equals("7"))
                    || (ithread.ToUpper().Equals("8"))
                    || (ithread.ToUpper().Equals("9")))
                {

                    thread = ithread;
                }

                else 
                {
                    throw new System.ArgumentException("Parameter cannot be other than [0,1,2,3,4,5,6,7,8,9]", "ithread");
                }


                
               
                
                
                logMessage(string.Format("Start of the Monthly TLM Run"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procsToCall;
                       
                        cmd.Parameters.Add("variant", OracleDbType.Varchar2).Value =variant ;
                        
                        cmd.Parameters.Add("batchDate", OracleDbType.Date).Value = BatchDate;
                        cmd.Parameters.Add("thread", OracleDbType.Int32).Value = thread;


                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        logMessage(string.Format("completion of the Monthly TLM Run"));
                      
                    }

                    connection.Close();
                }
                logMessage(string.Format("End of the MONTHLY TLM RUN from GIS"));
            }
            catch (Exception ex)
            {
                logMessage("Error occured during the MONTHLY TLM RUN ", ex);
                retValue = 1;
            }
            return retValue;
        }
        //added  on  12/04/2019 for TLM ENHANCEMENT
        public static int ProcessCcb()
        {
            int retValue = 0;
            string refreshv = null;
            DateTime? BatchDate = null;
            try
            {
                string procsToCall = "edtlm.transformer_loading_tools.Populate_CCB_data";


                string RefreshValue = Constants.TLMCCBRefresh;
                if (RefreshValue.ToUpper().Equals("YES") || (RefreshValue.ToUpper().Equals("NO")))
                {
                    
                    refreshv = RefreshValue;
                }

                else 
                {
                    throw new System.ArgumentException("Parameter cannot be other than YES or NO", "RefreshValue");
                }
               
                string batchdt = Constants.TLMPopCCBBatchDate;
                BatchDate = (string.IsNullOrEmpty(batchdt) ? (DateTime?)null : DateTime.Parse(batchdt)); ;
                
               
                
       
                logMessage(string.Format("Start of the Process  CCB data"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();

                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procsToCall;
                        cmd.Parameters.Add("refreshext", OracleDbType.Varchar2).Value = refreshv;

                        cmd.Parameters.Add("batchDate", OracleDbType.Date).Value = BatchDate;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                        logMessage(string.Format("Completion of the Process  CCB data"));
                       
                    }

                    connection.Close();
                }
                logMessage(string.Format("End of the populating  ccb data  Process "));
            }
            catch (Exception ex)
            {
                logMessage("Error occured during the populating  ccb data  Process", ex);
                retValue = 1;
            }
            return retValue;
        }

        public static int LoadTransformerData()
        {
            string destinationTable = "EXT_SM_TRF_LOAD";
            char delimeter = ',';
            int returnCode = 0;
            int arrayLenght = 4;

            try
            {

                logMessage(string.Format("Start of {0} Load Process", destinationTable));

                Dictionary<string, Tuple<string, bool, int>> columnRowMapping;
                DataTable dt = createTRFLoadTable(out columnRowMapping);

                returnCode = importFile(Constants.TLMSourceTransformerLoadFile, destinationTable, delimeter, arrayLenght, columnRowMapping, dt);

            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                returnCode = 1;
            }
            finally
            {
                logMessage(string.Format("End of {0} load process", destinationTable));
            }
            return returnCode;
        }

        public static int LoadServicePointData()
        {
            string destinationTable = "EXT_SM_SP_LOAD";
            char delimeter = ',';
            int returnCode = 0;
            int arrayLenght = 10;

            try
            {
                logMessage(string.Format("Start of {0} Load Process", destinationTable));

                Dictionary<string, Tuple<string, bool, int>> columnRowMapping;
                DataTable dt = createSPLoadTable(out columnRowMapping);
                returnCode = importFile(Constants.TLMSourceServicePointLoadFile, destinationTable, delimeter, arrayLenght, columnRowMapping, dt);
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                returnCode = 1;
            }
            finally
            {
                logMessage(string.Format("End of {0} load process", destinationTable));
            }
            return returnCode;
        }

        public static int LoadTransformerGenerationData()
        {
            string destinationTable = "EXT_SM_TRF_GEN_LOAD";
            char delimeter = ',';
            int returnCode = 0;
            int arrayLenght = 4;

            try
            {

                logMessage(string.Format("Start of {0} Load Process", destinationTable));

                Dictionary<string, Tuple<string, bool, int>> columnRowMapping;
                DataTable dt = createTRFLoadTable(out columnRowMapping);

                returnCode = importFile(Constants.TLMSourceTransformerGenerationFile, destinationTable, delimeter, arrayLenght, columnRowMapping, dt);

            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                returnCode = 1;
            }
            finally
            {
                logMessage(string.Format("End of {0} load process", destinationTable));
            }
            return returnCode;
        }

        public static int LoadServicePointGenerationData()
        {
            string destinationTable = "EXT_SM_SP_GEN_LOAD";
            char delimeter = ',';
            int returnCode = 0;
            int arrayLenght = 10;

            try
            {
                logMessage(string.Format("Start of {0} Load Process", destinationTable));

                Dictionary<string, Tuple<string, bool, int>> columnRowMapping;
                DataTable dt = createSPLoadTable(out columnRowMapping);
                returnCode = importFile(Constants.TLMSourceServicePointGenerationFile, destinationTable, delimeter, arrayLenght, columnRowMapping, dt);
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                returnCode = 1;
            }
            finally
            {
                logMessage(string.Format("End of {0} load process", destinationTable));
            }
            return returnCode;
        }

        public static int LoadCCBMeterData()
        {
            string destinationTable = "EXT_CCB_METER_LOAD";
            char delimeter = '^';
            int returnCode = 0;
            int arrayLenght = 50;

            try
            {
                logMessage(string.Format("Start of {0} Load Process", destinationTable));
                Dictionary<string, Tuple<string, bool, int>> columnRowMapping;
                DataTable dt = createCCBMeterTable(out columnRowMapping);
                returnCode = importFile(Constants.TLMSourceMeterFile, destinationTable, delimeter, arrayLenght, columnRowMapping, dt);
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                returnCode = 1;
            }
            finally
            {
                logMessage(string.Format("End of {0} load process", destinationTable));
            }
            return returnCode;
        }

        public static int MigrateExtToStaging()
        {
            int retValue = 0;
            DateTime batchDate = Constants.TLMMonthlyLoadBatchDate.AddMonths(-2);

            try
            {
                logMessage(string.Format("Start of the Data_Load_Validation Load Process"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Data_Load_Validation";
                        cmd.Parameters.Add("batchDate", OracleDbType.Date).Value = batchDate;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during the migration of data from ext to staging tables."), ex);
                retValue = 1;
            }
            finally
            {
                logMessage(string.Format("End of the Data_Load_Validation Load Process"));
            }
            return retValue;
        }

        public static int RunMonthyLoad()
        {
            int retValue = 0;
            try
            {
                DateTime batchDate = Constants.TLMMonthlyLoadBatchDate.AddMonths(-4);
                string schema = "EDTLM";
                logMessage(string.Format("Start of the Monthly Load Process"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Run_Monthly_Load";
                        cmd.Parameters.Add("p_schema", OracleDbType.Varchar2).Value = schema;
                        cmd.Parameters.Add("batchDate", OracleDbType.Date).Value = batchDate;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                logMessage(string.Format("End of the Monthly Load Process"));
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during the Monthly Load Process"), ex);
                retValue = 1;
            }
            return retValue;
        }

        public static int RunCYMEMonthyLoad()
        {
            int retValue = 0;
            try
            {
                DateTime batchDate = Constants.TLMCYMEMonthlyLoadBatchDate.AddMonths(-2);
                string schema = "EDTLM";
                logMessage(string.Format("Start of the TLM CYME Monthly Load Process"));
                using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
                {
                    connection.Open();
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        cmd.Connection = connection;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "CYME_MONTHLY_LOAD";
                        cmd.Parameters.Add("p_schema", OracleDbType.Varchar2).Value = schema;
                        cmd.Parameters.Add("batchDate", OracleDbType.Date).Value = batchDate;
                        cmd.CommandTimeout = 60000;
                        cmd.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                logMessage(string.Format("End of the TLM CYME Monthly Load Process"));
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during the TLM CYME Monthly Load Process"), ex);
                retValue = 1;
            }
            return retValue;
        }
        public static int SendMonthyLoadLog()
        {
            int retValue = 0;
            try
            {
                logMessage(string.Format("Creating monthly load log file."));
                StringBuilder oRow = new StringBuilder();
                DateTime createDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                using (StreamWriter w = new StreamWriter(Constants.TLMMonthlyLoadLogFile))
                {
                    using (OracleConnection connection = new OracleConnection(Constants.TLMConnectionString))
                    {
                        connection.Open();
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Connection = connection;
                            cmd.CommandText = string.Format("SELECT * FROM {0} WHERE CREATE_DTM >= To_Date('{1}','MM-DD-YYYY')", "Monthly_Load_Log", createDate.ToString("MM-dd-yyyy"));
                            OracleDataReader myReader = cmd.ExecuteReader();
                            bool writeHeader = true;
                            while (myReader.Read())
                            {
                                oRow.Clear();
                                if (writeHeader)
                                {
                                    for (int x = 0; x < myReader.FieldCount; x++)
                                        oRow.Append(myReader.GetName(x)).Append(",");

                                    w.WriteLine(oRow.ToString().TrimEnd(','));
                                    oRow.Clear();
                                    writeHeader = false;
                                }

                                for (int x = 0; x < myReader.FieldCount; x++)
                                    oRow.Append(myReader[x]).Append(",");

                                w.WriteLine(oRow.ToString().TrimEnd(','));
                            }
                        }
                    }
                }
                logMessage(string.Format("End creating monthly load log file."));
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured during creation of monthly load log file."), ex);
                retValue = 1;
            }

            return retValue;

        }

        private static void unzipCDWFile(string directory)
        {
            string[] files = Directory.GetFiles(Constants.TLMWorkingFolder);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension.Equals(".tar", StringComparison.InvariantCultureIgnoreCase))
                {
                    Common.UnZipFile(file, Constants.TLMWorkingFolder);
                    fi.Delete();
                }
            }
            files = Directory.GetFiles(Constants.TLMWorkingFolder);
            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Extension.Equals(".gz", StringComparison.InvariantCultureIgnoreCase))
                {
                    Common.UnZipFile(file, Constants.TLMWorkingFolder);
                    fi.Delete();
                }
            }
        }

        private static int importFile(string sourceFile, string destinationTable, char delimeter, int totalFields, Dictionary<string, Tuple<string, bool, int>> columnRowMapping, DataTable destinationTableSchema)
        {
            int counter = 0;
            int returnCode = 0;
            StreamReader reader = null;
            try
            {
                deleteData(destinationTable);
                reader = new StreamReader(sourceFile);
                destinationTableSchema.BeginLoadData();
                string line = string.Empty;
                while (!reader.EndOfStream)
                {
                    try
                    {
                        
                        line = reader.ReadLine();
                        var values = line.Split(delimeter);
                        if (!string.IsNullOrEmpty(line) && values.Length == totalFields)
                        {
                            counter++;
                            loadData(destinationTableSchema, columnRowMapping, values);
                            if (counter >= 50000)
                            {
                                System.Threading.Thread.Sleep(500);
                                destinationTableSchema.EndLoadData();
                                saveData(destinationTableSchema, destinationTable);
                                counter = 0;
                                destinationTableSchema.Clear();
                                destinationTableSchema.BeginLoadData();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                        logMessage(string.Format("Data: {0}", line));
                        returnCode = 1;
                    }

                }

                if (counter > 0)
                {
                    try
                    {
                        destinationTableSchema.EndLoadData();
                        saveData(destinationTableSchema, destinationTable);
                    }
                    catch (Exception ex)
                    {
                        logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                        returnCode = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                logMessage(string.Format("Error occured while importing file into {0} table", destinationTable), ex);
                returnCode = 1;
            }
            finally
            {

                if (reader != null)
                    reader.Close();

            }
            return returnCode;
        }

        private static void saveData(DataTable dt, string tableName)
        {
            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
            {
                connection.Open();
                using (OracleBulkCopy bulkCopy = new OracleBulkCopy(connection))
                {
                    bulkCopy.BulkCopyOptions = OracleBulkCopyOptions.UseInternalTransaction;
                    bulkCopy.BulkCopyTimeout = 60000;
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.WriteToServer(dt);
                    logMessage(string.Format("Saved {0} records in {1} table", dt.Rows.Count, tableName));
                }
                connection.Close();
            }
        }

        private static DataTable createCCBMeterTable(out Dictionary<string, Tuple<string, bool, int>> data)
        {
            DataTable dt = new System.Data.DataTable("STG_CCB_METER_LOAD");
            data = new Dictionary<string, Tuple<string, bool, int>>();
            data["SERVICE_POINT_ID"] = new Tuple<string, bool, int>("System.String", false, 0);
            data["UNQSPID"] = new Tuple<string, bool, int>("System.String", true, 1);
            data["ACCT_ID"] = new Tuple<string, bool, int>("System.String", true, 3);
            data["SERVICE_AGREEMENT_ID"] = new Tuple<string, bool, int>("System.String", true, 4);
            data["METER_NUMBER"] = new Tuple<string, bool, int>("System.String", true, 5);
            data["CGC#12"] = new Tuple<string, bool, int>("System.String", true, 34);
            data["SVC_ST_#"] = new Tuple<string, bool, int>("System.String", true, 19);
            data["SVC_ST_NAME"] = new Tuple<string, bool, int>("System.String", true, 20);
            data["SVC_ST_NAME2"] = new Tuple<string, bool, int>("System.String", true, 21);
            data["SVC_CITY"] = new Tuple<string, bool, int>("System.String", true, 22);
            data["SVC_STATE"] = new Tuple<string, bool, int>("System.String", true, 23);
            data["SVC_ZIP"] = new Tuple<string, bool, int>("System.String", true, 24);
            data["SVC_DATE"] = new Tuple<string, bool, int>("System.String", true, 6);
            data["NAICS"] = new Tuple<string, bool, int>("System.String", true, 7);
            data["BILLING_CYCLE"] = new Tuple<string, bool, int>("System.String", true, 8);
            data["ROUTE"] = new Tuple<string, bool, int>("System.String", true, 10);
            data["LOCAL_OFFICE"] = new Tuple<string, bool, int>("System.String", true, 9);
            data["REV_ACCT_CD"] = new Tuple<string, bool, int>("System.String", true, 11);
            data["ESSENTIAL"] = new Tuple<string, bool, int>("System.String", true, 12);
            data["SENSITIVE"] = new Tuple<string, bool, int>("System.String", true, 13);
            data["LIFE_SUPPORT"] = new Tuple<string, bool, int>("System.String", true, 14);
            data["RATE_SCHED"] = new Tuple<string, bool, int>("System.String", true, 15);
            data["MAIL_NAME"] = new Tuple<string, bool, int>("System.String", true, 16);
            data["MAIL_NAME2"] = new Tuple<string, bool, int>("System.String", true, 17);
            data["AREA_CODE"] = new Tuple<string, bool, int>("System.Int32", true, 25);
            data["PHONE_NUMBER"] = new Tuple<string, bool, int>("System.Int32", true, 26);
            data["MAIL_ST_#"] = new Tuple<string, bool, int>("System.String", true, 28);
            data["MAIL_ST_NAME"] = new Tuple<string, bool, int>("System.String", true, 29);
            data["MAIL_ST_NAME2"] = new Tuple<string, bool, int>("System.String", true, 30);
            data["MAIL_CITY"] = new Tuple<string, bool, int>("System.String", true, 31);
            data["MAIL_STATE"] = new Tuple<string, bool, int>("System.String", true, 32);
            data["MAIL_ZIP"] = new Tuple<string, bool, int>("System.String", true, 33);
            data["ROBC"] = new Tuple<string, bool, int>("System.String", true, 35);
            data["FDR_#"] = new Tuple<string, bool, int>("System.String", true, 36);
            data["SSD_OPER"] = new Tuple<string, bool, int>("System.String", true, 37);
            data["PREMISE_TYPE"] = new Tuple<string, bool, int>("System.String", true, 39);
            data["REV_MONTH"] = new Tuple<string, bool, int>("System.String", true, 40);
            data["REV_KWHR"] = new Tuple<string, bool, int>("System.String", true, 41);
            data["REV_KW"] = new Tuple<string, bool, int>("System.String", true, 42);
            data["PFACTOR"] = new Tuple<string, bool, int>("System.String", true, 43);
            data["TOWNSHIP_TERRITORY_CD"] = new Tuple<string, bool, int>("System.String", true, 38);
            data["NEM"] = new Tuple<string, bool, int>("System.String", true, 44);
            data["SM_SP_STATUS"] = new Tuple<string, bool, int>("System.String", true, 45);
            data["MEDICAL_BASELINE"] = new Tuple<string, bool, int>("System.String", true, 48);
            data["COMMUNICATION_PREFERENCE"] = new Tuple<string, bool, int>("System.String", true, 49);
            data["ERROR_FLG"] = new Tuple<string, bool, int>("System.String", true, 50);
            data["ERROR_TXT"] = new Tuple<string, bool, int>("System.String", true, 51);
            data["CREATE_DATE"] = new Tuple<string, bool, int>("System.String", true, 52);
            addColumns(dt, data);
            return dt;
        }

        private static DataTable createSPGenLoadTable(out Dictionary<string, Tuple<string, bool, int>> data)
        {
            DataTable dt = new System.Data.DataTable("STG_SM_SP_GEN_LOAD");
            data = new Dictionary<string, Tuple<string, bool, int>>();
            data["CGC"] = new Tuple<string, bool, int>("System.Int64", false, 0);
            data["SERVICE_POINT_ID"] = new Tuple<string, bool, int>("System.String", false, 1);
            data["SP_PEAK_KW"] = new Tuple<string, bool, int>("System.Decimal", true, 2);
            data["VEE_SP_KW_FLAG"] = new Tuple<string, bool, int>("System.String", true, 3);
            data["SP_PEAK_TIME"] = new Tuple<string, bool, int>("System.String", true, 4);
            data["SP_KW_TRF_PEAK"] = new Tuple<string, bool, int>("System.Decimal", true, 5);
            data["VEE_TRF_KW_FLAG"] = new Tuple<string, bool, int>("System.String", true, 6);
            data["INT_LEN"] = new Tuple<string, bool, int>("System.Int32", true, 7);
            data["SP_PEAK_KVAR"] = new Tuple<string, bool, int>("System.Decimal", true, 8);
            data["TRF_PEAK_KVAR"] = new Tuple<string, bool, int>("System.Decimal", true, 9);
            data["ERROR_FLG"] = new Tuple<string, bool, int>("System.String", true, 10);
            data["ERROR_TXT"] = new Tuple<string, bool, int>("System.String", true, 11);
            data["CREATE_DATE"] = new Tuple<string, bool, int>("System.String", true, 12);
            addColumns(dt, data);
            return dt;
        }

        private static DataTable createSPLoadTable(out Dictionary<string, Tuple<string, bool, int>> data)
        {
            DataTable dt = new System.Data.DataTable("STG_SM_SP_LOAD");
            data = new Dictionary<string, Tuple<string, bool, int>>();
            data["CGC"] = new Tuple<string, bool, int>("System.Int64", false, 0);
            data["SERVICE_POINT_ID"] = new Tuple<string, bool, int>("System.String", false, 1);
            data["SP_PEAK_KW"] = new Tuple<string, bool, int>("System.Decimal", true, 2);
            data["VEE_SP_KW_FLAG"] = new Tuple<string, bool, int>("System.String", true, 3);
            data["SP_PEAK_TIME"] = new Tuple<string, bool, int>("System.String", true, 4);
            data["SP_KW_TRF_PEAK"] = new Tuple<string, bool, int>("System.Decimal", true, 5);
            data["VEE_TRF_KW_FLAG"] = new Tuple<string, bool, int>("System.String", true, 6);
            data["INT_LEN"] = new Tuple<string, bool, int>("System.Int32", true, 7);
            data["SP_PEAK_KVAR"] = new Tuple<string, bool, int>("System.Decimal", true, 8);
            data["TRF_PEAK_KVAR"] = new Tuple<string, bool, int>("System.Decimal", true, 9);
            data["ERROR_FLG"] = new Tuple<string, bool, int>("System.String", true, 10);
            data["ERROR_TXT"] = new Tuple<string, bool, int>("System.String", true, 11);
            data["CREATE_DATE"] = new Tuple<string, bool, int>("System.String", true, 12);
            addColumns(dt, data);
            return dt;
        }

        private static DataTable createTRFGenLoadTable(out Dictionary<string, Tuple<string, bool, int>> data)
        {
            DataTable dt = new System.Data.DataTable("STG_SM_TRF_GEN_LOAD");
            data = new Dictionary<string, Tuple<string, bool, int>>();
            data["CGC"] = new Tuple<string, bool, int>("System.Int64", false, 0);
            data["TRF_PEAK_KW"] = new Tuple<string, bool, int>("System.Decimal", true, 1);
            data["TRF_PEAK_TIME"] = new Tuple<string, bool, int>("System.String", true, 2);
            data["TRF_AVG_KW"] = new Tuple<string, bool, int>("System.Decimal", true, 3);
            data["ERROR_FLG"] = new Tuple<string, bool, int>("System.String", true, 4);
            data["ERROR_TXT"] = new Tuple<string, bool, int>("System.String", true, 5);
            data["CREATE_DATE"] = new Tuple<string, bool, int>("System.String", true, 6);
            addColumns(dt, data);
            return dt;
        }

        private static DataTable createTRFLoadTable(out Dictionary<string, Tuple<string, bool, int>> data)
        {
            DataTable dt = new System.Data.DataTable("STG_SM_TRF_LOAD");
            data = new Dictionary<string, Tuple<string, bool, int>>();
            data["CGC"] = new Tuple<string, bool, int>("System.Int64", false, 0);
            data["TRF_PEAK_KW"] = new Tuple<string, bool, int>("System.Decimal", true, 1);
            data["TRF_PEAK_TIME"] = new Tuple<string, bool, int>("System.String", true, 2);
            data["TRF_AVG_KW"] = new Tuple<string, bool, int>("System.Decimal", true, 3);
            data["ERROR_FLG"] = new Tuple<string, bool, int>("System.String", true, 4);
            data["ERROR_TXT"] = new Tuple<string, bool, int>("System.String", true, 5);
            data["CREATE_DATE"] = new Tuple<string, bool, int>("System.String", true, 6);
            addColumns(dt, data);
            return dt;
        }

        private static void loadData(DataTable dt, Dictionary<string, Tuple<string, bool, int>> rowDefination, string[] row)
        {
            DataRow newRow = dt.NewRow();

            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToUpper() == "CGC#12")
                {
                    if (!isValidCGC(row[rowDefination[dc.ColumnName].Item3]))
                        throw new ApplicationException("Invalid CGC#12 data.");
                }
                if (rowDefination[dc.ColumnName].Item3 >= row.Length)
                    continue;
                else
                    newRow[dc.ColumnName] = cleanData(row[rowDefination[dc.ColumnName].Item3], dc.DataType);
            }
            //newRow["REV_MONTH"] = "12";
            newRow["CREATE_DATE"] = DateTime.Now.ToString("MMddyy");
            dt.Rows.Add(newRow);
        }

        private static bool isValidCGC(string data)
        {
            long result = 0;
            if (long.TryParse(data, out result))
                return true;
            else
                return false;
        }

        private static object cleanData(string data, Type dataType)
        {
            if (string.IsNullOrEmpty(data))
                return System.DBNull.Value;
            else if (dataType == typeof(DateTime))
                return cleanDate(data).Date;
            else
                return data.Trim();
        }

        private static DateTime cleanDate(string data)
        {
            return DateTime.ParseExact(data, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        private static void deleteData(string tableName)
        {
            logMessage(string.Format("Truncationg table: {0}", tableName));
            using (Oracle.DataAccess.Client.OracleConnection connection = new Oracle.DataAccess.Client.OracleConnection(Constants.TLMConnectionString))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = string.Format("DELETE FROM {0} WHERE CREATE_DATE='{1}'", tableName, DateTime.Now.ToString("MMddyy"));
                    //cmd.CommandText = string.Format("TRUNCATE TABLE {0}", tableName);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
            logMessage(string.Format("End truncationg table: {0}", tableName));
        }

        private static void addColumns(DataTable dt, Dictionary<string, Tuple<string, bool, int>> columns)
        {
            foreach (string key in columns.Keys)
            {
                if (key.StartsWith("filler", StringComparison.CurrentCultureIgnoreCase))
                    continue;
                else
                {
                    var dc = new DataColumn();
                    dc.ColumnName = key;
                    dc.AllowDBNull = columns[key].Item2;

                    dc.DataType = System.Type.GetType(columns[key].Item1);
                    dt.Columns.Add(dc);
                }
            }
        }

        private static void cleanDirectory(string directoryPath)
        {
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                File.Delete(filePath);
            }
        }

        private static void logMessage(string message)
        {
            Common.WriteLog(message, Constants.TLMApplicationLogFile);
        }

        private static void logMessage(string message, Exception ex)
        {
            Common.WriteLog(message, Constants.TLMApplicationLogFile);
            Common.WriteLog(ex.ToString(), Constants.TLMApplicationLogFile);
        }

        private static bool triggerFileExist(string directoryPath, string file)
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(file))
                retVal = true;
            else
                retVal = File.Exists(Path.Combine(directoryPath, file));
            return retVal;
        }
    }
}
