Prompt drop View ALL_SDO_GEOR_SYSDATA;
DROP VIEW MDSYS.ALL_SDO_GEOR_SYSDATA
/

/* Formatted on 6/27/2019 02:52:05 PM (QP5 v5.313) */
PROMPT View ALL_SDO_GEOR_SYSDATA;
--
-- ALL_SDO_GEOR_SYSDATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_GEOR_SYSDATA
(
    OWNER,
    TABLE_NAME,
    COLUMN_NAME,
    METADATA_COLUMN_NAME,
    RDT_TABLE_NAME,
    RASTER_ID,
    OTHER_TABLE_NAMES
)
AS
    SELECT SDO_OWNER                 OWNER,
           GEORASTER_TABLE_NAME      TABLE_NAME,
           GEORASTER_COLUMN_NAME     COLUMN_NAME,
           GEOR_METADATA_COLUMN_NAME METADATA_COLUMN_NAME,
           RDT_TABLE_NAME            RDT_TABLE_NAME,
           RASTER_ID                 RASTER_ID,
           OTHER_TABLE_NAMES         OTHER_TABLE_NAMES
      FROM SDO_GEOR_SYSDATA_TABLE
     WHERE (   (sdo_owner = SYS_CONTEXT ('userenv', 'SESSION_USER'))
            OR EXISTS
                   (SELECT table_name
                      FROM all_tables
                     WHERE     table_name = georaster_table_name
                           AND owner = sdo_owner
                    UNION ALL
                    SELECT table_name
                      FROM all_object_tables
                     WHERE     table_name = georaster_table_name
                           AND owner = sdo_owner
                    UNION ALL
                    SELECT view_name table_name
                      FROM all_views
                     WHERE     view_name = georaster_table_name
                           AND owner = sdo_owner))
/


Prompt Synonym ALL_SDO_GEOR_SYSDATA;
--
-- ALL_SDO_GEOR_SYSDATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_GEOR_SYSDATA FOR MDSYS.ALL_SDO_GEOR_SYSDATA
/


Prompt Grants on VIEW ALL_SDO_GEOR_SYSDATA TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_GEOR_SYSDATA TO PUBLIC
/
