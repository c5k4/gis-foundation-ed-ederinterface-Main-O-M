using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using Oracle.DataAccess.Client;
using Telvent.PGE.ED.Desktop.AutoUpdaters.Special;

namespace DeleteFeatures
{
    class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new DeleteFeatures.LicenseInitializer();
        private static string sPath = (String.IsNullOrEmpty(ConfigurationManager.AppSettings["LOGPATH"]) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : ConfigurationManager.AppSettings["LOGPATH"]) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static StreamWriter pSWriter = default(StreamWriter);
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitialize();
        private static string sArg = string.Empty, strSdeVerName = string.Empty;
        static void Main(string[] args)
        {
            if (args.Length > 0)
                sArg = args[0];
            (pSWriter = File.CreateText(sPath)).Close();
            List<string> issueList = new List<string>();

            Console.WriteLine("----Update Features In FC's-----");

            //string issueNumber = Console.ReadLine(); // Read string from console                

            //Console.WriteLine("\n");
            Console.WriteLine("\n");

            string strEDGISSdeConn = ConfigurationManager.AppSettings["CONN_EDGIS_FILE"].ToString();
            strSdeVerName = (string.IsNullOrEmpty(sArg) ? ConfigurationManager.AppSettings["SESSION_NAME"] : ConfigurationManager.AppSettings["SESSION_NAME"] + "_" + sArg);
            Console.WriteLine("Sde Connection Parameter :" + strEDGISSdeConn);
            Console.WriteLine("Sde Version Name Parameter :" + strSdeVerName);

            //Console.WriteLine("Please confirm all above important details, before proceed (Y/N):");
            //string strConfirm = Console.ReadLine().ToUpper(); // Read string from console

            //if (strConfirm == "N") // Try to parse the string as an integer
            //{
            //    Console.Write("Change sde connection string in configuration file and try again");
            //    return;
            //}
            //else if (strConfirm != "N" && strConfirm != "Y") // Try to parse the string as an integer
            //{
            //    Console.WriteLine("invalid input value, plz try again");
            //    return;

            //}

            //ESRI License Initializer generated code.
            WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
            m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced },
            new esriLicenseExtensionCode[] { });
            mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
            if (RuntimeManager.ActiveRuntime == null)
                RuntimeManager.BindLicense(ProductCode.Desktop);
             WriteLine(DateTime.Now.ToLongTimeString() + " -- Licences Initialized");
            //Process feature class wise

            List<string> lstFCNames = new System.Collections.Generic.List<string>();
            //ConfigurationManager.AppSettings["
            //Miner.Interop.Process.IMMSessionManager man=new            

            //ProcessSUPCorrection();
            //EDGIS.ElectricStitchPoint,EDGIS.CurcuitSource
            //Hashtable[] htTBLExcelData = ReadDataFromXls();
            //ProcessCircuitData(htTBLExcelData[0], "EDGIS.ElectricStitchPoint", true);
            //ProcessCircuitData(htTBLExcelData[1], "EDGIS.CircuitSource", false);

            UpdateFeatures();

        }



        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
        private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        {
            mmLicenseStatus licenseStatus;
            licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
            if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                licenseStatus = arcFMAppInitialize.Initialize(productCode);
            }
            return licenseStatus;
        }
        private static void UpdateFeatures()
        {
            IVersion pVersion = default(IVersion);
            IFeature pFeat = default(IFeature);
            IFeatureCursor pFCursor = default(IFeatureCursor);
            IQueryFilter pQFilter = new QueryFilterClass();
            IFeatureClass pFClass = default(IFeatureClass);
            //IFeature pFeat = default(IFeature);
            IFeatureCursor pFeatureCursor = default(IFeatureCursor);
            ICursor pCursor = default(ICursor);
            

            //List<string> lstTargetFCNames = new System.Collections.Generic.List<string>();
            IMMAutoUpdater autoupdater = default(IMMAutoUpdater);
            mmAutoUpdaterMode oldMode = mmAutoUpdaterMode.mmAUMArcMap;
            string strLastUser = string.Empty, strVersionName = string.Empty;

            try
            {
                DataSet pDSet = GetRecordtoProcess();


                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Connection " + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"] + " to provided Version " + strSdeVerName);
                IWorkspace pFWSpace = ArcSdeWorkspaceFromFile(ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Workspace initialized" + ConfigurationManager.AppSettings["CONN_EDGIS_FILE"]);
                try
                {
                    pVersion = ((IVersionedWorkspace)pFWSpace).FindVersion(strSdeVerName);
                }
                catch { }
                if (pVersion != null)
                    pVersion.Delete();
                pVersion = ((IVersionedWorkspace)pFWSpace).DefaultVersion.CreateVersion(strSdeVerName);
                pVersion.Access = esriVersionAccess.esriVersionAccessPublic;

                WriteLine(DateTime.Now.ToLongTimeString() + " -- Version Created" + strSdeVerName);

                string pFCName = ConfigurationManager.AppSettings["UPDATE_FEATURE_CLASS"];
                string pSourceTableName = ConfigurationManager.AppSettings["SourceTable"];  
                pFClass = (pVersion as IFeatureWorkspace).OpenFeatureClass(pFCName);
                int targetMeasuredLatIndex = pFClass.FindField("MEASUREDLATITUDE");
                int targetMeasuredLongIndex = pFClass.FindField("MEASUREDLONGITUDE");
                int targetMeasuredPositionSource = pFClass.FindField("MEASUREDPOSITIONSOURCE");
                int targetGUIDIndex = pFClass.FindField("GLOBALID");
                //started processing FC's
                WriteLine(DateTime.Now.ToLongTimeString() + " Started processing " + pFCName);
                //To Write change records to GIS-SAP Staging table....
               
                string strGUIDValue = string.Empty;
                ((IWorkspaceEdit)pVersion).StartEditing(true);
                ((IWorkspaceEdit)pVersion).StartEditOperation();
                int iCount = 0;

                #region Commented
                ////Manish
                //// Create the query definition.
                //IQueryDef queryDef = (pVersion as IFeatureWorkspace).CreateQueryDef();

                //// Provide a list of tables to join.
                //queryDef.Tables = pFCName + "," + pSourceTableName;

                //// Set the subfields and the WhereClause (in this case, the join condition).
                //queryDef.SubFields =
                //  "EDGIS.PLDBLoadingData.GLOBALID,EDGIS.PLDBLoadingData.MEASUREDLATITUDE,EDGIS.PLDBLoadingData.MEASUREDLONGITUDE,EDGIS.PLDBLoadingData.MEASUREDSOURCE, EDGIS.SupportStructure.OBJECTID";
                //queryDef.WhereClause = "EDGIS.PLDBLoadingData.GLOBALID = EDGIS.SupportStructure.GLOBALID";

                //// Get a cursor of the results and find the indexes of the fields to display.
                //ICursor cursor = queryDef.Evaluate();
                //int measuredLatitudeIndex = cursor.FindField("EDGIS.PLDBLoadingData.MEASUREDLATITUDE");
                //int measuredLongitudeIndex = cursor.FindField("EDGIS.PLDBLoadingData.MEASUREDLONGITUDE");
                //int measuredSourceIndex = cursor.FindField("EDGIS.PLDBLoadingData.MEASUREDSOURCE");
                //int globalIDIndex = cursor.FindField("EDGIS.PLDBLoadingData.GLOBALID");
                //int objectIDIndex = cursor.FindField("EDGIS.SupportStructure.OBJECTID");

                //// Use the cursor to step through the results, displaying the names and altnames of each street.
                //IRow row = null;
                //int iCount = 0;
                //if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
                //{
                //    while ((row = cursor.NextRow()) != null)
                //    {
                //        try
                //        {
                //            iCount++;
                //            int ObjectIDFldValue = Convert.ToInt32(row.get_Value(objectIDIndex));
                //            double latValue = Convert.ToDouble(row.get_Value(measuredLatitudeIndex));
                //            double longValue = Convert.ToDouble(row.get_Value(measuredLongitudeIndex));
                //            string measuredSource = row.get_Value(measuredSourceIndex).ToString();
                //            //string measuredSourceValue = GetValueFromDomain((IObject)row, pMeasuredSourceField);
                //            string globalIDValue = row.get_Value(globalIDIndex).ToString();
                //            //Console.WriteLine("Source Data\n: Lat:{0} Long:{1} MeasuredSource:{2} GlobalID:{3} - Suppoprt Structure:\n ObjectID: {4}.",
                //            //  latValue, longValue,
                //            //  measuredSource, globalIDValue, ObjectIDFldValue);
                //            IFeature pFeature = pFClass.GetFeature(ObjectIDFldValue);
                //            //IFeature pFeature = pFClass.GetFeature(12786129);
                //            pFeature.set_Value(targetMeasuredLatIndex, latValue);
                //            pFeature.set_Value(targetMeasuredLongIndex, longValue);
                //            pFeature.set_Value(targetMeasuredPositionSource, measuredSource);
                //            //pFeature.set_Value(targetMeasuredPositionSource, measuredSourceValue);
                //            pFeature.Store();

                //            if (iCount % 5000 == 0)
                //            {
                //                ((IWorkspaceEdit)pVersion).StopEditOperation();
                //                ((IWorkspaceEdit)pVersion).StopEditing(true);
                //                ((IWorkspaceEdit)pVersion).StartEditing(true);
                //                ((IWorkspaceEdit)pVersion).StartEditOperation();

                //            }
                //            WriteLine("Executing Feature Number: " + iCount);
                //        }
                //        catch (Exception ex)
                //        {
                //            WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing features " + ex.Message);
                //        }


                //    }
                //    ((IWorkspaceEdit)pVersion).StopEditOperation();
                //    autoupdater.AutoUpdaterMode = oldMode;
                //    ((IWorkspaceEdit)pVersion).StopEditing(true);
                //    WriteLine(DateTime.Now.ToLongTimeString() + " Total " + pFCName + " features processed : " + iCount);
                //}
                #endregion

                if (DisableAutoUpdaterFramework(out autoupdater, out oldMode))
                foreach (DataRow dRow in pDSet.Tables[0].Rows)
                {
                    ++iCount;
                    try
                    {
                        pQFilter.WhereClause = "GLOBALID = '" + dRow["GLOBALID"] + "'";
                        pFCursor = pFClass.Update(pQFilter, false);
                        pFeat = pFCursor.NextFeature();
                        if (pFeat == null)
                        {
                            WriteLine(DateTime.Now.ToLongTimeString() + " Didn't find Feature : " + iCount.ToString() + " " + pQFilter.WhereClause);
                            continue;
                        }
                        pFeat.set_Value(targetMeasuredLatIndex, dRow["MEASUREDPOSITIONLATITUDE"]);
                        pFeat.set_Value(targetMeasuredLongIndex, dRow["MEASUREDPOSITIONLONGITUDE"]);
                        pFeat.set_Value(targetMeasuredPositionSource, dRow["MEASUREDSOURCE"]);
                        PGEUpdateBearingAndLength pAU_Explicit = new PGEUpdateBearingAndLength();
                        pAU_Explicit.AUMain_Public(pFeat, mmEditEvent.mmEventFeatureUpdate);
                        pFeat.Store();
                        Marshal.FinalReleaseComObject(pFeat);
                        Marshal.FinalReleaseComObject(pFCursor);
                        if (iCount % 1000 == 0)
                        {
                            ((IWorkspaceEdit)pVersion).StopEditOperation();
                            ((IWorkspaceEdit)pVersion).StopEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditing(true);
                            ((IWorkspaceEdit)pVersion).StartEditOperation();
                            WriteLine(DateTime.Now.ToLongTimeString() + " Resting at : " + iCount + " " + pQFilter.WhereClause);
                            GC.Collect();
                        }
                        WriteLine(DateTime.Now.ToLongTimeString() + " Executing Feature : " + iCount.ToString() + " " + pQFilter.WhereClause);
                    }
                    catch (Exception ex1)
                    {
                        if (pFCursor != null) Marshal.ReleaseComObject(pFCursor);
                        WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing individual " + dRow["GLOBALID"] + " -- " + ex1.Message);
                    }
                }
                ((IWorkspaceEdit)pVersion).StopEditOperation();
                
                ((IWorkspaceEdit)pVersion).StopEditing(true);
                //WriteLine(DateTime.Now.ToLongTimeString() + " Total " + pFCName + " features processed : " + iCount);                              
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error while Processing features " + ex.Message);

            }
            finally
            {
                autoupdater.AutoUpdaterMode = oldMode;
                if (pVersion != null) Marshal.ReleaseComObject(pVersion);
                if (pFeat != null) Marshal.ReleaseComObject(pFeat);
                if (pFCursor != null) Marshal.ReleaseComObject(pFCursor);
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
                if (pQFilter != null) Marshal.ReleaseComObject(pQFilter);
                if (pFClass != null) Marshal.ReleaseComObject(pFClass);
                //if (pRow != null) Marshal.ReleaseComObject(pRow);
                if (pFeatureCursor != null) Marshal.ReleaseComObject(pFeatureCursor);
                if (autoupdater != null) Marshal.ReleaseComObject(autoupdater);
                //if (oldMode != null) Marshal.ReleaseComObject(oldMode);
            }

        }

        private static DataSet GetRecordtoProcess()
        {
            WriteLine(DateTime.Now.ToLongTimeString() + " -- GetRecordtoProcess");
            DataSet pDSet = new DataSet();
            OracleConnection pOConn = new OracleConnection(ConfigurationManager.AppSettings["CONNECTION_STRING"]);
            try
            {   
                pOConn.Open();

                OracleDataAdapter pODAdaptor = new OracleDataAdapter(new OracleCommand() { Connection = pOConn, CommandType = CommandType.Text, CommandText = "SELECT '{'||UPPER(GLOBALID)||'}' GLOBALID, MEASUREDPOSITIONLATITUDE, MEASUREDPOSITIONLONGITUDE, MEASUREDSOURCE FROM RELEDITOR.LIDAR_DATA WHERE ID LIKE '%" + sArg + "'" });
                pODAdaptor.Fill(pDSet);
                WriteLine(DateTime.Now.ToLongTimeString() + " -- GetRecordtoProcess Record Count" + pDSet.Tables[0].Rows.Count.ToString());
                return pDSet;
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Error while GetRecordtoProcess" + ex.Message);
                return pDSet;
            }
            finally
            {
                pOConn.Close();
            }
        }

        private string GetValueFromDomain(IObject iObj, IField field)
        {
            string valueFromDomain = null;

            try
            {
                if (iObj == null || field == null)
                {
                    return valueFromDomain;
                }

                //Get the field value
                valueFromDomain = iObj.get_Value(iObj.Fields.FindField(field.Name)).ToString();

                //Get the domain attached to the field
                IDomain domain = field.Domain;
                if (domain == null || !(domain.GetType() is ICodedValueDomain))
                {
                    return valueFromDomain;
                }

                ICodedValueDomain codeValueDomain = domain as ICodedValueDomain;
                if (codeValueDomain == null)
                {
                    return valueFromDomain;
                }

                int codeCount = codeValueDomain.CodeCount;
                if (codeCount < 1)
                {
                    return valueFromDomain;
                }

                //Get domain description matching domain code
                for (int i = 0; i < codeCount; i++)
                {
                    if (codeValueDomain.get_Value(i).ToString() == valueFromDomain)
                    {
                        valueFromDomain = codeValueDomain.get_Name(i);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " : " + ex.Message);
                
            }
            return valueFromDomain;
        }
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            return ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                OpenFromFile(connectionFile, 0);
        }
        private static bool DisableAutoUpdaterFramework(out IMMAutoUpdater autoupdater, out mmAutoUpdaterMode oldMode)
        {
            string strDisableAU = ConfigurationManager.AppSettings["DISABLE_AU_FRAMEWORK"].ToString();

            try
            {
                Type type = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
                object obj = Activator.CreateInstance(type);
                autoupdater = obj as IMMAutoUpdater;
                oldMode = autoupdater.AutoUpdaterMode;
                //disable all ArcFM Autoupdaters 
                if (Convert.ToBoolean(strDisableAU))
                {
                    autoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                    return true;
                }
                else
                {

                    return true;
                }
            }
            catch (Exception)
            {
                //WriteLine(DateTime.Now + "");
                //throw;
                WriteLine(DateTime.Now.ToLongTimeString() + " Error in disabling Auto Updaters. ");
            }
            autoupdater = null;
            oldMode = mmAutoUpdaterMode.mmAUMStandAlone;
            return false;
        }



    }


}
