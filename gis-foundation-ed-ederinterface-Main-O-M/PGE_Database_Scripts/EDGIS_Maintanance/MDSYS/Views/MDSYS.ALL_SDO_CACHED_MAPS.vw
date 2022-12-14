Prompt drop View ALL_SDO_CACHED_MAPS;
DROP VIEW MDSYS.ALL_SDO_CACHED_MAPS
/

/* Formatted on 6/27/2019 02:52:05 PM (QP5 v5.313) */
PROMPT View ALL_SDO_CACHED_MAPS;
--
-- ALL_SDO_CACHED_MAPS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_CACHED_MAPS
(
    OWNER,
    NAME,
    DESCRIPTION,
    TILES_TABLE,
    IS_ONLINE,
    IS_INTERNAL,
    DEFINITION,
    BASE_MAP,
    MAP_ADAPTER
)
AS
    SELECT SDO_OWNER OWNER,
           NAME,
           DESCRIPTION,
           tiles_table,
           is_online,
           is_internal,
           DEFINITION,
           base_map,
           map_adapter
      FROM mdsys.SDO_CACHED_MAPS_TABLE
/


Prompt Synonym ALL_SDO_CACHED_MAPS;
--
-- ALL_SDO_CACHED_MAPS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_CACHED_MAPS FOR MDSYS.ALL_SDO_CACHED_MAPS
/


Prompt Grants on VIEW ALL_SDO_CACHED_MAPS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_CACHED_MAPS TO PUBLIC
/
