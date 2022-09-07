using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.ComponentModel;
using System.ServiceModel.Activation;
using ArcFMSilverlight.Web;
using ArcFMSilverlight;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using Ionic.Zip;
using System.Globalization;
using System.Text.RegularExpressions;
namespace ArcFMSilverlight.Web.Services
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WCFPONSService" in code, svc and config file together.
    public class WCFPONSService : IWCFPONSService
    {
        string strFilePath = System.Configuration.ConfigurationManager.AppSettings["StandardPrintFileUploadDC2"];
        string strPrintFilePath = System.Configuration.ConfigurationManager.AppSettings["StandardPrintFileSourceDC2"];
        string StandardPrintFilePDFURLNew = System.Configuration.ConfigurationManager.AppSettings["StandardPrintFilePDFURLNew"];
        string strConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        string strPSLConnetionString = System.Configuration.ConfigurationManager.ConnectionStrings["PSLConnectionString"].ConnectionString;
        string strjetConnetionString = System.Configuration.ConfigurationManager.ConnectionStrings["JetTnsConnectionString"].ConnectionString;
        /****************************ENOS2SAP PhaseIII Start****************************/
        public string sapEgiNotifications = String.Empty;
        string strSettingsConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SettingsConnectionString"].ConnectionString;
        /****************************ENOS2SAP PhaseIII End****************************/

        //Adding function to make values configurable[Start]
        string strCGCINCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CGC_INC_SPName"];
        string strCGCEXCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CGC_EXC_SPName"];
        string strSERVPINCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["SERVP_INC_SPName"];
        string strSERVPEXCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["SERVP_EXC_SPName"];
        string strCIRCTINCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CIRCUITP_INC_SPName"];
        string strCIRCTEXCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CIRCUITP_EXC_SPName"];
        string strSEQGenPROCName = System.Configuration.ConfigurationManager.AppSettings["SEQSP_SPName"];
        // string ZipFileToCreate = @"c://temp";
        string root, strFolderName;
        //Enos
        string strEnocConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FeederConnectionString"].ConnectionString;
        //CIT connection/app strings *start
        string CITConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["FeederConnectionString"].ConnectionString;
        //string PTW1DConnectionString_WEBR = System.Configuration.ConfigurationManager.ConnectionStrings["PTW1DConnectionString_WEBR"].ConnectionString;
        string manual_table = System.Configuration.ConfigurationManager.AppSettings["Manual_Table_Name"];
        //CIT connection/app strings *end
        string Jetpagesize = System.Configuration.ConfigurationManager.AppSettings["Jetpagesize"];

        string strValidateAndAddJobSPName = System.Configuration.ConfigurationManager.AppSettings["VALIDATE_ADD_JETJOB_SPName"];

        string strWipConnetionString = System.Configuration.ConfigurationManager.ConnectionStrings["JetTnsConnectionString"].ConnectionString;
        string strFavInsertUpdateSPName = System.Configuration.ConfigurationManager.AppSettings["FAVORITES_INSERTUPDATE_SP"];

        //ME Q3 2019 - DA# 190501
        string strLoadingInfoCustDataSPName = System.Configuration.ConfigurationManager.AppSettings["LOADINGINFO_CUSTDATA_SPName"];

         //ME Q3 2019 - DA# 190506
        string webrLoginData_TableName = System.Configuration.ConfigurationManager.AppSettings["WEBRLOGINDATA_Table"];
        
        //Jet Connection
        private string GetJetConnectionString()
        {
            return System.Configuration.ConfigurationManager.AppSettings["JetTnsConnectionString"];
        }

        public string GetExportdataFile(string outagedate, string Area, string message, List<TransformercustomerdataComb> customerData, string strFileFormat, string strUserName)
        {
            List<TransformercustomerdataComb> CustomerResult1 = new List<TransformercustomerdataComb>();
            TransformercustomerdataComb objcustomerlist = new TransformercustomerdataComb();
            List<Pdfcustomerdataresult> PDFCustomerResult = new List<Pdfcustomerdataresult>();
            List<PdfcustomerdataPREMISEresult> PDFCustomerPremiseResult = new List<PdfcustomerdataPREMISEresult>();
            List<ExportFileName> GeneratedExportFileName = new List<ExportFileName>();
            ExportFileName FileName = new ExportFileName();
            string _CustomerResult = string.Empty;
            DataTable dt = new DataTable();
            string strFilePath = "";
            try
            {
                if (strFileFormat.ToUpper() == "RPT".ToUpper())
                {
                    foreach (TransformercustomerdataComb data in customerData)
                    {
                        PDFCustomerPremiseResult.Add(new PdfcustomerdataPREMISEresult()
                        {
                            Type = data.CustType,
                            CustomerName = data.CustMail1,
                            Address = data.AverySTNameNumber,
                            City_State_Zip = data.AddressCust,
                            PREMISEID = data.PREMISEID,
                            PREMISETYPE = data.PREMISETYPE,

                            CGC_TNum = data.CGC12,
                            SSD = data.TransSSD,
                            Phone = data.CustAreaCode + data.CustPhone
                        });
                    }
                    var objcus = PDFCustomerPremiseResult.ToList();
                    dt = ConvertToDataTable(PDFCustomerPremiseResult);
                }
                else
                {
                    foreach (TransformercustomerdataComb data in customerData)
                    {
                        PDFCustomerResult.Add(new Pdfcustomerdataresult()
                        {
                            Type = data.CustType,
                            CustomerName = data.CustMail1,
                            Address = data.AverySTNameNumber,
                            City_State_Zip = data.AddressCust,
                            CGC_TNum = data.CGC12,
                            SSD = data.TransSSD,
                            Phone = data.CustAreaCode + data.CustPhone
                        });
                    }
                    var objcus = PDFCustomerResult.ToList();
                    dt = ConvertToDataTable(PDFCustomerResult);
                }

                string outagetime = outagedate;
                string outagearea = Area;
                string othermsg = message;
                Exporthelper exportcusdata = new Exporthelper();

                if (strFileFormat.ToUpper() == "pdf".ToUpper())
                {
                    FileName.ExportedFileName = "";
                    strFilePath = "";
                    strFilePath = exportcusdata.createSilverlightPDF(outagedate, outagearea, othermsg, dt, strUserName);
                    FileName.ExportedFileName = strFilePath;
                    GeneratedExportFileName.Add(FileName);
                    return GeneratedExportFileName[0].ExportedFileName.ToString();
                }
                else if (strFileFormat.ToUpper() == "xml".ToUpper())
                {
                    FileName.ExportedFileName =
                    strFilePath = "";
                    strFilePath = exportcusdata.createExcelFile(outagedate.Trim(), outagearea.Trim(), othermsg, dt, strUserName);
                    FileName.ExportedFileName = strFilePath;
                    GeneratedExportFileName.Add(FileName);
                    return GeneratedExportFileName[0].ExportedFileName.ToString();
                }
                else if (strFileFormat.ToUpper() == "RPT".ToUpper())
                {
                    FileName.ExportedFileName =
                    strFilePath = "";
                    strFilePath = exportcusdata.createPremiseExcelFile(outagedate.Trim(), outagearea.Trim(), othermsg, dt, strUserName);
                    FileName.ExportedFileName = strFilePath;
                    GeneratedExportFileName.Add(FileName);
                    return GeneratedExportFileName[0].ExportedFileName.ToString();
                }
                else if (strFileFormat.ToUpper() == "txt".ToUpper())
                {
                    FileName.ExportedFileName =
                    strFilePath = "";
                    strFilePath = exportcusdata.createTEXTFile(outagedate.Trim(), outagearea.Trim(), othermsg, dt, strUserName);
                    FileName.ExportedFileName = strFilePath;
                    GeneratedExportFileName.Add(FileName);
                    return GeneratedExportFileName[0].ExportedFileName.ToString();
                }
            }
            catch (Exception ex)
            {
                return GeneratedExportFileName[0].ExportedFileName.ToString();
            }
            return GeneratedExportFileName[0].ExportedFileName.ToString();
        }
        public string GetAverydata(List<TransformercustomerdataComb> customerData, int skips, string UserName)
        {
            List<TransformercustomerdataComb> CustomerResult1 = new List<TransformercustomerdataComb>();
            TransformercustomerdataComb objcustomerlist = new TransformercustomerdataComb();
            List<Averycustomerdataresult> AveryCustomerResult = new List<Averycustomerdataresult>();
            string _CustomerResult = string.Empty;
            DataTable dt = new DataTable();
            string sPath = "";
            try
            {
                foreach (TransformercustomerdataComb data in customerData)
                {
                    AveryCustomerResult.Add(new Averycustomerdataresult()
                    {
                        CustomerName = data.CustMail1,
                        Address = data.CustMailStName1,
                        City_State_Zip = data.AddressCust
                    });
                }
                // var objcus = AveryCustomerResult.ToList();
                dt = ConvertToDataTable(AveryCustomerResult);

                Avery5160 Averyexporter = new Avery5160();
                sPath = Averyexporter.CreateDocument(dt, skips, UserName);

            }
            catch (Exception ex)
            {
            }
            return sPath;

        }

        //Device select from ID
        //Adding Code for Circuit Search[Start]
        public List<Devicestartcustomerdata> GetDevicedataCustomerSearch(string strDevice1, string strDevice2, int Divisioncode)
        {
            string devicesql = string.Empty;
            List<Devicestartcustomerdata> CustomerResult = new List<Devicestartcustomerdata>();
            try
            {
                devicesql = "Select 'Switch' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.Switch where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + Divisioncode + "' UNION Select 'Fuse' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Fuse where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + Divisioncode + "' UNION Select  'OpenPoint' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.OpenPoint where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + Divisioncode + "' UNION Select  'OpenPoint' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Transformer  where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + Divisioncode + "' Union Select 'PriUGConductor' as DeviceName, TO_CHAR(objectid)  as OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.PriUGConductor  where objectid IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + Divisioncode + "' union Select 'PrimaryOverheadConductor' as DeviceName, TO_CHAR(objectid)  as OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.PriOHConductor where objectid IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + Divisioncode + "'";
                OracleConnection sqlConnection = new OracleConnection(strConnectionString);
                OracleCommand sqlCommand = new OracleCommand(devicesql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(objSet);
                sqlConnection.Close();


                Devicestartcustomerdata objcustomerlist;

                if (objSet.Tables.Count > 0)
                {
                    foreach (DataRow dr in objSet.Tables[0].Rows)
                    {
                        objcustomerlist = new Devicestartcustomerdata();
                        objcustomerlist.DeviceName = dr[0].ToString();
                        objcustomerlist.OPERATINGNUMBER = dr[1].ToString();
                        objcustomerlist.objectid = dr[2].ToString();
                        objcustomerlist.globalid = dr[3].ToString();
                        objcustomerlist.circuitid = dr[4].ToString();
                        CustomerResult.Add(objcustomerlist);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return CustomerResult;
        }
        # region         List convert to datatable
        public static DataTable ConvertToDataTable<T>(IList<T> list) where T : class
        {
            try
            {
                DataTable table = CreateDataTable<T>();
                Type objType = typeof(T);
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
                foreach (T item in list)
                {
                    DataRow row = table.NewRow();
                    foreach (PropertyDescriptor property in properties)
                    {
                        if (!CanUseType(property.PropertyType)) continue;
                        row[property.Name.ToString().Replace('_', ' ')] = property.GetValue(item) ?? DBNull.Value;
                    }

                    table.Rows.Add(row);
                }
                return table;
            }
            catch (DataException ex)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        private static DataTable CreateDataTable<T>() where T : class
        {
            Type objType = typeof(T);
            DataTable table = new DataTable(objType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(objType);
            foreach (PropertyDescriptor property in properties)
            {
                Type propertyType = property.PropertyType;
                if (!CanUseType(propertyType)) continue;

                //nullables must use underlying types
                if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                //enums also need special treatment
                if (propertyType.IsEnum)
                    propertyType = Enum.GetUnderlyingType(propertyType);
                table.Columns.Add(property.Name.ToString().Replace('_', ' '), propertyType);
            }
            return table;
        }
        private static bool CanUseType(Type propertyType)
        {
            //only strings and value types
            if (propertyType.IsArray) return false;
            if (!propertyType.IsValueType && propertyType != typeof(string)) return false;
            return true;
        }

        # endregion

        //Add Function for get Customer data[Start]
        public DataSet getCustomer_CircuitID(string CircuitIds, string strDivisionCode, string strFlag)
        {
            DataSet oraDataSet = new DataSet();
            OracleDataAdapter da = new OracleDataAdapter();
            OracleConnection conn = null;
            int DivisionCode = 0;
            if (strDivisionCode != "")
            {
                DivisionCode = Convert.ToInt32(strDivisionCode);
            }
            try
            {
                conn = new OracleConnection(strConnectionString);
                OracleCommand comm = new OracleCommand();
                //Adding definition for parameter one
                OracleParameter output1 = new OracleParameter();
                output1.ParameterName = "P_INPUTSTRING";
                output1.OracleType = OracleType.VarChar;
                output1.Direction = ParameterDirection.Input;
                output1.Value = CircuitIds;
                output1.Size = 2000;
                //Adding definition for parameter two
                OracleParameter output2 = new OracleParameter();
                output2.ParameterName = "P_VERSION";
                output2.OracleType = OracleType.VarChar;
                output2.Direction = ParameterDirection.Input;
                output2.Value = "";
                output2.Size = 2000;
                //Adding definition for parameter Six
                OracleParameter output3 = new OracleParameter();
                output3.ParameterName = "P_DIVISION";
                output3.OracleType = OracleType.Number;
                output3.Direction = ParameterDirection.Input;
                output3.Value = DivisionCode;
                output3.Size = 2000;
                //Adding definition for parameter three
                OracleParameter output5 = new OracleParameter();
                output5.ParameterName = "P_CURSOR";
                output5.OracleType = OracleType.Cursor;
                output5.Direction = ParameterDirection.Output;

                OracleParameter output6 = new OracleParameter();
                output6.ParameterName = "P_ERROR";
                output6.OracleType = OracleType.VarChar;
                output6.Direction = ParameterDirection.Output;
                output6.Size = 2000;
                OracleParameter output7 = new OracleParameter();
                output7.ParameterName = "P_SUCCESS";
                output7.OracleType = OracleType.Number;
                output7.Direction = ParameterDirection.Output;
                output7.Size = 2000;
                comm.Parameters.Add(output1);
                comm.Parameters.Add(output2);
                //comm.Parameters.Add(output3);
                comm.Parameters.Add(output5);
                comm.Parameters.Add(output6);
                comm.Parameters.Add(output7);
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = strCIRCTINCPROCSPName;
                //if (strFlag == "0")
                //{
                //    comm.CommandText = strCIRCTINCPROCSPName;//"PONS.CUST_INFO_CIRCUITID_INCL";
                //}
                //else
                //{
                //    comm.CommandText = strCIRCTEXCPROCSPName;//"PONS.CUST_INFO_CIRCUITID_EXCL";
                //}
                comm.CommandTimeout = 600;
                conn.Open();
                da = new OracleDataAdapter(comm);
                da.Fill(oraDataSet);
                int i = oraDataSet.Tables[0].Rows.Count;
            }
            catch (TimeoutException exception)
            {
                Console.WriteLine("Got {0}", exception.GetType());
                conn.Close();
                oraDataSet = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                conn.Close();
                oraDataSet = null;
            }
            finally
            {
                conn.Close();
            }

            return oraDataSet;
        }
        public DataSet getCustomer_CGCNumber(string cgcNumber, string strFlag, string strDivisionCode)
        {
            DataSet oraCGCDataSet = new DataSet();
            OracleCommand comm = new OracleCommand();
            OracleDataAdapter da = null;
            OracleConnection conn = null;
            int DivisionCode = 0;
            if (strDivisionCode != "")
            {
                DivisionCode = Convert.ToInt32(strDivisionCode);
            }
            try
            {
                conn = new OracleConnection(strConnectionString);
                //Adding definition for parameter one
                OracleParameter output2 = new OracleParameter();
                output2.ParameterName = "P_VERSION";
                output2.OracleType = OracleType.VarChar;
                output2.Direction = ParameterDirection.Input;
                output2.Value = "";

                OracleParameter OradivNum = new OracleParameter();
                OradivNum.ParameterName = "P_DIVISION";
                OradivNum.OracleType = OracleType.Number;
                OradivNum.Direction = ParameterDirection.Input;
                OradivNum.Value = DivisionCode;

                //Adding definition for parameter two
                OracleParameter output1 = new OracleParameter();
                output1.ParameterName = "P_INPUTSTRING";
                output1.OracleType = OracleType.VarChar;
                output1.Direction = ParameterDirection.Input;
                output1.Value = cgcNumber;

                //Adding definition for parameter four
                OracleParameter output4 = new OracleParameter();
                output4.ParameterName = "P_CURSOR";
                output4.OracleType = OracleType.Cursor;
                output4.Direction = ParameterDirection.Output;

                //Adding definition for parameter five
                OracleParameter output5 = new OracleParameter();
                output5.ParameterName = "P_ERROR";
                output5.OracleType = OracleType.VarChar;
                output5.Direction = ParameterDirection.Output;
                output5.Size = 40000;

                //Adding definition for parameter six
                OracleParameter output6 = new OracleParameter();
                output6.ParameterName = "P_SUCCESS";
                output6.OracleType = OracleType.Number;
                output6.Direction = ParameterDirection.Output;
                output6.Size = 40000;

                comm.Parameters.Add(output1);
                comm.Parameters.Add(output2);
                //comm.Parameters.Add(OradivNum);
                comm.Parameters.Add(output4);
                comm.Parameters.Add(output5);
                comm.Parameters.Add(output6);
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = strCGCINCPROCSPName;
                //if (strFlag == "0")
                //{
                //    comm.CommandText = strCGCINCPROCSPName;//"PONS.CUST_INFO_CGC_INCL_T";
                //}
                //else
                //{
                //    comm.CommandText = strCGCEXCPROCSPName;//"PONS.CUST_INFO_CGC_EXCL_T";
                //}
                comm.CommandTimeout = 600;
                conn.Open();
                da = new OracleDataAdapter(comm);
                da.Fill(oraCGCDataSet);
                int i = oraCGCDataSet.Tables[0].Rows.Count;
            }
            catch (Exception ex)
            {
                oraCGCDataSet = null;
                conn.Close();
                comm.Dispose();
            }
            finally
            {
                conn.Close();
                comm.Dispose();
            }
            return oraCGCDataSet;
        }
        public DataSet getCustomer_ServicePoint(string servicePoint, string strFlag)
        {
            DataSet oraCustomerDataSet = null;
            OracleCommand comm = new OracleCommand();
            OracleDataAdapter da = null;
            OracleConnection conn = null;
            try
            {
                conn = new OracleConnection(strConnectionString);


                //Adding definition for parameter one
                OracleParameter output1 = new OracleParameter();
                output1.ParameterName = "P_VERSION";
                output1.OracleType = OracleType.VarChar;
                output1.Direction = ParameterDirection.Input;
                output1.Value = "";

                //Adding definition for parameter two
                OracleParameter output2 = new OracleParameter();
                output2.ParameterName = "P_INPUTSTRING";
                output2.OracleType = OracleType.VarChar;
                output2.Direction = ParameterDirection.Input;
                output2.Value = servicePoint;

                //Adding definition for parameter four
                OracleParameter output4 = new OracleParameter();
                output4.ParameterName = "P_CURSOR";
                output4.OracleType = OracleType.Cursor;
                output4.Direction = ParameterDirection.Output;

                //Adding definition for parameter five
                OracleParameter output5 = new OracleParameter();
                output5.ParameterName = "P_ERROR";
                output5.OracleType = OracleType.VarChar;
                output5.Direction = ParameterDirection.Output;
                output5.Size = 40000;

                //Adding definition for parameter six
                OracleParameter output6 = new OracleParameter();
                output6.ParameterName = "P_SUCCESS";
                output6.OracleType = OracleType.Number;
                output6.Direction = ParameterDirection.Output;
                output6.Size = 40000;

                comm.Parameters.Add(output1);
                comm.Parameters.Add(output2);
                comm.Parameters.Add(output4);
                comm.Parameters.Add(output5);
                comm.Parameters.Add(output6);
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                if (strFlag == "0")
                {
                    comm.CommandText = strSERVPINCPROCSPName;//"PONS.CUST_INFO_SERVICEPOINTS_INCL";
                }
                else
                {
                    comm.CommandText = strSERVPEXCPROCSPName;//"PONS.CUST_INFO_SERVICEPOINTS_EXCL";
                }
                comm.CommandTimeout = 600;
                oraCustomerDataSet = new DataSet();
                conn.Open();
                da = new OracleDataAdapter(comm);
                da.Fill(oraCustomerDataSet);
            }
            catch (Exception ex)
            {
                conn.Close();
                comm.Dispose();
                oraCustomerDataSet = null;
            }
            finally
            {
                conn.Close();
                comm.Dispose();

            }
            return oraCustomerDataSet;
        }
        public List<TransformercustomerdataComb> GetCustomerdataComb(string strCGSValue, string strCircuitId, string strServicePointID, string strDivisionIn, string strFlagIn)
        {
            string resultJson = "";
            string CGS = "";
            string CircuitId = "";
            string ServicePointID = "";
            CGS = strCGSValue;
            CircuitId = strCircuitId;
            ServicePointID = strServicePointID;
            List<TransformercustomerdataComb> CustomerResult = new List<TransformercustomerdataComb>();
            DataSet oraDataSetCGC = new DataSet();
            DataSet oraDataSetCGC1 = new DataSet();
            DataSet oraDataSetCGC2 = new DataSet();
            DataSet oraDataSetCicuitID = new DataSet();
            DataSet dtResults = new DataSet();

            string strCGC = "";
            string strSSD = "";
            string[] strCGCSSD = CGS.Split(',');
            try
            {
                if (CGS != "")
                {
                    //for loop to get SSD and CGC
                    for (int i = 0; i < strCGCSSD.Length; i++)
                    {
                        if (strCGCSSD[i].Contains('^') == true)
                        {
                            string[] strArrCGCSSD = strCGCSSD[i].Split('^');
                            strCGC = strCGC + strArrCGCSSD[0].ToString() + ",";
                        }
                    }
                    strCGC = strCGC.TrimEnd(strCGC[strCGC.Length - 1]);

                }
                if (strServicePointID != "")
                {
                    if (strCGC != "")
                        strCGC = strCGC + "," + strServicePointID;
                    else
                        strCGC = strServicePointID;
                }

                if (strCGC != "")
                {
                    //Add code for sp number more than 999
                    char[] delimiter1 = new char[] { ',' };


                    string[] Totalcgcno = strCGC.Split(delimiter1, StringSplitOptions.None);
                    int spcount = Totalcgcno.Count();



                    if (spcount > 995)
                    {
                        string[] firstgcsds = Totalcgcno.Take(995).Select(i => i.ToString()).ToArray();
                        string[] lastspvals = Totalcgcno.Skip(995).Select(i => i.ToString()).ToArray();

                        string Spval = cgcsplit(firstgcsds);
                        oraDataSetCGC = getCustomer_CGCNumber(Spval, strFlagIn, strDivisionIn);

                        string lastspval = cgcsplit(lastspvals);
                        oraDataSetCGC2 = getCustomer_CGCNumber(lastspval, strFlagIn, strDivisionIn);
                        oraDataSetCGC.Merge(oraDataSetCGC2);
                    }

                    else
                    {
                        oraDataSetCGC = getCustomer_CGCNumber(strCGC, strFlagIn, strDivisionIn);
                    }
                }


                if (CircuitId != "")
                {
                    oraDataSetCicuitID = getCustomer_CircuitID(CircuitId, strDivisionIn, strFlagIn);
                }
                //if (ServicePointID != "")
                //{
                //    dtResults = getCustomer_ServicePoint(ServicePointID, strFlagIn);
                //}

                //if (dtResults != null && dtResults.Tables.Count > 0)
                //{
                //    foreach (DataRow dr in dtResults.Tables[0].Rows)
                //    {
                //        TransformercustomerdataComb objcustomerlistCGC = null;
                //        objcustomerlistCGC = getCustomerRow(dr);

                //        if (objcustomerlistCGC != null)
                //            CustomerResult.Add(objcustomerlistCGC, dtResults.Tables[0]);
                //    }
                //}
                if (oraDataSetCicuitID != null && oraDataSetCicuitID.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetCicuitID.Tables[0].Rows)
                    {

                        TransformercustomerdataComb objcustomerlistCGC = null;
                        objcustomerlistCGC = getCustomerRow(dr, oraDataSetCicuitID.Tables[0]);

                        if (objcustomerlistCGC != null)
                            CustomerResult.Add(objcustomerlistCGC);

                    }
                }
                if (oraDataSetCGC != null && oraDataSetCGC.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetCGC.Tables[0].Rows)
                    {
                        TransformercustomerdataComb objcustomerlistCGC = null;
                        objcustomerlistCGC = getCustomerRow(dr, oraDataSetCGC.Tables[0]);

                        if (objcustomerlistCGC != null)
                            CustomerResult.Add(objcustomerlistCGC);
                    }

                }
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                throw;
            }
            finally
            {

            }
            return CustomerResult;
        }
        //string split
        private string cgcsplit(string[] sp)
        {
            string val1 = string.Empty;

            for (int i = 0; i < sp.Length; i++)
            {
                val1 = val1 + sp[i].ToString() + ",";
            }
            string finalval = val1.Remove(val1.Length - 1, 1);

            return finalval;
        }
        private TransformercustomerdataComb getCustomerRow(DataRow dr, DataTable pTable)
        {
            try
            {
                TransformercustomerdataComb objcustomerlistCGC = new TransformercustomerdataComb();

                string strMailZipCode = "";

                //if (Convert.ToInt16(dr["ORD"]) == 2)
                //{

                //    DataRow[] results = null;


                //    results = pTable.Select("SERVICEPOINTID = '" + dr["SERVICEPOINTID"] + "' AND ORD = 1");
                //    if (results != null && results.Count() > 0)
                //    {
                //        dr["MAILNAME1"] = "PG&E Customer at";
                //        dr["MAILNAME2"] = "";
                //    }



                //}

                strMailZipCode = Convert.ToString(dr["MAILZIPCODE"]);
                if (strMailZipCode != "" && strMailZipCode.Length == 9)
                {
                    strMailZipCode = strMailZipCode.Substring(0, 5).ToString() + "-" + strMailZipCode.Substring(5, 4).ToString();
                }
                else
                {
                    strMailZipCode = Convert.ToString(dr["MAILZIPCODE"]);
                }

                objcustomerlistCGC.TransSSD = Convert.ToString(dr["SOURCESIDEDEVICEID"]);
                objcustomerlistCGC.CustType = Convert.ToString(dr["CUSTOMERTYPE"]);
                objcustomerlistCGC.CustMail1 = Convert.ToString(dr["MAILNAME1"]);
                objcustomerlistCGC.CustMail2 = Convert.ToString(dr["MAILNAME2"]);
                objcustomerlistCGC.CustMailStNum = Convert.ToString(dr["MAILSTREETNUM"]);
                objcustomerlistCGC.CustMailStName1 = Convert.ToString(dr["MAILSTREETNAME1"]);
                objcustomerlistCGC.CustMailStName2 = Convert.ToString(dr["MAILSTREETNAME2"]);
                objcustomerlistCGC.SerStNumber = Convert.ToString(dr["MAILSTREETNUM"]);
                objcustomerlistCGC.SerStName1 = Convert.ToString(dr["MAILSTREETNAME1"]);
                objcustomerlistCGC.SerStName2 = Convert.ToString(dr["MAILSTREETNAME2"]);
                objcustomerlistCGC.ORD = Convert.ToInt16(dr["ORD"]);
                objcustomerlistCGC.PREMISEID = Convert.ToString(dr["PREMISEID"]);
                objcustomerlistCGC.PREMISETYPE = Convert.ToString(dr["PREMISETYPE"]);
                string Address = "";

                if (objcustomerlistCGC.CustMail1 == "" && objcustomerlistCGC.CustMail2 == "")
                {
                    objcustomerlistCGC.CustMail1 = "PG&E Customer at";
                }

                if (Convert.ToString(dr["MAILSTREETNUM"]) != "")
                    Address = Address + Convert.ToString(dr["MAILSTREETNUM"]);

                if (Convert.ToString(dr["MAILSTREETNAME1"]) != "")
                {
                    if (Address != "")
                        Address = Address + " " + Convert.ToString(dr["MAILSTREETNAME1"]);
                    else
                        Address = Convert.ToString(dr["MAILSTREETNAME1"]);
                }

                if (Convert.ToString(dr["MAILSTREETNAME2"]) != "")
                {
                    if (Address != "")
                        Address = Address + " " + Convert.ToString(dr["MAILSTREETNAME2"]);
                    else
                        Address = Convert.ToString(dr["MAILSTREETNAME2"]);
                }

                objcustomerlistCGC.AverySTNameNumber = Address;

                string strCustomername = "";
                string strGridAddress = "";

                if (objcustomerlistCGC.CustMail1 != "")
                    strCustomername = objcustomerlistCGC.CustMail1;

                if (Convert.ToString(dr["MAILNAME2"]) != "")
                    if (strCustomername != "")
                        strCustomername = strCustomername + " " + Convert.ToString(dr["MAILNAME2"]);
                    else
                        strCustomername = Convert.ToString(dr["MAILNAME2"]);

                if (Convert.ToString(dr["MAILSTREETNUM"]) != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + Convert.ToString(dr["MAILSTREETNUM"]);
                    else
                        strGridAddress = Convert.ToString(dr["MAILSTREETNUM"]);

                if (Convert.ToString(dr["MAILSTREETNAME1"]) != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + Convert.ToString(dr["MAILSTREETNAME1"]);
                    else
                        strGridAddress = Convert.ToString(dr["MAILSTREETNAME1"]);

                if (Convert.ToString(dr["MAILSTREETNAME2"]) != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + Convert.ToString(dr["MAILSTREETNAME2"]);
                    else
                        strGridAddress = Convert.ToString(dr["MAILSTREETNAME2"]);

                string strCitySateZip = "";

                if (Convert.ToString(dr["MAILCITY"]) != "")
                    strCitySateZip = Convert.ToString(dr["MAILCITY"]);

                if (Convert.ToString(dr["MAILSTATE"]) != "")
                    if (strCitySateZip != "")
                        strCitySateZip = strCitySateZip + ", " + Convert.ToString(dr["MAILSTATE"]);
                    else
                        strCitySateZip = Convert.ToString(dr["MAILSTATE"]);

                if (strMailZipCode != "")
                    if (strCitySateZip != "")
                        strCitySateZip = strCitySateZip + ", " + strMailZipCode;
                    else
                        strCitySateZip = strMailZipCode;

                objcustomerlistCGC.AddressCust = strCitySateZip;

                if (strCitySateZip != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + strCitySateZip;
                    else
                        strGridAddress = strCitySateZip;

                objcustomerlistCGC.CustomerName = strCustomername;
                objcustomerlistCGC.GridADDCol = strGridAddress; //Convert.ToString(dr["MAILNAME1"]) + " " + Convert.ToString(dr["MAILNAME2"])+ " " +Convert.ToString( dr["MAILSTREETNUM"]) + " " +Convert.ToString( dr["MAILSTREETNAME1"])+ " " + Convert.ToString(dr["MAILSTREETNAME2"]) + " " + strMailZipCode;



                objcustomerlistCGC.CustPhone = Convert.ToString(dr["PHONENUM"]);
                objcustomerlistCGC.CGC12 = Convert.ToString(dr["CGC12"]);
                objcustomerlistCGC.SerActID = Convert.ToString(dr["ACCOUNTID"]);
                objcustomerlistCGC.SerDivision = Convert.ToString(dr["DIVISION"]);
                objcustomerlistCGC.ServicepointId = Convert.ToString(dr["SERVICEPOINTID"]);
                objcustomerlistCGC.MeterNum = Convert.ToString(dr["METERNUMBER"]);
                objcustomerlistCGC.CustMailCity = Convert.ToString(dr["MAILCITY"]);
                objcustomerlistCGC.SerCity = Convert.ToString(dr["MAILCITY"]);
                objcustomerlistCGC.SerState = Convert.ToString(dr["MAILSTATE"]);
                objcustomerlistCGC.CustMailZipCode = strMailZipCode;
                objcustomerlistCGC.ServiceZip = strMailZipCode;

                string strmailState = "";

                if (Convert.ToString(dr["MAILSTREETNUM"]) != "")
                    strmailState = Convert.ToString(dr["MAILSTREETNUM"]);

                if (Convert.ToString(dr["MAILSTREETNAME1"]) != "")
                    if (strCitySateZip != "")
                        strmailState = strmailState + " " + Convert.ToString(dr["MAILSTREETNAME1"]);
                    else
                        strmailState = Convert.ToString(dr["MAILSTREETNAME1"]);

                if (Convert.ToString(dr["MAILSTREETNAME2"]) != "")
                    if (strCitySateZip != "")
                        strmailState = strmailState + " " + Convert.ToString(dr["MAILSTREETNAME2"]);
                    else
                        strmailState = Convert.ToString(dr["MAILSTREETNAME2"]);

                if (strMailZipCode != "")
                    if (strmailState != "")
                        strmailState = strmailState + " " + strMailZipCode;
                    else
                        strmailState = strMailZipCode;

                objcustomerlistCGC.MailState = strmailState; //Convert.ToString(dr["MAILSTREETNUM"]) + " " + Convert.ToString(dr["MAILSTREETNAME1"]) + " " + Convert.ToString(dr["MAILSTREETNAME2"]) + " " + strMailZipCode;

                return objcustomerlistCGC;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }

        private TransformercustomerdataComb getCustomerRowCircuit(DataRow dr)
        {
            try
            {
                TransformercustomerdataComb objcustomerlistCGC = new TransformercustomerdataComb();
                string strMailZipCode = "";

                strMailZipCode = Convert.ToString(dr["ZIP"]);
                if (strMailZipCode != "" && strMailZipCode.Length == 9)
                {
                    strMailZipCode = strMailZipCode.Substring(0, 5).ToString() + "-" + strMailZipCode.Substring(5, 4).ToString();
                }
                else
                {
                    strMailZipCode = Convert.ToString(dr["ZIP"]);
                }

                objcustomerlistCGC.TransSSD = Convert.ToString(dr["SOURCESIDEDEVICEID"]);
                objcustomerlistCGC.CustType = Convert.ToString(dr["CUSTOMERTYPE"]);
                objcustomerlistCGC.CustMail1 = Convert.ToString(dr["MAILNAME1"]);
                objcustomerlistCGC.CustMail2 = Convert.ToString(dr["MAILNAME2"]);
                objcustomerlistCGC.CustMailStNum = Convert.ToString(dr["STREETNUMBER"]);
                objcustomerlistCGC.CustMailStName1 = Convert.ToString(dr["STREETNAME1"]);
                objcustomerlistCGC.CustMailStName2 = Convert.ToString(dr["STREETNAME2"]);
                objcustomerlistCGC.SerStNumber = Convert.ToString(dr["STREETNUMBER"]);
                objcustomerlistCGC.SerStName1 = Convert.ToString(dr["STREETNAME1"]);
                objcustomerlistCGC.ORD = Convert.ToInt16(dr["ORD"]);

                string Address = "";

                if (Convert.ToString(dr["STREETNUMBER"]) != "")
                    Address = Address + Convert.ToString(dr["STREETNUMBER"]);

                if (Convert.ToString(dr["STREETNAME1"]) != "")
                {
                    if (Address != "")
                        Address = Address + " " + Convert.ToString(dr["STREETNAME1"]);
                    else
                        Address = Convert.ToString(dr["STREETNAME1"]);
                }

                if (Convert.ToString(dr["STREETNAME2"]) != "")
                {
                    if (Address != "")
                        Address = Address + " " + Convert.ToString(dr["STREETNAME2"]);
                    else
                        Address = Convert.ToString(dr["STREETNAME2"]);
                }

                objcustomerlistCGC.AverySTNameNumber = Address;

                string strGridAddress = "";

                if (Convert.ToString(dr["MAILNAME1"]) != "")
                    strGridAddress = Convert.ToString(dr["MAILNAME1"]);

                if (Convert.ToString(dr["MAILNAME2"]) != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + Convert.ToString(dr["MAILNAME2"]);
                    else
                        strGridAddress = Convert.ToString(dr["MAILNAME2"]);

                if (Convert.ToString(dr["STREETNUMBER"]) != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + Convert.ToString(dr["STREETNUMBER"]);
                    else
                        strGridAddress = Convert.ToString(dr["STREETNUMBER"]);

                if (Convert.ToString(dr["STREETNAME1"]) != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + Convert.ToString(dr["STREETNAME1"]);
                    else
                        strGridAddress = Convert.ToString(dr["STREETNAME1"]);

                if (Convert.ToString(dr["STREETNAME2"]) != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + Convert.ToString(dr["STREETNAME2"]);
                    else
                        strGridAddress = Convert.ToString(dr["STREETNAME2"]);

                if (strMailZipCode != "")
                    if (strGridAddress != "")
                        strGridAddress = strGridAddress + " " + strMailZipCode;
                    else
                        strGridAddress = strMailZipCode;


                objcustomerlistCGC.GridADDCol = strGridAddress; //Convert.ToString(dr["MAILNAME1"]) + " " + Convert.ToString(dr["MAILNAME2"])+ " " +Convert.ToString( dr["MAILSTREETNUM"]) + " " +Convert.ToString( dr["MAILSTREETNAME1"])+ " " + Convert.ToString(dr["MAILSTREETNAME2"]) + " " + strMailZipCode;

                string strCitySateZip = "";

                if (Convert.ToString(dr["CITY"]) != "")
                    strCitySateZip = Convert.ToString(dr["CITY"]);

                if (Convert.ToString(dr["STATE"]) != "")
                    if (strCitySateZip != "")
                        strCitySateZip = strCitySateZip + ", " + Convert.ToString(dr["STATE"]);
                    else
                        strCitySateZip = Convert.ToString(dr["STATE"]);

                if (strMailZipCode != "")
                    if (strCitySateZip != "")
                        strCitySateZip = strCitySateZip + ", " + strMailZipCode;
                    else
                        strCitySateZip = strMailZipCode;

                objcustomerlistCGC.AddressCust = strCitySateZip;

                objcustomerlistCGC.CustPhone = Convert.ToString(dr["PHONENUM"]);
                objcustomerlistCGC.CGC12 = Convert.ToString(dr["CGC12"]);
                objcustomerlistCGC.SerActID = Convert.ToString(dr["ACCOUNTID"]);
                objcustomerlistCGC.SerDivision = Convert.ToString(dr["DIVISION"]);
                objcustomerlistCGC.ServicepointId = Convert.ToString(dr["SERVICEPOINTID"]);
                objcustomerlistCGC.MeterNum = Convert.ToString(dr["METERNUMBER"]);
                objcustomerlistCGC.CustMailCity = Convert.ToString(dr["CITY"]);
                objcustomerlistCGC.SerCity = Convert.ToString(dr["CITY"]);
                objcustomerlistCGC.SerState = Convert.ToString(dr["STATE"]);
                objcustomerlistCGC.CustMailZipCode = strMailZipCode;
                objcustomerlistCGC.ServiceZip = strMailZipCode;

                string strmailState = "";

                if (Convert.ToString(dr["STREETNUMBER"]) != "")
                    strmailState = Convert.ToString(dr["STREETNUMBER"]);

                if (Convert.ToString(dr["STREETNAME1"]) != "")
                    if (strCitySateZip != "")
                        strmailState = strmailState + " " + Convert.ToString(dr["STREETNAME1"]);
                    else
                        strmailState = Convert.ToString(dr["STREETNAME1"]);

                if (Convert.ToString(dr["STREETNAME2"]) != "")
                    if (strCitySateZip != "")
                        strmailState = strmailState + " " + Convert.ToString(dr["STREETNAME2"]);
                    else
                        strmailState = Convert.ToString(dr["STREETNAME2"]);

                if (strMailZipCode != "")
                    if (strmailState != "")
                        strmailState = strmailState + " " + strMailZipCode;
                    else
                        strmailState = strMailZipCode;

                objcustomerlistCGC.MailState = strmailState; //Convert.ToString(dr["MAILSTREETNUM"]) + " " + Convert.ToString(dr["MAILSTREETNAME1"]) + " " + Convert.ToString(dr["MAILSTREETNAME2"]) + " " + strMailZipCode;

                return objcustomerlistCGC;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }
        //Add Function for get Customer data[End]
        public string SendtoPSL(List<StagingdataCust> pReport, int plannedOutageID, string contactID, string division, string strWork1DTStart, string strWork1TMStart, string strWork1TMEnd, string strWork2DTStart, string strWork2TMStart, string strWork2TMEnd, string vicinity, string strUserName)
        {

            string strMessage = "";
            string stagingProc = "";
            bool success = false;
            DateTime Work1DTStart = new DateTime();
            DateTime Work1TMStart = new DateTime();
            DateTime Work1TMEnd = new DateTime();
            DateTime Work2DTStart = new DateTime();
            DateTime Work2TMStart = new DateTime();
            DateTime Work2TMEnd = new DateTime();
            Work1DTStart = Convert.ToDateTime(strWork1DTStart);
            Work1TMStart = Convert.ToDateTime(strWork1TMStart);
            Work1TMEnd = Convert.ToDateTime(strWork1TMEnd);
            Work2DTStart = Convert.ToDateTime(strWork2DTStart);
            Work2TMStart = Convert.ToDateTime(strWork2TMStart);
            Work2TMEnd = Convert.ToDateTime(strWork2TMEnd);
            try
            {
                SqlConnection sqlConnection = new SqlConnection(strPSLConnetionString);
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(strPSLConnetionString, SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls))
                {
                    bulkCopy.DestinationTableName = System.Configuration.ConfigurationManager.AppSettings["PSLSTAGINGTAB"].ToString();
                    stagingProc = System.Configuration.ConfigurationManager.AppSettings["PSLSTAGINGPROC"].ToString();
                    try
                    {
                        sqlConnection.Open();
                        bulkCopy.ColumnMappings.Clear();
                        // Write from the source to the destination.
                        bulkCopy.ColumnMappings.Add("SubmitUser", "SubmitUser");
                        bulkCopy.ColumnMappings.Add("SubmitDT", "SubmitDT");
                        bulkCopy.ColumnMappings.Add("SubmitTM", "SubmitTM");
                        bulkCopy.ColumnMappings.Add("Vicinity", "Vicinity");
                        bulkCopy.ColumnMappings.Add("AddrLine1", "AddrLine1");
                        bulkCopy.ColumnMappings.Add("AddrLine2", "AddrLine2");
                        bulkCopy.ColumnMappings.Add("AddrLine3", "AddrLine3");
                        bulkCopy.ColumnMappings.Add("AddrLine4", "AddrLine4");
                        bulkCopy.ColumnMappings.Add("AddrLine5", "AddrLine5");
                        bulkCopy.ColumnMappings.Add("CGCTNum", "CGCTNum");
                        bulkCopy.ColumnMappings.Add("SSD", "SSD");
                        bulkCopy.ColumnMappings.Add("Phone", "Phone");
                        bulkCopy.ColumnMappings.Add("Work1DT", "Work1DT");
                        bulkCopy.ColumnMappings.Add("Work1TMStart", "Work1TMStart");
                        bulkCopy.ColumnMappings.Add("Work1TMEnd", "Work1TMEnd");
                        bulkCopy.ColumnMappings.Add("Work2DT", "Work2DT");
                        bulkCopy.ColumnMappings.Add("Work2TMStart", "Work2TMStart");
                        bulkCopy.ColumnMappings.Add("Work2TMEnd", "Work2TMEnd");
                        bulkCopy.ColumnMappings.Add("AccountID", "AccountID");
                        bulkCopy.ColumnMappings.Add("OutageID", "OutageID");
                        bulkCopy.ColumnMappings.Add("ContactID", "ContactID");
                        bulkCopy.ColumnMappings.Add("SPID", "SPID");
                        bulkCopy.ColumnMappings.Add("SPAddrLine1", "SPAddrLine1");
                        bulkCopy.ColumnMappings.Add("SPAddrLine2", "SPAddrLine2");
                        bulkCopy.ColumnMappings.Add("SPAddrLine3", "SPAddrLine3");
                        bulkCopy.ColumnMappings.Add("CustType", "CustType");
                        bulkCopy.ColumnMappings.Add("meter_number", "meter_number");
                        bulkCopy.ColumnMappings.Add("division", "division");
                        bulkCopy.ColumnMappings.Add("cn_fld1", "cn_fld1");
                        bulkCopy.ColumnMappings.Add("cn_fld2", "cn_fld2");
                        bulkCopy.ColumnMappings.Add("cn_fld3", "cn_fld3");
                        DataTable dt = CustomerListtoDataTable(pReport, plannedOutageID, contactID, division, Work1DTStart, Work1TMStart, Work1TMEnd, Work2DTStart, Work2TMStart, Work2TMEnd, vicinity, strUserName);
                        if (dt != null)
                        {
                            bulkCopy.WriteToServer(dt);
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }
                    }
                    catch (SqlException sqlex)
                    {
                        success = false;
                        Console.WriteLine(sqlex.Message);
                        bulkCopy.Close();
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        Console.WriteLine(ex.Message);
                        bulkCopy.Close();
                        sqlConnection.Close();

                    }
                    finally
                    {
                        try
                        {
                            if (bulkCopy != null && success == true)
                            {
                                bulkCopy.Close();
                                using (SqlCommand sqlCommand2 = new SqlCommand(stagingProc, sqlConnection))
                                {
                                    sqlCommand2.CommandType = CommandType.StoredProcedure;
                                    sqlCommand2.Parameters.Add("@iModeFlag", SqlDbType.TinyInt).Value = 1;
                                    sqlCommand2.Parameters.Add("@iOutageID", SqlDbType.Int).Value = plannedOutageID;
                                    sqlCommand2.CommandTimeout = 600;
                                    sqlCommand2.ExecuteNonQuery();
                                    strMessage = "Succcess";
                                }
                            }
                            sqlConnection.Close();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            strMessage = "Fail";
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                strMessage = "Fail";
            }
            return strMessage;
        }

        public string SendtoPSL_old(List<StagingdataCust> pReport, int PlannedOutageID, string ContactID, string Division, DateTime Work1DTStart, DateTime Work1TMStart, DateTime Work1TMEnd, DateTime Work2DTStart, DateTime Work2TMStart, DateTime Work2TMEnd, string vicinity, string strUserName)
        {

            string strMessage = "";
            string stagingProc = "";
            bool success = false;
            try
            {
                SqlConnection sqlConnection = new SqlConnection(strPSLConnetionString);
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(strPSLConnetionString, SqlBulkCopyOptions.KeepIdentity & SqlBulkCopyOptions.KeepNulls))
                {
                    bulkCopy.DestinationTableName = System.Configuration.ConfigurationManager.AppSettings["PSLSTAGINGTAB"].ToString();
                    stagingProc = System.Configuration.ConfigurationManager.AppSettings["PSLSTAGINGPROC"].ToString();
                    try
                    {
                        sqlConnection.Open();
                        bulkCopy.ColumnMappings.Clear();
                        // Write from the source to the destination.
                        bulkCopy.ColumnMappings.Add("SubmitUser", "SubmitUser");
                        bulkCopy.ColumnMappings.Add("SubmitDT", "SubmitDT");
                        bulkCopy.ColumnMappings.Add("SubmitTM", "SubmitTM");
                        bulkCopy.ColumnMappings.Add("Vicinity", "Vicinity");
                        bulkCopy.ColumnMappings.Add("AddrLine1", "AddrLine1");
                        bulkCopy.ColumnMappings.Add("AddrLine2", "AddrLine2");
                        bulkCopy.ColumnMappings.Add("AddrLine3", "AddrLine3");
                        bulkCopy.ColumnMappings.Add("AddrLine4", "AddrLine4");
                        bulkCopy.ColumnMappings.Add("AddrLine5", "AddrLine5");
                        bulkCopy.ColumnMappings.Add("CGCTNum", "CGCTNum");
                        bulkCopy.ColumnMappings.Add("SSD", "SSD");
                        bulkCopy.ColumnMappings.Add("Phone", "Phone");
                        bulkCopy.ColumnMappings.Add("Work1DT", "Work1DT");
                        bulkCopy.ColumnMappings.Add("Work1TMStart", "Work1TMStart");
                        bulkCopy.ColumnMappings.Add("Work1TMEnd", "Work1TMEnd");
                        bulkCopy.ColumnMappings.Add("Work2DT", "Work2DT");
                        bulkCopy.ColumnMappings.Add("Work2TMStart", "Work2TMStart");
                        bulkCopy.ColumnMappings.Add("Work2TMEnd", "Work2TMEnd");
                        bulkCopy.ColumnMappings.Add("AccountID", "AccountID");
                        bulkCopy.ColumnMappings.Add("OutageID", "OutageID");
                        bulkCopy.ColumnMappings.Add("ContactID", "ContactID");
                        bulkCopy.ColumnMappings.Add("SPID", "SPID");
                        bulkCopy.ColumnMappings.Add("SPAddrLine1", "SPAddrLine1");
                        bulkCopy.ColumnMappings.Add("SPAddrLine2", "SPAddrLine2");
                        bulkCopy.ColumnMappings.Add("SPAddrLine3", "SPAddrLine3");
                        bulkCopy.ColumnMappings.Add("CustType", "CustType");
                        bulkCopy.ColumnMappings.Add("meter_number", "meter_number");
                        bulkCopy.ColumnMappings.Add("division", "division");
                        bulkCopy.ColumnMappings.Add("cn_fld1", "cn_fld1");
                        bulkCopy.ColumnMappings.Add("cn_fld2", "cn_fld2");
                        bulkCopy.ColumnMappings.Add("cn_fld3", "cn_fld3");
                        DataTable dt = CustomerListtoDataTable(pReport, PlannedOutageID, ContactID, Division, Work1DTStart, Work1TMStart, Work1TMEnd, Work2DTStart, Work2TMStart, Work2TMEnd, vicinity, strUserName);
                        if (dt != null)
                        {
                            bulkCopy.WriteToServer(dt);
                            success = true;
                        }
                        else
                        {
                            success = false;
                        }
                    }
                    catch (SqlException sqlex)
                    {
                        success = false;
                        Console.WriteLine(sqlex.Message);
                        bulkCopy.Close();
                        sqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        Console.WriteLine(ex.Message);
                        bulkCopy.Close();
                        sqlConnection.Close();

                    }
                    finally
                    {
                        if (bulkCopy != null && success == true)
                        {
                            strMessage = "Succcess";
                            bulkCopy.Close();
                            using (SqlCommand sqlCommand2 = new SqlCommand(stagingProc, sqlConnection))
                            {
                                sqlCommand2.CommandType = CommandType.StoredProcedure;
                                sqlCommand2.Parameters.Add("@iModeFlag", SqlDbType.TinyInt).Value = 1;
                                sqlCommand2.Parameters.Add("@iOutageID", SqlDbType.Int).Value = PlannedOutageID;
                                sqlCommand2.CommandTimeout = 600;
                                sqlCommand2.ExecuteNonQuery();
                            }
                        }
                        sqlConnection.Close();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                strMessage = "Fail";
            }
            return strMessage;
        }
        private static DataTable CustomerListtoDataTable(List<StagingdataCust> pReport, int PlannedOutageID, string ContactID, string Division, DateTime Work1DTStart, DateTime Work1TMStart, DateTime Work1TMEnd, DateTime Work2DTStart, DateTime Work2TMStart, DateTime Work2TMEnd, string vicinity, string strUserName)
        {
            DataTable pDT_Return = new DataTable(System.Configuration.ConfigurationManager.AppSettings["PSLSTAGINGTAB"].ToString());
            try
            {

                pDT_Return.Columns.Add("SubmitUser", typeof(string));
                pDT_Return.Columns.Add("SubmitDT", typeof(DateTime));
                pDT_Return.Columns.Add("SubmitTM", typeof(DateTime));
                pDT_Return.Columns.Add("Vicinity", typeof(string));
                pDT_Return.Columns.Add("AddrLine1", typeof(string));
                pDT_Return.Columns.Add("AddrLine2", typeof(string));
                pDT_Return.Columns.Add("AddrLine3", typeof(string));
                pDT_Return.Columns.Add("AddrLine4", typeof(string));
                pDT_Return.Columns.Add("AddrLine5", typeof(string));
                pDT_Return.Columns.Add("CGCTNum", typeof(string));
                pDT_Return.Columns.Add("SSD", typeof(string));
                pDT_Return.Columns.Add("Phone", typeof(string));
                pDT_Return.Columns.Add("Work1DT", typeof(DateTime));
                pDT_Return.Columns.Add("Work1TMStart", typeof(DateTime));
                pDT_Return.Columns.Add("Work1TMEnd", typeof(DateTime));
                pDT_Return.Columns.Add("Work2DT", typeof(DateTime));
                pDT_Return.Columns.Add("Work2TMStart", typeof(DateTime));
                pDT_Return.Columns.Add("Work2TMEnd", typeof(DateTime));
                pDT_Return.Columns.Add("AccountID", typeof(string));
                pDT_Return.Columns.Add("OutageID", typeof(Int32));
                pDT_Return.Columns.Add("ContactID", typeof(Int16));
                pDT_Return.Columns.Add("SPID", typeof(string));
                pDT_Return.Columns.Add("SPAddrLine1", typeof(string));
                pDT_Return.Columns.Add("SPAddrLine2", typeof(string));
                pDT_Return.Columns.Add("SPAddrLine3", typeof(string));
                pDT_Return.Columns.Add("CustType", typeof(string));
                pDT_Return.Columns.Add("meter_number", typeof(string));
                pDT_Return.Columns.Add("division", typeof(string));
                pDT_Return.Columns.Add("cn_fld1", typeof(string));
                pDT_Return.Columns.Add("cn_fld2", typeof(string));
                pDT_Return.Columns.Add("cn_fld3", typeof(string));

                string userName = strUserName;

                foreach (StagingdataCust customer in pReport)
                {
                    object[] obj = new object[] {userName,
                                             DateTime.Today,
                                             DateTime.Now,
                                             vicinity,
                                             customer.AddrLine1,
                                             customer.AddrLine2,
                                             customer.AddrLine3,
                                             customer.AddrLine4,
                                             customer.AddrLine5,
                                             customer.CGCTNum,
                                             customer.SSD,
                                             customer.Phone,
                                             Convert.ToDateTime(Work1DTStart),
                                             Convert.ToDateTime(Work1TMStart),
                                             Convert.ToDateTime(Work1TMEnd),
                                             Convert.ToDateTime(Work2DTStart),
                                             Convert.ToDateTime(Work2TMStart),
                                             Convert.ToDateTime(Work2TMEnd),
                                             customer.AccountID,
                                             PlannedOutageID,// Convert.ToInt32(0),
                                             ContactID,//DBNull.Value,
                                             customer.SPID,
                                             customer.SPAddrLine1,
                                             customer.SPAddrLine2,
                                             customer.SPAddrLine3,
                                             customer.CustType,
                                             customer.meter_number,
                                             Convert.ToInt32(Division),
                                             DBNull.Value,
                                             DBNull.Value,
                                             DBNull.Value
                };

                    if (Work2DTStart == default(DateTime))
                    {
                        obj[15] = DBNull.Value;
                        obj[16] = DBNull.Value;
                        obj[17] = DBNull.Value;
                    }

                    pDT_Return.Rows.Add(obj);
                }
                return pDT_Return;
            }
            catch (Exception ex)
            {
                pDT_Return = null;
                return pDT_Return;
            }
        }
        public class Pdfcustomerdataresult
        {

            public string Type { set; get; }
            public string CustomerName { set; get; }
            public string Address { set; get; }
            public string City_State_Zip { set; get; }
            public string CGC_TNum { set; get; }
            public string SSD { set; get; }
            public string Phone { set; get; }

        }
        public class PdfcustomerdataPREMISEresult
        {

            public string Type { set; get; }
            public string CustomerName { set; get; }
            public string Address { set; get; }
            public string City_State_Zip { set; get; }
            public string CGC_TNum { set; get; }
            public string SSD { set; get; }
            public string Phone { set; get; }
            public string PREMISEID { set; get; }
            public string PREMISETYPE { set; get; }
        }
        //Standard Map Print
        public string GetStandardPrint(List<string> Gridlist, string urlpath, string template, string size, string userid)
        {
            string sPath = "";

            try
            {

                DateTime t1 = System.DateTime.Now;

                strFolderName = template + "_" + t1.Day.ToString() + t1.Month.ToString() + t1.Year.ToString() + t1.Hour.ToString() + t1.Minute.ToString() + t1.Second.ToString() + "_" + userid;

                root = strFilePath + "\\" + strFolderName;


                if (!Directory.Exists(root))
                {

                    Directory.CreateDirectory(root);
                    foreach (string dr in Gridlist)
                    {

                        string[] scale_grid = dr.Split('#');

                        string sourcePath = strPrintFilePath + template + "-" + scale_grid[1] + "\\" + urlpath;

                        string fileName = template + "_" + scale_grid[0] + size + scale_grid[1] + ".pdf";



                        // Use Path class to manipulate file and directory paths.
                        string sourceFile = Path.Combine(sourcePath, fileName);
                        string destFile = Path.Combine(root, fileName);

                        if (File.Exists(sourceFile))
                        {

                            //File.Copy(fileName, destFile, true);
                            File.Copy(sourceFile, destFile, true);

                        }
                        else
                        {

                        }


                    }



                }
                ZIP_folder(root);

            }
            catch
            {
                sPath = "No Pdf Map file in this folder";
            }
            if (root == "No Pdf file in folder")
            {
                sPath = "No Pdf Map file in this folder";
            }
            else
            {
                sPath = root.Replace("\\", "$");
            }
            return sPath;
        }
        // Create zip file

        private const long BUFFER_SIZE = 40960000;

        private static void AddFileToZip(string zipFilename, string fileToAdd, CompressionOption compression = CompressionOption.Normal)
        {
            try
            {
                using (Package zip = System.IO.Packaging.Package.Open(zipFilename, FileMode.OpenOrCreate))
                {
                    string destFilename = ".\\" + Path.GetFileName(fileToAdd);
                    Uri uri = PackUriHelper.CreatePartUri(new Uri(destFilename, UriKind.Relative));
                    if (zip.PartExists(uri))
                    {
                        zip.DeletePart(uri);
                    }
                    PackagePart part = zip.CreatePart(uri, "", compression);
                    using (FileStream fileStream = new FileStream(fileToAdd, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream dest = part.GetStream())
                        {
                            fileStream.CopyTo(dest);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }


        public void ZIP_folder(string DirectoryToZip)
        {


            try
            {
                using (ZipFile zip = new ZipFile())
                {

                    String[] filenames = System.IO.Directory.GetFiles(DirectoryToZip);
                    // string pdfurl = "http://edgisapppqa02/PDF/";
                    string path = strFilePath + "\\" + strFolderName + ".zip";
                    string pdfurl = StandardPrintFilePDFURLNew + strFolderName + ".zip";
                    if (filenames.Length > 0)
                    {
                        foreach (String filename in filenames)
                        {

                            ZipEntry e = zip.AddFile(filename);
                            AddFileToZip(path, filename);

                        }


                        root = pdfurl;
                        //  zip.Save(root);

                    }
                    else
                    {

                        root = "No Pdf file in folder";
                    }

                }


            }
            catch
            {

            }
        }
        //jet
        public List<JetJoblist> Getjetjoblist(string STATUS, string RESERVEDBY, string DIVISION, string JobNumber, int pagesize)
        {
            List<JetJoblist> JetjobResult = new List<JetJoblist>();
            try
            {
                DataSet oraDataSetJetjob = new DataSet();
                oraDataSetJetjob = GetJetjob(STATUS, RESERVEDBY, DIVISION, JobNumber, pagesize);
                if (oraDataSetJetjob != null && oraDataSetJetjob.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetJetjob.Tables[0].Rows)
                    {
                        JetJoblist objjetjoblist = null;
                        objjetjoblist = getjetjobRow(dr, oraDataSetJetjob.Tables[0]);

                        if (objjetjoblist != null)
                            JetjobResult.Add(objjetjoblist);


                    }
                }
            }
            catch
            {

            }
            return JetjobResult;
        }

        private JetJoblist getjetjobRow(DataRow dr, DataTable pTable)
        {
            try
            {
                JetJoblist objjetlist = new JetJoblist();



                objjetlist.OBJECTID = Convert.ToInt32(dr["objectid"]);
                objjetlist.JOBNUMBER = Convert.ToString(dr["jobnumber"]);
                objjetlist.RESERVEDBY = Convert.ToString(dr["reservedby"]);
                objjetlist.DIVISION = Convert.ToString(dr["division"]);
                objjetlist.DESCRIPTION = Convert.ToString(dr["description"]);
                objjetlist.ENTRYDATE = Convert.ToDateTime(dr["entrydate"]);
                objjetlist.LASTMODIFIEDDATE = Convert.ToDateTime(dr["lastmodifieddate"]);
                objjetlist.USERAUDIT = Convert.ToString(dr["useraudit"]);
                objjetlist.STATUS = Convert.ToString(dr["status"]);


                //objectid,jobnumber, reservedby, division, description, entrydate, lastmodifieddate, useraudit, status

                return objjetlist;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }
        //Jet row count
        private string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.AppSettings["JetTns"];
        }
        public void jetrowcount(string STATUS, string RESERVEDBY, string DIVISION, string JobNumber, int pagenumber)
        {
            int rowcount;

            string _status = string.Empty;
            string strsql = string.Empty;
            int DivisionCode = 0;
            int _pagenumber;

            int _jetpagesize;
            _jetpagesize = Convert.ToInt32(Jetpagesize);
            if (pagenumber == null)
            {
                _pagenumber = 1;
            }
            else
            {
                _pagenumber = pagenumber;

            }
            if (DIVISION != "ALL")
            {
                DivisionCode = Convert.ToInt32(DIVISION);
            }
            if (STATUS == "1")
            {
                _status = "1";
            }
            else
            {
                _status = "2";
            }
            OracleConnection connection = new OracleConnection();
            OracleDataReader reader;
            try
            {

                string connectionString = GetConnectionString();

                connection.ConnectionString = connectionString;
                connection.Open();


                OracleCommand command = connection.CreateCommand();
                if (DIVISION == "ALL")
                {
                    strsql = "select count(*) from jet_jobs  where status=" + _status + "";
                }
                else if (DIVISION != "ALL" && RESERVEDBY == "ALL")
                {
                    strsql = "select count(*) from jet_jobs where status=" + _status + ",division=" + DivisionCode + "";
                }
                else if (DIVISION == "ALL" && RESERVEDBY != "ALL")
                {
                    strsql = "select count(*) from jet_jobs where status=" + _status + "";
                }
                else if (DIVISION != "ALL" && RESERVEDBY != "ALL")
                {
                    strsql = "select count(*) from jet_jobs where status=" + _status + ", division=" + DivisionCode + ",reservedby=" + RESERVEDBY + "";
                }
                else if (DIVISION != "ALL" && RESERVEDBY != "ALL" && JobNumber == "")
                {
                    strsql = "select count(*) from jet_jobs where status=" + _status + ", division=" + DivisionCode + ",reservedby=" + RESERVEDBY + "";
                }
                else if (DIVISION != "ALL" && RESERVEDBY != "ALL" && JobNumber != "")
                {
                    strsql = "select count(*) from jet_jobs where status=" + _status + ", division=" + DivisionCode + ",reservedby=" + RESERVEDBY + ",jobnumber LIKE '" + JobNumber + "%' ";
                }
                else
                {
                    strsql = "select count(*) from jet_jobs where status=" + _status + ",reservedby=" + RESERVEDBY + "";
                }
                command.CommandText = strsql;

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rowcount = Convert.ToInt32(reader[0]);

                }



            }
            catch
            {
            }
            finally
            {
                //reader.Close();
                connection.Close();

            }
        }
        //JetDataset

        public DataSet GetJetjob(string STATUS, string RESERVEDBY, string DIVISION, string JobNumber, int pagenumber)
        {
            DataSet orajetDataSet = new DataSet();
            OracleCommand command = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection connection = new OracleConnection();
            string _status = string.Empty;
            string strsql = string.Empty;
            int DivisionCode = 0;
            //int _pagenumber;
            //int startrow;
            //int Endrow;
            int _jetpagesize;
            _jetpagesize = Convert.ToInt32(Jetpagesize);
            //if (pagenumber==null)
            //{
            //    _pagenumber = 1;
            //}
            //else {
            //    _pagenumber = pagenumber;

            //}
            if (DIVISION != "ALL")
            {
                DivisionCode = Convert.ToInt32(DIVISION);
            }
            if (STATUS == "1")
            {
                _status = "1";
            }
            else
            {
                _status = "2";
            }
            // int rowcount_jat;
            //  rowcount_jat=jetrowcount( _status,  RESERVEDBY, DivisionCode,  JobNumber,  pagenumber);
            // startrow = (_jetpagesize * _pagenumber) - _jetpagesize + 1;
            // Endrow = (_jetpagesize * _pagenumber);

            try
            {
                string connectionString = GetJetConnectionString();



                strsql = "select objectid,jobnumber, reservedby, division, description, entrydate, lastmodifieddate, useraudit, status from jet_jobs ORDER by jobnumber, status";
                OracleConnection sqlConnection = new OracleConnection(strWipConnetionString);
                OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                // DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();

                //

            }
            catch (Exception ex)
            {
                orajetDataSet = null;
                connection.Close();
                command.Dispose();
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }
            return orajetDataSet;
        }
        // Jet Equipmentlist
        public List<jetequipmentlist> GetEquipmentJob(string objectID, string JobNumber)
        {

            List<jetequipmentlist> EquipmentJobResult = new List<jetequipmentlist>();
            try
            {
                DataSet oraDataSetJetjob = new DataSet();
                oraDataSetJetjob = DataEquipmentJob(JobNumber, objectID);

                if (oraDataSetJetjob != null && oraDataSetJetjob.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetJetjob.Tables[0].Rows)
                    {
                        jetequipmentlist objjetjoblist = null;
                        objjetjoblist = GetEquipmentJobRow(dr, oraDataSetJetjob.Tables[0]);

                        if (objjetjoblist != null)
                            EquipmentJobResult.Add(objjetjoblist);


                    }
                }
            }
            catch
            {

            }
            return EquipmentJobResult;
        }

        private jetequipmentlist GetEquipmentJobRow(DataRow dr, DataTable pTable)
        {
            try
            {
                jetequipmentlist objjetlist = new jetequipmentlist();

                objjetlist.OBJECTID = Convert.ToInt32(dr["objectid"]);
                objjetlist.JOBNUMBER = Convert.ToString(dr["jobnumber"]);
                objjetlist.Address = Convert.ToString(dr["ADDRESS"]);
                objjetlist.City = Convert.ToString(dr["CITY"]);
                objjetlist.Cgc12 = Convert.ToString(dr["CGC12"]);
                objjetlist.OperatingNumber = Convert.ToString(dr["OPERATINGNUMBER"]);
                objjetlist.SketchLoc = Convert.ToString(dr["SKETCHLOC"]);
                objjetlist.LATITUDE = Convert.ToDecimal(dr["LATITUDE"]);
                objjetlist.LONGITUDE = Convert.ToDecimal(dr["LONGITUDE"]);
                objjetlist.InstallCdName = Convert.ToString(dr["INSTALLCD"]);
                objjetlist.EQUIPTYPEID = Convert.ToInt32(dr["EQUIPTYPEID"]);
                objjetlist.ENTRYDATE = Convert.ToDateTime(dr["entrydate"]);
                objjetlist.LastModifiedDateLocal = Convert.ToDateTime(dr["lastmodifieddate"]);
                objjetlist.USERAUDIT = Convert.ToString(dr["useraudit"]);
                objjetlist.STATUS = Convert.ToString(dr["status"]);


                //objectid,jobnumber, reservedby, division, description, entrydate, lastmodifieddate, useraudit, status

                return objjetlist;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }
        //Get equi data
        public DataSet DataEquipmentJob(string JobNumber, string objectID)
        {
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection sqlConnection = new OracleConnection();
            string _status = string.Empty;
            string strsql = string.Empty;

            try
            {
                string connectionString = GetJetConnectionString();


                strsql = "select  * from jet_equipment where JOBNUMBER is not null and status='1'";
                sqlConnection = new OracleConnection(strWipConnetionString);
                sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                // DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();

                //

            }
            catch (Exception ex)
            {
                orajetDataSet = null;
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            return orajetDataSet;
        }
        //Delete
        public string GetDeleteJob(string objectID, string userid)
        {
            string result = string.Empty;
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection sqlConnection = new OracleConnection();
            string _status = string.Empty;
            string strsql = string.Empty;
            try
            {
                strsql = "Delete jet_jobs where objectid='" + objectID + "' and reservedby='" + userid + "'";
                sqlConnection = new OracleConnection(strWipConnetionString);
                sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();

                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();
                result = "S";
            }
            catch
            {
                result = "F";
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            return result;
        }

        //Delete Equipment
        public List<jetequipmentlist> GetDeleteEquipment(string objectID, string Opstatus)
        {
            List<jetequipmentlist> EquipmentJobResult = new List<jetequipmentlist>();
            try
            {
                DataSet oraDataSetJetjob = new DataSet();
                oraDataSetJetjob = DeleteEquipmentJobDataset(objectID, Opstatus);

                if (oraDataSetJetjob != null && oraDataSetJetjob.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetJetjob.Tables[0].Rows)
                    {
                        jetequipmentlist objjetjoblist = null;
                        objjetjoblist = GetEquipmentJobRow(dr, oraDataSetJetjob.Tables[0]);

                        if (objjetjoblist != null)
                            EquipmentJobResult.Add(objjetjoblist);


                    }
                }
            }
            catch
            {

            }
            return EquipmentJobResult;
        }
        //Delete data
        public string DeleteEquipmentJob(string objectID, string Opstatus)
        {
            string result = string.Empty;
            DataSet Deleteds = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection sqlConnection = new OracleConnection();
            string _status = string.Empty;
            string strsql = string.Empty;
            try
            {
                if (Opstatus == "D")
                {
                    strsql = "Delete jet_equipment where objectid ='" + objectID + " '";
                }
                sqlConnection = new OracleConnection(strWipConnetionString);
                sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();

                sqlDataAdapter.Fill(Deleteds);
                sqlConnection.Close();
                result = "S";
            }
            catch
            {
                result = "F";
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            return result;
        }
        //Return  data after delete
        public DataSet DeleteEquipmentJobDataset(string objectID, string Opstatus)
        {
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;

            string _status = string.Empty;
            string strsql = string.Empty;
            OracleConnection sqlConnection = new OracleConnection();
            string Result = DeleteEquipmentJob(objectID, Opstatus);
            if (Result == "S")
            {
                try
                {
                    string connectionString = GetJetConnectionString();

                    strsql = "select  * from jet_equipment where JOBNUMBER is not null and status='1'";
                    sqlConnection = new OracleConnection(strWipConnetionString);
                    sqlCommand = new OracleCommand(strsql, sqlConnection);
                    sqlCommand.CommandTimeout = 60000;
                    OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    sqlConnection.Open();
                    // DataSet objSet = new DataSet();
                    sqlDataAdapter.Fill(orajetDataSet);
                    sqlConnection.Close();

                    //

                }
                catch (Exception ex)
                {
                    orajetDataSet = null;

                    sqlConnection.Close();

                    sqlCommand.Dispose();
                }
                finally
                {

                    sqlConnection.Close();

                    sqlCommand.Dispose();
                }
            }
            else
            {
                orajetDataSet = null;
            }
            return orajetDataSet;
        }

        //Edit Equipment
        public List<jetequipmentlist> AddEditJobEquipment(string objectID, int STATUS, int EQUIPTYPEID, string OPERATINGNUMBER, string City, string SketchLoc, string InstallCdName, string Address, Decimal LATITUDE, Decimal LONGITUDE, string CUSTOWNED, string JobNumber, string USERAUDIT, string Cgc12)
        {
            List<jetequipmentlist> EquipmentJobResult = new List<jetequipmentlist>();
            try
            {
                DataSet oraDataSetJetjob = new DataSet();
                oraDataSetJetjob = InsertUpdateEquipmentDataset(objectID, STATUS, EQUIPTYPEID, OPERATINGNUMBER, City, SketchLoc, InstallCdName, Address, LATITUDE, LONGITUDE, CUSTOWNED, JobNumber, USERAUDIT, Cgc12);

                if (oraDataSetJetjob != null && oraDataSetJetjob.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetJetjob.Tables[0].Rows)
                    {
                        jetequipmentlist objjetjoblist = null;
                        objjetjoblist = GetEquipmentJobRow(dr, oraDataSetJetjob.Tables[0]);

                        if (objjetjoblist != null)
                            EquipmentJobResult.Add(objjetjoblist);
                    }
                }
            }
            catch
            {

            }
            return EquipmentJobResult;
        }
        public DataSet InsertUpdateEquipmentDataset(string objectID, int STATUS, int EQUIPTYPEID, string OPERATINGNUMBER, string City, string SketchLoc, string InstallCdName, string Address, Decimal LATITUDE, Decimal LONGITUDE, string CUSTOWNED, string JobNumber, string USERAUDIT, string Cgc12)
        {
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;

            string _status = string.Empty;
            string strsql = string.Empty;
            OracleConnection sqlConnection = new OracleConnection();
            string Result = InsertUpdateEquipmentData(objectID, STATUS, EQUIPTYPEID, OPERATINGNUMBER, City, SketchLoc, InstallCdName, Address, LATITUDE, LONGITUDE, CUSTOWNED, JobNumber, USERAUDIT, Cgc12);
            if (Result == "S")
            {
                try
                {
                    string connectionString = GetJetConnectionString();

                    strsql = "select  * from webr.jet_equipment where JOBNUMBER is not null and status='1' order by jobnumber";
                    sqlConnection = new OracleConnection(strWipConnetionString);
                    sqlCommand = new OracleCommand(strsql, sqlConnection);
                    sqlCommand.CommandTimeout = 60000;
                    OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    sqlConnection.Open();
                    // DataSet objSet = new DataSet();
                    sqlDataAdapter.Fill(orajetDataSet);
                    sqlConnection.Close();

                    //

                }
                catch (Exception ex)
                {
                    orajetDataSet = null;

                    sqlConnection.Close();

                    sqlCommand.Dispose();
                }
                finally
                {

                    sqlConnection.Close();

                    sqlCommand.Dispose();
                }
            }
            else
            {
                orajetDataSet = null;
            }
            return orajetDataSet;
        }

        public string InsertUpdateEquipmentData(string objectID, int STATUS, int EQUIPTYPEID, string OPERATINGNUMBER, string City, string SketchLoc, string InstallCdName, string Address, Decimal LATITUDE, Decimal LONGITUDE, string CUSTOWNED, string JobNumber, string USERAUDIT, string Cgc12)
        {               
            string dt;
            string result = string.Empty;
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection sqlConnection = new OracleConnection();
            //string _status = string.Empty;
            string strsql = string.Empty;
            DateTime t1 = System.DateTime.Now;
            // string Currentyear= t1.Year.ToString().Substring(2);
            //string CurrentMonth = DateTime.Now.ToString("MMM", CultureInfo.CurrentCulture);
            //dt = t1.Day.ToString() + "-" + CurrentMonth + "-" + t1.Year.ToString().Substring(2);
            dt = t1.Year.ToString() + "/" + t1.Month.ToString() + "/" + t1.Day + " " + t1.Hour.ToString() + ":" + t1.Minute.ToString() + ":" + t1.Second.ToString();
            
            try
            {
                if (Convert.ToInt64(objectID.ToString()) > 0)
                {
                    //edit equipment
                    strsql = "update WEBR.JET_EQUIPMENT set CITY='" + City.Replace("'", "''") + "',JOBNUMBER='" + JobNumber + "', EQUIPTYPEID='" + EQUIPTYPEID + "',STATUS=" + STATUS + ",SKETCHLOC='" + SketchLoc + "',USERAUDIT='" + USERAUDIT + "',INSTALLCD='" + InstallCdName + "',ADDRESS='" + Address.Replace("'", "''") + "',LATITUDE='" + LATITUDE + "',LONGITUDE='" + LONGITUDE + "',CUSTOWNED='" + CUSTOWNED + "',OPERATINGNUMBER='" + OPERATINGNUMBER + "', CGC12='" + Cgc12 + "',LASTMODIFIEDDATE= TO_DATE('" + dt + "', 'yyyy/mm/dd hh24:mi:ss') where objectid=" + objectID;
                    //(TO_DATE('2003/05/03 21:02:44', 'yyyy/mm/dd hh24:mi:ss')); Learn more about the TO_DATE function.
                    //to_date(sysdate,'dd-mon-yy hh24:mi:ss')
                }
                else
                {
                    //reserve equipment
                    strsql = "insert into WEBR.JET_EQUIPMENT (OBJECTID, CITY, JOBNUMBER, EQUIPTYPEID, STATUS, SKETCHLOC, USERAUDIT, ENTRYDATE, INSTALLCD, ADDRESS, LATITUDE, LONGITUDE, CUSTOWNED, OPERATINGNUMBER, CGC12, LASTMODIFIEDDATE) select max(OBJECTID) + 1,'" + City.Replace("'", "''") +
                        "','" + JobNumber + "'," + EQUIPTYPEID + "," + STATUS + ",'" + SketchLoc + "','" + USERAUDIT + "',TO_DATE('" + dt + "', 'yyyy/mm/dd hh24:mi:ss'),'" + InstallCdName + "','" + Address.Replace("'", "''") + "','" + LATITUDE + "','" + LONGITUDE + "','" + CUSTOWNED + "','" + OPERATINGNUMBER + "','" + Cgc12 + "',TO_DATE('" + dt + "', 'yyyy/mm/dd hh24:mi:ss') from WEBR.JET_EQUIPMENT";
                }
                sqlConnection = new OracleConnection(strWipConnetionString);
                sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();

                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();
                result = "S";
            }
            catch
            {
                result = "F";
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            return result;
        }

        //Change Job Number
        public List<jetequipmentlist> EditJobNumberEquipment(string oldJobNum, string newJobNum)
        {
            List<jetequipmentlist> EquipmentJobResult = new List<jetequipmentlist>();
            try
            {
                DataSet oraDataSetJetjob = new DataSet();
                oraDataSetJetjob = UpdateJobNumberEquipmentJobDataset(oldJobNum, newJobNum);

                if (oraDataSetJetjob != null && oraDataSetJetjob.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetJetjob.Tables[0].Rows)
                    {
                        jetequipmentlist objjetjoblist = null;
                        objjetjoblist = GetEquipmentJobRow(dr, oraDataSetJetjob.Tables[0]);

                        if (objjetjoblist != null)
                            EquipmentJobResult.Add(objjetjoblist);


                    }
                }
            }
            catch
            {

            }
            return EquipmentJobResult;
        }
        public DataSet UpdateJobNumberEquipmentJobDataset(string oldjob, string Newjob)
        {
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;

            string _status = string.Empty;
            string strsql = string.Empty;
            OracleConnection sqlConnection = new OracleConnection();
            string Result = GetEditJobnumberEquipmentData(oldjob, Newjob);
            if (Result == "S")
            {
                try
                {
                    string connectionString = GetJetConnectionString();

                    strsql = "select  * from jet_equipment where JOBNUMBER is not null and status='1' order by jobnumber";
                    sqlConnection = new OracleConnection(strWipConnetionString);
                    sqlCommand = new OracleCommand(strsql, sqlConnection);
                    sqlCommand.CommandTimeout = 60000;
                    OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                    sqlDataAdapter.SelectCommand = sqlCommand;
                    sqlConnection.Open();
                    // DataSet objSet = new DataSet();
                    sqlDataAdapter.Fill(orajetDataSet);
                    sqlConnection.Close();

                    //

                }
                catch (Exception ex)
                {
                    orajetDataSet = null;

                    sqlConnection.Close();

                    sqlCommand.Dispose();
                }
                finally
                {

                    sqlConnection.Close();

                    sqlCommand.Dispose();
                }
            }
            else
            {
                orajetDataSet = null;
            }
            return orajetDataSet;
        }

        public string GetEditJobnumberEquipmentData(string oldjob, string Newjob)
        {
            string dt;
            string result = string.Empty;
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection sqlConnection = new OracleConnection();
            //string _status = string.Empty;
            string strsql = string.Empty;
            DateTime t1 = System.DateTime.Now;
            // string Currentyear= t1.Year.ToString().Substring(2);
            string CurrentMonth = DateTime.Now.ToString("MMM", CultureInfo.CurrentCulture);
            dt = t1.Day.ToString() + "-" + CurrentMonth + "-" + t1.Year.ToString().Substring(2);
            try
            {
                strsql = "update jet_equipment set JOBNUMBER='" + Newjob + "' where JOBNUMBER='" + oldjob + "' ";
                sqlConnection = new OracleConnection(strWipConnetionString);
                sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();

                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();
                result = "S";
            }
            catch
            {
                result = "F";
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            return result;
        }

        //Edit
        public static string GetAbbreviatedFromFullName(string fullname)
        {
            DateTime month;
            return DateTime.TryParseExact(
                    fullname,
                    "MMMM",
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.None,
                    out month)
                ? month.ToString("MMM")
                : null;
        }
        //Edit serrvice
        public string GetEditJob(string objectID, int STATUS, string RESERVEDBY, int DIVISION, string JobNumber, string USERAUDIT, string description, DateTime ENTRYDATE)
        {
            string dt;
            string result = string.Empty;
            DataSet orajetDataSet = new DataSet();
            OracleCommand sqlCommand = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection sqlConnection = new OracleConnection();
            //string _status = string.Empty;
            string strsql = string.Empty;
            DateTime t1 = System.DateTime.Now;
            // string Currentyear= t1.Year.ToString().Substring(2);
            string CurrentMonth = DateTime.Now.ToString("MMM", CultureInfo.CurrentCulture);
            dt = t1.Day.ToString() + "-" + CurrentMonth + "-" + t1.Year.ToString().Substring(2);
            try
            {
                // strsql = "update jet_jobs set description='" + description + "',JOBNUMBER='" + JobNumber + "', DIVISION='" + DIVISION + "',STATUS='" + STATUS + "',reservedby='" + RESERVEDBY + "',USERAUDIT='" + USERAUDIT + "',ENTRYDATE='" + dt + "',LASTMODIFIEDDATE='" + dt + "' where objectid='" + objectID + "' and reservedby='" + RESERVEDBY + "'";
                strsql = "update jet_jobs set description='" + description.Replace("'", "''") + "',JOBNUMBER='" + JobNumber + "', DIVISION='" + DIVISION + "',STATUS='" + STATUS + "',reservedby='" + RESERVEDBY + "',USERAUDIT='" + USERAUDIT + "',ENTRYDATE='" + dt + "',LASTMODIFIEDDATE='" + dt + "' where objectid='" + objectID + "'";
                sqlConnection = new OracleConnection(strWipConnetionString);
                sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();

                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();
                result = "S";
            }
            catch
            {
                result = "F";
            }
            finally
            {
                sqlConnection.Close();
                sqlCommand.Dispose();
            }
            return result;
        }

        //WEBR Stability - Replace Feature Service with WCF for JET
        public bool[] ValidateAddEditJob(string description, int division, DateTime entryDate, string jobNumber, DateTime lastModifiedDate, string reservedBy, string userAudit, int status, int objectId, string action)
        {
            bool isUniqueJobNo = false;
            bool isSuccess = false;
            DataSet oraDataSet = new DataSet();
            OracleCommand oraCommand = new OracleCommand();
            OracleConnection oraConnection = null;
            OracleDataAdapter da = new OracleDataAdapter();
            try
            {
                oraConnection = new OracleConnection(strWipConnetionString);

                #region parameters
                //Adding definition for parameter one
                OracleParameter input1 = new OracleParameter();
                input1.ParameterName = "P_JOBNUMBER";
                input1.OracleType = OracleType.VarChar;
                input1.Direction = ParameterDirection.Input;
                input1.Value = jobNumber;
                input1.Size = 100;

                //Adding definition for parameter two
                OracleParameter input2 = new OracleParameter();
                input2.ParameterName = "P_DESCRIPTION";
                input2.OracleType = OracleType.VarChar;
                input2.Direction = ParameterDirection.Input;
                input2.Value = description;
                input2.Size = 100;

                //Adding definition for parameter three
                OracleParameter input3 = new OracleParameter();
                input3.ParameterName = "P_DIVISION";
                input3.OracleType = OracleType.Int16;
                input3.Direction = ParameterDirection.Input;
                input3.Value = division;
                input3.Size = 100;

                //Adding definition for parameter four
                OracleParameter input4 = new OracleParameter();
                input4.ParameterName = "P_ENTRYDATE";
                input4.OracleType = OracleType.DateTime;
                input4.Direction = ParameterDirection.Input;
                input4.Value = entryDate;
                input4.Size = 100;

                //Adding definition for parameter five
                OracleParameter input5 = new OracleParameter();
                input5.ParameterName = "P_LASTMODEFIEDDATE";
                input5.OracleType = OracleType.DateTime;
                input5.Direction = ParameterDirection.Input;
                input5.Value = lastModifiedDate;

                //Adding definition for parameter six
                OracleParameter input6 = new OracleParameter();
                input6.ParameterName = "P_USERAUDIT";
                input6.OracleType = OracleType.VarChar;
                input6.Direction = ParameterDirection.Input;
                input6.Value = userAudit;

                //Adding definition for parameter seven
                OracleParameter input7 = new OracleParameter();
                input7.ParameterName = "P_RESERVEDBY";
                input7.OracleType = OracleType.VarChar;
                input7.Direction = ParameterDirection.Input;
                input7.Value = reservedBy;

                //Adding definition for parameter eight
                OracleParameter input8 = new OracleParameter();
                input8.ParameterName = "P_STATUS";
                input8.OracleType = OracleType.Int16;
                input8.Direction = ParameterDirection.Input;
                input8.Value = status;

                //Adding definition for parameter nine
                OracleParameter input9 = new OracleParameter();
                input9.ParameterName = "P_OBJECTID";
                input9.OracleType = OracleType.Int32;
                input9.Direction = ParameterDirection.Input;
                input9.Value = objectId;

                //Adding definition for parameter ten
                OracleParameter input10 = new OracleParameter();
                input10.ParameterName = "P_ACTION";
                input10.OracleType = OracleType.VarChar;
                input10.Direction = ParameterDirection.Input;
                input10.Value = action;

                //Adding definition for parameter eleven
                OracleParameter output1 = new OracleParameter();
                output1.ParameterName = "P_VALIDJOBNO";
                output1.OracleType = OracleType.Number;
                output1.Direction = ParameterDirection.Output;
                output1.Size = 10;
                output1.Value = 0;

                //Adding definition for parameter twelve
                OracleParameter output2 = new OracleParameter();
                output2.ParameterName = "P_SUCCESS";
                output2.OracleType = OracleType.Number;
                output2.Direction = ParameterDirection.Output;
                output2.Size = 10;
                output2.Value = 0;
                # endregion

                oraCommand.Parameters.Add(input1);
                oraCommand.Parameters.Add(input2);
                oraCommand.Parameters.Add(input3);
                oraCommand.Parameters.Add(input4);
                oraCommand.Parameters.Add(input5);
                oraCommand.Parameters.Add(input6);
                oraCommand.Parameters.Add(input7);
                oraCommand.Parameters.Add(input8);
                oraCommand.Parameters.Add(input9);
                oraCommand.Parameters.Add(input10);
                oraCommand.Parameters.Add(output1);
                oraCommand.Parameters.Add(output2);

                oraCommand.Connection = oraConnection;
                oraCommand.CommandType = CommandType.StoredProcedure;
                oraCommand.CommandText = strValidateAndAddJobSPName;
                oraCommand.CommandTimeout = 600;

                oraConnection.Open();
                da = new OracleDataAdapter(oraCommand);
                da.Fill(oraDataSet);

                if (Convert.ToInt16(output1.Value) == 1)
                    isUniqueJobNo = true;

                if (Convert.ToInt16(output2.Value) == 1)
                    isSuccess = true;

            }
            catch (Exception ex)
            {
                isSuccess = false;
                Console.WriteLine("ValidateAddEditJob method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            return new bool[] { isUniqueJobNo, isSuccess };
        }

        public List<EquipmentIdType> GetEquipmentIDTypes()
        {
            List<EquipmentIdType> GetEquipIdTypeResult = new List<EquipmentIdType>();
            try
            {
                DataSet oraDataSetEquipIdType = new DataSet();
                oraDataSetEquipIdType = getEquipIdTypeDataset();
                if (oraDataSetEquipIdType != null && oraDataSetEquipIdType.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetEquipIdType.Tables[0].Rows)
                    {
                        EquipmentIdType objEquipIdType = null;
                        objEquipIdType = getEquipIdTypeRow(dr, oraDataSetEquipIdType.Tables[0]);

                        if (objEquipIdType != null)
                            GetEquipIdTypeResult.Add(objEquipIdType);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetEquipmentIDTypes method: " + ex.Message);
            }
            return GetEquipIdTypeResult;
        }

        private DataSet getEquipIdTypeDataset()
        {
            DataSet oraEuipIdTypeDataSet = new DataSet();
            OracleCommand oraCommand = null;
            OracleConnection oraConnection = null;
            string strsql = string.Empty;

            try
            {
                strsql = "select OBJECTID, EQUIPTYPEID, EQUIPTYPEDESC, HASOPERATINGNUM, HASCGC12 from WEBR.JET_EQUIPIDTYPE";
                oraConnection = new OracleConnection(strWipConnetionString);
                oraCommand = new OracleCommand(strsql, oraConnection);
                oraCommand.CommandTimeout = 60000;
                OracleDataAdapter oraDataAdapter = new OracleDataAdapter();
                oraDataAdapter.SelectCommand = oraCommand;
                oraConnection.Open();
                oraDataAdapter.Fill(oraEuipIdTypeDataSet);
                oraConnection.Close();

            }
            catch (Exception ex)
            {
                oraEuipIdTypeDataSet = null;
                oraConnection.Close();
                oraCommand.Dispose();
                Console.WriteLine("getEquipIdTypeDataset method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            return oraEuipIdTypeDataSet;
        }

        private EquipmentIdType getEquipIdTypeRow(DataRow dr, DataTable pTable)
        {
            try
            {
                EquipmentIdType objEquipIdTypeRecord = new EquipmentIdType();

                objEquipIdTypeRecord.ObjectId = Convert.ToString(dr["OBJECTID"]);
                objEquipIdTypeRecord.EquipTypeId = Convert.ToInt16(dr["EQUIPTYPEID"]);
                objEquipIdTypeRecord.EquipTypeDesc = Convert.ToString(dr["EQUIPTYPEDESC"]);
                objEquipIdTypeRecord.HasOperatingNum = Convert.ToString(dr["HASOPERATINGNUM"]);
                objEquipIdTypeRecord.HasCGC12 = Convert.ToString(dr["HASCGC12"]);

                return objEquipIdTypeRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getEquipIdTypeRow method: " + ex.Message);
                return null;
            }

        }

        public Dictionary<string, string> GetInstallTypeDomain()
        {
            Dictionary<string, string> installTypeDomainResult = new Dictionary<string, string>();
            try
            {
                DataSet oraInsTypeDomainDataSet = new DataSet();
                oraInsTypeDomainDataSet = getCodeDomainDataset("WEBR.JET_EQUIPMENT", "INSTALLCD", strWipConnetionString);
                if (oraInsTypeDomainDataSet != null && oraInsTypeDomainDataSet.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraInsTypeDomainDataSet.Tables[0].Rows)
                    {
                        installTypeDomainResult.Add(Convert.ToString(dr["CODE"]), Convert.ToString(dr["VALUE"]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetEquipmentIDTypes method: " + ex.Message);
            }
            return installTypeDomainResult;
        }

        //get the code- domain values for the domain applied to a field of a table
        private DataSet getCodeDomainDataset(string tableName, string fieldName, string connetionString)
        {
            DataSet oraDataSet = new DataSet();
            OracleCommand oraCommand = null;
            OracleConnection oraConnection = null;
            string strsql = string.Empty;

            try
            {
                strsql = "SELECT EXTRACTVALUE(CodedValues.COLUMN_VALUE, 'CodedValue/Code') AS CODE,EXTRACTVALUE(CodedValues.COLUMN_VALUE, 'CodedValue/Name') AS VALUE FROM SDE.GDB_ITEMS_VW items INNER JOIN SDE.GDB_ITEMTYPES itemtypes ON items.Type = itemtypes.UUID,TABLE(XMLSEQUENCE(XMLType(Definition).Extract('/GPCodedValueDomain2/CodedValues/CodedValue'))) CodedValues WHERE itemtypes.Name = 'Coded Value Domain' AND items.Name = (SELECT EXTRACTVALUE(definition_xml.COLUMN_VALUE, 'GPFieldInfoEx/DomainName') AS domain_name FROM SDE.GDB_ITEMS_VW items JOIN SDE.GDB_ITEMTYPES it ON items.Type = it.UUID CROSS JOIN TABLE(XMLSEQUENCE(XMLType(definition).Extract('/DETableInfo/GPFieldInfoExs/GPFieldInfoEx'))) definition_xml WHERE upper(items.NAME) = '" + tableName + "' AND UPPER(EXTRACTVALUE(definition_xml.COLUMN_VALUE, 'GPFieldInfoEx/Name')) = '" + fieldName + "' and rownum = 1)";
                oraConnection = new OracleConnection(connetionString);
                oraCommand = new OracleCommand(strsql, oraConnection);
                oraCommand.CommandTimeout = 60000;
                OracleDataAdapter oraDataAdapter = new OracleDataAdapter();
                oraDataAdapter.SelectCommand = oraCommand;
                oraConnection.Open();
                oraDataAdapter.Fill(oraDataSet);
                oraConnection.Close();

            }
            catch (Exception ex)
            {
                oraDataSet = null;
                oraConnection.Close();
                oraCommand.Dispose();
                Console.WriteLine("getCodeDomainDataset method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            return oraDataSet;
        }

        //get the last equipment location for a job
        public jetequipmentlist GetJetLastEquipLocation(string jobNumber)
        {
            jetequipmentlist jetLastEquipLocResult = new jetequipmentlist();
            try
            {
                DataSet oraDataSetLastEquip = new DataSet();
                oraDataSetLastEquip = getLastEquipLocDataset(jobNumber);
                if (oraDataSetLastEquip != null && oraDataSetLastEquip.Tables.Count > 0)
                {
                    if (oraDataSetLastEquip.Tables[0].Rows.Count > 0)
                    {
                        jetLastEquipLocResult.Address = Convert.ToString(oraDataSetLastEquip.Tables[0].Rows[0]["ADDRESS"]);
                        jetLastEquipLocResult.City = Convert.ToString(oraDataSetLastEquip.Tables[0].Rows[0]["CITY"]);
                        jetLastEquipLocResult.LATITUDE = Convert.ToDecimal(oraDataSetLastEquip.Tables[0].Rows[0]["LATITUDE"]);
                        jetLastEquipLocResult.LONGITUDE = Convert.ToDecimal(oraDataSetLastEquip.Tables[0].Rows[0]["LONGITUDE"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetJetLastEquipLocation method: " + ex.Message);
            }
            return jetLastEquipLocResult;
        }

        private DataSet getLastEquipLocDataset(string jobNumber)
        {
            DataSet oraLastEquipDataSet = new DataSet();
            OracleCommand oraCommand = null;
            OracleConnection oraConnection = null;
            string strsql = string.Empty;

            try
            {
                strsql = "select ADDRESS, CITY, LATITUDE, LONGITUDE from WEBR.JET_EQUIPMENT where JOBNUMBER='" + jobNumber + "' and LASTMODIFIEDDATE = (SELECT MAX(LASTMODIFIEDDATE) from WEBR.JET_EQUIPMENT where JOBNUMBER='" + jobNumber + "')";
                oraConnection = new OracleConnection(strWipConnetionString);
                oraCommand = new OracleCommand(strsql, oraConnection);
                oraCommand.CommandTimeout = 60000;
                OracleDataAdapter oraDataAdapter = new OracleDataAdapter();
                oraDataAdapter.SelectCommand = oraCommand;
                oraConnection.Open();
                oraDataAdapter.Fill(oraLastEquipDataSet);
                oraConnection.Close();

            }
            catch (Exception ex)
            {
                oraLastEquipDataSet = null;
                oraConnection.Close();
                oraCommand.Dispose();
                Console.WriteLine("getLastEquipLocDataset method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            return oraLastEquipDataSet;
        }

        public Dictionary<object, string> GetJETDivisions()
        {
            Dictionary<object, string> divisionDomainResult = new Dictionary<object, string>();
            try
            {
                DataSet oraDivDomainDataSet = new DataSet();
                oraDivDomainDataSet = getCodeDomainDataset("WEBR.JET_JOBS", "DIVISION", strWipConnetionString);
                if (oraDivDomainDataSet != null && oraDivDomainDataSet.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDivDomainDataSet.Tables[0].Rows)
                    {
                        divisionDomainResult.Add(Convert.ToInt16(dr["CODE"]), Convert.ToString(dr["VALUE"]));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetJETDivisions method: " + ex.Message);
            }
            return divisionDomainResult;
        }

        public List<JetViewJobEquip> SearchJetJobEquip(string searchInput, string filterBase)
        {
            List<JetViewJobEquip> jetSearchResult = new List<JetViewJobEquip>();
            try
            {
                DataSet oraSearchDataSet = new DataSet();
                oraSearchDataSet = getSearchJobDataset(searchInput, filterBase);
                if (oraSearchDataSet != null && oraSearchDataSet.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraSearchDataSet.Tables[0].Rows)
                    {
                        JetViewJobEquip jobEquipObj = new JetViewJobEquip();
                        jobEquipObj.ObjectId = Convert.ToString(dr["OBJECTID"]);
                        jobEquipObj.JobNumber = Convert.ToString(dr["JOBNUMBER"]);
                        jobEquipObj.Address = Convert.ToString(dr["ADDRESS"]);
                        jobEquipObj.CGC12 = Convert.ToString(dr["CGC12"]);
                        jobEquipObj.Description = Convert.ToString(dr["DESCRIPTION"]);

                        jetSearchResult.Add(jobEquipObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SearchJetJobEquip method: " + ex.Message);
            }
            return jetSearchResult;
        }

        private DataSet getSearchJobDataset(string searchInput, string filterBase)
        {
            DataSet oraSearchJobDataSet = new DataSet();
            OracleCommand oraCommand = null;
            OracleConnection oraConnection = null;
            string strsql = string.Empty;

            try
            {
                strsql = "select * from WEBR.JET_VW_JOB_EQUIP where (upper(JOBNUMBER) like '%" + searchInput + "%' or upper(DESCRIPTION) like '%" + searchInput + "%' or upper(CGC12) like '%" + searchInput + "%' or upper(ADDRESS) like '%" + searchInput + "%') and (" + filterBase + ") order by OBJECTID";
                oraConnection = new OracleConnection(strWipConnetionString);
                oraCommand = new OracleCommand(strsql, oraConnection);
                oraCommand.CommandTimeout = 60000;
                OracleDataAdapter oraDataAdapter = new OracleDataAdapter();
                oraDataAdapter.SelectCommand = oraCommand;
                oraConnection.Open();
                oraDataAdapter.Fill(oraSearchJobDataSet);
                oraConnection.Close();

            }
            catch (Exception ex)
            {
                oraSearchJobDataSet = null;
                oraConnection.Close();
                oraCommand.Dispose();
                Console.WriteLine("getSearchJobDataset method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            return oraSearchJobDataSet;
        }

        List<DataToExportDetails> DataToExportDetailsResult = new List<DataToExportDetails>(); //ENOS Tariff Change
        //ENOS
        public List<GenFeederDetails> GetGenFeederDetails(string FeederId)
        {
            List<GenFeederDetails> FeederDetailsResult = new List<GenFeederDetails>();
            try
            {
                DataSet oraDataSetFeeder = new DataSet();
                oraDataSetFeeder = GenDataFromFeeder(FeederId);
                if (oraDataSetFeeder != null && oraDataSetFeeder.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetFeeder.Tables[0].Rows)
                    {
                        GenFeederDetails objjetjoblist = null;
                        objjetjoblist = getFeederRow(dr, oraDataSetFeeder.Tables[0], FeederId);

                        if (objjetjoblist != null)
                            FeederDetailsResult.Add(objjetjoblist);


                    }
                    /****************************ENOS2SAP PhaseIII Start****************************/
                    List<InverterNameplateDetails> ListInverterNP = null;
                    ListInverterNP = getInverterNameplateFromSettings(sapEgiNotifications);
                    if (ListInverterNP != null)
                    {
                        if (ListInverterNP.Count > 0)
                        {
                            for (int i = 0; i < FeederDetailsResult.Count; i++)
                            {
                                for (int j = 0; j < ListInverterNP.Count; j++)
                                {
                                    if (FeederDetailsResult[i].ProjectReference == Convert.ToString(ListInverterNP[j].ProjectReference))
                                    {
                                        FeederDetailsResult[i].Nameplate = Convert.ToDouble(ListInverterNP[j].GenSize);
                                        //FeederDetailsResult[i].GenSize = Convert.ToDouble(ListInverterNP[j].GenSize);
                                    }

                                }
                            }
                        }

                    }
                    /****************************ENOS2SAP PhaseIII End****************************/
                }

            }
            catch
            {

            }
            return FeederDetailsResult;
        }

        //Gen dataset from feeder
        public DataSet GenDataFromFeeder(string FeederId)
        {
            DataSet orajetDataSet = new DataSet();
            OracleCommand command = new OracleCommand();
            // OracleDataAdapter da = null;
            OracleConnection connection = new OracleConnection();
            string _status = string.Empty;
            string strsql = string.Empty;

            try
            {
                //string connectionString = GetJetConnectionString();


                //strsql =  "select sp.SERVICEPOINTID, sp.servicelocationguid,sp.transformerguid,sp.primarymeterguid,sp.City,sp.STREETNUMBER,sp.STREETNAME1,"
                //        + "sp.state,sp.meternumber,sp.CGC12,gen.GENTYPE,nvl(EFFRATINGMACHKW,0)+nvl(EFFRATINGINVKW,0) GENSIZE,gen.SAPEGINOTIFICATION,gen.globalid from edgis.zz_mv_servicepoint sp,"
                //        + "(select * from edgis.zz_mv_generationinfo where servicepointguid in ("
                //        + "select globalid from edgis.zz_mv_servicepoint where servicelocationguid in ("
                //        + "select globalid from edgis.zz_mv_servicelocation where circuitid='"+FeederId+"'))) gen where gen.servicepointguid=sp.globalid";


                //ENOS Taiff change- Query Modified
                strsql = "select sp.SERVICEPOINTID, sp.servicelocationguid,sp.transformerguid,sp.primarymeterguid,sp.City,sp.STREETNUMBER,sp.STREETNAME1,sp.state,sp.meternumber,sp.CGC12,"
+ "gen.GENTYPE,nvl(gen.EFFRATINGMACHKW,0)+nvl(gen.EFFRATINGINVKW,0) GENSIZE,gen.SAPEGINOTIFICATION,gen.globalid,DECODE(gen.DERATED,'Y','Yes','N','No') DERATED, DECODE(gen.METHODOFLIMITEDEXPORT,'1','Relay','2','Control Scheme') METHODOFLIMITEDEXPORT from edgis.zz_mv_servicepoint sp,(select * from edgis.zz_mv_generationinfo where servicepointguid in (select globalid from edgis.zz_mv_servicepoint where servicelocationguid in "
+ "(select globalid from edgis.zz_mv_servicelocation where circuitid='" + FeederId + "' and gencategory in (1,2)))) gen where gen.servicepointguid=sp.globalid";



                OracleConnection sqlConnection = new OracleConnection(strEnocConnectionString);
                OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();

                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();

                //

            }
            catch (Exception ex)
            {
                orajetDataSet = null;
                connection.Close();
                command.Dispose();
            }
            finally
            {
                connection.Close();
                command.Dispose();
            }
            return orajetDataSet;
        }

        private GenFeederDetails getFeederRow(DataRow dr, DataTable pTable, string FeederId)
        {
            try
            {
                GenFeederDetails objGenlist = new GenFeederDetails();
                string address = string.Empty;
                string streetnumber = string.Empty;
                string streetName1 = string.Empty;
                string state = string.Empty;
                //Adding City to Address 
                string city = string.Empty;
                if (Convert.ToString(dr["STREETNUMBER"]) != null)
                {
                    streetnumber = Convert.ToString(dr["STREETNUMBER"]);

                    address = streetnumber + " ";

                }
                if (Convert.ToString(dr["STREETNAME1"]) != null)
                {
                    streetName1 = Convert.ToString(dr["STREETNAME1"]);
                    address += streetName1 + ",";
                }

                if (Convert.ToString(dr["CITY"]) != null)
                {
                    city = Convert.ToString(dr["CITY"]);
                    address += city + ",";
                }


                if (Convert.ToString(dr["STATE"]) != null)
                {
                    state = Convert.ToString(dr["STATE"]);
                    address += state;
                }
                else
                {
                    address = address.TrimEnd(',');
                }

                if (Convert.ToString(dr["STATE"]) != null)
                {
                    state = Convert.ToString(dr["STATE"]);
                    address += state;
                }
                else
                {
                    address = address.TrimEnd(',');
                }
                /****************************ENOS2SAP PhaseIII Start****************************/
                if (pTable.Rows.IndexOf(dr) != pTable.Rows.Count - 1)
                {
                    sapEgiNotifications = sapEgiNotifications + "'" + Convert.ToString(dr["SAPEGINOTIFICATION"]) + "',";
                }
                else
                {
                    sapEgiNotifications = sapEgiNotifications + "'" + Convert.ToString(dr["SAPEGINOTIFICATION"]) + "'";
                }
                /****************************ENOS2SAP PhaseIII End****************************/
                //  address = address.ToUpper();

                objGenlist.Address = address.ToUpper();
                objGenlist.METERNUMBER = Convert.ToString(dr["MeterNumber"]) != null ? Convert.ToString(dr["MeterNumber"]) : "NA";
                objGenlist.SPID = Convert.ToString(dr["SERVICEPOINTID"]);
                objGenlist.CGC12 = Convert.ToString(dr["CGC12"]) != null ? Convert.ToString(dr["CGC12"]) : "NA";
                objGenlist.PMGUID = Convert.ToString(dr["PRIMARYMETERGUID"]) != null ? Convert.ToString(dr["PRIMARYMETERGUID"]) : "NA";
                objGenlist.TransGUID = (Convert.ToString(dr["TRANSFORMERGUID"])).Replace("}", "").Replace("{", "");
                objGenlist.SLGUID = (Convert.ToString(dr["SERVICELOCATIONGUID"])).Replace("}", "").Replace("{", "");
                objGenlist.GENGLOBALID = Convert.ToString(dr["GLOBALID"]) != null ? Convert.ToString(dr["GLOBALID"]) : "NA";
                objGenlist.GenType = Convert.ToString(dr["GENTYPE"]) != null ? Convert.ToString(dr["GENTYPE"]) : "NA";
                /****************************ENOS2SAP PhaseIII Start****************************/
                //objGenlist.GenSize = 0;
                objGenlist.GenSize = Convert.ToDouble(dr["GENSIZE"]);
                /****************************ENOS2SAP PhaseIII End****************************/
                objGenlist.ProjectReference = Convert.ToString(dr["SAPEGINOTIFICATION"]) != null ? Convert.ToString(dr["SAPEGINOTIFICATION"]) : "NA"; ;
                objGenlist.FEEDERNUM = FeederId;

                /*****ENOS Tariff Changes Starts*******/
                objGenlist.Derated = Convert.ToString(dr["DERATED"]) != null ? Convert.ToString(dr["DERATED"]) : "";
                objGenlist.LimitedExport = Convert.ToString(dr["METHODOFLIMITEDEXPORT"]) != null ? Convert.ToString(dr["METHODOFLIMITEDEXPORT"]) : "";
                /*****ENOS Tariff Changes End*******/

                //select sp.SERVICEPOINTID, sp.servicelocationguid,sp.transformerguid,sp.primarymeterguid,sp.City,sp.STREETNUMBER,sp.STREETNAME1,sp.state,sp.meternumber,sp.CGC12,gen.GENTYPE,nvl(EFFRATINGMACHKW,0)+nvl(EFFRATINGINVKW,0) GENSIZE,gen.SAPEGINOTIFICATION,gen.globalid from edgis.zz_mv_servicepoint sp,(select * from edgis.zz_mv_generationinfo where servicepointguid in (select globalid from edgis.zz_mv_servicepoint where servicelocationguid in (select globalid from edgis.zz_mv_servicelocation where circuitid='182011104'))) gen where gen.servicepointguid=sp.globalid

                return objGenlist;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }

        //CIT Common method for Filledduct button *start
        public bool CIT_Process(int objectID, string globalID, string filledduct, string requested_on, string lanid)
        {
            bool process_Run = false;
            try
            {
                DataSet oraDataSetCIT = new DataSet();
                oraDataSetCIT = CIT_Save(objectID, globalID, filledduct, requested_on, lanid);
                if (oraDataSetCIT != null)
                {
                    process_Run = true;
                }
                else
                {
                    process_Run = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return process_Run;
        }

        public DataSet CIT_Save(int objectid, string globalid, string filledDuct, string requested_On, string LANID)
        {
            DataTable check_ValueExisting = new DataTable();
            DataSet orajetDataSet = new DataSet();
            string _status = string.Empty;
            string strsql = string.Empty;
            try
            {
                //need to put value in config
                //Check if value already existing, if yes then update
                check_ValueExisting = Webr_ManualTable(globalid, LANID);
                if (check_ValueExisting.Rows.Count >= 1)
                {
                    strsql = "update " + manual_table + " set FILLEDDUCT =" + filledDuct + ", REQUESTED_ON = to_date('" + requested_On + "','MM/DD/YYYY') where GLOBALID='" + globalid + "' and STATUS = 'New' and REQUESTED_BY='" + LANID + "'"; //REQUESTED_ON = to_date('" + requested_On + "','MM/DD/YYYY')
                }
                else
                {
                    strsql = "insert into " + manual_table + "(OBJECTID, GLOBALID, FILLEDDUCT, STATUS, REQUESTED_ON, REQUESTED_BY ) values (" + objectid + ",'" + globalid + "'," + filledDuct + ",'New', to_date('" + requested_On + "','MM/DD/YYYY'),'" + LANID + "')";
                }
                OracleConnection sqlConnection = new OracleConnection(strWipConnetionString);
                OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                sqlDataAdapter.Fill(orajetDataSet);
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                orajetDataSet = null;
            }
            return orajetDataSet;
        }

        public DataTable Webr_ManualTable(string globalid, string LANID)
        {
            DataTable dtWebrManual_PriUG = null;
            try
            {
                string strsql = "select * from " + manual_table + " where GLOBALID='" + globalid + "' and STATUS = 'New' and REQUESTED_BY='" + LANID + "'";
                dtWebrManual_PriUG = new DataTable();
                OracleConnection sqlConnection = new OracleConnection(strWipConnetionString);
                OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                sqlDataAdapter.Fill(dtWebrManual_PriUG);
                sqlConnection.Close();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return dtWebrManual_PriUG;
        }

        public int CIT_ConduitType(int objectID)
        {
            int conduit_type = 0;
            int conduit_values_count = 0;
            try
            {
                DataTable oraDataTableCIT = new DataTable();
                oraDataTableCIT = CIT_Objectid(objectID);
                if (oraDataTableCIT != null)
                {
                    if (oraDataTableCIT.Rows.Count > 0)
                    {
                        conduit_values_count = oraDataTableCIT.Rows.Count;
                        //   bool values = oraDataTableCIT.Columns["SUBTYPECD"].Unique;


                        var values = oraDataTableCIT.Rows.Cast<DataRow>()
                                    .Select(r => r[0])
                                    .Distinct()
                                    .ToList();
                        var unique = values.Count == 1;


                        if (unique == true)
                        {
                            conduit_type = Convert.ToInt32(oraDataTableCIT.Rows[0]["SUBTYPECD"]);
                        }
                        else
                        {
                            conduit_type = 9;
                        }
                    }
                    else
                    {
                        conduit_type = 11;//Direct Buried
                    }
                }
                else
                {
                    conduit_type = 10;//Exception
                }
            }
            catch (Exception ex)
            {
            }
            return conduit_type;
        }

        public DataTable CIT_Objectid(int objectid)
        {
            DataTable orajetDataTable = new DataTable();

            string _status = string.Empty;
            string strsql = string.Empty;
            try
            {
                //need to put value in config
                strsql = "select SUBTYPECD from edgis.zz_mv_conduitsystem where objectid in (select ulsobjectid from EDGIS.zz_mv_ConduitSystem_PriUG where ugobjectid= '" + objectid + "')";
                OracleConnection sqlConnection = new OracleConnection(CITConnectionString);
                OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                sqlDataAdapter.Fill(orajetDataTable);
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                orajetDataTable = null;
                //if (ex.Message.Contains("single-row subquery returns more than one row"))
                //{
                //    orajetDataTable.Rows.Add("4");
                //}
                //else
                //{
                //    orajetDataTable = null;
                //}
            }
            return orajetDataTable;
        }

        public int CIT_FilledValue(int objectID)
        {
            int filled_Value = 0;
            try
            {
                DataTable oraDataTableCIT = new DataTable();
                oraDataTableCIT = CIT_FilledDuctValue(objectID);
                if (oraDataTableCIT != null)
                {
                    if (oraDataTableCIT.Rows.Count > 0)
                    {
                        filled_Value = Convert.ToInt32(oraDataTableCIT.Rows[0]["FILLEDDUCT"]);
                    }
                    else
                    {
                        filled_Value = 0;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return filled_Value;
        }

        public DataTable CIT_FilledDuctValue(int objectid)
        {
            DataTable orajetDataTable = new DataTable();

            string _status = string.Empty;
            string strsql = string.Empty;
            try
            {
                //need to put value in config
                strsql = "select FILLEDDUCT from edgis.zz_mv_priugconductor where objectid ='" + objectid + "'";
                OracleConnection sqlConnection = new OracleConnection(CITConnectionString);
                OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                sqlDataAdapter.Fill(orajetDataTable);
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                orajetDataTable = null;
            }
            return orajetDataTable;
        }

        public DateTime? CIT_UpdatedonValue(int objectID)
        {
            DateTime? Cit_updatedon = null;
            try
            {
                DataTable oraDataTableCIT = new DataTable();
                oraDataTableCIT = CIT_DateValue(objectID);
                if (oraDataTableCIT != null)
                {
                    if (oraDataTableCIT.Rows.Count > 0)
                    {
                        if (oraDataTableCIT.Rows[0]["CIT_UPDATEDON"] is DBNull)
                        {
                            Cit_updatedon = null;
                        }
                        else
                        {
                            Cit_updatedon = Convert.ToDateTime(oraDataTableCIT.Rows[0]["CIT_UPDATEDON"]);
                        }
                    }
                    else
                    {
                        Cit_updatedon = null;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return Cit_updatedon;
        }

        public DataTable CIT_DateValue(int objectid)
        {
            DataTable orajetDataTable = new DataTable();

            string _status = string.Empty;
            string strsql = string.Empty;
            try
            {
                //need to put value in config
                strsql = "select CIT_UPDATEDON from edgis.zz_mv_priugconductor where objectid ='" + objectid + "'";
                OracleConnection sqlConnection = new OracleConnection(CITConnectionString);
                OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                sqlDataAdapter.Fill(orajetDataTable);
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                orajetDataTable = null;
            }
            return orajetDataTable;
        }

        //CIT Common method for Filledduct button *end

        //Get Favorites list for the user - START
        public List<FavoritesData> GetFavoritesList(string LANID)
        {
            List<FavoritesData> GetFavoritesResult = new List<FavoritesData>();
            try
            {
                DataSet oraDataSetFavorites = new DataSet();
                oraDataSetFavorites = getFavoritesDataset(LANID);
                if (oraDataSetFavorites != null && oraDataSetFavorites.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetFavorites.Tables[0].Rows)
                    {
                        FavoritesData objFavoritesList = null;
                        objFavoritesList = getFavoritesRow(dr, oraDataSetFavorites.Tables[0]);

                        if (objFavoritesList != null)
                            GetFavoritesResult.Add(objFavoritesList);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetFavoritesList method: " + ex.Message);
            }
            return GetFavoritesResult;
        }

        private DataSet getFavoritesDataset(string LANID)
        {
            DataSet oraFavDataSet = new DataSet();
            OracleCommand oraCommand = null;
            OracleConnection oraConnection = null;
            string strsql = string.Empty;

            try
            {
                strsql = "select OBJECTID, LAYERVISIBILITIES, STOREDVIEW, NAME, DEFAULTYN, USERNAME from WEBR.PGE_BK_FAVORITE where UPPER(USERNAME)= '" + LANID.ToUpper() + "'";
                oraConnection = new OracleConnection(strWipConnetionString);
                oraCommand = new OracleCommand(strsql, oraConnection);
                oraCommand.CommandTimeout = 60000;
                OracleDataAdapter oraDataAdapter = new OracleDataAdapter();
                oraDataAdapter.SelectCommand = oraCommand;
                oraConnection.Open();
                oraDataAdapter.Fill(oraFavDataSet);
                oraConnection.Close();

            }
            catch (Exception ex)
            {
                oraFavDataSet = null;
                oraConnection.Close();
                oraCommand.Dispose();
                Console.WriteLine("getFavoritesDataset method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            return oraFavDataSet;
        }

        private FavoritesData getFavoritesRow(DataRow dr, DataTable pTable)
        {
            try
            {
                FavoritesData objFavoritesRecord = new FavoritesData();

                objFavoritesRecord.ObjectId = Convert.ToString(dr["OBJECTID"]);
                objFavoritesRecord.LayerVisibilities = Convert.ToString(dr["LAYERVISIBILITIES"]);
                objFavoritesRecord.StoredView = Convert.ToString(dr["STOREDVIEW"]);
                objFavoritesRecord.Name = Convert.ToString(dr["NAME"]);
                objFavoritesRecord.DefaultYN = Convert.ToString(dr["DEFAULTYN"]);
                objFavoritesRecord.UserName = Convert.ToString(dr["USERNAME"]);

                return objFavoritesRecord;
            }
            catch (Exception ex)
            {
                Console.WriteLine("getFavoritesRow method: " + ex.Message);
                return null;
            }

        }
        //Get Favorites list for the user - END

        //Delete a favorites based on object id - START
        public FavoritesSuccessData DeleteFavorites(int objectId, string lanId)
        {
            bool isSuccess = false;
            OracleCommand oraCommand = null;
            OracleConnection oraConnection = null;
            string strsql = string.Empty;
            List<FavoritesData> favoritesData = new List<FavoritesData>();
            try
            {
                strsql = "delete WEBR.PGE_BK_FAVORITE where OBJECTID='" + objectId + "'";
                oraConnection = new OracleConnection(strWipConnetionString);
                oraCommand = new OracleCommand(strsql, oraConnection);
                oraCommand.CommandTimeout = 60000;
                oraConnection.Open();
                int rowCount = oraCommand.ExecuteNonQuery();
                oraConnection.Close();
                if (rowCount > 0)
                    isSuccess = true;

                favoritesData = GetFavoritesList(lanId);
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Console.WriteLine("DeleteFavorites method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            FavoritesSuccessData output = new FavoritesSuccessData();
            output.isSuccess = isSuccess;
            output.FavoritesList = favoritesData;
            return output;
        }
        //Delete a favorites based on object id - END

        //Add or Edit Favorites - START
        public FavoritesSuccessData AddEditFavorites(string insertOrUpdate, string user, string name, string storedView, string layerVisibilities)
        {
            bool isSuccess = false;
            DataSet oraDataSet = new DataSet();
            OracleCommand oraCommand = new OracleCommand();
            OracleConnection oraConnection = null;
            OracleDataAdapter da = new OracleDataAdapter();
            string strsql = string.Empty;
            List<FavoritesData> favoritesData = new List<FavoritesData>();
            try
            {
                oraConnection = new OracleConnection(strWipConnetionString);

                //Adding definition for parameter one
                OracleParameter input1 = new OracleParameter();
                input1.ParameterName = "P_INSERTORUPDATE";
                input1.OracleType = OracleType.VarChar;
                input1.Direction = ParameterDirection.Input;
                input1.Value = insertOrUpdate;
                input1.Size = 100;

                //Adding definition for parameter two
                OracleParameter input2 = new OracleParameter();
                input2.ParameterName = "P_USERNAME";
                input2.OracleType = OracleType.VarChar;
                input2.Direction = ParameterDirection.Input;
                input2.Value = user;
                input2.Size = 100;

                //Adding definition for parameter three
                OracleParameter input3 = new OracleParameter();
                input3.ParameterName = "P_NAME";
                input3.OracleType = OracleType.VarChar;
                input3.Direction = ParameterDirection.Input;
                input3.Value = name;
                input3.Size = 100;

                //Adding definition for parameter four
                OracleParameter input4 = new OracleParameter();
                input4.ParameterName = "P_STOREDVIEW";
                input4.OracleType = OracleType.VarChar;
                input4.Direction = ParameterDirection.Input;
                input4.Value = storedView;
                input4.Size = 100;

                //Adding definition for parameter five
                OracleParameter input5 = new OracleParameter();
                input5.ParameterName = "P_LAYERVISIBILITIES";
                input5.OracleType = OracleType.Clob;
                input5.Direction = ParameterDirection.Input;
                input5.Value = layerVisibilities;

                OracleParameter output1 = new OracleParameter();
                output1.ParameterName = "P_SUCCESS";
                output1.OracleType = OracleType.Number;
                output1.Direction = ParameterDirection.Output;
                output1.Size = 10;
                output1.Value = 0;

                OracleParameter output2 = new OracleParameter();
                output2.ParameterName = "P_FAVORITES_LIST";
                output2.OracleType = OracleType.Cursor;
                output2.Direction = ParameterDirection.Output;

                oraCommand.Parameters.Add(input1);
                oraCommand.Parameters.Add(input2);
                oraCommand.Parameters.Add(input3);
                oraCommand.Parameters.Add(input4);
                oraCommand.Parameters.Add(input5);
                oraCommand.Parameters.Add(output1);
                oraCommand.Parameters.Add(output2);

                oraCommand.Connection = oraConnection;
                oraCommand.CommandType = CommandType.StoredProcedure;
                oraCommand.CommandText = strFavInsertUpdateSPName;
                oraCommand.CommandTimeout = 600;

                oraConnection.Open();
                da = new OracleDataAdapter(oraCommand);
                da.Fill(oraDataSet);

                if (Convert.ToInt16(output1.Value) == 1)
                    isSuccess = true;

                //reload favorites list
                if (oraDataSet != null && oraDataSet.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSet.Tables[0].Rows)
                    {
                        FavoritesData objFavoritesRow = null;
                        objFavoritesRow = getFavoritesRow(dr, oraDataSet.Tables[0]);

                        if (objFavoritesRow != null)
                            favoritesData.Add(objFavoritesRow);
                    }
                }
            }
            catch (Exception ex)
            {
                isSuccess = false;
                Console.WriteLine("AddEditFavorites method: " + ex.Message);
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            FavoritesSuccessData output = new FavoritesSuccessData();
            output.isSuccess = isSuccess;
            output.FavoritesList = favoritesData;
            return output;
        }
        //Add or Edit Favorites - END
        /****************************ENOS2SAP PhaseIII Start****************************/
        List<InverterNameplateDetails> InverterNPList = new List<InverterNameplateDetails>();
        public List<InverterNameplateDetails> getInverterNameplateFromSettings(string sapEGINotification)
        {
            string inverterNamplate = string.Empty;
            DataTable dt = new DataTable();
            if (sapEGINotification.Split(',').Length <= 999)
            {

                if (!string.IsNullOrEmpty(sapEGINotification))
                {

                    OracleCommand command = new OracleCommand();
                    // OracleDataAdapter da = null;
                    OracleConnection connection = new OracleConnection();

                    string strsql = string.Empty;

                    try
                    {
                        //string connectionString = GetJetConnectionString();


                        strsql = "select sap_egi_notification SAPEGINOTIFICATION, sum(nameplate_rating * quantity) NAMELPLATERATING from sm_generator where sap_egi_notification in (" + sapEGINotification + ") group by sap_egi_notification";

                        OracleConnection sqlConnection = new OracleConnection(strSettingsConnectionString);
                        OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                        sqlCommand.CommandTimeout = 60000;
                        OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                        sqlDataAdapter.SelectCommand = sqlCommand;
                        sqlConnection.Open();

                        sqlDataAdapter.Fill(dt);
                        sqlConnection.Close();
                        foreach (DataRow row in dt.Rows)
                        {
                            InverterNameplateDetails objInverterNP = new InverterNameplateDetails();
                            objInverterNP.ProjectReference = row["SAPEGINOTIFICATION"].ToString();
                            if (!string.IsNullOrEmpty(Convert.ToString(row["NAMELPLATERATING"]))) //ENOS Tariff Change- Bug Fix
                            {
                                objInverterNP.GenSize = Convert.ToDouble(row["NAMELPLATERATING"]);
                            }
                            else
                            {
                                objInverterNP.GenSize = 0;    //ENOS Tariff Change- Bug Fix
                            }
                            if (objInverterNP != null)
                                InverterNPList.Add(objInverterNP);
                        }

                    }
                    catch (Exception ex)
                    {
                        dt = null;
                        connection.Close();
                        command.Dispose();
                        return null;
                    }

                    finally
                    {
                        connection.Close();
                        command.Dispose();
                    }
                }
                else
                {
                    inverterNamplate = null;
                }
            }
            else
            {
                string[] ArraysapEGINotification = sapEGINotification.Split(',');
                List<string> whereInStringList = new List<string>();
                StringBuilder whereInStringBuilder = new StringBuilder();
                for (int item = 0; item < ArraysapEGINotification.Length; item++)
                {
                    if (item == ArraysapEGINotification.Length - 1 || (item != 0 && (item % 998) == 0))
                    {
                        whereInStringBuilder.Append(ArraysapEGINotification[item]);
                    }
                    else
                    {
                        whereInStringBuilder.Append(ArraysapEGINotification[item] + ",");
                    }

                    if ((item % 998) == 0 && item != 0)
                    {
                        whereInStringList.Add(whereInStringBuilder.ToString());
                        whereInStringBuilder = new StringBuilder();
                    }
                }
                if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                {
                    whereInStringList.Add(whereInStringBuilder.ToString());
                }
                foreach (string whereInString in whereInStringList)
                {
                    getInverterNameplateFromSettings(whereInString);
                }

            }
            return InverterNPList;

        }
        /****************************ENOS2SAP PhaseIII End****************************/

        /**************************** ENOS Tariff Change - Start ***************************/
        List<DataToExportDetails> DataToExportList = new List<DataToExportDetails>();
        public List<DataToExportDetails> getDataToExportFromSettings(string sapEGINotification)
        {
            string inverterNamplate = string.Empty;
            DataTable dt = new DataTable();
            if (sapEGINotification.Split(',').Length <= 999)
            {

                if (!string.IsNullOrEmpty(sapEGINotification))
                {

                    OracleCommand command = new OracleCommand();
                    // OracleDataAdapter da = null;
                    OracleConnection connection = new OracleConnection();

                    string strsql = string.Empty;

                    try
                    {
                        //string connectionString = GetJetConnectionString();                      

                        strsql = "select a.sap_egi_notification,a.Eff_Rating_Kw,(a.Nameplate_rating*a.quantity) Nameplate,DECODE(a.PROGRAM_TYPE,1,'Export',3,'RESBCT',5,'EXPNEM',6,'NEMFC',7,'NEM Other Renewable',8,'NEMMT',9,'VNEM',10,'VNEMMASH',11,'VNEMMASH Dev',12,'Non-Export',13,'SNEM',14,'NEM-BIO',15,'NEMCCSF',16,'E-NET',17,'DO NOT USE',18,'NEMCDCR',19,'NEM MIL',20,'Make Before Break',21,'Inadvertent Export',22,'Continuous Uncompensated Export',23,'SNEM Paired Storage',24,'NEM Paired Storage',25,'Break Before Make',26,'NEM2SOMAH') PROGRAM_TYPE,DECODE(a.EXPORT_TO_GRID,'Y','Yes','N','No') EXPORT_TO_GRID, a.GEN_TECH_CD,a.TECH_TYPE_CD, a.SAP_EQUIPMENT_ID,DECODE(a.Derated,'Y','Yes','N','No') DERATED,DECODE(a.METHOD_OF_BACKUP,1,'Relay',2,'ATS',3,'Certified Inverter') METHOD_OF_BACKUP, a.STANDBYGEN,  b.SAP_EQUIPMENT_ID EQUIP_SAP_EQUIPMENT_ID,b.GEN_TECH_CD EQUIP_GEN_TECH from SM_GENERATOR a inner join  SM_GEN_EQUIPMENT b  on a.id =b.Generator_id and a.sap_egi_notification in (" + sapEGINotification + ") UNION select a.sap_egi_notification,a.Eff_Rating_Kw,(a.Nameplate_rating*a.quantity) Nameplate,DECODE(a.PROGRAM_TYPE,1,'Export',3,'RESBCT',5,'EXPNEM',6,'NEMFC',7,'NEM Other Renewable',8,'NEMMT',9,'VNEM',10,'VNEMMASH',11,'VNEMMASH Dev',12,'Non-Export',13,'SNEM',14,'NEM-BIO',15,'NEMCCSF',16,'E-NET',17,'DO NOT USE',18,'NEMCDCR',19,'NEM MIL',20,'Make Before Break',21,'Inadvertent Export',22,'Continuous Uncompensated Export',23,'SNEM Paired Storage',24,'NEM Paired Storage',25,'Break Before Make',26,'NEM2SOMAH') PROGRAM_TYPE,DECODE(a.EXPORT_TO_GRID,'Y','Yes','N','No') EXPORT_TO_GRID, a.GEN_TECH_CD,a.TECH_TYPE_CD, a.SAP_EQUIPMENT_ID,DECODE(a.Derated,'Y','Yes','N','No') DERATED,DECODE(a.METHOD_OF_BACKUP,1,'Relay',2,'ATS',3,'Certified Inverter') METHOD_OF_BACKUP,a.STANDBYGEN, null as EQUIP_SAP_EQUIPMENT_ID,null as Equip_Gen_Tech from sm_generator a where id not in (select generator_id from sm_gen_equipment) and sap_egi_notification in (" + sapEGINotification + ")";

                        OracleConnection sqlConnection = new OracleConnection(strSettingsConnectionString);
                        OracleCommand sqlCommand = new OracleCommand(strsql, sqlConnection);
                        sqlCommand.CommandTimeout = 60000;
                        OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                        sqlDataAdapter.SelectCommand = sqlCommand;
                        sqlConnection.Open();

                        sqlDataAdapter.Fill(dt);
                        sqlConnection.Close();
                        foreach (DataRow row in dt.Rows)
                        {
                            DataToExportDetails objDataToExport = new DataToExportDetails();

                            //objDataToExport.Address = null;// DataToExportDetailsResult[i].Address;
                            //objDataToExport.METERNUMBER = DataToExportDetailsResult[i].METERNUMBER;
                            //objDataToExport.SPID = DataToExportDetailsResult[i].SPID;
                            //objDataToExport.CGC12 = DataToExportDetailsResult[i].CGC12;
                            if (!string.IsNullOrEmpty(Convert.ToString(row["Eff_Rating_Kw"])))
                            {
                                objDataToExport.GenSize = Convert.ToDouble(row["Eff_Rating_Kw"]);
                            }
                            else
                            {
                                objDataToExport.GenSize = 0;
                            }
                            //objDataToExport.GenSize = Convert.ToDouble(row["Eff_Rating_Kw"]);
                            if (!string.IsNullOrEmpty(Convert.ToString(row["Nameplate"])))
                            {
                                objDataToExport.Nameplate = Convert.ToDouble(row["Nameplate"]);
                            }
                            else
                            {
                                objDataToExport.Nameplate = 0;
                            }
                            //objDataToExport.Nameplate = Convert.ToDouble(row["Nameplate"]);
                            //objDataToExport.ProjectReference = DataToExportDetailsResult[i].ProjectReference;
                            //objDataToExport.FEEDERNUM = DataToExportDetailsResult[i].FEEDERNUM;
                            objDataToExport.ProjectReference = row["sap_egi_notification"].ToString();
                            objDataToExport.ProgramType = row["PROGRAM_TYPE"].ToString();
                            objDataToExport.ExportToGrid = row["EXPORT_TO_GRID"].ToString();
                            objDataToExport.GenType = row["GEN_TECH_CD"].ToString();
                            objDataToExport.TechType = row["TECH_TYPE_CD"].ToString();
                            objDataToExport.EquipmentType = row["EQUIP_GEN_TECH"].ToString();
                            objDataToExport.SAPEquipmentID = row["SAP_EQUIPMENT_ID"].ToString();
                            objDataToExport.EquipSAPEquipmentID = row["EQUIP_SAP_EQUIPMENT_ID"].ToString();
                            objDataToExport.Derated = row["DERATED"].ToString();
                            //objDataToExport.LimitedExport = row["METHOD_OF_BACKUP"].ToString();
                            objDataToExport.StandByGen = row["STANDBYGEN"].ToString();
                            if (objDataToExport != null)
                                DataToExportList.Add(objDataToExport);

                        }


                    }

                    catch (Exception ex)
                    {
                        dt = null;
                        connection.Close();
                        command.Dispose();
                        return null;
                    }

                    finally
                    {
                        connection.Close();
                        command.Dispose();
                    }
                }
                else
                {
                    inverterNamplate = null;
                }
            }
            else
            {
                string[] ArraysapEGINotification = sapEGINotification.Split(',');
                List<string> whereInStringList = new List<string>();
                StringBuilder whereInStringBuilder = new StringBuilder();
                for (int item = 0; item < ArraysapEGINotification.Length; item++)
                {
                    if (item == ArraysapEGINotification.Length - 1 || (item != 0 && (item % 998) == 0))
                    {
                        whereInStringBuilder.Append(ArraysapEGINotification[item]);
                    }
                    else
                    {
                        whereInStringBuilder.Append(ArraysapEGINotification[item] + ",");
                    }

                    if ((item % 998) == 0 && item != 0)
                    {
                        whereInStringList.Add(whereInStringBuilder.ToString());
                        whereInStringBuilder = new StringBuilder();
                    }
                }
                if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                {
                    whereInStringList.Add(whereInStringBuilder.ToString());
                }
                foreach (string whereInString in whereInStringList)
                {
                    getDataToExportFromSettings(whereInString);
                }

            }
            return DataToExportList;

        }
        /**************************** ENOS Tariff Change - End ***************************/

        //ME Q3 2019 Release - DA# 190501 ----START
        public List<LoadingInfoCustomerData> GetLoadingInfoCustData(string strCGC)
        {
            string resultJson = "";
            List<LoadingInfoCustomerData> CustomerResult = new List<LoadingInfoCustomerData>();
            DataSet oraDataSetCGC = new DataSet();
            DataSet oraDataSetCGC1 = new DataSet();
            DataSet oraDataSetCGC2 = new DataSet();
            try
            {
                if (strCGC != "")
                {
                    //Add code for cgc number more than 999
                    char[] delimiter1 = new char[] { ',' };

                    string[] Totalcgcno = strCGC.Split(delimiter1, StringSplitOptions.None);
                    int spcount = Totalcgcno.Count();

                    if (spcount > 995)
                    {
                        while (spcount > 0)
                        {
                            string[] firstgcsds = Totalcgcno.Take(995).Select(i => i.ToString()).ToArray();
                            string Spval = cgcsplit(firstgcsds);
                            oraDataSetCGC.Merge(getLoadingInfoCustomer_CGCNumber(Spval));
                            spcount = spcount - 995;
                            Totalcgcno = Totalcgcno.Skip(995).Select(i => i.ToString()).ToArray(); 
                        };
                        
                    }

                    else
                    {
                        oraDataSetCGC = getLoadingInfoCustomer_CGCNumber(strCGC);
                    }
                }

                if (oraDataSetCGC != null && oraDataSetCGC.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetCGC.Tables[0].Rows)
                    {
                        LoadingInfoCustomerData objCustdataLoadInfoRow = null;
                        if (!CustomerResult.Any(c => c.GLOBALID == Convert.ToString(dr["SERVIEPOINTGLOBALID"])))
                        {
                            objCustdataLoadInfoRow = getLoadInfoCustomerRow(dr, oraDataSetCGC.Tables[0]);

                            if (objCustdataLoadInfoRow != null)
                                CustomerResult.Add(objCustdataLoadInfoRow);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                throw;
            }
            finally
            {

            }
            return CustomerResult;
        }

        public DataSet getLoadingInfoCustomer_CGCNumber(string cgcNumber)
        {
            DataSet oraCGCDataSet = new DataSet();
            OracleCommand comm = new OracleCommand();
            OracleDataAdapter da = null;
            OracleConnection conn = null;
            try
            {
                conn = new OracleConnection(strConnectionString);
                //Adding definition for parameter one
                OracleParameter output1 = new OracleParameter();
                output1.ParameterName = "P_INPUTSTRING";
                output1.OracleType = OracleType.VarChar;
                output1.Direction = ParameterDirection.Input;
                output1.Value = cgcNumber;

                //Adding definition for parameter two
                OracleParameter output2 = new OracleParameter();
                output2.ParameterName = "P_VERSION";
                output2.OracleType = OracleType.VarChar;
                output2.Direction = ParameterDirection.Input;
                output2.Value = "";

                //Adding definition for parameter four
                OracleParameter output3 = new OracleParameter();
                output3.ParameterName = "P_CURSOR";
                output3.OracleType = OracleType.Cursor;
                output3.Direction = ParameterDirection.Output;

                //Adding definition for parameter five
                OracleParameter output4 = new OracleParameter();
                output4.ParameterName = "P_ERROR";
                output4.OracleType = OracleType.VarChar;
                output4.Direction = ParameterDirection.Output;
                output4.Size = 40000;

                //Adding definition for parameter six
                OracleParameter output5 = new OracleParameter();
                output5.ParameterName = "P_SUCCESS";
                output5.OracleType = OracleType.Number;
                output5.Direction = ParameterDirection.Output;
                output5.Size = 40000;

                comm.Parameters.Add(output1);
                comm.Parameters.Add(output2);
                comm.Parameters.Add(output3);
                comm.Parameters.Add(output4);
                comm.Parameters.Add(output5);
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = strLoadingInfoCustDataSPName;
                comm.CommandTimeout = 600;
                conn.Open();
                da = new OracleDataAdapter(comm);
                da.Fill(oraCGCDataSet);
                int i = oraCGCDataSet.Tables[0].Rows.Count;
            }
            catch (Exception ex)
            {
                oraCGCDataSet = null;
                conn.Close();
                comm.Dispose();
            }
            finally
            {
                conn.Close();
                comm.Dispose();
            }
            return oraCGCDataSet;
        }

        private LoadingInfoCustomerData getLoadInfoCustomerRow(DataRow dr, DataTable pTable)
        {
            try
            {
                LoadingInfoCustomerData objLoadingInfoCustRow = new LoadingInfoCustomerData();

                objLoadingInfoCustRow.GLOBALID = Convert.ToString(dr["SERVIEPOINTGLOBALID"]);
                objLoadingInfoCustRow.CUSTOMERTYPE = Convert.ToString(dr["CUSTOMERTYPE"]);

                return objLoadingInfoCustRow;
            }
            catch (Exception)
            {
                return null;
                //throw;
            }

        }
        //ME Q3 2019 Release - DA# 190501 ----END

        //DA# 190506 - WEBR Access Management - ME Q3 2019 Release
        public bool StoreUserDetails(string LANID, string machineName)
        {
            string dt;
            bool result = false;
            DataSet oraDataSet = new DataSet();
            OracleCommand oraCommand = new OracleCommand();
            OracleConnection oraConnection = new OracleConnection();
            string strsql = string.Empty;
            DateTime t1 = System.DateTime.Now;
            dt = t1.Year.ToString() + "/" + t1.Month.ToString() + "/" + t1.Day + " " + t1.Hour.ToString() + ":" + t1.Minute.ToString() + ":" + t1.Second.ToString();

            try
            {
                strsql = "insert into " + webrLoginData_TableName + " (LANID, LOGIN_DATETIME, MACHINE_NAME) VALUES ('" + LANID +
                    "',TO_DATE('" + dt + "', 'yyyy/mm/dd hh24:mi:ss'),'" + machineName + "')";
                oraConnection = new OracleConnection(strWipConnetionString);
                oraCommand = new OracleCommand(strsql, oraConnection);
                oraCommand.CommandTimeout = 60000;
                OracleDataAdapter oraDataAdapter = new OracleDataAdapter();
                oraDataAdapter.SelectCommand = oraCommand;
                oraConnection.Open();

                oraDataAdapter.Fill(oraDataSet);
                oraConnection.Close();
                result = true;
            }
            catch
            {
                result = false;
            }
            finally
            {
                oraConnection.Close();
                oraCommand.Dispose();
            }
            return result;
        }
    }

}




