using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.DataAccess.Client;

namespace PGE.BatchApplication.IGPPhaseUpdate.Processing_Logic_Classes
{
    class ProcessRelatedUnit
    {
        Common Common = new Common();

        //public  bool GetVoltageRegulatorOrStepDownUnits(string sFeatClassName, string sSubType, string sReviewBy)
        //{
        //    DataTable pDT = new DataTable();
        //    String sQuery = null;
        //    string sCircuitId = null;
        //    try
        //    {
        //        //records from Uprocessed
        //        pDT = new DataTable();
        //        sQuery = "SELECT DISTINCT(" + ReadConfigurations.DAPHIETablesFields.CircuitIdField + ") FROM " + ReadConfigurations.UnprocessedTableName + " WHERE NAME = '" +
        //            sFeatClassName + "'";
        //        pDT = DBHelperClass.GetDataTable(sQuery);

        //        if ((pDT != null) && (pDT.Rows.Count > 0))
        //        {
        //            for (int i = 0; i < pDT.Rows.Count; i++)
        //            {
        //                sCircuitId = null;
        //                sCircuitId = pDT.Rows[i][0].ToString();

        //                PrepareUnitTable(sCircuitId, sFeatClassName, sSubType, sReviewBy);
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
        //        return false;
        //    }
        //}

        public bool PrepareUnitTable(string sCircuitId, string sFeatClassName, string sSubType, string sReviewBy)
        {
            DBHelper DBHelperClass = new DBHelper();
            string sQuery = null;
            string sOperatingNumber = null;
            DataTable pDT_ONos = new DataTable();
            DataTable pDT_WholeData = new DataTable();
            DataRow[] pRows = null;
            string sViewTableName = null;
            string sRelatedUnitTableName_View = null;
            string sRelatedUnitTableName = null;
            try
            {
                if (sFeatClassName == ReadConfigurations.Devices.VOLTAGEREGULATORClassName)
                {
                    sViewTableName = ReadConfigurations.Devices_ViewName.VOLTAGEREGULATORViewName;
                    sRelatedUnitTableName_View = ReadConfigurations.Devices_ViewName.VoltageRegUnitTableViewName;
                    sRelatedUnitTableName = ReadConfigurations.RelatedTables.VOLTAGEREGULATORUNITTableName;
                }
                else if (sFeatClassName == ReadConfigurations.Devices.StepDownTableName)
                {
                    sViewTableName = ReadConfigurations.Devices_ViewName.STEPDOWNViewName;
                    sRelatedUnitTableName_View = ReadConfigurations.Devices_ViewName.StepDownUnitViewName;
                    sRelatedUnitTableName = ReadConfigurations.RelatedTables.STEPDOWNUNITableName;
                }
                //sQuery = "SELECT PHASEDESIGNATION,GLOBALID,OPERATINGNUMBER FROM EDGIS.ZZ_MV_VOLTAGEREGULATOR WHERE CIRCUITID = '" + sCircuitId + "'";
                //sQuery = "SELECT a.VALUE,a.FEATURE_GUID,a.CIRCUITID,b.OPERATINGNUMBER FROM " + ReadConfigurations.UnprocessedTableName + " a left join " +
                //    sViewTableName + " b on " +
                //    " a.FEATURE_GUID = b.GLOBALID WHERE a.CIRCUITID = '" + sCircuitId + "' and a.BATCHID = '" + MainClass.Batch_Number + "'";

                //EGIS-975 : Added one more condition "a.value is not Null" for Not updating unit record if voltage regulator phase is de-energized 
                sQuery = "SELECT a.VALUE,a.FEATURE_GUID,a.CIRCUITID,b.OPERATINGNUMBER,b.PHASEDESIGNATION FROM " + ReadConfigurations.UnprocessedTableName + " a left join " +
                    sViewTableName + " b on " +
                    " a.FEATURE_GUID = b.GLOBALID WHERE a.CIRCUITID = '" + sCircuitId + "' and a.BATCHID = '" + MainClass.Batch_Number + "' and a.value is not Null";

                pDT_WholeData = DBHelperClass.GetDataTable(sQuery);

                sQuery = "SELECT DISTINCT(" + ReadConfigurations.OperatingNumber_FieldName + ") FROM " + sViewTableName + " WHERE CIRCUITID = '" + sCircuitId + "'";
                pDT_ONos = DBHelperClass.GetDataTable(sQuery);

                for (int i = 0; i < pDT_ONos.Rows.Count; ++i)
                {
                    pRows = null;
                    sOperatingNumber = pDT_ONos.Rows[i][ReadConfigurations.OperatingNumber_FieldName].ToString();

                    pRows = pDT_WholeData.Select(ReadConfigurations.OperatingNumber_FieldName + " = '" + sOperatingNumber + "'");
                    if (pRows.Count() > 0)
                    {
                        GetAndUpdateUnprocessedUnits(pRows, sCircuitId, sRelatedUnitTableName_View, sRelatedUnitTableName, sSubType, sReviewBy, sFeatClassName);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }

        private void GetAndUpdateUnprocessedUnits(DataRow[] pRows, string sCircuitId, string sRelatedUnitTableName_View, string sRelatedUnitTableName, string sSubType,
            string sReviewBy, string sFeatClassName)
        {
            string sPhase1 = null;
            string sPhase2 = null;
            string sGuidReg_1 = null;
            string sGuidReg_2 = null;
            string cPhase1 = null;
            string cPhase2 = null;

            DataTable pDT_RegUnits = new DataTable();//a.VALUE,a.FEATURE_GUID,a.CIRCUITID,b.OPERATINGNUMBER
            try
            {
                //if voltage regulator count = 1 for operating number
                if (pRows.Count() == 1)
                {
                    sPhase1 = pRows[0][ReadConfigurations.UnprocessedTablesFields.ValuePhase_FieldName].ToString();
                    sGuidReg_1 = pRows[0][ReadConfigurations.UnprocessedTablesFields.FeatureGuid_FieldName].ToString();
                    //getting existing phase of Voltage regulator
                    cPhase1 = pRows[0]["PHASEDESIGNATION"].ToString();

                    pDT_RegUnits = GetRelatedUnitsByGuid(sGuidReg_1, sRelatedUnitTableName_View);
                    if ((pDT_RegUnits == null) || (pDT_RegUnits.Rows.Count == 0))
                    {
                        return;
                    }

                    //EGIS:975 : feedback from DMFM : For Voltage Regulator Units - The rule to keep in mind is: only update the phasing on voltage regulator units when the phasing on the shell record has also changed.
                    //Hence comparing existing phase and calculated predicted phase of Voltage regulator
                    //If predicted phase of Voltage regulator does not equal to existing phase then go-ahead for updating related regulator unit phase
                    if (sPhase1 != cPhase1)
                    {
                        ProcessUnitsFor_Count1(sPhase1, pDT_RegUnits, sCircuitId, sRelatedUnitTableName, sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                    else
                    {
                        Common.WriteLine_Info("For Voltage Regulator GUID - " + sGuidReg_1 + " : Predicted phase-" + sPhase1 + " is same its existing phase-" + cPhase1 + " so skipping phase update for voltage regulator unit ");
                    }

                }
                else if (pRows.Count() == 2)
                {
                    sPhase1 = pRows[0][ReadConfigurations.UnprocessedTablesFields.ValuePhase_FieldName].ToString();
                    sPhase2 = pRows[1][ReadConfigurations.UnprocessedTablesFields.ValuePhase_FieldName].ToString();
                    sGuidReg_1 = pRows[0][ReadConfigurations.UnprocessedTablesFields.FeatureGuid_FieldName].ToString();
                    sGuidReg_2 = pRows[1][ReadConfigurations.UnprocessedTablesFields.FeatureGuid_FieldName].ToString();
                    //getting existing phase of Voltage regulator
                    cPhase1 = pRows[0]["PHASEDESIGNATION"].ToString();
                    cPhase2 = pRows[1]["PHASEDESIGNATION"].ToString();

                    //EGIS:975 : feedback from DMFM : For Voltage Regulator Units - The rule to keep in mind is: only update the phasing on voltage regulator units when the phasing on the shell record has also changed.
                    //Hence comparing existing phase and calculated predicted phase of Voltage regulator
                    //If predicted phase of Voltage regulator does not equal to existing phase then go-ahead for updating related regulator unit phase
                    if (sPhase1 != cPhase1 || sPhase2 != cPhase2)
                    {
                        ProcessUnitsFor_Count2(sPhase1, sPhase2, sGuidReg_1, sGuidReg_2, sCircuitId, sRelatedUnitTableName_View, sRelatedUnitTableName, sSubType, sReviewBy, sFeatClassName);
                    }
                    else
                    {
                        Common.WriteLine_Info("For Voltage Regulator GUID - " + sGuidReg_1 + " : Predicted phase-" + sPhase1 + " is same its existing phase-" + cPhase1 + " so skipping phase update for voltage regulator unit ");
                        Common.WriteLine_Info("OR");
                        Common.WriteLine_Info("For Voltage Regulator GUID - " + sGuidReg_2 + " : Predicted phase-" + sPhase2 + " is same its existing phase-" + cPhase2 + " so skipping phase update for voltage regulator unit ");
                    }
                }
                else if (pRows.Count() == 3)
                {
                    ProcessUnitsFor_Count3(pRows, sCircuitId, sRelatedUnitTableName_View, sRelatedUnitTableName, sSubType, sReviewBy, sFeatClassName);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return;
            }
        }

        private void UpdateAttribute_VoltageRegulatorUnit(string sObjectClassType, string sClassName, string sPhaseToUpdate, int iFeatOid, string sExistingPhase,
            string sCircuitId, string sGuid, string sSubType, string sReviewBy, string sParentGuid, string sParentClass)
        {
            try
            {
                Common.InsertRecordInDatatable_UnProcessed(sObjectClassType, sClassName, iFeatOid, sPhaseToUpdate, "R", sCircuitId, sExistingPhase, string.Empty, sGuid, sSubType, sReviewBy, sParentGuid, sParentClass);
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
            }
        }




        private bool ProcessUnitsFor_Count2(string sPhase1, string sPhase2, string sGuidReg_1, string sGuidReg_2, string sCircuitId,
            string sRelatedUnitTableName_View, string sRelatedUnitTableName, string sSubType, string sReviewBy, string sFeatClassName)
        {
            DataTable pDT_RegUnits_1 = new DataTable();
            DataTable pDT_RegUnits_2 = new DataTable();
            string sObjectIdFieldName = null;
            string sGlobalIdFieldName = null;
            string sPhaseDesignFieldName = null;
            try
            {
                sObjectIdFieldName = ReadConfigurations.OBJECTIDFIeldName;
                sGlobalIdFieldName = ReadConfigurations.GUIDFIELDNAME;
                sPhaseDesignFieldName = ReadConfigurations.PhaseDesignationFieldName;
                pDT_RegUnits_1 = GetRelatedUnitsByGuid(sGuidReg_1, sRelatedUnitTableName_View);
                if ((pDT_RegUnits_1 == null) || (pDT_RegUnits_1.Rows.Count == 0))
                {
                    return false;
                }

                pDT_RegUnits_2 = GetRelatedUnitsByGuid(sGuidReg_2, sRelatedUnitTableName_View);
                if ((pDT_RegUnits_2 == null) || (pDT_RegUnits_2.Rows.Count == 0))
                {
                    return false;
                }

                //if each having single reg unit record
                if ((pDT_RegUnits_1.Rows.Count == 1) && (pDT_RegUnits_2.Rows.Count == 1))
                {
                    if ((sPhase1 == "7") && (sPhase2 == "7"))
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "6", Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "3", Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                    else
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase1, Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase2, Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }

                }
                //if each having 2 reg unit records
                else if ((pDT_RegUnits_1.Rows.Count == 2) && (pDT_RegUnits_2.Rows.Count == 2))
                {
                    if ((sPhase1 == "7") && (sPhase2 == "7"))
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "6", Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "3", Convert.ToInt32(pDT_RegUnits_1.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "6", Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "3", Convert.ToInt32(pDT_RegUnits_2.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                    //AB,BC
                    else if (((sPhase1 == "6") && (sPhase2 == "3")) || ((sPhase1 == "3") && (sPhase2 == "6")))
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits_1.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits_2.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                    //AB,AC
                    else if (((sPhase1 == "6") && (sPhase2 == "5")) || ((sPhase1 == "5") && (sPhase2 == "6")))
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits_1.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits_2.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                    //AC,BC
                    else if (((sPhase1 == "5") && (sPhase2 == "3")) || ((sPhase1 == "3") && (sPhase2 == "5")))
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits_1.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits_2.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                    //this condition is splited into 2 parts, with next one
                    else if (((sPhase1 == "6") && (sPhase2 == "4")) ||
                         ((sPhase1 == "6") && (sPhase2 == "2")) ||
                         ((sPhase1 == "6") && (sPhase2 == "1")) ||
                         ((sPhase1 == "5") && (sPhase2 == "4")) ||
                         ((sPhase1 == "5") && (sPhase2 == "2")) ||
                         ((sPhase1 == "5") && (sPhase2 == "1")) ||
                         ((sPhase1 == "3") && (sPhase2 == "4")) ||
                         ((sPhase1 == "3") && (sPhase2 == "2")) ||
                         ((sPhase1 == "3") && (sPhase2 == "1")))
                    {
                        string sTemp1 = null;
                        string sTemp2 = null;
                        if (sPhase1 == "6")
                        {
                            sTemp1 = "4";
                            sTemp2 = "2";
                        }
                        else if (sPhase1 == "5")
                        {
                            sTemp1 = "4";
                            sTemp2 = "1";
                        }
                        else if (sPhase1 == "3")
                        {
                            sTemp1 = "2";
                            sTemp2 = "1";
                        }

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sTemp1, Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sTemp2, Convert.ToInt32(pDT_RegUnits_1.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase2, Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase2, Convert.ToInt32(pDT_RegUnits_2.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                    //this condition is splited into 2 parts, with previous one
                    else if (((sPhase1 == "4") && (sPhase2 == "6")) ||
                        ((sPhase1 == "2") && (sPhase2 == "6")) ||
                        ((sPhase1 == "1") && (sPhase2 == "6")) ||
                        ((sPhase1 == "4") && (sPhase2 == "5")) ||
                        ((sPhase1 == "2") && (sPhase2 == "5")) ||
                        ((sPhase1 == "1") && (sPhase2 == "5")) ||
                        ((sPhase1 == "4") && (sPhase2 == "3")) ||
                        ((sPhase1 == "2") && (sPhase2 == "3")) ||
                        ((sPhase1 == "1") && (sPhase2 == "3")))
                    {
                        string sTemp1 = null;
                        string sTemp2 = null;
                        if (sPhase2 == "6")
                        {
                            sTemp1 = "4";
                            sTemp2 = "2";
                        }
                        else if (sPhase2 == "5")
                        {
                            sTemp1 = "4";
                            sTemp2 = "1";
                        }
                        else if (sPhase2 == "3")
                        {
                            sTemp1 = "2";
                            sTemp2 = "1";
                        }

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase1, Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase1, Convert.ToInt32(pDT_RegUnits_1.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sTemp1, Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sTemp2, Convert.ToInt32(pDT_RegUnits_2.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                    else if (((sPhase1 == "4") && (sPhase2 == "2")) ||
                        ((sPhase1 == "2") && (sPhase2 == "4")) ||
                        ((sPhase1 == "4") && (sPhase2 == "1")) ||
                        ((sPhase1 == "1") && (sPhase2 == "4")) ||
                        ((sPhase1 == "2") && (sPhase2 == "1")) ||
                        ((sPhase1 == "1") && (sPhase2 == "2")))
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase1, Convert.ToInt32(pDT_RegUnits_1.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase1, Convert.ToInt32(pDT_RegUnits_1.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_1.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_1.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase2, Convert.ToInt32(pDT_RegUnits_2.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);

                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase2, Convert.ToInt32(pDT_RegUnits_2.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits_2.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits_2.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_2, sFeatClassName);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }


        private bool ProcessUnitsFor_Count3(DataRow[] pRows, string sCircuitId, string sRelatedUnitTableName_View, string sRelatedUnitTableName, string sSubType,
            string sReviewBy, string sFeatClassName)
        {
            string sPhase = null;
            string sGuidReg = null;
            string cPhase = null;
            DataTable pDT_RegUnits = new DataTable();
            try
            {
                //same phase need to be updated
                for (int i = 0; i < pRows.Length; ++i)
                {
                    sPhase = null;
                    sGuidReg = null;
                    cPhase = null;
                    pDT_RegUnits = new DataTable();

                    sPhase = pRows[i][ReadConfigurations.UnprocessedTablesFields.ValuePhase_FieldName].ToString();
                    sGuidReg = pRows[i][ReadConfigurations.UnprocessedTablesFields.FeatureGuid_FieldName].ToString();
                    //getting existing phase of Voltage regulator
                    cPhase = pRows[i]["PHASEDESIGNATION"].ToString();

                    pDT_RegUnits = GetRelatedUnitsByGuid(sGuidReg, sRelatedUnitTableName_View);
                    if ((pDT_RegUnits == null) || (pDT_RegUnits.Rows.Count == 0))
                    {
                        continue;
                    }

                    //EGIS:975 : feedback from DMFM : For Voltage Regulator Units - The rule to keep in mind is: only update the phasing on voltage regulator units when the phasing on the shell record has also changed.
                    //Hence comparing existing phase and calculated predicted phase of Voltage regulator
                    //If predicted phase of Voltage regulator does not equal to existing phase then go-ahead for updating related regulator unit phase
                    if (sPhase != cPhase)
                    {
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase, Convert.ToInt32(pDT_RegUnits.Rows[0][ReadConfigurations.OBJECTIDFIeldName].ToString()),
                                pDT_RegUnits.Rows[0][ReadConfigurations.PhaseDesignationFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][ReadConfigurations.GUIDFIELDNAME].ToString(), sSubType, sReviewBy, sGuidReg, sFeatClassName);
                    }
                    else
                    {
                        Common.WriteLine_Info("For Voltage Regulator GUID - " + sGuidReg + " : Predicted phase-" + sPhase + " is same its existing phase-" + cPhase + " so skipping phase update for voltage regulator unit GUID : " + pDT_RegUnits.Rows[0][ReadConfigurations.GUIDFIELDNAME].ToString());
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }


        private bool ProcessUnitsFor_Count1(string sPhase1, DataTable pDT_RegUnits, string sCircuitId, string sRelatedUnitTableName, string sSubType, string sReviewBy,
            string sGuidReg_1, string sFeatClassName)
        {
            //string sTableName = null;
            string sObjectIdFieldName = null;
            string sGlobalIdFieldName = null;
            string sPhaseDesignFieldName = null;
            try
            {
                //sTableName = ReadConfigurations.Devices_ViewName.VoltageRegUnitTableName;
                sObjectIdFieldName = ReadConfigurations.OBJECTIDFIeldName;
                sGlobalIdFieldName = ReadConfigurations.GUIDFIELDNAME;
                sPhaseDesignFieldName = ReadConfigurations.PhaseDesignationFieldName;

                if (pDT_RegUnits.Rows.Count == 1)
                {
                    //update same phase
                    UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, sPhase1, Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                }
                else if (pDT_RegUnits.Rows.Count == 2)
                {
                    //ABC
                    if (sPhase1 == "7")
                    {
                        //1st Unit - AB
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "6", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        //2nd Unit - BC
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "3", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                    //AB
                    else if (sPhase1 == "6")
                    {
                        //1st Unit - A
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        //2nd Unit - B
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                    //BC
                    else if (sPhase1 == "3")
                    {
                        //1st Unit - B
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        //2nd Unit - C
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                    //AC
                    else if (sPhase1 == "5")
                    {
                        //1st Unit - A
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        //2nd Unit - C
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                    //A
                    else if (sPhase1 == "4")
                    {
                        //1st Unit - A
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        //2nd Unit - A
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                    //B
                    else if (sPhase1 == "2")
                    {
                        //1st Unit - B
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        //2nd Unit - B
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                    //C
                    else if (sPhase1 == "1")
                    {
                        //1st Unit - C
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                        //2nd Unit - C
                        UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                    }
                }
                else if (pDT_RegUnits.Rows.Count == 3)
                {
                    //1st Unit - A
                    UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "4", Convert.ToInt32(pDT_RegUnits.Rows[0][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[0][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[0][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                    //2nd Unit - B
                    UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "2", Convert.ToInt32(pDT_RegUnits.Rows[1][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[1][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[1][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);

                    //3rd Unit - C
                    UpdateAttribute_VoltageRegulatorUnit(ReadConfigurations.ObjectClassType.TableType, sRelatedUnitTableName, "1", Convert.ToInt32(pDT_RegUnits.Rows[2][sObjectIdFieldName].ToString()),
                            pDT_RegUnits.Rows[2][sPhaseDesignFieldName].ToString(), sCircuitId, pDT_RegUnits.Rows[2][sGlobalIdFieldName].ToString(), sSubType, sReviewBy, sGuidReg_1, sFeatClassName);
                }
                return true;
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return false;
            }
        }


        private DataTable GetRelatedUnitsByGuid(string sGuidReg, string sUnitTableName)
        {
            DBHelper DBHelperClass = new DBHelper();
            string sQuery = null;
            DataTable pDT = new DataTable();
            string sPrimaryKeyField = null;
            try
            {
                if (sUnitTableName == ReadConfigurations.Devices_ViewName.VoltageRegUnitTableViewName)
                {
                    sPrimaryKeyField = ReadConfigurations.RegulatorGuidFieldName;
                }
                else if (sUnitTableName == ReadConfigurations.Devices_ViewName.StepDownUnitViewName)
                {
                    sPrimaryKeyField = ReadConfigurations.StepDownGuidFieldName;
                }

                if (string.IsNullOrEmpty(sPrimaryKeyField) == false)
                {
                    //sQuery = "SELECT OBJECTID,GLOBALID,PHASEDESIGNATION FROM EDGIS.ZZ_MV_VOLTAGEREGULATORUNIT WHERE REGULATORGUID = '" + sGuidReg + "'";
                    //sQuery = "SELECT " + ReadConfigurations.OBJECTIDFIeldName + "," + ReadConfigurations.GUIDFIELDNAME + "," + ReadConfigurations.PhaseDesignationFieldName + " FROM " + 
                    //    ReadConfigurations.Devices_ViewName.VoltageRegUnitTableName + " WHERE " + ReadConfigurations.RegulatorGuidFieldName + " = '" + sGuidReg + "'";

                    sQuery = "SELECT " + ReadConfigurations.OBJECTIDFIeldName + "," + ReadConfigurations.GUIDFIELDNAME + "," + ReadConfigurations.PhaseDesignationFieldName + " FROM " +
                        sUnitTableName + " WHERE " + sPrimaryKeyField + " = '" + sGuidReg + "'";
                    pDT = DBHelperClass.GetDataTable(sQuery);
                }

                return pDT;
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error(ex.Message + " at " + ex.StackTrace);
                return null;
            }
        }


        internal void InsertRelatedRecordsFromStoredProcedure(string sProcedureName, string sCircuitId, string sUnprocessedTableName)
        {
            var pListParams = new List<OracleParameter>();
            string sCircuitIdsIncldQuotes = null;
            try
            {
                sCircuitIdsIncldQuotes = "'" + sCircuitId + "'";

                DBHelper DBHelperClass = new DBHelper();
                OracleParameter pParam = new OracleParameter("sCircuitIds", OracleDbType.Varchar2);
                pParam.Direction = ParameterDirection.Input;
                pParam.Size = 1000;
                pParam.Value = sCircuitIdsIncldQuotes;

                pListParams.Add(pParam);

                OracleParameter pParam1 = new OracleParameter("sUnprocessedTableName", OracleDbType.Varchar2);
                pParam1.Direction = ParameterDirection.Input;
                pParam1.Size = 100;
                pParam1.Value = sUnprocessedTableName;

                pListParams.Add(pParam1);

                OracleParameter pParam2 = new OracleParameter("sBatchNumber", OracleDbType.Varchar2);
                pParam2.Direction = ParameterDirection.Input;
                pParam2.Size = 100;
                pParam2.Value = MainClass.Batch_Number;

                pListParams.Add(pParam2);

                OracleParameter pParam3 = new OracleParameter("iSerialNo", OracleDbType.Int32);
                pParam3.Direction = ParameterDirection.Input;
                //pParam3.Size = 100;
                pParam3.Value = Convert.ToInt32(MainClass.sSerial_No);

                pListParams.Add(pParam3);

                if (DBHelperClass.ExecuteStoredProcedureCommand_TRUnit(sProcedureName, pListParams) == false)
                {
                    throw new Exception("Error in Inserting Related records through Stored Procedure," + sProcedureName);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Error in Inserting Related records through Stored Procedure," + sProcedureName + "," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }

        internal void InsertRelatedRecordsFromStoredProcedure_TranUnit(string sProcedureName, string sCircuitId, string sUnprocessedTableName, string sFeederNetworkTrace_Table)
        {
            var pListParams = new List<OracleParameter>();
            string sCircuitIdsIncldQuotes = null;
            try
            {
                sCircuitIdsIncldQuotes = "'" + sCircuitId + "'";

                DBHelper DBHelperClass = new DBHelper();
                OracleParameter pParam = new OracleParameter("sCircuitIds", OracleDbType.Varchar2);
                pParam.Direction = ParameterDirection.Input;
                pParam.Size = 1000;
                pParam.Value = sCircuitIdsIncldQuotes;

                pListParams.Add(pParam);

                OracleParameter pParam1 = new OracleParameter("sUnprocessedTableName", OracleDbType.Varchar2);
                pParam1.Direction = ParameterDirection.Input;
                pParam1.Size = 100;
                pParam1.Value = sUnprocessedTableName;

                pListParams.Add(pParam1);

                OracleParameter pParam2 = new OracleParameter("sFeederNetworkTrace_Table", OracleDbType.Varchar2);
                pParam2.Direction = ParameterDirection.Input;
                pParam2.Size = 100;
                pParam2.Value = sFeederNetworkTrace_Table;

                pListParams.Add(pParam2);

                OracleParameter pParam3 = new OracleParameter("sBatchNumber", OracleDbType.Varchar2);
                pParam3.Direction = ParameterDirection.Input;
                pParam3.Size = 100;
                pParam3.Value = MainClass.Batch_Number;

                pListParams.Add(pParam3);

                OracleParameter pParam4 = new OracleParameter("iSerialNo", OracleDbType.Int32);
                pParam4.Direction = ParameterDirection.Input;
                //pParam3.Size = 100;
                pParam4.Value = Convert.ToInt32(MainClass.sSerial_No);

                pListParams.Add(pParam4);


                if (DBHelperClass.ExecuteStoredProcedureCommand_TRUnit(sProcedureName, pListParams) == false)
                {
                    throw new Exception("Error in Inserting Related records through Stored Procedure," + sProcedureName);
                }
            }
            catch (Exception ex)
            {
                Common.WriteLine_Error("Error in Inserting Related records through Stored Procedure," + sProcedureName + "," + ex.Message + " at " + ex.StackTrace);
                throw ex;
            }
        }

    }
}
