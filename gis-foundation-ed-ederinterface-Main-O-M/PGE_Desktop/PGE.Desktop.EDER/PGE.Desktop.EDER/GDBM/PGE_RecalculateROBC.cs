using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using PGE.BatchApplication.ROBC_API.DatabaseRecords;
using PGE.BatchApplication.ROBC_API;

namespace PGE.Desktop.EDER.GDBM
{
    class PGE_RecalculateROBC : PxActionHandler
    {
        public PGE_RecalculateROBC()
        {
            this.Name = "PGE Recalculate ROBC";
            this.Description = "PGE_RecalculateROBC: Recalculates established ROBC of the circuit and parent/child circuits";
        }

        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            this.Log.Info("PGE_RecalculateROBC: Executing PGE Recalculate ROBC for version: " + actionData.Version.VersionName);

            try
            {
                if ((new CommonFunctions()).CheckUsage(actionData.Version, this.Log, ServiceConfiguration) == true)
                {
                    return true;
                }
                if (parameters.Length < 8)
                    throw new ArgumentException("Insufficient GDBM parameters");
                else
                {
                    foreach (GdbmParameter param in parameters)
                    {
                        if (string.IsNullOrEmpty(param.Value))
                            throw new ArgumentNullException("Null or empty GDBM parameter values");
                    }
                }

                string ederDatabaseTNSName = string.Empty;
                string ederUserName = string.Empty;
                string ederPassword = string.Empty;
                string edSubstationDatabaseTNSName = string.Empty;
                string edSubstationUserName = string.Empty;
                string edSubstationPassword = string.Empty;
                string ederGeometricNetworkName = string.Empty;
                string ederSubGeometricNetworkName = string.Empty;

                foreach (GdbmParameter param in parameters)
                {
                    switch (param.Name)
                    {
                        case "EDERDatabaseTNSName":
                            ederDatabaseTNSName = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDERDatabaseTNSName: " + ederDatabaseTNSName);
                            break;
                        case "EDERUserName":
                            ederUserName = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDERUserName: " + ederUserName);
                            break;
                        case "EDERPassword":
                            ederPassword = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDERPassword: " + ederPassword);
                            break;
                        case "EDSubstationDatabaseTNSName":
                            edSubstationDatabaseTNSName = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDSubstationDatabaseTNSName: " + edSubstationDatabaseTNSName);
                            break;
                        case "EDSubstationUserName":
                            edSubstationUserName = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDSubstationUserName: " + edSubstationUserName);
                            break;
                        case "EDSubstationPassword":
                            edSubstationPassword = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDSubstationPassword: " + edSubstationPassword);
                            break;
                        case "EDERGeometricNetworkName":
                            ederGeometricNetworkName = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDERGeometricNetworkName: " + ederGeometricNetworkName);
                            break;
                        case "EDERSUBGeometricNetworkName":
                            ederSubGeometricNetworkName = param.Value;
                            this.Log.Info("PGE_RecalculateROBC: EDERSUBGeometricNetworkName: " + ederSubGeometricNetworkName);
                            break;
                    }
                }

                if (actionData["ROBCCircuitIDs"] != null)
                {

                    string circuitString = actionData["ROBCCircuitIDs"].ToString();
                    string[] circuiIDs = Regex.Split(circuitString, ",");

                    List<string> PCPtoProcess = new List<string>();
                    List<string> testPCP = new List<string>();

                    this.Log.Info("PGE_RecalculateROBC: Circuit count: " + circuiIDs.Length);
                    if (circuiIDs.Length > 0)
                    {
                        this.Log.Info("PGE_RecalculateROBC: PGE_RecalculateROBC: Initializing ROBC Manager... ");
                        ROBCManager robcManager = new ROBCManager(ederDatabaseTNSName, ederUserName, ederPassword, edSubstationDatabaseTNSName, edSubstationUserName, edSubstationPassword, ederGeometricNetworkName, ederSubGeometricNetworkName, true);

                        this.Log.Info("PGE_RecalculateROBC: Setting Established ROBC for each Circuit IDs... ");
                        foreach (string circuitID in circuiIDs)
                        {
                            try
                            {
                                this.Log.Info("PGE_RecalculateROBC: Processing Circuit ID: " + circuitID);

                                CircuitSourceRecord circuirSource = robcManager.FindCircuit_NoLoadingNoScada(circuitID);
                                List<ROBCRecord> listROBC = robcManager.FindCircuitROBC(circuirSource);
                                foreach (ROBCRecord robc in listROBC)
                                {
                                    if (robc.GlobalID != null && !string.IsNullOrEmpty(robc.GlobalID.ToString()))
                                    {
                                        string pcpGUID ="";
                                        PCPRecord pcpRecord = null;
                                        if (robc.GetFieldValue(SchemaInfo.Electric.PartialCurtailmentGUID)!=null)
                                        {
                                            pcpGUID = robc.GetFieldValue(SchemaInfo.Electric.PartialCurtailmentGUID).ToString();
                                        }
                                        if (pcpGUID != null && pcpGUID != "")
                                        {
                                            pcpRecord = robcManager.FindPCPByGlobalID(pcpGUID, true, true);
                                            if (pcpRecord != null && pcpRecord is PCPRecord)
                                            {
                                                if (!testPCP.Contains(circuitID)) { testPCP.Add(circuitID); PCPtoProcess.Add(pcpGUID); }

                                                robcManager.setEstablishedRobcForPcp(pcpRecord);
                                            }
                                        }
                                        else
                                        {
                                            robcManager.setEstablishedRobcForCircuit(robc);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Log.Warn(ServiceConfiguration.Name, "PGE_RecalculateROBC: Failed to recalculate Established ROBC: " + ex.Message);
                            }
                        }

                        for (int i = 0; i < PCPtoProcess.Count; i++)
                        {
                            robcManager.processPCPbyPCPGUID(PCPtoProcess[i]);               // SET THE PCP GUIDS - ONCE PER CIRCUIT
                        }

                        this.Log.Info("PGE_RecalculateROBC: Established ROBCs for Circuit IDs are set for the version: " + actionData.Version.VersionName);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "PGE_RecalculateROBC: Error during recalculating Established ROBC: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }

            return false;
        }

        public override bool Enabled(Miner.Geodatabase.GeodatabaseManager.ActionType actionType, Miner.Geodatabase.GeodatabaseManager.Actions gdbmAction, Miner.Geodatabase.GeodatabaseManager.PostVersionType versionType)
        {
            if (actionType == Miner.Geodatabase.GeodatabaseManager.ActionType.Post && gdbmAction == Miner.Geodatabase.GeodatabaseManager.Actions.AfterPost)
            {
                return true;
            }
            else
                return false;
        }

    }
}
