using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.CatalogUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.UI.Commands;
using PGE.Desktop.EDER.ArcCatalogCommands.SDPropSync.UC;
using Miner.Geodatabase;

namespace PGE.Desktop.EDER.ArcCatalogCommands.SDPropSync
{
    /// <summary>
    /// ToolControl implementation
    /// </summary>
    [Guid("FA96B77E-982B-4DF0-95E2-BEF182836150")]
    [ComponentCategory(ComCategory.ArcCatalogCommands)]
    [ProgId("PGE.Desktop.EDER.UpdateFieldOrderCommand")]
    [ComVisible(true)]
    public class SDPropSyncCommand : BaseGxCommand, IDisposable
    {
        #region Com Reg Methods

        /// <summary>
        /// Registers the specified reg key.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComRegisterFunction]
        private static void Register(string regKey)
        {
            Miner.ComCategories.ArcCatalogCommands.Register(regKey);
        }
        /// <summary>
        /// Uns the register.
        /// </summary>
        /// <param name="regKey">The reg key.</param>
        [ComUnregisterFunction]
        private static void UnRegister(string regKey)
        {
            Miner.ComCategories.ArcCatalogCommands.Unregister(regKey);
        }

        #endregion

        #region Private Members

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private IGxApplication _gxApp;
        private IGxObject _gxObj;
        private ESRI.ArcGIS.Framework.IApplication _app;
        
        
        //private PropChooser uc;
        private bool aliases, visibility, order;
        private HashSet<string> visibilityFields=null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SDPropSyncCommand"/> class.
        /// </summary>
        public SDPropSyncCommand() :
            base("Sync SD Field Properties", "SD PropSync", "PGE Tools", "Sync Field Properties of Stored Display(s).", "Sync SD Field Properties") 
        { }

        #endregion

        #region Overridden BaseGxCommand Method
        /// <summary>
        /// Called when the command is created inside the application.
        /// </summary>
        /// <param name="hook">A reference to the application in which the command was created.
        /// The hook may be an IApplication reference (for commands created in ArcGIS Desktop applications)
        /// or an IHookHelper reference (for commands created on an Engine ToolbarControl).</param>
        public override void OnCreate(object hook)
        {
            if (hook is IGxApplication)
            {
                _gxApp = (IGxApplication)hook;
                _app = (ESRI.ArcGIS.Framework.IApplication)hook;
            }
        }

        /// <summary>
        /// Overridden Property to determine if command will be in enabled mode or not.
        /// </summary>
        public override bool Enabled
        {
            get {
                 //Enable if database is connected.
                _gxObj = _gxApp.SelectedObject;

                if (_gxObj is IGxDatabase)
                {
                    return (_gxObj as IGxDatabase).IsConnected;
                }
                return false;

            }
        }
        
        /// <summary>
        /// Display the Update Field Order form. 
        /// </summary>
        protected override void InternalClick()
        {
            //showing the UI.
            using (var uc = new PropChooser())
            {
                //User selected the options and clicked on the OK button in UI
                if (uc.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    aliases = uc.Aliases;
                    visibility = uc.Visibility;
                    order = uc.Order;
                    if (visibility) visibilityFields = new HashSet<string>(uc.VisibilityFields.Split(','));
                    ProcessActions();
                }
            }
            
        } 

        private void ProcessActions()
        {
            // Delegate function to get the list of storeddisplayname as per the parameter value mmStoredDisplayType
            Func<IMMStoredDisplayManager, mmStoredDisplayType, IList<IMMStoredDisplayName>>
                GetStoredDisplayNames = (storedDislayManager, storedDisplayType) =>
                {
                    IList<IMMStoredDisplayName> list = new List<IMMStoredDisplayName>();

                    IMMStoredDisplayName mmstoredDisplayName = null;
                    //Gets the stored display
                    IMMEnumStoredDisplayName storedDisplayNames = storedDislayManager.GetStoredDisplayNames(storedDisplayType);
                    storedDisplayNames.Reset();


                    while ((mmstoredDisplayName = storedDisplayNames.Next()) != null)
                    {
                        list.Add(mmstoredDisplayName);

                    }
                    return list;
                };
            //Get the stored display manager class

            IMMStoredDisplayManager storedDispMgr = new MMStoredDisplayManagerClass();
            storedDispMgr.Workspace = (_gxObj as IGxDatabase).Workspace;

            //Getting the storeddisplay based on system and user.
            IList<IMMStoredDisplayName> listSystemStoredDisplayName = GetStoredDisplayNames(storedDispMgr, mmStoredDisplayType.mmSDTSystem);
            IList<IMMStoredDisplayName> listUserStoredDisplayName = GetStoredDisplayNames(storedDispMgr, mmStoredDisplayType.mmSDTUser);
            _logger.Debug("Total system stored display count = " + listSystemStoredDisplayName.Count);
            _logger.Debug("Total user stored display count = " + listUserStoredDisplayName.Count);
            //checking if any system or user has the storeddisplay
            if (listSystemStoredDisplayName.Count > 0 || listUserStoredDisplayName.Count > 0)
            {
                //showing the UI.
                using (var ugReOrderFieldsForm = new Re_Order_Fields(listSystemStoredDisplayName, listUserStoredDisplayName))
                {
                    //User selected the stored display and clicked on the OK button in UI
                    if (ugReOrderFieldsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {

                        //get selected storeddisplay.
                        //Getting the selected stored display.
                        IList<IMMStoredDisplayName> selectedListSystemStoredDisplayName = ugReOrderFieldsForm.GetSelectedStoredDisplay(mmStoredDisplayType.mmSDTSystem);
                        IList<IMMStoredDisplayName> selectedListUserStoredDisplayName = ugReOrderFieldsForm.GetSelectedStoredDisplay(mmStoredDisplayType.mmSDTUser);
                        _logger.Debug("Total selected system stored display count = " + selectedListSystemStoredDisplayName.Count);
                        _logger.Debug("Total selected user stored display count = " + selectedListUserStoredDisplayName.Count);
                        int totalStoredDisplayCount = selectedListSystemStoredDisplayName.Count + selectedListUserStoredDisplayName.Count;
                        if (totalStoredDisplayCount < 1) return;

                        IStatusBar stbar = _app.StatusBar;
                        IStepProgressor sProgressor = stbar.ProgressBar;
                        sProgressor.Position = 0;
                        sProgressor.MaxRange = totalStoredDisplayCount;
                        sProgressor.Message = "Update progress:";
                        sProgressor.StepValue = 1;
                        sProgressor.Show();
                        try
                        {
                            //getting the Config Top Level
                            IExtensionManager extensionMan = Activator.CreateInstance(Type.GetTypeFromProgID("esriSystem.ExtensionManager")) as IExtensionManager;
                            IMMConfigTopLevel configTopLevel = (IMMConfigTopLevel)extensionMan.FindExtension("ConfigTopLevel");
                            _logger.Debug("Loop for each system stored display");
                            //updating the selected system stored display
                            foreach (IMMStoredDisplayName selectedSystemStoredDisplayName in selectedListSystemStoredDisplayName)
                            {

                                //Update the field order and save back to database.
                                UpdateFieldOrder(sProgressor, configTopLevel, selectedSystemStoredDisplayName, storedDispMgr);
                                sProgressor.Step();
                            }
                            _logger.Debug("Loop for each user stored display");
                            //updating the selected user stored display
                            foreach (IMMStoredDisplayName selectedUserStoredDisplayName in selectedListUserStoredDisplayName)
                            {
                                //Update the field order and save back to database.
                                UpdateFieldOrder(sProgressor, configTopLevel, selectedUserStoredDisplayName, storedDispMgr);
                                sProgressor.Step();
                            }
                        }
                        catch { throw; }
                        finally { sProgressor.Hide(); }
                    }
                    //ugReOrderFieldsForm
                }
            }
        }

        #endregion Overridden BaseGxCommand Method

        #region Private method/functions
        /// <summary>
        /// This method will update the featurelayer field order as per toplevel config and save back to database.
        /// </summary>
        /// <param name="selectedListSystemStoredDisplayName">Selected system stored display name</param>
        /// <param name="selectedListUserStoredDisplayName">Selected user stored display name</param>
        /// <param name="storedDispMgr">Stored Display Manager</param>
        private void UpdateFieldOrder(IStepProgressor sProgressor,IMMConfigTopLevel configTopLevel, IMMStoredDisplayName selectedStoredDisplayName, IMMStoredDisplayManager storedDispMgr)
        {
            //getting the unopened storeddisplay
            IMMStoredDisplay storedDisplay = storedDispMgr.GetUnopenedStoredDisplay(selectedStoredDisplayName);
            sProgressor.Message = "Opening storeddisplay " + selectedStoredDisplayName.Name;
            ESRI.ArcGIS.Carto.IMap map = storedDisplay.Map;
            //Getting all featurelayer in the map
            IEnumLayer enumLayer = map.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
            IFeatureLayer featLayer = null;
            ILayer layer;
            _logger.Debug("Loop for each feature layer in the stored display = " + selectedStoredDisplayName.Name);
            while ((layer = enumLayer.Next()) != null)//loop through each layer
            {
                sProgressor.Message = "On layer [" + layer.Name + "], storeddisplay [" + selectedStoredDisplayName.Name + "]";
                _logger.Debug("Feature layer name = " + layer.Name);
                featLayer = layer as IFeatureLayer;
                IFeatureClass fClass = featLayer.FeatureClass;
                //getting the mmsubtype for all subtypecode.
                IMMSubtype mmSubType = configTopLevel.GetSubtypeForEdit(fClass, -1);
                _logger.Debug("Getting field ordered from arcfm");
                //store the fieldname with ordering index.
                Dictionary<string, int> fieldOrderMapping = GetFieldOrder(mmSubType);
                _logger.Debug("Total field order count = " + fieldOrderMapping.Count);

                //set field order to esri layer.
                if (order)
                {
                    aliases = true;
                    _logger.Debug("setting field order and properties");
                }
                else
                {
                    _logger.Debug("setting field properties only, no ordering");
                }

                MatchSDPropsWithArcFM(featLayer, fieldOrderMapping);
            }
            //storing the storeddisplay back to database.
            storedDispMgr.CreateStoredDisplay(map, selectedStoredDisplayName, true);
        }

        private void MatchSDPropsWithArcFM(IFeatureLayer featureLayer, Dictionary<string, int> mmFieldOrder)
        {
            //Changed from List to Dictionary to hold the field name along with FieldInfo
            Dictionary<string, ESRI.ArcGIS.Carto.FieldInfo> fieldInfos = new Dictionary<string, ESRI.ArcGIS.Carto.FieldInfo>();

            IOrderedLayerFields orderedLayerFields = featureLayer as IOrderedLayerFields;
            IFieldInfoSet fieldInfoSet = orderedLayerFields.FieldInfos;
            IFieldInfo referenceFieldInfo;
            
            foreach (KeyValuePair<string, int> kv in mmFieldOrder)
            {
                try
                {
                    string fieldName = kv.Key;
                    referenceFieldInfo = fieldInfoSet.Find(fieldName);

                    if (aliases)
                    {
                        int fieldIndex = featureLayer.FeatureClass.FindField(fieldName);
                        string esriFieldAlias = featureLayer.FeatureClass.Fields.get_Field(fieldIndex).AliasName;

                        if (!referenceFieldInfo.Alias.Equals(esriFieldAlias))
                        {
                            _logger.Debug("Old field name [" + referenceFieldInfo.Alias
                                + "] on feature class [" + featureLayer.FeatureClass.AliasName
                                + "] being changed to new field name [" + esriFieldAlias + "]");
                            referenceFieldInfo.Alias = esriFieldAlias;
                        }
                    }

                    if (visibility)
                    {
                        SetFieldVisibility(featureLayer, referenceFieldInfo, kv.Value);
                    }

                    if (order)
                    {
                        ESRI.ArcGIS.Carto.FieldInfo fieldInfo = new ESRI.ArcGIS.Carto.FieldInfo();
                        fieldInfo.Alias = referenceFieldInfo.Alias;
                        fieldInfo.Visible = referenceFieldInfo.Visible;
                        fieldInfo.NumberFormat = referenceFieldInfo.NumberFormat;

                        fieldInfos.Add(fieldName, fieldInfo);
                    }
                }
                catch (Exception e)
                {
                    _logger.Debug("Joined field found on feature layer [" + featureLayer.Name + "]. Ignoring field."
                        + " Please remove join to re-order fields in this feature layer.");
                }
            }

            if (order)
            {
                // Setting the mmFieldOrder in fieldInfoSet to order the fields in feature layer.
                fieldInfoSet.Clear();

                foreach (KeyValuePair<string, ESRI.ArcGIS.Carto.FieldInfo> kp in fieldInfos)
                {
                    //Ravi - IFieldInfoSet.Add takes FieldName and IFieldInfo to add the field to the Layer. Otherwise it changes the field order but the Alias names are messed up.
                    fieldInfoSet.Add(kp.Key, kp.Value);
                }
                _logger.Debug("Calling SetFieldOrder");

                if (fieldInfoSet.Count > 0)
                {
                    orderedLayerFields.SetFieldOrder(fieldInfoSet);
                }
            }
        }

        private void SetFieldVisibility(IFeatureLayer featureLayer, IFieldInfo referenceFieldInfo, int fieldIndex)
        {
            IMMFeatureClass fc = ConfigTopLevel.Instance.GetFeatureClassOnly(featureLayer.FeatureClass);
            IMMField arcFMField = null;
            ID8List id8Subtypes = fc as ID8List;
            id8Subtypes.Reset();
            ID8List id8Fields = id8Subtypes.Next() as ID8List;
            id8Fields.Reset();

            while (fieldIndex-- > -1) arcFMField = id8Fields.Next() as IMMField;

            if (visibilityFields.Contains(arcFMField.FieldName))
            {
                ID8List settings = arcFMField as ID8List;
                ID8ListItem setting;
                settings.Reset();

                while ((setting = settings.Next()) != null)
                {
                    if (setting.ItemType == mmd8ItemType.mmitSimpleSetting
                        && ((IMMSimpleSetting)setting).SettingType == mmFieldSettingType.mmFSVisible)
                    {
                        referenceFieldInfo.Visible = ((IMMSimpleSetting)setting).SettingValue;
                    }
                    else continue;
                }
            }
        }

        /// <summary>
        /// This function will return the toplevelconfig field order based on mmSubTYpe
        /// </summary>
        /// <param name="mmSubType">mmSubType</param>
        /// <returns>Toplevel config field order in the Disctionary</returns>
        private Dictionary<string, int> GetFieldOrder(IMMSubtype mmSubType)
        {
            
            //Converting the mmSubtype to ID8List to get the properties.
            ID8List subTypeList = mmSubType as ID8List;
            subTypeList.Reset();
            ID8ListItem item = null;
            
            Dictionary<string, int> fieldOrderMapping = new Dictionary<string, int>();
            int i = 0;
            // loop through each ID8ListIteam
            while ((item = subTypeList.Next(true)) != null)
            {
                if (item.ItemType == mmd8ItemType.mmitField)
                {
                   // adding ordered field in dictionary.
                    IMMField mmField = item as IMMField;
                    fieldOrderMapping.Add(mmField.FieldName, i);
                }
                i++;
            }

            return fieldOrderMapping;
        }

        #endregion

        public void Dispose()
        {
            
        }
    }
}
