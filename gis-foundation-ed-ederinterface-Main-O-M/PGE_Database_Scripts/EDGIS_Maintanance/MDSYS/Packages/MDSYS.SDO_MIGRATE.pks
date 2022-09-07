Prompt drop Package SDO_MIGRATE;
DROP PACKAGE MDSYS.SDO_MIGRATE
/

Prompt Package SDO_MIGRATE;
--
-- SDO_MIGRATE  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.sdo_migrate authid current_user AS

   --
   --  NAME:
   --     to_734
   --  DESCRIPTION:
   --     Update an SDO 7.3.3 layer structure to a 7.3.4 layer structure.
   --  ARGUMENTS:
   --     sn        - Schema Name of owner of layer
   --     layer     - LAYER name
   --     tess_type - TESSellation TYPE
   --     param     - SDO_LEVEL or SDO_NUMTILES value
   --  USAGE:
   --     For FIXED size tessellation:
   --
   --        SQL> execute sdo_migrate.to_734('HERMAN','ROADS','FIXED',10)
   --
   --     For VARIABLE sized tessellation
   --
   --        SQL> execute sdo_migrate.to_734('HERMAN','ROADS','VARIABLE',4)
   --
   --     In each case the value of the PARAM argument is evaluated relative
   --     to the value of the TESS_TYPE arguments.  If the TESS_TYPE aregument
   --     is FIXED then PARAM is the SDO_LEVEL values.  If the TESS_TYPE
   --     argument is VARIABLE them the PARAM value is the SDO_NUMTILES value.
   --
  /* deprecated
   procedure to_734( sn        varchar2,
                     layer     varchar2,
                     tess_type varchar2,
                     param     integer ); */

   --
   -- Migrate from 734 format to 81x object format
   -- Index is NOT built!
   --
   -- The geom table and dim tables will be migrated.
   -- layer_gtype can either be 'POINT' or 'NOTPOINT' ('NOTPOINT' by default);
   -- update_flag can either be 'INSERT' or 'UPDATE' ('INSERT' by default);
   --
   procedure to_81x(layer        IN varchar2,
                    newtabname   IN varchar2,
                    gidcolumn    IN varchar2,
                    geocolname   IN varchar2,
                    layer_gtype  IN varchar2 := 'NOTPOINT',
                    update_flag  IN varchar2 := 'INSERT');

   procedure to_current(layer        IN varchar2,
                    newtabname   IN varchar2,
                    gidcolumn    IN varchar2,
                    geocolname   IN varchar2,
                    layer_gtype  IN varchar2 := 'NOTPOINT',
                    update_flag  IN varchar2 := 'INSERT');

   --
   -- Migrate from 815 format to 816 object format
   --
   -- All geometry objects in geom table will be migrated so that
   -- their GTYPE and ETYPE will be of 8.1.6 style.
   --
   procedure from_815_to_81x(tabname   IN varchar2,
                             commit_int IN number := -1);

   procedure to_current(tabname IN varchar2,
                        column_name IN varchar2 := NULL,
                        commit_int IN number := -1);
   --
   -- Migrate OGIS metadata entries in MDSYS's schema
   -- Called at the source of migration.
   -- This procedure will produce a temporary table called
   -- SDO_GC_MIG that needs to be exported with other data.
   --
   procedure ogis_metadata_from;

   --
   -- Migrate OGIS metadata entries in MDSYS's schema
   -- Called at the destination of migration
   -- This call expects SDO_GC_MIG to exist.
   --
   procedure ogis_metadata_to;
  function to_current(geom IN MDSYS.SDO_GEOMETRY,
                    dim  IN MDSYS.SDO_DIM_ARRAY)
  return MDSYS.SDO_GEOMETRY DETERMINISTIC;


END sdo_migrate;
/


Prompt Synonym SDO_MIGRATE;
--
-- SDO_MIGRATE  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_MIGRATE FOR MDSYS.SDO_MIGRATE
/


Prompt Grants on PACKAGE SDO_MIGRATE TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_MIGRATE TO PUBLIC
/
