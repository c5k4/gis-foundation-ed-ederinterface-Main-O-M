using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telvent.Delivery.Systems.Data;
using Telvent.Delivery.Systems.Data.Oracle;
using System.Data;
using Telvent.PGE.MapProduction.Processor;

namespace Telvent.PGE.MapProduction.Export
{
    public class PGEExporter:IMPExporter
    {
        private IDatabaseConnection _dbConnection = null;
        IMPDBDataManager _dbInteraction = null;
        private const string _successMessage = "Successfully processed maps";
        public PGEExporter()
        {
            
        }
        public IDatabaseConnection DatabaseConnection
        {
            get
            {
                if (_dbConnection == null)
                {
                    _dbConnection = new OracleDatabaseConnection(MapProductionConfigurationHandler.DatabaseServer, MapProductionConfigurationHandler.DataSource, MapProductionConfigurationHandler.UserName, MapProductionConfigurationHandler.Password);
                }
                return _dbConnection;
            }
        }

        public string SuccessMessage
        {
            get { return _successMessage; }
        }
        /// <summary>
        /// Returns an instance of PGEMapLookUpTable
        /// </summary>
        public IMPMapLookUpTable MapLookUpTable
        {
            get { return new PGEMapLookUpTable(); }
        }

        public List<string> Export(int processId)
        {
            List<string> retVal = new List<string>();
            DataTable mapLut = DatabaseManager.GetMapLookUpTable("where " + MapLookUpTable.MapExportState + "=" + ExportState.ReadyToExport + " and "+MapLookUpTable.ProcessingService+"="+processId);
            try
            {
                for (int i = 0; i < mapLut.Rows.Count - 1; i++)
                {
                    //Micro Transaction makes hte App chatty. May be we should bulk update everything and then do the export and bulk update fro idle state.
                    mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Processing;
                    DatabaseManager.UpdateLookUpTable(mapLut);
                    //Do export here
                    List<string> errorList = new List<string>();
                    PGEExportData exportData = new PGEExportData(DatabaseManager);
                    errorList=exportData.ProcessRow(mapLut.Rows[i], MapLookUpTable);
                    //Complete This
                    if (errorList.Count == 1)
                    {
                        mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Idle;
                        DatabaseManager.UpdateLookUpTable(mapLut);
                    }
                    else
                    {
                        mapLut.Rows[i][MapLookUpTable.MapExportState] = ExportState.Failed;
                        DatabaseManager.UpdateLookUpTable(mapLut);
                    }
                }
            }
            catch (Exception ex)
            {
                mapLut.Rows[0][MapLookUpTable.MapExportState] = ExportState.Failed;
                DatabaseManager.UpdateLookUpTable(mapLut);
                retVal.Add(ex.Message);
                retVal.Add(ex.StackTrace);
            }
            return retVal;
        }


        /// <summary>
        /// New instance of DBDataManager object
        /// </summary>
        public IMPDBDataManager DatabaseManager
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
    }
}
