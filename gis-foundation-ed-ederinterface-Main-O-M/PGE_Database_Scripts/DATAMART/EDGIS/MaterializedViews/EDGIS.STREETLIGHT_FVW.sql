Prompt drop Materialized View STREETLIGHT_FVW;
DROP MATERIALIZED VIEW EDGIS.STREETLIGHT_FVW
/
Prompt Materialized View STREETLIGHT_FVW;
--
-- STREETLIGHT_FVW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.STREETLIGHT_FVW (ACCOUNTNUMBER,ALTNM,ANCILLARYROLE,BALLASTCHANGEDATE,BALLASTTYPE, CCBOVERWRITEFLAG,CHANGEOFPARTYDATE,CIRCUITID,CIRCUITID2,CITY, COMMENTS,CONVCIRCUITID,CONVCIRCUITID2,CONVERSIONID,CONVERSIONWORKPACKAGE, CONVIENENCEOUTLETS,COUNTY,CREATIONUSER,CUSTOMERNAME,CUSTOMEROWNED, DATECREATED,DATEMODIFIED,DESCRIPTIVEADDRESS,DIFFADDR,DIFFBADGE, DIFFIT,DIFFIX,DIFFMAP,DIFFRS,DISTRICT, DIVISION,ELECTRICTRACEWEIGHT,ELEVATION,ENABLED,FAPPLIANCE1, FAPPLIANCE2,FAPPLIANCE3,FAPPLIANCE4,FAPPLIANCE5,FAR1, FAR2,FAR3,FAR4,FAR5,FAROTHER, FEEDERINFO,FIXTURECODE,FIXTUREMANUFACTURER,FMETRICOM,GEMSDISTRMAPNUM, GLOBALID,HALFHRADJTYPE,HISTGEMSAKA,HISTGEMSBADGENUM,HISTGEMSMAPNUM, HISTGEMSMAPRATE,INSTALLATIONDATE,INSTALLJOBNUMBER,INSTALLJOBPREFIX,INSTALLJOBYEAR, INVENTORIEDBY,INVENTORYDATE,ITEMTYPECODE,LAMPCHANGEDATE,LAMPTYPE, LASTMODIFYDATE,LASTUSER,LENSTYPE,LIGHTSTYLE,LITESIZETYPE, LITETYPETYPE,LOCALOFFICEID,LOCALOPOFFICE,LOCATIONDESC,LOCATIONID, LUMENOUTPUT,LUMNCHANGEDATE,MAILADDR1,MAILADDR2,MAILCITY, MAILSTATE,MAILZIP,MAINTNOTE,MAPNUMBER,MAPNUMBERNEW, METER,NEARESTST,NEWGRIDMAPNUM,OBJECTID,OFFICE, OPERATINGSCHEDULE,OPERATINGVOLTAGE,OWNERBADGENUMBER,OWNERSHIP,PAINTPOLE, PCELL,PCELLCHANGEDATE,PHASEDESIGNATION,PHASINGVERIFIEDSTATUS,PHOTOCELLTYPE, POLECHANGEDATE,POLELENGTH,POLEPAINTDATE,POLETYPE,POLEUSE, PREMISEID,RATESCHEDULE,RECEIVEDATE,RECORDSTATUS,REGION, REMOVEDATE,REPLACEGUID,SAPEQUIPID,SERIALNUMBER,SERVICEAGREEMENTID, SERVICEPOINTID,SERVICETYPE,SHAPE,SHIELDINGIDC,SPITEMHISTORY, STATUS,STATUSFLAG,STREETLIGHTNUMBER,STREETLIGHTROLE,STREETLIGHTSEQNO, STREETLIGHTSGISID,STREETLIGHTSGISSAID,STREETLIGHTSGISSPID,STREETLIGHTTRANSACTION,STREETNM, STRTCHDT,STRUCTURECONVID,STRUCTUREGUID,SUBTYPECD,SUSPENSION, SYMBOLROTATION,TEMPEQUIPID,TIMECONTROLIDC,TOTCODE,TOWNTERRDESC, UNIQUESPID,USERID,VOLTAGE,WATTAGE,YEARMANUFACTURED, ZIP,"CEDSADeviceID","Manufacturer","MaterialCode","ModelNumber", "SAP Object Type","OperatingNumber") TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:35 PM (QP5 v5.313) */
(SELECT a.ACCOUNTNUMBER,
        a.ALTNM,
        a.ANCILLARYROLE,
        a.BALLASTCHANGEDATE,
        a.BALLASTTYPE,
        a.CCBOVERWRITEFLAG,
        a.CHANGEOFPARTYDATE,
        a.CIRCUITID,
        a.CIRCUITID2,
        a.CITY,
        a.COMMENTS,
        a.CONVCIRCUITID,
        a.CONVCIRCUITID2,
        a.CONVERSIONID,
        a.CONVERSIONWORKPACKAGE,
        a.CONVIENENCEOUTLETS,
        a.COUNTY,
        NVL (a.CREATIONUSER, a.LASTUSER)
            CREATIONUSER,
        a.CUSTOMERNAME,
        a.CUSTOMEROWNED,
        NVL (a.DATECREATED, a.DATEMODIFIED)
            DATECREATED,
        NVL (a.DATEMODIFIED, a.DATECREATED)
            DATEMODIFIED,
        a.DESCRIPTIVEADDRESS,
        a.DIFFADDR,
        a.DIFFBADGE,
        a.DIFFIT,
        a.DIFFIX,
        a.DIFFMAP,
        a.DIFFRS,
        a.DISTRICT,
        a.DIVISION,
        a.ELECTRICTRACEWEIGHT,
        a.ELEVATION,
        a.ENABLED,
        a.FAPPLIANCE1,
        a.FAPPLIANCE2,
        a.FAPPLIANCE3,
        a.FAPPLIANCE4,
        a.FAPPLIANCE5,
        a.FAR1,
        a.FAR2,
        a.FAR3,
        a.FAR4,
        a.FAR5,
        a.FAROTHER,
        a.FEEDERINFO,
        a.FIXTURECODE,
        a.FIXTUREMANUFACTURER,
        a.FMETRICOM,
        a.GEMSDISTRMAPNUM,
        a.GLOBALID,
        a.HALFHRADJTYPE,
        a.HISTGEMSAKA,
        a.HISTGEMSBADGENUM,
        a.HISTGEMSMAPNUM,
        a.HISTGEMSMAPRATE,
        a.INSTALLATIONDATE,
        a.INSTALLJOBNUMBER,
        a.INSTALLJOBPREFIX,
        a.INSTALLJOBYEAR,
        a.INVENTORIEDBY,
        a.INVENTORYDATE,
        a.ITEMTYPECODE,
        a.LAMPCHANGEDATE,
        a.LAMPTYPE,
        a.LASTMODIFYDATE,
        NVL (a.LASTUSER, a.CREATIONUSER)
            LASTUSER,
        a.LENSTYPE,
        a.LIGHTSTYLE,
        a.LITESIZETYPE,
        a.LITETYPETYPE,
        a.LOCALOFFICEID,
        a.LOCALOPOFFICE,
        a.LOCATIONDESC,
        a.LOCATIONID,
        a.LUMENOUTPUT,
        a.LUMNCHANGEDATE,
        a.MAILADDR1,
        a.MAILADDR2,
        a.MAILCITY,
        a.MAILSTATE,
        a.MAILZIP,
        a.MAINTNOTE,
        a.MAPNUMBER,
        a.MAPNUMBERNEW,
        a.METER,
        a.NEARESTST,
        a.NEWGRIDMAPNUM,
        a.OBJECTID,
        a.OFFICE,
        a.OPERATINGSCHEDULE,
        a.OPERATINGVOLTAGE,
        a.OWNERBADGENUMBER,
        a.OWNERSHIP,
        a.PAINTPOLE,
        a.PCELL,
        a.PCELLCHANGEDATE,
        a.PHASEDESIGNATION,
        a.PHASINGVERIFIEDSTATUS,
        a.PHOTOCELLTYPE,
        a.POLECHANGEDATE,
        a.POLELENGTH,
        a.POLEPAINTDATE,
        a.POLETYPE,
        a.POLEUSE,
        a.PREMISEID,
        a.RATESCHEDULE,
        a.RECEIVEDATE,
        a.RECORDSTATUS,
        a.REGION,
        a.REMOVEDATE,
        a.REPLACEGUID,
        a.SAPEQUIPID,
        a.SERIALNUMBER,
        a.SERVICEAGREEMENTID,
        a.SERVICEPOINTID,
        a.SERVICETYPE,
        a.SHAPE,
        a.SHIELDINGIDC,
        a.SPITEMHISTORY,
        a.STATUS,
        a.STATUSFLAG,
        a.STREETLIGHTNUMBER,
        a.STREETLIGHTROLE,
        a.STREETLIGHTSEQNO,
        a.STREETLIGHTSGISID,
        a.STREETLIGHTSGISSAID,
        a.STREETLIGHTSGISSPID,
        a.STREETLIGHTTRANSACTION,
        a.STREETNM,
        a.STRTCHDT,
        a.STRUCTURECONVID,
        DECODE (ss.customerowned, 'N', a.STRUCTUREGUID, NULL)
            AS structureguid,
        a.SUBTYPECD,
        a.SUSPENSION,
        a.SYMBOLROTATION,
        a.TEMPEQUIPID,
        a.TIMECONTROLIDC,
        a.TOTCODE,
        a.TOWNTERRDESC,
        a.UNIQUESPID,
        a.USERID,
        a.VOLTAGE,
        a.WATTAGE,
        a.YEARMANUFACTURED,
        a.ZIP,
        CAST (NULL AS VARCHAR2 (50))
            AS "CEDSADeviceID",
        CAST (NULL AS VARCHAR2 (50))
            AS "Manufacturer",
        CAST (NULL AS VARCHAR2 (50))
            AS "MaterialCode",
        CAST (NULL AS VARCHAR2 (50))
            AS "ModelNumber",
        'ED.STLT'
            AS "SAP Object Type",
        CAST (NULL AS VARCHAR2 (50))
            AS "OperatingNumber"
   FROM edgis.zz_MV_STREETLIGHT a, edgis.zz_mv_supportstructure ss
  WHERE a.structureguid = ss.globalid(+))
/


COMMENT ON MATERIALIZED VIEW EDGIS.STREETLIGHT_FVW IS 'snapshot table for snapshot EDGIS.STREETLIGHT_FVW'
/

Prompt Grants on MATERIALIZED VIEW STREETLIGHT_FVW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.STREETLIGHT_FVW TO GIS_SAP_RECON
/
