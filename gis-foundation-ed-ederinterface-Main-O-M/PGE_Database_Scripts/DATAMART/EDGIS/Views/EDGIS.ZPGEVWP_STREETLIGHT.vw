Prompt drop View ZPGEVWP_STREETLIGHT;
DROP VIEW EDGIS.ZPGEVWP_STREETLIGHT
/

/* Formatted on 7/2/2019 01:18:15 PM (QP5 v5.313) */
PROMPT View ZPGEVWP_STREETLIGHT;
--
-- ZPGEVWP_STREETLIGHT  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVWP_STREETLIGHT
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.STREETLIGHT
      WHERE STATUS = 0
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D127 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A127
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A127
                            GROUP BY objectid)
                   AND STATUS = 0) A_Table
/


Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZPGEVWP_STREETLIGHT TO DATACONV
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_STREETLIGHT TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_STREETLIGHT TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_STREETLIGHT TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_STREETLIGHT TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_STREETLIGHT TO PUBLIC
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_STREETLIGHT TO SDE
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_STREETLIGHT TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_STREETLIGHT TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVWP_STREETLIGHT TO SDE_VIEWER
/