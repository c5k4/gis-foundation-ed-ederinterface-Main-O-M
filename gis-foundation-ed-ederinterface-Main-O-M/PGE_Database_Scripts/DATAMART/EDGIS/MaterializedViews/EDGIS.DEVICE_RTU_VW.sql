Prompt drop Materialized View DEVICE_RTU_VW;
DROP MATERIALIZED VIEW EDGIS.DEVICE_RTU_VW
/
Prompt Materialized View DEVICE_RTU_VW;
--
-- DEVICE_RTU_VW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.DEVICE_RTU_VW (CONTROLLERGUID,DEVICEGUID,SCADAGLOBALID,FIRMWAREVERSION,MANUFACTURER, MODELNUMBER,SERIALNUMBER,SOFTWAREVERSION,SCADATYPE,SCADA, LEASELINEACCOUNT,SCADADEVICEGUID,RADIOMODELNUM,RADIOSERIALNUM,RADIOMANUFACTURER) TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:31 PM (QP5 v5.313) */
(SELECT a.globalid                          AS CONTROLLERGUID,
        a.deviceguid                        AS DEVICEGUID,
        b.globalid                          AS SCADAGLOBALID,
        c.firmwareversion                   AS FIRMWAREVERSION,
        c.manufacturer                      AS MANUFACTURER,
        c.modelnumber                       AS MODELNUMBER,
        c.serialnumber                      AS SERIALNUMBER,
        c.softwareversion                   AS SOFTWAREVERSION,
        b.scadatype                         AS SCADATYPE,
        DECODE (b.globalid, NULL, 'N', 'Y') AS SCADA,
        b.LEASELINEACCOUNT                  AS LEASELINEACCOUNT,
        b.deviceguid                        AS SCADADEVICEGUID,
        b.RADIOMODELNUM,
        b.RADIOSERIALNUM,
        b.RADIOMANUFACTURER
   FROM edgis.zz_mv_controller      a,
        edgis.zz_mv_scada           b,
        edgis.zz_mv_remotetermunit  c
  WHERE a.deviceguid = b.deviceguid(+) AND b.globalid = c.deviceguid(+))
/


COMMENT ON MATERIALIZED VIEW EDGIS.DEVICE_RTU_VW IS 'snapshot table for snapshot EDGIS.DEVICE_RTU_VW'
/

Prompt Grants on MATERIALIZED VIEW DEVICE_RTU_VW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.DEVICE_RTU_VW TO GIS_SAP_RECON
/
