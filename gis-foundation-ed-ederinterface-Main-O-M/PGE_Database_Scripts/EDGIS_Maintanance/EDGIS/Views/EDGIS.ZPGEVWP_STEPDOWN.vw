Prompt drop View ZPGEVWP_STEPDOWN;
DROP VIEW EDGIS.ZPGEVWP_STEPDOWN
/

/* Formatted on 6/27/2019 02:59:10 PM (QP5 v5.313) */
PROMPT View ZPGEVWP_STEPDOWN;
--
-- ZPGEVWP_STEPDOWN  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVWP_STEPDOWN
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.STEPDOWN
      WHERE STATUS = 0
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D118 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A118
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A118
                            GROUP BY objectid)
                   AND STATUS = 0) A_Table
/


Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZPGEVWP_STEPDOWN TO DATACONV
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_STEPDOWN TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_STEPDOWN TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_STEPDOWN TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_STEPDOWN TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_STEPDOWN TO PUBLIC
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_STEPDOWN TO SDE
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_STEPDOWN TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_STEPDOWN TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVWP_STEPDOWN TO SDE_VIEWER
/
