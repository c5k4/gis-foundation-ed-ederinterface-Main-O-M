using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geometry;

namespace PGE.Desktop.EDER.ArcMapCommands
{
    /// <summary>
    /// Allows user to select a MapGrid to be printed
    /// </summary>
    public partial class PGEPrintMapGrids : Form
    {
        #region Private Variables

        /// <summary>
        /// Hodls the collection of MapNumber and MapOffie values
        /// </summary>
        private Dictionary<string, string> _mapNoAndOfficeColl;
        private Dictionary<string, IFeatureClass> _layersColl;
        /// <summary>
        /// Logger to log error/debug messages
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private IEnvelope _envelope;
        private IFeatureClass _featureClass;
        #endregion Private Variables

        #region Constrcutor

        /// <summary>
        /// Creates an instance of <see cref="PGEPrintMapGrids"/>
        /// </summary>
        /// <param name="mapNoAndOfficeColl">Hodls the collection of MapNumber and MapOffie values. If a Map Number has multiples Map Office values then they are seprarated by colon.</param>
        public PGEPrintMapGrids(Dictionary<string, IFeatureClass> layersCollection, IEnvelope envelope)
        {
            InitializeComponent();
            _envelope = envelope;
            SelectedMapGrid = string.Empty;
            SelectedFeatureClass = null;
            _layersColl = layersCollection;
        }

        #endregion Constrcutor

        #region Public Properties

        /// <summary>
        /// Holds the user selected MapGrid and MapOffie seperated by colon.
        /// </summary>
        public string SelectedMapGrid { get; set; }
        public IFeatureClass SelectedFeatureClass { get; set; }
        #endregion Public Properties

        #region Private Event Handler methods
        private void polulateLayers(IFeatureClass featureClass, IGeometry envelope)
        {

            //Get the index of MapNumber and MapOffice fields
            int mapNoFldIx = ModelNameFacade.FieldIndexFromModelName(featureClass, SchemaInfo.General.FieldModelNames.MapNumberMN);
            int mapOfficeFldIx = ModelNameFacade.FieldIndexFromModelName(featureClass, SchemaInfo.General.FieldModelNames.MapOfficeMN);
            if (mapNoFldIx == -1) //|| mapOfficeFldIx == -1)
            {
                string[] modelNames = new string[] { SchemaInfo.General.FieldModelNames.MapNumberMN, SchemaInfo.General.ClassModelNames.MapGridMN };
                _logger.Debug(string.Format("{0} field model names are missing on class with {1} model name", modelNames));
                return;
            }

            lstMapOffice.Enabled = (mapOfficeFldIx != -1);
            lblMapOffice.Enabled = (mapOfficeFldIx != -1);

            //Get map grids in the current visible extent and extract MapNumer/MapOffice of each Grid
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = envelope;
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            spatialFilter.GeometryField = featureClass.ShapeFieldName;
            spatialFilter.SearchOrder = esriSearchOrder.esriSearchOrderSpatial;
            IFeatureCursor featureCursor = featureClass.Search(spatialFilter, true);

            //Loop each grid feature
            IFeature feature;
            string mapNo, mapOffice;
            object objValue;
            Dictionary<string, string> mapNoandOffice = new Dictionary<string, string>();
            while ((feature = featureCursor.NextFeature()) != null)
            {
                mapNo = "";
                //Get Map Number
                objValue = feature.get_Value(mapNoFldIx);
                if (objValue == null || objValue == DBNull.Value)
                {
                    _logger.Debug(string.Format("Map Number is null for OID ={0}", feature.OID));
                    continue;
                }
                mapNo = objValue.ToString();
                mapOffice = "";
                if (mapOfficeFldIx != -1)
                {                    
                    //Get Map Office
                    objValue = feature.get_Value(mapOfficeFldIx);
                    if (objValue == null || objValue == DBNull.Value)
                    {
                        _logger.Debug(string.Format("Map Number is null for OID ={0}", feature.OID));
                        mapOffice = string.Empty;
                    }
                    mapOffice = objValue.ToString();
                }
                
                //Add the MapNumber and MapOffice to the dictionary if not already present.
                //If already present, then append the MapOffice to exiting MapOffice
                if (mapNoandOffice.ContainsKey(mapNo))
                {
                    mapNoandOffice[mapNo] = mapNoandOffice[mapNo] + ":" + mapOffice;
                }
                else
                {
                    mapNoandOffice.Add(mapNo, mapOffice);
                }
            }
            //Release the resources
            Marshal.ReleaseComObject(featureCursor);
            Marshal.ReleaseComObject(spatialFilter);

            _mapNoAndOfficeColl = mapNoandOffice;



        }
        /// <summary>
        /// Executed when <see cref="btnCancel">Cancel button</see> is clicked and dismisses the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            SelectedMapGrid = string.Empty;
            SelectedFeatureClass = null;
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Loads the GUI and MapNumbers/MapOffices into the dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PGEPrintMapGrids_Load(object sender, EventArgs e)
        {
            comboLayer.DataSource = new BindingSource(_layersColl, null);
            comboLayer.ValueMember = "Value";
            comboLayer.DisplayMember = "Key";

            if (comboLayer.Items.Count > 0) { comboLayer.SelectedIndex = 0; }

        }

        /// <summary>
        /// Executed when an item in selected on <see cref="lstMapNo">Map Number Listbox</see>    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstMapNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstMapOffice.Items.Clear();
            if (lstMapNo.SelectedIndex == -1) return;
            //get the selected map number
            string mapNo = lstMapNo.SelectedItem.ToString();
            //Get the Mapoffice for the selected Map Number
            string mapOfficeValue = _mapNoAndOfficeColl[mapNo];
            if (mapOfficeValue == string.Empty) return;
            string[] mapOffices = mapOfficeValue.Split(new char[] { ':' });
            if (mapOffices.Length < 1) return;
            lstMapOffice.Items.AddRange(mapOffices);
            if (lstMapOffice.Items.Count > 0) lstMapOffice.SelectedIndex = 0;
        }

        /// <summary>
        /// Executed when <see cref="btnOK">OK button</see> is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lstMapNo.Items.Count < 1) return;
            if (lstMapNo.Items.Count > 0 && lstMapNo.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Map Number from the list.", "Map Number", MessageBoxButtons.OK);
                return;
            }
            if (lstMapOffice.Items.Count > 0 && lstMapOffice.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Map Office from the list.", "Map Office", MessageBoxButtons.OK);
                return;
            }

            //Combine MapNumber and MapOffice by separating with colon
            string mapNoAndOffice = lstMapNo.SelectedItem.ToString();
            if (lstMapOffice.SelectedIndex != -1)
            {
                mapNoAndOffice += ":" + lstMapOffice.SelectedItem.ToString();
            }
            else
            {
                mapNoAndOffice += ":";
            }
            SelectedMapGrid = mapNoAndOffice;
            SelectedFeatureClass = ((KeyValuePair<string, IFeatureClass>)comboLayer.SelectedItem).Value;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

        }

        #endregion Private Event Handler methods

        private void comboLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboLayer.SelectedIndex == -1) return;
            IFeatureClass featureClass = ((KeyValuePair<string, IFeatureClass>)comboLayer.SelectedItem).Value;
            _featureClass = featureClass;
            polulateLayers(featureClass, _envelope);

            lstMapNo.Items.Clear();
            //Load the Map Number values to MapNumberListbox
            foreach (string mapGridNo in _mapNoAndOfficeColl.Keys)
            {
                lstMapNo.Items.Add(mapGridNo);
            }
            if (lstMapNo.Items.Count > 0) lstMapNo.SelectedIndex = 0;
        }
    }
}
