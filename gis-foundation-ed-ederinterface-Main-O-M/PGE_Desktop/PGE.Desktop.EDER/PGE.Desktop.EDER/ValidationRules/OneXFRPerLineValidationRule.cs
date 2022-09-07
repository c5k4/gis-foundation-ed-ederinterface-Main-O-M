using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.EditorExt;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;

namespace PGE.Desktop.EDER.ValidationRules
{
    /// <summary>
    /// Determines if there are multiple transformer features connected to a line.  
    /// </summary>
    [Guid("0B538319-AB9C-4380-AF08-C5A391024FED")]
    [ProgId("PGE.Desktop.EDER.OneXFRPerLineValidationRule")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    public class OneXFRPerLineValidationRule : BaseValidationRule, IMMCurrentStatus
    {
        #region Private
        /// <summary>
        /// logger to log all the information, warning and errors.
        /// </summary>
        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        /// <summary>
        /// Local IApplicaiton object
        /// </summary>
        private IApplication _application;

        /// <summary>
        /// Network Analyst Extension for tracing.
        /// </summary>
        private INetworkAnalysisExt _networkAnalystExtension;

        /// <summary>
        /// A list model names used to determine if the QAQC rule is enabled.  QA QC rule is enabled for
        /// feature classes with model names listed in the array. 
        /// </summary>
        private static readonly string[] PrimaryConductorModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.PrimaryConductor };

        /// <summary>
        /// A list model names used to determine if the QAQC rule is enabled.  QA QC rule is enabled for
        /// feature classes with model names listed in the array. 
        /// </summary>
        private static readonly string[] TransformerModelNames = new string[] { SchemaInfo.Electric.ClassModelNames.DistributionTransformer };

        /// <summary>
        /// Transformer Feature Class
        /// </summary>
        private IFeatureClass _transformerFeatureClass = null;

        /// <summary>
        /// Determines whether [the specified origin] feature [has multiple downstream XFR] .
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <returns>
        ///   <c>true</c> if [has multiple downstream XFR] [the specified origin]; otherwise, <c>false</c>.
        /// </returns>
        private bool HasMultipleDownstreamXFR(IFeature origin)
        {

            //Set up for the trace
            _logger.Debug("Getting Geometric network.");
            IGeometricNetwork geomNetwork = (origin as INetworkFeature).GeometricNetwork; //naExt.CurrentNetwork;

            int startEID = 0;
            //check if received feature is type of ISimpleEdgeFeature
            if (origin is ISimpleEdgeFeature)
            {
                _logger.Debug("Received feature is type of ISimpleEdgeFeature.");
                startEID = (origin as ISimpleEdgeFeature).EID;
                _logger.Debug("Got startEID " + startEID + ".");
            }
            else if (origin is IComplexEdgeFeature)
            {
                _logger.Debug("Received feature is type of IComplexEdgeFeature.");
                //startEID = GetFeatureEID(origin);
                startEID = TraceFacade.GetFeatureEID(origin);
                _logger.Debug("Got startEID " + startEID + ".");
            }
            else
            {
                _logger.Debug("Received feature is niether type of ISimpleEdgeFeature nor IComplexEdgeFeature.");
                return false;
            }
            #region added by Rizuan
            System.Collections.Generic.List<IFeature> downstreamFeatures = new System.Collections.Generic.List<IFeature>();
            downstreamFeatures = TraceFacade.GetFirstDownstreamJunctions(origin);
            if (downstreamFeatures != null && downstreamFeatures.Count > 0)
            {
                if (ModelNameFacade.ContainsClassModelName(downstreamFeatures[0].Class, SchemaInfo.Electric.ClassModelNames.PGETransformer))
                {
                    if (FindDuplicateXFR(downstreamFeatures[0]))
                    {
                        _logger.Debug("junction.OID:" + downstreamFeatures[0].OID + "do have duplicate XFR.");
                        return true;
                    }

                }
                else
                {
                    _logger.Debug("No downstream Transformer feature found.");
                }

                return false;
            }
            else
            {
                _logger.Debug("No downstream feature found.");
                return false;
            }
            #endregion
            #region written by K Heater..commented by rizuan
            //int[] barrierJuncs = new int[0];
            //int[] barrierEdges = new int[0];

            //IMMTracedElements tracedJunctions;
            //IMMTracedElements tracedEdges;
            ////Call the trace
            //IMMElectricTraceSettings mmElectricTraceSettings = new MMElectricTraceSettingsClass();
            //IMMElectricTracing mmElectricTracing = new MMFeederTracerClass();
            //_logger.Debug("Executing TraceDownstream.");
            //mmElectricTracing.TraceDownstream(
            //    geomNetwork, //The geometric network (derived from the ServicePoint feature)
            //    mmElectricTraceSettings, //Setting regarding whether to respect phasing and Enabled status

            //    this, //The class that implements IMMCurrentStatus

            //    startEID, //The EID of where the trace should start
            //    esriElementType.esriETEdge,  //What kind of EID is startEID
            //    SetOfPhases.abc, //What phases should be traced
            //    mmDirectionInfo.establishBySourceSearch,
            //    0,
            //    esriElementType.esriETNone,
            //    barrierJuncs, //What junctions should act as barriers 
            //    barrierEdges, //What edges should act as barriers

            //    false,
            //    out tracedJunctions,
            //    out tracedEdges);
            //_logger.Debug("Execution completed TraceDownstream.");

            //// Find the next downstream XFR
            //IMMTracedElement junction = null;

            //IFeatureClassContainer fcContainer = (IFeatureClassContainer)geomNetwork;
            //IFeatureClass fc = null;

            //tracedJunctions.Reset();
            ////check if tracvedJunction is not empty
            //if ((junction = tracedJunctions.Next()) == null)
            //{
            //    _logger.Debug("tracedJunctions is empty.");
            //    return false;
            //}

            //fc = fcContainer.get_ClassByID(junction.FCID);
            ////check if feature class do have xfrModelNames
            //if (!ModelNameFacade.ContainsClassModelName(fc, TransformerModelNames))
            //{
            //    _logger.Debug("Feature class doesn't have xfrModelNames.");
            //    return false;
            //}

            //// Spatial query for duplicate results.
            //if (FindDuplicateXFR(fc.GetFeature(junction.OID)))
            //{
            //    _logger.Debug("junction.OID:"+junction.OID+ "do have duplicate XFR.");
            //    return true;
            //}

            //return false;
            #endregion
        }

        /// <summary>
        /// Finds a duplicate transformer if present using a spatial query.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <returns></returns>
        private bool FindDuplicateXFR(IFeature feature)
        {
            //check if XFR featureclass is not null
            if (_transformerFeatureClass == null)
            {
                //Get the XFR feature class by modelname
                _logger.Debug("Getting feature class by model name.");
                IObjectClass objClass = feature.Class;
                _transformerFeatureClass = objClass as IFeatureClass;
                //_transformerFeatureClass = ModelNameFacade.FeatureClassByModelName(((IDataset)feature.Class).Workspace, SchemaInfo.Electric.ClassModelNames.Transformer);
                _logger.Debug("Got feature class by model name.");
            }

            ISpatialFilter spatialFilter = new SpatialFilterClass();

            spatialFilter.Geometry = feature.Shape;
            spatialFilter.GeometryField = _transformerFeatureClass.ShapeFieldName;
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            _logger.Debug("Prepared spatial filter.");
            int featureCount = _transformerFeatureClass.FeatureCount(spatialFilter);
            _logger.Debug("Feature count of spatial filter is " + featureCount);
            if (featureCount > 1)
            {
                return true;
            }

            //below part of code is commented to boost the performance
            //ESRI.ArcGIS.Geodatabase.IQueryFilter queryFilter = new ESRI.ArcGIS.Geodatabase.QueryFilterClass();

            //queryFilter = (ESRI.ArcGIS.Geodatabase.IQueryFilter)spatialFilter;

            //ESRI.ArcGIS.Geodatabase.IFeatureCursor featureCursor = null;

            //try
            //{

            //    int featureCount = 0;
            //    featureCursor = xfrFc.Search(queryFilter, false);

            //    IFeature otherXFR = null;
            //    while ((otherXFR = featureCursor.NextFeature()) != null) featureCount++;

            //    if (featureCount > 1) return true;

            //}
            //catch (Exception ex)
            //{
            //    EventLogger.Error(ex);
            //}
            //finally
            //{
            //    while (Marshal.ReleaseComObject(featureCursor) > 0) { }
            //}

            return false;
        }

        /// <summary>
        /// Returns the feature associated to the given eid, for the given feature class
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="eid"></param>
        /// <returns></returns>
        private IFeature GetFeatureForEID(IFeatureClass featureClass, int eid)
        {
            int userClassID = 0;
            int userID = 0;
            int userSubID = 0;
            //get the userid for input eid
            _logger.Debug("Querying id for received EID:" + eid);
            INetElements netElements = (INetElements)_networkAnalystExtension.CurrentNetwork.Network;
            netElements.QueryIDs(eid, esriElementType.esriETEdge,
                        out userClassID, out userID, out userSubID);
            _logger.Debug("User ID retrived:" + userID);
            //get the feature for retrived userid
            return featureClass.GetFeature(userID);
        }

        /// <summary>
        /// Returns the network id (EID) of the given feature
        /// </summary>
        /// <param name="feature"></param>
        /// <returns></returns>
        private int GetFeatureEID(IFeature feature)
        {
            //cast Ifeature.Calss to feature class
            IFeatureClass fc = feature.Class as IFeatureClass;
            //Get the net elements
            _logger.Debug("Getting the Network elements from Network.");
            INetElements netElements = (INetElements)(feature as INetworkFeature).GeometricNetwork.Network;
            //get all the EID's corresponding to feature.OID
            _logger.Debug("Getting all the EID's for feature.");
            IEnumNetEID eids = netElements.GetEIDs(fc.FeatureClassID, feature.OID, esriElementType.esriETEdge);
            //check if EID is not empty
            if (eids.Count == 0)
            {
                _logger.Debug("EID's retrived are empty.");
                return 0;
            }
            _logger.Debug(string.Format("Total {0} EID's retrived , returning first EID.", eids.Count));
            //return the first EID
            return eids.Next();
        }
        #endregion Private

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryVoltageRule"/> class.
        /// 
        /// </summary>
        public OneXFRPerLineValidationRule()
            : base("PGE One XFR Per Source Line", PrimaryConductorModelNames)
        {

            Type type = Type.GetTypeFromProgID("esriFramework.AppRef");
            object obj = Activator.CreateInstance(type);
            _application = (IApplication)obj;

            _networkAnalystExtension = (INetworkAnalysisExt)_application.FindExtensionByName("Utility Network Analyst");

        }
        #endregion Constructor

        #region Override for validation rule
        /// <summary>
        /// Execute validation rule on the provided record.  Returns a list of errors.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        protected override ID8List InternalIsValid(ESRI.ArcGIS.Geodatabase.IRow row)
        {
            try
            {
                _logger.Debug("Checking for multiple downstream XFR.");
                //check if multiple DownStream XFR are available
                if (HasMultipleDownstreamXFR(row as IFeature))
                {
                    _logger.Debug("Multiple downstream XFR found.");
                    //add the error
                    AddError("Two transformers have been found at this location.");
                }
            }
            catch (Exception ex)
            {
                //log the exception
                _logger.Debug("Errror occurred while validating multiple downstream XFR.", ex);
            }
            return base.InternalIsValid(row);
        }
        #endregion Override for validation rule

        #region Public
        /// <summary>
        /// Gets the current status as weight value.  Unused.
        /// </summary>
        /// <param name="EID">The EID.</param>
        /// <param name="FCID">The FCID.</param>
        /// <param name="OID">The OID.</param>
        /// <param name="SUBID">The SUBID.</param>
        /// <param name="ElementType">Type of the element.</param>
        /// <param name="weightVal">The weight val.</param>
        public void GetCurrentStatusAsWeightValue(int EID, int FCID, int OID, int SUBID, esriElementType ElementType, ref int weightVal)
        {
            // Unused.
        }
        #endregion Public
    }
}
