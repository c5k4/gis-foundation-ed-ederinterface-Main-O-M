--------------------------------------------------------
--  DDL for Procedure TRUNCATENETWORKMAPTABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRUNCATENETWORKMAPTABLE" AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt:= 'truncate table EDGIS.PGE_FEEDERFEDNETWORK_MAP';
  execute immediate sqlstmt;
  COMMIT;
END;
