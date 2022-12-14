Prompt drop View ZPGEVW_FAULTINDICATOR;
DROP VIEW EDGIS.ZPGEVW_FAULTINDICATOR
/

/* Formatted on 6/27/2019 02:58:58 PM (QP5 v5.313) */
PROMPT View ZPGEVW_FAULTINDICATOR;
--
-- ZPGEVW_FAULTINDICATOR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_FAULTINDICATOR
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.FAULTINDICATOR
      WHERE STATUS IN (1,
                       2,
                       3,
                       5,
                       30)
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D120 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A120
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A120
                            GROUP BY objectid)
                   AND STATUS IN (1,
                                  2,
                                  3,
                                  5,
                                  30)) A_Table
/


Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_FAULTINDICATOR TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_FAULTINDICATOR TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_FAULTINDICATOR TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_FAULTINDICATOR TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_FAULTINDICATOR TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_FAULTINDICATOR TO SDE
/

Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_FAULTINDICATOR TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_FAULTINDICATOR TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_FAULTINDICATOR TO SDE_VIEWER
/
