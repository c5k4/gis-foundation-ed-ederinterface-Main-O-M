Prompt drop Procedure CREATE_FEEDERFED_TRACE_INDICES;
DROP PROCEDURE EDGIS.CREATE_FEEDERFED_TRACE_INDICES
/

Prompt Procedure CREATE_FEEDERFED_TRACE_INDICES;
--
-- CREATE_FEEDERFED_TRACE_INDICES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.Create_FeederFed_Trace_Indices AS
BEGIN
	execute immediate 'create index feedfed_trace_idx_globID_FCID on edgis.PGE_FeederFedNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index feedfedidx_globIDSchemFCID on edgis.PGE_FeederFedNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_SCHEM_FCID)';
	execute immediate 'create index feederfed_idx_Upstream on edgis.PGE_FeederFedNetwork_Trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	execute immediate 'create index feederfed_idx_Upstream2 on edgis.PGE_FeederFedNetwork_Trace(FEEDERFEDBY,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_FeederFedNetwork_Trace');
END Create_FeederFed_Trace_Indices ;
/


Prompt Grants on PROCEDURE CREATE_FEEDERFED_TRACE_INDICES TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON EDGIS.CREATE_FEEDERFED_TRACE_INDICES TO GISINTERFACE
/
