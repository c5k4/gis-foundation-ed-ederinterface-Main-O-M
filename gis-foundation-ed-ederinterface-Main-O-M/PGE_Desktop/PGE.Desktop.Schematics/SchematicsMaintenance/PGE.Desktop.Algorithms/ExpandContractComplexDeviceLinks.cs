using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Schematic;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

namespace PGE.Desktop.SchematicsMaintenance.Algorithms
{
    /// <summary>
    /// Defines an Esri Schematic algorithm, which expands/or contracts the link length of complex device links.
    /// </summary>
    [ClassInterface(ClassInterfaceType.None)]
    [Guid(ExpandContractComplexDeviceLinks.GUID)]
    [ProgId(ExpandContractComplexDeviceLinks.PROGID)]
    [ComVisible(true)]
    public class ExpandContractComplexDeviceLinks : ISchematicAlgorithm, ISchematicJSONParameters, IExpandContractComplexDeviceLinks
    {

        #region Component Category Registration

        /// <summary>
        /// COM component category registration.
        /// </summary>
        /// <param name="regKey">The registry key of the object to register in the schematic algorithm category.</param>
        [ComRegisterFunction()]
        public static void Reg(string regKey)
        {
            SchematicAlgorithms.Register(regKey);
        }

        /// <summary>
        /// COM component category registration.
        /// </summary>
        /// <param name="regKey">The registry key of the object to unregister in the schematic algorithm category.</param>
        [ComUnregisterFunction()]
        public static void Unreg(string regKey)
        {
            SchematicAlgorithms.Unregister(regKey);
        }

        #endregion

        #region Constants

        /// <summary>
        /// String identifying a device group name.
        /// </summary>
        private const string DeviceGroupFeatureClassName = "DeviceGroup";

        /// <summary>
        /// String identifying the Dist Bus Bar feature class.
        /// </summary>
        private const string DistBusBarFeatureClassName = "DistBusBar";

        // COM registration IDs
        public const string GUID = "59FCEFBD-9160-4014-8C43-6DB17ABDA963";
        private const string PROGID = "PGE.Desktop.SchematicsMaintenance.AlgorithmProperty.ExpandContractComplexDeviceLinks";

        // Property names (for the algorithm property set)
        private const string ExpandContractFactorName = "Expand Contract Factor";

        // the JSON parameter names 
        private const string JSONExpandContractFactor = "ExpandContractFactor";

        // Algorithms parameters JSON representation Names used by the REST interface 
        private const string JSONName = "name";
        private const string JSONType = "type";
        private const string JSONValue = "value";

        // Algorithms parameters JSON representation Types used by the REST interface
        private const string JSONLong = "Long";
        private const string JSONDouble = "Double";
        private const string JSONBoolean = "Boolean";
        private const string JSONString = "String";

        #endregion

        #region Members

        private string _algorithmLabel = "Expand/Contract Device Group Links";

        private bool _available;
        private bool _overridable;
        private bool _useRootNode;
        private bool _useEndNode;

        /// <summary>
        /// The factor of expansion/contraction to apply to the link length.
        /// </summary>
        private double _expandContractFactor;

        private ISchematicDiagramClassName _schematicDiagramClassName;
        #endregion

        #region Constructors/Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ExpandContractComplexDeviceLinks()
        {
            _expandContractFactor = 5.0;
            _available = true;// In this example, the algorithm is available by default
            _overridable = true; // user is allowed to edit the parameters
            _useRootNode = false; // don't need the user to define root nodes
            _useEndNode = false; // don't need the user to define an end node

            _schematicDiagramClassName = null;

        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~ExpandContractComplexDeviceLinks()
        {
            _schematicDiagramClassName = null;
        }


        #endregion

        #region Properties

        /// <summary>
        /// The factor of expansion/contraction to apply to the link length.
        /// </summary>
        public double ExpandContractFactor
        {
            get
            {
                return _expandContractFactor;
            }
            set
            {
                _expandContractFactor = value;
            }
        }

        #endregion

        #region Methods

        // ISchematicAlgorithm interface : Defines its properties and methods (mandatory)
        #region ISchematicAlgorithm Interface Members


        public bool get_Enabled(ISchematicLayer schematicLayer)
        {
            try
            {
                if (schematicLayer == null)
                {
                    return false;
                }

                // an algorithm needs the diagram to be in editing mode in order to run
                if (!schematicLayer.IsEditingSchematicDiagram())
                {
                    return false;
                }

                IEnumSchematicFeature enumFeatures = schematicLayer.GetSchematicSelectedFeatures(true);
                bool hasSelection = false;
                // Count the selected group device nodes
                int groupDeviceCount = 0;
                if (enumFeatures == null && enumFeatures.Count > 0)
                {
                    hasSelection = true;
                    
                    ISchematicFeature feature;
                    enumFeatures.Reset();
                    feature = enumFeatures.Next();
                    while (feature != null)
                    {
                        ISchematicInMemoryFeatureClass inMemoryFeatureClass = feature.Class as ISchematicInMemoryFeatureClass;
                        IDataset dataset = inMemoryFeatureClass.SchematicElementClass as IDataset;
                        if (inMemoryFeatureClass != null &&
                            inMemoryFeatureClass.SchematicElementClass.SchematicElementType == esriSchematicElementType.esriSchematicNodeType &&
                            dataset != null &&
                            (dataset.Name == ExpandContractComplexDeviceLinks.DeviceGroupFeatureClassName ||
                            dataset.Name.EndsWith("." + ExpandContractComplexDeviceLinks.DeviceGroupFeatureClassName)))
                        {
                            groupDeviceCount++;
                            break;
                        }
                        feature = enumFeatures.Next();
                    }
                }

                if ((hasSelection && groupDeviceCount > 0) || !hasSelection)
                {
                    return true; // just need at least 1 group device, or no selections at all
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                // Eat, so application does not crash
            }
            return false;
        }

        public bool Available
        {
            get
            {
                return _available;
            }
            set
            {
                _available = value;
            }
        }

        public bool Overridable
        {
            get
            {
                return _overridable;
            }
            set
            {
                _overridable = value;
            }
        }

        public ISchematicDiagramClassName SchematicDiagramClassName
        {
            get
            {
                return _schematicDiagramClassName;
            }
            set
            {
                _schematicDiagramClassName = value;
            }
        }

        public string Label
        {
            get
            {
                return _algorithmLabel;
            }
            set
            {
                _algorithmLabel = value;
            }
        }

        public bool UseRootNode
        {
            get
            {
                return _useRootNode;
            }
        }

        public bool UseEndNode
        {
            get
            {
                return _useEndNode;
            }
        }

        public IPropertySet PropertySet
        {
            get
            {
                // build the property set
                IPropertySet propSet = new PropertySet();
                try
                {
                    if (propSet == null)
                        return null;

                    propSet.SetProperty(ExpandContractFactorName, _expandContractFactor);
                }
                catch
                {
                    // Eat, so application does not crash
                }
                return propSet;
            }
            set
            {
                IPropertySet pPropertySet = value;

                if (pPropertySet != null)
                {
                    try
                    {
                        _expandContractFactor = (double)pPropertySet.GetProperty(ExpandContractFactorName);
                    }
                    catch
                    {
                        // Catch exception at interface boundary to avoid application crashes
                    }
                }
            }
        }

        public string AlgorithmCLSID
        {
            get
            {
                //return "{" + GUID + "}"; Working as well with GUID
                return PROGID;
            }
        }

        // The execute part of the algorithm
        public void Execute(ISchematicLayer schematicLayer, ITrackCancel CancelTracker)
        {
            try
            {
                if (schematicLayer == null)
                {
                    return;
                }

                // Before Execute part
                ISchematicInMemoryDiagram inMemoryDiagram = null;
                try
                {
                    inMemoryDiagram = schematicLayer.SchematicInMemoryDiagram;

                    // Core algorithm
                    InternalExecute(schematicLayer, inMemoryDiagram, CancelTracker);
                }
                finally
                {
                    // Release the COM objects
                    if (inMemoryDiagram != null)
                    {
                        while (System.Runtime.InteropServices.Marshal.ReleaseComObject(inMemoryDiagram) > 0) { }
                    }

                    if (schematicLayer != null)
                    {
                        while (System.Runtime.InteropServices.Marshal.ReleaseComObject(schematicLayer) > 0) { }
                    }
                }
            }
            catch
            {
                // Eat, so application does not crash
                // TODO: Verify need here, could be expected case.
            }
        }


        // The execute part of the algorithm
        private void InternalExecute(ISchematicLayer schematicLayer, ISchematicInMemoryDiagram inMemoryDiagram, ITrackCancel cancelTracker)
        {
            if (schematicLayer == null || inMemoryDiagram == null)
            {
                return;
            }

            // get the diagram spatial reference for geometry transformation
            IGeoDataset geoDataset = (IGeoDataset)inMemoryDiagram;
            if (geoDataset == null)
            {
                return;
            }

            ISpatialReference spatialReference = geoDataset.SpatialReference;

            ISchematicDiagramClass diagramClass;
            diagramClass = inMemoryDiagram.SchematicDiagramClass;
            if (diagramClass == null)
            {
                return;
            }

            ISchematicDataset schemDataset;
            schemDataset = diagramClass.SchematicDataset;
            if (schemDataset == null)
            {
                return;
            }

            ISchematicAlgorithmEventsTrigger algorithmEventsTrigger;
            algorithmEventsTrigger = schemDataset as ISchematicAlgorithmEventsTrigger;
            if (algorithmEventsTrigger == null)
            {
                return;
            }

            ILayer layer = schematicLayer as ILayer;
            if (layer == null)
            {
                return;
            }

            ISchematicAlgorithm algorithm = this as ISchematicAlgorithm;
            if (algorithm == null)
            {
                return;
            }

            // Verify the algorithm can be executed.
            bool canExecute = true;
            algorithmEventsTrigger.FireBeforeExecuteAlgorithm(layer, algorithm, ref canExecute);
            if (!canExecute)
            {
                return; // cannot execute
            }

            // Helper to go between geometric network features and schematic in memory features.
            ISchematicFeatureLinker linker = new SchematicLinkerClass();

            // Get the selected Features
            IEnumSchematicFeature enumFeatures = schematicLayer.GetSchematicSelectedFeatures(true);
            bool hasSelection = false;
            List<IFeature> deviceGroupSchematicFeatures = new List<IFeature>();
            if (enumFeatures != null && enumFeatures.Count > 0)
            {
                hasSelection = true;

                ISchematicFeature feature;
                enumFeatures.Reset();
                feature = enumFeatures.Next();
                while (feature != null && cancelTracker.Continue())
                {
                    ISchematicInMemoryFeatureClass inMemoryFeatureClass = feature.Class as ISchematicInMemoryFeatureClass;
                    ISchematicElementClass elementClass = inMemoryFeatureClass.SchematicElementClass as ISchematicElementClass;
                    if (inMemoryFeatureClass != null &&
                        inMemoryFeatureClass.SchematicElementClass.SchematicElementType == esriSchematicElementType.esriSchematicNodeType &&
                        elementClass != null &&
                        (elementClass.Name == ExpandContractComplexDeviceLinks.DeviceGroupFeatureClassName) ||
                        elementClass.Name.EndsWith("." + ExpandContractComplexDeviceLinks.DeviceGroupFeatureClassName))
                    {
                        deviceGroupSchematicFeatures.Add(feature);
                    }
                    feature = enumFeatures.Next();
                }
            }

            // Exit if user has canceled operation
            if (!cancelTracker.Continue())
            {
                return;
            }

            if (hasSelection && deviceGroupSchematicFeatures.Count > 0)
            {
                // Use the groupDevicesFeatures list that was populated above
            }
            else if (!hasSelection)
            {
                deviceGroupSchematicFeatures = GetAllDeviceGroupFeatures(schematicLayer);
            }
            else
            {
                // Does not meet the criteria to continue
                // Must have at least one device group selected if there is a selection
                // Must have no features selected
                return;
            }

            // Exit if user has canceled operation
            if (!cancelTracker.Continue())
            {
                return;
            }

            // Get DeviceGroup geometric-network features
            var deviceGroupRealFeatureLookup = SchematicFeaturesToFeatures(linker, deviceGroupSchematicFeatures);

            
            // Get associated geometric-network features of type DistBusBar
            if (deviceGroupRealFeatureLookup.Count == 0)
            {
                return;
            }
            // Pull out just the real features.
            var deviceGroupRealFeatures = deviceGroupRealFeatureLookup.Keys.ToList();

            // Exit if user has canceled operation
            if (!cancelTracker.Continue())
            {
                return;
            }

            // Get the DistBusBar features associated with the device groups
            var distBusBarRealFeatures = GetDistBusBarFeaturesFromDeviceGroup(deviceGroupRealFeatures);

            // Exit if user has canceled operation
            if (!cancelTracker.Continue())
            {
                return;
            }

            // Get the in-memory schematic features of the real dist bus bar features
            var distBusBarSchematicFeatures = FeaturesToSchematicFeatures(schematicLayer, distBusBarRealFeatures);

            // Exit if user has canceled operation
            if (!cancelTracker.Continue())
            {
                return;
            }

            // Resize DistBusBars by adjusting length in both directions by 1/2 the factor defined
            if (!ExpandContractLinks(distBusBarSchematicFeatures, deviceGroupRealFeatureLookup, this.ExpandContractFactor, cancelTracker))
            {
                return;
            }


            // Exit if user has canceled operation
            if (!cancelTracker.Continue())
            {
                return;
            }
            
            // After Execution
            algorithmEventsTrigger.FireAfterExecuteAlgorithm(layer, algorithm);

            // update the diagram extent
            //schematicLayer.UpdateExtent();
        }

        /// <summary>
        /// Expand and contracts the input link features by the input factor.
        /// </summary>
        /// <param name="linkFeatures">
        /// The Features to expand/contract.
        /// </param>
        /// <param name="expandContractFactor">
        /// The factor to expand/contract by.
        /// </param>
        /// <returns>
        /// True if any links are altered, false otherwise.
        /// </returns>
        private bool ExpandContractLinks(Dictionary<IFeature,List<ISchematicFeature>> featureGroups, Dictionary<IFeature, IFeature> deviceGroupFeatureLookup, double expandContractFactor, ITrackCancel cancelTracker)
        {
            bool result = false;
            IGeometryCollection geometryCollection = null;

            
            foreach (var featureGroup in featureGroups)
            {
                geometryCollection = new GeometryBagClass();
                ((IGeometry)geometryCollection).SpatialReference = featureGroup.Key.Shape.SpatialReference;

                // Put all link geometries in a geometry bag
                foreach (var feature in featureGroup.Value)
                {
                    ISchematicInMemoryFeatureLink link = feature as ISchematicInMemoryFeatureLink;

                    if (link != null)
                    {
                        if (!cancelTracker.Continue())
                        {
                            result = false;
                            break;
                        }
                        geometryCollection.AddGeometry(link.ShapeCopy);
                    }
                }
                IFeature schematicDeviceGroupFeature = null;

                // Scale all the geometries around the device group
                if (geometryCollection.GeometryCount > 0)
                {
                    if (deviceGroupFeatureLookup.ContainsKey(featureGroup.Key))
                    {
                        schematicDeviceGroupFeature = deviceGroupFeatureLookup[featureGroup.Key];
                        // Add the device group too, to ensure that it is moved if the network dictates movement during scaling
                        geometryCollection.AddGeometry(schematicDeviceGroupFeature.ShapeCopy);
                    }

                    
                    ITransform2D transform = geometryCollection as ITransform2D;
                    IPoint centroid = featureGroup.Key.Shape as IPoint;
                    transform.Scale(centroid, expandContractFactor, expandContractFactor);
                }

                // Copy scaled shapes to actual features and store features
                int i = 0;
                foreach (var feature in featureGroup.Value)
                {
                    ISchematicInMemoryFeatureLink link = feature as ISchematicInMemoryFeatureLink;
                    if (link != null)
                    {
                        link.FromNode.Shape = ((IPolyline)geometryCollection.get_Geometry(i)).FromPoint;
                        link.ToNode.Shape = ((IPolyline)geometryCollection.get_Geometry(i)).ToPoint;
                        i++;
                        link.Store();
                        result = true;
                    }

                }

                if (result && schematicDeviceGroupFeature != null)
                {
                    // Update the device group too
                    var point = geometryCollection.get_Geometry(i) as IPoint;
                    if (point != null)
                    {
                        schematicDeviceGroupFeature.Shape = point;
                        schematicDeviceGroupFeature.Store();
                    }
                }
            }

            return result;
        }

        private bool ExpandContractLink(ISchematicInMemoryFeatureLink link, double expandContractFactor)
        {
            bool result = false;
            // Get a copy of the the in-memory link geometry
            IPolyline inMemoryPolylineCopy = link.ShapeCopy as IPolyline;
            if (inMemoryPolylineCopy == null)
            {
                return result;
            }

            var inMemoryPolylineCopyLength = inMemoryPolylineCopy.Length;

            if (inMemoryPolylineCopyLength <= 0.0)
            {
                //MessageBox.Show("Link line length less than zero. Unable to continue.", "Link Length Command", MessageBoxButton.OK, MessageBoxImage.Warning);
                return result;
            }

            ICurve curve = inMemoryPolylineCopy as ICurve;
            if (curve == null)
            {
                //MessageBox.Show("Invalid geometry encountered. Unable to continue.", "Link Length Command", MessageBoxButton.OK, MessageBoxImage.Warning);
                return result; // Cannot work on reflexive link (same FROM and TO)
            }

            var newFromPoint = ConstructPointAlong((expandContractFactor / 2), curve, esriSegmentExtension.esriExtendTangentAtTo, true);

            var tempPoint = curve.FromPoint;
            curve.FromPoint = curve.ToPoint;
            curve.ToPoint = tempPoint;

            var newToPoint = ConstructPointAlong((expandContractFactor / 2), curve, esriSegmentExtension.esriExtendTangentAtTo, true);

            link.ToNode.Shape = newFromPoint;
            link.FromNode.Shape = newToPoint;

            link.Store();
            return result;
        }

        public IPoint ConstructPointAlong(double distance, ICurve curve, esriSegmentExtension extension, bool asRatio)
        {
            IConstructPoint2 contructionPoint = new PointClass();
            contructionPoint.ConstructAlong(curve, extension, distance, asRatio);
            return contructionPoint as IPoint;
        }

        private bool ExpandContractLink(ISchematicInMemoryFeatureLink link, bool isFrom, double expandContractFactor, double inMemoryPolylineCopyLength = 0.0)
        {
            bool result = false;
            // Get a copy of the the in-memory link geometry
            IPolyline inMemoryPolylineCopy = link.ShapeCopy as IPolyline;
            if (inMemoryPolylineCopy == null)
            {
                return result;
            }
            if (isFrom)
            {
                inMemoryPolylineCopyLength = inMemoryPolylineCopy.Length;
            }
            if (inMemoryPolylineCopyLength <= 0.0)
            {
                //MessageBox.Show("Link line length less than zero. Unable to continue.", "Link Length Command", MessageBoxButton.OK, MessageBoxImage.Warning);
                return result;
            }

            ICurve curve = inMemoryPolylineCopy as ICurve;
            if (curve == null)
            {
                //MessageBox.Show("Invalid geometry encountered. Unable to continue.", "Link Length Command", MessageBoxButton.OK, MessageBoxImage.Warning);
                return result; // Cannot work on reflexive link (same FROM and TO)
            }

            // Modify the link geometry
            ICurve outputCurve;
            //distance by ratio (0 = start of curve and 1 = end of curve, outside curve either < 0 or > 1)
            // One half of the expand/contract factor, because both ends will be extended/contracted
            double ratioDistance = inMemoryPolylineCopyLength;

            // 5 / 100 ... 0.05 extends 5
            // 5 / 100 =  
            ratioDistance = (expandContractFactor / 2) ;

            if (ratioDistance < 1)
            {
                // Reduce geometry
                if (isFrom)
                {
                    curve.GetSubcurve((1.0 - ratioDistance), 1.0, true, out outputCurve);  // reduce at from node
                }
                else
                {
                    curve.GetSubcurve(0.0, ratioDistance, true, out outputCurve); // reduce at to node
                }
            }
            else if (ratioDistance > 1)
            {
                // Extend geometry
                IClone clone = curve as IClone;
                outputCurve = clone.Clone() as ICurve;
                IPointCollection points = outputCurve as IPointCollection;
                IPoint tangentPoint = new PointClass(); // must be co-created prior to get it from the curve

                if (isFrom)
                {
                    curve.QueryPoint(esriSegmentExtension.esriExtendTangentAtFrom, (1.0 - ratioDistance), true, tangentPoint); // extend first segment at from node
                    points.UpdatePoint(0, tangentPoint);
                }
                else
                {
                    curve.QueryPoint(esriSegmentExtension.esriExtendTangentAtTo, ratioDistance, true, tangentPoint); // extend last segment at to node
                    points.UpdatePoint((points.PointCount - 1), tangentPoint);
                }
            }
            else
            {
                return result; // Nothing to do (exact length)
            }

            // the output curve holds the new link geometry
            if (outputCurve == null || outputCurve.IsEmpty)
            {
                //MessageBox.Show("Modified geometry is invalid. Unable to continue.", "Link Length Command", MessageBoxButton.OK, MessageBoxImage.Error);
                return result;
            }

            // Store the changes to the in-memory feature link 
            IGeometry newGeom = outputCurve as IGeometry;
            link.Shape = newGeom;
            link.Store();

            // Store the new node position
            IPoint newPoint = new PointClass(); // newPoint must be co-created prior to get it from the curve
            ISchematicInMemoryFeatureNode schematicInMemoryFeatureNode;
            if (isFrom)
            {
                outputCurve.QueryFromPoint(newPoint);
                schematicInMemoryFeatureNode = link.FromNode;
            }
            else
            {
                outputCurve.QueryToPoint(newPoint);
                schematicInMemoryFeatureNode = link.ToNode;
            }
            schematicInMemoryFeatureNode.Shape = newPoint;
            schematicInMemoryFeatureNode.Store();
            result = true;
            if (isFrom)
            {
                result |= ExpandContractLink(link, false, expandContractFactor, inMemoryPolylineCopyLength);
            }
            return result;
        }


        /// <summary>
        /// Retrieves all the dist bus bars that are related to device groups.
        /// </summary>
        /// <param name="deviceGroupFeatures">The device group real features to derive the associated dist bus bar features from.</param>
        /// <returns>The device group associated dist bus bar features.</returns>
        private Dictionary<IFeature,List<IFeature>> GetDistBusBarFeaturesFromDeviceGroup(List<IFeature> deviceGroupFeatures)
        {
            Dictionary<IFeature, List<IFeature>> result = new Dictionary<IFeature, List<IFeature>>();
            if (deviceGroupFeatures.Count > 0)
            {
                // Get the relationship class for the DeviceGroup->DistBusBar relationship class
                var relationshipClass = GetDeviceGroupDistBusBarRelationshipClasses(deviceGroupFeatures[0].Class);
                if (relationshipClass != null)
                {
                    foreach (var deviceGroupFeature in deviceGroupFeatures)
                    {
                        try
                        {
                            var relatedObjects = relationshipClass.GetObjectsRelatedToObject(deviceGroupFeature);

                            if (relatedObjects != null)
                            {
                                var newList = new List<IFeature>();
                                // Append the list together
                                AppendDeviceGroupFeatures(relatedObjects, ref newList);
                                result.Add(deviceGroupFeature, newList);
                            }
                        }
                        catch (COMException ex)
                        {
                            // Different feature class, ignore
                            //Logger.Log.Debug(ex);
                            throw;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Takes a set of source features and adds them to an existing feature collection.
        /// </summary>
        /// <param name="source">The source IFeature set.</param>
        /// <param name="destination">The destination collection.</param>
        /// <exception cref="ArgumentNullException">Thrown if the destination parameter is null.</exception>
        private void AppendDeviceGroupFeatures(ISet source, ref List<IFeature> deviceGroupFeatures)
        {
            source.Reset();

            IFeature feature = source.Next() as IFeature;
            while (feature != null)
            {
                try
                {
                    // Only adds if not present
                    deviceGroupFeatures.Add(feature);
                }
                catch (Exception ex)
                {
                    throw;
                    //Logger.Log.Error("Feature device group keyFieldName could not be found.", ex);
                }

                feature = source.Next() as IFeature;
            }
        }

        /// <summary>
        /// Retrieves all device group features from the schematic in-memory diagram.
        /// </summary>
        /// <param name="schematicLayer">
        /// The schematic layer to retrieve data from.
        /// </param>
        /// <param name="deviceGroupElementClass">
        /// The DeviceGroup element class (feature class).
        /// </param>
        /// <returns>
        /// A list of device group features, or a list with a count of zero if none are found.
        /// </returns>
        private List<IFeature> GetAllDeviceGroupFeatures(ISchematicLayer schematicLayer)
        {
            List<IFeature> deviceGroupFeatures = new List<IFeature>();
            var deviceGroupElementClass = GetDeviceGroupElementClass(schematicLayer);

            var features = schematicLayer.SchematicInMemoryDiagram.GetSchematicInMemoryFeaturesByClass(deviceGroupElementClass);
            if (features != null && features.Count > 0)
            {
                features.Reset();
                var feature = features.Next();
                while (feature != null)
                {
                    deviceGroupFeatures.Add(feature);
                    feature = features.Next();
                }
            }
            return deviceGroupFeatures;
        }

        /// <summary>
        /// Get a reference to the DeviceGroup feature class.
        /// </summary>
        /// <returns>The DeviceGroup element class.</returns>
        private ISchematicElementClass GetDeviceGroupElementClass(ISchematicLayer schematicLayer)
        {
            ISchematicElementClass result = null;

            IEnumSchematicElementClass elementClasses = schematicLayer.SchematicInMemoryDiagram.SchematicDiagramClass.AssociatedSchematicElementClasses;
            if (elementClasses != null && elementClasses.Count > 0)
            {
                elementClasses.Reset();
                var elementClass = elementClasses.Next();
                while (elementClass != null)
                {
                    if (elementClass.Name == ExpandContractComplexDeviceLinks.DeviceGroupFeatureClassName ||
                        elementClass.Name.EndsWith("." + ExpandContractComplexDeviceLinks.DeviceGroupFeatureClassName))
                    {
                        result = elementClass;
                        break;
                    }
                    elementClass = elementClasses.Next();
                }
            }
            return result;
        }

        /// <summary>
        /// Retrieves relationship classes 
        /// </summary>
        /// <param name="objectClass">The device group feature class.</param>
        /// <param name="role">Role of the feature class in the relationship class.</param>
        /// <returns>A list of all relationship classes that me</returns>
        public IRelationshipClass GetDeviceGroupDistBusBarRelationshipClasses(
            IObjectClass objectClass)
        {
            IRelationshipClass result = null;

            if (objectClass != null)
            {
                var relationshipClasses =
                    objectClass.get_RelationshipClasses(esriRelRole.esriRelRoleOrigin);

                if (relationshipClasses != null)
                {
                    relationshipClasses.Reset();

                    var relationshipClass = relationshipClasses.Next();
                    while (relationshipClass != null)
                    {
                        IDataset dataset = relationshipClass.DestinationClass as IDataset;

                        if (dataset != null && 
                            (dataset.Name == ExpandContractComplexDeviceLinks.DistBusBarFeatureClassName) ||
                            dataset.Name.EndsWith("." + ExpandContractComplexDeviceLinks.DistBusBarFeatureClassName))
                        {
                            // Ensure it's not an anno relationship
                            IGeoDataset geoDataset = relationshipClass.DestinationClass as IGeoDataset;
                            IGeoDataset geoDataset2 = relationshipClass.OriginClass as IGeoDataset;
                            if (geoDataset != null && geoDataset2 != null)
                            {
                                result = relationshipClass;
                                break;
                            }
                        }

                        relationshipClass = relationshipClasses.Next();
                    }
                }
            }

            return result;
        }

        #region Geometric Network Methods

        /// <summary>
        /// Takes in-memory schematic features and retrieves the associated 'real' (geometric network / feature class) features.
        /// </summary>
        /// <param name="linker">The linker object to connect between real and schematic features.</param>
        /// <param name="schemataicfeatures">The schematic features to retrieve the linked real features of.</param>
        /// <returns>The associated real features.</returns>
        public static List<IFeature> SchematicFeaturesToFeatures(ISchematicFeatureLinker linker, IEnumSchematicFeature schemataicfeatures)
        {
            var features = new List<IFeature>();

            if (schemataicfeatures != null)
            {
                schemataicfeatures.Reset();
                var schematicFeature = schemataicfeatures.Next();
                while (schematicFeature != null)
                {
                    IObject linkedFeature = null;
                    var linkedFeatures = linker.FindObjectsFromSchematicFeature(schematicFeature);
                    if (linkedFeatures != null)
                    {
                        linkedFeature = linkedFeatures.Next();
                        while (linkedFeature != null)
                        {
                            features.Add(linkedFeature as IFeature);
                            linkedFeature = linkedFeatures.Next();
                        }
                    }
                    schematicFeature = schemataicfeatures.Next();
                }
            }

            return features;
        }

        /// <summary>
        /// Takes in-memory schematic features and retrieves the associated 'real' (geometric network / feature class) features.
        /// </summary>
        /// <param name="linker">The linker object to connect between real and schematic features.</param>
        /// <param name="schemataicfeatures">The schematic features to retrieve the linked real features of.</param>
        /// <returns>The associated schematic features as values, keyed by their linked real features.</returns>
        public static Dictionary<IFeature, IFeature> SchematicFeaturesToFeatures(ISchematicFeatureLinker linker, IEnumerable<IFeature> schemataicfeatures)
        {
            var features = new Dictionary<IFeature, IFeature>();
            foreach (var schematicFeature in schemataicfeatures)
            {
                IObject linkedFeature = null;
                var linkedFeatures = linker.FindObjectsFromSchematicFeature(schematicFeature as ISchematicFeature);
                if (linkedFeatures != null)
                {
                    linkedFeature = linkedFeatures.Next();
                    while (linkedFeature != null)
                    {
                        features.Add(linkedFeature as IFeature, schematicFeature as IFeature);
                        linkedFeature = linkedFeatures.Next();
                    }
                }
            }

            return features;
        }

        /// <summary>
        /// Takes 'real' (geometric network / feature class) features and returns the associated in-memory schematic features.
        /// </summary>
        /// <param name="schematicLayer">The current schematic layer.</param>
        /// <param name="features">The real features to retrieve in-memory features for.</param>
        /// <returns>In-memory schematic features.</returns>
        public static Dictionary<IFeature, List<ISchematicFeature>> FeaturesToSchematicFeatures(ISchematicLayer schematicLayer, Dictionary<IFeature, List<IFeature>> featureGroups)
        {
            var schematicFeatures = new Dictionary<IFeature, List<ISchematicFeature>>();
            foreach (var featuresGroup in featureGroups)
            {
                var newList = new List<ISchematicFeature>();
                foreach (var feature in featuresGroup.Value)
                {
                    if (feature.Class != null && feature.Shape != null)
                    {
                        
                        var schematicID = feature.Class.ObjectClassID + "-" + feature.OID + "-" + 0;
                        if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
                        {
                            var schematicFeature = schematicLayer.SchematicInMemoryDiagram.GetSchematicInMemoryFeatureByType(
                                esriSchematicElementType.esriSchematicNodeType,
                                schematicID);
                            if (schematicFeature != null)
                            {
                                newList.Add(schematicFeature);
                            }
                            else
                            {
                                schematicFeature = schematicLayer.SchematicInMemoryDiagram.GetSchematicInMemoryFeatureByType(
                                    esriSchematicElementType.esriSchematicNodeOnLinkType,
                                    schematicID);
                                if (schematicFeature != null)
                                {
                                    newList.Add(schematicFeature);
                                }
                            }
                        }
                        else if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                        {
                            var schematicFeature = schematicLayer.SchematicInMemoryDiagram.GetSchematicInMemoryFeatureByType(
                                esriSchematicElementType.esriSchematicLinkType,
                                schematicID);
                            if (schematicFeature != null)
                            {
                                newList.Add(schematicFeature);
                            }
                            else
                            {
                                schematicFeature = schematicLayer.SchematicInMemoryDiagram.GetSchematicInMemoryFeatureByType(
                                    esriSchematicElementType.esriSchematicSubLinkType,
                                    schematicID);
                                if (schematicFeature != null)
                                {
                                    newList.Add(schematicFeature);
                                }
                            }
                        }
                    }
                }
                if (newList.Count > 0)
                {
                    schematicFeatures[featuresGroup.Key] = newList;
                }
            }
            return schematicFeatures;
        }

        #endregion

        #endregion

        // ISchematicJSONParameters interface : Defines its properties and methods (mandatory to run on server)
        #region Implements ISchematicJSONParameters

        public IJSONArray JSONParametersArray
        {
            get
            {

                JSONArray aJSONArray = new JSONArray();
                try
                {
                    // build JSON object for the first parameter
                    IJSONObject oJSONObject1 = new JSONObject();

                    oJSONObject1.AddString(JSONName, JSONExpandContractFactor);
                    oJSONObject1.AddString(JSONType, JSONDouble);
                    oJSONObject1.AddDouble(JSONValue, _expandContractFactor);

                    aJSONArray.AddJSONObject(oJSONObject1);
                }
                catch
                {
                    // Catch exception at public boundary to avoid application crashes
                }

                // encode JSON array as a string
                return aJSONArray; // null propertyset 

            }
        }


        public IJSONObject JSONParametersObject
        {
            set
            {
                try
                {
                    IJSONObject oJSONObject = value;

                    double expandContractFactor;
                    if (oJSONObject != null)
                    {
                        // decode input parameters
                        if (oJSONObject.TryGetValueAsDouble(JSONExpandContractFactor, out expandContractFactor))
                        {
                            _expandContractFactor = expandContractFactor; // otherwise use current value
                        }
                    }
                }
                catch
                {
                    // Catch exception at public method boundary to avoid application crashes
                }
            }
        }
        #endregion

        #endregion

    }
}
