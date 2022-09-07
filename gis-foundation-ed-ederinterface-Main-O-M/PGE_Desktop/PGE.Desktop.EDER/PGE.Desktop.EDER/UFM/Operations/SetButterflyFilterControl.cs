using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;

using Miner.Framework;
using Miner.FrameworkUI;
using Miner.Geodatabase;
using Miner.Interop;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.UI.Tools;

namespace PGE.Desktop.EDER.UFM.Operations
{
    public partial class SetButterflyFilterControl : OperationUIControl
    {
        #region Member vars

        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Currently running ArcMap instance
        /// </summary>
        private IApplication _application = null;

        /// <summary>
        /// Current map on display
        /// </summary>
        private IMap _map = null;

        #endregion

        #region Constructor

        public SetButterflyFilterControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Initialize control

        private void SetButterflyFilterControl_Load(object sender, EventArgs e)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            // Store the supplied map doco
            // Store the application
            Type appRefType = Type.GetTypeFromProgID("esriFramework.AppRef");
            object appRefObj = Activator.CreateInstance(appRefType);
            _application = appRefObj as IApplication;

            // And the map
            IMxDocument mxDocument = _application.Document as IMxDocument;
            IActiveView activeView = mxDocument.FocusMap as IActiveView;
            _map = mxDocument.FocusMap as IMap;

            // Populate the dropdown
            //PopulateCombo();

            //Control parent = this.Parent;
            Form parent = this.FindForm();
            if (parent != null)
            {
                parent.Activated += new EventHandler(this.Control_Activated);
                //AddHandler parent.Activated, AddressOf Control_Activated
            }

            // Invoke some code when somebody selects a vault
            //cboVaults.SelectedValueChanged += new EventHandler(cboVaults_SelectedValueChanged);
        }

        #endregion

        private void Control_Activated(object sender, EventArgs e)
        {
            // Populate the dropdown
            PopulateCombo();
        }


        #region Control event handlers

        private void cboVaults_DropDown(object sender, EventArgs e)
        {
            PopulateCombo();
        }

        private void PopulateCombo()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            try
            {
                IMxDocument mxDocument = _application.Document as IMxDocument;
                IActiveView activeView = mxDocument.FocusMap as IActiveView;
                _map = mxDocument.FocusMap as IMap;

                // Get the vault feature class
                IWorkspace ws = UfmHelper.GetWorkspace();
                IFeatureClass fcSubs = ModelNameFacade.FeatureClassByModelName(ws, SchemaInfo.Electric.ClassModelNames.SubSurfaceStructure);

                // Lets cap this operation based on scale - if were beyond 1:3000, turning on/off UFM's is meaningless
                if (_map.MapScale < 3000)
                {
                    // Search all subsurface structures
                    ISpatialFilter qf = new SpatialFilterClass();
                    qf.Geometry = (_map as IActiveView).Extent;
                    qf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    qf.GeometryField = "SHAPE";
                    IFeatureCursor cursorSubs = fcSubs.Search(qf, false);

                    // Create an empty list of vaults
                    IList<VaultItem> vaults = new List<VaultItem>();
                    vaults.Add(new VaultItem(string.Empty, string.Empty));

                    // Make sure everything is configured correctly
                    int structFieldIndex = ModelNameFacade.FieldIndexFromModelName(fcSubs as IObjectClass, SchemaInfo.UFM.FieldModelNames.StructureNumber);
                    int facIdFieldIndex = ModelNameFacade.FieldIndexFromModelName(fcSubs as IObjectClass, SchemaInfo.UFM.FieldModelNames.FacilityId);

                    // If both model names were assigned...
                    if (structFieldIndex > 0 && facIdFieldIndex > 0)
                    {
                        // For each vault found, add its number to the vault combo
                        IFeature sub = cursorSubs.NextFeature();

                        while (sub != null)
                        {
                            // Get the display value (StructureNumber)
                            string displayVal = string.Empty;
                            if (sub.get_Value(structFieldIndex) != null)
                            {
                                displayVal = sub.get_Value(structFieldIndex).ToString();
                            }

                            // Get the unique value (globalID - indicated by FACILITYID model name)
                            string valueVal = sub.get_Value(facIdFieldIndex).ToString();

                            // Add to the list of vaults and move to the next
                            vaults.Add(new VaultItem(displayVal, valueVal));
                            sub = cursorSubs.NextFeature();
                        }
                    }

                    // Map the list of vaults to the combo box
                    cboVaults.DataSource = vaults;
                    cboVaults.DisplayMember = "VaultNo";
                    cboVaults.ValueMember = "GlobalId";
                }
            }
            catch (Exception ex)
            {
                // Log and return null
                _logger.Error("Failed to load vault drop down: " + ex.ToString());
            }
        }

        private void butRefresh_Click(object sender, EventArgs e)
        {
            butReset_Click(sender, e);
            PopulateCombo();
        }

        private void butReset_Click(object sender, EventArgs e)
        {
            // Clear the filter
            VaultFilter.FilteredVault = string.Empty;

            // Find all layers with the UFMVisible model name applied
            IList<IFeatureLayer> layers = ModelNameFacade.FeatureLayerByModelName("UFMVISIBLE", _map, true);

            // For each layer...
            foreach (IFeatureLayer layer in layers)
            {
                // Clear the facility ID filter if present
                IFeatureLayerDefinition layerDef = layer as IFeatureLayerDefinition;
                ClearExpression(layerDef);
            }

            // Reset the combo box and refresh the map
            cboVaults.Text = string.Empty;
            (_map as IActiveView).Refresh();

            PopulateCombo();
        }

        #endregion

        #region Private methods

        private void cboVaults_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            try
            {
                // Find all layers with the UFMVisible model name applied
                IList<IFeatureLayer> layers = ModelNameFacade.FeatureLayerByModelName("UFMVISIBLE", _map, true);

                // For each layer...
                foreach (IFeatureLayer layer in layers)
                {
                    // As long as were not using floors (floors are on all the time)
                    IFeatureClass fc = layer.FeatureClass;
                    if (ModelNameFacade.ContainsClassModelName(fc as IObjectClass, SchemaInfo.UFM.ClassModelNames.UfmFloor) != true)
                    {
                        // Update the layer definition
                        if (ModelNameFacade.FieldIndexFromModelName(fc as IObjectClass, SchemaInfo.UFM.FieldModelNames.FacilityId) > 0)
                        {
                            IFeatureLayerDefinition layerDef = layer as IFeatureLayerDefinition;
                            if ((cboVaults.SelectedItem as VaultItem).VaultNo == string.Empty)
                            {
                                // Clear the expression
                                ClearExpression(layerDef);
                            }
                            else
                            {
                                // Add to the existing definition query
                                if (layerDef.DefinitionExpression == string.Empty)
                                {
                                    // Replace the expression
                                    layerDef.DefinitionExpression = "FACILITYID='" + (cboVaults.SelectedItem as VaultItem).GlobalId + "'";
                                }
                                else
                                {
                                    // If a FACILITYID expression is already present
                                    if (layerDef.DefinitionExpression.Contains("FACILITYID=") == true)
                                    {
                                        // replace it
                                        int index = layerDef.DefinitionExpression.IndexOf("FACILITYID=");
                                        layerDef.DefinitionExpression = layerDef.DefinitionExpression.Substring(0, index) + "FACILITYID='" + (cboVaults.SelectedItem as VaultItem).GlobalId + "'";
                                    }
                                    else
                                    {
                                        // Append the expression
                                        layerDef.DefinitionExpression = layerDef.DefinitionExpression + " AND FACILITYID='" + (cboVaults.SelectedItem as VaultItem).GlobalId + "'";
                                    }
                                }
                            }
                        }
                    }
                }

                // Refresh the map
                (_map as IActiveView).Refresh();
            }
            catch (Exception ex)
            {
                _logger.Warn("Failed to filter vaults: " + ex.ToString());
            }
            finally
            {
                VaultFilter.FilteredVault = (cboVaults.SelectedItem as VaultItem).GlobalId;
            }
        }

        private void ClearExpression(IFeatureLayerDefinition layerDef)
        {
            // If the expression was 'AND'ed
            if (layerDef.DefinitionExpression.Contains("FACILITYID=") == true)
            {
                if (layerDef.DefinitionExpression.Contains(" AND FACILITYID=") == true)
                {
                    // Do some funky stuff
                    int index = layerDef.DefinitionExpression.IndexOf(" AND FACILITYID=");
                    layerDef.DefinitionExpression = layerDef.DefinitionExpression.Substring(0, index);
                }
                else
                {
                    // Just clear the expression
                    layerDef.DefinitionExpression = string.Empty;
                }
            }
        }

        #endregion
    }

    #region VaultItem class for combobox items

    public class VaultItem
    {
        private string _vaultNo;
        private string _globalId;

        public VaultItem(string vaultNo, string globalId)
        {
            _vaultNo = vaultNo;
            _globalId = globalId;
        }

        public string VaultNo
        {
            get { return _vaultNo; }
        }

        public string GlobalId
        {
            get { return _globalId; }
        }
    }

    #endregion
}
