Prompt drop View PGE_COUNT_XFMR;
DROP VIEW EDGIS.PGE_COUNT_XFMR
/

/* Formatted on 7/2/2019 01:18:06 PM (QP5 v5.313) */
PROMPT View PGE_COUNT_XFMR;
--
-- PGE_COUNT_XFMR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.PGE_COUNT_XFMR
(
    FEEDERID,
    GLOBALID,
    CGC12,
    COUNT
)
AS
    (  SELECT TX.circuitid      AS FEEDERID,
              TX.globalid       AS GLOBALID,
              TX.cgc12          AS CGC12,
              COUNT (sp.globalid) AS COUNT
         FROM EDGIS.TRANSFORMER TX
              INNER JOIN EDGIS.SERVICEPOINT SP
                  ON TX.GLOBALID = SP.TRANSFORMERGUID
     GROUP BY tx.circuitid,
              TX.globalid,
              TX.cedsadeviceid,
              TX.cgc12)
/


Prompt Grants on VIEW PGE_COUNT_XFMR TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.PGE_COUNT_XFMR TO BO_USER
/
