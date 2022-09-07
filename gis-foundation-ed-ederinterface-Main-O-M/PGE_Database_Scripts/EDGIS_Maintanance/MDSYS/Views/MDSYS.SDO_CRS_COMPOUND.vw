Prompt drop View SDO_CRS_COMPOUND;
DROP VIEW MDSYS.SDO_CRS_COMPOUND
/

/* Formatted on 6/27/2019 02:51:55 PM (QP5 v5.313) */
PROMPT View SDO_CRS_COMPOUND;
--
-- SDO_CRS_COMPOUND  (View)
--

CREATE OR REPLACE FORCE VIEW MDSYS.SDO_CRS_COMPOUND
(
    SRID,
    COORD_REF_SYS_NAME,
    CMPD_HORIZ_SRID,
    CMPD_VERT_SRID,
    INFORMATION_SOURCE,
    DATA_SOURCE
)
AS
    SELECT SRID,
           COORD_REF_SYS_NAME,
           CMPD_HORIZ_SRID,
           CMPD_VERT_SRID,
           INFORMATION_SOURCE,
           DATA_SOURCE
      FROM MDSYS.SDO_COORD_REF_SYS
     WHERE COORD_REF_SYS_KIND = 'COMPOUND'
/


Prompt Synonym SDO_CRS_COMPOUND;
--
-- SDO_CRS_COMPOUND  (Synonym) 
--
CREATE OR REPLACE PUBLIC SYNONYM SDO_CRS_COMPOUND FOR MDSYS.SDO_CRS_COMPOUND
/


Prompt Grants on VIEW SDO_CRS_COMPOUND TO PUBLIC to PUBLIC;
GRANT SELECT ON MDSYS.SDO_CRS_COMPOUND TO PUBLIC
/
