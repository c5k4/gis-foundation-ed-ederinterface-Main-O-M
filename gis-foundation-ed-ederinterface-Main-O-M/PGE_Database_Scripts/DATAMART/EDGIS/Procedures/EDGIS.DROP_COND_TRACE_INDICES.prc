Prompt drop Procedure DROP_COND_TRACE_INDICES;
DROP PROCEDURE EDGIS.DROP_COND_TRACE_INDICES
/

Prompt Procedure DROP_COND_TRACE_INDICES;
--
-- DROP_COND_TRACE_INDICES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.Drop_Cond_Trace_Indices AS
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
/
