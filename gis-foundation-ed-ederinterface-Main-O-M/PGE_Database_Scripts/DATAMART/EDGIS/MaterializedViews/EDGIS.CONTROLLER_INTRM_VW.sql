Prompt drop Materialized View CONTROLLER_INTRM_VW;
DROP MATERIALIZED VIEW EDGIS.CONTROLLER_INTRM_VW
/
Prompt Materialized View CONTROLLER_INTRM_VW;
--
-- CONTROLLER_INTRM_VW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.CONTROLLER_INTRM_VW (GLOBALID,CEDSADEVICEID,CIRCUITID,CITY,COUNTY, DIVISION,GEMSCIRCUITMAPNUM,LOCALOFFICEID,LOCDESC,MANUFACTURER, MATERIALCODE,OPERATINGNUMBER,REGION,X,Y, YEARMANUFACTURED,ZIP,SAPEQUIPID,GEMSDISTMAPNUM,CUSTOMEROWNED, DEVICETYPE) TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:30 PM (QP5 v5.313) */
(SELECT u.GLOBALID,
        r.CEDSADeviceID,
        r.CIRCUITID,
        r.CITY,
        r.COUNTY,
        r.DIVISION,
        r.gemscircuitmapnum,
        r.LocalOfficeID,
        r.locdesc,
        NULL        Manufacturer,
        NULL        MaterialCode,
        r.OperatingNumber,
        r.REGION,
        r.latitude  X,
        r.longitude Y,
        NULL        YearManufactured,
        r.Zip,
        u.sapequipid,
        r.GEMSDISTMAPNUM,
        r.CUSTOMEROWNED,
        'REGL'      DeviceType
   FROM edgis.zz_mv_voltageregulator r, edgis.zz_mv_voltageregulatorunit u
  WHERE r.globalid = u.regulatorguid
 UNION ALL
 SELECT GLOBALID,
        CEDSADeviceID,
        CIRCUITID,
        CITY,
        COUNTY,
        DIVISION,
        gemscircuitmapnum,
        LocalOfficeID,
        locdesc,
        Manufacturer,
        MaterialCode,
        OperatingNumber,
        REGION,
        latitude  X,
        longitude Y,
        YearManufactured,
        Zip,
        sapequipid,
        GEMSDISTMAPNUM,
        CUSTOMEROWNED,
        'CAPA'
   FROM edgis.zz_mv_capacitorbank
 UNION ALL
 SELECT GLOBALID,
        CEDSADeviceID,
        CIRCUITID,
        CITY,
        COUNTY,
        DIVISION,
        gemscircuitmapnum,
        LocalOfficeID,
        locdesc,
        Manufacturer,
        MaterialCode,
        OperatingNumber,
        REGION,
        latitude  X,
        longitude Y,
        YearManufactured,
        Zip,
        sapequipid,
        GEMSDISTMAPNUM,
        CUSTOMEROWNED,
        DECODE (subtypecd,  2, 'INTR',  3, 'RECL',  8, 'SECT',  'UNKN')
   FROM edgis.zz_mv_dynamicprotectivedevice
 UNION ALL
 SELECT GLOBALID,
        CEDSADeviceID,
        CIRCUITID,
        CITY,
        COUNTY,
        DIVISION,
        gemscircuitmapnum,
        LocalOfficeID,
        locdesc,
        Manufacturer,
        MaterialCode,
        OperatingNumber,
        REGION,
        latitude  X,
        longitude Y,
        YearManufactured,
        Zip,
        sapequipid,
        GEMSDISTMAPNUM,
        CUSTOMEROWNED,
        'SWIT'
   FROM edgis.zz_mv_switch)
/


COMMENT ON MATERIALIZED VIEW EDGIS.CONTROLLER_INTRM_VW IS 'snapshot table for snapshot EDGIS.CONTROLLER_INTRM_VW'
/

Prompt Grants on MATERIALIZED VIEW CONTROLLER_INTRM_VW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.CONTROLLER_INTRM_VW TO GIS_SAP_RECON
/
