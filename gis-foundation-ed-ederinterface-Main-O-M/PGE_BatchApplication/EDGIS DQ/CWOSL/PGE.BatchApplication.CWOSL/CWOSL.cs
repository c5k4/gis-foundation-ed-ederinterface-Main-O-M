using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using System.Reflection;
//using Telvent.Delivery.Diagnostics;
using PGE.BatchApplication.CWOSL.Interfaces;
using PGE.BatchApplication.CWOSL.Classes;
using System.Data;
//1368195 START
using System.Net.Mail;
 using PGE.Desktop.EDER.AutoUpdaters.Special;
using PGE.Desktop.EDER.GDBM;
using PGE.Desktop.EDER.ValidationRules;
using PGE.Common.ChangesManagerShared;
//1368195 END
using PGE_DBPasswordManagement;
namespace PGE.BatchApplication.CWOSL
{
    class CWOSL : IExtractTransformLoad
    {
        private static readonly Log4NetLoggerCWOSL _logger = new Log4NetLoggerCWOSL(MethodBase.GetCurrentMethod().DeclaringType, "CWOSLDefault.log4net.config");

        private ArrayList _servicePointsList = new ArrayList();
        private Dictionary<string, string> _CWOSL_ServicePoints = new Dictionary<string, string>();
        private PGE.Common.ChangesManagerShared.SDEWorkspaceConnection _sdeWorkspaceConnection = null;// new PGE.ChangesManagerShared.SDEWorkspaceConnection(Initialize.SDEConnectionFile);
        private AdoOleDbConnection _sessionManagerOleDbConnection = null;// new AdoOleDbConnection(Initialize.OleDBConnectionString);
        public Initialize InitializeProcess { get; set; }
      

        public CWOSL()
        {
        }

        public bool InitializeObject()
        {
            bool noError = true;
            _logger.Debug("");
            try
            {
                InitializeProcess.ErrorOccured = false;
                InitializeProcess.InitLogFilePath();
                if (!InitializeProcess.InitializeVariables())
                {
                    //log message and exit application
                    _logger.Error("Initialize process failed.");
                    noError = false;
                }

                _sdeWorkspaceConnection = new PGE.Common.ChangesManagerShared.SDEWorkspaceConnection(InitializeProcess.SDEConnectionFile);
                _sessionManagerOleDbConnection = new AdoOleDbConnection("Provider=OraOLEDB.Oracle.1;"+ReadEncryption.GetConnectionStr(InitializeProcess.OleDBConnectionString));
                ServicePointSession.LimitSegUGProcessing = InitializeProcess.LimitSegUGServices;
                ServicePointSession.IgnoreESRIGeocodeError = InitializeProcess.IgnoreESRIGeocodeServiceError;
                ServicePointSession.ESRIGeocodeScore = InitializeProcess.ESRIGeocodeScore;
                ServicePointSession.BufferGeocodeInFeet = InitializeProcess.BufferGeocodeInFeet;
                ServicePointSession.MaxParcelArea = InitializeProcess.MaxParcelArea;
                ServicePointSession.MaxParcelCustomerPoints = InitializeProcess.MaxParcelCustomerPoints;

            }

            catch (Exception Ex)
            { InitializeProcess.ErrorMessage(Ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); noError = false; }
            finally
            {
                try
                {   //Release memory.
                    //InitializeProcess.Dispose();
                }
                catch (Exception Exf) { }
            }
            return noError;
        }

        public bool Run_CWOSL_Process()
        {
            ICursor pCur = null;
            IFeatureCursor ptransformerCur = null;
            DataTable data = null;
            try
            {
                //This funciton determine customers without serviceLocations and processes those, 
                //and updates CWOSL-Table.
                if (InitializeProcess.SDEWorkspace == null) return false;

                // Disable model name lookup on the local office AU and force the map number AU to fire
                PopulateLocalOfficeAU.LookupLocalOfficeByModelName = false;
                PopulateDistMapNoAU.AllowExecutionOutsideOfArcMap = true;

                InitializeProcess.InitLogFilePath();
                Stopwatch stpWatch = new Stopwatch();
                stpWatch.Start();

                string fields = string.Format("{0},{1},{2},{3},{4},{5},{6},{7}", Initialize.ServicePointTable.OIDFieldName, InitializeProcess.StreetNumberFldName,
                    InitializeProcess.StreetName1FldName, InitializeProcess.CityFldName, InitializeProcess.StateFldName, InitializeProcess.ZipFldName, HelperCls.CGC12_FldName,
                    InitializeProcess.ServicePointIDFldName);

                //string sqlSP = InitializeProcess.ServiceLocationGUIDFldName + " is null AND " +
                //InitializeProcess.SecondaryLoadPointGUIDFldName + " is null ORDER BY " + InitializeProcess.ServicePointIDFldName;

                //#if DEBUG
                //                sqlSP = InitializeProcess.ServiceLocationGUIDFldName + " is null AND " +
                //                               InitializeProcess.SecondaryLoadPointGUIDFldName + " is null" + " AND OBJECTID IN (8116585,8116305)" + " ORDER BY " + InitializeProcess.ServicePointIDFldName;
                //#endif
                //ME Q1 : 2020 Change in where clause with noumber records to be processed

                //1368195 START
                //int noOfRec;
                //if (Program.noOfRecords != null || Program.noOfRecords != 0)
                //{
                //    noOfRec = Program.noOfRecords;
                //    InitializeProcess.LimitEditsInSession = noOfRec;
                //}

                //if (Program.programRumTime != null)
                //{
                //    InitializeProcess.MaxMinutesToRun = Program.programRumTime;
                //}
                if (Program.SketchPseudoserviceFeatures != string.Empty && Program.SketchPseudoserviceFeatures.ToUpper() != "NULL")
                {
                    InitializeProcess.SketchPreFeature = Program.SketchPseudoserviceFeatures;
                    ServicePointSession.SketchPreFeature = Program.SeconderyLoadPoint;
                }
                if (Program.TransformerSubtype != string.Empty && Program.TransformerSubtype.ToUpper() != "NULL")
                {
                    InitializeProcess.Transformer = Program.TransformerSubtype;
                }
                if (Program.LocalOfficeID != string.Empty && Program.LocalOfficeID.ToUpper() != "NULL")
                {
                    InitializeProcess.LocalOfficeID = Program.LocalOfficeID;
                }
                if (Program.SeconderyLoadPoint != string.Empty && Program.SeconderyLoadPoint.ToUpper() != "NULL")
                {
                    InitializeProcess.SecLoadPoint = Program.SeconderyLoadPoint;
                }
                if (Program.County != string.Empty && Program.County.ToUpper() != "NULL")
                {
                    InitializeProcess.County = Program.County;
                }
                if (Program.Division != string.Empty && Program.Division.ToUpper() != "NULL")
                {
                    InitializeProcess.Division = Program.Division;
                }
                if (Program.HFTD != string.Empty && Program.HFTD.ToUpper() != "NULL")
                {
                    InitializeProcess.HFTDDomainName = Program.HFTD;
                }
                if (Program.programRumTime != 0)
                {
                    InitializeProcess.MaxMinutesToRun = Program.programRumTime;
                }
                if (Program.noOfRecords != 0)
                {
                    InitializeProcess.LimitEditsInSession = Program.noOfRecords;
                }
                if (Program.FULLPROCESS != string.Empty && Program.FULLPROCESS.ToUpper() != "NULL")
                {
                    InitializeProcess.FULLPROCESS = Program.FULLPROCESS;
                }
                if (Program.CIRCUITID != string.Empty && Program.CIRCUITID.ToUpper() != "NULL")
                {
                    InitializeProcess.CIRCUITID = Program.CIRCUITID;
                }
                //1368195 END

                //End ME - Q1 2020
                //string sqlSP = "OBJECTID IN (8117254)";
                //Changes from 1368195 Start
                string sqlSP = null;
                string sqlSP1 = null;
                if (InitializeProcess.FULLPROCESS.ToUpper() == "FALSE")
                {
                    sqlSP1 = "with abc as(Select * from edgis.servicepoint spt where spt.servicelocationguid not in(Select globalid from edgis.servicelocation)UNION ALL Select * from edgis.servicepoint spt where spt.servicelocationguid is NULL) select abc.* from abc, edgis.TRANSFORMER def where abc.TRANSFORMERGUID = def.GLOBALID";
                    if (InitializeProcess.County.ToUpper() == "ALL" && InitializeProcess.Division.ToUpper() == "ALL" && InitializeProcess.LocalOfficeID.ToUpper() == "ALL" && InitializeProcess.Transformer.ToUpper() == "ALL" && InitializeProcess.CIRCUITID.ToUpper() == "ALL")
                    {
                        sqlSP1 = "Select * from edgis.servicepoint spt where spt.servicelocationguid not in(Select globalid from edgis.servicelocation) UNION ALL Select * from edgis.servicepoint spt where spt.servicelocationguid is NULL";
                    }
                    else
                    {
                        if (InitializeProcess.County.ToUpper() != "ALL")
                        {
                            sqlSP1 = sqlSP1 + " and def.COUNTY = " + InitializeProcess.County;
                        }
                        if (InitializeProcess.Division.ToUpper() != "ALL")
                        {
                            sqlSP1 = sqlSP1 + " and def.DIVISION = " + InitializeProcess.Division;
                        }
                        if (InitializeProcess.LocalOfficeID.ToUpper() != "ALL")
                        {
                            sqlSP1 = sqlSP1 + " and def.LOCALOFFICEID = '" + InitializeProcess.LocalOfficeID + "'";
                        }
                        if (InitializeProcess.Transformer.ToUpper() != "ALL")
                        {
                            sqlSP1 = sqlSP1 + " and def.SUBTYPECD = " + InitializeProcess.Transformer;
                        }
                        if (InitializeProcess.CIRCUITID.ToUpper() != "ALL")
                        {
                            sqlSP1 = sqlSP1 + " and def.CIRCUITID = '" + InitializeProcess.CIRCUITID + "'";
                        }
                    }
                }
                else
                {
                    sqlSP1 = "Select * from edgis.servicepoint spt where spt.servicelocationguid not in(Select globalid from edgis.servicelocation) UNION ALL Select * from edgis.servicepoint spt where spt.servicelocationguid is NULL";
                    //sqlSP = InitializeProcess.ServiceLocationGUIDFldName + " is null ORDER BY " + InitializeProcess.ServicePointIDFldName;
                }


                // m4jf
                HelperCls.GetData(sqlSP1, out data, "Provider=OraOLEDB.Oracle.1;" + ReadEncryption.GetConnectionStr(InitializeProcess.OleDBConnectionString));
                HelperCls.GetCursor(out pCur, Initialize.ServicePointTable, sqlSP, fields);
                //Changes from 1368195 Stop
                //if (pCur == null)
                //    throw new Exception("Zero records returned when searching ServicePoints.");
                if (data == null)
                    throw new Exception("Zero records returned when searching ServicePoints.");
                //IRow servicePointRow = pCur.NextRow(); //get all service points that have no service locations.

                //if (servicePointRow == null) throw new Exception("Service Point search returned null.");

                //IQueryFilter qf = new QueryFilterClass();
                //qf.WhereClause = sqlSP;
                //int toProcess = Initialize.ServicePointTable.RowCount(qf);
                int toProcess = data.Rows.Count;
                int processed = 0;
                ServicePoint servicePoint = null;
                stpWatch = new Stopwatch();
                stpWatch.Start();
                _logger.Info("Start loading ServicePoints...");
                ShowPercentProgress("Caching Service Points to process", processed, toProcess);
                foreach (DataRow row in data.Rows)
                {
                    HelperCls.GetCursor(out pCur, Initialize.ServicePointTable, "OBJECTID = " + row[0], "*");
                    IRow servicePointRow = pCur.NextRow();
                    if (servicePointRow != null)
                    {
                        servicePoint = new ServicePoint(servicePointRow);
                        _servicePointsList.Add(servicePoint);
                    }
                    if (pCur != null)
                    {
                        while (Marshal.ReleaseComObject(pCur) != 0) { };
                        pCur = null;
                    }
                    processed++;
                    if ((processed % 1000) == 0) { ShowPercentProgress("Caching Service Points to process", processed, toProcess); }

                }
                //while (servicePointRow != null)
                //{
                //    //Load service point.
                //    servicePoint = new ServicePoint(servicePointRow);
                //    _servicePointsList.Add(servicePoint);
                //    processed++;
                //    if ((processed % 1000) == 0) { ShowPercentProgress("Caching Service Points to process", processed, toProcess); }

                //    while (Marshal.ReleaseComObject(servicePointRow) > 0) { }
                //    servicePointRow = pCur.NextRow();
                //}
                if (pCur != null) { while (Marshal.ReleaseComObject(pCur) > 0) { } }
                pCur = null;
                ShowPercentProgress("Caching Service Points to process", processed, toProcess);
                Console.WriteLine();
                TimeSpan span = stpWatch.Elapsed;
                string strTmElapsed = null;
                if (span.Hours > 0) strTmElapsed = string.Format("{0} hours, {1} minutes", span.Hours, span.Minutes);
                else strTmElapsed = string.Format("{0} minutes, {1} seconds", span.Minutes, span.Seconds);
                _logger.Info(_servicePointsList.Count.ToString() + ". ServicePoints loaded to list: " + strTmElapsed);
                return true;
            }
            catch (Exception ex)
            { _logger.Error(ex.Message, ex); return false; }
            finally
            {
                if (pCur != null)
                {
                    while (Marshal.ReleaseComObject(pCur) != 0) { };
                    pCur = null;
                }
            }
        }

        /// <summary>
        /// Updates progress within the console application.  If anything fails, it just catches and moves on.  This is for the case that UC4 fails updating the console
        /// window which has been seen in the past
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currElementIndex"></param>
        /// <param name="totalElementCount"></param>
        private void ShowPercentProgress(string message, double currElementIndex, double totalElementCount)
        {
            try
            {
                if (currElementIndex < 0 || currElementIndex > totalElementCount)
                {
                    throw new InvalidOperationException("currElement out of range");
                }
                double percent = Math.Round((100.0 * ((double)currElementIndex)) / (double)totalElementCount, 2);
                Console.Write("\r{0}: {1}% complete", message, percent);
            }
            catch (Exception ex)
            {

            }
        }

        public void ProcessSessions()
        {
            try
            {
                //Takes the service points and process in batches in a session.
                if (_servicePointsList.Count == 0)
                    throw new Exception("No Service Points to process.");
                int countServicePnts = 0;
                int countJobs = 0;
                ArrayList servicePointsToProcess = new ArrayList();
                ServicePoint sevPoint = null;
                Stopwatch stpWatch = new Stopwatch();

                for (int i = 0; i < _servicePointsList.Count; i++)
                {
                    if (countServicePnts == InitializeProcess.LimitEditsInSession)
                    {
                        countJobs++;
                        //Process the service points in a session.
                        stpWatch = new Stopwatch();
                        stpWatch.Start();
                        _logger.Info("Start processing ServicePoints...");

                        ServicePointSession spSession = new ServicePointSession(_sdeWorkspaceConnection, _sessionManagerOleDbConnection, InitializeProcess.SessionName);
                        spSession.ProcessSession(servicePointsToProcess, InitializeProcess.MaxMinutesToRun);
                        WriteEndTime(stpWatch, countServicePnts.ToString(), countJobs);

                        //Reset the counter.
                        servicePointsToProcess = new ArrayList();
                        countServicePnts = 0;

                        if (InitializeProcess.NumberOfJobs == countJobs)
                            break;//Exit process after specified sessions have been processed.

                        if (HelperCls.ApplicationHasExceededMaxMinutesToRun(Initialize.AppStartDateTime, InitializeProcess.MaxMinutesToRun))
                            break;//Graceful exit if time limit reached.
                    }
                    sevPoint = _servicePointsList[i] as ServicePoint;
                    if (!_CWOSL_ServicePoints.ContainsKey(sevPoint.ServicePointID))
                    {
                        servicePointsToProcess.Add(_servicePointsList[i]);
                        countServicePnts++;
                    }
                }

                //Process the last batch
                if (countServicePnts > 0)
                {
                    //Process the service points in a session.
                    ServicePointSession spSession = new ServicePointSession(_sdeWorkspaceConnection, _sessionManagerOleDbConnection, InitializeProcess.SessionName);
                    spSession.ProcessSession(servicePointsToProcess, InitializeProcess.MaxMinutesToRun);
                }
                ////1368195 START
                //ServicePointSession spSessionQAQC = new ServicePointSession(_sdeWorkspaceConnection, _sessionManagerOleDbConnection, InitializeProcess.SessionName);
                ////spSessionQAQC.RunQAQC();
                ////1368195 STOP
            }
            catch (Exception ex)
            { _logger.Error(ex.Message, ex); }
        }
        public void Load_CWOSL_Records()
        {
            ICursor pCur = null;
            try
            {
                //Loads CWOSL ServicePoints to dictionary.
                HelperCls.GetCursor(out pCur, Initialize.CWOSLTable, null);
                if (pCur == null) return;

                IRow cwoslRow = pCur.NextRow();
                if (cwoslRow == null) return;

                int servicePointIdx = -1;
                HelperCls.GetFldIdx(Initialize.CWOSLTable, InitializeProcess.ServicePointIDFldName, out servicePointIdx);
                if (servicePointIdx == -1) throw new Exception("Failed to get field index: " + InitializeProcess.ServicePointIDFldName);

                InitializeProcess.LogMessage("Starting to load CWOSL records...");
                _logger.Info("Starting to load CWOSL records...");

                while (cwoslRow != null)
                {
                    string servicePntID = HelperCls.GetValue(cwoslRow, servicePointIdx);
                    if (!string.IsNullOrEmpty(servicePntID))
                    {
                        try
                        {
                            _CWOSL_ServicePoints.Add(servicePntID, servicePntID);
                        }
                        catch (Exception ignoreEx) { }
                    }
                    cwoslRow = pCur.NextRow();
                }
            }
            catch (Exception ex)
            { _logger.Error(ex.Message, ex); }
            finally
            {
                if (pCur != null)
                {
                    while (Marshal.ReleaseComObject(pCur) != 0) { };
                    pCur = null;
                }
            }
        }

        private void WriteEndTime(Stopwatch stpWatch, string numberOfPoints, int sessionCount)
        {
            try
            {
                TimeSpan span = stpWatch.Elapsed;
                string strTmElapsed = null;
                if (span.Hours > 0) strTmElapsed = string.Format("{0} hours, {1} minutes", span.Hours, span.Minutes);
                else strTmElapsed = string.Format("{0} minutes, {1} seconds", span.Minutes, span.Seconds);
                _logger.Info("(" + sessionCount + ") " + (ServicePointSession.ProcessedCount + 1) + " out of " + numberOfPoints.ToString() + " ServicePoints processed: " + strTmElapsed);

            }
            catch (Exception ex) { }
        }
        void IExtractTransformLoad.ProcessEtl()
        {
            if (!InitializeObject())
                return;//if error occured, terminete process
            //ME Q1 2020
            // m4jf edgisrearch 919
            IWorkspace wksp = OpenSDEWorkspacefromsdefile(ReadEncryption.GetSDEPath(InitializeProcess.SDEConnectionFile));
            if (wksp != null)
            {
                ITable sessionTable = ((IFeatureWorkspace)wksp).OpenTable("PROCESS.MM_SESSION");

                IQueryFilter qf = new QueryFilter();
                qf.SubFields = "SESSION_ID,CREATE_USER";
                qf.WhereClause = "hidden=0 and substr(session_name,0,12) = 'CWOSLProcess'";
                ICursor cur = sessionTable.Search(qf, false);
                IRow rw = cur.NextRow();
                //If no session exists then Prrocess will start

                if (rw == null)
                {
                    //Run process to determine service points without locations.
                    Run_CWOSL_Process();

                    //Get list of those that are already processed and written to CWOSL table.
                    Load_CWOSL_Records();

                    //Process the service points in sessions.
                    ProcessSessions();
                }
                else
                {
                    string sessionName = "";
                    sessionName = Convert.ToString(rw.get_Value(sessionTable.Fields.FindField("SESSION_ID")));
                    string sessionOwner = Convert.ToString(rw.get_Value(sessionTable.Fields.FindField("CREATE_USER")));
                    //1368195 Email Change
                    SendAutomatedEmail(InitializeProcess.SessionExist + sessionOwner + ".SN_" + sessionName);
                    //1368195 Email Change
                    _logger.Info("Session is already exists : " + sessionOwner + ".SN_" + sessionName + ".");
                }
            }
            else
            {
                _logger.Info("EDER Workspace is not found");
            }

        }

        void IExtractTransformLoad.Dispose()
        {
            //dispose here
        }

        void IExtractTransformLoad.ReconcileAndPostVersion()
        {
            try
            {
                //1368195 Start added if clouse
                if (RunQAQC() == true)
                {
                    // m4jf edgisrearch 919
                    string sdeCon = ReadEncryption.GetSDEPath(InitializeProcess.SDEConnectionFile);
                    string sessionName = InitializeProcess.SessionName;

                    //string sql = "SELECT max(SESSION_ID) from PROCESS.MM_SESSION where hidden=0 and substr(session_name,0," + sessionName.Length + ") = '" + sessionName + "'";
                    //m4jf
                    string connection = "Provider=OraOLEDB.Oracle.1;" + ReadEncryption.GetConnectionStr(InitializeProcess.OleDBConnectionString);

                    IWorkspace wksp = OpenSDEWorkspacefromsdefile(sdeCon);
                    if (wksp != null)
                    {
                        ITable sessionTable = ((IFeatureWorkspace)wksp).OpenTable("PROCESS.MM_SESSION");

                        IQueryFilter qf = new QueryFilter();
                        qf.SubFields = "SESSION_ID,CREATE_USER";
                        qf.WhereClause = "hidden=0 and substr(session_name,0,12) = 'CWOSLProcess'";
                        ICursor cur = sessionTable.Search(qf, false);
                        IRow rw = cur.NextRow();
                        if (rw != null)
                        {
                            sessionName = rw.get_Value(sessionTable.Fields.FindField("SESSION_ID")).ToString();
                            string sessionOwner = rw.get_Value(sessionTable.Fields.FindField("CREATE_USER")).ToString();
                            _logger.Info("Session posting started for : " + sessionOwner + ".SN_" + sessionName);
                            //getting sde connection to post version

                            // m4jf 
                            string sdeUserConn = ReadEncryption.GetSDEPath(InitializeProcess.EDERConnectionSDEUser); 
                            IWorkspace wkspSDEUser = OpenSDEWorkspacefromsdefile(sdeUserConn);

                            IVersionedWorkspace versionedEDERWksp = (IVersionedWorkspace)wkspSDEUser;
                            IVersion CWOSLVer = versionedEDERWksp.FindVersion(sessionOwner + ".SN_" + sessionName);
                            IVersion parentVer = versionedEDERWksp.FindVersion(CWOSLVer.VersionInfo.Parent.VersionName);
                            bool reconciled = ReconcilePost(CWOSLVer, parentVer);
                            if (reconciled)
                            {
                                //Delete Session

                                string sqlDeleteSession = "DELETE FROM PROCESS.MM_SESSION WHERE SESSION_ID = '" + sessionName + "' and CREATE_USER ='" + sessionOwner + "'";
                                wksp.ExecuteSQL(sqlDeleteSession);
                                CWOSLVer.Delete();
                                _logger.Info("Session is posted ");

                            }
                        }
                        else
                        {
                            _logger.Info("There is no session exists for posting.");
                        }
                        if (cur != null)
                        {
                            Marshal.ReleaseComObject(cur);
                        }
                    }

                    else
                    {
                        _logger.Info("EDER Workspace is not found");
                    }
                }
                else
                {
                    SendAutomatedEmail(InitializeProcess.QAQCMess);
                }
            }
            catch (Exception ex)
            { _logger.Info(ex.ToString()); }
        }
        //1368195 Start
        public Boolean RunQAQC()
        {
            bool success = false;
            try
            {
                // m4jf edgisrearch 919
                string sdeCon = ReadEncryption.GetSDEPath(InitializeProcess.SDEConnectionFile);
                string sessionName = InitializeProcess.SessionName;
                IWorkspaceEdit wkspcEdit = null;
                // m4jf
                string connection = "Provider=OraOLEDB.Oracle.1;" + ReadEncryption.GetConnectionStr(InitializeProcess.OleDBConnectionString);

                IWorkspace wksp = OpenSDEWorkspacefromsdefile(sdeCon);
                if (wksp != null)
                {
                    ITable sessionTable = ((IFeatureWorkspace)wksp).OpenTable("PROCESS.MM_SESSION");

                    IQueryFilter qf = new QueryFilter();
                    qf.SubFields = "SESSION_ID,CREATE_USER";
                    qf.WhereClause = "hidden=0 and substr(session_name,0,12) = 'CWOSLProcess'";
                    ICursor cur = sessionTable.Search(qf, false);
                    IRow rw = cur.NextRow();
                    if (rw != null)
                    {
                        sessionName = rw.get_Value(sessionTable.Fields.FindField("SESSION_ID")).ToString();
                        string sessionOwner = rw.get_Value(sessionTable.Fields.FindField("CREATE_USER")).ToString();
                        _logger.Info("Beginning QA/QC Processing" + sessionOwner + ".SN_" + sessionName);
                        //getting sde connection to post version

                        // m4jf
                        string sdeUserConn = ReadEncryption.GetSDEPath(InitializeProcess.EDERConnectionSDEUser);
                        IWorkspace wkspSDEUser = OpenSDEWorkspacefromsdefile(sdeUserConn);

                        IVersionedWorkspace versionedEDERWksp = (IVersionedWorkspace)wksp;
                        IVersion CWOSLVer = versionedEDERWksp.FindVersion(sessionOwner + ".SN_" + sessionName);
                        wkspcEdit = (IWorkspaceEdit)CWOSLVer;

                        wkspcEdit.StartEditing(true);
                        wkspcEdit.StartEditOperation();

                        if (wkspcEdit.IsBeingEdited())
                        {
                            try
                            {
                                wksp.ExecuteSQL("delete from EDGIS.PGE_QAQC_SESSIONERRORS");
                                ValidationEngine.QAQCWorkspace = wksp;
                                //ModelNameFacade.ModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;

                                //Delete current QAQC table results
                                PGE_QAQC_Ext runQAQC = null;
                                runQAQC = new PGE_QAQC_Ext();
                                //runQAQC.DeleteQAQCSessionRows(wksp);

                                //Run QAQC
                                success = runQAQC.RunQAQC((IWorkspace)CWOSLVer);
                            }
                            catch (Exception ex)
                            {
                            }
                            finally
                            {
                                //Save the version edits
                                wkspcEdit.StopEditOperation();
                                wkspcEdit.StopEditing(true);
                            }
                        }
                    }
                }
                //IFeatureWorkspace featWorkspace = wksp as IFeatureWorkspace;
                //IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)wksp;
                //IVersion qaVersion = versionedWorkspace.FindVersion("CWOSL_RW" + ".SN_" + sessionName);                
                
            }
            catch (Exception ex)
            {
                return success;
            }
            return success;
        }
        public void SendAutomatedEmail(string content)
        {
            _logger.Info("Mail Sent Started");
            try
            {
                string mailServer = InitializeProcess.SMTPSERVER;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(InitializeProcess.MAILFROM);
                foreach (string to in InitializeProcess.MAILTO.Split(';'))
                    message.To.Add(to);
                foreach (string cc in InitializeProcess.MAILCC.Split(';'))
                    message.To.Add(cc);
                message.IsBodyHtml = true;

                content = content.Replace("\\n", "<br />") +"<br /><br />Thank you,<br />EDGISSupport Team";

                message.Body = content;// "CWOSL process has already session.";

                message.Subject = InitializeProcess.MAILSUBJECT;

                SmtpClient client = new SmtpClient(mailServer);
                //var AuthenticationDetails = new NetworkCredential("user@domain.com", "password");
                //client.Credentials = AuthenticationDetails;
                client.Send(message);
                _logger.Info("Mail Sent.");

            }
            catch (Exception ex)
            {
                _logger.Info("SenAutomatedEmail Exception: " + ex.Message);

            }

        }
        //1368195 STOP

        public IWorkspace OpenSDEWorkspacefromsdefile(string sdefilenamewithpath)
        {
            Type t = null;
            IWorkspaceFactory workspaceFactory = null;
            IWorkspace workspace = null;
            try
            {

                t = Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory");
                workspaceFactory = (IWorkspaceFactory)Activator.CreateInstance(t);
                workspace = workspaceFactory.OpenFromFile(sdefilenamewithpath, 0);
            }
            catch (Exception exp)
            {
            }
            return workspace;

        }
        public Boolean ReconcilePost(IVersion editVersion, IVersion targetVersion)
        {
            IMultiuserWorkspaceEdit muWorkspaceEdit = (IMultiuserWorkspaceEdit)editVersion;
            IWorkspaceEdit workspaceEdit = (IWorkspaceEdit2)editVersion;
            IVersionEdit4 versionEdit = (IVersionEdit4)workspaceEdit;

            if (muWorkspaceEdit.SupportsMultiuserEditSessionMode
                (esriMultiuserEditSessionMode.esriMESMVersioned))
            {

                //Reconcile with the target version. 
                bool conflicts = true;
                while (conflicts)
                {
                    try
                    {
                        muWorkspaceEdit.StartMultiuserEditing(esriMultiuserEditSessionMode.esriMESMVersioned);
                        conflicts = versionEdit.Reconcile4(targetVersion.VersionName, false, false, false, true);
                        if (!conflicts)
                        {
                            //Posting this version
                            versionEdit.Post(targetVersion.VersionName);
                        }
                    }
                    catch (System.Runtime.InteropServices.COMException excom)
                    {
                        _logger.Info(excom.ToString());
                        if (excom.ErrorCode == -2147217137) //ESRI.ArcGIS.Geodatabase.fdoError.FDO_E_RECONCILE_VERSION_NOT_AVAILABLE 
                        {
                            //Logger.Error("The target version  " + targetVersion.VersionName + "  is currently being reconciled against some other version.");
                        }
                        else if (excom.ErrorCode == -2147217139) //FDO_E_VERSION_BEING_EDITED 
                        {
                            //Logger.Error("Reconcile not allowed as the version is being edited by another application.");
                        }
                        else if (excom.ErrorCode == -2147217146)//FDO_E_VERSION_NOT_FOUND 
                        {
                            //Logger.Error("reconcile failed.The version " + targetVersion.VersionName + " could not be located");
                        }

                        if (workspaceEdit.IsBeingEdited())
                            workspaceEdit.StopEditing(false);
                        conflicts = true;
                    }
                }
                if (workspaceEdit.IsBeingEdited())
                    workspaceEdit.StopEditing(true);
                return true;
            }
            return false;
        }




    }
}
