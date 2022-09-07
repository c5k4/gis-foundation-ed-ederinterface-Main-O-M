--This updates the data from FNM_COMPELTE table to FNM able.
--------------------------------------------------------
--  File created - Wednesday-July-31-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure GIS_MERGE_FNM_FROM_FNM_COMP
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE EDGIS.GIS_MERGE_FNM_FROM_FNM_COMP AS 
-- this will need to be modified or an additional sp will be needed to include lca
BEGIN
  MERGE INTO EDGIS.FNM TARGET
   USING 
   (
        select * from fnm_complete 
            where 
            latest_fnm_releasedate = (select max(latest_fnm_releasedate) from edgis.fnm_complete where latest_fnm_releasedate <= sysdate) or -- Most recent version that is less than today's date.
            --latest_fnm_releasedate > sysdate or -- All future versions
            latest_fnm_releasedate is null -- Manual overrides)
   ) SOURCE 
   on 
    (
      TARGET.cnode_id = SOURCE.cnode_id 
      and TARGET.bus_id = SOURCE.bus_id
      AND
          (
            TARGET.udc_id <> SOURCE.udc_id
            or TARGET.lap_id <> SOURCE.lap_id
            or TARGET.res_type <> SOURCE.res_type
            or TARGET.latest_fnm_version <> SOURCE.latest_fnm_version
            or TARGET.latest_fnm_releasedate <> SOURCE.latest_fnm_releasedate
          )     
    )
when matched 
   then update 
        set 
          TARGET.udc_id = SOURCE.udc_id,
          TARGET.lap_id = SOURCE.lap_id, 
          TARGET.res_type = SOURCE.res_type, 
          TARGET.latest_fnm_version = SOURCE.latest_fnm_version,
          TARGET.latest_fnm_releasedate = SOURCE.latest_fnm_releasedate,
          TARGET.DATEMODIFIED = sysdate,
          TARGET.MODIFYUSER = 'ED_GIS_USER';
  COMMIT;
END GIS_MERGE_FNM_FROM_FNM_COMP;
/