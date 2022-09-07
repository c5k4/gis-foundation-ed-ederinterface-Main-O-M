--------------------------------------------------------
--  DDL for Procedure TRACING_PONS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRACING_PONS" (P_TO_FEATURE_GLOBALID IN VARCHAR2, P_FEEDERID IN VARCHAR2, P_FEATURE_CLASS_ID IN VARCHAR2, P_FEATURE_CLASS_ID2 IN VARCHAR2, P_TRACING_CURSOR OUT SYS_REFCURSOR)
AS 
  v_order_number edgis.pge_feederfednetwork_trace_vw.order_num%TYPE;
  v_min_branch edgis.pge_feederfednetwork_trace_vw.min_branch%TYPE;
  v_max_branch edgis.pge_feederfednetwork_trace_vw.max_branch%TYPE;
  v_tree_level edgis.pge_feederfednetwork_trace_vw.treelevel%TYPE;
  v_to_feature_feederinfo edgis.pge_feederfednetwork_trace_vw.to_feature_feederinfo%TYPE;
  v_feederid edgis.pge_feederfednetwork_trace_vw.feederid%TYPE;
  v_feederfedby edgis.pge_feederfednetwork_trace_vw.feederfedby%TYPE;
  v_query1 VARCHAR2(25000);
  v_query2 VARCHAR2(25000); 
  v_final_query VARCHAR2(25000);
  counter number := 0;
  status_flag number := 0;
  c_query  sys_refcursor;
  v_table1 VARCHAR2(100);
 
BEGIN 
  --dbms_output.put_line('START');
  SELECT tracetablename INTO v_table1 FROM trace_cache_config WHERE tracetype = 'ELECTRIC';
  IF P_TO_FEATURE_GLOBALID != 'null' AND P_FEEDERID != 'null' AND P_FEATURE_CLASS_ID != 'null' THEN
    v_query1 := 'SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM ' || v_table1 || ' WHERE TO_FEATURE_GLOBALID = ' || chr(39) || P_TO_FEATURE_GLOBALID ||  chr(39) || ' AND TO_FEATURE_FCID = ' || P_FEATURE_CLASS_ID || ' AND FEEDERID = '|| chr(39) || P_FEEDERID || chr(39);
    --OPEN c_query FOR SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_A WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND TO_FEATURE_FCID = P_FEATURE_CLASS_ID AND FEEDERID = P_FEEDERID;
  ELSIF P_TO_FEATURE_GLOBALID != 'null' AND P_FEEDERID != 'null' AND P_FEATURE_CLASS_ID = 'null' THEN    
    v_query1 := 'SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM ' || v_table1 || ' WHERE TO_FEATURE_GLOBALID = ' || chr(39) || P_TO_FEATURE_GLOBALID ||  chr(39) || ' AND FEEDERID = '|| chr(39) || P_FEEDERID || chr(39);
    --OPEN c_query FOR SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_A WHERE TO_FEATURE_GLOBALID = P_TO_FEATURE_GLOBALID AND FEEDERID = P_FEEDERID;
  ELSIF P_TO_FEATURE_GLOBALID = 'null' AND P_FEEDERID != 'null' AND P_FEATURE_CLASS_ID = 'null' THEN
    v_query1 := 'SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM ' || v_table1 || ' WHERE FROM_FEATURE_EID = -1 AND FEEDERID = '|| chr(39) || P_FEEDERID || chr(39);
    --OPEN c_query FOR SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_A WHERE FROM_FEATURE_EID = -1 AND FEEDERID = P_FEEDERID;  
  END IF;
  OPEN c_query FOR v_query1;

  LOOP
    FETCH c_query INTO v_order_number, v_min_branch, v_max_branch, v_tree_level, v_to_feature_feederinfo, v_feederid, v_feederfedby;
    EXIT WHEN c_query%NOTFOUND;
    counter := counter + 1;
    IF P_FEATURE_CLASS_ID2 = 0 THEN
      v_query2 := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where (a.FEEDERID = ''' || v_feederid || ''') AND a.MIN_BRANCH >= ' || v_min_branch || ' AND a.MAX_BRANCH <= ' || v_max_branch || ' AND a.TREELEVEL >= ' || v_tree_level || ' AND a.ORDER_NUM <= ' || v_order_number || ' AND (TO_FEATURE_FCID IN (1001,1014,' || P_FEATURE_CLASS_ID || ') OR (TO_FEATURE_FCID IN (1021,1023) AND TO_CIRCUITID IS NOT NULL))';
    ELSE
      v_query2 := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where (a.FEEDERID = ''' || v_feederid || ''') AND a.MIN_BRANCH >= ' || v_min_branch || ' AND a.MAX_BRANCH <= ' || v_max_branch || ' AND a.TREELEVEL >= ' || v_tree_level || ' AND a.ORDER_NUM <= ' || v_order_number || ' AND (TO_FEATURE_FCID IN (1001,1014,' || P_FEATURE_CLASS_ID || ',' || P_FEATURE_CLASS_ID2 || ') OR (TO_FEATURE_FCID IN (1021,1023) AND TO_CIRCUITID IS NOT NULL))';
    END IF;
    IF counter > 1 THEN
      v_final_query := CONCAT(v_final_query, CONCAT(' union ',v_query2));
    ELSE
      v_final_query := v_query2;
    END IF;
    status_flag := 1;
  END LOOP;
  IF status_flag = 1 THEN
    v_final_query := CONCAT(v_final_query, 'order by order_num desc');
  ELSE
    v_final_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where TO_FEATURE_FCID IN (0)';
  END IF;  
  --dbms_output.put_line(v_final_query);
  
  open P_TRACING_CURSOR for v_final_query; 
      
END;
