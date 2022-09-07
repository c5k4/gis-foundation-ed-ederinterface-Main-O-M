Prompt drop View DBA_SDO_THEMES;
DROP VIEW MDSYS.DBA_SDO_THEMES
/

/* Formatted on 6/27/2019 02:51:59 PM (QP5 v5.313) */
PROMPT View DBA_SDO_THEMES;
--
-- DBA_SDO_THEMES  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.DBA_SDO_THEMES
(
    OWNER,
    NAME,
    DESCRIPTION,
    BASE_TABLE,
    GEOMETRY_COLUMN,
    STYLING_RULES
)
AS
    SELECT SDO_OWNER OWNER,
           NAME,
           DESCRIPTION,
           BASE_TABLE,
           GEOMETRY_COLUMN,
           STYLING_RULES
      FROM SDO_THEMES_TABLE
     WHERE (EXISTS
                (SELECT table_name
                   FROM dba_tables
                  WHERE table_name = base_table
                 UNION ALL
                 SELECT table_name
                   FROM dba_object_tables
                  WHERE table_name = base_table
                 UNION ALL
                 SELECT view_name table_name
                   FROM dba_views
                  WHERE view_name = base_table))
/


Prompt Synonym DBA_SDO_THEMES;
--
-- DBA_SDO_THEMES  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM DBA_SDO_THEMES FOR MDSYS.DBA_SDO_THEMES
/


Prompt Grants on VIEW DBA_SDO_THEMES TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.DBA_SDO_THEMES TO PUBLIC
/
