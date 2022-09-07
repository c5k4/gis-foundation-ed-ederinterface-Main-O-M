--Updates LCA_ID field of FNM table, from SUBLAP_LCA, by joinging on LAP_ID.
--------------------------------------------------------
--  File created - Wednesday-July-31-2019   
--------------------------------------------------------
--------------------------------------------------------
--  DDL for Procedure GIS_UPDATE_FNM_LCA_ID
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE EDGIS.GIS_UPDATE_FNM_LCA_ID AS 
BEGIN
 MERGE INTO EDGIS.FNM TARGET
   USING EDGIS.GIS_SUBLAP_LCA SOURCE
   on 
    (
      TARGET.LAP_ID = SOURCE.LAP_ID 
      AND
          (
            SOURCE.start_date <= sysdate AND
            (SOURCE.end_date is null OR SOURCE.end_date >=sysdate)
          )     
    )
when matched 
   then update 
        set 
          TARGET.LCA_ID = SOURCE.LCA_NAME;        
  COMMIT;
END GIS_UPDATE_FNM_LCA_ID;
/