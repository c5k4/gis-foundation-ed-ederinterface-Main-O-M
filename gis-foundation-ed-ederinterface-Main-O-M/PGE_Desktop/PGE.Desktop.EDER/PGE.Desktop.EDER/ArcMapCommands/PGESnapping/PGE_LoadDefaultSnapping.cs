using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using PGE.Desktop.EDER.ArcMapCommands.SuperUser;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using System.Windows.Forms;
using Miner.Interop;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using System.Security.Principal;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Summary description for PGE_LoadDefaultSnapping.
    /// </summary>
    [Guid("d9141e73-134f-449b-8b49-83bc48014409")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_LoadDefaultSnapping")]
    public sealed class PGE_LoadDefaultSnapping : BaseCommand
    {
            private IEditor _editor = null;
        
            public static bool isDeleteUserSnap = false;
            private Security privCheck = new Security();
            private IApplication m_application;
            private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        public PGE_LoadDefaultSnapping()
        {
            try
            {

                //
                // TODO: Define values for the public properties
                ////
                base.m_category = ""; //localizable text
                base.m_caption = "Load Default Snapping";  //localizable text
                base.m_message = "Load Default Snapping";  //localizable text 
                base.m_toolTip = "Load Default Snapping";  //localizable text 
                base.m_name = "PGE_LoadDefaultSnapping";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")
            }
            catch (Exception ex)
            {
                _logger.Warn("Error in Load Default Snapping: " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Load Default Snapping");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {

            if (hook == null)
                return;

            m_application = hook as IApplication;

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

         
           // privCheck.Initialise(m_application, AppID);
            // TODO:  Add other initialization code

            UID uID = new UID();
            uID.Value = "esriEditor.Editor";
            _editor = PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.FindExtensionByCLSID(uID) as IEditor;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            //LoadDefaultSnapCommon();
            LoadDefaultSnapAll();
        }

        /// <summary>
        /// Overridden Property to determine if command will be in enabled mode or not.
        /// </summary>
        public override bool Enabled
        {
            get
            {
                //Enable if map is in edit mode

                return _editor.EditState == esriEditState.esriStateEditing;
            }
        }
        #endregion Overridden Class Methods

        private void LoadDefaultSnapAll()
        {
            IFeatureSnapAgent2 pFSAgent2 = default(IFeatureSnapAgent2);

            try
            {

                DialogResult pDResult = MessageBox.Show("This would load Default Snap Settings. Press\nOK to Load Default Settings and Delete User's customised settings (if any).\nCancel to abort loading the Default Settings.",
                                        "Snap Settings",
                                        MessageBoxButtons.OKCancel,
                                        MessageBoxIcon.Question,
                                        MessageBoxDefaultButton.Button2);
                if (pDResult == DialogResult.Cancel) return;

                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                string sUserName = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1); 
                
               // PGEExtension.isDefaultSnappingNO = pDResult == DialogResult.No; //Shashwat: For Defect of Snapping Button Enable on No Click; Line 67 in C:\Work\Setup\Upgraded Code\TCS.GISUpgrade.AddinCommands\TCS.GISUpgrade.AddInCommand.Zoom500\***_Commands\Cmd_LoadDefaultSnap.cs
                if (isDeleteUserSnap = pDResult == DialogResult.OK)
                {
                    _editor.EditWorkspace.ExecuteSQL("DELETE from EDGIS.T_CLASSIC_SNAPPING WHERE LANID = '" + sUserName + "'");
                }

                PGEExtension.isDefaultSnapping = true;
                
                PGEExtension.LoadDataTable(false);
            
                if ((IDataset)((IEditLayers)_editor).CurrentLayer != null)
                {
                    Events_OnCurrentLayerChanged();
                }
               
                ((ISnappingWindow)_editor.FindExtension(new UIDClass() { Value = "esriEditor.SnappingWindow" })).RefreshContents();
                m_application.StatusBar.set_Message(0, "Default Classic Snapping Settings have been loaded...");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "PGE Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {
                if (pFSAgent2 != null) Marshal.ReleaseComObject(pFSAgent2);
            }
        }

        void Events_OnCurrentLayerChanged()
        {
            //if (isDefaultSnapping) pDataTable.Clear();
            if (_editor.EditWorkspace == null) return;
            IFeatureSnapAgent2 pFSAgent2 = default(IFeatureSnapAgent2);
            //DataRow[]          pDataRow        = default(DataRow[]);
            IDictionary pDic_SA_Checked = new Hashtable();
            try
            {
                if (PGEExtension.pDataTable.Rows.Count == 0) return;
               string sFeatureClassName = string.Empty;
              
                for (int iDR_Count = 0; iDR_Count < PGEExtension.pDataTable.Rows.Count; ++iDR_Count)
                {
                    if (Convert.ToString(PGEExtension.pDataTable.Rows[iDR_Count]["FEATURE_CLASS"]) == ((IDataset)((IEditLayers)_editor).CurrentLayer.FeatureClass).Name &&
                        Convert.ToInt32(PGEExtension.pDataTable.Rows[iDR_Count]["SUBTYPE_CODE"]) == ((IEditLayers)_editor).CurrentSubtype)
                    {
                        for (int iSnapCount = 0; iSnapCount < ((ISnapEnvironment3)_editor).SnapAgentCount; ++iSnapCount)
                        {
                            try
                            {
                                pFSAgent2 = (IFeatureSnapAgent2)((ISnapEnvironment3)_editor).SnapAgent[iSnapCount];
                            }
                            catch { continue; }
                            if (Convert.ToString(PGEExtension.pDataTable.Rows[iDR_Count]["SNAP_TO_FEATURE_CLASS"]) != pFSAgent2.Name) continue;
                            pFSAgent2.HitType = (esriGeometryHitPartType)Convert.ToInt32(PGEExtension.pDataTable.Rows[iDR_Count]["HITTYPE"]);
                            ((ISnapEnvironment3)_editor).SnapTolerance = Convert.ToInt32(PGEExtension.pDataTable.Rows[iDR_Count]["TOLERANCE"]);
                            ((ISnapEnvironment3)_editor).SnapToleranceUnits = (esriSnapToleranceUnits)Convert.ToInt32(PGEExtension.pDataTable.Rows[iDR_Count]["TOLERANCE_UNIT"]);
                            

                            pDic_SA_Checked.Add(pFSAgent2.Name, pFSAgent2);
                        }
                    }
                }
                PGEExtension.pDic_SnapAgent.Clear();

                pFSAgent2 = default(IFeatureSnapAgent2);
                //((ISnapEnvironment3)_editor).ClearSnapAgents();
                for (int iSnapCount = 0; iSnapCount < ((ISnapEnvironment3)_editor).SnapAgentCount; ++iSnapCount)
                {
                    try
                    {
                        pFSAgent2 = (IFeatureSnapAgent2)((ISnapEnvironment3)_editor).SnapAgent[iSnapCount];
                    }
                    catch { continue; }
                    if (PGEExtension.pDic_SnapAgent.Contains(pFSAgent2.Name)) continue;
                    //pFSAgent2.HitType = esriGeometryHitPartType.esriGeometryPartNone;
                    PGEExtension.pDic_SnapAgent.Add(pFSAgent2.Name, pFSAgent2);
                }
                int i = ((ISnapEnvironment3)_editor).SnapAgentCount;
                for (int iSnapCount = 0; iSnapCount < i; ++iSnapCount)
                {
                    ((ISnapEnvironment3)_editor).RemoveSnapAgent(0);
                }

                IEnumerator pEnum_Dic = default(IEnumerator);

                for (int iDR_Count = 0; iDR_Count < PGEExtension.pDataTable.Rows.Count; ++iDR_Count)
                {
                    try
                    {
                        if (Convert.ToString(PGEExtension.pDataTable.Rows[iDR_Count]["FEATURE_CLASS"]) == ((IDataset)((IEditLayers)_editor).CurrentLayer.FeatureClass).Name &&
                           Convert.ToInt32(PGEExtension.pDataTable.Rows[iDR_Count]["SUBTYPE_CODE"]) == ((IEditLayers)_editor).CurrentSubtype)
                        {
                            ((ISnapEnvironment3)_editor).AddSnapAgent((IFeatureSnapAgent2)pDic_SA_Checked[Convert.ToString(PGEExtension.pDataTable.Rows[iDR_Count]["SNAP_TO_FEATURE_CLASS"])]);
                        }
                    }
                    catch (Exception ex1) { }
                }

                pEnum_Dic = PGEExtension.pDic_SnapAgent.Values.GetEnumerator();
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

    }
}
