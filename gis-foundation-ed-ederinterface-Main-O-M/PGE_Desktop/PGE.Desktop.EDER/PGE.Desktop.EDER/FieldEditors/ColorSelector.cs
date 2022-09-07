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
    /* This class is a custom field editor for providing a list of color selections as defined by a domain.  The associated domain must be a text domain and the domain
 * must have codes that represent the RGB value being defined where R G B are separated by spaced (i.e Red = 255 0 0, Green = 0 255 0, Blue = 0 0 255).  The description
 * can be anything.  When a user selects the field drop down it will provide a list of the domain descriptions as well as a rectangle with the associated color.
 * 
*/    
    [ComponentCategory(ComCategory.MMCustomFieldEditor)]
    [ComponentCategory(ComCategory.MMCustomFieldEditorInPlace)]
    [ComVisible(true)]
    [Guid("90853C52-F63B-43A4-A115-3F7F8D2008B2")]
    public partial class ColorSelector : UserControl, IMMCustomFieldEditor
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

        private const string _NullDisplayText = "<Null>";
        private const string _ErrorDisplayText = "<Error>";
        private IMMFieldAdapter _field;
        private mmDisplayMode _mode;
        private bool _allowSaveInput;
        private static Dictionary<string, Color> ColorsByName = new Dictionary<string, Color>();

        public ColorSelector()
        {
            InitializeComponent();

            cboColors.DrawItem += new DrawItemEventHandler(cboColors_DrawItem);
            cboColors.KeyUp += new KeyEventHandler(cboColors_KeyUp);
            cboColors.SelectedIndexChanged += new EventHandler(cboColors_SelectedIndexChanged);

            if (ColorsByName.Count < 1)
            {
                ColorsByName.Add("None", Color.White);

                //Read the config location from the Registry Entry
                SystemRegistry sysRegistry = new SystemRegistry("PGE");
                string circuitColorFilePath = sysRegistry.ConfigPath;
                //prepare the xmlfile if config path exist
                string circuitColorXmlFilePath = System.IO.Path.Combine(circuitColorFilePath, "PGECircuitColor.xml");

                if (!System.IO.File.Exists(circuitColorXmlFilePath)) 
                {
                    string assemblyLoc = Assembly.GetExecutingAssembly().Location;
                    circuitColorXmlFilePath = System.IO.Path.Combine(assemblyLoc, "PGECircuitColor.xml");
                }

                _logger.Debug("Loading circuit colors from " + circuitColorXmlFilePath);

                try
                {
                    using (XmlReader reader = XmlReader.Create(new StreamReader(circuitColorXmlFilePath)))
                    {
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    if (reader.Name.ToUpper() == "COLOR")
                                    {
                                        string colorName = reader.GetAttribute("ColorName");
                                        string colorRGB = reader.GetAttribute("ColorRGB");
                                        string[] colorSplit = Regex.Split(colorRGB, ",");
                                        if (colorSplit.Length == 3)
                                        {
                                            Color newColor = Color.FromArgb(Int32.Parse(colorSplit[0]), Int32.Parse(colorSplit[1]), Int32.Parse(colorSplit[2]));
                                            ColorsByName.Add(colorName, newColor);
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("Error loading circuit color configuration: " + ex.Message);
                }
            }
        }

        void cboColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CloseFieldEditor();
        }

        void cboColors_KeyUp(object sender, KeyEventArgs e)
        {
            //When the user presses enter we can save the selection and notify the control we are finished editing
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Save();

                CloseFieldEditor();
            }
        }        

        /// <summary>
        /// This method handles drawing the colors/text to the custom drop down menu for selecting a color. It uses the domain to build its list of colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cboColors_DrawItem(object sender, DrawItemEventArgs e)
        {
            int index = e.Index >= 0 ? e.Index : 0;

            if (cboColors.Items.Count < 2) { return; }

            //First get the code and value associated with this entry.
            string code = cboColors.Items[index].ToString();
            string domainValue = "";
            ICodedValueDomain codedValueDomain = _field.Domain as ICodedValueDomain;
            for (int i = 0; i < codedValueDomain.CodeCount; i++)
            {
                if (codedValueDomain.get_Value(i).ToString() == code)
                {
                    domainValue = codedValueDomain.get_Name(i).ToString();
                }
            }

            Rectangle fillRectangle = e.Bounds;
            fillRectangle.Width = Convert.ToInt32(fillRectangle.Width * 0.9);
            fillRectangle.X = (e.Bounds.Width - fillRectangle.Width) / 2;
            fillRectangle.Height = Convert.ToInt32(fillRectangle.Height * 0.8);
            fillRectangle.Y += (e.Bounds.Height - fillRectangle.Height) / 2;

            //Now we can determine the actual color to draw from the code and draw our color name / color for this item.
            if (code == "")
            {
                e.DrawBackground();
                e.Graphics.DrawRectangle(new Pen(new SolidBrush(Color.Black)), fillRectangle);
                e.Graphics.DrawString(_NullDisplayText, e.Font, new SolidBrush(Color.Black), fillRectangle, StringFormat.GenericDefault);
                e.DrawFocusRectangle();
            }
            else
            {
                //Determine our RGB values
                Color color = ColorsByName["None"];
                if (ColorsByName.ContainsKey(domainValue))
                {
                    color = ColorsByName[domainValue];
                }
                
                Pen newPen = new Pen(new SolidBrush(Color.Black));

                e.DrawBackground();
                e.Graphics.FillRectangle(new SolidBrush(color), fillRectangle);
                e.Graphics.DrawRectangle(newPen, fillRectangle);
                e.Graphics.DrawString(domainValue, e.Font, new SolidBrush(Color.Black), fillRectangle, StringFormat.GenericDefault);
                e.DrawFocusRectangle();
            }
        }

        /// <summary>
        /// Save field value from the control.
        /// </summary>
        private void Save()
        {
            if (_allowSaveInput && (_field != null))
            {
                object value = cboColors.SelectedItem;
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

        /// <summary>
        /// Read field value and display in control.
        /// </summary>
        private void Read()
        {
            //If our combo box is empty we first need to populate it from the associated domain.
            if (cboColors.Items.Count < 2)
            {
                List<string> itemsToAdd = new List<string>();
                ICodedValueDomain codedValueDomain = _field.Domain as ICodedValueDomain;
                if (codedValueDomain == null)
                {
                    MessageBox.Show("The PGE Circuit Color field editor requires a domain to be assigned");
                    return;
                }
                for (int i = 0; i < codedValueDomain.CodeCount; i++)
                {
                    string code = codedValueDomain.get_Value(i).ToString();
                    itemsToAdd.Add(code);
                }
                itemsToAdd.Sort();
                cboColors.Items.AddRange(itemsToAdd.ToArray());
            }

            cboColors.Text = string.Empty;
            if (_field != null)
            {
                if ((_field.Value == null) || (_field.Value == DBNull.Value))
                {
                    cboColors.Text = _NullDisplayText;
                }
                else if (_field.Value != null)
                {
                    if (_field.Value is String)
                    {
                        string value = Convert.ToString(_field.Value);
                        if (cboColors.Items.Contains(value)) { cboColors.SelectedItem = value; }
                        else { cboColors.SelectedItem = ""; }
                    }
                    else
                    {
                        cboColors.Text = _ErrorDisplayText;
                    }
                }
            }
        }

        string IMMCustomFieldEditor.Name
        {
            get
            {
                return "PGE Circuit Color Picker";
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
            if (!setSizeChangedEvent && this.Parent != null)
            {
                this.Parent.SizeChanged += new EventHandler(Parent_SizeChanged);
                setSizeChangedEvent = true;
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
            cboColors.Width = this.Parent.Width;
            cboColors.IntegralHeight = false;
            cboColors.ItemHeight = this.Parent.Height;
            cboColors.Height = this.Parent.Height;
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
    }
}
