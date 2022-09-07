Prompt drop Procedure CREATE_ELEC_TRACE_INDICES;
DROP PROCEDURE EDGIS.CREATE_ELEC_TRACE_INDICES
/

Prompt Procedure CREATE_ELEC_TRACE_INDICES;
--
-- CREATE_ELEC_TRACE_INDICES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.Create_Elec_Trace_Indices AS
BEGIN
	execute immediate 'create index elec_trace_idx_globID_FCID on edgis.pge_elecdistnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index electraceidx_globIDSchemFCID on edgis.pge_elecdistnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_SCHEM_FCID)';
	execute immediate 'create index elec_trace_idx_Upstream on edgis.pge_elecdistnetwork_trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_ELECDISTNETWORK_TRACE');
END Create_Elec_Trace_Indices ;
/


Prompt Grants on PROCEDURE CREATE_ELEC_TRACE_INDICES TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.CREATE_ELEC_TRACE_INDICES TO GISINTERFACE
/
