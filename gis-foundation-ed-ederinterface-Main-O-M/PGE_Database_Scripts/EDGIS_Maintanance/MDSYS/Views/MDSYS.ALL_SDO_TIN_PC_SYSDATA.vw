Prompt drop View ALL_SDO_TIN_PC_SYSDATA;
DROP VIEW MDSYS.ALL_SDO_TIN_PC_SYSDATA
/

/* Formatted on 6/27/2019 02:52:01 PM (QP5 v5.313) */
PROMPT View ALL_SDO_TIN_PC_SYSDATA;
--
-- ALL_SDO_TIN_PC_SYSDATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_TIN_PC_SYSDATA
(
    OWNER,
    TABLE_NAME,
    COLUMN_NAME,
    DEP_TABLE_SCHEMA,
    DEP_TABLE_NAME
)
AS
    SELECT SDO_OWNER OWNER,
           TABLE_NAME,
           COLUMN_NAME,
           DEP_TABLE_SCHEMA,
           DEP_TABLE_NAME
      FROM SDO_TIN_PC_SYSDATA_TABLE a
     WHERE (EXISTS
                (SELECT table_name
                   FROM all_tables
                  WHERE table_name = a.table_name AND owner = sdo_owner
                 UNION ALL
                 SELECT table_name
                   FROM all_object_tables
                  WHERE table_name = a.table_name AND owner = sdo_owner
                 UNION ALL
                 SELECT view_name table_name
                   FROM all_views
                  WHERE view_name = a.table_name AND owner = sdo_owner))
/


Prompt Synonym ALL_SDO_TIN_PC_SYSDATA;
--
-- ALL_SDO_TIN_PC_SYSDATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_TIN_PC_SYSDATA FOR MDSYS.ALL_SDO_TIN_PC_SYSDATA
/


Prompt Grants on VIEW ALL_SDO_TIN_PC_SYSDATA TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_TIN_PC_SYSDATA TO PUBLIC
/
