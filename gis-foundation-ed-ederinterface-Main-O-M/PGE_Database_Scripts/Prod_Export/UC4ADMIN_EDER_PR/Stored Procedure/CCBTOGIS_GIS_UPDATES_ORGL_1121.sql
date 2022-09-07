--------------------------------------------------------
--  DDL for Procedure CCBTOGIS_GIS_UPDATES_ORGL_1121
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "UC4ADMIN"."CCBTOGIS_GIS_UPDATES_ORGL_1121" (version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing updates to the GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name) /* CCBTOGIS_GIS_UPDATES_V1_SETVER */ ;
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1) /* CCBTOGIS_GIS_UPDATES_V1_STARTEDIT*/ ;
    DECLARE
CURSOR update_rows_list is select
      sysdate as DATEMODIFIED,
      'GIS_I_WRITE' as LASTUSER,
      USTAGE.UNIQUESPID,
      USTAGE.INSERVICEDATE,
      USTAGE.STREETNAME1,
      USTAGE.STREETNAME2,
      USTAGE.ZIP,
      USTAGE.METERNUMBER,
      USTAGE.BILLINGCYCLE,
      USTAGE.METERROUTE,
      USTAGE.PREMISETYPE,
      USTAGE.TOWNSHIPTERRITORYCODE,
      USTAGE.NETENERGYMETERING,
      USTAGE.ESSENTIALCUSTOMERIDC,
      USTAGE.RATESCHEDULE,
      USTAGE.SERVICEAGREEMENTID,
      USTAGE.ACCOUNTNUM,
      USTAGE.CUSTOMERTYPE,
      USTAGE.REVENUEACCOUNTCODE,
      USTAGE.NAICS,
      USTAGE.CGC12,
      USTAGE.STREETNUMBER,
      USTAGE.CITY,
      USTAGE.LOCALOFFICEID,
      USTAGE.OBJECTID,
      USTAGE.SERVICEPOINTID,
      USTAGE.PREMISEID,
      -- USTAGE.DISTRICT,
      -- USTAGE.DIVISION,
      -- USTAGE.ACCOUNTID,
      -- USTAGE.METERID,
      -- USTAGE.LOADSOURCECONVID,
      -- USTAGE.TRANSFORMERGUID,
      -- USTAGE.MAILNAME1,
      -- USTAGE.MAILNAME2,
      -- USTAGE.AREACODE,
      -- USTAGE.PHONENUM,
      -- USTAGE.MAILSTREETNUM,
      -- USTAGE.MAILSTREETNAME1,
      -- USTAGE.MAILSTREETNAME2,
      -- USTAGE.MAILCITY,
      -- USTAGE.MAILSTATE,
      -- USTAGE.MAILZIPCODE,
      -- USTAGE.SENSITIVECUSTOMERIDC,
      -- USTAGE.LIFESUPPORTIDC,
      -- USTAGE.MEDICALBASELINEIDC,
      -- USTAGE.COMMUNICATIONPREFERENCE,
      'CA' as STATE,
      USTAGE.PHASE  -- ME Q1 2020
      from PGEDATA.PGE_CCBTOEDGIS_STG USTAGE
      where
      USTAGE.OBJECTID  is not null
      and NVL(USTAGE.SERVICEPOINTID,' ') IN (
         SELECT NVL(act.SERVICEPOINTID,' ') from
         PGEDATA.PGE_CCB_SP_ACTION act
         where act.ACTION='GISU'
         );
        sqlstm   VARCHAR2(10000);
        tot_cnt       NUMBER;
        custType   VARCHAR2(3);
BEGIN
  tot_cnt := 0;
for USTAGE in update_rows_list loop
    tot_cnt := tot_cnt + 1 ;
    sqlstm := 'UPDATE edgis.zz_mv_servicepoint servpt SET
    DATEMODIFIED='''||USTAGE.DATEMODIFIED||''' ,
    LASTUSER='''||USTAGE.LASTUSER||''' ,
    UNIQUESPID='''||USTAGE.UNIQUESPID||''' ,
    INSERVICEDATE='''||USTAGE.INSERVICEDATE||''' ,
    STREETNAME1='''||replace(USTAGE.STREETNAME1, Chr(39), '''''')||''' ,
    STREETNAME2='''||replace(USTAGE.STREETNAME2, Chr(39), '''''')||''',
    ZIP='''||USTAGE.ZIP||''',
    METERNUMBER='''||USTAGE.METERNUMBER||''' ,
    BILLINGCYCLE='''||USTAGE.BILLINGCYCLE||''' ,
    METERROUTE='''||USTAGE.METERROUTE||''' ,
    PREMISETYPE='''||USTAGE.PREMISETYPE||''' ,
    TOWNSHIPTERRITORYCODE='''||USTAGE.TOWNSHIPTERRITORYCODE||''' ,
    NETENERGYMETERING='''||USTAGE.NETENERGYMETERING||''',
    ESSENTIALCUSTOMERIDC='''||USTAGE.ESSENTIALCUSTOMERIDC||''',
    RATESCHEDULE='''||USTAGE.RATESCHEDULE||''',
    SERVICEAGREEMENTID='''||USTAGE.SERVICEAGREEMENTID||''',
    ACCOUNTNUM='''||USTAGE.ACCOUNTNUM||''',
    CUSTOMERTYPE=''' || USTAGE.CUSTOMERTYPE || ''',
    REVENUEACCOUNTCODE='''|| USTAGE.REVENUEACCOUNTCODE||''',
    NAICS='''||USTAGE.NAICS||''',
    CGC12='''||USTAGE.CGC12||''',
    STREETNUMBER='''||replace(USTAGE.STREETNUMBER, Chr(39), '''''')||''',
    CITY='''||USTAGE.CITY||''',
    LOCALOFFICEID='''||USTAGE.LOCALOFFICEID||''',
    PREMISEID='''||USTAGE.PREMISEID||''',
    STATE= '''||USTAGE.STATE||''',
    METER_PHASE = '''||USTAGE.PHASE||'''   --ME Q1 2020
    where servpt.OBJECTID='||USTAGE.OBJECTID||' /* CCBTOGIS_GIS_UPDATES_V1_SPUPDATE */ ' ;
   -- dbms_output.put_line('UNIQUESPID=' || USTAGE.UNIQUESPID);
   execute immediate sqlstm ;
   -- dbms_output.put_line(sqlstm);
commit;
   update PGE_CCB_SP_ACTION set TLMPROCESSED=sysdate where servicepointid=USTAGE.SERVICEPOINTID;
commit;
      exit when tot_cnt > 5600000 ;
end loop;
end;
commit;
dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2) /* CCBTOGIS_GIS_UPDATES_V1_STOPEDIT*/ ;
END  CCBTOGIS_GIS_UPDATES_ORGL_1121;
