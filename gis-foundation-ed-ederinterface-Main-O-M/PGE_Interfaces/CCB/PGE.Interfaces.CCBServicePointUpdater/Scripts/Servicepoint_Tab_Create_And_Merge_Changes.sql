set heading off;
/*
set linesize 120
set pagesize 1000
set pagesize 50000
spool create_sptab.txt append;
set autotrace on
set timing on
alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';
*/
-- Create log file for run to review execution times and errors
spool create_sptab.txt append;

alter session set NLS_DATE_FORMAT='Dy DD-Mon-YYYY HH24:MI:SS';

-- Create a variable and assign a name to it, so that we can create and edit a version
VARIABLE mv_version NVARCHAR2(15);
EXEC :mv_version := 'CCBWithViews';

-- Deletes the version if it already exists
SELECT 'Deleting previous version',sysdate from dual;
EXEC sde.version_user_ddl.delete_version(:mv_version);

-- Creates the version that we will edit in
SELECT 'Creating new version',sysdate from dual;
EXEC sde.version_user_ddl.create_version('sde.DEFAULT', :mv_version, sde.version_util.C_take_name_as_given, sde.version_util.C_version_public, 'IUDs from CEDSA');

-- Sets our version to the version that we just created
SELECT 'Setting our version to the created version',sysdate from dual;
EXEC sde.version_util.set_current_version(:mv_version);

-- Turn on per SQL event to track the length of time of each execution
set timing on;

-- Remove table created from previous executions. Will error if this is the first run on the instance
SELECT 'Dropping old SERVICEPOINT_TAB table',sysdate from dual;
DROP TABLE SERVICEPOINT_TAB;

-- Creates the source table for comparisons against the Servicepoint multiversioned view
SELECT 'Creating new SERVICEPOINT_TAB table',sysdate from dual;
CREATE TABLE SERVICEPOINT_TAB AS
SELECT
mtr.METER_ID as METERID,
CAST('1' as NUMBER(38)) as SUBTYPECD, 
CAST(mtr.SERVICE_POINT_ID as NVARCHAR2(100)) as SERVICEPOINTID, 
CAST(mtr.ACCT_ID as NVARCHAR2(11)) as ACCOUNTID, 
CAST(mtr.ACCT_ID as NVARCHAR2(10)) as ACCOUNTNUM, 
CAST(mtr.SVC_ST_# as NVARCHAR2(12)) as STREETNUMBER,
CAST(mtr.SVC_ST_NAME as NVARCHAR2(64)) as STREETNAME1,
CAST(mtr.SVC_ST_NAME2 as NVARCHAR2(64)) as STREETNAME2, 
CAST(mtr.SVC_CITY as NVARCHAR2(30)) as MAILCITY, 
CAST(mtr.SVC_STATE as NVARCHAR2(2)) as MAILSTATE, 
CAST(mtr.SVC_ZIP as NVARCHAR2(10)) as MAILZIPCODE,
CAST(str.CITY as NVARCHAR2(40)) as CITY, -- This is being truncated from 50 to 40
CAST(mtr.SVC_ZIP as NVARCHAR2(10)) as ZIP,
CAST(mtr.UNQSPID as NVARCHAR2(20)) as UNIQUESPID, 
mtr.TRF_ID as LOADSOURCECONVID,  -- Maps to CEDSADEVICEID in EDGIS.Transformer
CAST('' as CHAR(38)) as TRANSFORMERGUID,
CAST(DECODE(mtr.ESSENTIAL, 0, 'N', 1, 'Y') as NVARCHAR2(1)) as ESSENTIALCUSTOMERIDC, 
CAST(mtr.LOCAL_OFFICE as NVARCHAR2(4)) as LOCALOFFICEID, 
CAST(mtr.SERVICE_AGREEMENT_ID as NVARCHAR2(10)) as SERVICEAGREEMENTID, 
CAST(mtr.CUST_TYP as NVARCHAR2(3)) as CUSTOMERTYPE, 
mtr.SVC_DATE as INSERVICEDATE,
CAST(mtr.NAICS as NUMBER(38)) as NAICS, 
CAST(mtr.BILLING_CYCLE as NVARCHAR2(4)) as BILLINGCYCLE, 
CAST(mtr.ROUTE as NVARCHAR2(10)) as METERROUTE, 
CAST(mtr.REV_ACCT_CD as NVARCHAR2(8)) as REVENUEACCOUNTCODE, 
CAST(mtr.RATE_SCHED as NVARCHAR2(8)) as RATESCHEDULE, 
CAST(mtr.PREMISE_TYPE as NVARCHAR2(100)) as PREMISETYPE,
CAST(mtr.TOWNSHIP_TERRITORY_CD as NVARCHAR2(10)) as TOWNSHIPTERRITORYCODE, 
CAST(mtr.NEM as NVARCHAR2(1)) as NETENERGYMETERING,
CAST(1 as NVARCHAR2(6)) as CONVERSIONWORKPACKAGE,
CAST(mtr.METER_NUMBER as NVARCHAR2(18)) as METERNUMBER,
CAST(xfmr.CGC#12 as NUMBER(38,8)) as CGC12,
CAST(loc.DIVISION as NUMBER(5)) as DIVISION,
CAST(loc.DISTRICT as NUMBER(5)) as DISTRICT,
CAST('5' as NUMBER(2)) as STATUS,
CAST('CA' as NVARCHAR2(2)) as STATE
/* This section is for empty values that do not need to be in the table. They will simply be ignored when the compares are done. Leaving them in this file for future reference
LTRIM(mtr.SVC_ST_# || ' ' || mtr.SVC_ST_NAME) as ADDRESS, -- DOES NOT EXIST IN THE CURRENT TABLE
CAST('' as NVARCHAR2(64)) as MAILSTREETNAME1,
CAST('' as NVARCHAR2(64)) as MAILSTREETNAME2,
CAST('' as NVARCHAR2(10)) as GEMSOTHERMAPNUM,
CAST('' as NVARCHAR2(30)) as APNNUM,
CAST('' as NUMBER(3)) as AREACODE,
CAST('' as NUMBER(5)) as COUNTY,
CAST('' as NVARCHAR2(20)) as CUSTOMERCLASS,
CAST('' as NVARCHAR2(30)) as APNNUM,
CAST('' as NVARCHAR2(20)) as DATASOURCE,
CAST('' as NVARCHAR2(2)) as ELEVATION,
CAST('' as NUMBER(9,7)) as GPSLATITUDE,
CAST('' as NUMBER(10,7)) as GPSLONGITUDE,
CAST('' as NVARCHAR2(5)) as GPSSOURCE,
CAST('' as NVARCHAR2(1)) as LIFESUPPORTIDC,
CAST('' as NVARCHAR2(50)) as MAILNAME1,
CAST('' as NVARCHAR2(50)) as MAILNAME2,
CAST('' as NVARCHAR2(12)) as MAILSTREETNUM,
CAST('' as NVARCHAR2(13)) as PHONENUM,
CAST('' as NUMBER(12)) as PREMISEID,
CAST('' as NVARCHAR2(10)) as ACCOUNTNUM,
CAST('' as NVARCHAR2(1)) as SENSITIVECUSTOMERIDC,
CAST('' as NUMBER(38)) as SOURCEACCURACY
*/
FROM (select * from CEDSADATA.METER  WHERE METER.local_office in (SELECT loc.local_office FROM CEDSADATA.LOCAL_OFFICE loc WHERE loc.DIVISION IN (8,10,11,12,13,14,15,16,17,18,19))) mtr -- Filtering on Region 1's local offices. Will need to change this only if it is found that specific local offices are missing from existing data
LEFT OUTER JOIN CEDSADATA.LOCAL_OFFICE loc ON mtr.LOCAL_OFFICE  = loc.LOCAL_OFFICE
LEFT OUTER JOIN CEDSADATA.TRANSFORMER xfmr ON mtr.trf_id = xfmr.DEVICE_ID 
LEFT OUTER JOIN CEDSADATA.DEVICE dev ON xfmr.DEVICE_ID = dev.DEVICE_ID 
LEFT OUTER JOIN CEDSADATA.STRUCTURE str ON dev.STRUC_ID = str.STRUC_ID;

select 'Getting the counts from the new table',sysdate from dual;
select count(*) from servicepoint_tab;

exec dbms_stats.delete_table_stats(user,'SERVICEPOINT_TAB');
-- Supports the inserts, updates, and actual reading of source data from SERVICEPOINT_TAB for multiversioned views
create index spid_sptab on servicepoint_tab(SERVICEPOINTID); 
-- Supports the inserts, updates and deletes.
create index ldsid_sptab on servicepoint_tab(LOADSOURCECONVID);
exec dbms_stats.gather_table_stats(user,'SERVICEPOINT_TAB');

SELECT 'Inserting all additional servicepoints that have transformers in the GIS, outside the core districts to handle corner cases' from dual;
insert into SERVICEPOINT_TAB 
SELECT
mtr.METER_ID as METERID,
CAST('1' as NUMBER(38)) as SUBTYPECD, 
CAST(mtr.SERVICE_POINT_ID as NVARCHAR2(100)) as SERVICEPOINTID, 
CAST(mtr.ACCT_ID as NVARCHAR2(11)) as ACCOUNTID, 
CAST(mtr.ACCT_ID as NVARCHAR2(10)) as ACCOUNTNUM, 
CAST(mtr.SVC_ST_# as NVARCHAR2(12)) as STREETNUMBER,
CAST(mtr.SVC_ST_NAME as NVARCHAR2(64)) as STREETNAME1,
CAST(mtr.SVC_ST_NAME2 as NVARCHAR2(64)) as STREETNAME2, 
CAST(mtr.SVC_CITY as NVARCHAR2(30)) as MAILCITY, 
CAST(mtr.SVC_STATE as NVARCHAR2(2)) as MAILSTATE, 
CAST(mtr.SVC_ZIP as NVARCHAR2(10)) as MAILZIPCODE,
CAST(str.CITY as NVARCHAR2(40)) as CITY, -- This is being truncated from 50 to 40
CAST(mtr.SVC_ZIP as NVARCHAR2(10)) as ZIP,
CAST(mtr.UNQSPID as NVARCHAR2(20)) as UNIQUESPID, 
mtr.TRF_ID as LOADSOURCECONVID,  -- Maps to CEDSADEVICEID in EDGIS.Transformer
CAST('' as CHAR(38)) as TRANSFORMERGUID,
CAST(DECODE(mtr.ESSENTIAL, 0, 'N', 1, 'Y') as NVARCHAR2(1)) as ESSENTIALCUSTOMERIDC, 
CAST(mtr.LOCAL_OFFICE as NVARCHAR2(4)) as LOCALOFFICEID, 
CAST(mtr.SERVICE_AGREEMENT_ID as NVARCHAR2(10)) as SERVICEAGREEMENTID, 
CAST(mtr.CUST_TYP as NVARCHAR2(3)) as CUSTOMERTYPE, 
mtr.SVC_DATE as INSERVICEDATE,
CAST(mtr.NAICS as NUMBER(38)) as NAICS, 
CAST(mtr.BILLING_CYCLE as NVARCHAR2(4)) as BILLINGCYCLE, 
CAST(mtr.ROUTE as NVARCHAR2(10)) as METERROUTE, 
CAST(mtr.REV_ACCT_CD as NVARCHAR2(8)) as REVENUEACCOUNTCODE, 
CAST(mtr.RATE_SCHED as NVARCHAR2(8)) as RATESCHEDULE, 
CAST(mtr.PREMISE_TYPE as NVARCHAR2(100)) as PREMISETYPE,
CAST(mtr.TOWNSHIP_TERRITORY_CD as NVARCHAR2(10)) as TOWNSHIPTERRITORYCODE, 
CAST(mtr.NEM as NVARCHAR2(1)) as NETENERGYMETERING,
CAST(1 as NVARCHAR2(6)) as CONVERSIONWORKPACKAGE,
CAST(mtr.METER_NUMBER as NVARCHAR2(18)) as METERNUMBER,
CAST(xfmr.CGC#12 as NUMBER(38,8)) as CGC12,
CAST(loc.DIVISION as NUMBER(5)) as DIVISION,
CAST(loc.DISTRICT as NUMBER(5)) as DISTRICT,
CAST('5' as NUMBER(2)) as STATUS,
CAST('CA' as NVARCHAR2(2)) as STATE
FROM (select * from CEDSADATA.METER  WHERE METER.SERVICE_POINT_ID in (
select SERVICE_POINT_ID from cedsadata.meter MTR2 where MTR2.TRF_ID in (select gisxfmr.CEDSADEVICEID from edgis.zz_mv_transformer gisxfmr where gisxfmr.CEDSADEVICEID is not null group by gisxfmr.CEDSADEVICEID)
minus
select cast(servicepointid as VARCHAR2(10))  from servicepoint_tab )
) mtr LEFT OUTER JOIN CEDSADATA.LOCAL_OFFICE loc ON mtr.LOCAL_OFFICE  = loc.LOCAL_OFFICE
LEFT OUTER JOIN CEDSADATA.TRANSFORMER xfmr ON mtr.trf_id = xfmr.DEVICE_ID 
LEFT OUTER JOIN CEDSADATA.DEVICE dev ON xfmr.DEVICE_ID = dev.DEVICE_ID 
LEFT OUTER JOIN CEDSADATA.STRUCTURE str ON dev.STRUC_ID = str.STRUC_ID;

select 'Getting the counts of all servicepoint_tab rows',sysdate from dual;
select count(*) from servicepoint_tab;

select 'Gathering stats on SERVICEPOINT_TAB',sysdate from dual;
exec dbms_stats.delete_table_stats(user,'SERVICEPOINT_TAB');
exec dbms_stats.gather_table_stats(user,'SERVICEPOINT_TAB');

select 'Create XFMR mapping table',sysdate from dual;
drop table temp_xfmr_mapping ;

create table temp_xfmr_mapping as select spt.SERVICEPOINTID,V_xfmr_update.GLOBALID from gis_i.servicepoint_tab spt left outer join (
   select xfr.cedsadeviceid, xfr.globalid from EDGIS.zz_mv_transformer xfr  
   minus 
   select xfr2.cedsadeviceid,  xfr2.globalid from EDGIS.zz_mv_transformer xfr2 where xfr2.cedsadeviceid in (select xfr3.cedsadeviceid from EDGIS.zz_mv_transformer xfr3 group by xfr3.cedsadeviceid having count(*)>1)   
) V_xfmr_update on spt.LOADSOURCECONVID= V_xfmr_update.CEDSADEVICEID;

select 'Getting the counts from the new table',sysdate from dual;
select count(*) from temp_xfmr_mapping;

-- Supports inserts, updates, and deletes
create index temp_xfmr_spid_idx on temp_xfmr_mapping(SERVICEPOINTID,GLOBALID); 

update servicepoint_tab spt set spt.TRANSFORMERGUID = ( select tr.GLOBALID from temp_xfmr_mapping tr where tr.SERVICEPOINTID=spt.SERVICEPOINTID );
commit;



-- Remove table created from previous executions. Will error if this is the first run on the instance
SELECT 'Dropping SP_ACTION table',sysdate from dual;
drop table sp_action;

-- Creates the results table for comparisons between GIS and CEDSA
SELECT 'Creating new SP_ACTION table',sysdate from dual;
create table SP_ACTION
(
	ACTION NVARCHAR2(1),
	SERVICEPOINTID NVARCHAR2(20)
);

select 'Creating indexes on SERVICEPOINT_TAB table and SP_ACTION table',sysdate from dual;

-- Create indexes to support the interface logic and thereby reduce run times
-- Supports inserts, updates, and deletes
create index spid_SPACTION on SP_ACTION(SERVICEPOINTID); 
-- Supports inserts, updates, and delete action SQL
create index action_SPACTION on SP_ACTION(ACTION,SERVICEPOINTID); 


-- Populate the Insert values for the action. Takes all Servicepoint rows from CEDSA and subtracts the rows that are currently in GIS. The result is rows that are new in CEDSA and don't exist in GIS
SELECT 'Populating the insert values in the SP_ACTION table',sysdate from dual;
insert into SP_ACTION (ACTION,SERVICEPOINTID) select 'I',tab2.SERVICEPOINTID from (
select distinct tab.SERVICEPOINTID from servicepoint_tab tab
minus 
select distinct sp.SERVICEPOINTID from edgis.zz_mv_servicepoint sp
) tab2;

commit;

select 'Checking for duplicates in sp_action',sysdate from dual;
select servicepointid, count(*) from sp_action group by servicepointid having count(*)>1 ;

-- Populate the Delete values for the action. Takes all rows currently in GIS and subtracts the rows that are in CEDSA. The result is rows that have been deleted in CEDSA but not yet deleted in GIS.
select 'Populating the delete values in the SP_ACTION table',sysdate from dual;
insert into SP_ACTION (ACTION,SERVICEPOINTID) select 'D',tab2.SERVICEPOINTID from (
select distinct sp.SERVICEPOINTID from edgis.zz_mv_servicepoint sp
minus 
select distinct tab.SERVICEPOINTID from servicepoint_tab tab
) tab2;

commit;

select 'Checking for duplicates in sp_action',sysdate from dual;
select servicepointid, count(*) from sp_action group by servicepointid having count(*)>1 ;

--Populates the Update values for the action. The lines below group every SERVICEPOINT_TAB object including NULL columns and then look for differences with the Servicepoint multiversioned view. This is done through a group by and union logic that forces a single complete read of the SERVICEPOINT_TAB table and a single complete read of the Servicepoint multiversioned view.
select 'Populating the update values in the SP_ACTION table',sysdate from dual;
insert into SP_ACTION (ACTION,SERVICEPOINTID) SELECT
    'U',
    unique_list.SERVICEPOINTID
FROM 
( 
   Select DISTINCT total.SERVICEPOINTID from 
(
	SELECT 
spt.METERID,
spt.SERVICEPOINTID, 
spt.ACCOUNTID, 
spt.ACCOUNTNUM, 
spt.STREETNUMBER,
spt.STREETNAME1,
spt.STREETNAME2, 
spt.MAILCITY, 
spt.MAILSTATE, 
spt.MAILZIPCODE,
spt.CITY,
spt.ZIP,
spt.UNIQUESPID, 
spt.TRANSFORMERGUID, 
spt.SERVICEAGREEMENTID, 
spt.INSERVICEDATE,
spt.NAICS, 
spt.BILLINGCYCLE, 
spt.METERROUTE, 
spt.REVENUEACCOUNTCODE, 
spt.RATESCHEDULE, 
spt.PREMISETYPE,
spt.TOWNSHIPTERRITORYCODE, 
spt.NETENERGYMETERING,
spt.METERNUMBER,
spt.CUSTOMERTYPE,
spt.CGC12,
spt.DIVISION,
spt.DISTRICT,
spt.ESSENTIALCUSTOMERIDC,
spt.LOADSOURCECONVID,
spt.LOCALOFFICEID,
1 AS src1, 
TO_NUMBER(NULL) AS src2
from (select * from edgis.zz_mv_servicepoint where servicepointid in (select b1.servicepointid from servicepoint_tab b1 where b1.servicepointid is not null minus select servicepointid from sp_action) ) spt
    UNION ALL
SELECT 
tab.METERID,
tab.SERVICEPOINTID, 
tab.ACCOUNTID, 
tab.ACCOUNTNUM, 
tab.STREETNUMBER,
tab.STREETNAME1,
tab.STREETNAME2, 
tab.MAILCITY, 
tab.MAILSTATE, 
tab.MAILZIPCODE,
tab.CITY,
tab.ZIP,
tab.UNIQUESPID, 
tab.TRANSFORMERGUID, 
tab.SERVICEAGREEMENTID, 
tab.INSERVICEDATE,
tab.NAICS, 
tab.BILLINGCYCLE, 
tab.METERROUTE, 
tab.REVENUEACCOUNTCODE, 
tab.RATESCHEDULE, 
tab.PREMISETYPE,
tab.TOWNSHIPTERRITORYCODE, 
tab.NETENERGYMETERING,
tab.METERNUMBER,
tab.CUSTOMERTYPE,
tab.CGC12,
tab.DIVISION,
tab.DISTRICT,
tab.ESSENTIALCUSTOMERIDC,
tab.LOADSOURCECONVID,
tab.LOCALOFFICEID,
TO_NUMBER(NULL) AS src1, 
2 AS src2 
FROM (select * from SERVICEPOINT_TAB where servicepointid in (select b3.servicepointid from edgis.zz_mv_servicepoint b3 where b3.servicepointid is not null minus select servicepointid from sp_action)) tab
) TOTAL
GROUP BY 
METERID,
SERVICEPOINTID, 
ACCOUNTID, 
ACCOUNTNUM, 
STREETNUMBER,
STREETNAME1,
STREETNAME2, 
MAILCITY, 
MAILSTATE, 
MAILZIPCODE,
CITY,
ZIP,
UNIQUESPID, 
TRANSFORMERGUID,
SERVICEAGREEMENTID,  
INSERVICEDATE,
NAICS, 
BILLINGCYCLE, 
METERROUTE, 
REVENUEACCOUNTCODE, 
RATESCHEDULE, 
PREMISETYPE,
TOWNSHIPTERRITORYCODE, 
NETENERGYMETERING,
METERNUMBER,
CUSTOMERTYPE,
CGC12,
DIVISION,
DISTRICT,
ESSENTIALCUSTOMERIDC,
LOADSOURCECONVID,
LOCALOFFICEID
HAVING COUNT(src1) <> COUNT(src2)
) 
unique_list;
commit;

select 'Checking for duplicates in sp_action',sysdate from dual;
select servicepointid, count(*) from sp_action group by servicepointid having count(*)>1 ;

-- Toggle the timing on so that each statement execution below is timed
set timing on;

-- Sets our version to the version that we just created
SELECT 'Setting our version to the created version',sysdate from dual;
EXEC sde.version_util.set_current_version(:mv_version);

--Starts editing the version
SELECT 'Starting edits',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,1);


--Perform all inserts. Insert all rows and columns into the EDGIS.zz_mv_servicepoint multiversioned view from the SERVICEPOINT_TAB table that are marked as 'I' (for insert) in the SP_ACTION table
select 'Performing inserts into Servicepoint table in GIS',sysdate from dual;
INSERT INTO edgis.zz_mv_servicepoint
	(GLOBALID,
	CREATIONUSER,
	DATECREATED,
	DATEMODIFIED,
	LASTUSER,
	CONVERSIONWORKPACKAGE,
	SUBTYPECD,
	METERID,
	STATUS,
	SERVICEPOINTID,
	UNIQUESPID,
	INSERVICEDATE,
	STREETNAME1,
	STREETNAME2,
	STATE,
	COUNTY,
	ZIP,
	GPSLATITUDE,
	GPSLONGITUDE,
	GPSSOURCE,
	SOURCEACCURACY,
	ELEVATION,
	METERNUMBER,
	BILLINGCYCLE,
	METERROUTE,
	PREMISETYPE,
	PREMISEID,
	TOWNSHIPTERRITORYCODE,
	DISTRICT,
	DIVISION,
	NETENERGYMETERING,
	APNNUM,
	ESSENTIALCUSTOMERIDC,
	SENSITIVECUSTOMERIDC,
	LIFESUPPORTIDC,
	RATESCHEDULE,
	SERVICEAGREEMENTID,
	ACCOUNTNUM,
	DATASOURCE,
	AREACODE,
	PHONENUM,
	ACCOUNTID,
	CUSTOMERCLASS,
	CUSTOMERTYPE,
	REVENUEACCOUNTCODE,
	MAILNAME1,
	MAILNAME2,
	MAILSTREETNUM,
	MAILSTREETNAME1,
	MAILSTREETNAME2,
	MAILCITY,
	MAILSTATE,
	MAILZIPCODE,
	NAICS,
	CGC12,
	GEMSOTHERMAPNUM,
	STREETNUMBER,
	CITY,
	LOADSOURCECONVID,
	TRANSFORMERGUID) 
SELECT
	GIS_I.GDB_GUID as GLOBALID,
	'GIS_I' as CREATIONUSER,
	sysdate as DATECREATED,
	sysdate as DATEMODIFIED,
	'GIS_I' as LASTUSER,
	'1' as CONVERSIONWORKPACKAGE,
	'1' as SUBTYPECD,
	sp_tab.METERID,
	'5' as STATUS,
	sp_tab.SERVICEPOINTID,
	sp_tab.UNIQUESPID,
	sp_tab.INSERVICEDATE,
	sp_tab.STREETNAME1,
	sp_tab.STREETNAME2,
	'CA' as STATE,
	'' as COUNTY,
	sp_tab.ZIP,
	'' as GPSLATITUDE,
	'' as GPSLONGITUDE,
	'' as GPSSOURCE,
	'' as SOURCEACCURACY,
	'' as ELEVATION,
	sp_tab.METERNUMBER,
	sp_tab.BILLINGCYCLE,
	sp_tab.METERROUTE,
	sp_tab.PREMISETYPE,
	'' as PREMISEID,
	sp_tab.TOWNSHIPTERRITORYCODE,
	sp_tab.DISTRICT,
	sp_tab.DIVISION,
	sp_tab.NETENERGYMETERING,
	'' as APNNUM,
	sp_tab.ESSENTIALCUSTOMERIDC,
	'' as SENSITIVECUSTOMERIDC,
	'' as LIFESUPPORTIDC,
	sp_tab.RATESCHEDULE,
	sp_tab.SERVICEAGREEMENTID,
	sp_tab.ACCOUNTNUM,
	'' as DATASOURCE,
	'' as AREACODE,
	'' as PHONENUM,
	sp_tab.ACCOUNTID,
	'' as CUSTOMERCLASS,
	sp_tab.CUSTOMERTYPE,
	sp_tab.REVENUEACCOUNTCODE,
	'' as MAILNAME1,
	'' as MAILNAME2,
	'' as MAILSTREETNUM,
	'' as MAILSTREETNAME1,
	'' as MAILSTREETNAME2,
	sp_tab.MAILCITY,
	sp_tab.MAILSTATE,
	sp_tab.MAILZIPCODE,
	sp_tab.NAICS,
	sp_tab.CGC12,
	'' as GEMSOTHERMAPNUM,
	sp_tab.STREETNUMBER,
	sp_tab.CITY,
	sp_tab.LOADSOURCECONVID,
	sp_tab.TRANSFORMERGUID
FROM SERVICEPOINT_TAB sp_tab
WHERE sp_tab.SERVICEPOINTID in (SELECT SERVICEPOINTID FROM SP_ACTION WHERE ACTION='I');

COMMIT;

--Close version
SELECT 'Saving Version By closing it',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,2);

SELECT 'Making Sure to re-read the state of the versions',sysdate from dual;
EXEC sde.version_util.set_current_version(:mv_version);

--Re-opens the session for editing
SELECT 'Opening version for edits',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,1);

--Perform all deletes. Delete all rows from the EDGIS.zz_mv_servicepoint multiversioned view that are marked as 'D' (for delete) in the SP_ACTION table
select 'Performing deletes on Servicepoint table in GIS',sysdate from dual;
DELETE FROM edgis.zz_mv_servicepoint WHERE SERVICEPOINTID IN (SELECT SERVICEPOINTID FROM SP_ACTION WHERE ACTION='D');

COMMIT;

--Close and reopen the state to save all edits

--Close version
SELECT 'Saving Version By closing it',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,2);

SELECT 'Making Sure to re-read the state of the versions',sysdate from dual;
EXEC sde.version_util.set_current_version(:mv_version);

-- PGE IT runs this process as gis_i and it will not have permissions to do this performance step.
-- exec dbms_stats.delete_table_stats('EDGIS','A71');
-- exec dbms_stats.delete_table_stats('EDGIS','D71');

alter table SERVICEPOINT_TAB add (OBJECTID NUMBER);

drop table oids_service_temp ;

create table oids_service_temp as select servpt2.OBJECTID,servpt2.servicepointid from edgis.zz_mv_servicepoint servpt2 where servpt2.servicepointid  in  (
            select servpt4.SERVICEPOINTID from edgis.zz_mv_servicepoint servpt4
            minus
            select servpt3.SERVICEPOINTID from edgis.zz_mv_servicepoint servpt3 group by servicepointid having count(*)>1 );

--  Supports updates
create index temp_oid_spid_idx on oids_service_temp(SERVICEPOINTID,OBJECTID); 

update servicepoint_tab spt set spt.OBJECTID = ( select tr.OBJECTID from oids_service_temp tr where tr.SERVICEPOINTID=spt.SERVICEPOINTID );

commit;


--Perform all updates. Update all rows in the EDGIS.zz_mv_servicepoint multiversioned view if the SERVICEPOINTID is in the SERVICEPOINT_TAB table and the row is marked 'U' (for update) in the SP_ACTION table
select 'Performing updates on Servicepoint table in GIS',sysdate from dual;


drop table temp_status_debug;
drop sequence temp_status_debug_seq;

create table temp_status_debug (sqlstm VARCHAR2(4000),cnt_objectid NUMBER,sp_id NUMBER) ;
create sequence temp_status_debug_seq;

SELECT 'Making Sure to re-read the state of the versions',sysdate from dual;
EXEC sde.version_util.set_current_version(:mv_version);

--Re-opens the session for editing
SELECT 'Opening version for edits',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,1);

DECLARE
CURSOR insert_rows_list IS SELECT
	sysdate as DATEMODIFIED,
	'GIS_I' as LASTUSER,
	sptab.METERID,
	sptab.UNIQUESPID,
	sptab.INSERVICEDATE,
	sptab.STREETNAME1,
	sptab.STREETNAME2,
	sptab.ZIP,
	sptab.METERNUMBER,
	sptab.BILLINGCYCLE,
	sptab.METERROUTE,
	sptab.PREMISETYPE,
	sptab.TOWNSHIPTERRITORYCODE,
	sptab.DISTRICT,
	sptab.DIVISION,
	sptab.NETENERGYMETERING,
	sptab.ESSENTIALCUSTOMERIDC,
	sptab.RATESCHEDULE,
	sptab.SERVICEAGREEMENTID,
	sptab.ACCOUNTNUM,
	sptab.ACCOUNTID,
	sptab.CUSTOMERTYPE,
	sptab.REVENUEACCOUNTCODE,
	sptab.MAILCITY,
	sptab.MAILSTATE,
	sptab.MAILZIPCODE,
	sptab.NAICS,
	sptab.CGC12,
	sptab.STREETNUMBER,
	sptab.CITY,
	sptab.LOADSOURCECONVID,
	sptab.LOCALOFFICEID,
	sptab.TRANSFORMERGUID,
	sptab.OBJECTID
	FROM SERVICEPOINT_TAB sptab 
	WHERE sptab.OBJECTID  is not null
	AND sptab.SERVICEPOINTID IN (SELECT act.SERVICEPOINTID FROM SP_ACTION act WHERE act.ACTION='U') and sptab.OBJECTID not in (select sp_id from temp_status_debug )  ;
sqlstm        VARCHAR2(10000);
-- cnt_objectid  NUMBER;
tot_cnt       NUMBER;
BEGIN
  -- cnt_objectid := 0 ;
  tot_cnt := 0;
FOR sptab IN insert_rows_list LOOP	
    -- cnt_objectid := cnt_objectid + 1;
    tot_cnt := tot_cnt + 1 ;
    sqlstm := 'UPDATE edgis.zz_mv_servicepoint servpt SET DATEMODIFIED='''||sptab.DATEMODIFIED||''' ,LASTUSER='''||sptab.LASTUSER||''' ,METERID= '||sptab.METERID||' ,UNIQUESPID='''||sptab.UNIQUESPID||''' ,INSERVICEDATE='''||sptab.INSERVICEDATE||''' ,STREETNAME1='''||replace(sptab.STREETNAME1, Chr(39), '''''')||''' ,STREETNAME2='''||replace(sptab.STREETNAME2, Chr(39), '''''')||''',ZIP='''||sptab.ZIP||''',METERNUMBER='''||sptab.METERNUMBER||''' ,BILLINGCYCLE='''||sptab.BILLINGCYCLE||''' ,METERROUTE='''||sptab.METERROUTE||''' ,PREMISETYPE='''||sptab.PREMISETYPE||''' ,TOWNSHIPTERRITORYCODE='''||sptab.TOWNSHIPTERRITORYCODE||''' ,DISTRICT='||sptab.DISTRICT||',DIVISION='||sptab.DIVISION||',NETENERGYMETERING='''||sptab.NETENERGYMETERING||''',RATESCHEDULE='''||sptab.RATESCHEDULE||''',SERVICEAGREEMENTID='''||sptab.SERVICEAGREEMENTID||''',ACCOUNTNUM='''||sptab.ACCOUNTNUM||''',ACCOUNTID='''||sptab.ACCOUNTID||''',CUSTOMERTYPE='''||sptab.CUSTOMERTYPE||''',REVENUEACCOUNTCODE='''||sptab.REVENUEACCOUNTCODE||''',MAILCITY='''||sptab.MAILCITY||''',MAILSTATE='''||sptab.MAILSTATE||''',MAILZIPCODE='''||sptab.MAILZIPCODE||''',NAICS='''||sptab.NAICS||''',CGC12='''||sptab.CGC12||''',STREETNUMBER='''||replace(sptab.STREETNUMBER, Chr(39), '''''')||''',CITY='''||sptab.CITY||''',TRANSFORMERGUID='''||sptab.TRANSFORMERGUID||''',LOADSOURCECONVID='''||sptab.LOADSOURCECONVID||''',LOCALOFFICEID='''||sptab.LOCALOFFICEID||''',ESSENTIALCUSTOMERIDC='''||sptab.ESSENTIALCUSTOMERIDC||''' where servpt.OBJECTID='||sptab.OBJECTID||'  ' ;
    --dbms_output.put_line(sqlstm);
    insert into temp_status_debug (sqlstm,cnt_objectid,sp_id) values ( sqlstm ,temp_status_debug_seq.nextval,sptab.objectid);
    commit;
    execute immediate sqlstm ;
	exit when tot_cnt > 500000 ;
END LOOP;
END;
/
COMMIT;

--Close version
SELECT 'Saving Version By closing it',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,2);

SELECT 'Making Sure to re-read the state of the versions',sysdate from dual;
EXEC sde.version_util.set_current_version(:mv_version);

SELECT 'Opening version for edits',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,1);

/*
Now that we have finished the inserts/deletes/updates for the Servicepoint table, we can do the inserts and updates for the CWOT table
*/

--Find all CWOTs that have been fixed and mark them as resolved in the CWOT table
select 'Performing updates to CWOT table in GIS',sysdate from dual;
UPDATE edgis.zz_mv_cwot cwot_tab SET DATEFIXED=sysdate,RESOLVED='Y' WHERE cwot_tab.SERVICEPOINTGUID IN
(
	SELECT SP.GLOBALID FROM EDGIS.ZZ_MV_SERVICEPOINT SP WHERE SP.TRANSFORMERGUID IN (SELECT GLOBALID FROM edgis.zz_mv_transformer) and sp.globalid in (SELECT cwot2.SERVICEPOINTGUID FROM EDGIS.ZZ_MV_CWOT cwot2 WHERE cwot2.RESOLVED='N')
);

COMMIT;

--Close version
SELECT 'Saving Version By closing it',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,2);

SELECT 'Making Sure to re-read the state of the versions',sysdate from dual;
EXEC sde.version_util.set_current_version(:mv_version);

SELECT 'Opening version for edits',sysdate from dual;
EXEC sde.version_user_ddl.edit_version(:mv_version,1);

--Find all rows that are in the (now updated) GIS Servicepoint table but not referenced in the GIS Transformer table and insert them into the GIS CWOT table
select 'Performing inserts into CWOT table in GIS',sysdate from dual;
INSERT INTO edgis.zz_mv_cwot
(
	GLOBALID,
	CREATIONUSER,
	DATECREATED,
	CONVERSIONID,
	CONVERSIONWORKPACKAGE,
	SERVICEPOINTID,
	CGC12,
	DATEFIXED,
	RESOLVED,
	SERVICEPOINTGUID
)
SELECT 
	GIS_I.GDB_GUID as GLOBALID,
	user as CREATIONUSER,
	sysdate as DATECREATED,
	new_cwots.CONVERSIONID,
	new_cwots.CONVERSIONWORKPACKAGE,
	new_cwots.SERVICEPOINTID,
	new_cwots.CGC12,
	NULL as DATEFIXED,
	'N'  as RESOLVED,
	new_cwots.GLOBALID
from
(SELECT 
	CONVERSIONID,
	CONVERSIONWORKPACKAGE,
	SERVICEPOINTID,
	CGC12,
	GLOBALID
FROM edgis.zz_mv_servicepoint srvpt WHERE srvpt.OBJECTID IN 
(
	SELECT OBJECTID FROM edgis.zz_mv_servicepoint 
	MINUS
	SELECT OBJECTID FROM edgis.zz_mv_servicepoint where TRANSFORMERGUID in ( SELECT GLOBALID FROM edgis.zz_mv_transformer)
)
minus
select 
	cwt3.CONVERSIONID,
	cwt3.CONVERSIONWORKPACKAGE,
	cwt3.SERVICEPOINTID,
	cwt3.CGC12,
	cwt3.SERVICEPOINTGUID
from edgis.zz_mv_cwot cwt3 where cwt3.RESOLVED='N'
 )new_cwots;

COMMIT;

--Close version
EXEC sde.version_user_ddl.edit_version(:mv_version,2);

spool off;

exit;

