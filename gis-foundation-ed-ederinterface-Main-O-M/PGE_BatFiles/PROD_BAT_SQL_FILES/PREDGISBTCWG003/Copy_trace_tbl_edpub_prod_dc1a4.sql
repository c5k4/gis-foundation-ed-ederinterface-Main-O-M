WHENEVER SQLERROR EXIT 1
UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_FEEDERFEDNETWORK_TRACE_B' where TRACETYPE IN ('ELECTRIC','SUBSTATION','SCHEMATICS');
EXECUTE SYS.DBMS_LOCK.SLEEP(120);
TRUNCATE TABLE "EDGIS"."PGE_FEEDERFEDNETWORK_TRACE_A";
--INSERT INTO "EDGIS"."PGE_FEEDERFEDNETWORK_TRACE_A" (SELECT * FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW@EDER);
COPY FROM EDGIS/&1@EDER INSERT EDGIS.PGE_FEEDERFEDNETWORK_TRACE_A USING SELECT * FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW;
COMMIT;
UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_FEEDERFEDNETWORK_TRACE_A' where TRACETYPE IN ('ELECTRIC','SUBSTATION','SCHEMATICS');
EXECUTE SYS.DBMS_LOCK.SLEEP(120);
TRUNCATE TABLE "EDGIS"."PGE_FEEDERFEDNETWORK_TRACE_B";
--INSERT INTO "EDGIS"."PGE_FEEDERFEDNETWORK_TRACE_B" (SELECT * FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW@EDER);
COPY FROM EDGIS/&1@EDER INSERT EDGIS.PGE_FEEDERFEDNETWORK_TRACE_B USING SELECT * FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW;
COMMIT;
TRUNCATE TABLE "EDGIS"."PGE_CIRCUITSOURCE";
--INSERT INTO "EDGIS"."PGE_CIRCUITSOURCE" (SELECT * FROM EDGIS.PGE_CIRCUITSOURCE@EDER);
COPY FROM EDGIS/&1@EDER INSERT EDGIS.PGE_CIRCUITSOURCE USING SELECT * FROM EDGIS.PGE_CIRCUITSOURCE;
COMMIT;
EXIT;