using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Oracle.DataAccess.Client;

namespace OracleTableToCSV
{
    class Program
    {
        private static string sTemp = string.Empty;
        private static string sCommand = string.Empty;
        static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["TABLENAME"]) && string.IsNullOrEmpty(ConfigurationManager.AppSettings["FULLQUERY"])) return;
            //WritetoCSV(RecordSet());
            OracleReaderApproach();
        }

        private static void OracleReaderApproach()
        {
            OracleConnection pOConn = new OracleConnection(ConfigurationManager.AppSettings["CONNECTION_STRING"]);
            string sCSVPath = string.Empty;
            string[] sMultiTable;
            switch (ConfigurationManager.AppSettings["INPUT"])
            {
                case "QUERYFILEPATH":

                    //if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["QUERYFILEPATH"]))
                    {
                        FileInfo pFileQuery = new FileInfo(ConfigurationManager.AppSettings["QUERYFILEPATH"]);
                        sCSVPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["EXPORT_PATH"]) ?
                                   System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :
                                   ConfigurationManager.AppSettings["EXPORT_PATH"])
                                   + "\\" + (String.IsNullOrEmpty(ConfigurationManager.AppSettings["CSVFILENAME"]) ? pFileQuery.Name : ConfigurationManager.AppSettings["CSVFILENAME"])
                                   + ((!String.IsNullOrEmpty(ConfigurationManager.AppSettings["APPENDDATE"]) && (ConfigurationManager.AppSettings["APPENDDATE"].ToUpper() == "Y")) ? "_" + DateTime.Today.ToString("MMddyyyy") : string.Empty)
                                   + ".csv";
                        if (File.Exists(sCSVPath)) File.Delete(sCSVPath);
                        StreamWriter pSWriter = new StreamWriter(sCSVPath);

                        ExecuteCommand(File.ReadAllText(ConfigurationManager.AppSettings["QUERYFILEPATH"]), pOConn, pSWriter);
                        break;
                    }
                case "TABLENAME":
                    //else if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TABLENAME"]))
                    {
                        sMultiTable = ConfigurationManager.AppSettings["TABLENAME"].Split(',');
                        foreach (string sTablename in sMultiTable)
                        {
                            sCSVPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["EXPORT_PATH"]) ?
                                       System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :
                                       ConfigurationManager.AppSettings["EXPORT_PATH"])
                                       + "\\" + sTablename
                                       + ((!String.IsNullOrEmpty(ConfigurationManager.AppSettings["APPENDDATE"]) && (ConfigurationManager.AppSettings["APPENDDATE"].ToUpper() == "Y")) ? "_" + DateTime.Today.ToString("MMddyyyy") : string.Empty)
                                       + ".csv";
                            if (File.Exists(sCSVPath)) File.Delete(sCSVPath);
                            StreamWriter pSWriter = new StreamWriter(sCSVPath);

                            sCommand = "SELECT ";
                            sCommand += string.IsNullOrEmpty(sTemp = ConfigurationManager.AppSettings["OUTFIELD"]) ? "*" : sTemp;
                            sCommand += " FROM ";
                            sCommand += sTablename;
                            sCommand += string.IsNullOrEmpty(sTemp = ConfigurationManager.AppSettings["WHERECLAUSE"]) ? string.Empty : " WHERE " + sTemp;
                            ExecuteCommand(sCommand, pOConn, pSWriter);
                        }
                        break;
                    }
                // else
                case "FULLQUERY":
                    {
                        sCSVPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["EXPORT_PATH"]) ?
                                   System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :
                                   ConfigurationManager.AppSettings["EXPORT_PATH"])
                                   + "\\" + (String.IsNullOrEmpty(ConfigurationManager.AppSettings["CSVFILENAME"]) ? "TEMP_CSV" : ConfigurationManager.AppSettings["CSVFILENAME"])
                                   + ((!String.IsNullOrEmpty(ConfigurationManager.AppSettings["APPENDDATE"]) && (ConfigurationManager.AppSettings["APPENDDATE"].ToUpper() == "Y")) ? "_" + DateTime.Today.ToString("MMddyyyy") : string.Empty)
                                   + ".csv";
                        if (File.Exists(sCSVPath)) File.Delete(sCSVPath);
                        StreamWriter pSWriter = new StreamWriter(sCSVPath);
                        ExecuteCommand(ConfigurationManager.AppSettings["FULLQUERY"], pOConn, pSWriter);
                        break;
                    }
            }
        }

        private static void ExecuteCommand(string sCommand, OracleConnection pOConn, StreamWriter pSWriter)
        {
            try
            {
                OracleCommand pOCommand = new OracleCommand(sCommand, pOConn) { CommandTimeout = 0, CommandType = CommandType.Text };
                pOCommand.Connection.Open();
                using (pOConn)
                {
                    using (OracleDataReader oDReader = pOCommand.ExecuteReader())
                    using (pSWriter)
                    {
                        DataTable pDTable = new DataTable();
                        for (int iHeader = 0; iHeader < oDReader.FieldCount; iHeader++)
                        {
                            pDTable.Columns.Add(oDReader.GetName(iHeader));
                        }
                        pSWriter.WriteLine(string.Join(ConfigurationManager.AppSettings["COLUMNSEPERATOR"], pDTable.Columns.Cast<DataColumn>().Select(csvfile => csvfile.ColumnName)));
                        string sRow = string.Empty;
                        while (oDReader.Read())
                        {
                            sRow = string.Empty;
                            for (int iRow = 0; iRow < oDReader.FieldCount; iRow++)
                            {
                                try
                                {
                                    sRow += Convert.ToString(oDReader[iRow]).Replace(ConfigurationManager.AppSettings["COLUMNSEPERATOR"], ConfigurationManager.AppSettings["REPLACEEXISTINGSEPEARATOR"]) + ConfigurationManager.AppSettings["COLUMNSEPERATOR"];
                                }
                                catch
                                {
                                    sRow += "ExtractError" + ConfigurationManager.AppSettings["COLUMNSEPERATOR"];
                                }
                            }
                            sRow = sRow.Replace("\n", string.Empty).Replace("\r", string.Empty).Replace("\n\r", string.Empty).Replace("\r\n", string.Empty).Replace("\t", string.Empty).Replace(Environment.NewLine, string.Empty);
                            pSWriter.WriteLine(sRow.Substring(0, sRow.Length - 1));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //WriteLine(DateTime.Now.ToLongTimeString() + " Error while GetRecordtoProcess" + ex.Message);
                // return pDSet;
            }
            finally
            {
                pOConn.Close();
            }
        }

        private static void WritetoCSV(DataSet dataSet)
        {
            string sCSVPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["EXPORT_PATH"]) ?
                System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :
                ConfigurationManager.AppSettings["EXPORT_PATH"])
                + "\\" + (String.IsNullOrEmpty(ConfigurationManager.AppSettings["CSVFILENAME"]) ? dataSet.Tables[0].TableName : ConfigurationManager.AppSettings["CSVFILENAME"])
                + ((!String.IsNullOrEmpty(ConfigurationManager.AppSettings["APPENDDATE"]) && (ConfigurationManager.AppSettings["APPENDDATE"].ToUpper() == "Y")) ? "_" + DateTime.Today.ToString("MMddyyyy") : string.Empty)
                + ".csv";
            if (File.Exists(sCSVPath)) File.Delete(sCSVPath);
            StringBuilder sb = new StringBuilder();
            DataTable dt = dataSet.Tables[0];
            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(ConfigurationManager.AppSettings["COLUMNSEPERATOR"], columnNames));
            File.WriteAllText(sCSVPath, sb.ToString());
            foreach (DataRow row in dt.Rows)
            {
                File.AppendAllText(sCSVPath, Environment.NewLine);
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                //sb.AppendLine(string.Join(ConfigurationManager.AppSettings["COLUMNSEPERATOR"], fields));//.ToString().Replace(ConfigurationManager.AppSettings["COLUMNSEPERATOR"], ConfigurationManager.AppSettings["REPLACEEXISTANCE"])));
                File.AppendAllText(sCSVPath, string.Join(ConfigurationManager.AppSettings["COLUMNSEPERATOR"], fields));
                //dt.Rows.Remove(row);
            }

            //File.WriteAllText(sCSVPath, sb.ToString());
        }
        private static DataSet RecordSet()
        {
            //WriteLine(DateTime.Now.ToLongTimeString() + " -- GetRecordtoProcess");
            DataSet pDSet = new DataSet();
            OracleConnection pOConn = new OracleConnection(ConfigurationManager.AppSettings["CONNECTION_STRING"]);
           
            try
            {
                pOConn.Open();
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TABLENAME"]))
                {
                    sCommand = "SELECT ";
                    sCommand += string.IsNullOrEmpty(sTemp = ConfigurationManager.AppSettings["OUTFIELD"]) ? "*" : sTemp;
                    sCommand += " FROM ";
                    sCommand += ConfigurationManager.AppSettings["TABLENAME"];
                    sCommand += string.IsNullOrEmpty(sTemp = ConfigurationManager.AppSettings["WHERECLAUSE"]) ? string.Empty : " WHERE " + sTemp;
                }
                else
                {
                    sCommand = ConfigurationManager.AppSettings["FULLQUERY"];
                }

                OracleDataAdapter pODAdaptor = new OracleDataAdapter(new OracleCommand() { Connection = pOConn, CommandType = CommandType.Text, CommandText = sCommand });
               
                pODAdaptor.Fill(pDSet);
                pDSet.Tables[0].TableName = string.IsNullOrEmpty(ConfigurationManager.AppSettings["TABLENAME"]) ? "TEMP" : ConfigurationManager.AppSettings["TABLENAME"];
                //WriteLine(DateTime.Now.ToLongTimeString() + " -- GetRecordtoProcess Record Count" + pDSet.Tables[0].Rows.Count.ToString());
                return pDSet;
            }
            catch (Exception ex)
            {
                //WriteLine(DateTime.Now.ToLongTimeString() + " Error while GetRecordtoProcess" + ex.Message);
                return pDSet;
            }
            finally
            {
                pOConn.Close();
            }
        }
    }
}
