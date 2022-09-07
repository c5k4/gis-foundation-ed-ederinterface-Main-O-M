Prompt drop Materialized View SUBSURFACESTRUCTURE_FVW;
DROP MATERIALIZED VIEW EDGIS.SUBSURFACESTRUCTURE_FVW
/
Prompt Materialized View SUBSURFACESTRUCTURE_FVW;
--
-- SUBSURFACESTRUCTURE_FVW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.SUBSURFACESTRUCTURE_FVW (ACCESSINFO,ANCILLARYROLE,CITY,COMMENTS,CONVERSIONID, CONVERSIONWORKPACKAGE,COUNTY,COVERTYPE,CREATIONUSER,CUSTAGREEMENTGUID, CUSTOMEROWNED,DATECREATED,DATEMODIFIED,DEACTIVATEDINDICATOR,DISTMAP, GEMS_DIST_MAP,DISTRICT,DIVISION,ENABLED,FACILITYDIAGRAM, FUNCTIONALLOCATIONID,GLOBALID,GPSLATITUDE,GPSLONGITUDE,GPSSOURCE, IMAGEPATH,INSTALLATIONDATE,INSTALLJOBNUMBER,INSTALLJOBPREFIX,INSTALLJOBYEAR, LADDER,LASTUSER,LIGHT,LOCALOFFICEID,LOCDESC1, LOCDESC2,MAPOFFICE,MATERIAL,MHCOVERSIZE,MHCOVERTYPE, NECKSIZE,NOOFVENTS,NOTE1,NOTE2,OBJECTID, OTHERMAP,REGION,REPLACEGUID,RETIREDATE,SAPEQUIPID, SHAPE,SOURCEACCURACY,STATUS,STRUCTURENUMBER,MANHOLENUMBER, STRUCTURESIZE,STRUCTURETYPE,SUBTYPECD,SYMBOLROTATION,TEMPEQUIPID, URBANRURALCODE,ZIP,"CEDSADeviceID",GEMS_CIRCUIT_MAP,"Manufacturer", "ModelNumber","OperatingNumber","PartNumber","SAP Object Type","SerialNumber", "SumpPumpInstalled","YearManufactured") TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:36 PM (QP5 v5.313) */
(SELECT a.ACCESSINFO,
        a.ANCILLARYROLE,
        a.CITY,
        a.COMMENTS,
        a.CONVERSIONID,
        a.CONVERSIONWORKPACKAGE,
        a.COUNTY,
        a.COVERTYPE,
        (CASE
             WHEN a.CREATIONUSER IS NULL THEN a.LASTUSER
             ELSE a.CREATIONUSER
         END)
            AS "CREATIONUSER",
        a.CUSTAGREEMENTGUID,
        a.CUSTOMEROWNED,
        (CASE
             WHEN a.DATECREATED IS NULL THEN a.DATEMODIFIED
             ELSE a.DATECREATED
         END)
            AS "DATECREATED",
        (CASE
             WHEN a.DATEMODIFIED IS NULL THEN a.DATECREATED
             ELSE a.DATEMODIFIED
         END)
            AS "DATEMODIFIED",
        a.DEACTIVATEDINDICATOR,
        a.DISTMAP,
        a.DISTMAP
            AS "GEMS_DIST_MAP",
        a.DISTRICT,
        a.DIVISION,
        a.ENABLED,
        a.FACILITYDIAGRAM,
        a.FUNCTIONALLOCATIONID,
        a.GLOBALID,
        a.GPSLATITUDE,
        a.GPSLONGITUDE,
        a.GPSSOURCE,
        a.IMAGEPATH,
        a.INSTALLATIONDATE,
        a.INSTALLJOBNUMBER,
        a.INSTALLJOBPREFIX,
        a.INSTALLJOBYEAR,
        a.LADDER,
        (CASE WHEN a.LASTUSER IS NULL THEN a.CREATIONUSER ELSE a.LASTUSER END)
            AS "LASTUSER",
        a.LIGHT,
        a.LOCALOFFICEID,
        a.LOCDESC1,
        a.LOCDESC2,
        a.MAPOFFICE,
        a.MATERIAL,
        a.MHCOVERSIZE,
        a.MHCOVERTYPE,
        a.NECKSIZE,
        a.NOOFVENTS,
        a.NOTE1,
        a.NOTE2,
        a.OBJECTID,
        a.OTHERMAP,
        a.REGION,
        a.REPLACEGUID,
        a.RETIREDATE,
        a.SAPEQUIPID,
        a.SHAPE,
        a.SOURCEACCURACY,
        a.STATUS,
        a.structurenumber
            AS "STRUCTURENUMBER",
        a.structurenumber
            AS "MANHOLENUMBER",
        a.STRUCTURESIZE,
        a.STRUCTURETYPE,
        a.SUBTYPECD,
        a.SYMBOLROTATION,
        a.TEMPEQUIPID,
        a.URBANRURALCODE,
        a.ZIP,
        CAST (NULL AS VARCHAR2 (50))
            AS "CEDSADeviceID",
        CAST (NULL AS VARCHAR2 (50))
            AS "GEMS_CIRCUIT_MAP",
        CAST (NULL AS VARCHAR2 (50))
            AS "Manufacturer",
        CAST (NULL AS VARCHAR2 (50))
            AS "ModelNumber",
        CAST (NULL AS VARCHAR2 (50))
            AS "OperatingNumber",
        CAST (NULL AS VARCHAR2 (50))
            AS "PartNumber",
        (CASE
             WHEN subtypecd IN (1,
                                2,
                                5,
                                6,
                                7)
             THEN
                 'ED.ENCL'
             ELSE
                 (CASE WHEN subtypecd = 3 THEN 'ED.VLTS' ELSE 'NA' END)
         END)
            AS "SAP Object Type",
        CAST (NULL AS VARCHAR2 (50))
            AS "SerialNumber",
        CAST (NULL AS VARCHAR2 (50))
            AS "SumpPumpInstalled",
        CAST (NULL AS VARCHAR2 (50))
            AS "YearManufactured"
   FROM edgis.zz_mv_SUBSURFACESTRUCTURE a
  WHERE a.subtypecd IN (1,
                        2,
                        3,
                        5,
                        6,
                        7))
/


COMMENT ON MATERIALIZED VIEW EDGIS.SUBSURFACESTRUCTURE_FVW IS 'snapshot table for snapshot EDGIS.SUBSURFACESTRUCTURE_FVW'
/

Prompt Grants on MATERIALIZED VIEW SUBSURFACESTRUCTURE_FVW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.SUBSURFACESTRUCTURE_FVW TO GIS_SAP_RECON
/