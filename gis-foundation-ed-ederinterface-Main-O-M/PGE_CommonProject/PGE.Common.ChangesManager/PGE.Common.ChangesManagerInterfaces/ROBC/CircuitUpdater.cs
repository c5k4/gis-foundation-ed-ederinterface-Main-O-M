using System;
using System.Collections.Generic;
using System.Reflection;
using PGE.BatchApplication.ROBC_API;
using PGE.BatchApplication.ROBC_API.DatabaseRecords;
using PGE.Common.ChangesManagerShared;
//using ROBC_API;
//using ROBC_API.DatabaseRecords;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Common.ChangesManagerShared.ROBC
{
    public class CircuitUpdater
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "ChangeDetectionDefault.log4net.config");

        private SDEWorkspaceConnection _ederSDEWorkspaceConnection;
        private SDEWorkspaceConnection _ederSubSDEWorkspaceConnection;
        private AdoOracleConnection _ederAdoOracleConn;
        private AdoOracleConnection _ederSubAdoOracleConn;         
        private string _ederGeometricNetworkName;
        private string _ederSubGeometricNetworkName;
        private const string PartialCurtailmentGUID = "PARTCURTAILPOINTGUID";

        public CircuitUpdater(
            SDEWorkspaceConnection sdeEDERWorkspaceConnection,
            SDEWorkspaceConnection sdeEDERSUBWorkspaceConnection,
            AdoOracleConnection adoEDEROracleConnection,
            AdoOracleConnection adoEDERSUBOracleConnection,              
            string EDERGeometricNetworkName,
            string EDERSUBGeometricNetworkName)
        {
            _ederSDEWorkspaceConnection = sdeEDERWorkspaceConnection;
            _ederSubSDEWorkspaceConnection = sdeEDERSUBWorkspaceConnection;
            _ederAdoOracleConn = adoEDEROracleConnection;
            _ederSubAdoOracleConn = adoEDERSUBOracleConnection;            
            _ederGeometricNetworkName = EDERGeometricNetworkName;
            _ederSubGeometricNetworkName = EDERSUBGeometricNetworkName;
        }

        public void Dispose()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
        }
        public void InitializeWorkspace()
        {
            _logger.Debug(MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// Processes the ROBC edits for each circuit 
        /// </summary>
        /// <param name="htCircuitIds"></param>
        public void ProcessCircuits(HashSet<string> htCircuitIds)
        {
            try
            {
                _logger.Debug(MethodBase.GetCurrentMethod().Name);
                _logger.Info("Entering ROBC Change Detection circuit processing");
                int circuitCounter = 0;
                int circuitTotal = 0;

                if (htCircuitIds != null)
                {

                    List<string> PCPtoProcess = new List<string>();
                    List<string> testPCP = new List<string>();

                    _logger.Info("PGE_RecalculateROBC: Circuit count: " + htCircuitIds.Count);
                    if (htCircuitIds.Count > 0)
                    {
                        circuitTotal = htCircuitIds.Count;
                        _logger.Info("PGE_RecalculateROBC: PGE_RecalculateROBC: Initializing ROBC Manager... ");

                        //need a new initializer for this 
                        ROBCManager robcManager = new ROBCManager(
                            _ederSDEWorkspaceConnection.Workspace,
                            _ederSubSDEWorkspaceConnection.Workspace,
                            _ederAdoOracleConn.OracleConnection,
                            _ederSubAdoOracleConn.OracleConnection,
                            _ederGeometricNetworkName,
                            _ederSubGeometricNetworkName,
                            true,true); //(V3SF) Set True to save Workspace
                            //false);
                        _logger.Info("PGE_RecalculateROBC: Setting Established ROBC for each Circuit IDs... ");
                        foreach (string circuitID in htCircuitIds)
                        {
                            try
                            {
                                circuitCounter++;
                                _logger.Info("PGE_RecalculateROBC: Processing Circuit ID: " + circuitID);
                                _logger.Info("Processing circuit: " + circuitCounter + " of: " + circuitTotal);


                                CircuitSourceRecord circuirSource = robcManager.FindCircuit_NoLoadingNoScada(circuitID);
                                List<ROBCRecord> listROBC = robcManager.FindCircuitROBC(circuirSource);
                                foreach (ROBCRecord robc in listROBC)
                                {
                                    if (robc.GlobalID != null && !string.IsNullOrEmpty(robc.GlobalID.ToString()))
                                    {
                                        string pcpGUID = "";
                                        PCPRecord pcpRecord = null;
                                        if (robc.GetFieldValue(PartialCurtailmentGUID) != null)
                                        {
                                            pcpGUID = robc.GetFieldValue(PartialCurtailmentGUID).ToString();
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
                                _logger.Warn("PGE_RecalculateROBC: Failed to recalculate Established ROBC: " + ex.Message);
                            }
                        }

                        for (int i = 0; i < PCPtoProcess.Count; i++)
                        {
                            robcManager.processPCPbyPCPGUID(PCPtoProcess[i]);               // SET THE PCP GUIDS - ONCE PER CIRCUIT
                        }
                    }
                }

                _logger.Info("  Successfully processed the changed circuits.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error in routine " + MethodBase.GetCurrentMethod().Name + ": " +
                    ex.Message);
                throw new Exception("Error processing ROBC Circuits");
            }
            finally
            {
                if (_ederAdoOracleConn != null)
                    _ederAdoOracleConn.OracleConnection.Close();
                if (_ederSubAdoOracleConn != null)
                    _ederSubAdoOracleConn.OracleConnection.Close();

            }
        }
    }
}
