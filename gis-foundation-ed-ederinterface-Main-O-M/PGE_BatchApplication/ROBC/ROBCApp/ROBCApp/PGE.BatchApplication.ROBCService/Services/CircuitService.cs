using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.BatchApplication.ROBC_API;
using PGE.BatchApplication.ROBC_API.DatabaseRecords;
using PGE.BatchApplication.ROBCService.Common;
namespace PGE.BatchApplication.ROBCService
{
    public class CircuitService
    {

        private ROBCManager _RobcManager;
        #region constructors

        public CircuitService()
        {
            _RobcManager = Util.RobcManager;
        }
        #endregion

        #region private methods,


        #endregion

        #region public methods

        public Dictionary<string, string> FindCircuit(string circuitID, bool IsDetail)
        {
            try
            {
                CircuitSourceRecord circuitSourceRecordObj = CoreService.FindCircuit(circuitID);
                if (circuitSourceRecordObj.GlobalID == null || string.IsNullOrEmpty(circuitSourceRecordObj.GlobalID.ToString()))
                {
                    return null;
                }
                ROBCRecord robcRecord = CoreService.GetRobcRecordFromCircuit(circuitSourceRecordObj);
                return ROBCManagementService.MapCircuitSourceAndROBCRecords(circuitSourceRecordObj, robcRecord);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public List<Dictionary<string, string>> FindCircuits(string subStationName, string circuitName)
        {
            List<Dictionary<string, string>> circuitSourceROBCFieldValuesList = new List<Dictionary<string, string>>();
            try
            {

                List<CircuitSourceRecord> CircuitSourceRecords = CoreService.FindCircuit(subStationName, circuitName);


                foreach (CircuitSourceRecord circuitSource in CircuitSourceRecords)
                {
                    ROBCRecord robcRecord = CoreService.GetRobcRecordFromCircuit(circuitSource);
                    if (!string.IsNullOrEmpty(circuitSource.GlobalID.ToString()))
                        circuitSourceROBCFieldValuesList.Add(ROBCManagementService.MapCircuitSourceAndROBCRecords(circuitSource, robcRecord));
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return circuitSourceROBCFieldValuesList;

        }

        public List<Dictionary<string, string>> FindFeedingFeeders(string circuitID)
        {
            try
            {
                List<Dictionary<string, string>> feedingCircuitSourceROBCs = new List<Dictionary<string, string>>();
                CircuitSourceRecord CircuitSourceRecordObj = CoreService.FindCircuit(circuitID);
                List<CircuitSourceRecord> feedingFeeders = _RobcManager.GetFeedingFeeders(CircuitSourceRecordObj);

                foreach (CircuitSourceRecord feedingFeeder in feedingFeeders)
                {
                    if (feedingFeeder.GlobalID.ToString() != CircuitSourceRecordObj.GlobalID.ToString())
                    {
                        ROBCRecord robcRecord = CoreService.GetRobcRecordFromCircuit(feedingFeeder);

                        feedingCircuitSourceROBCs.Add(ROBCManagementService.MapCircuitSourceAndROBCRecords(feedingFeeder, robcRecord));
                    }
                }

                return feedingCircuitSourceROBCs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Dictionary<string, string> FindFeedingFeeder(string circuitID)
        {
            try
            {
                Dictionary<string, string> feedingCircuitSourceROBC = new Dictionary<string, string>();
                CircuitSourceRecord CircuitSourceRecordObj = CoreService.FindCircuit(circuitID);
                CircuitSourceRecord feedingFeeder = _RobcManager.GetFeedingFeeder(CircuitSourceRecordObj);
                if (feedingFeeder != null)
                {
                    if (feedingFeeder.GlobalID.ToString() != CircuitSourceRecordObj.GlobalID.ToString())
                    {
                        ROBCRecord robcRecord = CoreService.GetRobcRecordFromCircuit(feedingFeeder);
                        feedingCircuitSourceROBC = ROBCManagementService.MapCircuitSourceAndROBCRecords(feedingFeeder, robcRecord);
                    }
                }
                return feedingCircuitSourceROBC;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<Dictionary<string, string>> FindChildFeeders(string circuitID)
        {
            try
            {
                List<Dictionary<string, string>> childCircuitSourceROBCs = new List<Dictionary<string, string>>();
                CircuitSourceRecord CircuitSourceRecordObj = CoreService.FindCircuit(circuitID);
                List<CircuitSourceRecord> childFeeders = _RobcManager.GetChildFeeders(CircuitSourceRecordObj);
                foreach (CircuitSourceRecord childFeeder in childFeeders)
                {
                    if (childFeeder.GlobalID.ToString() != CircuitSourceRecordObj.GlobalID.ToString())
                    {
                        ROBCRecord robcRecord = CoreService.GetRobcRecordFromCircuit(childFeeder);
                        childCircuitSourceROBCs.Add(ROBCManagementService.MapCircuitSourceAndROBCRecords(childFeeder, robcRecord));
                    }
                }

                return childCircuitSourceROBCs;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool GetEssentialCustomer(string circuitId)
        {
            if (!string.IsNullOrEmpty(circuitId))
            {
                return _RobcManager.hasEssentialCustomerForCircuit(circuitId);
            }
            else
            {
                throw new ArgumentNullException("GetEssentialCustomer - NULL CircuitSourceGUID");
            }
        }


        #endregion
    }
}
