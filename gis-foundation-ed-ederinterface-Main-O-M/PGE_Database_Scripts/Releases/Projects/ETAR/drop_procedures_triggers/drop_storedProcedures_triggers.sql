  --Drop stored procedures and trigger, these are replaced with new onces.
  DROP PROCEDURE EDGIS.GIS_INSERT_FNM_RECORDS;
  DROP PROCEDURE EDGIS.GIS_MERGE_FNM_RECORDS;
  DROP TRIGGER EDGIS.GIS_UPDATE_FNM_HISTORY;
  COMMIT;
  /