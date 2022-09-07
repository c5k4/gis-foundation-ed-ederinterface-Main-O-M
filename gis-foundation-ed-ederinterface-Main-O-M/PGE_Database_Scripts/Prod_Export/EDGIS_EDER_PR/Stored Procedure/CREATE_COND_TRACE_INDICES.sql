--------------------------------------------------------
--  DDL for Procedure CREATE_COND_TRACE_INDICES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."CREATE_COND_TRACE_INDICES" AS
BEGIN
	execute immediate 'create index cond_trace_idx_globID_FCID on edgis.PGE_UndergroundNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index cond_trace_idx_Upstream on edgis.PGE_UndergroundNetwork_Trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_UNDERGROUNDNETWORK_TRACE');
END Create_Cond_Trace_Indices ;
