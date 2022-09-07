--------------------------------------------------------
--  DDL for Procedure DROP_FEEDERFED_TRACE_INDICES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."DROP_FEEDERFED_TRACE_INDICES" AS
BEGIN
	BEGIN
	execute immediate 'drop index feedfed_trace_idx_globID_FCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index feedfedidx_globIDSchemFCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index feederfed_idx_Upstream';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index feederfed_idx_Upstream2';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index temp_trace_idx_FCID_OID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
END Drop_FeederFed_Trace_Indices ;
