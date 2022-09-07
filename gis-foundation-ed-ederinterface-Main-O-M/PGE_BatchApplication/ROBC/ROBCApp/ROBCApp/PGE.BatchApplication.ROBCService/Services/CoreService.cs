using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.BatchApplication.ROBC_API;
using PGE.BatchApplication.ROBC_API.DatabaseRecords;
using System.IO;

namespace PGE.BatchApplication.ROBCService
{
    public class CoreService
    {
        private static ROBCManager RobcManagerObj = PGE.BatchApplication.ROBCService.Common.Util.RobcManager;
        private static Dictionary<string, CircuitSourceRecord> CircuitSourceRecordList=null;
        private static Dictionary<string, List<CircuitSourceRecord>> SubCircuitSourceRecordList=null;
        private static readonly string LogFileName = string.Format("c:\\ROBC_Application_{0}.log", DateTime.Now.ToString("MM-dd-yyyy"));
        
        #region constructors

        public CoreService()
        {
            
        }
        #endregion

        internal static ROBCRecord GetRobcRecordFromCircuit(CircuitSourceRecord circuitSourceRecordObj)
        {
            List<ROBCRecord> robcRecords = RobcManagerObj.FindCircuitROBC(circuitSourceRecordObj);
            return GetRobcRecordFromList(robcRecords);
        }
        internal static ROBCRecord GetRobcRecordFromList(List<ROBCRecord> robcRecords)
        {
            ROBCRecord robcRecordObj = null;
            if (robcRecords != null)
            {
                if (robcRecords.Any())
                {
                    foreach (var record in robcRecords)
                    {
                        if (record != null)
                        {
                            var pcpGuid = record.GetFieldValue("PARTCURTAILPOINTGUID") != null ? record.GetFieldValue("PARTCURTAILPOINTGUID").ToString() : string.Empty;
                            if (string.IsNullOrEmpty(pcpGuid))
                            {
                                robcRecordObj = record;
                            }
                        }
                    }
                }
            }
            return robcRecordObj;
        }

        public static void WriteLog(string message)
        {
            return;
        }
        internal static CircuitSourceRecord FindCircuit(string circuitID)
        {
            if (CircuitSourceRecordList == null) 
            {
                CircuitSourceRecordList = new Dictionary<string, CircuitSourceRecord>();
            }
            if(string.IsNullOrEmpty(circuitID)) 
            {
                return new CircuitSourceRecord();
            }
            CircuitSourceRecord searchedRecord = CircuitSourceRecordList.FirstOrDefault(s=>s.Key==circuitID.Trim().ToUpper()).Value;
            if (searchedRecord == null)
            {
                searchedRecord = RobcManagerObj.FindCircuit_NoLoadingNoScada(circuitID);
                CircuitSourceRecordList.Add(circuitID.Trim().ToUpper(), searchedRecord);
            }
            return searchedRecord;
        }
        internal static List<CircuitSourceRecord> FindCircuit(string subStationName,string circuitName)
        {
            string feederName = string.Empty;
            if (SubCircuitSourceRecordList == null) SubCircuitSourceRecordList = new Dictionary<string,List<CircuitSourceRecord>>();
            if(string.IsNullOrEmpty(circuitName) || string.IsNullOrEmpty(subStationName))
            {
                return new List<CircuitSourceRecord>();
            }
            feederName = string.Format("{0};{1}",circuitName.ToUpper(),subStationName.ToUpper());
            List<CircuitSourceRecord> searchedRecordList = SubCircuitSourceRecordList.FirstOrDefault(s => s.Key == feederName).Value;
            if (searchedRecordList == null || searchedRecordList.Count<=0)
            {
                searchedRecordList = RobcManagerObj.FindCircuit(subStationName,circuitName);
                SubCircuitSourceRecordList.Add(feederName, searchedRecordList);
            }
            return searchedRecordList;
        }
    }
}
