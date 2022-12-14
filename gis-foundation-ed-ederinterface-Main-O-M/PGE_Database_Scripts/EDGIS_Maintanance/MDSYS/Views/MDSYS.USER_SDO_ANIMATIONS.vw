Prompt drop View USER_SDO_ANIMATIONS;
DROP VIEW MDSYS.USER_SDO_ANIMATIONS
/

/* Formatted on 6/27/2019 02:51:50 PM (QP5 v5.313) */
PROMPT View USER_SDO_ANIMATIONS;
--
-- USER_SDO_ANIMATIONS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.USER_SDO_ANIMATIONS
(
    NAME,
    DESCRIPTION,
    DEFINITION
)
AS
    SELECT NAME, DESCRIPTION, DEFINITION
      FROM SDO_ANIMATIONS_TABLE
     WHERE sdo_owner = SYS_CONTEXT ('userenv', 'CURRENT_SCHEMA')
/


Prompt Synonym USER_SDO_ANIMATIONS;
--
-- USER_SDO_ANIMATIONS  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM USER_SDO_ANIMATIONS FOR MDSYS.USER_SDO_ANIMATIONS
/


Prompt Grants on VIEW USER_SDO_ANIMATIONS TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, SELECT, UPDATE ON MDSYS.USER_SDO_ANIMATIONS TO PUBLIC
/
