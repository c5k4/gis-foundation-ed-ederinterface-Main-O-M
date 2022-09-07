Prompt drop View ZPGEVWP_CAPACITORBANK;
DROP VIEW EDGIS.ZPGEVWP_CAPACITORBANK
/

/* Formatted on 6/27/2019 02:59:21 PM (QP5 v5.313) */
PROMPT View ZPGEVWP_CAPACITORBANK;
--
-- ZPGEVWP_CAPACITORBANK  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVWP_CAPACITORBANK
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.CAPACITORBANK
      WHERE STATUS = 0
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D124 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A124
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A124
                            GROUP BY objectid)
                   AND STATUS = 0) A_Table
/


Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZPGEVWP_CAPACITORBANK TO DATACONV
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_CAPACITORBANK TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_CAPACITORBANK TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_CAPACITORBANK TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_CAPACITORBANK TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_CAPACITORBANK TO PUBLIC
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_CAPACITORBANK TO SDE
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_CAPACITORBANK TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_CAPACITORBANK TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVWP_CAPACITORBANK TO SDE_VIEWER
/