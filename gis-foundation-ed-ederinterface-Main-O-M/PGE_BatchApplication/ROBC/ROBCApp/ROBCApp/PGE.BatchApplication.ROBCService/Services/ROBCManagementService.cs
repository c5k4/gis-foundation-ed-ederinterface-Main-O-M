using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PGE.BatchApplication.ROBC_API;
using PGE.BatchApplication.ROBC_API.DatabaseRecords;
using PGE.BatchApplication.ROBCService.Common;

namespace PGE.BatchApplication.ROBCService
{
    public class ROBCManagementService
    {
        #region private members
        private string _EDERDatabaseTNSName = Constants.EDERDatabaseTNSName;
        private string _EDERUserName = Constants.EDERUserName;
        private string _EDERPassword = Constants.EDERPassword;
        private string _EDERSUBDatabaseTNSName = Constants.EDERSUBDatabaseTNSName;
        private string _EDERSUBUserName = Constants.EDERSUBUserName;
        private string _EDERSUBPassword = Constants.EDERSUBPassword;

        private ROBCManager _RobcManager;

        private static List<string> _CircuitSourceFields;
        private static List<string> _ROBCFields;
        private static ObservableCollection<CircuitSourceModel> _UnassignedROBCList;
        private ObservableCollection<ROBCModel> _MultipleCircuitROBCList = new ObservableCollection<ROBCModel>();
        private static Dictionary<string, string> _RobcDomainValueList = null;
        private static Dictionary<string, string> _SubBlockDomainValueList = null;
        private static Dictionary<string, string> _DivisionDomainValueList = null;

        public ROBCManagementService()
        {
            _CircuitSourceFields = new List<string>() { "CIRCUITID", "DIVISION", "FEEDERID", "SUBSTATIONNAME", "CIRCUITNAME" };
            _ROBCFields = new List<string>() { "OBJECTID", "GLOBALID", "CREATIONUSER", "DATECREATED", "LASTUSER", "DATEMODIFIED", "DESIREDROBC", "ESTABLISHEDROBC", "DESIREDSUBBLOCK", "ESTABLISHEDSUBBLOCK" };
            _UnassignedROBCList = new ObservableCollection<CircuitSourceModel>();
            _RobcManager = Util.RobcManager;
            _DivisionDomainValueList = GetDomainCodeValues(Constants.Division);
        }
        private ROBCModel PopulateLoadingAndScada(ROBCModel model, CircuitSourceRecord circuitRecord)
        {
            ScadaIndicator isScada = _RobcManager.IsScada(circuitRecord);
            model.IsScada = isScada.ToString();
            model.SummerKVA = circuitRecord.SummerKVA.ToString("F2");
            model.WinterKVA = circuitRecord.WinterKVA.ToString("F2");
            model.TotalCustomer = circuitRecord.TotalCustomers.ToString("F0");
            return model;
        }
        #endregion
        #region Public members
        public ObservableCollection<CircuitSourceModel> GetCircuitsWithUnassignedROBCs(int start, int itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            totalItems = _UnassignedROBCList.Count;
            ObservableCollection<CircuitSourceModel> sortedCircuitSources = new ObservableCollection<CircuitSourceModel>();
            ObservableCollection<CircuitSourceModel> filteredROBCs = new ObservableCollection<CircuitSourceModel>();
            try
            {
                switch (sortColumn)
                {
                    case ("Id"):
                        break;
                    case ("CircuitId"):
                        sortedCircuitSources = new ObservableCollection<CircuitSourceModel>
                        (
                          from p in _UnassignedROBCList
                          orderby p.CircuitId
                          select p
                        );
                        break;

                    case ("FeederName"):
                        sortedCircuitSources = new ObservableCollection<CircuitSourceModel>
                        (
                          from p in _UnassignedROBCList
                          orderby p.FeederName
                          select p
                        );
                        break;
                    case ("Scada"):
                        break;
                    case ("ROBC"):
                        break;
                    case ("SubBlock"):
                        break;
                    case ("EstablishedROBC"):
                        break;
                    case ("EstablishedSubBlock"):
                        break;

                    case ("Division"):
                        sortedCircuitSources = new ObservableCollection<CircuitSourceModel>
                        (
                          from p in _UnassignedROBCList
                          orderby p.Division
                          select p
                        );
                        break;
                }

                sortedCircuitSources = ascending ? sortedCircuitSources : new ObservableCollection<CircuitSourceModel>(sortedCircuitSources.Reverse());

                for (int i = start; i < start + itemCount && i < totalItems; i++)
                {
                    filteredROBCs.Add(sortedCircuitSources[i]);
                }
            }
            catch (Exception ex)
            {
                Util.Logger.Error(string.Format("{0}: {1} == {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace));
            }

            return filteredROBCs;
        }
        public ObservableCollection<ROBCModel> GetMultipleCircuitROBCs(int start, int itemCount, string sortColumn, bool ascending, out int totalItems)
        {
            totalItems = _MultipleCircuitROBCList.Count;
            ObservableCollection<ROBCModel> sortedROBCs = new ObservableCollection<ROBCModel>();
            ObservableCollection<ROBCModel> filteredROBCs = new ObservableCollection<ROBCModel>();

            try
            {
                switch (sortColumn)
                {
                    case ("Id"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.Id
                          select p
                        );
                        break;
                    case ("CircuitId"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.CircuitId
                          select p
                        );
                        break;

                    case ("FeederName"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.FeederName
                          select p
                        );
                        break;
                    case ("Scada"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.IsScada
                          select p
                        );
                        break;
                    case ("ROBC"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.DesiredROBC
                          select p
                        );
                        break;
                    case ("SubBlock"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.DesiredSubBlock
                          select p
                        );
                        break;
                    case ("EstablishedROBC"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.DesiredROBC
                          select p
                        );
                        break;
                    case ("EstablishedSubBlock"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.DesiredSubBlock
                          select p
                        );
                        break;

                    case ("Division"):
                        sortedROBCs = new ObservableCollection<ROBCModel>
                        (
                          from p in _MultipleCircuitROBCList
                          orderby p.Division
                          select p
                        );
                        break;
                }

                sortedROBCs = ascending ? sortedROBCs : new ObservableCollection<ROBCModel>(sortedROBCs.Reverse());

                for (int i = start; i < start + itemCount && i < totalItems; i++)
                {
                    filteredROBCs.Add(sortedROBCs[i]);
                }
            }
            catch (Exception ex)
            {
                Util.Logger.Error(string.Format("{0}: {1} == {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace));
                throw;
            }
            return filteredROBCs;
        }

        public void PopulateCircuitWithoutROBCList()
        {
            try
            {
                ObservableCollection<CircuitSourceModel> circuitSources = new ObservableCollection<CircuitSourceModel>();
                List<CircuitSourceRecord> unassignCirciutSources = _RobcManager.GetCircuitsWithoutROBCWithoutLoadingInformation();
                foreach (CircuitSourceRecord circuitSourceRecord in unassignCirciutSources)
                {
                    circuitSources.Add(GetCircuitSourceModel(circuitSourceRecord));
                }

                _UnassignedROBCList = circuitSources;
            }
            catch (Exception ex)
            {
                Util.Logger.Error(string.Format("{0}: {1} == {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace));
                throw;
            }
        }

        public void PopulateMultipleCircuitROBCs(string subStationName, string circuitName)
        {
            if (_MultipleCircuitROBCList != null && _MultipleCircuitROBCList.Count > 0)
                return;
            List<Dictionary<string, string>> circuitSourceROBCFieldValuesList = null;
            ObservableCollection<ROBCModel> robcModels = new ObservableCollection<PGE.BatchApplication.ROBCService.ROBCModel>();
            try
            {
                if (!string.IsNullOrEmpty(subStationName) && !string.IsNullOrEmpty(circuitName))
                {
                    circuitSourceROBCFieldValuesList = new CircuitService().FindCircuits(subStationName, circuitName);

                    foreach (Dictionary<string, string> fieldValues in circuitSourceROBCFieldValuesList)
                    {

                        robcModels.Add(GetROBCModel(fieldValues));
                    }
                }

                _MultipleCircuitROBCList = robcModels;
            }
            catch (Exception ex)
            {
                Util.Logger.Error(string.Format("{0}: {1} == {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace));
                throw;
            }
        }

        public static Dictionary<string, string> MapCircuitSourceAndROBCRecords(CircuitSourceRecord circuitSourceRecord, ROBCRecord robcRecord)
        {
            Dictionary<string, string> circuitROBCFieldValues = new Dictionary<string, string>();
            try
            {
                if (circuitSourceRecord != null)
                {
                    foreach (string field in _CircuitSourceFields)
                    {
                        try
                        {
                            var fieldValue = circuitSourceRecord.GetFieldValue(field);
                            circuitROBCFieldValues.Add(field, fieldValue.ToString());
                        }
                        catch
                        {
                            circuitROBCFieldValues.Add(field, null);
                        }
                    }
                    circuitROBCFieldValues.Add("SUMMERKVA", circuitSourceRecord.SummerKVA.ToString("F2"));
                    circuitROBCFieldValues.Add("WINTERKVA", circuitSourceRecord.WinterKVA.ToString("F2"));
                    circuitROBCFieldValues.Add("TOTALCUSTOMER", circuitSourceRecord.TotalCustomers.ToString("F2"));

                    circuitROBCFieldValues.Add("CIRCUITSOURCEGUID", circuitSourceRecord.GetFieldValue("GLOBALID").ToString());
                    circuitROBCFieldValues.Add("SCADA", circuitSourceRecord.IsScada.ToString());

                    if (robcRecord != null && !string.IsNullOrEmpty(robcRecord.GlobalID.ToString()))
                    {
                        foreach (string field in _ROBCFields)
                        {
                            try
                            {
                                var fieldValue = robcRecord.GetFieldValue(field);
                                circuitROBCFieldValues.Add(field, fieldValue.ToString());
                            }
                            catch
                            {
                                circuitROBCFieldValues.Add(field, null);
                            }
                        }
                    }
                    else
                    {
                        foreach (string field in _ROBCFields)
                        {
                            circuitROBCFieldValues.Add(field, string.Empty);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Util.Logger.Error(string.Format("{0}: {1} == {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace));
                throw;
            }

            return circuitROBCFieldValues;
        }

        public ROBCModel GetROBCModel(Dictionary<string, string> circuitSourceROBCFieldValues)
        {
            ROBCModel robcModel = new ROBCModel();

            robcModel.Id = Convert.ToInt32(string.IsNullOrEmpty(circuitSourceROBCFieldValues["OBJECTID"]) ? "0" : circuitSourceROBCFieldValues["OBJECTID"]);
            robcModel.GlobalId = string.IsNullOrEmpty(circuitSourceROBCFieldValues["GLOBALID"]) ? "" : circuitSourceROBCFieldValues["GLOBALID"];
            robcModel.CreatedUser = string.IsNullOrEmpty(circuitSourceROBCFieldValues["CREATIONUSER"]) ? "" : circuitSourceROBCFieldValues["CREATIONUSER"];
            robcModel.CreatedDate = string.IsNullOrEmpty(circuitSourceROBCFieldValues["DATECREATED"]) ? "" : circuitSourceROBCFieldValues["DATECREATED"];
            robcModel.ModifiedUser = string.IsNullOrEmpty(circuitSourceROBCFieldValues["LASTUSER"]) ? "" : circuitSourceROBCFieldValues["LASTUSER"];
            robcModel.ModifiedDate = string.IsNullOrEmpty(circuitSourceROBCFieldValues["DATEMODIFIED"]) ? "" : circuitSourceROBCFieldValues["DATEMODIFIED"];
            robcModel.CircuitId = string.IsNullOrEmpty(circuitSourceROBCFieldValues["CIRCUITID"]) ? "" : circuitSourceROBCFieldValues["CIRCUITID"];
            robcModel.FeederId = string.IsNullOrEmpty(circuitSourceROBCFieldValues["FEEDERID"]) ? "" : circuitSourceROBCFieldValues["FEEDERID"];

            string division = string.IsNullOrEmpty(circuitSourceROBCFieldValues["DIVISION"]) ? string.Empty : circuitSourceROBCFieldValues["DIVISION"];
            try
            {
                robcModel.Division = string.IsNullOrEmpty(division) ? string.Empty : _DivisionDomainValueList[division];
            }
            catch { }

            robcModel.SubstationName = string.IsNullOrEmpty(circuitSourceROBCFieldValues["SUBSTATIONNAME"]) ? "" : circuitSourceROBCFieldValues["SUBSTATIONNAME"];
            robcModel.FeederName = string.Format("{0} {1}", robcModel.SubstationName, string.IsNullOrEmpty(circuitSourceROBCFieldValues["CIRCUITNAME"]) ? "" : circuitSourceROBCFieldValues["CIRCUITNAME"]);

            robcModel.CircuitSourceGuid = string.IsNullOrEmpty(circuitSourceROBCFieldValues["CIRCUITSOURCEGUID"]) ? "" : circuitSourceROBCFieldValues["CIRCUITSOURCEGUID"];

            robcModel.DesiredROBC = string.IsNullOrEmpty(circuitSourceROBCFieldValues["DESIREDROBC"]) ? "" : circuitSourceROBCFieldValues["DESIREDROBC"];
            robcModel.DesiredSubBlock = string.IsNullOrEmpty(circuitSourceROBCFieldValues["DESIREDSUBBLOCK"]) ? "" : circuitSourceROBCFieldValues["DESIREDSUBBLOCK"];
            robcModel.EstablishedROBC = string.IsNullOrEmpty(circuitSourceROBCFieldValues["ESTABLISHEDROBC"]) ? "" : circuitSourceROBCFieldValues["ESTABLISHEDROBC"];
            robcModel.EstablishedSubBlock = string.IsNullOrEmpty(circuitSourceROBCFieldValues["ESTABLISHEDSUBBLOCK"]) ? "" : circuitSourceROBCFieldValues["ESTABLISHEDSUBBLOCK"];
            robcModel.DesiredROBCDesc = string.IsNullOrEmpty(robcModel.DesiredROBC) ? string.Empty : GetDomainCodeValues(Constants.RobcDomainName)[robcModel.DesiredROBC];
            robcModel.EstablishedROBCDesc = string.IsNullOrEmpty(robcModel.EstablishedROBC) ? string.Empty : GetDomainCodeValues(Constants.RobcDomainName)[robcModel.EstablishedROBC];
            robcModel.DesiredSubBlockDesc = string.IsNullOrEmpty(robcModel.DesiredSubBlock) ? string.Empty : GetDomainCodeValues(Constants.SubBlockDomainName)[robcModel.DesiredSubBlock];
            robcModel.EstablishedSubBlockDesc = string.IsNullOrEmpty(robcModel.EstablishedSubBlock) ? string.Empty : GetDomainCodeValues(Constants.SubBlockDomainName)[robcModel.EstablishedSubBlock];

            robcModel.IsScada = string.IsNullOrEmpty(circuitSourceROBCFieldValues["SCADA"]) ? "" : circuitSourceROBCFieldValues["SCADA"];
            robcModel.SummerKVA = string.IsNullOrEmpty(circuitSourceROBCFieldValues["SUMMERKVA"]) ? "" : circuitSourceROBCFieldValues["SUMMERKVA"];
            robcModel.WinterKVA = string.IsNullOrEmpty(circuitSourceROBCFieldValues["WINTERKVA"]) ? "" : circuitSourceROBCFieldValues["WINTERKVA"];
            robcModel.TotalCustomer = string.IsNullOrEmpty(circuitSourceROBCFieldValues["TOTALCUSTOMER"]) ? "" : circuitSourceROBCFieldValues["TOTALCUSTOMER"];
            robcModel.CircuitName = string.IsNullOrEmpty(circuitSourceROBCFieldValues["CIRCUITNAME"]) ? "" : circuitSourceROBCFieldValues["CIRCUITNAME"];

            return robcModel;
        }
        public ROBCModel GetLoadingAndScadaInformation(ROBCModel model)
        {
            CircuitSourceRecord circuitRecord = CoreService.FindCircuit(model.CircuitId);
            _RobcManager.GetLoadingInformation(ref circuitRecord, true);
            model = PopulateLoadingAndScada(model, circuitRecord);
            return model;
        }

        public CircuitSourceModel GetCircuitSourceModel(CircuitSourceRecord circuitSourceRecord)
        {
            CircuitSourceModel circuitSource = new CircuitSourceModel();

            try
            {
                circuitSource.CircuitSourceGuid = circuitSourceRecord.GetFieldValue("GLOBALID").ToString();
                circuitSource.CircuitId = circuitSourceRecord.GetFieldValue("CIRCUITID").ToString();
                circuitSource.FeederId = circuitSourceRecord.GetFieldValue("FEEDERID").ToString();
                circuitSource.FeederName = string.Format("{0} {1}", circuitSource.SubstationName, circuitSourceRecord.GetFieldValue("CIRCUITNAME") == null ? "" : circuitSourceRecord.GetFieldValue("CIRCUITNAME").ToString());

                string division = circuitSourceRecord.GetFieldValue("DIVISION").ToString();
                try
                {
                    circuitSource.Division = string.IsNullOrEmpty(division) ? string.Empty : _DivisionDomainValueList[division];
                }
                catch { }
                circuitSource.SubstationName = circuitSourceRecord.GetFieldValue("SUBSTATIONNAME").ToString();
                circuitSource.SummerKVA = circuitSourceRecord.SummerKVA.ToString();
                circuitSource.WinterKVA = circuitSourceRecord.WinterKVA.ToString();
                circuitSource.TotalCustomer = circuitSourceRecord.TotalCustomers.ToString();
                circuitSource.IsScada = circuitSourceRecord.IsScada.ToString();
            }
            catch (Exception ex)
            {
                Util.Logger.Error(string.Format("{0}: {1} == {2}", DateTime.Now.ToString(), ex.Message, ex.StackTrace));
                throw;
            }

            return circuitSource;
        }

        public IEnumerable<RobcCodeValue> PopulateRobcCodeValues()
        {
            ObservableCollection<RobcCodeValue> robcCodeValues = new ObservableCollection<RobcCodeValue>();

            Dictionary<string, string> robcs = GetDomainCodeValues(Constants.RobcDomainName);
            foreach (KeyValuePair<string, string> entry in robcs)
            {
                string code = entry.Key;
                string value = entry.Value;
                if (code.Trim() != Constants.EstablishedROBCCode)
                    robcCodeValues.Add(new RobcCodeValue(code, value));
            }
            return robcCodeValues.OrderBy(x => x.RobcDesc);

        }

        public IEnumerable<SubBlockCodeValue> PopulateSubBlockCodeValues()
        {
            ObservableCollection<SubBlockCodeValue> subBlockCodeValues = new ObservableCollection<SubBlockCodeValue>();

            Dictionary<string, string> subBlocks = GetDomainCodeValues(Constants.SubBlockDomainName);
            foreach (KeyValuePair<string, string> entry in subBlocks)
            {
                string code = entry.Key;
                string value = entry.Value;
                subBlockCodeValues.Add(new SubBlockCodeValue(code, value));
            }

            return subBlockCodeValues.OrderBy(x => x.SubBlockDesc);
        }

        public ROBCModel SaveROBC(ROBCModel robcModelObject)
        {

            ROBCManager robcManager = Util.RobcManager;
            CircuitSourceRecord circuitSourceRecord = CoreService.FindCircuit(robcModelObject.CircuitId);
            ROBCRecord robcRecord = null;
            try
            {
                List<ROBCRecord> robcRecords = robcManager.FindCircuitROBC(circuitSourceRecord);
                if (robcRecords.Any())
                {
                    foreach (ROBCRecord record in robcRecords)
                    {
                        record.SetFieldValue("DESIREDROBC", robcModelObject.DesiredROBC);
                        record.SetFieldValue("DESIREDSUBBLOCK", robcModelObject.DesiredSubBlock);
                        ROBCRecord newRecord = robcManager.UpdateROBC(record);
                        var pcpGuid = record.GetFieldValue("PARTCURTAILPOINTGUID") != null ? record.GetFieldValue("PARTCURTAILPOINTGUID").ToString() : string.Empty;

                        if (!string.IsNullOrEmpty(pcpGuid))
                        {
                            PCPRecord pcpRecordObj = robcManager.FindPCPByGlobalID(pcpGuid);

                            if (!robcManager.hasEssentialCustomerForPcp(pcpRecordObj))
                            {
                                robcManager.setEstablishedRobcForPcp_WithoutValidation(pcpRecordObj, false);
                            }
                        }
                        else
                        {
                            robcRecord = robcManager.setEstablishedRobcForCircuit(newRecord);
                        }
                    }
                }
            }
            catch
            {
                robcRecord = new ROBCRecord();
            }
            if (robcRecord == null) //No ROBC record? Create a new one.
            {
                robcRecord = new ROBCRecord();
                robcRecord.SetFieldValue("CIRCUITSOURCEGUID", robcModelObject.CircuitSourceGuid);
                robcRecord.SetFieldValue("DESIREDROBC", robcModelObject.DesiredROBC);
                robcRecord.SetFieldValue("DESIREDSUBBLOCK", robcModelObject.DesiredSubBlock);
                robcRecord = robcManager.UpdateROBC(robcRecord);
                robcManager.setEstablishedRobcForCircuit(robcRecord);
            }

            robcModelObject.EstablishedROBC = robcRecord.GetFieldValue("ESTABLISHEDROBC") != null ? robcRecord.GetFieldValue("ESTABLISHEDROBC").ToString() : string.Empty;
            robcModelObject.EstablishedSubBlock = robcRecord.GetFieldValue("ESTABLISHEDSUBBLOCK") != null ? robcRecord.GetFieldValue("ESTABLISHEDSUBBLOCK").ToString() : string.Empty;
            robcModelObject.CreatedDate = robcRecord.GetFieldValue("DATECREATED") != null ? robcRecord.GetFieldValue("DATECREATED").ToString() : string.Empty;
            robcModelObject.ModifiedDate = robcRecord.GetFieldValue("DATEMODIFIED") != null ? robcRecord.GetFieldValue("DATEMODIFIED").ToString() : string.Empty;

            robcModelObject.EstablishedROBCDesc = string.IsNullOrEmpty(robcModelObject.EstablishedROBC) ? string.Empty : GetDomainCodeValues(Constants.RobcDomainName)[robcModelObject.EstablishedROBC];
            robcModelObject.EstablishedSubBlockDesc = string.IsNullOrEmpty(robcModelObject.EstablishedSubBlock) ? string.Empty : GetDomainCodeValues(Constants.SubBlockDomainName)[robcModelObject.EstablishedSubBlock];

            return robcModelObject;

        }

        public bool SaveROBCs(List<ROBCModel> robcModels, ROBCModel searchedCircuitROBC, bool isParent)
        {
            ROBCManager robcManager = Util.RobcManager;
            List<ROBCRecord> robcRecords = new List<ROBCRecord>();
            List<string> circuitIDs = new List<string>();
            foreach (ROBCModel robcModelObject in robcModels)
            {
                CircuitSourceRecord circuitSourceRecord = robcManager.FindCircuit_NoLoadingNoScada(robcModelObject.CircuitId);
                ROBCRecord robcRecord;
                try
                {
                    robcRecord = CoreService.GetRobcRecordFromCircuit(circuitSourceRecord);
                }
                catch
                {
                    robcRecord = new ROBCRecord();
                }
                if (robcRecord == null) //No ROBC record? Create a new one.
                    robcRecord = new ROBCRecord();
                bool isParentFixedEssential = false;
                if (isParent)
                {
                    var establishedRobcValue = robcRecord.GetFieldValue("ESTABLISHEDROBC");
                    string establishedRobcValueStr = establishedRobcValue != null ? establishedRobcValue.ToString() : string.Empty;
                    if (establishedRobcValueStr == Constants.FixedEssentialROBCCode || establishedRobcValueStr == Constants.EstablishedROBCCode)
                    {
                        isParentFixedEssential = true;
                    }
                }
                if (!isParentFixedEssential)
                {
                    robcRecord.SetFieldValue("DESIREDROBC", searchedCircuitROBC.DesiredROBC);
                    robcRecord.SetFieldValue("DESIREDSUBBLOCK", searchedCircuitROBC.DesiredSubBlock);
                }
                robcRecord.SetFieldValue("CIRCUITSOURCEGUID", robcModelObject.CircuitSourceGuid);
                robcRecords.Add(robcRecord);
                circuitIDs.Add(robcModelObject.CircuitId);
            }
            robcManager.SaveROBCs(robcRecords);

            foreach (var robcRecord in robcRecords)
            {
                robcManager.setEstablishedRobcForCircuit(robcRecord);
            }

            return true;

        }

        public Dictionary<string, string> GetDomainCodeValues(string domainName)
        {
            if (domainName == Constants.RobcDomainName)
            {
                if (_RobcDomainValueList == null)
                {
                    _RobcDomainValueList = _RobcManager.GetDomainValues(domainName);
                }
                return _RobcDomainValueList;

            }
            else if (domainName == Constants.SubBlockDomainName)
            {
                if (_SubBlockDomainValueList == null)
                {
                    _SubBlockDomainValueList = _RobcManager.GetDomainValues(domainName);
                }
                return _SubBlockDomainValueList;
            }
            else if (domainName == Constants.Division)
            {
                if (_DivisionDomainValueList == null)
                {
                    _DivisionDomainValueList = _RobcManager.GetDomainValues(domainName);
                }
                return _DivisionDomainValueList;
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }
        #endregion
    }


    public class RobcCodeValue
    {
        public string RobcCode { get; set; }
        public string RobcDesc { get; set; }

        public RobcCodeValue(string robc, string robcDesc)
        {
            RobcCode = robc;
            RobcDesc = robcDesc;
        }
    }

    public class SubBlockCodeValue
    {
        public string SubBlockCode { get; set; }
        public string SubBlockDesc { get; set; }

        public SubBlockCodeValue(string subBlock, string subBlockDesc)
        {
            SubBlockCode = subBlock;
            SubBlockDesc = subBlockDesc;
        }
    }
}
