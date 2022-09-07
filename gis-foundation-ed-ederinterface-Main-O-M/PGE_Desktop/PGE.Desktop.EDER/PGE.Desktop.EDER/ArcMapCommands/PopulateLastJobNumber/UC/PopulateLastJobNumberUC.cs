using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using log4net;
using System.Reflection;
using PGE.Common.Delivery.Diagnostics;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.Framework;
using PGE.Common.Delivery.ArcFM;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop.Process;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    public partial class PopulateLastJobNumberUC : UserControl
    {
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public static string jobNumber = null;
        private static string lastEditedVersion = null;
        //ME Q3 Substation Issue
        private static string _sessionExtentTableName = "EDGIS.SUBS_SESSION_EXTENT";
        private static string _fieldNameMapExtent = "MAP_EXTENT";
        private static string _fieldNameSessionName = "SESSION_NAME";
        private static string _fieldNameSD = "STORED_DISPLAY";
        private static string _storedDisplayName = "Substation Master";
        /// <summary>
        /// Constructs a new instance of the PopulateLastJobNUmber control.
        /// </summary>
        public PopulateLastJobNumberUC()
        {
            InitializeComponent();
            jobNumber = null;
            Miner.Geodatabase.Edit.Editor.OnStartEditing += Editor_OnStartEditing;
            Miner.Geodatabase.Edit.Editor.OnStopEditing += Editor_OnStopEditing;

            if (Miner.Geodatabase.Edit.Editor.EditState == Miner.Geodatabase.Edit.EditState.StateNotEditing)
            {
                JobNumberTxtBox.Enabled = false;
            }
            else
            {
                JobNumberTxtBox.Enabled = true;
            }
        }

        /// <summary>
        /// Called when the application moves out of edit mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_OnStopEditing(object sender, Miner.Geodatabase.Edit.SaveEditEventArgs e)
        {
            buttonCancel_Click(sender, e);
            JobNumberTxtBox.Enabled = false;


            //ME Q3 SUBSTATION ISSUE
            IWorkspace ws = GetWorkspace();
            string sCurrentStoreddisplayName = getSelectedStoredDisplay();
            //IList<IMMStoredDisplayName> listSystemStoredDisplayName = GetStoredDisplayNames(storedDispMgr, mmStoredDisplayType.mmSDTSystem);
            if (sCurrentStoreddisplayName == _storedDisplayName)
            {
                saveMapExtent(ws, sCurrentStoreddisplayName);
            }
        }

        /// <summary>
        /// Called when the application moves into edit mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_OnStartEditing(object sender, Miner.Geodatabase.Edit.EditEventArgs e)
        {
            if (lastEditedVersion == null || lastEditedVersion != ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName)
            {
                jobNumber = null;
                JobNumberTxtBox.Text = "";
                lastEditedVersion = ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName;
            }
            JobNumberTxtBox.Enabled = true;

            //ME Q3 Subtsation Issue
            string sCurrentStoreddisplayName = getSelectedStoredDisplay();
            if (sCurrentStoreddisplayName == _storedDisplayName)
            {
                setMapExtent();
            }

        }


        /// <summary>
        /// On textbox focus lost it will restore the jobnumber.
        /// </summary>
        /// <param name="sender">Sender of this event</param>
        /// <param name="e">Event arguments</param>
        private void JobNumberTxtBox_Leave(object sender, EventArgs e)
        {
            buttonCancel_Click(sender, e);
        }

        /// <summary>
        /// Performs input validation and, if passed, updates the job number and informs the user.
        /// Otherwise, informs the user that something is wrong with the input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOk_Click(object sender, EventArgs e)
        {
            JobNumberTxtBox.Text = JobNumberTxtBox.Text.Trim();
            if (JobNumberTxtBox.Text.Length > 11 || JobNumberTxtBox.Text.Length == 0)
            {
                MessageBox.Show("Job number length must be 1 to 11 characters.", "Info", MessageBoxButtons.OK);
                return;
            }
            //Update the value in the static properties.
            jobNumber = JobNumberTxtBox.Text;
            DisableButtons(sender, e);
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            JobNumberTxtBox.Text = jobNumber;
            DisableButtons(sender, e);
        }

        /// <summary>
        /// Used to indicate to the user that this tool has changes waiting.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EnableButtons(object sender, EventArgs e)
        {
            // buttonOk.Enabled = true;
            // buttonCancel.Enabled = true;
        }
        /// <summary>
        /// Used to indicate to the user that there are no changes, and that any previous change has been applied.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisableButtons(object sender, EventArgs e)
        {
            // buttonOk.Enabled = false;
            // buttonCancel.Enabled = false;
        }

        /// <summary>
        /// Handles shortcuts for the OK and Cancel buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JobNumberTxtBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.Handled = true;
                    buttonOk_Click(sender, e);
                    return;
                case Keys.Escape:
                    e.Handled = true;
                    buttonCancel_Click(sender, e);
                    return;
            }
        }

        #region ME Q3 Substation Issue
        private void ZoomToUserGivenExtents(string strExtentsCoordinates, IApplication objApp)
        {
            try
            {
                IMxDocument objMxDoc = (IMxDocument)objApp.Document;
                IEnvelope objEnvlp = objMxDoc.ActiveView.Extent;
                string[] strExtents = Convert.ToString(strExtentsCoordinates).Split(',');
                if ((strExtents != null) & ((strExtents.Length == 4)))
                {
                    objEnvlp.XMin = Convert.ToDouble(strExtents[0]);
                    objEnvlp.YMin = Convert.ToDouble(strExtents[1]);
                    objEnvlp.XMax = Convert.ToDouble(strExtents[2]);
                    objEnvlp.YMax = Convert.ToDouble(strExtents[3]);
                    objMxDoc.ActiveView.Extent = objEnvlp;
                    objMxDoc.FocusMap.SelectByShape(objEnvlp, (objApp as IMxApplication).SelectionEnvironment, true);
                }
                objApp.RefreshWindow();


            }
            catch (Exception CEx)
            {
                //throw CEx;
            }

        }
        private string getSelectedStoredDisplay()
        {
            Type type = Type.GetTypeFromProgID("esriSystem.ExtensionManager");
            object objExt = Activator.CreateInstance(type);
            IExtensionManager extMgr = objExt as IExtensionManager;
            IMMStoredDisplayManager objStoredDisplayMan = (IMMStoredDisplayManager)extMgr.FindExtension("MMStoredDisplayMgr");
            string sCurrentStoreddisplayName = objStoredDisplayMan.CurrentStoredDisplayName();
            return sCurrentStoreddisplayName;
        }
        private void saveMapExtent(IWorkspace ws, string sCurrentStoreddisplayName)
        {
            ITable T_SessionExtent = null;
            IRow row = null;
            IApplication application = ApplicationFacade.Application;
            IMxDocument mxDoc = (application.Document as IMxDocument);
            IActiveView view = mxDoc.FocusMap as IActiveView;
            IEnvelope currExtent = view.Extent;
            StringBuilder sbMapExtent = new StringBuilder();
            sbMapExtent.Append(currExtent.XMin);
            sbMapExtent.Append(",");
            sbMapExtent.Append(currExtent.YMin);
            sbMapExtent.Append(",");
            sbMapExtent.Append(currExtent.XMax);
            sbMapExtent.Append(",");
            sbMapExtent.Append(currExtent.YMax);

            IFeatureWorkspace fWS = ws as IFeatureWorkspace;
            T_SessionExtent = fWS.OpenTable(_sessionExtentTableName);


            string strVer = ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName;

            IQueryFilter pQf = null;
            ICursor cur = null;
            pQf = new QueryFilterClass();
            pQf.WhereClause = _fieldNameSessionName + "='" + strVer + "'";
            if ((cur = T_SessionExtent.Search(pQf, true)) != null)
            {
                try
                {
                    IRow pRow = cur.NextRow();
                    if (pRow != null)
                    {

                        pRow.set_Value(T_SessionExtent.Fields.FindField(_fieldNameSessionName), strVer);
                        pRow.set_Value(T_SessionExtent.Fields.FindField(_fieldNameMapExtent), sbMapExtent.ToString());
                        pRow.set_Value(T_SessionExtent.Fields.FindField(_fieldNameSD), sCurrentStoreddisplayName);
                        pRow.Store();

                    }
                    else
                    {
                        row = T_SessionExtent.CreateRow();
                        row.set_Value(T_SessionExtent.Fields.FindField(_fieldNameSessionName), strVer);
                        row.set_Value(T_SessionExtent.Fields.FindField(_fieldNameMapExtent), sbMapExtent.ToString());
                        row.set_Value(T_SessionExtent.Fields.FindField(_fieldNameSD), sCurrentStoreddisplayName);
                        row.Store();
                    }
                }
                catch (Exception ex)
                {
                }
            }


        }
        private void setMapExtent()
        {
            IWorkspace ws = GetWorkspace();
            ITable T_SessionExtent = null;
            IApplication application = ApplicationFacade.Application;
            IMxDocument mxDoc = (application.Document as IMxDocument);
            IActiveView view = mxDoc.FocusMap as IActiveView;
            IEnvelope currExtent = view.Extent;
            IFeatureWorkspace fWS = ws as IFeatureWorkspace;
            T_SessionExtent = fWS.OpenTable(_sessionExtentTableName);

            string strVer = ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName;
            IQueryFilter pQf = null;
            ICursor cur = null;
            pQf = new QueryFilterClass();
            pQf.WhereClause = _fieldNameSessionName + "='" + strVer + "'";
            if ((cur = T_SessionExtent.Search(pQf, true)) != null)
            {
                try
                {


                    for (IRow pRow = cur.NextRow(); pRow != null; pRow = cur.NextRow())
                    {
                        string extents = Convert.ToString(pRow.get_Value(T_SessionExtent.Fields.FindField(_fieldNameMapExtent)));
                        ZoomToUserGivenExtents(extents, application);
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
        private IWorkspace GetWorkspace()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;


            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            return utils.LoginWorkspace;

        }
        #endregion

    }
}
