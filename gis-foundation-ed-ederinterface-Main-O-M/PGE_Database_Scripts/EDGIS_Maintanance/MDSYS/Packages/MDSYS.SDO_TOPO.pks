Prompt drop Package SDO_TOPO;
DROP PACKAGE MDSYS.SDO_TOPO
/

Prompt Package SDO_TOPO;
--
-- SDO_TOPO  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_TOPO AUTHID current_user AS

 -- In memory representation of USER_SDO_TOPO_INFO
 TYPE topo_metadata IS RECORD
 (
   owner            VARCHAR2(32),
   topology         VARCHAR2(20),
   topology_id      NUMBER,
   tolerance        NUMBER,
   srid             NUMBER,
   table_schema     VARCHAR2(64),
   table_name       VARCHAR2(62),
   column_name      VARCHAR2(32),
   tg_layer_id      NUMBER,
   tg_layer_type    VARCHAR2(10),
   tg_layer_level   NUMBER,
   child_layer_id   NUMBER
 );

 PROCEDURE create_topology (Topology  IN varchar2, tolerance IN NUMBER,
                        SRID IN NUMBER default NULL,
                        node_table_storage in varchar2 DEFAULT NULL,
                        edge_table_storage in varchar2 DEFAULT NULL,
                        face_table_storage in varchar2 DEFAULT NULL,
                        history_table_storage in varchar2 DEFAULT NULL,
                        Digits_Right_Of_Decimal in number default 16,
                        Topology_ID in number default NULL );

 PROCEDURE drop_topology (Topology  IN varchar2);

 PROCEDURE add_topo_geometry_layer (Topology             IN varchar2,
                        Table_Name           IN varchar2,
                        Column_Name          IN varchar2,
                        Topo_Geometry_Layer_Type   IN varchar2,
                        Relation_table_storage in varchar2 DEFAULT NULL,
                        Child_Layer_id in number DEFAULT NULL,
                        Layer_ID in number default NULL);

 PROCEDURE delete_topo_geometry_layer (Topology     IN varchar2,
                           Table_Name   IN varchar2,
                           Column_Name  IN varchar2);

 PROCEDURE initialize_metadata (Topology IN varchar2);

 PROCEDURE prepare_for_export (Topology IN varchar2);

 PROCEDURE initialize_after_import (Topology IN varchar2,
                      tg_layer_owner IN varchar2 default NULL);

/*
 --
 -- For a given sdo_topo_geometry, this procedure gets its boundary and
 -- interior.
 --
 PROCEDURE get_feature_properties
 (
   mtdt      IN  TOPO_METADATA,
   tg_id     IN  NUMBER,                                --- feature id
   tg_type   IN  NUMBER,                                --- feature type
   nd_bdy    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- boundary nodes
   nd_int    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- interior nodes
   eg_bdy    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- boundary edges
   eg_int    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- interior edges
   fc_int    OUT NOCOPY SDO_TOPO_NSTD_TBL               --- interior faces
 );

 PROCEDURE get_topo_properties
 (
   Topology   IN VARCHAR2,
   topo_elem_array   IN   SDO_TOPO_OBJECT_ARRAY,
   tg_type   IN  NUMBER,                                --- feature type
   nd_bdy    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- boundary nodes
   nd_int    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- interior nodes
   eg_bdy    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- boundary edges
   eg_int    OUT NOCOPY SDO_TOPO_NSTD_TBL,              --- interior edges
   fc_int    OUT NOCOPY SDO_TOPO_NSTD_TBL               --- interior faces
 );
*/

 FUNCTION get_interacting_tgids
  (
   base_layer_id  IN   NUMBER,             --- base layer id
   q_feature      IN   SDO_TOPO_GEOMETRY,  --- the query feature
   mask           IN   VARCHAR2            --- the query
  )
 RETURN SDO_LIST_TYPE DETERMINISTIC;

 FUNCTION get_interacting_tgids
  (
   tp_id            IN NUMBER,             -- topology id
   base_layer_id    IN   NUMBER,             --- base layer id
   topo_elem_array  IN   SDO_TOPO_OBJECT_ARRAY,  --- the query topo elements
   mask             IN   VARCHAR2            --- the query
  )
 RETURN SDO_LIST_TYPE DETERMINISTIC;

 FUNCTION relate (
  geom1 IN SDO_TOPO_GEOMETRY, geom2 IN SDO_TOPO_GEOMETRY, mask  IN VARCHAR2
 ) RETURN VARCHAR2 DETERMINISTIC;

 FUNCTION relate (
  geom IN SDO_TOPO_GEOMETRY, topo_elem_array IN SDO_TOPO_OBJECT_ARRAY,
                            mask  IN VARCHAR2
 ) RETURN VARCHAR2 DETERMINISTIC;

/*
 -- an overloading version of relate
 FUNCTION relate(
   geom1        IN  SDO_TOPO_GEOMETRY,
   nd_bdy_2     IN  SDO_TOPO_NSTD_TBL,
   nd_int_2     IN  SDO_TOPO_NSTD_TBL,
   eg_bdy_2     IN  SDO_TOPO_NSTD_TBL,
   eg_int_2     IN  SDO_TOPO_NSTD_TBL,
   fc_int_2     IN  SDO_TOPO_NSTD_TBL,
   mask         IN  VARCHAR2
 ) RETURN VARCHAR2 DETERMINISTIC;

 -- an overloading version of relate
 FUNCTION relate
 (
   mtdt      IN TOPO_METADATA,
   tg_id     IN NUMBER,
   tg_type   IN NUMBER,
   nd_bdy    IN SDO_TOPO_NSTD_TBL,              --- geom2: boundary nodes
   nd_int    IN SDO_TOPO_NSTD_TBL,              ---        interior nodes
   eg_bdy    IN SDO_TOPO_NSTD_TBL,              ---        boundary edges
   eg_int    IN SDO_TOPO_NSTD_TBL,              ---        interior edges
   fc_int    IN SDO_TOPO_NSTD_TBL,              ---        interior faces
   mask      IN VARCHAR2
 ) RETURN VARCHAR2 DETERMINISTIC;

 -- another overloading version of relate
 FUNCTION relate (
   tsname    IN VARCHAR2,             tabname   IN VARCHAR2,
   colname   IN VARCHAR2,             rowid     IN VARCHAR2,
   nd_bdy    IN SDO_TOPO_NSTD_TBL,    nd_int    IN SDO_TOPO_NSTD_TBL,
   eg_bdy    IN SDO_TOPO_NSTD_TBL,    eg_int    IN SDO_TOPO_NSTD_TBL,
   fc_int    IN SDO_TOPO_NSTD_TBL,    mask      IN VARCHAR2
 ) RETURN VARCHAR2 DETERMINISTIC;

*/
 -- another overloaded version of relate
  FUNCTION relate (
   feature1 IN sdo_topo_geometry, feature2 IN sdo_geometry, mask IN VARCHAR2
  ) RETURN VARCHAR2 DETERMINISTIC;

 FUNCTION get_face_boundary (Topology IN varchar2, face_id IN number,
                             All_Edges IN varchar2 DEFAULT 'FALSE')
 RETURN SDO_LIST_TYPE DETERMINISTIC;

 FUNCTION get_topo_objects(Topology IN varchar2,
                       topo_geometry_layer_id IN number,
                       topo_geometry_id IN number )
 RETURN SDO_TOPO_OBJECT_ARRAY DETERMINISTIC;


 FUNCTION get_topo_objects(Topology IN varchar2, geometry IN SDO_GEOMETRY)
 RETURN SDO_TOPO_OBJECT_ARRAY DETERMINISTIC;

 FUNCTION internal_get_tgids(Topology IN varchar2, geometry IN SDO_GEOMETRY,
                           in_tg_layer_id IN NUMBER DEFAULT NULL,
                            operator IN varchar2)
 RETURN SDO_LIST_TYPE DETERMINISTIC;

 FUNCTION internal_get_tgids(Topology IN varchar2,
                             q_tg_layer_id IN NUMBER, q_tg_id in number,
                             in_tg_layer_id IN NUMBER DEFAULT NULL,
                             operator IN varchar2)
 RETURN SDO_LIST_TYPE DETERMINISTIC;

 Function tg_insert_relation (topology varchar2, tg_layer_id number,
                              topo_ids SDO_TOPO_OBJECT_ARRAY, gtype number,
                              input_tg_id number, input_tg_attribute varchar2)
 return NUMBER;

 Function tg_insert_relation (topology varchar2, tg_layer_id number,
                              topo_ids SDO_TGL_OBJECT_ARRAY, gtype number,
                              input_tg_id number, input_tg_attribute varchar2,
                              child_lid number)
 return NUMBER;

 PROCEDURE sdo_topo_crt_rlids_view(Topology in varchar2, topo_id number);

 PROCEDURE lockRow_UniverseFace(topologyid in number);

 FUNCTION align_linear_geometry(geometry1 SDO_GEOMETRY, geometry2 SDO_GEOMETRY,
                                              tolerance number )
 RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION align_edge_geometry(geometry1 SDO_GEOMETRY, geometry2 SDO_GEOMETRY,
                                              tolerance number, dgts number)
 RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

 FUNCTION align_point_geometry(geometry1 SDO_GEOMETRY, geometry2 SDO_GEOMETRY,
                                           tolerance number)
 RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE ;

 FUNCTION get_internal_geometry(geom sdo_geometry,
                          nodeIds sdo_number_array, dim sdo_dim_array)
 RETURN SDO_GEOMETRY DETERMINISTIC PARALLEL_ENABLE;

END SDO_TOPO;
/


Prompt Synonym SDO_TOPO;
--
-- SDO_TOPO  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_TOPO FOR MDSYS.SDO_TOPO
/


Prompt Grants on PACKAGE SDO_TOPO TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_TOPO TO PUBLIC
/
