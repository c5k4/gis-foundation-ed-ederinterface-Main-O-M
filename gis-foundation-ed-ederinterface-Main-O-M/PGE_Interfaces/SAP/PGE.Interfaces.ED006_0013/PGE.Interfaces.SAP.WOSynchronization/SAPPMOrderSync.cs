using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using log4net;
using PGE.Interfaces.Integration.Framework.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using PGE_DBPasswordManagement;



namespace PGE.Interfaces.SAP.WOSynchronization
{
    /// <summary>
    /// Class to process the messages and inserts/updates the SDE Geodatabase.
    /// </summary>
    public class SAPPMOrderSync
    {
        #region Private Variables
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        private log4net.ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Object that processes PMorder table data
        /// </summary>
        private PMOrderTableData _pmOrders;
        /// <summary>
        /// Object that deals with WIPCloud table data
        /// </summary>
        private TableData _WIPCloud;
        #endregion Private Variables

        #region Public Methods

        /// <summary>
        /// Process the messages in the <paramref name="pmOrderData"/> list and updated the SDE Geodatabase accordingly.
        /// </summary>
        /// <param name="pmOrderData">List of messages to process</param>
        public void OutPutData(List<IRowData2> pmOrderData)
        {
            string orderNumber = string.Empty;
            try
            {
                if (pmOrderData == null) return;
                //Checkout License
                if (LicenseManager.ChecketOutLicenses() == false)
                {
                    _logger.Info("ArcGIS & ArcFM License check-out failed. Hence exits the process.");
                    return;
                }

                _logger.Info("ArcGIS & ArcFM Licenses checked out.");
                //Open the Database using the credentials provided in configuration settings
               // string ConString = ConfigurationManager.AppSettings["WIPOracleConnectionString"].ToString();
             //   IWorkspace wSpace = ArcSdeWorkspaceFromFile(ConString);
                //Below code is commented for EDGIS Rearch Project 
               IWorkspace wSpace = GetSDEWorkSpace();
                if (wSpace == null)
                {
                    _logger.Debug("Could not connect to the SDE Database using the credentials provided in config file.");
                    return;
                }
                //Get the PMOrder and WIPCloud table names from config file
                string pmOrderTableName = Config.ReadConfigFromExeLocation().AppSettings.Settings["PMOrderName"].Value;
                _logger.Debug("'PMOrderName' config parameter read.");
                string WIPCloudTableName = Config.ReadConfigFromExeLocation().AppSettings.Settings["WIPCloudName"].Value;
                _logger.Debug("'WIPCloudName' config parameter read.");

                //Create an instance to process PMOrders
                _pmOrders = new PMOrderTableData(wSpace, pmOrderTableName);
                _pmOrders.JobOrderNumberFieldName = ResourceConstants.FieldNames.JOBORDER;
                _pmOrders.LastMessageDateTime = DateTime.Now;
                //Load the cache
                _pmOrders.LoadCache();

                //Create an instance to deal with WIPClouds
                _WIPCloud = new TableData(wSpace, WIPCloudTableName);
                _WIPCloud.JobOrderNumberFieldName = ResourceConstants.FieldNames.WIPJOBORDER;
                //Load the cache
                _WIPCloud.LoadCache();

                //Start editing the workspace
                IWorkspaceEdit wSpaceEdit = wSpace as IWorkspaceEdit;
                wSpaceEdit.StartEditing(false);

                _logger.Info(string.Format("Started inserting/updating PMOrders (Count={0})...", pmOrderData.Count));
                string wipMissingField = ResourceConstants.FieldNames.WIPMissing.ToUpper();
                string wipFoundField = ResourceConstants.FieldNames.WIPFound.ToUpper();
                IRowData2 pmOrder;
                Dictionary<string, string> fieldValues;
                string jobNumber;
                bool recordExists;
                bool wipPolygonFound;
                for (int i = 0; i < pmOrderData.Count; i++)
                {
                    pmOrder = pmOrderData[i];
                    fieldValues = pmOrder.FieldValues;

                    ///This check is already taken care
                    //if (fieldValues == null || !fieldValues.ContainsKey(ResourceConstants.XMLNodeNames.JOBORDERNODENAME))
                    //{
                    //    _logger.Debug(ResourceConstants.FieldNames.JOBORDER + " node is missing the XML file. Hence could not process this PMOrder");
                    //    continue;
                    //}

                    //Get the JObNumber of the PMOrder
                    jobNumber = fieldValues[ResourceConstants.XMLNodeNames.JOBORDERNODENAME];
                    orderNumber = jobNumber;
                    ////Set the current data field
                    //fieldValues.Add(ResourceConstants.FieldNames.LastMessageDate.ToUpper(), DateTime.Now.ToLongDateString());

                    //Checks whether any WIP Exists with given JobNumber
                    wipPolygonFound = _WIPCloud.RecordExists(jobNumber);

                    //Se the WIPFound and WIPMissing tags
                    if (wipPolygonFound)
                    {
                        fieldValues.Add(wipFoundField, "1");
                        fieldValues.Add(wipMissingField, "0");
                    }
                    else
                    {
                        fieldValues.Add(wipMissingField, "1");
                    }

                    //Check whether a record already exists with this ORDER number
                    recordExists = _pmOrders.RecordExists(jobNumber);

                    //Re-attach the field value collection to the PMORder
                    pmOrder.FieldValues = fieldValues;

                    //Insert PMorder or update the existing the one
                    if (recordExists)
                    {
                        _pmOrders.UpdateRow(pmOrder);
                    }
                    else
                    {
                        _pmOrders.InsertRow(pmOrder);
                    }

                    //Below  code is added to update process flag and processed time in ED13 staging table for EDGIS rearch Project -v1t8
                    PMOrderFileProcessor.UpdateED13StagingTable(jobNumber);                   
                   
                }

                _logger.Info("Completed inserting/updating PMOrders...");

                //Changed by v1t8
                //  if (Interface.Integration.Framework.Utilities.AppConfiguration.getBoolSetting("CreateCSVFiles", false))
                if (AppConfiguration.getBoolSetting("CreateCSVFiles", false))
                {
                    //Generate Data for exporting to CSV files 1. PMOrder without WIP 2. WIP without PMOrder
                    Export();
                }

                //Cleans the outdated PMOrders
                CleanOuteDatedPMOrders();

                //Save and Stop editing
                wSpaceEdit.StopEditing(true);

                _logger.Info("Saved the edits to the database.");

                //Release the workspace reference
                Marshal.ReleaseComObject(wSpace);
            }
            catch(Exception ex)
            {
                //Below  code is added to update process flag and error description in ED13 staging table for EDGIS rearch Project -v1t8
                _logger.Error("Exception occure while processing Job number: " + orderNumber  + ex.Message );
                 PMOrderFileProcessor.UpdateErrorDescription(orderNumber, ex.Message);                 
                 PMOrderFileProcessor.remark = "Exception occures while processing SAP data :" + " " + ex.Message + MethodBase.GetCurrentMethod().Name;
            }
            finally
            {
                //Shutdown the licenses
                LicenseManager.Shutdown();
                _logger.Info("ArcGIS & ArcFM Licenses checked-in if any checked-out already.");
                //Release the resources
                if (_pmOrders != null)
                {
                    _pmOrders.Dispose(); _pmOrders = null;
                }
                if (_WIPCloud != null)
                {
                    _WIPCloud.Dispose(); _WIPCloud = null;
                }
            }
        }

        /// <summary>
        /// Currently not implemented.
        /// </summary>
        /// <param name="pmJobOrderNo"></param>
        /// <returns></returns>
        public DataRow SearchPMOrder(string pmJobOrderNo)
        {
            return null;
        }

        /// <summary>
        /// Currently not implemented.
        /// </summary>
        /// <param name="pmJobOrderNo"></param>
        /// <returns></returns>
        public List<IRowData2> SearchWIPCloud(string pmJobOrderNo)
        {
            List<IRowData2> wipFeatures = new List<IRowData2>();
            //Get the Job Number of 
            return null;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Exports the report data to the CSV files
        /// </summary>
        private void Export()
        {
            try
            {
                //Get the Export File Directory
                string exportDirectory = Config.ReadConfigFromExeLocation().AppSettings.Settings["OutputDirectoy"].Value;
                _logger.Debug("'OutputDirectoy' config parameter read.");
                if (!System.IO.Directory.Exists(exportDirectory))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(exportDirectory);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error creating output directory.",ex);
                        _logger.Info("Failed to create directory : " + exportDirectory);
                        return;
                    }
                }

                //Get Date&Time to append at the end of exported CSV file name
                string dateTimeString = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                _logger.Info("Started exporting the PMOrders without WIP report...");
                //Get the PMOrders having no WIP Polygons
                string whereClause = ResourceConstants.FieldNames.WIPMissing + "=1";
                string[] fieldOrder = Config.ReadConfigFromExeLocation().AppSettings.Settings["PMOrderFieldOrder"].Value.Split(new char[] { ',' });
                _logger.Debug("'PMOrderFieldOrder' config parameter read.");
                IList<IRowData2> pmOrdersWithoutWIP = _pmOrders.SearchTable(whereClause, fieldOrder);
                //Prepare export CSV file name
                string exportFileName = System.IO.Path.Combine(exportDirectory, "PMOrdersWithoutWIP_" + dateTimeString + ".csv");
                CSVFileWriter.Export(pmOrdersWithoutWIP, exportFileName);
                _logger.Info("Completed exporting the PMOrders without WIP report...");

                _logger.Info("Started exporting the WIP without PMOrders report...");
                //Get WIP Polygons without PMorders
                //Get the JobNumber of all PMorders
                string allJobNumbers = _pmOrders.GetAllJobNumbers();
                if (string.IsNullOrEmpty(allJobNumbers)) whereClause = string.Empty;
                else
                {
                    // whereClause = _WIPCloud.JobOrderNumberFieldName + " not in (" + allJobNumbers + ")";
                    whereClause = _WIPCloud.JobOrderNumberFieldName + " NOT IN (SELECT " + _pmOrders.JobOrderNumberFieldName + " FROM " + _pmOrders.TableName + " )";
                }
                fieldOrder = Config.ReadConfigFromExeLocation().AppSettings.Settings["WIPFieldOrder"].Value.Split(new char[] { ',' });
                _logger.Debug("'WIPFieldOrder' config parameter read.");
                IList<IRowData2> wipsWithoutPMOrder = _WIPCloud.SearchTable(whereClause, fieldOrder);
                //Prepare export CSV file name
                exportFileName = System.IO.Path.Combine(exportDirectory, "WIPsWithoutPMOrder_" + dateTimeString + ".csv");
                CSVFileWriter.Export(wipsWithoutPMOrder, exportFileName);
                _logger.Info("Completed exporting the WIP without PMOrders report...");
            }
            catch (Exception ex)
            {
                _logger.Error("Error exporting reports.",ex);
            }
        }

        /// <summary>
        /// Cleans the outdated PMOrder records from the Database
        /// </summary>
        private void CleanOuteDatedPMOrders()
        {
            try
            {
                _logger.Info("Started cleaning the outdated PMOrders without WIP...");
                //Get the MaxAge from the config file
                string maxAgeValue = Config.ReadConfigFromExeLocation().AppSettings.Settings["MaxAge"].Value;
                _logger.Debug("'MaxAge' config parameter read.");
                //Prepare where clause
                string whereClause = ResourceConstants.FieldNames.LastMessageDate + "< (sysdate-" + maxAgeValue + ")";
                whereClause += " and " + ResourceConstants.FieldNames.WIPMissing + "=1";
                _pmOrders.CleanRecords(whereClause);
                _logger.Info("Completed cleaning the outdated PMOrders without WIP.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error deleting old PM Orders.",ex);
            }
        }

        /// <summary>
        /// Retrieves the Database workspace by reading the database credentials from the config file.
        /// </summary>
        /// <returns>Returns the reference to the Database</returns>
        private IWorkspace GetSDEWorkSpace()
        {
            string dbConn = default;
            string password = default;
            string user = default;
            try
            {
                // m4jf edgisreach 919 - get sde connection parameters using PGE_DBPasswordMnagement
                string[] UserInst = ConfigurationManager.AppSettings["WIP_SDEConnection"].ToString().Split('@');
                password = ReadEncryption.GetPassword(UserInst[0] + "@" + UserInst[1]);
                dbConn = "sde:oracle11g:/;local=" + UserInst[1].ToString();
                user = UserInst[0].ToString();
                KeyValueConfigurationCollection appSettings = Config.ReadConfigFromExeLocation().AppSettings.Settings;
                IPropertySet propertySet = new PropertySetClass();
                //propertySet.SetProperty("instance", appSettings["DBConnec"].Value);
                //propertySet.SetProperty("User", appSettings["UserName"].Value);
                //propertySet.SetProperty("Password", appSettings["Password"].Value);
                //m4jf
                 propertySet.SetProperty("instance", dbConn);
                propertySet.SetProperty("User", user);
                propertySet.SetProperty("Password", password);

                propertySet.SetProperty("version", appSettings["WIPCloudVersion"].Value);

                //propertySet.SetProperty("instance", "EDGWM1D");
                //propertySet.SetProperty("User", "webr");
                //propertySet.SetProperty("Password", "webr#WM1D");
                //propertySet.SetProperty("version", "SDE.DEFAULT");
               // propertySet.SetProperties("server", "DVEDGMDBOLX001");
                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
                IWorkspace wspace = workspaceFactory.Open(propertySet, 0);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                return wspace;
            }
            catch (Exception ex)
            {
                _logger.Error("Error getting workspace.",ex);
                return null;
            }

        }

        /// <summary>
        /// To Get the IworkSpace
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <returns></returns>
        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            IWorkspace workspace = null;
            try
            {
              //  IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactory();
             //   workspace= workspaceFactory.OpenFromFile(connectionFile, 0);
              workspace =((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).OpenFromFile(connectionFile, 0);
            }
            catch (Exception ex) 
            { 
            }
            return workspace;

        }

        #endregion Private Methods
    }
}
