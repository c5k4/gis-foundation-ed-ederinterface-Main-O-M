Prompt drop View USER_SDO_INDEX_INFO;
DROP VIEW MDSYS.USER_SDO_INDEX_INFO
/

/* Formatted on 6/27/2019 02:51:47 PM (QP5 v5.313) */
PROMPT View USER_SDO_INDEX_INFO;
--
-- USER_SDO_INDEX_INFO  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_INDEX_INFO
(
    INDEX_NAME,
    TABLE_OWNER,
    TABLE_NAME,
    COLUMN_NAME,
    SDO_INDEX_TYPE,
    SDO_INDEX_TABLE,
    SDO_INDEX_STATUS
)
AS
    SELECT SDO_INDEX_NAME                 index_name,
           table_owner,
           table_name,
           REPLACE (sdo_column_name, '"') column_name,
           SDO_INDEX_TYPE,
           SDO_INDEX_TABLE,
           SDO_INDEX_STATUS
      FROM user_sdo_index_metadata, user_indexes
     WHERE index_name = sdo_index_name
/


Prompt Synonym USER_SDO_INDEX_INFO;
--
-- USER_SDO_INDEX_INFO  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_INDEX_INFO FOR MDSYS.USER_SDO_INDEX_INFO
/


Prompt Grants on VIEW USER_SDO_INDEX_INFO TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.USER_SDO_INDEX_INFO TO PUBLIC
/
