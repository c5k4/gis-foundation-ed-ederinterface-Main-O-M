Prompt drop Procedure CCBTOGIS_GIS_INSERTS;
DROP PROCEDURE PGEDATA.CCBTOGIS_GIS_INSERTS
/

Prompt Procedure CCBTOGIS_GIS_INSERTS;
--
-- CCBTOGIS_GIS_INSERTS  (Procedure) 
--
CREATE OR REPLACE 
PROCEDURE ccbtogis_gis_inserts(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
-- 10/28/19 - tjj4 - modified the procedure to retain the most recent record CCB
--                   sent us for any new spids and delete the earlier versions
--                   A brand new service point should not have any usage so Action
--                   column being null in pge_ccbtoedgis_stg is not consideration
--                   ie we don't need to be concerned that we might be deleting
--                   a record that contains usage information.
--                   a brand new ccb spid has no usage for 2 months, a spid that's
--                   aged out and re-activated due to a new customer starting up
--                   service also has no usage for 2 months

dbms_output.put_line('Deleting duplicate records for new service points');
DELETE FROM pgedata.pge_ccbtoedgis_stg s1
WHERE servicepointid IN (SELECT servicepointid
                           FROM pgedata.pge_ccb_sp_action
                          WHERE action = 'GISI')
AND datecreated < (SELECT max(datecreated)
                     FROM pgedata.pge_ccbtoedgis_stg s2
                    WHERE s2.servicepointid = s1.servicepointid);

dbms_output.put_line('Performing inserts into Servicepoint GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name) /* CCBTOGIS_GIS_INSERTS_V1_SETVER */  ;
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1) /* CCBTOGIS_GIS_INSERTS_V1_STARTEDIT*/;
insert into EDGIS.ZZ_MV_SERVICEPOINT(
     GLOBALID,
     CREATIONUSER, --
     DATECREATED, --
     DATEMODIFIED, --
     LASTUSER,   --
     CONVERSIONID, --
     CONVERSIONWORKPACKAGE, --
     SUBTYPECD, --
     METERID, --
     STATUS, --
     SERVICEPOINTID,
     UNIQUESPID,
     INSERVICEDATE,
     STREETNAME1,
     STREETNAME2,
     STATE, --
     COUNTY, --
     ZIP,
     GPSLATITUDE, --
     GPSLONGITUDE, --
     GPSSOURCE, --
     SOURCEACCURACY, --
     ELEVATION, --
     METERNUMBER,
     BILLINGCYCLE,
     METERROUTE,
     PREMISETYPE,
     PREMISEID,
     TOWNSHIPTERRITORYCODE,
     DISTRICT, --
     DIVISION, --
     NETENERGYMETERING,
     APNNUM, --
     NOTRANSFORMERIDC, --
     ESSENTIALCUSTOMERIDC,
     SENSITIVECUSTOMERIDC,--
     LIFESUPPORTIDC,--
     RATESCHEDULE,
     SERVICEAGREEMENTID,
     ACCOUNTNUM,
     DATASOURCE, --
     AREACODE, --
     PHONENUM, --
     ACCOUNTID, --
     CUSTOMERCLASS, --
     CUSTOMERTYPE,
     SPECIALCUSTOMERTYPE, --
     REVENUEACCOUNTCODE,
     MAILNAME1, --
     MAILNAME2, --
     MAILSTREETNUM, --
     MAILSTREETNAME1, --
     MAILSTREETNAME2, --
     MAILCITY, --
     MAILSTATE, --
     MAILZIPCODE, --
     NAICS,
     LOADSOURCEGUID, --
     LOADSOURCECONVID, --
     SERVICELOCATIONGUID, --
     SERVICELOCATIONCONVID, --
     CGC12,
     PRIMARYMETERGUID, --
     GEMSOTHERMAPNUM,
     SECONDARYGENERATIONGUID, --
     PRIMARYGENERATIONGUID, --
     STREETNUMBER,
     TRANSFORMERGUID, --
     LOCALOFFICEID,
     CITY,
     DCSERVICELOCATIONGUID, --
     DCRECTIFIERGUID, --
     SDE_STATE_ID --
)
select
     EDGIS.GDB_GUID as GLOBALID, --
      'GIS_I_WRITE' as CREATIONUSER, --
      sysdate as DATECREATED, --
      sysdate as DATEMODIFIED, --
      'GIS_I_WRITE' as LASTUSER, --
     '' as CONVERSIONID, --
      '' as CONVERSIONWORKPACKAGE, --
      '1' as SUBTYPECD, --
      '' as METERID, --
      '5' as STATUS, --
      STAGE.SERVICEPOINTID,
      STAGE.UNIQUESPID,
      STAGE.INSERVICEDATE,
      STAGE.STREETNAME1,
      STAGE.STREETNAME2,
      'CA' as STATE, --
      '' as COUNTY, --
      STAGE.ZIP,
      '' as GPSLATITUDE, --
      '' as GPSLONGITUDE, --
      '' as GPSSOURCE, --
      '' as SOURCEACCURACY, --
      '' as ELEVATION, --
      STAGE.METERNUMBER,
      STAGE.BILLINGCYCLE,
      STAGE.METERROUTE,
      STAGE.PREMISETYPE,
      STAGE.PREMISEID,
      STAGE.TOWNSHIPTERRITORYCODE,
      '' as DISTRICT, --
      '' as DIVISION, --
      STAGE.NETENERGYMETERING,
      '' as APNNUM, --
     '' as NOTRANSFORMERIDC, --
      STAGE.ESSENTIALCUSTOMERIDC,
      '' as SENSITIVECUSTOMERIDC, --
      '' as LIFESUPPORTIDC, --
     STAGE.RATESCHEDULE,
      STAGE.SERVICEAGREEMENTID,
      STAGE.ACCOUNTNUM,
      '' as DATASOURCE, --
      '' as AREACODE, --
      '' as PHONENUM, --
      '' as ACCOUNTID, --
      '' as CUSTOMERCLASS, --
     STAGE.CUSTOMERTYPE,
     '' as SPECIALCUSTOMERTYPE, --
      STAGE.REVENUEACCOUNTCODE,
      '' as MAILNAME1, --
      '' as MAILNAME2, --
      '' as MAILSTREETNUM, --
      '' as MAILSTREETNAME1, --
      '' as MAILSTREETNAME2, --
      '' as MAILCITY, --
      '' as MAILSTATE, --
      '' as MAILZIPCODE, --
      STAGE.NAICS,
     '' as LOADSOURCEGUID, --
     '' as LOADSOURCECONVID, --
     '' as SERVICELOCATIONGUID, --
     '' as SERVICELOCATIONCONVID, --
      STAGE.CGC12,
     CGC_GUID.PRI_GUID as PRIMARYMETERGUID, --
      '' as GEMSOTHERMAPNUM, --
     '' as SECONDARYGENERATIONGUID, --
      '' as PRIMARYGENERATIONGUID, --
      STAGE.STREETNUMBER,
     CGC_GUID.XMFR_GUID as TRANSFORMERGUID, --
      STAGE.LOCALOFFICEID as LOCALOFFICEID, --
      STAGE.CITY,
     '' as DCSERVICELOCATIONGUID, --
      '' as DCRECTIFIERGUID, --
      '' as SDE_STATE_ID /* CCBTOGIS_GIS_INSERTS_V1_SPINSERT */
     from PGEDATA.PGE_CCBTOEDGIS_STG STAGE
     left outer join (select min(all_sources.XMFR_GUID) XMFR_GUID, all_sources.CGC12,min(all_sources.PRI_GUID) PRI_GUID  from (
          select globalid XMFR_GUID,NVL(CGC12,000000000000) CGC12,'' PRI_GUID from EDGIS.ZZ_MV_TRANSFORMER where CGC12 is not NULL
         union all
         select '' XFMR_GUID,NVL(CGC12,000000000000) CGC12,globalid PRI_GUID from EDGIS.ZZ_MV_PRIMARYMETER
            where NVL(CGC12,000000000000) not in (select NVL(CGC12,000000000000) CGC12 from EDGIS.ZZ_MV_TRANSFORMER) and CGC12 is not NULL
     ) all_sources   group by all_sources.CGC12) CGC_GUID
     on NVL(STAGE.CGC12,000000000000)=NVL(CGC_GUID.CGC12,000000000000)
     where STAGE.SERVICEPOINTID in ( select ACTION.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION ACTION where ACTION.ACTION = 'GISI' ) ;
commit;

dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2) /* CCBTOGIS_GIS_INSERTS_V1_STOPEDIT*/ ;
END CCBTOGIS_GIS_INSERTS;
/

GRANT EXECUTE ON ccbtogis_gis_inserts TO gis_i_write
/
GRANT EXECUTE ON ccbtogis_gis_inserts TO edgis
/
GRANT EXECUTE ON ccbtogis_gis_inserts TO sde
/
GRANT EXECUTE ON ccbtogis_gis_inserts TO gis_i
/
GRANT DEBUG ON ccbtogis_gis_inserts TO gis_i_write
/
GRANT DEBUG ON ccbtogis_gis_inserts TO edgis
/
GRANT DEBUG ON ccbtogis_gis_inserts TO sde
/
GRANT DEBUG ON ccbtogis_gis_inserts TO gis_i
/

