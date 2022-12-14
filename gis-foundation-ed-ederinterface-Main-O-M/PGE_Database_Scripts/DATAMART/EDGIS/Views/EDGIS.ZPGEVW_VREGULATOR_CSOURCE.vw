Prompt drop View ZPGEVW_VREGULATOR_CSOURCE;
DROP VIEW EDGIS.ZPGEVW_VREGULATOR_CSOURCE
/

/* Formatted on 7/2/2019 01:18:31 PM (QP5 v5.313) */
PROMPT View ZPGEVW_VREGULATOR_CSOURCE;
--
-- ZPGEVW_VREGULATOR_CSOURCE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_VREGULATOR_CSOURCE
(
    OBJECTID,
    GLOBALID,
    SUBTYPE,
    CIRCUITID,
    FEEDERNAME,
    OPERATINGNUMBER,
    HIGHSIDECONFIGURATION,
    RATEDAMPS,
    STATUS,
    SHAPE
)
AS
    SELECT EDGIS.VOLTAGEREGULATOR.OBJECTID,
           EDGIS.VOLTAGEREGULATOR.GLOBALID,
           EDGIS.VOLTAGEREGULATOR.SUBTYPECD,
           EDGIS.VOLTAGEREGULATOR.CIRCUITID,
              EDGIS.CIRCUITSOURCE.SUBSTATIONNAME
           || ' '
           || SUBSTR (EDGIS.CIRCUITSOURCE.CIRCUITNAME, -4, 4),
           EDGIS.VOLTAGEREGULATOR.OPERATINGNUMBER,
           EDGIS.VOLTAGEREGULATOR.HIGHSIDECONFIGURATION,
           EDGIS.VOLTAGEREGULATOR.RATEDAMPS,
           EDGIS.VOLTAGEREGULATOR.STATUS,
           EDGIS.VOLTAGEREGULATOR.SHAPE
      FROM EDGIS.VOLTAGEREGULATOR, EDGIS.CIRCUITSOURCE
     WHERE EDGIS.CIRCUITSOURCE.CIRCUITID = EDGIS.VOLTAGEREGULATOR.CIRCUITID
/


Prompt Grants on VIEW ZPGEVW_VREGULATOR_CSOURCE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_VREGULATOR_CSOURCE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_VREGULATOR_CSOURCE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_VREGULATOR_CSOURCE TO PUBLIC to PUBLIC;
GRANT SELECT ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_VREGULATOR_CSOURCE TO SDE to SDE;
GRANT SELECT ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO SDE WITH GRANT OPTION
/
GRANT DELETE, INSERT, REFERENCES, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO SDE
/

Prompt Grants on VIEW ZPGEVW_VREGULATOR_CSOURCE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_VREGULATOR_CSOURCE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_VREGULATOR_CSOURCE TO SDE_VIEWER
/
