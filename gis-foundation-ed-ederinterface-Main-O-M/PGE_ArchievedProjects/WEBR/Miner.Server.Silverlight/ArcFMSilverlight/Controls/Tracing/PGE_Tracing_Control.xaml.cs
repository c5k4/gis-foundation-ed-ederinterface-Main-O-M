using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Miner.Server.Client.Toolkit;
using Miner.Server.Client.Toolkit.Events;
using Miner.Server.Client.Events;
using ESRI.ArcGIS.Client;
using System.ComponentModel;
using System.Json;
using System.Windows.Browser;
using ESRI.ArcGIS.Client.Geometry;
using System.Threading;
using System.IO;
using Miner.Server.Client.Tasks;
using ArcFMSilverlight.Controls.Tracing;
using System.Text.RegularExpressions;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Symbols;
using System.Windows.Threading;
using Miner.Server.Client;
using System.Text;
using ESRI.ArcGIS.Client.FeatureService;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections.Generic;

namespace ArcFMSilverlight
{
    public partial class PGE_Tracing_Control : UserControl
    {
        public static List<string> TraceableLayers = null;
        public static List<string> ResultLayers = null;
        private const string CursorUri = "/Images/flag_cursor.png";
        private static bool isPGEUpstreamCurrentlyActive = false;
        private static bool isPGEDownstreamCurrentlyActive = false;
        private static bool isPGEProtectiveUpstreamCurrentlyActive = false;
        private static bool isPGEProtectiveDownstreamCurrentlyActive = false;
        private static bool isPGEUndergroundDownstreamCurrentlyActive = false;
        private static bool isPGEUndergroundUpstreamCurrentlyActive = false;
        private bool isActive = false;
        private List<TraceItem> OrigTraceResults = new List<TraceItem>();
        public TracingHelper tracingHelper = null;

        public PGE_Tracing_Control()
        {
            try
            {
                InitializeComponent();

                //DefaultStyleKey = typeof(PGE_Tracing_Control);
                EventAggregator.GetEvent<ControlActivatedEvent>().Subscribe(OnControlActivated);

                InitSettings();

                TraceResultsType = "Trace Results";
                //ImageSource.Source = new BitmapImage(new Uri("/Miner.Server.Client.Toolkit;component/Images/flag_cursor.png", 0));
                //ImageSource.Source = new BitmapImage(new Uri("/Miner.Server.Client.Toolkit;component/images/upstream_protective-device_trace.png", 0));
            }
            catch (Exception e)
            {

            }
        }

        public void ClearCurrentResults()
        {
            tracingHelper.StopCurrentAsyncTasks();

            OrigTraceResults.Clear();
            tracingHelper.CurrentFullTraceResultsByClassID.Clear();
            tracingHelper.CurrentFullTraceResultsInOrder.Clear();
            tracingHelper.CurrentProtectiveDeviceTraceResultsInOrder.Clear();

            //Clear the current graphics on screen
            GraphicsLayer pgeTraceGraphicsLayer = getTracingGraphicsLayer();
            pgeTraceGraphicsLayer.Graphics.Clear();

            CallMethodsTraceResults = false;
            this.TraceResults = new List<TraceItem>();
            TraceResultsToggle.IsChecked = false;
        }

        private GraphicsLayer getTracingGraphicsLayer()
        {
            GraphicsLayer pgeTraceGraphicsLayer = Map.Layers["PGE_Trace_Graphics"] as GraphicsLayer;
            if (pgeTraceGraphicsLayer == null)
            {
                pgeTraceGraphicsLayer = new GraphicsLayer();
                pgeTraceGraphicsLayer.ID = "PGE_Trace_Graphics";
                Map.Layers.Add(pgeTraceGraphicsLayer);
            }
            pgeTraceGraphicsLayer.Opacity = AttributesControl.TraceBufferOpacity;
            return pgeTraceGraphicsLayer;
        }

        private void InitSettings()
        {
            object flashTraceBufferColor = SettingHelper.ReadSetting("PGE_Tracing", "FlashBufferColor", "Value", "System.Int32");
            if (flashTraceBufferColor != null)
            {
                byte r = (byte)(((int)flashTraceBufferColor) / 0x10000);
                byte g = (byte)(((int)flashTraceBufferColor) / 0x100);
                byte b = (byte)(((int)flashTraceBufferColor) % 0x100);
                FlashBufferColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, r, g, b));
            }
            else
            {
                FlashBufferColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, 0, 0, 255));
            }

            object FlowAnimColor = SettingHelper.ReadSetting("PGE_Tracing", "FlashBufferColor", "Value", "System.Int32");
            if (FlowAnimColor != null)
            {
                byte r = (byte)(((int)FlowAnimColor) / 0x10000);
                byte g = (byte)(((int)FlowAnimColor) / 0x100);
                byte b = (byte)(((int)FlowAnimColor) % 0x100);
                FlowAnimationColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(0xff, r, g, b));
            }
            else
            {
                FlowAnimationColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue);
            }

            object traceAPhase = SettingHelper.ReadSetting("PGE_Tracing", "TraceAPhase", "Value", "System.bool");
            object traceBPhase = SettingHelper.ReadSetting("PGE_Tracing", "TraceBPhase", "Value", "System.bool");
            object traceCPhase = SettingHelper.ReadSetting("PGE_Tracing", "TraceCPhase", "Value", "System.bool");
            if (traceAPhase != null && traceBPhase != null && traceCPhase != null)
            {
                TraceAPhase = (bool)traceAPhase;
                TraceBPhase = (bool)traceBPhase;
                TraceCPhase = (bool)traceCPhase;
            }
            else
            {
                TraceAPhase = true;
                TraceBPhase = true;
                TraceCPhase = true;
            }
        }

        public bool IsPGEUpstreamActive
        {
            get
            {
                return (bool)GetValue(IsActivePGEUpstreamProperty);
            }
            set
            {
                SetValue(IsActivePGEUpstreamProperty, value);
                if (IsPGEUpstreamActive)
                {
                    if (IsPGEUndergroundDownstreamActive) { IsPGEUndergroundDownstreamActive = false; }
                    if (IsPGEUndergroundUpstreamActive) { IsPGEUndergroundUpstreamActive = false; }
                    if (IsPGEDownstreamActive) { IsPGEDownstreamActive = false; }
                    if (IsPGEProtectiveUpstreamActive) { IsPGEProtectiveUpstreamActive = false; }
                    if (IsPGEProtectiveDownstreamActive) { IsPGEProtectiveDownstreamActive = false; }
                }
                PGETraceUpstreamToggleButton.IsChecked = value;
            }
        }

        public bool IsPGEDownstreamActive
        {
            get
            {
                return (bool)GetValue(IsActivePGEDownstreamProperty);
            }
            set
            {
                SetValue(IsActivePGEDownstreamProperty, value);
                if (IsPGEDownstreamActive)
                {
                    if (IsPGEUndergroundDownstreamActive) { IsPGEUndergroundDownstreamActive = false; }
                    if (IsPGEUndergroundUpstreamActive) { IsPGEUndergroundUpstreamActive = false; }
                    if (IsPGEUpstreamActive) { IsPGEUpstreamActive = false; }
                    if (IsPGEProtectiveUpstreamActive) { IsPGEProtectiveUpstreamActive = false; }
                    if (IsPGEProtectiveDownstreamActive) { IsPGEProtectiveDownstreamActive = false; }
                }
                PGETraceDownstreamToggleButton.IsChecked = value;
            }
        }

        public bool IsPGEProtectiveDownstreamActive
        {
            get
            {
                return (bool)GetValue(IsActivePGEProtectiveDownstreamProperty);
            }
            set
            {
                SetValue(IsActivePGEProtectiveDownstreamProperty, value);
                if (IsPGEProtectiveDownstreamActive)
                {
                    if (IsPGEUndergroundDownstreamActive) { IsPGEUndergroundDownstreamActive = false; }
                    if (IsPGEUndergroundUpstreamActive) { IsPGEUndergroundUpstreamActive = false; }
                    if (IsPGEUpstreamActive) { IsPGEUpstreamActive = false; }
                    if (IsPGEDownstreamActive) { IsPGEDownstreamActive = false; }
                    if (IsPGEProtectiveUpstreamActive) { IsPGEProtectiveUpstreamActive = false; }
                }
                PGETraceProtectiveDownstreamToggleButton.IsChecked = value;
            }
        }

        public bool IsPGEProtectiveUpstreamActive
        {
            get
            {
                return (bool)GetValue(IsActivePGEProtectiveUpstreamProperty);
            }
            set
            {
                SetValue(IsActivePGEProtectiveUpstreamProperty, value);
                if (IsPGEProtectiveUpstreamActive)
                {
                    if (IsPGEUndergroundDownstreamActive) { IsPGEUndergroundDownstreamActive = false; }
                    if (IsPGEUndergroundUpstreamActive) { IsPGEUndergroundUpstreamActive = false; }
                    if (IsPGEUpstreamActive) { IsPGEUpstreamActive = false; }
                    if (IsPGEDownstreamActive) { IsPGEDownstreamActive = false; }
                    if (IsPGEProtectiveDownstreamActive) { IsPGEProtectiveDownstreamActive = false; }
                }
                PGETraceProtectiveUpstreamToggleButton.IsChecked = value;
            }
        }

        public bool IsPGEUndergroundDownstreamActive
        {
            get
            {
                return (bool)GetValue(IsActivePGEUndergroundDownstreamProperty);
            }
            set
            {
                SetValue(IsActivePGEUndergroundDownstreamProperty, value);
                if (IsPGEUndergroundDownstreamActive)
                {
                    if (IsPGEUndergroundUpstreamActive) { IsPGEUndergroundUpstreamActive = false; }
                    if (IsPGEUpstreamActive) { IsPGEUpstreamActive = false; }
                    if (IsPGEDownstreamActive) { IsPGEDownstreamActive = false; }
                    if (IsPGEProtectiveUpstreamActive) { IsPGEProtectiveUpstreamActive = false; }
                    if (IsPGEProtectiveDownstreamActive) { IsPGEProtectiveDownstreamActive = false; }
                }
                PGETraceUndergroundDownstreamToggleButton.IsChecked = value;
            }
        }

        public bool IsPGEUndergroundUpstreamActive
        {
            get
            {
                return (bool)GetValue(IsActivePGEUndergroundUpstreamProperty);
            }
            set
            {
                SetValue(IsActivePGEUndergroundUpstreamProperty, value);
                if (IsPGEUndergroundUpstreamActive)
                {
                    if (IsPGEUndergroundDownstreamActive) { IsPGEUndergroundDownstreamActive = false; }
                    if (IsPGEUpstreamActive) { IsPGEUpstreamActive = false; }
                    if (IsPGEDownstreamActive) { IsPGEDownstreamActive = false; }
                    if (IsPGEProtectiveUpstreamActive) { IsPGEProtectiveUpstreamActive = false; }
                    if (IsPGEProtectiveDownstreamActive) { IsPGEProtectiveDownstreamActive = false; }
                }
                PGETraceUndergroundUpstreamToggleButton.IsChecked = value;
            }
        }


        public void OnControlActivated(DependencyObject control)
        {
            if (control != this)
            {
                IsPGEUpstreamActive = false;
                IsPGEDownstreamActive = false;
                IsPGEProtectiveUpstreamActive = false;
                IsPGEProtectiveDownstreamActive = false;
                IsPGEUndergroundUpstreamActive = false;
                IsPGEUndergroundDownstreamActive = false;
            }
        }


        #region Select Trace Result feature for Attributes window

        public void SelectFeature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBox listBox = sender as ListBox;
                TraceItem listBoxSelectedItem = listBox.SelectedItem as TraceItem;
                int selectedIndex = OrigTraceResults.IndexOf(listBoxSelectedItem);
                TraceItem selectedItem = tracingHelper.CurrentFullTraceResultsInOrder[selectedIndex];
                if (tracingHelper.CurrentResultsAreProtective) { selectedItem = tracingHelper.CurrentProtectiveDeviceTraceResultsInOrder[selectedIndex]; }
                if (!String.IsNullOrEmpty(selectedItem.ToString()))
                {
                    string[] item = Regex.Split(selectedItem.ToString(), ":");
                    if (item.Length == 2)
                    {
                        foreach (Layer layer in Map.Layers)
                        {
                            if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                            {
                                ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;
                                string selectURL = dynamicMapLayer.Url;

                                //Adjust for schematics classID
                                int classID = Int32.Parse(item[0]);

                                List<int> currentSelectLayerIDs = ConfigUtility.GetLayerIDFromClassID(selectURL, classID).ToList();
                                if (currentSelectLayerIDs.Count < 1) { continue; }

                                foreach (int layerID in currentSelectLayerIDs)
                                {
                                    //Now query the first layerID returned to zoom to the feature
                                    QueryTask selectTraceResultsTask = new QueryTask(selectURL + "/" + layerID);
                                    selectTraceResultsTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(selectTraceResultsTask_ExecuteCompleted);
                                    selectTraceResultsTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(selectTraceResultsTask_Failed);
                                    Query query = new Query();
                                    query.ReturnGeometry = true;
                                    query.OutFields.Add("*");
                                    if (tracingHelper.CurrentResultsAreSchematics) { query.Where = TracingHelper.SchemFieldIdentifierName + " = '" + item[1] + "'"; }
                                    else { query.Where = TracingHelper.FieldIdentifierName + " = '" + item[1] + "'"; }
                                    tracingHelper.CurrentlyExecutingTasks.Add(selectTraceResultsTask);
                                    selectTraceResultsTask.ExecuteAsync(query);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Something failed add the feature to selection
            }
        }

        void selectTraceResultsTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Unable to select feature: " + e.Error.Message);
        }

        private Object selectFeatureLock = new Object();
        void selectTraceResultsTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count <= 0) { return; }

                QueryTask queryTask = sender as QueryTask;
                string url = queryTask.Url;
                string mapServer = url.Substring(0, url.LastIndexOf("/"));
                int layerID = -1;
                string layerIDString = url.Substring(url.LastIndexOf("/") + 1);
                Int32.TryParse(layerIDString, out layerID);

                if (layerID == -1) { return; }

                lock (selectFeatureLock)
                {
                    ObservableCollection<IResultSet> observables = new ObservableCollection<IResultSet>();
                    foreach (IResultSet set in AttributesControl.Results)
                    {
                        observables.Add(set);
                    }

                    bool foundCurrentResultSet = false;
                    foreach (IResultSet set in observables)
                    {
                        if (set.Service == mapServer && set.ID == layerID)
                        {
                            //Something breaks if we try to add the feature to the current result set, so we will create a new
                            //one in this scenario as well.
                            foreach (Graphic graphic in set.Features)
                            {
                                e.FeatureSet.Features.Add(graphic);
                            }

                            //We need to create a result set
                            ResultSet results = new ResultSet(e.FeatureSet);
                            results.Service = set.Service;
                            results.ID = set.ID;
                            results.Name = set.Name;

                            observables.Remove(set);
                            observables.Add(results);

                            foundCurrentResultSet = true;
                            break;
                        }
                    }

                    if (!foundCurrentResultSet)
                    {
                        string layerAlias = null;
                        foreach (Layer layer in Map.Layers)
                        {
                            if (layer is ArcGISDynamicMapServiceLayer)
                            {
                                ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                                if (mapServer == dynamicLayer.Url)
                                {
                                    foreach (LayerInfo layerInfo in dynamicLayer.Layers)
                                    {
                                        if (layerInfo.ID == layerID)
                                        {
                                            layerAlias = layerInfo.Name;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        //We need to create a result set
                        ResultSet results = new ResultSet(e.FeatureSet);
                        results.Service = mapServer;
                        results.ID = layerID;
                        results.Name = layerAlias;
                        observables.Add(results);
                    }
                    bool autoZoom = AttributesControl.AutoZoomToResults;
                    AttributesControl.AutoZoomToResults = false;
                    AttributesControl.Results = observables;
                    AttributesControl.AutoZoomToResults = autoZoom;
                }

            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region Zoom to trace results selected item

        private List<int> currentZoomToLayerIDs = null;
        private string currentZoomToUrl = "";
        private string currentZoomToSearchValue = "";
        public void ZoomToFeature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBox listBox = sender as ListBox;
                TraceItem listBoxSelectedItem = listBox.SelectedItem as TraceItem;
                int selectedIndex = OrigTraceResults.IndexOf(listBoxSelectedItem);
                if (selectedIndex < 0) { return; }
                TraceItem selectedItem = tracingHelper.CurrentFullTraceResultsInOrder[selectedIndex];
                if (tracingHelper.CurrentResultsAreProtective) { selectedItem = tracingHelper.CurrentProtectiveDeviceTraceResultsInOrder[selectedIndex]; }
                if (!String.IsNullOrEmpty(selectedItem.ToString()))
                {
                    string[] item = Regex.Split(selectedItem.ToString(), ":");
                    if (item.Length == 2)
                    {
                        foreach (Layer layer in Map.Layers)
                        {
                            if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                            {
                                ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;
                                currentZoomToUrl = dynamicMapLayer.Url;

                                //Adjust for schematics classID
                                int classID = Int32.Parse(item[0]);

                                currentZoomToLayerIDs = ConfigUtility.GetLayerIDFromClassID(currentZoomToUrl, classID).ToList();
                                if (currentZoomToLayerIDs.Count < 1) { continue; }

                                //Now query the first layerID returned to zoom to the feature
                                QueryTask zoomTraceResultsTask = new QueryTask(currentZoomToUrl + "/" + currentZoomToLayerIDs[0]);
                                currentZoomToLayerIDs.RemoveAt(0);
                                zoomTraceResultsTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(zoomTraceResultsTask_ExecuteCompleted);
                                zoomTraceResultsTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(zoomTraceResultsTask_Failed);
                                Query query = new Query();
                                query.ReturnGeometry = true;
                                query.OutFields.AddRange(new string[] { });
                                currentZoomToSearchValue = item[1];
                                if (tracingHelper.CurrentResultsAreSchematics) { query.Where = TracingHelper.SchemFieldIdentifierName + " = '" + item[1] + "'"; }
                                else { query.Where = TracingHelper.FieldIdentifierName + " = '" + item[1] + "'"; }

                                tracingHelper.CurrentlyExecutingTasks.Add(zoomTraceResultsTask);
                                zoomTraceResultsTask.ExecuteAsync(query);

                                //Only need to query the one layer
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Something failed zooming to the feature
            }
        }

        void zoomTraceResultsTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed zooming to feature: " + e.Error.Message);
        }

        void zoomTraceResultsTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    Geometry geometry = e.FeatureSet.Features[0].Geometry;
                    if (geometry != null)
                    {
                        Geometry centerpoint = geometry;
                        if (geometry is ESRI.ArcGIS.Client.Geometry.Polyline)
                        {
                            PointCollection path = (geometry as ESRI.ArcGIS.Client.Geometry.Polyline).Paths[0];
                            int closestCenterSegment = GetClosestCenterSegment(path);
                            if (path.Count <= 2)
                            {
                                centerpoint = new MapPoint((path[0].X + path[1].X) / 2.0, (path[0].Y + path[1].Y) / 2.0);
                            }
                            else
                            {
                                centerpoint = new MapPoint(path[closestCenterSegment].X, path[closestCenterSegment].Y);
                            }
                        }
                        else
                        {
                            centerpoint = geometry.Extent.GetCenter();
                        }
                        Envelope _newExtent = CalculateExtent(centerpoint, geometry);
                        Map.ZoomTo(_newExtent);

                        //After zooming we want to flash the feature
                        FlashTraceResultsTask_ExecuteCompleted(sender, e);
                    }
                }
                else if (currentZoomToLayerIDs.Count > 0)
                {
                    //If no features were returned it could be due to definition queries.  Let's make sure there are no more
                    //layers to check
                    //Query the first layerID returned to zoom to the feature
                    QueryTask zoomTraceResultsTask = new QueryTask(currentZoomToUrl + "/" + currentZoomToLayerIDs[0]);
                    currentZoomToLayerIDs.RemoveAt(0);
                    zoomTraceResultsTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(zoomTraceResultsTask_ExecuteCompleted);
                    zoomTraceResultsTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(zoomTraceResultsTask_Failed);
                    Query query = new Query();
                    query.ReturnGeometry = true;
                    query.OutFields.AddRange(new string[] { });
                    query.Where = TracingHelper.FieldIdentifierName + " = '" + currentZoomToSearchValue + "'";
                    tracingHelper.CurrentlyExecutingTasks.Add(zoomTraceResultsTask);
                    zoomTraceResultsTask.ExecuteAsync(query);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Envelope CalculateExtent(ESRI.ArcGIS.Client.Geometry.Geometry centerpoint, ESRI.ArcGIS.Client.Geometry.Geometry geometry)
        {
            if ((geometry == null) || (centerpoint == null))
            {
                return null;
            }
            Envelope extent = geometry.Extent;
            Envelope envelope2 = centerpoint.Extent;
            if (extent == null)
            {
                return null;
            }
            if (geometry is ESRI.ArcGIS.Client.Geometry.Polyline)
            {
                envelope2 = geometry.Extent;

                envelope2.XMax += 200;
                envelope2.YMax += 200;
                envelope2.XMin -= 200;
                envelope2.YMin -= 200;
            }
            else
            {
                envelope2.XMax += extent.Width / 2.0;
                envelope2.YMax += extent.Height / 2.0;
                envelope2.XMin -= extent.Width / 2.0;
                envelope2.YMin -= extent.Height / 2.0;
                envelope2.XMax += 200;
                envelope2.YMax += 200;
                envelope2.XMin -= 200;
                envelope2.YMin -= 200;
            }

            return envelope2;
        }

        private int GetClosestCenterSegment(PointCollection path)
        {
            List<double> segmentLengths = this.GetSegmentLengths(path);
            double num = Enumerable.Sum(segmentLengths) / 2.0;
            double num2 = 0.0;
            for (int i = 0; i < (segmentLengths.Count - 1); i++)
            {
                num2 += segmentLengths[i];
                if (num2 > num)
                {
                    return (i + 1);
                }
            }
            return 1;
        }

        private List<double> GetSegmentLengths(PointCollection path)
        {
            List<double> list = new List<double>();
            if (path != null)
            {
                if (path.Count == 0)
                {
                    return list;
                }
                for (int i = 1; i < path.Count; i++)
                {
                    list.Add(Distance(path[i - 1], path[i]));
                }
            }
            return list;
        }

        private double Distance(MapPoint point, MapPoint point2)
        {
            double num = point2.X - point.X;
            double num2 = point2.Y - point.Y;
            return Math.Sqrt((num * num) + (num2 * num2));
        }


        #endregion

        #region Flash trace results selected item

        private List<int> currentFlashLayerIDs = null;
        private string currentFlashUrl = "";
        private string currentFlashSearchValue = "";
        public void FlashFeature_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Initialize our graphics layer for this task
                GraphicsLayer pgeFlashGraphicsLayer = Map.Layers["PGE_Flash_Graphics"] as GraphicsLayer;
                if (pgeFlashGraphicsLayer == null)
                {
                    pgeFlashGraphicsLayer = new GraphicsLayer();
                    pgeFlashGraphicsLayer.ID = "PGE_Flash_Graphics";
                    Map.Layers.Add(pgeFlashGraphicsLayer);
                }

                ListBox listBox = sender as ListBox;
                TraceItem listBoxSelectedItem = listBox.SelectedItem as TraceItem;
                int selectedIndex = OrigTraceResults.IndexOf(listBoxSelectedItem);
                TraceItem selectedItem = tracingHelper.CurrentFullTraceResultsInOrder[selectedIndex];
                if (tracingHelper.CurrentResultsAreProtective) { selectedItem = tracingHelper.CurrentProtectiveDeviceTraceResultsInOrder[selectedIndex]; }
                if (!String.IsNullOrEmpty(selectedItem.ToString()))
                {
                    string[] item = Regex.Split(selectedItem.ToString(), ":");
                    if (item.Length == 2)
                    {
                        foreach (Layer layer in Map.Layers)
                        {
                            if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                            {
                                ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;
                                currentFlashUrl = dynamicMapLayer.Url;

                                //Adjust for schematics classID
                                int classID = Int32.Parse(item[0]);

                                currentFlashLayerIDs = ConfigUtility.GetLayerIDFromClassID(currentFlashUrl, classID).ToList();
                                if (currentFlashLayerIDs.Count < 1) { continue; }

                                //Now query the first layerID returned to zoom to the feature
                                QueryTask FlashTraceResultsTask = new QueryTask(currentFlashUrl + "/" + currentFlashLayerIDs[0]);
                                currentFlashLayerIDs.RemoveAt(0);
                                FlashTraceResultsTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(FlashTraceResultsTask_ExecuteCompleted);
                                FlashTraceResultsTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(FlashTraceResultsTask_Failed);
                                Query query = new Query();
                                query.ReturnGeometry = true;
                                query.OutFields.AddRange(new string[] { });
                                currentFlashSearchValue = item[1];
                                if (tracingHelper.CurrentResultsAreSchematics) { query.Where = TracingHelper.SchemFieldIdentifierName + " = '" + item[1] + "'"; }
                                else { query.Where = TracingHelper.FieldIdentifierName + " = '" + item[1] + "'"; }
                                tracingHelper.CurrentlyExecutingTasks.Add(FlashTraceResultsTask);
                                FlashTraceResultsTask.ExecuteAsync(query);

                                //Only need to query the one layer
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Something failed trying to flash the feature.
            }
        }

        void FlashTraceResultsTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed to flash feature: " + e.Error.Message);
        }

        void FlashTraceResultsTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count > 0)
                {
                    //First we need to buffer our point click by using the geometry service.
                    BufferParameters bufferParams = new BufferParameters();
                    bufferParams.Distances.Add(AttributesControl.TraceBufferSize);
                    bufferParams.Unit = new LinearUnit?(AttributesControl.TraceBufferUnit);
                    bufferParams.Features.AddRange(e.FeatureSet.Features);

                    //Make the geometry service call
                    GeometryService flashGeomService = new GeometryService(ConfigUtility.GeometryServiceURL);
                    flashGeomService.BufferCompleted += new EventHandler<GraphicsEventArgs>(flashGeomService_BufferCompleted);
                    flashGeomService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(flashGeomService_Failed);
                    tracingHelper.CurrentlyExecutingTasks.Add(flashGeomService);
                    flashGeomService.BufferAsync(bufferParams);
                }
                else if (currentFlashLayerIDs.Count > 0)
                {
                    //If no features were returned it could be due to definition queries.  Let's make sure there are no more
                    //layers to check
                    //Query the first layerID returned to zoom to the feature
                    QueryTask FlashTraceResultsTask = new QueryTask(currentFlashUrl + "/" + currentFlashLayerIDs[0]);
                    currentFlashLayerIDs.RemoveAt(0);
                    FlashTraceResultsTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(FlashTraceResultsTask_ExecuteCompleted);
                    FlashTraceResultsTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(FlashTraceResultsTask_Failed);
                    Query query = new Query();
                    query.ReturnGeometry = true;
                    query.OutFields.AddRange(new string[] { });
                    query.Where = TracingHelper.FieldIdentifierName + " = '" + currentFlashSearchValue + "'";
                    tracingHelper.CurrentlyExecutingTasks.Add(FlashTraceResultsTask);
                    FlashTraceResultsTask.ExecuteAsync(query);
                }
            }
            catch (Exception ex)
            {

            }
        }

        void flashGeomService_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            ConfigUtility.UpdateStatusBarText("Failed to buffer feature for flash: " + e.Error.Message);
        }

        private DispatcherTimer flashTimer;
        private int flashCount = 0;
        private IList<Graphic> currentFlashResults = null;
        void flashGeomService_BufferCompleted(object sender, GraphicsEventArgs e)
        {
            //Initialize our graphics layer for this task
            GraphicsLayer pgeFlashGraphicsLayer = Map.Layers["PGE_Flash_Graphics"] as GraphicsLayer;
            if (pgeFlashGraphicsLayer == null)
            {
                pgeFlashGraphicsLayer = new GraphicsLayer();
                pgeFlashGraphicsLayer.ID = "PGE_Flash_Graphics";
                Map.Layers.Add(pgeFlashGraphicsLayer);
            }

            pgeFlashGraphicsLayer.Graphics.Clear();

            SimpleFillSymbol symbol2 = new SimpleFillSymbol();
            symbol2.BorderThickness = 1.0;
            symbol2.BorderBrush = FlashBufferColor;
            symbol2.Fill = FlashBufferColor;
            pgeFlashGraphicsLayer.Opacity = AttributesControl.TraceBufferOpacity;

            for (int i = 0; i < e.Results.Count; i++)
            {
                e.Results[i].Symbol = symbol2;
            }
            currentFlashResults = e.Results;

            if (flashTimer != null)
            {
                flashCount = 0;
                flashTimer.Stop();
                flashTimer.Start();
            }
            else
            {
                flashTimer = new DispatcherTimer();
                flashTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                flashTimer.Tick += new EventHandler(flashTimer_Tick);
                flashTimer.Start();
            }

        }

        void flashTimer_Tick(object sender, EventArgs e)
        {
            //Initialize our graphics layer for this task
            GraphicsLayer pgeFlashGraphicsLayer = Map.Layers["PGE_Flash_Graphics"] as GraphicsLayer;
            if (pgeFlashGraphicsLayer == null)
            {
                pgeFlashGraphicsLayer = new GraphicsLayer();
                pgeFlashGraphicsLayer.ID = "PGE_Flash_Graphics";
                Map.Layers.Add(pgeFlashGraphicsLayer);
            }

            if (pgeFlashGraphicsLayer.Graphics.Count < 1 && flashCount < 4)
            {
                flashCount++;
                pgeFlashGraphicsLayer.Graphics.AddRange(currentFlashResults);
            }
            else
            {
                pgeFlashGraphicsLayer.Graphics.Clear();
            }
        }

        #endregion

        #region Get Loading Information

        public void LoadingInformation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListBox listBox = sender as ListBox;
                TraceItem listBoxSelectedItem = listBox.SelectedItem as TraceItem;
                int selectedIndex = OrigTraceResults.IndexOf(listBoxSelectedItem);
                TraceItem selectedItem = tracingHelper.CurrentFullTraceResultsInOrder[selectedIndex];
                if (tracingHelper.CurrentResultsAreProtective) { selectedItem = tracingHelper.CurrentProtectiveDeviceTraceResultsInOrder[selectedIndex]; }
                if (!String.IsNullOrEmpty(selectedItem.ToString()))
                {
                    string[] item = Regex.Split(selectedItem.ToString(), ":");
                    if (item.Length == 2)
                    {
                        int classID = Int32.Parse(item[0]);
                        string GUID = item[1];

                        LoadingInfo.GetLoadingInformation(classID, GUID, tracingHelper.CurrentResultsFeederID, Map);
                    }
                }
            }
            catch (Exception ex)
            {
                //Something failed add the feature to selection
            }
        }

        #endregion

        #region Draw trace results graphics to screen within current extent

        public static List<TaskBase> CurrentlyExecutingDrawTasks = new List<TaskBase>();
        private bool LayerConfiguredForTracing()
        {
            int elecIndex = GetTracingUrl(TracingHelper.PGEElectricTracingURLs);
            int schemIndex = GetTracingUrl(TracingHelper.PGESchemTracingURLs);
            int subIndex = GetTracingUrl(TracingHelper.PGESubstationTracingURLs);

            if (elecIndex < 0 && schemIndex < 0 && subIndex < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Returnes the index of the list where the URL exists in the current map layers. 
        /// </summary>
        /// <param name="possibleURLs">List of URLs to search</param>
        /// <returns></returns>
        private int GetTracingUrl(List<string> possibleURLs)
        {
            foreach (Layer layer in Map.Layers)
            {
                if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                {
                    ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                    for (int i = 0; i < possibleURLs.Count; i++)
                    {
                        string url = possibleURLs[i];
                        if (url.ToUpper() == dynamicLayer.Url.ToUpper())
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        private Object GraphicsDrawingLock = new Object();
        private void DrawTraceGraphics()
        {
            try
            {
                lock (ToProcessBeforeArrowsLock)
                {
                    CancelCurrentDrawTasks();

                    ToProcessBeforeArrows = 0;

                    //Clear the current GUID to shape mapping built for drawing the directional symbols
                    lock (GuidToPolylineShapeLock)
                    {
                        GuidToPolylineShapes.Clear();
                    }

                    string phases = "";
                    if (!TraceAPhase && !TraceBPhase && !TraceCPhase)
                    {
                        phases += " (No Phase selected)";
                    }
                    else if (!LayerConfiguredForTracing())
                    {
                        phases += " (No visible layers configured for tracing)";
                    }
                    else
                    {
                        phases = " (";
                        if (TraceAPhase) { phases += "A,"; }
                        if (TraceBPhase) { phases += "B,"; }
                        if (TraceCPhase) { phases += "C,"; }
                        if (phases.Contains(",")) { phases = phases.Substring(0, phases.LastIndexOf(',')); }
                        phases += ")";
                    }

                    //Assign the description for this trace in our results text
                    if (!tracingHelper.CurrentResultsAreUpstream && !tracingHelper.CurrentResultsAreProtective) { TraceResultsType = "Downstream Trace Results" + phases; }
                    else if (tracingHelper.CurrentResultsAreUpstream && !tracingHelper.CurrentResultsAreProtective) { TraceResultsType = "Upstream Trace Results" + phases; }
                    else if (!tracingHelper.CurrentResultsAreUpstream && tracingHelper.CurrentResultsAreProtective) { TraceResultsType = "Protective Downstream Trace Results" + phases; }
                    else if (tracingHelper.CurrentResultsAreUpstream && tracingHelper.CurrentResultsAreProtective) { TraceResultsType = "Protective Upstream Trace Results" + phases; }

                    AttachMapExtentChangedHandler();

                    GraphicsLayer pgeTraceGraphicsLayer = getTracingGraphicsLayer();

                    //Reset the trace graphics as we are drawing a new set
                    pgeTraceGraphicsLayer.Graphics.Clear();

                    GraphicsLayer pgeTracingGraphics = getTracingGraphicsLayer();
                    pgeTracingGraphics.Visible = AttributesControl.TraceBufferSwitch;

                    //Cache the current tracing settings
                    CacheTracingSettings();

                    //If the trace buffers are turned off, no need to draw anything. Also if the trace buffer isn't cached we
                    //will default to draw it
                    if (!AttributesControl.TraceBufferSwitch) { return; }

                    pgeTracingGraphics.Visible = true;

                    Dictionary<int, List<TraceItem>> classIDsToGlobalIDsMapping = tracingHelper.CurrentFullTraceResultsByClassID;
                    Dictionary<int, List<TraceItem>> classIDsToGlobalIDsMap = new Dictionary<int, List<TraceItem>>();

                    classIDsToGlobalIDsMap = classIDsToGlobalIDsMapping;
                    Envelope envelopeToQuery = Map.Extent.Clone();
                    //envelopeToQuery.XMax += 1000;
                    //envelopeToQuery.XMin -= 1000;
                    //envelopeToQuery.YMax += 1000;
                    //envelopeToQuery.YMin -= 1000;
                    foreach (Layer layer in Map.Layers)
                    {
                        if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                        {
                            foreach (KeyValuePair<int, List<TraceItem>> kvp in classIDsToGlobalIDsMap)
                            {
                                int classID = kvp.Key;

                                //Verify that this classID is a protective device classID if we are executing a prot device trace
                                if (tracingHelper.CurrentResultsAreProtective)
                                {
                                    bool isProtDevice = false;
                                    foreach (KeyValuePair<string, int> protDeviceKVP in TracingHelper.PGEProtectiveDevicesByClassID)
                                    {
                                        int protClassID = protDeviceKVP.Value;
                                        if (protClassID == classID) { isProtDevice = true; }
                                    }
                                    if (!isProtDevice) { continue; }
                                }

                                ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;
                                int[] visibleLayers = dynamicMapLayer.VisibleLayers;
                                string currentLayerURL = dynamicMapLayer.Url;
                                List<int> LayerIDsToQuery = ConfigUtility.GetLayerIDFromClassID(currentLayerURL, classID).ToList();
                                if (LayerIDsToQuery.Count < 1) { continue; }

                                for (int i = 0; i < LayerIDsToQuery.Count; i++)
                                {
                                    Query query = new Query();
                                    query.ReturnGeometry = true;
                                    if (tracingHelper.CurrentResultsAreSchematics) { query.OutFields.AddRange(new string[] { TracingHelper.SchemFieldIdentifierName }); }
                                    else { query.OutFields.AddRange(new string[] { TracingHelper.FieldIdentifierName }); }

                                    List<string> whereInStringList = new List<string>();
                                    StringBuilder whereInStringBuilder = new StringBuilder();
                                    for (int item = 0; item < kvp.Value.Count; item++)
                                    {
                                        if (item == kvp.Value.Count - 1 || (item % 999) == 0) { whereInStringBuilder.Append("'" + kvp.Value[item].GlobalID + "'"); }
                                        else { whereInStringBuilder.Append("'" + kvp.Value[item].GlobalID + "'" + ","); }

                                        if ((item % 999) == 0)
                                        {
                                            whereInStringList.Add(whereInStringBuilder.ToString());
                                            whereInStringBuilder = new StringBuilder();
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                                    {
                                        whereInStringList.Add(whereInStringBuilder.ToString());
                                    }

                                    foreach (string whereInString in whereInStringList)
                                    {
                                        lock (mapExtentChangedLock)
                                        {
                                            if (extentChanged)
                                            {
                                                //If the extent has changed return and let the next execution of the method redraw the appropriate graphics
                                                CancelCurrentDrawTasks();
                                                return;
                                            }
                                        }

                                        if (tracingHelper.CurrentResultsAreSchematics) { query.Where = TracingHelper.SchemFieldIdentifierName + " in (" + whereInString + ")"; }
                                        else { query.Where = TracingHelper.FieldIdentifierName + " in (" + whereInString + ")"; }
                                        query.SpatialRelationship = SpatialRelationship.esriSpatialRelIntersects;
                                        query.Geometry = envelopeToQuery;
                                        //Now query the first layerID returned to zoom to the feature
                                        QueryTask SelectExtentTraceResultsTask = new QueryTask(currentLayerURL + "/" + LayerIDsToQuery[i]);
                                        SelectExtentTraceResultsTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(SelectExtentTraceResultsTask_ExecuteCompleted);
                                        SelectExtentTraceResultsTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(SelectExtentTraceResultsTask_Failed);
                                        tracingHelper.CurrentlyExecutingTasks.Add(SelectExtentTraceResultsTask);
                                        CurrentlyExecutingDrawTasks.Add(SelectExtentTraceResultsTask);
                                        ToProcessBeforeArrows++;
                                        SelectExtentTraceResultsTask.ExecuteAsync(query);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Something failed in drawing the graphics.
            }
        }

        void SelectExtentTraceResultsTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            lock (ToProcessBeforeArrowsLock)
            {
                ToProcessBeforeArrows--;
                if (ToProcessBeforeArrows <= 0)
                {
                    DrawDirectionalArrows();
                }
            }
        }

        private Object GuidToMapPointShapeLock = new Object();
        private Object GuidToPolylineShapeLock = new Object();
        private Object ToProcessBeforeArrowsLock = new Object();
        private int ToProcessBeforeArrows = 0;
        private Dictionary<string, List<Polyline>> GuidToPolylineShapes = new Dictionary<string, List<Polyline>>();

        void SelectExtentTraceResultsTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            bool decrementForArrows = true;
            try
            {
                string guidField = TracingHelper.FieldIdentifierName;
                if (tracingHelper.CurrentResultsAreSchematics) { guidField = TracingHelper.SchemFieldIdentifierName; }

                //Now we have the results of features that need to be buffered
                if (e.FeatureSet.Features.Count > 0)
                {
                    //First check if this layer is visible.  If not, we won't buffer it
                    QueryTask originatingTask = sender as QueryTask;

                    //Determine if this layerID is visible currently
                    bool shouldDrawThisLayer = true;
                    string fullUrl = originatingTask.Url;
                    string layerUrl = fullUrl.Substring(0, fullUrl.LastIndexOf("/"));
                    string id = fullUrl.Substring(fullUrl.LastIndexOf("/") + 1, fullUrl.Length - fullUrl.LastIndexOf("/") - 1);
                    int layerID = Int32.Parse(id);
                    if (!ConfigUtility.IsLayerVisible(layerUrl, Map, layerID))
                    {
                        //This layer isn't visible so let's not try to draw the results for this layer
                        shouldDrawThisLayer = false;
                    }

                    //For point features (even if they aren't visible) we still need to build our list of guid to map points dictionary
                    //so that we can properly calculate the directional flow for any linear features
                    if (shouldDrawThisLayer || e.FeatureSet.GeometryType == GeometryType.Point)
                    {
                        foreach (Graphic feature in e.FeatureSet.Features)
                        {
                            string currentGUID = feature.Attributes[guidField].ToString();
                            //Adjust globalID in case it doesn't begin and end in {} and also call upper.  This ensures it should match a string in results
                            if (!currentGUID.Contains("{")) { currentGUID = "{" + currentGUID; }
                            if (!currentGUID.Contains("}")) { currentGUID = currentGUID + "}"; }
                            currentGUID = currentGUID.ToUpper();
                            if (e.FeatureSet.GeometryType == GeometryType.Polyline)
                            {
                                Polyline polyline = feature.Geometry as Polyline;
                                lock (GuidToPolylineShapeLock)
                                {
                                    try
                                    {
                                        GuidToPolylineShapes.Add(currentGUID, new List<Polyline>());
                                    }
                                    catch
                                    {
                                        //This may fail if we are in schematics and the a duplicate guid is attempted to
                                        //be added to our dictionary.  Ignore this as the first one is sufficient.
                                    }
                                    GuidToPolylineShapes[currentGUID].Add(polyline);
                                }
                            }
                        }
                    }

                    if (shouldDrawThisLayer)
                    {
                        /*
                        double TraceBufferSize = AttributesControl.TraceBufferSize;
                        if (e.FeatureSet.GeometryType == GeometryType.Point)
                        {
                            //For point features let's make the buffer slightly larger (20%) so we can see a difference when
                            //drawn on the map.
                            TraceBufferSize = TraceBufferSize * 1.4;
                        }
                        
                        //First we need to buffer our point click by using the geometry service.
                        BufferParameters bufferParams = new BufferParameters();
                        bufferParams.Distances.Add(TraceBufferSize);
                        bufferParams.Unit = new LinearUnit?(AttributesControl.TraceBufferUnit);
                        bufferParams.Features.AddRange(e.FeatureSet.Features);

                        //Make the geometry service call
                        GeometryService BufferExtentTraceResultsService = new GeometryService(ConfigUtility.GeometryServiceURL);
                        BufferExtentTraceResultsService.BufferCompleted += new EventHandler<GraphicsEventArgs>(BufferExtentTraceResultsService_BufferCompleted);
                        BufferExtentTraceResultsService.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(BufferExtentTraceResultsService_Failed);
                        TracingHelper.CurrentlyExecutingTasks.Add(BufferExtentTraceResultsService);
                        CurrentlyExecutingDrawTasks.Add(BufferExtentTraceResultsService);
                        BufferExtentTraceResultsService.BufferAsync(bufferParams);
                        */

                        TracedJunctionSymbol.Color = AttributesControl.TraceBufferColor;
                        TracedEdgeSymbol.Color = AttributesControl.TraceBufferColor;
                        if (AttributesControl.TraceBufferSize > 0)
                        {
                            TracedJunctionSymbol.Size = (5.0 + AttributesControl.TraceBufferSize) * 2;
                            TracedEdgeSymbol.Width = 5.0 + AttributesControl.TraceBufferSize;
                        }
                        else
                        {
                            TracedJunctionSymbol.Size = 7;
                            TracedEdgeSymbol.Width = 5;
                        }
                        for (int i = 0; i < e.FeatureSet.Features.Count; i++)
                        {
                            if (e.FeatureSet.Features[i].Geometry is MapPoint)
                            {
                                e.FeatureSet.Features[i].Symbol = TracedJunctionSymbol;
                                e.FeatureSet.Features[i].SetZIndex(1);
                            }
                            else
                            {
                                e.FeatureSet.Features[i].Symbol = TracedEdgeSymbol;
                                e.FeatureSet.Features[i].SetZIndex(0);
                            }
                        }

                        AddGraphics(e.FeatureSet.Features);

                        decrementForArrows = true;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (decrementForArrows)
                {
                    lock (ToProcessBeforeArrowsLock)
                    {
                        ToProcessBeforeArrows--;
                        if (ToProcessBeforeArrows <= 0)
                        {
                            DrawDirectionalArrows();
                        }
                    }
                }
            }
        }

        private static Dictionary<string, string> upstreamGUIDMap = new Dictionary<string, string>();
        private static Dictionary<string, string> downstreamGUIDMap = new Dictionary<string, string>();

        private void DrawDirectionalArrows()
        {
            try
            {
                if (!TraceFlowToggle) { return; }
                lock (GuidToPolylineShapeLock)
                {
                    lock (GuidToMapPointShapeLock)
                    {
                        upstreamGUIDMap.Clear();
                        downstreamGUIDMap.Clear();

                        Dictionary<int, List<string>> UpAndDownstreamTraceItemMap = new Dictionary<int, List<string>>();

                        foreach (TraceItem item in tracingHelper.CurrentFullTraceResultsInOrder)
                        {
                            try
                            {
                                if (GuidToPolylineShapes.ContainsKey(item.GlobalID))
                                {
                                    int itemIndex = tracingHelper.CurrentFullTraceResultsInOrder.IndexOf(item);

                                    if (tracingHelper.CurrentResultsAreUpstream)
                                    {
                                        if (itemIndex > 0)
                                        {
                                            TraceItem tempItem = tracingHelper.CurrentFullTraceResultsInOrder[itemIndex - 1];
                                            downstreamGUIDMap.Add(item.GlobalID, tempItem.GlobalID);
                                            try
                                            {
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                            catch
                                            {
                                                UpAndDownstreamTraceItemMap.Add(tempItem.ClassID, new List<string>());
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                        }
                                        if (itemIndex < tracingHelper.CurrentFullTraceResultsInOrder.Count - 1)
                                        {
                                            TraceItem tempItem = tracingHelper.CurrentFullTraceResultsInOrder[itemIndex + 1];
                                            upstreamGUIDMap.Add(item.GlobalID, tempItem.GlobalID);
                                            try
                                            {
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                            catch
                                            {
                                                UpAndDownstreamTraceItemMap.Add(tempItem.ClassID, new List<string>());
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (itemIndex > 0)
                                        {
                                            TraceItem tempItem = tracingHelper.CurrentFullTraceResultsInOrder[itemIndex - 1];
                                            upstreamGUIDMap.Add(item.GlobalID, tempItem.GlobalID);
                                            try
                                            {
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                            catch
                                            {
                                                UpAndDownstreamTraceItemMap.Add(tempItem.ClassID, new List<string>());
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                        }
                                        if (itemIndex < tracingHelper.CurrentFullTraceResultsInOrder.Count - 1)
                                        {
                                            TraceItem tempItem = tracingHelper.CurrentFullTraceResultsInOrder[itemIndex + 1];
                                            downstreamGUIDMap.Add(item.GlobalID, tempItem.GlobalID);
                                            try
                                            {
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                            catch
                                            {
                                                UpAndDownstreamTraceItemMap.Add(tempItem.ClassID, new List<string>());
                                                UpAndDownstreamTraceItemMap[tempItem.ClassID].Add(tempItem.GlobalID);
                                            }
                                        }
                                    }
                                }
                            }
                            catch { }
                        }

                        lock (UpAndDownstreamLock)
                        {
                            globalIDsDrawn.Clear();
                            UpAndDownstreamToProcess = 0;
                            foreach (KeyValuePair<int, List<string>> item in UpAndDownstreamTraceItemMap)
                            {
                                List<int> layerIDs = ConfigUtility.GetLayerIDFromClassID(tracingHelper.currentTracingTableUrl, item.Key);
                                //Query for the geometry of the upstream or downstream feature list so that we can determine the 
                                //actual directional flow for all of the poly line shapes that we have

                                foreach (int layerID in layerIDs)
                                {
                                    string tracingURL = tracingHelper.currentTracingTableUrl + "/" + layerID;
                                    StringBuilder whereInClause = new StringBuilder();
                                    for (int i = 0; i < item.Value.Count; i++)
                                    {
                                        if (i != item.Value.Count - 1)
                                        {
                                            whereInClause.Append("'" + item.Value[i] + "',");
                                        }
                                        else
                                        {
                                            whereInClause.Append("'" + item.Value[i] + "'");
                                        }
                                    }


                                    Query query = new Query();
                                    query.ReturnGeometry = true;

                                    if (tracingHelper.CurrentResultsAreSchematics)
                                    {
                                        query.OutFields.AddRange(new string[] { TracingHelper.SchemFieldIdentifierName });
                                        query.Where = TracingHelper.SchemFieldIdentifierName + " IN (" + whereInClause + ")";
                                    }
                                    else
                                    {
                                        query.OutFields.AddRange(new string[] { TracingHelper.FieldIdentifierName });
                                        query.Where = TracingHelper.FieldIdentifierName + " IN (" + whereInClause + ")";
                                    }

                                    QueryTask UpAndDownstreamItemCheck = new QueryTask();
                                    UpAndDownstreamItemCheck.Url = tracingURL;
                                    UpAndDownstreamItemCheck.ExecuteCompleted += new EventHandler<QueryEventArgs>(UpAndDownstreamItemCheck_ExecuteCompleted);
                                    UpAndDownstreamItemCheck.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(UpAndDownstreamItemCheck_Failed);
                                    CurrentlyExecutingDrawTasks.Add(UpAndDownstreamItemCheck);
                                    UpAndDownstreamToProcess++;
                                    UpAndDownstreamItemCheck.ExecuteAsync(query);
                                }
                            }
                        }


                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Object UpAndDownstreamLock = new Object();
        private int UpAndDownstreamToProcess = 0;

        void UpAndDownstreamItemCheck_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            lock (UpAndDownstreamLock)
            {
                UpAndDownstreamToProcess--;
                if (UpAndDownstreamToProcess == 0)
                {
                    foreach (KeyValuePair<string, List<Polyline>> kvp in GuidToPolylineShapes)
                    {
                        if (globalIDsDrawn.Contains(kvp.Key)) { continue; }

                        foreach (Polyline lineGeom in kvp.Value)
                        {
                            Graphic graphic = new Graphic();
                            graphic.Geometry = lineGeom;
                            DirectionStaticSymbol.Color = FlowAnimationColor;
                            graphic.Symbol = DirectionStaticSymbol;
                            graphic.SetZIndex(2);
                            AddGraphic(graphic);
                            globalIDsDrawn.Add(kvp.Key);
                        }
                    }
                }
            }
        }

        private List<string> globalIDsDrawn = new List<string>();
        /// <summary>
        /// This method is responsible for determining the direction of the flow for all of the line segments that
        /// are returned to this method.  It relies on the upstream and downstream guid maps that are built by
        /// the calling query.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UpAndDownstreamItemCheck_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                lock (GuidToMapPointShapeLock)
                {
                    lock (UpAndDownstreamLock)
                    {
                        List<MapPoint> allPoints = new List<MapPoint>();
                        Dictionary<string, List<MapPoint>> GuidToMapPointShapes = new Dictionary<string, List<MapPoint>>();

                        //Build our list of globalIDs that exist in this query results.
                        if (e.FeatureSet.GeometryType == GeometryType.Point)
                        {
                            foreach (Graphic feature in e.FeatureSet.Features)
                            {
                                string globalID = "";
                                if (tracingHelper.CurrentResultsAreSchematics)
                                {
                                    globalID = feature.Attributes[TracingHelper.SchemFieldIdentifierName].ToString();
                                }
                                else
                                {
                                    globalID = feature.Attributes[TracingHelper.FieldIdentifierName].ToString();
                                }

                                if (!globalID.Contains("{")) { globalID = "{" + globalID; }
                                if (!globalID.Contains("}")) { globalID = globalID + "}"; }
                                globalID = globalID.ToUpper();

                                MapPoint point = feature.Geometry as MapPoint;
                                if (point != null)
                                {
                                    try
                                    {
                                        GuidToMapPointShapes.Add(globalID, new List<MapPoint>());
                                    }
                                    catch { }

                                    GuidToMapPointShapes[globalID].Add(point);
                                    allPoints.Add(point);
                                }
                            }
                        }

                        //Now we need to parse through all of our polylines dictionary that we built earlier
                        //to determine the direction based on the MapPoints in this current results set.
                        foreach (KeyValuePair<string, List<Polyline>> kvp in GuidToPolylineShapes)
                        {
                            if (globalIDsDrawn.Contains(kvp.Key)) { continue; }

                            foreach (Polyline line in kvp.Value)
                            {
                                //To determine the direction we need the first and last point in the polyline.  The animation
                                //symbol will always draw in the direction of first point to last point.  so we need to determine
                                //whether the first or the last point is coincident with its associate upstream or downstream
                                //point feature.  From this we can determine if the line needs to be rebuilt in the reverse order
                                //or not, to fix the animation direction.
                                bool couldDetermineDirection = false;
                                MapPoint firstPoint = line.Paths[0][0];
                                firstPoint.SpatialReference = line.SpatialReference;
                                MapPoint lastPoint = line.Paths[0][line.Paths[0].Count - 1];
                                lastPoint.SpatialReference = line.SpatialReference;
                                bool reverse = true;

                                if (allPoints.Contains(firstPoint))
                                {
                                    foreach (KeyValuePair<string, List<MapPoint>> pointList in GuidToMapPointShapes)
                                    {
                                        foreach (MapPoint point in pointList.Value)
                                        {
                                            if (point.Equals(firstPoint))
                                            {
                                                if (downstreamGUIDMap.ContainsKey(kvp.Key) && downstreamGUIDMap[kvp.Key] == pointList.Key)
                                                {
                                                    reverse = false;
                                                    couldDetermineDirection = true;
                                                    break;
                                                }
                                                else if (upstreamGUIDMap.ContainsKey(kvp.Key) && upstreamGUIDMap[kvp.Key] == pointList.Key)
                                                {
                                                    couldDetermineDirection = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (allPoints.Contains(lastPoint))
                                {
                                    foreach (KeyValuePair<string, List<MapPoint>> pointList in GuidToMapPointShapes)
                                    {
                                        foreach (MapPoint point in pointList.Value)
                                        {
                                            if (point.Equals(lastPoint))
                                            {
                                                if (upstreamGUIDMap.ContainsKey(kvp.Key) && upstreamGUIDMap[kvp.Key] == pointList.Key)
                                                {
                                                    reverse = false;
                                                    couldDetermineDirection = true;
                                                    break;
                                                }
                                                else if (downstreamGUIDMap.ContainsKey(kvp.Key) && downstreamGUIDMap[kvp.Key] == pointList.Key)
                                                {
                                                    couldDetermineDirection = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (reverse && couldDetermineDirection)
                                {
                                    //Reverse our point collection for the animation to be in the correct direction
                                    ObservableCollection<PointCollection> newObservableCollection = new ObservableCollection<PointCollection>();
                                    foreach (PointCollection collection in line.Paths)
                                    {
                                        PointCollection newCollection = new PointCollection();
                                        for (int i = (collection.Count - 1); i >= 0; i--)
                                        {
                                            newCollection.Add(collection[i]);
                                        }
                                        newObservableCollection.Add(newCollection);
                                    }
                                    line.Paths = newObservableCollection;
                                }

                                if (couldDetermineDirection)
                                {
                                    //We were able to determine the direction for this graphic.  Let's draw a new
                                    //graphic with the specified symbol.
                                    Graphic graphic = new Graphic();
                                    graphic.Geometry = line;
                                    DirectionAnimationSymbol.Color = FlowAnimationColor;
                                    graphic.Symbol = DirectionAnimationSymbol;
                                    graphic.SetZIndex(2);
                                    AddGraphic(graphic);
                                    globalIDsDrawn.Add(kvp.Key);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            finally
            {
                lock (UpAndDownstreamLock)
                {
                    UpAndDownstreamToProcess--;
                    if (UpAndDownstreamToProcess == 0)
                    {
                        foreach (KeyValuePair<string, List<Polyline>> kvp in GuidToPolylineShapes)
                        {
                            if (globalIDsDrawn.Contains(kvp.Key)) { continue; }

                            foreach (Polyline line in kvp.Value)
                            {
                                //For any globalIDs that could not have their direction determined we
                                //will draw them with a static flow direction indicating the flow can't
                                //be determined.
                                Graphic graphic = new Graphic();
                                graphic.Geometry = line;
                                DirectionStaticSymbol.Color = FlowAnimationColor;
                                graphic.Symbol = DirectionStaticSymbol;
                                graphic.SetZIndex(2);
                                AddGraphic(graphic);
                                globalIDsDrawn.Add(kvp.Key);
                            }
                        }
                    }
                }
            }
        }

        void BufferExtentTraceResultsService_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            lock (ToProcessBeforeArrowsLock)
            {
                ToProcessBeforeArrows--;
                if (ToProcessBeforeArrows <= 0)
                {
                    DrawDirectionalArrows();
                }
            }
        }

        //Buffer a set of trace results features
        void BufferExtentTraceResultsService_BufferCompleted(object sender, GraphicsEventArgs e)
        {
            try
            {
                //Now that I have my set of results within the current extent we can draw the graphics to the map using the
                //configured fill symbol
                GraphicsLayer pgeTraceGraphicsLayer = getTracingGraphicsLayer();

                SimpleFillSymbol symbol2 = new SimpleFillSymbol();
                symbol2.BorderThickness = 1.0;
                System.Windows.Media.Color blackBorderColor = System.Windows.Media.Colors.Black;
                symbol2.BorderBrush = new System.Windows.Media.SolidColorBrush(blackBorderColor);
                symbol2.Fill = AttributesControl.TraceBufferColor;


                for (int i = 0; i < e.Results.Count; i++)
                {
                    e.Results[i].Symbol = symbol2;
                    if (e.Results[i].Geometry is MapPoint)
                    {
                        e.Results[i].SetZIndex(1);
                    }
                    else
                    {
                        e.Results[i].SetZIndex(0);
                    }
                }

                AddGraphics(e.Results);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                lock (ToProcessBeforeArrowsLock)
                {
                    ToProcessBeforeArrows--;
                    if (ToProcessBeforeArrows <= 0)
                    {
                        DrawDirectionalArrows();
                    }
                }
            }
        }

        private void CancelCurrentDrawTasks()
        {
            foreach (TaskBase task in CurrentlyExecutingDrawTasks)
            {
                if (task != null && task.IsBusy)
                {
                    task.CancelAsync();
                }
            }
            CurrentlyExecutingDrawTasks.Clear();
        }

        #endregion

        #region Get Layer ID Names for Trace Results Window

        Object LayerNamesByClassIDLock = new Object();
        int layerNamesByClassIDCount = 0;
        int initialLayerNamesByClassIDCount = 0;
        private void GetLayerNamesByClassID()
        {
            lock (LayerNamesByClassIDLock)
            {
                if (origCheckBoxes == null)
                {
                    //Configure the default visible devices for traces
                    origCheckBoxes = new Dictionary<string, bool>();
                    foreach (int classID in TracingHelper.DefaultVisibleClassIDs)
                    {
                        foreach (Layer layer in Map.Layers)
                        {
                            if (layer is ArcGISDynamicMapServiceLayer && layer.Visible)
                            {
                                ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;
                                if (dynamicMapLayer.Url != tracingHelper.currentIdentifyUrl) { continue; }
                                List<int> layerIDs = ConfigUtility.GetLayerIDFromClassID(tracingHelper.currentIdentifyUrl, classID);
                                foreach (LayerInfo info in dynamicMapLayer.Layers)
                                {
                                    if (layerIDs.Contains(info.ID))
                                    {
                                        try
                                        {
                                            origCheckBoxes.Add(info.Name, true);
                                        }
                                        catch { }
                                    }
                                }
                            }
                        }
                    }
                }

                layerNamesByClassIDCount = 0;
                foreach (Layer layer in Map.Layers)
                {
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        ArcGISDynamicMapServiceLayer dynamicMapLayer = layer as ArcGISDynamicMapServiceLayer;
                        if ((layer.Visible && !tracingHelper.CurrentResultsAreSchematics) ||
                            (tracingHelper.CurrentResultsAreSchematics && dynamicMapLayer.Url == tracingHelper.currentIdentifyUrl))
                        {
                            foreach (KeyValuePair<int, List<TraceItem>> kvp in tracingHelper.CurrentFullTraceResultsByClassID)
                            {
                                int classID = kvp.Key;
                                string currentLayerURL = dynamicMapLayer.Url;
                                List<int> LayerIDsToQuery = ConfigUtility.GetLayerIDFromClassID(currentLayerURL, classID).ToList();
                                if (LayerIDsToQuery.Count < 1) { continue; }
                                bool containsOpNumField = false;
                                bool containsStructureNumberField = false;

                                for (int i = 0; i < LayerIDsToQuery.Count; i++)
                                {
                                    if (ConfigUtility.GetLayerIDToFieldMapping(currentLayerURL, LayerIDsToQuery[0]).Contains(TracingHelper.OperatingNumberFieldName)) { containsOpNumField = true; }
                                    else { containsOpNumField = false; }

                                    if (ConfigUtility.GetLayerIDToFieldMapping(currentLayerURL, LayerIDsToQuery[0]).Contains(TracingHelper.StructureNumberFieldName)) { containsStructureNumberField = true; }
                                    else { containsStructureNumberField = false; }

                                    Query query = new Query();
                                    query.ReturnGeometry = false;
                                    string subtypeFieldName = ConfigUtility.GetSubtypeFieldName(classID);
                                    if (tracingHelper.CurrentResultsAreSchematics)
                                    {
                                        if (containsOpNumField) { query.OutFields.AddRange(new string[] { TracingHelper.SchemIDFieldName, TracingHelper.SchemFieldIdentifierName, TracingHelper.OperatingNumberFieldName }); }
                                        else { query.OutFields.AddRange(new string[] { TracingHelper.SchemFieldIdentifierName, TracingHelper.SchemIDFieldName }); }
                                    }
                                    else
                                    {
                                        if (containsOpNumField) { query.OutFields.AddRange(new string[] { TracingHelper.ObjectIDFieldName, TracingHelper.FieldIdentifierName, TracingHelper.OperatingNumberFieldName }); }
                                        else if (containsStructureNumberField) { { query.OutFields.AddRange(new string[] { TracingHelper.ObjectIDFieldName, TracingHelper.FieldIdentifierName, TracingHelper.StructureNumberFieldName }); } }
                                        else { query.OutFields.AddRange(new string[] { TracingHelper.FieldIdentifierName, TracingHelper.ObjectIDFieldName }); }
                                    }

                                    //ME Q4 2019 Release DA# 190806
                                    string customDisplayFieldName = getTraceCustomDisplayField(classID);
                                    if(customDisplayFieldName != null)
                                        query.OutFields.Add(customDisplayFieldName);                                   

                                    query.OutFields.Add(subtypeFieldName);

                                    //Code to add circuit ID field in out fields in order to show in trace result
                                    List<string> fieldsToObtain = TracingHelper.PGETraceResultFieldsByClassID[classID];
                                    for (int iCnt = 0; iCnt < TracingHelper.PGETraceResultLayerAliasNames.Count; iCnt++)
                                    {
                                        if (TracingHelper.PGETraceResultLayerAliasNames.ContainsKey(classID.ToString()))
                                        {
                                            query.OutFields.AddRange(fieldsToObtain);
                                        }
                                    }

                                    //
                                    List<string> whereInStringList = new List<string>();
                                    StringBuilder whereInStringBuilder = new StringBuilder();
                                    for (int item = 0; item < kvp.Value.Count; item++)
                                    {
                                        if (item == kvp.Value.Count - 1 || (item % 999) == 0) { whereInStringBuilder.Append("'" + kvp.Value[item].GlobalID + "'"); }
                                        else { whereInStringBuilder.Append("'" + kvp.Value[item].GlobalID + "'" + ","); }

                                        if ((item % 999) == 0)
                                        {
                                            whereInStringList.Add(whereInStringBuilder.ToString());
                                            whereInStringBuilder = new StringBuilder();
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(whereInStringBuilder.ToString()) && !whereInStringList.Contains(whereInStringBuilder.ToString()))
                                    {
                                        whereInStringList.Add(whereInStringBuilder.ToString());
                                    }

                                    foreach (string whereInString in whereInStringList)
                                    {
                                        if (tracingHelper.CurrentResultsAreSchematics)
                                        {
                                            query.Where = TracingHelper.SchemFieldIdentifierName + " in (" + whereInString + ")";
                                        }
                                        else
                                        {
                                            query.Where = TracingHelper.FieldIdentifierName + " in (" + whereInString + ")";
                                        }
                                        layerNamesByClassIDCount++;
                                        //Now query the first layerID returned to zoom to the feature
                                        QueryTask LayerIDToOIDMappingTask = new QueryTask(currentLayerURL + "/" + LayerIDsToQuery[i]);
                                        LayerIDToOIDMappingTask.ExecuteCompleted += new EventHandler<QueryEventArgs>(LayerIDToOIDMappingTask_ExecuteCompleted);
                                        LayerIDToOIDMappingTask.Failed += new EventHandler<ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs>(LayerIDToOIDMappingTask_Failed);
                                        tracingHelper.CurrentlyExecutingTasks.Add(LayerIDToOIDMappingTask);
                                        LayerIDToOIDMappingTask.ExecuteAsync(query);
                                    }
                                }
                            }
                        }
                    }
                }
                initialLayerNamesByClassIDCount = layerNamesByClassIDCount;
            }
        }

        void LayerIDToOIDMappingTask_Failed(object sender, ESRI.ArcGIS.Client.Tasks.TaskFailedEventArgs e)
        {
            //Still need to decrement our counter
            lock (LayerNamesByClassIDLock)
            {
                layerNamesByClassIDCount--;

                double percentComplete = Math.Round((((double)(initialLayerNamesByClassIDCount - layerNamesByClassIDCount)) / ((double)initialLayerNamesByClassIDCount)) * 100.0, 0);
                ConfigUtility.StatusBar.Text = "Analyzing Trace Results: " + percentComplete + "%";
                ConfigUtility.StatusBar.UpdateLayout();

                if (layerNamesByClassIDCount <= 0)
                {
                    CleanupLayers(true, cleanedUpResults);

                    ConfigUtility.StatusBar.Text = "";
                    ConfigUtility.StatusBar.UpdateLayout();
                }
            }
        }

        private static Object TraceResultsLock = new Object();
        private List<TraceItem> cleanedUpResults = new List<TraceItem>();
        void LayerIDToOIDMappingTask_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                if (e.FeatureSet.Features.Count <= 0)
                {
                    return;
                }

                QueryTask queryTask = sender as QueryTask;
                string url = queryTask.Url;
                string mapServer = url.Substring(0, url.LastIndexOf("/"));
                int layerID = -1;
                string layerIDString = url.Substring(url.LastIndexOf("/") + 1);
                Int32.TryParse(layerIDString, out layerID);

                if (layerID == -1) { return; }

                int classID = ConfigUtility.GetClassIDFromLayerID(mapServer, layerID);

                string layerAlias = null;
                foreach (Layer layer in Map.Layers)
                {
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                        if (mapServer == dynamicLayer.Url)
                        {
                            foreach (LayerInfo layerInfo in dynamicLayer.Layers)
                            {
                                if (layerInfo.ID == layerID)
                                {
                                    layerAlias = layerInfo.Name;
                                    break;
                                }
                            }
                        }
                    }
                }

                lock (TraceResultsLock)
                {
                    //List<TraceItem> newResults = TraceResults.ToList();
                    foreach (Graphic feature in e.FeatureSet.Features)
                    {
                        string displayValue = "";
                        string oid = "";
                        string globalID = "";

                        if (tracingHelper.CurrentResultsAreSchematics)
                        {
                            globalID = feature.Attributes[TracingHelper.SchemFieldIdentifierName].ToString();
                            oid = feature.Attributes[TracingHelper.SchemIDFieldName].ToString();
                        }
                        else
                        {
                            globalID = feature.Attributes[TracingHelper.FieldIdentifierName].ToString();
                            oid = feature.Attributes[TracingHelper.ObjectIDFieldName].ToString();
                        }

                        //Adjust globalID in case it doesn't begin and end in {} and also call upper.  This ensures it should match a string in results
                        if (!globalID.Contains("{")) { globalID = "{" + globalID; }
                        if (!globalID.Contains("}")) { globalID = globalID + "}"; }
                        globalID = globalID.ToUpper();

                        if (feature.Attributes.ContainsKey(TracingHelper.OperatingNumberFieldName) && feature.Attributes[TracingHelper.OperatingNumberFieldName] != null)
                        {
                            displayValue = feature.Attributes[TracingHelper.OperatingNumberFieldName].ToString();
                        }
                        else if (feature.Attributes.ContainsKey(TracingHelper.StructureNumberFieldName))
                        {
                            displayValue = feature.Attributes[TracingHelper.StructureNumberFieldName].ToString();
                        }

                        string subtypeName = "";
                        string subtypeFieldName = ConfigUtility.GetSubtypeFieldName(classID).ToUpper();
                        if (!string.IsNullOrEmpty(subtypeFieldName) && feature.Attributes.ContainsKey(subtypeFieldName) && feature.Attributes[subtypeFieldName] != null)
                        {
                            subtypeName = feature.Attributes[subtypeFieldName].ToString();
                        }

                        if (String.IsNullOrEmpty(displayValue) || displayValue == "<Null>") { displayValue = oid; }
                        TraceItem searchItem = new TraceItem(classID, globalID.ToString(), "", "");
                        //string searchString = classID + ":" + globalID;
                        int searchIndex = cleanedUpResults.IndexOf(searchItem);
                        if (searchIndex >= 0)
                        {
                            string description = layerAlias + " - " + displayValue;
                            if (!string.IsNullOrEmpty(subtypeName))
                            {
                                List<int> publicationLayerID = ConfigUtility.GetLayerIDFromClassID(TracingHelper.PGEElectricTracingURL, classID);
                                //if (publicationLayerID.Count > 0 && TracingHelper.ClassIDsToDisplaySubtype.Contains(classID))
                                //{
                                //    subtypeName = ConfigUtility.GetDomainDescription(TracingHelper.PGEElectricTracingURL, publicationLayerID[0], subtypeName, subtypeFieldName, subtypeName);
                                //    description = subtypeName + " - " + displayValue;
                                //}
                                //ME Q4 2019 Release DA# 190806
                                if (publicationLayerID.Count > 0 && TracingHelper.PGETraceResultCustomDisplayField.Keys.Any(k => Convert.ToInt16(k.ToString().Split('-')[0]) == classID))
                                {
                                    string customDisplayValue = string.Empty;
                                    string customDisplayFieldName = getTraceCustomDisplayField(classID);
                                    if (customDisplayFieldName != null)
                                    {
                                        //handle display field codes
                                        List<string> displayFieldCodes = TracingHelper.PGETraceResultCustomDisplayField.Where(k => Convert.ToInt16(k.Key.ToString().Split('-')[0]) == classID).FirstOrDefault().Value;
                                        if (displayFieldCodes != null && displayFieldCodes.Count > 0)
                                        {
                                            if(displayFieldCodes.Contains(feature.Attributes[customDisplayFieldName].ToString()))
                                                 customDisplayValue = ConfigUtility.GetDomainDescription(TracingHelper.PGEElectricTracingURL, publicationLayerID[0], subtypeName, customDisplayFieldName, feature.Attributes[customDisplayFieldName].ToString());
                                        }
                                        else{
                                              customDisplayValue = ConfigUtility.GetDomainDescription(TracingHelper.PGEElectricTracingURL, publicationLayerID[0], subtypeName, customDisplayFieldName, feature.Attributes[customDisplayFieldName].ToString());
                                        }
                                        if (customDisplayValue.Length > 0)
                                            description = customDisplayValue + " - " + displayValue;
                                        else
                                        {
                                            if (TracingHelper.ClassIDsToDisplaySubtype.Contains(classID))
                                            {
                                                subtypeName = ConfigUtility.GetDomainDescription(TracingHelper.PGEElectricTracingURL, publicationLayerID[0], subtypeName, subtypeFieldName, subtypeName);
                                                description = subtypeName + " - " + displayValue;
                                            }
                                        }
                                    }
                                }
                            }
                            //Showing Circuit id in trace result in case of circuit pouint
                            string[] strLyrName = null; string strCircuitID = string.Empty; string stModifiedCirID = string.Empty;
                            for (int iCnt = 0; iCnt < TracingHelper.PGETraceResultLayerAliasNames.Count; iCnt++)
                            {
                                //for (int jCnt = 0;jCnt<
                                if (TracingHelper.PGETraceResultLayerAliasNames.ElementAt(iCnt).Value.Contains(layerAlias))
                                {
                                    if (feature.Attributes.ContainsKey("CIRCUITID"))
                                    {
                                        strCircuitID = feature.Attributes["CIRCUITID"].ToString();
                                        if (strCircuitID.Length >= 8 && strCircuitID.Contains('-') == false)
                                        {
                                            stModifiedCirID = strCircuitID.Substring(strCircuitID.Length - 4, 4) + "/2";
                                            description = description.Replace(displayValue, stModifiedCirID);
                                        }
                                    }
                                    strLyrName = TracingHelper.PGETraceResultLayerAliasNames.ElementAt(iCnt).Value.Split(';');
                                    description = description.Replace(layerAlias, strLyrName[1]);
                                }
                            }
                            //
                            TraceItem newItem = new TraceItem(classID, globalID.ToString(), layerAlias, description);
                            cleanedUpResults[searchIndex] = newItem;
                        }
                    }

                    CallMethodsTraceResults = false;
                    //TraceResults = newResults;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                lock (LayerNamesByClassIDLock)
                {
                    layerNamesByClassIDCount--;

                    double percentComplete = Math.Round((((double)(initialLayerNamesByClassIDCount - layerNamesByClassIDCount)) / ((double)initialLayerNamesByClassIDCount)) * 100.0, 0);
                    ConfigUtility.StatusBar.Text = "Analyzing Trace Results: " + percentComplete + "%";
                    ConfigUtility.StatusBar.UpdateLayout();

                    if (layerNamesByClassIDCount <= 0)
                    {
                        CleanupLayers(true, cleanedUpResults);

                        ConfigUtility.StatusBar.Text = "";
                        ConfigUtility.StatusBar.UpdateLayout();
                    }
                }
            }
        }

        private void CleanupLayers(bool cacheOriginalList, List<TraceItem> workingList)
        {
            try
            {
                if (cacheOriginalList)
                {
                    OrigTraceResults = workingList.ToList();
                }

                PrepareLayersToInclude();

                List<string> layersToInclude = GetLayersToIncludeInResultsList();
                List<TraceItem> toDelete = new List<TraceItem>();
                List<TraceItem> currentResults = new List<TraceItem>();
                //Check if any features couldn't be found due to definition queries.
                foreach (TraceItem item in OrigTraceResults)
                {
                    if (!item.ToString().Contains(":"))
                    {
                        int lastIndexDash = item.ToString().LastIndexOf(" - ");
                        string layerName = item.LayerAliasName;
                        if (layersToInclude.Contains(layerName))
                        {
                            currentResults.Add(item);
                        }
                    }
                }

                /*
                foreach (TraceItem item in OrigTraceResults)
                {
                    if (!toDelete.Contains(item))
                    {
                        currentResults.Add(item);
                    }
                }
            
                foreach (TraceItem toDeleteItem in toDelete)
                {
                    while (currentResults.Contains(toDeleteItem))
                    {
                        currentResults.Remove(toDeleteItem);
                    }
                }
                */
                TraceResultsToggle.IsChecked = true;
                TraceResults = currentResults;
            }
            catch (Exception ex)
            {

            }
        }

        private Dictionary<string, bool> origCheckBoxes = null;
        private void PrepareLayersToInclude()
        {
            Dictionary<string, bool> checkBoxesToCreate = new Dictionary<string, bool>();
            //List<string> checkBoxesAlreadyExisting = new List<string>();
            foreach (UIElement UIEle in LayersToIncludeStackPanel.Children)
            {
                if (UIEle is CheckBox)
                {
                    CheckBox chkBox = UIEle as CheckBox;
                    if (!origCheckBoxes.ContainsKey(chkBox.Content.ToString()))
                    {
                        origCheckBoxes.Add(chkBox.Content.ToString(), Boolean.Parse(chkBox.IsChecked.ToString()));
                    }
                    else
                    {
                        origCheckBoxes[chkBox.Content.ToString()] = Boolean.Parse(chkBox.IsChecked.ToString());
                    }
                }
            }

            List<string> layerNamesChecked = new List<string>();
            foreach (TraceItem item in OrigTraceResults)
            {
                if (item.ToString().Contains(":"))
                {
                    continue;
                }
                int lastIndexDash = item.ToString().LastIndexOf(" - ");
                string layerAliasName = item.LayerAliasName;
                if (!checkBoxesToCreate.ContainsKey(layerAliasName))
                {
                    if (layerNamesChecked.Contains(layerAliasName)) { continue; }

                    layerNamesChecked.Add(layerAliasName);

                    int thisLayerID = ConfigUtility.GetLayerIDFromLayerName(Map, tracingHelper.currentIdentifyUrl, layerAliasName);
                    if (thisLayerID < 0) { continue; }
                    //if (!ConfigUtility.IsLayerVisible(TracingHelper.currentIdentifyUrl, Map, thisLayerID)) {continue;}

                    if (origCheckBoxes.ContainsKey(layerAliasName))
                    {
                        checkBoxesToCreate.Add(layerAliasName, origCheckBoxes[layerAliasName]);
                    }
                    else
                    {
                        checkBoxesToCreate.Add(layerAliasName, false);
                    }
                }
            }

            foreach (UIElement UIEle in LayersToIncludeStackPanel.Children)
            {
                if (UIEle is CheckBox)
                {
                    CheckBox chkBox = UIEle as CheckBox;
                    chkBox.Checked -= new RoutedEventHandler(newLayer_Checked);
                    chkBox.Unchecked -= new RoutedEventHandler(newLayer_Unchecked);
                }
            }

            LayersToIncludeStackPanel.Children.Clear();
            List<string> orderedList = checkBoxesToCreate.Keys.ToList();
            orderedList.Sort();
            foreach (string item in orderedList)
            {
                bool isChecked = checkBoxesToCreate[item];
                CheckBox newLayer = new CheckBox();
                newLayer.Content = item;
                newLayer.IsChecked = isChecked;
                newLayer.Checked += new RoutedEventHandler(newLayer_Checked);
                newLayer.Unchecked += new RoutedEventHandler(newLayer_Unchecked);
                LayersToIncludeStackPanel.Children.Add(newLayer);
            }
        }

        void newLayer_Unchecked(object sender, RoutedEventArgs e)
        {
            CleanupLayers(false, null);
        }

        void newLayer_Checked(object sender, RoutedEventArgs e)
        {
            CleanupLayers(false, null);
        }

        private List<string> GetLayersToIncludeInResultsList()
        {
            List<string> includeResults = new List<string>();
            foreach (UIElement UIEle in LayersToIncludeStackPanel.Children)
            {
                if (UIEle is CheckBox)
                {
                    CheckBox chkBox = UIEle as CheckBox;
                    if (Boolean.Parse(chkBox.IsChecked.ToString()))
                    {
                        includeResults.Add(chkBox.Content.ToString());
                    }
                }
            }
            return includeResults;
        }

        #endregion

        #region Dependency Properties

        public LoadingInformation LoadingInfo
        {
            get { return (LoadingInformation)GetValue(LoadingInfoProperty); }
            set { SetValue(LoadingInfoProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CustomerInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LoadingInfoProperty =
            DependencyProperty.Register("LoadingInfo", typeof(LoadingInformation), typeof(PGE_Tracing_Control), null);

        public bool TraceAPhase
        {
            get { return (bool)GetValue(TraceAPhaseProperty); }
            set { SetValue(TraceAPhaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraceAPhase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceAPhaseProperty =
            DependencyProperty.Register("TraceAPhase", typeof(bool), typeof(PGE_Tracing_Control), null);



        public bool TraceBPhase
        {
            get { return (bool)GetValue(TraceBPhaseProperty); }
            set { SetValue(TraceBPhaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraceBPhase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceBPhaseProperty =
            DependencyProperty.Register("TraceBPhase", typeof(bool), typeof(PGE_Tracing_Control), null);



        public bool TraceCPhase
        {
            get { return (bool)GetValue(TraceCPhaseProperty); }
            set { SetValue(TraceCPhaseProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraceCPhase.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceCPhaseProperty =
            DependencyProperty.Register("TraceCPhase", typeof(bool), typeof(PGE_Tracing_Control), null);


        public StackPanel LayersToIncludeStackPanel
        {
            get { return (StackPanel)GetValue(LayersToIncludeStackPanelProperty); }
            set { SetValue(LayersToIncludeStackPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LayersToIncludeStackPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LayersToIncludeStackPanelProperty =
            DependencyProperty.Register("LayersToIncludeStackPanel", typeof(StackPanel), typeof(PGE_Tracing_Control), null);

        /*
        public StackPanel ProtectiveDevicesPanel
        {
            get { return (StackPanel)GetValue(ProtectiveDevicesPanelProperty); }
            set { SetValue(ProtectiveDevicesPanelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ProtectiveDevicesPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProtectiveDevicesPanelProperty =
            DependencyProperty.Register("ProtectiveDevicesPanel", typeof(StackPanel), typeof(PGE_Tracing_Control), null);
        */



        public string TraceResultsCount
        {
            get { return (string)GetValue(TraceResultsCountProperty); }
            set { SetValue(TraceResultsCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraceResultsCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceResultsCountProperty =
            DependencyProperty.Register("TraceResultsCount", typeof(string), typeof(PGE_Tracing_Control), null);



        public string TraceResultsType
        {
            get { return (string)GetValue(TraceResultsTypeProperty); }
            set { SetValue(TraceResultsTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraceResultsType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceResultsTypeProperty =
            DependencyProperty.Register("TraceResultsType", typeof(string), typeof(PGE_Tracing_Control), null);


        public ToggleButton TraceResultsToggle
        {
            get { return (ToggleButton)GetValue(TraceResultsToggleProperty); }
            set { SetValue(TraceResultsToggleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraceResultsToggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceResultsToggleProperty =
            DependencyProperty.Register("TraceResultsToggle", typeof(ToggleButton), typeof(PGE_Tracing_Control), null);



        public bool TraceFlowToggle
        {
            get { return (bool)GetValue(TraceFlowToggleProperty); }
            set { SetValue(TraceFlowToggleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TraceFlowToggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceFlowToggleProperty =
            DependencyProperty.Register("TraceFlowToggle", typeof(bool), typeof(PGE_Tracing_Control), null);



        public bool FeederFedToggle
        {
            get { return (bool)GetValue(FeederFedToggleProperty); }
            set { SetValue(FeederFedToggleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FeederFedToggle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FeederFedToggleProperty =
            DependencyProperty.Register("FeederFedToggle", typeof(bool), typeof(PGE_Tracing_Control), null);




        public System.Windows.Media.SolidColorBrush FlowAnimationColor
        {
            get { return (System.Windows.Media.SolidColorBrush)GetValue(FlowAnimationColorProperty); }
            set
            {
                SetValue(FlowAnimationColorProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for FlowAnimationColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlowAnimationColorProperty =
            DependencyProperty.Register("FlowAnimationColor", typeof(System.Windows.Media.SolidColorBrush), typeof(PGE_Tracing_Control), null);



        public System.Windows.Media.SolidColorBrush FlashBufferColor
        {
            get { return (System.Windows.Media.SolidColorBrush)GetValue(FlashBufferColorProperty); }
            set
            {
                SetValue(FlashBufferColorProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for FlashBufferColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FlashBufferColorProperty =
            DependencyProperty.Register("FlashBufferColor", typeof(System.Windows.Media.SolidColorBrush), typeof(PGE_Tracing_Control), null);



        public AttributesViewerControl AttributesControl
        {
            get { return (AttributesViewerControl)GetValue(AttributesControlProperty); }
            set { SetValue(AttributesControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AttributesControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AttributesControlProperty =
            DependencyProperty.Register("AttributesControl", typeof(AttributesViewerControl), typeof(PGE_Tracing_Control), null);



        public MapTools MapToolsControl
        {
            get { return (MapTools)GetValue(MapToolsControlProperty); }
            set { SetValue(MapToolsControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MapToolsControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MapToolsControlProperty =
            DependencyProperty.Register("MapToolsControl", typeof(MapTools), typeof(PGE_Tracing_Control), null);

        private bool CallMethodsTraceResults = true;
        public List<TraceItem> TraceResults
        {
            get { return (List<TraceItem>)GetValue(TraceResultsProperty); }
            set
            {
                SetValue(TraceResultsProperty, value);
                TraceResultsCount = "Viewing " + value.Count + " results";
                if (CallMethodsTraceResults)
                {
                    if (value.Count > 0)
                    {
                        ConfigUtility.StatusBar.Text = "Analyzing Trace Results...";
                        ConfigUtility.StatusBar.UpdateLayout();
                    }

                    cleanedUpResults = value.ToList();
                    GetLayerNamesByClassID();

                    lock (GraphicsDrawingLock)
                    {
                        CancelCurrentDrawTasks();

                        DrawTraceGraphics();
                    }

                    if (value.Count <= 0)
                    {
                        ConfigUtility.StatusBar.Text = "The specified trace contains no results.";
                        ConfigUtility.StatusBar.UpdateLayout();
                    }
                }
            }
        }

        // Using a DependencyProperty as the backing store for TraceResults.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TraceResultsProperty =
            DependencyProperty.Register("TraceResults", typeof(List<TraceItem>), typeof(PGE_Tracing_Control), null);

        /// <summary>
        /// Gets the identifier for the <see cref="Map"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MapProperty = DependencyProperty.Register(
            "Map",
            typeof(Map),
            typeof(PGE_Tracing_Control),
            null);

        /// <summary>
        /// Gets or sets the Map.
        /// </summary>
        public Map Map
        {
            get
            {
                return (Map)GetValue(MapProperty);
            }
            set
            {
                SetValue(MapProperty, value);
            }
        }

        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActivePGEDownstreamProperty = DependencyProperty.Register(
            "IsPGEDownstreamActive",
            typeof(bool),
            typeof(PGE_Tracing_Control),
            new PropertyMetadata(OnIsPGEDownstreamActiveChanged));

        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActivePGEUpstreamProperty = DependencyProperty.Register(
            "IsPGEUpstreamActive",
            typeof(bool),
            typeof(PGE_Tracing_Control),
            new PropertyMetadata(OnIsPGEUpstreamActiveChanged));

        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActivePGEProtectiveUpstreamProperty = DependencyProperty.Register(
            "IsPGEProtectiveUpstreamActive",
            typeof(bool),
            typeof(PGE_Tracing_Control),
            new PropertyMetadata(OnIsPGEProtectiveUpstreamActiveChanged));

        /// <summary>
        /// Gets the identifier for the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActivePGEProtectiveDownstreamProperty = DependencyProperty.Register(
            "IsPGEProtectiveDownstreamActive",
            typeof(bool),
            typeof(PGE_Tracing_Control),
            new PropertyMetadata(OnIsPGEProtectiveDownstreamActiveChanged));

        public static readonly DependencyProperty IsActivePGEUndergroundDownstreamProperty = DependencyProperty.Register(
            "IsPGEUndergroundDownstreamActive",
            typeof(bool),
            typeof(PGE_Tracing_Control),
            new PropertyMetadata(OnIsPGEUndergroundDownstreamActiveChanged));

        public static readonly DependencyProperty IsActivePGEUndergroundUpstreamProperty = DependencyProperty.Register(
            "IsPGEUndergroundUpstreamActive",
            typeof(bool),
            typeof(PGE_Tracing_Control),
            new PropertyMetadata(OnIsPGEUndergroundUpstreamActiveChanged));

        private static void OnIsPGEUndergroundUpstreamActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PGE_Tracing_Control)d;
            isPGEUndergroundUpstreamCurrentlyActive = (bool)e.NewValue;
            if (!isPGEUndergroundUpstreamCurrentlyActive)
            {
                if (control.Map == null) return;
                control.Map.MouseClick -= control.MapControl_MouseClick;

                if ((!isPGEUpstreamCurrentlyActive && !isPGEDownstreamCurrentlyActive && !isPGEProtectiveDownstreamCurrentlyActive
                    && !isPGEProtectiveUpstreamCurrentlyActive && !isPGEUndergroundDownstreamCurrentlyActive))
                {
                    string currentMapCursor = CursorSet.GetID(control.Map);
                    if (currentMapCursor.Contains("flag_cursor.png") == true)
                    {
                        CursorSet.SetID(control.Map, "Arrow");
                    }
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;

                control.Map.MouseClick += control.MapControl_MouseClick;
                CursorSet.SetID(control.Map, CursorUri);
            }
        }

        private static void OnIsPGEUndergroundDownstreamActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PGE_Tracing_Control)d;
            isPGEUndergroundDownstreamCurrentlyActive = (bool)e.NewValue;
            if (!isPGEUndergroundDownstreamCurrentlyActive)
            {
                if (control.Map == null) return;
                control.Map.MouseClick -= control.MapControl_MouseClick;

                if ((!isPGEUpstreamCurrentlyActive && !isPGEDownstreamCurrentlyActive && !isPGEProtectiveDownstreamCurrentlyActive
                    && !isPGEProtectiveUpstreamCurrentlyActive && !isPGEUndergroundUpstreamCurrentlyActive))
                {
                    string currentMapCursor = CursorSet.GetID(control.Map);
                    if (currentMapCursor.Contains("flag_cursor.png") == true)
                    {
                        CursorSet.SetID(control.Map, "Arrow");
                    }
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;

                control.Map.MouseClick += control.MapControl_MouseClick;
                CursorSet.SetID(control.Map, CursorUri);
            }
        }

        private static void OnIsPGEUpstreamActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PGE_Tracing_Control)d;
            isPGEUpstreamCurrentlyActive = (bool)e.NewValue;
            if (!isPGEUpstreamCurrentlyActive)
            {
                if (control.Map == null) return;
                control.Map.MouseClick -= control.MapControl_MouseClick;

                if ((!isPGEUpstreamCurrentlyActive && !isPGEDownstreamCurrentlyActive && !isPGEProtectiveDownstreamCurrentlyActive
                    && !isPGEProtectiveUpstreamCurrentlyActive && !isPGEUndergroundDownstreamCurrentlyActive && !isPGEUndergroundUpstreamCurrentlyActive))
                {
                    string currentMapCursor = CursorSet.GetID(control.Map);
                    if (currentMapCursor.Contains("flag_cursor.png") == true)
                    {
                        CursorSet.SetID(control.Map, "Arrow");
                    }
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;

                control.Map.MouseClick += control.MapControl_MouseClick;
                CursorSet.SetID(control.Map, CursorUri);
            }
        }

        private static void OnIsPGEDownstreamActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PGE_Tracing_Control)d;
            isPGEDownstreamCurrentlyActive = (bool)e.NewValue;
            if (!isPGEDownstreamCurrentlyActive)
            {
                if (control.Map == null) return;
                control.Map.MouseClick -= control.MapControl_MouseClick;

                if ((!isPGEUpstreamCurrentlyActive && !isPGEDownstreamCurrentlyActive && !isPGEProtectiveDownstreamCurrentlyActive
                    && !isPGEProtectiveUpstreamCurrentlyActive && !isPGEUndergroundDownstreamCurrentlyActive && !isPGEUndergroundUpstreamCurrentlyActive))
                {
                    string currentMapCursor = CursorSet.GetID(control.Map);
                    if (currentMapCursor.Contains("flag_cursor.png") == true)
                    {
                        CursorSet.SetID(control.Map, "Arrow");
                    }
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;

                control.Map.MouseClick += control.MapControl_MouseClick;
                CursorSet.SetID(control.Map, CursorUri);
            }
        }

        private static void OnIsPGEProtectiveDownstreamActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PGE_Tracing_Control)d;
            isPGEProtectiveDownstreamCurrentlyActive = (bool)e.NewValue;
            if (!isPGEProtectiveDownstreamCurrentlyActive)
            {
                if (control.Map == null) return;
                control.Map.MouseClick -= control.MapControl_MouseClick;

                if ((!isPGEUpstreamCurrentlyActive && !isPGEDownstreamCurrentlyActive && !isPGEProtectiveDownstreamCurrentlyActive
                    && !isPGEProtectiveUpstreamCurrentlyActive && !isPGEUndergroundDownstreamCurrentlyActive && !isPGEUndergroundUpstreamCurrentlyActive))
                {
                    string currentMapCursor = CursorSet.GetID(control.Map);
                    if (currentMapCursor.Contains("flag_cursor.png") == true)
                    {
                        CursorSet.SetID(control.Map, "Arrow");
                    }
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;

                control.Map.MouseClick += control.MapControl_MouseClick;
                CursorSet.SetID(control.Map, CursorUri);
            }
        }

        private static void OnIsPGEProtectiveUpstreamActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (PGE_Tracing_Control)d;
            isPGEProtectiveUpstreamCurrentlyActive = (bool)e.NewValue;
            if (!isPGEProtectiveUpstreamCurrentlyActive)
            {
                if (control.Map == null) return;
                control.Map.MouseClick -= control.MapControl_MouseClick;

                if ((!isPGEUpstreamCurrentlyActive && !isPGEDownstreamCurrentlyActive && !isPGEProtectiveDownstreamCurrentlyActive
                    && !isPGEProtectiveUpstreamCurrentlyActive && !isPGEUndergroundDownstreamCurrentlyActive && !isPGEUndergroundUpstreamCurrentlyActive))
                {
                    string currentMapCursor = CursorSet.GetID(control.Map);
                    if (currentMapCursor.Contains("flag_cursor.png") == true)
                    {
                        CursorSet.SetID(control.Map, "Arrow");
                    }
                }
            }
            else
            {
                EventAggregator.GetEvent<ControlActivatedEvent>().Publish(control);
                if (control.Map == null) return;

                control.Map.MouseClick += control.MapControl_MouseClick;
                CursorSet.SetID(control.Map, CursorUri);
            }
        }

        #endregion

        bool mapExtentChangeHandlerInitialized = false;
        private void AttachMapExtentChangedHandler()
        {
            if (!mapExtentChangeHandlerInitialized)
            {
                Map.ExtentChanged += new EventHandler<ExtentEventArgs>(Map_ExtentChanged);
                mapExtentChangeHandlerInitialized = true;
            }
        }

        private Object mapExtentChangedLock = new Object();
        private bool extentChanged = false;
        void Map_ExtentChanged(object sender, ExtentEventArgs e)
        {
            lock (mapExtentChangedLock)
            {
                extentChanged = true;
            }

            lock (GraphicsDrawingLock)
            {
                lock (mapExtentChangedLock)
                {
                    extentChanged = false;
                }

                //If the extent has changed return and let the next execution of the method redraw the appropriate graphics
                CancelCurrentDrawTasks();

                DrawTraceGraphics();
            }
        }

        private bool MapCollectionEventRegistered = false;
        private void MapControl_MouseClick(object sender, Map.MouseEventArgs e)
        {
            if (e == null) return;
            if (e.MapPoint == null) return;

            if (tracingHelper == null)
            {
                tracingHelper = new TracingHelper(this);
            }

            tracingHelper.TraceFeederFedFeeders = FeederFedToggle;
            tracingHelper.StopCurrentAsyncTasks();

            //Turn off the toggle for viewing results and set our results to nothing
            TraceResultsToggle.IsChecked = false;

            GraphicsLayer pgeTraceGraphicsLayer = getTracingGraphicsLayer();

            //Reset the trace graphics as we are drawing a new set
            pgeTraceGraphicsLayer.Graphics.Clear();

            //Cache the current tracing settings
            CacheTracingSettings();

            //Wire a map event to watch for layer changes so we can clear trace results.
            if (!MapCollectionEventRegistered)
            {
                foreach (Layer layer in Map.Layers)
                {
                    layer.PropertyChanged += new PropertyChangedEventHandler(layer_PropertyChanged);
                }
            }

            CallMethodsTraceResults = true;

            List<int> protectiveDeviceClassIDs = new List<int>();
            if (IsPGEProtectiveDownstreamActive || IsPGEProtectiveUpstreamActive)
            {
                foreach (KeyValuePair<string, int> protDevice in TracingHelper.PGEProtectiveDevicesByClassID)
                {
                    protectiveDeviceClassIDs.Add(protDevice.Value);
                }
                tracingHelper.GetTraceResults(this.Map, e.MapPoint, protectiveDeviceClassIDs, (IsPGEProtectiveUpstreamActive || IsPGEUpstreamActive), false,
                    TraceAPhase, TraceBPhase, TraceCPhase);
            }
            else if (IsPGEUndergroundDownstreamActive || IsPGEUndergroundUpstreamActive)
            {
                tracingHelper.GetTraceResults(this.Map, e.MapPoint, IsPGEUndergroundUpstreamActive, true,
                    TraceAPhase, TraceBPhase, TraceCPhase);
            }
            else
            {
                tracingHelper.GetTraceResults(this.Map, e.MapPoint, (IsPGEProtectiveUpstreamActive || IsPGEUpstreamActive), false,
                    TraceAPhase, TraceBPhase, TraceCPhase);
            }
        }

        private void CacheTracingSettings()
        {
            try
            {
                //Cache the current flash buffer color settings
                System.Windows.Media.Color color = FlashBufferColor.Color;
                SettingHelper.WriteSetting("PGE_Tracing", "FlashBufferColor", "Value", ((color.R * 0x10000) + (color.G * 0x100)) + color.B);

                SettingHelper.WriteSetting("PGE_Tracing", "TraceAPhase", "Value", TraceAPhase);
                SettingHelper.WriteSetting("PGE_Tracing", "TraceBPhase", "Value", TraceBPhase);
                SettingHelper.WriteSetting("PGE_Tracing", "TraceCPhase", "Value", TraceCPhase);
            }
            catch (Exception e)
            {
                //Failed to cache flash buffer color setting
            }

            try
            {
                SettingHelper.WriteSetting("PGE_Tracing", "TraceBufferSwitch", "Value", AttributesControl.TraceBufferSwitch);
            }
            catch { }

            try
            {
                SettingHelper.WriteSetting("PGE_Tracing", "TraceBufferSize", "Value", AttributesControl.TraceBufferSize);
            }
            catch { }
        }

        void layer_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.ToUpper() == "VISIBLE")
            {
                //Something is no longer visible. Let's check the map layers and verify the current tracing URL is
                //still visible.  If not we can clear it.
                foreach (Layer layer in Map.Layers)
                {
                    if (layer is ArcGISDynamicMapServiceLayer)
                    {
                        ArcGISDynamicMapServiceLayer dynamicLayer = layer as ArcGISDynamicMapServiceLayer;
                        if (dynamicLayer.Url == tracingHelper.currentIdentifyUrl)
                        {
                            if (!dynamicLayer.Visible)
                            {
                                ClearCurrentResults();
                                LayersToIncludeStackPanel.Children.Clear();
                                origCheckBoxes = null;
                            }
                        }
                    }
                }
            }
        }

        private static Object PGEGraphicsLock = new Object();
        private void AddGraphics(IEnumerable<Graphic> graphicsToAdd)
        {
            lock (PGEGraphicsLock)
            {
                GraphicsLayer pgeTraceGraphics = getTracingGraphicsLayer();
                pgeTraceGraphics.Graphics.AddRange(graphicsToAdd);
            }
        }

        private void AddGraphic(Graphic graphicToAdd)
        {
            lock (PGEGraphicsLock)
            {
                GraphicsLayer pgeTraceGraphics = getTracingGraphicsLayer();
                pgeTraceGraphics.Graphics.Add(graphicToAdd);
            }
        }

        private void TraceUpstream_Click(object sender, RoutedEventArgs e)
        {
            IsPGEUpstreamActive = true;
        }

        private void TraceDownstream_Click(object sender, RoutedEventArgs e)
        {
            IsPGEDownstreamActive = true;
        }

        private void TraceProtectiveDownstream_Click(object sender, RoutedEventArgs e)
        {
            IsPGEProtectiveDownstreamActive = true;
        }

        private void TraceProtectiveUpstream_Click(object sender, RoutedEventArgs e)
        {
            IsPGEProtectiveUpstreamActive = true;
        }

        private void TraceUndergroundDownstream_Click(object sender, RoutedEventArgs e)
        {
            IsPGEUndergroundDownstreamActive = true;
        }

        private void TraceUndergroundUpstream_Click(object sender, RoutedEventArgs e)
        {
            IsPGEUndergroundUpstreamActive = true;
        }

        private void PGEClearResultsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearCurrentResults();
            LayersToIncludeStackPanel.Children.Clear();
            origCheckBoxes = null;
        }

        
        //ME Q4 2019 Release DA# 190806
        private string getTraceCustomDisplayField(int classID)
        {
            if (TracingHelper.PGETraceResultCustomDisplayField.Keys.Any(k => Convert.ToInt16(k.ToString().Split('-')[0]) == classID))
            {
                string displayFieldKey = TracingHelper.PGETraceResultCustomDisplayField.Keys.Where(k => Convert.ToInt16(k.ToString().Split('-')[0]) == classID).FirstOrDefault();
                if (displayFieldKey != null)
                {
                    string customDisplayFieldName = displayFieldKey.Split('-')[1];
                    if (customDisplayFieldName.Length > 0)
                        return customDisplayFieldName;
                }
            }
            return null;
        }

    }
}
