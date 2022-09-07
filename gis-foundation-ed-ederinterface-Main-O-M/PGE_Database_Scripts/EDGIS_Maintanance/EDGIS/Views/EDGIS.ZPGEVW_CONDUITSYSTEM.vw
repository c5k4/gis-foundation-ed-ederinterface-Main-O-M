Prompt drop View ZPGEVW_CONDUITSYSTEM;
DROP VIEW EDGIS.ZPGEVW_CONDUITSYSTEM
/

/* Formatted on 6/27/2019 02:59:04 PM (QP5 v5.313) */
PROMPT View ZPGEVW_CONDUITSYSTEM;
--
-- ZPGEVW_CONDUITSYSTEM  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_CONDUITSYSTEM
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.CONDUITSYSTEM
      WHERE STATUS IN (1,
                       2,
                       3,
                       5,
                       30)
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D134 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A134
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A134
                            GROUP BY objectid)
                   AND STATUS IN (1,
                                  2,
                                  3,
                                  5,
                                  30)) A_Table
/


Prompt Grants on VIEW ZPGEVW_CONDUITSYSTEM TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_CONDUITSYSTEM TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_CONDUITSYSTEM TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_CONDUITSYSTEM TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVW_CONDUITSYSTEM TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_CONDUITSYSTEM TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_CONDUITSYSTEM TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_CONDUITSYSTEM TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_CONDUITSYSTEM TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_CONDUITSYSTEM TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_CONDUITSYSTEM TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_CONDUITSYSTEM TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_CONDUITSYSTEM TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_CONDUITSYSTEM TO SDE_VIEWER
/
