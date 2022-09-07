using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Configuration;
using System.Data;
using Oracle.DataAccess.Client;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;

namespace PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes
{
    class DMSRULECLASS
    {
        DBHelper DBHelperClass = new DBHelper();
        Common CommonFuntions = new Common();
        string DMS_RULE_TABLE = ConfigurationManager.AppSettings["DMS_RULE_TABLE"];
        string DMS_SESSION_DESCRIPTION = ConfigurationManager.AppSettings["SESSION_DESC_FOR_DMS"];

        internal void UpdateDMSTableFromStoredProcedure(string sProcedureName, string sCircuitId, string versionFullName)
        {
            var pListParams = new List<OracleParameter>();
            string sCircuitIdsIncldQuotes = null;
            try
            {
                sCircuitIdsIncldQuotes = "'" + sCircuitId + "'";


                OracleParameter pParam = new OracleParameter("iBatchID", OracleDbType.Varchar2);
                pParam.Direction = ParameterDirection.Input;
                pParam.Size = 100;
                pParam.Value = MainClass.Batch_Number;

                pListParams.Add(pParam);


                OracleParameter pParam1 = new OracleParameter("sCircuitId", OracleDbType.Varchar2);
                pParam1.Direction = ParameterDirection.Input;
                pParam1.Size = 1000;
                pParam1.Value = sCircuitId;

                pListParams.Add(pParam1);

                OracleParameter pParam2 = new OracleParameter("versionFullName", OracleDbType.Varchar2);
                pParam2.Direction = ParameterDirection.Input;
                pParam2.Size = 1000;
                pParam2.Value = versionFullName;

                pListParams.Add(pParam2);



                DBHelper DBHelperClass = new DBHelper();
                if (DBHelperClass.ExecuteStoredProcedureCommand(sProcedureName, pListParams) == false)
                {
                    throw new Exception("Error in Updating DMS Table through Stored Procedure," + sProcedureName);
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Error in Updating DMS table records through Stored Procedure," + sProcedureName + "," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }
    
     
        internal void UpdatePhaseInfoInDMSTable(IWorkspace pVersionWorkspace, string sCircuitID)
        {
            try
            {
                string strquery = "Update " + DMS_RULE_TABLE + " set Version_Name ='" + ((IVersion)(pVersionWorkspace)).VersionName.Split('.')[1] + "' where circuitid='" + sCircuitID + "' and BATCHID='" + MainClass.Batch_Number + "'";
                DBHelperClass.UpdateQuery(strquery);

                CommonFuntions.WriteLine_Info("DMS Table Attribute Update starts at," + DateTime.Now);
                UpdateDMSATTRFromStoredProcedure(ReadConfigurations.DMSATTRUPDATESPNAME, sCircuitID,((IVersion)pVersionWorkspace).VersionName.ToString());
                CommonFuntions.WriteLine_Info("DMS Table Attribute Update completed at," + DateTime.Now);
           
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                throw ex;
            }
        }

        private void UpdateDMSATTRFromStoredProcedure(string sProcedureName, string sCircuitId, string sVersionName)
        {
            var pListParams = new List<OracleParameter>();
            string sCircuitIdsIncldQuotes = null;
            try
            {
                sCircuitIdsIncldQuotes = "'" + sCircuitId + "'";


                OracleParameter pParam = new OracleParameter("iBatchID", OracleDbType.Varchar2);
                pParam.Direction = ParameterDirection.Input;
                pParam.Size = 100;
                pParam.Value = MainClass.Batch_Number;

                pListParams.Add(pParam);


                OracleParameter pParam1 = new OracleParameter("sCircuitId", OracleDbType.Varchar2);
                pParam1.Direction = ParameterDirection.Input;
                pParam1.Size = 1000;
                pParam1.Value = sCircuitId;

                pListParams.Add(pParam1);

                OracleParameter pParam2 = new OracleParameter("sUnprocessedTableName", OracleDbType.Varchar2);
                pParam2.Direction = ParameterDirection.Input;
                pParam2.Size = 1000;
                pParam2.Value = ReadConfigurations.UnprocessedTableName;

                pListParams.Add(pParam2);

                OracleParameter pParam3 = new OracleParameter("versionFullName", OracleDbType.Varchar2);
                pParam3.Direction = ParameterDirection.Input;
                pParam3.Size = 1000;
                pParam3.Value = sVersionName;

                pListParams.Add(pParam3);

                
                if (DBHelperClass.ExecuteStoredProcedureCommand(sProcedureName, pListParams) == false)
                {
                    throw new Exception("Error in Updating DMS Attribute through Stored Procedure," + sProcedureName);
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Error in Updating DMS Attribute through Stored Procedure," + sProcedureName + "," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }

        internal void RunValidationRuleOnDMSTable(IWorkspace pVersionWorkspace, string sCircuitID,int sessionid)
        {                    
            try
            {
                MainClass.g_DT_DMSError= new DataTable();             
                ReadConfigurations.sDMSErrorFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\LOG\\DMS_ERRORLOG_" + sCircuitID + "_" + MainClass.Batch_Number + "_" + DateTime.Now.ToString("yyyy_MM_dd_hh_mm") + ".xls";
                CommonFuntions.WriteLine_Info("DMS rule checking Process is started for CircuitID " + sCircuitID + " at," + DateTime.Now);
              
                string strquery = "select * from process.MM_SESSION where session_id=" + sessionid + " and DESCRIPTION like '%" + DMS_SESSION_DESCRIPTION + "%'";
                DataTable dt = DBHelperClass.GetDataTableByQuery(strquery);
                if (dt.Rows.Count != 0)
                {
                    CommonFuntions.WriteLine_Info("No Need to check the DMS Rule as Session Desc contains- " + DMS_SESSION_DESCRIPTION) ;
                    return;
                }

                #region run Open Switch Error
                FindPhaseMismatchAcrossNormalOpenDevices(sCircuitID, pVersionWorkspace as IFeatureWorkspace);
                #endregion 
               
                UpdatePhaseInfoInDMSTable(pVersionWorkspace, sCircuitID);
               
              

                #region DMS RUle Check
                var pListParams = new List<OracleParameter>();
                OracleParameter pParam = new OracleParameter("iBatchID", OracleDbType.Varchar2);
                pParam.Direction = ParameterDirection.Input;
                pParam.Size = 100;
                pParam.Value = MainClass.Batch_Number;

                pListParams.Add(pParam);

                OracleParameter pParam1 = new OracleParameter("p_ref", OracleDbType.RefCursor);
                pParam1.Direction = ParameterDirection.Output ;
               

                pListParams.Add(pParam1);

                MainClass.g_DT_DMSError = DBHelperClass.GetDataTablefromSP(ReadConfigurations.DMSCHECKPHASEMISMATCHRULESPNAME, pListParams);
                # endregion DMS RUle Check
                
                MainClass.g_DT_DMSError.Columns[0].ColumnName = "RULE_NAME";


                #region Single Phase on 12 KV Transformer check
                CommonFuntions.WriteLine_Info("Update DMS Error Count For Wrong Transformer Voltage is started for CircuitID " + sCircuitID + " at," + DateTime.Now);
                IFeatureClass pFeatClass = ((IFeatureWorkspace)pVersionWorkspace).OpenFeatureClass(ReadConfigurations.Devices.TransformerClassName);
                string swherecluase = "CircuitID='" + sCircuitID + "' and OPERATINGVOLTAGE=5 and PHASEDESIGNATION  in ('4','2','1')";
                CommonFuntions.WriteLine_Info(" Wrong Transformer Voltage count Query==" + swherecluase);
                IQueryFilter pqueryfilter = new QueryFilterClass();
                pqueryfilter.WhereClause = swherecluase;
                IFeatureCursor pfeatcursor = pFeatClass.Search(pqueryfilter, false);
                IFeature pfeature = pfeatcursor.NextFeature();
                while (pfeature != null)
                {
                    DataRow dr = MainClass.g_DT_DMSError.NewRow();
                    dr["RULE_NAME"] = "Single Phase on 12 KV Transformer";
                    dr["FEATURE_CLASS"] = "EDGIS.TRANSFORMER";
                    dr["FEATURE_OID"] = pfeature.OID.ToString();
                    dr["FEATURE_GUID"] =pfeature.get_Value(pfeature.Fields.FindField("GLOBALID")).ToString();
                    dr["FEATURE_CURRENT_PHASE"] = CommonFuntions.GetPhaseDomainValue(pfeature.get_Value(pfeature.Fields.FindField("PHASEDESIGNATION")).ToString());
                    MainClass.g_DT_DMSError.Rows.Add(dr);
                    pfeature = pfeatcursor.NextFeature();
                }
                CommonFuntions.WriteLine_Info("Update Count For Wrong Transformer Voltage in Log is completed for CircuitID " + sCircuitID + " at," + DateTime.Now);
                #endregion Single Phase on 12 KV Transformer check

              
                ReadConfigurations.DMSRuleErrorCount = MainClass.g_DT_DMSError.Rows.Count;
                CommonFuntions.WriteLine_Info("Total no of DMS Error Count = " + ReadConfigurations.DMSRuleErrorCount);
                
                CommonFuntions.ExportToExcel(MainClass.g_DT_DMSError, ReadConfigurations.sDMSErrorFile, "DMS_PHASE_MISMATCH_REPORT");
                CommonFuntions.WriteLine_Info("DMS rule checking Process is Completed for CircuitID " + sCircuitID + " at," + DateTime.Now);
              
              
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while Run the Rule on DMS table," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }
        private void FindPhaseMismatchAcrossNormalOpenDevices(string sCircuitID, IFeatureWorkspace pVersionWorkspace)
        {
            try
            {
                CommonFuntions.WriteLine_Info("Update Device Information in DMS Info Table is started  for CircuitID " + sCircuitID + " at," + DateTime.Now);
                string[] DeviceList = ReadConfigurations.DeviceList.Split(',');
                IList<IFeatureClass> ConductorClassList = new List<IFeatureClass>();
                ConductorClassList.Add(pVersionWorkspace.OpenFeatureClass(ReadConfigurations.Conductors.PrimaryOHConductorClassName));
                ConductorClassList.Add(pVersionWorkspace.OpenFeatureClass(ReadConfigurations.Conductors.PrimaryUGConductorClassName));
                ConductorClassList.Add(pVersionWorkspace.OpenFeatureClass(ReadConfigurations.Conductors.SecondaryOHConductorClassName));
                ConductorClassList.Add(pVersionWorkspace.OpenFeatureClass(ReadConfigurations.Conductors.SecondaryUGConductorClassName));
                ConductorClassList.Add(pVersionWorkspace.OpenFeatureClass("EDGIS.DISTBUSBAR"));
                for (int i = 0; i < DeviceList.Length; i++)
                {
                    IFeatureClass DeviceClass = pVersionWorkspace.OpenFeatureClass(DeviceList[i].ToString());
                    CommonFuntions.WriteLine_Info("Update Device Information is started  for Class " + DeviceList[i].ToString() + " at," + DateTime.Now);
                    IDictionary<string, IFeatureClass> Featureclasslist = new Dictionary<string, IFeatureClass>();             
                    if (DeviceClass == null)
                    {
                        throw new Exception("Device Class is not present in database " + DeviceList[i].ToString());
                    }
                    string strquery = (new DBQueries()).GetDevicesFromTraceTable(sCircuitID, DeviceList[i].ToString());

                    DataTable InputTraceResultDT = DBHelperClass.GetDataTableByQuery(strquery);
                    if (InputTraceResultDT != null && InputTraceResultDT.Rows.Count > 0)
                    {
                        for (int icount = 0; icount < InputTraceResultDT.Rows.Count; icount++)
                        {
                            DataRow DR = InputTraceResultDT.Rows[icount];
                            string Downstreamphase = "";
                            string UpStreamphase = "";
                            string sDeviceGUID = DR["TO_FEATURE_GLOBALID"].ToString();
                            string sDeviceOID = DR["TO_FEATURE_OID"].ToString();
                            string DevicePhase = string.Empty;
                            IFeature DeviceFeature = null;
                            DevicePhase = GetPhase(DeviceClass, sDeviceOID, out DeviceFeature);
                            if (DeviceFeature == null)
                            {
                                CommonFuntions.WriteLine_Error("Device feature not found in database " + sDeviceOID);
                                continue;
                            }
                            if (CheckDeviceIsOpen(DeviceFeature) == true)
                            {

                                IList<IFeature> ConnectedFeature = GetConnectedFeature(DeviceFeature, DeviceClass, (IFeatureWorkspace)pVersionWorkspace, ConductorClassList);
                                if (ConnectedFeature.Count == 2)
                                {
                                    IFeature UpstreamFeature = ConnectedFeature[0];
                                    IFeature DownstreamFeature = ConnectedFeature[1];
                                    //Get Upstream Feature

                                    string UpstreamFeatureGUID = UpstreamFeature.get_Value(UpstreamFeature.Fields.FindField("GLOBALID")).ToString();
                                    string UpstreamFeatureOID = UpstreamFeature.get_Value(0).ToString();
                                    string upstreamclass = ((IDataset)UpstreamFeature.Class).BrowseName;
                                    UpStreamphase = CommonFuntions.GetPhaseDomainValue(UpstreamFeature.get_Value(UpstreamFeature.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString());

                                    //Get DownStream Feature

                                    string DownstreamFeatureGUID = DownstreamFeature.get_Value(DownstreamFeature.Fields.FindField("GLOBALID")).ToString();
                                    string DownstreamFeatureOID = DownstreamFeature.get_Value(0).ToString();
                                    string Downstreamclass = ((IDataset)DownstreamFeature.Class).BrowseName;
                                    Downstreamphase = CommonFuntions.GetPhaseDomainValue(DownstreamFeature.get_Value(DownstreamFeature.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString());


                                    //Update DMS Table for given switch
                                    UpdateDMSforSWITCH(UpstreamFeatureOID, UpstreamFeatureGUID, upstreamclass, UpStreamphase, DownstreamFeatureOID, DownstreamFeatureGUID, Downstreamphase, Downstreamclass, sDeviceOID, sDeviceGUID, DevicePhase, DeviceList[i].ToString(), sCircuitID);
                                }
                                else
                                {
                                    CommonFuntions.WriteLine_Error("Conductor Count is not equal to 2 " + sDeviceOID);
                                    continue;
                                }


                            }


                        }
                    }
                    CommonFuntions.WriteLine_Info("Update Device Information is completed  for Class " + DeviceList[i].ToString() + " at," + DateTime.Now);
                

                }
                CommonFuntions.WriteLine_Info("Update Device Information in DMS Info Table is Completed  for CircuitID " + sCircuitID + " at," + DateTime.Now);

            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while updating the open devices," + ex.Message + " at " + ex.StackTrace);

            }
          
        }

        private IList<IFeature> GetConnectedFeature(IFeature DeviceFeature, IFeatureClass DeviceClass, IFeatureWorkspace iFeatureWorkspace, IList<IFeatureClass> conductorclass)
        {
            IList<IFeature> ConductorFeatures = new List<IFeature>();
            try 
            {
                for (int i = 0; i < conductorclass.Count; i++)
                {
                     GetConnectedConductor(DeviceFeature, conductorclass[i],ref ConductorFeatures);
                   
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while checking the normal poistion of the queried device," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
            return ConductorFeatures;
        }

        private void GetConnectedConductor(IFeature DeviceFeature, IFeatureClass conductorclass, ref IList<IFeature> ConductorFeatures)
        {
            IFeatureCursor pfeaturecursor = null;
            try
            {
                ISpatialFilter pspatialfilter = new SpatialFilterClass();
                pspatialfilter.Geometry = DeviceFeature.ShapeCopy as IGeometry;
                pspatialfilter.GeometryField = "SHAPE";
                pspatialfilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                pfeaturecursor = conductorclass.Search(pspatialfilter, false);
                IFeature pFeature = pfeaturecursor.NextFeature();
                while (pFeature != null)
                {
                    if (ConductorFeatures.Contains(pFeature) == false)
                    {
                        ConductorFeatures.Add(pFeature);
                    }
                    pFeature = pfeaturecursor.NextFeature();
                }

            }
            catch
            {
            }
            finally
            {
                if (pfeaturecursor != null) { Marshal.ReleaseComObject(pfeaturecursor); pfeaturecursor = null; }
            
            }
             
        }

        private bool CheckDeviceIsOpen(IFeature DeviceFeature)
        {
            bool retval = false;
            try
            {
                string NP_A = DeviceFeature.get_Value(DeviceFeature.Fields.FindField(ReadConfigurations.NORMALPOSITION_A_FieldName)).ToString();
                string NP_B = DeviceFeature.get_Value(DeviceFeature.Fields.FindField(ReadConfigurations.NORMALPOSITION_B_FieldName)).ToString();
                string NP_C = DeviceFeature.get_Value(DeviceFeature.Fields.FindField(ReadConfigurations.NORMALPOSITION_C_FieldName)).ToString();
                if ((NP_A == ReadConfigurations.NormalPosition.Open) || (NP_B == ReadConfigurations.NormalPosition.Open) || (NP_C == ReadConfigurations.NormalPosition.Open))
                {
                    retval = true;
                }
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception occurred while checking the normal poistion of the queried device," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
            return retval;
        }

        private void FindSwitchError(string sCircuitID, IFeatureWorkspace pVersionWorkspace)
        {
            try
            {
                CommonFuntions.WriteLine_Info("Update Device Information in DMS Info Table is started  for CircuitID " + sCircuitID + " at," + DateTime.Now);
               
                    IFeatureClass pfeatureclass = null;
                    IFeatureClass switchclass = null;
                    IDictionary<string, IFeatureClass> Featureclasslist = new Dictionary<string, IFeatureClass>();
                    string strquery = (new DBQueries()).GetTraceTableDownStreamFeaturesForPhaseUpdate(sCircuitID);
                    DataTable InputTraceResultDT = DBHelperClass.GetDataTableByQuery(strquery);
                    if (InputTraceResultDT != null && InputTraceResultDT.Rows.Count > 0)
                    {                      
                       switchclass = ((IFeatureWorkspace)pVersionWorkspace).OpenFeatureClass("EDGIS.SWITCH");                                                 
                       for (int icount = 0; icount < InputTraceResultDT.Rows.Count; icount++)
                        {
                            DataRow DR = InputTraceResultDT.Rows[icount];
                            string Downstreamphase="";
                            string UpStreamphase="";
                            string sGUID = DR["TO_FEATURE_GLOBALID"].ToString();
                            string sOID = DR["TO_FEATURE_OID"].ToString();
                            string sClassName = DR["physicalname"].ToString();
                            string SwitchPhase = string.Empty;
                            if (sClassName.ToUpper().Trim() == "EDGIS.SWITCH")
                            {
                                //if (sOID == "3233923")
                                //{ }
                                #region Validate Switch
                                IFeature SwitchFeature = null;
                                SwitchPhase = GetPhase(switchclass, sOID,out SwitchFeature);
                                if(SwitchFeature ==null)
                                {
                                     CommonFuntions.WriteLine_Error("Device feature not found in database "   + sOID);
                                     continue;
                                }
                                //Get Upstream Feature
                                pfeatureclass = null;
                                DataRow tempDR = InputTraceResultDT.Rows[icount - 1];
                                string UpstreamFeatureGUID = tempDR["TO_FEATURE_GLOBALID"].ToString();
                                string UpstreamFeatureOID = tempDR["TO_FEATURE_OID"].ToString();
                                string upstreamclass = tempDR["physicalname"].ToString();
                                Featureclasslist.TryGetValue(upstreamclass, out pfeatureclass);
                                if (pfeatureclass == null)
                                {
                                    pfeatureclass = ((IFeatureWorkspace)pVersionWorkspace).OpenFeatureClass(upstreamclass);

                                    if (Featureclasslist.ContainsKey(upstreamclass) == false)
                                    {
                                        Featureclasslist.Add(upstreamclass, pfeatureclass);
                                    }
                                }
                                if (pfeatureclass != null)
                                {
                                    UpStreamphase = GetPhase(pfeatureclass, UpstreamFeatureOID);
                                }

                               
                                //Get Downstream Feature
                                string DownstreamfeatureGUID = string.Empty;
                                string DownstreamfeatureOID= string.Empty;
                                string downstreamclass = string.Empty;
                                if (icount + 1 < InputTraceResultDT.Rows.Count)
                                {
                                    pfeatureclass = null;
                                    tempDR = InputTraceResultDT.Rows[icount + 1];
                                    DownstreamfeatureGUID = tempDR["TO_FEATURE_GLOBALID"].ToString();
                                    DownstreamfeatureOID = tempDR["TO_FEATURE_OID"].ToString();
                                    downstreamclass = tempDR["physicalname"].ToString();
                                    Featureclasslist.TryGetValue(downstreamclass, out pfeatureclass);
                                    if (pfeatureclass == null)
                                    {
                                        pfeatureclass = ((IFeatureWorkspace)pVersionWorkspace).OpenFeatureClass(downstreamclass);

                                        if (Featureclasslist.ContainsKey(downstreamclass) == false)
                                        {
                                            Featureclasslist.Add(downstreamclass, pfeatureclass);
                                        }
                                    }
                                    if (pfeatureclass != null)
                                    {
                                        Downstreamphase = GetPhase(pfeatureclass, DownstreamfeatureOID);
                                    }
                                }
                                //Update DMS Table for given switch
                                UpdateDMSforSWITCH(UpstreamFeatureOID, UpstreamFeatureGUID, upstreamclass, UpStreamphase, DownstreamfeatureOID, DownstreamfeatureGUID, Downstreamphase, downstreamclass, sOID, sGUID, SwitchPhase, sClassName, sCircuitID);

                                #endregion
                            }
                        }
                   
                    }
                    CommonFuntions.WriteLine_Info("Update Device Information in DMS Info Table is Completed  for CircuitID " + sCircuitID + " at," + DateTime.Now);
              
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while updating the switch," + ex.Message + " at " + ex.StackTrace);

            }
        }

        private string GetPhase(IFeatureClass switchclass, string sOID, out IFeature SwitchFeature)
        {
            string sphase = string.Empty;
            SwitchFeature = null; 
            try
            {
                SwitchFeature = switchclass.GetFeature(Convert.ToInt32(sOID));
                sphase = CommonFuntions.GetPhaseDomainValue(SwitchFeature.get_Value(SwitchFeature.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString());
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while getting the phase of the feature," + ex.Message + " at " + ex.StackTrace);

            }
            return sphase;
        }

        private void UpdateDMSforSWITCH(string UpstreamFeatureOID, string UpstreamFeatureGUID, string upstreamclass, string UpStreamphase, string DownstreamfeatureOID, string DownstreamfeatureGUID, string Downstreamphase, string downstreamclass, string sOID, string sGUID, string SwitchPhase, string sClassName, string sCircuitID)
        {
            try
            {
                string strquery = "select Count(*) from " + DMS_RULE_TABLE + " where batchid = " + MainClass.Batch_Number + " and Feature_OID = " + sOID;
                DataTable dt = DBHelperClass.GetDataTableByQuery(strquery);
                if ((dt != null) && (dt.Rows.Count > 0))
                {
                  
                    if (Convert.ToInt32(dt.Rows[0].ItemArray[0].ToString()) > 0)
                    {
                        string sUpdateQuery = "update " + DMS_RULE_TABLE + " set feature_current_phase='" + SwitchPhase + "' ,UPSTREAM_FEATURE_OID = " + UpstreamFeatureOID + ", UPSTREAM_FEATURE_GUID = '" + UpstreamFeatureGUID + "',"
                             + " UPSTREAM_FEATURE_PHASE='" + UpStreamphase + "',UPSTREAM_FEATURE_CLASS='" + upstreamclass + "',DownSTREAM_FEATURE_OID = " + DownstreamfeatureOID + ", DOWNSTREAM_FEATURE_GUID = '" + DownstreamfeatureGUID + "',"
                             + " DownSTREAM_FEATURE_PHASE='" + Downstreamphase + "',DownSTREAM_FEATURE_CLASS='" + downstreamclass + "' where feature_oid=" + sOID + " and batchid='" + MainClass.Batch_Number + "'";

                        DBHelperClass.UpdateQuery(sUpdateQuery);
                    }
                    else
                    {
                        string sInsertQuery = "  Insert into PGEIGPDATA.PGE_IGP_DMS_RULE_INFO_DATA(feature_class,feature_oid,feature_guid,circuitid,feature_current_phase,BatchID,UPSTREAM_FEATURE_OID,UPSTREAM_FEATURE_GUID,UPSTREAM_FEATURE_PHASE,UPSTREAM_FEATURE_CLASS,DOWNSTREAM_FEATURE_OID,DOWNSTREAM_FEATURE_GUID,DOWNSTREAM_FEATURE_PHASE,DOWNSTREAM_FEATURE_CLASS) "
                       + "values ('" + sClassName + "', " + sOID + " ,'" + sGUID + "','" + sCircuitID + "','" + SwitchPhase + "' ," + MainClass.Batch_Number
                        + "," + UpstreamFeatureOID + ",'" + UpstreamFeatureGUID + "','" + UpStreamphase + "','" + upstreamclass + "'"
                       + "," + DownstreamfeatureOID + ",'" + DownstreamfeatureGUID + "','" + Downstreamphase + "','" + downstreamclass + "')";

                        DBHelperClass.UpdateQuery(sInsertQuery);
                      
                    }
                }


            }
            catch { }
        }
 

        private string GetPhase(IFeatureClass sclass,string sOID)
        {
            string sphase = string.Empty;
            try
            {
                IFeature pfeature = sclass.GetFeature(Convert.ToInt32(sOID));
                sphase = CommonFuntions.GetPhaseDomainValue(pfeature.get_Value(pfeature.Fields.FindField(ReadConfigurations.PhaseDesignationFieldName)).ToString());
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while getting the phase of the feature," + ex.Message + " at " + ex.StackTrace);

            }

            return sphase;
        }

     

        private void FillDatatable(ref DataTable dt)
        {
            try
            {
                //DataTable pDT = new DataTable();
                dt= new DataTable();
                DataColumn DC = new DataColumn("RULE_NAME");
                dt.Columns.Add(DC);
                DC = new DataColumn("FEATURE_CLASS");
                dt.Columns.Add(DC);
                DC = new DataColumn("FEATURE_OID");
                dt.Columns.Add(DC);
                DC = new DataColumn("FEATURE_GUID");
                dt.Columns.Add(DC);
                DC = new DataColumn("FEATURE_CURRENT_PHASE");
                dt.Columns.Add(DC);
                DC = new DataColumn("UPSTREAM_FEATURE_CLASS");
                dt.Columns.Add(DC);
                DC = new DataColumn("UPSTREAM_FEATURE_OID");
                dt.Columns.Add(DC);
                DC = new DataColumn("UPSTREAM_FEATURE_GUID");
                dt.Columns.Add(DC);
                DC = new DataColumn("UPSTREAM_FEATURE_PHASE");
                dt.Columns.Add(DC);
                DC = new DataColumn("DOWNSTREAM_FEATURE_CLASS");
                dt.Columns.Add(DC);
                DC = new DataColumn("DOWNSTREAM_FEATURE_OID");
                dt.Columns.Add(DC);
                DC = new DataColumn("DOWNSTREAM_FEATURE_GUID");
                dt.Columns.Add(DC);
                DC = new DataColumn("DOWNSTREAM_FEATURE_PHASE");
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + "   " + ex.StackTrace);
                throw ex;
            }
            
        }


        internal int FindPhaseMismatch(string sCircuitID)
        {
            int DMSMismatchCount = 0;
            DataTable dt = new DataTable();
            try
            {
                //CommonFuntions.WriteLine_Info("DMS Phase Mismatch count process is started  for CircuitID " + sCircuitID + " at," + DateTime.Now);
                 var pListParams = new List<OracleParameter>();
                OracleParameter pParam = new OracleParameter("iBatchID", OracleDbType.Varchar2);
                pParam.Direction = ParameterDirection.Input;
                pParam.Size = 100;
                pParam.Value = MainClass.Batch_Number;

                pListParams.Add(pParam);

                OracleParameter pParam1 = new OracleParameter("p_ref", OracleDbType.RefCursor);
                pParam1.Direction = ParameterDirection.Output ;
               

                pListParams.Add(pParam1);

                dt = DBHelperClass.GetDataTablefromSP(ReadConfigurations.DMSCHECKPHASEMISMATCHRULESPNAME, pListParams);
                DMSMismatchCount = dt.Rows.Count;
               // CommonFuntions.WriteLine_Info("Total no of DMS Error Count = " + DMSMismatchCount);
               // CommonFuntions.WriteLine_Info("DMS Phase Mismatch count process is Completed for CircuitID " + sCircuitID + " at," + DateTime.Now);
             
            }
            catch(Exception ex)
            {
                CommonFuntions.WriteLine_Error("Exception Occurred while Run the Rule on DMS table to send the reminder mail," + ex.Message + " at " + ex.StackTrace);
                
            }
            return DMSMismatchCount;
        }
    }
}
