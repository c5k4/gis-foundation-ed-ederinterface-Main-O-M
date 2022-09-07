using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Configuration;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.ADF;

namespace PGEElecCleanup
{
    public partial class frmDeleteAnno_bypassSwitches : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        IRelationshipClass pRel_Switch_SwAnno = null;
        IRelationshipClass pRel_Switch_Sw100Anno = null;
        IRelationshipClass pRel_Switch_Sw500Anno = null;

        IFeatureClass pFuseAnno = null;
        IFeatureClass pSwitchAnno = null;
        IFeatureClass pDPDAnno = null;

        public frmDeleteAnno_bypassSwitches()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                btnStart.Enabled = false;
                btnExit.Enabled = false;

                stbInfo.Text = "Starting Process..";

                stbInfo.Text = "Disable Autoupdaters ";
                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion


                string strChildTabName = string.Empty;
                objReportTable.Clear();
                objReportTable.Columns.Clear();
                _clsGlobalFunctions.Common_initSummaryTable("DeleteAnnotations_forBypassSwitch", "DeleteAnnotations_forBypassSwitch");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "SWITCH_GLOBALID,SWITCH_CEDSADEVICEID,SWITCH_OPERATINGNUMBER,DEVICE_NAME,DEVICE_GLOBALID,DEVICE_CEDSADEVICEID,DEVICE_OPERATINGNUMBER,REMARKS");

                clsTestWorkSpace.StartEditOperation();
                Logs.writeLine("StartEdit the database");
                startprocess();

                clsTestWorkSpace.StopEditOperation(true);
                stbInfo.Text = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion
                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "DeleteAnnotations_forBypassSwitch");
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                stbInfo.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");

                btnStart.Enabled = true;
                btnExit.Enabled = true;
            }
            catch (Exception ex)
            {
                btnStart.Enabled = true;
                btnExit.Enabled = true;

                Logs.writeLine("Error on process start button " + ex.Message);
                stbInfo.Text = "Error occurred, Please see the log file.";
            }
            finally
            {

            }
        }

        int switchGUIDIdx = -1;
        int switchOpNumIdx = -1;
        int switchCEDSAIDIdx = -1;
        private void startprocess()
        {
            Logs.writeLine("Beginning processing: " + DateTime.Now);
            IFeatureClass pfc = null, pfc_SWITCH = null, pfc_OPENPOINT = null, pfc_TRANSFORMER = null, pfc_FUSE = null;
            IFeature pFeat = null;
            IFeatureCursor pFCursor = null;
            IQueryFilter pQFilter = new QueryFilterClass();
            int intRecCnt = 0;
            int intProcessFeatCount = 0;
            IPoint pntDevOrig = null;

            IFeature pDevFeat = null;
            IFeature pSupportStrtFeat = null;
            IFeature pFuseFeat = null;
            List<IFeature> lstFeats = new List<IFeature>();
            List<string> Getoperatingvalues = new List<string>();
            int intiterationCount = 0;



            pRel_Switch_SwAnno = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.SwitchAnnoRel");
            pRel_Switch_Sw100Anno = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.SwitchSchem100AnnoRel");
            pRel_Switch_Sw500Anno = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.SwitchSchem500AnnoRel");

            pFuseAnno = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.FuseAnno");
            pSwitchAnno = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.SwitchAnno");
            pDPDAnno = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.DynamicProtectiveDeviceAnno");


            IRelationshipClass pRel_Fuse_Dgroup = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_Fuse");
            IRelationshipClass pRel_Switch_Dgroup = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_Switch");
            IRelationshipClass pRel_DPD_Dgroup = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_DynProtDevice");

            IRelationshipClass pRel_Switch_SupportStructure = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.SupportStruct_Switch");
            IRelationshipClass pRel_Fuse_SupportStructure = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.SupportStruct_Fuse");
            IRelationshipClass pRel_DPD_SupportStructure = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.DeviceGroup_DynProtDevice");

            pfc_SWITCH = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.SWITCH");

            string strSW_OperatingVal = string.Empty;
            string strSW_CedsaId = string.Empty;
            string strSW_Globalid = string.Empty;
            string strSwitch_JonNum = string.Empty;
            string strSwitch_SubTyp = string.Empty;
            //lstOIDS.Clear();
            switchGUIDIdx = pfc_SWITCH.FindField("GLOBALID");
            switchOpNumIdx = pfc_SWITCH.FindField("operatingnumber");
            switchCEDSAIDIdx = pfc_SWITCH.FindField("CEDSADEVICEID");
            using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
            {
                try
                {
                    pQFilter.WhereClause = "UPPER(operatingnumber) like UPPER('%BP%') OR UPPER(operatingnumber) like UPPER('%DI%') OR UPPER(operatingnumber) like UPPER('%SW%') OR UPPER(operatingnumber) like UPPER('%SB%')";
                    //pQFilter.WhereClause = "(UPPER(operatingnumber) like UPPER('%BP%') OR UPPER(operatingnumber) like UPPER('%DI%') OR UPPER(operatingnumber) like UPPER('%SW%') OR UPPER(operatingnumber) like UPPER('%SB%')) AND SUBTYPECD IN ('1','2')";
                    //pQFilter.WhereClause = "GLOBALID ='{EB7A2FD4-22B4-4497-A141-9E7B066EFA79}'";

                    intRecCnt = pfc_SWITCH.FeatureCount(pQFilter);
                    pFCursor = pfc_SWITCH.Search(pQFilter, false);
                    cr.ManageLifetime(pFCursor);
                    pFeat = pFCursor.NextFeature();
                    while (pFeat != null)
                    {
                        try
                        {
                            lstFeats.Clear();
                            intiterationCount++;
                            intProcessFeatCount++;
                            stbInfo.Text = "Processing... " + pfc_SWITCH.AliasName + " " + intProcessFeatCount + " from " + intRecCnt;
                            Application.DoEvents();
                            pntDevOrig = null;
                            strSW_Globalid = _clsGlobalFunctions.Cast(pFeat.get_Value(switchGUIDIdx).ToString(), string.Empty);
                            strSW_OperatingVal = _clsGlobalFunctions.Cast(pFeat.get_Value(switchOpNumIdx).ToString(), string.Empty);
                            strSW_CedsaId = _clsGlobalFunctions.Cast(pFeat.get_Value(switchCEDSAIDIdx).ToString(), string.Empty);
                            strSwitch_SubTyp = pFeat.get_Value(clsGlobalFunctions.GetFieldIndex((IObjectClass)pFeat.Table, "SUBTYPECD")).ToString();

                            strSwitch_JonNum = getJobNumber(pFeat);


                            //Logs.writeLine("SWITCH:globalid:" + pFeat.OID+strSW_OperatingVal+":"+pFeat.get_Value(pfc_SWITCH.FindField("globalid")).ToString());
                            //if (pFeat.get_Value(pfc_SWITCH.FindField("globalid")).ToString() == "{D32B512E-3225-4787-A131-9673096EFAF8}")
                            //{
                            //}
                            ISet pFuse_Set = null;
                            ISet pSwitch_Set = null;
                            ISet pDPD_Set = null;

                            ISet pStrtSet = pRel_Switch_Dgroup.GetObjectsRelatedToObject((IObject)pFeat);
                            string strRepValue = string.Empty;

                            if (pStrtSet.Count > 0)
                            {
                                //DeviceGroup                                
                                pDevFeat = (IFeature)pStrtSet.Next();
                                pFuse_Set = clsGlobalFunctions.getRealtedFeature(pDevFeat, "Fuse", false);
                                addFeatsToLst(ref lstFeats, pFuse_Set);
                                //pSwitch_Set = clsGlobalFunctions.getRealtedFeature(pDevFeat, "Switch", false);
                                //addFeatsToLst(ref lstFeats, pSwitch_Set);
                                pDPD_Set = clsGlobalFunctions.getRealtedFeature(pDevFeat, "Dynamic Protective Device", false);
                                addFeatsToLst(ref lstFeats, pDPD_Set);
                                if (lstFeats.Count > 0)
                                {
                                    validateSwitch_opNumber_withOtherDevices(strSwitch_SubTyp, strSW_OperatingVal, strSwitch_JonNum, lstFeats, pFeat);
                                }
                                else
                                {
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strSW_Globalid + "," + strSW_CedsaId + "," + strSW_OperatingVal + ",,,,," + " Device not found with same opnumber ");
                                }
                            }
                            else
                            {
                                //SupportStructure
                                pStrtSet = pRel_Switch_SupportStructure.GetObjectsRelatedToObject((IObject)pFeat);
                                if (pStrtSet.Count > 0)
                                {
                                    pSupportStrtFeat = (IFeature)pStrtSet.Next();
                                    pFuse_Set = clsGlobalFunctions.getRealtedFeature(pSupportStrtFeat, "Fuse", false);
                                    addFeatsToLst(ref lstFeats, pFuse_Set);
                                    //pSwitch_Set = clsGlobalFunctions.getRealtedFeature(pSupportStrtFeat, "Switch", false);
                                    //addFeatsToLst(ref lstFeats, pSwitch_Set);
                                    pDPD_Set = clsGlobalFunctions.getRealtedFeature(pSupportStrtFeat, "Dynamic Protective Device", false);
                                    addFeatsToLst(ref lstFeats, pDPD_Set);

                                    if (lstFeats.Count > 0)
                                    {
                                        validateSwitch_opNumber_withOtherDevices(strSwitch_SubTyp, strSW_OperatingVal, strSwitch_JonNum, lstFeats, pFeat);
                                    }
                                    else
                                    {
                                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strSW_Globalid + "," + strSW_CedsaId + "," + strSW_OperatingVal + ",,,,," + " Device not found with same opnumber ");
                                    }
                                }
                                else
                                {
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strSW_Globalid + "," + strSW_CedsaId + "," + strSW_OperatingVal + ",,,,," + " Not Related with any structure ");
                                    //Logs.writeLine("SWITCH:oid:" + pFeat.OID + " Not Related with any structure");
                                }
                            }

                            if (pFuse_Set != null) { while (Marshal.ReleaseComObject(pFuse_Set) > 0) { } }
                            pFuse_Set = null;
                            if (pSwitch_Set != null) { while (Marshal.ReleaseComObject(pSwitch_Set) > 0) { } }
                            pSwitch_Set = null;
                            if (pDPD_Set != null) { while (Marshal.ReleaseComObject(pDPD_Set) > 0) { } }
                            pDPD_Set = null;

                            /*
                            if (intiterationCount == 500)
                            {
                                //Logs.writeLine("  cedsa deviceid:" + lstCedsaDevIds[i]);
                                clsTestWorkSpace.StopEditOperation(true);
                                clsTestWorkSpace.StartEditOperation();
                                intiterationCount = 0;
                            }
                            */

                            if (pFeat != null) { while (Marshal.ReleaseComObject(pFeat) > 0) { } }
                            pFeat = pFCursor.NextFeature();

                        }
                        catch (Exception ex)
                        {
                            Logs.writeLine("Error at  " + pfc_SWITCH.AliasName + " : Objectid:" + pFeat.OID + " " + ex.Message);
                            pFeat = pFCursor.NextFeature();
                        }
                    }
                    if (pFCursor != null)
                    {
                        if (pFCursor != null) { while (Marshal.ReleaseComObject(pFCursor) > 0) { } }
                        //Marshal.ReleaseComObject(pFCursor);
                        pFCursor = null;
                    }

                    //Perform the actual deleting of the annoations marked to be deleted.
                    DeleteAnnotationsFlagged(toDeleteMapping, "");
                    DeleteAnnotationsFlagged(toDeleteMapping_otherThanJob, "DELOTHERTHENJOBANNO");

                    //UpdatingOperatingNumber_OID(Getoperatingvalues);
                }
                catch (Exception ex)
                {
                    Logs.writeLine("Error on startprocess " + pfc_SWITCH.AliasName + "  " + ex.Message);
                }
            }

            Logs.writeLine("Finished processing: " + DateTime.Now);
        }

        private string getJobNumber(IFeature pFeat)
        {

            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();
            List<string> lstInfoRec = new List<string>();
            ITable ptab = null;
            string strTotStr = string.Empty;

            try
            {
                if (((IDataset)pFeat.Table).Name == "EDGIS.Fuse")
                {
                    pFilter.WhereClause = "FEATUREID='" + pFeat.OID + "' and ANNOTATIONCLASSID ='" + 3 + "'";
                    ptab = (ITable)pFuseAnno;
                }
                else if (((IDataset)pFeat.Table).Name == "EDGIS.Switch")
                {
                    pFilter.WhereClause = "FEATUREID='" + pFeat.OID + "' and ANNOTATIONCLASSID ='" + 2 + "'";
                    ptab = (ITable)pSwitchAnno;
                }
                else if (((IDataset)pFeat.Table).Name == "EDGIS.DynamicProtectiveDevice")
                {
                    pFilter.WhereClause = "FEATUREID='" + pFeat.OID + "' and ANNOTATIONCLASSID ='" + 4 + "'";
                    ptab = (ITable)pDPDAnno;
                }
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = ptab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                    if (pRow != null)
                    {
                        strTotStr = pRow.get_Value(clsGlobalFunctions.GetFieldIndex((IObjectClass)pRow.Table, "TEXTSTRING")).ToString();

                        if (strTotStr.Length > 0)
                        {
                            string[] d = strTotStr.Split('\r');
                            //strTotStr = strTotStr.Replace("\r\n", "").Trim();                          

                            strTotStr = d[0];
                        }
                    }
                    if (pRow != null) { while (Marshal.ReleaseComObject(pRow) > 0) { } }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("getGis_InfoRecord " + ex.Message);
                return strTotStr;

            }
            finally
            {
                if (pCursor != null) { while (Marshal.ReleaseComObject(pCursor) > 0) { } }
                if (pFilter != null) { while (Marshal.ReleaseComObject(pFilter) > 0) { } }
            }
            return strTotStr.ToUpper();

            //string strTotStr = string.Empty;
            //try
            //{
            //    ICursor
            //    string strJobPrefix = pFeat.get_Value(clsGlobalFunctions.GetFieldIndex((IObjectClass)pFeat.Table, "INSTALLJOBPREFIX")).ToString();
            //    string strJobYear = pFeat.get_Value(clsGlobalFunctions.GetFieldIndex((IObjectClass)pFeat.Table, "INSTALLJOBYEAR")).ToString();
            //    string strJobNum = pFeat.get_Value(clsGlobalFunctions.GetFieldIndex((IObjectClass)pFeat.Table, "INSTALLJOBNUMBER")).ToString();
            //    strTotStr = strJobPrefix + strJobYear + strJobNum;
            //    return strTotStr.ToUpper();
            //}
            //catch (Exception ex)
            //{
            //    Logs.writeLine("Error on getJobNumber " + pFeat.Class.AliasName + "  " + ex.Message);
            //}
            //return strTotStr.ToUpper();
        }

        private void validateSwitch_opNumber_withOtherDevices(string strSwitch_SubTyp, string strSW_OperatingVal, string strSwitch_JonNum, List<IFeature> lstFeats, IFeature pSwitchFeat)
        {
            IFeature pTempfeat = null;
            IFeature pfeat = null;
            Boolean bolJobNumbersame = true;
            //IFeature pAnnofeat = null;

            strSW_OperatingVal = strSW_OperatingVal.ToUpper();

            string strSwitch_cedsaId = string.Empty;
            string strSwitch_Globalid = string.Empty;
            string strSwitch_Opnumber = string.Empty;

            string strDevice_cedsaId = string.Empty;
            string strDevice_Opnumber = string.Empty;
            string strDevice_Globalid = string.Empty;
            ISet pSet_Switch_Anno = null;
            ISet pSet_Switch_100Anno = null;
            ISet pSet_Switch_500Anno = null;
            string strDev_JonNum = string.Empty;

            try
            {
                strSwitch_Opnumber = strSW_OperatingVal;
                strSwitch_Opnumber = strSwitch_Opnumber.Replace("BP", "");
                strSwitch_Opnumber = strSwitch_Opnumber.Replace("DI", "");
                strSwitch_Opnumber = strSwitch_Opnumber.Replace("SB", "");
                strSwitch_Opnumber = strSwitch_Opnumber.Replace("SW", "");

                strSwitch_cedsaId = pSwitchFeat.get_Value(switchCEDSAIDIdx).ToString();
                strSwitch_Globalid = pSwitchFeat.get_Value(switchGUIDIdx).ToString();

                if (strSwitch_cedsaId == "" || strSwitch_cedsaId == "9999")
                {
                    //
                    for (int i = 0; i < lstFeats.Count; i++)
                    {
                        pTempfeat = lstFeats[i];
                        strDevice_Opnumber = pTempfeat.get_Value(pTempfeat.Fields.FindField("operatingnumber")).ToString();

                        if (strSW_OperatingVal.Contains(strDevice_Opnumber))
                        {
                            strDev_JonNum = getJobNumber(pTempfeat);
                            if ((strDev_JonNum != strSwitch_JonNum) && (strSwitch_SubTyp == "2" || strSwitch_SubTyp == "1") && strSwitch_JonNum != "")
                            {
                                strSwitch_JonNum = strSwitch_JonNum.Trim();

                                //Logs.writeLine("Switch Oid:"+pSwitchFeat.OID + ":Switch JobNum:" + strSwitch_JonNum + ":Device JobNum:" + strDev_JonNum );
                                bolJobNumbersame = false;
                            }

                            pfeat = pTempfeat;
                            i = lstFeats.Count;
                        }
                    }
                    if (pfeat != null)
                    {
                        //SWITCH_GLOBALID,SWITCH_CEDSADEVICEID,SWITCH_OPERATINGNUMBER,DEVICE_NAME,DEVICE_GLOBALID,DEVICE_CEDSADEVICEID,DEVICE_OPERATINGNUMBER,REMARKS

                        //strDevice_cedsaId = pfeat.get_Value(pfeat.Fields.FindField("CEDSADEVICEID")).ToString();
                        //strDevice_Globalid = pfeat.get_Value(pfeat.Fields.FindField("GLOBALID")).ToString();

                        //delete related Anno 

                        /*
                        pSet_Switch_Anno = pRel_Switch_SwAnno.GetObjectsRelatedToObject((IObject)pSwitchFeat);
                        pSet_Switch_100Anno = pRel_Switch_Sw100Anno.GetObjectsRelatedToObject((IObject)pSwitchFeat);
                        pSet_Switch_500Anno = pRel_Switch_Sw500Anno.GetObjectsRelatedToObject((IObject)pSwitchFeat);
                        deleteRelAnno(pSet_Switch_Anno, pRel_Switch_SwAnno);
                        deleteRelAnno(pSet_Switch_100Anno, pRel_Switch_Sw100Anno);
                        deleteRelAnno(pSet_Switch_500Anno, pRel_Switch_Sw500Anno);

                        if (pSet_Switch_Anno != null) { while (Marshal.ReleaseComObject(pSet_Switch_Anno) > 0) { } }
                        if (pSet_Switch_100Anno != null) { while (Marshal.ReleaseComObject(pSet_Switch_100Anno) > 0) { } }
                        if (pSet_Switch_500Anno != null) { while (Marshal.ReleaseComObject(pSet_Switch_500Anno) > 0) { } }
                        pSet_Switch_Anno = null;
                        pSet_Switch_100Anno = null;
                        pSet_Switch_500Anno = null;
                        */
                        if (bolJobNumbersame == true)
                        {
                            toDeleteMapping.Add(pSwitchFeat.OID);
                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strSwitch_Globalid + "," + strSwitch_cedsaId + "," + strSW_OperatingVal + "," + pfeat.Class.AliasName + "," + strDevice_Globalid + "," + strDevice_cedsaId + "," + strDevice_Opnumber + "," + " related Annos deleted to this Switch");
                        }
                        else
                        {
                            toDeleteMapping_otherThanJob.Add(pSwitchFeat.OID);
                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strSwitch_Globalid + "," + strSwitch_cedsaId + "," + strSW_OperatingVal + "," + pfeat.Class.AliasName + "," + strDevice_Globalid + "," + strDevice_cedsaId + "," + strDevice_Opnumber + "," + " related Annos deleted to this Switch.Other than JobAnno");
                        }
                    }
                    else
                    {
                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strSwitch_Globalid + "," + strSwitch_cedsaId + "," + strSW_OperatingVal + ",,,,," + " Device not found with same opnumber ");
                    }
                }
                else
                {
                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strSwitch_Globalid + "," + strSwitch_cedsaId + "," + strSW_OperatingVal + ",,,,," + " Its valid cedsadeviceid.Manual review required");
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on validateSwitch_opNumber_withOtherDevices " + ex.Message);
            }
            finally
            {
                if (pfeat != null) { while (Marshal.ReleaseComObject(pfeat) > 0) { } }
                if (pTempfeat != null) { while (Marshal.ReleaseComObject(pTempfeat) > 0) { } }
            }
        }

        private void DeleteAnnotationsFlagged(List<int> toDeleteMapping, string Strdel)
        {
            List<string> whereInStringList = new List<string>();
            StringBuilder whereInStringBuilder = new StringBuilder();
            for (int i = 0; i < toDeleteMapping.Count; i++)
            {
                if (i == toDeleteMapping.Count - 1 || (i != 0 && (i % 999) == 0)) { whereInStringBuilder.Append(toDeleteMapping[i]); }
                else { whereInStringBuilder.Append(toDeleteMapping[i] + ","); }

                if ((i % 999) == 0 && i != 0)
                {
                    whereInStringList.Add(whereInStringBuilder.ToString());
                    whereInStringBuilder = new StringBuilder();
                }
            }

            if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
            {
                whereInStringList.Add(whereInStringBuilder.ToString());
            }

            List<ITable> annoTables = new List<ITable>();
            annoTables.Add(((ITable)(pRel_Switch_SwAnno.DestinationClass)));
            annoTables.Add(((ITable)(pRel_Switch_Sw100Anno.DestinationClass)));
            annoTables.Add(((ITable)(pRel_Switch_Sw500Anno.DestinationClass)));

            Logs.writeLine("Beginning actual delete of annotations: " + DateTime.Now);
            foreach (string whereInClause in whereInStringList)
            {
                foreach (ITable annoTable in annoTables)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    if (Strdel == "DELOTHERTHENJOBANNO" && ((IDataset)annoTable).Name == "EDGIS.SwitchAnno")
                    {
                        qf.WhereClause = "FEATUREID in (" + whereInClause + ") and ANNOTATIONCLASSID <> '2'";
                    }
                    else
                    {
                        qf.WhereClause = "FEATUREID in (" + whereInClause + ")";
                    }
                    qf.SubFields = "FEATUREID";
                    int count = annoTable.RowCount(qf);

                    Logs.writeLine(((IDataset)annoTable).BrowseName + ": Deleting " + count + " annotations: " + DateTime.Now);

                    ICursor switchAnnoCursor = annoTable.Search(qf, false);
                    IRow rowToDelete = null;
                    while ((rowToDelete = switchAnnoCursor.NextRow()) != null)
                    {
                        rowToDelete.Delete();
                        while (Marshal.ReleaseComObject(rowToDelete) > 0) { }
                    }
                    if (switchAnnoCursor != null) { while (Marshal.ReleaseComObject(switchAnnoCursor) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    clsTestWorkSpace.StopEditOperation(true);
                    clsTestWorkSpace.StartEditOperation();
                }
            }
            Logs.writeLine("Finished deletion of annotations: " + DateTime.Now);
        }

        List<int> toDeleteMapping = new List<int>();
        List<int> toDeleteMapping_otherThanJob = new List<int>();

        /*
        Dictionary<string, List<int>> toDeleteMapping = new Dictionary<string, List<int>>();
        private void deleteRelAnno(ISet pSet_Switch_Anno, IRelationshipClass relClass)
        {
            List<int> toDelete;
            if (!toDeleteMapping.ContainsKey(relClass.DestinationClass.AliasName))
            {
                toDelete = new List<int>();
                toDeleteMapping.Add(relClass.DestinationClass.AliasName, toDelete);
            }
            else
            {
                toDelete = toDeleteMapping[relClass.DestinationClass.AliasName];
            }

            IFeature pAnnofeat = null;
            try
            {
                if (pSet_Switch_Anno.Count != 0)
                {
                    pAnnofeat = (IFeature)pSet_Switch_Anno.Next();
                    while (pAnnofeat != null)
                    {
                        toDelete.Add(pAnnofeat.OID);
                        //pAnnofeat.Delete();
                        pAnnofeat = (IFeature)pSet_Switch_Anno.Next();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on deleteRelAnno " + ex.Message);
            }
            
        }
        */
        private void addFeatsToLst(ref List<IFeature> lstFeats, ISet pSet)
        {
            IFeature pfeat = null;
            try
            {
                //if (pSet != null)
                // {
                if (pSet.Count != 0)
                {
                    pfeat = (IFeature)pSet.Next();
                    while (pfeat != null)
                    {

                        lstFeats.Add(pfeat);
                        pfeat = (IFeature)pSet.Next();
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error @ addFeatsToLst " + ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private mmAutoUpdaterMode DisableAutoupdaters()
        {
            object objAutoUpdater = null;

            //Create an MMAutoupdater 
            objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater"));
            autoupdater = objAutoUpdater as IMMAutoUpdater;
            //Save the existing mode
            mmAutoUpdaterMode oldMode = autoupdater.AutoUpdaterMode;
            //Turn off autoupdater events
            autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
            // insert code that needs to execute while autoupdaters 
            return oldMode;
        }
    }
}
