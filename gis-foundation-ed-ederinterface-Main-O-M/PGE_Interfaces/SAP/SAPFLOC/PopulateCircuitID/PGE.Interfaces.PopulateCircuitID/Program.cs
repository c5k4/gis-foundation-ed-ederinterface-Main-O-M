using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using ESRI.ArcGIS;
using ESRI.ArcGIS.esriSystem;
using Miner.Interop;
using ESRI.ArcGIS.Geodatabase;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Collections;

namespace PGE.Interface.PopulateCircuitID
{
    public class Program
    {
        #region Variables

        private static LicenseInitializer m_AOLicenseInitializer = new PGE.Interface.PopulateCircuitID.LicenseInitializer();
        private static string sCSVFilePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + GlobalVariables.sInputCSVFileName;
        private static string sPath = (String.IsNullOrEmpty(GlobalVariables.sLogFilePath) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) :GlobalVariables.sLogFilePath ) + "\\Logfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static string sExceptionPath = (String.IsNullOrEmpty(GlobalVariables.sLogFilePath) ? System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) : GlobalVariables.sLogFilePath) + "\\ExceptionLogfile_" + DateTime.Now.Ticks.ToString() + ".log";
        private static int RecordNotExst=0;
        private static int RecordExstinPMNotinWIP = 0;
        private static int RecordExstInGIS = 0;
        private static int RecordnotProcessed = 0;
        private static int RecordInCSV = 0;
        private static StreamWriter pSWriter = default(StreamWriter);
        private static IMMAppInitialize arcFMAppInitialize = new MMAppInitializeClass();        

        #endregion Variables

        #region Main Function

        static void Main(string[] args)
        {
            try
            {
                if (File.Exists(GlobalVariables.sTriggerFilePath) == true)
                {
                    WriteLine(DateTime.Now.ToLongTimeString() + " --Trigger File already exist.Please Try Again Later");
                    return;
                }
                else
                {
                    //ESRI License Initializer generated code.
                    WriteLine(DateTime.Now.ToLongTimeString() + " -- Initializing Licence");
                    m_AOLicenseInitializer.InitializeApplication(new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeBasic, esriLicenseProductCode.esriLicenseProductCodeStandard, esriLicenseProductCode.esriLicenseProductCodeAdvanced }, new esriLicenseExtensionCode[] { });
                    mmLicenseStatus licenseStatus = CheckOutLicenses(mmLicensedProductCode.mmLPArcFM);
                    if (RuntimeManager.ActiveRuntime == null)
                        RuntimeManager.BindLicense(ProductCode.Desktop);


                    if (PopulateCircuitID() == true)
                    {
                        WriteLine(DateTime.Now.ToLongTimeString() + " -- Process Completed Successfully");
                    }
                    else
                    {
                        WriteLine(DateTime.Now.ToLongTimeString() + " -- Process not completed successfully, Please check the log file");
                    }
                    //Console.ReadLine();
                }
              
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error : " + ex.Message);
            }
        }
        private static bool  PopulateCircuitID( )
        {
            bool _PopulateCircuitID = false;
            //IFeature pFeat = default(IFeature);           
            IQueryFilter pQFilter = default(IQueryFilter);
            IFeatureClass pWIPPolygonClass = default(IFeatureClass);           
            Dictionary<string,List<string>> DirInstallJobNumbers = new Dictionary<string,List<string>>();
            try
            {
                # region Initialize Variables
                Dictionary<string, string> circuitIDs = new Dictionary<string, string>();
                Dictionary<string, double> circuitIdAndLength = new Dictionary<string, double>();  

                //Creating EDER Connection              
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating EDER Connection  -- " + GlobalVariables.EderConnectionString);
                IWorkspace objEderFW = ArcSdeWorkspaceFromFile(GlobalVariables.EderConnectionString);
                if (objEderFW == null) { return _PopulateCircuitID ; }
                else { WriteLine(DateTime.Now.ToLongTimeString() + " --EDER Connection Establish  -- " + GlobalVariables.EderConnectionString); }

                //Creating WIP Connection
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating WIP Connection " + GlobalVariables.WberConnectionString);
                IWorkspace objWberFW = ArcSdeWorkspaceFromFile(GlobalVariables.WberConnectionString);
                if (objWberFW == null) { return _PopulateCircuitID ; }
                else { WriteLine(DateTime.Now.ToLongTimeString() + " -- WIP Connection Establish  -- " + GlobalVariables.WberConnectionString); }
            
                WriteLine(DateTime.Now.ToLongTimeString() + " -Open Wip Polygon Featureclass  -- " );
                pWIPPolygonClass = ((IFeatureWorkspace)objWberFW).OpenFeatureClass(GlobalVariables.sWIPPolygonFeatureclassName );
                WriteLine(DateTime.Now.ToLongTimeString() + " - Wip Polygon Featureclass Opened Successfully --. ");

                WriteLine(DateTime.Now.ToLongTimeString() + " -Open " + GlobalVariables.sWIPChangesTableName + " Table  -- ");
                ITable pTbleWipChanges = ((IFeatureWorkspace)objEderFW).OpenTable(GlobalVariables.sWIPChangesTableName);
                WriteLine(DateTime.Now.ToLongTimeString() + GlobalVariables.sWIPChangesTableName + " Table Opened Successfully --. ");

                List< string> lstInstallJobNumbers = new List<string>();
                //System.Collections.Generic.Dictionary<string, List<string>> DirInstallJobNumbers` = new Dictionary<string,List<string>>();
                if (GlobalVariables.sRunFromCSV.ToUpper() == "TRUE")
                {
                    //Getting Job Numbers
                    WriteLine(DateTime.Now.ToLongTimeString() + " -- Getting JobNumbers From CSV");
                    DataTable CSVDataTable = GetDataTableFromCSVFile(sCSVFilePath);
                    DirInstallJobNumbers = GetInstallJobNumbersFromCSV(CSVDataTable, pWIPPolygonClass, objWberFW);
                    WriteLine(DateTime.Now.ToLongTimeString() + " -- Total Valid JobNumbers From CSV :- " + DirInstallJobNumbers.Count);
                  
                }
                else
                {
                    //Getting Job Numbers
                    WriteLine(DateTime.Now.ToLongTimeString() + " -- Getting JobNumbers From " + GlobalVariables.sWIPChangesTableName);
                    //lstInstallJobNumbers = GetInstallJobNumbers(objEderFW,pTbleWipChanges );
                    DirInstallJobNumbers = GetInstallJobNumbersfromTable(objEderFW, pTbleWipChanges);
                    WriteLine(DateTime.Now.ToLongTimeString() + " -- Total Valid JobNumbers From  Table :- " + DirInstallJobNumbers.Count);
                   
                }
                # endregion
                
                #region Start Processing the JobNumber
                                                
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Process Started :  Finding CircuitIDs covered by WIP Cloud");
                circuitIDs = ProcessWIPs(DirInstallJobNumbers, objWberFW, objEderFW);               

                #endregion

                #region Generate Output
                
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Creating Output File with " + GlobalVariables.sInstallJobNumberOutputFieldName + " and " + GlobalVariables.sCircuitIDOutputFieldName );
              
                WriteToCSV(circuitIDs);
                CreateTriggerFile();
                #endregion
                if (GlobalVariables.sRunFromCSV.ToUpper() == "FALSE")
                {
                    pTbleWipChanges.DeleteSearchedRows(null);

                }
                else
                {
                    CreateLogSummary();
                }
                _PopulateCircuitID = true;
               
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error : " + ex.Message);
                _PopulateCircuitID = false;
            }
            finally
            {
               
                GC.Collect();                
                if (pQFilter != null) Marshal.FinalReleaseComObject(pQFilter);
                if (pWIPPolygonClass != null) Marshal.FinalReleaseComObject(pWIPPolygonClass);

            }
            return _PopulateCircuitID;
        }

        private static void CreateLogSummary()
        {
            try
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " Total Record in CSV = " + RecordInCSV.ToString()) ;
                WriteLine(DateTime.Now.ToLongTimeString() + " Record Not in GIS = " + RecordNotExst.ToString());
                WriteLine(DateTime.Now.ToLongTimeString() + " Record exist in " + GlobalVariables.sPGEWorkOderTableName + " but not present in the " + GlobalVariables.sWIPPolygonFeatureclassName +  " = " + RecordExstinPMNotinWIP.ToString());
                WriteLine(DateTime.Now.ToLongTimeString() + " Record Present in GIS = " + RecordExstInGIS.ToString());
                WriteLine(DateTime.Now.ToLongTimeString() + " Record Not Processed as CircuitID not found in GIS = " + RecordnotProcessed.ToString());
               

            }
            catch
            { 
            }
        }
       
        #endregion Main Function

        #region  Find the Circuit Covered by WIP Cloud

        private static void FindCircuitCoveredBySingleWipCloud(IGeometry pGeometry, IFeatureClass fcPriOHConductor, IFeatureClass fcPriUGConductor, IFeatureClass fcSecOHConductor, IFeatureClass fcSecUGConductor, out string longestCircuitID, out string longestLength)
        {

            longestCircuitID = string.Empty;
            longestLength = string.Empty;
            try
            {
                Dictionary<string, double> ConductorCircuitidAndLength = new Dictionary<string, double>();
                FindCircuitsCoveredByWipCloud(fcPriOHConductor, pGeometry, ref ConductorCircuitidAndLength); //Find Primary OH Circuits
                FindCircuitsCoveredByWipCloud(fcPriUGConductor, pGeometry, ref ConductorCircuitidAndLength); //Find Primary UG Circuits
                FindCircuitsCoveredByWipCloud(fcSecOHConductor, pGeometry, ref ConductorCircuitidAndLength); //Find Secondary OH Circuits
                FindCircuitsCoveredByWipCloud(fcSecUGConductor, pGeometry, ref ConductorCircuitidAndLength); //Find Secondary UG Circuits 
                longestCircuitID = ConductorCircuitidAndLength.Where(x => x.Value.Equals(ConductorCircuitidAndLength.Max(y => y.Value))).FirstOrDefault().Key;
                longestLength = ConductorCircuitidAndLength.Where(x => x.Value.Equals(ConductorCircuitidAndLength.Max(y => y.Value))).FirstOrDefault().Value.ToString();

                if (string.IsNullOrEmpty(longestCircuitID))
                {
                    //IArea area = (IArea)PFeature.Shape;

                    //If Circuit not found in the WIP Polygon then Find the nearest circuit
                    
                    List<IFeatureClass> lstFeatureClass = new List<IFeatureClass>();
                    lstFeatureClass.Add(fcPriOHConductor);
                    lstFeatureClass.Add(fcPriUGConductor);
                    lstFeatureClass.Add(fcSecOHConductor);
                    lstFeatureClass.Add(fcSecUGConductor);
                    double distance = Convert.ToDouble(ConfigurationManager.AppSettings["MAX_BUFFER_DISTANCE"].ToString());
                    longestCircuitID = FindNearestFeatureFromWIP((pGeometry as IGeometry), lstFeatureClass, distance, string.Empty);
                    
                }
            }
            catch (Exception ex)
            {
              
                WriteLine(DateTime.Now.ToLongTimeString() + "Exception Occurred while finding circuit id " +ex.Message+ex.StackTrace);
                throw ex;
            }
        }

        private static void FindCircuitsCoveredByWipCloud(IFeatureClass fcConductor, IGeometry shape, ref  Dictionary<string, double> conductorcircuitid)
        {
            IFeatureCursor featureCursor =null;
            IFeature feature = null;
            try
            {
                double dlength = 0;
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = shape;
                spatialFilter.GeometryField = fcConductor.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                spatialFilter.SubFields = GlobalVariables.sConductorObjectIdFieldName + "," + GlobalVariables.sConductorCircuitIDFieldName;

                // Execute the query and iterate through the cursor's results.
                featureCursor = fcConductor.Search(spatialFilter, false);
                while ((feature = featureCursor.NextFeature()) != null)
                {
                    dlength = 0;
                    string objectId = Convert.ToString(feature.get_Value(feature.Fields.FindField(GlobalVariables.sConductorObjectIdFieldName)));
                    string circuitId = Convert.ToString(feature.get_Value(feature.Fields.FindField(GlobalVariables.sConductorCircuitIDFieldName)));
                    if (!string.IsNullOrEmpty(circuitId))
                    {
                        ICurve curve = feature.Shape as ICurve;
                        ITopologicalOperator pTopo = new GeometryBagClass();
                        pTopo=(ITopologicalOperator)shape;

                        //pTopo.Simplify();
                        IGeometry pgeometry = pTopo.Intersect(feature.ShapeCopy, esriGeometryDimension.esriGeometry1Dimension);
                        if (pgeometry != null)
                        {
                            curve = pgeometry as ICurve;
                            if (conductorcircuitid.ContainsKey(circuitId) == false)
                            {
                                conductorcircuitid.Add(circuitId, ((IPolyline)pgeometry).Length);
                            }
                            else
                            {
                                conductorcircuitid.TryGetValue(circuitId, out dlength);
                                dlength = ((IPolyline)pgeometry).Length + dlength;
                                conductorcircuitid.Remove(circuitId);
                                conductorcircuitid.Add(circuitId, dlength);
                           }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                WriteLine("Error in processing "+ex.Message+ex.StackTrace);
            }
            finally
            {
                if (featureCursor != null) Marshal.ReleaseComObject(featureCursor );
            }
        }

        #region Find Nearest

        /// <summary>
        /// Find Nearest Circuit
        /// </summary>
        /// <param name="fi"></param>
        /// <param name="fcConductor"></param>
        /// <param name="shape"></param>
        /// <param name="nearestCircuitIdAndDistance"></param>
        /// <param name="Buffer_Distance"></param>
        private static void FindNearestCircuit(IFeatureClass fcConductor, IGeometry shape, ref Dictionary<string, double> nearestCircuitIdAndDistance, int Buffer_Distance)
        {
            try
            {

                double distanceLength, dlength;
                //int objectId = 0;
                //FindNearest(shape, fcConductor, fi, out objectId, out distanceLength, "");
                string strWhereClause = "";
                //IdDistancePair result = FindNearestFeature(shape, fcConductor, Buffer_Distance, strWhereClause);

                IdDistancePair result = FindNearestFeature(shape, fcConductor, Buffer_Distance, strWhereClause);
                IFeature nearestFeature = result.closestfeature;
                distanceLength = result.closestdistance;
                if (nearestFeature != null)
                {
                    string circuitId = Convert.ToString(nearestFeature.get_Value(nearestFeature.Fields.FindField(GlobalVariables.sConductorCircuitIDFieldName)));
                    if (!string.IsNullOrEmpty(circuitId))
                    {
                        if (nearestCircuitIdAndDistance.ContainsKey(circuitId) == false)
                        {

                            nearestCircuitIdAndDistance.Add(circuitId, distanceLength);
                        }
                        else
                        {
                            nearestCircuitIdAndDistance.TryGetValue(circuitId, out dlength);
                            dlength = distanceLength + dlength;
                            nearestCircuitIdAndDistance.Remove(circuitId);
                            nearestCircuitIdAndDistance.Add(circuitId, dlength);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// At least the spatial query can use the spatial index. It is surprisingly fast.
        /// This will optionally honor any pre-existing selection in the query FeatureLayer - 
        /// that is, it can do a 'select from' the existing selection, if any.
        /// </summary>
        private static IdDistancePair FindNearestFeature(IGeometry searchGeometry, IFeatureClass fcConductor, double bufferdistance, string where)
        {
            IFeatureCursor fcursor = null;
            ISpatialFilter spatialFilter = new SpatialFilterClass();

            // create a spatial query filter
            spatialFilter.GeometryField = fcConductor.ShapeFieldName;
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

            // specify the geometry to query with. apply a buffer if desired
            if (bufferdistance > 0.0)
            {
                // Use the ITopologicalOperator interface to create a buffer.
                ITopologicalOperator topoOperator = (ITopologicalOperator)searchGeometry;
                IGeometry buffer = topoOperator.Buffer(bufferdistance);
                spatialFilter.Geometry = buffer;
            }
            else
                spatialFilter.Geometry = searchGeometry;

            // apply the where clause if desired
            if (!string.IsNullOrEmpty(where))
                spatialFilter.WhereClause = where;

            // perform the query and use a cursor to hold the results

            fcursor = fcConductor.Search(spatialFilter, true);
            int closestoid = -1;
            double closestdistance = 0.0;
            IFeature feature = null;
            IFeature ClosestFeature = null;
            while ((feature = fcursor.NextFeature()) != null)
            {
                string circuitId = Convert.ToString(feature.get_Value(feature.Fields.FindField(GlobalVariables.sConductorCircuitIDFieldName)));
                if (!string.IsNullOrEmpty(circuitId))
                {
                    double distance = ((IProximityOperator)searchGeometry).ReturnDistance(feature.Shape);
                    if (closestoid == -1 || distance < closestdistance)
                    {
                        closestdistance = distance;
                        ClosestFeature = feature;
                    }
                }
            }
            if (fcursor != null) Marshal.ReleaseComObject(fcursor);
            return new IdDistancePair(ClosestFeature, closestdistance); ;
        }

        private static Dictionary<string, string> ProcessWIPs(System.Collections.Generic.Dictionary<string, List<string>> DirInstallJobNumbers, IWorkspace pWIPWorkspace, IWorkspace pEDERWorkSpace)
        {
            string jobnumber = string.Empty;
            List<string> lstObjectId = new List<string>();
            IQueryFilter queryFilter = new QueryFilterClass();
            IFeatureClass pWIPPolygonClass = default(IFeatureClass);
            IFeatureClass fcPriOHConductor = default(IFeatureClass);
            IFeatureClass fcSecOHConductor = default(IFeatureClass);
            IFeatureClass fcSecUGConductor = default(IFeatureClass);
            IFeatureClass fcPriUGConductor = default(IFeatureClass);
            string longestCircuitID = string.Empty;
            string longestLength;
            Dictionary<string, string> circuitIDs = new Dictionary<string, string>();
            Dictionary<string, double> circuitIdAndLength = new Dictionary<string, double>();
            List<string> failCircuitIDs = new List<string>();

            IGeometryCollection pGeoCollection = new GeometryBagClass();
            IGeometryBag pGeoBeg = new GeometryBagClass();
            IGeometry pUnionGeometry = new GeometryBagClass();

            try
            {
                fcPriOHConductor = (pEDERWorkSpace as IFeatureWorkspace).OpenFeatureClass(GlobalVariables.sPriOHConductorFeatureClassName);
                fcPriUGConductor = (pEDERWorkSpace as IFeatureWorkspace).OpenFeatureClass(GlobalVariables.sPriUGConductorFeatureClassName);
                fcSecOHConductor = (pEDERWorkSpace as IFeatureWorkspace).OpenFeatureClass(GlobalVariables.sSecOHConductorFeatureClassName);
                fcSecUGConductor = (pEDERWorkSpace as IFeatureWorkspace).OpenFeatureClass(GlobalVariables.sSecUGConductorFeatureClassName);
                foreach (var i in DirInstallJobNumbers)
                {
                    jobnumber = i.Key.ToString();
                    lstObjectId = i.Value;
                    string querystring = string.Empty;
                    if (lstObjectId != null)
                    {
                        if (lstObjectId.Count == 1)
                        {
                            querystring = GlobalVariables.sInstallJobNumberFieldName + " = " + jobnumber + " and " + GlobalVariables.sConductorObjectIdFieldName + " = '" + lstObjectId[0]+"'";
                        }
                        else if (lstObjectId.Count > 1)
                        {                            
                            string shortQuery = string.Empty;
                            int iCount999 = 0; bool isMore999=false;
                            foreach (var strObjectid in lstObjectId)
                            {
                                if (iCount999 == 0)
                                {
                                    if (isMore999)
                                    {
                                        querystring = querystring + shortQuery + ") OR ";
                                        shortQuery = string.Empty;
                                    }
                                    querystring += GlobalVariables.sConductorObjectIdFieldName + " in (";
                                }
                                ++iCount999;
                                if (lstObjectId.IndexOf(strObjectid) < lstObjectId.Count)
                                    shortQuery = shortQuery + "'" + strObjectid + "',";
                                if (iCount999 == 950)
                                {
                                    shortQuery = shortQuery.Remove(shortQuery.Length - 1);
                                    iCount999 = 0;
                                    isMore999 = true;
                                }
                            }

                            // GlobalVariables.sInstallJobNumberFieldName + " = " + jobnumber + "
                            shortQuery = shortQuery.Remove(shortQuery.Length - 1);
                            querystring = querystring + shortQuery+")";
                            querystring = GlobalVariables.sInstallJobNumberFieldName + " = " + jobnumber + " AND (" + querystring + ")";
                        }
                    }
                    else if (ConfigurationManager.AppSettings["RUN_FROM_CSV"].ToString().ToUpper() == "TRUE")
                    {
                        querystring = GlobalVariables.sInstallJobNumberFieldName + " = '" + jobnumber+"'";
                    }
                    queryFilter.WhereClause = querystring;
                    pWIPPolygonClass = (pWIPWorkspace as IFeatureWorkspace).OpenFeatureClass(GlobalVariables.sWIPPolygonFeatureclassName);
                    IFeatureCursor featureCursor = pWIPPolygonClass.Search(queryFilter, false);
                    IFeature wipFeature = featureCursor.NextFeature();                             
                    pUnionGeometry = null;
                    IGeometry tempGeo = null;
                    while (wipFeature != null)
                    {
                        if (pUnionGeometry != null)
                        {
                            tempGeo = wipFeature.ShapeCopy;
                            if (!tempGeo.IsEmpty)
                                pUnionGeometry = (pUnionGeometry as ITopologicalOperator4).Union(tempGeo);
                        }
                        else
                        {
                            tempGeo = wipFeature.ShapeCopy;
                            if (!tempGeo.IsEmpty)
                                pUnionGeometry = tempGeo;
                        }

                        wipFeature = featureCursor.NextFeature();
                    }
                    //pUnionGeometry = geometryCollection as IGeometry;
                    
                    //pGeoBeg = pGeoCollection as IGeometryBag; 
                    if (pUnionGeometry!=null)
                        FindCircuitCoveredBySingleWipCloud(pUnionGeometry, fcPriOHConductor, fcPriUGConductor, fcSecOHConductor, fcSecUGConductor, out longestCircuitID, out longestLength);
                    else
                    {
                        longestCircuitID = string.Empty;
                        WriteLine("Invalid geometry found in WIP Polygon for PM Ordernumber :" + jobnumber);
                    }
                    if (!string.IsNullOrEmpty(longestCircuitID))
                    {
                        if (!circuitIDs.ContainsKey(jobnumber))
                        {
                            circuitIDs.Add(jobnumber, longestCircuitID);

                        }
                    }
                    else
                    {
                        failCircuitIDs.Add(jobnumber);
                    }

                }
                return circuitIDs;
            }
            catch (Exception ex)
            {
                WriteLine("Error in processing.." + ex.Message + " " + ex.StackTrace);
                throw ex;
            }
            finally
            {

            }

        }

        private static string FindNearestFeatureFromWIP(IGeometry searchGeometry, List<IFeatureClass> lstFeatureClass, double bufferdistance, string where)
        {
            IFeatureCursor fcursor = null;
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            IArea pArea = default(IArea);
            IPoint pCenterPoint = new Point();
            IFeature feature = null;
            IFeature ClosestFeature = null;
            ITopologicalOperator topoOperator = default(ITopologicalOperator);
            IGeometry buffer = default(IGeometry);
            try
            {

                // apply the where clause if desired
                if (!string.IsNullOrEmpty(where))
                    spatialFilter.WhereClause = where;

                // perform the query and use a cursor to hold the results
                double closestdistance = 0.0;
                //IFeatureClass pSearchFeatureClass = fcPriOHConductor;
                //int closestoid = -1;
                string circuitId = string.Empty;
                double loopDistance = Convert.ToDouble(ConfigurationManager.AppSettings["MIN_BUFFER_DISTANCE"].ToString());
                bool status = false;
                while (loopDistance <= bufferdistance)
                {
                    spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                    topoOperator = (ITopologicalOperator)searchGeometry;
                    buffer = topoOperator.Buffer(loopDistance);
                    spatialFilter.Geometry = buffer;
                    WriteLine("Searching for near feature: " + DateTime.Now.ToString());
                    foreach (var pSearchFeatureClass in lstFeatureClass)
                    {
                        spatialFilter.GeometryField = pSearchFeatureClass.ShapeFieldName;
                        fcursor = pSearchFeatureClass.Search(spatialFilter, true);
                        while ((feature = fcursor.NextFeature()) != null)
                        {
                            double distance = ((IProximityOperator)searchGeometry).ReturnDistance(feature.Shape);
                            if (closestdistance == 0.0)
                                closestdistance = distance;

                            if (distance < closestdistance)
                            {
                                closestdistance = distance;
                                //ClosestFeature = feature;
                                circuitId = Convert.ToString(feature.get_Value(feature.Fields.FindField(GlobalVariables.sConductorCircuitIDFieldName)));
                                status = true;
                            }
                        }
                    }
                    if (status) break;
                    else loopDistance = loopDistance * 2;
                }
                if (fcursor != null) Marshal.ReleaseComObject(fcursor);
                return circuitId;
            }
            catch (Exception ex)
            {
                WriteLine("Error in finding nearest feature: " + ex.Message + " Trace" + ex.StackTrace.ToString());
                throw ex;
            }
            finally
            {

            }
        }
        
        # endregion    

        #endregion

        #region Private Methods
   
        private static List<string> GetInstallJobNumbers(IWorkspace objEderFW,ITable pTbleWipChanges)
        {
            List<string> lstInstallJobNumbers = null;
            try
            {
                lstInstallJobNumbers = new List<string>();
                ICursor pCursor = pTbleWipChanges.Search(null, false);
                IRow pRow = pCursor.NextRow();
                while (pRow != null)
                {
                    if (lstInstallJobNumbers.Contains(pRow.get_Value(pRow.Fields.FindField(GlobalVariables.sInstallJobNumberFieldName)).ToString())==false)
                    {
                        lstInstallJobNumbers.Add( pRow.get_Value(pRow.Fields.FindField(GlobalVariables.sInstallJobNumberFieldName)).ToString());
                    }
                   
                    pRow = pCursor.NextRow();
                }
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error : " + ex.Message);
                return null;
            }
            return lstInstallJobNumbers;
        }

        private static System.Collections.Generic.Dictionary<string, List<string>> GetInstallJobNumbersfromTable(IWorkspace objEderFW, ITable pTbleWipChanges)
        {

            //Hashtable lstInstallHTJobNmbers = null;
            Dictionary<string,List<string>> lstInstallHTJobNmbers = new Dictionary<string,List<string>>();
            List<string> lstInstallJobNumbers = null;
            try
            {
                lstInstallJobNumbers = new List<string>();
                ICursor pCursor = pTbleWipChanges.Search(null, false);
                IRow pRow = pCursor.NextRow();
                while (pRow != null)
                {

                    string jobNumber = pRow.get_Value(pRow.Fields.FindField(GlobalVariables.sInstallJobNumberFieldName)).ToString();
                    string objectID = pRow.get_Value(pRow.Fields.FindField(GlobalVariables.WIP_OBJECTID)).ToString();

                    if (!lstInstallHTJobNmbers.ContainsKey(jobNumber))
                    {
                        List<string> lstObjectId = new List<string>();
                        lstObjectId.Add(objectID);
                        lstInstallHTJobNmbers.Add(jobNumber,lstObjectId);
                    }
                    else
                    {
                        List<string> lstObjectId = lstInstallHTJobNmbers[jobNumber];
                        lstObjectId.Add(objectID);
                        lstInstallHTJobNmbers.Remove(jobNumber);
                        lstInstallHTJobNmbers.Add(jobNumber, lstObjectId);
                    }
                    pRow = pCursor.NextRow();
                }
                if (pCursor != null) Marshal.ReleaseComObject(pCursor);
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error : " + ex.Message);
                return null;
            }
            return lstInstallHTJobNmbers;
        }
               
        public static DataTable GetDataTableFromCSVFile(string csv_file_path)
        {
            DataTable csvData = new DataTable();
            try
            {
                using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
                {
                    csvReader.SetDelimiters(new string[] { "," });
                    csvReader.HasFieldsEnclosedInQuotes = true;
                    //read column names
                    string[] colFields = csvReader.ReadFields();
                    foreach (string column in colFields)
                    {
                        DataColumn datecolumn = new DataColumn(column);
                        datecolumn.AllowDBNull = true;
                        csvData.Columns.Add(datecolumn);
                    }
                    while (!csvReader.EndOfData)
                    {
                        string[] fieldData = csvReader.ReadFields();
                        //Making empty value as null
                        for (int i = 0; i < fieldData.Length; i++)
                        {
                            if (fieldData[i] == "")
                            {
                                fieldData[i] = null;
                            }
                        }
                        csvData.Rows.Add(fieldData);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return csvData;
        }
       
        private static Dictionary<string,List<string>> GetInstallJobNumbersFromCSV(DataTable CSVDataTable,IFeatureClass pWIPSPVW,IWorkspace wberworkspace )
        {
            Dictionary<string, List<string>> lstInstallJobNumbers = null;
            //List<string> lstInstallJobNumbers = null;
            try
            {
                
                ITable PGEPMORDERSTable = ((IFeatureWorkspace)wberworkspace).OpenTable(GlobalVariables.sPGEWorkOderTableName );
                lstInstallJobNumbers = new Dictionary<string, List<string>>();
                int i = 0;
                RecordInCSV = CSVDataTable.Rows.Count;
                foreach (DataRow dr in CSVDataTable.Rows)
                {
                    i = i + 1;
                    //If Job Present in PM WorkOder Table then Add the WIP Polygon ObjectID in the list
                    string sJobNumber = dr[GlobalVariables.sCSVField_WorkOrder].ToString().Trim();
                   
                     IQueryFilter pQueryFilter = new QueryFilterClass();
                     pQueryFilter.WhereClause = GlobalVariables.sInstallJobNumberFieldName  + " = '" + sJobNumber + "'"; ;
                     ICursor pPMWorkOrdercursor = PGEPMORDERSTable.Search(pQueryFilter, false);
                    IRow pPMOrderRow = pPMWorkOrdercursor.NextRow();
                    if (pPMOrderRow != null)
                    {

                        IQueryFilter pQF = new QueryFilterClass();
                        pQF.WhereClause = GlobalVariables.sInstallJobNumberFieldName + " = '" + sJobNumber + "'";
                        IFeatureCursor pcursor = pWIPSPVW.Search(pQF, false);
                        IFeature pWIPSW = pcursor.NextFeature();
                        if (pWIPSW != null)
                        {
                            while (pWIPSW != null)
                            {
                                if (lstInstallJobNumbers.Keys.Contains(pWIPSW.get_Value(pWIPSW.Fields.FindField(GlobalVariables.sInstallJobNumberFieldName)).ToString()) == false)
                                {
                                    RecordExstInGIS = RecordExstInGIS + 1;
                                    lstInstallJobNumbers.Add(pWIPSW.get_Value(pWIPSW.Fields.FindField(GlobalVariables.sInstallJobNumberFieldName)).ToString(),null);
                                }

                                pWIPSW = pcursor.NextFeature();
                            }
                        }
                        else
                        {
                            RecordExstinPMNotinWIP = RecordExstinPMNotinWIP + 1;
                            WriteLineToExceptionLog(DateTime.Now.ToLongTimeString() + " Record Present in the " +GlobalVariables.sPGEWorkOderTableName + " but not present in the " + GlobalVariables.sWIPPolygonFeatureclassName  + "  for OrderNumber- " + sJobNumber);
                        }
                            if (pcursor != null) Marshal.ReleaseComObject(pcursor);
                        

                    }
                    else
                    {
                        RecordNotExst = RecordNotExst + 1;
                        WriteLineToExceptionLog(DateTime.Now.ToLongTimeString() + " Record Not Present in the GIS for OrderNumber- " + sJobNumber);
                    }

                    if (pPMWorkOrdercursor!=null) Marshal.ReleaseComObject(pPMWorkOrdercursor);
                }
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error : " + ex.Message);
                return null;
            }
            return lstInstallJobNumbers;
        }

        private static void WriteToCSV(Dictionary<string, string> circuitIDs)
        {
            try
            {
                string filePath = GlobalVariables.sOUtPutFilePath ;
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(filePath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
             
                string FileName = "GIS_FLoc_update" + System.DateTime.Today.Date.ToShortDateString().Replace("/", "")  + ".txt";
                filePath = System.IO.Path.GetDirectoryName(filePath) + "\\" + FileName;
                string delimiter = "\t";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(GlobalVariables.sInstallJobNumberOutputFieldName + "\t" + GlobalVariables.sCircuitIDOutputFieldName );
                foreach (var circuits in circuitIDs)
                {
                    //Format- ED.01201.1121
                    string CircuitValue = "ED." + (circuits.Value).Substring(0, 5) + "." + (circuits.Value.Substring(circuits.Value.Length - (circuits.Value).Substring(0, 5).Length+1));
                    sb.AppendLine(circuits.Key + delimiter + CircuitValue);
                }

                File.WriteAllText(filePath, sb.ToString());

                filePath = System.IO.Path.GetDirectoryName(GlobalVariables.sOUtPutFilePath) + "\\" + ConfigurationManager.AppSettings["BACKUPFOLDERNAME"].ToString() + "\\";
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(filePath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                filePath = System.IO.Path.GetDirectoryName(GlobalVariables.sOUtPutFilePath) + "\\" + ConfigurationManager.AppSettings["BACKUPFOLDERNAME"].ToString() + "\\" + FileName.ToString();
                File.WriteAllText(filePath, sb.ToString());
               
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error : " + ex.Message);
            }
        }

        private static bool CreateTriggerFile()
        {
            try
            {
                string filePath = GlobalVariables.sOUtPutFilePath;
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(filePath)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));

                string FileName = ConfigurationManager.AppSettings["TRIGGER_FILENAME"].ToString();
                filePath = System.IO.Path.GetDirectoryName(filePath) + "\\" + FileName;
                string delimiter = "\t";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("------------ED09-TriggerFile-----------");               
                File.WriteAllText(filePath, sb.ToString());
                return true;
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error : " + ex.Message);
            }
            return false;
        }

        private static mmLicenseStatus CheckOutLicenses(mmLicensedProductCode productCode)
        {
            mmLicenseStatus licenseStatus;
            licenseStatus = arcFMAppInitialize.IsProductCodeAvailable(productCode);
            if (licenseStatus == mmLicenseStatus.mmLicenseAvailable)
            {
                licenseStatus = arcFMAppInitialize.Initialize(productCode);
            }
            return licenseStatus;
        }

        private static IWorkspace ArcSdeWorkspaceFromFile(String connectionFile)
        {
            IWorkspace _returnWorkspace = null;
            try
            {
                _returnWorkspace = ((IWorkspaceFactory)Activator.CreateInstance(Type.GetTypeFromProgID("esriDataSourcesGDB.SdeWorkspaceFactory"))).
                    OpenFromFile(connectionFile, 0);
            }
            catch (Exception ex)
            {
                WriteLine(DateTime.Now.ToLongTimeString() + " -- Error in Creating SDE Connection from -- : " + connectionFile + " Error Message -- " + ex.Message);
                _returnWorkspace = null;
            }
            return _returnWorkspace;
        }

        private static void WriteLine(string sMsg)
        {
            pSWriter = File.AppendText(sPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }
        private static void WriteLineToExceptionLog(string sMsg)
        {
            pSWriter = File.AppendText(sExceptionPath);
            pSWriter.WriteLine(sMsg);
            //DrawProgressBar();
            Console.WriteLine(sMsg);
            pSWriter.Close();
        }



        
        #endregion Private Methods

    }
}