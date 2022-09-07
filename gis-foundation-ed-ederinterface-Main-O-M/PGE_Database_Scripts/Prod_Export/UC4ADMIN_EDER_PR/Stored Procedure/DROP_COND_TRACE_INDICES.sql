--------------------------------------------------------
--  DDL for Procedure DROP_COND_TRACE_INDICES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "UC4ADMIN"."DROP_COND_TRACE_INDICES" AS
BEGIN
	BEGIN
	execute immediate 'drop index cond_trace_idx_globID_FCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index cond_trace_idx_Upstream';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
END Drop_Cond_Trace_Indices ;
