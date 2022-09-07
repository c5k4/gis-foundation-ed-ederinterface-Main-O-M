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
    public partial class GenerateController : Form
    {
        private IMMAutoUpdater autoupdater = null;
        private DataTable objReportTable = new DataTable();
        clsGlobalFunctions _clsGlobalFunctions = new clsGlobalFunctions();
        

        public GenerateController()
        {
            InitializeComponent();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Tool to create CONTROLLER records based on CEDSADEVICEID for CapacitorBank,DynamicProtectiveDevice,Switch,VoltageRegulatorUnit");
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

        private void GenerateController_Load(object sender, EventArgs e)
        {
            //cmbAllFCS.Items.Add("EDGIS.CapacitorBank");
            //cmbAllFCS.Items.Add("EDGIS.DynamicProtectiveDevice");
            //cmbAllFCS.Items.Add("EDGIS.Switch");
            //cmbAllFCS.Items.Add("EDGIS.VoltageRegulatorUnit");
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
                _clsGlobalFunctions.Common_initSummaryTable("createControllerRecs", "createControllerRecs");
                _clsGlobalFunctions.Common_addColumnToReportTable(objReportTable, "FEATURECLASS_NAME,FEAT_OID,CEDSADEVICEID,REMARKS");
                IWorkspace pCedsaWsp = null;
                IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
                pCedsaWsp =  workspaceFactory.OpenFromFile(txtFGDpath.Text, 0);
                IFeatureWorkspace pCedsaFws = (IFeatureWorkspace)pCedsaWsp;
                ITable pCedControlTab = pCedsaFws.OpenTable("CONTROLLER");
                Logs.writeLine("Opend the CONTROLLER table ");
                
                clsTestWorkSpace.StartEditOperation();
                Logs.writeLine("StartEdit the database");
                startprocess( pCedControlTab);
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

                clsGFMSGlobalFunctions._GFMSGlobalFunctions.GenerateTheReport_MultipleFeatureClass(objReportTable, "createControllerRecs");
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

        private void startprocess( ITable pCedControlTab)
        {
            IFeatureClass pParentfc = null;
            ITable pSwitchtab = null;
            ITable pDPDtab = null;
            ITable pCapacitorBanktab = null;
            ITable pVoltageUnittab = null;
            

            ITable pChildTab = null;
            int intiterationCount = 0;
            int intTotCount = 0;
            int intUpdCount = 0;
            IQueryFilter pQueryFilter = null;
            ICursor pCursor = null;
            
            IRow pCedsaControllRow = null;
            IRow pFeatRow = null;
            int intValidate = 0;

            try
            {
                pSwitchtab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.Switch"));
                pDPDtab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.DynamicProtectiveDevice"));
                pCapacitorBanktab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.CapacitorBank"));
                pVoltageUnittab = (ITable)(clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.VoltageRegulatorUnit"));

                pChildTab = clsTestWorkSpace.FeatureWorkspace.OpenTable("EDGIS.Controller");                
                IObjectClass obj = (IObjectClass)pChildTab;             

                //int intConvIdIdx = pParenttab.FindField("CONVERSIONID");
                //int intCEDSADEVIdx = pParenttab.FindField("CEDSADEVICEID");
                //int intGuidIdIdx = pParenttab.FindField("GLOBALID");

                int intCed_CEDSADEVIdx = pCedControlTab.FindField("CEDSADEVICEID");
                int intCed_UNITIDidx = pCedControlTab.FindField("VRUNITID");

                string strCEDSADEVIdVal = string.Empty;
                string strCed_UNITIDVal = string.Empty;
                string strFeatGuidVal = string.Empty;
                string strSubTypCDVal = string.Empty;
                string strFeatStatus = string.Empty;

                stbInfo.Text = "processing wait...";
                Application.DoEvents();
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    intTotCount = pCedControlTab.RowCount(null);
                    pCursor = pCedControlTab.Search(null, false);
                    cr.ManageLifetime(pCursor);
                    pCedsaControllRow = pCursor.NextRow();

                    deleteExistingUnitRecordsfromController(pChildTab);
                    while (pCedsaControllRow != null)
                    {
                        intValidate = 0;
                        try
                        {
                            pFeatRow = null;
                            strCEDSADEVIdVal = _clsGlobalFunctions.Cast(pCedsaControllRow.get_Value(intCed_CEDSADEVIdx).ToString(), string.Empty);
                            strCed_UNITIDVal = _clsGlobalFunctions.Cast(pCedsaControllRow.get_Value(intCed_UNITIDidx).ToString(), string.Empty);
                            intiterationCount++;
                            stbInfo.Text = "processing...." + intiterationCount + " of " + intTotCount;
                            Application.DoEvents();
                            if (strCEDSADEVIdVal.Length > 0)
                            {
                                //EDGIS.Switch
                                if (pFeatRow == null)
                                {
                                    pFeatRow = getRecWithCedsaID(strCEDSADEVIdVal, pSwitchtab, "CEDSADEVICEID");
                                    if (pFeatRow != null)
                                    {
                                        intValidate++;
                                        intUpdCount++;
                                        strFeatStatus = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("STATUS")).ToString(), string.Empty);
                                        strFeatGuidVal = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("GLOBALID")).ToString(), string.Empty);
                                        insertRow(strFeatStatus,pCedsaControllRow, pChildTab, strFeatGuidVal, strCEDSADEVIdVal, pSwitchtab, "6");
                                    }
                                }

                                if (pFeatRow == null)
                                {   //EDGIS.DynamicProtectiveDevice
                                    pFeatRow = getRecWithCedsaID(strCEDSADEVIdVal, pDPDtab, "CEDSADEVICEID");
                                    if (pFeatRow != null)
                                    {
                                        intValidate++;
                                        intUpdCount++;
                                        strFeatStatus = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("STATUS")).ToString(), string.Empty);
                                        strFeatGuidVal = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("GLOBALID")).ToString(), string.Empty);
                                        string strSubTypeCd = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("SUBTYPECD")).ToString(), string.Empty);
                                        if (strSubTypeCd == "2")//Interrupter
                                        {
                                            strSubTypCDVal = "4";
                                        }
                                        else if (strSubTypeCd == "3")//Recloser
                                        {
                                            strSubTypCDVal = "3";
                                        }
                                        else if (strSubTypeCd == "8")//Sectionalizer
                                        {
                                            strSubTypCDVal = "5";
                                        }
                                        insertRow(strFeatStatus,pCedsaControllRow, pChildTab, strFeatGuidVal, strCEDSADEVIdVal, pDPDtab, strSubTypCDVal);
                                    }
                                }
                                //
                                if (pFeatRow == null)
                                {
                                    //EDGIS.CapacitorBank
                                    pFeatRow = getRecWithCedsaID(strCEDSADEVIdVal, pCapacitorBanktab, "CEDSADEVICEID");
                                    if (pFeatRow != null)
                                    {
                                        intValidate++;
                                        intUpdCount++;
                                        strFeatStatus = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("STATUS")).ToString(), string.Empty);
                                        strFeatGuidVal = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("GLOBALID")).ToString(), string.Empty);
                                        insertRow(strFeatStatus,pCedsaControllRow, pChildTab, strFeatGuidVal, strCEDSADEVIdVal, pCapacitorBanktab, "1");
                                    }
                                }
                                
                                if (pFeatRow == null)
                                {
                                    //EDGIS.VoltageRegulatorUnit
                                    if (strCed_UNITIDVal.Length > 0)
                                    {
                                        pFeatRow = getRecWithCedsaID_vrunit(strCed_UNITIDVal,strCEDSADEVIdVal,pVoltageUnittab, "UNITID");
                                        if (pFeatRow != null)
                                        {
                                            intValidate++;
                                            intUpdCount++;
                                            strFeatStatus = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("STATUS")).ToString(), string.Empty);
                                            strFeatGuidVal = _clsGlobalFunctions.Cast(pFeatRow.get_Value(pFeatRow.Fields.FindField("GLOBALID")).ToString(), string.Empty);
                                            insertRow(strFeatStatus,pCedsaControllRow, pChildTab, strFeatGuidVal, strCEDSADEVIdVal, pVoltageUnittab, "2");
                                        }
                                    }
                                }
                            }
                            if (intUpdCount == 1000)
                            {
                                clsTestWorkSpace.StopEditOperation(true);
                                clsTestWorkSpace.StartEditOperation();
                                intUpdCount = 0;
                            }
                            if (pFeatRow != null)
                            {
                                //"FEATURECLASS_NAME,FEAT_OID,CEDSADEVICEID,REMARKS"
                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable, ((IDataset)pFeatRow.Table).Name + "," + pFeatRow.OID + "," + strCEDSADEVIdVal + ", Controller record created ");
                            }
                            else
                            {
                                //"FEATURECLASS_NAME,FEAT_OID,CEDSADEVICEID,REMARKS"
                                _clsGlobalFunctions.Common_addRowstoReportTable(objReportTable,  "," + "," + strCEDSADEVIdVal + ", Controller record not created ");
                            
                            }

                            //if (intValidate > 1)
                            //{
                            //    Logs.writeLine("MALTIPLE RECORDS CREATED WITH " + strCEDSADEVIdVal);                            
                            //}                            
                        }
                        catch (Exception ex)
                        {                            
                            pCedsaControllRow = pCursor.NextRow();
                        }
                        pCedsaControllRow = pCursor.NextRow();
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine( "  #startprocess " + ex.Message);
            } 
        }

        private void deleteExistingUnitRecordsfromController(ITable pControllerTab)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();
            int intUpdCount = 0;

            string strRefUnitGlobalid = string.Empty;
            try
            {                
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pControllerTab.Search(null, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                    while (pRow != null)
                    {
                        stbInfo.Text = "DELETING CONTROLLER ...." +pRow.OID ;
                        Application.DoEvents();
                        
                        intUpdCount++;                      
                        pRow.Delete();
                        pRow = pCursor.NextRow();

                        if (intUpdCount == 1000)
                        {
                            clsTestWorkSpace.StopEditOperation(true);
                            clsTestWorkSpace.StartEditOperation();
                            intUpdCount = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at deleteExistingUnitRecordsfromController for CEDSADEVICEID"  + ex.Message);
            }
        }

       

        private void insertRow(string strFeatStatus,IRow pCedsaControllerRow, ITable pChildTab, string strParentGuid, string strCEDSADEVIdVal, ITable pParenttab,string strSubtypeCdVal)
        {
            IRowBuffer pRowBuffer = null;
            ICursor pInserCursor = null;
            string strINSTALLATIONDATEVal = string.Empty;
            int intFldIdx = -1;
            try
            {
               
                string s = ((IObjectClass)pChildTab).AliasName;

                pRowBuffer = pChildTab.CreateRowBuffer();
                for (int i = 0; i < pChildTab.Fields.FieldCount; i++)
                {
                    int sourcefieldindex = -1;
                    int destinationfieldindex = -1;
                    IField objfield = pChildTab.Fields.get_Field(i);
                    try
                    {                        
                        if (objfield.Editable)
                        {
                            
                            destinationfieldindex = pChildTab.Fields.FindField(objfield.Name);
                            sourcefieldindex = pCedsaControllerRow.Fields.FindField(objfield.Name);

                            if (destinationfieldindex != -1 && sourcefieldindex != -1)
                            {
                                //if (objfield.Name.ToUpper() != "CONVERSIONID" && objfield.Name.ToUpper() != "DATEMODIFIED" && objfield.Name.ToUpper() != "LASTUSER")
                                if (objfield.Name.ToUpper() == "STATUS" || objfield.Name.ToUpper() == "SUBTYPECD" || objfield.Name.ToUpper() == "CONTROLLERTYPE" || objfield.Name.ToUpper() == "FIRMWAREVERSION" || objfield.Name.ToUpper() == "SOFTWAREVERSION" || objfield.Name.ToUpper() == "SERIALNUMBER")
                                {
                                    //Logs.writeLine(objfield.Name);
                                                                  
                                    if (objfield.Name.ToUpper() == "INSTALLJOBYEAR")
                                    {
                                        string str = _clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty);

                                        if (_clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty) != string.Empty)
                                        {
                                            strINSTALLATIONDATEVal = validateDate(_clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty));

                                            if (strINSTALLATIONDATEVal.Length > 0)
                                            {
                                                pRowBuffer.set_Value(destinationfieldindex, strINSTALLATIONDATEVal);
                                            }
                                        }
                                    }
                                    else if (objfield.Name.ToUpper() == "SUBTYPECD")
                                    {

                                        pRowBuffer.set_Value(destinationfieldindex, strSubtypeCdVal);
                                    
                                    }
                                    else if (objfield.Name.ToUpper() == "controllertype".ToUpper())
                                    {
                                        string strCtyp = _clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty);

                                        pRowBuffer.set_Value(destinationfieldindex, pCedsaControllerRow.get_Value(sourcefieldindex)); 
                                        //if (strCtyp.ToUpper() == "UVR1")
                                        //{
                                        //    pRowBuffer.set_Value(destinationfieldindex, "VR1");
                                        //}
                                        //else
                                        //{
                                        //    pRowBuffer.set_Value(destinationfieldindex, pCedsaControllerRow.get_Value(sourcefieldindex));                                        
                                        //}
                                    }
                                    else
                                    {
                                        string str = _clsGlobalFunctions.Cast(pCedsaControllerRow.get_Value(sourcefieldindex).ToString(), string.Empty);

                                        //Logs.writeLine(str);

                                        pRowBuffer.set_Value(destinationfieldindex, pCedsaControllerRow.get_Value(sourcefieldindex));
                                    }
                                }
                            }
                        }
                        
                    }
                    catch (Exception ex)
                    {
                        Logs.writeLine("Error @ retriving value :" + objfield.Name + " Cedsa Deviceid : "  +strCEDSADEVIdVal +" "+ ex.Message);
                    }
                }
                
                pRowBuffer.set_Value(pChildTab.FindField("CREATIONUSER"), "P1P4");
                pRowBuffer.set_Value(pChildTab.FindField("DATECREATED"), String.Format("{0:M/d/yyyy}", DateTime.Now));
                pRowBuffer.set_Value(pChildTab.Fields.FindField("DEVICEGUID"), strParentGuid);

                pInserCursor = pChildTab.Insert(false);
                pInserCursor.InsertRow(pRowBuffer);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error at insert RECORD for CEDSADEVID :" + strCEDSADEVIdVal + "  " + ex.Message);
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
                        strINSTALLATIONDATEVal = "19" + intYear ;
                    }
                }
                else if (strDate.Length == 1)
                {
                    strINSTALLATIONDATEVal = "200" + intYear;
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error Validating string:" + strDate +"  "+ ex.Message);
            }

            return strINSTALLATIONDATEVal;
        }

        private IRow getRecWithCedsaID(string strCEDSADEVIdVal, ITable pTab, string strColName)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                pFilter.WhereClause = "CEDSADEVICEID='" + strCEDSADEVIdVal + "'";

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pTab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("getFeature " + ex.Message);
                return pRow;

            }
            return pRow;

        }

        private IRow getRecWithCedsaID_vrunit(string strCEDSADEVIdVal, string strREALCEDSADEVIdVal, ITable pCedControlTab, string strColName)
        {
            ICursor pCursor = null;
            IRow pRow = null;
            IQueryFilter pFilter = new QueryFilterClass();

            try
            {
                pFilter.WhereClause = "CEDSADEVICEID='" + strREALCEDSADEVIdVal + "' AND " + strColName + "='" + strCEDSADEVIdVal + "'";

                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    pCursor = pCedControlTab.Search(pFilter, false);
                    cr.ManageLifetime(pCursor);
                    pRow = pCursor.NextRow();
                }
            }
            catch (Exception ex)
            {
                Logs.writeLine("getFeature " + ex.Message);
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

        private void txtFGDpath_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
