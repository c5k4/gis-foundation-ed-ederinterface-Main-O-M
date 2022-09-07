--------------------------------------------------------
--  DDL for Procedure TRACING_ISOLATION
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRACING_ISOLATION" (P_TO_FEATURE_GLOBALID IN VARCHAR2, P_FEEDERID IN VARCHAR2, P_FEATURE_CLASS_ID IN VARCHAR2, P_TRACING_CURSOR OUT SYS_REFCURSOR)
AS
  -- Declaring query column variables for the tracing result
  v_order_number edgis.pge_feederfednetwork_trace_vw.order_num%TYPE;
  v_to_feature_fcid edgis.pge_feederfednetwork_trace_vw.to_feature_fcid%TYPE;
  v_to_feature_feederinfo edgis.pge_feederfednetwork_trace_vw.to_feature_feederinfo%TYPE;
  v_physicalname sde.GDB_ITEMS.physicalname%TYPE;
  v_to_feature_oid edgis.pge_feederfednetwork_trace_vw.to_feature_oid%TYPE; 
  v_to_feature_globalid edgis.pge_feederfednetwork_trace_vw.to_feature_globalid%TYPE; 
  v_min_branch edgis.pge_feederfednetwork_trace_vw.min_branch%TYPE; 
  v_max_branch edgis.pge_feederfednetwork_trace_vw.max_branch%TYPE; 
  v_tree_level edgis.pge_feederfednetwork_trace_vw.treelevel%TYPE; 
  v_to_circuitid edgis.pge_feederfednetwork_trace_vw.to_circuitid%TYPE;
  v_feederfedby edgis.pge_feederfednetwork_trace_vw.feederfedby%TYPE; 
  v_to_line_globalid edgis.pge_feederfednetwork_trace_vw.to_line_globalid%TYPE;
  v_feederid edgis.pge_feederfednetwork_trace_vw.feederid%TYPE;
  v_to_point_fc_guid edgis.pge_feederfednetwork_trace_vw.to_point_fc_guid%TYPE;
  v_from_circuitid edgis.pge_feederfednetwork_trace_vw.from_circuitid%TYPE;
  v_from_line_globalid edgis.pge_feederfednetwork_trace_vw.from_line_globalid%TYPE; 
  v_from_point_fc_guid edgis.pge_feederfednetwork_trace_vw.from_point_fc_guid%TYPE;  
  -- Declaring conditional variables
  v_min_branch2 number:= -1;
  v_max_branch2 number:= -1;
  v_tree_level2 number;
  v_min_treelevel number:= -1;
  v_max_treelevel number:= -1;
  v_min_order_num number:= -1;
  v_max_order_num number:= -1;
  v_temp_min_branch number:= 0;
  v_temp_max_branch number:= 0;
  v_temp_tree number:= 0;
  v_temp_order number:= 0;
  v_order_num number:= -1;
  v_feeder_info number:= -1;
  v_fcid number:= -1;
  -- GLOBALID string variables
  v_upstream_globalid VARCHAR2(25000) := '';
  v_downstream_globalid VARCHAR2(25000) := '';
  v_downstream_globalid2 VARCHAR2(25000) := '';
  v_downstream_globalid3 VARCHAR2(25000) := '';
  v_final_globalid VARCHAR2(25000) := '';
  -- ORDERNUM string variables
  v_upstream_ordernum VARCHAR2(25000) := '';
  v_downstream_ordernum VARCHAR2(25000) := '';
  v_downstream_ordernum2 VARCHAR2(25000) := '';
  v_downstream_ordernum3 VARCHAR2(25000) := '';
  v_final_ordernum VARCHAR2(25000) := '';
  -- Query variables
  v_sub_query VARCHAR2(25000); 
  v_query1 VARCHAR2(25000); 
  v_query2 VARCHAR2(25000); 
  v_query3 VARCHAR2(25000);
  v_filter_query VARCHAR2(25000);
  v_final_query VARCHAR2(25000);  
  -- Counter variables
  v_counter number := 0;
  v_counter2 number := 0;
  v_counter3 number := 0;
  -- Flag variable
  v_flag number := 0; 
  -- Declaring cursors
  v_cursor1 SYS_REFCURSOR;
  v_cursor2 SYS_REFCURSOR;
  v_cursor3 SYS_REFCURSOR;
  -- Declaring variable for tablename
  v_table1 VARCHAR2(100);
  v_table2 VARCHAR2(100);
    
BEGIN

  dbms_output.put_line('START');
  SELECT tracetablename INTO v_table1 FROM trace_cache_config WHERE tracetype = 'ELECTRIC';
  SELECT tracetablename INTO v_table2 FROM trace_cache_config WHERE tracetype = 'CONDUIT';
  v_query1 := 'SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM ' || v_table1 || ' WHERE TO_FEATURE_GLOBALID = ' || chr(39) || P_TO_FEATURE_GLOBALID || chr(39) || ' AND TO_FEATURE_FCID = ' || P_FEATURE_CLASS_ID || ' AND FEEDERID = '|| chr(39) || P_FEEDERID || chr(39);
  dbms_output.put_line(v_query1);
  OPEN v_cursor1 FOR v_query1;
  FETCH v_cursor1 INTO v_order_number, v_min_branch, v_max_branch, v_tree_level, v_to_feature_feederinfo, v_feederid, v_feederfedby;
  v_tree_level2 :=  v_tree_level;
  v_temp_tree := v_tree_level;
  v_temp_order := v_order_number;
  v_temp_min_branch := v_min_branch;
  v_temp_max_branch := v_max_branch;
  IF (v_min_order_num = -1 OR v_temp_order < v_min_order_num) THEN v_min_order_num := v_temp_order; END IF;
  IF (v_max_order_num = -1 OR v_temp_order > v_max_order_num) THEN v_max_order_num := v_temp_order; END IF;
  IF (v_min_treelevel = -1 OR v_temp_tree < v_min_treelevel) THEN v_min_treelevel := v_temp_tree; END IF;
  IF (v_max_treelevel = -1 OR v_temp_tree > v_max_treelevel) THEN v_max_treelevel := v_temp_tree; END IF;
  IF (v_min_branch2 = -1 OR v_temp_min_branch < v_min_branch2) THEN v_min_branch2 := v_temp_min_branch; END IF;
  IF (v_max_branch2 = -1 OR v_temp_max_branch > v_max_branch2) THEN v_max_branch2 := v_temp_max_branch; END IF;
  -- adding flagged feature to the result list
  v_query1 := 'select a.ORDER_NUM,a.TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid  where a.to_feature_globalid = ' || chr(39) || P_TO_FEATURE_GLOBALID || chr(39);
  -- Upstream trace query
  v_sub_query := '(a.FEEDERID = ''' || v_feederid || ''') AND a.MIN_BRANCH <= ' || v_min_branch2 || ' AND a.MAX_BRANCH >= ' || v_max_branch2 || ' AND a.TREELEVEL <= ' || v_max_treelevel || ' AND a.ORDER_NUM >= ' || v_min_order_num;
  v_query2 := 'select a.ORDER_NUM,a.TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid  where ' || v_sub_query || ' AND TO_FEATURE_FCID IN (997, 998,1005,1003)';
  -- Downstream trace query
  v_sub_query := '(a.FEEDERID = ''' || v_feederid || ''') AND a.MIN_BRANCH >= ' || v_min_branch2 || ' AND a.MAX_BRANCH <= ' || v_max_branch2 || ' AND a.TREELEVEL >= ' || v_min_treelevel || ' AND a.ORDER_NUM <= ' || v_max_order_num;
  v_query3 := 'select a.ORDER_NUM,a.TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where ' || v_sub_query || ' AND TO_FEATURE_FCID IN (997, 998,1005,1003)';
  -- final union query
  v_final_query := v_query1 || ' union ' || v_query2 || ' union ' || v_query3 || ' order by order_num asc';
  dbms_output.put_line(v_final_query);  
  OPEN v_cursor2 FOR v_final_query;
  LOOP
    FETCH v_cursor2 INTO v_order_number, v_to_feature_fcid, v_to_feature_feederinfo, v_physicalname, v_to_feature_oid, v_to_feature_globalid, v_min_branch, v_max_branch, v_tree_level, v_feederfedby, v_feederid;
    EXIT WHEN v_cursor2%NOTFOUND;
    IF v_to_feature_globalid = P_TO_FEATURE_GLOBALID THEN
      dbms_output.put_line('Match');
      v_flag := 1;
      CONTINUE;
    ELSIF v_flag = 1 THEN
      v_upstream_ordernum := chr(39) || v_order_number || chr(39);
      v_upstream_globalid := chr(39) || v_to_feature_globalid || chr(39);      
      EXIT;
    ELSIF v_min_branch = v_min_branch2 THEN
      IF v_counter = 0 THEN
        v_downstream_ordernum := chr(39) || v_order_number || chr(39);
        v_downstream_globalid := chr(39) || v_to_feature_globalid || chr(39);         
      ELSE
        v_downstream_ordernum := v_downstream_ordernum || ',' || chr(39) || v_order_number || chr(39);
        v_downstream_globalid := v_downstream_globalid || ',' || chr(39) || v_to_feature_globalid || chr(39);
      END IF; 
      v_counter := v_counter + 1;
    ELSIF v_min_branch = v_tree_level2 THEN
      IF v_counter2 = 0 THEN
        v_downstream_ordernum2 := chr(39) || v_order_number || chr(39);
        v_downstream_globalid2 := chr(39) || v_to_feature_globalid || chr(39);
      ELSE
        v_downstream_ordernum2 := v_downstream_ordernum2 || ',' || chr(39) || v_order_number || chr(39);
        v_downstream_globalid2 := v_downstream_globalid2 || ',' || chr(39) || v_to_feature_globalid || chr(39);
      END IF; 
      v_counter2 := v_counter2 + 1;
    ELSIF v_max_branch = v_max_branch2 THEN
      IF v_counter3 = 0 THEN
        v_downstream_ordernum3 := chr(39) || v_order_number || chr(39);
        v_downstream_globalid3 := chr(39) || v_to_feature_globalid || chr(39);
      ELSE
        v_downstream_ordernum3 := v_downstream_ordernum3 || ',' || chr(39) || v_order_number || chr(39);
        v_downstream_globalid3 := v_downstream_globalid3 || ',' || chr(39) || v_to_feature_globalid || chr(39);
      END IF; 
      v_counter3 := v_counter3 + 1;
    END IF;
  END LOOP;
  dbms_output.put_line('Conditional downstream globalid');
  dbms_output.put_line(v_downstream_globalid);
  dbms_output.put_line(v_downstream_globalid2);
  dbms_output.put_line(v_downstream_globalid3);
  IF LENGTH(v_upstream_globalid) > 0 THEN
    v_final_globalid := v_upstream_globalid;
    v_final_ordernum := v_upstream_ordernum;
  END IF;
  dbms_output.put_line('Conditional upstream globalid');
  dbms_output.put_line(v_final_globalid);
  -- Same Min Branch - Take Nearest Order_Num Device
  IF LENGTH(v_downstream_globalid) > 0 THEN
    v_filter_query := 'select ORDER_NUM, TO_FEATURE_GLOBALID from (select a.ORDER_NUM,a.TO_FEATURE_GLOBALID, row_number() over (partition by a.TO_FEATURE_GLOBALID order by a.ORDER_NUM desc) rn from EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where a.to_feature_globalid in (' || v_downstream_globalid || ') and a.order_num in (' || v_downstream_ordernum || ') order by order_num desc) r where rn = 1';
    OPEN v_cursor3 FOR v_filter_query;
    FETCH v_cursor3 INTO v_order_num, v_to_feature_globalid;
    v_final_ordernum := v_final_ordernum || ',' || chr(39) || v_order_num || chr(39);
    v_final_globalid := v_final_globalid || ',' || chr(39) || v_to_feature_globalid || chr(39);
  END IF;
  
  -- Min_Branch of Device is equal to Tree Level of Clicked (Line)- Take Nearest Order_Num Device
  IF LENGTH(v_downstream_globalid2) > 0 THEN
    v_filter_query := 'select ORDER_NUM, TO_FEATURE_GLOBALID from (select a.ORDER_NUM,a.TO_FEATURE_GLOBALID, row_number() over (partition by a.TO_FEATURE_GLOBALID order by a.ORDER_NUM desc) rn from EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where a.to_feature_globalid in (' || v_downstream_globalid2 || ') and a.order_num in (' || v_downstream_ordernum2 || ') order by order_num desc) r where rn = 1';
    OPEN v_cursor3 FOR v_filter_query;
    FETCH v_cursor3 INTO v_order_num, v_to_feature_globalid;
    v_final_ordernum := v_final_ordernum || ',' || chr(39) || v_order_num || chr(39);
    v_final_globalid := v_final_globalid || ',' || chr(39) || v_to_feature_globalid || chr(39);
  END IF;
  
  -- Max_Branch of Device is equal to Max_Branch of Clicked (Line)- Take Nearest Order_Num Device
  IF LENGTH(v_downstream_globalid3) > 0 THEN
    v_filter_query := 'select ORDER_NUM, TO_FEATURE_GLOBALID from (select a.ORDER_NUM,a.TO_FEATURE_GLOBALID, row_number() over (partition by a.TO_FEATURE_GLOBALID order by a.ORDER_NUM desc) rn from EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid where a.to_feature_globalid in (' || v_downstream_globalid3 || ') and a.order_num in (' || v_downstream_ordernum3 || ') order by order_num desc) r where rn = 1';
    OPEN v_cursor3 FOR v_filter_query;
    FETCH v_cursor3 INTO v_order_num, v_to_feature_globalid;
    v_final_ordernum := v_final_ordernum || ',' || chr(39) || v_order_num || chr(39);
    v_final_globalid := v_final_globalid || ',' || chr(39) || v_to_feature_globalid || chr(39);
  END IF;

  v_final_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.to_feature_fcid = b.objectid  where a.to_feature_globalid in (' || v_final_globalid || ') and a.order_num in (' || v_final_ordernum || ') order by order_num asc';

  dbms_output.put_line(v_final_query);
  OPEN P_TRACING_CURSOR FOR v_final_query;
  
END;
