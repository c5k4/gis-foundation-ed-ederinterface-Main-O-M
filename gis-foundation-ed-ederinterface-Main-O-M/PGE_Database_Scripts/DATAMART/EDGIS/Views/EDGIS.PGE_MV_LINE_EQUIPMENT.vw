Prompt drop View PGE_MV_LINE_EQUIPMENT;
DROP VIEW EDGIS.PGE_MV_LINE_EQUIPMENT
/

/* Formatted on 7/2/2019 01:18:06 PM (QP5 v5.313) */
PROMPT View PGE_MV_LINE_EQUIPMENT;
--
-- PGE_MV_LINE_EQUIPMENT  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.PGE_MV_LINE_EQUIPMENT
(
    FEEDERID,
    GLOBALID,
    COUNT,
    CGC12
)
AS
    (SELECT feederid,
            globalid,
            COUNT,
            NULL AS CGC12
       FROM EDGIS.pge_count_capacitor
     UNION
     SELECT feederid,
            globalid,
            COUNT,
            NULL AS CGC12
       FROM EDGIS.pge_count_dpd
     UNION
     SELECT feederid,
            globalid,
            COUNT,
            NULL AS CGC12
       FROM EDGIS.pge_count_fuse
     UNION
     SELECT feederid,
            globalid,
            COUNT,
            NULL AS CGC12
       FROM EDGIS.pge_count_switch
     UNION
     SELECT feederid,
            globalid,
            COUNT,
            NULL AS CGC12
       FROM EDGIS.pge_count_regulator
     UNION
     SELECT feederid,
            globalid,
            COUNT,
            NULL AS CGC12
       FROM EDGIS.pge_count_stepdown
     UNION
     SELECT feederid, globalid, COUNT, cgc12 FROM EDGIS.pge_count_xfmr)
/


Prompt Grants on VIEW PGE_MV_LINE_EQUIPMENT TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.PGE_MV_LINE_EQUIPMENT TO BO_USER
/
