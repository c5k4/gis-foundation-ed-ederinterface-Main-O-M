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
    /* This class uses provides an interface that allows a user to create a new circuit ID from a selection of the Circuit Division, Substation ID, and manually entered Feeder ID.
     * It requires the CircuitDivision, and SubstationID domains to exist in the database.
*/    
    [ComponentCategory(ComCategory.MMCustomFieldEditor)]
    [ComponentCategory(ComCategory.MMCustomFieldEditorInPlace)]
    [ComVisible(true)]
    [Guid("A7F51C7C-C7F3-4C07-A429-AC2EF643AF2A")]
    public partial class CircuitIDSelector : UserControl, IMMCustomFieldEditor
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
        private const string CircuitDivisionDomain = "CircuitDivision";           
        
        private const string _NullDisplayText = "<Null>";
        private const string _ErrorDisplayText = "<Error>";
        private IMMFieldAdapter _field;
        private mmDisplayMode _mode;
        private bool _allowSaveInput;
        private static Dictionary<string, Color> ColorsByName = new Dictionary<string, Color>();
        private object selectedSubstationID = null;
        private object selectedCircuitDivision = null;
        private object selectedFeederID = null;

        public CircuitIDSelector()
        {
            this.ParentChanged += new EventHandler(CircuitIDSelector_ParentChanged);

            InitializeComponent();            
        }

        void cboColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CloseFieldEditor();
        }

        /// <summary>
        /// Save field value from the control.
        /// </summary>
        private void Save()
        {
            if (_allowSaveInput && (_field != null))
            {
                object value = txtCircuitID.Text;
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

                //Need to also set the CircuitDivison and SubstationID as well for this field editor.
                if (_field != null)
                {
                    ID8List containingList = ((ID8ListItem)_field).ContainedBy;
                    IMMFieldManagerEvents events = containingList as IMMFieldManagerEvents;
                    containingList.Reset();
                    ID8ListItem childItem = null;
                    while ((childItem = containingList.Next(false)) != null)
                    {
                        if (childItem is IMMFieldAdapter)
                        {
                            IMMFieldAdapter childAdapter = childItem as IMMFieldAdapter;
                            if (childAdapter.Name.ToUpper() == "SUBSTATIONID")
                            {
                                childAdapter.Value = selectedSubstationID;
                            }
                            else if (childAdapter.Name.ToUpper() == "DIVISION")
                            {
                                childAdapter.Value = selectedCircuitDivision;
                            }
                            else if (childAdapter.Name.ToUpper() == "FEEDERID")
                            {
                                childAdapter.Value = selectedFeederID;
                            }
                        }
                    }

                    events.OnValueChanged();

                    //IMMFieldManagerEvents events = containingList as IMMFieldManagerEvents;
                    //if (events != null)
                    //{
                        // Firing the "OnValueChanged" event on the field manager will
                        // cause the object editor to cleanup the active field editor.
                        // This will hide the custom field editor.
                    //    events.OnValueChanged();
                    //}
                }
            }
        }

        /// <summary>
        /// Read field value and display in control.
        /// </summary>
        private void Read()
        {
            txtCircuitID.Text = string.Empty;
            if (_field != null)
            {
                if (_field.Required)
                {
                    txtCircuitID.BackColor = Color.Yellow;
                }
                if ((_field.Value == null) || (_field.Value == DBNull.Value))
                {
                    txtCircuitID.Text = _NullDisplayText;
                }
                else if (_field.Value != null)
                {
                    if (_field.Value is String)
                    {
                        txtCircuitID.Text = Convert.ToString(_field.Value);
                    }
                    else
                    {
                        txtCircuitID.Text = _ErrorDisplayText;
                    }
                }
            }
        }

        string IMMCustomFieldEditor.Name
        {
            get
            {
                return "PGE Circuit ID Picker";
            }
        }

        void IMMCustomFieldEditor.Refresh()
        {
            Read();
        }

        bool setSizeChangedEvent = false;
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

        void CircuitIDSelector_ParentChanged(object sender, EventArgs e)
        {
            if (!setSizeChangedEvent)
            {
                if (this.Parent != null)
                {
                    this.Parent.SizeChanged += new EventHandler(Parent_SizeChanged);
                    setSizeChangedEvent = true;
                }
            }
        }

        /// <summary>
        /// If the user resized their attribute editor we can adjust the size of our control here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Parent_SizeChanged(object sender, EventArgs e)
        {
            this.Width = this.Parent.Width;
            this.Height = this.Parent.Height;
            txtCircuitID.Width = this.Parent.Width;
            txtCircuitID.Height = this.Parent.Height;
            btnOpenPicker.Left = this.Width - btnOpenPicker.Width;
            btnOpenPicker.Height = this.Parent.Height;
            //cboColors.DropDownHeight = cboColors.Height * 3;
        }

        /// <summary>
        /// This will force the custom field editor to be closed (hidden).
        /// </summary>
        /// <remarks>
        /// When you press 'enter' on the built-in editors the editor
        /// will be closed. However, for custom editors this will only occur
        /// if the field value actually changes because the OnValueChanged
        /// event will only fire if the value actually changes. In order to
        /// make the custom field editor "go away" when prettying 'enter' you
        /// can manually fire the event.
        /// </remarks>
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
            _field = null;
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

        private void OpenPicker()
        {
            _allowSaveInput = false;

            IWorkspace editWorkspace = Miner.Geodatabase.Edit.Editor.EditWorkspace;
            IWorkspaceDomains wsDomains = editWorkspace as IWorkspaceDomains;
            ICodedValueDomain circuitDivisionDomain = wsDomains.get_DomainByName(CircuitDivisionDomain) as ICodedValueDomain;
            IFeatureClass substationFeatureClass = PGE.Common.Delivery.Framework.ModelNameFacade.FeatureClassByModelName(editWorkspace, SchemaInfo.Electric.ClassModelNames.Substation);

            if (circuitDivisionDomain == null)
            {
                MessageBox.Show("Unable to find the " + CircuitDivisionDomain + " domain in the database");
            }
            else if (substationFeatureClass == null)
            {
                MessageBox.Show("Unable to find the feature class with the " + SchemaInfo.Electric.ClassModelNames.Substation + " model name applied");
            }
            else
            {
                CircuitIDSelectorForm circuitSelector = new CircuitIDSelectorForm(circuitDivisionDomain, substationFeatureClass);
                circuitSelector.StartPosition = FormStartPosition.CenterParent;
                DialogResult circuitSelectorResult = circuitSelector.ShowDialog(this.Parent);

                if (circuitSelectorResult == DialogResult.OK)
                {
                    txtCircuitID.Text = circuitSelector.CircuitID;
                    selectedSubstationID = circuitSelector.SubstationID;
                    selectedCircuitDivision = circuitSelector.CircuitDivision;
                    selectedFeederID = circuitSelector.FeederID;
                    _allowSaveInput = true;
                }
                Save();
            }
        }

        private void btnOpenPicker_Click(object sender, EventArgs e)
        {
            OpenPicker();
        }

        private void txtCircuitID_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtCircuitID_Click(object sender, EventArgs e)
        {
            OpenPicker();
        }
    }
}
