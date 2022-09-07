using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PGE.Common.Delivery.Geodatabase.ChangeDetection;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using PGE.Common.Delivery.Framework.FeederManager;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.NetworkAnalysis;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.esriSystem;

namespace PGE.BatchApplication.UDC_FAI_Tools
{
    public partial class CGC12UpdaterForm : Form
    {
        public CGC12UpdaterForm()
        {
            InitializeComponent();
        }

        private void UpdateProgress(string message, int currentPosition, int maxValue, bool reportInMessage)
        {
            double progress = (((double)currentPosition) / ((double)maxValue)) * 100.0;

            if (reportInMessage) { lblProgress.Text = message + ": " + currentPosition + " of " + maxValue; }
            else { lblProgress.Text = message; }
            prgProgressBar.Value = Int32.Parse(Math.Floor(progress).ToString());
            Application.DoEvents();
        }

        /// <summary>
        /// Process a differences list to determine what OIDs have been updated in each table
        /// </summary>
        /// <param name="sProgressor"></param>
        /// <param name="differences"></param>
        /// <param name="editedOIDsByTableName"></param>
        /// <param name="message"></param>
        private void ProcessChanges(Dictionary<string, DiffTable>.Enumerator differences,
            ref Dictionary<string, List<int>> editedOIDsByTableName, string message)
        {
            while (differences.MoveNext())
            {
                int totalProcessed = 0;
                if (!editedOIDsByTableName.ContainsKey(differences.Current.Key)) { editedOIDsByTableName.Add(differences.Current.Key, new List<int>()); }
                foreach (DiffRow diffRow in differences.Current.Value)
                {
                    editedOIDsByTableName[differences.Current.Key].Add(diffRow.OID);

                    totalProcessed++;
                    UpdateProgress(message + differences.Current.Key, totalProcessed, differences.Current.Value.Count, false);
                }
            }
        }

        /// <summary>
        /// Determines all edited features in this session
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, List<int>> GetEditedFeatures()
        {
            Dictionary<string, List<int>> editedOIDsByTableName = new Dictionary<string, List<int>>();
            
            try
            {
                lblProgress.Text = "Determining Version Differences";
                IVersionedWorkspace versionWS = Miner.Geodatabase.Edit.Editor.EditWorkspace as IVersionedWorkspace;

                lblProgress.Text = "Initializing version differences";
                IVersion editVersion = Miner.Geodatabase.Edit.Editor.EditWorkspace as IVersion;
                DifferenceManager differenceManager = new DifferenceManager(versionWS.DefaultVersion, editVersion);
                differenceManager.Initialize();

                lblProgress.Text = "Loading differences";
                esriDataChangeType[] differeces = { esriDataChangeType.esriDataChangeTypeInsert, esriDataChangeType.esriDataChangeTypeUpdate };
                differenceManager.LoadDifferences(false, differeces);

                lblProgress.Text = "Determining inserts";
                DifferenceDictionary insertDictionary = differenceManager.Inserts;
                Dictionary<string, DiffTable>.Enumerator inserts = insertDictionary.GetEnumerator();

                //Process the inserted features
                ProcessChanges(inserts, ref editedOIDsByTableName, "Processing inserts for ");

                DifferenceDictionary updatedDictionary = differenceManager.Updates;
                Dictionary<string, DiffTable>.Enumerator updates = updatedDictionary.GetEnumerator();

                //Process the inserted features
                ProcessChanges(updates, ref editedOIDsByTableName, "Processing updates for ");

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get version differences: " + ex.Message);
            }

            return editedOIDsByTableName;
        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private static List<string> GetWhereInClauses(List<int> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < guids.Count; i++)
                {
                    if ((((i % 999) == 0) && i != 0) || (guids.Count == i + 1))
                    {
                        builder.Append(guids[i]);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else { builder.Append(guids[i] + ","); }
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Determines the list of updated feeders in this session
        /// </summary>
        /// <param name="editedOIDsByTable"></param>
        /// <returns></returns>
        private List<string> UpdatedFeeders(Dictionary<string, List<int>> editedOIDsByTable)
        {
            List<string> feederIDsList = new List<string>();
            int totalToProcess = 0;
            foreach (KeyValuePair<string, List<int>> kvp in editedOIDsByTable)
            {
                IFeatureWorkspace featWorkspace = Miner.Geodatabase.Edit.Editor.EditWorkspace as IFeatureWorkspace;
                ITable table = featWorkspace.OpenTable(kvp.Key);
                if (!(table is IFeatureClass)) { continue; }
                if (!(table is INetworkClass)) { continue; }
                if (table.FindField("CIRCUITID") < 0) { continue; }

                totalToProcess += kvp.Value.Count;
            }

            int processed = 0;
            foreach (KeyValuePair<string, List<int>> kvp in editedOIDsByTable)
            {
                try
                {
                    IFeatureWorkspace featWorkspace = Miner.Geodatabase.Edit.Editor.EditWorkspace as IFeatureWorkspace;
                    ITable table = featWorkspace.OpenTable(kvp.Key);
                    if (!(table is IFeatureClass)) { continue; }
                    if (!(table is INetworkClass)) { continue; }
                    if (table.FindField("CIRCUITID") < 0) { continue; }

                    List<string> whereInClauses = GetWhereInClauses(kvp.Value);
                    foreach (string whereInClause in whereInClauses)
                    {
                        IQueryFilter qf = new QueryFilterClass();
                        qf.WhereClause = "OBJECTID IN (" + whereInClause + ")";
                        qf.SubFields = "OBJECTID";

                        //Process through these features and add their feeder ID to our list
                        ICursor rowCursor = table.Search(qf, false);
                        IRow row = null;
                        while ((row = rowCursor.NextRow()) != null)
                        {
                            try
                            {
                                string[] feederIDs = FeederManager2.GetCircuitIDs(row);
                                if (feederIDs != null)
                                {
                                    foreach (string feederID in feederIDs)
                                    {
                                        if (!string.IsNullOrEmpty(feederID) && !feederIDsList.Contains(feederID)) { feederIDsList.Add(feederID); }
                                    }
                                }
                                processed++;
                                UpdateProgress("Processing " + kvp.Key, processed, totalToProcess, false);
                                while (Marshal.ReleaseComObject(row) > 0) { }
                            }
                            catch (Exception ex)
                            {
                                throw ex;
                            }
                        }
                        while (Marshal.ReleaseComObject(qf) > 0) { }
                        while (Marshal.ReleaseComObject(rowCursor) > 0) { }
                    }

                    //while (Marshal.ReleaseComObject(table) > 0) { }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return feederIDsList;
        }

        /// <summary>
        /// Determines the list of service location OIDs that lie along the requested circuit.
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="circuitID"></param>
        /// <returns></returns>
        private List<int> GetServiceLocationOIDs(IGeometricNetwork geomNetwork, string circuitID)
        {
            List<int> ServiceLocationOIDs = new List<int>();
            string feederID = "";
            string message = string.Empty;
            IMMFeederExt feederExt = null;
            IMMFeederSpace feederSpace = null;
            IMMFeederSource feederSource = null;
            IMMElectricNetworkTracing downstreamTracer = null;
            IMMNetworkAnalysisExtForFramework networkAnalysisExtForFramework = null;
            IMMElectricTraceSettingsEx electricTraceSettings = null;

            Type type = Type.GetTypeFromProgID("mmFramework.MMFeederExt");
            object obj = Activator.CreateInstance(type);

            try
            {
                //Console.WriteLine("Initializing feeder space");
                //this.Log.Info(ServiceConfiguration.Name, "Initializing feeder space");

                feederExt = obj as IMMFeederExt;
                feederSpace = feederExt.get_FeederSpace(geomNetwork, false);
                if (feederSpace == null)
                {
                    throw new Exception("Unable to get feeder space information from geometric network");
                }

                //Console.WriteLine("Finsihed initializing feeder space");
                //this.Log.Info(ServiceConfiguration.Name, "Finished initializing feeder space");

                INetwork network = geomNetwork.Network;
                INetSchema networkSchema = network as INetSchema;
                INetWeight electricTraceWeight = networkSchema.get_WeightByName("MMELECTRICTRACEWEIGHT");
                downstreamTracer = new MMFeederTracerClass();

                electricTraceSettings = new MMElectricTraceSettingsClass();
                electricTraceSettings.RespectESRIBarriers = true;
                electricTraceSettings.UseFeederManagerCircuitSources = true;
                electricTraceSettings.UseFeederManagerProtectiveDevices = true;

                networkAnalysisExtForFramework = new MMNetworkAnalysisExtForFrameworkClass();
                networkAnalysisExtForFramework.DrawComplex = true;
                networkAnalysisExtForFramework.SelectionSemantics = mmESRIAnalysisOptions.mmESRIAnalysisOnAllFeatures;

                //Phases to trace.
                mmPhasesToTrace phasesToTrace = mmPhasesToTrace.mmPTT_Any;
                feederSource = feederSpace.FeederSources.get_FeederSourceByFeederID(circuitID);
                if (feederSource != null)
                {
                    feederID = feederSource.FeederID.ToString();

                    DateTime startTime = DateTime.Now;
                    IEnumNetEID junctions = null;
                    IEnumNetEID edges = null;

                    //Trace
                    downstreamTracer.TraceDownstream(geomNetwork, networkAnalysisExtForFramework, electricTraceSettings, feederSource.JunctionEID,
                                                        esriElementType.esriETJunction, phasesToTrace, out junctions, out edges);


                    List<int> edgesList = new List<int>();
                    List<int> junctionsList = new List<int>();
                    junctions.Reset();
                    edges.Reset();
                    int junction = -1;
                    while ((junction = junctions.Next()) > 0)
                    {
                        junctionsList.Add(junction);
                    }

                    IEnumNetEIDBuilder junctionsBuilder = new EnumNetEIDArrayClass();
                    junctionsBuilder.ElementType = esriElementType.esriETJunction;
                    junctionsBuilder.Network = geomNetwork.Network;

                    foreach (int SSDEid in junctionsList)
                    {
                        junctionsBuilder.Add(SSDEid);
                    }

                    lblProgress.Text = "Tracing Circuit " + circuitID + ": Obtaining junctions found in trace";
                    Application.DoEvents();

                    IEIDHelper eidHelper = new EIDHelperClass();
                    eidHelper.GeometricNetwork = geomNetwork;
                    eidHelper.ReturnGeometries = false;
                    eidHelper.ReturnFeatures = true;
                    eidHelper.AddField("OBJECTID");
                    IEnumEIDInfo junctionEidInfo = eidHelper.CreateEnumEIDInfo((IEnumNetEID)junctionsBuilder);

                    int junctionsProcessed = 0;
                    junctionEidInfo.Reset();
                    IEIDInfo junctionInfo = null;
                    while ((junctionInfo = junctionEidInfo.Next()) != null)
                    {
                        if (junctionInfo.Feature == null) { continue; }
                        if (((IDataset)junctionInfo.Feature.Table).BrowseName.ToUpper() == "EDGIS.SERVICELOCATION")
                        {
                            ServiceLocationOIDs.Add(junctionInfo.Feature.OID);
                        }

                        //Update user on some progress as this is where most of the time in this method is spent
                        junctionsProcessed++;
                        lblProgress.Text = "Tracing Circuit " + circuitID + ": Processing junction " + junctionsProcessed + " out of " + junctionsList.Count;
                        Application.DoEvents();

                        //while (Marshal.ReleaseComObject(junctionInfo.Feature) > 0) { }
                    }
                    if (junctionInfo != null) { while (Marshal.ReleaseComObject(junctionInfo) > 0) { } }
                    if (junctionEidInfo != null) { while (Marshal.ReleaseComObject(junctionEidInfo) > 0) { } }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (downstreamTracer != null) { while (Marshal.ReleaseComObject(downstreamTracer) > 0);}
                if (networkAnalysisExtForFramework != null) { while (Marshal.ReleaseComObject(networkAnalysisExtForFramework) > 0);}
                if (electricTraceSettings != null) { while (Marshal.ReleaseComObject(electricTraceSettings) > 0);}
                GC.Collect();
            }

            return ServiceLocationOIDs;
        }

        /// <summary>
        /// Processes through the provided list of service location OIDs to update relationships and CGC12 numbers of upstream transformers
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="serviceLocationOIDs"></param>
        private void ProcessServiceLocations(IGeometricNetwork geomNetwork, List<int> serviceLocationOIDs)
        {
            int processed = 0;
            IFeatureWorkspace featWorkspace = Miner.Geodatabase.Edit.Editor.EditWorkspace as IFeatureWorkspace;
            IFeatureClass serviceLocationFeatClass = featWorkspace.OpenFeatureClass("EDGIS.SERVICELOCATION");

            //Get our list of where in clauses
            List<string> whereInClauses = GetWhereInClauses(serviceLocationOIDs);
            foreach (string whereInClause in whereInClauses)
            {
                IQueryFilter qf = new QueryFilterClass();
                qf.WhereClause = "OBJECTID IN (" + whereInClause + ")";
                qf.SubFields = "GLOBALID,ELECTRICTRACEWEIGHT";

                //Search and process through all service locations
                IFeatureCursor featCursor = serviceLocationFeatClass.Search(qf, false);
                IFeature serviceLocation = null;
                while ((serviceLocation = featCursor.NextFeature()) != null)
                {
                    UpdateProgress("Processing Service Locations: ", processed, serviceLocationOIDs.Count, true);
                    ProcessServiceLocation(geomNetwork, serviceLocation);

                    processed++;
                    UpdateProgress("Processing Service Locations: ", processed, serviceLocationOIDs.Count, true);
                    while (Marshal.ReleaseComObject(serviceLocation) > 0) { }
                }
                while (Marshal.ReleaseComObject(featCursor) > 0) { }
            }
            while (Marshal.ReleaseComObject(serviceLocationFeatClass) > 0) { }
        }

        /// <summary>
        /// Traces upstream from the provided service location to find the first encountered upstream transformer. Once found, update all
        /// related service points to match the feeding transformers CGC12 number and relationship
        /// </summary>
        /// <param name="geomNetwork"></param>
        /// <param name="serviceLocationFeature"></param>
        private void ProcessServiceLocation(IGeometricNetwork geomNetwork, IFeature serviceLocationFeature)
        {
            //Get all of the feed paths for this servicelocation and find the first encountered transformer feature
            IMMFeedPath[] feedPaths = TraceFacade.GetFeedPaths(serviceLocationFeature);
            if (feedPaths.Length == 0) return;
            IFeature upstreamTransformer = null;
            IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            foreach (IMMFeedPath path in feedPaths)
            {
                foreach (IMMPathElement pathElement in path.PathElements)
                {
                    if (pathElement.ElementType == esriElementType.esriETJunction)
                    {
                        //Check if this upstream feature is a transformer. If it is we can exit our loop
                        IFeature upstreamFeature = TraceFacade.GetFeatureFromPathElement(pathElement, geomNetwork);
                        if (((IDataset)upstreamFeature.Table).BrowseName.ToUpper() == "EDGIS.TRANSFORMER")
                        {
                            upstreamTransformer = upstreamFeature;
                            break;
                        }
                    }
                }
                if (upstreamTransformer != null) { break; }
            }

            if (upstreamTransformer != null)
            {
                //Relate any Service Points of the service location to the transformer as well.
                UpdateServicePoints(serviceLocationFeature, upstreamTransformer);
            }
        }

        int transformerCGC12Idx = -1;
        int transformerGlobalIdx = -1;
        int servicePointCGC12Idx = -1;
        int servicePointXFRIdx = -1;
        
        /// <summary>
        /// Updates the provided service locations' related service points to match the provided transformers' CGC12 number. If the service point
        /// and transformer are not related, then it updates the service points relationship as well.
        /// </summary>
        /// <param name="serviceLocation"></param>
        /// <param name="transformerFeature"></param>
        private void UpdateServicePoints(IFeature serviceLocation, IFeature transformerFeature)
        {
            try
            {
                //Get our field indexes
                if (transformerCGC12Idx < 0) { transformerCGC12Idx = transformerFeature.Fields.FindField("CGC12"); }
                if (transformerGlobalIdx < 0) { transformerGlobalIdx = transformerFeature.Fields.FindField("GLOBALID"); }

                //Get our transformer values
                object CGC12Value = transformerFeature.get_Value(transformerCGC12Idx);
                object globalIDValue = transformerFeature.get_Value(transformerGlobalIdx);

                //Process through all the service points that are related to this service location
                ISet relatedServicePoints = ServiceLocationToServicePoint.GetObjectsRelatedToObject(serviceLocation);
                relatedServicePoints.Reset();
                IRow row = null;
                while ((row = relatedServicePoints.Next() as IRow) != null)
                {
                    //Get our service point field indexes
                    if (servicePointCGC12Idx < 0) { servicePointCGC12Idx = row.Fields.FindField("CGC12"); }
                    if (servicePointXFRIdx < 0) { servicePointXFRIdx = row.Fields.FindField("TRANSFORMERGUID"); }

                    //Check if this service point is already related to this transformer or not
                    object servicePointXFRGlobalID = row.get_Value(servicePointXFRIdx);
                    if (servicePointXFRGlobalID == null || servicePointXFRGlobalID.ToString().ToUpper() != globalIDValue.ToString().ToUpper())
                    {
                        //Create a relationship between this service point and transformer
                        TransformerToServicePoint.CreateRelationship(transformerFeature as IObject, row as IObject);
                    }

                    //Check if this service point already matches the transformers' CGC12 number
                    object currentServicePointCGC12 = row.get_Value(servicePointCGC12Idx);
                    if (currentServicePointCGC12.ToString() != CGC12Value.ToString())
                    {
                        //Update the CGC12 value on the service point to match the transformer
                        row.set_Value(servicePointCGC12Idx, CGC12Value);
                        row.Store();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private IRelationshipClass TransformerToServicePoint = null;
        private IRelationshipClass ServiceLocationToServicePoint = null;
        private void btnExecute_Click(object sender, EventArgs e)
        {
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;
            mmAutoUpdaterMode currentAUMode = immAutoupdater.AutoUpdaterMode;
            immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;

            try
            {
                if (Miner.Geodatabase.Edit.Editor.EditWorkspace == null) { MessageBox.Show("Must be editing to use this tool"); }

                Miner.Geodatabase.Edit.Editor.StartOperation();
                IFeatureWorkspace featWorkspace = Miner.Geodatabase.Edit.Editor.EditWorkspace as IFeatureWorkspace;
                ServiceLocationToServicePoint = featWorkspace.OpenRelationshipClass("EDGIS.ServiceLocation_ServicePoint");
                TransformerToServicePoint = featWorkspace.OpenRelationshipClass("EDGIS.Transformer_ServicePoint");

                //Get a list of our edited features
                lblCurrentStep.Text = "Determining version differences";
                Application.DoEvents();
                Dictionary<string, List<int>> editedOIDsByTable = GetEditedFeatures();

                lblCurrentStep.Text = "Determine edited feeders";
                Application.DoEvents();
                List<string> editedFeeders = UpdatedFeeders(editedOIDsByTable);

                lblCurrentStep.Text = "Determine Service Locations to Process";
                IFeatureDataset featDS = featWorkspace.OpenFeatureDataset("EDGIS.ElectricDataset");
                INetworkCollection networkCollection = featDS as INetworkCollection;
                IGeometricNetwork elecDistNetwork = networkCollection.get_GeometricNetworkByName("EDGIS.ElectricDistNetwork");

                int circuitsProcessed = 0;
                lblCurrentStep.Text = "Tracing circuits for Service Locations to Process";
                Application.DoEvents();
                List<int> ServiceLocationOIDs = new List<int>();
                foreach (string circuitID in editedFeeders)
                {
                    UpdateProgress("Tracing circuit " + circuitID, circuitsProcessed, editedFeeders.Count, false);
                    ServiceLocationOIDs.AddRange(GetServiceLocationOIDs(elecDistNetwork, circuitID));

                    circuitsProcessed++;
                    UpdateProgress("Tracing circuit " + circuitID, circuitsProcessed, editedFeeders.Count, false);
                }

                ServiceLocationOIDs = ServiceLocationOIDs.Distinct().ToList();

                //Finally we can process each service location
                lblCurrentStep.Text = "Updating Service Point CGC12 and Relationships to match feeding Transformers";
                ProcessServiceLocations(elecDistNetwork, ServiceLocationOIDs);
            }
            catch (Exception ex)
            {
                //Abort the edit operation if something failed
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.AbortOperation(); }
                MessageBox.Show("Failed processing: " + ex.Message);
            }
            finally
            {
                //Reset the autoupdater made
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.StopOperation("CGC12 Update"); }
                immAutoupdater.AutoUpdaterMode = currentAUMode;
            }
        }
    }
}
