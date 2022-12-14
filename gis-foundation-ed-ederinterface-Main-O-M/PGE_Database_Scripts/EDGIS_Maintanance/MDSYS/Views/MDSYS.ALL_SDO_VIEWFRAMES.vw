Prompt drop View ALL_SDO_VIEWFRAMES;
DROP VIEW MDSYS.ALL_SDO_VIEWFRAMES
/

/* Formatted on 6/27/2019 02:52:00 PM (QP5 v5.313) */
PROMPT View ALL_SDO_VIEWFRAMES;
--
-- ALL_SDO_VIEWFRAMES  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.ALL_SDO_VIEWFRAMES
(
    OWNER,
    NAME,
    DESCRIPTION,
    SCENE_NAME,
    DEFINITION
)
AS
    SELECT SDO_OWNER OWNER,
           NAME,
           DESCRIPTION,
           SCENE_NAME,
           DEFINITION
      FROM SDO_VIEWFRAMES_TABLE
/


Prompt Synonym ALL_SDO_VIEWFRAMES;
--
-- ALL_SDO_VIEWFRAMES  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM ALL_SDO_VIEWFRAMES FOR MDSYS.ALL_SDO_VIEWFRAMES
/


Prompt Grants on VIEW ALL_SDO_VIEWFRAMES TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.ALL_SDO_VIEWFRAMES TO PUBLIC
/
