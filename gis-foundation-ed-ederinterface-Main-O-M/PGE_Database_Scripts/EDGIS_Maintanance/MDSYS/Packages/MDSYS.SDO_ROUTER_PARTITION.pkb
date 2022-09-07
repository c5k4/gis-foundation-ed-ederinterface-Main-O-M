Prompt drop Package Body SDO_ROUTER_PARTITION;
DROP PACKAGE BODY MDSYS.SDO_ROUTER_PARTITION
/

Prompt Package Body SDO_ROUTER_PARTITION;
--
-- SDO_ROUTER_PARTITION  (Package Body) 
--
CREATE OR REPLACE PACKAGE BODY MDSYS.SDO_ROUTER_PARTITION AS

-- Partitioning log file
  part_log_file   utl_file.file_type := NULL;
  JAVA_ERROR      EXCEPTION;
  PARAMETER_ERROR EXCEPTION;

--
-- log a message to the partitioning log file
--
PROCEDURE log_message(message IN VARCHAR2, show_time IN BOOLEAN DEFAULT TRUE)
IS
BEGIN

  if  ( utl_file.is_open(part_log_file) = FALSE )  then
     return;
  end if;
  IF ( show_time ) THEN
    utl_file.put_line (part_log_file, to_char(sysdate,'Dy fmMon DD HH24:MI:SS YYYY'));
  END IF;
  utl_file.put_line (part_log_file, message);
  utl_file.fflush(part_log_file);

EXCEPTION
  WHEN OTHERS THEN
    raise_application_error(-20002, 'Error Writing Log File');
END log_message;

--
-- check if the given constraint exists
--
FUNCTION constraint_exists(constraint_name IN VARCHAR2)
  RETURN VARCHAR2
IS
  stmt  VARCHAR2(256);
  no    NUMBER := 0;
BEGIN
  stmt := 'SELECT COUNT(*) FROM user_constraints WHERE CONSTRAINT_NAME = :name';
  EXECUTE IMMEDIATE stmt into no using UPPER(constraint_name);
  IF (no = 1) THEN
    RETURN 'TRUE';
  ELSE
    RETURN 'FALSE';
  END IF;
END constraint_exists;

--
-- check if the given index exists
--
FUNCTION index_exists(index_name IN VARCHAR2)
  RETURN VARCHAR2
IS
  stmt  VARCHAR2(256);
  no    NUMBER := 0;
BEGIN
  stmt := 'SELECT COUNT(*) FROM IND WHERE INDEX_NAME = :name';
  EXECUTE IMMEDIATE stmt into no using UPPER(index_name);
  IF (no = 1) THEN
    RETURN 'TRUE';
  ELSE
    RETURN 'FALSE';
  END IF;
END index_exists;

--
-- check if the given network exists
--
FUNCTION network_exists(network_name IN VARCHAR2)
  RETURN VARCHAR2
IS
  md_stmt  VARCHAR2(256);
  ud_stmt  VARCHAR2(256);
  no    NUMBER := 0;
BEGIN
  md_stmt := 'SELECT COUNT(*) FROM USER_SDO_NETWORK_METADATA WHERE NETWORK = :name';
  EXECUTE IMMEDIATE md_stmt into no using UPPER(network_name);
  IF (no = 1) THEN
    ud_stmt := 'SELECT COUNT(*) FROM USER_SDO_NETWORK_USER_DATA WHERE NETWORK = :name';
    EXECUTE IMMEDIATE ud_stmt into no using UPPER(network_name);
        IF (no > 0) THEN
      RETURN 'TRUE';
    END IF;
  END IF;

  RETURN 'FALSE';
END network_exists;

--
-- check if the given table exists
--
FUNCTION table_exists(tab_name IN VARCHAR2)
  RETURN VARCHAR2
IS
  stmt  VARCHAR2(256);
  no    NUMBER := 0;
BEGIN
  stmt := 'SELECT COUNT(*) FROM TAB WHERE TNAME = :name';
  EXECUTE IMMEDIATE stmt into no using UPPER(tab_name);
  IF (no = 1) THEN
    RETURN 'TRUE';
  ELSE
    RETURN 'FALSE';
  END IF;
END table_exists;

--
-- disk based graph partition functions/procedures
-- based on moment of inertia appraoch
--
FUNCTION min_eigenvector(sum_x2 IN NUMBER, sum_y2 IN NUMBER, sum_xy IN NUMBER)
  RETURN mdsys.vector_2d
IS
  lamda     NUMBER := 0;
  tmp_sum   NUMBER := 0;
  k         NUMBER := 0;
  eigenvector_1 NUMBER := 0;
  eigenvector_2 NUMBER := 0;
BEGIN
  tmp_sum := sum_x2 + sum_y2;
  lamda := (tmp_sum -
    sqrt(tmp_sum*tmp_sum -4.0*(sum_x2*sum_y2-sum_xy*sum_xy)))/2;

  IF (sum_xy = 0) THEN
    IF (sum_x2 > sum_y2) THEN
      eigenvector_1 := 0;
      eigenvector_2 := 1.0;
    ELSE
      eigenvector_1 := 1.0;
      eigenvector_2 := 0;
    END IF;
  ELSE
    k := -sum_xy/(sum_x2-lamda);
    eigenvector_2 := 1.0/sqrt(k*k+1.0);
    eigenvector_1 := k*eigenvector_2;
  END IF;

  RETURN mdsys.vector_2d(eigenvector_1,eigenvector_2);

END min_eigenvector;

--
-- Return a negative value if the value falls in the given range (start_m,end_m)
--
FUNCTION adjust_m(start_m IN NUMBER, end_m IN NUMBER, m IN NUMBER)
  RETURN NUMBER
IS
BEGIN
  IF (m >= start_m AND m <= end_m) THEN
    RETURN -m;
  ELSE
    RETURN m;
  END IF;
END adjust_m;

--
-- return the p_id based on the given m value (m > 0) ? pid : (pid+1)
--
FUNCTION get_pid(m IN NUMBER, pid IN NUMBER)
  RETURN NUMBER
IS
BEGIN
  IF ( m < 0 ) THEN
    RETURN pid;
  ELSE
    RETURN pid+1;
  END IF;

END get_pid;

--
--
--
PROCEDURE adjust_final_pid(p_tab_name IN VARCHAR2)
IS
  min_pid NUMBER;
  stmt VARCHAR2(256);
BEGIN

  EXECUTE IMMEDIATE 'truncate table ' ||
      SYS.DBMS_ASSERT.ENQUOTE_NAME(p_tab_name);

  stmt := 'SELECT MIN(P_ID) FROM final_partition';
  EXECUTE IMMEDIATE stmt into min_pid ;

  stmt := 'INSERT /*+ APPEND */ into ' ||
    SYS.DBMS_ASSERT.ENQUOTE_NAME(p_tab_name) || ' (vertex_id,p_id,x,y) ' ||
    ' select vertex_id, (p_id-:min_pid+1),x,y from final_partition';
  EXECUTE IMMEDIATE stmt USING min_pid;

  COMMIT;
END adjust_final_pid;

--
-- move vertices along the principal axes to make two partition with equal size
--
PROCEDURE make_partition_equal(tab_name IN VARCHAR2,
                                pid IN NUMBER,
                                v_no IN NUMBER,
                                part_counter IN NUMBER)
IS
  no          INTEGER;
  vno1        NUMBER;
  vno2        NUMBER;
  stmt        VARCHAR2(256);
  part_m      NUMBER;
BEGIN
  part_m := 0;

  -- two partitions for partition pid is based on the m sign (<0 or >= 0 )
  stmt := 'SELECT COUNT(*) FROM ' ||
    SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(tab_name) || ' WHERE m < 0 ';
  EXECUTE IMMEDIATE stmt into vno1;

  vno2 := v_no - vno1;

  IF (vno1 > vno2) THEN
    -- move n vertices from set (m < 0) to set (m>= 0)
    -- by flipping its sign of m
    no := (vno1-vno2)/2;
    stmt := 'SELECT min(m) FROM (SELECT m FROM ' ||
      SYS.DBMS_ASSERT.ENQUOTE_NAME(tab_name) ||
        ' WHERE m < 0 ORDER BY m DESC)  ' || ' WHERE rownum <= :no ';
    EXECUTE IMMEDIATE stmt into part_m USING no;

    INSERT /*+ APPEND */ INTO partition_tmp_3 (vertex_id,p_id,x,y,m)
      SELECT vertex_id,
          mdsys.SDO_ROUTER_PARTITION.get_pid(
            mdsys.SDO_ROUTER_PARTITION.adjust_m(part_m,0,m),part_counter),
          x,y,mdsys.SDO_ROUTER_PARTITION.adjust_m(part_m,0,m)
        FROM partition_tmp_2;
  ELSE
    -- move n vertices from set (m >= 0) to set ( m < 0) by
    -- updating pid in vertex table
    no := (vno2-vno1)/2;
    stmt := 'SELECT max(m) FROM (SELECT m FROM ' ||
      SYS.DBMS_ASSERT.ENQUOTE_NAME(tab_name) ||
        ' WHERE m >= 0 ORDER BY m)  ' || ' WHERE rownum <= :no ' ;
    EXECUTE IMMEDIATE stmt into part_m USING no;

    INSERT /*+ APPEND */ INTO partition_tmp_3 (vertex_id,p_id,x,y,m)
      SELECT vertex_id,
              mdsys.SDO_ROUTER_PARTITION.get_pid(
                mdsys.SDO_ROUTER_PARTITION.adjust_m(0,part_m,m),part_counter),
              x,y,mdsys.SDO_ROUTER_PARTITION.adjust_m(0,part_m,m)
        FROM partition_tmp_2;
  END IF;

  COMMIT;

END make_partition_equal;

--
-- partition procedure
--
PROCEDURE new_partition_proc(p_tab_name IN VARCHAR2,
                              max_v_no IN NUMBER,
                              partition_id IN NUMBER,
                              make_equal IN BOOLEAN,
                              part_counter IN OUT NUMBER)
IS
  m_mean  NUMBER;
  v_no    NUMBER;
  x_mean  NUMBER;
  y_mean  NUMBER;
  eigenvec vector_2d;
  partition_tmp_1 VARCHAR2(256);
  stmt            VARCHAR2(256);
  table_name      VARCHAR2(256);
BEGIN
  --
  -- terminal condition for bisecting
  -- if vertex no. smaller than max_v_no ,stops

  IF (partition_id = 0) THEN
    partition_tmp_1 := SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(p_tab_name);
  ELSE
    partition_tmp_1 := SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(p_tab_name) || '_' || partition_id;
  END IF;

  stmt := 'SELECT COUNT(*) FROM ' || partition_tmp_1;
  EXECUTE IMMEDIATE stmt into  v_no;

  IF (v_no = 0) THEN
    RETURN;
  END IF;

  --
  -- prepare data for eigenvalue/eigenvector calculation
  --

  stmt := 'SELECT AVG(x), AVG(y) from ' || SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(partition_tmp_1);
  EXECUTE IMMEDIATE stmt into x_mean, y_mean;

  stmt := 'SELECT mdsys.SDO_ROUTER_PARTITION.min_eigenvector(
                          sum(power(x-:1,2)), sum(power(y-:2,2)),
                          sum((x-:3)*(y-:4))) FROM ' || partition_tmp_1;
  EXECUTE IMMEDIATE stmt INTO eigenvec USING x_mean, y_mean, x_mean, y_mean;

  stmt := 'SELECT AVG(:1*(x - :2) + :3*(y-:4)) FROM ' || partition_tmp_1;
  EXECUTE IMMEDIATE stmt INTO m_mean USING -eigenvec.y, x_mean, eigenvec.x, y_mean;

  stmt := 'INSERT /*+ APPEND */ into partition_tmp_2 (vertex_id,p_id,x,y,m)
    SELECT vertex_id,p_id,x,y, (:1*(x - :2) + :3*(y-:4) - :5)
      FROM ' || partition_tmp_1;
  EXECUTE IMMEDIATE stmt USING -eigenvec.y, x_mean, eigenvec.x, y_mean, m_mean;

  COMMIT;

  --
  -- make equal size if required
  --

  IF (make_equal) THEN
    make_partition_equal('partition_tmp_2',partition_id,v_no,part_counter);
  ELSE
    INSERT /*+ APPEND */ INTO partition_tmp_3 (vertex_id,p_id,x,y,m)
      SELECT vertex_id,
              mdsys.SDO_ROUTER_PARTITION.get_pid(m, part_counter),
              x,y,m
      FROM partition_tmp_2;

    COMMIT;
  END IF;

  IF (partition_id = 0) THEN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE ' || SYS.DBMS_ASSERT.ENQUOTE_NAME(p_tab_name);
  ELSE
    table_name := SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(p_tab_name) || '_' || partition_id;
    EXECUTE IMMEDIATE 'DROP TABLE ' || table_name;
  END IF;


  IF (v_no/2 > max_v_no) THEN

    table_name := SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(p_tab_name) || '_' || part_counter;
    IF ( table_exists(SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(table_name)) = 'TRUE') THEN
	    execute immediate 'DROP TABLE ' || SYS.DBMS_ASSERT.ENQUOTE_NAME(table_name);
    END IF;

    stmt := 'CREATE  TABLE ' || table_name ||
      ' STORAGE (maxextents unlimited), NOLOGGING as
        SELECT * FROM partition_tmp_3 WHERE  p_id=' || part_counter;
    EXECUTE IMMEDIATE stmt;

    table_name := SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(p_tab_name) || '_' || to_char(part_counter+1);

    IF ( table_exists(SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(table_name)) = 'TRUE') THEN
	    execute immediate 'DROP TABLE ' || SYS.DBMS_ASSERT.ENQUOTE_NAME(table_name);
    END IF;
    stmt := 'CREATE TABLE ' || table_name ||
      ' STORAGE (maxextents unlimited), NOLOGGING as
      SELECT * FROM partition_tmp_3 WHERE p_id=' || to_char(part_counter+1);
    EXECUTE IMMEDIATE stmt;

    COMMIT;
  ELSE
    INSERT /*+ APPEND */ INTO final_partition (vertex_id,p_id,x,y)
     SELECT vertex_id,p_id,x,y FROM partition_tmp_3;

    COMMIT;
  END IF;

  part_counter := part_counter+2;

  EXECUTE IMMEDIATE 'TRUNCATE TABLE partition_tmp_2';
  EXECUTE IMMEDIATE 'TRUNCATE TABLE partition_tmp_3';

END new_partition_proc;

--
-- main pl/sql procedure to partition a graph with coordinate information
--
PROCEDURE graph_partition(p_tab_name IN VARCHAR2,
                          max_v_no IN NUMBER,
                          make_equal IN BOOLEAN)
IS
  p_level   INTEGER;
  stmt      VARCHAR2(256);
  v_no      NUMBER;
  min_pid   NUMBER;
  max_pid   NUMBER;
  pid       NUMBER;
  p_counter NUMBER;
  p_date    date;
  show_time BOOLEAN := FALSE;
BEGIN
  IF (table_exists(p_tab_name) = 'FALSE' ) THEN
    log_message('ERROR: ' || p_tab_name || ' table not found');
    RETURN ;
  END IF;

  stmt := 'SELECT MIN(p_id), MAX(p_id) FROM ' ||
    SYS.DBMS_ASSERT.ENQUOTE_NAME(p_tab_name);
  EXECUTE IMMEDIATE stmt INTO min_pid, max_pid;

  stmt := 'SELECT COUNT(*) FROM ' ||
    SYS.DBMS_ASSERT.ENQUOTE_NAME(p_tab_name) || ' WHERE p_id = :min_pid';
  EXECUTE IMMEDIATE stmt INTO v_no USING  min_pid;

  p_level := floor(LN(v_no/max_v_no)/LN(2.0));


  -- issue warning if the no of nodes in the table is smaller than max_v_no
  if ( p_level < 0 ) THEN
   log_message('WARNING: no. of nodes: ' || v_no || ' in table: ' || p_tab_name ||
	' is smaller than the given max_v_no: ' || max_v_no);
   RETURN;
  end if;


  p_counter := max_pid+1; -- starting partition counter
  pid := min_pid;

  log_message('INFO: begin partitioning of '|| p_tab_name ||
    ' partition level: ' || p_level || ' min(partition id): ' ||
      min_pid || ' max(partition id): ' || max_pid);

  log_message('INFO: generating ' || power(2,p_level+1) ||
	      ' partitions from level:0 to level: ' || p_level ||' ...', show_time);

  FOR k IN min_pid..max_pid LOOP
    FOR i IN 0..p_level LOOP
      p_date := sysdate;
      FOR j IN 1..power(2,i) LOOP
        new_partition_proc(SYS.DBMS_ASSERT.NOOP(p_tab_name), max_v_no, pid, make_equal, p_counter);
        pid := pid +1;
      END LOOP;
      log_message('INFO:    partitioning '|| p_tab_name ||
        ' level: ' || i || ' partition id: ' || pid);
      -- add computation time form each level
      log_message('INFO:    partitioning level: ' || i || ' with ' || power(2,i+1) || ' partitions '|| ' took ' ||
		  to_char((sysdate-p_date)*24*60,'99999.999') || ' min.',show_time) ;
    END LOOP;
  END LOOP;

  -- Copy the result back to original table and ajust the pids
  adjust_final_pid(SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(p_tab_name));

  log_message('INFO: completed partitioning of '|| p_tab_name);

EXCEPTION
  WHEN OTHERS THEN
   log_message('Exception processing partition '|| pid ||
    ' of the ' || p_tab_name || ' table');
   log_message(SQLERRM);
   raise_application_error(-20009, 'Error Graphing Partitions');

END graph_partition;

--
-- drop all temporary tables for partitioning
--
PROCEDURE clean_tables (cleanup IN BOOLEAN DEFAULT TRUE)

IS
BEGIN
  log_message('INFO: cleaning up partitioning temporary tables');

  -- Tables that should always be cleaned up
  --
  IF (table_exists('edge_array_tmp') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE edge_array_tmp';
  END IF;

  IF (table_exists('final_partition') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE final_partition';
  END IF;

  IF (table_exists('partition_tmp_2') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE partition_tmp_2';
  END IF;

  IF (table_exists('partition_tmp_3') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE partition_tmp_3';
  END IF;

  -- Tables we may want to keep for debugging purposes.
  --
  IF (cleanup) THEN
    IF (table_exists('edge_part') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP TABLE edge_part';
    END IF;

    IF (table_exists('node_part') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP TABLE node_part';
    END IF;

    IF (table_exists('restricted_nodes')= 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP TABLE restricted_nodes';
    END IF;

    IF (table_exists('restricted_edges')= 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP TABLE restricted_edges';
    END IF;

    IF (table_exists('super_edge_ids') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP TABLE super_edge_ids';
    END IF;

    IF (table_exists('super_node_ids') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP TABLE super_node_ids';
    END IF;
    IF (table_exists('router_partitioned_truck_data') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP TABLE router_partitioned_truck_data';
    END IF;
  END IF;
END;
--
-- setup all temporary tables for partitioning
--
PROCEDURE setup_tables
IS
BEGIN
  log_message('INFO: setting up partitioning temporary tables');

  IF (table_exists('partition_tmp_2') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE partition_tmp_2';
  ELSE
   EXECUTE IMMEDIATE 'CREATE TABLE partition_tmp_2  (vertex_id NUMBER, p_id NUMBER, x NUMBER, y NUMBER,m NUMBER)
                        STORAGE (maxextents unlimited), NOLOGGING';
  END IF;

  IF (table_exists('partition_tmp_3') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE partition_tmp_3';
  ELSE
   EXECUTE IMMEDIATE 'CREATE TABLE partition_tmp_3  (vertex_id NUMBER, p_id NUMBER, x NUMBER, y NUMBER,m NUMBER)
                        STORAGE (maxextents unlimited), NOLOGGING';
  END IF;

  IF (table_exists('final_partition')= 'TRUE') THEN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE final_partition';
  ELSE
    EXECUTE IMMEDIATE 'CREATE TABLE final_partition  (vertex_id NUMBER, p_id NUMBER, x NUMBER, y NUMBER)
                        STORAGE (maxextents unlimited),  NOLOGGING';
  END IF;

  IF (table_exists('edge_part') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE edge_part';
  END IF;

  IF (table_exists('node_part') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE node_part';
  END IF;

  IF (table_exists('restricted_nodes')= 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE restricted_nodes';
  END IF;

  IF (table_exists('restricted_edges')= 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE restricted_edges';
  END IF;

  IF (table_exists('super_edge_ids') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE super_edge_ids';
  END IF;

  IF (table_exists('super_node_ids') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE super_node_ids';
  END IF;
END setup_tables;


--
-- Create the node_part table needed for partitioning
--
PROCEDURE create_node_part
IS
BEGIN
  IF (table_exists('node_part') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE node_part';
  END IF;

  log_message('INFO: create and load node_part table');

  EXECUTE IMMEDIATE 'CREATE TABLE node_part(vertex_id NUMBER, x NUMBER, y NUMBER, p_id NUMBER, outedges mdsys.num_array, inedges mdsys.num_array)
    STORAGE (maxextents unlimited), NOLOGGING';

  EXECUTE IMMEDIATE 'INSERT /*+ APPEND */ into node_part
    SELECT n.node_id, n.geometry.sdo_point.x, n.geometry.sdo_point.y, 0, null, null
      FROM node n';

  COMMIT;
END create_node_part;

--
-- Create and load the restricted_edges table.
--
PROCEDURE create_restricted_edges
IS
  TYPE CURSOR_TYPE IS REF CURSOR;
  coords    MDSYS.SDO_ORDINATE_ARRAY;
  divider   VARCHAR2(1);
  edge_id   NUMBER;
  ins_stmt  VARCHAR(256);
  p_cursor  CURSOR_TYPE;
BEGIN
  IF (table_exists('restricted_edges') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE restricted_edges';
  END IF;

  log_message('INFO: Create and load the restricted_edges table');

  EXECUTE IMMEDIATE 'CREATE TABLE restricted_edges(edge_id NUMBER,
                                                   divider VARCHAR2(1),
                                                   startx1 NUMBER, starty1 NUMBER,
                                                   startx2 NUMBER, starty2 NUMBER,
                                                   endx1 NUMBER, endy1 NUMBER,
                                                   endx2 NUMBER, endy2 NUMBER)
                       STORAGE (maxextents unlimited), NOLOGGING';

  ins_stmt := 'INSERT INTO restricted_edges VALUES ' ||
    '(:eid, :div, :sx1, :sy1, :sx2, :sy2,:ex1, :ey1, :ex2, :ey2)';

  -- Find all edges attached to nodes that have restricted edges either inbound
  -- or outbound from the node.
  OPEN p_cursor FOR 'SELECT t.edge_id, t.divider, t.geometry.sdo_ordinates' ||
    ' FROM edge t WHERE t.edge_id in ' ||
      '(SELECT edge_id FROM EDGE WHERE start_node_id IN (SELECT node_id FROM restricted_nodes) UNION' ||
      ' SELECT edge_id FROM EDGE WHERE end_node_id IN (SELECT node_id FROM restricted_nodes))';

  LOOP
    FETCH p_cursor INTO edge_id, divider, coords;
    EXIT WHEN p_cursor%NOTFOUND;

    -- Find and store the edges first and last segments
    EXECUTE IMMEDIATE ins_stmt USING
      edge_id, divider,
      coords(1), coords(2),
      coords(3), coords(4),
      coords(coords.count-3), coords(coords.count-2),
      coords(coords.count-1), coords(coords.count);
  END LOOP;
  CLOSE p_cursor;

  COMMIT;

  EXECUTE IMMEDIATE 'CREATE INDEX restricted_edges_idx on restricted_edges(edge_id)';
END create_restricted_edges;

--
-- Create the restricted_nodes table needed for turn restriction generation
--
PROCEDURE create_restricted_nodes
IS
BEGIN
  IF (table_exists('restricted_nodes') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE restricted_nodes';
  END IF;

  log_message('INFO: Create and load the restricted_nodes table');

  EXECUTE IMMEDIATE 'CREATE TABLE restricted_nodes(node_id NUMBER, inedges mdsys.num_array, outedges mdsys.num_array)
     STORAGE (maxextents unlimited), NOLOGGING';

  -- Find all nodes that are the start or end point of a restricted edge.
  EXECUTE IMMEDIATE 'INSERT /*+ APPEND */ INTO restricted_nodes
    SELECT vertex_id, inedges, outedges FROM NODE_PART where vertex_id IN
      (SELECT source_id FROM edge_part WHERE divider IN (''1'', ''A'') union
       SELECT target_id FROM edge_part WHERE divider IN (''2'', ''A''))';

  COMMIT;

END create_restricted_nodes;

PROCEDURE update_node_part_edge_arrays
IS
  TYPE CURSOR_TYPE IS REF CURSOR;
  e_cursor CURSOR_TYPE;
  e_id NUMBER;
  e_array1 mdsys.num_array;
  e_array2 mdsys.num_array;
  n_cursor CURSOR_TYPE;
  n_id NUMBER;
  stmt  VARCHAR2(512);
BEGIN
  -- Create a temporary table to store the results of the array builds
  IF (table_exists('edge_array_tmp') = 'TRUE') THEN
	  EXECUTE IMMEDIATE 'DROP TABLE edge_array_tmp';
  END IF;

  stmt := 'CREATE TABLE edge_array_tmp(vertex_id NUMBER, outedges mdsys.num_array, inedges mdsys.num_array)
             STORAGE (maxextents unlimited), NOLOGGING';

  EXECUTE IMMEDIATE stmt;

  -- For every vertex in the node_part table build an in and out edge array.
  -- The where clause forces a fast index only scan
  OPEN n_cursor FOR 'SELECT vertex_id FROM node_part where vertex_id>0';
  LOOP
    FETCH n_cursor into n_id;
      EXIT WHEN n_cursor%NOTFOUND;

    e_array1 := mdsys.num_array();

    -- Build an array of outedges for a particular node
    OPEN e_cursor for 'select edge_id from edge_part where source_id=:id' using n_id;
    LOOP
      FETCH e_cursor INTO e_id;
        EXIT WHEN e_cursor%NOTFOUND;

        e_array1.extend(1);
        e_array1(e_array1.count) := e_id;
    END LOOP;
    CLOSE e_cursor;

    e_array2 := mdsys.num_array();

    -- Build an array of inedges for a particular node
    OPEN e_cursor for 'select edge_id from edge_part where target_id=:id' using n_id;
    LOOP
      FETCH e_cursor INTO e_id;
        EXIT WHEN e_cursor%NOTFOUND;

        e_array2.extend(1);
        e_array2(e_array2.count) := e_id;
    END LOOP;
    CLOSE e_cursor;

    -- Store the results
    stmt := 'INSERT INTO edge_array_tmp VALUES (:node_id, :outedges, :inedges)';
    EXECUTE IMMEDIATE stmt USING n_id, e_array1, e_array2;
  END LOOP;
  CLOSE n_cursor;

  COMMIT;

  EXECUTE IMMEDIATE 'CREATE INDEX eat_idx on edge_array_tmp(vertex_id)';

  -- Rebuild the node_part table from scratch using the edge array information
  stmt := 'CREATE TABLE new_node_part
            STORAGE (maxextents unlimited), NOLOGGING AS
              SELECT n.vertex_id, n.x, n.y, n.p_id, e.outedges, e.inedges
              FROM node_part n, edge_array_tmp e
              WHERE n.vertex_id=e.vertex_id';
  EXECUTE IMMEDIATE stmt;

  -- Replace the old node_part table with the new one containing
  -- the in and out edge arrays
  --
  EXECUTE IMMEDIATE 'DROP TABLE node_part';
  EXECUTE IMMEDIATE 'RENAME new_node_part to node_part';

  -- Add needed indexes to the node_part table.
  --
  log_message('INFO: create index np_vp_idx on node_part');
  EXECUTE IMMEDIATE 'CREATE INDEX np_vp_idx on node_part(vertex_id, p_id)';

  log_message('INFO: create index node_part_p_idx on node_part');
  EXECUTE IMMEDIATE 'CREATE INDEX node_part_p_idx on node_part(p_id)';

  EXECUTE IMMEDIATE 'DROP TABLE edge_array_tmp';
END update_node_part_edge_arrays;

--
-- Create the node_part table needed for partitioning
--
PROCEDURE create_super_tables
IS
  stmt  VARCHAR2(256);
BEGIN
  -- Create an index on the node_part table
  --
  log_message('INFO: create index np_vp_idx on node_part');
  EXECUTE IMMEDIATE 'CREATE INDEX np_vp_idx on node_part(vertex_id, p_id)';

  -- Drop any constraints on the EDGE and  NODE tables
  IF (constraint_exists('FK_EDGE_START_NODE_ID') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'ALTER TABLE edge DROP CONSTRAINT FK_EDGE_START_NODE_ID';
  END IF;
  IF (constraint_exists('FK_EDGE_END_NODE_ID') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'ALTER TABLE edge DROP CONSTRAINT FK_EDGE_END_NODE_ID';
  END IF;
  IF (constraint_exists('PK_NODE') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'ALTER TABLE node DROP CONSTRAINT PK_NODE';
  ELSE
      EXECUTE IMMEDIATE 'ALTER TABLE node DROP PRIMARY KEY';
  END IF;

  -- Drop and indexes on the NODE table
  IF (index_exists('node_id_index') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP INDEX node_id_index';
  END IF;
  IF (index_exists('node_partition_index') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP INDEX node_partition_index';
  END IF;

  -- Rename the node table so we can use CTAS to rebuild it
  EXECUTE IMMEDIATE 'RENAME node to node_tmp';

  -- Create and populate the edge_part table.
  --
  IF (table_exists('edge_part') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE edge_part';
  END IF;

  log_message('INFO: create and load edge_part');
  EXECUTE IMMEDIATE 'CREATE TABLE edge_part(edge_id NUMBER, source_id NUMBER,
      target_id NUMBER, source_p_id NUMBER, target_p_id NUMBER,
      func_class NUMBER, length NUMBER, speed_limit NUMBER, divider VARCHAR2(1),
      turn_restrictions mdsys.num_array)
    STORAGE (maxextents unlimited), NOLOGGING';

  EXECUTE IMMEDIATE 'INSERT /*+ APPEND */ INTO edge_part
    SELECT edge_id, start_node_id, end_node_id,
        (SELECT p_id FROM node_part WHERE vertex_id = start_node_id),
        (SELECT p_id FROM node_part WHERE vertex_id = end_node_id),
        func_class, length, speed_limit, divider, null
      FROM edge';

  COMMIT;

  -- Create useful indices on edge_part.
  --
  log_message('INFO: create index edge_part_e_idx on edge_part');
  EXECUTE IMMEDIATE 'CREATE INDEX edge_part_e_idx on edge_part(edge_id)';

  log_message('INFO: create index edge_part_s_e_idx on edge_part');
  EXECUTE IMMEDIATE 'CREATE INDEX edge_part_s_e_idx on edge_part(source_id, edge_id)';

  log_message('INFO: create index edge_part_t_e_idx on edge_part');
  EXECUTE IMMEDIATE 'CREATE INDEX edge_part_t_e_idx on edge_part(target_id, edge_id)';

  log_message('INFO: create index edge_part_st_p_idx on edge_part');
  EXECUTE IMMEDIATE 'CREATE INDEX edge_part_st_p_idx on edge_part(source_p_id, target_p_id)';

  log_message('INFO: create index edge_part_ts_p_idx on edge_part');
  EXECUTE IMMEDIATE 'CREATE INDEX edge_part_ts_p_idx on edge_part(target_p_id, source_p_id)';

  -- Populate the inedges and outedges fields to the node_part table.
  --
  log_message('INFO: create and load outedge and inedge columns in node_part table');
  update_node_part_edge_arrays;

  create_restricted_nodes;

  -- Recreate the node table and load it from the node_tmp table.
  --
  log_message('INFO: recreating node table with partitioning information');
  EXECUTE IMMEDIATE 'CREATE TABLE node STORAGE (maxextents unlimited), NOLOGGING AS
                      SELECT nt.node_id, nt.geometry, np.p_id partition_id
                      FROM node_tmp nt, node_part np
                      WHERE nt.node_id = np.vertex_id';
  COMMIT;

  EXECUTE IMMEDIATE 'DROP TABLE node_tmp CASCADE CONSTRAINTS';

  -- Create an index on the node and partition id fields in the NODE table.
  --
  log_message('INFO: create index node_id_index on node');
  EXECUTE IMMEDIATE 'CREATE INDEX node_id_index on node(node_id)';

  -- Make node_id a Primary key
  EXECUTE IMMEDIATE 'ALTER TABLE node ADD CONSTRAINT pk_node PRIMARY KEY(node_id)';

  log_message('INFO: create foreign keys');
  EXECUTE IMMEDIATE 'ALTER TABLE edge ADD CONSTRAINT fk_edge_start_node_id FOREIGN KEY (start_node_id) REFERENCES node(node_id)';
  EXECUTE IMMEDIATE 'ALTER TABLE edge ADD CONSTRAINT fk_edge_end_node_id FOREIGN KEY (end_node_id) REFERENCES node(node_id)';

  log_message('INFO: create index node_partition_index on node');
  EXECUTE IMMEDIATE 'CREATE INDEX node_partition_index on node(partition_id)';

  -- Set the partition ids in the edge table.
  --
  EXECUTE IMMEDIATE 'ALTER TABLE edge nologging';
  IF (index_exists('edge_partition_index') = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP INDEX edge_partition_index';
  END IF;

  log_message('INFO: updating edge table with partitioning information');
  EXECUTE IMMEDIATE 'UPDATE edge
    SET partition_id = (SELECT p_id from node_part
                          WHERE  vertex_id = start_node_id)';
  COMMIT;

  -- Create an index on the partition id field in the EDGE table.
  --
  log_message('INFO: create index edge_partition_index on edge');
  EXECUTE IMMEDIATE 'CREATE INDEX edge_partition_index on edge(partition_id)';

  -- Create and populate the super_node and super_edge tables.
  --
  log_message('INFO: creating and loading super_node_ids table');
  IF (table_exists('super_node_ids') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE super_node_ids';
  ELSE
    EXECUTE IMMEDIATE 'CREATE TABLE super_node_ids (node_id number)';
  END IF;

  EXECUTE IMMEDIATE 'INSERT /*+APPEND */ into super_node_ids
    (SELECT source_id FROM edge_part WHERE func_class = 1 or func_class=2
      UNION
    SELECT target_id FROM edge_part WHERE func_class = 1 or func_class=2)';

  log_message('INFO: creating and loading super_edge_ids table');
  IF (table_exists('super_edge_ids') = 'TRUE') THEN
    EXECUTE IMMEDIATE 'TRUNCATE TABLE super_edge_ids';
  ELSE
    EXECUTE IMMEDIATE 'CREATE TABLE super_edge_ids (edge_id number)';
  END IF;

  EXECUTE IMMEDIATE 'INSERT /*+APPEND */ into super_edge_ids
    SELECT edge_id FROM edge_part WHERE func_class =1 or func_class = 2';

  COMMIT;
END create_super_tables;

---
---
---
FUNCTION get_edge_info(edge_ids       IN  sdo_list_type,
                       to_edge_ids    OUT sdo_list_type,
                       rets           OUT mdsys.string_array,
                       angle_segments OUT sdo_list_type)
RETURN mdsys.string_array AS
    n           INTEGER;
    i           INTEGER;
    k           INTEGER;
    base        INTEGER := 1;
    to_edge_id  INTEGER;
    coords      mdsys.sdo_ordinate_array;
    names       mdsys.string_array;
    name_query  VARCHAR2(2000);
    sign_query  VARCHAR2(2000);
    ret         VARCHAR2(200);
    TYPE cursor_type IS REF CURSOR;
    sign_cursor cursor_type;
BEGIN
    IF (edge_ids IS NULL) THEN
        RETURN NULL;
    END IF;

    -- Initialize varrays
    n := edge_ids.count;
    names := mdsys.string_array();
    to_edge_ids := sdo_list_type();
    rets := mdsys.string_array();
    angle_segments := sdo_list_type();
    names.extend(n);
    to_edge_ids.extend(n);
    rets.extend(n);
    -- Need 4 points to describe a start and end segment for each edge id
    angle_segments.extend(n*8);

    -- Initialize name query
    name_query := 'SELECT t.name,t.geometry.sdo_ordinates FROM edge t WHERE t.edge_id = :1';
    -- Initialize sign query
    sign_query := 'SELECT to_edge_id, ' ||
                  'ramp || '':'' || exit || '':'' || toward '||
                  'FROM sign_post ' ||
                  'WHERE from_edge_id = :1';


    -- Iterate through route edge_ids and find info for each.
    FOR i IN 1..n LOOP
        EXECUTE IMMEDIATE name_query
        INTO names(i), coords
        USING edge_ids(i);
        IF (names(i) IS NULL) THEN
            names(i) := 'RAMP';
        END IF;
        -- Get sign information, if any.
        to_edge_ids(i) := 0;
        rets(i) := NULL;
        -- Get the coordinates for the start and end segments of the edge
        angle_segments(base)   := coords(1);
        angle_segments(base+1) := coords(2);
        angle_segments(base+2) := coords(3);
        angle_segments(base+3) := coords(4);
        angle_segments(base+4) := coords(coords.count-3);
        angle_segments(base+5) := coords(coords.count-2);
        angle_segments(base+6) := coords(coords.count-1);
        angle_segments(base+7) := coords(coords.count);
        base := base + 8;
        -- We have to use ABS(edge_ids(i)) since sign_post
        -- table contains only NAVSTREETS edge ids
        -- (positive only) not routeserver edge ids (which
        -- can be negative).
        OPEN sign_cursor FOR sign_query USING ABS(edge_ids(i));
        LOOP
            FETCH sign_cursor INTO to_edge_id, ret;
            EXIT WHEN sign_cursor%NOTFOUND;
            FOR k IN i+1..n LOOP
                IF (to_edge_id = edge_ids(k) OR
                    (-1*to_edge_id) = edge_ids(k)) THEN
                    -- Make sure we assign router edge id:
                    -- (negative or positive)!!!
                    to_edge_ids(i) := edge_ids(k);
                    rets(i) := ret;
                    EXIT;
                END IF;
            END LOOP;
            IF (to_edge_ids(i) <> 0) THEN
                EXIT;
            END IF;
        END LOOP;
        CLOSE sign_cursor;
    END LOOP;
    RETURN names;
END get_edge_info;

---
---
---
FUNCTION get_geometry_info (edge_ids      IN  sdo_list_type,
                            merged_coords OUT sdo_list_type)
RETURN NUMBER AS
  coords            MDSYS.SDO_ORDINATE_ARRAY;
  j                 NUMBER;
  k                 NUMBER;
BEGIN
  IF (edge_ids IS NULL) THEN
    RETURN 0;
  END IF;

  k := 1;
  merged_coords := sdo_list_type();

  -- For each input edge id, get the list of coordinates for the edge and
  -- build a list of all coordinates for the edges.
  FOR i in 1 .. edge_ids.count
  LOOP
    EXECUTE IMMEDIATE
     'select t.geometry.sdo_ordinates from edge t ' ||
     'where edge_id=:i'
    INTO coords USING edge_ids(i);

    j := 1;

    merged_coords.extend(coords.count + 1);
    merged_coords(k) := coords.count;
    k := k + 1;

    WHILE j <= coords.count
    LOOP
      merged_coords(k) := coords(j);
      merged_coords(k+1) := coords(j+1);

      j := j + 2;
      k := k + 2;
    END LOOP;
  END LOOP;

  RETURN merged_coords.count;

END get_geometry_info;

---
--- Find and validate the Routeservers data version
---
FUNCTION get_version_info
  RETURN VARCHAR2
IS
  data_version  VARCHAR2(32) := '10.2.0.4.0';
  major_version VARCHAR2(16);
  stmt          VARCHAR2(256);
  v_count       NUMBER;
BEGIN
  IF (table_exists('SDO_ROUTER_DATA_VERSION')= 'TRUE') THEN
    stmt := 'SELECT COUNT(*) FROM SDO_ROUTER_DATA_VERSION';
    EXECUTE IMMEDIATE stmt INTO v_count;

    IF (v_count != 1) THEN
      log_message('ERROR: Routeserver data version table corrupted, ' ||
        'multiple versions found');
      raise_application_error(-20005, 'Error getting data version, multiple versions found');
    END IF;

    stmt := 'SELECT data_version FROM SDO_ROUTER_DATA_VERSION';
    EXECUTE IMMEDIATE stmt INTO data_version;

    major_version := SUBSTR(data_version, 0, INSTR(data_version, '.')-1);

    IF (INSTR(data_version, '.', 1, 4) = 0 OR
        (major_version!='10' AND major_version!='11')) THEN
      log_message('ERROR: Routeserver data version table corrupted, ' ||
        'unsupported data version ' || data_version);
      raise_application_error(-20005,
        'Error getting data version, unsupported data version ' || data_version);
    END IF;
  END IF;

  RETURN data_version;
END get_version_info;

---
---
---
FUNCTION is_10g(version IN VARCHAR2)
  RETURN BOOLEAN
IS
BEGIN
  RETURN (SUBSTR(version, 0, INSTR(version, '.')-1) = '10');
END;

---
---
---
FUNCTION open_log_file(log_file_name IN VARCHAR2)
RETURN VARCHAR2 AS
  full_file_name  VARCHAR2(256);
  stmt         VARCHAR2(256);
BEGIN

  BEGIN
  -- Open the routers partition log file
  part_log_file := utl_file.fopen ('SDO_ROUTER_LOG_DIR', log_file_name, 'A');

  stmt := 'SELECT directory_path from all_directories
    where directory_name=''SDO_ROUTER_LOG_DIR''';
  EXECUTE IMMEDIATE stmt into full_file_name;

  full_file_name := full_file_name || '/' || log_file_name;

  EXCEPTION
    WHEN OTHERS THEN
      raise_application_error(-20001, 'Error Opening Log File');
  END;

  RETURN full_file_name;
END open_log_file;

--
-- Entry point to used to create a network on the Router data
--
PROCEDURE create_router_network(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                                network_name  IN VARCHAR2 := 'NDM_US')
IS
  expression      VARCHAR2(1024);
  full_file_name  VARCHAR2(256);
  l_network_name  VARCHAR2(256);
  link_view_name  VARCHAR2(256);
  node_view_name  VARCHAR2(256);
  part_view_name  VARCHAR2(256);
  pblob_view_name VARCHAR2(256);
  stmt            VARCHAR2(1024);
BEGIN
  full_file_name := open_log_file(log_file_name);

  -- Sanity check the passed in network name
  l_network_name := UPPER(SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(network_name));

  -- Build the view names based on the network names
  link_view_name := l_network_name || '_LINK$';
  node_view_name := l_network_name || '_NODE$';
  part_view_name := l_network_name || '_PART$';
  pblob_view_name := l_network_name || '_PBLOB$';

  log_message('INFO: creating the Routeserver network: ' || l_network_name);

  -- cleanup metadata
  stmt := 'delete from USER_SDO_NETWORK_METADATA where NETWORK = :name';
  EXECUTE IMMEDIATE stmt using l_network_name;

  stmt := 'delete from USER_SDO_NETWORK_USER_DATA where NETWORK = :name';
  EXECUTE IMMEDIATE stmt using l_network_name;

  -- create index for edge func_class
  IF (index_exists('EDGE_FUNC_CLASS_IDX') = 'FALSE') THEN
    log_message('      creating function class index', FALSE);
    EXECUTE IMMEDIATE 'create index EDGE_FUNC_CLASS_IDX on EDGE (FUNC_CLASS)';
  END IF;

  -- create a function based index for link level
  IF (index_exists('EDGE_LEVEL_IDX') = 'FALSE') THEN
    log_message('      creating link level index', FALSE);
    EXECUTE IMMEDIATE 'create index EDGE_LEVEL_IDX on EDGE(elocation_edge_link_level(FUNC_CLASS))';
  ELSE
    stmt := 'SELECT COLUMN_EXPRESSION FROM USER_IND_EXPRESSIONS WHERE INDEX_NAME = :name';
    EXECUTE IMMEDIATE stmt into expression using 'EDGE_LEVEL_IDX';

    IF(substr(expression, 10, 25) <> 'ELOCATION_EDGE_LINK_LEVEL') THEN
      log_message('      dropping current link level index', FALSE);
      EXECUTE IMMEDIATE 'DROP INDEX EDGE_LEVEL_IDX';

      log_message('      creating link level index', FALSE);
      EXECUTE IMMEDIATE 'create index EDGE_LEVEL_IDX on EDGE(elocation_edge_link_level(FUNC_CLASS))';
    END IF;
  END IF;

  log_message('      creating views', FALSE);

  -- create a view on the NODE table
  stmt:=
    'create or replace view ' || node_view_name ||
        ' as select n.node_id node_id,
               n.geometry geometry,
               n.geometry.sdo_point.x x,
               n.geometry.sdo_point.y y
           from NODE n';

  EXECUTE IMMEDIATE stmt;

  -- create a view on the EDGE table
  stmt :=
    'create or replace view ' || link_view_name ||
        ' as select edge_id link_id,
              start_node_id start_node_id,
              end_node_id end_node_id,
              elocation_edge_link_level(FUNC_CLASS) link_level,
              length length,
              speed_limit s,
              func_class f,
              geometry geometry,
              name name,
              divider divider
            from EDGE';

  EXECUTE IMMEDIATE stmt;

  --create a view on the NODE table node_id and partition_id information
  stmt :=
    'create or replace view ' || part_view_name ||
        ' as select node_id node_id,
              partition_id partition_id,
              1 link_level
            from NODE';

  EXECUTE IMMEDIATE stmt;

  -- create a view on the PARTITION table adding link level and changing
  -- the format of the edge counts
  stmt := 'create or replace view ' || pblob_view_name ||
      ' as select link_level link_level,
            a.partition_id partition_id,
            subnetwork blob,
            num_nodes num_inodes,
            num_outgoing_boundary_edges+num_incoming_boundary_edges num_enodes,
            num_non_boundary_edges num_ilinks,
            num_outgoing_boundary_edges+num_incoming_boundary_edges num_elinks,
            num_incoming_boundary_edges num_inlinks,
            num_outgoing_boundary_edges num_outlinks, ' ||
            SYS.DBMS_ASSERT.ENQUOTE_LITERAL('Y') || ' user_data_included
           from
            (select 1 link_level, partition_id partition_id
             from PARTITION
             where partition_id > 0
              union all
             select 2 link_level, partition_id partition_id
             from PARTITION
             where partition_id = 0) a,
            PARTITION b
           where a.partition_id = b.partition_id';

  EXECUTE IMMEDIATE stmt;

  log_message('      generating metadata', FALSE);

  --insert network metadata
  stmt := 'insert into USER_SDO_NETWORK_METADATA
            (network,
             network_category,
             geometry_type,
             node_table_name,
             node_geom_column,
             link_table_name,
             link_geom_column,
             link_cost_column,
             link_direction,
             partition_table_name,
             partition_blob_table_name,
             user_defined_data)
           values (:1, :2, :3, :4, :5, :6, :7, :8, :9, :10, :11, :12)';

  EXECUTE IMMEDIATE stmt USING l_network_name, 'SPATIAL', 'SDO_GEOMETRY',
    node_view_name, 'GEOMETRY', link_view_name, 'GEOMETRY', 'LENGTH',
    'DIRECTED', part_view_name, pblob_view_name, 'Y';

  -- insert user data metadata
  -- node x coordinate
  stmt := 'insert into user_sdo_network_user_data
            (network, table_type, data_name,data_type)
           values (:1, :2, :3, :4)';
  EXECUTE IMMEDIATE stmt USING l_network_name, 'NODE', 'X', 'NUMBER';

  -- node y coordinate
  stmt := 'insert into user_sdo_network_user_data
            (network, table_type, data_name, data_type)
           values (:1, :2, :3, :4)';
  EXECUTE IMMEDIATE stmt USING l_network_name, 'NODE', 'Y', 'NUMBER';

  -- link speed limit
  stmt := 'insert into user_sdo_network_user_data
            (network, table_type, data_name, data_type)
           values (:1, :2, :3, :4)';
  EXECUTE IMMEDIATE stmt USING l_network_name, 'LINK', 'S', 'NUMBER';

  -- link function class
  stmt := 'insert into user_sdo_network_user_data
            (network, table_type, data_name, data_type)
           values (:1, :2, :3, :4)';
  EXECUTE IMMEDIATE stmt USING l_network_name, 'LINK', 'F', 'NUMBER';

  COMMIT;

  utl_file.fclose(part_log_file);
END;

--
-- Entry point to used to create a network on the Router data
--
PROCEDURE delete_router_network(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                                network_name  IN VARCHAR2 := 'NDM_US')
IS
  full_file_name  VARCHAR2(256);
  l_network_name  VARCHAR2(256);
  link_view_name  VARCHAR2(256);
  node_view_name  VARCHAR2(256);
  part_view_name  VARCHAR2(256);
  pblob_view_name VARCHAR2(256);
  stmt            VARCHAR2(512);
BEGIN
  full_file_name := open_log_file(log_file_name);

  -- raise an error if the network doesn't exist
  IF (network_exists(network_name) = 'FALSE') THEN
    log_message('ERROR: network delete failed, ' || network_name || ' network not found');
    raise_application_error(-20020, 'Network delete failed, ' || network_name || ' network not found');
  END IF;

  l_network_name := UPPER(SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(network_name));

  stmt := 'select node_table_name, link_table_name,
                  partition_table_name, partition_blob_table_name
           from USER_SDO_NETWORK_METADATA
           where NETWORK = :name';

  EXECUTE IMMEDIATE stmt
    INTO node_view_name, link_view_name, part_view_name, pblob_view_name
    USING l_network_name;

  log_message('INFO: deleting the Routeserver network: ' || l_network_name);

  -- cleanup metadata
  stmt := 'delete from USER_SDO_NETWORK_METADATA where NETWORK = :name';
  EXECUTE IMMEDIATE stmt using l_network_name;

  stmt := 'delete from USER_SDO_NETWORK_USER_DATA where NETWORK = :name';
  EXECUTE IMMEDIATE stmt using l_network_name;

  -- cleanup views
  IF (table_exists(link_view_name) = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP VIEW ' ||
        SYS.DBMS_ASSERT.ENQUOTE_NAME(link_view_name);
  END IF;

  IF (table_exists(node_view_name) = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP VIEW ' ||
        SYS.DBMS_ASSERT.ENQUOTE_NAME(node_view_name);
  END IF;

  IF (table_exists(part_view_name) = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP VIEW ' ||
        SYS.DBMS_ASSERT.ENQUOTE_NAME(part_view_name);
  END IF;

  IF (table_exists(pblob_view_name) = 'TRUE') THEN
      EXECUTE IMMEDIATE 'DROP VIEW ' ||
        SYS.DBMS_ASSERT.ENQUOTE_NAME(pblob_view_name);
  END IF;

  COMMIT;

  utl_file.fclose(part_log_file);
END;

--
-- Entry point and driver of the entire partitioning process.
-- high level procedure for partitioning graph based on coordinate
-- information (inertia bisecting). The parameters are defaulted
-- so the customers don't have to worry about them.
--
PROCEDURE partition_router(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                        max_v_no IN NUMBER DEFAULT 10000,
                        driving_side IN VARCHAR2 := 'R',
                        network_name IN VARCHAR := 'NDM_US',
                        cleanup IN BOOLEAN DEFAULT TRUE)
IS
  full_file_name VARCHAR2(256);
  stmt         VARCHAR2(256);
  msg_cleanup  VARCHAR2(10) := 'TRUE';
BEGIN
  IF (NOT cleanup) THEN
    msg_cleanup := 'FALSE';
  END IF;

  full_file_name := open_log_file(log_file_name);

  log_message('******** Beginning SDO Router partitioning');
  log_message('** Logfile location: ' || full_file_name, FALSE);
  log_message('** Nodes per partition: ' || max_v_no, FALSE);
  log_message('** Driving side: ' || driving_side, FALSE);
  log_message('** Router network name: ' || network_name, FALSE);
  log_message('** Cleanup temporary files: ' || msg_cleanup, FALSE);

  setup_tables;

  create_node_part;

  graph_partition('NODE_PART', max_v_no, TRUE);

  create_super_tables;

  -- Table of nodes that are either the start or end node of a restricted edge.
  create_restricted_nodes;

  -- Table of edges that that have either a start or end node in the
  -- restricted_nodes table. We store the first and last segment of each edge.
  -- These segments are far more accurate than the edge as a whole for
  -- computing turn angles.
  create_restricted_edges;

  -- Close the log file so the Java code can use it
  utl_file.fclose(part_log_file);

  -- Adjust the Oracle JVM maximum memory size to 800M.
  -- This is the equivelent to specifying -Xmx800m to
  -- java outside the database. Memory size is specified in bytes.
  ElocationSetJVMHeapSize(838860800);

  -- Java code to generate turn restrictions. This MUST be done after the
  -- NODE_PART and EDGE_PART tables have been populated but before we actually
  -- create the partition table.
  build_turn_restrictions(full_file_name, driving_side);

BEGIN
  -- Java code to create the partiton table
  elocation_partition_router(full_file_name);
EXCEPTION
  WHEN OTHERS THEN
    part_log_file := utl_file.fopen ('SDO_ROUTER_LOG_DIR', log_file_name, 'A');
    log_message(SQLERRM);
    utl_file.fclose(part_log_file);
    RAISE JAVA_ERROR;
END;

  -- Reopen the logfile
  full_file_name := open_log_file(log_file_name);

  log_message('INFO: creating the final partition table');

  -- Rename the new partition table and build an index on it.
  IF (table_exists('partition')= 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE partition';
  END IF;

  EXECUTE IMMEDIATE 'RENAME new_partition to partition';

  log_message('INFO: create index partition_p_idx on partition table');
  EXECUTE IMMEDIATE 'CREATE INDEX partition_p_idx on partition(partition_id)';

  -- Create the indexes, views and metadata needed by the NDM on top of
  -- the Router data
  create_router_network(log_file_name, network_name);

  -- If trucking user data exists, partition it.
  IF (table_exists('router_transport') = 'TRUE') THEN
    create_trucking_user_data(log_file_name);
  END IF;

  clean_tables(cleanup);
  log_message('******** Completed SDO Router partitioning');

  -- Close the log file
  utl_file.fclose(part_log_file);
EXCEPTION
    WHEN JAVA_ERROR THEN
        raise_application_error(-20000, 'Oracle Router partitioning failed');
    WHEN OTHERS THEN
        IF (utl_file.is_open(part_log_file) = FALSE) THEN
          part_log_file := utl_file.fopen ('SDO_ROUTER_LOG_DIR', log_file_name, 'A');
        END IF;
        log_message(SQLERRM);
        utl_file.fclose(part_log_file);
        raise_application_error(-20000, 'Oracle Router partitioning failed');
END partition_router;

---
--- Entry point to partition trucking data
---
PROCEDURE create_trucking_user_data(log_file_name IN VARCHAR2 := 'sdo_router_partition.log')
IS
  TYPE CURSOR_TYPE IS REF CURSOR;
  full_file_name  VARCHAR2(256);
  ins_stmt        VARCHAR2(256);
  stmt            VARCHAR2(256);
  p_cursor        CURSOR_TYPE;
  edge_id         NUMBER;
  partition_id    NUMBER;
  func_class      NUMBER;
  l_maintype      NUMBER;
  l_subtype       NUMBER;
  l_value         NUMBER;
BEGIN
  full_file_name := open_log_file(log_file_name);

  log_message('******** Begin generation of trucking user data');
  log_message('** Logfile location: ' || full_file_name, FALSE);

  -- Make sure the raw truck data exists and is in Router format
  IF (table_exists('router_transport') = 'FALSE') THEN
    log_message('ERROR: ROUTER_TRANSPORT table not found');
    RETURN ;
  END IF;

  -- Make sure the Edge table exists
  IF (table_exists('EDGE') = 'FALSE') THEN
    log_message('ERROR: EDGE table not found');
    RETURN ;
  END IF;

  -- Make sure the data version table exists
  IF (table_exists('sdo_router_data_version') = 'FALSE') THEN
    log_message('ERROR: SDO_ROUTER_DATA_VERSION table not found');
    RETURN ;
  END IF;

  -- Cleanup the intermediate truck data partitioning table if its still around
  IF (table_exists('router_partitioned_truck_data') = 'TRUE') THEN
    execute immediate 'DROP TABLE router_partitioned_truck_data';
  END IF;

  -- Create truck data intermediate partitioning table
  stmt := 'CREATE TABLE router_partitioned_truck_data(
              edge_id NUMBER, partition_id NUMBER, maintype NUMBER(2),
              subtype NUMBER(2), value NUMBER(6,2))
             STORAGE (maxextents unlimited), NOLOGGING';
  EXECUTE IMMEDIATE stmt;

  ins_stmt := 'INSERT INTO router_partitioned_truck_data VALUES(:eid, :pid, :mtype, :stype, :val)';

  OPEN p_cursor FOR
    'SELECT e.edge_id eid, partition_id, func_class, maintype, subtype, value
     FROM edge e, router_transport r
     WHERE (e.edge_id = r.edge_id)';

  LOOP
    FETCH p_cursor INTO edge_id, partition_id, func_class, l_maintype, l_subtype, l_value;
    EXIT WHEN p_cursor%NOTFOUND;

    -- Associate the edge with a partition.
    EXECUTE IMMEDIATE ins_stmt USING
      edge_id, partition_id, l_maintype, l_subtype, l_value;

    -- If the edge is a highway place it in the highway partition also
    IF ((func_class = 1) OR (func_class = 2)) THEN
      partition_id := 0;
      EXECUTE IMMEDIATE ins_stmt USING
        edge_id, partition_id, l_maintype, l_subtype, l_value;
    END IF;
  END LOOP;
  CLOSE p_cursor;
  EXECUTE IMMEDIATE 'CREATE INDEX rtd_p_idx ON router_partitioned_truck_data(partition_id)';

  -- Close the log file so the Java code can use it
  utl_file.fclose(part_log_file);

  -- Adjust the Oracle JVM maximum memory size to 800M.
  -- This is the equivelent to specifying -Xmx800m to
  -- java outside the database. Memory size is specified in bytes.
  ElocationSetJVMHeapSize(838860800);

BEGIN
  -- Java code to partition the trucking data
  elocation_trucking_user_data(full_file_name);
EXCEPTION
  WHEN OTHERS THEN
    part_log_file := utl_file.fopen ('SDO_ROUTER_LOG_DIR', log_file_name, 'A');
    log_message(SQLERRM);
    utl_file.fclose(part_log_file);
    RAISE JAVA_ERROR;
END;
  -- Reopen the logfile
  full_file_name := open_log_file(log_file_name);

  log_message('INFO: creating the final trucking user data table');

  -- Rename the new partition table and build an index on it.
  IF (table_exists('trucking_user_data')= 'TRUE') THEN
    EXECUTE IMMEDIATE 'DROP TABLE trucking_user_data';
  END IF;

  EXECUTE IMMEDIATE 'RENAME new_trucking_user_data to trucking_user_data';

  log_message('INFO: create index trucking_ud_p_idx on trucking_user_data table');
  EXECUTE IMMEDIATE 'CREATE INDEX trucking_ud_p_idx on trucking_user_data(partition_id)';

  log_message('******** Completed generartion of trucking user data ');

  -- Close the log file
  utl_file.fclose(part_log_file);
EXCEPTION
    WHEN JAVA_ERROR THEN
        raise_application_error(-20015, 'Oracle Router trucking user data generation failed');
    WHEN OTHERS THEN
        IF (utl_file.is_open(part_log_file) = FALSE) THEN
          part_log_file := utl_file.fopen ('SDO_ROUTER_LOG_DIR', log_file_name, 'A');
        END IF;
        log_message(SQLERRM);
        utl_file.fclose(part_log_file);
        raise_application_error(-20015, 'Oracle Router trucking user data generation failed');
END create_trucking_user_data;

--
-- Entry point to cleanup tables used for debugging
--
PROCEDURE cleanup_router(all_tables IN BOOLEAN)
IS
BEGIN
  -- Cleanup all temporary tables.
  clean_tables(all_tables);
END;

--
-- Entry point to used to produce a dump of the partition BLOBs
--
PROCEDURE dump_partitions(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                      start_pid IN NUMBER DEFAULT 0,
                      end_pid IN NUMBER DEFAULT -1,
                      verbose IN BOOLEAN DEFAULT FALSE)
IS
  full_file_name  VARCHAR2(256);
  l_end_pid       NUMBER := end_pid;
  max_pid         NUMBER;
  stmt            VARCHAR2(256);
  version         VARCHAR2(32);
BEGIN
  full_file_name := open_log_file(log_file_name);

  -- Make sure the table is actually there
  IF (table_exists('PARTITION') = 'FALSE' ) THEN
    log_message('ERROR: Partition dump failed, PARTITION table not found');
    utl_file.fclose(part_log_file);

    RAISE PARAMETER_ERROR;
  END IF;

  stmt := 'SELECT MAX(PARTITION_ID) FROM PARTITION';
  EXECUTE IMMEDIATE stmt INTO max_pid;

  -- The default value for the end partition id is max(partition_id)
  IF (l_end_pid < 0) THEN
    l_end_pid := max_pid;
  END IF;

  -- Validate the starting partition id.
  IF ((start_pid < 0) OR (start_pid > max_pid)) THEN
    log_message('ERROR: Invald Start Partition ID '||start_pid||', Valid Range (0,'||max_pid||')');
    utl_file.fclose(part_log_file);

    RAISE PARAMETER_ERROR;
  END IF;

  -- Validate the ending partition id.
  IF ((l_end_pid < start_pid) OR (l_end_pid > max_pid)) THEN
    log_message('ERROR: Invald End Partition ID '||to_char(end_pid)||', Valid Range ('||start_pid||','||max_pid||')');
    utl_file.fclose(part_log_file);

    RAISE PARAMETER_ERROR;
  END IF;

  version := get_version_info();
  log_message('******** Beginning partition dump');
  log_message('** Logfile location: ' || full_file_name, FALSE);
  log_message('** Routeserver data version: ' || version, FALSE);
  log_message('** Start partition id: ' || start_pid, FALSE);
  log_message('** End partition id: ' || l_end_pid, FALSE);
  IF (verbose) THEN
    log_message('** Verbose mode: TRUE', FALSE);
  ELSE
    log_message('** Verbose mode: FALSE', FALSE);
  END IF;

  log_message('', FALSE);

  -- Close the log file so the Java code can use it
  utl_file.fclose(part_log_file);

  elocation_dump_partition(full_file_name, start_pid, l_end_pid,
    verbose, is_10g(version));

  EXCEPTION
    WHEN PARAMETER_ERROR THEN
      raise_application_error(-20004, 'Error, partition dump failed, see log file.');
    WHEN OTHERS THEN
        IF (utl_file.is_open(part_log_file) = FALSE) THEN
          part_log_file := utl_file.fopen ('SDO_ROUTER_LOG_DIR', log_file_name, 'A');
        END IF;
        log_message(SQLERRM);
        utl_file.fclose(part_log_file);
        raise_application_error(-20004, 'Error, partition dump failed, see log file.');
END;

--
-- Entry point to used to validate the partition BLOBs
--
PROCEDURE validate_partitions(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                              start_pid IN NUMBER DEFAULT 0,
                              end_pid IN NUMBER DEFAULT -1,
                              verbose IN BOOLEAN DEFAULT FALSE)
IS
  full_file_name  VARCHAR2(256);
  l_end_pid       NUMBER := end_pid;
  max_pid         NUMBER;
  stmt            VARCHAR2(256);
  version         VARCHAR2(32);
BEGIN
  full_file_name := open_log_file(log_file_name);

  -- Make sure the table is actually there
  IF (table_exists('PARTITION') = 'FALSE' ) THEN
    log_message('ERROR: Partition validate failed, PARTITION table not found');
    utl_file.fclose(part_log_file);

    RAISE PARAMETER_ERROR;
  END IF;

  stmt := 'SELECT MAX(PARTITION_ID) FROM PARTITION';
  EXECUTE IMMEDIATE stmt INTO max_pid;

  -- The default value for the end partition id is max(partition_id)
  IF (l_end_pid < 0) THEN
    l_end_pid := max_pid;
  END IF;

  -- Validate the starting partition id.
  IF ((start_pid < 0) OR (start_pid > max_pid)) THEN
    log_message('ERROR: Invald Start Partition ID '||start_pid||
      ', Valid Range (0,'||max_pid||')');
    utl_file.fclose(part_log_file);

    RAISE PARAMETER_ERROR;
  END IF;

  -- Validate the ending partition id.
  IF ((l_end_pid < start_pid) OR (l_end_pid > max_pid)) THEN
    log_message('ERROR: Invald End Partition ID '||to_char(end_pid)||
      ', Valid Range ('||start_pid||','||max_pid||')');
    utl_file.fclose(part_log_file);

    RAISE PARAMETER_ERROR;
  END IF;

  version := get_version_info();
  log_message('******** Beginning partition validation');
  log_message('** Logfile location: ' || full_file_name, FALSE);
  log_message('** Routeserver data version: ' || version, FALSE);

  log_message('** Start partition id: ' || start_pid, FALSE);
  log_message('** End partition id: ' || l_end_pid, FALSE);
  IF (verbose) THEN
    log_message('** Verbose mode: TRUE', FALSE);
  ELSE
    log_message('** Verbose mode: FALSE', FALSE);
  END IF;
  log_message('', FALSE);

  -- Close the log file so the Java code can use it
  utl_file.fclose(part_log_file);

  -- Adjust the Oracle JVM maximum memory size to 800M.
  -- This is the equivelent to specifying -Xmx800m to
  -- java outside the database. Memory size is specified in bytes.
  ElocationSetJVMHeapSize(838860800);

  elocation_validate_partition(full_file_name, start_pid, l_end_pid,
    verbose, is_10g(version));

  EXCEPTION
    WHEN PARAMETER_ERROR THEN
      raise_application_error(-20003, 'Error, partition validation failed, see log file.');
    WHEN OTHERS THEN
        IF (utl_file.is_open(part_log_file) = FALSE) THEN
          part_log_file := utl_file.fopen ('SDO_ROUTER_LOG_DIR', log_file_name, 'A');
        END IF;
        log_message(SQLERRM);
        utl_file.fclose(part_log_file);
        raise_application_error(-20003, 'Error, partition validation failed, see log file.');
END;

--
-- Entry point to used to get the Router data version
--
PROCEDURE get_version(log_file_name IN VARCHAR2 := 'sdo_router_partition.log')
IS
  data_version    VARCHAR2(32);
  full_file_name  VARCHAR2(256);
BEGIN
  full_file_name := open_log_file(log_file_name);

  data_version := get_version_info();

  dbms_output.put_line('Routeserver: data version '|| data_version);
  log_message('INFO: Routeserver data version: ' || data_version);

  utl_file.fclose(part_log_file);
END;

--
-- main pl/sql procedure to partition a graph with coordinate information
-- for recovering with
--
PROCEDURE recover_graph_partition(p_tab_name IN VARCHAR2,
                                  min_pid IN NUMBER,
                                  max_pid IN NUMBER,
                                  p_level IN NUMBER,
                                  max_v_no IN NUMBER,
                                  make_equal IN BOOLEAN)
IS
  stmt      VARCHAR2(256);
  v_no      NUMBER;
  pid       NUMBER;
  p_counter NUMBER;
  tmp_p_tab_name VARCHAR2(32);
BEGIN
  p_counter := max_pid+1; -- starting partition counter
  pid := min_pid;

  -- start logging
  log_message('INFO: starting recovery of '|| p_tab_name || ' partitioning' ||
    ' partition level:' || p_level || ' min(partition id):' || min_pid ||
      ' max(partition id)' || max_pid);

  FOR k IN min_pid..max_pid LOOP
    FOR i IN 0..p_level LOOP
      FOR j IN 1..power(2,i) LOOP
        tmp_p_tab_name := p_tab_name || '_' || pid;

        IF (table_exists(tmp_p_tab_name) = 'TRUE' ) THEN
          new_partition_proc(p_tab_name, max_v_no,pid,make_equal,p_counter);

          log_message('INFO:    partitioning '|| p_tab_name ||
            ' level: ' || j || ' partition id: ' || pid);
        ELSE
          p_counter := p_counter + 2;
        END IF;

        pid := pid +1;
      end loop;
    end loop;
  end loop;

  -- copy the result back to original table and ajust the pids
  adjust_final_pid(SYS.DBMS_ASSERT.SIMPLE_SQL_NAME(p_tab_name));

  log_message('INFO: completed recovery of '|| p_tab_name || ' partitioning');

EXCEPTION
  WHEN OTHERS THEN
    log_message(SQLERRM);
    log_message('ERROR: exception recovering partition '|| pid ||
      ' of the ' || p_tab_name || ' table');
    raise_application_error(-20010, 'Error Recovering Graph Partitioning');
END recover_graph_partition;

--
-- Unused procedures and functions to delete
--
--

--
-- End unused procedures and functions
--

END SDO_ROUTER_PARTITION;
/


Prompt Synonym SDO_ROUTER_PARTITION;
--
-- SDO_ROUTER_PARTITION  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_ROUTER_PARTITION FOR MDSYS.SDO_ROUTER_PARTITION
/


Prompt Grants on PACKAGE SDO_ROUTER_PARTITION TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_ROUTER_PARTITION TO PUBLIC
/
