Prompt drop View ZPGEVW_SUBSURFACESTRUCTURE;
DROP VIEW EDGIS.ZPGEVW_SUBSURFACESTRUCTURE
/

/* Formatted on 6/27/2019 02:58:43 PM (QP5 v5.313) */
PROMPT View ZPGEVW_SUBSURFACESTRUCTURE;
--
-- ZPGEVW_SUBSURFACESTRUCTURE  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_SUBSURFACESTRUCTURE
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.SUBSURFACESTRUCTURE
      WHERE STATUS IN (1,
                       2,
                       3,
                       5,
                       30)
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D133 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A133
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A133
                            GROUP BY objectid)
                   AND STATUS IN (1,
                                  2,
                                  3,
                                  5,
                                  30)) A_Table
/


Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO SDE
/

Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_SUBSURFACESTRUCTURE TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_SUBSURFACESTRUCTURE TO SDE_VIEWER
/
