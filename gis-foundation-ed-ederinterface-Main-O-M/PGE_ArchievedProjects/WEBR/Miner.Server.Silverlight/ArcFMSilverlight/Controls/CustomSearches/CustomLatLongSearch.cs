using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;


using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Tasks;
using ESRI.ArcGIS.Client.Toolkit;

using Miner.Server.Client;
using Miner.Server.Client.Query;
using MinerTask = Miner.Server.Client.Tasks;

using NLog;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using Miner.Server.Client.Toolkit;
using ArcFMSilverlight;

namespace ArcFM.Silverlight.PGE.CustomTools
{


    public class CustomLatLongSearch : SearchItem
    {
        #region public properties        
       
        public string Service { get; set; }        
        public string _searchTitle { get; set; }
        public Envelope  LatLongExtent { get; set; }
        public Envelope XYExtent { get; set; } 
        public ESRI.ArcGIS.Client.Geometry.SpatialReference LocalMapSpatialRef { get; set; }
        public CoordinatePairs Coordinates { get; set; }
        public bool IsWgs { get; set; }
        
        #endregion public properties

        #region private declarations

        private const int WgsWKID = 4326;
        private static Logger logger = LogManager.GetCurrentClassLogger();        

        #endregion private declarations

        #region Constructor

        public CustomLatLongSearch(MinerTask.ILocateTask  locate):base(locate)
        {
            Service = "";
            Coordinates = new CoordinatePairs(); 
        }
        #endregion

        #region public overrides

        /// <summary>
        /// LocateAsync(string query): Configures and Submits LatLong Locate using the user input
        /// </summary>
        /// <param name="query">Input Where Clause</param>
        public override void LocateAsync(string query)
        {           
            IsWgs = false;
            Coordinates.MapCoordinatePoint = null;
            Coordinates.WGSPoint = null;

            ConfigUtility.StatusBar.Text = "Locating.."; 
            
            //CancelAsync();
            //Clear if any previous result is present
            Results.Clear();
            //remove the double quotes from the query string
            query = query.Substring(1, query.Length - 2);

            var coordinateText = query.Trim();
            coordinateText = CleanCoordinateLabels(coordinateText);

            var decimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var seperators = (decimalSeparator == ".") ?
                new[] { ',', ' ', '\n', '\r' } :
                new[] { ' ', '\n', '\r' };
            var coords = coordinateText.Split(seperators, StringSplitOptions.RemoveEmptyEntries);

            //Array.Reverse(coords);

            double xCoord;
            double yCoord;

            PanToPointType mode;
            const int x = 0;
            const int y = 1;

            if (coords.Length != 2)
            {
                ConfigUtility.StatusBar.Text = "Input Coordinates are not valid";
                OnLocateComplete(new ResultEventArgs(null));
                return;
            }

            if (!ValidInput(coords[x], coords[y], out mode))
            {
                ConfigUtility.StatusBar.Text = "Input Coordinates are not valid";
                OnLocateComplete(new ResultEventArgs(null));
                return;
            }

            switch (mode)
            {
                case PanToPointType.DegreesMinutesSeconds:
                    xCoord = ConvertDmsToDecDeg(coords[x]);
                    yCoord = ConvertDmsToDecDeg(coords[y]);
                    break;
                case PanToPointType.DecimalDegrees:
                    xCoord = Convert.ToDouble(coords[y]);
                    yCoord = Convert.ToDouble(coords[x]);
                    IsWgs = true;
                    if(xCoord > LatLongExtent.XMax  || xCoord < LatLongExtent.XMin  || yCoord > LatLongExtent.YMax  || yCoord < LatLongExtent.YMin )
                    {
                        ConfigUtility.StatusBar.Text = "Coordinates out of bound";
                        OnLocateComplete(new ResultEventArgs(null ));
                        return;
                    }
                    break;
                case PanToPointType.EastingNorthing:
                    xCoord = Convert.ToDouble(coords[x]);
                    yCoord = Convert.ToDouble(coords[y]);
                    IsWgs = false;
                    if (xCoord > XYExtent.XMax || xCoord < XYExtent.XMin  || yCoord > XYExtent.YMax  || yCoord < XYExtent.YMin )
                    {
                        ConfigUtility.StatusBar.Text = "Coordinates out of bound";
                        OnLocateComplete(new ResultEventArgs(null));
                        return;
                    }
                    break;
                default:
                    throw new Exception("Invalid Coordinate Mode");
            }

            if (!IsWgs)
            {
                
                var point = new MapPoint(xCoord, yCoord, this.SpatialReference);
                CallbackResult(point);
            }
            else
            {                
                SpatialReference sp = new SpatialReference(WgsWKID);
                var point = new MapPoint(xCoord, yCoord, sp);
                Coordinates.MapCoordinatePoint = point;
                var graphic = new Graphic
                {
                    Geometry = point
                };
                Coordinates.WGSPoint  = point;
                var graphicList = new List<Graphic> { graphic };
                var geometryService = new GeometryService(Service);

                geometryService.Failed += GeometryServiceFailed;
                geometryService.ProjectCompleted += GeometryServiceNavigateProjectCompleted;
                geometryService.ProjectAsync(graphicList, this.SpatialReference, point);
            }
           
        }

        #endregion public overrides

        #region private overrides

        /// <summary>
        /// Converts a degree minute second string to a decimal degree double
        /// </summary>
        /// <param name="dms">the degree minute second string to convert</param>
        /// <returns>the decimal degree</returns>
        private static double ConvertDmsToDecDeg(string dms)
        {
            try
            {
                // Get the positions of the separators.
                int dLoc = dms.IndexOf("d");
                int mLoc = dms.IndexOf("m");
                int sLoc = dms.IndexOf("s");

                // Parse out the values.
                double deg = Convert.ToDouble(dms.Substring(0, dLoc));
                double min = Convert.ToDouble(dms.Substring(dLoc + 1, mLoc - dLoc - 1));
                double sec = Convert.ToDouble(dms.Substring(mLoc + 1, sLoc - mLoc - 1));

                // Create the decimal degree value.
                double value = deg + (min / 60) + (sec / 3600);

                // Set positive od negative based on directional modifier.  West and South are negative, East and North are positive.
                if (dms.ToLower().EndsWith("w") || dms.ToLower().EndsWith("s")) value = (value * (-1));

                return value;

            }
            catch
            {
                return 0.0;
            }
        }

        /// <summary>
        /// Validates coordinate input based on the type of coordinate system.
        /// </summary>
        /// <param name="coordSys">the coordinate systems spatial reference WKID</param>
        /// <param name="xValue">the x axis value</param>
        /// <param name="yValue">the y axis value</param>
        /// <param name="mode">the type of values entered</param>
        /// <returns>true if valid, false if not</returns>
        private static bool ValidInput(string xValue, string yValue, out PanToPointType mode)
        {
            // Decide which rules to use in validation based on coordinate system.

           
           
                if (xValue.Contains("d"))
                {
                    mode = PanToPointType.DegreesMinutesSeconds;

                    // DMS values
                    try
                    {
                        xValue = xValue.ToLower();
                        yValue = yValue.ToLower();

                        // Parse X the value
                        int dLoc = xValue.IndexOf("d");
                        int mLoc = xValue.IndexOf("m");
                        int sLoc = xValue.IndexOf("s");

                        int xDeg = Convert.ToInt32(xValue.Substring(0, dLoc));
                        int xMin = Convert.ToInt32(xValue.Substring(dLoc + 1, mLoc - dLoc - 1));
                        double xSec = Convert.ToDouble(xValue.Substring(mLoc + 1, sLoc - mLoc - 1));

                        // Parse the Y value
                        dLoc = yValue.IndexOf("d");
                        mLoc = yValue.IndexOf("m");
                        sLoc = yValue.IndexOf("s");

                        int yDeg = Convert.ToInt32(yValue.Substring(0, dLoc));
                        int yMin = Convert.ToInt32(yValue.Substring(dLoc + 1, mLoc - dLoc - 1));
                        double ySec = Convert.ToDouble(yValue.Substring(mLoc + 1, sLoc - mLoc - 1));

                        // X degrees can be between 0 and 180, Y degrees can be between 0 and 90
                        if ((xDeg < 0) || (yDeg < 0) || (xDeg > 180) || (yDeg > 90)) return false;

                        // Minutes and seconds can be between 0 and 60
                        if ((xMin < 0) || (yMin < 0) || (xMin > 60) || (yMin > 60)) return false;
                        if ((xSec < 0) || (ySec < 0) || (xSec > 60) || (ySec > 60)) return false;

                        // Can only end in North, South, East, or West
                        return ((xValue.EndsWith("n")) || (xValue.EndsWith("s")) || (xValue.EndsWith("e")) || (xValue.EndsWith("w")));
                    }
                    catch
                    {
                        return false;
                    }
                }

                mode = PanToPointType.DecimalDegrees;
                // Decimal Degree value
                try
                {
                    double y = Convert.ToDouble(xValue);
                    double x = Convert.ToDouble(yValue);

                    if ((x >= -180.0) && (x <= 180.0))
                    {
                        // Y can be +-90
                        return ((y >= -90.0) && (y <= 90.0));
                    }

                    
                }
                catch
                {
                    // Probably failed convert to double.
                    return false;
                }
            

            mode = PanToPointType.EastingNorthing;
            // Easting Northing value
            try
            {
                // Is it a double?
                Convert.ToDouble(xValue);
                Convert.ToDouble(yValue);

                return true;
            }
            catch
            {
                return false;
            }

            return false;
        }
        
        /// <summary>
        /// Cleans any label text found on the passed in string.  Returns only possible coordinate values.
        /// </summary>
        /// <param name="coordinateText">the string to clean</param>
        /// <returns>possible coordinate values</returns>
        private static string CleanCoordinateLabels(string coordinateText)
        {
            // Clean labels
            coordinateText = coordinateText.Replace("x", string.Empty);
            coordinateText = coordinateText.Replace("y", string.Empty);
            coordinateText = coordinateText.Replace("X", string.Empty);
            coordinateText = coordinateText.Replace("Y", string.Empty);
            coordinateText = coordinateText.Replace(":", string.Empty);
            coordinateText = coordinateText.Replace("=", string.Empty);
            coordinateText = coordinateText.Replace("Northing", string.Empty);
            coordinateText = coordinateText.Replace("Easting", string.Empty);
            coordinateText = coordinateText.Replace("Latitude", string.Empty);
            coordinateText = coordinateText.Replace("Longitude", string.Empty);
            coordinateText = coordinateText.Replace("Lat", string.Empty);
            coordinateText = coordinateText.Replace("Lon", string.Empty);

            // Replace DMS symbols with characters for parsing
            coordinateText = coordinateText.Replace("°", "d");
            coordinateText = coordinateText.Replace("'", "m");
            coordinateText = coordinateText.Replace("\"", "s");

            return coordinateText;
        }

        
        private void GeometryServiceFailed(object sender, TaskFailedEventArgs e)
        {
            logger.Error("Coordinates tool geometry task failed! " + e.Error.Message);
            throw new Exception("CoordinatesControl: ProjectionError: " + e.Error.Message);
        }

        private void GeometryServiceProjectCompleted(object sender, GraphicsEventArgs e)
        {
            var wgsPoint = (MapPoint)e.Results[0].Geometry;            
        }

        private void CallbackResult(MapPoint point)
        {
            GraphicPlus graphic = new GraphicPlus();
            graphic.Geometry = point;
            Coordinates.MapCoordinatePoint = point ;
            Miner.Server.Client.Tasks.IResultSet resultSet = new Miner.Server.Client.Tasks.ResultSet();
            resultSet.Features.Add(graphic);
            Results.Add(resultSet);
            OnLocateComplete(new ResultEventArgs(Results));
        }

        private void GeometryServiceNavigateProjectCompleted(object sender, GraphicsEventArgs e)
        {
            try
            {
                var point = (MapPoint)e.Results[0].Geometry;                
                Coordinates.MapCoordinatePoint = point as MapPoint;
                CallbackResult(point);
                

            }
            catch (Exception ex)
            {
                throw new Exception("CoordinatesControl: An error occurred. " + ex.Message);
            }
        }

        #endregion private overrides

    }



    internal enum PanToPointType
    {
        DecimalDegrees = 1,
        EastingNorthing = 2,
        DegreesMinutesSeconds = 3
    };
}
