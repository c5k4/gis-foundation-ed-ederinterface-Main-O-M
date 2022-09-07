Prompt drop View ZPGEVWP_PRIUGCONDUCTOR;
DROP VIEW EDGIS.ZPGEVWP_PRIUGCONDUCTOR
/

/* Formatted on 6/27/2019 02:59:12 PM (QP5 v5.313) */
PROMPT View ZPGEVWP_PRIUGCONDUCTOR;
--
-- ZPGEVWP_PRIUGCONDUCTOR  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVWP_PRIUGCONDUCTOR
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.PRIUGCONDUCTOR
      WHERE STATUS = 0
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D137 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A137
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A137
                            GROUP BY objectid)
                   AND STATUS = 0) A_Table
/


Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO DATACONV
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO PUBLIC
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO SDE
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_PRIUGCONDUCTOR TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVWP_PRIUGCONDUCTOR TO SDE_VIEWER
/