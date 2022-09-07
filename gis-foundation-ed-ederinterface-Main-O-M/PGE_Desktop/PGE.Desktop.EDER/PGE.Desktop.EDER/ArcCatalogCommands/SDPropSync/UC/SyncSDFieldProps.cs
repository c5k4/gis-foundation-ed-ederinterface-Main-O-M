using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.Interop;
//using Miner.Controls;

namespace PGE.Desktop.EDER.ArcCatalogCommands.SDPropSync.UC
{
    public partial class Re_Order_Fields : Form
    {
        IList<IMMStoredDisplayName> _listSystemStoredDisplayName = null;
        IList<IMMStoredDisplayName> _listUserStoredDisplayName = null;
        //Indicate whether treeview node checked in process or not.
        Boolean isProcessing = false;
        #region Constructor
        public Re_Order_Fields(IList<IMMStoredDisplayName> listSystemStoredDisplayName, IList<IMMStoredDisplayName> listUserStoredDisplayName)
        {
            
            InitializeComponent();
            // checking if system and user stored display is null or not available.
            if (listSystemStoredDisplayName == null && listUserStoredDisplayName == null)
            {
                this.Hide();
            }
            else if (listSystemStoredDisplayName.Count < 1 && listUserStoredDisplayName.Count < 1)
            {
                this.Hide();
            }
           _listSystemStoredDisplayName =  listSystemStoredDisplayName;
           _listUserStoredDisplayName = listUserStoredDisplayName;

            // populating the treeview 
            populateTreeView();

        }
        #endregion

        #region private method
        /// <summary>
        /// This method will populate the treeview based on the system and user stored display
        /// </summary>
        private void populateTreeView()
        {
            Func<IList<IMMStoredDisplayName>,string, TreeNode>
                GetTreeNode = (listStoredDisplayName,name) =>
                    {
                        TreeNode tn = new TreeNode(name);
                        foreach (IMMStoredDisplayName storedDisplayName in listStoredDisplayName)
                        {
                            TreeNode tNode = new TreeNode(storedDisplayName.Name);
                            tNode.Tag = storedDisplayName;
                            tn.Nodes.Add(tNode);
                        }
                        return tn;
                    };

            TreeNode systemTreeNode = GetTreeNode(_listSystemStoredDisplayName, "System");
            TreeNode userTreeNode = GetTreeNode(_listUserStoredDisplayName, "User");
            
            tvStoredDisplay.Nodes.Add(systemTreeNode);
            tvStoredDisplay.Nodes.Add(userTreeNode);
        }

        /// <summary>
        /// Check or uncheck all the node in the trNodeCollection based on isChecked.
        /// </summary>
        /// <param name="trNodeCollection">Node Collection</param>
        /// <param name="isChecked">true/false</param>
        private void checkUncheck(TreeNodeCollection trNodeCollection, Boolean isChecked)
        {
            foreach (TreeNode trNode in trNodeCollection)
            {
                trNode.Checked = isChecked;
                if (trNode.Nodes.Count > 0)
                    checkUncheck(trNode.Nodes, isChecked);
            }


        }

        /// <summary>
        /// This function will return the selected stored display based on the requested mmStoredDisplayTYpe
        /// </summary>
        /// <param name="storedDisplayType">Type of storedDisplay.</param>
        /// <returns>List of selected stored display.</returns>
        internal protected List<IMMStoredDisplayName> GetSelectedStoredDisplay(mmStoredDisplayType storedDisplayType)
        {
            List<IMMStoredDisplayName> listSelected = new List<IMMStoredDisplayName>();
            // TvStoreDisplay.Nodes[0] will be always system node which contains all the system stored display name.
            TreeNodeCollection nodes = tvStoredDisplay.Nodes[0].Nodes;
            if (storedDisplayType != mmStoredDisplayType.mmSDTSystem)
            {
                nodes = tvStoredDisplay.Nodes[1].Nodes;
            }

            if (nodes != null)
            {
                foreach (System.Windows.Forms.TreeNode aNode in nodes)
                {
                    //Checking if node is checked then add into the listSelected.
                    if (aNode.Checked)
                    {
                        listSelected.Add(aNode.Tag as IMMStoredDisplayName);
                    }
                }
            }
            return listSelected;

        }

        /// <summary>
        /// This function will return ture/false based on the all child node of tn checked or not.
        /// </summary>
        /// <param name="tn">A TreeNode</param>
        /// <returns>true = if all child node checked else false</returns>
        private Boolean isAllNodesChecked(TreeNode tn)
        {

            foreach (TreeNode tNode in tn.Nodes)
            {
                if (tNode.Checked == false)
                {
                    return false;
                }
            }
            return true;

        }
        #endregion

        #region control events
        private void btnCancel_Click(object sender, EventArgs e)
        {
            
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Hide();
        }

        
        private void tvStoredDisplay_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (isProcessing == false)
            {
                isProcessing = true;
                if (e.Node.Parent != null)
                {
                    e.Node.Parent.Checked = isAllNodesChecked(e.Node.Parent);
                }
                else
                {
                    checkUncheck(e.Node.Nodes, e.Node.Checked);
                }

                chbSelectClear.Checked = (tvStoredDisplay.Nodes[0].Checked && tvStoredDisplay.Nodes[1].Checked);
                isProcessing = false;
                
            }
        }

        private void chbSelectClear_CheckedChanged(object sender, EventArgs e)
        {
            if (!isProcessing) checkUncheck(tvStoredDisplay.Nodes, chbSelectClear.Checked);
        }
        #endregion
    }
}
