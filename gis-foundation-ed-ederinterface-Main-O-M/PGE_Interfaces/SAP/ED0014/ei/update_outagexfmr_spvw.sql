
  CREATE OR REPLACE FORCE VIEW "WEBR"."PGE_OUTAGEXFMR_SPVW" ("OBJECTID", "SHAPE", "OUTAGE_NO", "XFMR_GUID", "XFMR_ID", "CUST_AFF", "ESSENTIAL_CUST_AFF", "CRITICAL_CUST_AFF", "XFMR_KEY", "XFMR_INT_ID", "DISTRICT_NAME", "FEEDER_ID", "FEEDER_DESC", "ETOR", "OUTAGE_START_TIME", "OIS_IVR_CAUSE_DESC", "OUTAGE_LEVEL", "OUTAGE_STATUS", "CREW_STATUS_DESC", "CUR_CUST_AFF", "EQUIP_GUID", "EQUIP_TYPE", "EQUIP_NAME", "EQUIP_CAT", "HAZARD_LEVEL_CODE", "HAZARD_LEVEL_DESC", "LATITUDE", "LONGITUDE") AS 
  SELECT tbguid.objectid,pt.shape,tbguid.outage_no,pt.globalid as xfmr_guid,tbguid.XFMR_ID,tbguid.CUST_AFF,tbguid.ESSENTIAL_CUST_AFF,tbguid.CRITICAL_CUST_AFF,tbguid.XFMR_KEY,tbguid.XFMR_INT_ID,tbguid.DISTRICT_NAME,tbguid.FEEDER_ID,tbguid.FEEDER_DESC,tbguid.ETOR,tbguid.OUTAGE_START_TIME,tbguid.OIS_IVR_CAUSE_DESC,tbguid.OUTAGE_LEVEL,tbguid.OUTAGE_STATUS,tbguid.CREW_STATUS_DESC,tbguid.CUR_CUST_AFF,tbguid.EQUIP_GUID,tbguid.EQUIP_TYPE,tbguid.EQUIP_NAME,tbguid.EQUIP_CAT,tbguid.HAZARD_LEVEL_CODE,tbguid.HAZARD_LEVEL_DESC,tbguid.LATITUDE,tbguid.LONGITUDE
FROM WEBR.pge_transformer pt, WEBR.pge_outagexfmr_vw tbguid
where pt.GLOBALID=tbguid.xfmr_guid
UNION ALL
SELECT tbguid.objectid,pt1.shape,tbguid.outage_no,pt1.globalid as xfmr_guid,tbguid.XFMR_ID,tbguid.CUST_AFF,tbguid.ESSENTIAL_CUST_AFF,tbguid.CRITICAL_CUST_AFF,tbguid.XFMR_KEY,tbguid.XFMR_INT_ID,tbguid.DISTRICT_NAME,tbguid.FEEDER_ID,tbguid.FEEDER_DESC,tbguid.ETOR,tbguid.OUTAGE_START_TIME,tbguid.OIS_IVR_CAUSE_DESC,tbguid.OUTAGE_LEVEL,tbguid.OUTAGE_STATUS,tbguid.CREW_STATUS_DESC,tbguid.CUR_CUST_AFF,tbguid.EQUIP_GUID,tbguid.EQUIP_TYPE,tbguid.EQUIP_NAME,tbguid.EQUIP_CAT,tbguid.HAZARD_LEVEL_CODE,tbguid.HAZARD_LEVEL_DESC,tbguid.LATITUDE,tbguid.LONGITUDE
FROM WEBR.pge_transformer pt1, WEBR.pge_outagexfmr_vw tbguid
where tbguid.xfmr_guid is null and pt1.CGC12=tbguid.XFMR_INT_ID;

grant select on "WEBR"."PGE_OUTAGEXFMR_SPVW" to gis_i;
