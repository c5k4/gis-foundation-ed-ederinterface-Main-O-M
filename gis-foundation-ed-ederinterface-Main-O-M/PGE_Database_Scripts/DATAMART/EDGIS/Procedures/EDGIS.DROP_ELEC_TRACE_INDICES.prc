Prompt drop Procedure DROP_ELEC_TRACE_INDICES;
DROP PROCEDURE EDGIS.DROP_ELEC_TRACE_INDICES
/

Prompt Procedure DROP_ELEC_TRACE_INDICES;
--
-- DROP_ELEC_TRACE_INDICES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.Drop_Elec_Trace_Indices AS
BEGIN
	BEGIN
	execute immediate 'drop index elec_trace_idx_globID_FCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index electraceidx_globIDSchemFCID';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_trace_idx_Upstream';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_net_idx_fcid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_trace_idx_type_and_eid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index elec_net_idx_fcid_feattype_eid';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
	BEGIN
	execute immediate 'drop index trace_info_idx_glids';
	EXCEPTION when others then
	if sqlcode = -01418 then
	DBMS_OUTPUT.PUT_LINE('Index does not exist.');
	end if;
	END;
END Drop_Elec_Trace_Indices ;
/


Prompt Grants on PROCEDURE DROP_ELEC_TRACE_INDICES TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.DROP_ELEC_TRACE_INDICES TO GISINTERFACE
/
