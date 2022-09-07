Prompt drop View ZPGEVW_DCRECTIFIER;
DROP VIEW EDGIS.ZPGEVW_DCRECTIFIER
/

/* Formatted on 7/2/2019 01:18:19 PM (QP5 v5.313) */
PROMPT View ZPGEVW_DCRECTIFIER;
--
-- ZPGEVW_DCRECTIFIER  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_DCRECTIFIER
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.DCRECTIFIER
      WHERE STATUS IN (1,
                       2,
                       3,
                       5,
                       30)
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D123 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A123
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A123
                            GROUP BY objectid)
                   AND STATUS IN (1,
                                  2,
                                  3,
                                  5,
                                  30)) A_Table
/


Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_DCRECTIFIER TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_DCRECTIFIER TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_DCRECTIFIER TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_DCRECTIFIER TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DCRECTIFIER TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DCRECTIFIER TO SDE
/

Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DCRECTIFIER TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_DCRECTIFIER TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_DCRECTIFIER TO SDE_VIEWER
/
