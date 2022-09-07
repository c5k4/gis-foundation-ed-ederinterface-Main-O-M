--------------------------------------------------------
--  DDL for Procedure TRUNCATE_SAP_TABLES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRUNCATE_SAP_TABLES" 
AS
BEGIN
  EXECUTE immediate 'truncate table edgis.gis_guid';
  EXECUTE immediate 'truncate table edgis.sap_integrated_result';
  EXECUTE immediate 'truncate table edgis.sap_to_gis';
END TRUNCATE_SAP_TABLES;
