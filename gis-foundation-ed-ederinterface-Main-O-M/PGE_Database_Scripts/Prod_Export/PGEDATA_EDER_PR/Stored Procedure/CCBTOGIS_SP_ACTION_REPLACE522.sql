--------------------------------------------------------
--  DDL for Procedure CCBTOGIS_SP_ACTION_REPLACE522
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."CCBTOGIS_SP_ACTION_REPLACE522" AS
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
END CCBTOGIS_SP_ACTION_REPLACE522;
