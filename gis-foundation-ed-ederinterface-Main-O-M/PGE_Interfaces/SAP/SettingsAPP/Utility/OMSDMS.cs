using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data.EntityClient;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using Oracle.DataAccess.Client;

namespace Utility
{
    class OMSDMS
    {
        static OMSDMS()
        {
            // check to see if the working directories are created
            Common.CreateDirectory(Constants.OmsDmsExportFileFolder);
            DirectoryInfo dir = Directory.GetParent(Constants.OmsDmsLogFile);
            Common.CreateDirectory(dir.FullName);
        }

        public static int Process()
        {
            try
            {
                logMessage("Start of OMSDMS export process");

                if (!Common.TriggerFileExist(Constants.OmsDmsTriggerFile))
                {
                    DateTime processStartDateTime = DateTime.Now;
                    string fileName = Path.GetFileNameWithoutExtension(Constants.OmsDmsExportFileName);
                    string extension = Path.GetExtension(Constants.OmsDmsExportFileName);
                    fileName = string.Format("{0}{1}_{2}{3}", Constants.OmsDmsExportFileFolder, fileName, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), extension);
                    DateTime? startDate = GetLastRunDate();

                    export(fileName, startDate, processStartDateTime);

                    UpdateLastRunDate(processStartDateTime);
                    Common.CreateTriggerFile(Constants.OmsDmsTriggerFile);
                }
                return 0;
            }
            catch (Exception ex)
            {
                logMessage("Error occur during the export process.", ex);
                return 1;
            }
            finally
            {
                logMessage("End of OMSDMS export process");
            }
        }

        private static void export(string fileName, DateTime? startDate, DateTime? endDate)
        {
            /*Changes for ENOS to SAP migration - ED51 changes ..*/
            string strGenGUID = null;
            DataTable dtAllPrimaryGenerations = GetAllPrimaryGenerations();
            /*Changes for ENOS to SAP migration - ED51 changes ..*/

            using (XmlWriter w = new XmlTextWriter(fileName, Encoding.UTF8))
            {
                w.WriteStartElement("Devices");
                int counter = 0;
                using (OracleConnection connection = new OracleConnection(Constants.OmsDmsConnectionString))
                {
                    connection.Open();
                    foreach (string key in OMSDMS.deviceTableMapping().Keys)
                    {
                        counter = 0;
                        w.WriteStartElement("DeviceCategory");
                        w.WriteAttributeString("Type", key);
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            cmd.Connection = connection;

                            /*Changes for ENOS to SAP migration - ED51 changes .. Start*/
                            if (key == Constants.OmsDmsDeviceGenerationName)
                            {
                                if (startDate == null || endDate == null)
                                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE CURRENT_FUTURE='C'", OMSDMS.deviceTableMapping()[key]);
                                else
                                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE CURRENT_FUTURE='C' AND DATE_MODIFIED >= To_Date('{1}','MM-DD-YYYY HH-MI-SS AM') AND DATE_MODIFIED <= To_Date('{2}','MM-DD-YYYY HH-MI-SS AM')  UNION SELECT * FROM {0} WHERE CURRENT_FUTURE='C' AND DATECREATED >= To_Date('{1}','MM-DD-YYYY HH-MI-SS AM') AND DATECREATED <= To_Date('{2}','MM-DD-YYYY HH-MI-SS AM') UNION SELECT * FROM {0} WHERE CURRENT_FUTURE='C' AND GLOBAL_ID IN (SELECT GLOBAL_ID FROM EDSETT.SM_COMMENT_HIST WHERE WORK_DATE >= To_Date('{1}','MM-DD-YYYY HH-MI-SS AM') AND WORK_DATE <= To_Date('{2}','MM-DD-YYYY HH-MI-SS AM') AND GLOBAL_ID IS NOT NULL AND DEVICE_TABLE_NAME = '{0}')", OMSDMS.deviceTableMapping()[key], startDate.Value.ToString("MM-dd-yyyy hh:mm:ss tt"), endDate.Value.ToString("MM-dd-yyyy hh:mm:ss tt"));
                            }
                            else
                            {
                                if (startDate == null || endDate == null)
                                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE CURRENT_FUTURE='C'", OMSDMS.deviceTableMapping()[key]);
                                else
                                    cmd.CommandText = string.Format("SELECT * FROM {0} WHERE CURRENT_FUTURE='C' AND DATE_MODIFIED >= To_Date('{1}','MM-DD-YYYY HH-MI-SS AM') AND DATE_MODIFIED <= To_Date('{2}','MM-DD-YYYY HH-MI-SS AM') UNION SELECT * FROM {0} WHERE CURRENT_FUTURE='C' AND GLOBAL_ID IN (SELECT GLOBAL_ID FROM EDSETT.SM_COMMENT_HIST WHERE WORK_DATE >= To_Date('{1}','MM-DD-YYYY HH-MI-SS AM') AND WORK_DATE <= To_Date('{2}','MM-DD-YYYY HH-MI-SS AM') AND GLOBAL_ID IS NOT NULL AND DEVICE_TABLE_NAME = '{0}')", OMSDMS.deviceTableMapping()[key], startDate.Value.ToString("MM-dd-yyyy hh:mm:ss tt"), endDate.Value.ToString("MM-dd-yyyy hh:mm:ss tt"));
                                //cmd.CommandText = string.Format("SELECT * FROM {0} WHERE CURRENT_FUTURE='C' AND DATE_MODIFIED >= To_Date('{1}','MM-DD-YYYY') AND DATE_MODIFIED <= To_Date('{2}','MM-DD-YYYY')", OMSDMS.deviceTableMapping()[key], startDate.Value.ToString("MM-dd-yyyy"), endDate.Value.ToString("MM-dd-yyyy")); --INC000004340052
                            }

                            logMessage("Command Text used: " + cmd.CommandText);
                            /*Changes for ENOS to SAP migration - ED51 changes .. End*/
                            
                            OracleDataReader myReader = cmd.ExecuteReader();
                            while (myReader.Read())
                            {
                                /*Changes for ENOS to SAP migration - ED51 changes ..*/
                                if (key == Constants.OmsDmsDeviceGenerationName)
                                {
                                    // Get GenerationGUID here and send to a function
                                    strGenGUID = Convert.ToString(myReader[Constants.OmsDmsglobalIDFieldNameSettings]);

                                    if (GenerationIsPrimary(strGenGUID, dtAllPrimaryGenerations))
                                    {
                                        counter++;
                                        w.WriteStartElement("Device");
                                        for (int x = 0; x < myReader.FieldCount; x++)
                                        {
                                            writeField(w, myReader.GetName(x), myReader[x]);
                                        }
                                        w.WriteEndElement();
                                    }
                                }
                                else
                                {
                                    counter++;
                                    w.WriteStartElement("Device");
                                    for (int x = 0; x < myReader.FieldCount; x++)
                                    {
                                        writeField(w, myReader.GetName(x), myReader[x]);
                                    }
                                    w.WriteEndElement();
                                }
                                /*Changes for ENOS to SAP migration - ED51 changes ..*/
                            }
                            logMessage(string.Format("{0} {1} exported.", counter, key));
                        }
                        w.WriteEndElement();
                    }
                    connection.Close();
                }
                w.WriteEndElement();
            }
        }

        private static void writeField(XmlWriter w, object fieldName, object fieldValue)
        {
            w.WriteStartElement("Field");
            w.WriteAttributeString("Name", (fieldName != null ? fieldName.ToString() : ""));
            w.WriteAttributeString("Value", (fieldValue != null ? fieldValue.ToString() : ""));
            w.WriteEndElement();
        }

        private static DateTime? GetLastRunDate()
        {
            DateTime? retVal = null;
            using (OracleConnection connection = new OracleConnection(Constants.OmsDmsConnectionString))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = string.Format("SELECT LAST_RUN_DATE FROM SM_UTILITY WHERE PROCESS_NAME='{0}'", Constants.OMSProcess);
                    OracleDataReader myReader = cmd.ExecuteReader();
                    while (myReader.Read())
                    {
                        if(myReader["LAST_RUN_DATE"] != null)
                            retVal = DateTime.Parse(myReader["LAST_RUN_DATE"].ToString());
                    }
                }
            }
            return retVal;
        }

        //private static void UpdateLastRunDate(DateTime processStartDateTime)
        //{
        //    using (OracleConnection connection = new OracleConnection(Constants.OmsDmsConnectionString))
        //    {
        //        connection.Open();
        //        using (OracleCommand cmd = new OracleCommand())
        //        {
        //            cmd.Connection = connection;
        //            cmd.CommandText = string.Format("UPDATE SM_UTILITY SET LAST_RUN_DATE=To_Date('{0}','MM-DD-YYYY HH24:MI:SS') WHERE PROCESS_NAME='{1}'", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss"), Constants.OMSProcess);
        //            var NoOfRows = cmd.ExecuteNonQuery();
        //            if (NoOfRows <= 0)
        //            {
        //                cmd.CommandText = string.Format("INSERT INTO SM_UTILITY(PROCESS_NAME,PROCESS_DESCRIPTION,LAST_RUN_DATE) VALUES('{0}','{1}',To_Date('{2}','MM-DD-YYYY HH24:MI:SS'))", Constants.OMSProcess, "OMS DMS Export", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss"));
        //                cmd.ExecuteNonQuery();
        //            }
        //        }
        //    }
        //}

        private static void UpdateLastRunDate(DateTime processStartDateTime)
        {
            using (OracleConnection connection = new OracleConnection(Constants.OmsDmsConnectionString))
            {
                connection.Open();
                using (OracleCommand cmd = new OracleCommand())
                {
                    cmd.Connection = connection;
                    cmd.CommandText = string.Format("UPDATE SM_UTILITY SET LAST_RUN_DATE=To_Date('{0}','MM-DD-YYYY HH:MI:SS AM') WHERE PROCESS_NAME='{1}'", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss tt"), Constants.OMSProcess);
                    var NoOfRows = cmd.ExecuteNonQuery();
                    if (NoOfRows <= 0)
                    {
                        cmd.CommandText = string.Format("INSERT INTO SM_UTILITY(PROCESS_NAME,PROCESS_DESCRIPTION,LAST_RUN_DATE) VALUES('{0}','{1}',To_Date('{2}','MM-DD-YYYY HH:MI:SS AM'))", Constants.OMSProcess, "OMS DMS Export", processStartDateTime.ToString("MM-dd-yyyy hh:mm:ss tt"));
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        private static Dictionary<string, string> deviceTableMapping()
        {
            Dictionary<string, string> retVal = new Dictionary<string, string>();

            /* Changes for Fuse Saver Start*/
            string[] mappingKey = Constants.DeviceMappingKey.Split(',');
            string[] mappingValue = Constants.DeviceMappingValue.Split(',');

            if (mappingKey.Length == mappingValue.Length)
            {
                for (int count = 0; count < mappingKey.Length; count++)
                {
                    retVal.Add(mappingKey[count], mappingValue[count]);
                }
            }
            else
            {
                logMessage("Device Mapping Key-Value Mismatch. Key Count: " + mappingKey.Length + " Value Count: " + mappingValue.Length);
            }
            //retVal["Capacitor"] = "SM_CAPACITOR";
            //retVal["CircuitBreaker"] = "SM_CIRCUIT_BREAKER";
            //retVal["Interrupter"] = "SM_INTERRUPTER";
            //retVal["NetworkProtector"] = "SM_NETWORK_PROTECTOR";
            //retVal["Recloser"] = "SM_RECLOSER";
            //retVal["Recloser_TS"] = "SM_RECLOSER_TS";
            //retVal["Regulator"] = "SM_REGULATOR";
            //retVal["Sectionalizer"] = "SM_SECTIONALIZER";
            //retVal["Switch"] = "SM_SWITCH";
            ///*Changes for ENOS to SAP migration - ED51 changes ..*/
            //retVal["Generation"] = "SM_GENERATION";

            ///* Changes for Fuse Saver ..*/
            //retVal["Recloser_FS"] = "SM_RECLOSER_FS";
            return retVal;

            /* Changes for Fuse Saver End*/
        }


        private static void logMessage(string message)
        {
            Common.WriteLog(message,Constants.OmsDmsLogFile);
        }

        private static void logMessage(string message, Exception ex)
        {
            Common.WriteLog(message,Constants.OmsDmsLogFile);
            Common.WriteLog(ex.ToString(), Constants.OmsDmsLogFile);
        }

        /*Changes for ENOS to SAP migration - ED51 changes ..*/
        /// <summary>
        /// This function tell whether a generation is primary generation or not 
        /// </summary>
        /// <param name="argStrGenGUID"></param>
        /// <returns></returns>
        public static bool GenerationIsPrimary(string argStrGenGUID, DataTable argDtAllPrimGenerations)
        {
            bool bGenIsPrimary = false;
            try
            {
                string genQuery = Constants.OmsDmsglobalIDFieldNameEDER + "='" + argStrGenGUID + "'";

                if (argDtAllPrimGenerations.Select(genQuery).Length > 0)
                {
                    bGenIsPrimary = true;
                }
            }
            catch (Exception exp)
            {
                //throw;
            }
            return bGenIsPrimary;
        }

        /*Changes for ENOS to SAP migration - ED51 changes ..*/
        /// <summary>
        /// This function fatches all primary generations
        /// </summary>
        /// <returns></returns>
        public static DataTable GetAllPrimaryGenerations()
        {
            DataTable dtAllPrimGenerations = null;
            try
            {
                using (OracleConnection oracleconn = new OracleConnection(Constants.OmsDmsEDERConnectionString))
                {
                    oracleconn.Open();
                    using (OracleCommand cmd = new OracleCommand(Constants.OmsDmsprimGenQuery, oracleconn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 600;

                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            dtAllPrimGenerations = ds.Tables[0];
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                // throw;
            }
            return dtAllPrimGenerations;
        }
    }
}
