Prompt drop View ALL_SDO_LIGHTSOURCES;
DROP VIEW MDSYS.ALL_SDO_LIGHTSOURCES
/

/* Formatted on 6/27/2019 02:52:04 PM (QP5 v5.313) */
PROMPT View ALL_SDO_LIGHTSOURCES;
--
-- ALL_SDO_LIGHTSOURCES  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_LIGHTSOURCES
(
    OWNER,
    NAME,
    DESCRIPTION,
    TYPE,
    DEFINITION
)
AS
    SELECT SDO_OWNER OWNER,
           NAME,
           DESCRIPTION,
           TYPE,
           DEFINITION
      FROM SDO_LIGHTSOURCEs_TABLE
/


Prompt Synonym ALL_SDO_LIGHTSOURCES;
--
-- ALL_SDO_LIGHTSOURCES  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_LIGHTSOURCES FOR MDSYS.ALL_SDO_LIGHTSOURCES
/


Prompt Grants on VIEW ALL_SDO_LIGHTSOURCES TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_LIGHTSOURCES TO PUBLIC
/