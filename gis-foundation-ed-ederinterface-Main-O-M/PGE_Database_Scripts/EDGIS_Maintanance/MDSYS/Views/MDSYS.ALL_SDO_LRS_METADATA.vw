Prompt drop View ALL_SDO_LRS_METADATA;
DROP VIEW MDSYS.ALL_SDO_LRS_METADATA
/

/* Formatted on 6/27/2019 02:52:04 PM (QP5 v5.313) */
PROMPT View ALL_SDO_LRS_METADATA;
--
-- ALL_SDO_LRS_METADATA  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_LRS_METADATA
(
    OWNER,
    TABLE_NAME,
    COLUMN_NAME,
    DIM_POS,
    DIM_UNIT
)
AS
    SELECT SDO_OWNER       OWNER,
           SDO_TABLE_NAME  TABLE_NAME,
           SDO_COLUMN_NAME COLUMN_NAME,
           SDO_DIM_POS     DIM_POS,
           SDO_DIM_UNIT    DIM_UNIT
      FROM SDO_LRS_METADATA_TABLE
/


Prompt Synonym ALL_SDO_LRS_METADATA;
--
-- ALL_SDO_LRS_METADATA  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_LRS_METADATA FOR MDSYS.ALL_SDO_LRS_METADATA
/


Prompt Grants on VIEW ALL_SDO_LRS_METADATA TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_LRS_METADATA TO PUBLIC
/