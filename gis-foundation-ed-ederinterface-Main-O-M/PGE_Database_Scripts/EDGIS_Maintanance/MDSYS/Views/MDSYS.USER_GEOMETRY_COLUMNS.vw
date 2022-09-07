Prompt drop View USER_GEOMETRY_COLUMNS;
DROP VIEW MDSYS.USER_GEOMETRY_COLUMNS
/

/* Formatted on 6/27/2019 02:51:51 PM (QP5 v5.313) */
PROMPT View USER_GEOMETRY_COLUMNS;
--
-- USER_GEOMETRY_COLUMNS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_GEOMETRY_COLUMNS
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
     WHERE f_table_schema = USER AND g_table_schema = USER
/


Prompt Synonym USER_GEOMETRY_COLUMNS;
--
-- USER_GEOMETRY_COLUMNS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_GEOMETRY_COLUMNS FOR MDSYS.USER_GEOMETRY_COLUMNS
/


Prompt Grants on VIEW USER_GEOMETRY_COLUMNS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.USER_GEOMETRY_COLUMNS TO PUBLIC
/
