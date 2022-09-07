Prompt drop Package SDO_ADMIN;
DROP PACKAGE MDSYS.SDO_ADMIN
/

Prompt Package SDO_ADMIN;
--
-- SDO_ADMIN  (Package) 
--
CREATE OR REPLACE PACKAGE MDSYS.SDO_ADMIN authid current_user as


-- NAME:
--    sdo_version - Return version number as a string
--
function sdo_version return varchar2;
pragma restrict_references(sdo_version,rnds,wnds,rnps,wnps);

function sdo_code_size ( layer varchar2 ) return binary_integer;
pragma restrict_references(sdo_code_size,wnds,rnps,wnps);

--
--  NAME:
--     populate_index - tesselate all geometric objects in a geometry table
--  DESCRIPTION:
--     This procedure creates a maximum number of tiles to cover every geometric
--     object in the geometry table (LAYER_sdogeom).  If the synch_flag is
--     TRUE only the geometric objects which do not already have corresponding
--     entries in the spatial index table (LAYER_sdoindex) are processed.
--  NOTES:
--     The behaviour of the POPULATE_INDEX( layer IN VARCHAR2 ) function is
--     determined by the values for the SDO_LEVEL and SDO_NUMTILES columns
--     in the <layer>_SDOLAYER table.  The behaviour is described in the
--     following table:
--
--     VALUE OF SDO_LEVEL   VALUE OF SDO_NUMTILES   BEHAVIOUR
--     ------------------   ---------------------   ------------------------
--           NULL                 NULL              Error (use 7.3.3)
--           NULL               NOT NULL            Variable Tessellation
--         NOT NULL               NULL              Fixed Tessellation
--         NOT NULL             NOT NULL            Hybrid Tessellation
--  ARGUMENTS:
--     layer         - geometry table name (LAYER_sdogeom) and spatial
--                     index table name (LAYER_sdoindex) input argument
--     gid           - Geometry IDentifier input argument
--     maxtiles      -  MAXimum number of TILES to generate input argument
--     synch_flag    - tesselate only geometric objects having no corresponding
--                     entries in the spatial index table if TRUE
--                     (default value is TRUE)
--     sdo_tile_flag - INTERNAL PARAMETER; DO NOT MODIFY
--
procedure populate_index ( layer IN VARCHAR2 );

procedure populate_index (layer IN VARCHAR2, maxtiles IN INTEGER,
			  synch_flag IN BOOLEAN := TRUE,
                          sdo_tile_flag IN BOOLEAN := FALSE );

procedure populate_index_fixed (layer IN VARCHAR2, tile_size IN INTEGER,
			        synch_flag IN BOOLEAN := TRUE,
                                sdo_tile_flag IN BOOLEAN := FALSE,
				sdo_maxcode_flag IN BOOLEAN := TRUE );

--
--  NAME:
--     update_index - tesselate a single geometric object
--  DESCRIPTION:
--     This procedure creates a maximum number of tiles to cover a geometric
--     object in the geometry table (LAYER_sdogeom).  If the replace_flag is
--     TRUE then any existing tiles for the specified object are first
--     removed from the index table (LAYER_sdoindex).
--  NOTES:
--     The behaviour of the POPULATE_INDEX( layer IN VARCHAR2 ) function is
--     determined by the values for the SDO_LEVEL and SDO_NUMTILES columns
--     in the <layer>_SDOLAYER table.  The behaviour is described in the
--     following table:
--
--     VALUE OF SDO_LEVEL   VALUE OF SDO_NUMTILES   BEHAVIOUR
--     ------------------   ---------------------   ------------------------
--           NULL                 NULL              Error (use 7.3.3)
--           NULL               NOT NULL            Variable Tessellation
--         NOT NULL               NULL              Fixed Tessellation
--         NOT NULL             NOT NULL            Hybrid Tessellation
--  ARGUMENTS:
--     layer        - geometry table name (LAYER_sdogeom) and
--                    index table name (LAYER_sdoindex) input argument
--     gid          - Geometry IDentifier input argument
--     maxtiles     - MAXimum number of TILES to generate input argument
--     replace_flag - REPLACE existing tiles in LAYER_sdoindex if TRUE
--                    (default value is TRUE)
--     sdo_tile_flag - INTERNAL PARAMETER; DO NOT MODIFY
--
procedure update_index (layer varchar2, gid number);

procedure update_index (layer IN VARCHAR2, gid IN NUMBER, maxtiles IN INTEGER,
                        replace_flag IN BOOLEAN := TRUE,
                        sdo_tile_flag IN BOOLEAN := FALSE);

procedure update_index_fixed (layer IN VARCHAR2, gid IN NUMBER,
			      tile_size IN INTEGER,
                              replace_flag IN BOOLEAN := TRUE,
                              sdo_tile_flag IN BOOLEAN := FALSE,
			      sdo_maxcode_flag IN BOOLEAN := TRUE );

--
--  NAME:
--     change_index_parameters - modify parameters in <layer>_SDOLAYER table
--  DESCRIPTION:
--     This function allows the user to modify the values of the SDO_LEVEL and
--     SDO_NUMTILES columns AND also takes the appropriate action to synchron-
--     ize the index according to the following rules:
--
--     VALUE OF LEVEL   VALUE OF MXTILES   BEHAVIOUR
--     --------------   ----------------   ---------------------------------
--          NULL             NULL          Do nothing.
--
--          NULL           NOT NULL        Call POPULATE_INDEX().  _SDOINDEX
--                                         table must be empty.
--        NOT NULL           NULL          Use UPDATE statement to update
--                                         SDO_GROUPCODE column.
--        NOT NULL         NOT NULL        Call POPULATE_INDEX(). _SDOINDEX
--                                         table must be empty.
--  ARGUMENTS:
--     layer        - name of LAYER to modify
--     sdo_level    - new value for SDO_LEVEL column
--     sdo_numtiles - new value for SDO_NUMTILES column
--
procedure change_index_parameters( layer varchar2,
                                   sdo_level number,
                                   sdo_numtiles number );

  --------------------------------------------------------------------------------
  --
  -- Description:
  --
  --   This routine is ONLY for creating fixed tiles for point data.
  --   All rows with SDO_CODE is NULL will be tiled.
  --   This routine will perform a database commit.
  --
  --   The <layer>_sdolayer table must be populated before this routine is called.
  --   The <layer>_sdodim table must be populated before this routine is called.
  --   The <layer>_sdogeom table must be populated before this routine is called.
  --   The <layer>_sdogeom table must contain the SDO_CODE column.
  --   The <layer>_sdogeom table can optionally contain the SDO_TILE column.
  --
  -- Parameters:
  --                layer         - The layer to tile
  --                sdo_tile_flag - If TRUE, will generate the sdo_tile column.
  --                                Default is FALSE.
  --                commit_cnt     - Routine updates in increments of commit_cnt,
  --                                 and commits each increment.
  --
  --------------------------------------------------------------------------------
  PROCEDURE populate_index_fixed_points (layer         varchar2,
                                         sdo_tile_flag boolean := FALSE,
                                         commit_cnt    number := 50);

  --------------------------------------------------------------------------------
  --
  -- Description:
  --
  --   This routine is ONLY for creating fixed tiles for point data.
  --   Set SDO_CODE column to NULL for all rows in layer.
  --   This routine will performe a database commit.
  --
  --   The <layer>_sdogeom table must contain the SDO_CODE column.
  --
  -- Parameters:
  --                layer - The layer to tile
  --                commit_cnt - Routine updates in increments of commit_cnt,
  --                             and commits each increment.
  --
  --------------------------------------------------------------------------------
  PROCEDURE set_code_null (layer         varchar2,
                           commit_cnt    number := 50);



end SDO_ADMIN;
/


Prompt Synonym SDO_ADMIN;
--
-- SDO_ADMIN  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_ADMIN FOR MDSYS.SDO_ADMIN
/


Prompt Grants on PACKAGE SDO_ADMIN TO PUBLIC to PUBLIC;
GRANT EXECUTE ON MDSYS.SDO_ADMIN TO PUBLIC
/
