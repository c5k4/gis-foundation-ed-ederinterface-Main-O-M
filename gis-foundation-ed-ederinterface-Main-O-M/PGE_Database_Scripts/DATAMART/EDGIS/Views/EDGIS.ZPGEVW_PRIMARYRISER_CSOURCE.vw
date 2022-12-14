Prompt drop View ZPGEVW_PRIMARYRISER_CSOURCE;
DROP VIEW EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE
/

/* Formatted on 7/2/2019 01:18:25 PM (QP5 v5.313) */
PROMPT View ZPGEVW_PRIMARYRISER_CSOURCE;
--
-- ZPGEVW_PRIMARYRISER_CSOURCE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE
(
    OBJECTID,
    GLOBALID,
    SUBTYPE,
    CIRCUITID,
    FEEDERNAME,
    SHAPE
)
AS
    SELECT EDGIS.PrimaryRiser.OBJECTID,
           EDGIS.PrimaryRiser.GLOBALID,
           EDGIS.PrimaryRiser.SUBTYPECD,
           EDGIS.PrimaryRiser.CIRCUITID,
              EDGIS.CIRCUITSOURCE.SUBSTATIONNAME
           || ' '
           || SUBSTR (EDGIS.CIRCUITSOURCE.CIRCUITNAME, -4, 4),
           EDGIS.PrimaryRiser.SHAPE
      FROM EDGIS.PRIMARYRISER, EDGIS.CIRCUITSOURCE
     WHERE EDGIS.CIRCUITSOURCE.CIRCUITID = EDGIS.PrimaryRiser.CIRCUITID
/


Prompt Grants on VIEW ZPGEVW_PRIMARYRISER_CSOURCE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_PRIMARYRISER_CSOURCE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_PRIMARYRISER_CSOURCE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_PRIMARYRISER_CSOURCE TO PUBLIC to PUBLIC;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_PRIMARYRISER_CSOURCE TO SDE to SDE;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO SDE WITH GRANT OPTION
/
GRANT DELETE, INSERT, REFERENCES, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO SDE
/

Prompt Grants on VIEW ZPGEVW_PRIMARYRISER_CSOURCE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_PRIMARYRISER_CSOURCE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYRISER_CSOURCE TO SDE_VIEWER
/
