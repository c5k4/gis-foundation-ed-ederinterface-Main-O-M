using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Oracle.DataAccess.Client;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using PGE.BatchApplication.ROBC_API.DatabaseRecords;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;

using Miner.Interop;
using Miner.Framework.TraceUtilities;


namespace PGE.BatchApplication.ROBC_API
{
    public class OracleConnectionControl
    {
        private string EderOracleConnectionString = "";
        private string EDSubstationOracleConnectionString = "";
        //private string EDPubOracleConnectionString = "";

        public OracleConnectionControl(string ederTnsName, string ederUser, string ederPass, string edSubstationTnsName, string edSubstationUser, string edSubstationPass)
        {
            EderOracleConnectionString = CreateOracleConnectionString(ederTnsName, ederUser, ederPass);
            EDSubstationOracleConnectionString = CreateOracleConnectionString(edSubstationTnsName, edSubstationUser, edSubstationPass);
        }

        public OracleConnectionControl(OracleConnection ederOracleConnection, OracleConnection edSubOracleConnection)
        {
            _EderOracleConnection = ederOracleConnection;
            _EdSubstationOracleConnection = edSubOracleConnection;
        }

        ~OracleConnectionControl()
        {
            if (EderOracleConn.State == ConnectionState.Open)
            {
                EderOracleConn.Close();
                EderOracleConn.Dispose();
            }
            EderOracleConn = null;

            if (EDSubstationOracleConn.State == ConnectionState.Open)
            {
                EDSubstationOracleConn.Close();
                EDSubstationOracleConn.Dispose();
            }
            EDSubstationOracleConn = null;
        }

        private string CreateOracleConnectionString(string tnsName, string user, string pass)
        {
            string connection = "User Id=" + user + ";Password=" + pass + ";Data Source=" + tnsName;
            return connection;
        }

        private OracleConnection _EderOracleConnection;
        private OracleConnection EderOracleConn
        {
            get
            {
                if (_EderOracleConnection == null)
                {
                    _EderOracleConnection = new OracleConnection(EderOracleConnectionString);
                }
                return _EderOracleConnection;
            }
            set
            {
                _EderOracleConnection = value;
            }
        }

        private OracleConnection _EdSubstationOracleConnection;
        private OracleConnection EDSubstationOracleConn
        {
            get
            {
                if (_EdSubstationOracleConnection == null)
                {
                    _EdSubstationOracleConnection = new OracleConnection(EDSubstationOracleConnectionString);
                }
                return _EdSubstationOracleConnection;
            }
            set
            {
                _EdSubstationOracleConnection = value;
            }
        }

        private void OpenEderConnection()
        {
            try
            {
                if (EderOracleConn.State != ConnectionState.Open) { EderOracleConn.Open(); }
                using (var cmd = new OracleCommand("alter session set ddl_lock_timeout = 60", EderOracleConn))
                {
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void OpenEDPubConnection()
        {
            try
            {
                if (EDSubstationOracleConn.State != ConnectionState.Open) { EDSubstationOracleConn.Open(); }
                using (var cmd = new OracleCommand("alter session set ddl_lock_timeout = 60", EDSubstationOracleConn))
                {
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Obtains a dictionary of code and description values for the desired domain name
        /// </summary>
        /// <param name="domainName">Name of Domain</param>
        /// <returns>Code/Description domain values</returns>
        public Dictionary<string, string> GetDomainDescriptions(string domainName)
        {
            Dictionary<string, string> DomainList = new Dictionary<string, string>();
            string selectCmd = "";
            try
            {
                //OpenEDPubConnection();
                OpenEderConnection();
                selectCmd = "select CODE,DESCRIPTION from EDGIS.PGE_CODES_AND_DESCRIPTIONS where upper(DOMAIN_NAME) = '" + domainName.ToUpper() + "' ORDER BY CODE";
                using (var cmd = new OracleCommand(selectCmd, EderOracleConn))
                // using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        DomainList.Add(reader.GetString(0), reader.GetString(1));
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Determining domain. Sql: " + selectCmd + ": Error:" + ex.Message);
            }
            finally
            {
                if (EDSubstationOracleConn.State == ConnectionState.Open)
                    EDSubstationOracleConn.Close();
                EDSubstationOracleConn.Dispose();
                EDSubstationOracleConn = null;
            }

            return DomainList;
        }

        /// <summary>
        /// Determines whether the specified circuit source record is scada. It performs an upstream trace through to the substation to find the 
        /// SubInterruptingDevice GUID and then queries the PGEDATA_SM_CIRCUIT_BREAKER_EAD table to find the scada indicator
        /// </summary>
        /// <param name="circuitSourceRecord"></param>
        /// <returns></returns>
        //public ScadaIndicator IsScada(CircuitSourceRecord circuitSourceRecord)
        //{
        //    ScadaIndicator isScada = ScadaIndicator.Unknown;
        //    string selectCmd = "";
        //    try
        //    {
        //        OpenEDPubConnection();
        //        OpenEderConnection();
        //        selectCmd = "select ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL,FEEDERFEDBY from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE TO_FEATURE_GLOBALID = '" + circuitSourceRecord.GetFieldValue("DEVICEGUID") + "'";
        //        int order_num = -1;
        //        int min_branch = -1;
        //        int max_branch = -1;
        //        int treelevel = -1;
        //        string feederFedBy = "";
        //        using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
        //        {
        //            cmd.CommandType = CommandType.Text;
        //            OracleDataReader reader = cmd.ExecuteReader();

        //            while (reader.Read())
        //            {
        //                order_num = Int32.Parse(reader[0].ToString());
        //                min_branch = Int32.Parse(reader[1].ToString());
        //                max_branch = Int32.Parse(reader[2].ToString());
        //                treelevel = Int32.Parse(reader[3].ToString());
        //                if (!reader.IsDBNull(4)) 
        //                {
        //                    feederFedBy = reader.GetString(4);
        //                }
        //            }
        //            cmd.Dispose();
        //        }

        //        if (!string.IsNullOrEmpty(feederFedBy))
        //        {
        //            //Now we can trace upstream to find the SubInterruptingDevice
        //            selectCmd = "select TO_FEATURE_GLOBALID from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE FEEDERID = '" + feederFedBy
        //                + "' AND MIN_BRANCH <= " + min_branch + " AND MAX_BRANCH >= " + max_branch + " AND TREELEVEL <= " + treelevel
        //                + " AND ORDER_NUM >= " + order_num + " AND TO_FEATURE_FCID = 132";

        //            string SubInterruptingDeviceGUID = "";
        //            using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
        //            {
        //                cmd.CommandType = CommandType.Text;
        //                OracleDataReader reader = cmd.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    SubInterruptingDeviceGUID = reader.GetString(0);
        //                }
        //                cmd.Dispose();
        //            }

        //            if (!string.IsNullOrEmpty(SubInterruptingDeviceGUID))
        //            {
        //                //Now query the PGEDATA.PGEDATA_SM_CIRCUIT_BREAKER_EAD table to determine if the circuit is scada
        //                selectCmd = "SELECT SCADA FROM PGEDATA.PGEDATA_SM_CIRCUIT_BREAKER_EAD WHERE GLOBAL_ID = '" + SubInterruptingDeviceGUID + "'";

        //                using (var cmd = new OracleCommand(selectCmd, EderOracleConn))
        //                {
        //                    cmd.CommandType = CommandType.Text;
        //                    OracleDataReader reader = cmd.ExecuteReader();

        //                    while (reader.Read())
        //                    {
        //                        if ((reader.GetString(0)) == "Y")
        //                        {
        //                            isScada = ScadaIndicator.Yes;
        //                        }
        //                        else
        //                        {
        //                            isScada = ScadaIndicator.No;
        //                        }
        //                    }
        //                    cmd.Dispose();
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error Determining domain. Sql: " + selectCmd + ": Error:" + ex.Message);
        //    }
        //    finally
        //    {
        //        if (EderOracleConn.State == ConnectionState.Open)
        //            EderOracleConn.Close();
        //        EderOracleConn.Dispose();
        //        EderOracleConn = null;

        //        if (EDSubstationOracleConn.State == ConnectionState.Open)
        //            EDSubstationOracleConn.Close();
        //        EDSubstationOracleConn.Dispose();
        //        EDSubstationOracleConn = null;
        //    }

        //    return isScada;
        //}

        /// <summary>
        /// Gets the customer counts and loading information for the circuit source record provided.
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit Source record to determine counts for</param>
        /// <returns></returns>
        //public void GetLoadingInformation(ref CircuitSourceRecord circuitSourceRecord, bool getAllCircuits)
        //{
        //    if (!getAllCircuits && CircuitInformationByFeeder.Count < 1)
        //    {
        //        string traceDownstreamSelectStatement = "";
        //        string selectCmd = "";
        //        try
        //        {
        //            OpenEDPubConnection();
        //            selectCmd = "select ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL,FEEDERID from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE TO_FEATURE_GLOBALID = '" + circuitSourceRecord.GetFieldValue("DEVICEGUID") + "'";
        //            int order_num = -1;
        //            int min_branch = -1;
        //            int max_branch = -1;
        //            int treelevel = -1;
        //            string feederID = "";
        //            using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
        //            {
        //                cmd.CommandType = CommandType.Text;
        //                OracleDataReader reader = cmd.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    order_num = Int32.Parse(reader[0].ToString());
        //                    min_branch = Int32.Parse(reader[1].ToString());
        //                    max_branch = Int32.Parse(reader[2].ToString());
        //                    treelevel = Int32.Parse(reader[3].ToString());
        //                    feederID = reader.GetString(4);
        //                }
        //                cmd.Dispose();
        //            }

        //            //Now we can trace downstream to find the ServiceLocationFeatures
        //            traceDownstreamSelectStatement = "select TO_FEATURE_GLOBALID from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE FEEDERID = '" + feederID
        //                + "' AND MIN_BRANCH >= " + min_branch + " AND MAX_BRANCH <= " + max_branch + " AND TREELEVEL >= " + treelevel
        //                + " AND ORDER_NUM <= " + order_num;

        //            selectCmd = "select CUSTOMERTYPE from EDGIS.SERVICEPOINT where SERVICELOCATIONGUID IN (" + traceDownstreamSelectStatement + " AND TO_FEATURE_FCID = 1012)";

        //            Dictionary<string, int> customerTypes = new Dictionary<string, int>();
        //            customerTypes.Add("AGR", 0);
        //            customerTypes.Add("COM", 0);
        //            customerTypes.Add("DOM", 0);
        //            customerTypes.Add("IND", 0);
        //            customerTypes.Add("OTH", 0);

        //            using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
        //            {
        //                cmd.CommandType = CommandType.Text;
        //                OracleDataReader reader = cmd.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    if (reader.IsDBNull(0)) { customerTypes["OTH"] += 1; }
        //                    else
        //                    {
        //                        string customerType = reader.GetString(0).ToUpper();
        //                        try { customerTypes[customerType] += 1; }
        //                        catch { }
        //                    }
        //                }
        //                cmd.Dispose();
        //            }

        //            circuitSourceRecord.AgriculturalCustomers = customerTypes["AGR"];
        //            circuitSourceRecord.CommercialCustomers = customerTypes["COM"];
        //            circuitSourceRecord.DomesticCustomers = customerTypes["DOM"];
        //            circuitSourceRecord.IndustrialCustomers = customerTypes["IND"];
        //            circuitSourceRecord.OtherCustomers = customerTypes["OTH"];

        //            //Now we want to query for the loading information.
        //            selectCmd = "select SUMMERKVA,WINTERKVA from EDGIS.TRANSFORMER where GLOBALID IN (" + traceDownstreamSelectStatement + " AND TO_FEATURE_FCID = 1001)";
        //            double summerKVA = 0.0;
        //            double winterKVA = 0.0;

        //            using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
        //            {
        //                cmd.CommandType = CommandType.Text;
        //                OracleDataReader reader = cmd.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    if (!reader.IsDBNull(0)) { summerKVA += Double.Parse(reader[0].ToString()); }
        //                    if (!reader.IsDBNull(1)) { winterKVA += Double.Parse(reader[1].ToString()); }
        //                }
        //                cmd.Dispose();
        //            }

        //            circuitSourceRecord.SummerKVA = summerKVA;
        //            circuitSourceRecord.WinterKVA = winterKVA;
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Error Determining domain. Sql: " + selectCmd + ": Error:" + ex.Message);
        //        }
        //        finally
        //        {
        //            if (EDSubstationOracleConn.State == ConnectionState.Open)
        //                EDSubstationOracleConn.Close();
        //            EDSubstationOracleConn.Dispose();
        //            EDSubstationOracleConn = null;
        //        }
        //    }
        //    else
        //    {
        //        if (CircuitInformationByFeeder.Count < 1) { GetLoadingInformationAllCircuits(); }

        //        if (circuitSourceRecord.GetFieldValue("CIRCUITID") != null)
        //        {
        //            string feederID = circuitSourceRecord.GetFieldValue("CIRCUITID").ToString();
        //            if (CircuitInformationByFeeder.ContainsKey(feederID))
        //            {
        //                CircuitInformation info = CircuitInformationByFeeder[feederID];
        //                circuitSourceRecord.AgriculturalCustomers = info.Agricultrual;
        //                circuitSourceRecord.CommercialCustomers = info.Commercial;
        //                circuitSourceRecord.DomesticCustomers = info.Domestic;
        //                circuitSourceRecord.IndustrialCustomers = info.Industrial;
        //                circuitSourceRecord.OtherCustomers = info.Other;
        //                circuitSourceRecord.SummerKVA = info.SummerKVA;
        //                circuitSourceRecord.WinterKVA = info.WinterKVA;
        //            }
        //        }
        //    }
        //}















        struct CircuitInformation
        {
            public int Agricultrual;
            public int Commercial;
            public int Domestic;
            public int Industrial;
            public int Other;
            public double SummerKVA;
            public double WinterKVA;
        }

        Dictionary<string, CircuitInformation> CircuitInformationByFeeder = new Dictionary<string, CircuitInformation>();
        /// <summary>
        /// Gets the customer counts and loading information for all circuits.
        /// </summary>
        /// <returns></returns>
        public void GetLoadingInformationAllCircuits()
        {
            string selectCmd = "";
            try
            {
                OpenEDPubConnection();
                selectCmd = "select customertype,feederid,count(*) COUNT from edgis.servicepoint SP join EDGIS.PGE_FEEDERFEDNETWORK_TRACE TRACE "
                            + "on SP.TRANSFORMERGUID = TRACE.TO_FEATURE_GLOBALID group by customertype,feederid order by feederid";

                using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.IsDBNull(1)) { continue; }
                        string feederID = reader[1].ToString();
                        if (!CircuitInformationByFeeder.ContainsKey(feederID)) { CircuitInformationByFeeder.Add(feederID, new CircuitInformation()); }

                        if (reader.IsDBNull(0)) { continue; }

                        string customerType = reader[0].ToString();
                        int count = Int32.Parse(reader[2].ToString());

                        CircuitInformation customerTypeValue = CircuitInformationByFeeder[feederID];
                        if (customerType == "OTH") { customerTypeValue.Other = count; }
                        else if (customerType == "AGR") { customerTypeValue.Agricultrual = count; }
                        else if (customerType == "COM") { customerTypeValue.Commercial = count; }
                        else if (customerType == "DOM") { customerTypeValue.Domestic = count; }
                        else if (customerType == "IND") { customerTypeValue.Industrial = count; }
                        CircuitInformationByFeeder[feederID] = customerTypeValue;
                    }
                    cmd.Dispose();
                }

                //Now we want to query for the loading information.
                selectCmd = "select sum(summerkva) SummerKVA,sum(winterkva) WinterKVA,feederid from EDGIS.TRANSFORMER T join " +
                    "EDGIS.PGE_FEEDERFEDNETWORK_TRACE TRACE on T.GLOBALID = TRACE.TO_FEATURE_GLOBALID group by feederid order by feederid";

                using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader.IsDBNull(2)) { continue; }
                        string feederID = reader[1].ToString();
                        if (!CircuitInformationByFeeder.ContainsKey(feederID)) { CircuitInformationByFeeder.Add(feederID, new CircuitInformation()); }

                        CircuitInformation circuitInfoValue = CircuitInformationByFeeder[feederID];

                        if (!reader.IsDBNull(0))
                        {
                            double summerKVA = double.Parse(reader[0].ToString());
                            circuitInfoValue.SummerKVA = summerKVA;
                        }

                        if (!reader.IsDBNull(1))
                        {
                            double winterKVA = double.Parse(reader[1].ToString());
                            circuitInfoValue.WinterKVA = winterKVA;
                        }

                        CircuitInformationByFeeder[feederID] = circuitInfoValue;
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Determining domain. Sql: " + selectCmd + ": Error:" + ex.Message);
            }
            finally
            {
                if (EDSubstationOracleConn.State == ConnectionState.Open)
                    EDSubstationOracleConn.Close();
                EDSubstationOracleConn.Dispose();
                EDSubstationOracleConn = null;
            }
        }

        /// <summary>
        /// Returns the list of feeders feeding the specified circuit source record.
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit source record to check</param>
        /// <returns></returns>
        public List<string> GetFeedingFeeders(CircuitSourceRecord circuitSourceRecord)
        {
            List<string> parentFeederIDs = new List<string>();
            string selectCmd = "";
            try
            {
                OpenEDPubConnection();
                OpenEderConnection();
                selectCmd = "select FEEDERID,FEEDERFEDBY from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE TO_FEATURE_GLOBALID = '" + circuitSourceRecord.GetFieldValue("DEVICEGUID") + "'";
                string parentFeeder = "";
                string feederFedBy = "";
                using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        try
                        {
                            //throws exception because of null values mainly on feederFedBy
                            parentFeeder = reader.GetString(0);
                            feederFedBy = reader.GetString(1);
                        }
                        catch { }
                    }
                    cmd.Dispose();
                }

                if (!string.IsNullOrEmpty(parentFeeder)) // no exception
                {

                    parentFeederIDs.Add(parentFeeder);
                    while (!string.IsNullOrEmpty(feederFedBy))
                    {
                        parentFeeder = feederFedBy;
                        parentFeederIDs.Add(parentFeeder);

                        selectCmd = "select distinct(FEEDERFEDBY) from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE FEEDERID = '" + parentFeeder + "'";
                        using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
                        {
                            cmd.CommandType = CommandType.Text;
                            OracleDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    feederFedBy = reader.GetString(0);
                                }
                                else { feederFedBy = ""; }
                            }
                            cmd.Dispose();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Determining domain. Sql: " + selectCmd + ": Error:" + ex.Message);
            }
            finally
            {
                if (EderOracleConn.State == ConnectionState.Open)
                    EderOracleConn.Close();
                EderOracleConn.Dispose();
                EderOracleConn = null;

                if (EDSubstationOracleConn.State == ConnectionState.Open)
                    EDSubstationOracleConn.Close();
                EDSubstationOracleConn.Dispose();
                EDSubstationOracleConn = null;
            }

            return parentFeederIDs;
        }

        /// <summary>
        /// Returns the list of feeders that this feeder is feeding.
        /// </summary>
        /// <param name="circuitSourceRecord">Circuit source record to check</param>
        /// <returns></returns>
        public List<string> GetChildFeeders(CircuitSourceRecord circuitSourceRecord)
        {
            List<string> childFeederIDs = new List<string>();
            string selectCmd = "";
            try
            {
                OpenEDPubConnection();
                OpenEderConnection();
                selectCmd = "select FEEDERID from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE TO_FEATURE_GLOBALID = '" + circuitSourceRecord.GetFieldValue("DEVICEGUID") + "'";
                string feederID = "";
                using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        feederID = reader.GetString(0);
                    }
                    cmd.Dispose();
                }

                childFeederIDs.Add(feederID);

                selectCmd = "select distinct(FEEDERID) from EDGIS.PGE_FEEDERFEDNETWORK_TRACE WHERE FEEDERFEDBY = '" + feederID + "'";
                using (var cmd = new OracleCommand(selectCmd, EDSubstationOracleConn))
                {
                    cmd.CommandType = CommandType.Text;
                    OracleDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            feederID = reader.GetString(0);
                            childFeederIDs.Add(feederID);
                        }
                    }
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error Determining domain. Sql: " + selectCmd + ": Error:" + ex.Message);
            }
            finally
            {
                if (EderOracleConn.State == ConnectionState.Open)
                    EderOracleConn.Close();
                EderOracleConn.Dispose();
                EderOracleConn = null;

                if (EDSubstationOracleConn.State == ConnectionState.Open)
                    EDSubstationOracleConn.Close();
                EDSubstationOracleConn.Dispose();
                EDSubstationOracleConn = null;
            }

            return childFeederIDs;
        }

    }
}