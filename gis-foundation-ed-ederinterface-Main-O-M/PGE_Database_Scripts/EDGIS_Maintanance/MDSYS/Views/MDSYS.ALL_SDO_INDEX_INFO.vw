Prompt drop View ALL_SDO_INDEX_INFO;
DROP VIEW MDSYS.ALL_SDO_INDEX_INFO
/

/* Formatted on 6/27/2019 02:52:05 PM (QP5 v5.313) */
PROMPT View ALL_SDO_INDEX_INFO;
--
-- ALL_SDO_INDEX_INFO  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_INDEX_INFO
(
    SDO_INDEX_OWNER,
    INDEX_NAME,
    TABLE_OWNER,
    TABLE_NAME,
    COLUMN_NAME,
    SDO_INDEX_TYPE,
    SDO_INDEX_TABLE,
    SDO_INDEX_STATUS
)
AS
    SELECT SDO_INDEX_OWNER,
           SDO_INDEX_NAME                 index_name,
           table_owner,
           table_name,
           REPLACE (sdo_column_name, '"') column_name,
           SDO_INDEX_TYPE,
           SDO_INDEX_TABLE,
           SDO_INDEX_STATUS
      FROM all_sdo_index_metadata, all_indexes
     WHERE index_name = sdo_index_name AND owner = sdo_index_owner
/


Prompt Synonym ALL_SDO_INDEX_INFO;
--
-- ALL_SDO_INDEX_INFO  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_INDEX_INFO FOR MDSYS.ALL_SDO_INDEX_INFO
/


Prompt Grants on VIEW ALL_SDO_INDEX_INFO TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_INDEX_INFO TO PUBLIC
/
