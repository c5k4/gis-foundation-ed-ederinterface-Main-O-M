Prompt drop View ZPGEVW_DYNAMICPROTECTIVEDEV;
DROP VIEW EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV
/

/* Formatted on 7/2/2019 01:18:22 PM (QP5 v5.313) */
PROMPT View ZPGEVW_DYNAMICPROTECTIVEDEV;
--
-- ZPGEVW_DYNAMICPROTECTIVEDEV  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV
(
    OBJECTID
)
AS
    (SELECT OBJECTID
       FROM EDGIS.DYNAMICPROTECTIVEDEVICE
      WHERE STATUS IN (1,
                       2,
                       3,
                       5,
                       30)
     MINUS
     SELECT d_table.SDE_DELETES_ROW_ID
       FROM EDGIS.D114 d_table)
    UNION
    SELECT A_Table.OBJECTID
      FROM (SELECT ObjectID, STATUS
              FROM EDGIS.A114
             WHERE     (objectid, sde_state_id) IN
                           (  SELECT objectid, MAX (sde_state_id)
                                FROM EDGIS.A114
                            GROUP BY objectid)
                   AND STATUS IN (1,
                                  2,
                                  3,
                                  5,
                                  30)) A_Table
/


Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO DAT_EDITOR to DAT_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO DAT_EDITOR
/

Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO DMSSTAGING to DMSSTAGING;
GRANT DELETE, INSERT, SELECT, UPDATE ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO DMSSTAGING
/

Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO GISINTERFACE to GISINTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO GISINTERFACE
/

Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO GIS_INTERFACE to GIS_INTERFACE;
GRANT SELECT ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO GIS_INTERFACE
/

Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO PUBLIC to PUBLIC;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO PUBLIC
/

Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO SDE to SDE;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO SDE
/

Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO SDE_EDITOR to SDE_EDITOR;
GRANT DELETE, INSERT, SELECT, UPDATE, ON COMMIT REFRESH, QUERY REWRITE, DEBUG, FLASHBACK, MERGE VIEW ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO SDE_EDITOR
/

Prompt Grants on VIEW ZPGEVW_DYNAMICPROTECTIVEDEV TO SDE_VIEWER to SDE_VIEWER;
GRANT SELECT ON EDGIS.ZPGEVW_DYNAMICPROTECTIVEDEV TO SDE_VIEWER
/