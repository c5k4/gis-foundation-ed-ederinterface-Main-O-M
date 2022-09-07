--------------------------------------------------------
--  DDL for Procedure TRUNCATEED08TEMPSTRUCTTABLE
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGISBO"."TRUNCATEED08TEMPSTRUCTTABLE" AS
BEGIN
	execute immediate 'Truncate table edgisbo.WORKORDERSTRUCTURE_TEMP';
END TruncateED08TempStructTable ;
