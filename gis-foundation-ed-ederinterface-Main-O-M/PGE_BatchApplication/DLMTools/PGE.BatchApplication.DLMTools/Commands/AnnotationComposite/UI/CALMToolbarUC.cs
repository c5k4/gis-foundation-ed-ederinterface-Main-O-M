using System;
using System.Windows.Forms;
using PGE.BatchApplication.DLMTools.Utility.Annotation;
using ESRI.ArcGIS.Geodatabase;

namespace PGE.BatchApplication.DLMTools.Commands.AnnotationComposite.UI
{
    /// <summary>
    /// Part of the CALM Toolbar (added to it via <see cref="CALMToolbarComposite"/>) - which contains all Windows Forms
    /// elements that will be directly placed on the toolbar.
    /// </summary>
    public partial class CALMToolbarUC : UserControl
    {
        #region Private Static Members

        private static string _lastEditedVersion = null;
        private static CALMConfig _config = new CALMConfig();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public CALMToolbarUC()
        {
            InitializeComponent();

            LoadUserData();

            //Set this to the blank value.
            CboCompositeType.SelectedValue = 1;

            Miner.Geodatabase.Edit.Editor.OnStartEditing += Editor_OnStartEditing;
            Miner.Geodatabase.Edit.Editor.OnStopEditing += Editor_OnStopEditing;

            bool enableControls = Miner.Geodatabase.Edit.Editor.EditState != Miner.Geodatabase.Edit.EditState.StateNotEditing;
            ControlsEnabled(enableControls);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads user data from the configuration class and inserts each favorite name into the dropdown.
        /// </summary>
        private void LoadUserData()
        {
            CboCompositeType.Items.Add("");
            _config.GetFavoriteNames().ForEach(n => CboCompositeType.Items.Add(n));
        }

        /// <summary>
        /// Enables or disables relevant controls in this form.
        /// </summary>
        /// <param name="enableControls"><c>true</c> to enable all relevant controls, <c>false</c> to disable them.</param>
        private void ControlsEnabled(bool enableControls)
        {
            CboCompositeType.Enabled = enableControls;
        }

        #endregion

        #region Events

        /// <summary>
        /// Called when the application moves out of edit mode.
        /// </summary>
        private void Editor_OnStopEditing(object sender, Miner.Geodatabase.Edit.SaveEditEventArgs e)
        {
            ControlsEnabled(false);
        }

        /// <summary>
        /// Called when the application moves into edit mode.
        /// </summary>
        private void Editor_OnStartEditing(object sender, Miner.Geodatabase.Edit.EditEventArgs e)
        {
            if (_lastEditedVersion == null || _lastEditedVersion != ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName)
            {
                _lastEditedVersion = ((IVersion)Miner.Geodatabase.Edit.Editor.EditWorkspace).VersionName;
            }
            ControlsEnabled(true);
        }

        /// <summary>
        /// Handles the change in selection for the composite type dropdown.
        /// Loads relevant values into the settings window.
        /// </summary>
        private void CboCompositeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string compositeName = CboCompositeType.SelectedItem.ToString();

            if (compositeName == string.Empty)
            {
                //Make sure the controls in the settings window are enabled, but don't reset them in case they want to tweak something.
                CALMSettingsUC.Instance.ControlsEnabled(true);
            }
            else
            {
                //Get the settings.
                string offsetX, offsetY, rotation, alignment, deleteJobNumber, inlineGroupAnno, fontSize, bold;
                if (_config.GetFavoriteSettings(compositeName, out offsetX, out offsetY, out rotation, out alignment, out deleteJobNumber, out inlineGroupAnno, out fontSize, out bold))
                {
                    //Load the settings into the settings window.
                    CALMSettingsUC.Instance.ControlsEnabled(false);
                    CALMSettingsUC.Instance.LoadFromSettings(rotation, offsetX, offsetY, alignment, deleteJobNumber, inlineGroupAnno, fontSize, bold);
                }
                else
                {
                    CALMSettingsUC.Instance.ControlsEnabled(true);
                }
            }
        }

        #endregion
    }
}
