--------------------------------------------------------
--  File created - Wednesday-July-31-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure GIS_INSERT_FNM_COMP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE EDGIS.GIS_INSERT_FNM_COMP AS 
BEGIN
  commit;
  insert into EDGIS.FNM_COMPLETE
  (         
    UDC_ID,
    LAP_ID,
    RES_TYPE,
    CNODE_ID,
    BUS_ID,
    FIRST_FNM_VERSION,
    FIRST_FNM_RELEASEDATE,
    LATEST_FNM_VERSION,
    LATEST_FNM_RELEASEDATE,
    DATECREATED,
    CREATEUSER
  )          
  (
    select 
      udc_id,
      lap_id,
      res_type,
      cnode_id,
      bus_id,
      first_fnm_version,
      first_fnm_release_date,
      latest_fnm_version,
      latest_fnm_release_date,
      sysdate,
      'ED_GIS_USER'
    from EDGIS.GIS_FNM_LAP_TEMP 
      where (lap_id, cnode_id, bus_id) not in 
        (
          select lap_id, cnode_id, bus_id from EDGIS.FNM_COMPLETE
        )
  );
  commit;
END GIS_INSERT_FNM_COMP;
/