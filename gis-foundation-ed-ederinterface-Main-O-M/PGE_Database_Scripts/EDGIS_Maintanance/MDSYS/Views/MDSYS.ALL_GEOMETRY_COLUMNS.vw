Prompt drop View ALL_GEOMETRY_COLUMNS;
DROP VIEW MDSYS.ALL_GEOMETRY_COLUMNS
/

/* Formatted on 6/27/2019 02:52:06 PM (QP5 v5.313) */
PROMPT View ALL_GEOMETRY_COLUMNS;
--
-- ALL_GEOMETRY_COLUMNS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_GEOMETRY_COLUMNS
(
    F_TABLE_SCHEMA,
    F_TABLE_NAME,
    F_GEOMETRY_COLUMN,
    G_TABLE_SCHEMA,
    G_TABLE_NAME,
    STORAGE_TYPE,
    GEOMETRY_TYPE,
    COORD_DIMENSION,
    MAX_PPR,
    SRID
)
AS
    SELECT "F_TABLE_SCHEMA",
           "F_TABLE_NAME",
           "F_GEOMETRY_COLUMN",
           "G_TABLE_SCHEMA",
           "G_TABLE_NAME",
           "STORAGE_TYPE",
           "GEOMETRY_TYPE",
           "COORD_DIMENSION",
           "MAX_PPR",
           "SRID"
      FROM OGIS_GEOMETRY_COLUMNS
     WHERE     (   EXISTS
                       (SELECT table_name
                          FROM all_tables
                         WHERE     table_name = f_table_name
                               AND owner = f_table_schema)
                OR EXISTS
                       (SELECT view_name
                          FROM all_views
                         WHERE     view_name = f_table_name
                               AND owner = f_table_schema)
                OR EXISTS
                       (SELECT table_name
                          FROM all_object_tables
                         WHERE     table_name = f_table_name
                               AND owner = f_table_schema))
           AND (   EXISTS
                       (SELECT table_name
                          FROM all_tables
                         WHERE     table_name = g_table_name
                               AND owner = g_table_schema)
                OR EXISTS
                       (SELECT view_name
                          FROM all_views
                         WHERE     view_name = g_table_name
                               AND owner = g_table_schema)
                OR EXISTS
                       (SELECT table_name
                          FROM all_object_tables
                         WHERE     table_name = g_table_name
                               AND owner = g_table_schema))
/


Prompt Synonym ALL_GEOMETRY_COLUMNS;
--
-- ALL_GEOMETRY_COLUMNS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_GEOMETRY_COLUMNS FOR MDSYS.ALL_GEOMETRY_COLUMNS
/


Prompt Grants on VIEW ALL_GEOMETRY_COLUMNS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_GEOMETRY_COLUMNS TO PUBLIC
/
