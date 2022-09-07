using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using Miner.Geodatabase.Integration.Electric;
using Miner.Geodatabase.Integration.Configuration;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Interface.Integration.DMS.Common;
using PGE.Interface.Integration.DMS.Processors;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.Geometry;
using System.Runtime.InteropServices;
using PGE.Interface.Integration.DMS.Manager;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesGDB;
using System.Text.RegularExpressions;
using Oracle.DataAccess.Client;
using PGE_DBPasswordManagement;

namespace PGE.Interface.Integration.DMS
{
    /// <summary>
    /// This class implements the Network Adapter PipelineRule interface to process data after it has been extracted. 
    /// It takes the data after it has been traced and extracted and processes each edge one at a time then each junction 
    /// one at a time. If an edge is idle or deactivated, the extractor will not export it.
    /// </summary>
    [ConfigurationElementType(typeof(PipelineRuleConfigurationElement))]
    public class CADOPS : SimpleHandler<ElectricExportScope, ElectricInfo>
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        private static int _schemDiagramClassID = -1;
        //private static SchematicExtractor _schematics;
        private static Subtypes _subtypes;
        private static Dictionary<ObjectKey, short> _lineState;
        private PolygonProcessor _polygons;
        public static int TotalFeaturesProcessed = 0;
        public static Exception CadopsException = null;
        public static string ErrorMessage = "";
        private static IWorkspace _schemWorkspace = null;
        private static IWorkspace _EDWorkspace = null;
        private static Dictionary<string, IFeatureClass> _schemTables = null;
        public static bool isSubstation = false;
        public static Dictionary<int, List<string>> junctionClassIDToGlobalIDs = new Dictionary<int, List<string>>();
        public static Dictionary<int, List<int>> ClassIDToObjectIDMap = new Dictionary<int, List<int>>();
        public static Dictionary<string, IFeatureClass> EDSecondaryObjectClasses = new Dictionary<string, IFeatureClass>();
        private ControlTable _controlTable = new ControlTable(Configuration.CadopsConnection);
        public static bool IsSecondAttempt = false;
        /// <summary>
        /// Stores the Status of line segments. Used to determine the Status of generic junctions, based on the lines 
        /// they are connected to, since generic junctions do not have a Status Field.
        /// </summary>
        public static Dictionary<ObjectKey, short> LineState
        {
            get
            {
                if (_lineState == null)
                {
                    _lineState = new Dictionary<ObjectKey, short>();
                }
                return CADOPS._lineState;
            }
        }
        /// <summary>
        /// All of the feature class subtypes. Used by processors to convert the subtype code to its description.
        /// </summary>
        public static Subtypes Subtypes
        {
            get { return CADOPS._subtypes; }
        }
        /// <summary>
        /// Perform any necessary initialization before processing circuits.
        /// </summary>
        /// <param name="scope">Settings used to trace the circuits</param>
        /// <returns>Settings used to trace the circuits</returns>
        protected override ElectricExportScope Initialize(ElectricExportScope scope)
        {
            _lineState = new Dictionary<ObjectKey, short>();
            
            if (_subtypes == null)//first time initializing
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    //_schematics = new SchematicExtractor(scope.GdbAccess.Workspace);
                    _log4.Debug(DateTime.Now + ": Initializing PolygonProcessor");
                    _polygons = new PolygonProcessor(scope.GdbAccess.Workspace);
                    _log4.Debug(DateTime.Now + ": Finished Initializing PolygonProcessor");
                    _log4.Debug(DateTime.Now + ": Initializing DomainManager");
                    DomainManager.Instance.Initialize("Domains.xml", "DMS");
                    _log4.Debug(DateTime.Now + ": Finished Initializing DomainManager");
                    _log4.Debug(DateTime.Now + ": Initializing ColumnMapper");
                    ColumnMapper.Instance.Initialize();
                    _log4.Debug(DateTime.Now + ": Finished Initializing ColumnMapper");
                    _log4.Debug(DateTime.Now + ": Initializing StaticSites");
                    StaticSites.Instance.Initialize();
                    _log4.Debug(DateTime.Now + ": Finished Initializing StaticSites");
                    _log4.Debug(DateTime.Now + ": Initializing Subtypes");
                    if (!CADOPS.isSubstation) { _subtypes = new Subtypes(scope.GdbAccess.Workspace, true, false, "EDSubtypeMapping.txt", "EDFCIDMapping.txt", "EDDomainsmapping.txt"); }
                    else { _subtypes = new Subtypes(scope.GdbAccess.Workspace, true, false, "SUBSubtypeMapping.txt", "SUBFCIDMapping.txt", "SUBDomainsmapping.txt"); }
                    _log4.Debug(DateTime.Now + ": Finished Initializing Subtypes");
                    stopwatch.Stop();
                    string time = stopwatch.Elapsed.ToString();
                    _log4.Debug("Configuration took " + time + " to load.");
                    //_subtypes.PrintDomains();
                }
                catch (Exception ex)
                {
                    _log4.Error("Failed initialization: " + ex.Message + " StackTrace: " + ex.StackTrace);
                }
            }
            return scope;
        }
        /// <summary>
        /// Process the traced circuits
        /// </summary>
        /// <param name="scope">Settings used to trace the circuits</param>
        /// <param name="childResult">The traced circuits</param>
        /// <returns>The traced circuits</returns>
        protected override ElectricInfo Execute(ElectricExportScope scope, ElectricInfo childResult)
        {
            _EDWorkspace = scope.GdbAccess.Workspace;
            _controlTable = new ControlTable(Configuration.CadopsConnection);
            string curfeeder = "";
            string featureclass = "";
            ErrorMessage = "";
            int objectid = 0;
            TotalFeaturesProcessed = 0;
            DataSet set = Utilities.BuildDataSet();
            try
            {
                PGE.Interface.Integration.DMS.Processors.Path.ClassIDToUIDMap.Clear();
                PGE.Interface.Integration.DMS.Processors.Path.InvalidGeometryByClassID.Clear();

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Site site = new Site(set);
                EdgeProcessor ep = new EdgeProcessor(set);
                ep.Sites = site;
                JunctionProcessor jp = new JunctionProcessor(set);
                jp.Sites = site;
                _polygons.Data = set;
                int features = 0;

                CADOPS.junctionClassIDToGlobalIDs = new Dictionary<int, List<string>>();
                CADOPS.ClassIDToObjectIDMap = new Dictionary<int, List<int>>();
                using (IEnumerator<ICircuit> enumerator = childResult.Circuits.Values.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        FeederInfo feederInfo = (FeederInfo)enumerator.Current;
                        jp.CurFeederInfo = feederInfo;
                        curfeeder = feederInfo.FeederID;
                        _log4.Debug("Processing Circuit: " + curfeeder);

                        //record that we started processing this circuit
                        DataRow export = set.Tables["DMSSTAGING.EXPORTED"].NewRow();
                        export["EXPORT_ID"] = curfeeder;
                        if (NAStandalone.Substation)
                        {
                            export["EXPORT_ID"] = feederInfo.SubstationID;
                            export["EXPORT_TYPE"] = "S";
                            _polygons.AddJunction(feederInfo.JunctionFeatures.Values.ElementAt(0), feederInfo.SubstationID);
                        }
                        else
                        {
                            export["EXPORT_TYPE"] = "C";
                        }
                        export["DATE_PROCESSED"] = DateTime.Now;
                        export["STATUS"] = 0; //set this to failed until it succeeds
                        set.Tables["DMSSTAGING.EXPORTED"].Rows.Add(export);

                        List<JunctionInfo> missingJunctions = new List<JunctionInfo>();
                        
                        //jp.Scoordinates = _schematics.GetCircuitCoordinates(curfeeder);
                        foreach (KeyValuePair<int, EdgeInfo> kvp in feederInfo.EdgeElements)
                        {
                            EdgeFeatureInfo current = null;
                            if (feederInfo.EdgeFeatures.ContainsKey(kvp.Value.ObjectKey))
                            {
                                current = feederInfo.EdgeFeatures[kvp.Value.ObjectKey];
                            }
                            else
                            {
                                //Some feature classes don't get returned in the EdgeFeatures list (i.e. SecOHConductor)
                                continue;
                            }

                            ControlTable.SettingsErrors.Clear();
                            featureclass = "EDGIS." + current.TableName;
                            objectid = current.ObjectID;
                            short state = Utilities.GetState(current);
                            if (!LineState.ContainsKey(current.Key))
                            {
                                LineState.Add(current.Key, state);
                            }

                            /*
                            try
                            {
                                JunctionInfo testInfo = GetOrCreateMissingJunction(scope, enumerator.Current, kvp.Value as ElementInfo, kvp.Value.FromJunction.EID);
                                JunctionFeatureInfo junctionFeatureInfoTest = new JunctionFeatureInfo(testInfo.ObjectKey);
                                junctionFeatureInfoTest.Junction = testInfo;
                                AddAttributesForClass(scope, enumerator.Current, junctionFeatureInfoTest.ObjectID, junctionFeatureInfoTest);
                                _log4.Debug("Junction missing from trace. Attempting to add it: " +
                                    junctionFeatureInfoTest.TableName + ": " + junctionFeatureInfoTest.ObjectID);
                                jp.AddJunction(junctionFeatureInfoTest);
                            }
                            catch (Exception e)
                            {

                            }
                            */

                            //This logic was added to confirm that all nodes for a given line will be properly added to the node table.
                            //This ensures that there is no longer any missing nodes.  we 
                            //will simply add a generic node for this feature to ensure there are no nodes missing.
                            if (ep.shouldIncludeJunction(kvp.Value.FromJunction) &&
                                !feederInfo.JunctionFeatures.ContainsKey(kvp.Value.FromJunction.ObjectKey))
                            {
                                missingJunctions.Add(kvp.Value.FromJunction);
                                //ep.AddGenericNode(kvp.Value.FromJunction, kvp.Value.Line.From);
                            }
                            if (ep.shouldIncludeJunction(kvp.Value.ToJunction) &&
                                !feederInfo.JunctionFeatures.ContainsKey(kvp.Value.ToJunction.ObjectKey))
                            {
                                missingJunctions.Add(kvp.Value.ToJunction);
                                //ep.AddGenericNode(kvp.Value.ToJunction, kvp.Value.Line.To);
                            }

                            ep.AddEdge(current, _controlTable);

                            //Process the settings warnings when associated data can't be found in the PGEData schema tables.
                            foreach (string warning in ControlTable.SettingsErrors)
                            {
                                string truncatedWarning = warning;
                                if (truncatedWarning.Length > 4000) { truncatedWarning = truncatedWarning.Substring(0, 4000); }

                                DataRow FeatErrorRow = set.Tables["DMSSTAGING.PGE_DMS_FEATURE_WARNINGS"].NewRow();
                                FeatErrorRow["FEATURE_CLASS"] = featureclass;
                                FeatErrorRow["OBJECTID"] = current.ObjectID;
                                FeatErrorRow["WARNING"] = truncatedWarning;
                                FeatErrorRow["CIRCUITID"] = curfeeder;
                                set.Tables["DMSSTAGING.PGE_DMS_FEATURE_WARNINGS"].Rows.Add(FeatErrorRow);
                            }
                            ControlTable.SettingsErrors.Clear();

                            features++;
                        }

                        if (missingJunctions.Count > 0)
                        {
                            Dictionary<int, List<int>> ClassIDToOIDMap = new Dictionary<int, List<int>>();
                            foreach (JunctionInfo info in missingJunctions)
                            {
                                try
                                {
                                    ClassIDToOIDMap[info.ObjectClassID].Add(info.ObjectID);
                                }
                                catch
                                {
                                    ClassIDToOIDMap.Add(info.ObjectClassID, new List<int>());
                                    ClassIDToOIDMap[info.ObjectClassID].Add(info.ObjectID);
                                }
                            }
                            StringBuilder builder = new StringBuilder();
                            builder.Append("ObjectClass:OID1,OID2,etc - ");
                            foreach (KeyValuePair<int, List<int>> classIDToObjectIDKVP in ClassIDToOIDMap)
                            {
                                string className = _controlTable.GetObjectClassName(classIDToObjectIDKVP.Key);
                                List<int> distinctList = classIDToObjectIDKVP.Value.Distinct().ToList();
                                builder.Append(className + ":");
                                for (int i = 0; i < distinctList.Count; i++)
                                {
                                    DataRow FeatErrorRow = set.Tables["DMSSTAGING.PGE_DMS_FEATURE_ERRORS"].NewRow();
                                    FeatErrorRow["FEATURE_CLASS"] = className;
                                    FeatErrorRow["OBJECTID"] = distinctList[i];
                                    FeatErrorRow["ERROR"] = "Node missing in extract";
                                    FeatErrorRow["CIRCUITID"] = curfeeder;
                                    set.Tables["DMSSTAGING.PGE_DMS_FEATURE_ERRORS"].Rows.Add(FeatErrorRow);
                                    if (i == distinctList.Count - 1)
                                    {
                                        builder.Append(distinctList[i]);
                                    }
                                    else
                                    {
                                        builder.Append(distinctList[i] + ",");
                                    }
                                }
                                builder.Append("; ");
                            }
                            throw new Exception("Unable to export circuit " + curfeeder + " due to missing nodes: " + builder);
                        }

                        if (PGE.Interface.Integration.DMS.Processors.Path.InvalidGeometryByClassID.Count > 0 && Configuration.getBoolSetting("ProcessSchematics", false))
                        {
                            //Invalid geometry found.  We won't export in this case.
                            StringBuilder builder = new StringBuilder();
                            builder.Append("ObjectClass:OID1,OID2,etc - ");
                            foreach (KeyValuePair<int, List<int>> classIDToObjectIDKVP in PGE.Interface.Integration.DMS.Processors.Path.InvalidGeometryByClassID)
                            {
                                string className = _controlTable.GetObjectClassName(classIDToObjectIDKVP.Key);
                                List<int> distinctList = classIDToObjectIDKVP.Value.Distinct().ToList();
                                builder.Append(className + ":");
                                for (int i = 0; i < distinctList.Count; i++)
                                {
                                    DataRow schemRow = set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"].NewRow();
                                    schemRow["FEATURE_CLASS"] = className;
                                    schemRow["OBJECTID"] = distinctList[i];
                                    schemRow["CIRCUITID"] = curfeeder;
                                    schemRow["ERROR"] = "Unable to determine schematics path";
                                    set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"].Rows.Add(schemRow);
                                    if (i == distinctList.Count - 1)
                                    {
                                        builder.Append(distinctList[i]);
                                    }
                                    else
                                    {
                                        builder.Append(distinctList[i] + ",");
                                    }
                                }
                                builder.Append("; ");
                            }
                            _log4.Error("Unable to determine schematics paths for the following features due to invalid complex edges: " + builder);
                            ErrorMessage += "Unable to determine schematics paths for the following features due to invalid complex edges: " + builder;
                            //It was requested to just note the schematics issues, but still send the circuits across. This may change later at which
                            //time logging and error setting above should be removed and the below error should be thrown
                            //throw new Exception("Unable to export circuit " + curfeeder + " due to invalid complex edges: " + builder);
                        }
                        List<JunctionFeatureInfo> defaultJunctions = new List<JunctionFeatureInfo>();                        

                        foreach (JunctionFeatureInfo current2 in feederInfo.JunctionFeatures.Values)
                        {
                            if (!isSubstation)
                            {
                                /*Changes for ENOS to SAP migration - DMS  .. Start */
                                try
                                {
                                    if (current2.TableName.ToUpper() == Configuration.ServiceLocationTableName || current2.TableName.ToUpper() == "EDGIS." + Configuration.ServiceLocationTableName)
                                    {
                                        //Check here the gencategory field value if Primary then only send to DMS
                                        string domainValue = null;
                                        object genCatvalue = current2.Fields[Configuration.GenCategoryFieldName].FieldValue;
                                        //string genCategoryValue = Convert.ToString(current2.Fields["GENCATEGORY"].FieldValue);

                                        //if the domain manager has the domain use that otherwise try looking up the subtype domain
                                        if (DomainManager.Instance.ContainsDomain(Configuration.GenCategoryDomainName))
                                        {
                                            if (genCatvalue is DBNull)
                                            {
                                                domainValue = DomainManager.Instance.GetValue(Configuration.GenCategoryDomainName, null);
                                            }
                                            else
                                            {
                                                domainValue = DomainManager.Instance.GetValue(Configuration.GenCategoryDomainName, genCatvalue.ToString());
                                            }
                                        }

                                        if (domainValue != Configuration.GenCategoryValueForPrimary)
                                            continue;
                                    }
                                }
                                catch (Exception exp)
                                {
                                    _log4.Error(exp.Message + " at " + exp.StackTrace);
                                }
                                /*Changes for ENOS to SAP migration - DMS  .. End */

                                try
                                {
                                    string guid = current2.Fields["GLOBALID"].FieldValue.ToString();
                                    try
                                    {
                                        junctionClassIDToGlobalIDs[current2.ObjectClassID].Add(guid);
                                    }
                                    catch
                                    {
                                        junctionClassIDToGlobalIDs.Add(current2.ObjectClassID, new List<string>());
                                        junctionClassIDToGlobalIDs[current2.ObjectClassID].Add(guid);
                                    }
                                }
                                catch { }
                            }

                            if (current2.ObjectClassID == FCID.Junction)
                            {
                                //Process default junctions last so we can not add them if they are
                                //coincident with any others
                                defaultJunctions.Add(current2);
                            }
                            else
                            {
                                featureclass = "EDGIS." + current2.TableName;
                                objectid = current2.ObjectID;
                                jp.AddJunction(current2, _controlTable);

                                //Process the settings warnings when associated data can't be found in the PGEData schema tables.
                                foreach (string warning in ControlTable.SettingsErrors)
                                {
                                    string truncatedWarning = warning;
                                    if (truncatedWarning.Length > 4000) { truncatedWarning = truncatedWarning.Substring(0, 4000); }

                                    DataRow FeatErrorRow = set.Tables["DMSSTAGING.PGE_DMS_FEATURE_WARNINGS"].NewRow();
                                    FeatErrorRow["FEATURE_CLASS"] = featureclass;
                                    FeatErrorRow["OBJECTID"] = current2.ObjectID;
                                    FeatErrorRow["WARNING"] = truncatedWarning;
                                    FeatErrorRow["CIRCUITID"] = curfeeder;
                                    set.Tables["DMSSTAGING.PGE_DMS_FEATURE_WARNINGS"].Rows.Add(FeatErrorRow);
                                }
                                ControlTable.SettingsErrors.Clear();

                                features++;
                            }
                        }

                        foreach (JunctionFeatureInfo defaultJunction in defaultJunctions)
                        {
                            featureclass = "EDGIS." + defaultJunction.TableName;
                            objectid = defaultJunction.ObjectID;
                            jp.AddJunction(defaultJunction, _controlTable);
                            features++;
                        }

                        //done processing all junctions now check for coincidence with device groups
                        objectid = 0;
                        featureclass = "Post Process";
                        jp.CheckCoincidence();
                        jp.RemapSites();

                        //Excluding remove logic for now as there are primary features downstream of Primary Ties
                        //FeatureRemover remove = new FeatureRemover(set);
                        //remove.RemoveJunctions(jp.Trace.TracedJunctions);
                        //remove.RemoveEdges(jp.Trace.TracedEdges);
                        export["FEATURE_COUNT"] = features;

                        TotalFeaturesProcessed += features;
                    }
                }
                                
                if (!isSubstation && Configuration.getBoolSetting("ProcessSchematics", false)) 
                {
                    _log4.Info("Obtaining schematics X/Y data for Node table");
                    GetGUIDsFromOIDs();

                    Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
                    Dictionary<string, List<string>> invalidSchemWarningGlobalIDs = new Dictionary<string, List<string>>();
                    Dictionary<string, List<string>> invalidSchemGlobalIDs = ProcessSchematicsNodesAndDevices(junctionClassIDToGlobalIDs, set.Tables["DMSSTAGING.NODE"], "GUID", "XN_2", "YN_2", ref invalidSchemWarningGlobalIDs);
                    _log4.Info("Finished obtaining schematics X/Y data for Node table");
                    _log4.Info("Obtaining schematics X/Y data for Device table");
                    ProcessSchematicsNodesAndDevices(junctionClassIDToGlobalIDs, set.Tables["DMSSTAGING.DEVICE"], "GUID", "XD_2", "YD_2", ref temp);
                    _log4.Info("Finished obtaining schematics X/Y data for Device table");

                    if (invalidSchemGlobalIDs.Count > 0)
                    {
                        //Invalid geometry found.  We won't export in this case.
                        StringBuilder builder = new StringBuilder();
                        builder.Append("ObjectClass:GUID1,GUID2,etc - ");
                        foreach (KeyValuePair<string, List<string>> classIDToObjectIDKVP in invalidSchemGlobalIDs)
                        {
                            List<string> distinctList = classIDToObjectIDKVP.Value.Distinct().ToList();
                            builder.Append(classIDToObjectIDKVP.Key + ":");
                            for (int i = 0; i < distinctList.Count; i++)
                            {
                                DataRow schemRow = set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"].NewRow();
                                schemRow["FEATURE_CLASS"] = classIDToObjectIDKVP.Key;
                                schemRow["GUID"] = distinctList[i];
                                schemRow["CIRCUITID"] = curfeeder;
                                schemRow["ERROR"] = "Unable to find matching schematics feature";
                                set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"].Rows.Add(schemRow);
                                if (i == distinctList.Count - 1)
                                {
                                    builder.Append(distinctList[i]);
                                }
                                else
                                {
                                    builder.Append(distinctList[i] + ",");
                                }
                            }
                            builder.Append("; ");
                        }
                        _log4.Error("Unable to find matching schematics node features for the following: " + builder);
                        ErrorMessage += "Unable to find matching schematics node features for the following: " + builder;
                        //It was requested to just note the schematics issues, but still send the circuits across. This may change later at which
                        //time logging and error setting above should be removed and the below error should be thrown
                        //throw new Exception("Unable to export circuit " + curfeeder + " due to no matching schematics features for the following: " + builder);
                    }
                    if (invalidSchemWarningGlobalIDs.Count > 0)
                    {
                        //Invalid geometry found.  We won't export in this case.
                        StringBuilder builder = new StringBuilder();
                        builder.Append("ObjectClass:GUID1,GUID2,etc - ");
                        foreach (KeyValuePair<string, List<string>> classIDToObjectIDKVP in invalidSchemWarningGlobalIDs)
                        {
                            List<string> distinctList = classIDToObjectIDKVP.Value.Distinct().ToList();
                            builder.Append(classIDToObjectIDKVP.Key + ":");
                            for (int i = 0; i < distinctList.Count; i++)
                            {
                                DataRow schemRow = set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_WARNINGS"].NewRow();
                                schemRow["FEATURE_CLASS"] = classIDToObjectIDKVP.Key;
                                schemRow["GUID"] = distinctList[i];
                                schemRow["CIRCUITID"] = curfeeder;
                                schemRow["WARNING"] = "Unable to find matching proposed install schematics feature";
                                set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_WARNINGS"].Rows.Add(schemRow);
                                if (i == distinctList.Count - 1)
                                {
                                    builder.Append(distinctList[i]);
                                }
                                else
                                {
                                    builder.Append(distinctList[i] + ",");
                                }
                            }
                            builder.Append("; ");
                        }
                        _log4.Info("Unable to find matching proposed schematics node features for the following: " + builder);
                        //ErrorMessage += "Unable to find matching proposed schematics node features for the following: " + builder;
                        //It was requested to just note the schematics issues, but still send the circuits across. This may change later at which
                        //time logging and error setting above should be removed and the below error should be thrown
                        //throw new Exception("Unable to export circuit " + curfeeder + " due to no matching schematics features for the following: " + builder);
                    }
                }
                //Now we have processed all the EDER data we need to add in the pathing information for schematics.
                if (!isSubstation && Configuration.getBoolSetting("ProcessSchematics", false))
                {
                    _log4.Info("Obtaining schematics X/Y data for Path table");
                    Dictionary<int, List<string>> invalidSchemWarningGlobalIDs = new Dictionary<int, List<string>>();
                    Dictionary<int, List<string>> invalidSchemGlobalIDs = ProcessSchematicsPaths(set.Tables["DMSSTAGING.NODE"], set.Tables["DMSSTAGING.LINE"], set.Tables["DMSSTAGING.PATH"], ref invalidSchemWarningGlobalIDs);
                    _log4.Info("Finished obtaining schematics X/Y data for Device table");

                    if (invalidSchemGlobalIDs.Count > 0)
                    {
                        //Invalid geometry found.  We won't export in this case.
                        StringBuilder builder = new StringBuilder();
                        builder.Append("ObjectClass:GUID1,GUID2,etc - ");
                        foreach (KeyValuePair<int, List<string>> classIDToObjectIDKVP in invalidSchemGlobalIDs)
                        {
                            IFeatureClass schemFeatClass = CADOPS.GetSchemFeatureClass("", classIDToObjectIDKVP.Key);
                            IDataset schemDS = schemFeatClass as IDataset;
                            List<string> distinctList = classIDToObjectIDKVP.Value.Distinct().ToList();
                            builder.Append(schemDS.BrowseName + ":");
                            for (int i = 0; i < distinctList.Count; i++)
                            {
                                DataRow schemRow = set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"].NewRow();
                                schemRow["FEATURE_CLASS"] = schemDS.BrowseName;
                                schemRow["GUID"] = distinctList[i];
                                schemRow["CIRCUITID"] = curfeeder;
                                schemRow["ERROR"] = "Unable to find matching schematics feature";
                                set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"].Rows.Add(schemRow);
                                if (i == distinctList.Count - 1)
                                {
                                    builder.Append(distinctList[i]);
                                }
                                else
                                {
                                    builder.Append(distinctList[i] + ",");
                                }
                            }
                            builder.Append("; ");
                        }
                        _log4.Error("Unable to find matching schematics line features for the following: " + builder);
                        ErrorMessage += "Unable to find matching schematics line features for the following: " + builder;
                        //It was requested to just note the schematics issues, but still send the circuits across. This may change later at which
                        //time logging and error setting above should be removed and the below error should be thrown
                        //throw new Exception("Unable to export circuit " + curfeeder + " due to no matching schematics features for the following: " + builder);
                    }
                    if (invalidSchemWarningGlobalIDs.Count > 0)
                    {
                        //Invalid geometry found.  We won't export in this case.
                        StringBuilder builder = new StringBuilder();
                        builder.Append("ObjectClass:GUID1,GUID2,etc - ");
                        foreach (KeyValuePair<int, List<string>> classIDToObjectIDKVP in invalidSchemWarningGlobalIDs)
                        {
                            IFeatureClass schemFeatClass = CADOPS.GetSchemFeatureClass("", classIDToObjectIDKVP.Key);
                            IDataset schemDS = schemFeatClass as IDataset;
                            List<string> distinctList = classIDToObjectIDKVP.Value.Distinct().ToList();
                            builder.Append(schemDS.BrowseName + ":");
                            for (int i = 0; i < distinctList.Count; i++)
                            {
                                DataRow schemRow = set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_WARNINGS"].NewRow();
                                schemRow["FEATURE_CLASS"] = schemDS.BrowseName;
                                schemRow["GUID"] = distinctList[i];
                                schemRow["CIRCUITID"] = curfeeder;
                                schemRow["WARNING"] = "Unable to find matching proposed install schematics feature";
                                set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_WARNINGS"].Rows.Add(schemRow);
                                if (i == distinctList.Count - 1)
                                {
                                    builder.Append(distinctList[i]);
                                }
                                else
                                {
                                    builder.Append(distinctList[i] + ",");
                                }
                            }
                            builder.Append("; ");
                        }
                        _log4.Info("Unable to find matching proposed install schematics line features for the following: " + builder);
                        //ErrorMessage += "Unable to find matching schematics line features for the following: " + builder;
                        //It was requested to just note the schematics issues, but still send the circuits across. This may change later at which
                        //time logging and error setting above should be removed and the below error should be thrown
                        //throw new Exception("Unable to export circuit " + curfeeder + " due to no matching schematics features for the following: " + builder);
                    }

                }

                //if we made it here there were no extract errors
                SetStatus(1, set);

                stopwatch.Stop();
                string time = stopwatch.Elapsed.ToString();
                _log4.Debug(features + " Features Processed. Total Time: " + time);

                Common.Oracle.BulkLoadData(set, Configuration.CadopsConnection);

            }
            catch (Exception ex)
            {
                CadopsException = ex;
                _log4.Error("Error extracting circuit " + curfeeder + " for CADOPS: " + ex.Message);
                set = SetError("Error extracting circuit " + curfeeder + " for CADOPS: " + ex.Message, set);
                if (IsSecondAttempt) { Common.Oracle.BulkLoadData(set, Configuration.CadopsConnection); }
                throw ex;
            }
            return childResult;
        }

        public static int GetSchemDiagramClassID()
        {
            if (_schemDiagramClassID >= 0) { return _schemDiagramClassID; }

            OracleConnection conn = null;

            try
            {
                conn = new OracleConnection(Configuration.SchemOracleConnection);
                conn.Open();
                string sql = "SELECT OBJECTCLASSID FROM " + Configuration.SchemDiagramClassIDTable + " WHERE UPPER(CREATIONNAME) = 'CIRCUITMAP'";
                _log4.Debug("Determining schematics diagram class ID: " + sql);
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.CommandType = CommandType.Text;

                    using (OracleDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                object schemDiagClassIDObject = dataReader[0];
                                _schemDiagramClassID = Int32.Parse(schemDiagClassIDObject.ToString());
                            }
                        }
                        dataReader.Close();
                        dataReader.Dispose();
                    }
                    cmd.Dispose();
                }
                _log4.Debug("Schematics diagram class ID: " + _schemDiagramClassID);
            }
            catch (Exception ex)
            {
                _log4.Error("Error determining schematics diagram class ID.  " + System.Environment.NewLine + ex.ToString());
                throw ex;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }

            return _schemDiagramClassID;
        }

        Dictionary<int, List<string>> ClassIDToOIDGUIDMap = new Dictionary<int, List<string>>();
        private void GetGUIDsFromOIDs()
        {
            ClassIDToOIDGUIDMap = new Dictionary<int, List<string>>();
            //Now we have our list of UIDs and GUIDs that we have to pull over from schematics.
            Dictionary<int, List<string>> InListValuesByClassID = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<int, List<int>> OIDList in CADOPS.ClassIDToObjectIDMap)
            {
                InListValuesByClassID.Add(OIDList.Key, new List<string>());
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<int, List<int>> kvp in CADOPS.ClassIDToObjectIDMap)
            {
                for (int i = 0; i < kvp.Value.Count; i++)
                {
                    if ((i == kvp.Value.Count - 1) || (((i % 999) == 0) && (i != 0)))
                    {
                        builder.Append("'" + kvp.Value[i] + "'");
                        InListValuesByClassID[kvp.Key].Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else
                    {
                        builder.Append("'" + kvp.Value[i] + "'" + ",");
                    }
                }
            }

            foreach (KeyValuePair<int, List<string>> InListValuesKVP in InListValuesByClassID)
            {
                string featClassName = _controlTable.GetObjectClassName(InListValuesKVP.Key);
                IFeatureClass FeatClass = CADOPS.GetFeatureClass(featClassName);
                int guidFieldIndex = FeatClass.FindField("GLOBALID");
                int OIDFieldIndex = FeatClass.FindField("OBJECTID");
                foreach (string guidInList in InListValuesKVP.Value)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.SubFields = "OBJECTID,GLOBALID";
                    qf.WhereClause = "OBJECTID in (" + guidInList + ")";
                    IFeatureCursor featCursor = FeatClass.Search(qf, true);
                    IFeature feat = null;
                    while ((feat = featCursor.NextFeature()) != null)
                    {
                        string OID = feat.get_Value(OIDFieldIndex).ToString();
                        string guid = feat.get_Value(guidFieldIndex).ToString();
                        try
                        {
                            //Changes for ENOS to SAP migration - DMS .. Start

                            //ClassIDToOIDGUIDMap[InListValuesKVP.Key].Add(OID + ":" + guid);
                            //junctionClassIDToGlobalIDs[InListValuesKVP.Key].Add(guid);


                            if (!ClassIDToOIDGUIDMap.Keys.Contains(InListValuesKVP.Key))
                                ClassIDToOIDGUIDMap.Add(InListValuesKVP.Key, new List<string>());


                            if (!ClassIDToOIDGUIDMap[InListValuesKVP.Key].Contains(OID + ":" + guid))
                                ClassIDToOIDGUIDMap[InListValuesKVP.Key].Add(OID + ":" + guid);

                            if (!junctionClassIDToGlobalIDs.Keys.Contains(InListValuesKVP.Key))
                                junctionClassIDToGlobalIDs.Add(InListValuesKVP.Key, new List<string>());

                            if (!junctionClassIDToGlobalIDs[InListValuesKVP.Key].Contains(guid))
                                junctionClassIDToGlobalIDs[InListValuesKVP.Key].Add(guid);

                            //Changes for ENOS to SAP migration - DMS .. End
                        }
                        catch
                        {
                            //Changes for ENOS to SAP migration - DMS .. Start

                            //junctionClassIDToGlobalIDs.Add(InListValuesKVP.Key, new List<string>());
                            //junctionClassIDToGlobalIDs[InListValuesKVP.Key].Add(guid);
                            //ClassIDToOIDGUIDMap.Add(InListValuesKVP.Key, new List<string>());
                            //ClassIDToOIDGUIDMap[InListValuesKVP.Key].Add(OID + ":" + guid);


                            if (!junctionClassIDToGlobalIDs.Keys.Contains(InListValuesKVP.Key))
                                junctionClassIDToGlobalIDs.Add(InListValuesKVP.Key, new List<string>());

                            if (!junctionClassIDToGlobalIDs[InListValuesKVP.Key].Contains(guid))
                                junctionClassIDToGlobalIDs[InListValuesKVP.Key].Add(guid);

                            if (!ClassIDToOIDGUIDMap.Keys.Contains(InListValuesKVP.Key))
                                ClassIDToOIDGUIDMap.Add(InListValuesKVP.Key, new List<string>());

                            if (!ClassIDToOIDGUIDMap[InListValuesKVP.Key].Contains(OID + ":" + guid))
                                ClassIDToOIDGUIDMap[InListValuesKVP.Key].Add(OID + ":" + guid);

                            //Changes for ENOS to SAP migration - DMS .. End
                        }
                    }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }
                }
            }
        }

        private Dictionary<string, List<string>> ProcessSchematicsNodesAndDevices(Dictionary<int, List<string>> ClassIDToGUIDMap, DataTable NodeDT,
            string guidFieldName, string xFieldName, string yFieldName, ref Dictionary<string, List<string>> invalidProposedInstallSchemFeatures)
        {
            Dictionary<string, List<string>> invalidSchemGlobalIDs = new Dictionary<string, List<string>>();

            //Need to assign the real globalID values to the SecondaryJunction features
            for (int i = 0; i < NodeDT.Rows.Count; i++)
            {
                string guid = NodeDT.Rows[i][guidFieldName].ToString();

                //If this is a secondary junction we don't care about mapping it to schematics.
                if (NodeDT.Rows[i]["FEATURE_CLASS"].ToString() == "SecondaryJunction")
                {
                    //For secondary junctions we have to interpret the guid field by "ObjectClassID:ObjectID" and then assign the proper GUID value
                    string[] ObjectIDToOID = Regex.Split(guid, ":");
                    int ObjectID = Int32.Parse(ObjectIDToOID[0]);
                    string OID = ObjectIDToOID[1];
                    foreach (KeyValuePair<int, List<string>> kvp in ClassIDToOIDGUIDMap)
                    {
                        if (kvp.Key != ObjectID) { continue; }

                        foreach (string item in kvp.Value)
                        {
                            string[] OIDToGUID = Regex.Split(item, ":");
                            string objectID = OIDToGUID[0];

                            if (OID == objectID)
                            {
                                guid = OIDToGUID[1];
                                NodeDT.Rows[i]["GUID"] = guid;
                                break;
                            }
                        }
                    }
                }
            }

            Dictionary<int, List<string>> SchemGUIDsNeeded = new Dictionary<int, List<string>>();
            foreach (DataRow row in NodeDT.Rows)
            {
                string node_guid = row["GUID"].ToString();
                int classID = -1;

                if (classID == -1)
                {
                    foreach (KeyValuePair<int, List<string>> kvp in ClassIDToGUIDMap)
                    {
                        if (kvp.Value.Contains(node_guid))
                        {
                            classID = kvp.Key;
                            break;
                        }
                    }
                }

                if (classID == -1) { continue; }

                try
                {
                    //Changes for ENOS to SAP migration - DMS .. Start

                    //SchemGUIDsNeeded[classID].Add(node_guid);


                    if (!SchemGUIDsNeeded.ContainsKey(classID))
                        SchemGUIDsNeeded.Add(classID, new List<string>());

                    if (!SchemGUIDsNeeded[classID].Contains(node_guid))
                        SchemGUIDsNeeded[classID].Add(node_guid);


                    //Changes for ENOS to SAP migration - DMS .. End
                }
                catch
                {
                    SchemGUIDsNeeded.Add(classID, new List<string>());
                    SchemGUIDsNeeded[classID].Add(node_guid);
                }
            }

            //Now we have our list of UIDs and GUIDs that we have to pull over from schematics.
            Dictionary<int, List<string>> InListValuesByClassID = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<int, List<string>> guidList in SchemGUIDsNeeded)
            {
                InListValuesByClassID.Add(guidList.Key, new List<string>());
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<int, List<string>> schemGUIDsKVP in SchemGUIDsNeeded)
            {
                for (int i = 0; i < schemGUIDsKVP.Value.Count; i++)
                {
                    if ((i == schemGUIDsKVP.Value.Count - 1) || (((i % 999) == 0) && (i != 0)))
                    {
                        builder.Append("'" + schemGUIDsKVP.Value[i] + "'");
                        InListValuesByClassID[schemGUIDsKVP.Key].Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else
                    {
                        builder.Append("'" + schemGUIDsKVP.Value[i] + "'" + ",");
                    }
                }
            }

            Dictionary<string, SchemPoint> guidToSchemPoint = new Dictionary<string, SchemPoint>();
            foreach (KeyValuePair<int, List<string>> InListValuesKVP in InListValuesByClassID)
            {
                IFeatureClass schemFeatClass = CADOPS.GetSchemFeatureClass("", InListValuesKVP.Key);
                bool hasStatusField = (schemFeatClass.FindField("STATUS") >= 0);
                int uguidFieldIndex = schemFeatClass.FindField("UGUID");
                foreach (string guidInList in InListValuesKVP.Value)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.SubFields = "SHAPE,UGUID";
                    if (hasStatusField) { qf.WhereClause = "DIAGRAMCLASSID = " + GetSchemDiagramClassID() + " AND (ISDISPLAYED = -1 OR (ISDISPLAYED = 0 AND STATUS = 0)) AND UGUID in (" + guidInList + ")"; }
                    else { qf.WhereClause = "DIAGRAMCLASSID = " + GetSchemDiagramClassID() + " AND ISDISPLAYED = -1 AND UGUID in (" + guidInList + ")"; }
                    IFeatureCursor featCursor = schemFeatClass.Search(qf, true);
                    IFeature feat = null;
                    while ((feat = featCursor.NextFeature()) != null)
                    {
                        string uguid = feat.get_Value(uguidFieldIndex).ToString();
                        IPoint point = feat.Shape as IPoint;
                        SchemPoint schemPoint = new SchemPoint();
                        schemPoint.x = Utilities.ConvertXY(point.X);
                        schemPoint.y = Utilities.ConvertXY(point.Y);
                        try
                        {
                            guidToSchemPoint.Add(uguid, schemPoint);
                        }
                        catch { }
                    }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (feat != null) { while (Marshal.ReleaseComObject(feat) > 0) { } }
                }
            }

            //Now we have our schematics X/Y values we can adjust the datarows
            for (int i = 0; i < NodeDT.Rows.Count; i++)
            {
                string guid = NodeDT.Rows[i][guidFieldName].ToString();

                try
                {
                    //Assign the schematic x/y coordinates
                    SchemPoint point = guidToSchemPoint[guid];
                    NodeDT.Rows[i][xFieldName] = point.x;
                    NodeDT.Rows[i][yFieldName] = point.y;
                    guidToSchemPoint.Remove(guid);
                }
                catch
                {
                    string state = "1";
                    try
                    {
                        state = NodeDT.Rows[i]["STATE"].ToString();
                    }
                    catch
                    {
                        _log4.Debug("Failed checking current node state");
                        _log4.Debug("Failed checking current node state: " + NodeDT.Rows[i]["FEATURE_CLASS"].ToString());
                    }
                    try
                    {
                        if (state == "2") { invalidProposedInstallSchemFeatures[NodeDT.Rows[i]["FEATURE_CLASS"].ToString()].Add(guid); }
                        else { invalidSchemGlobalIDs[NodeDT.Rows[i]["FEATURE_CLASS"].ToString()].Add(guid); }
                    }
                    catch
                    {
                        if (state == "2")
                        {
                            invalidProposedInstallSchemFeatures.Add(NodeDT.Rows[i]["FEATURE_CLASS"].ToString(), new List<string>());
                            invalidProposedInstallSchemFeatures[NodeDT.Rows[i]["FEATURE_CLASS"].ToString()].Add(guid);
                        }
                        else
                        {
                            invalidSchemGlobalIDs.Add(NodeDT.Rows[i]["FEATURE_CLASS"].ToString(), new List<string>());
                            invalidSchemGlobalIDs[NodeDT.Rows[i]["FEATURE_CLASS"].ToString()].Add(guid);                            
                        }
                    }
                }
            }

            return invalidSchemGlobalIDs;
        }

        /// <summary>
        /// Processes the schematics paths.  returns a list of guids that could not be found
        /// </summary>
        /// <param name="NodeDT">Node data table</param>
        /// <param name="LineDT">Line data table</param>
        /// <param name="PathDT">Path data table</param>
        /// <returns></returns>
        private Dictionary<int, List<string>> ProcessSchematicsPaths(DataTable NodeDT, DataTable LineDT, DataTable PathDT, ref Dictionary<int, List<string>> invalidProposedInstallSchemFeatures)
        {
            Dictionary<int, List<string>> guidsNeeded = new Dictionary<int, List<string>>();
            Dictionary<int, List<string>> SchemGUIDsNeeded = new Dictionary<int, List<string>>();
            Dictionary<string, string> SchemGUIDToUIDMap = new Dictionary<string, string>();
            foreach (DataRow row in LineDT.Rows)
            {
                string line_uid = row["FPOS"].ToString();
                string line_guid = row["GUID"].ToString();
                string no_key_1 = row["NO_KEY_1"].ToString();
                int classID = -1;
                foreach(KeyValuePair<int, List<string>> uidList in PGE.Interface.Integration.DMS.Processors.Path.ClassIDToUIDMap)
                {
                    if (uidList.Value.Contains(line_uid))
                    {
                        classID = uidList.Key;
                    }
                }

                if (classID == -1) { continue; }

                try
                {
                    SchemGUIDsNeeded[classID].Add(line_guid);
                    guidsNeeded[classID].Add(line_guid);
                }
                catch
                {
                    guidsNeeded.Add(classID, new List<string>());
                    SchemGUIDsNeeded.Add(classID, new List<string>());
                    SchemGUIDsNeeded[classID].Add(line_guid);
                    guidsNeeded[classID].Add(line_guid);
                }
                try
                {
                    SchemGUIDToUIDMap.Add(line_guid, line_uid + "," + no_key_1);
                }
                catch { /*Potentially already added the guid. Ignore and move on*/ }
                try
                {
                    
                }
                catch { }

            }

            //Create a mapping to lookup node NFPOS values to xn_2 values (the schematics x position)
            Dictionary<string, string> NFPOSToXN2 = new Dictionary<string, string>();
            foreach (DataRow row in NodeDT.Rows)
            {
                string nfpos = row["NFPOS"].ToString();
                string XN_2 = row["XN_2"].ToString();

                try
                {
                    NFPOSToXN2.Add(nfpos, XN_2);
                }
                catch { }
            }

            //Now we have our list of UIDs and GUIDs that we have to pull over from schematics.
            Dictionary<int, List<string>> InListValuesByClassID = new Dictionary<int, List<string>>();
            foreach (KeyValuePair<int, List<string>> uidList in PGE.Interface.Integration.DMS.Processors.Path.ClassIDToUIDMap)
            {
                InListValuesByClassID.Add(uidList.Key, new List<string>());
            }

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<int, List<string>> schemGUIDsKVP in SchemGUIDsNeeded)
            {
                for (int i = 0; i < schemGUIDsKVP.Value.Count; i++)
                {
                    if ((i == schemGUIDsKVP.Value.Count - 1) || (((i % 999) == 0) && (i != 0)))
                    {
                        builder.Append("'" + schemGUIDsKVP.Value[i] + "'");
                        InListValuesByClassID[schemGUIDsKVP.Key].Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else
                    {
                        builder.Append("'" + schemGUIDsKVP.Value[i] + "'" + ",");
                    }
                }
            }

            List<string> guidsChecked = new List<string>();
            foreach (KeyValuePair<int, List<string>> InListValuesKVP in InListValuesByClassID)
            {
                IFeatureClass schemFeatClass = CADOPS.GetSchemFeatureClass("", InListValuesKVP.Key);
                bool hasStatusField = (schemFeatClass.FindField("STATUS") >= 0);
                int uguidFieldIndex = schemFeatClass.FindField("UGUID");
                foreach (string guidInList in InListValuesKVP.Value)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.SubFields = "SHAPE,UGUID";
                    if (hasStatusField) { qf.WhereClause = "DIAGRAMCLASSID = " + GetSchemDiagramClassID() + " AND (ISDISPLAYED = -1 OR (ISDISPLAYED = 0 AND STATUS = 0)) AND UGUID in (" + guidInList + ")"; }
                    else { qf.WhereClause = "DIAGRAMCLASSID = " + GetSchemDiagramClassID() + " AND ISDISPLAYED = -1 AND UGUID in (" + guidInList + ")"; }
                    IFeatureCursor featCursor = schemFeatClass.Search(qf, true);
                    IFeature feat = null;
                    while ((feat = featCursor.NextFeature()) != null)
                    {
                        string uguid = feat.get_Value(uguidFieldIndex).ToString();
                        if (guidsChecked.Contains(uguid)) { continue; }
                        else { guidsChecked.Add(uguid); }
                        guidsNeeded[InListValuesKVP.Key].Remove(uguid);
                        //if (!uguid.Contains("{")) { uguid = "{" + uguid; }
                        //if (!uguid.Contains("}")) { uguid = uguid + "}"; }
                        //uguid = uguid.ToUpper();
                        IPointCollection linePointCollection = feat.Shape as IPointCollection;

                        bool reverse = false;
                        //go through the vertices
                        for (int i = 0; i < linePointCollection.PointCount; i++)
                        {
                            IPoint p = linePointCollection.get_Point(i);
                            int x = Utilities.ConvertXY(p.X);
                            int y = Utilities.ConvertXY(p.Y);
                            DataRow row = PathDT.NewRow();
                            string[] uidAndNoKey = Regex.Split(SchemGUIDToUIDMap[uguid], ",");

                            if (i == 0)
                            {
                                //check if we need to revers this line segements order
                                try
                                {
                                    string XN2 = NFPOSToXN2[uidAndNoKey[1]];
                                    if (XN2 != x.ToString()) { reverse = true; }
                                }
                                catch { }
                            }

                            int order = i;
                            if (reverse) { order = linePointCollection.PointCount - i - 1; }
                            
                            //Adjust to start from 1 as DMS requires
                            order++;

                            row["LINE_GUID"] = uidAndNoKey[0];
                            row["XDIFF"] = x;
                            row["YDIFF"] = y;
                            row["ORDER_NUM"] = order;
                            row["SUBSTATION_OBJ_IDC"] = "N";
                            row["TYPE"] = "S";
                            PathDT.Rows.Add(row);
                        }
                    }
                }
            }

            //Process back through our rows and determine which of our guids that we still need are in the proposed install state.  This will
            //tell us whether we need to remove them from the errors list and add them to the warnings list instead
            foreach (DataRow row in LineDT.Rows)
            {
                string line_uid = row["FPOS"].ToString();
                string line_guid = row["GUID"].ToString();
                int classID = -1;
                foreach (KeyValuePair<int, List<string>> uidList in PGE.Interface.Integration.DMS.Processors.Path.ClassIDToUIDMap)
                {
                    if (uidList.Value.Contains(line_uid))
                    {
                        classID = uidList.Key;
                    }
                }

                if (classID == -1) { continue; }

                try
                {
                    if (guidsNeeded[classID].Contains(line_guid))
                    {
                        //This guid is still needed.  Check if it should be a warning
                        string state =  row["STATE"].ToString();
                        if (state == "2")
                        {
                            //proposed install.
                            guidsNeeded[classID].Remove(line_guid);
                            try
                            {
                                invalidProposedInstallSchemFeatures[classID].Add(line_guid);
                            }
                            catch (Exception ex)
                            {
                                invalidProposedInstallSchemFeatures.Add(classID, new List<string>());
                                invalidProposedInstallSchemFeatures[classID].Add(line_guid);
                            }
                        }
                    }
                }
                catch
                {

                }
            }

            return guidsNeeded;

        }

        private void AddAttributesForClass(ElectricExportScope scope, ICircuit feeder, int oid, JunctionFeatureInfo featInfo)
        {
            IFeatureClass featureClass = scope.GdbAccess.ObjectClassCache[featInfo.ObjectClassID] as IFeatureClass;
            IFeature gdbFeature = featureClass.GetFeature(oid);

            scope.GdbAccess.Hydrate(featInfo as FeatureInfo, gdbFeature);

            featInfo.Junction.Point = scope.GdbAccess.GetPoint2D(gdbFeature.Shape as IPoint);

            if (featInfo.Junction is IElectricElement)
            {
                IElectricElement junction = (IElectricElement)featInfo.Junction;
                this.AddCircuitID(scope, junction, featInfo, feeder as FeederInfo);
            }
        }

        private void AddCircuitID(ElectricExportScope scope, IElectricElement element, ObjectInfo feature, FeederInfo feeder)
        {
            object obj2;
            string[] feederIDModelNames = {"FEEDERID", "FEEDERID2"};
            foreach (string str in feederIDModelNames)
            {
                if (TryGetValueFromFieldModelName(scope.GdbAccess.GdbConfig, str, feature, out obj2))
                {
                    if (obj2 != null && !string.IsNullOrEmpty(obj2.ToString()))
                    {
                        ElectricJunction ej = element as ElectricJunction;
                        ej.Feeder = feeder;
                        element.CircuitName = obj2.ToString();
                        return;
                    }
                }
            }
        }

        public static IFeatureClass GetFeatureClass(string FCName)
        {
            try
            {
                if (EDSecondaryObjectClasses.ContainsKey(FCName)) { return EDSecondaryObjectClasses[FCName]; }

                IFeatureWorkspace featWorkspace = _EDWorkspace as IFeatureWorkspace;

                IFeatureClass featClass = featWorkspace.OpenFeatureClass(FCName);
                EDSecondaryObjectClasses.Add(FCName, featClass);
                return featClass;
            }
            catch (Exception ex)
            {
                _log4.Error("Error getting ED Table: " + FCName, ex);
                throw ex;
            }
        }


        //Changes for ENOS to SAP migration - DMS .. Start
        public static ITable GetTable(string argTableName)
        {
            try
            {
                IFeatureWorkspace featWorkspace = _EDWorkspace as IFeatureWorkspace;
                ITable objTable = featWorkspace.OpenTable(argTableName);
                return objTable;
            }
            catch (Exception ex)
            {
                _log4.Error("Error getting ED Table: " + argTableName, ex);
                throw ex;
            }
        }
        //Changes for ENOS to SAP migration - DMS .. End

        public static IFeatureClass GetSchemFeatureClass(string schemFCName, int EDFCID)
        {
            try
            {
                if (_schemTables == null)
                {
                    _schemTables = new Dictionary<string, IFeatureClass>();
                    IWorkspace schemWorkspace = GetWorkSchemSpace() as IWorkspace;

                    IFeatureWorkspace featWorkspace = _schemWorkspace as IFeatureWorkspace;

                    foreach (KeyValuePair<int, string> kvp in Configuration.EDFcidToSchemFcidMap)
                    {
                        IFeatureClass featClass = featWorkspace.OpenFeatureClass(kvp.Value);
                        if (featClass == null)
                        {
                            throw new Exception("Error getting " + kvp.Value + " from schematics database");
                        }
                        _schemTables.Add(kvp.Value, featClass);
                    }
                    /*
                    for (int i = 0; i < featureClassContainer.ClassCount; i++)
                    {
                        foreach (KeyValuePair<int, int> classIDKVP in Configuration.EDFcidToSchemFcidMap)
                        {
                            if (_schemTables.ContainsKey(classIDKVP.Value))
                            {
                                IFeatureClass featClass = featureClassContainer.get_ClassByID(classIDKVP.Value);
                                if (featClass != null)
                                {
                                   
                                }
                            }
                        }
                    }
                    */
                }

                if (string.IsNullOrEmpty(schemFCName))
                {
                    schemFCName = Configuration.EDFcidToSchemFcidMap[EDFCID];
                    if (string.IsNullOrEmpty(schemFCName))
                    {
                        throw new Exception("Error getting feature class ID: " + EDFCID + " from schematics database");
                    }
                }
                return _schemTables[schemFCName];
                
            }
            catch (Exception ex)
            {
                _log4.Error("Error getting Schematics Table: " + schemFCName, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Retrieves the Database workspace by reading the database credentials from the config file.
        /// </summary>
        /// <returns>Returns the reference to the Database</returns>
        private static IWorkspace GetWorkSchemSpace()
        {
            try
            {
                if (_schemWorkspace == null)
                {
                    IPropertySet propertySet = new PropertySetClass();
                    // M4JF EDGISREARCH 388
                    // m4jf edgisrearch 919
                    //string[] commaSepConnectionList = Configuration.getCommaSeparatedList("SCHEMConnection", new string[3]);
                    //string dbconn = "sde:oracle11g:" + commaSepConnectionList[0];
                    //_log4.Debug("Using Schematics dbconn: " + dbconn);
                    //propertySet.SetProperty("instance", dbconn);
                    //propertySet.SetProperty("User", commaSepConnectionList[1]);
                    //propertySet.SetProperty("Password", commaSepConnectionList[2]);
                    //propertySet.SetProperty("version", "SDE.DEFAULT");
                    string[] userInst = System.Configuration.ConfigurationManager.AppSettings["EDSCHM_ConnectionStr"].Split('@');
                    string password = ReadEncryption.GetPassword(System.Configuration.ConfigurationManager.AppSettings["EDSCHM_ConnectionStr"].ToUpper());
                    string dbconn = "sde:oracle11g:" + userInst[1];
                    _log4.Debug("Using Schematics dbconn: " + dbconn);
                    propertySet.SetProperty("instance", dbconn);
                    propertySet.SetProperty("User", userInst[0]);
                    propertySet.SetProperty("Password", password);
                    propertySet.SetProperty("version", "SDE.DEFAULT");

                    IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                    _schemWorkspace = workspaceFactory.Open(propertySet, 0);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workspaceFactory);
                }

                return _schemWorkspace;
            }
            catch (Exception ex)
            {
                _log4.Error("Error getting Schematics SDE workspace.", ex);
                return null;
            }
        }

        private static bool TryGetValueFromFieldModelName(GdbConfiguration gdbConfig, string fieldModelName, ObjectInfo feature, out object fieldValue)
        {
            fieldValue = null;
            if (gdbConfig.Classes.ContainsKey(feature.ObjectClassID))
            {
                ClassConfiguration configuration = gdbConfig.Classes[feature.ObjectClassID];
                foreach (FieldConfiguration configuration2 in configuration.FieldsConfig.Values)
                {
                    if (configuration2.FieldModelNames.Contains(fieldModelName))
                    {
                        object obj2 = feature.Fields[configuration2.FieldName].FieldValue;
                        if ((obj2 == null) || DBNull.Value.Equals(obj2))
                        {
                            break;
                        }
                        fieldValue = obj2;
                        return true;
                    }
                }
            }
            return false;
        }

        private JunctionInfo GetOrCreateMissingJunction(ElectricExportScope scope, ICircuit feeder, ElementInfo edge, int junctionEID)
        {
            int num;
            int num2;
            int num3;
            scope.GdbAccess.QueryIDs(junctionEID, esriElementType.esriETJunction, out num, out num2, out num3);
            //if (feeder.JunctionElements.ContainsKey(junctionEID))
            //{
            //    return feeder.JunctionElements[junctionEID];
            //}
            JunctionInfo junction = new ElectricJunction(num, num2, junctionEID, scope.GdbAccess.GetWeight(junctionEID, esriElementType.esriETJunction));
            junction.Origin = ElementOrigin.AdjacentToTracedElement;
            junction.PreviousEID = edge.EID;
            //feeder.AddJunction(junction);
            //if (scope.GdbAccess.GdbConfig.Classes.ContainsKey(num))
            //{
            //    EdgeFeatureInfo edgeFeature = null;
            //    if (feeder.EdgeFeatures.ContainsKey(edge.ObjectKey))
            //    {
            //        edgeFeature = feeder.EdgeFeatures[edge.ObjectKey];
            //    }
            //    if ((BitHelper.LineHasEnoughWires((ElectricEdge)edge, (ElectricJunction)junction) && !feeder.JunctionFeatures.ContainsKey(junction.ObjectKey)) && (((SetOfPhases)((ElectricEdge)edge).EnergizedPhases) == SetOfPhases.None))
            //    {
            //        junction.Origin = ElementOrigin.Trace;
            //        feeder.AddJunctionFeature(junction);
            //        return junction;
            //    }
            //    new JunctionNotTracedException(edgeFeature, edge, junction).Handle(ref this._cancel);
            //}
            return junction;
        }



        /// <summary>
        /// Set the status of the circuit export. Assumes one circuit exported at a time.
        /// </summary>
        /// <param name="status">0 - success, 1 - error, 2 - missing</param>
        /// <param name="set">The staging schema dataset with the EXPORTED table</param>
        private void SetStatus(int status, DataSet set)
        {
            //there should only be one row
            DataRow row = set.Tables["DMSSTAGING.EXPORTED"].Rows[0];
            row["STATUS"] = status;
        }
        /// <summary>
        /// Set an error message for the circuit being exported. Assumes one circuit exported at a time.
        /// This method also deletes all of the circuit data.
        /// </summary>
        /// <param name="error">The error message</param>
        /// <param name="set">The staging schema dataset with the EXPORTED table</param>
        /// <returns>The new dataset with the circuit data removed.</returns>
        private DataSet SetError(string error, DataSet set)
        {
            //there should only be one row
            DataRow row = set.Tables["DMSSTAGING.EXPORTED"].Rows[0];
            row["MESSAGE"] = error;
            //dump all the data by creating a new empty set
            DataSet newset = Utilities.BuildDataSet();
            //copy the exported table so we can log the error
            newset.Tables["DMSSTAGING.EXPORTED"].Merge(set.Tables["DMSSTAGING.EXPORTED"]);
            newset.Tables["DMSSTAGING.PGE_DMS_FEATURE_ERRORS"].Merge(set.Tables["DMSSTAGING.PGE_DMS_FEATURE_ERRORS"]);
            newset.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"].Merge(set.Tables["DMSSTAGING.PGE_DMS_SCHEMATICS_ERRORS"]);
            return newset;
        }
    }

    struct SchemPoint
    {
        public int x;
        public int y;
    }
}
