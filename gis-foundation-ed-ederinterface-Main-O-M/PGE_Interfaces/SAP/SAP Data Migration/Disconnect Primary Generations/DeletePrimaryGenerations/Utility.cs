using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using Miner.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miner.Geodatabase.FeederManager
{
    internal static class Utility
    {
        public static double GetXYTolerance(IGeometricNetwork geometricNetwork)
        {
            double result = 0.0;
            IGeoDataset geoDataset = geometricNetwork as IGeoDataset;
            if (geoDataset != null)
            {
                ISpatialReferenceTolerance spatialReferenceTolerance = geoDataset.SpatialReference as ISpatialReferenceTolerance;
                if (spatialReferenceTolerance != null && spatialReferenceTolerance.XYToleranceValid == esriSRToleranceEnum.esriSRToleranceOK)
                {
                    result = spatialReferenceTolerance.XYTolerance;
                }
            }
            return result;
        }

        public static void SignalConnectOrDisconnectInProgress(bool suspend)
        {
            IMMSuspendAutoUpdaterActions aUController = Utility.GetAUController();
            if (aUController != null)
            {
                aUController.Suspended = suspend;
            }
            IMMFeederBooleanProperty iMMFeederBooleanProperty = aUController as IMMFeederBooleanProperty;
            if (iMMFeederBooleanProperty != null)
            {
                iMMFeederBooleanProperty.PutFMValue(1, suspend);
            }
        }

        public static bool IsConnectOrDisconnectInProgress()
        {
            bool result = false;
            IMMFeederBooleanProperty iMMFeederBooleanProperty = Utility.GetAUController() as IMMFeederBooleanProperty;
            if (iMMFeederBooleanProperty != null)
            {
                result = iMMFeederBooleanProperty.GetFMValue(1);
            }
            return result;
        }

        public static int[] GetJunctionEIDs(HashSet<IFeature> features)
        {
            HashSet<int> hashSet = new HashSet<int>();
            using (HashSet<IFeature>.Enumerator enumerator = features.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    int singleJunctionEID = Utility.GetSingleJunctionEID(enumerator.Current);
                    hashSet.Add(singleJunctionEID);
                }
            }
            return hashSet.ToArray<int>();
        }

        public static int GetSingleJunctionEID(IFeature feature)
        {
            int result = -1;
            if (feature != null)
            {
                if (feature.FeatureType == esriFeatureType.esriFTSimpleJunction)
                {
                    ISimpleJunctionFeature simpleJunctionFeature = (ISimpleJunctionFeature)feature;
                    result = simpleJunctionFeature.EID;
                }
                else if (feature.FeatureType == esriFeatureType.esriFTSimpleEdge || feature.FeatureType == esriFeatureType.esriFTComplexEdge)
                {
                    IEdgeFeature edgeFeature = (IEdgeFeature)feature;
                    result = edgeFeature.FromJunctionEID;
                }
            }
            return result;
        }

        public static IEnumerable<IFeature> GetCoincidentJunctions(IFeature feature, IGeometricNetwork network)
        {
            IEnumerable<IPoint> featurePoints = Utility.GetFeaturePoints(feature);
            HashSet<IFeature> hashSet = new HashSet<IFeature>();
            foreach (IPoint current in featurePoints)
            {
                hashSet = Utility.GetCoincidentFeatures(hashSet, network, current, esriFeatureType.esriFTSimpleJunction);
            }
            return hashSet;
        }

        public static HashSet<IFeature> GetCoincidentFeatures(IFeature feature, IGeometricNetwork network)
        {
            IEnumerable<IPoint> featurePoints = Utility.GetFeaturePoints(feature);
            return Utility.GetCoincidentFeatures(featurePoints, network);
        }

        private static HashSet<IFeature> GetCoincidentFeatures(IEnumerable<IPoint> points, IGeometricNetwork network)
        {
            HashSet<IFeature> hashSet = new HashSet<IFeature>();
            foreach (IPoint current in points)
            {
                int count = hashSet.Count;
                hashSet = Utility.GetCoincidentFeatures(hashSet, network, current, esriFeatureType.esriFTSimpleJunction);
                if (hashSet.Count - count == 1)
                {
                    hashSet = Utility.GetCoincidentFeatures(hashSet, network, current, esriFeatureType.esriFTComplexEdge);
                    hashSet = Utility.GetCoincidentFeatures(hashSet, network, current, esriFeatureType.esriFTSimpleEdge);
                }
            }
            return hashSet;
        }

        private static HashSet<IFeature> GetCoincidentFeatures(HashSet<IFeature> coincidentFeatures, IGeometricNetwork network, IPoint point, esriFeatureType type)
        {
            IEnumFeature enumFeature = network.SearchForNetworkFeature(point, type);
            enumFeature.Reset();
            for (IFeature item = enumFeature.Next(); item != null; item = enumFeature.Next())
            {
                coincidentFeatures.Add(item);
            }
            return coincidentFeatures;
        }

        private static IEnumerable<IPoint> GetFeaturePoints(IFeature feature)
        {
            List<IPoint> list = new List<IPoint>();
            if (feature.FeatureType == esriFeatureType.esriFTComplexEdge || feature.FeatureType == esriFeatureType.esriFTSimpleEdge)
            {
                IPointCollection pointCollection = feature.Shape as IPointCollection;
                for (int i = 0; i < pointCollection.PointCount; i++)
                {
                    list.Add(pointCollection.get_Point(i));
                }
            }
            else if (feature.FeatureType == esriFeatureType.esriFTSimpleJunction)
            {
                list.Add((IPoint)feature.Shape);
            }
            return list;
        }

        private static IMMSuspendAutoUpdaterActions GetAUController()
        {
            IExtensionManager extensionManager = Activator.CreateInstance(Type.GetTypeFromProgID("esriSystem.ExtensionManager")) as IExtensionManager;
            return extensionManager.FindExtension("mmFramework.MMFeederExt") as IMMSuspendAutoUpdaterActions;
        }
    }
}