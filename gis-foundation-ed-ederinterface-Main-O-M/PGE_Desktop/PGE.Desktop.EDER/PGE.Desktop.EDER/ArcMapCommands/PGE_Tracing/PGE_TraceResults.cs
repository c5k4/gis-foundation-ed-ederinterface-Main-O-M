using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.ArcMapUI;
using System.Runtime.InteropServices;
using Miner.Interop;
using PGE.Common.Delivery.Systems.Configuration;
using System.Text.RegularExpressions;
using System.Collections;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using Miner;

namespace PGE.Desktop.EDER.ArcMapCommands.PGE_Tracing
{
    public partial class PGE_TraceResults : Form
    {
        #region Private Vars

        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private Dictionary<string, IFeature> featureLookup = new Dictionary<string, IFeature>();
        private string[] excludeFeatureClasses = new string[0];
        private List<IFeatureClass> allFeatureClasses = new List<IFeatureClass>();
        private TraceType traceType = TraceType.Downstream;
        private bool saveLocationAndSize = false;
        private ArrayList protectiveClassIDs;
        private string zoomTo = "Zoom To";
        private string addToSelection = "Add to Selection";
        private List<string> menuItemsExcluded = new List<string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="geomNetwork">Geometric network that these results are based on</param>
        /// <param name="traceType">Type of trace executed</param>
        public PGE_TraceResults(IGeometricNetwork geomNetwork, TraceType traceType)
        {
            InitializeComponent();

            //Event handlers for our right click popup menu for zooming to and adding results to selection
            listBoxResults.ContextMenuStrip = new ContextMenuStrip();
            listBoxResults.ContextMenuStrip.Items.Add(zoomTo);
            listBoxResults.ContextMenuStrip.Items.Add(addToSelection);
            listBoxResults.ContextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(ContextMenuStrip_ItemClicked);
            
            //Set our trace type (Downstream, Upstream, etc)
            this.traceType = traceType;

            UserRegistry userRegistry = new UserRegistry("PGE\\Traces");
            bool traceSettings = userRegistry.Contains(traceType.ToString());
            if (!traceSettings)
            {
                SaveRegistryValue(traceType.ToString(), "");
            }

            //Event handlers for our list box control.
            listBoxResults.SelectedIndexChanged += new EventHandler(lstBoxResults_SelectedIndexChanged);

            //Set the initial size and location for our results dialog. This information is saved everytime a user
            //moves or resizes the control so they don't have to resize and move it every time
            SetSizeAndLocation();
            saveLocationAndSize = true;
            
            //Obtain the list of protective class IDs for the protective device traces
            if (protectiveClassIDs == null) { protectiveClassIDs = PGE_Trace.GetProtectiveClassIDs(); }

            //Build the list of features classes for the provided geometric network
            if (geomNetwork != null) { BuildFeatureClassList(geomNetwork); }
        }

        #endregion

        #region Event handlers

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        /// <summary>
        /// This method provides the functionality for the zoom to and add to selection commands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string itemName = "";
            try
            {
                itemName = e.ClickedItem.Text;
                IEnumerator selectedItems = listBoxResults.SelectedIndices.GetEnumerator();

                //Zoom To
                if (itemName == zoomTo)
                {
                    IEnvelope zoomToEnvelope = null;

                    //Build the envelope for all features that are selected in the list box
                    while (selectedItems.MoveNext())
                    {
                        string item = selectedItems.Current.ToString();
                        int selectedIndex = Int32.Parse(selectedItems.Current.ToString());

                        IFeature feat = featureLookup[listBoxResults.Items[selectedIndex].ToString()];
                        if (zoomToEnvelope == null) { zoomToEnvelope = feat.ShapeCopy.Envelope; }
                        else { zoomToEnvelope.Union(feat.ShapeCopy.Envelope); }
                    }

                    //Once the envelope is build we can zoom to the envelope in the same manner that ArcFM currently does
                    double zoomScale = 5.0;
                    IMMRegistry registry = new MMRegistry();
                    registry.OpenKey(mmHKEY.mmHKEY_CURRENT_USER, mmBaseKey.mmArcFM, "General");
                    try
                    {
                        zoomScale = Convert.ToDouble(registry.Read("Zoom To Buffer Size", 5));
                    }
                    catch
                    {
                        zoomScale = 5.0;
                    }
                    IMMMapUtilities mapUtils = new mmMapUtilsClass();
                    mapUtils.ZoomTo(zoomToEnvelope, ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap as IActiveView, zoomScale);
                }
                //Add to Selection
                else if (itemName == addToSelection)
                {
                    //Get the list of all IFeatureLayers in the map
                    string featureLayerUID = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    List<ILayer> featLayers = PGE_Trace.getFeatureLayers(featureLayerUID);

                    //Cycle through all of the selected features in our list box and select each feature into its respective IFeatureLayer
                    while (selectedItems.MoveNext())
                    {
                        string item = selectedItems.Current.ToString();
                        int selectedIndex = Int32.Parse(selectedItems.Current.ToString());

                        foreach (ILayer layer in featLayers)
                        {
                            IFeatureLayer featLayer = layer as IFeatureLayer;
                            if (featLayer.Visible)
                            {
                                IFeature feat = featureLookup[listBoxResults.Items[selectedIndex].ToString()];
                                if (feat.Class == featLayer.FeatureClass)
                                {
                                    ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap.SelectFeature(layer, featureLookup[listBoxResults.Items[selectedIndex].ToString()]);
                                }
                            }
                        }
                    }
                    //Refresh our map graphics to reflect the new selections
                    ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error executing " + itemName + ": " + ex.Message);
            }
        }

        /// <summary>
        /// Event handler to handle when a menu item is checked on or off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void newMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            UserRegistry userRegistry = new UserRegistry("PGE\\Traces");
            bool traceSettings = userRegistry.Contains(traceType.ToString());
            if (traceSettings)
            {
                //When the menu item is checked make sure the registry is updated to reflect the user settings for
                //feature classes to exclude
                if (menuItem.Checked && excludeFeatureClasses.Contains(menuItem.Text))
                {
                    List<string> excludeList = excludeFeatureClasses.ToList();
                    excludeList.Remove(menuItem.Text);
                    if (excludeList.Contains("")) { excludeList.Remove(""); }
                    excludeFeatureClasses = excludeList.ToArray();
                    SaveRegistryValue(traceType.ToString(), excludeFeatureClasses.Concatenate(","));
                    if (menuItemsExcluded.Contains(menuItem.Text)) { menuItemsExcluded.Remove(menuItem.Text); }
                    if (menuItemsExcluded.Count == 0) { includeFeaturesToolStripMenuItem.Text = "Include Features"; }
                }
                else if (!menuItem.Checked && !excludeFeatureClasses.Contains(menuItem.Text))
                {
                    List<string> excludeList = excludeFeatureClasses.ToList();
                    excludeList.Add(menuItem.Text);
                    if (excludeList.Contains("")) { excludeList.Remove(""); }
                    excludeFeatureClasses = excludeList.ToArray();
                    SaveRegistryValue(traceType.ToString(), excludeFeatureClasses.Concatenate(","));
                    if (!menuItemsExcluded.Contains(menuItem.Text)) { menuItemsExcluded.Add(menuItem.Text); }
                    includeFeaturesToolStripMenuItem.Text = "Include Features *";
                }
            }

            //Update our list box to only include those results which are not in our exclude feature classes list
            UpdateTreeList();

            
        }

        /// <summary>
        /// Whenever a new item is selected the feature should flash on the map
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lstBoxResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Only flash features if there is only one feature selected in the list box
            if (listBoxResults.SelectedItems.Count == 1)
            {
                if (featureLookup.ContainsKey(listBoxResults.SelectedItem.ToString()))
                {
                    IFeature feature = featureLookup[listBoxResults.SelectedItem.ToString()];
                    if (feature != null)
                    {
                        FlashFeature(((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).ActiveView, feature);
                    }
                }
            }
        }
        
        #endregion

        #region Overrides
        
        /// <summary>
        /// When the form is closed we should clear the tree of all cached features and clear the trace graphics on the map
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            ClearTree();
            ClearTraceGraphics();
            ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).ActiveView.Refresh();
        }

        /// <summary>
        /// Override default shown functionality
        /// </summary>
        /// <param name="e"></param>
        protected override void OnShown(EventArgs e)
        {
            //Update our list box tree when the form is first shown
            UpdateTreeList();

            base.OnShown(e);
        }

        /// <summary>
        /// Override default on move functionality
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);

            //When a user move the form, we need to save these preferences to the registry
            if (saveLocationAndSize)
            {
                SaveRegistryValue("Location", this.Location.X + "," + this.Location.Y);
            }
        }

        /// <summary>
        /// Override default on resize functionality
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //When the user resizes the form, we need to save these preferences to the registry
            if (saveLocationAndSize)
            {
                SaveRegistryValue("Size", this.Width + "," + this.Height);
            }

            //When the user resizes the form, we also need to resize all elements on the form as well to fit in the new size
            try
            {
                int newWidth = Convert.ToInt32(Math.Floor((double)this.ClientSize.Width * 0.96));
                int newHeight = Convert.ToInt32(Math.Floor((((double)this.ClientSize.Height - menuStrip1.Height) * 0.96)));

                Size newSize = new Size(newWidth, newHeight);
                listBoxResults.Size = newSize;
                listBoxResults.Location = new System.Drawing.Point((this.ClientSize.Width - listBoxResults.Width) / 2, ((this.ClientSize.Height - listBoxResults.Height) / 2) + (menuStrip1.Height / 2));
            }
            catch (Exception ex) 
            {
                _logger.Error("Error resizing PGE Trace results: " + ex.Message);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Clears any PGE Tracing graphics from the map
        /// </summary>
        private void ClearTraceGraphics()
        {
            IMap currentMap = ((IMxDocument)PGE.Common.Delivery.ArcFM.ApplicationFacade.Application.Document).FocusMap;
            if (currentMap != null)
            {
                ICompositeGraphicsLayer compositeGraphicsLayer = currentMap.ActiveGraphicsLayer as ICompositeGraphicsLayer;
                try
                {
                    List<string> toRemove = new List<string>();
                    ICompositeLayer compLayer = compositeGraphicsLayer as ICompositeLayer;
                    for (int i = 0; i < compLayer.Count; i++)
                    {
                        if (compLayer.get_Layer(i).Name.Contains("PGETraceGraphics"))
                        {
                            toRemove.Add(compLayer.get_Layer(i).Name);
                        }
                    }
                    foreach (string removeLayer in toRemove)
                    {
                        try
                        {
                            compositeGraphicsLayer.DeleteLayer(removeLayer);
                        }
                        catch (Exception e) { }
                    }
                }
                catch { /*Nothing to handle here*/}
            }
        }

        /// <summary>
        /// Parses through an enumration of feature classes and adds them to the list of menu items that will need to be added
        /// </summary>
        /// <param name="enumFeatClass">List of feature class</param>
        /// <param name="MenuItemsToAdd">List to add the aliasname strings to</param>
        private void BuildFeatureClassList(IEnumFeatureClass enumFeatClass, ref List<string> MenuItemsToAdd)
        {
            enumFeatClass.Reset();
            IFeatureClass featClass = null;
            while ((featClass = enumFeatClass.Next()) != null)
            {
                //For downstream and upstream we will add all of the feature classes for the network to our list
                if ((traceType == TraceType.Downstream || traceType == TraceType.Upstream) && !allFeatureClasses.Contains(featClass))
                {
                    allFeatureClasses.Add(featClass);
                    MenuItemsToAdd.Add(featClass.AliasName);
                }
                //For protective device traces we will only add those protective device feature classes as configured via ArcFM options
                else if (!allFeatureClasses.Contains(featClass))
                {
                    if (protectiveClassIDs.Contains(featClass.ObjectClassID))
                    {
                        allFeatureClasses.Add(featClass);
                        MenuItemsToAdd.Add(featClass.AliasName);
                    }
                    else
                    {
                        //If this classID isn't a protective device then we add it to the exclude feature classes list
                        UserRegistry userRegistry = new UserRegistry("PGE\\Traces");
                        bool traceSettings = userRegistry.Contains(traceType.ToString());
                        if (traceSettings)
                        {
                            excludeFeatureClasses = Regex.Split(userRegistry.GetSetting(traceType.ToString(), "").ToString(), ",");
                        }
                        else
                        {
                            SaveRegistryValue(traceType.ToString(), "");
                            excludeFeatureClasses = new string[0];
                        }
                        List<string> excludeList = excludeFeatureClasses.ToList();
                        excludeList.Add(featClass.AliasName);
                        if (excludeList.Contains("")) { excludeList.Remove(""); }
                        excludeFeatureClasses = excludeList.ToArray();
                        SaveRegistryValue(traceType.ToString(), excludeFeatureClasses.Concatenate(","));
                    }
                }
            }
        }

        /// <summary>
        /// Add the menu item with the specified text
        /// </summary>
        /// <param name="menuItemName">Menu item display name</param>
        private void AddMenuItem(string menuItemName)
        {
            ToolStripMenuItem newMenuItem = new ToolStripMenuItem(menuItemName);
            newMenuItem.CheckOnClick = true;

            //Get our trace settings so we know if this menu item should automatically be unchecked
            UserRegistry userRegistry = new UserRegistry("PGE\\Traces");
            bool traceSettings = userRegistry.Contains(traceType.ToString());
            if (traceSettings)
            {
                excludeFeatureClasses = Regex.Split(userRegistry.GetSetting(traceType.ToString(), "").ToString(), ",");
            }
            else
            {
                SaveRegistryValue(traceType.ToString(), "");
                excludeFeatureClasses = new string[0];
            }
            //If our exclude list includes this item then we uncheck it immediately
            if (excludeFeatureClasses.Contains(menuItemName))
            {
                newMenuItem.Checked = false;
                includeFeaturesToolStripMenuItem.Text = "Include Features *";
                menuItemsExcluded.Add(menuItemName);
            }
            else
            {
                newMenuItem.Checked = true;
            }

            //Event handler to handle when an item is checked on or off
            newMenuItem.CheckedChanged += new EventHandler(newMenuItem_CheckedChanged);

            //Add our new menu item
            includeFeaturesToolStripMenuItem.DropDownItems.Add(newMenuItem);
        }

        /// <summary>
        /// Saves the specified value to the key specified (All to PGE\Traces registry key)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void SaveRegistryValue(string key, string value)
        {
            //Read the config location from the Registry Entry
            UserRegistry userRegistry = new UserRegistry("PGE\\Traces");
            userRegistry.SetSetting(key, value);
            userRegistry.Save();
        }

        /// <summary>
        /// Applies the last user specified size and location of the form
        /// </summary>
        private void SetSizeAndLocation()
        {
            try
            {
                //Read the config location from the Registry Entry
                UserRegistry userRegistry = new UserRegistry("PGE\\Traces");

                //Obtain the previous size specified for the form from the registry and apply that size
                bool lastSize = userRegistry.Contains("Size");
                if (lastSize)
                {
                    string[] size = Regex.Split(userRegistry.GetSetting("Size", "").ToString(), ",");
                    if (size.Count() == 2)
                    {
                        Size newSize = new Size();
                        newSize.Width = Int32.Parse(size[0]);
                        newSize.Height = Int32.Parse(size[1]);
                        if (this.Width != newSize.Width || this.Height != newSize.Height)
                        {
                            this.Size = newSize;
                        }
                    }
                }

                //Obtain the last specified position of the form from the registry and apply that position
                bool lastPosition = userRegistry.Contains("Location");
                if (lastPosition)
                {
                    string[] position = Regex.Split(userRegistry.GetSetting("Location", "").ToString(), ",");
                    if (position.Count() == 2)
                    {
                        System.Drawing.Point newLocation = new System.Drawing.Point();
                        newLocation.X = Int32.Parse(position[0]);
                        newLocation.Y = Int32.Parse(position[1]);
                        if (this.Location.X != newLocation.X || this.Location.Y != newLocation.Y)
                        {
                            bool existsOnScreen = false;
                            //Check first if this location exists on a screen. This will keep the window from opening on a
                            //non existent second screen.
                            Screen[] screens = Screen.AllScreens;
                            foreach (Screen screen in screens)
                            {
                                Rectangle formRectangle = new Rectangle(newLocation, this.Size);
                                if (screen.WorkingArea.IntersectsWith(formRectangle))
                                {
                                    existsOnScreen = true;
                                }
                            }
                            if (existsOnScreen) { this.Location = newLocation; }
                            else { this.StartPosition = FormStartPosition.CenterScreen; }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error("Error setting size and location for PGE Trace Results: " + e.Message);
            }
        }

        /// <summary>
        /// Clears the contents of the featureLookup dictionary and clears the list box list
        /// </summary>
        private void ClearTree()
        {
            try
            {
                foreach (KeyValuePair<string, IFeature> kvp in featureLookup)
                {
                    while (Marshal.ReleaseComObject(kvp.Value) > 0) { }
                }
                featureLookup.Clear();

                listBoxResults.Items.Clear();
            }
            catch (Exception e)
            {
                _logger.Error("Error clearing PGE trace results: " + e.Message);
            }
        }

        /// <summary>
        /// Determines if a feature class should currently be included based on the feature classes specified
        /// in the exclude features list
        /// </summary>
        /// <param name="featClass"></param>
        /// <returns></returns>
        private bool shouldInclude(IFeatureClass featClass)
        {
            if (excludeFeatureClasses.Contains(featClass.AliasName)) { return false; }
            return true;
        }

        /// <summary>
        /// Flashes the specified feature on the specified active view
        /// </summary>
        /// <param name="_activeView"></param>
        /// <param name="_feature"></param>
        private void FlashFeature(IActiveView _activeView, IFeature _feature)
        {
            try
            {
                IActiveView activeView = _activeView;
                IFeatureIdentifyObj featIdentify = new FeatureIdentifyObject();
                featIdentify.Feature = _feature;
                IIdentifyObj identify = featIdentify as IIdentifyObj;
                identify.Flash(activeView.ScreenDisplay);
            }
            catch (Exception e)
            {
                _logger.Error("Error flashing feature on map: " + e.Message);
            }
        }

        /// <summary>
        /// Build the list of feature classes that exist for a given geometric network.  This allows us to add them
        /// to the include features drop down list for users to turn them on or off
        /// </summary>
        /// <param name="geomNetwork"></param>
        private void BuildFeatureClassList(IGeometricNetwork geomNetwork)
        {
            //We need to parse through every possible esriFeatureType to get a list of all of the feature classes in the network
            List<string> menuItemsToAdd = new List<string>();
            IEnumFeatureClass featClasses = geomNetwork.get_ClassesByType(esriFeatureType.esriFTComplexEdge);
            BuildFeatureClassList(featClasses, ref menuItemsToAdd);
            featClasses = geomNetwork.get_ClassesByType(esriFeatureType.esriFTComplexJunction);
            BuildFeatureClassList(featClasses, ref menuItemsToAdd);
            featClasses = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleEdge);
            BuildFeatureClassList(featClasses, ref menuItemsToAdd);
            featClasses = geomNetwork.get_ClassesByType(esriFeatureType.esriFTSimpleJunction);            
            BuildFeatureClassList(featClasses, ref menuItemsToAdd);
            //**************************************IMPORTANT NOTES - Sep 2016 Release**********************************************
            //***************Added below code for Removal of EDGIS.SecondaryGenration features from PGE Trace results*******
            //*******This hot fix will be reverted once EDGIS.SecondaryGeneration feature class is removed from Geometric Network in future release*******
            
            if(!menuItemsToAdd.Remove("Secondary Generation"))
                menuItemsToAdd.Remove("EDGIS.SecondaryGeneration");

            //**************************************End**********************************************


            //Now that we have our list we can add the menu items
            menuItemsToAdd.Sort();

            foreach (string menuItem in menuItemsToAdd)
            {
                AddMenuItem(menuItem);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the title of the trace results window
        /// </summary>
        /// <param name="title"></param>
        public void SetTraceTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Adds a new entry to the featureLookup dictionary to cache features
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="feature"></param>
        public void AddTreeNode(string nodeName, IFeature feature)
        {
            try
            {
                featureLookup.Add(nodeName, feature);
            }
            catch (Exception e)
            {
                _logger.Error("Unable to add feature to PGE Trace Results: " + e.Message);
            }
        }

        /// <summary>
        /// Updates the items in the list box list based on whether it should be shown with current settings
        /// </summary>
        public void UpdateTreeList()
        {
            listBoxResults.Items.Clear();
            foreach (KeyValuePair<string, IFeature> kvp in featureLookup)
            {
                if (shouldInclude(kvp.Value.Class as IFeatureClass))
                {
                    //**************************************IMPORTANT NOTES - Sep 2016 Release**********************************************
                    //***************Added below code for Removal of EDGIS.SecondaryGenration features from PGE Trace results*******
                    //*******This hot fix will be reverted once EDGIS.SecondaryGeneration feature class is removed from Geometric Network in future release*******
                    if ((kvp.Value.Class as ESRI.ArcGIS.Geodatabase.IDataset).Name.ToUpper() != "EDGIS.SECONDARYGENERATION")
                        listBoxResults.Items.Add(kvp.Key);
                }
            }
        }
        
        #endregion
    }
}
