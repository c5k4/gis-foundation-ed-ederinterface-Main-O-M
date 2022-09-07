WHENEVER SQLERROR EXIT SQL.SQLCODE
UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_UNDERGROUNDNETWORK_TRACE_A' where TRACETYPE = 'CONDUIT';
EXECUTE SYS.DBMS_LOCK.SLEEP(30);
TRUNCATE TABLE "EDGIS"."PGE_UNDERGROUNDNETWORK_TRACE_B";
INSERT INTO "EDGIS"."PGE_UNDERGROUNDNETWORK_TRACE_B" (SELECT * FROM EDGIS.PGE_UNDERGROUNDNETWORK_TRACE@EDER);
COMMIT;
exit;