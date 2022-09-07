using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Geodatabase;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Framework;
using System.Collections.Generic;
using ESRI.ArcGIS.NetworkAnalysis;
using ESRI.ArcGIS.Geometry;
using System.Timers;
using System.Diagnostics;
using Miner.Geodatabase.Edit;
using PGE.Common.Delivery.Framework.FeederManager;
using ESRI.ArcGIS.esriSystem;
using PGE.Desktop.EDER.ValidationRules.UI;
using System.Windows.Forms;

namespace PGE.Desktop.EDER.ValidationRules
{
    [ComVisible(true)] 
    [Guid("EA572289-C48F-46EC-8918-632219A61EFD")]
    [ProgId("PGE.Desktop.EDER.ValidationRules.ValidateDataAndGeometry")]
    [ComponentCategory(ComCategory.MMValidationRules)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ValidateDataAndGeometry : BaseValidationRule
    {
        #region Constructors
        public ValidateDataAndGeometry()
            : base("PGE Validate Data and Geometry", _modelNames)
        {    
        }
        //RunQAQCForm frm = new RunQAQCForm();
        #endregion Constructors

        #region Private

        private static readonly PGE.Common.Delivery.Diagnostics.Log4NetLogger _logger = new PGE.Common.Delivery.Diagnostics.Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        private static string[] _modelNames = new string[] { SchemaInfo.Electric.ClassModelNames.ValidateDataAndGeometry };
        private const string _errMsg = "Feature has null or empty geometry";
        // private const string _primaryMeterGUIDField = "primarymeterguid";
        // private const string _transformerGUIDField = "transformerguid";
        private const string _shape = "SHAPE";

        #endregion Private

        #region Override for Data and Geometry Validator
        /// <summary>
        /// Validates the object for defined rule.
        /// </summary>
        /// <param name="row">the Object to be validated.</param>
        /// <returns>Error list</returns>
        protected override ID8List InternalIsValid(IRow row)
        {
            try
            {
                IFeature feature = row as IFeature;
                IDataset pDS = null;
                double length = 0;
                IObject pObject = row as IObject;
                IObjectClass objClass = pObject.Class;
                var relClasses = objClass.RelationshipClasses[esriRelRole.esriRelRoleAny];
                relClasses.Reset();
                IRelationshipClass relClass = null;

                //RunQAQCForm.ActiveForm;            
                               
                bool _shape = feature.Shape.IsEmpty;
                IGeometry pGeometry = feature.Shape;
                IFeatureClass pFeatureClass = feature.Class as IFeatureClass;
                // IEnumerable<IObject> relatedObjects =   feature.GetRelatedObjects(null, esriRelRole.esriRelRoleAny);

               // INetElements netElements = (INetElements)(feature as INetworkFeature).GeometricNetwork.Network;

                if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPoint  && ValidationEngine.Instance.FilterType !=
                        ValidationFilterType.valFilterTypeAll)

                {
                    //  int transformerGUIDFldIdx = row.Fields.FindField(_transformerGUIDField);

                    if (feature.Shape.IsEmpty)
                    {
                        AddError(_errMsg);
                    }
                  /*  if (isDuplicateGeometry(feature, pFeatureClass))
                    {
                        AddError("Point feature has duplicate geometry.");
                    } */
                }

                if( feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon && ValidationEngine.Instance.FilterType !=
                        ValidationFilterType.valFilterTypeAll)

                {
                    //  int transformerGUIDFldIdx = row.Fields.FindField(_transformerGUIDField);

                    if (feature.Shape.IsEmpty)
                    {
                        AddError(_errMsg);
                    }
                   /* if (isDuplicateGeometry(feature, pFeatureClass))
                    {
                        AddError("Polygon feature has duplicate or Overlapping geometry.");
                    } */
                }

                if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline && ValidationEngine.Instance.FilterType !=
                        ValidationFilterType.valFilterTypeAll)// || feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)

                {
                    //INetElements network = geometricNetwork.Network as INetElements;
                    //if (network == null)
                        INetElements netElements = (INetElements)(feature as INetworkFeature).GeometricNetwork.Network;
                    //  int transformerGUIDFldIdx = row.Fields.FindField(_transformerGUIDField);
                    ICurve pCurve = pGeometry as ICurve;

                    if (pCurve.Length == 0)
                    {
                        AddError(_errMsg);
                    }
                    if (pCurve.Length <= .5)
                    {
                        AddError("Insufficient Length of conductor.");
                    }
                    if (IsMultipart(pGeometry))
                    {
                       
                        if (netElements != null)
                        {
                            AddError("Feature has multipart geometry.");
                        }
                    }

                   /* if (isDuplicateGeometry(feature, pFeatureClass))
                    {
                        if (netElements != null)
                        {
                            AddError("Line Feature has duplicate or overlapping features.");
                        }
                    } */                   

                    IPolyline pLine = feature.Shape as IPolyline;
                    if (IsLineSelfIntersecting(pLine,feature))
                    {
                        if (netElements != null)
                        {
                            AddError("Self intersecting line feature.");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return _ErrorList;
        }
        #endregion Override for Data and Geometry validator
        //private bool IsNetworkElement(IFeature feature)
        //{
        //    bool isNetworkElement = false;

        //    if (GeometricNetwork != null)
        //    {
        //        var netElements = (INetElements)GeometricNetwork.Network;

        //        int elementId = netElements.GetEID(feature.Class.ObjectClassID, feature.OID, 0, esriElementType.esriETEdge);

        //        if (elementId > 0)
        //        {
        //            isNetworkElement = true;
        //        }
        //    }

        //    return isNetworkElement;
        //}

        private bool IsMultipart(IGeometry geometry)
        {
            IGeometryCollection geometryCollection = geometry as IGeometryCollection;
            return geometryCollection != null && geometryCollection.GeometryCount > 1;

        }
        private bool isDuplicateGeometry(IFeature pFeature, IFeatureClass featClass)
        {
            ISpatialFilter featSpatialFilter = default(ISpatialFilter);
            featSpatialFilter = new SpatialFilterClass();
            //featSpatialFilter.WhereClause = "OBJECTID <> " + Convert.ToString(pFeature.OID);
            featSpatialFilter.Geometry = pFeature.ShapeCopy;

            if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
            {
                featSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin;
                if (featClass.FeatureCount(featSpatialFilter) > 1)
                    return true;

                featSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                if (featClass.FeatureCount(featSpatialFilter) > 1)
                    return true;

                featSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;                
                if (featClass.FeatureCount(featSpatialFilter) > 0)
                    return true;
            }
            else if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                featSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                if (featClass.FeatureCount(featSpatialFilter) > 1)
                    return true;

                //featSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;
                
                //if (featClass.FeatureCount(featSpatialFilter) > 0)
                //    return true;
            }

            else if (pFeature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                featSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                if (featClass.FeatureCount(featSpatialFilter) > 1)
                    return true;

                featSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelOverlaps;                
                if (featClass.FeatureCount(featSpatialFilter) > 0)
                    return true;
            }




            return false;

        }
           

      

        private bool IsLineSelfIntersecting(IPolyline line,IFeature pfeat)
        {
            bool isSelfIntersecting = false;

            IPointCollection polygonVertices = new PolylineClass();
            IPointCollection lineVertices = line as IPointCollection;

            polygonVertices.AddPointCollection(lineVertices); //convert to Polygon            

            ITopologicalOperator3 topo = line as ITopologicalOperator3;
            esriNonSimpleReasonEnum reason = esriNonSimpleReasonEnum.esriNonSimpleSelfIntersections;
            topo.IsKnownSimple_2 = false;
            
            if (!topo.IsSimpleEx[out reason])
            {
                  if ( reason == esriNonSimpleReasonEnum.esriNonSimpleSelfIntersections)
                
                {
                    isSelfIntersecting = true;
                }
            }
            
            return isSelfIntersecting;
        }

        private ISet GetRelatedObjects(IObject obj, IRelationshipClass relationshipClass)
        {


            //Get related record associated with object being passed in.
            ISet relObject = null;
            try
            {
                if (relationshipClass == null) return null;
                relObject = relationshipClass.GetObjectsRelatedToObject(obj);
            }
            catch (Exception ex)
            {
                throw ex;
                // EventLogger.Error(ex);
                //  throw new COMException("", (int)mmErrorCodes.MM_S_NOCHANGE);
            }

            return relObject;
        }
        //private double GetLength(IGeometry geometry)
        //{
        //    double retVal = 0;
        //    geometry.

        //    return Math.Round(retVal);
        //}

    }
}
