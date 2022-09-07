using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Miner.Process.GeodatabaseManager.ActionHandlers;
using ESRI.ArcGIS.Geodatabase;
using Telvent.Delivery.Framework.FeederManager;
using Miner.Framework.Trace.Strategies;
using Miner.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.NetworkAnalysis;
using Telvent.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using Miner.Geodatabase.GeodatabaseManager.Serialization;
using ROBC_API;
using ROBC_API.DatabaseRecords;

namespace Telvent.PGE.ED.Desktop.GDBM
{
    class PGE_RecalculateROBC : PxActionHandler
    {
        public PGE_RecalculateROBC()
        {
            this.Name = "PGE Recalculate ROBC";
            this.Description = "Recalculates established ROBC of the circuit and parent/child circuits";
        }

        protected override bool PxSubExecute(Miner.Process.GeodatabaseManager.PxActionData actionData, Miner.Geodatabase.GeodatabaseManager.Serialization.GdbmParameter[] parameters)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            this.Log.Info("Executing PGE Recalculate ROBC for version: " + actionData.Version.VersionName);

            try
            {
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
                            this.Log.Info("EDERDatabaseTNSName: " + ederDatabaseTNSName);
                            break;
                        case "EDERUserName":
                            ederUserName = param.Value;
                            this.Log.Info("EDERUserName: " + ederUserName);
                            break;
                        case "EDERPassword":
                            ederPassword = param.Value;
                            this.Log.Info("EDERPassword: " + ederPassword);
                            break;
                        case "EDSubstationDatabaseTNSName":
                            edSubstationDatabaseTNSName = param.Value;
                            this.Log.Info("EDSubstationDatabaseTNSName: " + edSubstationDatabaseTNSName);
                            break;
                        case "EDSubstationUserName":
                            edSubstationUserName = param.Value;
                            this.Log.Info("EDSubstationUserName: " + edSubstationUserName);
                            break;
                        case "EDSubstationPassword":
                            edSubstationPassword = param.Value;
                            this.Log.Info("EDSubstationPassword: " + edSubstationPassword);
                            break;
                        case "EDERGeometricNetworkName":
                            ederGeometricNetworkName = param.Value;
                            this.Log.Info("EDERGeometricNetworkName: " + ederGeometricNetworkName);
                            break;
                        case "EDERSUBGeometricNetworkName":
                            ederSubGeometricNetworkName = param.Value;
                            this.Log.Info("EDERSUBGeometricNetworkName: " + ederSubGeometricNetworkName);
                            break;
                    }
                }

                if (actionData["ROBCCircuitIDs"] != null)
                {

                    string circuitString = actionData["ROBCCircuitIDs"].ToString();
                    string[] circuiIDs = Regex.Split(circuitString, ",");


                    this.Log.Info("Circuit count: " + circuiIDs.Length);
                    if (circuiIDs.Length > 0)
                    {
                        this.Log.Info("Initializing ROBC Manager... ");
                        ROBCManager robcManager = new ROBCManager(ederDatabaseTNSName, ederUserName, ederPassword, edSubstationDatabaseTNSName, edSubstationUserName, edSubstationPassword, ederGeometricNetworkName, ederSubGeometricNetworkName);

                        this.Log.Info("Setting Established ROBC for each Circuit IDs... ");
                        foreach (string circuitID in circuiIDs)
                        {
                            try
                            {
                                CircuitSourceRecord circuirSource = robcManager.FindCircuit(circuitID);
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
                                            pcpRecord = robcManager.FindPCPByGlobalID(pcpGUID);
                                            if (pcpRecord != null && pcpRecord is PCPRecord)
                                            {
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
                                this.Log.Warn(ServiceConfiguration.Name, "Failed to recalculate Established ROBC: " + ex.Message);
                            }
                        }

                        this.Log.Info("Established ROBCs for Circuit IDs are set for the version: " + actionData.Version.VersionName);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                this.Log.Error(ServiceConfiguration.Name, "Error during recalculating Established ROBC: " + ex.Message + " StackTrace: " + ex.StackTrace);
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
