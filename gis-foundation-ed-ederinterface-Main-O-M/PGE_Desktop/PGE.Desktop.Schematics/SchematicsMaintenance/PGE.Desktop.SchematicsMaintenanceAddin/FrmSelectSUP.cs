using System.Collections.Generic;
using System.Windows.Forms;

namespace PGE.Desktop.SchematicsMaintenance
{
    public partial class FrmSelectSUP : Form
    {
        public FrmSelectSUP()
        {
            InitializeComponent();
        }

        internal void SetSUPList(List<string> supList)
        {
            cboSupList.Items.Clear();
            if (supList != null && supList.Count > 0)
            {
                foreach (var item in supList)
                {
                    cboSupList.Items.Add(item);
                }
            }
        }

        internal string GetSelectedSessionID()
        {
            return cboSupList.SelectedItem.ToString();
        }
    }
}
