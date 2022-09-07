Prompt drop Procedure CCBTOGIS_INITIALIZE_FILTERS;
DROP PROCEDURE PGEDATA.CCBTOGIS_INITIALIZE_FILTERS
/

Prompt Procedure CCBTOGIS_INITIALIZE_FILTERS;
--
-- CCBTOGIS_INITIALIZE_FILTERS  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.CCBTOGIS_INITIALIZE_FILTERS AS
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


Prompt Grants on PROCEDURE CCBTOGIS_INITIALIZE_FILTERS TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_INITIALIZE_FILTERS TO EDGIS
/

Prompt Grants on PROCEDURE CCBTOGIS_INITIALIZE_FILTERS TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_INITIALIZE_FILTERS TO GIS_I
/

Prompt Grants on PROCEDURE CCBTOGIS_INITIALIZE_FILTERS TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_INITIALIZE_FILTERS TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE CCBTOGIS_INITIALIZE_FILTERS TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_INITIALIZE_FILTERS TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_INITIALIZE_FILTERS TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_INITIALIZE_FILTERS TO IGPEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_INITIALIZE_FILTERS TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_INITIALIZE_FILTERS TO SDE
/
