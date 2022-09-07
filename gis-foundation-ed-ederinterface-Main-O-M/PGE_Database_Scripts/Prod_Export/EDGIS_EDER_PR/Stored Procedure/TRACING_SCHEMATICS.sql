--------------------------------------------------------
--  DDL for Procedure TRACING_SCHEMATICS
--------------------------------------------------------
set define off;

  CREATE OR REPLACE EDITIONABLE PROCEDURE "EDGIS"."TRACING_SCHEMATICS" (P_TO_FEATURE_GLOBALID IN VARCHAR2, P_FEEDERID IN VARCHAR2, P_FEATURE_CLASS_ID IN VARCHAR2, P_TRACING_TYPE IN VARCHAR2, P_TRACING_ON IN VARCHAR2, P_TRACE_FEEDER_FED_FEEDERS IN BOOLEAN, P_TRACE_A_PHASE IN BOOLEAN, P_TRACE_B_PHASE IN BOOLEAN, P_TRACE_C_PHASE IN BOOLEAN, P_TRACING_CURSOR OUT SYS_REFCURSOR)
AS
  -- Declaring query variables for the tracing result
  v_order_number edgis.pge_feederfednetwork_trace_vw.order_num%TYPE;
  v_to_feature_fcid edgis.pge_feederfednetwork_trace_vw.to_feature_schem_fcid%TYPE;
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
  
  -- Declaring query variables for the extra tracing result
  v_order_number_2 edgis.pge_feederfednetwork_trace_vw.order_num%TYPE;
  v_to_feature_fcid_2 edgis.pge_feederfednetwork_trace_vw.to_feature_schem_fcid%TYPE;
  v_to_feature_feederinfo_2 edgis.pge_feederfednetwork_trace_vw.to_feature_feederinfo%TYPE;
  v_physicalname_2 sde.GDB_ITEMS.physicalname%TYPE;
  v_to_feature_oid_2 edgis.pge_feederfednetwork_trace_vw.to_feature_oid%TYPE; 
  v_to_feature_globalid_2 edgis.pge_feederfednetwork_trace_vw.to_feature_globalid%TYPE; 
  v_min_branch_2 edgis.pge_feederfednetwork_trace_vw.min_branch%TYPE; 
  v_max_branch_2 edgis.pge_feederfednetwork_trace_vw.max_branch%TYPE; 
  v_tree_level_2 edgis.pge_feederfednetwork_trace_vw.treelevel%TYPE; 
  v_to_circuitid_2 edgis.pge_feederfednetwork_trace_vw.to_circuitid%TYPE;
  v_feederfedby_2 edgis.pge_feederfednetwork_trace_vw.feederfedby%TYPE; 
  v_to_line_globalid_2 edgis.pge_feederfednetwork_trace_vw.to_line_globalid%TYPE;
  v_feederid_2 edgis.pge_feederfednetwork_trace_vw.feederid%TYPE;
  v_to_point_fc_guid_2 edgis.pge_feederfednetwork_trace_vw.to_point_fc_guid%TYPE;
  v_from_circuitid_2 edgis.pge_feederfednetwork_trace_vw.from_circuitid%TYPE;
  v_from_line_globalid_2 edgis.pge_feederfednetwork_trace_vw.from_line_globalid%TYPE; 
  v_from_point_fc_guid_2 edgis.pge_feederfednetwork_trace_vw.from_point_fc_guid%TYPE;
  
  -- Declaring conditional variables
  v_min_branch2 number:= -1;
  v_max_branch2 number:= -1;
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
  v_sub_query VARCHAR2(25000); 
  v_query1 VARCHAR2(25000); 
  v_query VARCHAR2(25000); 
  v_query2 VARCHAR2(25000); 
  v_final_query VARCHAR2(25000); 
  v_query_final CLOB; 
  v_aphase BINARY_INTEGER;
  v_bphase BINARY_INTEGER;
  v_cphase BINARY_INTEGER;
  v_feederinfo BINARY_INTEGER;
  v_execute BOOLEAN := True;
  v_counter number := 0;
  v_counter2 number := 0;
  v_count number := 0;
  v_flag number := 0;
  v_subsurface_structure_fcid VARCHAR2(10) := '1017';
  -- Declaring array for global id 
  TYPE array_type IS TABLE OF VARCHAR(20000);
  globalids array_type := array_type();
  -- Declaring cursors
  v_cursor1 SYS_REFCURSOR;
  v_cursor SYS_REFCURSOR;
  v_cursor2 SYS_REFCURSOR;
  v_add_cursor SYS_REFCURSOR;
  -- Declaring variable for tablename
  v_table1 VARCHAR2(100);
  v_table2 VARCHAR2(100);
  
  NULL_STRING EXCEPTION;
  PRAGMA
  EXCEPTION_INIT(NULL_STRING, -06535);
  
BEGIN
  dbms_output.put_line('START');
  SELECT tracetablename INTO v_table1 FROM trace_cache_config WHERE tracetype = 'ELECTRIC';
  SELECT tracetablename INTO v_table2 FROM trace_cache_config WHERE tracetype = 'CONDUIT';
  v_query1 := 'SELECT ORDER_NUM, MIN_BRANCH, MAX_BRANCH, TREELEVEL, TO_FEATURE_FEEDERINFO, FEEDERID, FEEDERFEDBY FROM ' || v_table1 || ' WHERE TO_FEATURE_GLOBALID = ' || chr(39) || P_TO_FEATURE_GLOBALID || chr(39) || ' AND TO_FEATURE_SCHEM_FCID = ' || P_FEATURE_CLASS_ID || ' AND FEEDERID = '|| chr(39) || P_FEEDERID || chr(39);
  dbms_output.put_line(v_query1);
  OPEN v_cursor1 FOR v_query1;
  LOOP
    FETCH v_cursor1 INTO v_order_number, v_min_branch, v_max_branch, v_tree_level, v_to_feature_feederinfo, v_feederid, v_feederfedby;
    EXIT WHEN v_cursor1%NOTFOUND;
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
    
    dbms_output.put_line('Inside Fist Loop');
    IF v_to_feature_feederinfo = null THEN
      v_feederinfo := 7;
    ELSE
      v_feederinfo := v_to_feature_feederinfo;
    END IF;
    v_cphase := BitAnd(v_feederinfo,1);
    v_bphase := BitAnd(v_feederinfo,2);
    v_aphase := BitAnd(v_feederinfo,4);
    IF P_TRACE_A_PHASE AND P_TRACE_B_PHASE AND P_TRACE_C_PHASE THEN
      v_execute := True;
--      dbms_output.put_line('v_execute is True');
    ELSIF (NOT((v_aphase = 4 AND P_TRACE_A_PHASE) OR (v_bphase = 2 AND P_TRACE_B_PHASE) OR (v_cphase = 1 AND P_TRACE_C_PHASE))) THEN
      v_execute := False;
--      dbms_output.put_line('v_execute is False');
    END IF;
    IF (v_execute) THEN
--      dbms_output.put_line('v_execute when True');
      v_counter := v_counter + 1;
      IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
        IF P_TRACE_FEEDER_FED_FEEDERS = TRUE THEN
          v_sub_query := '(a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH >= ' || v_min_branch2 || ' AND a.MAX_BRANCH <= ' || v_max_branch2 || ' AND a.TREELEVEL >= ' || v_min_treelevel || ' AND a.ORDER_NUM <= ' || v_max_order_num;
        ELSE
          v_sub_query := '(a.FEEDERID = ''' || v_feederid || ''') AND a.MIN_BRANCH >= ' || v_min_branch2 || ' AND a.MAX_BRANCH <= ' || v_max_branch2 || ' AND a.TREELEVEL >= ' || v_min_treelevel || ' AND a.ORDER_NUM <= ' || v_max_order_num;
        END IF;
        IF UPPER(P_TRACING_ON) = 'DEVICE' THEN
          v_query := 'select a.ORDER_NUM,a.TO_FEATURE_SCHEM_FCID as TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.TO_FEATURE_SCHEM_FCID = b.objectid where ' || v_sub_query;
        ELSIF UPPER(P_TRACING_ON) = 'PROTECTIVEDEVICE' THEN
          v_query := 'select a.ORDER_NUM,a.TO_FEATURE_SCHEM_FCID as TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.TO_FEATURE_SCHEM_FCID = b.objectid where ' || v_sub_query || ' AND TO_FEATURE_SCHEM_FCID IN (6750,6753,6744,6738)';
        ELSE
          v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID, b.physicalname,a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from ' || v_table2 || ' a LEFT JOIN sde.GDB_ITEMS b ON a.TO_FEATURE_FCID = b.objectid where ' || v_sub_query || ' AND SUBTYPECD IN (3)';
        END IF;
      ELSE
        IF length(v_feederfedby) != 0 AND P_TRACE_FEEDER_FED_FEEDERS = True THEN
--          dbms_output.put_line('P_TRACE_FEEDER_FED_FEEDERS is True');
          v_sub_query := '(a.FEEDERID = ''' || v_feederid || ''' OR a.FEEDERFEDBY = ''' || v_feederfedby || ''') AND a.MIN_BRANCH <= ' || v_min_branch2 || ' AND a.MAX_BRANCH >= ' || v_max_branch2 || ' AND a.TREELEVEL <= ' || v_max_treelevel || ' AND a.ORDER_NUM >= ' || v_min_order_num;
        ELSE
--          dbms_output.put_line('P_TRACE_FEEDER_FED_FEEDERS is False');
          v_sub_query := '(a.FEEDERID = ''' || v_feederid || ''') AND a.MIN_BRANCH <= ' || v_min_branch2 || ' AND a.MAX_BRANCH >= ' || v_max_branch2 || ' AND a.TREELEVEL <= ' || v_max_treelevel || ' AND a.ORDER_NUM >= ' || v_min_order_num;
        END IF;
        IF UPPER(P_TRACING_ON) = 'DEVICE' THEN
          v_query := 'select a.ORDER_NUM,a.TO_FEATURE_SCHEM_FCID as TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FROM_CIRCUITID,a.FEEDERFEDBY,a.FROM_LINE_GLOBALID,a.FEEDERID,a.FROM_POINT_FC_GUID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.TO_FEATURE_SCHEM_FCID = b.objectid  where ' || v_sub_query;
        ELSIF UPPER(P_TRACING_ON) = 'PROTECTIVEDEVICE' THEN
          v_query := 'select a.ORDER_NUM,a.TO_FEATURE_SCHEM_FCID as TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FROM_CIRCUITID,a.FEEDERFEDBY,a.FROM_LINE_GLOBALID,a.FEEDERID,a.FROM_POINT_FC_GUID from ' || v_table1 || ' a left join sde.GDB_ITEMS b on a.TO_FEATURE_SCHEM_FCID = b.objectid  where ' || v_sub_query || ' AND TO_FEATURE_SCHEM_FCID IN (6750,6753,6744,6738)';
        ELSE
          v_query := 'select a.ORDER_NUM,a.TO_FEATURE_FCID,b.physicalname,a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FEEDERFEDBY,a.FEEDERID from ' || v_table2 || ' a LEFT JOIN sde.GDB_ITEMS b ON a.TO_FEATURE_FCID = b.objectid where ' || v_sub_query || ' AND SUBTYPECD IN (3)';
--          dbms_output.put_line(v_query);
        END IF;
      END IF;
      -- if more than 1 record for the first query
      IF v_counter > 1 THEN
        v_final_query := CONCAT(v_final_query, CONCAT(' union ',v_query));
      ELSE
        v_final_query := v_query;
      END IF;
      v_flag := 1;
    END IF;
  END LOOP;
  dbms_output.put_line('End of first loop');
  IF v_flag = 1 THEN
    IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
      v_final_query := CONCAT(v_final_query, ' order by order_num desc');
    ELSE
      v_final_query := CONCAT(v_final_query, ' order by order_num asc');
    END IF;
  END IF;
  OPEN v_cursor2 FOR v_final_query;
  IF UPPER(P_TRACING_ON) = 'CONDUIT' THEN
    FETCH v_cursor2 INTO v_order_number, v_to_feature_fcid, v_physicalname, v_to_feature_oid, v_to_feature_globalid, v_min_branch, v_max_branch, v_tree_level, v_feederfedby, v_feederid;  
  ELSE    
    FETCH v_cursor2 INTO v_order_number, v_to_feature_fcid, v_to_feature_feederinfo, v_physicalname, v_to_feature_oid, v_to_feature_globalid, v_min_branch, v_max_branch, v_tree_level, v_to_circuitid, v_feederfedby, v_to_line_globalid, v_feederid, v_to_point_fc_guid;
  END IF;
  IF v_cursor2%ROWCOUNT = 0 THEN
    v_flag := 0;
    dbms_output.put_line('Empty tracing result');
  ELSE    
    v_flag := 1;
    dbms_output.put_line(v_flag);
  END IF;
  dbms_output.put_line(v_final_query);
  IF UPPER(P_TRACING_ON) = 'CONDUIT' OR v_flag = 0 THEN
    OPEN P_TRACING_CURSOR FOR v_final_query;
  END IF;
  IF  UPPER(P_TRACING_ON) != 'CONDUIT' AND v_flag = 1 THEN
    OPEN v_cursor FOR v_final_query;
    v_counter := 0;
    v_counter2 := 0;
    IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
      v_query_final := 'SELECT a.ORDER_NUM,a.TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.TO_FEATURE_SCHEM_FCID = b.objectid WHERE to_feature_globalid =' || chr(39)|| 'Just-for- column-header' || chr(39);
    ELSE
      v_query_final := 'SELECT a.ORDER_NUM,a.TO_FEATURE_FCID, a.TO_FEATURE_FEEDERINFO, b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.FROM_CIRCUITID,a.FEEDERFEDBY,a.FROM_LINE_GLOBALID,a.FEEDERID,a.FROM_POINT_FC_GUID from edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.TO_FEATURE_SCHEM_FCID = b.objectid WHERE to_feature_globalid =' || chr(39)|| 'Just-for- column-header' || chr(39);
    END IF;
    dbms_output.put_line(v_query_final);
    LOOP
      IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
        FETCH v_cursor INTO v_order_number, v_to_feature_fcid, v_to_feature_feederinfo, v_physicalname, v_to_feature_oid, v_to_feature_globalid, v_min_branch, v_max_branch, v_tree_level, v_to_circuitid, v_feederfedby, v_to_line_globalid, v_feederid, v_to_point_fc_guid;
--        dbms_output.put_line('Downstream');
      ELSIF UPPER(P_TRACING_TYPE) = 'UPSTREAM' THEN
        FETCH v_cursor INTO v_order_number, v_to_feature_fcid, v_to_feature_feederinfo, v_physicalname, v_to_feature_oid, v_to_feature_globalid, v_min_branch, v_max_branch, v_tree_level, v_from_circuitid, v_feederfedby, v_from_line_globalid, v_feederid, v_from_point_fc_guid;
--        dbms_output.put_line('Upstream');
      END IF;
      EXIT WHEN v_cursor%NOTFOUND;
      v_order_num := v_order_number;
      IF v_counter2 = 1 THEN EXIT; END IF;
      
      IF v_to_feature_fcid = null THEN CONTINUE; END IF;
      v_fcid := v_to_feature_fcid; 
      IF (UPPER(P_TRACING_ON) = 'CONDUIT' AND v_fcid != v_subsurface_structure_fcid) THEN CONTINUE; END IF;
      IF v_to_feature_globalid = null THEN CONTINUE; END IF;
      IF v_to_feature_globalid member of globalids THEN  CONTINUE; END IF;
      v_counter := globalids.COUNT + 1;
      globalids.EXTEND;
      globalids(v_counter) := v_to_feature_globalid;
--      dbms_output.put_line(v_counter2);
      IF v_to_feature_feederinfo = null THEN 
        v_feederinfo := 7;
      ELSE
        v_feederinfo := v_to_feature_feederinfo;
      END IF;
      v_cphase := BitAnd(v_feederinfo,1);
      v_bphase := BitAnd(v_feederinfo,2);
      v_aphase := BitAnd(v_feederinfo,4);
      IF P_TRACE_A_PHASE AND P_TRACE_B_PHASE AND P_TRACE_C_PHASE THEN
--        dbms_output.put_line('Execute is True');
        v_execute := True;
      ELSIF (NOT((v_aphase = 4 AND P_TRACE_A_PHASE) OR (v_bphase = 2 AND P_TRACE_B_PHASE) OR (v_cphase = 1 AND P_TRACE_C_PHASE))) THEN
--        dbms_output.put_line('Execute is False');
        v_execute := False;
      END IF;
      IF (v_execute) THEN
--        dbms_output.put_line('Execute is True');
        IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
          v_query_final := CONCAT(v_query_final,' UNION ALL SELECT ' || v_order_number || ', ' || v_to_feature_fcid || ', CAST('|| chr(39) || v_to_feature_feederinfo ||chr(39) ||' as number), CAST('|| chr(39) || v_physicalname || chr(39) ||' as nvarchar2(100)), ' || v_to_feature_oid || ', CAST('|| chr(39) || v_to_feature_globalid || chr(39) ||' as varchar2(100)), ' || v_min_branch || ', ' || v_max_branch || ', ' || v_tree_level || ', '|| chr(39) || v_to_circuitid || chr(39) ||', CAST('|| chr(39) || v_feederfedby || chr(39) ||' as nvarchar2(100)), '|| chr(39) || v_to_line_globalid || chr(39) ||', CAST('|| chr(39) || v_feederid || chr(39) ||' as nvarchar2(100)), ' || chr(39) || v_to_point_fc_guid || chr(39) ||' from dual');
          v_count := v_count + 1;
        ELSE
          v_query_final := CONCAT(v_query_final,' UNION ALL SELECT ' || v_order_number || ', ' || v_to_feature_fcid || ', CAST('|| chr(39) || v_to_feature_feederinfo ||chr(39) ||' as number), CAST('|| chr(39) || v_physicalname || chr(39) ||' as nvarchar2(100)), ' || v_to_feature_oid || ', CAST('|| chr(39) || v_to_feature_globalid || chr(39) ||' as varchar2(100)), ' || v_min_branch || ', ' || v_max_branch || ', ' || v_tree_level || ', '|| chr(39) || v_from_circuitid || chr(39) ||', CAST('|| chr(39) || v_feederfedby || chr(39) ||' as nvarchar2(100)), '|| chr(39) || v_from_line_globalid || chr(39) ||', CAST('|| chr(39) || v_feederid || chr(39) ||' as nvarchar2(100)), ' || chr(39) || v_from_point_fc_guid || chr(39) ||' from dual');
          v_count := v_count + 1;
        END IF;
        v_counter2 := v_counter2 + 1;
        -- If there exists any connected feeder will add those data too.
        IF (P_TRACE_FEEDER_FED_FEEDERS) THEN
          IF v_from_line_globalid != null OR v_to_line_globalid != null THEN
            dbms_output.put_line('Add connected feeder data');
            IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
               v_query2 := 'SELECT a.ORDER_NUM,a.TO_FEATURE_SCHEM_FCID,a.TO_FEATURE_FEEDERINFO,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a LEFT JOIN sde.GDB_ITEMS b ON a.TO_FEATURE_SCHEM_FCID = b.objectid WHERE TO_FEATURE_GLOBALID = v_to_point_fc_guid AND FEEDERID = v_to_circuitid';
            ELSE
               v_query2 := 'SELECT a.ORDER_NUM,a.TO_FEATURE_SCHEM_FCID,a.TO_FEATURE_FEEDERINFO,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID FROM edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a LEFT JOIN sde.GDB_ITEMS b ON a.TO_FEATURE_SCHEM_FCID = b.objectid WHERE TO_FEATURE_GLOBALID = v_from_line_globalid AND FEEDERID = v_from_circuitid';        
            END IF;
            OPEN v_add_cursor FOR v_query2;
            LOOP
              IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
                FETCH v_add_cursor INTO v_order_number_2, v_to_feature_fcid_2, v_to_feature_feederinfo_2, v_physicalname_2, v_to_feature_oid_2, v_to_feature_globalid_2, v_min_branch_2, v_max_branch_2, v_tree_level_2, v_to_circuitid_2, v_feederfedby_2, v_to_line_globalid_2, v_feederid_2, v_to_point_fc_guid_2;
              ELSIF UPPER(P_TRACING_TYPE) = 'UPSTREAM' THEN
                FETCH v_add_cursor INTO v_order_number_2, v_to_feature_fcid_2, v_to_feature_feederinfo_2, v_physicalname_2, v_to_feature_oid_2, v_to_feature_globalid_2, v_min_branch_2, v_max_branch_2, v_tree_level_2, v_from_circuitid_2, v_feederfedby_2, v_from_line_globalid_2, v_feederid_2, v_from_point_fc_guid_2;
              END IF;
              EXIT WHEN v_add_cursor%NOTFOUND;
              IF v_to_feature_globalid member of globalids THEN  CONTINUE; END IF;
              dbms_output.put_line('Add connected feeder global id');
              v_counter := globalids.COUNT + 1;
              globalids.EXTEND;
              globalids(v_counter) := v_to_feature_globalid_2;
              IF UPPER(P_TRACING_TYPE) = 'DOWNSTREAM' THEN
                v_query_final := CONCAT(v_query_final,' UNION ALL SELECT ' || v_order_number_2 || ', ' || v_to_feature_fcid_2 || ', CAST('|| chr(39) || v_to_feature_feederinfo ||chr(39) ||' as number), CAST('|| chr(39) || v_physicalname_2 || chr(39) ||' as nvarchar2(100)), ' || v_to_feature_oid_2 || ', CAST('|| chr(39) || v_to_feature_globalid_2|| chr(39) ||' as varchar2(100)), ' || v_min_branch_2 || ', ' || v_max_branch_2 || ', ' || v_tree_level_2 || ', '|| chr(39) || v_to_circuitid_2 || chr(39) ||', CAST('|| chr(39) || v_feederfedby_2 || chr(39) ||' as nvarchar2(100)), '|| chr(39) || v_to_line_globalid_2 || chr(39) ||', CAST('|| chr(39) || v_feederid_2 || chr(39) ||' as nvarchar2(100)), ' || chr(39) || v_to_point_fc_guid_2 || chr(39) ||' from dual');
              ELSE
                v_query_final := CONCAT(v_query_final,' UNION ALL SELECT ' || v_order_number_2 || ', ' || v_to_feature_fcid_2 || ', CAST('|| chr(39) || v_to_feature_feederinfo ||chr(39) ||' as number), CAST('|| chr(39) || v_physicalname_2 || chr(39) ||' as nvarchar2(100)), ' || v_to_feature_oid_2 || ', CAST('|| chr(39) || v_to_feature_globalid_2 || chr(39) ||' as varchar2(100)), ' || v_min_branch_2 || ', ' || v_max_branch_2 || ', ' || v_tree_level_2 || ', '|| chr(39) || v_from_circuitid_2 || chr(39) ||', CAST('|| chr(39) || v_feederfedby_2 || chr(39) ||' as nvarchar2(100)), '|| chr(39) || v_from_line_globalid_2 || chr(39) ||', CAST('|| chr(39) || v_feederid_2 || chr(39) ||' as nvarchar2(100)), ' || chr(39) || v_from_point_fc_guid_2 || chr(39) ||' from dual');
              END IF;
            END LOOP;
          END IF;
        END IF;
      ELSE
        dbms_output.put_line('Execute is False');
      END IF;
--      dbms_output.put_line(v_counter2);
    END LOOP;
    dbms_output.put_line(v_count);
    dbms_output.put_line(v_counter2);
--    dbms_output.put_line(v_query_final);
--    OPEN P_TRACING_CURSOR FOR v_query_final;
    OPEN P_TRACING_CURSOR FOR v_final_query;
  END IF;     
  EXCEPTION
  WHEN NULL_STRING THEN
    v_final_query := 'select a.ORDER_NUM,a.TO_FEATURE_SCHEM_FCID,a.TO_FEATURE_FEEDERINFO,b.physicalname, a.TO_FEATURE_OID,a.TO_FEATURE_GLOBALID,a.MIN_BRANCH,a.MAX_BRANCH,a.TREELEVEL,a.TO_CIRCUITID,a.FEEDERFEDBY,a.TO_LINE_GLOBALID,a.FEEDERID,a.TO_POINT_FC_GUID from edgis.PGE_FEEDERFEDNETWORK_TRACE_VW a left join sde.GDB_ITEMS b on a.TO_FEATURE_SCHEM_FCID = b.objectid where TO_FEATURE_SCHEM_FCID IN (0)';
        
END;
