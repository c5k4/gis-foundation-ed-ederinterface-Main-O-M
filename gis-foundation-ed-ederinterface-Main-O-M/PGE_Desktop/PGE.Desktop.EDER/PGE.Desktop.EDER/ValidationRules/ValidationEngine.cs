using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using Miner.Interop;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Edit;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.GeoDatabaseDistributed;
using ESRI.ArcGIS.Geometry;
using PGE.Desktop.EDER.ValidationRules.UI;
using PGE.Common.Delivery.Framework.FeederManager;
using Miner.Geodatabase.GeodatabaseManager.Logging;
using System.Xml.Serialization;

namespace PGE.Desktop.EDER.ValidationRules
{
    public enum ValidationFilterType
    {
        valFilterTypeAll = 0,
        valFilterTypeErrorOnly = 1,
        valFilterTypeWarningOnly = 2,
        valFilterTypeSelection = 3
    }

    public enum ValidationSelectionType
    {
        ValidationSelectionTypeVersionDiff = 0,
        ValidationSelectionTypeSelection = 1
    }

    public enum PGEDifferenceType
    {
        pgeDifferenceTypeInsert = 0,
        pgeDifferenceTypeUpdate = 1,
        pgeDifferenceTypeNotApplicable = 2,
        pgeFeederManagerDifference = 3
    }


    /// <summary>
    /// Singleton class for evaluating whether validation rules should fire. We require a means to 
    /// allow the application to run validation selectively. We need to  rules of severity Error to 
    /// be run in isolation. 
    /// 
    /// </summary>
    public class ValidationEngine
    {
        #region Private
        /// <summary>
        /// Logger to log all the information as Debug/Warning/Error.
        /// </summary>
        private readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Public Constants  
        /// </summary>

        //Context menu strings 
        public const string CTXT_MENU_ZOOM_TO = "Zoom To Feature";
        public const string CTXT_MENU_FLASH = "Flash Feature";
        public const string CTXT_MENU_ADD_SELECT = "Add To Selection";
        public const string CTXT_MENU_REMOVE_SELECT = "Remove From Selection";
        public const string CTXT_MENU_SELECT_ALL_BY_ERROR = "Select All Objects With This Error";
        public const string CTXT_MENU_DELETE = "Delete Record";  //INC000004230713, INC000003945000

        //Image index constants 
        public const int IMG_IDX_POINT_FC = 0;
        public const int IMG_IDX_LINE_FC = 1;
        public const int IMG_IDX_POLYGON_FC = 2;
        public const int IMG_IDX_TBL = 3;
        public const int IMG_IDX_ANNO_FC = 4;
        public const int IMG_IDX_ERROR = 5;
        public const int IMG_IDX_WARNING = 6;
        public const int IMG_IDX_FOLDER = 7;
        public const int IMG_IDX_LAYER = 8;
        public const int IMG_IDX_INSERT = 9;
        public const int IMG_IDX_UPDATE = 10;

        
        /// <summary>
        /// Hashtable to store the filtered list of validation errors 
        /// </summary>
        private Hashtable _hshSeverities;

        /// <summary>
        /// Hashtables to store QAQC rules  
        /// </summary>
        private Hashtable _hshAllQAQCRules;
        private Hashtable _hshLoadedQAQCRules;
        private Hashtable _hshSelectedQAQCRules;
        private Hashtable _hshVersionDifferences;
        

        private RunQAQCForm _frmRunQAQC;
        private QAQCErrorForm _frmQAQCErrorForm; 
        

        /// <summary>
        /// Form for displaying the validation Msg  
        /// </summary>
        private QAQCErrorForm _frmValidationMsg;

        /// <summary>
        /// Stores administrative boundary information for better performance of 
        /// the regional attrubutes AU 
        /// </summary>
        private AdminCache _adminCache; 

        /// <summary>
        /// Private instance member.
        /// </summary>
        private static ValidationEngine _instance = null;

        /// <summary>
        /// Private instance member to show what kind of filter to use 
        /// for the validation 
        /// </summary>
        private ValidationFilterType _valFilterType;

        /// <summary>
        /// Private instance member to show what list of features to 
        /// use when running the QAQC 
        /// </summary>
        private ValidationSelectionType _valSelectionType;

        /// <summary>
        /// ArcMap application handle 
        /// </summary>
        private IApplication _App;

        /// <summary>
        /// Private Constants  
        /// </summary>
        /// 
        private const string NODE_TEXT_WARNINGS = "Warnings";
        private const string NODE_TEXT_ERRORS = "Errors";
        private const string NODE_TEXT_INSERTS = "Inserts";
        private const string NODE_TEXT_UPDATES = "Updates";
        private const string NODE_TEXT_EMPTY_BRACKETS = "{}";
        private const string NODE_TEXT_OPEN_BRACKET = "{";
        private const string NODE_TEXT_CLOSE_BRACKET = "}";

        private static IWorkspace _workspace = null;
        public static IWorkspace QAQCWorkspace
        {
            get
            {
                return _workspace;
            }
            set
            {
                _workspace = value;
            }
        }
        /// <summary>
        /// Gets the workspace containing the TELVENT_VALIDATION_SEVERITY_MAP table.
        /// </summary>
        /// <returns></returns>
        public IWorkspace getWorkspace()
        {
            if (QAQCWorkspace == null)
            {
                Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
                object obj = Activator.CreateInstance(type);
                IApplication app = obj as IApplication;

                // get the workspace based on the ArcFM login object
                IMMLogin2 log2 = (IMMLogin2)app.FindExtensionByName("MMPROPERTIESEXT");
                QAQCWorkspace = log2.LoginObject.LoginWorkspace;
            }
            return QAQCWorkspace;
        }

        #endregion Private

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationEngine"/> class.
        /// </summary>
        private ValidationEngine()
        {
            //By default QAQC must validate everything 
            _valFilterType = ValidationFilterType.valFilterTypeAll;
            _valSelectionType = ValidationSelectionType.ValidationSelectionTypeVersionDiff;
            _hshSeverities = LoadSeverities(getWorkspace());
        }
        #endregion Constructor

        #region Public
        #region Property
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ValidationEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ValidationEngine();
                }
                //_logger.Debug("Returning the singleton.");
                return ValidationEngine._instance;
            }
        }

        /// <summary>
        /// Sets what kind of validation filter type is to be applied to QAQC 
        /// </summary>
        public ValidationFilterType FilterType
        {
            get { return _valFilterType; }
            set { _valFilterType = value; }
        }

        /// <summary>
        /// Allows the storing of the VersionDifferences  
        /// </summary>
        public Hashtable VersionDifferences 
        {
            get { return _hshVersionDifferences; }
            set { _hshVersionDifferences = value; }
        }

        /// <summary>
        /// Sets what kind of validation selection type is to be applied to QAQC 
        /// </summary>
        public ValidationSelectionType SelectionType
        {
            get { return _valSelectionType; }
            set { _valSelectionType = value; }
        }

        /// <summary>
        /// Stores a list of all val rules with their severity  
        /// </summary>
        public Hashtable Severities
        {
            get
            {
                if (_hshSeverities == null)
                    _hshSeverities = LoadSeverities(getWorkspace());
                return _hshSeverities;
            }
        }

        /// <summary>
        /// Stores a list of admin attributes so that the regional attributes 
        /// don't have to be re-calculated for each spatial edit to each feature 
        /// </summary>
        public AdminCache AdminCache
        {
            get { return _adminCache; }
            set { _adminCache = value; }
        }

        public Hashtable SelectedQAQCRules
        {
            get { return _hshSelectedQAQCRules; }
            set { _hshSelectedQAQCRules = value; }
        }

        /// <summary>
        /// Sets what kind of validation selection type is to be applied to QAQC 
        /// </summary>
        public IApplication Application
        {
            get { return _App; }
            set { _App = value; }
        }

        #endregion Property

        private Hashtable LoadSeverities(IWorkspace workSpace)
        {
            Hashtable hshSeverities = new Hashtable();
            IObjectClass validationMapClass = ModelNameFacade.ObjectClassByModelName(workSpace, "VALIDATION_SEVERITY");
            ICursor rowCursor = null;
            IRow row = null;
            try
            {
                if (validationMapClass != null)
                {
                    int severityFieldIdx = validationMapClass.Fields.FindField("SEVERITY");
                    int nameFieldIdx = validationMapClass.Fields.FindField("NAME");
                    if (severityFieldIdx == -1 || nameFieldIdx == -1)
                    {
                        throw new Exception("Error loading Severities");
                    }
                    rowCursor = ((ITable)validationMapClass).Search(null, false);
                    string name = string.Empty;
                    int severity = int.MinValue;
                    while ((row = rowCursor.NextRow()) != null)
                    {
                        name = TypeCastFacade.Cast<string>(row.get_Value(nameFieldIdx), string.Empty);
                        severity = TypeCastFacade.Cast<int>(row.get_Value(severityFieldIdx), 8);
                        if (string.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        if (!hshSeverities.ContainsKey(name))
                            hshSeverities.Add(name, severity);
                    }
                }
                else
                {
                    throw new Exception("Error loading Severities - validation severity model name not found");
                }
            }
            finally
            {
                if (rowCursor != null)
                {
                    while (Marshal.ReleaseComObject(rowCursor) > 0) { }
                }
                if (row != null)
                {
                    while (Marshal.ReleaseComObject(row) > 0) { }
                }
            }
            return hshSeverities;
        }

        void QAQC_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (sender is QAQCErrorForm)
                _frmQAQCErrorForm = null;
            else if (sender is RunQAQCForm)
                _frmRunQAQC = null;
        }

        /// <summary>
        /// Displays the PGE Custom QAQC Form 
        /// </summary>
        /// <param name="name">The name.</param>
        /// 
        public void OpenRunQAQCForm()
        {
            try
            {
                //Bring up the form 
                if (_frmRunQAQC == null)
                {
                    _frmRunQAQC = new RunQAQCForm(
                                            _App,
                                            getWorkspace());
                    _frmRunQAQC.FormClosed += new FormClosedEventHandler(QAQC_FormClosed);
                    _frmRunQAQC.Show(new ModelessDialog(_App.hWnd));
                }
                else
                {
                    if (_frmRunQAQC.WindowState == FormWindowState.Minimized)
                        _frmRunQAQC.WindowState = FormWindowState.Normal;
                    _frmRunQAQC.Focus();
                }
            }
            catch
            {
                //Ignore 
            }
        }



        /// <summary>
        /// Displays a form to the user showing QAQC results with 
        /// items of severity: error only (no warnings) 
        /// </summary>
        /// <param name="name">The name.</param>
        public void OpenQAQCErrorForm(Hashtable hshFilteredErrors)
        {
            try
            {
                //Bring up the form 
                if (_frmQAQCErrorForm == null)
                {
                    _frmQAQCErrorForm = new QAQCErrorForm(
                                            hshFilteredErrors,
                                            _App,
                                            getWorkspace());
                    _frmQAQCErrorForm.FormClosed += new FormClosedEventHandler(QAQC_FormClosed);
                    _frmQAQCErrorForm.Show(new ModelessDialog(_App.hWnd));
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("entering error handler for DisplayValidationMsg: " + ex.Message);
                //assume the error should be run (to be safe) 
                throw new Exception("Error returning the QAQC error count");
            }
        }

        public bool IsQAQCErrorOpen()
        {
            try
            {
                //Bring up the form 
                if (_frmQAQCErrorForm == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                _logger.Debug("entering error handler for IsQAQCErrorOpen: " + ex.Message);
                return true;
            }
        }
        
        /// <summary>
        /// Loads validation results into the passed treeview
        /// </summary>
        /// <param name="hshNodes"></param>
        /// 
        public void LoadResults(
            bool isVersionDiffMode,
            TreeView tvwResults,
            Hashtable hshNodes)
        {
            try
            {
                //Add the nodes in the treeview  
                tvwResults.Nodes.Clear();
                tvwResults.CheckBoxes = false;

                TreeView tempTreeView = new TreeView();

                foreach (int key in hshNodes.Keys)
                {
                    PGEError pPGEError = (PGEError)hshNodes[key];

                    //Reset all variables 
                    int errImgIdx = -1;
                    int diffTypeImgIdx = -1;
                    string errCategory = "";
                    string diffType = "";
                    int topNodeIdx = -1;
                    int errNodeIdx = -1;
                    int featureNodeIdx = -1;
                    int datasetNodeIdx = -1;
                    int diffTypeNodeIdx = -1;
                    TreeNode pTopLevelNode = null;
                    TreeNode pDiffTypeNode = null;
                    TreeNode pDatasetNode = null;
                    TreeNode pFeatureNode = null;
                    TreeNode pTreeNode = null;

                    if (pPGEError.Severity == 1)
                    {
                        errCategory = NODE_TEXT_WARNINGS;
                        errImgIdx = IMG_IDX_WARNING;
                    }
                    else
                    {
                        errCategory = NODE_TEXT_ERRORS;
                        errImgIdx = IMG_IDX_ERROR;
                    }

                    //Look for a Error/Warnings node 
                    topNodeIdx = GetTreeNodeIndex(tempTreeView, null, errCategory);
                    if (topNodeIdx == -1)
                    {
                        //Add the top level tree node for Errors/Warnings  
                        pTreeNode = new TreeNode(
                            errCategory + NODE_TEXT_EMPTY_BRACKETS,
                            IMG_IDX_FOLDER,
                            IMG_IDX_FOLDER);
                        topNodeIdx = tempTreeView.Nodes.Add(pTreeNode);
                    }

                    //Look for a Diff Type level node and add it if necessary 
                    if (isVersionDiffMode)
                    {
                        if (pPGEError.DifferenceType == PGEDifferenceType.pgeDifferenceTypeInsert)
                        {
                            diffType = NODE_TEXT_INSERTS;
                            diffTypeImgIdx = IMG_IDX_INSERT;
                        }
                        else if (pPGEError.DifferenceType == PGEDifferenceType.pgeDifferenceTypeUpdate)
                        {
                            diffType = NODE_TEXT_UPDATES;
                            diffTypeImgIdx = IMG_IDX_UPDATE;
                        }
                        else if (pPGEError.DifferenceType == PGEDifferenceType.pgeFeederManagerDifference)
                        {
                            diffType = "Circuit Updates";
                            diffTypeImgIdx = IMG_IDX_UPDATE;
                        }
                        pTopLevelNode = tempTreeView.Nodes[topNodeIdx];
                        diffTypeNodeIdx = GetTreeNodeIndex(tempTreeView, pTopLevelNode, diffType);
                        if (diffTypeNodeIdx == -1)
                        {
                            //Add the diff type node 
                            pTreeNode = new TreeNode(
                                diffType + NODE_TEXT_EMPTY_BRACKETS,
                                diffTypeImgIdx,
                                diffTypeImgIdx);
                            diffTypeNodeIdx = pTopLevelNode.Nodes.Add(pTreeNode);
                        }
                    }

                    //Look for a Dataset level node and add it if necessary 
                    if (isVersionDiffMode)
                    {
                        pDiffTypeNode = pTopLevelNode.Nodes[diffTypeNodeIdx];
                        datasetNodeIdx = GetTreeNodeIndex(tempTreeView, pDiffTypeNode, pPGEError.Dataset);
                    }
                    else
                    {
                        pTopLevelNode = tempTreeView.Nodes[topNodeIdx];
                        datasetNodeIdx = GetTreeNodeIndex(tempTreeView, pTopLevelNode, pPGEError.Dataset);
                    }
                    if (datasetNodeIdx == -1)
                    {
                        //Add the dataset level node  
                        pTreeNode = new TreeNode(
                            pPGEError.Dataset + NODE_TEXT_EMPTY_BRACKETS,
                            IMG_IDX_LAYER,
                            IMG_IDX_LAYER);
                        if (isVersionDiffMode)
                            datasetNodeIdx = pDiffTypeNode.Nodes.Add(pTreeNode);
                        else
                            datasetNodeIdx = pTopLevelNode.Nodes.Add(pTreeNode);
                    }

                    //Look for a feature level node and add it if necessary 
                    if (isVersionDiffMode)
                        pDatasetNode = pDiffTypeNode.Nodes[datasetNodeIdx];
                    else
                        pDatasetNode = pTopLevelNode.Nodes[datasetNodeIdx];
                    featureNodeIdx = GetTreeNodeIndex(tempTreeView, pDatasetNode, pPGEError.ObjectId.ToString());
                    if (featureNodeIdx == -1)
                    {
                        //Add the dataset level node  
                        pTreeNode = new TreeNode(
                            pPGEError.ObjectId.ToString() + NODE_TEXT_EMPTY_BRACKETS,
                        pPGEError.Type,
                        pPGEError.Type);
                        featureNodeIdx = pDatasetNode.Nodes.Add(pTreeNode);
                    }

                    //Add the error node 
                    pFeatureNode = pDatasetNode.Nodes[featureNodeIdx];
                    pTreeNode = new TreeNode(pPGEError.ErrorMsg, errImgIdx, errImgIdx);
                    pTreeNode.Tag = pPGEError.RuleName;
                    errNodeIdx = pFeatureNode.Nodes.Add(pTreeNode);
                }


                GetNodeCollectionCounts(tempTreeView.Nodes);
                tempTreeView.Sort();
                foreach (TreeNode node in tempTreeView.Nodes)
                {
                    tvwResults.Nodes.Add((TreeNode)node.Clone());
                }

                /*
                //Now loop through setting all of the counts inside the brackets {} 
                //1. Get all of the feature level treeviw nodes, the counts at this 
                //   level will be simply the number of child nodes 
                //   First get all of the feature / row level (bottom tier) nodes  
                Hashtable hshImageIndexes = new Hashtable();
                Hashtable hshFeatureLevelNodes = new Hashtable();
                hshImageIndexes.Add(IMG_IDX_POINT_FC, hshImageIndexes.Count);
                hshImageIndexes.Add(IMG_IDX_LINE_FC, hshImageIndexes.Count);
                hshImageIndexes.Add(IMG_IDX_POLYGON_FC, hshImageIndexes.Count);
                hshImageIndexes.Add(IMG_IDX_ANNO_FC, hshImageIndexes.Count);
                hshImageIndexes.Add(IMG_IDX_TBL, hshImageIndexes.Count);
                GetNodesByImageIndex(tvwResults, hshImageIndexes, ref hshFeatureLevelNodes);
                foreach (TreeNode pNode in hshFeatureLevelNodes.Values)
                {
                    //Replace the empty brackets in node text with updated count 
                    pNode.Text = pNode.Text.Replace(
                        NODE_TEXT_EMPTY_BRACKETS,
                        NODE_TEXT_OPEN_BRACKET + pNode.Nodes.Count.ToString() + NODE_TEXT_CLOSE_BRACKET);
                }

                //2. Get all of the layer level treeviw nodes, and count up all 
                //   of the counts below this level 
                hshImageIndexes.Clear();
                Hashtable hshDatasetLevelNodes = new Hashtable();
                hshImageIndexes.Add(IMG_IDX_LAYER, hshImageIndexes.Count);
                GetNodesByImageIndex(tvwResults, hshImageIndexes, ref hshDatasetLevelNodes);
                int errorCount = 0;
                int errorTotal = 0;
                foreach (TreeNode pNode in hshDatasetLevelNodes.Values)
                {
                    errorTotal = 0;
                    foreach (TreeNode pSubNode in pNode.Nodes)
                    {
                        errorCount = StripNumberFromNodeText(pSubNode.Text);
                        errorTotal += errorCount;
                    }
                    //Replace the empty brackets in node text with updated count 
                    pNode.Text = pNode.Text.Replace(
                        NODE_TEXT_EMPTY_BRACKETS,
                        NODE_TEXT_OPEN_BRACKET + errorTotal.ToString() + NODE_TEXT_CLOSE_BRACKET);
                }

                //3. Get all of the diff type level treeviw nodes, and count up all 
                //   of the counts below this level
                if (isVersionDiffMode)
                {
                    hshImageIndexes.Clear();
                    Hashtable hshDiffTypeLevelNodes = new Hashtable();
                    hshImageIndexes.Add(IMG_IDX_INSERT, hshImageIndexes.Count);
                    hshImageIndexes.Add(IMG_IDX_UPDATE, hshImageIndexes.Count);
                    GetNodesByImageIndex(tvwResults, hshImageIndexes, ref hshDiffTypeLevelNodes);
                    errorCount = 0;
                    foreach (TreeNode pNode in hshDiffTypeLevelNodes.Values)
                    {
                        errorTotal = 0;
                        foreach (TreeNode pSubNode in pNode.Nodes)
                        {
                            errorCount = StripNumberFromNodeText(pSubNode.Text);
                            errorTotal += errorCount;
                        }
                        //Replace the empty brackets in node text with updated count 
                        pNode.Text = pNode.Text.Replace(
                            NODE_TEXT_EMPTY_BRACKETS,
                            NODE_TEXT_OPEN_BRACKET + errorTotal.ToString() + NODE_TEXT_CLOSE_BRACKET);
                    }
                }

                //4. Get all of the errors/warnings level treeviw nodes, and count up all 
                //   of the counts below this level
                hshImageIndexes.Clear();
                Hashtable hshErrWarnLevelNodes = new Hashtable();
                hshImageIndexes.Add(IMG_IDX_FOLDER, hshImageIndexes.Count);
                GetNodesByImageIndex(tvwResults, hshImageIndexes, ref hshErrWarnLevelNodes);
                errorCount = 0;
                foreach (TreeNode pNode in hshErrWarnLevelNodes.Values)
                {
                    errorTotal = 0;
                    foreach (TreeNode pSubNode in pNode.Nodes)
                    {
                        errorCount = StripNumberFromNodeText(pSubNode.Text);
                        errorTotal += errorCount;
                    }
                    //Replace the empty brackets in node text with updated count 
                    pNode.Text = pNode.Text.Replace(
                        NODE_TEXT_EMPTY_BRACKETS,
                        NODE_TEXT_OPEN_BRACKET + errorTotal.ToString() + NODE_TEXT_CLOSE_BRACKET);
                }
                */
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading treeview: " + ex.Message);
            }
        }

        /// <summary>
        /// This method updates the text property of all treenodes with the number of children nodes it contains.
        /// </summary>
        /// <param name="node">TreeNodeCollection to process</param>
        /// <returns></returns>
        private void GetNodeCollectionCounts(TreeNodeCollection Nodes)
        {
                foreach (TreeNode childNode in Nodes)
                {
                    int childCount = GetNodeCounts(childNode);

                    childNode.Text = childNode.Text.Replace(NODE_TEXT_EMPTY_BRACKETS,
                        NODE_TEXT_OPEN_BRACKET + childCount + NODE_TEXT_CLOSE_BRACKET);
                }
        }

        /// <summary>
        /// This method updates the text property of all treenodes with the number of children nodes it contains.
        /// </summary>
        /// <param name="node">Treenode to process</param>
        /// <returns></returns>
        private int GetNodeCounts(TreeNode node)
        {
            int childCount = 0;
            if (node.Nodes.Count > 0)
            {
                foreach (TreeNode childNode in node.Nodes)
                {
                    childCount += GetNodeCounts(childNode);
                }
            }

            if (childCount > 0)
            {
                node.Text = node.Text.Replace(NODE_TEXT_EMPTY_BRACKETS,
                            NODE_TEXT_OPEN_BRACKET + childCount + NODE_TEXT_CLOSE_BRACKET);
                return childCount;
            }
            else
            {
                //There were no children, so this is an error node.  Simply return 1 representing itself
                return 1;
            }
        }

        /// <summary>
        /// Simply strips the error count from the nodetext e.g.  
        /// "Inserts{7}" will return 7   
        /// </summary>
        /// <param name="nodeText"></param>
        /// <returns></returns>
        private int StripNumberFromNodeText(string nodeText)
        {
            try
            {
                int startIdx = nodeText.IndexOf(NODE_TEXT_OPEN_BRACKET);
                int finishIdx = nodeText.IndexOf(NODE_TEXT_CLOSE_BRACKET);
                string countString = nodeText.Substring(startIdx + 1, (finishIdx - (startIdx + 1)));
                int errorCount = Convert.ToInt32(countString);
                return errorCount;
            }
            catch
            {
                throw new Exception("Error stripping error count from node text");
            }
        }

        /// <summary>
        /// Simply strips out the error count from the nodetext e.g.  
        /// "Inserts{7}" will return "Inserts" 
        /// </summary>
        /// <param name="nodeText"></param>
        /// <returns></returns>
        public string StripOutNumberFromNodeText(string nodeText)
        {
            try
            {
                string nodeTextStripped = "";
                int posOfLeftBracket = nodeText.IndexOf(NODE_TEXT_OPEN_BRACKET);
                if (posOfLeftBracket != -1)
                    nodeTextStripped = nodeText.Substring(0, posOfLeftBracket);
                else
                    nodeTextStripped = nodeText;

                return nodeTextStripped;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns a hashtable of treeview nodes that have a particular 
        /// image index 
        /// </summary>
        /// <param name="treeView"></param>
        /// <param name="hshIndexes"></param>
        /// <param name="hshNodes"></param>
        private void GetNodesByImageIndex(TreeView treeView, Hashtable hshIndexes, ref Hashtable hshNodes)
        {
            // Print each node recursively.
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                //Add the node if it has a matching index  
                if (hshIndexes.ContainsKey(n.ImageIndex))
                    hshNodes.Add(hshNodes.Count, n);
                if (n.Nodes.Count > 0)
                    GetNodes(n, hshIndexes, ref hshNodes);
            }
        }
        /// <summary>
        /// Returns a hashtable of treeview nodes that have a particular 
        /// image index (recursive call) 
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="hshIndexes"></param>
        /// <param name="hshNodes"></param>
        private void GetNodes(TreeNode treeNode, Hashtable hshIndexes, ref Hashtable hshNodes)
        {
            // Print the node.
            System.Diagnostics.Debug.WriteLine(treeNode.Text);
            foreach (TreeNode tn in treeNode.Nodes)
            {
                //Add the node if it has a matching index 
                if (hshIndexes.ContainsKey(tn.ImageIndex))
                    hshNodes.Add(hshNodes.Count, tn);
                if (tn.Nodes.Count > 0)
                    GetNodes(tn, hshIndexes, ref hshNodes);
            }
        }

        /// <summary>
        /// Returns a treeview node index with the passed nodeText 
        /// </summary>
        /// <param name="tvwTarget"></param>
        /// <param name="pParentNode"></param>
        /// <param name="nodeText"></param>
        /// <returns>treeNode index</returns>
        private int GetTreeNodeIndex(TreeView tvwTarget, TreeNode pParentNode, string nodeText)
        {
            try
            {
                int nodeIdx = -1;
                if (pParentNode == null)
                {
                    foreach (TreeNode pNode in tvwTarget.Nodes)
                    {
                        if (StripOutNumberFromNodeText(pNode.Text) == nodeText)
                        {
                            nodeIdx = pNode.Index;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (TreeNode pNode in pParentNode.Nodes)
                    {
                        if (StripOutNumberFromNodeText(pNode.Text) == nodeText)
                        {
                            nodeIdx = pNode.Index;
                            break;
                        }
                    }
                }
                return nodeIdx;
            }
            catch
            {
                throw new Exception("Error returning treeview node");
            }
        } 

        public string StripOutCount(string nodeText)
        {
            try
            {
                string nodeTextStripped = ""; 
                int posOfLeftBracket = nodeText.IndexOf("{");
                if (posOfLeftBracket != -1)
                    nodeTextStripped = nodeText.Substring(0, posOfLeftBracket); 
                else
                    nodeTextStripped = nodeText;

                return nodeTextStripped; 
            }
            catch(Exception ex) 
            {
                throw ex;                 
            }
        }

        public void ZoomToGeometry(IGeometry geometry, IApplication pApp)
        {
            try
            {
                IMxDocument doc = (IMxDocument)pApp.Document;
                IMap map = (IMap)doc.FocusMap;
                IActiveView pActiveView = (IActiveView)map;

                IEnvelope pEnvelope;
                if (geometry is IPoint)
                {
                    IEnvelope currentEnv = pActiveView.Extent;
                    IPoint point = (IPoint)geometry;
                    currentEnv.CenterAt(point);
                    pActiveView.Extent = currentEnv;
                    map.MapScale = 1 / 100; //set to 1:100 scale          
                }
                else
                {
                    pEnvelope = geometry.Envelope;
                    pEnvelope.Expand(1.2, 1.2, true);
                    pActiveView.Extent = pEnvelope;
                }
                pActiveView.Refresh();
            }
            catch
            {
                //Ignore 
            }
        }

        public bool FlashGeometry(
                        IGeometry pGeo, 
                        IApplication pApp)
        {

            try
            {

                IMxDocument pMxDoc = (IMxDocument)pApp.Document;
                IMap pMap = pMxDoc.FocusMap;
                IActiveView pActiveView = (IActiveView)pMap; 

                IRgbColor pColor = new RgbColorClass();
                pColor.Red = 0;
                pColor.Green = 255;
                pColor.Blue = 0;
                IScreenDisplay pScreenDisplay = (IScreenDisplay)pActiveView.ScreenDisplay;
                int pSize = 12;
                int pInterval = 250; 

                short pScrCache;
                ILineSymbol pSimpleLineSymbol;
                ISimpleFillSymbol pSimpleFillSymbol;
                ISimpleMarkerSymbol pSimpleMarkersymbol;
                ISymbol pSymbol;

                pScrCache = (short)esriScreenCache.esriNoScreenCache;
                pScreenDisplay.StartDrawing(0, pScrCache);

                switch (pGeo.GeometryType)
                {
                    case esriGeometryType.esriGeometryPolyline:
                        pSimpleLineSymbol = new SimpleLineSymbol();
                        pSymbol = (ISymbol)pSimpleLineSymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                        pSimpleLineSymbol.Width = pSize;
                        pSimpleLineSymbol.Color = pColor;
                        pScreenDisplay.SetSymbol(pSymbol);
                        pScreenDisplay.DrawPolyline(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolyline(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolyline(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolyline(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolyline(pGeo);
                        System.Threading.Thread.Sleep(pInterval); 
                        break;

                    case esriGeometryType.esriGeometryPolygon:
                        pSimpleFillSymbol = new SimpleFillSymbol();
                        pSymbol = (ISymbol)pSimpleFillSymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                        pSimpleFillSymbol.Color = pColor;
                        pScreenDisplay.SetSymbol(pSymbol);
                        pScreenDisplay.DrawPolygon(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolygon(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolygon(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolygon(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPolygon(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        break;

                    case esriGeometryType.esriGeometryPoint:
                        pSimpleMarkersymbol = new SimpleMarkerSymbol();
                        pSymbol = (ISymbol)pSimpleMarkersymbol;
                        pSymbol.ROP2 = esriRasterOpCode.esriROPNotXOrPen;
                        pSimpleMarkersymbol.Color = pColor;
                        pSimpleMarkersymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                        pSimpleMarkersymbol.Size = pSize;
                        pScreenDisplay.SetSymbol(pSymbol);
                        pScreenDisplay.DrawPoint(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPoint(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPoint(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPoint(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        pScreenDisplay.DrawPoint(pGeo);
                        System.Threading.Thread.Sleep(pInterval);
                        break;

                    case esriGeometryType.esriGeometryMultipoint:
                        break;
                    default:
                        break;
                }

                pScreenDisplay.FinishDrawing();
                PartialRefresh(pActiveView);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in FlashGeometry function. " + ex.Message);

            }
        }

        private bool PartialRefresh(IActiveView pActiveView)
        {
            try
            {
                pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, pActiveView.Extent);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in PartialRefresh function. " + ex.Message);
            }
        }


        public void SelectObjectsByError(TreeNode pNode)
        {
            try
            {
                //First clear the map selection 
                IMxDocument pMxDoc = (IMxDocument)_App.Document;
                IMap pMap = pMxDoc.FocusMap;
                pMap.ClearSelection();

                //The treenode tag will have the rule name so 
                //select all features that have this rule name
                Hashtable hshNodesWithSameIdx = new Hashtable();
                Hashtable hshImagesIndexes = new Hashtable();
                Hashtable hshNodesWithError = new Hashtable();
                hshImagesIndexes.Add(pNode.ImageIndex, 0);
                GetNodesByImageIndex(pNode.TreeView, hshImagesIndexes, ref hshNodesWithSameIdx);
                foreach (TreeNode pTreeNode in hshNodesWithSameIdx.Values)
                {
                    //If they have the same tag - this is the same rule 
                    
                    if (pTreeNode.Tag.ToString().Trim().ToUpper() == pNode.Tag.ToString().Trim().ToUpper())
                    {
                        hshNodesWithError.Add(hshNodesWithError.Count + 1, pTreeNode);
                    }
                }

                //Add them all to a hashtable keyed by the 
                //datasetname with a hashtable of OIds 
                Hashtable hshObjects = new Hashtable();
                Hashtable hshOIds;
                string datasetName = "";
                Int32 oid = -1;
                foreach (TreeNode pTreeNode in hshNodesWithError.Values)
                {
                    datasetName = StripOutNumberFromNodeText(pTreeNode.Parent.Parent.Text.ToLower());
                    oid = Convert.ToInt32(StripOutNumberFromNodeText(pTreeNode.Parent.Text));
                    if (!hshObjects.ContainsKey(datasetName))
                        hshObjects.Add(datasetName, new Hashtable());
                    hshOIds = (Hashtable)hshObjects[datasetName];
                    if (!hshOIds.ContainsKey(oid))
                    {
                        hshOIds.Add(oid, 0);
                        hshObjects[datasetName] = hshOIds;
                    }
                }

                //Add all the objects to the selection 
                foreach (string fcName in hshObjects.Keys)
                {
                    AddToSelection(fcName, (Hashtable)hshObjects[fcName]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in SelectObjectsByError function. " + ex.Message);

            }
        }

        public void AddToSelection(TreeNode pNode)
        {
            try
            {
                //Get the focus map 
                bool redrawSelectionPhase = false;
                bool selectionChanged = false;
                IFeatureLayer pFL = null;
                IDataset pDS = null;
                IMxDocument pMxDoc = (IMxDocument)_App.Document;
                IMap pMap = pMxDoc.FocusMap;
                ISelectionEvents pSelectionEvents;
                pSelectionEvents = pMap as ISelectionEvents;
                IEnvelope bounds = pMxDoc.ActiveView.ScreenDisplay.
                    DisplayTransformation.FittedBounds;
                int b4SelCount = -1;
                int afterSelCount = -1;
                int oid;

                if ((pNode.ImageIndex == IMG_IDX_POINT_FC) ||
                    (pNode.ImageIndex == IMG_IDX_LINE_FC) ||
                    (pNode.ImageIndex == IMG_IDX_POLYGON_FC) ||
                    (pNode.ImageIndex == IMG_IDX_ANNO_FC))
                {
                    IEnumLayer enumLayer = pMap.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
                    enumLayer.Reset();
                    ILayer layer;
                    oid = Convert.ToInt32(StripOutNumberFromNodeText(pNode.Text));
                    IQueryFilter pQueryFilter = new QueryFilterClass();
                    //loop through map feature layers  
                    while ((layer = enumLayer.Next()) != null)
                    {
                        //Validate the layer and check layer name
                        if (layer is IFeatureLayer)
                        {
                            pFL = (IFeatureLayer)layer;
                            pDS = (IDataset)pFL.FeatureClass;
                            if ((layer.Valid) &&
                                (pDS.Name.ToLower() == StripOutNumberFromNodeText(pNode.Parent.Text).ToLower() &&
                                (pFL.Selectable)))
                            {
                                IFeatureSelection pFSel = (IFeatureSelection)pFL;
                                b4SelCount = pFSel.SelectionSet.Count;
                                //April 2019 release - Fix for Green halo ME item# 39 - to avoid issue on PGEReplaceAsset as Switch and DPD layers are configured into two (Opened and Closed) sublayers
                                pQueryFilter.WhereClause = "OBJECTID = " + oid;
                                IFeatureCursor pCursor = pFL.Search(pQueryFilter, false);
                                IFeature pFeature = pCursor.NextFeature();
                                if (pFeature != null) pFSel.Add(pFeature);
                                //pFSel.Add(pFL.FeatureClass.GetFeature(oid));
                                afterSelCount = pFSel.SelectionSet.Count;

                                //If selection has increased we can exit out of this loop 
                                if (afterSelCount > b4SelCount)
                                {
                                    redrawSelectionPhase = true;
                                    selectionChanged = true;
                                    break;
                                }
                                Marshal.ReleaseComObject(pCursor);
                            }
                        }
                    }
                }
                else if (pNode.ImageIndex == IMG_IDX_TBL)
                {
                    IStandaloneTableCollection pStandAloneTableCollection =
                        (IStandaloneTableCollection)pMap;
                    IStandaloneTable pStandaloneTable = null;
                    ITable pTable = null;
                    oid = Convert.ToInt32(StripOutNumberFromNodeText(pNode.Text));

                    //loop through map standalone tables  
                    for (int i = 0; i <= pStandAloneTableCollection.StandaloneTableCount - 1; i++)
                    {
                        pStandaloneTable = pStandAloneTableCollection.get_StandaloneTable(i);
                        pDS = (IDataset)pStandaloneTable;
                        pTable = (ITable)pStandaloneTable;
                        if (pTable == null)
                        {
                            continue;
                        }

                        if ((pStandaloneTable.Valid) &&
                                (pDS.Name.ToLower() == pNode.Parent.Text.ToLower()))
                        {
                            //get selection set from table
                            ITableSelection pTableSel = (ITableSelection)pStandaloneTable;
                            b4SelCount = pTableSel.SelectionSet.Count;
                            pTableSel.AddRow(pTable.GetRow(oid));
                            afterSelCount = pTableSel.SelectionSet.Count;

                            //If selection has increased we can exit out of this loop 
                            if (afterSelCount > b4SelCount)
                            {
                                selectionChanged = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //Do nothing 
                    return;
                }

                //Notify of select change and refresh selection phase ONLY  
                if (selectionChanged)
                    pSelectionEvents.SelectionChanged();
                if (redrawSelectionPhase)
                    pMxDoc.ActiveView.PartialRefresh(
                        esriViewDrawPhase.esriViewGeoSelection,
                        null,
                        bounds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in AddToSelection function. " + ex.Message);
            }
        }

        public void AddToSelection(string datasetName, Hashtable hshOIds)
        {
            try
            {
                //Get the focus map 
                bool redrawSelectionPhase = false;
                bool selectionChanged = false;
                bool isFC = false;
                IFeatureLayer pFL = null;
                IDataset pDS = null;
                IMxDocument pMxDoc = (IMxDocument)_App.Document;
                IMap pMap = pMxDoc.FocusMap;
                ISelectionEvents pSelectionEvents;
                pSelectionEvents = pMap as ISelectionEvents;
                IEnvelope bounds = pMxDoc.ActiveView.ScreenDisplay.
                    DisplayTransformation.FittedBounds;
                int b4SelCount = -1;
                int afterSelCount = -1;
                int selCount = 0;

                IEnumLayer enumLayer = pMap.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
                enumLayer.Reset();
                ILayer layer;
                IQueryFilter pQueryFilter = new QueryFilterClass();
                //loop through map feature layers  
                while ((layer = enumLayer.Next()) != null)
                {
                    //Validate the layer and check layer name
                    if (layer is IFeatureLayer)
                    {
                        pFL = (IFeatureLayer)layer;
                        pDS = (IDataset)pFL.FeatureClass;
                        if ((layer.Valid) &&
                            (pDS.Name.ToLower() == datasetName &&
                            (pFL.Selectable)))
                        {
                            isFC = true;
                            IFeatureSelection pFSel = (IFeatureSelection)pFL;
                            foreach (int oid in hshOIds.Keys)
                            {
                                b4SelCount = pFSel.SelectionSet.Count;
                                //April 2019 release - Fix for Green halo ME item# 39 - to avoid issue on PGEReplaceAsset as Switch and DPD layers are configured into two (Opened and Closed) sublayers
                                pQueryFilter.WhereClause = "OBJECTID = " + oid;
                                IFeatureCursor pCursor = pFL.Search(pQueryFilter, false);
                                IFeature pFeature = pCursor.NextFeature();
                                if (pFeature != null) pFSel.Add(pFeature);
                                //pFSel.Add(pFL.FeatureClass.GetFeature(oid));
                                afterSelCount = pFSel.SelectionSet.Count;

                                //If selection has increased we can exit out of this loop 
                                if (afterSelCount > b4SelCount)
                                {
                                    selCount++;
                                    redrawSelectionPhase = true;
                                    selectionChanged = true;
                                }
                            }
                            //Can exit out if we have all of them 
                            if (selCount == hshOIds.Count)
                                break;
                        }
                    }
                }

                if (!isFC)
                {
                    IStandaloneTableCollection pStandAloneTableCollection =
                        (IStandaloneTableCollection)pMap;
                    IStandaloneTable pStandaloneTable = null;
                    ITable pTable = null;

                    //loop through map standalone tables  
                    for (int i = 0; i <= pStandAloneTableCollection.StandaloneTableCount - 1; i++)
                    {
                        pStandaloneTable = pStandAloneTableCollection.get_StandaloneTable(i);
                        pDS = (IDataset)pStandaloneTable;
                        pTable = (ITable)pStandaloneTable;
                        if (pTable == null)
                        {
                            continue;
                        }

                        if ((pStandaloneTable.Valid) &&
                                (pDS.Name.ToLower() == datasetName))
                        {
                            //get selection set from table
                            ITableSelection pTableSel = (ITableSelection)pStandaloneTable;

                            foreach (int oid in hshOIds.Keys)
                            {
                                b4SelCount = pTableSel.SelectionSet.Count;
                                pTableSel.AddRow(pTable.GetRow(oid));
                                afterSelCount = pTableSel.SelectionSet.Count;

                                //If selection has increased we can exit out of this loop 
                                if (afterSelCount > b4SelCount)
                                {
                                    selCount++;
                                    selectionChanged = true;
                                }
                            }

                            //Can exit out if we have all of them 
                            if (selCount == hshOIds.Count)
                                break;
                        }
                    }
                }

                //Notify of select change and refresh selection phase ONLY  
                if (selectionChanged)
                    pSelectionEvents.SelectionChanged();
                if (redrawSelectionPhase)
                    pMxDoc.ActiveView.PartialRefresh(
                        esriViewDrawPhase.esriViewGeoSelection,
                        null,
                        bounds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in AddToSelection function. " + ex.Message);
            }
        }

        public void RemoveFromSelection(TreeNode pNode)
        {
            try
            {
                //Get the focus map 
                bool redrawSelectionPhase = false;
                bool selectionChanged = false;
                IFeatureLayer pFL = null;
                IDataset pDS = null;
                IMxDocument pMxDoc = (IMxDocument)_App.Document;
                IMap pMap = pMxDoc.FocusMap;
                ISelectionEvents pSelectionEvents;
                pSelectionEvents = pMap as ISelectionEvents;
                IEnvelope bounds = pMxDoc.ActiveView.ScreenDisplay.
                    DisplayTransformation.FittedBounds;
                int b4SelCount = -1;
                int afterSelCount = -1;
                int oid;

                if ((pNode.ImageIndex == IMG_IDX_POINT_FC) ||
                    (pNode.ImageIndex == IMG_IDX_LINE_FC) ||
                    (pNode.ImageIndex == IMG_IDX_POLYGON_FC) ||
                    (pNode.ImageIndex == IMG_IDX_ANNO_FC))
                {
                    IEnumLayer enumLayer = pMap.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
                    enumLayer.Reset();
                    ILayer layer;
                    oid = Convert.ToInt32(StripOutNumberFromNodeText(pNode.Text));

                    //loop through map feature layers  
                    while ((layer = enumLayer.Next()) != null)
                    {
                        //Validate the layer and check layer name
                        if (layer is IFeatureLayer)
                        {
                            pFL = (IFeatureLayer)layer;
                            pDS = (IDataset)pFL.FeatureClass;
                            if ((layer.Valid) &&
                                (pDS.Name.ToLower() == StripOutNumberFromNodeText(pNode.Parent.Text).ToLower() &&
                                (pFL.Selectable)))
                            {
                                IFeatureSelection pFSel = (IFeatureSelection)pFL;
                                b4SelCount = pFSel.SelectionSet.Count;

                                List<int> removeList = new List<int>();
                                removeList.Add(oid);
                                int[] oidRemoveList = removeList.ToArray();
                                pFSel.SelectionSet.RemoveList(
                                    oidRemoveList.Length,
                                    ref oidRemoveList[0]);
                                afterSelCount = pFSel.SelectionSet.Count;

                                //If selection has increased we can exit out of this loop 
                                if (b4SelCount > afterSelCount)
                                {
                                    redrawSelectionPhase = true;
                                    selectionChanged = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (pNode.ImageIndex == IMG_IDX_TBL)
                {
                    IStandaloneTableCollection pStandAloneTableCollection =
                        (IStandaloneTableCollection)pMap;
                    IStandaloneTable pStandaloneTable = null;
                    ITable pTable = null;
                    oid = Convert.ToInt32(StripOutNumberFromNodeText(pNode.Text));

                    //loop through map standalone tables  
                    for (int i = 0; i <= pStandAloneTableCollection.StandaloneTableCount - 1; i++)
                    {
                        pStandaloneTable = pStandAloneTableCollection.get_StandaloneTable(i);
                        pDS = (IDataset)pStandaloneTable;
                        pTable = (ITable)pStandaloneTable;
                        if (pTable == null)
                        {
                            continue;
                        }

                        if ((pStandaloneTable.Valid) &&
                                (pDS.Name.ToLower() == pNode.Parent.Text.ToLower()))
                        {
                            //get selection set from table
                            ITableSelection pTableSel = (ITableSelection)pStandaloneTable;
                            b4SelCount = pTableSel.SelectionSet.Count;
                            List<int> removeList = new List<int>();
                            removeList.Add(oid);
                            int[] oidRemoveList = removeList.ToArray();
                            pTableSel.SelectionSet.RemoveList(
                                 oidRemoveList.Length,
                                    ref oidRemoveList[0]);
                            afterSelCount = pTableSel.SelectionSet.Count;

                            //If selection has increased we can exit out of this loop 
                            if (b4SelCount > afterSelCount)
                            {
                                selectionChanged = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    //Do nothing 
                    return;
                }

                //Notify of select change and refresh selection phase ONLY  
                if (selectionChanged)
                    pSelectionEvents.SelectionChanged();
                if (redrawSelectionPhase)
                    pMxDoc.ActiveView.PartialRefresh(
                        esriViewDrawPhase.esriViewGeoSelection,
                        null,
                        bounds);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in RemoveFromSelection function. " + ex.Message);
            }
        }

        //INC000004230713, INC000003945000
        public void DeleteTableRecord(string datasetName, int oid, TreeView tvwResults)
        {
            IFeatureWorkspace pFWS = null;
            IWorkspaceEdit pWSE = null;
            ITable pTable = null;
            IQueryFilter qFilter = null;
            ICursor pCursor = null;
            try
            {
                //delete record 
                pWSE = (IWorkspaceEdit)Editor.EditWorkspace;
                if ((pWSE == null) || !(pWSE.IsBeingEdited()))
                    return;
                pWSE.StartEditOperation();

                pFWS = (IFeatureWorkspace)pWSE;
                pTable = pFWS.OpenTable(datasetName);
                qFilter = new QueryFilterClass();
                qFilter.WhereClause = pTable.OIDFieldName + "=" + oid;
                ICursor pDelCursor = pTable.Search(qFilter, false);
                IRow pDelRow = null;
                while ((pDelRow = pDelCursor.NextRow()) != null)
                {
                    pDelRow.Delete();
                }
                if (pDelCursor != null) { Marshal.ReleaseComObject(pDelCursor); }
                pWSE.StopEditOperation();

                //update tree view
                TreeNode pTreeNode = tvwResults.SelectedNode;
                if (pTreeNode.ImageIndex == ValidationEngine.IMG_IDX_TBL)
                {
                    TreeNode newNode = pTreeNode.Parent;
                    pTreeNode.Remove();

                    deleteParentNode(newNode);
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in DeleteTableRecord function. " + ex.Message);
            }
            finally
            {
                if (qFilter != null) { Marshal.ReleaseComObject(qFilter); }
                if (pCursor != null) { Marshal.ReleaseComObject(pCursor); }                
            }
        }

        public void deleteParentNode(TreeNode node)
        {
            try
            {
                if (node != null && node.Nodes != null && node.Nodes.Count == 0)
                {
                    TreeNode newNode = node.Parent;
                    node.Remove();
                    if (newNode != null)
                        deleteParentNode(newNode);
                }
                else
                    return;
            }
            catch
            {
                return;
            }
        }

        public bool IsSelected(TreeNode pNode)
        {
            try
            {
                //Get the focus map 
                bool isSelected = false;
                IFeatureLayer pFL = null;
                IDataset pDS = null;
                IMxDocument pMxDoc = (IMxDocument)_App.Document;
                IMap pMap = pMxDoc.FocusMap;
                ISelectionEvents pSelectionEvents;
                pSelectionEvents = pMap as ISelectionEvents;
                int oid;

                if ((pNode.ImageIndex == IMG_IDX_POINT_FC) ||
                    (pNode.ImageIndex == IMG_IDX_LINE_FC) ||
                    (pNode.ImageIndex == IMG_IDX_POLYGON_FC) ||
                    (pNode.ImageIndex == IMG_IDX_ANNO_FC))
                {
                    IEnumLayer enumLayer = pMap.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
                    enumLayer.Reset();
                    ILayer layer;
                    oid = Convert.ToInt32(StripOutNumberFromNodeText(pNode.Text));

                    //loop through map feature layers  
                    while ((layer = enumLayer.Next()) != null)
                    {
                        //Validate the layer and check layer name
                        if (layer is IFeatureLayer)
                        {
                            pFL = (IFeatureLayer)layer;
                            pDS = (IDataset)pFL.FeatureClass;
                            if ((layer.Valid) &&
                                (pDS.Name.ToLower() == StripOutNumberFromNodeText(pNode.Parent.Text).ToLower() &&
                                (pFL.Selectable)))
                            {
                                IFeatureSelection pFSel = (IFeatureSelection)pFL;
                                IEnumIDs enumIDs = pFSel.SelectionSet.IDs;
                                int iD = enumIDs.Next();
                                while (iD != -1) //-1 is reutned after the last valid ID has been reached       
                                {
                                    if (oid == iD)
                                    {
                                        isSelected = true;
                                        break;
                                    }
                                    iD = enumIDs.Next();
                                }
                            }
                        }
                    }
                }
                else if (pNode.ImageIndex == IMG_IDX_TBL)
                {
                    IStandaloneTableCollection pStandAloneTableCollection =
                        (IStandaloneTableCollection)pMap;
                    IStandaloneTable pStandaloneTable = null;
                    ITable pTable = null;
                    oid = Convert.ToInt32(StripOutNumberFromNodeText(pNode.Text));

                    //loop through map standalone tables  
                    for (int i = 0; i <= pStandAloneTableCollection.StandaloneTableCount - 1; i++)
                    {
                        pStandaloneTable = pStandAloneTableCollection.get_StandaloneTable(i);
                        pDS = (IDataset)pStandaloneTable;
                        pTable = (ITable)pStandaloneTable;
                        if (pTable == null)
                        {
                            continue;
                        }

                        if ((pStandaloneTable.Valid) &&
                                (pDS.Name.ToLower() == pNode.Parent.Text.ToLower()))
                        {
                            //get selection set from table
                            ITableSelection pTableSel = (ITableSelection)pStandaloneTable;
                            IEnumIDs enumIDs = pTableSel.SelectionSet.IDs;
                            int iD = enumIDs.Next();
                            while (iD != -1)
                            {
                                if (oid == iD)
                                {
                                    isSelected = true;
                                    break;
                                }
                                iD = enumIDs.Next();
                            }
                        }
                    }
                }
                else
                {
                    //Do nothing 
                    return false;
                }

                //Return flag to indicate whether or not the row is selected 
                //in focus map 
                return isSelected;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in IsSelected function. " + ex.Message);
            }
        }


        /// <summary>
        /// Pass the function what kind of filter is being applied to the validation and 
        /// the name of the validation rule to be run and the function will return a boolean
        /// to indicate whether the rule should be enabled 
        /// </summary>
        /// <param name="name">The name.</param>
        public bool IsQAQCRuleEnabled(string ruleName, PGEDifferenceType differenctType, INameLog GDBMLogger = null, string dataSetName = null)
        {
            try
            {
                bool IsEnabled = false;
                _logger.Debug("Entering IsQAQCRuleEnabled");

                //_logger.Debug("Rule count for hshValidationSeverity is: " + _hshValidationSeverity.Count.ToString());                
                //if (_hshValidationSeverity.Count == 0)
                //    _hshValidationSeverity = LoadRuleSeverities();
                //_logger.Debug("After loading rules hshValidationSeverity count is: " + _hshValidationSeverity.Count.ToString());
                //int severity = -1; 

                //Domain used with SEVERITY field 
                //0 - error 
                //1 - warning 
                //7 - product not in use - esri rules 
                //8 - product not in use - telvent rules 

                //Determine if the passed rule is a warning or an error 
                //if (_hshValidationSeverity.ContainsKey(ruleName.ToLower()))
                //    severity = (int)_hshValidationSeverity[ruleName.ToLower()]; 
                //else 
                //    _logger.Debug("Unable to determine severity for rule: " + ruleName);

                int severity = GetSeverity(ruleName);

                //Determine if the rule should be enabled 
                switch (_valFilterType)
                {
                    case ValidationFilterType.valFilterTypeAll:
                        //enable provided the rule is in use 
                        IsEnabled = true;
                        break;
                    case ValidationFilterType.valFilterTypeErrorOnly:
                        //enable provided the rule is error 
                        if (severity == 0)
                            IsEnabled = true;
                        break;
                    case ValidationFilterType.valFilterTypeWarningOnly:
                        if (severity == 1)
                            IsEnabled = true;
                        break;
                    case ValidationFilterType.valFilterTypeSelection:
                        if (_hshSelectedQAQCRules != null)
                        {
                            if (_hshSelectedQAQCRules.ContainsKey(ruleName))
                                IsEnabled = true;
                        }
                        break;
                    default:
                        _logger.Debug("Error unknown validation filter");
                        throw new Exception("Invalid validation filter");
                }

                //if (IsEnabled)
                //    System.Diagnostics.Debug.Print("this is one"); 
                _logger.Debug("IsQAQCRuleEnabled Returning: " + IsEnabled.ToString() + " for: " + ruleName);

                if (IsEnabled && differenctType == PGEDifferenceType.pgeFeederManagerDifference)
                {
                    //If this is a feeder manager difference, then we only want to run the validation rule if it is
                    //one for feeder manager validation.
                    if (!FeederManager2.FeederManagerValidationRules.Contains(ruleName))
                    {
                        IsEnabled = false;
                    }
                    else
                    {

                    }
                }

                //**Added for ETAR project, May 10th-2019
                if (!IsEnabled)
                {
                    if (!string.IsNullOrEmpty(dataSetName))
                    {
                        string sFcName = null;
                        for (int i = 0; i < PGEError._errorForceClassList.Count; i++)
                        {
                            sFcName = PGEError._errorForceClassList[i];
                            if (!string.IsNullOrEmpty(sFcName))
                            {
                                //Logic specific to Substation-PNode requirement.
                                if (dataSetName.ToUpper() == sFcName.ToUpper())
                                {
                                    if (ruleName.ToUpper().Contains(SchemaInfo.General.ESRI_RULES))
                                    {
                                        //if class has model name applied, and rule is ESRI Rules, then change sevirity to 0, which is an error.
                                        IsEnabled = true;
                                        if (GDBMLogger != null)
                                        { GDBMLogger.Info("IsQAQCRuleEnabled Returning 3: " + IsEnabled.ToString() + " for: " + ruleName); }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                //**
                return IsEnabled;
            }
            catch (Exception ex)
            {
                if (GDBMLogger != null)
                { GDBMLogger.Info("entering error handler for IsQAQCRuleEnabled: " + ex.Message); }
                _logger.Debug("entering error handler for IsQAQCRuleEnabled: " + ex.Message);
                //assume the error should be run (to be safe) 
                return true;
            }
        }

        public void PrintErrors(ID8List fullErrorList, string callingClass)
        {
            try
            {
                //Print out the deletes 
                if (fullErrorList == null)
                    return;
                bool reachedValLevel = false;
                fullErrorList.Reset();
                ID8ListItem pListItem = fullErrorList.Next();
                ID8ListItem pParListItem = null;

                //If the parent of the item is a attribute rel item 
                //or a iD8mmFeature then we have come down far enough 
                //and no need to proceed any further 

                while (pListItem != null)
                {
                    System.Diagnostics.Debug.Print(callingClass + " : " + pListItem.DisplayName);

                    if (pListItem.ContainedBy != null)
                    {
                        pParListItem = (ID8ListItem)pListItem.ContainedBy;
                        if ((pParListItem.ItemType == mmd8ItemType.mmd8itFeature) ||
                            (pParListItem.ItemType == mmd8ItemType.mmitAttrRelationshipInstance))
                            reachedValLevel = true;
                        else
                            reachedValLevel = false;
                    }

                    if (pListItem is IMMValidationError)
                    {
                        IMMValidationError pValError = (IMMValidationError)pListItem;
                        System.Diagnostics.Debug.Print(pListItem.DisplayName);
                    }

                    if (pListItem is ID8List)
                    {
                        if (!reachedValLevel)
                        {
                            //System.Diagnostics.Debug.Print(pListItem.DisplayName); 
                            ID8List pChildD8List = (ID8List)pListItem;
                            //Recursive call here 
                            PrintErrors(pChildD8List, callingClass);
                        }
                    }
                    pListItem = fullErrorList.Next();
                }
            }
            catch
            {
                throw new Exception("Error populating the session errors");
            }
        }

        /// <summary>
        /// Where QAQC is run on a session and where there are a huge amount 
        /// of errors returned - looping through the ID8List is taking a large 
        /// amount of time. This routine will add just the filtered custom  QA 
        /// rules to a list so that finding the QAQC failures of the filtered 
        /// severity will be extremely fast 
        /// </summary>
        //public void AddError(string errorMsg, int severity, IObject pObject)
        //{
        //    try
        //    {

        //        switch (_valFilterType) 
        //        {
        //            case ValidationHelper.ValidationFilterType.valFilterTypeAll:
        //                //Do nothing, no need to capture because no filter 
        //                //is being applied 
        //                break; 
        //            case ValidationHelper.ValidationFilterType.valFilterTypeErrorOnly:
        //                if (severity == 0)
        //                {
        //                    IDataset pDS = (IDataset)pObject.Class; 
        //                    string datasetName = pDS.BrowseName;
        //                    PGEError pError = new PGEError(GetShortDatasetName(datasetName), pObject.OID,
        //                        GetFCImageIndex(pObject.Class), errorMsg);
        //                    _hshFilteredErrors.Add(_hshFilteredErrors.Count, pError); 
        //                }
        //                break; 
        //            case ValidationHelper.ValidationFilterType.valFilterTypeWarningOnly:
        //                if (severity == 1)
        //                {
        //                    IDataset pDS = (IDataset)pObject.Class;
        //                    string datasetName = pDS.BrowseName;
        //                    PGEError pError = new PGEError(GetShortDatasetName(datasetName), pObject.OID,
        //                        GetFCImageIndex(pObject.Class), errorMsg);
        //                    _hshFilteredErrors.Add(_hshFilteredErrors.Count, pError);
        //                }
        //                break; 
        //            default:  
        //                _logger.Debug("Error unknown validation filter"); 
        //                throw new Exception("Invalid validation filter"); 
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Debug("entering error handler for AddError: " + ex.Message);
        //        throw new Exception("Error adding error in ValidationEngine"); 
        //    }
        //}

        private string GetShortDatasetName(string datasetName)
        {
            try
            {
                string shortDatasetName = "";
                int posOfLastPeriod = -1;

                posOfLastPeriod = datasetName.LastIndexOf(".");
                if (posOfLastPeriod != -1)
                {
                    shortDatasetName = datasetName.Substring(
                        posOfLastPeriod + 1, (datasetName.Length - posOfLastPeriod) - 1);
                }
                else
                {
                    shortDatasetName = datasetName;
                }

                return shortDatasetName;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning the shortened dataset name");
            }
        }

        public int GetSeverity(string ruleName)
        {
            try
            {
                int severity = 1;
                if (_hshSeverities.ContainsKey(ruleName))
                    severity = (int)_hshSeverities[ruleName];
                else
                {
                    //throw new Exception("Severity for rule: " + ruleName + " is not listed in severitymap table");
                    severity = 1;
                }
                return severity;
            }
            catch
            {
                throw new Exception("Unable to get the severity for rule: " + ruleName);
            }
        }

        public Hashtable GetVersionDifferences_CWOSL(
          IVersionedWorkspace pVWS,
          esriDifferenceType[] pDiffTypes,
          bool returnFMDifferences)
        {
            List<IGeometricNetwork> editedNetworks = new List<IGeometricNetwork>();
            try
            {
                //Get the edit version
                IVersion pDefaultVersion = pVWS.DefaultVersion;
                IVersion pDesignVersion = (IVersion)pVWS;
                Hashtable hshEditedObjects = new Hashtable();

                if ((pDefaultVersion != null) && (pDesignVersion != null))
                {
                    //Find the common ancestor version 
                    IVersion2 pDesignVersion2 = (IVersion2)pDesignVersion;
                    IVersion pCommonAncestorVersion = pDesignVersion2.
                        GetCommonAncestor(pDefaultVersion);

                    //Find the featureclasses edited in the version 
                    Hashtable hshEditedClasses = GetClassesEditedInVersion(
                        pDesignVersion, pDefaultVersion);

                    //Set up step progressor
                    if (hshEditedClasses.Count != 0)
                    {
                        
                        //Loop through each of the modified classes
                        int fcCounter = 0;
                        foreach (string className in hshEditedClasses.Keys)
                        {
                            fcCounter++;

                            //Get the version differences cursor 
                            IGeometricNetwork editedNetwork = FindVersionDifferences(
                                pDesignVersion,
                                pDefaultVersion,
                                pCommonAncestorVersion,
                                className,
                                ref hshEditedObjects,
                                pDiffTypes);

                            if (editedNetwork != null && !editedNetworks.Contains(editedNetwork))
                            {
                                editedNetworks.Add(editedNetwork);
                            }
                        }
                    }

                    if (returnFMDifferences)
                    {
                        //Now need to process for the feeder manager 2.0 edits.
                        foreach (IGeometricNetwork geomNetwork in editedNetworks)
                        {
                            
                            IFeatureClassContainer featClassContainer = geomNetwork as IFeatureClassContainer;

                            Dictionary<int, List<int>> FeatClassToOIDsEdited = FeederManager2.GetEditedFeatures((IFeatureWorkspace)pDefaultVersion, pDesignVersion.VersionName);
                            //foreach (KeyValuePair<int, List<int>> kvp in FeatClassToOIDsEdited)
                            //{
                            //    sProgressor.MaxRange += kvp.Value.Count;
                            //}

                            IFeatureClass featClass = null;

                            for (int i = 0; i < featClassContainer.ClassCount; i++)
                            {
                                featClass = featClassContainer.get_Class(i);
                                IDataset ds = featClass as IDataset;

                                if (!FeatClassToOIDsEdited.ContainsKey(featClass.ObjectClassID)) { continue; }

                                List<int> oids = FeatClassToOIDsEdited[featClass.ObjectClassID];

                                //For validation purposes we actually only need the object ID field, so let's only request that
                                List<string> whereInStringList = new List<string>();
                                StringBuilder whereInStringBuilder = new StringBuilder();
                                for (int j = 0; j < oids.Count; j++)
                                {
                                    if (j == oids.Count - 1 || (j != 0 && (j % 999) == 0)) { whereInStringBuilder.Append(oids[j]); }
                                    else { whereInStringBuilder.Append(oids[j] + ","); }

                                    if ((j % 999) == 0 && j != 0)
                                    {
                                        whereInStringList.Add(whereInStringBuilder.ToString());
                                        whereInStringBuilder = new StringBuilder();
                                    }
                                }

                                if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                                {
                                    whereInStringList.Add(whereInStringBuilder.ToString());
                                }

                                foreach (string whereInClause in whereInStringList)
                                {
                                    IQueryFilter qf = new QueryFilterClass();
                                    qf.AddField(featClass.OIDFieldName);
                                    qf.WhereClause = featClass.OIDFieldName + " in (" + whereInClause + ")";

                                    IFeatureCursor features = featClass.Search(qf, false);
                                    IFeature feature = null;
                                    while ((feature = features.NextFeature()) != null)
                                    {
                                        FeederManagerDiffObject diffObject = new FeederManagerDiffObject(feature, ds.BrowseName, feature.OID);
                                        hshEditedObjects.Add(hshEditedObjects.Count, diffObject);
                                    }
                                    Marshal.ReleaseComObject(features);
                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Error returning version difference - version not found");
                }
                return hshEditedObjects;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning version differences");
            }
            finally
            {
            }
        }

        public Hashtable GetVersionDifferences(
            IVersionedWorkspace pVWS,
            esriDifferenceType[] pDiffTypes,
            bool returnFMDifferences)
        {
            IStatusBar stbar = _App.StatusBar;
            IStepProgressor sProgressor = stbar.ProgressBar;
            List<IGeometricNetwork> editedNetworks = new List<IGeometricNetwork>();
            try
            {
                //Get the edit version
                sProgressor.Message = "Performing Version Difference";
                IVersion pDefaultVersion = pVWS.DefaultVersion;
                IVersion pDesignVersion = (IVersion)pVWS;
                Hashtable hshEditedObjects = new Hashtable();

                if ((pDefaultVersion != null) && (pDesignVersion != null))
                {
                    //Find the common ancestor version 
                    IVersion2 pDesignVersion2 = (IVersion2)pDesignVersion;
                    IVersion pCommonAncestorVersion = pDesignVersion2.
                        GetCommonAncestor(pDefaultVersion);

                    //Find the featureclasses edited in the version 
                    Hashtable hshEditedClasses = GetClassesEditedInVersion(
                        pDesignVersion, pDefaultVersion);

                    //Set up step progressor
                    if (hshEditedClasses.Count != 0)
                    {
                        sProgressor.MinRange = 0;
                        sProgressor.MaxRange = hshEditedClasses.Count;
                        sProgressor.Position = 1;
                        sProgressor.Message = "Performing Version Difference";
                        sProgressor.StepValue = 1;
                        sProgressor.Show();

                        //Loop through each of the modified classes
                        int fcCounter = 0;
                        foreach (string className in hshEditedClasses.Keys)
                        {
                            fcCounter++;
                            sProgressor.Message =
                                "Returning changes for class: " +
                                fcCounter.ToString() + " of: " + hshEditedClasses.Count.ToString() +
                                " : " + className;

                            //Get the version differences cursor 
                            IGeometricNetwork editedNetwork = FindVersionDifferences(
                                pDesignVersion,
                                pDefaultVersion,
                                pCommonAncestorVersion,
                                className,
                                ref hshEditedObjects,
                                pDiffTypes);
                            if (sProgressor.Position != sProgressor.MaxRange)
                                sProgressor.Step();

                            if (editedNetwork != null && !editedNetworks.Contains(editedNetwork))
                            {
                                editedNetworks.Add(editedNetwork);
                            }
                        }
                    }

                    if (returnFMDifferences)
                    {
                        //Now need to process for the feeder manager 2.0 edits.
                        foreach (IGeometricNetwork geomNetwork in editedNetworks)
                        {
                            sProgressor.Message = "Returning circuit changes for network: " + ((IDataset)geomNetwork).BrowseName;

                            IFeatureClassContainer featClassContainer = geomNetwork as IFeatureClassContainer;

                            sProgressor.MinRange = 0;
                            sProgressor.MaxRange = 1;
                            sProgressor.Position = 1;
                            sProgressor.StepValue = 1;
                            sProgressor.Show();

                            Dictionary<int, List<int>> FeatClassToOIDsEdited = FeederManager2.GetEditedFeatures((IFeatureWorkspace)pDefaultVersion, pDesignVersion.VersionName);
                            //foreach (KeyValuePair<int, List<int>> kvp in FeatClassToOIDsEdited)
                            //{
                            //    sProgressor.MaxRange += kvp.Value.Count;
                            //}
                            sProgressor.MaxRange = FeatClassToOIDsEdited.Keys.Count;

                            IFeatureClass featClass = null;

                            for (int i = 0; i < featClassContainer.ClassCount; i++)
                            {
                                featClass = featClassContainer.get_Class(i);
                                IDataset ds = featClass as IDataset;

                                sProgressor.Message = "Returning circuit changes for: " + ds.BrowseName;

                                if (!FeatClassToOIDsEdited.ContainsKey(featClass.ObjectClassID)) { continue; }

                                List<int> oids = FeatClassToOIDsEdited[featClass.ObjectClassID];

                                //For validation purposes we actually only need the object ID field, so let's only request that
                                List<string> whereInStringList = new List<string>();
                                StringBuilder whereInStringBuilder = new StringBuilder();
                                for (int j = 0; j < oids.Count; j++)
                                {
                                    if (j == oids.Count - 1 || (j != 0 && (j % 999) == 0)) { whereInStringBuilder.Append(oids[j]); }
                                    else { whereInStringBuilder.Append(oids[j] + ","); }

                                    if ((j % 999) == 0 && j != 0)
                                    {
                                        whereInStringList.Add(whereInStringBuilder.ToString());
                                        whereInStringBuilder = new StringBuilder();
                                    }
                                }

                                if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                                {
                                    whereInStringList.Add(whereInStringBuilder.ToString());
                                }

                                foreach (string whereInClause in whereInStringList)
                                {
                                    IQueryFilter qf = new QueryFilterClass();
                                    qf.AddField(featClass.OIDFieldName);
                                    qf.WhereClause = featClass.OIDFieldName + " in (" + whereInClause + ")";

                                    IFeatureCursor features = featClass.Search(qf, false);
                                    IFeature feature = null;
                                    while ((feature = features.NextFeature()) != null)
                                    {
                                        FeederManagerDiffObject diffObject = new FeederManagerDiffObject(feature, ds.BrowseName, feature.OID);
                                        hshEditedObjects.Add(hshEditedObjects.Count, diffObject);
                                    }
                                    Marshal.ReleaseComObject(features);
                                }

                                if (sProgressor.Position != sProgressor.MaxRange)
                                    sProgressor.Step();
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Error returning version difference - version not found");
                }
                return hshEditedObjects;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning version differences");
            }
            finally
            {
                sProgressor.Hide();
            }
        }

        public Hashtable GetVersionDifferences(
            IVersionedWorkspace pVWS,
            esriDifferenceType[] pDiffTypes,
            bool returnFMDefaultJunctions,
            bool returnFMDifferences,
            INameLog GDBMLogger,
            string GDBMServiceConfigurationName)
        {
            List<IGeometricNetwork> editedNetworks = new List<IGeometricNetwork>();
            try
            {
                //Get the edit version 
                GDBMLogger.Info(GDBMServiceConfigurationName, "Performing Version Difference");
                IVersion pDefaultVersion = pVWS.DefaultVersion;
                IVersion pDesignVersion = (IVersion)pVWS;
                Hashtable hshEditedObjects = new Hashtable();

                if ((pDefaultVersion != null) && (pDesignVersion != null))
                {
                    //Find the common ancestor version 
                    IVersion2 pDesignVersion2 = (IVersion2)pDesignVersion;
                    IVersion pCommonAncestorVersion = pDesignVersion2.
                        GetCommonAncestor(pDefaultVersion);

                    //Find the featureclasses edited in the version 
                    Hashtable hshEditedClasses = GetClassesEditedInVersion(
                        pDesignVersion, pDefaultVersion);

                    //Set up step progressor
                    if (hshEditedClasses.Count != 0)
                    {
                        //Loop through each of the modified classes
                        int fcCounter = 0;
                        foreach (string className in hshEditedClasses.Keys)
                        {
                            fcCounter++;
                            GDBMLogger.Info(GDBMServiceConfigurationName, "Returning changes for class: " + fcCounter.ToString() + " of: " +
                                hshEditedClasses.Count.ToString() + " : " + className);

                            //Get the version differences cursor 
                            IGeometricNetwork editedNetwork = FindVersionDifferences(
                                pDesignVersion,
                                pDefaultVersion,
                                pCommonAncestorVersion,
                                className,
                                ref hshEditedObjects,
                                pDiffTypes);

                            if (editedNetwork != null && !editedNetworks.Contains(editedNetwork))
                            {
                                editedNetworks.Add(editedNetwork);
                            }
                        }
                    }

                    if (returnFMDifferences)
                    {
                        //Now need to process for the feeder manager 2.0 edits.
                        foreach (IGeometricNetwork geomNetwork in editedNetworks)
                        {
                            GDBMLogger.Info(GDBMServiceConfigurationName, "Returning circuit changes for network: " + ((IDataset)geomNetwork).BrowseName);

                            IFeatureClassContainer featClassContainer = geomNetwork as IFeatureClassContainer;

                            Dictionary<int, List<int>> FeatClassToOIDsEdited = FeederManager2.GetEditedFeatures((IFeatureWorkspace)pDefaultVersion, pDesignVersion.VersionName);

                            IFeatureClass featClass = null;

                            for (int i = 0; i < featClassContainer.ClassCount; i++)
                            {
                                featClass = featClassContainer.get_Class(i);

                                if (!returnFMDefaultJunctions &&
                                    geomNetwork.OrphanJunctionFeatureClass.ObjectClassID == featClass.ObjectClassID) { continue; }

                                IDataset ds = featClass as IDataset;
                                GDBMLogger.Info(GDBMServiceConfigurationName, "Returning circuit changes for: " + ds.BrowseName);

                                if (!FeatClassToOIDsEdited.ContainsKey(featClass.ObjectClassID)) { continue; }

                                List<int> oids = FeatClassToOIDsEdited[featClass.ObjectClassID];

                                //For validation purposes we actually only need the object ID field, so let's only request that
                                List<string> whereInStringList = new List<string>();
                                StringBuilder whereInStringBuilder = new StringBuilder();
                                for (int j = 0; j < oids.Count; j++)
                                {
                                    if (j == oids.Count - 1 || (j != 0 && (j % 999) == 0)) { whereInStringBuilder.Append(oids[j]); }
                                    else { whereInStringBuilder.Append(oids[j] + ","); }

                                    if ((j % 999) == 0 && j != 0)
                                    {
                                        whereInStringList.Add(whereInStringBuilder.ToString());
                                        whereInStringBuilder = new StringBuilder();
                                    }
                                }

                                if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                                {
                                    whereInStringList.Add(whereInStringBuilder.ToString());
                                }

                                foreach (string whereInClause in whereInStringList)
                                {
                                    IQueryFilter qf = new QueryFilterClass();
                                    qf.AddField(featClass.OIDFieldName);
                                    qf.WhereClause = featClass.OIDFieldName + " in (" + whereInClause + ")";

                                    IFeatureCursor features = featClass.Search(qf, false);
                                    IFeature feature = null;
                                    while ((feature = features.NextFeature()) != null)
                                    {
                                        FeederManagerDiffObject diffObject = new FeederManagerDiffObject(feature, ds.BrowseName, feature.OID);
                                        hshEditedObjects.Add(hshEditedObjects.Count, diffObject);
                                    }
                                    Marshal.ReleaseComObject(features);

                                }
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Error returning version difference - version not found");
                }
                return hshEditedObjects;
            }
            catch (Exception ex)
            {
                throw new Exception("Error returning version differences");
            }
            finally
            {

            }
        }


        private Hashtable GetClassesEditedInVersion(
            IVersion pDesignVersion,
            IVersion pDefaultVersion)
        {
            try
            {
                Hashtable hshEditedClasses = new Hashtable();
                IDataChanges diffs = null;
                IEnumModifiedClassInfo modifiedClasses = null;
                IVersionDataChangesInit versionDiffsInit = (IVersionDataChangesInit)new VersionDataChangesClass();
                IWorkspaceName pWSNDesign = (IWorkspaceName)(pDesignVersion as IDataset).FullName;
                IWorkspaceName pWSNDefault = (IWorkspaceName)(pDefaultVersion as IDataset).FullName;

                //Initialize the IVersionDataChangesInit 
                versionDiffsInit.Init(pWSNDesign, pWSNDefault);

                diffs = (IDataChanges)versionDiffsInit;
                modifiedClasses = diffs.GetModifiedClassesInfo();

                IModifiedClassInfo oneModifiedClass = null;
                string sName = null;

                //Find the modified classes that we are interested in                 
                oneModifiedClass = modifiedClasses.Next();
                while (oneModifiedClass != null)
                {
                    sName = oneModifiedClass.ChildClassName.ToUpper();
                    if (!hshEditedClasses.ContainsKey(sName))
                        hshEditedClasses.Add(sName, 0);
                    oneModifiedClass = modifiedClasses.Next();
                }
                return hshEditedClasses;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed returning version difference:" + ex.Message);
                throw ex;
            }
        }

        public IGeometricNetwork FindVersionDifferences(
            IVersion pEditVersion,
            IVersion pDefaultVersion,
            IVersion pCommonAncestorVersion,
            string tableName,
            ref Hashtable hshDiffObjects,
            esriDifferenceType[] pDiffTypes)
        {
            IGeometricNetwork editedNetwork = null;
            try
            {
                _logger.Debug("Entering FindVersionDifferences " + DateTime.Now.ToString());
                _logger.Debug("    Table: " + tableName);
                _logger.Debug("    Version: " + pEditVersion.VersionName);

                // Cast the child version to IFeatureWorkspace and open the table.
                Debug.Print("Processing featureclass: " + tableName);
                IFeatureWorkspace pChildFWS = (IFeatureWorkspace)pEditVersion;
                IFeatureWorkspace pDefaultFWS = (IFeatureWorkspace)pDefaultVersion;
                ITable pChildTable = pChildFWS.OpenTable(tableName);
                if (pChildTable is INetworkClass)
                {
                    INetworkClass networkClass = pChildTable as INetworkClass;
                    editedNetwork = networkClass.GeometricNetwork;
                }

                IDataset pChildDS = (IDataset)pChildTable;

                // Cast the common ancestor version to IFeatureWorkspace and open the table.
                IFeatureWorkspace commonAncestorFWS = (IFeatureWorkspace)pCommonAncestorVersion;
                ITable commonAncestorTable = commonAncestorFWS.OpenTable(tableName);

                // Cast to the IVersionedTable interface to create a difference cursor.
                IVersionedTable versionedTable = (IVersionedTable)pChildTable;

                for (int i = 0; i < pDiffTypes.Length; i++)
                {
                    _logger.Debug("Looking for differences of type: " + pDiffTypes[i].ToString());
                    IDifferenceCursor differenceCursor = versionedTable.Differences
                        (commonAncestorTable, pDiffTypes[i], null);

                    // Step through the cursor, storing OId of each row
                    IRow differenceRow = null;
                    int objectID = -1;
                    differenceCursor.Next(out objectID, out differenceRow);
                    List<int> VersionDiffOIDs = new List<int>();

                    while (objectID != -1)
                    {
                        if ((pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeDeleteNoChange) ||
                            (pDiffTypes[i] == esriDifferenceType.esriDifferenceTypeDeleteUpdate))
                        {
                            //Create a DeletedObject
                            DeletedObject pDelObj = new DeletedObject(
                                tableName.ToUpper(), objectID);
                            hshDiffObjects.Add(hshDiffObjects.Count, pDelObj); 
                        }
                        else
                        {
                            VersionDiffOIDs.Add(objectID);
                        }
                        differenceCursor.Next(out objectID, out differenceRow);
                    }
                    
                    //Version information is not associated with the difference row, so we need to get the objects directly from the child
                    //table once we know what has changed.
                    int[] oidsToObtain = VersionDiffOIDs.ToArray();
                    if (oidsToObtain.Length > 0)
                    {
                        ICursor differenceRowsToAdd = pChildTable.GetRows(oidsToObtain, false);
                        IRow row = null;
                        while ((row = differenceRowsToAdd.NextRow()) != null)
                        {
                            IDataset ds = row.Table as IDataset;
                            string currentVersion = ds.Workspace.ConnectionProperties.GetProperty("VERSION").ToString();
                            //Create a VersionDiffObject 
                            VersionDiffObject pQAQCSrc = new VersionDiffObject(
                            pDiffTypes[i], (IObject)row, tableName, row.OID);
                            hshDiffObjects.Add(hshDiffObjects.Count, pQAQCSrc);
                        }

                        if (differenceRowsToAdd != null) { while (Marshal.ReleaseComObject(differenceRowsToAdd) > 0) { } }

                        Marshal.FinalReleaseComObject(differenceCursor);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Debug("Entering FindVersionDifferences Error Handler: " + ex.Message);
                throw ex;
            }
            return editedNetwork;
        }

        /// <summary>
        /// Runs the PGE Custom QAQC 
        /// </summary>
        /// <param name="hshVersionDiffObjects"></param>
        /// <param name="hshFullErrorsList"></param>
        
        public void RunQAQCCustomised(Hashtable hshVersionDiffObjects, ref Hashtable hshFullErrorsList)
        {
            IStatusBar stbar = _App.StatusBar;
            IStepProgressor sProgressor = stbar.ProgressBar;
            bool bSetVersionDiff = false;

            try
            {
                //Hashtables to store the validation rules 
                PGEDifferenceType pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;
                if (_hshLoadedQAQCRules == null)
                    _hshLoadedQAQCRules = new Hashtable();
                if (_hshAllQAQCRules == null)
                    _hshAllQAQCRules = new Hashtable();

                //Check if there is anything to process 
                if (hshVersionDiffObjects.Count == 0)
                    return;

                //Set up step progressor 
                IDataset pDS = null;
                sProgressor.MinRange = 0;
                sProgressor.Position = 1;
                sProgressor.MaxRange = hshVersionDiffObjects.Count;
                sProgressor.Message = "Running PGE QAQC";
                sProgressor.StepValue = 1;
                sProgressor.Show();
                int qaqcCounter = 0;

                //Run QAQC on each object 
                if (hshVersionDiffObjects.Count != 0)
                {
                    if (ValidationEngine.Instance.VersionDifferences == null)
                    {
                        ValidationEngine.Instance.VersionDifferences = hshVersionDiffObjects;
                        bSetVersionDiff = true;
                    }

                    for (int i = 0; i <= hshVersionDiffObjects.Count - 1; i++)
                    {
                        IObject pTargetObject = null;
                        if (hshVersionDiffObjects[i] is VersionDiffObject)
                        {
                            //Running the QAQC on for version differences 
                            VersionDiffObject pQAQCSrc = (VersionDiffObject)
                                hshVersionDiffObjects[i];
                            pTargetObject = pQAQCSrc.Object;
                            pPGEDiffType = GetPGEDifferenceType(pQAQCSrc.DifferenceType);
                        }
                        else if (hshVersionDiffObjects[i] is FeederManagerDiffObject)
                        {
                            FeederManagerDiffObject feederDiffObject = hshVersionDiffObjects[i] as FeederManagerDiffObject;
                            pTargetObject = feederDiffObject.Object;
                            pPGEDiffType = PGEDifferenceType.pgeFeederManagerDifference;
                        }
                        else if (hshVersionDiffObjects[i] is IObject)
                        {
                            //Running the QAQC for Map Selection 
                            pTargetObject = (IObject)hshVersionDiffObjects[i];
                            pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;
                        }                        
                        else
                        {
                            //Must be a deleted object so QAQC is not necessary 
                            pTargetObject = null;
                        }

                        if (pTargetObject != null)
                        {
                            pDS = (IDataset)pTargetObject.Class;
                            sProgressor.Message = "Running PGE QAQC on: " +
                                pDS.Name + " : " + pTargetObject.OID.ToString();
                            RunQAQCOnObject(
                                pTargetObject,
                                ref hshFullErrorsList,
                                pPGEDiffType, false);
                            qaqcCounter++;
                        }

                        //Step the progressor 
                        if (sProgressor.Position != sProgressor.MaxRange)
                            sProgressor.Step();
                    }
                }

                sProgressor.Message = "";
                sProgressor.Hide();

                _logger.Debug("QAQC was run on: " + qaqcCounter.ToString() + " rows");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed in RunQAQCCustomised: " + ex.Message);
                throw new Exception("Error in RunQAQCCustomised");
            }
            finally
            {
                //Clean up resources 
                sProgressor.Message = "";
                sProgressor.Hide();
                if (_hshAllQAQCRules != null)
                {
                    _hshAllQAQCRules.Clear();
                    _hshAllQAQCRules = null;
                }
                if (_hshLoadedQAQCRules != null)
                {
                    _hshLoadedQAQCRules.Clear();
                    _hshLoadedQAQCRules = null;
                }
                if (_hshSelectedQAQCRules != null)
                {
                    _hshSelectedQAQCRules.Clear();
                    _hshSelectedQAQCRules = null;
                }
                if (bSetVersionDiff)
                {
                    // VersionDiff was null at the start of this routine so we're going to set it back to null.
                    if (ValidationEngine.Instance.VersionDifferences != null) { ValidationEngine.Instance.VersionDifferences.Clear(); }
                }
                GC.Collect();
            }
        }

        public void RunQAQCCustomised_CWOSL(Hashtable hshVersionDiffObjects, ref Hashtable hshFullErrorsList)
        {
            try
            {
                //Hashtables to store the validation rules 
                PGEDifferenceType pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;
                if (_hshLoadedQAQCRules == null)
                    _hshLoadedQAQCRules = new Hashtable();
                if (_hshAllQAQCRules == null)
                    _hshAllQAQCRules = new Hashtable();

                //Check if there is anything to process 
                if (hshVersionDiffObjects.Count == 0)
                    return;

                //Set up step progressor 
                IDataset pDS = null;
                int qaqcCounter = 0;

                //Run QAQC on each object 
                if (hshVersionDiffObjects.Count != 0)
                {
                    for (int i = 0; i <= hshVersionDiffObjects.Count - 1; i++)
                    {
                        IObject pTargetObject = null;
                        if (hshVersionDiffObjects[i] is VersionDiffObject)
                        {
                            //Running the QAQC on for version differences 
                            VersionDiffObject pQAQCSrc = (VersionDiffObject)
                                hshVersionDiffObjects[i];
                            pTargetObject = pQAQCSrc.Object;
                            pPGEDiffType = GetPGEDifferenceType(pQAQCSrc.DifferenceType);
                        }
                        else if (hshVersionDiffObjects[i] is FeederManagerDiffObject)
                        {
                            FeederManagerDiffObject feederDiffObject = hshVersionDiffObjects[i] as FeederManagerDiffObject;
                            pTargetObject = feederDiffObject.Object;
                            pPGEDiffType = PGEDifferenceType.pgeFeederManagerDifference;
                        }
                        else if (hshVersionDiffObjects[i] is IObject)
                        {
                            //Running the QAQC for Map Selection 
                            pTargetObject = (IObject)hshVersionDiffObjects[i];
                            pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;
                        }
                        else
                        {
                            //Must be a deleted object so QAQC is not necessary 
                            pTargetObject = null;
                        }

                        if (pTargetObject != null)
                        {
                            pDS = (IDataset)pTargetObject.Class;
                            RunQAQCOnObject(
                                pTargetObject,
                                ref hshFullErrorsList,
                                pPGEDiffType, false);
                            qaqcCounter++;
                        }

                       
                    }
                }


                _logger.Debug("QAQC was run on: " + qaqcCounter.ToString() + " rows");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed in RunQAQCCustomised: " + ex.Message);
                throw new Exception("Error in RunQAQCCustomised");
            }
            finally
            {
                //Clean up resources 
               
                if (_hshAllQAQCRules != null)
                {
                    _hshAllQAQCRules.Clear();
                    _hshAllQAQCRules = null;
                }
                if (_hshLoadedQAQCRules != null)
                {
                    _hshLoadedQAQCRules.Clear();
                    _hshLoadedQAQCRules = null;
                }
                if (_hshSelectedQAQCRules != null)
                {
                    _hshSelectedQAQCRules.Clear();
                    _hshSelectedQAQCRules = null;
                }
                GC.Collect();
            }
        }

        public void RunQAQCCustomised(Hashtable hshVersionDiffObjects, ref Hashtable hshFullErrorsList, INameLog GDBMLogger = null, string GDBMServiceConfigurationName = null)
        {
            try
            {
                //Clear any QAQC rules that were loaded earlier.
                if (_hshLoadedQAQCRules != null) { _hshLoadedQAQCRules.Clear(); }

                //Hashtables to store the validation rules 
                PGEDifferenceType pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;
                if (_hshLoadedQAQCRules == null)
                    _hshLoadedQAQCRules = new Hashtable();
                if (_hshAllQAQCRules == null)
                    _hshAllQAQCRules = new Hashtable();

                //Check if there is anything to process 
                if (hshVersionDiffObjects.Count == 0)
                    return;

                //Set up step progressor 
                IDataset pDS = null;
                int qaqcCounter = 0;

                //Run QAQC on each object 
                if (hshVersionDiffObjects.Count != 0)
                {
                    for (int i = 0; i <= hshVersionDiffObjects.Count - 1; i++)
                    {
                        IObject pTargetObject = null;
                        if (hshVersionDiffObjects[i] is VersionDiffObject)
                        {
                            //Running the QAQC on for version differences 
                            VersionDiffObject pQAQCSrc = (VersionDiffObject)
                                hshVersionDiffObjects[i];
                            pTargetObject = pQAQCSrc.Object;
                            pPGEDiffType = GetPGEDifferenceType(pQAQCSrc.DifferenceType);
                        }
                        else if (hshVersionDiffObjects[i] is FeederManagerDiffObject)
                        {
                            FeederManagerDiffObject feederDiffObject = hshVersionDiffObjects[i] as FeederManagerDiffObject;
                            pTargetObject = feederDiffObject.Object;
                            pPGEDiffType = PGEDifferenceType.pgeFeederManagerDifference;
                        }
                        else if (hshVersionDiffObjects[i] is IObject)
                        {
                            //Running the QAQC for Map Selection 
                            pTargetObject = (IObject)hshVersionDiffObjects[i];
                            pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;
                        }
                        else
                        {
                            //Must be a deleted object so QAQC is not necessary 
                            pTargetObject = null;
                        }

                        if (pTargetObject != null)
                        {
                            pDS = (IDataset)pTargetObject.Class;
                            if (GDBMLogger != null)
                            {
                                GDBMLogger.Info(GDBMServiceConfigurationName, "Running QAQC on: " + pDS.Name + " : " + pTargetObject.OID.ToString());
                            }
                            RunQAQCOnObject(
                                pTargetObject,
                                ref hshFullErrorsList,
                                pPGEDiffType, true, GDBMLogger);
                            qaqcCounter++;
                        }
                    }
                }
                if (GDBMLogger != null)
                {
                    GDBMLogger.Info(GDBMServiceConfigurationName, "QAQC was run on: " + qaqcCounter.ToString() + " rows");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Maps the PGEDifferenceType to the PGEDifferenceType 
        /// </summary>
        /// <param name="pESRIDiffType"></param>
        /// <returns></returns>
        public PGEDifferenceType GetPGEDifferenceType(esriDifferenceType pESRIDiffType)
        {
            PGEDifferenceType pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeNotApplicable;

            try
            {
                switch (pESRIDiffType)
                {
                    case esriDifferenceType.esriDifferenceTypeInsert:
                        pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeInsert;
                        break;
                    case esriDifferenceType.esriDifferenceTypeUpdateDelete:
                        pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeUpdate;
                        break;
                    case esriDifferenceType.esriDifferenceTypeUpdateNoChange:
                        pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeUpdate;
                        break;
                    case esriDifferenceType.esriDifferenceTypeUpdateUpdate:
                        pPGEDiffType = PGEDifferenceType.pgeDifferenceTypeUpdate;
                        break;
                    default:
                        throw new Exception("Failed returning PGEDifferenceType");
                }
                return pPGEDiffType;
            }
            catch (Exception ex)
            {
                _logger.Error("Failed returning PGEDifferenceType " + ex.Message);
                throw new Exception("Failed returning PGEDifferenceType");
            }
        }

        /// <summary>
        /// Runs the custom PGE QAQC on the passed feature / row 
        /// </summary>
        /// <param name="pObject"></param>
        /// <param name="hshFullErrorsList"></param>
        private void RunQAQCOnObject(
            IObject pObject,
            ref Hashtable hshFullErrorsList,
            PGEDifferenceType pPGEDiffType,
            bool cacheAllValidationRules, INameLog GDBMLogger = null)
        {
            string datasetName = "";

            try
            {
                //Check the feature has not already been run 
                IDataset pDS = (IDataset)pObject.Class;
                datasetName = pDS.BrowseName.ToUpper();
                ID8List pFullErrorsListForObject = new D8ListClass();

                //Check to see if we already have the rules for this 
                //featureclass                 
                if (!_hshAllQAQCRules.ContainsKey(datasetName))
                {
                    if (!cacheAllValidationRules)
                    {
                        //Load the QAQC rules for this dataset (and subtypes).
                        _hshAllQAQCRules.Add(
                            datasetName,
                            LoadValidationRulesForObjectClass(pObject.Class, cacheAllValidationRules));
                    }
                    else
                    {
                        LoadValidationRulesForObjectClass(pObject.Class, cacheAllValidationRules);
                    }
                }

                //Run all the rules that are designated for the FC and subtype 
                //for each row                  
                int subtypeValue = -1;
                string objectKey = datasetName;
                if (pObject.Class is ISubtypes)
                {
                    ISubtypes pSubtypes = (ISubtypes)pObject.Class;
                    if (pSubtypes.HasSubtype)
                    {
                        int subtypeFieldIdx = pSubtypes.SubtypeFieldIndex;
                        subtypeValue = (int)pObject.get_Value(pSubtypes.SubtypeFieldIndex);
                        objectKey = datasetName + "*" + subtypeValue.ToString();
                    }
                }
                else
                    Debug.Print(datasetName + " has no subtypes");

                //Get the validation rules for the specific dataset and subtype 
                Hashtable hshQAQCRulesForDataset = (Hashtable)_hshAllQAQCRules[datasetName];
                Hashtable hshQAQCRulesForRow = (Hashtable)hshQAQCRulesForDataset[objectKey];
                Hashtable hshFailedToLoadRules = new Hashtable();
                //if (GDBMLogger != null)
                //{
                //    GDBMLogger.Info("Latest-2:");
                //}

                if (hshQAQCRulesForRow == null)
                    return;
                foreach (string valRuleGUID in hshQAQCRulesForRow.Keys)
                {
                   
                    //Check if we have load the rule 
                    if ((!_hshLoadedQAQCRules.ContainsKey(valRuleGUID)) &&
                        (!hshFailedToLoadRules.ContainsKey(valRuleGUID)))
                    {
                        Guid pGUID = new Guid(valRuleGUID);
                        try
                        {
                            Type pMMRuleClass = Type.GetTypeFromCLSID(pGUID);
                            System.Object obj = Activator.CreateInstance(pMMRuleClass);
                            IMMValidationRule pMMValRule = (IMMValidationRule)obj;
                            IMMExtObject pExtObj = (IMMExtObject)obj;
                            //Don't load the rule unless it is enabled 
                            if (ValidationEngine.Instance.IsQAQCRuleEnabled(
                                pExtObj.Name, pPGEDiffType, GDBMLogger, datasetName))
                                _hshLoadedQAQCRules.Add(valRuleGUID, pMMValRule);

                        }
                        catch
                        {
                            //Debug.Print("Failed to load rule with Guid: " + valRuleGUID);
                            //if (GDBMLogger != null)
                            //{
                            //    GDBMLogger.Info("Failed to load rule with Guid: " + valRuleGUID);
                            //}
                            if (!hshFailedToLoadRules.ContainsKey(valRuleGUID))
                                hshFailedToLoadRules.Add(valRuleGUID, 0);
                        }
                    }

                    if (_hshLoadedQAQCRules.ContainsKey(valRuleGUID))
                    {
                        IMMValidationRule pRule = (IMMValidationRule)_hshLoadedQAQCRules[valRuleGUID];
                        IMMExtObject pExtObj = (IMMExtObject)pRule;
                        if (pExtObj.Name == "PGE Validate Source Connectivity")
                            Debug.Print("source connectivity");

                        Debug.Print("Running : " + pExtObj.Name);
                        ID8List pD8List = null;

                        try
                        {
                            pD8List = pRule.IsValid((IRow)pObject);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Failed in Rule: " +
                                pExtObj.Name + " in the IsValid Routine on object " +
                                datasetName + ":" + pObject.OID.ToString() + " " +
                                ex.Message);

                            if (GDBMLogger != null)
                            {
                                GDBMLogger.Info("Failed in Rule: " +
                                pExtObj.Name + " in the IsValid Routine on object " +
                                datasetName + ":" + pObject.OID.ToString() + " " +
                                ex.Message);
                            }
                        }
                        finally
                        {
                            GC.Collect();
                        }
                        if (pD8List != null)
                        {                            
                            UpdateFullErrorList(
                                pPGEDiffType,
                                ref hshFullErrorsList,
                                pD8List,
                                pObject,
                                pExtObj.Name);
                            pD8List.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (GDBMLogger != null)
                {
                    GDBMLogger.Info("Failed in RunQAOnObject " + datasetName + ":" + pObject.OID.ToString());
                }

                _logger.Error("Failed in RunQAOnObject " +
                    datasetName + ":" + pObject.OID.ToString());
                throw ex;
            }
        }


        public void UpdateFullErrorList(
            PGEDifferenceType pPGEDiffType,
            ref Hashtable hshFullErrorList,
            ID8List ruleErrorList,
            IObject pRow,
            string ruleName, INameLog GDBMLogger = null)
        {
            try
            {
                if (GDBMLogger != null)
                {
                    GDBMLogger.Info("hshFullErrorList " + hshFullErrorList.Count.ToString());
                }
                
                //Print out the deletes 
                if (ruleErrorList == null)
                    return;
                bool reachedValLevel = false;
                ruleErrorList.Reset();
                ID8ListItem pListItem = ruleErrorList.Next();
                ID8ListItem pParListItem = null;

                //If the parent of the item is a attribute rel item 
                //or a iD8mmFeature then we have come down far enough 
                //and no need to proceed any further 

                while (pListItem != null)
                {
                    //System.Diagnostics.Debug.Print(callingClass + " : " + pListItem.DisplayName);

                    if (pListItem.ContainedBy != null)
                    {
                        pParListItem = (ID8ListItem)pListItem.ContainedBy;
                        if ((pParListItem.ItemType == mmd8ItemType.mmd8itFeature) ||
                            (pParListItem.ItemType == mmd8ItemType.mmitAttrRelationshipInstance))
                            reachedValLevel = true;
                        else
                            reachedValLevel = false;
                    }

                    if (pListItem is IMMValidationError)
                    {
                        //Check for duplicates 
                        bool isDuplicate = false;
                        IDataset pDS = (IDataset)pRow.Table;
                        foreach (PGEError pErr in hshFullErrorList.Values)
                        {
                            if ((pErr.Dataset == pDS.Name) && (pErr.ObjectId == pRow.OID))
                            {
                                if (pErr.ErrorMsg == pListItem.DisplayName)
                                {
                                    isDuplicate = true;
                                    break;
                                }
                            }
                        }

                        if (!isDuplicate)
                        {
                            IMMValidationError pValError = (IMMValidationError)pListItem;
                            PGEError pPGEError = new PGEError(
                                pPGEDiffType,
                                pDS.Name,
                                ValidationEngine.Instance.GetSeverity(ruleName),
                                pRow.OID,
                                ValidationEngine.Instance.GetFCImageIndex((IObjectClass)pDS),
                                pValError.ErrorMessage,
                                ruleName);
                            hshFullErrorList.Add(hshFullErrorList.Count, pPGEError);
                            System.Diagnostics.Debug.Print(pListItem.DisplayName);
                        }
                        else
                        {
                            System.Diagnostics.Debug.Print("found duplicate: " + pListItem.DisplayName);
                        }
                    }

                    if (pListItem is ID8List)
                    {
                        if (!reachedValLevel)
                        {
                            //System.Diagnostics.Debug.Print(pListItem.DisplayName); 
                            ID8List pChildD8List = (ID8List)pListItem;
                            //Recursive call here 
                            UpdateFullErrorList(
                                pPGEDiffType,
                                ref hshFullErrorList,
                                pChildD8List,
                                pRow,
                                ruleName);
                        }
                    }
                    if (GDBMLogger != null)
                    {
                        GDBMLogger.Info("hshFullErrorList " + hshFullErrorList.Count.ToString());
                    }
                    pListItem = ruleErrorList.Next();
                }
            }
            catch
            {
                throw new Exception("Error populating the session errors");
            }
        }

        /// <summary>
        /// This routine will ensure for the wrapped out-of-the-box QAQC rule 
        /// that all rules have a consistent error message with a prefix dependent 
        /// on whether it is a 'Warning' or 'Error', and will screen out ESRI internal 
        /// errors 
        /// </summary>
        /// <param name="pInitialList"></param>
        /// <param name="pPGEList"></param>
        /// <param name="ruleName"></param>
        public void GetModifiedList(ID8List pInitialList, ref ID8List pPGEList, string ruleName)
        {
            try
            {
                //Print out the deletes                
                if (pInitialList == null)
                {
                    pPGEList = null;
                    return;
                }
                else
                    pPGEList = new D8ListClass();

                pInitialList.Reset();
                ID8ListItem pListItem = pInitialList.Next();

                while (pListItem != null)
                {
                    if (pListItem is IMMValidationError)
                    {
                        IMMValidationError pValError = (IMMValidationError)pListItem;
                        System.Diagnostics.Debug.Print(pListItem.DisplayName);

                        if (!IsErrorExcluded( pListItem.DisplayName))                        
                        {
                            //Create a new validation error item and add to pPGEList 
                            IMMValidationError pNewValError = new MMValidationErrorClass();
                            pNewValError.Severity = GetSeverity(ruleName);
                            pNewValError.BitmapID = 0;
                            //Get the Domain Description and Append it to Error Message
                            string errorPrefix = "";
                            if (pNewValError.Severity == 1)
                                errorPrefix = "Warning" + " - ";
                            else
                                errorPrefix = "Error" + " - ";

                            //Add the prefix if necessary 
                            if (!pListItem.DisplayName.StartsWith(errorPrefix))
                                pNewValError.ErrorMessage = errorPrefix + pListItem.DisplayName;
                            else
                                pNewValError.ErrorMessage = pListItem.DisplayName;

                            pPGEList.Add((ID8ListItem)pNewValError);
                        }
                        else
                            Debug.Print("screened out ESRI internal error");
                    }

                    if (pListItem is ID8List)
                    {
                        //System.Diagnostics.Debug.Print(pListItem.DisplayName); 
                        ID8List pChildD8List = (ID8List)pListItem;
                        //Recursive call here 
                        GetModifiedList(pChildD8List, ref pPGEList, ruleName);
                    }
                    pListItem = pInitialList.Next();
                }
            }
            catch
            {
                throw new Exception("Error returning the modified list");
            }
        }

        /// <summary>
        /// We want to exclude esri internal errors and also the 
        /// CGC range domain error, since we are including this 
        /// rule as an exception see INC3895203 
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        private bool IsErrorExcluded(string displayName)
        {
            const string internalESRIErrorString = "esri internal error";
            const string cgcHint1 = "cgc";
            const string cgcHint2 = " of range domain";
            bool isExcluded = false; 

            try
            {
                if (displayName.ToLower().Contains(internalESRIErrorString))
                    isExcluded = true;
                else if (displayName.ToLower().Contains(cgcHint1) &&
                    displayName.ToLower().Contains(cgcHint2))
                    isExcluded = true;

                return isExcluded; 
            }
            catch
            {
                return false; 
            }
        }

        private Hashtable LoadValidationRulesForObjectClass(IObjectClass pObjectClass, bool cacheAllValidationRules)
        {
            //IExtensionManager extensionMan = null;
            IMMConfigTopLevel configTopLevel = null;

            try
            {
                //extensionMan = Activator.CreateInstance(Type.GetTypeFromProgID(
                //        "esriSystem.ExtensionManager")) as IExtensionManager;
                configTopLevel = Miner.Geodatabase.ConfigTopLevel.Instance;

                if (cacheAllValidationRules)
                {
                    //Get list of feature dataset and all feature classes within those to cache all validation rules.
                    IWorkspace ws = ((IDataset)pObjectClass).Workspace;
                    IEnumDataset featureDatasets = ws.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureDataset);
                    featureDatasets.Reset();
                    IDataset ds = null;
                    while((ds = featureDatasets.Next()) != null)
                    {
                        IFeatureClassContainer featDS = ds as IFeatureClassContainer;
                        for (int i = 0; i < featDS.ClassCount; i++)
                        {
                            IFeatureClass featClass = featDS.get_Class(i);
                            if (!_hshAllQAQCRules.ContainsKey(((IDataset)featClass).BrowseName.ToUpper()))
                            {
                                _hshAllQAQCRules.Add(((IDataset)featClass).BrowseName.ToUpper(), GetQAQCRules(featClass));
                            }
                        }

                        IRelationshipClassContainer relClasses = ds as IRelationshipClassContainer;
                        IEnumRelationshipClass relClassesEnum = relClasses.RelationshipClasses;
                        relClassesEnum.Reset();
                        while ((ds = relClassesEnum.Next() as IDataset) != null)
                        {
                            ITable table = ds as ITable;
                            if (table == null) { continue; }
                            if (!_hshAllQAQCRules.ContainsKey(ds.BrowseName.ToUpper()))
                            {
                                _hshAllQAQCRules.Add(ds.BrowseName.ToUpper(), GetQAQCRules((IObjectClass)table));
                            }
                        }
                    }

                    IEnumDataset featureClasses = ws.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTFeatureClass);
                    featureClasses.Reset();
                    while((ds = featureClasses.Next()) != null)
                    {
                        IFeatureClass featClass = ds as IFeatureClass;
                        if (!_hshAllQAQCRules.ContainsKey(ds.BrowseName.ToUpper()))
                        {
                            _hshAllQAQCRules.Add(ds.BrowseName.ToUpper(), GetQAQCRules(featClass));
                        }
                    }

                    IEnumDataset tables = ws.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTTable);
                    tables.Reset();
                    while ((ds = tables.Next()) != null)
                    {
                        ITable table = ds as ITable;
                        if (!_hshAllQAQCRules.ContainsKey(ds.BrowseName.ToUpper()))
                        {
                            _hshAllQAQCRules.Add(ds.BrowseName.ToUpper(), GetQAQCRules((IObjectClass)table));
                        }
                    }

                    IEnumDataset relationships = ws.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTRelationshipClass);
                    relationships.Reset();
                    while ((ds = relationships.Next()) != null)
                    {
                        ITable table = ds as ITable;
                        if (table == null) { continue; }
                        if (!_hshAllQAQCRules.ContainsKey(ds.BrowseName.ToUpper()))
                        {
                            _hshAllQAQCRules.Add(ds.BrowseName.ToUpper(), GetQAQCRules((IObjectClass)table));
                        }
                    }

                    return null;
                }
                else
                {
                    return GetQAQCRules(pObjectClass);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Error returning rules for passed IObjectClass: " +
                    ex.Message);
            }
            finally
            {
            }
        }

        private Hashtable GetQAQCRules(IObjectClass pObjectClass)
        {
            Hashtable hshQAQCRules = new Hashtable();
            IDataset pDS = (IDataset)pObjectClass;
            Hashtable hshRules = new Hashtable();
            UID pUID = null;
            string datasetName = pDS.BrowseName.ToUpper();
            ISubtypes pSubtypes = pObjectClass as ISubtypes;

            if (pObjectClass != null && pSubtypes != null)
            {
                IMMFeatureClass pMMFC = Miner.Geodatabase.ConfigTopLevel.Instance.GetFeatureClassOnly(pObjectClass);
                ID8List pFCList = (ID8List)pMMFC;
                pFCList.Reset();
                ID8ListItem pFCListItem = pFCList.Next(false);

                while (pFCListItem != null)
                {
                    if (pFCListItem.ItemType == mmd8ItemType.mmitSubtype)
                    {
                        //Get the MMSubtype 
                        IMMSubtype pSubType = (IMMSubtype)pFCListItem;

                        //Loop through the subtypes 
                        ID8List pList = (ID8List)pFCListItem;
                        pList.Reset();
                        ID8ListItem pListItem = pList.Next(false);
                        while (pListItem != null)
                        {
                            if (pListItem.DisplayName == "Validation Rules")
                            {
                                if (pListItem.ItemType == mmd8ItemType.mmitAutoValue)
                                {
                                    IMMAutoValue pAutoValue = (IMMAutoValue)pListItem;
                                    if (pAutoValue.AutoGenID != null)
                                    {
                                        pUID = pAutoValue.AutoGenID;
                                        string key = datasetName;
                                        if (pSubtypes.HasSubtype)
                                            key += "*" + pSubType.SubtypeCode.ToString();
                                        if (!hshQAQCRules.ContainsKey(key))
                                            hshQAQCRules.Add(key, new Hashtable());

                                        //Add the rule for this subtype 
                                        hshRules = (Hashtable)hshQAQCRules[key];
                                        if (!hshRules.ContainsKey(pUID.Value))
                                            hshRules.Add(pUID.Value, "");
                                        hshQAQCRules[key] = hshRules;
                                    }
                                }
                            }
                            pListItem = pList.Next(false);
                        }
                    }
                    pFCListItem = pFCList.Next();
                }
            }
            return hshQAQCRules;
        }

        public int GetFCImageIndex(IObjectClass pOC)
        {
            int defaultImageIdx = 0;

            try
            {
                int imageIdx = defaultImageIdx;
                IDataset pDS = (IDataset)pOC;
                esriDatasetType pType = pDS.Type;
                if (pType != esriDatasetType.esriDTFeatureClass)
                {
                    //Assume a table 
                    imageIdx = 3;
                }
                else
                {
                    IFeatureClass pFC = (IFeatureClass)pDS;
                    if (pFC.FeatureType == esriFeatureType.esriFTAnnotation)
                    {
                        imageIdx = 4;
                    }
                    else
                    {
                        if (pFC.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                        {
                            imageIdx = 0;
                        }
                        else if (pFC.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                        {
                            imageIdx = 1;
                        }
                        else if (pFC.ShapeType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                        {
                            imageIdx = 2;
                        }
                    }
                }
                return imageIdx;
            }
            catch
            {
                return defaultImageIdx;
            }
        }

        #endregion Public
    }

    public class PGEError : IComparable, IXmlSerializable
    {
        //Property storage variables 
        private string m_datasetName;
        private int m_severity;
        private int m_ObjectId;
        private int m_Type;
        private string m_errorMsg;
        private string m_ruleName;
        private PGEDifferenceType m_differenceType;
        public static List<string> _errorForceClassList = new List<string>();

        public PGEError()
        {

        }

        public PGEError(
            PGEDifferenceType pDiffType,
            string datasetName,
            int severity,
            int objectId,
            int type,
            string errorMsg,
            string errorName)
        {
            m_differenceType = pDiffType;
            m_datasetName = datasetName;
            m_severity = severity;
            m_ObjectId = objectId;
            m_Type = type;
            m_errorMsg = errorMsg;
            m_ruleName = errorName;

            try
            {
                if(string.IsNullOrEmpty(m_ruleName) || string.IsNullOrEmpty(m_datasetName)) return;
                if (_errorForceClassList.Count < 1) return;
                string sFcName = null;
                for (int i = 0; i < _errorForceClassList.Count; i++)
                {
                    sFcName = _errorForceClassList[i];
                    if (!string.IsNullOrEmpty(sFcName))
                    {
                        //Logic specific to Substation-PNode requirement.
                        if (m_datasetName.ToUpper() == sFcName.ToUpper())
                        {
                            if (m_ruleName.ToUpper().Contains(SchemaInfo.General.ESRI_RULES))
                            {
                                //if class has model name applied, and rule is ESRI Rules, then change sevirity to 0, which is an error.
                                m_severity = 0;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {};//ignore error, if any.
        }

        //IComparable implementation (for sort order) 
        //Sort Dataset and then OId  
        int stringCompare = -1;
        int IComparable.CompareTo(object obj)
        {
            PGEError temp =
                (PGEError)obj;
            if (this.Dataset != temp.Dataset)
                stringCompare = string.Compare(
                this.Dataset, temp.Dataset);
            else
            {
                if (this.ObjectId > temp.ObjectId)
                    stringCompare = 1;
                else
                    stringCompare = 0;
            }
            return stringCompare;
        }

        public PGEDifferenceType DifferenceType
        {
            get { return m_differenceType; }
        }
        public string Dataset
        {
            get { return m_datasetName; }
        }
        public int ObjectId
        {
            get { return m_ObjectId; }
        }
        public int Severity
        {
            get { return m_severity; }
        }
        public string ErrorMsg
        {
            get { return m_errorMsg; }
        }
        public int Type
        {
            get { return m_Type; }
        }
        public string RuleName
        {
            get { return m_ruleName; }
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            m_differenceType = (PGEDifferenceType)Int32.Parse(reader["DifferenceType"]);
            m_datasetName = reader["DatasetName"];
            m_severity = Int32.Parse(reader["Severity"]);
            m_ObjectId = Int32.Parse(reader["ObjectID"]);
            m_Type = Int32.Parse(reader["ErrorType"]);
            m_errorMsg = reader["ErrorMessage"];
            m_ruleName = reader["RuleName"];
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("DifferenceType", ((int)m_differenceType).ToString());
            writer.WriteAttributeString("DatasetName", m_datasetName);
            writer.WriteAttributeString("Severity", m_severity.ToString());
            writer.WriteAttributeString("ObjectID", m_ObjectId.ToString());
            writer.WriteAttributeString("ErrorType", m_Type.ToString());
            writer.WriteAttributeString("ErrorMessage", m_errorMsg);
            writer.WriteAttributeString("RuleName", m_ruleName);
        }
        public static void FeatureClassList_ForceErrors(IWorkspace ws, Miner.Geodatabase.GeodatabaseManager.Logging.INameLog gdbmLog = null)
        {
            try
            {
                //Get names of feature classes in a list, that have 'ESRI_Rules_Severity_Error' model name applied to them.
                if (_errorForceClassList.Count > 0) return;
                Miner.Interop.IMMModelNameManager iModelNameManager = Miner.Geodatabase.ModelNameManager.Instance;
                //IEnumBSTR listFeatureClasses = iModelNameManager.ClassNamesFromModelNameWS(Miner.Geodatabase.Edit.Editor.EditWorkspace, SchemaInfo.Electric.ClassModelNames.ESRI_Rules_Severity_Error);
                IEnumBSTR listFeatureClasses = iModelNameManager.ClassNamesFromModelNameWS(ws, SchemaInfo.Electric.ClassModelNames.ESRI_Rules_Severity_Error);
                listFeatureClasses.Reset();

                string sFCName = listFeatureClasses.Next();
                //if (string.IsNullOrEmpty(sFCName2))
                //{
                //    //During this functionality being called from GDBM, different workspace is being used. Hence, 
                //    //use a differnt workspace to get acces to feature classes names.. 
                //    listFeatureClasses = iModelNameManager.ClassNamesFromModelNameWS(ValidationEngine.QAQCWorkspace, SchemaInfo.Electric.ClassModelNames.ESRI_Rules_Severity_Error);
                //    listFeatureClasses.Reset();
                //    sFCName = listFeatureClasses.Next();
                //}

                while (sFCName != null)
                {
                    _errorForceClassList.Add(sFCName);
                    sFCName = listFeatureClasses.Next();
                }
                if (gdbmLog != null)
                {
                    gdbmLog.Info("[Substation Requirement]: List of feature class to display 'WARNINGS' as 'ERRORS', count: " + _errorForceClassList.Count.ToString());
                    //gdbmLog.Info("Class list count to force Error instead of warnings: " + _errorForceClassList.Count.ToString());
                }
            }
            catch (Exception ex) 
            {
                if (gdbmLog != null)
                {
                    gdbmLog.Error("[Substation Requirement]- Failed to get feature class names to display warnings as errors.");
                }
            };//ignore error, if any.
        }
    }

    public class FeederManagerDiffObject
    {
        //Property storage variables 
        private IObject m_Object;
        private string m_datasetName;
        private int m_oid;

        public FeederManagerDiffObject(
            IObject pObject,
            string datasetName,
            int oid)
        {
            m_Object = pObject;
            m_datasetName = datasetName;
            m_oid = oid;
        }

        public IObject Object
        {
            get { return m_Object; }
            set { m_Object = value; }
        }
        public string DatasetName
        {
            get { return m_datasetName; }
            set { m_datasetName = value; }
        }
        public int OID
        {
            get { return m_oid; }
            set { m_oid = value; }
        }
    }

    public class VersionDiffObject
    {
        //Property storage variables 
        private esriDifferenceType m_diffType;
        private IObject m_Object;
        private string m_datasetName;
        private int m_oid; 

        public VersionDiffObject(
            esriDifferenceType diffType,
            IObject pObject, 
            string datasetName, 
            int oid)
        {
            m_diffType = diffType;
            m_Object = pObject;
            m_datasetName = datasetName;
            m_oid = oid; 
        }

        public esriDifferenceType DifferenceType
        {
            get { return m_diffType; }
        }
        public IObject Object
        {
            get { return m_Object; }
            set { m_Object = value; } 
        }
        public string DatasetName
        {
            get { return m_datasetName; }
            set { m_datasetName = value; }
        }
        public int OID
        {
            get { return m_oid; }
            set { m_oid = value; }
        }
    }

    public class DeletedObject
    {
        //Property storage variables 
        private string m_datasetName;
        private int m_oid;

        public DeletedObject(
            string datasetName,
            int oid)
        {
            m_datasetName = datasetName;
            m_oid = oid;
        }

        public string DatasetName
        {
            get { return m_datasetName; }
        }
        public int OID
        {
            get { return m_oid; }
        }
    }

    public class AdminCache
    {
        //Property storage variables
        private Hashtable _hshRegionalAtts;
        private IPolygon _adminIntersect;

        public AdminCache(
            Hashtable hshRegionalAtts,
            IPolygon adminIntersect)
        {
            _hshRegionalAtts = hshRegionalAtts;
            _adminIntersect = adminIntersect;
        }

        public Hashtable RegionalAttributes
        {
            get { return _hshRegionalAtts; }
        }

        public IPolygon AdminIntersect
        {
            get { return _adminIntersect; }
        }
    }
    
}
