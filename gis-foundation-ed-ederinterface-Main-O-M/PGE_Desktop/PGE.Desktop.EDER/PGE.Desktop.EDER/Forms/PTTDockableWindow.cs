using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ADF.CATIDs;
using Miner.ComCategories;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using Telvent.PGE.ED.Desktop.ArcMapCommands.PTTTools;
using Miner.Interop;

namespace Telvent.PGE.ED.Desktop.Forms
{
    [Guid("052c2344-7c28-49e5-9a75-5c5a23ec6528")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("Telvent.PGE.ED.Desktop.Forms.PTTDockableWindow")]
    [ComponentCategory(ComCategory.MxDockableWindow)]
    public partial class PTTDockableWindow : UserControl, IDockableWindowDef, IDockableWindowImageDef
    {
        public static PTTDockableWindow Instance = null;
        public static Bitmap CaptionBitmap = null;
        private IApplication m_application;
        private IWorkspace EditWorkspace = null;
        private List<Pole> Poles = new List<Pole>();
        private List<Pole> CombineToPoles = new List<Pole>();
        private IFeatureClass PTTSupportStructure = null;
        private IFeatureClass SupportStructure = null;

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
            MxDockableWindows.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxDockableWindows.Unregister(regKey);

        }

        #endregion
        #endregion

        public PTTDockableWindow()
        {
            InitializeComponent();
            Instance = this;
        }

        #region IDockableWindowDef Members


        string IDockableWindowDef.Caption
        {
            get
            {
                //TODO: Replace with locale-based initial title bar caption
                return "PTT Reconciliation Tools";
            }
        }

        int IDockableWindowDef.ChildHWND
        {
            get 
            {
                return this.Handle.ToInt32(); 
            }
        }

        string IDockableWindowDef.Name
        {
            get
            {
                return this.Name;
            }
        }

        void IDockableWindowDef.OnCreate(object hook)
        {
            m_application = hook as IApplication;
        }

        void IDockableWindowDef.OnDestroy()
        {
            //TODO: Release resources and call dispose of any ActiveX control initialized
        }

        object IDockableWindowDef.UserData
        {
            get { return null; }
        }

        #endregion

        #region IDockableWindowImageDef

        public int Bitmap
        {
            get
            {
                //Create temporary command to initalize the bitmap
                PTTDockableWindowCommand tempCmd = new PTTDockableWindowCommand();
                int bitmapPTR = (int)CaptionBitmap.GetHbitmap();
                return bitmapPTR;
            }
        }

        #endregion

        #region Public Methods

        public void GetPoleInformation(IWorkspace editWorkspace)
        {
            EditWorkspace = editWorkspace;

            ClearInformation();
            EnableCommands();
            GetInformation();
        }

        public void ClearPoleInformation()
        {
            ClearInformation();
            DisableCommands();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Obtains the information for all of the Inserts / Deletes / Combines
        /// </summary>
        private void GetInformation()
        {
            IMMEnumObjectClass objectClasses = ModelNameManager.Instance.ObjectClassesFromModelNameWS(EditWorkspace, SchemaInfo.General.ClassModelNames.PTTSupportStructure);
            objectClasses.Reset();

            PTTSupportStructure = objectClasses.Next() as IFeatureClass;
            if (PTTSupportStructure == null)
            {
                MessageBox.Show("Unable to find feature class with model name " + SchemaInfo.General.ClassModelNames.PTTSupportStructure);
                return;
            }

            SupportStructure = ((IFeatureWorkspace)EditWorkspace).OpenFeatureClass("EDGIS.SUPPORTSTRUCTURE");
            if (SupportStructure == null)
            {
                MessageBox.Show("Unable to find the EDGIS.SupportStructure feature class");
                return;
            }

            //Watch for undo operations to enable the refresh command
            Miner.Geodatabase.Edit.Editor.OnUndo += new EventHandler<Miner.Geodatabase.Edit.EditEventArgs>(Editor_OnUndo);
            Miner.Geodatabase.Edit.Editor.OnRedo += new EventHandler<Miner.Geodatabase.Edit.EditEventArgs>(Editor_OnRedo);

            //Determine our poles for insert, delete, and combine
            DeterminePolesToProcess();

            //Now let's populate our check boxes
            PopulateCheckboxes();

            //Lable our counts for the user 
            LabelPoleCounts(0, 0, 0);
        }

        /// <summary>
        /// An undo operation was executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_OnRedo(object sender, Miner.Geodatabase.Edit.EditEventArgs e)
        {
            //btnRefresh.Enabled = true;
            //btnRefresh.Visible = true;
        }

        /// <summary>
        /// A redo operation was executed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Editor_OnUndo(object sender, Miner.Geodatabase.Edit.EditEventArgs e)
        {
            //btnRefresh.Enabled = true;
            //btnRefresh.Visible = true;
        }

        /// <summary>
        /// Populates the checkboxes for Inserts / Deletes / Combines
        /// </summary>
        private void PopulateCheckboxes()
        {
            Poles.Sort();
            foreach (Pole pole in Poles)
            {
                if (pole.PoleType == PTTPoleType.Insert)
                {
                    //Add to our inserts checkbox
                    chkPromotes.Items.Add(pole);
                }
                else if (pole.PoleType == PTTPoleType.Delete)
                {
                    chkDeletes.Items.Add(pole);
                }
                else if (pole.PoleType == PTTPoleType.Combine)
                {
                    chkCombine.Items.Add(pole);
                }
            }
        }

        /// <summary>
        /// Determines the poles to process for Inserts / Deletes / Combines
        /// </summary>
        private void DeterminePolesToProcess()
        {
            IQueryFilter qf = new QueryFilterClass();
            qf.SubFields = "OBJECTID,PTTDIDC,SAPEQUIPID,GLOBALID";
            //Where clause to exclude SAPEQUIPID value of -1. That indicates that a pole has already been promoted
            qf.WhereClause = "SAPEQUIPID <> '-1' OR SAPEQUIPID IS NULL";

            int pttdIDCIdx = PTTSupportStructure.Fields.FindField("PTTDIDC");
            int sapEquipIDIdx = PTTSupportStructure.Fields.FindField("SAPEQUIPID");
            int guidIdx = PTTSupportStructure.Fields.FindField("GLOBALID");

            //Iterate over all of the PTT support structures for inserts and deletes
            IFeatureCursor rowCursor = PTTSupportStructure.Search(qf, false);
            IFeature row = null;
            while ((row = rowCursor.NextFeature()) != null)
            {
                //Process each row and add to our list
                int oid = row.OID;
                object pttdIDCObj = row.get_Value(pttdIDCIdx).ToString();
                object sapEquipIDObj = row.get_Value(sapEquipIDIdx);

                string pttdIDC = "";
                if (pttdIDCObj != null) { pttdIDC = pttdIDCObj.ToString(); }

                string sapEquipID = "";
                if (sapEquipIDObj != null) { sapEquipID = sapEquipIDObj.ToString(); }

                string GlobalID = row.get_Value(guidIdx).ToString();

                Pole newPole = new Pole(sapEquipID, GlobalID, oid, PTTPoleType.Insert);
                if (pttdIDC == "Y")
                {
                    //This is a delete
                    newPole.PoleType = PTTPoleType.Delete;
                }

                Poles.Add(newPole);
                while (Marshal.ReleaseComObject(row) > 0) { }
            }
            if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }


            //Find all SupportStructure features with PTTIDC = 'Y'
            pttdIDCIdx = SupportStructure.Fields.FindField("PTTDIDC");
            sapEquipIDIdx = SupportStructure.Fields.FindField("SAPEQUIPID");
            guidIdx = SupportStructure.Fields.FindField("GLOBALID");

            qf.WhereClause = "PTTDIDC = 'Y'";
            rowCursor = SupportStructure.Search(qf, false);
            row = null;
            while ((row = rowCursor.NextFeature()) != null)
            {
                //Process each row and add to our list
                int oid = row.OID;
                object sapEquipIDObj = row.get_Value(sapEquipIDIdx);

                string pttdIDC = "Y";

                string sapEquipID = "";
                if (sapEquipIDObj != null) { sapEquipID = sapEquipIDObj.ToString(); }

                string globalID = row.get_Value(guidIdx).ToString();

                Pole newPole = new Pole(sapEquipID, globalID, oid, PTTPoleType.Delete);

                Poles.Add(newPole);
                while (Marshal.ReleaseComObject(row) > 0) { }
            }
            if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }

            //Finally, iterate over all of the poles identified as inserts and check if they already exist
            //in the support structure table.  If they do, then modify them to be a combine pole.
            List<string> sapEquipIDs = new List<string>();
            foreach (Pole pole in Poles) 
            {
                if (pole.PoleType == PTTPoleType.Insert)
                {
                    sapEquipIDs.Add(pole.SAPEquipID);
                }
            }

            List<string> whereInClauses = GetWhereInClauses(sapEquipIDs);
            foreach (string whereInClause in whereInClauses)
            {
                qf.WhereClause = "SAPEQUIPID IN (" + whereInClause + ")";
                rowCursor = SupportStructure.Search(qf, false);
                row = null;
                while ((row = rowCursor.NextFeature()) != null)
                {
                    //Process each row and add to our list
                    int oid = row.OID;
                    object pttdIDCObj = row.get_Value(pttdIDCIdx).ToString();
                    object sapEquipIDObj = row.get_Value(sapEquipIDIdx);

                    string pttdIDC = "";
                    if (pttdIDCObj != null) { pttdIDC = pttdIDCObj.ToString(); }

                    string sapEquipID = "";
                    if (sapEquipIDObj != null) { sapEquipID = sapEquipIDObj.ToString(); }

                    string globalID = row.get_Value(guidIdx).ToString();

                    Pole newPole = new Pole(sapEquipID, globalID, oid, PTTPoleType.Combine);

                    //Update our PTT pole to be a combine type
                    foreach (Pole pole in Poles)
                    {
                        if (pole.SAPEquipID == newPole.SAPEquipID) { pole.PoleType = PTTPoleType.Combine; }
                    }

                    CombineToPoles.Add(newPole);
                    while (Marshal.ReleaseComObject(row) > 0) { }
                }
                if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
            }
        }

        /// <summary>
        /// Enables Commands
        /// </summary>
        private void EnableCommands()
        {
            tabActions.Enabled = true;
        }

        /// <summary>
        /// Disables Commands
        /// </summary>
        private void DisableCommands()
        {
            tabActions.Enabled = false;
        }

        /// <summary>
        /// Clears any information from the three pole tabs
        /// </summary>
        private void ClearInformation()
        {
            chkPromotes.Items.Clear();
            chkDeletes.Items.Clear();
            chkCombine.Items.Clear();
            Poles.Clear();
            CombineToPoles.Clear();
        }

        private void LabelPoleCounts(int promotesCheckedCount, int deletesCheckedCount, int combineCheckedCount)
        {
            lblPromotesCount.Text = string.Format("{0} Poles to Promote ({1} Selected)", chkPromotes.Items.Count, promotesCheckedCount);
            lblDeletesCount.Text = string.Format("{0} Poles to Delete ({1} Selected)", chkDeletes.Items.Count, deletesCheckedCount);
            lblCombineCount.Text = string.Format("{0} Poles to Combine ({1} Selected)", chkCombine.Items.Count, combineCheckedCount);
        }

        /// <summary>
        /// Refreshes the current Pole check boxes.  Enables when an undo or re-do operation has ocurred
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //Disable our controls while refreshing
            tabActions.Enabled = false;

            try
            {
                //Clear our current information
                ClearInformation();

                //Determine our poles for insert, delete, and combine
                DeterminePolesToProcess();

                //Now let's populate our check boxes
                PopulateCheckboxes();

                //Lable our counts for the user 
                LabelPoleCounts(0, 0, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to refresh: " + ex.Message);
            }

            //Re-enable our controls
            tabActions.Enabled = true;
            //btnRefresh.Visible = false;
            //btnRefresh.Enabled = false;
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
                    if ((((i % 1000) == 0) && i != 0) || (guids.Count == i + 1))
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
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private List<string> GetWhereInClauses(List<string> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                int counter = 0;
                foreach (string guid in guids)
                {
                    if (counter == 999)
                    {
                        builder.Append("'" + guid + "'");
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                        counter = 0;
                    }
                    else
                    {
                        builder.Append("'" + guid + "',");
                        counter++;
                    }
                }

                if (builder.Length > 0)
                {
                    string whereInClause = builder.ToString();
                    whereInClause = whereInClause.Substring(0, whereInClause.LastIndexOf(","));
                    whereInClauses.Add(whereInClause);
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        private void chkPromotes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //Lable our counts for the user 
            int count = 1;
            if (e.NewValue == CheckState.Unchecked) { count = -1; }
            LabelPoleCounts(chkPromotes.CheckedItems.Count + count, chkDeletes.CheckedItems.Count, chkCombine.CheckedItems.Count);
        }

        private void chkDeletes_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //Lable our counts for the user 
            int count = 1;
            if (e.NewValue == CheckState.Unchecked) { count = -1; }
            LabelPoleCounts(chkPromotes.CheckedItems.Count, chkDeletes.CheckedItems.Count + count, chkCombine.CheckedItems.Count);
        }

        private void chkCombine_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            //Lable our counts for the user 
            int count = 1;
            if (e.NewValue == CheckState.Unchecked) { count = -1; }
            LabelPoleCounts(chkPromotes.CheckedItems.Count + count, chkDeletes.CheckedItems.Count, chkCombine.CheckedItems.Count + count);
        }

        #endregion

        #region Promotes, Deletes, Combines

        /// <summary>
        /// Promotes a staging pole to the support structure table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPromotes_Click(object sender, EventArgs e)
        {
            try
            {
                //Start a new edit operation
                Miner.Geodatabase.Edit.Editor.StartOperation();

                List<int> OIDs = new List<int>();
                //Promote the selected poles to the support structure table
                foreach (object selectedItem in chkPromotes.CheckedItems)
                {
                    Pole selectedPole = selectedItem as Pole;
                    OIDs.Add(selectedPole.ObjectID);
                }

                List<string> whereInClauses = GetWhereInClauses(OIDs);
                foreach (string whereInClause in whereInClauses)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = PTTSupportStructure.OIDFieldName + " in (" + whereInClause + ")";

                    IFeatureCursor featCursor = PTTSupportStructure.Search(qf, false);
                    IFeature poleToCopy = null;
                    while ((poleToCopy = featCursor.NextFeature()) != null)
                    {
                        //Copy all of the attributes
                        IFeature newPole = SupportStructure.CreateFeature();
                        newPole.Shape = poleToCopy.ShapeCopy;

                        for (int i = 0; i < newPole.Fields.FieldCount; i++)
                        {
                            IField field = newPole.Fields.get_Field(i);
                            if (field.Editable)
                            {
                                //This field is editable, so let's set it
                                newPole.set_Value(i, poleToCopy.get_Value(i));
                            }
                        }

                        newPole.Store();
                        while (Marshal.ReleaseComObject(poleToCopy) > 0) { }
                    }

                    if (poleToCopy != null) { while (Marshal.ReleaseComObject(poleToCopy) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                }

                //Now we need to go through all of the staging table poles and set the sap equipment id's to -1
                //This will notify SAP that this is promoting a staging pole and to update the GUID
                foreach (string whereInClause in whereInClauses)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = PTTSupportStructure.OIDFieldName + " in (" + whereInClause + ")";

                    int sapEquipIDIdx = PTTSupportStructure.FindField("SAPEQUIPID");
                    IFeatureCursor featCursor = PTTSupportStructure.Search(qf, false);
                    IFeature stagingPoleToDelete = null;
                    while ((stagingPoleToDelete = featCursor.NextFeature()) != null)
                    {
                        stagingPoleToDelete.set_Value(sapEquipIDIdx, "-1");
                        stagingPoleToDelete.Store();

                        while (Marshal.ReleaseComObject(stagingPoleToDelete) > 0) { }
                    }

                    if (stagingPoleToDelete != null) { while (Marshal.ReleaseComObject(stagingPoleToDelete) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                }

                //Stop our edit operation
                Miner.Geodatabase.Edit.Editor.StopOperation("Promote Staging Poles");

                //Remove the processed poles from our list
                List<Pole> polesToRemove = new List<Pole>();
                foreach (object selectedItem in chkPromotes.CheckedItems)
                {
                    polesToRemove.Add(selectedItem as Pole);
                }

                foreach (Pole poleToRemove in polesToRemove) { chkPromotes.Items.Remove(poleToRemove); }

                LabelPoleCounts(chkPromotes.CheckedItems.Count, chkDeletes.CheckedItems.Count, chkCombine.CheckedItems.Count);
            }
            catch (Exception ex)
            {
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.AbortOperation(); }
                MessageBox.Show("Failed to promote staging poles: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes the poles that were marked with the PTTDIDC
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDeletePoles_Click(object sender, EventArgs e)
        {
            try
            {
                //Start a new edit operation
                Miner.Geodatabase.Edit.Editor.StartOperation();

                List<int> OIDs = new List<int>();
                //Promote the selected poles to the support structure table
                foreach (object selectedItem in chkDeletes.CheckedItems)
                {
                    Pole selectedPole = selectedItem as Pole;
                    OIDs.Add(selectedPole.ObjectID);
                }

                List<string> whereInClauses = GetWhereInClauses(OIDs);

                //Delete any that exist in the staging table
                foreach (string whereInClause in whereInClauses)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = PTTSupportStructure.OIDFieldName + " in (" + whereInClause + ") AND PTTDIDC = 'Y'";

                    IFeatureCursor featCursor = PTTSupportStructure.Search(qf, false);
                    IFeature poleToDelete = null;
                    while ((poleToDelete = featCursor.NextFeature()) != null)
                    {
                        //Delete the specified pole from the staging table
                        poleToDelete.Delete();
                        while (Marshal.ReleaseComObject(poleToDelete) > 0) { }
                    }

                    if (poleToDelete != null) { while (Marshal.ReleaseComObject(poleToDelete) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                }

                //Delete any that exist in the staging table
                foreach (string whereInClause in whereInClauses)
                {
                    IQueryFilter qf = new QueryFilterClass();
                    qf.WhereClause = SupportStructure.OIDFieldName + " in (" + whereInClause + ") AND PTTDIDC = 'Y'";

                    IFeatureCursor featCursor = SupportStructure.Search(qf, false);
                    IFeature poleToDelete = null;
                    while ((poleToDelete = featCursor.NextFeature()) != null)
                    {
                        //Delete the specified pole from the staging table
                        poleToDelete.Delete();
                        while (Marshal.ReleaseComObject(poleToDelete) > 0) { }
                    }

                    if (poleToDelete != null) { while (Marshal.ReleaseComObject(poleToDelete) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
                }

                //Stop our edit operation
                Miner.Geodatabase.Edit.Editor.StopOperation("Delete Poles");

                //Remove the processed poles from our list
                List<Pole> polesToRemove = new List<Pole>();
                foreach (object selectedItem in chkDeletes.CheckedItems)
                {
                    polesToRemove.Add(selectedItem as Pole);
                }

                foreach (Pole poleToRemove in polesToRemove) { chkDeletes.Items.Remove(poleToRemove); }

                LabelPoleCounts(chkPromotes.CheckedItems.Count, chkDeletes.CheckedItems.Count, chkCombine.CheckedItems.Count);
            }
            catch (Exception ex)
            {
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.AbortOperation(); }
                MessageBox.Show("Failed to delete selected poles: " + ex.Message);
            }
        }


        private void btnCombine_Click(object sender, EventArgs e)
        {
            try
            {
                //Start a new edit operation
                Miner.Geodatabase.Edit.Editor.StartOperation();

                List<int> OIDs = new List<int>();
                //Promote the selected poles to the support structure table
                foreach (object selectedItem in chkCombine.CheckedItems)
                {
                    Pole selectedPole = selectedItem as Pole;

                    IFeature pole = GetFeatureByGuid(SupportStructure, selectedPole.SAPEquipID);
                    IFeature stagingPole = GetFeatureByGuid(PTTSupportStructure, selectedPole.SAPEquipID);

                    PTTCombinePole combinePoleWindow = new PTTCombinePole(pole, stagingPole, selectedPole);
                    combinePoleWindow.ShowDialog(new ArcMapWindow(m_application));

                    
                }

                //Stop our edit operation
                Miner.Geodatabase.Edit.Editor.StopOperation("Combine Poles");

                //Remove the processed poles from our list
                List<Pole> polesToRemove = new List<Pole>();
                foreach (object selectedItem in chkCombine.CheckedItems)
                {
                    polesToRemove.Add(selectedItem as Pole);
                }

                foreach (Pole poleToRemove in polesToRemove) { chkCombine.Items.Remove(poleToRemove); }

                LabelPoleCounts(chkPromotes.CheckedItems.Count, chkDeletes.CheckedItems.Count, chkCombine.CheckedItems.Count);
            }
            catch (Exception ex)
            {
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.AbortOperation(); }
                MessageBox.Show("Failed to combine selected poles: " + ex.Message);
            }
        }

        private IFeature GetFeatureByGuid(IFeatureClass featClass, string globalID)
        {
            IQueryFilter qf = new QueryFilterClass();
            qf.WhereClause = "SAPEQUIPID = '" + globalID + "'";

            IFeatureCursor featCursor = featClass.Search(qf, false);
            IFeature feature = featCursor.NextFeature();

            while (Marshal.ReleaseComObject(qf) > 0) { }
            while (Marshal.ReleaseComObject(featCursor) > 0) { }

            return feature;
        }

        #endregion

    }

    /// <summary>
    /// Class to hold pole information for the PTT Reconciliation Tools
    /// </summary>
    public class Pole : IComparable<Pole>
    {
        public Pole(string sapEquipID, string globalID, int OID, PTTPoleType type)
        {
            _SAPEquipID = sapEquipID;
            _GlobalID = globalID;
            _objectID = OID;
            _poleType = type;
        }

        private string _SAPEquipID = "";
        public string SAPEquipID
        {
            get { return _SAPEquipID; }
        }

        private string _GlobalID = "";
        public string GlobalID
        {
            get { return _GlobalID; }
        }

        private int _objectID = -1;
        public int ObjectID
        {
            get { return _objectID; }
        }

        private PTTPoleType _poleType;
        public PTTPoleType PoleType
        {
            get { return _poleType; }
            set { _poleType = value; }
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(SAPEquipID)) { return "SAP Equipment ID: " + SAPEquipID; }
            else { return "Global ID: " + GlobalID.ToUpper(); }
        }

        public int CompareTo(Pole other)
        {
            string compareOne = SAPEquipID;
            if (string.IsNullOrEmpty(SAPEquipID)) { compareOne = GlobalID.ToUpper(); }

            string compareTwo = other.SAPEquipID;
            if (string.IsNullOrEmpty(SAPEquipID)) { compareTwo = other.GlobalID.ToUpper(); }

            return compareOne.CompareTo(compareTwo);
        }
    }

    public enum PTTPoleType
    {
        Insert,
        Delete,
        Combine
    }
}
