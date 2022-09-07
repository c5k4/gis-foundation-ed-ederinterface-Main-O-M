using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Miner.ComCategories;
using System.Runtime.InteropServices;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using System.Text.RegularExpressions;
using PGE.Common.Delivery.Systems.Configuration;
using System.Reflection;
using System.Xml;
using System.IO;
using PGE.Common.Delivery.Diagnostics;

namespace PGE.Desktop.EDER.FieldEditors
{
    [ComponentCategory(ComCategory.MMCustomFieldEditor)]
    [ComponentCategory(ComCategory.MMCustomFieldEditorInPlace)]
    [ComVisible(true)]
    [Guid("1B462EBC-A84B-48CA-8541-EB7775ADEA97")]
    public partial class DateSelector : UserControl, IMMCustomFieldEditor
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.MMCustomFieldEditor.Register(regKey);
            Miner.ComCategories.MMCustomFieldEditorInPlace.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.MMCustomFieldEditor.Unregister(regKey);
            Miner.ComCategories.MMCustomFieldEditorInPlace.Unregister(regKey);
        }

        #endregion

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private IMMFieldAdapter _field;
        private mmDisplayMode _mode;
        private bool _allowSaveInput;
        private const string _NullDisplayText = "<Null>";
        private const string _ErrorDisplayText = "<Error>";
        public static string DefaultValue = null;
        private object selectedDate = null;
        private static int _defaultSelectedIndex = -1;
        
        public DateSelector()
        {
            InitializeComponent();
            this.dateTimeSelection.ValueChanged += new EventHandler(dateTimePicker1_ValueChanged);
            
        }

        public void ActivateEditor()
        {
            _allowSaveInput = true;
        }

        public string Caption
        {
            get { return null; }
        }

        public void Commit()
        {
            Save();
        }

        public void DeactivateEditor()
        {
            if (_field != null)
            {
                Marshal.ReleaseComObject(_field);
                _field = null;
            }
        }

        public mmCustomFieldEditorType EditorType
        {
            get { return mmCustomFieldEditorType.mmCFEField; }
        }

        public void InitEditor(IMMFieldAdapter pFA, mmDisplayMode eMode)
        {
            _field = pFA;
            _mode = eMode;
        }

        public esriFieldType[] SupportedFieldTypes
        {
            get { return new esriFieldType[] { esriFieldType.esriFieldTypeDate }; }
        }

        string IMMCustomFieldEditor.Name
        {
            get
            {
                return "PGE Date Picker";
            }
        }

        /// <summary>
        /// Save field value from the control.
        /// </summary>
        private void Save()
        {
            if (_allowSaveInput && (_field != null))
            {
                object value = dateTimeSelection.Value.ToString().Split(new char[0], StringSplitOptions.RemoveEmptyEntries)[0];
                if (value == null || string.IsNullOrEmpty(value.ToString()) ||
                    (string.Compare(value.ToString(), _NullDisplayText, true) == 0) ||
                    (string.Compare(value.ToString(), _ErrorDisplayText, true) == 0)
                    )
                {
                    _field.Value = DBNull.Value;
                }
                else
                {
                    _field.Value = value.ToString();
                }
            }
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            DefaultValue = null;
            Save();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            //base.OnSizeChanged(e);
            //this.dateTimeSelection.Size = this.Size;
        }

        private void dateTimePicker1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Enter)
            //{
            //    e.Handled = true;
            //    Save();
            //}
        }
       
        private void CloseFieldEditor()
        {
            if (_field != null)
            {
                ID8List containingList = ((ID8ListItem)_field).ContainedBy;
                IMMFieldManagerEvents events = containingList as IMMFieldManagerEvents;
                if (events != null)
                {
                    // Firing the "OnValueChanged" event on the field manager will
                    // cause the object editor to cleanup the active field editor.
                    // This will hide the custom field editor.
                    events.OnValueChanged();
                }
            }
        }

        private void dateTimePicker1_KeyPress(object sender, KeyPressEventArgs e)
        {
            //e.Handled = true;
            //Save();
            //this.dateTimeSelection.UseWaitCursor = true;
        }

        private void dateTimePicker1_Enter(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void dateTimePicker1_MouseEnter(object sender, EventArgs e)
        {
            //dateTimeSelection.Format = DateTimePickerFormat.Custom;
            //dateTimeSelection.CustomFormat = "MM/dd/yyyy";

        }

    }
}
