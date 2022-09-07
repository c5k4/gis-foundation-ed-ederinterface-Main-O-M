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
    public partial class frmRebuildDuctdefinition : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        List<string> lstTabNames = new List<string>();
        object _au;

        string strSubtype = string.Empty;
        string strLastUser = string.Empty;
        string strCreationUser = string.Empty;
        string strConduit_convid = string.Empty;
        string strConduit_DuctCnt = string.Empty;
        string strStatus = string.Empty;
        string strConduit_Globalid = string.Empty;

        int intPge_DuctCntIdx = -1;
        int intPge_DUCTSIZEIdx = -1;
        int intPge_AVAILABLEIdx = -1;
        int intPge_MaterialIdx = -1;
        int intUpdCount = 0;

        public frmRebuildDuctdefinition()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                addWhereClause();
                FolderBrowserDialog fldrDia = new FolderBrowserDialog();
                fldrDia.Description = "Select FGD database for PGEDuctDefinition generation..";
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

        List<string> whereClauses = new List<string>();
        int totalTransformersToProcess = 0;

        private void addWhereClause()
        {
            //Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
            IFeatureClass pFC = null;
            IFeatureCursor pFCursor = null;
            IQueryFilter pFilter = new QueryFilterClass();
            IFeature pfeat = null;
            int intRecCount = 0;
            int intMin = 0;
            int intMax = 0;
            int iterationCount = 0;
            int inOid = 0;
            List<string> lstString = new List<string>();

            try
            {
                cmbWhereClause.Enabled = true;
                stbInfo.Text = "Please wait..Processing.. adding Where Clause..it take couple of mints ";
                Application.DoEvents();

                btnStart.Enabled = false;
                btnExit.Enabled = false;

                pFC = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.ConduitSystem");
                intRecCount = pFC.FeatureCount(null);
                pFilter.WhereClause = "SUBTYPECD NOT IN ('1','3') order by objectid ";
                pFilter.SubFields = pFC.OIDFieldName;
                pFCursor = pFC.Search(pFilter, false);

                List<int> oids = new List<int>();
                IFeature transformerFeat = null;
                while ((transformerFeat = pFCursor.NextFeature()) != null)
                {
                    oids.Add(transformerFeat.OID);
                    if (transformerFeat != null) { while (Marshal.ReleaseComObject(transformerFeat) > 0) { } }
                }

                if (pFilter != null) { while (Marshal.ReleaseComObject(pFilter) > 0) { } }
                if (pFCursor != null) { while (Marshal.ReleaseComObject(pFCursor) > 0) { } }

                oids.Sort();

                int i = 0;
                int increment = 25000;
                totalTransformersToProcess = increment;

                for (i = 0; i < oids.Count; i += increment)
                {
                    if (oids.Count < i + increment)
                    {
                        whereClauses.Add("Objectid >= " + oids[i] + " and Objectid <= " + oids[oids.Count - 1]);
                        cmbWhereClause.Items.Add("Objectid >= " + oids[i] + " and Objectid <= " + oids[oids.Count - 1]);
                    }
                    else
                    {
                        whereClauses.Add("Objectid >= " + oids[i] + " and Objectid < " + oids[i + increment]);
                        cmbWhereClause.Items.Add("Objectid >= " + oids[i] + " and Objectid < " + oids[i + increment]);
                    }
                }

                stbInfo.Text = "Select WhereClause and click Run button";
                Application.DoEvents();
                btnStart.Enabled = true;
                btnExit.Enabled = true;
                //Logs.writeLine("End Date and Time  :" + System.DateTime.Now);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error @ cmbAllFC_SelectedIndexChanged  " + ex.Message);
            }
            finally
            {
                if (pFCursor != null)
                {
                    Marshal.ReleaseComObject(pFCursor);
                }
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
                EnableRequiredAutoupdaters();
                #endregion

                string strChildTabName = string.Empty;
                objReportTable.Clear();
                objReportTable.Columns.Clear();

                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FEATURECLASS_NAME,GLOBALID,CREATIONUSER,LASTUSER,CONVERSIONID,CONDUITSYSTEM_DUCTCOUNT,CHILDTABNAME,EXISTING_DEF_COUNT,CREATED_UNIT_COUNT,REMARKS");
                IWorkspace pSrcWsp = null;
                IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                pSrcWsp = workspaceFactory.OpenFromFile(txtFGDpath.Text, 0);

                IFeatureWorkspace pSrcFws = (IFeatureWorkspace)pSrcWsp;
                Logs.writeLine("opend FGDB workspace");
                ITable pSrc_ductDefTab = pSrcFws.OpenTable("DuctDefinition");
                Logs.writeLine("Read DuctDefinition table from input FGDB ");

                string strWhereClause = string.Empty;
                string strfileName = string.Empty;

                strWhereClause = cmbWhereClause.SelectedItem.ToString().Replace(">", "_");
                strWhereClause = strWhereClause.Replace("<", "_");
                strfileName = strWhereClause;

                if (chkDel.CheckState == CheckState.Checked)
                {
                    Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
                    clsTestWorkSpace.StartEditOperation();
                    deleteRelForDuctBankAndCIC();
                    clsTestWorkSpace.StopEditOperation(true);
                    Logs.writeLine("END Date and Time  :" + System.DateTime.Now);
                }

                Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
                clsTestWorkSpace.StartEditOperation();
                startprocess(pSrc_ductDefTab, strWhereClause);
                clsTestWorkSpace.StopEditOperation(true);
                Logs.writeLine("END Date and Time  :" + System.DateTime.Now);


                //if (cmbAllFC.SelectedItem.ToString() == "ALL")
                //{
                //    Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
                //    //addTabNam_toLst(ref lstTabNames, "ALL");
                //    for (int i = 0; i < lstTabNames.Count; i++)
                //    {
                //        clsTestWorkSpace.StartEditOperation();
                //        deleteRelForDuctBankAndCIC();
                //        clsTestWorkSpace.StopEditOperation(true);


                //        clsTestWorkSpace.StartEditOperation();                        
                //        startprocess(lstTabNames[i], pSrc_ductDefTab);
                //        clsTestWorkSpace.StopEditOperation(true);
                //    }
                //    Logs.writeLine("END Date and Time  :" + System.DateTime.Now);
                //}
                //else
                //{
                //    //Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
                //    //clsTestWorkSpace.StartEditOperation();
                //    //deleteRelForDuctBankAndCIC();
                //    //clsTestWorkSpace.StopEditOperation(true);

                //    clsTestWorkSpace.StartEditOperation();                    
                //    startprocess(cmbAllFC.SelectedItem.ToString(), pSrc_ductDefTab);
                //    clsTestWorkSpace.StopEditOperation(true);
                //    Logs.writeLine("END Date and Time  :" + System.DateTime.Now);
                //}


                stbInfo.Text = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "RebuildDuctdefinition_" + strfileName);
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

        private void deleteRelForDuctBankAndCIC()
        {

            ITable pConduitSystemTab = null;
            //ITable pDuctDeftab = null;
            ITable pPgeDuctDeftab = null;
            IRow pConduitRow = null;
            int intiterationCount = 0;
            int intTotCount = 0;

            ICursor pCursor = null;
            string strSrc_Duct_ConvVal = string.Empty;
            string strSubtype = string.Empty;
            string strStatus = string.Empty;
            intUpdCount = 0;


            try
            {

                pPgeDuctDeftab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.PGEDuctDefinition"));
                pConduitSystemTab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.ConduitSystem"));
                IRelationshipClass pRel_Conduit_pgeductDef = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.ConduitSystem_PGEDuctDef");
                //IRelationshipClass pRel_Conduit_pgeductDef = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass(strRelTabName);
                //IRelationshipClass pRel_Conduit_pgeductDef = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.ConduitSystem_PGEDuctDef");

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    IQueryFilter pFilter = new QueryFilterClass();
                    pFilter.WhereClause = "SUBTYPECD IN ('1','3')";
                    intTotCount = pConduitSystemTab.RowCount(pFilter);
                    pCursor = pConduitSystemTab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pConduitRow = pCursor.NextRow();

                    while (pConduitRow != null)
                    {
                        try
                        {
                            intUpdCount++;
                            //strSrc_Duct_ConvVal = _clsGlobalFunctions.Cast(pSrc_DuctDif_Row.get_Value(intSrc_Duct_ConvIdx).ToString(), string.Empty);
                            intiterationCount++;
                            stbInfo.Text = "processing...." + intiterationCount + " of " + intTotCount;
                            Application.DoEvents();
                            strStatus = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("STATUS")).ToString(), string.Empty);
                            strSubtype = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("SUBTYPECD")).ToString(), string.Empty);

                            ISet pset = pRel_Conduit_pgeductDef.GetObjectsRelatedToObject((IObject)pConduitRow);
                            if (pset.Count > 0)
                            {
                                deleteExistingDefRecords(pPgeDuctDeftab, pConduitRow.OID.ToString());
                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + pConduitRow.get_Value(pConduitRow.Fields.FindField("GLOBALID")).ToString() + ",,,,,,,, SUBTYPECD :" + strSubtype + " : Related PGEDuctDefinition reords delected to this conduit system");

                                enabelAu((IObject)pConduitRow, mmAutoUpdaterMode.mmAUMStandAlone, mmEditEvent.mmEventFeatureUpdate);
                                if (strStatus != "")
                                {
                                    pConduitRow.set_Value(pConduitRow.Fields.FindField("STATUS"), strStatus);
                                }
                                else
                                {
                                    pConduitRow.set_Value(pConduitRow.Fields.FindField("SUBTYPECD"), strSubtype);
                                }
                                pConduitRow.Store();
                            }


                            if (intUpdCount > 500)
                            {
                                clsTestWorkSpace.StopEditOperation(true);
                                clsTestWorkSpace.StartEditOperation();
                                intUpdCount = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logs.writeLine("  #deleteRelForDuctBankAndCIC Error at objectid : " + pConduitRow.OID + " " + ex.Message);
                        }
                        pConduitRow = pCursor.NextRow();
                        //pConduitRow = null;
                    }
                }
            }

            catch (Exception ex)
            {
                Logs.writeLine("  #deleteRelForDuctBankAndCIC " + ex.Message);
            }
            finally
            {
                pPgeDuctDeftab = null;
                pConduitSystemTab = null;
            }
        }

        private void startprocess(ITable pSrc_ductDefTab, string strWhereClause)
        {

            ITable pConduitSystemTab = null;
            //ITable pDuctDeftab = null;
            ITable pPgeDuctDeftab = null;
            IRow pConduitRow = null;
            int intiterationCount = 0;
            int intTotCount = 0;

            ICursor pCursor = null;
            IRow pSrc_DuctDif_Row = null;

            string strSrc_Duct_ConvVal = string.Empty;


            int intConduit_DuctCnt = 0;
            IQueryFilter pFilter = new QueryFilterClass();
            int intExistRecCnt = 0;
            List<string> lstUsersList = new List<string>();
            ISet pPgeDuctDefset = null;
            Boolean bolRecUpdated = false;
            int intpgeDuctDef_Upd_Doctcount = 0;
            string strConduitSystem_DuctCnt = string.Empty;

            try
            {
                addUsersToList(ref lstUsersList);
                string strDefTabName = "EDGIS.PGEDuctDefinition";
                string strRelTabName = "EDGIS.ConduitSystem_PGEDuctDef";
                //pDuctDeftab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.DuctDefinition"));
                pPgeDuctDeftab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable(strDefTabName));
                pConduitSystemTab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.ConduitSystem"));
                IRelationshipClass pRel_Conduit_pgeductDef = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass(strRelTabName);
                //IRelationshipClass pRel_Conduit_pgeductDef = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.ConduitSystem_PGEDuctDef");
                //IDataStatistics
                int intSrc_Duct_ConvIdx = pSrc_ductDefTab.FindField("CONDUITCONVERSIONID"); //centralvally,


                intPge_DuctCntIdx = pPgeDuctDeftab.Fields.FindField("DUCTCOUNT");
                intPge_DUCTSIZEIdx = pPgeDuctDeftab.Fields.FindField("DUCTSIZE");
                intPge_AVAILABLEIdx = pPgeDuctDeftab.Fields.FindField("AVAILABLE");
                intPge_MaterialIdx = pPgeDuctDeftab.Fields.FindField("MATERIAL");


                if (intPge_DuctCntIdx == -1 || intPge_DUCTSIZEIdx == -1 || intPge_AVAILABLEIdx == -1 || intPge_MaterialIdx == -1 || intSrc_Duct_ConvIdx == -1)
                {
                    MessageBox.Show("One of the field DUCTCOUNT,DUCTSIZE,AVAILABLE,MATERIAL not found in PGEDuctDefinition table", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }


                //int intSrc_Duct_ConvIdx = pSrc_ductDefTab.FindField("CONDUITSYSTEMCONVERSIONID");
                stbInfo.Text = "processing wait...";
                Application.DoEvents();
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pFilter.WhereClause = cmbWhereClause.SelectedItem.ToString() + " and SUBTYPECD NOT IN ('1','3')";
                    //pFilter.WhereClause = " globalid ='{E3979FE5-C936-4802-910B-7C33F79A1817}'";


                    //pFilter.WhereClause = " globalid in ('{0CB858D7-D499-4865-AF8C-5A885C1F862B}','{105FDC00-0461-4947-9152-FBA8790FA402}','{10BC5EF9-08ED-46EE-A792-AF69DFE15BA3}','{10DEC6F7-7E29-44AF-BF3E-AAFB6B1CEA28}','{1280B40A-34C1-4661-9556-C0886F50B8EA}','{12B19C22-1ADC-41F6-AAE4-48A6A6CE3BB6}','{174BE6EB-CD7F-4590-B84F-5F31489518FA}','{17FFBD2D-FE52-41A6-B5A5-3CDE7364C3AA}','{18FDBC9B-121A-43AD-83C0-1B03FBC06122}','{19FE2754-6317-468E-9846-9DDE256A5E42}')";
                    //pFilter.WhereClause = " globalid in ('{18FDBC9B-121A-43AD-83C0-1B03FBC06122}','{19FE2754-6317-468E-9846-9DDE256A5E42}')";                    

                    intTotCount = pConduitSystemTab.RowCount(pFilter);
                    pCursor = pConduitSystemTab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pConduitRow = pCursor.NextRow();
                    while (pConduitRow != null)
                    {
                        try
                        {
                            //strSrc_Duct_ConvVal = _clsGlobalFunctions.Cast(pSrc_DuctDif_Row.get_Value(intSrc_Duct_ConvIdx).ToString(), string.Empty);                            
                            intiterationCount++;
                            stbInfo.Text = "processing...." + intiterationCount + " of " + intTotCount;
                            Application.DoEvents();

                            //1	Duct Bank,2	Conduit,3	CIC
                            strConduit_Globalid = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("GLOBALID")).ToString(), string.Empty);
                            strSubtype = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("SUBTYPECD")).ToString(), string.Empty);
                            strLastUser = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("lastuser")).ToString(), string.Empty);
                            strCreationUser = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("CREATIONUSER")).ToString(), string.Empty);
                            strConduit_convid = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("CONVERSIONID")).ToString(), string.Empty);
                            strConduit_DuctCnt = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("DUCTCOUNT")).ToString(), string.Empty);
                            strStatus = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("STATUS")).ToString(), string.Empty);
                            strConduitSystem_DuctCnt = _clsGlobalFunctions.Cast(pConduitRow.get_Value(pConduitRow.Fields.FindField("DUCTCOUNT")).ToString(), string.Empty);

                            pPgeDuctDefset = pRel_Conduit_pgeductDef.GetObjectsRelatedToObject((IObject)pConduitRow);
                            intExistRecCnt = pPgeDuctDefset.Count;
                            //Logs.writeLine(strConduit_Globalid);

                            if (lstUsersList.Contains(strCreationUser.ToUpper()) || lstUsersList.Contains(strLastUser.ToUpper()))
                            {
                                if (intExistRecCnt != 0)
                                {
                                    //group by size, material and availability create # of duct definitons based on the grouping
                                    bolRecUpdated = false;
                                    validateAndUpdductcount(pPgeDuctDefset, pPgeDuctDeftab, strConduit_Globalid,strConduitSystem_DuctCnt, ref bolRecUpdated);
                                    //FEATURECLASS_NAME,GLOBALID,CREATIONUSER,LASTUSER,CONVERSIONID,DUCTCOUNT,CHILDTABNAME,EXISTING_DEF_COUNT,CREATED_UNIT_COUNT,REMARKS 
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + strConduit_Globalid + "," + strCreationUser + "," + strLastUser + "," + strConduit_convid + "," + strConduit_DuctCnt + "," + strDefTabName + "," + intExistRecCnt + ",, Its Mapper feature.PGE_DUCTDEFINITION->DUCTCOUNT attribute updated");
                                }
                                else
                                {
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + strConduit_Globalid + "," + strCreationUser + "," + strLastUser + "," + strConduit_convid + "," + strConduit_DuctCnt + "," + strDefTabName + "," + intExistRecCnt + ",, Its Mapper feature.No PGE_DUCTDEFINITION record related to this conduit system");
                                }
                            }
                            else if (strConduit_convid.Trim() == "")
                            {
                                if (intExistRecCnt != 0)
                                {
                                    //group by size, material and availability create # of duct definitons based on the grouping
                                    bolRecUpdated = false;

                                    validateAndUpdductcount(pPgeDuctDefset, pPgeDuctDeftab, strConduit_Globalid,strConduitSystem_DuctCnt, ref bolRecUpdated);
                                    //FEATURECLASS_NAME,GLOBALID,CREATIONUSER,LASTUSER,CONVERSIONID,DUCTCOUNT,CHILDTABNAME,EXISTING_DEF_COUNT,CREATED_UNIT_COUNT,REMARKS 
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + strConduit_Globalid + "," + strCreationUser + "," + strLastUser + "," + strConduit_convid + "," + strConduit_DuctCnt + "," + strDefTabName + "," + intExistRecCnt + "," + "0" + ",Conduit conversionid is null. PGE_DUCTDEFINITION->DUCTCOUNT attribute updated ");
                                }
                                else
                                {
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + strConduit_Globalid + "," + strCreationUser + "," + strLastUser + "," + strConduit_convid + "," + strConduit_DuctCnt + "," + strDefTabName + "," + intExistRecCnt + "," + "0" + ",Conduit conversionid is null.No PGE_DUCTDEFINITION record related to this conduit system ");
                                }

                            }
                            else
                            {

                                pSrc_DuctDif_Row = getSrcCondFeatWithConvId(strConduit_convid, pSrc_ductDefTab);
                                if (pSrc_DuctDif_Row != null)
                                {
                                    deleteExistingDefRecords(pPgeDuctDeftab, pConduitRow.OID.ToString());
                                    insertDefRecord(pSrc_DuctDif_Row, pPgeDuctDeftab, pConduitRow.OID.ToString(), strConduit_convid, strConduitSystem_DuctCnt);

                                    if (pPgeDuctDefset != null) { while (Marshal.ReleaseComObject(pPgeDuctDefset) > 0) { } }
                                    pPgeDuctDefset = null;
                                    //pPgeDuctDefset = pRel_Conduit_pgeductDef.GetObjectsRelatedToObject((IObject)pConduitRow);

                                    //group by size, material and availability create # of duct definitons based on the grouping
                                    //bolRecUpdated = false;/
                                    //validateAndUpdductcount(pPgeDuctDefset, pPgeDuctDeftab, strConduit_Globalid, ref bolRecUpdated);
                                    intUpdCount++;
                                    //FEATURECLASS_NAME,GLOBALID,CREATIONUSER,LASTUSER,CONVERSIONID,DUCTCOUNT,CHILDTABNAME,EXISTING_DEF_COUNT,CREATED_UNIT_COUNT,REMARKS 
                                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + strConduit_Globalid + "," + strCreationUser + "," + strLastUser + "," + strConduit_convid + "," + strConduit_DuctCnt + "," + strDefTabName + "," + intExistRecCnt + "," + "1" + ", PGE_DUCTDEFINITION Unit record created as per input table and PGE_DUCTDEFINITION->DUCTCOUNT attribute updated ");

                                }
                                else
                                {
                                    if (intExistRecCnt != 0)
                                    {
                                        //Input table not having Ductrecord with this conversionid
                                        //group by size, material and availability create # of duct definitons based on the grouping
                                        bolRecUpdated = false;

                                        validateAndUpdductcount(pPgeDuctDefset, pPgeDuctDeftab, strConduit_Globalid,strConduitSystem_DuctCnt, ref bolRecUpdated);
                                        //FEATURECLASS_NAME,GLOBALID,CREATIONUSER,LASTUSER,CONVERSIONID,DUCTCOUNT,CHILDTABNAME,EXISTING_DEF_COUNT,CREATED_UNIT_COUNT,REMARKS 
                                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + pConduitRow.get_Value(pConduitRow.Fields.FindField("GLOBALID")).ToString() + "," + strCreationUser + "," + strLastUser + "," + strConduit_convid + "," + strConduit_DuctCnt + "," + strDefTabName + "," + intExistRecCnt + ",0,Input PGE_DUCTDEFINITION table not having record with this conversionid. PGE_DUCTDEFINITION->DUCTCOUNT attribute updated. ");
                                    }
                                    else
                                    {
                                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pConduitRow.Table).Name + "," + pConduitRow.get_Value(pConduitRow.Fields.FindField("GLOBALID")).ToString() + "," + strCreationUser + "," + strLastUser + "," + strConduit_convid + "," + strConduit_DuctCnt + "," + strDefTabName + "," + intExistRecCnt + ",0,Input PGE_DUCTDEFINITION table not having record with this conversionid. No PGE_DUCTDEFINITION record related to this conduit system ");
                                    }

                                }
                                if (pSrc_DuctDif_Row != null) { while (Marshal.ReleaseComObject(pSrc_DuctDif_Row) > 0) { } }
                            }
                            enabelAu((IObject)pConduitRow, mmAutoUpdaterMode.mmAUMStandAlone, mmEditEvent.mmEventFeatureUpdate);
                            if (strStatus != "")
                            {
                                pConduitRow.set_Value(pConduitRow.Fields.FindField("STATUS"), strStatus);
                            }
                            else
                            {
                                pConduitRow.set_Value(pConduitRow.Fields.FindField("SUBTYPECD"), strSubtype);
                            }
                            pConduitRow.Store();

                            if (intUpdCount > 500)
                            {
                                clsTestWorkSpace.StopEditOperation(true);
                                clsTestWorkSpace.StartEditOperation();
                                intUpdCount = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logs.writeLine(" #startprocess : Conduit system GLOBALID:" + strConduit_Globalid + "  " + ex.Message);

                        }
                        if (pConduitRow != null) { while (Marshal.ReleaseComObject(pConduitRow) > 0) { } }
                        pConduitRow = pCursor.NextRow();

                        //pConduitRow = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("  #startprocess " + ex.Message);
            }
        }

        private void validateAndUpdductcount(ISet pPgeDuctDefset, ITable pPgeDuctDeftab, string strConduitGlobalid,string strConduitSystem_DuctCnt, ref Boolean bolRecUpdated)
        {
            string strPge_DuctCntVal = string.Empty;
            string strPge_AVAILABLEVal = string.Empty;
            string strPge_MaterialVal = string.Empty;
            string strPge_DUCTSIZEVal = string.Empty;
            string strCombinedVals = string.Empty;
            bolRecUpdated = false;
            int intPgeDefCount = 0;


            Dictionary<string, List<int>> defDist = new System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>();
            intPgeDefCount = pPgeDuctDefset.Count;
            IRow pPgeDuctDefRow = null;
            pPgeDuctDefset.Reset();
            pPgeDuctDefRow = (IRow)pPgeDuctDefset.Next();

            try
            {
                //check pgeductdefinition record count is 1 then check pgeductdefinition.ductcount is value is '1',if not update with '1'
                if (pPgeDuctDefset.Count == 1)
                {
                    strPge_DuctCntVal = _clsGlobalFunctions.Cast(pPgeDuctDefRow.get_Value(intPge_DuctCntIdx), string.Empty);

                    if (strConduitSystem_DuctCnt != "")
                    {
                        pPgeDuctDefRow.set_Value(intPge_DuctCntIdx, strConduitSystem_DuctCnt);
                        pPgeDuctDefRow.Store();
                        bolRecUpdated = true;
                        intUpdCount++;
                    }
                    else if (strPge_DuctCntVal != "1")
                    {

                        pPgeDuctDefRow.set_Value(intPge_DuctCntIdx, "1");
                        pPgeDuctDefRow.Store();
                        bolRecUpdated = true;
                        intUpdCount++;
                        ////FEATURECLASS_NAME,GLOBALID,PGEDEF_COUNT,PGEDEF_DUCTCOUNT,REMARKS
                        //_clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "CONDUITSYSTEM" + "," + strConduitGlobalid + "," + intPgeDefCount + "," + strPge_DuctCntVal + ", PgeDuctDefinition record updated ");
                    }
                }
                else
                {
                    //check pgeductdefinition record count is > 1 then 
                    //grouping PgeDuctdefinitions based on attributes DUCTSIZE,Material,AVAILABLE
                    while (pPgeDuctDefRow != null)
                    {
                        strPge_DuctCntVal = _clsGlobalFunctions.Cast(pPgeDuctDefRow.get_Value(intPge_DuctCntIdx), string.Empty);
                        strPge_AVAILABLEVal = _clsGlobalFunctions.Cast(pPgeDuctDefRow.get_Value(intPge_AVAILABLEIdx), string.Empty);
                        strPge_MaterialVal = _clsGlobalFunctions.Cast(pPgeDuctDefRow.get_Value(intPge_MaterialIdx), string.Empty);
                        strPge_DUCTSIZEVal = _clsGlobalFunctions.Cast(pPgeDuctDefRow.get_Value(intPge_DUCTSIZEIdx), string.Empty);

                        strCombinedVals = strPge_DUCTSIZEVal + strPge_MaterialVal + strPge_AVAILABLEVal;

                        if (strCombinedVals != "")
                        {
                            if (!defDist.ContainsKey(strCombinedVals))
                            {
                                List<int> lstOIds = new List<int>();
                                lstOIds.Add(pPgeDuctDefRow.OID);
                                defDist.Add(strCombinedVals, lstOIds);
                            }
                            else
                            {
                                List<int> lstOIds = defDist[strCombinedVals];
                                lstOIds.Add(pPgeDuctDefRow.OID);
                            }
                        }
                        if (pPgeDuctDefRow != null) { while (Marshal.ReleaseComObject(pPgeDuctDefRow) > 0) { } }
                        pPgeDuctDefRow = (IRow)pPgeDuctDefset.Next();
                    }


                    foreach (KeyValuePair<string, List<int>> keyVal in defDist)
                    {
                        List<int> lstOIds = keyVal.Value;

                        //if only one ductdefinition then validate and update ductcount as '1'
                        if (lstOIds.Count == 1)
                        {
                            pPgeDuctDefRow = pPgeDuctDeftab.GetRow(lstOIds[0]);
                            strPge_DuctCntVal = _clsGlobalFunctions.Cast(pPgeDuctDefRow.get_Value(intPge_DuctCntIdx), string.Empty);

                            if (strPge_DuctCntVal != "1")
                            {

                                pPgeDuctDefRow.set_Value(intPge_DuctCntIdx, "1");
                                pPgeDuctDefRow.Store();
                                bolRecUpdated = true;
                                intUpdCount++;
                                ////FEATURECLASS_NAME,GLOBALID,PGEDEF_COUNT,PGEDEF_DUCTCOUNT,REMARKS
                                //_clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "CONDUITSYSTEM" + "," + strConduitGlobalid + "," + intPgeDefCount + "," + strPge_DuctCntVal + ", PgeDuctDefinition record updated ");

                            }
                            if (pPgeDuctDefRow != null) { while (Marshal.ReleaseComObject(pPgeDuctDefRow) > 0) { } }
                        }
                        else if (lstOIds.Count > 1)
                        {
                            //if more than one ductdefinition are there with combination of (size,material,availability) then 
                            //validate and update first ductdefinition with ductcount as lstOIds[0].count and delete the remaining ductdefinition records.

                            for (int i = 0; i < lstOIds.Count; i++)
                            {
                                //first definition
                                if (i == 0)
                                {
                                    pPgeDuctDefRow = pPgeDuctDeftab.GetRow(lstOIds[i]);
                                    strPge_DuctCntVal = _clsGlobalFunctions.Cast(pPgeDuctDefRow.get_Value(intPge_DuctCntIdx), string.Empty);

                                    if (strPge_DuctCntVal != lstOIds.Count.ToString())
                                    {

                                        pPgeDuctDefRow.set_Value(intPge_DuctCntIdx, lstOIds.Count);
                                        pPgeDuctDefRow.Store();
                                        bolRecUpdated = true;
                                        intUpdCount++;
                                        ////FEATURECLASS_NAME,GLOBALID,PGEDEF_COUNT,PGEDEF_DUCTCOUNT,REMARKS
                                        //_clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "CONDUITSYSTEM" + "," + strConduitGlobalid + "," + intPgeDefCount + "," + lstOIds.Count + ", PgeDuctDefinition record updated ");
                                    }
                                }
                                else
                                {
                                    //Delete other than first definition
                                    pPgeDuctDefRow = pPgeDuctDeftab.GetRow(lstOIds[i]);
                                    pPgeDuctDefRow.Delete();
                                    bolRecUpdated = true;
                                    intUpdCount++;
                                    ////FEATURECLASS_NAME,GLOBALID,PGEDEF_COUNT,PGEDEF_DUCTCOUNT,REMARKS
                                    //_clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "CONDUITSYSTEM" + "," + strConduitGlobalid + "," + intPgeDefCount + "," + strPge_DuctCntVal + ", PgeDuctDefinition record deleted ");
                                }
                                if (pPgeDuctDefRow != null) { while (Marshal.ReleaseComObject(pPgeDuctDefRow) > 0) { } }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine(" Error in validateAndUpdductcount. ConduitGlobalid:" + strConduitGlobalid + " " + ex.Message);
            }
        }




        private void addUsersToList(ref List<string> lstUsersList)
        {
            try
            {
                string strPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                strPath = strPath + "\\MappersList.txt";
                TextReader tx = new StreamReader(strPath);
                string s = tx.ReadLine();
                while (s != null)
                {
                    lstUsersList.Add(s.ToUpper());
                    s = tx.ReadLine();
                }
                tx.Close();
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at adding mappers list from the input txt file #addUsersToList " + ex.Message);
            }

        }

        private void insertDefRecord(IRow pSrc_DuctDif_Row, ITable pPgeDuctDeftab, string strConductOid, string strConduitConvId, string strConduitSystem_DuctCnt)
        {
            IRowBuffer pRowBuffer = null;
            ICursor pInserCursor = null;
            string strINSTALLATIONDATEVal = string.Empty;

            try
            {
                pRowBuffer = pPgeDuctDeftab.CreateRowBuffer();
                for (int i = 0; i < pPgeDuctDeftab.Fields.FieldCount; i++)
                {
                    int sourcefieldindex = -1;
                    int destinationfieldindex = -1;
                    IField objfield = pPgeDuctDeftab.Fields.get_Field(i);
                    try
                    {
                        if (objfield.Editable)
                        {
                            destinationfieldindex = pPgeDuctDeftab.Fields.FindField(objfield.Name);
                            sourcefieldindex = pSrc_DuctDif_Row.Fields.FindField(objfield.Name);

                            if (destinationfieldindex != -1 && sourcefieldindex != -1)
                            {
                                if (objfield.Name.ToUpper() != "CONDUITSYSTEMOBJECTID" && objfield.Name.ToUpper() != "SUBTYPECD")
                                {
                                    //if (objfield.Name.ToUpper() == "MATERIAL")
                                    //{
                                    //    pRowBuffer.set_Value(destinationfieldindex, "STL");
                                    //}
                                    //else
                                    //{
                                    pRowBuffer.set_Value(destinationfieldindex, pSrc_DuctDif_Row.get_Value(sourcefieldindex));
                                    //}
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logs.writeLine("Error @ retriving value :" + objfield.Name + " Conduit ConvId : " + strConduitConvId + " " + ex.Message);
                    }
                }
                pRowBuffer.set_Value(pPgeDuctDeftab.Fields.FindField("SUBTYPECD"), "2");
                if (strConduitSystem_DuctCnt != "")
                {
                    pRowBuffer.set_Value(pPgeDuctDeftab.Fields.FindField("DUCTCOUNT"), strConduitSystem_DuctCnt);
                }
                else
                {
                    pRowBuffer.set_Value(pPgeDuctDeftab.Fields.FindField("DUCTCOUNT"), "1");
                }
                pRowBuffer.set_Value(pPgeDuctDeftab.Fields.FindField("CONDUITSYSTEMOBJECTID"), strConductOid);
                pInserCursor = pPgeDuctDeftab.Insert(false);
                pInserCursor.InsertRow(pRowBuffer);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at insert RECORD for Conduit ConvId :" + strConduitConvId + "  " + ex.Message);
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

        private void deleteExistingDefRecords(ITable pPgeDuctDeftab, string strConduitOid)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                pFilter.WhereClause = "CONDUITSYSTEMOBJECTID='" + strConduitOid + "'";
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pPgeDuctDeftab.Search(pFilter, false);
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
                Logs.writeLine("Error at deleteExistingDefRecords for Conduit OidCEDSADEVICEID:" + strConduitOid + " " + ex.Message);
            }
        }
        private void enabelAu(IObject pObject, mmAutoUpdaterMode pmmAutoUpdaterMode, mmEditEvent pmmEditEvent)
        {
            try
            {
                if (_au is IMMSpecialAUStrategy)
                    (_au as IMMSpecialAUStrategy).Execute(pObject);
                else if (_au is IMMSpecialAUStrategyEx)
                    (_au as IMMSpecialAUStrategyEx).Execute(pObject, pmmAutoUpdaterMode, pmmEditEvent);
                else if (_au is IMMAttrAUStrategy)
                    (_au as IMMAttrAUStrategy).GetAutoValue(pObject);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error enabelAu " + ex.Message);
            }
        }
        private IRow getSrcCondFeatWithConvId(string str_ConvVal, ITable pConduitSystemTab)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                pFilter.WhereClause = "CONDUITCONVERSIONID=" + str_ConvVal + "  ";

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pConduitSystemTab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("getCondFeatWithConvId " + ex.Message);
                return pRow;

            }
            return pRow;
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void EnableRequiredAutoupdaters()
        {
            try
            {
                object objAutoUpdater = null;
                objAutoUpdater = Activator.CreateInstance(Type.GetTypeFromProgID("Telvent.PGE.ED.ConduitLabel"));
                _au = objAutoUpdater;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Please check Telvent.PGE.ED.ConduitLabel not found" + ex.Message);
            }
        }

        private void frmRebuildDuctdefinition_Load(object sender, EventArgs e)
        {
            _clsGlobalFunctions.Common_initSummaryTable("RebuildDuctdefinition", "RebuildDuctdefinition");
        }

        private void chkDel_CheckedChanged(object sender, EventArgs e)
        {

        }
        //private void addWhereClause()
        //{
        //    //cmbAllFC.Items.Add("ALL");
        //    //cmbAllFC.Items.Add("EDGIS.DuctDefinition|EDGIS.ConduitSystem_DuctDefinition");
        //    //cmbAllFC.Items.Add("EDGIS.PGEDuctDefinition|EDGIS.ConduitSystem_PGEDuctDef");
        //    //lstTabNames.Add("EDGIS.DuctDefinition|EDGIS.ConduitSystem_DuctDefinition");
        //    //lstTabNames.Add("EDGIS.PGEDuctDefinition|EDGIS.ConduitSystem_PGEDuctDef");
        //    Logs.writeLine("Start Date and Time  :" + System.DateTime.Now);
        //    IFeatureClass pFC = null;
        //    IFeatureCursor pFCursor = null;
        //    IQueryFilter pFilter = new QueryFilterClass();
        //    IFeature pfeat = null;
        //    int intRecCount = 0;
        //    int intMin = 0;
        //    int intMax = 0;
        //    int iterationCount = 0;
        //    int inOid = 0;
        //    List<string> lstString = new List<string>();

        //    try
        //    {
        //        cmbWhereClause.Enabled = true;
        //        stbInfo.Text = "Processing wait... adding Where Clause..it take couple of mints ";
        //        Application.DoEvents();

        //        btnStart.Enabled = false;
        //        btnExit.Enabled = false;

        //        pFC = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.ConduitSystem");
        //        //pFC = _clsGlobalFunctions.getFeatureclassByName(pWS, "EDGIS.Transformer");
        //        intRecCount = pFC.FeatureCount(null);
        //        //SUBTYPECD NOT IN ('1','3') AND lastuser IS NULL AND CONVERSIONID IS NOT NULL
        //        //pFilter.WhereClause = "Objectid is not null order by objectid ";
        //        //pFilter.WhereClause = "SUBTYPECD NOT IN ('1','3') AND lastuser IS NULL AND CONVERSIONID IS NOT NULL order by objectid";
        //        pFilter.WhereClause = "SUBTYPECD NOT IN ('1','3') order by objectid";

        //        pFCursor = pFC.Search(pFilter, false);

        //        int int1 = pFC.FeatureCount(pFilter);

        //        IDataStatistics dataStatistics = new DataStatisticsClass();
        //        dataStatistics.Field = "Objectid";
        //        dataStatistics.Cursor = (ICursor)pFCursor;
        //        System.Collections.IEnumerator enumerator = dataStatistics.UniqueValues;
        //        enumerator.Reset();

        //        while (enumerator.MoveNext())
        //        {
        //            object myObject = enumerator.Current;
        //            //lstUniqueFldVals.Add(myObject.ToString());
        //            inOid = int.Parse(myObject.ToString());
        //            if (iterationCount == 0)
        //            {
        //                intMin = inOid;
        //            }
        //            else if (iterationCount == 25000)
        //            {
        //                intMax = inOid;
        //            }
        //            iterationCount++;

        //            if (iterationCount > 25000)
        //            {
        //                lstString.Add("Objectid > " + intMin + " and Objectid < " + intMax);
        //                cmbWhereClause.Items.Add("Objectid > " + intMin + " and Objectid < " + intMax);
        //                iterationCount = 0;
        //            }
        //        }

        //        if (iterationCount > 0 && iterationCount < 25000)
        //        {
        //            lstString.Add("Objectid > " + intMin + " and Objectid < " + inOid);
        //            cmbWhereClause.Items.Add("Objectid > " + intMin + " and Objectid < " + inOid);
        //        }
        //        cmbWhereClause.Text = lstString[0];

        //        stbInfo.Text = "Select WhereClause and click Run button";
        //        Application.DoEvents();
        //        btnStart.Enabled = true;
        //        btnExit.Enabled = true;
        //        Logs.writeLine("End Date and Time  :" + System.DateTime.Now);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error @ cmbAllFC_SelectedIndexChanged  " + ex.Message);
        //    }
        //    finally
        //    {
        //        if (pFCursor != null)
        //        {
        //            Marshal.ReleaseComObject(pFCursor);
        //        }
        //    }

        //}

    }
}
