Prompt drop View SDO_ANGLE_UNITS;
DROP VIEW MDSYS.SDO_ANGLE_UNITS
/

/* Formatted on 6/27/2019 02:51:57 PM (QP5 v5.313) */
PROMPT View SDO_ANGLE_UNITS;
--
-- SDO_ANGLE_UNITS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.SDO_ANGLE_UNITS
(
    SDO_UNIT,
    UNIT_NAME,
    CONVERSION_FACTOR
)
AS
    SELECT SHORT_NAME            "SDO_UNIT",
           UNIT_OF_MEAS_NAME     "UNIT_NAME",
           (FACTOR_B / FACTOR_C) "CONVERSION_FACTOR"
      FROM MDSYS.SDO_UNITS_OF_MEASURE
     WHERE LOWER (UNIT_OF_MEAS_TYPE) = 'angle'
/


Prompt Synonym SDO_ANGLE_UNITS;
--
-- SDO_ANGLE_UNITS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_ANGLE_UNITS FOR MDSYS.SDO_ANGLE_UNITS
/


Prompt Grants on VIEW SDO_ANGLE_UNITS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.SDO_ANGLE_UNITS TO PUBLIC
/
