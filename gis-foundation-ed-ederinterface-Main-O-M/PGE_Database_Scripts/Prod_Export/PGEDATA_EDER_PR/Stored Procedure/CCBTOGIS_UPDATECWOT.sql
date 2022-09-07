--------------------------------------------------------
--  DDL for Procedure CCBTOGIS_UPDATECWOT
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."CCBTOGIS_UPDATECWOT" (version_name IN VARCHAR2) AS
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
