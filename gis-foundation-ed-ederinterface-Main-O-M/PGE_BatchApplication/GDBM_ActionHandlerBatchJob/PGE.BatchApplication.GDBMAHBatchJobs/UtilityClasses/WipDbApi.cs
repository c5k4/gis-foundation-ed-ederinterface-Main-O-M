using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using System.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.BatchApplication.GDBMAHBatchJobs
{
    public class WipDbApi
    {
        public const String WipDbTns = "wip";
        private const int StatusDomainCodeForRetired = 2;
        public Common CommonFuntions = new Common();
        private OracleDbConnection _conn;

        private readonly String _serviceConfigName;

        [Obsolete]
        public WipDbApi(String username, String password, String serviceConfigName = "")
        {
            _conn = new OracleDbConnection(username, password, WipDbTns);
            
            _serviceConfigName = serviceConfigName;
        }

        [Obsolete]
        public int GetEquipmentId(IRow row)
        {
            OracleDataReader  rdr;
            IObjectClass oc = (IObjectClass)row.Table;

            IDataset dSet = (IDataset) oc;
            string ocName = dSet.Name;

            const string searchQuery = "count(*)";
            const string inDepthSearchQuery = "fieldname";
            const string resQuery = "equiptypeid";
           //const string resQuery = "equipmenttypeid";

             string queryPredicate = " from " + SchemaInfo.Wip.JETEquipmentTypeSelectionTable;

            string whereClause = " where FEATURECLASS_NAME = '" + ocName.ToUpper() + "' and subtypeCd = " +
                                 row.Value[row.Fields.FindField("SUBTYPECD")];

            //string whereClause = " where objectclassid=" + oc.ObjectClassID + " and subtypecd=" +
            //                     row.Value[row.Fields.FindField("SUBTYPECD")];

            int count = Convert.ToInt32(_conn.ExecuteScalar("select " + searchQuery + queryPredicate + whereClause));

            if (count == 0)
            {
                Logger.Write(TraceEventType.Information, "Could not find Feature class/subtype combination in " +
                            SchemaInfo.Wip.JETEquipmentTypeSelectionTable +
                            " table. Cannot remove used operating number for " + oc.AliasName + " OID: " + row.OID);
                return -1;
            }

            if (count == 1)
            {
                rdr = _conn.ExecuteCommand("select " + resQuery + queryPredicate + whereClause);
                rdr.Read();
                return rdr.GetInt32(0);
            }

            rdr = _conn.ExecuteCommand("select distinct " + inDepthSearchQuery + queryPredicate + whereClause);
            while (rdr.Read())
            {
                int foundLoc = row.Fields.FindField(rdr.GetString(0));

                if (foundLoc == -1) continue;
                using (
                    OracleDataReader rdr2 =
                        _conn.ExecuteCommand("select " + resQuery + queryPredicate + whereClause + " and fieldvalue='" +
                                             row.Value[foundLoc]+"'"))
                {

                    if (rdr2.Read())
                    {
                        return rdr2.GetInt32(0);
                    }
                }
            }

            return -1;
        }


        /// <summary>
        /// QUERY THE JET TABLES TO RETIRE EQUIPMENT (OPERATING NUMBERS/CGC12 NUMBERS)
        /// </summary>
        /// <param name="jobNums">A LIST OF JOB NUMBERS TO PROCESS</param>
        /// <param name="procOpNum">A LIST OF RELATED OPERATING NUMBERS/CGC12 NUMBERS TO PROCESS</param>

        [Obsolete]
        public void CheckRetireInJETJobsTable(IDictionary<String, IList<String>> jobNums, bool procOpNum)
        {
            OracleDataReader rdr = null;

            String opNum = string.Empty;
            string jobNumber = string.Empty;

            IList<string> opNumberList = null;
            string opNumber = null;

            try
            {
                foreach (String jobNum in jobNums.Keys)                                     // PROCESS EACH JOB NUMBER GATHERED FROM THE SESSION
                {
                    jobNumber = jobNum;
                    opNumberList = jobNums[jobNum];

                    for (int i = 0; i < opNumberList.Count; i++)                            // GET EACH OPERATING NUMBER/CGC12 FOR THE JOB NUMBER BEING PROCESSED
                    {
                        if (i == 0)
                        {
                            opNumber = "'" + opNumberList[i] + "'";
                        }
                        else 
                        {
                            opNumber = opNumber + "," + "'" + opNumberList[i] + "'";
                        }
                    }

                    if (opNumber.Trim().Length != 0)
                    {
                        if (procOpNum)                                                      // RETIRE ALL OPERATING NUMBERS FOR THIS JOB NUMBER
                        {
                            rdr = _conn.ExecuteCommand("update " + SchemaInfo.Wip.JETEquipmentTable +
                                                   " set STATUS = 2 where jobnumber = '" + jobNumber + "'" +
                                                   " and OPERATINGNUMBER in (" + opNumber + ")");
                        }
                        else
                        {                                                                   // RETIRE ALL CGC12 NUMBER FOR THIS JOB NUMBER
                            rdr = _conn.ExecuteCommand("update " + SchemaInfo.Wip.JETEquipmentTable +
                                                  " set STATUS = 2 where jobnumber = '" + jobNumber + "'" +
                                                  " and CGC12 in (" + opNumber + ")");
                        }

                        if (procOpNum)
                        {
                           CommonFuntions.WriteLine_Info("WipDbApi.CheckRetireInJETJobsTable: Retired Operating Numbers (" + opNumber + ").");
                        }
                        else
                        {
                            CommonFuntions.WriteLine_Info("WipDbApi.CheckRetireInJETJobsTable: Retired CGC12 Numbers (" + opNumber + ").");
                        }

                        if (procOpNum)
                        {
                            rdr = _conn.ExecuteCommand("select OPERATINGNUMBER from " + SchemaInfo.Wip.JETEquipmentTable +
                                                       " where OPERATINGNUMBER is not null and JOBNUMBER = '" + jobNumber + "'");
                        }
                        else
                        {
                            rdr = _conn.ExecuteCommand("select CGC12 from " + SchemaInfo.Wip.JETEquipmentTable +
                                                       " where CGC12 is not null and JOBNUMBER = '" + jobNumber + "'");
                        }

                        if (!rdr.HasRows) continue;

                        while (rdr.Read())
                        {
                            if (opNum.Trim().Length == 0)                                       // CLEAN UP THE PROCESSING LIST
                            {
                                opNum = rdr.GetString(0);
                            }
                            else
                            {
                                opNum = opNum + "," + rdr.GetString(0);
                            }

                            jobNums[jobNum].Remove(opNum);
                        }

                        if (opNum.Trim().Length != 0)                                            // DETERMINE IF THE JOB NUMBER SHOULD BE RETIRED
                        {
                            rdr = _conn.ExecuteCommand("select jobnumber from " + SchemaInfo.Wip.JETEquipmentTable + " where jobnumber = '" + jobNumber + "'" +
                                      " and status <> 2");

                            if (!rdr.HasRows)
                            {
                                rdr = _conn.ExecuteCommand("update " + SchemaInfo.Wip.JETJobsTable + " set STATUS = 2 where jobnumber = '" + jobNumber + "'");
                            }
                        }
                    }
                }
            }
            //catch (Exception e)
            //{
            //    _log.Info("WipDbApi.CheckRetireInJETJobsTable - Error: " + e.Message + " - " + e.StackTrace);
            //}
            finally
            {
                if (rdr != null && !rdr.IsClosed) { rdr.Close(); }
            }
        }
    }
}
