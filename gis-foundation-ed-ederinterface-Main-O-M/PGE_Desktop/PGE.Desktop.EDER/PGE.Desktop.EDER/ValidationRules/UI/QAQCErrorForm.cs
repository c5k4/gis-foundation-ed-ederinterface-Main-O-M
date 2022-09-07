using System;
using System.Collections; 
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Reflection;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Framework;

using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Systems;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.ValidationRules.UI
{
    /// <summary>
    /// A form that allows user to edit Symbol number configuration documents stored in the database.
    /// </summary>
    public partial class QAQCErrorForm : Form
    {

        #region Constructor
        private const int maxDisplayRecords = 1000;
        ContextMenuStrip _treeContextMenu = null;
        private Hashtable _hshFCs = new Hashtable();
        private IApplication _application = null;
        private IWorkspace _pWS = null;
        
        /// <summary>
        /// Form to show user the QAQC rules of severity: Error that 
        /// have been broken in the session 
        /// </summary>
        /// <param name="workspace">The workspace.</param>
        public QAQCErrorForm( 
            Hashtable hshErrors, 
            IApplication pApp, 
            IWorkspace pWS)
        {
            _hshErrors = hshErrors; 
            InitializeComponent();
            _application = pApp;
            _pWS = pWS; 
        }
        #endregion Constructor

        #region Private
        /// <summary>
        /// logger to log all the information, warning, and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Hashtable of errors from validation  
        /// </summary>
        Hashtable _hshErrors; 
                
        /// <summary>
        /// Handles the Load event of the ValidationErrorForm control.
        /// 
        /// Clear the UI and load feature class names.  
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ValidationErrorForm_Load(object sender, EventArgs e)
        {
            try
            {
                _logger.Debug("Loading list of errors");
                ValidationEngine.Instance.LoadResults(true, tvwResults, _hshErrors);

                string captionMsg = "";
                
                /*
                if (_hshErrors.Count > maxDisplayRecords)
                {
                    captionMsg =
                        "Found [" + _hshErrors.Count.ToString() +
                        "] QAQC errors. Displaying the first [" +
                        maxDisplayRecords.ToString() +
                        "] only.";
                }
                else
                {
                */
                    captionMsg =
                        "Found [" + _hshErrors.Count.ToString() +
                        "] QAQC errors.";
                //}

                this.validateRuleGroupBox.Text = captionMsg; 

            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message); 
            }
        }


        #endregion Private

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                if (_hshErrors != null) 
                {
                    _hshErrors.Clear();
                    _hshErrors = null; 
                }
                this.Close(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        /// <summary>
        /// Context menu clicked event, either Zoom To or Flash the feature 
        /// represented by the treeview node 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

                if (_hshFCs.ContainsKey(datasetName))
                    pFC = (IFeatureClass)_hshFCs[datasetName];
                else
                {
                    IFeatureWorkspace pFWS = (IFeatureWorkspace)_pWS;
                    pFC = pFWS.OpenFeatureClass(datasetName);
                    _hshFCs.Add(datasetName, pFC);
                }

                pFeature = (IFeature)GetFeature(pFC, oid);
                if (pFeature != null)
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
    }
}
