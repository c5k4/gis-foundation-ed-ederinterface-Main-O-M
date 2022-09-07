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
    public partial class GenerateMissingInfoRecords : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();

        public GenerateMissingInfoRecords()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fldrDia = new FolderBrowserDialog();
                fldrDia.Description = "Select FGD database for Controller generation..";
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
                if (txtFGDpath.Text == string.Empty)
                {
                    MessageBox.Show("Please browse the path", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                _clsGlobalFunctions.Common_initSummaryTable("GenerateMissingInfoRecords.", "GenerateMissingInfoRecords.");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FC_NAME,COND_GLOBALID,CONDUCTORCODE,CONDUCTORUSE,REMARKS");                
                stbInfo.Text = "Starting Process..";
                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion

                clsTestWorkSpace.StartEditOperation();
                Logs.writeLine("StartEdit the database");
                startprocess();
                
                clsTestWorkSpace.StopEditOperation(true);

                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "GenerateMissingInfoRecords");
                
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

        private void startprocess()
        {
            try
            {
                IWorkspace pCedsaWsp = null;
                IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                pCedsaWsp = workspaceFactory.OpenFromFile(txtFGDpath.Text, 0);
                IFeatureWorkspace pCedsaFws = (IFeatureWorkspace)pCedsaWsp;
                if (pCedsaFws == null)
                {
                    Logs.writeLine("Unable to read FGD workspace ");
                    return;
                }
                //ITable pCedControlTab = pCedsaFws.OpenTable("CONTROLLER");
                List<List<string>> lstToInsertRecs = new List<List<string>>();
                //FCNAME,GLOBALID,FIELDNAME,VALUE,TASKTYPE,CEDSADEVICEID
                GetListToInsertRec_fromLog(ref lstToInsertRecs);
                for (int i = 0; i < lstToInsertRecs.Count; i++)
                {
                    processEachList(lstToInsertRecs[i], pCedsaFws);
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error on startprocess " + ex.Message);
            }
        }

        private void processEachList(List<string> lstToInsertRecs, IFeatureWorkspace pCedsaFws)
        {

            ITable pGisCondFC = null;
            ITable pGisTab = null;
            ITable pCedTab = null;
            string strFCName = string.Empty;
            string strCondGlobalid = string.Empty;
            string strfldName = string.Empty;
            string strConductorCodeVal = string.Empty;
            string strConductorUseVal = string.Empty;
            string strAction = string.Empty;
            string strCedsadeviceid = string.Empty;
            string strCondPhase = string.Empty;
            IRow pCedsaRow = null;
            IRow pGisConductorRow = null;
            int intUpdCount = 0;
            Boolean bolRecCreated = false ;
            
            for (int i = 0; i < lstToInsertRecs.Count; i++)
            {
                try
                {
                    stbInfo.Text = "processing...." + i + " of " + lstToInsertRecs.Count;
                    Application.DoEvents();

                    //FCNAME,GLOBALID,FIELDNAME,VALUE,TASKTYPE,CEDSADEVICEID (VALUE contines ConductorCode and Conductoruse)
                    strFCName = lstToInsertRecs[i].Split(',')[0];
                    strCondGlobalid = lstToInsertRecs[i].Split(',')[1];
                    strfldName = lstToInsertRecs[i].Split(',')[2];

                    strConductorCodeVal = lstToInsertRecs[i].Split(',')[3];
                    strConductorUseVal = lstToInsertRecs[i].Split(',')[4];
                    strAction = lstToInsertRecs[i].Split(',')[5];
                    strCedsadeviceid = lstToInsertRecs[i].Split(',')[6];

                    if (i == 0)
                    {
                        if (strFCName.ToUpper() == "PriOHConductorInfo".ToUpper())
                        {
                            pGisCondFC = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.PriOHConductor"));
                            pGisTab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.PriOHConductorinfo"));
                            pCedTab = pCedsaFws.OpenTable("PriOHConductorInfo");
                        }
                        else if (strFCName.ToUpper() == "PriUGConductorInfo".ToUpper())
                        {
                            pCedTab = pCedsaFws.OpenTable("PriugConductorInfo");
                            pGisTab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.PriUGConductorinfo"));
                            pGisCondFC = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.PriUGConductor"));
                        }
                    }
                    //get cedsa info record with matching cedsadeviceid,code and use
                    pCedsaRow = getcedsaRecord(pCedTab, strCedsadeviceid, strConductorCodeVal, strConductorUseVal);
                    if (pCedsaRow == null) continue;
                    pGisConductorRow = getCondRec(pGisCondFC, strCondGlobalid);
                    strCondPhase = _clsGlobalFunctions.Cast(pGisConductorRow.get_Value(pGisConductorRow.Fields.FindField("PHASEDESIGNATION")), string.Empty);

                    bolRecCreated = false;
                    insertInfoRecord(pCedsaRow, pCedTab, pGisTab, strCondGlobalid, strCondPhase, ref bolRecCreated);
                    //FC_NAME,COND_GLOBALID,CONDUCTORCODE,CONDUCTORUSE,REMARKS
                    if (bolRecCreated = true)
                    {
                        _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, strFCName + "," + strCondGlobalid + "," + strConductorCodeVal + ","+strConductorUseVal+", Recore created ");
                    }
                    if (intUpdCount == 1000)
                    {
                        clsTestWorkSpace.StopEditOperation(true);
                        clsTestWorkSpace.StartEditOperation();
                        intUpdCount = 0;
                    }
                }
                catch (Exception ex)
                {
                    Logs.writeLine("Error on processEachList "+strFCName + ":CONDUCTORGLOBALID:"+strCondGlobalid +" "+ ex.Message);
                }

            }
        }

        private void insertInfoRecord(IRow pCedsaRow, ITable pCedTab, ITable pGisTab, string strCondGlobalid,string strCondPhase,ref Boolean bolRecCreated)
        {

            IRowBuffer pRowBuffer = null;
            ICursor pInserCursor = null;
            string strINSTALLATIONDATEVal = string.Empty;
            string strCedsaCondUse = string.Empty;
            bolRecCreated = false;
            int intFldIdx = -1;
            try
            {

                strCedsaCondUse = _clsGlobalFunctions.Cast(pCedsaRow.get_Value(pCedsaRow.Fields.FindField("CONDUCTORUSE")), string.Empty);

                string s = ((IObjectClass)pGisTab).AliasName;

                pRowBuffer = pGisTab.CreateRowBuffer();
                for (int i = 0; i < pGisTab.Fields.FieldCount; i++)
                {
                    int sourcefieldindex = -1;
                    int destinationfieldindex = -1;
                    IField objfield = pGisTab.Fields.get_Field(i);
                    try
                    {
                        if (objfield.Editable)
                        {
                            destinationfieldindex = pGisTab.Fields.FindField(objfield.Name);
                            sourcefieldindex = pCedsaRow.Fields.FindField(objfield.Name);

                            if (destinationfieldindex != -1 && sourcefieldindex != -1)
                            {
                                if (objfield.Name.ToUpper() != "CONVERSIONID" && objfield.Name.ToUpper() != "DATEMODIFIED" && objfield.Name.ToUpper() != "LASTUSER" && objfield.Name.ToUpper() != "CONVERSIONWORKPACKAGE" && objfield.Name.ToUpper() != "CONVERSIONID" && objfield.Name.ToUpper() != "CONDUCTORCONVID")
                                {

                                   if (objfield.Name.ToUpper() == "PHASEDESIGNATION")
                                    {
                                        //if conductoruse is primary or parallel 
                                        if (strCedsaCondUse == "1" || strCedsaCondUse == "4")
                                        {
                                            pRowBuffer.set_Value(destinationfieldindex, strCondPhase);
                                        }
                                        else
                                        {
                                            pRowBuffer.set_Value(destinationfieldindex, pCedsaRow.get_Value(sourcefieldindex));                                        
                                        }
                                    }
                                    else
                                    {
                                        pRowBuffer.set_Value(destinationfieldindex, pCedsaRow.get_Value(sourcefieldindex));
                                    }
                                    
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Logs.writeLine("Error @ retriving value :" + objfield.Name + " Conductor Globalid : " + strCondGlobalid + " " + ex.Message);
                    }
                }
                pRowBuffer.set_Value(pGisTab.FindField("CONDUCTORGUID"), strCondGlobalid);
                pRowBuffer.set_Value(pGisTab.FindField("SUBTYPECD"), "1");
                pRowBuffer.set_Value(pGisTab.FindField("CREATIONUSER"), "P1P4");
                pRowBuffer.set_Value(pGisTab.FindField("DATECREATED"), String.Format("{0:M/d/yyyy}", DateTime.Now));                

                pInserCursor = pGisTab.Insert(false);
                pInserCursor.InsertRow(pRowBuffer);
                bolRecCreated = true;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at insert RECORD for Conductor Globalid :" + strCondGlobalid + "  " + ex.Message);
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

      
        private IRow getCondRec(ITable pGisCondFC, string strCondGlobalid)
        {

            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                pFilter.WhereClause = "GLOBALID='" + strCondGlobalid + "'";

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pGisCondFC.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();

                    if (pRow == null)
                    {
                        Logs.writeLine("Feature not found with this GLOBALID " + strCondGlobalid);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("getCondRec " + ex.Message);
                return pRow;

            }
            return pRow;
        }

        private IFeature getFeature(IFeatureClass pFC, string strCedsaID_Val)
        {
            IFeatureCursor pFCursor = null;
            IFeature pFeature = null;
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                pFilter.WhereClause = "CEDSADEVICEID='" + strCedsaID_Val + "'";

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pFCursor = pFC.Search(pFilter, false);
                    cr.ManageLifetime(pFCursor);
                    pFeature = pFCursor.NextFeature();

                    if (pFeature == null)
                    {
                        Logs.writeLine("Feature not found with this CEDSADEVICEID " + strCedsaID_Val);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("getFeature " + ex.Message);
                return pFeature;

            }
            return pFeature;
        }

        private IRow  getcedsaRecord(ITable pCedTab,string strCedsadeviceid, string strConductorCodeVal, string strConductorUseVal)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                pFilter.WhereClause = "CEDSALINESECTIONID=" + strCedsadeviceid + " AND PGE_CONDUCTORCODE=" + strConductorCodeVal + " AND CONDUCTORUSE=" + strConductorUseVal ;

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pCedTab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();

                    if (pRow == null)
                    {
                        Logs.writeLine("Cedsa record not found With info CEDSALINESECTIONID:PGE_CONDUCTORCODE:CONDUCTORUSE " + strCedsadeviceid + ":" + strConductorCodeVal + ":" + strConductorUseVal);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("getcedsaRecord " + ex.Message);
                return pRow;

            }
            return pRow;    
        }

        private void GetListToInsertRec_fromLog(ref List<List<string>> lstToUpdRecs)
        {
            //ref List<string> lstToUpdRecs
            try
            {
                lstToUpdRecs.Clear();
                List<string> lstToUpd = new List<string>();
                int intIteration = 0;
                string strFcName = string.Empty;
                //File format should be like this : FCNAME,GLOBALID,FIELDNAME,FIELDNAME,VALUE,CEDSADEVICEID,               
                string strPath = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                strPath = strPath + "\\GenerateMissingInfoRecords.CSV";
                if (File.Exists(strPath) == false)
                {
                    MessageBox.Show("GenerateMissingInfoRecords.CSV file not found in applicationpath.Please check", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                TextReader tx = new StreamReader(strPath);
                string strFromCsv = tx.ReadLine().Trim();

                while (strFromCsv != null)
                {
                    //strFcName = strFromCsv.Split(',')[0];
                    //1st iteration is header
                    if (intIteration == 0)
                    {
                        intIteration++;
                        strFromCsv = tx.ReadLine();
                        continue;
                    }
                    if (strFcName.Length == 0)
                    {
                        strFcName = strFromCsv.Split(',')[0];
                        lstToUpd.Add(strFromCsv);
                    }
                    if (strFcName == strFromCsv.Split(',')[0])
                    {
                        lstToUpd.Add(strFromCsv);
                    }
                    else
                    {
                        lstToUpdRecs.Add(lstToUpd);
                        lstToUpd = new List<string>();
                        strFcName = string.Empty;
                    }
                    intIteration++;
                    strFromCsv = tx.ReadLine();
                }
                tx.Close();
                lstToUpdRecs.Add(lstToUpd);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error #GetListToUpdateRec_fromLog " + ex.Message);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
