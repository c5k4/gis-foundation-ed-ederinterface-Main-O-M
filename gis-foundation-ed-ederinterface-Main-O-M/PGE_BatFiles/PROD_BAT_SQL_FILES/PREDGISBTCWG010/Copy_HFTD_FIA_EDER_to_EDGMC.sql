TRUNCATE TABLE "EDGIS"."PGE_HFTD_FIA_VALUES";
INSERT INTO "EDGIS"."PGE_HFTD_FIA_VALUES" (SELECT * FROM EDGIS.PGE_HFTD_FIA_VALUES@EDER);
COMMIT;
exit;