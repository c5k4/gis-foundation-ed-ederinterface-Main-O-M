WHENEVER SQLERROR EXIT SQL.SQLCODE
UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_SUBGEOMNETWORK_TRACE_B' where TRACETYPE = 'SUBSTATION';
EXECUTE SYS.DBMS_LOCK.SLEEP(30);
TRUNCATE TABLE "EDGIS"."PGE_SUBGEOMNETWORK_TRACE_A";
INSERT INTO "EDGIS"."PGE_SUBGEOMNETWORK_TRACE_A" (SELECT * FROM EDGIS.PGE_SUBGEOMNETWORK_TRACE@EDERSUB);
COMMIT;
UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_SUBGEOMNETWORK_TRACE_A' where TRACETYPE = 'SUBSTATION';
EXECUTE SYS.DBMS_LOCK.SLEEP(30);
TRUNCATE TABLE "EDGIS"."PGE_SUBGEOMNETWORK_TRACE_B";
INSERT INTO "EDGIS"."PGE_SUBGEOMNETWORK_TRACE_B" (SELECT * FROM EDGIS.PGE_SUBGEOMNETWORK_TRACE@EDERSUB);
COMMIT;
EXIT;