using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using PGE.Common.Delivery.ArcFM;
using PGE.Common.Delivery.Diagnostics;
using System.Reflection;
using ESRI.ArcGIS.Geodatabase;
using Miner.Interop;
using PGE.Desktop.EDER.PLC;
using PGE.Common.Delivery.Framework;
using ESRI.ArcGIS.Geometry;
using Miner.ComCategories;
using PGE.Desktop.EDER.ArcMapCommands;

namespace PGE.Desktop.EDER.AutoUpdaters.Special.PLC
{
    
    [Guid("901F63E6-DBE6-4580-A5B7-2CC652DA9319")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdatePLDBID")]
    [ComponentCategory(ComCategory.SpecialAutoUpdateStrategy)]
    public class PGEUpdateElevationAU: BaseSpecialAU
    {
        private const int SUBTYPE_POLE = 1;      
        private const string CUST_OWNED_NO = "N";
        private string SLookupTable = "EDGIS.PGE_PLC_CONFIG";
        protected static Log4NetLogger _logger = new Log4NetLogger(MethodBase.GetCurrentMethod().DeclaringType, "EDERDesktop.log4net.config");
        public static bool callByInterfaceED11 = false;
        public static IWorkspace PTTlandbaseWksp = null;
        public static IWorkspace PTTSessionWksp = null;
        public static string PTTJobNumber = "0";
        //M4JF EDGISREARCH 415 - getting Ed11 Interface User name to generate pldbid 
        public static string ED11interfaceUser = default;
        #region Constructor

        /// <summary>
        /// Constructor for BaseSplitAU
        /// </summary>
        /// <param name="name">Name of the AU</param>
        public PGEUpdateElevationAU()
            : base("PGE Update PLDBID AU")
        { }

        #endregion

        #region BaseSpecialAU overrides

        /// <summary>
        /// Determines in which class the AU will be enabled.
        /// </summary>
        /// <param name="objectClass"> Object's class. </param>
        /// <param name="eEvent">The edit event.</param>
        /// <returns> <c>true</c> if the AuoUpdater should be enabled; otherwise <c>false</c> </returns>
        protected override bool InternalEnabled(IObjectClass objectClass, mmEditEvent eEvent)
        {
            if( (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate)|| (eEvent == Miner.Interop.mmEditEvent.mmEventFeatureUpdate))
            {
                _logger.Debug("Returning Visible: true.");
                return true;
            }
            _logger.Debug("Returning Visible: false.");
            return false;
           
            
        }

        /// <summary>
        /// Implementation of AutoUpdater Execute Ex method for derived classes.
        /// </summary>
        /// <param name="obj">The object that triggered the AutoUpdater.</param>
        /// <param name="eAUMode">The auto updater mode.</param>
        /// <param name="eEvent">The edit event.</param>
        protected override void InternalExecute(IObject obj, mmAutoUpdaterMode eAUMode, mmEditEvent eEvent)
        {
            try
            {
                if ((eEvent == Miner.Interop.mmEditEvent.mmEventFeatureCreate))
                {
                    UpdateFeature(obj);

                }               
            }
            catch (Exception e)
            {
                _logger.Error("PGE Update PLDBID AU failed. Message: " + e.Message + " Stacktrace: " + e.StackTrace);
            }
        }

        

        public void UpdateFeature(IObject obj)
        {
            try
            {
                #region Update PLDBID

                //Get the PLDBID field Index 
                int idxPLDBID = ModelNameFacade.FieldIndexFromModelName(
                    obj.Class,
                    SchemaInfo.Electric.FieldModelNames.PLDBID);
                int idxCustomerOwned = ModelNameFacade.FieldIndexFromModelName(
                    obj.Class,
                    SchemaInfo.Electric.FieldModelNames.CustomerOwned);

                ITable tPLCLookup = GetLookUpTable(((obj as IFeature).Class as IFeatureClass).FeatureDataset.Workspace as IFeatureWorkspace);
                IRow ConfigTableRow = GetConfigTableRowForNewExceptionTool(tPLCLookup);
                //Get the subtype field index 
                int idxSubtype = -1;
                int subtypeCd = -1;
                string custOwned = string.Empty;
                ISubtypes pSubTypes = (ISubtypes)((IRow)obj).Table;
                if (pSubTypes != null)
                    idxSubtype = pSubTypes.SubtypeFieldIndex;
                  //Check for null PLDBID 
                if (idxPLDBID != -1)
                {
                    subtypeCd = Convert.ToInt32 (obj.get_Value(idxSubtype).ToString());
                    if ((subtypeCd == SUBTYPE_POLE))
                    {
                        if (obj.get_Value(idxPLDBID) == DBNull.Value)
                        {
                            StartAnalysisCall sacObj = new StartAnalysisCall();
                            IPoint pPole = (obj as IFeature).ShapeCopy as IPoint;
                            //Fix : 22-Oct-20 : Cal by ED 11 Interface
                            string jobNumber = string.Empty;
                            if (PGE.Desktop.EDER.AutoUpdaters.Special.PLC.PGEUpdateElevationAU.callByInterfaceED11)
                            {
                                jobNumber = PTTJobNumber;
                                if(string.IsNullOrEmpty(jobNumber)) { jobNumber = "0"; }
                            }
                            else
                            {
                                jobNumber = PopulateLastJobNumberUC.jobNumber;
                            }
                            double dpld = (new StartAnalysisCall()).Createpldbid(pPole.X, pPole.Y, obj as IFeature, jobNumber);
                            if (dpld == 0.0)
                            {
                                Console.WriteLine("PLDBID not returned from the StartAnalysis service");
                                throw new Exception("PLDBID not returned from the StartAnalysis service");
                            }
                            obj.set_Value(idxPLDBID, dpld);
                            ////Update the Lat and Long
                            double _dLatitude, _dLongitude;
                            XY_LatLong(obj as IFeature, out _dLatitude ,out _dLongitude);
                            obj.set_Value(obj.Fields.FindField(ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("GpsLongitude_ColName")).ToString()), _dLatitude);
                            obj.set_Value(obj.Fields.FindField(ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("GpsLongitude_ColName")).ToString()), _dLongitude);
                            IFeatureClass featureClassMP = (((obj as IFeature).Class as IFeatureClass).FeatureDataset.Workspace as IFeatureWorkspace).OpenFeatureClass((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("Maintenanceplat_TblName"))).ToString());
                            IFeature iFeatMp = GetFeatureFromSpatialQuery(pPole, featureClassMP);
                            string strLocalOfcId = string.Empty;
                            if (iFeatMp == null)
                                strLocalOfcId = "XJ";
                            else
                                strLocalOfcId = iFeatMp.get_Value(iFeatMp.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("LocalOfficeId_ColName"))).ToString())).ToString().Trim();
                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("LocalOfficeId_ColName"))).ToString()), strLocalOfcId);

                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("InstallJobNumber_ColName"))).ToString()), PopulateLastJobNumberUC.jobNumber);
                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("Datecreated_ColName"))).ToString()), System.DateTime.Now.ToString());


                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("CustomerOwner_ColName"))).ToString()), "N");
                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("Status_ColName"))).ToString()), (ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("STATUS_DEFAULT_VAL"))).ToString());
                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("POLEUSE_ColName"))).ToString()), (ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("POLEUSE_DEFAULT_VAL"))).ToString());
                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("POLETYPE_ColName"))).ToString()), (ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("POLETYPE_DEFAULT_VAL"))).ToString());
                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("MATERIAL_ColName"))).ToString()), (ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("MATERIAL_DEFAULT_VAL"))).ToString());
                            obj.set_Value(obj.Fields.FindField((ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("JOINTCOUNT_ColName"))).ToString()), (ConfigTableRow.get_Value(ConfigTableRow.Fields.FindField("JOINTCOUNT_DEFAULT_VAL"))).ToString());

                        }
                    }
                }
               
              
                #endregion
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>Get feature from spatial serch </summary>
        /// <param name="feat">esri feature</param>
        /// <param name="polyFeatClass">feature class to serch</param>
        /// <returns>return polygon feature</returns>
        private IFeature GetFeatureFromSpatialQuery(IPoint ppole, IFeatureClass polyFeatClass)
        {
            IFeature searchedFeature = null;
            IFeatureCursor featCursor = null;
            try
            {
                //perform spatial filter
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = ppole;
                spatialFilter.GeometryField = polyFeatClass.ShapeFieldName;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;

                //get featureCursor
                featCursor = polyFeatClass.Search(spatialFilter, false);

                //Get the feature from feature cursor
                while ((searchedFeature = featCursor.NextFeature()) != null)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PG&E PLC Create pole on Notification, Error in GetFeatureFromSpatialQuery function", ex);
            }
            finally
            {
                //Release COM object
                while (Marshal.ReleaseComObject(featCursor) > 0) { }
                featCursor = null;
            }
            return searchedFeature;

        }
        private ITable GetLookUpTable(IFeatureWorkspace iFeatWs)
        {
            ITable tPLCLookup = null;
            try
            {

                

                tPLCLookup = new MMTableUtilsClass().OpenTable(SLookupTable, iFeatWs);

                if (tPLCLookup == null)
                    throw new Exception("Failed to load table " + SLookupTable);

            }
            catch (Exception ex)
            { }

            return tPLCLookup;
        }

        private IRow GetConfigTableRowForNewExceptionTool(ITable tPLCLookup)
        {
            IRow ConfigTableRow = null;
            try
            {
                ICursor ICur = null;

                QueryFilter queryFilter = null;
                queryFilter = new QueryFilterClass();
                queryFilter.WhereClause = string.Format("ClassData='{0}'", "EXP_CREATE_POLE");
                ICur = tPLCLookup.Search(queryFilter, false);
                ConfigTableRow = ICur.NextRow();
                if (ICur != null) { Marshal.ReleaseComObject(ICur); }
                if (ConfigTableRow == null) throw new Exception("Configuration Data not found for CREATE_POLE in " + SLookupTable + " table");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ConfigTableRow;
        }

        //Function to convert XY to Latitude longitude
        public void XY_LatLong(IFeature feature, out double _dLatitude, out double _dLongitude)
        {
            _dLatitude = 0;
            _dLongitude = 0;
            try
            {

                ISpatialReferenceFactory2 srFactory = new SpatialReferenceEnvironmentClass();
                int NAD27_Geograpic = (4326);
                ISpatialReference GeographicSR = srFactory.CreateSpatialReference(NAD27_Geograpic);

                IFeatureChanges pFeatChange = (IFeatureChanges)feature;
                if (pFeatChange.ShapeChanged)
                {
                    IPoint pPoint = (IPoint)feature.ShapeCopy;
                    ISpatialReference sr1 = pPoint.SpatialReference;
                    ISpatialReferenceResolution srRes = sr1 as ISpatialReferenceResolution;
                    IPoint geographicPoint = new ESRI.ArcGIS.Geometry.Point();
                    geographicPoint.X = pPoint.X;
                    geographicPoint.Y = pPoint.Y;
                    geographicPoint.Z = pPoint.Z;
                    geographicPoint.SpatialReference = sr1; // PCS

                    geographicPoint.Project(GeographicSR);

                    _dLatitude = geographicPoint.Y;
                    _dLongitude = geographicPoint.X;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

    }
}
