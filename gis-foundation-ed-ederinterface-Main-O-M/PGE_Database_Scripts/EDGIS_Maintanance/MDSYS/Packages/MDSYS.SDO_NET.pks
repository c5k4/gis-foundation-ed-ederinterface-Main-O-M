Prompt drop Package SDO_NET;
DROP PACKAGE MDSYS.SDO_NET
/

Prompt Package SDO_NET;
--
-- SDO_NET  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_net AUTHID current_user AS

  ---------------------------
  --CONSTANTS
  ---------------------------
  --Logging levels
  LOGGING_LEVEL_FATAL     CONSTANT INTEGER := 1;
  LOGGING_LEVEL_ERROR     CONSTANT INTEGER := 2;
  LOGGING_LEVEL_WARN      CONSTANT INTEGER := 3;
  LOGGING_LEVEL_INFO      CONSTANT INTEGER := 4;
  LOGGING_LEVEL_DEBUG     CONSTANT INTEGER := 5;
  LOGGING_LEVEL_FINEST    CONSTANT INTEGER := 6;

  ---------------------------
  --TYPES
  ---------------------------
  type ref_cursor is ref cursor;

  ---------------------------
  --SUBPROGRAMS
  ---------------------------
  -- check if the network exists in the network metadata
  FUNCTION network_exists(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(network_exists,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return the network ID from network name
  FUNCTION get_network_id(network IN VARCHAR2)
    RETURN NUMBER;
  PRAGMA RESTRICT_REFERENCES(get_network_id,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return  network name from network ID
  FUNCTION get_network_name(network_id IN NUMBER)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_network_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return  network name from network ID
  FUNCTION get_network_name(network_id IN NUMBER, owner OUT VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_network_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return  network owner from network ID
  FUNCTION get_network_owner(network_id IN NUMBER)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_network_owner,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return the type of network
  FUNCTION get_network_type(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_network_type,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return the type of network
  FUNCTION get_network_category(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_network_category,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return the type of geometry
  FUNCTION get_geometry_type(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_geometry_type,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return no of hierarchy levels
  FUNCTION get_no_of_hierarchy_levels(network IN VARCHAR2)
    RETURN NUMBER;
  PRAGMA RESTRICT_REFERENCES(get_no_of_hierarchy_levels,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: query partition table instead      --
  -----------------------------------------------------
  -- return no of partition
  FUNCTION get_no_of_partitions(network IN VARCHAR2,
                                link_level IN NUMBER DEFAULT 1)
    RETURN NUMBER;

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return geometry table(LRS) name of network
  FUNCTION get_lrs_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_lrs_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return LRS geom column of network
  FUNCTION get_lrs_geom_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_lrs_geom_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return node table name of network
  FUNCTION get_node_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_node_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return node geom column of network
  FUNCTION get_node_geom_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_node_geom_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return node cost column of network
  FUNCTION get_node_cost_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_node_cost_column,WNDS,WNPS,RNPS);

  ----------------------------------------------------------------------
  -- @deprecated: node partition column is no longer used since 11gR1.--
  ----------------------------------------------------------------------
  -- return node partition column of network
  FUNCTION get_node_partition_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_node_partition_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return link table name of network
  FUNCTION get_link_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_link_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return link geom column of network
  FUNCTION get_link_geom_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_link_geom_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return link direction of network
  FUNCTION get_link_direction(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_link_direction,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: link partition column is not used. --
  -----------------------------------------------------
  -- return link partition column of network
  FUNCTION get_link_partition_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_link_partition_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return link cost column of network
  FUNCTION get_link_cost_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_link_cost_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return path table name of network
  FUNCTION get_path_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_path_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return path geom column name of network
  FUNCTION get_path_geom_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_path_geom_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return path-link table name of network
  FUNCTION get_path_link_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_path_link_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return subpath table name of network
  FUNCTION get_subpath_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_subpath_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return subpath geom column name of network
  FUNCTION get_subpath_geom_column(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_subpath_geom_column,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return partition table name of network
  FUNCTION get_partition_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_partition_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return partition blob table name of network
  FUNCTION get_partition_blob_table_name(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_partition_blob_table_name,WNDS,WNPS,RNPS);

  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return if there is any user defined data in the metadata
  FUNCTION get_user_defined_data(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_user_defined_data,WNDS,WNPS,RNPS);


  -- return no of nodes of the network
  FUNCTION get_no_of_nodes(network IN VARCHAR2)
    RETURN NUMBER;

  -- return no of nodes of the network of specified hierarchy_level
  FUNCTION get_no_of_nodes(network IN VARCHAR2, hierarchy_level IN NUMBER)
    RETURN NUMBER;

  -- return no of links of the network
  FUNCTION get_no_of_links(network IN VARCHAR2)
    RETURN NUMBER;
--  PRAGMA RESTRICT_REFERENCES(get_no_of_links,WNDS,WNPS,RNPS);

  -- return no of links of the network of specified hierarchy_level
  FUNCTION get_no_of_links(network IN VARCHAR2, hierarchy_level IN NUMBER)
    RETURN NUMBER;

  -- return no of links of the given path
  FUNCTION get_no_of_links_in_path(network IN VARCHAR2, path_id IN NUMBER)
    RETURN NUMBER;

  -- return no of nodes of the given path
  FUNCTION get_no_of_nodes_in_path(network IN VARCHAR2, path_id IN NUMBER)
    RETURN NUMBER;

  -- return the in-link ids of a node
  FUNCTION get_in_links(network IN VARCHAR2, node_id in number)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC ;

  -- return the out-link ids of a node
  FUNCTION get_out_links(network IN VARCHAR2, node_id in number)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC;


  -- return the in-degree of a node
  FUNCTION get_node_in_degree(network IN VARCHAR2, node_id in number)
    RETURN NUMBER;

  -- return the out-degree of a node
  FUNCTION get_node_out_degree(network IN VARCHAR2, node_id in number)
    RETURN NUMBER;
--  PRAGMA RESTRICT_REFERENCES(get_node_out_degree,WNDS,WNPS,RNPS);

  -- return the degree of a node(in_degree+out_degree)
  FUNCTION get_node_degree(network IN VARCHAR2, node_id in number)
    RETURN NUMBER;


  -- return the child nodes of a given node
  FUNCTION get_child_nodes(network IN VARCHAR2, node_id in number)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC;

  -- return the child links of a given link
  FUNCTION get_child_links(network IN VARCHAR2, link_id in number)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC;

  -----------------
  -- @deprecated --
  -----------------
  -- return the path links of a given path
  FUNCTION get_links_in_path(network IN VARCHAR2, path_id in number)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC;

  -- return the path nodes of a given path
  FUNCTION get_nodes_in_path(network IN VARCHAR2, path_id in number)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC;


  --
  -- create xxx_network assumes the following naming convention
  -- table name length limitation: 32 char.
  -- network name length limitation : 24 char
  --

  -- default table/columns names:
  -- node table			: <network>_node$
  -- link table			: <network>_link$
  -- path table			: <network>_path$
  -- path link  table 		: <network>_plink$ (path_id, link_id)
  -- node geom. column  	: geometry
  -- node lrs geom column	: geom_id and measure
  -- node topo geom column	: topo_geometry
  -- link geom. column  	: geometry
  -- link lrs geom column	: geom_id, start_measure, and end_measure
  -- link topo geom column	: topo_geometry
  -- path geom. column  	: geometry
  -- node cost  column  	: cost
  -- link cost  column  	: cost
  -- path cost  column  	: cost
  -- subpath table name		: <network>_spath$
  -- subpath geom. column 	: geometry
  -- storage parameters         : for create table statement



  -- create a logical network
  PROCEDURE create_logical_network(network 			IN VARCHAR2,
				   no_of_hierarchy_levels 	IN NUMBER,
				   is_directed       		IN BOOLEAN,
				   node_table_name   		IN VARCHAr2,
   	                           node_cost_column  		IN VARCHAR2,
				   link_table_name   		IN VARCHAR2,
				   link_cost_column  		IN VARCHAR2,
				   path_table_name     		IN VARCHAR2,
				   path_link_table_name		IN VARCHAR2,
				   subpath_table_name		IN VARCHAR2,
				   is_complex		        IN BOOLEAN DEFAULT FALSE,
			   	   storage_parameters           IN VARCHAR2 DEFAULT '');

  -- create a logical network
  PROCEDURE create_logical_network(network 			IN VARCHAR2,
				   no_of_hierarchy_levels 	IN NUMBER,
				   is_directed       		IN BOOLEAN,
				   node_table_name   		IN VARCHAr2,
   	                           node_cost_column  		IN VARCHAR2,
				   link_table_name   		IN VARCHAR2,
				   link_cost_column  		IN VARCHAR2,
				   path_table_name     		IN VARCHAR2,
				   path_link_table_name		IN VARCHAR2,
				   is_complex		        IN BOOLEAN DEFAULT FALSE,
			   	   storage_parameters           IN VARCHAR2 DEFAULT '');

  -- create a logical network (simplified version)
  PROCEDURE create_logical_network(network 			IN VARCHAR2,
				   no_of_hierarchy_levels 	IN NUMBER,
				   is_directed 			IN BOOLEAN,
				   node_with_cost		IN BOOLEAN DEFAULT FALSE,
				   is_complex		        IN BOOLEAN DEFAULT FALSE,
			           storage_parameters           IN VARCHAR2 DEFAULT '');



  -- create a spatial (SDO_GEOMETRY) network
  PROCEDURE create_sdo_network(network 			IN VARCHAR2,
			       no_of_hierarchy_levels 	IN NUMBER,
			       is_directed 		IN BOOLEAN,
			       node_table_name		IN VARCHAR2,
			       node_geom_column		IN VARCHAR2,
			       node_cost_column		IN VARCHAR2,
			       link_table_name		IN VARCHAR2,
			       link_geom_column		IN VARCHAR2,
			       link_cost_column		IN VARCHAR2,
			       path_table_name		IN VARCHAR2,
			       path_geom_column		IN VARCHAR2,
			       path_link_table_name	IN VARCHAR2,
			       subpath_table_name	IN VARCHAR2,
  			       subpath_geom_column	IN VARCHAR2,
			       is_complex		IN BOOLEAN DEFAULT FALSE,
			       storage_parameters       IN VARCHAR2 DEFAULT '');

  -- create a spatial (SDO_GEOMETRY) network
  PROCEDURE create_sdo_network(network 			IN VARCHAR2,
			       no_of_hierarchy_levels 	IN NUMBER,
			       is_directed 		IN BOOLEAN,
			       node_table_name		IN VARCHAR2,
			       node_geom_column		IN VARCHAR2,
			       node_cost_column		IN VARCHAR2,
			       link_table_name		IN VARCHAR2,
			       link_geom_column		IN VARCHAR2,
			       link_cost_column		IN VARCHAR2,
			       path_table_name		IN VARCHAR2,
			       path_geom_column		IN VARCHAR2,
			       path_link_table_name	IN VARCHAR2,
			       is_complex		IN BOOLEAN DEFAULT FALSE,
			       storage_parameters       IN VARCHAR2 DEFAULT '');



  -- create a spatial (SDO_GEOMETRY) network
  PROCEDURE create_sdo_network(network IN VARCHAR2,
			       no_of_hierarchy_levels 	IN NUMBER,
			       is_directed 		IN BOOLEAN,
			       node_with_cost	 	IN BOOLEAN DEFAULT FALSE,
			       is_complex		IN BOOLEAN DEFAULT FALSE,
			       storage_parameters   	IN VARCHAR2 DEFAULT '');


  -- create a spatial (LRS_GEOMETRY) network
  PROCEDURE create_lrs_network(network 			IN VARCHAR2,
			       no_of_hierarchy_levels 	IN NUMBER,
			       is_directed 		IN BOOLEAN,
			       node_table_name		IN VARCHAR2,
			       node_cost_column		IN VARCHAR2,
			       link_table_name		IN VARCHAR2,
			       link_cost_column		IN VARCHAR2,
			       lrs_table_name  		IN VARCHAR2,
			       lrs_geom_column 		IN VARCHAR2,
			       path_table_name		IN VARCHAR2,
			       path_geom_column		IN VARCHAR2,
			       path_link_table_name	IN VARCHAR2,
			       subpath_table_name	IN VARCHAR2,
  			       subpath_geom_column	IN VARCHAR2,
			       is_complex		IN BOOLEAN DEFAULT FALSE,
			       storage_parameters   	IN VARCHAR2 DEFAULT '');
  -- create a spatial (LRS_GEOMETRY) network
  PROCEDURE create_lrs_network(network 			IN VARCHAR2,
			       no_of_hierarchy_levels 	IN NUMBER,
			       is_directed 		IN BOOLEAN,
			       node_table_name		IN VARCHAR2,
			       node_cost_column		IN VARCHAR2,
			       link_table_name		IN VARCHAR2,
			       link_cost_column		IN VARCHAR2,
			       lrs_table_name  		IN VARCHAR2,
			       lrs_geom_column 		IN VARCHAR2,
			       path_table_name		IN VARCHAR2,
			       path_geom_column		IN VARCHAR2,
			       path_link_table_name	IN VARCHAR2,
			       is_complex		IN BOOLEAN DEFAULT FALSE,
			       storage_parameters   	IN VARCHAR2 DEFAULT '');


  -- create a spatial (LRS_GEOMETRY) network
  PROCEDURE create_lrs_network(network 			IN VARCHAR2,
			       lrs_table_name  		IN VARCHAR2,
			       lrs_geom_column 		IN VARCHAR2,
			       no_of_hierarchy_levels 	IN NUMBER,
			       is_directed 		IN BOOLEAN,
                               node_with_cost 		IN BOOLEAN DEFAULT FALSE,
			       is_complex		IN BOOLEAN DEFAULT FALSE,
			       storage_parameters   	IN VARCHAR2 DEFAULT '');



  PROCEDURE create_topo_network(network IN VARCHAR2,
			        no_of_hierarchy_levels IN NUMBER,
			        is_directed 		IN BOOLEAN,
				node_table_name		IN VARCHAR2,
				node_geom_column	IN VARCHAR2,
			        node_cost_column	IN VARCHAR2,
			        link_table_name		IN VARCHAR2,
				link_geom_column	IN VARCHAR2,
			        link_cost_column	IN VARCHAR2,
			        path_table_name		IN VARCHAR2,
			        path_geom_column	IN VARCHAR2,
			        path_link_table_name	IN VARCHAR2,
				subpath_table_name	IN VARCHAR2,
  				subpath_geom_column	IN VARCHAR2,
			        is_complex		IN BOOLEAN DEFAULT FALSE,
			        storage_parameters   	IN VARCHAR2 DEFAULT '');

  -- create a spatial (TOPO_GEOMETRY) network
  PROCEDURE create_topo_network(network IN VARCHAR2,
			        no_of_hierarchy_levels IN NUMBER,
			        is_directed 		IN BOOLEAN,
				node_table_name		IN VARCHAR2,
				node_geom_column	IN VARCHAR2,
			        node_cost_column	IN VARCHAR2,
			        link_table_name		IN VARCHAR2,
				link_geom_column	IN VARCHAR2,
			        link_cost_column	IN VARCHAR2,
			        path_table_name		IN VARCHAR2,
			        path_geom_column	IN VARCHAR2,
			        path_link_table_name	IN VARCHAR2,
			        is_complex		IN BOOLEAN DEFAULT FALSE,
			        storage_parameters   	IN VARCHAR2 DEFAULT '');


  -- create a spatial (TOPO_GEOMETRY) network
  PROCEDURE create_topo_network(network IN VARCHAR2,
			        no_of_hierarchy_levels 	IN NUMBER,
			        is_directed 		IN BOOLEAN,
				node_table_name		IN VARCHAR2,
			        node_cost_column	IN VARCHAR2,
			        link_table_name		IN VARCHAR2,
			        link_cost_column	IN VARCHAR2,
			        path_table_name		IN VARCHAR2,
			        path_geom_column	IN VARCHAR2,
			        path_link_table_name	IN VARCHAR2,
			        is_complex		IN BOOLEAN DEFAULT FALSE,
			        storage_parameters   	IN VARCHAR2 DEFAULT '');



  -- create a spatial (TOPO_GEOMETRY) network
  PROCEDURE create_topo_network(network 		IN VARCHAR2,
			        no_of_hierarchy_levels 	IN NUMBER,
			        is_directed 		IN BOOLEAN,
				node_with_cost 		IN BOOLEAN DEFAULT FALSE,
			        is_complex		IN BOOLEAN DEFAULT FALSE,
			        storage_parameters   	IN VARCHAR2 DEFAULT '');



  -- create a spatial (TOPO_GEOMETRY) network from existing topology data
  -- The nodes and links in the resultant network have one-to-one mapping
  -- to the nodes and edges in the given topology
  PROCEDURE create_topo_network(network                  in varchar2,
                                no_of_hierarchy_levels   in number,
                                is_directed              in boolean,
                                node_with_cost           in boolean,
                                is_complex               in boolean,
                                topology                 in varchar2,
			        storage_parameters   	 in varchar2 );

  -----------------
  -- @deprecated --
  -----------------
  PROCEDURE create_network(network 			IN VARCHAR2,
		           geom_type   			IN VARCHAR2,
			   no_of_hierarchy_levels 	IN NUMBER,
			   is_directed 			IN BOOLEAN,
			   node_table_name 		IN VARCHAR2,
			   node_geom_column 		IN VARCHAR2,
			   node_cost_column		IN VARCHAR2,
			   link_table_name 		IN VARCHAR2,
			   link_geom_column 		IN VARCHAR2,
			   link_cost_column		IN VARCHAR2,
			   lrs_table_name   		IN VARCHAR2,
			   lrs_geom_column  		IN VARCHAR2,
			   path_table_name		IN VARCHAR2,
			   path_geom_column		IN VARCHAR2,
			   path_link_table_name		IN VARCHAR2,
			   subpath_table_name		IN VARCHAR2,
  			   subpath_geom_column		IN VARCHAR2,
			   is_complex			IN BOOLEAN DEFAULT FALSE,
			   storage_parameters   	IN VARCHAR2 DEFAULT '');

  -----------------
  -- @deprecated --
  -----------------
  -- create a general network creation
  PROCEDURE create_network(network 			IN VARCHAR2,
		           geom_type   			IN VARCHAR2,
			   no_of_hierarchy_levels 	IN NUMBER,
			   is_directed 			IN BOOLEAN,
			   node_table_name 		IN VARCHAR2,
			   node_geom_column 		IN VARCHAR2,
			   node_cost_column		IN VARCHAR2,
			   link_table_name 		IN VARCHAR2,
			   link_geom_column 		IN VARCHAR2,
			   link_cost_column		IN VARCHAR2,
			   lrs_table_name   		IN VARCHAR2,
			   lrs_geom_column  		IN VARCHAR2,
			   path_table_name		IN VARCHAR2,
			   path_geom_column		IN VARCHAR2,
			   path_link_table_name		IN VARCHAR2,
			   is_complex			IN BOOLEAN DEFAULT FALSE,
			   storage_parameters   	IN VARCHAR2 DEFAULT '');


  -----------------
  -- @deprecated --
  -----------------
  -- create a general network creation
  PROCEDURE create_network(network 			IN VARCHAR2,
		           geom_type   			IN VARCHAR2,
			   no_of_hierarchy_levels 	IN NUMBER,
			   is_directed 			IN BOOLEAN,
			   node_with_cost 		IN BOOLEAN,
			   lrs_table_name   		IN VARCHAR2,
			   lrs_geom_column  		IN VARCHAR2,
			   is_complex			IN BOOLEAN DEFAULT FALSE,
			   storage_parameters   	IN VARCHAR2 DEFAULT '');


  -- validate a network
  FUNCTION validate_network(network IN VARCHAR2, check_data IN VARCHAR2 default 'FALSE')
    RETURN VARCHAR2;

  -- validate the table consistency of a network
  FUNCTION validate_consistency(network IN VARCHAR2, is_fast IN VARCHAR2 default 'TRUE')
    RETURN VARCHAR2;

  -- create an empty  node table with necessary columns
  PROCEDURE create_node_table(table_name  		in varchar2,
			      geom_type   		in varchar2,
			      geom_column 		in varchar2,
			      cost_column 		in varchar2,
			      partition_column 		in varchar2,
			      no_of_hierarchy_levels 	in number,
			      is_complex 		in boolean default false,
			      storage_parameters   	in varchar2 default '');


  -- create an empty  node table with necessary columns
  PROCEDURE create_node_table(table_name  		in varchar2,
			      geom_type   		in varchar2,
			      geom_column 		in varchar2,
			      cost_column 		in varchar2,
			      no_of_hierarchy_levels 	in number,
			      is_complex 		in boolean default false,
			      storage_parameters   	in varchar2 default '');



  -- create an empty link table with necessary columns
  PROCEDURE create_link_table(table_name  		in varchar2,
			      geom_type   		in varchar2,
			      geom_column 		in varchar2,
			      cost_column 		in varchar2,
			      no_of_hierarchy_levels 	in number,
			      add_bidirected_column 	in boolean default false,
			      storage_parameters   	in varchar2 default '');

  -- create an empty compact path table with necessary columns
  PROCEDURE create_path_table(table_name in varchar2,  geom_column in varchar2,
			      storage_parameters   IN VARCHAR2 DEFAULT '');
  -- create an empty compact subpath table with necessary columns
  PROCEDURE create_subpath_table(table_name in varchar2,  geom_column in varchar2,
			      storage_parameters   IN VARCHAR2 DEFAULT '');

  -- create an empty compact path table with necessary columns
  PROCEDURE create_path_link_table(table_name in varchar2,
			      	   storage_parameters   IN VARCHAR2 DEFAULT '');

  -- create an empty lrs geom table with necessary columns
  PROCEDURE create_lrs_table(table_name in varchar2,  geom_column in varchar2,
			      storage_parameters   IN VARCHAR2 DEFAULT '');

  -- create an empty partition table with necessary columns
  PROCEDURE create_partition_table(table_name in varchar2);

  -- create an empty partition blob table with necessary schema
  PROCEDURE create_partition_blob_table(table_name in varchar2);

  -- create an empty component table with necessary schema
  PROCEDURE create_component_table(table_name in varchar2);

  -- validate a node schema
  FUNCTION validate_node_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  -- validate a link schema
  FUNCTION validate_link_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  -- validate a path schema
  FUNCTION validate_path_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  -- validate a subpath schema
  FUNCTION validate_subpath_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  -- validate an lrs geom schema
  FUNCTION validate_lrs_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  -- validate a partition table schema
  FUNCTION validate_partition_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  -- validate necessary partition blob table schema and blob content
  FUNCTION validate_partition_blob_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  -- validate necessary connected component schema
  FUNCTION validate_component_schema(network IN VARCHAR2)
    RETURN VARCHAR2;

  --
  -- drop a network
  -- it drops :
  -- node table, link table, and path table
  -- network metadata
  --

  PROCEDURE drop_network(network	in varchar2);

  --
  -- create a point geometry from an LRS geometry and a measure
  --
  FUNCTION get_lrs_node_geometry(network	in VARCHAR2,
			         node_id	in number)
	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;

  --
  -- create a point geometry from a topology
  --
  FUNCTION get_topo_node_geometry(network	in VARCHAR2,
			          node_id	in number)
	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;


  --
  -- get the node geometry
  --
  FUNCTION get_node_geometry(network	in VARCHAR2,
			     node_id	in number)
	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;


  --
  -- create linestring geometry from an LRS geometry and start/end measure
  --
  FUNCTION get_lrs_link_geometry(network	in VARCHAR2,
			         link_id	in number)

	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;

  --
  -- create linestring geometry from a topo geometry
  --
  FUNCTION get_topo_link_geometry(network	in VARCHAR2,
			          link_id	in number)

	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;


  --
  -- get the link geometry
  --
  FUNCTION get_link_geometry(network	in VARCHAR2,
			     link_id	in number,
			     start_percentage IN NUMBER default 0,
			     end_percentage   IN NUMBER default 1.0)

	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;



  --
  -- network type queries
  --
  FUNCTION is_spatial(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(is_spatial,WNDS,WNPS,RNPS);

  FUNCTION is_logical(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(is_logical,WNDS,WNPS,RNPS);

  FUNCTION is_hierarchical(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(is_hierarchical,WNDS,WNPS,RNPS);

  -----------------
  -- @deprecated --
  -----------------
  -- a simple network does not refer nodes/links in another networks
  -- a complex network has the network_type anything other than  'COMPLEX'
  -- before 10GR1, all networks are simple networks
  FUNCTION is_simple(network IN VARCHAR2)
    RETURN VARCHAR2;

  -----------------
  -- @deprecated --
  -----------------
  -- a complex network can refer nodes/links in another networks
  -- a complex network has the network_type = 'COMPLEX'
  -- after 10GR2, a network can refer nodes/links in other networks (network_id, and element_id)
  FUNCTION is_complex(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(is_complex,WNDS,WNPS,RNPS);

  FUNCTION is_simple(network IN VARCHAR2,path_id in number)
    RETURN VARCHAR2;

  FUNCTION is_link_in_path(network IN VARCHAR2,path_id in number, link_id in number)
    RETURN VARCHAR2;

  FUNCTION is_node_in_path(network IN VARCHAR2,path_id in number,node_id in number)
    RETURN VARCHAR2;


  FUNCTION sdo_geometry_network(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(sdo_geometry_network,WNDS,WNPS,RNPS);

  FUNCTION lrs_geometry_network(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(lrs_geometry_network,WNDS,WNPS,RNPS);

  FUNCTION topo_geometry_network(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(topo_geometry_network,WNDS,WNPS,RNPS);



  --
  -- copy network structure
  --

 PROCEDURE copy_network(source_network 	in varchar2,
		        target_network  in varchar2,
			storage_parameters in varchar2 default '');


  --
  -- create delete trigger on node/link table
  --

 PROCEDURE create_delete_trigger(network in varchar2);


  -- return the hierarchy_level of a node
  FUNCTION get_node_hierarchy_level(network IN VARCHAR2, node_id in number)
    RETURN NUMBER;


  --
  -- add referential constraints
  --

  PROCEDURE create_ref_constraints(network IN VARCHAR2);

  --
  -- drop referential constraints
  --

  PROCEDURE drop_ref_constraints(network IN VARCHAR2);

  --
  -- enable referential constraints
  --

  PROCEDURE enable_ref_constraints(network IN VARCHAR2);


  --
  -- disable referential constraints
  --

  PROCEDURE disable_ref_constraints(network IN VARCHAR2);



  --
  -- add an geometry metadata entry
  --

  PROCEDURE insert_geom_metadata(geom_table_name       IN VARCHAR2,
                                 geom_column_name      IN VARCHAR2,
                                 diminfo               IN MDSYS.SDO_DIM_ARRAY,
                                 srid                  IN NUMBER);
  PRAGMA RESTRICT_REFERENCES(insert_geom_metadata,WNPS,RNPS);


  --
  -- add geometry metadata entries for a spatial network
  --

  PROCEDURE insert_geom_metadata(network 	       IN VARCHAR2,
                                 diminfo               IN MDSYS.SDO_DIM_ARRAY,
                                 srid                  IN NUMBER);
  PRAGMA RESTRICT_REFERENCES(insert_geom_metadata,WNPS,RNPS);



  --
  -- return the ids of isolated nodes
  --

  FUNCTION get_isolated_nodes(network IN VARCHAR2)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC ;

  --
  -- return the ids of invalid nodes
  --

  FUNCTION get_invalid_nodes(network IN VARCHAR2)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC ;


  --
  -- return the ids of invalid links
  --

  FUNCTION get_invalid_links(network IN VARCHAR2)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC ;

  --
  -- return the ids of invalid paths
  --

  FUNCTION get_invalid_paths(network IN VARCHAR2)
    RETURN SDO_NUMBER_ARRAY DETERMINISTIC ;


  -----------------
  -- @deprecated --
  -----------------
  --
  -- add path-link information to the path-link table
  -- the seq no is indicated by the order in the given link array
  --
  PROCEDURE insert_path_link_info(network 	        IN VARCHAR2,
				  path_id		IN NUMBER,
     	                          links			IN SDO_NUMBER_ARRAY,
        	                  is_simple		IN BOOLEAN DEFAULT TRUE) ;


  --
  -- delete a node from a network
  --
  --
  PROCEDURE delete_node(network 	IN VARCHAR2,
			node_id		IN NUMBER) ;

  --
  -- delete a link from a network
  --
  --
  PROCEDURE delete_link(network 	IN VARCHAR2,
			link_id		IN NUMBER) ;


  --
  -- delete a path from a network
  --
  --
  PROCEDURE delete_path(network 	IN VARCHAR2,
			path_id		IN NUMBER) ;


  --
  -- delete a subpath from a network
  --
  --
  PROCEDURE delete_subpath(network 	IN VARCHAR2,
			   subpath_id	IN NUMBER) ;

  -----------------
  -- @deprecated --
  -----------------
  --
  -- change a spatial network into a logical network (Metadata Level)
  --
  --
  PROCEDURE switch_to_logical_network(network 	IN VARCHAR2);
  PRAGMA RESTRICT_REFERENCES(switch_to_logical_network,WNPS,RNPS);

  -----------------
  -- @deprecated --
  -----------------
  --
  -- change a logical network into a spatial network (Metadata Level)
  --
  --
  PROCEDURE switch_to_spatial_network(network 	    IN VARCHAR2,
				      node_geom_col IN VARCHAR2,
				      link_geom_col IN VARCHAR2,
				      path_geom_col IN VARCHAR2,
				      subpath_geom_col IN VARCHAR2);


  -----------------------------------------------------
  -- @deprecated: use network metadata views instead --
  -----------------------------------------------------
  -- return the topology of a topology network
  FUNCTION get_topology(network IN VARCHAR2)
    RETURN VARCHAR2;
  PRAGMA RESTRICT_REFERENCES(get_topology,WNDS,WNPS,RNPS);

  -- compute the geometry of a path from its constituent link geometries
  FUNCTION compute_path_geometry(network IN VARCHAR2,
                                 path_id IN NUMBER,
                                 tolerance IN NUMBER)
    RETURN MDSYS.SDO_GEOMETRY;

  -- Partition the nodes in the network and write the result into the
  -- partition table.
  PROCEDURE spatial_partition(network IN VARCHAR2,
			      partition_table_name IN VARCHAR2,
	    	      	      max_num_nodes IN NUMBER,
			      log_loc	IN VARCHAR2,
			      log_file	IN VARCHAR2,
			      open_mode IN VARCHAR2 DEFAULT 'a',
			      link_level IN INTEGER DEFAULT 1);

  -- Generate or regenerate blobs for all the partitions.
  -- @param regenerate_node_levels whether to overwrite existing node level table.
  --                               Set to true if there are nodes added/deleted to/from second or higher
  --                               link levels, since the last time blobs were generated.
  PROCEDURE generate_partition_blobs(
                              network                   IN VARCHAR2,
			      link_level                IN NUMBER DEFAULT 1,
			      partition_blob_table_name IN VARCHAR2,
			      include_user_data         IN BOOLEAN,
                              commit_for_each_blob      IN BOOLEAN DEFAULT true,
			      log_loc	IN VARCHAR2,
			      log_file	IN VARCHAR2,
			      open_mode IN VARCHAR2 DEFAULT 'a',
			      perform_delta_update IN BOOLEAN DEFAULT false,
                              regenerate_node_levels IN BOOLEAN DEFAULT false);

  -- generate or regenerate the blob for the specified partition
  -- into the partition blob table
  -- assumption: partition blob table already exists
  PROCEDURE generate_partition_blob(
                              network           IN VARCHAR2,
			      link_level        IN NUMBER DEFAULT 1,
			      partition_id      IN NUMBER,
			      include_user_data IN BOOLEAN,
			      log_loc	IN VARCHAR2,
			      log_file	IN VARCHAR2,
			      open_mode IN VARCHAR2 DEFAULT 'a',
			      perform_delta_update IN BOOLEAN DEFAULT false);

  -- Find connected components for the network on the specified link level,
  -- and store node_id component_id relation into the component table.
  PROCEDURE find_connected_components(
                              network              IN VARCHAR2,
			      link_level           IN NUMBER DEFAULT 1,
			      component_table_name IN VARCHAR2,
			      log_loc	IN VARCHAR2,
			      log_file	IN VARCHAR2,
			      open_mode IN VARCHAR2 DEFAULT 'a');

  -- Sets the maximum java heap size for the java stored procedure.
  PROCEDURE set_max_java_heap_size(bytes NUMBER);

  -- Sets the logging level of the java stored procedure.
  PROCEDURE set_logging_level( level     IN NUMBER );

  -- Loads the configuration xml for the java stored procedure.
  PROCEDURE load_config( file_directory  IN VARCHAR2 ,
			 file_name       IN VARCHAR2 );

  -- return the partition size in bytes
  FUNCTION get_partition_size(network IN VARCHAR2,
			      partition_id IN NUMBER,
			      link_level IN NUMBER DEFAULT 1,
			      include_user_data IN VARCHAR2 DEFAULT 'FALSE',
			      include_spatial_data IN VARCHAR2 DEFAULT 'FALSE')
  RETURN NUMBER;

  --- register given network constraint into user_sdo_network_constraints
  PROCEDURE register_constraint(
    constraint_name VARCHAR2, class_name  VARCHAR2,
    directory_name VARCHAR2,  description VARCHAR2);

  --- deregister given constraint from user_sdo_network_constraints
  PROCEDURE deregister_constraint(
    constraint_name  VARCHAR2);

  -- clear the content of the given network's related history tables
  PROCEDURE db_sync_clear(network IN VARCHAR2);

  -- disable database synchronization option for the given network
  PROCEDURE db_sync_disable(network IN VARCHAR2);

  -- enable database synchronization option for the given network
  PROCEDURE db_sync_enable(network IN VARCHAR2, base_node_table IN VARCHAR2 default NULL,
    base_link_table IN VARCHAR2 default NULL);

  -- for the given network and link level, get updates between the given
  -- timestamp and the current largest timestamp recorded
  FUNCTION db_sync_get_update(network IN VARCHAR2, last_get_time  IN TIMESTAMP,
    node_change_history OUT SDO_NET_UPD_HIST_NTBL,
    link_change_history OUT SDO_NET_UPD_HIST_TBL)
    RETURN TIMESTAMP;

  -- the variant using the cursor to facilitate the calls made by JDBC
  FUNCTION db_sync_get_update(network IN VARCHAR2, last_get_time  IN TIMESTAMP,
    node_change_history OUT ref_cursor,
    link_change_history OUT ref_cursor)
    RETURN TIMESTAMP;

  --- register given java object into user_sdo_network_java_objects
  PROCEDURE register_java_object(
    name VARCHAR2, class_name  VARCHAR2,
    directory_name VARCHAR2,  description VARCHAR2, java_interface VARCHAR2);

  --- deregister given java object  from user_sdo_network_java_objects
  PROCEDURE deregister_java_object(
    name  VARCHAR2);
  --- validate lod partition information
  FUNCTION validate_partition_info(network IN VARCHAR2,check_data IN VARCHAR2 default 'FALSE')
    RETURN VARCHAR2;

  -- return the percentage of the given pt geometry from the start of the linestring geometry
  -- if the pt geometry is not on the link geometry, the nearest pt on the link geometry is used
  -- percentage is between [0,1]
  FUNCTION get_percentage(network IN VARCHAR2, link_id IN NUMBER, pt_geom IN mdsys.sdo_geometry)
    RETURN NUMBER;


  -- return the pt geometry on the givne link geometry based on the given percentage
  -- if the pt geometry is not on the link geometry, the nearest pt on the link geometry is used
  -- valid percentage is between [0,1]
  FUNCTION get_pt(network IN VARCHAR2, link_id IN NUMBER, percentage IN NUMBER)
	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;

  -- return the geometry on the givne link geometry based on the given percentages
  -- valid percentage is between [0,1]
  FUNCTION get_geometry(network IN VARCHAR2, link_id IN NUMBER, start_percentage IN NUMBER, end_percentage IN NUMBER)
	RETURN MDSYS.SDO_GEOMETRY DETERMINISTIC;

  -- Generates node levels for the specified multi-level network.
  -- If the the input network is single level, do nothing.
  -- @param overwrite: whether to overwrite existing node level table or not
  PROCEDURE generate_node_levels(
    network   in varchar2,
    node_level_table_name in varchar2,
    overwrite in boolean default false,
    log_loc   IN VARCHAR2,
    log_file  IN VARCHAR2,
    open_mode IN VARCHAR2 DEFAULT 'a');

  -- Make sure the data in the newwork tables are consistent.
  PROCEDURE update_consistency(network in VARCHAR2);

  -- create the logical network partitions on node table
  PROCEDURE logical_partition(network IN VARCHAR2,
			      partition_table_name IN VARCHAR2,
	    	      	      max_num_nodes IN NUMBER,
			      log_loc	IN VARCHAR2,
			      log_file	IN VARCHAR2,
			      open_mode IN VARCHAR2 DEFAULT 'a',
			      link_level IN INTEGER DEFAULT 1);

  PROCEDURE logical_powerlaw_partition(network IN VARCHAR2,
                              partition_table_name IN VARCHAR2,
                              max_num_nodes  IN NUMBER,
                              log_loc   IN VARCHAR2,
                              log_file  IN VARCHAR2,
                              open_mode IN VARCHAR2 default 'a',
                              link_level IN INTEGER default 1);
END sdo_net;
/


Prompt Synonym SDO_NET;
--
-- SDO_NET  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_NET FOR MDSYS.SDO_NET
/


Prompt Grants on PACKAGE SDO_NET TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_NET TO PUBLIC
/
