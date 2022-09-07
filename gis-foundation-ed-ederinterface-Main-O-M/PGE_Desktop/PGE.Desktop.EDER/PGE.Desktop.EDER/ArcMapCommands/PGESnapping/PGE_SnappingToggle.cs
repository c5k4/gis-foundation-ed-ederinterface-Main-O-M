using System;
using System.Runtime.InteropServices;
using Miner.ComCategories;
using Miner.Interop;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using PGE.Common.Delivery.UI.Commands;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Controls;
using Miner.Geodatabase.Edit;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using System.Security.Principal;
using ESRI.ArcGIS.Geodatabase;
using Oracle.DataAccess.Client;
using System.IO;


namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// PGE Toggle Snapping Command 
    /// </summary>
    [Guid("51698800-1DF8-47EC-992B-01D2A4D764EE")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    [ProgId("PGE.Desktop.EDER.ArcMapCommands.PGE_SnappingToggle")]
    [ComVisible(true)]
    public sealed class PGE_SnappingToggle : BaseArcGISCommand
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcMapCommands.Unregister(regKey);
        }

        #endregion

        #region Private Varibales
        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application = null;
        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        public static string tableName_Snapping = "EDGIS.T_SNAPPING";
        public static IWorkspace _loginWorkspace;

        #endregion Private Variables

        #region Constructor
        /// <summary>
        /// Creates an instance of <see cref="PGE_SnappingCommand"/>
        /// </summary>
        public PGE_SnappingToggle() :
            base("PGE_SnappingToggle", "Toggle", "", "Toggle Snapping", "Toggle Snapping")
        {
            base.m_name = "PGE_SnappingToggle";
            try
            {
                Miner.Geodatabase.Edit.Editor.OnStartEditing += Editor_OnStartEditing;  //Load Snapping Type on Start Editing

            }
            catch (Exception ex)
            {
                _logger.Warn("Error in Toggle Snapping: " + ex.Message);
                System.Diagnostics.Trace.WriteLine(ex.Message, "PGE Snapping");
            }
        }
        #endregion Constructor
       
        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            if (hook == null) return;
            _application = hook as IApplication;
        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        protected override void InternalClick()
        {
            string Snapper = string.Empty;
            try
            {
                UID pID = new UIDClass();
                pID.Value = "esriEditor.Editor";
                IEditProperties4 editPropObj = _application.FindExtensionByCLSID(pID) as IEditProperties4;
                editPropObj.ClassicSnapping = !(editPropObj.ClassicSnapping);
                if (editPropObj.ClassicSnapping == true)
                {
                    _application.StatusBar.set_Message(0, "Classic Snapping is ON...");
                    Snapper = "Y";
                }
                else
                {
                    _application.StatusBar.set_Message(0, "Classic Snapping is OFF...");
                    Snapper = "N";
                }


                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                if (wi != null)
                {
                    IWorkspace loginWorkspace = GetWorkspace();

                    if (loginWorkspace != null)
                    {
                        if (((IWorkspace2)loginWorkspace).NameExists[esriDatasetType.esriDTTable, tableName_Snapping])
                        {

                           

                            IFeatureWorkspace pFWS = (IFeatureWorkspace)loginWorkspace;
                            ITable pTable = pFWS.OpenTable(tableName_Snapping);
                            int fieldFrstIndex = pTable.FindField("LANID");
                            int fieldSecIndex = pTable.FindField("ISCLASSIC");
                            int fieldthirdIndex = pTable.FindField("CLASSIC_XML");

                            string UserName = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);

                            //check if name exists
                            IQueryFilter queryFilter = new QueryFilterClass();
                            queryFilter.WhereClause = " LANID ='" + UserName +"'" ;
                            ICursor Cursor = pTable.Search(queryFilter, true);
                            IRow pRow = Cursor.NextRow();
                            if (pRow != null)
                            {
                                pRow.set_Value(fieldSecIndex, Snapper);
                                pRow.Store();
                            }

                            else
                            {
                                //insert row
                                IRow row = pTable.CreateRow();
                                row.set_Value(fieldFrstIndex, UserName);
                                row.set_Value(fieldSecIndex, Snapper);
                                row.Store();
                            }
                        }
                    }
                }

               // PGEExtension.BlobFileToTable();
               
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                MessageBox.Show(ex.Message);
            }
        }

        #endregion Overridden Class Methods

        #region Events
        /// <summary>
        /// Called when the application moves into edit mode.
        /// </summary>
        private void Editor_OnStartEditing(object sender, Miner.Geodatabase.Edit.EditEventArgs e)
        {
            try
            {
                ////Check Snapping Type from Database
                WindowsIdentity wi = WindowsIdentity.GetCurrent();
                if (wi != null)
                {
                    IWorkspace loginWorkspace = GetWorkspace();

                    if (loginWorkspace != null)
                    {
                        if (((IWorkspace2)loginWorkspace).NameExists[esriDatasetType.esriDTTable, tableName_Snapping])
                        {
                            //query on Snapping table and find if default Snapping for user is Classic / New
                            ITable snappingTable = ((IFeatureWorkspace)loginWorkspace).OpenTable(tableName_Snapping);
                            string UserName = wi.Name.Substring(wi.Name.LastIndexOf("\\") + 1);
                            if (snappingTable != null)
                            {
                                IQueryFilter pQf = new QueryFilterClass();
                                pQf.SubFields = "ISCLASSIC";
                                pQf.WhereClause = "LANID='" + UserName + "'";

                                ICursor pCur = snappingTable.Search(pQf, true);
                               // string transGlobalIds = getTransGlobalIds(pCur);


                                UID pID = new UIDClass();
                                pID.Value = "esriEditor.Editor";
                                IEditProperties4 editPropObj = _application.FindExtensionByCLSID(pID) as IEditProperties4;

                                if (editPropObj != null)
                                {
                                    IRow pRow = pCur.NextRow();
                                    if (pRow == null)
                                    {

                                        editPropObj.ClassicSnapping = true;
                                    }
                                    else
                                    {
                                        int ClassicFieldIndex = pRow.Fields.FindField("ISCLASSIC");
                                        string snapper = Convert.ToString(pRow.get_Value(ClassicFieldIndex));
                                        if (snapper == "Y")
                                        {
                                            editPropObj.ClassicSnapping = true;
                                        }
                                        else
                                        {
                                            editPropObj.ClassicSnapping = false;
                                        }
                                    }
                                }

                                Marshal.ReleaseComObject(pCur);

                            }
                        }
                        else
                        {
                            MessageBox.Show(tableName_Snapping + " table was not found in the logged in workspace", "Table Not Found", System.Windows.Forms.MessageBoxButtons.OK);
                           // ResultLabel.Text = "";
                           // sProgressor.Message = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PGE_SnappingToggle" + ex.Message, ex);
                //MessageBox.Show("Error in fetching default Snapping settings from database.", "Error", System.Windows.Forms.MessageBoxButtons.OK);
            }
        }

        #endregion Events

        #region Helper Methods

        public static IWorkspace GetWorkspace()
        {
            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            _loginWorkspace = utils.LoginWorkspace;
            return _loginWorkspace;
        }

        #endregion Helper Methods

    }
}
