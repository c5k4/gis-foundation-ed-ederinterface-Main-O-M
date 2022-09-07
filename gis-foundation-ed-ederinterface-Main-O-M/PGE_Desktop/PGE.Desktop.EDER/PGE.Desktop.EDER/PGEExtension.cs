using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using Miner.ComCategories;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using Miner;
using System.Data;
using System.Collections;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ADODB;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.DataSourcesOleDB;
using System.Security.Principal;
using PGE.Desktop.EDER.ArcMapCommands;
using System.Runtime.Serialization.Json;
using System.IO;
using Oracle.DataAccess.Client;
using PGE.Desktop.EDER.Login;

namespace PGE.Desktop.EDER
{
    [Guid("c754a511-6140-4eee-9d2b-5027e4cb49d4")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.PGEExtension")]
    [ComponentCategory(ComCategory.ArcMapExtensions)]
    public class PGEExtension : IExtension
    {
        private static Timer EsriNetworkToolsRemovalTimer = new Timer();
        private static Timer AUCheckTimer = new Timer();
        //Connect Command UID
        private static UID connectCommandItemUID;
        private static UID disconnectCommandItemUID;

        public static IEditor _editor = null;
        public IDocument _Document = null;
        private static string strSysSchema = "EDER";
        public static IDictionary pDic_SnapAgent = new Hashtable();
        public static bool isDefaultSnapping = false;
        public static bool isDefaultSnappingNO = false; 
        IApplication _app;
        public static IMxApplication _mxapp;
        private static bool isFirstDraw = false;
        public static DataTable pDataTable;

        //ME Q4-19
        public static DataTable pDataTableFormInfo;


        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapExtensions.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapExtensions.Unregister(regKey);
        }

        #endregion

        public PGEExtension()
        {
            try
            {
                pDataTable = new DataTable(strSysSchema + ".USERSNAPPINGSETTINGS");
                pDataTable.Columns.Add("USERID", typeof(String));
                pDataTable.Columns.Add("FEATURE_CLASS", typeof(String));
                pDataTable.Columns.Add("SUBTYPE", typeof(String));
                pDataTable.Columns.Add("SUBTYPE_CODE", typeof(Int32));
                pDataTable.Columns.Add("SNAP_TO_FEATURE_CLASS", typeof(String));
                pDataTable.Columns.Add("HITTYPE", typeof(Int32));
                pDataTable.Columns.Add("TOLERANCE", typeof(Double));
                pDataTable.Columns.Add("TOLERANCE_UNIT", typeof(Int32));
                //ME Q4-19 : DA#190903
                pDataTableFormInfo = new DataTable(strSysSchema + ".TX_SEARCH_FORM_LOC");
                pDataTableFormInfo.Columns.Add("LAN_ID", typeof(String));
                pDataTableFormInfo.Columns.Add("FORM_NAME", typeof(String));
                pDataTableFormInfo.Columns.Add("FORM_STATE", typeof(String));
                pDataTableFormInfo.Columns.Add("FORM_LOCATION", typeof(String));
                pDataTableFormInfo.Columns.Add("LAST_FILTER", typeof(String));
                //pDataTableFormInfo.Columns.Add("REC_INSERT_DT", typeof(DateTime));
                
                UID uID = new UID();
                uID.Value = "esriEditor.Editor";
                _editor = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(uID) as IEditor;

                //UID UID = new UID();
                //UID.Value = "esriEditor.Document";
                //_Document = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(UID) as IDocument;

                _app = Activator.CreateInstance(Type.GetTypeFromProgID("esriFramework.AppRef")) as IApplication;
                _mxapp = _app as IMxApplication;
            }
            catch (Exception ex)
            {

            }

        }

        public static void LoadDataTable(bool bFirstDraw)
        {
            if (bFirstDraw) return;
            ITable pTable_SnapSetting = default(ITable);
            IRow pRow_SnapSetting = default(IRow);
            IQueryFilter pQFilter = default(IQueryFilter);
            ICursor pCursor = default(ICursor);
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string sUserName = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);
            try
            {

                pDataTable.Clear();
                if (_editor.EditWorkspace == null) return;
                pTable_SnapSetting = ((IFeatureWorkspace)
                                     _editor.EditWorkspace).
                                     OpenTable("EDGIS.T_CLASSIC_SNAPPING");

                pQFilter = new QueryFilterClass();
                if (!isDefaultSnapping)
                {
                    pQFilter.WhereClause = "LANID = '" + sUserName + "'"; // TODO: CHECK USERNAME            
                    pCursor = pTable_SnapSetting.Search(pQFilter, false);
                    if (pCursor == null) return;

                    while ((pRow_SnapSetting = pCursor.NextRow()) != null)
                    {
                        try
                        {
                            object[] pRow = new object[] 

                        { 
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("LANID")) != DBNull.Value ? Convert.ToString(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("LANID"))) : string.Empty, 
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("FEATURE_CLASS")) != DBNull.Value ? Convert.ToString(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("FEATURE_CLASS"))) : string.Empty,
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("SUBTYPE")) != DBNull.Value ? Convert.ToString(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("SUBTYPE"))) : string.Empty,
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("SUBTYPE_CODE")) != DBNull.Value ? Convert.ToInt32(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("SUBTYPE_CODE"))) : 0, 
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("SNAP_TO_FEATURE_CLASS")) != DBNull.Value ? Convert.ToString(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("SNAP_TO_FEATURE_CLASS"))) : string.Empty,
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("HITTYPE")) != DBNull.Value ? Convert.ToInt32(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("HITTYPE"))) : 0,
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("TOLERANCE")) != DBNull.Value ? Convert.ToInt32(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("TOLERANCE"))) : 0,
                          pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("TOLERANCE_UNIT")) != DBNull.Value ? Convert.ToInt32(pRow_SnapSetting.get_Value(pRow_SnapSetting.Fields.FindField("TOLERANCE_UNIT"))) : 0
                        };
                            pDataTable.Rows.Add(pRow);
                        }
                        catch { }
                    }
                }
                if (pDataTable.Rows.Count == 0)
                {
                    Recordset pRSet = new Recordset();
                    pRSet.CursorLocation = CursorLocationEnum.adUseClient;
                    pRSet.Open("SELECT * FROM SDE.MM_SNAPPING", //WHERE UPPER(FEATURE_CLASS) LIKE '" + strSchema + ".%' OR UPPER(FEATURE_CLASS) LIKE '" + strOSMMSchema + ".%'",
                        (ADODB.Connection)((IFDOToADOConnection)new FdoAdoConnection()).CreateADOConnection(_editor.EditWorkspace),
                        CursorTypeEnum.adOpenKeyset, LockTypeEnum.adLockOptimistic, 0);
                    pRSet.MoveFirst();
                    while (!pRSet.EOF)
                    {
                        //do
                        //{
                        object[] pRow = new object[] 
                        { 
                          sUserName,
                          pRSet.Fields["FEATURE_CLASS"].Value != DBNull.Value ? Convert.ToString(pRSet.Fields["FEATURE_CLASS"].Value) : string.Empty,
                          pRSet.Fields["SUBTYPE"].Value != DBNull.Value ? Convert.ToString(pRSet.Fields["SUBTYPE"].Value) : string.Empty,
                          pRSet.Fields["SUBTYPE_CODE"].Value != DBNull.Value ? Convert.ToInt32(pRSet.Fields["SUBTYPE_CODE"].Value) == -1 ? 0 : Convert.ToInt32(pRSet.Fields["SUBTYPE_CODE"].Value) : 0, 
                          pRSet.Fields["SNAP_TO_FEATURE_CLASS"].Value != DBNull.Value ? Convert.ToString(pRSet.Fields["SNAP_TO_FEATURE_CLASS"].Value) : string.Empty,
                          pRSet.Fields["HITTYPE"].Value != DBNull.Value ? Convert.ToInt32(pRSet.Fields["HITTYPE"].Value) : 0,
                          pRSet.Fields["TOLERANCE"].Value != DBNull.Value ? Convert.ToInt32(pRSet.Fields["TOLERANCE"].Value) : 0,
                          0
                        };
                        pDataTable.Rows.Add(pRow);
                        pRSet.MoveNext();

                    }//while (!pRSet.EOF);
                    pRSet.Close();
                }
                isFirstDraw = true;
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message, "PGE Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                if (pTable_SnapSetting != null) Marshal.ReleaseComObject(pTable_SnapSetting);
                if (pRow_SnapSetting != null) Marshal.ReleaseComObject(pRow_SnapSetting);
                if (pQFilter != null) Marshal.ReleaseComObject(pQFilter);
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
            }
        }

        //ME Q4-19 : DA#190903
        public static void LoadDataTable_FormTx(bool bFirstDraw)
        {
            if (bFirstDraw) return;
            ITable pTable_FormTx = default(ITable);
            IRow pRow_FormTx = default(IRow);
            IQueryFilter pQFilter = default(IQueryFilter);
            ICursor pCursor = default(ICursor);
            WindowsIdentity wi = WindowsIdentity.GetCurrent();
            string sUserName = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);
            try
            {

                pDataTableFormInfo.Clear();
                IMMLoginUtils utils = new MMLoginUtils();

                if (utils.LoginWorkspace == null) return;
                pTable_FormTx = ((IFeatureWorkspace)
                                     utils.LoginWorkspace).
                                     OpenTable("EDGIS.TX_SEARCH_FORM_LOC");
                
                pQFilter = new QueryFilterClass();
                if (!isDefaultSnapping)
                {
                    pQFilter.WhereClause = "FORM_STATE = 'OPEN' AND LAN_ID = '" + sUserName.ToUpper() + "'"; // TODO: CHECK USERNAME            
                    pCursor = pTable_FormTx.Search(pQFilter, false);
                    if (pCursor == null) return;

                    while ((pRow_FormTx = pCursor.NextRow()) != null)
                    {
                        try
                        {
                            object[] pRowTx = new object[] 
                        { 
                          pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("LAN_ID")) != DBNull.Value ? Convert.ToString(pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("LAN_ID"))) : string.Empty, 
                          pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("FORM_NAME")) != DBNull.Value ? Convert.ToString(pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("FORM_NAME"))) : string.Empty,
                          pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("FORM_STATE")) != DBNull.Value ? Convert.ToString(pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("FORM_STATE"))) : string.Empty,
                          pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("FORM_LOCATION")) != DBNull.Value ? Convert.ToString(pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("FORM_LOCATION"))) :string.Empty, 
                          pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("LAST_FILTER")) != DBNull.Value ? Convert.ToString(pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("LAST_FILTER"))) : string.Empty,
                         // pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("REC_INSERT_DT")) != DBNull.Value ? Convert.ToInt32(pRow_FormTx.get_Value(pRow_FormTx.Fields.FindField("REC_INSERT_DT"))) : 0,
                          
                        };
                            pDataTableFormInfo.Rows.Add(pRowTx);
                        }
                        catch (Exception ex){ }
                    }
                }

                isFirstDraw = true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "PGE Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                if (pTable_FormTx != null) Marshal.ReleaseComObject(pTable_FormTx);
                if (pRow_FormTx != null) Marshal.ReleaseComObject(pRow_FormTx);
                if (pQFilter != null) Marshal.ReleaseComObject(pQFilter);
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
            }
        }

        void Events_OnStartEditing()
        {
            if (_editor.EditWorkspace == null) return;
            if (((IWorkspace)_editor.EditWorkspace).Type != esriWorkspaceType.esriRemoteDatabaseWorkspace) return;
            //if (((IVersion)_editor.EditWorkspace).VersionName.EndsWith("DEFAULT")) return;
            Events.OnCurrentLayerChanged += new IEditEvents_OnCurrentLayerChangedEventHandler(Events_OnCurrentLayerChanged);

           //((Map)_Document).AfterDraw += new IActiveViewEvents_AfterDrawEventHandler(Ext_LoadSnapping_AfterDraw);

            LoadDataTable(isFirstDraw);
            Events_OnCurrentLayerChanged();
        }

        void Events_OnCurrentLayerChanged()
        {
            if (_editor.EditWorkspace == null) return;
            IFeatureSnapAgent2 pFSAgent2 = default(IFeatureSnapAgent2);
            IDictionary pDic_SA_Checked = new Hashtable();
            try
            {
                if (pDataTable.Rows.Count == 0) return;

                for (int iDR_Count = 0; iDR_Count < pDataTable.Rows.Count; ++iDR_Count)
                {
                    if (Convert.ToString(pDataTable.Rows[iDR_Count]["FEATURE_CLASS"]) == ((IDataset)((IEditLayers)_editor).CurrentLayer.FeatureClass).Name &&
                        Convert.ToInt32(pDataTable.Rows[iDR_Count]["SUBTYPE_CODE"]) == ((IEditLayers)_editor).CurrentSubtype)
                    {
                        for (int iSnapCount = 0; iSnapCount < ((ISnapEnvironment3)_editor).SnapAgentCount; ++iSnapCount)
                        {
                            try
                            {
                                pFSAgent2 = (IFeatureSnapAgent2)((ISnapEnvironment3)_editor).SnapAgent[iSnapCount];
                            }
                            catch { continue; }

                            if (Convert.ToString(pDataTable.Rows[iDR_Count]["SNAP_TO_FEATURE_CLASS"]) != pFSAgent2.Name) continue;
                            pFSAgent2.HitType = (esriGeometryHitPartType)Convert.ToInt32(pDataTable.Rows[iDR_Count]["HITTYPE"]);
                            ((ISnapEnvironment3)_editor).SnapTolerance = Convert.ToInt32(pDataTable.Rows[iDR_Count]["TOLERANCE"]);
                            ((ISnapEnvironment3)_editor).SnapToleranceUnits = (esriSnapToleranceUnits)Convert.ToInt32(pDataTable.Rows[iDR_Count]["TOLERANCE_UNIT"]);

                            pDic_SA_Checked.Add(pFSAgent2.Name, pFSAgent2);
                        }
                    }
                }
                pDic_SnapAgent.Clear();
                
                pFSAgent2 = default(IFeatureSnapAgent2);
                for (int iSnapCount = 0; iSnapCount < ((ISnapEnvironment3)_editor).SnapAgentCount; ++iSnapCount)
                {
                    try
                    {
                        pFSAgent2 = (IFeatureSnapAgent2)((ISnapEnvironment3)_editor).SnapAgent[iSnapCount];
                    }
                    catch { continue; }
                    if (pDic_SnapAgent.Contains(pFSAgent2.Name)) continue;
                    pDic_SnapAgent.Add(pFSAgent2.Name, pFSAgent2);
                }
                int i = ((ISnapEnvironment3)_editor).SnapAgentCount;
                for (int iSnapCount = 0; iSnapCount < i; ++iSnapCount)
                {
                    ((ISnapEnvironment3)_editor).RemoveSnapAgent(0);
                }

                IEnumerator pEnum_Dic = default(IEnumerator);

                for (int iDR_Count = 0; iDR_Count < pDataTable.Rows.Count; ++iDR_Count)
                {
                    try
                    {
                        if (Convert.ToString(pDataTable.Rows[iDR_Count]["FEATURE_CLASS"]) == ((IDataset)((IEditLayers)_editor).CurrentLayer.FeatureClass).Name &&
                           Convert.ToInt32(pDataTable.Rows[iDR_Count]["SUBTYPE_CODE"]) == ((IEditLayers)_editor).CurrentSubtype)
                        {
                            ((ISnapEnvironment3)_editor).AddSnapAgent((IFeatureSnapAgent2)pDic_SA_Checked[Convert.ToString(pDataTable.Rows[iDR_Count]["SNAP_TO_FEATURE_CLASS"])]);
                        }
                    }
                    catch { }
                }

                pEnum_Dic = pDic_SnapAgent.Values.GetEnumerator();
                while (pEnum_Dic.MoveNext())
                {
                    if (pDic_SA_Checked.Contains(((IFeatureSnapAgent2)pEnum_Dic.Current).Name)) continue;
                    ((IFeatureSnapAgent2)pEnum_Dic.Current).HitType = esriGeometryHitPartType.esriGeometryPartNone;
                    ((ISnapEnvironment3)_editor).AddSnapAgent((IFeatureSnapAgent2)pEnum_Dic.Current);
                }

                ((ISnappingWindow)_editor.FindExtension(new UIDClass() { Value = "esriEditor.SnappingWindow" })).RefreshContents();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (pFSAgent2 != null) Marshal.ReleaseComObject(pFSAgent2);
            }
        }

        void Events_OnStopEditing(bool save)
        {
            Events.OnCurrentLayerChanged -= Events_OnCurrentLayerChanged;
            isFirstDraw = false;
            isDefaultSnapping = false;
            isDefaultSnappingNO = false;//Shashwat: For Defect of Snapping Button Enable on No Click; Line 161 in C:\Work\Setup\Upgraded Code\TCS.GISUpgrade.Extension\PGE.Desktop.EDER.ArcMapCommands\Ext_LoadSnapping.cs
        }

        #region Shortcut properties to the various editor event interfaces
        private IEditEvents_Event Events
        {
            get { return _editor as IEditEvents_Event; }
        }
        #endregion
      
        public string Name
        {
            get { return "PG&E Extension"; }
        }

        public void Shutdown()
        {
            //Nothing to do
        }

        public void Startup(ref object initializationData)
        {
            if (connectCommandItemUID == null) { connectCommandItemUID = new UIDClass(); }
            if (disconnectCommandItemUID == null) { disconnectCommandItemUID = new UIDClass(); }
            connectCommandItemUID.Value = "{111ACB08-D71E-11D2-9F3E-00C04F6BDD84}";
            disconnectCommandItemUID.Value = "{448E1796-168B-11D2-AEF7-0000F80372B4}";

            //Execute our tick every 3 seconds
            EsriNetworkToolsRemovalTimer.Interval = 3000;
            EsriNetworkToolsRemovalTimer.Tick += new EventHandler(EsriNetworkToolsRemovalTimer_Tick);

            EsriNetworkToolsRemovalTimer.Start();

            //Execute our check every 10 seconds to determine when autoupdater mode has been disabled.
            AUCheckTimer.Interval = 10000;
            AUCheckTimer.Tick += new EventHandler(AUCheckTimer_Tick);
            //AUCheckTimer.Start();


            Events.OnStartEditing += new IEditEvents_OnStartEditingEventHandler(Events_OnStartEditing);
            Events.OnStopEditing += new IEditEvents_OnStopEditingEventHandler(Events_OnStopEditing);

            LoadDataTable_FormTx(isFirstDraw);
            

        }

        List<string> versionsEdited = new List<string>();

        void AUCheckTimer_Tick(object sender, EventArgs e)
        {
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;

            if (immAutoupdater.AutoUpdaterMode == mmAutoUpdaterMode.mmAUMNoEvents)
            {
                if (Miner.Geodatabase.Edit.Editor.EditWorkspace != null)
                {
                    IVersion editVersion = Miner.Geodatabase.Edit.Editor.EditWorkspace as IVersion;
                    if (!versionsEdited.Contains(editVersion.VersionName))
                    {
                        versionsEdited.Add(editVersion.VersionName);
                        try
                        {
                            string mmAUModeDescription = System.Enum.GetName(typeof(mmAutoUpdaterMode), immAutoupdater.AutoUpdaterMode);
                            Miner.Geodatabase.Edit.Editor.EditWorkspace.ExecuteSQL("Insert into EDGIS.PGE_AU_MODE_MODIFICATION VALUES(user,'" + editVersion.VersionName + "','" + mmAUModeDescription + "',sysdate)");
                        }
                        catch (Exception ex) { }
                        if (immAutoupdater.AutoUpdaterMode == mmAutoUpdaterMode.mmAUMNoEvents)
                        {
                            MessageBox.Show("Editing with autoupdaters disabled can cause invalid data within Feeder Manager! Your user name will be logged for tracking purposes.", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        void EsriNetworkToolsRemovalTimer_Tick(object sender, EventArgs e)
        {
            EsriNetworkToolsRemovalTimer.Stop();
            //Remove any instances of the Esri Disconnect tool from any toolbars
            try
            {
                ICommandItem connectCmdItem = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document.CommandBars.Find(connectCommandItemUID, false, true);
                while (connectCmdItem != null)
                {
                    connectCmdItem.Delete();
                    connectCmdItem = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document.CommandBars.Find(connectCommandItemUID, false, true);
                }

                ICommandItem disconnectCmdItem = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document.CommandBars.Find(disconnectCommandItemUID, false, true);
                while (disconnectCmdItem != null)
                {
                    disconnectCmdItem.Delete();
                    disconnectCmdItem = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document.CommandBars.Find(disconnectCommandItemUID, false, true);
                }
            }
            catch (Exception ex) { }

            EsriNetworkToolsRemovalTimer.Start();
        }
        
    }
}

