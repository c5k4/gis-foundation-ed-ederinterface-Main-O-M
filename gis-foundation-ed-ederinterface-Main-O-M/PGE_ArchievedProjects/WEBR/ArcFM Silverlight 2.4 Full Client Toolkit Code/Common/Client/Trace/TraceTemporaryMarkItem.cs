using System;
using System.Collections.Generic;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
#if SILVERLIGHT
using Miner.Server.Client.Tasks;
#elif WPF
using Miner.Mobile.Client.Tasks;
#endif

#if SILVERLIGHT
namespace Miner.Server.Client.Trace
#elif WPF
namespace Miner.Mobile.Client.Trace
#endif
{
    /// <summary>
    /// A class that represents a temporary mark
    /// item on the trace toolbar (used with gas and water traces).
    /// </summary>
    public class TraceTemporaryMarkItem : ToolbarItem
    {
        #region declarations
        private const string GasValveModelName = "Valve";
        private const string GasDistributionMainModelName = "DistributionMain";

        private object _content;
        private Map _map;
        #endregion declarations

        #region ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        public TraceTemporaryMarkItem()
        {
            ExecutingTasks = new List<GetElementIdTask>();
        }
        #endregion ctor

        #region public properties
        /// <summary>
        /// Type of the temporary mark.
        /// </summary>
        public TraceTemporaryMarksType TemporaryMarkType { get; set; }

        /// <summary>
        /// A point on the map where a mark is located.
        /// </summary>
        public MapPoint MapPoint { get; set; }

        /// <summary>
        /// Tolerance within which to search for a temporary mark.
        /// </summary>
        public double Tolerance { get; set; }

        #endregion public properties

        #region overrides
        /// <summary>
        /// Name of the temporary mark.
        /// </summary>
        public override string Name
        {
            get { return string.IsNullOrWhiteSpace(ToolTip) ?  TemporaryMarkType.ToString() : ToolTip; }
        }

        public string ToolTip { get; set; }

        /// <summary>
        /// Content of the temporary mark (e.g. an icon
        /// on the toolbar).
        /// </summary>
        public override object Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// An asynchronous call to place a temporary mark.
        /// </summary>
        /// <param name="map"><see cref="ESRI.ArcGIS.Client.Map">Map</see></param>
        /// <param name="traceMapService">Name of the trace map service.</param>
        public override void ExecuteAsync(Map map, string traceMapService)
        {
            if (map == null) throw new ArgumentNullException("map");
            if (string.IsNullOrEmpty(traceMapService) == true) throw new ArgumentNullException("traceMapService");

            _map = map;

            Layer layer = map.LayerFromUrl(traceMapService);
            if (layer == null) return;

            GetElementIdTask elementIdTask = new GetElementIdTask();
            elementIdTask.Url = traceMapService;
            elementIdTask.ProxyURL = layer.ProxyUrl();

            ExecutingTasks.Add(elementIdTask);
            elementIdTask.Failed += (s, e) => RemoveTask((GetElementIdTask)s);
            elementIdTask.ExecuteCompleted += new EventHandler<FeatureElementIDArgs>(ElementIdTask_ExecuteCompleted);
            elementIdTask.ExecuteAsync(GetElementIdParameters());
        }

        /// <summary>
        /// Allows to cancel placing a temporary mark.
        /// </summary>
        public override void CancelAsync()
        {
            foreach (GetElementIdTask task in ExecutingTasks)
            {
                task.CancelAsync();
            }
            ExecutingTasks.Clear();
        }
        #endregion overrides

        #region events
        /// <summary>
        /// Fires when a placement of a temporary mark is complete.
        /// </summary>
        public event EventHandler<FeatureElementIDArgs> RetrievedElement;
        protected virtual void OnRetrievedElement(FeatureElementIDArgs e)
        {
            EventHandler<FeatureElementIDArgs> handler = this.RetrievedElement;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ElementIdTask_ExecuteCompleted(object sender, FeatureElementIDArgs e)
        {
            if (e == null) return;

            OnRetrievedElement(e);
        }
        #endregion events

        #region private properties/methods
        private List<GetElementIdTask> ExecutingTasks { get; set; }

        private void RemoveTask(GetElementIdTask task)
        {
            ExecutingTasks.Remove(task);
            if (ExecutingTasks.Count == 0)
            {
                FeatureElementID elementId = new FeatureElementID();
                elementId.ElementId = -1;
                elementId.ObjectClassName = string.Empty;
                elementId.ObjectId = -1;
                FeatureElementIDArgs args = new FeatureElementIDArgs(elementId, null);
                OnRetrievedElement(args);
            }
        }

        private GetElementIdParameters GetElementIdParameters()
        {
            if (MapPoint == null) throw new ArgumentNullException("MapPoint");

            GetElementIdParameters getElementIdParameters = new GetElementIdParameters();
            getElementIdParameters.Point = MapPoint;
            getElementIdParameters.Tolerance = Tolerance;
            getElementIdParameters.SpatialReference = MapPoint.SpatialReference;

            if ((TemporaryMarkType == TraceTemporaryMarksType.ExcludeValve) ||
                (TemporaryMarkType == TraceTemporaryMarksType.IncludeValve))
            {
                getElementIdParameters.ModelName = GasValveModelName;
            }
            else
            {
                getElementIdParameters.ModelName = GasDistributionMainModelName;
            }

            return getElementIdParameters;
        }
        #endregion private properties/methods
    }
}
