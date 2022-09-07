#region Organize and sorted using
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
#endregion

namespace PGE.Desktop.EDER.LocatorTools.MlxRelationshipTool
{
    /// <summary>
    /// User interface class for PM Order number search.
    /// </summary>
    [Guid("0EA82D48-5BB5-4818-A9DA-1AF3DF826370")]
    [ProgId("PGE.Desktop.EDER.PGE_PMOrderNumberRelateUI")]
    public partial class PGE_PMOrderNumberRelateUI : Form
    {
        #region Private Member variable

        /// <summary>
        /// User entered PM order number.
        /// </summary>
        private string _PMOrderNumber = string.Empty;

        #endregion

        #region Public property and constructor

        /// <summary>
        /// Default constructor for the UI.
        /// </summary>
        public PGE_PMOrderNumberRelateUI()
        {
            InitializeComponent();
        }

        /// <summary>
        /// PM Order number entered by the user.
        /// </summary>
        public string PMOrderNumber
        {
            get
            {
                return _PMOrderNumber;
            }
            set
            {
                _PMOrderNumber = PMOrderNumber;
            }
        }

        #endregion

        #region Private Members

        /// <summary>
        /// OK button click event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {            
            this.btnOk.Enabled = false;
            txtPMOrderNumber.Text = txtPMOrderNumber.Text.Trim();
            if (txtPMOrderNumber.Text.Length > 11 || txtPMOrderNumber.Text.Length == 0)
            {
                MessageBox.Show("MLX number length must be 1 to 11 characters.", "Info", MessageBoxButtons.OK);
                return;
            }
            _PMOrderNumber = txtPMOrderNumber.Text;
            this.Close();
        }

        /// <summary>
        /// Cancel button event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPMOrderNumber.Text = string.Empty;
            this.Close();
        }

        /// <summary>
        /// Form Closing event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PGE_PMOrderNumberRelateUI_FormClosing(object sender, FormClosingEventArgs e)
        {
            txtPMOrderNumber.Text = string.Empty;
        }

        #endregion
    }
}
