#region Organized and sorted using

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems.Configuration;

#endregion

namespace PGE.Desktop.EDER.D8TreeTools.CreateRelatedObject
{
    /// <summary>
    /// Class for Sub Type Field custom editor.
    /// </summary>
    [ComVisible(true)]
    [Guid("863b2bcd-5d90-4c49-9aa4-da1882d39ab0")]
    [ComponentCategory(ComCategory.MMCustomFieldEditor)]
    [ComponentCategory(ComCategory.MMCustomFieldEditorInPlace)]
    public partial class Subtype_Field_Editor : UserControl, IMMCustomFieldEditor
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

        #region Private member variable

        /// <summary>
        /// Used for containing subtypes of this feature and later for populate the drop down.
        /// </summary>
        private Dictionary<string, string> subtypeValues = new Dictionary<string, string>();

        /// <summary>
        /// Used for the containing field value.
        /// </summary>
        private IMMFieldAdapter _field = null;

        /// <summary>
        /// Check editor is setup or not.
        /// </summary>
        private bool _allowSaveInput = false;

        ///// <summary>
        ///// Hold the value for Installation Type Code <example>SUB, PAD</example>.
        ///// </summary>
        //public static string InstallationTypeCode = string.Empty;

        /// <summary>
        /// Variable for error logging.
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        #endregion

        #region  Public Member variable

        /// <summary>
        /// Default value for sub type combo box.
        /// </summary>
        public static string DefaultValue = null;

        /// <summary>
        /// Parent Feature Class name.
        /// </summary>
        public static string ParentFeatureClass = null;

        /// <summary>
        /// Parent Sub type for the feature.
        /// </summary>
        public static string ParentSubtype = null;

        /// <summary>
        /// Used this for maintaining the selected index of the combo box.
        /// </summary>
        private static int _defaultSelectedIndex = -1;

        /// <summary>
        /// The mapping dictionary hold mapping for the subtype.
        /// </summary>
        private static Dictionary<string, Dictionary<string, string>> RelationshipSubtypeMapping = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for the class. Hold the event handler for combo box events.
        /// </summary>
        public Subtype_Field_Editor()
        {
            InitializeComponent();
            this.cbo_Subtype.LostFocus += new EventHandler(cbo_Subtype_FocusLost);
            //this.cbo_Subtype.Click += new EventHandler(cbo_Subtype_Click);
            this.cbo_Subtype.GotFocus += new EventHandler(cbo_Subtype_GotFocus);
            this.cbo_Subtype.SelectedIndexChanged += new EventHandler(cbo_Subtype_SelectedIndexChanged);
        }

        #endregion

        #region IMMCustomFieldEditor implementation
        /// <summary>
        ///  Set up the editor
        /// </summary>
        public void ActivateEditor()
        {
            _allowSaveInput = true;
        }

        /// <summary>
        /// Determine how the field appears when it is NOT being edited
        /// <remarks> The field value displayed will not reflect the format used by the field editor. As we used <c>null</c> here.</remarks>
        /// </summary>
        public string Caption
        {
            get { return null; }
        }

        /// <summary>
        ///  Save the values in the editor
        /// </summary>
        public void Commit()
        {
            Save();
        }

        /// <summary>
        /// Release member variables
        /// </summary>
        public void DeactivateEditor()
        {
            if (_field != null)
            {
                Marshal.ReleaseComObject(_field);
                _field = null;
            }
        }

        /// <summary>
        ///  specifies the type of the editor in terms of the <code>mmCustomFieldEditorType</code> enumeration (in-place or tabbed)
        /// </summary>
        public mmCustomFieldEditorType EditorType
        {
            get { return mmCustomFieldEditorType.mmCFEField; }
        }

        /// <summary>
        ///  Initialize variables for the field editor.
        /// </summary>
        /// <param name="pFA"></param>
        /// <param name="eMode"></param>
        public void InitEditor(IMMFieldAdapter pFA, mmDisplayMode eMode)
        {
            try
            {
                GetRelationshipMapping();
                //First we need to cycle through all of the subtypes of this feature and populate the drop
                //down list with the options just as the out of the box one would
                subtypeValues = new Dictionary<string, string>();
                IEnumSubtype subtypes = pFA.Subtypes;
                subtypes.Reset();
                int subtypeCode = -1;
                string subtype = subtypes.Next(out subtypeCode);
                //Determine all subtypes and their descriptions
                while (subtype != null)
                {
                    if (subtypeCode != -1 && !subtypeValues.ContainsKey(subtype))
                    {
                        subtypeValues.Add(subtype, subtypeCode.ToString());
                    }
                    subtype = subtypes.Next(out subtypeCode);
                }
                //Add subtype values to drop down
                foreach (KeyValuePair<string, string> kvp in subtypeValues)
                {
                    cbo_Subtype.Items.Add(kvp.Key);
                }

                //Assign our mode and field adapters for later use
                _field = pFA;
                mmDisplayMode mode = eMode;

                //Cycle through this ID8List to determine if this is a proposed new feature.  If it is,
                //determine if there is a relationship mapping defined for it and assign the appropriate
                //default value based on that mapping
                ID8List containingList = ((ID8ListItem)_field).ContainedBy;
                if (containingList != null && containingList is ID8ListItem)
                {
                    ID8ListItem listItem = containingList as ID8ListItem;
                    mmd8ItemType itemType = listItem.ItemType;

                    ID8List containingList2 = listItem.ContainedBy;
                    if (containingList2 != null && containingList2 is ID8ListItem)
                    {
                        ID8ListItem listItem2 = containingList2 as ID8ListItem;
                        mmd8ItemType itemType2 = listItem2.ItemType;
                        //if this is a target, then we are creating a new related unit and we should
                        //derive the related objects subtype if the definition exists.
                        if (listItem2.ItemType == mmd8ItemType.mmitTarget) // Code for selection tab.
                        {
                            IMMTarget target = listItem2 as IMMTarget;
                            string objClass = ((IDataset)target.ObjectClass).BrowseName;
                            if (ParentFeatureClass == null || objClass == null || RelationshipSubtypeMapping == null)
                            {
                                return;
                            }
                            string RelationshipName = ParentFeatureClass.ToUpper() + "_" + objClass.ToUpper();

                            if (RelationshipSubtypeMapping.ContainsKey(RelationshipName))
                            {
                                DefaultValue = RelationshipSubtypeMapping[RelationshipName][ParentSubtype];
                            }
                        }
                        else if (listItem2.ItemType == mmd8ItemType.mmd8itFavorite)// Code for target tab
                        {
                            if (ParentFeatureClass == null || RelationshipSubtypeMapping == null)
                            {
                                return;
                            }
                            string RelationshipName = ParentFeatureClass.ToUpper() + "_EDGIS." + listItem2.DisplayName.ToUpper().Replace(" ", string.Empty);

                            if (RelationshipSubtypeMapping.ContainsKey(RelationshipName))
                            {
                                DefaultValue = RelationshipSubtypeMapping[RelationshipName][ParentSubtype];
                            }
                        }
                    }
                }
                //Read in our current value and set the drop down
                Read();
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
        }

        /// <summary>
        /// Specifies the type of fields supported by the custom editor.
        /// </summary>
        public ESRI.ArcGIS.Geodatabase.esriFieldType[] SupportedFieldTypes
        {
            get
            {
                return new esriFieldType[] { esriFieldType.esriFieldTypeInteger };
            }
        }

        /// <summary>
        /// Name for the custom editor. This name will appear in the ArcFM field editor drop down box for a particular type of editor.  
        /// </summary>
        string IMMCustomFieldEditor.Name
        {
            get
            {
                _logger.Info("PG&E - Subtype Default-> Loaded");
                return "PG&E - Subtype Default";
            }
        }

        #endregion

        #region Private and Protected method

        /// <summary>
        /// Save field value from the control.
        /// </summary>
        private void Save()
        {
            try
            {
                if (_allowSaveInput && (_field != null))
                {
                    if (cbo_Subtype.SelectedIndex == -1)
                    {
                        cbo_Subtype.SelectedItem = null;
                    }
                    else
                    {
                        _field.Value = subtypeValues[cbo_Subtype.Items[cbo_Subtype.SelectedIndex].ToString()];
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
        }

        /// <summary>
        /// Read field value and display in control.
        /// </summary>
        private void Read()
        {
            try
            {
                if (_field != null)
                {
                    //If a default value is specified, then this is the value we will update
                    //and display in the drop down list when the dialog is first opened.
                    if (DefaultValue != null)
                    {
                        _field.Value = DefaultValue;
                        foreach (KeyValuePair<string, string> kvp in subtypeValues)
                        {
                            if (kvp.Value == DefaultValue)
                            {
                                int indexToSelect = cbo_Subtype.Items.IndexOf(kvp.Key);
                                _defaultSelectedIndex = indexToSelect;
                                cbo_Subtype.SelectedIndex = indexToSelect;
                                //Save();
                            }
                        }
                    }
                    //Otherwise we will simply display the default value that is brought in
                    //initially in the drop down list
                    else if (_field.Value != null)
                    {
                        string currentValue = _field.Value.ToString();
                        foreach (KeyValuePair<string, string> kvp in subtypeValues)
                        {
                            if (kvp.Value == currentValue)
                            {
                                int indexToSelect = cbo_Subtype.Items.IndexOf(kvp.Key);
                                _defaultSelectedIndex = indexToSelect;
                                cbo_Subtype.SelectedIndex = indexToSelect;
                                //Save();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
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

        /// <summary>
        /// Obtain the location of the configuration file which specifies the relationship subtype mapping
        /// </summary>
        /// <returns></returns>
        private string GetConfigPath()
        {
            //Read the config location from the Registry Entry
            SystemRegistry sysRegistry = new SystemRegistry("PGE");
            string _xmlFilePath = sysRegistry.ConfigPath;
            //prepare the xmlfile if config path exist
            return System.IO.Path.Combine(_xmlFilePath, "Relationship_Subtype_Mapping.xml");
        }

        /// <summary>
        /// When the field editor lost focus.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbo_Subtype_FocusLost(object sender, EventArgs e)
        {
            if (_defaultSelectedIndex != -1 && _field != null)
            {
                _field.Value = _defaultSelectedIndex;
            }
        }

        /// <summary>
        /// When the field editor gets focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbo_Subtype_GotFocus(object sender, EventArgs e)
        {
            //Bug#13226
            foreach (KeyValuePair<string, string> kvp in subtypeValues)
            {
                if (Convert.ToInt32(kvp.Value) == Convert.ToInt32(_field.Value))
                {
                    _defaultSelectedIndex = cbo_Subtype.Items.IndexOf(kvp.Key);
                    break;
                }
            }
            //_defaultSelectedIndex = Convert.ToInt16(_field.Value);
            cbo_Subtype.SelectedIndex = _defaultSelectedIndex;
        }      
                
        /// <summary>
        /// When the selected index changes let's save the new value and close the field editor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbo_Subtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            DefaultValue = null;
            ParentFeatureClass = null;
            ParentSubtype = null;
            Save();
        }
          
        /// <summary>
        /// Load the XML mapping for currently selected feature.
        /// </summary>
        private void GetRelationshipMapping()
        {
            try
            {
                //If the mapping dictionary is null then we need to get the xml configuration document
                //and parse through it to build the relationship subtype mapping which will
                //define what the default subtype should be for a given relationship when creating it
                RelationshipSubtypeMapping = new Dictionary<string, Dictionary<string, string>>();
                string ConfigPath = GetConfigPath();
                //Load the configuration xml document
                XmlDocument configDoc = new XmlDocument();
                configDoc.Load(ConfigPath);

                XmlNodeList relationshipNodes = configDoc.SelectNodes("/RelationshipClasses/Relationship");

                foreach (XmlNode node in relationshipNodes)
                {
                    if (ParentSubtype == node.Attributes["SubTypeCode"].InnerText.ToUpper())
                    {
                        string OriginName = node.Attributes["Origin"].InnerText.ToUpper();
                        string DestinationName = node.Attributes["Destination"].InnerText.ToUpper();
                        string RelationshipName = OriginName + "_" + DestinationName;

                        if (!RelationshipSubtypeMapping.ContainsKey(RelationshipName))
                        {
                            Dictionary<string, string> subtypeMappingDictionary = new Dictionary<string, string>();
                            XmlNodeList subtypeMapping = node.SelectNodes("Subtype");

                            foreach (XmlNode subtypeNode in subtypeMapping)
                            {
                                string OriginSubtype = subtypeNode.Attributes["Origin"].InnerText;
                                if (!subtypeMappingDictionary.ContainsKey(OriginSubtype))
                                {
                                    string DestinationSubtype = subtypeNode.Attributes["Destination"].InnerText;
                                    subtypeMappingDictionary.Add(OriginSubtype, DestinationSubtype);
                                }
                            }
                            RelationshipSubtypeMapping.Add(RelationshipName, subtypeMappingDictionary);
                            break;
                        }
                    }                   
                }
            }
            catch (Exception ex)
            {
                _logger.Error("ERROR - ", ex);
            }
        }

        /// <summary>
        /// When the form is resized the combo box needs to be resized as well.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.cbo_Subtype.Size = this.Size;
        }

        /// <summary>
        /// Key down event for the combo box.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cbo_Subtype_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            // Note: While you will receive the 'enter' key as expected in the
            // ArcFM attribute editor, this may not be true in the ArcFM locator.
            // This is believed to be a limitation of embedding ActiveX controls 
            // in a WinForm container.
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Save();
            }
        }

        #endregion

        private void cbo_Subtype_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
    }
}
