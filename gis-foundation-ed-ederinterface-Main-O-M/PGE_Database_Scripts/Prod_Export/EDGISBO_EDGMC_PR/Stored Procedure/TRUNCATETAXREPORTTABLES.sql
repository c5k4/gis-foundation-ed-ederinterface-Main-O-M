--------------------------------------------------------
--  DDL for Procedure TRUNCATETAXREPORTTABLES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGISBO"."TRUNCATETAXREPORTTABLES" (tablename IN VARCHAR2)
AS
BEGIN
IF (upper(tablename) = 'EDGISBO.WORKORDERSTRUCTURE') OR (upper(tablename) = 'EDGISBO.WORKORDERSTRUCTURE_STAGING') THEN
   execute immediate 'TRUNCATE TABLE ' || tablename;
END IF;
END;
