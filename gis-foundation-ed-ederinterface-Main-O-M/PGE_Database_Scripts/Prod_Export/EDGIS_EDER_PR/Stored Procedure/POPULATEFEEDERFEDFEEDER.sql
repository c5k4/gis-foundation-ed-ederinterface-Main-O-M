--------------------------------------------------------
--  DDL for Procedure POPULATEFEEDERFEDFEEDER
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."POPULATEFEEDERFEDFEEDER" AS
BEGIN
	execute immediate 'Truncate table edgis.PGE_FeederFedNetwork_Trace';
	insert into edgis.PGE_FeederFedNetwork_Trace (select * from edgis.pge_elecdistnetwork_trace);
	insert into edgis.PGE_FeederFedNetwork_Trace (FEEDERFEDBY,FEEDERID,FROM_FEATURE_EID,TO_FEATURE_EID,TO_FEATURE_GLOBALID,TO_FEATURE_FCID,TO_FEATURE_FEEDERINFO,TO_FEATURE_TYPE,ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL) (select FEEDERFEDBY,FEEDERID,FROM_FEATURE_EID,TO_FEATURE_EID,TO_FEATURE_GLOBALID,TO_FEATURE_FCID,TO_FEATURE_FEEDERINFO,TO_FEATURE_TYPE,ORDER_NUM,MIN_BRANCH,MAX_BRANCH,TREELEVEL from edgis.PGE_SubGeomNetwork_Trace);
END PopulateFeederFedFeeder ;
