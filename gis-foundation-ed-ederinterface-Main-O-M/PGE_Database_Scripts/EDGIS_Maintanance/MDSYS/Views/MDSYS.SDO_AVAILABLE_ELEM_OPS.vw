Prompt drop View SDO_AVAILABLE_ELEM_OPS;
DROP VIEW MDSYS.SDO_AVAILABLE_ELEM_OPS
/

/* Formatted on 6/27/2019 02:51:57 PM (QP5 v5.313) */
PROMPT View SDO_AVAILABLE_ELEM_OPS;
--
-- SDO_AVAILABLE_ELEM_OPS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.SDO_AVAILABLE_ELEM_OPS
(
    SOURCE_SRID,
    COORD_OP_ID,
    TARGET_SRID,
    IS_IMPLEMENTED
)
AS
    (SELECT OPS.SOURCE_SRID,
            OPS.COORD_OP_ID            "COORD_OP_ID",
            OPS.TARGET_SRID,
            OPS.IS_IMPLEMENTED_FORWARD "IS_IMPLEMENTED"
       FROM MDSYS.SDO_COORD_OPS OPS
      WHERE NOT (OPS.COORD_OP_TYPE = 'CONCATENATED OPERATION'))
    UNION
    (SELECT OPS.TARGET_SRID            "SOURCE_SRID",
            -OPS.COORD_OP_ID           "COORD_OP_ID",
            OPS.SOURCE_SRID            "TARGET_SRID",
            OPS.IS_IMPLEMENTED_REVERSE "IS_IMPLEMENTED"
       FROM MDSYS.SDO_COORD_OPS OPS
      WHERE     NOT (OPS.COORD_OP_TYPE = 'CONCATENATED OPERATION')
            AND OPS.REVERSE_OP = 1)
/


Prompt Synonym SDO_AVAILABLE_ELEM_OPS;
--
-- SDO_AVAILABLE_ELEM_OPS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_AVAILABLE_ELEM_OPS FOR MDSYS.SDO_AVAILABLE_ELEM_OPS
/


Prompt Grants on VIEW SDO_AVAILABLE_ELEM_OPS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.SDO_AVAILABLE_ELEM_OPS TO PUBLIC
/
