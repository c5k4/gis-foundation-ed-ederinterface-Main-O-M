Prompt drop View SDO_AVAILABLE_OPS;
DROP VIEW MDSYS.SDO_AVAILABLE_OPS
/

/* Formatted on 6/27/2019 02:51:56 PM (QP5 v5.313) */
PROMPT View SDO_AVAILABLE_OPS;
--
-- SDO_AVAILABLE_OPS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.SDO_AVAILABLE_OPS
(
    SOURCE_SRID,
    COORD_OP_ID,
    TARGET_SRID,
    IS_IMPLEMENTED
)
AS
    SELECT SOURCE_SRID,
           COORD_OP_ID,
           TARGET_SRID,
           IS_IMPLEMENTED
      FROM MDSYS.SDO_AVAILABLE_ELEM_OPS
    UNION
    SELECT SOURCE_SRID,
           COORD_OP_ID,
           TARGET_SRID,
           IS_IMPLEMENTED
      FROM MDSYS.SDO_AVAILABLE_NON_ELEM_OPS
/


Prompt Synonym SDO_AVAILABLE_OPS;
--
-- SDO_AVAILABLE_OPS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_AVAILABLE_OPS FOR MDSYS.SDO_AVAILABLE_OPS
/


Prompt Grants on VIEW SDO_AVAILABLE_OPS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.SDO_AVAILABLE_OPS TO PUBLIC
/
