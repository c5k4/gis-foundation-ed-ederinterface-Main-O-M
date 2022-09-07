using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;

namespace PGE.Desktop.EDER.ArcMapCommands.VersionDifference
{
    public partial class VersionChangesForm : Form
    {
        Dictionary<string, List<int>> InsertedOIDs = new Dictionary<string, List<int>>();
        Dictionary<string, List<int>> UpdatedOIDs = new Dictionary<string, List<int>>();
        Dictionary<string, List<int>> DeletedOIDs = new Dictionary<string, List<int>>();
        Dictionary<string, List<string>> ArcFMFieldOrderMapping = new Dictionary<string, List<string>>();
        string editType = "";
        int totalInserts = 0;
        int totalUpdates = 0;
        int totalDeletes = 0;
        
        IFeatureWorkspace ParentWorkspace, ChildWorkspace;
        Dictionary<string, VersionChangesRow> gridViewDataSource = new Dictionary<string, VersionChangesRow>();
        List<VersionChangesRow> SortedGridViewDataSource = new List<VersionChangesRow>();

        public VersionChangesForm(ID8List changesList, IWorkspace parentWorkspace, IWorkspace childWorkspace)
        {
            InitializeComponent();

            this.ParentWorkspace = parentWorkspace as IFeatureWorkspace;
            this.ChildWorkspace = childWorkspace as IFeatureWorkspace;

            PopulateTree(changesList);

            treeChanges.AfterSelect += new TreeViewEventHandler(treeChanges_AfterSelect);
        }

        /// <summary>
        /// When the changes dialog is resized, the tree view and grid should resize as well.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            treeChanges.Size = new Size(treeChanges.Width, this.Height - 60);
            gridViewChanges.Size = new Size(this.Width - treeChanges.Width - 40, this.Height - 60);
            gridViewChanges.Location = new Point(18 + treeChanges.Width, gridViewChanges.Location.Y);
        }

        /// <summary>
        /// When the grid has a node selected referring to a feature class then the grid view needs
        /// to be populated with the row information of the child and parent versions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void treeChanges_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ((NodeType)e.Node.Tag == NodeType.ObjectID)
            {
                gridViewDataSource.Clear();
                SortedGridViewDataSource.Clear();
                gridViewChanges.DataSource = null;
                int oid = Int32.Parse(e.Node.Text);
                string tableName = e.Node.Parent.Text;
                PopulateDataGridView(tableName, oid);
                SortToArcFMFieldOrder(tableName, oid);
                gridViewChanges.DataSource = SortedGridViewDataSource;
            }
            else
            {
                gridViewChanges.DataSource = null;
                gridViewDataSource.Clear();
                SortedGridViewDataSource.Clear();
            }
        }

        /// <summary>
        /// Populate the grid view data source in the correct ArcFM field order.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="oid"></param>
        private void SortToArcFMFieldOrder(string tableName, int oid)
        {
            ITable childTable = null;
            IRow childRow = null;

            try
            {
                childTable = ChildWorkspace.OpenTable(tableName);
                childRow = childTable.GetRow(oid);
                List<string> fieldOrder = getObjectClassFieldOrder(childTable as IObjectClass, childRow);

                //Sort the list that will be used as the data source for the grid view
                //so that it is sorted off of the ArcFM field order.
                foreach (string fieldName in fieldOrder)
                {
                    SortedGridViewDataSource.Add(gridViewDataSource[fieldName]);
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (childTable != null) { while (Marshal.ReleaseComObject(childTable) > 0) { } }
                if (childRow != null) { while (Marshal.ReleaseComObject(childRow) > 0) { } }
            }
        }

        /// <summary>
        /// Obtains the field order for the given object class for the subtype of the row provided.
        /// </summary>
        /// <param name="objClass"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        private List<string> getObjectClassFieldOrder(IObjectClass objClass, IRow row)
        {
            List<string> fieldOrder = new List<string>();
            ISubtypes subTypes = objClass as ISubtypes;
            IEnumSubtype enumSubtype = subTypes.Subtypes;
            int subtypeCodeToGet = -1;

            if (subTypes.HasSubtype)
            {
                subtypeCodeToGet = Int32.Parse(row.get_Value(subTypes.SubtypeFieldIndex).ToString());
            }

            string fieldOrderMappingName = ((IDataset)objClass).BrowseName + "-" + subtypeCodeToGet;
            if (ArcFMFieldOrderMapping.ContainsKey(fieldOrderMappingName))
            {
                fieldOrder = ArcFMFieldOrderMapping[fieldOrderMappingName];
                return fieldOrder;
            }

            //string subtypeString = "All";
            int subtypeCode = -1;

            IMMSubtype subtype = Miner.Geodatabase.ConfigTopLevel.Instance.GetSubtypeByID(objClass, subtypeCode, true);
            string nextSubtype = "All";
            //For each subtype of this Object, get the associated AUs
            while (subtype != null)
            {
                if (subtypeCode == -1 && -1 == subtypeCodeToGet)
                {
                    //Get the subtype ID8List and recurse the list to get all the autoupdaters for this subtype
                    ID8List subtypeList = subtype as ID8List;
                    getSubtypeFieldOrder(subtypeList, ref fieldOrder);
                    ArcFMFieldOrderMapping.Add(fieldOrderMappingName, fieldOrder);
                }
                else if (subtypeCodeToGet == subtypeCode)
                {
                    ISubtypes subtypes = objClass as ISubtypes;
                    if (subtypes.HasSubtype)
                    {
                        //Get the subtype ID8List and recurse the list to get all the autoupdaters for this subtype
                        ID8List subtypeList = subtype as ID8List;
                        getSubtypeFieldOrder(subtypeList, ref fieldOrder);
                        ArcFMFieldOrderMapping.Add(fieldOrderMappingName, fieldOrder);
                    }
                }
                nextSubtype = enumSubtype.Next(out subtypeCode);
                if (nextSubtype != null)
                {
                    subtype = Miner.Geodatabase.ConfigTopLevel.Instance.GetSubtypeByID(objClass, subtypeCode, true);
                }
                else
                {
                    subtype = null;
                }
            }
            return fieldOrder;
        }

        /// <summary>
        /// This method will determine the field order specified by the ArcFM field properties.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="FieldName"></param>
        /// <param name="subTypeCode"></param>
        /// <param name="subtypeName"></param>
        private void getSubtypeFieldOrder(ID8List list, ref List<string> fieldOrder)
        {
            list.Reset();
            ID8ListItem listItem = list.Next(false);
            while (listItem != null)
            {
                if (listItem.ItemType == mmd8ItemType.mmitField)
                {
                    IMMField field = (IMMField)listItem;
                    fieldOrder.Add(field.FieldName);
                }
                listItem = list.Next(false);
            }
        }

        /// <summary>
        /// This method will populate the grid view with the information from the given table / OID for
        /// both the parent and child versions of the row.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="oid"></param>
        private void PopulateDataGridView(string tableName, int oid)
        {
            ITable parentTable = null;
            ITable childTable = null;
            IRow parentRow = null;
            IRow childRow = null;
            try
            {
                parentTable = ParentWorkspace.OpenTable(tableName);
                childTable = ChildWorkspace.OpenTable(tableName);

                parentRow = parentTable.GetRow(oid);
                childRow = childTable.GetRow(oid);

                for (int i = 0; i < parentTable.Fields.FieldCount; i++)
                {
                    VersionChangesRow ChangesRow = new VersionChangesRow();
                    ChangesRow.FieldName = parentTable.Fields.get_Field(i).Name;
                    if (childRow != null) { ChangesRow.ChildValue = childRow.get_Value(i).ToString(); }
                    else { ChangesRow.ChildValue = "<Null>"; }
                    ChangesRow.ParentValue = parentRow.get_Value(i).ToString();
                    gridViewDataSource.Add(ChangesRow.FieldName, ChangesRow);
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (parentTable != null) { while (Marshal.ReleaseComObject(parentTable) > 0);}
                if (childTable != null) { while (Marshal.ReleaseComObject(childTable) > 0);}
                if (parentRow != null) { while (Marshal.ReleaseComObject(parentRow) > 0);}
                if (childRow != null) { while (Marshal.ReleaseComObject(childRow) > 0);}
            }
        }

        /// <summary>
        /// This method will populate our tree of all version changes for the D8List provided.
        /// </summary>
        /// <param name="changesList"></param>
        private void PopulateTree(ID8List changesList)
        {
            //First parse through our d8list to populate our dictionaries
            //full of the current changes
            changesList.Reset();
            ID8ListItem item = changesList.Next(true);
            while (item != null)
            {
                string displayName = item.DisplayName.ToUpper();
                if (displayName.Contains("INSERTED IN SOURCE"))
                {
                    editType = "INSERT";
                    populateDictionary(ref InsertedOIDs, item as ID8List);
                }
                else if (displayName.Contains("UPDATED IN SOURCE"))
                {
                    editType = "UPDATE";
                    populateDictionary(ref UpdatedOIDs, item as ID8List);
                }
                else if (displayName.Contains("DELETED IN SOURCE"))
                {
                    editType = "DELETE";
                    populateDictionary(ref DeletedOIDs, item as ID8List);
                }
                item = changesList.Next(true);
            }

            //Now populate the Tree list for user view
            TreeNode changesNode = new TreeNode("Changes(" + (totalInserts + totalUpdates + totalDeletes) + ")");
            changesNode.Tag = NodeType.TopLevel;
            treeChanges.Nodes.Add(changesNode);

            //Inserts
            TreeNode insertNode = new TreeNode("Inserts(" + totalInserts + ")");
            insertNode.Tag = NodeType.EditType;
            foreach (KeyValuePair<string, List<int>> kvp in InsertedOIDs)
            {
                TreeNode newTableNode = new TreeNode(kvp.Key);
                newTableNode.Tag = NodeType.Table;
                foreach (int oid in kvp.Value)
                {
                    TreeNode oidNode = new TreeNode(oid.ToString());
                    oidNode.Tag = NodeType.ObjectID;
                    newTableNode.Nodes.Add(oidNode);
                }
                insertNode.Nodes.Add(newTableNode);
            }

            //Updates
            TreeNode updateNode = new TreeNode("Updates(" + totalUpdates + ")");
            updateNode.Tag = NodeType.EditType;
            foreach (KeyValuePair<string, List<int>> kvp in UpdatedOIDs)
            {
                TreeNode newTableNode = new TreeNode(kvp.Key);
                newTableNode.Tag = NodeType.Table;
                foreach (int oid in kvp.Value)
                {
                    TreeNode oidNode = new TreeNode(oid.ToString());
                    oidNode.Tag = NodeType.ObjectID;
                    newTableNode.Nodes.Add(oidNode);
                }
                updateNode.Nodes.Add(newTableNode);
            }

            //Deletes
            TreeNode deleteNode = new TreeNode("Deletes(" + totalDeletes + ")");
            deleteNode.Tag = NodeType.EditType;
            foreach (KeyValuePair<string, List<int>> kvp in DeletedOIDs)
            {
                TreeNode newTableNode = new TreeNode(kvp.Key);
                newTableNode.Tag = NodeType.Table;
                foreach (int oid in kvp.Value)
                {
                    TreeNode oidNode = new TreeNode(oid.ToString());
                    oidNode.Tag = NodeType.ObjectID;
                    newTableNode.Nodes.Add(oidNode);
                }
                deleteNode.Nodes.Add(newTableNode);
            }

            treeChanges.Nodes[0].Nodes.Add(insertNode);
            treeChanges.Nodes[0].Nodes.Add(updateNode);
            treeChanges.Nodes[0].Nodes.Add(deleteNode);
        }

        /// <summary>
        /// Populates the oid dictionary with a list of all OIDs 
        /// </summary>
        /// <param name="oidDictionary"></param>
        /// <param name="changes"></param>
        private void populateDictionary(ref Dictionary<string, List<int>> oidDictionary, ID8List changes)
        {
            changes.Reset();
            ID8ListItem item = changes.Next(true);
            while (item != null)
            {
                if (item is ID8Table)
                {
                    string tableName = "";
                    List<int> oidList = new List<int>();
                    populateOIDsInList(ref oidList, item as ID8List, ref tableName);
                    oidDictionary.Add(tableName, oidList);
                }
                item = changes.Next(true);
            }
        }

        private void populateOIDsInList(ref List<int> oidLIst, ID8List changes, ref string TableName)
        {
            changes.Reset();
            ID8ListItem item = changes.Next(true);
            while (item != null)
            {
                if (item is ID8GeoAssoc)
                {
                    ID8GeoAssoc geoAssoc = item as ID8GeoAssoc;
                    if (TableName == "") { TableName = ((IDataset)geoAssoc.AssociatedGeoRow.Table).BrowseName; }
                    oidLIst.Add(geoAssoc.AssociatedGeoRow.OID);
                    if (editType == "INSERT") { totalInserts++; }
                    else if (editType == "UPDATE") { totalUpdates++; }
                    else if (editType == "DELETE") { totalDeletes++; }
                }
                item = changes.Next(true);
            }
        }

        enum NodeType
        {
            TopLevel,
            EditType,
            Table,
            ObjectID
        }
    }

    public class VersionChangesRow
    {
        public string FieldName { get; set; }
        public string ChildValue { get; set; }
        public string ParentValue { get; set; }
    }
}
