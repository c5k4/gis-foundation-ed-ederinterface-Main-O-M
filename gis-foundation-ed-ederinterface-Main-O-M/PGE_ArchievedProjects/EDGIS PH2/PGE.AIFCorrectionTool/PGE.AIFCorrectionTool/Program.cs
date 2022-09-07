using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PGE.AIFCorrection;
using System.IO;
using System.Configuration;
using Miner.Interop;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
namespace AIFCorrection
{
    class Program
    {

        private static LicenseInitializer m_AOLicenseInitializer = new LicenseInitializer();
        private static string sExceptionPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PROCESS_LOG\\ExceptionLogfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static string sPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PROCESS_LOG\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static string sCSVFilePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + cls_GlobalVariables.strAPPCONFIG_CSVNAME;
        private static StreamWriter pSWriter = default(StreamWriter);

        private static IMMAppInitialize arcFMAppInitialize = new Miner.Interop.MMAppInitializeClass();
        private static List<string> lstFCNames = new List<string>();
        private static ITable pCustAgrmntTable = default(ITable);
        private static Hashtable htFCRelNames = new Hashtable();
        private static Hashtable ignoredPMOrders = new Hashtable();
        private static int intCountPM = 0;

        static void Main(string[] args)
        {
            try
            {

                CreateDirectory(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\PROCESS_LOG");
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing License.");

                m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
                new esriLicenseExtensionCode[] { });
                mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
                if (RuntimeManager.ActiveRuntime == null)
                    RuntimeManager.BindLicense(ProductCode.Desktop);

                WriteLine(DateTime.Now.ToLongTimeString() + " -- License Successfully Initialized.");
                //Create Log File
                (pSWriter = File.CreateText(sPath)).Close();

                // Reading Connection Parameter
                string strEDGISSdeConn = cls_GlobalVariables.strAPPCONFIG_SDEFILEPATH;
                string strSdeVerName = cls_GlobalVariables.strAPPCONFIG_VERSIONNAME;
                WriteLine(DateTime.Now.ToLongTimeString() + "-- Sde File Path :" + strEDGISSdeConn);
                WriteLine(DateTime.Now.ToLongTimeString() + "-- Sde Version Name :" + strSdeVerName);

                DataTable DtcsvData = GetDataTableFromCSVFile(sCSVFilePath);
                Hashtable htPMNumbers = GetListFromDatatable(DtcsvData);

                intCountPM = htPMNumbers.Count;
                CreateGDBItemsList();
                //Open Sde Workspace
                IWorkspace objEDGISFW = ArcSdeWorkspaceFromFile(strEDGISSdeConn);
                if (objEDGISFW != null)
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + "-- SDE Workspace Successfully Opened. ");
                }
                IVersion pVersion = default(IVersion);
                pVersion = ((IVersionedWorkspace)objEDGISFW).FindVersion(strSdeVerName);
                if (pVersion != null)
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + "-- Version Successfully Opened. ");
                }
                //Read Customer Agreement Table
                string pCustAgrmntTableName = cls_GlobalVariables.strCustomer_Agreement_TableName;
                pCustAgrmntTable = (pVersion as IFeatureWorkspace).OpenTable(pCustAgrmntTableName);
                if (pCustAgrmntTable != null)
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + "-- Customer Agreement Table Successfully Opened. ");
                }
                RemoveExistPMOrders(htPMNumbers, objEDGISFW);
                //CreateWarrentyRecords(htPMNumbers, (pVersion as IWorkspace));

                if (CreateWarrentyRecords(htPMNumbers, (pVersion as IWorkspace)) == true)
                {

                    WriteLine(DateTime.Now.ToLongTimeString() + "-- Process End Successfully");
                }
                else
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + "-- Warranty Data not Updated Succesfully ");

                }

                //}

            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "-- Exception Occurred While Processing " + ex.Message);

            }
        }
        public static void CreateDirectory(string sLogFilePath)
        {
            try
            {
                if (Directory.Exists(sLogFilePath) == false)
                {
                    Directory.CreateDirectory(sLogFilePath);
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Correct Warrenty Information in the Customer Agreement Table.
        /// </summary>
        /// <param name="pVersion"></param>
        private static bool CreateWarrentyRecords(Hashtable htPMNumbers, IWorkspace pWorkSpace)
        {
            List<string> QueryStrings = PrepareQueryStrings(htPMNumbers);
            IQueryFilter pQFilter = default(IQueryFilter);
            IFeatureClass pSearchFeatureClass = default(IFeatureClass);
            IRelationshipClass pRelClass = default(IRelationshipClass);
            IFeature pQueriedFeature = default(IFeature);
            Dictionary<string, IRow> NewRelatedRecordList = new Dictionary<string, IRow>();
            IRow CustomerAgreementRow = default(IRow);
            string officeCode = string.Empty;
            try
            {

                WriteLine((DateTime.Now.ToLongTimeString() + "-- Start Editing "));
                ((IWorkspaceEdit)pWorkSpace).StartEditing(true);
                ((IWorkspaceEdit)pWorkSpace).StartEditOperation();
                int intCountRecords = 0;
                foreach (string strFC in lstFCNames)
                {
                    foreach (string strQueryString in QueryStrings)
                    {
                        pQFilter = new QueryFilterClass();
                        pQFilter.WhereClause = cls_GlobalVariables.strInstallJobNumber + " in (" + strQueryString + ")";
                        string strRelClass = htFCRelNames[strFC].ToString();
                        pSearchFeatureClass = (pWorkSpace as IFeatureWorkspace).OpenFeatureClass(strFC);
                        pRelClass = (pWorkSpace as IFeatureWorkspace).OpenRelationshipClass(strRelClass);
                        IFeatureCursor pFCursor = pSearchFeatureClass.Search(pQFilter, false);
                        pQueriedFeature = pFCursor.NextFeature();
                        while (pQueriedFeature != null)
                        {
                            bool boolExistWarrenty = false;
                            string strJobNumber = pQueriedFeature.get_Value(pQueriedFeature.Fields.FindField(cls_GlobalVariables.strInstallJobNumber)).ToString();
                            NewRelatedRecordList.TryGetValue(strJobNumber, out CustomerAgreementRow);

                            if (CustomerAgreementRow != null)
                            {
                                pRelClass.CreateRelationship((IObject)CustomerAgreementRow, (IObject)pQueriedFeature);
                            }
                            else
                            {

                                officeCode = pQueriedFeature.get_Value(pQueriedFeature.Fields.FindField(cls_GlobalVariables.strLocalOfficeID)).ToString();
                                CustomerAgreementRow = CreateCustAgreementRecord_New(pCustAgrmntTable, htPMNumbers, officeCode, strJobNumber, pQueriedFeature);
                                pRelClass.CreateRelationship((IObject)CustomerAgreementRow, (IObject)pQueriedFeature);
                                NewRelatedRecordList.Add(strJobNumber, CustomerAgreementRow);
                                WriteLine((DateTime.Now.ToLongTimeString() + "--Record successfully inserted in the Customer Agreement Table for ObjectID =  " + pQueriedFeature.get_Value(0).ToString() + " of Featureclass " + pSearchFeatureClass.AliasName + " and OrderNumber= " + strJobNumber + " CustAgree OID: " + CustomerAgreementRow.OID.ToString() + " : Successfull"));
                                intCountRecords = intCountRecords + 1;
                            }
                            pQueriedFeature = pFCursor.NextFeature();
                        }
                    }
                }
                int count = 0;
                foreach (string s in htPMNumbers.Keys)
                {
                    if (!NewRelatedRecordList.Keys.Contains(s) && !ignoredPMOrders.ContainsKey(s))
                    {
                        WriteLine((DateTime.Now.ToLongTimeString() + "--Customer Agreement Record is not created for PM Order as no feautes found in GIS  " + s));
                        count++;
                    }
                }
                ((IWorkspaceEdit)pWorkSpace).StopEditing(true);
                ((IWorkspaceEdit)pWorkSpace).StopEditOperation();

                WriteLine("Total number of records Processed  - " + intCountPM);
                WriteLine("Total number of Warranty records created - " + NewRelatedRecordList.Count);
                WriteLine("Total number of Existing records in GIS  - " + ignoredPMOrders.Count);
                WriteLine("Total number of records not in EDGIS  - " + count);

                return true;
            }

            catch (Exception ex)
            {
                WriteLine((DateTime.Now.ToLongTimeString() + "-- Stop Editing "));
                if (((IWorkspaceEdit)pWorkSpace).IsBeingEdited() == true)
                {
                    ((IWorkspaceEdit)pWorkSpace).StopEditing(false);
                    ((IWorkspaceEdit)pWorkSpace).StopEditOperation();
                }
                //bWarrantyDataupdated = false;
                WriteLine(DateTime.Now.ToLongTimeString() + "-- Exception Occurred While Updating the Warranty Data " + ex.Message);
                throw ex;
            }
            //return bWarrantyDataupdated;
        }

        private static IRow CreateCustAgreementRecord_New(ITable pCustAgrmntTable, Hashtable htPMNumbers, string officeCode, string sOrderNumber, IFeature pQueryFeature)
        {
            string Source_AIEXPIRATIONDATE = htPMNumbers[sOrderNumber].ToString();
            string Source_AGREEMENTTYPE = cls_GlobalVariables.strCustomerFieldValue_AgreementTypeValue;
            string Source_AGREEMENTNUM = DBNull.Value.ToString();
            string Source_OFFICECODE = string.Empty;
            try
            {
                DateTime DtSource_AIEXPIRATIONDATE = Convert.ToDateTime(Source_AIEXPIRATIONDATE);
                DtSource_AIEXPIRATIONDATE = DtSource_AIEXPIRATIONDATE.AddYears(2);
                DtSource_AIEXPIRATIONDATE = DtSource_AIEXPIRATIONDATE.AddDays(-1);
                Source_AIEXPIRATIONDATE = DtSource_AIEXPIRATIONDATE.ToShortDateString();
                Source_AGREEMENTTYPE = cls_GlobalVariables.strCustomerFieldValue_AgreementTypeValue;
                Source_AGREEMENTNUM = DBNull.Value.ToString();
                IRow pNewRow = pCustAgrmntTable.CreateRow();
                pNewRow.set_Value(pNewRow.Fields.FindField(cls_GlobalVariables.strCustomer_Agreement_Field_AGREEMENTTYPE), Source_AGREEMENTTYPE);
                pNewRow.set_Value(pNewRow.Fields.FindField(cls_GlobalVariables.strCustomer_Agreement_Field_AGREEMENTNUM), Source_AGREEMENTNUM);
                pNewRow.set_Value(pNewRow.Fields.FindField(cls_GlobalVariables.strCustomer_Agreement_Field_AIEXPIRATIONDATE), Source_AIEXPIRATIONDATE);
                pNewRow.set_Value(pNewRow.Fields.FindField(cls_GlobalVariables.strCustomer_Agreement_Field_ATEXPIRATIONDATE), DBNull.Value);
                pNewRow.set_Value(pNewRow.Fields.FindField(cls_GlobalVariables.strCustomer_Agreement_Field_AGREEMENTCOMMENTS), sOrderNumber);
                pNewRow.set_Value(pNewRow.Fields.FindField(cls_GlobalVariables.strCustomer_Agreement_Field_OFFICECODE), officeCode);
                pNewRow.Store();
                return pNewRow;

            }
            catch (Exception)
            {
                WriteLine("Error in creating new record for " + (pQueryFeature.Class as IDataset).Name.ToString() + " Object id: " + sOrderNumber);
                throw;
            }
        }

        private static Hashtable RemoveExistPMOrders(Hashtable HtInputData, IWorkspace pWorkSpace)
        {

            string sCSV_OrderNumber1 = string.Empty;
            int RecordCount = 0, Iteration = 1;
            List<string> QueryStrings = PrepareQueryStrings(HtInputData);
            IFeature pQueriedFeature = default(IFeature);
            IQueryFilter pQFilter = default(IQueryFilter);
            IFeatureClass pSearchFeatureClass = default(IFeatureClass);
            IRelationshipClass pRelClass = default(IRelationshipClass);

            try
            {


                foreach (string strFC in lstFCNames)
                {
                    foreach (string strQueryString in QueryStrings)
                    {
                        pQFilter = new QueryFilterClass();
                        pQFilter.WhereClause = cls_GlobalVariables.strInstallJobNumber + " in (" + strQueryString + ")";
                        string strRelClass = htFCRelNames[strFC].ToString();
                        pSearchFeatureClass = (pWorkSpace as IFeatureWorkspace).OpenFeatureClass(strFC);
                        pRelClass = (pWorkSpace as IFeatureWorkspace).OpenRelationshipClass(strRelClass);
                        IFeatureCursor pFCursor = pSearchFeatureClass.Search(pQFilter, false);
                        pQueriedFeature = pFCursor.NextFeature();
                        while (pQueriedFeature != null)
                        {
                            bool boolExistWarrenty = false;
                            string strJobNumber = pQueriedFeature.get_Value(pQueriedFeature.Fields.FindField(cls_GlobalVariables.strInstallJobNumber)).ToString();
                            ISet pResultSet = pRelClass.GetObjectsRelatedToObject(pQueriedFeature);
                            object pRelatedObj = pResultSet.Next();
                            if (HtInputData.ContainsKey(strJobNumber))
                            {
                                while (pRelatedObj != null)
                                {
                                    IRow prow = (IRow)pRelatedObj;
                                    string sType = prow.get_Value(prow.Fields.FindField(cls_GlobalVariables.strCustomer_Agreement_Field_AGREEMENTTYPE)).ToString();
                                    if (sType.ToUpper() == cls_GlobalVariables.strCustomerFieldValue_AgreementTypeValue)
                                    {
                                        HtInputData.Remove(strJobNumber);
                                        ignoredPMOrders.Add(strJobNumber, "");
                                        boolExistWarrenty = true;
                                        WriteLine("Warranty information is available for ObjectId : " + pQueriedFeature.OID + " of " + pQueriedFeature.Class.AliasName.ToString() + "For PM Order :  " + strJobNumber);
                                        break;
                                    }
                                    pRelatedObj = pResultSet.Next();
                                }

                            }
                            pQueriedFeature = pFCursor.NextFeature();
                        }
                    }
                }

            }
            catch
            {

            }
            finally
            {
                //if (pFCursor != null) Marshal.ReleaseComObject(pFCursor);
                if (pQueriedFeature != null) Marshal.ReleaseComObject(pQueriedFeature);
                if (pSearchFeatureClass != null) Marshal.ReleaseComObject(pSearchFeatureClass);
                if (pRelClass != null) Marshal.ReleaseComObject(pRelClass);

            }
            return HtInputData;
        }



        private static List<string> PrepareQueryStrings(Hashtable HtInputData)
        {
            string strQuerystring = string.Empty;
            List<string> lstQueryString = new List<string>();
            try
            {

                int i = 1;
                foreach (string s in HtInputData.Keys)
                {

                    strQuerystring = strQuerystring + "'" + s + "',";
                    if (i % 1000 == 0)
                    {
                        lstQueryString.Add(strQuerystring.Remove(strQuerystring.Length - 1, 1));
                        strQuerystring = "";
                    }

                    if (i == HtInputData.Keys.Count)
                    {
                        lstQueryString.Add(strQuerystring.Remove(strQuerystring.Length - 1, 1));
                        strQuerystring = "";
                    }
                    i++;
                }

            }
            catch (Exception ex)
            {
                WriteLine("Preparing query strings");
                throw ex;
            }
            return lstQueryString;
        }

        private static void Retrun_CSVRecordNumber(int RecordCount, DataRow dr, ref string sReturnCSV_OrderNumber, int RemainingRecord, int Iteration, int FractionNumber)
        {

            try
            {
                if (RecordCount == 1)
                {
                    sReturnCSV_OrderNumber = "('" + dr[cls_GlobalVariables.strCSV_ORDERNUMBER].ToString() + "'";
                }
                //else if (RecordCount == 1000 || ((Iteration>FractionNumber ) & ( RecordCount == RemainingRecord )))                   
                //{
                //    sReturnCSV_OrderNumber = ",'" + sReturnCSV_OrderNumber + dr[cls_GlobalVariables.strCSV_ORDERNUMBER].ToString() + "')";
                //}

                else
                {
                    sReturnCSV_OrderNumber = sReturnCSV_OrderNumber + ",'" + dr[cls_GlobalVariables.strCSV_ORDERNUMBER].ToString() + "'";
                }

            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "-- Exception occurred while generating the CSV Order Number String " + ex.Message);
            }

        }
        /// <summary>
        /// Fill Queried Featureclasses
        /// </summary>
        /// <param name="pVersion"></param>

        /// <summary>
        /// Set AIExpirationDate as per Business Rule 
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        /// 

        private static void CreateGDBItemsList()
        {
            string[] QueriedFeatureClasses = new string[] { }; ;
            string[] QueriedRelClasses = new string[] { }; ;

            try
            {

                QueriedFeatureClasses = ConfigurationManager.AppSettings["QUERIED_FEATURECLASSES"].Split(',');
                lstFCNames = QueriedFeatureClasses.ToList();
                QueriedRelClasses = ConfigurationManager.AppSettings["QUERIED_RELCLASSES"].Split(',');
                for (int i = 0; i < QueriedFeatureClasses.Length; i++)
                {
                    htFCRelNames.Add(QueriedFeatureClasses[i], QueriedRelClasses[i]);
                }


            }
            catch (Exception)
            {
                WriteLine("Configuration Missing for featureclasses/Relation ship classes");
                throw;
            }

        }

        private static string Return_AIEXPIRATIONDATE(DataRow dr)
        {
            string returnDate = string.Empty;
            string sDate = string.Empty;
            try
            {

                bool blnValidDESCDate = false;
                if (dr.Table.Columns.Contains(cls_GlobalVariables.strCSV_DESCDATE) == true)
                {
                    if (!string.IsNullOrEmpty(dr[cls_GlobalVariables.strCSV_DESCDATE].ToString()))
                    {
                        try
                        {
                            string decsdate = dr[cls_GlobalVariables.strCSV_DESCDATE].ToString();
                            DateTime dt = DateTime.Parse(decsdate);
                            blnValidDESCDate = true;

                        }
                        catch
                        {
                        }
                    }
                }
                if (blnValidDESCDate == true)
                {

                    DateTime Source_AIEXPIRATIONDATE = Convert.ToDateTime(dr[cls_GlobalVariables.strCSV_DESCDATE].ToString());

                    Source_AIEXPIRATIONDATE = Source_AIEXPIRATIONDATE.AddYears(2);
                    Source_AIEXPIRATIONDATE = Source_AIEXPIRATIONDATE.AddDays(-1);

                    returnDate = Source_AIEXPIRATIONDATE.ToShortDateString();
                }
                else
                {
                    if (!string.IsNullOrEmpty(dr[cls_GlobalVariables.strCSV_CN24DATE].ToString()))
                    {
                        string sCSV_CN24Date = dr[cls_GlobalVariables.strCSV_CN24DATE].ToString();

                        DateTime Source_AIEXPIRATIONDATE = Convert.ToDateTime(sCSV_CN24Date);

                        Source_AIEXPIRATIONDATE = Source_AIEXPIRATIONDATE.AddYears(2);
                        Source_AIEXPIRATIONDATE = Source_AIEXPIRATIONDATE.AddDays(-1);

                        returnDate = Source_AIEXPIRATIONDATE.ToShortDateString();
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnDate;
        }

        public static DataTable GetDataTableFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    //read column names
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return csvData;
        }

        public static Hashtable GetListFromDatatable(DataTable dt)
        {
            Hashtable ht = new Hashtable();
            string CSV_COLUMN_ORDER_NO = ConfigurationManager.AppSettings["CSV_COLUMN_ORDER_NO"].ToString();
            string CSV_COLUMN_CN24_DATE = ConfigurationManager.AppSettings["CSV_COLUMN_CN24_CompDate"].ToString();
            string CSV_COLUMN_DESC_DATE = ConfigurationManager.AppSettings["CSV_COLUMN_DESC_DATE"].ToString();
            //DateTime dtWarrentyDate = new DateTime();
            bool blnValidDESCDate = false;
            try
            {
                foreach (DataRow r in dt.Rows)
                {
                    if (!ht.ContainsKey(CSV_COLUMN_ORDER_NO))
                    {
                        blnValidDESCDate = false;
                        try
                        {
                            string decsdate = r[CSV_COLUMN_DESC_DATE].ToString();
                            DateTime dtWarrentyDate = new DateTime();
                            if (DateTime.TryParse(decsdate, out dtWarrentyDate))
                            {
                                //dtWarrentyDate = DateTime.Parse(decsdate);
                                blnValidDESCDate = true;
                                if (!ht.ContainsKey(r[CSV_COLUMN_ORDER_NO]))
                                    ht.Add(r[CSV_COLUMN_ORDER_NO], r[CSV_COLUMN_DESC_DATE]);
                            }

                        }
                        catch
                        {
                        }
                        if (!blnValidDESCDate)
                        {
                            if (!ht.ContainsKey(r[CSV_COLUMN_ORDER_NO]))
                                ht.Add(r[CSV_COLUMN_ORDER_NO], r[CSV_COLUMN_CN24_DATE]);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ht;
        }



        /// <summary>
        /// Set SDEWorkSpace from sde connection file path
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <returns></returns>
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            try
            {
                return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                    OpenFromFile(connectionFile, 0);
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + "--Error occurred while setting up the sde connection- " + ex.Message.ToString());
                throw ex;
            }
        }
        /// <summary>
        /// Update log
        /// </summary>
        /// <param name="sMsg"></param>
        private static void WriteLine(string sMsg)
        {
            try
            {

                pSWriter = File.AppendText(sPath);
                pSWriter.WriteLine(sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                //Console.WriteLine("\n");
                pSWriter.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// ExceptionLog log
        /// </summary>
        /// <param name="sMsg"></param>
        private static void WriteLineInExceptionLog(string sMsg)
        {
            try
            {

                pSWriter = File.AppendText(sExceptionPath);
                pSWriter.WriteLine(sMsg);
                //DrawProgressBar();
                Console.WriteLine(sMsg);
                //Console.WriteLine("\n");
                pSWriter.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// CheckOut ArcFm License
        /// </summary>
        /// <param name="productCode"></param>
        /// <returns></returns>
        private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        {
            mmLicenseStatus licenseStatus;
            try
            {

                licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
                if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
                {
                    licenseStatus = arcFMAppInitialize.Initialize(productCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return licenseStatus;
        }
    }
}
