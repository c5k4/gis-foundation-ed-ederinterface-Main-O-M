Prompt drop Procedure REMOVE_DUPLICATES;
DROP PROCEDURE DMSSTAGING.REMOVE_DUPLICATES
/

Prompt Procedure REMOVE_DUPLICATES;
--
-- REMOVE_DUPLICATES  (Procedure) 
--
CREATE OR REPLACE PROCEDURE DMSSTAGING.REMOVE_DUPLICATES AS
BEGIN
  DELETE FROM source s where s.feature_class = 'SUBElectricStitchPoint' AND s.sofpos in (select nfpos from node where feature_class = 'ElectricStitchPoint');
  DELETE FROM node n where n.feature_class = 'SUBElectricStitchPoint' AND n.nfpos in (select nfpos from node where feature_class = 'ElectricStitchPoint');
  DELETE FROM DEVICE A WHERE a.rowid > ANY (SELECT B.rowid FROM DEVICE B WHERE a.dfpos = b.dfpos);
  DELETE FROM NODE A WHERE a.rowid > ANY (SELECT B.rowid FROM NODE B WHERE a.nfpos = b.nfpos);
  DELETE FROM SITE A WHERE a.rowid > ANY (SELECT B.rowid FROM SITE B WHERE a.sifpos = b.sifpos);
  delete from node where feature_class = 'DeviceGroup' and nfpos not in (select no_key from site);
END REMOVE_DUPLICATES;
/


Prompt Grants on PROCEDURE REMOVE_DUPLICATES TO GISINTERFACE to GISINTERFACE;
GRANT EXECUTE ON DMSSTAGING.REMOVE_DUPLICATES TO GISINTERFACE
/

Prompt Grants on PROCEDURE REMOVE_DUPLICATES TO GIS_INTERFACE to GIS_INTERFACE;
GRANT EXECUTE ON DMSSTAGING.REMOVE_DUPLICATES TO GIS_INTERFACE
/
