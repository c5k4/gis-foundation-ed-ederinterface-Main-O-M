Prompt drop View ZPGEVWP_DCCONDUCTOR50;
DROP VIEW EDGIS.ZPGEVWP_DCCONDUCTOR50
/

/* Formatted on 7/2/2019 01:18:09 PM (QP5 v5.313) */
PROMPT View ZPGEVWP_DCCONDUCTOR50;
--
-- ZPGEVWP_DCCONDUCTOR50  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVWP_DCCONDUCTOR50
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.DCCONDUCTOR
      WHERE STATUS = 0
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
                   AND STATUS = 0) A_Table
/


Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO DATACONV
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO PUBLIC
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO SDE
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_DCCONDUCTOR50 TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVWP_DCCONDUCTOR50 TO SDE_VIEWER
/
