using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using PGE.BatchApplication.DLMTools.Utility.Annotation;

namespace PGE.BatchApplication.DLMTools.Commands.AnnotationComposite.UI
{
    /// <summary>
    /// A form allowing for the loading and changing of Annotation management settings.
    /// This form is launched via <see cref="CALMSettingsTool"/> and should remain on top of the ArcMap session.
    /// </summary>
    public partial class CALMSettingsUC : Form
    {
        #region Private Members
        private bool _populating = false;
        private const string Smaller = "Smaller";
        private const string Larger = "Larger";
        private const string NotApplicable = "N/A";
        #endregion

        private static readonly Dictionary<int, string> FontMappings = new Dictionary<int, string>
        {
            {AnnotationFacade.DoNotChangeFontSize, NotApplicable},
            {AnnotationFacade.UniformIncrease, Larger},
            {AnnotationFacade.UniformDecrease, Smaller},
            {6, "6"},
            {8, "8"},
            {10, "10"},
            {12, "12"},
            
        };

        private static readonly Dictionary<int, String> AlignmentMappings = new Dictionary<int, string>
        {
            {0, "Left"},
            {1, "Center"},
            {2, "Right"},
            {3, "N/A"}
        };

        #region Static Members/Properties

        public static string Rotation = null;
        
        //Defaults for X/Y as requested by business
        public static string XOffset = "0";
        public static string YOffset = "0";

        public static int FontSize = AnnotationFacade.DoNotChangeFontSize;
        public static int Alignment = 3;
        public static int Bold = 2;
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

            bool fontSizeSet = false;
            //Add toolbar values, set default value

            foreach (KeyValuePair<int, string> align in AlignmentMappings)
            {
                cboAlignment.Items.Add(new KeyValuePair<int, string>(align.Key, align.Value));
            }

            foreach (KeyValuePair<int, string> font in FontMappings)
            {
                cboSize.Items.Add(new KeyValuePair<int, string>(font.Key,
                    font.Value.ToString(CultureInfo.InvariantCulture)));
            }

            //Set values from static variables.
            _populating = true;
            txtOffsetX.Text = XOffset ?? "";
            txtOffsetY.Text = YOffset ?? "";
            txtRotation.Text = Rotation ?? "";

            for (int i = 0; i < cboSize.Items.Count; i++)
            {
                if (((KeyValuePair<int, String>) cboSize.Items[i]).Key != FontSize) continue;
                fontSizeSet = true;
                cboSize.SelectedIndex = i;
            }

            if (!fontSizeSet) cboSize.SelectedIndex = 0;
            
            cboAlignment.SelectedIndex = Alignment;
            chkDeleteAnno.Checked = Delete;
            chkInlineAnno.Checked = Inline;
            boldRadioNa.Checked = true;
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
            if (Int32.TryParse(alignment, out alignmentInt) && alignmentInt >= 0 && alignmentInt <= 3)
                cboAlignment.SelectedIndex = alignmentInt;

            int boldInt = 2;
            //Int32.TryParse(bold, out boldInt);

            switch (boldInt)
            {
                case 0:
                    boldRadioNo.Checked = true;
                    break;
                case 1:
                    boldRadioYes.Checked = true;
                    break;
                default:
                    boldRadioNa.Checked = true;
                    break;
            }

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
            cboSize.Enabled = enableControls;
            //boldGroupBox.Enabled = enableControls;
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
            Close();
        }

        #endregion

        private void GetAllProperties()
        {
            if (_populating) return;

            //Update the values in the static properties.
            Rotation = txtRotation.Text.Trim();
            XOffset = txtOffsetX.Text.Trim();
            YOffset = txtOffsetY.Text.Trim();

            if (cboSize.SelectedItem == null)
            {
                FontSize = AnnotationFacade.DoNotChangeFontSize;
            }
            else
            {
                string selectedValue = ((KeyValuePair<int, string>) cboSize.SelectedItem).Value;
                if (selectedValue.Equals(Smaller)) FontSize = AnnotationFacade.UniformDecrease;
                else if (selectedValue.Equals(Larger)) FontSize = AnnotationFacade.UniformIncrease;
                else if (selectedValue.Equals(NotApplicable)) FontSize = AnnotationFacade.DoNotChangeFontSize;
            }

            Alignment = cboAlignment.SelectedItem == null
                ? AnnotationFacade.DoNotChangeAlignment
                : ((KeyValuePair<int, string>) cboAlignment.SelectedItem).Key;
            Delete = chkDeleteAnno.Checked;
            Inline = chkInlineAnno.Checked;

            Bold = AnnotationFacade.DoNotChangeBoldSetting;
        }

        private void btnResetSettings_Click(object sender, EventArgs e)
        {
            txtOffsetX.Text = "0";
            txtOffsetY.Text = "0";
            txtRotation.Text = "";
            cboSize.SelectedIndex = 0;
            cboAlignment.SelectedIndex = 3;
            chkDeleteAnno.Checked = false;
            chkInlineAnno.Checked = false;
            boldRadioNa.Checked = true;

            GetAllProperties();
        }

    }
}
