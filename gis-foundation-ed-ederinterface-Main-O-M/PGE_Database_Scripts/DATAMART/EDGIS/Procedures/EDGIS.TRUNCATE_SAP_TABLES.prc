Prompt drop Procedure TRUNCATE_SAP_TABLES;
DROP PROCEDURE EDGIS.TRUNCATE_SAP_TABLES
/

Prompt Procedure TRUNCATE_SAP_TABLES;
--
-- TRUNCATE_SAP_TABLES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.TRUNCATE_SAP_TABLES
AS
BEGIN
  EXECUTE immediate 'truncate table edgis.gis_guid';
  EXECUTE immediate 'truncate table edgis.sap_integrated_result';
  EXECUTE immediate 'truncate table edgis.sap_to_gis';
END TRUNCATE_SAP_TABLES;
/


Prompt Grants on PROCEDURE TRUNCATE_SAP_TABLES TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON EDGIS.TRUNCATE_SAP_TABLES TO GIS_I
/
