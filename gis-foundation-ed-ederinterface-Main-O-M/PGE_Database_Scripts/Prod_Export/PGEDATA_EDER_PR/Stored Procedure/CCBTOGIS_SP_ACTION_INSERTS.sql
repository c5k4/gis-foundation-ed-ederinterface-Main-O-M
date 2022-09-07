--------------------------------------------------------
--  DDL for Procedure CCBTOGIS_SP_ACTION_INSERTS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."CCBTOGIS_SP_ACTION_INSERTS" AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  dbms_output.put_line( 'Populating the insert values into the Action Table');
sde.version_util.set_current_version('SDE.DEFAULT') /* CCBTOGIS_SP_ACTION_INSERTS_V1_SETVER */;
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED)
select 'GISI', TABLEI.SERVICEPOINTID, TABLEI.DATECREATED /* CCBTOGIS_SP_ACTION_INSERTS_V1_GISI*/
from
(
select STG.SERVICEPOINTID, STG.DATECREATED , STG.ERROR_DESCRIPTION from PGEDATA.PGE_CCBTOEDGIS_STG STG
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
) TABLEI WHERE TABLEI.ERROR_DESCRIPTION IS NULL;
commit;
EXCEPTION
WHEN no_data_found THEN
  dbms_output.put_line('Error');
END CCBTOGIS_SP_ACTION_INSERTS;
