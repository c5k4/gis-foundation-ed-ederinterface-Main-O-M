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
    public partial class updateLocalOfficeid_device : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        List<string> lstFcNamesList = new List<string>();

        IRelationshipClass pRel_Trans_Unit = null;
        IRelationshipClass pRel_VR_Unit = null;
        IRelationshipClass pRel_StepDown_Unit = null;

        List<string> lstTabNames = new List<string>();
        int intUpdCount = 0;
        int intUpdCntToStop = 0;


        public updateLocalOfficeid_device()
        {
            InitializeComponent();
        }

        private void updateLocalOfficeid_device_Load(object sender, EventArgs e)
        {

            getFcName(ref  lstFcNamesList);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {


            try
            {
                btnStart.Enabled = false;
                btnExit.Enabled = false;

                stbInfo.Text = "Starting Process..";

                //stbInfo.Text = "Disable Autoupdaters ";
                //#region "Disable Autoupdaters "
                //mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                //#endregion

                string strChildTabName = string.Empty;
                objReportTable.Clear();
                objReportTable.Columns.Clear();
                _clsGlobalFunctions.Common_initSummaryTable("Validate_LocalOfficeID_MapNum", "Validate_LocalOfficeID_MapNum");
                //_clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FCNAME,GLOBALID,LOCALOFFICEID,MAPNUMBER,TAB_QRY,A_QRY");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FCNAME,GLOBALID,SUBTYPECD,OLD_LOCALOFFICEID,NEW_LOCALOFFICEID,OLD_MAPNUMBER,NEW_MAPNUMBER,COUNTY,CIRCUITID,INSTALLATIONTYPE,STRUCTUREGUID,STATUS,CUSTOMEROWNED,UNIT_GLOBALID,DEVICEGROUPTYPE_CODE,DEVICEGROUPTYPE_DESC,TAB_QRY,A_QRY");

                IFeatureClass pLOPC_FC = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.PGE_LOPC");
                IFeatureClass pMaintenancePlat_FC = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.MaintenancePlat");

                pRel_Trans_Unit = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.Transformer_TransformerUnit");
                pRel_VR_Unit = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.VoltageReg_VoltageRegUnit");
                pRel_StepDown_Unit = clsTestWorkSpace.FeatureWorkspace.OpenRelationshipClass("EDGIS.StepDown_StepDownUnit");

                //getFcName(ref  lstFcNamesList);

                List<string> lstCheckedFCNames = new List<string>();
                for (int itr = 0; itr < chkFCListBox.CheckedItems.Count; itr++)
                {
                    startprocess(pLOPC_FC, pMaintenancePlat_FC, chkFCListBox.CheckedItems[itr].ToString());
                }

                string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                string strReporthpath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString() + strTime + "_Validate_LocalOfficeID_MapNum.CSV";
                generateLogReport(objReportTable, strReporthpath, "~", true);


                //public void DataTable2CSV(DataTable table, string filename, string sepChar,bool blnHeader)

                //clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "Validate_LocalOfficeID_MapNum");
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

        private void generateLogReport(DataTable table, string filename, string sepChar, bool blnHeader)
        {
            System.IO.StreamWriter writer = default(System.IO.StreamWriter);
            try
            {
                writer = new System.IO.StreamWriter(filename);

                // first write a line with the columns name
                string sep = "";
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                if (blnHeader)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        builder.Append(sep).Append(col.ColumnName);
                        sep = sepChar;
                    }
                }
                writer.WriteLine(builder.ToString());

                // then write all the rows
                foreach (DataRow row in table.Rows)
                {
                    sep = "";
                    builder = new System.Text.StringBuilder();

                    foreach (DataColumn col in table.Columns)
                    {
                        builder.Append(sep).Append(row[col.ColumnName]);
                        sep = sepChar;
                    }
                    writer.WriteLine(builder.ToString());
                }
            }
            finally
            {
                if ((writer != null)) writer.Close();
            }
        }

        private void getFcName(ref List<string> lstFcNamesList)
        {
            try
            {
                lstFcNamesList.Clear();
                string strPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                strPath = strPath + "\\LocaloffidMapnumber_fcnames.txt";
                TextReader tx = new StreamReader(strPath);
                string s = tx.ReadLine();
                while (s != null)
                {
                    lstFcNamesList.Add(s.ToUpper());
                    chkFCListBox.Items.Add(s);
                    s = tx.ReadLine();
                }
                tx.Close();
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at adding getFcName list from the input txt file 'LocaloffidMapnumber_fcnames.txt'  #getFcName " + ex.Message);
            }
        }

        private void startprocess(IFeatureClass pLOPC_FC, IFeatureClass pMaintenancePlat_FC, string strA_Tab_Name)
        {
            IFeatureCursor pFCursor = null;
            //IQueryFilter pqFilter = new QueryFilter();
            IRow pRow = null;

            IFeature pFeat = null;
            int intTotCount = 0;
            int intiterationCount = 0;
            string strLocOffVal = string.Empty;
            IFeatureClass pfc = null;
            IRelationshipClass pRel_Class = null;

            string strFldName = string.Empty;
            string strUnitGlobalid = string.Empty;
            string strGisDeviceGroupTyp = string.Empty;
            string strGisDeviceGroupCode = string.Empty;

            int intGis_LocalOfficeIdIdx = -1;
            int intGis_MapNumIdx = -1;
            int intGis_CountyIdx = -1;
            int intGis_CircuitIdIdx = -1;
            int intGis_InstallationTypIdx = -1;
            int intGis_StrtGuidIdx = -1;
            int intGis_SubtypecdIdx = -1;
            int intGis_StatusdIdx = -1;
            int intGis_CustOwnedIdx = -1;

            string strGis_LocalOfficeId = string.Empty;
            string strGis_MapNum = string.Empty;


            string strGis_County = string.Empty;
            string strGis_CircuitId = string.Empty;
            string strGis_InstallationTyp = string.Empty;
            string strGis_StrtGuid = string.Empty;
            string strGis_Subtypecd = string.Empty;
            string strGis_Status = string.Empty;
            string strGis_CustOwned = string.Empty;


            int intPoly_LocalOfficeIdIdx = -1;
            int intPoly_MapNum = -1;

            string strPoly_LocalOfficeId = string.Empty;
            string strPoly_MapNum = string.Empty;


            string strReportVals = string.Empty;
            List<string> lstUpdVals = new List<string>();
            string strUpdQry = string.Empty;
            string strA_UpdQry = string.Empty;
            string strGis_Globalid = string.Empty;
            IPoint pFeatPnt = null;

            string strTabName = string.Empty;
            string strA_TabName = string.Empty;
            try
            {

                strTabName = strA_Tab_Name.Split('|')[1];
                strA_TabName = strA_Tab_Name.Split('|')[0];
                pfc = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass(strTabName);
                intGis_LocalOfficeIdIdx = pfc.FindField("LOCALOFFICEID");
                intGis_MapNumIdx = pfc.FindField("DISTMAP");
                strFldName = "DISTMAP";
                if (intGis_MapNumIdx == -1)
                {
                    intGis_MapNumIdx = pfc.FindField("GEMSDISTMAPNUM");
                    strFldName = "GEMSDISTMAPNUM";
                    if (intGis_MapNumIdx == -1)
                    {
                        intGis_MapNumIdx = pfc.FindField("MAPNUMBER"); //Streetlight
                        strFldName = "MAPNUMBER";
                    }
                }

                intGis_CountyIdx = pfc.FindField("COUNTY");
                intGis_CircuitIdIdx = pfc.FindField("CIRCUITID");
                intGis_InstallationTypIdx = pfc.FindField("INSTALLATIONTYPE");
                intGis_StrtGuidIdx = pfc.FindField("STRUCTUREGUID");
                intGis_SubtypecdIdx = pfc.FindField("SUBTYPECD");
                intGis_StatusdIdx = pfc.FindField("STATUS");
                intGis_CustOwnedIdx = pfc.FindField("customerowned");


                intPoly_LocalOfficeIdIdx = pLOPC_FC.FindField("LOCALOFFICEID");
                intPoly_MapNum = pLOPC_FC.FindField("MAPNUMBER");

                if (strTabName == "EDGIS.TRANSFORMER") { pRel_Class = pRel_Trans_Unit; }
                if (strTabName == "EDGIS.VOLTAGEREGULATOR") { pRel_Class = pRel_VR_Unit; }
                if (strTabName == "EDGIS.STEPDOWN") { pRel_Class = pRel_StepDown_Unit; }

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    //pqFilter.WhereClause = strWhereClause;
                    intTotCount = pfc.FeatureCount(null);
                    pFCursor = pfc.Search(null, false);
                    cr.ManageLifetime(pFCursor);
                    pFeat = pFCursor.NextFeature();

                    while (pFeat != null)
                    {
                        try
                        {
                            intiterationCount++;
                            lstUpdVals.Clear();
                            stbInfo.Text = "processing...." + strTabName + " " + intiterationCount + " of " + intTotCount;
                            Application.DoEvents();
                            strGis_Globalid = _clsGlobalFunctions.Cast(pFeat.get_Value(pFeat.Fields.FindField("GLOBALID")).ToString(), string.Empty);

                            ////Validate field index and get the field value
                            if (intGis_LocalOfficeIdIdx != -1)
                            {
                                strGis_LocalOfficeId = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_LocalOfficeIdIdx).ToString(), string.Empty);
                            }
                            if (intGis_MapNumIdx != -1)
                            {
                                strGis_MapNum = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_MapNumIdx).ToString(), string.Empty);
                            }
                            if (intGis_CountyIdx != -1)
                            {
                                //County length should be 3 chars
                                strGis_County = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_CountyIdx).ToString(), string.Empty);
                                if (strGis_County.Length == 1) { strGis_County = "00" + strGis_County; }
                                if (strGis_County.Length == 2) { strGis_County = "0" + strGis_County; }
                            }
                            if (intGis_CircuitIdIdx != -1)
                            {
                                strGis_CircuitId = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_CircuitIdIdx).ToString(), string.Empty);
                            }

                            if (intGis_InstallationTypIdx != -1)
                            {
                                //PAD = PM , SUB=SS
                                strGis_InstallationTyp = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_InstallationTypIdx).ToString(), string.Empty);
                                if (strGis_InstallationTyp.Length > 0)
                                {
                                    if (strGis_InstallationTyp.ToUpper() == "PAD") { strGis_InstallationTyp = "PM"; }
                                    if (strGis_InstallationTyp.ToUpper() == "SUB") { strGis_InstallationTyp = "SS"; }
                                }
                            }

                            if (intGis_StrtGuidIdx != -1)
                            {
                                strGis_StrtGuid = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_StrtGuidIdx).ToString(), string.Empty);
                            }
                            if (intGis_SubtypecdIdx != -1)
                            {
                                strGis_Subtypecd = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_SubtypecdIdx).ToString(), string.Empty);
                            }

                            if (intGis_StatusdIdx != -1)
                            {
                                strGis_Status = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_StatusdIdx).ToString(), string.Empty);
                            }

                            if (intGis_CustOwnedIdx != -1)
                            {
                                strGis_CustOwned = _clsGlobalFunctions.Cast(pFeat.get_Value(intGis_CustOwnedIdx).ToString(), string.Empty);
                            }

                            //if (strGis_Globalid == "{16CC9DBC-1D4D-429A-8690-332C04769A56}")
                            //{ 

                            //}

                            //get point based on Geometry type
                            if (pfc.ShapeType == esriGeometryType.esriGeometryPoint)
                            {
                                pFeatPnt = (IPoint)pFeat.Shape;
                            }
                            else
                            {
                                pFeatPnt = ((IPointCollection)pFeat.ShapeCopy).get_Point(0);
                            }

                            //get LocalOfficeId
                            //getLocalOfficeId_mapnum(pFeatPnt, pLOPC_FC, ref strPoly_LocalOfficeId, ref strPoly_MapNum);
                            getLocalOfficeId_mapnum(pFeatPnt, pLOPC_FC, ref strPoly_LocalOfficeId, "LOCALOFFICEID");
                            getLocalOfficeId_mapnum(pFeatPnt, pMaintenancePlat_FC, ref strPoly_MapNum, "MAPNUMBER");

                            strUpdQry = "update " + strTabName + " ";
                            strA_UpdQry = "update " + strA_TabName + " ";

                            strReportVals = ((IDataset)pFeat.Class).Name + "," + strGis_Globalid + ",";
                            //LOCALOFFICEID
                            if (strGis_LocalOfficeId != strPoly_LocalOfficeId && intGis_LocalOfficeIdIdx != -1)
                            {
                                strReportVals = strReportVals + strPoly_LocalOfficeId + ",";
                                lstUpdVals.Add("LOCALOFFICEID ='" + strPoly_LocalOfficeId + "'");
                                //strUpdQry = strUpdQry + " set DISTRICT ='" + strLandbase_Districts + "'|";
                                //bolUpdReq = true;
                            }
                            else
                            {
                                strReportVals = strReportVals + ",";
                            }

                            //MAPNUMBER
                            if (strGis_MapNum != strPoly_MapNum && intGis_MapNumIdx != -1)
                            {
                                strReportVals = strReportVals + strPoly_MapNum + ",";
                                lstUpdVals.Add(strFldName + "='" + strPoly_MapNum + "'");
                                //strUpdQry = strUpdQry + " set DIVISION ='" + strLandbase_Divison + "'|";
                                //bolUpdReq = true;
                            }
                            else
                            {
                                strReportVals = strReportVals + ",";
                            }

                            formUpdQry(lstUpdVals, ref strUpdQry, ref strA_UpdQry, strGis_Globalid);

                            if (strUpdQry != "" && strTabName != "EDGIS.TRANSFORMER" && strTabName != "EDGIS.VOLTAGEREGULATOR" && strTabName != "EDGIS.STEPDOWN" && strTabName != "EDGIS.DEVICEGROUP")
                            {
                                //FCNAME,GLOBALID,SUBTYPECD,OLD_LOCALOFFICEID,NEW_LOCALOFFICEID,OLD_MAPNUMBER,NEW_MAPNUMBER,COUNTY,CIRCUITID,INSTALLATIONTYPE,STRUCTUREGUID,STATUS,CUSTOMEROWNED,UNITGLOBALID,TAB_QRY,A_QRY
                                strReportVals = ((IDataset)pFeat.Class).Name + "~" + strGis_Globalid + "~" + strGis_Subtypecd + "~" + strGis_LocalOfficeId + "~" + strPoly_LocalOfficeId + "~" + strGis_MapNum + "~" + strPoly_MapNum + "~" + strGis_County + "~" + strGis_CircuitId + "~" + strGis_InstallationTyp + "~" + strGis_StrtGuid + "~" + strGis_Status + "~" + strGis_CustOwned + "~~~~";
                                //_clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strReportVals + strUpdQry +","+ strA_UpdQry);
                                //strUpdQry = "";
                                //strA_UpdQry = "";

                                addRecToDatatab(objReportTable, strReportVals + strUpdQry + "~" + strA_UpdQry);
                            }
                            if (strUpdQry != "" && (strTabName == "EDGIS.TRANSFORMER" || strTabName == "EDGIS.VOLTAGEREGULATOR" || strTabName == "EDGIS.STEPDOWN"))
                            {
                                ISet pset = pRel_Class.GetObjectsRelatedToObject((IObject)pFeat);
                                pset.Reset();
                                pRow = (IRow)pset.Next();
                                while (pRow != null)
                                {
                                    try
                                    {
                                        strUnitGlobalid = _clsGlobalFunctions.Cast(pRow.get_Value(pRow.Fields.FindField("GLOBALID")), string.Empty);
                                        strReportVals = ((IDataset)pFeat.Class).Name + "~" + strGis_Globalid + "~" + strGis_Subtypecd + "~" + strGis_LocalOfficeId + "~" + strPoly_LocalOfficeId + "~" + strGis_MapNum + "~" + strPoly_MapNum + "~" + strGis_County + "~" + strGis_CircuitId + "~" + strGis_InstallationTyp + "~" + strGis_StrtGuid + "~" + strGis_Status + "~" + strGis_CustOwned + "~" + strUnitGlobalid + "~~~";
                                        addRecToDatatab(objReportTable, strReportVals + strUpdQry + "~" + strA_UpdQry);
                                        if (pRow != null) { while (Marshal.ReleaseComObject(pRow) > 0) { } }
                                    }
                                    catch (Exception ex) { Logs.writeLine("Error @ reporting unit Globalid info FC name :  " + strTabName + ex.Message); }

                                    pRow = (IRow)pset.Next();
                                }
                                if (pset != null) { while (Marshal.ReleaseComObject(pset) > 0) { } }
                                pset = null;
                            }

                            if (strUpdQry != "" && strTabName == "EDGIS.DEVICEGROUP")
                            {

                                getDeviceTyp(pFeat, strGis_Subtypecd, ref strGisDeviceGroupTyp, ref strGisDeviceGroupCode);
                                strReportVals = ((IDataset)pFeat.Class).Name + "~" + strGis_Globalid + "~" + strGis_Subtypecd + "~" + strGis_LocalOfficeId + "~" + strPoly_LocalOfficeId + "~" + strGis_MapNum + "~" + strPoly_MapNum + "~" + strGis_County + "~" + strGis_CircuitId + "~" + strGis_InstallationTyp + "~" + strGis_StrtGuid + "~" + strGis_Status + "~" + strGis_CustOwned + "~~" + strGisDeviceGroupCode + "~" + strGisDeviceGroupTyp + "~";

                                addRecToDatatab(objReportTable, strReportVals + strUpdQry + "~" + strA_UpdQry);

                            }

                            if (objReportTable.Rows.Count > 500000)
                            {
                                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "Localoffice_MapNum_report");
                                objReportTable.Rows.Clear();
                            }
                        }
                        catch (Exception ex)
                        {
                            Logs.writeLine("Error @ startprocess FC name :  " + strTabName);
                            //pFeat = pFCursor.NextFeature();
                        }
                        if (pFeat != null) { while (Marshal.ReleaseComObject(pFeat) > 0) { } }
                        pFeat = pFCursor.NextFeature();

                        //if (intiterationCount == 1000)
                        //{
                        //    pFeat = null;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on process start button " + ex.Message);
            }
            finally
            {
                if (pFCursor != null)
                    Marshal.ReleaseComObject(pFCursor);
            }
        }

        private void addRecToDatatab(DataTable objReportTable, string strReportValues)
        {
            try
            {
                DataRow pDataRow = objReportTable.NewRow();

                string[] strFldVals = strReportValues.Split('~');

                for (int x = 0; x < strFldVals.Length; x++)
                {
                    pDataRow[x] = strFldVals[x];
                }
                objReportTable.Rows.Add(pDataRow);
            }
            catch (Exception ex)
            {
                Logs.writeLine("getting error addRecToDatatab due to " + ex.Message);
            }
        }

        private void getDeviceTyp(IFeature pFeat, string strGis_Subtypecd, ref string strGisDeviceGroupTyp, ref string strGisDeviceGroupCode)
        {
            strGisDeviceGroupTyp = string.Empty;
            strGisDeviceGroupCode = string.Empty;
            string strDomainName = string.Empty;

            try
            {
                ISubtypes subtypes = (ISubtypes)pFeat.Class;
                strDomainName = subtypes.get_Domain(int.Parse(strGis_Subtypecd), "DEVICEGROUPTYPE").Name;

                IField pFld = pFeat.Fields.get_Field(pFeat.Fields.FindField("DEVICEGROUPTYPE"));
                strGisDeviceGroupCode = pFeat.get_Value(pFeat.Fields.FindField("DEVICEGROUPTYPE")).ToString();
                strGisDeviceGroupTyp = _clsGlobalFunctions.getCodedDomainDescription(clsTestWorkSpace.Workspace, strDomainName, strGisDeviceGroupCode);
                strGisDeviceGroupTyp = strGisDeviceGroupTyp.Replace(',', '|');

            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on getDeviceTyp " + ex.Message);

            }
        }

        //function to form update statement
        private void formUpdQry(List<string> lstUpdVals, ref string strUpdQry, ref string strA_UpdQry, string strGis_Globalid)
        {
            try
            {
                string strUpdVal = string.Empty;
                strUpdVal = " set ";
                if (lstUpdVals.Count == 0)
                {
                    strUpdQry = "";
                }
                else
                {
                    for (int i = 0; i < lstUpdVals.Count; i++)
                    {
                        strUpdVal = strUpdVal + lstUpdVals[i] + "|";

                    }
                    if (strUpdVal.EndsWith("|"))
                    {
                        strUpdVal = strUpdVal.TrimEnd('|');

                    }
                    strUpdQry = strUpdQry + strUpdVal + " where globalid ='" + strGis_Globalid + "';";
                    strA_UpdQry = strA_UpdQry + strUpdVal + " where globalid ='" + strGis_Globalid + "';";
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on formUpdQry " + ex.Message);

            }
        }

        private void getLocalOfficeId_mapnum(IPoint pPnt, IFeatureClass pLOPC_FC, ref string strFldValue, string strFldName)
        {
            IFeatureCursor pFCursor = null;
            ISpatialFilter pSptlFltr = new SpatialFilterClass();
            //IBufferConstruction pBuffConst = new BufferConstructionClass();
            IFeature pfeat = null;
            try
            {
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pSptlFltr.Geometry = (IGeometry)pPnt;
                    pSptlFltr.GeometryField = pLOPC_FC.ShapeFieldName;
                    pSptlFltr.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    pSptlFltr.WhereClause = "MAPTYPE IS NOT NULL order by MAPTYPE ASC";
                    //objectid is not null order by objectid
                    pFCursor = pLOPC_FC.Search(pSptlFltr, false);
                    cr.ManageLifetime(pFCursor);
                    int intTotCount = pLOPC_FC.FeatureCount(pSptlFltr);
                    pfeat = pFCursor.NextFeature();
                    if (pfeat != null)
                    {
                        //string strPoly_Maptyp = pfeat.get_Value(pfeat.Fields.FindField("MAPTYPE")).ToString();
                        //strPoly_LocalOfficeId = pfeat.get_Value(pfeat.Fields.FindField("LOCALOFFICEID")).ToString();
                        //strPoly_MapNum = pfeat.get_Value(pfeat.Fields.FindField("MAPNUMBER")).ToString();   

                        strFldValue = pfeat.get_Value(pfeat.Fields.FindField(strFldName)).ToString();
                    }

                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on getLocalOfficeId_mapnum " + ex.Message);
                //return str_locOffId_val;
            }
            finally
            {
                if (pFCursor != null)
                    Marshal.ReleaseComObject(pFCursor);
                if (pSptlFltr != null)
                    Marshal.ReleaseComObject(pSptlFltr);
                if (pSptlFltr != null)
                    Marshal.ReleaseComObject(pSptlFltr);
            }
        }






























        //private void updLocOff(IFeatureClass pfc, IFeatureClass pLOPC_FC, IFeature pPolyFeat, string strLocOffVal, int int_LOCALOFFICEID_Idx, string strLocFildName)
        //{
        //    try
        //    {
        //        Boolean blnStatus = false;
        //        string strex_loc_val = string.Empty;
        //        IFeatureCursor pFCursor = null;
        //        ISpatialFilter pSptlFltr = new SpatialFilterClass();
        //        IBufferConstruction pBuffConst = new BufferConstructionClass();
        //        IFeature pfeat = null;

        //        int intTotCount = 0;
        //        using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
        //        {
        //            try
        //            {
        //                pSptlFltr.Geometry = (IGeometry)pPolyFeat.Shape;
        //                pSptlFltr.GeometryField = pfc.ShapeFieldName;
        //                pSptlFltr.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
        //                //pSptlFltr.WhereClause = strLocFildName + " is null";
        //                //if (pfc.AliasName == "Open Point1")
        //                //{
        //                //    pSptlFltr.WhereClause = "SUBTYPECD <>'11' and "+strMapFildName+" is null";
        //                //}
        //                //else
        //                //{
        //                //    pSptlFltr.WhereClause = strMapFildName+" is null";
        //                //}
        //                pFCursor = pfc.Search(pSptlFltr, false);
        //                cr.ManageLifetime(pFCursor);
        //               //intTotCount = pfc.FeatureCount(pSptlFltr);
        //                pfeat = pFCursor.NextFeature();
        //                while (pfeat != null)
        //                {
        //                    try
        //                    {                                
        //                        strex_loc_val = pfeat.get_Value(int_LOCALOFFICEID_Idx).ToString();
        //                        if (strex_loc_val != strLocOffVal)
        //                        {                                    
        //                            //intUpdCount++;
        //                            //intUpdCntToStop++;
        //                            //pfeat.set_Value(int_LOCALOFFICEID_Idx, strLocOffVal);
        //                            //pfeat.Store();
        //                            //"FEATURECLASS_NAME,FEAT_OID,GEMSDISTMAPNUM,REMARKS"
        //                            _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pfc).Name + "," + pfeat.OID + "," + strLocFildName + "," + strLocOffVal + "," + pfeat.get_Value(pfeat.Fields.FindField("GLOBALID")).ToString() + ", feature updated ");

        //                        }
        //                        if (intUpdCount == 1000)
        //                        {
        //                            clsTestWorkSpace.StopEditOperation(true);
        //                            clsTestWorkSpace.StartEditOperation();
        //                            intUpdCount = 0;
        //                        }

        //                    }
        //                    catch (System.Exception ex)
        //                    {
        //                        Logs.writeLine("EXCP@updMapNum." + pfc.AliasName + " : ObjectId :" + pfeat.OID + " : LOCALOFFICEID :" + strLocOffVal + "   " + ex.Message);
        //                        //pfeat = pFCursor.NextFeature();
        //                    }
        //                    pfeat = pFCursor.NextFeature();


        //                    //terminate the tool if feature update count meats to 5000   
        //                    if (intUpdCntToStop == 5000)
        //                    {
        //                        pfeat = null;
        //                    }
        //                }
        //            }
        //            catch (System.Exception ex)
        //            {
        //                Logs.writeLine("EXCP@updMapNum " + ex.Message);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logs.writeLine("Error on process start button " + ex.Message);
        //    }
        //    finally
        //    {

        //    }
        //}

        //private void addTabTo_lst(List<string> lstTabNames)
        //{
        //    lstTabNames.Add("EDGIS.PRIMARYRISER");
        //    lstTabNames.Add("EDGIS.SECONDARYGENERATION");
        //    lstTabNames.Add("EDGIS.SECONDARYLOADPOINT");
        //    lstTabNames.Add("EDGIS.SERVICEPOINT");
        //    lstTabNames.Add("EDGIS.SMARTMETERNETWORKDEVICE");
        //    lstTabNames.Add("EDGIS.TIE");
        //    lstTabNames.Add("EDGIS.SERVICELOCATION");

        //    lstTabNames.Add("EDGIS.DISTBUSBAR");
        //    lstTabNames.Add("EDGIS.PRIOHCONDUCTOR");
        //    lstTabNames.Add("EDGIS.SECOHCONDUCTOR");
        //    lstTabNames.Add("EDGIS.SECUGCONDUCTOR");
        //    lstTabNames.Add("EDGIS.NEUTRALCONDUCTOR");
        //    lstTabNames.Add("EDGIS.PRIUGCONDUCTOR");

        //    lstTabNames.Add("EDGIS.PADMOUNTSTRUCTURE");
        //    lstTabNames.Add("EDGIS.DEVICEGROUP");
        //    lstTabNames.Add("EDGIS.SUBSURFACESTRUCTURE");
        //    lstTabNames.Add("EDGIS.SUPPORTSTRUCTURE");
        //    lstTabNames.Add("EDGIS.SWITCH");
        //    lstTabNames.Add("EDGIS.STEPDOWN");
        //    lstTabNames.Add("EDGIS.STREETLIGHT");
        //    lstTabNames.Add("EDGIS.TRANSFORMER");
        //    lstTabNames.Add("EDGIS.VOLTAGEREGULATOR");
        //    lstTabNames.Add("EDGIS.CAPACITORBANK");

        //    lstTabNames.Add("EDGIS.DCRECTIFIER");
        //    lstTabNames.Add("EDGIS.DELIVERYPOINT");
        //    lstTabNames.Add("EDGIS.DYNAMICPROTECTIVEDEVICE");
        //    lstTabNames.Add("EDGIS.ELECTRICSTITCHPOINT");
        //    lstTabNames.Add("EDGIS.FAULTINDICATOR");
        //    lstTabNames.Add("EDGIS.FUSE");
        //    lstTabNames.Add("EDGIS.LOADCHECKPOINT");
        //    lstTabNames.Add("EDGIS.OPENPOINT");
        //    lstTabNames.Add("EDGIS.PRIMARYGENERATION");
        //    lstTabNames.Add("EDGIS.PRIMARYMETER");

        //}

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
