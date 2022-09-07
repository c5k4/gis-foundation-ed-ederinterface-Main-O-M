--------------------------------------------------------
--  DDL for Procedure CREATE_ELEC_TRACE_INDICES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."CREATE_ELEC_TRACE_INDICES" AS
BEGIN
	execute immediate 'create index elec_trace_idx_globID_FCID on edgis.pge_elecdistnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index electraceidx_globIDSchemFCID on edgis.pge_elecdistnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_SCHEM_FCID)';
	execute immediate 'create index elec_trace_idx_Upstream on edgis.pge_elecdistnetwork_trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_ELECDISTNETWORK_TRACE');
END Create_Elec_Trace_Indices ;
