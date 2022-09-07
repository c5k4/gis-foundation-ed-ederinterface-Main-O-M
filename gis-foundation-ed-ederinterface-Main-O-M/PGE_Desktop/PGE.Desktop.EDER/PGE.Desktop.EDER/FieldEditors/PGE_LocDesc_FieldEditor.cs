#region references
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
#endregion

namespace PGE.Desktop.EDER.FieldEditors
{
    [ComponentCategory(ComCategory.MMCustomFieldEditor)]
    [ComponentCategory(ComCategory.MMCustomFieldEditorInPlace)]
    [ComVisible(true)]
    [Guid("7DA421B1-0449-452A-8EA4-D7815263A681")]
    public partial class PGE_LocDesc_FieldEditor : UserControl, IMMCustomFieldEditor
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

        #region Global Varriables
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private const string _NullDisplayText = "<Null>";
        private const string _ErrorDisplayText = "<Error>";
        private IMMFieldAdapter _field;
        private mmDisplayMode _mode;
        private bool _allowSaveInput;
        ToolTip toolTip1 = new ToolTip();
        #endregion

        #region Constructor
        public PGE_LocDesc_FieldEditor()
        {
            this.TextChanged += new EventHandler(LocDesc_textBox_TextChanged);
            InitializeComponent();
        }

        #endregion

        #region events
        private void LocDesc_textBox_TextChanged(object sender, EventArgs e)
        {
            Save();
        }

        bool setSizeChangedEvent = false;
        /// <summary>
        /// If the user resized their attribute editor we can adjust the size of our control here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Parent_SizeChanged(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;
            LocDesc_textBox.Width = this.Parent.Width;
            LocDesc_textBox.Height = this.Parent.Height;
        }

        #endregion

        #region CutomFieldEditor methods

        public string Caption
        {
            get
            {
                if (_field != null)
                {
                    try
                    {
                        if (_field.Value != null)
                        {
                            if (_field.Value.ToString().Length > 9)
                            {
                                return _field.Value.ToString();
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                return null;
            }
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
            if (toolTip1 != null)
            {
                toolTip1.Dispose();
                toolTip1 = null;
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

        public ESRI.ArcGIS.Geodatabase.esriFieldType[] SupportedFieldTypes
        {
            get { return new esriFieldType[] { esriFieldType.esriFieldTypeString }; }
        }

        string IMMCustomFieldEditor.Name
        {
            get
            {
                return "PGE LocDesc Field Editor";
            }
        }
                
        public void ActivateEditor()
        {
            Read();
            _allowSaveInput = true;

            //If we haven't set our event to watch for parent size changes we can do that here.
            if (!setSizeChangedEvent)
            {
                if (this.Parent != null)
                {
                    this.Parent.SizeChanged += new EventHandler(Parent_SizeChanged);
                    setSizeChangedEvent = true;
                }
            }

        }
              
        void IMMCustomFieldEditor.Refresh()
        {
            Read();
        }

        #endregion

        #region private methods

        /// <summary>
        /// Save field value from the control.
        /// </summary>
        private void Save()
        {
            if (_allowSaveInput && (_field != null))
            {
                object value = LocDesc_textBox.Text;
                if (value == null || string.IsNullOrEmpty(value.ToString()) ||
                    (string.Compare(value.ToString(), _NullDisplayText, true) == 0) ||
                    (string.Compare(value.ToString(), _ErrorDisplayText, true) == 0)
                    )
                {
                    _field.Value = DBNull.Value;
                }
                else if (value.ToString().Length < 10)
                {
                    if (toolTip1 == null)
                    {
                        toolTip1 = new ToolTip();
                    }
                    toolTip1.Show("value must be minimum 10 characters", LocDesc_textBox, 3000);
                    _field.Value = DBNull.Value;
                }
                else
                {
                    if (toolTip1 == null)
                    {
                        toolTip1 = new ToolTip();
                    }
                    toolTip1.Hide(LocDesc_textBox);
                    _field.Value = value.ToString();
                }
            }
        }


        /// <summary>
        /// Read field value and display in control.
        /// </summary>
        private void Read()
        {
            LocDesc_textBox.Text = string.Empty;
            if (_field != null)
            {
                if (_field.Required)
                {
                    //textBox1.BackColor = Color.Yellow;
                }
                if ((_field.Value == null) || (_field.Value == DBNull.Value))
                {
                    LocDesc_textBox.Text = _NullDisplayText;
                }
                else if (_field.Value != null)
                {
                    if (_field.Value is String)
                    {
                        LocDesc_textBox.Text = Convert.ToString(_field.Value);
                    }
                    else
                    {
                        LocDesc_textBox.Text = _ErrorDisplayText;
                    }
                }
            }

        }

        #endregion
    }
}
