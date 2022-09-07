using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Data.OracleClient;
using System.Data;
using PGE.Interface.PNodeSync.OracleClasses;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace PGE.Interface.PNodeSync.PNodeSyncProcess
{
    /// <summary>
    /// Feb-2019, ETAR Project - Moving FNM data from MDR to GIS
    /// This is a Console (EXE) application, that when executed will read the data from
    /// source and write the data to target.
    /// In this case SOURCE data is MDR and the TARGET data is GIS.
    /// The connection properties are stored in CONFIG file.
    /// to do: FNM and MDR?
    /// source and target table names.
    /// 
    /// The process reads FNM data from MDR and write to FNM table in GIS.
    /// -The process is desinged to run nighly as UC4 job, and keep the GIS FNM table in Sync with FNM data in MDR.
    /// FNM: Full Network Mode
    /// MDR: Market Data Repository
    /// </summary>
    /// 
    internal class LoadDataToGISFNM
    {
        #region Private variables
        // Create a logger for use in this class
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Table Name:
        private const string _GIS_FNM_LAP_TEMPtblName = "EDGIS.GIS_FNM_LAP_TEMP";
        private const string _GIS_SUBLAP_LCAtblName = "EDGIS.GIS_SUBLAP_LCA";

        //Stored Procedure Names:
        private const string _GIS_MERGE_FNM_COMP_storedProcName = "GIS_MERGE_FNM_COMP";// "GIS_MERGE_FNM_RECORDS";
        private const string _GIS_INSERT_FNM_COMP_storedProcName = "GIS_INSERT_FNM_COMP";//"GIS_INSERT_FNM_RECORDS";

        private const string _GIS_MERGE_FNM_FROM_FNM_COMP_storedProcName = "GIS_MERGE_FNM_FROM_FNM_COMP";// "GIS_MERGE_FNM_RECORDS";
        private const string _GIS_INSERT_FNM_FROM_FNM_COMP_storedProcName = "GIS_INSERT_FNM_FROM_FNM_COMP";

        //Below code commented for project ARET Substation - changes done on 25 JUly 2022 by v1t8
        //This changes suggested by PNode Business owner steav : Start
        //private const string _GIS_UPDATE_FNM_LCA_ID_storedProcName = "GIS_UPDATE_FNM_LCA_ID";

        //above code commented for project ARET Substation -  End

        private OracleConnection _gisOracleCon = null;
        private OracleConnection _mdrOracleCon = null;
        #endregion

        /// <summary>
        ///Constructor that creates GIS and MDR oracle connections.
        /// </summary>
        public LoadDataToGISFNM()
        {
            try
            {
                //Open GIS-Oracle connection.
                _gisOracleCon = DBUtils.GetDBConnection(true, false);
                _logger.Info("Open Connection GIS: " + _gisOracleCon.DataSource);
                _gisOracleCon.Open();
                if (_gisOracleCon.State == ConnectionState.Open)
                    _logger.Info("-Successful Connection.");
                else
                    throw new Exception("Failed to open GIS Oracle Connection.");

                //Open MDR-Oracle connection.
                _mdrOracleCon = DBUtils.GetDBConnection(false, true);
                _logger.Info("Open Connection MDR: " + _mdrOracleCon.DataSource);
                _mdrOracleCon.Open();
                if (_mdrOracleCon.State == ConnectionState.Open)
                    _logger.Info("-Successful Connection.");
                else
                    throw new Exception("Failed to open MDR Oracle Connection.");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
            }
        }

        /// <summary>
        ///Loads data from MDR to GIS FNM table.
        /// </summary>
        public void ExecuteLoad()
        {
            try
            {
                //This is executed as SQL statement with the code.
                if (_gisOracleCon == null || _mdrOracleCon == null)
                    return;

                //Injest MSR SubLap LCA data into GIS.
                DeleteAllFromTable(_gisOracleCon, _GIS_SUBLAP_LCAtblName);
                Load_MDR_To_GIS_SUBLAP_LCA(_mdrOracleCon, _gisOracleCon);// TO DO: once mdr table is availble, change source connection to MDR.

                DeleteAllFromTable(_gisOracleCon, _GIS_FNM_LAP_TEMPtblName);

                //Step 1. Move data from MDR to GIS_FNM_LAP_TEMP table.
                if (!Load_MDR_To_GIS_FNM_TEMP(_mdrOracleCon, _gisOracleCon))
                {
                    _logger.Info("EXIT PROCESS.");
                    return;
                }

                //Step 2. Merge data (updates only) from GIS Temp table to GIS FNM_COMPLETE table.
                DBUtils.ExecuteStoredProcedure(_gisOracleCon, _GIS_MERGE_FNM_COMP_storedProcName);

                //Step 3. Insert new data (inserts only) from GIS Temp table to GIS FNM_COMPELTE table.
                DBUtils.ExecuteStoredProcedure(_gisOracleCon, _GIS_INSERT_FNM_COMP_storedProcName);

                //Step 4 and 5, we can't do dump and reload becuase FNM table is ESRI table, and has relationships to SubPNode feature class, and 
                //those relationships need to be maintainted. We do not delete records from FNM table either.

                //Step 4. Merge data (updates only) from FNM_COMPELTE table to GIS FNM table.
                DBUtils.ExecuteStoredProcedure(_gisOracleCon, _GIS_MERGE_FNM_FROM_FNM_COMP_storedProcName);

                //Step 5. Insert new data (inserts only) from FNM_COMPLETE table to GIS FNM table.
                DBUtils.ExecuteStoredProcedure(_gisOracleCon, _GIS_INSERT_FNM_FROM_FNM_COMP_storedProcName);

                //Below code commented for project ARET Substation - changes done on 25 JUly 2022 by v1t8
                //This changes suggested by PNode Business owner steav : Start
                //Update LCA value in GIS_FNM table.
                // DBUtils.ExecuteStoredProcedure(_gisOracleCon, _GIS_UPDATE_FNM_LCA_ID_storedProcName);
                //above code commented for project ARET Substation -  End

                //Below code commented for project ARET Substation - Changes for not to delete data from GIS_FNM_LAP_TEMP table as it is already happning from start of the process
                //This changes suggested by PNode Business owner steav : Start
                //Step 6. Clear GIS_FNM_LAP_TEMP table (delete all the data).

                //  DeleteAllFromTable(_gisOracleCon, _GIS_FNM_LAP_TEMPtblName);

                //above code commented for project ARET Substation - Changes for not to delete data from GIS_FNM_LAP_TEMP table as it is already happning from start of the process : End
            }
            //catch (Exception Ex)
            //{ _logger.Debug(Ex.Message); }
            catch (Exception ex)
            { InitializeProcess.ErrorMessage(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name); }
        }

        ///<summary>
        /// Step 1. Move data from MDR to GIS_FNM_LAP_TEMP table.
        /// This is executed as SQL statement within the code.
        /// </summary>
        /// <param name="mdrConn">The MDR connection the data will read from.</param>
        /// <param name="gisOracleCon">The GIS connection the data will be written to.</param>
        private bool Load_MDR_To_GIS_FNM_TEMP(OracleConnection mdrConn, OracleConnection gisOracleCon)
        {
            try
            {
                #region MDR-Query Old
                //                string sql = @"
                //                select
                //                  c.udc_id,
                //                  c.lap_id,
                //                  c.res_type,
                //                  c.cnode_id,
                //                  c.bus_id,
                //                  f.first_fnm_version,
                //                  f.first_fnm_release_date,
                //                  c.latest_fnm_version,
                //                  c.latest_fnm_release_date
                //                from
                //                  (
                //                    select
                //                      a.udc_id,
                //                      a.lap_id,
                //                      a.res_type,
                //                      a.cnode_id,
                //                      a.bus_id,
                //                      a.fnm_version latest_fnm_version,
                //                      b.max_release_date latest_fnm_release_date
                //                    from ze_data.caiso_fnm_pnm_lap a
                //                    inner join
                //                      (
                //                        select
                //                          cnode_id,
                //                          bus_id,
                //                          max(release_date) max_release_date
                //                        from ze_data.caiso_fnm_pnm_lap
                //                        where
                //                          upper(udc_id) in ('PGAE', 'SCE')
                //                          and upper(lap_id) like 'SLAP%'
                //                          and upper(res_type) = 'LOAD'
                //						  and upper(lap_id) not in ('SLAP_PGF2','SLAP_PGF3','SLAP_PGF4','SLAP_PGF5','SLAP_PGP3')
                //                        group by
                //                          cnode_id, bus_id
                //                      ) b
                //                    on a.cnode_id = b.cnode_id
                //                      and a.bus_id = b.bus_id
                //                      and a.release_date = b.max_release_date
                //                    where
                //                      upper(a.udc_id) in ('PGAE', 'SCE')
                //                      and upper(a.lap_id) like 'SLAP%'
                //                      and upper(a.res_type) = 'LOAD'
                //					  and upper(lap_id) not in ('SLAP_PGF2','SLAP_PGF3','SLAP_PGF4','SLAP_PGF5','SLAP_PGP3')
                //                  ) c
                //                  inner join
                //                  (
                //                    select
                //                      d.cnode_id,
                //                      d.bus_id,
                //                      d.fnm_version first_fnm_version,
                //                      e.min_release_date first_fnm_release_date
                //                    from ze_data.caiso_fnm_pnm_lap d
                //                    inner join
                //                      (
                //                        select
                //                          cnode_id,
                //                          bus_id,
                //                          min(release_date) min_release_date
                //                        from ze_data.caiso_fnm_pnm_lap
                //                        where
                //                          upper(udc_id) in ('PGAE', 'SCE')
                //                          and upper(lap_id) like 'SLAP%'
                //                          and upper(res_type) = 'LOAD'
                //						  and upper(lap_id) not in ('SLAP_PGF2','SLAP_PGF3','SLAP_PGF4','SLAP_PGF5','SLAP_PGP3')
                //                        group by cnode_id, bus_id
                //                      ) e
                //                    on d.cnode_id = e.cnode_id
                //                      and d.bus_id = e.bus_id
                //                      and d.release_date = e.min_release_date
                //                    where
                //                      upper(d.udc_id) in ('PGAE', 'SCE')
                //                      and upper(d.lap_id) like 'SLAP%'
                //                      and upper(d.res_type) = 'LOAD'
                //					  and upper(lap_id) not in ('SLAP_PGF2','SLAP_PGF3','SLAP_PGF4','SLAP_PGF5','SLAP_PGP3')
                //                  ) f
                //                on
                //                  c.cnode_id = f.cnode_id
                //                                  and c.bus_id = f.bus_id";
                #endregion

                //Below code commented for project ARET Substation - changes done on 25 JUly 2022 by v1t8
                //This changes suggested by PNode Business owner steav : Start

                #region MDR-Query
                //string sql = @" with dataset1 as
                //              (
                //                select
                //                  udc_id,
                //                  lap_id,
                //                  res_type,
                //                  cnode_id,
                //                  bus_id,
                //                  fnm_version,
                //                  release_date
                //                from 
                //                  ze_data.caiso_fnm_pnm_lap 
                //               where 
                //                  upper(udc_id) in ('PGAE', 'SCE')
                //                  and upper(lap_id) like 'SLAP%'
                //                  and upper(res_type) = 'LOAD'
                //                  and upper(lap_id) not in ('SLAP_PGF2','SLAP_PGF3','SLAP_PGF4','SLAP_PGF5','SLAP_PGP3') -- prevents bad data in MDR from being extracted 
                //              )
                //              (
                //                select 
                //                  c.udc_id,
                //                  c.lap_id,
                //                  c.res_type,
                //                  c.cnode_id,
                //                  c.bus_id,
                //                  f.first_fnm_version,
                //                  f.first_fnm_release_date,
                //                  c.latest_fnm_version,
                //                  c.latest_fnm_release_date   
                //                from
                //                  (
                //                    select 
                //                      a.udc_id,
                //                      a.lap_id,
                //                      a.res_type,
                //                      a.cnode_id,
                //                      a.bus_id,
                //                      a.fnm_version latest_fnm_version,
                //                      b.max_release_date latest_fnm_release_date
                //                    from dataset1 a
                //                    inner join
                //                      (
                //                        select
                //                          lap_id,
                //                          cnode_id,
                //                          bus_id,
                //                          max(release_date) max_release_date
                //                        from 
                //                          dataset1 
                //                        group by 
                //                          lap_id,
                //                          cnode_id,
                //                          bus_id
                //                      ) b
                //                    on 
                //                        a.lap_id = b.lap_id
                //                        and a.cnode_id = b.cnode_id 
                //                        and a.bus_id = b.bus_id 
                //                        and a.release_date = b.max_release_date
                //                  ) c  
                //                inner join     
                //                  (  
                //                    select
                //                      d.lap_id,
                //                      d.cnode_id,
                //                      d.bus_id,
                //                      d.fnm_version first_fnm_version,
                //                      e.min_release_date first_fnm_release_date
                //                    from dataset1 d
                //                    inner join
                //                      (
                //                        select
                //                          lap_id,
                //                          cnode_id,
                //                          bus_id,
                //                          min(release_date) min_release_date
                //                        from 
                //                          dataset1
                //                        group by 
                //                          lap_id,
                //                          cnode_id,
                //                          bus_id
                //                      ) e
                //                    on 
                //                        d.lap_id = e.lap_id
                //                        and d.cnode_id = e.cnode_id 
                //                        and d.bus_id = e.bus_id 
                //                        and d.release_date = e.min_release_date
                //                  ) f    
                //                on 
                //                  c.lap_id = f.lap_id
                //                  and c.cnode_id = f.cnode_id
                //                  and c.bus_id = f.bus_id
                //          )";
                #endregion

                //above code commented for project ARET Substation -  End

                //Below code added for project ARET Substation - changes done on 25 JUly 2022 by v1t8
                //This changes suggested by PNode Business owner steav : Start

                #region MDR-Query

                //For a while, the CAISO was publishing the same FNM data in two different systems.
                //The data in table caiso_fnm_pnm_lap is from the older system and is no longer being used.
                //The data in table mrtu_fnm_ciso_lap is from the newer system and it will be used going forward.
                //The CAISO published versions FNM_DB21M4 and FNM_DB21M6 to table caiso_fnm_pnm_lap, then later published the same data as versions 21M4_DB104 and 21M6_DB105 to table mrtu_fnm_ciso_lap.
                //We decided to use versions FNM_DB21M4 and FNM_DB21M6 from table caiso_fnm_pnm_lap instead of versions 21M4_DB104 and 21M6_DB105 from table mrtu_fnm_ciso_lap 
                //because versions FNM_DB21M4 and FNM_DB21M6 had already been imported into GIS.

                //The following two versions of FNM are duplicates of each other. 
                //caiso_fnm_pnm_lap	FNM_DB21M4	28-APR-2021 12:00:00 AM
                //mrtu_fnm_ciso_lap	21M4_DB104	28-APR-2021 12:00:00 AM      

                //The following two versions of FNM are duplicates of each other. 
                //caiso_fnm_pnm_lap	FNM_DB21M6	09-JUN-2021 12:00:00 AM
                //mrtu_fnm_ciso_lap	21M6_DB105	09-JUN-2021 12:00:00 AM

                string sql = @"with dataset1 as
					(
						select
							udc_id,
							lap_id,
							res_type,
							cnode_id,
							bus_id,
							--fnm_version fnm_version1,
							case fnm_version
								when '21M4_DB104' then 'FNM_DB21M4' -- corrects for duplicate versions
								when '21M6_DB105' then 'FNM_DB21M6' -- corrects for duplicate versions
								else fnm_version
							end fnm_version,
							release_date
						from 
							ze_data.caiso_fnm_pnm_lap 
						where 
							upper(udc_id) in ('PGAE', 'SCE')
							and upper(lap_id) like 'SLAP%'
							and upper(res_type) = 'LOAD'
							
						union  
						
						select
							udc udc_id,
							lap_id,
							resource_type res_type,
							cnode_id,
							bus_id,
							--fnm_version fnm_version1,
							case fnm_version
								when '21M4_DB104' then 'FNM_DB21M4' -- corrects for duplicate versions
								when '21M6_DB105' then 'FNM_DB21M6' -- corrects for duplicate versions
								else fnm_version
							end fnm_version,
							release_date
						from 
							ze_data.mrtu_fnm_ciso_lap 
						where 
							upper(udc) in ('PGAE', 'SCE')
							and upper(lap_id) like 'SLAP%'
							and upper(resource_type) = 'LOAD'
					)

						select distinct
							c.udc_id,
							c.lap_id,
							c.res_type,
							c.cnode_id,
							c.bus_id,
							f.first_fnm_version,
							f.first_fnm_release_date,
							c.latest_fnm_version,
							c.latest_fnm_release_date   
						from
						(
							select 
								a.udc_id,
								a.lap_id,
								a.res_type,
								a.cnode_id,
								a.bus_id,
								a.fnm_version latest_fnm_version,
								b.max_release_date latest_fnm_release_date
							from dataset1 a
							inner join
							(
								select
									lap_id,
									cnode_id,
									bus_id,
									max(release_date) max_release_date
								from dataset1 
								group by 
									lap_id,
									cnode_id,
									bus_id
							) b
							on 
								a.lap_id = b.lap_id
								and a.cnode_id = b.cnode_id 
								and a.bus_id = b.bus_id 
								and a.release_date = b.max_release_date
						) c  
						inner join     
						(  
							select 
								d.lap_id,
								d.cnode_id,
								d.bus_id,
								d.fnm_version first_fnm_version,
								e.min_release_date first_fnm_release_date
							from dataset1 d
							inner join
							(
								select
									lap_id,
									cnode_id,
									bus_id,
									min(release_date) min_release_date
								from 
									dataset1
								group by 
									lap_id,
									cnode_id,
									bus_id
							) e
							on 
								d.lap_id = e.lap_id
								and d.cnode_id = e.cnode_id 
								and d.bus_id = e.bus_id 
								and d.release_date = e.min_release_date
						) f    
						on 
							c.lap_id = f.lap_id
							and c.cnode_id = f.cnode_id
							and c.bus_id = f.bus_id
						order by 
							c.udc_id,
							c.cnode_id,
							f.first_fnm_release_date";
                #endregion
                //above code added for project ARET Substation -  End


                var dataTable = new DataTable();
                OracleCommand cmd = new OracleCommand(sql, mdrConn);
                cmd.CommandType = CommandType.Text;

                //**Read the data from MDR-table into data-table.
                OracleDataReader dataReader = cmd.ExecuteReader();

                //**Insert data from data-table into GIS_FNM_LAP_TEMP table, one INSERT at a time.
                if (dataReader.HasRows)
                {
                    dataTable.Load(dataReader);
                    MDR_FNM_TableToTable(dataTable, gisOracleCon);
                }
                else
                {
                    _logger.Info(mdrConn.DataSource + " returned ZERO data.");
                    return false;
                }
                return true;

            }
            catch (Exception Ex)
            { _logger.Error(Ex.Message); throw Ex; }
        }

        /// <summary>
        /// Step 1. part two of step 1.
        /// This takes the data from data table in memory, and writes the data to GIS_FNM_LAP_TEMP table.
        /// </summary>
        /// <param name="dt">The data table filled with data from MDR.</param>
        /// <param name="con">The GIS connection the data will be written to.</param>
        private void MDR_FNM_TableToTable(DataTable dt, OracleConnection con)
        {
            try
            {
                string finalSql = "";
                string UDC_IDFldName = "UDC_ID";
                string LAP_IDFldName = "LAP_ID";
                string RES_TYPEFldName = "RES_TYPE";
                string CNODE_IDFldName = "CNODE_ID";
                string BUS_IDFldname = "BUS_ID";
                string FIRST_FNM_VERSIONFldname = "FIRST_FNM_VERSION";
                string FIRST_FNM_RELEASE_DATEFldName = "FIRST_FNM_RELEASE_DATE";
                string LATEST_FNM_VERSIONFldName = "LATEST_FNM_VERSION";
                string LATEST_FNM_RELEASE_DATEFldName = "LATEST_FNM_RELEASE_DATE";

                string udcID = null;
                string lapID = null;
                string resType = null;
                string cnodeId = null;
                string busID = null;
                string fstFnmVer = null;
                string fstFnmRelDt = null;
                string latestFnmVer = null;
                string latestFnmDt = null;
                List<string> cols = new List<string>() { UDC_IDFldName, LAP_IDFldName, RES_TYPEFldName, CNODE_IDFldName, BUS_IDFldname, FIRST_FNM_VERSIONFldname, FIRST_FNM_RELEASE_DATEFldName, LATEST_FNM_VERSIONFldName, LATEST_FNM_RELEASE_DATEFldName };
                string sqlFristpart = string.Format("INSERT INTO " + _GIS_FNM_LAP_TEMPtblName + "({0}", string.Join(", ", cols.ToArray()));


                //**Insert data from data-table into GIS_FNM_LAP_TEMP table, one INSERT at a time.
                int inserts = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    udcID = dt.Rows[i][UDC_IDFldName].ToString();
                    lapID = dt.Rows[i][LAP_IDFldName].ToString();
                    resType = dt.Rows[i][RES_TYPEFldName].ToString();
                    cnodeId = dt.Rows[i][CNODE_IDFldName].ToString();
                    busID = dt.Rows[i][BUS_IDFldname].ToString();
                    fstFnmVer = dt.Rows[i][FIRST_FNM_VERSIONFldname].ToString();

                    //_logger.Info("fstFnmVer in datatable  " + dt.Rows[i][LATEST_FNM_RELEASE_DATEFldName].ToString());
                    if (dt.Rows[i][LATEST_FNM_RELEASE_DATEFldName].ToString() != string.Empty)
                        latestFnmDt = DateTime.Parse((dt.Rows[i][LATEST_FNM_RELEASE_DATEFldName].ToString())).ToString("MM/dd/yyyy hh:mm:ss tt");
                    else
                        latestFnmDt = string.Empty;

                    if (dt.Rows[i][FIRST_FNM_RELEASE_DATEFldName].ToString() != string.Empty)
                        fstFnmRelDt = DateTime.Parse((dt.Rows[i][FIRST_FNM_RELEASE_DATEFldName].ToString())).ToString("MM/dd/yyyy hh:mm:ss tt");
                    else
                        fstFnmRelDt = string.Empty;

                    //commented on 04/19/2022

                    //fstFnmRelDt = dt.Rows[i][FIRST_FNM_RELEASE_DATEFldName].ToString();
                    //latestFnmDt = dt.Rows[i][LATEST_FNM_RELEASE_DATEFldName].ToString();

                    latestFnmVer = dt.Rows[i][LATEST_FNM_VERSIONFldName].ToString();


                    //finalSql = "INSERT INTO GIS_FNM_LAP_TEMP(UDC_ID,LAP_ID,RES_TYPE,CNODE_ID,BUS_ID,FIRST_FNM_VERSION,FIRST_FNM_RELEASE_DATE,LATEST_FNM_VERSION,LATEST_FNM_RELEASE_DATE) VALUES(:UDC_ID,:LAP_ID,:RES_TYPE,:CNODE_ID,:BUS_ID,:FIRST_FNM_VERSION,:FIRST_FNM_RELEASE_DATE,:LATEST_FNM_VERSION,:LATEST_FNM_RELEASE_DATE) ";
                    //finalSql = "INSERT INTO GIS_FNM_LAP_TEMP(UDC_ID,LAP_ID,RES_TYPE,CNODE_ID,BUS_ID,FIRST_FNM_VERSION,FIRST_FNM_RELEASE_DATE,LATEST_FNM_VERSION,LATEST_FNM_RELEASE_DATE) VALUES('" +
                    finalSql = sqlFristpart + ") VALUES('" +
                    udcID + "', '" +
                    lapID + "', '" +
                    resType + "', '" +
                    cnodeId + "', '" +
                    busID + "', '" +
                    fstFnmVer + "',    TO_DATE('" + fstFnmRelDt + "', 'MM/DD/YYYY HH:MI:SS AM'), '" +
                    latestFnmVer + "', TO_DATE('" + latestFnmDt + "', 'MM/DD/YYYY HH:MI:SS AM'))";

                    DBUtils.ExecuteSql(con, finalSql, false);
                    //for (int j = 0; j < dt.Columns.Count; j++)
                    //{
                    //    colValue.Add(dt.Rows[i][j].ToString());
                    //}
                    //InsertWithParams(con, finalSql, colValue, cols, out error);
                    inserts++;
                    //colValue.Clear();
                }
                _logger.Info("Number of rows inserted in " + _GIS_FNM_LAP_TEMPtblName + " table:  " + inserts.ToString());
            }
            catch (Exception Ex)
            { _logger.Debug(Ex.Message); throw Ex; }
        }

        /// <summary>
        /// Step 4. Clear GIS_FNM_LAP_TEMP table (delete all the data).
        /// </summary>
        /// <param name="con">The GIS connection the data will be deleted from.</param>
        private void DeleteAllFromTable(OracleConnection con, string tableName)
        {
            try
            {
                //_logger.Info("clearing from  " + _GIS_FNM_LAP_TEMPtblName + " table...");
                _logger.Info("clearing from  " + tableName + " table...");

                //string sqlFinal = "DELETE FROM " + _GIS_FNM_LAP_TEMPtblName;
                string sqlFinal = "DELETE FROM " + tableName;
                DBUtils.ExecuteSql(con, sqlFinal);
            }
            catch (Exception Ex)
            { _logger.Debug(Ex.Message); throw Ex; }
        }


        ///<summary>
        /// Move data from MDR to GIS_SUBLAP_LCA table.
        /// This is executed as SQL statement within the code.
        /// </summary>
        /// <param name="mdrConn">The MDR connection the data will read from.</param>
        /// <param name="gisOracleCon">The GIS connection the data will be written to.</param>
        private bool Load_MDR_To_GIS_SUBLAP_LCA(OracleConnection mdrConn, OracleConnection gisOracleCon)
        {
            try
            {

                //Below code commented for project ARET Substation - changes done on 25 JUly 2022 by v1t8
                //This changes suggested by PNode Business owner steav : Start

                #region MDR-LCA-Query
                //string sql = @"Select * from EDGIS.MDR_SUBLAP_LCA";
                //  string sql = @"Select * from ZE_DATA.CAISO_SUB_LAP_LCL_CAP_A";

                #endregion

                //above code commented for project ARET Substation -  End

                //Below code added for project ARET Substation - changes done on 25 JUly 2022 by v1t8
                //This changes suggested by PNode Business owner steav : Start
                #region MDR-LCA-Query
                string sql = @"select udc_id, lap_id, local_cap_area, start_date, 
		case end_date
            when cast('31-DEC-9999 12:00:00.000000000 AM' as timestamp) then end_date

            else end_date + interval '1' day
        end end_date,
		create_user, update_user, date_inserted, date_updated
        from ze_data.caiso_sub_lap_lcl_cap_a";
                #endregion


                //above code added for project ARET Substation -  End

                _logger.Info("SQL: " + sql);
                var dataTable = new DataTable();
                OracleCommand cmd = new OracleCommand(sql, mdrConn);
                cmd.CommandType = CommandType.Text;

                //**Read the data from MDR-table into data-table.
                OracleDataReader dataReader = cmd.ExecuteReader();

                //**Insert data from data-table into GIS_SUBLAP_LCA table, one INSERT at a time.
                if (dataReader.HasRows)
                {
                    dataTable.Load(dataReader);
                    MDR_LCA_TableToTable(dataTable, gisOracleCon);
                }
                else
                {
                    _logger.Info(mdrConn.DataSource + " returned ZERO data.");
                    return false;
                }
                return true;
            }
            catch (Exception Ex)
            { _logger.Error(Ex.Message); throw Ex; }
        }
        /// <summary>
        /// This takes the data from data table in memory, and writes the data to GIS SUBLAP LCA table.
        /// </summary>
        /// <param name="dt">The data table filled with data from MDR.</param>
        /// <param name="con">The GIS connection the data will be written to.</param>
        private void MDR_LCA_TableToTable(DataTable dt, OracleConnection con)
        {
            try
            {
                string finalSql = "";
                string UDC_IDFldName = "UDC_ID";
                string LAP_IDFldName = "LAP_ID";
                string LCA_NAMEFldName = "LCA_NAME";
                string START_DATEFldName = "START_DATE";
                string END_DATEFldname = "END_DATE";
                string CREATE_DATEFldname = "CREATE_DATE";
                string CREATED_BYFldName = "CREATED_BY";
                string UPDATE_DATEFldName = "UPDATE_DATE";
                string UPDATED_BYFldName = "UPDATED_BY";

                string LCA_NAMEFldName_MDR = "LOCAL_CAP_AREA";
                string CREATE_DATEFldname_MDR = "DATE_INSERTED";
                string END_DATEFldname_MDR = "END_DATE";
                string CREATED_BYFldName_MDR = "CREATE_USER";
                string UPDATE_DATEFldName_MDR = "DATE_UPDATED";
                string UPDATED_BYFldName_MDR = "UPDATE_USER";

                string udcID = null;
                string lapID = null;
                string lcaName = null;
                string startDate = null;
                string endDate = null;
                string createDate = null;
                string createdBy = null;
                string updateDate = null;
                string updatedBy = null;
                List<string> cols = new List<string>() { UDC_IDFldName, LAP_IDFldName, LCA_NAMEFldName, START_DATEFldName, END_DATEFldname, CREATE_DATEFldname, CREATED_BYFldName, UPDATE_DATEFldName, UPDATED_BYFldName };
                string sqlFristpart = string.Format("INSERT INTO " + _GIS_SUBLAP_LCAtblName + "({0}", string.Join(", ", cols.ToArray()));


                //**Insert data from data-table into GIS LCA table, one INSERT at a time.
                int inserts = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    udcID = dt.Rows[i][UDC_IDFldName].ToString();
                    lapID = dt.Rows[i][LAP_IDFldName].ToString();
                    lcaName = dt.Rows[i][LCA_NAMEFldName_MDR].ToString();
                    // startDate =DateTime.Parse((dt.Rows[i][START_DATEFldName].ToString())).ToString("mm/dd/yyyy hh:mi:ss am");
                    // _logger.Info("startDate in datatable  " + dt.Rows[i][START_DATEFldName].ToString());
                    if (dt.Rows[i][START_DATEFldName].ToString() != string.Empty)
                        startDate = DateTime.Parse((dt.Rows[i][START_DATEFldName].ToString())).ToString("MM/dd/yyyy hh:mm:ss tt");
                    else
                        startDate = string.Empty;
                    //_logger.Info("startDate " + startDate);

                    //Below code commented for project ARET Substation - changes done on 11 Aug  2022 by v1t8
                    //This changes suggested by PNode Business owner steav : Start
                    //if (dt.Rows[i][CREATE_DATEFldname_MDR].ToString() != string.Empty)
                    //    endDate = DateTime.Parse((dt.Rows[i][CREATE_DATEFldname_MDR].ToString())).ToString("MM/dd/yyyy hh:mm:ss tt");

                    if (dt.Rows[i][END_DATEFldname_MDR].ToString() != string.Empty)
                        endDate = DateTime.Parse((dt.Rows[i][END_DATEFldname_MDR].ToString())).ToString("MM/dd/yyyy hh:mm:ss tt");
                    else
                        endDate = string.Empty;
                    
                    if (dt.Rows[i][CREATE_DATEFldname_MDR].ToString() != string.Empty)
                        createDate = DateTime.Parse((dt.Rows[i][CREATE_DATEFldname_MDR].ToString())).ToString("MM/dd/yyyy hh:mm:ss tt");
                    else
                        createDate = string.Empty;

                    if (dt.Rows[i][UPDATE_DATEFldName_MDR].ToString() != string.Empty)
                        updateDate = DateTime.Parse((dt.Rows[i][UPDATE_DATEFldName_MDR].ToString())).ToString("MM/dd/yyyy hh:mm:ss tt");
                    else
                        updateDate = string.Empty;
                    //comment by kaleem 04/19
                    //startDate = dt.Rows[i][START_DATEFldName].ToString();
                    //endDate = dt.Rows[i][END_DATEFldname].ToString();
                    //createDate = dt.Rows[i][CREATE_DATEFldname_MDR].ToString();
                    createdBy = dt.Rows[i][CREATED_BYFldName_MDR].ToString();
                    //updateDate = dt.Rows[i][UPDATE_DATEFldName_MDR].ToString();
                    updatedBy = dt.Rows[i][UPDATED_BYFldName_MDR].ToString();

                    //Prepare insert statement.
                    finalSql = sqlFristpart + ") VALUES('" +
                    udcID + "', '" +
                    lapID + "', '" +
                    lcaName + "', " +
                    "TO_DATE('" + startDate + "', 'MM/DD/YYYY HH:MI:SS AM'), " +
                    "TO_DATE('" + endDate + "', 'MM/DD/YYYY HH:MI:SS AM'), " +
                    "TO_DATE('" + createDate + "', 'MM/DD/YYYY HH:MI:SS AM'), '" +
                    createdBy + "', TO_DATE('" + updateDate + "', 'MM/DD/YYYY HH:MI:SS AM'), '" +
                    updatedBy + "')";

                    DBUtils.ExecuteSql(con, finalSql, false);
                    //  _logger.Info("Inserted:"+ inserts.ToString());
                    inserts++;
                }
                _logger.Info("Number of rows inserted in " + _GIS_SUBLAP_LCAtblName + " table:  " + inserts.ToString());
            }
            catch (Exception Ex)
            { _logger.Debug(Ex.Message); throw Ex; }
        }

        /// <summary>
        /// Close connections.
        /// </summary>
        internal void Close()
        {
            try
            {
                _logger.Info("Closing connections...");
                Close(_gisOracleCon);
                Close(_mdrOracleCon);
            }
            catch (Exception Ex)
            { _logger.Error(Ex.Message); }
        }
        private void Close(OracleConnection con)
        {
            try
            {
                if (con != null)
                    if (con.State != ConnectionState.Closed)
                        con.Close();
                con.Dispose();
            }
            catch (Exception ex) { }
        }
    }
}
