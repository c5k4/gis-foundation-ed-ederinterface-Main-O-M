Prompt drop View ZPGEVWP_VOLTAGEREGULATOR;
DROP VIEW EDGIS.ZPGEVWP_VOLTAGEREGULATOR
/

/* Formatted on 7/2/2019 01:18:18 PM (QP5 v5.313) */
PROMPT View ZPGEVWP_VOLTAGEREGULATOR;
--
-- ZPGEVWP_VOLTAGEREGULATOR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVWP_VOLTAGEREGULATOR
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.VOLTAGEREGULATOR
      WHERE STATUS = 0
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D116 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A116
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A116
                            GROUP BY objectid)
                   AND STATUS = 0) A_Table
/


Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO DATACONV
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO PUBLIC
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO SDE
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_VOLTAGEREGULATOR TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVWP_VOLTAGEREGULATOR TO SDE_VIEWER
/
