Prompt drop Procedure CREATE_SUB_TRACE_INDICES;
DROP PROCEDURE EDGIS.CREATE_SUB_TRACE_INDICES
/

Prompt Procedure CREATE_SUB_TRACE_INDICES;
--
-- CREATE_SUB_TRACE_INDICES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.Create_Sub_Trace_Indices AS
BEGIN
	execute immediate 'create index sub_trace_idx_globID_FCID on edgis.pge_subgeomnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index sub_trace_idx_Upstream on edgis.pge_subgeomnetwork_trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_SubGeomNetwork_Trace');
END Create_Sub_Trace_Indices ;
/


Prompt Grants on PROCEDURE CREATE_SUB_TRACE_INDICES TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.CREATE_SUB_TRACE_INDICES TO GISINTERFACE
/
