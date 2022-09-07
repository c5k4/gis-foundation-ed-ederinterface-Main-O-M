create or replace PROCEDURE EDGIS.GIS_COPY_MDSS_DATA AS
--This is for error handling case, if for some reason, 'GIS_SPP_ZONE' and 'GIS_SPP_FEEDER_SUBLAP' tables have zero records
--then copy the data from repective MDSS tables into those tables.
  BEGIN
    INSERT INTO EDGIS.GIS_SPP_ZONE
    SELECT * FROM EDGIS.MDSS_SPP_ZONE;    
    COMMIT;
    
    INSERT INTO EDGIS.GIS_SPP_FEEDER_SUBLAP
    SELECT * FROM EDGIS.MDSS_SPP_FEEDER_SUBLAP;    
    COMMIT;
END GIS_COPY_MDSS_DATA;
/

Prompt Grants on PROCEDURE EDGIS.GIS_COPY_MDSS_DATA TO GIS_SUB_MDSS_RW to GIS_SUB_MDSS_RW;
GRANT EXECUTE ON EDGIS.GIS_COPY_MDSS_DATA TO GIS_SUB_MDSS_RW
/