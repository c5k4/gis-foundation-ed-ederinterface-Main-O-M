using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PGE.Desktop.Custom.Commands.AnnotationComposite.UI
{
    /// <summary>
    /// A form allowing for the loading and changing of Annotation management settings.
    /// This form is launched via <see cref="CALMSettingsTool"/> and should remain on top of the ArcMap session.
    /// </summary>
    public partial class CALMSettingsUC : Form
    {
        #region Private Members
        bool _populating = false;
        #endregion

        #region Static Members/Properties

        public static string Rotation = null;

        //Defaults for X/Y as requested by business
        public static string XOffset = "0";
        public static string YOffset = "0";

        public static string FontSize = "";
        public static int Alignment = 3;
        public static bool Delete = false;
        public static bool Inline = false;

        private static bool _controlsEditable = true;

        /// <summary>
        /// The instance of the settings UI, for use with the toolbar.
        /// Creates an instance if none exists.
        /// </summary>
        public static CALMSettingsUC Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = null;
                    _instance = new CALMSettingsUC();
                }

                return _instance;
            }
        }
        private static CALMSettingsUC _instance = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        private CALMSettingsUC()
        {
            InitializeComponent();

            //Add toolbar values
            cboAlignment.Items.Add(new KeyValuePair<int, string>(0, "Left"));
            cboAlignment.Items.Add(new KeyValuePair<int, string>(1, "Center"));
            cboAlignment.Items.Add(new KeyValuePair<int, string>(2, "Right"));
            cboAlignment.Items.Add(new KeyValuePair<int, string>(3, "None"));


            //Set values from static variables.
            _populating = true;
            txtOffsetX.Text = XOffset ?? "";
            txtOffsetY.Text = YOffset ?? "";
            txtRotation.Text = Rotation ?? "";
            txtSize.Text = FontSize ?? "";
            cboAlignment.SelectedIndex = Alignment;
            chkDeleteAnno.Checked = Delete;
            chkInlineAnno.Checked = Inline;
            _populating = false;

            //Enable/disable controls based on the static variable to preserve this.
            ControlsEnabled(_controlsEditable);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads from XML configuration settings - casting into relevant types as necessary.
        /// </summary>
        /// <param name="rotation">The "Rotation" configuration value to load.</param>
        /// <param name="xOffset">The "X Offset" configuration value to load.</param>
        /// <param name="yOffset">The "Y Offset" configuration value to load.</param>
        /// <param name="alignment">The "Alignment" configuration value to load.</param>
        /// <param name="delete">The "DeleteJobNumberAnno" configuration value to load.</param>
        /// <param name="inline">The "InlineGroupAnno" configuration value to load.</param>
        /// <param name="fontSize">The font size configuration value to load</param>
        /// <param name="bold">The bold setting configuration value to load</param>
        public void LoadFromSettings(string rotation, string xOffset, string yOffset, string alignment, string delete, string inline,
            string fontSize, string bold)
        {
            //Load these into the textbox fields, let error messages manage them if they are off-base, though they shouldn't be.
            txtRotation.Text = rotation;
            txtOffsetX.Text = xOffset;
            txtOffsetY.Text = yOffset;

            //Select alignment value if our config value corresponds to one of the options; otherwise leave it alone.
            int alignmentInt = -1;
            if (Int32.TryParse(alignment, out alignmentInt) && alignmentInt >= 0 && alignmentInt <= cboAlignment.Items.Count)
                cboAlignment.SelectedIndex = alignmentInt;

            //Look into each "checkable" value if we can discern a "true" out of the configuration value.
            bool deleteBool = false;
            Boolean.TryParse(delete, out deleteBool);
            chkDeleteAnno.Checked = deleteBool;

            bool inlineBool = false;
            Boolean.TryParse(inline, out inlineBool);
            chkInlineAnno.Checked = inlineBool;

            //Manually call this event to set the static properties.
            ApplyToProperties(null, new EventArgs());
        }

        /// <summary>
        /// Enables or disables relevant controls in this form.
        /// </summary>
        /// <param name="enableControls"><c>true</c> to enable all relevant controls, <c>false</c> to disable them.</param>
        public void ControlsEnabled(bool enableControls)
        {
            txtOffsetX.Enabled = enableControls;
            txtOffsetY.Enabled = enableControls;
            txtRotation.Enabled = enableControls;
            cboAlignment.Enabled = enableControls;
            chkDeleteAnno.Enabled = enableControls;
            chkInlineAnno.Enabled = enableControls;
            txtSize.Enabled = enableControls;
            btnResetSettings.Enabled = enableControls;

            _controlsEditable = enableControls;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires when any settings checkbox changes.
        /// </summary>
        private void AnnoCtrl_CheckedChanged(object sender, EventArgs e)
        {
            ApplyToProperties(sender, e);
        }

        /// <summary>
        /// Handles keyboard shortcuts.
        /// </summary>
        private void AnnoCtrl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    e.Handled = true;
                    ApplyToProperties(sender, e);
                    return;
            }
        }

        /// <summary>
        /// Applies the annotation changes to the selected objects.
        /// </summary>
        private void ApplyToProperties(object sender, EventArgs e)
        {
            GetAllProperties();
        }

        private void ApplyToProperties(object sender, MouseEventArgs e)
        {
            GetAllProperties();
        }

        /// <summary>
        /// Closes the UI form.
        /// </summary>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private void GetAllProperties()
        {
            if (_populating) return;

            //Update the values in the static properties.
            Rotation = txtRotation.Text.Trim();
            XOffset = txtOffsetX.Text.Trim();
            YOffset = txtOffsetY.Text.Trim();
            FontSize = txtSize.Text.Trim();

            Alignment = cboAlignment.SelectedItem == null
                ? cboAlignment.Items.Count - 1
                : ((KeyValuePair<int, string>)cboAlignment.SelectedItem).Key;

            Delete = chkDeleteAnno.Checked;
            Inline = chkInlineAnno.Checked;
        }

        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            txtOffsetX.Text = "0";
            txtOffsetY.Text = "0";
            txtRotation.Text = "";
            txtSize.Text = "";
            cboAlignment.SelectedIndex = cboAlignment.Items.Count - 1;
            chkDeleteAnno.Checked = false;
            chkInlineAnno.Checked = false;

            GetAllProperties();
        }

    }
}
