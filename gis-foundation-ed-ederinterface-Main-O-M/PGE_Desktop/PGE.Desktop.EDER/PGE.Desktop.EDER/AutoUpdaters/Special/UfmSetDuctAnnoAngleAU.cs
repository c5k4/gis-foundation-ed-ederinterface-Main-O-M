using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;
using Miner.Interop;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework;
using PGE.Desktop.EDER.UFM;

namespace PGE.Desktop.EDER.AutoUpdaters.Special
{
    [ComVisible(true)]
    [Guid("79FAB3FD-7BBA-4B9C-8099-370A8C3E9DBA")]
    [ProgId("PGE.Desktop.EDER.UfmSetDuctAnnoAngleAU")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class UfmSetDuctAnnoAngleAu : BaseSpecialAU
    {
        private static readonly Log4NetLogger Logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");

        private static IDictionary<int, int> rotateTracker = new Dictionary<int, int>();

        public UfmSetDuctAnnoAngleAu() : base("PGE Set Duct Anno Angle AU")
        {
        }

        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            //Enable if Feature Event type is Create or Update 
            if ((eEvent == mmEditEvent.mmEventFeatureCreate || eEvent == mmEditEvent.mmEventFeatureUpdate)
                && objectClass is IFeatureClass)
            {
                IFeatureClass featClass = objectClass as IFeatureClass;
                if (featClass.FeatureType == esriFeatureType.esriFTAnnotation)
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool CanExecute(mmAutoUpdaterMode eAUMode)
        {
            //Enable if Application type is ArcMap
            return eAUMode == mmAutoUpdaterMode.mmAUMArcMap || eAUMode == mmAutoUpdaterMode.mmAUMSplit;
        }

        protected override void InternalExecute(IObject pObject, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            mmAutoUpdaterMode currentAuMode = eAUMode;
            Type auType = Type.GetTypeFromProgID("mmGeodatabase.MMAutoUpdater");
            object auObj = Activator.CreateInstance(auType);
            IMMAutoUpdater immAutoupdater = auObj as IMMAutoUpdater;

            try
            {
                // Get the feature
                if (pObject.Class is IFeatureClass)
                {
                    IFeatureClass featClass = (IFeatureClass) pObject.Class;
                    if (featClass.FeatureType != esriFeatureType.esriFTAnnotation) return;
                }
                IFeature ductAnnoFt = (IFeature) pObject;

                // Get the wall the anno is on
                IEnumerable<IFeature> wallFeats = GetFeaturesNearPoint((ductAnnoFt.Shape.Envelope as IArea).Centroid, 0,
                    SchemaInfo.UFM.ClassModelNames.UfmWall, ((IDataset) (ductAnnoFt.Class)).Workspace,
                    esriSpatialRelEnum.esriSpatialRelWithin);
                IFeature correctWallFt = UfmHelper.MatchFieldValueForFeatureList(wallFeats, ductAnnoFt, "FACILITYID");
                if (correctWallFt == null) return;

                // get the floor the anno is 
                IEnumerable<IFeature> floorFeats = GetFeaturesNearPoint(((IArea) correctWallFt.Shape).Centroid, 200,
                    SchemaInfo.UFM.ClassModelNames.UfmFloor, ((IDataset) (ductAnnoFt.Class)).Workspace,
                    esriSpatialRelEnum.esriSpatialRelIntersects);
                IFeature correctFloorFt = UfmHelper.MatchFieldValueForFeatureList(floorFeats, ductAnnoFt, "FACILITYID");
                if (correctFloorFt == null) return;

                // Determine the angle to use and the current angle of the anno
                double wallAngleVal = GetSouthernmostWallAngle(correctWallFt, correctFloorFt); 
                immAutoupdater.AutoUpdaterMode = mmAutoUpdaterMode.mmAUMNoEvents;
                double annoAngleVal = eEvent == mmEditEvent.mmEventFeatureCreate
                    ? 0
                    : Convert.ToDouble(ductAnnoFt.Value[ductAnnoFt.Fields.FindField("ANGLE")]);

                // Get the annotations anchor point
                IElement annoElement = (ductAnnoFt as IAnnotationFeature2).Annotation;
                IPoint rotatePoint = annoElement.Geometry as IPoint;
                if (rotatePoint == null)
                {
                    rotatePoint = ((IArea)ductAnnoFt.Shape).Centroid;
                }
                
                if (annoElement is ITextElement)
                {
                    // Set the angle of the text
                    ESRI.ArcGIS.Display.IFormattedTextSymbol ts = (ESRI.ArcGIS.Display.IFormattedTextSymbol)((ITextElement)annoElement).Symbol;
                    ts.Angle = wallAngleVal;
                    ((ITextElement)annoElement).Symbol = ts;

                    // If the anno is feature linked
                    ISet ductSet = UfmHelper.GetRelatedObjects(ductAnnoFt as IRow, "UFMDUCT");
                    if (ductSet.Count > 0)
                    {
                        // Some sort of x-offset required based on the angle (closer to 45, greater it is)
                        ductSet.Reset();
                        IFeature nextFeat = ductSet.Next() as IFeature;
                        annoElement.Geometry = (nextFeat.Shape as IArea).Centroid;
                        ITransform2D transform2D = annoElement.Geometry as ITransform2D;
                        transform2D.Move(((45 - Math.Abs(45 - wallAngleVal)) * 0.0012) + 0.01, -0.1);
                        annoElement.Geometry = transform2D as IGeometry;
                    }

                    // Save all our changes
                    (ductAnnoFt as IAnnotationFeature2).Annotation = annoElement;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Error executing duct annotation angle setter. Message: " + ex.Message);
            }
            finally
            {
                immAutoupdater.AutoUpdaterMode = currentAuMode;
            }
        }

        private static IEnumerable<IFeature> GetFeaturesNearPoint(IPoint p, double rad, String modelName,
            IWorkspace editWorkspace, esriSpatialRelEnum relType)
        {
            ITopologicalOperator topoOperator = (ITopologicalOperator) p;
            IGeometry searchArea = topoOperator.Buffer(rad);

            IFeatureClass fcToSearch = ModelNameFacade.FeatureClassByModelName(editWorkspace, modelName);

            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = searchArea;
            spatialFilter.GeometryField = fcToSearch.ShapeFieldName;
            spatialFilter.SpatialRel = relType;

            IFeatureCursor featureCursor = fcToSearch.Search(spatialFilter, true);

            IFeature nextFeat;

            while ((nextFeat = featureCursor.NextFeature()) != null)
            {
                yield return nextFeat;
            }
        }

        private double GetSouthernmostWallAngle(IFeature wallFeature, IFeature floorFeature)
        {
            double wallAngleVal = 0;

            try
            {
                // Get the veritces for the wall
                IPointCollection wallVertices = wallFeature.Shape as IPointCollection;

                IPoint nearest = null;
                IPoint nextNearest = null;
                double nearestDistance = 0;
                double nextNearestDistance = 0;

                // Determine the two nearest vertices to the center of the floor
                for (int vertexIndex = 0; vertexIndex < wallVertices.PointCount; vertexIndex++)
                {                    
                    IPoint currentPoint = wallVertices.Point[vertexIndex];
                    double distance = (currentPoint as IProximityOperator).ReturnDistance((floorFeature.Shape as IArea).Centroid);
                 
                    if (nearest == null)
                    {
                        nearestDistance = distance;
                        nearest = currentPoint;
                    }
                    else if (nextNearest == null)
                    {
                        if (distance >= nearestDistance)
                        {
                            nextNearestDistance = distance;
                            nextNearest = currentPoint;
                        }
                        else
                        {
                            nextNearestDistance = nearestDistance;
                            nextNearest = nearest;
                            nearestDistance = distance;
                            nearest = currentPoint;
                        }
                    }
                    else
                    {
                        if (distance < nearestDistance)
                        {
                            nextNearest = nearest;
                            nextNearestDistance = nearestDistance;
                            nearest = currentPoint;
                            nearestDistance = distance;
                        }
                        else if (distance < nextNearestDistance && distance != nearestDistance)
                        {
                            nextNearest = currentPoint;
                            nextNearestDistance = distance;
                        }
                    }
                }

                // Get the resulting angle and make it face up
                wallAngleVal = UfmHelper.GetAngle(nearest, nextNearest);
                wallAngleVal = UfmHelper.GetShallowestAngle(wallAngleVal);

                // Handle the SOMA offset (aka: if facing SW by just a few degrees, force to face SE)
                if (wallAngleVal > -45 && wallAngleVal < -43)
                {
                    wallAngleVal = wallAngleVal + 90;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn("Failed to determine southernmost wall angle: " + ex.ToString());
            }

            return wallAngleVal;
        }
    }
}