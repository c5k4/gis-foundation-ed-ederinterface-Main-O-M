using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PGE.BatchApplication.ROBCService.Common;
using PGE.BatchApplication.ROBC_API;
using PGE.BatchApplication.ROBC_API.DatabaseRecords;
using System.ComponentModel;
namespace PGE.BatchApplication.ROBCService
{
    public class PCPManagementService
    {
        #region private members
        private static ROBCManager robcManagerObj = Util.RobcManager;
        private const string SwitchRecordTypeName = "SWITCHRECORD";
        private const string DPDRecordTypeName = "DPDRECORD";
        private const string CircuitSourceRecordTypeName = "CIRCUITSOURCERECORD";
        private const string PcpRecordTypeName = "PCPRECORD";
        private const string RobcRecordTypeName = "ROBCRECORD";
        private static readonly string SwitchDeviceTypeName = "Switch";
        private static readonly Dictionary<string, string> DPDDeviceTypeNames = new Dictionary<string, string>() { { "2", "Interrupter" }, { "3", "Recloser" }, { "8", "Sectionalizer" } };

        
        private static List<PCPModel> FindInvalidPcps(Dictionary<PCPRecord, BaseRecord[]> invalidPcpRecords)
        {
            var pcpModelList = new List<PCPModel>();
            if (invalidPcpRecords != null && invalidPcpRecords.Any())
            {
                foreach (var invalidPcpRecord in invalidPcpRecords)
                {
                    var pcpModelObj = new PCPModel();

                    var pcpRecord = invalidPcpRecord.Key;
                    var otherBaseRecords = invalidPcpRecord.Value;
                    if (pcpRecord != null)
                    {
                        PopulatePcpFields(pcpRecord, ref pcpModelObj);
                    }
                    if (otherBaseRecords != null && otherBaseRecords.Any())
                    {
                        BaseRecord baseRecordObj = null;
                        baseRecordObj = otherBaseRecords.ElementAt(0);
                        PopulateRobcFields(baseRecordObj, ref pcpModelObj);
                        baseRecordObj = otherBaseRecords.ElementAt(1);
                        PopulateCircuitSourceFields(baseRecordObj, ref pcpModelObj);
                        baseRecordObj = otherBaseRecords.ElementAt(2);
                        if (baseRecordObj != null)
                        {
                            PopulateDeviceFields(baseRecordObj, ref pcpModelObj, true);
                        }
                        else
                        {
                            baseRecordObj = otherBaseRecords.ElementAt(3);
                            if (baseRecordObj != null)
                            {
                                PopulateDeviceFields(baseRecordObj, ref pcpModelObj, true);
                            }
                            else
                            {
                                pcpModelObj.DeviceType = "Invalid";
                                pcpModelObj.OperatingNo = "Invalid";
                            }
                        }
                    }
                    pcpModelList.Add(pcpModelObj);
                }
            }
            return pcpModelList;
        }
        private static ICollection<PCPModel> GetSortedCollection(IEnumerable<PCPModel> pcpModelList, Dictionary<string, string> gridParams, out int totalItems)
        {
            totalItems = 0;
            totalItems = pcpModelList.Count();
            ICollection<PCPModel> sortedCollection = new ObservableCollection<PCPModel>();
            if (totalItems == 0)
                return sortedCollection;

            switch (gridParams["SortColumn"])
            {
                case ("OperatingNo"):
                    sortedCollection = new ObservableCollection<PCPModel>
                    (
                      from p in pcpModelList
                      orderby p.OperatingNo
                      select p
                    );
                    break;
                case ("DeviceGuid"):
                    sortedCollection = new ObservableCollection<PCPModel>
                    (
                      from p in pcpModelList
                      orderby p.DeviceGuid
                      select p
                    );
                    break;

                case ("CircuitId"):
                    sortedCollection = new ObservableCollection<PCPModel>
                    (
                      from p in pcpModelList
                      orderby p.CircuitId
                      select p
                    );
                    break;
                case ("ObjectId"):
                    sortedCollection = new ObservableCollection<PCPModel>
                    (
                      from p in pcpModelList
                      orderby p.ObjectId
                      select p
                    );
                    break;

                case ("DeviceType"):
                    sortedCollection = new ObservableCollection<PCPModel>
                    (
                      from p in pcpModelList
                      orderby p.DeviceType
                      select p
                    );
                    break;
            }

            sortedCollection = Convert.ToBoolean(gridParams["Ascending"]) ? sortedCollection : new ObservableCollection<PCPModel>(sortedCollection.Reverse());
            var filteredDevices = new ObservableCollection<PCPModel>();
            for (int i = Convert.ToInt32(gridParams["Start"]); i < Convert.ToInt32(gridParams["Start"]) + Convert.ToInt32(gridParams["ItemsPerPage"]) && i < totalItems; i++)
            {
                filteredDevices.Add(sortedCollection.ElementAt(i));
            }
            return filteredDevices;

        }
        private static IEnumerable<PCPModel> FindPcpDevices(string circuitID, string operatingNo)
        {
            List<BaseRecord> baseRecords = robcManagerObj.FindDevices(circuitID, operatingNo);
            var pcpModelList = new List<PCPModel>();
            bool isFeederNameRequired = false;
            if (baseRecords.Count > 1)
                isFeederNameRequired = true;
            if (baseRecords.Any())
            {
                foreach (var record in baseRecords)
                {
                    PCPModel pcpModelObj = new PCPModel();
                    //PopulatePcpModelFromRecord(record, ref pcpModelObj);
                    PopulateDeviceFields(record, ref pcpModelObj, isFeederNameRequired);
                    pcpModelList.Add(pcpModelObj);
                }
            }
            return pcpModelList;
        }
        private static void PopulateRobcAndSubBlock(ref PCPModel pcpModelObj)
        {
            if (pcpModelObj.PcpGlobalId == null) return;
            ROBCRecord robcRecordObj = robcManagerObj.FindPCPROBC(pcpModelObj.PcpGlobalId.ToString());
            if (robcRecordObj == null) return;
            PopulateRobcFields(robcRecordObj, ref pcpModelObj);
        }
       
        private static void PopulatePcpFields(BaseRecord baseRecordObj, ref PCPModel pcpModelObj)
        {
            if (baseRecordObj != null && pcpModelObj != null)
            {
                if (baseRecordObj.GetType().Name.ToUpper() == PcpRecordTypeName)
                {
                    pcpModelObj.ObjectId = baseRecordObj.GetFieldValue("OBJECTID");
                    pcpModelObj.DeviceGuid = baseRecordObj.GetFieldValue("DEVICEGUID");
                    pcpModelObj.PcpGlobalId = baseRecordObj.GetFieldValue("GLOBALID");
                    pcpModelObj.CreatedUser = baseRecordObj.GetFieldValue("CREATIONUSER");
                    pcpModelObj.CreatedDate = baseRecordObj.GetFieldValue("DATECREATED");
                    pcpModelObj.ModifiedUser = baseRecordObj.GetFieldValue("LASTUSER");
                    pcpModelObj.ModifiedDate = baseRecordObj.GetFieldValue("DATEMODIFIED");
                    pcpModelObj.EstablishedRobc = baseRecordObj.GetFieldValue("ROBC");
                    pcpModelObj.EstablishedSubBlock = baseRecordObj.GetFieldValue("SUBBLOCK");
                    pcpModelObj.SummerProjected = baseRecordObj.GetFieldValue("SUMMERPROJECTED");
                    pcpModelObj.WinterProjected = baseRecordObj.GetFieldValue("WINTERPROJECTED");
                    pcpModelObj.TotalCustomer = baseRecordObj.GetFieldValue("CUSTOMERCOUNT");
                }
            }
        }
        private static void PopulateRobcFields(BaseRecord baseRecordObj, ref PCPModel pcpModelObj)
        {
            if (baseRecordObj != null && pcpModelObj != null)
            {
                if (baseRecordObj.GetType().Name.ToUpper() == RobcRecordTypeName)
                {

                    pcpModelObj.DesiredRobc = baseRecordObj.GetFieldValue("DESIREDROBC");
                    pcpModelObj.EstablishedRobc = baseRecordObj.GetFieldValue("ESTABLISHEDROBC");
                    pcpModelObj.DesiredSubBlock = baseRecordObj.GetFieldValue("DESIREDSUBBLOCK");
                    pcpModelObj.EstablishedSubBlock = baseRecordObj.GetFieldValue("ESTABLISHEDSUBBLOCK");
                    pcpModelObj.LastCheckDate = baseRecordObj.GetFieldValue("LASTCHECKEDDATE");
                    pcpModelObj.DesiredRobcDesc = GetRobcSubBlockDomainValue(Constants.RobcDomainName, pcpModelObj.DesiredRobc);
                    pcpModelObj.EstablishedRobcDesc = GetRobcSubBlockDomainValue(Constants.RobcDomainName, pcpModelObj.EstablishedRobc);
                    pcpModelObj.DesiredSubBlockDesc = GetRobcSubBlockDomainValue(Constants.SubBlockDomainName, pcpModelObj.DesiredSubBlock);
                    pcpModelObj.EstablishedSubBlockDesc = GetRobcSubBlockDomainValue(Constants.SubBlockDomainName, pcpModelObj.EstablishedSubBlock);
                    pcpModelObj.PcpRobcGuid = baseRecordObj.GetFieldValue("GLOBALID");
                }
            }
        }
        private static void PopulateCircuitSourceFields(BaseRecord baseRecordObj, ref PCPModel pcpModelObj)
        {
            if (baseRecordObj != null && pcpModelObj != null)
            {
                if (baseRecordObj.GetType().Name.ToUpper() == CircuitSourceRecordTypeName)
                {

                    pcpModelObj.ParentCircuitSourceGuid = baseRecordObj.GetFieldValue("GLOBALID");
                    pcpModelObj.CircuitId = baseRecordObj.GetFieldValue("CIRCUITID");
                    pcpModelObj.FeederName = string.Format("{0} {1}", baseRecordObj.GetFieldValue("CIRCUITNAME"), baseRecordObj.GetFieldValue("SUBSTATIONNAME"));
                }
            }
        }
        private static void PopulateDeviceFields(BaseRecord baseRecordObj, ref PCPModel pcpModelObj, bool isFeederNameRequired)
        {
            if (baseRecordObj != null && pcpModelObj != null)
            {
                if (baseRecordObj.GetType().Name.ToUpper() == DPDRecordTypeName || baseRecordObj.GetType().Name.ToUpper() == SwitchRecordTypeName)
                {

                    pcpModelObj.OperatingNo = baseRecordObj.GetFieldValue("OPERATINGNUMBER");
                    
                    pcpModelObj.CircuitId = baseRecordObj.GetFieldValue("CIRCUITID");
                    if (isFeederNameRequired)
                    {
                        if (baseRecordObj.GetFieldValue("CIRCUITNAME") == null || string.IsNullOrEmpty(baseRecordObj.GetFieldValue("CIRCUITNAME").ToString().Trim()))
                        {
                            pcpModelObj.FeederName = GetFeederNameFromDevice(pcpModelObj.CircuitId);
                        }
                        else
                        {
                            pcpModelObj.FeederName = string.Format("{0} {1}", baseRecordObj.GetFieldValue("CIRCUITNAME"), baseRecordObj.GetFieldValue("SUBSTATIONNAME"));
                        }
                    }
                    pcpModelObj.DeviceType = GetDeviceType(baseRecordObj.GetType().Name, baseRecordObj.GetFieldValue("SUBTYPECD")!=null?baseRecordObj.GetFieldValue("SUBTYPECD").ToString():string.Empty);
                    pcpModelObj.DeviceGuid = baseRecordObj.GetFieldValue("GLOBALID");
                    pcpModelObj.DeviceObjectId = baseRecordObj.GetFieldValue("OBJECTID");
                }
            }
            
        }
        private static string GetFeederNameFromDevice(object circuitID)
        {
            var feederName = string.Empty;
            string circuitIDStr = circuitID != null ? circuitID.ToString() : string.Empty;
            CircuitSourceRecord circuitSourceRecordObj = robcManagerObj.FindCircuit_NoLoadingNoScada(circuitIDStr);
            if (circuitSourceRecordObj != null)
            {
                feederName = string.Format("{0} {1}", circuitSourceRecordObj.GetFieldValue("CIRCUITNAME"), circuitSourceRecordObj.GetFieldValue("SUBSTATIONNAME"));
            }
            return feederName;
        }
        private static string GetRobcSubBlockDomainValue(string robcSubbBockDomainName, object robcSubBlockCode)
        {
            string robcSubBlockCodeStr = robcSubBlockCode != null ? robcSubBlockCode.ToString() : string.Empty;
            if (string.IsNullOrEmpty(robcSubBlockCodeStr)) return string.Empty;
            return (new ROBCManagementService()).GetDomainCodeValues(robcSubbBockDomainName)[robcSubBlockCodeStr];
        }
        private static string GetDeviceType(string deviceRecordType, string subTypeCD)
        {
            var deviceType = string.Empty;
            deviceRecordType = deviceRecordType.ToUpper();
            if (deviceRecordType == SwitchRecordTypeName)
            {
                deviceType = SwitchDeviceTypeName;
            }
            else if (deviceRecordType == DPDRecordTypeName && !string.IsNullOrEmpty(subTypeCD))
            {
                deviceType = DPDDeviceTypeNames[subTypeCD];
            }
            return deviceType;
        }
        private static void CreatePcpAndRobcRecords(ref PCPModel pcpModelObj, CircuitSourceRecord circuitSourceRecord, ref ROBCRecord robcRecordObj, ref PCPRecord pcpRecordObj,bool hasEssentialCustomer)
        {
            pcpRecordObj.SetFieldValue("SUMMERPROJECTED", pcpModelObj.SummerProjected);
            pcpRecordObj.SetFieldValue("WINTERPROJECTED", pcpModelObj.WinterProjected);
            pcpRecordObj.SetFieldValue("CUSTOMERCOUNT", pcpModelObj.TotalCustomer);
            pcpRecordObj.SetFieldValue("DEVICEGUID", pcpModelObj.DeviceGuid);
            CoreService.WriteLog("Start-->CreatePCP");
            pcpRecordObj = robcManagerObj.CreatePCP(pcpRecordObj);
            CoreService.WriteLog("End-->CreatePCP");
            pcpModelObj.PcpGlobalId = (string)pcpRecordObj.GetFieldValue("GLOBALID");
            robcRecordObj = new ROBCRecord();
            robcRecordObj.SetFieldValue("PARTCURTAILPOINTGUID", pcpRecordObj.GetFieldValue("GLOBALID"));
            robcRecordObj.SetFieldValue("CIRCUITSOURCEGUID", circuitSourceRecord.GlobalID);
            robcRecordObj.SetFieldValue("DESIREDROBC", pcpModelObj.DesiredRobc);
            robcRecordObj.SetFieldValue("DESIREDSUBBLOCK", pcpModelObj.DesiredSubBlock);
            CoreService.WriteLog("Start-->CreateROBC");
            robcManagerObj.CreateROBC(robcRecordObj);
            robcManagerObj.processPCPbyPCPGUID((string)pcpModelObj.PcpGlobalId);
            CoreService.WriteLog("End-->CreateROBC");
            CoreService.WriteLog("Start-->setEstablishedRobcForPcp");
            robcRecordObj = robcManagerObj.setEstablishedRobcForPcp_WithoutValidation(pcpRecordObj,hasEssentialCustomer);
            CoreService.WriteLog("End-->setEstablishedRobcForPcp");
            PopulateRobcFields(robcRecordObj, ref pcpModelObj);
        }
        #endregion
        #region public members
        #region Invalid PCP
        public static ObservableCollection<PCPModel> GetInvalidPcps(bool isRefreshedData, Dictionary<string, string> gridParams, out int totalItems)
        {

            Dictionary<PCPRecord, BaseRecord[]> invalidPcpRecords = new Dictionary<PCPRecord, BaseRecord[]>();
            if (isRefreshedData)
            {
                invalidPcpRecords = robcManagerObj.reCalculateInvalidPCPs();
            }
            else
            {
                invalidPcpRecords = robcManagerObj.getInvalidPCPs();
            }
            List<PCPModel> pcpModelList = FindInvalidPcps(invalidPcpRecords);
            return (ObservableCollection<PCPModel>)GetSortedCollection(pcpModelList, gridParams, out totalItems);

        }

        #endregion

        #region PCP CRUD
        public static PCPModel CreatePcp(PCPModel pcpModelObj, out StringBuilder errorMessages)
        {
            errorMessages = new StringBuilder();
            bool hasEssentialCustomer = false;
            string deviceGUID = pcpModelObj.DeviceGuid != null ? pcpModelObj.DeviceGuid.ToString() : string.Empty;
            try
            {
                var canProcess = true;
                CoreService.WriteLog("Start-->FindCircuit");
                CircuitSourceRecord circuitSourceRecordObj = robcManagerObj.FindCircuit_NoLoadingNoScada(pcpModelObj.CircuitId != null ? pcpModelObj.CircuitId.ToString() : string.Empty);
                CoreService.WriteLog("End-->FindCircuit");
                CoreService.WriteLog("Start-->FindCircuitROBC");
                ROBCRecord robcRecordObj = CoreService.GetRobcRecordFromCircuit(circuitSourceRecordObj);
                CoreService.WriteLog("End-->FindCircuitROBC");
                if (robcRecordObj != null)
                {
                    CoreService.WriteLog("Start-->GetDomainCodeValues");
                    PopulateRobcFields(robcRecordObj, ref pcpModelObj);
                    CoreService.WriteLog("End-->GetDomainCodeValues");
                    string establishedRobc = pcpModelObj.EstablishedRobc != null ? pcpModelObj.EstablishedRobc.ToString().Trim() : string.Empty;
                    string desiredRobc = pcpModelObj.DesiredRobc != null ? pcpModelObj.DesiredRobc.ToString().Trim() : string.Empty;
                    string establishedSubBlock = pcpModelObj.EstablishedSubBlock != null ? pcpModelObj.EstablishedSubBlock.ToString().Trim() : string.Empty;
                    string desiredSubBlock = pcpModelObj.DesiredSubBlock != null ? pcpModelObj.DesiredSubBlock.ToString().Trim() : string.Empty;
                    if (string.IsNullOrEmpty(desiredRobc) && string.IsNullOrEmpty(establishedRobc) && string.IsNullOrEmpty(desiredSubBlock) && string.IsNullOrEmpty(establishedSubBlock))
                    {
                        errorMessages.AppendLine("The established ROBC for the circuit has not been calculated");
                        canProcess = false;
                    }
                    if (!string.IsNullOrEmpty(desiredRobc) && !string.IsNullOrEmpty(desiredSubBlock) && string.IsNullOrEmpty(establishedRobc) && string.IsNullOrEmpty(establishedSubBlock))
                    {
                        errorMessages.AppendLine("The circuit has no ROBC assigned");
                        canProcess = false;
                    }
                    if (!string.IsNullOrEmpty(establishedRobc) && establishedRobc != Constants.EstablishedROBCCode)
                    {
                        errorMessages.AppendLine("The circuit needs to have an established ROBC of ‘E - essential");
                        //pcpConditions.Add("HAS_PARENT_EST_ROBC_E", "FALSE");
                        canProcess = false;
                    }
                    CoreService.WriteLog("Start-->hasEssentialCustomerForDevice");
                    hasEssentialCustomer = robcManagerObj.hasEssentialCustomerForDevice(deviceGUID);
                    if (hasEssentialCustomer)
                    {
                        errorMessages.AppendLine("There are essential customers downstream of the device");
                        canProcess = false;
                        //pcpConditions.Add("HAS_ESSENTIAL_CUSTOMER", "TRUE");
                    }
                    CoreService.WriteLog("End-->hasEssentialCustomerForDevice");
                }
                else
                {
                    errorMessages.AppendLine("The circuit has no ROBC assigned");
                    canProcess = false;
                    //pcpConditions.Add("HAS_ROBC_RECORD", "FALSE");
                }
                if (canProcess)
                {
                    bool isPcpFound = false;
                    CoreService.WriteLog("Start-->FindPCP");
                    PCPRecord pcpRecordObj = robcManagerObj.FindPCP(deviceGUID);
                    CoreService.WriteLog("End-->FindPCP");
                    if (pcpRecordObj != null)
                        isPcpFound = true;
                    else
                    {
                        pcpRecordObj = new PCPRecord();
                        isPcpFound = false;
                    }
                    if (isPcpFound)
                    {
                        errorMessages.AppendLine("PCP record already exists!");
                        //pcpConditions.Add("IS_PCP_FOUND_FOR_NEW", "TRUE");
                    }
                    else
                    {
                        CreatePcpAndRobcRecords(ref pcpModelObj, circuitSourceRecordObj, ref robcRecordObj, ref pcpRecordObj,hasEssentialCustomer);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

            }
            return pcpModelObj;
        }
        public static PCPModel UpdatePcp(PCPModel pcpModelObj, out StringBuilder errorMessages)
        {
            bool isPcpFound = false;
            PCPRecord pcpRecordObj = null;
            errorMessages = new StringBuilder();
            try
            {
                pcpRecordObj = robcManagerObj.FindPCP(pcpModelObj.DeviceGuid != null ? pcpModelObj.DeviceGuid.ToString() : string.Empty);
                if (pcpRecordObj != null)
                    isPcpFound = true;
                else
                {
                    errorMessages.AppendLine("PCP does not exist in the system.");
                }
                if (isPcpFound)
                {
                    pcpRecordObj.SetFieldValue("SUMMERPROJECTED", pcpModelObj.SummerProjected);
                    pcpRecordObj.SetFieldValue("WINTERPROJECTED", pcpModelObj.WinterProjected);
                    robcManagerObj.UpdatePCP(pcpRecordObj);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return pcpModelObj;
        }
        public static PCPModel RemovePcp(PCPModel pcpModelObj)
        {
            PCPRecord pcpRecordObj = null;
            pcpRecordObj = robcManagerObj.FindPCP(pcpModelObj.DeviceGuid != null ? pcpModelObj.DeviceGuid.ToString() : string.Empty);
            if (pcpRecordObj != null)
            {
                string pcpGlobalID = pcpModelObj.PcpGlobalId != null ? pcpModelObj.PcpGlobalId.ToString() : string.Empty;
                robcManagerObj.DeletePCP(pcpGlobalID);
                pcpModelObj.PcpRobcGuid = robcManagerObj.FindPCPROBC(pcpGlobalID).GetFieldValue("GLOBALID");
                robcManagerObj.DeleteROBC(pcpModelObj.PcpRobcGuid != null ? pcpModelObj.PcpRobcGuid.ToString() : string.Empty);
            }
            return pcpModelObj;
        }
        #endregion

        #region PCP devices and other
        public static ObservableCollection<PCPModel> GetPcpDevices(string circuitID, string operatingNo, Dictionary<string, string> gridParams, out int totalItems)
        {
            IEnumerable<PCPModel> pcpModelList = FindPcpDevices(circuitID, operatingNo);
            return (ObservableCollection<PCPModel>)GetSortedCollection(pcpModelList, gridParams, out totalItems);
        }
        public static PCPModel GetPcp(PCPModel pcpModelObj)
        {
            PCPRecord pcpRecordObj = robcManagerObj.FindPCP(pcpModelObj.DeviceGuid != null ? pcpModelObj.DeviceGuid.ToString().Trim() : string.Empty);
            if (pcpRecordObj != null)
            {
                PopulatePcpFields(pcpRecordObj, ref pcpModelObj);
                PopulateRobcAndSubBlock(ref pcpModelObj);
            }
            else
            {
                pcpModelObj.PcpGlobalId = null;
            }
            return pcpModelObj;
        }
        public static int GetPcpCustomerCounts(string deviceGUID)
        {
            CoreService.WriteLog("Start-->getDeviceCustomerCount");
            int counts = robcManagerObj.getDeviceCustomerCount(deviceGUID);
            CoreService.WriteLog("End-->getDeviceCustomerCount");
            return counts;
        }
        #endregion
        #endregion

    }
}



