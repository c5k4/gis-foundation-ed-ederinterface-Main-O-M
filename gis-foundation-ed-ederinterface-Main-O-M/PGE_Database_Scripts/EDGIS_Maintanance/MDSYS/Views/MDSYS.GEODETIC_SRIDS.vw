Prompt drop View GEODETIC_SRIDS;
DROP VIEW MDSYS.GEODETIC_SRIDS
/

/* Formatted on 6/27/2019 02:51:58 PM (QP5 v5.313) */
PROMPT View GEODETIC_SRIDS;
--
-- GEODETIC_SRIDS  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.GEODETIC_SRIDS
(
    SRID
)
AS
    SELECT srid
      FROM MDSYS.CS_SRS
     WHERE WKTEXT LIKE 'GEOGCS%'
    MINUS
    SELECT srid
      FROM MDSYS.SDO_COORD_REF_SYS
     WHERE coord_ref_sys_kind = 'GEOCENTRIC'
/


Prompt Grants on VIEW GEODETIC_SRIDS TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.GEODETIC_SRIDS TO PUBLIC
/