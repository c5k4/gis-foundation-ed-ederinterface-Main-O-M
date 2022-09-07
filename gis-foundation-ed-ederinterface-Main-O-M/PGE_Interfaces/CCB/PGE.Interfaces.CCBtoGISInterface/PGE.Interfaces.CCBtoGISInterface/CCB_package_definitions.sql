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
drop procedure CCBTOGIS_UPDATECWOT ;

GRANT all on edgis.ZZ_MV_PARTIALCURTAILPOINT to PGEDATA,GIS_I_WRITE;

create or replace PROCEDURE CCBTOGIS_INITIALIZE_FILTERS AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN 
  sde.version_util.set_current_version('SDE.DEFAULT') /* CCBTOGIS_INITIALIZE_FILTERS_V1_SETVER */;
   -- update STREETNAME2 field from SERVICEADDRESS2
   update  pgedata.pge_ccbtoedgis_STG set STREETNAME2 = substr(SERVICEADDRESS2, 0, 64) where STREETNAME2 is null and SERVICEADDRESS2 is not null;
   commit;
   rowcnt :=0;
    dbms_output.put_line('Gathering Statistics on the Staging Table');
   dbms_stats.gather_table_stats('PGEDATA', 'PGE_CCBTOEDGIS_STG') /* CCBTOGIS_INITIALIZE_FILTERS_V1_STATSCCBSTG */;
-- TRUNCATE THE ACTION TABLE WHICH IS GENERATED FROM THE CCandB STAGING TABLE DATA AND WILL DRIVE THE GIS UPDATE PROCESS
   dbms_output.put_line('Truncating the Action Table');
delete from PGEDATA.PGE_CCB_SP_ACTION /* CCBTOGIS_INITIALIZE_FILTERS_V1_DELA*/;
commit;
-- GET A START COUNT FROM THE ACTION TABLE
   dbms_output.put_line('Getting the count from the Action Table');
select count(*) into rowcnt from PGEDATA.PGE_CCB_SP_ACTION /* CCBTOGIS_INITIALIZE_FILTERS_V1_DELCHECK*/;
   dbms_output.put_line('Count of rows returned :'||rowcnt);
--UPDATE THE STAGING TABLE STRUCTURE
dbms_output.put_line('Altering the Staging Table Structure');
update pgedata.pge_ccbtoedgis_STG set UNIQUESPID=SERVICEPOINTID where UNIQUESPID is null /* CCBTOGIS_INITIALIZE_FILTERS_V1_DELCHECK*/;
update pgedata.pge_ccbtoedgis_STG set ESSENTIALCUSTOMERIDC='Y' where ESSENTIALCUSTOMERIDC='E' /* CCBTOGIS_INITIALIZE_FILTERS_V1_ECTOY*/;
commit;
 dbms_output.put_line('Creating Indexes on the Action Table');
-- PROCESS THE STAGING TABLE FOR ERRORS
  -- NULL SERVICEPOINTID
 dbms_output.put_line('Writing Null Service Point IDs to the Error Table');
insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
	select SPIDNULLERR.DATECREATED, 'Null SPID (' || SPIDNULLERR.SERVICEPOINTID || ') - Meter ID = ' 
	|| SPIDNULLERR.METERNUMBER || ' - Unique SPID = ' || SPIDNULLERR.UNIQUESPID , SYSDATE, 'SERVICEPOINTID' /* CCBTOGIS_INITIALIZE_FILTERS_V1_NULSPIDRECORD*/
	from
	(
		select UNIQUESPID, SERVICEPOINTID, METERNUMBER, DATECREATED 
		from  PGEDATA.PGE_CCBTOEDGIS_STG
		where SERVICEPOINTID is null
	)SPIDNULLERR;
commit;		
  -- DELETE FROM STAGING TABLE WHERE SERVICE POINT IS NULL
 dbms_output.put_line('Removing Null Service Point IDs from the Staging Table');
delete from PGEDATA.PGE_CCBTOEDGIS_STG STAGE where STAGE.SERVICEPOINTID is null /* CCBTOGIS_INITIALIZE_FILTERS_V1_NULSPIDDEL*/; 
commit;
  -- DUPLICATE SERVICEPOINT (SAME MX DATA AND SAME PROCESSING TYPE)
 dbms_output.put_line('Writing Duplicate Service Point IDs to the Error Table'); 
insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
	select SPIDERR.DATECREATED, 'Duplicate SPID (' || SPIDERR.SERVICEPOINTID || ') - Meter ID = ' 
	|| SPIDERR.METERNUMBER || ' - Unique SPID = ' || SPIDERR.UNIQUESPID ||' count of :'||cnt_spid , SYSDATE, 'SERVICEPOINTID' /* CCBTOGIS_INITIALIZE_FILTERS_V1_DUPSPIDRECORD*/
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
		group by servicepointid,DATECREATED having count(*) >1) /* CCBTOGIS_INITIALIZE_FILTERS_V1_DUPSPIDDEL*/ ;
commit;		
 dbms_output.put_line('Writing Duplicate Unique Service Point IDs to the Error Table');
insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
	select USPIDERR.DATECREATED, 'Duplicate Unique SPID (' || UNIQUESPID || ') - Meter ID = ' 
	|| USPIDERR.METERNUMBER || ' SPID = ' || USPIDERR.SERVICEPOINTID, SYSDATE, 'UNIQUESPID' /* CCBTOGIS_INITIALIZE_FILTERS_V1_DUPUNIQIDRECORD*/
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
		having count(*) >1) /* CCBTOGIS_INITIALIZE_FILTERS_V1_DUPUNIQIDDEL*/ ;
commit;
  -- NULL CGC12 (XFR RELATE)
-- dbms_output.put_line('Writing Null CGC12 to the Error Table');
--insert into PGEDATA.PGE_CCBTOEDGIS_STG_ERRORS (ERROR_ID, ERROR_DESCRIPTION, ERROR_TIMESTAMP, FIELD_IN_ERROR)
--	select XFRER.DATECREATED, 'Null CGC12 Value (SPID = ' || XFRER.SERVICEPOINTID || ')' , SYSDATE, 'CGC12' /* CCBTOGIS_INITIALIZE_FILTERS_V1_NULCGCRECORD*/
--	from
--	(
--		select SERVICEPOINTID, DATECREATED 
--		from  PGEDATA.PGE_CCBTOEDGIS_STG
--		where CGC12 is null
--	) XFRER;
--commit;
  -- DELETE FROM STAGING TABLE WITH NULL CGC12
-- dbms_output.put_line('Removing Null CGC12 from the Staging Table');
--delete from PGEDATA.PGE_CCBTOEDGIS_STG STAGE3 where CGC12 is null /* CCBTOGIS_INITIALIZE_FILTERS_V1_NULCGCDEL*/ ; 
--commit;
			
-- PROCESS THE RECORDS FROM THE STAGING TABLE WHICH REQUIRE PROCESSING
--update PGEDATA.PGE_CCBTOEDGIS_STG set CUSTOMERTYPE= DECODE(
--		NVL(RATESCHEDULE,'ABS-TX3'), 
--		'ABS-TX4','IND',
--		'ABS-TX2','DOM',
--		'ABS-TX5','AGR',
--		'ABS-TX3','OTH', 
--		'OTH' 
--	  ) where customertype is null /* CCBTOGIS_INITIALIZE_FILTERS_V1_PREPCUSTTYPE*/ ;

update PGEDATA.PGE_CCBTOEDGIS_STG set CUSTOMERTYPE =  
    nvl(
        decode(revenueaccountcode, 'ERES','DOM',
                              'EAG','AGR',
                              'EINTDPT','OTH',
                              'ELCI','IND',
                              'ELIGHT','OTH',
                              'EMCI','IND',
                              'EMRES','DOM',
                              'EOTHER','OTH',
                              'EPGEET','OTH',
                              'ESCI','COM',
                              'ESLSPA','OTH',
                              'ESLSRS','OTH',
                              'EWHLNG','OTH',
                              'GCOM','COM',
                              'GIND','IND', null
                              ), 
           decode(NVL(RATESCHEDULE,'ABS-TX3'), 
           'ABS-TX4','IND',
           'ABS-TX2','DOM',
           'ABS-TX5','AGR',
           'ABS-TX3','OTH', 'OTH' )
          ) where trim(customertype) is null /* CCBTOGIS_INITIALIZE_FILTERS_V1_PREPCUSTTYPE*/ ;


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
when matched then update set tab1.objectid=datasource.objectid /* CCBTOGIS_INITIALIZE_FILTERS_V1_PREPSPIDOBJECTID*/  ;
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
  when matched then update set tab1.objectid=datasource.objectid /* CCBTOGIS_INITIALIZE_FILTERS_V1_PREPUNIQUESPIDOBJECTID*/ ;
commit;
exception when others then dbms_output.put_line('Found Oracle error: '||SQLERRM);	  
END CCBTOGIS_INITIALIZE_FILTERS;
/

create or replace PROCEDURE CCBTOGIS_SP_ACTION_INSERTS AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  dbms_output.put_line( 'Populating the insert values into the Action Table');
sde.version_util.set_current_version('SDE.DEFAULT') /* CCBTOGIS_SP_ACTION_INSERTS_V1_SETVER */;
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED) 
select 'GISI', TABLEI.SERVICEPOINTID, TABLEI.DATECREATED /* CCBTOGIS_SP_ACTION_INSERTS_V1_GISI*/
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
) TABLEI;
commit;
END CCBTOGIS_SP_ACTION_INSERTS;
/

create or replace PROCEDURE CCBTOGIS_SP_ACTION_REPLACE AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  dbms_output.put_line('Populating the replace values into the Action Table');
  sde.version_util.set_current_version('SDE.DEFAULT') /* CCBTOGIS_SP_ACTION_REPLACE_V1_SETVER */ ;
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED) 
select 'GISR', TABLER.SERVICEPOINTID, TABLER.DATECREATED /* CCBTOGIS_SP_ACTION_REPLACE_V1_GISR*/
from 
(
	select STG2.SERVICEPOINTID, STG2.DATECREATED from PGEDATA.PGE_CCBTOEDGIS_STG STG2 
		where (NVL(STG2.SERVICEPOINTID,' ')) in
	(
		select NVL(STAGETABLE2.SERVICEPOINTID,' ') from PGEDATA.PGE_CCBTOEDGIS_STG STAGETABLE2 
			where STAGETABLE2.SERVICEPOINTID is not NUll and TO_CHAR(STAGETABLE2.DATECREATED,'YYYYMMDD') = 
			(select max(TO_CHAR(stg1.DATECREATED,'YYYYMMDD')) from PGEDATA.PGE_CCBTOEDGIS_STG stg1 
			where NVL(stg1.SERVICEPOINTID,' ') = NVL(STAGETABLE2.SERVICEPOINTID,' ')
			)
			and NVL(STAGETABLE2.UNIQUESPID,' ') in (select distinct NVL(UNIQUESPID,' ') UNIQUESPID 
													from EDGIS.ZZ_MV_SERVICEPOINT 
													)
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
dbms_output.put_line('Remove values from source SP Action Table');
delete from PGEDATA.TEMP_SP_ACTION_SOURCE /* CCBTOGIS_SP_ACTION_UPDATES_V1_DELTABLE*/;
dbms_output.put_line('Populating the source values into the Action Table for update');
insert into PGEDATA.TEMP_SP_ACTION_SOURCE(
SERVICEPOINTID, ACCOUNTNUM, STREETNUMBER,STREETNAME1,STREETNAME2,
CITY,ZIP,UNIQUESPID,SERVICEAGREEMENTID,INSERVICEDATE,NAICS,CGC12,BILLINGCYCLE,
METERROUTE,REVENUEACCOUNTCODE,RATESCHEDULE,TOWNSHIPTERRITORYCODE,NETENERGYMETERING,
METERNUMBER,CUSTOMERTYPE,ESSENTIALCUSTOMERIDC,LOCALOFFICEID,PREMISETYPE,PREMISEID)
 select mv2.SERVICEPOINTID, mv2.ACCOUNTNUM, mv2.STREETNUMBER, mv2.STREETNAME1, mv2.STREETNAME2, 
        mv2.CITY,mv2.ZIP,mv2.UNIQUESPID, mv2.SERVICEAGREEMENTID, mv2.INSERVICEDATE, mv2.NAICS, mv2.CGC12, mv2.BILLINGCYCLE, 
		mv2.METERROUTE, mv2.REVENUEACCOUNTCODE, mv2.RATESCHEDULE, mv2.TOWNSHIPTERRITORYCODE, mv2.NETENERGYMETERING,
		mv2.METERNUMBER,mv2.CUSTOMERTYPE,mv2.ESSENTIALCUSTOMERIDC, mv2.LOCALOFFICEID, mv2.PREMISETYPE, mv2.PREMISEID					 
 from EDGIS.ZZ_MV_SERVICEPOINT mv2 
 where mv2.SERVICEPOINTID is not null 
  and mv2.SERVICEPOINTID in 
     (
	  select STAGE.SERVICEPOINTID from PGEDATA.PGE_CCBTOEDGIS_STG STAGE 
	  where STAGE.SERVICEPOINTID is not null and STAGE.DATECREATED is not null
	and (STAGE.SERVICEPOINTID, TO_CHAR(STAGE.DATECREATED, 'YYYYMMDDHH24MISS')) in
	( select stg2.servicepointid, max(TO_CHAR(stg2.DATECREATED, 'YYYYMMDDHH24MISS')) 
        from PGEDATA.PGE_CCBTOEDGIS_STG stg2 group by stg2.servicepointid )
	) and mv2.SERVICEPOINTID not in ( select spa.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION spa where SERVICEPOINTID is not null ) 
  /* CCBTOGIS_SP_ACTION_UPDATES_V1_POPUSRC*/;
sde.version_util.set_current_version('SDE.DEFAULT') /* CCBTOGIS_SP_ACTION_UPDATES_V1_SETVER */ ;
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION,SERVICEPOINTID, DATEINSERTED) SELECT 
'GISU', sp42.SERVICEPOINTID, sp42.DATECREATED /* CCBTOGIS_SP_ACTION_UPDATES_V4_GISU*/
from PGEDATA.PGE_CCBTOEDGIS_STG sp42 where TO_CHAR(sp42.DATECREATED,'YYYYMMDDHH24MISS') = 
			(
			select max(TO_CHAR(stg0.DATECREATED, 'YYYYMMDDHH24MISS'))  from PGEDATA.PGE_CCBTOEDGIS_STG stg0
			where stg0.SERVICEPOINTID = sp42.SERVICEPOINTID
			) 
and sp42.servicepointid in 
  (  select UNIQUE_LIST.servicepointid from 
			  ( 
				select distinct total.SERVICEPOINTID from (
			select 
			SP.SERVICEPOINTID, SP.UNIQUESPID, SP.ACCOUNTNUM, 
			SP.STREETNUMBER,   SP.STREETNAME1, SP.STREETNAME2, 
			SP.CITY,           SP.ZIP,       SP.SERVICEAGREEMENTID, 
			SP.INSERVICEDATE,  SP.NAICS,     SP.CGC12,
			SP.BILLINGCYCLE,   SP.METERROUTE, 
			SP.REVENUEACCOUNTCODE, SP.RATESCHEDULE, 
			SP.TOWNSHIPTERRITORYCODE, SP.NETENERGYMETERING,
			SP.METERNUMBER,    SP.CUSTOMERTYPE, 
			SP.ESSENTIALCUSTOMERIDC,   SP.LOCALOFFICEID,
			SP.PREMISETYPE,    SP.PREMISEID,
			1 as src1, to_number(null) as src2
			from 
			(
				select mv2.SERVICEPOINTID, mv2.ACCOUNTNUM, mv2.STREETNUMBER, mv2.STREETNAME1, mv2.STREETNAME2, 
					 mv2.CITY,mv2.ZIP,mv2.UNIQUESPID, mv2.SERVICEAGREEMENTID, mv2.INSERVICEDATE, mv2.NAICS, mv2.CGC12, mv2.BILLINGCYCLE, 
					 mv2.METERROUTE, mv2.REVENUEACCOUNTCODE, mv2.RATESCHEDULE, mv2.TOWNSHIPTERRITORYCODE, mv2.NETENERGYMETERING,
					 mv2.METERNUMBER,mv2.CUSTOMERTYPE,mv2.ESSENTIALCUSTOMERIDC, mv2.LOCALOFFICEID,
                     mv2.PREMISETYPE, mv2.PREMISEID					 
			  from PGEDATA.TEMP_SP_ACTION_SOURCE mv2 
			) SP
			union all
			select 
			STG.SERVICEPOINTID, STG.UNIQUESPID, STG.ACCOUNTNUM, 
			STG.STREETNUMBER,   STG.STREETNAME1, STG.STREETNAME2, 
			STG.CITY,           STG.ZIP,        STG.SERVICEAGREEMENTID, 
			STG.INSERVICEDATE,  STG.NAICS,      STG.CGC12, 
			STG.BILLINGCYCLE,   STG.METERROUTE, 
			STG.REVENUEACCOUNTCODE,     STG.RATESCHEDULE, 
			STG.TOWNSHIPTERRITORYCODE,  STG.NETENERGYMETERING,
			STG.METERNUMBER,    STG.CUSTOMERTYPE, 
			STG.ESSENTIALCUSTOMERIDC, STG.LOCALOFFICEID,
			STG.PREMISETYPE,    STG.PREMISEID,
			to_number(null) as  src1, 2 as src2 
			from 
			(
				select sg.SERVICEPOINTID, sg.ACCOUNTNUM, sg.STREETNUMBER,sg.STREETNAME1, 
				sg.STREETNAME2, sg.CITY, sg.ZIP,sg.UNIQUESPID, sg.SERVICEAGREEMENTID,
				sg.INSERVICEDATE, sg.NAICS, sg.CGC12, sg.BILLINGCYCLE, sg.METERROUTE,
				sg.REVENUEACCOUNTCODE, sg.RATESCHEDULE, sg.TOWNSHIPTERRITORYCODE, 
				sg.NETENERGYMETERING, sg.METERNUMBER, sg.CUSTOMERTYPE,
				sg.ESSENTIALCUSTOMERIDC, sg.LOCALOFFICEID, sg.PREMISETYPE, sg.PREMISEID  
			from PGEDATA.PGE_CCBTOEDGIS_STG sg where NVL(sg.SERVICEPOINTID,' ') in 
				(
					select MVV.SERVICEPOINTID from PGEDATA.TEMP_SP_ACTION_SOURCE MVV where MVV.SERVICEPOINTID is not null 
					minus 
					select spa.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION spa where spa.SERVICEPOINTID is not null
				) and sg.SERVICEPOINTID is not null
			) STG  ) TOTAL
			group by 
			TOTAL.SERVICEPOINTID, TOTAL.UNIQUESPID,  TOTAL.ACCOUNTNUM, 
			TOTAL.STREETNUMBER,   TOTAL.STREETNAME1, TOTAL.STREETNAME2, 
			TOTAL.CITY,           TOTAL.ZIP,         TOTAL.SERVICEAGREEMENTID,  
			TOTAL.INSERVICEDATE,  TOTAL.NAICS,       TOTAL.CGC12,
			TOTAL.BILLINGCYCLE,   TOTAL.METERROUTE, 
			TOTAL.REVENUEACCOUNTCODE,    TOTAL.RATESCHEDULE, 
			TOTAL.TOWNSHIPTERRITORYCODE, TOTAL.NETENERGYMETERING,
			TOTAL.METERNUMBER,    TOTAL.CUSTOMERTYPE, 
			TOTAL.ESSENTIALCUSTOMERIDC, TOTAL.LOCALOFFICEID,
			TOTAL.PREMISETYPE,    TOTAL.PREMISEID
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
	select 'GISD', sp1.SERVICEPOINTID, max(sp1.DATECREATED) /* CCBTOGIS_SP_ACTION_DELETES_V2_GISD*/
	from PGEDATA.PGE_CCBTOEDGIS_STG sp1
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
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED) 
select 'NONE', TABLEN.SERVICEPOINTID, TABLEN.DATECREATED /* CCBTOGIS_SP_ACTION_NO_ACTION_V1_NONE*/
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
	       select globalid XMFR_GUID,NVL(CGC12,000000000000) CGC12,' ' PRI_GUID from EDGIS.ZZ_MV_TRANSFORMER where CGC12 is not NULL
		   union all 
		   select ' ' XFMR_GUID,NVL(CGC12,000000000000) CGC12,globalid PRI_GUID from EDGIS.ZZ_MV_PRIMARYMETER 
		      where NVL(CGC12,000000000000) not in (select NVL(CGC12,000000000000) CGC12 from EDGIS.ZZ_MV_TRANSFORMER) and CGC12 is not NULL
	  ) all_sources   group by all_sources.CGC12) CGC_GUID
	  on NVL(STAGE.CGC12,000000000000)=NVL(CGC_GUID.CGC12,000000000000)
	  where STAGE.SERVICEPOINTID in ( select ACTION.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION ACTION where ACTION.ACTION = 'GISI' ) ;
commit;

dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2) /* CCBTOGIS_GIS_INSERTS_V1_STOPEDIT*/ ;
END CCBTOGIS_GIS_INSERTS;
/

create or replace PROCEDURE CCBTOGIS_GIS_UPDATES(version_name IN VARCHAR2) AS
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
	  'CA' as STATE
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
	STATE= '''||USTAGE.STATE||'''
	where servpt.OBJECTID='||USTAGE.OBJECTID||' /* CCBTOGIS_GIS_UPDATES_V1_SPUPDATE */ ' ;
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
END CCBTOGIS_GIS_UPDATES;
/

create or replace PROCEDURE CCBTOGIS_GIS_REPLACEMENTS(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing updates to the GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name) /* CCBTOGIS_GIS_REPLACEMENTS_V1_SETVER */ ;
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1) /* CCBTOGIS_GIS_REPLACEMENTS_V1_STARTEDIT*/ ;
    DECLARE
CURSOR insert_rows_list is select
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
      -- USTAGE.CGC12,
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
	  'CA' as STATE
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
	LASTUSER='''||USTAGE.LASTUSER||''' ,
	SERVICEPOINTID='''||USTAGE.SERVICEPOINTID||''',
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
	STREETNUMBER='''||replace(USTAGE.STREETNUMBER, Chr(39), '''''')||''',
	CITY='''||USTAGE.CITY||''',
	LOCALOFFICEID='''||USTAGE.LOCALOFFICEID||''',  
	PREMISEID='''||USTAGE.PREMISEID||''',
	STATE= '''||USTAGE.STATE||''' 
	where servpt.objectid='||USTAGE.objectid||' /* CCBTOGIS_GIS_REPLACEMENTS_V1_SPUPDATE */ ' ;
   execute immediate sqlstm ;
   commit;
      exit when tot_cnt > 5600000 ;
end loop;
end;
commit;
dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2) /* CCBTOGIS_GIS_REPLACEMENTS_V1_STOPEDIT*/ ;	
END CCBTOGIS_GIS_REPLACEMENTS;
/

create or replace PROCEDURE CCBTOGIS_GIS_DELETES(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing deletes from the Servicepoint GIS table');
dbms_output.put_line('Switch to the created version version.');
sde.version_util.set_current_version(version_name) /* CCBTOGIS_GIS_DELETES_V1_SETVER */ ;
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1) /* CCBTOGIS_GIS_DELETES_V1_STARTEDIT*/ ;
delete from EDGIS.ZZ_MV_SERVICEPOINT MVVIEW where MVVIEW.SERVICEPOINTID in
	(select ACTION2.SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION ACTION2 where ACTION2.ACTION = 'GISD') /* CCBTOGIS_GIS_DELETES_V1_SPDELETES */;
commit;
dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2) /* CCBTOGIS_GIS_DELETES_V1_STOPEDIT*/;
END CCBTOGIS_GIS_DELETES;
/

create or replace PROCEDURE CCBTOGIS_RETURN_GISVALUES AS
sqlstmt varchar2(2000); 
rowcnt number;
BEGIN
dbms_output.put_line('RETURN GIS Values');
 -- initialize process by clearing old table values
dbms_output.put_line('Clearing Before Table');
  sqlstmt := 'truncate table PGEDATA.TEMP_CCB_SP_OUT_B /* CCBTOGIS_RETURN_GISVALUES_V1_TRUNCATEB */ ';
  dbms_output.put_line(sqlstmt);
  execute immediate sqlstmt ;
  -- delete from PGEDATA.TEMP_CCB_SP_OUT_B;
  -- commit;
dbms_output.put_line('Populate GIS table Before');
Insert into PGEDATA.TEMP_CCB_SP_OUT_B (SERVICEPOINTID,UNIQUESPID,ROBC,SOURCESIDEDEVICEID,CGC12,CIRCUITID,CREATE_DTM,CREATE_USERID,NEWSUBBLOCK)
   select SERVICEPOINTID,UNIQUESPID,ROBC,SOURCESIDEDEVICEID,CGC12,CIRCUITID,CREATE_DTM,CREATE_USERID,NEWSUBBLOCK
     from PGEDATA.TEMP_CCB_SP_OUT_A /* CCBTOGIS_RETURN_GISVALUES_V1_POPULATEB */ ;  
commit;
dbms_output.put_line('Clearing After Table');  
  sqlstmt := 'truncate table PGEDATA.TEMP_CCB_SP_OUT_A /* CCBTOGIS_RETURN_GISVALUES_V1_TRUNCATEA */';
  dbms_output.put_line(sqlstmt);
  execute immediate sqlstmt ;
  -- delete from PGEDATA.TEMP_CCB_SP_OUT_A;
  -- commit;	 
dbms_output.put_line('Populate GIS table After');	 
sde.version_util.set_current_version('SDE.DEFAULT') /* CCBTOGIS_RETURN_GISVALUES_V1_SETVER */ ;
Insert into PGEDATA.TEMP_CCB_SP_OUT_A (
   SERVICEPOINTID,UNIQUESPID,ROBC,
   SOURCESIDEDEVICEID,CGC12,CIRCUITID,
   CREATE_DTM,CREATE_USERID,NEWSUBBLOCK ) 
   select 
   SP.SERVICEPOINTID, SP.UNIQUESPID,
   DECODE(NVL(trim(TR.XMFR_GUID), 'NOVALUE'), 'NOVALUE', PM.PM_ROBC, TR.XFMR_ROBC) ROBC,
   DECODE(NVL(trim(TR.XMFR_GUID), 'NOVALUE'), 'NOVALUE', PM.PM_SOURCESIDEDEVICEID, TR.XFMR_SOURCESIDEDEVICEID) SOURCESIDEDEVICEID, 
   DECODE(NVL(trim(TR.XMFR_GUID), 'NOVALUE'), 'NOVALUE',PM.PM_CGC12, TR.XFMR_CGC12) CGC12,
   DECODE(NVL(trim(TR.XMFR_GUID), 'NOVALUE'), 'NOVALUE', PM.PM_CIRCUITID, TR.XFMR_CIRCUITID) CIRCUITID,
   sysdate, 'GIS_I_WRITE',
   DECODE(NVL(trim(TR.XMFR_GUID), 'NOVALUE'), 'NOVALUE',PM.PM_SUBBLOCK, TR.XFMR_SUBBLOCK) SUBBLOCK  /* CCBTOGIS_RETURN_GISVALUES_V6_POPULATEA */   
from edgis.zz_mv_servicepoint sp
left outer join (
select  TR1.globalid                   XMFR_GUID,
        TR1.CGC12                      XFMR_CGC12,
        TR1.CIRCUITID                  XFMR_CIRCUITID,
        TR1.SOURCESIDEDEVICEID         XFMR_SOURCESIDEDEVICEID,
        TR1.PCPGUID                    XFMR_PCPGUID,
        DECODE(NVL(trim(PCP.PCPROBC), 'NOVALUE'), 'NOVALUE', ROBC.ROBC, PCP.PCPROBC)  XFMR_ROBC,        
        DECODE(NVL(trim(PCP.PCPSUBBLOCK), 'NOVALUE'), 'NOVALUE', ROBC.SUBBLOCK, PCP.PCPSUBBLOCK)   XFMR_SUBBLOCK
  from EDGIS.ZZ_MV_TRANSFORMER TR1
  left outer join
( select cs.circuitid,cs.globalid,rb.ESTABLISHEDROBC robc, rb.ESTABLISHEDSUBBLOCK SUBBLOCK
  from edgis.zz_mv_circuitsource cs left outer join (select * from edgis.zz_mv_robc where PARTCURTAILPOINTGUID is null) rb
  on cs.globalid=rb.circuitsourceguid ) robc
     on tr1.circuitid=robc.circuitid 
left outer join
 (select r.ESTABLISHEDROBC PCPROBC, r.ESTABLISHEDSUBBLOCK PCPSUBBLOCK, p.DEVICEGUID
  from edgis.zz_mv_robc r inner join edgis.ZZ_MV_PARTIALCURTAILPOINT p on r.PARTCURTAILPOINTGUID = p.GlobalID) pcp
  on tr1.PCPGUID=pcp.DEVICEGUID
 ) TR ON sp.transformerguid=TR.XMFR_GUID
left outer join (
SELECT  PM1.globalid                       PM_GUID,
        PM1.CGC12                          PM_CGC12,
        PM1.CIRCUITID                      PM_CIRCUITID,
        PM1.SOURCESIDEDEVICEID             PM_SOURCESIDEDEVICEID,
        PM1.PCPGUID                        PM_PCPGUID,
        DECODE(NVL(trim(PCP.PCPROBC), 'NOVALUE'), 'NOVALUE', ROBC.ROBC, PCP.PCPROBC)  PM_ROBC,
        DECODE(NVL(trim(PCP.PCPSUBBLOCK), 'NOVALUE'), 'NOVALUE', ROBC.SUBBLOCK, PCP.PCPSUBBLOCK)   PM_SUBBLOCK
	from EDGIS.ZZ_MV_PRIMARYMETER	 PM1
left outer join
( select cs.circuitid,cs.globalid,rb.ESTABLISHEDROBC robc, rb.ESTABLISHEDSUBBLOCK SUBBLOCK
  from edgis.zz_mv_circuitsource cs left outer join (select * from edgis.zz_mv_robc where PARTCURTAILPOINTGUID is null) rb
  on cs.globalid=rb.circuitsourceguid ) robc
     on PM1.circuitid=robc.circuitid 
left outer join
 (select r.ESTABLISHEDROBC PCPROBC, r.ESTABLISHEDSUBBLOCK PCPSUBBLOCK, p.DEVICEGUID
  from edgis.zz_mv_robc r inner join edgis.ZZ_MV_PARTIALCURTAILPOINT p on r.PARTCURTAILPOINTGUID = p.GlobalID) pcp
  on PM1.PCPGUID=pcp.DEVICEGUID  
) PM on sp.primarymeterguid=PM.PM_GUID;
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
UPDATE_USERID,
SUBBLOCK)
select 
   SP.SERVICEPOINTID,
   SP.UNIQUESPID,
   TO_CHAR(SP.ROBC) ROBC,
   SP.SOURCESIDEDEVICEID,
   SP.CGC12,
   SP.CIRCUITID,
   SP.CREATE_DTM,
   SP.CREATE_USERID,
   SP.NEWSUBBLOCK /* CCBTOGIS_RETURN_GISVALUES_V1_RETURNGISVALS */
   from (
   select * from pgedata.pge_ccbtoedgis_stg
) ccb
left outer join ( select * from
   PGEDATA.TEMP_CCB_SP_OUT_A SP
) SP
on ccb.servicepointid=sp.servicepointid
where 
   NVL(ccb.NEWROBC,'50') <> DECODE(NVL(TO_CHAR(sp.ROBC),'50'),'60','50',NVL(TO_CHAR(sp.ROBC),'50')) or
   NVL(ccb.SOURCESIDEDEVICEID,'OCB') <> NVL(sp.SOURCESIDEDEVICEID,'OCB') or
   NVL(ccb.CGC12,9999) <> NVL(sp.CGC12,9999) or
   NVL(ccb.CIRCUITID,'000000000') <> NVL(sp.circuitid,'000000000') or 
   NVL(ccb.NEWSUBBLOCK,' ') <> NVL(sp.NEWSUBBLOCK,' ') 
   and SP.CGC12 is not null;	 
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
UPDATE_USERID,
SUBBLOCK)
select 
VSP.SERVICEPOINTID,
VSP.UNIQUESPID,
TO_CHAR(VSP.ROBC) ROBC,
VSP.SOURCESIDEDEVICEID,
VSP.CGC12,
VSP.CIRCUITID,
VSP.CREATE_DTM,
VSP.CREATE_USERID,
VSP.NEWSUBBLOCK /* CCBTOGIS_RETURN_GISVALUES_V1_RETURNGISCHANGES */
from (   
   select 
     SP.SERVICEPOINTID,
     SP.UNIQUESPID,
     SP.ROBC,
     NVL(sp.SOURCESIDEDEVICEID,'OCB') SOURCESIDEDEVICEID,
     SP.CGC12,
     SP.CIRCUITID,
     SP.CREATE_DTM,
     SP.CREATE_USERID,
     SP.NEWSUBBLOCK	 from (
   select * from PGEDATA.TEMP_CCB_SP_OUT_B 
  ) ccb
  left outer join ( select * from
   PGEDATA.TEMP_CCB_SP_OUT_A SP
  ) SP
  on ccb.servicepointid=sp.servicepointid
  where 
   DECODE(NVL(TO_CHAR(ccb.ROBC),'50'),'60','50',NVL(TO_CHAR(ccb.ROBC),'50')) <> DECODE(NVL(TO_CHAR(sp.ROBC),'50'),'60','50',NVL(TO_CHAR(sp.ROBC),'50')) or
   NVL(ccb.SOURCESIDEDEVICEID,'OCB') <> NVL(sp.SOURCESIDEDEVICEID,'OCB') or
   NVL(ccb.CGC12,9999) <> NVL(sp.CGC12,9999) or
   NVL(ccb.CIRCUITID,'000000000') <> NVL(sp.circuitid,'000000000') or
   NVL(ccb.NEWSUBBLOCK,' ') <> NVL(sp.NEWSUBBLOCK,' ') 
) VSP
where 
(NVL(VSP.servicepointid,'0'),NVL(TO_CHAR(VSP.CREATE_DTM,'YYYYMMDDHH24MISS'),'19000101000000') ) not in (
      select NVL(stg1.servicepointid,'0'),NVL(TO_CHAR(stg1.UPDATE_DTM,'YYYYMMDDHH24MISS'),'19000101000000') 
	  from PGEDATA.PGE_EDGISTOCCB_STG stg1
)
and VSP.CGC12 is not null;
commit;
dbms_output.put_line('Returning newly inserted data values');	
insert into PGEDATA.PGE_EDGISTOCCB_STG	 (
SERVICEPOINTID,
UNIQUESPID,
ROBC,
SOURCESIDEDEVICEID,
CGC12,
CIRCUITID,
UPDATE_DTM,
UPDATE_USERID,
SUBBLOCK)
select 
SP.SERVICEPOINTID,
SP.UNIQUESPID,
TO_CHAR(SP.ROBC) ROBC,
SP.SOURCESIDEDEVICEID,
SP.CGC12,
SP.CIRCUITID,
SP.CREATE_DTM,
SP.CREATE_USERID,
SP.NEWSUBBLOCK /* CCBTOGIS_RETURN_GISVALUES_V1_RETURNGISINSERTS */
from ( select * from
   PGEDATA.TEMP_CCB_SP_OUT_A
   minus
   select * from PGEDATA.TEMP_CCB_SP_OUT_A
   where servicepointid in (
   select servicepointid from PGEDATA.TEMP_CCB_SP_OUT_B)
   minus 
   select * from PGEDATA.TEMP_CCB_SP_OUT_A
   where servicepointid in (
   select servicepointid from PGEDATA.PGE_CCBTOEDGIS_STG) 
  ) SP
  where 
  SP.CGC12 is not null;
commit;
dbms_output.put_line('Removing invalid date time');	
delete from pgedata.pge_edgistoccb_stg VSP where UPDATE_DTM is null /* CCBTOGIS_RETURN_GISVALUES_V1_DELNULDATES */;
delete from pgedata.pge_edgistoccb_stg VSP where servicepointid is null /* CCBTOGIS_RETURN_GISVALUES_V1_DELNULSPIDS */;
dbms_output.put_line('Removing Duplicates with same max date time');	
delete from pgedata.pge_edgistoccb_stg VSP 
where (VSP.servicepointid,TO_CHAR(VSP.UPDATE_DTM, 'YYYYMMDDHH24MISS')) in (
 select stg1.servicepointid,TO_CHAR(stg1.UPDATE_DTM, 'YYYYMMDDHH24MISS') 
 from PGEDATA.PGE_EDGISTOCCB_STG stg1
 minus
 select stg2.servicepointid,max(TO_CHAR(stg2.UPDATE_DTM, 'YYYYMMDDHH24MISS')) 
 from PGEDATA.PGE_EDGISTOCCB_STG stg2 
 group by stg2.servicepointid 
) /* CCBTOGIS_RETURN_GISVALUES_V1_DELNONMAX */;
dbms_output.put_line('Removing Exact Duplicates');	
delete from pgedata.pge_edgistoccb_stg sp1 where rowid not in ( 
   select max(rowid) 
   from pgedata.pge_edgistoccb_stg sp2 
   where sp1.servicepointid=sp2.servicepointid
) /* CCBTOGIS_RETURN_GISVALUES_V1_DELDUPLICATES */ ;
commit;
delete from pgedata.pge_edgistoccb_stg where CGC12 is null; /* CCBTOGIS_RETURN_GISVALUES_V1_DELNULCGC12 */
commit;
sqlstmt := 'truncate table pgedata.pge_ccbtoedgis_stg /* CCBTOGIS_RETURN_GISVALUES_V1_TRUNCATECCB */ ';
  dbms_output.put_line(sqlstmt);
  execute immediate sqlstmt ;
update pgedata.PGE_CCB_SP_IO_MONITOR set STATUS = 'Insert-Completed' where INTERFACETYPE = 'Outbound';
commit;
END CCBTOGIS_RETURN_GISVALUES;
/

create or replace PROCEDURE CCBTOGIS_UPDATECWOT(version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
dbms_output.put_line('Performing status changes for all CWOTS fixed');
dbms_output.put_line('Switch to the version for editing.');
sde.version_util.set_current_version(version_name) /* CCBTOGIS_UPDATECWOT_V1_SETVER */ ;
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1) /* CCBTOGIS_UPDATECWOT_V1_STARTEDIT*/ ;
-- First setting the CWOTS as resolved for those that have a transformer or primary meter and were not resolved.
UPDATE edgis.zz_mv_cwot cwot_tab SET DATEFIXED=sysdate,RESOLVED='Y' WHERE cwot_tab.SERVICEPOINTGUID IN
(
	SELECT SP.GLOBALID FROM EDGIS.ZZ_MV_SERVICEPOINT SP 
	WHERE (
	   NVL(SP.TRANSFORMERGUID,'NONE') IN (SELECT NVL(GLOBALID,'NONE2') FROM edgis.zz_mv_transformer) 
	   or
	   NVL(SP.PRIMARYMETERGUID,'NONE')  IN (SELECT NVL(GLOBALID,'NONE2') FROM edgis.zz_mv_primarymeter) 
	   )
	and sp.globalid in (SELECT NVL(cwot2.SERVICEPOINTGUID,'NONE4') FROM EDGIS.ZZ_MV_CWOT cwot2 WHERE cwot2.RESOLVED='N')
)
and cwot_tab.OBJECTID in (SELECT cwot3.OBJECTID FROM EDGIS.ZZ_MV_CWOT cwot3 WHERE cwot3.RESOLVED='N') /* CCBTOGIS_UPDATECWOT_V1_UPDATERESOLVED */ ;
commit;
update edgis.zz_mv_cwot set SERVICEPOINTGUID=null where RESOLVED='Y' and servicepointguid is not null /* CCBTOGIS_UPDATECWOT_V1_REMOVESPIDSFORRESOLVED */ ;
commit;
-- Inserting the new CWOT entries for those service points without a transformer or primary meter.
INSERT INTO edgis.zz_mv_cwot
(
	GLOBALID,
	CREATIONUSER,
	DATECREATED,
	SERVICEPOINTID,
	CGC12,
	DATEFIXED,
	RESOLVED,
	SERVICEPOINTGUID
)
SELECT 
	EDGIS.GDB_GUID as GLOBALID,
	user as CREATIONUSER,
	sysdate as DATECREATED,
	new_cwots.SERVICEPOINTID,
	new_cwots.CGC12,
	NULL as DATEFIXED,
	'N'  as RESOLVED,
	new_cwots.GLOBALID /* CCBTOGIS_UPDATECWOT_V1_INSERTCWOTS */
from
(SELECT 
	SERVICEPOINTID,
	CGC12,
	GLOBALID
FROM EDGIS.ZZ_MV_SERVICEPOINT SP 
	WHERE 
	     NVL(SP.TRANSFORMERGUID,'NONE') NOT IN (SELECT NVL(GLOBALID,'NONE')  FROM edgis.zz_mv_transformer) 
	     and
	     NVL(SP.PRIMARYMETERGUID,'NONE') NOT IN (SELECT NVL(GLOBALID,'NONE')  FROM edgis.zz_mv_primarymeter) 
	     and 
		 NVL(sp.globalid,'NONE') not in (SELECT NVL(cwot2.SERVICEPOINTGUID,'NONE') FROM EDGIS.ZZ_MV_CWOT cwot2 WHERE cwot2.RESOLVED='N')
) new_cwots;
dbms_output.put_line('Stop editing and save the edits.');
sde.version_user_ddl.edit_version(version_name, 2)  /* CCBTOGIS_UPDATECWOT_V1_STOPEDIT*/;
END CCBTOGIS_UPDATECWOT;
/



CREATE OR REPLACE FUNCTION PGEDATA.LOG_PROCESS_STATUS (
  V_Action_Type           Char,
  V_Action_name           VARCHAR2,
  V_Error_text            Varchar2,
  V_num_records_migrated  NUMBER
)
  RETURN VARCHAR2
AS
BEGIN
----------------------------------------------------------------------------
-- Initial VERSION 1.0
----------------------------------------------------------------------------
  if V_Action_Type ='I' then  
     Insert into PROCESS_LOG (ACTION_NAME) values (V_Action_Name);
  End if;  
  if V_Action_Type ='U' then  
      UPDATE PROCESS_LOG
      SET NUM_RECORDS_MIGRATED = num_records_migrated,
          RUN_END              = sysdate
      WHERE ACTION_NAME         = V_Action_Name;
  End if;  
  if V_Action_Type ='E' then  
      update PROCESS_LOG set ERROR_TEXT = V_error_text 
       where ACTION_NAME = V_Action_Name ;
  End if;  
  RETURN 'SUCCESS';
END LOG_PROCESS_STATUS;
/

-- ****************************************************
-- Packages to support TLM, provided by Hitesh Kanuga
-- ****************************************************
--------------------------------------------------------
--  DDL for Procedure GENERATE_CD_TABLES
--------------------------------------------------------
set define off;
CREATE OR REPLACE PROCEDURE "PGEDATA"."GENERATE_CD_TABLES" 
As
     V_Date        date;
     DB_ERROR_CODE VARCHAR2(20);
     DB_ERROR_MSG  VARCHAR2(200);
     V_Default_version sde.versions.State_id%type;
     V_Chgdtl_version sde.versions.State_id%type;
     V_Action_name Varchar2(50);
     V_Status Varchar2(50);
     v_rowsProcessed Number;
begin
   V_Action_name := 'Truncating Before  Temp Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   dbms_output.put_line(V_Action_name||' '||V_Status);
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_a';
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_BANK_a' ;
   execute immediate 'truncate table PGEDATA.Temp_CD_METER_a' ;
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_b';
   execute immediate 'truncate table PGEDATA.Temp_CD_transformer_BANK_b' ;
   execute immediate 'truncate table PGEDATA.Temp_CD_METER_b' ;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Generating Before Temp Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   sde.version_util.set_current_version('GIS_I.Change_Detection_Sync_Tlm');
   DBMS_OUTPUT.PUT_LINE('1');
   Insert into PGEDATA.Temp_CD_transformer_b(CGC_ID,GLOBAL_ID,COAST_INTERIOR_FLG,CLIMATE_ZONE_CD,INSTALLATION_TYP,
        TRANS_DATE,OLD_GLOBAL_ID,LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT )
   SELECT CGC12,GLOBALID,COASTALIDC,CLIMATEZONE,INSTALLATIONTYPE,DATEMODIFIED,REPLACEGUID,
        LOWSIDEVOLTAGE,OPERATINGVOLTAGE,CIRCUITID,VAULT from edgis.zz_mv_transformer;
   DBMS_OUTPUT.PUT_LINE('2');
   Insert into PGEDATA.Temp_CD_transformer_BANK_b(TRF_GLOBAL_ID,BANK_CD,NP_KVA,GLOBAL_ID,PHASE_CD,TRANS_DATE,OLD_GLOBAL_ID,TRF_TYP )
   SELECT TRANSFORMERGUID,BANKCODE,RATEDKVA,GLOBALID,NUMBEROFPHASES,DATEMODIFIED,REPLACEGUID,TRANSFORMERTYPE from edgis.zz_mv_transformerunit;
   DBMS_OUTPUT.PUT_LINE('3');
   Insert into PGEDATA.TEMP_CD_METER_B
   ( SERVICE_POINT_ID,UNQSPID,REV_ACCT_CD,SVC_ST_NUM,SVC_ST_NAME,SVC_ST_NAME2,SVC_CITY,SVC_STATE,SVC_ZIP,CUST_TYP,RATE_SCHED,GLOBAL_ID,
     METER_NUMBER,TRANS_DATE,TRF_GLOBAL_ID)
    SELECT SERVICEPOINTID,UNIQUESPID,REVENUEACCOUNTCODE,STREETNUMBER,STREETNAME1,STREETNAME2,CITY,STATE,ZIP,CUSTOMERTYPE,RATESCHEDULE,
           GLOBALID,METERNUMBER,DATEMODIFIED,TRANSFORMERGUID from edgis.zz_mv_servicepoint;
   DBMS_OUTPUT.PUT_LINE('4');        
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Generating After Temp Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   sde.version_util.set_current_version('SDE.DEFAULT');
   Insert into PGEDATA.Temp_CD_transformer_a(CGC_ID,GLOBAL_ID,COAST_INTERIOR_FLG,CLIMATE_ZONE_CD,INSTALLATION_TYP,
        TRANS_DATE,OLD_GLOBAL_ID,LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT)
   SELECT CGC12,GLOBALID,COASTALIDC,CLIMATEZONE,INSTALLATIONTYPE,DATEMODIFIED,REPLACEGUID,
        LOWSIDEVOLTAGE,OPERATINGVOLTAGE,CIRCUITID,VAULT from edgis.zz_mv_transformer;
  DBMS_OUTPUT.PUT_LINE('1');
   Insert into PGEDATA.Temp_CD_transformer_BANK_a(TRF_GLOBAL_ID,BANK_CD,NP_KVA,GLOBAL_ID,PHASE_CD,TRANS_DATE,OLD_GLOBAL_ID,TRF_TYP )
   SELECT TRANSFORMERGUID,BANKCODE,RATEDKVA,GLOBALID,NUMBEROFPHASES,DATEMODIFIED,REPLACEGUID,TRANSFORMERTYPE from edgis.zz_mv_transformerunit;
   DBMS_OUTPUT.PUT_LINE('2');
   Insert into PGEDATA.TEMP_CD_METER_A
   ( SERVICE_POINT_ID,UNQSPID,REV_ACCT_CD,SVC_ST_NUM,SVC_ST_NAME,SVC_ST_NAME2,SVC_CITY,SVC_STATE,SVC_ZIP,CUST_TYP,RATE_SCHED,GLOBAL_ID,
     METER_NUMBER,TRANS_DATE,TRF_GLOBAL_ID)
    SELECT SERVICEPOINTID,UNIQUESPID,REVENUEACCOUNTCODE,STREETNUMBER,STREETNAME1,STREETNAME2,CITY,STATE,ZIP,CUSTOMERTYPE,RATESCHEDULE,
           GLOBALID,METERNUMBER,DATEMODIFIED,TRANSFORMERGUID  from edgis.zz_mv_servicepoint;
    dbms_stats.gather_table_stats('pgedata','Temp_cd_meter_a');     
    dbms_stats.gather_table_stats('pgedata','Temp_cd_meter_b');
    dbms_stats.gather_table_stats('pgedata','Temp_cd_Transformer_a');
    dbms_stats.gather_table_stats('pgedata','Temp_cd_transformer_b');
    dbms_stats.gather_table_stats('pgedata','Temp_cd_Transformer_bank_a');
    dbms_stats.gather_table_stats('pgedata','Temp_cd_transformer_bank_b');  
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Insert CD_Meter Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.pge_cd_meter
   ( SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
     GLOBAL_ID, SM_FLG, METER_NUMBER, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID )
   select SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
          GLOBAL_ID, SM_FLG, METER_NUMBER, 'I', TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID from PGEDATA.TEMP_CD_METER_A
    where global_id in ( select global_id from PGEDATA.TEMP_CD_METER_A where Nvl(OLD_GLOBAL_ID,' ') not in (select global_id from PGEDATA.TEMP_CD_METER_B)
                        minus
                       select global_id from PGEDATA.TEMP_CD_METER_B );
   v_rowsProcessed := SQL%ROWCOUNT;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Update CD_Meter Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.pge_cd_meter
   ( SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
     GLOBAL_ID, SM_FLG, METER_NUMBER, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID )
      select a.SERVICE_POINT_ID, a.UNQSPID, a.REV_ACCT_CD, a.SVC_ST_NUM, a.SVC_ST_NAME, a.SVC_ST_NAME2, a.SVC_CITY, a.SVC_STATE, a.SVC_ZIP, a.CUST_TYP, a.RATE_SCHED,
       a.GLOBAL_ID, a.SM_FLG, a.METER_NUMBER, 'U', a.TRANS_DATE, a.OLD_GLOBAL_ID, a.TRF_GLOBAL_ID from PGEDATA.TEMP_CD_METER_A a, PGEDATA.TEMP_CD_METER_B b
       where a.global_id = b.global_id
         and ( nvl(a.SERVICE_POINT_ID,' ') <> nvl(b.SERVICE_POINT_ID,' ')
          or nvl(a.UNQSPID, ' ')           <> nvl(b.UNQSPID, ' ')
          or nvl(a.REV_ACCT_CD,' ')        <> nvl(b.REV_ACCT_CD, ' ')
          or nvl(a.SVC_ST_NUM, ' ')        <> nvl(b.SVC_ST_NUM, ' ')
          or nvl(a.SVC_ST_NAME, ' ')       <> nvl(b.SVC_ST_NAME, ' ')
          or nvl(a.SVC_ST_NAME2, ' ')      <> nvl(b.SVC_ST_NAME2, ' ')
          or nvl(a.SVC_CITY,  ' ')         <> nvl(b.SVC_CITY,  ' ')
          or nvl(a.SVC_STATE, ' ')         <> nvl(b.SVC_STATE, ' ')
          or nvl(a.SVC_ZIP, ' ')           <> nvl(b.SVC_ZIP, ' ')
          or nvl(a.CUST_TYP, ' ')          <> nvl(b.CUST_TYP, ' ')
          or nvl(a.RATE_SCHED, ' ')        <> nvl(b.RATE_SCHED, ' ')
          or nvl(a.SM_FLG, ' ')            <> nvl(b.SM_FLG, ' ')
          or nvl(a.METER_NUMBER, ' ')      <> nvl(b.METER_NUMBER, ' ')
          or nvl(a.TRANS_DATE, '01-Jan-1900') <> nvl(b.TRANS_DATE,'01-Jan-1900')
          or nvl(a.OLD_GLOBAL_ID, ' ')     <> nvl(b.OLD_GLOBAL_ID, ' ')
          or nvl(a.TRF_GLOBAL_ID, ' ')     <> nvl(b.TRF_GLOBAL_ID, ' ') );
   v_rowsProcessed := SQL%ROWCOUNT;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Replace CD_Meter Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.pge_cd_meter
   ( SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
     GLOBAL_ID, SM_FLG, METER_NUMBER, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID )
   select SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
          GLOBAL_ID, SM_FLG, METER_NUMBER, 'R', TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID from PGEDATA.TEMP_CD_METER_A
    where GLOBAL_ID in ( select global_id from PGEDATA.TEMP_CD_METER_A where OLD_GLOBAL_ID in (select global_id from PGEDATA.TEMP_CD_METER_B)
                          minus
                          select global_id from PGEDATA.TEMP_CD_METER_B );
   v_rowsProcessed := SQL%ROWCOUNT;
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Delete CD_Meter Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.pge_cd_meter
   ( SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
     GLOBAL_ID, SM_FLG, METER_NUMBER, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID )
   select SERVICE_POINT_ID, UNQSPID, REV_ACCT_CD, SVC_ST_NUM, SVC_ST_NAME, SVC_ST_NAME2, SVC_CITY, SVC_STATE, SVC_ZIP, CUST_TYP, RATE_SCHED,
          GLOBAL_ID, SM_FLG, METER_NUMBER, 'D', TRANS_DATE, OLD_GLOBAL_ID, TRF_GLOBAL_ID from PGEDATA.TEMP_CD_METER_B
    where GLOBAL_ID in ( select global_id from PGEDATA.TEMP_CD_METER_B
                          minus
                          select global_id from PGEDATA.TEMP_CD_METER_A );
   v_rowsProcessed := SQL%ROWCOUNT;                          
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Generating CD_Transformer Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.PGE_CD_TRANSFORMER
   ( CGC_ID,GLOBAL_ID,COAST_INTERIOR_FLG,CLIMATE_ZONE_CD,INSTALLATION_TYP,REGION,TRANS_TYPE,TRANS_DATE,OLD_GLOBAL_ID,
     LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT )
   select CGC_ID,GLOBAL_ID,
     case to_char(coast_interior_flg) when 'Y' then 1 when 'N' then 0   else null end,
     CLIMATE_ZONE_CD,INSTALLATION_TYP,REGION,'I',TRANS_DATE,OLD_GLOBAL_ID,
     LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT from PGEDATA.TEMP_CD_TRANSFORMER_A
    where global_id in ( select global_id from PGEDATA.TEMP_CD_TRANSFORMER_A where Nvl(OLD_GLOBAL_ID,' ') not in (select global_id from PGEDATA.TEMP_CD_TRANSFORMER_B)
                        minus
                       select global_id from PGEDATA.TEMP_CD_TRANSFORMER_B );
   insert into PGEDATA.pge_cd_transformer
   ( CGC_ID,GLOBAL_ID,COAST_INTERIOR_FLG,CLIMATE_ZONE_CD,INSTALLATION_TYP,REGION,TRANS_TYPE,TRANS_DATE,OLD_GLOBAL_ID,
     LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT )
   select a.CGC_ID,a.GLOBAL_ID,
     case to_char(a.coast_interior_flg) when 'Y' then 1 when 'N' then 0   else null end,
     a.CLIMATE_ZONE_CD,a.INSTALLATION_TYP,a.REGION,'U',a.TRANS_DATE,a.OLD_GLOBAL_ID,
     a.LOWSIDE_VOLTAGE,a.OPERATING_VOLTAGE,a.CIRCUIT_ID,a.VAULT from PGEDATA.TEMP_CD_TRANSFORMER_A a, PGEDATA.TEMP_CD_TRANSFORMER_B b
       where a.global_id = b.global_id
         and ( nvl(a.CGC_ID,0) <> nvl(b.CGC_ID,0)
          or nvl(a.COAST_INTERIOR_FLG, ' ')    <> nvl(b.COAST_INTERIOR_FLG, ' ')
          or nvl(a.CLIMATE_ZONE_CD,' ')        <> nvl(b.CLIMATE_ZONE_CD, ' ')
          or nvl(a.INSTALLATION_TYP, ' ')      <> nvl(b.INSTALLATION_TYP, ' ')
          or nvl(a.REGION, 0)                  <> nvl(b.REGION, 0)
          or nvl(a.TRANS_DATE, '01-Jan-1900')  <> nvl(b.TRANS_DATE,'01-Jan-1900')
          or nvl(a.OLD_GLOBAL_ID,  ' ')        <> nvl(b.OLD_GLOBAL_ID,  ' ')
          or nvl(a.LOWSIDE_VOLTAGE, 0)         <> nvl(b.LOWSIDE_VOLTAGE, 0)
          or nvl(a.OPERATING_VOLTAGE, 0)       <> nvl(b.OPERATING_VOLTAGE, 0)
          or nvl(a.CIRCUIT_ID, ' ')            <> nvl(b.CIRCUIT_ID, ' ')
          or nvl(a.VAULT, ' ')                 <> nvl(b.VAULT, ' ') );
   insert into PGEDATA.PGE_CD_TRANSFORMER
   ( CGC_ID,GLOBAL_ID,COAST_INTERIOR_FLG,CLIMATE_ZONE_CD,INSTALLATION_TYP,REGION,TRANS_TYPE,TRANS_DATE,OLD_GLOBAL_ID,
     LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT )
   select CGC_ID,GLOBAL_ID,
     case to_char(coast_interior_flg) when 'Y' then 1 when 'N' then 0   else null end,
     CLIMATE_ZONE_CD,INSTALLATION_TYP,REGION,'R',TRANS_DATE,OLD_GLOBAL_ID,
     LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT from TEMP_CD_TRANSFORMER_A
    where GLOBAL_ID in ( select global_id from PGEDATA.TEMP_CD_TRANSFORMER_A where OLD_GLOBAL_ID in (select global_id from PGEDATA.TEMP_CD_TRANSFORMER_B)
                          minus
                          select global_id from PGEDATA.TEMP_CD_TRANSFORMER_B );
   insert into PGEDATA.PGE_CD_TRANSFORMER
   ( CGC_ID,GLOBAL_ID,COAST_INTERIOR_FLG,CLIMATE_ZONE_CD,INSTALLATION_TYP,REGION,TRANS_TYPE,TRANS_DATE,OLD_GLOBAL_ID,
     LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT )
   select CGC_ID,GLOBAL_ID,
     case to_char(coast_interior_flg) when 'Y' then 1 when 'N' then 0   else null end,
     CLIMATE_ZONE_CD,INSTALLATION_TYP,REGION,'D',TRANS_DATE,OLD_GLOBAL_ID,
     LOWSIDE_VOLTAGE,OPERATING_VOLTAGE,CIRCUIT_ID,VAULT from PGEDATA.TEMP_CD_TRANSFORMER_B
    where GLOBAL_ID in ( select global_id from PGEDATA.TEMP_CD_TRANSFORMER_B
                          minus
                          select global_id from PGEDATA.TEMP_CD_TRANSFORMER_A );
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   V_Action_name := 'Generating CD_Tran_Bank Tables ' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   insert into PGEDATA.PGE_CD_TRANSFORMER_BANK
   ( TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD,'I',TRANS_DATE,OLD_GLOBAL_ID, TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A
    where global_id in ( select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A where Nvl(OLD_GLOBAL_ID,' ') not in (select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_B)
                          minus
                         select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_B );
   insert into PGEDATA.pge_cd_transformer_BANK
   ( TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select A.TRF_GLOBAL_ID, A.BANK_CD, A.NP_KVA, A.GLOBAL_ID, A.PHASE_CD,'U',A.TRANS_DATE,A.OLD_GLOBAL_ID, A.TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A A, PGEDATA.TEMP_CD_TRANSFORMER_BANK_B B
    where a.global_id = b.global_id
         and ( nvl(a.TRF_GLOBAL_ID,' ') <> nvl(b.TRF_GLOBAL_ID,' ')
          or nvl(a.BANK_CD, ' ')        <> nvl(b.BANK_CD, ' ')
          or nvl(a.NP_KVA,0)            <> nvl(b.NP_KVA, 0)
          or nvl(a.PHASE_CD, 0)         <> nvl(b.PHASE_CD,0)
          or nvl(a.TRF_TYP, ' ')        <> nvl(b.TRF_TYP, ' ')
          or nvl(a.TRANS_DATE, '01-Jan-1900') <> nvl(b.TRANS_DATE,'01-Jan-1900')
          or nvl(a.OLD_GLOBAL_ID,  ' ') <> nvl(b.OLD_GLOBAL_ID,  ' ') );
   insert into PGEDATA.PGE_CD_TRANSFORMER_BANK
   ( TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD,'R',TRANS_DATE,OLD_GLOBAL_ID, TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A
    where GLOBAL_ID in ( select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A where OLD_GLOBAL_ID in (select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_B)
                          minus
                          select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_B );
   insert into PGEDATA.PGE_CD_TRANSFORMER_BANK
   ( TRF_GLOBAL_ID, BANK_CD, NP_KVA, GLOBAL_ID, PHASE_CD, TRANS_TYPE, TRANS_DATE, OLD_GLOBAL_ID,  TRF_TYP )
   select TRF_GLOBAL_ID, to_number(BANK_CD), NP_KVA, GLOBAL_ID, PHASE_CD,'D',TRANS_DATE,OLD_GLOBAL_ID, TRF_TYP
     from PGEDATA.TEMP_CD_TRANSFORMER_BANK_B
    where GLOBAL_ID in ( select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_B
                          minus
                          select global_id from PGEDATA.TEMP_CD_TRANSFORMER_BANK_A );
   V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
   select state_id into V_Default_version from sde.versions where upper(name)='DEFAULT' and owner='SDE';
   select state_id into V_Chgdtl_version from sde.versions where upper(name)='CHANGE_DETECTION_SYNC_TLM' and owner='GIS_I';
   sde.version_util.change_version_state('GIS_I.Change_Detection_Sync_Tlm',V_Chgdtl_version,V_Default_version);
EXCEPTION
     WHEN OTHERS THEN
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        DBMS_OUTPUT.PUT_LINE(SQLCODE || ' ' || SQLERRM);
     V_Status := log_process_status('E',V_action_name,DB_ERROR_CODE || DB_ERROR_CODE ,0);
End GENERATE_CD_TABLES;
/

--------------------------------------------------------
--  DDL for Procedure GENERATE_EXT_CCB_METER_LOAD
--------------------------------------------------------
set define off;

  CREATE OR REPLACE PROCEDURE "PGEDATA"."GENERATE_EXT_CCB_METER_LOAD" 
As
     V_Date date;
     DB_ERROR_CODE VARCHAR2(20);
     DB_ERROR_MSG  VARCHAR2(200);
     V_Action_name Varchar2(50);
     V_Status Varchar2(50);
     v_rowsProcessed Number;
begin 
-- Delete existing record for Service point and revanue month which present in staging tables. 

   V_Action_name := 'Deleting Data from Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
   V_Status := log_process_status('I',V_action_name,'',0);
   v_rowsProcessed :=0 ;
   
    delete /*+ parallel(d,4) */ from PGE_EXT_CCB_METER_LOAD d
     Where exists 
    ( select b.UNIQUESPID,b.ACCOUNTNUM, b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
        From PGE_CCBTOEDGIS_STG b
       where b.servicepointid = d.service_point_id
         and to_char(b.batchdate,'MMYYYY') = d.REV_MONTH  || D.REV_YEAR
         and b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG a where a.servicepointid = b.servicepointid 
                                and to_char(a.batchdate,'MMYYYY') = to_char(b.batchdate,'MMYYYY') ));

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);

-- Inserting records for Service point and revanue month which is not present in staging tables. 

    V_Action_name := 'Inserting Data in the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
    V_Status := log_process_status('I',V_action_name,'',0);
    v_rowsProcessed :=0 ;
    
    insert into PGE_EXT_CCB_METER_LOAD (SERVICE_POINT_ID,UNQSPID,ACCT_ID,REV_MONTH,REV_YEAR,REV_KWHR,REV_KW,PFACTOR,SM_SP_STATUS)
    select b.servicepointid,b.UNIQUESPID,b.ACCOUNTNUM,to_char(b.batchdate,'MM'),to_char(b.batchdate,'YYYY'), b.SPPEAKKWHR,b.SPPEAKKW ,b.SPPFACTOR,b.SMFLG
      From PGE_CCBTOEDGIS_STG b
     where b.DATECREATED = ( select max(a.DATECREATED) from PGE_CCBTOEDGIS_STG a where a.servicepointid = b.servicepointid 
                                and to_char(a.batchdate,'MM') = to_char(b.batchdate,'MM') )
       and not exists ( select * from PGE_EXT_CCB_METER_LOAD c 
                         where c.SERVICE_POINT_ID = b.servicepointid  
                           and c.REV_MONTH = to_char(b.batchdate,'MM') 
                           and c.Rev_year  = to_char(b.batchdate,'YYYY')   );

    v_rowsProcessed := SQL%ROWCOUNT;
    V_Status := log_process_status('U',V_action_name,'',v_rowsProcessed);
    
    update PGE_CCB_SP_ACTION set TLMPROCESSED = 'YES' ;
 
    V_Action_name := 'Gathering Stats on the Staging' || to_char(sysdate ,'MM-DD-YYYY HH24:MI');
 
    V_Status := log_process_status('I',V_action_name,'',0);
    dbms_stats.gather_table_stats('PGEDATA','PGE_EXT_CCB_METER_LOAD');
    V_Status := log_process_status('U',V_action_name,'',0);
 
 
    Delete From pge_ccbtoedgis_stg VSP 
     where ( VSP.servicepointid , TO_CHAR(VSP.DATECREATED, 'YYYYMMDDHH24MISS')) 
           in ( Select stg1.servicepointid,TO_CHAR(stg1.DATECREATED, 'YYYYMMDDHH24MISS') 
                  From PGEDATA.pge_ccbtoedgis_stg stg1 where stg1.DATECREATED is not null
                minus
                Select stg2.servicepointid,max(TO_CHAR(stg2.DATECREATED, 'YYYYMMDDHH24MISS')) 
                  from PGEDATA.pge_ccbtoedgis_stg stg2  where stg2.DATECREATED is not null
                 group by stg2.servicepointid 
               );
   
EXCEPTION
     WHEN OTHERS THEN
        DB_ERROR_CODE:=SUBSTR(SQLCODE,1,20);
        DB_ERROR_MSG :=SUBSTR(SQLERRM,1,200);
        DBMS_OUTPUT.PUT_LINE(SQLCODE || ' ' || SQLERRM);    
     V_Status := log_process_status('I',V_action_name,DB_ERROR_CODE || DB_ERROR_CODE ,0);   
End;
/

grant all on CCBTOGIS_INITIALIZE_FILTERS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_INSERTS  to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_REPLACE  to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_UPDATES  to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_NO_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_REPLACEMENTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_RETURN_GISVALUES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
--grant all on CCBTOGIS_CLEANSTAGINGTABLE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_UPDATECWOT to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CCB_SP_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_CCBTOEDGIS_STG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on PGE_EDGISTOCCB_STG to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CD_GIS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on GIS_SAP to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on GENERATE_EXT_CCB_METER_LOAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on GENERATE_CD_TABLES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_UPDATES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_INITIALIZE_FILTERS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_REPLACE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_SP_ACTION_NO_ACTION to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_INSERTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_REPLACEMENTS to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_GIS_DELETES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on CCBTOGIS_RETURN_GISVALUES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
--grant all on CCBTOGIS_CLEANSTAGINGTABLE to GIS_I_WRITE,GIS_I,EDGIS,SDE ;	
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


/*
GRANT all on edgis.ZZ_MV_SERVICEPOINT to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.ZZ_MV_TRANSFORMER to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.ZZ_MV_ROBC to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.ZZ_MV_PRIMARYMETER to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.ZZ_MV_DCRECTIFIER to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;

GRANT all on edgis.SERVICEPOINT to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.TRANSFORMER to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.ROBC to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.PRIMARYMETER to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.DCRECTIFIER to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;

-- select registration_id from sde.table_registry where table_name='ROBC';         REGISTRATION_ID -- 77
-- select registration_id from sde.table_registry where table_name='TRANSFORMER';  REGISTRATION_ID -- 117
-- select registration_id from sde.table_registry where table_name='PRIMARYMETER'; REGISTRATION_ID -- 130
-- select registration_id from sde.table_registry where table_name='SERVICEPOINT'; REGISTRATION_ID -- 71
-- select registration_id from sde.table_registry where table_name='DCRECTIFIER'; REGISTRATION_ID -- 123

GRANT all on edgis.A71 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.A117 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.A77 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.A130 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.A123 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;

GRANT all on edgis.D71 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.D117 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.D77 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.D130 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;
GRANT all on edgis.D123 to PGEDATA,GIS_I,GIS_INTERFACE,GIS_I_WRITE;

*/



grant all on GENERATE_EXT_CCB_METER_LOAD to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
grant all on GENERATE_CD_TABLES to GIS_I_WRITE,GIS_I,EDGIS,SDE ;
