Prompt drop Package SDO_TOPO_MAP;
DROP PACKAGE MDSYS.SDO_TOPO_MAP
/

Prompt Package SDO_TOPO_MAP;
--
-- SDO_TOPO_MAP  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_TOPO_MAP AUTHID current_user AS

 PROCEDURE set_max_memory_size(bytes NUMBER)
    AS LANGUAGE JAVA NAME
    'oracle.aurora.vm.OracleRuntime.setMaxMemorySize(long)';
 PROCEDURE create_topo_map(topology VARCHAR2, topo_map VARCHAR2,
    number_of_edges NUMBER, number_of_nodes NUMBER,
    number_of_faces NUMBER)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.createTopoMap(java.lang.String,
     java.lang.String, int, int, int)';
 PROCEDURE create_topo_map(topology VARCHAR2, topo_map VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.createTopoMap(java.lang.String,
     java.lang.String)';
 FUNCTION list_topo_maps RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.listTopoMaps()
     return java.lang.String';
 FUNCTION get_topo_name(topo_map VARCHAR2) RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getTopoName(java.lang.String)
     return java.lang.String';
 FUNCTION load_topo_map(topo_map VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER,
    allow_updates VARCHAR2)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap(
     java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, java.lang.String) return java.lang.String';
 FUNCTION load_topo_map(topo_map VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER,
    allow_updates VARCHAR2, build_indexes VARCHAR2)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap(
     java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, java.lang.String, java.lang.String)
    return java.lang.String';
 FUNCTION load_topo_map(topo_map VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER,
    allow_updates VARCHAR2, build_indexes VARCHAR2, next_load VARCHAR2)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap(
     java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, java.lang.String, java.lang.String,  java.lang.String)
    return java.lang.String';
 FUNCTION load_topo_map(topo_map VARCHAR2, allow_updates VARCHAR2)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap(
     java.lang.String, java.lang.String) return java.lang.String';
 FUNCTION load_topo_map(topo_map VARCHAR2,
    allow_updates VARCHAR2, build_indexes VARCHAR2)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap(
     java.lang.String,
     java.lang.String, java.lang.String) return java.lang.String';
 PROCEDURE load_topo_map(topo_map VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER,
    allow_updates VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap2(
     java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, java.lang.String)';
 PROCEDURE load_topo_map(topo_map VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER,
    allow_updates VARCHAR2, build_indexes VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap2(
     java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, java.lang.String, java.lang.String)';
 PROCEDURE load_topo_map(topo_map VARCHAR2, allow_updates VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap2(
     java.lang.String, java.lang.String)';
 PROCEDURE load_topo_map(topo_map VARCHAR2,
    allow_updates VARCHAR2, build_indexes VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.loadTopoMap2(
     java.lang.String,
     java.lang.String, java.lang.String)';
 PROCEDURE create_edge_index(topo_map VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.createEdgeIndex(
     java.lang.String)';
 PROCEDURE create_face_index(topo_map VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.createFaceIndex(
     java.lang.String)';
 PROCEDURE update_topo_map
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.updateTopoMap()';
 PROCEDURE commit_topo_map
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.commitTopoMap()';
 PROCEDURE rollback_topo_map
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.rollbackTopoMap()';
 PROCEDURE clear_topo_map(topo_map VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.clearTopoMap(
     java.lang.String)';
 PROCEDURE drop_topo_map(topo_map VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.dropTopoMap(java.lang.String)';
 FUNCTION validate_topo_map(topo_map VARCHAR2) RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopoMap(
     java.lang.String) return java.lang.String';
 FUNCTION validate_topo_map(topo_map VARCHAR2, level NUMBER)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopoMap(
     java.lang.String, oracle.sql.NUMBER) return java.lang.String';

 FUNCTION search_face_rtree_topo_map(topo_map VARCHAR2, xmin NUMBER,
    ymin NUMBER, xmax NUMBER, ymax NUMBER, capacity NUMBER)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.searchFaceRTreeTopoMap(
     java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER
     ) return oracle.spatial.type.SdoNumberArray';

 FUNCTION search_edge_rtree_topo_map(topo_map VARCHAR2, xmin NUMBER,
    ymin NUMBER, xmax NUMBER, ymax NUMBER, capacity NUMBER)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.searchEdgeRTreeTopoMap(
     java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER
     ) return oracle.spatial.type.SdoNumberArray';

 FUNCTION get_node_changes
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNodeChanges()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_edge_changes
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getEdgeChanges()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_face_changes
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getFaceChanges()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_node_additions
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNodeAdditions()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_edge_additions
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getEdgeAdditions()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_face_additions
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getFaceAdditions()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_node_deletions
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNodeDeletions()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_edge_deletions
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getEdgeDeletions()
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_face_deletions
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getFaceDeletions()
     return oracle.spatial.type.SdoNumberArray';

 FUNCTION validate_topology(topology VARCHAR2)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopology(
     java.lang.String) return java.lang.String';
 FUNCTION validate_topology(topology VARCHAR2, prevent_updates VARCHAR2)
    RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopology(
     java.lang.String, java.lang.String) return java.lang.String';
 FUNCTION validate_topology(topology VARCHAR2, prevent_updates VARCHAR2,
    level NUMBER) RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopology(
     java.lang.String, java.lang.String, oracle.sql.NUMBER)
     return java.lang.String';
 FUNCTION validate_topology(topology VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER) RETURN VARCHAR2
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopology(
     java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER) return java.lang.String';
 FUNCTION validate_topology(topology VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER,
    prevent_updates VARCHAR2) RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopology(
     java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER, java.lang.String)
     return java.lang.String';
 FUNCTION validate_topology(topology VARCHAR2,
    xmin NUMBER, ymin NUMBER, xmax NUMBER, ymax NUMBER,
    prevent_updates VARCHAR2, level NUMBER) RETURN VARCHAR2 DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.validateTopology(
     java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER, java.lang.String,
     oracle.sql.NUMBER) return java.lang.String';

 FUNCTION add_edge(topology VARCHAR2,
    node_id1 NUMBER, node_id2 NUMBER, geom SDO_GEOMETRY)
    RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addEdge(java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry) return oracle.sql.NUMBER';
 FUNCTION add_loop(topology VARCHAR2,
    node_id NUMBER, geom SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addLoop(java.lang.String,
     oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry) return oracle.sql.NUMBER';
 FUNCTION get_edge_coords(topology VARCHAR2, topo_map VARCHAR2,
    edge_id NUMBER) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getEdgeCoords(java.lang.String,
     java.lang.String, oracle.sql.NUMBER)
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION add_node(topology VARCHAR2,
    edge_id NUMBER, point SDO_GEOMETRY, coord_index NUMBER,
    is_new_shape_point VARCHAR2) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addNode(java.lang.String,
     oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry, int, java.lang.String)
     return oracle.sql.NUMBER';
 FUNCTION add_node(topology VARCHAR2,
    edge_id NUMBER, x NUMBER, Y NUMBER, coord_index NUMBER,
    is_new_shape_point VARCHAR2) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addNode(java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.sql.NUMBER, int, java.lang.String)
     return oracle.sql.NUMBER';
 FUNCTION add_isolated_node(topology VARCHAR2,
    face_id NUMBER, point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addIsolatedNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry) return oracle.sql.NUMBER';
 FUNCTION add_isolated_node(topology VARCHAR2,
    face_id NUMBER, x NUMBER, y NUMBER) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addIsolatedNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER) return oracle.sql.NUMBER';
 FUNCTION add_isolated_node(topology VARCHAR2,
    point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addIsolatedNode(
     java.lang.String,
     oracle.spatial.type.SdoGeometry) return oracle.sql.NUMBER';
 FUNCTION add_isolated_node(topology VARCHAR2,
    x NUMBER, y NUMBER) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addIsolatedNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER) return oracle.sql.NUMBER';
 FUNCTION get_node_coord(topology VARCHAR2, topo_map VARCHAR2,
    node_id NUMBER) RETURN SDO_GEOMETRY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNodeCoord(java.lang.String,
     java.lang.String, oracle.sql.NUMBER)
     return oracle.spatial.type.SdoGeometry';
 PROCEDURE move_node(topology VARCHAR2,
    node_id NUMBER, edges_coords SDO_EDGE_ARRAY)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.spatial.type.SdoEdgeArray)';
 PROCEDURE move_node(topology VARCHAR2,
    node_id NUMBER, edges_coords SDO_EDGE_ARRAY,
    moved_iso_nodes OUT SDO_NUMBER_ARRAY,
    moved_iso_edges OUT SDO_NUMBER_ARRAY,
    allow_iso_moves VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.spatial.type.SdoEdgeArray,
     oracle.spatial.type.SdoNumberArray[],
     oracle.spatial.type.SdoNumberArray[],
     java.lang.String)';
 PROCEDURE move_isolated_node(topology VARCHAR2,
    node_id NUMBER, point SDO_GEOMETRY)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveIsolatedNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry)';
 PROCEDURE move_isolated_node(topology VARCHAR2,
    node_id NUMBER, point SDO_GEOMETRY,
                allow_face_change VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveIsolatedNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry,java.lang.String)';
 PROCEDURE move_isolated_node(topology VARCHAR2,
    node_id NUMBER, x NUMBER, y NUMBER)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveIsolatedNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER)';
 PROCEDURE move_isolated_node(topology VARCHAR2,
    node_id NUMBER, x NUMBER, y NUMBER,
               allow_face_change VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveIsolatedNode(
     java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER, oracle.sql.NUMBER,java.lang.String)';
 PROCEDURE remove_node(topology VARCHAR2,
    node_id NUMBER)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.removeNode(
     java.lang.String, oracle.sql.NUMBER)';
 PROCEDURE remove_obsolete_nodes(topology VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.removeObsoleteNodes(
     java.lang.String)';
 PROCEDURE move_edge(topology VARCHAR2,
    edge_id NUMBER, s_node_id NUMBER, t_node_id NUMBER,
    edge_coords SDO_NUMBER_ARRAY)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveEdge(
     java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.spatial.type.SdoNumberArray)';
 PROCEDURE move_edge(topology VARCHAR2,
    edge_id NUMBER, s_node_id NUMBER, t_node_id NUMBER,
    edge_coords SDO_NUMBER_ARRAY,
    moved_iso_nodes OUT SDO_NUMBER_ARRAY,
    moved_iso_edges OUT SDO_NUMBER_ARRAY,
    allow_iso_moves VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.moveEdge(
     java.lang.String,
     oracle.sql.NUMBER, oracle.sql.NUMBER, oracle.sql.NUMBER,
     oracle.spatial.type.SdoNumberArray,
     oracle.spatial.type.SdoNumberArray[],
     oracle.spatial.type.SdoNumberArray[],
     java.lang.String)';
 PROCEDURE remove_edge(topology VARCHAR2,
    edge_id NUMBER)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.removeEdge(
     java.lang.String, oracle.sql.NUMBER)';
 PROCEDURE change_edge_coords(topology VARCHAR2,
    edge_id NUMBER, geom SDO_GEOMETRY)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.changeEdgeCoords(
     java.lang.String, oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry)';
 PROCEDURE change_edge_coords(topology VARCHAR2,
    edge_id NUMBER, geom SDO_GEOMETRY,
    moved_iso_nodes OUT SDO_NUMBER_ARRAY,
    moved_iso_edges OUT SDO_NUMBER_ARRAY,
    allow_iso_moves VARCHAR2)
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.changeEdgeCoords(
     java.lang.String, oracle.sql.NUMBER,
     oracle.spatial.type.SdoGeometry,
     oracle.spatial.type.SdoNumberArray[],
     oracle.spatial.type.SdoNumberArray[],
     java.lang.String)';
 FUNCTION add_point_geometry(topology VARCHAR2,
    point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addPointGeometry(
     java.lang.String, oracle.spatial.type.SdoGeometry)
     return oracle.sql.NUMBER';
 FUNCTION add_point_geometry(topology VARCHAR2,
    coord SDO_NUMBER_ARRAY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addPointGeometry(
     java.lang.String, oracle.spatial.type.SdoNumberArray)
     return oracle.sql.NUMBER';
 FUNCTION add_linear_geometry(topology VARCHAR2,
    curve SDO_GEOMETRY) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addLinearGeometry(
     java.lang.String, oracle.spatial.type.SdoGeometry)
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION add_linear_geometry(topology VARCHAR2,
    coords SDO_NUMBER_ARRAY) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addLinearGeometry(
     java.lang.String, oracle.spatial.type.SdoNumberArray)
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION add_polygon_geometry(topology VARCHAR2,
    polygon SDO_GEOMETRY) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addPolygonGeometry(
     java.lang.String, oracle.spatial.type.SdoGeometry)
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION add_polygon_geometry(topology VARCHAR2,
    coords SDO_NUMBER_ARRAY) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.addPolygonGeometry(
      java.lang.String, oracle.spatial.type.SdoNumberArray)
     return oracle.spatial.type.SdoNumberArray';

 FUNCTION create_feature(topology VARCHAR2, table_name VARCHAR2,
    column_name VARCHAR2, geom SDO_GEOMETRY)
    RETURN SDO_TOPO_GEOMETRY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.createFeature(
      java.lang.String, java.lang.String, java.lang.String,
      oracle.spatial.type.SdoGeometry)
     return oracle.spatial.type.SdoTopoGeometry';
 FUNCTION create_feature(topology VARCHAR2, table_name VARCHAR2,
    column_name VARCHAR2, dml_condition VARCHAR2)
    RETURN SDO_TOPO_GEOMETRY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.createFeature(
      java.lang.String, java.lang.String, java.lang.String,
      java.lang.String)
     return oracle.spatial.type.SdoTopoGeometry';

 FUNCTION get_containing_face(topology VARCHAR2, topo_map VARCHAR2,
    point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getContainingFace(
     java.lang.String, java.lang.String,
     oracle.spatial.type.SdoGeometry)  return oracle.sql.NUMBER';

 FUNCTION get_containing_face(topology VARCHAR2, topo_map VARCHAR2,
    point SDO_GEOMETRY, allow_iso_coincidence varchar2)
   RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getContainingFace(
     java.lang.String, java.lang.String,
     oracle.spatial.type.SdoGeometry, java.lang.String)
    return oracle.sql.NUMBER';

 FUNCTION get_containing_face(topology VARCHAR2, topo_map VARCHAR2,
    x NUMBER, y NUMBER) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getContainingFace(
     java.lang.String, java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER) return oracle.sql.NUMBER';
 FUNCTION get_node_star(topology VARCHAR2, topo_map VARCHAR2,
    node_id NUMBER) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNodeStar(
     java.lang.String, java.lang.String, oracle.sql.NUMBER)
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_node_face_star(topology VARCHAR2, topo_map VARCHAR2,
    node_id NUMBER) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNodeFaceStar(
     java.lang.String, java.lang.String, oracle.sql.NUMBER)
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_edge_nodes(topology VARCHAR2, topo_map VARCHAR2,
    edge_id NUMBER) RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getEdgeNodes(
     java.lang.String, java.lang.String, oracle.sql.NUMBER)
     return oracle.spatial.type.SdoNumberArray';
 FUNCTION get_face_boundary(topology VARCHAR2, topo_map VARCHAR2,
    face_id NUMBER, opiton NUMBER)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getFaceBoundary(
     java.lang.String, java.lang.String, oracle.sql.NUMBER, oracle.sql.NUMBER)
     return oracle.spatial.type.SdoNumberArray';

 FUNCTION get_nearest_node(topology VARCHAR2, topo_map VARCHAR2,
    point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestNode(
     java.lang.String, java.lang.String, oracle.spatial.type.SdoGeometry)
     return oracle.sql.NUMBER';
 FUNCTION get_nearest_node(topology VARCHAR2, topo_map VARCHAR2,
    x NUMBER, y NUMBER) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestNode(
     java.lang.String, java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER) return oracle.sql.NUMBER';
 FUNCTION get_nearest_edge(topology VARCHAR2, topo_map VARCHAR2,
    point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestEdge(
     java.lang.String, java.lang.String, oracle.spatial.type.SdoGeometry)
     return oracle.sql.NUMBER';
 FUNCTION get_nearest_edge(topology VARCHAR2, topo_map VARCHAR2,
    x NUMBER, y NUMBER) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestEdge(
     java.lang.String, java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER) return oracle.sql.NUMBER';

 FUNCTION get_nearest_node_in_cache(topo_map VARCHAR2,
    x NUMBER, y NUMBER) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestNodeInCache(
     java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER) return oracle.sql.NUMBER';
 FUNCTION get_nearest_node_in_cache(topo_map VARCHAR2,
    point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestNodeInCache(
     java.lang.String, oracle.spatial.type.SdoGeometry)
    return oracle.sql.NUMBER';

 FUNCTION get_nearest_edge_in_cache(topo_map VARCHAR2,
    x NUMBER, y NUMBER) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestEdgeInCache(
     java.lang.String, oracle.sql.NUMBER,
     oracle.sql.NUMBER) return oracle.sql.NUMBER';
 FUNCTION get_nearest_edge_in_cache(topo_map VARCHAR2,
    point SDO_GEOMETRY) RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getNearestEdgeInCache(
     java.lang.String, oracle.spatial.type.SdoGeometry)
    return oracle.sql.NUMBER';

 FUNCTION get_topo_transaction_id RETURN NUMBER DETERMINISTIC
    AS LANGUAGE JAVA NAME
    'oracle.spatial.type.TopoMapWrapper.getTopoTransactionId()
     return oracle.sql.NUMBER';

END SDO_TOPO_MAP;
/


Prompt Synonym SDO_TOPO_MAP;
--
-- SDO_TOPO_MAP  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_TOPO_MAP FOR MDSYS.SDO_TOPO_MAP
/


Prompt Grants on PACKAGE SDO_TOPO_MAP TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_TOPO_MAP TO PUBLIC
/
