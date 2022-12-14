Prompt drop View ZPGEVW_DCCONDUCTOR50;
DROP VIEW EDGIS.ZPGEVW_DCCONDUCTOR50
/

/* Formatted on 6/27/2019 02:59:03 PM (QP5 v5.313) */
PROMPT View ZPGEVW_DCCONDUCTOR50;
--
-- ZPGEVW_DCCONDUCTOR50  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_DCCONDUCTOR50
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.DCCONDUCTOR
      WHERE STATUS IN (1,
                       2,
                       3,
                       5,
                       30)
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D136 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A136
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A136
                            GROUP BY objectid)
                   AND STATUS IN (1,
                                  2,
                                  3,
                                  5,
                                  30)) A_Table
/


Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO SDE
/

Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_DCCONDUCTOR50 TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_DCCONDUCTOR50 TO SDE_VIEWER
/
