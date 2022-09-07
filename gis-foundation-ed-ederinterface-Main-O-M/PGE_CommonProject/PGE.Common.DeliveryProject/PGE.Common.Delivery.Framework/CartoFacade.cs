using System;
using System.Collections.Generic;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Miner.Geodatabase;
using Miner.Interop;

namespace PGE.Common.Delivery.Framework
{
    /// <summary>
    /// Carto facade contains a set of functions that could be used to work with commonly used carto classes like IMap,ILayer, IStandAloneTable etc. 
    /// Finding the fetaureclass name associated given a layer, Getting all the layers given a map,
    /// checking ifa layer has a given modelname etc.
    /// </summary>
    public class CartoFacade
    {
        private IMap map;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="focusmap"></param>
        public CartoFacade(IMap focusmap)
        {
            map = focusmap;
        }

        #region Private Members
        private static UID uid=new UIDClass();
        #endregion

        #region UID struct
        /// <summary>
        /// 
        /// </summary>
        public struct UIDFacade
        {
            /// <summary>
            /// The <see cref="IDataLayer"/> interface GUID.
            /// </summary>
            public static UID DataLayers
            {
                get
                {
                    uid.Value = "{6CA416B1-E160-11D2-9F4E-00C04F6BC78E}";
                    return uid;
                }
            }

            /// <summary>
            /// The <see cref="IFeatureLayer"/> interface GUID.
            /// </summary>
            public static UID FeatureLayers
            {
                get
                {
                    uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                    return uid;

                }
            }

            /// <summary>
            /// The <see cref="IGraphicsLayer"/> interface GUID.
            /// </summary>
            public static UID GraphicsLayers
            {
                get
                {
                    uid.Value = "{34B2EF81-F4AC-11D1-A245-080009B6F22B}";
                    return uid;
                }
            }

            /// <summary>
            /// The <see cref="IGroupLayer"/> interface GUID.
            /// </summary>
            public static UID GroupLayers
            {
                get
                {
                    uid.Value = "{EDAD6644-1810-11D1-86AE-0000F8751720}";
                    return uid;
                }
            }
        }
        #endregion

        #region Layer Methods
        /// <summary>
        /// Get a List of all Feature layers from the given Map Instance.
        /// </summary>
        /// <returns>A Generic List of Feature Layers</returns>
        /// <remarks>Will return an Empty List if the Imap Instance is null or the Modelname passed is null</remarks>
        public virtual List<IFeatureLayer> FeatureLayersFromMap()
        {
            return FeatureLayersWithModelName(string.Empty);
        }
        /// <summary>
        /// Gets a list of Feature Layers in the given IMap instance that has been assigned the given Modelname
        /// </summary>
        /// <param name="modelName">Modelname to check. This is an object of type string.</param>
        /// <returns>A List of all featurelayers layers in the given map with the specified modelname.</returns>
        /// <remarks>Will return an Empty List if the IMap Instance is null or the Modelname passed is null</remarks>
        public virtual List<IFeatureLayer> FeatureLayersWithModelName(string modelName)
        {
            List<IFeatureLayer> items = new List<IFeatureLayer>();
            try
            {
                if (map == null || modelName == null)
                    return items;

                if (map.LayerCount == 0) return null;

                IEnumLayer enumerator = map.get_Layers(UIDFacade.FeatureLayers, true);
                ILayer layer;

                while ((layer = enumerator.Next()) != null)
                {
                    if (layer is IFeatureLayer && layer.Valid)
                    {
                        IFeatureLayer layerFilter = ((IFeatureLayer)layer);
                        if (string.IsNullOrEmpty(modelName))
                        {
                            items.Add(layerFilter);
                        }
                        else
                        {
                            IFeatureClass classFilter = layerFilter.FeatureClass;
                            if (ModelNameFacade.ModelNameManager.ContainsClassModelName(classFilter, modelName))
                                items.Add(layerFilter);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return items;
        }
        /// <summary>
        /// Gets a list of Layers in the given IMap instance.
        /// </summary>
        /// <returns>A generic list of ILayers in the given map</returns>
        /// <remarks>Will return an Empty List if the IMap Instance is null or the Modelname passed is null</remarks>
        public virtual List<ILayer> GetAllLayers()
        {
            List<ILayer> items = new List<ILayer>();
            try
            {
                for (int lyrCnt = 0; lyrCnt < map.LayerCount; lyrCnt++)
                {
                    if (map.get_Layer(lyrCnt) is IGroupLayer)
                        items.AddRange(GetLayers(map.get_Layer(lyrCnt)));
                    else
                        items.Add(map.get_Layer(lyrCnt));
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }

            return items;
        }

        /// <summary>
        /// This will return the first feature layer in the current Map that is associated with the given featureclass.
        /// </summary>
        /// <param name="classFilter">IFeatureClass to find it's IFeatureLayer</param>
        /// <returns>IFeatureLayer</returns>
        public virtual IFeatureLayer FeatureLayerFromClass(IFeatureClass classFilter)
        {
            if (map == null || classFilter == null)
                return null;

            if (map.LayerCount == 0) return null;

            IEnumLayer enumerator = map.get_Layers(UIDFacade.FeatureLayers, true);
            ILayer layer;

            while ((layer = enumerator.Next()) != null)
            {
                if (layer is IFeatureLayer && layer.Valid)
                {
                    IFeatureLayer layerFilter = ((IFeatureLayer)layer);
                    if (layerFilter.FeatureClass.ObjectClassID == classFilter.ObjectClassID)
                        return layerFilter;
                }
            }

            return null;
        }
        #endregion

        #region Table Methods

        /// <summary>
        /// Gets a list of all the tables that are added to the given IMap instance
        /// </summary>
        /// <returns>A generic list of ITable in the given map</returns>
        /// <remarks>Will return an Empty List if the IMap Instance is null or the Modelname passed is null</remarks>
        public virtual List<ITable> GetAllTables()
        {
            return TablesWithModelName(string.Empty);
        }
        /// <summary>
        /// Gets a list of tables that are added to the given IMap instance and has the given modelname assigned.
        /// </summary>
        /// <param name="modelName">String parameter that defines the modelname to be checked for</param>
        /// <returns> generic list of ITable in the given map that has been assigned the given modelname</returns>
        /// <remarks>Will return an Empty List if the IMap Instance is null or the Modelname passed is null</remarks>
        public virtual List<ITable> TablesWithModelName(string modelName)
        {
            List<ITable> tables = new List<ITable>();
            try
            {
                if (map == null) return tables;
                if (map == null || modelName == null)
                    return tables;
                IStandaloneTableCollection tableCollection = (IStandaloneTableCollection)map;
                //This should never happen, if this happenes then there is some problem with the IMap
                if (tableCollection == null) return tables;
                if (tableCollection.StandaloneTableCount == 0) return tables;
                for (int tblCnt = 0; tblCnt < tableCollection.StandaloneTableCount; tblCnt++)
                {
                    if (string.IsNullOrEmpty(modelName))
                    {
                        tables.Add(tableCollection.get_StandaloneTable(tblCnt).Table);
                    }
                    else
                    {
                        if (ModelNameFacade.ModelNameManager.ContainsClassModelName((IObjectClass)tableCollection.get_StandaloneTable(tblCnt).Table,modelName))
                            tables.Add(tableCollection.get_StandaloneTable(tblCnt).Table);
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
            }
            return tables;
        }

        #endregion

        #region Workspace Methods

        /// <summary>
        /// Gets the workspace from map that has been assigned the given database model name.
        /// </summary>
        /// <param name="modelName">Name of the model.</param>
        /// <returns>The workspace; otherwise null.</returns>
        public virtual IWorkspace WorkspaceFromMap(string modelName)
        {
            IMMWorkspaceManagerMap wkspcMgrMap = new MMWorkspaceManagerClass();
            IEnumWorkspaceEx enumWkspcs = wkspcMgrMap.GetMapWorkspaces(map);
            enumWkspcs.Reset();
            IWorkspace wkspc = null;

            while ((wkspc = enumWkspcs.Next()) != null)
            {
                IWorkspaceExtensionManager wem = wkspc as IWorkspaceExtensionManager;
                if (wem != null)
                {
                    UID uid = new UIDClass();
                    uid.Value = "{54148E70-336D-11D5-9AB3-0001031AE963}"; // MMWorkspaceExtension

                    IWorkspaceExtension we = wem.FindExtension(uid);
                    if (we != null)
                    {
                        IMMModelNameInfo mni = (IMMModelNameInfo)we;
                        if (mni.ModelNameExists(modelName))
                            return wkspc;
                    }
                }
            }

            return null;
        }

        #endregion

        #region private methods
        /// <summary>
        /// Gets a list of ILayer given a ILayer. If the passed in ILayer is a Composite/Group Layer then all the layers under the composite layers are returned as a Generic list.
        /// </summary>
        /// <param name="layer">An object of type ILayer</param>
        /// <returns>A generic list containing all the layers that are part of the Group layer if the passed in layer is of type ICompositeLayer else the generic list contains the passed in layer</returns>
        private List<ILayer> GetLayers(ILayer layer)
        {
            List<ILayer> items=new List<ILayer>();
            if (layer == null) return items;
            if (layer is IGroupLayer)
            {
                ICompositeLayer2 compLayer=(ICompositeLayer2)layer;
                for (int lyrCnt = 0; lyrCnt < compLayer.Count; lyrCnt++)
                {
                    if(compLayer.get_Layer(lyrCnt) is IGroupLayer)
                        items.AddRange(GetLayers(compLayer.get_Layer(lyrCnt)));
                    else
                        items.Add(compLayer.get_Layer(lyrCnt)); 
                }
            }
            else
            {
                items.Add(layer);
            }
            return items;
        }
        #endregion
    }
}
