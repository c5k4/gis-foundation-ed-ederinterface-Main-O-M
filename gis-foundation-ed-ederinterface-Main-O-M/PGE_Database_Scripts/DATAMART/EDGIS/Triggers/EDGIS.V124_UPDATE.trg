Prompt drop Trigger V124_UPDATE;
DROP TRIGGER EDGIS.V124_UPDATE
/

Prompt Trigger V124_UPDATE;
--
-- V124_UPDATE  (Trigger) 
--
CREATE OR REPLACE TRIGGER EDGIS.V124_UPDATE INSTEAD OF UPDATE ON EDGIS.ZZ_MV_CAPACITORBANK REFERENCING OLD as OLD
DECLARE current_state SDE.version_util.state_id_t;current_lineage INTEGER;new_state INTEGER;new_lineage INTEGER;edit_cnt INTEGER;ret INTEGER := -1; next_rowid INTEGER;BEGIN /* ArcSDE plsql */ IF :NEW.OBJECTID <> :OLD.OBJECTID THEN raise_application_error (SDE.sde_util.SE_MVV_ROWID_UPDATE, 'Cannot update multiversion view ROWID.'); END IF; IF SDE.version_util.G_edit_mode_default = FALSE THEN IF SDE.version_util.G_edit_state != SDE.version_util.C_edit_state_start Then raise_application_error (SDE.sde_util.SE_MVV_NOT_STD_EDIT_MODE,'User must call edit_version before editing the view.'); End If;current_state := SDE.version_util.current_state; SDE.version_util.current_version_writable; IF :old.sde_state_id != current_state THEN INSERT INTO EDGIS.A124 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.SWITCHTYPE,:new.SWITCHPOSITION,:new.ONLINESTATUS,:new.FILLTYPE,:new.SWITCHFILLTYPE,:new.MATERIAL,:new.TOTALKVAR,:new.UNITCOUNT,:new.UNITKVAR,:new.CTRATING,:new.FUSED,:new.SEASONOFF,:new.SERIESIMPEDIENCE,:new.SUPERVISORYCONTROL,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.AREASERVED,:new.SWITCHEDWITHVARSENSORIDC,:new.LIGHTNINGARRESTORIDC,:new.MANUFACTURER,:new.YEARMANUFACTURED,:new.SERIALNUMBER,:new.BUCKETTRUCKIDC,:new.CALTRANSPERMITIDC,:new.LATITUDE,:new.LONGITUDE,:new.INSTALLATIONTYPE,:new.MATERIALCODE,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.REGION,:new.UNITPHASE,:new.CITY,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SSDGUID); INSERT INTO EDGIS.D124 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id); ELSE UPDATE EDGIS.A124 SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,NUMBEROFPHASES = :new.NUMBEROFPHASES,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,COMMENTS = :new.COMMENTS,CEDSADEVICEID = :new.CEDSADEVICEID,LOCALOPOFFICE = :new.LOCALOPOFFICE,SOURCESIDEDEVICEID = :new.SOURCESIDEDEVICEID,COMPLEXDEVICEIDC = :new.COMPLEXDEVICEIDC,CUSTOMEROWNED = :new.CUSTOMEROWNED,SYMBOLNUMBER = :new.SYMBOLNUMBER,LOCDESC = :new.LOCDESC,COUNTY = :new.COUNTY,ZIP = :new.ZIP,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,ANIMALGUARDTYPE = :new.ANIMALGUARDTYPE,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,VAULT = :new.VAULT,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,SWITCHTYPE = :new.SWITCHTYPE,SWITCHPOSITION = :new.SWITCHPOSITION,ONLINESTATUS = :new.ONLINESTATUS,FILLTYPE = :new.FILLTYPE,SWITCHFILLTYPE = :new.SWITCHFILLTYPE,MATERIAL = :new.MATERIAL,TOTALKVAR = :new.TOTALKVAR,UNITCOUNT = :new.UNITCOUNT,UNITKVAR = :new.UNITKVAR,CTRATING = :new.CTRATING,FUSED = :new.FUSED,SEASONOFF = :new.SEASONOFF,SERIESIMPEDIENCE = :new.SERIESIMPEDIENCE,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,AREASERVED = :new.AREASERVED,SWITCHEDWITHVARSENSORIDC = :new.SWITCHEDWITHVARSENSORIDC,LIGHTNINGARRESTORIDC = :new.LIGHTNINGARRESTORIDC,MANUFACTURER = :new.MANUFACTURER,YEARMANUFACTURED = :new.YEARMANUFACTURED,SERIALNUMBER = :new.SERIALNUMBER,BUCKETTRUCKIDC = :new.BUCKETTRUCKIDC,CALTRANSPERMITIDC = :new.CALTRANSPERMITIDC,LATITUDE = :new.LATITUDE,LONGITUDE = :new.LONGITUDE,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,MATERIALCODE = :new.MATERIALCODE,LOCALOFFICEID = :new.LOCALOFFICEID,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,REGION = :new.REGION,UNITPHASE = :new.UNITPHASE,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,VERSIONNAME = :new.VERSIONNAME,SHAPE = :new.SHAPE,PROTECTIVESSD = :new.PROTECTIVESSD,AUTOPROTECTIVESSD = :new.AUTOPROTECTIVESSD,FEEDERTYPE = :new.FEEDERTYPE,SSDGUID = :new.SSDGUID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF;  ELSE IF SDE.version_util.G_default_version_set = TRUE THEN raise_application_error (SDE.sde_util.SE_MVV_SET_DEFAULT,'Call SET_DEFAULT routine before attempting to edit the DEFAULT version. '); End If;SELECT state_id, lineage_name INTO current_state, current_lineage FROM SDE.states WHERE state_id = (SELECT state_id FROM SDE.versions WHERE name = 'DEFAULT' AND owner = 'SDE'); LOCK TABLE EDGIS.d124 IN EXCLUSIVE MODE;SELECT COUNT(*) INTO edit_cnt FROM SDE.state_lineages WHERE lineage_id = current_state AND lineage_id IN (SELECT DISTINCT lineage_id FROM SDE.state_lineages WHERE lineage_name IN (SELECT lineage_name FROM SDE.state_lineages WHERE lineage_id IN (SELECT deleted_at FROM EDGIS.d124 WHERE sde_deletes_row_id = :OLD.OBJECTID AND deleted_at > current_state)));IF current_state = 0 THEN  IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; INSERT INTO EDGIS.A124 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.SWITCHTYPE,:new.SWITCHPOSITION,:new.ONLINESTATUS,:new.FILLTYPE,:new.SWITCHFILLTYPE,:new.MATERIAL,:new.TOTALKVAR,:new.UNITCOUNT,:new.UNITKVAR,:new.CTRATING,:new.FUSED,:new.SEASONOFF,:new.SERIESIMPEDIENCE,:new.SUPERVISORYCONTROL,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.AREASERVED,:new.SWITCHEDWITHVARSENSORIDC,:new.LIGHTNINGARRESTORIDC,:new.MANUFACTURER,:new.YEARMANUFACTURED,:new.SERIALNUMBER,:new.BUCKETTRUCKIDC,:new.CALTRANSPERMITIDC,:new.LATITUDE,:new.LONGITUDE,:new.INSTALLATIONTYPE,:new.MATERIALCODE,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.REGION,:new.UNITPHASE,:new.CITY,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SSDGUID); INSERT INTO EDGIS.D124 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.CAPACITORBANK SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,NUMBEROFPHASES = :new.NUMBEROFPHASES,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,COMMENTS = :new.COMMENTS,CEDSADEVICEID = :new.CEDSADEVICEID,LOCALOPOFFICE = :new.LOCALOPOFFICE,SOURCESIDEDEVICEID = :new.SOURCESIDEDEVICEID,COMPLEXDEVICEIDC = :new.COMPLEXDEVICEIDC,CUSTOMEROWNED = :new.CUSTOMEROWNED,SYMBOLNUMBER = :new.SYMBOLNUMBER,LOCDESC = :new.LOCDESC,COUNTY = :new.COUNTY,ZIP = :new.ZIP,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,ANIMALGUARDTYPE = :new.ANIMALGUARDTYPE,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,VAULT = :new.VAULT,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,SWITCHTYPE = :new.SWITCHTYPE,SWITCHPOSITION = :new.SWITCHPOSITION,ONLINESTATUS = :new.ONLINESTATUS,FILLTYPE = :new.FILLTYPE,SWITCHFILLTYPE = :new.SWITCHFILLTYPE,MATERIAL = :new.MATERIAL,TOTALKVAR = :new.TOTALKVAR,UNITCOUNT = :new.UNITCOUNT,UNITKVAR = :new.UNITKVAR,CTRATING = :new.CTRATING,FUSED = :new.FUSED,SEASONOFF = :new.SEASONOFF,SERIESIMPEDIENCE = :new.SERIESIMPEDIENCE,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,AREASERVED = :new.AREASERVED,SWITCHEDWITHVARSENSORIDC = :new.SWITCHEDWITHVARSENSORIDC,LIGHTNINGARRESTORIDC = :new.LIGHTNINGARRESTORIDC,MANUFACTURER = :new.MANUFACTURER,YEARMANUFACTURED = :new.YEARMANUFACTURED,SERIALNUMBER = :new.SERIALNUMBER,BUCKETTRUCKIDC = :new.BUCKETTRUCKIDC,CALTRANSPERMITIDC = :new.CALTRANSPERMITIDC,LATITUDE = :new.LATITUDE,LONGITUDE = :new.LONGITUDE,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,MATERIALCODE = :new.MATERIALCODE,LOCALOFFICEID = :new.LOCALOFFICEID,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,REGION = :new.REGION,UNITPHASE = :new.UNITPHASE,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,VERSIONNAME = :new.VERSIONNAME,SHAPE = :new.SHAPE,PROTECTIVESSD = :new.PROTECTIVESSD,AUTOPROTECTIVESSD = :new.AUTOPROTECTIVESSD,FEEDERTYPE = :new.FEEDERTYPE,SSDGUID = :new.SSDGUID WHERE OBJECTID = :old.OBJECTID;END IF;  ELSE IF :old.sde_state_id != current_state THEN IF edit_cnt > 0 THEN ret := SDE.version_user_ddl.new_branch_state (current_state,current_lineage,new_state);IF ret != SDE.sde_util.SE_SUCCESS Then raise_application_error (SDE.sde_util.SE_VERSION_HAS_MOVED, 'The DEFAULT version continues to be modified, commit, rollback or re-execute the last statement to proceed.'); End IF; current_state := new_state; END IF; INSERT INTO EDGIS.A124 VALUES (:old.OBJECTID,:new.ANCILLARYROLE,:new.ENABLED,:new.GLOBALID,:new.CREATIONUSER,:new.DATECREATED,:new.DATEMODIFIED,:new.LASTUSER,:new.CONVERSIONID,:new.CONVERSIONWORKPACKAGE,:new.STATUS,:new.ELECTRICTRACEWEIGHT,:new.CIRCUITID,:new.CIRCUITID2,:new.CONVCIRCUITID,:new.CONVCIRCUITID2,:new.FEEDERINFO,:new.INSTALLATIONDATE,:new.RETIREDATE,:new.NUMBEROFPHASES,:new.PHASEDESIGNATION,:new.PHASINGVERIFIEDSTATUS,:new.SYMBOLROTATION,:new.INSTALLJOBPREFIX,:new.INSTALLJOBYEAR,:new.COMMENTS,:new.CEDSADEVICEID,:new.LOCALOPOFFICE,:new.SOURCESIDEDEVICEID,:new.COMPLEXDEVICEIDC,:new.CUSTOMEROWNED,:new.SYMBOLNUMBER,:new.LOCDESC,:new.COUNTY,:new.ZIP,:new.GEMSDISTMAPNUM,:new.GEMSCIRCUITMAPNUM,:new.GEMSOTHERMAPNUM,:new.ANIMALGUARDTYPE,:new.DIVISION,:new.DISTRICT,:new.VAULT,:new.REPLACEGUID,:new.SUBTYPECD,:new.OPERATINGVOLTAGE,:new.SWITCHTYPE,:new.SWITCHPOSITION,:new.ONLINESTATUS,:new.FILLTYPE,:new.SWITCHFILLTYPE,:new.MATERIAL,:new.TOTALKVAR,:new.UNITCOUNT,:new.UNITKVAR,:new.CTRATING,:new.FUSED,:new.SEASONOFF,:new.SERIESIMPEDIENCE,:new.SUPERVISORYCONTROL,:new.SAPEQUIPID,:new.TEMPEQUIPID,:new.STRUCTUREGUID,:new.STRUCTURECONVID,:new.AREASERVED,:new.SWITCHEDWITHVARSENSORIDC,:new.LIGHTNINGARRESTORIDC,:new.MANUFACTURER,:new.YEARMANUFACTURED,:new.SERIALNUMBER,:new.BUCKETTRUCKIDC,:new.CALTRANSPERMITIDC,:new.LATITUDE,:new.LONGITUDE,:new.INSTALLATIONTYPE,:new.MATERIALCODE,:new.LOCALOFFICEID,:new.INSTALLJOBNUMBER,:new.REGION,:new.UNITPHASE,:new.CITY,:new.OPERATINGNUMBER,:new.VERSIONNAME,:new.SHAPE,:new.PROTECTIVESSD,:new.AUTOPROTECTIVESSD,:new.FEEDERTYPE,current_state,:new.SSDGUID); INSERT INTO EDGIS.D124 (DELETED_AT,SDE_DELETES_ROW_ID,SDE_STATE_ID) VALUES (current_state, :old.OBJECTID,:old.sde_state_id);  ELSE UPDATE EDGIS.A124 SET ANCILLARYROLE = :new.ANCILLARYROLE,ENABLED = :new.ENABLED,GLOBALID = :new.GLOBALID,CREATIONUSER = :new.CREATIONUSER,DATECREATED = :new.DATECREATED,DATEMODIFIED = :new.DATEMODIFIED,LASTUSER = :new.LASTUSER,CONVERSIONID = :new.CONVERSIONID,CONVERSIONWORKPACKAGE = :new.CONVERSIONWORKPACKAGE,STATUS = :new.STATUS,ELECTRICTRACEWEIGHT = :new.ELECTRICTRACEWEIGHT,CIRCUITID = :new.CIRCUITID,CIRCUITID2 = :new.CIRCUITID2,CONVCIRCUITID = :new.CONVCIRCUITID,CONVCIRCUITID2 = :new.CONVCIRCUITID2,FEEDERINFO = :new.FEEDERINFO,INSTALLATIONDATE = :new.INSTALLATIONDATE,RETIREDATE = :new.RETIREDATE,NUMBEROFPHASES = :new.NUMBEROFPHASES,PHASEDESIGNATION = :new.PHASEDESIGNATION,PHASINGVERIFIEDSTATUS = :new.PHASINGVERIFIEDSTATUS,SYMBOLROTATION = :new.SYMBOLROTATION,INSTALLJOBPREFIX = :new.INSTALLJOBPREFIX,INSTALLJOBYEAR = :new.INSTALLJOBYEAR,COMMENTS = :new.COMMENTS,CEDSADEVICEID = :new.CEDSADEVICEID,LOCALOPOFFICE = :new.LOCALOPOFFICE,SOURCESIDEDEVICEID = :new.SOURCESIDEDEVICEID,COMPLEXDEVICEIDC = :new.COMPLEXDEVICEIDC,CUSTOMEROWNED = :new.CUSTOMEROWNED,SYMBOLNUMBER = :new.SYMBOLNUMBER,LOCDESC = :new.LOCDESC,COUNTY = :new.COUNTY,ZIP = :new.ZIP,GEMSDISTMAPNUM = :new.GEMSDISTMAPNUM,GEMSCIRCUITMAPNUM = :new.GEMSCIRCUITMAPNUM,GEMSOTHERMAPNUM = :new.GEMSOTHERMAPNUM,ANIMALGUARDTYPE = :new.ANIMALGUARDTYPE,DIVISION = :new.DIVISION,DISTRICT = :new.DISTRICT,VAULT = :new.VAULT,REPLACEGUID = :new.REPLACEGUID,SUBTYPECD = :new.SUBTYPECD,OPERATINGVOLTAGE = :new.OPERATINGVOLTAGE,SWITCHTYPE = :new.SWITCHTYPE,SWITCHPOSITION = :new.SWITCHPOSITION,ONLINESTATUS = :new.ONLINESTATUS,FILLTYPE = :new.FILLTYPE,SWITCHFILLTYPE = :new.SWITCHFILLTYPE,MATERIAL = :new.MATERIAL,TOTALKVAR = :new.TOTALKVAR,UNITCOUNT = :new.UNITCOUNT,UNITKVAR = :new.UNITKVAR,CTRATING = :new.CTRATING,FUSED = :new.FUSED,SEASONOFF = :new.SEASONOFF,SERIESIMPEDIENCE = :new.SERIESIMPEDIENCE,SUPERVISORYCONTROL = :new.SUPERVISORYCONTROL,SAPEQUIPID = :new.SAPEQUIPID,TEMPEQUIPID = :new.TEMPEQUIPID,STRUCTUREGUID = :new.STRUCTUREGUID,STRUCTURECONVID = :new.STRUCTURECONVID,AREASERVED = :new.AREASERVED,SWITCHEDWITHVARSENSORIDC = :new.SWITCHEDWITHVARSENSORIDC,LIGHTNINGARRESTORIDC = :new.LIGHTNINGARRESTORIDC,MANUFACTURER = :new.MANUFACTURER,YEARMANUFACTURED = :new.YEARMANUFACTURED,SERIALNUMBER = :new.SERIALNUMBER,BUCKETTRUCKIDC = :new.BUCKETTRUCKIDC,CALTRANSPERMITIDC = :new.CALTRANSPERMITIDC,LATITUDE = :new.LATITUDE,LONGITUDE = :new.LONGITUDE,INSTALLATIONTYPE = :new.INSTALLATIONTYPE,MATERIALCODE = :new.MATERIALCODE,LOCALOFFICEID = :new.LOCALOFFICEID,INSTALLJOBNUMBER = :new.INSTALLJOBNUMBER,REGION = :new.REGION,UNITPHASE = :new.UNITPHASE,CITY = :new.CITY,OPERATINGNUMBER = :new.OPERATINGNUMBER,VERSIONNAME = :new.VERSIONNAME,SHAPE = :new.SHAPE,PROTECTIVESSD = :new.PROTECTIVESSD,AUTOPROTECTIVESSD = :new.AUTOPROTECTIVESSD,FEEDERTYPE = :new.FEEDERTYPE,SSDGUID = :new.SSDGUID WHERE OBJECTID = :old.OBJECTID AND sde_state_id = :old.sde_state_id; END IF; END IF;  END IF; IF current_state != 0 THEN  SDE.version_util.flag_mvtable_modified (124,current_state);  END IF; END;
/