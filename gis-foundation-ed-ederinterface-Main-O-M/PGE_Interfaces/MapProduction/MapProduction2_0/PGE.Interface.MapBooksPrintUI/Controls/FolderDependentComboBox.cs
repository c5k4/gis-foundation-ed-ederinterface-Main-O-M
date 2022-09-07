using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PGE.Interfaces.MapBooksPrintUI.Controls
{
    public class FolderDependentComboBox : ComboBox
    {
        #region Private Members
        private FolderDependentComboBox _dependentBox = null;
        private bool _nullOption = false;
        #endregion

        #region Static Members
        /// <summary>
        /// Indicates the number of levels deep into the dependency hierarchy we are.
        /// Useful for executing logic after the highest-level control is done changing,
        /// without running it on any dependent controls.
        /// </summary>
        private static int _tiersChangingValue = 0;
        #endregion

        #region Properties
        /// <summary>
        /// The control that will depend on this control.
        /// </summary>
        public FolderDependentComboBox DependentBox
        {
            get { return _dependentBox; }
            set { _dependentBox = value; }
        }

        /// <summary>
        /// Indicates whether or not a non-empty value has been selected.
        /// </summary>
        public bool HasSelectedValue
        {
            get { return !string.IsNullOrEmpty(ToString()); }
        }

        /// <summary>
        /// Indicates whether or not a blank first item should be automatically added to the list.
        /// </summary>
        public bool NullOption
        {
            get { return _nullOption; }
            set { _nullOption = value; }
        }
        #endregion

        #region Public Events and Handlers
        /// <summary>
        /// Indicates that a selected index has been changed. 
        /// </summary>
        public event EventHandler SelectedIndexChangeCompleted;

        /// <summary>
        /// Indicates that the items within the object should be changed
        /// due to a change in the control that this object depends on.
        /// </summary>
        public event DropDownInitializingEventHandler DropDownInitializing;
        public delegate void DropDownInitializingEventHandler(FolderDependentComboBox comboBox);
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public FolderDependentComboBox()
            : base()
        {
            this.EnabledChanged += new EventHandler(fdc_EnabledChanged);
            this.SelectedIndexChanged += new EventHandler(fdc_SelectedIndexChanged);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Event Handler method for changing the Enabled property. Resets the selected item
        /// if the control is disabled.
        /// </summary>
        private void fdc_EnabledChanged(object sender, EventArgs e)
        {
            if (!Enabled)
                SelectedItem = "";
        }

        /// <summary>
        /// Event Handler method for changing the selected index. Controls logic to update the
        /// dependent box and keep track of how many levels deep this method is running.
        /// </summary>
        private void fdc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_dependentBox != null)
            {
                _tiersChangingValue++;

                _dependentBox.SelectedItem = "";
                if (HasSelectedValue)
                {
                    _dependentBox.Enabled = true;
                    if (_dependentBox.DropDownInitializing != null)
                        _dependentBox.DropDownInitializing(_dependentBox);
                }
                else
                    _dependentBox.Enabled = false;

                _tiersChangingValue--;
            }

            if (_tiersChangingValue == 0)
            {
                if (SelectedIndexChangeCompleted != null)
                    SelectedIndexChangeCompleted(sender, e);
            }
        }
        #endregion

        #region Public Overridden Methods
        /// <summary>
        /// .NET ComboBoxes support the <c>ValueMember</c> property only when data is bound to them.
        /// This method returns the selected value using the <c>ValueMember</c> property and converts
        /// it to a string.
        /// </summary>
        /// <returns>A string containing the value of the selected item (using <c>ValueMember</c> where possible) - remains <c>null</c> if no value was found.</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(ValueMember))
                return SelectedItem == null ? null : SelectedItem.ToString();

            if (Items.Count > 0 && SelectedIndex >= 0)
            {
                object item = Items[SelectedIndex];

                System.Reflection.PropertyInfo valueInfo = item.GetType().GetProperty(ValueMember);
                if (valueInfo != null)
                {
                    object thisValue = valueInfo.GetValue(item, null);
                    return thisValue == null ? null : thisValue.ToString();
                }
            }

            return null;
        }
        #endregion
    }
}
