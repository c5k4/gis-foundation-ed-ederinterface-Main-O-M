--------------------------------------------------------
--  DDL for Procedure CREATE_FEEDERFED_TRACE_INDICES
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."CREATE_FEEDERFED_TRACE_INDICES" AS
BEGIN
	execute immediate 'create index feedfed_trace_idx_globID_FCID on edgis.PGE_FeederFedNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_FCID)';
	execute immediate 'create index feedfedidx_globIDSchemFCID on edgis.PGE_FeederFedNetwork_Trace(TO_FEATURE_GLOBALID,TO_FEATURE_SCHEM_FCID)';
	execute immediate 'create index feederfed_idx_Upstream on edgis.PGE_FeederFedNetwork_Trace(FEEDERID,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	execute immediate 'create index feederfed_idx_Upstream2 on edgis.PGE_FeederFedNetwork_Trace(FEEDERFEDBY,MIN_BRANCH,MAX_BRANCH,TREELEVEL)';
	dbms_stats.gather_table_stats('EDGIS','PGE_FeederFedNetwork_Trace');
END Create_FeederFed_Trace_Indices ;
