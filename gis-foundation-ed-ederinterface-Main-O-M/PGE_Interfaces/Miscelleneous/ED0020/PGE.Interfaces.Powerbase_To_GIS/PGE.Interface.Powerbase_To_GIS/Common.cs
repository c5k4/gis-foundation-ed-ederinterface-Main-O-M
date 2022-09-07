using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Oracle.DataAccess.Client;
using System.Data;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using System.Configuration;
using System.IO;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Collections;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;

namespace PGE.Interface.Powerbase_To_GIS
{
    public class Common
    {
        public static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, ReadConfigurations.LOGCONFIG);
        private static AoInitializeClass _mAoInit = null;
        public static IMMAppInitialize _mmAppInit = null;
        public static mmLicenseStatus _arcFMLicenseStatus;

        public static string _mailContent = null;

        /// <summary>
        /// Initialize the license required to read/edit ArcGIS components
        /// </summary>
        /// <returns></returns>
        public static void InitializeESRILicense()
        {
            try
            {
                //Cache product codes by enum int so can be sorted without custom sorter
                List<int> m_requestedProducts = new List<int>();
                foreach (esriLicenseProductCode code in new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeAdvanced })
                {
                    int requestCodeNum = Convert.ToInt32(code);
                    if (!m_requestedProducts.Contains(requestCodeNum))
                    {
                        m_requestedProducts.Add(requestCodeNum);
                    }
                }

                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Desktop);

                _mAoInit = new AoInitializeClass();
                esriLicenseProductCode currentProduct = new esriLicenseProductCode();
                foreach (int prodNumber in m_requestedProducts)
                {
                    esriLicenseProductCode prod = (esriLicenseProductCode)Enum.ToObject(typeof(esriLicenseProductCode), prodNumber);
                    esriLicenseStatus status = _mAoInit.IsProductCodeAvailable(prod);
                    if (status == esriLicenseStatus.esriLicenseAvailable)
                    {
                        status = _mAoInit.Initialize(prod);
                        if (status == esriLicenseStatus.esriLicenseAlreadyInitialized ||
                            status == esriLicenseStatus.esriLicenseCheckedOut)
                        {
                            currentProduct = _mAoInit.InitializedProduct();
                        }
                    }
                }

                Common._log.Info("ESRI License checked out successfully.");
            }
            catch (Exception exp)
            {
                Common._log.Error("ESRI License check out failed.");
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                throw exp;
            }
        }

        /// <summary>
        /// Initializes the license necessary to read/edit ArcFM components
        /// </summary>
        /// <returns></returns>
        public static void InitializeArcFMLicense()
        {
            try
            {
                //Comm.LogManager.WriteLine("Checking out ArcFM license...");
                _mmAppInit = new MMAppInitialize();
                // check and see if the type of license is available to check out
                _arcFMLicenseStatus = _mmAppInit.IsProductCodeAvailable(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
                if (_arcFMLicenseStatus == mmLicenseStatus.mmLicenseAvailable)
                {
                    // if the license is available, try to check it out.
                    _arcFMLicenseStatus = _mmAppInit.Initialize(Miner.Interop.mmLicensedProductCode.mmLPArcFM);

                    if (_arcFMLicenseStatus != mmLicenseStatus.mmLicenseCheckedOut)
                    {
                        // if the license cannot be checked out, an exception is raised
                        Common._log.Error("Arc FM License check out failed.");
                        throw new Exception("The ArcFM license requested could not be checked out");
                    }
                    Common._log.Info("Arc FM License checked out successfully.");
                }

            }
            catch (Exception exp)
            {
                Common._log.Error("Arc FM License check out failed.");
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                throw exp;
            }
        }

        /// <summary>
        /// Closes the license object. The user will not be able to read/edit ArcGIS
        /// or ArcFM components after a call to this method
        /// </summary>
        /// <returns></returns>
        public static void CloseLicenseObject()
        {
            //ESRI License Initializer generated code.
            //Do not make any call to ArcObjects after ShutDownApplication()            
            try
            {
                if (_mmAppInit != null)
                {
                    _mmAppInit.Shutdown();
                }

                if (_mAoInit != null)
                {
                    _mAoInit.Shutdown();
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());               
            }
        }

        /// <summary>
        /// This function update all data in session/version
        /// </summary>
        /// <param name="argFeatversionWorkspace"></param>
        /// <param name="argdtAllDataToUpdate"></param>
        /// <returns></returns>
        public static bool UpdatePowerbaseDataInVersion(IFeatureWorkspace argFeatversionWorkspace, ref DataTable argdtAllDataToUpdate)
        {
            bool bProcessSuccess = false;
            IQueryFilter pqueryfilter = null;
            IFeatureCursor pfeatcursor = null;
            IFeature pfeature = null;
            string strGLOBALID = null;
            Dictionary<string, System.Object> dictFeatureClasses = null;
            IFeatureClass pFeatureClass = null;
            bool bValueSaved = false;
            bool bRelatedValueSaved = false;
            bool bAnyOfRelatedValuesOrUnitRecordUpdated = false;
            DataRow[] pRows = null;
            DataRow[] pColumnRows = null;
            string PBColumnName = null;
            string GISColumnName = null;
            string valueToSave = null;
            int iFieldIndex = 0;
            bool bHasSubtypes = false;
            string colNameGlobalID = null;
            string strAllAcceptableValues = string.Empty;
            string strAcceptableValueString = null;
            string strorderbyclause = null;
            string connstringpgedata = null;
            string comment = null;
            string strOperatingNumber = null;
            Int64 ipb_rlid =0;
            string commentsToStore = string.Empty;

            int previouslength = 0;
            try
            {
                connstringpgedata = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata);

                // Find all distinct featureclasses in table and keep in List
                dictFeatureClasses = GetDistinctFeatureClassesTables(argdtAllDataToUpdate, argFeatversionWorkspace);

                foreach (String fcName in dictFeatureClasses.Keys)
                {
                    string gisFeatureclassName = fcName;
                    try
                    {
                        strorderbyclause = ReadConfigurations.col_LAST_MODIFIED + " ASC";

                        pRows = argdtAllDataToUpdate.Select(ReadConfigurations.col_FEATURECLASS + "='" + gisFeatureclassName + "'", strorderbyclause);

                        //DataTable dt = pRows.CopyToDataTable();

                        if (gisFeatureclassName.Contains(ReadConfigurations.value_UNIT))
                        {
                            gisFeatureclassName = fcName.Replace(ReadConfigurations.value_UNIT, string.Empty);
                        }

                        colNameGlobalID = GetGLOBALIDColumnName(gisFeatureclassName);

                        pFeatureClass = dictFeatureClasses[gisFeatureclassName] as IFeatureClass;

                        foreach (DataRow pRow in pRows)
                        {
                            try
                            {
                                if (commentsToStore.Length > previouslength)
                                {
                                    previouslength = commentsToStore.Length;
                                }

                                strGLOBALID = Convert.ToString(pRow[colNameGlobalID]);
                                commentsToStore = string.Empty;

                                try
                                {
                                    strOperatingNumber = Convert.ToString(pRow[ReadConfigurations.col_OPERATINGNUMBER]);
                                    ipb_rlid = Convert.ToInt64(pRow[ReadConfigurations.col_PB_RLID]);
                                }
                                catch (System.Exception exp)
                                {
                                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                }

                                Common._log.Info("Processing for GUID : " + strGLOBALID);

                                pqueryfilter = new QueryFilterClass();
                                pqueryfilter.WhereClause = ReadConfigurations.col_GLOBALID + " = '" + strGLOBALID + "'";
                                pfeatcursor = pFeatureClass.Update(pqueryfilter, false);
                                pfeature = pfeatcursor.NextFeature();
                                if (pfeature != null)
                                {
                                    pColumnRows = MainClass._dtPBGISMapping.Select(ReadConfigurations.col_GISFeatureClassName + "='" + gisFeatureclassName + "'");

                                    bHasSubtypes = !string.IsNullOrEmpty(Convert.ToString(pColumnRows[0][ReadConfigurations.col_SUBTYPE]));

                                    if (bHasSubtypes)
                                    {
                                        ISubtypes subTypes = pfeature.Class as ISubtypes;
                                        int subtypeCodeToGet = -1;
                                        subtypeCodeToGet = Int32.Parse(pfeature.get_Value(subTypes.SubtypeFieldIndex).ToString());

                                        pColumnRows = MainClass._dtPBGISMapping.Select(ReadConfigurations.col_GISFeatureClassName + "='" + gisFeatureclassName + "' AND " + ReadConfigurations.col_SUBTYPE + "=" + subtypeCodeToGet + "");
                                    }

                                    bValueSaved = false;
                                    bAnyOfRelatedValuesOrUnitRecordUpdated = false;

                                    foreach (DataRow pColumnRow in pColumnRows)
                                    {
                                        try
                                        {
                                            PBColumnName = Convert.ToString(pColumnRow[ReadConfigurations.col_PBFieldName]);
                                            GISColumnName = Convert.ToString(pColumnRow[ReadConfigurations.col_GISFieldName]);
                                            valueToSave = Convert.ToString(pRow[PBColumnName]);

                                            if (!string.IsNullOrEmpty(GISColumnName) && !(valueToSave == ReadConfigurations.GetValue(ReadConfigurations.PB_Value_NO_UPDATE) || valueToSave == ReadConfigurations.GetValue(ReadConfigurations.PB_Value_NOT_RELEVANT)))
                                            {
                                                if (GISColumnName.Contains("."))
                                                {
                                                    // Update the corresponding columns in all related records and create record for SCADA if record does not exist
                                                    bRelatedValueSaved = ManageRelatedRecords(gisFeatureclassName, strGLOBALID, strOperatingNumber, ipb_rlid, GISColumnName, pFeatureClass, pfeature, valueToSave, ref commentsToStore);

                                                    if (bRelatedValueSaved)
                                                    {
                                                        bAnyOfRelatedValuesOrUnitRecordUpdated = true;
                                                    }
                                                }
                                                else
                                                {
                                                    iFieldIndex = pfeature.Fields.FindField(GISColumnName);

                                                    // value ToSave Can be checked for correctness of the code for doamin controlled columns
                                                    if (iFieldIndex != -1)
                                                    {
                                                        IField pField = pfeature.Fields.get_Field(iFieldIndex);

                                                        // Check the empty or Null value here , whether we need to check the empty value also from domain codes                                                                                           
                                                        if (pField.Domain != null && pField.Domain is ICodedValueDomain)
                                                        {
                                                            if (!ChecktheGivenValueIsPresentInDomainValues(valueToSave, (ICodedValueDomain)pField.Domain, ref strAllAcceptableValues))
                                                            {
                                                                strAcceptableValueString = "EDGIS Acceptable values : " + strAllAcceptableValues;

                                                                comment = "Given attribute value is not eligible to update in EDGIS. ";
                                                                comment += "Feature Class : " + gisFeatureclassName + "Globalid : " + strGLOBALID + ", Attribute Name : " + GISColumnName + ", Attribute Value : " + valueToSave + ", Comments : " + strAcceptableValueString;
                                                                Common._log.Info(comment);

                                                                MainClass._dtRejectedRecords.Rows.Add(gisFeatureclassName, strGLOBALID, strOperatingNumber, ipb_rlid, GISColumnName, valueToSave, strAcceptableValueString);

                                                                commentsToStore += ",Value not present in EDGIS Domains for column : " + GISColumnName + " " + strAcceptableValueString;

                                                                continue;
                                                                // Add to log - The value given is not matching with the values from given domain name
                                                            }
                                                        }

                                                        IRow precordRow = (IRow)pfeature;

                                                        bValueSaved = UpdateFieldValue(ref precordRow, iFieldIndex, valueToSave);

                                                        //pfeature.set_Value(iFieldIndex, valueToSave);
                                                        //bValueSaved = true;
                                                        if (bValueSaved)
                                                        {
                                                            comment = "Updated value for FeatureClass : " + gisFeatureclassName + ", GlobalID : " + strGLOBALID + " , Column Name : " + GISColumnName + " , Value : " + valueToSave;
                                                        }
                                                        else
                                                        {
                                                            comment = "Not saving because DB value is same as given value for FeatureClass : " + gisFeatureclassName + ", GlobalID : " + strGLOBALID + " , Column Name : " + GISColumnName + " , Value : " + valueToSave;
                                                            commentsToStore += ",Given value is same as DB value for Column Name:" + GISColumnName;
                                                        }
                                                        Common._log.Info(comment);
                                                    }
                                                    else
                                                    {
                                                        comment = "Given field as per mapping config is not present in EDGIS featureclass/Table.";
                                                        comment += "Feature Class : " + gisFeatureclassName + "Globalid : " + strGLOBALID + ", Attribute Name : " + GISColumnName + ", Attribute Value : " + valueToSave;
                                                        Common._log.Info(comment);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // Given coumn is not relevant to update feature class "FC"
                                            }
                                        }
                                        catch (System.Exception exp)
                                        {
                                            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                        }
                                    }

                                    // After updating the voltage regulator record , better to update voltage regulator unit record and save the record after that
                                    if (fcName.Contains(ReadConfigurations.value_UNIT))
                                    {
                                        bool bUnitRecordUpdated = HandleUnitRecordsUpdate(pRow, dictFeatureClasses, fcName, strOperatingNumber, ipb_rlid, ref commentsToStore);

                                        if (bUnitRecordUpdated)
                                            bAnyOfRelatedValuesOrUnitRecordUpdated = true;
                                    }

                                    commentsToStore = commentsToStore.Trim(',');
                                    pRow[ReadConfigurations.col_COMMENTS] = commentsToStore;

                                    pRow[ReadConfigurations.col_RECORD_STATUS] = ReadConfigurations.value_PROCESSED;

                                    if (bValueSaved || bAnyOfRelatedValuesOrUnitRecordUpdated)
                                    {
                                        try
                                        {
                                            if (bValueSaved)
                                                pfeature.Store();

                                            comment = "Stored all values for FeatureClass : " + gisFeatureclassName + ", GlobalID : " + strGLOBALID;
                                            Common._log.Info(comment);
                                        }
                                        catch (System.Exception exp)
                                        {
                                            comment = "Error occured while storing all values for FeatureClass : " + gisFeatureclassName + ", GlobalID : " + strGLOBALID;
                                            Common._log.Error(comment);
                                            Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                            pRow[ReadConfigurations.col_RECORD_STATUS] = ReadConfigurations.value_FAILED;
                                        }
                                    }
                                    else
                                    {
                                        //pRow[ReadConfigurations.col_RECORD_STATUS] = ReadConfigurations.value_PROCESSED;
                                    }
                                }
                                else
                                {
                                    // The feature with given GUID does not exist in EDGIS. 
                                    comment = "The feature with given GUID does not exist in EDGIS.";
                                    comment += "Feature Class : " + gisFeatureclassName + " , Globalid : " + strGLOBALID;
                                    Common._log.Info(comment);

                                    commentsToStore += ",GUID does not exist in EDGIS.";

                                    pRow[ReadConfigurations.col_RECORD_STATUS] = ReadConfigurations.value_GUID_NOT_FOUND;

                                    commentsToStore = commentsToStore.Trim(',');
                                    pRow[ReadConfigurations.col_COMMENTS] = commentsToStore;
                                }
                            }
                            catch (System.Exception exp)
                            {
                                comment = "Error in processing record with GUID : " + strGLOBALID;
                                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                Common._log.Error(comment);
                                pRow[ReadConfigurations.col_RECORD_STATUS] = ReadConfigurations.value_FAILED;
                                pRow[ReadConfigurations.col_COMMENTS] = comment;
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                    }
                    finally
                    {
                        if (pfeatcursor != null)
                        {
                            Marshal.FinalReleaseComObject(pfeatcursor);
                            pfeatcursor = null;
                        }
                        if (pqueryfilter != null)
                        {
                            Marshal.FinalReleaseComObject(pqueryfilter);
                            pqueryfilter = null;
                        }
                    }
                }

                // Insert the rejected records data into the table
                DBHelper.ProcessRejectedRecords(MainClass._dtRejectedRecords);

                bProcessSuccess = true;

                Common._log.Info("Max length comments : "+previouslength);
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                bProcessSuccess = false;
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            finally
            {
                if (dictFeatureClasses != null)
                {
                    dictFeatureClasses.Clear();
                    dictFeatureClasses = null;
                }
            }
            return bProcessSuccess;
        }

        /// <summary>
        /// This function update value for the given field for given feature if the DB value is different
        /// </summary>
        /// <param name="argpRow"></param>
        /// <param name="argiFieldIndex"></param>
        /// <param name="argValueToSave"></param>
        /// <returns></returns>
        private static bool UpdateFieldValue(ref IRow argpRow,int argiFieldIndex, string argValueToSave)
        {
            bool bValueSaved = false;
            string dbValue = null;
            try
            {
                dbValue = Convert.ToString(argpRow.get_Value(argiFieldIndex));

                if (dbValue.Trim() != argValueToSave)
                {
                    argpRow.set_Value(argiFieldIndex, argValueToSave);
                    bValueSaved = true;
                }
                else
                { 
                    // The value in DB is same as the value to save
                }
            }
            catch (System.Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                bValueSaved = false;
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bValueSaved;
        }


        /// <summary>
        /// This function returns the featureclasses objects
        /// </summary>
        /// <param name="argdtAllDataToUpdate"></param>
        /// <param name="argFeatversionWorkspace"></param>
        /// <returns></returns>
        public static Dictionary<string, System.Object> GetDistinctFeatureClassesTables(DataTable argdtAllDataToUpdate, IFeatureWorkspace argFeatversionWorkspace)
        {
            Dictionary<string, System.Object> dictFeatuerClassesAndTables = new System.Collections.Generic.Dictionary<string, System.Object>();
            try
            {
                // Find all distinct featureclasses in table and keep in List
                var featureclasses = (from r in argdtAllDataToUpdate.AsEnumerable()
                                      select r[ReadConfigurations.col_FEATURECLASS]).Distinct().ToList();

                foreach (string fc in featureclasses)
                {
                    string fcName = fc;
                    try
                    {
                        if (fcName.Contains(ReadConfigurations.value_UNIT))
                        {
                            ITable pTable = argFeatversionWorkspace.OpenTable(fcName);
                            dictFeatuerClassesAndTables.Add(fcName, pTable);

                            fcName = fc.Replace(ReadConfigurations.value_UNIT, string.Empty);
                        }

                        IFeatureClass pFeatureClass = argFeatversionWorkspace.OpenFeatureClass(fcName);
                        dictFeatuerClassesAndTables.Add(fcName, pFeatureClass);
                    }
                    catch (Exception exp)
                    {
                        Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                        ErrorCodeException ece = new ErrorCodeException(exp);
                        Environment.ExitCode = ece.CodeNumber;
                    }
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return dictFeatuerClassesAndTables;
        }

        /// <summary>
        /// This function make changes in PB Session table
        /// </summary>
        /// <param name="argStatus"></param>
        /// <param name="argstrSessionName"></param>
        /// <param name="argStrOperation"></param>
        /// <returns></returns>
        public static bool MakeChangesForPBSessionTable(string argStatus, string argstrSessionName, string argStrOperation)
        {
            string sQuery = null;
            string strTableName = null;
            try
            {
                strTableName = ReadConfigurations.GetValue(ReadConfigurations.TB_PB_Session);

                if (argStrOperation == "INSERT")
                {
                    sQuery = "INSERT INTO " + strTableName + " (" + ReadConfigurations.col_SESSION_NAME + "," + ReadConfigurations.col_PROCESSED_ON + "," + ReadConfigurations.col_STATUS + ") VALUES " +
                            "('" + argstrSessionName + "'," + "TO_DATE(sysdate,'" + ConfigurationManager.AppSettings["dateFormatToChange"] + "'),'" + argStatus + "')";
                }
                else if (argStrOperation == "UPDATE")
                {
                    sQuery = "UPDATE " + strTableName + " SET STATUS='" + argStatus + "'";
                }

                string strConnString = ReadConfigurations.GetValue(ReadConfigurations.ConnectionString_pgedata);
                int intUpdateResult = DBHelper.ExecuteQueryOnDatabase(strConnString, sQuery);

                Common._log.Info("Ran query : " + sQuery + " and result count: " + intUpdateResult);

                return true;
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                Common._log.Error("Error in Updating the Custom Session Table . Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                return false;
            }
        }

        /// <summary>
        /// This function moves data to Archive table
        /// </summary>
        /// <param name="argstrConnString"></param>
        /// <param name="argStrMainTable"></param>
        /// <param name="argStrArchiveTable"></param>
        /// <returns></returns>
        public static bool MoveDataToArchiveTable(string argstrConnString, string argStrMainTable, string argStrArchiveTable)
        {
            string sQuery = null;
            try
            {
                sQuery = "insert into " + argStrArchiveTable + " select * from " + argStrMainTable;

                int intUpdateResult = DBHelper.ExecuteQueryOnDatabase(argstrConnString, sQuery);
                Common._log.Info("Ran query : " + sQuery + " and result count: " + intUpdateResult);

                sQuery = "delete from " + argStrMainTable;

                intUpdateResult = DBHelper.ExecuteQueryOnDatabase(argstrConnString, sQuery);
                Common._log.Info("Ran query : " + sQuery + " and result count: " + intUpdateResult);

                return true;
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                Common._log.Error("Error in Updating the Custom Session Table . Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                return false;
            }
        }

        /// <summary>
        /// This function checks whether given value is present in given domain code or not
        /// </summary>
        /// <param name="argStrValue"></param>
        /// <param name="argCodeValueDomain"></param>
        /// <param name="strAllAcceptableValues"></param>
        /// <returns></returns>
        private static bool ChecktheGivenValueIsPresentInDomainValues(string argStrValue, ICodedValueDomain argCodeValueDomain, ref string strAllAcceptableValues)
        {
            bool bValueExist = false;
            strAllAcceptableValues = string.Empty;
            try
            {
                for (int i = 0; i < argCodeValueDomain.CodeCount; i++)
                {
                    string strValue = Convert.ToString(argCodeValueDomain.get_Value(i));

                    strAllAcceptableValues += strValue + ",";

                    if (argStrValue == strValue)
                    {
                        bValueExist = true;
                        break;
                    }
                }

                strAllAcceptableValues = strAllAcceptableValues.TrimEnd(',');
            }
            catch (Exception exp)
            {
                bValueExist = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bValueExist;
        }

        /// <summary>
        /// This function manages the related records
        /// </summary>
        /// <param name="argstrgisFeatureclassName"></param>
        /// <param name="argstrGLOBALID"></param>
        /// <param name="argstrGISColumnName"></param>
        /// <param name="argpFeatureClass"></param>
        /// <param name="argpfeature"></param>
        /// <param name="argstrValueToSave"></param>
        /// <returns></returns>
        private static bool ManageRelatedRecords(string argstrgisFeatureclassName, string argstrGLOBALID, string argStrOperatingNumber, Int64 argipb_rlid, string argstrGISColumnName, IFeatureClass argpFeatureClass, IFeature argpfeature, string argstrValueToSave, ref string commentsToStore)
        {
            bool bUpdateSuccessfull = false;
            int iFieldIndex = 0;
            string relatedTableName = null;
            string relatedTableColumnName = null;
            IField pField = null;
            IDomain pDomain = null;
            bool bbreakloop = false;
            string strAllAcceptableValues = string.Empty;
            string strAcceptableValueString = string.Empty;
            string comment = null;
            bool bValueSaved = false;
            try
            {
                // Find related records and update        
                relatedTableName = argstrGISColumnName.Split('.')[0] + "." + argstrGISColumnName.Split('.')[1];
                relatedTableColumnName = argstrGISColumnName.Split('.')[2];

                IEnumRelationshipClass relClasses = argpFeatureClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                IRelationshipClass rel = relClasses.Next();
                while (rel != null)
                {
                    string relDestinationClassName = ((IDataset)rel.DestinationClass).Name.ToUpper();
                    string relOriginClassName = ((IDataset)rel.OriginClass).Name.ToUpper();

                    if (relatedTableName.ToUpper() == relDestinationClassName.ToUpper())
                    {
                        ESRI.ArcGIS.esriSystem.ISet resultSet = rel.GetObjectsRelatedToObject(argpfeature);

                        // Case 1 If there are related records 
                        // Case 2 If there are no related records   
                                                   
                        if (resultSet.Count > 0)
                        {
                            IRow pRelRow = (IRow)resultSet.Next();

                            while (pRelRow != null)
                            {
                                try
                                {
                                    iFieldIndex = pRelRow.Fields.FindField(relatedTableColumnName);

                                    if (iFieldIndex != -1)
                                    {
                                        pField = pRelRow.Fields.get_Field(iFieldIndex);

                                        pDomain = Common.GetDomain(pRelRow, pField);

                                        // Check the empty or Null value here , whether we need to check the empty value also from domain codes                                                                                           
                                        if (pDomain != null && pDomain is ICodedValueDomain && (ICodedValueDomain)pDomain != null)
                                        {
                                            if (ChecktheGivenValueIsPresentInDomainValues(argstrValueToSave, (ICodedValueDomain)pDomain, ref strAllAcceptableValues))
                                            {
                                                // Check the value before final update , if the not null value is same then don't update

                                                bValueSaved = UpdateFieldValue(ref pRelRow, iFieldIndex, argstrValueToSave);

                                                //pRelRow.set_Value(iFieldIndex, argstrValueToSave);

                                                if (bValueSaved)
                                                {
                                                    comment = "Updated value for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID + " , Column Name : " + relatedTableColumnName + " , Value : " + argstrValueToSave;
                                                }
                                                else
                                                {
                                                    commentsToStore += ",Given value is same as DB value for Column Name:" + argstrGISColumnName;
                                                }

                                                Common._log.Info(comment);
                                                Common._log.Info(commentsToStore);

                                                try
                                                {
                                                    if (bValueSaved)
                                                    {
                                                        pRelRow.Store();
                                                    }
                                                    bUpdateSuccessfull = true;

                                                    comment = "Stored all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                    Common._log.Info(comment);
                                                }
                                                catch (System.Exception exp)
                                                {
                                                    comment = "Error occured while storing all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                    Common._log.Info(comment);
                                                    // throw;
                                                }
                                            }
                                            else
                                            {
                                                // Add to log - The value given is not matching with the values from given domain name

                                                strAcceptableValueString = "EDGIS Acceptable values : " + strAllAcceptableValues;

                                                comment = "Given attribute value is not eligible to update in EDGIS. ";
                                                comment += "Feature Class : " + argstrgisFeatureclassName + "Globalid : " + argstrGLOBALID + ", Attribute Name : " + argstrGISColumnName + ", Attribute Value : " + argstrValueToSave + ", Comments : " + strAcceptableValueString;
                                                Common._log.Info(comment);

                                                MainClass._dtRejectedRecords.Rows.Add(argstrgisFeatureclassName, argstrGLOBALID, argStrOperatingNumber, argipb_rlid, argstrGISColumnName, argstrValueToSave, strAcceptableValueString);

                                                commentsToStore += ",Value not present in EDGIS Domains for column : " + argstrGISColumnName + " " + strAcceptableValueString;                                                                
                                                               
                                                bbreakloop = true;
                                            }
                                        }
                                        else
                                        {
                                            bValueSaved = UpdateFieldValue(ref pRelRow, iFieldIndex, argstrValueToSave);

                                            //pRelRow.set_Value(iFieldIndex, argstrValueToSave);

                                            if (bValueSaved)
                                            {
                                                comment = "Updated value for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID + " , Column Name : " + relatedTableColumnName + " , Value : " + argstrValueToSave;
                                            }
                                            else
                                            {
                                                commentsToStore += ",Given value is same as DB value for Column Name:" + argstrGISColumnName;
                                            }
                                            
                                            Common._log.Info(comment);
                                            Common._log.Info(commentsToStore);

                                            try
                                            {
                                                if (bValueSaved)
                                                {
                                                    pRelRow.Store();
                                                }

                                                bUpdateSuccessfull = true;

                                                comment = "Stored all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                Common._log.Info(comment);
                                            }
                                            catch (System.Exception exp)
                                            {
                                                comment = "Error occured while storing all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                Common._log.Info(comment);
                                                // throw;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        comment = "Given field as per mapping config is not present in EDGIS featureclass/Table.";
                                        comment += "Feature Class : " + relatedTableName + "Globalid : " + argstrGLOBALID + ", Attribute Name : " + relatedTableColumnName + ", Attribute Value : " + argstrValueToSave;
                                        Common._log.Info(comment);
                                    }
                                }
                                catch (System.Exception exp)
                                {
                                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                }                              

                                pRelRow = (IRow)resultSet.Next();
                            }
                        }
                        else
                        {
                            // No related record found , create the new record in case of SCADA
                            if (relatedTableName.ToUpper() == ReadConfigurations.TB_SCADA)
                            {
                                if (argstrValueToSave == ReadConfigurations.value_Y || argstrValueToSave == ReadConfigurations.value_N)
                                {
                                    bUpdateSuccessfull = CreateRelatedRow((IRow)argpfeature, ReadConfigurations.TB_SCADA, relatedTableColumnName, argstrValueToSave);

                                    if (bUpdateSuccessfull)
                                    {
                                        Common._log.Info("SCADA record created successfully.");
                                    }
                                    else
                                    {
                                        Common._log.Info("There is some issue in creating SCADA record.");
                                    }
                                }
                                else
                                {
                                    strAcceptableValueString = "EDGIS Acceptable values : N,Y";

                                    // The value shared by EI/Powerbase is not from domain code values for SCADA record.
                                    comment = "Given attribute value is not eligible to update in EDGIS. ";
                                    comment += "Feature Class : " + argstrgisFeatureclassName + "Globalid : " + argstrGLOBALID + ", Attribute Name : " + argstrGISColumnName + ", Attribute Value : " + argstrValueToSave + ", Comments : " + strAcceptableValueString;
                                    Common._log.Info(comment);

                                    MainClass._dtRejectedRecords.Rows.Add(argstrgisFeatureclassName, argstrGLOBALID, argStrOperatingNumber, argipb_rlid, argstrGISColumnName, argstrValueToSave, strAcceptableValueString);

                                    commentsToStore += ",Value not present in EDGIS Domains for column : " + argstrGISColumnName + " " + strAcceptableValueString;                                                                
                
                                    bbreakloop = true;
                                }
                            }
                        }
                    }

                    if (bUpdateSuccessfull || bbreakloop)
                    {
                        break;
                    }

                    rel = relClasses.Next();
                }
            }
            catch (System.Exception exp)
            {               
                bUpdateSuccessfull = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bUpdateSuccessfull;
        }

        /// <summary>
        /// This function prepare the data to create the related row
        /// </summary>
        /// <param name="argParentRow"></param>
        /// <param name="argStrrelatedTableName"></param>
        /// <param name="argStrrelatedFieldName"></param>
        /// <param name="argStrfieldValueToSave"></param>
        /// <returns></returns>
        private static bool CreateRelatedRow(IRow argParentRow, string argStrrelatedTableName, string argStrrelatedFieldName, string argStrfieldValueToSave)
        {
            IRow relatedRow = null;
            int iFieldIndex = 0;
            bool bRecordCreated = false;
            string comment = null;
            try
            {
                relatedRow = CreateRelatedRow(argParentRow, argStrrelatedTableName);

                if (relatedRow.Fields.FindField(ReadConfigurations.col_STATUS) > -1)
                {
                    // Use Parent Status
                    if (argParentRow.Fields.FindField(ReadConfigurations.col_STATUS) > -1)
                    {
                        object parentRowStatus = argParentRow.get_Value(argParentRow.Fields.FindField(ReadConfigurations.col_STATUS));
                        if (parentRowStatus != DBNull.Value)
                        {
                            relatedRow.set_Value(relatedRow.Fields.FindField(ReadConfigurations.col_STATUS), parentRowStatus);
                        }
                        else
                        {
                            relatedRow.set_Value(relatedRow.Fields.FindField(ReadConfigurations.col_STATUS), 5); // 5 == In Service
                        }
                    }
                    else
                    {
                        relatedRow.set_Value(relatedRow.Fields.FindField(ReadConfigurations.col_STATUS), 5); // 5 == In Service
                    }
                }

                iFieldIndex = relatedRow.Fields.FindField(argStrrelatedFieldName);

                if (iFieldIndex != -1)
                {
                    relatedRow.set_Value(iFieldIndex, argStrfieldValueToSave);

                    comment = "Updated value for FeatureClass/Table Name : " + argStrrelatedTableName + ", OID : " + relatedRow.OID + " , Column Name : " + argStrrelatedFieldName + " , Value : " + argStrfieldValueToSave;
                    Common._log.Info(comment);

                    try
                    {
                        relatedRow.Store();
                        bRecordCreated = true;

                        comment = "Stored all values for FeatureClass/Table Name : " + argStrrelatedTableName + ", OID : " + relatedRow.OID;
                        Common._log.Info(comment);
                    }
                    catch (System.Exception exp)
                    {
                        comment = "Error occured while storing all values for FeatureClass/Table Name : " + argStrrelatedTableName + ", OID : " + relatedRow.OID;
                        Common._log.Info(comment);
                    }
                }
                else
                {
                    comment = "Given field as per mapping config is not present in EDGIS featureclass/Table.";
                    comment += "Feature Class/Table : " + argStrrelatedTableName + "OID : " + relatedRow.OID + ", Attribute Name : " + argStrfieldValueToSave + ", Attribute Value : " + argStrfieldValueToSave;
                    Common._log.Info(comment);
                }
            }
            catch (System.Exception exp)
            {
                bRecordCreated = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bRecordCreated;
        }

        /// <summary>
        /// This function create the related row
        /// </summary>
        /// <param name="parentRow"></param>
        /// <param name="relatedTable"></param>
        /// <returns></returns>
        private static IRow CreateRelatedRow(IRow parentRow, string relatedTable)
        {
            IWorkspace workspace = null;
            IRow newRow = null;
            bool parentRowIsSourceRelClass = true;
            try
            {
                workspace = ((IDataset)parentRow.Table).Workspace;                
                parentRowIsSourceRelClass = true;

                IRelationshipClass relationshipClass = GetRelationshipClass(parentRow, relatedTable);
                if (relationshipClass == null)
                {
                    throw new ApplicationException("No such relationshipClass for [ " + relatedTable + " ]");
                }

                if (((IDataset)relationshipClass.OriginClass).Name.ToUpper() == relatedTable)
                {
                    parentRowIsSourceRelClass = false;
                }

                IEnumRelationship relationshipEnum = relationshipClass.GetRelationshipsForObject((IObject)parentRow);
                IRelationship relationship = relationshipEnum.Next();

                if (relationship == null)
                {
                    IObjectClass objectClass = relationshipClass.DestinationClass;
                    newRow = ((ITable)objectClass).CreateRow();

                    ISubtypes subtypes = (ISubtypes)objectClass;
                    IRowSubtypes rowSubtypes = (IRowSubtypes)newRow;
                    if (subtypes.HasSubtype)// does the feature class have subtypes?
                    {
                        rowSubtypes.SubtypeCode = subtypes.DefaultSubtypeCode;
                    }

                    // initalize any default values that the feature has
                    rowSubtypes.InitDefaultValues();

                    if (parentRowIsSourceRelClass)
                    {
                        relationshipClass.CreateRelationship(parentRow as IObject, newRow as IObject);
                    }
                    else
                    {
                        relationshipClass.CreateRelationship(newRow as IObject, parentRow as IObject);
                    }
                }                
            }
            catch (System.Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return newRow;
        }

        /// <summary>
        /// This function returns the relatonship classes for given feature
        /// </summary>
        /// <param name="row"></param>
        /// <param name="relatedClass"></param>
        /// <returns></returns>
        private static IRelationshipClass GetRelationshipClass(IRow row, string relatedClass)
        {
            //_logger.Debug(MethodBase.GetCurrentMethod().Name);

            IEnumRelationshipClass enumRelationshipClass = null;
            IRelationshipClass relationshipClass = null;
            try
            {
                enumRelationshipClass = ((row.Table as IObjectClass)).get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                bool containsRelatedClass = false;
                while ((relationshipClass = enumRelationshipClass.Next()) != null)
                {
                    containsRelatedClass =
                        relatedClass.Equals(((IDataset)relationshipClass.DestinationClass).Name.ToUpper());
                    if (!containsRelatedClass)
                    {
                        containsRelatedClass =
                            relatedClass.Equals(((IDataset)relationshipClass.OriginClass).Name.ToUpper());
                    }
                    if (containsRelatedClass)
                    {
                        //_logger.Debug("Found RelationshipClass Orig [ " + relationshipClass.OriginClass.AliasName +
                        //              " ] Dest [ " + relationshipClass.DestinationClass.AliasName + " ]");

                        return relationshipClass;
                    }
                }
                return null;
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                throw;
            }
            finally
            {
                if (enumRelationshipClass != null) Marshal.FinalReleaseComObject(enumRelationshipClass);
            }
        }

        /// <summary>
        /// This function returns the domain for given field
        /// </summary>
        /// <param name="row"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static IDomain GetDomain(IRow row, IField field)
        {
            IObject obj = null;
            IDomain domain = null;
            try
            {
                obj = row as IObject;

                if (!((ISubtypes)obj.Class).HasSubtype)
                {
                    domain = field.Domain;
                }
                else
                {
                    ISubtypes subTypes = obj.Class as ISubtypes;
                    int subtypeCodeToGet = -1;

                    subtypeCodeToGet = Int32.Parse(row.get_Value(subTypes.SubtypeFieldIndex).ToString());
                    domain = subTypes.get_Domain(subtypeCodeToGet, field.Name);
                }
            }
            catch (System.Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return domain;
        }

        /// <summary>
        /// This function returns the GLOBALID column Name
        /// </summary>
        /// <param name="argStrFeatureClassName"></param>
        /// <returns></returns>
        private static string GetGLOBALIDColumnName(string argStrFeatureClassName)
        {
            string globalidColumnName = null;
            try
            {
                DataRow[] pRows = MainClass._dtPBGISMapping.Select(ReadConfigurations.col_GISFeatureClassName + "='" + argStrFeatureClassName + "'");

                if (pRows.Length > 0)
                {
                    globalidColumnName = Convert.ToString(pRows[0][ReadConfigurations.col_GLOBALIDColumnName]);
                }
            }
            catch (System.Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return globalidColumnName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static bool HandleUnitRecordsUpdate(DataRow argpRow, Dictionary<string, System.Object> argDictFeatureClasses, string argTableName, string argStrOperatingNumber, Int64 argipb_rlid,ref string commentsToStore)
        {
            bool bSuccess = false;
            IQueryFilter pqueryfilter = null;
            ICursor pcursor = null;
            IRow pRecordRow = null;
            string sGLOBALID = null;
            ITable pTable = null;
            bool bValueSaved = false;
            bool bRelatedValueSaved = false;
            bool bAnyRelatedValueSaved = false;
            DataRow[] pColumnRows = null;
            string PBColumnName = null;
            string GISColumnName = null;
            string valueToSave = null;
            int iFieldIndex = 0;
            string colNameGlobalID = null;
            string strAllAcceptableValues = string.Empty;
            string strAcceptableValueString = null;
            string comment = null;
            try
            {
                colNameGlobalID = GetGLOBALIDColumnName(argTableName); ;

                pTable = argDictFeatureClasses[argTableName] as ITable;

                sGLOBALID = Convert.ToString(argpRow[colNameGlobalID]);
                pqueryfilter = new QueryFilterClass();
                pqueryfilter.WhereClause = ReadConfigurations.col_GLOBALID + " = '" + sGLOBALID + "'";
                pcursor = pTable.Update(pqueryfilter, false);
                pRecordRow = pcursor.NextRow();
                if (pRecordRow != null)
                {
                    pColumnRows = MainClass._dtPBGISMapping.Select(ReadConfigurations.col_GISFeatureClassName + "='" + argTableName + "'");

                    foreach (DataRow pColumnRow in pColumnRows)
                    {
                        PBColumnName = Convert.ToString(pColumnRow[ReadConfigurations.col_PBFieldName]);
                        GISColumnName = Convert.ToString(pColumnRow[ReadConfigurations.col_GISFieldName]);
                        valueToSave = Convert.ToString(argpRow[PBColumnName]);

                        if (!string.IsNullOrEmpty(GISColumnName) && !(valueToSave == ReadConfigurations.GetValue(ReadConfigurations.PB_Value_NO_UPDATE) || valueToSave == ReadConfigurations.GetValue(ReadConfigurations.PB_Value_NO_UPDATE)))
                        {
                            if (GISColumnName.Contains("."))
                            {
                                // Update the corresponding columns in all related records and create record for SCADA if record does not exist
                                bRelatedValueSaved = ManageRelatedRecords_HandleUnitRecords(argTableName, sGLOBALID,argStrOperatingNumber,argipb_rlid, GISColumnName, pTable, pRecordRow, valueToSave,ref commentsToStore);

                                if (bRelatedValueSaved)
                                {
                                    bAnyRelatedValueSaved = true;
                                }
                            }
                            else
                            {
                                iFieldIndex = pRecordRow.Fields.FindField(GISColumnName);

                                // value ToSave Can be checked for correctness of the code for doamin controlled columns
                                if (iFieldIndex != -1)
                                {
                                    IField pField = pRecordRow.Fields.get_Field(iFieldIndex);

                                    // Check the empty or Null value here , whether we need to check the empty value also from domain codes                                                                                           
                                    if ((ICodedValueDomain)pField.Domain != null)
                                    {
                                        if (!ChecktheGivenValueIsPresentInDomainValues(valueToSave, (ICodedValueDomain)pField.Domain, ref strAllAcceptableValues))
                                        {
                                            strAcceptableValueString = "EDGIS Acceptable values : " + strAllAcceptableValues;

                                            // Add to log - The value given is not matching with the values from given domain name
                                            comment = "Given attribute value is not eligible to update in EDGIS. ";
                                            comment += "Feature Class : " + argTableName + "Globalid : " + sGLOBALID + ", Attribute Name : " + GISColumnName + ", Attribute Value : " + valueToSave + ", Comments : " + strAcceptableValueString;
                                            Common._log.Info(comment);
                                            
                                            MainClass._dtRejectedRecords.Rows.Add(argTableName, sGLOBALID, argStrOperatingNumber, argipb_rlid, GISColumnName, valueToSave, strAcceptableValueString);

                                            commentsToStore += ",Value not present in EDGIS Domains for column : " + GISColumnName + " " + strAcceptableValueString;                                                                
                                                               
                                            continue;
                                        }
                                    }
                                    
                                    bValueSaved = UpdateFieldValue(ref pRecordRow, iFieldIndex, valueToSave);

                                    if (bValueSaved)
                                    {
                                        comment = "Updated value for FeatureClass/Table Name : " + argTableName + ", GlobalID : " + sGLOBALID + " , Column Name : " + GISColumnName + " , Value : " + valueToSave;
                                    }
                                    else
                                    {
                                        commentsToStore += ",Given value is same as DB value for Column Name:" + GISColumnName;
                                    }
                                                                        
                                    Common._log.Info(comment);
                                    Common._log.Info(commentsToStore);
                                }
                                else
                                {
                                    comment = "Given field as per mapping config is not present in EDGIS featureclass/Table.";
                                    comment += "Feature Class : " + argTableName + "Globalid : " + sGLOBALID + ", Attribute Name : " + GISColumnName + ", Attribute Value : " + valueToSave;
                                    Common._log.Info(comment);                                
                                }
                            }
                        }
                        else
                        {
                            // Given coumn is not relevant to update feature class "FC"
                        }
                    }

                    if (bValueSaved)
                    {
                        try
                        {
                            pRecordRow.Store();
                            bSuccess = true;                           
                        }
                        catch (System.Exception exp)
                        {
                            comment = "Error occured while storing all values for FeatureClass/Table Name : " + argTableName + ", GlobalID : " + sGLOBALID;
                            Common._log.Info(comment);                            
                        }
                    }

                    comment = "Stored all values for FeatureClass/Table Name : " + argTableName + ", GlobalID : " + sGLOBALID;
                    Common._log.Info(comment);
                }
            }
            catch (Exception exp)
            {
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
                bSuccess = false;
            }
            finally
            {
                if (pcursor != null)
                {
                    Marshal.FinalReleaseComObject(pcursor);
                    pcursor = null;
                }
                if (pqueryfilter != null)
                {
                    Marshal.FinalReleaseComObject(pqueryfilter);
                    pqueryfilter = null;
                }
            }
            return bSuccess;
        }

        /// <summary>
        /// This function handles the related records for a unit record
        /// </summary>
        /// <param name="argstrgisFeatureclassName"></param>
        /// <param name="argstrGLOBALID"></param>
        /// <param name="argstrGISColumnName"></param>
        /// <param name="argpTable"></param>
        /// <param name="argpRow"></param>
        /// <param name="argstrValueToSave"></param>
        /// <returns></returns>
        private static bool ManageRelatedRecords_HandleUnitRecords(string argstrgisFeatureclassName, string argstrGLOBALID,string argStrOperatingNumber,Int64 argipb_rlid, string argstrGISColumnName, ITable argpTable, IRow argpRow, string argstrValueToSave,ref string commentsToStore)
        {
            bool bUpdateSuccessfull = false;
            int iFieldIndex = 0;
            string relatedTableName = null;
            string relatedTableColumnName = null;
            IField pField = null;
            IDomain pDomain = null;
            bool bbreakloop = false;
            string strAllAcceptableValues = string.Empty;
            string strAcceptableValueString = null;
            string comment = null;
            bool bValueSaved = false;
            try
            {
                // Find related records and update        
                relatedTableName = argstrGISColumnName.Split('.')[0] + "." + argstrGISColumnName.Split('.')[1];
                relatedTableColumnName = argstrGISColumnName.Split('.')[2];

                IObjectClass pClass = (IObjectClass)argpTable;
                IEnumRelationshipClass relClassesEnum = pClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
                IRelationshipClass rel = relClassesEnum.Next();
                while (rel != null)
                {
                    string relDestinationClassName = ((IDataset)rel.DestinationClass).Name.ToUpper();
                    string relOriginClassName = ((IDataset)rel.OriginClass).Name.ToUpper();

                    if (relatedTableName.ToUpper() == relDestinationClassName.ToUpper())
                    {
                        ESRI.ArcGIS.esriSystem.ISet resultSet = rel.GetObjectsRelatedToObject((ESRI.ArcGIS.Geodatabase.IObject)argpRow);

                        // Case 1 If there are related records 
                        // Case 2 If there are no related records                                                      
                        if (resultSet.Count > 0)
                        {
                            IRow pRelRow = (IRow)resultSet.Next();

                            while (pRelRow != null)
                            {
                                try
                                {
                                    iFieldIndex = pRelRow.Fields.FindField(relatedTableColumnName);

                                    if (iFieldIndex != -1)
                                    {
                                        pField = pRelRow.Fields.get_Field(iFieldIndex);
                                        pDomain = Common.GetDomain(pRelRow, pField);

                                        // Check the empty or Null value here , whether we need to check the empty value also from domain codes                                                                                           
                                        if (pDomain != null && (ICodedValueDomain)pDomain != null)
                                        {
                                            if (ChecktheGivenValueIsPresentInDomainValues(argstrValueToSave, (ICodedValueDomain)pDomain, ref strAllAcceptableValues))
                                            {
                                                // Check the value before final update , if the not null value is same then don't update
                                               
                                                bValueSaved = UpdateFieldValue(ref pRelRow, iFieldIndex, argstrValueToSave);

                                                bbreakloop = true;

                                                if (bValueSaved)
                                                {
                                                    comment = "Updated value for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID + " , Column Name : " + relatedTableColumnName + " , Value : " + argstrValueToSave;
                                                }
                                                else
                                                {
                                                    commentsToStore += ",Given value is same as DB value for Column Name:" + argstrGISColumnName;
                                                }

                                                Common._log.Info(comment);
                                                Common._log.Info(commentsToStore);

                                                try
                                                {
                                                    if (bValueSaved)
                                                    {
                                                        pRelRow.Store();
                                                        bUpdateSuccessfull = true;
                                                    }

                                                    comment = "Stored all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                    Common._log.Info(comment);
                                                }
                                                catch (System.Exception exp)
                                                {
                                                    comment = "Error occured while storing all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                    Common._log.Info(comment);                                                   
                                                }
                                            }
                                            else
                                            {
                                                // Add to log - The value given is not matching with the values from given domain name
                                                strAcceptableValueString = "EDGIS Acceptable values : " + strAllAcceptableValues;

                                                comment = "Given attribute value is not eligible to update in EDGIS. ";
                                                comment += "Feature Class : " + argstrgisFeatureclassName + "Globalid : " + argstrGLOBALID + ", Attribute Name : " + argstrGISColumnName + ", Attribute Value : " + argstrValueToSave + ", Comments : " + strAcceptableValueString;
                                                Common._log.Info(comment);

                                                MainClass._dtRejectedRecords.Rows.Add(argstrgisFeatureclassName, argstrGLOBALID, argStrOperatingNumber, argipb_rlid, argstrGISColumnName, argstrValueToSave, "EDGIS Acceptable values : " + strAcceptableValueString);

                                                commentsToStore += ",Value not present in EDGIS Domains for column : " + argstrGISColumnName + " " + strAcceptableValueString;                                                                
                                                               
                                                bbreakloop = true;
                                            }
                                        }
                                        else
                                        {
                                            bValueSaved = UpdateFieldValue(ref pRelRow, iFieldIndex, argstrValueToSave);

                                            bbreakloop = true;

                                            if (bValueSaved)
                                            {
                                                comment = "Updated value for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID + " , Column Name : " + relatedTableColumnName + " , Value : " + argstrValueToSave;
                                            }
                                            else
                                            {
                                                commentsToStore += ",Given value is same as DB value for Column Name:" + argstrGISColumnName;
                                            }
                                            
                                            Common._log.Info(comment);
                                            Common._log.Info(commentsToStore);

                                            try
                                            {
                                                if (bValueSaved)
                                                {
                                                    pRelRow.Store();
                                                    bUpdateSuccessfull = true;
                                                }                                               

                                                comment = "Stored all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                Common._log.Info(comment);
                                            }
                                            catch (System.Exception exp)
                                            {
                                                comment = "Error occured while storing all values for FeatureClass/Table Name : " + relatedTableName + ", OID : " + pRelRow.OID;
                                                Common._log.Info(comment);
                                                // throw;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        comment = "Given field as per mapping config is not present in EDGIS featureclass/Table.";
                                        comment += "Feature Class/Table : " + relatedTableName + "Globalid : " + argstrGLOBALID + ", Attribute Name : " + relatedTableColumnName + ", Attribute Value : " + argstrValueToSave;
                                        Common._log.Info(comment);
                                    }
                                }
                                catch (System.Exception exp)
                                {                                    
                                    Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                                }
                            
                                pRelRow = (IRow)resultSet.Next();
                            }

                            if (bUpdateSuccessfull || bbreakloop)
                            {
                                break;
                            }
                        }
                        else
                        {
                            comment = "No record found for given globalid.";
                            comment += "Feature Class/Table : " + relatedTableName + "Globalid : " + argstrGLOBALID + ", Attribute Name : " + relatedTableColumnName + ", Attribute Value : " + argstrValueToSave;
                            Common._log.Info(comment);
                        }
                    }
                    rel = relClassesEnum.Next();
                }
            }
            catch (System.Exception exp)
            {               
                bUpdateSuccessfull = false;
                Common._log.Error("Exception : " + exp.Message + " |  Stack Trace | " + exp.StackTrace + " at " + exp.ToString());
                ErrorCodeException ece = new ErrorCodeException(exp);
                Environment.ExitCode = ece.CodeNumber;
            }
            return bUpdateSuccessfull;
        }
    }
}
