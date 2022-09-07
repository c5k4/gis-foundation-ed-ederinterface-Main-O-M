Prompt drop View ALL_SDO_THEMES;
DROP VIEW MDSYS.ALL_SDO_THEMES
/

/* Formatted on 6/27/2019 02:52:01 PM (QP5 v5.313) */
PROMPT View ALL_SDO_THEMES;
--
-- ALL_SDO_THEMES  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_THEMES
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
                   FROM all_tables
                  WHERE table_name = base_table AND owner = sdo_owner
                 UNION ALL
                 SELECT table_name
                   FROM all_object_tables
                  WHERE table_name = base_table AND owner = sdo_owner
                 UNION ALL
                 SELECT view_name table_name
                   FROM all_views
                  WHERE view_name = base_table AND owner = sdo_owner))
/


Prompt Synonym ALL_SDO_THEMES;
--
-- ALL_SDO_THEMES  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_THEMES FOR MDSYS.ALL_SDO_THEMES
/


Prompt Grants on VIEW ALL_SDO_THEMES TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_THEMES TO PUBLIC
/
