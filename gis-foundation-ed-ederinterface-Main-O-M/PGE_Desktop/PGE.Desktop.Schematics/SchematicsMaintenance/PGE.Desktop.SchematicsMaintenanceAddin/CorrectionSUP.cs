// ========================================================================
// Copyright © 2021 PGE.
// <history>
// Schematics Correction SUP Button for assining and editing Schematic
// TCS V3SF (EDGISREARC-813) 05/20/2021                             Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SchematicUI;
using PGE.Desktop.SchematicsMaintenance.Core;
using PGE.Desktop.SchematicsMaintenance.UI.Extensions;
using Tool = ESRI.ArcGIS.Desktop.AddIns.Tool;

namespace PGE.Desktop.SchematicsMaintenance
{
    /// <summary>
    /// (V3SF)
    /// Create SUP Correction Geometry and Call Form to Submit Correction SUP
    /// </summary>
    class CorrectionSUP : Tool
    {
        #region Data members
        private SchematicExtension m_schematicExtension;
        private PGESchematicsMaintenanceExtension m_maintenanceExt;
        #endregion

        /// <summary>
        /// (V3SF)
        /// Constructor to initialize value of SchematicExtension as null
        /// </summary>
        public CorrectionSUP()
        {
            m_schematicExtension = null;

        }

        /// <summary>
        /// (V3SF)
        /// Create SUP Correction Geometry
        /// </summary>
        /// <param name="arg"></param>
        protected override void OnMouseDown(MouseEventArgs arg)
        {
            IMap map = default(IMap);
            ILayer layer = default;
            IQueryFilter qf = default;
            IFeatureClass featureClass = default;
            IFeatureLayer featureLayer = default;
            ILegendInfo legendInfo = default;
            ILegendGroup legendGroup = default;
            ILegendClass legendClass = default;
            ESRI.ArcGIS.Display.ISymbol symbol = default;
            ESRI.ArcGIS.Geometry.IGeometry geometry = default;
            ESRI.ArcGIS.Display.IRubberBand rubberBand = default;
            ESRI.ArcGIS.Display.IScreenDisplay screenDisplay = default;

            try
            {
                //Set Cursor to Cross
                this.Cursor = Cursors.Cross;
                System.Windows.Forms.Cursor.Current = Cursors.Cross;

                //Get Current Map
                map = ArcMap.Document.FocusMap;

                //Initiazlize Screen Display Element
                screenDisplay = ArcMap.Document.ActivatedView.ScreenDisplay;
                screenDisplay.StartDrawing(screenDisplay.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);

                #region Commented Create Custom Symbology
                /*ESRI.ArcGIS.Display.IRgbColor rgbColor = new ESRI.ArcGIS.Display.RgbColorClass();
                rgbColor.Red = 255;
                ESRI.ArcGIS.Display.IColor color = rgbColor;

                ESRI.ArcGIS.Display.ISimpleFillSymbol simpleFillSymbol = new ESRI.ArcGIS.Display.SimpleFillSymbolClass();
                simpleFillSymbol.Color = color;

                ESRI.ArcGIS.Display.ISymbol symbol = simpleFillSymbol as ESRI.ArcGIS.Display.ISymbol;*/
                #endregion

                //Get SUP Feature Class
                featureClass = Utils.FindFeatureClass("EDSCHEM_UpdatePolygon");
                layer = GetFeatureLayer(featureClass, map);
                qf = new QueryFilterClass();

                featureLayer = layer as IFeatureLayer;
                legendInfo = featureLayer as ILegendInfo;
                legendGroup = legendInfo.LegendGroup[0];
                legendClass = legendGroup.Class[0];

                //Uncomment to use custom Symbology
                //IFeature feature = featureClass.Search(qf, false).NextFeature();
                //IGeoFeatureLayer geoFeatureLayer = layer as IGeoFeatureLayer;
                //ESRI.ArcGIS.Display.ISymbol symbol = geoFeatureLayer.Renderer.SymbolByFeature[feature] as ESRI.ArcGIS.Display.ISymbol;

                //Set Symbology same as SUP Feature Class
                symbol = legendClass.Symbol as ESRI.ArcGIS.Display.ISymbol;

                //Draw Polygon
                rubberBand = new ESRI.ArcGIS.Display.RubberPolygonClass();
                geometry = rubberBand.TrackNew(screenDisplay, symbol);
                screenDisplay.SetSymbol(symbol);
                screenDisplay.DrawPolygon(geometry);
                screenDisplay.FinishDrawing();

                //Call Form here
                if (!geometry.IsEmpty)
                {
                    FrmCorrectionSUP frmCorrectionSUP = new FrmCorrectionSUP(m_maintenanceExt, geometry);
                    frmCorrectionSUP.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Unable to create Correction SUP ");
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error("CorrectionSUP OnMouseDown() : " + ex.Message + " : " + ex.StackTrace);
            }

            //Reset Cursor to default
            this.Cursor = Cursors.Default;
            ((IActiveView)ArcMap.Document.FocusMap).Refresh();
        }

        /// <summary>
        /// (V3SF)
        /// Get Layer from Active Map
        /// </summary>
        /// <param name="OFFeatureclass"></param>
        /// <returns></returns>
        private ILayer GetFeatureLayer(IFeatureClass OFFeatureclass, IMap map)
        {
            #region Data Members
            ILayer returnLayer = default;
            ILayer layer = default;

            IFeatureLayer mapFLayer = default;
            IFeatureClass mapFC = default;
            IEnumLayer layers = default;

            bool found = default;

            #endregion

            if (OFFeatureclass == null)
                return returnLayer;

            try
            {
                layers = map.Layers[null, true];
                layer = layers.Next();
                found = false;

                while (layer != null)
                {
                    if (found)
                        break;
                    else if (layer is IFeatureLayer)
                    {
                        mapFLayer = layer as IFeatureLayer;
                        try
                        {
                            if (mapFLayer.FeatureClass is FeatureClass)
                            {
                                mapFC = mapFLayer.FeatureClass as IFeatureClass;

                                if (mapFC.ObjectClassID == OFFeatureclass.ObjectClassID)
                                {
                                    returnLayer = layer;
                                    found = true;
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.Log.Error("WorkQueueSelectorWindow Class GetFeatureLayer(): " + ex.Message + " : " + ex.StackTrace);
                        }
                    }
                    layer = layers.Next();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error("WorkQueueSelectorWindow Class GetFeatureLayer(): " + ex.Message + " : " + ex.StackTrace);
            }

            return returnLayer;
        }

        /// <summary>
        /// (V3SF)
        /// Function to Enable or disable WorkQueue Button in ArcMap
        /// </summary>
        protected override void OnUpdate()
        {
            //Enabled = true;
            //Enabled = ArcMap.Application != null;
            Enabled = false;
            if (m_schematicExtension == null)
                m_schematicExtension = Utils.GetSchematicExtension();

            if (m_maintenanceExt == null)
                m_maintenanceExt = PGESchematicsMaintenanceExtension.GetExtension();

            if (m_schematicExtension == null)
            {
                Enabled = false;
                return;
            }

            if (m_maintenanceExt == null)
            {
                Enabled = false;
                return;
            }

            if (m_maintenanceExt != null)
            {
                if (m_maintenanceExt.Configuration != null)
                    if (m_maintenanceExt.Configuration.IsInitialized)
                        if (m_maintenanceExt.Configuration.SUPERVISOR_Role)
                            Enabled = (Utils.IsSchematicMap(ArcMap.Document.ActiveView.FocusMap, false));
            }

        }
    }
}
