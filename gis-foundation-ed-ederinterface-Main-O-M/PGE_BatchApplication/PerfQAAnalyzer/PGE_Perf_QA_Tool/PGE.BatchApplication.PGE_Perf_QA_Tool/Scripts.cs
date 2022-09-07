using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using ESRI.ScriptEngine;
using Miner.Interop;
using System.Runtime.InteropServices;

namespace PGE.BatchApplication.PGE_Perf_QA_Tool
{
    [Export(typeof(IScriptCommand))]
    public class Scripts : IScriptCommand
    {
        private CommandParser _parser;
        private Stopwatch SW1;
        private IApplication m_pApp;
        public ESRI.ScriptEngine.Logger Logger
        {
            get;
            set;
        }


        private ESRI.ArcGIS.Carto.IActiveViewEvents_AfterDrawEventHandler m_ActiveViewEventsAfterDraw;
        private ESRI.ArcGIS.Carto.IActiveViewEvents_ViewRefreshedEventHandler m_ActiveViewEventsViewRefreshed;
        public Scripts()
        {
            this._parser = new CommandParser()
            {
                CommentIdentifier = "//"
            };
            this.Logger = new ESRI.ScriptEngine.Logger();
            this.SW1 = new Stopwatch();
        }
        private static int start = 0;
        private static int finish = 0;
        private bool refreshed = false;
        private static string toolname;
        private void OnActiveViewEventsAfterDraw(ESRI.ArcGIS.Display.IDisplay display, ESRI.ArcGIS.Carto.esriViewDrawPhase phase)
        {
            // TODO: Add your code here
            // System.Windows.Forms.MessageBox.Show("ViewRefreshed");
            start = 1;

            //start++;
            //this.Logger.WriteLine("START-" + start + "-" + phase.ToString());

            if (phase == esriViewDrawPhase.esriViewForeground)
            {
                activeViewEvents.AfterDraw -= m_ActiveViewEventsAfterDraw;
                activeViewEvents.ViewRefreshed -= m_ActiveViewEventsViewRefreshed;
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat(toolname.ToUpper() + ":", this.SW1.ElapsedMilliseconds), false, null);
                this.Logger.WriteLine(DateTime.Now.ToString());
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                refreshed = true;
                //System.Threading.Thread.Sleep(3000);
            }
        }

        private void OnActiveViewEventsViewRefreshed(ESRI.ArcGIS.Carto.IActiveView view, ESRI.ArcGIS.Carto.esriViewDrawPhase phase, System.Object data, ESRI.ArcGIS.Geometry.IEnvelope envelope)
        {
            start = 1;
        }

        public int Execute(string command)
        {
            string str;
            string[] parameters;
            string[] parameterGroups;
            string[] strArrays;
            string[] parameters1;
            int num = 3;
            string verb = this._parser.GetVerb(command, true);
            if (command.Length <= verb.Length)
            {
                str = "";
            }
            else
            {
                try
                {
                    str = command.Substring(verb.Length + 1);
                }
                catch
                {
                    str = "";
                }
            }

            try
            {
                string str1 = verb.Trim();
                if (str1 != null)
                {
                    //For Summary Log- Shashwat Nigam
                    PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log = str1;
                    toolname = str1;
                    //this.Logger.WriteLine("Running Tool:" + toolname + " - " + DateTime.Now.ToString());
                    switch (str1)
                    {
                        case "ZOOMTOSELECTED":
                            {
                                this.ZOOMTOSELECTED();
                                num = 3;
                                return num;
                            }

                        case "CLEARSELECTION":
                            {
                                this.CLEARSELECTION();
                                num = 3;
                                return num;
                            }

                        case "SETSCALE":
                            {
                                parameters = this.GetParameters(str);
                                this.SetScale(parameters[0]);
                                num = 3;
                                return num;
                            }
                        case "WRITELINE":
                            {
                                parameters = this.GetParameters(str);
                                this.Logger.WriteLine(parameters[0]);
                                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log = parameters[0].Split(':')[0].Trim();//For Summary Log- Shashwat Nigam
                                num = 3;
                                return num;
                            }
                        case "PAN":
                            {
                                parameters = this.GetParameters(str);
                                this.PAN(parameters[0]);
                                num = 3;
                                return num;
                            }
                        case "REFRESH":
                            {
                                parameters = this.GetParameters(str);

                                this.Refresh();
                                num = 3;
                                return num;
                            }
                        case "COMMANDBUTTON":
                            {
                                parameters = this.GetParameters(str);
                                this.CommandClick(parameters[0]);
                                num = 3;
                                return num;
                            }

                        case "CREATEVERSION":
                            {
                                parameters = this.GetParameters(str);
                                this.CreateVersion(parameters[0]);
                                num = 3;
                                return num;
                            }
                        case "CHANGEVERSION":
                            {
                                parameters = this.GetParameters(str);
                                this.ChangeVersion(parameters[0]);
                                num = 3;
                                return num;
                            }

                        case "ZOOMFULL":
                            {
                                this.ZoomToFullExtents();
                                num = 3;
                                return num;
                            }

                        //case "EXECUTESQL":
                        //    {
                        //        this.EXECUTESQL(str);
                        //        num = 3;
                        //        return num;
                        //    }
                        case "SHUTDOWN":
                            {
                                this.ShutdownArcMap();
                                num = 3;
                                return num;
                            }

                        case "SELECTLAYERBYATTRIBUTE":
                            {
                                parameters = str.Split(';');
                                this.SelectByAttribute(parameters[0], parameters[1], Convert.ToInt32(parameters[2]));
                                num = 3;
                                return num;
                            }
                        case "SETLAYERVISIBILITY":
                            {

                                parameters = this.GetParameters(str);
                                this.SetLayersVisibility(parameters[0], parameters[1]);
                                num = 3;
                                return num;
                            }

                        case "REMOVELAYERS":
                            {
                                this.RemoveLayer(str);
                                num = 3;
                                return num;
                            }

                        case "UNDOLAYER":
                            {
                                this.Logger.WriteLine(toRemove.Count);
                                parameters = this.GetParameters(str);
                                int iCount = Convert.ToInt16(parameters[0]);
                                for (int i = 0; i < iCount; i++)
                                {
                                    this.AddLayer("{FBF8C3FB-0480-11D2-8D21-080009EE4E51}:1");
                                }
                                num = 3;
                                return num;
                            }
                        case "WRITELOG_SUMMARY":
                            {
                                WRITELOGSUMMARY();

                                return 3;
                            }
                    }
                }

                if (str != "WAIT")
                {
                    num = (!string.IsNullOrEmpty(this._parser.RemoveComments(command)) ? 0 : 1);
                }
                else
                {
                    do
                    {
                        this.Logger.WriteLine("WAIT COMMAND");
                    } while (!refreshed);
                    num = 3;
                    num = (!string.IsNullOrEmpty(this._parser.RemoveComments(command)) ? 0 : 1);
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Error in Commands ", exception.Message, " ", exception.StackTrace), false, null);
                num = 3;
            }

            return num;

        }

        //IDictionary pDic_NFRTotalTime = new Hashtable();
        private void WRITELOGSUMMARY()
        {
            // PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam

            var pDic_NFRTotalTime = new Dictionary<string, double>();

            StreamReader sr = new StreamReader(new System.IO.FileStream(this.Logger.LogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));

            List<String> sLogAllLines = new List<string>();

            while (!sr.EndOfStream)
                sLogAllLines.Add(sr.ReadLine());
            try { sr.Close(); }
            catch { }
            //MessageBox.Show(sLogAllLines.Count.ToString());
            //string[] sLogAllLines = File.ReadAllLines(this.Logger.LogPath);
            
            double dTimeMilliSecond = 0.0;
            string sNFR = string.Empty;
            pDic_NFRTotalTime.Clear();
            foreach (string sLogLine in sLogAllLines)
            {
                //NFR to NFR is one block and not consider NFR without number
                if (sLogLine.StartsWith("NFR"))// && !sLogLine.Split(':')[0].Trim().EndsWith("NFR"))
                    if (!pDic_NFRTotalTime.ContainsKey(sLogLine.Split(':')[0].Trim()))
                    { pDic_NFRTotalTime.Add(sNFR = sLogLine.Split(':')[0].Trim(), 0.0); continue; }
                if (pDic_NFRTotalTime.Count > 0)
                {
                    if (sLogLine.Split(':').Length == 2 &&
                        isAllUpper(sLogLine.Split(':')[0].Trim()) &&
                        !sLogLine.Split(':')[0].Trim().StartsWith("EXECUTESQL") &&
                        double.TryParse(sLogLine.Split(':')[1].Trim(), out dTimeMilliSecond))
                    {
                        pDic_NFRTotalTime[sNFR] = Convert.ToDouble(pDic_NFRTotalTime[sNFR]) + dTimeMilliSecond;
                       // MessageBox.Show(Convert.ToString(pDic_NFRTotalTime[sNFR]));
                    }
                    else if (sLogLine.Split(',')[0] == "All Layers")
                    {
                      //  MessageBox.Show("All Layers");
                        pDic_NFRTotalTime[sNFR] = Convert.ToDouble(pDic_NFRTotalTime[sNFR]) + Convert.ToDouble(sLogLine.Split(',')[sLogLine.Split(',').Length - 1]) * 1000;
                    }
                }
                //Assumption Command Name must be in Block Letter and EXECUTESQL not to be considered
            }
            //IEnumerator pEnumKeys = pDic_NFRTotalTime.Keys.GetEnumerator();
            List<string> pEnumKeys = pDic_NFRTotalTime.Keys.ToList<string>();
            pEnumKeys.Sort();
            //while (pEnumKeys.MoveNext())

            string sUserName = string.Empty;
            try
            {
                sUserName = ((IMMDefaultConnection)((IExtensionManager)Activator.CreateInstance(Type.GetTypeFromProgID("esriSystem.ExtensionManager"))).FindExtension("MMPropertiesExt")).UserName;
            }
            catch { sUserName = Convert.ToString(this.Workspace.ConnectionProperties.GetProperty("USER")); }
            
            this.Logger.WriteLine();
            this.Logger.WriteLine("LOG_FOR_USER: " + sUserName); 
            foreach(string sKey in pEnumKeys)
            {
                //if (pEnumKeys.Current.ToString() == "NFR") continue;
                if (sKey == "NFR") continue;
                //this.Logger.WriteLine(pEnumKeys.Current.ToString() + ": " + Convert.ToString(pDic_NFRTotalTime[Convert.ToString(pEnumKeys.Current)]), false, string.Empty);
                this.Logger.WriteLine(sKey + ": " + Convert.ToString(pDic_NFRTotalTime[sKey]), false, string.Empty);
            }
        }

        bool isAllUpper(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsLetter(input[i])) continue;
                if (!Char.IsUpper(input[i]))
                    return false;
            }

            return true;
        }

        public IWorkspace Workspace
        {
            get;
            set;
        }

        #region Private Methods

        private IApplication getArcMapApp()
        {
            m_pApp = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            return m_pApp;
        }

        List<ILayer> toRemove = new List<ILayer>();
        List<string> lyrname = new List<string>();

        private bool AddLayer(string layers)
        {
            ILayer lyr = null;
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                string[] parameters = this.GetParameters(layers);

                IMap pMap = ((IMxDocument)m_pApp.Document).FocusMap;

                //this.Logger.WriteLine
                for (int i = 0; i < toRemove.Count; i++)
                {
                    pMap.AddLayer(toRemove[i]);
                    lyr = pMap.get_Layer(0);
                    pMap.MoveLayer(lyr, pMap.LayerCount - 2);
                }


                ((IMxDocument)m_pApp.Document).UpdateContents();
                for (int i = 0; i < ((IMxDocument)m_pApp.Document).ContentsViewCount; i++)
                {
                    IContentsView con = ((IMxDocument)m_pApp.Document).get_ContentsView(i);
                    con.Refresh(null);
                }
                //SetEvents(m_pApp);
                ((IMxDocument)m_pApp.Document).ActiveView.Refresh();
                //customRefresh(m_pApp);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("ADDLAYERS:", this.SW1.ElapsedMilliseconds), false, null);
                flag = true;
            }
            catch (Exception exception1)
            {

                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("ADDLAYERS error:", exception.Message), false, null);
                flag = false;
            }
            finally
            {
                if (lyr != null) Marshal.FinalReleaseComObject(lyr);
            }
            return flag;
        }

        private bool RemoveLayer(string layers)
        {
            //IFeatureLayer featLayer = null;
            //IEnumLayer featureLayers = null;
            //ILayer lyr;
            //UID featureUID = null;

            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                string[] parameters = this.GetParameters(layers);

                IMap pMap = ((IMxDocument)m_pApp.Document).FocusMap;

                //featureUID = new UID();
                //featureUID.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";
                //featureLayers = pMap.get_Layers(featureUID, true);

                toRemove.Clear();
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    if (parameters.Contains(pMap.get_Layer(i).Name))
                    {
                        toRemove.Add(pMap.get_Layer(i));
                        lyrname.Add(pMap.get_Layer(i).Name);
                        pMap.DeleteLayer(pMap.get_Layer(i));
                        i--;
                    }
                }

                ((IMxDocument)m_pApp.Document).UpdateContents();
                for (int i = 0; i < ((IMxDocument)m_pApp.Document).ContentsViewCount; i++)
                {
                    IContentsView con = ((IMxDocument)m_pApp.Document).get_ContentsView(i);
                    con.Refresh(null);
                }
                //SetEvents(m_pApp);
                ((IMxDocument)m_pApp.Document).ActiveView.Refresh();
                //customRefresh(m_pApp);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("REMOVELAYERS:", this.SW1.ElapsedMilliseconds), false, null);
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("REMOVELAYERS error:", exception.Message), false, null);
                flag = false;
            }

            return flag;
        }

        private bool SetLayersVisibility(string sLayerName, string sStatus)
        {
            IEnumLayer featureLayers = null;
            IFeatureLayer featLayer = null;
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();


                IMap pMap = ((IMxDocument)m_pApp.Document).FocusMap;

                UID featureUID = new UID();
                featureUID.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                featureLayers = pMap.get_Layers(featureUID, true);
                while ((featLayer = featureLayers.Next() as IFeatureLayer) != null)
                {
                    try
                    {
                        if (featLayer.FeatureClass == null) { continue; }
                        if ((featLayer.Name == sLayerName ? true : sLayerName == "*"))
                        {

                            if (!(sStatus == "ON"))
                            {
                                featLayer.Visible = false;
                            }
                            else
                            {
                                featLayer.Visible = true;
                            }
                        }
                        //this.Logger.WriteLine(featLayer.Name);

                    }
                    catch (Exception e)
                    {

                    }
                }

                //SetEvents(m_pApp);
                ((IMxDocument)m_pApp.Document).ActiveView.Refresh();

                //customRefresh(m_pApp);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("SETLAYERSVISIBILITY:", this.SW1.ElapsedMilliseconds), false, null);
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("SetLayersVisibility error:", exception.Message), false, null);
                flag = false;
            }
            finally
            {
                if (featureLayers != null) Marshal.FinalReleaseComObject(featureLayers);
                if (featLayer != null) Marshal.FinalReleaseComObject(featLayer);
            }
            return flag;
        }

        private bool SelectByAttribute(string sLayerName, string sQuery, int option)
        {
            IEnumLayer layers = null;
            IQueryFilter queryFilterClass = null;

            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMap pMap = ((IMxDocument)m_pApp.Document).FocusMap;
                layers = pMap.get_Layers() as IEnumLayer;

                for (ILayer i = layers.Next(); i != null; i = layers.Next())
                {
                    if (i.Name == sLayerName)
                    {
                        if (i is IFeatureLayer)
                        {
                            IFeatureSelection variable = (IFeatureSelection)((IFeatureLayer)i);
                            queryFilterClass = new QueryFilter()
                            {
                                WhereClause = sQuery
                            };
                            switch (option)
                            {
                                case 0:
                                    variable.SelectFeatures(queryFilterClass, esriSelectionResultEnum.esriSelectionResultNew, false);
                                    break;
                                case 1:
                                    variable.SelectFeatures(queryFilterClass, esriSelectionResultEnum.esriSelectionResultAdd, false);
                                    break;
                                case 2:
                                    variable.SelectFeatures(queryFilterClass, esriSelectionResultEnum.esriSelectionResultSubtract, false);
                                    break;
                                case 3:
                                    variable.SelectFeatures(queryFilterClass, esriSelectionResultEnum.esriSelectionResultAnd, false);
                                    break;
                                case 4:
                                    variable.SelectFeatures(queryFilterClass, esriSelectionResultEnum.esriSelectionResultXOR, false);
                                    break;
                            }
                            //variable.SelectFeatures(queryFilterClass, esriSelectionResultEnum.esriSelectionResultNew, false);
                            ISelectionSet selectionSet = variable.SelectionSet;
                            ((IMxDocument)m_pApp.Document).ActivatedView.Refresh();
                            this.SW1.Stop();
                            ESRI.ScriptEngine.Logger logger = this.Logger;
                            logger.WriteLine("SelectByAttribute Features:" + selectionSet.Count.ToString(), false, null);
                            object[] count = new object[] { "SelectByAttribute_Time:".ToUpper(), this.SW1.ElapsedMilliseconds };
                            logger.WriteLine(string.Concat(count), false, null);
                            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                            flag = true;
                            return flag;
                        }
                    }
                }
                flag = false;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Failed:", exception.Message), false, null);
                flag = false;
            }
            finally
            {
                if (layers != null) Marshal.FinalReleaseComObject(layers);
                if (queryFilterClass != null) Marshal.FinalReleaseComObject(queryFilterClass);
            }
            return flag;
        }

        private void ExecuteCmd(string uid)
        {
            try
            {
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMxDocument document = (IMxDocument)m_pApp.Document;
                IDocument document1 = m_pApp.Document;

                UID uIDClass = new UID()
                {
                    Value = uid
                };

                ICommandItem cmditem = document1.CommandBars.Find(uIDClass, false, false);

                if (cmditem != null)
                {
                    cmditem.Execute();
                }
                else
                    this.Logger.WriteLine("Could not find the command " + uid);
            }

            catch (Exception e)
            {
                this.Logger.WriteLine("Could not find the command " + e.Message);
            }
        }

        private bool CommandClick(string sCommandID)
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                bool flag1 = false;
                IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                UID uIDClass = new UID()
                {
                    Value = sCommandID
                };
                ICommandBars commandBars = variable.Document.CommandBars;
                ICommandItem variable1 = commandBars.Find(uIDClass, false, false);
                if (variable1 == null)
                {
                    this.Logger.WriteLine("Not found", false, null);
                }
                else if (variable1.Type != esriCommandTypes.esriCmdTypeCommand)
                {
                    this.Logger.WriteLine("Invalid Type", false, null);
                }
                else
                {
                    ((ICommand)variable1).OnClick();
                    //SetEvents(m_pApp);
                    this.SW1.Stop();
                    this.Logger.WriteLine(string.Concat("COMMANDCLICK:", this.SW1.ElapsedMilliseconds), false, null);
                    flag1 = true;
                }
                flag = flag1;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("CommandClick error:", exception.Message), false, null);
                flag = false;
            }
            return flag;
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
                for (int i = 0; i < strArrays.Count<string>(); i++)
                {
                    ESRI.ScriptEngine.Logger logger = this.Logger;
                    object[] objArray = new object[] { "Arg:[", i, "]", strArrays[i] };
                    logger.WriteLine(string.Concat(objArray), false, null);
                }
            }
            return strArrays;
        }

        private bool ZOOMTOSELECTED()
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMxDocument document = (IMxDocument)m_pApp.Document;
                IDocument document1 = m_pApp.Document;

                ExecuteCmd("{FA252249-1FDD-4384-9C75-8E3D8C7AD4EA}");

                //SetEvents(m_pApp);
                //((IMxDocument)m_pApp.Document).ActiveView.Refresh();

                //customRefresh(m_pApp);

                document.ActiveView.Refresh();

                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("ZoomToSelected_Time:".ToUpper(), this.SW1.ElapsedMilliseconds), false, null);

                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("ZoomToSelected error:", exception.Message), false, null);
                flag = false;
            }
            return flag;
        }

        public bool CLEARSELECTION()
        {
            this.SW1.Reset();
            this.SW1.Start();

            if (m_pApp == null)
                m_pApp = getArcMapApp();

            IMxDocument document = (IMxDocument)m_pApp.Document;
            document.FocusMap.ClearSelection();
            this.SW1.Stop();
            this.Logger.WriteLine(string.Concat("ClearSelection_Time:".ToUpper(), this.SW1.ElapsedMilliseconds), false, null);
            PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
            return true;
        }

        private bool SetScale(string sScale)
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                bool flag1 = false;
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMxDocument document = (IMxDocument)m_pApp.Document;
                document.FocusMap.MapScale = Convert.ToDouble(sScale);
                //SetEvents(m_pApp);
                //customRefresh(m_pApp); 
                document.ActiveView.Refresh();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("SETSCALE:", this.SW1.ElapsedMilliseconds), false, null);
                flag = flag1;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("SetScale error:", exception.Message), false, null);
                flag = false;
            }
            return flag;
        }

        private bool CreateVersion(string sName)
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();
                IMxDocument document = m_pApp.Document as IMxDocument;
                IVersion workspace = (IVersion)this.Workspace;
                IVersion version = workspace.CreateVersion(sName);
                IChangeDatabaseVersion variable = (ChangeDatabaseVersion)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0038A3AF-0FCB-487A-B3EE-65C0E80D13F0")));
                variable.Execute(workspace, version, (IBasicMap)document.FocusMap);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("CREATEVERSION:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("CreateVersion error:", exception.Message), false, null);
                flag = false;
            }
            return flag;
        }

        private bool ChangeVersion(string sName)
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMxDocument document = m_pApp.Document as IMxDocument;
                IWorkspace workspace = this.Workspace;
                IVersionedWorkspace versionedWorkspace = (IVersionedWorkspace)this.Workspace;
                IVersion version = (IVersion)workspace;
                IVersion version1 = versionedWorkspace.FindVersion(sName);
                IWorkspace workspace1 = (IWorkspace)version1;
                IChangeDatabaseVersion variable = (ChangeDatabaseVersion)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("0038A3AF-0FCB-487A-B3EE-65C0E80D13F0")));
                variable.Execute(version, version1, (IBasicMap)document.FocusMap);
                //ScriptEngine.BroadcastProperty("Workspace", workspace, this);
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("CHANGEVERSION:", this.SW1.ElapsedMilliseconds), false, null);
                PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.pAList_Log.Add(Convert.ToString(++PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.iCount_Log) + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sNFR_Log + ":" + PGE.BatchApplication.ArcFM_PerfQA_Tools.Commands.sCommand_Log + ":" + Convert.ToString(this.SW1.ElapsedMilliseconds));//For Summary Log- Shashwat Nigam
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("ChangeVersion error:", exception.Message), false, null);
                flag = false;
            }
            return flag;
        }

        private bool ZoomToFullExtents()
        {
            bool flag;
            try
            {
                this.SW1.Reset();
                this.SW1.Start();
                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMxDocument doc = m_pApp.Document as IMxDocument;
                IEnvelope fullExtent = doc.ActiveView.FullExtent;
                doc.ActiveView.Extent = fullExtent;

                //SetEvents(m_pApp);
                ((IMxDocument)m_pApp.Document).ActiveView.Refresh();

                //customRefresh(m_pApp);
                //doc.ActiveView.Refresh();
                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("ZOOMTOFULLEXTENT:", this.SW1.ElapsedMilliseconds), false, null);
                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("ZoomToFullExtents error:", exception.Message), false, null);
                flag = false;
            }
            return flag;
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

        private bool ShutdownArcMap()
        {
            IApplication variable = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
            ((IDocumentDirty2)((IMxDocument)variable.Document)).SetClean();
            variable.Shutdown();
            this.Logger.WriteLine("Closing Arcmap", false, null);
            return true;
        }

        ESRI.ArcGIS.Carto.IActiveViewEvents_Event activeViewEvents;

        ESRI.ArcGIS.Carto.esriViewDrawPhase esriDraw;

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

                //displayViewEvents = ((IMxDocument)m_pApp.Document).ActivatedView.ScreenDisplay as IDisplayEvents ; 
                //displayViewEvents.DisplayFinished += new ac

                doc.ActivatedView.Refresh();
            }
        }

        private bool Refresh()
        {
            bool flag = false;
            refreshed = false;
            esriDraw = esriViewDrawPhase.esriViewNone;
            try
            {
                start = 0;
                finish = 0;
                this.SW1.Reset();
                this.SW1.Start();

                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMxDocument doc = m_pApp.Document as IMxDocument;

                //if (m_pApp != null)
                //{
                //    IMap map = ((IMxDocument)m_pApp.Document).FocusMap;
                //    activeViewEvents = map as ESRI.ArcGIS.Carto.IActiveViewEvents_Event;
                //    //Create an instance of the delegate, add it to AfterDraw event
                //    m_ActiveViewEventsAfterDraw = new ESRI.ArcGIS.Carto.IActiveViewEvents_AfterDrawEventHandler(OnActiveViewEventsAfterDraw);

                //    activeViewEvents.AfterDraw += m_ActiveViewEventsAfterDraw;

                //    m_ActiveViewEventsViewRefreshed = new ESRI.ArcGIS.Carto.IActiveViewEvents_ViewRefreshedEventHandler(OnActiveViewEventsViewRefreshed);
                //    activeViewEvents.ViewRefreshed += m_ActiveViewEventsViewRefreshed;
                //}

                //SetEvents(m_pApp);
                //customRefresh(m_pApp); 
                doc.ActivatedView.Refresh();
                //System.Threading.Thread.Sleep(500);
                start = 1;

                this.SW1.Stop();
                this.Logger.WriteLine(string.Concat("Refresh:".ToUpper(), this.SW1.ElapsedMilliseconds), false, null);

                //do
                //{
                //    //sum += ids[i];
                //    //i++;
                //    this.Logger.WriteLine(esriDraw);
                //} while (esriDraw != esriViewDrawPhase.esriViewForeground);

                flag = true;
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("Refresh error:", exception.Message), false, null);
                flag = false;
            }

            return flag;
        }

        private bool PAN(string direction)
        {
            bool flag;

            try
            {
                this.SW1.Reset();
                this.SW1.Start();

                if (m_pApp == null)
                    m_pApp = getArcMapApp();

                IMxDocument doc = m_pApp.Document as IMxDocument;

                Envelope extent;

                if (direction == "L")
                {

                    extent = new Envelope
                    {

                        XMin = doc.ActivatedView.Extent.XMin + doc.ActivatedView.Extent.Width,
                        YMin = doc.ActivatedView.Extent.YMin + doc.ActivatedView.Extent.Height,
                        XMax = doc.ActivatedView.Extent.XMax + doc.ActivatedView.Extent.Width,
                        YMax = doc.ActivatedView.Extent.YMax + doc.ActivatedView.Extent.Height
                    };
                }
                else
                {
                    extent = new Envelope
                    {

                        XMin = doc.ActivatedView.Extent.XMin - doc.ActivatedView.Extent.Width,
                        YMin = doc.ActivatedView.Extent.YMin - doc.ActivatedView.Extent.Height,
                        XMax = doc.ActivatedView.Extent.XMax - doc.ActivatedView.Extent.Width,
                        YMax = doc.ActivatedView.Extent.YMax - doc.ActivatedView.Extent.Height
                    };
                }

                doc.ActivatedView.Extent = (IEnvelope)extent;
                //SetEvents(m_pApp);
                doc.ActivatedView.Refresh();
                //customRefresh(m_pApp);

                this.SW1.Stop();

                this.Logger.WriteLine(string.Concat("PAN:".ToUpper(), this.SW1.ElapsedMilliseconds), false, null);

                flag = true;

            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                this.Logger.WriteLine(string.Concat("PAN error:", exception.Message), false, null);
                flag = false;
            }

            return flag;
        }

        #endregion

    }
}
