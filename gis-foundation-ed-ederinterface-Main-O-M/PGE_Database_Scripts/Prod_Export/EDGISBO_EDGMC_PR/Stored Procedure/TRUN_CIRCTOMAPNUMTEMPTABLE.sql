--------------------------------------------------------
--  DDL for Procedure TRUN_CIRCTOMAPNUMTEMPTABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGISBO"."TRUN_CIRCTOMAPNUMTEMPTABLE" AS
BEGIN
	execute immediate 'Truncate table edgisbo.PGE_CIRCUITTOMAPNUM_TEMP';
END TRUN_CIRCTOMAPNUMTEMPTABLE ;
