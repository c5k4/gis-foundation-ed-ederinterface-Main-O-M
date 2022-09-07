Prompt drop View ALL_ST_GEOMETRY_COLUMNS_V;
DROP VIEW SDE.ALL_ST_GEOMETRY_COLUMNS_V
/

/* Formatted on 7/2/2019 01:17:55 PM (QP5 v5.313) */
PROMPT View ALL_ST_GEOMETRY_COLUMNS_V;
--
-- ALL_ST_GEOMETRY_COLUMNS_V  (View)
--

CREATE OR REPLACE FORCE VIEW SDE.ALL_ST_GEOMETRY_COLUMNS_V
(
    OWNER,
    TABLE_NAME,
    COLUMN_NAME,
    GEOMETRY_TYPE,
    PROPERTIES,
    SRID
)
AS
    SELECT owner,
           table_name,
           column_name,
           geometry_type,
           properties,
           srid
      FROM SDE.st_geometry_columns
/


Prompt Grants on VIEW ALL_ST_GEOMETRY_COLUMNS_V TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON SDE.ALL_ST_GEOMETRY_COLUMNS_V TO PUBLIC
/
