using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Configuration;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System.Reflection;
using System.IO;

namespace PGE.BatchApplication.GenerateRequiredMxds
{
    public class CreateNewMxd
    {
        private IWorkspace workspace = null;
        private MapDocument mapDoc = new MapDocumentClass();
        private string LayersToRemove = "";
        private bool IsTiff = false;
        private string LayersToAdd = "";
        private string projSystem = "";
        private StreamWriter writer = null;
        private string MxdName = "";

        public CreateNewMxd(string sdeFileLocation, string mxdName, string layersToRemove, string layersToAdd, bool isTiff, string projSystem)
        {
            if (!Directory.Exists(Common.Common.FolderLogFiles))
            {
                Directory.CreateDirectory(Common.Common.FolderLogFiles);
            }

            int counter = 0;
            bool tryAgain = true;
            while (tryAgain)
            {
                try
                {
                    tryAgain = false;
                    writer = new StreamWriter(Common.Common.FolderLogFiles + "\\" + "MxdCreation_Log" + counter + ".txt", true);
                }
                catch (Exception e) { tryAgain = true; }
                counter++;
            }

            Log("Making changes to Mxd: " + mxdName);

            MxdName = mxdName;
            LayersToRemove = layersToRemove;
            IsTiff = isTiff;
            this.projSystem = projSystem;
            LayersToAdd = layersToAdd;
            workspace = Common.Common.OpenWorkspace(sdeFileLocation);
            mapDoc.Open(Common.Common.FolderMxdLocation + "\\" + mxdName);
            InitActiveViews();
        }

        ~CreateNewMxd()
        {
            if (writer != null)
            {
                writer.Close();
            }
        }

        public void Log(string log)
        {
            Console.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.WriteLine(DateTime.Now.ToString() + ": " + log);
            writer.Flush();
        }

        public void CreateMxd()
        {
            RemoveLayers();
            TurnOffTransparency();

            if (IsTiff)
            {
                //For Tiffs Ravi has two layer files that need to be added to replace the maintenance plat that was originally there
                AddLayers();
                MoveCOTSLayer();
                //SetLineColors();
            }
            else
            {
                AddLayers();
            }

            SetSpatialReference();

            try
            {
                mapDoc.Save(true, false);
                mapDoc.Close();
                Log("Saved changes to " + MxdName);
            }
            catch (Exception e)
            {

            }
        }

        private void SetSpatialReference()
        {
            int proj = (int)esriSRProjCSType.esriSRProjCS_NAD1927SPCS_CAI;

            if (projSystem.ToUpper() == "CAI")
            {
                proj = (int)esriSRProjCSType.esriSRProjCS_NAD1927SPCS_CAI;
            }
            if (projSystem.ToUpper() == "CAII")
            {
                proj = (int)esriSRProjCSType.esriSRProjCS_NAD1927SPCS_CAII;
            }
            if (projSystem.ToUpper() == "CAIII")
            {
                proj = (int)esriSRProjCSType.esriSRProjCS_NAD1927SPCS_CAIII;
            }
            if (projSystem.ToUpper() == "CAIV")
            {
                proj = (int)esriSRProjCSType.esriSRProjCS_NAD1927SPCS_CAIV;
            }
            if (projSystem.ToUpper() == "CAV")
            {
                proj = (int)esriSRProjCSType.esriSRProjCS_NAD1927SPCS_CAV;
            }

            ISpatialReferenceFactory refFactory = new SpatialReferenceEnvironmentClass();
            for (int i = 0; i < mapDoc.MapCount; i++)
            {
                IMap map = mapDoc.get_Map(i);
                map.SpatialReference = refFactory.CreateProjectedCoordinateSystem(proj);
            }
        }

        private void SetLineColors()
        {
            IRgbColor rgbLineColor = new RgbColorClass();
            rgbLineColor.Red = 255;
            rgbLineColor.Green = 255;
            rgbLineColor.Blue = 255;

            IRgbColor rgbFillColor = new RgbColorClass();
            rgbFillColor.Red = 0;
            rgbFillColor.Green = 0;
            rgbFillColor.Blue = 0;

            int mapCount = mapDoc.MapCount;
            for (int i = 0; i < mapCount; i++)
            {
                IMap map = mapDoc.get_Map(i);

                //Get all of the ILayers
                IEnumLayer enumLayer = map.get_Layers(null, true);
                enumLayer.Reset();
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    SetLineColor(layer, rgbLineColor, rgbFillColor);
                    layer = enumLayer.Next();
                }
            }
        }

        private void SetLineColor(ILayer layer, IRgbColor LineColor, IRgbColor fillColor)
        {
            
            //If this layer supports transparency and it is not set 0, then set it to 0 now.
            if (layer is IFeatureLayer)
            {
                IFeatureLayer featLayer = layer as IFeatureLayer;
                IFeature feat = featLayer.Search(null, false).NextFeature();
                if (feat != null)
                {
                    IGeoFeatureLayer geoFeatLayer = featLayer as IGeoFeatureLayer;
                    if (geoFeatLayer != null)
                    {
                        IFeatureRenderer featRenderer = geoFeatLayer.Renderer;
                        if (featRenderer != null)
                        {
                            ISymbol symbol = featRenderer.get_SymbolByFeature(feat);
                            if (symbol is IFillSymbol)
                            {
                                IFillSymbol fillSymbol = symbol as IFillSymbol;
                                fillSymbol.Color = fillColor;
                            }
                            if (symbol is ILineSymbol)
                            {
                                ILineSymbol lineSymbol = symbol as ILineSymbol;
                                lineSymbol.Color = LineColor;
                            }
                        }
                    }
                }
            }

            //Loop through this layers containing layers if it is a composite layer
            if (layer is ICompositeLayer)
            {
                ICompositeLayer compLayer = layer as ICompositeLayer;
                for (int i = 0; i < compLayer.Count; i++)
                {
                    SetLineColor(compLayer.get_Layer(i), LineColor, fillColor);
                }
            }
        }

        /// <summary>
        /// Add the layers specified for the mxd
        /// </summary>
        private void AddLayers()
        {
            string exeLocation = Assembly.GetExecutingAssembly().Location;
            string fileFolder = exeLocation.Substring(0,  exeLocation.LastIndexOf("\\")) + "\\" + Common.Common.FolderLayerLocation;
            string[] layers = Regex.Split(LayersToAdd, ",");
            foreach (string layer in layers)
            {
                if (String.IsNullOrEmpty(layer)) { continue; }
                AddLayer(fileFolder + "\\" + layer);
            }
        }

        /// <summary>
        /// Add the specified layer file
        /// </summary>
        /// <param name="LayerFile">Layer file name</param>
        private void AddLayer(string LayerFile)
        {
            ILayerFile layerFile = new LayerFileClass();
            layerFile.Open(LayerFile);

            ILayer layer = layerFile.Layer;
            SetDataSource(layer);
            for (int i = 0; i < mapDoc.MapCount; i++)
            {
                mapDoc.get_Map(i).AddLayer(layer);
                mapDoc.get_Map(i).MoveLayer(layer, 0);
            }
        }

        /// <summary>
        /// This will move the COTS group layer (the landbase) to in between the two maintenance plat layers for the TIFF
        /// </summary>
        private void MoveCOTSLayer()
        {
            for (int i = 0; i < mapDoc.MapCount; i++)
            {
                IMap map = mapDoc.get_Map(i);
                ILayer layer = Common.Common.getLayerFromMap(map, Common.Common.COTSLayerName);
                if (layer != null)
                {
                    map.MoveLayer(layer, 1);
                }
            }
        }

        private void SetDataSource(ILayer layer)
        {
            IFeatureLayer featLayer = layer as IFeatureLayer;
            if (featLayer != null)
            {
                IFeatureClass featClass = Common.Common.getFeatureClass(workspace, ((IDataset)featLayer.FeatureClass).BrowseName);
                featLayer.FeatureClass = featClass;
            }
        }

        /// <summary>
        /// Removes all layers from the list specified to the program arguments
        /// </summary>
        private void RemoveLayers()
        {
            string[] layersToRemoveArray = Regex.Split(LayersToRemove, ",");
            foreach (string layer in layersToRemoveArray)
            {
                if (String.IsNullOrEmpty(layer)) { continue; }
                RemoveLayer(layer);
            }
        }

        /// <summary>
        /// Removes any feature layers referring to the specified feature class name
        /// </summary>
        /// <param name="featClassName"></param>
        private void RemoveLayer(string featClassName)
        {
            for(int i = 0; i < mapDoc.MapCount; i++)
            {
                IMap currentMap = mapDoc.get_Map(i);
                List<IFeatureLayer> featLayers = Common.Common.getFeatureLayersFromMap(currentMap, featClassName);
                foreach (IFeatureLayer featLayer in featLayers)
                {
                    currentMap.DeleteLayer(featLayer);
                }
            }
        }

        /// <summary>
        /// Reads the app.config file for any necessary configuration information
        /// </summary>
        private void ReadConfiguration()
        {
            
        }

        /// <summary>
        /// Initialize all of the activeviews of the current map document
        /// </summary>
        private void InitActiveViews()
        {
            int mapCount = mapDoc.MapCount;
            for (int i = 0; i < mapCount; i++)
            {
                IActiveView mapActiveView = mapDoc.get_Map(i) as IActiveView;
            }
            IActiveView activeView = mapDoc.PageLayout as IActiveView;
            activeView.Activate(0);
        }

        /// <summary>
        /// This method will turn transparency off on all feature layers.  This is necessary to ensure no pixallation as transparency
        /// does vectorize the layers.
        /// http://support.esri.com/en/knowledgebase/techarticles/detail/22070
        /// http://support.esri.com/en/bugs/nimbus/TklNMDcwMTQ4
        /// </summary>
        private void TurnOffTransparency()
        {
            int mapCount = mapDoc.MapCount;
            for (int i = 0; i < mapCount; i++)
            {
                IMap map = mapDoc.get_Map(i);

                //Get all of the ILayers
                IEnumLayer enumLayer = map.get_Layers(null, true);
                enumLayer.Reset();
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    TurnOffTransparency(layer);
                    layer = enumLayer.Next();
                }
            }
        }

        private void TurnOffTransparency(ILayer layer)
        {
            //If this layer supports transparency and it is not set 0, then set it to 0 now.
            ILayerEffects effects = layer as ILayerEffects;
            if (effects != null && effects.SupportsTransparency)
            {
                short transparency = effects.Transparency;
                if (effects.Transparency != 0)
                {
                    effects.Transparency = 0;
                }
            }

            //Loop through this layers containing layers if it is a composite layer
            if (layer is ICompositeLayer)
            {
                ICompositeLayer compLayer = layer as ICompositeLayer;
                for (int i = 0; i < compLayer.Count; i++)
                {
                    TurnOffTransparency(compLayer.get_Layer(i));
                }
            }
        }


        private void SetMinimumScale(int minScale)
        {
            int mapCount = mapDoc.MapCount;
            for (int i = 0; i < mapCount; i++)
            {
                IMap map = mapDoc.get_Map(i);

                //Get all of the ILayers
                IEnumLayer enumLayer = map.get_Layers(null, true);
                enumLayer.Reset();
                ILayer layer = enumLayer.Next();
                while (layer != null)
                {
                    SetMinimumScale(minScale);
                    layer = enumLayer.Next();
                }
            }
        }

        private void SetMinimumScale(ILayer layer, int minScale)
        {
            if (layer.MinimumScale < 100000 && layer.MinimumScale != 0)
            {
                layer.MinimumScale = 100000;
            }

            //Loop through this layers containing layers if it is a composite layer
            if (layer is ICompositeLayer)
            {
                ICompositeLayer compLayer = layer as ICompositeLayer;
                for (int i = 0; i < compLayer.Count; i++)
                {
                    SetMinimumScale(compLayer.get_Layer(i), minScale);
                }
            }
        }

    }
}
