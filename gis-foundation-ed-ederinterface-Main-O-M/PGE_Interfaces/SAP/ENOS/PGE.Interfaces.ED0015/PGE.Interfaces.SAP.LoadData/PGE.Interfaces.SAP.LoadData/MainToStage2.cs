using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using PGE.Interfaces.SAP.LoadData.Classes;
using System.Reflection;
using System.Globalization;


namespace PGE.Interfaces.SAP.LoadData
{
    public class MainToStage2
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
       
        private List<KeyValuePair<string, GenerationInfo>> _lstOfActionsForGenInfo;

        public bool StartProcess()
        {
            bool _processSuccessfull = false;
            try
            {
                ReadConfigurations.ReadFromConfiguration();
                //_listOfGenInfo = new List<GenerationInfo>();
                _lstOfActionsForGenInfo = new List<KeyValuePair<string, GenerationInfo>>();
                _log.Info("Loading Data From Main to Stage2 starting up, loading the config.");
                if (!string.IsNullOrEmpty(ReadConfigurations.EDER_CONNECTIONSTRING) && !string.IsNullOrEmpty(ReadConfigurations.SETTINGS_CONNECTION_STRING))
                {
                    // DBCHNG-8.
                    //using (OracleConnection connection = Common.GetDBConnection(ReadConfigurations.EDER_CONNECTIONSTRING))
                    //{

                        string stageTabQueryString = ReadConfigurations.QUERY_STRING_ON_SUMMARY_TABLE;
                        string mainTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_TABLE;
                        string cmdText = string.Format(stageTabQueryString, ReadConfigurations.STAGE1_SUMMARY_TABLE_NAME);

                        //OracleCommand cmdSQL = new OracleCommand();
                        //cmdSQL.Connection = connection;
                        //cmdSQL.CommandText = cmdText;
                        //cmdSQL.CommandType = CommandType.Text;

                        _log.Info("Reading 'PGEDATA.GEN_SUMMARY_STAGE' table for Update & Delete Actions...");

                        OracleDataReader dataReader = cls_DBHelper_For_EDER.GetDataReaderByQuery(cmdText)  ;// cmdSQL.ExecuteReader();
                        if (dataReader.HasRows)
                        {
                            DataTable stage1Table = new DataTable();
                            stage1Table.Load(dataReader);
                            if (stage1Table.Rows.Count > 0)
                            {
                                _processSuccessfull = true ;
                                foreach (DataRow row in stage1Table.Rows)
                                {
                                    string spId = row[0].ToString();
                                    string guidValue = row[1].ToString();
                                    string action = row[2].ToString();
                                    Guid currentRowGUID;
                                    if (!string.IsNullOrEmpty(guidValue) && TryParseGuid(guidValue, out currentRowGUID) && !string.IsNullOrEmpty(spId))
                                    {
                                        cmdText = string.Format(mainTabQueryString, ReadConfigurations.MAIN_GEN_INFO_TABLE_NAME, guidValue);
                                        // DBCHNG-9.
                                        //cmdSQL.CommandText = cmdText;
                                        //dataReader = cmdSQL.ExecuteReader();
                                         dataReader = cls_DBHelper_For_EDER.GetDataReaderByQuery(cmdText)  ;
                                        if (dataReader.HasRows)
                                        {
                                            while (dataReader.Read())
                                            {
                                                Dictionary<string, object> rowToDict = Enumerable.Range(0, dataReader.FieldCount).ToDictionary(i => dataReader.GetName(i), i => dataReader.GetValue(i));
                                                GenerationInfo existingGen = new GenerationInfo(rowToDict, spId);
                                                _lstOfActionsForGenInfo.Add(new KeyValuePair<string, GenerationInfo>(action, existingGen));
                                            }
                                            dataReader.Dispose();
                                        }

                                        else
                                        {
                                            try
                                            {
                                                _log.Error("GLOBALID sent by SAP does not exists Main GenerationInfo table, unable to process further");
                                                string updateQuery = ReadConfigurations.UPDATE_STRING_ON_SUMMARY_TABLE;
                                                // m4jf - updated processedtime field
                                                cmdText = string.Format(updateQuery, ReadConfigurations.STAGE1_SUMMARY_TABLE_NAME, "F", "GLOBALID not exist in EDGIS", spId, guidValue);
                                                // DBCHNG-10.
                                                //cmdSQL.CommandText = cmdText;
                                                var result = cls_DBHelper_For_EDER.UpdateQuery(cmdText ) ;//cmdSQL.ExecuteNonQuery();
                                                //transaction.Commit();
                                                _log.Info("Successfully Updated Status for the failed GLOBALID sent by SAP");
                                            }
                                            catch (Exception ex1)
                                            {
                                                _processSuccessfull = false;
                                                _log.Error("Failed to Update Status as 'F' for " + ex1.Message);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _log.Error("Error proccessing records from Main to Stage2,GUID value is null, unable to process");
                                    }


                                
                                }
                                if (_lstOfActionsForGenInfo.Count > 0)
                                {
                                    //Till here, we jsut prepared class objects for the Updates & deletes in GEN_SUMMARY_STAGE table, now
                                    //we will get the existing data from SETTINGS database
                                    GetSettingsData(_lstOfActionsForGenInfo);
                                    //The main method , which will inserts existing data in Secondary stage tables.
                                    InsertDataInStage2Settings(_lstOfActionsForGenInfo);

                                    _processSuccessfull = true;
                                    _log.Info("Main to Stage2...Process Completed");
                                }
                            }
                        }
                        else
                        {
                            _processSuccessfull = true;
                        }
                   // }
                }
                else
                {
                    _log.Error("Conenction strings for both EDER & SETTINGS database should not be blank");
                }
            }
            catch (Exception ex)
            {
                _log.Error("Error in Starting process of Main to Stage2, function StartProcess: "+ex.Message);
                _lstOfActionsForGenInfo = null;
                _processSuccessfull = false;
            }
            return _processSuccessfull;
        }

      
        private void GetCommitChangesmade(OracleConnection connection)
        {
            try
            {
                OracleCommand cmdSQL = new OracleCommand();
                cmdSQL.Connection = connection;
                cmdSQL.CommandText = "COMMIT";
                cmdSQL.CommandType = CommandType.Text;
                var test = cmdSQL.ExecuteNonQuery();
                _log.Info("Successfully committed changes made");
            }
            catch (Exception ex)
            {
                _log.Info("Failed to commit changes,function GetCommitChangesmade, Error Message : " + ex.Message);
            }

        }
        /// <summary>
        /// This method inserts the data that we got from actual tables in SETTINGS database to secondary stage tables in PEGEDATA schema
        /// </summary>
        /// <param name="_lstOfActionsForGenInfo"></param>
        /// <param name="connection"></param>
        private void InsertDataInStage2Settings(List<KeyValuePair<string, GenerationInfo>> _lstOfActionsForGenInfo)
        {
            try
            {
                // DBCHNG-11.
                //using (connection)
                //{
                    if (ReadConfigurations.TRIGGERS_ASSIGNED_IN_STAGE2 == "Y")
                    {
                       // bool executed = ToggleTriggers(connection, ReadConfigurations.TRIGGERS_ASSIGNED_IN_STAGE2);
                        bool executed = ToggleTriggers(ReadConfigurations.TRIGGERS_ASSIGNED_IN_STAGE2);
                    }
                    foreach (KeyValuePair<string, GenerationInfo> item in _lstOfActionsForGenInfo)
                    {
                        GenerationInfo existingGen = item.Value;
                        var action = item.Key;
                        string sqlUpdateproccesedtime = default;
                        try
                        {
                           // string insertedOIDInStage2GenInfo = InsertDataInStage2GenInfo(existingGen, connection, action);
                            string insertedOIDInStage2GenInfo = InsertDataInStage2GenInfo(existingGen, action);

                            if (!string.IsNullOrEmpty(insertedOIDInStage2GenInfo))
                            {
                                _log.Info("Successfully inserted row in stage 2 GenerationInfo table with OID: " + insertedOIDInStage2GenInfo);
                                _log.Info("Now Inserting SETTINGS related data in Stage 2 .........");

                            

                            // InsertDataInStage2SettingsTables<SmGeneration>(existingGen._ListOfSmGen, connection, action);
                            InsertDataInStage2SettingsTables<SmGeneration>(existingGen._ListOfSmGen, action);

                                _log.Info("Successfully inserted Data in SETTINGS tables in stage 2 for GenerationInfo OID: " + insertedOIDInStage2GenInfo);
                            }
                            else
                            {
                                _log.Error("Unable to proceed further for existing data in Generation Info table with OID: " + existingGen.OBJECTID.ToString());
                            }

                        }
                        catch (Exception ex)
                        {
                            _log.Error("Error in function InsertDataInStage2Settings: " + ex.Message);
                        }
                    }
               // }
            }
            catch (Exception ex)
            {
                _log.Error("Error in function InsertDataInStage2Settings: " + ex.Message);
            }
            finally
            {
                //ToggleTriggers(connection, "N");
            }
        }

       

      //  private bool ToggleTriggers(OracleConnection connection, string p)
        private bool ToggleTriggers( string p)
        {
            try
            {
                // DBCHNG-11.--Need to Check
                string sqlQuery = string.Empty;
                //OracleCommand cmdSQL = new OracleCommand();
                //cmdSQL.Connection = connection;
                //If triggers are enabled , run the stored procedure to disable them
                if (p == "Y")
                {
                    //cmdSQL.CommandText = ReadConfigurations.SP_NAME_TO_DISABLE_TRIGGERS;
                    sqlQuery  = ReadConfigurations.SP_NAME_TO_DISABLE_TRIGGERS;
                }
                else if (p == "N")
                {
                    //cmdSQL.CommandText = ReadConfigurations.SP_NAME_TO_ENABLE_TRIGGERS;
                    sqlQuery = ReadConfigurations.SP_NAME_TO_ENABLE_TRIGGERS;
                }
               // cmdSQL.CommandType = CommandType.StoredProcedure;
                //var result = cmdSQL.ExecuteNonQuery();
                bool result = cls_DBHelper_For_EDER.ExecuteStoredProcedureWithoutParameter(ReadConfigurations.SP_NAME_TO_ENABLE_TRIGGERS);
                return result;
               // if (result != null && (int)result == -1)
               
                //{
                //    return true;
                //}
                //return false;
            }
            catch (Exception ex)
            {
                _log.Error("Error in function ToggleTriggers: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Method which inserts data in Secodnary stage tables in PGEDATA schema in EDER database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        //private bool InsertDataInStage2SettingsTables<T>(List<T> list, OracleConnection connection, string action)
        private bool InsertDataInStage2SettingsTables<T>(List<T> list,string action)
        {
            try
            {
                //if (connection.State == ConnectionState.Closed)
                //    connection.Open();
                foreach (T item in list)
                {
                    //Get Feilds required to for the current table
                    string fieldsForTable = string.Empty;
                    string currentTabName = string.Empty;
                    Type nextTypeToInsert = null;
                    GetCorrespondingValues<T>(ref fieldsForTable, ref currentTabName, ref nextTypeToInsert);
                    string[] arrayOfFields = fieldsForTable.Split(',');
                    string fieldvalueParam = string.Join(",", arrayOfFields.Select(x => ":" + x).ToArray());
                    string stageTabQueryString = string.Empty;

                    if (typeof(T) == typeof(GenerationInfo))
                    {
                        stageTabQueryString = "INSERT INTO {0}(" + fieldsForTable + ") VALUES(" + fieldvalueParam + ") returning OBJECTID into :myOId";
                    }
                    else
                    {
                        stageTabQueryString = "INSERT INTO {0}(" + fieldsForTable + ") VALUES(" + fieldvalueParam + ") returning ID into :myOId";
                    }

                    string cmdText = string.Format(stageTabQueryString, currentTabName);

                    OracleCommand cmdSQL = new OracleCommand();
                    //cmdSQL.Connection = connection;
                    cmdSQL.CommandText = cmdText;
                    cmdSQL.CommandType = CommandType.Text;
                    cmdSQL.Parameters.AddRange(GetParameterCollection<T>(item, action, arrayOfFields).ToArray());

                    cmdSQL.Parameters.Add(new OracleParameter("myOId", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                    //cmdSQL.ExecuteNonQuery();
                    //var oid = cmdSQL.Parameters["myOId"].Value;
                    object oid = cls_DBHelper_For_EDER.UpdateSpacialQueryWithResult(cmdSQL);
                    if (!string.IsNullOrEmpty(oid.ToString()))
                    {
                        if (nextTypeToInsert == typeof(SmProtection))
                        {
                            _log.Info("Successfully inserted row in stage 2 SM_Generation table with OID: " + oid.ToString());
                            SmGeneration currentSmGen = (SmGeneration)Convert.ChangeType(item, typeof(SmGeneration));
                            List<SmProtection> lstofPro = currentSmGen._ListOfProtections as List<SmProtection>;
                            if (lstofPro != null && lstofPro.Count > 0)
                            {
                                lstofPro.Select(x => x.PARENT_ID = oid.ToString()).ToList();
                                if (InsertDataInStage2SettingsTables<SmProtection>(lstofPro, action))
                                {
                                    _log.Info("Successfully inserted row in stage 2 SM_Protection table for SM_GENERATION OID: " + oid.ToString());
                                }
                                else
                                {
                                    _log.Info("Failed to insert row in stage 2 SM_Protection table for SM_GENERATION OID: " + currentSmGen.ID.ToString());
                                }
                            }
                            else
                            {
                                _log.Info("No Protection Data found for SM_Generation OID: " + currentSmGen.ID.ToString());
                            }
                        }
                        else if (nextTypeToInsert == typeof(SmGenerator))
                        {
                            SmProtection currentSmPro = (SmProtection)Convert.ChangeType(item, typeof(SmProtection));
                            List<SmGenerator> lstofSmGenerators = currentSmPro._ListOfGenerators as List<SmGenerator>;
                            if (lstofSmGenerators != null && lstofSmGenerators.Count > 0)
                            {
                                lstofSmGenerators.Select(x => x.PROTECTION_ID = oid.ToString()).ToList();
                                if (InsertDataInStage2SettingsTables<SmGenerator>(lstofSmGenerators,  action))
                                {
                                    _log.Info("Successfully inserted row in stage 2 SM_Generator table with OID: " + oid.ToString());
                                }
                                else
                                {
                                    _log.Info("Failed to insert row in stage 2 SM_Generator table for SM_Protection OID: " + currentSmPro.ID.ToString());
                                }
                            }
                            else
                            {
                                _log.Info("No Data found in SM_GENERATOR for PROTECTION OID: " + currentSmPro.ID.ToString());
                            }
                        }
                        else if (nextTypeToInsert == typeof(SmGenEquipment))
                        {
                            SmGenerator currentSmGenerator = (SmGenerator)Convert.ChangeType(item, typeof(SmGenerator));
                            List<SmGenEquipment> lstofEquipments = currentSmGenerator._ListOfEqipments as List<SmGenEquipment>;
                            if (lstofEquipments != null && lstofEquipments.Count > 0)
                            {
                                //lstofEquipments.Select(x => x.GENERATOR_ID = oid.ToString()).ToList();
                                if (InsertDataInStage2SettingsTables<SmGenEquipment>(lstofEquipments, action))
                                {
                                    _log.Info("Successfully inserted row in stage 2 Sm_Protection table with oID: " + oid.ToString());
                                }
                                else
                                {
                                    _log.Info("Failed to insert row in stage 2 SM_Generator table for SM_Protection OID: " + currentSmGenerator.ID.ToString());
                                }
                            }
                            else
                            {
                                _log.Info("No Data found in SM_GEN_EQUIPMENT table for Generator SAP Equipment ID: " + currentSmGenerator.SAP_EQUIPMENT_ID.ToString());
                            }
                        }
                        else if (nextTypeToInsert == typeof(SmGeneration))
                        {
                            GenerationInfo currentGenerationInfo = (GenerationInfo)Convert.ChangeType(item, typeof(GenerationInfo));
                            List<SmGeneration> lstofSmGen = currentGenerationInfo._ListOfSmGen as List<SmGeneration>;
                            if (lstofSmGen != null && lstofSmGen.Count > 0)
                            {
                                if (InsertDataInStage2SettingsTables<SmGeneration>(lstofSmGen, action))
                                {
                                    _log.Info("Successfully inserted row in stage 2 Sm_Generation table with oID: " + oid.ToString());
                                }
                                else
                                {
                                    _log.Info("Failed to insert row in stage 2 SM_Generation table for Generation Info OID: " + currentGenerationInfo.OBJECTID.ToString());
                                }
                            }
                            else
                            {
                                _log.Info("No Data found in SM_GENERATION table for Generation GLOBALID : " + currentGenerationInfo.GLOBALID.ToString());
                            }
                        }
                        else
                        {

                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _log.Info("Error in function InsertDataInStage2SettingsTables: " + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Method which get the corresponding set of feilds for a table depending upon the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldsToInsert"></param>
        /// <param name="tabName"></param>
        /// <param name="nextType"></param>
        private void GetCorrespondingValues<T>(ref string fieldsToInsert, ref string tabName,ref Type nextType)
        {
            Type whichType = typeof(T);
            if (whichType == typeof(SmGeneration))
            {
                fieldsToInsert = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMGEN;
                tabName = ReadConfigurations.STAGE2_SM_GEN_TABLE_NAME;
                nextType = typeof(SmProtection);
            }else if(whichType == typeof(SmProtection))
            {
                fieldsToInsert = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMPRO;
                tabName = ReadConfigurations.STAGE2_SM_PROTECTION_TABLE_NAME;
                nextType = typeof(SmGenerator);
            }
            else if (whichType == typeof(SmGenerator))
            {
                fieldsToInsert = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMGENERATOR;
                tabName = ReadConfigurations.STAGE2_SM_GENRATOR_TABLE_NAME;
                nextType = typeof(SmGenEquipment);
            }
            else if (whichType == typeof(SmGenEquipment))
            {
                fieldsToInsert = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMGENEQUIP;
                tabName = ReadConfigurations.STAGE2_SM_GEN_EQUIP_TABLE_NAME;
                nextType = null;
            }
            else if (whichType == typeof(GenerationInfo))
            {
                fieldsToInsert = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_GENINFO;
                tabName = ReadConfigurations.STAGE2_GEN_INFO_TABLE_NAME;
                nextType = typeof(SmGeneration);
            }
            else
            {
                fieldsToInsert = string.Empty;
                tabName = string.Empty;
                nextType = null;
            } 
        }

        /// <summary>
        /// Inserts data in PGEDATA.PGEDATA_SM_GENERATION_STAGE table, the data we are inserting is from actal table 'EDSETT.SM_GENERATION'
        /// </summary>
        /// <param name="list"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool InsertDataInStage2SMGen(List<SmGeneration> list, OracleConnection connection, string action)
        {
            try
            {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    int successful = 0;
                    foreach (SmGeneration smGen in list)
                    {
                        string fieldsToInsertInSmGen = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMGEN;
                        string[] arrayOfFields = fieldsToInsertInSmGen.Split(',');
                        string fieldvalueParam = string.Join(",", arrayOfFields.Select(x => ":" + x).ToArray());
                        string stageTabQueryString = "INSERT INTO {0}(" + fieldsToInsertInSmGen + ")" +
                            " VALUES(" + fieldvalueParam + ") returning ID into :myOId";
                        string tabName = ReadConfigurations.STAGE2_SM_GEN_TABLE_NAME;
                        string cmdText = string.Format(stageTabQueryString, tabName);

                        OracleCommand cmdSQL = new OracleCommand();
                        cmdSQL.Connection = connection;
                        cmdSQL.CommandText = cmdText;
                        cmdSQL.CommandType = CommandType.Text;
                        cmdSQL.Parameters.AddRange(GetParameterCollection(smGen, action, arrayOfFields).ToArray());

                        cmdSQL.Parameters.Add(new OracleParameter("myOId", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                        cmdSQL.ExecuteNonQuery(); // an INSERT is always a Non Query
                        var oid = cmdSQL.Parameters["myOId"].Value;
                        if(!string.IsNullOrEmpty(oid.ToString()))
                        {
                            successful++;
                            _log.Info("Successfully inserted row in stage 2 Sm_Generation table with oID: " + oid.ToString());
                            if (smGen._ListOfProtections!= null && smGen._ListOfProtections.Count > 0)
                            {
                                smGen._ListOfProtections.Select(x => x.PARENT_ID = oid.ToString());
                                foreach (var c in smGen._ListOfProtections)
                                {
                                    c.PARENT_ID = oid.ToString();
                                }
                                InsertDataInStage2SMProtection(smGen._ListOfProtections, connection, action);
                            }
                            else
                            {
                                _log.Info("No Protection Schema's found in SETTINGS database to insert in " + tabName);
                            }
                        }
                    }
                    return list.Count == successful ? true : false;
                
            }
            catch (Exception ex)
            {
                _log.Error("Error in inserting row in Stage2 GenerationInfo table, function InsertDataInStage2SMGen :" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Inserts data in PGEDATA.PGEDATA_SM_PROTECTION_STAGE table, the data we are inserting is from actal table 'EDSETT.SM_PROTECTION'
        /// </summary>
        /// <param name="list"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool InsertDataInStage2SMProtection(List<SmProtection> list, OracleConnection connection, string action)
        {
            try
            {

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    int successful = 0;
                    foreach (SmProtection smGen in list)
                    {
                        string fieldsToInsertInSmPro = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMPRO;
                        string[] arrayOfFields = fieldsToInsertInSmPro.Split(',');
                        string fieldvalueParam = string.Join(",", arrayOfFields.Select(x => ":" + x).ToArray());
                        string stageTabQueryString = "INSERT INTO {0}(" + fieldsToInsertInSmPro + ")" +
                            " VALUES(" + fieldvalueParam + ") returning ID into :myOId";
                        string tabName = ReadConfigurations.STAGE2_SM_PROTECTION_TABLE_NAME;
                        string cmdText = string.Format(stageTabQueryString, tabName);

                        OracleCommand cmdSQL = new OracleCommand();
                        cmdSQL.Connection = connection;
                        cmdSQL.CommandText = cmdText;
                        cmdSQL.CommandType = CommandType.Text;
                        cmdSQL.Parameters.AddRange(GetParameterCollection(smGen, action, arrayOfFields).ToArray());

                        cmdSQL.Parameters.Add(new OracleParameter("myOId", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                        cmdSQL.ExecuteNonQuery(); // an INSERT is always a Non Query
                        var oid = cmdSQL.Parameters["myOId"].Value;
                        if (!string.IsNullOrEmpty(oid.ToString()))
                        {
                            successful++;
                            _log.Info("Successfully inserted row in stage 2 Sm_Protection table with oID: " + oid.ToString());
                            if (smGen._ListOfGenerators != null && smGen._ListOfGenerators.Count > 0)
                            {
                                smGen._ListOfGenerators.Select(x => { x.PROTECTION_ID = oid.ToString(); return x; }).ToList();
                                foreach (var c in smGen._ListOfGenerators)
                                {
                                    c.PROTECTION_ID = oid.ToString();
                                }
                                InsertDataInStage2SMGenerator(smGen._ListOfGenerators, connection, action);
                            }
                            else
                            {
                                _log.Info("No Generators found in SETTINGS database to insert in " + tabName);
                            }
                        }
                           
                    }
                    return list.Count == successful ? true : false;
                
            }
            catch (Exception ex)
            {
                _log.Error("Error in inserting row in Stage2 SM_Protection table, function InsertDataInStage2SMProtection :" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Inserts data in PGEDATA.PGEDATA_SM_GENERATOR_STAGE table, the data we are inserting is from actal table 'EDSETT.SM_GENERATOR'
        /// </summary>
        /// <param name="list"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool InsertDataInStage2SMGenerator(List<SmGenerator> list, OracleConnection connection, string action)
        {
            try
            {

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    int successful = 0;
                    foreach (SmGenerator smGen in list)
                    {
                        string fieldsToInsertInSmGenerator = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMGENERATOR;
                        string[] arrayOfFields = fieldsToInsertInSmGenerator.Split(',');
                        string fieldvalueParam = string.Join(",", arrayOfFields.Select(x => ":" + x).ToArray());
                        string stageTabQueryString = "INSERT INTO {0}(" + fieldsToInsertInSmGenerator + ")" +
                            " VALUES(" + fieldvalueParam + ") returning ID into :myOId";
                        string tabName = ReadConfigurations.STAGE2_SM_GENRATOR_TABLE_NAME;
                        string cmdText = string.Format(stageTabQueryString, tabName);

                        OracleCommand cmdSQL = new OracleCommand();
                        cmdSQL.Connection = connection;
                        cmdSQL.CommandText = cmdText;
                        cmdSQL.CommandType = CommandType.Text;
                        cmdSQL.Parameters.AddRange(GetParameterCollection(smGen, action, arrayOfFields).ToArray());

                        cmdSQL.Parameters.Add(new OracleParameter("myOId", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                        cmdSQL.ExecuteNonQuery(); // an INSERT is always a Non Query
                        var oid = cmdSQL.Parameters["myOId"].Value;
                        if (!string.IsNullOrEmpty(oid.ToString()))
                        {
                            successful++;
                            _log.Info("Successfully inserted row in stage 2 Sm_Generator table with oID: " + oid.ToString());
                            if (smGen._ListOfEqipments != null && smGen._ListOfEqipments.Count > 0)
                            {
                                //smGen._ListOfEqipments.Select(x => { x.GENERATOR_ID = oid.ToString(); return x; }).ToList();
                                //foreach (var c in smGen._ListOfEqipments)
                                //{
                                //    c.GENERATOR_ID = oid.ToString();
                                //}
                                InsertDataInStage2SMGenEquip(smGen._ListOfEqipments, connection, action);
                            }
                            else
                            {
                                _log.Info("No Equipment found in SETTINGS database to insert in " + tabName);
                            }
                        }

                    }
                    return list.Count == successful ? true : false;
                
            }
            catch (Exception ex)
            {
                _log.Error("Error in inserting row in Stage2 SM_Generator table, function InsertDataInStage2SMGenerator :" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Inserts data in PGEDATA.PGEDATA_SM_GEN_EQUIPMENT_STAGE table, the data we are inserting is from actal table 'EDSETT.SM_GEN_EQUIPMENT'
        /// </summary>
        /// <param name="list"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool InsertDataInStage2SMGenEquip(List<SmGenEquipment> list, OracleConnection connection, string action)
        {
            try
            {

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    int successful = 0;
                    foreach (SmGenEquipment smGen in list)
                    {
                        string fieldsToInsertInSmGenEquip = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_SMGENEQUIP;
                        string[] arrayOfFields = fieldsToInsertInSmGenEquip.Split(',');
                        string fieldvalueParam = string.Join(",", arrayOfFields.Select(x => ":" + x).ToArray());
                        string stageTabQueryString = "INSERT INTO {0}(" + fieldsToInsertInSmGenEquip + ")" +
                            " VALUES(" + fieldvalueParam + ") returning ID into :myOId";
                        string tabName = ReadConfigurations.STAGE2_SM_GEN_EQUIP_TABLE_NAME;
                        string cmdText = string.Format(stageTabQueryString, tabName);

                        OracleCommand cmdSQL = new OracleCommand();
                        cmdSQL.Connection = connection;
                        cmdSQL.CommandText = cmdText;
                        cmdSQL.CommandType = CommandType.Text;
                        cmdSQL.Parameters.AddRange(GetParameterCollection(smGen, action, arrayOfFields).ToArray());

                        cmdSQL.Parameters.Add(new OracleParameter("myOId", OracleDbType.Decimal, ParameterDirection.ReturnValue));
                        cmdSQL.ExecuteNonQuery(); // an INSERT is always a Non Query
                        var oid = cmdSQL.Parameters["myOId"].Value;
                        if (!string.IsNullOrEmpty(oid.ToString()))
                        {
                            successful++;
                            _log.Info("Successfully inserted row in stage 2 Sm_Gee_Equipment table with oID: " + oid.ToString());
                        }

                    }
                    return list.Count == successful ? true : false;
                
            }
            catch (Exception ex)
            {
                _log.Error("Error in inserting row in Stage2 SM_Generator table, function InsertDataInStage2SMGenEquip :" + ex.Message);
                return false;
            }
        }
        /// <summary>
        /// Inserts data in PGEDATA.GENERATIONINFO table, the data we are inserting is from actal table in EDGIS
        /// </summary>
        /// <param name="existingGen"></param>
        /// <param name="connection"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// 
       // private string InsertDataInStage2GenInfo(GenerationInfo existingGen, OracleConnection connection,string action)
        //DBCHNG 13.
        private string InsertDataInStage2GenInfo(GenerationInfo existingGen, string action)
        {
            try
            {
                //if (connection.State == ConnectionState.Closed)
                //    connection.Open();

                string fieldsToInsertInGenInfo = ReadConfigurations.FIELDS_TO_INSERT_STAGE2_GENINFO;
                string[] arrayOfFields = fieldsToInsertInGenInfo.Split(',');
                string fieldvalueParam = string.Join(",", arrayOfFields.Select(x => ":" + x).ToArray());
                //string stageTabQueryString = "INSERT INTO {0}(" + fieldsToInsertInGenInfo + ")" +
                //    " VALUES(" + fieldvalueParam + ") returning OBJECTID into :myObjectId";
                //string stage2GenInfoTabName = ReadConfigurations.STAGE2_GEN_INFO_TABLE_NAME;
                string dt_created = string.Empty;
                string dt_modified = string.Empty;                
                if (existingGen.DATECREATED != DBNull.Value)
                {
                    DateTime dt1 = Convert.ToDateTime(existingGen.DATECREATED);
                    dt_created = dt1.ToString("MM/dd/yyyy hh:mm:ss tt");
                }
                if (existingGen.DATE_MODIFIED != DBNull.Value)
                {
                    DateTime dt2 = Convert.ToDateTime(existingGen.DATE_MODIFIED);
                    dt_modified = dt2.ToString("MM/dd/yyyy hh:mm:ss tt");                   
                }
                //ENOS Tariff Change - Updated Insert Query
                //DERMS Change- Updated Insert Query  04 / 13 / 2021
                // WG1 Change - update insert query start - 10/07/2021
                string Sql_Insert = "INSERT INTO "+ReadConfigurations.STAGE2_GEN_INFO_TABLE_NAME+" (" + fieldsToInsertInGenInfo + ") VALUES (" + existingGen.OBJECTID + ",'" + existingGen.GLOBALID + "','" + existingGen.SERVICEPOINTGUID + "',TO_DATE('" + dt_created+ "','"+ReadConfigurations.Date_Format_for_GenInfo+"'),'" + existingGen.CREATEDBY + "',TO_DATE('" 
                  + dt_modified + "','"+ReadConfigurations.Date_Format_for_GenInfo+"'),'" + existingGen.MODIFIEDBY + "','" + existingGen.SAPEGINOTIFICATION + "','" + existingGen.PROJECTNAME.ToString().Replace("'"," ") + "','" + existingGen.GENTYPE + "','" + existingGen.PROGRAMTYPE + "','" + existingGen.EFFRATINGMACHKW + "','" 
                 + existingGen.EFFRATINGINVKW + "','" + existingGen.EFFRATINGMACHKVA + "','" + existingGen.EFFRATINGINVKVA + "','" + existingGen.BACKUPGEN + "','" + existingGen.MAXSTORAGECAPACITY + "','" + existingGen.CHARGEDEMANDKW + "','" + existingGen.GENSYMBOLOGY + "','" 
                 + existingGen.POWERSOURCE + "','" + action + "','" +  existingGen.DERATED+ "','" + existingGen.TELEMETRYENABLED + "','" + existingGen.COMMUNICATIONTYPE + "','" + existingGen.CONTROLENABLED + "','" + existingGen.CONTROLPROGRAMTYPE + "','" + existingGen.PROJECTTOTALEXPORTKW + "','" + existingGen.LIMITED + "'"  +") returning OBJECTID into :myObjectId";
                
                // WG1 Change query - end
               // string cmdText = string.Format(test_Sql, ReadConfigurations.STAGE2_GEN_INFO_TABLE_NAME);
             //   string cmdText = Sql_Insert;
             //   OracleCommand cmdSQL = new OracleCommand();
             //   cmdSQL.Connection = connection;
             //   cmdSQL.CommandText = cmdText;
             //   cmdSQL.CommandType = CommandType.Text;
             ////   cmdSQL.Parameters.AddRange(GetParameterCollection(existingGen, action, arrayOfFields).ToArray());

             //   cmdSQL.Parameters.Add(new OracleParameter("myObjectId", OracleDbType.Decimal, ParameterDirection.ReturnValue));
             //   cmdSQL.ExecuteNonQuery(); // an INSERT is always a Non Query
             //   var oid = cmdSQL.Parameters["myObjectId"].Value;
                //DBCHNG 14:
                object oid = cls_DBHelper_For_EDER.UpdateQueryWithResult(Sql_Insert);
                return !string.IsNullOrEmpty(Convert.ToString(oid)) ? Convert.ToString(oid) : string.Empty;
                
            }
            catch (Exception ex)
            {
                _log.Error("Error in inserting row in Stage2 GenerationInfo table, function InsertDataInStage2GenInfo :" + ex.Message);
                return string.Empty;
            }
        }
        /// <summary>
        /// Gets all the properties of an object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static PropertyInfo[] GetProperties(object obj)
        {
            return obj.GetType().GetProperties() as PropertyInfo[];
        }
        /// <summary>
        /// Returns list of oracle parameters required for an insert operation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="existingGen"></param>
        /// <param name="action"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        private List<OracleParameter> GetParameterCollection<T>(T existingGen, string action, string[] fields)
        {
            var oraParams = new List<OracleParameter>();
            var myDict = existingGen.ToDictionary();

            foreach (string x in fields)
            {
                if (myDict.ContainsKey(x))
                {
                    if (x == "ACTION")
                    {
                        oraParams.Add(new OracleParameter(x, action));
                    }else
                        oraParams.Add(new OracleParameter(x, myDict[x]));
                }
            }
            return oraParams;
        }
        /// <summary>
        /// Get all related data in SETTIGNS database for a single Generation Info record 
        /// </summary>
        /// <param name="_listOfGenInfo"></param>
        //private void GetSettingsData(List<KeyValuePair<string, GenerationInfo>> _listOfGenInfo)
        //{
        //    try
        //    {
        //        using (OracleConnection settingsConn = Common.GetDBConnection(ReadConfig.SETTINGS_CONNECTION_STRING))
        //        {
        //            foreach (KeyValuePair<string, GenerationInfo> x in _listOfGenInfo)
        //            {
        //                GenerationInfo genItem = x.Value;
        //                List<SmGeneration> _listOfSmGen = new List<SmGeneration>();
        //                if (genItem.GLOBALID != null)
        //                {
        //                    _listOfSmGen = QueryAndReturnSmGenData(settingsConn, genItem);
        //                }
        //                genItem._ListOfSmGen = _listOfSmGen;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        private void GetSettingsData(List<KeyValuePair<string, GenerationInfo>> _listOfGenInfo)
        {
            try
            {
                //DBCHNG :14:
                //using (OracleConnection settingsConn = Common.GetDBConnection(ReadConfigurations.SETTINGS_CONNECTION_STRING))
                //{
                    foreach (KeyValuePair<string, GenerationInfo> x in _listOfGenInfo)
                    {
                        GenerationInfo genItem = x.Value;
                        List<SmGeneration> _listOfSmGen = new List<SmGeneration>();
                        if (genItem.GLOBALID != null)
                        {

                            //_listOfSmGen = QueryAndReturnData<SmGeneration>(settingsConn, genItem);
                            _listOfSmGen = QueryAndReturnData<SmGeneration>(genItem);
                        }
                        genItem._ListOfSmGen = _listOfSmGen;
                    }
                //}
            }
            catch (Exception ex)
            {
                _log.Error("Error in Fectching Data from Settings, function GetSettingsData: " + ex.Message);
               
            }
        }


        private List<T> QueryAndReturnData<T>( object existingData)
        {
            List<T> lstOfTData = new List<T>();
            try
            {
                string queryString = string.Empty;
                string smTabName = string.Empty;
                string cmdText = string.Empty;
                GetCorrespondingValues<T>(existingData, ref queryString, ref smTabName, ref cmdText);

                if (!string.IsNullOrEmpty(cmdText))
                {
                    //OracleCommand cmdSQL = new OracleCommand();
                    //cmdSQL.Connection = settingsConn;
                    //cmdSQL.CommandText = cmdText;
                    //OracleDataReader dataReder = cmdSQL.ExecuteReader();
                    OracleDataReader dataReader = cls_DBHelper_For_Settings.GetDataReaderByQuery(cmdText);

                    if (dataReader.HasRows)
                    {
                        while (dataReader.Read())
                        {
                            Dictionary<string, object> rowToDict = Enumerable.Range(0, dataReader.FieldCount).ToDictionary(i => dataReader.GetName(i), i => dataReader.GetValue(i));
                            object newData;
                            if (typeof(T) == typeof(SmProtection))
                            {
                                newData = new SmProtection(rowToDict);
                                if (((SmProtection)newData).ID != null)
                                {
                                    List<SmGenerator> lstOfGenerators = QueryAndReturnData<SmGenerator>(newData);
                                    if (lstOfGenerators != null && lstOfGenerators.Count > 0)
                                        ((SmProtection)newData)._ListOfGenerators = lstOfGenerators;
                                    else
                                        _log.Info("No Generator Data Found in Stage 2 in PGEDATA schema for SM_PROTECTION record OID" + ((SmProtection)newData).ID.ToString());
                                    lstOfTData.Add((T)newData);
                                }
                            }
                            else if (typeof(T) == typeof(SmGenerator))
                            {
                                newData = new SmGenerator(rowToDict);
                                if (((SmGenerator)newData).SAP_EQUIPMENT_ID != null)
                                {
                                    List<SmGenEquipment> lstOfEquipment = QueryAndReturnData<SmGenEquipment>(newData);
                                    if (lstOfEquipment != null && lstOfEquipment.Count > 0)
                                        ((SmGenerator)newData)._ListOfEqipments = lstOfEquipment;
                                    else
                                        _log.Info("No Equipment Data Found in Stage 2 in PGEDATA schema for SM_GENERATOR SAP Equipment ID" + ((SmGenerator)newData).SAP_EQUIPMENT_ID.ToString());
                                    lstOfTData.Add((T)newData);
                                }
                            }
                            else if (typeof(T) == typeof(SmGenEquipment))
                            {
                                newData = new SmGenEquipment(rowToDict);
                                if (((SmGenEquipment)newData).ID != null)
                                {
                                    lstOfTData.Add((T)newData);
                                }
                            }
                            else if (typeof(T) == typeof(SmGeneration))
                            {
                                newData = new SmGeneration(rowToDict);
                                if (((SmGeneration)newData).ID != null)
                                {
                                    List<SmProtection> lstOfProtection = QueryAndReturnData<SmProtection>(newData);
                                    if (lstOfProtection != null && lstOfProtection.Count > 0)
                                        ((SmGeneration)newData)._ListOfProtections = lstOfProtection;
                                    else
                                        _log.Info("No Protection Data Found in in Stage 2 in PGEDATA schema for SM_GENERATION record OID" + ((SmProtection)newData).ID.ToString());
                                    lstOfTData.Add((T)newData);
                                }
                            }
                        }
                        dataReader.Dispose();
                    }
                    
                }
                return lstOfTData;
            }
            catch (Exception ex)
            {
                _log.Info("Error in function QueryAndReturnData : " + ex.Message);
                return lstOfTData;
            }
        }

        /// <summary>
        /// Method which get the corresponding set of feilds for a table depending upon the type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fieldsToInsert"></param>
        /// <param name="tabName"></param>
        /// <param name="nextType"></param>
        private void GetCorrespondingValues<T>(object existingData, ref string queryToSearch, ref string tabName, ref string cmdText)
        {
            Type whichType = typeof(T);
            if (whichType == typeof(SmGeneration))
            {
                queryToSearch = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGEN_TABLE;
                tabName = ReadConfigurations.MAIN_SM_GEN_TABLE_NAME;
                cmdText = string.Format(queryToSearch, tabName, ((GenerationInfo)Convert.ChangeType(existingData, typeof(GenerationInfo))).GLOBALID);
            }
            else if (whichType == typeof(SmProtection))
            {
                queryToSearch = ReadConfigurations.QUERY_STRING_ON_MAIN_SMPRO_TABLE;
                tabName = ReadConfigurations.MAIN_SM_PROTECTION_TABLE_NAME;
                if (!DBNull.Value.Equals(existingData))
                    cmdText = string.Format(queryToSearch, tabName, ((SmGeneration)Convert.ChangeType(existingData, typeof(SmGeneration))).ID);
            }
            else if (whichType == typeof(SmGenerator))
            {
                queryToSearch = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGENERATOR_TABLE;
                tabName = ReadConfigurations.MAIN_SM_GENRATOR_TABLE_NAME;
                if (!DBNull.Value.Equals(existingData))
                    cmdText = string.Format(queryToSearch, tabName, ((SmProtection)Convert.ChangeType(existingData, typeof(SmProtection))).ID);
            }
            else if (whichType == typeof(SmGenEquipment))
            {
                queryToSearch = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGENEQUP_TABLE;
                tabName = ReadConfigurations.MAIN_SM_GEN_EQUIP_TABLE_NAME;
                if (!DBNull.Value.Equals(existingData))
                    cmdText = string.Format(queryToSearch, tabName, ((SmGenerator)Convert.ChangeType(existingData, typeof(SmGenerator))).ID);
            }
            else
            {
                queryToSearch = string.Empty;
                tabName = string.Empty;
                cmdText = string.Empty;
            }
        }


        /// <summary>
        /// Returns list of Sm_Generations for an existing GenerationInfo record
        /// </summary>
        /// <param name="settingsConn"></param>
        /// <param name="genItem"></param>
        /// <returns></returns>
        private List<SmGeneration> QueryAndReturnSmGenData(OracleConnection settingsConn,GenerationInfo genItem)
        {
            List<SmGeneration> listOfSmGen = new List<SmGeneration>();
            try
            {
                string mainSmGenTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGEN_TABLE;
                string smGenTabName = ReadConfigurations.MAIN_SM_GEN_TABLE_NAME;
                string cmdText = string.Format(mainSmGenTabQueryString, smGenTabName, genItem.GLOBALID);
                OracleCommand cmdSQL = new OracleCommand();
                cmdSQL.Connection = settingsConn;
                cmdSQL.CommandText = cmdText;
                cmdSQL.CommandType = CommandType.Text;

                _log.Info("Trying to check for data in SM_GENERATION table...");

                OracleDataReader dataReader = cmdSQL.ExecuteReader();
                if (dataReader.HasRows)
                {
                    while (dataReader.Read())
                    {
                        Dictionary<string, object> smGenRowToDict = Enumerable.Range(0, dataReader.FieldCount).ToDictionary(i => dataReader.GetName(i),i => dataReader.GetValue(i));

                        SmGeneration existingSmGen = new SmGeneration(smGenRowToDict);
                        if (existingSmGen.ID != null)
                        {
                            List<SmProtection> lstOfProtection = QueryAndReturnSmProtectionData(settingsConn, existingSmGen);
                            if (lstOfProtection != null && lstOfProtection.Count > 0)
                                existingSmGen._ListOfProtections = lstOfProtection;
                            else
                                _log.Info("No Protection Data Found in SETTINGS database for SM_GENERATION record OID" + existingSmGen.ID.ToString());
                        }
                        listOfSmGen.Add(existingSmGen);
                    }
                }
                return listOfSmGen;
            }
            catch (Exception ex)
            {
                _log.Error("Error in fetching SMGEN Data, function QueryAndReturnSmGenData: " + ex.Message);
                return listOfSmGen;
            }
        }
        /// <summary>
        /// Returns list of Sm_Protections for an existing Sm_Generation
        /// </summary>
        /// <param name="settingsConn"></param>
        /// <param name="existingSmGen"></param>
        /// <returns></returns>
        private List<SmProtection> QueryAndReturnSmProtectionData(OracleConnection settingsConn, SmGeneration existingSmGen)
        {
            List<SmProtection> lstOfProtection = new List<SmProtection>();
            try
            {
                string mainSmProTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMPRO_TABLE;
                string smProTabName = ReadConfigurations.MAIN_SM_PROTECTION_TABLE_NAME;
                string cmdText = string.Format(mainSmProTabQueryString, smProTabName, existingSmGen.ID);

                OracleCommand cmdSQL = new OracleCommand();
                cmdSQL.Connection = settingsConn;
                cmdSQL.CommandText = cmdText;
                OracleDataReader protectionReder = cmdSQL.ExecuteReader();
                if (protectionReder.HasRows)
                {
                    if (protectionReder.HasRows)
                    {
                        while (protectionReder.Read())
                        {
                            Dictionary<string, object> smProRowToDict = Enumerable.Range(0, protectionReder.FieldCount).ToDictionary(i => protectionReder.GetName(i),i => protectionReder.GetValue(i));
                            SmProtection smPro = new SmProtection(smProRowToDict);
                            if (smPro.ID != null)
                            {
                                List<SmGenerator> lstOfGenerators = QueryAndReturnSmGenetaorsData(settingsConn, smPro);
                                if (lstOfGenerators != null && lstOfGenerators.Count > 0)
                                    smPro._ListOfGenerators = lstOfGenerators;
                                else
                                    _log.Info("No Generator Data Found in SETTINGS database for SM_PROTECTION record OID" + smPro.ID.ToString());

                                lstOfProtection.Add(smPro);
                            }
                        }
                    }
                }
                return lstOfProtection;
            }
            catch (Exception ex)
            {
                _log.Error("Error in fetching Protection Data, function QueryAndReturnSmProtectionData: " + ex.Message);
                return lstOfProtection;
            }
        }

        /// <summary>
        /// Returns list of Sm_generators for a protection
        /// </summary>
        /// <param name="settingsConn"></param>
        /// <param name="smPro"></param>
        /// <returns></returns>
        private List<SmGenerator> QueryAndReturnSmGenetaorsData(OracleConnection settingsConn, SmProtection smPro)
        {
            List<SmGenerator> lstOfGenerators = new List<SmGenerator>();
            try
            {
                string mainSmGeneratorTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGENERATOR_TABLE;
                string smGeneratorTabName = ReadConfigurations.MAIN_SM_GENRATOR_TABLE_NAME;
                string cmdText = string.Format(mainSmGeneratorTabQueryString, smGeneratorTabName, smPro.ID);
                OracleCommand cmdSQL = new OracleCommand();
                cmdSQL.Connection = settingsConn;
                cmdSQL.CommandText = cmdText;
                OracleDataReader smGeneratorReder = cmdSQL.ExecuteReader();
                if (smGeneratorReder.HasRows)
                {
                    while (smGeneratorReder.Read())
                    {
                        Dictionary<string, object> rowToDict = Enumerable.Range(0, smGeneratorReder.FieldCount).ToDictionary(i => smGeneratorReder.GetName(i),i => smGeneratorReder.GetValue(i));
                        SmGenerator smGenerator = new SmGenerator(rowToDict);
                        if (smGenerator.SAP_EQUIPMENT_ID != null)
                        {
                            List<SmGenEquipment> lstOfEquipments = QueryAndReturnSmGenEquipData(settingsConn, smGenerator);
                            if (lstOfEquipments != null && lstOfEquipments.Count > 0)
                                smGenerator._ListOfEqipments = lstOfEquipments;
                            else
                                _log.Info("No Equipment Data Found in SETTINGS database for SM_GENERATOR SAP Equipment ID" + smGenerator.SAP_EQUIPMENT_ID.ToString());

                            lstOfGenerators.Add(smGenerator);
                        }
                    }
                }
                return lstOfGenerators;
            }
            catch (Exception ex)
            {
                return lstOfGenerators;
            }
        }

        /// <summary>
        /// Returns list of generator equipments for a generators
        /// </summary>
        /// <param name="settingsConn"></param>
        /// <param name="smGenerator"></param>
        /// <returns></returns>
        private List<SmGenEquipment> QueryAndReturnSmGenEquipData(OracleConnection settingsConn, SmGenerator smGenerator)
        {
            List<SmGenEquipment> lstOfGenEquipments = new List<SmGenEquipment>();
            try
            {
                string mainSmGenGeneratorTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGENEQUP_TABLE;
                string smGenEquipTabName = ReadConfigurations.MAIN_SM_GEN_EQUIP_TABLE_NAME;
                string cmdText = string.Format(mainSmGenGeneratorTabQueryString, smGenEquipTabName, smGenerator.ID);
                OracleCommand cmdSQL = new OracleCommand();
                cmdSQL.Connection = settingsConn;
                cmdSQL.CommandText = cmdText;
                OracleDataReader genEquipReder = cmdSQL.ExecuteReader();
                if (genEquipReder.HasRows)
                {
                    if (genEquipReder.HasRows)
                    {
                        while (genEquipReder.Read())
                        {
                            Dictionary<string, object> genEquiRowToDict = Enumerable.Range(0, genEquipReder.FieldCount).ToDictionary(i => genEquipReder.GetName(i),i => genEquipReder.GetValue(i));
                            SmGenEquipment genEqip = new SmGenEquipment(genEquiRowToDict);
                            lstOfGenEquipments.Add(genEqip);
                        }
                    }
                }
                return lstOfGenEquipments;
            }
            catch (Exception ex)
            {
                _log.Error("Error in fetching SmGenEquipment Data, function QueryAndReturnSmGenEquipData: " + ex.Message);
                return lstOfGenEquipments;
            }
        }

        private void GetSettingsData_Master(List<KeyValuePair<string, GenerationInfo>> _listOfGenInfo)
        {
            try
            {
                //DBCHNG 15:
                //using (OracleConnection settingsConn = Common.GetDBConnection(ReadConfigurations.SETTINGS_CONNECTION_STRING))
                //{
                    foreach (KeyValuePair<string, GenerationInfo> x in _listOfGenInfo)
                    {
                        GenerationInfo genItem = x.Value;
                        List<SmGeneration> _listOfSmGen = new List<SmGeneration>();
                        if (genItem.GLOBALID != null)
                        {
                            string mainSmGenTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGEN_TABLE;
                            string smGenTabName = ReadConfigurations.MAIN_SM_GEN_TABLE_NAME;
                            string cmdText = string.Format(mainSmGenTabQueryString, smGenTabName, genItem.GLOBALID);
                            //OracleCommand cmdSQL = new OracleCommand();
                            //cmdSQL.Connection = settingsConn;
                            //cmdSQL.CommandText = cmdText;
                            //cmdSQL.CommandType = CommandType.Text;
                            //OracleDataReader dataReader = cmdSQL.ExecuteReader();
                            _log.Info("Executing Oracle CMD to create result.");

                           OracleDataReader dataReader = cls_DBHelper_For_Settings.GetDataReaderByQuery(cmdText);
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    Dictionary<string, object> smGenRowToDict = Enumerable.Range(0, dataReader.FieldCount)
                                                         .ToDictionary(
                                                             i => dataReader.GetName(i),
                                                             i => dataReader.GetValue(i));

                                    SmGeneration existingSmGen = new SmGeneration(smGenRowToDict);
                                    if (existingSmGen.ID != null)
                                    {
                                        List<SmProtection> lstOfProtections = new List<SmProtection>();
                                        string mainSmProTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMPRO_TABLE;
                                        string smProTabName = ReadConfigurations.MAIN_SM_PROTECTION_TABLE_NAME;
                                        cmdText = string.Format(mainSmProTabQueryString, smProTabName, existingSmGen.ID);

                                        //cmdSQL.CommandText = cmdText;
                                        //OracleDataReader protectionReder = cmdSQL.ExecuteReader();
                                        OracleDataReader protectionReader = cls_DBHelper_For_Settings.GetDataReaderByQuery(cmdText);
                                        if (protectionReader.HasRows)
                                        {
                                            if (protectionReader.HasRows)
                                            {
                                                while (protectionReader.Read())
                                                {
                                                    Dictionary<string, object> smProRowToDict = Enumerable.Range(0, protectionReader.FieldCount)
                                                                                                             .ToDictionary(
                                                                                                                 i => protectionReader.GetName(i),
                                                                                                                 i => protectionReader.GetValue(i));
                                                    SmProtection smPro = new SmProtection(smProRowToDict);
                                                    if (smPro.ID != null)
                                                    {
                                                        List<SmGenerator> lstOfSmGenerator = new List<SmGenerator>();
                                                        string mainSmGeneratorTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGENERATOR_TABLE;
                                                        string smGeneratorTabName = ReadConfigurations.MAIN_SM_GENRATOR_TABLE_NAME;
                                                        cmdText = string.Format(mainSmGeneratorTabQueryString, smGeneratorTabName, smPro.ID);

                                                        //cmdSQL.CommandText = cmdText;
                                                        //OracleDataReader generatorReder = cmdSQL.ExecuteReader();
                                                        OracleDataReader generatorReader = cls_DBHelper_For_Settings.GetDataReaderByQuery(cmdText);
                                                        if (generatorReader.HasRows)
                                                        { 
                                                            while (generatorReader.Read())
                                                            {
                                                                Dictionary<string, object> smGeneratorRowToDict = Enumerable.Range(0, generatorReader.FieldCount)
                                                                                                            .ToDictionary(
                                                                                                                i => generatorReader.GetName(i),
                                                                                                                i => generatorReader.GetValue(i));

                                                                SmGenerator smGenerator = new SmGenerator(smGeneratorRowToDict);
                                                                if (smGenerator.SAP_EQUIPMENT_ID != null)
                                                                {
                                                                    List<SmGenEquipment> lstOfGenEquip = new List<SmGenEquipment>();
                                                                    string mainSmGenGeneratorTabQueryString = ReadConfigurations.QUERY_STRING_ON_MAIN_SMGENEQUP_TABLE;
                                                                    string smGenEquipTabName = ReadConfigurations.MAIN_SM_GEN_EQUIP_TABLE_NAME;
                                                                    cmdText = string.Format(mainSmGenGeneratorTabQueryString, smGenEquipTabName, smGenerator.SAP_EQUIPMENT_ID);

                                                                    //cmdSQL.CommandText = cmdText;
                                                                    //OracleDataReader genEquipReder = cmdSQL.ExecuteReader();
                                                                    OracleDataReader genEquipReader  = cls_DBHelper_For_Settings.GetDataReaderByQuery(cmdText);
                                                                    if (genEquipReader.HasRows)
                                                                    {
                                                                        if (genEquipReader.HasRows)
                                                                        {
                                                                            while (genEquipReader.Read())
                                                                            {
                                                                                Dictionary<string, object> genEquiRowToDict = Enumerable.Range(0, genEquipReader.FieldCount)
                                                                                                                                                            .ToDictionary(
                                                                                                                                                                i => genEquipReader.GetName(i),
                                                                                                                                                                i => genEquipReader.GetValue(i));

                                                                                SmGenEquipment genEqip = new SmGenEquipment(genEquiRowToDict);
                                                                                lstOfGenEquip.Add(genEqip);
                                                                            }
                                                                        }
                                                                    }
                                                                    smGenerator._ListOfEqipments = lstOfGenEquip;
                                                                }
                                                                lstOfSmGenerator.Add(smGenerator);
                                                            }
                                                        }
                                                        smPro._ListOfGenerators = lstOfSmGenerator;
                                                     }
                                                    lstOfProtections.Add(smPro);
                                                }
                                            }
                                        }
                                        existingSmGen._ListOfProtections = lstOfProtections;
                                    }
                                    _listOfSmGen.Add(existingSmGen);

                                }
                            }
                        }
                        genItem._ListOfSmGen = _listOfSmGen;
                    }
               // }
            }
            catch (Exception ex)
            {
                _log.Error("Error in function GetSettingsData_Master: "+ex.Message);
            }
        }     

        public static bool TryParseGuid(string guidString, out Guid guid)
        {
            if (guidString == null) throw new ArgumentNullException("guidString");
            try
            {
                guid = new Guid(guidString);
                return true;
            }
            catch (FormatException)
            {
                guid = default(Guid);
                return false;
            }
        }

        private static void InsertDataInStage2(DataTable dt)
        {
            
        }

        private IWorkspace GetWorkSpace(string server,string userName,string pass)
        {
            try
            {

                string sdeConn = server.Contains("sde:oracle11g:/;")? server : "sde:oracle11g:/;local="+server;
                IPropertySet propertySet = new PropertySetClass();
                _log.Info("Using dbconn: " + sdeConn);
                propertySet.SetProperty("instance", sdeConn);
                propertySet.SetProperty("User", userName);
                propertySet.SetProperty("Password", pass);
                propertySet.SetProperty("version", "SDE.DEFAULT");

                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
                IWorkspace wspace = workspaceFactory.Open(propertySet, 0);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                return wspace;
            }
            catch (Exception ex)
            {
                _log.Error("Error getting SDE workspace. function GetWorkSpace", ex);
                return null;
            }

        }

    }

    
}
