// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Schematics Correction SUP Form for assining and editing Schematic
// TCS V3SF (EDGISREARC-813) 05/20/2021                             Created
// </history>
// All rights reserved.
// ========================================================================

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using PGE.Desktop.SchematicsMaintenance.Core;
using PGE.Desktop.SchematicsMaintenance.UI.Extensions;
using System;
using System.Text;
using System.Windows.Forms;

namespace PGE.Desktop.SchematicsMaintenance
{
    /// <summary>
    /// (V3SF)
    /// Form To Create Correction SUP
    /// </summary>
    public partial class FrmCorrectionSUP : Form
    {
        #region Data Members
        PGESchematicsMaintenanceExtension m_maintenanceExt;
        IGeometry supGeometry = default;
        string Mapno = default;
        string Region = default;
        string Division = default;
        #endregion

        /// <summary>
        /// (V3SF)
        /// Constructor to calculate MAPGRID Names
        /// </summary>
        /// <param name="m_maintenanceExt"></param>
        /// <param name="geometry"></param>
        public FrmCorrectionSUP(PGESchematicsMaintenanceExtension m_maintenanceExt, IGeometry geometry)
        {
            //Initialize Components
            InitializeComponent();

            try
            {
                //Assigning Values from Constructor
                this.m_maintenanceExt = m_maintenanceExt;
                supGeometry = geometry;

                //Finding MapGrids
                Mapno = getMapGrids(geometry);
                
                
                if (string.IsNullOrWhiteSpace(Mapno))
                {
                    MessageBox.Show("No Valid MapNo Found for Correction SUP,Please create valid SUP");
                    throw new Exception("No Schematic Map Grid Found");
                }

                txtSessionID.Text = Mapno;

            }
            catch (Exception ex)
            {
                Logger.Log.Error("FrmCorrectionSUP Class Constructor : " + ex.Message + " : " + ex.StackTrace);
                this.Close();
            }
        }

        /// <summary>
        /// (V3SF)
        /// Get MapGrids,Region and Division of Correction SUP
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        private string getMapGrids(IGeometry geometry)
        {
            #region Data Members
            string returnStr = default;
            IFeatureClass mapGridFeatureClass = default;
            IFeatureClass regionDivisionFeatureClass = default;
            ISpatialFilter spatialFilter = default;
            int indexMapNoMG = default;
            int indexRegion = default;
            int indexDivision = default;
            StringBuilder Mapno = default;
            IFeature feature = default;
            IFeatureCursor featureCursor = default;
            #endregion

            try
            {
                //Finding FeatureClass
                mapGridFeatureClass = Utils.FindFeatureClass("EDGIS.Schematics_Unified_Grid");
                regionDivisionFeatureClass = Utils.FindFeatureClass("LBGIS.ELECDIVISION");

                if (mapGridFeatureClass == null)
                    throw new Exception("EDGIS.Schematics_Unified_Grid Feature Class not found");

                if (regionDivisionFeatureClass == null)
                    throw new Exception("LBGIS.ELECDIVISION Feature Class not found");

                indexMapNoMG = mapGridFeatureClass.FindField("MAPNO");
                if (indexMapNoMG == -1)
                    throw new Exception("MAPNO Column not found");

                indexRegion = regionDivisionFeatureClass.FindField("PGE_REGION");
                if (indexRegion == -1)
                    throw new Exception("PGE_REGION Column not found");

                indexDivision = regionDivisionFeatureClass.FindField("DIVISION");
                if (indexDivision == -1)
                    throw new Exception("DIVISION Column not found");

                //Intersection Filter
                spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = geometry;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                #region MapNO Update
                Mapno = new StringBuilder();

                featureCursor = mapGridFeatureClass.Search(spatialFilter, false);
                feature = featureCursor.NextFeature();
                Mapno.Clear();

                while (feature != null)
                {
                    try
                    {
                        if (Mapno.Length == 0)
                        {
                            Mapno.Append(Convert.ToString(feature.Value[indexMapNoMG]));
                        }
                        else
                            Mapno.Append("," + Convert.ToString(feature.Value[indexMapNoMG]));
                    }
                    catch (Exception ex)
                    {
                        // (V3SF) Test Feedback 09/06/2021
                        //Added Logging
                        Logger.Log.Error("Unable to Collect MAPNO for Correction SUP Error :: " + ex.Message + " at " + ex.StackTrace);
                        throw ex;
                    }
                    feature = featureCursor.NextFeature();
                }

                returnStr = Convert.ToString(Mapno);
                // (V3SF) Test Feedback 09/06/2021
                //Added Info Log
                Logger.Log.Info("Correction SUP MapNO Found : " + Mapno);
                #endregion

                #region Region and Division Update
                featureCursor = regionDivisionFeatureClass.Search(spatialFilter, false);
                feature = featureCursor.NextFeature();
                if (feature != null)
                {
                    // (V3SF) Test Feedback 09/06/2021
                    //Added Try/Catch
                    try
                    {
                        Region = Convert.ToString(feature.Value[indexRegion]);
                        Division = Convert.ToString(feature.Value[indexDivision]);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log.Error("Unable to Collect Region/Division for Correction SUP Error :: " + ex.Message + " at " + ex.StackTrace);
                        throw ex;
                    }
                }
                // (V3SF) Test Feedback 09/06/2021
                //Added Info Log
                Logger.Log.Info("Correction SUP Region,Division : " + Region + " " + Division);
                #endregion

            }
            catch (Exception ex)
            {
                Logger.Log.Error("FrmCorrectionSUP getMapGrids() : " + ex.Message + " : " + ex.StackTrace);
                throw ex;
            }
            finally
            {
                if (featureCursor != null) System.Runtime.InteropServices.Marshal.FinalReleaseComObject(featureCursor);
            }
            return returnStr;
        }

        /// <summary>
        /// (V3SF)
        /// Create Correction SUP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Submit_Click(object sender, EventArgs e)
        {
            #region Data Members
            IFeatureClass featureClass = Utils.FindFeatureClass("EDSCHEM_UpdatePolygon");
            IFeature feature = null;
            #endregion
            try
            {
                if (!string.IsNullOrEmpty(txtSessionID.Text) && !string.IsNullOrEmpty(txtLanID.Text))
                {
                    feature = featureClass.CreateFeature();

                    int idxSESSIONID = featureClass.Fields.FindField("SESSIONID");
                    int idxDATEPROCESSED = featureClass.Fields.FindField("DATEPROCESSED");
                    int idxMAPNO = featureClass.Fields.FindField("MAPNO");
                    int idxELANID = featureClass.Fields.FindField("ELANID");
                    int idxSTATUS = featureClass.Fields.FindField("STATUS");
                    int idxTOT = featureClass.Fields.FindField("TOTALNUMEDITS");
                    int idxREGION = featureClass.Fields.FindField("REGION");
                    int idxDIVISION = featureClass.Fields.FindField("DIVISION");

                    if (idxSESSIONID != -1 && idxDATEPROCESSED != -1 &&
                        idxMAPNO != -1 && idxELANID != -1 && idxSTATUS != -1 && idxTOT != -1
                        && idxREGION != -1 && idxDIVISION != -1)
                    {
                        feature.Value[idxTOT] = 1;//Check field Type
                        feature.Value[idxREGION] = Region;
                        feature.Value[idxDIVISION] = Division;
                        feature.Value[idxSESSIONID] = "Correction_" + txtSessionID.Text;
                        feature.Value[idxDATEPROCESSED] = System.DateTime.Now;
                        feature.Value[idxMAPNO] = Mapno;
                        feature.Value[idxELANID] = txtLanID.Text;
                        feature.Value[idxSTATUS] = 2;

                        feature.Shape = supGeometry;
                        feature.Store();
                        this.Close();
                    }

                }
                else
                {
                    MessageBox.Show("Session ID or LAN ID Fields cannot be left Empty");
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Unable to Process Correction SUP");
                Logger.Log.Error("FrmCorrectionSUP Submit_Click() : " + ex.Message + " : " + ex.StackTrace);
                this.Close();
            }
        }

        /// <summary>
        /// (V3SF)
        /// Close Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// (V3SF) 
        /// Validating LANID Test Input to be Alphabet or digit
        /// (V3SF) Test Feedback 09/06/2021
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtLanID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLetterOrDigit(e.KeyChar)     // Allowing only any letter OR Digit
            || e.KeyChar == '\b')                 // Allowing BackSpace character
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
