using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geodatabase;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Carto;
using Miner.Interop;
using Miner;

namespace PGE.BatchApplication.GenerateRequiredMxds.Common
{
    public static class Common
    {
        #region Public variables

        public static string FolderLayerLocation = "Map Production Layer Files";
        public static string FolderMxdLocation = "Map Production Mxds";
        public static string COTSLayerName = "Common Landbase - Vector";
        public static string FolderLogFiles = "Mxd Creation Log Files";

        #endregion

        #region Private static variables

        private static IMMRegistry _reg = new MMRegistry();

        #endregion

        #region Public Static methods

        public static void ReadAppSettings()
        {
            
        }

        /// <summary>
        /// Return the feature class specified from the workspace.  Returns null if feature class does not exist
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="FeatureClassName"></param>
        /// <returns></returns>
        public static IFeatureClass getFeatureClass(IWorkspace workspace, string FeatureClassName)
        {
            if (workspace != null)
            {
                IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
                if (featWorkspace != null)
                {
                    return featWorkspace.OpenFeatureClass(FeatureClassName);
                }
            }
            return null;
        }

        /// <summary>
        /// Return the table specified from the workspace.  Returns null if table does not exist
        /// </summary>
        /// <param name="workspace"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static ITable getTable(IWorkspace workspace, string tableName)
        {
            if (workspace != null)
            {
                IFeatureWorkspace featWorkspace = workspace as IFeatureWorkspace;
                if (featWorkspace != null)
                {
                    return featWorkspace.OpenTable(tableName);
                }
            }
            return null;
        }

        public static ILayer getLayerFromMap(IMap map, string layerName)
        {
            IEnumLayer enumLayer = map.get_Layers(null, true);
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                if (layer.Name.ToUpper() == layerName.ToUpper())
                {
                    return layer;
                }
                layer = enumLayer.Next();
            }
            return null;
        }

        public static IFeatureLayer getFeatureLayerFromMap(IMap map, string featureClassName)
        {
            //Get all of the IFeatureLayers
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
            IEnumLayer enumLayer = map.get_Layers(uid, true);
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                IFeatureLayer featLayer = layer as IFeatureLayer;
                if (featLayer != null)
                {
                    IDataset ds = featLayer as IDataset;
                    if (ds.BrowseName.ToUpper() == featureClassName.ToUpper())
                    {
                        return featLayer;
                    }
                }
                layer = enumLayer.Next();
            }
            return null;
        }

        public static List<IFeatureLayer> getFeatureLayersFromMap(IMap map, string featureClassName)
        {
            List<IFeatureLayer> featLayers = new List<IFeatureLayer>();

            //Get all of the IFeatureLayers
            ESRI.ArcGIS.esriSystem.UID uid = new ESRI.ArcGIS.esriSystem.UIDClass();
            uid.Value = "{40A9E885-5533-11D0-98BE-00805F7CED21}";
            IEnumLayer enumLayer = map.get_Layers(uid, true);
            enumLayer.Reset();
            ILayer layer = enumLayer.Next();
            while (layer != null)
            {
                IFeatureLayer featLayer = layer as IFeatureLayer;
                if (featLayer != null)
                {
                    IDataset ds = featLayer as IDataset;
                    if (ds.BrowseName.ToUpper() == featureClassName.ToUpper())
                    {
                        featLayers.Add(featLayer);
                    }
                }
                layer = enumLayer.Next();
            }
            return featLayers;
        }

        /// <summary>
        /// Opens a workspace given a direct connect string (i.e. sde:oracle11g:/;local=EDGISA1T)
        /// </summary>
        /// <param name="directConnectString"></param>
        /// <returns></returns>
        public static IWorkspace OpenWorkspace(string sdeFileLocation)
        {
            /*
            // Create and populate the property set to connect to the database.
            ESRI.ArcGIS.esriSystem.IPropertySet propertySet = new ESRI.ArcGIS.esriSystem.PropertySetClass();
            propertySet.SetProperty("SERVER", "");
            propertySet.SetProperty("INSTANCE", directConnectString);
            propertySet.SetProperty("DATABASE", "");
            propertySet.SetProperty("USER", user);
            propertySet.SetProperty("PASSWORD", password);
            propertySet.SetProperty("VERSION", "sde.DEFAULT");
            */

            SdeWorkspaceFactory wsFactory = new SdeWorkspaceFactoryClass();
            return wsFactory.OpenFromFile(sdeFileLocation, 0);
        }

        /// <summary>
        /// Checks if the map grid feature contains any of the features in the specified feature class
        /// </summary>
        /// <param name="mapGridFeature">Map grid feature</param>
        /// <param name="featClass">Feature class to check against</param>
        /// <returns></returns>
        public static bool HasData(IFeature mapGridFeature, IFeatureClass featClass)
        {
            IFeatureCursor featCursor = null;
            IFeature feat = null;
            try
            {
                ISpatialFilter qf = new SpatialFilterClass();
                qf.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                qf.Geometry = mapGridFeature.Shape;
                qf.GeometryField = featClass.ShapeFieldName;
                qf.SubFields = featClass.OIDFieldName;
                featCursor = featClass.Search(qf, true);
                feat = featCursor.NextFeature();
                if (feat != null)
                {
                    return true;
                }
                return false;
            }
            finally
            {
                if (featCursor != null) { while (Marshal.ReleaseComObject(featCursor) > 0) { } }
            }
        }

        #endregion

    }
}
