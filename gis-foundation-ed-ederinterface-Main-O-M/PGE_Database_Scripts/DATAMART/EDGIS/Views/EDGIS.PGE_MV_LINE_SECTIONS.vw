Prompt drop View PGE_MV_LINE_SECTIONS;
DROP VIEW EDGIS.PGE_MV_LINE_SECTIONS
/

/* Formatted on 7/2/2019 01:18:07 PM (QP5 v5.313) */
PROMPT View PGE_MV_LINE_SECTIONS;
--
-- PGE_MV_LINE_SECTIONS  (View)
--

CREATE OR REPLACE FORCE VIEW EDGIS.PGE_MV_LINE_SECTIONS
(
    FEEDERID,
    GLOBALID,
    COUNT
)
AS
    (  SELECT FEEDERID, GLOBALID, COUNT (1) AS COUNT
         FROM (SELECT  /*+  use_merge(source,SL) use_merge(SL, sp) PARALLEL(source) PARALLEL(SL) PARALLEL(sp) */
                      source.feederid          AS FEEDERID,
                      source.TO_FEATURE_GLOBALID AS GLOBALID
                 FROM (SELECT MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID,
                              TO_FEATURE_GLOBALID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.PRIOHCONDUCTOR'))
                      source
                      INNER JOIN
                      (SELECT TO_FEATURE_GLOBALID globalid,
                              MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.SERVICELOCATION'))
                      SL
                          ON SL.FEEDERID = source.FEEDERID
                      INNER JOIN edgis.servicepoint sp
                          ON SL.globalid = sp.servicelocationguid
                WHERE     SL.MIN_BRANCH >= source.MIN_BRANCH
                      AND SL.MAX_BRANCH <= source.MAX_BRANCH
                      AND SL.TREELEVEL >= source.TREELEVEL
                      AND SL.ORDER_NUM <= source.ORDER_NUM) A
     GROUP BY FEEDERID, GLOBALID)
    UNION
    (  SELECT FEEDERID, GLOBALID, COUNT (1) AS COUNT
         FROM (SELECT  /*+  use_merge(source,SL) use_merge(SL, sp) PARALLEL(source) PARALLEL(SL) PARALLEL(sp) */
                      source.feederid          AS FEEDERID,
                      source.TO_FEATURE_GLOBALID AS GLOBALID
                 FROM (SELECT MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID,
                              TO_FEATURE_GLOBALID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.SECOHCONDUCTOR'))
                      source
                      INNER JOIN
                      (SELECT TO_FEATURE_GLOBALID globalid,
                              MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.SERVICELOCATION'))
                      SL
                          ON SL.FEEDERID = source.FEEDERID
                      INNER JOIN edgis.servicepoint sp
                          ON SL.globalid = sp.servicelocationguid
                WHERE     SL.MIN_BRANCH >= source.MIN_BRANCH
                      AND SL.MAX_BRANCH <= source.MAX_BRANCH
                      AND SL.TREELEVEL >= source.TREELEVEL
                      AND SL.ORDER_NUM <= source.ORDER_NUM) A
     GROUP BY FEEDERID, GLOBALID)
    UNION
    (  SELECT FEEDERID, GLOBALID, COUNT (1) AS COUNT
         FROM (SELECT  /*+  use_merge(source,SL) use_merge(SL, sp) PARALLEL(source) PARALLEL(SL) PARALLEL(sp) */
                      source.feederid          AS FEEDERID,
                      source.TO_FEATURE_GLOBALID AS GLOBALID
                 FROM (SELECT MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID,
                              TO_FEATURE_GLOBALID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.PRIUGCONDUCTOR'))
                      source
                      INNER JOIN
                      (SELECT TO_FEATURE_GLOBALID globalid,
                              MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.SERVICELOCATION'))
                      SL
                          ON SL.FEEDERID = source.FEEDERID
                      INNER JOIN edgis.servicepoint sp
                          ON SL.globalid = sp.servicelocationguid
                WHERE     SL.MIN_BRANCH >= source.MIN_BRANCH
                      AND SL.MAX_BRANCH <= source.MAX_BRANCH
                      AND SL.TREELEVEL >= source.TREELEVEL
                      AND SL.ORDER_NUM <= source.ORDER_NUM) A
     GROUP BY FEEDERID, GLOBALID)
    UNION
    (  SELECT FEEDERID, GLOBALID, COUNT (1) AS COUNT
         FROM (SELECT  /*+  use_merge(source,SL) use_merge(SL, sp) PARALLEL(source) PARALLEL(SL) PARALLEL(sp) */
                      source.feederid          AS FEEDERID,
                      source.TO_FEATURE_GLOBALID AS GLOBALID
                 FROM (SELECT MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID,
                              TO_FEATURE_GLOBALID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.SECUGCONDUCTOR'))
                      source
                      INNER JOIN
                      (SELECT TO_FEATURE_GLOBALID globalid,
                              MIN_BRANCH,
                              MAX_BRANCH,
                              TREELEVEL,
                              ORDER_NUM,
                              FEEDERID
                         FROM EDGIS.PGE_ELECDISTNETWORK_TRACE
                        WHERE TO_FEATURE_FCID =
                              (SELECT OBJECTID
                                 FROM sde.gdb_items
                                WHERE PHYSICALNAME = 'EDGIS.SERVICELOCATION'))
                      SL
                          ON SL.FEEDERID = source.FEEDERID
                      INNER JOIN edgis.servicepoint sp
                          ON SL.globalid = sp.servicelocationguid
                WHERE     SL.MIN_BRANCH >= source.MIN_BRANCH
                      AND SL.MAX_BRANCH <= source.MAX_BRANCH
                      AND SL.TREELEVEL >= source.TREELEVEL
                      AND SL.ORDER_NUM <= source.ORDER_NUM) A
     GROUP BY FEEDERID, GLOBALID)
/


Prompt Grants on VIEW PGE_MV_LINE_SECTIONS TO BO_USER to BO_USER;
GRANT SELECT ON EDGIS.PGE_MV_LINE_SECTIONS TO BO_USER
/
