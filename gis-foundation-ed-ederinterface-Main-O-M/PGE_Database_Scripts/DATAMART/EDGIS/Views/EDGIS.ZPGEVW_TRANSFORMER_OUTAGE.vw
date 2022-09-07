Prompt drop View ZPGEVW_TRANSFORMER_OUTAGE;
DROP VIEW EDGIS.ZPGEVW_TRANSFORMER_OUTAGE
/

/* Formatted on 7/2/2019 01:18:30 PM (QP5 v5.313) */
PROMPT View ZPGEVW_TRANSFORMER_OUTAGE;
--
-- ZPGEVW_TRANSFORMER_OUTAGE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_TRANSFORMER_OUTAGE
(
    OBJECTID,
    GLOBALID,
    CGC12,
    SHAPE
)
AS
    SELECT OBJECTID, GLOBALID, CGC12, SHAPE FROM edgis.zz_mv_transformer
/


Prompt Grants on VIEW ZPGEVW_TRANSFORMER_OUTAGE TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ZPGEVW_TRANSFORMER_OUTAGE TO GIS_I
/

Prompt Grants on VIEW ZPGEVW_TRANSFORMER_OUTAGE TO SDE to SDE;
GRANT SELECT ON EDGIS.ZPGEVW_TRANSFORMER_OUTAGE TO SDE
/