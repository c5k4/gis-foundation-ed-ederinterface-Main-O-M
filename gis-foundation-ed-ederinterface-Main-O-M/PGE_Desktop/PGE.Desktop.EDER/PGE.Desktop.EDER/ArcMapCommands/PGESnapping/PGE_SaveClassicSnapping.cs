using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using PGE.Desktop.EDER.ArcMapCommands.SuperUser;
using ESRI.ArcGIS.Editor;
using System.Data;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using Miner.Interop;
using System.Security.Principal;
using System.Xml.Linq;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Summary description for PGE_SaveClassicSnapping.
    /// </summary>
    [Guid("8325b806-e596-4c6d-ae35-6d29a485317b")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_SaveClassicSnapping")]
    public class Cmd_SaveSnap : BaseCommand
    {
        private IEditor _editor = null;
        private Security privCheck = new Security();
        public static string sUserName = string.Empty;
        public static string XMLString = string.Empty;
        private string sFeatureClassName = string.Empty, sSubTypeCD = string.Empty;
        private bool isSaveRunning = false;
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

        private IApplication m_application;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public Cmd_SaveSnap()
        {
            try
            {
                //
                // TODO: Define values for the public properties
                ////
                base.m_category = ""; //localizable text
                base.m_caption = "Save Classic Snapping";  //localizable text
                base.m_message = "Save Classic Snapping for selected target layer";  //localizable text 
                base.m_toolTip = "Save Classic Snapping";  //localizable text 
                base.m_name = "PGE_SaveClassicSnapping";   //unique id, non-localizable (e.g. "MyCategory_ArcMapCommand")


            }
            catch (Exception ex)
            {
                _logger.Warn("Error in Save Classic Snapping: " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.Message, "Save Classic Snapping");
            }
        }


        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            try
            {
                if (hook == null)
                    return;

                m_application = hook as IApplication;

                //Disable if it is not ArcMap
                if (hook is IMxApplication)
                    base.m_enabled = true;
                else
                    base.m_enabled = false;

                UID uID = new UID();
                uID.Value = "esriEditor.Editor";
                if (m_application == null)
                    return;

                _editor = m_application.FindExtensionByCLSID(uID) as IEditor;

                if (m_application == null)
                    return;
                _editor = m_application.FindExtensionByName("ESRI Object Editor") as IEditor;
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                sUserName = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);

                // TODO:  Add other initialization code
            }
            catch (Exception ex)
            {
                _logger.Warn("Error on Create of Save Classic Snapping: " + ex.Message);
            }
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {
            SaveSnappingAll();
            // TODO: Add Cmd_SaveSnap.OnClick implementation
        }

        /// <summary>
        /// Overridden Property to determine if command will be in enabled mode or not.
        /// </summary>
        public override bool Enabled
        {
            get
            {
                //Enable if classic snapping is on and map is in edit mode

                UID pID = new UIDClass();
                pID.Value = "esriEditor.Editor";
                IEditProperties4 editPropObj = m_application.FindExtensionByCLSID(pID) as IEditProperties4;
                if (editPropObj.ClassicSnapping == true && _editor.EditState == esriEditState.esriStateEditing)
                    return true;
                else
                    return false;
            }
        }

        #endregion Overridden Class Methods

        private void SaveSnappingAll()
        {
            IFeatureSnapAgent2 pFSAgent2 = default(IFeatureSnapAgent2);
            string sSubTypeName = string.Empty;
            IMouseCursor pCursor = new MouseCursorClass();
            DataRow[] pDataRows = default(DataRow[]);
            try
            {
                pCursor.SetCursor(2);
                if ((IDataset)((IEditLayers)_editor).CurrentLayer == null)
                {
                    MessageBox.Show("Please select a target layer to save its Snapping properties.", "Message", MessageBoxButtons.OK);
                    return;
                }
                sFeatureClassName = ((IDataset)((IEditLayers)_editor).CurrentLayer.FeatureClass).Name;
                sSubTypeCD = Convert.ToString(((IEditLayers)_editor).CurrentSubtype);

                pDataRows = PGEExtension.pDataTable.Select("FEATURE_CLASS = '" + sFeatureClassName +
                    "' AND SUBTYPE_CODE = " + Convert.ToInt32(sSubTypeCD), string.Empty, DataViewRowState.CurrentRows);

                for (int iDR_Count = 0; iDR_Count < pDataRows.Length; ++iDR_Count)
                {
                    pDataRows[iDR_Count].Delete();
                }
                PGEExtension.pDataTable.AcceptChanges();

                for (int iSnapCount = 0; iSnapCount < ((ISnapEnvironment3)_editor).SnapAgentCount; ++iSnapCount)
                {
                    try
                    {
                        pFSAgent2 = (IFeatureSnapAgent2)((ISnapEnvironment3)_editor).SnapAgent[iSnapCount];
                    }
                    catch { continue; }
                    pDataRows = PGEExtension.pDataTable.
                        Select("FEATURE_CLASS = '" + sFeatureClassName + "' AND SUBTYPE_CODE = " + Convert.ToInt32(sSubTypeCD) +
                        " AND SNAP_TO_FEATURE_CLASS = '" + pFSAgent2.Name + "'", string.Empty, DataViewRowState.CurrentRows);

                    if (pDataRows.Length == 0)
                    {
                        if (Convert.ToInt32(pFSAgent2.HitType) == 0) continue;
                        object[] pRow = new object[] { 
                                                        sUserName, 
                                                        ((IDataset)((IEditLayers)_editor).CurrentLayer.FeatureClass).Name ,
                                                        ((ISubtypes)((IEditLayers)_editor).CurrentLayer.FeatureClass).HasSubtype ? 
                                                        (((IEditLayers)_editor).CurrentSubtype == 0 || ((IEditLayers)_editor).CurrentSubtype == -1) ? 
                                                        string.Empty : 
                                                        ((ISubtypes)((IEditLayers)_editor).CurrentLayer.FeatureClass).get_SubtypeName(((IEditLayers)_editor).CurrentSubtype) : 
                                                        string.Empty,
                                                        ((IEditLayers)_editor).CurrentSubtype, 
                                                        pFSAgent2.Name, 
                                                        Convert.ToInt32(pFSAgent2.HitType),
                                                        ((ISnapEnvironment3)_editor).SnapTolerance, 
                                                        Convert.ToInt32(((ISnapEnvironment3)_editor).SnapToleranceUnits)
                                                    };
                        PGEExtension.pDataTable.Rows.Add(pRow);
                        PGEExtension.pDataTable.AcceptChanges();
                    }
                    else
                    {
                        if (Convert.ToInt32(pFSAgent2.HitType) == 0) { pDataRows[0].Delete(); continue; }
                        pDataRows[0]["HITTYPE"] = Convert.ToInt32(pFSAgent2.HitType);
                        pDataRows[0]["TOLERANCE"] = Convert.ToInt32(((ISnapEnvironment3)_editor).SnapTolerance);
                        pDataRows[0]["TOLERANCE_UNIT"] = Convert.ToInt32(((ISnapEnvironment3)_editor).SnapToleranceUnits);
                        PGEExtension.pDataTable.AcceptChanges();
                    }
                }


                if (!PGEExtension.isDefaultSnapping || PGE_LoadDefaultSnapping.isDeleteUserSnap)
                {
                    m_application.StatusBar.ShowProgressBar("Saving Classic Snapping Settings...", 0, 100, 1, true);
                    //(new Thread(new ThreadStart(SaveSnappingAllinDB)) { Priority = ThreadPriority.AboveNormal }).Start();
                    SaveSnappingAllinDB();
                }
                m_application.StatusBar.set_Message(0, "Classic Snapping Settings have been saved successfully...");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show(ex.Message, "PGE Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (pFSAgent2 != null) Marshal.ReleaseComObject(pFSAgent2);
            }
        }

        private void SaveSnappingAllinDB()
        {
            ITable pTable_SnapSetting = ((IFeatureWorkspace)
                                      _editor.EditWorkspace).
                                      OpenTable("EDGIS.T_CLASSIC_SNAPPING");

            int iStep = 0;
            bool isAnyRecord = false;
            double dStep = PGEExtension.pDataTable.Rows.Count / 100D;


            string sSql = string.Empty;
            if (PGEExtension.pDataTable.Rows.Count < 100)
                m_application.StatusBar.ProgressBar.StepValue = Convert.ToInt32(100 / PGEExtension.pDataTable.Rows.Count);

            isSaveRunning = true;
            try
            {
                string sQuery = null;
                sQuery = "DELETE FROM EDGIS.T_CLASSIC_SNAPPING WHERE LANID = '" + sUserName + "' AND FEATURE_CLASS = '" + sFeatureClassName + "' AND SUBTYPE_CODE = " + Convert.ToInt32(sSubTypeCD);
                _editor.EditWorkspace.ExecuteSQL(sQuery);

                if (pTable_SnapSetting.RowCount(new QueryFilterClass() { WhereClause = "LANID = '" + sUserName + "'" }) > 0)
                {
                    sSql = "INSERT ALL ";
                    for (int iCount_Row = 0; iCount_Row < PGEExtension.pDataTable.Rows.Count; ++iCount_Row)
                    {
                        if (iCount_Row >= (dStep * iStep))// if (iCount_Row == iStep)
                        {
                            m_application.StatusBar.StepProgressBar();
                            iStep++;// = Convert.ToInt32(100 * iCount_Row / PGEExtension.pDataTable.Rows.Count);
                        }
                        if (Convert.ToString(PGEExtension.pDataTable.Rows[iCount_Row]["FEATURE_CLASS"]) == sFeatureClassName &&
                        Convert.ToString(PGEExtension.pDataTable.Rows[iCount_Row]["SUBTYPE_CODE"]) == sSubTypeCD)
                        {
                            try
                            {
                                if (Convert.ToInt32(PGEExtension.pDataTable.Rows[iCount_Row]["HITTYPE"]) == 0) continue;
                                isAnyRecord = true;
                                sSql += "INTO EDGIS.T_CLASSIC_SNAPPING " + "(LANID, FEATURE_CLASS, HITTYPE, SNAP_TO_FEATURE_CLASS, SUBTYPE, SUBTYPE_CODE, TOLERANCE, TOLERANCE_UNIT) VALUES " + "('" +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["USERID"] + "', '" +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["FEATURE_CLASS"] + "', " +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["HITTYPE"] + ", '" +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["SNAP_TO_FEATURE_CLASS"] + "', '" +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["SUBTYPE"] + "', " +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["SUBTYPE_CODE"] + ", " +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["TOLERANCE"] + ", " +
                                       PGEExtension.pDataTable.Rows[iCount_Row]["TOLERANCE_UNIT"] + ") ";
                            }
                            catch { continue; }
                        }
                    }
                    sSql += "SELECT * FROM DUAL";
                    if (isAnyRecord)
                        _editor.EditWorkspace.ExecuteSQL(sSql);
                }
                else
                {
                    sSql = "INSERT ALL ";

                    for (int iCount_Row = 0; iCount_Row < PGEExtension.pDataTable.Rows.Count; ++iCount_Row)
                    {
                        if (iCount_Row >= (dStep * iStep))// if (iCount_Row == iStep)
                        {
                            m_application.StatusBar.StepProgressBar();
                            iStep++;
                        }
                        try
                        {
                            if (Convert.ToInt32(PGEExtension.pDataTable.Rows[iCount_Row]["HITTYPE"]) == 0) continue;

                            sSql += "INTO " + "EDGIS.T_CLASSIC_SNAPPING " + "(LANID, FEATURE_CLASS, HITTYPE, SNAP_TO_FEATURE_CLASS, SUBTYPE, SUBTYPE_CODE, TOLERANCE, TOLERANCE_UNIT) VALUES " + "('" +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["USERID"] + "', '" +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["FEATURE_CLASS"] + "', " +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["HITTYPE"] + ", '" +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["SNAP_TO_FEATURE_CLASS"] + "', '" +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["SUBTYPE"] + "', " +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["SUBTYPE_CODE"] + ", " +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["TOLERANCE"] + ", " +
                                   PGEExtension.pDataTable.Rows[iCount_Row]["TOLERANCE_UNIT"] + ") ";
                        }
                        catch { continue; }
                    }
                    sSql += "SELECT * FROM DUAL";
                    _editor.EditWorkspace.ExecuteSQL(sSql);
                }

                m_application.StatusBar.set_Message(0, "Snap Settings Saved for " + sUserName);
                PGEExtension.isDefaultSnapping = false;
                PGEExtension.LoadDataTable(false);
            }
            catch (Exception ex1)
            {
                _logger.Error(ex1.Message, ex1);
                MessageBox.Show(ex1.Message, "PGE Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                m_application.StatusBar.HideProgressBar();
                m_application.StatusBar.ProgressBar.Position = 0;
                if (pTable_SnapSetting != null) Marshal.ReleaseComObject(pTable_SnapSetting);
                isSaveRunning = false;
            }
        }

    }
}
