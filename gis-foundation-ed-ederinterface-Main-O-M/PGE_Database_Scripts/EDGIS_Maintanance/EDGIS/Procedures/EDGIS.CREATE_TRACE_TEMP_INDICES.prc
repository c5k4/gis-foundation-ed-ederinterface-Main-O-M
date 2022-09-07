Prompt drop Procedure CREATE_TRACE_TEMP_INDICES;
DROP PROCEDURE EDGIS.CREATE_TRACE_TEMP_INDICES
/

Prompt Procedure CREATE_TRACE_TEMP_INDICES;
--
-- CREATE_TRACE_TEMP_INDICES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE EDGIS.Create_Trace_Temp_Indices AS
BEGIN
	execute immediate 'create index temp_trace_idx_FCID_OID on edgis.PGE_TRACE_TEMP(TO_FEATURE_FCID, TO_FEATURE_OID)';
	dbms_stats.gather_table_stats('EDGIS','PGE_TRACE_TEMP');
END Create_Trace_Temp_Indices ;
/
