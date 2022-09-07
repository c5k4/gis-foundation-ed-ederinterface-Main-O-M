Prompt drop Materialized View SUPPORTSTRUCTURE_FVW;
DROP MATERIALIZED VIEW EDGIS.SUPPORTSTRUCTURE_FVW
/
Prompt Materialized View SUPPORTSTRUCTURE_FVW;
--
-- SUPPORTSTRUCTURE_FVW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.SUPPORTSTRUCTURE_FVW (ACCESSINFO,BARCODE,CEDSAID,CITY,CLASS, COMMENTS,CONVERSIONID,CONVERSIONWORKPACKAGE,COUNTY,CURRENTCIRCUMFERENCE, CUSTAGREEMENTGUID,CUSTOMEROWNED,DEVICEGROUPGUID,DISTMAP,DISTRICT, DIVISION,EXISTINGREINFORCEMENT,FOREIGNATTACHIDC,FOUNDATIONTYPE,FUNCTIONALLOCATIONID, GAUGE,GLOBALID,GPSLATITUDE,GPSLONGITUDE,GPSSOURCE, GROUNDEDIDC,HEIGHT,INSTALLATIONDATE,INSTALLJOBNUMBER,INSTALLJOBPREFIX, INSTALLJOBYEAR,JOINTCOUNT,JP_DISTRICT,JPAGREEMENTDATE,JPNUMBER, JPSEQUENCE,LABELTEXT,LOCALOFFICEID,LOCDESC1,LOCDESC2, LOCDESC3,MANUFACTUREDYEAR,MANUFACTURER,MAPOFFICE,MATERIAL, MAXVOLTAGELEVEL,METEREDIDC,OBJECTID,ORIGINALCIRCUMFERENCE,ORIGINALTREATMENTTYPE, OTHERMAP,POLECOUNT,POLENUMBER,POLETOPEXTIDC,POLETYPE, POLEUSE,PTTDIDC,REGION,REPLACEGUID,RETIREDATE, SAPEQUIPID,SERIALNUMBER,SHAPE,SMARTMETERNETWORKDEVICEOID,SOURCEACCURACY, SPECIES,STATEREGAREA,STATUS,STRUCTURECODE,STRUCTUREGUID, SUBTYPECD,SYMBOLNUMBER,SYMBOLROTATION,TECHIDENTNO,TEMPEQUIPID, TRANSMISSIONMILENUMBER,TRANSMISSIONSTRUCTNUMBER,URBANRURALCODE,VC_NUM,"DistributionMap", ZIP,"SAP Object Type","AnimalGuardType","CEDSADeviceID","GEMS Map office", GEMS_DIST_MAP,"OperatingNumber",CREATIONUSER,DATECREATED,DATEMODIFIED, LASTUSER) TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY USING TRUSTED CONSTRAINTS AS 
/* Formatted on 7/2/2019 01:16:37 PM (QP5 v5.313) */
(SELECT a.ACCESSINFO,
        a.BARCODE,
        a.CEDSAID,
        a.CITY,
        a.CLASS,
        a.COMMENTS,
        a.CONVERSIONID,
        a.CONVERSIONWORKPACKAGE,
        a.COUNTY,
        a.CURRENTCIRCUMFERENCE,
        a.CUSTAGREEMENTGUID,
        a.CUSTOMEROWNED,
        a.DEVICEGROUPGUID,
        a.DISTMAP,
        a.DISTRICT,
        a.DIVISION,
        a.EXISTINGREINFORCEMENT,
        a.FOREIGNATTACHIDC,
        a.FOUNDATIONTYPE,
        a.FUNCTIONALLOCATIONID,
        a.GAUGE,
        a.GLOBALID,
        a.GPSLATITUDE,
        a.GPSLONGITUDE,
        a.GPSSOURCE,
        a.GROUNDEDIDC,
        a.HEIGHT,
        a.INSTALLATIONDATE,
        a.INSTALLJOBNUMBER,
        a.INSTALLJOBPREFIX,
        a.INSTALLJOBYEAR,
        a.JOINTCOUNT,
        a.JP_DISTRICT,
        a.JPAGREEMENTDATE,
        a.JPNUMBER,
        a.JPSEQUENCE,
        a.LABELTEXT,
        a.LOCALOFFICEID,
        a.LOCDESC1,
        a.LOCDESC2,
        a.LOCDESC3,
        a.MANUFACTUREDYEAR,
        a.MANUFACTURER,
        a.MAPOFFICE,
        a.MATERIAL,
        a.MAXVOLTAGELEVEL,
        a.METEREDIDC,
        a.OBJECTID,
        a.ORIGINALCIRCUMFERENCE,
        a.ORIGINALTREATMENTTYPE,
        a.OTHERMAP,
        a.POLECOUNT,
        a.POLENUMBER,
        a.POLETOPEXTIDC,
        a.POLETYPE,
        a.POLEUSE,
        a.PTTDIDC,
        a.REGION,
        a.REPLACEGUID,
        a.RETIREDATE,
        a.SAPEQUIPID,
        a.SERIALNUMBER,
        a.SHAPE,
        a.SMARTMETERNETWORKDEVICEOID,
        a.SOURCEACCURACY,
        a.SPECIES,
        a.STATEREGAREA,
        a.STATUS,
        a.STRUCTURECODE,
        a.STRUCTUREGUID,
        a.SUBTYPECD,
        a.SYMBOLNUMBER,
        a.SYMBOLROTATION,
        a.TECHIDENTNO,
        a.TEMPEQUIPID,
        a.TRANSMISSIONMILENUMBER,
        a.TRANSMISSIONSTRUCTNUMBER,
        a.URBANRURALCODE,
        a.VC_NUM,
        a.distmap
            AS "DistributionMap",
        a.ZIP,
        'ED.POLE'
            AS "SAP Object Type",
        CAST (NULL AS VARCHAR2 (50))
            AS "AnimalGuardType",
        CAST (NULL AS VARCHAR2 (50))
            AS "CEDSADeviceID",
        CAST (NULL AS VARCHAR2 (50))
            AS "GEMS Map office",
        CAST (NULL AS VARCHAR2 (50))
            AS "GEMS_DIST_MAP",
        CAST (NULL AS VARCHAR2 (50))
            AS "OperatingNumber",
        (CASE
             WHEN a.CREATIONUSER IS NULL THEN a.LASTUSER
             ELSE a.CREATIONUSER
         END)
            AS "CREATIONUSER",
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
        (CASE WHEN a.LASTUSER IS NULL THEN a.CREATIONUSER ELSE a.LASTUSER END)
            AS "LASTUSER"
   FROM edgis.zz_mv_SUPPORTSTRUCTURE a
  WHERE    a.subtypecd IN (1,
                           2,
                           4,
                           5,
                           6,
                           7,
                           8)
        OR (    a.subtypecd = '3'
            AND a.customerowned = 'N'
            AND (a.poleuse = '4' OR a.poleuse = '5')
            AND (a.sapequipid = '9999' OR a.sapequipid IS NULL)
            AND (a.status = '5' OR a.status = '30')))
/


COMMENT ON MATERIALIZED VIEW EDGIS.SUPPORTSTRUCTURE_FVW IS 'snapshot table for snapshot EDGIS.SUPPORTSTRUCTURE_FVW'
/

Prompt Grants on MATERIALIZED VIEW SUPPORTSTRUCTURE_FVW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.SUPPORTSTRUCTURE_FVW TO GIS_SAP_RECON
/
