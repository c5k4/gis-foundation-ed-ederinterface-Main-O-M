using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PGE.Interfaces.MapBooksPrintUI.Controls
{
    public class MultiSelectCheckBox : MultiSelect
    {
        CheckedListBox _cbGridMultiSelect;

        public bool ChecksUpdating = false;

        public event ItemCheckEventHandler MultiItemChecked
        {
            add
            {
                if (_cbGridMultiSelect != null)
                    _cbGridMultiSelect.ItemCheck += value;
            }
            remove
            {
                if (_cbGridMultiSelect != null)
                    _cbGridMultiSelect.ItemCheck -= value;
            }
        }

        public MultiSelectCheckBox()
            : base()
        {
            _cbGridMultiSelect = new CheckedListBox() { FormattingEnabled = true, Name = "cbGridMultiSelect", Size = new Size(170, 150), CheckOnClick = true, ThreeDCheckBoxes = true, BorderStyle = BorderStyle.Fixed3D };
            _cbGridMultiSelect.ItemCheck += new ItemCheckEventHandler(cbGridMultiSelect_ItemCheck);
            this.DropDownControl = _cbGridMultiSelect;
        }

        void cbGridMultiSelect_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (ChecksUpdating) return;

            ChecksUpdating = true;

            //Determine the index of the "Select All" checkbox.
            int idxSelectAll = -1;
            for (int i = 0; i < _cbGridMultiSelect.Items.Count; i++)
            {
                if (_cbGridMultiSelect.Items[0].ToString() == Common.SelectAllText)
                {
                    idxSelectAll = i;
                    break;
                }
            }
                
            if (idxSelectAll == e.Index)
            {
                //Select/unselect all based on the value of the "Select All" option.
                for (int i = 0; i < _cbGridMultiSelect.Items.Count; i++)
                {
                    if (i == idxSelectAll) continue;

                    _cbGridMultiSelect.SetItemChecked(i, e.NewValue == CheckState.Checked);
                }
            }
            else if (e.NewValue == CheckState.Unchecked && _cbGridMultiSelect.GetItemChecked(idxSelectAll))
            {
                //Set to indeterminate (also useful so that this event isn't called again) if something is being unchecked
                _cbGridMultiSelect.SetItemCheckState(idxSelectAll, (_cbGridMultiSelect.CheckedItems.Count > 0 ? CheckState.Indeterminate : CheckState.Unchecked));
            }
            else if (e.NewValue == CheckState.Checked && _cbGridMultiSelect.GetItemCheckState(idxSelectAll) != CheckState.Checked)
            {
                //If everything is selected, check the "select all" box.
                if (_cbGridMultiSelect.CheckedItems.Count == _cbGridMultiSelect.Items.Count - 1)
                    _cbGridMultiSelect.SetItemCheckState(idxSelectAll, CheckState.Checked);
            }

            ChecksUpdating = false;
        }

        public List<string> GetSelectedItemList()
        {
            List<string> items = new List<string>();

            foreach (object checkItem in (this.DropDownControl as CheckedListBox).CheckedItems)
            {
                if (checkItem.ToString() != Common.SelectAllText)
                    items.Add(checkItem.ToString());
            }

            return items;
        }
    }
}
