using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Miner.Geodatabase.Integration;
using System.Data;
using PGE.Interface.Integration.DMS.Common;
using Miner.Geodatabase.Integration.Electric;
using Miner.Interop;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geometry;
using PGE.Common.Delivery.Diagnostics;
using PGE.Common.Delivery.Framework.Exceptions;
using PGE.Interface.Integration.DMS.Manager;

namespace PGE.Interface.Integration.DMS
{
    /// <summary>
    /// This class contains many common functions used by many other classes
    /// </summary>
    public class Utilities
    {
        private static Log4NetLogger _log4 = new Log4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "ED50.log4net.config");
        /// <summary>
        /// Looks up the subtype and devicegrouptype codes and returns the description 
        /// </summary>
        /// <param name="devgroup">The device group object</param>
        /// <returns>The description of the devicegrouptype</returns>
        public static string GetDeviceGroupType(ObjectInfo devgroup)
        {
            string subtype = devgroup.Fields["SUBTYPECD"].FieldValue.ToString();
            string devtype = devgroup.Fields["DEVICEGROUPTYPE"].FieldValue.ToString();
            switch (subtype)
            {
                case "1":
                    return DomainManager.Instance.GetValue("Device Group Type - Overhead", devtype);
                case "2":
                    return DomainManager.Instance.GetValue("Device Group Type - Subsurface", devtype);
                case "3":
                    return DomainManager.Instance.GetValue("Device Group Type - Padmount", devtype);
            }
            return null;
        }
        /// <summary>
        /// Looks up the controllertype and subtype codes and returns the description
        /// </summary>
        /// <param name="info">The controller object</param>
        /// <returns>The description of the controllertype</returns>
        public static string GetControllerType(ObjectInfo info, ControlTable controlTable)
        {
            ObjectInfo controller = Utilities.getRelatedObject(info, FCID.Value[FCID.Controller]);
            if (controller != null)
            {
                string subtype = controller.Fields["SUBTYPECD"].FieldValue.ToString();
                string globalID = controller.Fields["DEVICEGUID"].FieldValue.ToString();

                return controlTable.GetControllerType(subtype, globalID);

                /*
                //For Capacitor, Regulator, Recloser, Interrupter, and Sectionalizer the field is "Control_Type" from settings tables.  For switch use "Control_Unit_Type"
                string contype = controller.Fields["CONTROLLERTYPE"].FieldValue.ToString();
                switch (subtype)
                {
                    case "1":
                        return DomainManager.Instance.GetValue("CapacitorControlType", contype);
                    case "2":
                        return DomainManager.Instance.GetValue("RegulatorControlType", contype);
                    case "3":
                        return DomainManager.Instance.GetValue("RecloserControlType", contype);
                    case "4":
                        return DomainManager.Instance.GetValue("InterrupterControlType", contype);
                    case "5":
                        return DomainManager.Instance.GetValue("SectionalizerControlType", contype);
                    case "6":
                        return DomainManager.Instance.GetValue("SwitchControlType", contype);
                }
                */
            }
            return null;
        }
        /// <summary>
        /// Find the angle of the line
        /// </summary>
        /// <param name="x1">Start X</param>
        /// <param name="y1">Start Y</param>
        /// <param name="x2">End X</param>
        /// <param name="y2">End Y</param>
        /// <returns>The angle in radians</returns>
        [Obsolete]
        public static double CalculateAngle(double x1, double y1, double x2, double y2)
        {
            double output = 0.0;

            double xDiff = x2 - x1;
            double yDiff = y2 - y1;
            
            // Calculate the angle 
            if (xDiff == 0.0)
            {
                if (xDiff == 0.0)
                {
                    output = 0.0;
                }
                else if (yDiff > 0.0)
                {
                    output = System.Math.PI / 2.0;
                }
                else
                {
                    output = System.Math.PI * 3.0 / 2.0;
                }
            }
            else if (yDiff == 0.0)
            {
                if (xDiff > 0.0)
                {
                    output = 0.0;
                }

                else
                {
                    output = System.Math.PI;
                }
            }
            else
            {
                if (xDiff < 0.0)
                {
                    output = System.Math.Atan(yDiff / xDiff) + System.Math.PI;
                }
                else if (yDiff < 0.0)
                {
                    output = System.Math.Atan(yDiff / xDiff) + (2 * System.Math.PI);
                }
                else
                {
                    output = System.Math.Atan(yDiff / xDiff);
                }
            }

            // Convert to degrees 
            //output = output * 180 / System.Math.PI; 
            
            return output;


        }
        /// <summary>
        /// Move a point a configurable distance up a line. Was originally used to offset devices, but DMS didn't like this.
        /// </summary>
        /// <param name="x1">Start X</param>
        /// <param name="y1">Start Y</param>
        /// <param name="x2">End X</param>
        /// <param name="y2">End Y</param>
        /// <returns>New point</returns>
        [Obsolete]
        public static IPoint MovePoint(double x1, double y1, double x2, double y2)
        {
            double newX = 0.0;
            double newY = 0.0;
            //the distance to move the point
            int distance = Configuration.getIntSetting("OffsetDistance", 2);
            double max = GetSquaredDistanceBetweenPoints(x1, y1, x2, y2);
            if (max < (distance * distance))//if the other point is too close then just use the middle
            {
                newX = (x1 - x2) / 2;
                newY = (y1 - y2) / 2;
            }
            else
            {
                double angle = CalculateAngle(x1, y1, x2, y2);
                newX = distance * Math.Cos(angle) + x1;
                newY = distance * Math.Sin(angle) + y1;
            }
            ESRI.ArcGIS.Geometry.IPoint pPoint = new ESRI.ArcGIS.Geometry.PointClass();
            pPoint.X = newX;
            pPoint.Y = newY;
            return pPoint;
        }
        /// <summary>
        /// Distance squared between two points. If comparing a lot of distances, not taking the square root saves time.
        /// </summary>
        /// <param name="x1">Start X</param>
        /// <param name="y1">Start Y</param>
        /// <param name="x2">End X</param>
        /// <param name="y2">End Y</param>
        /// <returns>The squared distance</returns>
        public static double GetSquaredDistanceBetweenPoints(double x1, double y1, double x2, double y2)
        {
            double a = x1 - x2;
            double b = y1 - y2;
            double distance = a * a + b * b;
            return distance;
        }
        /// <summary>
        /// Get the distance between two points
        /// </summary>
        /// <param name="x1">Start X</param>
        /// <param name="y1">Start Y</param>
        /// <param name="x2">End X</param>
        /// <param name="y2">End Y</param>
        /// <returns>The distance</returns>
        public static double GetDistanceBetweenPoints(double x1, double y1, double x2, double y2)
        {
            double distance = Math.Sqrt(GetSquaredDistanceBetweenPoints(x1, y1, x2, y2));
            return distance;
        }
        private static ISpatialReference _spatial;
        /// <summary>
        /// Returns a new point with WGS lat/lon coordinates
        /// </summary>
        /// <param name="x">The X coordinate</param>
        /// <param name="y">The Y coordinate</param>
        /// <returns>A point with the reprojected coordinates</returns>
        public static IPoint ProjectPoint(double x, double y)
        {

            int geoType = (int)ESRI.ArcGIS.Geometry.esriSRGeoCSType.esriSRGeoCS_WGS1984;

            ESRI.ArcGIS.Geometry.ISpatialReferenceFactory pSRF = new ESRI.ArcGIS.Geometry.SpatialReferenceEnvironmentClass();

            ESRI.ArcGIS.Geometry.IGeographicCoordinateSystem m_GeographicCoordinateSystem;

            m_GeographicCoordinateSystem = pSRF.CreateGeographicCoordinateSystem(geoType);

            if (_spatial == null)
            {
                _spatial = pSRF.CreateESRISpatialReferenceFromPRJFile("spatial.prj");
            }

            ESRI.ArcGIS.Geometry.IPoint pPoint = new ESRI.ArcGIS.Geometry.PointClass();
            pPoint.X = x;
            pPoint.Y = y;
            pPoint.SpatialReference = _spatial;
            pPoint.Project(m_GeographicCoordinateSystem);

            return pPoint;
        }
        /// <summary>
        /// Find the next upstream vertex on the edge so we can form a straight line
        /// </summary>
        /// <param name="edge">The edge to look at</param>
        /// <returns>The point of the vertex directly upstream from the downstream end</returns>
        [Obsolete]
        public static GeoPoint GetNextUpstreamVertex(EdgeInfo edge)
        {
            if (edge.DigitizedDirection == EdgeOrientation.Reverse)
            {
                _log4.Debug("Edge " + edge + " is reversed.");
                return edge.Line[1];
            }
            return edge.Line[edge.Line.Count - 2];
        }
        /// <summary>
        /// Find the ID of an upstream line. If more than one returns the first
        /// </summary>
        /// <param name="junct">The device to check</param>
        /// <returns>The unique ID of the upstream line or 0 if none found</returns>
        public static double GetUpstreamLineID(JunctionFeatureInfo junct)
        {
            EdgeInfo e = GetUpstreamLine(junct);
            if (e != null)
            {
                return Utilities.getID(e);
            }

            return 0;
        }
        /// <summary>
        /// Find the edge upstream of a junction. If more than one returns the first
        /// </summary>
        /// <param name="junct">The device to check</param>
        /// <returns>The upstream edge feature or null if none found</returns>
        public static EdgeInfo GetUpstreamLine(JunctionFeatureInfo junct)
        {
            foreach (EdgeInfo e in junct.Junction.AdjacentEdges.Values)
            {
                if (e.ToJunction.ObjectKey.Equals(junct.Junction.ObjectKey))
                {
                    return e;
                }
            }
            return null;
        }
        /// <summary>
        /// Check if a feature is deactivated, i.e. its DMS state is 0
        /// </summary>
        /// <param name="info">The feature to check</param>
        /// <returns>True if the feature is deactivated/idle</returns>
        public static bool IsDeactivated(FeatureInfo info)
        {
            short state = GetState(info);
            return (state == 0);
        }
        /// <summary>
        /// Get the DMS state of a junction based on the edges it is connected to. If the junction is connected to at least
        /// one edge that is not deactivated, the junction is needed for data continuity.
        /// </summary>
        /// <param name="junct">The junction to check</param>
        /// <returns>The DMS state of the junction</returns>
        public static short GetJunctionState(JunctionInfo junct)
        {
            short output = 1;
            //need to check the status of the connected lines
            foreach (EdgeInfo edge in junct.AdjacentEdges.Values)
            {
                ObjectKey key = edge.ObjectKey;
                if (CADOPS.LineState.ContainsKey(key))
                {
                    output = CADOPS.LineState[key];
                    if (output == 1)//if any line is normal this junction needs to be normal
                    {
                        break;
                    }
                }
                else
                {
                    //_log4.Debug("Line " + key + " not found.");
                }
                int id = edge.EID;
            }
            return output;
        }
        /// <summary>
        /// Get the DMS state based on the GIS construction status. Generic junctions will look at the attached edges to get state.
        /// If the state is unknown, it will return normal, 1. The state will only be unknown if the data model is changed to add new statuses.
        /// </summary>
        /// <param name="info">The feature to get the state of</param>
        /// <returns>DMS state: 2=planned addition, 3=planned deletion, 1=normal, 0=idle(do not send to DMS)</returns>
        public static short GetState(ObjectInfo info)
        {
            object val = null;
            if (info.ObjectClassID == FCID.Value[FCID.Junction] || info.ObjectClassID == FCID.Value[FCID.SUBJunction])
            {
                JunctionInfo junct = ((JunctionFeatureInfo)info).Junction;
                return GetJunctionState(junct); //junctions don't have a status
            }
            val = GetFieldValue(info, "STATUS");
            if (val != null)
            {
                short test;
                if (val is short)
                {
                    test = (short)val;
                }
                else
                {
                    test = Convert.ToInt16(val);
                }
                switch (test)
                {
                    case 0:     //Proposed - Install
                        return 2;   //planned addition
                    case 2:     //Proposed - Remove
                    case 3:     //Proposed - Deactivated
                        return 3;   //planned deletion
                    case 1:     //Proposed - Change
                    case 5:     //In Service  
                    case 50:	//In Service - PTTD
                    case 60:	//Not Found
                    case 70:	//Not Found - PTTD
                        return 1; //normal
                    case 30:	//Idle
                    case 20:	//Removed                    
                    case 10:	//Deactivated
                        return 0;
                }
            }
            return 1;
        }
        /// <summary>
        /// Determines where a given object is currently flagged as idle
        /// </summary>
        /// <param name="info">The feature to get the state of</param>
        /// <returns>Returns true if current state = 30 (idle)</returns>
        public static bool IsIdle(ObjectInfo info)
        {
            object val = null;
            if (info.ObjectClassID == FCID.Value[FCID.Junction] || info.ObjectClassID == FCID.Value[FCID.SUBJunction])
            {
                return false;
            }
            val = GetFieldValue(info, "STATUS");
            if (val != null)
            {
                short test;
                if (val is short)
                {
                    test = (short)val;
                }
                else
                {
                    test = Convert.ToInt16(val);
                }
                if (test == 30) { return true; }
            }
            return false;
        }
        /// <summary>
        /// Look up the localofficeid description using the domain manager
        /// </summary>
        /// RBAE - 11/13/13 - not needed
        /// <param name="info">The feature to get the value for</param>
        /// <returns>The description of the localofficeid</returns>
        //public static int GetDistrict(FeatureInfo info)
        //{
        //    object val = GetFieldValue(info, "LOCALOFFICEID");
        //    if (val != null)
        //    {
        //        int? map = DomainManager.Instance.GetIntValue("LocalOffice", val.ToString());
        //        if (map != null)
        //        {
        //            return (int)map;
        //        }
        //    }
        //    return -1;
        //}
        /// <summary>
        /// Gets the value from the field. Returns null instead of DBNull.
        /// Logs an error if the field is not present.
        /// </summary>
        /// <param name="info">The object to get the value from</param>
        /// <param name="field">The name of the field on the object</param>
        /// <returns>The value in the field or null if there is no value</returns>
        public static object GetFieldValue(ObjectInfo info, string field)
        {
            if (info.Fields.ContainsKey(field))
            {
                object val = info.Fields[field].FieldValue;
                if (val is DBNull)
                {
                    return null;
                }
                return val;
            }
            else
            {
                _log4.Error(info.TableName + " does not contain field " + field);
                //for testing just log the error, change this to throw exception later
                //throw new InvalidConfigurationException("Field " + field + " not found on " + info.TableName);
            }
            return null;
        }
        /// <summary>
        /// Get the value from the object and converts it from the code to the description using the domain manager
        /// </summary>
        /// <param name="info">The object to get the value from</param>
        /// <param name="field">The name of the field on the object</param>
        /// <param name="domain">The name of the domain the field uses</param>
        /// <returns>The converted value or DBNull if there was no value</returns>
        public static object GetDBFieldValue(ObjectInfo info, string field, string domain)
        {
            DomainManager dm = DomainManager.Instance;
            object obj = Utilities.GetDBFieldValue(info, field);
            if (!(obj is DBNull))
            {
                obj = dm.GetValue(domain, obj.ToString());
            }
            return obj;
        }
        /// <summary>
        /// Get a field value from a network adapter traced object
        /// </summary>
        /// <param name="info">The object to get the value from</param>
        /// <param name="field">he name of the field on the object</param>
        /// <returns>The field value or DBNull if there was no value</returns>
        public static object GetDBFieldValue(ObjectInfo info, string field)
        {
            if (info.Fields.ContainsKey(field))
            {
                object value = info.Fields[field].FieldValue;
                if (value == null)
                {
                    value = DBNull.Value;
                }
                return value;
            }
            else
            {
                _log4.Error(info.TableName + " does not contain field " + field);
                //for testing just log the error, change this to throw exception later
                //throw new InvalidConfigurationException("Field " + field + " not found on " + info.TableName);
            }
            return DBNull.Value;
        }
        /// <summary>
        /// Check the normal status of each phase to see if the junction is normally open. If any phase is closed, the 
        /// junction is not normally open
        /// </summary>
        /// <param name="junct">The junction to check</param>
        /// <returns>True if the junction is normally open</returns>
        public static bool IsNormallyOpen(ElectricJunction junct)
        {
            return (junct.NormalStatusA == EnumNormalStatus.NO && junct.NormalStatusB == EnumNormalStatus.NO && junct.NormalStatusC == EnumNormalStatus.NO);
        }

        /// <summary>
        /// Get the operational phase code from a junction feature
        /// </summary>
        /// <param name="info">The junction feature</param>
        /// <returns>The DMS phase code</returns>
        public static int getOpPhaseValue(JunctionFeatureInfo info)
        {
            return getOpPhaseValue((ElectricJunction)info.Junction);
        }
        /// <summary>
        /// Get the energized phase code from a junction feature
        /// </summary>
        /// <param name="info">The junction feature</param>
        /// <returns>The DMS phase code</returns>
        public static int getPhaseValue(JunctionFeatureInfo info)
        {
            return getPhaseValue((ElectricJunction)info.Junction);
        }
        /// <summary>
        /// Get the operational phase code from an electric junction
        /// </summary>
        /// <param name="info">The electric junction</param>
        /// <returns>The DMS phase code</returns>
        public static int getOpPhaseValue(ElectricJunction info)
        {
            return getPhaseValue(info.OperationalPhases);
        }
        /// <summary>
        /// Get the energized phase code from an electric junction
        /// </summary>
        /// <param name="info">The electric junction</param>
        /// <returns>The DMS phase code</returns>
        public static int getPhaseValue(ElectricJunction info)
        {
            return getPhaseValue(info.EnergizedPhases);
        }
        /// <summary>
        /// Get the energized phase code from an edge feature
        /// </summary>
        /// <param name="info">The edge feature</param>
        /// <returns>The DMS phase code</returns>
        public static int getOpPhaseValue(EdgeInfo info)
        {
            return getPhaseValue(((ElectricEdge)info).OperationalPhases);
        }
        /// <summary>
        /// Get the operational phase code from an edge feature
        /// </summary>
        /// <param name="info">The edge feature</param>
        /// <returns>The DMS phase code</returns>
        public static int getPhaseValue(EdgeInfo info)
        {
            return getPhaseValue(((ElectricEdge)info).EnergizedPhases);
        }
        /// <summary>
        /// Get the DMS phase code from the GIS phase
        /// </summary>
        /// <param name="phase">The GIS phase</param>
        /// <returns>The DMS phase code</returns>
        public static int getPhaseValue(SetOfPhases? phase)
        {
            if (phase == null)
            {
                return 0;
            }
            return getPhaseValue((int)phase);
        }
        /// <summary>
        /// Get the DMS phase code from the GIS phase
        /// </summary>
        /// <param name="phase">The GIS phase</param>
        /// <returns>The DMS phase code</returns>
        public static int getPhaseValue(int  phase)
        {
            switch (phase)
            {
                case (0): return 0; // none
                case (1): return 100; // A
                case (2): return 10; // B
                case (3): return 110; // AB
                case (4): return 1; // C
                case (5): return 101; // AC
                case (6): return 11; // BC
                case (7): return 111; // B
            }

            return 0;
        }
        /// <summary>
        /// Returns the first related object otherwise NULL.
        /// </summary>
        /// <param name="feat">The feature with related objects</param>
        /// <param name="fcid">The FCID of the related object class</param>
        /// <returns>The first related object otherwise NULL</returns>
        public static ObjectInfo getRelatedObject(ObjectInfo feat, int fcid)
        {
            if (feat.Relationships != null)
            {
                foreach (RelationshipInfo rel in feat.Relationships)
                {
                    foreach (ObjectInfo o in rel.RelatedObjects)
                    {
                        if (o.ObjectClassID == fcid)
                        {
                            return o;
                        }
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// Get all the objects related to a feature
        /// </summary>
        /// <param name="feat">The feature with related objects</param>
        /// <param name="fcid">The FCID of the related object class</param>
        /// <returns>A list of related objects or an empty list if none</returns>
        public static List<ObjectInfo> getRelatedObjects(ObjectInfo feat, int fcid)
        {
            List<ObjectInfo> output = new List<ObjectInfo>();
            if (feat.Relationships != null)
            {
                foreach (RelationshipInfo rel in feat.Relationships)
                {
                    foreach (ObjectInfo o in rel.RelatedObjects)
                    {
                        if (o.ObjectClassID == fcid)
                        {
                            output.Add(o);
                            return output;
                        }
                    }
                }
            }
            return output;
        }
        /// <summary>
        /// Convert coordinate for DMS. This prevents a loss of precision since DMS coordinates are only 10 digits
        /// with no decimal places
        /// </summary>
        /// <param name="value">The GIS coordinate</param>
        /// <returns>The DMS coordinate</returns>
        /// 

        /*Changes for ENOS to SAP migration - DMS ..Start */
        /// <summary>
        /// Get all the objects related to a feature
        /// </summary>
        /// <param name="feat">The feature with related objects</param>
        /// <param name="fcid">The FCID of the related object class</param>
        /// <returns>A list of related objects or an empty list if none</returns>
        public static List<ObjectInfo> getAllRelatedObjects(ObjectInfo feat, int fcid)
        {
            List<ObjectInfo> output = new List<ObjectInfo>();
            if (feat.Relationships != null)
            {
                foreach (RelationshipInfo rel in feat.Relationships)
                {
                    foreach (ObjectInfo o in rel.RelatedObjects)
                    {
                        if (o.ObjectClassID == fcid)
                        {
                            output.Add(o);
                        }
                    }
                }
            }
            return output;
        }
        /*Changes for ENOS to SAP migration - DMS ..End */

        public static int ConvertXY(double value)
        {
            return System.Convert.ToInt32(value * 100);
        }
        /// <summary>
        /// Get the unique DMS ID for a junction
        /// </summary>
        /// <param name="junct">The feature to get the ID for</param>
        /// <returns>The unique DMS ID</returns>
        public static double getID(JunctionInfo junct)
        {
            try
            {
                return getID(junct.ObjectClassID, junct.ObjectID);
            }
            catch (Exception ex)
            {
                string message = "FCID map entry not found for FCID: " + junct.ObjectClassID;
                _log4.Debug(message);

                throw new ApplicationException(message, ex);
            }
        }
        /// <summary>
        /// Get the unique DMS ID for a junction. This is basically the object ID and feature class ID concatenated together.
        /// The feature class ID is always the last two digits
        /// </summary>
        /// <param name="feat">The feature to get the ID for</param>
        /// <returns>The unique DMS ID</returns>
        public static double getID(JunctionFeatureInfo feat)
        {
            if (feat.ObjectClassID == FCID.Value[FCID.SUBStitchPoint])
            {
                return getIDRelate(feat);
            }
            return getID(feat.ObjectClassID, feat.ObjectID);

        }
        /// <summary>
        /// Get the unique ID of the related ED Electric Stichpoint or just return the features unique ID is there is no related ED Electric Stichpoint
        /// </summary>
        /// <param name="feat">The feature to get the ID for</param>
        /// <returns>The unique DMS ID</returns>
        public static double getIDRelate(JunctionFeatureInfo feat)
        {
            int subtype = (int)Utilities.GetFieldValue(feat, "SUBTYPECD");
            if (subtype == 1)
            {
                ObjectInfo EDStitchPoint = Utilities.getRelatedObject(feat, FCID.Value[FCID.StitchPoint]);
                if (EDStitchPoint != null)
                {
                    return getID(EDStitchPoint.ObjectClassID, EDStitchPoint.ObjectID);
                }
            }
            return getID(feat.ObjectClassID, feat.ObjectID);
        }
        /// <summary>
        /// Get the unique DMS ID for an edge. This is basically the object ID, Sub segment ID, and feature class ID concatenated together.
        /// The feature class ID is always the last two digits. The Sub ID is the 3 digits before that. Having more than 1000 segments
        /// in an edge could cause problems.
        /// </summary>
        /// <param name="edge">The edge to get the ID for</param>
        /// <returns>The unique DMS ID</returns>
        public static double getID(EdgeInfo edge)
        {
            if (edge.SubID > 999)
            {
                string error = "Edge " + edge.ObjectClassID + ":" + edge.ObjectID + " has more than 999 segments.";
                _log4.Error(error);
            }
            double oid = edge.ObjectID * 1000 + edge.SubID;
            return getID(edge.ObjectClassID, oid);
        }
        /// <summary>
        /// Get the unique DMS ID for a path element. This was for the original CADOPS PATH table.
        /// </summary>
        /// <param name="fcid">The feature class ID</param>
        /// <param name="oid">The ID of the object</param>
        /// <param name="subid">The ID of the segment of the object</param>
        /// <param name="nid">The ID of the sub segment of the segment</param>
        /// <returns>The unique DMS ID</returns>
        [Obsolete]
        public static double getID(double fcid, double oid, double subid, double nid)
        {
            double noid = oid * 1000 + nid;
            return getID(fcid, noid, subid);
        }
        /// <summary>
        /// Get the unique DMS ID
        /// </summary>
        /// <param name="fcid">The feature class ID</param>
        /// <param name="oid">The ID of the object</param>
        /// <param name="subid">The ID of the segment of the object</param>
        /// <returns>The unique DMS ID</returns>
        public static double getID(double fcid, double oid, double subid)
        {
            double soid = oid * 10 + (uint)subid;
            return getIDEdge(fcid, soid);
        }
        /// <summary>
        /// Gets the unique DMS ID from an object
        /// </summary>
        /// <param name="info">The object to get the ID from</param>
        /// <returns>The unique DMS ID</returns>
        public static double getID(ObjectInfo info)
        {
            return getID(info.ObjectClassID, info.ObjectID);
        }
        /// <summary>
        /// Get the unique DMS ID
        /// </summary>
        /// <param name="fcid">The feature class ID</param>
        /// <param name="oid">The ID of the object</param>
        /// <returns>The unique DMS ID</returns>
        public static double getID(double fcid, double oid)
        {
            double id = oid * 100 + mapFCID(fcid);

            return id;
        }
        /// <summary>
        /// Get the unique DMS ID
        /// </summary>
        /// <param name="fcid">The feature class ID</param>
        /// <param name="oid">The ID of the object</param>
        /// <returns>The unique DMS ID</returns>
        public static double getIDEdge(double fcid, double oid)
        {
            double id = oid * 100 + mapFCID(fcid);

            return id;
        }
        /// <summary>
        /// Look up the static ID for a feature class based on its FCID.
        /// This static ID is used in generating the unique DMS ID
        /// </summary>
        /// <param name="fcid">The FCID of the feature class</param>
        /// <returns>The feature class's static ID</returns>
        public static double mapFCID(double fcid)
        {

            try
            {
                int id = (int)fcid;
                return (double)FCID.Map[id];
            }
            catch
            {
                throw new InvalidConfigurationException("Feature Class ID " + fcid + " not found. Please check the configuration file.");
            }
        }
        private static DataSet _schema;
        /// <summary>
        /// Build a DataSet based on the active staging schema
        /// </summary>
        /// <returns>The DataSet</returns>
        public static DataSet BuildDataSet()
        {
            if (_schema == null)
            {
                _schema = Common.Oracle.GetSchema(Configuration.CadopsConnection, "DMSSTAGING");
                //_schema = new DataSet();
                //_schema.ReadXmlSchema("gis_stg_schema.xml");
            }
            return _schema.Copy();
        }
    }
}
