Prompt drop View ZPGEVWP_SECONDARYGENERATION;
DROP VIEW EDGIS.ZPGEVWP_SECONDARYGENERATION
/

/* Formatted on 6/27/2019 02:59:11 PM (QP5 v5.313) */
PROMPT View ZPGEVWP_SECONDARYGENERATION;
--
-- ZPGEVWP_SECONDARYGENERATION  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVWP_SECONDARYGENERATION
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.SECONDARYGENERATION
      WHERE STATUS = 0
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D131 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A131
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A131
                            GROUP BY objectid)
                   AND STATUS = 0) A_Table
/


Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO DATACONV to DATACONV;
GRANT SELECT ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO DATACONV
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO PUBLIC
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO SDE
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVWP_SECONDARYGENERATION TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVWP_SECONDARYGENERATION TO SDE_VIEWER
/