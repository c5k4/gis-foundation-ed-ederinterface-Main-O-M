Prompt drop Package SDO_ROUTER_PARTITION;
DROP PACKAGE MDSYS.SDO_ROUTER_PARTITION
/

Prompt Package SDO_ROUTER_PARTITION;
--
-- SDO_ROUTER_PARTITION  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_ROUTER_PARTITION AUTHID current_user AS
  FUNCTION adjust_m(start_m IN NUMBER, end_m IN NUMBER, m IN NUMBER)
    RETURN NUMBER;

  FUNCTION get_pid(m IN NUMBER, pid IN NUMBER)
    RETURN NUMBER;

  FUNCTION min_eigenvector(sum_x2 IN NUMBER, sum_y2 IN NUMBER, sum_xy IN NUMBER)
    RETURN mdsys.vector_2d;

  PROCEDURE cleanup_router(all_tables IN BOOLEAN DEFAULT TRUE);

  PROCEDURE partition_router(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                        max_v_no IN NUMBER DEFAULT 10000,
                        driving_side IN VARCHAR2 := 'R',
                        network_name IN VARCHAR := 'NDM_US',
                        cleanup IN BOOLEAN DEFAULT TRUE);

  PROCEDURE create_trucking_user_data(
                      log_file_name IN VARCHAR2 := 'sdo_router_partition.log');

  PROCEDURE dump_partitions(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                      start_pid IN NUMBER DEFAULT 0,
                      end_pid IN NUMBER DEFAULT -1,
                      verbose IN BOOLEAN DEFAULT FALSE);

  PROCEDURE validate_partitions(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                      start_pid IN NUMBER DEFAULT 0,
                      end_pid IN NUMBER DEFAULT -1,
                      verbose IN BOOLEAN DEFAULT FALSE);

  PROCEDURE get_version(log_file_name IN VARCHAR2 := 'sdo_router_partition.log');

  PROCEDURE create_router_network(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                                  network_name  IN VARCHAR2 := 'NDM_US');

  PROCEDURE delete_router_network(log_file_name IN VARCHAR2 := 'sdo_router_partition.log',
                                  network_name  IN VARCHAR2 := 'NDM_US');

  PROCEDURE elocation_partition_router (logfile_name in VARCHAR2) AS LANGUAGE java
    NAME 'oracle.spatial.router.partitioning.ElocationPartition.partition_router(
      java.lang.String)';

  PROCEDURE elocation_trucking_user_data(
                                  logfile_name in VARCHAR2) AS LANGUAGE java
    NAME 'oracle.spatial.router.partitioning.ElocationPartition.create_trucking_user_data(
      java.lang.String)';

  PROCEDURE elocation_dump_partition (logfile_name in VARCHAR2,
                                      start_pid in NUMBER,
                                      end_pid in NUMBER,
                                      verbose in BOOLEAN,
                                      is10g in BOOLEAN) AS LANGUAGE java
    NAME 'oracle.spatial.router.partitioning.ElocationPartition.dumpPartition(
      java.lang.String, int, int, boolean, boolean)';

  PROCEDURE elocation_validate_partition (logfile_name in VARCHAR2,
                                          start_pid in NUMBER,
                                          end_pid in NUMBER,
                                          verbose in BOOLEAN,
                                          is10g in BOOLEAN) AS LANGUAGE java
    NAME 'oracle.spatial.router.partitioning.ElocationPartition.validatePartition(
      java.lang.String, int, int, boolean, boolean)';

  PROCEDURE build_turn_restrictions (logdir in VARCHAR2,
                                     drivingside in VARCHAR2) AS LANGUAGE java
    NAME 'oracle.spatial.router.partitioning.TurnRestriction.buildTurnRestrictions(java.lang.String, java.lang.String)';

  FUNCTION get_edge_info(edge_ids       IN  sdo_list_type,
                         to_edge_ids    OUT sdo_list_type,
                         rets           OUT mdsys.string_array,
                         angle_segments OUT sdo_list_type)
  RETURN mdsys.string_array ;


  FUNCTION get_geometry_info(edge_ids       IN  sdo_list_type,
                             merged_coords  OUT sdo_list_type)
  RETURN NUMBER;

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
