using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using ADODB;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ScriptEngine;
using Miner.Interop;
using Miner.Interop.Process;
using PGE.Common.Delivery.Framework;


namespace PGE.BatchApplication.ArcFM_PerfQA_Tools
{
    [Export(typeof(IScriptCommand))]
    public class Commands : IScriptCommand
    {
        private IMMStoredDisplayManager _SDMGR;

        private CommandParser _parser;

        private Stopwatch SW1;

        private ISelection pSelection;

        public IFeatureClass FeatureClass
        {
            get;
            set;
        }

        public ESRI.ScriptEngine.Logger Logger
        {
            get;
            set;
        }

        public IWorkspace Workspace
        {
            get;
            set;
        }

        public IWorkspaceEdit Workspaceedit
        {
            get;
            set;
        }

        //For Summary Log- Shashwat Nigam
        public static ArrayList pAList_Log = new ArrayList();
        public static int iCount_Log = 0;
        public static string sCommand_Log = string.Empty, sNFR_Log = string.Empty;

        public Commands()
        {
            this._parser = new CommandParser()
            {
                CommentIdentifier = "//"
            };
            this.Logger = new ESRI.ScriptEngine.Logger();
            this.SW1 = new Stopwatch();
        }

        private void AddSelection(IMxDocument pMXDoc, IDictionary<int, SelectedObjects> dicClasses)
        {
            SelectedObjects selectedObject;
            IMap focusMap = pMXDoc.FocusMap;
            UID uIDClass = new UID()
            {
                Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}"
            };
            IEnumLayer layers = focusMap.Layers[uIDClass, true];

            for (ILayer i = layers.Next(); i != null; i = layers.Next())
            {
                if (i is IFeatureLayer && i.Valid)
                {
                    IFeatureLayer selectionSet = (IFeatureLayer)i;
                    if (dicClasses.TryGetValue(selectionSet.FeatureClass.FeatureClassID, out selectedObject))
                    {
                        ((IFeatureSelection)selectionSet).SelectionSet = selectedObject.SelectionSet;
                    }
                }
            }
        }

        public bool ARCFM_AUTO_ASSIGN()
        {
            this.SW1.Reset();
            this.SW1.Start();
            this.SW1.Stop();
            this.Logger.WriteLine(string.Concat("ARCFM_AUTO_ASSIGN:", this.SW1.ElapsedMilliseconds), false, null);
            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            return true;
        }

        public bool ARCFM_ZOOMTO(string sScale, string sX, string sY)
        {
            bool flag;
            IAppROT variable = (AppROT)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("FABC30FB-D273-11D2-9F36-00C04F6BC61A")));
            try
            {
                IMMMapUtilities _mmMapUtilsClass = new mmMapUtils();
                Type typeFromProgID = Type.GetTypeFromProgID("esriFramework.AppRef");
                IMxDocument document = (IMxDocument)(Activator.CreateInstance(typeFromProgID) as IApplication).Document;
                IPoint pointClass = new Point()
                {
                    SpatialReference = document.FocusMap.SpatialReference,
                    X = Convert.ToDouble(sX),
                    Y = Convert.ToDouble(sY)
                };
                double num = Convert.ToDouble(sScale);
                this.SW1.Reset();
                this.SW1.Start();
                _mmMapUtilsClass.ZoomTo(pointClass, document.ActiveView, num);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("ARCFM_ZOOMTO:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        private bool ArcFMGasTrace(string sTraceType)
        {
            int i;
            int num;
            bool flag;
            INetworkAnalysisExt variable = null;
            INetworkAnalysisExtFlags variable1 = null;
            INetworkAnalysisExtResults variable2 = null;
            ITraceTask traceTaskByName = null;
            IGeometricNetwork currentNetwork = null;
            IEnumNetEID resultJunctions = null;
            IEnumNetEID resultEdges = null;
            IMMTraceUIUtilities variable3 = null;
            ArrayList arrayLists = null;
            ArrayList arrayLists1 = null;
            try
            {
                IApplication variable4 = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = (IMxDocument)variable4.Document;
                variable = (INetworkAnalysisExt)variable4.FindExtensionByName("Utility Network Analyst");
                if (variable == null)
                {
                    this.Logger.WriteLine("Missing Utility Network AnalysisExt", false, null);
                    flag = false;
                }
                else
                {
                    variable1 = (INetworkAnalysisExtFlags)variable;
                    variable2 = (INetworkAnalysisExtResults)variable;
                    currentNetwork = variable.CurrentNetwork;
                    variable3 = (MMTraceUIUtilities)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0306FF9C-C236-491D-9D1E-3D8514C4C59B")));
                    if (variable3 != null)
                    {
                        traceTaskByName = variable3.GetTraceTaskByName(variable, "ArcFM Gas Valve Isolation");
                        if (traceTaskByName != null)
                        {
                            ((ITraceTasks)variable).CurrentTask = traceTaskByName;
                            (new ArrayList()).Clear();
                            INetElements network = (INetElements)currentNetwork.Network;
                            arrayLists = new ArrayList();
                            arrayLists.Clear();
                            arrayLists1 = new ArrayList();
                            arrayLists1.Clear();
                            ITraceTaskResults variable5 = (ITraceTaskResults)traceTaskByName;
                            resultJunctions = variable5.ResultJunctions;
                            resultJunctions.Reset();
                            for (i = 0; i < resultJunctions.Count; i++)
                            {
                                num = resultJunctions.Next();
                                if (!arrayLists1.Contains(num))
                                {
                                    arrayLists1.Add(num);
                                }
                            }
                            resultEdges = variable5.ResultEdges;
                            resultEdges.Reset();
                            for (i = 0; i < resultEdges.Count; i++)
                            {
                                num = resultEdges.Next();
                                if (!arrayLists.Contains(num))
                                {
                                    arrayLists.Add(num);
                                }
                            }
                        }
                    }
                    flag = true;
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Error in ArcFMGasTrace ", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }

        private bool ArcFMTrace(string lyrname, string sOID, string sSelect, int iTraceType)
        {
            IMMTracedElements mMTracedElement=null;
            IMMTracedElements mMTracedElement1=null;
            IMMTracedElement i;
            IMMEnumFeedPath mMEnumFeedPath;
            SelectedObjects selectedObject;
            esriElementType _esriElementType;
            int fromJunctionEID;
            IMMEnumTraceStopper mMEnumTraceStopper;
            IMMEnumTraceStopper mMEnumTraceStopper1;
            Exception exception;
            bool flag;
            int[] numArray = new int[0];
            int[] numArray1 = new int[0];
            IMMCurrentStatus mMCurrentStatu = null;
            int selectionCount = 0;
            try
            {
                IMMElectricTracingEx mMFeederTracerClass = new MMFeederTracerClass();
                IMMElectricTraceSettings mMElectricTraceSettingsClass = new MMElectricTraceSettingsClass()
                {
                    RespectConductorPhasing = true,
                    RespectEnabledField = false
                };
                this.SW1.Reset();
                this.SW1.Start();
                try
                {
                    IApplication m_pApp = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                    IMxDocument document = m_pApp.Document as IMxDocument;
                    IMap pMap = ((IMxDocument)m_pApp.Document).FocusMap;
                    IEnumLayer layers = pMap.get_Layers() as IEnumLayer;

                    for (ILayer layer = layers.Next(); layer != null; layer = layers.Next())
                    {

                        if (layer.Name == lyrname)
                        {

                            if (layer is IFeatureLayer && layer.Valid)
                            {
                                this.FeatureClass = ((IFeatureLayer)layer).FeatureClass;
                                break;
                            }
                        }
                    }

                    if (this.FeatureClass is INetworkClass)
                    {
                        //Type typeFromProgID = Type.GetTypeFromProgID("esriFramework.AppRef");
                        //IMxDocument document = (IMxDocument)(Activator.CreateInstance(typeFromProgID) as IApplication).Document;
                        ISelection featureSelection = pMap.FeatureSelection;
                        IGeometricNetwork geometricNetwork = ((INetworkClass)this.FeatureClass).GeometricNetwork;
                        IDictionary<int, SelectedObjects> classIDs = this.GetClassIDs(geometricNetwork);

                        IFeature feature = this.FeatureClass.GetFeature(Convert.ToInt32(sOID));

                        if (!(feature is IJunctionFeature))
                        {
                            _esriElementType = esriElementType.esriETEdge;
                            fromJunctionEID = ((IEdgeFeature)feature).FromJunctionEID;
                        }
                        else
                        {
                            _esriElementType = esriElementType.esriETJunction;
                            fromJunctionEID = ((ISimpleJunctionFeature)feature).EID;
                        }


                        if (classIDs.TryGetValue(feature.Class.ObjectClassID, out selectedObject))
                        {
                            selectedObject.SelectionSet.Add(feature.OID);
                        }
                        if (iTraceType == 1)
                        {
                            mMFeederTracerClass.TraceDownstream(geometricNetwork, mMElectricTraceSettingsClass, mMCurrentStatu, fromJunctionEID, _esriElementType, SetOfPhases.abc, mmDirectionInfo.establishBySourceSearch, 0, esriElementType.esriETEdge, numArray, numArray1, false, out mMTracedElement, out mMTracedElement1);
                            if (sSelect.ToUpper() == "TRUE")
                            {
                                for (i = mMTracedElement1.Next(); i != null; i = mMTracedElement1.Next())
                                {
                                    if (classIDs.TryGetValue(i.FCID, out selectedObject))
                                    {
                                        selectedObject.SelectionSet.Add(i.OID);
                                    }
                                }
                                for (i = mMTracedElement.Next(); i != null; i = mMTracedElement.Next())
                                {
                                    if (classIDs.TryGetValue(i.FCID, out selectedObject))
                                    {
                                        selectedObject.SelectionSet.Add(i.OID);
                                    }
                                }
                            }
                        }
                        else if (iTraceType != 2)
                        {
                            MMFeederTracerClass mMFeederTracerClass1 = new MMFeederTracerClass();
                            IMMNetworkAnalysisExtForFramework mMNetworkAnalysisExtForFrameworkClass = new MMNetworkAnalysisExtForFrameworkClass();
                        }
                        else
                        {

                            mMFeederTracerClass.FindFeedPaths(geometricNetwork, mMElectricTraceSettingsClass, mMCurrentStatu, fromJunctionEID, _esriElementType, SetOfPhases.abc, numArray, numArray1, out mMEnumFeedPath, out mMEnumTraceStopper, out mMEnumTraceStopper1);
                            if (sSelect.ToUpper() == "TRUE")
                            {
                                IMMFeedPathEx mMFeedPathEx = mMEnumFeedPath.Next();
                                while (mMFeedPathEx != null)
                                {
                                    IMMEnumPathElement pathElementEnum = mMFeedPathEx.GetPathElementEnum();
                                    if (pathElementEnum == null)
                                    {
                                        this.Logger.WriteLine("No Soureces", false, null);
                                    }
                                    else
                                    {
                                        for (IMMPathElementEx j = pathElementEnum.Next(); j != null; j = pathElementEnum.Next())
                                        {
                                            if (classIDs.TryGetValue(j.FCID, out selectedObject))
                                            {
                                                selectedObject.SelectionSet.Add(j.OID);
                                            }
                                        }
                                        mMFeedPathEx = mMEnumFeedPath.Next();
                                    }
                                }
                            }

                        }

                        if (classIDs.Count > 0)
                        {
                            this.AddSelection(document, classIDs);
                            selectionCount = document.FocusMap.SelectionCount;
                        }
                    }
                    document.ActiveView.Refresh();
                    this.SW1.Stop();
                    ESRI.ScriptEngine.Logger logger = this.Logger;
                    logger.WriteLine("Trace Selection Count is " + selectionCount.ToString());
                    object[] str = new object[] { "TRACE_EXECUTION_TIME: ", this.SW1.ElapsedMilliseconds };//TraceDownStream Execution Time
                    logger.WriteLine(string.Concat(str), false, null);
                    PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                    flag = true;
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    this.Logger.WriteLine(string.Concat("TraceDownStream Failed: ", exception.Message), false, null);
                    flag = false;
                }
            }
            catch (Exception exception2)
            {
                exception = exception2;
                this.Logger.WriteLine(string.Concat("TraceDownStream Failed: ", exception.Message), false, null);
                flag = false;
            }
            finally
            {
                if (mMTracedElement != null) Marshal.FinalReleaseComObject(mMTracedElement);
                if (mMTracedElement1 != null) Marshal.FinalReleaseComObject(mMTracedElement1);
            }
            return flag;
        }


        private bool ArcFMTrace1(string sOID, string sSelect, int iTraceType)
        {
            IMMTracedElements mMTracedElement;
            IMMTracedElements mMTracedElement1;
            IMMTracedElement i;
            IMMEnumFeedPath mMEnumFeedPath;
            SelectedObjects selectedObject;
            esriElementType _esriElementType;
            int fromJunctionEID;
            IMMEnumTraceStopper mMEnumTraceStopper;
            IMMEnumTraceStopper mMEnumTraceStopper1;
            Exception exception;
            bool flag;
            int[] numArray = new int[0];
            int[] numArray1 = new int[0];
            IMMCurrentStatus mMCurrentStatu = null;
            int selectionCount = 0;

            IMMElectricTracingEx mMFeederTracerClass = new MMFeederTracer() as IMMElectricTracingEx;
            IMMElectricTraceSettings mMElectricTraceSettingsClass = new MMElectricTraceSettings()
            {
                RespectConductorPhasing = true,
                RespectEnabledField = false
            };
            try
            {
                
                this.SW1.Reset();
                this.SW1.Start();
                try
                {
                    if (this.FeatureClass is INetworkClass)
                    {
                        Type typeFromProgID = Type.GetTypeFromProgID("esriFramework.AppRef");
                        IMxDocument document = (IMxDocument)(Activator.CreateInstance(typeFromProgID) as IApplication).Document;
                        ISelection featureSelection = document.FocusMap.FeatureSelection;
                        IGeometricNetwork geometricNetwork = ((INetworkClass)this.FeatureClass).GeometricNetwork;
                        IDictionary<int, SelectedObjects> classIDs = this.GetClassIDs(geometricNetwork);
                        IFeature feature = this.FeatureClass.GetFeature(Convert.ToInt32(sOID));
                        if (!(feature is IJunctionFeature))
                        {
                            _esriElementType = esriElementType.esriETEdge;
                            fromJunctionEID = ((IEdgeFeature)feature).FromJunctionEID;
                        }
                        else
                        {
                            _esriElementType = esriElementType.esriETJunction;
                            fromJunctionEID = ((ISimpleJunctionFeature)feature).EID;
                        }
                        if (classIDs.TryGetValue(feature.Class.ObjectClassID, out selectedObject))
                        {
                            selectedObject.SelectionSet.Add(feature.OID);
                        }
                        if (iTraceType == 1)
                        {
                            mMFeederTracerClass.TraceDownstream(geometricNetwork, mMElectricTraceSettingsClass, mMCurrentStatu, fromJunctionEID, _esriElementType, SetOfPhases.abc, mmDirectionInfo.establishBySourceSearch, 0, esriElementType.esriETEdge, numArray, numArray1, false, out mMTracedElement, out mMTracedElement1);
                            if (sSelect.ToUpper() == "TRUE")
                            {
                                for (i = mMTracedElement1.Next(); i != null; i = mMTracedElement1.Next())
                                {
                                    if (classIDs.TryGetValue(i.FCID, out selectedObject))
                                    {
                                        selectedObject.SelectionSet.Add(i.OID);
                                    }
                                }
                                for (i = mMTracedElement.Next(); i != null; i = mMTracedElement.Next())
                                {
                                    if (classIDs.TryGetValue(i.FCID, out selectedObject))
                                    {
                                        selectedObject.SelectionSet.Add(i.OID);
                                    }
                                }
                            }
                        }
                        else if (iTraceType != 2)
                        {
                            MMFeederTracer mMFeederTracerClass1 = new MMFeederTracer() as MMFeederTracer;
                            IMMNetworkAnalysisExtForFramework mMNetworkAnalysisExtForFrameworkClass = new MMNetworkAnalysisExtForFramework();
                        }
                        else
                        {
                            mMFeederTracerClass.FindFeedPaths(geometricNetwork, mMElectricTraceSettingsClass, mMCurrentStatu, fromJunctionEID, _esriElementType, SetOfPhases.abc, numArray, numArray1, out mMEnumFeedPath, out mMEnumTraceStopper, out mMEnumTraceStopper1);
                            if (sSelect.ToUpper() == "TRUE")
                            {
                                IMMFeedPathEx mMFeedPathEx = mMEnumFeedPath.Next();
                                while (mMFeedPathEx != null)
                                {
                                    IMMEnumPathElement pathElementEnum = mMFeedPathEx.GetPathElementEnum();
                                    if (pathElementEnum == null)
                                    {
                                        this.Logger.WriteLine("No Soureces", false, null);
                                    }
                                    else
                                    {
                                        for (IMMPathElementEx j = pathElementEnum.Next(); j != null; j = pathElementEnum.Next())
                                        {
                                            if (classIDs.TryGetValue(j.FCID, out selectedObject))
                                            {
                                                selectedObject.SelectionSet.Add(j.OID);
                                            }
                                        }
                                        mMFeedPathEx = mMEnumFeedPath.Next();
                                    }
                                }
                            }
                        }
                        this.AddSelection(document, classIDs);
                        selectionCount = document.FocusMap.SelectionCount;
                    }
                    this.SW1.Stop();
                    ESRI.ScriptEngine.Logger logger = this.Logger;
                    logger.WriteLine("Trace Selection Count is " + selectionCount.ToString());
                    object[] str = new object[] { "TRACE_EXECUTION_TIME: ", this.SW1.ElapsedMilliseconds };//TraceDownStream Execution Time
                    logger.WriteLine(string.Concat(str), false, null);
                    PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                    flag = true;
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    this.Logger.WriteLine(string.Concat("TraceDownStream Failed: ", exception.Message), false, null);
                    flag = false;
                }
            }
            catch (Exception exception2)
            {
                exception = exception2;
                this.Logger.WriteLine(string.Concat("TraceDownStream Failed: ", exception.Message), false, null);
                flag = false;
            }
            return flag;
        }

        public bool CLEARSELECTION()
        {
            this.SW1.Reset();
            this.SW1.Start();
            object obj = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef"));
            ((IMxDocument)(obj as IApplication).Document).FocusMap.ClearSelection();
            this.SW1.Stop();
            this.Logger.WriteLine(string.Concat("SELECTION_CLEAR_TIME:", this.SW1.ElapsedMilliseconds), false, null);
            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            return true;
        }

        public bool CLOSE_ARCFM_SESSION(string sODBC, string sSave)
        {
            IAppROT variable = (AppROT)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("FABC30FB-D273-11D2-9F36-00C04F6BC61A")));
            this.SW1.Reset();
            this.SW1.Start();
            IApplication variable1 = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            UID uIDClass = new UID()
            {
                Value = "esriEditor.Editor"
            };
            IEditor variable2 = variable1.FindExtensionByCLSID(uIDClass) as IEditor;
            if (!(sSave.ToUpper() == "TRUE"))
            {
                variable2.StopEditing(false);
            }
            else
            {
                variable2.StopEditing(true);
            }
            this.SW1.Stop();
            this.Logger.WriteLine(string.Concat("CLOSE_ARCFM_SESSION:", this.SW1.ElapsedMilliseconds), false, null);
            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            return true;
        }

        private bool CloseSession()
        {
            bool flag;
            try
            {
                IWorkspace loginWorkspace = (new MMLoginUtils()).LoginWorkspace;
                Type typeFromProgID = Type.GetTypeFromProgID("esriFramework.AppRef");
                IMxDocument document = (IMxDocument)(Activator.CreateInstance(typeFromProgID) as IApplication).Document;
                IVersion version = (IVersion)loginWorkspace;
                IVersion parent = (IVersion)version.VersionInfo.Parent;
                IChangeDatabaseVersion variable = (ChangeDatabaseVersion)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0038A3AF-0FCB-487A-B3EE-65C0E80D13F0")));
                variable.Execute(version, parent, (IBasicMap)document.FocusMap);
                flag = true;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }

        public bool CREATE_ARCFM_SESSION(string sName, string sConnStr)
        {
            Connection connection;
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                IWorkspace loginWorkspace = (new MMLoginUtils()).LoginWorkspace;
                IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = (IMxDocument)variable.Document;
                IMMPxApplication application = ((IMMPxIntegrationCache)variable.FindExtensionByName("Session Manager Integration Extension")).Application;
                IMMPxLogin login = application.Login;
                IMMSessionManager2 variable1 = (IMMSessionManager2)application.FindPxExtensionByName("MMSessionManager");
                if (login.Connection != null)
                {
                    connection = login.Connection;
                }
                else
                {
                    connection = (Connection)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00000514-0000-0010-8000-00AA006D2EA4")));
                    login.ConnectionString = sConnStr;
                    connection.Open("", "", "", -1);
                }
                application.Startup(login);
                IMMSessionVersion variable2 = (IMMSessionVersion)variable1.CreateSession();
                IVersion version = (IVersion)loginWorkspace;
                IVersion version1 = version.CreateVersion(variable2.get_Name());

                //IChangeDatabaseVersion variable3 = (ChangeDatabaseVersion)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0038A3AF-0FCB-487A-B3EE-65C0E80D13F0")));//Commented By Shashwat as it is not being used anytime
                this.SwizzleDatasets(variable, (IFeatureWorkspace)version, (IFeatureWorkspace)version1);
                this.Workspace = (IWorkspace)version1;
                //ScriptEngine.BroadcastProperty("Workspace", this.Workspace, null);
                UID uIDClass = new UID()
                {
                    Value = "esriEditor.Editor"
                };
                (variable.FindExtensionByCLSID(uIDClass) as IEditor).StartEditing(this.Workspace);
                document.ActiveView.Refresh();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("CREATE_ARCFM_SESSION:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("CREATE_ARCFM_SESSION Failed: ", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }

        public void CreateServicePoint(double X, double Y)
        {
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            Dictionary<int, object> fieldMapping_Create = new Dictionary<int, object>();
            UID featureUID = new UID();
            featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer featureLayers = ((IMxDocument)variable.Document).FocusMap.get_Layers(featureUID, true);
            int iCount_ClassGot = 0;
            IFeatureClass pFClass_Transformer = null;
            IObjectClass pOClass_Related = null;
            ILayer pLayer = null;
            try
            {
                //Finding the Feature Class for the NewFeature and Where to locate Feature
                while ((pLayer = featureLayers.Next()) != null)
                {
                    if (((IDataset)((IFeatureLayer)pLayer).FeatureClass).Name.ToUpper() == "EDGIS.ServiceLocation".ToUpper())
                    { pFClass_Transformer = ((IFeatureLayer)pLayer).FeatureClass; ++iCount_ClassGot; }

                    else continue;
                    if (iCount_ClassGot == 1) break;
                }

                this.SW1.Restart();
                this.SW1.Start();

                IFeatureWorkspace fworkspace = ((IDataset)pFClass_Transformer).Workspace as IFeatureWorkspace;

                IWorkspaceEdit we = fworkspace as IWorkspaceEdit;

                we.StartEditOperation();

                pOClass_Related = (IObjectClass)fworkspace.OpenTable("EDGIS.SERVICEPOINT");

                fieldMapping_Create.Clear();
                //iSUBTYPECD = new int[] { 1 };//{ 1, 2, 3 };
                //iSTATUS = new short[] { 0, 2, 3, 5, 30 };
                //Creating Transformerunit Feature
                fieldMapping_Create.Add(pOClass_Related.FindField("SubtypeCD"), 1);
                //fieldMapping_Create.Add(pOClass_Related.FindField("STATUS"), 5);

                IObject pObject_Related = null; //CreateObject(variable, pOClass_Related, fieldMapping_Create);                
                pObject_Related = ((ITable)pOClass_Related).CreateRow() as IObject;

                //Setting the Fields through Field Mapping dictionary
                foreach (KeyValuePair<int, object> kvp in fieldMapping_Create)
                {
                    if (kvp.Value != null)
                        pObject_Related.set_Value(kvp.Key, kvp.Value);
                }

                pObject_Related.Store();

                fieldMapping_Create.Clear();

                //Creating Feature


                //Settings Manadatory Not Null Fields
                Random pRandom = new Random();

                int[] iSUBTYPECD = new int[] { 1 },//{ 1, 2, 3, 4, 7 },
                    iNUMBEROFPHASES = new int[] { 1, 2, 3 },
                    iOPERATINGVOLTAGE = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                    iUNITCOUNT = new int[] { 0, 1, 2, 3, 4, 5, 6 },
                    iLOWSIDEVOLTAGE = new int[] { 25, 30, 27, 29, 99, 24, 22, 23, 20, 26, 21, 28, 31, 33, 0 };
                short[] iSTATUS = new short[] { 0, 1, 2, 3, 5, 30 };

                string[] sLOCALOFFICEID = new string[] { "XL", "XB", "TQ", "TJ", "XN", "TX", "TT", "TF", "TV", "93ZZ", "TN", "TC", "F93", "TG", "XV", "TL", "JR", "XT", "TM", "TR", "J60", "TK", "XH", "XR", "XD", "TS", "P72" },
                    sSUBWAYIDC = new string[] { "Y", "N" },
                    sINTERRUPTERIDC = new string[] { "Y", "N" },
                    sAUTOIDC = new string[] { "Y", "N" },
                    sINSTALLATIONTYPE = new string[] { "UG", "NEW", "PAD", "SUB", "OH" };
                //short iStatus = 0;
                //int iSubtypeCD = 0, iINSTALLJOBNUMBER = 0;

                fieldMapping_Create.Add(pFClass_Transformer.FindField("SubtypeCD"), 1);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("STATUS"), 5);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("LOCALOFFICEID"), "XL");
                fieldMapping_Create.Add(pFClass_Transformer.FindField("INSTALLJOBNUMBER"), "12345678");

                IObject pObject_Main = CreateFeature(variable, pFClass_Transformer, fieldMapping_Create, X, Y);


                //Creating Relationship
                pOClass_Related.get_RelationshipClasses(esriRelRole.esriRelRoleDestination).Next().CreateRelationship(pObject_Related, pObject_Main);
                fieldMapping_Create.Clear();
                we.StopEditOperation();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("CREATE_SERVICEPOINT:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam

            }
            catch (Exception exception)
            {
                this.Logger.WriteLine(string.Concat("CREATE_TRANSFORMER error:", exception.Message, " ", exception.StackTrace), false, null);
            }
            finally
            {
                if (featureLayers != null) Marshal.FinalReleaseComObject(featureLayers);
            }
        }


        private bool InitializeSessionManager(string sConnStr)
        {
            Connection connection;
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();

                IWorkspace loginWorkspace = (new MMLoginUtils()).LoginWorkspace;

                string filePath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;

                //string[] sAllCredentials = File.ReadAllLines(System.IO.Path.GetDirectoryName(filePath) + "\\UserCredentials.txt");

                //IPropertySet connectionProperties = this.Workspace.ConnectionProperties;
                //string username;
                //string password = "";
                //username = connectionProperties.GetProperty("USER") as string;
                //foreach (string line in sAllCredentials)
                //{
                //    string[] credentials = line.Split(',');

                //    if (credentials[0].ToUpper() == username.ToUpper())
                //    {
                //        password = credentials[1];
                //        break;
                //    }
                //}

                IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = (IMxDocument)variable.Document;
                IMMPxApplication application = ((IMMPxIntegrationCache)variable.FindExtensionByName("Session Manager Integration Extension")).Application;
                IMMPxLogin login = application.Login;
                if (login != null)
                {
                    connection = login.Connection;
                    this.Logger.WriteLine(connection.ConnectionString + "::" + "NOT");
                }
                else
                {
                    login = (IMMPxLogin)((PxLogin)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("F39B3DE5-175E-460C-AA99-9921FF07E8FA"))));
                    connection = (Connection)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00000514-0000-0010-8000-00AA006D2EA4")));
                    //sConnStr = sConnStr + ";User ID=" + username + ";Password=" + password;
                    this.Logger.WriteLine(sConnStr);
                    connection.ConnectionString = sConnStr;
                    connection.Open("", "", "", -1);
                    login.Connection = connection;
                    application.Startup(login);
                }
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("INITIALIZESESSION:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("INITIALIZESESSION Error", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            finally
            {
            }
            return flag;
        }

        private bool CreateSession(string sConnStr, string _versionname)
        {
            Connection connection;
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();

                IWorkspace loginWorkspace = (new MMLoginUtils()).LoginWorkspace;
                

                IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = (IMxDocument)variable.Document;
                IMMPxApplication application = ((IMMPxIntegrationCache)variable.FindExtensionByName("Session Manager Integration Extension")).Application;
                IMMPxLogin login = application.Login;
                if (login != null)
                {
                    connection = login.Connection;
                }
                else
                {
                    login = (IMMPxLogin)((PxLogin)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("F39B3DE5-175E-460C-AA99-9921FF07E8FA"))));
                    connection = (Connection)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00000514-0000-0010-8000-00AA006D2EA4")));
                    connection.ConnectionString = sConnStr;
                    connection.Open("", "", "", -1);
                    login.Connection = connection;
                    application.Startup(login);
                }
                IMMSessionVersion variable1 = (IMMSessionVersion)((IMMSessionManager2)application.FindPxExtensionByName("MMSessionManager")).CreateSession();
                IMMSession mmsession = variable1 as IMMSession;
                mmsession.set_Name(_versionname);

                //variable1.set_Name(_versionname);
                //variable1.set_Description(_versionname);
                this.Logger.WriteLine(string.Concat("Session:", mmsession.get_Name()), false, null);
                IVersion version = (IVersion)loginWorkspace;
                //IPropertySet connectionProperties = loginWorkspace.ConnectionProperties;
                //object[] objArray = new object[1];
                //object[] objArray1 = new object[2];
                //connectionProperties.GetAllProperties(out objArray[0], out objArray1[0]);
                //IVersion version1 = version.CreateVersion(variable1.get_Name() );
                //IChangeDatabaseVersion variable2 = (ChangeDatabaseVersion)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0038A3AF-0FCB-487A-B3EE-65C0E80D13F0")));
                //variable2.Execute(version, version1, (IBasicMap)document.FocusMap);
                //this.Workspace = (IWorkspace)version1;
                ////ScriptEngine.BroadcastProperty("Workspace", this.Workspace, null);
                //flag = true;
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("CREATE_ARCFM_SESSION:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("CreateArcFMSession Error", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }

        private bool DisableAutoUpdateres()
        {
            this.Logger.WriteLine("Disable Autoupdaters", false, null);
            Type typeFromProgID = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                (Activator.CreateInstance(typeFromProgID) as IMMAutoUpdater).AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNotSet;
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("DISABLE_AUTOUPDATERS:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Error in Disabling Autoupdaters:", exception.Message), false, null);
            }
            return true;
        }

        private bool EnableAutoUpdaters()
        {
            this.Logger.WriteLine("Enable Autoupdaters", false, null);
            Type typeFromProgID = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                (Activator.CreateInstance(typeFromProgID) as IMMAutoUpdater).AutoUpdaterMode = mmAutoUpdaterMode.mmAUMArcMap;
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("ENABLE_AUTOUPDATERS:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Error in Enabling Autoupdaters:", exception.Message), false, null);
            }
            return true;
        }

        public int Execute(string command)
        {
            try
            {
                string str;
                string[] parameters;
                int num;
                string verb = this._parser.GetVerb(command, true);
                str = (command.Length <= verb.Length ? "" : command.Substring(verb.Length + 1));
                string str1 = verb.Trim();
                if (str1 != null)
                {
                    //For Summary Log- Shashwat Nigam
                    PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log = str1;

                    toolname = str1;
                    //this.Logger.WriteLine("Running Tool:" + toolname + " - " + DateTime.Now.ToString());
                    switch (str1)
                    {
                        case "SETARCFMWORKSPACE":
                            {
                                this.SW1.Start();
                                if (!this.SetArcFMWorkspace())
                                {
                                    num = 2;
                                    //break;
                                }
                                else
                                {
                                    this.SW1.Stop();
                                    this.Logger.WriteLine(string.Concat("SETARCFMWORKSPACE_TIME:", this.SW1.Elapsed), false, null);
                                    num = 3;
                                    //break;
                                }
                                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                                break;
                            }

                        case "EXECUTESQL":
                            {
                                this.EXECUTESQL(str);
                                num = 3;
                                break;
                            }

                        case "INITIALIZESESSION":
                            {
                                this.InitializeSessionManager(str);
                                num = 3;
                                break;
                            }
                        case "OPENSTOREDDISPLAY":
                            {
                                parameters = this.GetParameters(str);
                                if (!this.OpenStoredDisplay(parameters[0], parameters[1]))
                                {
                                    num = 2;
                                    break;
                                }
                                else
                                {
                                    num = 3;
                                    break;
                                }
                            }
                        case "SEARCHOPERATING":
                            {
                                parameters = this.GetParameters(str);
                                if (!this.SearchOperating(str))
                                {
                                    num = 2;
                                    break;
                                }
                                else
                                {
                                    num = 3;
                                    break;
                                }
                            }
                        case "UPDATERELATED_STRING":
                            {
                                parameters = str.Split(';');
                                this.UpdateRelatedStringAttribute(parameters[0], parameters[1], parameters[2], parameters[3]);
                                num = 3;
                                return num;
                            }
                        case "UPDATE_STRING":
                            {
                                parameters = this.GetParameters(str);
                                this.UpdateStringAttribute(parameters[0], parameters[1], parameters[2], parameters[3]);
                                num = 3;
                                return num;
                            }
                        case "SEARCHJOBNUMBER":
                            {
                                parameters = this.GetParameters(str);
                                if (!this.SearchJobNumber(parameters[0]))
                                {
                                    num = 2;
                                    break;
                                }
                                else
                                {
                                    num = 3;
                                    break;
                                }
                            }
                        case "TRACEUPSTREAM":
                            {
                                parameters = this.GetParameters(str);
                                this.ArcFMTrace(parameters[0], parameters[1], parameters[2], 2);
                                num = 3;
                                break;
                            }
                        case "TRACEDOWNSTREAM":
                            {
                                parameters = this.GetParameters(str);
                                this.ArcFMTrace(parameters[0], parameters[1], parameters[2], 1);
                                num = 3;
                                break;
                            }
                        case "TRACEISOLATING":
                            {
                                parameters = this.GetParameters(str);
                                this.ArcFMTrace(parameters[0], parameters[1], parameters[2], 3);
                                num = 3;
                                break;
                            }
                        case "DISABLEAUTOUPDATERS":
                            {
                                this.DisableAutoUpdateres();
                                num = 3;
                                break;
                            }
                        case "ENABLEAUTOUPDATERS":
                            {
                                this.EnableAutoUpdaters();
                                num = 3;
                                break;
                            }
                        case "AUTO_ASSIGN_WL":
                            {
                                this.ARCFM_AUTO_ASSIGN();
                                num = 3;
                                break;
                            }
                        case "OPEN_ARCFM_SESSION":
                            {
                                parameters = this.GetParameters(str);
                                this.OpenSession(parameters[0], parameters[1]);
                                num = 3;
                                break;
                            }
                        case "CLOSE_ARCFM_SESSION":
                            {
                                parameters = this.GetParameters(str);
                                this.CLOSE_ARCFM_SESSION(parameters[0], parameters[1]);
                                num = 3;
                                break;
                            }
                        case "CREATE_ARCFM_SESSION":
                            {
                                parameters = this.GetParameters(str);
                                this.CreateSession(parameters[0], parameters[1]);
                                num = 3;
                                break;
                            }
                        case "ARCFMZOOMTO":
                            {
                                parameters = this.GetParameters(str);
                                this.ARCFM_ZOOMTO(parameters[0], parameters[1], parameters[2]);
                                num = 3;
                                break;
                            }
                        case "ARCFMGasTrace":
                            {
                                parameters = this.GetParameters(str);
                                this.ArcFMGasTrace(parameters[0]);
                                num = 3;
                                break;
                            }
                        case "RECREATEANNO":
                            {
                                parameters = this.GetParameters(str);
                                if (!this.ReCreateAnno(str))
                                {
                                    num = 2;
                                    break;
                                }
                                else
                                {
                                    num = 3;
                                    break;
                                }
                            }

                        case "ASSETREPLACE":
                            {
                                parameters = this.GetParameters(str);
                                this.AssetReplace(parameters[0], Convert.ToInt32(parameters[1]));
                                num = 3;
                                break;
                            }

                        case "CREATETRANSFORMER":
                            {
                                parameters = this.GetParameters(str);
                                this.CreateTransformer(Convert.ToDouble(parameters[0]), Convert.ToDouble(parameters[1]));
                                num = 3;
                                break;
                            }

                        case "CREATESERVICEPOINT":
                            {
                                parameters = this.GetParameters(str);
                                this.CreateServicePoint(Convert.ToDouble(parameters[0]), Convert.ToDouble(parameters[1]));
                                num = 3;
                                break;
                            }

                        case "CREATECONDUCTOR":
                            {
                                parameters = this.GetParameters(str);
                                this.CreateSecUGConductor(Convert.ToDouble(parameters[0]), Convert.ToDouble(parameters[1]), Convert.ToDouble(parameters[2]), Convert.ToDouble(parameters[3]));
                                num = 3;
                                break;
                            }
                        case "MOVEVERTEX":
                            {
                                parameters = this.GetParameters(str);
                                this.MoveVertex(Convert.ToInt32(parameters[0]), parameters[1], Convert.ToInt32(parameters[2]), Convert.ToDouble(parameters[3]), Convert.ToDouble(parameters[4]));
                                num = 3;
                                break;
                            }
                        case "DELETEFEATURE":
                            {
                                parameters = this.GetParameters(str);
                                this.DELETEFEATURE(parameters[0], Convert.ToInt32(parameters[1]));
                                num = 3;
                                break;
                            }
                        default:
                            {
                                goto Label0;
                            }
                    }
                }
                else
                {
                    goto Label0;
                }
                return num;
            Label0:
                if (!string.IsNullOrEmpty(this._parser.RemoveComments(command)))
                {
                    num = 0;
                    return num;
                }
                else
                {
                    num = 1;
                    return num;
                }

            }
            catch { return 0; }
            finally { GC.Collect(); }
        }

        private IDictionary<int, SelectedObjects> GetClassIDs(IGeometricNetwork pGN)
        {
            IFeatureClass i;
            IDataset dataset;
            SelectedObjects selectedObject;
            IDictionary<int, SelectedObjects> nums;
            IEnumFeatureClass classesByType = pGN.get_ClassesByType(esriFeatureType.esriFTComplexEdge) as IEnumFeatureClass;
            try
            {
                IDictionary<int, SelectedObjects> nums1 = new Dictionary<int, SelectedObjects>();

                for (i = classesByType.Next(); i != null; i = classesByType.Next())
                {
                    if (!nums1.ContainsKey(i.FeatureClassID))
                    {
                        dataset = (IDataset)i;
                        selectedObject = new SelectedObjects(i);
                        nums1.Add(i.FeatureClassID, selectedObject);
                    }
                }
                classesByType = pGN.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);
                for (i = classesByType.Next(); i != null; i = classesByType.Next())
                {
                    if (!nums1.ContainsKey(i.FeatureClassID))
                    {
                        dataset = (IDataset)i;
                        selectedObject = new SelectedObjects(i);
                        nums1.Add(i.FeatureClassID, selectedObject);
                    }
                }
                classesByType = pGN.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);
                for (i = classesByType.Next(); i != null; i = classesByType.Next())
                {
                    if (!nums1.ContainsKey(i.FeatureClassID))
                    {
                        dataset = (IDataset)i;
                        selectedObject = new SelectedObjects(i);
                        nums1.Add(i.FeatureClassID, selectedObject);
                    }
                }
                Marshal.FinalReleaseComObject(classesByType);
                nums = nums1;
            }
            catch (Exception exception)
            {
                nums = null;
            }
            finally
            {
                if (classesByType != null) Marshal.FinalReleaseComObject(classesByType);
            }
            return nums;
        }

        private IMxDocument GetDoc()
        {
            Type typeFromProgID = Type.GetTypeFromProgID("esriFramework.AppRef");
            return (IMxDocument)(Activator.CreateInstance(typeFromProgID) as IApplication).Document;
        }

        private string[] GetParameters(string sInArgs)
        {
            string[] strArrays;
            if (sInArgs.Length <= 0)
            {
                strArrays = new string[] { "", "" };
                this.Logger.WriteLine("No Parameters", false, null);
            }
            else
            {
                strArrays = sInArgs.Split(new char[] { ',' });
                int num = 0;
                while (num < strArrays.GetUpperBound(0))
                {
                    num++;
                }
            }
            return strArrays;
        }

        public bool UpdateRelatedStringAttribute(string sClassName, string relationships, string sValue, string sWhere)
        {
            Exception exception;
            bool flag;
            UID uIDClass = new UID();
            IEnumLayer featureLayers = null;
            try
            {
                try
                {
                    IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                    uIDClass.Value = "esriEditor.Editor";
                    IEditor variable1 = variable.FindExtensionByCLSID(uIDClass) as IEditor;

                    IMap pMap = ((IMxDocument)variable.Document).FocusMap;

                    UID featureUID = new UID();
                    featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    featureLayers = pMap.get_Layers(featureUID, true);

                    IFeatureLayer featLayer = null;
                    IFeatureWorkspace workspace = null;

                    string[] coumnvalues = sValue.Split('-');
                    string[] sColumnNames = coumnvalues[0].Split(':');
                    string[] values = coumnvalues[1].Split(':');

                    while ((featLayer = featureLayers.Next() as IFeatureLayer) != null)
                    {
                        try
                        {
                            if (((IDataset)(featLayer.FeatureClass)).BrowseName == sClassName)
                            {
                                IVersion pversion = ((IDataset)(featLayer.FeatureClass)).Workspace as IVersion;
                                this.Logger.WriteLine(pversion.VersionName);
                                this.Workspace = ((IDataset)(featLayer.FeatureClass)).Workspace;
                                workspace = ((IDataset)(featLayer.FeatureClass)).Workspace as IFeatureWorkspace;
                                break;
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }


                    this.SW1.Reset();
                    this.SW1.Start();
                    //IFeatureWorkspace workspace = (IFeatureWorkspace)this.Workspace;
                    IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
                    IObjectClass objectClass = (IObjectClass)workspace.OpenTable(sClassName);

                    try
                    {
                        IQueryFilter queryFilterClass = new QueryFilter()
                        {
                            WhereClause = sWhere
                        };
                        this.Logger.WriteLine(string.Concat("Where=", sWhere), false, null);
                        ITable table = (ITable)objectClass;
                        ICursor cursor = null;
                        IRow row = null;
                        IEnumRelationshipClass relClasses = null;
                        try
                        {
                            cursor = table.Search(queryFilterClass, false);
                            this.Logger.WriteLine("Got Cursor", false, null);
                            row = cursor.NextRow();
                            this.Logger.WriteLine("Got Row", false, null);
                            workspaceEdit.StartEditOperation();
                            this.Logger.WriteLine("Started Editor", false, null);

                            ISet relatedFeatures = null;


                            if (row != null)
                            {
                                IFeature pfeature = row as IFeature;
                                relClasses = pfeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                                IRelationshipClass rel = relClasses.Next();

                                while (rel != null)
                                {
                                    if (rel.DestinationClass.AliasName.ToUpper() == relationships.ToUpper())
                                    {
                                        relatedFeatures = rel.GetObjectsRelatedToObject(pfeature);
                                        break;
                                    }
                                    rel = relClasses.Next();
                                    this.Logger.WriteLine(rel.DestinationClass.AliasName);
                                }
                            }

                            IRow pRow = relatedFeatures.Next() as IRow;

                            while (pRow != null)
                            {


                                for (int i = 0; i < sColumnNames.Length; i++)
                                {
                                    int columidx = pRow.Fields.FindField(sColumnNames[i]);
                                    if (columidx != -1)
                                    {
                                        pRow.set_Value(columidx, values[i] as object);
                                    }
                                }
                                pRow.Store();
                                pRow = relatedFeatures.Next() as IRow;

                            }

                            workspaceEdit.StopEditOperation();

                            //SetEvents(variable);
                            ((IMxDocument)variable.Document).ActiveView.Refresh();
                            //customRefresh(variable); 

                            this.SW1.Stop();
                            this.Logger.WriteLine(string.Concat("UPDATERELATED_STRING:", this.SW1.ElapsedMilliseconds), false, null);
                            flag = true;
                            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                        }
                        catch (Exception exception1)
                        {
                            exception = exception1;
                            this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                            flag = false;
                        }
                        finally
                        {
                            if (relClasses != null) Marshal.FinalReleaseComObject(relClasses);
                            if (cursor != null) Marshal.FinalReleaseComObject(cursor);
                            if (row != null) Marshal.FinalReleaseComObject(row);
                        }
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                        flag = false;
                    }
                }
                catch (Exception exception3)
                {
                    exception = exception3;
                    this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                    flag = false;
                }
            }
            catch (Exception exception4)
            {
                exception = exception4;
                this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            finally
            {
                if (featureLayers != null)
                    Marshal.FinalReleaseComObject(featureLayers);
            }
            return flag;
        }


        public bool UpdateStringAttribute(string sClassName, string sColumnName, string sValue, string sWhere)
        {
            Exception exception;
            bool flag;
            UID uIDClass = new UID();
            IEnumLayer featureLayers = null;
            try
            {
                try
                {
                    IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                    uIDClass.Value = "esriEditor.Editor";
                    IEditor variable1 = variable.FindExtensionByCLSID(uIDClass) as IEditor;

                    IMap pMap = ((IMxDocument)variable.Document).FocusMap;

                    UID featureUID = new UID();
                    featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    featureLayers = pMap.get_Layers(featureUID, true);

                    IFeatureLayer featLayer = null;
                    IFeatureWorkspace workspace = null;

                    while ((featLayer = featureLayers.Next() as IFeatureLayer) != null)
                    {
                        //this.Logger.WriteLine(featLayer.Name);
                        try
                        {
                            if (((IDataset)(featLayer.FeatureClass)).BrowseName == sClassName)
                            {
                                IVersion pversion = ((IDataset)(featLayer.FeatureClass)).Workspace as IVersion;
                                this.Logger.WriteLine(pversion.VersionName);
                                this.Workspace = ((IDataset)(featLayer.FeatureClass)).Workspace;
                                workspace = ((IDataset)(featLayer.FeatureClass)).Workspace as IFeatureWorkspace;
                                break;
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }


                    this.SW1.Reset();
                    this.SW1.Start();
                    //IFeatureWorkspace workspace = (IFeatureWorkspace)this.Workspace;
                    IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
                    IObjectClass objectClass = (IObjectClass)workspace.OpenTable(sClassName);
                    ICursor cursor = null;
                    IRow row = null;
                    IQueryFilter queryFilterClass = new QueryFilter()
                    {
                        WhereClause = sWhere
                    };
                    try
                    {
                        this.Logger.WriteLine(string.Concat("Where=", sWhere), false, null);
                        ITable table = (ITable)objectClass;
                        try
                        {
                            cursor = table.Search(queryFilterClass, false);
                            this.Logger.WriteLine("Got Cursor", false, null);
                            row = cursor.NextRow();
                            this.Logger.WriteLine("Got Row", false, null);
                            workspaceEdit.StartEditOperation();
                            this.Logger.WriteLine("Started Editor", false, null);
                            while (row != null)
                            {
                                int num = row.Fields.FindField(sColumnName);
                                this.Logger.WriteLine(string.Concat("Field:", num), false, null);
                                row.set_Value(num, sValue);
                                row.Store();
                                row = cursor.NextRow();
                            }
                            workspaceEdit.StopEditOperation();

                            //SetEvents(variable);
                            ((IMxDocument)variable.Document).ActiveView.Refresh();
                            //customRefresh(variable); 

                            this.SW1.Stop();
                            this.Logger.WriteLine(string.Concat("UPDATE_STRING:", this.SW1.ElapsedMilliseconds), false, null);
                            flag = true;
                            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                        }
                        catch (Exception exception1)
                        {
                            exception = exception1;
                            this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                            flag = false;
                        }
                        finally
                        {
                            if (cursor != null) Marshal.FinalReleaseComObject(cursor);
                            if (row != null) Marshal.FinalReleaseComObject(row);
                            if (queryFilterClass != null) Marshal.FinalReleaseComObject(queryFilterClass);
                        }
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                        flag = false;
                    }
                }
                catch (Exception exception3)
                {
                    exception = exception3;
                    this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                    flag = false;
                }
            }
            catch (Exception exception4)
            {
                exception = exception4;
                this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            finally
            {
                if (featureLayers != null) Marshal.FinalReleaseComObject(featureLayers);
            }
            return flag;
        }

        public bool OPEN_ARCFM_SESSION(string sSessionName, string sConnStr)
        {
            Connection connection;
            bool flag;
            this.SW1.Reset();
            this.SW1.Start();
            try
            {
                IWorkspace loginWorkspace = (new MMLoginUtils()).LoginWorkspace;
                IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = (IMxDocument)variable.Document;
                IMMPxApplication application = ((IMMPxIntegrationCache)variable.FindExtensionByName("Session Manager Integration Extension")).Application;
                IMMPxLogin login = application.Login;
                IMMSessionManager2 variable1 = (IMMSessionManager2)application.FindPxExtensionByName("MMSessionManager");
                if (login.Connection != null)
                {
                    connection = login.Connection;
                }
                else
                {
                    connection = (Connection)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00000514-0000-0010-8000-00AA006D2EA4")));
                    login.ConnectionString = sConnStr;
                    connection.Open("", "", "", -1);
                }
                application.Startup(login);
                int num = Convert.ToInt32(sSessionName);
                bool flag1 = false;
                IMMSessionVersion session = (IMMSessionVersion)variable1.GetSession(ref num, ref flag1);
                IVersion version = (IVersion)loginWorkspace;
                IVersion version1 = version.CreateVersion(session.get_Name());
                IChangeDatabaseVersion variable2 = (ChangeDatabaseVersion)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0038A3AF-0FCB-487A-B3EE-65C0E80D13F0")));
                variable2.Execute(version, version1, (IBasicMap)document.FocusMap);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("OPEN_ARCFM_SESSION:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                //pAList_Log
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Failed: OPEN_ARCFM_SESSION", exception.Message), false, null);
                flag = false;
            }
            return flag;
        }

        private bool OpenSession(string sSessionName, string sConnStr)
        {
            Connection connection;
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                IWorkspace loginWorkspace = (new MMLoginUtils()).LoginWorkspace;
                IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = (IMxDocument)variable.Document;
                IMMPxApplication application = ((IMMPxIntegrationCache)variable.FindExtensionByName("Session Manager Integration Extension")).Application;
                IMMPxLogin login = application.Login;
                //this.Logger.WriteLine("1");
                IMMSessionManager2 variable1 = (IMMSessionManager2)application.FindPxExtensionByName("MMSessionManager");
                if (login.Connection != null)
                {
                    connection = login.Connection;
                }
                else
                {
                    connection = (Connection)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("00000514-0000-0010-8000-00AA006D2EA4")));
                    login.ConnectionString = sConnStr;
                    connection.Open("", "", "", -1);
                }
                application.Startup(login);
                //this.Logger.WriteLine("2");
                int num = Convert.ToInt32(sSessionName);
                //this.Logger.WriteLine("3");
                bool flag1 = false;


                IMMSessionVersion session = (IMMSessionVersion)variable1.GetSession(ref num, ref flag1);

                IVersion version = (IVersion)loginWorkspace;
                IVersion version1 = version.CreateVersion(session.get_Name());
                IChangeDatabaseVersion variable2 = (ChangeDatabaseVersion)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0038A3AF-0FCB-487A-B3EE-65C0E80D13F0")));
                variable2.Execute(version, version1, (IBasicMap)document.FocusMap);
                this.Workspace = (IWorkspace)version1;
                UID uIDClass = new UID()
                {
                    Value = "esriEditor.Editor"
                };



                //this.Logger.WriteLine("4");
                (variable.FindExtensionByCLSID(uIDClass) as IEditor).StartEditing(this.Workspace);
                //ScriptEngine.BroadcastProperty("Workspace", this.Workspace, null);
                //this.Logger.WriteLine("5");
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("OPEN_ARCFM_SESSION:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception)
            {
                this.Logger.WriteLine(string.Concat("Error in OPEN_ARCFM_SESSION:", exception.Message, ":", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }

        private bool OpenStoredDisplay(string SDName, string SDType)
        {
            IMMEnumStoredDisplayName mMEnumStoredDisplayName;
            Exception exception;
            bool flag;
            try
            {

                IApplication m_pApp = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;

                this.Logger.WriteLine(string.Concat("OpenStoredDisplay:", SDName), false, null);
                if (!(SDName == ""))
                {
                    if (this._SDMGR == null)
                    {
                        this._SDMGR = new MMStoredDisplayManager();
                    }
                    this._SDMGR.Workspace = this.Workspace;
                    IMMStoredDisplayName mMStoredDisplayNameClass = new MMStoredDisplayName()
                    {
                        Name = SDName
                    };
                    mMEnumStoredDisplayName = (!(SDType.ToUpper() == "SYSTEM") ? this._SDMGR.GetStoredDisplayNames(mmStoredDisplayType.mmSDTUser) : this._SDMGR.GetStoredDisplayNames(mmStoredDisplayType.mmSDTSystem));
                    mMStoredDisplayNameClass = mMEnumStoredDisplayName.Next();
                    while (mMStoredDisplayNameClass != null)
                    {
                        if (!(mMStoredDisplayNameClass.Name.Trim().ToUpper() == SDName.Trim().ToUpper()))
                        {
                            mMStoredDisplayNameClass = mMEnumStoredDisplayName.Next();
                        }
                        else
                        {
                            this.SW1.Reset();
                            this.SW1.Start();
                            try
                            {
                                this._SDMGR.OpenStoredDisplay(mMStoredDisplayNameClass);
                                //SetEvents(m_pApp);
                                this.SW1.Stop();
                                this.Logger.WriteLine(string.Concat("OPEN_STORED_DISPLAY:", this.SW1.ElapsedMilliseconds), false, null);
                                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                            }
                            catch (Exception exception1)
                            {
                                exception = exception1;
                                this.Logger.WriteLine(string.Concat("Error Opening Stored Display :", exception.Message, ":", exception.StackTrace), false, null);
                                this.SW1.Stop();
                            }
                            break;
                        }
                    }
                    if (mMStoredDisplayNameClass != null)
                    {
                        flag = true;
                    }
                    else
                    {
                        this.Logger.WriteLine("StoredDisplay not found", false, null);
                        flag = false;
                    }
                }
                else
                {
                    this.Logger.WriteLine("StoredDisplay Name not provided", false, null);
                    flag = false;
                }
            }
            catch (Exception exception2)
            {
                exception = exception2;
                this.Logger.WriteLine(string.Concat("Error in OpenStoredDisplay:", exception.Message, ":", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }

        private void SetEvents(IApplication m_pApp)
        {
            start = 1;

            if (m_pApp != null)
            {
                IMxDocument doc = m_pApp.Document as IMxDocument;

                IMap map = ((IMxDocument)m_pApp.Document).FocusMap;
                activeViewEvents = map as ESRI.ArcGIS.Carto.IActiveViewEvents_Event;
                //Create an instance of the delegate, add it to AfterDraw event
                m_ActiveViewEventsAfterDraw = new ESRI.ArcGIS.Carto.IActiveViewEvents_AfterDrawEventHandler(OnActiveViewEventsAfterDraw);

                activeViewEvents.AfterDraw += m_ActiveViewEventsAfterDraw;

                m_ActiveViewEventsViewRefreshed = new ESRI.ArcGIS.Carto.IActiveViewEvents_ViewRefreshedEventHandler(OnActiveViewEventsViewRefreshed);
                activeViewEvents.ViewRefreshed += m_ActiveViewEventsViewRefreshed;

                ////displayViewEvents = ((IMxDocument)m_pApp.Document).ActivatedView.ScreenDisplay as IDisplayEvents ; 
                ////displayViewEvents.DisplayFinished += new ac

                //doc.ActivatedView.Refresh();
            }
        }

        private bool EXECUTESQL(string sCommand)
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                this.Logger.WriteLine(sCommand, false, null);
                this.Workspace.ExecuteSQL(sCommand);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("EXECUTESQL:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Error in EXECUTESQL ", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }

        private bool SetArcFMWorkspace()
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                this._SDMGR = new MMStoredDisplayManager();
                this.Workspace = (new MMLoginUtils()).LoginWorkspace;
                //ScriptEngine.BroadcastProperty("Workspace", this.Workspace, null);
                if (this.Workspace == null)
                    this.Logger.WriteLine("emptyworkspace");
                ScriptEngine.GetSdeVersion(this.Workspace);
                this.SW1.Stop();
                this._SDMGR.Workspace = this.Workspace;
                this.Logger.WriteLine(string.Concat("SETARCFMWORKSPACE:", this.SW1.ElapsedMilliseconds), false, null);

                //PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Error in SetArcFMWorkspace:", exception.Message, ":", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }

        private void SwizzleDatasets(IApplication pApp, IFeatureWorkspace pOldWorkspace, IFeatureWorkspace pNewWorkspace)
        {
            IDataset table;
            IMxDocument document = (IMxDocument)pApp.Document;
            IVersion version = (IVersion)pOldWorkspace;
            IVersion version1 = (IVersion)pNewWorkspace;
            IMaps maps = document.Maps;
            IMapAdmin focusMap = (IMapAdmin)document.FocusMap;
            for (int i = 0; i >= maps.Count - 1; i++)
            {
                IMap item = maps.get_Item(i);
                if (item.LayerCount > 0)
                {
                    IEnumLayer layers = item.Layers[null, true];
                    for (ILayer j = layers.Next(); j != null; j = layers.Next())
                    {
                        if (j is IFeatureLayer)
                        {
                            IFeatureLayer variable = (IFeatureLayer)j;
                            if (variable.FeatureClass != null)
                            {
                                IFeatureClass featureClass = variable.FeatureClass;
                                table = (IDataset)featureClass;
                                if (table.Workspace.Equals(pOldWorkspace))
                                {
                                    IFeatureClass featureClass1 = pNewWorkspace.OpenFeatureClass(table.Name);
                                    variable.FeatureClass = featureClass1;
                                    focusMap.FireChangeFeatureClass(featureClass, featureClass1);
                                }
                            }
                        }
                    }
                }
                ITableCollection variable1 = (ITableCollection)item;
                for (int k = 0; k >= variable1.TableCount - 1; k++)
                {
                    table = (IDataset)variable1.get_Table(k);
                    if (table.Workspace.Equals(pOldWorkspace))
                    {
                        variable1.RemoveTable((ITable)table);
                        variable1.AddTable(pNewWorkspace.OpenTable(table.Name));
                    }
                }
            }
            document.UpdateContents();
            focusMap.FireChangeVersion(version, version1);
            version1.RefreshVersion();
        }

        private bool TraceIsolating(string sOID, string sSelect)
        {
            IMMTracedElements mMTracedElement;
            IMMTracedElements mMTracedElement1;
            Exception exception;
            bool flag;
            int[] numArray = new int[0];
            int[] numArray1 = new int[0];
            IMMCurrentStatus mMCurrentStatu = null;
            try
            {
                IMMElectricTracingEx mMFeederTracerClass = new MMFeederTracer() as IMMElectricTracingEx;
                IMMElectricTraceSettings mMElectricTraceSettingsClass = new MMElectricTraceSettings()
                {
                    RespectConductorPhasing = false,
                    RespectEnabledField = false
                };
                this.SW1.Reset();
                this.SW1.Start();
                try
                {
                    if (this.FeatureClass is INetworkClass)
                    {
                        IGeometricNetwork geometricNetwork = ((INetworkClass)this.FeatureClass).GeometricNetwork;
                        IFeature feature = this.FeatureClass.GetFeature(Convert.ToInt32(sOID));
                        if (feature is IJunctionFeature)
                        {
                            ISimpleJunctionFeature simpleJunctionFeature = (ISimpleJunctionFeature)feature;
                            mMFeederTracerClass.TraceDownstream(geometricNetwork, mMElectricTraceSettingsClass, mMCurrentStatu, simpleJunctionFeature.EID, esriElementType.esriETJunction, SetOfPhases.abc, mmDirectionInfo.establishBySourceSearch, 0, esriElementType.esriETEdge, numArray, numArray1, false, out mMTracedElement, out mMTracedElement1);
                        }
                    }
                    this.SW1.Stop();
                    this.Logger.WriteLine(string.Concat("TRACE_ISOLATION_EXECUTION_TIME:", this.SW1.ElapsedMilliseconds), false, null);
                    PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                    flag = true;
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    this.Logger.WriteLine(string.Concat("TraceDownStream Failed: ", exception.Message), false, null);
                    flag = false;
                }
            }
            catch (Exception exception2)
            {
                exception = exception2;
                this.Logger.WriteLine(string.Concat("TraceDownStream Failed: ", exception.Message), false, null);
                flag = false;
            }
            return flag;
        }

        public bool UPDATE_String(string sValue, string sWhere)
        {
            this.SW1.Reset();
            this.SW1.Start();
            this.SW1.Stop();
            this.Logger.WriteLine(string.Concat("UPDATE_STRING:", this.SW1.ElapsedMilliseconds), false, null);
            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            return true;
        }

        public bool ZOOMTOSELECTED()
        {
            this.SW1.Reset();
            this.SW1.Start();
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            IMxDocument document = (IMxDocument)variable.Document;
            ICommand variable1 = (ControlsZoomToSelectedCommand)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("06DD3F57-CF78-41BA-83F4-D13A8679914F")));
            variable1.OnCreate(variable);
            variable1.OnClick();
            this.SW1.Stop();
            this.Logger.WriteLine(string.Concat("ZOOM_TO_SELECTED:", this.SW1.ElapsedMilliseconds), false, null);
            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            return true;
        }

        private IApplication p_app;

        private const string _featureLayerClassID = "7158830B-DAE3-461F-A900-58F201BF873E";


        private static HashSet<IFeatureLayer> _featLayerList;

        private int recordCount;
        private IMMSearchControl _searchControl;
        private IMap _map;
        private string _userInput = string.Empty;
        private bool _layerCountChanged;
        private int _thresholdValue;
        private int _divisionCode;
        private bool _debugDisplay;

        public static void InitializeLayerList(IMap map)
        {
            if (_featLayerList == null)
                _featLayerList = new HashSet<IFeatureLayer>();
            _featLayerList.Clear();

            //get all layer in enumerator
            IEnumLayer enumerator = map.Layers[CartoFacade.UIDFacade.FeatureLayers];
            ILayer layer;

            while ((layer = enumerator.Next()) != null)// if layer not null
            {
                //check if layer is feature layer and layer is valid
                if (layer is IFeatureLayer && layer.Valid)
                {
                    //add layer to list 
                    _featLayerList.Add((IFeatureLayer)layer);
                }
            }
        }

        class Comparer : IEquatable<Comparer>
        {
            int _objectId;
            int _classId;
            public Comparer(IRow row)
            {
                _objectId = row.OID;
                _classId = ((IObject)row).Class.ObjectClassID;
            }

            public bool Equals(Comparer other)
            {
                return _objectId == other._objectId && _classId == other._classId;
            }

            public override bool Equals(object obj)
            {
                var other = obj as Comparer;
                if (obj is IRow)
                    other = new Comparer((IRow)obj);
                if (other == null)
                    return false;
                return Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked { return _classId * 397 ^ _objectId; }
            }

            public override string ToString()
            {
                return string.Format("OID: {0}, CLSID: {1}", _objectId, _classId);
            }
        }
        private bool SearchJobNumber(string PMOrderNumber)
        {
            ICursor cursor = null;
            IRow currentRow = null;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                recordCount = 0;
                if (p_app == null)
                    p_app = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = p_app.Document as IMxDocument;
                IMap _map = document.FocusMap;
                if (_featLayerList == null || _layerCountChanged)
                {
                    InitializeLayerList(_map);
                }


                Func<IObjectClass, string> layerOperatingNumberIsNull =
                objectClass =>
                    ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.JobNumber).Name + " IS NULL";

                Func<IObjectClass, string, string> layerOperatingNumberEquals =
                    (objectClass, value) =>
                        string.Format("UPPER({0}) = UPPER('{1}')", ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.JobNumber).Name, value);

                IEnvelope penv = null;
                var uniqueResults = new HashSet<Comparer>();
                foreach (var layer in _featLayerList)
                {
                    // check whether the layer name has the JobNumber field model name
                    if (!ModelNameFacade.ContainsFieldModelName(layer.FeatureClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.JobNumber))
                        continue;

                    //this.Logger.WriteLine(layer.Name);
                    var objectClass = (IObjectClass)layer.FeatureClass;

                    IQueryFilter queryFilter = new QueryFilter
                    {
                        WhereClause = string.IsNullOrEmpty(PMOrderNumber) ? layerOperatingNumberIsNull(objectClass) : layerOperatingNumberEquals(objectClass, PMOrderNumber)
                    };

                    if (_thresholdValue > 0)
                    {
                        queryFilter.WhereClause += " And ROWNUM <= " + _thresholdValue;
                    }
                    //_logger.Debug("WhereClause statement: " + queryFilter.WhereClause);
                    cursor = (ICursor)layer.FeatureClass.Search(queryFilter, false);
                    
                    while ((currentRow = cursor.NextRow()) != null)
                    {
                        recordCount++;
                        if (penv == null)
                        {
                            penv = new Envelope() as IEnvelope;
                            penv.XMax = ((IFeature)currentRow).Shape.Envelope.XMin + 200;
                            penv.YMax = ((IFeature)currentRow).Shape.Envelope.YMin + 200;
                            penv.XMin = ((IFeature)currentRow).Shape.Envelope.XMin - 200;
                            penv.YMin = ((IFeature)currentRow).Shape.Envelope.YMin - 200;

                            //this.Logger.WriteLine(pFeature.Shape.Envelope.Width);
                        }
                        else
                        {
                            IEnvelope penv1 = new Envelope() as IEnvelope;
                            penv1.XMax = ((IFeature)currentRow).Shape.Envelope.XMin + 200;
                            penv1.YMax = ((IFeature)currentRow).Shape.Envelope.YMin + 200;
                            penv1.XMin = ((IFeature)currentRow).Shape.Envelope.XMin - 200;
                            penv1.YMin = ((IFeature)currentRow).Shape.Envelope.YMin - 200;
                            penv.Union(penv1);
                        }
                    }
                }

                //SetEvents(p_app);
                document.ActivatedView.Extent = (IEnvelope)penv;
                document.ActivatedView.Refresh();

                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("SEARCHJOBNUMBER:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam

            }
            catch (Exception ex)
            {
                //_logger.Error("ERROR - ", ex);
                this.Logger.WriteLine(string.Concat("Error in SearchJobNumber:", ex.Message, ":", ex.StackTrace), false, null);
                return false;
            }
            finally
            {
                if (cursor != null)
                {
                    //Release COM object
                    //while (Marshal.FinalReleaseComObject(cursor) > 0) { }
                    //cursor = null;
                    if (currentRow != null) Marshal.FinalReleaseComObject(currentRow);
                    if (cursor != null) Marshal.FinalReleaseComObject(cursor);
                }
            }
            return true;
        }

        public bool SearchOperating(string command)
        {
            try
            {
                this.SW1.Reset();
                this.SW1.Start();

                string[] args = this.GetParameters(command);

                recordCount = 0;
                if (p_app == null)
                    p_app = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                IMxDocument document = p_app.Document as IMxDocument;

                IMap _map = document.FocusMap;

                if (_featLayerList == null || _layerCountChanged)
                {
                    InitializeLayerList(_map);
                }

                string operatingNumber = "";

                operatingNumber = args[0];


                Func<IObjectClass, string> layerOperatingNumberIsNull =
                    objectClass =>
                        ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorOperatingNumber).Name + " IS NULL";

                Func<IObjectClass, string, string> layerOperatingNumberEquals =
                    (objectClass, value) =>
                        string.Format("UPPER({0}) = UPPER('{1}')", ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorOperatingNumber).Name, value);

                Func<IObjectClass, int, string> layerDivisionNumberEquals =
                    (objectClass, value) => ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorDivision) == null ? null :
                        string.Format("{0} = {1}", ModelNameFacade.FieldFromModelName(objectClass, SchemaInfo.Electric.FieldModelNames.LocatorDivision).Name, value);


                List<string> debugList = null;
                var uniqueResults = new HashSet<Comparer>();
                IEnvelope penv = null;
                IFeatureSelection fselection = null;
                //for (int i = 0; i < _featLayerList.Count(); i++)// loop through each layers
                foreach (var layer in _featLayerList)
                {

                    // check whether the layer name has the OperatingNumber field model name
                    if (!ModelNameFacade.ContainsFieldModelName(layer.FeatureClass as IObjectClass, SchemaInfo.Electric.FieldModelNames.LocatorOperatingNumber))
                        continue;

                    var objectClass = (IObjectClass)layer.FeatureClass;



                    IQueryFilter queryFilter = new QueryFilter()
                    {
                        WhereClause =
                            string.IsNullOrEmpty(operatingNumber)
                            ? layerOperatingNumberIsNull(objectClass)
                            : layerOperatingNumberEquals(objectClass, operatingNumber)
                    };

                    if (_thresholdValue > 0)
                    {
                        queryFilter.WhereClause += " And ROWNUM <= " + _thresholdValue;
                    }
                    _divisionCode = Convert.ToInt16(args[1]);

                    if (_divisionCode != -1)
                    {
                        if (layerDivisionNumberEquals(objectClass, _divisionCode) != null)
                            queryFilter.WhereClause += " And " + layerDivisionNumberEquals(objectClass, _divisionCode);
                        else
                        {
                            //_logger.Error("Layer : " + objectClass.AliasName + " does not have Division field or the field model name " + SchemaInfo.Electric.FieldModelNames.LocatorDivision + " is not assigned to Division field");
                            continue;
                        }
                        //logger
                    }


                    var cursor = (ICursor)layer.FeatureClass.Search(queryFilter, false); 
                    IRow currentRow=null;
                    try
                    {
                       
                        while ((currentRow = cursor.NextRow()) != null)
                        {
                            //if (Stopped)
                            //    return;

                            var comparer = new Comparer(currentRow);
                            if (_debugDisplay)
                            {
                                if (debugList == null)
                                    debugList = new List<string>();
                                debugList.Add(comparer.ToString());
                            }
                            if (uniqueResults.Contains(comparer))
                            {
                                if (_debugDisplay)
                                {
                                    var item = debugList[debugList.Count - 1];
                                    debugList[debugList.Count - 1] = "duplicate " + item;
                                }
                                continue;
                            }
                            uniqueResults.Add(comparer);
                            recordCount++;
                            IFeature pFeature = currentRow as IFeature;
                            fselection = layer as IFeatureSelection;

                            fselection.SelectFeatures(queryFilter, esriSelectionResultEnum.esriSelectionResultAdd, false);

                            if (penv == null)
                            {
                                penv = new Envelope() as IEnvelope;
                                penv.XMax = pFeature.Shape.Envelope.XMin + 200;
                                penv.YMax = pFeature.Shape.Envelope.YMin + 200;
                                penv.XMin = pFeature.Shape.Envelope.XMin - 200;
                                penv.YMin = pFeature.Shape.Envelope.YMin - 200;

                                //this.Logger.WriteLine(pFeature.Shape.Envelope.Width);
                            }
                            else
                            {
                                IEnvelope penv1 = new Envelope() as IEnvelope;
                                penv1.XMax = pFeature.Shape.Envelope.XMin + 200;
                                penv1.YMax = pFeature.Shape.Envelope.YMin + 200;
                                penv1.XMin = pFeature.Shape.Envelope.XMin - 200;
                                penv1.YMin = pFeature.Shape.Envelope.YMin - 200;
                                penv.Union(penv1);
                            }

                        }

                    }
                    finally
                    {
                        //Release COM object
                       if(cursor != null) Marshal.FinalReleaseComObject(cursor);
                       if (currentRow != null) Marshal.FinalReleaseComObject(currentRow);                        
                    }
                }

                this.Logger.WriteLine(recordCount.ToString());
                //SetEvents(p_app);
                document.ActivatedView.Extent = (IEnvelope)penv;
                document.ActivatedView.Refresh();

                //customRefresh(p_app);

                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("SEARCHOPERATING_TIME:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                return true;

            }
            catch (Exception e)
            {

                this.Logger.WriteLine(string.Concat("Error in SearchOperating:", e.Message, ":", e.StackTrace), false, null);
                return false;
            }



        }

        private IDisplayEvents_Event m_dispevt;
        ESRI.ArcGIS.Carto.IActiveViewEvents_Event activeViewEvents;
        ESRI.ArcGIS.Display.IDisplayEvents displayViewEvents;
        ESRI.ArcGIS.Carto.esriViewDrawPhase esriDraw;
        private static int start = 0;
        private static int finish = 0;
        private bool refreshed = false;
        private static string toolname;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_AfterDrawEventHandler m_ActiveViewEventsAfterDraw;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ViewRefreshedEventHandler m_ActiveViewEventsViewRefreshed;

        private void OnActiveViewEventsAfterDraw(ESRI.ArcGIS.Display.IDisplay display, ESRI.ArcGIS.Carto.esriViewDrawPhase phase)
        {
            // TODO: Add your code here
            // System.Windows.Forms.MessageBox.Show("ViewRefreshed");
            start = 1;

            if (phase == esriViewDrawPhase.esriViewForeground)
            {
                activeViewEvents.AfterDraw -= m_ActiveViewEventsAfterDraw;
                activeViewEvents.ViewRefreshed -= m_ActiveViewEventsViewRefreshed;
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat(toolname.ToUpper() + ":", this.SW1.ElapsedMilliseconds), false, null);
                this.Logger.WriteLine(DateTime.Now.ToString());
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                refreshed = true;
            }
        }

        private void OnActiveViewEventsViewRefreshed(ESRI.ArcGIS.Carto.IActiveView view, ESRI.ArcGIS.Carto.esriViewDrawPhase phase, System.Object data, ESRI.ArcGIS.Geometry.IEnvelope envelope)
        {
            start = 1;
        }

        private void customRefresh(IApplication m_pApp)
        {
            start = 1;

            if (m_pApp != null)
            {
                IMxDocument doc = m_pApp.Document as IMxDocument;

                IMap map = ((IMxDocument)m_pApp.Document).FocusMap;
                activeViewEvents = map as ESRI.ArcGIS.Carto.IActiveViewEvents_Event;
                //Create an instance of the delegate, add it to AfterDraw event
                m_ActiveViewEventsAfterDraw = new ESRI.ArcGIS.Carto.IActiveViewEvents_AfterDrawEventHandler(OnActiveViewEventsAfterDraw);

                activeViewEvents.AfterDraw += m_ActiveViewEventsAfterDraw;

                m_ActiveViewEventsViewRefreshed = new ESRI.ArcGIS.Carto.IActiveViewEvents_ViewRefreshedEventHandler(OnActiveViewEventsViewRefreshed);
                activeViewEvents.ViewRefreshed += m_ActiveViewEventsViewRefreshed;

                doc.ActivatedView.Refresh();
            }
        }

        private bool ReCreateAnno(string command)
        {
            Exception exception;
            bool flag = false;
            UID uIDClass = new UID();
            string[] parameters = this.GetParameters(command);

            try
            {
                try
                {
                    IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                    uIDClass.Value = "esriEditor.Editor";
                    IEditor variable1 = variable.FindExtensionByCLSID(uIDClass) as IEditor;

                    IMap pMap = ((IMxDocument)variable.Document).FocusMap;

                    UID featureUID = new UID();
                    featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    IEnumLayer featureLayers = pMap.get_Layers(featureUID, true);

                    IFeatureLayer featLayer = null;
                    IFeatureWorkspace workspace = null;

                    while ((featLayer = featureLayers.Next() as IFeatureLayer) != null)
                    {
                        try
                        {
                            if (((IDataset)(featLayer.FeatureClass)).BrowseName.ToUpper() == parameters[0].ToUpper())
                            {
                                IVersion pversion = ((IDataset)(featLayer.FeatureClass)).Workspace as IVersion;
                                this.Logger.WriteLine(pversion.VersionName);
                                this.FeatureClass = featLayer.FeatureClass;
                                this.Workspace = ((IDataset)(featLayer.FeatureClass)).Workspace;
                                workspace = ((IDataset)(featLayer.FeatureClass)).Workspace as IFeatureWorkspace;
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                        }
                    }


                    this.SW1.Reset();
                    this.SW1.Start();


                    IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;

                    IFeature pfeature = this.FeatureClass.GetFeature(Convert.ToInt32(parameters[1]));

                    int annoClassID = Convert.ToInt32(pfeature.get_Value(pfeature.Fields.FindField("ANNOTATIONCLASSID")));
                    IPoint newPlacementPoint;
                    IPolygon annoPoly = pfeature.Shape as IPolygon;
                    IArea area = annoPoly as IArea;

                    newPlacementPoint = area.Centroid;

                    IEnumRelationshipClass relClasses = pfeature.Class.get_RelationshipClasses(esriRelRole.esriRelRoleAny);
                    IRelationshipClass rel = relClasses.Next();
                    //Loop through and delete.
                    if (rel != null)
                    {
                        IObject parentObject = AnnotationFacade.FindAnnoParent(pfeature, rel);

                        this.Logger.WriteLine(parentObject.OID);
                        if (parentObject == null)
                        {
                            this.Logger.WriteLine("Could not find parent feature for this object.");
                            return false;
                        }

                        workspaceEdit.StartEditOperation();

                        AnnotationFacade.RecreateAnno(parentObject, rel);
                        AnnotationFacade.ApplySettingsToNewAnno(rel, parentObject, 1, 0, annoClassID, newPlacementPoint);

                        workspaceEdit.StopEditOperation();

                        ((IMxDocument)variable.Document).ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                    }

                    this.SW1.Stop();
                    this.Logger.WriteLine(string.Concat("RECREATEANNO:", this.SW1.ElapsedMilliseconds), false, null);
                    PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));
                    flag = true;
                }
                catch (Exception exception3)
                {
                    exception = exception3;
                    this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                    flag = false;
                }
            }
            catch (Exception exception4)
            {
                exception = exception4;
                this.Logger.WriteLine(string.Concat("UpdateStringAttribute error:", exception.Message, " ", exception.StackTrace), false, null);
                flag = false;
            }
            return flag;
        }


        public void CreateTransformer(double X, double Y)
        {
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            Dictionary<int, object> fieldMapping_Create = new Dictionary<int, object>();
            UID featureUID = new UID();
            featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer featureLayers = ((IMxDocument)variable.Document).FocusMap.get_Layers(featureUID, true);
            int iCount_ClassGot = 0;
            IFeatureClass pFClass_Transformer = null;
            IObjectClass pOClass_Related = null;
            ILayer pLayer = null;
            try
            {
                //Finding the Feature Class for the NewFeature and Where to locate Feature
                while ((pLayer = featureLayers.Next()) != null)
                {
                    if (((IDataset)((IFeatureLayer)pLayer).FeatureClass).Name.ToUpper() == "EDGIS.TRANSFORMER".ToUpper())
                    { pFClass_Transformer = ((IFeatureLayer)pLayer).FeatureClass; ++iCount_ClassGot; }

                    else continue;
                    if (iCount_ClassGot == 1) break;
                }

                this.SW1.Restart();
                this.SW1.Start();

                IFeatureWorkspace fworkspace = ((IDataset)pFClass_Transformer).Workspace as IFeatureWorkspace;

                IWorkspaceEdit we = fworkspace as IWorkspaceEdit;

                we.StartEditOperation();

                //pOClass_Related = (IObjectClass)((IFeatureWorkspace)((IFeatureLayer)((IMxDocument)ArcMap.Application.Document).FocusMap.get_Layer(2)).FeatureClass.FeatureDataset.Workspace).OpenTable("EDGIS.TRANSFORMERUNIT");
                pOClass_Related = (IObjectClass)fworkspace.OpenTable("EDGIS.TRANSFORMERUNIT");

                fieldMapping_Create.Clear();
                //iSUBTYPECD = new int[] { 1 };//{ 1, 2, 3 };
                //iSTATUS = new short[] { 0, 2, 3, 5, 30 };
                //Creating Transformerunit Feature
                fieldMapping_Create.Add(pOClass_Related.FindField("SubtypeCD"), 1);
                fieldMapping_Create.Add(pOClass_Related.FindField("STATUS"), 5);
                fieldMapping_Create.Add(pOClass_Related.FindField("TRANSFORMERTYPE"), "01");
                fieldMapping_Create.Add(pOClass_Related.FindField("MANUFACTURER"), "AB");
                fieldMapping_Create.Add(pOClass_Related.FindField("RATEDKVA"), 300);
                fieldMapping_Create.Add(pOClass_Related.FindField("INSTALLJOBNUMBER"), "1234");

                IObject pObject_Related = null; //CreateObject(variable, pOClass_Related, fieldMapping_Create);                
                pObject_Related = ((ITable)pOClass_Related).CreateRow() as IObject;

                //Setting the Fields through Field Mapping dictionary
                foreach (KeyValuePair<int, object> kvp in fieldMapping_Create)
                {
                    if (kvp.Value != null)
                        pObject_Related.set_Value(kvp.Key, kvp.Value);
                }

                pObject_Related.Store();

                fieldMapping_Create.Clear();

                //Creating Feature


                //Settings Manadatory Not Null Fields
                Random pRandom = new Random();

                int[] iSUBTYPECD = new int[] { 1 },//{ 1, 2, 3, 4, 7 },
                    iNUMBEROFPHASES = new int[] { 1, 2, 3 },
                    iOPERATINGVOLTAGE = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                    iUNITCOUNT = new int[] { 0, 1, 2, 3, 4, 5, 6 },
                    iLOWSIDEVOLTAGE = new int[] { 25, 30, 27, 29, 99, 24, 22, 23, 20, 26, 21, 28, 31, 33, 0 };
                short[] iSTATUS = new short[] { 0, 1, 2, 3, 5, 30 };

                string[] sLOCALOFFICEID = new string[] { "XL", "XB", "TQ", "TJ", "XN", "TX", "TT", "TF", "TV", "93ZZ", "TN", "TC", "F93", "TG", "XV", "TL", "JR", "XT", "TM", "TR", "J60", "TK", "XH", "XR", "XD", "TS", "P72" },
                    sSUBWAYIDC = new string[] { "Y", "N" },
                    sINTERRUPTERIDC = new string[] { "Y", "N" },
                    sAUTOIDC = new string[] { "Y", "N" },
                    sINSTALLATIONTYPE = new string[] { "UG", "NEW", "PAD", "SUB", "OH" };
                short iStatus = 0;
                int iSubtypeCD = 0, iINSTALLJOBNUMBER = 0;

                fieldMapping_Create.Add(pFClass_Transformer.FindField("SubtypeCD"), 1);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("STATUS"), 5);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("NUMBEROFPHASES"), 3);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("OPERATINGVOLTAGE"), 1);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("UNITCOUNT"), 1);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("LOCALOFFICEID"), "XL");
                fieldMapping_Create.Add(pFClass_Transformer.FindField("CGC12"), 111000111001);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("INSTALLJOBNUMBER"), "1234");

                fieldMapping_Create.Add(pFClass_Transformer.FindField("OPERATINGNUMBER"), "T1234");
                fieldMapping_Create.Add(pFClass_Transformer.FindField("LOWSIDEVOLTAGE"), 25);
                fieldMapping_Create.Add(pFClass_Transformer.FindField("SUBWAYIDC"), "Y");
                fieldMapping_Create.Add(pFClass_Transformer.FindField("AUTOIDC"), "Y");
                fieldMapping_Create.Add(pFClass_Transformer.FindField("INTERRUPTERIDC"), "Y");
                fieldMapping_Create.Add(pFClass_Transformer.FindField("INSTALLATIONTYPE"), "OH");

                IObject pObject_Main = CreateFeature(variable, pFClass_Transformer, fieldMapping_Create, X, Y);


                //Creating Relationship
                pOClass_Related.get_RelationshipClasses(esriRelRole.esriRelRoleDestination).Next().CreateRelationship(pObject_Related, pObject_Main);
                fieldMapping_Create.Clear();
                we.StopEditOperation();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("CREATE_TRANSFORMER:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam

            }
            catch (Exception exception)
            {
                this.Logger.WriteLine(string.Concat("CREATE_TRANSFORMER error:", exception.Message, " ", exception.StackTrace), false, null);
            }
        }

        private IObject CreateFeature(IApplication variable, IFeatureClass pFClass_Create, Dictionary<int, object> fieldMapping_Create, double X, double Y)
        {
            try
            {
                IFeature pFeature_Create = default(IFeature);

                //setting editor object
                IEditor pEditor = variable.FindExtensionByCLSID(new UID() { Value = "esriEditor.Editor" }) as IEditor;

                //pEditor.StartEditing(this.Workspace); //Commented bcoz it is expected that session is already opened               
                //this.Workspaceedit.StartEditOperation(); 
                //pEditor.StartOperation();
                //Getting the features for New and Where location

                pFeature_Create = pFClass_Create.CreateFeature();


                //pFeature_Create.Shape = (pFClass_Create.ShapeType == esriGeometryType.esriGeometryPoint) ? (IGeometry)(IPoint)new Point() { X = X, Y = Y } : (IGeometry)new Line() { FromPoint = new Point() { X = X, Y = Y }, ToPoint = new Point() { X = X + 10, Y = Y + 10 } };
                if (pFClass_Create.ShapeType == esriGeometryType.esriGeometryPoint)
                {
                    pFeature_Create.Shape = new Point() { X = X, Y = Y };
                }
                else
                {
                    pFeature_Create.Shape = new PolylineClass() { FromPoint = (IPoint)new Point() { X = X, Y = Y }, ToPoint = (IPoint)new Point() { X = X + 100, Y = Y + 100 } };
                }


                //Setting the Fields through Field Mapping dictionary
                foreach (KeyValuePair<int, object> kvp in fieldMapping_Create)
                {
                    if (kvp.Value != null)
                        pFeature_Create.set_Value(kvp.Key, kvp.Value);
                }

                // pFeature_Create.set_Value();

                pFeature_Create.Store();
                //this.Workspaceedit.StopEditOperation(); 
                //pEditor.StopOperation(pFClass_Create.AliasName + " Creation");
                return pFeature_Create;
            }
            catch (Exception exception)
            {
                this.Logger.WriteLine(string.Concat("CreateFeature error:", exception.Message, " ", exception.StackTrace), false, null);
                return default(IFeature);
            }
        }

        private IObject CreateObject(IApplication variable, IObjectClass pOClass_TransformerUnit, Dictionary<int, object> fieldMapping_Create)
        {
            IRow pObj_TransUnit = default(IRow);
            try
            {
                //setting editor object
                //IEditor pEditor = variable.FindExtensionByCLSID(new UID() { Value = "esriEditor.Editor" }) as IEditor;

                //pEditor.StartEditing(this.Workspace); //Commented bcoz it is expected that session is already opened     



                //this.Workspaceedit.StartEditOperation();  
                this.Logger.WriteLine("1");
                pObj_TransUnit = ((ITable)pOClass_TransformerUnit).CreateRow();
                this.Logger.WriteLine("2");
                //Setting the Fields through Field Mapping dictionary
                foreach (KeyValuePair<int, object> kvp in fieldMapping_Create)
                {
                    if (kvp.Value != null)
                        pObj_TransUnit.set_Value(kvp.Key, kvp.Value);
                }

                pObj_TransUnit.Store();

                //this.Workspaceedit.StopEditOperation(); 
                //pEditor.StopOperation(pOClass_TransformerUnit.AliasName + " Creation");
                return (IObject)pObj_TransUnit;
            }
            catch (Exception exception)
            {
                this.Logger.WriteLine(string.Concat("CreateObject error:", exception.Message, " ", exception.StackTrace), false, null);
                return default(IFeature);
            }

        }

        private IObject CreateFeature(IApplication variable, IFeatureClass pFClass_Create, Dictionary<int, object> fieldMapping_Create, IFeatureClass pFClass_Where, int iWhereOID)
        {
            try
            {
                IFeature pFeature_Create = default(IFeature), pFeature_Where = default(IFeature);

                //setting editor object
                IEditor pEditor = variable.FindExtensionByCLSID(new UID() { Value = "esriEditor.Editor" }) as IEditor;

                //pEditor.StartEditing(this.Workspace); //Commented bcoz it is expected that session is already opened               

                pEditor.StartOperation();
                //Getting the features for New and Where location
                pFeature_Where = pFClass_Where.GetFeature(iWhereOID);
                pFeature_Create = pFClass_Create.CreateFeature();

                //Setting the shape of new feature
                switch (pFeature_Where.Shape.GeometryType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        pFeature_Create.Shape = (pFClass_Create.ShapeType == esriGeometryType.esriGeometryPoint) ? pFeature_Where.ShapeCopy : (IPolyline)new Line() { FromPoint = ((IPoint)pFeature_Where.ShapeCopy), ToPoint = new Point() { X = ((IPoint)pFeature_Where.ShapeCopy).X + 10.0, Y = ((IPoint)pFeature_Where.ShapeCopy).Y + 10.0 } };
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        pFeature_Create.Shape = (pFClass_Create.ShapeType == esriGeometryType.esriGeometryPolyline) ? pFeature_Where.ShapeCopy : ((IPolyline)pFeature_Where.ShapeCopy).ToPoint;
                        break;
                    default:
                        break;
                }

                //Setting the Fields through Field Mapping dictionary
                foreach (KeyValuePair<int, object> kvp in fieldMapping_Create)
                {
                    if (kvp.Value != null)
                        pFeature_Create.set_Value(kvp.Key, kvp.Value);
                }

                // pFeature_Create.set_Value();

                pFeature_Create.Store();
                pEditor.StopOperation(pFClass_Create.AliasName + " Creation");
                return pFeature_Create;
            }
            catch (Exception exception)
            {
                // this.Logger.WriteLine(string.Concat("CreateFeature error:", exception.Message, " ", exception.StackTrace), false, null);
                return default(IFeature);
            }
        }

        public void CreateSecUGConductor(double FromX, double FromY, double ToX, double ToY)
        {
            ToX = (ToX == 0) ? FromX + 10 : ToX;
            ToY = (ToY == 0) ? FromY + 10 : ToY;
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            Dictionary<int, object> fieldMapping_Create = new Dictionary<int, object>();
            UID featureUID = new UID();
            featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer featureLayers = ((IMxDocument)variable.Document).FocusMap.get_Layers(featureUID, true);
            int iCount_ClassGot = 0;
            IFeatureClass pFClass_Conductor = null;
            IObjectClass pOClass_Related = null;
            ILayer pLayer = null;
            try
            {
                //Finding the Feature Class for the NewFeature and Where to locate Feature
                while ((pLayer = featureLayers.Next()) != null)
                {
                    //if (((IDataset)((IFeatureLayer)pLayer).FeatureClass).Name.ToUpper() == "EDGIS.TRANSFORMER".ToUpper())
                    //{ pFClass_Transformer = ((IFeatureLayer)pLayer).FeatureClass; ++iCount_ClassGot; }
                    if (((IDataset)((IFeatureLayer)pLayer).FeatureClass).Name.ToUpper() == "EDGIS.SECUGCONDUCTOR".ToUpper())
                    { pFClass_Conductor = ((IFeatureLayer)pLayer).FeatureClass; ++iCount_ClassGot; }
                    //else if (((IDataset)((IFeatureLayer)pLayer).FeatureClass).Name.ToUpper() == sFClass_Where.ToUpper())
                    //{ pFClass_Where = ((IFeatureLayer)pLayer).FeatureClass; ++iCount_ClassGot; }
                    else continue;
                    if (iCount_ClassGot == 1) break;
                }

                /// For Cable
                /// 
                this.SW1.Restart();
                this.SW1.Start();

                IFeatureWorkspace pfw = ((IDataset)((IFeatureLayer)pLayer).FeatureClass).Workspace as IFeatureWorkspace;
                IWorkspaceEdit we = pfw as IWorkspaceEdit;

                we.StartEditOperation();

                // ConductorInfo Creation
                pOClass_Related = (IObjectClass)pfw.OpenTable("EDGIS.SECUGCONDUCTORINFO");
                fieldMapping_Create.Clear();

                //Creating Transformerunit Feature
                fieldMapping_Create.Add(pOClass_Related.FindField("SubtypeCD"), 1);
                fieldMapping_Create.Add(pOClass_Related.FindField("PHASEDESIGNATION"), 4);

                fieldMapping_Create.Add(pOClass_Related.FindField("CONDUCTORSIZE"), "300");
                fieldMapping_Create.Add(pOClass_Related.FindField("MATERIAL"), "C");
                fieldMapping_Create.Add(pOClass_Related.FindField("CONDUCTORCOUNT"), 3);
                fieldMapping_Create.Add(pOClass_Related.FindField("CONDUCTORTYPE"), "DPX");

                //ConductorInfo Creation
                //IObject pObject_Related = CreateObject(variable, pOClass_Related, fieldMapping_Create);

                IObject pObject_Related = null; //CreateObject(variable, pOClass_Related, fieldMapping_Create);

                pObject_Related = ((ITable)pOClass_Related).CreateRow() as IObject;
                //Setting the Fields through Field Mapping dictionary
                foreach (KeyValuePair<int, object> kvp in fieldMapping_Create)
                {
                    if (kvp.Value != null)
                        pObject_Related.set_Value(kvp.Key, kvp.Value);
                }

                pObject_Related.Store();

                fieldMapping_Create.Clear();

                int[] iCONSTRUCTIONTYPE = new int[] { 4, 5, 6, 7, 8, 9 },
                    iSUBTYPECD = new int[] { 3 },// { 6, 7, 5, 1, 2, 3, 4 },
                iOPERATINGVOLTAGE = new int[] { 25, 8, 5, 30, 27, 29, 99, 24, 22, 23, 20, 26, 31, 21, 28, 33 };
                short[] iSTATUS = new short[] { 60, 5, 30, 3, 2, 1, 0, 10 };

                Random pRandom = new Random();

                string[] sLOCALOFFICEID = new string[] { "11ZZ", "1ZZ", "24ZZ", "66ZZ", "70ZZ", "78ZZ", "80ZZ", "91ZZ", "BG", "BJ", "BM", "BR", "BT", "BV", "BX", "D56", "F20", "F9", "F93", "FB", "FF", "FM", "FQ", "HB", "HM", "HP", "HS", "J48", "J60", "JC", "JG", "JJ", "JK", "JL", "JN", "JQ", "JR", "LF", "LM", "NC", "NF", "NJ", "NL", "NN", "NQ", "NR", "NV", "P26", "P48", "P56", "P72", "PX", "PY", "RB", "RG", "RH", "RN", "RZ", "TC", "TF", "TG", "TJ", "TK", "TL", "TM", "TN", "TQ", "TR", "TS", "TT", "TV", "TX", "VF", "VJ", "VP", "VT", "VV", "W30", "X76", "XB", "XD", "XH", "XJ", "XL", "XN", "XR", "XT", "XV" };
                int iStatus = 0, iSubtypeCD = 0;

                fieldMapping_Create.Add(pFClass_Conductor.FindField("SubtypeCD"), iSubtypeCD = iSUBTYPECD[pRandom.Next(0, iSUBTYPECD.Length)]);
                fieldMapping_Create.Add(pFClass_Conductor.FindField("STATUS"), 5);
                fieldMapping_Create.Add(pFClass_Conductor.FindField("OPERATINGVOLTAGE"), iOPERATINGVOLTAGE[pRandom.Next(0, iOPERATINGVOLTAGE.Length)]);
                fieldMapping_Create.Add(pFClass_Conductor.FindField("CONSTRUCTIONTYPE"), iCONSTRUCTIONTYPE[pRandom.Next(0, iCONSTRUCTIONTYPE.Length)]);
                fieldMapping_Create.Add(pFClass_Conductor.FindField("LOCALOFFICEID"), sLOCALOFFICEID[pRandom.Next(0, sLOCALOFFICEID.Length)]);

                fieldMapping_Create.Add(pFClass_Conductor.FindField("MEASUREDLENGTH"), pRandom.Next(1, 1000));
                fieldMapping_Create.Add(pFClass_Conductor.FindField("INSTALLJOBNUMBER"), pRandom.Next(10000000, 99999999));

                // Feature Conductor Creation
                IObject pObject_Main = CreateFeature(variable, pFClass_Conductor, fieldMapping_Create, FromX, FromY);
                pObject_Main.Store();
                this.Logger.WriteLine(pObject_Main.OID);

                this.Logger.WriteLine(pObject_Related.OID);
                //Relationship Creation
                IEnumRelationshipClass pERClass = pOClass_Related.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
                IRelationshipClass pRClass = default(IRelationshipClass);
                while ((pRClass = pERClass.Next()) != null)
                {
                    if (pRClass.OriginClass.AliasName == pFClass_Conductor.AliasName)
                    {
                        pRClass.CreateRelationship(pObject_Related, pObject_Main);
                        break;
                    }
                }

                we.StopEditOperation();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("CREATE_CONDUCTOR:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam

            }
            catch (Exception ex)
            {
                this.Logger.WriteLine(string.Concat("CREATE_CONDUCTOR error:", ex.Message, " ", ex.StackTrace), false, null);
            }
        }

        private void MoveVertex(int iOID, string sFClass_Name, int iVertexNumber, double dVertexX, double dVertexY)
        {
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            Dictionary<int, object> fieldMapping_Create = new Dictionary<int, object>();
            UID featureUID = new UID();
            featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer featureLayers = ((IMxDocument)variable.Document).FocusMap.get_Layers(featureUID, true);
            int iCount_ClassGot = 0;
            IFeatureClass pFClass_Polyline = default(IFeatureClass);//, pFClass_Where = default(IFeatureClass);
            IFeature pFeat_Polyline = default(IFeature);
            ILayer pLayer = default(ILayer);
            try
            {
                //Finding the Feature Class for the NewFeature and Where to locate Feature
                while ((pLayer = featureLayers.Next()) != null)
                {
                    if (((IDataset)((IFeatureLayer)pLayer).FeatureClass).Name.ToUpper() == sFClass_Name.ToUpper())
                    { pFClass_Polyline = ((IFeatureLayer)pLayer).FeatureClass; ++iCount_ClassGot; }
                    if (iCount_ClassGot == 1) break;
                    continue;
                }
                if (pFClass_Polyline.ShapeType != esriGeometryType.esriGeometryPolyline) return;
                pFeat_Polyline = pFClass_Polyline.GetFeature(iOID);
                this.SW1.Reset();
                this.SW1.Start();
                //setting editor object
                IEditor pEditor = variable.FindExtensionByCLSID(new UID() { Value = "esriEditor.Editor" }) as IEditor;

                //pEditor.StartEditing(this.Workspace); //Commented bcoz it is expected that session is already opened               
                //pEditor.StartOperation();

                IFeatureWorkspace pfw = ((IDataset)((IFeatureLayer)pLayer).FeatureClass).Workspace as IFeatureWorkspace;
                IWorkspaceEdit we = pfw as IWorkspaceEdit;

                we.StartEditOperation();

                IPoint pPoint_Vertex = ((IPointCollection4)pFeat_Polyline.Shape).get_Point(iVertexNumber);


                ((ITransform2D)pPoint_Vertex).Move(dVertexX, dVertexY);
                pPoint_Vertex.X = dVertexX;
                pPoint_Vertex.Y = dVertexY;
                ((IPointCollection4)pFeat_Polyline.Shape).UpdatePoint(iVertexNumber, pPoint_Vertex);
                pFeat_Polyline.Store();
                we.StopEditOperation();
                //pEditor.StopOperation("Vertex Moved for " + sFClass_Name + ": " + iOID);
                ((IMxDocument)variable.Document).ActivatedView.Refresh();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("MOVEVERTEX:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            }
            catch (Exception ex)
            {
                this.Logger.WriteLine(string.Concat("Error in MOVEVERTEX:", ex.Message), false, null);
            }
        }

        private void DELETEFEATURE(string sFClass_Name, int oid)
        {
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            Dictionary<int, object> fieldMapping_Create = new Dictionary<int, object>();
            UID featureUID = new UID();
            featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
            IEnumLayer featureLayers = ((IMxDocument)variable.Document).FocusMap.get_Layers(featureUID, true);
            int iCount_ClassGot = 0;
            IFeatureClass pFClass_Polyline = null;
            IFeature pFeat_Polyline = null;
            ILayer pLayer = null;
            try
            {
                //Finding the Feature Class for the NewFeature and Where to locate Feature
                while ((pLayer = featureLayers.Next()) != null)
                {
                    if (((IDataset)((IFeatureLayer)pLayer).FeatureClass).Name.ToUpper() == sFClass_Name.ToUpper())
                    { pFClass_Polyline = ((IFeatureLayer)pLayer).FeatureClass; ++iCount_ClassGot; }
                    if (iCount_ClassGot == 1) break;
                    continue;
                }

                pFeat_Polyline = pFClass_Polyline.GetFeature(oid);
                this.SW1.Reset();
                this.SW1.Start();
                //setting editor object
                IEditor pEditor = variable.FindExtensionByCLSID(new UID() { Value = "esriEditor.Editor" }) as IEditor;

                //pEditor.StartEditing(this.Workspace); //Commented bcoz it is expected that session is already opened               
                IFeatureWorkspace pfw = ((IDataset)pFClass_Polyline).Workspace as IFeatureWorkspace;
                IWorkspaceEdit pwe = pfw as IWorkspaceEdit;

                //pEditor.StartOperation();
                pwe.StartEditOperation();
                pFeat_Polyline.Delete();
                pwe.StopEditOperation();
                //pEditor.StopOperation("Deleted the feature for " + sFClass_Name + ": " + oid);
                ((IMxDocument)variable.Document).ActivatedView.Refresh();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("DELETE_FEATURE:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            }
            catch (Exception ex)
            {
                this.Logger.WriteLine(string.Concat("Error in DELETE FEATURE:", ex.Message), false, null);
            }
        }


        private void AssetReplace(string sFeatClassName, int iOID)
        {
            UID uIDClass = new UID();
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;


            UID featureUID = new UID();
            featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";

            IMap pMap = ((IMxDocument)variable.Document).FocusMap;
            this.Logger.WriteLine(pMap.Name);
            IEnumLayer pELayer = pMap.get_Layers(featureUID, true);
            this.Logger.WriteLine(pMap.Name);
            ILayer pLayer = null;
            IObject oldObject = null;
            IObject newObject = null;
            IObjectClass objectClass = null;

            Dictionary<int, object> fieldMapping = null;
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = null;
            IEditor _editor = null;

            if (_invalidArea == null)
            {
                _invalidArea = new InvalidAreaClass();
            }

            try
            {

                IFeatureLayer featLayer = null;
                IFeatureWorkspace workspace = null;


                while ((featLayer = pELayer.Next() as IFeatureLayer) != null)
                {
                    try
                    {
                        if (((IDataset)(featLayer.FeatureClass)).BrowseName.ToUpper() == sFeatClassName.ToUpper())
                        {
                            IVersion pversion = ((IDataset)(featLayer.FeatureClass)).Workspace as IVersion;
                            this.Logger.WriteLine(pversion.VersionName);
                            this.Workspace = ((IDataset)(featLayer.FeatureClass)).Workspace;
                            workspace = ((IDataset)(featLayer.FeatureClass)).Workspace as IFeatureWorkspace;
                            oldObject = (IObject)((IFeatureLayer)featLayer).FeatureClass.GetFeature(iOID);
                            break;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

                this.SW1.Reset();
                this.SW1.Start();

                if (oldObject == null) throw new Exception("OID: " + iOID.ToString() + " not found for feature class " + sFeatClassName);
                objectClass = oldObject.Class;

                uIDClass.Value = "esriEditor.Editor";
                _editor = variable.FindExtensionByCLSID(uIDClass) as IEditor;

                if (_editor == null) throw new Exception("Editor cannot be initialized!!!");

                fieldMapping = GetValuesToMap(oldObject);

                //Update relationships from old feature to new feature
                relationshipMapping = GetRelationshipMapping(oldObject);


                //Execute the out of the box abandon and remove functionality
                //OOTBAbandonAndRemove.Execute(enumListItems, pSelection.Count);
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)workspace;
                workspaceEdit.StartEditOperation();
                //_editor.StartOperation();

                DeleteObject(oldObject);

                //Set flag to indicate Disable Symbol Number AU 
                //EvaluationEngine.Instance.EnableSymbolNumberAU = false; //Shashwat

                //Create our new feature
                newObject = CreateNewObject(fieldMapping, objectClass, oldObject);

                //Set all relationships back up with the new feature
                SetRelationships(relationshipMapping, newObject);

                //Enable Undo/Redo button in ArcMap
                IMxDocument mxDoc = _editor.Parent.Document as IMxDocument;
                mxDoc.UpdateContents();

                //_editor.StopOperation("PGE - Replace Asset");
                workspaceEdit.StartEditOperation();
                //Relate the new feature with the newly created abandoned row if possible
                //RelatedAbandonedRow(newFeature);

                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("ASSETREPLACE:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam

            }
            catch (Exception ex)
            {
                this.Logger.WriteLine(string.Concat("Error in AssetReplace: ", ex.Message, ":", ex.StackTrace), false, null);
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
        /// This method will take all relationships that are current assigned to the oldFeature, remove
        /// them and then apply them to the new feature.
        /// </summary>
        /// <param name="oldObject">Feature being replaced</param>
        private Dictionary<IRelationshipClass, List<IObject>> GetRelationshipMapping(IObject oldObject) //IFeature oldFeature
        {
            Dictionary<IRelationshipClass, List<IObject>> relationshipMapping = new Dictionary<IRelationshipClass, List<IObject>>();
            //IFeatureClass featClass = oldFeature.Class as IFeatureClass;
            IObjectClass objClass = oldObject.Class as IObjectClass;

            //Process relationships where this class is the destination
            IEnumRelationshipClass relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleDestination);
            relClasses.Reset();
            IRelationshipClass relClass = relClasses.Next();
            while (relClass != null)
            {
                //Don't map annotation relationships
                if (relClass.OriginClass is IFeatureClass)
                {
                    IFeatureClass originClass = relClass.OriginClass as IFeatureClass;
                    if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        relClass = relClasses.Next();
                        continue;
                    }
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
                    //relClass.CreateRelationship(obj, newFeature);
                }
                relClass = relClasses.Next();
            }

            //Process relationships where this class is the origin
            relClasses = objClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);
            relClasses.Reset();
            relClass = relClasses.Next();
            while (relClass != null)
            {
                //Don't map annotation relationships
                if (relClass.DestinationClass is IFeatureClass)
                {
                    IFeatureClass originClass = relClass.DestinationClass as IFeatureClass;
                    if (originClass.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        relClass = relClasses.Next();
                        continue;
                    }
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
                    //relClass.CreateRelationship(obj, newFeature);
                }
                relClass = relClasses.Next();
            }

            return relationshipMapping;
        }

        private object _replacedFeatureGUIDValue = null;
        private IGeometry _replacedFeatureShape = null;
        private IInvalidArea _invalidArea = null;

        private IFeature _newFeature = null;

        /// <summary>
        /// This method delets a feature
        /// </summary>
        /// <param name="origFeature"></param>
        private void DeleteObject(IObject objectToDelete) //IFeature feature
        {
            IFeatureEdit featureEdit = null;
            ISet deleteSet = null;

            int GUIDIndex = objectToDelete.Fields.FindField(SchemaInfo.Electric.GlobalID);
            _replacedFeatureGUIDValue = objectToDelete.get_Value(GUIDIndex);

            if (objectToDelete is IFeature)
            {
                _replacedFeatureShape = (objectToDelete as IFeature).ShapeCopy;

                deleteSet = new ESRI.ArcGIS.esriSystem.SetClass();
                deleteSet.Add(objectToDelete);
                deleteSet.Reset();

                featureEdit = deleteSet.Next() as IFeatureEdit;
                featureEdit.DeleteSet(deleteSet);

                _invalidArea.Add(objectToDelete);
            }
            else
            {
                objectToDelete.Delete();
            }

        }

        /// <summary>
        /// This method will create the new feature based off of the configured values to map
        /// from the original feature
        /// </summary>
        /// <param name="origFeature"></param>
        private IObject CreateNewObject(Dictionary<int, object> fieldMapping, IObjectClass objectClass, IObject replacedObject) //Dictionary<int, object> fieldMapping, IFeatureClass featClass, IFeature replacedFeature
        {
            IObject newObject = null;

            if (objectClass is IFeatureClass)
            {
                IFeature feat = (objectClass as IFeatureClass).CreateFeature();

                foreach (KeyValuePair<int, object> kvp in fieldMapping)
                {
                    if (kvp.Value != null)
                        feat.set_Value(kvp.Key, kvp.Value);
                }

                feat.Shape = _replacedFeatureShape;

                int ReplaceGUIDIndex = ModelNameFacade.FieldIndexFromModelName(feat.Class, SchemaInfo.General.FieldModelNames.ReplaceGUID);
                feat.set_Value(ReplaceGUIDIndex, _replacedFeatureGUIDValue);

                int jobid = ModelNameFacade.FieldIndexFromModelName(feat.Class, SchemaInfo.General.FieldModelNames.JobNumber);
                feat.set_Value(jobid, "1234");

                feat.Store();

                newObject = _newFeature = feat;
            }
            else
            {
                IRow newRow = (objectClass as ITable).CreateRow();
                foreach (KeyValuePair<int, object> kvp in fieldMapping)
                {
                    if (kvp.Value != null)
                        newRow.set_Value(kvp.Key, kvp.Value);
                }

                int ReplaceGUIDIndex = ModelNameFacade.FieldIndexFromModelName(objectClass, SchemaInfo.General.FieldModelNames.ReplaceGUID);
                newRow.set_Value(ReplaceGUIDIndex, _replacedFeatureGUIDValue);

                newRow.Store();

                newObject = newRow as IObject;
            }
            ////_newFeatureList.Add(feat);

            return newObject; //feat
        }

        /// <summary>
        /// Relates all features that were related with the previous feature to the newly created feature
        /// </summary>
        /// <param name="relationshipMapping"></param>
        /// <param name="newObject"></param>
        private void SetRelationships(Dictionary<IRelationshipClass, List<IObject>> relationshipMapping, IObject newObject) //, IFeature newFeature
        {
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
                            kvp.Key.CreateRelationship(newObject, obj);
                        }
                    }
                }
                else
                {
                    foreach (IObject obj in kvp.Value)
                    {
                        if (obj != null)
                        {
                            kvp.Key.CreateRelationship(obj, newObject);
                        }
                    }
                }
            }
        }
    }
}