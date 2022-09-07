using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;

using Miner.ComCategories;
using Miner.Interop;

using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ArcMapCommands.UFMFilter.UC
{
    [ComVisible(true)]
    [Guid("38E5D68D-BBF0-4ECB-986C-56C86C819F05")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComponentCategory(ComCategory.ArcMapCommands)]
    public class UfmFilter : UserControl
    {
        #region Member vars

        // For map refreshes
        private IMap _map = null;
        private IApplication _application = null;

        // For error handling
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        #endregion

        #region Control vars

        private Label lblVault;
        private System.Windows.Forms.Button butClear;

        #endregion

        #region Controls

        private ComboBox cboVaults;

        private void InitializeComponent()
        {
            this.cboVaults = new System.Windows.Forms.ComboBox();
            this.lblVault = new System.Windows.Forms.Label();
            this.butClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cboVaults
            // 
            this.cboVaults.FormattingEnabled = true;
            this.cboVaults.ItemHeight = 13;
            this.cboVaults.Location = new System.Drawing.Point(55, 2);
            this.cboVaults.Name = "cboVaults";
            this.cboVaults.Size = new System.Drawing.Size(102, 21);
            this.cboVaults.TabIndex = 0;
            this.cboVaults.DropDown += new System.EventHandler(this.cboVaults_DropDown);
            this.cboVaults.SelectedIndexChanged += new System.EventHandler(this.cboVaults_SelectedIndexChanged);
            this.cboVaults.SelectionChangeCommitted += new System.EventHandler(this.cboVaults_SelectionChangeCommitted);
            // 
            // lblVault
            // 
            this.lblVault.AutoSize = true;
            this.lblVault.Location = new System.Drawing.Point(5, 4);
            this.lblVault.Name = "lblVault";
            this.lblVault.Size = new System.Drawing.Size(44, 13);
            this.lblVault.TabIndex = 1;
            this.lblVault.Text = "Vault #:";
            // 
            // butClear
            // 
            this.butClear.Location = new System.Drawing.Point(163, 3);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(66, 22);
            this.butClear.TabIndex = 3;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // UfmFilter
            // 
            this.Controls.Add(this.butClear);
            this.Controls.Add(this.lblVault);
            this.Controls.Add(this.cboVaults);
            this.Name = "UfmFilter";
            this.Size = new System.Drawing.Size(233, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Constructor

        public UfmFilter()
        {
            InitializeComponent();
        }

        #endregion

        #region Populate Combobox

        public void Configure(IApplication app)
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            // Store the supplied map doco
            _application = app;
            //_map = map;

            // Populate the dropdown
            //PopulateCombo();

            // Invoke some code when somebody selects a vault
            // cboVaults.SelectedValueChanged += new EventHandler(cboVaults_SelectedValueChanged);
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
                IWorkspace ws = GetWorkspace();
                IFeatureClass fcSubs = ModelNameFacade.FeatureClassByModelName(ws, SchemaInfo.Electric.ClassModelNames.SubSurfaceStructure);

                // Lets cap this operation based on scale - if were beyond 1:3000, turning on/off UFM's is meaningless
                if (_map.MapScale < 3000)
                {
                    // Search all subsurface structures
                    ISpatialFilter qf = new SpatialFilterClass();
                    qf.Geometry = (_map as IActiveView).Extent;
                    qf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    qf.GeometryField = "SHAPE";
                    qf.WhereClause = "SUBTYPECD=3";
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

                            if (displayVal == string.Empty)
                            {
                                displayVal = sub.OID.ToString();
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

        #endregion

        #region UI Event Handlers

        //private void butRefresh_Click(object sender, EventArgs e)
        //{
        //    butClear_Click(sender, e);
        //    PopulateCombo();
        //}

        private void butClear_Click(object sender, EventArgs e)
        {
            // Clear the filter
            //VaultFilter.FilteredVault = string.Empty;

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
        }

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
                //VaultFilter.FilteredVault = (cboVaults.SelectedItem as VaultItem).GlobalId;
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Returns a workspace based on where the user logged in to
        /// </summary>
        /// <returns></returns>
        private IWorkspace GetWorkspace()
        {
            // Log entry
            string name = MethodInfo.GetCurrentMethod().Name;
            _logger.Debug("Entered " + name);

            // Get and return the logged in workspace
            IMMLoginUtils utils = new MMLoginUtils();
            return utils.LoginWorkspace;
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

        private void cboVaults_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboVaults_DropDown(object sender, EventArgs e)
        {
            PopulateCombo();
        }
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
