Prompt drop View PGE_COUNT_STEPDOWN;
DROP VIEW EDGIS.PGE_COUNT_STEPDOWN
/

/* Formatted on 7/2/2019 01:18:06 PM (QP5 v5.313) */
PROMPT View PGE_COUNT_STEPDOWN;
--
-- PGE_COUNT_STEPDOWN  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.PGE_COUNT_STEPDOWN
(
    FEEDERID,
    GLOBALID,
    COUNT
)
AS
    (  SELECT source.feederid          AS FEEDERID,
              source.TO_FEATURE_GLOBALID AS GLOBALID,
              COUNT (*)                AS COUNT
         FROM (SELECT MIN_BRANCH,
                      MAX_BRANCH,
                      TREELEVEL,
                      ORDER_NUM,
                      FEEDERID,
                      TO_FEATURE_GLOBALID
                 FROM edgis.PGE_ELECDISTNETWORK_TRACE
                WHERE TO_FEATURE_FCID =
                      (SELECT OBJECTID
                         FROM sde.gdb_items
                        WHERE     PHYSICALNAME = 'EDGIS.STEPDOWN'
                              AND TYPE IN
                                      (SELECT UUID
                                         FROM SDE.GDB_ITEMTYPES
                                        WHERE NAME IN ('Feature Dataset',
                                                       'Feature Class',
                                                       'Relationship Class',
                                                       'Table',
                                                       'Schematic Dataset',
                                                       'Dataset')))) source
              JOIN
              (SELECT TO_FEATURE_GLOBALID globalid,
                      MIN_BRANCH,
                      MAX_BRANCH,
                      TREELEVEL,
                      ORDER_NUM,
                      FEEDERID
                 FROM edgis.PGE_ELECDISTNETWORK_TRACE
                WHERE TO_FEATURE_FCID =
                      (SELECT OBJECTID
                         FROM sde.gdb_items
                        WHERE     PHYSICALNAME = 'EDGIS.SERVICELOCATION'
                              AND TYPE IN
                                      (SELECT UUID
                                         FROM SDE.GDB_ITEMTYPES
                                        WHERE NAME IN ('Feature Dataset',
                                                       'Feature Class',
                                                       'Relationship Class',
                                                       'Table',
                                                       'Schematic Dataset',
                                                       'Dataset')))) SL
                  ON     SL.MIN_BRANCH >= source.MIN_BRANCH
                     AND SL.MAX_BRANCH <= source.MAX_BRANCH
                     AND SL.TREELEVEL >= source.TREELEVEL
                     AND SL.ORDER_NUM <= source.ORDER_NUM
                     AND SL.FEEDERID = source.FEEDERID
              INNER JOIN edgis.servicepoint sp
                  ON sl.globalid = sp.servicelocationguid
     GROUP BY source.feederid, source.TO_FEATURE_GLOBALID)
/


Prompt Grants on VIEW PGE_COUNT_STEPDOWN TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.PGE_COUNT_STEPDOWN TO BO_USER
/
