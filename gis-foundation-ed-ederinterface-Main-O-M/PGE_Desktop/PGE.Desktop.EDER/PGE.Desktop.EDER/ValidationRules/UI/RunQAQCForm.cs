using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;

using Miner.ComCategories;
using Miner.Interop;
using Miner.Geodatabase.Edit; 

using PGE.Common.Delivery.UI.Commands;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.ArcFM;
using PGE.Desktop.EDER.AutoTextElements;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


namespace PGE.Desktop.EDER.ValidationRules.UI
{
    public partial class RunQAQCForm : Form
    {
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config"); 
        private IApplication _application = null;
        private IWorkspace _pWS = null; 
        private bool _suppressEvents = false;
        private Hashtable _hshFCs = new Hashtable();
        ContextMenuStrip _treeContextMenu = null;


        //INC000004230713, INC000003945000
        private Hashtable _hshTables = new Hashtable(); 
        private const string ORPHAN_TEXT = "must participate in one composite relationship as a child";

        public RunQAQCForm(IApplication pApp, IWorkspace pWS)
        {
            InitializeComponent();
            _application = pApp;
            _pWS = pWS; 
            
            //Load the treeview with validation rules
            LoadRules(ValidationEngine.Instance.Severities);

            //INC000004230713, INC000003945000
            btnExpandAll.Enabled = false; 
            btnCollapseAll.Enabled = false;
        }

        private ITable GetQAQCTable(IWorkspace ws)
        {
            IMMEnumTable QAQCTableEnum = Miner.Geodatabase.ModelNameManager.Instance.TablesFromModelNameWS(ws, SchemaInfo.Electric.ClassModelNames.SessionQAQC);
            QAQCTableEnum.Reset();
            ITable QAQCTable = null;
            QAQCTable = QAQCTableEnum.Next();
            if (QAQCTable == null)
            {
                throw new Exception("Unable to find table with model name: " + SchemaInfo.Electric.ClassModelNames.SessionQAQC);
            }
            return QAQCTable;
        }

        private void btnPostingErrors_Click(object sender, EventArgs e)
        {
            ITable QAQCTable = null;
            IQueryFilter qf = new QueryFilterClass();
            ICursor rowCursor = null;
            IRow row = null;
            try
            {
                //Load the hash table from the row found in the QAQC session errors table
                QAQCTable = GetQAQCTable(Miner.Geodatabase.Edit.Editor.EditWorkspace);
                qf.WhereClause = "SESSIONNAME = '" + ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName + "'";
                rowCursor = QAQCTable.Search(qf, false);
                row = rowCursor.NextRow();

                if (row != null)
                {
                    int errorsIdx = row.Fields.FindField("ERRORSLIST");
                    object errorsValue = row.get_Value(errorsIdx);
                    if (errorsValue != null)
                    {

                        tvwResults.Nodes.Clear();

                        //INC000004230713, INC000003945000
                        btnExpandAll.Enabled = false; 
                        btnCollapseAll.Enabled = false;
                        List<PGEError> QAQCErrorList = new List<PGEError>();
                        Hashtable qaqcResultsHash = new Hashtable();
                        if (errorsValue.ToString().Contains("Deleted feature"))
                        {
                            string[] Values= errorsValue.ToString().Split(':');
                            PGEDifferenceType m_differenceType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;
                            string m_datasetName = Values[1].ToUpper() ;
                                int m_severity =0;
                                int m_ObjectId = 10000;
                                int m_Type =0;
                                string m_errorMsg = Values[0].ToString();
                                string m_ruleName ="PGE Validate Delete Features";
                                PGEError newError = new PGEError(m_differenceType, m_datasetName, m_severity, m_ObjectId, m_Type, m_errorMsg, m_ruleName);
                                QAQCErrorList.Add(newError);
                            
                        }
                        else
                        {
                            XmlSerializer QAQCResultsSerializer = new XmlSerializer(typeof(List<PGEError>));
                            //XMLReader reader = new XMLReader();
                            XmlTextReader reader = new XmlTextReader(new StringReader(errorsValue.ToString()));
                           

                            while (reader.Read())
                            {
                                if (reader.Name.ToString() == "PGEError")
                                {
                                    PGEDifferenceType m_differenceType = (PGEDifferenceType)Int32.Parse(reader["DifferenceType"]);
                                    string m_datasetName = reader["DatasetName"];
                                    int m_severity = Int32.Parse(reader["Severity"]);
                                    int m_ObjectId = Int32.Parse(reader["ObjectID"]);
                                    int m_Type = Int32.Parse(reader["ErrorType"]);
                                    string m_errorMsg = reader["ErrorMessage"];
                                    string m_ruleName = reader["RuleName"];
                                    PGEError newError = new PGEError(m_differenceType, m_datasetName, m_severity, m_ObjectId, m_Type, m_errorMsg, m_ruleName);
                                    QAQCErrorList.Add(newError);
                                }
                            }
                        }
                        int counter = 0;
                        foreach (PGEError error in QAQCErrorList)
                        {
                            qaqcResultsHash.Add(counter, error);
                            counter++;
                        }

                        //Display results in treeview 
                        ValidationEngine.Instance.LoadResults(
                            true,
                            tvwResults,
                            qaqcResultsHash);

                        //INC3891098 - should switch to the results tab automatically after running 
                        //validation 
                        if (tvwResults.Nodes.Count > 0)
                        {
                            tabQAQC.SelectedTab = pgeResults;
                            btnExpandAll.Enabled = true; //INC000004230713, INC000003945000
                            btnCollapseAll.Enabled = true;
                        }
                        else
                        {
                            btnExpandAll.Enabled = false; //INC000004230713, INC000003945000
                            btnCollapseAll.Enabled = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No errors were found from the posting process");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed loading error results identified in posting process");
            }
            finally
            {
                if (QAQCTable != null) { while (Marshal.ReleaseComObject(QAQCTable) > 0) { } }
                if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
                if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
            }
        }

        private void btnRunQAQC_Click(object sender, EventArgs e)
        {
            try
            {
                //Set the val selection
                Cursor = Cursors.WaitCursor;
                tvwResults.Nodes.Clear();

                //INC000004230713, INC000003945000
                btnExpandAll.Enabled = false; 
                btnCollapseAll.Enabled = false;

                bool isVersionDiff = false;
                if (optSelection.Checked)
                    ValidationEngine.Instance.SelectionType =
                        ValidationSelectionType.ValidationSelectionTypeSelection;
                else
                {
                    ValidationEngine.Instance.SelectionType =
                        ValidationSelectionType.ValidationSelectionTypeVersionDiff;
                    isVersionDiff = true;
                }

                //Set the val filter 
                if (optRulesSeverityError.Checked)
                    ValidationEngine.Instance.FilterType =
                        ValidationFilterType.valFilterTypeErrorOnly;
                else if (optRulesSeverityWarning.Checked)
                    ValidationEngine.Instance.FilterType =
                        ValidationFilterType.valFilterTypeWarningOnly;
                else if (optRulesAll.Checked)
                    ValidationEngine.Instance.FilterType =
                        ValidationFilterType.valFilterTypeAll;
                else if (optRulesSelection.Checked)
                {
                    ValidationEngine.Instance.FilterType =
                        ValidationFilterType.valFilterTypeSelection;
                    Hashtable hshSelectedRules = new Hashtable();
                    TreeNode pErrorsNode = tvwRules.Nodes[0];
                    TreeNode pWarningsNode = tvwRules.Nodes[1];

                    foreach (TreeNode pRuleNode in pErrorsNode.Nodes)
                    {
                        if (pRuleNode.Checked)
                        {
                            if (!hshSelectedRules.ContainsKey(pRuleNode.Text))
                                hshSelectedRules.Add(pRuleNode.Text, 0);
                        }
                    }

                    foreach (TreeNode pRuleNode in pWarningsNode.Nodes)
                    {
                        if (pRuleNode.Checked)
                        {
                            if (!hshSelectedRules.ContainsKey(pRuleNode.Text))
                                hshSelectedRules.Add(pRuleNode.Text, 0);
                        }
                    }

                    ValidationEngine.Instance.SelectedQAQCRules =
                        hshSelectedRules;
                }

                //Run the QAQC                 
                //Get the version difference if necessary 
                Hashtable hshErrorList = new Hashtable();
                if (ValidationEngine.Instance.SelectionType ==
                            ValidationSelectionType.ValidationSelectionTypeVersionDiff)
                {
                    esriDifferenceType[] pDiffTypes = new esriDifferenceType[6];
                    pDiffTypes[0] = esriDifferenceType.esriDifferenceTypeInsert;
                    pDiffTypes[1] = esriDifferenceType.esriDifferenceTypeUpdateDelete;
                    pDiffTypes[2] = esriDifferenceType.esriDifferenceTypeUpdateNoChange;
                    pDiffTypes[3] = esriDifferenceType.esriDifferenceTypeUpdateUpdate;
                    pDiffTypes[4] = esriDifferenceType.esriDifferenceTypeDeleteNoChange;
                    pDiffTypes[5] = esriDifferenceType.esriDifferenceTypeDeleteUpdate;

                    IVersionedWorkspace pVWS = (IVersionedWorkspace)Editor.EditWorkspace;
                    ValidationEngine.Instance.Application = _application;
                    System.Collections.Hashtable hshVersionDiffObjects =
                        ValidationEngine.Instance.GetVersionDifferences(
                            pVWS,
                            pDiffTypes, false);

                    _logger.Debug("Version Difference from 'DesktopVersionDifference' class successful");
                    if (hshVersionDiffObjects.Count != 0)
                    {
                        _logger.Debug("Running QA QC on the Version Difference");
                        _logger.Debug("Found: " + hshVersionDiffObjects.Count.ToString() + " version difference objects");

                        //Get list of FC names, and for those FCs QA/QC errors are displayed as 'ERRORS' and not as 'WARNINGS'. SubPNode requirement. April-16th-2019.
                        PGEError.FeatureClassList_ForceErrors(Editor.EditWorkspace);

                        //Run the QAQC and generate the error list                         
                        ValidationEngine.Instance.RunQAQCCustomised(
                            hshVersionDiffObjects, ref hshErrorList);
                    }
                    else
                    {
                        MessageBox.Show("No version differences found!",
                            "PGE Custom QAQC",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                else if (ValidationEngine.Instance.SelectionType ==
                            ValidationSelectionType.ValidationSelectionTypeSelection)
                {
                    IMxDocument pMxDoc = (IMxDocument)_application.Document;
                    ISelection pSelection = (ISelection)pMxDoc.FocusMap.FeatureSelection;
                    IMap pMap = pMxDoc.FocusMap;

                    //Hashtable to store the map selection objects
                    Hashtable hshValidationObjects = new Hashtable();

                    //1. Look for standalone table selection 
                    IStandaloneTableCollection pStandAloneTableCollection =
                        (IStandaloneTableCollection)pMap;
                    IStandaloneTable pStandaloneTable = null;
                    ITable pTable = null;
                    for (int i = 0; i <= pStandAloneTableCollection.StandaloneTableCount - 1; i++)
                    {
                        pStandaloneTable = pStandAloneTableCollection.get_StandaloneTable(i);
                        pTable = (ITable)pStandaloneTable;
                        if (pTable == null) { continue; }

                        //get selection set from table
                        ITableSelection pTableSel = (ITableSelection)pStandaloneTable;
                        if (pTableSel == null) { continue; }

                        ISelectionSet pSelSet = pTableSel.SelectionSet;
                        if (pSelSet == null || pSelSet.IDs == null) { continue; }

                        IEnumIDs enumIDs = pSelSet.IDs;
                        int iD = enumIDs.Next();
                        while (iD != -1) //-1 is reutned after the last valid ID has been reached       
                        {
                            IRow pRow = pTable.GetRow(iD);
                            hshValidationObjects.Add(
                                hshValidationObjects.Count,
                                (IObject)pRow);
                            iD = enumIDs.Next();
                        }
                    }

                    //2. Get the featureselection of the map 
                    IEnumFeature pEnumFeature = (IEnumFeature)pMap.FeatureSelection;
                    IEnumFeatureSetup pEnumFeatureSetup = (IEnumFeatureSetup)pEnumFeature;
                    pEnumFeatureSetup.AllFields = true;

                    _application.StatusBar.set_Message(0, "Running QA/QC on map selection...");
                    IFeature pFeature = pEnumFeature.Next();
                    while (pFeature != null)
                    {
                        hshValidationObjects.Add(
                            hshValidationObjects.Count, (IObject)pFeature);
                        pFeature = pEnumFeature.Next();
                    }

                    //Make sure user has selected something 
                    if (hshValidationObjects.Count == 0)
                    {
                        MessageBox.Show(
                            "No features or standalone table rows selected in the map!",
                            "PGE Custom QAQC",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        return;
                    }
                    //Get list of FC names, and for those FCs QA/QC errors are displayed as 'ERRORS' and not as 'WARNINGS'. SubPNode requirement. April-16th-2019.
                    PGEError.FeatureClassList_ForceErrors(Editor.EditWorkspace);

                    //Run the validation on the selection 
                    ValidationEngine.Instance.Application = _application;
                    ValidationEngine.Instance.RunQAQCCustomised(
                        hshValidationObjects, ref hshErrorList);
                }

                //Display results in treeview 
                ValidationEngine.Instance.LoadResults(
                    isVersionDiff,
                    tvwResults,
                    hshErrorList);

                //INC3891098 - should switch to the results tab automatically after running 
                //validation 
                if (tvwResults.Nodes.Count > 0)
                {
                    tabQAQC.SelectedTab = pgeResults;
                    btnExpandAll.Enabled = true; //INC000004230713, INC000003945000
                    btnCollapseAll.Enabled = true;
                }
                else
                {
                    btnExpandAll.Enabled = false; //INC000004230713, INC000003945000
                    btnCollapseAll.Enabled = false;
                    MessageBox.Show(
                            "No Validation Results Found!",
                            "PGE Custom QAQC",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                }

            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show("Error running the QAQC: " + ex.Message);
            }
            finally
            {
                //Reset the filters 
                Cursor = Cursors.Default;
                ValidationEngine.Instance.FilterType =
                        ValidationFilterType.valFilterTypeAll;
                ValidationEngine.Instance.SelectionType =
                        ValidationSelectionType.ValidationSelectionTypeVersionDiff;
            }
        }

        /// <summary>
        /// Loads validation rules into the rules treeview  
        /// </summary>
        /// <param name="hshNodes"></param>
        private void LoadRules(Hashtable hshRules)
        {
            try
            {
                //Clear existing nodes 
                tvwRules.Nodes.Clear();

                //Display the rules 
                TreeNode pErrorsNode = new TreeNode("Errors", 7, 7);
                TreeNode pWarningsNode = new TreeNode("Warnings", 7, 7);
                int severity = -1;
                tvwRules.Nodes.Add(pErrorsNode);
                tvwRules.Nodes.Add(pWarningsNode);

                foreach (string key in hshRules.Keys)
                {
                    severity = (int)hshRules[key];
                    if (severity == 1)
                        pWarningsNode.Nodes.Add(new TreeNode(key.ToString(), 6, 6));
                    else
                        pErrorsNode.Nodes.Add(new TreeNode(key.ToString(), 5, 5));
                }

                //Shrink them up by default 
                pErrorsNode.Collapse();
                pWarningsNode.Collapse();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading treeview: " + ex.Message);
            }
        }

        private void tvwRules_AfterCheck(object sender, TreeViewEventArgs e)
        {
            try
            {
                //Exit if we are suppressing 
                if (_suppressEvents)
                    return;

                bool initialCheckedState = e.Node.Checked;
                bool allSiblingNodesHaveSameCheckedState = true;                 

                //For all the child nodes - make sure they have the 
                //same checked state 
                if (e.Node.Nodes != null)
                {
                    _suppressEvents = true; 
                    foreach (TreeNode pNode in e.Node.Nodes)
                    {
                        if (pNode.Checked != initialCheckedState)
                            pNode.Checked = initialCheckedState;
                    }
                    _suppressEvents = false; 
                }

                //Check if all nodes at the current level have 
                //the same checked state if so apply that state 
                //to the parent node                 

                //Loop through all nodes on the same level 
                if (e.Node.Parent != null)
                {
                    foreach (TreeNode pNode in e.Node.Parent.Nodes)
                    {
                        if (pNode.Checked != initialCheckedState)
                        {
                            allSiblingNodesHaveSameCheckedState = false;
                            break;
                        }
                    }

                    if (allSiblingNodesHaveSameCheckedState)
                    {
                        //Give the partent node the same checked state
                        _suppressEvents = true;
                        e.Node.Parent.Checked = initialCheckedState;
                        _suppressEvents = false;
                    }
                    else
                    {
                        _suppressEvents = true;
                        e.Node.Parent.Checked = false;
                        _suppressEvents = false;
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void optRulesIncluded_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (optRulesSelection.Checked)
                    tvwRules.Enabled = true; 
                else
                    tvwRules.Enabled = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                tvwResults.Nodes.Clear();

                //INC000004230713, INC000003945000
                btnExpandAll.Enabled = false;
                btnCollapseAll.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void contexMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                ToolStripItem item = e.ClickedItem;
                TreeNode pTreeNode = tvwResults.SelectedNode;
                string datasetName = "";
                int oid = -1;

                if ((pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_WARNING) ||
                    (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_ERROR))
                {
                    datasetName = ValidationEngine.
                        Instance.StripOutNumberFromNodeText(pTreeNode.Parent.Parent.Text);
                    oid = Convert.ToInt32(ValidationEngine.
                        Instance.StripOutNumberFromNodeText(pTreeNode.Parent.Text));
                }
                else
                {
                    datasetName = ValidationEngine.
                        Instance.StripOutNumberFromNodeText(pTreeNode.Parent.Text);
                    oid = Convert.ToInt32(ValidationEngine.
                        Instance.StripOutNumberFromNodeText(pTreeNode.Text));
                }

                IFeatureClass pFC = null;
                IFeature pFeature = null;

                ITable pTable = null;
                IRow pRecord = null;

                if (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_TBL)      //INC000004230713, INC000003945000 - added condition for table
                {
                    if (_hshTables.ContainsKey(datasetName))
                        pTable = (ITable)_hshTables[datasetName];
                    else
                    {
                        IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS;
                        if (Miner.Geodatabase.Edit.Editor.EditWorkspace != null) { pFWS = (IFeatureWorkspace)Miner.Geodatabase.Edit.Editor.EditWorkspace; }
                        pTable = pFWS.OpenTable(datasetName);
                        _hshTables.Add(datasetName, pTable);
                    }

                    pRecord = pTable.GetRow(oid);
                }
                else
                {
                    if (_hshFCs.ContainsKey(datasetName))
                        pFC = (IFeatureClass)_hshFCs[datasetName];
                    else
                    {
                        IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS;
                        if (Miner.Geodatabase.Edit.Editor.EditWorkspace != null) { pFWS = (IFeatureWorkspace)Miner.Geodatabase.Edit.Editor.EditWorkspace; }
                        pFC = pFWS.OpenFeatureClass(datasetName);
                        _hshFCs.Add(datasetName, pFC);
                    }

                    pFeature = (IFeature)GetFeature(pFC, oid);
                }
                
                if (pFeature != null || pRecord != null)
                {
                    switch (item.Text)
                    {
                        case ValidationEngine.CTXT_MENU_ZOOM_TO:
                            ValidationEngine.Instance.ZoomToGeometry(
                            pFeature.ShapeCopy, _application);
                            break;
                        case ValidationEngine.CTXT_MENU_FLASH:
                            ValidationEngine.Instance.FlashGeometry(
                            pFeature.ShapeCopy, _application);
                            break;
                        case ValidationEngine.CTXT_MENU_ADD_SELECT:
                            ValidationEngine.Instance.AddToSelection(
                            pTreeNode);
                            break;
                        case ValidationEngine.CTXT_MENU_REMOVE_SELECT:
                            ValidationEngine.Instance.RemoveFromSelection(
                            pTreeNode);
                            break;
                        case ValidationEngine.CTXT_MENU_SELECT_ALL_BY_ERROR:
                            ValidationEngine.Instance.SelectObjectsByError(
                            pTreeNode);
                            break;
                        case ValidationEngine.CTXT_MENU_DELETE:    //INC000004230713, INC000003945000
                            ValidationEngine.Instance.DeleteTableRecord(datasetName, oid, tvwResults);
                            break;
                    }   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private IFeature GetFeature(IFeatureClass pFC, int oid)
        {
            try
            {
                IFeature pFeature = pFC.GetFeature(oid);
                return pFeature;
            }
            catch
            {
                return null;
            }
        }

        private void tvwResults_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                bool showMenu = false;
                bool isRow = false;
                bool isOrphaned = false;  //INC000004230713, INC000003945000
                if (e.Button == MouseButtons.Right)
                {
                    // Get the treeview node at the click point
                    System.Drawing.Point p = new System.Drawing.Point(e.X, e.Y);
                    TreeNode pTreeNode = tvwResults.GetNodeAt(p);
                    if (pTreeNode != null)
                    {
                        //Select the node the user has clicked.
                        tvwResults.SelectedNode = pTreeNode;

                        //Create the appropriate ContextMenu depending on the selected node.
                        _treeContextMenu = new System.Windows.Forms.ContextMenuStrip();
                        if ((pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_POINT_FC) ||
                            (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_LINE_FC) ||
                            (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_POLYGON_FC) ||
                            (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_ANNO_FC))
                        {
                            showMenu = true;
                            isRow = true;
                            ToolStripMenuItem zoomToMenuItem = new ToolStripMenuItem();
                            zoomToMenuItem.Text = ValidationEngine.CTXT_MENU_ZOOM_TO;
                            _treeContextMenu.Items.Add(zoomToMenuItem);
                            ToolStripMenuItem flashMenuItem = new ToolStripMenuItem();
                            flashMenuItem.Text = ValidationEngine.CTXT_MENU_FLASH;
                            _treeContextMenu.Items.Add(flashMenuItem);
                        }
                        else if (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_TBL)
                        {
                            isRow = true;
                            showMenu = true;

                            //get orphaned records - INC000004230713, INC000003945000
                            foreach(TreeNode node in pTreeNode.Nodes){
                                if(node.Text.Contains(ORPHAN_TEXT) && node.Text.ToUpper().StartsWith("WARNING")){
                                    isOrphaned = true;
                                    break;
                                }
                            }
                        }
                        else if ((pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_ERROR) ||
                            (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_WARNING))
                        {
                            showMenu = true;
                            ToolStripMenuItem selectAllObjectsWithErrorMenuItem = new ToolStripMenuItem();
                            selectAllObjectsWithErrorMenuItem.Text = ValidationEngine.CTXT_MENU_SELECT_ALL_BY_ERROR;
                            _treeContextMenu.Items.Add(selectAllObjectsWithErrorMenuItem);
                        }

                        //Determine whether select/deselect menu item depending on whether 
                        //row is currently selected in focus map  
                        if (isRow)
                        {
                            if (!ValidationEngine.Instance.IsSelected(pTreeNode))
                            {
                                ToolStripMenuItem addToSelectMenuItem = new ToolStripMenuItem();
                                addToSelectMenuItem.Text = ValidationEngine.CTXT_MENU_ADD_SELECT;
                                _treeContextMenu.Items.Add(addToSelectMenuItem);
                            }
                            else
                            {
                                ToolStripMenuItem removeFromSelectMenuItem = new ToolStripMenuItem();
                                removeFromSelectMenuItem.Text = ValidationEngine.CTXT_MENU_REMOVE_SELECT;
                                _treeContextMenu.Items.Add(removeFromSelectMenuItem);
                            }
                        }

                        //add delete option for orphaned records - INC000004230713, INC000003945000
                        if (isOrphaned)
                        {
                            ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem();
                            deleteMenuItem.Text = ValidationEngine.CTXT_MENU_DELETE;
                            _treeContextMenu.Items.Add(deleteMenuItem);
                        }

                        //Show the context menu
                        if (showMenu)
                        {
                            int x = tvwResults.SelectedNode.Bounds.X;
                            int y = tvwResults.SelectedNode.Bounds.Y +
                                tvwResults.SelectedNode.Bounds.Height;
                            System.Drawing.Point point = new System.Drawing.Point(x, y);
                            System.Drawing.Point absPoint = tvwResults.PointToScreen(point);
                            _treeContextMenu.ItemClicked += new ToolStripItemClickedEventHandler(
                                contexMenu_ItemClicked);
                            _treeContextMenu.Show(absPoint);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void RunQAQCForm_Load(object sender, EventArgs e)
        {
            //check if the current edit version has any posting errors.
            if (Miner.Geodatabase.Edit.Editor.EditWorkspace != null)
            {
                ITable QAQCTable = null;
                IQueryFilter qf = new QueryFilterClass();
                ICursor rowCursor = null;
                IRow row = null;
                try
                {
                    //Load the hash table from the row found in the QAQC session errors table
                    QAQCTable = GetQAQCTable(Miner.Geodatabase.Edit.Editor.EditWorkspace);
                    qf.WhereClause = "SESSIONNAME = '" + ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName + "'";
                    rowCursor = QAQCTable.Search(qf, false);
                    row = rowCursor.NextRow();

                    if (row != null)
                    {
                        btnPostingErrors.Visible = true;
                    }
                    else
                    {
                        btnPostingErrors.Visible = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed checking for errors in posting process: " + ex.Message);
                }
                finally
                {
                    if (QAQCTable != null) { while (Marshal.ReleaseComObject(QAQCTable) > 0) { } }
                    if (qf != null) { while (Marshal.ReleaseComObject(qf) > 0) { } }
                    if (rowCursor != null) { while (Marshal.ReleaseComObject(rowCursor) > 0) { } }
                    if (row != null) { while (Marshal.ReleaseComObject(row) > 0) { } }
                }
            }
            else
            {
                btnPostingErrors.Visible = false;
            }
        }

        //INC000004230713, INC000003945000 - starts
        //Expand all nodes of Warnings and Errors in Results window
        private void btnExpandAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvwResults != null && tvwResults.Nodes.Count > 0)
                {
                    tvwResults.ExpandAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //Collapse all nodes of Warnings and Errors in Results window
        private void btnCollapseAll_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvwResults != null && tvwResults.Nodes.Count > 0)
                {

                    tvwResults.CollapseAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
