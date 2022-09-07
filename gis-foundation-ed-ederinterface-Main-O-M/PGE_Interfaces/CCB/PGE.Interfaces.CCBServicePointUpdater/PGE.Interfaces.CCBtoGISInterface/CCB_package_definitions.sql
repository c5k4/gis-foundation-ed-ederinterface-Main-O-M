drop procedure CCBTOGIS_INITIALIZE_FILTERS;
drop procedure CCBTOGIS_SP_ACTION_INSERTS;
drop procedure CCBTOGIS_SP_ACTION_REPLACE  ;
drop procedure CCBTOGIS_SP_ACTION_UPDATES ;
drop procedure CCBTOGIS_SP_ACTION_DELETES ;
drop procedure CCBTOGIS_SP_ACTION_NO_ACTION ;
drop procedure CCBTOGIS_GIS_INSERTS ;
drop procedure CCBTOGIS_GIS_UPDATES ;
drop procedure CCBTOGIS_GIS_REPLACEMENTS ;
drop procedure CCBTOGIS_GIS_DELETES ;
drop procedure CCBTOGIS_RETURN_GISVALUES ;
drop procedure CCGTOGIS_CLEANSTAGINGTABLE ;


create or replace PROCEDURE CCBTOGIS_INITIALIZE_FILTERS AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
   rowcnt :=0;
    dbms_output.put_line('Gathering Statistics on the Staging Table');
   dbms_stats.gather_table_stats('PGEDATA', 'PGE_CCBTOEDGIS_STG');
-- TRUNCATE THE ACTION TABLE WHICH IS GENERATED FROM THE CCandB STAGING TABLE DATA AND WILL DRIVE THE GIS UPDATE PROCESS
   dbms_output.put_line('Truncating the Action Table');
delete from PGEDATA.PGE_CCB_SP_ACTION;
-- GET A START COUNT FROM THE ACTION TABLE
   dbms_output.put_line('Getting the count from the Action Table');
select count(*) into rowcnt from PGEDATA.PGE_CCB_SP_ACTION;
   dbms_output.put_line('Count of rows returned :'||rowcnt);
--UPDATE THE STAGING TABLE STRUCTURE
dbms_output.put_line('Altering the Staging Table Structure');
-- update pgedata.pge_ccbtoedgis_STG set STREETNAME1=replace(STREETNAME1, Chr(39), '''''') where streetname1 is not null;
-- update pgedata.pge_ccbtoedgis_STG set STREETNAME2=replace(STREETNAME2, Chr(39), '''''') where STREETNAME2 is not null;
-- update pgedata.pge_ccbtoedgis_STG set STREETNUMBER=replace(STREETNUMBER,  Chr(39), '''''') where STREETNUMBER is not null;
update pgedata.pge_ccbtoedgis_STG set UNIQUESPID=NVL(UNIQUESPID,SERVICEPOINTID) where UNIQUESPID is null;
commit;
 dbms_output.put_line('Creating Indexes on the Action Table');
-- PROCESS THE STAGING TABLE FOR ERRORS
  -- NULL SERVICEPOINTID
 dbms_output.put_line('Writing Null Service Point IDs to the Error Table');
insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
	select SPIDNULLERR.DATECREATED, 'Null SPID (' || SPIDNULLERR.SERVICEPOINTID || ') - Meter ID = ' 
	|| SPIDNULLERR.METERNUMBER || ' - Unique SPID = ' || SPIDNULLERR.UNIQUESPID , SYSDATE, 'SERVICEPOINTID' 
	from
	(
		select UNIQUESPID, SERVICEPOINTID, METERNUMBER, DATECREATED 
		from  PGEDATA.PGE_CCBTOEDGIS_STG
		where SERVICEPOINTID is null
	)SPIDNULLERR;
commit;		
  -- DELETE FROM STAGING TABLE WHERE SERVICE POINT IS NULL
 dbms_output.put_line('Removing Null Service Point IDs from the Staging Table');
delete from PGEDATA.PGE_CCBTOEDGIS_STG STAGE where STAGE.SERVICEPOINTID is null; 
commit;
  -- DUPLICATE SERVICEPOINT (SAME MX DATA AND SAME PROCESSING TYPE)
 dbms_output.put_line('Writing Duplicate Service Point IDs to the Error Table'); 
insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
	select SPIDERR.DATECREATED, 'Duplicate SPID (' || SPIDERR.SERVICEPOINTID || ') - Meter ID = ' 
	|| SPIDERR.METERNUMBER || ' - Unique SPID = ' || SPIDERR.UNIQUESPID ||' count of :'||cnt_spid , SYSDATE, 'SERVICEPOINTID' 
	from
	(
		select UNIQUESPID, SERVICEPOINTID, METERNUMBER, DATECREATED, count(SERVICEPOINTID) cnt_spid
			from  PGEDATA.PGE_CCBTOEDGIS_STG where (SERVICEPOINTID,DATECREATED) in 
			(select /*+ SPID_SPID_DATE_CR_IDX */ servicepointid,DATECREATED from PGEDATA.PGE_CCBTOEDGIS_STG 
				group by servicepointid,DATECREATED having count(*) >1)
		group by UNIQUESPID, SERVICEPOINTID, METERNUMBER, DATECREATED
	)SPIDERR;
commit;
 dbms_output.put_line('Removing Duplicate Service Point IDs from the Staging Table');
delete from PGEDATA.PGE_CCBTOEDGIS_STG STAGE where (SERVICEPOINTID,DATECREATED) in 
	(select servicepointid,DATECREATED from PGEDATA.PGE_CCBTOEDGIS_STG 
		group by servicepointid,DATECREATED having count(*) >1);
commit;		
 dbms_output.put_line('Writing Duplicate Unique Service Point IDs to the Error Table');
insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
	select USPIDERR.DATECREATED, 'Duplicate Unique SPID (' || UNIQUESPID || ') - Meter ID = ' 
	|| USPIDERR.METERNUMBER || ' SPID = ' || USPIDERR.SERVICEPOINTID, SYSDATE, 'UNIQUESPID' 
	from
	(
		select UNIQUESPID, SERVICEPOINTID, METERNUMBER, DATECREATED, ACTION, count(UNIQUESPID) 
		from  PGEDATA.PGE_CCBTOEDGIS_STG where (UNIQUESPID,DATECREATED, action) in 
		(select UNIQUESPID,DATECREATED, action from PGEDATA.PGE_CCBTOEDGIS_STG 
		group by UNIQUESPID,DATECREATED,action having count(*) >1)
	group by UNIQUESPID, SERVICEPOINTID, METERNUMBER, DATECREATED, ACTION
	) USPIDERR;
commit;
  -- DELETE FROM THE STAGING TABLE WITH DUPLICATE UNIQUE SERVICEPOINTID
 dbms_output.put_line('Removing Duplicate Unique Service Point IDs with the same date and edit operation from the Staging Table');
delete from PGEDATA.PGE_CCBTOEDGIS_STG STAGE2 where (UNIQUESPID,DATECREATED, action) in 
	(select UNIQUESPID,DATECREATED, action from PGEDATA.PGE_CCBTOEDGIS_STG group by UNIQUESPID,DATECREATED,action 
		having count(*) >1);
commit;
  -- NULL CGC12 (XFR RELATE)
 dbms_output.put_line('Writing Null CGC12 to the Error Table');
insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
	select XFRER.DATECREATED, 'Null CGC12 Value (SPID = ' || XFRER.SERVICEPOINTID || ')' , SYSDATE, 'CGC12' 
	from
	(
		select SERVICEPOINTID, DATECREATED 
		from  PGEDATA.PGE_CCBTOEDGIS_STG
		where CGC12 is null
	) XFRER;
commit;
  -- DELETE FROM STAGING TABLE WITH NULL CGC12
 dbms_output.put_line('Removing Null CGC12 from the Staging Table');
delete from PGEDATA.PGE_CCBTOEDGIS_STG STAGE3 where CGC12 is null; 
commit;			
-- PROCESS THE RECORDS FROM THE STAGING TABLE WHICH REQUIRE PROCESSING
--
update PGEDATA.PGE_CCBTOEDGIS_STG set CUSTOMERTYPE= DECODE(
		NVL(RATESCHEDULE,'ABS-TX3'), 
		'ABS-TX4','IND',
		'ABS-TX2','DOM',
		'ABS-TX5','AGR',
		'ABS-TX3','OTH', 
		'OTH' 
	  ) where customertype is null;
commit;
merge into pgedata.pge_ccbtoedgis_stg tab1 using
(
  select sp.objectid,sp.servicepointid from (
       select min(objectid) objectid,
	          servicepointid 
        from edgis.zz_mv_servicepoint 
		group by servicepointid
		) sp
) datasource
on ( datasource.servicepointid=tab1.servicepointid )
when matched then update set tab1.objectid=datasource.objectid ;
commit;
merge into (select * from pgedata.pge_ccbtoedgis_stg where objectid is null) tab1 using
(
  select sp.objectid,sp.UNIQUESPID from (
       select min(objectid) objectid,
	          UNIQUESPID 
        from edgis.zz_mv_servicepoint 
		group by UNIQUESPID
		) sp
) datasource
  on ( datasource.UNIQUESPID=tab1.UNIQUESPID )
  when matched then update set tab1.objectid=datasource.objectid ;
commit;
exception when others then dbms_output.put_line('Found Oracle error: '||SQLERRM);	  
END CCBTOGIS_INITIALIZE_FILTERS;
/

create or replace PROCEDURE CCBTOGIS_SP_ACTION_INSERTS AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  dbms_output.put_line( 'Populating the insert values into the Action Table');
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED) select 'GISI', TABLEI.SERVICEPOINTID, TABLEI.DATECREATED
from 
(
	select STG.SERVICEPOINTID, STG.DATECREATED from PGEDATA.PGE_CCBTOEDGIS_STG STG 
	where (NVL(STG.servicepointid,' '),TO_CHAR(stg.DATECREATED,'YYYYMMDD')) in (
		select NVL(STAGETABLE.servicepointid,' ') SERVICEPOINTID,TO_CHAR(STAGETABLE.DATECREATED,'YYYYMMDD') DATECREATED 
		    from PGEDATA.PGE_CCBTOEDGIS_STG STAGETABLE 
			where STAGETABLE.SERVICEPOINTID is not NUll and TO_CHAR(STAGETABLE.DATECREATED,'YYYYMMDD') = 
			(select max(TO_CHAR(stg2.DATECREATED,'YYYYMMDD')) DATECREATED 
			from PGEDATA.PGE_CCBTOEDGIS_STG stg2
			where NVL(stg2.servicepointid,' ')  = NVL(STAGETABLE.servicepointid,' ')
			)
			and NVL(STAGETABLE.UNIQUESPID,' ') not in (
			                               select distinct NVL(UNIQUESPID,' ') UNIQUESPID 
			                               from EDGIS.ZZ_MV_SERVICEPOINT 
		                                   ) 
	    )
	and  NVL(STG.servicepointid,' ') not in (select distinct NVL(MVVIEW.SERVICEPOINTID,' ') from EDGIS.ZZ_MV_SERVICEPOINT MVVIEW )
) TABLEI
commit;
END CCBTOGIS_SP_ACTION_INSERTS;
/

create or replace PROCEDURE CCBTOGIS_SP_ACTION_REPLACE AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  dbms_output.put_line('Populating the replace values into the Action Table');
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED) select 'GISR', TABLER.SERVICEPOINTID, TABLER.DATECREATED
from 
(
	select STG2.SERVICEPOINTID, STG2.DATECREATED from PGEDATA.PGE_CCBTOEDGIS_STG STG2 
		where (NVL(STG2.SERVICEPOINTID,' ')) in
	(
		select NVL(STAGETABLE2.SERVICEPOINTID,' ') from PGEDATA.PGE_CCBTOEDGIS_STG STAGETABLE2 
			where STAGETABLE2.SERVICEPOINTID is not NUll and TO_CHAR(STAGETABLE2.DATECREATED,'YYYYMMDD') = 
			(select max(TO_CHAR(DATECREATED,'YYYYMMDD')) from PGEDATA.PGE_CCBTOEDGIS_STG 
			where NVL(SERVICEPOINTID,' ') = NVL(STAGETABLE2.SERVICEPOINTID,' ')
			and NVL(STAGETABLE2.UNIQUESPID,' ') in (select distinct NVL(UNIQUESPID,' ') 
													from EDGIS.ZZ_MV_SERVICEPOINT 
													))
	)
	and NVL(STG2.servicepointid,' ') not in (select distinct NVL(MVVIEW.SERVICEPOINTID,' ') from EDGIS.ZZ_MV_SERVICEPOINT MVVIEW)
) TABLER;
commit;
END CCBTOGIS_SP_ACTION_REPLACE;
/

create or replace PROCEDURE CCBTOGIS_SP_ACTION_UPDATES AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Populating the update values into the Action Table');
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION,SERVICEPOINTID, DATEINSERTED) 
SELECT 'GISU', sp42.SERVICEPOINTID, sp42.DATECREATED
from PGEDATA.PGE_CCBTOEDGIS_STG sp42 where sp42.DATECREATED = 
			(
			select max(DATECREATED) from PGEDATA.PGE_CCBTOEDGIS_STG
			where SERVICEPOINTID = sp42.SERVICEPOINTID
			) and sp42.servicepointid in (select UNIQUE_LIST.servicepointid from 
			( 
				select distinct total.SERVICEPOINTID from 
				(
					select 
						--SP.METERID,
						SP.SERVICEPOINTID, 
						--SP.ACCOUNTID, 
						SP.ACCOUNTNUM, 
						SP.STREETNUMBER,
						SP.STREETNAME1,
						SP.STREETNAME2, 
						-- SP.MAILCITY, 
						-- SP.MAILSTATE, 
						-- SP.MAILZIPCODE,
						SP.CITY,
						SP.ZIP,
						-- SP.STATE,
						SP.UNIQUESPID, 
						--SP.TRANSFORMERGUID, 
						SP.SERVICEAGREEMENTID, 
						SP.INSERVICEDATE,
						SP.NAICS, 
						SP.BILLINGCYCLE, 
						SP.METERROUTE, 
						SP.REVENUEACCOUNTCODE, 
						SP.RATESCHEDULE, 
						--SP.PREMISETYPE,
						SP.TOWNSHIPTERRITORYCODE, 
						SP.NETENERGYMETERING,
						SP.METERNUMBER,
						SP.CUSTOMERTYPE,
						SP.CGC12,
						--SP.DIVISION,
						--SP.DISTRICT,
						SP.ESSENTIALCUSTOMERIDC,
						--SP.LOADSOURCECONVID,
						SP.LOCALOFFICEID,
						1 as src1, 
						to_number(null) as src2
						from 
						(
							select * from EDGIS.ZZ_MV_SERVICEPOINT where SERVICEPOINTID in 
							(
								select STAGE.SERVICEPOINTID from PGEDATA.PGE_CCBTOEDGIS_STG STAGE 
								where STAGE.SERVICEPOINTID is not null and STAGE.DATECREATED = 
									(select max(DATECREATED) from PGEDATA.PGE_CCBTOEDGIS_STG where 
										SERVICEPOINTID = STAGE.SERVICEPOINTID)
							minus  
								select SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION
							) 
						) SP
						union all
							select 
								--STG.METERID,
								STG.SERVICEPOINTID, 
								--STG.ACCOUNTID, 
								STG.ACCOUNTNUM, 
								STG.STREETNUMBER,
								STG.STREETNAME1,
								STG.STREETNAME2, 
								-- STG.MAILCITY, 
								-- STG.MAILSTATE, 
								-- STG.MAILZIPCODE,
								STG.CITY,
								STG.ZIP,
								-- STG.STATE,
								STG.UNIQUESPID, 
								--STG.TRANSFORMERGUID, 
								STG.SERVICEAGREEMENTID, 
								STG.INSERVICEDATE,
								STG.NAICS, 
								STG.BILLINGCYCLE, 
								STG.METERROUTE, 
								STG.REVENUEACCOUNTCODE, 
								STG.RATESCHEDULE, 
								--STG.PREMISETYPE,
								STG.TOWNSHIPTERRITORYCODE, 
								STG.NETENERGYMETERING,
								STG.METERNUMBER,
								STG.CUSTOMERTYPE,
								STG.CGC12,
								--STG.DIVISION,
								--STG.DISTRICT,
								STG.ESSENTIALCUSTOMERIDC,
								--STG.LOADSOURCECONVID,
								STG.LOCALOFFICEID,
								to_number(null) as  src1, 
								2 as src2 
								from 
								(
									select * from PGEDATA.PGE_CCBTOEDGIS_STG where SERVICEPOINTID in 
									(
										select MVV.SERVICEPOINTID from EDGIS.ZZ_MV_SERVICEPOINT MVV where MVV.SERVICEPOINTID is not null 
									minus 
										select SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION
									)
								) STG
					) TOTAL
					group by 
						--METERID,
						SERVICEPOINTID, 
						--ACCOUNTID, 
						ACCOUNTNUM, 
						STREETNUMBER,
						STREETNAME1,
						STREETNAME2, 
						-- MAILCITY, 
						-- MAILSTATE, 
						-- MAILZIPCODE,
						CITY,
						ZIP,
						-- STATE,
						UNIQUESPID, 
						--TRANSFORMERGUID,
						SERVICEAGREEMENTID,  
						INSERVICEDATE,
						NAICS, 
						BILLINGCYCLE, 
						METERROUTE, 
						REVENUEACCOUNTCODE, 
						RATESCHEDULE, 
						--PREMISETYPE,  
						TOWNSHIPTERRITORYCODE, 
						NETENERGYMETERING,
						METERNUMBER,
						CUSTOMERTYPE,
						CGC12,
						--DIVISION,
						--DISTRICT,
						ESSENTIALCUSTOMERIDC,
						--LOADSOURCECONVID,
						LOCALOFFICEID
						having count(src1) <> count(src2)
					) 
						UNIQUE_LIST
					);
					commit; 
END CCBTOGIS_SP_ACTION_UPDATES;
/

create or replace PROCEDURE CCBTOGIS_SP_ACTION_DELETES AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Populating the delete values into the Action Table');
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION,SERVICEPOINTID, DATEINSERTED) 
	select 'GISD', sp1.SERVICEPOINTID, max(sp1.DATECREATED) from PGEDATA.PGE_CCBTOEDGIS_STG sp1
	where sp1.servicepointid in 
	(
		select distinct STG.SERVICEPOINTID 
		from PGEDATA.PGE_CCBTOEDGIS_STG STG 
		where STG.ACTION = 'D' 
		and STG.SERVICEPOINTID is not null
	)
	group by sp1.servicepointid;
commit;  
END CCBTOGIS_SP_ACTION_DELETES;
/

create or replace PROCEDURE CCBTOGIS_SP_ACTION_NO_ACTION AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Populating the no action (NONE) into the Action Table');
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED) select 'NONE', TABLEN.SERVICEPOINTID, TABLEN.DATECREATED
from 
(
	select distinct STG.SERVICEPOINTID, STG.DATECREATED from PGEDATA.PGE_CCBTOEDGIS_STG STG 
		where STG.SERVICEPOINTID in(
			select distinct STAGETABLE.SERVICEPOINTID from PGEDATA.PGE_CCBTOEDGIS_STG STAGETABLE 
				where STAGETABLE.SERVICEPOINTID is not NUll and STAGETABLE.DATECREATED = 
					(select max(DATECREATED) from PGEDATA.PGE_CCBTOEDGIS_STG where SERVICEPOINTID = STAGETABLE.SERVICEPOINTID)
					and STAGETABLE.SERVICEPOINTID not in 
					(select distinct SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION)) 
) TABLEN;
commit;
END CCBTOGIS_SP_ACTION_NO_ACTION;
/

create or replace PROCEDURE CCBTOGIS_GIS_INSERTS(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing inserts into Servicepoint GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name);
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1);
insert into EDGIS.ZZ_MV_SERVICEPOINT(
	  GLOBALID,
	  CREATIONUSER,
	  DATECREATED,
	  DATEMODIFIED,
	  LASTUSER,
	  CONVERSIONID, --
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
	  NOTRANSFORMERIDC, --
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
	  SPECIALCUSTOMERTYPE, --
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
	  LOCALOFFICEID, --
	  CITY,
	  DCSERVICELOCATIONGUID, --
	  DCRECTIFIERGUID, --
	  SDE_STATE_ID --
	  --TRANSFORMERGUID
) 
select
	  EDGIS.GDB_GUID as GLOBALID,
      'GIS_I' as CREATIONUSER,
      sysdate as DATECREATED,
      sysdate as DATEMODIFIED,
      'GIS_I' as LASTUSER,
	  '' as CONVERSIONID, --
      '1' as CONVERSIONWORKPACKAGE,
      '1' as SUBTYPECD,
      '' as METERID,
      '5' as STATUS,
      STAGE.SERVICEPOINTID,
      STAGE.UNIQUESPID,
      STAGE.INSERVICEDATE,
      STAGE.STREETNAME1,
      STAGE.STREETNAME2,
      'CA' as STATE,
      '' as COUNTY,
      STAGE.ZIP,
      '' as GPSLATITUDE,
      '' as GPSLONGITUDE,
      '' as GPSSOURCE,
      '' as SOURCEACCURACY,
      '' as ELEVATION,
      STAGE.METERNUMBER,
      STAGE.BILLINGCYCLE,
      STAGE.METERROUTE,
      STAGE.PREMISETYPE,
      STAGE.PREMISEID as PREMISEID,
      STAGE.TOWNSHIPTERRITORYCODE,
      '' as DISTRICT,
      '' as DIVISION,
      STAGE.NETENERGYMETERING,
      '' as APNNUM,
	  '' as NOTRANSFORMERIDC, --
      STAGE.ESSENTIALCUSTOMERIDC,
      STAGE.SENSITIVECUSTOMERIDC as SENSITIVECUSTOMERIDC,
      STAGE.LIFESUPPORTIDC as LIFESUPPORTIDC,
	  STAGE.RATESCHEDULE,
      STAGE.SERVICEAGREEMENTID,
      STAGE.ACCOUNTNUM,
      '' as DATASOURCE,
      '' as AREACODE,
      '' as PHONENUM,
      '' as ACCOUNTID,
      '' as CUSTOMERCLASS,
	  STAGE.CUSTOMERTYPE,
	  '' as SPECIALCUSTOMERTYPE, --
      STAGE.REVENUEACCOUNTCODE,
      '' as MAILNAME1,
      '' as MAILNAME2,
      STAGE.MAILSTREETNUM as MAILSTREETNUM,
      STAGE.MAILSTREETNAME1 as MAILSTREETNAME1,
      STAGE.MAILSTREETNAME2 as MAILSTREETNAME2,
      STAGE.MAILCITY,
      STAGE.MAILSTATE,
      STAGE.MAILZIPCODE,
      STAGE.NAICS,
	  '' as LOADSOURCEGUID, --
	  '' as LOADSOURCECONVID, --
	  '' as SERVICELOCATIONGUID, --
	  '' as SERVICELOCATIONCONVID, --
      STAGE.CGC12,
	  '' as PRIMARYMETERGUID, --
      '' as GEMSOTHERMAPNUM,
	  '' as SECONDARYGENERATIONGUID, --
      '' as PRIMARYGENERATIONGUID, --
      STAGE.STREETNUMBER,
	  CGC_GUID.globalid as TRANSFORMERGUID, --
      STAGE.LOCALOFFICEID as LOCALOFFICEID, --
      STAGE.CITY,
	  '' as DCSERVICELOCATIONGUID, --
      '' as DCRECTIFIERGUID, --
      '' as SDE_STATE_ID --
      -- '' as TRANSFORMERGUID
	  from PGEDATA.PGE_CCBTOEDGIS_STG STAGE
	  left outer join (select min(all_sources.globalid) globalid, all_sources.CGC12 from ( 
	       select globalid,NVL(CGC12,000000000000) CGC12 from EDGIS.ZZ_MV_TRANSFORMER 
		   union all 
		   select globalid,NVL(CGC12,000000000000) CGC12 from EDGIS.ZZ_MV_PRIMARYMETER 
		      where NVL(CGC12,000000000000) not in (select NVL(CGC12,000000000000) CGC12 from EDGIS.ZZ_MV_TRANSFORMER)  
	  ) all_sources   group by all_sources.CGC12) CGC_GUID
	  on NVL(STAGE.CGC12,000000000000)=NVL(CGC_GUID.CGC12,000000000000)
	  where STAGE.SERVICEPOINTID in ( select ACTION.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION ACTION where ACTION.ACTION = 'GISI' ) ;
commit;
dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2);
END CCBTOGIS_GIS_INSERTS;
/

create or replace PROCEDURE CCBTOGIS_GIS_UPDATES(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing updates to the GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name);
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1);
    DECLARE
CURSOR insert_rows_list is select
      sysdate as DATEMODIFIED,
      'GIS_I' as LASTUSER,
      --USTAGE.METERID,
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
      --USTAGE.DISTRICT,
      --USTAGE.DIVISION,
      USTAGE.NETENERGYMETERING,
      USTAGE.ESSENTIALCUSTOMERIDC,
      USTAGE.RATESCHEDULE,
      USTAGE.SERVICEAGREEMENTID,
      USTAGE.ACCOUNTNUM,
      --USTAGE.ACCOUNTID,
	  USTAGE.CUSTOMERTYPE,
      USTAGE.REVENUEACCOUNTCODE,
      -- USTAGE.MAILCITY,
      -- USTAGE.MAILSTATE,
      -- USTAGE.MAILZIPCODE,
      USTAGE.NAICS,
      USTAGE.CGC12,
      USTAGE.STREETNUMBER,
      USTAGE.CITY,
      --USTAGE.LOADSOURCECONVID,
      USTAGE.LOCALOFFICEID,
      --USTAGE.TRANSFORMERGUID,
      USTAGE.OBJECTID,
	  USTAGE.SERVICEPOINTID,
	  -- USTAGE.AREACODE,
	  -- USTAGE.LIFESUPPORTIDC,
	  -- USTAGE.MAILNAME1,
	  -- USTAGE.MAILNAME2,
	  -- USTAGE.PHONENUM,
	  USTAGE.PREMISEID,
	  -- USTAGE.SENSITIVECUSTOMERIDC,
	  USTAGE.STATE
      from PGEDATA.PGE_CCBTOEDGIS_STG USTAGE 
      where 
	  USTAGE.OBJECTID  is not null
      and
	  USTAGE.SERVICEPOINTID IN (SELECT act.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION act where act.ACTION='GISU'); 
	  --and 		USTAGE.OBJECTID not in (select sp_id from temp_status_debug )  ;
		sqlstm   VARCHAR2(10000);
		tot_cnt       NUMBER;
		custType   VARCHAR2(3);
BEGIN
  tot_cnt := 0;
for USTAGE in insert_rows_list loop  
    tot_cnt := tot_cnt + 1 ;
    sqlstm := 'UPDATE edgis.zz_mv_servicepoint servpt SET DATEMODIFIED='''||USTAGE.DATEMODIFIED||''' ,
	LASTUSER='''||USTAGE.LASTUSER||''' ,UNIQUESPID='''||USTAGE.UNIQUESPID||''' 
	,INSERVICEDATE='''||USTAGE.INSERVICEDATE||''' ,STREETNAME1='''||replace(USTAGE.STREETNAME1, Chr(39), '''''')||
	''' ,STREETNAME2='''||replace(USTAGE.STREETNAME2, Chr(39), '''''')||''',ZIP='''||USTAGE.ZIP||''',METERNUMBER='''
	||USTAGE.METERNUMBER||''' ,BILLINGCYCLE='''||USTAGE.BILLINGCYCLE||''' ,METERROUTE='''||USTAGE.METERROUTE||''' ,
	PREMISETYPE='''||USTAGE.PREMISETYPE||''' ,TOWNSHIPTERRITORYCODE='''||USTAGE.TOWNSHIPTERRITORYCODE||''' ,NETENERGYMETERING='''||USTAGE.NETENERGYMETERING||''',RATESCHEDULE='''
	||USTAGE.RATESCHEDULE||''',SERVICEAGREEMENTID='''||USTAGE.SERVICEAGREEMENTID||''',ACCOUNTNUM='''||USTAGE.ACCOUNTNUM||''',
	CUSTOMERTYPE=''' || USTAGE.CUSTOMERTYPE || ''', REVENUEACCOUNTCODE='''|| USTAGE.REVENUEACCOUNTCODE||''',
	NAICS='''||USTAGE.NAICS||''',CGC12='''||USTAGE.CGC12||''',STREETNUMBER='''||
	replace(USTAGE.STREETNUMBER, Chr(39), '''''')||''',CITY='''||USTAGE.CITY||''',LOCALOFFICEID='''||USTAGE.LOCALOFFICEID||''',  
	  PREMISEID='''||USTAGE.PREMISEID||''',STATE= '''||USTAGE.STATE||''',
	ESSENTIALCUSTOMERIDC='''||USTAGE.ESSENTIALCUSTOMERIDC||''' where servpt.OBJECTID='||USTAGE.OBJECTID||'  ' ;
   -- dbms_output.put_line('****');
   -- dbms_output.put_line(sqlstm);
   execute immediate sqlstm ;
commit;
      exit when tot_cnt > 5600000 ;
end loop;
end;
commit;
dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2);	
END CCBTOGIS_GIS_UPDATES;
/

create or replace PROCEDURE CCBTOGIS_GIS_REPLACEMENTS(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing updates to the GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name);
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1);
    DECLARE
CURSOR insert_rows_list is select
      sysdate as DATEMODIFIED,
      'GIS_I' as LASTUSER,
      --USTAGE.METERID,
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
      --USTAGE.DISTRICT,
      --USTAGE.DIVISION,
      USTAGE.NETENERGYMETERING,
      USTAGE.ESSENTIALCUSTOMERIDC,
      USTAGE.RATESCHEDULE,
      USTAGE.SERVICEAGREEMENTID,
      USTAGE.ACCOUNTNUM,
      --USTAGE.ACCOUNTID,
	  USTAGE.CUSTOMERTYPE,
      USTAGE.REVENUEACCOUNTCODE,
      USTAGE.MAILCITY,
      USTAGE.MAILSTATE,
      USTAGE.MAILZIPCODE,
      USTAGE.NAICS,
      USTAGE.CGC12,
      USTAGE.STREETNUMBER,
      USTAGE.CITY,
      --USTAGE.LOADSOURCECONVID,
      USTAGE.LOCALOFFICEID,
      --USTAGE.TRANSFORMERGUID,
      USTAGE.OBJECTID,
	  USTAGE.SERVICEPOINTID,
      -- USTAGE.AREACODE,
	  -- USTAGE.LIFESUPPORTIDC,
	  -- USTAGE.MAILNAME1,
	  -- USTAGE.MAILNAME2,
	  -- USTAGE.PHONENUM,
	  USTAGE.PREMISEID,
	  -- USTAGE.SENSITIVECUSTOMERIDC,
	  USTAGE.STATE
      from PGEDATA.PGE_CCBTOEDGIS_STG USTAGE 
      where 
	  USTAGE.OBJECTID  is not null
      and
	  USTAGE.SERVICEPOINTID IN (SELECT act.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION act where act.ACTION='GISR'); 
	  --and 		USTAGE.OBJECTID not in (select sp_id from temp_status_debug )  ;
		sqlstm   VARCHAR2(10000);
		tot_cnt       NUMBER;
		custType   VARCHAR2(3);
BEGIN
  tot_cnt := 0;
for USTAGE in insert_rows_list loop  
    tot_cnt := tot_cnt + 1 ;
    sqlstm := 'UPDATE edgis.zz_mv_servicepoint servpt SET DATEMODIFIED='''||USTAGE.DATEMODIFIED||''' ,
	LASTUSER='''||USTAGE.LASTUSER||''' ,SERVICEPOINTID='''||USTAGE.SERVICEPOINTID||''' 
	,INSERVICEDATE='''||USTAGE.INSERVICEDATE||''' ,STREETNAME1='''||replace(USTAGE.STREETNAME1, Chr(39), '''''')||
	''' ,STREETNAME2='''||replace(USTAGE.STREETNAME2, Chr(39), '''''')||''',ZIP='''||USTAGE.ZIP||''',METERNUMBER='''
	||USTAGE.METERNUMBER||''' ,BILLINGCYCLE='''||USTAGE.BILLINGCYCLE||''' ,METERROUTE='''||USTAGE.METERROUTE||''' ,
	PREMISETYPE='''||USTAGE.PREMISETYPE||''' ,TOWNSHIPTERRITORYCODE='''||USTAGE.TOWNSHIPTERRITORYCODE||''' ,NETENERGYMETERING='''||USTAGE.NETENERGYMETERING||''',RATESCHEDULE='''
	||USTAGE.RATESCHEDULE||''',SERVICEAGREEMENTID='''||USTAGE.SERVICEAGREEMENTID||''',ACCOUNTNUM='''||USTAGE.ACCOUNTNUM||''',
	CUSTOMERTYPE=''' || USTAGE.CUSTOMERTYPE || ''', REVENUEACCOUNTCODE='''|| USTAGE.REVENUEACCOUNTCODE||''',
	MAILCITY='''||USTAGE.MAILCITY||''',MAILSTATE='''||USTAGE.MAILSTATE||''',MAILZIPCODE='''||
	USTAGE.MAILZIPCODE||''',NAICS='''||USTAGE.NAICS||''',CGC12='''||USTAGE.CGC12||''',STREETNUMBER='''||
	replace(USTAGE.STREETNUMBER, Chr(39), '''''')||''',CITY='''||USTAGE.CITY||''',LOCALOFFICEID='''||USTAGE.LOCALOFFICEID||''', 
	  PREMISEID='''||USTAGE.PREMISEID||''', STATE= '''||USTAGE.STATE||''',
	ESSENTIALCUSTOMERIDC='''||USTAGE.ESSENTIALCUSTOMERIDC||''' where servpt.objectid='||USTAGE.objectid||'  ' ;
   execute immediate sqlstm ;
   commit;
      exit when tot_cnt > 5600000 ;
end loop;
end;
commit;
dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2);	
END CCBTOGIS_GIS_REPLACEMENTS;
/

create or replace PROCEDURE CCBTOGIS_GIS_DELETES(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing deletes from the Servicepoint GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name);
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1);
delete from EDGIS.ZZ_MV_SERVICEPOINT MVVIEW where MVVIEW.SERVICEPOINTID in
	(select ACTION2.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION ACTION2 where ACTION2.ACTION = 'GISD');
commit;
dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2);
END CCBTOGIS_GIS_DELETES;
/

create or replace PROCEDURE CCBTOGIS_RETURN_GISVALUES AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('RETURN GIS Values');
 -- initialize process by clearing old table values
dbms_output.put_line('Clearing Before Table');
  delete from PGEDATA.TEMP_CCB_SP_OUT_B;
  commit;
dbms_output.put_line('Clearing After Table');  
  delete from PGEDATA.TEMP_CCB_SP_OUT_A;
  commit;
dbms_output.put_line('Populate GIS table Before');
sde.version_util.set_current_version('GIS_I.Change_Detection_Sync_Tlm');
Insert into PGEDATA.TEMP_CCB_SP_OUT_B (SERVICEPOINTID,UNIQUESPID,ROBC,SOURCESIDEDEVICEID,CGC12,CIRCUITID,CREATE_DTM,CREATE_USERID)
   select SP.SERVICEPOINTID, SP.UNIQUESPID, ROBC.ROBC, TR.SOURCESIDEDEVICEID, TR.CGC12, TR.CIRCUITID, sysdate, 'GIS_I_WRITE'
     from edgis.zz_mv_servicepoint sp left outer join edgis.zz_mv_transformer tr on sp.transformerguid=tr.globalid
     left outer join 
   ( select cs.circuitid,cs.globalid,rb.ESTABLISHEDROBC robc 
       from edgis.zz_mv_circuitsource cs left outer join edgis.zz_mv_robc rb on cs.globalid=rb.circuitsourceguid ) robc 
     on tr.circuitid=robc.circuitid;  
commit;	 
dbms_output.put_line('Populate GIS table After');	 
sde.version_util.set_current_version('SDE.DEFAULT');
Insert into PGEDATA.TEMP_CCB_SP_OUT_A (SERVICEPOINTID,UNIQUESPID,ROBC,SOURCESIDEDEVICEID,CGC12,CIRCUITID,CREATE_DTM,CREATE_USERID)
   select SP.SERVICEPOINTID, SP.UNIQUESPID, ROBC.ROBC, TR.SOURCESIDEDEVICEID, TR.CGC12, TR.CIRCUITID, sysdate, 'GIS_I_WRITE'
     from edgis.zz_mv_servicepoint sp left outer join edgis.zz_mv_transformer tr on sp.transformerguid=tr.globalid
     left outer join 
   ( select cs.circuitid,cs.globalid,rb.ESTABLISHEDROBC robc 
       from edgis.zz_mv_circuitsource cs left outer join edgis.zz_mv_robc rb on cs.globalid=rb.circuitsourceguid ) robc 
     on tr.circuitid=robc.circuitid;  
commit;	 
 -- Insert those rows with different values than CCB sent.
dbms_output.put_line('Populate GIS values for those sent not matching');	
insert into PGEDATA.PGE_EDGISTOCCB_STG	 (
SERVICEPOINTID,
UNIQUESPID,
ROBC,
SOURCESIDEDEVICEID,
CGC12,
CIRCUITID,
UPDATE_DTM,
UPDATE_USERID)
select 
   SP.SERVICEPOINTID,
   SP.UNIQUESPID,
   SP.ROBC,
   SP.SOURCESIDEDEVICEID,
   SP.CGC12,
   SP.CIRCUITID,
   SP.CREATE_DTM,
   SP.CREATE_USERID from (
   select * from pgedata.pge_ccbtoedgis_stg
) ccb
left outer join ( select * from
   PGEDATA.TEMP_CCB_SP_OUT_A SP
) SP
on ccb.servicepointid=sp.servicepointid
where 
   NVL(ccb.NEWROBC,'000') <> NVL(sp.ROBC,'000') and
   NVL(ccb.SOURCESIDEDEVICEID,'NONE') <> NVL(sp.SOURCESIDEDEVICEID,'NONE') and
   NVL(ccb.CGC12,9999) <> NVL(sp.CGC12,9999) and
   NVL(ccb.CIRCUITID,'000000000') <> NVL(sp.circuitid,'000000000');	 
commit;
 -- Insert those rows updated in GIS and not already sent.
dbms_output.put_line('Populate GIS values for those GIS changed');	 
insert into PGEDATA.PGE_EDGISTOCCB_STG	 (
SERVICEPOINTID,
UNIQUESPID,
ROBC,
SOURCESIDEDEVICEID,
CGC12,
CIRCUITID,
UPDATE_DTM,
UPDATE_USERID)
select 
VSP.SERVICEPOINTID,
VSP.UNIQUESPID,
VSP.ROBC,
VSP.SOURCESIDEDEVICEID,
VSP.CGC12,
VSP.CIRCUITID,
VSP.CREATE_DTM,
VSP.CREATE_USERID
from (   
   select 
     SP.SERVICEPOINTID,
     SP.UNIQUESPID,
     SP.ROBC,
     SP.SOURCESIDEDEVICEID,
     SP.CGC12,
     SP.CIRCUITID,
     SP.CREATE_DTM,
     SP.CREATE_USERID from (
   select * from PGEDATA.TEMP_CCB_SP_OUT_B 
  ) ccb
  left outer join ( select * from
   PGEDATA.TEMP_CCB_SP_OUT_A SP
  ) SP
  on ccb.servicepointid=sp.servicepointid
  where 
   NVL(ccb.ROBC,'000') <> NVL(sp.ROBC,'000') and
   NVL(ccb.SOURCESIDEDEVICEID,'NONE') <> NVL(sp.SOURCESIDEDEVICEID,'NONE') and
   NVL(ccb.CGC12,9999) <> NVL(sp.CGC12,9999) and
   NVL(ccb.CIRCUITID,'000000000') <> NVL(sp.circuitid,'000000000')
) VSP
where 
(VSP.servicepointid,VSP.CREATE_DTM) not in (
      select stg1.servicepointid,stg1.UPDATE_DTM 
	  from PGEDATA.PGE_EDGISTOCCB_STG stg1
     );
commit;
END CCBTOGIS_RETURN_GISVALUES;
/


create or replace PROCEDURE CCBTOGIS_CLEANSTAGINGTABLE AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN

END;
/

grant all on CCBTOGIS_INITIALIZE_FILTERS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_INSERTS  to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_REPLACE  to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_UPDATES  to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_NO_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on  CCBTOGIS_GIS_REPLACEMENTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_RETURN_GISVALUES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_CLEANSTAGINGTABLE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;

grant all on PGE_CCB_SP_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CCBTOEDGIS_STG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_EDGISTOCCB_STG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;

grant all on CD_GIS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on GIS_SAP to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on GENERATE_EXT_CCB_METER_LOAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on GENERATE_CD_TABLES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_INITIALIZE_FILTERS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_SP_ACTION_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_SP_ACTION_REPLACE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_SP_ACTION_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_SP_ACTION_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_SP_ACTION_NO_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_GIS_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_GIS_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_GIS_REPLACEMENTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_GIS_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCGTOGIS_RETURN_GISVALUES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on EDGISTOCCB_STG_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_INITIALIZE_FILTERS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_REPLACE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_NO_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_REPLACEMENTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_RETURN_GISVALUES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_CLEANSTAGINGTABLE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;	
grant all on LOG_PROCESS_STATUS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;	
grant all on PGEDATA_SM_CAPACITOR_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_CIRCUIT_BREAKER_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_INTERRUPTER_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_NETWORK_PROT_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_RECLOSER_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_REGULATOR_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_SECTIONALIZER_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_SWITCH_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_FC_LAYER_MAPPING to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_TABLE_LOOKUP to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CD_ERROR to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_CD_ERROR to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_EXECUTED to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_SCADA_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CD_LIST to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CD_MAP_SETTINGS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on ENOS_ARCHIVE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on ENOS_ERROR to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on ENOS_STAGE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on SAP_NOTIFICATIONHEADER to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on SAP_NOTIFICATIONHEADER_STG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGEDATA_SM_PRIMARY_GEN_EAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_EDGISTOCCB_STG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CCBTOEDGIS_STG_ERRORS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CCBTOEDGIS_STG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_EXT_CCB_METER_LOAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CCB_SP_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CD_TRANSFORMER_BANK to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CD_TRANSFORMER to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CD_METER to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CD_METER_A to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CD_METER_B to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CD_TRANSFORMER_A to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CD_TRANSFORMER_B to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CD_TRANSFORMER_BANK_A to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CD_TRANSFORMER_BANK_B to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PROCESS_LOG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CCB_SP_OUT_A to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on TEMP_CCB_SP_OUT_B to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CCB_SP_OUT to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PROCESS_LOG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;

