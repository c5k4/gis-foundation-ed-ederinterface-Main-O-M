--------------------------------------------------------
--  DDL for Procedure JOINTEMPTABLES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."JOINTEMPTABLES" (base_temp_table_name IN VARCHAR2,numTempTables in number) AS

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
