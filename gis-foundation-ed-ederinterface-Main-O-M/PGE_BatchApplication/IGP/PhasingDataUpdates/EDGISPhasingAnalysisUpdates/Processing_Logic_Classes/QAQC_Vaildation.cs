using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase.Network;
using PGE.BatchApplication.IGPPhaseUpdate.Validation_Rules;

namespace PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes
{
    class QAQC_Vaildation
    {
        private static bool m_blFeatNotCompleted = false;
        private static string m_sEnergizedPhase = null;
        private static string m_GuidIds_Issue = null;
        private static string m_sObjectId_Correct = null;
        private static string m_sObjectId_LastIncorrect = null;
        private static int m_iParentCounter = 0;
        private static int m_iCase = 0;
        private static int m_iOrderNum = 0;
        public static Hashtable m_pHT_All_FeatCls = new Hashtable();
        private static Hashtable m_pHT_Msg_Case = new Hashtable();
        private DBHelper m_DBHelperClass = new DBHelper();
        Common CommonFuntions = new Common();
        PGE_Validate_Source_Connectivity checkvalidity = new PGE_Validate_Source_Connectivity();
        private static string[] m_sAll_PrimaryClasses = null;
        private static bool m_blTraceTillStitchPoint = false;

        public bool Update_QAQC_Validation(IFeatureWorkspace pFeatureWorkspace)
        {
            DataRow[] pRows = null;
            IFeatureClass pFeatClass = null;
            IFeature pFeature = null;
            string sFeatClsName = null;
            int iObjectId = 0;
            string sCircuitId = null;
            string sGuid = null;
            //DataTable pDT_Temp = new DataTable();
            int iCount = 0;
            string sVersionName = null;
            string sWhereClause = null;
            
            try
            {
                m_iCase = 0;
                sVersionName = ((IVersion)pFeatureWorkspace).VersionName.Split('.')[1];
                m_sAll_PrimaryClasses = ReadConfigurations.PrimaryFeatureClasses.Split(',');
                //pDT_Temp = MainClass.g_DT_QAQC;

                foreach (DataRow pRow in MainClass.g_DT_QAQC.Rows)
                {
                    m_blFeatNotCompleted = true;
                    m_sEnergizedPhase = null;
                    m_GuidIds_Issue = null;
                    m_sObjectId_Correct = null;
                    m_sObjectId_LastIncorrect = null;
                    iObjectId = 0;
                    m_iParentCounter = 0;
                    m_iOrderNum = 0;
                    sCircuitId = null;
                    sGuid = null;
                    pRows = null;
                    sWhereClause = null;
                    m_blTraceTillStitchPoint = false;
                    iCount++;

                    sGuid = pRow["FEATURE_GUID"].ToString();

                    sWhereClause = "FEATURE_GUID" + " = '" + sGuid + "' AND COMMENTS = 'NA'";

                    pRows = MainClass.g_DT_QAQC.Select(sWhereClause);
                    if (pRows.Count() == 0)
                    {
                        continue;
                    }
                    

                    sFeatClsName = pRow["NAME"].ToString();
                    iObjectId = Convert.ToInt32(pRow["FEATURE_OID"].ToString());
                    sCircuitId = pRow["CIRCUITID"].ToString();

                    //temp
                    if (sFeatClsName.Contains("IGPEDITOR.") == true)
                    {
                        sFeatClsName = sFeatClsName.Replace("IGPEDITOR.", "EDGIS.");
                    }

                    //CommonFuntions.WriteLine_Info("sGuid = " + sGuid + ",sFeatClsName = " + sFeatClsName);

                    if (m_pHT_All_FeatCls.Contains(sFeatClsName) == false)
                    {
                        pFeatClass = pFeatureWorkspace.OpenFeatureClass(sFeatClsName);
                        m_pHT_All_FeatCls.Add(sFeatClsName, pFeatClass);
                    }
                    else
                    {
                        pFeatClass = (IFeatureClass)m_pHT_All_FeatCls[sFeatClsName];
                    }

                    pFeature = pFeatClass.GetFeature(iObjectId);

                    if (pFeature != null)
                    {
                        m_GuidIds_Issue = "'" + sGuid + "'";
                        string EnergizedPhase = CheckDeEnerzige(pFeature, sCircuitId);
                        if (!String.IsNullOrEmpty(EnergizedPhase) && EnergizedPhase == "None")
                        {
                            m_sObjectId_LastIncorrect = pFeature.OID.ToString() + "," + sFeatClsName;
                            string pEnergizedPhase = checkParentPhase(pRow["CIRCUITID"].ToString(), pFeature.OID.ToString(), pFeatureWorkspace, sFeatClsName);

                            UpdateQAQCTable(sVersionName);


                        }

                       
                    }

                }

                return true;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }

        private string checkParentPhase(string sCircuitid, string sTo_Feature_Oid, IFeatureWorkspace pFeatureWorkspace, string sFeatClsName)
        {
            PGE_Validate_Source_Connectivity checkvalidity = new PGE_Validate_Source_Connectivity();
            string sQuery = string.Empty;
            string EnergizedPhase_P = string.Empty;
            string sFeatClassName_Parent = null;
            DataTable pDataTable = new DataTable();
            IFeatureClass pFeatClass_Parent = null;
            IFeature pFeature = null;
            int iObjectId = 0;
            
            string sGuid = null;
            int iRow = 0;
            try
            {
                if (m_blFeatNotCompleted == false)
                {
                    return string.Empty;
                }
                m_iParentCounter++;
                if (m_iParentCounter == 1000)
                {
                    return string.Empty ;
                }

                try
                {
                    sQuery = "Select a.ORDER_NUM,a.FROM_FEATURE_EID,a.TO_FEATURE_EID, a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH, " +
                        "a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from " + ReadConfigurations.FeederNetworkTraceTableName + " a left join " + ReadConfigurations.GDBITEMSTableName + " b on a.to_feature_fcid = b.objectid where " +
                        "(a.FEEDERID = '" + sCircuitid + "' OR a.FEEDERFEDBY = '" + sCircuitid + "') and a.to_Feature_EID= (select FROM_FEATURE_EID from " +
                         ReadConfigurations.FeederNetworkTraceTableName + " c left join " + ReadConfigurations.GDBITEMSTableName + " d on c.to_feature_fcid = d.objectid where c.TO_FEATURE_OID=" + sTo_Feature_Oid +
                        " and (c.FEEDERID = '" + sCircuitid + "' OR c.FEEDERFEDBY = '" + sCircuitid + "') and d.physicalname = '" + sFeatClsName.ToUpper() + "')";

                    pDataTable = m_DBHelperClass.GetDataTable_QAQC(sQuery);

                    if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                    {
                        //muliple rows in Open point case
                        if ((sFeatClsName.ToUpper() == ReadConfigurations.Devices.OPENPOINTClassName) && (m_iOrderNum > 0))
                        {
                            sQuery = "Select a.ORDER_NUM,a.FROM_FEATURE_EID,a.TO_FEATURE_EID, a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH, " +
                                "a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from " + ReadConfigurations.FeederNetworkTraceTableName + " a left join " + ReadConfigurations.GDBITEMSTableName + " b on a.to_feature_fcid = b.objectid where " +
                                "(a.FEEDERID = '" + sCircuitid + "' OR a.FEEDERFEDBY = '" + sCircuitid + "') and a.to_Feature_EID= (select FROM_FEATURE_EID from " +
                                 ReadConfigurations.FeederNetworkTraceTableName + " c left join " + ReadConfigurations.GDBITEMSTableName + " d on c.to_feature_fcid = d.objectid where c.TO_FEATURE_OID=" + sTo_Feature_Oid +
                                " and (c.FEEDERID = '" + sCircuitid + "' OR c.FEEDERFEDBY = '" + sCircuitid + "') and d.physicalname = '" + sFeatClsName.ToUpper() + "' AND c.ORDER_NUM = " + m_iOrderNum.ToString() + ")";

                            pDataTable = m_DBHelperClass.GetDataTable_QAQC(sQuery);

                            if ((pDataTable == null) || (pDataTable.Rows.Count == 0))
                            {
                                return string.Empty;
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                            
                    }
                  
                }
                catch 
                {
                    return string.Empty;
                }

                iRow = 0;
                if (pDataTable.Rows.Count > 1)
                {
                    iRow = ValidateRow(pDataTable, sFeatClsName);
                }


                sFeatClassName_Parent = pDataTable.Rows[iRow]["PHYSICALNAME"].ToString();
                iObjectId = Convert.ToInt32(pDataTable.Rows[iRow]["TO_FEATURE_OID"].ToString());
                m_iOrderNum = Convert.ToInt32(pDataTable.Rows[iRow]["ORDER_NUM"].ToString());
                sGuid = pDataTable.Rows[iRow]["TO_FEATURE_GLOBALID"].ToString();

                if (sFeatClassName_Parent == "EDGIS.ELECTRICSTITCHPOINT")
                {
                    m_blTraceTillStitchPoint = true;
                    return string.Empty;
                }
                if (m_pHT_All_FeatCls.Contains(sFeatClassName_Parent) == false)
                {
                    pFeatClass_Parent = pFeatureWorkspace.OpenFeatureClass(sFeatClassName_Parent);
                    m_pHT_All_FeatCls.Add(sFeatClassName_Parent, pFeatClass_Parent);
                }
                else
                {
                    pFeatClass_Parent = (IFeatureClass)m_pHT_All_FeatCls[sFeatClassName_Parent];
                }

                pFeature = pFeatClass_Parent.GetFeature(iObjectId);
                if (pFeature != null)
                {
                    EnergizedPhase_P = CheckDeEnerzige(pFeature, sCircuitid);
                    if (EnergizedPhase_P == "None")
                    {
                        if (sFeatClassName_Parent.ToUpper().Contains("JUNCTIONS") == false)
                        {
                            m_GuidIds_Issue = m_GuidIds_Issue + ",'" + sGuid + "'";
                            m_sObjectId_LastIncorrect = pFeature.OID.ToString() + "," + sFeatClassName_Parent;
                        }


                        string e_Phase = checkParentPhase(sCircuitid, pFeature.OID.ToString(), pFeatureWorkspace, sFeatClassName_Parent);

                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(EnergizedPhase_P))
                        {
                            if (sFeatClassName_Parent.ToUpper().Contains("JUNCTIONS") == true)
                            {
                                string e_Phase = checkParentPhase(sCircuitid, pFeature.OID.ToString(), pFeatureWorkspace, sFeatClassName_Parent);
                            }
                            else
                            {
                                m_blFeatNotCompleted = false;
                                m_sEnergizedPhase = EnergizedPhase_P;
                                m_sObjectId_Correct = pFeature.OID.ToString() + "," + sFeatClassName_Parent;
                                return m_sEnergizedPhase;
                            }

                        }

                    }
                }
                return EnergizedPhase_P;
               
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Error in Function checkParentPhase: " + ex.Message);
                //_log.Error("Error in Function checkParentPhase: " + ex.Message);
                return EnergizedPhase_P;
            }
          
        }
        internal string CheckDeEnerzige(IFeature pFeature, string sCircuitID)
        {


            try
            {
                IFeederInfo<FeatureKey> feederInfo = CommonFuntions.GetFeederInfo(pFeature as IRow);
                string EnergizedPhase = feederInfo.EnergizedPhases.ToString();
                return EnergizedPhase;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                //_log.Error(ex.Message + " at " + ex.StackTrace);
                return null;
            }
        }
        private int ValidateRow(DataTable pDataTable, string sFeatClsName)
        {
            string sFeatClassName_Parent = null;
            int iRow = 0;
            try
            {
                if (m_sAll_PrimaryClasses.Contains(sFeatClsName.ToUpper()))
                {
                    for (int i = 0; i < pDataTable.Rows.Count; ++i)
                    {
                        sFeatClassName_Parent = pDataTable.Rows[i]["PHYSICALNAME"].ToString().ToUpper();

                        //if same feature class, then skip
                        if (sFeatClassName_Parent == sFeatClsName)
                        {
                            continue;
                        }
                        //if another primary feature class, then return row number
                        if (m_sAll_PrimaryClasses.Contains(sFeatClassName_Parent))
                        {
                            return i;
                        }
                    }
                }
                    //skip for common feature classes, which lie in both Primary Network, Secondary Network
                else if ((sFeatClsName == ReadConfigurations.Devices.OPENPOINTClassName) || (sFeatClsName == ReadConfigurations.Devices.TieClassName) || 
                    (sFeatClsName.ToUpper().Contains("JUNCTIONS") == true))
                {
                }
                else if (MainClass.m_pHT_All_FeatCls_DownStream.Contains(sFeatClsName) == true)
                {
                    for (int i = 0; i < pDataTable.Rows.Count; ++i)
                    {
                        sFeatClassName_Parent = pDataTable.Rows[i]["PHYSICALNAME"].ToString().ToUpper();

                        //if same feature class, then skip
                        if (sFeatClassName_Parent == sFeatClsName)
                        {
                            continue;
                        }

                        //if parent feat class is transformer and child are sec ug, sec oh or treansformer lead, then return row number
                        if (sFeatClassName_Parent == ReadConfigurations.Devices.TransformerClassName)
                        {
                            if ((sFeatClsName == ReadConfigurations.Conductors.SecondaryUGConductorClassName) || (sFeatClsName == ReadConfigurations.Conductors.SecondaryOHConductorClassName)
                             || (sFeatClsName == ReadConfigurations.Conductors.TransformerLeadClassName))
                            {
                                return i;
                            }
                            else
                            {
                                continue;
                            }
                        }
                        else
                        {
                            //if primary feature class, then its not correct
                            if (m_sAll_PrimaryClasses.Contains(sFeatClassName_Parent))
                            {
                                continue;
                            }
                            else if (MainClass.m_pHT_All_FeatCls_DownStream.Contains(sFeatClsName) == true)
                            {
                                return i;
                            }
                        }

                       
                    }
                }
               
                return iRow;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error("Error in Function ValidateRow: " + ex.Message);
                return iRow;
            }
        }

  

        public bool UpdateQAQCTable(string sVersionName)
        {
            string sComments = null;
            string sCase = null;
            DataRow[] pRows = null;
            try
            {
                if (m_blTraceTillStitchPoint == true)
                {
                    sComments = "De-Energized till ELECTRICSTITCHPOINT";
                }
                else
                {
                    sComments = GetErrorMsg();
                }
                

                if (string.IsNullOrEmpty(sComments) == true)
                {
                    return false;
                }

                if (m_pHT_Msg_Case.Contains(sComments) == false)
                {
                    m_iCase++;
                    sCase = "CASE_" + m_iCase.ToString();
                    m_pHT_Msg_Case.Add(sComments, sCase);
                }
                else
                {
                    sCase = m_pHT_Msg_Case[sComments].ToString();
                }

                pRows = MainClass.g_DT_QAQC.Select("FEATURE_GUID" + " IN (" + m_GuidIds_Issue + ")");
                if (pRows.Count() > 0)
                {
                    foreach (DataRow pRow in pRows)
                    {
                        pRow["COMMENTS"] = sComments;
                        pRow["DE_ENERGIZE_TYPE"] = sCase;
                        pRow["VERSION_NAME"] = sVersionName;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }

        public string GetErrorMsg()
        {
            string sErrorMsg = null;
            int iObjectId_Correct = 0;
            string sClass_Correct = null;
            int iObjectId_InCorrect = 0;
            string sClass_INCorrect = null;

            IFeatureClass pFeatCls_Correct = null;
            IFeatureClass pFeatCls_InCorrect = null;
            IFeature pFeature_Correct = null;
            IFeature pFeature_InCorrect = null;

            string sPhaseDesignation_Correct = null;
            string sPhaseDesignation_InCorrect = null;
            try
            {
                if (string.IsNullOrEmpty(m_sObjectId_Correct))
                {
                    return null;
                }
                iObjectId_Correct = Convert.ToInt32(m_sObjectId_Correct.Split(',')[0]);
                sClass_Correct = m_sObjectId_Correct.Split(',')[1];
                iObjectId_InCorrect = Convert.ToInt32(m_sObjectId_LastIncorrect.Split(',')[0]);
                sClass_INCorrect = m_sObjectId_LastIncorrect.Split(',')[1];

                pFeatCls_Correct = (IFeatureClass)m_pHT_All_FeatCls[sClass_Correct];
                pFeatCls_InCorrect = (IFeatureClass)m_pHT_All_FeatCls[sClass_INCorrect];

                pFeature_Correct = pFeatCls_Correct.GetFeature(iObjectId_Correct);
                pFeature_InCorrect = pFeatCls_InCorrect.GetFeature(iObjectId_InCorrect);

                sPhaseDesignation_Correct = pFeature_Correct.get_Value(pFeature_Correct.Fields.FindField("PHASEDESIGNATION")).ToString();
                sPhaseDesignation_InCorrect = pFeature_InCorrect.get_Value(pFeature_InCorrect.Fields.FindField("PHASEDESIGNATION")).ToString();

                if (ValidatePhase(sPhaseDesignation_Correct, sPhaseDesignation_InCorrect) == false)
                {
                    //phase mismatch
                    sErrorMsg = "Phase Mismatch between " + m_sObjectId_Correct + " and " + m_sObjectId_LastIncorrect;
                }
                else
                {
                    if ((sClass_INCorrect.ToUpper().Trim() == "EDGIS.SWITCH") || (sClass_INCorrect.ToUpper().Trim() == "EDGIS.FUSE") ||
                        (sClass_INCorrect.ToUpper().Trim() == "EDGIS.OPENPOINT") || (sClass_INCorrect.ToUpper().Trim() == "EDGIS.DYNAMICPROTECTIVEDEVICE"))
                    {
                        if (CheckNormalPositionError(pFeature_InCorrect, sPhaseDesignation_InCorrect) == true)
                        {
                            sErrorMsg = "Normal Position Error in " + m_sObjectId_LastIncorrect;
                        }
                        else
                        {
                            sErrorMsg = "Last Energized Feature = " + m_sObjectId_Correct + ", First De-Energized Feature = " + m_sObjectId_LastIncorrect;
                        }
                    }
                    else
                    {
                        sErrorMsg = "Last Energized Feature = " + m_sObjectId_Correct + ", First De-Energized Feature = " + m_sObjectId_LastIncorrect;
                    }

                }

                if ((m_sObjectId_Correct.Contains("11638917") && m_sObjectId_LastIncorrect.Contains("4509048")) || (m_sObjectId_Correct.Contains("6755106") && m_sObjectId_LastIncorrect.Contains("4585709"))
                    || (m_sObjectId_Correct.Contains("11765100") && m_sObjectId_LastIncorrect.Contains("6887050")))
                {
                    int i = 0;
                }

                return sErrorMsg;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(e.Message);
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return sErrorMsg;
            }
        }

        private bool CheckNormalPositionError(IFeature pFeature, string sFeaturePhaseDesignation)
        {
            string sNormalPosition_A = null;
            string sNormalPosition_B = null;
            string sNormalPosition_C = null;
            string sNormalPosition = null;
            string Open = "0";
            string Close = "1";
            string NotApplicable = "2";
            try
            {
                int idx_A = pFeature.Fields.FindField("NORMALPOSITION_A");
                int idx_B = pFeature.Fields.FindField("NORMALPOSITION_B");
                int idx_C = pFeature.Fields.FindField("NORMALPOSITION_C");
                int idx_NormalPosition = pFeature.Fields.FindField("NORMALPOSITION");
                switch (sFeaturePhaseDesignation)
                {

                    case "7":
                        //For ABC Normal Position either close or open for all.

                        sNormalPosition_A = pFeature.get_Value(idx_A).ToString();
                        sNormalPosition_B = pFeature.get_Value(idx_B).ToString();
                        sNormalPosition_C = pFeature.get_Value(idx_C).ToString();
                        sNormalPosition = pFeature.get_Value(idx_NormalPosition).ToString();

                        if ((sNormalPosition_A == Open) && (sNormalPosition_B == Open) && (sNormalPosition_C == Open) && (sNormalPosition == Open))
                        {
                        }
                        else if ((sNormalPosition_A == Close) && (sNormalPosition_B == Close) && (sNormalPosition_C == Close) && (sNormalPosition == Close))
                        {
                        }
                        else
                        {
                            //errror
                            return true;
                        }


                        break;
                    case "6":
                        //For AB Normal Position A and B either close or open and C is Not Applicable.
                        if ((sNormalPosition_A == Open) && (sNormalPosition_B == Open) && (sNormalPosition_C == NotApplicable) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else if ((sNormalPosition_A == Close) && (sNormalPosition_B == Close) && (sNormalPosition_C == NotApplicable) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else
                        {
                            //errror
                            return true;
                        }


                        break;
                    case "5":
                        //For AC Normal Position A and C either close or open and B is Not Applicable.
                        if ((sNormalPosition_A == Open) && (sNormalPosition_B == NotApplicable) && (sNormalPosition_C == Open) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else if ((sNormalPosition_A == Close) && (sNormalPosition_B == NotApplicable) && (sNormalPosition_C == Close) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else
                        {
                            //errror
                            return true;
                        }

                        break;
                    case "4":
                        //For A Normal Position A is either close or open and B/C is Not Applicable.
                        if ((sNormalPosition_A == Open) && (sNormalPosition_B == NotApplicable) && (sNormalPosition_C == NotApplicable) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else if ((sNormalPosition_A == Close) && (sNormalPosition_B == NotApplicable) && (sNormalPosition_C == NotApplicable) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else
                        {
                            //errror
                            return true;
                        }

                        break;
                    case "3":
                        //For BC Normal Position B and C either close or open and A is Not Applicable.
                        if ((sNormalPosition_A == NotApplicable) && (sNormalPosition_B == Open) && (sNormalPosition_C == Open) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else if ((sNormalPosition_A == NotApplicable) && (sNormalPosition_B == Close) && (sNormalPosition_C == Close) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else
                        {
                            //errror
                            return true;
                        }

                        break;
                    case "2":
                        //For B Normal Position B is either close or open and A/C is Not Applicable.
                        if ((sNormalPosition_A == NotApplicable) && (sNormalPosition_B == Open) && (sNormalPosition_C == NotApplicable) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else if ((sNormalPosition_A == NotApplicable) && (sNormalPosition_B == Close) && (sNormalPosition_C == NotApplicable) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else
                        {
                            //errror
                            return true;
                        }

                        break;
                    case "1":
                        //For C Normal Position C is either close or open and A/B is Not Applicable.
                        if ((sNormalPosition_A == NotApplicable) && (sNormalPosition_B == NotApplicable) && (sNormalPosition_C == Open) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else if ((sNormalPosition_A == NotApplicable) && (sNormalPosition_B == NotApplicable) && (sNormalPosition_C == Close) && (sNormalPosition == NotApplicable))
                        {
                        }
                        else
                        {
                            //errror
                            return true;
                        }

                        break;

                }

                return false;
            }
            catch (Exception ex)
            {
                CommonFuntions.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }


        private bool ValidatePhase(string sSourcePhase, string sDownStreamPhase)
        {
            bool _retval = true;
            try
            {

                if (((sSourcePhase == "4") && (sDownStreamPhase != "4")) ||
                    ((sSourcePhase == "2") && (sDownStreamPhase != "2")) ||
                    ((sSourcePhase == "1") && (sDownStreamPhase != "1")) ||
                    ((sSourcePhase == "6") && ((sDownStreamPhase != "6") && (sDownStreamPhase != "4") && (sDownStreamPhase != "2"))) ||
                    ((sSourcePhase == "5") && ((sDownStreamPhase != "5") && (sDownStreamPhase != "4") && (sDownStreamPhase != "1"))) ||
                    ((sSourcePhase == "3") && ((sDownStreamPhase != "3") && (sDownStreamPhase != "2") && (sDownStreamPhase != "1"))))
                {
                    _retval = false;
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine(e.Message);
                CommonFuntions.WriteLine_Error("Error in ValidatePhase:" + ex.Message);
            }
            return _retval;
        }

     


    }
}
