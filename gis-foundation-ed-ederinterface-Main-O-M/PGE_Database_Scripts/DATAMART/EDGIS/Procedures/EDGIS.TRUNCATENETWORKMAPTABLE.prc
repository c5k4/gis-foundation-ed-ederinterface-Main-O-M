Prompt drop Procedure TRUNCATENETWORKMAPTABLE;
DROP PROCEDURE EDGIS.TRUNCATENETWORKMAPTABLE
/

Prompt Procedure TRUNCATENETWORKMAPTABLE;
--
-- TRUNCATENETWORKMAPTABLE  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.TruncateNetworkMapTable AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt:= 'truncate table EDGIS.PGE_FEEDERFEDNETWORK_MAP';
  execute immediate sqlstmt;
  COMMIT;
END;
/


Prompt Grants on PROCEDURE TRUNCATENETWORKMAPTABLE TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.TRUNCATENETWORKMAPTABLE TO GISINTERFACE
/
