Prompt drop Trigger V120_INSERT;
DROP TRIGGER EDGIS.V120_INSERT
/

Prompt Trigger V120_INSERT;
--
-- V120_INSERT  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V120_INSERT INSTEAD OF INSERT ON EDGIS.ZZ_MV_FAULTINDICATOR REFERENCING NEW AS NEW
DECLARE current_state SDE.version_util.state_id_t;next_rowid INTEGER;BEGIN /* ArcSDE plsql */ If :new.OBJECTID IS NOT NULL Then next_rowid := :new.OBJECTID; Else next_rowid := SDE.version_user_ddl.next_row_id ('EDGIS',120); End If; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; INSERT INTO EDGIS.A120 VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.ACTUATINGCURRENT,:new.FITYPE,:new.FIBATTERYDATE,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.UNITCOUNT,:new.BATTERYDATE,:new.YEARMANUFACTURED,:new.SERIALNUMBER,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.CITY,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.SHAPE,:new.MINREQLINEAMPS,:new.MAXRATEDAMPS,:new.MANUFACTURER,:new.MODELA,:new.MODELB,:new.MODELC,:new.COMMUNICATION,:new.CELLULARPROVIDER,:new.MACADDRESSA,:new.MACADDRESSB,:new.MACADDRESSC,:new.NETWORKOPSTATE,:new.SERIALNUMBERB,:new.SERIALNUMBERC,:new.BATTERYDATEB,:new.BATTERYDATEC,:new.YEARMANUFACTUREDB,:new.YEARMANUFACTUREDC,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SSDGUID,:new.INSTALLJOBNUMBER,:new.MATERIALCODE);   ELSE  IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id INTO current_state FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'; IF current_state = 0 THEN  INSERT INTO EDGIS.FAULTINDICATOR VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.ACTUATINGCURRENT,:new.FITYPE,:new.FIBATTERYDATE,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.UNITCOUNT,:new.BATTERYDATE,:new.YEARMANUFACTURED,:new.SERIALNUMBER,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.CITY,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.SHAPE,:new.MINREQLINEAMPS,:new.MAXRATEDAMPS,:new.MANUFACTURER,:new.MODELA,:new.MODELB,:new.MODELC,:new.COMMUNICATION,:new.CELLULARPROVIDER,:new.MACADDRESSA,:new.MACADDRESSB,:new.MACADDRESSC,:new.NETWORKOPSTATE,:new.SERIALNUMBERB,:new.SERIALNUMBERC,:new.BATTERYDATEB,:new.BATTERYDATEC,:new.YEARMANUFACTUREDB,:new.YEARMANUFACTUREDC,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,:new.SSDGUID,:new.INSTALLJOBNUMBER,:new.MATERIALCODE);  ELSE INSERT INTO EDGIS.A120  VALUES (next_rowid,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.ACTUATINGCURRENT,:new.FITYPE,:new.FIBATTERYDATE,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.UNITCOUNT,:new.BATTERYDATE,:new.YEARMANUFACTURED,:new.SERIALNUMBER,:new.INSTALLATIONTYPE,:new.LOCALOFFICEID,:new.REGION,:new.CITY,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.SHAPE,:new.MINREQLINEAMPS,:new.MAXRATEDAMPS,:new.MANUFACTURER,:new.MODELA,:new.MODELB,:new.MODELC,:new.COMMUNICATION,:new.CELLULARPROVIDER,:new.MACADDRESSA,:new.MACADDRESSB,:new.MACADDRESSC,:new.NETWORKOPSTATE,:new.SERIALNUMBERB,:new.SERIALNUMBERC,:new.BATTERYDATEB,:new.BATTERYDATEC,:new.YEARMANUFACTUREDB,:new.YEARMANUFACTUREDC,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SSDGUID,:new.INSTALLJOBNUMBER,:new.MATERIALCODE);  END IF; END IF;IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (120,current_state);  END IF;END;
/