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
    public partial class frmGenerateVoltageRegUnits : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();

        public frmGenerateVoltageRegUnits()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fldrDia = new FolderBrowserDialog();
                fldrDia.Description = "Select FGD database for VoltageRegUnits generation..";
                if (fldrDia.ShowDialog() == DialogResult.OK)
                {
                    txtFGDpath.Text = fldrDia.SelectedPath;
                }
                if (txtFGDpath.Text.Length == 0)
                {
                    MessageBox.Show("Select FileGeoDatabase", "Message", MessageBoxButtons.OK);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
                _clsGlobalFunctions.Common_initSummaryTable("GenerateVoltageRegUnits", "GenerateVoltageRegUnits");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "CEDSADEVICEID,GIS_FEAT_COUNT,CEDSA_FEAT_COUNT,CEDSA_UNIT_COUNT,REMARKS");
                IWorkspace pCedsaWsp = null;
                IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                pCedsaWsp = workspaceFactory.OpenFromFile(txtFGDpath.Text, 0);
                IFeatureWorkspace pCedsaFws = (IFeatureWorkspace)pCedsaWsp;
                ITable pCedVolRegUnitTab = pCedsaFws.OpenTable("VoltageRegulatorUnit");
                Logs.writeLine("Opend the VoltageRegulatorUnit table ");

                clsTestWorkSpace.StartEditOperation();
                Logs.writeLine("StartEdit the database");
                startprocess(pCedVolRegUnitTab);
                //if (strTabName == "ALL")
                //{
                //    for (int i = 0; i < cmbAllFCS.Items.Count; i++)
                //    {
                //        if (cmbAllFCS.Items[i].ToString().ToUpper() != "ALL")
                //        {
                //            clsTestWorkSpace.StartEditOperation();
                //            startprocess(cmbAllFCS.Items[i].ToString().ToUpper(), pCedControlTab);
                //            clsTestWorkSpace.StopEditOperation(true);
                //        }
                //    }
                //}
                //else
                //{
                //    startprocess(cmbAllFCS.SelectedItem.ToString().ToUpper(), pCedControlTab);

                //}
                clsTestWorkSpace.StopEditOperation(true);

                stbInfo.Text = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "GenerateVoltageRegUnits");
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

        private void startprocess(ITable pCedVolRegUnitTab)
        {

            IFeatureClass pVolRegfc = null;            
            ITable pVoltageUnittab = null; 
            ITable pControltab = null;
            int intiterationCount = 0;           

            try
            {
                pVolRegfc = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.VoltageRegulator");               
                pVoltageUnittab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.VoltageRegulatorUnit"));

                pControltab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.Controller"));
                    

                IObjectClass obj = (IObjectClass)pCedVolRegUnitTab;

                int intCed_CEDSADEVIdx = pCedVolRegUnitTab.FindField("CEDSADEVICEID");
                int intCed_UNITIDidx = pCedVolRegUnitTab.FindField("VRUNITID");

                string strCEDSADEVIdVal = string.Empty;
                string strCed_UNITIDVal = string.Empty;
                string strFeatGuidVal = string.Empty;
                string strSubTypCDVal = string.Empty;

                stbInfo.Text = "processing wait...";
                Application.DoEvents();

                List<string> lstCedsaDevIds = new List<string>();
                clsGFMSGlobalFunctions._gerUniqueFieldVals((ITable)pVolRegfc, "CEDSADEVICEID", ref lstCedsaDevIds);
                List<IFeature> lstRegFeats = new List<IFeature>();
                List<IRow> lstCedsaRegUnits = new List<IRow>();
                string strNetworkType = string.Empty ;

                for (int i = 0; i < lstCedsaDevIds.Count; i++)
                {
                    try
                    {
                        stbInfo.Text = "processing...." + i + " of " + lstCedsaDevIds.Count;
                        Application.DoEvents();

                        intiterationCount++;
                        if (lstCedsaDevIds[i] == "9999") continue;
                        //if (lstCedsaDevIds[i] != "1040033351")
                        //{
                        //    continue;
                        //}
                        
                        lstRegFeats.Clear();
                        getFeatWithDeviId(lstCedsaDevIds[i], pVolRegfc, lstRegFeats);
                        lstCedsaRegUnits.Clear();
                        getCedsaUnitsWithDeviId(lstCedsaDevIds[i], pCedVolRegUnitTab, lstCedsaRegUnits);
                        GetFeatType(lstRegFeats[0], ref strNetworkType);
                        ValidateAndInsertRecords(pControltab,pCedVolRegUnitTab, pVoltageUnittab, lstRegFeats, lstCedsaRegUnits, lstCedsaDevIds[i], strNetworkType);
                        //Logs.writeLine("VoltageRegulator Units generated for Feature cedsa deviceid :" + lstCedsaDevIds[i]);
                        //"CEDSADEVICEID,GIS_FEAT_COUNT,CEDSA_UNIT_COUNT,REMARKS"
                        
                         if (intiterationCount == 200)
                        {
                            //Logs.writeLine("  cedsa deviceid:" + lstCedsaDevIds[i]);
                            clsTestWorkSpace.StopEditOperation(true);
                            clsTestWorkSpace.StartEditOperation();
                            intiterationCount = 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.writeLine("  #startprocess processing....cedsa deviceid:"+lstCedsaDevIds[i] + "  "+ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("  #startprocess " + ex.Message);
            }
        }

        private void GetFeatType(IFeature pfeat, ref string strNetworkType)
        {
            IFeature pCondFeat = null;
            
            try
            {
                ISimpleJunctionFeature pJun = (ISimpleJunctionFeature)pfeat;
                if (pJun.EdgeFeatureCount == 0)
                {
                    Logs.writeLine("Hanging feature. VoltageRegulator: " + pfeat.OID);
                }

                for (int i = 0; i < pJun.EdgeFeatureCount; i++)
                {
                    IEdgeFeature pEdge = pJun.get_EdgeFeature(i);
                    pCondFeat = (IFeature)pEdge;
                    if (((IDataset)pCondFeat.Table).Name.ToUpper().Contains("OH"))
                    {
                        strNetworkType = "OH";
                    }
                    else if (((IDataset)pCondFeat.Table).Name.ToUpper().Contains("UG"))
                    {
                        strNetworkType = "UG";
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("  #GetFeatType " + ex.Message);
            }
        }

        private void ValidateAndInsertRecords(ITable pControltab,ITable pCedVolRegUnitTab, ITable pVoltageUnittab, List<IFeature> lstRegFeats, List<IRow> lstCedsaRegUnits, string strCedsaDevId, string strInstallationTyp)
        {
            string strFeatGlobalid = string.Empty;
            string strFeatStatus = string.Empty;
            string strPhase = string.Empty;

            try
            {
                if (lstRegFeats.Count > lstCedsaRegUnits.Count)
                {
                    //Logs.writeLine("CedsaDeviceId : " + strCedsaDevId + ": Cedsa Unit count is lesser then Gis Feat Count.");
                    //"CEDSADEVICEID,GIS_FEAT_COUNT,CEDSA_UNIT_COUNT,REMARKS"
                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strCedsaDevId + "," + lstRegFeats.Count + ",0" + "," + lstCedsaRegUnits.Count + "," + " Cedsa Unit count is lesser then Gis Feat Count.Need to check manually ");
                }
                else
                {
                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strCedsaDevId + "," + lstRegFeats.Count + ",1" + "," + lstCedsaRegUnits.Count + "," + " VoltageRegulatorUnits generated ");


                    if (lstRegFeats.Count == 1)
                    {
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[0], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        for (int i = 0; i < lstCedsaRegUnits.Count; i++)
                        {
                            insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[i], strCedsaDevId, strInstallationTyp, strPhase);
                        }
                    }
                    else if (lstRegFeats.Count == 2 && lstCedsaRegUnits.Count == 2)
                    {
                        //Feat 1
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[0], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[0], strCedsaDevId, strInstallationTyp, strPhase);
                        //Feat 2
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[1], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[1], strCedsaDevId, strInstallationTyp, strPhase);
                    }
                    else if (lstRegFeats.Count == 2 && lstCedsaRegUnits.Count == 3)
                    {
                        //Feat 1
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[0], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[0], strCedsaDevId, strInstallationTyp, strPhase);
                        //Feat 2
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[1], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[1], strCedsaDevId, strInstallationTyp, strPhase);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[2], strCedsaDevId, strInstallationTyp, strPhase);
                    }
                    else if (lstRegFeats.Count == 3 && lstCedsaRegUnits.Count == 3)
                    {
                        //Feat 1
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[0].get_Value(lstRegFeats[0].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[0], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[0], strCedsaDevId, strInstallationTyp, strPhase);
                        //Feat 2
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[1].get_Value(lstRegFeats[1].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[1], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[1], strCedsaDevId, strInstallationTyp, strPhase);
                        //Feat 3
                        strFeatGlobalid = _clsGlobalFunctions.Cast(lstRegFeats[2].get_Value(lstRegFeats[2].Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        strFeatStatus = _clsGlobalFunctions.Cast(lstRegFeats[2].get_Value(lstRegFeats[2].Fields.FindField("STATUS")).ToString(), string.Empty);
                        strPhase = _clsGlobalFunctions.Cast(lstRegFeats[2].get_Value(lstRegFeats[0].Fields.FindField("PHASEDESIGNATION")).ToString(), string.Empty);
                        deleteExistingUnitRecords(pControltab,lstRegFeats[2], strFeatGlobalid, pVoltageUnittab, strCedsaDevId);
                        insertRow(strFeatStatus, strFeatGlobalid, pVoltageUnittab, lstCedsaRegUnits[2], strCedsaDevId, strInstallationTyp, strPhase);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at ValidateAndInsertRecords for CEDSADEVICEID:" + strCedsaDevId + " " + ex.Message);
            }
        }

        private void deleteExistingUnitRecords(ITable pControltab,IFeature iFeature, string strFeatGlobalid, ITable pVoltageUnittab, string strCedsaDevId)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();

            string strRefUnitGlobalid = string.Empty;
            try
            {
                pFilter.WhereClause = "REGULATORGUID='" + strFeatGlobalid + "'";
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pVoltageUnittab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                    while (pRow != null)
                    {
                        strRefUnitGlobalid = _clsGlobalFunctions.Cast(pRow.get_Value(pRow.Fields.FindField("GLOBALID")).ToString(), string.Empty);
                        deleteChildControllerRec(pControltab, strRefUnitGlobalid, strCedsaDevId);

                        pRow.Delete();
                        pRow = pCursor.NextRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at deleteExistingUnitRecords for CEDSADEVICEID:" + strCedsaDevId +" " + ex.Message);
            }
        }

        private void deleteChildControllerRec(ITable pControltab, string strRefUnitGlobalid, string strCedsaDevId)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();
            
            try
            {
                pFilter.WhereClause = "DEVICEGUID='" + strRefUnitGlobalid + "'";
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pControltab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                    while (pRow != null)
                    {
                        pRow.Delete();
                        pRow = pCursor.NextRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at deleteChildControllerRec for CEDSADEVICEID:" + strCedsaDevId + " " + ex.Message);
            }
        }

        private void insertRow(string strFeatStatus, string strFeatGlobalid, ITable pVoltageUnittab, IRow pCedsaRow, string strCedsaDevId, string strInstallationTyp, string strPhase)
        {

            IRowBuffer pRowBuffer = null;
            ICursor pInserCursor = null;
            string strINSTALLATIONDATEVal = string.Empty;
            string strCedsastrInstallationTyp = string.Empty;
            
            try
            {
                string s = ((IObjectClass)pVoltageUnittab).AliasName;
                //Logs.writeLine("");
                pRowBuffer = pVoltageUnittab.CreateRowBuffer();
                for (int i = 0; i < pVoltageUnittab.Fields.FieldCount; i++)
                {
                    int sourcefieldindex = -1;
                    int destinationfieldindex = -1;
                    IField objfield = pVoltageUnittab.Fields.get_Field(i);
                    try
                    {
                        
                        if (objfield.Editable)
                        {
                            destinationfieldindex = pVoltageUnittab.Fields.FindField(objfield.Name);
                            sourcefieldindex = pCedsaRow.Fields.FindField(objfield.Name);
                            
                            //if (objfield.Name != "INSTALLJOBYEAR")
                            //{
                            //    continue;
                            
                            //}
                            //if(sourcefieldindex != -1)
                            //{
                            //    string s1 = _clsGlobalFunctions.Cast(pCedsaRow.get_Value(sourcefieldindex).ToString(), string.Empty);
                            //    Logs.writeLine("CedsaFld value :"+objfield.Name + ": "+_clsGlobalFunctions.Cast(pCedsaRow.get_Value(sourcefieldindex).ToString(), string.Empty)); 
                            //}

                            if (destinationfieldindex != -1 && sourcefieldindex != -1)
                            {
                                if (objfield.Name.ToUpper() != "CONVERSIONID" && objfield.Name.ToUpper() != "DATEMODIFIED" && objfield.Name.ToUpper() != "LASTUSER" && objfield.Name.ToUpper() != "CONVERSIONWORKPACKAGE")
                                {
                                    //Logs.writeLine(objfield.Name);
                                    if (objfield.Name.ToUpper() == "INSTALLJOBYEAR")
                                    {
                                        string str = _clsGlobalFunctions.Cast(pCedsaRow.get_Value(sourcefieldindex).ToString(), string.Empty);
                                        if (_clsGlobalFunctions.Cast(pCedsaRow.get_Value(sourcefieldindex).ToString(), string.Empty) != string.Empty)
                                        {
                                            strINSTALLATIONDATEVal = validateDate(_clsGlobalFunctions.Cast(pCedsaRow.get_Value(sourcefieldindex).ToString(), string.Empty));

                                            if (strINSTALLATIONDATEVal.Length > 0)
                                            {
                                                pRowBuffer.set_Value(destinationfieldindex, strINSTALLATIONDATEVal);
                                            }
                                        }
                                    }
                                    else if (objfield.Name.ToUpper() == "PHASEDESIGNATION")
                                    {
                                        pRowBuffer.set_Value(destinationfieldindex, strPhase);
                                    }
                                    else if (objfield.Name.ToUpper() == "SUBTYPECD")
                                    {
                                        pRowBuffer.set_Value(destinationfieldindex, '1');
                                    }
                                    else if (objfield.Name.ToUpper() == "CEDSADEVICEID")
                                    {
                                        pRowBuffer.set_Value(destinationfieldindex, pCedsaRow.get_Value(sourcefieldindex));
                                        pRowBuffer.set_Value(pVoltageUnittab.Fields.FindField("CEDSAREGULATORID"), pCedsaRow.get_Value(sourcefieldindex));
                                    }

                                    else if (objfield.Name.ToUpper() == "INSTALLATIONTYPE")
                                    {
                                        strCedsastrInstallationTyp = pCedsaRow.get_Value(sourcefieldindex).ToString();

                                        if (strCedsastrInstallationTyp != "OH" && strInstallationTyp == "OH")
                                        {
                                            pRowBuffer.set_Value(destinationfieldindex, strInstallationTyp);
                                        }
                                        else
                                        {
                                            pRowBuffer.set_Value(destinationfieldindex, pCedsaRow.get_Value(sourcefieldindex).ToString());
                                        }
                                    }
                                    else
                                    {
                                        //string str = _clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty);
                                        ////Logs.writeLine(str);
                                        pRowBuffer.set_Value(destinationfieldindex, pCedsaRow.get_Value(sourcefieldindex));
                                    }
                                }
                            }
                            else
                            {
                               // Logs.writeLine(objfield.Name);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Logs.writeLine("Error @ retriving value :" + objfield.Name + " Cedsa Deviceid : " + strCedsaDevId + " " + ex.Message);
                    }
                }
                pRowBuffer.set_Value(pVoltageUnittab.FindField("SUBTYPECD"), "1");
                pRowBuffer.set_Value(pVoltageUnittab.FindField("CREATIONUSER"), "P1P4");
                pRowBuffer.set_Value(pVoltageUnittab.FindField("DATECREATED"), String.Format("{0:M/d/yyyy}", DateTime.Now));
                pRowBuffer.set_Value(pVoltageUnittab.Fields.FindField("REGULATORGUID"), strFeatGlobalid);
                pRowBuffer.set_Value(pVoltageUnittab.Fields.FindField("STATUS"), strFeatStatus);

                pInserCursor = pVoltageUnittab.Insert(false);
                pInserCursor.InsertRow(pRowBuffer);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at insert RECORD for CEDSADEVID :" + strCedsaDevId + "  " + ex.Message);
            }
            finally
            {
                if (pInserCursor != null)
                {
                    Marshal.ReleaseComObject(pInserCursor);
                }
                if (pRowBuffer != null)
                {
                    Marshal.ReleaseComObject(pRowBuffer);
                }
            }

        }
        private string validateDate(string strDate)
        {
            string strINSTALLATIONDATEVal = string.Empty;
            int intYear = 0;
            try
            {
                if (strDate.Length == 2)
                {
                    intYear = int.Parse(strDate);                   
                    if (intYear < 13)
                    {
                        if (intYear > 9)
                        {
                            strINSTALLATIONDATEVal = "20" + intYear;
                        }
                        else
                        {
                            strINSTALLATIONDATEVal = "200" + intYear;
                        }
                        //strINSTALLATIONDATEVal = "01-JAN-20" + intYear;
                        //strINSTALLATIONDATEVal = " 1/1/20" + intYear + " 12:00:00 AM";
                        //strINSTALLATIONDATEVal = "20" + intYear;
                    }
                    else
                    {
                        //strINSTALLATIONDATEVal = " 1/1/19" + intYear + " 12:00:00 AM";
                        //strINSTALLATIONDATEVal = "01-JAN-19" + intYear; 
                        strINSTALLATIONDATEVal = "19" + intYear;
                    }
                }
                else if (strDate.Length == 1)
                {
                    strINSTALLATIONDATEVal = "200" + intYear;
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error Validating string:" + strDate + "  " + ex.Message);
            }

            return strINSTALLATIONDATEVal;
        }
        private void getCedsaUnitsWithDeviId(string strDevId, ITable pCedVolRegUnitTab, List<IRow> lstRegUnits)
        {
            IRow prow = null;

            IQueryFilter pFilter = new QueryFilterClass();
            ICursor pCursor = null;
            try
            {
                pFilter.WhereClause = "CEDSADEVICEID =" + strDevId ;
                pCursor = pCedVolRegUnitTab.Search(pFilter, false);
                prow = pCursor.NextRow();
                while (prow != null)
                {
                    lstRegUnits.Add(prow);

                    prow = pCursor.NextRow();
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error in " +  strDevId + "  " + ex.Message);
            }
            finally
            {
                if (pCursor != null)
                {
                    Marshal.ReleaseComObject(pCursor);
                    pCursor = null;
                }
            } 
            
        }

        private void getFeatWithDeviId(string strDevId, IFeatureClass pVolRegfc, List<IFeature> lstRegFeats)
        {
            IFeature pfeature = null;
            
            IQueryFilter pFilter = new QueryFilterClass();
            IFeatureCursor pFCursor = null;            
            try
            {
                pFilter.WhereClause = "CEDSADEVICEID ='" + strDevId + "'";
                pFCursor = pVolRegfc.Search(pFilter, false);
                pfeature = pFCursor.NextFeature();
                while (pfeature != null)
                {
                    lstRegFeats.Add(pfeature);

                    pfeature = pFCursor.NextFeature();
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error in " + pVolRegfc.AliasName + " : " + strDevId + "  " + ex.Message);
            }
            finally
            {
                if (pFCursor != null)
                {
                    Marshal.ReleaseComObject(pFCursor);
                    pFCursor = null;
                }
            } 
            
        }
        //private void insertRow(IRow pCedsaControllerRow, ITable pChildTab, string strParentGuid, string strCEDSADEVIdVal, ITable pParenttab, string strSubtypeCdVal)
        //{
        //    //IRowBuffer pRowBuffer = null;
        //    //ICursor pInserCursor = null;
        //    //string strINSTALLATIONDATEVal = string.Empty;
        //    //int intFldIdx = -1;
        //    //try
        //    //{
        //    //    string s = ((IObjectClass)pChildTab).AliasName;

        //    //    pRowBuffer = pChildTab.CreateRowBuffer();
        //    //    for (int i = 0; i < pChildTab.Fields.FieldCount; i++)
        //    //    {
        //    //        int sourcefieldindex = -1;
        //    //        int destinationfieldindex = -1;
        //    //        IField objfield = pChildTab.Fields.get_Field(i);
        //    //        try
        //    //        {
        //    //            if (objfield.Editable)
        //    //            {
        //    //                destinationfieldindex = pChildTab.Fields.FindField(objfield.Name);
        //    //                sourcefieldindex = pCedsaControllerRow.Fields.FindField(objfield.Name);

        //    //                if (destinationfieldindex != -1 && sourcefieldindex != -1)
        //    //                {
        //    //                    if (objfield.Name.ToUpper() != "CONVERSIONID" && objfield.Name.ToUpper() != "DATEMODIFIED" && objfield.Name.ToUpper() != "LASTUSER")
        //    //                    {

        //    //                        if (objfield.Name.ToUpper() == "INSTALLJOBYEAR")
        //    //                        {
        //    //                            //string str = _clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty);

        //    //                            //if (_clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty) != string.Empty)
        //    //                            //{
        //    //                            //    strINSTALLATIONDATEVal = validateDate(_clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty));

        //    //                            //    if (strINSTALLATIONDATEVal.Length > 0)
        //    //                            //    {
        //    //                            //        pRowBuffer.set_Value(destinationfieldindex, strINSTALLATIONDATEVal);
        //    //                            //    }
        //    //                            //}
        //    //                        }
        //    //                        else if (objfield.Name.ToUpper() == "SUBTYPECD")
        //    //                        {

        //    //                            pRowBuffer.set_Value(destinationfieldindex, strSubtypeCdVal);

        //    //                        }
        //    //                        else
        //    //                        {
        //    //                            string str = _clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty);

        //    //                            //Logs.writeLine(str);

        //    //                            pRowBuffer.set_Value(destinationfieldindex, pCedsaControllerRow.get_Value(sourcefieldindex));
        //    //                        }
        //    //                    }
        //    //                }
        //    //            }

        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            Logs.writeLine("Error @ retriving value :" + objfield.Name + " Cedsa Deviceid : " + strCEDSADEVIdVal + " " + ex.Message);
        //    //        }
        //    //    }
        //    //    pRowBuffer.set_Value(pChildTab.FindField("CREATIONUSER"), "P1P4");
        //    //    pRowBuffer.set_Value(pChildTab.FindField("DATECREATED"), String.Format("{0:d/M/yyyy}", DateTime.Now));
        //    //    pRowBuffer.set_Value(pChildTab.Fields.FindField("DEVICEGUID"), strParentGuid);

        //    //    pInserCursor = pChildTab.Insert(false);
        //    //    pInserCursor.InsertRow(pRowBuffer);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error at insert RECORD for CEDSADEVID :" + strCEDSADEVIdVal + "  " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (pInserCursor != null)
        //        {
        //            Marshal.ReleaseComObject(pInserCursor);
        //        }
        //        if (pRowBuffer != null)
        //        {
        //            Marshal.ReleaseComObject(pRowBuffer);
        //        }
        //    }
        //}


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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
