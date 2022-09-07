--------------------------------------------------------
--  DDL for Procedure PGE_INSERT_IN_STAGE_ARCHIVE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."PGE_INSERT_IN_STAGE_ARCHIVE" AS
BEGIN

  --INSERT into Summary Archive from Summary Stage
  insert into PGEDATA.GEN_SUMMARY_ARCHIVE(GUID,ACTION,SERVICE_POINT_ID,SAP_EGI_NOTIFICATION,PROJECT_NAME,PROGRAM_TYPE,POWER_SOURCE,EFF_RATING_MACH_KW,EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_INV_KVA,MAX_STORAGE_CAPACITY,CHARGE_DEMAND_KW,STATUS,OBJECTID,STATUS_MESSAGE,UPDATED_IN_MAIN,UPDATED_IN_ED_MAIN,UPDATED_IN_SETTINGS,CEDSA_MATCH_FOUND,ARCHIVED_DATE)
select GUID,ACTION,SERVICE_POINT_ID,SAP_EGI_NOTIFICATION,PROJECT_NAME,PROGRAM_TYPE,POWER_SOURCE,EFF_RATING_MACH_KW,EFF_RATING_INV_KW,EFF_RATING_MACH_KVA,EFF_RATING_INV_KVA,MAX_STORAGE_CAPACITY,CHARGE_DEMAND_KW,STATUS,OBJECTID,STATUS_MESSAGE,UPDATED_IN_MAIN,UPDATED_IN_ED_MAIN,UPDATED_IN_SETTINGS,CEDSA_MATCH_FOUND, sysdate as ARCHIVED_DATE from PGEDATA.GEN_SUMMARY_STAGE;
commit;

--INSERT into Equipment archive from Equipment Stage Table
INSERT INTO PGEDATA.GEN_EQUIPMENT_ARCHIVE (OBJECTID,ACTION,SERVICE_POINT_ID,GUID,SAP_EGI_NOTIFICATION,PROJECT_NAME,SAP_QUEUE_NUMBER,GEN_TECH_CD,POWER_SOURCE,MANUFACTURER,MODEL,PTC_RATING,INVERTER_EFFICIENCY,NAMEPLATE_RATING,QUANTITY,POWER_FACTOR,EFF_RATING_KW,EFF_RATING_KVA,RATED_VOLTAGE,NUMBER_OF_PHASES,MODE_OF_INV,GEN_TECH_EQUIPMENT,MAX_STORAGE_CAPACITY,RATED_DISCHARGE,CHARGE_DEMAND_KW,GRID_CHARGED,SS_REACTANCE,SS_RESISTANCE,TRANS_REACTANCE,TRANS_RESISTANCE,SUBTRANS_REACTANCE,SUBTRANS_RESISTANCE,NEG_REACTANCE,NEG_RESISTANCE,ZERO_REACTANCE,ZERO_RESISTANCE,GRD_REACTANCE,GRD_RESISTANCE,PTO_DATE,PROGRAM_TYPE,STATUS,SAP_EQUIPMENT_ID,STATUS_MESSAGE,ENOS_EQUIP_REF,ENOS_PROJ_REF,CERTIFICATION,UPDATED_IN_SETTINGS,CEDSA_MATCH_FOUND,TECH_TYPE_CD,NAMEPLATE_CAPACITY,ARCHIVED_DATE)
select OBJECTID,ACTION,SERVICE_POINT_ID,GUID,SAP_EGI_NOTIFICATION,PROJECT_NAME,SAP_QUEUE_NUMBER,GEN_TECH_CD,POWER_SOURCE,MANUFACTURER,MODEL,PTC_RATING,INVERTER_EFFICIENCY,NAMEPLATE_RATING,QUANTITY,POWER_FACTOR,EFF_RATING_KW,EFF_RATING_KVA,RATED_VOLTAGE,NUMBER_OF_PHASES,MODE_OF_INV,GEN_TECH_EQUIPMENT,MAX_STORAGE_CAPACITY,RATED_DISCHARGE,CHARGE_DEMAND_KW,GRID_CHARGED,SS_REACTANCE,SS_RESISTANCE,TRANS_REACTANCE,TRANS_RESISTANCE,SUBTRANS_REACTANCE,SUBTRANS_RESISTANCE,NEG_REACTANCE,NEG_RESISTANCE,ZERO_REACTANCE,ZERO_RESISTANCE,GRD_REACTANCE,GRD_RESISTANCE,PTO_DATE,PROGRAM_TYPE,STATUS,SAP_EQUIPMENT_ID,STATUS_MESSAGE,ENOS_EQUIP_REF,ENOS_PROJ_REF,CERTIFICATION,UPDATED_IN_SETTINGS,CEDSA_MATCH_FOUND,TECH_TYPE_CD,NAMEPLATE_CAPACITY,sysdate as ARCHIVED_DATE from PGEDATA.gen_equipment_stage
commit;

END PGE_INSERT_IN_STAGE_ARCHIVE;
