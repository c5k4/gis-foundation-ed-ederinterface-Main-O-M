Prompt drop Procedure JOINTEMPTABLES;
DROP PROCEDURE EDGIS.JOINTEMPTABLES
/

Prompt Procedure JOINTEMPTABLES;
--
-- JOINTEMPTABLES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.JoinTempTables(base_temp_table_name IN VARCHAR2,numTempTables in number) AS

counter number;
sqlstmt varchar2(2000);

BEGIN
	counter := 0;
	sqlstmt := 'truncate table EDGIS.PGE_ElecDistNetwork_Trace';
	execute immediate sqlstmt;
	WHILE counter<numTempTables
	LOOP
		sqlstmt := 'insert into EDGIS.PGE_ElecDistNetwork_Trace (select * from '||base_temp_table_name||counter||')';
		execute immediate sqlstmt;
		counter := counter + 1;
		COMMIT;
	END LOOP;
END;
/


Prompt Grants on PROCEDURE JOINTEMPTABLES TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.JOINTEMPTABLES TO GISINTERFACE
/
