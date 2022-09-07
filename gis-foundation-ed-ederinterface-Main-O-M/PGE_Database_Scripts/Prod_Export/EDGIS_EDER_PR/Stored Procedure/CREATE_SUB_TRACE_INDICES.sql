--------------------------------------------------------
--  DDL for Procedure CREATE_SUB_TRACE_INDICES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."CREATE_SUB_TRACE_INDICES" AS
BEGIN
	execute immediate 'create index sub_trace_idx_globID_FCID on edgis.pge_subgeomnetwork_trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index sub_trace_idx_Upstream on edgis.pge_subgeomnetwork_trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_SubGeomNetwork_Trace');
END Create_Sub_Trace_Indices ;
