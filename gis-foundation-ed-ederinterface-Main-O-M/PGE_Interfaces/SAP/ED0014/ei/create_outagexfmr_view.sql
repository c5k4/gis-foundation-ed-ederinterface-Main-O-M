
  CREATE OR REPLACE FORCE VIEW "WEBR"."PGE_OUTAGEXFMR_VW" ("OBJECTID", "OUTAGE_NO", "XFMR_GUID", "XFMR_ID", "CUST_AFF", "ESSENTIAL_CUST_AFF", "CRITICAL_CUST_AFF", "XFMR_KEY", "XFMR_INT_ID", "DISTRICT_NAME", "FEEDER_ID", "FEEDER_DESC", "ETOR", "OUTAGE_START_TIME", "OIS_IVR_CAUSE_DESC", "OUTAGE_LEVEL", "OUTAGE_STATUS", "CREW_STATUS_DESC", "CUR_CUST_AFF", "EQUIP_GUID", "EQUIP_TYPE", "EQUIP_NAME", "EQUIP_CAT", "HAZARD_LEVEL_CODE", "HAZARD_LEVEL_DESC", "LATITUDE", "LONGITUDE") AS 
  SELECT 
    PGE_OUTAGETRANSFORMER.OBJECTID OBJECTID, 
    PGE_OUTAGETRANSFORMER.OUTAGE_NO OUTAGE_NO, 
    PGE_OUTAGETRANSFORMER.XFMR_GUID XFMR_GUID, 
    PGE_OUTAGETRANSFORMER.XFMR_ID XFMR_ID, 
    PGE_OUTAGETRANSFORMER.CUST_AFF CUST_AFF, 
    PGE_OUTAGETRANSFORMER.ESSENTIAL_CUST_AFF ESSENTIAL_CUST_AFF, 
    PGE_OUTAGETRANSFORMER.CRITICAL_CUST_AFF CRITICAL_CUST_AFF, 
    PGE_OUTAGETRANSFORMER.XFMR_KEY XFMR_KEY, 
    PGE_OUTAGETRANSFORMER.XFMR_INT_ID XFMR_INT_ID, 
    PGE_OUTAGENOTIFICATION.DISTRICT_NAME DISTRICT_NAME, 
    PGE_OUTAGENOTIFICATION.FEEDER_ID FEEDER_ID, 
    PGE_OUTAGENOTIFICATION.FEEDER_DESC FEEDER_DESC, 
    PGE_OUTAGENOTIFICATION.ETOR ETOR, 
    PGE_OUTAGENOTIFICATION.OUTAGE_START_TIME OUTAGE_START_TIME, 
    PGE_OUTAGENOTIFICATION.OIS_IVR_CAUSE_DESC OIS_IVR_CAUSE_DESC, 
    PGE_OUTAGENOTIFICATION.OUTAGE_LEVEL OUTAGE_LEVEL, 
    PGE_OUTAGENOTIFICATION.OUTAGE_STATUS OUTAGE_STATUS, 
    PGE_OUTAGENOTIFICATION.CREW_STATUS_DESC CREW_STATUS_DESC, 
    PGE_OUTAGENOTIFICATION.CUR_CUST_AFF CUR_CUST_AFF, 
    PGE_OUTAGENOTIFICATION.EQUIP_GUID EQUIP_GUID, 
    PGE_OUTAGENOTIFICATION.EQUIP_TYPE EQUIP_TYPE, 
    PGE_OUTAGENOTIFICATION.EQUIP_NAME EQUIP_NAME, 
    PGE_OUTAGENOTIFICATION.EQUIP_CAT EQUIP_CAT, 
    PGE_OUTAGENOTIFICATION.HAZARD_LEVEL_CODE HAZARD_LEVEL_CODE, 
    PGE_OUTAGENOTIFICATION.HAZARD_LEVEL_DESC HAZARD_LEVEL_DESC, 
    PGE_OUTAGENOTIFICATION.LATITUDE LATITUDE, 
    PGE_OUTAGENOTIFICATION.LONGITUDE LONGITUDE 
FROM 
    PGE_OUTAGETRANSFORMER, 
    PGE_OUTAGENOTIFICATION 
WHERE 
    PGE_OUTAGETRANSFORMER.OUTAGE_NO = PGE_OUTAGENOTIFICATION.OUTAGE_NO;
/
GRANT ALL ON "WEBR"."PGE_OUTAGEXFMR_VW" TO GIS_I;