using System;
using System.ServiceModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.ServiceModel.Activation;
using System.IO;
using System.ComponentModel;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using ArcFMSilverlight.Web;
using System.Collections;
using ArcFMSilverlight;

namespace ArcFMSilverlight.Web.Services
{
    /// <summary>
    /// Summary description for PONSWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PONSWebService : System.Web.Services.WebService
    {
        string strConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        //string strConnectionStringEDER = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringEder"].ConnectionString;
        string strPSLConnetionString = System.Configuration.ConfigurationManager.ConnectionStrings["PSLConnectionString"].ConnectionString;
        string strContactList = System.Configuration.ConfigurationManager.AppSettings["PSLContacts"];
        //Adding function to make values configurable[Start]
        string strCGCINCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CGC_INC_SPName"];
        string strCGCEXCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CGC_EXC_SPName"];
        string strSERVPINCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["SERVP_INC_SPName"];
        string strSERVPEXCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["SERVP_EXC_SPName"];
        string strCIRCTINCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CIRCUITP_INC_SPName"];
        string strCIRCTEXCPROCSPName = System.Configuration.ConfigurationManager.AppSettings["CIRCUITP_EXC_SPName"];
        string strSEQGenPROCName = System.Configuration.ConfigurationManager.AppSettings["SEQSP_SPName"];
        string strSubankCircuitPROCSPName = System.Configuration.ConfigurationManager.AppSettings["SUBBANKCIRCUITPSPName"];
        //Adding function to make values configurable[End]
        string _PrintDeleteDate = System.Configuration.ConfigurationManager.AppSettings["PrintDeleteDate"];

        //DA# 190904 - ME Q1 2020
        string strSettingsConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SettingsConnectionString"].ConnectionString;

        private string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        }
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
        public DataSet getSubSbankCircuitID(string strDivisionCode)
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
                OracleParameter input1 = new OracleParameter();
                input1.ParameterName = "P_DIVISION";
                input1.OracleType = OracleType.VarChar;
                input1.Direction = ParameterDirection.Input;
                input1.Value = DivisionCode;
                input1.Size = 2000;
                //Adding definition for parameter three
                OracleParameter output1 = new OracleParameter();
                output1.ParameterName = "P_CURSOR";
                output1.OracleType = OracleType.Cursor;
                output1.Direction = ParameterDirection.Output;

                OracleParameter output2 = new OracleParameter();
                output2.ParameterName = "P_ERROR";
                output2.OracleType = OracleType.VarChar;
                output2.Direction = ParameterDirection.Output;
                output2.Size = 2000;
                OracleParameter output3 = new OracleParameter();
                output3.ParameterName = "P_SUCCESS";
                output3.OracleType = OracleType.Number;
                output3.Direction = ParameterDirection.Output;
                output3.Size = 2000;
                comm.Parameters.Add(input1);
                comm.Parameters.Add(output1);
                comm.Parameters.Add(output2);
                comm.Parameters.Add(output3);

                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = strSubankCircuitPROCSPName;
                comm.CommandTimeout = 600;
                conn.Open();
                da = new OracleDataAdapter(comm);
                da.Fill(oraDataSet);


                for (int iCount_Row = 0; iCount_Row < oraDataSet.Tables[0].Rows.Count; ++iCount_Row)
                {
                    try
                    {
                        oraDataSet.Tables[0].Rows[iCount_Row]["TX_BANK_CODE"] = Convert.ToString(oraDataSet.Tables[0].Rows[iCount_Row]["TX_BANK_CODE"]).Contains("BK") ?
                          "Bank " + Convert.ToString(oraDataSet.Tables[0].Rows[iCount_Row]["TX_BANK_CODE"]).Split(new string[] { "BK" }, StringSplitOptions.None)[1] :
                            Convert.ToString(oraDataSet.Tables[0].Rows[iCount_Row]["TX_BANK_CODE"]);
                    }
                    catch { }
                }
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
                comm.Parameters.Add(output3);
                comm.Parameters.Add(output5);
                comm.Parameters.Add(output6);
                comm.Parameters.Add(output7);
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                if (strFlag == "0")
                {
                    comm.CommandText = strCIRCTINCPROCSPName;//"PONS.CUST_INFO_CIRCUITID_INCL";
                }
                else
                {
                    comm.CommandText = strCIRCTEXCPROCSPName;//"PONS.CUST_INFO_CIRCUITID_EXCL";
                }
                comm.CommandTimeout = 600;
                conn.Open();
                da = new OracleDataAdapter(comm);
                da.Fill(oraDataSet);

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
                comm.Parameters.Add(OradivNum);
                comm.Parameters.Add(output4);
                comm.Parameters.Add(output5);
                comm.Parameters.Add(output6);
                comm.Connection = conn;
                comm.CommandType = CommandType.StoredProcedure;
                if (strFlag == "0")
                {
                    comm.CommandText = strCGCINCPROCSPName;//"PONS.CUST_INFO_CGC_INCL_T";
                }
                else
                {
                    comm.CommandText = strCGCEXCPROCSPName;//"PONS.CUST_INFO_CGC_EXCL_T";
                }
                comm.CommandTimeout = 600;
                conn.Open();
                da = new OracleDataAdapter(comm);
                da.Fill(oraCGCDataSet);
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

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetBank(string strSubstationName)
        {
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            string strBankID = string.Empty;
            List<SubStationSearchData> CustomerResult = new List<SubStationSearchData>();
            try
            {
                strBankID = "Select distinct TX_BANK_CODE from EDGIS.PGE_CIRCUITSOURCE where substationname = " + "'" + strSubstationName + "'" + " and TX_BANK_CODE is not null";
                OracleConnection sqlConnection = new OracleConnection(strConnectionString);
                OracleCommand sqlCommand = new OracleCommand(strBankID, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(objSet);
                sqlConnection.Close();

                SubStationSearchData objcustomerlist;

                if (objSet.Tables.Count > 0)
                {
                    DataTable dt = objSet.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        objcustomerlist = new SubStationSearchData();
                        //objcustomerlist.SubStationID = dr["STATIONNUMBER"].ToString();
                        //objcustomerlist.SubStationName = dr["SUBSTATIONNAME"].ToString();
                        objcustomerlist.BANK = dr["TX_BANK_CODE"].ToString();
                        //objcustomerlist.CircuitID = dr["TO_CIRCUITID"].ToString();
                        CustomerResult.Add(objcustomerlist);
                        resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                    }
                }
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetCircuit(string strBankName)
        {
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            string strCircuitID = string.Empty;
            List<SubStationSearchData> CustomerResult = new List<SubStationSearchData>();
            try
            {
                strCircuitID = "Select distinct TO_CIRCUITID from EDGIS.PGE_CIRCUITSOURCE where TX_BANK_CODE =  " + "'" + strBankName + "'" + " and TO_CIRCUITID is not null";
                OracleConnection sqlConnection = new OracleConnection(strConnectionString);
                OracleCommand sqlCommand = new OracleCommand(strBankName, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(objSet);
                sqlConnection.Close();

                SubStationSearchData objcustomerlist;

                if (objSet.Tables.Count > 0)
                {
                    DataTable dt = objSet.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        objcustomerlist = new SubStationSearchData();
                        //objcustomerlist.SubStationID = dr["STATIONNUMBER"].ToString();
                        //objcustomerlist.SubStationName = dr["SUBSTATIONNAME"].ToString();
                        //objcustomerlist.BANK = dr["TX_BANK_CIRCUITID"].ToString();
                        objcustomerlist.CircuitID = dr["TO_CIRCUITID"].ToString();
                        CustomerResult.Add(objcustomerlist);
                        resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                    }
                }
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetDevicedataCustomerSearch(string strDevice1, string strDevice2, int divisioncode)
        {
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            string devicesql = string.Empty;
            List<Devicestartcustomerdata> CustomerResult = new List<Devicestartcustomerdata>();
            if (strDevice1.Contains('^'))
            {
                strDevice1 = strDevice1.Replace('^', '#');
            }
            if (strDevice2.Contains('#'))
            {
                strDevice2 = strDevice2.Replace('^', '#');
            }
            try
            {
                devicesql = "Select 'Switch' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid,circuitid2, division from  edgis.zz_mv_Switch where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + divisioncode + "' UNION Select 'Fuse' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid,circuitid2, division from edgis.zz_mv_Fuse where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + divisioncode + "' UNION Select  'OpenPoint' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid,circuitid2, division from edgis.zz_mv_OpenPoint where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + divisioncode + "' UNION Select  'Transformer' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid,circuitid2, division from edgis.zz_mv_Transformer  where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + divisioncode + "' Union Select 'PriUGConductor' as DeviceName, TO_CHAR(objectid)  as OPERATINGNUMBER, objectid, globalid, circuitid,circuitid2, division from  edgis.zz_mv_PriUGConductor  where TO_CHAR(objectid) IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + divisioncode + "' union Select 'PrimaryOverheadConductor' as DeviceName, TO_CHAR(objectid)  as OPERATINGNUMBER, objectid, globalid, circuitid,circuitid2, division from  edgis.zz_mv_PriOHConductor where TO_CHAR(objectid) IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + divisioncode + "' Union Select 'Dynamicprotectivedevice' as DeviceName, to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid,circuitid2, division from  edgis.zz_mv_dynamicprotectivedevice where OPERATINGNUMBER IN('" + strDevice1 + "','" + strDevice2 + "'" + ") and Division='" + divisioncode + "'";

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
                        objcustomerlist.circuitid2 = dr[5].ToString();
                        CustomerResult.Add(objcustomerlist);
                        resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                    }
                }
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetDevicestartcusdata(int devisionnumber)
        {
            string devicesql = string.Empty;
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            try
            {
                devicesql = "Select 'Switch' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.Switch where  Division='" + devisionnumber + "' UNION Select  'Fuse' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Fuse where Division='" + devisionnumber + "' UNION Select  'OpenPoint' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.OpenPoint where Division='" + devisionnumber + "' UNION Select  'OpenPoint' as DeviceName,to_char(OPERATINGNUMBER) as OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Transformer  where Division='" + devisionnumber + "' Union Select 'PriUGConductor' as DeviceName, TO_CHAR(objectid)  as OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.PriUGConductor where  Division='" + devisionnumber + "' UNION Select 'PrimaryOverheadConductor' as DeviceName, TO_CHAR(objectid)  as OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.PriOHConductor  where  Division='" + devisionnumber + "'";
                OracleConnection sqlConnection = new OracleConnection(strConnectionString);
                OracleCommand sqlCommand = new OracleCommand(devicesql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(objSet);
                sqlConnection.Close();
                List<Devicestartcustomerdata> CustomerResult = new List<Devicestartcustomerdata>();
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
                        resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                    }
                }
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetDevicecusdata(string operationnumber, int dvisionnumber)
        {
            string devicesql = string.Empty;
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            try
            {
                devicesql = "Select 'Switch' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.Switch where OPERATINGNUMBER='" + operationnumber + "' and Division='" + dvisionnumber + "' UNION Select  'Fuse' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Fuse where OPERATINGNUMBER='" + operationnumber + "' and Division='" + dvisionnumber + "' UNION Select  'OpenPoint' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.OpenPoint where OPERATINGNUMBER='" + operationnumber + "' and Division='" + dvisionnumber + "' UNION Select  'OpenPoint' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Transformer  where OPERATINGNUMBER='" + operationnumber + "' and Division='" + dvisionnumber + "' Union Select  'CapacitorBank' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.CapacitorBank   where OPERATINGNUMBER='" + operationnumber + "' and Division='" + dvisionnumber + "'";
                //devicesql = "Select 'Switch' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from  edgis.Switch where  Division='" + Devisionnumber + "' UNION Select  'Fuse' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Fuse where Division='" + Devisionnumber + "' UNION Select  'OpenPoint' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.OpenPoint where Division='" + Devisionnumber + "' UNION Select  'OpenPoint' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.Transformer  where Division='" + Devisionnumber + "' Union Select  'CapacitorBank' as DeviceName,OPERATINGNUMBER, objectid, globalid, circuitid, division from edgis.CapacitorBank   where Division='" + Devisionnumber + "'";

                OracleConnection sqlConnection = new OracleConnection(strConnectionString);
                OracleCommand sqlCommand = new OracleCommand(devicesql, sqlConnection);
                sqlCommand.CommandTimeout = 60000;
                OracleDataAdapter sqlDataAdapter = new OracleDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Open();
                DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(objSet);
                sqlConnection.Close();

                List<Devicestartcustomerdata> CustomerResult = new List<Devicestartcustomerdata>();
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
                        resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                    }
                }
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetTransformercusdataComb(string strCGSValue, string strCircuitId, string strServicePointID, string strDivisionIn, string strFlagIn)
        {
            string resultJson = "";
            string CGS = "";
            string CircuitId = "";
            string ServicePointID = "";
            CGS = strCGSValue;
            CircuitId = strCircuitId;
            ServicePointID = strServicePointID;
            HttpContext.Current.Response.ContentType = "application/json";
            List<TransformercustomerdataComb> CustomerResult = new List<TransformercustomerdataComb>();
            DataSet oraDataSetCGC = new DataSet();
            DataSet oraDataSetCicuitID = new DataSet();
            DataSet dtResults = new DataSet();
            TransformercustomerdataComb objcustomerlist;
            TransformercustomerdataComb objcustomerlistCircuit;
            TransformercustomerdataComb objcustomerlistCGC;
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
                    oraDataSetCGC = getCustomer_CGCNumber(strCGC, strFlagIn, strDivisionIn);
                }
                if (CircuitId != "")
                {
                    oraDataSetCicuitID = getCustomer_CircuitID(CircuitId, strDivisionIn, strFlagIn);
                }
                if (ServicePointID != "")
                {
                    dtResults = getCustomer_ServicePoint(ServicePointID, strFlagIn);
                }
                string strZipCode = "";
                string strMailZipCode = "";

                if (dtResults != null && dtResults.Tables.Count > 0)
                {
                    foreach (DataRow dr in dtResults.Tables[0].Rows)
                    {
                        strZipCode = dr[7].ToString();
                        strMailZipCode = dr[7].ToString();
                        if (strZipCode != "" && strZipCode.Length == 9)
                        {
                            strZipCode = strZipCode.Substring(0, 5).ToString() + "-" + strZipCode.Substring(5, 4).ToString();
                        }
                        else
                        {
                            strZipCode = dr[7].ToString();
                        }
                        if (strMailZipCode != "" && strMailZipCode.Length == 9)
                        {
                            strMailZipCode = strMailZipCode.Substring(0, 5).ToString() + "-" + strMailZipCode.Substring(5, 4).ToString();
                        }
                        else
                        {
                            strMailZipCode = dr[7].ToString();
                        }
                        objcustomerlist = new TransformercustomerdataComb();
                        objcustomerlist.CustType = dr[3].ToString();
                        objcustomerlist.CustMail1 = dr[1].ToString();
                        objcustomerlist.CustMailStNum = dr[4].ToString();
                        objcustomerlist.CustMailStName1 = dr[5].ToString();
                        objcustomerlist.CustMailStName2 = dr[6].ToString();
                        objcustomerlist.SerStNumber = dr[4].ToString();
                        objcustomerlist.SerStName1 = dr[5].ToString();
                        objcustomerlist.AverySTNameNumber = dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString();
                        objcustomerlist.GridADDCol = dr[1].ToString() + " " + dr[2].ToString() + " " + dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString() + " " + strMailZipCode;
                        objcustomerlist.AddressCust = dr[8].ToString() + " " + dr[9].ToString() + " " + strMailZipCode;
                        objcustomerlist.CustPhone = dr[10].ToString();
                        objcustomerlist.CGC12 = dr[11].ToString();
                        objcustomerlist.TransOperatingNum = dr[12].ToString();
                        objcustomerlist.TransSSD = dr[13].ToString();
                        objcustomerlist.ServicepointId = dr[0].ToString();
                        objcustomerlist.MeterNum = dr[17].ToString();
                        objcustomerlist.SerActID = dr[14].ToString();
                        objcustomerlist.SerDivision = dr[15].ToString();
                        objcustomerlist.CustMailCity = dr[8].ToString();
                        objcustomerlist.SerCity = dr[8].ToString();
                        objcustomerlist.SerState = dr[9].ToString();
                        objcustomerlist.CustMailZipCode = strMailZipCode;
                        objcustomerlist.ServiceZip = strMailZipCode;
                        objcustomerlist.MailState = dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString() + " " + strMailZipCode;
                        CustomerResult.Add(objcustomerlist);
                    }
                }
                if (oraDataSetCicuitID != null && oraDataSetCicuitID.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetCicuitID.Tables[0].Rows)
                    {
                        strMailZipCode = dr[7].ToString();
                        if (strMailZipCode != "" && strMailZipCode.Length == 9)
                        {
                            strMailZipCode = strMailZipCode.Substring(0, 5).ToString() + "-" + strMailZipCode.Substring(5, 4).ToString();
                        }
                        else
                        {
                            strMailZipCode = dr[7].ToString();
                        }
                        objcustomerlistCircuit = new TransformercustomerdataComb();
                        objcustomerlistCircuit.CustType = dr[3].ToString();
                        objcustomerlistCircuit.CustMail1 = dr[1].ToString();
                        objcustomerlistCircuit.CustMailStNum = dr[4].ToString();
                        objcustomerlistCircuit.CustMailStName1 = dr[5].ToString();
                        objcustomerlistCircuit.CustMailStName2 = dr[6].ToString();
                        objcustomerlistCircuit.SerStNumber = dr[4].ToString();
                        objcustomerlistCircuit.SerStName1 = dr[5].ToString();
                        objcustomerlistCircuit.AverySTNameNumber = dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString();
                        objcustomerlistCircuit.GridADDCol = dr[1].ToString() + " " + dr[2].ToString() + " " + dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString() + " " + strMailZipCode;
                        objcustomerlistCircuit.AddressCust = dr[8].ToString() + " " + dr[9].ToString() + " " + strMailZipCode;
                        objcustomerlistCircuit.CustPhone = dr[10].ToString();
                        objcustomerlistCircuit.CGC12 = dr[11].ToString();
                        objcustomerlistCircuit.TransOperatingNum = dr[12].ToString();
                        objcustomerlistCircuit.TransSSD = dr[13].ToString();
                        objcustomerlistCircuit.ServicepointId = dr[0].ToString();
                        objcustomerlistCircuit.MeterNum = dr[17].ToString();
                        objcustomerlistCircuit.SerActID = dr[14].ToString();
                        objcustomerlistCircuit.SerDivision = dr[15].ToString();
                        objcustomerlistCircuit.CustMailCity = dr[8].ToString();
                        objcustomerlistCircuit.SerCity = dr[8].ToString();
                        objcustomerlistCircuit.SerState = dr[9].ToString();
                        objcustomerlistCircuit.CustMailZipCode = strMailZipCode;
                        objcustomerlistCircuit.ServiceZip = strMailZipCode;
                        objcustomerlistCircuit.MailState = dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString() + " " + strMailZipCode;
                        CustomerResult.Add(objcustomerlistCircuit);
                    }
                }
                if (oraDataSetCGC != null && oraDataSetCGC.Tables.Count > 0)
                {
                    foreach (DataRow dr in oraDataSetCGC.Tables[0].Rows)
                    {
                        objcustomerlistCGC = new TransformercustomerdataComb();
                        strMailZipCode = dr[7].ToString();
                        if (strMailZipCode != "" && strMailZipCode.Length == 9)
                        {
                            strMailZipCode = strMailZipCode.Substring(0, 5).ToString() + "-" + strMailZipCode.Substring(5, 4).ToString();
                        }
                        else
                        {
                            strMailZipCode = dr[7].ToString();
                        }
                        for (int j = 0; j < strCGCSSD.Length; j++)
                        {
                            if (strCGCSSD[j].Contains(dr[8].ToString()))
                            {
                                string[] strArrCGCSSD = strCGCSSD[j].Split('^');
                                if (strArrCGCSSD[1] != null)
                                {
                                    strSSD = strArrCGCSSD[1];
                                }
                                break;
                            }
                        }
                        objcustomerlistCGC.TransSSD = strSSD;
                        objcustomerlistCGC.CustType = dr[3].ToString();
                        objcustomerlistCGC.CustMail1 = dr[1].ToString();
                        objcustomerlistCGC.CustMailStNum = dr[4].ToString();
                        objcustomerlistCGC.CustMailStName1 = dr[5].ToString();
                        objcustomerlistCGC.CustMailStName2 = dr[6].ToString();
                        objcustomerlistCGC.SerStNumber = dr[4].ToString();
                        objcustomerlistCGC.SerStName1 = dr[5].ToString();
                        objcustomerlistCGC.AverySTNameNumber = dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString();
                        objcustomerlistCGC.GridADDCol = dr[1].ToString() + " " + dr[2].ToString() + " " + dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString() + " " + strMailZipCode;
                        objcustomerlistCGC.AddressCust = dr[8].ToString() + " " + dr[9].ToString() + " " + strMailZipCode;
                        objcustomerlistCGC.CustPhone = dr[10].ToString();
                        objcustomerlistCGC.CGC12 = dr[11].ToString();
                        objcustomerlistCGC.SerActID = dr[12].ToString();
                        objcustomerlistCGC.SerDivision = dr[13].ToString();
                        objcustomerlistCGC.ServicepointId = dr[0].ToString();
                        objcustomerlistCGC.MeterNum = dr[15].ToString();
                        objcustomerlistCGC.CustMailCity = dr[8].ToString();
                        objcustomerlistCGC.SerCity = dr[8].ToString();
                        objcustomerlistCGC.SerState = dr[9].ToString();
                        objcustomerlistCGC.CustMailZipCode = strMailZipCode;
                        objcustomerlistCGC.ServiceZip = strMailZipCode;
                        objcustomerlistCGC.MailState = dr[4].ToString() + " " + dr[5].ToString() + " " + dr[6].ToString() + " " + strMailZipCode;
                        CustomerResult.Add(objcustomerlistCGC);
                    }

                }
                resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetPSCdata()
        {
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            List<PSLData> CustomerResult = new List<PSLData>();
            SqlConnection sqlConnection = new SqlConnection(strPSLConnetionString);
            try
            {
                string sql = "Select FName, LName, LANID, ContactID, Division  from " + strContactList + " where type = " + "'" + "PSC" + "'" + "and Active = 1 order by Division, FName, LName";


                SqlCommand sqlCommand = new SqlCommand(sql, sqlConnection);
                sqlCommand.CommandTimeout = 6000;
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlConnection.Close();
                sqlConnection.Open();
                DataSet objSet = new DataSet();
                sqlDataAdapter.Fill(objSet);
                sqlConnection.Close();
                PSLData objcustomerlist;

                if (objSet.Tables.Count > 0)
                {
                    DataTable dt = objSet.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        objcustomerlist = new PSLData();
                        objcustomerlist.FName = dr[0].ToString();
                        objcustomerlist.LName = dr[1].ToString();
                        objcustomerlist.LanID = dr[2].ToString();
                        objcustomerlist.ContactID = dr[3].ToString();
                        objcustomerlist.Division = dr[4].ToString();
                        CustomerResult.Add(objcustomerlist);
                        resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                    }
                }
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                sqlConnection.Close();
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                sqlConnection.Close();
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void GetSubstationBankCiercuitID(string strdivCode)
        {
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            List<SubStationSearchData> CustomerResult = new List<SubStationSearchData>();
            SqlConnection sqlConnection = new SqlConnection(strConnectionString);
            try
            {
                DataSet objSet = new DataSet();
                if (strdivCode != "")
                {
                    objSet = getSubSbankCircuitID(strdivCode);
                }
                SubStationSearchData objcustomerlist;

                if (objSet.Tables.Count > 0)
                {
                    DataTable dt = objSet.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        objcustomerlist = new SubStationSearchData();
                        objcustomerlist.SubStationID = dr["STATIONNUMBER"].ToString();
                        objcustomerlist.SubStationName = dr["SUBSTATIONNAME"].ToString();
                        objcustomerlist.BANK = dr["TX_BANK_CODE"].ToString();
                        objcustomerlist.CircuitID = dr["TO_CIRCUITID"].ToString();
                        CustomerResult.Add(objcustomerlist);
                        resultJson = new JavaScriptSerializer().Serialize(CustomerResult.ToList());
                    }
                }
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                sqlConnection.Close();
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                sqlConnection.Close();
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void genSequence()
        {
            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            ShutDownID objShutdownID = new ShutDownID();
            List<ShutDownID> ShutdownIDResult = new List<ShutDownID>();
            OracleConnection oraConnection = new OracleConnection();
            try
            {
                //System.Data.OracleClient.OracleConnection conn = new OracleConnection(strConnectionString);

                ////Adding definition for parameter one
                //System.Data.OracleClient.OracleParameter output1 = new OracleParameter();
                //output1.ParameterName = "P_SHUTDOWNID";
                //output1.OracleType = OracleType.Number;
                //output1.Direction = ParameterDirection.Output;
                //output1.Size = 40000;
                ////Adding definition for parameter two
                //OracleParameter output2 = new OracleParameter();
                //output2.ParameterName = "P_ERROR";
                //output2.OracleType = OracleType.VarChar;
                //output2.Direction = ParameterDirection.Output;
                //output2.Size = 4000;
                ////Adding definition for parameter three
                //OracleParameter output3 = new OracleParameter();
                //output3.ParameterName = "P_SUCCESS";
                //output3.OracleType = OracleType.Number;
                //output3.Direction = ParameterDirection.Output;

                //comm.Parameters.Add(output1);
                //comm.Parameters.Add(output2);
                //comm.Parameters.Add(output3);
                //comm.Connection = conn;
                //comm.CommandType = CommandType.StoredProcedure;
                //comm.CommandText = "PONS.gen_shutdownID";
                //comm.CommandTimeout = 6000;
                //conn.Open();
                //comm.ExecuteNonQuery();
                //conn.Close();


                // DA# 190904 - ME Q1 2020               
                OracleDataReader reader;
                string strCommand = string.Empty;
                strCommand = "select EDSETT.PONSSEQ_SHUTDOWN_ID.nextval from dual";
                oraConnection.ConnectionString = strSettingsConnectionString;
                OracleCommand oraCommand = new OracleCommand(strCommand, oraConnection);
                oraCommand.CommandTimeout = 60000;
                oraConnection.Open();
                reader = oraCommand.ExecuteReader();
                while (reader.Read())
                {
                    string strShutdown = reader[0].ToString();
                    objShutdownID.SDownID = reader[0].ToString();
                    ShutdownIDResult.Add(objShutdownID);

                }

                resultJson = new JavaScriptSerializer().Serialize(ShutdownIDResult);
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                oraConnection.Close();
                HttpContext.Current.Response.End();
            }
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void SendEmail(string include, string emailTo, string strShutdownID, string strUserName, string strMsgBody, string strDateTime, string strDescription)
        {

            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            bool Flag = false;
            string strPSLPortalURL = "";
            if (System.Configuration.ConfigurationManager.AppSettings["PSLPortalURL"] != null)
            {
                strPSLPortalURL = System.Configuration.ConfigurationManager.AppSettings["PSLPortalURL"];
            }
            else
            {
                return;
            }
            try
            {
                DateTime localDate = DateTime.Now;
                string txtEmailTo = emailTo;
                string EmailFrom = strUserName + "@pge.com";
                strMsgBody = strUserName + " submitted an outage for you on " + localDate + ". The PONS information for this outage is available on the Shutdown Letters Website: " + strPSLPortalURL;
                strMsgBody += "\n\nPSC Information\n------------------------------------------------------------------\nShutdown ID Number: " + strShutdownID + Environment.NewLine + "Submitter's LanID: " + strUserName;

                strMsgBody += "\n" + "Outage Date: " + strDateTime;
                strMsgBody += "\n\nAffected Area: " + strDescription;


                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(EmailFrom);
                if (include == "0")
                    msg.To.Add(new MailAddress(txtEmailTo));
                else
                {
                    msg.CC.Add(new MailAddress(EmailFrom));
                    msg.To.Add(new MailAddress(txtEmailTo));
                }
                msg.Subject = "Planned Shutdown Request Confirmation";
                msg.Body = strMsgBody;
                string mailhost = WebConfigurationManager.AppSettings["mailhost"];
                SmtpClient smtp = new SmtpClient(mailhost);
                smtp.UseDefaultCredentials = true;
                smtp.Send(msg);
                Flag = true;
                resultJson = new JavaScriptSerializer().Serialize(Flag);
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }
        //StandardPrinting
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public void StandardPrintSendEmail(string include, string emailTo, string strUserName, string strMsgBody, string strDateTime, string strDescription)
        {

            string resultJson = "";
            HttpContext.Current.Response.ContentType = "application/json";
            bool Flag = false;


            try
            {
                DateTime localDate = DateTime.Now;
                string txtEmailTo = emailTo;
                string EmailFrom = "EDGISSupport@pge.com";
                strMsgBody = "Hi,";

                strMsgBody += "<br>" + " Your map print files are available  now " + localDate + ". " + "<a href=\"" + strDescription + "\">Please click here to save the files</a>.";

                //strMsgBody += "\n" + " Your map print files are available  now " + localDate + " ";
                //strMsgBody = "\n" + "Please download the file from below path:  <a href=\"" + strDescription + "\">here</ID></a>"; 
                strMsgBody += "<br><br> Warning: Files more than " + _PrintDeleteDate + "  days old will be auto-deleted from shared location.";
                strMsgBody += "<br><br> Thank You,<br> EDGIS Support.";


                MailMessage msg = new MailMessage();
                msg.IsBodyHtml = true;
                msg.From = new MailAddress(EmailFrom);
                if (include == "0")
                    msg.To.Add(new MailAddress(txtEmailTo));
                else
                {
                    // msg.CC.Add(new MailAddress(EmailFrom));
                    msg.To.Add(new MailAddress(txtEmailTo));
                }
                msg.Subject = "Standard Print Map Zip file";
                msg.Body = strMsgBody;
                string mailhost = WebConfigurationManager.AppSettings["mailhost"];
                SmtpClient smtp = new SmtpClient(mailhost);
                smtp.UseDefaultCredentials = true;
                smtp.Send(msg);
                Flag = true;
                resultJson = new JavaScriptSerializer().Serialize(Flag);
                HttpContext.Current.Response.Write(resultJson);
            }
            catch (Exception exception)
            {
                resultJson = "{\"Error\":\"" + exception.ToString() + "\"}";
                HttpContext.Current.Response.Write(resultJson);
                HttpContext.Current.Response.End();
                throw;
            }
            finally
            {
                HttpContext.Current.Response.End();
            }
        }

    }
}

