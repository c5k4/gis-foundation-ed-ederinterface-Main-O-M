using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;

namespace PGE.Common.Delivery.Framework
{
    /// <summary>
    /// Singleton with helpers methods to execute the Map Production related tasks
    /// </summary>
    public class PSPSMapProductionFacade
    {
        #region Public Properties
        /// <summary>
        /// 
        /// </summary>
        public static IMap PGEMap { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static IPageLayout PGEPageLayout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public static string PGEYMaxCaption
        {
            get { return "PGE Y Max"; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string PGEXMaxCaption
        {
            get { return "PGE X Max"; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string PGEYMinCaption
        {
            get { return "PGE Y Min"; }
        }
        /// <summary>
        /// 
        /// </summary>
        public static string PGEXMinCaption
        {
            get { return "PGE X Min"; }
        }
        /// <summary>
        /// Caption of the <c>PGE Hash Buffer</c> AutoTextElement
        /// </summary>
        public static string PGECircuitNameCaption
        {
            get { return "#CircuitName#"; }
        }

        /// <summary>
        /// Caption of the <c>PGE Hash Buffer</c> AutoTextElement
        /// </summary>
        public static string PGEPSPSNameCaption
        {
            get { return "#PSPSName#"; }
        }

        /// <summary>
        /// Caption of the <c>PGE Hash Buffer</c> AutoTextElement
        /// </summary>
        public static string PGETotalMileageCaption
        {
            get { return "#TotalMilage#"; }
        }

        private static IGeometry OriginalHashPolygonGeometry
        {
            get;
            set;
        }
        private static ESRI.ArcGIS.Display.ISymbol OriginalHashSymbol
        {
            get;
            set;
        }
        /// <summary>
        /// Logger to log information
        /// </summary>
        private static readonly Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "");
        #endregion Public Properties

        /// <summary>
        /// Returns the ATE with the given caption
        /// </summary>
        /// <param name="pageLayout">Pagelayout to look for the ATE</param>
        /// <param name="autoTextElementCaption">Caption of the ATE looking for </param>
        /// <returns>Returns the first ATE with the given caption</returns>
        public static IElement GetArcFMAutoTextElement(IPageLayout pageLayout, string autoTextElementCaption)
        {
            IElement element = null;

            if (pageLayout == null) return element;

            IGraphicsContainer graphicsContainer = pageLayout as IGraphicsContainer;
            graphicsContainer.Reset();

            //Looping through the Elements of the Map Document to find the element with requested name
            while ((element = graphicsContainer.Next()) != null)
            {
                if (element is IMMAutoTextElement == false) continue;
                IMMAutoTextSource ATESource = (element as IMMAutoTextElement).SourceObject;
                if (ATESource == null) continue;
                if (ATESource.Caption == autoTextElementCaption) break;
            }

            return element;
        }

        /// <summary>
        /// Deletes the ATEs with the given caption
        /// </summary>
        /// <param name="pageLayout">Pagelayout to look for the ATE</param>
        /// <param name="autoTextElementCaption">Caption of the ATE looking for </param>
        /// <param name="recursive">True to look for all ATEs in the pagelayout and false to delete only the first ATE with given caption</param>
        public static void DeleteArcFMAutoTextElement(IPageLayout pageLayout, string autoTextElementCaption, bool recursive)
        {
            if (pageLayout == null) return;


            IGraphicsContainer graphicsContainer = pageLayout as IGraphicsContainer;
            graphicsContainer.Reset();
            IElement element = null;
            //Looping through the Elements of the Map Document to find the element with requested name
            while ((element = graphicsContainer.Next()) != null)
            {
                if (element is IMMAutoTextElement == false) continue;
                IMMAutoTextSource ATESource = (element as IMMAutoTextElement).SourceObject;
                if (ATESource.Caption == autoTextElementCaption)
                {
                    graphicsContainer.DeleteElement(element);
                    if (!recursive) break;
                    graphicsContainer.Reset();
                }
            }

        }

        /// <summary>
        /// Deletes the Element with the given caption
        /// </summary>
        /// <param name="activeView">ActiveView to look for the ATE</param>
        /// <param name="bufferHashElementCaption">Caption of the Hash Buffer element looking for </param>
        /// <param name="recursive">True to look for all elements in the pagelayout and false to delete only the first ATE with given caption</param>
        public static void DeleteElement(IActiveView activeView, string bufferHashElementCaption, bool recursive)
        {
            if (activeView == null) return;


            IGraphicsContainer graphicsContainer = activeView as IGraphicsContainer;
            graphicsContainer.Reset();
            IElement element = null;
            //Looping through the Elements of the Map Document to find the element with requested name
            while ((element = graphicsContainer.Next()) != null)
            {
                if ((element as IElementProperties).Name == bufferHashElementCaption)
                {
                    graphicsContainer.DeleteElement(element);
                    if (!recursive) break;
                    graphicsContainer.Reset();
                }
            }

        }

        /// <summary>
        /// Retrieves the MapGrid Number stored in the AutoTextElement
        /// </summary>
        /// <param name="element">ATE Element</param>
        /// <returns>Returns the MapGrid Number stored in the AutoTextElement</returns>
        public static string GetMapGridNumberFromATE(IElement element)
        {
            if (element == null) return string.Empty;
            object elementCustomProperty = (element as IElementProperties).CustomProperty;
            return (elementCustomProperty == null ? string.Empty : elementCustomProperty.ToString());
        }

        /// <summary>
        /// Sets the MapGrid Number for the AutoTextElement
        /// </summary>
        /// <param name="element">ATE Element</param>
        /// <param name="mapGridNumber">Map Grid Number to be written to the AutoTextElement</param>
        public static void SetMapGridNumberFromATE(IElement element, string mapGridNumber)
        {
            if (element == null) return;
            (element as IElementProperties).CustomProperty = mapGridNumber;
        }

        /// <summary>
        /// Refresh the Map from the PageLayout
        /// </summary>
        /// <param name="pageLayout">PageLayout</param>
        public static void RefreshMapFromPageLayout(IPageLayout pageLayout)
        {

            if (pageLayout != null)
            {
                IGraphicsContainer graphicsContainer = pageLayout as IGraphicsContainer;
                graphicsContainer.Reset();
                IElement element = null;
                //Looping through the Elements of the Map Document to find the element
                while ((element = graphicsContainer.Next()) != null)
                {
                    //Get the IMapFrame object from the GraphicsContainer Elements. 
                    if (element is IMapFrame)
                    {
                        //Get Map from IMapFrame
                        IMap map = (element as IMapFrame).Map;
                        //Cast the IActiveView from Map and refresh the map.
                        (map as IActiveView).Refresh();
                    }
                }
            }

        }

        /// <summary>
        /// Prepare the Hash Geometry with the specified Buffer. Note this will only return Buffer geometry excluding the future like a disc.
        /// </summary>
        /// <param name="polygonGeometry">Grid Feature to created hash buffer around</param>
        /// <param name="buffredGeometry"></param>
        /// <returns>Returns buffered has geometry</returns>
        public static IGeometry PrepareHashGeometry(IGeometry polygonGeometry, IGeometry buffredGeometry)
        {
            ITopologicalOperator topologicalOperator = null;
            try
            {
                if (polygonGeometry == null) return null;
                //IClone polygonGeometryClone = polygonGeometry as IClone;
                //ITopologicalOperator topologicalOperator = (polygonGeometryClone.Clone() as IGeometry) as ITopologicalOperator;
                //Get the buffer distance from config file 
                //IGeometry bufferGeometry = GetBuffredGeometry(polygonGeometry, bufferDistance); //topologicalOperator.Buffer(bufferDistance);
                topologicalOperator = buffredGeometry as ITopologicalOperator;
                topologicalOperator.Simplify();
                IGeometry hashGeometry = topologicalOperator.Difference(polygonGeometry);
                return hashGeometry;
            }
            finally
            {
                if (topologicalOperator != null)
                {
                    while (Marshal.ReleaseComObject(topologicalOperator) > 0) { }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="bufferDistance"></param>
        /// <returns></returns>
        public static IGeometry GetBuffredGeometry(IGeometry geom, double bufferDistance)
        {
            ITopologicalOperator topologicalOperator = null;
            try
            {
                IClone polygonGeometryClone = geom as IClone;
                topologicalOperator = (polygonGeometryClone.Clone() as IGeometry) as ITopologicalOperator;
                //Get the buffer distance from config file 
                IGeometry bufferGeometry = topologicalOperator.Buffer(bufferDistance);

                return bufferGeometry;
            }
            finally
            {
                if (topologicalOperator != null)
                {
                    while (Marshal.ReleaseComObject(topologicalOperator) > 0) { }
                }
            }
        }

        /// <summary>
        /// This methode will return the element whose name is matching.
        /// </summary>
        /// <param name="name">Name of the element(Name is not the case sensetive).</param>
        /// <param name="graphicsContainer">Graphics Container.</param>
        /// <returns>IElement (Matching element with parameter name).</returns>
        public static IElement GetElementByName(string name, IGraphicsContainer graphicsContainer)
        {
            graphicsContainer.Reset();
            IElement element = graphicsContainer.Next();
            IElement foundElement = null;
            while (element != null)
            {
                if ((element as IElementProperties).Name.ToUpper().Equals(name.ToUpper()))
                {
                    foundElement = element;
                    break;
                }
                element = graphicsContainer.Next();
            }

            return foundElement;
        }

        public static void StoreElementPropeties(IElement element)
        {
            if (element == null)
            {
                PSPSMapProductionFacade.OriginalHashPolygonGeometry = null;
                PSPSMapProductionFacade.OriginalHashSymbol = null;
                return;
            }

            //Stores the properties only if they are not stored already
            if (PSPSMapProductionFacade.OriginalHashPolygonGeometry != null || PSPSMapProductionFacade.OriginalHashSymbol != null) return;

            //Store Geometry
            if (element.Geometry == null) PSPSMapProductionFacade.OriginalHashPolygonGeometry = null;
            else
            {
                //MapProductionFacade.OriginalHashPolygonGeometry = objCopy.Copy(element.Geometry) as IGeometry;
                PSPSMapProductionFacade.OriginalHashPolygonGeometry = (element.Geometry as IClone).Clone() as IGeometry;
            }

            //Store Symbol
            IFillShapeElement polygonElement = element as IFillShapeElement;
            if (polygonElement != null && polygonElement.Symbol != null)
            {
                PSPSMapProductionFacade.OriginalHashSymbol = (polygonElement.Symbol as IClone).Clone() as ISymbol;
            }
            else PSPSMapProductionFacade.OriginalHashSymbol = null;
        }

        public static void RestoreElementPropeties(IGraphicsContainer container, IElement element, bool reset)
        {
            if (container == null || element == null) return;
            if (PSPSMapProductionFacade.OriginalHashPolygonGeometry != null)
            {
                element.Geometry = (PSPSMapProductionFacade.OriginalHashPolygonGeometry as IClone).Clone() as IGeometry;
            }
            IFillSymbol symbol = PSPSMapProductionFacade.OriginalHashSymbol as IFillSymbol;
            if (symbol != null)
            {
                (element as IFillShapeElement).Symbol = (symbol as IClone).Clone() as IFillSymbol;
            }
            container.UpdateElement(element);
            if (reset)
            {
                if (PSPSMapProductionFacade.OriginalHashPolygonGeometry != null)
                {
                    while (Marshal.ReleaseComObject(PSPSMapProductionFacade.OriginalHashPolygonGeometry) > 0) { }
                    PSPSMapProductionFacade.OriginalHashPolygonGeometry = null;
                }

                if (PSPSMapProductionFacade.OriginalHashSymbol != null)
                {
                    while (Marshal.ReleaseComObject(PSPSMapProductionFacade.OriginalHashSymbol) > 0) { }
                    PSPSMapProductionFacade.OriginalHashSymbol = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureClassModelName"></param>
        /// <param name="map"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static List<IFeatureLayer> FeatureLayerByModelName(string featureClassModelName, IMap map, bool all)
        {
            List<IFeatureLayer> flList = new List<IFeatureLayer>();
            try
            {
                UID uid = new UIDClass();
                uid.Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}";
                IEnumLayer enumLayer = map.get_Layers(uid, true);
                enumLayer.Reset();
                IFeatureLayer featLayer = null;
                while ((featLayer = (IFeatureLayer)enumLayer.Next()) != null)
                {
                    //if featlayer.featureclass is null then open the featureclass using IworkspaceName.
                    IFeatureClass featureClass = featLayer.FeatureClass;
                    if (featureClass != null)
                    {
                        if (ModelNameFacade.ModelNameManager.ContainsClassModelName(featLayer.FeatureClass, featureClassModelName))
                        {
                            flList.Add(featLayer);
                            if (!all)
                            {
                                return flList;
                            }
                        }
                    }
                }
                while (Marshal.ReleaseComObject(enumLayer) > 0) { }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return flList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="map"></param>
        /// <returns></returns>
        public static IGeometry ProjectGeometryToMapCoordinateSystem(IGeometry geometry, IMap map)
        {
            IGeometry2 geometry2 = null;
            IProjectedCoordinateSystem fromPCS = null;
            IProjectedCoordinateSystem toPCS = null;
            IGeographicCoordinateSystem fromGCS = null;
            IGeographicCoordinateSystem toGCS = null;
            ISpatialReference geometrySpatialRefrence = geometry.SpatialReference;
            ISpatialReference toSpatialReference = map.SpatialReference;
            //If the spatial references are the same just return the Geometry
            if (toSpatialReference is IGeographicCoordinateSystem)
            {
                toGCS = toSpatialReference as IGeographicCoordinateSystem;
            }
            else
            {
                toPCS = toSpatialReference as IProjectedCoordinateSystem;
                toGCS = toPCS.GeographicCoordinateSystem;
            }
            if (geometrySpatialRefrence is IGeographicCoordinateSystem)
            {
                fromGCS = geometrySpatialRefrence as IGeographicCoordinateSystem;
            }
            else
            {
                fromPCS = geometrySpatialRefrence as IProjectedCoordinateSystem;
                fromGCS = fromPCS.GeographicCoordinateSystem;
            }

            IMapGeographicTransformations mapGeoTransformation = map as IMapGeographicTransformations;
            IGeoTransformationOperationSet geoTransSet = mapGeoTransformation.GeographicTransformations;
            IGeoTransformation geoTransformation = null;
            esriTransformDirection transformDirection = esriTransformDirection.esriTransformForward;

            ISpatialReference fromSpReference = null;
            ISpatialReference toSpReference = null;
            bool spatialTransformationFound = false;
            geoTransSet.Reset();
            for (int i = 0; i < geoTransSet.Count; i++)
            {
                geoTransSet.Next(out transformDirection, out geoTransformation);
                if (geoTransformation != null)
                {
                    geoTransformation.GetSpatialReferences(out fromSpReference, out toSpReference);
                    if (transformDirection == esriTransformDirection.esriTransformForward)
                    {
                        if (fromSpReference.Name == fromGCS.Name && toSpReference.Name == toGCS.Name)
                        {
                            spatialTransformationFound = true;
                            break;
                        }
                    }
                    else if (transformDirection == esriTransformDirection.esriTransformReverse)
                    {
                        if (fromSpReference.Name == toGCS.Name && toSpReference.Name == fromGCS.Name)
                        {
                            spatialTransformationFound = true;
                            break;
                        }
                    }

                }//if (geoTransformation != null)
            }//for (int i = 0; i < geoTransSet.Count; i++)
            geometry2 = geometry as IGeometry2;
            if (spatialTransformationFound)
            {
                geometry2.ProjectEx(toSpatialReference, transformDirection, geoTransformation, false, 0, 0);
            }
            else
            {
                geometry2.ProjectEx(toSpatialReference, transformDirection, null, false, 0, 0);
            }
            return geometry2 as IGeometry;
        }

        /// <summary>
        /// Assumed that the gemoetry passed in here is always going to be in WGS 1984 Geographic Coordinate system and not GCS 1927 or HARN
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IGeometry ProjectToWGS84(IGeometry geometry)
        {
            int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;

            ISpatialReferenceFactory pSRF = new SpatialReferenceEnvironmentClass();

            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem m_GeographicCoordinateSystem;

            m_GeographicCoordinateSystem = pSRF.CreateGeographicCoordinateSystem(geoType);

            ESRI.ArcGIS.Geometry.IPoint pPoint = new ESRI.ArcGIS.Geometry.PointClass();
            IGeometry geom = (IGeometry)((IClone)geometry).Clone();
            geom.Project(m_GeographicCoordinateSystem);
            return geom;
        }

        /// <summary>
        /// Holds a reference to the Current Map Grid being exported.
        /// </summary>
        public static IFeature CurrentMapGridFeature { get; set; }

        /// <summary>
        /// Holds a reference to the Primary Key (MapNumber : MapOffice) of Current Map Grid being exported.
        /// </summary>
        public static string CurrentMapGridPrimaryKey { get; set; }

    }
}
