using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.DataArchiving
{
    class Program
    {

        public static Log4NetLogger _log = new Log4NetLogger(System.Configuration.ConfigurationManager.AppSettings["LogConfigName"].ToString());
        /// <summary>
        /// start the process
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                _log.Info("Archiving process started, read values from config file");

                string sTableDetails = System.Configuration.ConfigurationManager.AppSettings["Table_Details"].ToString();

                _log.Info("Archiving of table  started");
                if (ArchiveData(sTableDetails) == true)
                {
                    _log.Info("Archiving of table  successfully completed");
                }
                else
                {
                    _log.Error("Archiving of table failed");
                }
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
            }
        }
        /// <summary>
        ///Validate the Input string as string.
        /// </summary>
        /// <param name="Table_Info"></param>
        /// <returns></returns>
        private static bool ValidateStringIncorrect(string Table_Info)
        {
            bool _retval = false;
            string[] Table_Details = Table_Info.Split('$');
            try
            {
                if (Table_Details.Length == 4)
                {
                    for (int i = 0; i < Table_Details.Length; i++)
                    {
                        string Field = Table_Details[i].Trim();
                        if (string.IsNullOrEmpty(Field))
                        {
                            _log.Error("Error encountered.A field is left null or empty.Please verify the Table_Details --" + Table_Info);
                            _retval = true;
                            break;
                        }
                    }
                }
                else
                {
                    _log.Error("The input Parameters are not in correct format(<ConncetionString$TableName$TimeInDays$ColumnToRefer>),Please verify the Table_Details --" + Table_Info);
                    _retval = true;
                }                
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message + " at " + exp.StackTrace);
            }
            return _retval;
        }

        /// <summary>
        /// Archive the data of the given table
        /// </summary>
        /// <param name="sTableDetails"></param>
        /// <returns></returns>
        private static bool ArchiveData(string sTableDetails)
        {
            bool _retval = false;
            try
            {
                if (string.IsNullOrEmpty(sTableDetails))
                {
                    _log.Error("Table details are not getting from config file,please check the config value to run the process.");
                    return false;
                }

                string[] Table = sTableDetails.Split(';');

                for (int i = 0; i < Table.Length; i++)
                {
                    if (ValidateStringIncorrect(Table[i]) == false)
                    {
                        string[] Table_Info = Table[i].Split('$');
                        // M4JF EDGISREARCH 919 -  Get connection string usin password management tool
                        string sConnectionstring = default;
                        //string sConnectionstring = Table_Info[0];
                        //string TableName = Table_Info[1];
                        //string DaysDiffernce = Table_Info[2];
                        //string DateField = Table_Info[3];
                        if (i == 0)
                        {
                          sConnectionstring = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToUpper());
                        }
                        else
                        {
                          sConnectionstring = ReadEncryption.GetConnectionStr(System.Configuration.ConfigurationManager.AppSettings["WIP_ConnectionStr"].ToUpper());
                        }
                        string TableName = Table_Info[1];
                        string DaysDiffernce = Table_Info[2];
                        string DateField = Table_Info[3];

                        _log.Info("Deleteing data from " + Table_Info[1] + " for days prior to last " + Table_Info[2] + " days.");
                        (new DBHelper()).DeleteData(TableName, DaysDiffernce, DateField, sConnectionstring);
                    }
                    else
                    {
                        return _retval;
                    }                   
                    _retval= true;
                }                
            }
            catch (Exception exp)
            {
                _log.Error(exp.Message +" at "+exp.StackTrace);
            }
            return _retval;
        }
    }
}
