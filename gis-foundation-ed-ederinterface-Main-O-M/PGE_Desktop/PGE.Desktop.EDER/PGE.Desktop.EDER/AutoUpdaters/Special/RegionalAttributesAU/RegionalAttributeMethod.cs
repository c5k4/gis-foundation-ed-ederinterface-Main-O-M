// ========================================================================
// Copyright © 2006 Telvent, Consulting Engineers, Inc.
// <history>
// Editing - Point in Polygon Component Specification v0.7
// Shaikh Rizuan 10/15/2012	Created
// </history>
// All rights reserved.
// ========================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Reflection;
using System.Collections;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using PGE.Common.Delivery.Framework;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Systems.Configuration;

namespace PGE.Desktop.EDER.AutoUpdaters.Special.RegionalAttributesAU
{
    public class RegionalAttributeMethod
    {
        #region cliass variables
        private static string _defaultIntegration = string.Empty;
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string _xmlFilePath = @"C:\Program Files (x86)\Miner and Miner\PG&E Custom Components\Config\";//@"C:\Projects\pge\ED\Desktop\PGE.Desktop.EDER\AutoUpdaters\Special\RegionalAttributesAU\Config\";
        public static string _integratnName;//="PointInPolygon";
        private static List<IFeatureLayer> _featLayerList;
        #endregion

        public static string IntegrationName
        {
            get { return _integratnName; }
            set { _integratnName = value; }
        }
        /// <summary>
        /// get the domain mapping name from XML
        /// </summary>
        public static string Initialize
        {
            get
            {
                if (string.IsNullOrEmpty(_defaultIntegration))
                {
                    //Read the config location from the Registry Entry
                    SystemRegistry sysRegistry = new SystemRegistry("PGE");
                    _xmlFilePath = sysRegistry.ConfigPath;
                    //get file path name
                    string domainsXmlFilePath = System.IO.Path.Combine(_xmlFilePath, "ExternalAttrConfig.xml");
                    XmlDocument document = new XmlDocument();
                    if (File.Exists(domainsXmlFilePath))//check for file path
                    {
                        //load file
                        document.Load(domainsXmlFilePath);
                        //initialize the domain manager
                        DomainManager.Instance.Initialize(document.FirstChild, _integratnName);
                        _defaultIntegration = DomainManager.Instance.DefaultIntegration;
                    }
                }
                return _defaultIntegration;
            }
        }
        /// <summary>
        /// Get list of model names
        /// </summary>
        /// <param name="configObjClass">Esri IObjectClass</param>
        /// <param name="fieldName">Table FieldName</param>
        /// <returns>return list of model names</returns>
        public static List<string> GetModelNamesList(IObjectClass configObjClass, string fieldName)
        {
            ITable configTbl = configObjClass as ITable;
            List<string> modelNameList = new List<string>();
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.SubFields = fieldName;
            ICursor modelNameCursr = null;
            IRow row = null;
            try
            {
                modelNameCursr = configTbl.Search(queryFilter, false);
                while ((row = modelNameCursr.NextRow()) != null)//if row is not null
                {
                    //add model name to list
                    string modelName = row.get_Value((row.Fields.FindField(fieldName))).ToString();
                    if (modelNameList.Contains(modelName) == false)
                    {
                        modelNameList.Add(modelName);
                    }

                }
                return modelNameList;
            }
            finally
            {
                //Release COM object
                while (Marshal.ReleaseComObject(modelNameCursr) > 0) { }
                modelNameCursr = null;
            }


        }

        /// <summary>Get feature from spatial serch </summary>
        /// <param name="feat">esri feature</param>
        /// <param name="polyFeatClass">feature class to serch</param>
        /// <returns>return polygon feature</returns>
        public static IFeature GetFeatureFromSpatialQuery(IFeature feat, IFeatureClass polyFeatClass)
        {
            IFeature searchedFeature = null;
            IFeatureCursor featCursor = null;
            try
            {
                //perform spatial filter
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = feat.Shape;
                spatialFilter.GeometryField = polyFeatClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //get featureCursor
                featCursor = polyFeatClass.Search(spatialFilter, false);


                //Get the feature from feature cursor
                while ((searchedFeature = featCursor.NextFeature()) != null)
                {
                    break;
                }
            }
            finally
            {
                //Release COM object
                while (Marshal.ReleaseComObject(featCursor) > 0) { }
                featCursor = null;
            }
            return searchedFeature;

        }

        /// <summary>
        /// get feature from spatial search
        /// </summary>
        /// <param name="geom"> esri geometry</param>
        /// <param name="polyFeatClass">esri feature class to be search</param>
        /// <returns>return feature</returns>
        public static IFeature GetFeatureFromSpatialQuery(IGeometry geom, IFeatureClass polyFeatClass)
        {
            IFeature searchedFeature = null;
            IFeatureCursor featCursor = null;
            try
            {
                //perform spatial filter
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = geom;
                spatialFilter.GeometryField = polyFeatClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //get feature cursor
                featCursor = polyFeatClass.Search(spatialFilter, false);


                //Get the feature from feature cursor
                while ((searchedFeature = featCursor.NextFeature()) != null)
                {
                    break;
                }
                return searchedFeature;
            }
            finally
            {
                //Release COM object
                while (Marshal.ReleaseComObject(featCursor) > 0) { }
                featCursor = null;

            }

        }
        /// <summary>Get feature Class </summary>
        /// <param name="currentMap">Esri Map</param>
        /// <param name="layerName">Layer name</param>
        /// <returns>return featurClass</returns>
        public static IFeatureClass GetFeatureClassToSearch(IMap currentMap, string layerName)
        {
            if (RegionalAttributeFeatClasses.ContainsKey(layerName.ToUpper())) { return RegionalAttributeFeatClasses[layerName.ToUpper()]; }

            //get all layers in IEnumLayer
            IEnumLayer enumerator = currentMap.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
            IFeatureLayer featLayer = null;
            ILayer layer;
            IFeatureClass featClass = null;

            while ((layer = enumerator.Next()) != null)//loop each layer
            {
                if (layer is IFeatureLayer && layer.Valid && layer.Name.ToLower() == layerName.ToLower()) //check layer name matching
                {
                    _logger.Debug(layer.Name);
                    featLayer = layer as IFeatureLayer;
                    featClass = featLayer.FeatureClass;
                    break;
                }
            }


            return featClass;
        }

        public static void AddRegionalAttributeClass(string layerName, IFeatureClass featClass)
        {
            if (!RegionalAttributeFeatClasses.ContainsKey(layerName.ToUpper()))
            {
                RegionalAttributeFeatClasses.Add(layerName.ToUpper(), featClass);
            }
        }

        public static Dictionary<string, IFeatureClass> RegionalAttributeFeatClasses = new Dictionary<string, IFeatureClass>();
        /// <summary>
        /// Get feature Class
        /// </summary>
        /// <param name="layerName">layer name</param>
        /// <returns>return feature class</returns>
        public static IFeatureClass GetFeatureClassToSearch(string layerName)
        {
            if (RegionalAttributeFeatClasses.ContainsKey(layerName.ToUpper())) { return RegionalAttributeFeatClasses[layerName.ToUpper()]; }

            IFeatureClass featClass = null;
            if (_featLayerList.Count > 0)
            {
                for (int i = 0; i < _featLayerList.Count; i++)
                {
                    if (_featLayerList[i].Name.ToLower() == layerName.ToLower())
                    {
                        _logger.Debug(_featLayerList[i].Name);
                        featClass = _featLayerList[i].FeatureClass;
                        break;
                    }
                }
            }
            else
            {
                _logger.Debug("Feature layer count is zero");
            }
            return featClass;
        }

        ///<summary>Get Map from ArcMap</summary>
        ///  
        ///<param name="application">An IApplication interface that is the ArcMap application.</param>
        ///   
        ///<returns>An IMap interface.</returns>
        ///   
        ///<remarks></remarks>
        public static ESRI.ArcGIS.Carto.IMap GetMapFromArcMap(ESRI.ArcGIS.Framework.IApplication application)
        {
            if (application == null)//if null
            {
                _logger.Error("IApplication is null");
                return null;
            }
            //get current map document
            ESRI.ArcGIS.ArcMapUI.IMxDocument mxDocument = ((ESRI.ArcGIS.ArcMapUI.IMxDocument)(application.Document)); // Explicit Cast
            ESRI.ArcGIS.Carto.IActiveView activeView = mxDocument.ActiveView;
            //get IMap
            ESRI.ArcGIS.Carto.IMap map = activeView.FocusMap;
            if (map != null)
            {
                _featLayerList = new List<IFeatureLayer>();
                GetFeatureLayerList(map);
            }
            _logger.Debug("Map Name:" + map.Name);
            return map;

        }

        /// <summary>
        /// get layer list for current map
        /// </summary>
        /// <param name="map">esri map document</param>
        /// <returns>return all layer in list</returns>
        public static List<IFeatureLayer> GetFeatureLayerList(IMap map)
        {
            _featLayerList = new List<IFeatureLayer>();
            IEnumLayer enumerator = map.get_Layers(CartoFacade.UIDFacade.FeatureLayers, true);
            ILayer layer;

            while ((layer = enumerator.Next()) != null)
            {
                if (layer is IFeatureLayer && layer.Valid)
                {
                    IFeatureLayer layerFilter = ((IFeatureLayer)layer);
                    {
                        _featLayerList.Add(layerFilter);
                    }
                }
            }
            return _featLayerList;
        }

        /// <summary> Get value from XML </summary>
        /// <param name="domainsXmlFilePath">File Path</param>
        /// <param name="domain">Domain Name</param>
        /// <param name="value">From to Value</param>
        /// <param name="errorMsg">error messgae if file path not found</param>
        /// <returns> return to value from xml</returns>
        public static string GetValueFromXml(string domain, string value)
        {
            string result = string.Empty;

            // XmlDocument document = new XmlDocument();
            //if (File.Exists(domainsXmlFilePath))
            //{
            try
            {
                if (DomainManager.Instance.DefaultIntegration == Initialize)
                {
                    result = DomainManager.Instance.GetValue(domain, value.ToLower());
                    _logger.Debug("Domain Value: " + result);
                }
                else
                {
                    DomainManager.Instance.DefaultIntegration = Initialize;
                    result = DomainManager.Instance.GetValue(domain, value.ToLower());
                    _logger.Debug("Domain Value: " + result);

                }
            }
            catch (Exception ex)
            {
                _logger.Equals(ex.Message);
                return "";
            }

            return result;
        }




    }
}
