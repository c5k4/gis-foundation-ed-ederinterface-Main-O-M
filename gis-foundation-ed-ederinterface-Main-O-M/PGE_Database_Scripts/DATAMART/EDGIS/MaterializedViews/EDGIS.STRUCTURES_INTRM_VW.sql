Prompt drop Materialized View STRUCTURES_INTRM_VW;
DROP MATERIALIZED VIEW EDGIS.STRUCTURES_INTRM_VW
/
Prompt Materialized View STRUCTURES_INTRM_VW;
--
-- STRUCTURES_INTRM_VW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.STRUCTURES_INTRM_VW (DEVICEGUID,STRUCTUREGUID,STRUCTURENUMBER,STRUCTURENAME,DEVICETYPE) TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:36 PM (QP5 v5.313) */
(SELECT *
   FROM (SELECT a.globalid         AS DeviceGUID,
                a.structureguid,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'DEVICEGROUP'      AS DEVICETYPE
           FROM edgis.zz_mv_devicegroup a, edgis.zz_mv_supportstructure b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureguid,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'DEVICEGROUP'         AS DEVICETYPE
           FROM edgis.zz_mv_devicegroup a, edgis.zz_mv_subsurfacestructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureguid,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'DEVICEGROUP'       AS DEVICETYPE
           FROM edgis.zz_mv_devicegroup a, edgis.zz_mv_padmountstructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                b.structureGUID    AS structureGUID,
                c.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'CAPACITORBANK'    AS DEVICETYPE
           FROM edgis.zz_mv_capacitorbank     a,
                edgis.zz_mv_devicegroup       b,
                edgis.zz_mv_supportstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                a.structureGUID    AS structureGUID,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'CAPACITORBANK'    AS DEVICETYPE
           FROM edgis.zz_mv_capacitorbank a, edgis.zz_mv_supportstructure b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                b.structureGUID       AS structureGUID,
                c.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'CAPACITORBANK'       AS DEVICETYPE
           FROM edgis.zz_mv_capacitorbank        a,
                edgis.zz_mv_devicegroup          b,
                edgis.zz_mv_subsurfacestructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureGUID       AS structureGUID,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'CAPACITORBANK'       AS DEVICETYPE
           FROM edgis.zz_mv_capacitorbank        a,
                edgis.zz_mv_subsurfacestructure  b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                b.structureGUID     AS structureGUID,
                c.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'CAPACITORBANK'     AS DEVICETYPE
           FROM edgis.zz_mv_capacitorbank      a,
                edgis.zz_mv_devicegroup        b,
                edgis.zz_mv_padmountstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureGUID     AS structureGUID,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'CAPACITORBANK'     AS DEVICETYPE
           FROM edgis.zz_mv_capacitorbank a, edgis.zz_mv_padmountstructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                b.structureGUID    AS structureGUID,
                c.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'TRANSFORMER'      AS DEVICETYPE
           FROM edgis.zz_mv_transformer       a,
                edgis.zz_mv_devicegroup       b,
                edgis.zz_mv_supportstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                a.structureGUID    AS structureGUID,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'TRANSFORMER'      AS DEVICETYPE
           FROM edgis.zz_mv_transformer a, edgis.zz_mv_supportstructure b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                b.structureGUID       AS structureGUID,
                c.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'TRANSFORMER'         AS DEVICETYPE
           FROM edgis.zz_mv_transformer          a,
                edgis.zz_mv_devicegroup          b,
                edgis.zz_mv_subsurfacestructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureGUID       AS structureGUID,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'TRANSFORMER'         AS DEVICETYPE
           FROM edgis.zz_mv_transformer a, edgis.zz_mv_subsurfacestructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                b.structureGUID     AS structureGUID,
                c.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'TRANSFORMER'       AS DEVICETYPE
           FROM edgis.zz_mv_transformer        a,
                edgis.zz_mv_devicegroup        b,
                edgis.zz_mv_padmountstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureGUID     AS structureGUID,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'TRANSFORMER'       AS DEVICETYPE
           FROM edgis.zz_mv_transformer a, edgis.zz_mv_padmountstructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                b.structureGUID    AS structureGUID,
                c.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'SWITCH'           AS DEVICETYPE
           FROM edgis.zz_mv_switch            a,
                edgis.zz_mv_devicegroup       b,
                edgis.zz_mv_supportstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                a.structureGUID    AS structureGUID,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'SWITCH'           AS DEVICETYPE
           FROM edgis.zz_mv_switch a, edgis.zz_mv_supportstructure b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                b.structureGUID       AS structureGUID,
                c.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'SWITCH'              AS DEVICETYPE
           FROM edgis.zz_mv_switch               a,
                edgis.zz_mv_devicegroup          b,
                edgis.zz_mv_subsurfacestructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureGUID       AS structureGUID,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'SWITCH'              AS DEVICETYPE
           FROM edgis.zz_mv_switch a, edgis.zz_mv_subsurfacestructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                b.structureGUID     AS structureGUID,
                c.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'SWITCH'            AS DEVICETYPE
           FROM edgis.zz_mv_switch             a,
                edgis.zz_mv_devicegroup        b,
                edgis.zz_mv_padmountstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureGUID     AS structureGUID,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'SWITCH'            AS DEVICETYPE
           FROM edgis.zz_mv_switch a, edgis.zz_mv_padmountstructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                b.structureGUID    AS structureGUID,
                c.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'VOLTAGEREGULATOR' AS DEVICETYPE
           FROM edgis.zz_mv_VoltageRegulator  a,
                edgis.zz_mv_devicegroup       b,
                edgis.zz_mv_supportstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                a.structureGUID    AS structureGUID,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'VOLTAGEREGULATOR' AS DEVICETYPE
           FROM edgis.zz_mv_VoltageRegulator  a,
                edgis.zz_mv_supportstructure  b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                b.structureGUID       AS structureGUID,
                c.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'VOLTAGEREGULATOR'    AS DEVICETYPE
           FROM edgis.zz_mv_VoltageRegulator     a,
                edgis.zz_mv_devicegroup          b,
                edgis.zz_mv_subsurfacestructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureGUID       AS structureGUID,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'VOLTAGEREGULATOR'    AS DEVICETYPE
           FROM edgis.zz_mv_VoltageRegulator     a,
                edgis.zz_mv_subsurfacestructure  b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                b.structureGUID     AS structureGUID,
                c.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'VOLTAGEREGULATOR'  AS DEVICETYPE
           FROM edgis.zz_mv_VoltageRegulator   a,
                edgis.zz_mv_devicegroup        b,
                edgis.zz_mv_padmountstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureGUID     AS structureGUID,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'VOLTAGEREGULATOR'  AS DEVICETYPE
           FROM edgis.zz_mv_VoltageRegulator   a,
                edgis.zz_mv_padmountstructure  b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                b.structureGUID    AS structureGUID,
                c.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'FAULTINDICATOR'   AS DEVICETYPE
           FROM edgis.zz_mv_FaultIndicator    a,
                edgis.zz_mv_devicegroup       b,
                edgis.zz_mv_supportstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                a.structureGUID    AS structureGUID,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'FAULTINDICATOR'   AS DEVICETYPE
           FROM edgis.zz_mv_FaultIndicator a, edgis.zz_mv_supportstructure b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                b.structureGUID       AS structureGUID,
                c.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'FAULTINDICATOR'      AS DEVICETYPE
           FROM edgis.zz_mv_FaultIndicator       a,
                edgis.zz_mv_devicegroup          b,
                edgis.zz_mv_subsurfacestructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureGUID       AS structureGUID,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'FAULTINDICATOR'      AS DEVICETYPE
           FROM edgis.zz_mv_FaultIndicator       a,
                edgis.zz_mv_subsurfacestructure  b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                b.structureGUID     AS structureGUID,
                c.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'FAULTINDICATOR'    AS DEVICETYPE
           FROM edgis.zz_mv_FaultIndicator     a,
                edgis.zz_mv_devicegroup        b,
                edgis.zz_mv_padmountstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureGUID     AS structureGUID,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'FAULTINDICATOR'    AS DEVICETYPE
           FROM edgis.zz_mv_FaultIndicator a, edgis.zz_mv_padmountstructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                b.structureGUID    AS structureGUID,
                c.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'STEPDOWN'         AS DEVICETYPE
           FROM edgis.zz_mv_StepDown          a,
                edgis.zz_mv_devicegroup       b,
                edgis.zz_mv_supportstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                a.structureGUID    AS structureGUID,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'STEPDOWN'         AS DEVICETYPE
           FROM edgis.zz_mv_StepDown a, edgis.zz_mv_supportstructure b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                b.structureGUID       AS structureGUID,
                c.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'STEPDOWN'            AS DEVICETYPE
           FROM edgis.zz_mv_StepDown             a,
                edgis.zz_mv_devicegroup          b,
                edgis.zz_mv_subsurfacestructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureGUID       AS structureGUID,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'STEPDOWN'            AS DEVICETYPE
           FROM edgis.zz_mv_StepDown a, edgis.zz_mv_subsurfacestructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                b.structureGUID     AS structureGUID,
                c.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'STEPDOWN'          AS DEVICETYPE
           FROM edgis.zz_mv_StepDown           a,
                edgis.zz_mv_devicegroup        b,
                edgis.zz_mv_padmountstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureGUID     AS structureGUID,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'STEPDOWN'          AS DEVICETYPE
           FROM edgis.zz_mv_StepDown a, edgis.zz_mv_padmountstructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                b.structureGUID    AS structureGUID,
                c.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'OPENPOINT'        AS DEVICETYPE
           FROM edgis.zz_mv_OpenPoint         a,
                edgis.zz_mv_devicegroup       b,
                edgis.zz_mv_supportstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid         AS DeviceGUID,
                a.structureGUID    AS structureGUID,
                b.polenumber       AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE' AS STRUCTURENAME,
                'OPENPOINT'        AS DEVICETYPE
           FROM edgis.zz_mv_OpenPoint a, edgis.zz_mv_supportstructure b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                b.structureGUID       AS structureGUID,
                c.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'OPENPOINT'           AS DEVICETYPE
           FROM edgis.zz_mv_OpenPoint            a,
                edgis.zz_mv_devicegroup          b,
                edgis.zz_mv_subsurfacestructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid            AS DeviceGUID,
                a.structureGUID       AS structureGUID,
                b.structurenumber     AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE' AS STRUCTURENAME,
                'OPENPOINT'           AS DEVICETYPE
           FROM edgis.zz_mv_OpenPoint a, edgis.zz_mv_subsurfacestructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                b.structureGUID     AS structureGUID,
                c.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'OPENPOINT'         AS DEVICETYPE
           FROM edgis.zz_mv_OpenPoint          a,
                edgis.zz_mv_devicegroup        b,
                edgis.zz_mv_padmountstructure  c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid          AS DeviceGUID,
                a.structureGUID     AS structureGUID,
                b.structurenumber   AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE' AS STRUCTURENAME,
                'OPENPOINT'         AS DEVICETYPE
           FROM edgis.zz_mv_OpenPoint a, edgis.zz_mv_padmountstructure b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid                AS DeviceGUID,
                b.structureGUID           AS structureGUID,
                c.polenumber              AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE'        AS STRUCTURENAME,
                'DYNAMICPROTECTIVEDEVICE' AS DEVICETYPE
           FROM edgis.zz_mv_DynamicProtectiveDevice  a,
                edgis.zz_mv_devicegroup              b,
                edgis.zz_mv_supportstructure         c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.polenumber IS NOT NULL
         UNION
         SELECT a.globalid                AS DeviceGUID,
                a.structureGUID           AS structureGUID,
                b.polenumber              AS STRUCTURENUMBER,
                'SUPPORTSTRUCTURE'        AS STRUCTURENAME,
                'DYNAMICPROTECTIVEDEVICE' AS DEVICETYPE
           FROM edgis.zz_mv_DynamicProtectiveDevice  a,
                edgis.zz_mv_supportstructure         b
          WHERE a.structureguid = b.globalid AND b.polenumber IS NOT NULL
         UNION
         SELECT a.globalid                AS DeviceGUID,
                b.structureGUID           AS structureGUID,
                c.structurenumber         AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE'     AS STRUCTURENAME,
                'DYNAMICPROTECTIVEDEVICE' AS DEVICETYPE
           FROM edgis.zz_mv_DynamicProtectiveDevice  a,
                edgis.zz_mv_devicegroup              b,
                edgis.zz_mv_subsurfacestructure      c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid                AS DeviceGUID,
                a.structureGUID           AS structureGUID,
                b.structurenumber         AS STRUCTURENUMBER,
                'SUBSURFACESTRUCTURE'     AS STRUCTURENAME,
                'DYNAMICPROTECTIVEDEVICE' AS DEVICETYPE
           FROM edgis.zz_mv_DynamicProtectiveDevice  a,
                edgis.zz_mv_subsurfacestructure      b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid                AS DeviceGUID,
                b.structureGUID           AS structureGUID,
                c.structurenumber         AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE'       AS STRUCTURENAME,
                'DYNAMICPROTECTIVEDEVICE' AS DEVICETYPE
           FROM edgis.zz_mv_DynamicProtectiveDevice  a,
                edgis.zz_mv_devicegroup              b,
                edgis.zz_mv_padmountstructure        c
          WHERE     a.structureguid = b.globalid
                AND b.structureguid = c.globalid
                AND c.structurenumber IS NOT NULL
         UNION
         SELECT a.globalid                AS DeviceGUID,
                a.structureGUID           AS structureGUID,
                b.structurenumber         AS STRUCTURENUMBER,
                'PADMOUNTSTRUCTURE'       AS STRUCTURENAME,
                'DYNAMICPROTECTIVEDEVICE' AS DEVICETYPE
           FROM edgis.zz_mv_DynamicProtectiveDevice  a,
                edgis.zz_mv_padmountstructure        b
          WHERE     a.structureguid = b.globalid
                AND b.structurenumber IS NOT NULL))
/


COMMENT ON MATERIALIZED VIEW EDGIS.STRUCTURES_INTRM_VW IS 'snapshot table for snapshot EDGIS.STRUCTURES_INTRM_VW'
/

Prompt Grants on MATERIALIZED VIEW STRUCTURES_INTRM_VW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.STRUCTURES_INTRM_VW TO GIS_SAP_RECON
/
