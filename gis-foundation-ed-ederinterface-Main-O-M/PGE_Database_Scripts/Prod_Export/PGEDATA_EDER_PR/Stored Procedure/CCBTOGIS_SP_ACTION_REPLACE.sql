--------------------------------------------------------
--  DDL for Procedure CCBTOGIS_SP_ACTION_REPLACE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."CCBTOGIS_SP_ACTION_REPLACE" AS
sqlstmt varchar2(2000);
rowcnt number;
-- 05/19/20 - TJJ4 updated with "materialize" hint provided by Chris Lawson
-- formatted sql for clarity
-- replaced literals with variables to get Oracle 12 to use a better
-- execution plan, Chris observed that the conversion of literals to
-- bind variables in a regular (non-procedure) sql statement resulted
-- in an execelent execution plan, we can trigger that same execution
-- plan by replacing the imbedded literals with variables as below
--2/12/2022 - T1KJ Optimized the query with Inner Join clause to run it faster. There is scope to optimize this query further. Recommend Chris to review this query
											  
BEGIN
dbms_output.put_line('Populating the replace values into the Action Table');
sde.version_util.set_current_version('SDE.DEFAULT') /* CCBTOGIS_SP_ACTION_REPLACE_V1_SETVER */ ;
insert into PGEDATA.PGE_CCB_SP_ACTION (ACTION, SERVICEPOINTID, DATEINSERTED)
select 'GISR', SERVICEPOINTID, DATECREATED /* CCBTOGIS_SP_ACTION_REPLACE_V1_GISR*/
from
						  
(select STG2.SERVICEPOINTID, STG2.DATECREATED
from PGEDATA.PGE_CCBTOEDGIS_STG STG2
where NVL(STG2.SERVICEPOINTID,' ') In
(select SERVICEPOINTID
from PGEDATA.PGE_CCBTOEDGIS_STG STAGETABLE2
Inner Join (select max(stg1.DATECREATED) DATECRTD,NVL(stg1.SERVICEPOINTID,' ') SRVCPTID
														  
															  
from PGEDATA.PGE_CCBTOEDGIS_STG stg1
																								
															
where NVL(stg1.UNIQUESPID,' ') in (select distinct NVL(UNIQUESPID,' ') UNIQUESPID
from EDGIS.ZZ_MV_SERVICEPOINT ) group by NVL(stg1.SERVICEPOINTID,' ')) STG2
ON STAGETABLE2.DATECREATED = STG2.DATECRTD AND NVL(STAGETABLE2.SERVICEPOINTID,' ') = STG2.SRVCPTID
where STAGETABLE2.error_description is NULL AND STAGETABLE2.SERVICEPOINTID is not NUll
))PRE
where nvl(servicepointid,' ') not in
(select MVVIEW.SERVICEPOINTID
from EDGIS.ZZ_MV_SERVICEPOINT MVVIEW ); --112
commit;
EXCEPTION
WHEN no_data_found THEN
dbms_output.put_line('Error');
END CCBTOGIS_SP_ACTION_REPLACE;
