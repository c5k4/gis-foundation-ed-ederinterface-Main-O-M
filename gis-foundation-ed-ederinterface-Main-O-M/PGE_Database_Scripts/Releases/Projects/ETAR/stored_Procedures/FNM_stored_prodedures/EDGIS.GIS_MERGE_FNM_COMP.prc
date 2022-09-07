--This merges the records from MDR( using GIS_FNM_LAP_TEMP table as temp for ease of processing), into FNM_COMPLETE table.
--------------------------------------------------------
--  File created - Wednesday-July-31-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure GIS_MERGE_FNM_COMP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE EDGIS.GIS_MERGE_FNM_COMP AS 
BEGIN
  MERGE INTO EDGIS.FNM_COMPLETE TARGET
   USING EDGIS.GIS_FNM_LAP_TEMP SOURCE
   on 
    (
      target.lap_id = source.lap_id
      and TARGET.cnode_id = SOURCE.cnode_id 
      and TARGET.bus_id = SOURCE.bus_id
      AND
          (
            TARGET.udc_id <> SOURCE.udc_id
            or TARGET.res_type <> SOURCE.res_type
            or TARGET.latest_fnm_version <> SOURCE.latest_fnm_version
            or TARGET.latest_fnm_releasedate <> SOURCE.latest_fnm_release_date  
          )     
    )
when matched 
   then update 
        set 
          TARGET.udc_id = SOURCE.udc_id,
          TARGET.res_type = SOURCE.res_type, 
          TARGET.latest_fnm_version = SOURCE.latest_fnm_version,
          TARGET.latest_fnm_releasedate = SOURCE.latest_fnm_release_date,
          TARGET.DATEMODIFIED = sysdate,
          TARGET.MODIFYUSER = 'ED_GIS_USER';        
  COMMIT;
END GIS_MERGE_FNM_COMP;
/
