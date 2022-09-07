using System.Collections.Generic;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geometry;

namespace PGE.Desktop.SchematicsMaintenance.Core
{
    /// <summary>
    /// Simplifies the snapping process for interactive map tools.
    /// </summary>
    public class SnappingSupport
    {
        private const string SnapEnvironmentUid =
            "{E07B4C52-C894-4558-B8D4-D4050018D1DA}";

        private readonly IPointSnapper pointSnapper;
         
        private readonly IDictionary<IGeometry, int> snapCacheGeometries =
            new Dictionary<IGeometry, int>();

        private readonly ISnappingEnvironment snapEnvironment;
        private readonly ISnappingFeedback snapFeedback;

        private volatile bool paused;

        private Map map;

        private readonly IActiveViewEvents_SpatialReferenceChangedEventHandler spatialReferenceChangedEventHandler;
        private readonly IActiveViewEvents_ViewRefreshedEventHandler viewRefreshedEventHandler;

        /// <summary>
        /// SnappingSupport constructor
        /// </summary>
        /// <param name="hook">Hook to the current application.</param>
        public SnappingSupport()
        {
            this.snapEnvironment =
                (ISnappingEnvironment)
                ArcMap.Application.FindExtensionByCLSID(
                    new UID {Value = SnapEnvironmentUid});

            this.pointSnapper = this.snapEnvironment.PointSnapper;

            this.snapFeedback = new CachedSnappingFeedback();

            this.snapFeedback.Initialize(ArcMap.Application, this.snapEnvironment);

            var docEvents = ArcMap.Application.Document as IDocumentEvents_Event;
            if (docEvents != null)
            {
                docEvents.ActiveViewChanged += ActiveViewChanged;
            }

            this.ActiveViewChanged();

            this.spatialReferenceChangedEventHandler =
                        new IActiveViewEvents_SpatialReferenceChangedEventHandler(this.ReprojectCacheGeometries);

            this.viewRefreshedEventHandler = 
                new IActiveViewEvents_ViewRefreshedEventHandler(this.RefreshView);
        }

        public bool Paused
        {
            get
            {
                return this.paused;
            }
            set
            {
                this.paused = value;
                if(this.snapFeedback != null)
                {
                    this.snapFeedback.Update(null, ArcMap.Document.ActiveView.ScreenDisplay.hDC);
                }
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.snapEnvironment.Enabled;
            }
        }

        private void ActiveViewChanged()
        {
            var mxDoc = ArcMap.Application.Document as IMxDocument;

            if(this.map != null)
            {
                this.map.SpatialReferenceChanged -=
                    this.spatialReferenceChangedEventHandler;
                this.map.ViewRefreshed -=
                    this.viewRefreshedEventHandler;
            }

            if (mxDoc != null)
            {
                this.map = mxDoc.FocusMap as Map;

                if (this.map != null)
                {
                    this.map.SpatialReferenceChanged += 
                        this.spatialReferenceChangedEventHandler;
                    this.map.ViewRefreshed += 
                        this.viewRefreshedEventHandler;
                }
            }

            this.ClearSnapPoints();
        }

        public void Uninitalize()
        {
            var docEvents = ArcMap.Application.Document as IDocumentEvents_Event;
            if (docEvents != null)
            {
                docEvents.ActiveViewChanged -= this.ActiveViewChanged;
            }

            this.snapFeedback.UnInitialize();

            if (this.map != null)
            {
                this.map.SpatialReferenceChanged -=
                    this.spatialReferenceChangedEventHandler;

                this.map.ViewRefreshed -= 
                    this.viewRefreshedEventHandler;
            }
        }

        private void ReprojectCacheGeometries()
        {
            foreach (IPoint point in this.snapCacheGeometries.Keys)
            {
                int cacheId = this.snapCacheGeometries[point];

                point.Project(
                    ArcMap.Document.ActiveView.ScreenDisplay.DisplayTransformation.SpatialReference);

                var bag = (IGeometryCollection)new GeometryBag();
                ((IGeometry)bag).SpatialReference = point.SpatialReference;
                bag.AddGeometry(point);

                this.pointSnapper.UpdateCachedShapes(
                    cacheId, (IGeometryBag)bag);
            }
        }

        private void RefreshView(IActiveView view,
                                 esriViewDrawPhase phase,
                                 object data,
                                 IEnvelope envelope)
        {
            if(phase == esriViewDrawPhase.esriViewGraphics)
            {
                this.snapFeedback.Refresh(view.ScreenDisplay.hDC);
            }
        }


        /// <summary>
        /// Snaps to the nearest point, based on the ArcMap Snapping Environment. 
        /// </summary>
        /// <param name="mapPoint">Input point to perform the point snapping.</param>
        /// <param name="updateDisplay">If set to true (default), map display is updated with the snapping circle. 
        /// If set to false, the snap point is returned without updating the map display.</param>
        /// <returns>The nearest snap point, or the input point if no snap point is found.</returns>
        public IPoint Snap(IPoint mapPoint, bool updateDisplay = true)
        {
            var snapPoint = mapPoint;

            if (this.snapEnvironment.Enabled && !this.paused)
            {
                ISnappingResult snapResult = this.pointSnapper.Snap(mapPoint);
                
                if(updateDisplay)
                {
                    this.snapFeedback.Update(snapResult, ArcMap.Document.ActiveView.ScreenDisplay.hDC);
                }

                if (snapResult != null)
                {
                    snapPoint = snapResult.Location;
                }
            }

            return snapPoint;
        }

        /// <summary>
        /// Removes a snap point from the snapping cache.
        /// </summary>
        /// <param name="point">The point to remove from the snapping cache.</param>
        public void RemoveSnapPoint(IPoint point)
        {            
            if (point != null && point.SpatialReference != null &&
                this.snapEnvironment.Enabled && !this.paused &&
                this.snapCacheGeometries.ContainsKey(point))
            {
                var id = this.snapCacheGeometries[point];

                this.pointSnapper.RemoveCachedShapes(id);
                this.snapCacheGeometries.Remove(point);
            }
            return;
        }

        /// <summary>
        /// Adds a snap point to the snapping cache.
        /// </summary>
        /// <param name="point">The point to add to the snapping cache.</param>
        /// <param name="name">Name of the snap point, displayed in the snap tip.</param>
        /// <param name="refreshSnapPoint">Refreshes (hides) the snap point after adding it. 
        /// Set to true if adding point during interactive snapping, and false for bulk add operations.</param>
        /// <returns></returns>
        public bool AddSnapPoint(
            IPoint point,
            string name = "",
            bool refreshSnapPoint = true)
        {
            if (point != null && point.SpatialReference != null &&
                this.snapEnvironment.Enabled && 
                !this.snapCacheGeometries.ContainsKey(point))
            {
                var geometryBag =
                    (IGeometryBag)
                    new GeometryBag
                    {
                        SpatialReference = point.SpatialReference
                    };

                ((IGeometryCollection)geometryBag).AddGeometry(point);

                int snapToken = this.pointSnapper.CacheShapes(geometryBag, name);

                this.snapCacheGeometries.Add(point, snapToken);

                if(refreshSnapPoint && !this.paused)
                {
                    // Re-snap fixes issue with snap icon showing up when it shouldn't.
                    this.Snap(point);
                }
                return true;
            }

            return false;
        }

        public bool AddSnapGeometryBag(IGeometryBag geometryBag, string name = "")
        {
            if (geometryBag != null && this.snapEnvironment.Enabled &&
                !this.snapCacheGeometries.ContainsKey(geometryBag))
            {
                int snapToken = this.pointSnapper.CacheShapes(
                    geometryBag, name);

                this.snapCacheGeometries.Add(geometryBag, snapToken);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Clears all snap points from the snapping cache.
        /// </summary>
        public void ClearSnapPoints()
        {
            foreach (int snapToken in this.snapCacheGeometries.Values)
            {
                this.pointSnapper.RemoveCachedShapes(snapToken);
            }
            this.snapCacheGeometries.Clear();
        }
    }
}

