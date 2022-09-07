Prompt drop Procedure CCBTOGIS_RETURN_GISVALUES;
DROP PROCEDURE PGEDATA.CCBTOGIS_RETURN_GISVALUES
/

Prompt Procedure CCBTOGIS_RETURN_GISVALUES;
--
-- CCBTOGIS_RETURN_GISVALUES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.CCBTOGIS_RETURN_GISVALUES AS
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


Prompt Grants on PROCEDURE CCBTOGIS_RETURN_GISVALUES TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_RETURN_GISVALUES TO EDGIS
/

Prompt Grants on PROCEDURE CCBTOGIS_RETURN_GISVALUES TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_RETURN_GISVALUES TO GIS_I
/

Prompt Grants on PROCEDURE CCBTOGIS_RETURN_GISVALUES TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_RETURN_GISVALUES TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE CCBTOGIS_RETURN_GISVALUES TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_RETURN_GISVALUES TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_RETURN_GISVALUES TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_RETURN_GISVALUES TO IGPEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_RETURN_GISVALUES TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_RETURN_GISVALUES TO SDE
/
