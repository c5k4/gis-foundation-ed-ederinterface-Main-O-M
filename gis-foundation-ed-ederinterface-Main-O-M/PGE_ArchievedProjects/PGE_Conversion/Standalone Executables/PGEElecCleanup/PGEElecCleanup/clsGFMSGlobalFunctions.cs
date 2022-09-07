using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using System.Configuration;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using ESRI.ArcGIS.Carto;
using System.Runtime.InteropServices;

namespace PGEElecCleanup
{
    class clsGFMSGlobalFunctions
    {
        public static clsGFMSGlobalFunctions _GFMSGlobalFunctions = new clsGFMSGlobalFunctions();
        private IMMAutoUpdater autoupdater = null;
        public StatusBar objStatusbar = new StatusBar();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strExcelQueryString"></param>
        /// <param name="strSheetname"></param>
        /// <param name="strColumsname"></param>
        /// <param name="myInputFolder"></param>
        /// <param name="myInputFile"></param>
        /// <returns></returns>
        public static DataTable GetDatafromExcel(string strExcelQueryString, string strSheetname, string strColumsname,
            string myInputFolder,string myInputFile)
        {
            OleDbConnection myAccessConn = null;
            DataTable myGFMSDt = new DataTable();
            
            try
            {
                //string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                //            "Data Source=" + myInputFolder + @"\" + myInputFile + ";" +
                //            "Extended Properties='Excel 8.0;HDR=YES;IMEX=2'";
                myAccessConn = getConnection(myInputFolder, myInputFile);

                string strAccessSelect = strExcelQueryString + " [" + strSheetname + "]";//"SELECT SWI_NO,UFID,AMPS FROM [" + strSheetName + "]";

                string[] strColoumname = strColumsname.Split(',');
                foreach (string FiledName in strColoumname)
                {
                    myGFMSDt.Columns.Add(FiledName);
                }
                
                //myAccessConn = new OleDbConnection(strAccessConn);
                OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect, myAccessConn);
                OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
                myAccessConn.Open();

                try
                {
                    myDataAdapter.Fill(myGFMSDt);
                    return myGFMSDt;
                }
                finally
                {
                    myAccessConn.Close();
                }
            }
            catch(Exception ex)
            {
                Logs.writeLine("Error while getting data from Excel sheet."+ ex.Message.ToString());
                return myGFMSDt = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myInputFolder"></param>
        /// <param name="myInputFile"></param>
        /// <returns></returns>
        public static OleDbConnection getConnection(string myInputFolder, string myInputFile)
        { 
            OleDbConnection myAccessConn = null;
            try
            {
                string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;" +
                            "Data Source=" + myInputFolder + @"\" + myInputFile + ";" +
                            "Extended Properties='Excel 8.0;HDR=YES;IMEX=2'";
                myAccessConn = new OleDbConnection(strAccessConn);
                return myAccessConn;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error: Connecting to the excel file." + ex.Message.ToString());
                return myAccessConn;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myInputFolder"></param>
        /// <param name="myInputFile"></param>
        /// <returns></returns>
        public static string[] GetExcelSheet(string myInputFolder, string myInputFile)
        {
            OleDbConnection objConn = null;
            System.Data.DataTable dt = null;
            string[] excelSheets=null;
            try
            {
                objConn = getConnection(myInputFolder, myInputFile);
                
                // Open connection with the database.
                objConn.Open();

                // Get the data table containg the schema guid.
                dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                if (dt == null)
                {
                    string[] Error = null;
                    return Error; ;
                }
                excelSheets = new String[dt.Rows.Count];
                int i = 0;
               
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[i] = row["TABLE_NAME"].ToString();
                        //excelSheets = row["TABLE_NAME"].ToString();
                        i++;
                    }
              
                
                return excelSheets;
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error In getting Excel sheet:" + ex.ToString());
                String[] EexcelSheets=null;
                EexcelSheets[0] = "Error:Unable to get excelsheet.";
                return EexcelSheets;
            }
            finally
            {
                objConn.Close();
            }
        }

        /// <summary>
        /// Get the subtype of the feature.
        /// </summary>
        /// <param name="pFeatureClass">FeatureClass</param>
        /// <param name="pFeature">Feature</param>
        /// <returns>Subtype name</returns>
        public static string getSubtype(IFeature pFeature)
        {
            string strSubtypeName = string.Empty;
            try
            {
                IRowSubtypes pRowSubtype = (IRowSubtypes)pFeature;
                ISubtypes pSubtypes = (ISubtypes)pFeature.Class;
                int intSubtypeCode = pRowSubtype.SubtypeCode;
                strSubtypeName = pSubtypes.get_SubtypeName(intSubtypeCode);
                return strSubtypeName;
            }
            catch(Exception ex)
            {
                throw (ex);
                strSubtypeName = "Error";
                return strSubtypeName;
            }
            
         }

        public static bool validateSubtype(string inputSubtype, string FeatureSubtype)
        {
            string[] strSubtypelist = inputSubtype.Split(',');
            List<string> strSubtypes = new List<string>(strSubtypelist);
            if (strSubtypes.Contains(FeatureSubtype))
            {
                return true;
            }
            else
            {
                return false;
            }
                
        }

        public void updateMaxContinuousCurrentValue(IFeatureClass pFC, DataTable dt, string TypeOfQuery)
        {

            IFeatureCursor pFeatureCursor = null;
            StringBuilder objStrBuilder = new StringBuilder();
            DataTable objReportdt = null;

            if (TypeOfQuery == "Recloser")
            {
                 
                objReportdt = CreateRecloserReportTable();
            }
            else if (TypeOfQuery == "Fuse")
            {
                objReportdt = CreateFuseReportTable();
            }
            IQueryFilter pQueryFilter = null;
            int intFeatureCount = 0;
            try
            {


                objStatusbar.Text = "Disabling autoupdaters...";
                Application.DoEvents();

                #region "Disable Autoupdaters "
                mmAutoUpdaterMode objautoupdateroldmode = DisableAutoupdaters();
                #endregion

                objStatusbar.Text = "Stoping auto creation of annotations...";
                Application.DoEvents();

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

                bool test = clsTestWorkSpace.StartEditOperation();

                int intFacilityID = pFC.FindField(ConfigurationManager.AppSettings["FacilityID"]);
                int intExteralID = pFC.FindField(ConfigurationManager.AppSettings["ExteralID"]);
                int intMaxContinuousCurrent = pFC.FindField(ConfigurationManager.AppSettings["MAXCONTINUOUSCURRENT"]);
                int intSubTypCDIdx = pFC.FindField("SUBTYPECD");
                string strSubtypeCdVal = string.Empty;


                string strSWI_NO = string.Empty;
                string strUFID = string.Empty;
                string strAMPS = string.Empty;
                string strID = string.Empty;

                foreach (DataRow dr in dt.Rows)
                {
                    pQueryFilter = new QueryFilterClass();
                    pQueryFilter.SubFields = ConfigurationManager.AppSettings["MAXCONTINUOUSCURRENT"];

                    if (TypeOfQuery == "Recloser")
                    {
                        objStatusbar.Text = "Processing..EXTERNALID " + dr.ItemArray[2].ToString(); 
                        Application.DoEvents();

                        strAMPS = dr.ItemArray[0].ToString();
                        strID = dr.ItemArray[1].ToString();
                        strUFID = dr.ItemArray[2].ToString();
                       
                       //pQueryFilter.WhereClause = "FACILITYID ='" + strID + "' AND EXTERNALID ='" + strUFID + "'AND ( SUBTYPECD = 1 OR SUBTYPECD = 2 OR SUBTYPECD = 3 )";
                        pQueryFilter.WhereClause = "FACILITYID ='" + strID + "' AND EXTERNALID ='" + strUFID + "'" ;
                    }
                    else if (TypeOfQuery == "Fuse")
                    {
                        strAMPS = dr.ItemArray[0].ToString();
                        strUFID = dr.ItemArray[1].ToString();

                        objStatusbar.Text = "Processing..EXTERNALID " + dr.ItemArray[1].ToString(); 
                        Application.DoEvents();


                        //pQueryFilter.WhereClause = "EXTERNALID ='" + strUFID + "' AND SUBTYPECD = 5 ";
                        pQueryFilter.WhereClause = "EXTERNALID ='" + strUFID +"'" ;
                    }

                    //if (strUFID == "0x381706F300E9000101B30000")
                    //{
                    //    MessageBox.Show("Success");

                    //}
                    pFeatureCursor = pFC.Search(pQueryFilter, false);
                    intFeatureCount = pFC.FeatureCount(pQueryFilter);
                    
                    IFeature pFeature = pFeatureCursor.NextFeature();
                   
                    int intOID = 0;

                    string strRemarks = string.Empty;
                    string AMPSValue = string.Empty;
                    string strTempMaxCC = string.Empty;
                    strSubtypeCdVal = "";
                    if (intFeatureCount != 0 || pFeature != null)
                    {
                        strSubtypeCdVal = pFeature.get_Value(intSubTypCDIdx).ToString();
                        if (pFeature.get_Value(intMaxContinuousCurrent) == DBNull.Value ||
                                                                              pFeature.get_Value(intMaxContinuousCurrent) == null)
                        {
                            strTempMaxCC = "";
                        }
                        else
                        {
                            strTempMaxCC = pFeature.get_Value(intMaxContinuousCurrent).ToString();
                        }
                    }


                    if (intFeatureCount == 0 || pFeature == null)
                    {
                        strRemarks = "Feature not found in GIS Database.";
                        intOID = 0;
                        if (TypeOfQuery == "Recloser")
                        {
                            AddRecloserReportRow(pFC,objReportdt, intOID,"",strTempMaxCC,strID, strUFID, strAMPS,"" ,strRemarks);
                        }
                        else
                        { 
                            AddFuseReportRow(pFC,objReportdt, intOID,"",strTempMaxCC,strUFID, strAMPS,"", strRemarks);
                        }
                        
                    }
                    else if (intFeatureCount > 1)
                    {
                        while (pFeature != null)
                        {
                            strSubtypeCdVal = pFeature.get_Value(intSubTypCDIdx).ToString();
                            strRemarks = "More than one Feature found in GIS Database.";
                            intOID = pFeature.OID;
                            if (TypeOfQuery == "Recloser")
                            {
                                if (strSubtypeCdVal != "1" || strSubtypeCdVal != "2" || strSubtypeCdVal != "3")
                                {
                                    strRemarks = "More than one Feature found in GIS Database. And Subtypecd wrong";
                                    
                                }
                                AddRecloserReportRow(pFC,objReportdt, intOID, strSubtypeCdVal, strTempMaxCC, strID, strUFID, strAMPS, "", strRemarks);
                            }
                            else
                            {
                                if (strSubtypeCdVal != "5")
                                {
                                    strRemarks = "More than one Feature found in GIS Database. And Subtypecd wrong"; 

                                }                               
                                AddFuseReportRow(pFC,objReportdt, intOID, strSubtypeCdVal, strTempMaxCC, strUFID, strAMPS, "", strRemarks);
                            }
                            pFeature = pFeatureCursor.NextFeature();
                        }
                    }
                    else if (intFeatureCount == 1)
                    {
                        if (dr.ItemArray[0] == DBNull.Value || strAMPS == null || strAMPS == "")
                        {
                            if (strTempMaxCC.Length > 0)
                            {
                                //pFeature.set_Value(intMaxContinuousCurrent, "NULL");
                                //pFeature.Store();
                                strRemarks = "AMPS Value is NUll. Null is updated in the GIS feature.";
                                intOID = pFeature.OID;
                                if (TypeOfQuery == "Recloser")
                                {

                                    if (strSubtypeCdVal == "1" || strSubtypeCdVal == "2" || strSubtypeCdVal == "3")
                                    {
                                        pFeature.set_Value(intMaxContinuousCurrent, "");
                                        pFeature.Store();

                                    }
                                    else
                                    {
                                        //strRemarks = "Wrong subtype";
                                        strRemarks = "Not matching to conditions or Invalid data";
                                    }

                                    AddRecloserReportRow(pFC,objReportdt, intOID,strSubtypeCdVal,strTempMaxCC, strID, strUFID, strAMPS, pFeature.get_Value(intMaxContinuousCurrent).ToString(), strRemarks);
                                }
                                else
                                {
                                    if (strSubtypeCdVal == "5")
                                    {
                                        pFeature.set_Value(intMaxContinuousCurrent, "");
                                        pFeature.Store();
                                    }
                                    else
                                    {

                                        //strRemarks = "Wrong subtype";
                                        strRemarks = "Not matching to conditions or Invalid data";
                                    }
                                    AddFuseReportRow(pFC,objReportdt, intOID,strSubtypeCdVal, strTempMaxCC, strUFID, strAMPS, pFeature.get_Value(intMaxContinuousCurrent).ToString(), strRemarks);
                                }
                            }
                        }
                        else
                        {
                            if (strTempMaxCC != strAMPS)
                            {
                                //pFeature.set_Value(intMaxContinuousCurrent, strAMPS);                                
                                //pFeature.Store();
                                strRemarks = "MaxContinuousCurrent value is updated based AMPS Value.";
                                intOID = pFeature.OID;

                                if (TypeOfQuery == "Recloser")
                                {
                                    if (strSubtypeCdVal == "1" || strSubtypeCdVal == "2" || strSubtypeCdVal == "3")
                                    {
                                        pFeature.set_Value(intMaxContinuousCurrent, strAMPS);
                                        pFeature.Store();                                   

                                    }
                                    else
                                    {
                                        //strRemarks = "Wrong subtype"; 
                                        strRemarks = "Not matching to conditions or Invalid data"; 
                                        
                                    }
                                    AddRecloserReportRow(pFC,objReportdt, intOID, strSubtypeCdVal, strTempMaxCC, strID, strUFID, strAMPS, pFeature.get_Value(intMaxContinuousCurrent).ToString(), strRemarks);
                                }
                                else
                                {
                                    if (strSubtypeCdVal == "5")
                                    {
                                        pFeature.set_Value(intMaxContinuousCurrent, strAMPS);
                                        pFeature.Store();                                        
                                    }
                                    else
                                    {
                                        
                                        //strRemarks = "Wrong subtype";
                                        strRemarks = "Not matching to conditions or Invalid data";
                                    }
                                    AddFuseReportRow(pFC,objReportdt, intOID, strSubtypeCdVal, strTempMaxCC, strUFID, strAMPS, pFeature.get_Value(intMaxContinuousCurrent).ToString(), strRemarks);
                                }
                            }
                        }
                    }
                }

                objStatusbar.Text = "Reseting auto creation of annotations...";
                Application.DoEvents();
                #region "Start Auto Creation of Annotations in Destination database"

                // Sbmessage.Panels["stsBarPanelToolStatus"].Text = "Start Auto Creation of Annotations in Destination database......";
                DestinationEnumdataset.Reset();
                Destinationdataset = DestinationEnumdataset.Next();
                while (Destinationdataset != null)
                {
                    Application.DoEvents();
                    AutoAnotationcreate(Destinationdataset, true, ref objarraylist);
                    Destinationdataset = DestinationEnumdataset.Next();
                }
                #endregion

                objStatusbar.Text = "Enabling autoupdaters...";
                Application.DoEvents();
                #region start AU
                if (autoupdater != null)
                {
                    autoupdater.AutoUpdaterMode = objautoupdateroldmode;
                }
                #endregion


            }
            catch (Exception ex)
            {
                objStrBuilder.AppendLine("Error: Updating FuseAMPS - " + ex.Message.ToString());
                File.AppendAllText("C:\\ApplicationError.txt", objStrBuilder.ToString());
            }
            finally
            {
                clsTestWorkSpace.StopEditOperation(true);
                pFeatureCursor = null;
                string strFCname = pFC.AliasName;
                GenerateTheReport(objReportdt, strFCname + "_GFMS_" + TypeOfQuery+"_AMPS");
                if (pFeatureCursor!=null)
                    Marshal.ReleaseComObject(pFeatureCursor);
                if (objReportdt != null)
                    objReportdt.Dispose();

            }
        }

        private DataTable CreateRecloserReportTable()
        {
            DataTable ReportTable = new DataTable();

            ReportTable.Columns.Add("FearureOID");
            ReportTable.Columns.Add("SubtypeName");
            ReportTable.Columns.Add("SubtypeID");
            ReportTable.Columns.Add("MaxContinuousCurrent");
            ReportTable.Columns.Add("ID");
            ReportTable.Columns.Add("UFID");
            ReportTable.Columns.Add("AMPS");
            ReportTable.Columns.Add("UpdatedValue");
            ReportTable.Columns.Add("Remarks");
            return ReportTable;


        }

        private DataTable CreateFuseReportTable()
        {
            DataTable ReportTable = new DataTable();

            ReportTable.Columns.Add("FearureOID");
            ReportTable.Columns.Add("SubtypeName");
            ReportTable.Columns.Add("SubtypeID");
            ReportTable.Columns.Add("MaxContinuousCurrent");
            ReportTable.Columns.Add("UFID");
            ReportTable.Columns.Add("AMPS");
            ReportTable.Columns.Add("UpdatedValue");
            ReportTable.Columns.Add("Remarks");
            return ReportTable;


        }

        private void AddRecloserReportRow(IFeatureClass pFC,DataTable objdt, int intOID, string strSubtypeCdVal, string strMAxCC, string strSWI_NO, string strUFID, string AMPS, string strUpdatedValue, string strRemarks)
        {
            string SrtDecription = string.Empty;
            try
            {
                if (strSubtypeCdVal.Length > 0)
                {
                    SrtDecription = getDescriptionFromCodeVal(pFC, strSubtypeCdVal);
                }
                else
                {
                    SrtDecription = strSubtypeCdVal;
                }

                DataRow objReoprtRow = objdt.NewRow();
                objReoprtRow["FearureOID"] = intOID; 
                objReoprtRow["SubtypeName"] = SrtDecription;
                objReoprtRow["SubtypeID"] = strSubtypeCdVal;
                objReoprtRow["MaxContinuousCurrent"] = strMAxCC;
                objReoprtRow["ID"] = strSWI_NO;
                objReoprtRow["UFID"] = strUFID;
                objReoprtRow["AMPS"] = AMPS;
                objReoprtRow["UpdatedValue"] = strUpdatedValue;
                objReoprtRow["Remarks"] = strRemarks;
                objdt.Rows.Add(objReoprtRow);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while adding row in to datatable." + ex.Message.ToString());
            }
        }

        private void AddFuseReportRow(IFeatureClass pFC,DataTable objdt, int intOID,string strSubtypeCdVal, string strMAxCC, string strUFID, string AMPS,string strUpdatedValue, string strRemarks)
        {
            string SrtDecription = string.Empty;
            try
            {
                if (strSubtypeCdVal.Length > 0)
                {
                    SrtDecription = getDescriptionFromCodeVal(pFC,strSubtypeCdVal);
                }
                else
                {
                    SrtDecription = strSubtypeCdVal;
                }

                DataRow objReoprtRow = objdt.NewRow();
                objReoprtRow["FearureOID"] = intOID;
                objReoprtRow["SubtypeName"] = SrtDecription;
                objReoprtRow["SubtypeID"] = strSubtypeCdVal;
                objReoprtRow["MaxContinuousCurrent"] = strMAxCC;
                objReoprtRow["UFID"] = strUFID;
                objReoprtRow["AMPS"] = AMPS;
                objReoprtRow["UpdatedValue"] = strUpdatedValue;
                objReoprtRow["Remarks"] = strRemarks;
                objdt.Rows.Add(objReoprtRow);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while adding row in to datatable." + ex.Message.ToString());
            }
        }

        public string getDescriptionFromCodeVal(IFeatureClass pFC,string strSubtypeCdVal)
        {
            string strSubtype = String.Empty;

            try
            {

                ISubtypes pSubTyps = (ISubtypes)pFC;
                int subtypeCode;
                string subtypeName = string.Empty;

                if (pSubTyps.HasSubtype == true)
                {
                    IEnumSubtype pEnumSubtype = pSubTyps.Subtypes;
                    subtypeName = pEnumSubtype.Next(out subtypeCode);

                    while (subtypeName != null)
                    {
                        if (subtypeCode == int.Parse(strSubtypeCdVal))
                        {
                            strSubtype = subtypeName;
                            subtypeName = null;
                        }
                        else
                        {
                            subtypeName = pEnumSubtype.Next(out subtypeCode);
                        }
                    }
                }
                return strSubtype;
            }
            catch(Exception ex)
            {
                return strSubtype = strSubtypeCdVal;
            }
        }

        public void GenerateTheReport(DataTable ReportTable, string strFCName)
        {
            try
            {
                string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                string strReporthpath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString() + strTime + " " + strFCName + ".CSV";
                clsGlobalFunctions._globalFunctions.DataTable2CSV(ReportTable, strReporthpath, true);
                MessageBox.Show("Process Completed, please see the Report File.", "Report", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while generating report. Error Details: "+ ex.Message.ToString());
            }
            
        }

        public void GenerateTheReport_MultipleFeatureClass(DataTable ReportTable, string strFCName)
        {
            try
            {
                string strTime = DateTime.Today.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                string strReporthpath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString() + strTime + " " + strFCName + ".CSV";
                clsGlobalFunctions._globalFunctions.DataTable2CSV(ReportTable, strReporthpath, true);
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while generating report. Error Details: " + ex.Message.ToString());
            }

        }

        public void GenerateTheReport_MultipleFeatureClass(DataTable ReportTable, string strFCName, bool blnHeader, string strTime)
        {
            try
            {
                string strReporthpath = System.Configuration.ConfigurationManager.AppSettings["LogFilePath"].ToString() + strTime + " " + strFCName + ".CSV";
                clsGlobalFunctions._globalFunctions.DataTable2CSV(ReportTable, strReporthpath, blnHeader);              
            }
            catch (Exception ex)
            {
                Logs.writeLine("Error while generating report. Error Details: " + ex.Message.ToString());
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
                                    // objfeatureclass = (IFeatureClass)enudataset;
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

       
        public string updateStaus
        {
            set
            {
                objStatusbar.Text = value;
                Application.DoEvents();
            }
        }
        internal static void _gerUniqueFieldVals(ITable pTab, string strFldName, ref List<string> lstUniqueFldVals)
        {
            try
            {
                ICursor cursor = null;

                int intIdx = pTab.FindField(strFldName);
                if (intIdx == -1)
                {
                    Logs.writeLine("Field not Exists. " + ((IDataset)pTab).Name + " : Field Name " + strFldName);
                    return;
                }
                using (ESRI.ArcGIS.ADF.ComReleaser cr = new ESRI.ArcGIS.ADF.ComReleaser())
                {
                    cursor = pTab.Search(null, false);
                    cr.ManageLifetime(cursor);
                    IDataStatistics dataStatistics = new DataStatisticsClass();
                    dataStatistics.Field = strFldName;
                    dataStatistics.Cursor = cursor;
                    System.Collections.IEnumerator enumerator = dataStatistics.UniqueValues;
                    enumerator.Reset();
                    while (enumerator.MoveNext())
                    {
                        object myObject = enumerator.Current;
                        lstUniqueFldVals.Add(myObject.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logs.writeLine("Error at _gerUniqueFieldVals. TableName: " + ((IDataset)pTab).Name + " : Field Name " + strFldName + " " + ex.Message);
            }

        }
        public void addUsersToList(ref List<string> lstUsersList)
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
    }
}
