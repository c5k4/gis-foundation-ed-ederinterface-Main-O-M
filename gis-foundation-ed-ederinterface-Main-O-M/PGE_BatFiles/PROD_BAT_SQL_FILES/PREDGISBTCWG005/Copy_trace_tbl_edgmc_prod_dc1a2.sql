-- UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_FEEDERFEDNETWORK_TRACE_A' where TRACETYPE IN ('ELECTRIC','SUBSTATION','SCHEMATICS');
-- COMMIT;
-- EXECUTE SYS.DBMS_LOCK.SLEEP(30);
WHENEVER SQLERROR EXIT 1
TRUNCATE TABLE EDGIS.PGE_FEEDERFEDNETWORK_TRACE;
COPY FROM EDGIS/&1@EDER INSERT EDGIS.PGE_FEEDERFEDNETWORK_TRACE USING SELECT * FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE;
-- UPDATE EDGIS.TRACE_CACHE_CONFIG SET TRACETABLENAME = 'EDGIS.PGE_FEEDERFEDNETWORK_TRACE_B' where TRACETYPE IN ('ELECTRIC','SUBSTATION','SCHEMATICS');
-- COMMIT;
-- EXECUTE SYS.DBMS_LOCK.SLEEP(30);
-- TRUNCATE TABLE EDGIS.PGE_FEEDERFEDNETWORK_TRACE_A;
-- COPY FROM EDGIS/&1@EDER INSERT EDGIS.PGE_FEEDERFEDNETWORK_TRACE_A USING SELECT * FROM EDGIS.PGE_FEEDERFEDNETWORK_TRACE_VW;
-- TRUNCATE TABLE EDGIS.PGE_CIRCUITSOURCE;
-- COPY FROM EDGIS/&1@EDER INSERT EDGIS.PGE_CIRCUITSOURCE USING SELECT * FROM EDGIS.PGE_CIRCUITSOURCE;
exit;