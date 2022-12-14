Prompt drop Procedure CCBTOGIS_GIS_DELETES;
DROP PROCEDURE PGEDATA.CCBTOGIS_GIS_DELETES
/

Prompt Procedure CCBTOGIS_GIS_DELETES;
--
-- CCBTOGIS_GIS_DELETES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE PGEDATA.CCBTOGIS_GIS_DELETES(version_name IN VARCHAR2) AS
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


Prompt Grants on PROCEDURE CCBTOGIS_GIS_DELETES TO EDGIS to EDGIS;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_GIS_DELETES TO EDGIS
/

Prompt Grants on PROCEDURE CCBTOGIS_GIS_DELETES TO GIS_I to GIS_I;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_GIS_DELETES TO GIS_I
/

Prompt Grants on PROCEDURE CCBTOGIS_GIS_DELETES TO GIS_I_WRITE to GIS_I_WRITE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_GIS_DELETES TO GIS_I_WRITE
/

Prompt Grants on PROCEDURE CCBTOGIS_GIS_DELETES TO IGPCITEDITOR to IGPCITEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_GIS_DELETES TO IGPCITEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_GIS_DELETES TO IGPEDITOR to IGPEDITOR;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_GIS_DELETES TO IGPEDITOR
/

Prompt Grants on PROCEDURE CCBTOGIS_GIS_DELETES TO SDE to SDE;
GRANT EXECUTE, DEBUG ON PGEDATA.CCBTOGIS_GIS_DELETES TO SDE
/
