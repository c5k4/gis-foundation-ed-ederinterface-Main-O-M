Prompt drop View ALL_SDO_3DTHEMES;
DROP VIEW MDSYS.ALL_SDO_3DTHEMES
/

/* Formatted on 6/27/2019 02:52:06 PM (QP5 v5.313) */
PROMPT View ALL_SDO_3DTHEMES;
--
-- ALL_SDO_3DTHEMES  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_3DTHEMES
(
    OWNER,
    NAME,
    DESCRIPTION,
    BASE_TABLE,
    THEME_COLUMN,
    STYLE_COLUMN,
    THEME_TYPE,
    DEFINITION
)
AS
    SELECT SDO_OWNER OWNER,
           NAME,
           DESCRIPTION,
           BASE_TABLE,
           THEME_COLUMN,
           STYLE_COLUMN,
           THEME_TYPE,
           DEFINITION
      FROM SDO_3DTHEMES_TABLE
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


Prompt Synonym ALL_SDO_3DTHEMES;
--
-- ALL_SDO_3DTHEMES  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_3DTHEMES FOR MDSYS.ALL_SDO_3DTHEMES
/


Prompt Grants on VIEW ALL_SDO_3DTHEMES TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_3DTHEMES TO PUBLIC
/
