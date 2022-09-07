--------------------------------------------------------
--  DDL for Procedure TRACING_SUBSTATION
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRACING_SUBSTATION" (P_TO_FEATURE_GLOBALID IN VARCHAR2, P_FEEDERID IN VARCHAR2, P_FEATURE_CLASS_ID IN VARCHAR2, P_TRACING_TYPE IN VARCHAR2, P_TRACING_ON IN VARCHAR2, P_TRACING_CURSOR OUT SYS_REFCURSOR)
AS
  v_to_feature_fcid VARCHAR2(100);
  v_order_num VARCHAR2(100);
  v_min_branch VARCHAR2(100);
  v_max_branch VARCHAR2(100);
  v_treelevel VARCHAR2(100);
  v_to_feature_feederinfo VARCHAR2(100);
  v_feederid VARCHAR2(100);
  v_feederfedby VARCHAR2(100);
  v_query VARCHAR2(25000);
  
BEGIN  
  IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
    IF UPPER(P_TRACING_ON) = 'DEVICE' THEN
      SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY INTO v_order_num, v_min_branch, v_max_branch, v_treelevel, v_to_feature_feederinfo, v_feederid, v_feederfedby FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_VW WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND TO_FEATURE_FCID = P_FEATURE_CLASS_ID AND FEEDERID = P_FEEDERID;
      v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where (a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH >= ' || v_min_branch || ' AND a.MAX_BRANCH <= ' || v_max_branch || ' AND a.TREELEVEL >= ' || v_treelevel || ' AND a.ORDER_NUM <= ' || v_order_num || ' order by a.order_num desc';
    ELSIF UPPER(P_TRACING_ON) = 'PROTECTIVEDEVICE' THEN
      SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY INTO v_order_num, v_min_branch, v_max_branch, v_treelevel, v_to_feature_feederinfo, v_feederid, v_feederfedby FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_VW WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND TO_FEATURE_FCID = P_FEATURE_CLASS_ID AND FEEDERID = P_FEEDERID;
      v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where (a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH >= ' || v_min_branch || ' AND a.MAX_BRANCH <= ' || v_max_branch || ' AND a.TREELEVEL >= ' || v_treelevel || ' AND a.ORDER_NUM <= ' || v_order_num || ' AND (TO_FEATURE_FCID IN (1001,1014) OR (TO_FEATURE_FCID IN (1021,1023,' || P_FEATURE_CLASS_ID || ') AND TO_CIRCUITID IS NOT NULL)) order by a.order_num desc';
    ELSE
      SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, FEEDERID, FEEDERFEDBY INTO v_order_num, v_min_branch, v_max_branch, v_treelevel, v_feederid, v_feederfedby FROM edgis.PGE_UNDERGROUNDNETWORK_TRACE WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND TO_FEATURE_FCID = P_FEATURE_CLASS_ID AND FEEDERID = P_FEEDERID;
      v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname,a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from edgis.PGE_UNDERGROUNDNETWORK_TRACE a LEFT JOIN sde.GDB_ITEMS b ON a.to_feature_fcid = b.objectid where (a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH >= ' || v_min_branch || ' AND a.MAX_BRANCH <= ' || v_max_branch || ' AND a.TREELEVEL >= ' || v_treelevel || ' AND a.ORDER_NUM <= ' || v_order_num || ' AND TO_FEATURE_FCID IN (1017,1018) AND SUBTYPECD IN (3) order by a.order_num desc';
    END IF;
  ELSE
    IF UPPER(P_TRACING_ON) = 'DEVICE' THEN
      SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY INTO v_order_num, v_min_branch, v_max_branch, v_treelevel, v_to_feature_feederinfo, v_feederid, v_feederfedby FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_VW WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND TO_FEATURE_FCID = P_FEATURE_CLASS_ID AND FEEDERID = P_FEEDERID;
      v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FROM_CIRCUITID,a.FEEDERFEDBY,a.FROM_LINE_GLOBALID,a.FEEDERID,a.FROM_POINT_FC_GUID from edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid  where (a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH <= ' || v_min_branch || ' AND a.MAX_BRANCH >= ' || v_max_branch || ' AND a.TREELEVEL <= ' || v_treelevel || ' AND a.ORDER_NUM >= ' || v_order_num || ' order by a.order_num asc';
     ELSIF UPPER(P_TRACING_ON) = 'PROTECTIVEDEVICE' THEN
      SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY INTO v_order_num, v_min_branch, v_max_branch, v_treelevel, v_to_feature_feederinfo, v_feederid, v_feederfedby FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_VW WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND TO_FEATURE_FCID = P_FEATURE_CLASS_ID AND FEEDERID = P_FEEDERID;
      v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FROM_CIRCUITID,a.FEEDERFEDBY,a.FROM_LINE_GLOBALID,a.FEEDERID,a.FROM_POINT_FC_GUID from edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid  where (a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH <= ' || v_min_branch || ' AND a.MAX_BRANCH >= ' || v_max_branch || ' AND a.TREELEVEL <= ' || v_treelevel || ' AND a.ORDER_NUM >= ' || v_order_num || ' AND (TO_FEATURE_FCID IN (1001,1014) OR (TO_FEATURE_FCID IN (1021,1023) AND TO_CIRCUITID IS NOT NULL)) order by a.order_num asc';
    ELSE
      SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, FEEDERID, FEEDERFEDBY INTO v_order_num, v_min_branch, v_max_branch, v_treelevel, v_feederid, v_feederfedby FROM edgis.PGE_UNDERGROUNDNETWORK_TRACE WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND TO_FEATURE_FCID = P_FEATURE_CLASS_ID AND FEEDERID = P_FEEDERID;
      v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname,a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from edgis.PGE_UNDERGROUNDNETWORK_TRACE a LEFT JOIN sde.GDB_ITEMS b ON a.to_feature_fcid = b.objectid where (a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH <= ' || v_min_branch || ' AND a.MAX_BRANCH >= ' || v_max_branch || ' AND a.TREELEVEL <= ' || v_treelevel || ' AND a.ORDER_NUM >= ' || v_order_num || ' AND TO_FEATURE_FCID IN (1017,1018) AND SUBTYPECD IN (3) order by a.order_num asc';
    END IF;
  END IF;
  dbms_output.put_line(v_query);
  open P_TRACING_CURSOR for v_query;    
END;
