using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Controls;
using System.IO;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.SystemUI;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;
using Miner.Interop;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
//using Oracle.DataAccess.Client;
using ESRI.ArcGIS.esriSystem;
using System.Collections.Generic;
using System.Linq;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using System.Collections;
using Miner.ComCategories;
using System.Text;
namespace PGE.Desktop.EDER
{
    /// <summary>
    /// Command that works in ArcMap/Map/PageLayout
    /// This is 
    /// </summary>
    [Guid("fc7f8355-5dc8-4090-948b-8daa1e6d526c")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.SelectReviewMode")]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public sealed class SelectReviewMode : BaseCommand, IComboBox, IMMSimpleSetting
    {
        // global var
        string[] fileslist = { "All", "Pre-Map", "CMCS", "As-Built" };
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GMxCommands.Register(regKey);
            MxCommands.Register(regKey);
            SxCommands.Register(regKey);
            ControlsCommands.Register(regKey);
        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            GMxCommands.Unregister(regKey);
            MxCommands.Unregister(regKey);
            SxCommands.Unregister(regKey);
            ControlsCommands.Unregister(regKey);
        }

        #endregion
        #endregion

        private IApplication m_application;
        private System.Collections.Generic.Dictionary<int, string> m_itemMap;
        private string m_strWidth = @"c:\documents\map documents";
        private IComboBoxHook comboHook = null;

        public SelectReviewMode()
        {
            m_itemMap = new System.Collections.Generic.Dictionary<int, string>();

            base.m_category = "PGE Tools";
            base.m_caption = "Review Mode Selector";
            base.m_message = "Review Mode Selector";
            base.m_toolTip = "Review Mode Selector";
            base.m_name = "ReviewModeSelector";
        }

        #region Overriden Class Methods

        /// <summary>
        /// Occurs when this command is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            comboHook = hook as IComboBoxHook;
            comboHook.Select(0);
            if (comboHook == null)
            {
                m_enabled = false;
                return;
            }

            m_application = comboHook.Hook as IApplication;

            int cookie = 0;

            foreach (string fileName in fileslist)
            {

                //Add item to list
                if (cookie == 0)
                {
                    cookie = comboHook.Add(fileName);
                    m_itemMap.Add(cookie, fileName);
                    comboHook.Select(cookie);
                }
                else
                {
                    cookie = comboHook.Add(fileName);
                    m_itemMap.Add(cookie, fileName);
                }
            }


        }

        /// <summary>
        /// Occurs when this command is clicked
        /// </summary>
        public override void OnClick()
        {

        }

        #endregion

        #region IComboBox Members

        public int DropDownHeight
        {
            get { return 50; }
        }

        public string DropDownWidth
        {
            get { return m_strWidth; }
        }

        public bool Editable
        {
            get { return false; }
        }

        public string HintText
        {
            get { return "Select Review Mode"; }
        }

        public void OnEditChange(string editString)
        {
        }
        public void OnEnter()
        {
        }

        public void OnFocus(bool set)
        {
        }
        IMxDocument pmxDocument;
        string pLayerName = null;
        IFeatureLayer player;
        IDataset pDataSet;
        private static bool selectionChanging = false;
        private static string _selectedMode = string.Empty;
        public void OnSelChange(int cookie)
        {
            try
            {
                if (selectionChanging) { return; }
                selectionChanging = true;

                if (_selectedMode.Equals(m_itemMap[cookie])) return;
                _selectedMode = m_itemMap[cookie];

                pmxDocument = (IMxDocument)m_application.Document;
                IMap map = pmxDocument.FocusMap;
                if (map.LayerCount != 0)
                {
                    System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
                    List<SelectedFeatureModel> selectedfeaturesList = null;
                    //Prepare Selection Set
                    if (map.FeatureSelection != null)
                    {
                        //Loop through layers and get the selection set
                        selectedfeaturesList = PrepareSelectedFeatures(map);
                        map.ClearSelection();

                    }

                    //Revert back the settings
                    if (_lstFieldCache != null &&
                        _lstFieldCache.Count > 0)
                    {
                        foreach (var item in _lstFieldCache)
                        {
                            ID8List settings = item as ID8List;
                            ID8ListItem setting;
                            settings.Reset();
                            while ((setting = settings.Next()) != null)
                            {
                                if (setting.ItemType == mmd8ItemType.mmitSimpleSetting
                                    && ((IMMSimpleSetting)setting).SettingType == mmFieldSettingType.mmFSVisible)
                                {

                                    IMMSimpleSetting fsettings = (IMMSimpleSetting)setting;
                                    if (fsettings != null)
                                    {
                                        fsettings.SettingValue = !fsettings.SettingValue;
                                    }
                                }
                            }
                        }
                        _lstFieldCache.Clear();
                    }
                    //clear mode settings
                    _featureClassCache.Clear();


                    //Do not load any mode settings if mode is all
                    if (!string.IsNullOrEmpty(_selectedMode) && !_selectedMode.ToLower().Equals("all"))
                    {
                        //Load Session Confi and apply mode settings
                        ApplyReviewMode(map, _selectedMode);
                        System.Windows.Forms.Cursor.Current = Cursors.Default;
                        m_application.StatusBar.set_Message(0, "Mode Setting finished");
                    }

                    if (selectedfeaturesList != null)
                    {
                        ReSelectFeatures(map, selectedfeaturesList);
                    }
                    //RefreshAttributeEditorSelections();
                    //refresh the map view
                    pmxDocument.ActiveView.Refresh();
                    pmxDocument.ActiveView.ContentsChanged();
                }

                //Ensure that the proper element is still selected. Bug in Esri causes it to revert if a user selects on another window during selection change
                comboHook.Select(cookie);
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
                _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
            finally
            {
                selectionChanging = false;
            }
        }

        private void ReSelectFeatures(IMap map, List<SelectedFeatureModel> selectedfeaturesList)
        {
            try
            {
                if (map != null)
                {
                    foreach (var layerSelection in selectedfeaturesList)
                    {
                        if (layerSelection.Features != null && layerSelection.Features.Count > 0)
                        {
                            IFeatureSelection featSelection = layerSelection.FeatureLayer as IFeatureSelection;
                            int[] refOIDList = layerSelection.Features.ToArray();
                            featSelection.SelectionSet.AddList(layerSelection.Features.Count, ref refOIDList[0]);
                            
                            //This doesn't seem to cause an update by itself to the arcfm attribute editor, so we'll select one feature using the map method.
                            featSelection.SelectionChanged();
                            map.SelectFeature(layerSelection.FeatureLayer, layerSelection.FeatureLayer.FeatureClass.GetFeature(layerSelection.Features[0]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// Obtains a list of where in clauses for a list of guids
        /// </summary>
        /// <param name="guids"></param>
        /// <returns></returns>
        private static List<string> GetWhereInClauses(List<int> guids)
        {
            try
            {
                List<string> whereInClauses = new List<string>();
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < guids.Count; i++)
                {
                    if ((((i % 1000) == 0) && i != 0) || (guids.Count == i + 1))
                    {
                        builder.Append(guids[i]);
                        whereInClauses.Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                    else { builder.Append(guids[i] + ","); }
                }
                return whereInClauses;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create guid where in clauses. Error: " + ex.Message);
            }
        }

        private List<SelectedFeatureModel> PrepareSelectedFeatures(IMap map)
        {
            List<SelectedFeatureModel> selectedFeatures = new List<SelectedFeatureModel>();
            try
            {
                List<int> lstfeatures = new List<int>();
                if (map != null && map.FeatureSelection != null)
                {
                    //Loop through layers
                    UID id = new UIDClass();
                    id.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; //IGeoFeatureLayer
                    //Loop through feature layers recursively
                    IEnumLayer enumLayer = map.get_Layers(id, true);
                    ILayer layer = enumLayer.Next();
                    while (layer != null)
                    {
                        IFeatureLayer fLayer = layer as IFeatureLayer;
                        if (fLayer != null)
                        {
                            lstfeatures = new List<int>();
                            IFeatureSelection fSelection = fLayer as IFeatureSelection;
                            if (fSelection != null && fSelection.SelectionSet != null && fSelection.SelectionSet.Count > 0)
                            {
                                IEnumIDs features = fSelection.SelectionSet.IDs;
                                int objectid = features.Next();
                                while (objectid != -1)
                                {
                                    lstfeatures.Add(objectid);
                                    objectid = features.Next();
                                }
                                selectedFeatures.Add(new SelectedFeatureModel
                                {
                                    FeatureLayer = fLayer,
                                    Features = lstfeatures
                                });
                            }
                        }
                        layer = enumLayer.Next();
                    }

                }
            }
            catch (Exception)
            {
            }

            return selectedFeatures;
        }



        private void ApplyReviewMode(IMap map, string selectedMode)
        {
            if (map != null)
            {

                UID id = new UIDClass();
                id.Value = "{E156D7E5-22AF-11D3-9F99-00C04F6BC78E}"; //IGeoFeatureLayer
                //Loop through feature layers recursively
                IEnumLayer enumLayer = map.get_Layers(id, true);
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    IFeatureLayer fLayer = layer as IFeatureLayer;
                    if (fLayer != null)
                    {
                        IFeatureClass fClass = fLayer.FeatureClass as IFeatureClass;
                        if (fClass != null)
                        {
                            IDataset ds = fClass as IDataset;
                            if (ds != null)
                            {
                                m_application.StatusBar.set_Message(0, "Processing " + ds.Name + " for mode settings");
                                GetPropertiesfromSessionConfig(ds.Name, selectedMode, fLayer.FeatureClass as IObjectClass);
                            }
                        }
                        ReleaseCOMObject(fClass);
                    }
                    layer = enumLayer.Next();
                }
                ReleaseCOMObject(layer);

                //Now look for the stand alone tables
                IStandaloneTableCollection tableCollection = ((IStandaloneTableCollection)map);
                for (int i = 0; i < tableCollection.StandaloneTableCount; i++)
                {
                    try
                    {
                        IStandaloneTable standaloneTable = tableCollection.get_StandaloneTable(i);
                        GetPropertiesfromSessionConfig(((IDataset)standaloneTable.Table).BrowseName, selectedMode, standaloneTable.Table as IObjectClass);
                    }
                    catch (Exception ex)
                    {
                        //Ignore any tables that fail.  These are likely tables with broken data connections
                    }
                }
            }
        }
        string _sessionConfigTableName = "EDGIS.sessionconfig";

        List<int> _featureClassCache = new List<int>();
        // Get properties from sessionconfig table for layer on map 
        public void GetPropertiesfromSessionConfig(string featureClassName, string selectedMode, IObjectClass oc)
        {
            //IObjectClass oc = fLayer.FeatureClass as IObjectClass;
            if (oc == null) return;
            if (_featureClassCache.Contains(oc.ObjectClassID)) return;// visible settings already applied 
            List<SessionConfigModel> lyrModeSettings = new List<SessionConfigModel>();
            IMMLogin2 mmLogin2 = m_application.FindExtensionByName("MMPropertiesExt") as IMMLogin2;
            if (mmLogin2 != null)
            {
                IFeatureWorkspace featureworkspace;
                featureworkspace = mmLogin2.LoginObject.LoginWorkspace as IFeatureWorkspace;
                if (featureworkspace != null)
                {
                    ITable mtable;
                    mtable = featureworkspace.OpenTable(_sessionConfigTableName);
                    if (mtable == null)
                    {
                        //Session config not availble in user schema - Try to load from PGE schema
                        _sessionConfigTableName = "PGE.sessionconfig";
                    }
                    //Open Session Config table
                    if (featureworkspace.OpenTable(_sessionConfigTableName) != null)
                    {
                        ICursor cur = null;
                        mtable = featureworkspace.OpenTable(_sessionConfigTableName);
                        IQueryFilter qFilter = new QueryFilterClass();
                        //Query Mode settings for the feauture classs
                        qFilter.WhereClause = "OBJECTCLASSRO= '" + featureClassName + "' and RMODE  = '" + selectedMode + "'";
                        if ((cur = mtable.Search(qFilter, true)) != null)
                        {
                            try
                            {
                                SessionConfigModel modeSetting = null;

                                for (IRow row = cur.NextRow(); row != null; row = cur.NextRow())
                                {
                                    try
                                    {
                                        //Load mode setting
                                        modeSetting = new SessionConfigModel();
                                        modeSetting.FeatureClassName = featureClassName;
                                        modeSetting.FieldName = row.get_Value(2) != null ? row.get_Value(2).ToString() : string.Empty;
                                        modeSetting.SubtypeCode = row.get_Value(4) != null ? int.Parse(row.get_Value(4).ToString()) : -1;
                                        modeSetting.Visible = row.get_Value(7) != null ? bool.Parse(row.get_Value(7).ToString()) : true;
                                        modeSetting.Mode = selectedMode;

                                        lyrModeSettings.Add(modeSetting);
                                    }
                                    catch (Exception ex)
                                    {
                                        //MessageBox.Show(ex.Message);
                                        _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
                                    }
                                }

                                //Apply featureclass settings based on mode selected
                                if (lyrModeSettings != null && lyrModeSettings.Count > 0)
                                {
                                    updateFieldSetting(oc, lyrModeSettings);
                                    _featureClassCache.Add(oc.ObjectClassID);
                                }

                            }
                            catch (Exception ex)
                            {
                                // MessageBox.Show(ex.Message);
                                _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);

                            }
                            finally
                            {
                                // code realese cursor 
                                ReleaseCOMObject(cur);
                            }
                        }
                    }
                    else
                    {
                        // table not availble 
                        MessageBox.Show("SessionConfig Table is not available");
                        return;
                    }
                }
                else
                {
                    // not logedin into arcfm
                    MessageBox.Show("Please Login in to ArcFM and try again");
                    return;
                }
            }

        }


        /// <summary>
        /// Helper method to change the visibility settings for the feature class in attribute editor
        /// </summary>
        /// <param name="lyrModeSettings"></param>
        private void updateFieldSetting(IObjectClass oc, List<SessionConfigModel> lyrModeSettings)
        {
            try
            {
                //validate parameters
                if (oc == null) return;
                if (lyrModeSettings == null || lyrModeSettings.Count == 0) return;
                IMMConfigTopLevel configTopLevel = (IMMConfigTopLevel)m_application.FindExtensionByName("ConfigTopLevel");
                IMMFeatureClass mmfc = ConfigTopLevel.Instance.GetFeatureClassOnly(oc);
                //Get the feature class name
                string featureClassName = mmfc.get_FeatureClassName().ToUpper();

                ID8List id8fc = mmfc as ID8List;
                id8fc.Reset();
                ID8ListItem listItem;
                listItem = id8fc.Next();
                while (listItem != null)
                {
                    //Loop though subtypes
                    if (listItem.ItemType == mmd8ItemType.mmitSubtype)
                    {
                        IMMSubtype mmsubtype = listItem as IMMSubtype;

                        if (mmsubtype != null)
                        {
                            //Verify mode available for this subtype
                            if (lyrModeSettings.Count(x => x.FeatureClassName.ToUpper().Equals(featureClassName) && x.SubtypeCode == mmsubtype.SubtypeCode) == 0)
                            {
                                listItem = id8fc.Next();
                                continue;
                            }
                            else
                            {

                                ID8List id8subtypes = mmsubtype as ID8List;
                                id8subtypes.Reset();
                                ID8ListItem id8Fields = id8subtypes.Next();
                                IMMField field;
                                while (id8Fields != null)
                                {
                                    //Apply setting to the Fields
                                    if (id8Fields.ItemType == mmd8ItemType.mmitField)
                                    {
                                        field = id8Fields as IMMField;
                                        if (field != null)
                                        {
                                            bool visible = true;
                                            //Check if this field name exists in the mode settings
                                            if (lyrModeSettings.Count(x => x.FeatureClassName.ToUpper().Equals(featureClassName)
                                                                            && x.SubtypeCode == mmsubtype.SubtypeCode
                                                                            && x.FieldName.ToUpper().Equals(field.FieldName.ToUpper())) > 0)
                                            {
                                                SessionConfigModel settingModel = lyrModeSettings.FirstOrDefault(x => x.FeatureClassName.ToUpper().Equals(featureClassName)
                                                                                && x.SubtypeCode == mmsubtype.SubtypeCode
                                                                                && x.FieldName.ToUpper().Equals(field.FieldName.ToUpper()));

                                                visible = settingModel.Visible;

                                            }
                                            else
                                            {
                                                //Keep the visibility of the field to false
                                                visible = false;
                                            }

                                            //Loop through field settings and apply visiblity 
                                            ID8List settings = field as ID8List;
                                            ID8ListItem setting;
                                            settings.Reset();
                                            while ((setting = settings.Next()) != null)
                                            {
                                                if (setting.ItemType == mmd8ItemType.mmitSimpleSetting
                                                    && ((IMMSimpleSetting)setting).SettingType == mmFieldSettingType.mmFSVisible)
                                                {

                                                    IMMSimpleSetting fsettings = (IMMSimpleSetting)setting;
                                                    if (fsettings != null)
                                                    {

                                                        if (visible != fsettings.SettingValue)
                                                        {
                                                            fsettings.SettingValue = !fsettings.SettingValue;
                                                            if (!_lstFieldCache.Contains(field))
                                                            {
                                                                //Keep a copy of field reference so that we can revert back its setting
                                                                _lstFieldCache.Add(field);
                                                            }
                                                        }
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                    }
                                    //Go to next field
                                    id8Fields = id8subtypes.Next();
                                }

                            }
                        }
                    }
                    //Go to next subtype within the featureclass
                    listItem = id8fc.Next();
                }
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
                _logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
        }

        List<IMMField> _lstFieldCache = new List<IMMField>();

        public bool ShowCaption
        {
            get { return false; }
        }

        public string Width
        {
            get { return m_strWidth; }
        }

        #endregion
        public int DisplayOrder
        {
            get;
            set;
        }
        private void ReleaseCOMObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    int refsLeft = 0;
                    do
                    {
                        refsLeft = Marshal.ReleaseComObject(obj);
                    }
                    while (refsLeft > 0);
                }
            }
            catch (Exception ex)
            {

                //MessageBox.Show(ex.Message);
                //_logger.Debug("Message From SessionChecker -" + ex.Message + " StackTrace:" + ex.StackTrace);
            }
        }
        public mmFieldSettingType SettingType { get; set; }
        public bool SettingValue { get; set; }
        public int FieldIndex { get; set; }
    }

    class SelectedFeatureModel
    {
        private IFeatureLayer fLayer;

        public IFeatureLayer FeatureLayer
        {
            get { return fLayer; }
            set { fLayer = value; }
        }
        private List<int> features;

        public List<int> Features
        {
            get { return features; }
            set { features = value; }
        }
    }

    class SessionConfigModel
    {
        private string featureClassName = string.Empty;

        public string FeatureClassName
        {
            get { return featureClassName; }
            set { featureClassName = value; }
        }
        private string fieldName = string.Empty;

        public string FieldName
        {
            get { return fieldName; }
            set { fieldName = value; }
        }
        private int subtypeCode = -1;

        public int SubtypeCode
        {
            get { return subtypeCode; }
            set { subtypeCode = value; }
        }
        private bool visible = true;

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }
        private string mode = string.Empty;

        public string Mode
        {
            get { return mode; }
            set { mode = value; }
        }
    }
}