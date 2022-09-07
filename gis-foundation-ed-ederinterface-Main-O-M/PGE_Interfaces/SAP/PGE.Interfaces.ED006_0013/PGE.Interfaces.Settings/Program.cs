using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

using PGE.Interfaces.SAP;
using PGE.Interfaces.Integration.Framework;

namespace PGE.Interfaces.Settings
{
    class Program
    {
        private static esriLicenseProductCode[] _prodCodes = { esriLicenseProductCode.esriLicenseProductCodeAdvanced };
        private static LicenseInitializer _licenceManager = new LicenseInitializer();
        private const string CONTROLLER_CLASSNAME = "EDGIS.Controller";


        static void Main(string[] args)
        {
            IWorkspace workSpace = null;
            IFeatureWorkspace fWorkspace = null;
            SettingsDataHelper sd = new SettingsDataHelper();
            DateTime currentDate = DateTime.Now;

            try 
            {
                if (CheckLicense(_licenceManager))
                {
                    Console.WriteLine("Processing Settings Data...");
                    Common.WriteToLog("Processing Settings Data...", LoggingLevel.Info);
                    Common.WriteToLog("Start time : " + DateTime.Now.ToLocalTime(), LoggingLevel.Info);

                    // Get data for SAP
                   
                    DateTime? lastRunDate = sd.GetLastRunDate();
                    Dictionary<string, string> data = sd.GetDataForSAP(lastRunDate); //Data to be processed


                    #region Controller Data Processing

                    Dictionary<string, string> ControllerData = new Dictionary<string, string>();
                    string[] allGUIDs = data.Where(v => !string.IsNullOrEmpty(v.Key)).Select(k => k.Key).Distinct().ToArray();

                    //Since IN clause at the query cannot take more than 1000 values, create list of comma seperated guids
                    List<string> allGUIDsList = Common.GetCommaSeperatedValues(allGUIDs);
                    List<IRow> rows = new List<IRow>();

                    try
                    {
                        try
                        {
                            if (workSpace == null) { workSpace = Common.SetWorkspace(); }
                            if (fWorkspace == null) { fWorkspace = workSpace as IFeatureWorkspace; }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("SDE connection error.");
                            Common.WriteToLog("SDE connection error. " + ex.Message + Environment.NewLine + ex.StackTrace, LoggingLevel.Error);
                            Environment.Exit(1);
                        }

                        ITable controllerTable = fWorkspace.OpenTable(CONTROLLER_CLASSNAME);
                        IObjectClass controllerClass = controllerTable as IObjectClass;

                        ICursor cursor = null;
                        

                        foreach (string guids in allGUIDsList)
                        {
                            try
                            {
                                IRow row = null;
                                IQueryFilter queryFilter = new QueryFilterClass();
                                queryFilter.WhereClause = string.Format("DEVICEGUID IN ({0})", guids);

                                cursor = controllerTable.Search(queryFilter, false);
                                while ((row = cursor.NextRow()) != null)
                                {
                                    rows.Add(row);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error in creating object row list for " + CONTROLLER_CLASSNAME);
                                Common.WriteToLog("Error in creating object row list for " + CONTROLLER_CLASSNAME + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, LoggingLevel.Error);
                                Environment.Exit(1);
                            }
                            finally
                            {
                                if (cursor != null) { while (Marshal.ReleaseComObject(cursor) > 0); cursor = null; }
                            }
                        }

                        IEnumerable<IRow> changedRows = rows as IEnumerable<IRow>;

                        //We are not comparing versions to get the difference. So both edit and target versions are default version.
                        GISSAPIntegrator gisSAPIntegrator = new GISSAPIntegrator((workSpace as IVersionedWorkspace).DefaultVersion, (workSpace as IVersionedWorkspace).DefaultVersion);

                        if (gisSAPIntegrator.Initialize()) //Initialize() will load all configuration XMLs.
                        {
                            Console.WriteLine("GISSAPIntegrator initialized...");
                            Common.WriteToLog("GISSAPIntegrator initialized...", LoggingLevel.Info);

                            if (rows.Count > 0)
                            {
                                Console.WriteLine("Processing Controller Data for SAP integration...");
                                Common.WriteToLog("Processing Controller Data  for SAP integration...", LoggingLevel.Info);

                                Dictionary<string, Dictionary<string, IRowData>> assetIDandDataByClass = gisSAPIntegrator.UpdateSettingsData(CONTROLLER_CLASSNAME, changedRows);

                                if (assetIDandDataByClass != null)
                                {
                                    AssetProcessor assetProcessor = new AssetProcessor();
                                    try
                                    {
                                        Console.WriteLine("Running AssetProcessor...");
                                        Common.WriteToLog("Running AssetProcessor...", LoggingLevel.Info);

                                        foreach (Dictionary<string, IRowData> classAssetIDandData in assetIDandDataByClass.Values)
                                        {
                                            foreach (KeyValuePair<string, IRowData> row in classAssetIDandData)
                                            {
                                                //Process and save into database
                                                assetProcessor.Process(row);
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        assetProcessor.Dispose();
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("No controller found.");
                                Common.WriteToLog("No controller found.", LoggingLevel.Info);
                            }
                        }
                        else
                        {
                            Console.WriteLine("GISSAPIntegrator could not be initialized. Program ended!");
                            Common.WriteToLog("GISSAPIntegrator could not be initialized. Program ended!", LoggingLevel.Error);
                            Environment.Exit(1);
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing " + CONTROLLER_CLASSNAME + ". Program ended!");
                        Common.WriteToLog("Error processing " + CONTROLLER_CLASSNAME + ". Program ended!" + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, LoggingLevel.Error);
                        Environment.Exit(1);
                    }
                    finally
                    {
                        if (workSpace != null) { while (Marshal.ReleaseComObject(workSpace) > 0); workSpace = null; }
                        if (fWorkspace != null) { while (Marshal.ReleaseComObject(fWorkspace) > 0); fWorkspace = null; }
                    }

                    
                    #endregion
                }
                else
                {
                    Environment.Exit(1);
                }

                //Update last run date
                sd.UpdateLastRunDate(currentDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred. Program ended!");
                Common.WriteToLog("Error occurred. Program ended! " + Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace, LoggingLevel.Error);
                Environment.Exit(1);
            }
            finally
            {
                _licenceManager.ShutdownApplication();
            }

            Console.WriteLine("Done...!");
            Common.WriteToLog("Done...!", LoggingLevel.Info);
            Common.WriteToLog("End time : " + DateTime.Now.ToLocalTime(), LoggingLevel.Info);
            Environment.Exit(0);
        }


        /// <summary>
        /// Initializes ArcGIS license
        /// </summary>
        /// <param name="licenceManager"></param>
        /// <returns></returns>
        private static bool CheckLicense(LicenseInitializer licenceManager)
        {
            bool isOk = false;

            try
            {
                isOk = licenceManager.InitializeApplication(_prodCodes);
                //isOk = licenceManager.GetArcFMLicense(Miner.Interop.mmLicensedProductCode.mmLPArcFM);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize license : " + ex.Message);
                Common.WriteToLog("Failed to initialize license : " + ex.Message, LoggingLevel.Error);
            }

            return isOk;
        }
    }
}
