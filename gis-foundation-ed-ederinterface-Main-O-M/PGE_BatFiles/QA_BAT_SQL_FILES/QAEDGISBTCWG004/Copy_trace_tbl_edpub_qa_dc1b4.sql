WHENEVER SQLERROR EXIT SQL.SQLCODE
UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_FEEDERFEDNETWORK_TRACE_A' where TRACETYPE IN ('ELECTRIC','SUBSTATION','SCHEMATICS');
EXECUTE SYS.DBMS_LOCK.SLEEP(30);
TRUNCATE TABLE "EDGIS"."PGE_FEEDERFEDNETWORK_TRACE_B";
INSERT INTO "EDGIS"."PGE_FEEDERFEDNETWORK_TRACE_B" (SELECT * FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW@EDER);
COMMIT;
exit;