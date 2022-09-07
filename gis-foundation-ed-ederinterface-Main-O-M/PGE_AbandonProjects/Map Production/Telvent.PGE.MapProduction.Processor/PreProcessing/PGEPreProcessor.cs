using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Systems.Data;
using Telvent.Delivery.Systems.Data.Oracle;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesGDB;
using Telvent.PGE.MapProduction.Processor;

namespace Telvent.PGE.MapProduction.PreProcessing
{
    /// <summary>
    /// PGE Specific PreProcessor that would read the ChangeDetection table,Map Polygon featurecalss and updates the Map Look up table
    /// Extends BasePreProcessor abstract class.
    /// </summary>
    public class PGEPreProcessor:BaseMapPreProcessor
    {
        #region Private Members
        private IDatabaseConnection _dbConnection = null;
        private Dictionary<string, string> _gdbMapLUTLookup = null;
        private IMPDBDataManager _dbInteraction = null;
        private IMPGDBDataManager _gdbInteraction = null;
        #endregion Private Members

        #region public overrides
        /// <summary>
        /// Returns an instance of PGEGDBFieldLookup
        /// </summary>
        public override IMPGDBFieldLookUp GDBFieldLookUp
        {
            get { return new PGEGDBFieldLookUp(); }
        }
        /// <summary>
        /// Returns an instance of PGEMapLookUpTable
        /// </summary>
        public override IMPMapLookUpTable MapLookUpTable
        {
            get { return new PGEMapLookUpTable(); }
        }

        /// <summary>
        /// Returns an Instance of PGEMPChangeDetector object
        /// </summary>
        public override IMPChangeDetector ChangeDetector
        {
            get { return new PGEMPChangeDetector2(this.Workspace, this.GeodatabaseManager); }
        }
        /// <summary>
        /// Returns an instance of OracleDatabaseConnection object
        /// </summary>
        public override IDatabaseConnection DatabaseConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    _dbConnection = new OracleDatabaseConnection(MapProductionConfigurationHandler.DatabaseServer,MapProductionConfigurationHandler.DataSource, MapProductionConfigurationHandler.UserName, MapProductionConfigurationHandler.Password);
                }
                return _dbConnection;
            }
        }
        /// <summary>
        /// Field LookUp utilized for PG&E
        /// MapOffice="PGE_MAPOFFICE"
        /// LEGACYCOORDINATE="PGE_LEGACYCOORDINATE"
        /// </summary>
        public override Dictionary<string, string> MapLUTGDBFieldLookUp
        {
            get
            {
                if (_gdbMapLUTLookup == null)
                {
                    _gdbMapLUTLookup = new Dictionary<string, string>();
                    _gdbMapLUTLookup.Add("MAPOFFICE",PGEGDBFieldLookUp.PGEGDBFields.MapOfficeMN);
                    _gdbMapLUTLookup.Add("LEGACYCOORDINATE",PGEGDBFieldLookUp.PGEGDBFields.LegacyCorrdSystemMN);
                    _gdbMapLUTLookup.Add("MAPSCALE", PGEGDBFieldLookUp.PGEGDBFields.MapTypeMN);
                    _gdbMapLUTLookup.Add("MAPID", PGEGDBFieldLookUp.PGEGDBFields.MapIDMN);
                }
                return _gdbMapLUTLookup;
            }
        }

        /// <summary>
        /// New instance of DBDataManager object
        /// </summary>
        public override IMPDBDataManager DatabaseManager
        {
            get
            {
                if (_dbInteraction == null)
                {
                    _dbInteraction = new DBDataManager(DatabaseConnection, MapLookUpTable);
                }
                return _dbInteraction;
            }
            protected set
            {
                _dbInteraction = value;
            }
        }
        /// <summary>
        /// New instance of GDBDataManager
        /// </summary>
        public override IMPGDBDataManager GeodatabaseManager
        {
            get
            {
                if (_gdbInteraction == null)
                {
                    _gdbInteraction = new PGEGDBDataManager(Workspace, GDBFieldLookUp);
                }
                return _gdbInteraction;
            }
        }
        #endregion

        #region protected overrides
        /// <summary>
        /// Initializes a ArcSDE SpatialWorkspace using Configuration file
        /// </summary>
        /// <returns></returns>
        protected override IWorkspace InitializeWorkspace()
        {
            IPropertySet propSet = GetPropertySet();
            IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
            return workspaceFactory.Open(propSet, 0);
        }
        #endregion

        #region private methods

        /// <summary>
        /// Gets the PropertySet to use to open ArcSDE Workspace
        /// </summary>
        /// <returns></returns>
        private IPropertySet GetPropertySet()
        {
            IPropertySet propSet = new PropertySetClass();
            if (string.IsNullOrEmpty(MapProductionConfigurationHandler.GeodatabaseSetting.Instance))
                throw new NullReferenceException("Instance is required");
            else
                propSet.SetProperty("INSTANCE", MapProductionConfigurationHandler.GeodatabaseSetting.Instance);

            if (string.IsNullOrEmpty(MapProductionConfigurationHandler.UserName))
                throw new NullReferenceException("Username is required");
            else
                propSet.SetProperty("USER", MapProductionConfigurationHandler.UserName);

            if (string.IsNullOrEmpty(MapProductionConfigurationHandler.Password))
                throw new NullReferenceException("Password is required");
            else
                propSet.SetProperty("PASSWORD", MapProductionConfigurationHandler.Password);

            if (!string.IsNullOrEmpty(MapProductionConfigurationHandler.GeodatabaseSetting.Database))
                propSet.SetProperty("DATABASE", MapProductionConfigurationHandler.GeodatabaseSetting.Database);

            if (!string.IsNullOrEmpty(MapProductionConfigurationHandler.GeodatabaseSetting.Server))
                propSet.SetProperty("SERVER", MapProductionConfigurationHandler.GeodatabaseSetting.Server);

            if (!string.IsNullOrEmpty(MapProductionConfigurationHandler.GeodatabaseSetting.Version))
                propSet.SetProperty("VERSION", MapProductionConfigurationHandler.GeodatabaseSetting.Version.ToUpper());
            else
                propSet.SetProperty("VERSION", "SDE.DEFAULT");

            return propSet;
        }

        #endregion
    }
}
