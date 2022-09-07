using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Diagnostics;
using Oracle.DataAccess;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using PGE_DBPasswordManagement;

namespace PGE.BatchApplication.GUIDCsvToResyncTable
{
    class Program
    {
        static string _inputFile = "guids.csv";
        static bool _doBulkInsert = true;
        static bool _exportOcName = false;

        static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            try
            {
                string con = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["ConnectionString"]);
                Console.WriteLine("Opening Database..." + con);

                if (args.Length > 0 && args[0].ToUpper() == "-RECALC")
                {
                    _doBulkInsert = false;
                    if (args.Length > 1 && args[1].ToUpper() == "-F")
                    {
                        _inputFile = args[2];
                        if (args.Length > 3)
                        {
                            if (args[3].ToUpper() == "-BULK")
                            {
                                _doBulkInsert = true;
                            }
                        }
                    }
                    else if (args.Length > 1 && args[1].ToUpper() == "-BULK")
                    {
                        _doBulkInsert = true;
                    }
                    if (_doBulkInsert)
                    {
                        ProcessRecalcListBulk(con);
                    }
                    else
                    {
                        ProcessRecalcList(con);
                    }
                }
                else // not recalc
                {
                    if (args.Length > 0)
                    {
                        _inputFile = args[0];
                        if (args.Length > 1)
                        {
                            if (args[1].ToUpper() == "-NOBULK")
                            {
                                _doBulkInsert = false;
                            }
                            else if (args[1].ToUpper() == "-EXPORTOCNAME")
                            {
                                _exportOcName = true;
                            }
                        }
                    }
                    if (_doBulkInsert)
                    {
                        ProcessResyncListBulk(con);
                    }
                    else
                    {
                        ProcessResyncList(con);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex);
            }
            finally
            {
                timer.Stop();
                Console.WriteLine("Total execution time: " + Math.Round(((timer.ElapsedMilliseconds / 1000.0) / 60.0), 2) + " minutes");
                if (Debugger.IsAttached)
                {
                    Console.WriteLine("Press enter to end...");
                    Console.ReadLine();
                }
            }
            Console.WriteLine("Bye...");
        }

        private static void ProcessResyncList(string con)
        {
            IDictionary<string, string> guids = GetGuids();

            OracleConnection oracleConnection = new OracleConnection(con);
            oracleConnection.Open();
            OracleTransaction transaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);

            int i = 1;
            foreach (KeyValuePair<string, string> kvp in guids)
            {
                Console.WriteLine(kvp.Key);
                Console.WriteLine("[ " + i++ + " ] of [ " + guids.Count + " ]");
                if (kvp.Value == "")
                {
                    ProcessResyncGuidsCsv(oracleConnection, transaction, kvp.Key);
                }
                else
                {
                    ProcessResyncGuid(oracleConnection, transaction, kvp.Key, kvp.Value);
                }
            }

            transaction.Commit();
        }


        private static void ProcessResyncListBulk(string con)
        {
            IDictionary<string, string> guids = GetGuids();
            if (guids.Count == 0) return;
            OracleConnection oracleConnection = new OracleConnection(con);
            oracleConnection.Open();
            OracleTransaction transaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);

            int i = 1;
            if (_exportOcName)
            {
                ProcessResyncGuidsCsvExportOcName(oracleConnection, transaction, guids.Keys.Select(s => Convert.ToString(s)).ToArray());
            }
            else if (guids.First().Value == "")
            {
                ProcessResyncGuidsCsvBulk(oracleConnection, transaction, guids.Keys.Select(s => Convert.ToString(s)).ToArray());
            }
            else
            {
                ProcessResyncGuidBulk(oracleConnection, transaction, guids.Keys.Select(s=>Convert.ToString(s)).ToArray(), guids.Values.ToArray());
            }

            transaction.Commit();
        }


        private static void ProcessRecalcList(string con)
        {
            IDictionary<string, string> guids = GetGuids();
            Console.WriteLine("Recalcing (not bulk)..");

            OracleConnection oracleConnection = new OracleConnection(con);
            oracleConnection.Open();
  //          OracleTransaction transaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);


            int i = 1;
            foreach (KeyValuePair<string, string> kvp in guids)
            {
                Console.WriteLine(kvp.Key);
                Console.WriteLine("[ " + i++ + " ] of [ " + guids.Count + " ]");
                try
                {
                    ProcessRecalcGuid(oracleConnection, kvp.Key, kvp.Value, null);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(kvp.Key + " " + ex.ToString());
                    File.AppendAllText("errors.txt", kvp.Key + " " + ex.ToString());
                    File.AppendAllText("failed_recalc.csv", kvp.Key + "," + kvp.Value + Environment.NewLine);
                }
            }

//            transaction.Commit();
        }

        private static void ProcessRecalcListBulk(string con)
        {
            IDictionary<string, string> guids = GetGuids();
            if (guids.Count == 0) return;
            Console.WriteLine("Recalcing..");

            OracleConnection oracleConnection = new OracleConnection(con);
            oracleConnection.Open();
            //          OracleTransaction transaction = oracleConnection.BeginTransaction(IsolationLevel.ReadCommitted);

            int i = 1;
            ProcessRecalcGuidBulk(oracleConnection, guids.Keys.ToArray(), guids.Values.ToArray(), null);
            //            transaction.Commit();
        }


        private static string FormatNumericOnlyGuid(string guidUnformatted)
        {
            if (guidUnformatted.Contains("{")) return guidUnformatted;

            string guid = "{";

            guid += guidUnformatted.Substring(0, 8) + "-";
            guid += guidUnformatted.Substring(8, 4) + "-";
            guid += guidUnformatted.Substring(12, 4) + "-";
            guid += guidUnformatted.Substring(16, 4) + "-";
            guid += guidUnformatted.Substring(20, 12) + "}";

            return guid;
        }

        private static void ProcessResyncGuidsCsv(OracleConnection oracleConnection, OracleTransaction transaction, string guid)
        {
            string storedProc = "edgis.process_resync_guids_csv";
            Console.WriteLine("ExecuteSQL [ " + storedProc + " ] [ " + guid + " ]");
            OracleCommand command = new OracleCommand(storedProc, oracleConnection);
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("guids_list", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["guids_list"].Value = guid;

            command.ExecuteNonQuery();

        }

        private static void ProcessResyncGuidsCsvBulk(OracleConnection oracleConnection, OracleTransaction transaction, string[] guids)
        {
            string storedProc = "edgis.process_resync_guids_csv";
//            Console.WriteLine("ExecuteSQL [ " + storedProc + " ] [ " + guid + " ]");
            OracleCommand command = new OracleCommand(storedProc, oracleConnection);
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;
            command.ArrayBindCount = guids.Length;

            command.Parameters.Add("guids_list", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["guids_list"].Value = guids;

            command.ExecuteNonQuery();

        }

        private static void ProcessResyncGuidsCsvExportOcName(OracleConnection oracleConnection, OracleTransaction transaction, string[] guids)
        {
            string storedProc = "edgis.get_fc_name";
            //            Console.WriteLine("ExecuteSQL [ " + storedProc + " ] [ " + guid + " ]");
            int i = 0;
            foreach (string guid in guids)
            {
                if (++i % 10 == 0)
                {
                    Console.WriteLine("Processed [ " + i + " ] rows");
                }
                string sql = "select " + storedProc + "('" + guid + "') from dual";
                using (OracleCommand command = new OracleCommand(sql, oracleConnection))
                {
                    command.CommandType = CommandType.Text;
                    object retVal = command.ExecuteScalar();
                    string ocName = Convert.ToString(retVal);
                    if (!String.IsNullOrEmpty(ocName))
                    {
                        File.AppendAllText(@"c:\temp\guids_oc.csv", guid + "," + ocName + "\r\n");
                    }
                }
            }
            //OracleCommand command = new OracleCommand("select edgis.get_fc_name('{C70541DC-B269-4D7C-B042-3D9E1180227A}') from dual", oracleConnection);
//            command.CommandText = "select edgis.get_fc_name(:guid) from dual";
        //(:EVENT_NAME, :EVENT_DESC)
        //RETURNING EVENT_ID INTO :EVENT_ID
        //";
//            object retval = command.ExecuteScalar();
            //command.Transaction = transaction;
            //command.CommandType = CommandType.Text;
            //command.ArrayBindCount = guids.Length;

            //command.Parameters.Add("guids", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            //command.Parameters["guids"].Value = guids;

            //OracleParameter prm = new OracleParameter();
            //prm.Direction = ParameterDirection.ReturnValue;
            //prm.DbType = DbType.AnsiString;
            //prm.Size = 38;
            //prm.ParameterName = "kiekis";
            //command.CommandType = CommandType.StoredProcedure;
            //command.Parameters.Add("guid", guid);
            //command.Parameters.Add(prm);
            //command.Parameters["kiekis"].Direction = ParameterDirection.ReturnValue;
            //command.CommandText = "edgis.get_fc_name";

            //command.ExecuteNonQuery();
            //var kiekis = Convert.ToString(command.Parameters["kiekis"].Value);
            //command.CommandType = CommandType.Text;

        }


        private static void ProcessResyncGuid(OracleConnection oracleConnection, OracleTransaction transaction, string guid, string featureclass)
        {
            string storedProc = "edgis.process_resync_guid";
            Console.WriteLine("ExecuteSQL [ " + storedProc + " ] [ " + guid + " ] fc [" + featureclass + " ]");
            OracleCommand command = new OracleCommand(storedProc, oracleConnection);
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("guid", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["guid"].Value = guid;
            command.Parameters.Add("fc", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["fc"].Value = featureclass;

            command.ExecuteNonQuery();

        }

        private static void ProcessResyncGuidBulk(OracleConnection oracleConnection, OracleTransaction transaction, string[] guids, string[] featureclasses)
        {
            string storedProc = "edgis.process_resync_guid";
//            Console.WriteLine("ExecuteSQL [ " + storedProc + " ] [ " + guid + " ] fc [" + featureclass + " ]");
            OracleCommand command = new OracleCommand(storedProc, oracleConnection);
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;
            command.ArrayBindCount = guids.Length;
            command.Parameters.Add("guid", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["guid"].Value = guids;
            command.Parameters.Add("fc", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["fc"].Value = featureclasses;

            command.ExecuteNonQuery();

        }


        private static void ProcessRecalcGuid(OracleConnection oracleConnection, string guid, string featureclass, OracleTransaction transaction)
        {
            const string STORED_PROC_RECALC_LO = "edgis.recalc_localofficeid";
            const string STORED_PROC_RECALC_MAPNUMBER = "edgis.recalc_mapnumber";

            Console.WriteLine("ExecuteSQL [ " + STORED_PROC_RECALC_LO + " ] [ " + guid + " ] fc [" + featureclass + " ]");
            OracleCommand command = new OracleCommand(STORED_PROC_RECALC_LO, oracleConnection);
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("globalidIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["globalidIn"].Value = guid;
            command.Parameters.Add("fcnameIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["fcnameIn"].Value = featureclass;

            command.ExecuteNonQuery();

            Console.WriteLine("ExecuteSQL [ " + STORED_PROC_RECALC_MAPNUMBER + " ] [ " + guid + " ] fc [" + featureclass + " ]");
            command = new OracleCommand(STORED_PROC_RECALC_MAPNUMBER, oracleConnection);
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add("globalidIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["globalidIn"].Value = guid;
            command.Parameters.Add("fcnameIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["fcnameIn"].Value = featureclass;

            command.ExecuteNonQuery();


        }

        private static void ProcessRecalcGuidBulk(OracleConnection oracleConnection, string[] guids, string[] featureclasses, OracleTransaction transaction)
        {
            const string STORED_PROC_RECALC_LO = "edgis.recalc_localofficeid";
            const string STORED_PROC_RECALC_MAPNUMBER = "edgis.recalc_mapnumber";

            //Console.WriteLine("ExecuteSQL [ " + STORED_PROC_RECALC_LO + " ] [ " + guid + " ] fc [" + featureclass + " ]");
            OracleCommand command = new OracleCommand(STORED_PROC_RECALC_LO, oracleConnection);
            if (transaction != null)
            {
                command.Transaction = transaction;
            }
            command.CommandType = CommandType.StoredProcedure;
            command.ArrayBindCount = guids.Length;

            command.Parameters.Add("globalidIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["globalidIn"].Value = guids;
            command.Parameters.Add("fcnameIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["fcnameIn"].Value = featureclasses;

            command.ExecuteNonQuery();

//            Console.WriteLine("ExecuteSQL [ " + STORED_PROC_RECALC_MAPNUMBER + " ] [ " + guid + " ] fc [" + featureclass + " ]");
            command = new OracleCommand(STORED_PROC_RECALC_MAPNUMBER, oracleConnection);
            command.Transaction = transaction;
            command.CommandType = CommandType.StoredProcedure;
            command.ArrayBindCount = guids.Length;
            command.Parameters.Add("globalidIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["globalidIn"].Value = guids;
            command.Parameters.Add("fcnameIn", OracleDbType.Varchar2).Direction = ParameterDirection.Input;
            command.Parameters["fcnameIn"].Value = featureclasses;

            command.ExecuteNonQuery();


        }


        private static IDictionary<string, string> GetGuids()
        {
            IList<string> guidsList = new List<string>();
            IDictionary<string, string> guidsDictionary = new Dictionary<string, string>();

            string guidsAsString = File.ReadAllText(_inputFile);

            if (guidsAsString.Contains(','))
            {

                var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(_inputFile);
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(new string[] { "," });

                while (!parser.EndOfData)
                {
                    string[] row = parser.ReadFields();
                    /* do something */
                    string trimmedGuid = FormatNumericOnlyGuid(row[0].Trim());
                    if (!guidsDictionary.ContainsKey(trimmedGuid))
                    {
                        if (row.Length > 1) { guidsDictionary.Add(trimmedGuid, row[1].Trim()); }
                        else { guidsDictionary.Add(trimmedGuid, ""); }
                    }
                }

            }
            else
            {
                guidsList = File.ReadAllLines(_inputFile).ToList();
                foreach (string guid in guidsList)
                {
                    string trimmedGuid = FormatNumericOnlyGuid(guid.Trim());
                    if (!guidsDictionary.ContainsKey(trimmedGuid))
                    {
                        guidsDictionary.Add(trimmedGuid, "");
                    }
                }
            }

            return guidsDictionary;
        }
    }
}
