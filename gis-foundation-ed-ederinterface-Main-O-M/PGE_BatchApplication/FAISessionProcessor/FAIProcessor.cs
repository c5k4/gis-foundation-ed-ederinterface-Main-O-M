using System;
using System.Collections.Generic;
using ESRI.ArcGIS.Geodatabase;
using System.IO;
using System.Xml.Linq;
using PGE.BatchApplication.FAISessionProcessor.SessionManager;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.DataSourcesGDB;
using Miner.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using PGE.Desktop.EDER;
using PGE.Desktop.EDER.AutoUpdaters;
using PGE.Desktop.EDER.AutoUpdaters.Special;
using PGE.Desktop.EDER.AutoUpdaters.Special.RegionalAttributesAU;
using PGE.Common.Delivery.Framework;

namespace PGE.BatchApplication.FAISessionProcessor
{
    public class FAIProcessor
    {
        #region Public Properties

        public static List<TableInformation> TableInfo = new List<TableInformation>();

        #endregion

        #region Private Properties

        private static Log4NetLogger _log = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "PGE.BatchApplication.FAISessionProcessor.log4net.config");
        private static string SessionName = "";
        private List<string> FGDBFilesToProcess = new List<string>();
        private List<GUIDMap> FAIGuidToOriginalGUID = new List<GUIDMap>();
        private SortedDictionary<string, List<string>> ExistingOperatingNumbers = new SortedDictionary<string, List<string>>();
        private int transactionsToProcess = 0;
        private int transactionsProcessed = 0;
        private int transactionsSinceLastSave = 0;
        private static IMMAutoUpdater auController = null;

        private IWorkspace DefaultVersionWorkspace { get; set; }
        private IWorkspace LandbaseWorkspace { get; set; }
        private IWorkspace FAIWorkspace { get; set; }

        private MinerSession ParentSession { get; set; }
        private IVersionedWorkspace EditWorkspace { get; set; }

        #endregion

        #region Constructor

        public FAIProcessor(IWorkspace defaultWorkspace, IWorkspace landbaseWorkspace)
        {
            if (defaultWorkspace == null) { throw new Exception("FAIProcessor: Invalid GIS Workspace specified"); }
            if (landbaseWorkspace == null) { throw new Exception("FAIProcessor: Invalid Landbase workspace specified"); }
            DefaultVersionWorkspace = defaultWorkspace;
            LandbaseWorkspace = landbaseWorkspace;

            //Determine the FGDB files to process
            string[] inputFGDBFiles = Directory.GetDirectories(Common.InputFGDBLocation);
            foreach (string FGDBFile in inputFGDBFiles)
            {
                if (FGDBFile.ToUpper().EndsWith(".GDB"))
                {
                    //Valid FGDB
                    FGDBFilesToProcess.Add(FGDBFile);
                }
            }

            if (FGDBFilesToProcess.Count < 1) { Logger.Warning("FAIProcessor: No input FGDB in input directory " + Common.InputFGDBLocation); }
        }
        
        #endregion

        #region Public Methods

        public void ProcessFAIData()
        {
            foreach (string FGDBFile in FGDBFilesToProcess)
            {
                ResetInformation();

                FileGDBWorkspaceFactory wsFactory = new FileGDBWorkspaceFactoryClass();
                FAIWorkspace = wsFactory.OpenFromFile(FGDBFile, 0);

                //Name of the FGDB file that should provide some description of the area that was exported from
                string FAIName = FGDBFile.Substring(FGDBFile.LastIndexOf("\\") + 1);
                FAIName = FAIName.Substring(0, FAIName.ToUpper().IndexOf(".GDB"));
                SessionName = "FAI_" + FAIName;

                ResultCounts resultCounts = new ResultCounts();
                try
                {
                    //Check for existing session and fail if one exists

#if !DEBUG
                    string currentSessionName = CurrentFAISessionName(DefaultVersionWorkspace, SessionName);
                    if (!string.IsNullOrEmpty(currentSessionName))
                    {
                        throw new Exception(FGDBFile + ": Current FAI session already exists for this FGDB input file: " + currentSessionName);
                    }
#endif

                    //Create a new Session
                    CreateMinerSession();
                    Logger.Info("Session created: " + SessionName);

                    //Cache our operating number information
                    Logger.Info("Determine existing operating numbers in system");
#if !DEBUG
                    GetExistingOperatingNumbers();
#endif

                    //Process the data now
                    ProcessFAIData(ref resultCounts);

                    Logger.Info(resultCounts.InsertsProcessed + " FAI inserts  were processed with " + resultCounts.InsertsFailed + " failures.");
                    Logger.Info(resultCounts.UpdatesProcessed + " FAI updates transactions were processed with " + resultCounts.UpdatesFailed + " failures.");
                    Logger.Info(resultCounts.DeletesProcessed + " FAI deletes transactions were processed with " + resultCounts.DeletesFailed + " failures.");

                    try
                    {
                        Logger.Info("Archiving processed FGDB");
                        while (Marshal.ReleaseComObject(FAIWorkspace) > 0) { }
                    
                        IFileNames fileNames = new FileNamesClass();
                        fileNames.Add(FGDBFile);
                        IWorkspaceName wsName = wsFactory.GetWorkspaceName(Common.InputFGDBLocation, fileNames);
                        wsFactory.Move(wsName, Common.ArchiveFGDBLocation);
                        Logger.Info("FGDB successfully archived to " + Common.ArchiveFGDBLocation);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning("Could not archive " + FGDBFile + ". Error: " + ex.Message);
                    }

                    Logger.Info("");
                    Logger.Info("");
                }
                catch (Exception ex)
                {
                    //Something failed.  Report the erorr and throw the exception
                    Logger.Error("Failed FAI processor " + ex.Message + "\r\nStacktrace: " + ex.StackTrace);
                    throw ex;
                }
                finally
                {

                }
            }
        }

        #endregion

        #region FAI Transaction Processing

        /// <summary>
        /// Process our FAI data for inserts, updates, and deletes
        /// </summary>
        /// <param name="resultCounts"></param>
        private void ProcessFAIData(ref ResultCounts resultCounts)
        {
            try
            {
                List<string> tablesToProcess = GetTablesToProcess();
                Logger.Info("There are " + tablesToProcess.Count + " FAI tables to process");

                //Start a new edit session
                ((IWorkspaceEdit)EditWorkspace).StartEditing(false);

                //For performance sake, disable AUs
                Type type = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
                auController = (IMMAutoUpdater)Activator.CreateInstance(type);
                mmAutoUpdaterMode prevAUMode = auController.AutoUpdaterMode;

                try
                {
                    //Cache our first set of poles for transaction processing
                    PopulateDistMapNoAU.AllowExecutionOutsideOfArcMap = true;
                    InheritRegionalAttributesAU.AllowExecutionOutsideOfArcMap(LandbaseWorkspace);
                    auController.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMStandAlone;
                    List<TransactionType> transactionTypesToProcess = new List<TransactionType>();
                    transactionTypesToProcess.Add(TransactionType.Delete);
                    transactionTypesToProcess.Add(TransactionType.Update);
                    transactionTypesToProcess.Add(TransactionType.Insert);


                    foreach (TransactionType transactionType in transactionTypesToProcess)
                    {
                        if (transactionType == TransactionType.Insert)
                        {
                            Logger.Info("Processing Inserts from FAI database");
                            PopulateLocalOfficeAU.LookupLocalOfficeByModelName = false;
                        }
                        else if (transactionType == TransactionType.Update)
                        {
                            Logger.Info("Processing Updates from FAI database");
                            PopulateLocalOfficeAU.LookupLocalOfficeByModelName = true;
                        }
                        else if (transactionType == TransactionType.Delete)
                        {
                            Logger.Info("Processing Deletes from FAI database");
                            PopulateLocalOfficeAU.LookupLocalOfficeByModelName = true;
                        }

                        //Process through all of our tables for this transaction type
                        List<string> insertTablesProcessed = new List<string>();
                        for (int i = 0; i < 3; i++)
                        {
                            foreach (string tableName in tablesToProcess)
                            {
                                ITable gisTable = null;
                                string gisTableName = tableName;
                                if (!gisTableName.Contains(".")) { gisTableName = "EDGIS." + tableName; }
                                try { gisTable = ((IFeatureWorkspace)EditWorkspace).OpenTable(gisTableName); }
                                catch { }

#if DEBUG
                                //if (gisTable != null && ((IDataset)gisTable).BrowseName != "EDGIS.PriOHConductor"
                                //    && ((IDataset)gisTable).BrowseName != "EDGIS.PriOHConductorInfo") { continue; }
                                //if (gisTable != null && ((IDataset)gisTable).BrowseName != "EDGIS.PriOHConductor") { continue; }
                                //if (gisTable == null || transactionType != TransactionType.Insert || 
                                //    (((IDataset)gisTable).BrowseName.ToUpper() != "EDGIS.PRIOHCONDUCTOR" && ((IDataset)gisTable).BrowseName.ToUpper() != "EDGIS.PRIOHCONDUCTORINFO")) { continue; }
                                //if (gisTable != null && ((IDataset)gisTable).BrowseName != "EDGIS.DynamicProtectiveDevice") { continue; }
                                //if (gisTable != null && ((IDataset)gisTable).BrowseName != "EDGIS.SupportStructure") { continue; }
#endif

                                //Process all of the point features first
                                bool process = false;
                                if (i == 0 && gisTable is IFeatureClass && ((IFeatureClass)gisTable).ShapeType != esriGeometryType.esriGeometryPolyline) { process = true; }
                                else if (i == 1 && gisTable is IFeatureClass && ((IFeatureClass)gisTable).ShapeType == esriGeometryType.esriGeometryPolyline) { process = true; }
                                else if (i == 2 && !(gisTable is IFeatureClass)) { process = true; }

                                if (!process) { continue; }

                                insertTablesProcessed.Add(tableName);
                                if (gisTable == null)
                                {
                                    Logger.Warning("     " + gisTableName + " from FAI database does not exist in GIS");
                                    Console.WriteLine();
                                    continue;
                                }

                                int successes = 0;
                                int failures = 0;
                                List<FAIFeatureInformation> FAIFeatInfo = GetFAIFeatureInformation(tableName, transactionType);
                                transactionsToProcess = FAIFeatInfo.Count;
                                transactionsProcessed = 0;

                                //Logger.Info("     Processing table " + tableName + ": " + transactionsToProcess + " to process");
                                if (transactionsToProcess > 0)
                                {
                                    Console.WriteLine();
                                    Logger.Info("          Processing " + tableName);


                                    //Don't execute the split AU when inserting new features.  We'll assume that splitting has already been completed. If this
                                    //AU executes during processing it can fail unexpectedly
                                    PGESegmentSplitAU.DisableAU = true;

                                    if (transactionType == TransactionType.Insert)
                                    {
                                        //Don't need to query for our original guids since these are new features
                                        foreach (FAIFeatureInformation featInfo in FAIFeatInfo)
                                        {
                                            ShowPercentProgress(string.Format("               {0} out of {1} processed", transactionsProcessed, transactionsToProcess), transactionsProcessed, transactionsToProcess);
                                            try
                                            {
                                                ((IWorkspaceEdit)EditWorkspace).StartEditOperation();
                                                ProcessTransaction(featInfo, null, gisTable);
                                                ((IWorkspaceEdit)EditWorkspace).StopEditOperation();
                                                successes++;
                                            }
                                            catch (Exception ex)
                                            {
                                                ((IWorkspaceEdit)EditWorkspace).AbortEditOperation();
                                                ReportErroredFeature(featInfo, "Failed to process insert: " + ex.Message + " StackTrace: " + ex.StackTrace);
                                                failures++;
                                                //throw new Exception("Failed to process inserts for " + featInfo.OriginalGlobalID + ": " + ex.Message + " StackTrace: " + ex.StackTrace);
                                            }
                                            transactionsProcessed++;
                                        }
                                        ShowPercentProgress(string.Format("               {0} out of {1} processed", transactionsProcessed, transactionsToProcess), transactionsProcessed, transactionsToProcess);

                                        Console.WriteLine();
                                    }
                                    else
                                    {
                                        //Process the Updates and Deletes here

                                        //Query for our original guid features since these are deletes and updates
                                        List<Guid> guidsFound = new List<Guid>();
                                        int globalIDIdx = gisTable.Fields.FindField("GLOBALID");

                                        List<string> guidsWhereInClausesToProcess = GetWhereInClausesForFAIFeatInfo(FAIFeatInfo);
                                        foreach (string whereInClause in guidsWhereInClausesToProcess)
                                        {
                                            IQueryFilter qf = new QueryFilterClass();
                                            qf.WhereClause = "GLOBALID IN (" + whereInClause + ")";
                                            ICursor rowCursor = gisTable.Search(qf, false);
                                            IRow gisRow = null;
                                            while ((gisRow = rowCursor.NextRow()) != null)
                                            {
                                                ShowPercentProgress(string.Format("               {0} out of {1} processed", transactionsProcessed, transactionsToProcess), transactionsProcessed, transactionsToProcess);
                                                Guid globalID = new Guid(gisRow.get_Value(globalIDIdx).ToString());
#if DEBUG
                                                //if (globalID.ToString("B").ToUpper() != "{4334821E-82D6-473C-821A-DCBBDA65614E}") { continue; }
#endif
                                                guidsFound.Add(globalID);
                                                FAIFeatureInformation associatedFAIInfo = GetAssociatedFeatureInfo(FAIFeatInfo, globalID);

                                                try
                                                {
                                                    ((IWorkspaceEdit)EditWorkspace).StartEditOperation();
                                                    bool transactionSuccess = ProcessTransaction(associatedFAIInfo, gisRow);
                                                    ((IWorkspaceEdit)EditWorkspace).StopEditOperation();
                                                    successes++;
                                                    transactionsProcessed++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    ((IWorkspaceEdit)EditWorkspace).AbortEditOperation();
                                                    transactionsProcessed++;
                                                    ReportErroredFeature(associatedFAIInfo, "Failed to process " + transactionType + ": " + ex.Message + " StackTrace: " + ex.StackTrace);
                                                    failures++;
                                                }
                                                while (Marshal.ReleaseComObject(gisRow) > 0) { }
                                            }
                                            while (Marshal.ReleaseComObject(qf) > 0) { }
                                            while (Marshal.ReleaseComObject(rowCursor) > 0) { }
                                            ShowPercentProgress(string.Format("               {0} out of {1} processed", transactionsProcessed, transactionsToProcess), transactionsProcessed, transactionsToProcess);
                                        }

                                        //Check for features not found in the GIS database
                                        List<Guid> missingGuids = new List<Guid>();
                                        foreach (FAIFeatureInformation featInfo in FAIFeatInfo)
                                        {
                                            if (!guidsFound.Contains(featInfo.OriginalGlobalID))
                                            {
                                                missingGuids.Add(featInfo.OriginalGlobalID);
                                                ReportMissingFeature(featInfo, "Feature is missing for transaction type " + transactionType);
                                                transactionsProcessed++;
                                                failures++;
                                                ShowPercentProgress(string.Format("               {0} out of {1} processed", transactionsProcessed, transactionsToProcess), transactionsProcessed, transactionsToProcess);
                                            }
                                        }
                                        ShowPercentProgress(string.Format("               {0} out of {1} processed", transactionsProcessed, transactionsToProcess), transactionsProcessed, transactionsToProcess);
                                    }
                                }

                                if (transactionType == TransactionType.Insert)
                                {
                                    resultCounts.InsertsFailed += failures;
                                    resultCounts.InsertsProcessed += successes;
                                }
                                else if (transactionType == TransactionType.Update)
                                {
                                    resultCounts.UpdatesFailed += failures;
                                    resultCounts.UpdatesProcessed += successes;
                                }
                                else if (transactionType == TransactionType.Delete)
                                {
                                    resultCounts.DeletesFailed += failures;
                                    resultCounts.DeletesProcessed += successes;
                                }

                                if (transactionsToProcess > 0)
                                {
                                    Console.WriteLine();
                                    Logger.Info("               " + successes + " successfully processed for transaction type " + transactionType);
                                    Logger.Info("               " + failures + " failed to process for transaction type " + transactionType);
                                    //Save any edits that were completed for this table
                                    if (((IWorkspaceEdit)EditWorkspace).IsBeingEdited())
                                    {
                                        Console.WriteLine("               Saving Edits...");
                                        ((IWorkspaceEdit)EditWorkspace).StopEditing(true);
                                        ((IWorkspaceEdit)EditWorkspace).StartEditing(false);
                                    }
                                }
                                while (Marshal.ReleaseComObject(gisTable) > 0) { }
                            }
                        }
                    }
                    Console.WriteLine();
                    Logger.Info("Completed processing all tables");

                    //Update all of the relationships as needed
                    ((IWorkspaceEdit)EditWorkspace).StartEditOperation();
                    Logger.Info("Fixing relationships for updated features");
                    UpdateRelationships();
                    Logger.Info("Completed Fixing relationships");
                    ((IWorkspaceEdit)EditWorkspace).StopEditOperation();

                    //Process orphaned records
                    Logger.Info("Cleaning up any orphaned records for updated features");
                    FindOrphanedRecords();
                    Logger.Info("Completed Cleaning up orphaned records");
                }
                catch (Exception ex)
                {
                    if (((IWorkspaceEdit2)EditWorkspace).IsInEditOperation) { ((IWorkspaceEdit)EditWorkspace).AbortEditOperation(); }

                    //Abort any editing that was done
                    if (((IWorkspaceEdit)EditWorkspace).IsBeingEdited())
                    {
                        ((IWorkspaceEdit)EditWorkspace).StopEditing(false);
                    }

                    throw new Exception("Failed to process FAI Features: " + ex.Message + "\r\nStacktrace: " + ex.StackTrace);
                }

                //Save any edits that were completed
                if (((IWorkspaceEdit)EditWorkspace).IsBeingEdited())
                {
                    ((IWorkspaceEdit)EditWorkspace).StopEditing(true);
                }
            }
            catch (Exception ex)
            {
                //Something failed.  Report the erorr and throw the exception
                throw new Exception("Failed FAI processor " + ex.Message + "\r\nStacktrace: " + ex.StackTrace);
            }
            finally
            {

            }
        }

        /// <summary>
        /// Process a FAI transaction record
        /// </summary>
        /// <param name="transaction"></param>
        private bool ProcessTransaction(FAIFeatureInformation FAIFeature, IRow gisRow, ITable gisTable = null)
        {
            bool success = true;
            try
            {
                IRow rowForJPG = gisRow;
                if (FAIFeature.Type == TransactionType.Insert)
                {
                    //Insert a new pole
                    success = ProcessInsert(FAIFeature, gisTable, ref rowForJPG);
                }
                else if (FAIFeature.Type == TransactionType.Delete)
                {
                    //Delete an existing pole
                    success = ProcessDelete(FAIFeature, gisRow);
                }
                else if (FAIFeature.Type == TransactionType.Update)
                {
                    //Update an existing pole
                    if (ShouldReplaceFeature(FAIFeature, gisRow)) { success = ProcessReplace(FAIFeature, gisRow, ref rowForJPG); }
                    else { success = ProcessUpdate(FAIFeature, gisRow); }
                }

                if (success && FAIFeature.Type != TransactionType.Delete) { DetermineJPGMapping(rowForJPG, FAIFeature); }
            }
            catch (Exception ex)
            {
                //Logger.Error("Failed to process " + FAIFeature.OriginalGlobalID.ToString("B").ToUpper() + " from table " + FAIFeature.TableName + ": " + ex.Message + " StackTrace: " + ex.StackTrace);
                if (FAIFeature.Type == TransactionType.Insert) { throw new Exception("Failed to process " + FAIFeature.FAIGlobalID.ToString("B").ToUpper() + " from table " + FAIFeature.TableName + ": " + ex.Message + " StackTrace: " + ex.StackTrace); }
                else { throw new Exception("Failed to process " + FAIFeature.OriginalGlobalID.ToString("B").ToUpper() + " from table " + FAIFeature.TableName + ": " + ex.Message + " StackTrace: " + ex.StackTrace); }
            }
            return success;
        }

        /// <summary>
        /// Resets some local variables for a new session
        /// </summary>
        private void ResetInformation()
        {
            transactionsToProcess = 0;
            transactionsProcessed = 0;
            transactionsSinceLastSave = 0;

            FAIGuidToOriginalGUID = new List<GUIDMap>();
            ExistingOperatingNumbers = new SortedDictionary<string, List<string>>();

            foreach (TableInformation tableInfo in TableInfo)
            {
                foreach (IRelationshipClass relClass in tableInfo.DestinationRelationshipClasses)
                {
                    while (Marshal.ReleaseComObject(relClass) > 0) { }
                }

                foreach (IRelationshipClass relClass in tableInfo.OriginRelationshipClasses)
                {
                    while (Marshal.ReleaseComObject(relClass) > 0) { }
                }
            }
            TableInfo = new List<TableInformation>();
        }

        #endregion

        #region Session Manager

        /// <summary>
        /// Create (or get existing) ArcFM session manager session for FAI Processing
        /// </summary>
        private void CreateMinerSession()
        {
            MinerSession minerSession = new MinerSession(DefaultVersionWorkspace, Common.SessionManagerConnectionString);
            minerSession.ReuseSession = false;
            minerSession.CreateMMSessionVersion(SessionName);
            ParentSession = minerSession;
            EditWorkspace = ParentSession.Version as IVersionedWorkspace;
        }

        /// <summary>
        /// Returns the session name if one exists matching the desired session name
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="desiredSessionName"></param>
        /// <returns></returns>
        private string CurrentFAISessionName(IWorkspace workspace, string desiredSessionName)
        {
            try
            {
                string sessionName = "";
                ITable mmSessionTable = ((IFeatureWorkspace)workspace).OpenTable("PROCESS.MM_SESSION");
                int sessionNameIdx = mmSessionTable.FindField("SESSION_NAME");
                IQueryFilter qf = new QueryFilterClass();
                qf.SubFields = "SESSION_NAME";
                qf.WhereClause = "HIDDEN = 0 AND SESSION_NAME LIKE '%" + desiredSessionName + "%'";
                ICursor rowCursor = mmSessionTable.Search(qf, false);
                IRow row = null;
                while ((row = rowCursor.NextRow()) != null)
                {
                    sessionName = row.get_Value(sessionNameIdx).ToString();
                    while (Marshal.ReleaseComObject(row) > 0) { }
                }

                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
                if (mmSessionTable != null) { while (Marshal.ReleaseComObject(mmSessionTable) > 0) { } }

                return sessionName;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to check PROCESS.MM_SESSION table for existing sessions: " + ex.Message);
            }
        }


        #endregion

        #region Insert Processing

        /// <summary>
        /// Insert the new pole
        /// </summary>
        /// <param name="poleToInsert"></param>
        /// <returns></returns>
        private bool ProcessInsert(FAIFeatureInformation FAIFeature, ITable gisTable, ref IRow createdRow)
        {
            if (gisTable is IFeatureClass)
            {
                IGeometry geometry = FAIFeature.FieldValues["SHAPE"] as IGeometry;
                if (gisTable is INetworkClass && CheckForDuplicateGeometry(gisTable, geometry))
                {
                    ReportConflictingFeature(FAIFeature, "SHAPE", "", "", "Coincident geometry encountered. The FAI feature is wholly contained by another GIS feature", ConflictType.CoincidentGeometry);
                    throw new Exception("Coincident geometry encountered. The FAI feature is wholly contained by another GIS feature");
                }
                else if (gisTable is INetworkClass && CheckForDuplicateOperatingNumber(gisTable, FAIFeature))
                {
                    IField opNumField = ModelNameManager.Instance.FieldFromModelName(gisTable as IObjectClass, "PGE_OPERATINGNUMBER");
                    ReportConflictingFeature(FAIFeature, opNumField.Name, "", "", "Duplicate operating number found on same circuit.", ConflictType.DuplicateOperatingNumber);
                    throw new Exception("Duplicate operating number found on same circuit.");
                }
                else if (CheckForDuplicatePoles(gisTable, FAIFeature))
                {
                    ReportConflictingFeature(FAIFeature, "BARCODE", "", "", "Duplicate barcode found for new pole.", ConflictType.DuplicateBarcode);
                    throw new Exception("Duplicate barcode found for new pole.");
                }
            }

            IRow newRow = gisTable.CreateRow();
            CopyAttributes(FAIFeature, newRow, true);

            newRow.Store();

            int guidIdx = FindFieldIndex(newRow.Table, "GLOBALID");
            string guid = newRow.get_Value(guidIdx).ToString();
            GUIDMap newMap = new GUIDMap();
            newMap.TableName = ((IDataset)gisTable).BrowseName;
            newMap.FAIGuid = FAIFeature.FAIGlobalID.ToString("B").ToUpper();
            newMap.GISGuid = guid.ToUpper();
            FAIGuidToOriginalGUID.Add(newMap);
            createdRow = newRow;

            return true;
        }

        /// <summary>
        /// Checks to verify that this feature is not whollly contained by another feature
        /// </summary>
        /// <param name="gisTable"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private bool CheckForDuplicateGeometry(ITable gisTable, IGeometry geometry)
        {
            ISpatialFilter sf = new SpatialFilterClass();
            sf.Geometry = ((ITopologicalOperator)geometry).Buffer(1);
            sf.GeometryField = "SHAPE";
            sf.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
            return gisTable.RowCount(sf) > 0;
        }

        /// <summary>
        /// Checks for a duplicate support structure that has a matching bar code value
        /// </summary>
        /// <param name="gisTable"></param>
        /// <param name="FAIFeature"></param>
        /// <returns></returns>
        private bool CheckForDuplicatePoles(ITable gisTable, FAIFeatureInformation FAIFeature)
        {
            bool duplicatePoles = false;
            if (!ModelNameManager.Instance.ContainsClassModelName(gisTable as IObjectClass, "PGE_SUPPORTSTRUCTURE")) { return false; }

            int barCodeIdx = gisTable.FindField("BARCODE");

            if (barCodeIdx > 0)
            {
                object FAIBarCodeObj = FAIFeature.FieldValues["BARCODE"];
                string FAIBarCode = "";
                if (FAIBarCodeObj != null) { FAIBarCode = FAIBarCodeObj.ToString(); }
                if (string.IsNullOrEmpty(FAIBarCode)) { duplicatePoles = false; }
                else
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = "BARCODE = " + FAIBarCode;
                    int count = gisTable.RowCount(qf);
                    while (Marshal.ReleaseComObject(qf) > 0) { }
                    duplicatePoles = count > 0;
                }
            }
            return duplicatePoles;
        }

        /// <summary>
        /// Determines if the FAI feature has an operating number that already exists on the given feeder
        /// </summary>
        /// <param name="gisTable"></param>
        /// <param name="FAIFeature"></param>
        /// <returns></returns>
        private bool CheckForDuplicateOperatingNumber(ITable gisTable, FAIFeatureInformation FAIFeature)
        {
            IField OperatingNumberField = ModelNameManager.Instance.FieldFromModelName(gisTable as IObjectClass, "PGE_OPERATINGNUMBER");

            if (OperatingNumberField != null)
            {
                IField FID = ModelNameManager.Instance.FieldFromModelName(gisTable as IObjectClass, "FEEDERID");
                IField FID2 = ModelNameManager.Instance.FieldFromModelName(gisTable as IObjectClass, "FEEDERID2");
                string FIDValue = "";
                string FID2Value = "";
                if (FID != null) { if (FAIFeature.FieldValues[FID.Name] != null) { FIDValue = FAIFeature.FieldValues[FID.Name].ToString(); } }
                if (FID2 != null) { if (FAIFeature.FieldValues[FID2.Name] != null) { FID2Value = FAIFeature.FieldValues[FID2.Name].ToString(); } }

                string operatingNumber = "";
                if (FAIFeature.FieldValues[OperatingNumberField.Name] != null) { operatingNumber = FAIFeature.FieldValues[OperatingNumberField.Name].ToString(); }

                if (!string.IsNullOrEmpty(operatingNumber))
                {
                    if (ExistingOperatingNumbers.ContainsKey(FIDValue) && ExistingOperatingNumbers[FIDValue].Contains(operatingNumber)) { return true; }
                    if (!string.IsNullOrEmpty(FID2Value) && ExistingOperatingNumbers.ContainsKey(FID2Value) &&
                        ExistingOperatingNumbers[FID2Value].Contains(operatingNumber)) { return true; }
                }
            }
            return false;
        }

        /// <summary>
        /// Obtains a list of all existing operating numbers in the system today
        /// </summary>
        private void GetExistingOperatingNumbers()
        {
            ITable mmFieldModelNames = ((IFeatureWorkspace)EditWorkspace).OpenTable("SDE.MM_FIELD_MODELNAMES");
            IQueryFilter qf = new QueryFilterClass();
            IQueryFilterDefinition qfd = qf as IQueryFilterDefinition;
            qfd.PostfixClause = "ORDER BY OBJECTCLASSNAME";

            qf.WhereClause = "MODELNAME = 'PGE_OPERATINGNUMBER'";
            int objClassIdx = mmFieldModelNames.FindField("OBJECTCLASSNAME");
            int FieldNameIdx = mmFieldModelNames.FindField("FIELDNAME");

            List<string> ProcessedFeatureClasses = new List<string>();
            ICursor rowCursor = mmFieldModelNames.Search(qf, false);
            IRow row = null;
            while ((row = rowCursor.NextRow()) != null)
            {
                string objectClassName = row.get_Value(objClassIdx).ToString();
                string fieldName = row.get_Value(FieldNameIdx).ToString();

                if (ProcessedFeatureClasses.Contains(objectClassName)) { continue; }

                ITable table = ((IFeatureWorkspace)EditWorkspace).OpenTable(objectClassName);
                if (table != null)
                {
                    ProcessedFeatureClasses.Add(objectClassName);
                    Logger.Info("     Processing " + objectClassName);
                    IField FID = ModelNameManager.Instance.FieldFromModelName(table as IObjectClass, "FEEDERID");
                    IField FID2 = ModelNameManager.Instance.FieldFromModelName(table as IObjectClass, "FEEDERID2");
                    int opNumIdx = table.FindField(fieldName);
                    int FIDIdx = -1;
                    if (FID != null) { FIDIdx = table.FindField(FID.Name); }
                    int FID2Idx = -1;
                    if (FID2 != null) { FID2Idx = table.FindField(FID2.Name); }

                    qfd.PostfixClause = "";
                    qf.WhereClause = fieldName + " is not null";
                    qf.SubFields = fieldName;
                    if (FID != null) { qf.SubFields += "," + FID.Name; }
                    if (FID2 != null) { qf.SubFields += "," + FID2.Name; }
                    ICursor opNumCursor = table.Search(qf, false);
                    IRow opNumRow = null;
                    while ((opNumRow = opNumCursor.NextRow()) != null)
                    {
                        string opNum = opNumRow.get_Value(opNumIdx).ToString();
                        object FIDValueObj = null;
                        if (FIDIdx > 0) { FIDValueObj = opNumRow.get_Value(FIDIdx).ToString(); }
                        object FID2ValueObj = null;
                        if (FID2Idx > 0) { FID2ValueObj = opNumRow.get_Value(FID2Idx).ToString(); }
                        string FIDValue = "";
                        if (FIDValueObj != null) { FIDValue = FIDValueObj.ToString(); }
                        string FID2Value = "";
                        if (FID2ValueObj != null) { FID2Value = FID2ValueObj.ToString(); }

                        if (!ExistingOperatingNumbers.ContainsKey(FIDValue)) { ExistingOperatingNumbers.Add(FIDValue, new List<string>()); }
                        if (!ExistingOperatingNumbers.ContainsKey(FID2Value)) { ExistingOperatingNumbers.Add(FID2Value, new List<string>()); }

                        ExistingOperatingNumbers[FIDValue].Add(opNum);
                        if (!string.IsNullOrEmpty(FID2Value)) { ExistingOperatingNumbers[FID2Value].Add(opNum); }

                        while (Marshal.ReleaseComObject(opNumRow) > 0) { }
                    }
                    while (Marshal.ReleaseComObject(opNumCursor) > 0) { }
                }
                while (Marshal.ReleaseComObject(table) > 0) { }
                while (Marshal.ReleaseComObject(row) > 0) { }
            }
            while (Marshal.ReleaseComObject(qf) > 0) { }
            while (Marshal.ReleaseComObject(rowCursor) > 0) { }
            while (Marshal.ReleaseComObject(mmFieldModelNames) > 0) { }
        }

        

        #endregion

        #region Update Processing

        /// <summary>
        /// Update the existing pole
        /// </summary>
        /// <param name="poleToUpdate"></param>
        /// <returns></returns>
        private bool ProcessUpdate(FAIFeatureInformation FAIFeature, IRow gisRow)
        {
            CopyAttributes(FAIFeature, gisRow, false);
            gisRow.Store();

            //Make note of this global ID map
            GUIDMap newMap = new GUIDMap();
            newMap.TableName = ((IDataset)gisRow.Table).BrowseName;
            newMap.FAIGuid = FAIFeature.FAIGlobalID.ToString("B").ToUpper();
            newMap.GISGuid = FAIFeature.OriginalGlobalID.ToString("B").ToUpper();
            FAIGuidToOriginalGUID.Add(newMap);

            return true;
        }

        /// <summary>
        /// Replace the existing pole
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private bool ProcessReplace(FAIFeatureInformation FAIFeature, IRow gisRow, ref IRow createdRow)
        {
            IFeature assetToReplace = gisRow as IFeature;
            bool success = true;
            /*
             * This replace operation must be modified to use the same logic as the PGE - Asset Replace functionality.  After, the attributes that were passed in with the 
             * replace transaction should be utilized and updated for the new feature.
             */
            int globalIDIdx = FindFieldIndex(assetToReplace.Table, "GLOBALID");
            Guid deletedFeatureGUID = new Guid(assetToReplace.get_Value(globalIDIdx).ToString());

            //Get list of annotation features existing prior to replace
            List<IFeature> existingAnnotationFeatures = DetermineRelatedAnnotationsFeatures(assetToReplace);

            IFeature newStructure = ReplaceAssetLogic(assetToReplace);

            //Get our projected geometry and assign it to our new feature.  For now, we will just use the original geometry during a replace
            //IPoint projectedPoint = ProjectLatLongPoint(poleToReplace.PoleToInsert.Latitude, poleToReplace.PoleToInsert.Longitude, ((IGeoDataset)newStructure.Class).SpatialReference);
            //newStructure.Shape = projectedPoint as IGeometry;

            string newFeatureGuid = newStructure.get_Value(globalIDIdx).ToString();

            //Update the replaceguid field on the new structure to the one we are deleting
            int replaceGuidIdx = FindFieldIndex(newStructure.Table, "REPLACEGUID");
            string deletedGuid = deletedFeatureGUID.ToString("B").ToUpper();
            newStructure.set_Value(replaceGuidIdx, deletedGuid);
            //SetInstallJobNumber(newStructure);

            //Now that the feature has been replaced we can copy the attributes from the FAI feature
            CopyAttributes(FAIFeature, newStructure, true);
            newStructure.Store();

            //Determine the new annotation features and delete them as we don't want to create any new annotation features
            List<IFeature> newAnnotationFeatures = DetermineRelatedAnnotationsFeatures(newStructure);
            foreach (IFeature newAnno in newAnnotationFeatures)
            {
                bool newAnnotation = true;
                foreach (IFeature oldAnno in existingAnnotationFeatures)
                {
                    if (oldAnno.Class == newAnno.Class && oldAnno.OID == newAnno.OID) { newAnnotation = false; }
                }

                if (newAnnotation) { newAnno.Delete(); }
            }

            //Notify the existing annotations that the related feature was updated to ensure that the annotation is updated accordingly.
            NotifyAnnotationOfChanges(newStructure, existingAnnotationFeatures);

            //Make note of this GUID map from FAI to GIS
            int guidIdx = FindFieldIndex(newStructure.Table, "GLOBALID");
            string guid = newStructure.get_Value(guidIdx).ToString();
            GUIDMap newMap = new GUIDMap();
            newMap.TableName = ((IDataset)newStructure.Table).BrowseName;
            newMap.FAIGuid = FAIFeature.FAIGlobalID.ToString("B").ToUpper();
            newMap.GISGuid = guid.ToUpper();
            FAIGuidToOriginalGUID.Add(newMap);
            createdRow = newStructure;

            return success;

        }


        /// <summary>
        /// Determines if the specified FAI feature should be treated as a replace operation instead of a simple update
        /// </summary>
        /// <param name="FAIFeature"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private bool ShouldReplaceFeature(FAIFeatureInformation FAIFeature, IRow row)
        {
            List<string> FeatureClassesToReplace = new List<string>();
            FeatureClassesToReplace.Add("EDGIS.CapacitorBank".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.DynamicProtectiveDevice".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.FaultIndicator".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.OpenPoint".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.PadmountStructure".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.StepDownUnit".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.StreetLight".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.SubsurfaceStructure".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.SupportStructure".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.Switch".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.Transformer".ToUpper());
            FeatureClassesToReplace.Add("EDGIS.VoltageRegulator".ToUpper());

            //Update the subtype field first so that our domain checks are valid
            ISubtypes subtypes = row.Table as ISubtypes;
            if (subtypes.HasSubtype)
            {
                TableInformation GISTableInfo = GetTableInformation(row.Table);
                string subtypeFieldName = subtypes.SubtypeFieldName;
                int subtypeIdx = GISTableInfo.GetFieldIndex(subtypeFieldName);
                IField subtypeField = row.Fields.get_Field(subtypeIdx);
                object newValue = FAIFeature.FieldValues[subtypeFieldName.ToUpper()];
                object oldValue = row.get_Value(subtypeIdx);

                if (!RowValueEquals(oldValue, newValue, subtypeField.Type, false)
                    && FeatureClassesToReplace.Contains(((IDataset)row.Table).BrowseName.ToUpper()))
                {
                    //This feature class is in our list and the subtype was changed so it needs to
                    //be replaced
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines any currently related annotation features
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private List<IFeature> DetermineRelatedAnnotationsFeatures(IFeature feature)
        {
            List<IFeature> originalAnnotationFeatures = new List<IFeature>();
            if (feature != null)
            {
                //Process relationships where this class is the destination
                TableInformation featClassInfo = GetTableInformation(feature.Class as ITable);

                if (featClassInfo != null)
                {
                    //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
                    IObjectClass objClass = feature.Class as IObjectClass;

                    //Process relationships where this class is the origin
                    foreach (IRelationshipClass relClass in featClassInfo.OriginRelationshipClasses)
                    {
                        //Don't map annotation relationships
                        if (relClass.DestinationClass is IFeatureClass && ((IFeatureClass)relClass.DestinationClass).FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            ISet relatedFeatures = relClass.GetObjectsRelatedToObject(feature);
                            relatedFeatures.Reset();
                            for (int i = 0; i < relatedFeatures.Count; i++)
                            {
                                IFeature annoFeature = relatedFeatures.Next() as IFeature;
                                originalAnnotationFeatures.Add(annoFeature);
                            }
                        }
                    }
                }
            }

            return originalAnnotationFeatures;
        }

        /// <summary>
        /// Fires the esri notifications to annotation features to update their information
        /// </summary>
        /// <param name="newFeature"></param>
        /// <param name="annotationFeatures"></param>
        private void NotifyAnnotationOfChanges(IFeature newFeature, List<IFeature> annotationFeatures)
        {
#if DEBUG
            //Debugger.Launch();
#endif
            if (newFeature != null)
            {
                //Process relationships where this class is the destination
                TableInformation featClassInfo = GetTableInformation(newFeature.Class as ITable);

                if (featClassInfo != null)
                {
                    //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
                    IObjectClass objClass = newFeature.Class as IObjectClass;

                    //Process relationships where this class is the origin
                    foreach (IRelationshipClass relClass in featClassInfo.OriginRelationshipClasses)
                    {
                        //Don't map annotation relationships
                        if (relClass.DestinationClass is IFeatureClass && ((IFeatureClass)relClass.DestinationClass).FeatureType == esriFeatureType.esriFTAnnotation)
                        {
                            foreach (IFeature annoFeature in annotationFeatures)
                            {
                                if (annoFeature.Class == relClass.DestinationClass)
                                {
                                    //Correct annotation relationship class found
                                    IRelatedObjectEvents relObjectEvents = annoFeature as IRelatedObjectEvents;
                                    relObjectEvents.RelatedObjectChanged(relClass, newFeature);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Replace logic code modified from the PGE Asset Replace Desktop functionality
        /// </summary>
        private IFeature ReplaceAssetLogic(IFeature oldObject)
        {
            IFeature newFeature = null;

            //Set flag to indicate Enable Symbol Number AU 
            EvaluationEngine.Instance.EnableSymbolNumberAU = true;

            Dictionary<int, object> fieldMapping = null;
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;
            try
            {
                IFeatureClass objectClass = oldObject.Class as IFeatureClass;

                //Get list of fields with modelname PGE_ASSETCOPY which will be 
                //copied from the old object to the new object 
                fieldMapping = GetValuesToMap(oldObject);

                //Update relationships from old feature to new feature
                relationshipMapping = GetRelationshipMapping(oldObject, false);

                int installJobPrefixFldIdx = FindFieldIndex(oldObject.Class as ITable, "installjobprefix");

                //Give the install job prefix the default value for the field
                string installJobPrefix = "";
                if (!fieldMapping.ContainsKey(installJobPrefixFldIdx))
                {
                    installJobPrefix = oldObject.Fields.get_Field(
                        installJobPrefixFldIdx).DefaultValue.ToString();
                }
                string jobNumberFromMemory = PGE.Desktop.EDER.ArcMapCommands.
                    PopulateLastJobNumberUC.jobNumber;

                //Set flag to indicate Disable Symbol Number AU 
                EvaluationEngine.Instance.EnableSymbolNumberAU = false;

                //Create our new feature
                IObject newObject = CreateNewObject(fieldMapping, objectClass, oldObject);
                IFeature newStructure = newObject as IFeature;

                //Set all relationships back up with the new feature
                SetRelationships(relationshipMapping, newObject);

                oldObject.Delete();

                //*******************************************************
                //INC000004070566 
                //Trigger the store to update the anno and make sure it is 
                //in sync with the source feature e.g. may include job number 
                //int installjobNumFldIdx = FindFieldIndex(newObject.Class as ITable, "installjobnumber");
                //newObject.set_Value(installJobPrefixFldIdx, installJobPrefix);
                //newObject.set_Value(installjobNumFldIdx, jobNumberFromMemory);
                newFeature = newObject as IFeature;

                return newFeature;
                //******************************************************* 
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to replace asset: " + e.Message);
                throw new Exception("Failed to replace asset: " + e.Message);

            }
            finally
            {
                //Set flag to indicate Disable Symbol Number AU 
                EvaluationEngine.Instance.EnableSymbolNumberAU = false;
                //Cleanup
                if (fieldMapping != null)
                {
                    fieldMapping.Clear();
                }
            }
        }

        /// <summary>
        /// Returns a list of all the fields that are configured for the asset replacement field copy
        /// </summary>
        /// <param name="origOject"></param>
        /// <returns></returns>
        private Dictionary<int, object> GetValuesToMap(IObject origOject) //IFeature origFeature
        {
            Dictionary<int, object> valuesToMap = new Dictionary<int, object>();
            List<int> fieldMapping = ModelNameFacade.FieldIndicesFromModelName(origOject.Class, SchemaInfo.General.FieldModelNames.AssetCopyFMN);
            foreach (int fieldIndex in fieldMapping)
            {
                valuesToMap.Add(fieldIndex, origOject.get_Value(fieldIndex));
            }
            return valuesToMap;
        }

        /// <summary>
        /// Relates all features that were related with the previous feature to the newly created feature
        /// </summary>
        /// <param name="relationshipMapping"></param>
        /// <param name="newObject"></param>
        private void SetRelationships(Dictionary<IRelationshipClass, List<IObject>> relationshipMapping, IObject newObject) //, IFeature newFeature
        {
            TableInformation featClassInfo = GetTableInformation(newObject.Class as ITable);

            //First delete any new anno that has been created for the new object 
            //since we are going to use the old anno 
            foreach (IRelationshipClass pRelClass in featClassInfo.OriginRelationshipClasses)
            {
                if (pRelClass.DestinationClass is IFeatureClass && ((IFeatureClass)pRelClass.DestinationClass).FeatureType ==
                    esriFeatureType.esriFTAnnotation)
                {
                    ISet pSet = pRelClass.GetObjectsRelatedToObject(newObject);
                    IObject pRelObj = (IObject)pSet.Next();
                    while (pRelObj != null)
                    {
                        pRelObj.Delete();
                        pRelObj = (IObject)pSet.Next();
                    }
                }
            }


            foreach (KeyValuePair<IRelationshipClass, List<IObject>> kvp in relationshipMapping)
            {
                bool isOrigin = false;
                if (kvp.Key.OriginClass == newObject.Class)
                {
                    isOrigin = true;
                }

                if (isOrigin)
                {
                    foreach (IObject obj in kvp.Value)
                    {
                        if (obj != null)
                        {
                            try { kvp.Key.CreateRelationship(newObject, obj); }
                            catch (Exception ex)
                            {
                                string error = "Failed to relate to " + ((IDataset)obj.Class).BrowseName + " with OID " + obj.OID + " Error: " + ex.Message;
                                ReportErroredRelationship(newObject, error);
                            }
                        }
                    }
                }
                else
                {
                    foreach (IObject obj in kvp.Value)
                    {
                        if (obj != null)
                        {
                            try { kvp.Key.CreateRelationship(obj, newObject); }
                            catch (Exception ex)
                            {
                                string error = "Failed to relate to " + ((IDataset)obj.Class).BrowseName + " with OID " + obj.OID + " Error: " + ex.Message;
                                ReportErroredRelationship(newObject, error);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method will create the new feature based off of the configured values to map
        /// from the original feature
        /// </summary>
        /// <param name="origFeature"></param>
        private IFeature CreateNewObject(Dictionary<int, object> fieldMapping, IFeatureClass objectClass, IFeature replacedObject) //Dictionary<int, object> fieldMapping, IFeatureClass featClass, IFeature replacedFeature
        {
            IFeature feat = (objectClass as IFeatureClass).CreateFeature();

            foreach (KeyValuePair<int, object> kvp in fieldMapping)
            {
                if (kvp.Value != null)
                    feat.set_Value(kvp.Key, kvp.Value);
            }

            feat.Shape = ((IFeature)replacedObject).ShapeCopy;

            int guidIdx = FindFieldIndex(objectClass as ITable, "GLOBALID");
            int ReplaceGUIDIndex = FindFieldIndex(feat.Class as ITable, "REPLACEGUID");
            feat.set_Value(ReplaceGUIDIndex, replacedObject.get_Value(guidIdx));

            return feat;
        }

        /// <summary>
        /// This method will take all relationships that are current assigned to the oldFeature, remove
        /// them and then apply them to the new feature.
        /// </summary>
        /// <param name="oldObject">Feature being replaced</param>
        private Dictionary<IRelationshipClass, List<IObject>> GetRelationshipMapping(IObject oldObject, bool rebuildCache) //IFeature oldFeature
        {
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;

            if (oldObject != null)
            {
                //Process relationships where this class is the destination
                TableInformation featClassInfo = GetTableInformation(oldObject.Class as ITable);

                //Relationship mapping wasn't cached so we need to do some work
                relationshipMapping = new Dictionary<IRelationshipClass, List<IObject>>();

                //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
                IObjectClass objClass = oldObject.Class as IObjectClass;

                //FeatureClassInformation featInfo = featureClassInformation[
                foreach (IRelationshipClass relClass in featClassInfo.DestinationRelationshipClasses)
                {
                    if (!relationshipMapping.ContainsKey(relClass))
                    {
                        relationshipMapping.Add(relClass, new List<IObject>());
                    }
                    ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
                    relatedFeatures.Reset();
                    for (int i = 0; i < relatedFeatures.Count; i++)
                    {
                        IObject obj = relatedFeatures.Next() as IObject;
                        relationshipMapping[relClass].Add(obj);
                        relClass.DeleteRelationship(obj, oldObject);
                        //relClass.CreateRelationship(obj, newFeature);
                    }
                }


                //Process relationships where this class is the origin
                foreach (IRelationshipClass relClass in featClassInfo.OriginRelationshipClasses)
                {
                    //Don't map annotation relationships
                    if (relClass.DestinationClass is IFeatureClass)
                    {
                        IFeatureClass originClass = relClass.DestinationClass as IFeatureClass;

                        //Simon Change remove this (we want to include the anno) 
                        if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                            System.Diagnostics.Debug.Print(originClass.AliasName);
                        //if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                        //{
                        //    relClass = relClasses.Next();
                        //    continue;
                        //}
                    }
                    if (!relationshipMapping.ContainsKey(relClass))
                    {
                        relationshipMapping.Add(relClass, new List<IObject>());
                    }
                    ISet relatedFeatures = relClass.GetObjectsRelatedToObject(oldObject);
                    relatedFeatures.Reset();
                    for (int i = 0; i < relatedFeatures.Count; i++)
                    {
                        IObject obj = relatedFeatures.Next() as IObject;
                        relationshipMapping[relClass].Add(obj);
                        relClass.DeleteRelationship(obj, oldObject);
                        System.Diagnostics.Debug.Print("Deleting rel to OId: " + obj.OID.ToString());
                        //relClass.CreateRelationship(obj, newFeature);
                    }
                }
            }
            return relationshipMapping;
        }


        #endregion

        #region Delete Processing

        /// <summary>
        /// Delete the existing pole
        /// </summary>
        /// <param name="poleToDelete"></param>
        private bool ProcessDelete(FAIFeatureInformation FAIFeature, IRow gisRow)
        {
            //Delete any configured relationships first
            DeleteRelatedAnnotation(gisRow);

            //Delete the gisRow
            gisRow.Delete();

            //Make note of this global ID map
            GUIDMap newMap = new GUIDMap();
            newMap.TableName = ((IDataset)gisRow.Table).BrowseName;
            newMap.FAIGuid = FAIFeature.FAIGlobalID.ToString("B").ToUpper();
            newMap.GISGuid = FAIFeature.OriginalGlobalID.ToString("B").ToUpper();
            FAIGuidToOriginalGUID.Add(newMap);

            return true;
        }

        /// <summary>
        /// Deletes any related annotation features
        /// </summary>
        /// <param name="gisRow"></param>
        private void DeleteRelatedAnnotation(IRow gisRow)
        {
            TableInformation tableInfo = GetTableInformation(gisRow.Table);

            //Process relationships where this row is the destination class
            foreach (IRelationshipClass relClass in tableInfo.DestinationRelationshipClasses)
            {
                if (relClass.OriginClass is IFeatureClass && ((IFeatureClass)relClass.OriginClass).FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    //Delete our associated annotation
                    ISet relatedRows = relClass.GetObjectsRelatedToObject(gisRow as IObject);
                    relatedRows.Reset();
                    IFeature feature = null;
                    while ((feature = relatedRows.Next() as IFeature) != null)
                    {
                        feature.Delete();
                        while (Marshal.ReleaseComObject(feature) > 0) { }
                    }
                    while (Marshal.ReleaseComObject(relatedRows) > 0) { }
                }
            }

            //Process relationships where this row is the origin class
            foreach (IRelationshipClass relClass in tableInfo.OriginRelationshipClasses)
            {
                if (relClass.DestinationClass is IFeatureClass && ((IFeatureClass)relClass.DestinationClass).FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    //Delete our associated annotation
                    ISet relatedRows = relClass.GetObjectsRelatedToObject(gisRow as IObject);
                    relatedRows.Reset();
                    IFeature feature = null;
                    while ((feature = relatedRows.Next() as IFeature) != null)
                    {
                        feature.Delete();
                        while (Marshal.ReleaseComObject(feature) > 0) { }
                    }
                    while (Marshal.ReleaseComObject(relatedRows) > 0) { }
                }
            }
        }


        #endregion

        #region Post Processing Methods

        private void DetermineJPGMapping(IRow gisRow, FAIFeatureInformation faiFeature)
        {
            try
            {
                int globalIDIdx = FindFieldIndex(gisRow.Table, "GLOBALID");
                if (globalIDIdx > -1)
                {
                    string globalID = gisRow.get_Value(globalIDIdx).ToString().ToUpper();
                    for (int i = 1; i < 50; i++)
                    {
                        string photoFieldName = "PHOTO" + i;
                        if (faiFeature.FieldValues.ContainsKey(photoFieldName))
                        {
                            object fieldValue = faiFeature.FieldValues[photoFieldName];
                            if (fieldValue != null && !string.IsNullOrEmpty(fieldValue.ToString())
                                && !string.IsNullOrEmpty(fieldValue.ToString().Trim()))
                            {
                                string sql = "INSERT INTO EDGIS.PGE_FAI_JPGMapping (TableName, GLOBALID, JPGNAME) VALUES ('" + ((IDataset)gisRow.Table).BrowseName + "','" + globalID + "','" + fieldValue.ToString() + "')";
                                DefaultVersionWorkspace.ExecuteSQL(sql);
                            }
                        }
                        else { break; }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to updated EDGIS.PGE_FAI_JPGMapping: " + ex.Message);
            }
        }

        /// <summary>
        /// Finds and deletes any stand alone table records that were orphaned and deletes them
        /// </summary>
        private void UpdateRelationships()
        {

            //This method will process all of the edited records and determine any orphaned records that need to
            //be deleted or logged

            SortedDictionary<string, List<string>> GISTableNameToGUID = new SortedDictionary<string, List<string>>();
            SortedDictionary<string, List<string>> FAITableNameToGUID = new SortedDictionary<string, List<string>>();
            foreach (GUIDMap map in FAIGuidToOriginalGUID)
            {
                string tableName = map.TableName;
                if (!FAITableNameToGUID.ContainsKey(tableName)) { FAITableNameToGUID.Add(tableName, new List<string>()); }
                if (!GISTableNameToGUID.ContainsKey(tableName)) { GISTableNameToGUID.Add(tableName, new List<string>()); }
                FAITableNameToGUID[tableName].Add(map.FAIGuid);
                GISTableNameToGUID[tableName].Add(map.GISGuid);
            }

            foreach (KeyValuePair<string, List<string>> kvp in FAITableNameToGUID)
            {
                string tableName = kvp.Key;
                Logger.Info("     Processing : " + tableName);

                ITable gisTable = ((IFeatureWorkspace)EditWorkspace).OpenTable(tableName);
                ITable FAITable = ((IFeatureWorkspace)FAIWorkspace).OpenTable(tableName.Remove("EDGIS."));
                TableInformation gisTableInfo = GetTableInformation(gisTable);
                TableInformation FAITableInfo = GetTableInformation(FAITable);

                int gisGuidIdx = FindFieldIndex(gisTable, "GLOBALID");
                List<string> guids = kvp.Value;

                List<FAIFeatureInformation> FAIFeatInfo = GetFAIFeatureInformation(tableName.Remove("EDGIS."), guids);

                List<string> whereInClauses = Common.GetWhereInClauses(GISTableNameToGUID[tableName]);
                foreach (string whereInClause in whereInClauses)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = "GLOBALID IN (" + whereInClause + ")";
                    ICursor rowCursor = gisTable.Search(qf, false);
                    IRow row = null;
                    while ((row = rowCursor.NextRow()) != null)
                    {
                        string gisGuid = row.get_Value(gisGuidIdx).ToString().ToUpper();
                        string FAIGuid = "";
                        for (int i = 0; i < FAIGuidToOriginalGUID.Count; i++)
                        {
                            if (gisGuid == FAIGuidToOriginalGUID[i].GISGuid)
                            {
                                FAIGuid = FAIGuidToOriginalGUID[i].FAIGuid;
                                break;
                            }
                        }

                        FAIFeatureInformation FAIFeatInformation = null;
                        foreach (FAIFeatureInformation info in FAIFeatInfo)
                        {
                            if (info.FAIGlobalID.ToString("B").ToUpper() == FAIGuid)
                            {
                                FAIFeatInformation = info;
                                break;
                            }
                        }

                        if (FAIFeatInformation != null)
                        {
                            ProcessRelationshipClass(FAITableInfo, FAIFeatInformation, gisTableInfo.DestinationRelationshipClasses, false, row);
                            ProcessRelationshipClass(FAITableInfo, FAIFeatInformation, gisTableInfo.OriginRelationshipClasses, true, row);
                        }
                    }
                }

                //Release FAI table resources
                while (Marshal.ReleaseComObject(FAITable) > 0) { }
            }
        }

        /// <summary>
        /// This method will create or delete relationships based on what is existing in the FAI database
        /// </summary>
        /// <param name="FAITableInfo"></param>
        /// <param name="FAIFeatInformation"></param>
        /// <param name="relClasses"></param>
        /// <param name="isOrigin"></param>
        /// <param name="gisRow"></param>
        private void ProcessRelationshipClass(TableInformation FAITableInfo, FAIFeatureInformation FAIFeatInformation,
            List<IRelationshipClass> relClasses, bool isOrigin, IRow gisRow)
        {
            //Process our relationship classes
            foreach (IRelationshipClass relClass in relClasses)
            {
                try
                {
                    string relatedTableName = ((IDataset)relClass.OriginClass).BrowseName;
                    if (isOrigin) { relatedTableName = ((IDataset)relClass.DestinationClass).BrowseName; }

                    //Don't process annotation relationship classes. Those are handled elsewhere
                    if (isOrigin && relClass.DestinationClass is IFeatureClass &&
                        ((IFeatureClass)relClass.DestinationClass).FeatureType == esriFeatureType.esriFTAnnotation) { continue; }
                    else if (!isOrigin && relClass.OriginClass is IFeatureClass &&
                        ((IFeatureClass)relClass.OriginClass).FeatureType == esriFeatureType.esriFTAnnotation) { continue; }

                    //Only process a relationship class if it was contained in the FAI database. We don't want to delete relationships
                    //where a relationship class didn't even exist in the FAI datagbase
                    if (isOrigin && !FAITableInfo.OriginRelationshipTableNames.Contains(relatedTableName.Remove("EDGIS."))) { continue; }
                    else if (!isOrigin && !FAITableInfo.DestinationRelationshipTableNames.Contains(relatedTableName.Remove("EDGIS."))) { continue; }

                    //Don't process relationship class if the source and destination classes don't both contain
                    //a global ID value
                    if (relClass.OriginClass.FindField("GLOBALID") < 0 || relClass.DestinationClass.FindField("GLOBALID") < 0)
                    {
                        continue;
                    }

                    //Get the related features in the GIS database
                    ISet relatedObjects = relClass.GetObjectsRelatedToObject(gisRow as IObject);

                    //Process relationships that need to be deleted first
                    relatedObjects.Reset();
                    IObject gisRelatedObject = null;
                    while ((gisRelatedObject = relatedObjects.Next() as IObject) != null)
                    {
                        int relGuidIdx = FindFieldIndex(gisRelatedObject.Table, "GLOBALID");
                        string relGuid = gisRelatedObject.get_Value(relGuidIdx).ToString().ToUpper();
                        bool foundRelatedObject = false;
                        //Found our associated FAI feature information so we can map our relationships back properly
                        foreach (FAIFeatureInformation relatedFeature in FAIFeatInformation.RelatedFeatures)
                        {
                            if (relatedFeature.GISTableName == relatedTableName)
                            {
                                //Found a related feature for this relationship class. Verify the relationship exists
                                string relatedOriginalGuid = GetOriginalGUID(relatedFeature);
                                if (relatedOriginalGuid == relGuid) { foundRelatedObject = true; }
                            }
                        }
                        //Delete the relationship if it wasn't found
                        if (!foundRelatedObject && isOrigin) { relClass.DeleteRelationship(gisRow as IObject, gisRelatedObject); }
                        else if (!foundRelatedObject) { relClass.DeleteRelationship(gisRelatedObject, gisRow as IObject); }
                    }

                    //Process relationships that need to be add next
                    foreach (FAIFeatureInformation relatedFeature in FAIFeatInformation.RelatedFeatures)
                    {
                        if (relatedFeature.GISTableName == relatedTableName)
                        {
                            bool foundRelatedObject = false;
                            string relatedOriginalGuid = GetOriginalGUID(relatedFeature);

                            relatedObjects.Reset();
                            gisRelatedObject = null;
                            while ((gisRelatedObject = relatedObjects.Next() as IObject) != null)
                            {
                                int relGuidIdx = FindFieldIndex(gisRelatedObject.Table, "GLOBALID");
                                string relGuid = gisRelatedObject.get_Value(relGuidIdx).ToString().ToUpper();
                                //Found a related feature for this relationship class. Verify the relationship exists
                                if (relatedOriginalGuid == relGuid) { foundRelatedObject = true; }

                            }

                            if (!foundRelatedObject)
                            {
                                //Existing relationship was not found so we need to create it.
                                ITable relatedTable = ((IFeatureWorkspace)EditWorkspace).OpenTable(relatedTableName);
                                IQueryFilter qf = new QueryFilterClass();
                                qf.WhereClause = "GLOBALID = '" + relatedOriginalGuid + "'";
                                ICursor rowCursor = relatedTable.Search(qf, false);
                                IRow relatedRow = null;
                                while ((relatedRow = rowCursor.NextRow()) != null)
                                {
                                    //Create the relationship
                                    try
                                    {
                                        if (isOrigin) { relClass.CreateRelationship(gisRow as IObject, relatedRow as IObject); }
                                        else { relClass.CreateRelationship(relatedRow as IObject, gisRow as IObject); }
                                    }
                                    catch (Exception ex)
                                    {
                                        string error = "Failed to relate to " + ((IDataset)relatedRow.Table).BrowseName + " with OID " + relatedRow.OID + " Error: " + ex.Message;
                                        ReportErroredRelationship(gisRow, error);
                                    }
                                    while (Marshal.ReleaseComObject(relatedRow) > 0) { }
                                }
                                while (Marshal.ReleaseComObject(rowCursor) > 0) { }
                                while (Marshal.ReleaseComObject(relatedTable) > 0) { }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// Finds and deletes any stand alone table records that were orphaned and deletes them
        /// </summary>
        private void FindOrphanedRecords()
        {
            //This method will process all of the edited records and determine any orphaned records that need to
            //be deleted or logged

            //Need to determine version differences
            DifferenceManager diffManager = new DifferenceManager(((IVersionedWorkspace)EditWorkspace).DefaultVersion, EditWorkspace as IVersion);
            diffManager.Initialize();
            esriDataChangeType[] changeTypes = { esriDataChangeType.esriDataChangeTypeUpdate };
            diffManager.LoadDifferences(false, changeTypes);

            foreach (KeyValuePair<string, DiffTable> kvp in diffManager.Updates)
            {
                string tableName = kvp.Key;
                DiffTable diffTable = kvp.Value;

                ITable editedTable = ((IFeatureWorkspace)EditWorkspace).OpenTable(tableName);
                if (!(editedTable is IFeatureClass))
                {
                    Logger.Info("     Processing " + tableName);
                    ((IWorkspaceEdit)EditWorkspace).StartEditOperation();
                    //Process this table and see if the are any orphaned records to delete
                    TableInformation tableInfo = GetTableInformation(editedTable);
                    foreach (DiffRow row in diffTable)
                    {
                        IRow editedRow = editedTable.GetRow(row.OID);
                        bool hasRelationships = false;
                        foreach (IRelationshipClass relClass in tableInfo.DestinationRelationshipClasses)
                        {
                            ISet relatedObjectSet = relClass.GetObjectsRelatedToObject(editedRow as IObject);
                            if (relatedObjectSet.Count > 0)
                            {
                                hasRelationships = true;
                                break;
                            }
                        }
                        if (!hasRelationships)
                        {
                            foreach (IRelationshipClass relClass in tableInfo.OriginRelationshipClasses)
                            {
                                ISet relatedObjectSet = relClass.GetObjectsRelatedToObject(editedRow as IObject);
                                if (relatedObjectSet.Count > 0)
                                {
                                    hasRelationships = true;
                                    break;
                                }
                            }
                        }

                        if (!hasRelationships)
                        {
                            //Delete the orphaned row
                            editedRow.Delete();
                        }
                        while (Marshal.ReleaseComObject(editedRow) > 0) { }
                    }
                    ((IWorkspaceEdit)EditWorkspace).StopEditOperation();
                }
                while (Marshal.ReleaseComObject(editedTable) > 0) { }
            }
        }

        #endregion

        #region Reporting

        /// <summary>
        /// Reports on any features that are missing from the GIS database
        /// </summary>
        /// <param name="featInfo"></param>
        private void ReportMissingFeature(FAIFeatureInformation featInfo, string missingErrorMessage)
        {
            DateTime now = DateTime.Now;
            string dateString = now.Year + "/" + now.Month + "/" + now.Day + " " + now.Hour + ":" + now.Minute + ":" +
                                now.Second;

            string sql = string.Format("INSERT INTO {0} (TABLENAME, ROWGUID, FAIDATABASE, FAILUREDATE, ERRORMESSAGE) VALUES('{1}','{2}','{3}',"
            + "TO_DATE('{4}', 'yyyy/mm/dd hh24:mi:ss'),'{5}')", "EDGIS.PGE_FAI_MISSINGFEATURES", featInfo.TableName, featInfo.OriginalGlobalID,
            FAIWorkspace.PathName, dateString, missingErrorMessage);
            DefaultVersionWorkspace.ExecuteSQL(sql);
        }

        /// <summary>
        /// Reports on any features that fail to process for any reason
        /// </summary>
        /// <param name="featInfo"></param>
        /// <param name="errorMessage"></param>
        private void ReportErroredFeature(FAIFeatureInformation featInfo, string errorMessage)
        {
            DateTime now = DateTime.Now;
            string dateString = now.Year + "/" + now.Month + "/" + now.Day + " " + now.Hour + ":" + now.Minute + ":" +
                                now.Second;

            string sql = string.Format("INSERT INTO {0} (TABLENAME, ROWGUID, FAIDATABASE, FAILUREDATE, ERRORMESSAGE) VALUES('{1}','{2}','{3}',"
            + "TO_DATE('{4}', 'yyyy/mm/dd hh24:mi:ss'),'{5}')", "EDGIS.PGE_FAI_ERRORS", featInfo.TableName, featInfo.OriginalGlobalID,
            FAIWorkspace.PathName, dateString, errorMessage);
            DefaultVersionWorkspace.ExecuteSQL(sql);
        }

        /// <summary>
        /// Reports on any features that fail to relate for any reason
        /// </summary>
        /// <param name="featInfo"></param>
        /// <param name="errorMessage"></param>
        private void ReportErroredRelationship(IRow row, string errorMessage)
        {
            string tableName = ((IDataset)row.Table).BrowseName;
            int globalIDx = FindFieldIndex(row.Table, "GLOBALID");
            string globalID = row.get_Value(globalIDx).ToString().ToUpper();
            DateTime now = DateTime.Now;
            string dateString = now.Year + "/" + now.Month + "/" + now.Day + " " + now.Hour + ":" + now.Minute + ":" +
                                now.Second;

            string sql = string.Format("INSERT INTO {0} (TABLENAME, ROWGUID, FAIDATABASE, FAILUREDATE, ERRORMESSAGE) VALUES('{1}','{2}','{3}',"
            + "TO_DATE('{4}', 'yyyy/mm/dd hh24:mi:ss'),'{5}')", "EDGIS.PGE_FAI_RELATIONSHIP_ERRORS", tableName, globalID,
            FAIWorkspace.PathName, dateString, errorMessage);
            DefaultVersionWorkspace.ExecuteSQL(sql);
        }

        /// <summary>
        /// Reports on any features that fail to process for any reason
        /// </summary>
        /// <param name="featInfo"></param>
        /// <param name="errorMessage"></param>
        private void ReportConflictingFeature(FAIFeatureInformation featInfo, string fieldName, string FAIValue, string GISValue, string errorMessage, ConflictType conflictType)
        {
            DateTime now = DateTime.Now;
            string dateString = now.Year + "/" + now.Month + "/" + now.Day + " " + now.Hour + ":" + now.Minute + ":" +
                                now.Second;

            string sql = string.Format("INSERT INTO {0} (TABLENAME, CONFLICTTYPE, CONFLICTINGGUID, FIELDNAME, FAI_VALUE, GIS_VALUE, FAILUREDATE, ERRORMESSAGE, FAIDATABASE) "
            + "VALUES('{1}','{2}','{3}','{4}','{5}','{6}', TO_DATE('{7}', 'yyyy/mm/dd hh24:mi:ss'),'{8}','{9}')", "EDGIS.PGE_FAI_CONFLICTINGFEATURES", featInfo.TableName, conflictType,
            featInfo.OriginalGlobalID, fieldName, FAIValue, GISValue, dateString, errorMessage, FAIWorkspace.PathName);
            DefaultVersionWorkspace.ExecuteSQL(sql);
        }


        #endregion

        #region Helper Methods

        /// <summary>
        /// Returns a TableInformation object for the specified ITable
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static TableInformation GetTableInformation(ITable table)
        {
            foreach (TableInformation info in TableInfo)
            {
                if (info.TableName == ((IDataset)table).BrowseName) { return info; }
            }

            //Couldn't find an existing table information object
            TableInformation newInfo = new TableInformation(table);
            TableInfo.Add(newInfo);
            return newInfo;
        }


        /// <summary>
        /// Determines all tables in the FAI FGDB database that we will need to process. This will include all tables in the
        /// FGDB database, but not all of these will necessarily have data to update
        /// </summary>
        /// <returns></returns>
        private List<string> GetTablesToProcess()
        {
            List<string> tablesToProcess = new List<string>();
            IEnumDatasetName dsNames = ((IWorkspace)FAIWorkspace).get_DatasetNames(esriDatasetType.esriDTAny);
            IDatasetName dsName = null;
            dsNames.Reset();
            while ((dsName = dsNames.Next()) != null)
            {
                if (dsName.Type == esriDatasetType.esriDTFeatureDataset)
                {
                    IFeatureDatasetName featDSName = dsName as IFeatureDatasetName;
                    IEnumDatasetName featDSNames = featDSName.FeatureClassNames;
                    IDatasetName featDS = null;
                    featDSNames.Reset();
                    while ((featDS = featDSNames.Next()) != null)
                    {
                        tablesToProcess.Add(featDS.Name);
                        while (Marshal.ReleaseComObject(featDS) > 0) { }
                    }
                    while (Marshal.ReleaseComObject(featDSNames) > 0) { }
                }
                else if (dsName.Type == esriDatasetType.esriDTFeatureClass || dsName.Type == esriDatasetType.esriDTTable)
                {
                    tablesToProcess.Add(dsName.Name);
                }
                while (Marshal.ReleaseComObject(dsName) > 0) { }
            }
            tablesToProcess.Sort();
            return tablesToProcess;
        }

        /// <summary>
        /// This will return a list of FAI feature information objects from the source FGDB database
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        private List<FAIFeatureInformation> GetFAIFeatureInformation(string tableName, TransactionType transactionType)
        {
            List<FAIFeatureInformation> featureInformationList = new List<FAIFeatureInformation>();
            ITable table = null;
            IQueryFilter qf = null;
            ICursor tableCursor = null;
            string whereClause = "";
            try
            {
                table = ((IFeatureWorkspace)FAIWorkspace).OpenTable(tableName);

                if (table.Fields.FindField(Common.OriginaGUIDFieldName) > 0)
                {
                    qf = new QueryFilterClass();

                    //Different where clauses may be used depending on whether this is an insert, update, or delete transaction
                    if (transactionType == TransactionType.Insert && table is IFeatureClass) { whereClause = Common.AddedFeaturesFCWhereClause; }
                    else if (transactionType == TransactionType.Insert) { whereClause = Common.AddedFeaturesTablesWhereClause; }
                    else if (transactionType == TransactionType.Update && table is IFeatureClass) { whereClause = Common.UpdatedFeaturesFCWhereClause; }
                    else if (transactionType == TransactionType.Update) { whereClause = Common.UpdatedFeaturesTablesWhereClause; }
                    else if (transactionType == TransactionType.Delete && table is IFeatureClass) { whereClause = Common.DeletedFeaturesFCWhereClause; }
                    else if (transactionType == TransactionType.Delete) { whereClause = Common.DeletedFeaturesTablesWhereClause; }

                    qf.WhereClause = whereClause;
                    //Determine our list of subfields to include
                    for (int i = 0; i < table.Fields.FieldCount; i++)
                    {
                        IField field = table.Fields.get_Field(i);
                        if (i == 0) { qf.SubFields = field.Name; }
                        else { qf.SubFields += "," + field.Name; }
                    }

                    //Search our FGDB table for all features matching the given where clause and add to our list
                    tableCursor = table.Search(qf, false);
                    IRow row = null;
                    while ((row = tableCursor.NextRow()) != null)
                    {
                        FAIFeatureInformation newFeatInfo = new FAIFeatureInformation(transactionType, row, true, false);
#if DEBUG
                        //if (newFeatInfo.FAIGlobalID.ToString("B").ToUpper() != "{0E19E8AD-41FC-43CE-B888-7C253E7FC2E3}"
                        //    && newFeatInfo.FAIGlobalID.ToString("B").ToUpper() != "{9621D2DA-F492-44C8-BC32-9E684759F7F0}") { continue; }
#endif
                        featureInformationList.Add(newFeatInfo);
                        while (Marshal.ReleaseComObject(row) > 0) { }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning("Unexpected issue querying table " + tableName + " with query " + whereClause + ": " + ex.Message);
#if !DEBUG
                //throw new Exception("Unexpected issue querying table " + tableName + " with query " + Common.DeletedFeaturesWhereClause);
#endif
            }

            //Release our resources
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            if (tableCursor != null) { while (Marshal.ReleaseComObject(tableCursor) > 0) { } }
            if (table != null) { while (Marshal.ReleaseComObject(table) > 0) { } }

            return featureInformationList;
        }

        /// <summary>
        /// This will return a list of FAI feature information objects from the source FGDB database
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        private List<FAIFeatureInformation> GetFAIFeatureInformation(string tableName, List<string> GUIDs)
        {
            List<FAIFeatureInformation> featureInformationList = new List<FAIFeatureInformation>();
            ITable table = null;
            IQueryFilter qf = null;
            ICursor tableCursor = null;
            string whereClause = "";
            try
            {
                table = ((IFeatureWorkspace)FAIWorkspace).OpenTable(tableName);

                if (table.Fields.FindField(Common.OriginaGUIDFieldName) > 0)
                {
                    qf = new QueryFilterClass();

                    List<string> whereInClauses = Common.GetWhereInClauses(GUIDs);

                    foreach (string whereInClause in whereInClauses)
                    {
                        whereClause = "GLOBALID in (" + whereInClause + ")";
                        qf.WhereClause = whereClause;
                        //Determine our list of subfields to include
                        for (int i = 0; i < table.Fields.FieldCount; i++)
                        {
                            IField field = table.Fields.get_Field(i);
                            if (i == 0) { qf.SubFields = field.Name; }
                            else { qf.SubFields += "," + field.Name; }
                        }

                        //Search our FGDB table for all features matching the given where clause and add to our list
                        tableCursor = table.Search(qf, false);
                        IRow row = null;
                        while ((row = tableCursor.NextRow()) != null)
                        {
                            FAIFeatureInformation newFeatInfo = new FAIFeatureInformation(TransactionType.None, row, false, true);
                            featureInformationList.Add(newFeatInfo);
                            while (Marshal.ReleaseComObject(row) > 0) { }
                        }
                        while (Marshal.ReleaseComObject(tableCursor) > 0) { }
                    }
                    while (Marshal.ReleaseComObject(qf) > 0) { }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning("Unexpected issue querying FAI table " + tableName + " with query " + whereClause + ": " + ex.Message);
#if !DEBUG
                //throw new Exception("Unexpected issue querying table " + tableName + " with query " + Common.DeletedFeaturesWhereClause);
#endif
            }

            //Release our resources
            if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
            if (tableCursor != null) { while (Marshal.ReleaseComObject(tableCursor) > 0) { } }
            if (table != null) { while (Marshal.ReleaseComObject(table) > 0) { } }

            return featureInformationList;
        }

        /// <summary>
        /// Gets a list of where in clauses for all of the orignal globalIDs in an FAI feature information list
        /// </summary>
        /// <param name="FAIInfo"></param>
        /// <returns></returns>
        private List<string> GetWhereInClausesForFAIFeatInfo(List<FAIFeatureInformation> FAIInfo)
        {
            List<string> originalGuids = new List<string>();
            foreach (FAIFeatureInformation featInfo in FAIInfo) { originalGuids.Add(featInfo.OriginalGlobalID.ToString("B").ToUpper()); }
            return Common.GetWhereInClauses(originalGuids);
        }

        /// <summary>
        /// Returns the associated FAI Feature INformation for the provided global ID
        /// </summary>
        /// <param name="featInfo"></param>
        /// <param name="searchGuid"></param>
        /// <returns></returns>
        private FAIFeatureInformation GetAssociatedFeatureInfo(List<FAIFeatureInformation> featInfo, Guid searchGuid)
        {
            foreach (FAIFeatureInformation featInformation in featInfo)
            {
                if (featInformation.OriginalGlobalID == searchGuid) { return featInformation; }
            }
            return null;
        }

        /// <summary>
        /// Determines if the current row value equals the new value to update to
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="fieldType"></param>
        /// <param name="isNewFeature"></param>
        /// <returns></returns>
        private bool RowValueEquals(object oldValue, object newValue, esriFieldType fieldType, bool isNewFeature)
        {
            //If it's a new feature we'll just return false for equivalency
            if (isNewFeature) { return false; }

            bool valuesEquivalent = false;
            try
            {
                if (oldValue == DBNull.Value && newValue != DBNull.Value) { valuesEquivalent = false; }
                else if (oldValue != DBNull.Value && newValue == DBNull.Value) { valuesEquivalent = false; }
                else if (oldValue == null && newValue != null) { valuesEquivalent = false; }
                else if (oldValue != null && newValue == null) { valuesEquivalent = false; }
                else if (oldValue == DBNull.Value && newValue == DBNull.Value) { valuesEquivalent = true; }
                else if (oldValue == null && newValue == null) { valuesEquivalent = true; }
                else if (fieldType == esriFieldType.esriFieldTypeString) { valuesEquivalent = oldValue.ToString() == newValue.ToString(); }
                else if (fieldType == esriFieldType.esriFieldTypeDate) { valuesEquivalent = oldValue.ToString() == newValue.ToString(); }
                else if (fieldType == esriFieldType.esriFieldTypeDouble) { valuesEquivalent = (double)oldValue == (double)newValue; }
                else if (fieldType == esriFieldType.esriFieldTypeGeometry)
                {
                    if (oldValue is IPoint)
                    {
                        IPoint oldPoint = oldValue as IPoint;
                        IPoint newPoint = newValue as IPoint;
                        if (oldPoint.X == newPoint.X && oldPoint.Y == oldPoint.Y) { valuesEquivalent = true; }
                    }
                    else if (oldValue is IPointCollection)
                    {
                        IPointCollection oldPointCollection = ((IPointCollection)oldValue);
                        IPointCollection newPointCollection = ((IPointCollection)newValue);

                        if (oldPointCollection.PointCount == newPointCollection.PointCount)
                        {
                            bool pointCollectionEqual = true;
                            for (int i = 0; i < oldPointCollection.PointCount; i++)
                            {
                                IPoint oldPoint = oldPointCollection.get_Point(i);
                                IPoint newPoint = newPointCollection.get_Point(i);
                                if (oldPoint.X != newPoint.X || oldPoint.Y != newPoint.Y) { pointCollectionEqual = false; }
                            }
                            valuesEquivalent = pointCollectionEqual;
                        }
                    }
                    else
                    {

                    }
                }
                else if (fieldType == esriFieldType.esriFieldTypeGUID) { valuesEquivalent = oldValue.ToString().ToUpper() == newValue.ToString().ToUpper(); }
                else if (fieldType == esriFieldType.esriFieldTypeInteger) { valuesEquivalent = (int)oldValue == (int)newValue; }
                else if (fieldType == esriFieldType.esriFieldTypeSingle) { valuesEquivalent = (Int16)oldValue == (int)newValue; }
                else if (fieldType == esriFieldType.esriFieldTypeSmallInteger) { valuesEquivalent = (short)oldValue == (short)newValue; }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (oldValue == DBNull.Value && newValue != DBNull.Value) { }
                else if (oldValue != DBNull.Value && newValue == DBNull.Value) { }
                else if (oldValue == DBNull.Value && newValue == DBNull.Value) { }
                else if (oldValue == null && newValue == null) { }
                else if (fieldType == esriFieldType.esriFieldTypeGeometry) { }
                else if (fieldType == esriFieldType.esriFieldTypeDouble) { }
                else if (!valuesEquivalent && oldValue.ToString() == newValue.ToString())
                {

                }
                else if (valuesEquivalent && oldValue.ToString() != newValue.ToString())
                {

                }
            }

            return valuesEquivalent;
        }

        /// <summary>
        /// This method will determine if a feature needs to be disconnected from any features 
        /// </summary>
        /// <param name="FAIFeature"></param>
        /// <param name="newRow"></param>
        private void DisconnectIfNeeded(FAIFeatureInformation FAIFeature, IRow newRow)
        {
            if (newRow is IFeature && ((IFeature)newRow).Class is INetworkClass)
            {
                System.Type typeAU = Type.GetTypeFromProgID("mmGeoDatabase.MMAutoUpdater");
                IMMAutoUpdater mmAutoUpdater = (IMMAutoUpdater)System.Activator.CreateInstance(typeAU);

                IFeatureClass FAIFeatClass = ((IFeatureWorkspace)FAIWorkspace).OpenFeatureClass(FAIFeature.TableName);
                IFeature FAIFeatureObject = FAIFeatClass.GetFeature(FAIFeature.FAIObjectID);
                List<FAIFeatureInformation> connectedFAIFeatures = new List<FAIFeatureInformation>();
                if (FAIFeatureObject is ISimpleJunctionFeature)
                {
                    //We'll always disconnect junction features
                    ISimpleJunctionFeature GISSimpleJunction = newRow as ISimpleJunctionFeature;
                    for (int i = 0; i < GISSimpleJunction.EdgeFeatureCount; i++)
                    {
                        IFeature GISEdgeFeature = GISSimpleJunction.get_EdgeFeature(i) as IFeature;

                        ((IEdgeFeature)GISEdgeFeature).DisconnectAtJunction(((ISimpleEdgeFeature)GISEdgeFeature).EID, GISSimpleJunction.EID);
                        mmAutoUpdater.FeatureDisconnected(GISEdgeFeature as INetworkFeature);
                        mmAutoUpdater.FeatureDisconnected(GISSimpleJunction as INetworkFeature);
                        while (Marshal.ReleaseComObject(GISEdgeFeature) > 0) { }
                    }
                }
                else if (FAIFeatureObject is ISimpleEdgeFeature)
                {
                    IEdgeFeature gisEdgeFeature = newRow as IEdgeFeature;
                    IPoint GISFromJunctionPoint = ((IFeature)gisEdgeFeature.FromJunctionFeature).Shape as IPoint;
                    IPoint GISToJunctionPoint = ((IFeature)gisEdgeFeature.ToJunctionFeature).Shape as IPoint;

                    IPointCollection FAIPointCollection = ((IPointCollection)FAIFeatureObject.Shape);

                    IPoint faiStartPoint = new PointClass();
                    IPoint faiEndPoint = new PointClass();
                    FAIPointCollection.QueryPoint(0, faiStartPoint);
                    FAIPointCollection.QueryPoint(FAIPointCollection.PointCount - 1, faiEndPoint);

                    //Check the GIS From Junction
                    if ((faiStartPoint.X == GISFromJunctionPoint.X && faiStartPoint.Y == GISFromJunctionPoint.Y) ||
                        (faiEndPoint.X == GISFromJunctionPoint.X && faiEndPoint.Y == GISFromJunctionPoint.Y)) { /*Do Nothing*/ }
                    else
                    {
                        //The FAI start point doesn't match the original to or from junctions in GIS so let's disconnect
                        ((IEdgeFeature)newRow).DisconnectAtJunction(((ISimpleEdgeFeature)gisEdgeFeature).EID, ((ISimpleJunctionFeature)gisEdgeFeature.FromJunctionFeature).EID);
                        mmAutoUpdater.FeatureDisconnected(gisEdgeFeature as INetworkFeature);
                        mmAutoUpdater.FeatureDisconnected(gisEdgeFeature.FromJunctionFeature as INetworkFeature);
                    }

                    //Check the GIS To Junction
                    if ((faiStartPoint.X == GISToJunctionPoint.X && faiStartPoint.Y == GISToJunctionPoint.Y) ||
                        (faiEndPoint.X == GISToJunctionPoint.X && faiEndPoint.Y == GISToJunctionPoint.Y)) { /*Do Nothing*/ }
                    else
                    {
                        //The FAI start point doesn't match the original to or from junctions in GIS so let's disconnect
                        ((IEdgeFeature)newRow).DisconnectAtJunction(((ISimpleEdgeFeature)gisEdgeFeature).EID, ((ISimpleJunctionFeature)gisEdgeFeature.ToJunctionFeature).EID);
                        mmAutoUpdater.FeatureDisconnected(gisEdgeFeature as INetworkFeature);
                        mmAutoUpdater.FeatureDisconnected(gisEdgeFeature.ToJunctionFeature as INetworkFeature);
                    }

                }
                if (FAIFeatureObject != null) { while (Marshal.ReleaseComObject(FAIFeatureObject) > 0) { } }
                if (FAIFeatClass != null) { while (Marshal.ReleaseComObject(FAIFeatClass) > 0) { } }
            }
        }

        /// <summary>
        /// Copies attributes from the FAI feature to the new row feature
        /// </summary>
        /// <param name="FAIFeature"></param>
        /// <param name="newRow"></param>
        /// <param name="isNew"></param>
        private void CopyAttributes(FAIFeatureInformation FAIFeature, IRow newRow, bool isNew)
        {
            TableInformation GISTableInfo = GetTableInformation(newRow.Table);

            //Update the subtype field first so that our domain checks are valid
            ISubtypes subtypes = newRow.Table as ISubtypes;
            if (subtypes.HasSubtype)
            {
                string subtypeFieldName = subtypes.SubtypeFieldName;
                int subtypeIdx = GISTableInfo.GetFieldIndex(subtypeFieldName);
                IField subtypeField = newRow.Fields.get_Field(subtypeIdx);
                object newValue = FAIFeature.FieldValues[subtypeFieldName.ToUpper()];
                object oldValue = newRow.get_Value(subtypeIdx);

                if (!RowValueEquals(oldValue, newValue, subtypeField.Type, isNew)) { newRow.set_Value(subtypeIdx, FAIFeature.FieldValues[subtypeFieldName.ToUpper()]); }
            }

            //Iterate over all of the FAIFeature field values and update our new GIS feature to match
            foreach (KeyValuePair<string, object> kvp in FAIFeature.FieldValues)
            {
                int fieldIndex = GISTableInfo.GetFieldIndex(kvp.Key);
                if (kvp.Key.ToUpper() != "OBJECTID" && kvp.Key.ToUpper() != "GLOBALID" && fieldIndex > -1)
                {
                    try
                    {
                        //This field exists in the GIS table. Let's set the value
                        IField field = newRow.Fields.get_Field(fieldIndex);
                        object fieldValue = kvp.Value;
                        object oldValue = newRow.get_Value(fieldIndex);

                        //Only process a row if it actually differs from existing value
                        if (RowValueEquals(oldValue, fieldValue, field.Type, isNew)) { continue; }

                        if (fieldValue != null && fieldValue != DBNull.Value)
                        {
                            string domainName = "";
                            //If an FAI field value has an invalid precision for the what is defined in GIS then report a conflict to our conflicting features
                            //table and continue on.  These will need to be reviewed by the mapper.
                            if ((field.Type == esriFieldType.esriFieldTypeSingle || field.Type == esriFieldType.esriFieldTypeSmallInteger
                                || field.Type == esriFieldType.esriFieldTypeInteger) && fieldValue.ToString().Length > field.Precision)
                            {
                                string errorMessage = "Specified precision exceeded for field " + field.Name + ". Value: " + fieldValue.ToString() + " Precision: " + field.Precision;
                                ReportConflictingFeature(FAIFeature, field.Name, fieldValue.ToString(), "", errorMessage, ConflictType.InvalidFieldData);
                                //throw new Exception("Specified precision exceeded for field " + field.Name + ". Value: " + fieldValue.ToString() + " Precision: " + field.Precision);
                            }
                            else if (!IsValidDomainValue(newRow, field, fieldValue, ref domainName))
                            {
                                //This value is invalid for the domain that is assigned
                                string errorMessage = "Specified value for field " + field.Name + ". Value: " + fieldValue.ToString() + " is invalid for assigned domain " + domainName;
                                ReportConflictingFeature(FAIFeature, field.Name, fieldValue.ToString(), "", errorMessage, ConflictType.InvalidDomainValue);
                            }
                            else
                            {
                                if (field.Type == esriFieldType.esriFieldTypeGeometry && !isNew)
                                {
                                    //Since this is a geometry field we are updating we want to ensure that we don't have to disconnect from
                                    //any other network features first before updating the shape.  If we don't disconnect from features that
                                    //aren't supposed to be connected then they will move along with this shape update
                                    DisconnectIfNeeded(FAIFeature, newRow);
                                }
                                newRow.set_Value(fieldIndex, fieldValue);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Check to verify that the specified value is valid for any domains assigned to this field
        /// </summary>
        /// <param name="row"></param>
        /// <param name="field"></param>
        /// <param name="valueToCheck"></param>
        /// <returns></returns>
        private bool IsValidDomainValue(IRow row, IField field, object valueToCheck, ref string domainName)
        {
            //We will assume that null or empty values are valid domain values
            if (valueToCheck == null || valueToCheck == DBNull.Value || string.IsNullOrEmpty(valueToCheck.ToString())) { return true; }

            //Determine our mapped field value
            IRowSubtypes rowSubtypes = row as IRowSubtypes;
            ISubtypes subtypes = row.Table as ISubtypes;
            IDomain fieldDomain = subtypes.get_Domain(rowSubtypes.SubtypeCode, field.Name);
            if (fieldDomain != null)
            {
                domainName = fieldDomain.Name;
                if (fieldDomain is ICodedValueDomain)
                {
                    ICodedValueDomain codedValueDomain = fieldDomain as ICodedValueDomain;
                    for (int i = 0; i < codedValueDomain.CodeCount; i++)
                    {
                        string name = codedValueDomain.get_Name(i);
                        object value = codedValueDomain.get_Value(i);
                        if (field.Type == esriFieldType.esriFieldTypeString)
                        {
                            if (value.ToString() == valueToCheck.ToString()) { return true; }
                        }
                        else
                        {
                            if (value.Equals(valueToCheck)) { return true; }
                        }
                    }
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Returns the original GUID value for the FAI feature passed in
        /// </summary>
        /// <param name="faiFeature"></param>
        /// <returns></returns>
        private string GetOriginalGUID(FAIFeatureInformation faiFeature)
        {
            string FAIGuid = faiFeature.FAIGlobalID.ToString("B").ToUpper();
            foreach (GUIDMap map in FAIGuidToOriginalGUID)
            {
                if (map.FAIGuid == FAIGuid) { return map.GISGuid; }
            }
            return faiFeature.OriginalGlobalID.ToString("B").ToUpper();
        }

        /// <summary>
        /// Returns the field index for the specified field on the specified table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        private int FindFieldIndex(ITable table, string fieldName)
        {
            try
            {
                TableInformation featClassInfo = GetTableInformation(table);
                return featClassInfo.GetFieldIndex(fieldName);
            }
            finally
            {
            }
        }


        /// <summary>
        /// Updates progress within the console application.  If anything fails, it just catches and moves on.  This is for the case that UC4 fails updating the console
        /// window which has been seen in the past
        /// </summary>
        /// <param name="message"></param>
        /// <param name="currElementIndex"></param>
        /// <param name="totalElementCount"></param>
        private void ShowPercentProgress(string message, double totalProcessed, double totalToProcess)
        {
            try
            {
                if (totalProcessed < 0 || totalProcessed > totalToProcess)
                {
                    throw new InvalidOperationException("currElement out of range");
                }
                double percent = Math.Round((100.0 * ((double)totalProcessed)) / (double)totalToProcess, 2);
                Console.Write("\r{0}: {1}% complete    ", message, percent);
            }
            catch (Exception ex)
            {

            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        #endregion

        #region Structs

        /// <summary>
        /// Structure to hold the results of successfes and failures for a session
        /// </summary>
        private struct ResultCounts
        {
            public int InsertsProcessed;
            public int InsertsFailed;
            public int UpdatesProcessed;
            public int UpdatesFailed;
            public int DeletesProcessed;
            public int DeletesFailed;
        }

        /// <summary>
        /// Structure to hold the map between an FAI global ID and a GIS global ID
        /// </summary>
        private struct GUIDMap
        {
            public string FAIGuid;
            public string GISGuid;
            public string TableName;
        }

        #endregion

        #region Enumerations

        /// <summary>
        /// The type of transaction being executed
        /// </summary>
        public enum TransactionType
        {
            Insert,
            Update,
            Delete,
            Replace,
            None
        }

        /// <summary>
        /// Enumeration defining the type of conflict encountered for a feature
        /// </summary>
        private enum ConflictType
        {
            InvalidFieldData,
            InvalidDomainValue,
            CoincidentGeometry,
            DuplicateOperatingNumber,
            DuplicateBarcode,
            Unknown
        }

        #endregion
    }

    /// <summary>
    /// Holds information pertinent to a given FAI feature insert, update, or delete
    /// </summary>
    public class FAIFeatureInformation
    {
        public FAIFeatureInformation(FAIProcessor.TransactionType transactionType, IRow row, bool cacheAllFields, bool cacheRelatedFeatures)
        {
            int GUID = FAIProcessor.GetTableInformation(row.Table).GetFieldIndex("GLOBALID");
            int origGUID = FAIProcessor.GetTableInformation(row.Table).GetFieldIndex(Common.OriginaGUIDFieldName);

            //Get the feature information
            Guid faiGUID = new Guid();
            if (GUID > -1) { faiGUID = new Guid(row.get_Value(GUID).ToString()); }

            string originalGuidValue = "";
            if (origGUID > 0) { originalGuidValue = row.get_Value(origGUID).ToString(); }
            if (!string.IsNullOrEmpty(originalGuidValue))
            {
                Guid origGuid = new Guid(row.get_Value(origGUID).ToString());
                OriginalGlobalID = origGuid;
            }
            //Set the FAI feature information
            
            FAIGlobalID = faiGUID;
            FAIObjectID = row.OID;

            FieldValues = new Dictionary<string, object>();
            if (cacheAllFields)
            {
                for (int i = 0; i < row.Fields.FieldCount; i++)
                {
                    try
                    {
                        IField field = row.Fields.get_Field(i);
                        FieldValues.Add(field.Name.ToUpper(), row.get_Value(i));
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            TableName = ((IDataset)row.Table).BrowseName;
            Type = transactionType;

            if (cacheRelatedFeatures)
            {
                RelatedFeatures = new List<FAIFeatureInformation>();
                //Now let's get all of our related features
                TableInformation FAITableInfo = FAIProcessor.GetTableInformation(row.Table);
                //Process destination classes
                foreach (IRelationshipClass relClass in FAITableInfo.DestinationRelationshipClasses)
                {
                    //Don't process annotation features
                    if (relClass.OriginClass is IFeatureClass &&
                        ((IFeatureClass)relClass.OriginClass).FeatureType == esriFeatureType.esriFTAnnotation) { continue; }

                    ISet relatedObjects = relClass.GetObjectsRelatedToObject(row as IObject);
                    relatedObjects.Reset();
                    IRow relatedObject = null;
                    while ((relatedObject = relatedObjects.Next() as IRow) != null)
                    {
                        RelatedFeatures.Add(new FAIFeatureInformation(transactionType, relatedObject, cacheAllFields, false));
                    }
                }
                //Process origin classes
                foreach (IRelationshipClass relClass in FAITableInfo.OriginRelationshipClasses)
                {
                    //Don't process annotation features
                    if (relClass.DestinationClass is IFeatureClass &&
                        ((IFeatureClass)relClass.DestinationClass).FeatureType == esriFeatureType.esriFTAnnotation) { continue; }

                    ISet relatedObjects = relClass.GetObjectsRelatedToObject(row as IObject);
                    relatedObjects.Reset();
                    IRow relatedObject = null;
                    while ((relatedObject = relatedObjects.Next() as IRow) != null)
                    {
                        RelatedFeatures.Add(new FAIFeatureInformation(transactionType, relatedObject, cacheAllFields, false));
                    }
                }
            }
        }


        public int FAIObjectID { get; set; }
        public int GISObjectID { get; set; }

        public Guid FAIGlobalID { get; set; }
        public Guid OriginalGlobalID { get; set; }

        public string TableName { get; set; }
        public string GISTableName
        {
            get
            {
                return "EDGIS." + TableName;
            }
        }

        public Dictionary<string, object> FieldValues { get; set; }

        public List<FAIFeatureInformation> RelatedFeatures { get; set; }

        public PGE.BatchApplication.FAISessionProcessor.FAIProcessor.TransactionType Type { get; set; }
    }

    /// <summary>
    /// Holds information for a particular ITable object.  This ensure quick access to already derived information
    /// </summary>
    public class TableInformation : IComparable
    {
        public TableInformation(ITable featureClass)
        {
            TableName = ((IDataset)featureClass).BrowseName;
            FieldIndices = new Dictionary<string, int>();

            for (int i = 0; i < featureClass.Fields.FieldCount; i++)
            {
                IField field = featureClass.Fields.get_Field(i);
                FieldIndices.Add(field.Name.ToUpper(), i);
            }

            DestinationRelationshipTableNames = new List<string>();
            OriginRelationshipTableNames = new List<string>();
            DestinationRelationshipClasses = new List<IRelationshipClass>();
            OriginRelationshipClasses = new List<IRelationshipClass>();

            IEnumRelationshipClass destinationRelClasses = ((IObjectClass)featureClass).get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            IRelationshipClass relClass = null;
            while ((relClass = destinationRelClasses.Next()) != null)
            {
                DestinationRelationshipClasses.Add(relClass);
                DestinationRelationshipTableNames.Add(((IDataset)relClass.OriginClass).BrowseName);
            }

            IEnumRelationshipClass OriginRelClasses = ((IObjectClass)featureClass).get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
            relClass = null;
            while ((relClass = OriginRelClasses.Next()) != null)
            {
                OriginRelationshipClasses.Add(relClass);
                OriginRelationshipTableNames.Add(((IDataset)relClass.DestinationClass).BrowseName);
            }
        }

        public List<string> DestinationRelationshipTableNames { get; set; }
        public List<string> OriginRelationshipTableNames { get; set; }

        public List<IRelationshipClass> DestinationRelationshipClasses { get; set; }
        public List<IRelationshipClass> OriginRelationshipClasses { get; set; }

        public string TableName { get; set; }

        private Dictionary<string, int> FieldIndices { get; set; }

        public int GetFieldIndex(string fieldName)
        {
            if (FieldIndices.ContainsKey(fieldName.ToUpper()))
            {
                return FieldIndices[fieldName.ToUpper()];
            }
            else { return -1; }
        }

        public override bool Equals(object obj)
        {
            if (obj is IDataset && ((IDataset)obj).BrowseName == TableName)
            {
                return true;
            }
            else if (obj is TableInformation && ((TableInformation)obj).TableName == TableName)
            {
                return true;
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (obj is IDataset)
            {
                return ((IDataset)obj).BrowseName.CompareTo(TableName);
            }
            else if (obj is TableInformation)
            {
                return ((TableInformation)obj).TableName.CompareTo(TableName);
            }
            return 0;
        }
    }

}
