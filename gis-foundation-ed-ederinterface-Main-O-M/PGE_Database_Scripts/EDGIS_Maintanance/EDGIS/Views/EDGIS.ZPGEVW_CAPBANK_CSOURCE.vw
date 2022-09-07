Prompt drop View ZPGEVW_CAPBANK_CSOURCE;
DROP VIEW EDGIS.ZPGEVW_CAPBANK_CSOURCE
/

/* Formatted on 6/27/2019 02:59:04 PM (QP5 v5.313) */
PROMPT View ZPGEVW_CAPBANK_CSOURCE;
--
-- ZPGEVW_CAPBANK_CSOURCE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_CAPBANK_CSOURCE
(
    OBJECTID,
    GLOBALID,
    SUBTYPE,
    CIRCUITID,
    FEEDERNAME,
    OPERATINGNUMBER,
    SAPEQUIPID,
    TOTALKVAR,
    PROTECTIVESSD,
    AUTOPROTECTIVESSD,
    STATUS,
    SHAPE
)
AS
    SELECT EDGIS.CAPACITORBANK.OBJECTID,
           EDGIS.CAPACITORBANK.GLOBALID,
           EDGIS.CAPACITORBANK.SUBTYPECD,
           EDGIS.CAPACITORBANK.CIRCUITID,
              EDGIS.CIRCUITSOURCE.SUBSTATIONNAME
           || ' '
           || SUBSTR (EDGIS.CIRCUITSOURCE.CIRCUITNAME, -4, 4),
           EDGIS.CAPACITORBANK.OPERATINGNUMBER,
           EDGIS.CAPACITORBANK.SAPEQUIPID,
           EDGIS.CAPACITORBANK.TOTALKVAR,
           EDGIS.CAPACITORBANK.PROTECTIVESSD,
           EDGIS.CAPACITORBANK.AUTOPROTECTIVESSD,
           EDGIS.CAPACITORBANK.STATUS,
           EDGIS.CAPACITORBANK.SHAPE
      FROM EDGIS.CAPACITORBANK, EDGIS.CIRCUITSOURCE
     WHERE EDGIS.CIRCUITSOURCE.CIRCUITID = EDGIS.CAPACITORBANK.CIRCUITID
/


Prompt Grants on VIEW ZPGEVW_CAPBANK_CSOURCE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_CAPBANK_CSOURCE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_CAPBANK_CSOURCE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_CAPBANK_CSOURCE TO PUBLIC to PUBLIC;
GRANT SELECT ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_CAPBANK_CSOURCE TO SDE to SDE;
GRANT SELECT ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO SDE WITH GRANT OPTION
/
GRANT DELETE, INSERT, REFERENCES, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO SDE
/

Prompt Grants on VIEW ZPGEVW_CAPBANK_CSOURCE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_CAPBANK_CSOURCE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_CAPBANK_CSOURCE TO SDE_VIEWER
/