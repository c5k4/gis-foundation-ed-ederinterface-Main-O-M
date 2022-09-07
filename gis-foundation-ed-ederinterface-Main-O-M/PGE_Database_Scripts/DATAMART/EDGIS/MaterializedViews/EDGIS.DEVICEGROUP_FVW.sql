Prompt drop Materialized View DEVICEGROUP_FVW;
DROP MATERIALIZED VIEW EDGIS.DEVICEGROUP_FVW
/
Prompt Materialized View DEVICEGROUP_FVW;
--
-- DEVICEGROUP_FVW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.DEVICEGROUP_FVW (ACCESSINFO,CIRCUITID,CITY,COMMENTS,CONVERSIONID, CONVERSIONWORKPACKAGE,COUNTY,CREATIONUSER,CUSTOMEROWNED,DATECREATED, DATEMODIFIED,DEVICEGROUPNAME,DEVICEGROUPTYPE,DISTMAP,DISTRICT, DIVISION,FUNCTIONALLOCATIONID,GLOBALID,GPSLATITUDE,GPSLONGITUDE, GPSSOURCE,INSTALLATIONDATE,INSTALLJOBNUMBER,INSTALLJOBPREFIX,INSTALLJOBYEAR, LASTUSER,LOCALOFFICEID,LOCDESC1,LOCDESC2,MAPOFFICE, OBJECTID,OTHERMAP,"OperatingNumber",POSITIONDESCRIPTION,REGION, RETIREDATE,SAPEQUIPID,SHAPE,SOURCEACCURACY,STATUS, STRUCTUREGUID,SUBTYPECD,SYMBOLNUMBER,SYMBOLROTATION,TEMPEQUIPID, URBANRURALCODE,VERSIONNAME,ZIP,"CEDSADeviceID","SAP Object Type", STRUCTURENUMBER) TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:31 PM (QP5 v5.313) */
(SELECT a.ACCESSINFO,
        a.CIRCUITID,
        a.CITY,
        a.COMMENTS,
        a.CONVERSIONID,
        a.CONVERSIONWORKPACKAGE,
        a.COUNTY,
        NVL (a.CREATIONUSER, a.LASTUSER)
            CREATIONUSER,
        a.CUSTOMEROWNED,
        NVL (a.DATECREATED, a.DATEMODIFIED)
            DATECREATED,
        NVL (a.DATEMODIFIED, a.DATECREATED)
            DATEMODIFIED,
        a.DEVICEGROUPNAME,
        a.DEVICEGROUPTYPE,
        a.DISTMAP,
        a.DISTRICT,
        a.DIVISION,
        a.FUNCTIONALLOCATIONID,
        a.GLOBALID,
        a.GPSLATITUDE,
        a.GPSLONGITUDE,
        a.GPSSOURCE,
        a.INSTALLATIONDATE,
        a.INSTALLJOBNUMBER,
        a.INSTALLJOBPREFIX,
        a.INSTALLJOBYEAR,
        NVL (a.LASTUSER, a.CREATIONUSER)
            LASTUSER,
        a.LOCALOFFICEID,
        a.LOCDESC1,
        a.LOCDESC2,
        a.MAPOFFICE,
        a.OBJECTID,
        a.OTHERMAP,
        (CASE
             WHEN a.subtypecd = 2 AND a.devicegrouptype = 33
             THEN
                 DECODE (b.structurename,
                         'SUBSURFACESTRUCTURE', b.structurenumber,
                         NULL)
             ELSE
                 DECODE (
                     p.globalid,
                     NULL, DECODE (b.structurename,
                                   'SUBSURFACESTRUCTURE', b.structurenumber,
                                   NULL),
                     p.objectid)
         END)
            AS "OperatingNumber",
        a.POSITIONDESCRIPTION,
        a.REGION,
        a.RETIREDATE,
        a.SAPEQUIPID,
        a.SHAPE,
        a.SOURCEACCURACY,
        a.STATUS,
        a.STRUCTUREGUID,
        a.SUBTYPECD,
        a.SYMBOLNUMBER,
        a.SYMBOLROTATION,
        a.TEMPEQUIPID,
        a.URBANRURALCODE,
        a.VERSIONNAME,
        a.ZIP,
        CAST (NULL AS VARCHAR2 (50))
            AS "CEDSADeviceID",
        (CASE
             WHEN a.subtypecd = 2 AND a.devicegrouptype = 33
             THEN
                 'ED.XD_NETW'
             ELSE
                 (CASE
                      WHEN a.subtypecd = 2 AND a.DEVICEGROUPTYPE IN (9)
                      THEN
                          'ED.XD_SSCT'
                      ELSE
                          (CASE
                               WHEN     a.subtypecd = 2
                                    AND a.DEVICEGROUPTYPE IN (16, 17, 18)
                               THEN
                                   'ED.XD_SSIR'
                               ELSE
                                   (CASE
                                        WHEN     a.subtypecd = 2
                                             AND a.DEVICEGROUPTYPE IN
                                                     (30, 31, 32)
                                        THEN
                                            'ED.XD_SSMS'
                                        ELSE
                                            (CASE
                                                 WHEN     a.subtypecd = 2
                                                      AND a.DEVICEGROUPTYPE IN
                                                              (19)
                                                 THEN
                                                     'ED.XD_SSSE'
                                                 ELSE
                                                     (CASE
                                                          WHEN     a.subtypecd =
                                                                   2
                                                               AND a.DEVICEGROUPTYPE IN
                                                                       (7,
                                                                        8,
                                                                        9,
                                                                        10,
                                                                        11,
                                                                        12,
                                                                        13,
                                                                        14,
                                                                        15,
                                                                        26,
                                                                        27,
                                                                        28,
                                                                        29,
                                                                        34)
                                                          THEN
                                                              'ED.XD_SSSW'
                                                          ELSE
                                                              (CASE
                                                                   WHEN     a.subtypecd =
                                                                            2
                                                                        AND a.DEVICEGROUPTYPE IN
                                                                                (1,
                                                                                 2,
                                                                                 3,
                                                                                 4,
                                                                                 5,
                                                                                 6,
                                                                                 35)
                                                                   THEN
                                                                       'ED.XD_SSTX'
                                                                   ELSE
                                                                       (CASE
                                                                            WHEN     a.subtypecd =
                                                                                     2
                                                                                 AND a.DEVICEGROUPTYPE IN
                                                                                         (23,
                                                                                          24)
                                                                            THEN
                                                                                'ED.XD_VTIR'
                                                                            ELSE
                                                                                (CASE
                                                                                     WHEN     a.subtypecd =
                                                                                              2
                                                                                          AND a.DEVICEGROUPTYPE IN
                                                                                                  (20,
                                                                                                   21,
                                                                                                   22)
                                                                                     THEN
                                                                                         'ED.XD_VTSW'
                                                                                     ELSE
                                                                                         (CASE
                                                                                              WHEN     a.subtypecd =
                                                                                                       2
                                                                                                   AND a.DEVICEGROUPTYPE IN
                                                                                                           (25)
                                                                                              THEN
                                                                                                  'ED.XD_VTTX'
                                                                                              ELSE
                                                                                                  (CASE
                                                                                                       WHEN     a.subtypecd =
                                                                                                                3
                                                                                                            AND a.DEVICEGROUPTYPE IN
                                                                                                                    (24,
                                                                                                                     25)
                                                                                                       THEN
                                                                                                           'ED.XD_PMCA'
                                                                                                       ELSE
                                                                                                           (CASE
                                                                                                                WHEN     a.subtypecd =
                                                                                                                         3
                                                                                                                     AND a.DEVICEGROUPTYPE IN
                                                                                                                             (30,
                                                                                                                              31)
                                                                                                                THEN
                                                                                                                    'ED.XD_PMIR'
                                                                                                                ELSE
                                                                                                                    (CASE
                                                                                                                         WHEN     a.subtypecd =
                                                                                                                                  3
                                                                                                                              AND a.DEVICEGROUPTYPE IN
                                                                                                                                      (37,
                                                                                                                                       38,
                                                                                                                                       39,
                                                                                                                                       40,
                                                                                                                                       41,
                                                                                                                                       42,
                                                                                                                                       43,
                                                                                                                                       44,
                                                                                                                                       45,
                                                                                                                                       46,
                                                                                                                                       47)
                                                                                                                         THEN
                                                                                                                             'ED.XD_PMMS'
                                                                                                                         ELSE
                                                                                                                             (CASE
                                                                                                                                  WHEN     a.subtypecd =
                                                                                                                                           3
                                                                                                                                       AND a.DEVICEGROUPTYPE IN
                                                                                                                                               (34)
                                                                                                                                  THEN
                                                                                                                                      'ED.XD_PMRC'
                                                                                                                                  ELSE
                                                                                                                                      (CASE
                                                                                                                                           WHEN     a.subtypecd =
                                                                                                                                                    3
                                                                                                                                                AND a.DEVICEGROUPTYPE IN
                                                                                                                                                        (26)
                                                                                                                                           THEN
                                                                                                                                               'ED.XD_PMRG'
                                                                                                                                           ELSE
                                                                                                                                               (CASE
                                                                                                                                                    WHEN     a.subtypecd =
                                                                                                                                                             3
                                                                                                                                                         AND a.DEVICEGROUPTYPE IN
                                                                                                                                                                 (35)
                                                                                                                                                    THEN
                                                                                                                                                        'ED.XD_PMSE'
                                                                                                                                                    ELSE
                                                                                                                                                        (CASE
                                                                                                                                                             WHEN     a.subtypecd =
                                                                                                                                                                      3
                                                                                                                                                                  AND a.DEVICEGROUPTYPE IN
                                                                                                                                                                          (1,
                                                                                                                                                                           2,
                                                                                                                                                                           3,
                                                                                                                                                                           4,
                                                                                                                                                                           5,
                                                                                                                                                                           6,
                                                                                                                                                                           7,
                                                                                                                                                                           8,
                                                                                                                                                                           9,
                                                                                                                                                                           10,
                                                                                                                                                                           11,
                                                                                                                                                                           12,
                                                                                                                                                                           13,
                                                                                                                                                                           14,
                                                                                                                                                                           15,
                                                                                                                                                                           27,
                                                                                                                                                                           28,
                                                                                                                                                                           29,
                                                                                                                                                                           36)
                                                                                                                                                             THEN
                                                                                                                                                                 'ED.XD_PMSW'
                                                                                                                                                             ELSE
                                                                                                                                                                 (CASE
                                                                                                                                                                      WHEN     a.subtypecd =
                                                                                                                                                                               3
                                                                                                                                                                           AND a.DEVICEGROUPTYPE IN
                                                                                                                                                                                   (16,
                                                                                                                                                                                    17,
                                                                                                                                                                                    18,
                                                                                                                                                                                    19,
                                                                                                                                                                                    20,
                                                                                                                                                                                    21,
                                                                                                                                                                                    22,
                                                                                                                                                                                    23,
                                                                                                                                                                                    32,
                                                                                                                                                                                    33)
                                                                                                                                                                      THEN
                                                                                                                                                                          'ED.XD_PMTX'
                                                                                                                                                                      ELSE
                                                                                                                                                                          'NA'
                                                                                                                                                                  END)
                                                                                                                                                         END)
                                                                                                                                                END)
                                                                                                                                       END)
                                                                                                                              END)
                                                                                                                     END)
                                                                                                            END)
                                                                                                   END)
                                                                                          END)
                                                                                 END)
                                                                        END)
                                                               END)
                                                      END)
                                             END)
                                    END)
                           END)
                  END)
         END)
            AS "SAP Object Type",
        (CASE
             WHEN a.subtypecd = 2 AND a.devicegrouptype = 33
             THEN
                 DECODE (b.structurename,
                         'SUBSURFACESTRUCTURE', b.structurenumber,
                         NULL)
             ELSE
                 DECODE (
                     p.globalid,
                     NULL, DECODE (b.structurename,
                                   'SUBSURFACESTRUCTURE', b.structurenumber,
                                   NULL),
                     p.objectid)
         END)
            AS STRUCTURENUMBER
   FROM edgis.zz_MV_DEVICEGROUP        a,
        edgis.STRUCTURES_INTRM_VW      b,
        edgis.zz_mv_padmountstructure  p
  WHERE     a.GLOBALID = b.DEVICEGUID(+)
        AND a.structureguid = p.globalid(+)
        AND a.subtypecd IN (2, 3))
/


COMMENT ON MATERIALIZED VIEW EDGIS.DEVICEGROUP_FVW IS 'snapshot table for snapshot EDGIS.DEVICEGROUP_FVW'
/

Prompt Grants on MATERIALIZED VIEW DEVICEGROUP_FVW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.DEVICEGROUP_FVW TO GIS_SAP_RECON
/
