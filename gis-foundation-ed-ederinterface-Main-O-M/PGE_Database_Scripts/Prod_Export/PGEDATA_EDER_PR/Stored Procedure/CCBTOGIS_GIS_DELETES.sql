--------------------------------------------------------
--  DDL for Procedure CCBTOGIS_GIS_DELETES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "PGEDATA"."CCBTOGIS_GIS_DELETES" (version_name IN VARCHAR2) AS
sqlstmt varchar2(2000);
rowcnt number;
h_sap_spid NVARCHAR2 (10)  := 0;

BEGIN
dbms_output.put_line('Performing deletes from the Servicepoint GIS table');
dbms_output.put_line('Switch to the created version version.');

sde.version_util.set_current_version(version_name) /* CCBTOGIS_GIS_DELETES_V1_SETVER */ ;
-- START EDITING AGAINST THE VERSION
dbms_output.put_line('Start editing.');
sde.version_user_ddl.edit_version(version_name, 1) /* CCBTOGIS_GIS_DELETES_V1_STARTEDIT*/ ;

FOR i in (SELECT SERVICEPOINTID from PGEDATA.PGE_CCB_SP_ACTION where ACTION = 'GISD')
LOOP
	h_sap_spid := i.servicepointid;
	BEGIN

		delete from EDGIS.ZZ_MV_SERVICEPOINT MVVIEW where MVVIEW.SERVICEPOINTID = h_sap_spid;

		rowcnt:=sql%rowcount;
		INSERT INTO PGEDATA.CCB_SPID_DELETE_TEMP VALUES (h_sap_spid,rowcnt, SYSDATE);
		commit;
	END;
END LOOP;
 /* CCBTOGIS_GIS_DELETES_V1_SPDELETES */

dbms_output.put_line('Stop editing.');
sde.version_user_ddl.edit_version(version_name, 2) /* CCBTOGIS_GIS_DELETES_V1_STOPEDIT*/;
END CCBTOGIS_GIS_DELETES;
