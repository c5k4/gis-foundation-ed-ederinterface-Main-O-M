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
    public partial class frmCreateSpecialConditions : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        OracleConnection OrclConnSrc = null;

        public frmCreateSpecialConditions()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string strConnDetails = txtConnDetails.Text;
                if (strConnDetails == "")
                {
                    MessageBox.Show("Enter Connection details");
                    return;
                }
                string[] strConnDetailsArr = strConnDetails.Split(':');
                OrclConnSrc = _clsGlobalFunctions.GetOracleConnection(strConnDetailsArr[1], strConnDetailsArr[2], strConnDetailsArr[0]);
                btnStart.Enabled = true;
                btnExit.Enabled = true;

                stbSpeConditions.Text = "connection succeeded";
                Application.DoEvents();

            }
            catch (System.Exception ex)
            {
                Logs.writeLine("EXCP@button1_Click. " + ex.Message);

            }
            finally
            {

            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {   
            try
            {
                btnStart.Enabled = false;
                btnExit.Enabled = false;

                stbSpeConditions.Text = "Starting Process..";

                stbSpeConditions.Text = "Disable Autoupdaters ";
                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion

                stbSpeConditions.Text = "Stopping Auto Creation of Annotations in Destination database ";
                #region "Stop Auto Creation of Annotations in Destination database"
                IEnumDataset DestinationEnumdataset = clsTestWorkSpace.Workspace.get_Datasets(esriDatasetType.esriDTAny);
                //Loop through all the Destination datasets to append the data 
                IDataset Destinationdataset = DestinationEnumdataset.Next();
                ArrayList objarraylist = null;
                while (Destinationdataset != null)
                {
                    Application.DoEvents();
                    AutoAnotationcreate(Destinationdataset, false, ref objarraylist);
                    Destinationdataset = DestinationEnumdataset.Next();
                }
                #endregion

                
                
                
                objReportTable.Clear();
                objReportTable.Columns.Clear();
                _clsGlobalFunctions.Common_initSummaryTable("createSpecialConditionRecs", "createSpecialConditionRecs");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FEATURECLASS_NAME,CHILDTABLE_NAME,SS_CONVERSIONID,ACCESSINFO_VAL,REMARKS");

                clsTestWorkSpace.StartEditOperation();
                startprocess();
                clsTestWorkSpace.StopEditOperation(true);

                stbSpeConditions.Text = "Reseting auto creation of annotations...";
                #region "Start Auto Creation of Annotations in Destination database"

                DestinationEnumdataset.Reset();
                Destinationdataset = DestinationEnumdataset.Next();

                while (Destinationdataset != null)
                {
                    Application.DoEvents();
                    AutoAnotationcreate(Destinationdataset, true, ref objarraylist);
                    Destinationdataset = DestinationEnumdataset.Next();
                }
                #endregion

                stbSpeConditions.Text = "Enabling autoupdaters...";
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "createSpecialConditionRecs");
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                stbSpeConditions.Text = "Process completed see the log file.";
                Logs.writeLine("Successfully Completed");

                btnStart.Enabled = true;
                btnExit.Enabled = true;
            }
            catch (Exception ex)
            {
                btnStart.Enabled = true;
                btnExit.Enabled = true;

                Logs.writeLine("Error on process start button " + ex.Message);
                stbSpeConditions.Text = "Error occurred, Please see the log file.";
            }
            finally
            {
                if (OrclConnSrc.State == ConnectionState.Open)
                {
                    OrclConnSrc.Close();
                }
            }

        }

        private void startprocess()
        {
            IFeatureClass pFC = null;
            ITable pTab = null;
            ITable pChildTab = null;
            IFeature pfeature = null;

            IFeatureCursor pFCursor = null;
            int intACCESSINFO_Structure ;
            int intACCESSINFO_SpecialCond ;
            int intSAPEQUIPID ;
            string strSTRUCTURE_GlobalIgVal = string.Empty;
            string strACCESSINFO_StructureVal = string.Empty;
            string strACCESSINFO_SpecialCondVal = string.Empty;
            string strSTRUCTURE_CONVERSIONIDVal = string.Empty;
            int intIterationCount = 0;
            int intIterationUpdCount = 0;
            int intTotalRecCount = 0;

            string strSAPEQUIPIDVal = string.Empty;
            string strSource_ACCESSINFOVal = string.Empty;
            ISet pSet = null;
            List<string> lstNeedtoGenerateAccessInfoRecs = new List<string>();
            List<string> lstExtraRecsInGis = new List<string>();

            pFC = clsTestWorkSpace.FeatureWorkspace.OpenFeatureClass("EDGIS.SupportStructure");
            pChildTab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.SpecialConditions"));
            IObjectClass obj = (IObjectClass)pChildTab;

            intACCESSINFO_Structure = pFC.Fields.FindField("ACCESSINFO");
            intACCESSINFO_SpecialCond = pChildTab.Fields.FindField("ACCESSINFO");
            intSAPEQUIPID = pFC.Fields.FindField("SAPEQUIPID");
            
            //SAPEQUIPID
            try
            {

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    intTotalRecCount = pFC.FeatureCount(null);
                    pFCursor = pFC.Search(null, false);
                    cr.ManageLifetime(pFCursor);
                    pfeature = pFCursor.NextFeature();
                    while (pfeature != null)
                    {
                        try
                        {
                            intIterationCount++;
                            stbSpeConditions.Text = "processing...." + intIterationCount + " of " + intTotalRecCount;
                            Application.DoEvents();

                            lstNeedtoGenerateAccessInfoRecs.Clear();                            
                            strSAPEQUIPIDVal = _clsGlobalFunctions.Cast(pfeature.get_Value(intSAPEQUIPID).ToString(), string.Empty);
                            strACCESSINFO_StructureVal = pfeature.get_Value(intACCESSINFO_Structure).ToString();
                            strSTRUCTURE_GlobalIgVal = pfeature.get_Value(pFC.Fields.FindField("GLOBALID")).ToString();//GLOBALID
                            strSTRUCTURE_CONVERSIONIDVal = pfeature.get_Value(pFC.Fields.FindField("CONVERSIONID")).ToString();

                            if (strSAPEQUIPIDVal == string.Empty)
                            {
                                pfeature = pFCursor.NextFeature();
                                continue;
                            }

                            getAccessInfoVal_FromDataconvSupportStrt(ref strSource_ACCESSINFOVal, strSAPEQUIPIDVal);
                            if (strSource_ACCESSINFOVal != string.Empty)
                            {

                                pSet = clsGlobalFunctions.getRealtedFeature(pfeature, obj.AliasName, false);
                                pSet.Reset();

                                getAccessInfoString_FromUnitRecords(ref lstNeedtoGenerateAccessInfoRecs, ref lstExtraRecsInGis, strSource_ACCESSINFOVal, pSet, intACCESSINFO_SpecialCond);

                                if (lstNeedtoGenerateAccessInfoRecs.Count > 0)
                                {
                                    intIterationUpdCount++;
                                    generatemissingInfo_records(lstNeedtoGenerateAccessInfoRecs, strSTRUCTURE_GlobalIgVal, pChildTab, strSTRUCTURE_CONVERSIONIDVal);
                                }

                                if (intIterationUpdCount > 500)
                                {
                                    clsTestWorkSpace.StopEditOperation(true);
                                    clsTestWorkSpace.StartEditOperation();
                                    intIterationUpdCount = 0;
                                }
                            }
                            pfeature = pFCursor.NextFeature();
                            //pfeature = null;
                        }
                        catch (System.Exception ex)
                        {
                            Logs.writeLine("EXCP@startprocess at SUPPORTSTRUCTURE oid:" + pfeature.OID + "  " + ex.Message);
                            pfeature = pFCursor.NextFeature();
                        }
                    }
                    //
                    if (lstExtraRecsInGis.Count > 0)
                    {
                        intIterationUpdCount = 0;
                        for (int i = 0; i < lstExtraRecsInGis.Count; i++)
                        {
                            Logs.writeLine("DELETE RECORD : " + lstExtraRecsInGis[i]);
                            stbSpeConditions.Text = "processing...." + i + " of " + lstExtraRecsInGis.Count;
                            Application.DoEvents();
                            deleteSpecialConditions(pChildTab, lstExtraRecsInGis[i]);

                            if (intIterationUpdCount > 1000)
                            {
                                clsTestWorkSpace.StopEditOperation(true);
                                clsTestWorkSpace.StartEditOperation();
                                intIterationUpdCount = 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("startprocess " + ex.Message);
            }
        }

        private void deleteSpecialConditions(ITable ptab, string strExtraRecord)
        {

            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();
            int intIterationCount = 0;

            string strGlobalId = strExtraRecord.Split('|')[1];
            string strExta_AccessInfo = strExtraRecord.Split('|')[0];

            try
            {

                pFilter.WhereClause = "GLOBALID='" + strGlobalId + "'";

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = ptab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                    if (pRow != null)
                    {
                        pRow.Delete();                    
                    }                    
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("deleteSpecialConditions " + ex.Message);

            }
        }

        private void generatemissingInfo_records(List<string> lstNeedtoGenerateAccessInfoRecs, string strSTRUCTURE_GlobalIgVal, ITable pChildTab, string strSTRUCTURE_CONVERSIONIDVal)
        {
            try
            {
                for (int k = 0; k < lstNeedtoGenerateAccessInfoRecs.Count; k++)
                {
                    insertRow(lstNeedtoGenerateAccessInfoRecs[k].ToString(), pChildTab, strSTRUCTURE_GlobalIgVal, strSTRUCTURE_CONVERSIONIDVal);

                    //"FEATURECLASS_NAME,CHILDTABLE_NAME,SS_CONVERSIONID,ACCESSINFO_VAL,REMARKS"

                    _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, "SUPPORTSTRUCTURE" + "," + "SPECIALCONDITIONS" + "," + strSTRUCTURE_CONVERSIONIDVal+","+lstNeedtoGenerateAccessInfoRecs[k].ToString()+ ",Record inserted");
                }
            }
            catch (System.Exception ex)
            {
                Logs.writeLine("EXCP@generatemissingInfo_records at SUPPORTSTRUCTURE:" + strSTRUCTURE_CONVERSIONIDVal +  "  "+ex.Message);
            }
        }

        private void insertRow(string strAccessInfoVal, ITable pChildTab, string strSTRUCTURE_GlobalIgVal, string strSTRUCTURE_CONVERSIONIDVal)
        {
            IRowBuffer pRowBuffer = null;
            ICursor pInserCursor = null;
            
            try
            {
                pRowBuffer = pChildTab.CreateRowBuffer();
                pRowBuffer.set_Value(pChildTab.FindField("CREATIONUSER"), "P1P4");
                pRowBuffer.set_Value(pChildTab.FindField("DATECREATED"), String.Format("{0:M/d/yyyy}", DateTime.Now));
                pRowBuffer.set_Value(pChildTab.Fields.FindField("STRUCTUREGUID"), strSTRUCTURE_GlobalIgVal);
                pRowBuffer.set_Value(pChildTab.Fields.FindField("ACCESSINFO"), strAccessInfoVal);
                pInserCursor = pChildTab.Insert(false);
                pInserCursor.InsertRow(pRowBuffer);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at insert record, SUPPORTSTRUCTURE:" + strSTRUCTURE_CONVERSIONIDVal + "  "+ ex.Message);
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

    

        private void getAccessInfoString_FromUnitRecords(ref List<string> lstNeedtoGenerateAccessInfoRecs,ref List<string> lstExtraRecsInGis, string strSource_ACCESSINFOVal, ISet pSet, int intACCESSINFO_SpecialCond)
        {
            IRow pRow = null;
            pSet.Reset();
            string strInfo = string.Empty;
            string strGlobalID = string.Empty;
            string strAllAccessInfo = string.Empty;
            string[] ArrSource_ACCESSINFOVal = strSource_ACCESSINFOVal.Split(',');
            
            try
            {
                pRow = pSet.Next() as IRow;

                while (pRow != null)
                {
                    strInfo = _clsGlobalFunctions.Cast(pRow.get_Value(intACCESSINFO_SpecialCond).ToString(), string.Empty);
                    strGlobalID = _clsGlobalFunctions.Cast(pRow.get_Value(pRow.Fields.FindField("GLOBALID")).ToString(), string.Empty);

                    if (strInfo != string.Empty)
                    {
                        if (strAllAccessInfo.Length > 0)
                        {
                            strAllAccessInfo = strAllAccessInfo + "," + strInfo;
                        }
                        else
                        {
                            strAllAccessInfo = strInfo;
                        }
                        //Add to the list which are extra in gis
                        if(!strSource_ACCESSINFOVal.ToUpper().Contains(strInfo.ToUpper()))
                        {
                            lstExtraRecsInGis.Add(strInfo + "|" + strGlobalID);
                        }
                    }
                    pRow = pSet.Next() as IRow;
                }

                if (strAllAccessInfo.Length > 0)
                {
                    for (int k = 0; k < ArrSource_ACCESSINFOVal.Length; k++)
                    {
                        if (!strAllAccessInfo.ToUpper().Contains(ArrSource_ACCESSINFOVal[k].ToUpper()))
                        {
                            Logs.writeLine("MissingInfoRecord " + ArrSource_ACCESSINFOVal[k].ToString());

                            lstNeedtoGenerateAccessInfoRecs.Add(ArrSource_ACCESSINFOVal[k]);
                        }
                    }
                }
                //delete from GIS which are not in CEDSA
                //if (lstExtraRecsInGis.Count > 0)
                //{
                //    pSet.Reset();
                //    pRow = pSet.Next() as IRow;
                //    while (pRow != null)
                //    {
                //        strInfo = _clsGlobalFunctions.Cast(pRow.get_Value(intACCESSINFO_SpecialCond).ToString(), string.Empty);
                //        if (lstExtraRecsInGis.Contains(strInfo))
                //        {
                //            pRow.Delete();                            
                //        }
                //        pRow = pSet.Next() as IRow;
                //    }                
                //}
            }
            catch (System.Exception ex)
            {
                Logs.writeLine("EXCP@getAccessInfoString_FromUnitRecords "  + ex.Message);
            }
        }

        private void getAccessInfoVal_FromDataconvSupportStrt(ref string strSource_ACCESSINFOVal, string strSAPEQUIPIDVal)
        {
            OracleDataReader oleRdr = null;
            OracleCommand oleCmd = null;
            string strQry = string.Empty;
            strSource_ACCESSINFOVal = string.Empty;
            try
            {

                strQry = "select ACCESSINFO FROM SUPPORTSTRUCTURE WHERE SAPEQUIPID ='" + strSAPEQUIPIDVal + "'";
               
                oleCmd = new OracleCommand(strQry, OrclConnSrc);
                oleRdr = oleCmd.ExecuteReader();
                if (oleRdr.HasRows)
                {
                    oleRdr.Read();
                    strSource_ACCESSINFOVal = oleRdr["ACCESSINFO"].ToString();
                }
            }
            catch (System.Exception ex)
            {
                Logs.writeLine("EXCP@getAccessInfoVal_FromDataconvSupportStrt " + strQry + " " + ex.Message);
            }
            finally
            {
                oleCmd.Dispose();
                oleRdr.Close();
                oleRdr.Dispose();
            }
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

        /// <summary>
        /// This method is to find the feature class and set the auto anotation create false 
        /// </summary>
        /// <param name="pdestinationdataset"></param>
        private void AutoAnotationcreate(IDataset pdestinationdataset, Boolean status, ref ArrayList strAnnoclass)
        {
            IFeatureClass objfeatureclass = null;
            IAnnoClassAdmin2 objannotation = null;
            IEnumDataset Enumdataset = null;
            IDataset enudataset = null;

            try
            {
                switch (pdestinationdataset.Type)
                {
                    case esriDatasetType.esriDTFeatureDataset:
                        {
                            Enumdataset = pdestinationdataset.Subsets;
                            enudataset = Enumdataset.Next();
                            while (enudataset != null)
                            {
                                if (enudataset.Type == esriDatasetType.esriDTFeatureClass)
                                {
                                    objfeatureclass = clsGlobalFunctions._globalFunctions.getFeatureclassByName(clsTestWorkSpace.Workspace, enudataset.Name);
                                    if (objfeatureclass.FeatureType == esriFeatureType.esriFTAnnotation)
                                    {

                                        objannotation = (IAnnoClassAdmin2)objfeatureclass.Extension;
                                        objannotation.AutoCreate = status;

                                        objannotation.UpdateProperties();
                                    }
                                }
                                enudataset = Enumdataset.Next();
                            }
                            break;
                        }
                    case esriDatasetType.esriDTFeatureClass:
                        {
                            objfeatureclass = (IFeatureClass)pdestinationdataset;
                            if (objfeatureclass.FeatureType == esriFeatureType.esriFTAnnotation)
                            {
                                objannotation = (IAnnoClassAdmin2)objfeatureclass.Extension;
                                objannotation.AutoCreate = status;
                                objannotation.UpdateProperties();
                            }
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            catch
            {
                Logs.writeLine("Error occured in AutoAnotationcreate method ");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
