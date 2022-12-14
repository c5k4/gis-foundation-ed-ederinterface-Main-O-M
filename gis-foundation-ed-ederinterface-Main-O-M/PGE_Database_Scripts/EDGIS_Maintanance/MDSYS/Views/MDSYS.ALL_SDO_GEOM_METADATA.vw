Prompt drop View ALL_SDO_GEOM_METADATA;
DROP VIEW MDSYS.ALL_SDO_GEOM_METADATA
/

/* Formatted on 6/27/2019 02:52:05 PM (QP5 v5.313) */
PROMPT View ALL_SDO_GEOM_METADATA;
--
-- ALL_SDO_GEOM_METADATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_GEOM_METADATA
(
    OWNER,
    TABLE_NAME,
    COLUMN_NAME,
    DIMINFO,
    SRID
)
AS
    SELECT b.SDO_OWNER       OWNER,
           b.SDO_TABLE_NAME  TABLE_NAME,
           b.SDO_COLUMN_NAME COLUMN_NAME,
           b.SDO_DIMINFO     DIMINFO,
           b.SDO_SRID        SRID
      FROM mdsys.SDO_GEOM_METADATA_TABLE b, all_objects a
     WHERE     b.sdo_table_name = a.object_name
           AND b.sdo_owner = a.owner
           AND a.object_type IN ('TABLE', 'SYNONYM', 'VIEW')
    UNION ALL
    SELECT b.SDO_OWNER       OWNER,
           b.SDO_TABLE_NAME  TABLE_NAME,
           b.SDO_COLUMN_NAME COLUMN_NAME,
           b.SDO_DIMINFO     DIMINFO,
           b.SDO_SRID        SRID
      FROM mdsys.SDO_GEOM_METADATA_TABLE b, all_object_tables a
     WHERE b.sdo_table_name = a.table_name AND b.sdo_owner = a.owner
/


Prompt Synonym ALL_SDO_GEOM_METADATA;
--
-- ALL_SDO_GEOM_METADATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_GEOM_METADATA FOR MDSYS.ALL_SDO_GEOM_METADATA
/


Prompt Grants on VIEW ALL_SDO_GEOM_METADATA TO LIC_MONITOR to LIC_MONITOR;
GRANT SELECT ON MDSYS.ALL_SDO_GEOM_METADATA TO LIC_MONITOR
/

Prompt Grants on VIEW ALL_SDO_GEOM_METADATA TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_GEOM_METADATA TO PUBLIC
/
