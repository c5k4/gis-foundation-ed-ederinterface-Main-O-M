Prompt drop Trigger V126_INSERT;
DROP TRIGGER EDGIS.V126_INSERT
/

Prompt Trigger V126_INSERT;
--
-- V126_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V126_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_OPENPOINT REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',126); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A126 VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.NORMALPOSITION,:new.NORMALPOSITION_A,:new.NORMALPOSITION_B,:new.NORMALPOSITION_C,:new.SPLICESIZE,:new.SPLICETYPE,:new.MANUFACTURER,:new.RATEDAMPS,:new.LABELIDC,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CCRATING,:new.SERIALNUMBER,:new.MATERIALCODE,:new.YEARMANUFACTURED,:new.BUCKETTRUCKIDC,:new.CALTRANSPERMITIDC,:new.INSTALLATIONTYPE,:new.EXTERNALOPERATINGCODE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.SAPEQUIPID,:new.VERSIONNAME,:new.SECONDARYIDC,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SAPSORTFIELD,:new.INSULATINGFLUIDTYPE,:new.SUPERVISORYCONTROL,:new.SSDGUID);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.OPENPOINT VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.NORMALPOSITION,:new.NORMALPOSITION_A,:new.NORMALPOSITION_B,:new.NORMALPOSITION_C,:new.SPLICESIZE,:new.SPLICETYPE,:new.MANUFACTURER,:new.RATEDAMPS,:new.LABELIDC,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CCRATING,:new.SERIALNUMBER,:new.MATERIALCODE,:new.YEARMANUFACTURED,:new.BUCKETTRUCKIDC,:new.CALTRANSPERMITIDC,:new.INSTALLATIONTYPE,:new.EXTERNALOPERATINGCODE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.SAPEQUIPID,:new.VERSIONNAME,:new.SECONDARYIDC,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,:new.SAPSORTFIELD,:new.INSULATINGFLUIDTYPE,:new.SUPERVISORYCONTROL,:new.SSDGUID);  ELSE INSERT INTO EDGIS.A126  VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.NORMALPOSITION,:new.NORMALPOSITION_A,:new.NORMALPOSITION_B,:new.NORMALPOSITION_C,:new.SPLICESIZE,:new.SPLICETYPE,:new.MANUFACTURER,:new.RATEDAMPS,:new.LABELIDC,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.CCRATING,:new.SERIALNUMBER,:new.MATERIALCODE,:new.YEARMANUFACTURED,:new.BUCKETTRUCKIDC,:new.CALTRANSPERMITIDC,:new.INSTALLATIONTYPE,:new.EXTERNALOPERATINGCODE,:new.LOCALOFFICEID,:new.REGION,:new.INSTALLJOBNUMBER,:new.CITY,:new.OPERATINGNUMBER,:new.SAPEQUIPID,:new.VERSIONNAME,:new.SECONDARYIDC,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SAPSORTFIELD,:new.INSULATINGFLUIDTYPE,:new.SUPERVISORYCONTROL,:new.SSDGUID);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (126,current_state);  END IF;END;
/