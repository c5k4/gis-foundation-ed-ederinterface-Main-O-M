WHENEVER SQLERROR EXIT 1
TRUNCATE TABLE "EDGIS"."PGE_SUBGEOMNETWORK_TRACE";
INSERT INTO "EDGIS"."PGE_SUBGEOMNETWORK_TRACE" (SELECT * FROM EDGIS.PGE_SUBGEOMNETWORK_TRACE@EDERSUB);
COMMIT;
exit;