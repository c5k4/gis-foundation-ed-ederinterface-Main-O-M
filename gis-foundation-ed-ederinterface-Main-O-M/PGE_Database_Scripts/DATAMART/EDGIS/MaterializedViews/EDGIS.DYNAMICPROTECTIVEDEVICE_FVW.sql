Prompt drop Materialized View DYNAMICPROTECTIVEDEVICE_FVW;
DROP MATERIALIZED VIEW EDGIS.DYNAMICPROTECTIVEDEVICE_FVW
/
Prompt Materialized View DYNAMICPROTECTIVEDEVICE_FVW;
--
-- DYNAMICPROTECTIVEDEVICE_FVW  (Materialized View) 
--
CREATE MATERIALIZED VIEW EDGIS.DYNAMICPROTECTIVEDEVICE_FVW (ANCILLARYROLE,ANIMALGUARDTYPE,AREASERVED,AUTOTRANSFERIDC,BUCKETTRUCKIDC, BYPASSPLANS,BYPASSSWITCH,CALTRANSPERMITIDC,CCRATING,CEDSADEVICEID, CIRCUITID,CIRCUITID2,CITY,CLOSEUPDATE,COASTALIDC, COMMENTS,COMPLEXDEVICEIDC,CONVCIRCUITID,CONVCIRCUITID2,CONVERSIONID, CONVERSIONWORKPACKAGE,COUNTY,CTRATIO,CUSTOMEROWNED,DEVICETYPE, DISCONNECTIDC,DISTRICT,DIVISION,ELECTRICTRACEWEIGHT,ENABLED, FAULTDUTY,FEEDERINFO,FLIPFLOP,GANGOPERATED,GEMSCIRCUITMAPNUM, GEMSDISTMAPNUM,GEMSOTHERMAPNUM,GLOBALID,INSTALLATIONDATE,INSTALLATIONTYPE, INSTALLJOBNUMBER,INSTALLJOBPREFIX,INSTALLJOBYEAR,INTERRUPTINGMEDIUM,LABELTEXT, LATITUDE,LOCALOFFICEID,LOCALOPOFFICE,LOCDESC,LOCKOUTNUM, LONGITUDE,MANUFACTURER,MATERIALCODE,MAXINTERRUPTINGCURRENT,MCRATING, MOUNTINGTYPE,MULTIFUNCTIONALIDC,NORMALPOSITION,NORMALPOSITION_A,NORMALPOSITION_B, NORMALPOSITION_C,NUMBEROFPHASES,OBJECTID,OKTOBYPASS,OPERATINGAS, OPERATINGNUMBER,OPERATINGVOLTAGE,PHASEDESIGNATION,PHASINGVERIFIEDSTATUS,POTENTIAL, RECLOSEBLOCKIDC,REGION,RELAYCONTROLGUID,RELAYCONTROLOBJECTID,REPLACEGUID, RETIREDATE,SAPEQUIPID,SECTIONALIZERCUTOUT,SERIALNUMBER,SHAPE, SOURCESIDEDEVICEID,STATUS,STRUCTURECONVID,STRUCTUREGUID,SUBBANKCONVID, SUBBANKGUID,SUBTYPECD,SUPERVISORYCONTROL,SWITCHMODEIDC,SYMBOLNUMBER, SYMBOLROTATION,TEMPEQUIPID,TIESWITCHIDC,VAULT,VERSIONNAME, YEARMANUFACTURED,ZIP,"ModelNumber","PartNumber","SAP Object Type", STRUCTURENUMBER,CREATIONUSER,DATECREATED,DATEMODIFIED,LASTUSER) TABLESPACE EDGIS PCTUSED 0 PCTFREE 10 INITRANS 2 MAXTRANS 255 STORAGE ( INITIAL 64K NEXT 1M MINEXTENTS 1 MAXEXTENTS UNLIMITED PCTINCREASE 0 BUFFER_POOL DEFAULT ) NOCACHE NOLOGGING NOCOMPRESS BUILD IMMEDIATE REFRESH FORCE ON DEMAND WITH PRIMARY KEY AS 
/* Formatted on 7/2/2019 01:16:32 PM (QP5 v5.313) */
(SELECT a.ANCILLARYROLE,
        a.ANIMALGUARDTYPE,
        a.AREASERVED,
        a.AUTOTRANSFERIDC,
        a.BUCKETTRUCKIDC,
        a.BYPASSPLANS,
        a.BYPASSSWITCH,
        a.CALTRANSPERMITIDC,
        a.CCRATING,
        a.CEDSADEVICEID,
        a.CIRCUITID,
        a.CIRCUITID2,
        a.CITY,
        a.CLOSEUPDATE,
        a.COASTALIDC,
        a.COMMENTS,
        a.COMPLEXDEVICEIDC,
        a.CONVCIRCUITID,
        a.CONVCIRCUITID2,
        a.CONVERSIONID,
        a.CONVERSIONWORKPACKAGE,
        a.COUNTY,
        a.CTRATIO,
        a.CUSTOMEROWNED,
        a.DEVICETYPE,
        a.DISCONNECTIDC,
        a.DISTRICT,
        a.DIVISION,
        a.ELECTRICTRACEWEIGHT,
        a.ENABLED,
        a.FAULTDUTY,
        a.FEEDERINFO,
        a.FLIPFLOP,
        a.GANGOPERATED,
        a.GEMSCIRCUITMAPNUM,
        a.GEMSDISTMAPNUM,
        a.GEMSOTHERMAPNUM,
        a.GLOBALID,
        a.INSTALLATIONDATE,
        a.INSTALLATIONTYPE,
        a.INSTALLJOBNUMBER,
        a.INSTALLJOBPREFIX,
        a.INSTALLJOBYEAR,
        a.INTERRUPTINGMEDIUM,
        a.LABELTEXT,
        a.LATITUDE,
        a.LOCALOFFICEID,
        a.LOCALOPOFFICE,
        a.LOCDESC,
        a.LOCKOUTNUM,
        a.LONGITUDE,
        a.MANUFACTURER,
        a.MATERIALCODE,
        a.MAXINTERRUPTINGCURRENT,
        a.MCRATING,
        a.MOUNTINGTYPE,
        a.MULTIFUNCTIONALIDC,
        a.NORMALPOSITION,
        a.NORMALPOSITION_A,
        a.NORMALPOSITION_B,
        a.NORMALPOSITION_C,
        a.NUMBEROFPHASES,
        a.OBJECTID,
        a.OKTOBYPASS,
        a.OPERATINGAS,
        a.OPERATINGNUMBER,
        a.OPERATINGVOLTAGE,
        a.PHASEDESIGNATION,
        a.PHASINGVERIFIEDSTATUS,
        a.POTENTIAL,
        a.RECLOSEBLOCKIDC,
        a.REGION,
        a.RELAYCONTROLGUID,
        a.RELAYCONTROLOBJECTID,
        a.REPLACEGUID,
        a.RETIREDATE,
        a.SAPEQUIPID,
        a.SECTIONALIZERCUTOUT,
        a.SERIALNUMBER,
        a.SHAPE,
        a.SOURCESIDEDEVICEID,
        a.STATUS,
        a.STRUCTURECONVID,
        a.STRUCTUREGUID,
        a.SUBBANKCONVID,
        a.SUBBANKGUID,
        a.SUBTYPECD,
        a.SUPERVISORYCONTROL,
        a.SWITCHMODEIDC,
        a.SYMBOLNUMBER,
        a.SYMBOLROTATION,
        a.TEMPEQUIPID,
        a.TIESWITCHIDC,
        a.VAULT,
        a.VERSIONNAME,
        a.YEARMANUFACTURED,
        a.ZIP,
        CAST (NULL AS VARCHAR2 (50))
            AS "ModelNumber",
        CAST (NULL AS VARCHAR2 (50))
            AS "PartNumber",
        DECODE (a.subtypecd,
                2, 'ED.INTR',
                3, 'ED.RECL',
                8, 'ED.SECT',
                NULL)
            AS "SAP Object Type",
        b.StructureNumber,
        NVL (a.CREATIONUSER, a.LASTUSER)
            CREATIONUSER,
        NVL (a.DATECREATED, a.DATEMODIFIED)
            DATECREATED,
        NVL (a.DATEMODIFIED, a.DATECREATED)
            DATEMODIFIED,
        NVL (a.LASTUSER, a.CREATIONUSER)
            LASTUSER
   FROM edgis.zz_MV_DYNAMICPROTECTIVEDEVICE a, EDGIS.STRUCTURES_INTRM_VW b
  WHERE a.GLOBALID = b.DEVICEGUID(+) AND a.subtypecd IN (2, 3, 8))
/


COMMENT ON MATERIALIZED VIEW EDGIS.DYNAMICPROTECTIVEDEVICE_FVW IS 'snapshot table for snapshot EDGIS.DYNAMICPROTECTIVEDEVICE_FVW'
/

Prompt Grants on MATERIALIZED VIEW DYNAMICPROTECTIVEDEVICE_FVW TO GIS_SAP_RECON to GIS_SAP_RECON;
GRANT SELECT ON EDGIS.DYNAMICPROTECTIVEDEVICE_FVW TO GIS_SAP_RECON
/