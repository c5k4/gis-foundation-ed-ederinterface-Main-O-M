using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geodatabase;

namespace Telvent.PGE.ED.Desktop.Forms
{
    public partial class PTTCombinePole : Form
    {
        public PTTCombinePole(IFeature supportStructure, IFeature pttSupportStructure, Pole poleToCombine)
        {
            InitializeComponent();

            SupportStructure = supportStructure;
            PTTSupportStructure = pttSupportStructure;
            PoleToCombine = poleToCombine;

            InitializePole();
        }

        private bool _cancelled = true;
        public bool Cancelled { get { return _cancelled; } }
        private bool _cancelledAll = true;
        public bool CancelledAll { get { return _cancelledAll; } }

        private IFeature SupportStructure { get; set; }
        private IFeature PTTSupportStructure { get; set; }
        private Pole PoleToCombine { get; set; }

        private void InitializePole()
        {
            for (int i = 0; i < SupportStructure.Fields.FieldCount; i++)
            {
                IField field = SupportStructure.Fields.get_Field(i);
                int PTTFieldIdx = PTTSupportStructure.Fields.FindField(field.Name);

                if (PTTFieldIdx < 0) { throw new Exception("Unable to find matching field '" + field.Name + "' on staging table '" + ((IDataset)PTTSupportStructure.Table).BrowseName + "'"); }

                //Compare our attributes
                object poleFieldObject = SupportStructure.get_Value(i);
                object stagingFieldObject = PTTSupportStructure.get_Value(PTTFieldIdx);

                if ((poleFieldObject == null && stagingFieldObject == null) || (poleFieldObject == DBNull.Value && stagingFieldObject == DBNull.Value)
                    || ((poleFieldObject != null && stagingFieldObject != null) && poleFieldObject.ToString() == stagingFieldObject.ToString()))
                {
                    //Fields match, do nothing
                }
                else
                {
                    //Fields do not match
                    gridAttributes.Rows.Add();
                    int rowNumber = gridAttributes.Rows.Count -1;
                    gridAttributes[0, rowNumber].Value = field.AliasName;
                    gridAttributes[1, rowNumber].Value = PTTSupportStructure.get_Value(PTTFieldIdx);
                    gridAttributes[2, rowNumber].Value = SupportStructure.get_Value(i);
                    gridAttributes[3, rowNumber].Value = field.Name;

                    if (!field.Editable)
                    {
                        //Set this field with red border, indicating that it is not editable
                        gridAttributes.Rows[rowNumber].DefaultCellStyle.ForeColor = Color.Red;
                        gridAttributes.Rows[rowNumber].DefaultCellStyle.SelectionForeColor = Color.Red;
                        gridAttributes.Rows[rowNumber].DefaultCellStyle.SelectionBackColor = Color.White;
                    }
                    else
                    {
                        gridAttributes[2, rowNumber].Style.BackColor = Color.Green;
                        gridAttributes[2, rowNumber].Style.ForeColor = Color.White;
                        gridAttributes[2, rowNumber].Style.SelectionBackColor = Color.Green;
                        gridAttributes[1, rowNumber].Style.SelectionBackColor = Color.Green;
                        gridAttributes[2, rowNumber].Style.SelectionForeColor = Color.White;
                        gridAttributes[1, rowNumber].Style.SelectionForeColor = Color.White;
                        gridAttributes[0, rowNumber].Style.SelectionBackColor = Color.White;
                        gridAttributes[0, rowNumber].Style.SelectionForeColor = Color.Black;
                    }
                }
            }
        }

        private void gridAttributes_SelectionChanged(object sender, EventArgs e)
        {
            //When the user clicks a cell, it indicates that is the cell to use for copying attributes from. Let's
            //change the color to provide feedback to the user on which cell whill be used
            if (gridAttributes.SelectedCells.Count > 0)
            {
                int columnIndex = gridAttributes.SelectedCells[0].ColumnIndex;
                int rowIndex = gridAttributes.SelectedCells[0].RowIndex;
                UpdateCellColors(columnIndex, rowIndex, gridAttributes.Rows[rowIndex].DefaultCellStyle.SelectionForeColor);
            }
        }

        private void UpdateCellColors(int columnIndex, int rowIndex, Color SelectionForeColor)
        {
            if (SelectionForeColor != Color.Red)
            {
                if (columnIndex == 1)
                {
                    gridAttributes[1, rowIndex].Style.BackColor = Color.Green;
                    gridAttributes[1, rowIndex].Style.ForeColor = Color.White;
                    gridAttributes[2, rowIndex].Style.BackColor = Color.White;
                    gridAttributes[2, rowIndex].Style.ForeColor = Color.Black;
                }
                else if (columnIndex == 2)
                {
                    gridAttributes[2, rowIndex].Style.BackColor = Color.Green;
                    gridAttributes[2, rowIndex].Style.ForeColor = Color.White;
                    gridAttributes[1, rowIndex].Style.BackColor = Color.White;
                    gridAttributes[1, rowIndex].Style.ForeColor = Color.Black;
                }
            }
        }

        private void btnCombineToStaging_Click(object sender, EventArgs e)
        {
            _cancelled = false;
            _cancelledAll = false;

            try
            {
                //Get the retire GUID field.
                int retireGuidIdx = SupportStructure.Fields.FindField("REPLACEGUID");
                int stagingGlobalIDIdx = PTTSupportStructure.Fields.FindField("GLOBALID");
                int existingGlobalIDIdx = PTTSupportStructure.Fields.FindField("GLOBALID");
                object stagingPoleGuid = PTTSupportStructure.get_Value(stagingGlobalIDIdx);
                object existingPoleGuid = SupportStructure.get_Value(existingGlobalIDIdx);

                //Combine the attributes onto the Support Structure feature
                CombineAttributes(PTTSupportStructure);

                //Update retire guid to the global ID of the PTT Support Structure
                SupportStructure.set_Value(retireGuidIdx, stagingPoleGuid);
                SupportStructure.Store();

                //Delete the existing structure
                SupportStructure.Delete();

                //Promote the staging pole
                PromoteCombinePole(PTTSupportStructure);

                //Delete the staging structure.  The SAPEquipID needs to be set to 0 first to 
                //let ED06 know that it shouldn't do anything with this deleted pole
                int sapEquipIDIdx = PTTSupportStructure.Table.FindField("SAPEQUIPID");
                PTTSupportStructure.set_Value(sapEquipIDIdx, "-1");
                PTTSupportStructure.Delete();
            }
            catch (Exception ex) { throw new Exception("Failed to combine to existing pole: " + ex.Message); }
        }

        /// <summary>
        /// Promote the new pole
        /// </summary>
        /// <param name="StagingPole"></param>
        private void PromoteCombinePole(IFeature StagingPole)
        {
            try
            {
                //Copy all of the attributes
                IFeature newPole = ((IFeatureClass)SupportStructure).CreateFeature();
                newPole.Shape = StagingPole.ShapeCopy;

                for (int i = 0; i < newPole.Fields.FieldCount; i++)
                {
                    IField field = newPole.Fields.get_Field(i);
                    if (field.Editable)
                    {
                        //This field is editable, so let's set it
                        newPole.set_Value(i, StagingPole.get_Value(i));
                    }
                }

                //Store the new pole
                newPole.Store();
            }
            catch (Exception ex)
            {
                if (Miner.Geodatabase.Edit.Editor.IsOperationInProgress()) { Miner.Geodatabase.Edit.Editor.AbortOperation(); }
                MessageBox.Show("Failed to combine pole to staging. Promoting the staging pole failed: " + ex.Message);
            }
        }

        private void btnCombingToExisting_Click(object sender, EventArgs e)
        {
            _cancelled = false;
            _cancelledAll = false;

            try
            {
                //Get the retire GUID field.
                int retireGuidIdx = SupportStructure.Fields.FindField("REPLACEGUID");
                int globalIDIdx = PTTSupportStructure.Fields.FindField("GLOBALID");
                object retireGuid = PTTSupportStructure.get_Value(globalIDIdx);

                //Combine the attributes onto the Support Structure feature
                CombineAttributes(SupportStructure);

                //Update retire guid to the global ID of the PTT Support Structure
                SupportStructure.set_Value(retireGuidIdx, retireGuid);
                SupportStructure.Store();

                //Delete the staging structure
                PTTSupportStructure.Delete();
            }
            catch (Exception ex) { throw new Exception("Failed to combine to existing pole: " + ex.Message); }
        }

        private void CombineAttributes(IFeature featureToUpdate)
        {
            try
            {
                //Use the grid cells that are currently hightlight green.  This will determine which values that
                //the user selected to use.
                for (int i = 0; i < gridAttributes.Rows.Count; i++)
                {
                    DataGridViewRow row = gridAttributes.Rows[i];
                    for (int j = 0; j < row.Cells.Count; j++)
                    {
                        if (row.Cells[j].Style.BackColor == Color.Green)
                        {
                            string fieldName = gridAttributes.Rows[i].Cells[3].Value.ToString();
                            int fieldIndex = featureToUpdate.Fields.FindField(fieldName);
                            featureToUpdate.set_Value(fieldIndex, gridAttributes.Rows[i].Cells[j].Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to copy attributes: " + ex.Message);
            }
        }
    }
}
