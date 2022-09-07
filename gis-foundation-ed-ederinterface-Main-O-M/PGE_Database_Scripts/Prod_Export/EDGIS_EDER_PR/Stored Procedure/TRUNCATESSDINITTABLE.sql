--------------------------------------------------------
--  DDL for Procedure TRUNCATESSDINITTABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRUNCATESSDINITTABLE" AS
sqlstmt varchar2(2000);
rowcnt number;
BEGIN
  sqlstmt:= 'truncate table EDGIS.PGE_SSD_Initialization';
  execute immediate sqlstmt;
  COMMIT;
END;
