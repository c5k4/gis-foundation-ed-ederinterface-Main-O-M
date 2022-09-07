Prompt drop View ZPGEVW_PRIMARYMETER_CSOURCE;
DROP VIEW EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE
/

/* Formatted on 7/2/2019 01:18:25 PM (QP5 v5.313) */
PROMPT View ZPGEVW_PRIMARYMETER_CSOURCE;
--
-- ZPGEVW_PRIMARYMETER_CSOURCE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE
(
    OBJECTID,
    GLOBALID,
    SUBTYPE,
    CIRCUITID,
    FEEDERNAME,
    STATUS,
    SHAPE
)
AS
    SELECT EDGIS.PrimaryMeter.OBJECTID,
           EDGIS.PrimaryMeter.GLOBALID,
           EDGIS.PrimaryMeter.SUBTYPECD,
           EDGIS.PrimaryMeter.CIRCUITID,
              EDGIS.CIRCUITSOURCE.SUBSTATIONNAME
           || ' '
           || SUBSTR (EDGIS.CIRCUITSOURCE.CIRCUITNAME, -4, 4),
           EDGIS.PrimaryMeter.STATUS,
           EDGIS.PrimaryMeter.SHAPE
      FROM EDGIS.PRIMARYMETER, EDGIS.CIRCUITSOURCE
     WHERE EDGIS.CIRCUITSOURCE.CIRCUITID = EDGIS.PrimaryMeter.CIRCUITID
/


Prompt Grants on VIEW ZPGEVW_PRIMARYMETER_CSOURCE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_PRIMARYMETER_CSOURCE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_PRIMARYMETER_CSOURCE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_PRIMARYMETER_CSOURCE TO PUBLIC to PUBLIC;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_PRIMARYMETER_CSOURCE TO SDE to SDE;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO SDE WITH GRANT OPTION
/
GRANT DELETE, INSERT, REFERENCES, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO SDE
/

Prompt Grants on VIEW ZPGEVW_PRIMARYMETER_CSOURCE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_PRIMARYMETER_CSOURCE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_PRIMARYMETER_CSOURCE TO SDE_VIEWER
/
