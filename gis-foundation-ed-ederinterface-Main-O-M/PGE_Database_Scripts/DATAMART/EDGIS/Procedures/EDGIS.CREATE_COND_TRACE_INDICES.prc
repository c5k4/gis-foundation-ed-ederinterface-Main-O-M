Prompt drop Procedure CREATE_COND_TRACE_INDICES;
DROP PROCEDURE EDGIS.CREATE_COND_TRACE_INDICES
/

Prompt Procedure CREATE_COND_TRACE_INDICES;
--
-- CREATE_COND_TRACE_INDICES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.Create_Cond_Trace_Indices AS
BEGIN
	execute immediate 'create index cond_trace_idx_globID_FCID on edgis.PGE_UndergroundNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index cond_trace_idx_Upstream on edgis.PGE_UndergroundNetwork_Trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_UNDERGROUNDNETWORK_TRACE');
END Create_Cond_Trace_Indices ;
/
