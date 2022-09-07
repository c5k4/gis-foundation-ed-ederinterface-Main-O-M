Prompt drop View ZPGEVW_FDIDEVICES;
DROP VIEW EDGIS.ZPGEVW_FDIDEVICES
/

/* Formatted on 7/2/2019 01:18:23 PM (QP5 v5.313) */
PROMPT View ZPGEVW_FDIDEVICES;
--
-- ZPGEVW_FDIDEVICES  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_FDIDEVICES
(
    GUID,
    MAXASYMSUM,
    MAXASYMWIN,
    MAXSYMSUM,
    MAXSYMWIN,
    LINETOLINESUM,
    LINETOLINEWIN,
    LINETOLINETOGROUNDSUM,
    LINETOLINETOGROUNDWIN,
    LINETOGROUNDSUM,
    LINETOGROUNDWIN,
    PEAKLOADSUM,
    PEAKLOADWIN
)
AS
    SELECT NVL (S.GLOBALID, w.globalid) AS GUID,
           S.maxasym                    AS MAXASYMSUM,
           w.maxasym                    AS MAXASYMWIN,
           S.maxsym                     AS MAXSYMSUM,
           w.maxsym                     AS MAXSYMWIN,
           s.linetoline                 AS LINETOLINESUM,
           w.linetoline                 AS LINETOLINEWIN,
           s.linetolinetoground         AS LINETOLINETOGROUNDSUM,
           w.linetolinetoground         AS LINETOLINETOGROUNDWIN,
           s.linetoground               AS LINETOGROUNDSUM,
           w.linetoground               AS LINETOGROUNDWIN,
           s.peakload                   AS PEAKLOADSUM,
           w.peakload                   AS PEAKLOADWIN
      FROM EDGIS.PGE_FDIDEVICESSUM  S
           FULL OUTER JOIN PGE_FDIDEVICESWIN W ON S.GLOBALID = W.GLOBALID
     WHERE S.GLOBALID IS NOT NULL OR W.GLOBALID IS NOT NULL
/


Prompt Grants on VIEW ZPGEVW_FDIDEVICES TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_FDIDEVICES TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_FDIDEVICES TO GIS_I to GIS_I;
GRANT SELECT ON EDGIS.ZPGEVW_FDIDEVICES TO GIS_I
/

Prompt Grants on VIEW ZPGEVW_FDIDEVICES TO GIS_I_WRITE to GIS_I_WRITE;
GRANT SELECT ON EDGIS.ZPGEVW_FDIDEVICES TO GIS_I_WRITE
/

Prompt Grants on VIEW ZPGEVW_FDIDEVICES TO MM_ADMIN to MM_ADMIN;
GRANT SELECT ON EDGIS.ZPGEVW_FDIDEVICES TO MM_ADMIN
/

Prompt Grants on VIEW ZPGEVW_FDIDEVICES TO SDE_EDITOR to SDE_EDITOR;
GRANT SELECT ON EDGIS.ZPGEVW_FDIDEVICES TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_FDIDEVICES TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_FDIDEVICES TO SDE_VIEWER
/