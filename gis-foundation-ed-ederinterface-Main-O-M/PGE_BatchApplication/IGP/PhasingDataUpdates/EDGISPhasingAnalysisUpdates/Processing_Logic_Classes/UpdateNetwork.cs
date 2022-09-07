using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.NetworkAnalysis;
using Miner.Interop;

namespace PGE.BatchApplication.IGPPhaseUpdate
{
    class UpdateNetwork
    {
        private string sPreviousGUID = string.Empty;
        private string sPreviousConductorPhase = string.Empty;
        private string PreviousClass = string.Empty;
        private string PreviousOrderNum = string.Empty;
        private string sError = string.Empty;
        Common Common = new Common();
        DBHelper DBHelper = new DBHelper();
        DBQueries DBQueries = new DBQueries();
        private DataTable ConductorTable = new DataTable();
        private DataTable TransformerTable = new DataTable();
        private DataTable PriOHDataTable = new DataTable();
        private DataTable PriUGDataTable = new DataTable();
        private DataTable BusBarDataTable = new DataTable();
        private DataTable EDGISTransformerDataTable = new DataTable();
        UpdateSecondaryNetwork UpdateSecondaryNetwork = new UpdateSecondaryNetwork();
      

        #region Update Network
        public void IGPPhaseUpdate(DataTable InputTraceResultDT, string sFeederID, string Batch_Number, string LoadType)
        {
            try
            {
                #region Set Datatable
                string strSelectQuery = "select * from " + ReadConfigurations.DAPHIEConductorTableName + " where CircuitID='" + sFeederID + "' and Batchid='" + Batch_Number + "'";
                ConductorTable = DBHelper.GetDataTable(strSelectQuery);

                strSelectQuery = "select * from " + ReadConfigurations.DAPHIETransformerTableName + " where CircuitID='" + sFeederID + "' and Batchid='" + Batch_Number + "'";
                TransformerTable = DBHelper.GetDataTable(strSelectQuery);

                strSelectQuery = "select " + ReadConfigurations.PhaseDesignationFieldName + ", OBJECTID,GlobalID,SUBTYPECD from " + ReadConfigurations.Conductors_ViewName.PrimaryOHConductorViewName + " where circuitid='" + sFeederID + "'";
                PriOHDataTable = DBHelper.GetDataTable(strSelectQuery);

                strSelectQuery = "select " + ReadConfigurations.PhaseDesignationFieldName + ", OBJECTID,GlobalID,SUBTYPECD from " + ReadConfigurations.Conductors_ViewName.PrimaryUGConductorViewName + " where circuitid='" + sFeederID + "'";
                PriUGDataTable = DBHelper.GetDataTable(strSelectQuery);

                strSelectQuery = "select " + ReadConfigurations.PhaseDesignationFieldName + ", OBJECTID,GlobalID,SUBTYPECD from " + ReadConfigurations.Conductors_ViewName.DistBusBarViewName + " where circuitid='" + sFeederID + "'";
                BusBarDataTable = DBHelper.GetDataTable(strSelectQuery);

                strSelectQuery = "select " + ReadConfigurations.PhaseDesignationFieldName + ", OBJECTID,GlobalID,SUBTYPECD from " + ReadConfigurations.Devices_ViewName.TransformerViewName + " where circuitid='" + sFeederID + "'";
                EDGISTransformerDataTable = DBHelper.GetDataTable(strSelectQuery);

                strSelectQuery = "SELECT " + ReadConfigurations.DAPHIETablesFields.ServiceGUIDField + "," + ReadConfigurations.DAPHIETablesFields.ServicePhaseField + " FROM " +
                    ReadConfigurations.DAPHIEMeterTableName + " WHERE " + ReadConfigurations.DAPHIETablesFields.BATCH_NUMBER + " = '" + Batch_Number + "' AND PHASE_PREDICTION IN (1,2,3,4,5,6,7)";
                UpdateSecondaryNetwork.m_pDT_DaphieServiceLocations = DBHelper.GetDataTable(strSelectQuery);

                #endregion


                for (int icount = 0; icount < InputTraceResultDT.Rows.Count; icount++)
                {
                    DataRow DR = InputTraceResultDT.Rows[icount];
                    Console.WriteLine(icount);
                    sError = string.Empty;
                    string sReviewBy = null;
                    string order_num = DR["ORDER_NUM"].ToString();
                    string sOID = DR["TO_FEATURE_OID"].ToString();
                    string sToFeature_EID = DR["TO_FEATURE_EID"].ToString();
                    string sFROMFeature_EID = DR["FROM_FEATURE_EID"].ToString();
                    string sClassName = DR["physicalname"].ToString();
                    string sClass_ViewName = Common.ReturnViewNameFromClassName(DR["physicalname"].ToString());
                    if ((sClass_ViewName == ReadConfigurations.Devices_ViewName.ElectricNetJunctionViewName) ||
                        (sClassName == "EDGIS.ELECTRICSTITCHPOINT")) continue;
                    //if (sOID == "5890911")
                    //{ Console.WriteLine("ss"); }
                     
                    string sSubType = null;
                    string sGUID = Common.GetGUIDFromOID(sOID, sClass_ViewName, ref sSubType);

                    if (string.IsNullOrEmpty(sGUID))
                    {
                        sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[0];
                        sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[1];
                        Common.WriteLine_Error(sError + "," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                        Common.UpdateErrorAttribute(sError, string.Empty, sClassName, Convert.ToInt32(sOID), string.Empty, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, sPreviousGUID, PreviousClass);
                    }

                    if ((!string.IsNullOrEmpty(sOID)) && (!string.IsNullOrEmpty(sToFeature_EID)) && (!string.IsNullOrEmpty(sFROMFeature_EID)) && (!string.IsNullOrEmpty(sClass_ViewName)))
                    {

                        if ((sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.PrimaryOHConductorViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.PrimaryUGConductorViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.DistBusBarViewName))
                        {

                            #region Update Primary Conductors

                            sError = string.Empty;
                            string UpstreamConductorphase = string.Empty;
                            string sCurrentPhase = GetFeatureFieldValuefromClass(sClassName, sClass_ViewName, sOID, sGUID, sFeederID, sSubType, sReviewBy, ReadConfigurations.PhaseDesignationFieldName);
                            string SourceClass = string.Empty;
                            string sDAPHIEPhase = GetSingleConductorPhaseFromDAPHIEwithClass(sGUID, Batch_Number, out SourceClass, sFeederID);
                            //If the errorneous record coming from daphie
                            if (!(string.IsNullOrEmpty(sError)))
                            {
                                sReviewBy = "";
                                Common.WriteLine_Error(sError + " for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                Common.UpdateErrorAttribute(sError, sCurrentPhase, sClassName, Convert.ToInt32(sOID), sDAPHIEPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, sPreviousGUID, PreviousClass);
                            }
                            else
                            {
                                if (LoadType == "BULK")
                                {
                                    if (string.IsNullOrEmpty(sDAPHIEPhase))
                                    {
                                        sError = string.Empty;
                                        //To Handle the Missing Phase get the Immediate Upstream Conductor Phase                                        
                                        //EGIS-975 :ME Q3 : Feedback from DMFM : If existing phasing of conductor w/ missing JSON prediction = ABC, then default to ABC phasing; else use existing logic for populating phasing of missing conductor predictions.” 
                                        if (sCurrentPhase == "7")
                                        {
                                            UpstreamConductorphase = "7";
                                        }
                                        else
                                        {
                                            UpstreamConductorphase = GetSingleConductorPhaseFromDAPHIEwithClass(sPreviousGUID, Batch_Number, out SourceClass, sFeederID);
                                            //EGIS-975 : Feedback from DMFM : we never change the LEN(PhaseDesignation) for any conductor segment on account of a missing conductor prediction in JSON.
                                            //Check for For Two Phase for which phase prediction missing in JSON : If LEN(PhaseDesignation) of new phase prediction does not equal LEN(PhaseDesignation) of existing GIS record, then making it to default AB i.e. 6
                                            if (sCurrentPhase == "6" || sCurrentPhase == "5" || sCurrentPhase == "3")
                                            {
                                                if (sCurrentPhase != UpstreamConductorphase)
                                                {
                                                    UpstreamConductorphase = "6";
                                                }
                                            }
                                            //Check for For Single Phase for which phase prediction missing in JSON : If LEN(PhaseDesignation) of new phase prediction does not equal LEN(PhaseDesignation) of existing GIS record, then making it to default A i.e. 4
                                            if (sCurrentPhase == "4" || sCurrentPhase == "2" || sCurrentPhase == "1")
                                            {
                                                if (sCurrentPhase != UpstreamConductorphase)
                                                {
                                                    UpstreamConductorphase = "4";
                                                }
                                            }
                                        }
                                        if (string.IsNullOrEmpty(sError))
                                        {
                                            sDAPHIEPhase = UpstreamConductorphase;
                                        }
                                        if (string.IsNullOrEmpty(sDAPHIEPhase) && (string.IsNullOrEmpty(sError)))
                                        {
                                            sDAPHIEPhase = Common.GetDataFromDatatable("FEATURE_GUID='" + sPreviousGUID + "' and ERROR_MSG is null", "VALUE", MainClass.g_dtAllDetails); ;
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(sDAPHIEPhase))
                                {
                                    //Update Primary Conductors
                                    if (UpdatePrimaryConductors(sClassName, sClass_ViewName, sOID, sFeederID, sFROMFeature_EID, sDAPHIEPhase, sSubType, sReviewBy, "BULK", Batch_Number, sCurrentPhase, sGUID) == false)
                                    {
                                        Common.WriteLine_Error("Conductor is Not Updated for ObjectID," + sOID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                    }
                                }
                                else
                                {

                                    if (LoadType == "BULK")
                                    {
                                        sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_10.Split(':')[0];
                                        sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_10.Split(':')[1];

                                        Common.WriteLine_Error(sError + " for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                        Common.UpdateErrorAttribute(sError, sCurrentPhase, sClassName, Convert.ToInt32(sOID), string.Empty, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, sPreviousGUID, PreviousClass);
                                    }
                                }

                            }
                            # endregion
                        }
                        else if ((sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.SwitchViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.FUSEViewName) ||
                                    (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.OPenPointViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.DPDViewName))
                        {
                            #region Update Switch,DPD,Fuse,OpenPoint
                            //Get Upstream Feature
                            DataRow tempDR = InputTraceResultDT.Rows[icount - 1];
                            string UpstreamFetureGUID = tempDR["TO_FEATURE_GLOBALID"].ToString();
                            string upstreamclass = tempDR["physicalname"].ToString();
                            //Get Downstream Feature
                            string DownstreamfeatureGUID = string.Empty;
                            string downstreamclass = string.Empty;
                            if (icount + 1 < InputTraceResultDT.Rows.Count)
                            {
                                tempDR = InputTraceResultDT.Rows[icount + 1];
                                DownstreamfeatureGUID = tempDR["TO_FEATURE_GLOBALID"].ToString();
                                downstreamclass = tempDR["physicalname"].ToString();
                            }
                            if (((upstreamclass == "EDGIS.SECOHCONDUCTOR") || (upstreamclass == "EDGIS.SECUGCONDUCTOR")) && ((downstreamclass == "EDGIS.SECOHCONDUCTOR") || (downstreamclass == "EDGIS.SECUGCONDUCTOR")))
                            {
                                continue;
                            }
                            sError = string.Empty;
                            string valueToSearch = sOID;
                            string columnName = "FEATURE_OID";
                            //int count = (int)MainClass.g_dtAllDetails.Compute(string.Format("count({0})", columnName),
                            //        string.Format("{0} like '{1}'", columnName, valueToSearch));
                            int count = (int)MainClass.g_dtAllDetails.Compute("count(" + columnName + ")", columnName + "=" + valueToSearch);

                            //Update switch,Fuse,DPD,OpenPoint with Upstream Conductor
                            if (count == 0)
                            {
                                //if (Common.CheckNormalPosition(sOID, sClass_ViewName) == "CLOSE")
                                //{
                                    if (UpdateSDOFDeviceForBulk(UpstreamFetureGUID, DownstreamfeatureGUID, sClassName, sClass_ViewName, sOID, sGUID, sFeederID, sToFeature_EID, sFROMFeature_EID, sSubType, sReviewBy, Batch_Number) == false)
                                    {
                                        Common.WriteLine_Error("SDOF Device Phase is Not Updated for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                    }
                                //}
                                
                            }
                            # endregion
                        }
                        else if (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.TransformerViewName)
                        {
                            #region Update Transformer
                            sError = string.Empty;
                            string SourceGUID = string.Empty;
                            string sCurrentPhase = GetFeatureFieldValuefromClass(sClassName, sClass_ViewName, sOID, sGUID, sFeederID, sSubType, sReviewBy, ReadConfigurations.PhaseDesignationFieldName);
                            string sTransformerPhase = GetTransformerPhaseFromDAPHIEAlongWithSource(sGUID, sFeederID, Batch_Number, out SourceGUID);
                            if (!(string.IsNullOrEmpty(sError)))
                            {
                                Common.WriteLine_Error(sError + " for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                Common.UpdateErrorAttribute(sError, sCurrentPhase, sClassName, Convert.ToInt32(sOID), sTransformerPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, sPreviousGUID, PreviousClass);

                            }
                            else
                            {
                                 string SourceClass = string.Empty;
                                if (LoadType == "BULK")
                                {
                                    string UpstreamConductorphase = string.Empty;
                                   
                                    if (string.IsNullOrEmpty(sTransformerPhase))
                                    {

                                        //To Handle the Missing Phase get the Immidiate Upstream Conductor Phase
                                        sError = string.Empty;
                                        UpstreamConductorphase = GetSingleConductorPhaseFromDAPHIEwithClass(sPreviousGUID, Batch_Number, out SourceClass, sFeederID);
                                        if (string.IsNullOrEmpty(sError))
                                        {
                                            sTransformerPhase = UpstreamConductorphase;
                                        }
                                        else
                                        {
                                            sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_17.Split(':')[0] + sError;
                                            sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_17.Split(':')[1];

                                            Common.WriteLine_Error(sError + "," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                            Common.UpdateErrorAttribute(sError, sCurrentPhase, sClassName, Convert.ToInt32(sOID), string.Empty, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, SourceGUID, SourceClass);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //Just for Verification of DAPHIE Data
                                        if ((SourceGUID != sPreviousGUID) && (!string.IsNullOrEmpty(SourceGUID)) && (!string.IsNullOrEmpty(sPreviousGUID)))
                                        {
                                            Common.WriteLine_Error("DAPHIE SourceID and GIS Source ID for given Transformer is not Matched" + " for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                        }
                                    }
                                }
                                if ((string.IsNullOrEmpty(sTransformerPhase)) || sTransformerPhase == "0")
                                {
                                    if (LoadType == "BULK")
                                    {
                                        //sError = "Phase information for this asset is not available in JSON, tried deriving within GIS but phase prediction was not available for immediate conductor.";
                                        sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_10.Split(':')[0];
                                        sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_10.Split(':')[1];

                                        Common.WriteLine_Error(sError + " for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                        Common.UpdateErrorAttribute(sError, sCurrentPhase, sClassName, Convert.ToInt32(sOID), string.Empty, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, sPreviousGUID, SourceClass);
                                    }
                                }
                                else
                                {
                                    int iExistingIsThreePhaseOrOtherPhase = 0;
                                    if (ValidateTransformerPhase(sCurrentPhase, sTransformerPhase, ref iExistingIsThreePhaseOrOtherPhase) == true)
                                    {
                                        string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + ", " + ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sOID;
                                        DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                                        if (dt != null && dt.Rows.Count > 0)
                                        {
                                            string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();
                                            string sExistingStatus = dt.Rows[0].ItemArray[1].ToString();
                                            Common.UpdateAttribute(sExistingStatus, sCurrentPhase, sClassName, Convert.ToInt32(sOID), sTransformerPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, SourceGUID, SourceClass);
                                            if ((sExistingStatus.ToUpper() != ReadConfigurations.PhasingStatus.FieldVerified) && (sExistingStatus.ToUpper() != ReadConfigurations.PhasingStatus.OfficeVerified))
                                            {
                                                UpdateSecondaryNetwork.UpdateSecondaryDownStreamNetwork(sClass_ViewName, sOID, sFeederID, sTransformerPhase, sGUID, sSubType, Batch_Number, sCurrentPhase);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //1 - Current phase is Other Phase, 2 - Current phase is three phase
                                        if (iExistingIsThreePhaseOrOtherPhase == 1)
                                        {
                                            sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_20.Split(':')[0];
                                            sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_20.Split(':')[1];
                                        }
                                        else if (iExistingIsThreePhaseOrOtherPhase == 2)
                                        {
                                            sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_19.Split(':')[0];
                                            sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_19.Split(':')[1];
                                        }

                                        Common.WriteLine_Error(sError + " for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                        Common.UpdateErrorAttribute(sError, sCurrentPhase, sClassName, Convert.ToInt32(sOID), sTransformerPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, sPreviousGUID, SourceClass);
                                  
                                    }
                                }
                            }
                            #endregion
                        }
                        else if ((sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.PrimaryRiserViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.VOLTAGEREGULATORViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.STEPDOWNViewName) ||
                                    (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.TieViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.FAULTINDICATORViewName) ||
                             (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.PrimaryMeterViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.CapacitorBankViewName))
                        {
                            #region Other than intruppting devices
                            //Get Upstream Feature
                            DataRow tempDR = InputTraceResultDT.Rows[icount - 1];
                            string UpstreamFetureGUID = tempDR["TO_FEATURE_GLOBALID"].ToString();
                            //Get Downstream Feature
                            string DownstreamfeatureGUID = string.Empty;
                            if (icount + 1 < InputTraceResultDT.Rows.Count)
                            {
                                tempDR = InputTraceResultDT.Rows[icount + 1];
                                DownstreamfeatureGUID = tempDR["TO_FEATURE_GLOBALID"].ToString();
                            }
                            sError = string.Empty;
                            string valueToSearch = sOID;
                            string columnName = "FEATURE_OID";

                            //int count = (int)MainClass.g_dtAllDetails.Compute(string.Format("count({0})", columnName),
                            //        string.Format("{0} like '{1}'", columnName, valueToSearch));
                            int count = (int)MainClass.g_dtAllDetails.Compute("count(" + columnName + ")", columnName + "=" + valueToSearch);


                            if (count == 0)
                            {
                                //Update other then switch,Fuse,DPD,OpenPoint with Downstream Conductor
                                if (UpdateOtherThenSDOFDeviceForBulk(UpstreamFetureGUID, DownstreamfeatureGUID, sClassName, sClass_ViewName, sOID, sGUID, sFeederID, sFROMFeature_EID, sToFeature_EID, sSubType, sReviewBy, Batch_Number) == false)
                                {
                                    Common.WriteLine_Error("Other then SDOF Device Phase is  Not Updated for GUID," + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                                }
                            }
                            #endregion
                        }

                    }
                    if ((sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.PrimaryOHConductorViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.PrimaryUGConductorViewName) || (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.DistBusBarViewName))
                    {
                        PreviousClass = sClass_ViewName;
                        sPreviousGUID = sGUID;
                        PreviousOrderNum = order_num;
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while updating the Bulk network for FeederID," + sFeederID + ",Exception  :- " + ex.Message.ToString() + " at " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (ConductorTable != null) { ConductorTable = null; }
                if (TransformerTable != null) { TransformerTable = null; }
                if (PriOHDataTable != null) { PriOHDataTable = null; }
                if (PriUGDataTable != null) { PriUGDataTable = null; }
                if (BusBarDataTable != null) { BusBarDataTable = null; }
                if (EDGISTransformerDataTable != null) { EDGISTransformerDataTable = null; }
            }
        }

         

        #endregion

        # region Update Conductors

        private bool UpdatePrimaryConductors(string sConductorClass, string sConductorClassViewName, string sConductorOID, string sFeederID, string sFROMFeature_EID, string sDAPHIEPhase,
         string sSubType, string sReviewBy, string LoadType, string Batch_Number, string sExistingPhase,string sGUID)
        {
            bool _retval = false;
            try
            {
                
                //Update Primary Conductors
                string strquery = "select " + ReadConfigurations.GUIDFIELDNAME + "," + ReadConfigurations.PhasingVerifiedStatusFldName + ","
                    + ReadConfigurations.PhaseDesignationFieldName + "  from  " + sConductorClassViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "  = " + sConductorOID;
                DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                if (dt != null && dt.Rows.Count > 0)
                {
                     sGUID = dt.Rows[0].ItemArray[0].ToString();
                    string sPHASINGVERIFIEDSTATUS =dt.Rows[0].ItemArray[1].ToString();

                    Common.UpdateAttribute(sPHASINGVERIFIEDSTATUS, sExistingPhase, sConductorClass, Convert.ToInt32(sConductorOID), sDAPHIEPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, string.Empty, string.Empty);
                    _retval = true;
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while updating the Primary Conductors," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }
        #endregion

        #region Update Switch,DPD,FUSE,OPEN POINT

        private bool UpdateSDOFDeviceForBulk(string UpstreamConductorGUID, string DownstreamConductorGUID ,string sClass, string sClass_ViewName, string sDeviceOID, string sDeviceGUID, string sFeederID, string sToFeature_EID,
            string sFROMFeature_EID, string sSubType, string sReviewBy, string Batch_Number)
        {
            bool _retval = false;
            try
            {
                sError = string.Empty;
                string scurrentphase = GetFeatureFieldValuefromClass(sClass, sClass_ViewName, sDeviceOID, sDeviceGUID, sFeederID, sSubType, sReviewBy, ReadConfigurations.PhaseDesignationFieldName);
                string sourceclass = string.Empty;
                string SourceConductorPhase = GetSingleConductorPhaseFromDAPHIEwithClass(UpstreamConductorGUID, Batch_Number, out sourceclass, sFeederID);              
                if (!string.IsNullOrEmpty(sError))
                {
                    sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_11.Split(':')[0] + sError;
                    sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_11.Split(':')[1];

                    Common.WriteLine_Error(sError + "," + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);
                    Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, sourceclass);
                    return false;
                }               
                if (!string.IsNullOrEmpty(SourceConductorPhase))               
                {
                    string DownstreamConductorPhase = GetSingleConductorPhaseFromDAPHIE(DownstreamConductorGUID, Batch_Number, sFeederID);  // GetDownstreamFeaturePhaseDesignation(sDeviceOID, sFeederID, out DownstreamConductorGUID, sSubType, sReviewBy, Batch_Number,scurrentphase);
                    if (!string.IsNullOrEmpty(sError))
                    {
                        sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_16.Split(':')[0] + sError;
                        sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_16.Split(':')[1];

                        Common.WriteLine_Error(sError + "," + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);
                        Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                        return false;
                        
                    }  
                    if (!string.IsNullOrEmpty(DownstreamConductorPhase))
                    {
                        //EGIS-975 ME Q3 CHANGE : If DEVICE_PHASE_VALIDATE_BY_CONDUTOR_PHASELENGTH is set true then Device phase will be validated based on phase length
                        if (System.Configuration.ConfigurationManager.AppSettings["DEVICE_PHASE_VALIDATE_BY_CONDUTOR_PHASELENGTH"].ToString().ToUpper() == ("TRUE").ToUpper())
                        {
                            string devicePhaseTobeUpdated = ValidateDevicePhaseByConductorPhaseLength(SourceConductorPhase, DownstreamConductorPhase, scurrentphase, sClass_ViewName);
                            if (!string.IsNullOrEmpty(devicePhaseTobeUpdated))
                            {
                                //For device de-energization
                                if (devicePhaseTobeUpdated.ToUpper().Equals("NONE"))
                                {
                                    devicePhaseTobeUpdated = null;
                                }
                                string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + ", " + ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sDeviceOID;
                                DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();
                                    string sExistingStatus = dt.Rows[0].ItemArray[1].ToString();

                                    Common.UpdateAttribute(sExistingStatus, sExistingPhase, sClass, Convert.ToInt32(sDeviceOID), devicePhaseTobeUpdated, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                    _retval = true;
                                }
                                else
                                {

                                    //sError = "Feature not found in the database.";

                                    sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[0];
                                    sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[1];

                                    Common.WriteLine_Error(sError + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);
                                    Common.UpdateErrorAttribute(sError, string.Empty, sClass, Convert.ToInt32(sDeviceOID), devicePhaseTobeUpdated, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                    return false;
                                }
                            }
                            else
                            {
                                sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[0];
                                sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[1];
                                sError = sError + ": Immediate UpStream Conductor GUID = " + UpstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(SourceConductorPhase) + " :  Downstream GUID = " + DownstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(DownstreamConductorPhase);
                                Common.WriteLine_Error(sError + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);

                                Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                return false;
                            }
                        }
                        else
                        {
                            if (ValidateDevicePhase(SourceConductorPhase, DownstreamConductorPhase) == true)
                            {
                                string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + ", " + ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sDeviceOID;
                                DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();
                                    string sExistingStatus = dt.Rows[0].ItemArray[1].ToString();

                                    Common.UpdateAttribute(sExistingStatus, sExistingPhase, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                    _retval = true;
                                }
                                else
                                {

                                    //sError = "Feature not found in the database.";

                                    sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[0];
                                    sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[1];

                                    Common.WriteLine_Error(sError + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);
                                    Common.UpdateErrorAttribute(sError, string.Empty, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                    return false;
                                }
                            }
                            else
                            {
                                sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[0];
                                sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[1];
                                sError = sError + ": Immediate UpStream Conductor GUID = " + UpstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(SourceConductorPhase) + " :  Downstream GUID = " + DownstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(DownstreamConductorPhase);
                                Common.WriteLine_Error(sError + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);

                                Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + ", " + ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sDeviceOID;
                        DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();
                            string sExistingStatus = dt.Rows[0].ItemArray[1].ToString();

                            Common.UpdateAttribute(sExistingStatus, sExistingPhase, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                            _retval = true;
                        }
                        else
                        {
                            //sError = "Feature not found in the database.";
                            sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[0];
                            sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[1];

                            Common.WriteLine_Error(sError + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);
                            Common.UpdateErrorAttribute(sError, string.Empty, sClass, Convert.ToInt32(sDeviceOID), SourceConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                            return false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while updating the SDOF Device (Switch - DPD - OpenPoint - Fuse)," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }
        private string CheckNormalPosition(string sOID, string sClass_ViewName)
        {
            string _retval = "OPEN";
            try
            {
                string strquery = " select " + ReadConfigurations.NORMALPOSITION_FieldName + "," + ReadConfigurations.NORMALPOSITION_A_FieldName + "," +
                ReadConfigurations.NORMALPOSITION_B_FieldName + "," + ReadConfigurations.NORMALPOSITION_C_FieldName + "," + ReadConfigurations.PhaseDesignationFieldName + "," +
                ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where " + ReadConfigurations.OBJECTIDFIeldName + "= " + sOID;
                DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string Val_NormalPosition = dt.Rows[0].ItemArray[0].ToString();
                    string Val_A = dt.Rows[0].ItemArray[1].ToString();
                    string Val_B = dt.Rows[0].ItemArray[2].ToString();
                    string Val_C = dt.Rows[0].ItemArray[3].ToString();
                    string sOldPhase = dt.Rows[0].ItemArray[4].ToString();
                    string sExistingStatus = dt.Rows[0].ItemArray[5].ToString();

                    //For ABC Normal Position either close or open for all.

                    if ((Val_A == ReadConfigurations.NormalPosition.Close) || (Val_B == ReadConfigurations.NormalPosition.Close) || (Val_C == ReadConfigurations.NormalPosition.Close))
                    {
                        _retval = "CLOSE";

                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while updating the normal position of the feature," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }
        #endregion

        #region Update other than Switch,DPD,FUSE,OPEN POINT
        private bool UpdateOtherThenSDOFDeviceForBulk(string UpstreamConductorGUID,string DownstreamConductorGUID,string sClass, string sClass_ViewName, string sDeviceOID, string sDeviceGUID, string sFeederID, string sFROMFeature_EID,
          string sToFeature_EID, string sSubType, string sReviewBy, string Batch_Number)
        {
            bool _retval = false;
            try
            {
                string scurrentphase = GetFeatureFieldValuefromClass(sClass, sClass_ViewName, sDeviceOID, sDeviceGUID, sFeederID, sSubType, sReviewBy, ReadConfigurations.PhaseDesignationFieldName);
                sError = string.Empty;
                string SourceConductorPhase = GetSingleConductorPhaseFromDAPHIE(UpstreamConductorGUID, Batch_Number, sFeederID);

                if (!string.IsNullOrEmpty(sError))
                {
                    //sError = "Feature is not updated as erroneous immediate upstream conductor record is coming from DAPHIE and Error = " + sError;
                    sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_11.Split(':')[0] + sError;
                    sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_11.Split(':')[1];

                    Common.WriteLine_Error(sError + "," + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);
                    Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), string.Empty, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                    return false;
                }               
               
                if (!string.IsNullOrEmpty(SourceConductorPhase))
                {
                    sError = string.Empty;
                    string DownstreamConductorPhase = GetSingleConductorPhaseFromDAPHIE(DownstreamConductorGUID, Batch_Number, sFeederID);  // GetDownstreamFeaturePhaseDesignation(sDeviceOID, sFeederID, out DownstreamConductorGUID, sSubType, sReviewBy, Batch_Number,scurrentphase);
                    if (!string.IsNullOrEmpty(sError))
                    {
                        
                        sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_16.Split(':')[0] + sError;
                        sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_16.Split(':')[1];

                        Common.WriteLine_Error(sError + "," + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);
                        Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), string.Empty, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                        return false;
                    }
                    if (!string.IsNullOrEmpty(DownstreamConductorPhase))
                    {
                        //EGIS-975 ME Q3 CHANGE : If DEVICE_PHASE_VALIDATE_BY_CONDUTOR_PHASELENGTH is set true then Device phase will be validated based on phase length
                        if (System.Configuration.ConfigurationManager.AppSettings["DEVICE_PHASE_VALIDATE_BY_CONDUTOR_PHASELENGTH"].ToString().ToUpper() == ("TRUE").ToUpper())
                        {
                            string devicePhaseTobeUpdated = ValidateDevicePhaseByConductorPhaseLength(SourceConductorPhase, DownstreamConductorPhase, scurrentphase, sClass_ViewName);
                            if (!string.IsNullOrEmpty(devicePhaseTobeUpdated))
                            {
                                //For device de-energization
                                if (devicePhaseTobeUpdated.ToUpper().Equals("NONE"))
                                {
                                    devicePhaseTobeUpdated = null;
                                }
                                if (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.PrimaryRiserViewName)
                                {
                                    string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sDeviceOID;
                                    DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();


                                        Common.UpdateAttribute(string.Empty, sExistingPhase, sClass, Convert.ToInt32(sDeviceOID), devicePhaseTobeUpdated, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                        _retval = true;
                                    }
                                }
                                else
                                {
                                    string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + ", " + ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sDeviceOID;
                                    DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();
                                        string sExistingStatus = dt.Rows[0].ItemArray[1].ToString();

                                        Common.UpdateAttribute(sExistingStatus, sExistingPhase, sClass, Convert.ToInt32(sDeviceOID), devicePhaseTobeUpdated, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                        _retval = true;
                                    }
                                }
                            }
                            else
                            {
                                sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[0];
                                sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[1];
                                sError = sError + " : Immediate UpStream Conductor GUID = " + UpstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(SourceConductorPhase) + " :  Downstream GUID = " + DownstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(DownstreamConductorPhase);
                                Common.WriteLine_Error(sError + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);

                                Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), DownstreamConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                _retval = false;
                            }
                        }
                        else
                        {
                            //ValidateOperatingVoltage()
                            if (ValidateDevicePhase(SourceConductorPhase, DownstreamConductorPhase) == true)
                            {
                                if (sClass_ViewName.ToUpper().Trim() == ReadConfigurations.Conductors_ViewName.PrimaryRiserViewName)
                                {
                                    string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sDeviceOID;
                                    DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();


                                        Common.UpdateAttribute(string.Empty, sExistingPhase, sClass, Convert.ToInt32(sDeviceOID), DownstreamConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                        _retval = true;
                                    }
                                }
                                else
                                {
                                    string strquery = " select  " + ReadConfigurations.PhaseDesignationFieldName + ", " + ReadConfigurations.PhasingVerifiedStatusFldName + " from " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "= " + sDeviceOID;
                                    DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                                    if (dt != null && dt.Rows.Count > 0)
                                    {
                                        string sExistingPhase = dt.Rows[0].ItemArray[0].ToString();
                                        string sExistingStatus = dt.Rows[0].ItemArray[1].ToString();

                                        Common.UpdateAttribute(sExistingStatus, sExistingPhase, sClass, Convert.ToInt32(sDeviceOID), DownstreamConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                        _retval = true;
                                    }
                                }
                            }
                            else
                            {
                                sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[0];
                                sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_14.Split(':')[1];
                                sError = sError + " : Immediate UpStream Conductor GUID = " + UpstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(SourceConductorPhase) + " :  Downstream GUID = " + DownstreamConductorGUID + " and Phase = " + Common.GetPhaseDomainValue(DownstreamConductorPhase);
                                Common.WriteLine_Error(sError + sDeviceGUID + ",Class," + sClass + ",FeederID," + sFeederID);

                                Common.UpdateErrorAttribute(sError, scurrentphase, sClass, Convert.ToInt32(sDeviceOID), DownstreamConductorPhase, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sDeviceGUID, sSubType, sReviewBy, UpstreamConductorGUID, string.Empty);
                                _retval = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while updating  other than SDOF Device (Switch - DPD - OpenPoint - Fuse)," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }
     
        #endregion

        #region Validation Functions
        private bool ValidateTransformerPhase(string sCurrentPhase, string sPredictedPhase, ref int iExistingIsThreePhaseOrOtherPhase)
        {
            bool _retval = true;
            try
            {
                if ((sCurrentPhase != "7") && (sPredictedPhase == "7"))
                {
                    iExistingIsThreePhaseOrOtherPhase = 1;
                    _retval = false;
                }
                else if ((sCurrentPhase == "7") && (sPredictedPhase != "7"))
                {
                    iExistingIsThreePhaseOrOtherPhase = 2;
                    _retval = false;
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while validating the PhaseDesignation of Device Feature," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }
        private bool ValidateDevicePhase(string sourceConductorPhase, string DownstreamConductorPhase)
        {
            bool _retval = true;
            try
            {

                if (((sourceConductorPhase == "4") && (DownstreamConductorPhase != "4")) ||
                    ((sourceConductorPhase == "2") && (DownstreamConductorPhase != "2")) ||
                    ((sourceConductorPhase == "1") && (DownstreamConductorPhase != "1")) ||
                    ((sourceConductorPhase == "6") && ((DownstreamConductorPhase != "6") && (DownstreamConductorPhase != "4") && (DownstreamConductorPhase != "2"))) ||
                    ((sourceConductorPhase == "5") && ((DownstreamConductorPhase != "5") && (DownstreamConductorPhase != "4") && (DownstreamConductorPhase != "1"))) ||
                    ((sourceConductorPhase == "3") && ((DownstreamConductorPhase != "3") && (DownstreamConductorPhase != "2") && (DownstreamConductorPhase != "1"))))
                {
                    _retval = false;
                }

            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while validating the PhaseDesignation of Device Feature," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }

        //EGIS-975 : VALIDATE DEVICE PHASE BASED ON DEVICE EXISTING PHASE, UPSTREAM & DOWNSTREAM CONDUCTOR PHASE
        private string ValidateDevicePhaseByConductorPhaseLength(string sourceConductorPhase, string DownstreamConductorPhase, string currentPhase, string deviceName)
        {
            bool _retval = true;
            string devicePhaseToBeUpdated = string.Empty;
            int sourceCondPhaseLength = getLenOfPhase(sourceConductorPhase);
            int DownstreamCondPhaseLength = getLenOfPhase(DownstreamConductorPhase);
            int currentPhaseLength = getLenOfPhase(currentPhase);
            try
            {
                if ((sourceCondPhaseLength == currentPhaseLength) && (DownstreamCondPhaseLength == currentPhaseLength))
                {
                    devicePhaseToBeUpdated = sourceConductorPhase;
                }
                else if ((sourceCondPhaseLength > currentPhaseLength) && (DownstreamCondPhaseLength == currentPhaseLength))
                {
                    devicePhaseToBeUpdated = DownstreamConductorPhase;
                }
                else if ((sourceCondPhaseLength == currentPhaseLength) && (DownstreamCondPhaseLength < currentPhaseLength))
                {
                    devicePhaseToBeUpdated = sourceConductorPhase;
                }
                else if ((sourceCondPhaseLength > currentPhaseLength) && (DownstreamCondPhaseLength > currentPhaseLength))
                {
                    //For Voltage Regulators, do not update device phasing - valid condition; for Non-Voltage Regulators, report for manual QA-QC
                    if (deviceName.ToUpper().Trim() == ReadConfigurations.Devices_ViewName.VOLTAGEREGULATORViewName)
                    {
                        //do not update device phasing -> so assigning currentPhase
                        devicePhaseToBeUpdated = currentPhase;
                    }
                    else
                    {
                        //de-energize this device -> energized phase should be Null
                        devicePhaseToBeUpdated = "None";
                    }
                }
                else if ((sourceCondPhaseLength < currentPhaseLength))
                {
                    //De-Energization to be reported for manual QA-QC
                    //de-energize this device -> energized phase should be Null
                    devicePhaseToBeUpdated = "None";
                }
                else if ((sourceCondPhaseLength == currentPhaseLength) && (DownstreamCondPhaseLength > currentPhaseLength))
                {
                    //De-Energization to be reported for manual QA-QC
                    //de-energize this device -> energized phase should be Null
                    devicePhaseToBeUpdated = "None";
                }

            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while validating the PhaseDesignation of Device Feature based on Phase Length," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return devicePhaseToBeUpdated;
        }
        //EGIS-975 : GET PHASE LENGTH BY PHASE CODE
        private int getLenOfPhase(string ConductorPhase)
        {
            int LenOfPhase = 0;
            if (ConductorPhase == "4") { LenOfPhase = 1; }
            if (ConductorPhase == "2") { LenOfPhase = 1; }
            if (ConductorPhase == "1") { LenOfPhase = 1; }
            if (ConductorPhase == "6") { LenOfPhase = 2; }
            if (ConductorPhase == "3") { LenOfPhase = 2; }
            if (ConductorPhase == "5") { LenOfPhase = 2; }
            if (ConductorPhase == "7") { LenOfPhase = 3; }
            return LenOfPhase;
        }
        # endregion

        #region Get Phase/Subtype from DAPHIE/ViewTable 
        private string GetTransformerPhaseFromDAPHIEAlongWithSource(string sGUID, string sFeederID, string Batch_Number, out string SourceGUID)
        {
            string _returnPhase = string.Empty;
            SourceGUID = string.Empty;
            try
            {

                string strquery = "GLOBALID = '" + sGUID + "' and BATCHID='" + Batch_Number + "' and CIRCUITID = '" + sFeederID + "'";
                DataRow[] drd = TransformerTable.Select(strquery);

                if (drd != null)
                {
                    if (drd.Length > 0)
                    {
                        DataRow dr = drd[0];
                        sError = dr["ERROR_DESCRIPTION"].ToString();
                        if (string.IsNullOrEmpty(sError))
                        {
                            _returnPhase = dr["PHASE_PREDICTION"].ToString();
                       
                            SourceGUID = dr["PRICONDUCTORGUID"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while getting the transformer phase from the DAPHIE," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _returnPhase;
        }
        private string GetSingleConductorPhaseFromDAPHIE(string sGUID, string Batch_Number, string sFeederID)
        {
            string _returnPhase = string.Empty;

            try
            {
                string strquery = "GLOBALID = '" + sGUID + "' and BATCHID='" + Batch_Number + "' and CIRCUITID = '" + sFeederID + "'";
                DataRow[] drd = ConductorTable.Select(strquery);

                if (drd != null)
                {
                    if (drd.Length > 0)
                    {
                        for (int i = 0; i < drd.Length; i++)
                        {
                            DataRow dr = drd[0];
                            _returnPhase = dr["PHASE_PREDICTION"].ToString();
                            sError = dr["ERROR_DESCRIPTION"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while getting  the PhaseDesignation of feature from DAPHIE Data," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _returnPhase;
        }

        private string GetSingleConductorPhaseFromDAPHIEwithClass(string sGUID, string Batch_Number, out string SourceClass, string sFeederID)
        {
            string _returnPhase = string.Empty;
            SourceClass = string.Empty;
            try
            {

                string strquery = "GLOBALID = '" + sGUID + "' and BATCHID='" + Batch_Number + "' and CIRCUITID = '" + sFeederID + "'";
                DataRow[] drd = ConductorTable.Select(strquery);

                if (drd != null)
                {
                    if (drd.Length > 0)
                    {
                        DataRow dr = drd[0];
                        _returnPhase = dr["PHASE_PREDICTION"].ToString();
                        sError = dr["ERROR_DESCRIPTION"].ToString();
                        SourceClass = dr["FEATURE_CLASS"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while getting  the PhaseDesignation of feature from DAPHIE Data," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _returnPhase;
        }


        private string GetFeatureFieldValuefromClass(string sClassName, string sClass_ViewName, string sOID, string sGUID, string sFeederID, string sSubType, string sReviewBy,string sField)
        {
            string _retval = string.Empty;
            DataRow dr = null;
            try
            {
                string strquery = ReadConfigurations.OBJECTIDFIeldName + "  = " + sOID;       
                DataRow[] drd=null;
                if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Conductors_ViewName.PrimaryOHConductorViewName.Trim().ToUpper())
                {
                    drd = PriOHDataTable.Select(strquery);
                    if (drd.Length != 0) 
                    { dr = drd[0];
                    _retval = dr[sField].ToString();
                    }
                }
                else if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Conductors_ViewName.PrimaryUGConductorViewName.Trim().ToUpper())
                {
                    drd = PriUGDataTable.Select(strquery);
                    if (drd.Length != 0) 
                    { dr = drd[0];
                    _retval = dr[sField].ToString();
                    }
                }
                else if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Conductors_ViewName.DistBusBarViewName.Trim().ToUpper())
                {
                    drd =BusBarDataTable.Select(strquery);
                    if (drd.Length != 0)
                    {    dr = drd[0];

                    _retval = dr[sField].ToString();
                    }
                }
                else if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Devices_ViewName.TransformerViewName.Trim().ToUpper())
                {
                    drd = EDGISTransformerDataTable.Select(strquery);
                    if (drd.Length != 0) 
                    { dr = drd[0];
                    _retval = dr[sField].ToString();
                    }
                }
                if (string.IsNullOrEmpty(_retval))
                {
                    strquery = "select  " + sField + "  from  " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "  = " + sOID;
                    DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        _retval = dt.Rows[0].ItemArray[0].ToString();
                    }
                    else
                    {
                        //sError = "Feature not found in database";
                        sError = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[0];
                        sReviewBy = ReadConfigurations.ERROR_MESSAGES.ERROR_CODE_09.Split(':')[1];

                        Common.WriteLine_Error(sError + sGUID + ",Class," + sClassName + ",FeederID," + sFeederID);
                        Common.UpdateErrorAttribute(sError, string.Empty, sClassName, Convert.ToInt32(sOID), string.Empty, sFeederID, ReadConfigurations.ObjectClassType.FeatureClassType, sGUID, sSubType, sReviewBy, string.Empty, string.Empty);
                
                    }
                }           
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Unhandled exception encountered while getting the feature PhaseDesignation from featureclass," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }

       
        internal string GetSubtype(string sPrimaryConductorOID, string sClass_ViewName)
        {
            string _retval = null;
            DataRow dr = null;
            try
            {
                string strquery = ReadConfigurations.OBJECTIDFIeldName + "  = " + sPrimaryConductorOID;
                DataRow[] drd = null;
                if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Conductors_ViewName.PrimaryOHConductorViewName.Trim().ToUpper())
                {
                    drd = PriOHDataTable.Select(strquery);                   
                }
                else if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Conductors_ViewName.PrimaryUGConductorViewName.Trim().ToUpper())
                {
                    drd = PriUGDataTable.Select(strquery);                  
                }
                else if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Conductors_ViewName.DistBusBarViewName.Trim().ToUpper())
                {
                    drd = BusBarDataTable.Select(strquery);                    
                }
                else if (sClass_ViewName.Trim().ToUpper() == ReadConfigurations.Devices_ViewName.TransformerViewName.Trim().ToUpper())
                {
                    drd = EDGISTransformerDataTable.Select(strquery);

                }
                
                if (drd.Length != 0)
                {
                    dr = drd[0];
                    _retval = dr["SUBTYPECD"].ToString();
                }
                if (string.IsNullOrEmpty(_retval))
                {

                    strquery = "select SUBTYPECD  from  " + sClass_ViewName + " where  " + ReadConfigurations.OBJECTIDFIeldName + "  = " + sPrimaryConductorOID;
                    DataTable dt = DBHelper.GetDataTableByQuery(strquery);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        _retval = dt.Rows[0].ItemArray[0].ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                _retval = string.Empty;
                Common.WriteLine_Error("Unhandled exception encountered while getting the feature PhaseDesignation from featureclass," + ex.Message.ToString() + " at " + ex.StackTrace);
            }
            return _retval;
        }

        #endregion
    }
}
