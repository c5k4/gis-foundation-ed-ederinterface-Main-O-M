using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Reflection;
using System.Xml;
using log4net;
using Oracle.DataAccess.Client;
using PGE.Common.Delivery.Systems;
using System.Configuration;
using PGE_DBPasswordManagement;

namespace PGE.Interfaces.SAP.WOSynchronization
{
    /// <summary>
    /// Extracts messages from each XML file after validating the XML file against its schema  and then process the messages
    /// </summary>
    public class PMOrderFileProcessor
    {
        #region Private Variables

        /// <summary>
        /// Collection PMOrder messages to be processed
        /// </summary>
        private List<IRowData2> _lstPMOrder = new List<IRowData2>();
        /// <summary>
        /// Logger to log error / debug/ user information
        /// </summary>
        private static ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public  static string remark = string.Empty;
        private static string ConString = string.Empty;


        #endregion Private Variables

        #region Public Methods

        /// <summary>
        /// Process the messages in XML files- This Method is no more needed for EDGIs Rearch Project. As input will come via layer 7 and stored in staging table by v1t8
        /// </summary>
        /// <param name="fileNames">Array of XML files to process</param>
        public void ProcessFiles(string[] fileNames)
        {
            XmlDocument xDoc;
            XMLProcessor objXmlProcessor;
            List<IRowData2> lstPMOrder;
            try
            {
                //Read the Schema content and validate
                string schemaContent = ReadXSDFile();
                if (string.IsNullOrEmpty(schemaContent))
                {
                    _logger.Debug("Content of XSD Schema file is empty.");
                    return;
                }

                //Loop through each XML file and process
                for (int fileIndex = 0; fileIndex < fileNames.Length; fileIndex++)
                {
                    //Open the XML File
                    string fileName = fileNames[fileIndex];

                    xDoc = new XmlDocument();
                    xDoc.Load(fileName);

                    //Validate the XML against the XSD file
                    if (XmlFacade.ValidateXML(xDoc.InnerXml, schemaContent, true) == false)
                    {
                        _logger.Info(string.Format("'{0}' validation failed against the XSD file.", fileName));
                        continue;
                    }
                    _logger.Info(string.Format("'{0}' file successfully validated against the XSD file.", fileName));

                    //Get the top XML Node i.e. ORDERS
                    XmlNode jobOrderAllNodes = xDoc.SelectSingleNode("ORDERS");
                    if (jobOrderAllNodes == null)
                    {
                        _logger.Info(string.Format("'ORDERS' node is not present in '{0}' file.", fileName));
                        continue;
                    }
                    _logger.Info(string.Format("Started retrieving messages from '{0}' file...", fileName));
                    //Create an object that converts messages to List
                    objXmlProcessor = new XMLProcessor();
                    lstPMOrder = objXmlProcessor.ConvertToIRowData(jobOrderAllNodes);
                    //Close the XML Document
                    xDoc = null;

                    //Add the message list from the current XML file to the existing private list
                    if (lstPMOrder != null && lstPMOrder.Count > 0)
                    {
                        if (_lstPMOrder.Count == 0) _lstPMOrder = lstPMOrder;
                        else
                        {
                            for (int i = 0; i < lstPMOrder.Count; i++)
                            {
                                //If the Message already exists then do not add else add
                                //Equality of ListItem is checked using 'IRowData2.FacilityID' i.e. JobNumber
                                if (_lstPMOrder.Contains(lstPMOrder[i])) continue;
                                _lstPMOrder.Add(lstPMOrder[i]);
                            }
                        }
                    }
                    else
                    {
                        _logger.Info(string.Format("'{0}' file does not have any message to process.", fileName));
                    }
                    _logger.Info(string.Format("Completed retrieving messages from '{0}' file.", fileName));
                }
                //Process the entire message list at once
                if (_lstPMOrder.Count > 0)
                {
                    _logger.Info("Started processing the messages....");
                    SAPPMOrderSync objSAPSync = new SAPPMOrderSync();
                    objSAPSync.OutPutData(_lstPMOrder);
                    _logger.Info("Completed processing the messages.");
                }
                else
                {
                    _logger.Info("No messages to process.");
                }

                //Archive the files
                foreach (string file in fileNames)
                {
                    //Archive the file
                    ArchiveFile(file);
                }
                _logger.Info("Archived the XML files.");

            }
            catch (Exception ex)
            {
                _logger.Error("Error processing files.", ex);
            }
        }

        /// <summary>
        /// Method is to process the SAP data- EDGIS Rearch project(v1t8)
        /// </summary>
        public void ProcessSAPData()
        {
            XmlDocument xDoc;
            XMLProcessor objXmlProcessor;
            List<IRowData2> lstPMOrder;
            try
            {
                _logger.Info("Starting" + MethodBase.GetCurrentMethod().Name);

                //Below code is added for EDGIS Rearch Project to read input data from staging table in place of file-v1t8
                //To get xmlDocument from staging table
                _logger.Info("Getting XML document from the table");
                xDoc = TableToXml();

                //Read the Schema content and validate
                string schemaContent = ReadXSDFile();
                if (string.IsNullOrEmpty(schemaContent))
                {
                    _logger.Debug("Content of XSD Schema file is empty.");
                    return;
                }

                //Below code is added for EDGIS Rearch Project to read input data from staging table in place of file-v1t8
                //Get the top XML Node i.e. ROWSET
                _logger.Info("Get the top XML Node i.e. ROWSET");
                XmlNode jobOrderAllNodes = xDoc.SelectSingleNode("ROWSET");
                if (jobOrderAllNodes == null)
                {
                    _logger.Info(string.Format("'ROWSET' node is not present in  file."));

                }

                //Create an object that converts messages to List
                objXmlProcessor = new XMLProcessor();
                _logger.Info("calling XMLProcessor.ConvertToIRowData");
                lstPMOrder = objXmlProcessor.ConvertToIRowData(jobOrderAllNodes);
                //Close the XML Document
                xDoc = null;

                //Add the message list from the current XML file to the existing private list
                if (lstPMOrder != null && lstPMOrder.Count > 0)
                {
                    if (_lstPMOrder.Count == 0) _lstPMOrder = lstPMOrder;
                    else
                    {
                        for (int i = 0; i < lstPMOrder.Count; i++)
                        {
                            //If the Message already exists then do not add else add
                            //Equality of ListItem is checked using 'IRowData2.FacilityID' i.e. JobNumber
                            if (_lstPMOrder.Contains(lstPMOrder[i])) continue;
                            _lstPMOrder.Add(lstPMOrder[i]);
                        }
                    }
                }
                else
                {
                    _logger.Info(string.Format("SAP data does not have any message to process."));
                }

                //Process the entire message list at once
                if (_lstPMOrder.Count > 0)
                {
                    _logger.Info("Started processing the messages....");
                    SAPPMOrderSync objSAPSync = new SAPPMOrderSync();
                    objSAPSync.OutPutData(_lstPMOrder);
                    _logger.Info("Completed processing the messages.");
                }
                else
                {
                    _logger.Info("No messages to process.");
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Exception occures while processing SAP data :" + " " + ex.Message + MethodBase.GetCurrentMethod().Name);
                 remark = "Exception occures while processing SAP data :" + " " + ex.Message + MethodBase.GetCurrentMethod().Name;
            }
            _logger.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
        }

        /// <summary>
        /// This method is to update process flag and processedtime in Ed13 staging table
        /// </summary>
        /// <param name="orderNumber"></param>
        public static void UpdateED13StagingTable(string orderNumber)
        {
            _logger.Info("Starting" + MethodBase.GetCurrentMethod().Name);

            try
            {
                // current date time for PROCESSEDTIME field
                string dateTime = DateTime.Now.ToString();

                //sql quert to execute
                string CmdString = "update " + ConfigurationManager.AppSettings["ED13StagingTable"].ToString() + " Set " + ConfigurationManager.AppSettings["PROCESSEDTIME"].ToString() + "= '" + dateTime + "' ," + ConfigurationManager.AppSettings["PROCESSEDFLAG"].ToString() + " = '" + ProcessFlag.Completed + "' where " + "\"" + "ORDER" + "\"=" + "'" + orderNumber + "'";
                if (ConString != string.Empty)
                {
                    _logger.Info("connection string: " + ConString);
                    _logger.Info("Getting oracle Connection");
                    //Getting oracle Connection instance 
                    OracleConnection conn = new OracleConnection(ConString);
                    //open oracle connection
                    conn.Open();
                    _logger.Info("Successfully opened  oracle Connection");

                    _logger.Info("query to be executed : " + CmdString);
                    //create an OracleCommand object 
                    OracleCommand cmd = new OracleCommand(CmdString, conn);

                    //execute oracle command
                    cmd.ExecuteNonQuery();

                    _logger.Info("Sucessfully query executed : " + CmdString);
                    _logger.Info("Sucessfully updated PROCESSEDFLAG and PROCESSEDTIME in PGEDATA.PGE_ED13_STAGING for order number : " + orderNumber);
                    cmd.Dispose();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error ("Exception while logging the updatng in ED13 staging table for order number: " + orderNumber + ex.Message + "   " + ex.StackTrace + MethodBase.GetCurrentMethod().Name);
            }
            _logger.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
        }

        public static void UpdateErrorDescription(string orderNumber, string error)
        {
            _logger.Info("Starting" + MethodBase.GetCurrentMethod().Name);
            try
            {
                if (error.Contains("'"))
                {
                    //if error has ' in string so replace because db does not accept this character 
                    error = error.Replace("'", "");
                }
                _logger.Info("Error description is getting populated");

                string CmdString = "update " + ConfigurationManager.AppSettings["ED13StagingTable"].ToString() + " Set " + ConfigurationManager.AppSettings["ERRORDESCRIPTION"].ToString() + "= '" + error + "' ," + ConfigurationManager.AppSettings["PROCESSEDFLAG"].ToString() + " = '" + ProcessFlag.Failed + "' where " + "\"" + "ORDER" + "\"=" + "'" + orderNumber + "'";
                if (ConString != string.Empty)
                {
                    _logger.Info("connection string: " + ConString);
                    _logger.Info("Getting oracle Connection");
                    //Getting oracle Connection instance 
                    OracleConnection conn = new OracleConnection(ConString);
                    //open oracle connection
                    conn.Open();
                    _logger.Info("Successfully opened  oracle Connection");
                    OracleCommand cmd = new OracleCommand(CmdString, conn);

                    _logger.Info("query to be executed : " + CmdString);
                    //create an OracleCommand object 
                    cmd.ExecuteNonQuery();

                    cmd.Dispose();
                    conn.Close();
                    _logger.Info("Sucessfully query executed : " + CmdString);
                    _logger.Info("Sucessfully updated PROCESSEDFLAG and ERRORDESCRIPTION in PGEDATA.PGE_ED13_STAGING for order number : " + orderNumber);
                }
            }
            catch (Exception ex)
            {
                _logger.Error ("Exception while logging the exception in ED13 staging table for order number: " + orderNumber + ex.Message + "   " + ex.StackTrace + MethodBase.GetCurrentMethod().Name);
            }
            _logger.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// This method is used to get SAP data from staging table and convert the same into XmlDocument EDGIS Rearch(v1t8)
        /// </summary>
        /// <returns>XmlDocument</returns>
        public XmlDocument TableToXml()
        {
            _logger.Info("Starting" + MethodBase.GetCurrentMethod().Name);
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                //Get connection string from app config 

                // m4jf edgisrearch 919
                //ConString = ConfigurationManager.AppSettings["OracleConnectionString"].ToString();
                ConString = ReadEncryption.GetConnectionStr(ConfigurationManager.AppSettings["EDER_ConnectionStr"].ToString().ToUpper())+ " Pooling=true; Min Pool Size=6;";

                string CmdString = "SELECT " + "\"" + "ORDER" + "\"" + "," + "\"" + "TYPE" + "\"" + ", ORDERCREATEDATE, JOBDESRIPTION, MAINWORKCENTER, JOBOWNER, LOCATION, EQUIPMENT, MAINTACTTYPE, WORKTYPE, LEGACYMAPNUMBER, REGION, DIVISION, STATUS, PRIMARYESTIMATOR, SECONDARYESTIMATOR FROM PGEDATA.PGE_ED13_STAGING where PROCESSEDFLAG = 'T' AND ERRORDESCRIPTION IS NULL";

                if (ConString != string.Empty)
                {
                    _logger.Info("connection string: " + ConString);
                    _logger.Info("Getting oracle Connection");
                    //Getting oracle Connection instance 
                    OracleConnection conn = new OracleConnection(ConString);
                    //open oracle connection
                    conn.Open();
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.XmlCommandType = OracleXmlCommandType.Query;
                    cmd.CommandText = CmdString;
                    XmlReader reader = cmd.ExecuteXmlReader();
                    _logger.Info("Sucessfully query executed : " + CmdString);
                    // Load records to xml object .
                    while (reader.Read())
                    {
                        xmlDoc.Load(reader);
                        _logger.Info("Sucessfully loaded SAP data from staging table to XmlDocument for further process");
                    }
                    //Dispose OracleCommand
                    cmd.Dispose();
                    //close oracle connection
                    conn.Close();

                    _logger.Info("Sucessfully updated PROCESSEDFLAG and ERRORDESCRIPTION in PGEDATA.PGE_ED13_STAGING for order number : ");
                }
            }
            catch (Exception ex)
            {
                _logger.Error ("Exception while loading SAP data from staging table to XmlDocument  " + ex.Message + "   " + ex.StackTrace + MethodBase.GetCurrentMethod().Name);
                throw ex;
            }
            _logger.Info("Completed Execution :" + MethodBase.GetCurrentMethod().Name);
            return xmlDoc;
        }
        /// <summary>
        /// Reads the contents of XSD Schema file
        /// </summary>
        /// <returns>Returns the contents of XSD Schema file</returns>
        private string ReadXSDFile()
        {
            string schemaContent = string.Empty;
            try
            {
                System.IO.Stream schemaStream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PGE.Interfaces.SAP.WOSynchronization.PMOrderXSD.xsd");
                byte[] contentBytes = new byte[schemaStream.Length];
                schemaStream.Read(contentBytes, 0, (int)schemaStream.Length);
                schemaContent = Encoding.ASCII.GetString(contentBytes);
                schemaStream.Close();
                _logger.Debug("XSD Schema file is read successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error reading XSL file.", ex);
                schemaContent = null;
            }
            return schemaContent;
        }

        /// <summary>
        /// Archives the <paramref name="fileName"/> to <c>ArchiveLocation</c> location
        /// </summary>
        /// <param name="fileName">File to be archived</param>
        private void ArchiveFile(string fileName)
        {
            try
            {
                FileInfo xmlFileInfo = new FileInfo(fileName);
                string archiveDirectory = Config.ReadConfigFromExeLocation().AppSettings.Settings["ArchiveLocation"].Value;
                _logger.Debug("'ArchiveLocation' config parameter read.");

                //Create if the Archive directory does not exist
                if (!Directory.Exists(archiveDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(archiveDirectory);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error("Error creating archive directory.", ex);
                        _logger.Info("Failed to create directory : " + archiveDirectory);
                        _logger.Info("Failed to archive file : " + fileName);
                        return;
                    }
                }

                //If a file with the same same already exists then append the number at the end to new file than replacing the existing file
                string archiveFileName = Path.Combine(archiveDirectory, xmlFileInfo.Name);
                int loopCount = 1;
                string newFileName = archiveFileName;
                while (File.Exists(newFileName))
                {
                    newFileName = archiveFileName.Substring(0, archiveFileName.Length - 4) + "_" + loopCount.ToString() + archiveFileName.Substring(archiveFileName.Length - 4);
                    loopCount++;
                }

                //Move to archive location
                xmlFileInfo.MoveTo(newFileName);
            }
            catch (Exception ex)
            {
                _logger.Error("Error archiving files.", ex);
            }
        }

        #endregion Private Methods

    }


    public class ProcessFlag
    {
        // Processed flag in all interfaces should be updated properly :(P- GIS Processed/E-GIS Error/T-In Transition/F-Failed/D- Done)

        public static string GISProcessed { get; } = "P";
        public static string GISError { get; } = "E";
        public static string InTransition { get; } = "T";
        public static string Failed { get; } = "F";
        public static string Completed { get; } = "D";

    }
}
